using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using DotSpatial.Controls;
using DotSpatial.Symbology;

namespace BenMAP.DataLayerExport
{
    public class DataLayerExporter
    {
        #region Fields

        private const string MENU_ITEM_DATA = "Data";
        private const string MENU_ITEM_EXPORT_DATA = "Export Data";
        private const string MENU_ITEM_EXPORT_LAYER = "Export Layer";
        private const string MENU_ITEM_EXPORT_RESULTS = "Export Layer with Results Data";
        private const string RESULTS_GRID_COLUMN = "Column";
        private const string RESULTS_GRID_ROW = "Row";
        private const string LAYER_GRID_COLUMN = "COL";
        private const string LAYER_GRID_ROW = "ROW";
        private const string RESULTS_GROUP = "Results";


        private readonly Map _map;
        private readonly IWin32Window _windowOwner;
        private readonly ObjectListView _oblw;
        private readonly Func<IEnumerable> _tableObject;
        private bool _windowShown;

        public event EventHandler<ExportProgressEventArgs> ExportProgress;


        #endregion

        #region Ctor

        public DataLayerExporter(Map map, IWin32Window windowOwner, ObjectListView oblw, Func<IEnumerable> tableObject)
        {
            if (map == null) throw new ArgumentNullException("map");

            _map = map;
            _windowOwner = windowOwner;
            _oblw = oblw;
            _tableObject = tableObject;

            map.LayerAdded +=  (s, e) => OnMapLayerAdded(e.Layer);
        }

        #endregion

        #region Public methods

        internal void Export(List<IMapFeatureLayer> layersToExport, List<OLVColumn> columnsToExport, string exportFolder)
        {
            if (layersToExport == null) throw new ArgumentNullException("layersToExport");
            if (columnsToExport == null) throw new ArgumentNullException("columnsToExport");
            if (exportFolder == null) throw new ArgumentNullException("exportFolder");

            var currentProgress = 0.0;
            ReportProgress(currentProgress, "Preparing data");

            if (!Directory.Exists(exportFolder))
            {
                Directory.CreateDirectory(exportFolder);
            }

            // Prepare objects dictionary
            OLVColumn results_column = null;
            OLVColumn results_row = null;

            foreach (OLVColumn column in _oblw.Columns)
            {
                if (column.Text == RESULTS_GRID_COLUMN)
                    results_column = column;
                if (column.Text == RESULTS_GRID_ROW)
                    results_row = column;
                if (results_column != null && results_row != null)
                    break;
            }

            if (results_column == null)
            {
                throw new Exception("results_column not found");
            }
            if (results_row == null)
            {
                throw new Exception("results_row not found");
            }

            var objDictionary = new Dictionary<KeyValuePair<int, int>, object>();
            foreach (var obj in _tableObject())
            {
                var res_col = Convert.ToInt32(results_column.GetAspectByName(obj));
                var res_row = Convert.ToInt32(results_row.GetAspectByName(obj));
                var key = new KeyValuePair<int, int>(res_col, res_row);
                objDictionary[key] = obj;
            }

            // Remove RESULTS_GRID_COLUMN\RESULTS_GRID_ROW from export columns
            columnsToExport = columnsToExport.Where(_ => _.Text != RESULTS_GRID_COLUMN &&
                                                         _.Text != RESULTS_GRID_ROW).ToList();

            currentProgress = 10;
            var percentageToExportLayer = (100.0 - currentProgress) / layersToExport.Count;
            var percentageToBuildLayer = percentageToExportLayer * 0.8;

            var layerNames = new HashSet<string>();
            foreach (var mapFeatureLayer in layersToExport)
            {
                var sourceDataSet = mapFeatureLayer.DataSet;
                
                var fileName = mapFeatureLayer.LegendText;

                // Make unique filename
                int fileNameCounter = 2;
                while (!layerNames.Add(fileName))
                {
                    fileName = fileName + "_" + fileNameCounter;
                    fileNameCounter++;
                }

                var expDataset = sourceDataSet.CopyFeatures(true);

                // Add columns to data set
                foreach (var olvColumn in columnsToExport)
                {
                    expDataset.DataTable.Columns.Add(olvColumn.Text, olvColumn.DataType);
                }
                 
                // Add data to features
                var feature_col_idx = expDataset.DataTable.Columns.IndexOf(expDataset.DataTable.Columns[LAYER_GRID_COLUMN]);
                var feature_row_idx = expDataset.DataTable.Columns.IndexOf(expDataset.DataTable.Columns[LAYER_GRID_ROW]);

                if (feature_row_idx == -1)
                {
                    throw new Exception("feature_row_idx not found");
                }
                if (feature_col_idx == -1)
                {
                    throw new Exception("feature_col_idx not found");
                }
                
                expDataset.DataTable.BeginLoadData();  // this a bit improve speed
                for (var i = 0; i < expDataset.DataTable.Rows.Count; i++)
                {
                    if (i%1000 == 0)
                    {
                        ReportProgress(currentProgress + percentageToBuildLayer * i / expDataset.DataTable.Rows.Count, string.Format("Building: {0}", fileName));
                    }

                    var row = expDataset.DataTable.Rows[i];
                    var feature_col = Convert.ToInt32(row[feature_col_idx]);
                    var feature_row = Convert.ToInt32(row[feature_row_idx]);
                    var key = new KeyValuePair<int, int>(feature_col, feature_row);

                    object results_row_object;
                    if (!objDictionary.TryGetValue(key, out results_row_object)) continue;

                    // Set data rows in feature
                    if (results_row_object != null)
                    {
                        foreach (var olvColumn in columnsToExport)
                        {
                            row[olvColumn.Text] = olvColumn.GetAspectByName(results_row_object);
                        }
                    } 
                }
                expDataset.DataTable.EndLoadData();
                
                // Save
                ReportProgress(currentProgress + percentageToBuildLayer, string.Format("Writing to disk: {0}", fileName));
                expDataset.SaveAs(Path.Combine(exportFolder, fileName + ".shp"), true);

                currentProgress += percentageToExportLayer;
            }
        }

        public void ShowExportWindow(IMapFeatureLayer layer = null)
        {
            if (_windowShown) return;

            var col_column = false;
            var col_row = false;
            foreach (OLVColumn column in _oblw.Columns)
            {
                if (column.Text == RESULTS_GRID_COLUMN)
                    col_column = true;
                if (column.Text == RESULTS_GRID_ROW)
                    col_row = true;
                if (col_column && col_row)
                    break;
            }

            if (!(col_column && col_row))
            {
                MessageBox.Show(_windowOwner, string.Format("Results table must include '{0}' and '{1}' columns.", RESULTS_GRID_COLUMN, RESULTS_GRID_ROW), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var layers = _map.GetAllLayers().OfType<IMapPolygonLayer>()
                .Where(_ =>
                            HasParentGroup(_, RESULTS_GROUP) &&
                            _.DataSet.GetColumn(LAYER_GRID_COLUMN) != null &&
                            _.DataSet.GetColumn(LAYER_GRID_ROW) != null);

            using (var window = new DataLayerExportDialog(this, layers, _oblw.Columns, layer))
            {
                _windowShown = true;
                window.ShowDialog(_windowOwner);
                _windowShown = false;
            }
        }

        #endregion

        #region Private methods

        private static bool HasParentGroup(ILegendItem item, string groupName)
        {
            while (true)
            {
                var parent = item.GetParentItem();
                if (parent == null) return false;
                if (parent is MapGroup && parent.LegendText == groupName)
                    return true;
                item = parent;
            }
        }

        private void ReportProgress(double percentage, string message)
        {
            var h = ExportProgress;
            if (h != null)
            {
                h(this, new ExportProgressEventArgs((int)percentage, message));    
            }
        }

        private void OnMapLayerAdded(ILegendItem layer)
        {
            if (layer == null) return;

            IMapGroup grp = layer as IMapGroup;
            if (grp != null)
            {
                // map.layerAdded event doesn't fire for groups. Therefore, it's necessary
                // to handle this event separately for groups.
                grp.LayerAdded += delegate(object sender, LayerEventArgs args)
                {
                    OnMapLayerAdded(args.Layer);
                };
            }

            // Search "Data" menu item
            var menuItemData = layer.ContextMenuItems.FirstOrDefault(_ => _.Name == MENU_ITEM_DATA);
            if ((menuItemData == null) || !(layer is IMapFeatureLayer)) return;

            // Rename "Export Data" menu item
            var menuItemExpData = menuItemData.MenuItems.FirstOrDefault(_ => _.Name == MENU_ITEM_EXPORT_DATA);
            if (menuItemExpData != null)
            {
                menuItemExpData.Name = MENU_ITEM_EXPORT_LAYER;
            }

            if (menuItemData.MenuItems.All(_ => _.Name != MENU_ITEM_EXPORT_RESULTS)) // Check menu item not added yet
            {
                menuItemData.MenuItems.Add(new SymbologyMenuItem(MENU_ITEM_EXPORT_RESULTS,
                    delegate
                    {
                        ShowExportWindow((IMapFeatureLayer) layer);
                    }));
            }
        }

        #endregion
    }

    public class ExportProgressEventArgs : EventArgs
    {
        public ExportProgressEventArgs(int percentage, string message)
        {
            Percentage = percentage;
            Message = message;
        }

        public int Percentage { get; private set; }
        public string Message { get; private set; }
    }
}
