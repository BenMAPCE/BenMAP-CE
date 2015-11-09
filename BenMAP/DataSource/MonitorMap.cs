using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Symbology;
using DotSpatial.Controls;
using DotSpatial.Projections;
using System.IO;
using DotSpatial.Topology;
using DotSpatial.Data;
using System.Threading;

namespace BenMAP
{
    public partial class MonitorMap : FormBase
    {
        public MonitorMap()
        {
            InitializeComponent();
            mainMap.LayerAdded += new EventHandler<LayerEventArgs>(mainMap_LayerAdded);
        }

        private void mainMap_LayerAdded(object sender, LayerEventArgs e)
        {
        }

        private void MonitorMap_Load(object sender, EventArgs e)
        {
            try
            {
                DrawMonitorMap();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private string _gridShapeFile;
        public string GridShapeFile
        {
            get { return _gridShapeFile; }
            set
            {
                _gridShapeFile = value;
            }
        }
        private string _monitorPointsShapeFile;
        public string MonitorPointsShapeFile
        {
            get { return _monitorPointsShapeFile; }
            set
            {
                _monitorPointsShapeFile = value;
            }
        }
        private List<MonitorValue> _lstMonitorPoints;
        public List<MonitorValue> LstMonitorPoints
        {
            get { return _lstMonitorPoints; }
            set
            {
                _lstMonitorPoints = value;
            }
        }

        private object LayerObject = null;
        private bool DrawMonitorMap()
        {
            try
            {
                IFeatureSet fsPoints = new FeatureSet();
                fsPoints.DataTable.Columns.Add("Name");
                fsPoints.DataTable.Columns.Add("Description");
                fsPoints.DataTable.Columns.Add("Longitude");
                fsPoints.DataTable.Columns.Add("Latitude");
                string[] tmps = new string[_lstMonitorPoints[0].dicMetricValues.Count];
                _lstMonitorPoints[0].dicMetricValues.Keys.CopyTo(tmps, 0);
                for (int i = 0; i < tmps.Length; i++)
                { fsPoints.DataTable.Columns.Add(tmps[i]); }
                MonitorValue mv = null;
                Feature feature = null;
                List<Coordinate> lstCoordinate = new List<Coordinate>();
                List<double> fsInter = new List<double>();
                mainMap.Layers.Clear();
                mainMap.ProjectionModeReproject = ActionMode.Never;
                mainMap.ProjectionModeDefine = ActionMode.Never;
                mainMap.Layers.Clear();

                if (File.Exists(this._gridShapeFile))
                { mainMap.Layers.Add(this._gridShapeFile); }
                if (this._lstMonitorPoints != null && this.LstMonitorPoints.Count > 0)
                {
                    PolygonScheme myScheme = new PolygonScheme();
                    PolygonCategory pcin = new PolygonCategory();
                    pcin.Symbolizer.SetFillColor(Color.Red);
                    myScheme.Categories.Add(pcin);
                    DotSpatial.Topology.Point point;
                    for (int i = 0; i < LstMonitorPoints.Count; i++)
                    {
                        mv = LstMonitorPoints[i];
                        point = new DotSpatial.Topology.Point(mv.Longitude, mv.Latitude);
                        feature = new Feature(point);
                        fsPoints.AddFeature(feature);
                        fsPoints.DataTable.Rows[i]["Name"] = mv.MonitorName;
                        fsPoints.DataTable.Rows[i]["Latitude"] = mv.Latitude;
                        fsPoints.DataTable.Rows[i]["Longitude"] = mv.Longitude;
                        for (int col = 0; col < tmps.Length; col++)
                        { fsPoints.DataTable.Rows[i][tmps[col]] = mv.dicMetricValues[tmps[col]]; }
                    }
                    IMapFeatureLayer imfl = mainMap.Layers.Add(fsPoints);
                    imfl.LegendText = "Monitors";
                    PointSymbolizer ps = new PointSymbolizer(Color.Yellow, DotSpatial.Symbology.PointShape.Ellipse, 8);
                    ps.SetOutline(Color.Black, 1);
                    imfl.Symbolizer = ps;
                    mainMap.Layers[0].LegendText = "Air quality grid";
                }
                
                PolygonLayer player = mainMap.Layers[0] as PolygonLayer;
                Color c = Color.Transparent;
                PolygonSymbolizer Transparent = new PolygonSymbolizer(c);
                Transparent.OutlineSymbolizer = new LineSymbolizer(Color.DarkBlue, 1);
                player.Symbolizer = Transparent;
                return true;
            }
            catch (Exception ex)
            { Logger.LogError(ex); return false; }
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            mainMap.FunctionMode = FunctionMode.ZoomIn;
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            mainMap.FunctionMode = FunctionMode.ZoomOut;
        }

        private void btnPan_Click(object sender, EventArgs e)
        {
            mainMap.FunctionMode = FunctionMode.Pan;
        }

        private void btnFullExtent_Click(object sender, EventArgs e)
        {
            mainMap.ZoomToMaxExtent();
            mainMap.FunctionMode = FunctionMode.None;
        }

        private void btnIdentify_Click(object sender, EventArgs e)
        {
            mainMap.FunctionMode = FunctionMode.Info;
        }

        private void btnLayerSet_Click(object sender, EventArgs e)
        {
            Extent et = mainMap.Extent;
            splitContainer2.Panel1.Hide();
            splitContainer2.SplitterDistance = 0;
            this.splitContainer2.BorderStyle = BorderStyle.None;
            mainMap.ViewExtents = et;

        }

        private void tsbSaveMap_Click(object sender, EventArgs e)
        {
            try
            {
                if (mainMap.Layers.Count == 0)
                    return;
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "SHP(*.shp)|*.shp";
                saveFileDialog1.InitialDirectory = "C:\\";
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                string fileName = saveFileDialog1.FileName;
                FeatureLayer fl = mainMap.Layers[mainMap.Layers.Count - 1] as FeatureLayer;
                fl.DataSet.SaveAs(fileName, true);
                MessageBox.Show("Shapefile saved.", "File saved");
            }
            catch
            {
            }
        }

        private void tsbSavePic_Click(object sender, EventArgs e)
        {
            try
            {
                string s = tsbSavePic.ToolTipText;
                tsbSavePic.ToolTipText = "";
                Image i = new Bitmap(mainMap.Width, mainMap.Height - toolStrip1.Height);
                Graphics g = Graphics.FromImage(i);
                tsbSavePic.ToolTipText = s;
                g.CopyFromScreen(this.PointToScreen(new System.Drawing.Point(splitContainer2.Panel1.Width + 6, toolStrip1.Parent.Location.Y + toolStrip1.Height)), new System.Drawing.Point(0, 0), new Size(this.Width, splitContainer2.Height - toolStrip1.Height - 1));
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "PNG(*.png)|*.png|JPG(*.jpg)|*.jpg";
                saveFileDialog1.InitialDirectory = "C:\\";
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                string fileName = saveFileDialog1.FileName;

                i.Save(fileName);
                MessageBox.Show("Map exported.");
                g.Dispose();
            }
            catch (Exception ex)
            {
            }
        }

        private void tsbChangeProjection_Click(object sender, EventArgs e)
        {
            try
            {
                if (mainMap.Layers.Count == 0) return;

                //exit if we have no setup projection
                if (String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
                {
                    return;
                }

                ProjectionInfo setupProjection = CommonClass.getProjectionInfoFromName(CommonClass.MainSetup.SetupProjection);
                if (setupProjection == null)
                {
                    return;
                }

                if (mainMap.Projection != setupProjection)
                {
                    mainMap.Projection = setupProjection;
                    foreach (FeatureLayer layer in mainMap.GetAllLayers())
                    {
                        layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                        layer.Reproject(mainMap.Projection);
                    }
                    tsbChangeProjection.Text = "change projection to WGS1984";
                }
                else
                {
                    mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                    foreach (FeatureLayer layer in mainMap.GetAllLayers())
                    {
                        layer.Projection = setupProjection;
                        layer.Reproject(mainMap.Projection);
                    }
                    tsbChangeProjection.Text = "change projection to setup projection (" + CommonClass.MainSetup.SetupProjection + ")";
                }

                foreach (IMapGroup grp in mainMap.GetAllGroups())
                {
                    grp.Projection.CopyProperties(mainMap.Projection);
                }
                mainMap.Projection.CopyProperties(mainMap.Projection);
                mainMap.ViewExtents = mainMap.Layers[0].Extent;
            }
            catch (Exception ex)
            {
            }
        }

        private void tsbChangeCone_Click(object sender, EventArgs e)
        {
            if (mainMap.Layers.Count < 2)
                return;
        }

        private void tsbAddLayer_Click(object sender, EventArgs e)
        {
            mainMap.AddLayer();
        }





    }
}
