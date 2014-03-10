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
using System.Linq;
using DataWorker;

namespace BenMAP
{
    public partial class MainRibbonForm : FormBase
    {
        #region Variable
        private string _projectFileName = string.Empty;
        private const string SaveFileDialogFilterText = "Project Files (*.dspx)|*.dspx";
                private RibbonTab _mapView;
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
                private RibbonButton _rbThemePie;
                private RibbonButton _rbThemeBar;
                private RibbonOrbMenuItem _rbomModel;
        private RibbonOrbMenuItem _rbomMonitor;
        private RibbonOrbMenuItem _rbomPopulation;
        private RibbonOrbMenuItem _rbomAQG;
        private RibbonOrbMenuItem _rbomCFG;

                private List<Extent> _previousExtents = new List<Extent>();
        private List<Extent> _nextExtents = new List<Extent>();
        private int _mCurrentExtents = 0;
        bool _IsManualExtentsChange = false;

                                                public double xMin, xMax, yMin, yMax, zMin, zMax;
                public int colCount = 0;           public int rowCount = 0;                   public Dictionary<string, string> dicRefLayers = new Dictionary<string, string>();
                private string GridLayerName = "";


        #endregion

        #region Constructor
        public MainRibbonForm()
        {
            InitializeComponent();
        }
        public DotSpatial.Controls.Map BenMapMainMap;
        #endregion

        public MainRibbonForm(string[] args)
        {
            InitializeComponent();

        }
    }

}
