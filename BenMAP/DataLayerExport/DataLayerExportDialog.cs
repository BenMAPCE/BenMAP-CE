using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using DotSpatial.Controls;

namespace BenMAP.DataLayerExport
{
    public partial class DataLayerExportDialog : Form
    {
        private readonly DataLayerExporter _exporter;
        private bool _isBusy;

        public DataLayerExportDialog(DataLayerExporter exporter, IEnumerable<IMapFeatureLayer> layers, IEnumerable columns, params IMapFeatureLayer[] layersToSelect)
        {
            if (exporter == null) throw new ArgumentNullException("exporter");
            if (layers == null) throw new ArgumentNullException("layers");

            _exporter = exporter;

            InitializeComponent();

            chlbLayers.DataSource = layers
                .Select(
                delegate(IMapFeatureLayer _)
                {
                    var parent = _.GetParentItem();
                    return new Tuple<string, IMapFeatureLayer>(
                        (parent != null ? parent.LegendText + "\\" : "") + _.LegendText,
                        _)
                 ;
                }).ToList();
            chlbLayers.DisplayMember = "Item1";
            chlbLayers.ValueMember = "Item2";

            if (layersToSelect != null)
            {
                for (var i = 0; i < chlbLayers.Items.Count; i++)
                {
                    var item = (Tuple<string, IMapFeatureLayer>)chlbLayers.Items[i];
                    foreach (var mapFeatureLayer in layersToSelect)
                    {
                        if (item.Item2 == mapFeatureLayer)
                        {
                            chlbLayers.SetItemChecked(i, true);
                        }
                    }
                }
            }

            chlbColumns.DataSource = columns;
            chlbColumns.DisplayMember = "Text";
            for (var i = 0; i < chlbColumns.Items.Count; i++)
            {
                chlbColumns.SetItemChecked(i, true);
            }

            tbExportFolder.Text = CommonClass.ResultFilePath + @"\Export";

            SetBusy(false);
        }

        #region Fields

        private void SetBusy(bool busy)
        {
            pbProgress.Visible = busy;
            lblProgressInfo.Visible = busy;
            paMain.Enabled = !busy;
            _isBusy = busy;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DataLayerExportDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = _isBusy;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DoSave();
        }

        private void DoSave()
        {
            var layersToExport = new List<IMapFeatureLayer>(chlbLayers.CheckedItems.Count);
            foreach (Tuple<string, IMapFeatureLayer> tp in chlbLayers.CheckedItems)
            {
                 layersToExport.Add(tp.Item2);   
            }
            if (layersToExport.Count == 0)
            {
                MessageBox.Show(this, "Select at least one layer to export.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var columnsToExport = new List<OLVColumn>(chlbColumns.CheckedItems.Count);
            foreach (OLVColumn tp in chlbColumns.CheckedItems)
            {
                columnsToExport.Add(tp);
            }
            if (columnsToExport.Count == 0)
            {
                MessageBox.Show(this, "Select at least one column to export.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var exportFolder = tbExportFolder.Text;
            if (string.IsNullOrWhiteSpace(exportFolder))
            {
                MessageBox.Show(this, "Select folder to export.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SetBusy(true);
            
            Task.Factory.StartNew(delegate
            {
                _exporter.ExportProgress += ExporterOnExportProgress;
                _exporter.Export(layersToExport, columnsToExport, exportFolder);
            }).ContinueWith(delegate(Task t)
            {
                SetBusy(false);
                _exporter.ExportProgress -= ExporterOnExportProgress;

                if (t.Exception != null)
                {
                    MessageBox.Show("Error: " + t.Exception.GetBaseException(), "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                else
                {
                    Close();
                }
                
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        private void ExporterOnExportProgress(object sender, ExportProgressEventArgs args)
        {
            Invoke((Action)delegate
            {
                lblProgressInfo.Text = string.Format("{0}% {1}", args.Percentage, args.Message);
            });
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var dr = new FolderBrowserDialog())
            {
                dr.SelectedPath = tbExportFolder.Text;
                if (dr.ShowDialog(this) == DialogResult.OK)
                {
                    tbExportFolder.Text = dr.SelectedPath;
                }
            }
        }

        #endregion
    }
}
