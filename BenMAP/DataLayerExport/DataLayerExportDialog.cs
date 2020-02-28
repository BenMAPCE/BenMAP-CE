using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        private List<Tuple<string, IMapFeatureLayer>> _allLayers;
        private Dictionary<Tuple<string, string, int>, Tuple<int, CheckState>> _dictAllLayers = new Dictionary<Tuple<string, string, int>, Tuple<int, CheckState>>();

        public DataLayerExportDialog(DataLayerExporter exporter, IEnumerable<IMapFeatureLayer> layers, IEnumerable columns, params IMapFeatureLayer[] layersToSelect)
        {
            if (exporter == null) throw new ArgumentNullException("exporter");          //BenMAP-400: Previously BenMAP could only export the GIS layer for the most recently loaded results. It relied on the column headers of the "Data" tab
            if (layers == null) throw new ArgumentNullException("layers");              //Now, when the DataLayerExportDialog form loads, it takes data for all layers to populate the checked listboxes for layers/variables in user selection 

            _exporter = exporter;

            InitializeComponent();

            _allLayers = layers.
                Select(
                delegate (IMapFeatureLayer _)
                {
                    var parent = _.GetParentItem();
                    return new Tuple<string, IMapFeatureLayer>(
                        (parent != null ? parent.LegendText + "\\" : "") + _.LegendText,
                        _)
                 ;
                }).ToList();

            int columnCount = 1;
            foreach (List<string> list in columns)          //BenMAP-400: If the list of strings for each listchecked box is empty--BenMAP shows it as inactive, otherwise it populates with export options
            {                                               //There are no export options for AQ (only D24 and Quarterly mean)
                if (columnCount == 1) //HIF
                {
                    if (list.Count() != 0)
                    {
                        chlbColumns_HIF.DataSource = list;
                        chlbColumns_HIF.Enabled = true;
                    }
                    else
                    {
                        chlbColumns_HIF.Enabled = false;
                        chlbColumns_HIF.BackColor = System.Drawing.Color.LightGray;
                    }
                }
                if (columnCount == 2) //Pooling
                {
                    if (list.Count() != 0)
                    {
                        chlbColumns_Pool.DataSource = list;
                        chlbColumns_Pool.Enabled = true;
                    }
                    else
                    {
                        chlbColumns_Pool.Enabled = false;
                        chlbColumns_Pool.BackColor = System.Drawing.Color.LightGray;
                    }
                }
                if (columnCount == 3) //Valuation
                {
                    if (list.Count() != 0)
                    {
                        chlbColumns_Valuation.DataSource = list;
                        chlbColumns_Valuation.Enabled = true;
                    }
                    else
                    {
                        chlbColumns_Valuation.Enabled = false;
                        chlbColumns_Valuation.BackColor = System.Drawing.Color.LightGray;
                    }
                }
                columnCount += 1;
            }

            int layerCount = 0;
            int aqCount = 0;
            int hifCount = 0;
            int valuationCount = 0;
            int poolingCount = 0;

            foreach (Tuple<string, IMapFeatureLayer> tuple in _allLayers)               //BenMAP 400: This code creates a dictionary showing where the layer is located in the original "all layers" list and where it is located in the 
            {                                                                           //respective layer-specific box (& whether or not it is checked).
                string currInfo = tuple.Item1;
                int charIndex = currInfo.IndexOf("\\", StringComparison.Ordinal);
                string layerType = currInfo.Substring(0, charIndex);
                string layerInfo = currInfo.Substring(charIndex + 1);

                if (layerInfo == "Baseline" || layerInfo == "Control" || layerInfo == "Delta")
                {
                    string fullName = ((DotSpatial.Symbology.FeatureLayer)tuple.Item2).Name;
                    int pollIndex = fullName.IndexOf("_", StringComparison.Ordinal);
                    string pollutantName = fullName.Substring(0, pollIndex);

                    layerInfo = pollutantName + ": " + layerInfo;

                    var findLayer = _dictAllLayers.Where(x => x.Key.Item2 == layerInfo);

                    if (findLayer.Count() == 0)
                        _dictAllLayers.Add(new Tuple<string, string, int>("Air Quality", layerInfo, aqCount), new Tuple<int, CheckState>(layerCount, CheckState.Unchecked));
                    aqCount += 1;
                }
                else if (layerType == "Health Impacts")
                {
                    _dictAllLayers.Add(new Tuple<string, string, int>(layerType, layerInfo, hifCount), new Tuple<int, CheckState>(layerCount, CheckState.Unchecked));
                    hifCount += 1;
                }
                else if (layerType == "Pooled Incidence")
                {
                    _dictAllLayers.Add(new Tuple<string, string, int>(layerType, layerInfo, valuationCount), new Tuple<int, CheckState>(layerCount, CheckState.Unchecked));
                    valuationCount += 1;
                }
                else if (layerType == "Pooled Valuation")
                {
                    _dictAllLayers.Add(new Tuple<string, string, int>(layerType, layerInfo, poolingCount), new Tuple<int, CheckState>(layerCount, CheckState.Unchecked));
                    poolingCount += 1;
                }
                else
                    MessageBox.Show("Error Loading Export Variables");

                layerCount += 1;
            }

            for (var i = 0; i < chlbColumns_HIF.Items.Count; i++)
            {
                chlbColumns_HIF.SetItemChecked(i, true);
            }
            for (var i = 0; i < chlbColumns_Pool.Items.Count; i++)
            {
                chlbColumns_Pool.SetItemChecked(i, true);
            }
            for (var i = 0; i < chlbColumns_Valuation.Items.Count; i++)
            {
                chlbColumns_Valuation.SetItemChecked(i, true);
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
            var toExport = _dictAllLayers.Where(x => x.Value.Item2 == CheckState.Checked);
            var layers = new List<IMapFeatureLayer>();
            var columns = new List<List<string>>();
            var fileNames = new List<string>();
            
            if (toExport.Count() == 0)
            {
                MessageBox.Show(this, "Select at least one layer to export.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (KeyValuePair<Tuple<string, string, int>, Tuple<int, CheckState>> entry in toExport)
            {
                int layerLocation = entry.Value.Item1;
                string layerType = entry.Key.Item1;

                IMapFeatureLayer layerExport = _allLayers[layerLocation].Item2;
                var colExport = new List<string>();

                switch (layerType)
                {
                    case "Air Quality":         //No Column Options for AQ
                        break;
                    case "Health Impacts":
                        foreach (string checkedItem in chlbColumns_HIF.CheckedItems)
                        {
                            colExport.Add(checkedItem);
                        }
                        break;
                    case "Pooled Incidence":
                        foreach (string checkedItem in chlbColumns_Pool.CheckedItems)
                        {
                            colExport.Add(checkedItem);
                        }
                        break;
                    case "Pooled Valuation":
                        foreach (string checkedItem in chlbColumns_Valuation.CheckedItems)
                        {
                            colExport.Add(checkedItem);
                        }
                        break;
                    default:
                        break;
                }

                if (layerType != "Air Quality" && colExport.Count == 0)
                {
                    MessageBox.Show(this, "Select at least one column to export.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var fileName = Regex.Replace(layerType + "-" + layerExport.LegendText, "[;\\/:*?\"<>|&',]", "");
                layers.Add(layerExport);
                columns.Add(colExport);
                fileNames.Add(fileName);
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
                _exporter.Export(layers, columns, exportFolder, fileNames);                //After assembling the selections made for variables to export (in columns), pass all layers and selections to the export function.
            }).ContinueWith(delegate (Task t)
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
        private void cboLayerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!chlbLayers.Enabled)
            {
                chlbLayers.Enabled = true;
                chlbLayers.BackColor = System.Drawing.Color.White;
            }

            chlbLayers.Items.Clear();

            var outLayers = _dictAllLayers.Where(x => x.Key.Item1 == cboLayerType.SelectedItem.ToString());

            foreach (KeyValuePair<Tuple<string, string, int>, Tuple<int, CheckState>> entry in outLayers)
            {
                chlbLayers.Items.Add(entry.Key.Item2, entry.Value.Item2);
            }
        }
        #endregion

        private void chlbLayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckState newState;
            if (e.CurrentValue == CheckState.Checked)           //Checked state not updated until after ItemCheck event completes.
                newState = CheckState.Unchecked;
            else if (e.CurrentValue == CheckState.Unchecked)
                newState = CheckState.Checked;
            else
                return;

            if (chlbLayers.SelectedItem != null)                //SelectedItem is null when changing options in cboLayerType (which sets check state for each item in dict)
            {
                string layerType = cboLayerType.Text;
                string itemCheck = chlbLayers.SelectedItem.ToString();
                int itemIndex = chlbLayers.SelectedIndex;

                Tuple<string, string, int> key = new Tuple<string, string, int>(layerType, itemCheck, itemIndex);
                Tuple<int, CheckState> dicVal = new Tuple<int, CheckState>(0, CheckState.Indeterminate);
                if (_dictAllLayers.TryGetValue(key, out dicVal))
                {
                    int layerLocation = dicVal.Item1;
                    _dictAllLayers[key] = new Tuple<int, CheckState>(layerLocation, newState);
                }
            }
        }

        private void DataLayerExportDialog_Load(object sender, EventArgs e)
        {
            cboLayerType.SelectedItem = null;
            cboLayerType.SelectedText = "--Select--";
            chlbLayers.Enabled = false;
            chlbLayers.BackColor = System.Drawing.Color.LightGray;
        }
    }
}
