using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using DotSpatial.Controls;
using DotSpatial.Data;
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
        private const string POLLUTANTS_GROUP = "Pollutants";



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

            map.LayerAdded += (s, e) => OnMapLayerAdded(e.Layer);
        }

        #endregion

        #region Public methods

        internal void Export(List<IMapFeatureLayer> layersToExport, List<List<string>> columnsToExport, string exportFolder, List<string> fileNames)
        {
            if (layersToExport == null) throw new ArgumentNullException("layerToExport");
            if (columnsToExport == null) throw new ArgumentNullException("columnsToExport");
            if (exportFolder == null) throw new ArgumentNullException("exportFolder");

            var currentProgress = 0.0;
            ReportProgress(currentProgress, "Preparing data");

            if (!Directory.Exists(exportFolder))
            {
                Directory.CreateDirectory(exportFolder);
            }

            var layerNames = new HashSet<string>();
            var layerCount = 1;

            foreach (var mapFeatureLayer in layersToExport)
            {

                var adminLayers = _map.GetAllLayers().OfType<IMapPolygonLayer>()
               .Where(_layer =>
                           (HasParentGroup(_layer, "Region Admin Layers")));

                var sourceDataSet = mapFeatureLayer.DataSet;

                var fileName = fileNames[layerCount-1] + "_" + DateTime.Now.ToString("yyyyMMdd");

                // Make unique filename
                int fileNameCounter = 2;
                while (!layerNames.Add(fileName))
                {
                    fileName = fileName + "_" + fileNameCounter;
                    fileNameCounter++;
                }

                var expDataset = sourceDataSet.CopyFeatures(true);
                var expDT = expDataset.DataTable;

                if (columnsToExport[layerCount - 1].Count != 0)     //If no data in column export list, then it is an Air Quality layer, which only exports mean (D24 and Quarterly) 
                {
                    List<string> lstRemoveName = new List<string>();
                    bool found;

                    for (int j = 3; j < expDT.Columns.Count; j++) // Find Columns Not Selected For Export-->Starting after 2 to always include col/row/incidence
                    {
                        found = false;
                        foreach (string exportText in columnsToExport[layerCount - 1])
                        {
                            if (exportText == expDT.Columns[j].ColumnName || (exportText == "Percentiles" && expDT.Columns[j].ColumnName.Contains("Percentile")))
                                found = true;
                        }

                        if (!found)
                            lstRemoveName.Add(expDT.Columns[j].ColumnName);
                    }

                    foreach (string s in lstRemoveName)     //Remove columns not requested from the layer
                    {
                        expDT.Columns.Remove(s);
                    }
                }
                // Save
                expDataset.SaveAs(Path.Combine(exportFolder, fileName + ".shp"), false);            //Save and Report Progress

                ReportProgress((double)layerCount/layersToExport.Count() * 100, string.Format("Writing to disk: {0}", fileName)); 

                layerCount += 1;
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
                            (HasParentGroup(_, RESULTS_GROUP) || HasParentGroup(_, POLLUTANTS_GROUP)) &&
                            _.DataSet.GetColumn(LAYER_GRID_COLUMN) != null &&
                            _.DataSet.GetColumn(LAYER_GRID_ROW) != null);

            var layers_HIF = _map.GetAllLayers().OfType<IMapPolygonLayer>()                     //Take the datatable from each subcategory of map layer and provide the column options
                .Where(_ =>
                            (HasParentGroup(_, "Health Impacts")));

            List<string> export_Cols_HIF = new List<string>();
            if (layers_HIF.Count() != 0)
            {
                export_Cols_HIF = layers_HIF.First().DataSet.DataTable.Columns.Cast<DataColumn>()
                      .Select(x => x.ColumnName)
                      .ToList();

                export_Cols_HIF.RemoveAll(x => x.Contains("Percentile"));
                export_Cols_HIF.Remove("Col");
                export_Cols_HIF.Remove("Row");
                export_Cols_HIF.Add("Percentiles");
            }

            var layers_Incidence = _map.GetAllLayers().OfType<IMapPolygonLayer>()
                .Where(_ =>
                            (HasParentGroup(_, "Pooled Incidence")));

            List<string> export_Cols_Incidence = new List<string>();
            if (layers_Incidence.Count() != 0)
            {
                export_Cols_Incidence = layers_Incidence.First().DataSet.DataTable.Columns.Cast<DataColumn>()
                       .Select(x => x.ColumnName)
                       .ToList();

                export_Cols_Incidence.RemoveAll(x => x.Contains("Percentile"));
                export_Cols_Incidence.Remove("COL");
                export_Cols_Incidence.Remove("ROW");
                export_Cols_Incidence.Add("Percentiles");
            }

            var layers_Pooling = _map.GetAllLayers().OfType<IMapPolygonLayer>()
                .Where(_ =>
                            (HasParentGroup(_, "Pooled Valuation")));

            List<string> export_Cols_Pooling = new List<string>();
            if (layers_Pooling.Count() != 0)
            {
                export_Cols_Pooling = layers_Pooling.First().DataSet.DataTable.Columns.Cast<DataColumn>()
                       .Select(x => x.ColumnName)
                       .ToList();

                export_Cols_Pooling.RemoveAll(x => x.Contains("Percentile"));
                export_Cols_Pooling.Remove("COL");
                export_Cols_Pooling.Remove("ROW");
                export_Cols_Pooling.Add("Percentiles");
            }

            List<List<string>> export_Cols = new List<List<string>>();
            export_Cols.Add(export_Cols_HIF);
            export_Cols.Add(export_Cols_Incidence);
            export_Cols.Add(export_Cols_Pooling);

            using (var window = new DataLayerExportDialog(this, layers, export_Cols, layer))
            {
                _windowShown = true;
                window.ShowDialog(_windowOwner);
                _windowShown = false;
                MessageBox.Show("Export Complete");
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
                grp.LayerAdded += delegate (object sender, LayerEventArgs args)
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
                        ShowExportWindow((IMapFeatureLayer)layer);
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
