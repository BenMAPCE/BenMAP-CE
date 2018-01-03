using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotSpatial.Controls;
using GeoAPI.Geometries;

// This form is used to select by location
// MM/DPA 8/26/2016
namespace BenMAP.SelectByLocation
{
    public partial class SelectByLocationDialog : Form
    {
        private bool _isBusy;

        public SelectByLocationDialog(Map map)
        {
            if (map == null) throw new ArgumentNullException("map");

            InitializeComponent();

            // Initialize comboboxes
            cmbSelectionMethod.DataSource = EnumHelper.ToEnumEntities(typeof (SelectionMethod)).ToList();
            cmbSelectionMethod.ValueMember = "Value";
            cmbSelectionMethod.DisplayMember = "Name";

            cmbSpatialSelectionMethod.DataSource = EnumHelper.ToEnumEntities(typeof (SpatialSelectionMethod)).ToList();
            cmbSpatialSelectionMethod.ValueMember = "Value";
            cmbSpatialSelectionMethod.DisplayMember = "Name";

            cmbSelectionLayer.DataSource = map.GetAllLayers().OfType<IMapPolygonLayer>().Select(
                delegate(IMapPolygonLayer _)
                {
                    var parent = _.GetParentItem();
                    return new NamedEntity
                    {
                        Name = (parent != null? parent.LegendText + "\\" : "") + _.LegendText,
                        Value = _
                    };
                }).ToList();
            cmbSelectionLayer.DisplayMember = "Name";
            cmbSelectionLayer.ValueMember = "Value";
            cmbSelectionLayer.SelectedItem = null;

            cmbTargetLayer.DataSource = map.GetAllLayers().OfType<IMapFeatureLayer>().Select(
                delegate(IMapFeatureLayer _)
                {
                    var parent = _.GetParentItem();
                    return new NamedEntity
                    {
                        Name = (parent != null ? parent.LegendText + "\\" : "") + _.LegendText,
                        Value = _
                    };
                }).ToList();
            cmbTargetLayer.DisplayMember = "Name";
            cmbTargetLayer.ValueMember = "Value";
            cmbTargetLayer.SelectedItem = null;

            SetBusy(false);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            DoSelectByLocation(false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DoSelectByLocation(true);
        }

        private void DoSelectByLocation(bool close)
        {
            var selectionMethod = (SelectionMethod) cmbSelectionMethod.SelectedValue;
            var spatialSelectionMethod = (SpatialSelectionMethod)cmbSpatialSelectionMethod.SelectedValue;
            var selectionLayer = (IMapPolygonLayer)cmbSelectionLayer.SelectedValue;
            var targetLayer = (IMapFeatureLayer)cmbTargetLayer.SelectedValue;

            if (selectionLayer == null)
            {
                MessageBox.Show(this, "Select 'Selection layer'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (targetLayer == null)
            {
                MessageBox.Show(this, "Select 'Target layer'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetBusy(true);

            Envelope area;
            targetLayer.ClearSelection(out area,true);
            Task.Factory.StartNew(delegate
            {
                var worker = new SelectByLocationWorker(selectionLayer, targetLayer, selectionMethod, spatialSelectionMethod);
                worker.OnSelecting += delegate(object sender, OnSelectingEventArgs args)
                {
                    Invoke((Action) delegate
                    {
                        Text = string.Format("Select By Location: {0} of {1} done.", args.Current + 1, args.Total);
                    });
                };
                return worker.Select();
            }).ContinueWith(delegate(Task<List<int>> task)
            {
                targetLayer.Select(task.Result);
                selectionLayer.ClearSelection(out area,true);

                SetBusy(false);
                if (close)
                {
                    Close();
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        private void SetBusy(bool busy)
        {
            pbProgress.Visible = busy;
            paMain.Enabled = !busy;
            Text = "Select By Location";
            _isBusy = busy;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SelectByLocationDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = _isBusy;
        }
    }
}
