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
using System.IO;
using DotSpatial.Topology;
using DotSpatial.Data;
using System.Threading;

namespace BenMAP
{
    public partial class MonitorMap : FormBase
    {
        public MonitorMap()
        {// 本窗体主要是负责显示底图和监测点两个图层
            InitializeComponent();
            mainMap.LayerAdded += new EventHandler<LayerEventArgs>(mainMap_LayerAdded);
            //DrawMonitorMap();
        }

        private void mainMap_LayerAdded(object sender, LayerEventArgs e)
        {
            //picGIS.Visible = false;
            //throw new NotImplementedException();
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
        /// <summary>
        /// 网格图层的Shape文件路径
        /// </summary>
        public string GridShapeFile
        {
            get { return _gridShapeFile; }
            set
            {
                _gridShapeFile = value;
            }
        }// GridShapeFile

        private string _monitorPointsShapeFile;
        /// <summary>
        /// 网格图层的Shape文件路径
        /// </summary>
        public string MonitorPointsShapeFile
        {
            get { return _monitorPointsShapeFile; }
            set
            {
                _monitorPointsShapeFile = value;
            }
        }// GridShapeFile

        private List<MonitorValue> _lstMonitorPoints;
        /// <summary>
        /// 检测点图层
        /// </summary>
        public List<MonitorValue> LstMonitorPoints
        {
            get { return _lstMonitorPoints; }
            set
            {
                _lstMonitorPoints = value;
            }
        }// LstMonitorPoints


        private object LayerObject = null;//存储图层对象
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
                {
                    fsPoints.DataTable.Columns.Add(tmps[i]);
                }
                //fsPoints.DataTable.Columns.Add("Col");
                //fsPoints.DataTable.Columns.Add("Row");
                //fsPoints.DataTable.Columns.Add("D24HourMean");
                //fsPoints.DataTable.Columns.Add("QuarterlyMean");
                //fsPoints.DataTable.Columns.Add("Value");
                MonitorValue mv = null;
                Feature feature = null;
                List<Coordinate> lstCoordinate = new List<Coordinate>();
                List<double> fsInter = new List<double>();
                // 画边界图网格图层
                mainMap.Layers.Clear();
                mainMap.ProjectionModeReproject = ActionMode.Never;
                mainMap.ProjectionModeDefine = ActionMode.Never;
                mainMap.Layers.Clear();
                if (File.Exists(this._gridShapeFile))
                {
                    mainMap.Layers.Add(this._gridShapeFile);
                    //mainMap.Layers.Add(
                }
                if (this._lstMonitorPoints != null && this.LstMonitorPoints.Count > 0)
                {
                    //featureSet.DataTable=new DotSpatial
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
                        {
                            fsPoints.DataTable.Rows[i][tmps[col]] = mv.dicMetricValues[tmps[col]];
                        }

                    }
                    mainMap.Layers.Add(fsPoints);
                    //mainMap.Layers[1].
                    mainMap.Layers[0].LegendText = "Air quality grid";
                    mainMap.Layers[1].LegendText = "Monitors";
                }
                PolygonLayer player = mainMap.Layers[0] as PolygonLayer;
                float f = 0.9f;
                Color c = Color.Transparent;
                PolygonSymbolizer Transparent = new PolygonSymbolizer(c);
                //Transparent.OutlineSymbolizer = new LineSymbolizer(Color.Black, 1);
                Transparent.OutlineSymbolizer = new LineSymbolizer(Color.DarkBlue, 1);     //设置region图层outline宽度为2
                player.Symbolizer = Transparent;
                LayerObject = null;
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// 放大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            mainMap.FunctionMode = FunctionMode.ZoomIn;
        }

        /// <summary>
        /// 缩小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            mainMap.FunctionMode = FunctionMode.ZoomOut;
        }

        /// <summary>
        /// 拖拽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPan_Click(object sender, EventArgs e)
        {
            //set the function mode
            mainMap.FunctionMode = FunctionMode.Pan;
        }

        /// <summary>
        /// 完整范围
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            // mainMap.ZoomToMaxExtent();
            //mainMap.ZoomToPrevious();
            //mainMap.ZoomToPrevious();
            splitContainer2.SplitterDistance = 0;
            this.splitContainer2.BorderStyle = BorderStyle.None;
            //mainMap.ZoomToMaxExtent();
            //mainMap.ProjectionChanged
            //mainMap.ResetExtents();
            mainMap.ViewExtents = et;
            //mainMap.ZoomToMaxExtent();
            //mainMap.Projection.
            //mainMap.ZoomToPrevious();
            //mainMap.ZoomToPrevious();
            //mainMap.z

        }

        /// <summary>
        /// 保存地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                //Thread.Sleep(new TimeSpan(0, 0, 2));
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
                //Thread.Sleep(300);

                i.Save(fileName);
                MessageBox.Show("Map exported.");
                //----Save SHP File
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
                if (mainMap.Projection != DotSpatial.Projections.KnownCoordinateSystems.Projected.NorthAmerica.USAContiguousLambertConformalConic)
                {
                    mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Projected.NorthAmerica.USAContiguousLambertConformalConic;
                    foreach (FeatureLayer layer in mainMap.Layers)
                    {
                        //if (layer.Projection == null || (layer.Projection as DotSpatial.Projections.ProjDescriptor).ToString() == "Nothing.")
                        layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                        layer.Reproject(mainMap.Projection);
                    }
                }
                else
                {
                    mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                    foreach (FeatureLayer layer in mainMap.Layers)
                    {
                        //if (layer.Projection == null || (layer.Projection as DotSpatial.Projections.ProjDescriptor).ToString() == "Nothing.")
                        layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Projected.NorthAmerica.USAContiguousLambertConformalConic;
                        layer.Reproject(mainMap.Projection);
                    }
                    //mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                }
                foreach (IMapGroup grp in mainMap.GetAllGroups())
                {
                    grp.Projection.CopyProperties(mainMap.Projection);
                }
                //re-assign the map projection
                mainMap.Projection.CopyProperties(mainMap.Projection);
                //zoom to reprojected extent
                //mainMap.Invalidate();
                //mainMap.ResetExtents();
                mainMap.ViewExtents = mainMap.Layers[0].Extent;
            }
            catch (Exception ex)
            {
            }
        }

        private void tsbChangeCone_Click(object sender, EventArgs e)
        {
            //
            if (mainMap.Layers.Count < 2)
                return;
        }

        /// <summary>
        /// 加图层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbAddLayer_Click(object sender, EventArgs e)
        {
            mainMap.AddLayer();
        }





    }
}
