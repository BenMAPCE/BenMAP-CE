using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Text.RegularExpressions;

using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Controls;
using DotSpatial.Topology;
using DotSpatial.Projections;
using System.Diagnostics;
using DotSpatial.Controls.RibbonControls;
//using BenMAP.Properties;
//using MapWinGIS;
using System.Linq;
using DataWorker;

namespace BenMAP
{
    public partial class MainRibbonForm : FormBase
    {
        #region Variable
        private string _projectFileName = string.Empty;
        private const string SaveFileDialogFilterText = "Project Files (*.dspx)|*.dspx";
        //MapView Ribbon TabPage and its related controls
        private RibbonTab _mapView;
        //private RibbonButton _rbPrint;
        private RibbonButton _rbAdd;
        private RibbonButton _rbPan;
        private RibbonButton _rbSelect;
        private RibbonButton _rbZoomIn;
        private RibbonButton _rbZoomOut;
        private RibbonButton _rbIdentifier;
        private RibbonButton _rbAttribute;
        private RibbonButton _rbMaxExtents;
        private RibbonButton _rbMeasure;
        private RibbonButton _rbZoomToPrevious;
        private RibbonButton _rbZoomToNext;
        //专题图-饼图
        private RibbonButton _rbThemePie;
        //专题图-柱状图
        private RibbonButton _rbThemeBar;
        //Model Monitor Population AQG CFG
        private RibbonOrbMenuItem _rbomModel;
        private RibbonOrbMenuItem _rbomMonitor;
        private RibbonOrbMenuItem _rbomPopulation;
        private RibbonOrbMenuItem _rbomAQG;
        private RibbonOrbMenuItem _rbomCFG;

        //store the map extents
        private List<Extent> _previousExtents = new List<Extent>();
        private List<Extent> _nextExtents = new List<Extent>();
        private int _mCurrentExtents = 0;
        bool _IsManualExtentsChange = false;

        //Create a new instance for MapWinGIS.Shapefile
        //public MapWinGIS.Shapefile myShapefile = new MapWinGIS.Shapefile();
        //Create a new instance for MapWinGIS.Grid
        //public MapWinGIS.Grid myGrid = new Grid();
        //MapExtent
        public double xMin, xMax, yMin, yMax, zMin, zMax;
        //get the Col and Row count from shapefile
        public int colCount = 0;   //number of columns for the new grid
        public int rowCount = 0;   //number of rows for the new grid
        //dictionary of reference layer
        public Dictionary<string, string> dicRefLayers = new Dictionary<string, string>();
        //string str36kmGridPath = CommonClass.GridGrid.ToString(); //  Application.StartupPath + @"\Data\USAData\ShapeFile\36km1.shp";
        private string GridLayerName = "";


        #endregion

        #region Constructor
        public MainRibbonForm()
        {
            InitializeComponent();
        }
        public DotSpatial.Controls.Map BenMapMainMap;
        #endregion

        /// <summary>
        /// Initialize MapView
        /// </summary>
        /// <param name="args">The file name to open when HydroDesktop starts</param>
        public MainRibbonForm(string[] args)
        {
            InitializeComponent();

        }
    }

}
