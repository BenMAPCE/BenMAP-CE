using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BenMAP.Configuration;
using BrightIdeasSoftware;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Extensions;  //MCB- needed?
using DotSpatial.Projections;
//using DotSpatial.Plugins; //MCB - needed?
//using DotSpatial.Plugins.TableEditor;  // MCB-?
//using DotSpatial.Plugins.AttributeDataExplorer;  //MCB- needed?
using ZedGraph;
using ESIL.DBUtility;
using System.Configuration;
using ProtoBuf;
using System.Collections;
using BenMAP.Grid;

namespace BenMAP
{
    public partial class BenMAP : FormBase
    {
        BenMAPGrid chartGrid;
        BenMAPGrid ChartGrid
        {
            get { return chartGrid; }
            set
            {
                chartGrid = value;
                CommonClass.changeGridType(chartGrid);
            }
        }
        FeatureSet fs36km = new FeatureSet();
        private const string _readyImageKey = "ready";

        private const string _unreadyImageKey = "unready";

        private const string _yibuImageKey = "yibu";

        private const string _errorImageKey = "error";

        private string _baseFormTitle = "";
        public Main mainFrm = null;

        private string _CurrentMapTitle = "";
        private string _CurrentMapTableTitle = "";
        private Extent _SavedExtent;
        private List<string> _listAddGridTo36km = new List<string>();
        private string _reportTableFileName = "";

        private FeatureSet _fsregion = new FeatureSet();
        private bool isLegendHide = false;
        private object LayerObject = null; private string _currentNode = string.Empty; private string _homePageName;

        public string HomePageName
        {
            get { return _homePageName; }
            set { _homePageName = value; }
        }

        string chartXAxis = ""; string strchartTitle = ""; string strchartX = ""; string strchartY = ""; string strCDFTitle = "";
        string strCDFX = "";
        string strCDFY = "";
        int iCDF = -1; bool canshowCDF = false;
        List<CRSelectFunctionCalculateValue> lstCFGRforCDF = new List<CRSelectFunctionCalculateValue>();
        List<AllSelectCRFunction> lstCFGRpoolingforCDF = new List<AllSelectCRFunction>();
        List<AllSelectValuationMethodAndValue> lstAPVRforCDF = new List<AllSelectValuationMethodAndValue>();

        //private DotSpatial.Plugins.AttributeDataExplorer.AttributeDataExplorerPlugin dspADE;  //-MCB
        public BenMAP(string homePageName)
        {
            try
            {
                InitializeComponent();
                Control.CheckForIllegalCrossThreadCalls = false;
                _homePageName = homePageName;
                splitContainer1.Visible = false;
                zedGraphCtl.Visible = false;
                this.tabCtlReport.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
                this.tabCtlReport.DrawItem += new DrawItemEventHandler(DrawTabControlItems);

                CommonClass.NodeAnscy -= ChangeNodeStatus;
                CommonClass.NodeAnscy += ChangeNodeStatus;

                mainMap.LayerAdded += new EventHandler<LayerEventArgs>(mainMap_LayerAdded);
                mainMap.Layers.LayerVisibleChanged += new EventHandler(mainMap_LayerVisibleChanged);
                //this.appManager1 = new DotSpatial.Controls.AppManager();
                //appManager1.Directories.Clear();
                //appManager1.Directories.Add(@"Plugins\GDAL");
                appManager1.LoadExtensions();
                //Console.WriteLine("MCB-test");
                //foreach (DotSpatial.Extensions.IExtension iext in appManager1.Extensions)
                //{
                //    Console.WriteLine(iext.Name);
                //}
           
                //MCB- right place for this???
              //AttributeDataExplorerPlugin AttEx = new AttributeDataExplorerPlugin();
              // AttEx.Activate();
              //  AttEx.IsActive = true;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private string _currentImage = _unreadyImageKey;
        private string _currentAnsyNode = string.Empty;
        private string _currentPollutant = string.Empty;
        int _count = 0;
        private void ChangeNodeStatus()
        {
            try
            {
                string ansy = string.Empty;
                lock (CommonClass.NodeAnscyStatus)
                {
                    if (CommonClass.NodeAnscyStatus != string.Empty)
                    {
                        ansy = CommonClass.NodeAnscyStatus;
                        string[] tmps = ansy.Split(new char[] { ';' }); _currentPollutant = tmps[0].ToLower();
                        _currentAnsyNode = tmps[1];
                        if (tmps[2] == "on") { _currentImage = _yibuImageKey; }
                        else if (tmps[2] == "off") { _currentImage = _readyImageKey; }
                        foreach (TreeNode node in trvSetting.Nodes)
                        {
                            RecursiveQuery(node);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void RecursiveQuery(TreeNode node)
        {
            try
            {


                if (_currentAnsyNode == node.Name)
                {
                    string nodeTag = node.Parent.Tag.ToString();
                    string pollutantName = "";
                    foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
                    {
                        if (int.Parse(nodeTag) == b.Pollutant.PollutantID)
                        {
                            pollutantName = b.Pollutant.PollutantName;
                        }
                    }
                    if (_currentPollutant.ToLower() == pollutantName.ToLower())
                    {
                        node.ImageKey = _currentImage;
                        node.SelectedImageKey = _currentImage;

                        return;
                    }
                }
                else
                {
                    if (node.Nodes.Count > 0)
                    {
                        foreach (TreeNode child in node.Nodes)
                        {
                            RecursiveQuery(child);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void SetTabControl(TabControl tc)
        {
            try
            {
                if (tc == null) { return; }
                for (int i = tc.TabPages.Count - 2; i >= 0; i--)
                {
                }
                tc.Refresh();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void DrawTabControlItems(object sender, DrawItemEventArgs e)
        {
            try
            {
                Font tabFont = null;
                Brush bBackColor = null;
                Brush bForeColor = null;
                string tag = tabCtlReport.TabPages[e.Index].Tag.ToString().ToLower();
                string selectedPage = tabCtlReport.SelectedTab.Tag.ToString().ToLower();

                tabFont = new System.Drawing.Font("Cambri", 9, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                int iImageKey = -1;
                switch (tag)
                {
                    case "function":
                        iImageKey = 12;
                        break;
                    case "apvx":
                        iImageKey = 13;
                        break;
                    case "incidence":
                        iImageKey = 13;
                        break;
                    case "qaly":
                        iImageKey = 13;
                        break;
                }
                if ((tag == "function") && (CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count > 0 && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues != null))
                {
                    bForeColor = Brushes.Black;
                    if (tag == selectedPage)
                    {
                        bBackColor = new SolidBrush(Color.White);
                    }
                    else
                    {
                        bBackColor = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, SystemColors.Control, SystemColors.Control, System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal);
                    }
                    iImageKey = 12;
                }
                else if ((tag == "apvx") && (cbPoolingWindowAPV != null && cbPoolingWindowAPV.Items.Count > 0))
                {
                    bForeColor = Brushes.Black;
                    if (tag == selectedPage)
                    {
                        bBackColor = new SolidBrush(Color.White);
                    }
                    else
                    {
                        bBackColor = new LinearGradientBrush(e.Bounds, SystemColors.Control, SystemColors.Control, LinearGradientMode.BackwardDiagonal);
                    }
                    iImageKey = 13;
                }
                else if ((tag == "incidence") && (cbPoolingWindowIncidence != null && cbPoolingWindowIncidence.Items.Count > 0))
                {
                    bForeColor = Brushes.Black;
                    if (tag == selectedPage)
                    {
                        bBackColor = new SolidBrush(Color.White);
                    }
                    else
                    {
                        bBackColor = new LinearGradientBrush(e.Bounds, SystemColors.Control, SystemColors.Control, LinearGradientMode.BackwardDiagonal);
                    }
                    iImageKey = 13;
                }
                else if (tag == "audit")
                {
                    bForeColor = Brushes.Black;
                    if (tag == selectedPage)
                    {
                        bBackColor = new SolidBrush(Color.White);
                    }
                    else
                    {
                        bBackColor = new LinearGradientBrush(e.Bounds, SystemColors.Control, SystemColors.Control, LinearGradientMode.BackwardDiagonal);
                    }
                }
                else
                {
                    bBackColor = new LinearGradientBrush(e.Bounds, SystemColors.Control, SystemColors.Control, LinearGradientMode.BackwardDiagonal);
                    bForeColor = Brushes.Gray;
                }
                string tabName = this.tabCtlReport.TabPages[e.Index].Text;
                StringFormat sftTab = new StringFormat();
                e.Graphics.FillRectangle(bBackColor, e.Bounds);
                Rectangle recTab = e.Bounds;
                if (iImageKey != -1)
                {

                    recTab = new Rectangle(recTab.X + 18, recTab.Y + 4, recTab.Width - 12, recTab.Height - 4);
                    e.Graphics.DrawImage(SmallImageList.Images[iImageKey], (new Point(e.Bounds.X + 2, e.Bounds.Y + 4)));
                    e.Graphics.DrawString(tabName, tabFont, bForeColor, recTab, sftTab);
                }
                else
                {
                    recTab = new Rectangle(recTab.X, recTab.Y + 4, recTab.Width + 6, recTab.Height - 4);
                    e.Graphics.DrawString(tabName, tabFont, bForeColor, recTab, sftTab);
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void mainMap_LayerAdded(object sender, LayerEventArgs e)
        {
            picGIS.Visible = false;
        }

        private void mainMap_LayerVisibleChanged(object sender, EventArgs e)
        {
            Update_Map_Title();   
        }

        private void Update_Map_Title()   //Change the map title to be equal to the top layer that is visible
        {
            ILayer TopLayer = null;
            bool IgnoreAdminMapGroup = true;
            TopLayer = FindTopVisibleLayer(IgnoreAdminMapGroup);
            if (TopLayer != null & TopLayer is FeatureLayer)  //Change the map title depending on the layer legendtext and the map group that it is in. 
            { 
               //Find the Parent, grandparent, etc. nodes of the layer of interest and based on identity of parent, grandparent or greate grandparent, modify the title
                MapGroup ParentMG = null;
                MapGroup GrandParentMG = null;
                MapGroup GreatGrandParent = null;
                
                List<IMapGroup> AllMG = new List<IMapGroup>();
                AllMG = mainMap.GetAllGroups();
                ParentMG = (MapGroup)AllMG.Find(m => m.Contains(TopLayer));
                if (ParentMG != null) 
                {
                    GrandParentMG = (MapGroup)AllMG.Find(m => m.Contains(ParentMG));
                    if (GrandParentMG != null) 
                    {
                        if (GrandParentMG.LegendText.ToLower() == "results")
                        {
                            _CurrentMapTitle = CommonClass.MainSetup.SetupName + " Setup:" + ParentMG.LegendText + " , " + TopLayer.LegendText;
                            tbMapTitle.Text = _CurrentMapTitle;
                        }
                        else //May be in the Pollutants Mapgroup
                        {
                            GreatGrandParent = (MapGroup)AllMG.Find(m => m.Contains(GrandParentMG));
                            if (GreatGrandParent != null & GreatGrandParent.LegendText.ToLower() == "pollutants")
                            { 
                                string polText = GrandParentMG.LegendText;
                                string statText = ParentMG.LegendText;
                                _CurrentMapTitle = CommonClass.MainSetup.SetupName + " Setup:" + polText + " - " + statText + " , " + TopLayer.LegendText;
                                tbMapTitle.Text = _CurrentMapTitle;
                            }
                            else  //Unknown mapgroup 
                            {
                                // Don't update the map title
                                return; 
                            }

                        }
                    }
                    else//layer only has a parent MapGroup - could be "Region Admin group or unknkown MapGroup
                    {   
                        // Don't update the map title
                        return; 
                    }
                }
                else //Top visible layer not in a map group;
                {
                    // Don't update the map title. 
                    return;
                }

            }
        }

        private bool AddData2CommonClass(TreeView tree)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }

        public void NewFile()
        {
            splitContainer1.Visible = true;
            CommonClass.ClearAllObject();
            _MapAlreadyDisplayed = false;
            ClearAll();
            ResetParamsTree(Application.StartupPath + @"\Configs\ParamsTree_USA.xml");
        }

        public void ClearAll()
        {
            try
            {
                if (_MapAlreadyDisplayed) picGIS.Visible = false;
                else picGIS.Visible = true;
                //this.picGIS.Visible = false; // true;
                if (!_MapAlreadyDisplayed) mainMap.Layers.Clear();
                pnlChart.BackgroundImage = null;
                tabCtlMain.SelectTab(tabGIS);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        public string ProjFileName = "";
        public void OpenProject()
        {
            try
            {
                OpenFileDialog openfile = new OpenFileDialog();
                openfile.InitialDirectory = CommonClass.ResultFilePath + @"\Result\Project";
                openfile.Filter = "BenMAP Project File(*.projx)|*.projx";
                openfile.FilterIndex = 1;
                openfile.RestoreDirectory = true;
                if (openfile.ShowDialog() != DialogResult.OK)
                { ProjFileName = ""; return; }
                else
                    ProjFileName = openfile.FileName;
                WaitShow("Loading project file");


                if (!CommonClass.LoadBenMAPProject(openfile.FileName))
                {
                    WaitClose();
                    MessageBox.Show("BenMAP-CE was unable to open the file. The file may be corrupt, or it may have been created using a previous incompatible version of BenMAP-CE.");
                    return;
                }
                this.OpenFile();
                CommonClass.LstPollutant = null;
                CommonClass.RBenMAPGrid = null;
                CommonClass.GBenMAPGrid = null;
                CommonClass.LstBaseControlGroup = null;
                CommonClass.LstCreateShapeFileParams = null;
                CommonClass.CRThreshold = 0;
                CommonClass.CRLatinHypercubePoints = 20;
                CommonClass.CRRunInPointMode = false;
                CommonClass.BenMAPPopulation = null;
                CommonClass.BaseControlCRSelectFunction = null; CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
                CommonClass.lstIncidencePoolingAndAggregation = null;

                CommonClass.IncidencePoolingResult = null;
                CommonClass.ValuationMethodPoolingAndAggregation = null;
                CommonClass.BaseControlCRSelectFunction = null;
                CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
                CommonClass.ValuationMethodPoolingAndAggregation = null;
                GC.Collect();
                CommonClass.LoadBenMAPProject(openfile.FileName);
                BenMAP_Load(this, null);
                if (CommonClass.ValuationMethodPoolingAndAggregation != null)
                {
                    CommonClass.lstIncidencePoolingAndAggregation = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Select(a => a.IncidencePoolingAndAggregation).ToList();
                    CommonClass.IncidencePoolingAndAggregationAdvance = CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance;
                    cbPoolingWindowAPV.Items.Clear();
                    CommonClass.BaseControlCRSelectFunctionCalculateValue = CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue;
                    CommonClass.BaseControlCRSelectFunction = null;
                    CommonClass.BaseControlCRSelectFunction = new BaseControlCRSelectFunction();
                    CommonClass.BaseControlCRSelectFunction.BaseControlGroup = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup;
                    CommonClass.BaseControlCRSelectFunction.BenMAPPopulation = CommonClass.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
                    CommonClass.BaseControlCRSelectFunction.CRLatinHypercubePoints = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints;
                    CommonClass.BaseControlCRSelectFunction.CRRunInPointMode = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRRunInPointMode;
                    CommonClass.BaseControlCRSelectFunction.CRSeeds = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRSeeds;
                    CommonClass.BaseControlCRSelectFunction.CRThreshold = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRThreshold;
                    CommonClass.BaseControlCRSelectFunction.RBenMapGrid = CommonClass.BaseControlCRSelectFunctionCalculateValue.RBenMapGrid;
                    CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction = new List<CRSelectFunction>();
                    for (int i = 0; i < CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; i++)
                    {
                        CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Add(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[i].CRSelectFunction);
                    }
                    try
                    {
                        if (CommonClass.BaseControlCRSelectFunction != null)
                        {
                            showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, this.trvSetting.Nodes["aggregationpoolingvaluation"]);

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        Logger.LogError(ex);
                    }

                    errorNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]); //health impact functions
                    //errorNodeImage(trvSetting.Nodes[2].Nodes[0]); //aggregation
                    errorNodeImage(trvSetting.Nodes[2].Nodes[1]); //pooling
                    errorNodeImage(trvSetting.Nodes[2].Nodes[2]); //valuation



                }
                else if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null)
                {
                    errorNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
                    showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, this.trvSetting.Nodes["aggregationpoolingvaluation"]);                    
                }
                else if (CommonClass.BaseControlCRSelectFunction != null)
                {
                    errorNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
                    showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, this.trvSetting.Nodes["aggregationpoolingvaluation"]);

                }
                else if (CommonClass.LstBaseControlGroup != null && CommonClass.LstBaseControlGroup.Count > 0)
                {
                    int nodesCount = 0;
                    foreach (TreeNode trchild in trvSetting.Nodes)
                    {
                        if (trchild.Name == "airqualitygridgroup")
                        {
                            nodesCount = trchild.Nodes.Count;


                            for (int i = nodesCount - 1; i > -1; i--)
                            {
                                TreeNode node = trchild.Nodes[i];
                                if (trchild.Nodes[i].Name == "datasource") { trchild.Nodes.RemoveAt(i); }
                            }
                            for (int i = CommonClass.LstBaseControlGroup.Count - 1; i > -1; i--)
                            {
                                AddDataSourceNode(CommonClass.LstBaseControlGroup[i], trchild);
                                if (CommonClass.LstBaseControlGroup[i] != null && CommonClass.LstBaseControlGroup[i].Base != null)
                                {
                                    changeNodeImage(trchild.Nodes[trchild.Nodes.Count - 1].Nodes[0]);
                                }
                                if (CommonClass.LstBaseControlGroup[i] != null && CommonClass.LstBaseControlGroup[i].Control != null)
                                {
                                    changeNodeImage(trchild.Nodes[trchild.Nodes.Count - 1].Nodes[1]);
                                }
                                if (CommonClass.LstBaseControlGroup[i] != null && CommonClass.LstBaseControlGroup[i].Base != null && CommonClass.LstBaseControlGroup[i].Control != null)
                                {
                                    changeNodeImage(trchild.Nodes[trchild.Nodes.Count - 1]);
                                }
                            }
                            trchild.ExpandAll();

                            foreach (TreeNode trair in trchild.Nodes)
                            {
                                if (trair.Name != "datasource")
                                    changeNodeImage(trair);
                                TreeNode tr = trair;
                                if (trair.Name == "gridtype")
                                {
                                    AddChildNodes(ref tr, "", "", new BenMAPLine());
                                    trair.ExpandAll();
                                }
                            }
                        }
                        if (trchild.Name == "configuration")
                        {
                            foreach (TreeNode tr in trchild.Nodes)
                            {
                                initNodeImage(tr);
                            }
                            trchild.ExpandAll();
                        }
                        if (trchild.Name == "aggregationpoolingvaluation")
                        {
                            foreach (TreeNode tr in trchild.Nodes)
                            {
                                initNodeImage(tr);
                            }
                            trchild.ExpandAll();
                        }
                    }
                }
                else
                {
                    if (CommonClass.GBenMAPGrid != null)
                    {
                        TreeNode currentNode = trvSetting.Nodes[0].Nodes["gridtype"];
                        AddChildNodes(ref currentNode, "", "", null);
                        changeNodeImage(currentNode);
                    }
                    if (CommonClass.LstPollutant != null)
                    {
                        int nodesCount = trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes.Count;
                        if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0)
                        {
                            for (int i = nodesCount - 2; i > -1; i--)
                            {
                                TreeNode node = trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i];
                                if (trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Name == "datasource") { trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes.RemoveAt(i); }
                            }
                            for (int i = nodesCount - 1; i > -1; i--)
                            {
                                TreeNode node = trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i];
                                if (trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Name == "datasource")
                                {
                                    trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Text = "Source of Air Quality Data";
                                    trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Nodes[0].Tag = null;
                                    trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Nodes[0].Nodes.Clear();
                                    trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Nodes[1].Tag = null;
                                    trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Nodes[1].Nodes.Clear();
                                }
                            }

                            olvCRFunctionResult.SetObjects(null);
                            olvIncidence.SetObjects(null);
                            tlvAPVResult.SetObjects(null);
                            cbPoolingWindowIncidence.Items.Clear();
                            cbPoolingWindowAPV.Items.Clear();
                            ClearMapTableChart();
                            initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);

                            initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
                            initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
                            return;
                        }

                        trvSetting.Nodes["pollutant"].Parent.ExpandAll();
                    }
                }
                if (CommonClass.BenMAPPopulation != null)
                {
                    changeNodeImage(trvSetting.Nodes[1].Nodes[0]);
                }
                if (CommonClass.IncidencePoolingAndAggregationAdvance != null)
                {
                    changeNodeImage(trvSetting.Nodes[2].Nodes[0]);
                }
                WaitClose();
            }
            catch (Exception ex)
            {
                WaitClose();
                Logger.LogError(ex);
            }
        }
        private void ChangeAllAggregationCombox()
        {
            System.Data.DataSet dsCRAggregationGridType = BindGridtype();
            cbCRAggregation.DataSource = dsCRAggregationGridType.Tables[0];
            cbCRAggregation.DisplayMember = "GridDefinitionName";
            cbCRAggregation.SelectedIndex = 0;
            System.Data.DataSet dsIncidenceAggregationGridType = BindGridtype();
            cbIncidenceAggregation.DataSource = dsIncidenceAggregationGridType.Tables[0];
            cbIncidenceAggregation.DisplayMember = "GridDefinitionName";
            cbIncidenceAggregation.SelectedIndex = 0;
            System.Data.DataSet dsAPVAggregationGridType = BindGridtype();
            cbAPVAggregation.DataSource = dsAPVAggregationGridType.Tables[0];
            cbAPVAggregation.DisplayMember = "GridDefinitionName";
            cbAPVAggregation.SelectedIndex = 0;
            System.Data.DataSet dsQALYAggregationGridType = BindGridtype();
            FireBirdHelperBase fbRegion = new ESILFireBirdHelper();
            string commandTextRegion = string.Format("select * from GridDefinitions where columns<=56 and setupid={0}  order by GridDefinitionName desc", CommonClass.MainSetup.SetupID);
            System.Data.DataSet dsRegion = fbRegion.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandTextRegion);
            cboRegion.DataSource = dsRegion.Tables[0];
            cboRegion.DisplayMember = "GridDefinitionName";
            for (int i = 0; i < dsRegion.Tables[0].Rows.Count; i++)
            {
                if (CommonClass.rBenMAPGrid != null && CommonClass.rBenMAPGrid.GridDefinitionID == Convert.ToInt32(dsRegion.Tables[0].Rows[i]["GridDefinitionID"]))
                {
                    cboRegion.SelectedIndex = i;
                    break;
                }

            }
        }
        public void OpenFile()
        {
            try
            {

                splitContainer1.Visible = true;
                CommonClass.ClearAllObject();
                CommonClass.CRSeeds = 1;
                _MapAlreadyDisplayed = false;
                ClearAll();
                
                ResetParamsTree("");

                ClearMapTableChart();
                initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);

                initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
                initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
                CommonClass.IncidencePoolingAndAggregationAdvance = null;
                if (CommonClass.IncidencePoolingAndAggregationAdvance != null)
                {
                    changeNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);

                }
                olvCRFunctionResult.SetObjects(null);
                olvIncidence.SetObjects(null);
                tlvAPVResult.SetObjects(null);

                cbPoolingWindowIncidence.Items.Clear();
                cbPoolingWindowAPV.Items.Clear();
                ClearMapTableChart();
                picGIS.Visible = true;

                SetTabControl(tabCtlReport);
                HealthImpactFunctions.MaxCRID = 0;
                BenMAP_Load(this, null);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        public void OpenFile(string filePath)
        {
            try
            {
                splitContainer1.Visible = true;
                _MapAlreadyDisplayed = false;
                ClearAll();
                ResetParamsTree(filePath);
                string chinaOrUSA = System.IO.Path.GetFileNameWithoutExtension(filePath);
                chinaOrUSA = chinaOrUSA.Substring(chinaOrUSA.LastIndexOf('_') + 1, chinaOrUSA.Length - chinaOrUSA.LastIndexOf('_') - 1);
                switch (chinaOrUSA)
                {
                    case "China":
                        System.Drawing.Image backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\RSM_PM2.5_China.jpg");
                        string shapeRoot = string.Format(@"{0}\Data\ChinaData\ShapeFile\", Application.StartupPath);
                        string shapeFile = shapeRoot + "China_Region.shp";
                        break;
                    case "USA":
                        backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\RSM_PM2.5.png");
                        shapeRoot = string.Format(@"{0}\Data\USAData\ShapeFile\", Application.StartupPath);
                        shapeFile = shapeRoot + "US_Region.shp";
                        break;
                    default:
                        backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\RSM_PM2.5_China.jpg");
                        backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\RSM_PM2.5.png");
                        shapeRoot = string.Format(@"{0}\Data\USAData\ShapeFile\", Application.StartupPath);
                        shapeFile = shapeRoot + "US_Region.shp";
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }


        private TreeNode FindNodeByText(TreeNode root, string nodeText)
        {
            if (root == null) return null;
            if (root.Text == nodeText) return root;
            TreeNode node = null;
            foreach (TreeNode tn in root.Nodes)
            {
                node = FindNodeByText(tn, nodeText);
                if (node != null) break;
            }
            return node;
        }

        public void changeBaseControlDelta()
        {
            int nodesCount = 0;
            BaseControlGroup bcg = null;
            foreach (TreeNode trchild in trvSetting.Nodes)
            {
                if (trchild.Name == "airqualitygridgroup")
                {
                    nodesCount = trchild.Nodes.Count;


                    for (int i = nodesCount - 1; i > -1; i--)
                    {
                        TreeNode node = trchild.Nodes[i];
                        if (trchild.Nodes[i].Name == "datasource")
                        {
                            if (CommonClass.LstBaseControlGroup == null) continue;
                            foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
                            {
                                if (int.Parse(node.Tag.ToString()) == b.Pollutant.PollutantID) { bcg = b; break; }
                            }

                            if (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0 && node.Nodes[0].Nodes.Count > 0)
                            {

                                node.Nodes[0].Nodes[0].ImageKey = "doc";
                                node.Nodes[0].Nodes[0].SelectedImageKey = "doc";
                            }
                            else if (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count == 0)
                            {
                                errorNodeImage(node);

                            }
                            if (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0 && node.Nodes[1].Nodes.Count > 0)
                            {
                                node.Nodes[1].Nodes[0].ImageKey = "doc";
                                node.Nodes[1].Nodes[0].SelectedImageKey = "doc";
                            }
                            else if (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count == 0)
                            {
                                errorNodeImage(node);
                            }
                            if (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0 && bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0)
                            {
                                node.Nodes[2].ImageKey = "doc";
                                node.Nodes[2].SelectedImageKey = "doc";


                            }
                            trvSetting.Refresh();
                        }
                    }
                }
            }

        }

        private bool AddDataSourceNode(BaseControlGroup bcg, TreeNode parentNode)
        {
            string nodeName = string.Empty;
            string pollutantName = string.Empty;
            try
            {
                if (bcg == null) { return false; }
                TreeNode node = new TreeNode();
                int index = 2;
                node = new TreeNode()
                {
                    Name = "datasource",
                    Tag = bcg.Pollutant.PollutantID,
                    Text = string.Format("Source of Air Quality Data ({0})", bcg.Pollutant.PollutantName),
                    ToolTipText = "Double-click to load AQ data files",
                    ImageKey = (bcg.Base == null || bcg.Control == null) ? _unreadyImageKey : _readyImageKey,
                    SelectedImageKey = (bcg.Base == null || bcg.Control == null) ? _unreadyImageKey : _readyImageKey,
                    Nodes = { new TreeNode() {
                            Name = "baseline",
                            Text = "Baseline",
                            ToolTipText="Double-click to load AQ data",
                            Tag = bcg.Base,
                            ImageKey =(bcg.Base == null || bcg.Control == null) ? _unreadyImageKey : _readyImageKey,
                            SelectedImageKey =(bcg.Base == null || bcg.Control == null) ? _unreadyImageKey : _readyImageKey
                            },
                            new TreeNode() {
                                Name = "control",
                                Text = "Control",
                                ToolTipText="Double-click to load AQ data",
                                Tag = bcg.Control,
                            ImageKey =(bcg.Base == null || bcg.Control == null) ? _unreadyImageKey : _readyImageKey,
                            SelectedImageKey =(bcg.Base == null || bcg.Control == null) ? _unreadyImageKey : _readyImageKey },
                            new TreeNode() {
                                Name = "delta",
                                Text = "Air quality delta (baseline - control)",
                                ToolTipText="Double-click AQ data file to display map/data",
                                Tag = bcg,
                             ImageKey =(bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0 && bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) ?"doc": "docgrey",
                        SelectedImageKey =(bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0 && bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) ?"doc": "docgrey" }
                }
                };
                if (bcg.Base != null)
                {
                    string s = "";
                    try
                    {
                        s = (bcg.Base as ModelDataLine).DatabaseFilePath.Substring((bcg.Base as ModelDataLine).DatabaseFilePath.LastIndexOf(@"\") + 1);

                    }
                    catch
                    { }
                    node.Nodes[0].Nodes.Add(new TreeNode()
                    {
                        Name = "basedata",
                        Text = (bcg.Base is ModelDataLine) ? s : "Base Data",
                        ToolTipText = "Double-click AQ data file to display map/data",
                        Tag = bcg.Base,
                        ImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
                        SelectedImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
                    });
                }
                if (bcg.Control != null)
                {
                    string s = "";
                    try
                    {
                        s = (bcg.Control as ModelDataLine).DatabaseFilePath.Substring((bcg.Control as ModelDataLine).DatabaseFilePath.LastIndexOf(@"\") + 1);

                    }
                    catch
                    { }
                    node.Nodes[1].Nodes.Add(new TreeNode()
                    {
                        Name = "controldata",
                        Text = (bcg.Control is ModelDataLine) ? s : "Control Data",
                        ToolTipText = "Double-click AQ data file to display map/data",
                        Tag = bcg.Control,
                        ImageKey = (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
                        SelectedImageKey = (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
                    });
                }
                parentNode.Nodes.Insert(index, node);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return false;
            }
        }

        private bool HideSplitContainer2()
        {
            try
            {
                _SavedExtent = mainMap.Extent;
                //splitContainer2.Panel1.Hide();
                splitContainer2.SplitterDistance = 50;
                //splitContainer2.SplitterDistance = 0;
                splitContainer2.BorderStyle = BorderStyle.None;
                isLegendHide = true;
                mainMap.ViewExtents = _SavedExtent;
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }
        private int iGridTypeOld = -1;
        private void trvSetting_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                iGridTypeOld = CommonClass.MainSetup.SetupID;
                TreeNode currentNode = trvSetting.SelectedNode; if (currentNode == null) { currentNode = (sender as TreeNode); }
                string nodeName = currentNode.Name.ToLower();
                string nodeTag = string.Empty;
                TreeNode parentNode = currentNode.Parent as TreeNode;
                TreeNode childNode = new TreeNode();
                TreeNode deltaNode = new TreeNode();
                DialogResult rtn = DialogResult.Cancel;
                var frm = new Form();
                BenMAPPollutant p;
                BenMAPLine bml = new BenMAPLine();
                BaseControlGroup bcg = null;
                string currStat = string.Empty;
                string msg = string.Empty;
                string str = string.Empty;
                List<ModelResultAttribute> DeltaQ = null;
                switch (nodeName)
                {
                    case "gridtype":
                        _currentNode = "gridtype";
                        if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0)
                        {
                            MessageBox.Show(string.Format("BenMAP is still creating the air quality surface map."), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        int GridTypId = -1;
                        if (CommonClass.GBenMAPGrid != null)
                            GridTypId = CommonClass.GBenMAPGrid.GridDefinitionID;
                        frm = new GridType();
                        rtn = frm.ShowDialog();
                        if (rtn != DialogResult.OK) { return; }
                        AddChildNodes(ref currentNode, currStat, currStat, bml);
                        changeNodeImage(currentNode);
                        currentNode.ExpandAll();
                        int nodesCountGrid = currentNode.Parent.Nodes.Count;

                        if (GridTypId != CommonClass.GBenMAPGrid.GridDefinitionID)
                        {
                            if (CommonClass.LstBaseControlGroup != null)
                            {
                                for (int i = 0; i < CommonClass.LstBaseControlGroup.Count; i++)
                                {
                                    CommonClass.LstBaseControlGroup[i].Base = null;
                                    CommonClass.LstBaseControlGroup[i].Control = null;
                                    GC.Collect();
                                }
                            }
                            for (int i = nodesCountGrid - 1; i > -1; i--)
                            {
                                TreeNode node = currentNode.Parent.Nodes[i];
                                if (currentNode.Parent.Nodes[i].Name == "datasource")
                                {

                                    currentNode.Parent.Nodes[i].Nodes[0].Nodes.Clear();
                                    initNodeImage(currentNode.Parent.Nodes[i].Nodes[0]);
                                    currentNode.Parent.Nodes[i].Nodes[1].Nodes.Clear();
                                    initNodeImage(currentNode.Parent.Nodes[i].Nodes[1]);
                                    initNodeImage(currentNode.Parent.Nodes[i]);
                                }
                            }



                            if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null)
                            {
                                CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue = null;
                            }
                            olvCRFunctionResult.SetObjects(null);
                            olvIncidence.SetObjects(null);
                            tlvAPVResult.SetObjects(null);

                            cbPoolingWindowIncidence.Items.Clear();
                            cbPoolingWindowAPV.Items.Clear();
                            ClearMapTableChart();
                            ClearMapTableChart();
                            CommonClass.BenMAPPopulation = null;
                            initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes[0]);
                            if (CommonClass.BaseControlCRSelectFunction != null)
                            {
                                errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes.Count - 1]);
                            }
                            if (CommonClass.lstIncidencePoolingAndAggregation != null)
                            {
                                errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
                            }
                            if (CommonClass.ValuationMethodPoolingAndAggregation != null)
                            {
                                errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
                            }
                        }
                        break;
                    case "grid":
                        _currentNode = "grid";
                        mainMap.Layers.Clear();
                        HideSplitContainer2();
                        mainMap.ProjectionModeReproject = ActionMode.Never;
                        mainMap.ProjectionModeDefine = ActionMode.Never;
                        tabCtlMain.SelectedIndex = 0;
                        PolygonLayer player = null; //mainMap.GetAllLayers()[0] as PolygonLayer; //may not work if there aren't any layers
                        if (currentNode.Tag is ShapefileGrid)
                        {
                            mainMap.Layers.Clear();
                            if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (currentNode.Tag as ShapefileGrid).ShapefileName + ".shp"))
                            {
                                player = (PolygonLayer)mainMap.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (currentNode.Tag as ShapefileGrid).ShapefileName + ".shp");
                            
                            }
                        }
                        else if (currentNode.Tag is RegularGrid)
                        {
                            mainMap.Layers.Clear();
                            if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (currentNode.Tag as RegularGrid).ShapefileName + ".shp"))
                            {
                                player = (PolygonLayer)mainMap.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (currentNode.Tag as RegularGrid).ShapefileName + ".shp");
                            }
                        }
                        //PolygonLayer player = mainMap.Layers[0] as PolygonLayer;
                        float f = (float)0.9;
                        Color c = Color.Transparent;
                        PolygonSymbolizer Transparent = new PolygonSymbolizer(c);

                        Transparent.OutlineSymbolizer = new LineSymbolizer(Color.DarkBlue, 1); player.Symbolizer = Transparent;
                        LayerObject = null;
                        break;
                    case "region":
                        _currentNode = "region";
                        HideSplitContainer2();
                        _MapAlreadyDisplayed = false;
                        mainMap.Layers.Clear();
                        tabCtlMain.SelectedIndex = 0;
                        addRegionLayerGroupToMainMap();
                        LayerObject = null;
                        break;
                    case "pollutant":
                        _currentNode = "pollutant";
                        if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0)
                        {
                            MessageBox.Show(string.Format("BenMAP is still creating the air quality surface map."), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        BenMAPPollutant[] benMAPPollutantArray = null; 
                        if (CommonClass.LstPollutant != null)
                        {
                            benMAPPollutantArray = CommonClass.LstPollutant.ToArray();
                        }

                        frm = new PollutantMulti();
                        rtn = frm.ShowDialog();
                        if (rtn != DialogResult.OK)
                        {
                            if (benMAPPollutantArray != null)
                            {
                                CommonClass.LstPollutant = new List<BenMAPPollutant>();
                                CommonClass.LstPollutant = benMAPPollutantArray.ToList();
                            }
                            else
                                CommonClass.LstPollutant = new List<BenMAPPollutant>();

                            return;
                        }
                        changeNodeImage(currentNode);
                        int nodesCount = currentNode.Parent.Nodes.Count;
                        if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0)  //the branch under this if statement never called as far as I can tell????-MCB
                        {
                            for (int i = nodesCount - 2; i > -1; i--)
                            {
                                TreeNode node = currentNode.Parent.Nodes[i];
                                if (currentNode.Parent.Nodes[i].Name == "datasource") { currentNode.Parent.Nodes.RemoveAt(i); }
                            }
                            for (int i = nodesCount - 1; i > -1; i--)
                            {
                                TreeNode node = currentNode.Parent.Nodes[i];
                                if (currentNode.Parent.Nodes[i].Name == "datasource")
                                {
                                    currentNode.Parent.Nodes[i].Text = "Source of Air Quality Data";
                                    currentNode.Parent.Nodes[i].Nodes[0].Tag = null;
                                    currentNode.Parent.Nodes[i].Nodes[0].Nodes.Clear();
                                    currentNode.Parent.Nodes[i].Nodes[1].Tag = null;
                                    currentNode.Parent.Nodes[i].Nodes[1].Nodes.Clear();
                                    currentNode.Parent.Nodes[i].Nodes.RemoveAt(2);
                                }
                            }
                            CommonClass.LstBaseControlGroup.Clear();
                            CommonClass.LstCreateShapeFileParams.Clear();
                            CommonClass.BaseControlCRSelectFunction = null;
                            CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
                            CommonClass.lstIncidencePoolingAndAggregation = null;
                            CommonClass.IncidencePoolingResult = null;
                            CommonClass.ValuationMethodPoolingAndAggregation = null;
                            olvCRFunctionResult.SetObjects(null);
                            olvIncidence.SetObjects(null);
                            tlvAPVResult.SetObjects(null);

                            cbPoolingWindowIncidence.Items.Clear();
                            cbPoolingWindowAPV.Items.Clear();
                            ClearMapTableChart();

                            //Update tree node symbols
                            initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);
                            foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes) //turn yellow the pooled nodes
                            {
                                initNodeImage(tn);
                            }
                            foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes)  //turn yellow the population nodes
                            {
                                initNodeImage(tn);
                            }

                            return;
                        }
                        else
                        {
                  
                            if (benMAPPollutantArray == null || (benMAPPollutantArray != null && CommonClass.lstPollutantAll.Count != benMAPPollutantArray.Count()) ||
                            (benMAPPollutantArray != null && benMAPPollutantArray.ToList().Select(pp => pp.PollutantID).ToList() != CommonClass.lstPollutantAll.Select(ppp => ppp.PollutantID).ToList()))
                            {
                                currentNode.Tag = CommonClass.LstPollutant;
                                CommonClass.LstCreateShapeFileParams = null;
                                List<BaseControlGroup> ExtraListBCG = new List<BaseControlGroup>(CommonClass.LstPollutant.Count + 1); 
                                List<BenMAPPollutant> MissingLstPollutant = new List<BenMAPPollutant>(CommonClass.LstPollutant.Count);

                                //removes the pollunat template node in the pollutant area if it exists
                                for (int i = nodesCount - 1; i > -1; i--)
                                {
                                    TreeNode node = currentNode.Parent.Nodes[i];
                                    if (currentNode.Parent.Nodes[i].Name == "datasource" &&  currentNode.Parent.Nodes[i].Text == "Source of Air Quality Data")
                                    { 
                                        currentNode.Parent.Nodes.RemoveAt(i); 
                                    }
                                }
                               
                                //check for extra bcgs and remove if found
                                if (CommonClass.LstBaseControlGroup != null)  //Pollutants have been added earlier already, add these to the existing pollutants
                                {
                                    foreach (BaseControlGroup testbcg in CommonClass.LstBaseControlGroup)  //look for matching pollutant in pollutant list
                                    {    
                                        bool PopulatedPollutantsAlreadyExist = false;
                                        foreach (BenMAPPollutant BMpol in CommonClass.LstPollutant)
                                        {
                                            if (testbcg.Pollutant != null)
                                            {
                                                if (BMpol.PollutantID == testbcg.Pollutant.PollutantID)
                                                {
                                                    PopulatedPollutantsAlreadyExist = true;
                                                    break;
                                                }
                                            }
                                        }

                                        if (!PopulatedPollutantsAlreadyExist)  //can't find it in pollutant list, so it must be an extra bcg record
                                        {
                                            ExtraListBCG.Add(testbcg);  
                                        }

                                    }

                                    if (ExtraListBCG.Count > 0)                 //remove extra bcg records
                                    {
                                        foreach (BaseControlGroup extrabcg in ExtraListBCG)
                                        {                                              
                                            //remove this pollutant node
                                            //refresh node count in case a node was removed above
                                            nodesCount = currentNode.Parent.Nodes.Count; 
                                            for (int i = nodesCount - 1; i > -1; i--)
                                            {
                                                TreeNode node = currentNode.Parent.Nodes[i];
                                                if (currentNode.Parent.Nodes[i].Name == "datasource" && (int)currentNode.Parent.Nodes[i].Tag == extrabcg.Pollutant.PollutantID)
                                                {
                                                    currentNode.Parent.Nodes.RemoveAt(i);
                                                }
                                            } 
                                            //remove this pollutant's bcg record too
                                            CommonClass.LstBaseControlGroup.Remove(extrabcg);
                                        }
                                    }

                                    //Find any missing bcgs and add them
                                    foreach (BenMAPPollutant BMpol in CommonClass.LstPollutant)  //look for matching pollutant in bcg records
                                    {
                                        bool PopulatedPollutantsAlreadyExist = false;
                                        foreach (BaseControlGroup testBCG in CommonClass.LstBaseControlGroup)
                                        {

                                            if (testBCG.Pollutant.PollutantID == BMpol.PollutantID)
                                            {
                                                PopulatedPollutantsAlreadyExist = true;
                                                break;
                                            }
                                        }
                                        if (!PopulatedPollutantsAlreadyExist)  //can't find match so need to add a bcg record for this pollutant
                                        {
                                            MissingLstPollutant.Add(BMpol);
                                        }

                                    }
                                    if (MissingLstPollutant.Count > 0)                 //Add missing bcg records
                                    {
                                        foreach (BenMAPPollutant missingPol in MissingLstPollutant)
                                        {
                                            p = missingPol;
                                            bcg = new BaseControlGroup() { GridType = CommonClass.GBenMAPGrid, Pollutant = p };
                                            CommonClass.LstBaseControlGroup.Add(bcg);
                                            AddDataSourceNode(bcg, currentNode.Parent);
                                            //turn the new pollutant header node yellow.
                                            initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);
                                        }
                                    }
                          

                                }
                                else
                                {                                
                                    CommonClass.LstBaseControlGroup = null;
                                    CommonClass.LstCreateShapeFileParams = null;
                                    GC.Collect();

                                    //rebuilds the polluntant section of the tree
                                    CommonClass.LstBaseControlGroup = new List<BaseControlGroup>(CommonClass.LstPollutant.Count);
                                    for (int i = CommonClass.LstPollutant.Count - 1; i > -1; i--)
                                    {
                                       

                                        p = CommonClass.LstPollutant[i];
                                        bcg = new BaseControlGroup() { GridType = CommonClass.GBenMAPGrid, Pollutant = p };
                                        CommonClass.LstBaseControlGroup.Add(bcg);
                                        AddDataSourceNode(bcg, currentNode.Parent);

                                        //turn the new pollutant header node yellow.
                                        initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);
                                    }
                                }
                                //turn all nodes after BCDG to yellow (AKA unready)
                                foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes) //turn yellow the pooled nodes
                                {
                                    initNodeImage(tn);
                                }
                                foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes)  //turn yellow the population nodes
                                {
                                    initNodeImage(tn);
                                }
                                //Assumes everything else is unready
                                CommonClass.BaseControlCRSelectFunction = null;
                                CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
                                CommonClass.lstIncidencePoolingAndAggregation = null;
                                CommonClass.IncidencePoolingResult = null;
                                CommonClass.ValuationMethodPoolingAndAggregation = null;
                                //ClearMapTableChart();

                                CommonClass.BenMAPPopulation = null;
                                CommonClass.IncidencePoolingAndAggregationAdvance = null;

                                olvCRFunctionResult.SetObjects(null);
                                olvIncidence.SetObjects(null);
                                tlvAPVResult.SetObjects(null);

                                cbPoolingWindowIncidence.Items.Clear();
                                cbPoolingWindowAPV.Items.Clear();
                                ClearMapTableChart();
                            }
                        }
                        currentNode.Parent.ExpandAll();

                        break;
                    case "datasource":
                        _currentNode = "datasource";
                        TreeNode pNode = currentNode.Parent;
                        BaseControlGroup bcgLoadAQG = new BaseControlGroup();
                        if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0)
                        {
                            CommonClass.LstBaseControlGroup = new List<BaseControlGroup>();
                            CommonClass.LstBaseControlGroup.Add(new BaseControlGroup());
                            bcgLoadAQG = CommonClass.LstBaseControlGroup[0];
                            currentNode.Tag = null;

                        }
                        string currentNodeTag = currentNode.Tag != null ? currentNode.Tag.ToString() : "";
                        if (currentNodeTag != "")
                        {
                            foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
                            {
                                if (int.Parse(currentNodeTag) == b.Pollutant.PollutantID) { bcgLoadAQG = b; break; }
                            }
                        }
                        if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0 && bcgLoadAQG.Pollutant != null)
                        {
                            lock (CommonClass.LstAsynchronizationStates)
                            {
                                if (CommonClass.LstAsynchronizationStates.Contains(string.Format("{0}{1}", bcgLoadAQG.Pollutant.PollutantName.ToLower(), "baseline"))
                                    || CommonClass.LstAsynchronizationStates.Contains(string.Format("{0}{1}", bcgLoadAQG.Pollutant.PollutantName.ToLower(), "control")))
                                {
                                    msg = " BenMAP is still creating the air quality surface map.";
                                    MessageBox.Show(msg);
                                    return;
                                }
                            }
                        }
                        OpenExistingAQG openAQG = new OpenExistingAQG(bcgLoadAQG);
                        openAQG.ShowDialog();
                        if (openAQG.DialogResult != DialogResult.OK) { return; }
                        bcgLoadAQG.DeltaQ = null;
                        if (openAQG.isGridTypeChanged)
                        {
                            if (CommonClass.LstBaseControlGroup != null)
                            {
                                for (int i = 0; i < CommonClass.LstBaseControlGroup.Count; i++)
                                {
                                    if (currentNodeTag != null && currentNodeTag.ToString() != "" && CommonClass.LstBaseControlGroup[i].Pollutant.PollutantID != int.Parse(currentNodeTag))
                                    {
                                        CommonClass.LstBaseControlGroup[i].Base = null;
                                        CommonClass.LstBaseControlGroup[i].Control = null;
                                        GC.Collect();
                                    }
                                }
                            }

                            for (int i = currentNode.Parent.Nodes.Count - 1; i > -1; i--)
                            {
                                TreeNode node = currentNode.Parent.Nodes[i];
                                if (currentNode.Parent.Nodes[i].Name == "datasource")
                                {
                                    if (currentNodeTag != null && currentNodeTag.ToString() != "" && int.Parse(currentNodeTag) != int.Parse(currentNode.Parent.Nodes[i].Tag.ToString()))
                                    {
                                        currentNode.Parent.Nodes[i].Nodes[0].Nodes.Clear();
                                        initNodeImage(currentNode.Parent.Nodes[i].Nodes[0]);
                                        currentNode.Parent.Nodes[i].Nodes[1].Nodes.Clear();
                                        initNodeImage(currentNode.Parent.Nodes[i].Nodes[1]);
                                        initNodeImage(currentNode.Parent.Nodes[i]);
                                    }
                                }
                            }


                            if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null)
                            {
                                CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue = null;
                            }

                            olvCRFunctionResult.SetObjects(null);
                            olvIncidence.SetObjects(null);
                            tlvAPVResult.SetObjects(null);

                            cbPoolingWindowIncidence.Items.Clear();
                            cbPoolingWindowAPV.Items.Clear();
                            ClearMapTableChart();
                            CommonClass.BenMAPPopulation = null;
                            initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes[0]);
                            if (CommonClass.BaseControlCRSelectFunction != null)
                            {
                                errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes.Count - 1]);
                            }
                            if (CommonClass.lstIncidencePoolingAndAggregation != null)
                            {
                                errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
                            }
                            if (CommonClass.ValuationMethodPoolingAndAggregation != null)
                            {
                                errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
                            }
                        }
                        //bcgLoadAQG = openAQG.bcgOpenAQG;
                        //int index = currentNode.Index;

                        //BrushBaseControl(ref pNode, bcgLoadAQG, index);
                        foreach(BaseControlGroup b in CommonClass.LstBaseControlGroup) {
                            if(b.Base != null && b.Control != null) {
                                foreach (TreeNode n in pNode.Nodes)
                                {
                                    if(n.Name != "pollutant")
                                    {
                                        if (int.Parse(n.Tag.ToString()) == b.Pollutant.PollutantID)
                                        {
                                            BrushBaseControl(ref pNode, b, n.Index);
                                            break;
                                        }
                                    }

                                }
                            }
                        }

                        break;
                    case "baseline":
                        _currentNode = "baseline";
                        currStat = "baseline";

                        bool BCResultOK = BaseControlOP(currStat, ref currentNode);
                        //if (BCResultOK)
                        //{
                        //    //Advance to first child node and draw base data layer-MCB
                        //    childNode = currentNode.FirstNode;  //refresh child node
                        //    if (childNode != null)
                        //    {
                        //        currentNode = childNode;
                        //        trvSetting.SelectedNode = currentNode;
                        //        nodeName = currentNode.Name.ToLower();
                        //        DrawBaseline(currentNode, str);
                        //    }
                        //}
                        break;
                    case "basedata":
                        DrawBaseline(currentNode, str); //-MCB
                        break;
                    case "delta":
                        DrawDelta(currentNode, str);
                        break;

                    case "control":
                        _currentNode = "control";
                        currStat = "control";
                        bool BCResultOK2 = BaseControlOP(currStat, ref currentNode);
                        //if (BCResultOK2)
                        //{
                        //    //Advance to first child node and draw control data layer-MCB
                        //    childNode = currentNode.FirstNode;
                        //    if (childNode != null)
                        //    {
                        //        currentNode = childNode;
                        //        trvSetting.SelectedNode = currentNode;
                        //        nodeName = currentNode.Name.ToLower();
                        //        DrawControlData(currentNode, str);
                        //        //Attempt to display the delta layer as well-MCB
                        //        //NOTE-uncomment when multiple layers can be displayed at once-MCB
                        //        //deltaNode = parentNode.LastNode as TreeNode;
                        //        //if (deltaNode != null)
                        //        //{
                        //        //    currentNode = deltaNode;
                        //        //    trvSetting.SelectedNode = currentNode;
                        //        //    nodeName = currentNode.Name.ToLower();
                        //        //    DrawDelta(currentNode, str);
                        //        //}
                        //    }
                        //}
                        break;

                    case "controldata":              
                        DrawControlData(currentNode, str); //-MCB
                        break;

                    case "configuration":
                        _currentNode = "gridtype";
                        frm = new OpenExistingConfiguration();
                        rtn = frm.ShowDialog();
                        if (rtn != DialogResult.OK) { return; }
                        if ((frm as OpenExistingConfiguration).strCRPath != "")
                        {
                            WaitShow("Loading configuration results file");
                            try
                            {
                                CommonClass.ClearAllObject();
                                string err = "";
                                CommonClass.BaseControlCRSelectFunctionCalculateValue = Configuration.ConfigurationCommonClass.LoadCFGRFile((frm as OpenExistingConfiguration).strCRPath, ref err);
                                if (CommonClass.BaseControlCRSelectFunctionCalculateValue == null)
                                {
                                    System.Threading.Thread.Sleep(300);
                                    WaitClose();
                                    MessageBox.Show(err);
                                    return;
                                }

                                if (CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue == null)
                                {
                                    System.Threading.Thread.Sleep(300); WaitClose();
                                    MessageBox.Show("BenMAP-CE was unable to open the file. The file may be corrupt, or it may have been created using a previous incompatible version of BenMAP-CE.");
                                    return;
                                }
                                CommonClass.BaseControlCRSelectFunction = null;
                                CommonClass.BaseControlCRSelectFunction = new BaseControlCRSelectFunction();
                                CommonClass.BaseControlCRSelectFunction.BaseControlGroup = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup;
                                CommonClass.BaseControlCRSelectFunction.BenMAPPopulation = CommonClass.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
                                CommonClass.BaseControlCRSelectFunction.CRLatinHypercubePoints = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints;
                                CommonClass.BaseControlCRSelectFunction.CRRunInPointMode = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRRunInPointMode;
                                CommonClass.BaseControlCRSelectFunction.CRSeeds = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRSeeds;
                                CommonClass.BaseControlCRSelectFunction.CRThreshold = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRThreshold;
                                CommonClass.BaseControlCRSelectFunction.RBenMapGrid = CommonClass.BaseControlCRSelectFunctionCalculateValue.RBenMapGrid;
                                CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction = new List<CRSelectFunction>();
                                CommonClass.PollutantGroup = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[0].CRSelectFunction.BenMAPHealthImpactFunction.PollutantGroup;

                                CommonClass.MainSetup = CommonClass.getBenMAPSetupFromID(CommonClass.BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID);

                                for (int i = 0; i < CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; i++)
                                {
                                    CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Add(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[i].CRSelectFunction);
                                }
                                try
                                {
                                    if (CommonClass.BaseControlCRSelectFunction != null)
                                    {
                                        showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, currentNode);
                                        tabCtlReport.SelectedIndex = 0;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    Logger.LogError(ex);
                                }

                                olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
                                CommonClass.ValuationMethodPoolingAndAggregation = null;
                                CommonClass.lstIncidencePoolingAndAggregation = null;
                                CommonClass.IncidencePoolingResult = null;
                                CommonClass.GBenMAPGrid = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.First().GridType;
                                foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes)
                                {
                                    initNodeImage(tn);
                                }

                                ConfigurationResultsReport frmColumn = new ConfigurationResultsReport();
                                frmColumn.userAssignPercentile = false;
                                strHealthImpactPercentiles = null;

                                olvIncidence.SetObjects(null);
                                tlvAPVResult.SetObjects(null);
                                cbPoolingWindowIncidence.Items.Clear();
                                cbPoolingWindowAPV.Items.Clear();
                                ClearMapTableChart();
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(ex);
                            }
                            WaitClose();
                        }
                        else
                        {
                            try
                            {
                                if (CommonClass.BaseControlCRSelectFunction != null)
                                {
                                    CommonClass.MainSetup = CommonClass.getBenMAPSetupFromID(CommonClass.BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID);

                                    showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, currentNode);
                                    olvIncidence.SetObjects(null);
                                    tlvAPVResult.SetObjects(null);
                                    cbPoolingWindowIncidence.Items.Clear();
                                    cbPoolingWindowAPV.Items.Clear();
                                    ClearMapTableChart();
                                    CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
                                    CommonClass.lstIncidencePoolingAndAggregation = null;
                                    CommonClass.IncidencePoolingResult = null;
                                    CommonClass.ValuationMethodPoolingAndAggregation = null;

                                    GC.Collect();

                                    olvCRFunctionResult.SetObjects(null);
                                    foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes)
                                    {
                                        initNodeImage(tn);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        SetTabControl(tabCtlReport);
                        break;
                    case "latinhypercubepoints":
                        _currentNode = "latinhypercubepoints";
                        frm = new LatinHypercubePoints();
                        (frm as LatinHypercubePoints).LatinHypercubePointsCount = CommonClass.CRLatinHypercubePoints;
                        (frm as LatinHypercubePoints).IsRunInPointMode = CommonClass.CRRunInPointMode;
                        (frm as LatinHypercubePoints).Threshold = CommonClass.CRThreshold;
                        rtn = frm.ShowDialog();
                        if (rtn != DialogResult.OK) { return; }
                        CommonClass.CRLatinHypercubePoints = (frm as LatinHypercubePoints).LatinHypercubePointsCount;
                        CommonClass.CRRunInPointMode = (frm as LatinHypercubePoints).IsRunInPointMode;
                        CommonClass.CRThreshold = (frm as LatinHypercubePoints).Threshold;
                        changeNodeImage(currentNode);

                        break;
                    case "populationdataset":
                        _currentNode = "populationdataset";
                        frm = new PopulationDataset();
                        (frm as PopulationDataset).BenMAPPopulation = CommonClass.BenMAPPopulation;
                        rtn = frm.ShowDialog();
                        if (rtn != DialogResult.OK) { return; }
                        if (CommonClass.BenMAPPopulation == null || CommonClass.BenMAPPopulation.DataSetID != (frm as PopulationDataset).BenMAPPopulation.DataSetID ||
                            CommonClass.BenMAPPopulation.Year != (frm as PopulationDataset).BenMAPPopulation.Year)
                        {
                            Configuration.ConfigurationCommonClass.DicGrowth = null;
                            Configuration.ConfigurationCommonClass.DicWeight = null;
                        }
                        if (CommonClass.BenMAPPopulation != null && (CommonClass.BenMAPPopulation.DataSetID != (frm as PopulationDataset).BenMAPPopulation.DataSetID ||
                            CommonClass.BenMAPPopulation.Year != (frm as PopulationDataset).BenMAPPopulation.Year))
                        {
                            if (currentNode.Parent.Nodes[currentNode.Parent.Nodes.Count - 1].ImageKey != _errorImageKey)
                                initNodeImage(currentNode.Parent.Nodes[currentNode.Parent.Nodes.Count - 1]);
                        }
                        CommonClass.BenMAPPopulation = (frm as PopulationDataset).BenMAPPopulation;
                        changeNodeImage(currentNode);

                        break;
                    case "healthimpactfunctions":
                        _currentNode = "healthimpactfunctions";
                        if (CommonClass.GBenMAPGrid == null)
                        {
                            MessageBox.Show("Please select an air quality surface.");
                            return;
                        }
                        if (CommonClass.LstPollutant == null)
                        {
                            MessageBox.Show("Please select a pollutant.");
                            return;
                        }
                        if (CommonClass.LstBaseControlGroup == null)
                        {
                            MessageBox.Show("Please define baseline and control air quality surfaces.");
                            return;
                        }
                        foreach (BaseControlGroup bcGroup in CommonClass.LstBaseControlGroup)
                        {
                            if (bcGroup.Base == null || bcGroup.Control == null || bcGroup.Base.ModelResultAttributes == null ||
                                  bcGroup.Control.ModelResultAttributes == null)
                            {
                                MessageBox.Show("Please define baseline and control air quality surfaces.");
                                return;
                            }
                        }
                        if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0)
                        {
                            MessageBox.Show("Baseline or control air quality surface is being created. Please wait.");
                            return;
                        }
                        if (CommonClass.BenMAPPopulation == null)
                        {
                            MessageBox.Show("Please select population dataset and year.");
                            return;
                        }
                        frm = new HealthImpactFunctions();
                        rtn = frm.ShowDialog();
                        if (rtn != DialogResult.OK)
                        {
                            return;
                        }
                        ConfigurationResultsReport frmReport = new ConfigurationResultsReport();
                        frmReport.userAssignPercentile = false;
                        strHealthImpactPercentiles = null;

                        olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
                        changeNodeImage(currentNode);
                        if (CommonClass.ValuationMethodPoolingAndAggregation != null)
                        {
                            CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue;
                            CommonClass.ValuationMethodPoolingAndAggregation.CFGRPath = "";
                            foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                            {
                                foreach (AllSelectCRFunction alsc in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.ToList())
                                {
                                    if (alsc.PoolingMethod == "")
                                    {
                                        try
                                        {
                                            alsc.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(a => a.CRSelectFunction.CRID == alsc.CRID).First();
                                        }
                                        catch
                                        {
                                            alsc.CRSelectFunctionCalculateValue = null;
                                        }
                                    }
                                    else
                                    {
                                        alsc.CRSelectFunctionCalculateValue = null;
                                    }

                                }
                            }


                            if (trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 2].ImageKey == _readyImageKey)
                            {
                                errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 2]);
                            }
                            if (trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1].ImageKey == _readyImageKey)
                            {
                                errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
                            }

                        }
                        olvIncidence.SetObjects(null);
                        tlvAPVResult.SetObjects(null);

                        cbPoolingWindowIncidence.Items.Clear();
                        cbPoolingWindowAPV.Items.Clear();
                        ClearMapTableChart();
                        changeNodeImage(currentNode);
                        SetTabControl(tabCtlReport);
                        break;
                    case "aggregationpoolingvaluation":
                        _currentNode = "aggregationpoolingvaluation";
                        frm = new OpenExistingAPVConfiguration();
                        rtn = frm.ShowDialog();
                        if (rtn != DialogResult.OK) { return; }
                        if ((frm as OpenExistingAPVConfiguration).strCRPath != "")
                        {
                            WaitShow("Loading configuration results file ");
                            try
                            {
                                string err = "";
                                CommonClass.BaseControlCRSelectFunctionCalculateValue = Configuration.ConfigurationCommonClass.LoadCFGRFile((frm as OpenExistingAPVConfiguration).strCRPath, ref err);
                                if (CommonClass.BaseControlCRSelectFunctionCalculateValue == null)
                                {
                                    MessageBox.Show(err);
                                    return;
                                }
                                CommonClass.BaseControlCRSelectFunction = null;
                                CommonClass.BaseControlCRSelectFunction = new BaseControlCRSelectFunction();
                                CommonClass.BaseControlCRSelectFunction.BaseControlGroup = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup;
                                CommonClass.BaseControlCRSelectFunction.BenMAPPopulation = CommonClass.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
                                CommonClass.BaseControlCRSelectFunction.CRLatinHypercubePoints = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints;
                                CommonClass.BaseControlCRSelectFunction.CRRunInPointMode = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRRunInPointMode;
                                CommonClass.BaseControlCRSelectFunction.CRSeeds = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRSeeds;
                                CommonClass.BaseControlCRSelectFunction.CRThreshold = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRThreshold;
                                CommonClass.BaseControlCRSelectFunction.RBenMapGrid = CommonClass.BaseControlCRSelectFunctionCalculateValue.RBenMapGrid;
                                CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction = new List<CRSelectFunction>();
                                CommonClass.MainSetup = CommonClass.getBenMAPSetupFromID(CommonClass.BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID);

                                for (int i = 0; i < CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; i++)
                                {
                                    CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Add(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[i].CRSelectFunction);
                                }
                                try
                                {
                                    if (CommonClass.BaseControlCRSelectFunction != null)
                                    {
                                        showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, currentNode);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    Logger.LogError(ex);
                                }

                                olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
                                CommonClass.ValuationMethodPoolingAndAggregation = null;
                                CommonClass.lstIncidencePoolingAndAggregation = null;
                                CommonClass.IncidencePoolingResult = null;
                                foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes)
                                {
                                    initNodeImage(tn);
                                }
                                frmReport = new ConfigurationResultsReport();
                                frmReport.userAssignPercentile = false;
                                strHealthImpactPercentiles = null;
                                strPoolIncidencePercentiles = null;

                                olvIncidence.SetObjects(null);
                                tlvAPVResult.SetObjects(null);

                                cbPoolingWindowIncidence.Items.Clear();
                                cbPoolingWindowAPV.Items.Clear();
                                ClearMapTableChart();
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(ex);
                            }
                            WaitClose();
                        }
                        else if ((frm as OpenExistingAPVConfiguration).strAPVPath != "")
                        {
                            if (((frm as OpenExistingAPVConfiguration).strAPVPath.Substring(((frm as OpenExistingAPVConfiguration).strAPVPath.Length - 5), 5)) == "apvrx")
                            {
                                WaitShow("Loading APV results file ");
                            }
                            else
                            {
                                WaitShow("Loading APV configuration file ");
                            }
                            CommonClass.ValuationMethodPoolingAndAggregation = null;
                            CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
                            CommonClass.lstIncidencePoolingAndAggregation = null;
                            CommonClass.LstBaseControlGroup = null;
                            CommonClass.LstCreateShapeFileParams = null;
                            CommonClass.BaseControlCRSelectFunction = null;
                            GC.Collect();
                            CommonClass.ClearAllObject();
                            string err = "";
                            CommonClass.ValuationMethodPoolingAndAggregation = APVX.APVCommonClass.loadAPVRFile((frm as OpenExistingAPVConfiguration).strAPVPath, ref err);
                            if (CommonClass.ValuationMethodPoolingAndAggregation == null)
                            {
                                WaitClose();
                                MessageBox.Show(err);
                                return;
                            }
                            foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                            {
                                for (int iVB = 0; iVB < vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Count; iVB++)
                                {
                                    if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[iVB].EndPointGroup == null)
                                    {
                                        vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[iVB].EndPointGroup = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[0].EndPointGroup;
                                        vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[iVB].EndPointGroupID = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[0].EndPointGroupID;

                                    }
                                }
                                if (vb.LstAllSelectValuationMethod != null)
                                {
                                    for (int iVB = 0; iVB < vb.LstAllSelectValuationMethod.Count; iVB++)
                                    {
                                        if (vb.LstAllSelectValuationMethod[iVB].EndPointGroup == null)
                                        {
                                            vb.LstAllSelectValuationMethod[iVB].EndPointGroup = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[0].EndPointGroup;

                                        }
                                    }
                                }
                            }
                            CommonClass.IncidencePoolingAndAggregationAdvance = CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance;
                            CommonClass.BaseControlCRSelectFunctionCalculateValue = CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue;
                            CommonClass.BaseControlCRSelectFunction = null;
                            CommonClass.BaseControlCRSelectFunction = new BaseControlCRSelectFunction();
                            CommonClass.BaseControlCRSelectFunction.BaseControlGroup = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup;
                            CommonClass.BaseControlCRSelectFunction.BenMAPPopulation = CommonClass.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
                            CommonClass.BaseControlCRSelectFunction.CRLatinHypercubePoints = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints;
                            CommonClass.BaseControlCRSelectFunction.CRRunInPointMode = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRRunInPointMode;
                            CommonClass.BaseControlCRSelectFunction.CRSeeds = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRSeeds;
                            CommonClass.BaseControlCRSelectFunction.CRThreshold = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRThreshold;
                            CommonClass.BaseControlCRSelectFunction.RBenMapGrid = CommonClass.BaseControlCRSelectFunctionCalculateValue.RBenMapGrid;
                            CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction = new List<CRSelectFunction>();
                            CommonClass.MainSetup = CommonClass.getBenMAPSetupFromID(CommonClass.BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID);

                            for (int i = 0; i < CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; i++)
                            {
                                CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Add(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[i].CRSelectFunction);
                            }
                            try
                            {
                                if (CommonClass.BaseControlCRSelectFunction != null)
                                {
                                    showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, currentNode);

                                }
                            }
                            catch (Exception ex)
                            {
                            }
                            if (((frm as OpenExistingAPVConfiguration).strAPVPath.Substring(((frm as OpenExistingAPVConfiguration).strAPVPath.Length - 5), 5)) == "apvrx")
                            {
                                olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);

                            }
                            else
                            {
                                olvCRFunctionResult.SetObjects(null);
                            }

                            CommonClass.lstIncidencePoolingAndAggregation = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Select(a => a.IncidencePoolingAndAggregation).ToList();
                            CommonClass.IncidencePoolingAndAggregationAdvance = CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance;
                            cbPoolingWindowAPV.Items.Clear();

                            olvIncidence.SetObjects(null);
                            tlvAPVResult.SetObjects(null);

                            cbPoolingWindowIncidence.Items.Clear();
                            cbPoolingWindowAPV.Items.Clear();
                            ClearMapTableChart();
                            if ((frm as OpenExistingAPVConfiguration).strAPVPath.Substring((frm as OpenExistingAPVConfiguration).strAPVPath.Count() - 5, 5).ToLower() == "apvrx")
                            {
                                if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null
&& CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID && (CommonClass.lstCRResultAggregation == null || CommonClass.lstCRResultAggregation.Count == 0))
                                {
                                    CommonClass.lstCRResultAggregation = new List<CRSelectFunctionCalculateValue>();
                                    foreach (CRSelectFunctionCalculateValue crv in CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
                                    {
                                        CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
                                    }
                                }
                                foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                                {
                                    bool bHavePooling = false;
                                    foreach (AllSelectCRFunction alsc in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(pa => pa.PoolingMethod == "").ToList())
                                    {
                                        if (alsc.PoolingMethod == "")
                                        {
                                            try
                                            {
                                                if (bHavePooling == false && alsc.CRSelectFunctionCalculateValue != null && alsc.CRSelectFunctionCalculateValue.CRCalculateValues != null && alsc.CRSelectFunctionCalculateValue.CRCalculateValues.Count > 0)
                                                {
                                                    bHavePooling = true;
                                                }
                                                if (CommonClass.lstCRResultAggregation == null || CommonClass.lstCRResultAggregation.Count == 0)
                                                {
                                                    alsc.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(pa => pa.CRSelectFunction.CRID == alsc.CRID).First();
                                                }
                                                else
                                                {
                                                    alsc.CRSelectFunctionCalculateValue = CommonClass.lstCRResultAggregation.Where(pa => pa.CRSelectFunction.CRID == alsc.CRID).First();
                                                }
                                            }
                                            catch
                                            {
                                            }
                                        }
                                        else
                                        {
                                            alsc.CRSelectFunctionCalculateValue = null;
                                        }
                                    }

                                    if (bHavePooling == false)
                                    {
                                        List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
                                        if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().PoolingMethod == "None")
                                        {
                                            APVX.APVCommonClass.getAllChildCRNotNoneForPooling(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First(), vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);

                                        }
                                        lstCR.Insert(0, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First());
                                        if (lstCR.Count == 1 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID < 9999 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID > 0) { }
                                        else
                                        {
                                            APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(true, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(pa => pa.NodeType != 100).Max(pa => pa.NodeType), vb.IncidencePoolingAndAggregation.lstColumns);
                                        }
                                    }
                                }
                                foreach (ValuationMethodPoolingAndAggregationBase vbAPVFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                                {
                                    cbPoolingWindowAPV.Items.Add(vbAPVFrom.IncidencePoolingAndAggregation.PoolingName);
                                }
                                cbPoolingWindowAPV.SelectedIndex = 0;
                                cbPoolingWindowIncidence.Items.Clear();
                                foreach (ValuationMethodPoolingAndAggregationBase vbAPVFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                                {
                                    this.cbPoolingWindowIncidence.Items.Add(vbAPVFrom.IncidencePoolingAndAggregation.PoolingName);
                                }
                                cbPoolingWindowIncidence.SelectedIndex = 0;


                                foreach (TreeNode trnd in currentNode.Nodes)
                                {
                                    changeNodeImage(trnd);
                                }
                                if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance == null)
                                {
                                    initNodeImage(currentNode.Nodes[0]);
                                }

                            }
                            else
                            {
                                foreach (TreeNode trnd in currentNode.Nodes)
                                {
                                    changeNodeImage(trnd);
                                }
                                if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance == null)
                                {
                                    initNodeImage(currentNode.Nodes[0]);
                                }
                                initNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
                                errorNodeImage(currentNode.Nodes[1]);
                                errorNodeImage(currentNode.Nodes[2]);
                            }





                            SetTabControl(tabCtlReport);
                            WaitClose();
                        }
                        this.trvSetting.ExpandAll();
                        break;
                    case "aggregation":
                        _currentNode = "aggregation";
                        frm = new APVX.Aggregation();
                        rtn = frm.ShowDialog();
                        if (rtn != DialogResult.OK) { return; }
                        DataRowView drv = null;
                        if (CommonClass.IncidencePoolingAndAggregationAdvance == null)
                            CommonClass.IncidencePoolingAndAggregationAdvance = new IncidencePoolingAndAggregationAdvance();
                        if ((frm as APVX.Aggregation).cboIncidenceAggregation.SelectedIndex != -1)
                        {
                            drv = (frm as APVX.Aggregation).cboIncidenceAggregation.SelectedItem as DataRowView;
                            CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drv["GridDefinitionID"]));
                        }
                        if ((frm as APVX.Aggregation).cboValuationAggregation.SelectedIndex != -1)
                        {
                            drv = (frm as APVX.Aggregation).cboValuationAggregation.SelectedItem as DataRowView;
                            if (CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != Convert.ToInt32(drv["GridDefinitionID"]))
                            {
                                CommonClass.lstCRResultAggregation = new List<CRSelectFunctionCalculateValue>();
                            }
                            CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drv["GridDefinitionID"]));
                        }
                        if ((frm as APVX.Aggregation).cboQALYAggregation.SelectedIndex != -1)
                        {
                            drv = (frm as APVX.Aggregation).cboQALYAggregation.SelectedItem as DataRowView;
                            if (CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation != null && CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID != Convert.ToInt32(drv["GridDefinitionID"]))
                            {
                                CommonClass.lstCRResultAggregationQALY = new List<CRSelectFunctionCalculateValue>();
                            }
                            CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drv["GridDefinitionID"]));
                        }
                        changeNodeImage(currentNode);
                        break;
                    case "poolingmethod":
                        _currentNode = "poolingmethod";
                        if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null)
                        {
                            frm = new IncidencePoolingandAggregation();
                            rtn = frm.ShowDialog();
                            if (rtn != DialogResult.OK) { return; }
                            cbPoolingWindowAPV.Items.Clear();
                            foreach (ValuationMethodPoolingAndAggregationBase vbAPVFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                            {
                                cbPoolingWindowAPV.Items.Add(vbAPVFrom.IncidencePoolingAndAggregation.PoolingName);
                            }
                            cbPoolingWindowAPV.SelectedIndex = 0;
                            cbPoolingWindowIncidence.Items.Clear();
                            foreach (ValuationMethodPoolingAndAggregationBase vbAPVFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                            {
                                this.cbPoolingWindowIncidence.Items.Add(vbAPVFrom.IncidencePoolingAndAggregation.PoolingName);
                            }
                            cbPoolingWindowIncidence.SelectedIndex = 0;






                            olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
                            changeNodeImage(currentNode.Parent.Nodes[currentNode.Parent.Nodes.Count - 2]);
                            changeNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
                            changeNodeImage(currentNode);
                            changeNodeImage(currentNode);
                            changeNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
                        }
                        SetTabControl(tabCtlReport);
                        break;
                    case "valuationmethod":
                        _currentNode = "valuationmethod";
                        if (CommonClass.ValuationMethodPoolingAndAggregation == null)
                            return;
                        if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.First().LstAllSelectValuationMethod == null)
                            return;
                        if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null)
                        {
                        }
                        else
                            return;
                        frm = new SelectValuationMethods();
                        rtn = frm.ShowDialog();
                        if (rtn != DialogResult.OK) { return; }

                        frmReport = new ConfigurationResultsReport();
                        frmReport.userAssignPercentile = false;
                        strHealthImpactPercentiles = null;
                        strPoolIncidencePercentiles = null;

                        cbPoolingWindowAPV.Items.Clear();
                        foreach (ValuationMethodPoolingAndAggregationBase vbAPVFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                        {
                            cbPoolingWindowAPV.Items.Add(vbAPVFrom.IncidencePoolingAndAggregation.PoolingName);
                        }
                        cbPoolingWindowAPV.SelectedIndex = 0;
                        cbPoolingWindowIncidence.Items.Clear();
                        foreach (ValuationMethodPoolingAndAggregationBase vbAPVFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                        {
                            this.cbPoolingWindowIncidence.Items.Add(vbAPVFrom.IncidencePoolingAndAggregation.PoolingName);
                        }
                        cbPoolingWindowIncidence.SelectedIndex = 0;


                        olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
                        changeNodeImage(currentNode.Parent.Nodes[currentNode.Parent.Nodes.Count - 2]);
                        changeNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
                        changeNodeImage(currentNode);
                        SetTabControl(tabCtlReport);
                        break;
                }
                if (iGridTypeOld != CommonClass.MainSetup.SetupID)
                {
                    ChangeAllAggregationCombox();
                }
                if (CommonClass.IncidencePoolingAndAggregationAdvance != null)
                {
                    changeNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
                    FireBirdHelperBase fb = new ESILFireBirdHelper();
                    string commandText = "";
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        private int EnforceLegendOrder() //string mapgroup, string newLayerPath)
        {  //Reorders the top level map groups, (Region Admin, Pollutants, Results) and the baseline, control, and delta layers within the Pollutant map groups

            MapGroup TopMG1 = new MapGroup(); //"Region Admin Layers"
            MapGroup TopMG2 = new MapGroup(); //"Pollutants"
            MapGroup TopMG3 = new MapGroup(); //"Results"
            MapGroup polMG = new MapGroup();  //A temp pollutant map group
            MapGroup statMG = new MapGroup(); // temp pollutant stat map group
            
            MapGroup HI_ResultMG = new MapGroup();  //Health Impacts
            MapGroup PI_ResultMG = new MapGroup();  //Pooled Incidence
            MapGroup PV_ResultMG = new MapGroup();  //Pooled Valuation

            IMapLayer baseIML = new MapPolygonLayer();
            IMapLayer controlIML = new MapPolygonLayer();
            IMapLayer deltaIML = new MapPolygonLayer();
            
            //Cycle through top level map groups first to get a pointer to each map group and remove the layer from the mainMapLayers list
            if (mainMap.Layers.Count > 1)
            {   
                mainMap.Layers.SuspendEvents();
                foreach (IMapLayer Toplayer in mainMap.Layers)
                {
                    if (Toplayer.LegendText == "Region Admin Layers") TopMG1 = (MapGroup)Toplayer;
                    if (Toplayer.LegendText == "Pollutants")
                    {
                        TopMG2 = (MapGroup)Toplayer;
                        if (TopMG2.Count > 0)
                        {
                            foreach (IMapLayer polLayer in TopMG2.GetLayers())
                            {
                                polMG = (MapGroup)polLayer;
                                foreach (IMapLayer statLayer in polMG.GetLayers())
                                {
                                    statMG = (MapGroup)statLayer;
                                    if (statMG.Count > 1) //
                                    {
                                        baseIML = null;
                                        controlIML = null;
                                        deltaIML = null;
                                        foreach (IMapLayer lowlayer in statMG)
                                        {
                                            if (lowlayer.LegendText == "Baseline") baseIML = lowlayer;
                                            if (lowlayer.LegendText == "Control") controlIML = lowlayer;
                                            if (lowlayer.LegendText == "Delta") deltaIML = lowlayer;
                                        }
                                        if (deltaIML != null)
                                        {
                                            deltaIML.LockDispose();
                                            statMG.Remove(deltaIML);
                                            statMG.Add(deltaIML);
                                            deltaIML.UnlockDispose();
                                        }
                                        if (controlIML != null)
                                        {
                                            controlIML.LockDispose();
                                            statMG.Remove(controlIML);
                                            statMG.Add(controlIML);
                                            controlIML.UnlockDispose();
                                        }
                                        if (baseIML != null)
                                        {
                                            baseIML.LockDispose();
                                            statMG.Remove(baseIML);
                                            statMG.Add(baseIML);
                                            baseIML.UnlockDispose();
                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (Toplayer.LegendText == "Results")
                    {
                        TopMG3 = (MapGroup)Toplayer;
                        if (TopMG3.Count > 0)
                        {
                            foreach (IMapLayer resultMGLayer in TopMG3.GetLayers())
                            {
                                if (resultMGLayer.LegendText == "Health Impacts") { HI_ResultMG = (MapGroup)resultMGLayer; }
                                if (resultMGLayer.LegendText == "Pooled Incidence") { PI_ResultMG = (MapGroup)resultMGLayer; }
                                if (resultMGLayer.LegendText == "Pooled Valuation") { PV_ResultMG = (MapGroup)resultMGLayer; }
                            }
                            if (!(PV_ResultMG.LegendText == null))
                            {
                                PV_ResultMG.LockDispose();
                                TopMG3.Remove(PV_ResultMG);
                                TopMG3.Add((IMapLayer)PV_ResultMG);
                                PV_ResultMG.UnlockDispose();
                            }
                            if (!(PI_ResultMG.LegendText == null))
                            {
                                PI_ResultMG.LockDispose();
                                TopMG3.Remove(PI_ResultMG);
                                TopMG3.Add((IMapLayer)PI_ResultMG);
                                PI_ResultMG.UnlockDispose();
                            }
                            if (!(HI_ResultMG.LegendText == null))
                            {
                                HI_ResultMG.LockDispose();
                                TopMG3.Remove(HI_ResultMG);
                                TopMG3.Add((IMapLayer)HI_ResultMG);
                                HI_ResultMG.UnlockDispose();
                            }
                        }
                    }
                }        
           
                //Add the layers in reverse desired display order
                if (!(TopMG3.LegendText == null))
                {
                    TopMG3.LockDispose();
                    mainMap.Layers.Remove(TopMG3);
                    mainMap.Layers.Add((IMapLayer)TopMG3);
                    TopMG3.UnlockDispose();
                }
                if (!(TopMG2.LegendText == null))
                {
                    TopMG2.LockDispose();
                    mainMap.Layers.Remove(TopMG2);
                    mainMap.Layers.Add((IMapLayer)TopMG2);
                    TopMG2.UnlockDispose();
                }
                if (!(TopMG1.LegendText == null))
                {
                    TopMG1.LockDispose();
                    mainMap.Layers.Remove(TopMG1);
                    mainMap.Layers.Add((IMapLayer)TopMG1);
                    TopMG1.UnlockDispose();
                }
                mainMap.Layers.ResumeEvents();
            }
            return 1; //if no result
        }
        private void DrawBaseline (TreeNode currentNode, string str)
        {   //MCB- draws base data on main map
            _currentNode = "basedata";
            str = string.Format("{0}baseline", (currentNode.Tag as BenMAPLine).Pollutant.PollutantName);
            string _PollutantName = (currentNode.Tag as BenMAPLine).Pollutant.PollutantName;
            //string _BenMapSetupName = (currentNode.Tag as BenMAPLine).GridType.SetupName;
            string _BenMapSetupName = CommonClass.MainSetup.SetupName;
            _CurrentMapTitle = _BenMapSetupName + " Setup: " + _PollutantName + ", Baseline";
            
            if (CommonClass.LstAsynchronizationStates != null &&
                CommonClass.LstAsynchronizationStates.Contains(str.ToLower()))
            {
                MessageBox.Show(string.Format("BenMAP is still creating the Baseline air quality surface map.", (currentNode.Tag as BenMAPLine).Pollutant.PollutantName), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            WaitShow("Drawing layer...");
            try
            {
                tabCtlMain.SelectedIndex = 0;
                //mainMap.Layers.Clear();

                //set change projection text
                string changeProjText = "change projection to setup projection";
                if (!String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
                {
                    changeProjText = changeProjText + " (" + CommonClass.MainSetup.SetupProjection + ")";
                }
                tsbChangeProjection.Text = changeProjText;

                BenMAPLine b = currentNode.Tag as BenMAPLine;
                foreach (BaseControlGroup bc in CommonClass.LstBaseControlGroup)
                {
                    if (bc.Pollutant.PollutantID == b.Pollutant.PollutantID)
                    { b = bc.Base; }
                }
                currentNode.Tag = b;
                addBenMAPLineToMainMap(b, "B");
                addRegionLayerGroupToMainMap();
                LayerObject = currentNode.Tag as BenMAPLine;
                InitTableResult(currentNode.Tag as BenMAPLine);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                Debug.WriteLine("DraawBaseline: " + ex.ToString());
            }
            WaitClose();
            int result = EnforceLegendOrder();
            return;
        }
        private void DrawControlData(TreeNode currentNode, string str)
        {
            _currentNode = "controldata";
  
            //Map Title
            string _PollutantName = (currentNode.Tag as BenMAPLine).Pollutant.PollutantName;
            string _BenMapSetupName = (currentNode.Tag as BenMAPLine).GridType.SetupName;
            _CurrentMapTitle = _BenMapSetupName + " Setup: " + _PollutantName + ", Control";
            
            str = string.Format("{0}control", (currentNode.Tag as BenMAPLine).Pollutant.PollutantName);
            if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Contains(str.ToLower()))
            {
                MessageBox.Show(string.Format("BenMAP is still creating the Control air quality surface map. ", (currentNode.Tag as BenMAPLine).Pollutant.PollutantName), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            WaitShow("Drawing layer...");
            try
            {
                tabCtlMain.SelectedIndex = 0;
                //mainMap.Layers.Clear();
                BenMAPLine cc = currentNode.Tag as BenMAPLine;
                foreach (BaseControlGroup bc in CommonClass.LstBaseControlGroup)
                {
                    if (bc.Pollutant.PollutantID == cc.Pollutant.PollutantID)
                    { cc = bc.Control; }
                }
                currentNode.Tag = cc;
                
                addBenMAPLineToMainMap(cc, "C");
                addRegionLayerGroupToMainMap();
                LayerObject = currentNode.Tag as BenMAPLine;
                InitTableResult(currentNode.Tag as BenMAPLine);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex); 
                Debug.WriteLine("DrawControlData: " + ex.ToString());
            }
            WaitClose();
            int result = EnforceLegendOrder();
            return;
        }
        private void DrawDelta(TreeNode currentNode, string str)
        {
            _currentNode = "delta";
            BaseControlGroup bcgDelta = currentNode.Tag as BaseControlGroup;
            if (bcgDelta == null)
            {
                MessageBox.Show("There is no result for delta.");
                return;
            }
            if (bcgDelta.Base == null || bcgDelta.Control == null)
            {
                MessageBox.Show("There is no result for delta.");
                return;
            }
            if (bcgDelta.Base.ModelResultAttributes == null || bcgDelta.Control.ModelResultAttributes == null)
            {
                MessageBox.Show("There is no result for delta.");
                return;
            }
            str = string.Format("{0}baseline", bcgDelta.Pollutant.PollutantName);
            if (CommonClass.LstAsynchronizationStates != null &&
                CommonClass.LstAsynchronizationStates.Contains(str.ToLower()))
            {
                MessageBox.Show(string.Format("BenMAP is still creating the Baseline air quality surface map. ", bcgDelta.Pollutant.PollutantName), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            str = string.Format("{0}control", bcgDelta.Pollutant.PollutantName);
            if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Contains(str.ToLower()))
            {
                MessageBox.Show(string.Format("BenMAP is still creating the Control air quality surface map. ", bcgDelta.Pollutant.PollutantName), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            WaitShow("Drawing layer...");

            //set change projection text
            string changeProjText = "change projection to setup projection";
            if (!String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
            {
                changeProjText = changeProjText + " (" + CommonClass.MainSetup.SetupProjection + ")";
            }
            tsbChangeProjection.Text = changeProjText;

            if (bcgDelta.DeltaQ == null)
            {
                bcgDelta.DeltaQ = new BenMAPLine();
                bcgDelta.DeltaQ.Pollutant = bcgDelta.Base.Pollutant;
                
                bcgDelta.DeltaQ.GridType = bcgDelta.Base.GridType;
                bcgDelta.DeltaQ.ModelResultAttributes = new List<ModelResultAttribute>();
                float deltaresult;
                Dictionary<string, Dictionary<string, float>> dicControl = new Dictionary<string, Dictionary<string, float>>();
                foreach (ModelResultAttribute mra in bcgDelta.Control.ModelResultAttributes)
                {
                    if (!dicControl.ContainsKey(mra.Col + "," + mra.Row))
                    {
                        dicControl.Add(mra.Col + "," + mra.Row, mra.Values);
                    }
                }
                foreach (ModelResultAttribute mra in bcgDelta.Base.ModelResultAttributes)
                {
                    try
                    {
                        if (dicControl.ContainsKey(mra.Col + "," + mra.Row))
                        {
                            bcgDelta.DeltaQ.ModelResultAttributes.Add(new ModelResultAttribute()
                            {
                                Col = mra.Col,
                                Row = mra.Row
                            });
                            bcgDelta.DeltaQ.ModelResultAttributes[bcgDelta.DeltaQ.ModelResultAttributes.Count - 1].Values = new Dictionary<string, float>();
                            foreach (KeyValuePair<string, float> k in mra.Values)        //Populates the Delta modelresultattributes by subtracting the control values from the base line values
                            {
                                if (dicControl[mra.Col + "," + mra.Row].ContainsKey(k.Key))
                                {
                                    deltaresult = k.Value - (dicControl[mra.Col + "," + mra.Row][k.Key]);
                                    if (deltaresult < 0) deltaresult = (float)0.0;
                                    bcgDelta.DeltaQ.ModelResultAttributes[bcgDelta.DeltaQ.ModelResultAttributes.Count - 1].Values.Add(k.Key, deltaresult);
                                }
                                else
                                    bcgDelta.DeltaQ.ModelResultAttributes[bcgDelta.DeltaQ.ModelResultAttributes.Count - 1].Values.Add(k.Key, Convert.ToSingle(0.0));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex); 
                        Debug.WriteLine("DrawDelta: " + ex.ToString());
                    }
                }
            }

            try
            {
                //Map Title
                string PollutantName = bcgDelta.DeltaQ.Pollutant.PollutantName;
                string BenMapSetupName = bcgDelta.Base.GridType.SetupName;
                _CurrentMapTitle = BenMapSetupName + " Setup: " + PollutantName + ", Delta";
            
                tabCtlMain.SelectedIndex = 0;
                //mainMap.Layers.Clear();
                addBenMAPLineToMainMap(bcgDelta.DeltaQ, "D");
                addRegionLayerGroupToMainMap();
                LayerObject = bcgDelta.DeltaQ;
                InitTableResult(bcgDelta.DeltaQ);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                Debug.WriteLine("DrawDelta (2): " + ex.ToString());
            }
            WaitClose();
            int result = EnforceLegendOrder();
            return;
        }
        private void CRResultChangeVPV()
        {
            try
            {
                if (CommonClass.lstIncidencePoolingAndAggregation == null) return;
                List<string> lstCRID = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Select(p => p.CRSelectFunction.CRID + "," + p.CRSelectFunction.BenMAPHealthImpactFunction.ID).ToList();
                foreach (IncidencePoolingAndAggregation ipa in CommonClass.lstIncidencePoolingAndAggregation)
                {
                    List<AllSelectCRFunction> lstRemove = new List<AllSelectCRFunction>();
                    foreach (AllSelectCRFunction asc in ipa.lstAllSelectCRFuntion)
                    {
                        if (asc.NodeType != 100) continue;
                        if (!lstCRID.Contains(asc.CRSelectFunctionCalculateValue.CRSelectFunction.CRID + "," + asc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.ID))
                        {
                            lstRemove.Add(asc);
                        }
                    }
                    foreach (AllSelectCRFunction ascremove in lstRemove)
                    {

                        ipa.lstAllSelectCRFuntion.Remove(ascremove);
                    }

                    List<AllSelectCRFunction> lstRemoveSec = new List<AllSelectCRFunction>();
                    foreach (AllSelectCRFunction allSelectCRFunction in ipa.lstAllSelectCRFuntion)
                    {
                        List<AllSelectCRFunction> lstTmp = new List<AllSelectCRFunction>();
                        APVX.APVCommonClass.getAllChildCR(allSelectCRFunction, ipa.lstAllSelectCRFuntion, ref lstTmp);
                        if (lstTmp.Where(p => p.NodeType == 100).Count() == 0)
                            lstRemoveSec.Add(allSelectCRFunction);
                    }
                    lstRemoveSec = lstRemoveSec.Where(p => p.NodeType != 100).ToList();
                    foreach (AllSelectCRFunction allSelectCRFunction in lstRemoveSec)
                    {
                        ipa.lstAllSelectCRFuntion.Remove(allSelectCRFunction);
                    }
                    foreach (AllSelectCRFunction allSelectCRFunction in ipa.lstAllSelectCRFuntion.Where(p => p.PoolingMethod == ""))
                    {
                        allSelectCRFunction.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(p => p.CRSelectFunction.CRID == allSelectCRFunction.CRID).First();
                    }
                }
                List<IncidencePoolingAndAggregation> lstIPRemove = new List<IncidencePoolingAndAggregation>();
                foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
                {
                    if (ip.lstAllSelectCRFuntion == null || ip.lstAllSelectCRFuntion.Count == 0)
                    {
                        lstIPRemove.Add(ip);
                    }
                }
                foreach (IncidencePoolingAndAggregation ip in lstIPRemove)
                {
                    CommonClass.lstIncidencePoolingAndAggregation.Remove(ip);
                }

                if (CommonClass.ValuationMethodPoolingAndAggregation == null) return;
                List<ValuationMethodPoolingAndAggregationBase> lstVBRemove = new List<ValuationMethodPoolingAndAggregationBase>();
                foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                {
                    if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Count == 0)
                    {
                        lstVBRemove.Add(vb);
                    }
                    else
                    {



                        List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
                        if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().PoolingMethod == "None")
                        {
                            APVX.APVCommonClass.getAllChildCRNotNone(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First(), vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);

                        }
                        lstCR.Insert(0, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First());
                        if (lstCR.Count == 1 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID < 9999 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID > 0) { }
                        else
                        {
                            APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(true, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.NodeType != 100).Max(p => p.NodeType), vb.IncidencePoolingAndAggregation.lstColumns);
                        }
                        List<AllSelectValuationMethod> lstASVMRemove = new List<AllSelectValuationMethod>();
                        List<int> lstAVM = new List<int>();
                        if (vb.LstAllSelectValuationMethod != null && vb.LstAllSelectValuationMethod.Count > 0)
                        {
                            lstAVM = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.CRSelectFunctionCalculateValue != null).Select(p => p.CRID).ToList();
                            List<AllSelectValuationMethod> lstVTemp = vb.LstAllSelectValuationMethod.Where(p => vb.LstAllSelectValuationMethod.Where(a => a.PID == p.ID).Where(c => c.NodeType != 2000).Count() == 0 && p.NodeType != 2000).ToList();
                            foreach (AllSelectValuationMethod asvm in lstVTemp)
                            {
                                if (!lstAVM.Contains(asvm.CRID))
                                {
                                    lstASVMRemove.Add(asvm);
                                }
                            }
                            foreach (AllSelectValuationMethod asvm in lstASVMRemove)
                            {
                                vb.LstAllSelectValuationMethod.Remove(asvm);
                            }
                            var query = vb.LstAllSelectValuationMethod.Where(p => p.NodeType == 2000);
                            lstASVMRemove = new List<AllSelectValuationMethod>();
                            foreach (AllSelectValuationMethod asvm in query)
                            {
                                if (vb.LstAllSelectValuationMethod.Where(p => p.ID == asvm.PID).Count() == 0)
                                    lstASVMRemove.Add(asvm);
                            }
                            foreach (AllSelectValuationMethod asvm in lstASVMRemove)
                            {
                                vb.LstAllSelectValuationMethod.Remove(asvm);
                            }
                        }
                        List<AllSelectQALYMethod> lstQALYRemove = new List<AllSelectQALYMethod>();
                        if (vb.lstAllSelectQALYMethod != null && vb.lstAllSelectQALYMethod.Count > 0)
                        {
                            lstAVM = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.CRSelectFunctionCalculateValue != null).Select(p => p.CRID).ToList();
                            List<AllSelectQALYMethod> lstQTemp = vb.lstAllSelectQALYMethod.Where(p => vb.lstAllSelectQALYMethod.Where(a => a.PID == p.ID).Where(c => c.NodeType != 3000).Count() == 0 && p.NodeType != 3000).ToList();
                            foreach (AllSelectQALYMethod QALY in lstQTemp)
                            {
                                if (!lstAVM.Contains(QALY.CRID))
                                {
                                    lstQALYRemove.Add(QALY);
                                }
                            }
                            foreach (AllSelectQALYMethod QALY in lstQALYRemove)
                            {
                                vb.lstAllSelectQALYMethod.Remove(QALY);
                            }
                            var queryQALY = vb.lstAllSelectQALYMethod.Where(p => p.NodeType == 3000);
                            lstQALYRemove = new List<AllSelectQALYMethod>();
                            foreach (AllSelectQALYMethod QALY in queryQALY)
                            {
                                if (vb.lstAllSelectQALYMethod.Where(p => p.ID == QALY.PID).Count() == 0)
                                    lstQALYRemove.Add(QALY);
                            }
                            foreach (AllSelectQALYMethod QALY in lstQALYRemove)
                            {
                                vb.lstAllSelectQALYMethod.Remove(QALY);
                            }
                        }
                    }
                }
                lstVBRemove = new List<ValuationMethodPoolingAndAggregationBase>();
                foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                {
                    if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion == null || vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Count == 0)
                    {
                        lstVBRemove.Add(vb);
                    }
                }
                foreach (ValuationMethodPoolingAndAggregationBase vbRemove in lstVBRemove)
                {
                    CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Remove(vbRemove);
                }

            }
            catch (Exception ex)
            {
            }
        }

        private void showExistBaseControlCRSelectFunction(BaseControlCRSelectFunction baseControlCRSelectFunction, TreeNode currentNode)
        {
            HealthImpactFunctions.MaxCRID = baseControlCRSelectFunction.lstCRSelectFunction.Max(p => p.CRID);
            CommonClass.LstPollutant = baseControlCRSelectFunction.BaseControlGroup.Select(pa => pa.Pollutant).ToList();
            CommonClass.LstBaseControlGroup = baseControlCRSelectFunction.BaseControlGroup;
            CommonClass.CRLatinHypercubePoints = baseControlCRSelectFunction.CRLatinHypercubePoints;
            CommonClass.CRRunInPointMode = baseControlCRSelectFunction.CRRunInPointMode;
            CommonClass.CRThreshold = baseControlCRSelectFunction.CRThreshold;
            CommonClass.CRSeeds = baseControlCRSelectFunction.CRSeeds;
            CommonClass.BenMAPPopulation = baseControlCRSelectFunction.BenMAPPopulation;
            CommonClass.GBenMAPGrid = baseControlCRSelectFunction.BaseControlGroup.First().GridType;
            if (baseControlCRSelectFunction.RBenMapGrid != null)
                CommonClass.RBenMAPGrid = baseControlCRSelectFunction.RBenMapGrid;
            CommonClass.BenMAPPopulation = baseControlCRSelectFunction.BenMAPPopulation;
            int nodesCount = 0;
            foreach (TreeNode trchild in trvSetting.Nodes)
            {
                if (trchild.Name == "airqualitygridgroup")
                {
                    nodesCount = trchild.Nodes.Count;


                    for (int i = nodesCount - 1; i > -1; i--)
                    {
                        TreeNode node = trchild.Nodes[i];
                        if (trchild.Nodes[i].Name == "datasource") { trchild.Nodes.RemoveAt(i); }
                    }
                    for (int i = CommonClass.LstBaseControlGroup.Count - 1; i > -1; i--)
                    {
                        AddDataSourceNode(CommonClass.LstBaseControlGroup[i], trchild);
                    }
                    trchild.ExpandAll();

                    foreach (TreeNode trair in trchild.Nodes)
                    {
                        changeNodeImage(trair);
                        TreeNode tr = trair;
                        if (trair.Name == "gridtype")
                        {
                            AddChildNodes(ref tr, "", "", new BenMAPLine());
                            trair.ExpandAll();
                        }
                    }
                }
                if (trchild.Name == "configuration")
                {
                    foreach (TreeNode tr in trchild.Nodes)
                    {
                        changeNodeImage(tr);
                    }
                    if (CommonClass.BaseControlCRSelectFunctionCalculateValue == null || CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue == null)
                        errorNodeImage(trchild.Nodes[trchild.Nodes.Count - 1]);
                    trchild.ExpandAll();
                }
            }
        }

        private List<AllSelectQALYMethod> getChildFromAllSelectQALYMethod(AllSelectQALYMethod allSelectQALYMethod, ValuationMethodPoolingAndAggregationBase vb)
        {
            List<AllSelectQALYMethod> lstAll = new List<AllSelectQALYMethod>();
            var query = from a in vb.lstAllSelectQALYMethod where a.PID == allSelectQALYMethod.ID select a;
            lstAll = query.ToList();
            return lstAll;
        }

        private List<AllSelectValuationMethod> getChildFromAllSelectValuationMethod(AllSelectValuationMethod allSelectValuationMethod, ValuationMethodPoolingAndAggregationBase vb)
        {
            List<AllSelectValuationMethod> lstAll = new List<AllSelectValuationMethod>();
            if (vb == null)
            {
                lstAll = lstAPVPoolingAndAggregationAll.Where(p => p.PID == allSelectValuationMethod.ID).ToList();
            }
            else
            {
                var query = from a in vb.LstAllSelectValuationMethod where a.PID == allSelectValuationMethod.ID select a;
                lstAll = query.ToList();
            }
            return lstAll;
        }
        private void getChildFromAllSelectCRFunctionUnPooled(AllSelectCRFunction allSelectValuationMethod, ValuationMethodPoolingAndAggregationBase vb, ref List<AllSelectCRFunction> lstAll)
        {
            if (allSelectValuationMethod.PoolingMethod != null && allSelectValuationMethod.PoolingMethod == "None")
            {
                var query = from a in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion where a.PID == allSelectValuationMethod.ID select a;
                lstAll.AddRange(query.ToList());
                foreach (AllSelectCRFunction acr in query)
                {
                    getChildFromAllSelectCRFunctionUnPooled(acr, vb, ref lstAll);
                }
            }

        }
        private void getChildFromAllSelectValuationMethodUnPooled(AllSelectValuationMethod allSelectValuationMethod, ValuationMethodPoolingAndAggregationBase vb, ref List<AllSelectValuationMethod> lstAll)
        {
            if (allSelectValuationMethod.PoolingMethod != null && allSelectValuationMethod.PoolingMethod == "None")
            {
                var query = from a in vb.LstAllSelectValuationMethod where a.PID == allSelectValuationMethod.ID select a;
                lstAll.AddRange(query.ToList());
                foreach (AllSelectValuationMethod acr in query)
                {
                    getChildFromAllSelectValuationMethodUnPooled(acr, vb, ref lstAll);
                }
            }

        }
        private void getChildFromAllSelectQALYMethodUnPooled(AllSelectQALYMethod allSelectValuationMethod, ValuationMethodPoolingAndAggregationBase vb, ref List<AllSelectQALYMethod> lstAll)
        {
            if (allSelectValuationMethod.PoolingMethod != null && allSelectValuationMethod.PoolingMethod == "None")
            {
                var query = from a in vb.lstAllSelectQALYMethod where a.PID == allSelectValuationMethod.ID select a;
                lstAll.AddRange(query.ToList());
                foreach (AllSelectQALYMethod acr in query)
                {
                    getChildFromAllSelectQALYMethodUnPooled(acr, vb, ref lstAll);
                }
            }

        }
        private void addBenMAPLineToMainMap(BenMAPLine benMAPLine, string isBase)
        {
            mainMap.ProjectionModeReproject = ActionMode.Never;
            mainMap.ProjectionModeDefine = ActionMode.Never;
           
            //string s = isBase;
            string IsBaseLongText;
            switch (isBase)
            {
                case "B":
                    IsBaseLongText = "Baseline";
                    break;
                case "C":
                    IsBaseLongText = "Control";
                    break;
                case "D":
                    IsBaseLongText = "Delta";
                    break;
                default:
                    return; 
            }
            
            MapGroup bcgMapGroup = new MapGroup();
            MapGroup TopPollutantMapGroup = new MapGroup();
            MapGroup polMapGroup = new MapGroup();

            //MapPolygonLayer polLayer = new MapPolygonLayer();
            string pollutantMGText;
            string bcgMGText;
            string LayerNameText;
            string LayerLegendText;

            //Add Pollutants Mapgroup if it doesn't exist already -MCB
            TopPollutantMapGroup = AddMapGroup("Pollutants", "Map Layers", false, false);

            //Get Metrics fields for this pollutant.  If no metrics then return with warning/error
            List<string> lstAddField = new List<string>();
            if (benMAPLine.Pollutant.Metrics != null)
            {
                foreach (Metric metric in benMAPLine.Pollutant.Metrics)
                {
                    lstAddField.Add(metric.MetricName);
                }
            }
            if (benMAPLine.Pollutant.SesonalMetrics != null)
            {
                foreach (SeasonalMetric sesonalMetric in benMAPLine.Pollutant.SesonalMetrics)
                {
                    lstAddField.Add(sesonalMetric.SeasonalMetricName);
                }
            }
            //Add a layer for each metric for this pollutant
            for (int iAddField = 2; iAddField < 2 + lstAddField.Count; iAddField++)
            {   
                MapPolygonLayer polLayer = new MapPolygonLayer();


                pollutantMGText = benMAPLine.Pollutant.PollutantName.ToString();
                _columnName = lstAddField[iAddField - 2];
                bcgMGText = _columnName.ToString();
                LayerLegendText = IsBaseLongText;
                LayerNameText = pollutantMGText + "_" + "_" + bcgMGText + "_" + LayerLegendText;

                try
                {
                    polMapGroup = AddMapGroup(pollutantMGText, "Pollutants", false, false);
                    bcgMapGroup = AddMapGroup(bcgMGText, pollutantMGText, false, false);
                    //Remove the old version of the layer if exists already
                    RemoveOldPolygonLayer(LayerNameText, bcgMapGroup.Layers, false);  //!!!!!!!!!!!!Need to trap for problems removing the old layer if it exists?

                    // Add a new layer baseline, control or delta layer to the Pollutants group
                    if (File.Exists(benMAPLine.ShapeFile))
                    {
                        try
                        {
                            // mainMap.Layers.Add(benMAPLine.ShapeFile);
                            polLayer = (MapPolygonLayer)bcgMapGroup.Layers.Add(benMAPLine.ShapeFile);                           
                        }
                        catch (Exception ex)
                        {
                            DataSourceCommonClass.SaveBenMAPLineShapeFile(CommonClass.GBenMAPGrid, benMAPLine.Pollutant, benMAPLine, benMAPLine.ShapeFile);
                            polLayer = (MapPolygonLayer)bcgMapGroup.Layers.Add(benMAPLine.ShapeFile);   //-MCB use when mapgroup layers is working correctly
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(benMAPLine.ShapeFile))
                        {
                            //benMAPLine.ShapeFile = benMAPLine.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "B" + isBase + ".shp";
                            benMAPLine.ShapeFile = benMAPLine.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "B" + IsBaseLongText + ".shp";
                            benMAPLine.ShapeFile = string.Format("{0}\\Tmp\\{1}", CommonClass.DataFilePath, benMAPLine.ShapeFile);
                        }
                        if (benMAPLine.ModelResultAttributes != null)  //MCB added this until we can figure out why the result attributes are not being populated 
                        {
                            DataSourceCommonClass.SaveBenMAPLineShapeFile(CommonClass.GBenMAPGrid, benMAPLine.Pollutant, benMAPLine, benMAPLine.ShapeFile);    ///MCB- Commemented out to resolve issues with not drawing non-saved data (e.g., Monitor data).  This may just be a twmp fix and May cause problems elsewhere
                        }
                        polLayer = (MapPolygonLayer)bcgMapGroup.Layers.Add(benMAPLine.ShapeFile);  
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    Debug.WriteLine("addBenMAPLineToMainMap: Error adding new layer " + LayerNameText + " :" + ex.ToString());
                }

                //define the symbology, legend text and identifying name for the layer
                polLayer.DataSet.DataTable.Columns[iAddField].ColumnName = _columnName; // lstAddField[iAddField - 2];
                polLayer.LegendText = LayerLegendText;
                polLayer.Name = LayerNameText;

                //MapPolygonLayer polLayer = bcgMapGroup.Layers[mainMap.Layers.Count-1] as MapPolygonLayer; -MCB use when mapgroup layers is working correctly
                //MapPolygonLayer polLayer = mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer;
                ////Get Metrics fields.  If no metrics then return with warning/error
                //List<string> lstAddField = new List<string>();
                //if (benMAPLine.Pollutant.Metrics != null)
                //{
                //    foreach (Metric metric in benMAPLine.Pollutant.Metrics)
                //    {
                //        lstAddField.Add(metric.MetricName);
                //    }
                //}
                //if (benMAPLine.Pollutant.SesonalMetrics != null)
                //{
                //    foreach (SeasonalMetric sesonalMetric in benMAPLine.Pollutant.SesonalMetrics)
                //    {
                //        lstAddField.Add(sesonalMetric.SeasonalMetricName);
                //    }
                //}
                //-------------------------------------------------------------------------------------------
                ////Add a layer for each metric for this pollutant
                //for (int iAddField = 2; iAddField < 2 + lstAddField.Count; iAddField++)
                //{
                //    polLayer.DataSet.DataTable.Columns[iAddField].ColumnName = lstAddField[iAddField - 2];
                //}

                //if (isBase == "B") polLayer.LegendText = "Baseline";
                //if (isBase == "D") polLayer.LegendText = "Delta";
                //if (isBase == "C") polLayer.LegendText = "Control";

                //polLayer.LegendText = benMAPLine.Pollutant.PollutantName + "_" + IsBaseLongText;
                //polLayer.Name = polLayer.LegendText + "_" + benMAPLine.Pollutant.Metrics[0].MetricName;  //-MCB using name as a layer handle to grab it elsewhere

                //string strValueField = polLayer.DataSet.DataTable.Columns[2].ColumnName;
                //_columnName = strValueField;

                try
                {
                    PolygonScheme myNewScheme = CreateBCGPolyScheme(ref polLayer, 6, isBase);
                    polLayer.Symbology = myNewScheme;
                    polLayer.ApplyScheme(myNewScheme);

                    //set the layer to have its symbology expanded by default //-MCB
                    polLayer.Symbology.IsExpanded = true;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    Debug.WriteLine("Error applying symbology for " + LayerNameText + " :" + ex.ToString());
                }

                //double dMinValue = 0.0;
                //double dMaxValue = 0.0;
                //dMinValue = benMAPLine.ModelResultAttributes.Min(a => a.Values[strValueField]);
                //dMaxValue = benMAPLine.ModelResultAttributes.Max(a => a.Values[strValueField]);

                //if (double.IsNaN(dMinValue)) dMinValue = 0;
                //if (double.IsNaN(dMaxValue)) dMaxValue = 0;
                //if (isBase == "C")
                //{
                //    try
                //    {
                //        foreach (BaseControlGroup baseControlGroup in CommonClass.LstBaseControlGroup)
                //        {

                //            if (baseControlGroup.GridType.GridDefinitionID == benMAPLine.GridType.GridDefinitionID && baseControlGroup.Pollutant.PollutantID == benMAPLine.Pollutant.PollutantID)
                //            {
                //                if (baseControlGroup.Base != null && baseControlGroup.Base.ModelResultAttributes != null && baseControlGroup.Base.ModelResultAttributes.Count > 0)
                //                {
                //                    dMinValue = baseControlGroup.Base.ModelResultAttributes.Min(a => a.Values.ToArray()[0].Value);
                //                    dMaxValue = baseControlGroup.Base.ModelResultAttributes.Max(a => a.Values.ToArray()[0].Value);
                //                }
                //            }
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //    }

                //}

                ////_currentLayerIndex = mainMap.Layers.Count - 1;

                //_dMinValue = dMinValue;
                //_dMaxValue = dMaxValue;
                //_columnName = strValueField;
            }
                

            RenderMainMap(true,isBase); 
            //}
            return;
            
        }

        private string _drawStatus = string.Empty; 
        private double _dMinValue = 0.0;
        private double _dMaxValue = 0.0;
        private IMapLayer _CurrentIMapLayer = null;
        //private int _currentLayerIndex = 1; //MCB- used in old way of accessing layers
        private string _columnName = string.Empty;
        private string regionGroupLegendText = "Region Admin Layers";
        private string _bcgGroupLegendText = "Pollutants";
        private bool _HealthResultsDragged = false;
        private bool _IncidenceDragged = false;
        private bool _APVdragged = false;
        
        private Color[] _blendColors;
        public Color[] BlendColors
        {
            get { return _blendColors; }
            set { _blendColors = value; }
        }
        
        private PolygonScheme CreateBCGPolyScheme(ref MapPolygonLayer polLayer, int CategoryNumber = 6, string isBase = "B")
        {
            if (isBase == "D") //use the delta color ramp
            {   
                colorBlend.ColorArray = GetColorRamp("oranges", CategoryNumber);
            }
            else //use the default color ramp
            {   
                colorBlend.ColorArray = GetColorRamp("pale_yellow_blue", CategoryNumber); //pale_yellow_blue
            }
            PolygonScheme myScheme1 = new PolygonScheme();
            myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
            myScheme1.EditorSettings.IntervalMethod = IntervalMethod.NaturalBreaks;
            myScheme1.EditorSettings.IntervalSnapMethod = IntervalSnapMethod.SignificantFigures;
            myScheme1.EditorSettings.IntervalRoundingDigits = 3; //number of significant figures (or decimal places if using rounding)
            myScheme1.EditorSettings.NumBreaks = CategoryNumber;
            myScheme1.EditorSettings.FieldName = _columnName;
            myScheme1.EditorSettings.UseGradient = false;
            myScheme1.ClearCategories();
            myScheme1.CreateCategories(polLayer.DataSet.DataTable);    ///MCB- Note: This method can't deal with negative numbers correctly
            
            // Set the category colors equal to the selected color ramp
           
            for (int catNum = 0; catNum < CategoryNumber; catNum++)
            { 
                //Create the simple pattern with opacity
                SimplePattern sp = new SimplePattern(colorBlend.ColorArray[catNum]);
                //SimplePattern sp = new SimplePattern(Color.Purple);
                sp.Opacity = 0.8F;  //80% opaque = 20% transparent
                PolygonSymbolizer poly = new PolygonSymbolizer(colorBlend.ColorArray[catNum], Color.Transparent, 0);
                //PolygonSymbolizer poly = new PolygonSymbolizer(Color.Red, Color.Transparent, 0);
                poly.Patterns.Clear();
                poly.Patterns.Add(sp);

                myScheme1.Categories[catNum].Symbolizer = poly;
                //myScheme1.Categories[catNum].SetColor(colorBlend.ColorArray[catNum]);

                //make a copy of the category and add it to the color ramp:  -MCB - needed to get the property editor to work correctly
                PolygonCategory tempCat = new PolygonCategory();
                tempCat = (PolygonCategory)myScheme1.Categories[catNum].Clone();
                myScheme1.AddCategory(tempCat);

                //alternate method ignoring transparency of inside color  
                //myScheme1.Categories[catNum].Symbolizer.SetOutline(Color.Transparent, 0); //make the outlines invisble
                //myScheme1.Categories[catNum].SetColor(colorBlend.ColorArray[catNum]);
            }

            for (int catNum = 0; catNum < (CategoryNumber); catNum++)
            {
                myScheme1.RemoveCategory(myScheme1.Categories[0]);
            }

            myScheme1.AppearsInLegend = false; //if true then legend text displayed
            myScheme1.IsExpanded = true;
            myScheme1.LegendText = _columnName;
            
            return myScheme1;
        }
        private PolygonScheme CreateResultPolyScheme(ref MapPolygonLayer polLayer, int CategoryNumber = 6, string isBase = "R")
        {
            switch (isBase)
            {
                case "D":  //use the delta color ramp
                    //colorBlend.ColorArray = GetColorRamp("blue_red", CategoryNumber);
                    colorBlend.ColorArray = GetColorRamp("oranges", CategoryNumber);
                    break;
                case "R": //Configuration Results -MCB choose another color ramp???
                    colorBlend.ColorArray = GetColorRamp("brown_green", CategoryNumber);
                    break;
                case "I": //Pooled Incidence Results??? -MCB choose another color ramp???
                    colorBlend.ColorArray = GetColorRamp("yellow_red", CategoryNumber);
                    break;
                case "H": //Health Impact Function -MCB choose another color ramp???
                    colorBlend.ColorArray = GetColorRamp("blues", CategoryNumber);  //"pale_blue_green"
                    break;
                case "A": //Pooled Valuation Results -MCB choose another color ramp???
                    colorBlend.ColorArray = GetColorRamp("purples", CategoryNumber);
                    break;
                case "IP": //Pooled Incidence Results -MCB choose another color ramp???
                    colorBlend.ColorArray = GetColorRamp("oranges", CategoryNumber);
                    break;
                default: //use the default color ramp
                     colorBlend.ColorArray = GetColorRamp("pale_yellow_blue", CategoryNumber); //pale_yellow_blue
                     break;
            }
            
            PolygonScheme myScheme1 = new PolygonScheme();
            myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
            myScheme1.EditorSettings.IntervalMethod = IntervalMethod.Quantile; //IntervalMethod.NaturalBreaks;
            myScheme1.EditorSettings.IntervalSnapMethod = IntervalSnapMethod.Rounding; //IntervalSnapMethod.SignificantFigures;
            myScheme1.EditorSettings.IntervalRoundingDigits = 5; // (was 3) number of significant figures (or decimal places if using rounding)
            myScheme1.EditorSettings.NumBreaks = CategoryNumber;
           // myScheme1.EditorSettings.MaxSampleCount = polLayer.DataSet.DataTable.Rows.Count; //Temporary addition to ensure good breaks
            myScheme1.EditorSettings.FieldName = _columnName;
            myScheme1.EditorSettings.UseGradient = false;
            try
            {
                myScheme1.CreateCategories(polLayer.DataSet.DataTable);
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            // Set the category colors equal to the selected color ramp
            for (int catNum = 0; catNum < myScheme1.Categories.Count; catNum++)
            {
                myScheme1.Categories[catNum].Symbolizer.SetOutline(Color.Transparent, 0); //make the outlines invisble
                myScheme1.Categories[catNum].SetColor(colorBlend.ColorArray[catNum].ToTransparent((float)0.9));
            }
            myScheme1.AppearsInLegend = true; //if true then legend text displayed
            myScheme1.IsExpanded = true;
            myScheme1.LegendText = _columnName;

            return myScheme1;
        }
        private void ResetGisMap(object sender, EventArgs e, string isBase)
        {
            try
            {   
                //Number of categories
            //    int _CategoryNumber = 6;

            //    //Replace the color ramp
            //    if (isDelta)
            //    {   //use the delta color ramp
            //        colorBlend.ColorArray = GetColorRamp("red_blue", 6);
            //    }
            //    else
            //    {   //use the default color ramp
            //        colorBlend.ColorArray = GetColorRamp("pale_yellow_blue", 6); //pale_yellow_blue
            //    }

                //_blendColors = colorBlend.ColorArray;
                //_dMaxValue = colorBlend.MaxValue;
                //_dMinValue = colorBlend.MinValue;
                //colorBlend.SetValueRange(_dMinValue, _dMaxValue, false);
                //Color[] colors = new Color[_blendColors.Length];
                //_blendColors.CopyTo(colors, 0);
                //PolygonCategoryCollection pcc = new PolygonCategoryCollection();
                //int iColor = 0;
                //string ColumnName = _columnName;
                
                //PolygonScheme myScheme1 = new PolygonScheme();
                
                //float fl = (float)0.3;
                //float fColor = (float)0.2;
                //Color ctemp = new Color();

                //iColor = 0;
                //for (int iBlend = 0; iBlend < 6; iBlend++)
                //{
                //    PolygonCategory pcin = new PolygonCategory();
                //    double dnow = 0; double dnowUp = 0; dnow = colorBlend.ValueArray[iBlend];
                //    if (iBlend < 5)
                //        dnowUp = colorBlend.ValueArray[iBlend + 1];
                //    pcin.FilterExpression = string.Format("[{0}]>=" + dnow + " and [{0}] <" + dnowUp, ColumnName);
                //    pcin.LegendText = ">=" + dnow.ToString() + " and <" + dnowUp.ToString(); if (iBlend == 0)
                //    {
                //        pcin.FilterExpression = string.Format(" [{0}] <" + dnowUp, ColumnName);
                //        pcin.LegendText = "<" + dnowUp.ToString();
                //    }
                //    if (iBlend == 5)
                //    {
                //        pcin.FilterExpression = string.Format(" [{0}] >=" + dnow, ColumnName);
                //        pcin.LegendText = ">=" + dnow.ToString();
                //    }


                //    pcin.Symbolizer.SetOutline(Color.Transparent, 0);
                //    ctemp = pcin.Symbolizer.GetFillColor();
                //    pcin.Symbolizer.SetFillColor(ctemp.ToTransparent(fColor));
                //    ctemp.ToTransparent(fColor);
                //    pcin.Symbolizer.SetFillColor(colors[iColor]);
                //    pcc.Add(pcin);
                //    iColor++;
                //}
                //myScheme1.ClearCategories();
                
                //-MCB-----------------------------Replaces custom categories above with natural breaks
                //IFeatureLayer _MyLayer = (mainMap.Layers[_currentLayerIndex] as IFeatureLayer);

          //      MapPolygonLayer polLayer = mainMap.Layers[_currentLayerIndex] as MapPolygonLayer;
                //Replace the color ramp
                //if (isDelta)
                //{//use the delta color ramp
                //    myScheme1.EditorSettings.StartColor = Color.FromArgb(215, 48, 39); // red
                //    myScheme1.EditorSettings.StartColor.ToTransparent(fl);
                //    myScheme1.EditorSettings.EndColor = Color.FromArgb(0, 0, 255); //blue
                //    myScheme1.EditorSettings.EndColor.ToTransparent(fl);
                //}
                //else
                //{//use the default color ramp

                //    myScheme1.EditorSettings.StartColor = Color.FromArgb(255, 255, 153); // pale yellow
                //    myScheme1.EditorSettings.StartColor.ToTransparent(fl);
                //    myScheme1.EditorSettings.EndColor = Color.FromArgb(8, 104, 172); //blue
                //    myScheme1.EditorSettings.EndColor.ToTransparent(fl);
                //}
               
                //myScheme1.EditorSettings.UseColorRange = true;
                //myScheme1.EditorSettings.RampColors = true;
                 
                //myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
                
                //myScheme1.EditorSettings.IntervalMethod = IntervalMethod.Geometrical;
                //myScheme1.EditorSettings.IntervalMethod = IntervalMethod.StandardDeviation;
                //myScheme1.EditorSettings.IntervalMethod = IntervalMethod.NaturalBreaks;

                ////myScheme1.EditorSettings.IntervalMethod = IntervalMethod.EqualInterval;
                ////myScheme1.EditorSettings.IntervalSnapMethod = IntervalSnapMethod.None;
                ////myScheme1.EditorSettings.IntervalSnapMethod = IntervalSnapMethod.Rounding;
                //myScheme1.EditorSettings.IntervalSnapMethod = IntervalSnapMethod.SignificantFigures;
                //myScheme1.EditorSettings.IntervalRoundingDigits = 3; //number of significant figures (or decimal places if using rounding)
                //myScheme1.EditorSettings.NumBreaks = _CategoryNumber;
                //myScheme1.EditorSettings.FieldName = _columnName;
                //myScheme1.EditorSettings.UseGradient = false;  

                //myScheme1.CreateCategories(polLayer.DataSet.DataTable);
                
                //// Set the category colors equal to the selected color ramp
                //for (int catNum = 0; catNum < _CategoryNumber; catNum++)
                //{
                //    myScheme1.Categories[catNum].Symbolizer.SetOutline(Color.Transparent, 0); //make the outlines invisble
                //    myScheme1.Categories[catNum].SetColor(colorBlend.ColorArray[catNum]);
                //}
                //myScheme1.AppearsInLegend = true; //if true then legend text displayed
                //polLayer.Symbology
                //polLayer.Symbolizer.SetOutline(Color.Transparent,0);

                // -MCB---------------------------------


                //foreach (PolygonCategory pct in pcc)
                //{
                //    myScheme1.Categories.Add(pct);
                //}
                //myScheme1.AppearsInLegend = true;//-MCB changed from true

                //myScheme1.IsExpanded = true;
                //myScheme1.LegendText = _columnName;
                
                // myScheme1.EditorSettings.ClassificationType = ClassificationType.Custom;

                //(mainMap.Layers[_currentLayerIndex] as IFeatureLayer).Symbology = myScheme1;
                // (mainMap.Layers[_currentLayerIndex] as IFeatureLayer).Symbology = CreateBCGPolyScheme(ref polLayer, 6, isBase);

                _SavedExtent = mainMap.GetAllLayers()[0].Extent;
                mainMap.ViewExtents = _SavedExtent;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        private void RenderMainMap(bool isCone, string isBase)
        {
            //double min = _dMinValue;
            //double max = _dMaxValue;
            //colorBlend.SetValueRange(min, max, true);
            //colorBlend._minPlotValue = _dMinValue;
            //colorBlend._maxPlotValue = _dMaxValue;
            tbMapTitle.Text = _CurrentMapTitle;
            
            ResetGisMap(null, null, isBase);
            _MapAlreadyDisplayed = true;   //-MCB lets other parts of the program know that the map is present.
            return;
            ////Color[] colors = new Color[] { Color.Blue, Color.FromArgb(0, 255, 255), Color.FromArgb(0, 255, 0), Color.Yellow, Color.Red, Color.FromArgb(255, 0, 255) };
           
            ////Replace the color ramp -MCB
            //Color[] colors = GetColorRamp("pale_yellow_blue", 6);
            
            //colorBlend.SetValueRange(min, max, true);
            //_blendColors = colorBlend.ColorArray;
            //_dMinValue = colorBlend.MinValue;
            //_dMaxValue = colorBlend.MaxValue;

            //PolygonCategoryCollection pcc = new PolygonCategoryCollection();
            //int iColor = 0;
            //PolygonScheme myScheme1 = new PolygonScheme();
            //float fl = (float)0.1;
            ////Replaced originial rainbow schme with current scheme
            //// myScheme1.EditorSettings.StartColor = Color.Blue;
            //// myScheme1.EditorSettings.EndColor = Color.FromArgb(255, 0, 255);
            //myScheme1.EditorSettings.StartColor = colors[0];
            //myScheme1.EditorSettings.EndColor = colors[5];
 
            //float fColor = (float)0.2;
            //Color ctemp = new Color();
            //if (isCone)
            //{
            //    myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
            //    myScheme1.EditorSettings.IntervalMethod = IntervalMethod.NaturalBreaks;
            //    myScheme1.EditorSettings.IntervalSnapMethod = IntervalSnapMethod.Rounding;
            //    myScheme1.EditorSettings.IntervalRoundingDigits = 1;
            //    myScheme1.EditorSettings.NumBreaks = 6;
            //    myScheme1.EditorSettings.FieldName = _columnName; myScheme1.EditorSettings.UseGradient = false;
            //    myScheme1.CreateCategories((_CurrentIMapLayer as IFeatureLayer).DataSet.DataTable);
            //    if (myScheme1.Categories.Count == 1)
            //    {

            //        PolygonSymbolizer ps = new PolygonSymbolizer();
            //        ps.SetFillColor(colors[iColor]);
            //        ps.SetOutline(Color.Transparent, 0);

            //        (_CurrentIMapLayer as IFeatureLayer).Symbolizer = ps;
            //        return;

            //    }
            //    else
            //    {
            //        foreach (PolygonCategory pc in myScheme1.Categories)
            //        {
            //            PolygonCategory pcin = pc;
            //            double dnow = Math.Round(_dMinValue + ((_dMaxValue - _dMinValue) / 6.0000) * Convert.ToDouble(iColor), 3);
            //            double dnowUp = Math.Round(_dMinValue + ((_dMaxValue - _dMinValue) / 6.0000) * Convert.ToDouble(iColor + 1), 3);

            //            pcin.FilterExpression = string.Format("[{0}]>=" + dnow + " and [{0}] <" + dnowUp, _columnName);
            //            pcin.LegendText = string.Format(">=" + dnow.ToString("E2") + " and  <" + dnowUp.ToString("E2"), _columnName);
            //            if (iColor == 0)
            //            {
            //                pcin.FilterExpression = string.Format(" [{0}] <" + dnowUp, _columnName);
            //                pcin.LegendText = string.Format("<" + dnowUp.ToString("E2"), _columnName);
            //            }
            //            if (iColor == myScheme1.Categories.Count - 1)
            //            {
            //                pcin.FilterExpression = string.Format(" [{0}] >=" + dnow, _columnName);
            //                pcin.LegendText = string.Format("<" + dnowUp.ToString("E2"), _columnName);

            //            }


            //            pcin.Symbolizer.SetOutline(Color.Transparent, 0);
            //            ctemp = pcin.Symbolizer.GetFillColor();
            //            pcin.Symbolizer.SetFillColor(ctemp.ToTransparent(fColor));
            //            ctemp.ToTransparent(fColor);
            //            pcin.Symbolizer.SetFillColor(colors[iColor]);
            //            pcc.Add(pcin);
            //            iColor++;
            //        }
            //    }
            //    myScheme1.ClearCategories();  //-MCB
            //    foreach (PolygonCategory pct in pcc)
            //    {
            //        myScheme1.Categories.Add(pct);
            //    }
            //    (_CurrentIMapLayer as IFeatureLayer).Symbology = myScheme1;
            //}
            //else  //Results?
            //{
            //    pcc = new PolygonCategoryCollection();
            //    myScheme1.EditorSettings.ClassificationType = ClassificationType.UniqueValues;
            //    myScheme1.EditorSettings.FieldName = _columnName; myScheme1.EditorSettings.UseGradient = false;

            //    myScheme1.CreateCategories((_CurrentIMapLayer as IFeatureLayer).DataSet.DataTable);
            //    foreach (PolygonCategory pc in myScheme1.Categories)
            //    {
            //        PolygonCategory pcin = pc;
            //        pcin.Symbolizer.SetOutline(Color.Transparent, 0);
            //        pcc.Add(pcin);
            //    }
            //    myScheme1.ClearCategories();
            //    foreach (PolygonCategory pct in pcc)
            //    {
            //        myScheme1.Categories.Add(pct);
            //    }
            //    if (myScheme1.LegendText == "Pooled Inci") myScheme1.LegendText = "Pooled Incidence";
            //    if (myScheme1.LegendText == "Pooled Valu") myScheme1.LegendText = "Pooled Valuation";
            //    (_CurrentIMapLayer as IFeatureLayer).Symbology = myScheme1;
            //}

        }
        private void addRegionLayerToMainMap()
        {
            try
            {
                if (CommonClass.RBenMAPGrid == null)
                {
                    try
                    {
                        DataRowView drGrid = cboRegion.SelectedItem as DataRowView;
                        CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));
                    }
                    catch
                    {
                    }
                }
                bool isWGS84 = true;
                if (tsbChangeProjection.Text == "change projection to WGS1984")
                {
                    tsbChangeProjection_Click(null, null);
                    isWGS84 = false;
                }
                if (CommonClass.RBenMAPGrid == null)
                {
                    cboRegion.SelectedIndex = 0;
                };
                mainMap.ProjectionModeReproject = ActionMode.Never;
                mainMap.ProjectionModeDefine = ActionMode.Never;
                //-MCB  Added Reference group to the legend
               MapGroup RegionMapGroup = new MapGroup(mainMap, "Region Reference"); //-MCB use when mapgroup layers is working correctly

                if (CommonClass.RBenMAPGrid is ShapefileGrid)
                {
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                    {
                       RegionMapGroup.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"); //-MCB use when mapgroup layers is working correctly

                        //mainMap.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as ShapefileGrid).ShapefileName + ".shp");
                    }
                }
                else if (CommonClass.RBenMAPGrid is RegularGrid)
                {
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as RegularGrid).ShapefileName + ".shp"))
                    {
                        RegionMapGroup.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as RegularGrid).ShapefileName + ".shp"); //-MCB use when mapgroup layers is working correctly
                        //mainMap.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as RegularGrid).ShapefileName + ".shp");
                    }
                }
                //-MCB added region layer(s) to region mapgroup
                RegionMapGroup.Layers[0].LegendText = CommonClass.RBenMAPGrid.GridDefinitionName; //-MCB use when mapgroup layers is working correctly
                //mainMap.Layers[mainMap.Layers.Count() - 1].LegendText = CommonClass.RBenMAPGrid.GridDefinitionName;

                PolygonLayer playerRegion = RegionMapGroup.Layers[0] as PolygonLayer;  //-MCB use when mapgroup layers is working correctly
                //PolygonLayer playerRegion = mainMap.Layers[mainMap.Layers.Count - 1] as PolygonLayer;
                Color cRegion = Color.Transparent;
                PolygonSymbolizer TransparentRegion = new PolygonSymbolizer(cRegion);

                TransparentRegion.OutlineSymbolizer = new LineSymbolizer(Color.DarkBlue, 1); playerRegion.Symbolizer = TransparentRegion;
                if (isWGS84 == false)
                {
                    tsbChangeProjection_Click(null, null);
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void addRegionLayerGroupToMainMap()
        {
            try
            {
                MapPolygonLayer ReferenceLayer1 = new MapPolygonLayer(); 

                if (CommonClass.RBenMAPGrid == null)  //Get single region ID
                {
                    try
                    {
                        DataRowView drGrid = cboRegion.SelectedItem as DataRowView;
                        CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));
                    }
                    catch
                    {

                    }
                }
                //Change the projection to WGS1984 if needed
                bool isWGS84 = true;
                if (tsbChangeProjection.Text == "change projection to WGS1984")
                {
                    tsbChangeProjection_Click(null, null);
                    isWGS84 = false;
                }
                //If no region selected then take first region from region dropdown ?
                if (CommonClass.RBenMAPGrid == null)
                {
                    cboRegion.SelectedIndex = 0;
                };
                mainMap.ProjectionModeReproject = ActionMode.Never;
                mainMap.ProjectionModeDefine = ActionMode.Never;

                //-MCB  Add RegionAdmin group to the legend if it doesn't exist already---------
                 MapGroup RegionMapGroup = AddMapGroup(regionGroupLegendText, "Map Layers", false, false);
                RegionMapGroup.IsExpanded = false;
               
                //add the default region admin layer if it doen't exist already
                bool DefaultRegionLayerFound = false;
                foreach (ILayer Ilay in mainMap.GetAllLayers())
                {
                    if (Ilay.LegendText == CommonClass.RBenMAPGrid.GridDefinitionName)
                    {
                        DefaultRegionLayerFound = true;
                        break;
                    }
                }
                if (!DefaultRegionLayerFound)
                {

                    if (CommonClass.RBenMAPGrid is ShapefileGrid)
                    {
                        if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                        {
                            ReferenceLayer1 = (MapPolygonLayer)RegionMapGroup.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as ShapefileGrid).ShapefileName + ".shp");
                        }
                    }
                    else if (CommonClass.RBenMAPGrid is RegularGrid)
                    {
                        if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as RegularGrid).ShapefileName + ".shp"))
                        {
                            ReferenceLayer1 = (MapPolygonLayer)RegionMapGroup.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as RegularGrid).ShapefileName + ".shp");
                        }
                    }

                    if (CommonClass.RBenMAPGrid.GridDefinitionName == "State") CommonClass.RBenMAPGrid.GridDefinitionName = "States";
                    ReferenceLayer1.LegendText = CommonClass.RBenMAPGrid.GridDefinitionName;
                    Color cRegion = Color.Transparent;
                    PolygonSymbolizer TransparentRegion = new PolygonSymbolizer(cRegion);

                    TransparentRegion.OutlineSymbolizer = new LineSymbolizer(Color.DarkBlue, 1);
                    ReferenceLayer1.Symbolizer = TransparentRegion;
                    ReferenceLayer1.IsExpanded = false;
                    ReferenceLayer1.IsVisible = true;
                }

                //MCB - NEED to add code to handle other countries eventually !!!!!!!!!!!!!!!!!!!!
                //If Setup in china then add national boundary and regions
                // If Setup in U.S. then add States and County layers too if they don't exist on the legend already
                if (CommonClass.MainSetup.SetupName.ToLower() == "china")  
                {
                    bool ChinaNationLayFound = false;
                    bool ChinaRegionLayFound = false;
                    string ChinaDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\BenMap-CE\Data\Shapefiles\China\";  /// MCB- Should this be the PROGRAMDATA Path instead???? 
                    ChinaRegionLayFound = mainMap.GetAllLayers().Any<ILayer>(mylay => mylay.LegendText == "Nation");
                    ChinaNationLayFound = mainMap.GetAllLayers().Any<ILayer>(mylay => mylay.LegendText == "Regions");

                    //Add China Regional boundaries
                    if (!ChinaRegionLayFound)
                    {
                        if (File.Exists(ChinaDataPath + "China_Region" + ".shp"))   //*grabbing US county layer from known location
                        {
                            MapPolygonLayer RegionReferenceLayer = new MapPolygonLayer();
                            RegionReferenceLayer = (MapPolygonLayer)RegionMapGroup.Layers.Add(ChinaDataPath + "China_Region" + ".shp");
                            RegionReferenceLayer.LegendText = "Regions";
                            PolygonSymbolizer StateRegionSym = new PolygonSymbolizer(Color.Transparent);
                            StateRegionSym.OutlineSymbolizer = new LineSymbolizer(Color.DarkBlue, 1);
                            RegionReferenceLayer.Symbolizer = StateRegionSym;
                            RegionReferenceLayer.IsExpanded = false;
                            RegionReferenceLayer.IsVisible = false;
                        }
                    }

                    //Add China National border
                    if (!ChinaNationLayFound)
                    {
                        if (File.Exists(ChinaDataPath + "China_Boundary" + ".shp"))   //*grabbing China boundary layer from known location
                        {
                            MapPolygonLayer NationReferenceLayer = new MapPolygonLayer();
                            NationReferenceLayer = (MapPolygonLayer)RegionMapGroup.Layers.Add(ChinaDataPath + "China_Boundary" + ".shp");
                            NationReferenceLayer.LegendText = "Nation";
                            PolygonSymbolizer NationRegionSym = new PolygonSymbolizer(Color.Transparent);
                            NationRegionSym.OutlineSymbolizer = new LineSymbolizer(Color.Black, 1.5);
                            NationReferenceLayer.Symbolizer = NationRegionSym;
                            NationReferenceLayer.IsExpanded = false;
                            NationReferenceLayer.IsVisible = true;
                        }
                    }
                }
                else  ///Assume conterminous US (or subset of contrerminous US) setup for now
                {
                    bool CountiesLayFound = false;
                    bool StatesLayFound = false;
                    bool USNationLayFound = false;
                    string USDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\BenMap-CE\Data\Shapefiles\United States\";

                    //Add US Counties if it is not on the map yet -----------------------
                    CountiesLayFound = mainMap.GetAllLayers().Any<ILayer>(mylay => mylay.LegendText == "Counties");
                    if (!CountiesLayFound)
                    {
                        //if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + "County_epa2" + ".shp"))

                        if (File.Exists(USDataPath + "County_epa2" + ".shp"))   //*grabbing US county layer from known location
                        {
                            MapPolygonLayer CountyReferenceLayer = new MapPolygonLayer();
                            CountyReferenceLayer = (MapPolygonLayer)RegionMapGroup.Layers.Add(USDataPath + "County_epa2" + ".shp");
                            CountyReferenceLayer.LegendText = "Counties";
                            PolygonSymbolizer CountyRegionSym = new PolygonSymbolizer(Color.Transparent);
                            CountyRegionSym.OutlineSymbolizer = new LineSymbolizer(Color.LightBlue, 0.5);
                            CountyReferenceLayer.Symbolizer = CountyRegionSym;
                            CountyReferenceLayer.IsExpanded = false;
                            CountyReferenceLayer.IsVisible = false;
                        }
                    }

                    //Add US States if it is not on the map yet -----------------------
                    StatesLayFound = mainMap.GetAllLayers().Any<ILayer>(mylay => mylay.LegendText == "States");
                    if (!StatesLayFound)
                    {
                        if (File.Exists(USDataPath + "State_epa2" + ".shp"))   //*grabbing US county layer from known location
                        {
                            MapPolygonLayer StateReferenceLayer = new MapPolygonLayer();
                            StateReferenceLayer = (MapPolygonLayer)RegionMapGroup.Layers.Add(USDataPath + "State_epa2" + ".shp");
                            StateReferenceLayer.LegendText = "States";
                            PolygonSymbolizer StateRegionSym = new PolygonSymbolizer(Color.Transparent);
                            StateRegionSym.OutlineSymbolizer = new LineSymbolizer(Color.DarkBlue, 1);
                            StateReferenceLayer.Symbolizer = StateRegionSym;
                            StateReferenceLayer.IsExpanded = false;
                            StateReferenceLayer.IsVisible = false;
                        }
                    }
                    //Add US Nation border if it is not on the map yet -----------------------
                    USNationLayFound = mainMap.GetAllLayers().Any<ILayer>(mylay => mylay.LegendText == "Nation");
                    if (!USNationLayFound)
                    {
                        if (File.Exists(USDataPath + "Nation_epa2" + ".shp"))   //*grabbing US county layer from known location
                        {
                            MapPolygonLayer NationReferenceLayer = new MapPolygonLayer();
                            NationReferenceLayer = (MapPolygonLayer)RegionMapGroup.Layers.Add(USDataPath + "Nation_epa2" + ".shp");
                            NationReferenceLayer.LegendText = "Nation";
                            PolygonSymbolizer NationRegionSym = new PolygonSymbolizer(Color.Transparent);
                            NationRegionSym.OutlineSymbolizer = new LineSymbolizer(Color.Black, 1.5);
                            NationReferenceLayer.Symbolizer = NationRegionSym;
                            NationReferenceLayer.IsExpanded = false;
                            NationReferenceLayer.IsVisible = false;
                        }
                    }
                }

                //Change the projection back to it's original projection
                //MCB- NEED better way to handle each countries default projections.  Store it in the grid definition or setup maybe?  XXXXXXXXXXXXXXXXXXXXXXXXXXXXX
                if (isWGS84 == false)
                {
                    tsbChangeProjection_Click(null, null);
                }
            }
            catch (Exception ex)
            {
            }
        }
        private bool BaseControlOP(string currStat, ref TreeNode currentNode)
        {
            string msg = string.Empty;
            DialogResult rtn;
            BaseControlGroup bcg = new BaseControlGroup();
            BenMAPLine bml = new BenMAPLine();
            try
            {
                if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0) { msg = "Please select pollutant."; return false; }
                if (CommonClass.LstBaseControlGroup == null || CommonClass.LstBaseControlGroup.Count == 0) { msg = "Please select pollutant."; return false; }
                string nodeTag = currentNode.Parent.Tag.ToString();
                foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
                {
                    if (int.Parse(nodeTag) == b.Pollutant.PollutantID) { bcg = b; break; }
                }
                if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0)
                {
                    lock (CommonClass.LstAsynchronizationStates)
                    {
                        if (CommonClass.LstAsynchronizationStates.Contains(string.Format("{0}{1}", bcg.Pollutant.PollutantName.ToLower(), currStat)))
                        {
                            msg = " BenMAP is creating the air quality surface.";
                            return false; ;
                        }
                    }
                }

                GridCreationMethods frm = null;
                bool isGridTypeChanged = false;
                bool removeNode = true; switch (currStat)
                {
                    case "baseline":
                        if (bcg.Base == null)
                        {
                            bcg.Base = new BenMAPLine();
                            bcg.Base.GridType = bcg.GridType;
                            bcg.Base.Pollutant = bcg.Pollutant;
                        }
                        frm = new GridCreationMethods(bcg, currStat);
                        rtn = frm.ShowDialog();
                        if (rtn != DialogResult.OK) { return false; }
                        isGridTypeChanged = frm.isGridTypeChanged;
                        if (frm.PageStat == "monitor")
                        {
                        }
                        if (frm.PageStat == "monitorrollback" && MonitorRollbackSettings3.MakeBaselineGrid.Length > 1 && MonitorRollbackSettings3.MakeBaselineGrid.Substring(0, 1) == "T")
                        {
                            string err = "";
                            bcg.Base = DataSourceCommonClass.LoadAQGFile(MonitorRollbackSettings3.MakeBaselineGrid.Substring(1, MonitorRollbackSettings3.MakeBaselineGrid.Length - 1), ref err);
                            bml = bcg.Base;
                            removeNode = false;
                            if (CommonClass.LstBaseControlGroup != null)
                            {
                                for (int i = 0; i < CommonClass.LstBaseControlGroup.Count; i++)
                                {
                                    if (CommonClass.LstBaseControlGroup[i].Pollutant.PollutantID == bcg.Pollutant.PollutantID)
                                    {
                                        initNodeImage(currentNode.Parent.Nodes[1]);
                                        currentNode.Parent.Nodes[1].Nodes.Clear();
                                    }
                                }
                            }
                            TreeNode controlNode = currentNode.Parent.Nodes[1];
                            AddChildNodes(ref controlNode, frm.PageStat, frm.StrPath.Substring(frm.StrPath.LastIndexOf(@"\") + 1), bcg.Control);
                            changeNodeImage(currentNode.Parent.Nodes[1]);
                        }
                        else
                        {
                            bml = bcg.Base;
                            changeNodeImage(currentNode);
                        }
                        break;

                    case "control":
                        if (bcg.Control == null)
                        {
                            bcg.Control = new BenMAPLine();
                            bcg.Control.GridType = bcg.GridType;
                            bcg.Control.Pollutant = bcg.Pollutant;
                        }
                        frm = new GridCreationMethods(bcg, currStat);
                        rtn = frm.ShowDialog();
                        if (rtn != DialogResult.OK) { return false; }
                        isGridTypeChanged = frm.isGridTypeChanged;
                        if (frm.PageStat == "monitor")
                        {
                        }
                        if (frm.PageStat == "monitorrollback" && MonitorRollbackSettings3.MakeBaselineGrid.Length > 1 && MonitorRollbackSettings3.MakeBaselineGrid.Substring(0, 1) == "T")
                        {
                            string err = "";
                            bcg.Base = DataSourceCommonClass.LoadAQGFile(MonitorRollbackSettings3.MakeBaselineGrid.Substring(1, MonitorRollbackSettings3.MakeBaselineGrid.Length - 1), ref err);
                            bml = bcg.Control;
                            removeNode = false;
                            if (CommonClass.LstBaseControlGroup != null)
                            {
                                for (int i = 0; i < CommonClass.LstBaseControlGroup.Count; i++)
                                {
                                    if (CommonClass.LstBaseControlGroup[i].Pollutant.PollutantID == bcg.Pollutant.PollutantID)
                                    {
                                        initNodeImage(currentNode.Parent.Nodes[0]);
                                        currentNode.Parent.Nodes[0].Nodes.Clear();
                                    }
                                }
                            }
                            TreeNode bsaeNode = currentNode.Parent.Nodes[0];
                            AddChildNodes(ref bsaeNode, frm.PageStat, frm.StrPath.Substring(frm.StrPath.LastIndexOf(@"\") + 1), bcg.Base);
                        }
                        else
                        {
                            bml = bcg.Control;
                            changeNodeImage(currentNode);
                        }
                        break;
                }
                bcg.DeltaQ = null;
                if (isGridTypeChanged)
                {
                    if (CommonClass.LstBaseControlGroup != null)
                    {
                        for (int i = 0; i < CommonClass.LstBaseControlGroup.Count; i++)
                        {
                            if (CommonClass.LstBaseControlGroup[i].Pollutant.PollutantID != bcg.Pollutant.PollutantID)
                            {
                                CommonClass.LstBaseControlGroup[i].Base = null;
                                CommonClass.LstBaseControlGroup[i].Control = null;
                                GC.Collect();
                            }
                            else
                            {
                                switch (currStat)
                                {
                                    case "baseline":
                                        if (removeNode)
                                        {
                                            initNodeImage(currentNode.Parent.Nodes[1]);
                                            currentNode.Parent.Nodes[1].Nodes.Clear();
                                            CommonClass.LstBaseControlGroup[i].Control = null;
                                        }
                                        break;
                                    case "control":
                                        if (removeNode)
                                        {
                                            initNodeImage(currentNode.Parent.Nodes[0]);
                                            currentNode.Parent.Nodes[0].Nodes.Clear();
                                            CommonClass.LstBaseControlGroup[i].Base = null;
                                        }
                                        break;
                                }

                            }
                        }
                    }

                    for (int i = currentNode.Parent.Parent.Nodes.Count - 1; i > -1; i--)
                    {
                        TreeNode node = currentNode.Parent.Parent.Nodes[i];
                        if (currentNode.Parent.Parent.Nodes[i].Name == "datasource")
                        {
                            if (bcg.Pollutant.PollutantID != int.Parse(currentNode.Parent.Parent.Nodes[i].Tag.ToString()))
                            {
                                currentNode.Parent.Parent.Nodes[i].Nodes[0].Nodes.Clear();
                                initNodeImage(currentNode.Parent.Parent.Nodes[i].Nodes[0]);
                                currentNode.Parent.Parent.Nodes[i].Nodes[1].Nodes.Clear();
                                initNodeImage(currentNode.Parent.Parent.Nodes[i].Nodes[1]);
                                initNodeImage(currentNode.Parent.Parent.Nodes[i]);
                            }
                        }
                    }


                    if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null)
                    {
                        CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue = null;
                    }

                    olvCRFunctionResult.SetObjects(null);
                    olvIncidence.SetObjects(null);
                    tlvAPVResult.SetObjects(null);

                    cbPoolingWindowIncidence.Items.Clear();
                    cbPoolingWindowAPV.Items.Clear();
                    ClearMapTableChart();
                    CommonClass.BenMAPPopulation = null;
                    initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes[0]);
                    if (CommonClass.BaseControlCRSelectFunction != null)
                    {
                        errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes.Count - 1]);
                    }
                    if (CommonClass.lstIncidencePoolingAndAggregation != null)
                    {
                        errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
                    }
                    if (CommonClass.ValuationMethodPoolingAndAggregation != null)
                    {
                        errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
                    }
                }
                string strName = "";
                try
                {
                    strName = frm.StrPath.Substring(frm.StrPath.LastIndexOf(@"\") + 1);
                }
                catch
                { }
                AddChildNodes(ref currentNode, frm.PageStat, strName, bml);
                bcg.DeltaQ = null;
                currentNode.ExpandAll();
                if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null)
                {
                    CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
                }
                if (bcg.Base != null && bcg.Control != null) changeNodeImage(currentNode.Parent);
                if (CommonClass.BaseControlCRSelectFunction != null)
                {
                    errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes[1]);
                }
                if (CommonClass.lstIncidencePoolingAndAggregation != null)
                {
                    errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[1]);
                }
                if (CommonClass.ValuationMethodPoolingAndAggregation != null)
                {
                    errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[2]);
                }
                int index = currentNode.Parent.Index;
                TreeNode parent = currentNode.Parent.Parent;
                BrushBaseControl(ref parent, bcg, index);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
            finally
            {
                if (msg != string.Empty)
                { MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            }
        }

        private void BrushBaseControl(ref TreeNode pnode, BaseControlGroup bcg, int index)
        {
            try
            {
                TreeNode node = new TreeNode()
                 {
                     Name = "datasource",
                     Tag = bcg.Pollutant.PollutantID,
                     Text = string.Format("Source of Air Quality Data ({0})", bcg.Pollutant.PollutantName),
                     ToolTipText = "Double-click to upload AQ data files",
                     ImageKey = (bcg.Base == null || bcg.Control == null) ? _unreadyImageKey : _readyImageKey,
                     SelectedImageKey = (bcg.Base == null || bcg.Control == null) ? _unreadyImageKey : _readyImageKey,
                     Nodes = { new TreeNode() {
                            Name = "baseline",
                            Text = "Baseline",
                            ToolTipText="Double-click to load AQ data",
                            Tag = bcg.Base,
                            ImageKey = (CommonClass.LstAsynchronizationStates!=null &&CommonClass.LstAsynchronizationStates.Contains(bcg.Base.Pollutant.PollutantName.ToLower()+"baseline"))? _yibuImageKey:(bcg.Base == null || bcg.Base.ModelResultAttributes==null || bcg.Base.ModelResultAttributes.Count==0 ) ? _unreadyImageKey : _readyImageKey,
                            SelectedImageKey =(CommonClass.LstAsynchronizationStates!=null &&CommonClass.LstAsynchronizationStates.Contains(bcg.Base.Pollutant.PollutantName.ToLower()+"baseline"))? _yibuImageKey:(bcg.Base == null || bcg.Base.ModelResultAttributes==null || bcg.Base.ModelResultAttributes.Count==0 ) ? _unreadyImageKey : _readyImageKey,
                            },
                            new TreeNode() {
                                Name = "control",
                                Text = "Control",
                                ToolTipText="Double-click to load AQ data",
                                Tag = bcg.Control,
                            ImageKey = (CommonClass.LstAsynchronizationStates!=null &&CommonClass.LstAsynchronizationStates.Contains(bcg.Base.Pollutant.PollutantName.ToLower()+"control"))? _yibuImageKey:(bcg.Control == null || bcg.Control.ModelResultAttributes==null || bcg.Control.ModelResultAttributes.Count==0 ) ? _unreadyImageKey : _readyImageKey,
                            SelectedImageKey =(CommonClass.LstAsynchronizationStates!=null &&CommonClass.LstAsynchronizationStates.Contains(bcg.Base.Pollutant.PollutantName.ToLower()+"control"))? _yibuImageKey:(bcg.Control == null || bcg.Control.ModelResultAttributes==null || bcg.Control.ModelResultAttributes.Count==0 ) ? _unreadyImageKey : _readyImageKey},
                            new TreeNode() {
                                Name = "delta",
                                Text = "Air quality delta (baseline - control)",
                                ToolTipText="Double-click to load AQ data",
                                Tag = bcg,
                             ImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0 &&(bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) )? "doc" :"docgrey",
                        SelectedImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0 &&(bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) )? "doc" :"docgrey", }
                }
                 };
                if (bcg.Base != null)
                {
                    string s = "";
                    try
                    {
                        //ModelDataLine mdl = (BenMAPLine)bcg.Base;
                        //String test = mdl.DatabaseFilePath;
                        //bcg.Base.da
                        s = (bcg.Base as ModelDataLine).DatabaseFilePath.Substring((bcg.Base as ModelDataLine).DatabaseFilePath.LastIndexOf(@"\") + 1);

                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Broken cast "+e.ToString());
                    }
                    node.Nodes[0].Nodes.Add(new TreeNode()
                    {
                        Name = "basedata",
                        Text = (bcg.Base is ModelDataLine) ? s : "Base Data",
                        ToolTipText = "Double-click AQ data file to display map/data",
                        Tag = bcg.Base,
                        ImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
                        SelectedImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
                    });
                }
                if (bcg.Control != null)
                {
                    string s = "";
                    try
                    {
                        s = (bcg.Control as ModelDataLine).DatabaseFilePath.Substring((bcg.Control as ModelDataLine).DatabaseFilePath.LastIndexOf(@"\") + 1);

                    }
                    catch
                    { }
                    node.Nodes[1].Nodes.Add(new TreeNode()
                    {
                        Name = "controldata",
                        Text = (bcg.Control is ModelDataLine) ? s : "Control Data",
                        ToolTipText = "Double-click AQ data file to display map/data",
                        Tag = bcg.Control,
                        ImageKey = (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
                        SelectedImageKey = (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
                    });
                }
                if (CommonClass.LstPollutant != null && CommonClass.LstPollutant.Count > 0)
                {
                    changeNodeImage(trvSetting.Nodes[0].Nodes[0]);
                }
                node.ExpandAll();
                pnode.Nodes.RemoveAt(index);
                pnode.Nodes.Insert(index, node);
                pnode.ExpandAll();
                if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null)
                {
                    CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
                }
                { changeNodeImage(pnode); }
                if (CommonClass.BaseControlCRSelectFunction != null)
                {
                    errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes[1]);
                }
                if (CommonClass.lstIncidencePoolingAndAggregation != null)
                {
                    errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[1]);
                }
                if (CommonClass.ValuationMethodPoolingAndAggregation != null)
                {
                    errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[2]);
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void AddChildNodes(ref TreeNode node, string currStat, string txt, BenMAPLine bml)
        {
            try
            {
                if (node == null) { return; }
                string nodeName = node.Name; ;
                string tag = string.Empty;
                TreeNode newNode;
                BaseControlGroup bcg = null;
                switch (nodeName)
                {
                    case "gridtype":
                        if (CommonClass.GBenMAPGrid == null) { return; }
                        node.Nodes.Clear(); newNode = CreateNewNode(CommonClass.GBenMAPGrid, "Grid");
                        if (newNode != null) { node.Nodes.Add(newNode); }
                        if (CommonClass.RBenMAPGrid != null)
                        {
                            string target = "Domain";
                            BenMAPGrid grid = CommonClass.RBenMAPGrid;
                            newNode = new TreeNode
                            {
                                Name = "region",
                                Text = string.Format("{0}: {1}", target, grid.GridDefinitionName),
                                Tag = grid,
                                ToolTipText = "Double-click domain data file to display ",
                                ImageKey = "doc",
                                SelectedImageKey = "doc",
                            };
                            if (newNode != null) { node.Nodes.Add(newNode); }
                        }
                        break;
                    case "baseline":
                        node.Nodes.Clear();
                        foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
                        {
                            if (int.Parse(node.Parent.Tag.ToString()) == b.Pollutant.PollutantID) { bcg = b; break; }
                        }
                        if (string.IsNullOrEmpty(txt)) { txt = "data from library"; }
                        newNode = new TreeNode()
                        {
                            Name = "basedata",
                            Text = txt,
                            Tag = bml,
                            ToolTipText = "Double-click AQ data file to display ",
                            ImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
                            SelectedImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
                        };
                        node.Nodes.Add(newNode);
                        break;
                    case "control":
                        node.Nodes.Clear(); foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
                        {
                            if (int.Parse(node.Parent.Tag.ToString()) == b.Pollutant.PollutantID) { bcg = b; break; }
                        }
                        newNode = new TreeNode()
                        {
                            Name = "controldata",
                            Text = txt,
                            Tag = bml,
                            ToolTipText = "Double-click AQ data file to display ",
                            ImageKey = (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
                            SelectedImageKey = (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
                        };
                        node.Nodes.Add(newNode);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private TreeNode CreateNewNode(BenMAPGrid grid, string target)
        {
            TreeNode node;
            try
            {
                node = new TreeNode()
                {
                    Name = target.ToLower(),
                    Text = string.Format("{0}: {1}", target, grid.GridDefinitionName),

                    Tag = grid,
                    ToolTipText = "Double-click grid data file to display ",
                    ImageKey = "doc",
                    SelectedImageKey = "doc",
                };

                return node;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return null;
            }
        }
        private void initNodeImage(TreeNode rootNode)
        {
            try
            {
                rootNode.ImageKey = _unreadyImageKey;
                rootNode.SelectedImageKey = _unreadyImageKey;
            }
            catch
            {
            }
        }
        private void errorNodeImage(TreeNode rootNode)
        {

            try
            {
                rootNode.ImageKey = _errorImageKey;
                rootNode.SelectedImageKey = _errorImageKey;
            }
            catch
            {
            }
        }
        private void changeNodeImage(TreeNode rootNode)
        {
            bool hasUnRead = false;
            try
            {
                hasUnRead = JudgeStatus(rootNode);
                if (hasUnRead) { return; }
                rootNode.ImageKey = _readyImageKey;
                rootNode.SelectedImageKey = _readyImageKey;

                rootNode.ExpandAll();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        private bool JudgeStatus(TreeNode node)
        {
            try
            {
                bool hasUnRead = false;
                string nodeName = node.Name;
                switch (nodeName)
                {
                    case "airqualitygridgroup":
                    case "configuration":
                    case "aggregationpoolingvaluation":
                        hasUnRead = true;
                        break;
                }
                return hasUnRead;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void btnSpatial_Click(object sender, EventArgs e)
        {
        }





        private void ShowTable(string file)
        {

            _reportTableFileName = file;
            tabCtlMain.SelectTab(tabData);
        }


        private void ShowChart(string resultFile)
        {
            zedGraphCtl.Visible = true;
            ZedGraphResult(zedGraphCtl, resultFile);
            zedGraphCtl.AxisChange();
            zedGraphCtl.Refresh();
            tabCtlMain.SelectTab(tabChart);
        }

        private void ZedGraphResult(ZedGraphControl zgc, string file)
        {
            try
            {
                System.Data.DataTable dt = DataSourceCommonClass.getDataSetFromCSV(file).Tables[0]; System.Data.DataSet dsOut = new System.Data.DataSet();

                int i = 0;
                int irepeat = 0;

                while (i < 5)
                {
                    if (dt.Rows[0][0].ToString() == dt.Rows[i][0].ToString() &&
                        dt.Rows[0][1].ToString() == dt.Rows[i][1].ToString())
                    {
                        irepeat = i + 1;
                    }
                    i++;
                }
                DataTable dt1 = dt.Clone();
                DataTable dt2 = dt.Clone();
                dt2.TableName = "Table2";
                DataTable dt3 = dt.Clone();
                dt3.TableName = "Table3";

                dt1.Rows.Clear();
                dt2.Rows.Clear();
                dt3.Rows.Clear();

                dsOut.Tables.Add(dt1);
                dsOut.Tables.Add(dt2);
                dsOut.Tables.Add(dt3);

                if (irepeat > 0)
                {
                    while (i < dt.Rows.Count / irepeat)
                    {
                        if (irepeat > 1)
                        {
                            dsOut.Tables[0].Rows.Add(dt.Rows[i * irepeat + 0].ItemArray);
                            dsOut.Tables[1].Rows.Add(dt.Rows[i * irepeat + 1].ItemArray);
                        }
                        if (irepeat > 2)
                        {
                            dsOut.Tables[2].Rows.Add(dt.Rows[i * irepeat + 2].ItemArray);
                        }
                        i++;
                    }
                }
                if (dsOut.Tables[2].Rows.Count == 0) dsOut.Tables.RemoveAt(2);
                if (dsOut.Tables[1].Rows.Count == 0) dsOut.Tables.RemoveAt(1);
                if (dsOut.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("");
                    return;
                }
                Dictionary<string, double>[] dicRegionValues = new Dictionary<string, double>[irepeat];
                i = 0;
                while (i < irepeat)
                {
                    dicRegionValues[i] = addRegionValue(dsOut.Tables[i]);
                    i++;
                }


                string[] strValuations = { "Woodruff,etc.", "Laden,etc.", "Pope,etc." };
                List<string> strValuationsNow = new List<string>();
                GraphPane myPane = zgc.GraphPane;
                myPane.Title.Text = "Valuation Result";
                myPane.XAxis.Title.Text = "Region";
                myPane.YAxis.Title.Text = "Monetary Value($)";
                myPane.CurveList.Clear();
                i = 0;
                Color[] colorArray = new Color[] { Color.Blue, Color.Red, Color.Green };
                while (i < irepeat)
                {
                    PointPairList list = new PointPairList();
                    int j = 0;
                    while (j < 5)
                    {
                        list.Add(new PointPair(Convert.ToInt32(j), dicRegionValues[i].ToArray()[j].Value));
                        j++;
                    }

                    BarItem myCurve = myPane.AddBar(strValuationsNow[i], list, colorArray[i]);

                    i++;
                }
                myPane.Chart.Fill = new Fill(Color.White,
                 Color.FromArgb(255, 255, 166), 45.0F);

                myPane.XAxis.Scale.TextLabels = new string[] { dicRegionValues[0].Keys.ToArray()[0], dicRegionValues[0].Keys.ToArray()[1], dicRegionValues[0].Keys.ToArray()[2], dicRegionValues[0].Keys.ToArray()[3], dicRegionValues[0].Keys.ToArray()[4] };
                myPane.XAxis.Type = AxisType.Text;
                myPane.XAxis.Scale.FontSpec.Angle = 65;
                myPane.XAxis.Scale.FontSpec.IsBold = true;
                myPane.XAxis.Scale.FontSpec.Size = 12;
                zgc.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;

                zgc.AxisChange();
                zgc.Refresh();


            }
            catch (Exception err)
            {
                Logger.LogError(err);
            }
        }

        private Dictionary<string, double> addRegionValue(DataTable dt)
        {
            Dictionary<string, double> DicRegionValue = new Dictionary<string, double>();
            int im = 0, iCmaqGrid = 0, i = 0, j = 0, iRegion = 0;

            i = 0;
            List<ModelAttribute> lsdt = new List<ModelAttribute>();


            return DicRegionValue;
        }

        private void ZedGraphDemo(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;
            string[] str = { "North", "South", "West", "East", "Central" };

            myPane.Title.Text = "Vertical Bars with Value Labels Above Each Bar";
            myPane.XAxis.Title.Text = "Position Number";
            myPane.YAxis.Title.Text = "Some Random Thing";

            PointPairList list = new PointPairList();
            PointPairList list2 = new PointPairList();
            PointPairList list3 = new PointPairList();
            Random rand = new Random();


            for (int i = 0; i < 5; i++)
            {
                double x = (double)i;
                double y = rand.NextDouble() * 1000;
                double y2 = rand.NextDouble() * 1000;
                double y3 = rand.NextDouble() * 1000;
                list.Add(x, y, 0, "aaaa");
                list2.Add(x, y2);
                list3.Add(x, y3);
            }

            BarItem myCurve = myPane.AddBar("curve 1", list, Color.Blue);
            BarItem myCurve2 = myPane.AddBar("curve 2", list2, Color.Red);
            BarItem myCurve3 = myPane.AddBar("curve 3", list3, Color.Green);

            myPane.Chart.Fill = new Fill(Color.White,
    Color.FromArgb(255, 255, 166), 45.0F);

            myPane.XAxis.Scale.TextLabels = str;
            myPane.XAxis.Type = AxisType.Text;
            BarItem.CreateBarLabels(myPane, false, "f0");
            zgc.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;

            zgc.AxisChange();
            zgc.Refresh();

        }

        private void ShowCumulative()
        {
            zedGraphCtl.Visible = false;
            System.Drawing.Image backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\Cumulative Distributions.JPG");
            pnlChart.BackgroundImage = backImg;
            tabCtlMain.SelectTab(tabChart);
        }

        private void ShowBoxPlot()
        {
            zedGraphCtl.Visible = false;
            System.Drawing.Image backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\BoxPlot.jpg");
            pnlChart.BackgroundImage = backImg;
            tabCtlMain.SelectTab(tabChart);
        }










        private bool ExportDataset2CSV(System.Data.DataSet ds, string fileName)
        {
            try
            {
                StreamWriter swriter = new StreamWriter(fileName, false, Encoding.Default);
                string str = string.Empty;
                str = string.Format("Column,Row,ValuationResult");
                swriter.WriteLine(str);

                int tableCount = ds.Tables.Count;
                int rowCount = ds.Tables[0].Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < tableCount; j++)
                    {
                        if (ds.Tables[j].Rows.Count == 0) continue;

                        str = string.Format("{0},{1},{2}", ds.Tables[j].Rows[i][0].ToString(), ds.Tables[j].Rows[i][1].ToString(), ds.Tables[j].Rows[i][2].ToString());

                        swriter.WriteLine(str);
                    }
                }


                swriter.Flush(); swriter.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }






        public string _outputFileName;
        public void btnTableOutput_Click(object sender, EventArgs e)
        {
            if (_tableObject != null)
            {
                bool isBatch = false;

                if (CommonClass.InputParams != null && CommonClass.InputParams.Count() > 0 && CommonClass.InputParams[0].ToLower().Contains(".ctlx"))
                {
                    isBatch = true;
                }
                try
                {

                    if (!isBatch)
                    {
                        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                        saveFileDialog1.Filter = "CSV File|*.CSV";
                        if (_tableObject is CRSelectFunctionCalculateValue || _tableObject is Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> || _tableObject is List<CRSelectFunctionCalculateValue>)
                            saveFileDialog1.InitialDirectory = CommonClass.ResultFilePath + @"\Result\CFGR";
                        else if (_tableObject is BenMAPLine)
                            saveFileDialog1.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG";
                        else if (_tableObject is List<AllSelectValuationMethodAndValue> || _tableObject is AllSelectValuationMethodAndValue)
                            saveFileDialog1.InitialDirectory = CommonClass.ResultFilePath + @"\Result\APVR";
                        else if (_tableObject is List<AllSelectCRFunction>)
                            saveFileDialog1.InitialDirectory = CommonClass.ResultFilePath + @"\Result\APVR";

                        if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                        {
                            return;
                        }

                        _outputFileName = saveFileDialog1.FileName;
                    }
                    int i = 0;
                    DataTable dt = new DataTable();
                    if (_tableObject is List<AllSelectCRFunction>)
                    {
                        List<AllSelectCRFunction> lstAllSelectCRFuntion = (List<AllSelectCRFunction>)_tableObject;
                        if (this.IncidencelstColumnRow == null)
                        {
                            dt.Columns.Add("Col", typeof(int));
                            dt.Columns.Add("Row", typeof(int));
                        }
                        else
                        {
                            foreach (FieldCheck fieldCheck in IncidencelstColumnRow)
                            {

                                if (fieldCheck.isChecked)
                                {
                                    dt.Columns.Add(getFieldNameFromlstHealth(fieldCheck.FieldName), typeof(int));
                                }
                            }
                        }

                        if (IncidencelstHealth != null)
                        {
                            foreach (FieldCheck fieldCheck in IncidencelstHealth)
                            {
                                if (fieldCheck.FieldName.ToLower() == "version" && fieldCheck.isChecked)
                                {
                                    dt.Columns.Add("Version", typeof(string));

                                }
                                else if (fieldCheck.isChecked)
                                {
                                    dt.Columns.Add(fieldCheck.FieldName, typeof(string));
                                }
                            }
                        }
                        // JHA - Added for testing
                        if (true || lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().BetaVariationName != null)
                        {
                            dt.Columns.Add("Seasonal", typeof(string));
                        }
                        if (IncidencelstResult == null)
                        {
                            dt.Columns.Add("Point Estimate", typeof(double));
                            dt.Columns.Add("Population", typeof(double));

                            // set up delta columns dynamically for multipollutant
                            if (lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().DeltaList != null)
                            {
                                int dCount = lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().DeltaList.Count();

                                for (int j = 0; j < dCount; j++)
                                {
                                    string pollName = lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Variables[j].PollutantName;
                                    dt.Columns.Add("Delta_" + pollName, typeof(double));
                                }
                            }
                            else
                            {
                                dt.Columns.Add("Delta", typeof(double));
                            }

                            dt.Columns.Add("Mean", typeof(double));
                            dt.Columns.Add("Baseline", typeof(double));
                            dt.Columns.Add("Percent Of Baseline", typeof(double));
                            dt.Columns.Add("Standard Deviation", typeof(double));
                            dt.Columns.Add("Variance", typeof(double));
                        }
                        else
                        {
                            foreach (FieldCheck fieldCheck in IncidencelstResult)
                            {

                                if (fieldCheck.isChecked && fieldCheck.FieldName != IncidencelstResult.Last().FieldName)
                                {
                                    dt.Columns.Add(fieldCheck.FieldName, typeof(double));
                                }
                            }
                        }
                        if (lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue != null &&
                            lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues != null &&
                            lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile != null)
                        {
                            while (i < lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count())
                            {

                                dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()))))), typeof(double));
                                i++;
                            }
                        }

                        foreach (AllSelectCRFunction cr in lstAllSelectCRFuntion)
                        {
                            foreach (CRCalculateValue crcv in cr.CRSelectFunctionCalculateValue.CRCalculateValues)
                            {
                                DataRow dr = dt.NewRow();
                                if (dt.Columns.Contains("Col")) dr["Col"] = crcv.Col;
                                if (dt.Columns.Contains("Row")) dr["Row"] = crcv.Row;
                                if (IncidencelstHealth != null)
                                {
                                    foreach (FieldCheck fieldCheck in IncidencelstHealth)
                                    {
                                        if (fieldCheck.FieldName.ToLower() == "version" && fieldCheck.isChecked)
                                        {
                                            dr["Version"] = cr.Version;

                                        }
                                        else if (fieldCheck.isChecked)
                                        {
                                            dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, crcv, cr.CRSelectFunctionCalculateValue.CRSelectFunction);
                                        }
                                    }
                                }
                                if (dt.Columns.Contains("Seasonal")) dr["Seasonal"] = crcv.BetaName;
                                if (dt.Columns.Contains("Point Estimate")) dr["Point Estimate"] = crcv.PointEstimate;
                                if (dt.Columns.Contains("Population")) dr["Population"] = crcv.Population;

                                if (crcv.DeltaList != null)
                                {
                                    string pollName, tempKey;
                                    int dCount = crcv.DeltaList.Count();
                                    for (int j = 0; j < dCount; j++)
                                    {
                                        pollName = cr.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Variables[j].PollutantName;
                                        tempKey = "Delta_" + pollName;

                                        if (dt.Columns.Contains(tempKey)) dr[tempKey] = crcv.DeltaList[j];
                                    }
                                }
                                else
                                {
                                    if (dt.Columns.Contains("Delta")) dr["Delta"] = crcv.Delta;
                                }

                                if (dt.Columns.Contains("Mean")) dr["Mean"] = crcv.Mean;
                                if (dt.Columns.Contains("Baseline")) dr["Baseline"] = crcv.Baseline;
                                if (dt.Columns.Contains("Percent Of Baseline")) dr["Percent Of Baseline"] = crcv.PercentOfBaseline;
                                if (dt.Columns.Contains("Standard Deviation")) dr["Standard Deviation"] = crcv.StandardDeviation;
                                if (dt.Columns.Contains("Variance")) dr["Variance"] = crcv.Variance;
                                i = 0;
                                if (cr.CRSelectFunctionCalculateValue.CRCalculateValues != null && cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile != null)
                                {
                                    while (i < cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count())
                                    {

                                        dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count())))))] = crcv.LstPercentile[i];
                                        i++;
                                    }
                                }
                                dt.Rows.Add(dr);
                            }

                        }
                        CommonClass.SaveCSV(dt, _outputFileName);

                    }
                    if (_tableObject is CRSelectFunctionCalculateValue)
                    {
                        CRSelectFunctionCalculateValue crTable = (CRSelectFunctionCalculateValue)_tableObject;
                        dt.Columns.Add("Col", typeof(int));
                        dt.Columns.Add("Row", typeof(int));
                        if (crTable.CRCalculateValues.First().BetaVariationName != null)
                        {
                            dt.Columns.Add("Seasonal", typeof(string));
                        }
                        dt.Columns.Add("Point Estimate", typeof(double));
                        dt.Columns.Add("Population", typeof(double));

                        // set up delta columns dynamically for multipollutant
                        if (crTable.CRCalculateValues.First().DeltaList != null)
                        {
                            int dCount = crTable.CRCalculateValues.First().DeltaList.Count();

                            for (int j = 0; j < dCount; j++)
                            {
                                string pollName = crTable.CRSelectFunction.BenMAPHealthImpactFunction.Variables[j].PollutantName;
                                dt.Columns.Add("Delta_" + pollName, typeof(double));
                            }
                        }
                        else
                        {
                            dt.Columns.Add("Delta", typeof(double));
                        }

                        dt.Columns.Add("Mean", typeof(double));
                        dt.Columns.Add("Baseline", typeof(double));
                        dt.Columns.Add("Percent Of Baseline", typeof(double));
                        dt.Columns.Add("Standard Deviation", typeof(double));
                        dt.Columns.Add("Variance", typeof(double));
                        i = 0;
                        while (i < crTable.CRCalculateValues.First().LstPercentile.Count())
                        {

                            dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()))))), typeof(double));
                            i++;
                        }

                        foreach (CRCalculateValue crcv in crTable.CRCalculateValues)
                        {
                            DataRow dr = dt.NewRow();
                            dr["Col"] = crcv.Col;
                            dr["Row"] = crcv.Row;
                            if (crcv.BetaVariationName != null)
                            {
                                dr["Seasonal"] = crcv.BetaName;
                            }
                            dr["Point Estimate"] = crcv.PointEstimate;
                            dr["Population"] = crcv.Population;

                            if (crcv.DeltaList != null)
                            {
                                string pollName, tempKey;
                                int dCount = crcv.DeltaList.Count();
                                for (int j = 0; j < dCount; j++)
                                {
                                    pollName = crTable.CRSelectFunction.BenMAPHealthImpactFunction.Variables[j].PollutantName;
                                    tempKey = "Delta_" + pollName;

                                    if (dt.Columns.Contains(tempKey)) dr[tempKey] = crcv.DeltaList[j];
                                }
                            }
                            else
                            {
                                if (dt.Columns.Contains("Delta")) dr["Delta"] = crcv.Delta;
                            }

                            dr["Mean"] = crcv.Mean;
                            dr["Baseline"] = crcv.Baseline;
                            dr["Percent Of Baseline"] = crcv.PercentOfBaseline;
                            dr["Standard Deviation"] = crcv.StandardDeviation;
                            dr["Variance"] = crcv.Variance;
                            i = 0;
                            if (crTable.CRCalculateValues != null && crTable.CRCalculateValues.First().LstPercentile != null)
                            {
                                while (i < crTable.CRCalculateValues.First().LstPercentile.Count())
                                {

                                    dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count())))))] = crcv.LstPercentile[i];
                                    i++;
                                }
                            }
                            dt.Rows.Add(dr);
                        }
                        CommonClass.SaveCSV(dt, _outputFileName);
                    }
                    else if (_tableObject is Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>)
                    {
                        Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicAPV = _tableObject as Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>;
                        if (cflstColumnRow == null)
                        {
                            dt.Columns.Add("Col", typeof(int));
                            dt.Columns.Add("Row", typeof(int));
                        }
                        else
                        {
                            foreach (FieldCheck fieldCheck in cflstColumnRow)
                            {

                                if (fieldCheck.isChecked)
                                {
                                    dt.Columns.Add(fieldCheck.FieldName);
                                }
                            }
                        }
                        if (cflstHealth != null)
                        {
                            foreach (FieldCheck fieldCheck in cflstHealth)
                            {

                                if (fieldCheck.isChecked)
                                {
                                    dt.Columns.Add(fieldCheck.FieldName);
                                }
                            }
                        }
                        if (dicAPV.First().Key.Key.BetaVariationName != null)
                        {
                            dt.Columns.Add("Seasonal", typeof(string));
                        }
                        if (cflstResult == null)
                        {

                            dt.Columns.Add("Point Estimate", typeof(double));
                            dt.Columns.Add("Population", typeof(double));
                            dt.Columns.Add("Delta", typeof(double));

                            // set up delta columns dynamically for multipollutant
                            if (dicAPV.First().Key.Key.DeltaList != null)
                            {
                                int dCount = dicAPV.First().Key.Key.DeltaList.Count();

                                for (int j = 0; j < dCount; j++)
                                {
                                    string pollName = dicAPV.First().Value.BenMAPHealthImpactFunction.Variables[j].PollutantName;
                                    dt.Columns.Add("Delta_" + pollName, typeof(double));
                                }
                            }
                            else
                            {
                                dt.Columns.Add("Delta", typeof(double));
                            }

                            dt.Columns.Add("Mean", typeof(double));
                            dt.Columns.Add("Baseline", typeof(double));
                            dt.Columns.Add("Percent Of Baseline", typeof(double));
                            dt.Columns.Add("Standard Deviation", typeof(double));
                            dt.Columns.Add("Variance", typeof(double));
                        }
                        else
                        {
                            foreach (FieldCheck fieldCheck in cflstResult)
                            {

                                if (fieldCheck.isChecked && fieldCheck.FieldName != cflstResult.Last().FieldName && fieldCheck.FieldName != "Population Weighted Delta"
                                    && fieldCheck.FieldName != "Population Weighted Base" && fieldCheck.FieldName != "Population Weighted Control")
                                {
                                    dt.Columns.Add(fieldCheck.FieldName);
                                }
                            }
                        }
                        if (dicAPV.First().Key.Key.LstPercentile != null)
                        {
                            i = 0;
                            while (i < dicAPV.First().Key.Key.LstPercentile.Count())
                            {
                                if (cflstResult == null || cflstResult.Last().isChecked)
                                {
                                    dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(dicAPV.First().Key.Key.LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(dicAPV.First().Key.Key.LstPercentile.Count()))))), typeof(double));
                                }
                                i++;
                            }
                        }
                        foreach (KeyValuePair<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> k in dicAPV)
                        {
                            CRCalculateValue crcv = k.Key.Key;

                            DataRow dr = dt.NewRow();
                            if (cflstColumnRow == null)
                            {
                                dr["Col"] = crcv.Col;
                                dr["Row"] = crcv.Row;
                            }
                            else
                            {
                                foreach (FieldCheck fieldCheck in cflstColumnRow)
                                {

                                    if (fieldCheck.isChecked)
                                    {
                                        dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, crcv, k.Value);
                                    }
                                }
                            }
                            if (cflstHealth != null)
                            {
                                foreach (FieldCheck fieldCheck in cflstHealth)
                                {

                                    if (fieldCheck.isChecked)
                                    {
                                        dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, crcv, k.Value);
                                    }
                                }
                            }
                            if (crcv.BetaVariationName != null)
                            {
                                dt.Columns.Add("Seasonal", typeof(string));
                            }
                            if (cflstResult == null)
                            {

                                dr["Point Estimate"] = crcv.PointEstimate;
                                dr["Population"] = crcv.Population;
                                dr["Delta"] = crcv.Delta;

                                if (crcv.DeltaList != null)
                                {
                                    string pollName, tempKey;
                                    int dCount = crcv.DeltaList.Count();
                                    for (int j = 0; j < dCount; j++)
                                    {
                                        pollName = k.Value.BenMAPHealthImpactFunction.Variables[j].PollutantName;
                                        tempKey = "Delta_" + pollName;

                                        if (dt.Columns.Contains(tempKey)) dr[tempKey] = crcv.DeltaList[j];
                                    }
                                }
                                else
                                {
                                    if (dt.Columns.Contains("Delta")) dr["Delta"] = crcv.Delta;
                                }

                                dr["Mean"] = crcv.Mean;
                                dr["Baseline"] = crcv.Baseline;
                                dr["Percent of Baseline"] = crcv.PercentOfBaseline;
                                dr["Standard Deviation"] = crcv.StandardDeviation;
                                dr["Variance"] = crcv.Variance;
                            }
                            else
                            {
                                foreach (FieldCheck fieldCheck in cflstResult)
                                {

                                    if (fieldCheck.isChecked && fieldCheck.FieldName != cflstResult.Last().FieldName && fieldCheck.FieldName != "Population Weighted Delta"
                                    && fieldCheck.FieldName != "Population Weighted Base" && fieldCheck.FieldName != "Population Weighted Control")
                                    {
                                        dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, crcv, k.Value);
                                    }
                                }
                            }


                            i = 0;
                            if ((cflstResult == null || cflstResult.Last().isChecked) && crcv.LstPercentile != null)
                            {
                                while (i < crcv.LstPercentile.Count())
                                {

                                    dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crcv.LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(crcv.LstPercentile.Count())))))] = crcv.LstPercentile[i];
                                    i++;
                                }
                            }


                            dt.Rows.Add(dr);
                        }

                        CommonClass.SaveCSV(dt, _outputFileName);
                    }
                    else if (_tableObject is List<CRSelectFunctionCalculateValue>)
                    {
                        List<CRSelectFunctionCalculateValue> lstCRTable = (List<CRSelectFunctionCalculateValue>)_tableObject;
                        if (this.tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Tag.ToString() == "incidence")
                        {
                            if (IncidencelstColumnRow == null)
                            {
                                dt.Columns.Add("Col", typeof(int));
                                dt.Columns.Add("Row", typeof(int));
                            }
                            else
                            {
                                foreach (FieldCheck fieldCheck in IncidencelstColumnRow)
                                {

                                    if (fieldCheck.isChecked)
                                    {
                                        dt.Columns.Add(fieldCheck.FieldName);
                                    }
                                }
                            }
                            if (IncidencelstHealth != null)
                            {
                                foreach (FieldCheck fieldCheck in IncidencelstHealth)
                                {

                                    if (fieldCheck.isChecked)
                                    {
                                        dt.Columns.Add(fieldCheck.FieldName);
                                    }
                                }
                            }
                            if (lstCRTable.First().CRCalculateValues.First().BetaVariationName != null)
                            {
                                dt.Columns.Add("Seasonal", typeof(string));
                            }
                            if (IncidencelstResult == null)
                            {

                                dt.Columns.Add("Point Estimate", typeof(double));
                                dt.Columns.Add("Population", typeof(double));

                                // set up delta columns dynamically for multipollutant
                                if (lstCRTable.First().CRCalculateValues.First().DeltaList != null)
                                {
                                    int dCount = lstCRTable.First().CRCalculateValues.First().DeltaList.Count();

                                    for (int j = 0; j < dCount; j++)
                                    {
                                        string pollName = lstCRTable.First().CRSelectFunction.BenMAPHealthImpactFunction.Variables[j].PollutantName;
                                        dt.Columns.Add("Delta_" + pollName, typeof(double));
                                    }
                                }
                                else
                                {
                                    dt.Columns.Add("Delta", typeof(double));
                                }

                                dt.Columns.Add("Mean", typeof(double));
                                dt.Columns.Add("Baseline", typeof(double));
                                dt.Columns.Add("Percent of Baseline", typeof(double));
                                dt.Columns.Add("Standard Deviation", typeof(double));
                                dt.Columns.Add("Variance", typeof(double));
                            }
                            else
                            {
                                foreach (FieldCheck fieldCheck in IncidencelstResult)
                                {

                                    if (fieldCheck.isChecked && fieldCheck.FieldName != IncidencelstResult.Last().FieldName && fieldCheck.FieldName != "Population Weighted Delta"
                                        && fieldCheck.FieldName != "Population Weighted Base" && fieldCheck.FieldName != "Population Weighted Control")
                                    {
                                        dt.Columns.Add(fieldCheck.FieldName);
                                    }
                                }
                            }

                            if (lstCRTable.First().CRCalculateValues.First().LstPercentile != null)
                            {
                                i = 0;
                                while (i < lstCRTable.First().CRCalculateValues.First().LstPercentile.Count())
                                {
                                    if (IncidencelstResult == null || IncidencelstResult.Last().isChecked)
                                    {
                                        dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()))))), typeof(double));
                                    }
                                    i++;
                                }
                            }
                        }
                        else
                        {
                            if (cflstColumnRow == null)
                            {
                                dt.Columns.Add("Col", typeof(int));
                                dt.Columns.Add("Row", typeof(int));
                            }
                            else
                            {
                                foreach (FieldCheck fieldCheck in cflstColumnRow)
                                {

                                    if (fieldCheck.isChecked)
                                    {
                                        dt.Columns.Add(fieldCheck.FieldName);
                                    }
                                }
                            }
                            if (cflstHealth != null)
                            {
                                foreach (FieldCheck fieldCheck in cflstHealth)
                                {

                                    if (fieldCheck.isChecked)
                                    {
                                        dt.Columns.Add(fieldCheck.FieldName);
                                    }
                                }
                            }
                            if (lstCRTable.First().CRCalculateValues.First().BetaVariationName != null)
                            {
                                dt.Columns.Add("Seasonal", typeof(string));
                            }
                            if (cflstResult == null)
                            {

                                dt.Columns.Add("Point Estimate", typeof(double));
                                dt.Columns.Add("Population", typeof(double));

                                // set up delta columns dynamically for multipollutant
                                if (lstCRTable.First().CRCalculateValues.First().DeltaList != null)
                                {
                                    int dCount = lstCRTable.First().CRCalculateValues.First().DeltaList.Count();

                                    for (int j = 0; j < dCount; j++)
                                    {
                                        string pollName = lstCRTable.First().CRSelectFunction.BenMAPHealthImpactFunction.Variables[j].PollutantName;
                                        dt.Columns.Add("Delta_" + pollName, typeof(double));
                                    }
                                }
                                else
                                {
                                    dt.Columns.Add("Delta", typeof(double));
                                }

                                dt.Columns.Add("Mean", typeof(double));
                                dt.Columns.Add("Baseline", typeof(double));
                                dt.Columns.Add("Percent of Baseline", typeof(double));
                                dt.Columns.Add("Standard Deviation", typeof(double));
                                dt.Columns.Add("Variance", typeof(double));
                            }
                            else
                            {
                                foreach (FieldCheck fieldCheck in cflstResult)
                                {

                                    if (fieldCheck.isChecked && fieldCheck.FieldName != cflstResult.Last().FieldName && fieldCheck.FieldName != "Population Weighted Delta"
                                        && fieldCheck.FieldName != "Population Weighted Base" && fieldCheck.FieldName != "Population Weighted Control")
                                    {
                                        dt.Columns.Add(fieldCheck.FieldName);
                                    }
                                }
                            }

                            if (lstCRTable.First().CRCalculateValues.First().LstPercentile != null)
                            {
                                i = 0;
                                while (i < lstCRTable.First().CRCalculateValues.First().LstPercentile.Count())
                                {
                                    if (cflstResult == null || cflstResult.Last().isChecked)
                                    {
                                        dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()))))), typeof(double));
                                    }
                                    i++;
                                }
                            }
                        }
                        Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicAPV = new Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>();
                        if (!isBatch && cflstResult != null && cflstResult.Where(p => p.FieldName == "Population Weighted Delta").Count() == 1 && this.tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Tag.ToString() != "incidence")
                        {
                            if (cflstResult.Where(p => p.FieldName == "Population Weighted Delta").First().isChecked == true)
                            {
                                foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                                {

                                    CRCalculateValue crv = new CRCalculateValue();
                                    crv.PointEstimate = cr.CRCalculateValues.Sum(p => p.Population * p.Delta) / cr.CRCalculateValues.Sum(p => p.Population);
                                    if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
                                    {
                                        crv.LstPercentile = new List<float>();
                                        foreach (float f in cr.CRCalculateValues.First().LstPercentile)
                                        {
                                            crv.LstPercentile.Add(0);
                                        }
                                    }

                                    CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
                                    crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Delta";
                                    Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();
                                    dicKey.Add(crv, dicAPV.Count);
                                    dicAPV.Add(dicKey.First(), crNew);
                                }
                            }
                            if (cflstResult.Where(p => p.FieldName == "Population Weighted Base").First().isChecked == true)
                            {
                                foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                                {

                                    CRCalculateValue crv = new CRCalculateValue();
                                    CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
                                    BenMAPLine benMAPLineBase = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == crNew.BenMAPHealthImpactFunction.PollutantGroup.Pollutants.First().PollutantID).First().Base;
                                    Dictionary<string, float> dicBase = new Dictionary<string, float>();
                                    string strMetric = "";
                                    if (cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
                                    {
                                        strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName;
                                    }
                                    else
                                        strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName;
                                    foreach (ModelResultAttribute mr in benMAPLineBase.ModelResultAttributes)
                                    {
                                        dicBase.Add(mr.Col + "," + mr.Row, mr.Values[strMetric]);

                                    }
                                    float dPointEstimate = 0;
                                    foreach (CRCalculateValue crvForEstimate in cr.CRCalculateValues)
                                    {
                                        if (dicBase.ContainsKey(crvForEstimate.Col + "," + crvForEstimate.Row))
                                        {
                                            dPointEstimate = dPointEstimate + dicBase[crvForEstimate.Col + "," + crvForEstimate.Row] * crvForEstimate.Population;
                                        }
                                    }
                                    crv.PointEstimate = dPointEstimate / cr.CRCalculateValues.Sum(p => p.Population);
                                    if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
                                    {
                                        crv.LstPercentile = new List<float>();
                                        foreach (float f in cr.CRCalculateValues.First().LstPercentile)
                                        {
                                            crv.LstPercentile.Add(0);
                                        }
                                    }


                                    crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Base";
                                    Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();
                                    dicKey.Add(crv, dicAPV.Count);
                                    dicAPV.Add(dicKey.First(), crNew);
                                }
                            }
                            if (cflstResult.Where(p => p.FieldName == "Population Weighted Control").First().isChecked == true)
                            {
                                foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                                {

                                    CRCalculateValue crv = new CRCalculateValue();
                                    CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
                                    BenMAPLine benMAPLineControl = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == crNew.BenMAPHealthImpactFunction.PollutantGroup.Pollutants.First().PollutantID).First().Control;
                                    Dictionary<string, float> dicControl = new Dictionary<string, float>();
                                    string strMetric = "";
                                    if (cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
                                    {
                                        strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName;
                                    }
                                    else
                                        strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName;
                                    foreach (ModelResultAttribute mr in benMAPLineControl.ModelResultAttributes)
                                    {
                                        dicControl.Add(mr.Col + "," + mr.Row, mr.Values[strMetric]);

                                    }
                                    float dPointEstimate = 0;
                                    foreach (CRCalculateValue crvForEstimate in cr.CRCalculateValues)
                                    {
                                        if (dicControl.ContainsKey(crvForEstimate.Col + "," + crvForEstimate.Row))
                                        {
                                            dPointEstimate = dPointEstimate + dicControl[crvForEstimate.Col + "," + crvForEstimate.Row] * crvForEstimate.Population;
                                        }
                                    }
                                    crv.PointEstimate = dPointEstimate / cr.CRCalculateValues.Sum(p => p.Population);
                                    if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
                                    {
                                        crv.LstPercentile = new List<float>();
                                        foreach (float f in cr.CRCalculateValues.First().LstPercentile)
                                        {
                                            crv.LstPercentile.Add(0);
                                        }
                                    }


                                    crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Control";
                                    Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();
                                    dicKey.Add(crv, dicAPV.Count);
                                    dicAPV.Add(dicKey.First(), crNew);
                                }
                            }
                        }

                        foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                        {
                            foreach (CRCalculateValue crcv in cr.CRCalculateValues)
                            {
                                Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();
                                dicKey.Add(crcv, dicAPV.Count);
                                dicAPV.Add(dicKey.First(), cr.CRSelectFunction);

                            }
                        }


                        if (isSumChecked) // && dicAPV.Keys.First().Key.BetaVariationName.ToLower() != "full year")
                        {
                            // dictionary in the structure the table needs
                            Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicTempResults = new Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>();
                            // dictionary in a format we can use to collect by row, col, season. This dic will reference the same objects as dicTempResults and won't be needed after this process
                            Dictionary<string, Object> dicRowColSeasonLookup = new Dictionary<string, Object>();

                            foreach (KeyValuePair<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> kvp in dicAPV)
                            {
                                string keyRowColSeason = kvp.Key.Key.Row + "-" + kvp.Key.Key.Col + "-" + kvp.Key.Key.BetaName;
                                if (dicRowColSeasonLookup.ContainsKey(keyRowColSeason))
                                {
                                    // Just add to it then
                                    KeyValuePair<CRCalculateValue, int> k = (KeyValuePair<CRCalculateValue, int>)dicRowColSeasonLookup[keyRowColSeason];
                                    k.Key.PointEstimate += kvp.Key.Key.PointEstimate;

                                    int percentileIndex = 0;
                                    foreach (float percentile in kvp.Key.Key.LstPercentile)
                                    {
                                        k.Key.LstPercentile[percentileIndex] += percentile;
                                        percentileIndex++;

                                    }
                                }
                                else
                                {
                                    // Create a deep copy
                                    KeyValuePair<CRCalculateValue, int> k = kvp.Key;
                                    CRCalculateValue deepCopy = ConfigurationCommonClass.getKeyValuePairDeepCopy(k).Key;
                                    KeyValuePair<CRCalculateValue, int> newPair = new KeyValuePair<CRCalculateValue, int>(deepCopy, k.Value);
                                    dicRowColSeasonLookup.Add(keyRowColSeason, newPair);
                                    dicTempResults.Add(newPair, kvp.Value);
                                }
                            }

                            dicAPV.Clear();
                            dicAPV = dicTempResults;
                        }



                        foreach (KeyValuePair<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> k in dicAPV)
                        {

                            DataRow dr = dt.NewRow();
                            if (this.tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Tag.ToString() == "incidence")
                            {
                                if (IncidencelstColumnRow == null)
                                {
                                    dr["Col"] = k.Key.Key.Col;
                                    dr["Row"] = k.Key.Key.Row;
                                }
                                else
                                {
                                    foreach (FieldCheck fieldCheck in IncidencelstColumnRow)
                                    {

                                        if (fieldCheck.isChecked)
                                        {
                                            dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, k.Key.Key, k.Value);
                                        }
                                    }
                                }
                                if (IncidencelstHealth != null)
                                {
                                    foreach (FieldCheck fieldCheck in IncidencelstHealth)
                                    {

                                        if (fieldCheck.isChecked)
                                        {
                                            dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, k.Key.Key, k.Value);
                                        }
                                    }
                                }
                                if (IncidencelstResult == null)
                                {

                                    dr["Point Estimate"] = k.Key.Key.PointEstimate;
                                    dr["Population"] = k.Key.Key.Population;

                                    // set delta values dynamically
                                    if (lstCRTable.First().CRCalculateValues.First().DeltaList != null)
                                    {
                                        string pollName, tempKey;
                                        int dCount = lstCRTable.First().CRCalculateValues.First().DeltaList.Count();
                                        for (int j = 0; j < dCount; j++)
                                        {
                                            pollName = lstCRTable.First().CRSelectFunction.BenMAPHealthImpactFunction.Variables[j].PollutantName;
                                            tempKey = "Delta_" + pollName;
                                            dr[tempKey] = k.Key.Key.DeltaList[j];
                                        }
                                    }
                                    else
                                    {
                                        dr["Delta"] = k.Key.Key.Delta;
                                    }

                                    dr["Mean"] = k.Key.Key.Mean;
                                    dr["Baseline"] = k.Key.Key.Baseline;
                                    dr["Percent Of Baseline"] = k.Key.Key.PercentOfBaseline;
                                    dr["Standard Deviation"] = k.Key.Key.StandardDeviation;
                                    dr["Variance"] = k.Key.Key.Variance;
                                }
                                else
                                {
                                    foreach (FieldCheck fieldCheck in IncidencelstResult)
                                    {

                                        if (fieldCheck.isChecked && fieldCheck.FieldName != IncidencelstResult.Last().FieldName && fieldCheck.FieldName != "Population Weighted Delta"
                                    && fieldCheck.FieldName != "Population Weighted Base" && fieldCheck.FieldName != "Population Weighted Control")
                                        {
                                            dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, k.Key.Key, k.Value);
                                        }
                                    }
                                }


                                i = 0;
                                if ((IncidencelstResult == null || IncidencelstResult.Last().isChecked) && k.Key.Key.LstPercentile != null)
                                {
                                    while (i < k.Key.Key.LstPercentile.Count())
                                    {

                                        dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(k.Key.Key.LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(k.Key.Key.LstPercentile.Count())))))] = k.Key.Key.LstPercentile[i];
                                        i++;
                                    }
                                }
                            }
                            else
                            {
                                if (cflstColumnRow == null)
                                {
                                    dr["Col"] = k.Key.Key.Col;
                                    dr["Row"] = k.Key.Key.Row;
                                }
                                else
                                {
                                    foreach (FieldCheck fieldCheck in cflstColumnRow)
                                    {

                                        if (fieldCheck.isChecked)
                                        {
                                            dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, k.Key.Key, k.Value);
                                        }
                                    }
                                }
                                if (cflstHealth != null)
                                {
                                    foreach (FieldCheck fieldCheck in cflstHealth)
                                    {

                                        if (fieldCheck.isChecked)
                                        {
                                            dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, k.Key.Key, k.Value);
                                        }
                                    }
                                }
                                if (k.Key.Key.BetaVariationName != null)
                                {
                                    dr["Seasonal"] = k.Key.Key.BetaName;
                                }
                                if (cflstResult == null)
                                {

                                    dr["Point Estimate"] = k.Key.Key.PointEstimate;
                                    dr["Population"] = k.Key.Key.Population;

                                    // set delta values dynamically
                                    if (lstCRTable.First().CRCalculateValues.First().DeltaList != null)
                                    {
                                        string pollName, tempKey;
                                        int dCount = lstCRTable.First().CRCalculateValues.First().DeltaList.Count();
                                        for (int j = 0; j < dCount; j++)
                                        {
                                            pollName = lstCRTable.First().CRSelectFunction.BenMAPHealthImpactFunction.Variables[j].PollutantName;
                                            tempKey = "Delta_" + pollName;
                                            dr[tempKey] = k.Key.Key.DeltaList[j];
                                        }
                                    }
                                    else
                                    {
                                        dr["Delta"] = k.Key.Key.Delta;
                                    }

                                    dr["Mean"] = k.Key.Key.Mean;
                                    dr["Baseline"] = k.Key.Key.Baseline;
                                    dr["Percent Of Baseline"] = k.Key.Key.PercentOfBaseline;
                                    dr["Standard Deviation"] = k.Key.Key.StandardDeviation;
                                    dr["Variance"] = k.Key.Key.Variance;
                                }
                                else
                                {
                                    foreach (FieldCheck fieldCheck in cflstResult)
                                    {

                                        if (fieldCheck.isChecked && fieldCheck.FieldName != cflstResult.Last().FieldName && fieldCheck.FieldName != "Population Weighted Delta"
                                    && fieldCheck.FieldName != "Population Weighted Base" && fieldCheck.FieldName != "Population Weighted Control")
                                        {
                                            dr[fieldCheck.FieldName] = getFieldNameFromlstHealthObject(fieldCheck.FieldName, k.Key.Key, k.Value);
                                        }
                                    }
                                }


                                i = 0;
                                if ((cflstResult == null || cflstResult.Last().isChecked) && k.Key.Key.LstPercentile != null)
                                {
                                    while (i < k.Key.Key.LstPercentile.Count())
                                    {

                                        dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(k.Key.Key.LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(k.Key.Key.LstPercentile.Count())))))] = k.Key.Key.LstPercentile[i];
                                        i++;
                                    }
                                }

                            }

                            dt.Rows.Add(dr);

                        }
                        CommonClass.SaveCSV(dt, _outputFileName);

                    }
                    else if (_tableObject is BenMAPLine)
                    {
                        BenMAPLine crTable = (BenMAPLine)_tableObject;
                        DataSourceCommonClass.SaveModelDataLineToNewFormatCSV(crTable, _outputFileName);
                    }













                    else if (_tableObject is List<AllSelectValuationMethodAndValue> || _tableObject is AllSelectValuationMethodAndValue)
                    {
                        List<AllSelectValuationMethodAndValue> lstallSelectValuationMethodAndValue = new List<AllSelectValuationMethodAndValue>();
                        if (_tableObject is List<AllSelectValuationMethodAndValue>)
                        {
                            lstallSelectValuationMethodAndValue = (List<AllSelectValuationMethodAndValue>)_tableObject;
                        }
                        else
                        {
                            lstallSelectValuationMethodAndValue.Add((AllSelectValuationMethodAndValue)_tableObject);
                        }
                        if (apvlstColumnRow == null)
                        {
                            dt.Columns.Add("Col", typeof(int));
                            dt.Columns.Add("Row", typeof(int));
                        }
                        else
                        {
                            foreach (FieldCheck fieldCheck in apvlstColumnRow)
                            {

                                if (fieldCheck.isChecked)
                                {
                                    dt.Columns.Add(fieldCheck.FieldName);
                                }
                            }

                        }
                        if (apvlstHealth != null)
                        {
                            foreach (FieldCheck fieldCheck in apvlstHealth)
                            {

                                if (fieldCheck.isChecked)
                                {
                                    dt.Columns.Add(fieldCheck.FieldName);
                                }
                            }

                        }
                        if (apvlstResult == null)
                        {
                            dt.Columns.Add("Point Estimate", typeof(double));

                            dt.Columns.Add("Mean", typeof(double));

                            dt.Columns.Add("Standard Deviation", typeof(double));
                            dt.Columns.Add("Variance", typeof(double));
                        }
                        else
                        {
                            foreach (FieldCheck fieldCheck in apvlstResult)
                            {

                                if (fieldCheck.FieldName != apvlstResult.Last().FieldName && fieldCheck.isChecked)
                                {
                                    dt.Columns.Add(fieldCheck.FieldName);
                                }
                            }

                        }
                        if ((apvlstResult == null || apvlstResult.Last().isChecked) && lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile != null)
                        {
                            i = 0;
                            while (i < lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count())
                            {

                                dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count()))))), typeof(double));
                                i++;
                            }
                        }


                        foreach (AllSelectValuationMethodAndValue allSelectValuationMethodAndValue in lstallSelectValuationMethodAndValue)
                        {
                            foreach (APVValueAttribute apvx in allSelectValuationMethodAndValue.lstAPVValueAttributes)
                            {
                                DataRow dr = dt.NewRow();
                                if (apvlstColumnRow == null)
                                {
                                    dr["Col"] = apvx.Col;
                                    dr["Row"] = apvx.Row;
                                }
                                else
                                {
                                    foreach (FieldCheck fieldCheck in apvlstColumnRow)
                                    {

                                        if (fieldCheck.isChecked)
                                        {
                                            dr[fieldCheck.FieldName] = getFieldNameFromlstAPVObject(fieldCheck.FieldName, allSelectValuationMethodAndValue.AllSelectValuationMethod, apvx);
                                        }
                                    }

                                }
                                if (apvlstHealth != null)
                                {
                                    foreach (FieldCheck fieldCheck in apvlstHealth)
                                    {

                                        if (fieldCheck.isChecked)
                                        {
                                            dr[fieldCheck.FieldName] = getFieldNameFromlstAPVObject(fieldCheck.FieldName, allSelectValuationMethodAndValue.AllSelectValuationMethod, apvx);
                                        }
                                    }

                                }
                                if (apvlstResult == null)
                                {

                                    dr["Point Estimate"] = apvx.PointEstimate;
                                    dr["Mean"] = apvx.Mean;
                                    dr["Standard Deviation"] = apvx.StandardDeviation;
                                    dr["Variance"] = apvx.Variance;

                                }
                                else
                                {
                                    foreach (FieldCheck fieldCheck in apvlstResult)
                                    {

                                        if (fieldCheck.FieldName != apvlstResult.Last().FieldName && fieldCheck.isChecked)
                                        {
                                            dr[fieldCheck.FieldName] = getFieldNameFromlstAPVObject(fieldCheck.FieldName, allSelectValuationMethodAndValue.AllSelectValuationMethod, apvx);
                                        }
                                    }

                                }
                                if ((apvlstResult == null || apvlstResult.Last().isChecked) && lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile != null)
                                {
                                    i = 0;
                                    while (i < lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count())
                                    {
                                        dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count())))))] = apvx.LstPercentile[i];
                                        i++;
                                    }
                                }


                                dt.Rows.Add(dr);
                            }
                        }
                        CommonClass.SaveCSV(dt, _outputFileName);

                    }

                    else if (_tableObject is List<AllSelectQALYMethodAndValue> || _tableObject is AllSelectQALYMethodAndValue)
                    {
                        List<AllSelectQALYMethodAndValue> lstAllSelectQALYMethodAndValue = new List<AllSelectQALYMethodAndValue>();
                        if (_tableObject is List<AllSelectQALYMethodAndValue>)
                        {
                            lstAllSelectQALYMethodAndValue = (List<AllSelectQALYMethodAndValue>)_tableObject;
                        }
                        else
                        {
                            lstAllSelectQALYMethodAndValue.Add((AllSelectQALYMethodAndValue)_tableObject);
                        }
                        if (qalylstColumnRow == null)
                        {
                            dt.Columns.Add("Col", typeof(int));
                            dt.Columns.Add("Row", typeof(int));
                        }
                        else
                        {
                            foreach (FieldCheck fieldCheck in qalylstColumnRow)
                            {

                                if (fieldCheck.isChecked)
                                {
                                    dt.Columns.Add(fieldCheck.FieldName);
                                }
                            }

                        }
                        if (qalylstHealth != null)
                        {
                            foreach (FieldCheck fieldCheck in qalylstHealth)
                            {

                                if (fieldCheck.isChecked)
                                {
                                    dt.Columns.Add(fieldCheck.FieldName);
                                }
                            }

                        }
                        if (qalylstResult == null)
                        {
                            dt.Columns.Add("Point Estimate", typeof(double));

                            dt.Columns.Add("Mean", typeof(double));

                            dt.Columns.Add("Standard Deviation", typeof(double));
                            dt.Columns.Add("Variance", typeof(double));
                        }
                        else
                        {
                            foreach (FieldCheck fieldCheck in qalylstResult)
                            {

                                if (fieldCheck.FieldName != qalylstResult.Last().FieldName && fieldCheck.isChecked)
                                {
                                    dt.Columns.Add(fieldCheck.FieldName);
                                }
                            }

                        }
                        if ((qalylstResult == null || qalylstResult.Last().isChecked) && lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile != null)
                        {
                            i = 0;
                            while (i < lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count())
                            {

                                dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()))))), typeof(double));
                                i++;
                            }
                        }


                        foreach (AllSelectQALYMethodAndValue AllSelectQALYMethodAndValue in lstAllSelectQALYMethodAndValue)
                        {
                            foreach (QALYValueAttribute apvx in AllSelectQALYMethodAndValue.lstQALYValueAttributes)
                            {
                                DataRow dr = dt.NewRow();
                                if (qalylstColumnRow == null)
                                {
                                    dr["Col"] = apvx.Col;
                                    dr["Row"] = apvx.Row;
                                }
                                else
                                {
                                    foreach (FieldCheck fieldCheck in qalylstColumnRow)
                                    {

                                        if (fieldCheck.isChecked)
                                        {
                                            dr[fieldCheck.FieldName] = getFieldNameFromlstQALYObject(fieldCheck.FieldName, AllSelectQALYMethodAndValue.AllSelectQALYMethod, apvx);
                                        }
                                    }

                                }
                                if (qalylstHealth != null)
                                {
                                    foreach (FieldCheck fieldCheck in qalylstHealth)
                                    {

                                        if (fieldCheck.isChecked)
                                        {
                                            dr[fieldCheck.FieldName] = getFieldNameFromlstQALYObject(fieldCheck.FieldName, AllSelectQALYMethodAndValue.AllSelectQALYMethod, apvx);
                                        }
                                    }

                                }
                                if (qalylstResult == null)
                                {

                                    dr["Point Estimate"] = apvx.PointEstimate;
                                    dr["Mean"] = apvx.Mean;
                                    dr["Standard Deviation"] = apvx.StandardDeviation;
                                    dr["Variance"] = apvx.Variance;

                                }
                                else
                                {
                                    foreach (FieldCheck fieldCheck in qalylstResult)
                                    {

                                        if (fieldCheck.FieldName != qalylstResult.Last().FieldName && fieldCheck.isChecked)
                                        {
                                            dr[fieldCheck.FieldName] = getFieldNameFromlstQALYObject(fieldCheck.FieldName, AllSelectQALYMethodAndValue.AllSelectQALYMethod, apvx);
                                        }
                                    }

                                }
                                if ((qalylstResult == null || qalylstResult.Last().isChecked) && lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile != null)
                                {
                                    i = 0;
                                    while (i < lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count())
                                    {
                                        dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count())))))] = apvx.LstPercentile[i];
                                        i++;
                                    }
                                }


                                dt.Rows.Add(dr);
                            }
                        }
                        CommonClass.SaveCSV(dt, _outputFileName);

                    }
                    else if (_tableObject is AllSelectQALYMethodAndValue)
                    {
                        AllSelectQALYMethodAndValue allSelectQALYMethodAndValue = (AllSelectQALYMethodAndValue)_tableObject;
                        dt.Columns.Add("Col", typeof(int));
                        dt.Columns.Add("Row", typeof(int));
                        dt.Columns.Add("Point Estimate", typeof(double));

                        dt.Columns.Add("Mean", typeof(double));

                        dt.Columns.Add("Standard Deviation", typeof(double));
                        dt.Columns.Add("Variance", typeof(double));

                        i = 0;
                        while (i < allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count())
                        {
                            dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count()))))), typeof(double));
                            i++;
                        }

                        dt.Columns.Add("Name", typeof(string));
                        dt.Columns.Add("Pooling Method", typeof(string));
                        dt.Columns.Add("Qualifier", typeof(string));
                        dt.Columns.Add("Start Age", typeof(string));
                        dt.Columns.Add("End Age", typeof(string));


                        foreach (QALYValueAttribute apvx in allSelectQALYMethodAndValue.lstQALYValueAttributes)
                        {
                            DataRow dr = dt.NewRow();
                            dr["Col"] = apvx.Col;
                            dr["Row"] = apvx.Row;
                            dr["Point Estimate"] = apvx.PointEstimate;

                            dr["Mean"] = apvx.Mean;

                            dr["Standard Deviation"] = apvx.StandardDeviation;
                            dr["Variance"] = apvx.Variance;

                            dr["Name"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.Name;
                            dr["Pooling Method"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.PoolingMethod;
                            dr["Qualifier"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.Qualifier;
                            dr["Start Age"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.StartAge;
                            dr["End Age"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.EndAge;

                            i = 0;
                            while (i < allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count())
                            {

                                dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count())))))] = apvx.LstPercentile[i];
                                i++;
                            }
                            dt.Rows.Add(dr);

                        }
                        CommonClass.SaveCSV(dt, _outputFileName);

                    }

                    else if (_tableObject is List<AllSelectQALYMethodAndValue>)
                    {
                        List<AllSelectQALYMethodAndValue> lstAllSelectQALYMethodAndValue = (List<AllSelectQALYMethodAndValue>)_tableObject;
                        dt.Columns.Add("Col", typeof(int));
                        dt.Columns.Add("Row", typeof(int));
                        dt.Columns.Add("Point Estimate", typeof(double));

                        dt.Columns.Add("Mean", typeof(double));

                        dt.Columns.Add("Standard Deviation", typeof(double));
                        dt.Columns.Add("Variance", typeof(double));

                        i = 0;
                        while (i < lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count())
                        {
                            dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()))))), typeof(double));
                            i++;
                        }

                        dt.Columns.Add("Name", typeof(string));
                        dt.Columns.Add("Pooling Method", typeof(string));
                        dt.Columns.Add("Qualifier", typeof(string));
                        dt.Columns.Add("Start Age", typeof(string));
                        dt.Columns.Add("End Age", typeof(string));

                        foreach (AllSelectQALYMethodAndValue allSelectQALYMethodAndValue in lstAllSelectQALYMethodAndValue)
                        {
                            foreach (QALYValueAttribute apvx in allSelectQALYMethodAndValue.lstQALYValueAttributes)
                            {
                                DataRow dr = dt.NewRow();
                                dr["Col"] = apvx.Col;
                                dr["Row"] = apvx.Row;
                                dr["Point Estimate"] = apvx.PointEstimate;

                                dr["Mean"] = apvx.Mean;

                                dr["Standard Deviation"] = apvx.StandardDeviation;
                                dr["Variance"] = apvx.Variance;

                                dr["Name"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.Name;
                                dr["Pooling Method"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.PoolingMethod;
                                dr["Qualifier"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.Qualifier;
                                dr["Start Age"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.StartAge;
                                dr["End Age"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.EndAge;

                                i = 0;
                                while (i < allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count())
                                {

                                    dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count())))))] = apvx.LstPercentile[i];
                                    i++;
                                }
                                dt.Rows.Add(dr);

                            }
                        }
                        CommonClass.SaveCSV(dt, _outputFileName);

                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        private void btnSaveChart_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void pnlRSM_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }


        private void ListviewSelectionChange(ListView list)
        {
            foreach (ListViewItem item in list.Items)
            {
                if (item.Selected)
                {
                    item.BackColor = Color.LightBlue;
                    item.ForeColor = Color.Red;
                    list.Refresh();
                }
                else
                {
                    item.BackColor = Color.Transparent;
                }
            }

        }

        private void lvwRawData_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {

        }

        private void lvwRawData_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void lvwRawForm_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void lvwResultType_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {

            }
        }

        private void lvwResultType_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void lvwProcessType_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void lvwProcessForm_MouseClick(object sender, MouseEventArgs e)
        {
        }


        private void tabCtlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnRawAudit_Click(object sender, EventArgs e)
        {
            //IMapFeatureLayer ilayer = mainMap.Layers[0] as IMapFeatureLayer;
            //string layerName = ilayer.LegendText;
            if (mainMap.GetAllLayers().Count == 0)
            {
                MessageBox.Show("Please set up necessary data.", "Error", MessageBoxButtons.OK);
                return;
            }
        }




        TipFormGIF waitMess = new TipFormGIF(); 
        bool sFlog = true;

        private void ShowWaitMess()
        {
            try
            {
                if (!waitMess.IsDisposed)
                {
                    waitMess.ShowDialog();
                }
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }

        public void WaitShow(string msg)
        {
            try
            {
                if (sFlog == true)
                {
                    sFlog = false;
                    waitMess.Msg = msg;
                    System.Threading.Thread upgradeThread = null;
                    upgradeThread = new System.Threading.Thread(new System.Threading.ThreadStart(ShowWaitMess));
                    upgradeThread.Start();
                }
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }

        private delegate void CloseFormDelegate();

        public void WaitClose()
        {
            try
            {
                if (waitMess.InvokeRequired)
                    waitMess.Invoke(new CloseFormDelegate(DoCloseJob));
                else
                    DoCloseJob();
            }
            catch (Exception ex)
            { }
        }

        private void DoCloseJob()
        {
            try
            {
                if (!waitMess.IsDisposed)
                {
                    if (waitMess.Created)
                    {
                        sFlog = true;
                        waitMess.Close();
                    }
                }
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }


        private void lvwRawData_MouseLeave(object sender, EventArgs e)
        {
        }

        private void lvwRawData_Click(object sender, EventArgs e)
        {
        }

        private void lvwRawData_MouseUp(object sender, MouseEventArgs e)
        {
        }
        private void BenMAP_Load(object sender, EventArgs e)
        {
            olvCRFunctionResult.EmptyListMsg = "After results are generated here, double-click the selected study to display map/data/chart below." + Environment.NewLine + " Ctrl- or shift-click to select multiple studies and then click \"Show result\" to display data for multiple studies.";
            mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
            if (!Directory.Exists(CommonClass.ResultFilePath + @"\Result\CFGR"))
                System.IO.Directory.CreateDirectory(CommonClass.ResultFilePath + @"\Result\CFGR");
            if (!Directory.Exists(CommonClass.ResultFilePath + @"\Result\Project"))
                System.IO.Directory.CreateDirectory(CommonClass.ResultFilePath + @"\Result\Project");
            if (!Directory.Exists(CommonClass.ResultFilePath + @"\Result\APV"))
                System.IO.Directory.CreateDirectory(CommonClass.ResultFilePath + @"\Result\APV");
            if (!Directory.Exists(CommonClass.ResultFilePath + @"\Result\CFG"))
                System.IO.Directory.CreateDirectory(CommonClass.ResultFilePath + @"\Result\CFG");
            if (!Directory.Exists(CommonClass.ResultFilePath + @"\Result\APVR"))
                System.IO.Directory.CreateDirectory(CommonClass.ResultFilePath + @"\Result\APVR");
            if (!Directory.Exists(CommonClass.ResultFilePath + @"\Result\AQG"))
                System.IO.Directory.CreateDirectory(CommonClass.ResultFilePath + @"\Result\AQG");
            if (!Directory.Exists(CommonClass.DataFilePath + @"\Tmp"))
                System.IO.Directory.CreateDirectory(CommonClass.DataFilePath + @"\Tmp");





            bindingNavigatorCountItem.Enabled = true;
            bindingNavigatorMoveFirstItem.Enabled = true;
            bindingNavigatorMoveNextItem.Enabled = true;
            bindingNavigatorMoveLastItem.Enabled = true;
            bindingNavigatorMovePreviousItem.Enabled = true;
            bindingNavigatorPositionItem.Enabled = true;
            //colorBlend.CustomizeValueRange -= ResetGisMap();-MCB - may still be needed 
            //colorBlend.CustomizeValueRange += ResetGisMap();
            InitAggregationAndRegionList();
            Dictionary<string, string> dicSeasonStaticsAll = DataSourceCommonClass.DicSeasonStaticsAll;
            InitColumnsShowSet();

            this.treeListView.CanExpandGetter = delegate(object x)
            {
                return (x is TreeNode && (x as TreeNode).Nodes.Count > 0);
            };
            this.treeListView.ChildrenGetter = delegate(object x)
            {
                TreeNode dir = (TreeNode)x;
                try
                {
                    return dir.Nodes;
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show(this, ex.Message, "ObjectListViewDemo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return null;
                }
            };

            isLoad = true;
        }

        public void CopyDir(string srcPath, string aimPath)
        {
            try
            {
                if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
                    aimPath += Path.DirectorySeparatorChar;

                if (!Directory.Exists(aimPath))
                    Directory.CreateDirectory(aimPath);


                string[] fileList = Directory.GetFileSystemEntries(srcPath);

                foreach (string file in fileList)
                {
                    if (Directory.Exists(file))
                    {
                        CopyDir(file, aimPath + Path.GetFileName(file));
                    }
                    else
                    {
                        File.Copy(file, aimPath + Path.GetFileName(file), true);
                    }
                }
            }
            catch
            {
            }
        }

        public void InitAggregationAndRegionList()
        {
            try
            {
                System.Data.DataSet dsCRAggregationGridType = BindGridtype();
                cbCRAggregation.DataSource = dsCRAggregationGridType.Tables[0];
                cbCRAggregation.DisplayMember = "GridDefinitionName";
                cbCRAggregation.SelectedIndex = 0;
                FireBirdHelperBase fb = new ESILFireBirdHelper();
                string commandText = string.Format("select distinct GridDefinitionID,GridDefinitionName from GridDefinitions where columns<=56 and setupid={0}  order by GridDefinitionName desc", CommonClass.MainSetup.SetupID);
                System.Data.DataSet dsRegion = BindGridtypeDomain(); cboRegion.DisplayMember = "GridDefinitionName";
                cboRegion.DataSource = dsRegion.Tables[0];

                for (int i = 0; i < dsRegion.Tables[0].Rows.Count; i++)
                {
                    if (CommonClass.rBenMAPGrid != null && CommonClass.rBenMAPGrid.GridDefinitionID == Convert.ToInt32(dsRegion.Tables[0].Rows[i]["GridDefinitionID"]))
                    {
                        cboRegion.SelectedIndex = i;
                        break;
                    }

                }
            }
            catch
            {

            }
        }
        public void loadHomePageFunction()
        {
            switch (_homePageName)
            {
                case "picCreateProject":
                    break;
                case "picLoadProject":
                    OpenProject();
                    break;
                case "picLoadCFG":
                    trvSetting.SelectedNode = trvSetting.Nodes[1];
                    trvSetting_NodeMouseDoubleClick(trvSetting.Nodes[1], null);
                    break;
                case "picLoadCFGR":
                    trvSetting.SelectedNode = trvSetting.Nodes[1];
                    trvSetting_NodeMouseDoubleClick(trvSetting.Nodes[1], null);
                    break;
                case "picLoadAPV":
                    trvSetting.SelectedNode = trvSetting.Nodes[2];
                    trvSetting_NodeMouseDoubleClick(trvSetting.Nodes[2], null);
                    break;
                case "picLoadAPVR":
                    trvSetting.SelectedNode = trvSetting.Nodes[2];
                    trvSetting_NodeMouseDoubleClick(trvSetting.Nodes[2], null);
                    break;
            }
        }
        public void loadInputParamProject()
        {
            try
            {
                if (CommonClass.InputParams != null && CommonClass.InputParams.Length > 0 && CommonClass.InputParams[0].ToLower().IndexOf("ctlx") > 0)
                {


                }
                else if (CommonClass.InputParams != null && CommonClass.InputParams.Length > 0 && CommonClass.InputParams[0].ToLower().IndexOf("projx") > 0)
                {
                    splitContainer1.Visible = true;
                    CommonClass.ClearAllObject();
                    _MapAlreadyDisplayed = false;
                    ClearAll();
                    ResetParamsTree("");

                    ClearMapTableChart();
                    initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);

                    initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
                    initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
                    CommonClass.IncidencePoolingAndAggregationAdvance = null;
                    if (CommonClass.IncidencePoolingAndAggregationAdvance != null)
                    {
                        changeNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);

                    }
                    olvCRFunctionResult.SetObjects(null);
                    olvIncidence.SetObjects(null);
                    tlvAPVResult.SetObjects(null);

                    cbPoolingWindowIncidence.Items.Clear();
                    cbPoolingWindowAPV.Items.Clear();
                    ClearMapTableChart();
                    SetTabControl(tabCtlReport);
                    CommonClass.LstPollutant = null;
                    CommonClass.RBenMAPGrid = null;
                    CommonClass.GBenMAPGrid = null;
                    CommonClass.LstBaseControlGroup = null;
                    CommonClass.LstCreateShapeFileParams = null;
                    CommonClass.CRThreshold = 0;
                    CommonClass.CRLatinHypercubePoints = 20;
                    CommonClass.CRRunInPointMode = false;
                    CommonClass.BenMAPPopulation = null;
                    CommonClass.BaseControlCRSelectFunction = null; CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
                    CommonClass.lstIncidencePoolingAndAggregation = null;

                    CommonClass.IncidencePoolingResult = null;
                    CommonClass.ValuationMethodPoolingAndAggregation = null;
                    CommonClass.BaseControlCRSelectFunction = null;
                    CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
                    CommonClass.ValuationMethodPoolingAndAggregation = null;
                    GC.Collect();
                    CommonClass.LoadBenMAPProject(CommonClass.InputParams[0]);
                    CommonClass.InputParams = null;
                    if (CommonClass.ValuationMethodPoolingAndAggregation != null)
                    {
                        CommonClass.lstIncidencePoolingAndAggregation = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Select(a => a.IncidencePoolingAndAggregation).ToList();
                        CommonClass.IncidencePoolingAndAggregationAdvance = CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance;
                        cbPoolingWindowAPV.Items.Clear();
                        CommonClass.BaseControlCRSelectFunctionCalculateValue = CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue;
                        CommonClass.BaseControlCRSelectFunction = null;
                        CommonClass.BaseControlCRSelectFunction = new BaseControlCRSelectFunction();
                        CommonClass.BaseControlCRSelectFunction.BaseControlGroup = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup;
                        CommonClass.BaseControlCRSelectFunction.BenMAPPopulation = CommonClass.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
                        CommonClass.BaseControlCRSelectFunction.CRLatinHypercubePoints = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints;
                        CommonClass.BaseControlCRSelectFunction.CRRunInPointMode = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRRunInPointMode;
                        CommonClass.BaseControlCRSelectFunction.CRSeeds = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRSeeds;
                        CommonClass.BaseControlCRSelectFunction.CRThreshold = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRThreshold;
                        CommonClass.BaseControlCRSelectFunction.RBenMapGrid = CommonClass.BaseControlCRSelectFunctionCalculateValue.RBenMapGrid;
                        CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction = new List<CRSelectFunction>();
                        for (int i = 0; i < CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; i++)
                        {
                            CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Add(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[i].CRSelectFunction);
                        }
                        try
                        {
                            if (CommonClass.BaseControlCRSelectFunction != null)
                            {
                                showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, this.trvSetting.Nodes["aggregationpoolingvaluation"]);

                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            Logger.LogError(ex);
                        }

                        errorNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);

                        errorNodeImage(trvSetting.Nodes[2].Nodes[1]);
                        errorNodeImage(trvSetting.Nodes[2].Nodes[2]);


                    }
                    else if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null)
                    {


                        showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, this.trvSetting.Nodes["aggregationpoolingvaluation"]);
                        errorNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
                    }
                    else if (CommonClass.BaseControlCRSelectFunction != null)
                    {
                        errorNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
                        showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, this.trvSetting.Nodes["aggregationpoolingvaluation"]);

                    }
                    else if (CommonClass.LstBaseControlGroup != null && CommonClass.LstBaseControlGroup.Count > 0)
                    {
                        int nodesCount = 0;
                        foreach (TreeNode trchild in trvSetting.Nodes)
                        {
                            if (trchild.Name == "airqualitygridgroup")
                            {
                                nodesCount = trchild.Nodes.Count;


                                for (int i = nodesCount - 1; i > -1; i--)
                                {
                                    TreeNode node = trchild.Nodes[i];
                                    if (trchild.Nodes[i].Name == "datasource") { trchild.Nodes.RemoveAt(i); }
                                }
                                for (int i = CommonClass.LstBaseControlGroup.Count - 1; i > -1; i--)
                                {
                                    AddDataSourceNode(CommonClass.LstBaseControlGroup[i], trchild);
                                    if (CommonClass.LstBaseControlGroup[i] != null && CommonClass.LstBaseControlGroup[i].Base != null)
                                    {
                                        changeNodeImage(trchild.Nodes[trchild.Nodes.Count - 1].Nodes[0]);
                                    }
                                    if (CommonClass.LstBaseControlGroup[i] != null && CommonClass.LstBaseControlGroup[i].Control != null)
                                    {
                                        changeNodeImage(trchild.Nodes[trchild.Nodes.Count - 1].Nodes[1]);
                                    }
                                    if (CommonClass.LstBaseControlGroup[i] != null && CommonClass.LstBaseControlGroup[i].Base != null && CommonClass.LstBaseControlGroup[i].Control != null)
                                    {
                                        changeNodeImage(trchild.Nodes[trchild.Nodes.Count - 1]);
                                    }
                                }
                                trchild.ExpandAll();

                                foreach (TreeNode trair in trchild.Nodes)
                                {
                                    if (trair.Name != "datasource")
                                        changeNodeImage(trair);
                                    TreeNode tr = trair;
                                    if (trair.Name == "gridtype")
                                    {
                                        AddChildNodes(ref tr, "", "", new BenMAPLine());
                                        trair.ExpandAll();
                                    }
                                }
                            }
                            if (trchild.Name == "configuration")
                            {
                                foreach (TreeNode tr in trchild.Nodes)
                                {
                                    initNodeImage(tr);
                                }
                                trchild.ExpandAll();
                            }
                            if (trchild.Name == "aggregationpoolingvaluation")
                            {
                                foreach (TreeNode tr in trchild.Nodes)
                                {
                                    initNodeImage(tr);
                                }
                                trchild.ExpandAll();
                            }
                        }
                    }
                    else
                    {
                        if (CommonClass.GBenMAPGrid != null)
                        {
                            TreeNode currentNode = trvSetting.Nodes[0].Nodes["gridtype"];
                            AddChildNodes(ref currentNode, "", "", null);
                            changeNodeImage(currentNode);
                        }
                        if (CommonClass.LstPollutant != null)
                        {
                            int nodesCount = trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes.Count;
                            if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0)
                            {
                                for (int i = nodesCount - 2; i > -1; i--)
                                {
                                    TreeNode node = trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i];
                                    if (trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Name == "datasource") { trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes.RemoveAt(i); }
                                }
                                for (int i = nodesCount - 1; i > -1; i--)
                                {
                                    TreeNode node = trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i];
                                    if (trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Name == "datasource")
                                    {
                                        trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Text = "Source of Air Quality Data";
                                        trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Nodes[0].Tag = null;
                                        trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Nodes[0].Nodes.Clear();
                                        trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Nodes[1].Tag = null;
                                        trvSetting.Nodes[0].Nodes["pollutant"].Parent.Nodes[i].Nodes[1].Nodes.Clear();
                                    }
                                }


                                olvCRFunctionResult.SetObjects(null);
                                olvIncidence.SetObjects(null);
                                tlvAPVResult.SetObjects(null);
                                cbPoolingWindowIncidence.Items.Clear();
                                cbPoolingWindowAPV.Items.Clear();
                                ClearMapTableChart();
                                initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);

                                initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
                                initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
                                return;
                            }

                            trvSetting.Nodes["pollutant"].Parent.ExpandAll();
                        }
                    }
                    if (CommonClass.BenMAPPopulation != null)
                    {
                        changeNodeImage(trvSetting.Nodes[1].Nodes[0]);
                    }
                    if (CommonClass.IncidencePoolingAndAggregationAdvance != null)
                    {
                        changeNodeImage(trvSetting.Nodes[2].Nodes[0]);
                    }


                }
                else if (CommonClass.InputParams != null && CommonClass.InputParams.Length > 0 && CommonClass.InputParams[0].ToLower().IndexOf("smat") > 0)
                {
                    splitContainer1.Visible = true;
                    CommonClass.ClearAllObject();
                    ClearAll();
                    ResetParamsTree("");

                    ClearMapTableChart();
                    initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);

                    initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
                    initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
                    CommonClass.IncidencePoolingAndAggregationAdvance = null;

                    olvCRFunctionResult.SetObjects(null);
                    olvIncidence.SetObjects(null);
                    tlvAPVResult.SetObjects(null);

                    cbPoolingWindowIncidence.Items.Clear();
                    cbPoolingWindowAPV.Items.Clear();
                    ClearMapTableChart();
                    SetTabControl(tabCtlReport);
                    CommonClass.LstPollutant = null;
                    CommonClass.RBenMAPGrid = null;

                    CommonClass.GBenMAPGrid = null;
                    CommonClass.LstBaseControlGroup = null;
                    CommonClass.LstCreateShapeFileParams = null;
                    CommonClass.CRThreshold = 0;
                    CommonClass.CRLatinHypercubePoints = 20;
                    CommonClass.CRRunInPointMode = false;

                    CommonClass.BenMAPPopulation = null;

                    CommonClass.BaseControlCRSelectFunction = null;
                    CommonClass.BaseControlCRSelectFunctionCalculateValue = null;

                    CommonClass.lstIncidencePoolingAndAggregation = null;

                    CommonClass.IncidencePoolingResult = null;
                    CommonClass.ValuationMethodPoolingAndAggregation = null;
                    CommonClass.BaseControlCRSelectFunction = null;
                    CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
                    CommonClass.ValuationMethodPoolingAndAggregation = null;
                    GC.Collect();

                    LoadAirQualityData frm = new LoadAirQualityData(CommonClass.InputParams[0]);
                    DialogResult dr = frm.ShowDialog();
                    if (dr != DialogResult.OK)
                        Environment.Exit(0);
                    BaseControlGroup bcg = frm.bcgOpenAQG;
                    frm.Dispose();

                    for (int i = 0; i < trvSetting.Nodes.Count; i++)
                    {
                        TreeNode trchild = trvSetting.Nodes[i];
                        if (trchild.Name == "airqualitygridgroup")
                        {
                            int nodesCount = trchild.Nodes.Count;
                            for (int j = nodesCount - 1; j > -1; j--)
                            {
                                TreeNode node = trchild.Nodes[j];
                                if (trchild.Nodes[j].Name == "datasource")
                                {
                                    BrushBaseControl(ref trchild, bcg, trchild.Nodes[j].Index);
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }
        private bool isLoad = false;
        private System.Data.DataSet BindGridtype()
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select -1 as  GridDefinitionID, '' as GridDefinitionName from GridDefinitions union select distinct GridDefinitionID,GridDefinitionName from griddefinitions where SetupID={0}  ", CommonClass.MainSetup.SetupID);
                System.Data.DataSet dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                return dsGrid;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return null;
            }
        }
        private System.Data.DataSet BindGridtypeDomain()
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select distinct GridDefinitionID,GridDefinitionName from griddefinitions  where columns<=56 and setupid={0}  order by GridDefinitionName desc  ", CommonClass.MainSetup.SetupID);
                System.Data.DataSet dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                return dsGrid;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return null;
            }
        }
        private void tsbSavePic_Click(object sender, EventArgs e)
        {
            try
            {
                string s = tsbSavePic.ToolTipText;
                tsbSavePic.ToolTipText = "";
                //Print dialog
               
                //LayoutControl MyLC = new LayoutControl();
               // MyLC.NewLayout(false);
                //MyLC.LoadLayout(true, true, true);
                SetUpPortaitMainMapLayout();
               
            //    Image i = new Bitmap(mainMap.Width, mainMap.Height);
            //    Graphics g = Graphics.FromImage(i);
            //    tsbSavePic.ToolTipText = s;
            //    g.CopyFromScreen(this.PointToScreen(new Point(splitContainer1.Width - splitContainer2.Panel2.Width - 6, this.tabCtlMain.Parent.Location.Y + 27)), new Point(0, 0), new Size(this.Width, this.Height));

            //    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //    saveFileDialog1.Filter = "PNG(*.png)|*.png|JPG(*.jpg)|*.jpg";
            //    saveFileDialog1.InitialDirectory = "C:\\";
            //    if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
            //    {
            //        return;
            //    }

            //    string fileName = saveFileDialog1.FileName;

            //    i.Save(fileName);
            //    MessageBox.Show("Map exported.");
            //    g.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                Debug.WriteLine("tsbSavePic_Click: " + ex.ToString());
            }
        }
        private void SetUpPortaitMainMapLayout()
        {
            //Map MapClone = new Map();
            //string newtype, newLeg;
            //LegendItem newLegSym = null;
            //int laycount = mainMap.Layers.Cast<IMapLayer>().Count();
            //foreach (IMapLayer thislayer in mainMap.GetAllLayers().Cast<IMapLayer>())
            //{
            //    IMapLayer NewLayer = (IMapLayer)thislayer.Clone();
            //    MapPolygonLayer mpoly = null;
            //    mpoly = (MapPolygonLayer)NewLayer;
            //    mpoly.LegendItemVisible = true;
            //    if (mpoly.Projection == null)
            //    {
            //        mpoly.Projection = mainMap.Projection;
            //    }
            //    mpoly.Symbology.AppearsInLegend = true;
            //    mpoly.Symbolizer.LegendText = "Default text";
               
            //    newtype = NewLayer.LegendType.ToString();
            //    newLegSym = (LegendItem)NewLayer;
                
            //    newLeg = NewLayer.LegendSymbolMode.ToString();
            //    MapClone.Layers.Add(NewLayer);
            //}
            //MapClone.Legend = mainMap.Legend;

            //Create Layout form and Layout control            
            LayoutForm _myLayoutForm = new LayoutForm { MapControl = mainMap };
            //MapClone };
            LayoutControl _myLayout = new LayoutControl();
            LayoutMenuStrip lms = null;
            foreach (Control curCTL in _myLayoutForm.Controls)
            {
                if (curCTL is LayoutMenuStrip)
                {
                    lms = (LayoutMenuStrip)curCTL;
                }
            }
            _myLayout = lms.LayoutControl;
            
           

            //Get a list of the layout element
            List<DotSpatial.Controls.LayoutElement> lstMmyLE = new List<DotSpatial.Controls.LayoutElement>();

            //Load an export template (landscape by default)
            //string ExportTemplatePath =   "C:/ProgramData/BenMAP-CE/Data/ExportTemplates";

            string ExportTemplateFile = "BenMAP-CE_landscape_8.5x11.mwl";
            string ExportTemplateFilePath = Path.Combine(CommonClass.DataFilePath, "Data\\ExportTemplates", ExportTemplateFile);
            if (File.Exists(ExportTemplateFilePath))
            {
                _myLayout.LoadLayout(ExportTemplateFilePath, true, false);
            }
            
            //Reset the page margins to 1/2 inch.
            _myLayout.PrinterSettings.DefaultPageSettings.Margins.Left = 50;
            _myLayout.PrinterSettings.DefaultPageSettings.Margins.Right = 50;
            _myLayout.PrinterSettings.DefaultPageSettings.Margins.Top = 50;
            _myLayout.PrinterSettings.DefaultPageSettings.Margins.Bottom = 50;

            //Set drawing quality
            _myLayout.DrawingQuality = SmoothingMode.HighQuality;
            
            // Add MapDisplayElement
            // LayoutMap _MapDisplay = new LayoutMap(mainMap);
            LayoutElement MapLE = _myLayout.LayoutElements.Find(le => le.Name == "Map 1");
            lstMmyLE.Add(MapLE);

            // Add Map Title
            string MapTitleName = "Title 1";
            if (File.Exists(ExportTemplateFilePath))
            {
                MapTitleName = "MapTitle";
            }
            LayoutElement MapTitle =_myLayout.LayoutElements.Find(le => le.Name == MapTitleName);
            if (MapTitle != null)
            {
                LayoutText MapTitleText = null;
                MapTitleText = (LayoutText)MapTitle;
                MapTitleText.Text = _CurrentMapTitle;
                lstMmyLE.Add(MapTitle);              
            }

            //Fit the title & map to the width (and top) of the margins
            _myLayout.MatchElementsSize(lstMmyLE, Fit.Width, true);

            List<LayoutElement> lstMyLE2 = (List<LayoutElement>)lstMmyLE;
            _myLayout.AlignElements(lstMyLE2, Alignment.Top, true);
            _myLayout.AlignElements(lstMyLE2, Alignment.Left, true);

            //Fit & align Legend to the width (and bottom) of the margins
            lstMmyLE.Clear();
            LayoutElement LegendLE = _myLayout.LayoutElements.Find(le => le.Name == "Legend 1");
            lstMmyLE.Add(LegendLE);
            _myLayout.MatchElementsSize(lstMmyLE, Fit.Width, true);
            _myLayout.AlignElements(lstMyLE2, Alignment.Left, true);
            _myLayout.AlignElements(lstMyLE2, Alignment.Bottom, true);
            _myLayout.AlignElements(lstMyLE2, Alignment.Vertical, false);

            //Resize and reposition the legend so it is just below the map layout element
            int MapTop = MapLE.Location.Y;
            int MapBottom = MapTop + (int)MapLE.Size.Height;
            int LegendTop = LegendLE.Location.Y;
            int LegendBottom = LegendTop + (int)LegendLE.Size.Height;
            Size newsize = new System.Drawing.Size((int)LegendLE.Size.Width,(int)(LegendBottom - MapBottom));
            LegendLE.Size = newsize;
            Point newlegendTopPoint = new Point(LegendLE.Location.X,MapBottom);
            LegendLE.Location = newlegendTopPoint;

            //remove extra map 2 (if possible???)
            //string LEName = "", LELoc = "";
            //LayoutElement Map2LE = _myLayout.LayoutElements.Find(le => le.Name == "Map 2");
            //if (Map2LE != null) _myLayout.LayoutElements.Remove(Map2LE);
            //foreach (LayoutElement LE in _myLayout.LayoutElements)
            //{
            //    LEName = LE.Name.ToString();
            //    LELoc = LE.Location.ToString();
            //}

            //Resize the screen so the map is bigger -------------------
            Size prefsize = new Size(1800, 1000);
            _myLayoutForm.Size = prefsize;
            _myLayout.ShowMargin = true;
            _myLayout.ZoomFitToScreen();
            //_myLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _myLayout.ZoomFullViewExtentMap();
            //_myLayoutForm.PerformAutoScale();
            _myLayout.RefreshElements();
            _myLayout.Refresh();
                
            //Add/modify MapLegend
            //LayoutElement _myMapLegendLE = _myLayout.CreateLegendElement();
            //LayoutLegend _myMapLegend = null; // (LayoutLegend)_myMapLegendLE;
            //string thisname = "";
            //int NumLayers = 0;
            //NumLayers = mainMap.GetAllLayers().Count();
            ////_myLayout.MapControl.Layers.Clear();
            //NumLayers = mainMap.GetAllLayers().Count();
            //foreach (IMapLayer thislayer in mainMap.GetAllLayers().Cast<IMapLayer>())
            //{ 
            //    thisname = thislayer.LegendText;
            //    _myLayout.MapControl.Layers.Add(thislayer);
            //}

            //LayoutElement MapLegend = _myLayout.LayoutElements.Find(le => le.Name == "Legend 1");
            //_myMapLegend = (LayoutLegend)MapLegend;

            //int laypos = 0;
            //laypos = _myLayout.MapControl.Layers.Count();
            //foreach (IMapLayer thislayer in _myLayout.MapControl.Layers)
            //{
            //    thisname = thislayer.LegendText;
            //    if (thislayer.IsVisible && thislayer is MapPolygonLayer)
            //    {
            //        IMapLayer modML = new MapPolygonLayer();

            //        modML = (IMapLayer)thislayer.Clone();
            //        modML.LegendText = "test " + laypos.ToString();
            //        //_myLayout.MapControl.Layers.RemoveAt(laypos);
            //        _myLayout.MapControl.Layers.Insert(laypos, modML);
            //        _myMapLegend.Layers.Add(laypos);
            //    }
            //    laypos++;
            //}
            
            //MapLegend = (LayoutElement)_myMapLegend;

            //string LayersString = _myMapLegend.Layers.ToString();
            // Add North Arrow
 
            //Add Map neatline

            _myLayoutForm.ShowDialog(this);

            _myLayoutForm.Dispose();
            //return;
        }


        private void saveFileDialog1_Disposed(object sender, EventArgs e)
        {
        }
        private void tsbChangeProjection_Click(object sender, EventArgs e)
        {
            try
            {
                if (mainMap.GetAllLayers().Count == 0) return;

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

                mainMap.Projection.CopyProperties(mainMap.Projection);
                _SavedExtent = mainMap.GetAllLayers()[0].Extent;
                mainMap.ViewExtents = _SavedExtent;
               
            }
            catch (Exception ex)
            {
            }
        }

        private void tsbAddLayer_Click(object sender, EventArgs e)
        {
            IMapLayer mylayer =  mainMap.AddLayer();
            if (mylayer != null)
            {
                //MapPointLayer myptlayer = (MapPointLayer)mylayer;
                //myptlayer.DataSet.FillAttributes();
                
                //dt = stateLayer.DataSet.DataTable
                // 'Set the datagridview datasource from datatable dt
                //dgvAttributeTable.DataSource = myptlayer.DataSet.DataTable;
                //btnShowHideAttributeTable.Visible = true;
            }
        }

        private void tsbSaveMap_Click(object sender, EventArgs e)
        {// MCB- Shouldn't this be changed 
            try
            {
                if (mainMap.GetAllLayers().Count == 0)
                    return;

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "SHP(*.shp)|*.shp";
                saveFileDialog1.InitialDirectory = "C:\\";
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                tsbChangeProjection_Click(sender, e);
                tsbChangeProjection_Click(sender, e);

                string fileName = saveFileDialog1.FileName;

                FeatureLayer fl = mainMap.GetAllLayers()[0] as FeatureLayer;
                fl.DataSet.SaveAs(fileName, true);
                MessageBox.Show("Shapefile saved.", "File saved");
            }
            catch
            {
            }
        }
        private void tsbChangeCone_Click(object sender, EventArgs e)
        {
            if (mainMap.GetAllLayers().Count < 2)
                return;
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
            if (isLegendHide)
            {
                if (_currentNode == "grid" || _currentNode == "region") { return; }
                this.splitContainer2.BorderStyle = BorderStyle.FixedSingle;
                this.splitContainer2.Panel1.Show();
                splitContainer2.SplitterDistance = 264;
                isLegendHide = false;
                mainMap.ViewExtents = _SavedExtent;  //MCB
            }
            else
            {
                _SavedExtent = mainMap.Extent;
                //splitContainer2.Panel1.Hide();
                splitContainer2.SplitterDistance = 50;
                this.splitContainer2.BorderStyle = BorderStyle.None;
                isLegendHide = true;

                mainMap.ViewExtents = _SavedExtent;
            }
        }
        private FeatureSet getThemeFeatureSet(int iValue, ref double MinValue, ref double MaxValue)
        {
            try
            {
                FeatureSet fsReturn = new FeatureSet();
                MinValue = 0;
                MaxValue = 0;

                Dictionary<string, double> dicValue = new Dictionary<string, double>();
                GridRelationship gRegionGridRelationship = null;
                foreach (GridRelationship gr in CommonClass.LstGridRelationshipAll)
                {
                    if (gr.bigGridID == CommonClass.RBenMAPGrid.GridDefinitionID && gr.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID)
                    {
                        gRegionGridRelationship = gr;
                    }
                    else if (gr.bigGridID == CommonClass.RBenMAPGrid.GridDefinitionID && gr.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID)
                    {
                        gRegionGridRelationship = gr;
                    }
                }

                int idCboAPV = Convert.ToInt32((cbAPVAggregation.SelectedItem as DataRowView)["GridDefinitionID"]);
                int idCboCFGR = Convert.ToInt32((this.cbCRAggregation.SelectedItem as DataRowView)["GridDefinitionID"]);
                int idCboQALY = -1; int idTo = Convert.ToInt32((cboRegion.SelectedItem as DataRowView)["GridDefinitionID"]);
                IFeatureSet fsValue = (mainMap.GetAllLayers()[0] as FeatureLayer).DataSet;
                IFeatureSet fsRegion = (mainMap.GetAllLayers()[1] as FeatureLayer).DataSet;
                int iRow = 0, iCol = 0;
                int i = 0;
                foreach (DataColumn dc in fsValue.DataTable.Columns)
                {
                    if (dc.ColumnName.ToLower() == "col")
                        iCol = i;
                    if (dc.ColumnName.ToLower() == "row")
                        iRow = i;

                    i++;
                }
                foreach (DataRow dr in fsValue.DataTable.Rows)
                {
                    dicValue.Add(dr[iCol].ToString() + "," + dr[iRow].ToString(), Convert.ToDouble(dr[iValue]));
                }
                fsReturn.DataTable.Columns.Add("Col", typeof(int));
                fsReturn.DataTable.Columns.Add("Row", typeof(int));
                fsReturn.DataTable.Columns.Add("ThemeValue", typeof(double));
                i = 0;
                if (_tableObject is List<CRSelectFunctionCalculateValue>)
                {
                    CRSelectFunctionCalculateValue cr = ((List<CRSelectFunctionCalculateValue>)_tableObject).First();
                    cr = APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(cr, idCboCFGR == -1 ? CommonClass.GBenMAPGrid.GridDefinitionID : idCboCFGR, idTo);
                    dicValue.Clear();
                    foreach (CRCalculateValue crv in cr.CRCalculateValues)
                    {
                        if (!dicValue.ContainsKey(crv.Col + "," + crv.Row))
                            dicValue.Add(crv.Col + "," + crv.Row, crv.PointEstimate);

                    }
                    foreach (DataRow dr in fsRegion.DataTable.Rows)
                    {
                        Feature f = new Feature();
                        f.BasicGeometry = new DotSpatial.Topology.Point(fsRegion.GetFeature(i).Envelope.ToExtent().Center.X, fsRegion.GetFeature(i).Envelope.ToExtent().Center.Y);
                        fsReturn.AddFeature(f);
                        fsReturn.DataTable.Rows[i]["Col"] = dr["Col"];
                        fsReturn.DataTable.Rows[i]["Row"] = dr["Row"];
                        fsReturn.DataTable.Rows[i]["ThemeValue"] = 0;
                        try
                        {
                            if (dicValue.Keys.Contains(dr["Col"] + "," + dr["Row"]))
                            {
                                fsReturn.DataTable.Rows[i]["ThemeValue"] = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
                                if (MinValue > Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4)) MinValue = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
                                if (MaxValue < Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4)) MaxValue = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
                            }
                        }
                        catch (Exception ex)
                        { }
                        i++;
                    }
                    return fsReturn;
                }
                else if (_tableObject is List<AllSelectValuationMethodAndValue>)
                {
                    AllSelectValuationMethodAndValue cr = ((List<AllSelectValuationMethodAndValue>)_tableObject).First();
                    GridRelationship gr = null;
                    int id = idCboAPV == -1 ? (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != -1) ? CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID : CommonClass.GBenMAPGrid.GridDefinitionID : idCboAPV;
                    BenMAPGrid benMAPGrid = Grid.GridCommon.getBenMAPGridFromID(id);
                    if (CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == id && p.smallGridID == idTo).Count() > 0)
                    {
                        gr = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == benMAPGrid.GridDefinitionID && p.smallGridID == idTo).First();
                    }
                    else if (CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idTo && p.smallGridID == benMAPGrid.GridDefinitionID).Count() > 0)
                    {
                        gr = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idTo && p.smallGridID == benMAPGrid.GridDefinitionID).First();

                    }
                    cr = APVX.APVCommonClass.ApplyAllSelectValuationMethodAndValueAggregation(gr, benMAPGrid, cr);
                    dicValue.Clear();
                    foreach (APVValueAttribute crv in cr.lstAPVValueAttributes)
                    {
                        if (!dicValue.ContainsKey(crv.Col + "," + crv.Row))
                            dicValue.Add(crv.Col + "," + crv.Row, crv.PointEstimate);

                    }
                    foreach (DataRow dr in fsRegion.DataTable.Rows)
                    {
                        Feature f = new Feature();
                        f.BasicGeometry = new DotSpatial.Topology.Point(fsRegion.GetFeature(i).Envelope.ToExtent().Center.X, fsRegion.GetFeature(i).Envelope.ToExtent().Center.Y);
                        fsReturn.AddFeature(f);
                        fsReturn.DataTable.Rows[i]["Col"] = dr["Col"];
                        fsReturn.DataTable.Rows[i]["Row"] = dr["Row"];
                        fsReturn.DataTable.Rows[i]["ThemeValue"] = 0;
                        try
                        {
                            if (dicValue.Keys.Contains(dr["Col"] + "," + dr["Row"]))
                            {
                                fsReturn.DataTable.Rows[i]["ThemeValue"] = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
                                if (MinValue > Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4)) MinValue = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
                                if (MaxValue < Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4)) MaxValue = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
                            }
                        }
                        catch (Exception ex)
                        { }
                        i++;
                    }
                    return fsReturn;
                }
                else if (_tableObject is List<AllSelectQALYMethodAndValue>)
                {
                    AllSelectQALYMethodAndValue cr = ((List<AllSelectQALYMethodAndValue>)_tableObject).First();
                    GridRelationship gr = null;
                    int id = idCboQALY == -1 ? (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation != null && CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID != -1) ? CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID : CommonClass.GBenMAPGrid.GridDefinitionID : idCboQALY;
                    BenMAPGrid benMAPGrid = Grid.GridCommon.getBenMAPGridFromID(id);
                    if (CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == id && p.smallGridID == idTo).Count() > 0)
                    {
                        gr = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == benMAPGrid.GridDefinitionID && p.smallGridID == idTo).First();
                    }
                    else if (CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idTo && p.smallGridID == benMAPGrid.GridDefinitionID).Count() > 0)
                    {
                        gr = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idTo && p.smallGridID == benMAPGrid.GridDefinitionID).First();

                    }
                    cr = APVX.APVCommonClass.ApplyAllSelectQALYMethodAndValueAggregation(gr, benMAPGrid, cr);
                    dicValue.Clear();
                    foreach (QALYValueAttribute crv in cr.lstQALYValueAttributes)
                    {
                        if (!dicValue.ContainsKey(crv.Col + "," + crv.Row))
                            dicValue.Add(crv.Col + "," + crv.Row, crv.PointEstimate);

                    }
                    foreach (DataRow dr in fsRegion.DataTable.Rows)
                    {
                        Feature f = new Feature();
                        f.BasicGeometry = new DotSpatial.Topology.Point(fsRegion.GetFeature(i).Envelope.ToExtent().Center.X, fsRegion.GetFeature(i).Envelope.ToExtent().Center.Y);
                        fsReturn.AddFeature(f);
                        fsReturn.DataTable.Rows[i]["Col"] = dr["Col"];
                        fsReturn.DataTable.Rows[i]["Row"] = dr["Row"];
                        fsReturn.DataTable.Rows[i]["ThemeValue"] = 0;
                        try
                        {
                            if (dicValue.Keys.Contains(dr["Col"] + "," + dr["Row"]))
                            {
                                fsReturn.DataTable.Rows[i]["ThemeValue"] = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
                                if (MinValue > Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4)) MinValue = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
                                if (MaxValue < Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4)) MaxValue = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
                            }
                        }
                        catch (Exception ex)
                        { }
                        i++;
                    }
                    return fsReturn;
                }


                if (CommonClass.RBenMAPGrid.GridDefinitionID == CommonClass.GBenMAPGrid.GridDefinitionID)
                {
                    i = 0;
                    foreach (DataRow dr in fsRegion.DataTable.Rows)
                    {
                        Feature f = new Feature();
                        f.BasicGeometry = new DotSpatial.Topology.Point(fsRegion.GetFeature(i).Envelope.ToExtent().Center.X, fsRegion.GetFeature(i).Envelope.ToExtent().Center.Y);
                        fsReturn.AddFeature(f);
                        fsReturn.DataTable.Rows[i]["Col"] = dr["Col"];
                        fsReturn.DataTable.Rows[i]["Row"] = dr["Row"];
                        try
                        {
                            if (dicValue.Keys.Contains(dr["Col"] + "," + dr["Row"]))
                            {
                                fsReturn.DataTable.Rows[i]["ThemeValue"] = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
                                if (MinValue > Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4)) MinValue = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
                                if (MaxValue < Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4)) MaxValue = Math.Round(dicValue[dr["Col"] + "," + dr["Row"]], 4);
                            }
                        }
                        catch (Exception ex)
                        { }
                        i++;
                    }
                    return fsReturn;
                }
                i = 0;
                foreach (DataRow dr in fsRegion.DataTable.Rows)
                {
                    Feature f = new Feature();
                    f.BasicGeometry = new DotSpatial.Topology.Point(fsRegion.GetFeature(i).Envelope.ToExtent().Center.X, fsRegion.GetFeature(i).Envelope.ToExtent().Center.Y);
                    fsReturn.AddFeature(f);
                    fsReturn.DataTable.Rows[i]["Col"] = dr["Col"];
                    fsReturn.DataTable.Rows[i]["Row"] = dr["Row"];
                    if (gRegionGridRelationship.bigGridID == CommonClass.RBenMAPGrid.GridDefinitionID)
                    {
                        var query = from a in gRegionGridRelationship.lstGridRelationshipAttribute
                                    where a.bigGridRowCol.Col == Convert.ToInt32(fsReturn.DataTable.Rows[i]["Col"]) &&
                                        a.bigGridRowCol.Row == Convert.ToInt32(fsReturn.DataTable.Rows[i]["Row"])
                                    select a.smallGridRowCol;
                        double d = 0;
                        if (query != null && query.Count() > 0)
                        {
                            foreach (RowCol rc in query.First())
                            {
                                try
                                {
                                    if (dicValue.Keys.Contains(rc.Col + "," + rc.Row))
                                        d += dicValue[rc.Col + "," + rc.Row];
                                }
                                catch (Exception ex)
                                { }
                            }
                            d = d / Convert.ToDouble(query.First().Count());
                        }
                        if (MinValue > d) MinValue = d;
                        if (MaxValue < d) MaxValue = d;
                        fsReturn.DataTable.Rows[i]["ThemeValue"] = Math.Round(d, 4);
                    }
                    else
                    {
                        var query = from a in gRegionGridRelationship.lstGridRelationshipAttribute
                                    where a.smallGridRowCol.Contains(new RowCol()
                                    {
                                        Col = Convert.ToInt32(fsReturn.DataTable.Rows[i]["Col"])
                                        ,
                                        Row = Convert.ToInt32(fsReturn.DataTable.Rows[i]["Row"])
                                    }, new RowColComparer())
                                    select a.bigGridRowCol;
                        if (query != null && query.Count() > 0)
                        {
                            RowCol rc = query.First();
                            try
                            {
                                if (dicValue.Keys.Contains(rc.Col + "," + rc.Row))
                                {
                                    fsReturn.DataTable.Rows[i]["ThemeValue"] = Math.Round(dicValue[rc.Col + "," + rc.Row], 4);
                                }
                                else
                                {
                                    fsReturn.DataTable.Rows[i]["ThemeValue"] = 0.000;
                                }
                                if (MinValue > Math.Round(dicValue[rc.Col + "," + rc.Row], 12)) MinValue = Math.Round(dicValue[rc.Col + "," + rc.Row], 4);
                                if (MaxValue < Math.Round(dicValue[rc.Col + "," + rc.Row], 12)) MaxValue = Math.Round(dicValue[rc.Col + "," + rc.Row], 4);
                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                    i++;
                }

                return fsReturn;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void btnPieTheme_Click(object sender, EventArgs e)
        {
            try
            {
                double MinValue = 0;
                double MaxValue = 0;
                IMapLayer removeLayer = null;
                if (mainMap.GetAllLayers().Count == 3)
                {
                    foreach (IMapLayer layer in mainMap.GetAllLayers())
                    {
                        if (layer is MapPointLayer)
                            removeLayer = layer;
                    }
                }
                if (removeLayer != null)
                    mainMap.GetAllLayers().Remove(removeLayer);
                if (mainMap.GetAllLayers().Count != 2)
                {
                    MessageBox.Show("No available layer to generate pie theme.");
                    return;
                }
                else
                {
                    IFeatureSet fs = (mainMap.GetAllLayers()[1] as PolygonLayer).DataSet;
                    if (fs.DataTable.Rows.Count > 5000)
                    {
                        MessageBox.Show("Too many features to be displayed in this aggregation layer.");
                        return;
                    }
                    WaitShow("Loading theme layer... ");
                    FeatureSet fsValue = null;
                    if (LayerObject != null && LayerObject is BenMAPLine)
                    {
                    }

                    fsValue = getThemeFeatureSet((mainMap.GetAllLayers()[0] as PolygonLayer).DataSet.DataTable.Columns.Count - 1, ref MinValue, ref MaxValue);

                    if (MaxValue <= 0)
                    {
                        double fz = Math.Abs(MaxValue);
                        if (fz < Math.Abs(MinValue))
                        {
                            MaxValue = Math.Abs(MinValue);
                            MinValue = fz;
                        }
                        else
                        {
                            MinValue = Math.Abs(MinValue);
                            MaxValue = Math.Abs(MaxValue);
                        }
                    }
                    if (MaxValue == 0)
                    {
                        WaitClose();
                        return;
                    }
                    MaxValue = Math.Abs(MaxValue);
                    MinValue = Math.Abs(MinValue);
                    if (MinValue > MaxValue)
                    {
                        double d = MinValue;
                        MinValue = MaxValue;
                        MaxValue = d;
                    }
                    fsValue.Name = "ThemeLayer";
                    mainMap.Layers.Add(fsValue);
                    PointScheme ps = new PointScheme();

                    PointSymbolizer commonSymboliser = new PointSymbolizer();
                    commonSymboliser.IsVisible = true;
                    commonSymboliser.Smoothing = true;
                    ps.AppearsInLegend = true;

                    ps.LegendText = "ThemeValue";
                    ps.ClearCategories();
                    ps.Categories.Clear();
                    foreach (DataRow dr in fsValue.DataTable.Rows)
                    {
                        double gridvalue = Math.Abs(Convert.ToDouble(dr["ThemeValue"]) * (100.000 / MaxValue));
                        if (gridvalue > 0)
                        {
                            Bitmap ig = null; if (Convert.ToDouble(dr["ThemeValue"]) < 0)
                            {
                                ig = getonly3DPie(150, Color.Green);
                            }
                            else
                            {
                                ig = getonly3DPie(150, Color.Red);
                            }
                            if (Convert.ToInt32(gridvalue) == 0) continue;
                            Bitmap ig2 = null;
                            if ((sender as ToolStripButton).Name == "btnPieTheme")
                            {
                                ig2 = new Bitmap(ig, new System.Drawing.Size(Convert.ToInt32(gridvalue), Convert.ToInt32(gridvalue)));
                            }
                            else
                            {
                                if (Convert.ToDouble(dr["ThemeValue"]) < 0)
                                {
                                    ig2 = DrawCell(Color.Green, 0, 10, 20, Convert.ToInt32(gridvalue), 10);
                                }
                                else
                                    ig2 = DrawCell(Color.Red, 0, 10, 20, Convert.ToInt32(gridvalue), 10);
                            }

                            PictureSymbol psymbol = new PictureSymbol(ig2);
                            psymbol.Size = new Size2D(ig2.Width, ig2.Height);
                            PointSymbolizer NONcommonSymboliser = new PointSymbolizer();
                            NONcommonSymboliser.Smoothing = true;
                            NONcommonSymboliser.IsVisible = true;
                            NONcommonSymboliser.Symbols.Clear();
                            NONcommonSymboliser.Symbols.Add(psymbol);

                            PointCategory pc1 = new PointCategory(NONcommonSymboliser);
                            pc1.FilterExpression = "[ThemeValue] = " + dr["ThemeValue"].ToString();
                            pc1.LegendText = "[ThemeValue] = " + dr["ThemeValue"].ToString();
                            pc1.DisplayExpression();

                            ps.AddCategory(pc1);
                        }
                    }
                    (mainMap.GetAllLayers()[2] as PointLayer).Symbology = ps;
                }
                WaitClose();
            }
            catch
            {
                WaitClose();
            }
        }


        private Bitmap DrawCell(Color myColor, int x, int y, int width, int height, float iDeep)
        {
            Bitmap objBitmap = new Bitmap(Convert.ToInt32(width + iDeep * 1.2), Convert.ToInt32(height + iDeep * 1.2));
            Graphics objGraphics = Graphics.FromImage(objBitmap);
            objGraphics.Clear(Color.Transparent);
            Rectangle Frect =
                new Rectangle((int)x, (int)y, (int)width, (int)height);
            Rectangle Brect =
                new Rectangle(Frect.X + (int)iDeep, Frect.Y - (int)iDeep, Frect.Width, Frect.Height);

            PointF p1 = new PointF((float)Frect.X, (float)Frect.Y);
            PointF p2 = new PointF((float)Frect.X, (float)(Frect.Y + Frect.Height));
            PointF p3 = new PointF((float)(Frect.X + Frect.Width), (float)(Frect.Y + Frect.Height));
            PointF p4 = new PointF((float)(Frect.X + Frect.Width), (float)Frect.Y);

            PointF p5 = new PointF((float)Brect.X, (float)Brect.Y);
            PointF p6 = new PointF((float)Brect.X, (float)(Brect.Y + Brect.Height));
            PointF p7 = new PointF((float)(Brect.X + Brect.Width), (float)(Brect.Y + Brect.Height));
            PointF p8 = new PointF((float)(Brect.X + Brect.Width), (float)Brect.Y);

            PointF[] ptsArray1 =         {
          p1, p2, p6, p5
      };

            PointF[] ptsArray2 =          {
          p2, p3, p7, p6
      };

            PointF[] ptsArray3 =         {
          p4, p3, p7, p8
      };

            PointF[] ptsArray4 =         {
          p1, p4, p8, p5
      };

            SolidBrush defaultBrush = new SolidBrush(Color.FromArgb(128, myColor.R, myColor.G, myColor.B));
            int r, g, b;
            r = myColor.R - 37;
            g = myColor.G - 37;
            b = myColor.B - 45;
            if (r < 0) r = 0;
            if (g < 0) g = 0;
            if (b < 0) b = 0;
            SolidBrush topBrush = new SolidBrush(Color.FromArgb(188, r, g, b));
            int r1, g1, b1;

            r1 = myColor.R - 52;
            g1 = myColor.G - 52;
            b1 = myColor.B - 88;
            if (r1 < 0) r1 = 0;
            if (g1 < 0) g1 = 0;
            if (b1 < 0) b1 = 0;
            SolidBrush rightBrush = new SolidBrush(Color.FromArgb(220, r1, g1, b1));

            SolidBrush invisibleBrush =
                new SolidBrush(Color.FromArgb(111, myColor.R, myColor.G, myColor.B));
            SolidBrush visibleBrush =
                new SolidBrush(Color.FromArgb(188, myColor.R, myColor.G, myColor.B));
            SolidBrush FrectBrush =
                new SolidBrush(Color.FromArgb(220, myColor.R, myColor.G, myColor.B));

            objGraphics.FillRectangle(defaultBrush, Brect);
            objGraphics.DrawRectangle(Pens.Black, Brect);

            objGraphics.FillPolygon(defaultBrush, ptsArray1);
            objGraphics.DrawPolygon(Pens.Black, ptsArray1);
            objGraphics.FillPolygon(defaultBrush, ptsArray2);
            objGraphics.DrawPolygon(Pens.Black, ptsArray2);
            objGraphics.FillPolygon(rightBrush, ptsArray3);
            objGraphics.DrawPolygon(Pens.Black, ptsArray3);
            objGraphics.FillPolygon(topBrush, ptsArray4);
            objGraphics.DrawPolygon(Pens.Black, ptsArray4);

            objGraphics.FillRectangle(FrectBrush, Frect);
            objGraphics.DrawRectangle(Pens.Black, Frect);

            invisibleBrush.Dispose();
            visibleBrush.Dispose();
            FrectBrush.Dispose();
            return objBitmap;
        }

        private Bitmap get3DPie()
        {
            const int width = 150, height = 150;
            int x = 30, y = 50;

            int pieWidth = 120, pieHeight = 40, pieShadow = 15;
            int[] arrVote = { 70, 90, 80, 20, 60, 40 };
            Random oRan = new Random();

            Bitmap objBitmap = new Bitmap(width, height);
            Graphics objGraphics = Graphics.FromImage(objBitmap);
            objGraphics.Clear(Color.Transparent);
            SolidBrush objBrush = new SolidBrush(Color.Blue);
            objGraphics.SmoothingMode = SmoothingMode.AntiAlias;

            int iCurrentPos = 0;

            Color[] arrColor = { Color.Red, Color.Red, Color.Red, Color.Red, Color.Red, Color.Red };

            for (int i = arrVote.Length - 1; i >= 0; i--)
            {
                arrColor[i] = Color.FromArgb(oRan.Next(255), oRan.Next(255), oRan.Next(255));
            }

            for (int i = arrVote.Length - 1; i >= 0; i--)
            {
                objBrush.Color = arrColor[i];
                for (int iLoop2 = 0; iLoop2 < pieShadow; iLoop2++)
                    objGraphics.FillPie(new HatchBrush(HatchStyle.Percent50, objBrush.Color), x, y + iLoop2, pieWidth, pieHeight, iCurrentPos, arrVote[i]);
                iCurrentPos += arrVote[i];
            }

            iCurrentPos = 0;
            for (int i = arrVote.Length - 1; i >= 0; i--)
            {
                objBrush.Color = arrColor[i];
                objGraphics.FillPie(objBrush, x, y, pieWidth, pieHeight, iCurrentPos, arrVote[i]);
                iCurrentPos += arrVote[i];
            }
            return objBitmap;
        }

        private Bitmap getonly3DPie(int width, Color c)
        {
            int height = width;
            int x = 30, y = 50;

            int pieWidth = 120, pieHeight = 40, pieShadow = 15;
            int[] arrVote = { 70, 90, 80, 20, 60, 40 };
            Random oRan = new Random();

            Bitmap objBitmap = new Bitmap(width, height);
            Graphics objGraphics = Graphics.FromImage(objBitmap);
            objGraphics.Clear(Color.Transparent);
            SolidBrush objBrush = new SolidBrush(Color.Blue);
            objGraphics.SmoothingMode = SmoothingMode.AntiAlias;

            int iCurrentPos = 0;

            Color[] arrColor = { c, c, c, c, c, c };
            for (int i = arrVote.Length - 1; i >= 0; i--)
            {
                arrColor[i] = c;
            }

            for (int i = arrVote.Length - 1; i >= 0; i--)
            {
                objBrush.Color = arrColor[i];
                for (int iLoop2 = 0; iLoop2 < pieShadow; iLoop2++)
                    objGraphics.FillPie(new HatchBrush(HatchStyle.Percent50, objBrush.Color), x, y + iLoop2, pieWidth, pieHeight, iCurrentPos, arrVote[i]);
                iCurrentPos += arrVote[i];
            }

            iCurrentPos = 0;
            for (int i = arrVote.Length - 1; i >= 0; i--)
            {
                objBrush.Color = arrColor[i];
                objGraphics.FillPie(objBrush, x, y, pieWidth, pieHeight, iCurrentPos, arrVote[i]);
                iCurrentPos += arrVote[i];
            }
            return objBitmap;
        }


        private void ClearMapTableChart()
        {
            if (!_MapAlreadyDisplayed) mainMap.Layers.Clear();

            OLVResultsShow.SetObjects(null);
            _tableObject = null;
            zedGraphCtl.Visible = false;
            btnApply.Visible = false;
            olvRegions.Visible = false;
            cbGraph.Visible = false;
            groupBox9.Visible = false;
            groupBox1.Visible = false;
            btnSelectAll.Visible = false;
            if (_MapAlreadyDisplayed) picGIS.Visible = false;
            else picGIS.Visible = true;
        }

        private void olvCRFunctionResult_DoubleClick(object sender, EventArgs e)
        {
            btShowCRResult_Click(sender, e);

            
            return;

            //NOTE: nothing done here!  All done by btShowCRResult_Click()

            //if (olvCRFunctionResult.Objects == null) return;
            //string Tip = "Drawing health impact function result layer";
            //WaitShow(Tip);
            //bool bGIS = true;
            //bool bTable = true;
            //bool bChart = true;
            //int i = 0;
            //int iOldGridType = CommonClass.GBenMAPGrid.GridDefinitionID;
            //CRSelectFunctionCalculateValue crSelectFunctionCalculateValue = null;
            //for (int icro = 0; icro < CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; icro++)
            //{
            //    CRSelectFunctionCalculateValue cro = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[icro];
            //}
            //foreach (CRSelectFunctionCalculateValue cr in olvCRFunctionResult.SelectedObjects)
            //{
            //    crSelectFunctionCalculateValue = cr;
            //}
            //if (crSelectFunctionCalculateValue != null)
            //{
            //    if (cbCRAggregation.SelectedIndex != -1 && cbCRAggregation.SelectedIndex != 0)
            //    {
            //        DataRowView drv = cbCRAggregation.SelectedItem as DataRowView;
            //        int iAggregationGridType = Convert.ToInt32(drv["GridDefinitionID"]);
            //        if (iAggregationGridType != CommonClass.GBenMAPGrid.GridDefinitionID)
            //        {
            //            crSelectFunctionCalculateValue = APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crSelectFunctionCalculateValue, CommonClass.GBenMAPGrid.GridDefinitionID, iAggregationGridType);
            //            CommonClass.GBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(iAggregationGridType);
            //        }

            //    }
            //    if (i == 0)
            //    {
            //        ClearMapTableChart();
            //        if (rdbShowActiveCR.Checked)
            //        {
            //            if (tabCtlMain.SelectedIndex == 0)
            //            {
            //                bTable = false;
            //                bChart = false;
            //            }
            //            else if (tabCtlMain.SelectedIndex == 1)
            //            {
            //                bGIS = false;
            //                bChart = false;
            //            }
            //            else if (tabCtlMain.SelectedIndex == 2)
            //            {
            //                bGIS = false;
            //                bTable = false;
            //            }
            //        }
            //        if (bTable)
            //        {
            //            InitTableResult(crSelectFunctionCalculateValue);

            //        }
            //        if (bChart)
            //        {
            //            foreach (CRSelectFunctionCalculateValue cr in olvCRFunctionResult.SelectedObjects)
            //            {
            //                InitChartResult(cr, iOldGridType);
            //                break;
            //            }
            //        }
            //        if (bGIS)
            //        {   
            //            //Remove the old version of the layer if exists already
            //            foreach (MapPolygonLayer aLayer in mainMap.GetPolygonLayers())
            //            {
            //                if (aLayer.Name == "CRResult")
            //                {
            //                    mainMap.Layers.Remove(aLayer);
            //                    break;
            //                }
            //            }
                        
            //            mainMap.ProjectionModeReproject = ActionMode.Never;
            //            mainMap.ProjectionModeDefine = ActionMode.Never;

            //            string shapeFileName = "";
                        
            //            if (CommonClass.GBenMAPGrid is ShapefileGrid)
            //            {
            //                //mainMap.Layers.Clear();
            //                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
            //                {
            //                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
            //                }
            //            }
            //            else if (CommonClass.GBenMAPGrid is RegularGrid)
            //            {
            //                //mainMap.Layers.Clear();
            //                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp"))
            //                {
            //                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp";
            //                }
            //            }
            //            tsbChangeProjection.Text = "change projection to Albers";
            //            MapPolygonLayer _ResultPolygonLayer = new MapPolygonLayer();
            //            _ResultPolygonLayer = (MapPolygonLayer)mainMap.AddLayer(shapeFileName);

            //            //DataTable dt = (mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).DataSet.DataTable;                        
            //            //(mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).LegendText = "CRResult";
            //            //(mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).Name = "CRResult";

            //            DataTable dt = _ResultPolygonLayer.DataSet.DataTable;
            //            _ResultPolygonLayer.LegendText = "CRResult";
            //            _ResultPolygonLayer.Name = "CRResult";
                        
            //            int j = 0;
            //            int iCol = 0;
            //            int iRow = 0;
            //            List<string> lstRemoveName = new List<string>();
            //            while (j < dt.Columns.Count)
            //            {
            //                if (dt.Columns[j].ColumnName.ToLower() == "col") iCol = j;
            //                if (dt.Columns[j].ColumnName.ToLower() == "row") iRow = j;

            //                j++;
            //            }
            //            j = 0;

            //            while (j < dt.Columns.Count)
            //            {
            //                if (dt.Columns[j].ColumnName.ToLower() == "col" || dt.Columns[j].ColumnName.ToLower() == "row")
            //                { }
            //                else
            //                    lstRemoveName.Add(dt.Columns[j].ColumnName);

            //                j++;
            //            }
            //            foreach (string s in lstRemoveName)
            //            {
            //                dt.Columns.Remove(s);
            //            }
            //            dt.Columns.Add("Value", typeof(double));
            //            j = 0;
            //            while (j < dt.Columns.Count)
            //            {
            //                if (dt.Columns[j].ColumnName.ToLower() == "col") iCol = j;
            //                if (dt.Columns[j].ColumnName.ToLower() == "row") iRow = j;

            //                j++;
            //            }
            //            j = 0;
            //            Dictionary<string, double> dicAll = new Dictionary<string, double>();
            //            foreach (CRCalculateValue crcv in crSelectFunctionCalculateValue.CRCalculateValues)
            //            {
            //                if (!dicAll.ContainsKey(crcv.Col + "," + crcv.Row))
            //                    dicAll.Add(crcv.Col + "," + crcv.Row, crcv.PointEstimate);
            //            }
            //            foreach (DataRow dr in dt.Rows)
            //            {
            //                try
            //                {
            //                    if (dicAll.ContainsKey(dr[iCol] + "," + dr[iRow]))
            //                        dr["Value"] = dicAll[dr[iCol] + "," + dr[iRow]];
            //                    else
            //                        dr["Value"] = 0;
            //                }
            //                catch (Exception ex)
            //                {
            //                }
            //            }
            //            if (File.Exists(CommonClass.DataFilePath + @"\Tmp\Incidence.shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Tmp\Incidence.shp");
            //            //(mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).DataSet.SaveAs(CommonClass.DataFilePath + @"\Tmp\Incidence.shp", true);
            //            _ResultPolygonLayer.DataSet.SaveAs(CommonClass.DataFilePath + @"\Tmp\Incidence.shp", true);
                        
            //            //mainMap.Layers.Clear();

            //            MapPolygonLayer polLayer = _ResultPolygonLayer;
            //            //mainMap.Layers.Add(polLayer);           //(CommonClass.DataFilePath + @"\Tmp\Incidence.shp");
                        
            //            //MapPolygonLayer polLayer = mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer;
            //            string strValueField = polLayer.DataSet.DataTable.Columns[polLayer.DataSet.DataTable.Columns.Count - 1].ColumnName;
                        
            //            _columnName = strValueField;
            //            polLayer.Symbology = CreateResultPolyScheme(ref polLayer, 6, "H"); //-MCB added
            //            double dMinValue = 0.0;
            //            double dMaxValue = 0.0;
            //            dMinValue = crSelectFunctionCalculateValue.CRCalculateValues.Count == 0 ? 0 : crSelectFunctionCalculateValue.CRCalculateValues.Min(a => a.PointEstimate);
            //            dMaxValue = crSelectFunctionCalculateValue.CRCalculateValues.Count == 0 ? 0 : crSelectFunctionCalculateValue.CRCalculateValues.Max(a => a.PointEstimate);

            //            _dMinValue = dMinValue;
            //            _dMaxValue = dMaxValue;
            //            //_currentLayerIndex = mainMap.Layers.Count - 1;
            //            _CurrentIMapLayer = polLayer;
            //            string pollutantUnit = string.Empty; 
            //            _columnName = strValueField;
            //            CurrentMapTitle = CommonClass.MainSetup.SetupName + " Setup: " + _ResultPolygonLayer.LegendText + ", Health Impact Function Result"; 
            //            RenderMainMap(true, "H");

            //            addRegionLayerGroupToMainMap();
            //        }
            //    }
            //    i++;
            //    CommonClass.GBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(iOldGridType);
            //}
            //WaitClose();
        }
        private void tlvAPVResult_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                ClearMapTableChart();
                if (tlvAPVResult.SelectedObjects.Count == 0) return;

                string Tip = "Drawing pooled valuation results layer";
                WaitShow(Tip);
                bool bGIS = true;
                bool bTable = true;
                bool bChart = true;

                List<AllSelectValuationMethodAndValue> lstallSelectValuationMethodAndValue = new List<AllSelectValuationMethodAndValue>();
                if (CommonClass.IncidencePoolingAndAggregationAdvance == null || CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation == null)
                {
                    chbAPVAggregation.Checked = false;

                }
                else
                {
                    chbAPVAggregation.Checked = true;

                }

                if (sender is ObjectListView || sender is Button)
                {
                    foreach (KeyValuePair<AllSelectValuationMethod, string> keyValue in tlvAPVResult.SelectedObjects)
                    {
                        AllSelectValuationMethod allSelectValuationMethod = keyValue.Key;
                        if (rbAPVOnlyOne.Checked)
                        {
                            ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == keyValue.Value).First();

                            AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = null;

                            try
                            {
                                if (vb.LstAllSelectValuationMethodAndValueAggregation == null || vb.LstAllSelectValuationMethodAndValueAggregation.Count == 0)
                                {

                                    allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue.Where(p => p.AllSelectValuationMethod.ID == allSelectValuationMethod.ID).First();

                                }
                                else
                                {

                                    allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValueAggregation.Where(p => p.AllSelectValuationMethod.ID == allSelectValuationMethod.ID).First();
                                }
                                if (allSelectValuationMethodAndValue != null && allSelectValuationMethodAndValue.lstAPVValueAttributes != null && allSelectValuationMethodAndValue.lstAPVValueAttributes.Count() > 0)
                                    lstallSelectValuationMethodAndValue.Add(allSelectValuationMethodAndValue);
                            }
                            catch
                            { }
                        }
                        else
                        {
                            AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = null;
                            if (allSelectValuationMethod.ID < 0) continue;

                            ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == keyValue.Value).First();

                            try
                            {


                                allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue.Where(p => p.AllSelectValuationMethod.ID == allSelectValuationMethod.ID).First();

                                if (allSelectValuationMethodAndValue != null && allSelectValuationMethodAndValue.lstAPVValueAttributes != null && allSelectValuationMethodAndValue.lstAPVValueAttributes.Count() > 0)
                                    lstallSelectValuationMethodAndValue.Add(allSelectValuationMethodAndValue);
                            }
                            catch
                            {
                            }
                        }
                    }
                    tabCtlMain.SelectedIndex = 1;
                    if (tlvAPVResult.SelectedObjects.Count > 1)
                    {

                        bGIS = false;
                        bChart = false;
                    }
                }
                else
                {
                    if (_MapAlreadyDisplayed && _APVdragged)//MCB- a kluge: Need a better way to determine if sender was from map
                    {
                        bGIS = true;
                        bChart = false;
                        tabCtlMain.SelectedIndex = 0;

                    }
                    else
                    {
                        bGIS = false;
                        bChart = false;
                        tabCtlMain.SelectedIndex = 1;
                    }

                    foreach (KeyValuePair<AllSelectValuationMethod, string> keyValue in tlvAPVResult.Objects)
                    {
                        AllSelectValuationMethod allSelectValuationMethod = keyValue.Key;
                        if (rbAPVOnlyOne.Checked)
                        {
                            ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == keyValue.Value).First();

                            AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = null;

                            try
                            {
                                if (vb.LstAllSelectValuationMethodAndValueAggregation == null || vb.LstAllSelectValuationMethodAndValueAggregation.Count == 0)
                                {

                                    allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue.Where(p => p.AllSelectValuationMethod.ID == allSelectValuationMethod.ID).First();

                                }
                                else
                                {

                                    allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValueAggregation.Where(p => p.AllSelectValuationMethod.ID == allSelectValuationMethod.ID).First();
                                }
                                if (allSelectValuationMethodAndValue != null && allSelectValuationMethodAndValue.lstAPVValueAttributes != null && allSelectValuationMethodAndValue.lstAPVValueAttributes.Count() > 0)
                                    lstallSelectValuationMethodAndValue.Add(allSelectValuationMethodAndValue);
                            }
                            catch
                            { }
                        }
                        else
                        {
                            AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = null;

                            ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == keyValue.Value).First();
                            try
                            {


                                allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue.Where(p => p.AllSelectValuationMethod.ID == allSelectValuationMethod.ID).First();

                                if (allSelectValuationMethodAndValue != null && allSelectValuationMethodAndValue.lstAPVValueAttributes != null && allSelectValuationMethodAndValue.lstAPVValueAttributes.Count() > 0)
                                    lstallSelectValuationMethodAndValue.Add(allSelectValuationMethodAndValue);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                if (lstallSelectValuationMethodAndValue[0].lstAPVValueAttributes.Count == 1 && !CommonClass.CRRunInPointMode)
                {
                    lstAPVRforCDF = lstallSelectValuationMethodAndValue;
                    canshowCDF = true;
                    bChart = true;
                }
                else
                {
                    canshowCDF = false;
                    lstAPVRforCDF = new List<AllSelectValuationMethodAndValue>();
                }
                iCDF = 2;
                ClearMapTableChart();
                if (this.rbShowActiveAPV.Checked)
                {
                    if (tabCtlMain.SelectedIndex == 0)
                    {
                        bTable = false;
                        bChart = false;
                    }
                    else if (tabCtlMain.SelectedIndex == 1)
                    {
                        bGIS = false;
                        bChart = false;
                    }
                    else if (tabCtlMain.SelectedIndex == 2)
                    {
                        bGIS = false;
                        bTable = false;
                    }
                }
                if (lstallSelectValuationMethodAndValue == null)
                {
                    WaitClose();
                    MessageBox.Show("No result in this method. It might have been pooled before!");
                    return;
                }

                else if (lstallSelectValuationMethodAndValue.Count == 0)
                {
                    WaitClose();
                    MessageBox.Show("No result in this method. It might have been pooled before!");
                    return;
                }
                BenMAPGrid benMapGridShow = null;

                if (bChart && lstallSelectValuationMethodAndValue != null)
                {

                    InitChartResult(lstallSelectValuationMethodAndValue, CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null ? CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID : CommonClass.GBenMAPGrid.GridDefinitionID);
                }

                if (lstallSelectValuationMethodAndValue != null)
                {
                    if (this.cbAPVAggregation.SelectedIndex != -1 && cbAPVAggregation.SelectedIndex != 0)
                    {
                        int idCbo = Convert.ToInt32((cbAPVAggregation.SelectedItem as DataRowView)["GridDefinitionID"]);
                        int idAggregation = -1;
                        GridRelationship gridRelationship = null;
                        if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null)
                        {
                            idAggregation = CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID;

                            if (CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idCbo && p.smallGridID == idAggregation).Count() > 0)
                            {
                                gridRelationship = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idCbo && p.smallGridID == idAggregation).First();
                            }
                            else
                            {
                                CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idAggregation && p.smallGridID == idCbo).First();

                            }
                        }
                        else
                        {
                            if (CommonClass.GBenMAPGrid.GridDefinitionID == idCbo)
                            {
                            }
                            else if (CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idCbo && p.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID).Count() > 0)
                            {
                                gridRelationship = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idCbo && p.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID).First();
                            }
                            else
                            {
                                gridRelationship = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == CommonClass.GBenMAPGrid.GridDefinitionID && p.smallGridID == idCbo).First();

                            }
                        }

                        if (idCbo != idAggregation)
                        {
                            List<AllSelectValuationMethodAndValue> lstTmp = new List<AllSelectValuationMethodAndValue>();
                            foreach (AllSelectValuationMethodAndValue asvm in lstallSelectValuationMethodAndValue)
                            {
                                lstTmp.Add(APVX.APVCommonClass.ApplyAllSelectValuationMethodAndValueAggregation(gridRelationship, idAggregation == -1 ? CommonClass.GBenMAPGrid : CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation, asvm));
                            }
                            lstallSelectValuationMethodAndValue = lstTmp;
                            benMapGridShow = Grid.GridCommon.getBenMAPGridFromID(idCbo);
                        }

                    }

                }

                if (bTable && lstallSelectValuationMethodAndValue != null)
                {
                    InitTableResult(lstallSelectValuationMethodAndValue);
                }

                if (bGIS)
                {
                    //set change projection text
                    string changeProjText = "change projection to setup projection";
                    if (!String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
                    {
                        changeProjText = changeProjText + " (" + CommonClass.MainSetup.SetupProjection + ")";
                    }
                    tsbChangeProjection.Text = changeProjText;

                    mainMap.ProjectionModeReproject = ActionMode.Never;
                    mainMap.ProjectionModeDefine = ActionMode.Never;
                    string shapeFileName = "";

                    MapGroup ResultsMG = AddMapGroup("Results", "Map Layers", false, false);
                    MapGroup PVResultsMG = AddMapGroup("Pooled Valuation", "Results", false, false);

                    string author = "Author Unknown";

                    if (lstallSelectValuationMethodAndValue.First().AllSelectValuationMethod != null
                        && lstallSelectValuationMethodAndValue.First().AllSelectValuationMethod.Author != null)
                    {
                        author = lstallSelectValuationMethodAndValue.First().AllSelectValuationMethod.Author;
                        if (author.IndexOf(" ") > -1)
                        {
                            author = author.Substring(0, author.IndexOf(" "));
                        }
                    }
                    string LayerNameText = "Pooled Valuation: " + author;

                    RemoveOldPolygonLayer(LayerNameText, PVResultsMG.Layers, false);

                    if (CommonClass.GBenMAPGrid is ShapefileGrid)
                    {
                        if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                        {
                            shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
                        }
                    }
                    else if (CommonClass.GBenMAPGrid is RegularGrid)
                    {
                        if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp"))
                        {
                            shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp";
                        }
                    }


                    //if (benMapGridShow != null)
                    //{
                    //    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + ((benMapGridShow is ShapefileGrid) ? (benMapGridShow as ShapefileGrid).ShapefileName : (benMapGridShow as RegularGrid).ShapefileName) + ".shp";
                    //}

                    // get unique beta variation count (such as number of seasons) and create layer for each
                    //int bvCount = CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction[0].BenMAPHealthImpactFunction.Variables[0].PollBetas.Count();
                    Dictionary<int, string> dicUniqueBV = new Dictionary<int, string>();
                    int bvUniqueCount = 0;
                    for (int ind = 0; ind < lstallSelectValuationMethodAndValue[0].lstAPVValueAttributes.Count(); ind++)
                    {
                        if (!dicUniqueBV.ContainsValue(lstallSelectValuationMethodAndValue[0].lstAPVValueAttributes[ind].BetaName))
                        {
                            dicUniqueBV.Add(bvUniqueCount++, lstallSelectValuationMethodAndValue[0].lstAPVValueAttributes[ind].BetaName);
                        }

                    }

                    for (int ind = 0; ind < bvUniqueCount; ind++)
                    {
                        MapPolygonLayer APVResultPolyLayer1 = (MapPolygonLayer)PVResultsMG.Layers.Add(shapeFileName);

                        DataTable dt = APVResultPolyLayer1.DataSet.DataTable;

                        int j = 0;
                        int iCol = 0;
                        int iRow = 0;
                        List<string> lstRemoveName = new List<string>();
                        while (j < dt.Columns.Count)
                        {
                            if (dt.Columns[j].ColumnName.ToLower() == "col") iCol = j;
                            if (dt.Columns[j].ColumnName.ToLower() == "row") iRow = j;

                            j++;
                        }
                        j = 0;

                        while (j < dt.Columns.Count)
                        {
                            if (dt.Columns[j].ColumnName.ToLower() == "col" || dt.Columns[j].ColumnName.ToLower() == "row")
                            { }
                            else
                                lstRemoveName.Add(dt.Columns[j].ColumnName);

                            j++;
                        }
                        foreach (string s in lstRemoveName)
                        {
                            dt.Columns.Remove(s);
                        }
                        string layerName = "Pooled Valuation" + (dicUniqueBV[ind] == "" ? "" : " (" + dicUniqueBV[ind] + ")");
                        dt.Columns.Add(layerName, typeof(double));
                        j = 0;
                        while (j < dt.Columns.Count)
                        {
                            if (dt.Columns[j].ColumnName.ToLower() == "col") iCol = j;
                            if (dt.Columns[j].ColumnName.ToLower() == "row") iRow = j;

                            j++;
                        }
                        j = 0;

                        Dictionary<string, double> dicAll = new Dictionary<string, double>();
                        foreach (APVValueAttribute crcv in lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes)
                        {
                            if (crcv.BetaName.Equals(dicUniqueBV[ind]))
                            {
                                if (dicAll.ContainsKey(crcv.Col + "," + crcv.Row))
                                {
                                    dicAll[crcv.Col + "," + crcv.Row] += crcv.PointEstimate;
                                }
                                else
                                {
                                    dicAll.Add(crcv.Col + "," + crcv.Row, crcv.PointEstimate);
                                }

                            }

                        }

                        foreach (DataRow dr in dt.Rows)
                        {
                            try
                            {
                                if (dicAll.ContainsKey(dr[iCol] + "," + dr[iRow]))
                                {
                                    dr[layerName] = Math.Round(dicAll[dr[iCol] + "," + dr[iRow]], 10);
                                }
                                else
                                {
                                    dr[layerName] = 0;
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        if (File.Exists(CommonClass.DataFilePath + @"\Tmp\APVTemp.shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Tmp\APVTemp.shp");
                        APVResultPolyLayer1.DataSet.SaveAs(CommonClass.DataFilePath + @"\Tmp\APVTemp.shp", true);
                        //mainMap.Layers.Clear();

                        //APVResultPolyLayer1 = (MapPolygonLayer)mainMap.Layers.Add(CommonClass.DataFilePath + @"\Tmp\APVTemp.shp");
                        //(mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).DataSet.DataTable.Columns[(mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).DataSet.DataTable.Columns.Count - 1].ColumnName = "Pooled Valuation";
                       // APVResultPolyLayer1.DataSet.DataTable.Columns[(APVResultPolyLayer1).DataSet.DataTable.Columns.Count - 1].ColumnName = "Pooled Valuation";
                        //string author = "Pooled Valuation";
                        //if (lstallSelectValuationMethodAndValue.First().AllSelectValuationMethod != null
                        //    && lstallSelectValuationMethodAndValue.First().AllSelectValuationMethod.Author != null)
                        //{
                        //    author = lstallSelectValuationMethodAndValue.First().AllSelectValuationMethod.Author;
                        //    if (author.IndexOf(" ") > -1)
                        //    {
                        //        author = author.Substring(0, author.IndexOf(" "));
                        //    }
                        //}   
                        //APVResultPolyLayer1.LegendText = "PV:"+ author;
                        MapPolygonLayer polLayer = APVResultPolyLayer1;
                        string strValueField = polLayer.DataSet.DataTable.Columns[polLayer.DataSet.DataTable.Columns.Count - 1].ColumnName;

                        _columnName = strValueField;
                        polLayer.Symbology = CreateResultPolyScheme(ref polLayer, 6, "A"); //-MCB added

                        double dMinValue = 0.0;
                        double dMaxValue = 0.0;
                        dMinValue = lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.Min(a => a.PointEstimate);
                        dMaxValue = lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.Max(a => a.PointEstimate);
                        _dMinValue = dMinValue;
                        _dMaxValue = dMaxValue;
                        APVResultPolyLayer1.Name = "Pooled Valuation: " + author;
                        APVResultPolyLayer1.LegendText = author;
                        //_currentLayerIndex = mainMap.Layers.Count - 1;
                        _CurrentIMapLayer = APVResultPolyLayer1;
                        _columnName = strValueField;
                        _CurrentMapTitle = CommonClass.MainSetup.SetupName + " Setup: Pooled Valuation- " + APVResultPolyLayer1.LegendText;
                        RenderMainMap(true, "A");
                    }
                    addRegionLayerGroupToMainMap();
                    int result = EnforceLegendOrder();
                }
               // WaitClose();
            }
            catch (Exception ex)
            { }
            finally
            {
                WaitClose();
            }
        }

        private void getAllChildMethodNotNone(AllSelectValuationMethod allSelectValueMethod, List<AllSelectValuationMethod> lstAll, ref List<AllSelectValuationMethod> lstReturn)
        {
            List<AllSelectValuationMethod> lstOne = lstAll.Where(p => p.PID == allSelectValueMethod.ID).ToList(); lstReturn.AddRange(lstOne.Where(p => p.PoolingMethod != "None" || p.NodeType == 3).ToList());
            foreach (AllSelectValuationMethod asvm in lstOne)
            {
                getAllChildMethodNotNone(asvm, lstAll, ref lstReturn);
            }
        }

        private void getAllChildQALYMethodNotNone(AllSelectQALYMethod allSelectQALYMethod, List<AllSelectQALYMethod> lstAll, ref List<AllSelectQALYMethod> lstReturn)
        {
            List<AllSelectQALYMethod> lstOne = lstAll.Where(p => p.PID == allSelectQALYMethod.ID).ToList(); lstReturn.AddRange(lstOne.Where(p => p.PoolingMethod != "None" || p.NodeType == 3).ToList());
            foreach (AllSelectQALYMethod asvm in lstOne)
            {
                getAllChildQALYMethodNotNone(asvm, lstAll, ref lstReturn);
            }
        }


        private int _pageCurrent;
        private int _currentRow;
        private int _pageSize;
        private int _pageCount;
        public bool _MapAlreadyDisplayed = false;
        public object _tableObject;
        private string getFieldNameFromlstHealth(string s)
        {
            string fieldName = "";

            switch (s)
            {
                case "Column":
                    fieldName = "Col";
                    break;
                case "Row":
                    fieldName = "Row";
                    break;
                case "Point Estimate":
                    fieldName = "PointEstimate";
                    break;
                case "Incidence":
                    fieldName = "Incidence";
                    break;
                case "Population":
                    fieldName = "Population";
                    break;
                case "Delta":
                    fieldName = "Delta";
                    break;
                case "Mean":
                    fieldName = "Mean";
                    break;
                case "Baseline":
                    fieldName = "Baseline";
                    break;
                case "Percent of Baseline":
                    fieldName = "PercentOfBaseline";
                    break;
                case "Standard Deviation":
                    fieldName = "StandardDeviation";
                    break;
                case "Variance":
                    fieldName = "Variance";
                    break;
                case "Dataset":
                    fieldName = "BenMAPHealthImpactFunction.DataSetName";
                    break;
                case "Endpoint Group":
                    fieldName = "BenMAPHealthImpactFunction.EndPointGroup";
                    break;
                case "Endpoint":
                    fieldName = "BenMAPHealthImpactFunction.EndPoint";
                    break;
                case "Pollutant":
                    fieldName = "BenMAPHealthImpactFunction.PollutantGroup.PollutantGroupName";
                    break;
                case "Metric":
                    fieldName = "BenMAPHealthImpactFunction.Metric.MetricName";
                    break;
                case "Seasonal Metric":
                    fieldName = "BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName";
                    break;
                case "Metric Statistic":
                    fieldName = "BenMAPHealthImpactFunction.MetricStatistic";
                    break;
                case "Author":
                    fieldName = "BenMAPHealthImpactFunction.Author";
                    break;
                case "Year":
                    fieldName = "BenMAPHealthImpactFunction.Year";
                    break;
                case "Study Location":
                    fieldName = "BenMAPHealthImpactFunction.strLocations";
                    break;
                case "Geographic Area":
                    //fieldName = "BenMAPHealthImpactFunction.GeographicAreaName";
                    fieldName = "GeographicAreaName";
                    break;
                case "Other Pollutants":
                    fieldName = "BenMAPHealthImpactFunction.OtherPollutants";
                    break;
                case "Qualifier":
                    fieldName = "BenMAPHealthImpactFunction.Qualifier";
                    break;
                case "Reference":
                    fieldName = "BenMAPHealthImpactFunction.Reference";
                    break;
                case "Race":
                    fieldName = "Race";
                    break;
                case "Ethnicity":
                    fieldName = "Ethnicity";
                    break;
                case "Gender":
                    fieldName = "Gender";
                    break;
                case "Start Age":
                    fieldName = "StartAge";
                    break;
                case "End Age":
                    fieldName = "EndAge";
                    break;
                case "Function":
                    fieldName = "BenMAPHealthImpactFunction.Function";
                    break;
                case "Incidence Dataset":
                    fieldName = "IncidenceDataSetName";
                    break;
                case "Prevalence Dataset":
                    fieldName = "PrevalenceDataSetName";
                    break;
                case "Beta":
                    fieldName = "BenMAPHealthImpactFunction.Beta";
                    break;
                case "Beta Distribution":
                    fieldName = "BenMAPHealthImpactFunction.BetaDistribution";
                    break;
                case "Beta Parameter 1":
                    fieldName = "BenMAPHealthImpactFunction.BetaParameter1";
                    break;
                case "Beta Parameter 2":
                    fieldName = "BenMAPHealthImpactFunction.BetaParameter2";
                    break;
                case "A":
                    fieldName = "BenMAPHealthImpactFunction.AContantValue";
                    break;
                case "A Description":
                    fieldName = "BenMAPHealthImpactFunction.AContantDescription";
                    break;
                case "B":
                    fieldName = "BenMAPHealthImpactFunction.BContantValue";
                    break;
                case "B Description":
                    fieldName = "BenMAPHealthImpactFunction.BContantDescription";
                    break;
                case "C":
                    fieldName = "BenMAPHealthImpactFunction.CContantValue";
                    break;
                case "C Description":
                    fieldName = "BenMAPHealthImpactFunction.CContantDescription";
                    break;
                case "Beta Variation":
                    fieldName = "BenMAPHealthImpactFunction.BetaVariation.BetaVariationName";
                    break;
                case "Model Specification":
                    fieldName = "BenMAPHealthImpactFunction.ModelSpecification.MSDescription";
                    break;

            }
            return fieldName;
        }

        private object getFieldNameFromlstHealthObject(string s, CRCalculateValue crv, CRSelectFunction crf)
        {
            object fieldName = "";

            switch (s)
            {
                case "Column":
                    fieldName = crv.Col;
                    break;
                case "Row":
                    fieldName = crv.Row;
                    break;
                case "Point Estimate":
                    fieldName = crv.PointEstimate;
                    break;
                case "Incidence":
                    fieldName = crv.Incidence;
                    break;
                case "Population":
                    fieldName = crv.Population;
                    break;
                case "Delta":
                    fieldName = crv.Delta;
                    break;
                case "Mean":
                    fieldName = crv.Mean;
                    break;
                case "Baseline":
                    fieldName = crv.Baseline;
                    break;
                case "Percent of Baseline":
                    fieldName = crv.PercentOfBaseline;
                    break;
                case "Standard Deviation":
                    fieldName = crv.StandardDeviation;
                    break;
                case "Variance":
                    fieldName = crv.Variance;
                    break;
                case "Dataset":
                    fieldName = crf.BenMAPHealthImpactFunction.DataSetName;
                    break;
                case "Endpoint Group":
                    fieldName = crf.BenMAPHealthImpactFunction.EndPointGroup == null ? "" : crf.BenMAPHealthImpactFunction.EndPointGroup.Replace(",", " ");
                    break;
                case "Endpoint":
                    fieldName = crf.BenMAPHealthImpactFunction.EndPoint == null ? "" : crf.BenMAPHealthImpactFunction.EndPoint.Replace(",", " ");
                    break;
                case "Pollutant":
                    fieldName = crf.BenMAPHealthImpactFunction.PollutantGroup.PollutantGroupName;
                    break;
                case "Metric":
                    fieldName = crf.BenMAPHealthImpactFunction.Metric.MetricName;
                    break;
                case "Seasonal Metric":
                    fieldName = crf.BenMAPHealthImpactFunction.SeasonalMetric == null ? "" : crf.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName;
                    break;
                case "Metric Statistic":
                    fieldName = crf.BenMAPHealthImpactFunction.MetricStatistic;
                    break;
                case "Author":
                    fieldName = crf.BenMAPHealthImpactFunction.Author == null ? "" : crf.BenMAPHealthImpactFunction.Author.Replace(",", " ");
                    break;
                case "Year":
                    fieldName = crf.BenMAPHealthImpactFunction.Year;
                    break;
                case "Study Location":
                    fieldName = crf.BenMAPHealthImpactFunction.strLocations;
                    break;
                case "Geographic Area":
                    fieldName = crf.GeographicAreaName;
                    break;
                case "Other Pollutants":
                    fieldName = crf.BenMAPHealthImpactFunction.OtherPollutants == null ? "" : crf.BenMAPHealthImpactFunction.OtherPollutants.Replace(",", " ");
                    break;
                case "Qualifier":
                    fieldName = crf.BenMAPHealthImpactFunction.Qualifier == null ? "" : crf.BenMAPHealthImpactFunction.Qualifier.Replace(",", " ");
                    break;
                case "Reference":
                    fieldName = crf.BenMAPHealthImpactFunction.Reference == null ? "" : crf.BenMAPHealthImpactFunction.Reference.Replace(",", " ");
                    break;
                case "Race":
                    fieldName = crf.Race;
                    break;
                case "Ethnicity":
                    fieldName = crf.Ethnicity;
                    break;
                case "Gender":
                    fieldName = crf.Gender;
                    break;
                case "Start Age":
                    fieldName = crf.StartAge;
                    break;
                case "End Age":
                    fieldName = crf.EndAge;
                    break;
                case "Function":
                    fieldName = crf.BenMAPHealthImpactFunction.Function;
                    break;
                case "Incidence Dataset":
                    fieldName = crf.IncidenceDataSetName;
                    break;
                case "Prevalence Dataset":
                    fieldName = crf.PrevalenceDataSetName;
                    break;
                case "Beta":
                    fieldName = crf.BenMAPHealthImpactFunction.Beta;
                    break;
                case "Beta Distribution":
                    fieldName = crf.BenMAPHealthImpactFunction.BetaDistribution;
                    break;
                case "Beta Parameter 1":
                    fieldName = crf.BenMAPHealthImpactFunction.BetaParameter1;
                    break;
                case "Beta Parameter 2":
                    fieldName = crf.BenMAPHealthImpactFunction.BetaParameter2;
                    break;
                case "A":
                    fieldName = crf.BenMAPHealthImpactFunction.AContantValue;
                    break;
                case "A Description":
                    fieldName = crf.BenMAPHealthImpactFunction.AContantDescription;
                    break;
                case "B":
                    fieldName = crf.BenMAPHealthImpactFunction.BContantValue;
                    break;
                case "B Description":
                    fieldName = crf.BenMAPHealthImpactFunction.BContantDescription;
                    break;
                case "C":
                    fieldName = crf.BenMAPHealthImpactFunction.CContantValue;
                    break;
                case "C Description":
                    fieldName = crf.BenMAPHealthImpactFunction.CContantDescription;
                    break;
                case "Beta Variation":
                    fieldName = crf.BenMAPHealthImpactFunction.BetaVariation.BetaVariationName;
                    break;
                case "Model Specification":
                    fieldName = crf.BenMAPHealthImpactFunction.ModelSpecification.MSDescription;
                    break;


            }
            return fieldName;
        }
        private string getFieldNameFromlstAPV(string s)
        {
            string fieldName = s;

            switch (s)
            {
                case "Column":
                    fieldName = "Col";
                    break;
                case "Row":
                    fieldName = "Row";
                    break;
                case "Point Estimate":
                    fieldName = "PointEstimate";
                    break;
                case "Population":
                    fieldName = "Population";
                    break;
                case "Delta":
                    fieldName = "Delta";
                    break;
                case "Mean":
                    fieldName = "Mean";
                    break;
                case "Baseline":
                    fieldName = "Baseline";
                    break;
                case "Percent of Baseline":
                    fieldName = "PercentOfBaseline";
                    break;
                case "Standard Deviation":
                    fieldName = "StandardDeviation";
                    break;
                case "Variance":
                    fieldName = "Variance";
                    break;
                case "Dataset":
                    fieldName = "DataSet";
                    break;
                case "Endpoint Group":
                    fieldName = "EndPointGroup";
                    break;
                case "Endpoint":
                    fieldName = "EndPoint";
                    break;
                case "Pollutant":
                    fieldName = "Pollutant";
                    break;
                case "Metric":
                    fieldName = "Metric";
                    break;
                case "Seasonal Metric":
                    fieldName = "SeasonalMetric";
                    break;
                case "Metric Statistic":
                    fieldName = "MetricStatistic";
                    break;
                case "Author":
                    fieldName = "Author";
                    break;
                case "Year":
                    fieldName = "Year";
                    break;
                case "Study Location":
                    fieldName = "Location";
                    break;
                case "Geographic Area":
                    fieldName = "GeographicArea";
                    break;
                case "Other Pollutants":
                    fieldName = "OtherPollutants";
                    break;
                case "Qualifier":
                    fieldName = "Qualifier";
                    break;
                case "Reference":
                    fieldName = "Reference";
                    break;
                case "Race":
                    fieldName = "Race";
                    break;
                case "Ethnicity":
                    fieldName = "Ethnicity";
                    break;
                case "Gender":
                    fieldName = "Gender";
                    break;
                case "Start Age":
                    fieldName = "StartAge";
                    break;
                case "End Age":
                    fieldName = "EndAge";
                    break;
                case "Function":
                    fieldName = "Function";
                    break;
                case "Incidence Dataset":
                    fieldName = "IncidenceDataSetID";
                    break;
                case "Prevalence Dataset":
                    fieldName = "PrevalenceDataSetID";
                    break;
                case "Version":
                    fieldName = "Version";
                    break;
            }
            return fieldName;
        }
        private object getFieldNameFromlstAPVObject(string s, AllSelectValuationMethod allSelectValuationMethod, APVValueAttribute apvValueAttribute)
        {
            object fieldName = "";

            switch (s)
            {
                case "Column":
                    fieldName = apvValueAttribute.Col;
                    break;
                case "Row":
                    fieldName = apvValueAttribute.Row;
                    break;
                case "Point Estimate":
                    fieldName = apvValueAttribute.PointEstimate;
                    break;

                case "Mean":
                    fieldName = apvValueAttribute.Mean;
                    break;

                case "Standard Deviation":
                    fieldName = apvValueAttribute.StandardDeviation;
                    break;
                case "Variance":
                    fieldName = apvValueAttribute.Variance;
                    break;
                case "Name":
                    fieldName = allSelectValuationMethod.Name;
                    break;
                case "Dataset":
                    fieldName = allSelectValuationMethod.DataSet;
                    break;
                case "Endpoint Group":
                    fieldName = allSelectValuationMethod.EndPointGroup;
                    break;
                case "Endpoint":
                    fieldName = allSelectValuationMethod.EndPoint;
                    break;
                case "Pollutant":
                    fieldName = allSelectValuationMethod.PollutantGroup;
                    break;
                case "Metric":
                    fieldName = allSelectValuationMethod.Metric;
                    break;
                case "Seasonal Metric":
                    fieldName = allSelectValuationMethod.SeasonalMetric;
                    break;
                case "Metric Statistic":
                    fieldName = allSelectValuationMethod.MetricStatistic;
                    break;
                case "Author":
                    fieldName = allSelectValuationMethod.Author;
                    break;
                case "Year":
                    fieldName = allSelectValuationMethod.Year;
                    break;
                case "Study Location":
                    fieldName = allSelectValuationMethod.Location;
                    break;
                case "Geographic Area":
                    fieldName = allSelectValuationMethod.GeographicArea;
                    break;
                case "Other Pollutants":
                    fieldName = allSelectValuationMethod.OtherPollutants;
                    break;
                case "Qualifier":
                    fieldName = allSelectValuationMethod.Qualifier;
                    break;

                case "Race":
                    fieldName = allSelectValuationMethod.Race;
                    break;
                case "Ethnicity":
                    fieldName = allSelectValuationMethod.Ethnicity;
                    break;
                case "Gender":
                    fieldName = allSelectValuationMethod.Gender;
                    break;
                case "Start Age":
                    fieldName = allSelectValuationMethod.StartAge;
                    break;
                case "End Age":
                    fieldName = allSelectValuationMethod.EndAge;
                    break;
                case "Function":
                    fieldName = allSelectValuationMethod.Function;
                    break;

                case "Version":
                    fieldName = allSelectValuationMethod.Version;
                    break;
            }
            return fieldName;
        }
        private object getFieldNameFromlstQALYObject(string s, AllSelectQALYMethod allSelectQALYMethod, QALYValueAttribute qalyValueAttribute)
        {
            object fieldName = "";

            switch (s)
            {
                case "Column":
                    fieldName = qalyValueAttribute.Col;
                    break;
                case "Row":
                    fieldName = qalyValueAttribute.Row;
                    break;
                case "Point Estimate":
                    fieldName = qalyValueAttribute.PointEstimate;
                    break;

                case "Mean":
                    fieldName = qalyValueAttribute.Mean;
                    break;

                case "Standard Deviation":
                    fieldName = qalyValueAttribute.StandardDeviation;
                    break;
                case "Variance":
                    fieldName = qalyValueAttribute.Variance;
                    break;
                case "Dataset":
                    fieldName = allSelectQALYMethod.DataSet;
                    break;
                case "Endpoint Group":
                    fieldName = allSelectQALYMethod.EndPointGroup;
                    break;
                case "Endpoint":
                    fieldName = allSelectQALYMethod.EndPoint;
                    break;
                case "Pollutant":
                    fieldName = allSelectQALYMethod.Pollutant;
                    break;
                case "Metric":
                    fieldName = allSelectQALYMethod.Metric;
                    break;
                case "Seasonal Metric":
                    fieldName = allSelectQALYMethod.SeasonalMetric;
                    break;
                case "Metric Statistic":
                    fieldName = allSelectQALYMethod.MetricStatistic;
                    break;
                case "Author":
                    fieldName = allSelectQALYMethod.Author;
                    break;
                case "Year":
                    fieldName = allSelectQALYMethod.Year;
                    break;
                case "Location":
                    fieldName = allSelectQALYMethod.Location;
                    break;
                case "Other Pollutants":
                    fieldName = allSelectQALYMethod.OtherPollutants;
                    break;
                case "Qualifier":
                    fieldName = allSelectQALYMethod.Qualifier;
                    break;

                case "Race":
                    fieldName = allSelectQALYMethod.Race;
                    break;
                case "Ethnicity":
                    fieldName = allSelectQALYMethod.Ethnicity;
                    break;
                case "Gender":
                    fieldName = allSelectQALYMethod.Gender;
                    break;
                case "Start Age":
                    fieldName = allSelectQALYMethod.StartAge;
                    break;
                case "End Age":
                    fieldName = allSelectQALYMethod.EndAge;
                    break;
                case "Function":
                    fieldName = allSelectQALYMethod.Function;
                    break;

                case "Version":
                    fieldName = allSelectQALYMethod.Version;
                    break;
            }
            return fieldName;
        }
        private void InitColumnsShowSet()
        {
            if (IncidencelstHealth == null)
            {
                IncidencelstHealth = new List<FieldCheck>();
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Dataset", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Endpoint Group", isChecked = true });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Endpoint", isChecked = true });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Pollutant", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Metric", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Seasonal Metric", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Metric Statistic", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Author", isChecked = true });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Year", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Study Location", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Geographic Area", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Other Pollutants", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Qualifier", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Reference", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Race", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Ethnicity", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Gender", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Start Age", isChecked = true });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "End Age", isChecked = true });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Function", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Incidence Dataset", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Prevalence Dataset", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Beta", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Beta Distribution", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Beta Parameter 1", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Beta Parameter 2", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "A", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "A Description", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "B", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "B Description", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "C", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "C Description", isChecked = false });
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Version", isChecked = true });
            }
            if (cflstHealth == null)
            {
                cflstHealth = new List<FieldCheck>();
                cflstHealth.Add(new FieldCheck() { FieldName = "Dataset", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Endpoint Group", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Endpoint", isChecked = true });
                cflstHealth.Add(new FieldCheck() { FieldName = "Pollutant", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Metric", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Seasonal Metric", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Metric Statistic", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Author", isChecked = true });
                cflstHealth.Add(new FieldCheck() { FieldName = "Year", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Study Location", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Geographic Area", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Other Pollutants", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Qualifier", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Reference", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Race", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Ethnicity", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Gender", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Start Age", isChecked = true });
                cflstHealth.Add(new FieldCheck() { FieldName = "End Age", isChecked = true });
                cflstHealth.Add(new FieldCheck() { FieldName = "Function", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Incidence Dataset", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Prevalence Dataset", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Beta", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Beta Distribution", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Beta Parameter 1", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Beta Parameter 2", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "A", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "A Description", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "B", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "B Description", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "C", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "C Description", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Beta Variation", isChecked = false });
                cflstHealth.Add(new FieldCheck() { FieldName = "Model Specification", isChecked = false });
            }
            if (apvlstHealth == null)
            {
                apvlstHealth = new List<FieldCheck>();
                apvlstHealth.Add(new FieldCheck() { FieldName = "Name", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Dataset", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Endpoint Group", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Endpoint", isChecked = true });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Pollutant", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Metric", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Seasonal Metric", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Metric Statistic", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Author", isChecked = true });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Year", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Study Location", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Geographic Area", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Other Pollutants", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Qualifier", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Race", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Ethnicity", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Gender", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Start Age", isChecked = true });
                apvlstHealth.Add(new FieldCheck() { FieldName = "End Age", isChecked = true });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Function", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Version", isChecked = true });

            }
            if (qalylstHealth == null)
            {
                qalylstHealth = new List<FieldCheck>();
                qalylstHealth.Add(new FieldCheck() { FieldName = "Dataset", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Endpoint Group", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Endpoint", isChecked = true });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Pollutant", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Metric", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Seasonal Metric", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Metric Statistic", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Author", isChecked = true });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Year", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Study Location", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Other Pollutants", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Qualifier", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Race", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Ethnicity", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Gender", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Start Age", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "End Age", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Function", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Version", isChecked = true });
            }
        }

        private bool isSumChecked = false;
        private void chkSumAcrossYear_CheckedChanged(object sender, EventArgs e)
        {
            isSumChecked = chkSumAcrossYear.Checked;
            btShowCRResult_Click(sender, e);
        }

        private void InitTableResult(object oTable)
        {
            try
            {
                numericUpDownResult.ValueChanged -= numericUpDownResult_ValueChanged;
                numericUpDownResult.Value = 4;
                numericUpDownResult.ValueChanged += numericUpDownResult_ValueChanged;

                OLVResultsShow.Items.Clear();
                OLVResultsShow.Columns.Clear();
                int i = 0;
                Boolean forceShowGeographicArea = false;

                if (oTable is Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>)
                {
                    //Option 1
                    List<CRSelectFunctionCalculateValue> lstCRSelectFunctionCalculateValue = new List<CRSelectFunctionCalculateValue>();
                    if (this.tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Tag.ToString() == "incidence")
                    {
                        foreach (KeyValuePair<AllSelectCRFunction, string> keyValueCR in olvIncidence.SelectedObjects)
                        {
                            AllSelectCRFunction cr = keyValueCR.Key;

                            if (cr.CRSelectFunctionCalculateValue == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues.Count == 0)
                            {
                                continue;
                            }
                            else
                            {

                                lstCRSelectFunctionCalculateValue.Add(cr.CRSelectFunctionCalculateValue);
                            }

                        }
                    }
                    else
                    {
                        foreach (CRSelectFunctionCalculateValue cr in olvCRFunctionResult.SelectedObjects)
                        {
                            lstCRSelectFunctionCalculateValue.Add(cr);
                        }
                        if (cbCRAggregation.SelectedIndex != -1 && cbCRAggregation.SelectedIndex != 0)
                        {
                            DataRowView drv = cbCRAggregation.SelectedItem as DataRowView;
                            int iAggregationGridType = Convert.ToInt32(drv["GridDefinitionID"]);
                            if (iAggregationGridType != CommonClass.GBenMAPGrid.GridDefinitionID)
                            {
                                List<CRSelectFunctionCalculateValue> lstTemp = new List<CRSelectFunctionCalculateValue>();
                                foreach (CRSelectFunctionCalculateValue cr in lstCRSelectFunctionCalculateValue)
                                {
                                    lstTemp.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(cr, CommonClass.GBenMAPGrid.GridDefinitionID, iAggregationGridType));
                                }
                                lstCRSelectFunctionCalculateValue = lstTemp;
                            }

                        }
                    }
                    oTable = lstCRSelectFunctionCalculateValue;
                }
                if (oTable is List<AllSelectCRFunction>)
                {
                    // Option 2 - Pooled Incidence Results
                    List<AllSelectCRFunction> lstAllSelectCRFuntion = (List<AllSelectCRFunction>)oTable;
                    foreach (AllSelectCRFunction cf in lstAllSelectCRFuntion)
                    {
                        if (string.IsNullOrWhiteSpace(cf.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.GeographicAreaName) == false &&
                            cf.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.GeographicAreaName.Equals(Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_EVERYWHERE) == false)
                        {
                            forceShowGeographicArea = true;
                        }
                    }
                    if (this.IncidencelstColumnRow == null)
                    {
                        BrightIdeasSoftware.OLVColumn olvColumnCol = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Col", Text = "Column", IsEditable = false, Width = 8 * 8 }; OLVResultsShow.Columns.Add(olvColumnCol);
                        BrightIdeasSoftware.OLVColumn olvColumnRow = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Row", Text = "Row", IsEditable = false, Width = 6 * 8 }; OLVResultsShow.Columns.Add(olvColumnRow);
                    }
                    else
                    {
                        foreach (FieldCheck fieldCheck in IncidencelstColumnRow)
                        {

                            if (fieldCheck.isChecked)
                            {
                                BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                            }
                        }
                    }

                    if (IncidencelstHealth != null)
                    {
                        foreach (FieldCheck fieldCheck in IncidencelstHealth)
                        {
                            if (fieldCheck.FieldName.Equals("Geographic Area") && forceShowGeographicArea)
                            {
                                fieldCheck.isChecked = true;
                            }

                            if (fieldCheck.FieldName.ToLower() == "version" && fieldCheck.isChecked)
                            {
                                BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value.Version", Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);

                            }
                            else if (fieldCheck.FieldName.Equals("Geographic Area") && fieldCheck.isChecked)
                            {
                                BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                            }
                            else if (fieldCheck.isChecked)
                            {
                                BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value.CRSelectFunctionCalculateValue.CRSelectFunction." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                            }
                        }
                    }
                    if (IncidencelstResult == null)
                    {
                        Boolean hasSeasonal = true;
                        /*
                         * Commented out until we can implement a good way to determine if the pooled results involve seasons here
                         * // lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().BetaVariationName != null
                        foreach (AllSelectCRFunction o in lstAllSelectCRFuntion)
                        {
                            if (o.CRSelectFunction.BenMAPHealthImpactFunction.BetaVariation != null && o.CRSelectFunction.BenMAPHealthImpactFunction.BetaVariation.BetaVariationID == 2) //Seasonal
                            {
                                hasSeasonal = true;
                                break;
                            }
                        }
                        */
                        if (hasSeasonal)
                        {
                            string bvType = "Seasonal";
                            OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.BetaName", AspectToStringFormat = "{0:N4}", Text = bvType, Width = bvType.Length * 8, IsEditable = false });
                        }


                        OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PointEstimate", AspectToStringFormat = "{0:N4}", Width = "Point Estimate".Length * 8, Text = "Point Estimate", IsEditable = false });
                        OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Population", AspectToStringFormat = "{0:N4}", Text = "Population", Width = "Population".Length * 8, IsEditable = false });

                        // set up delta columns dynamically for multipollutant 
                        if (lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().DeltaList != null)
                        {
                            int dCount = lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().DeltaList.Count();
                            for (int j = 0; j < dCount; j++)
                            {
                                string pollName = lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Variables[j].PollutantName;
                                int pollID = ConfigurationCommonClass.getPollutantIDFromPollutantNameAndObject(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRSelectFunction, pollName);
                                
                                OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.DeltaList[" + j + "]", AspectToStringFormat = "{0:N4}", Text = "Delta_" + pollName, Width = ("Delta_" + pollName).Length * 8, IsEditable = false });
                            }
                        }
                        else
                        {
                            OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Delta", AspectToStringFormat = "{0:N4}", Text = "Delta", Width = "Variance".Length * 8, IsEditable = false });
                        }

                        OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Mean", AspectToStringFormat = "{0:N4}", Text = "Mean", Width = "Variance".Length * 8, IsEditable = false }); 
                        OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Baseline", AspectToStringFormat = "{0:N4}", Text = "Baseline", Width = "Baseline2".Length * 8, IsEditable = false }); 
                        OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PercentOfBaseline", AspectToStringFormat = "{0:N4}", Width = "Percent Of Baseline".Length * 8, Text = "Percent Of Baseline", IsEditable = false }); 
                        OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.StandardDeviation", AspectToStringFormat = "{0:N4}", Width = "Standard Deviation".Length * 8, Text = "Standard Deviation", IsEditable = false }); 
                        OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Variance", Text = "Variance", AspectToStringFormat = "{0:N4}", Width = "Variance".Length * 8, IsEditable = false }); 
                    }
                    else
                    {
                        foreach (FieldCheck fieldCheck in IncidencelstResult)
                        {

                            if (fieldCheck.isChecked && fieldCheck.FieldName != IncidencelstResult.Last().FieldName)
                            {
                                BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), AspectToStringFormat = "{0:N4}", Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                            }
                        }
                    }
                    if (lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile != null)
                    {
                        if (strPoolIncidencePercentiles != null && strPoolIncidencePercentiles.Count > 0)
                        {
                            double interval = 50.0 / CommonClass.CRLatinHypercubePoints;
                            i = 0;
                            while (i < strPoolIncidencePercentiles.Count)
                            {
                                BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + (int)(Convert.ToDouble(strPoolIncidencePercentiles[i]) / interval - 1) / 2 + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + strPoolIncidencePercentiles[i].ToString(), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
                                i++;
                            }
                        }
                        else
                        {
                            i = 0;
                            while (i < lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count())
                            {
                                if (IncidencelstResult == null || IncidencelstResult.Last().isChecked)
                                {
                                    BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + i + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()))))), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
                                }
                                i++;
                            }
                        }
                    }
                    Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction> dicAPV = new Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>(); int iLstCRTable = 0;
                    Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();

                    Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction> dicAPV_Sum = new Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>();

                    foreach (AllSelectCRFunction cr in lstAllSelectCRFuntion)
                    {
                        foreach (CRCalculateValue crv in cr.CRSelectFunctionCalculateValue.CRCalculateValues)
                        {
                            dicKey = null;
                            dicKey = new Dictionary<CRCalculateValue, int>();
                            dicKey.Add(crv, iLstCRTable);

                            dicAPV.Add(dicKey.ToList()[0], cr);
                            dicAPV_Sum.Add(ConfigurationCommonClass.getKeyValuePairDeepCopy(dicKey.ToList()[0]), cr);
                        }
                        iLstCRTable++;
                    }
                    /*
                    if (isSumChecked && dicAPV.Keys.First().Key.BetaVariationName.ToLower() != "full year")
                    {
                        int c = 0;
                        // Use number of beta variations to separate results per grid cell
                        // TODO: Fix This
                        int numBetas = lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Variables.First().PollBetas.Count;
                        Dictionary<string, List<int>> betaNamesForSum = new Dictionary<string, List<int>>();
                        Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction> dicTempResults = new Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>();

                        foreach (KeyValuePair<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction> kvp in dicAPV_Sum)
                        {
                            // Add other results from grid cell
                            if (c < numBetas - 1)
                            {
                                if (betaNamesForSum.ContainsKey(kvp.Key.Key.BetaName))
                                {
                                    betaNamesForSum[kvp.Key.Key.BetaName].Add(c);
                                }
                                else
                                {
                                    List<int> newList = new List<int>();
                                    newList.Add(c);
                                    betaNamesForSum.Add(kvp.Key.Key.BetaName, newList);
                                }
                            }
                            // Add last result for grid cell then sum across
                            else if (c == numBetas - 1)
                            {
                                // Add last function 
                                if (betaNamesForSum.ContainsKey(kvp.Key.Key.BetaName))
                                {
                                    betaNamesForSum[kvp.Key.Key.BetaName].Add(c);
                                }
                                else
                                {
                                    List<int> newList = new List<int>();
                                    newList.Add(c);
                                    betaNamesForSum.Add(kvp.Key.Key.BetaName, newList);
                                }

                                foreach (KeyValuePair<string, List<int>> p in betaNamesForSum)
                                {
                                    if (p.Value.Count > 1)
                                    {
                                        // Get first result to add others into 
                                        CRCalculateValue summed = new CRCalculateValue();
                                        summed = ConfigurationCommonClass.getKeyValuePairDeepCopy(dicAPV_Sum.ElementAt(p.Value.First()).Key).Key;

                                        // For each result i nthe grid cell with that beta name 
                                        int ind = 0;
                                        CRCalculateValue toAdd = new CRCalculateValue();
                                        foreach (int v in p.Value)
                                        {
                                            if (ind > 0)
                                            {
                                                toAdd = ConfigurationCommonClass.getKeyValuePairDeepCopy(dicAPV_Sum.ElementAt(v).Key).Key;
                                                summed.PointEstimate += toAdd.PointEstimate;

                                                int percentileIndex = 0;
                                                foreach (float percentile in toAdd.LstPercentile)
                                                {
                                                    summed.LstPercentile[percentileIndex] += percentile;
                                                    percentileIndex++;
                                                }
                                            }
                                            ind++;
                                        }

                                        KeyValuePair<CRCalculateValue, int> newPair = new KeyValuePair<CRCalculateValue, int>(summed, dicAPV_Sum.ElementAt(p.Value.First()).Key.Value);
                                        dicTempResults.Add(newPair, dicAPV_Sum.ElementAt(p.Value.First()).Value);
                                    }
                                    else
                                    {
                                        CRCalculateValue deepCopy = ConfigurationCommonClass.getKeyValuePairDeepCopy(dicAPV_Sum.ElementAt(p.Value.First()).Key).Key;
                                        KeyValuePair<CRCalculateValue, int> newPair = new KeyValuePair<CRCalculateValue, int>(deepCopy, dicAPV_Sum.ElementAt(p.Value.First()).Key.Value);
                                        dicTempResults.Add(newPair, dicAPV_Sum.ElementAt(p.Value.First()).Value);
                                    }
                                }

                                // Reset for next grid cell
                                c = -1;
                                betaNamesForSum.Clear();
                            }
                            else { }
                            c++;
                        }

                        dicAPV_Sum.Clear();
                        dicAPV_Sum = dicTempResults;

                        _tableObject = lstAllSelectCRFuntion;
                        OLVResultsShow.SetObjects(dicAPV_Sum.ToList().GetRange(0, dicAPV_Sum.Count > 50 ? 50 : dicAPV_Sum.Count));
                        _pageSize = 50;
                        _currentRow = 0;
                        _pageCount = dicAPV_Sum.Count / 50 + 1; _pageCurrent = 1;
                        bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
                        bindingNavigatorCountItem.Text = _pageCount.ToString();
                    }

                    else
                    {
                        */
                    _tableObject = lstAllSelectCRFuntion;
                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(0, dicAPV.Count > 50 ? 50 : dicAPV.Count));
                    _pageSize = 50;
                    _currentRow = 0;
                    _pageCount = dicAPV.Count / 50 + 1; _pageCurrent = 1;
                    bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
                    bindingNavigatorCountItem.Text = _pageCount.ToString();
                    //}
                }
                // The user is showing HIF estimates (either raw or pooled)
                if (oTable is List<CRSelectFunctionCalculateValue> || oTable is CRSelectFunctionCalculateValue)
                {
                    // Option 3
                    List<CRSelectFunctionCalculateValue> lstCRTable = new List<CRSelectFunctionCalculateValue>();
                    if (oTable is List<CRSelectFunctionCalculateValue>)
                        lstCRTable = (List<CRSelectFunctionCalculateValue>)oTable;
                    else
                        lstCRTable.Add(oTable as CRSelectFunctionCalculateValue);


                    for (int iCR = 0; iCR < lstCRTable.Count; iCR++)
                    {
                        CRSelectFunctionCalculateValue cr = lstCRTable[iCR];
                        cr.CRCalculateValues = cr.CRCalculateValues.Where(p => p != null).OrderBy(p => p.Col).ToList();
                        if (string.IsNullOrWhiteSpace(cr.CRSelectFunction.GeographicAreaName) == false && cr.CRSelectFunction.GeographicAreaName.Equals(Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_EVERYWHERE) == false)
                        {
                            forceShowGeographicArea = true;
                        }
                    }
                    // Is this pooled incidence?
                    if (this.tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Tag.ToString() == "incidence")
                    {
                        if (this.IncidencelstColumnRow == null)
                        {
                            BrightIdeasSoftware.OLVColumn olvColumnCol = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Col", Text = "Column", IsEditable = false, Width = 8 * 8 }; OLVResultsShow.Columns.Add(olvColumnCol);
                            BrightIdeasSoftware.OLVColumn olvColumnRow = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Row", Text = "Row", IsEditable = false, Width = 6 * 8 }; OLVResultsShow.Columns.Add(olvColumnRow);
                        }
                        else
                        {
                            foreach (FieldCheck fieldCheck in IncidencelstColumnRow)
                            {

                                if (fieldCheck.isChecked)
                                {
                                    BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                }
                            }
                        }

                        if (IncidencelstHealth != null)
                        {
                            foreach (FieldCheck fieldCheck in IncidencelstHealth)
                            {

                                if (fieldCheck.isChecked)
                                {
                                    BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                }
                            }
                        }
                        if (IncidencelstResult == null)
                        {
                            if (lstCRTable.First().CRCalculateValues.First().BetaVariationName != null)
                            {
                                string bvType = lstCRTable.First().CRCalculateValues.First().BetaVariationName;
                                OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.BetaName", AspectToStringFormat = "{0:N4}", Text = bvType, Width = bvType.Length * 8, IsEditable = false });
                            }
                            OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PointEstimate", AspectToStringFormat = "{0:N4}", Width = "PointEstimate".Length * 8, Text = "Point Estimate", IsEditable = false });
                            OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Population", AspectToStringFormat = "{0:N4}", Text = "Population", Width = "Population".Length * 8, IsEditable = false });

                            // set up delta columns dynamically for multipollutant 
                            if (lstCRTable.First().CRCalculateValues.First().DeltaList != null)
                            {
                                int dCount = lstCRTable.First().CRCalculateValues.First().DeltaList.Count();
                                for (int j = 0; j < dCount; j++)
                                {
                                    string pollName = lstCRTable.First().CRSelectFunction.BenMAPHealthImpactFunction.Variables[j].PollutantName;
                                    int pollID = ConfigurationCommonClass.getPollutantIDFromPollutantNameAndObject(lstCRTable.First().CRSelectFunction, pollName);

                                    OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.DeltaList[" + j + "]", AspectToStringFormat = "{0:N4}", Text = "Delta_" + pollName, Width = ("Delta_" + pollName).Length * 8, IsEditable = false });
                                }
                            }
                            else
                            {
                                OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Delta", AspectToStringFormat = "{0:N4}", Text = "Delta", Width = "Variance".Length * 8, IsEditable = false });
                            }

                            OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Mean", AspectToStringFormat = "{0:N4}", Text = "Mean", Width = "Variance".Length * 8, IsEditable = false });
                            OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Baseline", AspectToStringFormat = "{0:N4}", Text = "Baseline", Width = "Baseline2".Length * 8, IsEditable = false });
                            OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PercentOfBaseline", AspectToStringFormat = "{0:N4}", Width = "Percent Of Baseline".Length * 8, Text = "Percent Of Baseline", IsEditable = false });
                            OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.StandardDeviation", AspectToStringFormat = "{0:N4}", Width = "Standard Deviation".Length * 8, Text = "Standard Deviation", IsEditable = false });
                            OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Variance", Text = "Variance", AspectToStringFormat = "{0:N4}", Width = "Variance".Length * 8, IsEditable = false }); 
                        }
                        else
                        {
                            foreach (FieldCheck fieldCheck in IncidencelstResult)
                            {

                                if (fieldCheck.isChecked && fieldCheck.FieldName != IncidencelstResult.Last().FieldName)
                                {
                                    BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), AspectToStringFormat = "{0:N4}", Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                }
                            }
                        }
                        if (lstCRTable.First().CRCalculateValues.First().LstPercentile != null)
                        {
                            if (strPoolIncidencePercentiles != null && strPoolIncidencePercentiles.Count > 0)
                            {
                                double interval = 50.0 / CommonClass.CRLatinHypercubePoints;
                                i = 0;
                                while (i < strPoolIncidencePercentiles.Count)
                                {
                                    BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + (int)(Convert.ToDouble(strPoolIncidencePercentiles[i]) / interval - 1) / 2 + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + strPoolIncidencePercentiles[i].ToString(), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
                                    i++;
                                }
                            }
                            else
                            {
                                i = 0;
                                while (i < lstCRTable.First().CRCalculateValues.First().LstPercentile.Count())
                                {
                                    if (IncidencelstResult == null || IncidencelstResult.Last().isChecked)
                                    {
                                        BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + i + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()))))), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
                                    }
                                    i++;
                                }
                            }
                        }
                    }
                    // It's not pooled incidence, so it must be raw HIF estimates
                    else
                    {
                        foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                        {
                            cr.CRCalculateValues = cr.CRCalculateValues.Where(p => p.Population > 0).ToList();
                        }
                        if (cflstColumnRow == null)
                        {
                            BrightIdeasSoftware.OLVColumn olvColumnCol = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Col", Text = "Column", IsEditable = false, Width = "Columnnn".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnCol);
                            BrightIdeasSoftware.OLVColumn olvColumnRow = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Row", Text = "Row", IsEditable = false, Width = "Rowww".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnRow);
                        }
                        else
                        {
                            foreach (FieldCheck fieldCheck in cflstColumnRow)
                            {

                                if (fieldCheck.isChecked)
                                {
                                    BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                }
                            }
                        }
                        if (cflstHealth != null)
                        {
                            foreach (FieldCheck fieldCheck in cflstHealth)
                            {
                                if (fieldCheck.FieldName.Equals("Geographic Area") && forceShowGeographicArea)
                                {
                                    fieldCheck.isChecked = true;
                                }
                                if (fieldCheck.isChecked)
                                {
                                    BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                }
                            }
                        }
                        if (cflstResult == null)
                        {
                            Boolean hasSeasonal = false;
                            foreach(CRSelectFunctionCalculateValue o in lstCRTable)
                            {
                                if(o.CRSelectFunction.BenMAPHealthImpactFunction.BetaVariation != null && o.CRSelectFunction.BenMAPHealthImpactFunction.BetaVariation.BetaVariationID == 2) //Seasonal
                                {
                                    hasSeasonal = true;
                                    break;
                                }
                            }
                            if (hasSeasonal)
                            {
                                string bvType = "Seasonal"; 
                                OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.BetaName", AspectToStringFormat = "{0:N4}", Text = bvType, Width = bvType.Length * 8, IsEditable = false });
                            }
                            OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PointEstimate", AspectToStringFormat = "{0:N4}", Width = "PointEstimate".Length * 8, Text = "Point Estimate", IsEditable = false });
                            OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Population", AspectToStringFormat = "{0:N4}", Text = "Population", Width = "Population".Length * 8, IsEditable = false });

                            // set up delta columns dynamically for multipollutant 
                            if (lstCRTable.First().CRCalculateValues.Count > 0 && lstCRTable.First().CRCalculateValues.First().DeltaList != null)
                            {
                                int dCount = lstCRTable.First().CRCalculateValues.First().DeltaList.Count();
                                for (int j = 0; j < dCount; j++)
                                {
                                    string pollName = lstCRTable.First().CRSelectFunction.BenMAPHealthImpactFunction.Variables[j].PollutantName;
                                    int pollID = ConfigurationCommonClass.getPollutantIDFromPollutantNameAndObject(lstCRTable.First().CRSelectFunction, pollName);

                                    OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.DeltaList[" + j + "]", AspectToStringFormat = "{0:N4}", Text = "Delta_" + pollName, Width = ("Delta_" + pollName).Length * 8, IsEditable = false });
                                }
                            }
                            else
                            {
                                OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Delta", AspectToStringFormat = "{0:N4}", Text = "Delta", Width = "Variance".Length * 8, IsEditable = false });
                            }

                            OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Mean", AspectToStringFormat = "{0:N4}", Text = "Mean", Width = "Variance".Length * 8, IsEditable = false });
                            OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Baseline", Text = "Baseline", AspectToStringFormat = "{0:N4}", Width = "Baseline2".Length * 8, IsEditable = false });
                            OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PercentOfBaseline", AspectToStringFormat = "{0:N4}", Width = "Percent Of Baseline".Length * 8, Text = "Percent Of Baseline", IsEditable = false });
                            OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.StandardDeviation", AspectToStringFormat = "{0:N4}", Width = "Standard Deviation".Length * 8, Text = "Standard Deviation", IsEditable = false });
                            OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Variance", AspectToStringFormat = "{0:N4}", Text = "Variance", IsEditable = false, Width = "Variance".Length * 8 }); 
                        }
                        else
                        {
                            if (lstCRTable.First().CRCalculateValues.First().BetaVariationName != null)
                            {
                                string bvType = lstCRTable.First().CRCalculateValues.First().BetaVariationName;
                                OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.BetaName", AspectToStringFormat = "{0:N4}", Text = bvType, Width = bvType.Length * 8, IsEditable = false });
                            }
                            foreach (FieldCheck fieldCheck in cflstResult)
                            {

                                if (fieldCheck.isChecked && fieldCheck.FieldName != cflstResult.Last().FieldName && fieldCheck.FieldName != "Population Weighted Delta"
                                    && fieldCheck.FieldName != "Population Weighted Base" && fieldCheck.FieldName != "Population Weighted Control")
                                {
                                    if (fieldCheck.FieldName == "Delta")
                                    {
                                        // set up delta columns dynamically for multipollutant 
                                        if (lstCRTable.First().CRCalculateValues.Count > 0 && lstCRTable.First().CRCalculateValues.First().Deltas != null)
                                        {
                                            int dCount = lstCRTable.First().CRCalculateValues.First().Deltas.Count();
                                            for (int j = 0; j < dCount; j++)
                                            {
                                                string pollName = lstCRTable.First().CRSelectFunction.BenMAPHealthImpactFunction.Variables[j].PollutantName;

                                                OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.DeltaList[" + j + "]", AspectToStringFormat = "{0:N4}", Text = "Delta_" + pollName, Width = ("Delta_" + pollName).Length * 8, IsEditable = false });
                                            }
                                        }
                                        else
                                        {
                                            OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Delta", AspectToStringFormat = "{0:N4}", Text = "Delta", Width = "Variance".Length * 8, IsEditable = false });
                                        }
                                    }
                                    else
                                    {
                                        BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), AspectToStringFormat = "{0:N4}", Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false };
                                        OLVResultsShow.Columns.Add(olvColumnID);
                                    }
                                }
                            }
                        }
                        if (lstCRTable.First().CRCalculateValues.First().LstPercentile != null)
                        {
                            if (strHealthImpactPercentiles != null && strHealthImpactPercentiles.Count > 0)
                            {
                                double interval = 50.0 / CommonClass.CRLatinHypercubePoints;
                                i = 0;
                                while (i < strHealthImpactPercentiles.Count)
                                {
                                    BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + (int)(Convert.ToDouble(strHealthImpactPercentiles[i]) / interval - 1) / 2 + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + strHealthImpactPercentiles[i].ToString(), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
                                    i++;
                                }
                            }
                            else
                            {
                                i = 0;
                                while (i < lstCRTable.First().CRCalculateValues.First().LstPercentile.Count())
                                {
                                    if (cflstResult == null || cflstResult.Last().isChecked)
                                    {
                                        BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + i + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()))))), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
                                    }
                                    i++;
                                }
                            }
                        }
                    }

                    Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicAPV = new Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>(); int iLstCRTable = 0;
                    Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();

                    if (cflstResult != null && cflstResult.Where(p => p.FieldName == "Population Weighted Delta").Count() == 1 && this.tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Tag.ToString() != "incidence")
                    {
                        if (cflstResult.Where(p => p.FieldName == "Population Weighted Delta").First().isChecked == true)
                        {
                            for (int ind = 0; ind < lstCRTable.First().CRCalculateValues.First().DeltaList.Count; ind++)
                            {
                                foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                                {
                                    dicKey = null;
                                    dicKey = new Dictionary<CRCalculateValue, int>();
                                    CRCalculateValue crv = new CRCalculateValue();
                                    crv.PointEstimate = cr.CRCalculateValues.Sum(p => p.Population *  (float)p.DeltaList[ind]) / cr.CRCalculateValues.Sum(p => p.Population);

                                    if (cr.CRCalculateValues.First().DeltaList != null && cr.CRCalculateValues.First().DeltaList.Count > 0)
                                    {
                                        crv.DeltaList = new List<double>();
                                        foreach (double d in cr.CRCalculateValues.First().DeltaList)
                                        {
                                            crv.DeltaList.Add(0);
                                        }
                                    }

                                    if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
                                    {
                                        crv.LstPercentile = new List<float>();
                                        foreach (float f in cr.CRCalculateValues.First().LstPercentile)
                                        {
                                            crv.LstPercentile.Add(0);
                                        }
                                    }
                                    dicKey.Add(crv, iLstCRTable);
                                    CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
                                    crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Delta_" + cr.CRSelectFunction.BenMAPHealthImpactFunction.Variables[ind].PollutantName;
                                    dicAPV.Add(dicKey.ToList()[0], crNew);
                                    iLstCRTable++;
                                }
                            }
                        }

                        if (cflstResult.Where(p => p.FieldName == "Population Weighted Base").First().isChecked == true)
                        {
                            for (int ind = 0; ind < lstCRTable.First().CRCalculateValues.First().DeltaList.Count; ind++)
                            {
                                foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                                {
                                    dicKey = null;
                                    dicKey = new Dictionary<CRCalculateValue, int>();
                                    CRCalculateValue crv = new CRCalculateValue();
                                    CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
                                    BenMAPLine benMAPLineBase = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == cr.CRSelectFunction.BenMAPHealthImpactFunction.Variables[ind].Pollutant1ID).First().Base;
                                    Dictionary<string, float> dicBase = new Dictionary<string, float>();
                                    string strMetric = "";

                                    if (cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
                                    {
                                        strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName;
                                    }
                                    else
                                        strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName;

                                    foreach (ModelResultAttribute mr in benMAPLineBase.ModelResultAttributes)
                                    {
                                        dicBase.Add(mr.Col + "," + mr.Row, mr.Values[strMetric]);
                                    }

                                    float dPointEstimate = 0;
                                    foreach (CRCalculateValue crvForEstimate in cr.CRCalculateValues)
                                    {
                                        if (dicBase.ContainsKey(crvForEstimate.Col + "," + crvForEstimate.Row))
                                        {
                                            dPointEstimate += dicBase[crvForEstimate.Col + "," + crvForEstimate.Row] * crvForEstimate.Population;
                                        }
                                    }

                                    crv.PointEstimate = dPointEstimate / cr.CRCalculateValues.Sum(p => p.Population);

                                    if (cr.CRCalculateValues.First().DeltaList != null && cr.CRCalculateValues.First().DeltaList.Count > 0)
                                    {
                                        crv.DeltaList = new List<double>();
                                        foreach (double d in cr.CRCalculateValues.First().DeltaList)
                                        {
                                            crv.DeltaList.Add(0);
                                        }
                                    }

                                    if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
                                    {
                                        crv.LstPercentile = new List<float>();
                                        foreach (float f in cr.CRCalculateValues.First().LstPercentile)
                                        {
                                            crv.LstPercentile.Add(0);
                                        }
                                    }
                                    dicKey.Add(crv, iLstCRTable);

                                    crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Base_" + cr.CRSelectFunction.BenMAPHealthImpactFunction.Variables[ind].PollutantName;
                                    dicAPV.Add(dicKey.ToList()[0], crNew);
                                    iLstCRTable++;
                                }
                            }
                           
                        }

                        if (cflstResult.Where(p => p.FieldName == "Population Weighted Control").First().isChecked == true)
                        {
                            for (int ind = 0; ind < lstCRTable.First().CRCalculateValues.First().DeltaList.Count; ind++)
                            {
                                foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                                {
                                    dicKey = null;
                                    dicKey = new Dictionary<CRCalculateValue, int>();
                                    CRCalculateValue crv = new CRCalculateValue();
                                    CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
                                    BenMAPLine benMAPLineControl = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == cr.CRSelectFunction.BenMAPHealthImpactFunction.Variables[ind].Pollutant1ID).First().Control;
                                    Dictionary<string, float> dicControl = new Dictionary<string, float>();
                                    string strMetric = "";

                                    if (cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
                                    {
                                        strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName;
                                    }
                                    else
                                        strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName;

                                    foreach (ModelResultAttribute mr in benMAPLineControl.ModelResultAttributes)
                                    {
                                        dicControl.Add(mr.Col + "," + mr.Row, mr.Values[strMetric]);

                                    }

                                    float dPointEstimate = 0;
                                    foreach (CRCalculateValue crvForEstimate in cr.CRCalculateValues)
                                    {
                                        if (dicControl.ContainsKey(crvForEstimate.Col + "," + crvForEstimate.Row))
                                        {
                                            dPointEstimate += dicControl[crvForEstimate.Col + "," + crvForEstimate.Row] * crvForEstimate.Population;
                                        }
                                    }

                                    crv.PointEstimate = dPointEstimate / cr.CRCalculateValues.Sum(p => p.Population);

                                    if (cr.CRCalculateValues.First().DeltaList != null && cr.CRCalculateValues.First().DeltaList.Count > 0)
                                    {
                                        crv.DeltaList = new List<double>();
                                        foreach (double d in cr.CRCalculateValues.First().DeltaList)
                                        {
                                            crv.DeltaList.Add(0);
                                        }
                                    }

                                    if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
                                    {
                                        crv.LstPercentile = new List<float>();
                                        foreach (float f in cr.CRCalculateValues.First().LstPercentile)
                                        {
                                            crv.LstPercentile.Add(0);
                                        }
                                    }
                                    dicKey.Add(crv, iLstCRTable);

                                    crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Control_" + cr.CRSelectFunction.BenMAPHealthImpactFunction.Variables[ind].PollutantName;
                                    dicAPV.Add(dicKey.ToList()[0], crNew);
                                    iLstCRTable++;
                                }
                            }
                        }
                    }

                    Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicAPV_Sum = new Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>();

                    foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                    {
                        foreach (CRCalculateValue crv in cr.CRCalculateValues)
                        {
                            dicKey = null;
                            dicKey = new Dictionary<CRCalculateValue, int>();
                            dicKey.Add(crv, iLstCRTable);

                            dicAPV.Add(dicKey.ToList()[0], cr.CRSelectFunction);
                            dicAPV_Sum.Add(ConfigurationCommonClass.getKeyValuePairDeepCopy(dicKey.ToList()[0]), cr.CRSelectFunction);
                        }
                        iLstCRTable++;
                    }

                    // If the user has checked "Sum values across year" then they want to combine seasons having the same name
                    // For example, if they have defined two code seasons: at the beginning and end of the year
                    // Loop over all results and build a list that is unique by row, col, season
                    // If we don't have the row/col/season in the list, add a deep copy
                    // If we do, add the current record's estimate to it
                    if (isSumChecked) //&& dicAPV.Keys.First().Key.BetaVariationName.ToLower() != "full year")
                    {
                        // dictionary in the structure the table needs
                        Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicTempResults = new Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>();
                        // dictionary in a format we can use to collect by row, col, season. This dic will reference the same objects as dicTempResults and won't be needed after this process
                        Dictionary <string, Object> dicRowColSeasonLookup = new Dictionary<string, Object>();

                        foreach (KeyValuePair<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> kvp in dicAPV_Sum)
                        {
                            string keyRowColSeason = kvp.Key.Key.Row + "-" + kvp.Key.Key.Col + "-" + kvp.Key.Key.BetaName;
                            if (dicRowColSeasonLookup.ContainsKey(keyRowColSeason))
                            {
                                // Just add to it then
                                KeyValuePair<CRCalculateValue, int> k = (KeyValuePair<CRCalculateValue, int>)dicRowColSeasonLookup[keyRowColSeason];
                                k.Key.PointEstimate += kvp.Key.Key.PointEstimate;

                                int percentileIndex = 0;
                                foreach (float percentile in kvp.Key.Key.LstPercentile)
                                {
                                    k.Key.LstPercentile[percentileIndex] += percentile;
                                    percentileIndex++;

                                }
                            } 
                            else
                            {
                                // Create a deep copy
                                KeyValuePair<CRCalculateValue, int> k = kvp.Key;
                                CRCalculateValue deepCopy = ConfigurationCommonClass.getKeyValuePairDeepCopy(k).Key;
                                KeyValuePair<CRCalculateValue, int> newPair = new KeyValuePair<CRCalculateValue, int>(deepCopy, k.Value);
                                dicRowColSeasonLookup.Add(keyRowColSeason, newPair);
                                dicTempResults.Add(newPair, kvp.Value);
                            }
                        }

                        dicAPV_Sum.Clear();
                        dicAPV_Sum = dicTempResults;

                        _tableObject = lstCRTable;
                        OLVResultsShow.SetObjects(dicAPV_Sum.ToList().GetRange(0, dicAPV_Sum.Count > 50 ? 50 : dicAPV_Sum.Count));
                        _pageSize = 50;
                        _currentRow = 0;
                        _pageCount = dicAPV_Sum.Count / 50 + 1; _pageCurrent = 1;
                        bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
                        bindingNavigatorCountItem.Text = _pageCount.ToString();
                    }

                    else
                    {
                        _tableObject = lstCRTable;
                        OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(0, dicAPV.Count > 50 ? 50 : dicAPV.Count));
                        _pageSize = 50;
                        _currentRow = 0;
                        _pageCount = dicAPV.Count / 50 + 1; _pageCurrent = 1;
                        bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
                        bindingNavigatorCountItem.Text = _pageCount.ToString();
                    }
                }



                else if (oTable is List<AllSelectValuationMethodAndValue> || oTable is AllSelectValuationMethodAndValue)
                {
                    // Option 4 - Pooled Valuation
                    List<AllSelectValuationMethodAndValue> lstallSelectValuationMethodAndValue = new List<AllSelectValuationMethodAndValue>();
                    if (oTable is List<AllSelectValuationMethodAndValue>)
                    {
                        lstallSelectValuationMethodAndValue = (List<AllSelectValuationMethodAndValue>)oTable;
                    }
                    else
                    {
                        lstallSelectValuationMethodAndValue.Add((AllSelectValuationMethodAndValue)oTable);
                    }
                    for (int iValuation = 0; iValuation < lstallSelectValuationMethodAndValue.Count; iValuation++)
                    {
                        lstallSelectValuationMethodAndValue[iValuation].lstAPVValueAttributes = lstallSelectValuationMethodAndValue[iValuation].lstAPVValueAttributes.OrderBy(p => p.Col).ToList();
                        if (string.IsNullOrWhiteSpace(lstallSelectValuationMethodAndValue[iValuation].AllSelectValuationMethod.GeographicArea) == false &&
   lstallSelectValuationMethodAndValue[iValuation].AllSelectValuationMethod.GeographicArea.Equals(Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_EVERYWHERE) == false)
                        {
                            forceShowGeographicArea = true;
                        }
                    }
                    if (apvlstColumnRow == null)
                    {
                        BrightIdeasSoftware.OLVColumn olvColumnCol = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Col", Text = "Column", IsEditable = false, Width = "Columnnn".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnCol);
                        BrightIdeasSoftware.OLVColumn olvColumnRow = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Row", Text = "Row", IsEditable = false, Width = "Rowww".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnRow);

                    }
                    else
                    {
                        foreach (FieldCheck fieldCheck in apvlstColumnRow)
                        {

                            if (fieldCheck.isChecked)
                            {
                                BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
                            }
                        }

                    }

                    if (apvlstHealth != null)
                    {
                        foreach (FieldCheck fieldCheck in apvlstHealth)
                        {
                            if (fieldCheck.FieldName.Equals("Geographic Area") && forceShowGeographicArea)
                            {
                                fieldCheck.isChecked = true;
                            }
                            if (fieldCheck.isChecked)
                            {
                                BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
                            }
                        }

                    }
                    Boolean hasSeasonal = true;
                    /*
                     * Commented out until we can implement a good way to determine if the pooled results involve seasons here
                     * // lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().BetaVariationName != null
                    foreach (AllSelectCRFunction o in lstAllSelectCRFuntion)
                    {
                        if (o.CRSelectFunction.BenMAPHealthImpactFunction.BetaVariation != null && o.CRSelectFunction.BenMAPHealthImpactFunction.BetaVariation.BetaVariationID == 2) //Seasonal
                        {
                            hasSeasonal = true;
                            break;
                        }
                    }
                    */
                    if (hasSeasonal)
                    {
                        string bvType = "Seasonal";
                        OLVResultsShow.Columns.Add(new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.BetaName", AspectToStringFormat = "{0:N4}", Text = bvType, Width = bvType.Length * 8, IsEditable = false });
                    }
                    if (apvlstResult == null)
                    {
                        BrightIdeasSoftware.OLVColumn olvColumnPointEstimate = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PointEstimate", AspectToStringFormat = "{0:N4}", Width = "Point Estimate".Length * 8, Text = "Point Estimate", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPointEstimate);
                        BrightIdeasSoftware.OLVColumn olvColumnMean = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Mean", AspectToStringFormat = "{0:N4}", Text = "Mean", Width = "MethodID".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnMean);
                        BrightIdeasSoftware.OLVColumn olvColumnStandardDeviation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.StandardDeviation", AspectToStringFormat = "{0:N4}", Width = "Standard Deviation".Length * 8, Text = "Standard Deviation", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnStandardDeviation);
                        BrightIdeasSoftware.OLVColumn olvColumnVariance = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Variance", AspectToStringFormat = "{0:N4}", Text = "Variance", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnVariance);
                    }
                    else
                    {
                        foreach (FieldCheck fieldCheck in apvlstResult)
                        {

                            if (fieldCheck.FieldName != apvlstResult.Last().FieldName && fieldCheck.isChecked)
                            {
                                BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key." + getFieldNameFromlstAPV(fieldCheck.FieldName), AspectToStringFormat = "{0:N4}", Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
                            }
                        }

                    }
                    if (apvlstResult == null || apvlstResult.Last().isChecked)
                    {
                        if (strAPVPercentiles != null && strAPVPercentiles.Count > 0)
                        {
                            i = 0;
                            while (i < strAPVPercentiles.Count)
                            {
                                BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + (int)(Convert.ToDouble(strAPVPercentiles[i]) / 0.5 - 1) / 2 + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + strAPVPercentiles[i].ToString(), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
                                i++;
                            }
                        }
                        else
                        {
                            i = 0;
                            if (lstallSelectValuationMethodAndValue != null && lstallSelectValuationMethodAndValue.Count > 0
                                && lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes != null
                                && lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.Count > 0
                                && lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile != null)
                            {
                                while (i < lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count())
                                {

                                    BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + i + "]", AspectToStringFormat = "{0:N4}", Width = "LstPercentile".Length * 8, Text = "Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count()))))), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

                                    i++;
                                }
                            }
                        }
                    }
                    _tableObject = lstallSelectValuationMethodAndValue;
                    Dictionary<KeyValuePair<APVValueAttribute, int>, AllSelectValuationMethod> dicAPV = new Dictionary<KeyValuePair<APVValueAttribute, int>, AllSelectValuationMethod>();
                    Dictionary<APVValueAttribute, int> dicLstAllSelectValuationMethodAndValue = new Dictionary<APVValueAttribute, int>();
                    int ilstallSelectValuationMethodAndValue = 0;
                    foreach (AllSelectValuationMethodAndValue allSelectValuationMethodAndValue in lstallSelectValuationMethodAndValue)
                    {
                        foreach (APVValueAttribute apvx in allSelectValuationMethodAndValue.lstAPVValueAttributes)
                        {
                            dicLstAllSelectValuationMethodAndValue = null;
                            dicLstAllSelectValuationMethodAndValue = new Dictionary<APVValueAttribute, int>();
                            dicLstAllSelectValuationMethodAndValue.Add(apvx, ilstallSelectValuationMethodAndValue);
                            dicAPV.Add(dicLstAllSelectValuationMethodAndValue.ToList()[0], allSelectValuationMethodAndValue.AllSelectValuationMethod);
                        }
                        ilstallSelectValuationMethodAndValue++;
                    }
                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(0, dicAPV.Count > 50 ? 50 : dicAPV.Count));
                    _pageSize = 50;
                    _currentRow = 0;
                    _pageCount = dicAPV.Count / 50 + 1;
                    _pageCurrent = 1;
                    bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
                    bindingNavigatorCountItem.Text = _pageCount.ToString();
                }


                else if (oTable is List<AllSelectQALYMethodAndValue> || oTable is AllSelectQALYMethodAndValue)
                {
                    // Option 5
                    List<AllSelectQALYMethodAndValue> lstallSelectQALYMethodAndValue = new List<AllSelectQALYMethodAndValue>();
                    if (oTable is List<AllSelectQALYMethodAndValue>)
                    {
                        lstallSelectQALYMethodAndValue = (List<AllSelectQALYMethodAndValue>)oTable;
                    }
                    else
                    {
                        lstallSelectQALYMethodAndValue.Add((AllSelectQALYMethodAndValue)oTable);
                    }
                    for (int iQALY = 0; iQALY < lstallSelectQALYMethodAndValue.Count; iQALY++)
                    {
                        lstallSelectQALYMethodAndValue[iQALY].lstQALYValueAttributes = lstallSelectQALYMethodAndValue[iQALY].lstQALYValueAttributes.OrderBy(p => p.Col).ToList();
                    }
                    if (qalylstColumnRow == null)
                    {
                        BrightIdeasSoftware.OLVColumn olvColumnCol = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Col", Text = "Column", IsEditable = false, Width = "Columnnn".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnCol);
                        BrightIdeasSoftware.OLVColumn olvColumnRow = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Row", Text = "Row", IsEditable = false, Width = "Rowww".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnRow);
                    }
                    else
                    {
                        foreach (FieldCheck fieldCheck in qalylstColumnRow)
                        {

                            if (fieldCheck.isChecked)
                            {
                                BrightIdeasSoftware.OLVColumn olvColumnQALYID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnQALYID);
                            }
                        }

                    }
                    if (qalylstHealth != null)
                    {
                        foreach (FieldCheck fieldCheck in qalylstHealth)
                        {

                            if (fieldCheck.isChecked)
                            {
                                BrightIdeasSoftware.OLVColumn olvColumnQALYID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnQALYID);
                            }
                        }

                    }
                    if (qalylstResult == null)
                    {
                        BrightIdeasSoftware.OLVColumn olvColumnPointEstimate = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.PointEstimate", AspectToStringFormat = "{0:N4}", Width = "Point Estimatee".Length * 8, Text = "Point Estimate", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPointEstimate);
                        BrightIdeasSoftware.OLVColumn olvColumnMean = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Mean", AspectToStringFormat = "{0:N4}", Text = "Mean", Width = "MethodID".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnMean);
                        BrightIdeasSoftware.OLVColumn olvColumnStandardDeviation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.StandardDeviation", AspectToStringFormat = "{0:N4}", Width = "Standard Deviationn".Length * 8, Text = "Standard Deviation", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnStandardDeviation);
                        BrightIdeasSoftware.OLVColumn olvColumnVariance = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Variance", AspectToStringFormat = "{0:N4}", Text = "Variance", Width = "Variancee".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnVariance);
                    }
                    else
                    {
                        foreach (FieldCheck fieldCheck in qalylstResult)
                        {

                            if (fieldCheck.FieldName != qalylstResult.Last().FieldName && fieldCheck.isChecked)
                            {
                                BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstAPV(fieldCheck.FieldName), AspectToStringFormat = "{0:N4}", Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
                            }
                        }

                    }
                    if (qalylstResult == null || qalylstResult.Last().isChecked)
                    {
                        i = 0;
                        if (lstallSelectQALYMethodAndValue != null && lstallSelectQALYMethodAndValue.Count > 0 && lstallSelectQALYMethodAndValue.First().lstQALYValueAttributes != null
                            && lstallSelectQALYMethodAndValue.First().lstQALYValueAttributes.Count > 0
                            && lstallSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile != null)
                        {
                            while (i < lstallSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count())
                            {

                                BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.LstPercentile[" + i + "]", AspectToStringFormat = "{0:N4}", Width = "LstPercentile".Length * 8, Text = "Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstallSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstallSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()))))), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

                                i++;
                            }
                        }
                    }
                    _tableObject = lstallSelectQALYMethodAndValue;
                    Dictionary<QALYValueAttribute, AllSelectQALYMethod> dicAPV = new Dictionary<QALYValueAttribute, AllSelectQALYMethod>();
                    foreach (AllSelectQALYMethodAndValue allSelectQALYMethodAndValue in lstallSelectQALYMethodAndValue)
                    {
                        foreach (QALYValueAttribute apvx in allSelectQALYMethodAndValue.lstQALYValueAttributes)
                        {
                            dicAPV.Add(apvx, allSelectQALYMethodAndValue.AllSelectQALYMethod);
                        }
                    }
                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(0, dicAPV.Count > 50 ? 50 : dicAPV.Count));
                    _pageSize = 50;
                    _currentRow = 0;
                    _pageCount = dicAPV.Count / 50 + 1;
                    _pageCurrent = 1;
                    bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
                    bindingNavigatorCountItem.Text = _pageCount.ToString();
                }

                // The user is displaying AQ data
                else if (oTable is BenMAPLine)
                {
                    //Option 6
                    BenMAPLine crTable = (BenMAPLine)oTable;
                    crTable.ModelResultAttributes = crTable.ModelResultAttributes.OrderBy(p => p.Col).ToList();
                    BrightIdeasSoftware.OLVColumn olvColumnCol = new BrightIdeasSoftware.OLVColumn() { AspectName = "Col", Text = "Column", IsEditable = false, Width = "Columnss".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnCol);
                    BrightIdeasSoftware.OLVColumn olvColumnRow = new BrightIdeasSoftware.OLVColumn() { AspectName = "Row", Text = "Row", IsEditable = false, Width = "Rowss".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnRow);
                    List<string> lstAddField = new List<string>();
                    List<double[]> lstResultCopy = new List<double[]>();
                    if (crTable.Pollutant.Metrics != null)
                    {
                        foreach (Metric metric in crTable.Pollutant.Metrics)
                        {
                            lstAddField.Add(metric.MetricName);
                        }
                    }
                    if (crTable.Pollutant.SesonalMetrics != null)
                    {
                        foreach (SeasonalMetric sesonalMetric in crTable.Pollutant.SesonalMetrics)
                        {
                            lstAddField.Add(sesonalMetric.SeasonalMetricName);
                        }
                    }

                    i = 0;
                    while (i < lstAddField.Count())
                    {

                        BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Values[" + lstAddField[i] + "]", AspectToStringFormat = "{0:N2}", Width = lstAddField[i].Length * 9, Text = lstAddField[i], IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

                        i++;
                    }

                    _tableObject = crTable;
                    OLVResultsShow.SetObjects(crTable.ModelResultAttributes.GetRange(0, crTable.ModelResultAttributes.Count > 50 ? 50 : crTable.ModelResultAttributes.Count));
                    _pageSize = 50;
                    _currentRow = 0;
                    _pageCount = crTable.ModelResultAttributes.Count / 50 + 1;
                    _pageCurrent = 1;
                    bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
                    bindingNavigatorCountItem.Text = _pageCount.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void InitChartResult(object oTable, int GridID)
        {
            try
            {
                zedGraphCtl.Visible = true;
                olvRegions.Visible = true;
                cbGraph.Visible = true;
                btnApply.Visible = true;
                groupBox9.Visible = true;
                groupBox1.Visible = true;
                btnSelectAll.Visible = true;
                Dictionary<string, double> dicValue = new Dictionary<string, double>();
                GridRelationship gRegionGridRelationship = null;
                List<List<RowCol>> lstlstRowCol = null;
                List<string> lstString = null;
                List<RowCol> lstRowCol = null;
                int iRowCount = 0;
                double d = 0;
                GraphPane myPane = this.zedGraphCtl.GraphPane;
                DataRowView drGrid = cboRegion.SelectedItem as DataRowView;
                bool showOriginalGrid;
                if (tabCtlReport.SelectedTab == tabPoolingIncidence || tabCtlReport.SelectedTab == tabAPVResultGISShow)
                    showOriginalGrid = true;
                else
                    showOriginalGrid = false;
                if (!showOriginalGrid)
                {
                    if (CommonClass.RBenMAPGrid == null)
                    {

                        CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));
                    }
                    if (GridID != CommonClass.RBenMAPGrid.GridDefinitionID)
                    {
                        foreach (GridRelationship gr in CommonClass.LstGridRelationshipAll)
                        {
                            if (gr.bigGridID == CommonClass.RBenMAPGrid.GridDefinitionID && gr.smallGridID == GridID)
                            {
                                gRegionGridRelationship = gr;
                            }
                            else if (gr.smallGridID == CommonClass.RBenMAPGrid.GridDefinitionID && gr.bigGridID == GridID)
                            {
                                gRegionGridRelationship = gr;
                            }
                        }
                    }
                }
                if (oTable is CRSelectFunctionCalculateValue)
                {
                    CRSelectFunctionCalculateValue crTable = (CRSelectFunctionCalculateValue)oTable;
                    if (!showOriginalGrid && CommonClass.RBenMAPGrid.GridDefinitionID != GridID)
                        crTable = APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crTable, GridID, CommonClass.RBenMAPGrid.GridDefinitionID);
                    foreach (CRCalculateValue crv in crTable.CRCalculateValues)
                    {
                        if (!dicValue.Keys.Contains(crv.Col + "," + crv.Row))
                            dicValue.Add(crv.Col + "," + crv.Row, crv.PointEstimate);
                    }

                    strCDFTitle = "Health Impact Results";
                    strCDFX = "Health Impact";
                    strCDFY = "Cumulative Percentage of Health Impact";
                    strchartTitle = "Incidence";
                    strchartX = "Region";
                    strchartY = "Health Impact";
                }
                else if (oTable is AllSelectValuationMethodAndValue)
                {
                    AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = (AllSelectValuationMethodAndValue)oTable;
                    foreach (APVValueAttribute crv in allSelectValuationMethodAndValue.lstAPVValueAttributes)
                    {
                        if (!dicValue.Keys.Contains(crv.Col + "," + crv.Row))
                            dicValue.Add(crv.Col + "," + crv.Row, crv.PointEstimate);
                    }

                    strCDFTitle = "Monetized Benefits";
                    strCDFX = "Monetized Benefits";
                    strCDFY = "Cumulative Percentage of Monetized Benefits";
                    strchartTitle = "Pooled Valuation";
                    strchartX = "Region";
                    strchartY = "Valuation($)";
                }
                else if (oTable is List<AllSelectValuationMethodAndValue>)
                {
                    List<AllSelectValuationMethodAndValue> lstallSelectValuationMethodAndValue = (List<AllSelectValuationMethodAndValue>)oTable;
                    AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.First(); foreach (APVValueAttribute crv in allSelectValuationMethodAndValue.lstAPVValueAttributes)
                    {
                        if (!dicValue.Keys.Contains(crv.Col + "," + crv.Row))
                            dicValue.Add(crv.Col + "," + crv.Row, crv.PointEstimate);
                    }

                    strCDFTitle = "Monetized Benefits";
                    strCDFX = "Monetized Benefits";
                    strCDFY = "Cumulative Percentage of Monetized Benefits";
                    strchartTitle = "Pooled Valuation";
                    strchartX = "Region";
                    strchartY = "Valuation($)";
                }
                if (canshowCDF)
                {
                    cbGraph.Enabled = true;
                }
                else
                {
                    cbGraph.SelectedIndex = 0;
                    cbGraph.Enabled = false;
                }
                if (cbGraph.Text == "") cbGraph.SelectedIndex = 0;
                if (showOriginalGrid)
                {
                    ChartGrid = Grid.GridCommon.getBenMAPGridFromID(GridID);
                }
                else
                    ChartGrid = CommonClass.RBenMAPGrid;
                if (CommonClass.lstChartResult.Count > 5) iRowCount = 5;
                else iRowCount = CommonClass.lstChartResult.Count;
                if (dicValue.Count == 0) return;
                int i = 0;
                if (1 == 1)
                {
                    i = 0;
                    while (i < CommonClass.lstChartResult.Count)
                    {
                        if (dicValue.Keys.Contains(CommonClass.lstChartResult[i].Col + "," + CommonClass.lstChartResult[i].Row))
                        {
                            CommonClass.lstChartResult[i].RegionValue = dicValue[CommonClass.lstChartResult[i].Col + "," + CommonClass.lstChartResult[i].Row];
                        }
                        else
                            CommonClass.lstChartResult[i].RegionValue = 0;
                        i++;
                    }
                }
                else if (CommonClass.RBenMAPGrid.GridDefinitionID == gRegionGridRelationship.bigGridID)
                {
                    foreach (ChartResult cr in CommonClass.lstChartResult)
                    {
                        cr.RegionValue = 0;
                    }
                    IEnumerable<KeyValuePair<string, double>> IDicValue = null;
                    ChartResult crResult = null;
                    Dictionary<string, double> dicSum = new Dictionary<string, double>();
                    foreach (GridRelationshipAttribute gridRelationshipAttribute in gRegionGridRelationship.lstGridRelationshipAttribute)
                    {
                        dicSum.Add(gridRelationshipAttribute.bigGridRowCol.Col + "," + gridRelationshipAttribute.bigGridRowCol.Row, 0.0);
                    }
                    foreach (GridRelationshipAttribute gridRelationshipAttribute in gRegionGridRelationship.lstGridRelationshipAttribute)
                    {
                        try
                        {
                            foreach (RowCol rc in gridRelationshipAttribute.smallGridRowCol)
                            {
                                if (dicValue.Keys.Contains(rc.Col + "," + rc.Row))
                                {
                                    dicSum[gridRelationshipAttribute.bigGridRowCol.Col + "," + gridRelationshipAttribute.bigGridRowCol.Row] += dicValue[rc.Col + "," + rc.Row];
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    foreach (ChartResult chartResult in CommonClass.lstChartResult)
                    {
                        if (dicSum.Keys.Contains(chartResult.Col + "," + chartResult.Row))
                            chartResult.RegionValue = dicSum[chartResult.Col + "," + chartResult.Row];
                    }

                }
                else
                {
                    i = 0;
                    while (i < CommonClass.lstChartResult.Count)
                    {
                        var query = from a in gRegionGridRelationship.lstGridRelationshipAttribute
                                    where a.smallGridRowCol.Contains(new RowCol()
                                    {
                                        Col = CommonClass.lstChartResult[i].Col
                                        ,
                                        Row = CommonClass.lstChartResult[i].Row
                                    }, new RowColComparer())
                                    select a.bigGridRowCol;
                        if (query != null && query.Count() > 0)
                        {
                            RowCol rc = query.First();
                            try
                            {
                                if (dicValue.Keys.Contains(rc.Col + "," + rc.Row))
                                {
                                    CommonClass.lstChartResult[i].RegionValue = Math.Round(dicValue[rc.Col + "," + rc.Row], 4);
                                }
                                else
                                {
                                    CommonClass.lstChartResult[i].RegionValue = 0.00;
                                }
                            }
                            catch (Exception ex)
                            { }
                        }
                        i++;
                    }
                }

                List<string> lstPane = new List<string>();

                olvRegions.SetObjects(CommonClass.lstChartResult);

                olvRegions.MultiSelect = true;
                if (CommonClass.lstChartResult.Count > 5)
                {
                    lstPane = CommonClass.lstChartResult.Select(p => p.RegionName).ToList().GetRange(0, 5);

                    for (int j = 0; j < 5; j++)
                    {
                        OLVListItem olvi = olvRegions.Items[j] as OLVListItem;
                        olvi.Checked = true;

                    }
                }
                else
                {
                    lstPane = CommonClass.lstChartResult.Select(p => p.RegionName).ToList();

                    for (int j = 0; j < CommonClass.lstChartResult.Count; j++)
                    {
                        OLVListItem olvi = olvRegions.Items[j] as OLVListItem;
                        olvi.Checked = true;

                    }
                }







                cbChartXAxis.Items.Clear();
                string shapefilename = "";
                if (ChartGrid is ShapefileGrid)
                    shapefilename = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (ChartGrid as ShapefileGrid).ShapefileName + ".shp";
                else if (CommonClass.RBenMAPGrid is RegularGrid)
                    shapefilename = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (ChartGrid as RegularGrid).ShapefileName + ".shp";
                DotSpatial.Data.IFeatureSet fs = new DotSpatial.Data.FeatureSet();
                if (File.Exists(shapefilename))
                {
                    int iName = -1;
                    int icName = -1;
                    int c = 0;
                    cbChartXAxis.Items.Add("Col/Row");
                    fs = DotSpatial.Data.FeatureSet.Open(shapefilename);
                    foreach (DataColumn dc in fs.DataTable.Columns)
                    {
                        if (dc.ColumnName.ToLower().Trim() != "col" && dc.ColumnName.ToLower().Trim() != "row")
                        {
                            cbChartXAxis.Items.Add(dc.ColumnName);
                            if (dc.ColumnName.ToLower().Trim() == "name")
                            {
                                iName = c;
                            }
                            else if (dc.ColumnName.ToLower().Contains("name"))
                            {
                                icName = c;
                            }
                        }
                        c++;
                    }
                    if (iName != -1)
                    {
                        cbChartXAxis.Text = fs.DataTable.Columns[iName].ColumnName;
                    }
                    else if (icName != -1)
                    {
                        cbChartXAxis.Text = fs.DataTable.Columns[icName].ColumnName;
                    }
                    else
                    {
                        cbChartXAxis.Text = "Col/Row";
                    }
                    chartXAxis = cbChartXAxis.Text;
                    fs.Close();
                    fs.Dispose();
                }

                if (cbGraph.Text == "Cumulative Distribution Functions")
                    cbGraph_SelectedIndexChanged(null, null);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }


        private void UpdateTableResult(object oTable)
        {


            if (oTable is List<AllSelectCRFunction>)
            {
                List<AllSelectCRFunction> lstAllSelectCRFuntion = (List<AllSelectCRFunction>)oTable;

                Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction> dicAPV = new Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>(); int iLstCRTable = 0;
                Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();

                foreach (AllSelectCRFunction cr in lstAllSelectCRFuntion)
                {
                    foreach (CRCalculateValue crv in cr.CRSelectFunctionCalculateValue.CRCalculateValues)
                    {
                        dicKey = null;
                        dicKey = new Dictionary<CRCalculateValue, int>();
                        dicKey.Add(crv, iLstCRTable);
                        dicAPV.Add(dicKey.ToList()[0], cr);
                    }
                    iLstCRTable++;
                }
                _tableObject = lstAllSelectCRFuntion;
                if (_pageCurrent == _pageCount)

                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, dicAPV.Count - (_pageCurrent - 1) * 50));
                else
                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, 50));
            }
            if (oTable is List<CRSelectFunctionCalculateValue> || oTable is CRSelectFunctionCalculateValue)
            {

                List<CRSelectFunctionCalculateValue> lstCRTable = new List<CRSelectFunctionCalculateValue>();
                if (oTable is List<CRSelectFunctionCalculateValue>)
                    lstCRTable = (List<CRSelectFunctionCalculateValue>)oTable;
                else
                    lstCRTable.Add(oTable as CRSelectFunctionCalculateValue);
                Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicAPV = new Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>(); int iLstCRTable = 0;
                Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();
                if (cflstResult != null && cflstResult.Where(p => p.FieldName == "Population Weighted Delta").Count() == 1 && this.tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Tag.ToString() != "incidence")
                {
                    if (cflstResult.Where(p => p.FieldName == "Population Weighted Delta").First().isChecked == true)
                    {
                        foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                        {
                            dicKey = null;
                            dicKey = new Dictionary<CRCalculateValue, int>();
                            CRCalculateValue crv = new CRCalculateValue();
                            crv.PointEstimate = cr.CRCalculateValues.Sum(p => p.Population * p.Delta) / cr.CRCalculateValues.Sum(p => p.Population);
                            if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
                            {
                                crv.LstPercentile = new List<float>();
                                foreach (float f in cr.CRCalculateValues.First().LstPercentile)
                                {
                                    crv.LstPercentile.Add(0);
                                }
                            }
                            dicKey.Add(crv, iLstCRTable);
                            CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
                            crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Delta";
                            dicAPV.Add(dicKey.ToList()[0], crNew);
                            iLstCRTable++;
                        }
                    }
                    if (cflstResult.Where(p => p.FieldName == "Population Weighted Base").First().isChecked == true)
                    {
                        foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                        {
                            dicKey = null;
                            dicKey = new Dictionary<CRCalculateValue, int>();
                            CRCalculateValue crv = new CRCalculateValue();
                            CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
                            BenMAPLine benMAPLineBase = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == crNew.BenMAPHealthImpactFunction.PollutantGroup.Pollutants.First().PollutantID).First().Base;
                            Dictionary<string, float> dicBase = new Dictionary<string, float>();
                            string strMetric = "";
                            if (cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
                            {
                                strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName;
                            }
                            else
                                strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName;
                            foreach (ModelResultAttribute mr in benMAPLineBase.ModelResultAttributes)
                            {
                                dicBase.Add(mr.Col + "," + mr.Row, mr.Values[strMetric]);

                            }
                            float dPointEstimate = 0;
                            foreach (CRCalculateValue crvForEstimate in cr.CRCalculateValues)
                            {
                                if (dicBase.ContainsKey(crvForEstimate.Col + "," + crvForEstimate.Row))
                                {
                                    dPointEstimate = dPointEstimate + dicBase[crvForEstimate.Col + "," + crvForEstimate.Row] * crvForEstimate.Population;
                                }
                            }
                            crv.PointEstimate = dPointEstimate / cr.CRCalculateValues.Sum(p => p.Population);
                            if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
                            {
                                crv.LstPercentile = new List<float>();
                                foreach (float f in cr.CRCalculateValues.First().LstPercentile)
                                {
                                    crv.LstPercentile.Add(0);
                                }
                            }
                            dicKey.Add(crv, iLstCRTable);

                            crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Base";
                            dicAPV.Add(dicKey.ToList()[0], crNew);
                            iLstCRTable++;
                        }
                    }
                    if (cflstResult.Where(p => p.FieldName == "Population Weighted Control").First().isChecked == true)
                    {
                        foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                        {
                            dicKey = null;
                            dicKey = new Dictionary<CRCalculateValue, int>();
                            CRCalculateValue crv = new CRCalculateValue();
                            CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
                            BenMAPLine benMAPLineControl = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == crNew.BenMAPHealthImpactFunction.PollutantGroup.Pollutants.First().PollutantID).First().Control;
                            Dictionary<string, float> dicControl = new Dictionary<string, float>();
                            string strMetric = "";
                            if (cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
                            {
                                strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName;
                            }
                            else
                                strMetric = cr.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName;
                            foreach (ModelResultAttribute mr in benMAPLineControl.ModelResultAttributes)
                            {
                                dicControl.Add(mr.Col + "," + mr.Row, mr.Values[strMetric]);

                            }
                            float dPointEstimate = 0;
                            foreach (CRCalculateValue crvForEstimate in cr.CRCalculateValues)
                            {
                                if (dicControl.ContainsKey(crvForEstimate.Col + "," + crvForEstimate.Row))
                                {
                                    dPointEstimate = dPointEstimate + dicControl[crvForEstimate.Col + "," + crvForEstimate.Row] * crvForEstimate.Population;
                                }
                            }
                            crv.PointEstimate = dPointEstimate / cr.CRCalculateValues.Sum(p => p.Population);
                            if (cr.CRCalculateValues.First().LstPercentile != null && cr.CRCalculateValues.First().LstPercentile.Count > 0)
                            {
                                crv.LstPercentile = new List<float>();
                                foreach (float f in cr.CRCalculateValues.First().LstPercentile)
                                {
                                    crv.LstPercentile.Add(0);
                                }
                            }
                            dicKey.Add(crv, iLstCRTable);

                            crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Control";
                            dicAPV.Add(dicKey.ToList()[0], crNew);
                            iLstCRTable++;
                        }

                    }
                }
                Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicAPV_Sum = new Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>();
                foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                {
                    foreach (CRCalculateValue crv in cr.CRCalculateValues)
                    {
                        dicKey = null;
                        dicKey = new Dictionary<CRCalculateValue, int>();
                        dicKey.Add(crv, iLstCRTable);
                        dicAPV.Add(dicKey.ToList()[0], cr.CRSelectFunction);
                        dicAPV_Sum.Add(ConfigurationCommonClass.getKeyValuePairDeepCopy(dicKey.ToList()[0]), cr.CRSelectFunction);
                    }
                    iLstCRTable++;
                }

                if (isSumChecked ) // && dicAPV.Keys.First().Key.BetaVariationName.ToLower() != "full year")
                {
                    // dictionary in the structure the table needs
                    Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicTempResults = new Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>();
                    // dictionary in a format we can use to collect by row, col, season. This dic will reference the same objects as dicTempResults and won't be needed after this process
                    Dictionary<string, Object> dicRowColSeasonLookup = new Dictionary<string, Object>();

                    foreach (KeyValuePair<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> kvp in dicAPV_Sum)
                    {
                        string keyRowColSeason = kvp.Key.Key.Row + "-" + kvp.Key.Key.Col + "-" + kvp.Key.Key.BetaName;
                        if (dicRowColSeasonLookup.ContainsKey(keyRowColSeason))
                        {
                            // Just add to it then
                            KeyValuePair<CRCalculateValue, int> k = (KeyValuePair<CRCalculateValue, int>)dicRowColSeasonLookup[keyRowColSeason];
                            k.Key.PointEstimate += kvp.Key.Key.PointEstimate;

                            int percentileIndex = 0;
                            foreach (float percentile in kvp.Key.Key.LstPercentile)
                            {
                                k.Key.LstPercentile[percentileIndex] += percentile;
                                percentileIndex++;

                            }
                        }
                        else
                        {
                            // Create a deep copy
                            KeyValuePair<CRCalculateValue, int> k = kvp.Key;
                            CRCalculateValue deepCopy = ConfigurationCommonClass.getKeyValuePairDeepCopy(k).Key;
                            KeyValuePair<CRCalculateValue, int> newPair = new KeyValuePair<CRCalculateValue, int>(deepCopy, k.Value);
                            dicRowColSeasonLookup.Add(keyRowColSeason, newPair);
                            dicTempResults.Add(newPair, kvp.Value);
                        }
                    }

                    dicAPV_Sum.Clear();
                    dicAPV_Sum = dicTempResults;

                    if (_pageCurrent == _pageCount)
                        OLVResultsShow.SetObjects(dicAPV_Sum.ToList().GetRange(_pageCurrent * 50 - 50, dicAPV_Sum.Count - (_pageCurrent - 1) * 50));
                    else
                        OLVResultsShow.SetObjects(dicAPV_Sum.ToList().GetRange(_pageCurrent * 50 - 50, 50));
                }

                else
                {

                    if (_pageCurrent == _pageCount)
                        OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, dicAPV.Count - (_pageCurrent - 1) * 50));
                    else
                        OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, 50));
                }

            }
            else if (oTable is Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>)
            {
                Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicAPV = oTable as Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>;
                if (_pageCurrent == _pageCount)

                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, dicAPV.Count - (_pageCurrent - 1) * 50));
                else
                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, 50));
            }


            else if (oTable is List<AllSelectValuationMethodAndValue> || oTable is AllSelectValuationMethodAndValue)
            {
                List<AllSelectValuationMethodAndValue> lstallSelectValuationMethodAndValue = new List<AllSelectValuationMethodAndValue>();
                if (oTable is List<AllSelectValuationMethodAndValue>)
                {
                    lstallSelectValuationMethodAndValue = (List<AllSelectValuationMethodAndValue>)oTable;
                }
                else
                {
                    lstallSelectValuationMethodAndValue.Add((AllSelectValuationMethodAndValue)oTable);
                }
                Dictionary<KeyValuePair<APVValueAttribute, int>, AllSelectValuationMethod> dicAPV = new Dictionary<KeyValuePair<APVValueAttribute, int>, AllSelectValuationMethod>();
                Dictionary<APVValueAttribute, int> dicLstAllSelectValuationMethodAndValue = new Dictionary<APVValueAttribute, int>();
                int ilstallSelectValuationMethodAndValue = 0;
                foreach (AllSelectValuationMethodAndValue allSelectValuationMethodAndValue in lstallSelectValuationMethodAndValue)
                {
                    foreach (APVValueAttribute apvx in allSelectValuationMethodAndValue.lstAPVValueAttributes)
                    {
                        dicLstAllSelectValuationMethodAndValue = null;
                        dicLstAllSelectValuationMethodAndValue = new Dictionary<APVValueAttribute, int>();
                        dicLstAllSelectValuationMethodAndValue.Add(apvx, ilstallSelectValuationMethodAndValue);
                        dicAPV.Add(dicLstAllSelectValuationMethodAndValue.ToList()[0], allSelectValuationMethodAndValue.AllSelectValuationMethod);
                    }
                    ilstallSelectValuationMethodAndValue++;
                }
                if (_pageCurrent == _pageCount)

                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, dicAPV.ToList().Count - (_pageCurrent - 1) * 50));
                else
                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, 50));
            }
            else if (oTable is AllSelectQALYMethodAndValue)
            {
                AllSelectQALYMethodAndValue allSelectQALYMethodAndValue = (AllSelectQALYMethodAndValue)oTable;


                Dictionary<QALYValueAttribute, AllSelectQALYMethod> dicAPV = new Dictionary<QALYValueAttribute, AllSelectQALYMethod>();

                foreach (QALYValueAttribute apvx in allSelectQALYMethodAndValue.lstQALYValueAttributes)
                {
                    dicAPV.Add(apvx, allSelectQALYMethodAndValue.AllSelectQALYMethod);
                }


                if (_pageCurrent == _pageCount)

                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, dicAPV.ToList().Count - (_pageCurrent - 1) * 50));
                else
                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, 50));
            }
            else if (oTable is List<AllSelectQALYMethodAndValue>)
            {
                List<AllSelectQALYMethodAndValue> lstAllSelectQALYMethodAndValue = (List<AllSelectQALYMethodAndValue>)oTable;

                Dictionary<QALYValueAttribute, AllSelectQALYMethod> dicAPV = new Dictionary<QALYValueAttribute, AllSelectQALYMethod>();
                foreach (AllSelectQALYMethodAndValue allSelectQALYMethodAndValue in lstAllSelectQALYMethodAndValue)
                {
                    foreach (QALYValueAttribute apvx in allSelectQALYMethodAndValue.lstQALYValueAttributes)
                    {
                        dicAPV.Add(apvx, allSelectQALYMethodAndValue.AllSelectQALYMethod);
                    }
                }

                if (_pageCurrent == _pageCount)

                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, dicAPV.ToList().Count - (_pageCurrent - 1) * 50));
                else
                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, 50));
            }
            else if (oTable is BenMAPLine)
            {
                BenMAPLine crTable = (BenMAPLine)oTable;

                if (_pageCurrent == _pageCount)

                    OLVResultsShow.SetObjects(crTable.ModelResultAttributes.GetRange(_pageCurrent * 50 - 50, crTable.ModelResultAttributes.Count - (_pageCurrent - 1) * 50));
                else
                    OLVResultsShow.SetObjects(crTable.ModelResultAttributes.GetRange(_pageCurrent * 50 - 50, 50));
            }
        }

        private void btnResultShow_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                if (e.ClickedItem == null) { return; }
                string tag = e.ClickedItem.Name.ToString();
                Console.WriteLine(tag);
                switch (tag)
                {
                    case "bindingNavigatorMoveFirstItem":
                        _pageCurrent = 1;
                        _currentRow = 0;
                        UpdateTableResult(_tableObject);
                        break;
                    case "bindingNavigatorMovePreviousItem":
                        _pageCurrent--;
                        if (_pageCurrent <= 0)
                        {
                            _pageCurrent = 1;
                            return;
                        }
                        else
                        {
                            _currentRow = _pageSize * (_pageCurrent - 1);
                        }
                        UpdateTableResult(_tableObject);
                        break;
                    case "bindingNavigatorMoveNextItem":
                        _pageCurrent++;
                        if (_pageCurrent > _pageCount)
                        {
                            _pageCurrent = _pageCount;
                            return;
                        }
                        else
                        {
                            _currentRow = _pageSize * (_pageCurrent - 1);
                        }
                        UpdateTableResult(_tableObject);
                        break;
                    case "bindingNavigatorMoveLastItem":
                        _pageCurrent = _pageCount;
                        _currentRow = _pageSize * (_pageCurrent - 1);
                        UpdateTableResult(_tableObject);
                        break;
                }
                bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void bindingNavigatorPositionItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    _pageCurrent = Convert.ToInt32(bindingNavigatorPositionItem.Text);
                    _currentRow = _pageSize * (_pageCurrent - 1);
                    UpdateTableResult(_tableObject);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        private void btShowIncidencePooling_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbPoolingWindowAPV.Items.Count == 0)
                    return;
                ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == cbPoolingWindowAPV.Items[cbPoolingWindowAPV.SelectedIndex].ToString()).First();
                if (chbAPVAggregation.Checked)
                {
                    if (vb.IncidencePoolingResultAggregation == null)
                    {
                        MessageBox.Show("No aggregated result.");
                        return;
                    }
                }
                else
                {
                    if (vb.IncidencePoolingAndAggregation == null)
                    {
                        MessageBox.Show("No result.");
                        return;
                    }
                }

                if (vb.IncidencePoolingResult != null && vb.IncidencePoolingResult.CRCalculateValues != null
                    && vb.IncidencePoolingResult.CRCalculateValues.Count > 0)
                {
                    bool bGIS = true;
                    bool bTable = true;
                    bool bChart = true;
                    WaitShow("Creating pooled incidence results");
                    CRSelectFunctionCalculateValue crSelectFunctionCalculateValue = vb.IncidencePoolingResult;
                    if (chbAPVAggregation.Checked)
                    {
                        crSelectFunctionCalculateValue = vb.IncidencePoolingResultAggregation;
                    }
                    if (this.rbShowActiveAPV.Checked)
                    {
                        if (tabCtlMain.SelectedIndex == 0)
                        {
                            bTable = false;
                            bChart = false;
                        }
                        else if (tabCtlMain.SelectedIndex == 1)
                        {
                            bGIS = false;
                            bChart = false;
                        }
                        else if (tabCtlMain.SelectedIndex == 2)
                        {
                            bGIS = false;
                            bTable = false;
                        }
                    }
                    if (bChart)
                    {
                        InitChartResult(crSelectFunctionCalculateValue, CommonClass.GBenMAPGrid.GridDefinitionID);
                    }
                    if (bTable)
                    {
                        InitTableResult(crSelectFunctionCalculateValue);
                    }

                    if (bGIS)
                    {
                        //set change projection text
                        string changeProjText = "change projection to setup projection";
                        if (!String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
                        {
                            changeProjText = changeProjText + " (" + CommonClass.MainSetup.SetupProjection + ")";
                        }
                        tsbChangeProjection.Text = changeProjText;

                        mainMap.ProjectionModeReproject = ActionMode.Never;
                        mainMap.ProjectionModeDefine = ActionMode.Never;
                        string shapeFileName = "";
                        if (chbAPVAggregation.Checked)
                        {
                            if (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation is ShapefileGrid)
                            {
                                //mainMap.Layers.Clear();
                                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation as ShapefileGrid).ShapefileName + ".shp"))
                                {
                                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation as ShapefileGrid).ShapefileName + ".shp";
                                }
                            }
                            else if (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation is RegularGrid)
                            {
                                //mainMap.Layers.Clear();
                                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation as RegularGrid).ShapefileName + ".shp"))
                                {
                                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation as RegularGrid).ShapefileName + ".shp";
                                }
                            }
                        }
                        else
                        {
                            if (CommonClass.GBenMAPGrid is ShapefileGrid)
                            {
                                //mainMap.Layers.Clear();
                                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                                {
                                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
                                }
                            }
                            else if (CommonClass.GBenMAPGrid is RegularGrid)
                            {
                                //mainMap.Layers.Clear();
                                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp"))
                                {
                                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp";
                                }
                            }
                        }

                        IFeatureSet fs = FeatureSet.Open(shapeFileName);

                        int j = 0;
                        int iCol = 0;
                        int iRow = 0;
                        List<string> lstRemoveName = new List<string>();
                        while (j < fs.DataTable.Columns.Count)
                        {
                            if (fs.DataTable.Columns[j].ColumnName.ToLower() == "col") iCol = j;
                            if (fs.DataTable.Columns[j].ColumnName.ToLower() == "row") iRow = j;

                            j++;
                        }
                        j = 0;

                        while (j < fs.DataTable.Columns.Count)
                        {
                            if (fs.DataTable.Columns[j].ColumnName.ToLower() == "col" || fs.DataTable.Columns[j].ColumnName.ToLower() == "row")
                            { }
                            else
                                lstRemoveName.Add(fs.DataTable.Columns[j].ColumnName);

                            j++;
                        }
                        foreach (string s in lstRemoveName)
                        {
                            fs.DataTable.Columns.Remove(s);
                        }
                        fs.DataTable.Columns.Add("Value", typeof(double));
                        j = 0;
                        while (j < fs.DataTable.Columns.Count)
                        {
                            if (fs.DataTable.Columns[j].ColumnName.ToLower() == "col") iCol = j;
                            if (fs.DataTable.Columns[j].ColumnName.ToLower() == "row") iRow = j;

                            j++;
                        }
                        j = 0;
                        Dictionary<string, double> dicAll = new Dictionary<string, double>();
                        foreach (CRCalculateValue crcv in crSelectFunctionCalculateValue.CRCalculateValues)
                        {
                            dicAll.Add(crcv.Col + "," + crcv.Row, crcv.PointEstimate);
                        }
                        foreach (DataRow dr in fs.DataTable.Rows)
                        {
                            try
                            {
                                if (dicAll.ContainsKey(dr[iCol] + "," + dr[iRow]))
                                    dr["Value"] = dicAll[dr[iCol] + "," + dr[iRow]];
                                else
                                    dr["Value"] = 0.0;
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        if (File.Exists(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp");
                        fs.SaveAs(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp", true);
                        
                        //mainMap.Layers.Clear();
                        
                        MapPolygonLayer polLayer = (MapPolygonLayer)mainMap.Layers.Add(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp");
                        //MapPolygonLayer polLayer = mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer;
                        string strValueField = polLayer.DataSet.DataTable.Columns[polLayer.DataSet.DataTable.Columns.Count - 1].ColumnName;
                       
                        _columnName = strValueField;
                        polLayer.Symbology = CreateResultPolyScheme(ref polLayer, 6, "IP"); //-MCB added

                        double dMinValue = 0.0;
                        double dMaxValue = 0.0;
                        dMinValue = crSelectFunctionCalculateValue.CRCalculateValues.Min(a => a.PointEstimate);
                        dMaxValue = crSelectFunctionCalculateValue.CRCalculateValues.Max(a => a.PointEstimate);
                        _dMinValue = dMinValue;
                        _dMaxValue = dMaxValue;
                        //_currentLayerIndex = mainMap.Layers.Count - 1;
                        _CurrentIMapLayer = polLayer;
                        _columnName = strValueField;
                        _CurrentMapTitle = CommonClass.MainSetup.SetupName + " Setup: Pooled Incidence- " + strValueField; 
                        RenderMainMap(true, "IP");

                        addRegionLayerGroupToMainMap();
                        int result = EnforceLegendOrder();
                    }
                    WaitClose();
                }
            }
            catch
            {
                WaitClose();
            }
        }

        private void tabCtlReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabCtlMain.SelectedIndex == 3 && tabCtlReport.SelectedIndex < 5)
            {
                tabCtlMain.SelectedIndex = 0;
            }
            if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabPoolingIncidence")
            {
               // olvIncidence.SelectAll();
               // _IncidenceDragged = true;
               //tlvIncidence_DoubleClick(sender, e);
                _IncidenceDragged = false;
            }
            if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabAPVResultGISShow")
            {
                // tlvAPVResult.SelectAll();
                // _APVdragged = true;
                // tlvAPVResult_DoubleClick(sender, e);
                 _APVdragged = false;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openfile = new OpenFileDialog();
                openfile.InitialDirectory = CommonClass.ResultFilePath;
                openfile.Filter = "AQG file(*.aqgx)|*.aqgx|CFG file(*.cfgx)|*.cfgx|CFGR file(*.cfgrx)|*.cfgrx|APV file(*.apvx)|*.apvx|APVR file(*.apvrx)|*.apvrx";
                openfile.FilterIndex = 1;
                openfile.RestoreDirectory = true;
                if (openfile.ShowDialog() != DialogResult.OK)
                { return; }
                txtExistingConfiguration.Text = openfile.FileName;
            }
            catch (Exception ex)
            { Logger.LogError(ex); }
        }
        private void btShowAudit_Click(object sender, EventArgs e)
        {
            try
            {
                About about = new About();
                tabCtlMain.SelectedIndex = 3;
                List<TreeNode> lstTmp = new List<TreeNode>();
                if (rbAuditFile.Checked)
                {
                    string filePath = txtExistingConfiguration.Text;
                    string fileType = Path.GetExtension(txtExistingConfiguration.Text);
                    switch (fileType)
                    {
                        case ".aqgx":
                            BenMAPLine aqgBenMAPLine = new BenMAPLine();
                            string err = "";
                            aqgBenMAPLine = DataSourceCommonClass.LoadAQGFile(filePath, ref err);
                            if (aqgBenMAPLine == null)
                            {
                                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                                {
                                    try
                                    {
                                        aqgBenMAPLine = Serializer.Deserialize<BenMAPLine>(fs);
                                        fs.Close();
                                        fs.Dispose();
                                    }
                                    catch
                                    {
                                        fs.Close();
                                        fs.Dispose();
                                    }
                                }
                            }
                            TreeNode aqgTreeNode = new TreeNode();
                            aqgTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBenMAPLine(aqgBenMAPLine);
                            TreeNode tnVersion = new TreeNode();
                            tnVersion.Text = aqgBenMAPLine.Version == null ? "BenMAP-CE" : aqgBenMAPLine.Version;
                            lstTmp.Add(tnVersion);
                            lstTmp.Add(aqgTreeNode);
                            break;
                        case ".cfgx":
                            BaseControlCRSelectFunction cfgFunction = new BaseControlCRSelectFunction();
                            err = "";
                            cfgFunction = ConfigurationCommonClass.loadCFGFile(filePath, ref err);
                            if (cfgFunction == null)
                            {
                                BaseControlCRSelectFunction baseControlCRSelectFunction = null;
                                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                                {
                                    try
                                    {
                                        cfgFunction = Serializer.Deserialize<BaseControlCRSelectFunction>(fs);
                                        fs.Close();
                                        fs.Dispose();
                                    }
                                    catch (Exception ex)
                                    {
                                        fs.Close();
                                        fs.Dispose();
                                    }
                                }
                            }
                            TreeNode cfgTreeNode = new TreeNode();
                            cfgTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunction(cfgFunction);
                            tnVersion = new TreeNode();
                            tnVersion.Text = cfgFunction.Version == null ? "BenMAP-CE" : cfgFunction.Version;
                            lstTmp.Add(tnVersion);
                            lstTmp.Add(cfgTreeNode);
                            break;
                        case ".cfgrx":
                            BaseControlCRSelectFunctionCalculateValue cfgrFunctionCV = new BaseControlCRSelectFunctionCalculateValue();
                            err = "";
                            cfgrFunctionCV = ConfigurationCommonClass.LoadCFGRFile(filePath, ref err);
                            if (cfgrFunctionCV == null)
                            {
                                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                                {
                                    try
                                    {
                                        cfgrFunctionCV = Serializer.Deserialize<BaseControlCRSelectFunctionCalculateValue>(fs);
                                        fs.Close();
                                        fs.Dispose();
                                    }
                                    catch
                                    {
                                        fs.Close();
                                        fs.Dispose();
                                    }
                                }
                            }
                            TreeNode cfgrTreeNode = new TreeNode();
                            cfgrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunctionCalculateValue(cfgrFunctionCV);
                            tnVersion = new TreeNode();
                            tnVersion.Text = cfgrFunctionCV.Version == null ? "BenMAP-CE" : cfgrFunctionCV.Version;
                            lstTmp.Add(tnVersion);
                            lstTmp.Add(cfgrTreeNode);
                            break;
                        case ".apvx":
                            ValuationMethodPoolingAndAggregation apvVMPA = new ValuationMethodPoolingAndAggregation();
                            err = "";
                            apvVMPA = APVX.APVCommonClass.loadAPVRFile(filePath, ref err);
                            if (apvVMPA == null)
                            {
                                ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation = null;
                                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                                {
                                    try
                                    {
                                        valuationMethodPoolingAndAggregation = Serializer.Deserialize<ValuationMethodPoolingAndAggregation>(fs);
                                    }
                                    catch
                                    {
                                        fs.Close();
                                        fs.Dispose();
                                        FileStream fsSec = new FileStream(filePath, FileMode.Open);
                                        valuationMethodPoolingAndAggregation = Serializer.DeserializeWithLengthPrefix<ValuationMethodPoolingAndAggregation>(fsSec, PrefixStyle.Fixed32);
                                        fsSec.Close();
                                        fsSec.Dispose();
                                    }
                                    fs.Close();
                                    fs.Dispose();
                                }
                                apvVMPA = valuationMethodPoolingAndAggregation;
                            }
                            TreeNode apvTreeNode = new TreeNode();
                            apvTreeNode = AuditTrailReportCommonClass.getTreeNodeFromValuationMethodPoolingAndAggregation(apvVMPA);
                            tnVersion = new TreeNode();
                            tnVersion.Text = apvVMPA.Version == null ? "BenMAP-CE" : apvVMPA.Version;
                            lstTmp.Add(tnVersion);
                            lstTmp.Add(apvTreeNode);
                            break;
                        case ".apvrx":
                            ValuationMethodPoolingAndAggregation apvrVMPA = new ValuationMethodPoolingAndAggregation();
                            err = "";
                            apvrVMPA = APVX.APVCommonClass.loadAPVRFile(filePath, ref err);
                            if (apvrVMPA == null)
                            {
                                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                                {
                                    try
                                    {
                                        apvrVMPA = Serializer.Deserialize<ValuationMethodPoolingAndAggregation>(fs);
                                    }
                                    catch
                                    {
                                        fs.Close();
                                        fs.Dispose();
                                        FileStream fsSec = new FileStream(filePath, FileMode.Open);
                                        apvrVMPA = Serializer.DeserializeWithLengthPrefix<ValuationMethodPoolingAndAggregation>(fsSec, PrefixStyle.Fixed32);
                                        fsSec.Close();
                                        fsSec.Dispose();
                                    }
                                    fs.Close();
                                    fs.Dispose();
                                }
                            }
                            TreeNode apvrTreeNode = new TreeNode();
                            apvrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromValuationMethodPoolingAndAggregation(apvrVMPA);
                            tnVersion = new TreeNode();
                            tnVersion.Text = apvrVMPA.Version == null ? "BenMAP-CE" : apvrVMPA.Version;
                            lstTmp.Add(tnVersion);
                            lstTmp.Add(apvrTreeNode);
                            break;
                    }
                }
                else if (rbAuditCurrent.Checked)
                {
                    if (CommonClass.ValuationMethodPoolingAndAggregation != null)
                    {
                        ValuationMethodPoolingAndAggregation apvrVMPA = new ValuationMethodPoolingAndAggregation();
                        apvrVMPA = CommonClass.ValuationMethodPoolingAndAggregation;
                        TreeNode apvrTreeNode = new TreeNode();
                        apvrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromValuationMethodPoolingAndAggregation(apvrVMPA);
                        TreeNode tnVersion = new TreeNode();
                        tnVersion.Text = apvrVMPA.Version == null ? "BenMAP-CE" : apvrVMPA.Version;
                        lstTmp.Add(tnVersion);
                        lstTmp.Add(apvrTreeNode);
                    }
                    else if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null)
                    {
                        BaseControlCRSelectFunctionCalculateValue cfgrFunctionCV = new BaseControlCRSelectFunctionCalculateValue();
                        cfgrFunctionCV = CommonClass.BaseControlCRSelectFunctionCalculateValue;
                        TreeNode cfgrTreeNode = new TreeNode();
                        cfgrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunctionCalculateValue(cfgrFunctionCV);
                        TreeNode tnVersion = new TreeNode();
                        tnVersion.Text = cfgrFunctionCV.Version == null ? "BenMAP-CE" : cfgrFunctionCV.Version;
                        lstTmp.Add(tnVersion);
                        lstTmp.Add(cfgrTreeNode);
                    }
                    else if (CommonClass.BaseControlCRSelectFunction != null)
                    {
                        BaseControlCRSelectFunction cfgFunction = new BaseControlCRSelectFunction();
                        cfgFunction = CommonClass.BaseControlCRSelectFunction;
                        TreeNode cfgTreeNode = new TreeNode();
                        cfgTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunction(cfgFunction);
                        TreeNode tnVersion = new TreeNode();
                        tnVersion.Text = cfgFunction.Version == null ? "BenMAP-CE" : cfgFunction.Version;
                        lstTmp.Add(tnVersion);
                        lstTmp.Add(cfgTreeNode);
                    }
                    else
                    {
                        MessageBox.Show("Please finish your configuration first.");
                        return;
                    }
                }
                treeListView.Objects = lstTmp;
                treeListView.ExpandAll();
				
            }
            catch (Exception ex)
            {
                MessageBox.Show("BenMAP-CE was unable to open the file. The file may be corrupt, or it may have been created using a previous incompatible version of BenMAP-CE.");
                Logger.LogError(ex.Message);
            }
        }
        //TODO: Get Metadata for the autid trail
        /// <summary>
        /// Gets the metadata of the dataset used for audit trail.
        /// NOT YET COMPLETED
        /// passing in the tree view that will be added to.
        /// </summary>
        /// <param name="trv">The TRV.</param>
        private void getMetadataForAuditTrail(TreeView trv)
        {
            string datasetTypeName = string.Empty;
            string datasetName = string.Empty;
            TreeNode tnTemp = null;
            TreeNode tnDatasetTypeName = null;//i.e. Monitor
            TreeNode tnDataSetName = null;//i.e. MDS1
            TreeNode tnDataFileName = null;//i.e. DetroitMonitors PM 25
            TreeNode tnMetadata = new TreeNode();//Top node - Datasts
            tnMetadata.Text = "Datasets";
            tnMetadata.Name = "Datasets";
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            System.Data.DataSet ds = new System.Data.DataSet();
            string commandText = string.Empty;
            commandText = string.Format("SELECT a.METADATAID, a.SETUPID, a.DATASETID, a.DATASETTYPEID, a.FILENAME, a.EXTENSION, a.DATAREFERENCE, a.FILEDATE, " +
                                        "a.IMPORTDATE, a.DESCRIPTION, a.PROJECTION, a.GEONAME, a.DATUMNAME, a.DATUMTYPE, a.SPHEROIDNAME, a.MERIDIANNAME, " +
                                        "a.UNITNAME, a.PROJ4STRING, a.NUMBEROFFEATURES, a.METADATAENTRYID " +
                                        "FROM METADATAINFORMATION a " +
                                        "where setupid = {0} " +
                                        "order by a.DATASETTYPEID", CommonClass.MainSetup.SetupID);
            ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                //get the dataset ID, with the dataset ID get the dataset name
                commandText = string.Format("SELECT DATASETTYPENAME FROM DATASETTYPES WHERE DATASETTYPEID = {0}", dr["DATASETTYPEID"].ToString());
                object temp = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                datasetTypeName = temp.ToString();

                if(!tnMetadata.Nodes.ContainsKey(datasetTypeName))//its new
                {
                    tnDatasetTypeName = new TreeNode();
                    tnDatasetTypeName.Name = datasetTypeName;
                    tnDatasetTypeName.Text = datasetTypeName;

                    tnMetadata.Nodes.Add(tnDatasetTypeName);
                    //Get the Datasets names
                    datasetName = getDatasetEntryName(datasetTypeName, Convert.ToInt32(dr["SETUPID"].ToString()), Convert.ToInt32(dr["DATASETID"].ToString()));
                    if(!string.IsNullOrEmpty(datasetName))
                    {
                        tnTemp = tnMetadata.Nodes[datasetTypeName];
                        if (!tnTemp.Nodes.ContainsKey(datasetName))
                        {
                            tnDataSetName = new TreeNode();
                            tnDataSetName.Name = datasetName;
                            tnDataSetName.Text = datasetName;
                            tnDatasetTypeName.Nodes.Add(tnDataSetName);
                            
                            DataRow[] drs = ds.Tables[0].Select(string.Format("DATASETID = {0} AND DATASETTYPEID = {1}", Convert.ToInt32(dr["DATASETID"].ToString()), Convert.ToInt32(dr["DATASETTYPEID"].ToString())));
                            foreach (DataRow drMetadata in drs)
                            {
                                tnDataFileName = new TreeNode();//file level
                                tnDataFileName.Name = drMetadata["FILENAME"].ToString();
                                tnDataFileName.Text = string.Format("File Name: {0}", drMetadata["FILENAME"].ToString());
                                tnDataSetName.Nodes.Add(tnDataFileName);
                                //now getting the metadata and placeing it under the tnDataFileName
                                tnTemp = new TreeNode();
                                tnTemp.Name = drMetadata["EXTENSION"].ToString();
                                tnTemp.Text = string.Format("Extension: {0}", drMetadata["EXTENSION"].ToString());
                                tnDataFileName.Nodes.Add(tnTemp);
                                tnTemp = new TreeNode();
                                tnTemp.Name = drMetadata["DATAREFERENCE"].ToString();
                                tnTemp.Text = string.Format("Data Reference: {0}", drMetadata["DATAREFERENCE"].ToString());
                                tnDataFileName.Nodes.Add(tnTemp);
                                tnTemp = new TreeNode();
                                tnTemp.Name = "FILEDATE"; 
                                tnTemp.Text = string.Format("File Date: {0}", drMetadata["FILEDATE"].ToString());
                                tnDataFileName.Nodes.Add(tnTemp);
                                tnTemp = new TreeNode();
                                tnTemp.Name = "IMPORTDATE"; 
                                tnTemp.Text = string.Format("Import Date: {0}", drMetadata["IMPORTDATE"].ToString());
                                tnDataFileName.Nodes.Add(tnTemp);
                                tnTemp = new TreeNode();
                                tnTemp.Name = "DESCRIPTION"; 
                                tnTemp.Text = string.Format("Description: {0}", drMetadata["DESCRIPTION"].ToString());
                                tnDataFileName.Nodes.Add(tnTemp);
                                if(drMetadata["EXTENSION"].ToString().ToLower().Equals(".shp"))
                                {
                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "PROJECTION"; 
                                    tnTemp.Text = string.Format("Projection: {0}", drMetadata["PROJECTION"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);

                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "GEONAME"; 
                                    tnTemp.Text = string.Format("Geoname: {0}", drMetadata["GEONAME"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);

                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "DATUMNAME"; 
                                    tnTemp.Text = string.Format("Datum Name: {0}", drMetadata["DATUMNAME"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);

                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "DATUMTYPE"; 
                                    tnTemp.Text = string.Format("Datumtype: {0}", drMetadata["DATUMTYPE"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);

                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "SPHEROIDNAME"; 
                                    tnTemp.Text = string.Format("Spheroid Name: {0}", drMetadata["SPHEROIDNAME"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);

                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "MERIDIANNAME"; 
                                    tnTemp.Text = string.Format("Meridian Name: {0}", drMetadata["MERIDIANNAME"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);

                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "UNITNAME"; 
                                    tnTemp.Text = string.Format("Unit Name: {0}", drMetadata["UNITNAME"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);

                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "PROJ4STRING"; 
                                    tnTemp.Text = string.Format("PROJ4STRING: {0}", drMetadata["PROJ4STRING"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);

                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "NUMBEROFFEATURES";
                                    tnTemp.Text = string.Format("Number Of Features: {0}", drMetadata["NUMBEROFFEATURES"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);
                                }
                            }
                        }
                    }
                }
                else
                {
                    tnDatasetTypeName = tnMetadata.Nodes[datasetTypeName];
                    datasetName = getDatasetEntryName(datasetTypeName, Convert.ToInt32(dr["SETUPID"].ToString()), Convert.ToInt32(dr["DATASETID"].ToString()));
                    if(!string.IsNullOrEmpty(datasetName))
                    {
                        if (!tnDatasetTypeName.Nodes.ContainsKey(datasetName))
                        {
                            tnDataSetName = new TreeNode();
                            tnDataSetName.Name = datasetName;
                            tnDataSetName.Text = datasetName;
                            tnDatasetTypeName.Nodes.Add(tnDataSetName);

                            DataRow[] drs = ds.Tables[0].Select(string.Format("DATASETID = {0} AND DATASETTYPEID = {1}", Convert.ToInt32(dr["DATASETID"].ToString()), Convert.ToInt32(dr["DATASETTYPEID"].ToString())));
                            foreach (DataRow drMetadata in drs)
                            {
                                tnDataFileName = new TreeNode();//file level
                                tnDataFileName.Name = drMetadata["FILENAME"].ToString();
                                tnDataFileName.Text = string.Format("File Name: {0}", drMetadata["FILENAME"].ToString());
                                tnDataSetName.Nodes.Add(tnDataFileName);
                                //now getting the metadata and placeing it under the tnDataFileName
                                tnTemp = new TreeNode();
                                tnTemp.Name = drMetadata["EXTENSION"].ToString();
                                tnTemp.Text = string.Format("Extension: {0}", drMetadata["EXTENSION"].ToString());
                                tnDataFileName.Nodes.Add(tnTemp);
                                tnTemp = new TreeNode();
                                tnTemp.Name = drMetadata["DATAREFERENCE"].ToString();
                                tnTemp.Text = string.Format("Data Reference: {0}", drMetadata["DATAREFERENCE"].ToString());
                                tnDataFileName.Nodes.Add(tnTemp);
                                tnTemp = new TreeNode();
                                tnTemp.Name = "FILEDATE"; //drMetadata["FILEDATE"].ToString();
                                tnTemp.Text = string.Format("File Date: {0}", drMetadata["FILEDATE"].ToString());
                                tnDataFileName.Nodes.Add(tnTemp);
                                tnTemp = new TreeNode();
                                tnTemp.Name = "IMPORTDATE"; //drMetadata["IMPORTDATE"].ToString();
                                tnTemp.Text = string.Format("Import Date: {0}", drMetadata["IMPORTDATE"].ToString());
                                tnDataFileName.Nodes.Add(tnTemp);
                                tnTemp = new TreeNode();
                                tnTemp.Name = "DESCRIPTION"; //drMetadata["IMPORTDATE"].ToString();
                                tnTemp.Text = string.Format("Description: {0}", drMetadata["DESCRIPTION"].ToString());
                                tnDataFileName.Nodes.Add(tnTemp);

                                if (drMetadata["EXTENSION"].ToString().ToLower().Equals(".shp"))
                                {
                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "PROJECTION";
                                    tnTemp.Text = string.Format("Projection: {0}", drMetadata["PROJECTION"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);

                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "GEONAME";
                                    tnTemp.Text = string.Format("Geoname: {0}", drMetadata["GEONAME"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);

                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "DATUMNAME";
                                    tnTemp.Text = string.Format("Datum Name: {0}", drMetadata["DATUMNAME"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);

                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "DATUMTYPE";
                                    tnTemp.Text = string.Format("Datumtype: {0}", drMetadata["DATUMTYPE"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);

                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "SPHEROIDNAME";
                                    tnTemp.Text = string.Format("Spheroid Name: {0}", drMetadata["SPHEROIDNAME"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);

                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "MERIDIANNAME";
                                    tnTemp.Text = string.Format("Meridian Name: {0}", drMetadata["MERIDIANNAME"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);

                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "UNITNAME";
                                    tnTemp.Text = string.Format("Unit Name: {0}", drMetadata["UNITNAME"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);

                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "PROJ4STRING";
                                    tnTemp.Text = string.Format("PROJ4STRING: {0}", drMetadata["PROJ4STRING"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);

                                    tnTemp = new TreeNode();
                                    tnTemp.Name = "NUMBEROFFEATURES";
                                    tnTemp.Text = string.Format("Number Of Features: {0}", drMetadata["NUMBEROFFEATURES"].ToString());
                                    tnDataFileName.Nodes.Add(tnTemp);
                                }

                            }
                        }
                    }
                }

           
            }
            trv.Nodes.Add(tnMetadata);
        }
        private string getDatasetEntryName(string DatasetTypeName, int setupid, int datasetid)
        {
            string commandText = string.Empty;
            object rtv = null;
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            switch (DatasetTypeName.ToLower())
            {
                case "incidence":
                    commandText = string.Format("select INCIDENCEDATASETNAME from INCIDENCEDATASETS where SETUPID = {0} and INCIDENCEDATASETID = {1}", setupid, datasetid);
                    rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                break;
                case "variabledataset":
                    commandText = string.Format("select SETUPVARIABLEDATASETNAME from SETUPVARIABLEDATASETS where SETUPID = {0} and SETUPVARIABLEDATASETID = {1}", setupid, datasetid);
                    rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                break;
                case "inflation":
                    commandText = string.Format("select INFLATIONDATASETNAME from INFLATIONDATASETS where SETUPID = {0} and INFLATIONDATASETID = {1}", setupid, datasetid);
                    rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                break;
                case "incomegrowth":
                commandText = string.Format("select INCOMEGROWTHADJDATASETNAME from INCOMEGROWTHADJDATASETS where SETUPID = {0} and INCOMEGROWTHADJDATASETID = {1}", setupid, datasetid);
                    rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                break;
                case "healthfunctions":
                    commandText = string.Format("select CRFUNCTIONDATASETNAME from CRFUNCTIONDATASETS where SETUPID = {0} and CRFUNCTIONDATASETID = {1}", setupid, datasetid);
                    rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                break;
                case "valuationfunction":
                    commandText = string.Format("select VALUATIONFUNCTIONDATASETNAME from VALUATIONFUNCTIONDATASETS where SETUPID = {0} and VALUATIONFUNCTIONDATASETID = {1}", setupid, datasetid);
                    rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                break;
                case "population":
                    commandText = string.Format("select POPULATIONDATASETNAME from POPULATIONDATASETS where SETUPID = {0} and POPULATIONDATASETID = {1}", setupid, datasetid);
                    rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                break;
                //case "baseline":

                //break;
                //case "control":

                //break;
                case "griddefinition":
                    commandText = string.Format("select GRIDDEFINITIONNAME from GRIDDEFINITIONS where SETUPID = {0} and GRIDDEFINITIONID = {1}", setupid, datasetid);
                    rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                break;
                case "monitor":
                    commandText = string.Format("select monitordatasetname from monitordatasets where setupid = {0} and monitordatasetid = {1}", setupid, datasetid);
                    rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                 break;

                default:
                    break;
            }

            return rtv.ToString();
        }
        private void rbAuditFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (rbAuditFile.Checked)
                {
                    txtExistingConfiguration.Enabled = true;
                    btnBrowse.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        private void rbAuditCurrent_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (rbAuditCurrent.Checked)
                {
                    txtExistingConfiguration.Enabled = false;
                    btnBrowse.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                GraphPane myPane = this.zedGraphCtl.GraphPane;
                List<string> lstPane = new List<string>();
                myPane.CurveList.Clear();
                int i = 0;
                Color[] colorArray = new Color[] { Color.Blue, Color.Red, Color.Green };
                while (i < 1)
                {
                    PointPairList list = new PointPairList();
                    int j = 0;
                    foreach (ChartResult cr in olvRegions.CheckedObjects)
                    {
                        list.Add(new PointPair(Convert.ToInt32(j), cr.RegionValue));
                        lstPane.Add(cr.RegionName);
                        j++;
                    }
                    BarItem myCurve = myPane.AddBar("Result", list, colorArray[i]);

                    i++;
                }
                myPane.Chart.Fill = new Fill(Color.White,
                 Color.FromArgb(255, 255, 166), 45.0F);


                myPane.XAxis.Scale.TextLabels = lstPane.ToArray(); myPane.XAxis.Type = AxisType.Text;
                myPane.XAxis.Scale.FontSpec.Angle = 65;
                myPane.XAxis.Scale.FontSpec.IsBold = true;
                myPane.XAxis.Scale.FontSpec.Size = 12;
                zedGraphCtl.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
                myPane.IsFontsScaled = false; myPane.YAxis.Scale.MinAuto = true; myPane.YAxis.Scale.MaxAuto = true;
                myPane.XAxis.Scale.MinAuto = true;
                myPane.XAxis.Scale.MaxAuto = true;
                myPane.Title.Text = strchartTitle;
                myPane.XAxis.Title.Text = strchartX;
                myPane.YAxis.Title.Text = strchartY;
                myPane.YAxis.Scale.Format = "#,##0.####";
                zedGraphCtl.AxisChange();
                zedGraphCtl.Refresh();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void cbChartXAxis_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbChartXAxis.Text != chartXAxis)
                {
                    if (cbChartXAxis.Text == "Col/Row")
                    {
                        for (int j = 0; j < CommonClass.lstChartResult.Count; j++)
                        {
                            CommonClass.lstChartResult[j].RegionName = CommonClass.lstChartResult[j].Col.ToString() + "/" + CommonClass.lstChartResult[j].Row.ToString();
                        }
                    }
                    else
                    {
                        string shapefilename = "";
                        if (ChartGrid is ShapefileGrid)
                            shapefilename = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (ChartGrid as ShapefileGrid).ShapefileName + ".shp";
                        else if (CommonClass.RBenMAPGrid is RegularGrid)
                            shapefilename = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (ChartGrid as RegularGrid).ShapefileName + ".shp";
                        DotSpatial.Data.IFeatureSet fs = new DotSpatial.Data.FeatureSet();
                        if (File.Exists(shapefilename))
                        {
                            fs = DotSpatial.Data.FeatureSet.Open(shapefilename);

                            try
                            {
                                for (int iDt = 0; iDt < fs.DataTable.Rows.Count; iDt++)
                                {
                                    fs.DataTable.Rows[iDt]["ROW"] = Convert.ToInt32(fs.DataTable.Rows[iDt]["ROW"].ToString());
                                    fs.DataTable.Rows[iDt]["COL"] = Convert.ToInt32(fs.DataTable.Rows[iDt]["COL"].ToString());

                                }
                            }
                            catch
                            { }
                            foreach (DataColumn dc in fs.DataTable.Columns)
                            {
                                if (dc.ColumnName == cbChartXAxis.Text)
                                {
                                    for (int j = 0; j < CommonClass.lstChartResult.Count; j++)
                                    {
                                        DataRow dr = fs.DataTable.Select("COL='" + CommonClass.lstChartResult[j].Col + "' and ROW='" + CommonClass.lstChartResult[j].Row + "'").First();
                                        CommonClass.lstChartResult[j].RegionName = dr[cbChartXAxis.Text].ToString();
                                    }
                                    break;
                                }
                            }
                            fs.Close();
                            fs.Dispose();
                        }
                    }
                    chartXAxis = cbChartXAxis.Text;
                    List<int> lstChecked = new List<int>();
                    for (int j = 0; j < olvRegions.Items.Count; j++)
                    {
                        if (olvRegions.Items[j].Checked)
                            lstChecked.Add(j);
                    }
                    olvRegions.SetObjects(CommonClass.lstChartResult);
                    for (int j = 0; j < olvRegions.Items.Count; j++)
                    {
                        OLVListItem olvi = olvRegions.Items[j] as OLVListItem;
                        if (lstChecked.Contains(j))
                        {
                            olvi.Checked = true;
                        }
                        else
                            olvi.Checked = false;

                    }
                }
                btnApply_Click(sender, e);
            }
            catch (Exception ex)
            { Logger.LogError(ex); }
        }

        private void TimedFilter(ObjectListView olv, string txt)
        {
            this.TimedFilter(olv, txt, 0);
        }

        private void TimedFilter(ObjectListView olv, string txt, int matchKind)
        {
            TextMatchFilter filter = null;
            if (!String.IsNullOrEmpty(txt))
            {
                switch (matchKind)
                {
                    case 0:
                    default:
                        filter = TextMatchFilter.Contains(olv, txt);
                        break;
                    case 1:
                        filter = TextMatchFilter.Prefix(olv, txt);
                        break;
                    case 2:
                        filter = TextMatchFilter.Regex(olv, txt);
                        break;
                }
            }
            if (filter == null)
                olv.DefaultRenderer = null;
            else
            {
                olv.DefaultRenderer = new HighlightTextRenderer(filter);

                //olv.DefaultRenderer = new HighlightTextRenderer { Filter = filter, UseGdiTextRendering = true };
            }

            // Some lists have renderers already installed
            HighlightTextRenderer highlightingRenderer = olv.GetColumn(0).Renderer as HighlightTextRenderer;
            if (highlightingRenderer != null)
                highlightingRenderer.Filter = filter;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            olv.ModelFilter = filter;
            stopWatch.Stop();
        }

        private void textChartFilter_TextChanged(object sender, EventArgs e)
        {
            this.TimedFilter(this.olvRegions, textChartFilter.Text);
        }
        private List<AllSelectValuationMethod> lstAPVPoolingAndAggregationAll;
        public Dictionary<AllSelectValuationMethod, string> dicAPVPoolingAndAggregation;
        public Dictionary<AllSelectValuationMethod, string> dicAPVPoolingAndAggregationUnPooled;
        public void loadAllAPVPooling()
        {
            try
            {
                dicAPVPoolingAndAggregation = new Dictionary<AllSelectValuationMethod, string>();
                dicAPVPoolingAndAggregationUnPooled = new Dictionary<AllSelectValuationMethod, string>();
                AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = null;
                foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                {

                    var query = vb.LstAllSelectValuationMethod.Where(p => p.PID == -1);
                    foreach (AllSelectValuationMethod allSelectValuationMethod in query)
                    {
                        List<AllSelectValuationMethod> lstShow = new List<AllSelectValuationMethod>();
                        lstShow.Add(allSelectValuationMethod);
                        getChildFromAllSelectValuationMethodUnPooled(allSelectValuationMethod, vb, ref lstShow);
                        foreach (AllSelectValuationMethod avm in lstShow)
                        {
                            if (avm.PoolingMethod != "None")
                            {
                                try
                                {
                                    if (vb.LstAllSelectValuationMethodAndValueAggregation == null || vb.LstAllSelectValuationMethodAndValueAggregation.Count == 0)
                                    {

                                        allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue.Where(p => p.AllSelectValuationMethod.ID == avm.ID).First();

                                    }
                                    else
                                    {

                                        allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValueAggregation.Where(p => p.AllSelectValuationMethod.ID == avm.ID).First();
                                    }
                                    if (allSelectValuationMethodAndValue != null && allSelectValuationMethodAndValue.lstAPVValueAttributes != null && allSelectValuationMethodAndValue.lstAPVValueAttributes.Count() > 0)
                                        dicAPVPoolingAndAggregation.Add(avm, vb.IncidencePoolingAndAggregation.PoolingName);
                                }
                                catch
                                {
                                }

                            }
                        }
                    }

                    foreach (AllSelectValuationMethod avm in vb.LstAllSelectValuationMethod)
                    {
                        if (avm.PoolingMethod == null)
                        {
                            try
                            {
                                if (vb.LstAllSelectValuationMethodAndValueAggregation == null || vb.LstAllSelectValuationMethodAndValueAggregation.Count == 0)
                                {

                                    allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue.Where(p => p.AllSelectValuationMethod.ID == avm.ID).First();

                                }
                                else
                                {

                                    allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValueAggregation.Where(p => p.AllSelectValuationMethod.ID == avm.ID).First();
                                }
                                if (allSelectValuationMethodAndValue != null && allSelectValuationMethodAndValue.lstAPVValueAttributes != null && allSelectValuationMethodAndValue.lstAPVValueAttributes.Count() > 0)
                                    dicAPVPoolingAndAggregationUnPooled.Add(avm, vb.IncidencePoolingAndAggregation.PoolingName);
                            }
                            catch
                            {
                            }

                        }
                    }
                }







            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        private void cbPoolingWindowAPV_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rbAPVAll.Checked)
            {
                loadAllAPVPooling();
                if (this.btShowDetailValuation.Text == "Show aggregated")
                {
                    this.tlvAPVResult.SetObjects(dicAPVPoolingAndAggregation);

                }
                else
                {
                    tlvAPVResult.SetObjects(dicAPVPoolingAndAggregationUnPooled);
                }

                tlvAPVResult.SelectAll();
                if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabAPVResultGISShow")
                {
                    tlvAPVResult_DoubleClick(sender, e);
                }

                return;
            } if (CommonClass.ValuationMethodPoolingAndAggregation == null || CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase == null) return;
            ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == cbPoolingWindowAPV.Items[cbPoolingWindowAPV.SelectedIndex].ToString()).First();

            dicAPVPoolingAndAggregation = new Dictionary<AllSelectValuationMethod, string>();
            dicAPVPoolingAndAggregationUnPooled = new Dictionary<AllSelectValuationMethod, string>();
            AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = null;
            var query = vb.LstAllSelectValuationMethod.Where(p => p.PID == -1);
            foreach (AllSelectValuationMethod allSelectValuationMethod in query)
            {
                List<AllSelectValuationMethod> lstShow = new List<AllSelectValuationMethod>();
                lstShow.Add(allSelectValuationMethod);
                getChildFromAllSelectValuationMethodUnPooled(allSelectValuationMethod, vb, ref lstShow);
                foreach (AllSelectValuationMethod avm in lstShow)
                {
                    if (avm.PoolingMethod != "None")
                    {
                        try
                        {
                            if (vb.LstAllSelectValuationMethodAndValueAggregation == null || vb.LstAllSelectValuationMethodAndValueAggregation.Count == 0)
                            {

                                allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue.Where(p => p.AllSelectValuationMethod.ID == avm.ID).First();

                            }
                            else
                            {

                                allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValueAggregation.Where(p => p.AllSelectValuationMethod.ID == avm.ID).First();
                            }
                            if (allSelectValuationMethodAndValue != null && allSelectValuationMethodAndValue.lstAPVValueAttributes != null && allSelectValuationMethodAndValue.lstAPVValueAttributes.Count() > 0)
                                dicAPVPoolingAndAggregation.Add(avm, vb.IncidencePoolingAndAggregation.PoolingName);
                        }
                        catch
                        {
                        }

                    }
                }
            }

            foreach (AllSelectValuationMethod avm in vb.LstAllSelectValuationMethod)
            {
                if (avm.PoolingMethod == null)
                {
                    try
                    {
                        if (vb.LstAllSelectValuationMethodAndValueAggregation == null || vb.LstAllSelectValuationMethodAndValueAggregation.Count == 0)
                        {

                            allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue.Where(p => p.AllSelectValuationMethod.ID == avm.ID).First();

                        }
                        else
                        {

                            allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValueAggregation.Where(p => p.AllSelectValuationMethod.ID == avm.ID).First();
                        }
                        if (allSelectValuationMethodAndValue != null && allSelectValuationMethodAndValue.lstAPVValueAttributes != null && allSelectValuationMethodAndValue.lstAPVValueAttributes.Count() > 0)
                            dicAPVPoolingAndAggregationUnPooled.Add(avm, vb.IncidencePoolingAndAggregation.PoolingName);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex);
                    }

                }
            }

            if (this.btShowDetailValuation.Text == "Show aggregated")
            {
                this.tlvAPVResult.SetObjects(dicAPVPoolingAndAggregation);

            }
            else
            {
                tlvAPVResult.SetObjects(dicAPVPoolingAndAggregationUnPooled);
            }
            tlvAPVResult.SelectAll();
            if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabAPVResultGISShow")
            {
                tlvAPVResult_DoubleClick(sender, e);
            }


        }
        public Dictionary<AllSelectCRFunction, string> dicIncidencePoolingAndAggregation;
        public Dictionary<AllSelectCRFunction, string> dicIncidencePoolingAndAggregationUnPooled;
        public void LoadAllIncidencePooling(ref Dictionary<AllSelectCRFunction, string> Pooled, ref Dictionary<AllSelectCRFunction, string> UnPooled)
        {
            if (!rbIncidenceAll.Checked) return;
            try
            {
                Pooled = new Dictionary<AllSelectCRFunction, string>();
                UnPooled = new Dictionary<AllSelectCRFunction, string>();
                foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                {

                    var query = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.PID == -1);
                    foreach (AllSelectCRFunction allSelectCRFunction in query)
                    {
                        List<AllSelectCRFunction> lstShow = new List<AllSelectCRFunction>();
                        lstShow.Add(allSelectCRFunction);
                        getChildFromAllSelectCRFunctionUnPooled(allSelectCRFunction, vb, ref lstShow);

                        foreach (AllSelectCRFunction acr in lstShow)
                        {
                            if (acr.PoolingMethod != "None")
                            {
                                Pooled.Add(acr, vb.IncidencePoolingAndAggregation.PoolingName);
                            }
                        }
                    }
                    foreach (AllSelectCRFunction acr in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion)
                    {
                        if (acr.PoolingMethod == "")
                        {
                            UnPooled.Add(acr, vb.IncidencePoolingAndAggregation.PoolingName);
                        }

                    }




                }




            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

        }
        private void cbPoolingWindowIncidence_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rbIncidenceAll.Checked)
            {
                LoadAllIncidencePooling(ref dicIncidencePoolingAndAggregation, ref dicIncidencePoolingAndAggregationUnPooled);
                if (btShowDetailIncidence.Text == "Show aggregated")
                {
                    olvIncidence.SetObjects(dicIncidencePoolingAndAggregation);

                }
                else
                {
                    olvIncidence.SetObjects(dicIncidencePoolingAndAggregationUnPooled);
                }
                olvIncidence.SelectAll();
                if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabPoolingIncidence")
                {

                    tlvIncidence_DoubleClick(sender, e);
                }

                return;
            }
            if (CommonClass.ValuationMethodPoolingAndAggregation == null || CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase == null) return;
            ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == cbPoolingWindowAPV.Items[cbPoolingWindowIncidence.SelectedIndex].ToString()).First();
            dicIncidencePoolingAndAggregation = new Dictionary<AllSelectCRFunction, string>();
            dicIncidencePoolingAndAggregationUnPooled = new Dictionary<AllSelectCRFunction, string>();
            var query = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.PID == -1);
            foreach (AllSelectCRFunction allSelectCRFunction in query)
            {
                List<AllSelectCRFunction> lstShow = new List<AllSelectCRFunction>();
                lstShow.Add(allSelectCRFunction);
                getChildFromAllSelectCRFunctionUnPooled(allSelectCRFunction, vb, ref lstShow);

                foreach (AllSelectCRFunction acr in lstShow)
                {
                    if (acr.PoolingMethod != "None")
                    {
                        dicIncidencePoolingAndAggregation.Add(acr, vb.IncidencePoolingAndAggregation.PoolingName);
                    }
                }
            }
            foreach (AllSelectCRFunction acr in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion)
            {
                if (acr.PoolingMethod == "")
                {
                    dicIncidencePoolingAndAggregationUnPooled.Add(acr, vb.IncidencePoolingAndAggregation.PoolingName);
                }

            }
            if (btShowDetailIncidence.Text == "Show aggregated")
            {
                olvIncidence.SetObjects(dicIncidencePoolingAndAggregation);

            }
            else
            {
                olvIncidence.SetObjects(dicIncidencePoolingAndAggregationUnPooled);
            }
            olvIncidence.SelectAll();
            if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabPoolingIncidence")
            {
                tlvIncidence_DoubleClick(sender, e);
            }


        }
        Dictionary<AllSelectQALYMethod, string> dicQALYPoolingAndAggregation;
        Dictionary<AllSelectQALYMethod, string> dicQALYPoolingAndAggregationUnPooled;




















        private void picCRHelp_Click(object sender, EventArgs e)
        {
            this.toolTip1.Show("Double click datagrid to create result.\r\nIf you choose \'Create map,data and chart \',GIS Map/Table" +
                    "/Chart results will be created.\r\nIf you choose \'Create data (table) for multiple studies\',Only one acti" +
                    "ve result will be created.", picCRHelp, 5000);
        }

        private void btnAuditTrailOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sDlg = new SaveFileDialog();
                sDlg.Title = "Save Audit Trail Report to XML Document or Text";
                sDlg.Filter = "TXT Files (*.txt)|*.txt|CTLX Files (*.ctlx)|*.ctlx|XML Files (*.xml)|*.xml";
                sDlg.AddExtension = true;
                bool saveOk = false;
                if (rbAuditFile.Checked)
                {
                    if (string.IsNullOrEmpty(txtExistingConfiguration.Text))
                        return;
                }
                else
                {
                    if (CommonClass.ValuationMethodPoolingAndAggregation != null)
                    {

                    }
                    else if (CommonClass.BaseControlCRSelectFunction != null)
                    {
                    }
                    else
                    {
                        MessageBox.Show("Please finish your configuration first.");
                        return;
                    }

                }
                if (sDlg.ShowDialog() == DialogResult.OK && sDlg.FileName != string.Empty)
                {
                    switch (Path.GetExtension(sDlg.FileName))
                    {
                        case ".ctlx":
                            if (rbAuditFile.Checked)
                            {
                                string filePath = txtExistingConfiguration.Text;
                                if (!System.IO.File.Exists(filePath))
                                {
                                    MessageBox.Show("File not found.", "Error", MessageBoxButtons.OK);
                                    return;
                                }
                                string fileType = Path.GetExtension(txtExistingConfiguration.Text);
                                switch (fileType)
                                {
                                    case ".aqgx":
                                        BenMAPLine aqgBenMAPLine = new BenMAPLine();
                                        string err = "";
                                        aqgBenMAPLine = DataSourceCommonClass.LoadAQGFile(filePath, ref err);
                                        if (aqgBenMAPLine == null)
                                        {
                                            MessageBox.Show(err);
                                            return;
                                        }
                                        BatchCommonClass.OutputAQG(aqgBenMAPLine, sDlg.FileName, filePath);
                                        break;
                                    case ".cfgx":
                                        BaseControlCRSelectFunction cfgFunction = new BaseControlCRSelectFunction();
                                        err = "";
                                        cfgFunction = ConfigurationCommonClass.loadCFGFile(filePath, ref err);
                                        if (cfgFunction == null)
                                        {
                                            MessageBox.Show(err);
                                            return;
                                        }
                                        BatchCommonClass.OutputCFG(cfgFunction, sDlg.FileName, filePath);
                                        break;
                                    case ".cfgrx":
                                        BaseControlCRSelectFunctionCalculateValue cfgrFunctionCV = new BaseControlCRSelectFunctionCalculateValue();
                                        err = "";
                                        cfgrFunctionCV = ConfigurationCommonClass.LoadCFGRFile(filePath, ref err);
                                        if (cfgrFunctionCV == null)
                                        {
                                            MessageBox.Show(err);
                                            return;
                                        }
                                        BaseControlCRSelectFunction bc = new BaseControlCRSelectFunction();
                                        bc.CRLatinHypercubePoints = cfgrFunctionCV.CRLatinHypercubePoints;
                                        bc.CRRunInPointMode = cfgrFunctionCV.CRRunInPointMode;
                                        bc.CRThreshold = cfgrFunctionCV.CRThreshold;
                                        bc.CRSeeds = cfgrFunctionCV.CRSeeds;
                                        bc.RBenMapGrid = cfgrFunctionCV.RBenMapGrid;
                                        bc.BenMAPPopulation = cfgrFunctionCV.BenMAPPopulation;
                                        bc.BaseControlGroup = cfgrFunctionCV.BaseControlGroup;
                                        List<CRSelectFunction> lstCRSelectFunction = new List<CRSelectFunction>();
                                        foreach (CRSelectFunctionCalculateValue crfc in cfgrFunctionCV.lstCRSelectFunctionCalculateValue)
                                        {
                                            lstCRSelectFunction.Add(crfc.CRSelectFunction);
                                        }
                                        bc.lstCRSelectFunction = lstCRSelectFunction;
                                        BatchCommonClass.OutputCFG(bc, sDlg.FileName, "");
                                        ConfigurationCommonClass.SaveCFGFile(bc, sDlg.FileName.Substring(0, sDlg.FileName.Length - 4) + "cfgx");
                                        break;
                                    case ".apvx":
                                        ValuationMethodPoolingAndAggregation apvVMPA = new ValuationMethodPoolingAndAggregation();
                                        err = "";
                                        apvVMPA = APVX.APVCommonClass.loadAPVRFile(filePath, ref err);
                                        if (apvVMPA == null)
                                        {
                                            MessageBox.Show(err);
                                            return;
                                        }
                                        BatchCommonClass.OutputAPV(apvVMPA, sDlg.FileName, filePath);
                                        break;
                                    case ".apvrx":
                                        ValuationMethodPoolingAndAggregation apvrVMPA = new ValuationMethodPoolingAndAggregation();
                                        err = "";
                                        apvrVMPA = APVX.APVCommonClass.loadAPVRFile(filePath, ref err);
                                        if (apvrVMPA == null)
                                        {
                                            MessageBox.Show(err);
                                            return;
                                        }
                                        APVX.APVCommonClass.SaveAPVFile(sDlg.FileName.Substring(0, sDlg.FileName.Length - 4) + "apvx", apvrVMPA);
                                        BatchCommonClass.OutputAPV(apvrVMPA, sDlg.FileName, "");
                                        break;
                                }
                                MessageBox.Show("Configuration file saved.", "File saved");
                            }
                            else if (rbAuditCurrent.Checked)
                            {
                                int retVal = AuditTrailReportCommonClass.exportToCtlx(sDlg.FileName);
                                if (retVal == -1)
                                {
                                    MessageBox.Show("Please finish your configuration first.");
                                }
                                else
                                {
                                    MessageBox.Show("Configuration file saved.", "File saved");
                                }                               
                            }
                            break;
                        case ".txt":
							//MERGE CHECK
                            List<TreeNode> lstTmp = new List<TreeNode>();//treeListView.Objects as List<TreeNode>;
                            for (int i = 0; i < (treeListView.Objects as ArrayList).ToArray().Count(); i++)
                            {
                                lstTmp.Add((treeListView.Objects as ArrayList).ToArray()[i] as TreeNode);
                            }
                            if (lstTmp.Count > 0)
                            {
                                saveOk = exportToTxt(lstTmp, sDlg.FileName);
                            }
                            break;
                        case ".xml":
							//MERGE CHECK
                            List<TreeNode> lstTmpXML = new List<TreeNode>();//treeListView.Objects as List<TreeNode>;
                            for (int i = 0; i < (treeListView.Objects as ArrayList).ToArray().Count(); i++)
                            {
                                lstTmpXML.Add((treeListView.Objects as ArrayList).ToArray()[i] as TreeNode);
                            }
                            if (lstTmpXML.Count > 0)
                            {
                                saveOk = exportToXml(lstTmpXML, sDlg.FileName);
                            }
                            break;
                    }
                }
                if (saveOk)
                {
                    MessageBox.Show("Audit trail report saved.", "File saved");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private StreamWriter sw;
        public bool exportToTxt(List<TreeNode> tv, string filename)
        {
            try
            {
                Boolean first = true;
                FileStream fs = new FileStream(filename, FileMode.Create);
                sw = new StreamWriter(fs, Encoding.UTF8);
                for (int i = 0; i < tv.Count; i++)
                {
                    if (tv[i].Nodes.Count > 0)
                    {
                        sw.WriteLine("<" + tv[i].Text + ">");
                        foreach (TreeNode node in tv[i].Nodes)
                        {
                            if (first)
                            {
                                saveStartNodes(tv[i].Nodes);
                                first = false;
                            }
                            else
                            {
                                saveNode(node.Nodes);
                            }
                        }
                        sw.WriteLine("</" + tv[i].Text + ">");
                    }
                    else
                        sw.WriteLine(tv[i].Text);
                }
                sw.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }
        public bool exportToXml(List<TreeNode> tv, string filename)
        {
            try
            {
                sw = new StreamWriter(filename, false, System.Text.Encoding.UTF8);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                string file = Path.GetFileNameWithoutExtension(filename);
                file = file.Replace(" ", ".");
                file = file.Replace(",", ".");
                file = file.Replace("&", "And");
                file = file.Replace(":", "");
                file = file.Replace("..", ".");
                sw.WriteLine("<" + file + ">");
                for (int i = 0; i < tv.Count; i++)
                {
                    if (tv[i].Nodes.Count > 0)
                    {
                        string txtWithoutSpace = tv[i].Text;
                        txtWithoutSpace = txtWithoutSpace.Replace(" ", ".");
                        txtWithoutSpace = txtWithoutSpace.Replace(",", ".");
                        txtWithoutSpace = txtWithoutSpace.Replace("&", "And");
                        txtWithoutSpace = txtWithoutSpace.Replace(":", "");
                        txtWithoutSpace = txtWithoutSpace.Replace("..", ".");
                        sw.WriteLine("<" + txtWithoutSpace + ">");

                        foreach (TreeNode node in tv[i].Nodes)
                        {
                            saveNode(node.Nodes);
                        }
                        //Close the root node
                        sw.WriteLine("</" + txtWithoutSpace + ">");
                    }
                    else
                        sw.WriteLine(tv[i].Text);
                }
                sw.WriteLine("</" + file + ">");
                sw.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }

        private void saveNode(TreeNodeCollection tnc)
        {
            foreach (TreeNode node in tnc)
            {
                if (node.Nodes.Count > 0)
                {
                    string txtWithoutSpace = node.Text;
                    txtWithoutSpace = txtWithoutSpace.Replace(" ", ".");
                    txtWithoutSpace = txtWithoutSpace.Replace(",", ".");
                    txtWithoutSpace = txtWithoutSpace.Replace("&", "And");
                    txtWithoutSpace = txtWithoutSpace.Replace(":", "");
                    txtWithoutSpace = txtWithoutSpace.Replace("..", ".");

                    sw.WriteLine("<" + txtWithoutSpace + ">");
                    saveNode(node.Nodes);
                    sw.WriteLine("</" + txtWithoutSpace + ">");
                }
                else sw.WriteLine(node.Text);
            }
        }

        private void saveStartNodes(TreeNodeCollection tnc)
        {
            foreach (TreeNode node in tnc)
            {
                sw.WriteLine(node.Text);
            }
        }

        private void btShowCRResult_Click(object sender, EventArgs e)
        {
            try
            {
                if (olvCRFunctionResult.SelectedObjects == null || olvCRFunctionResult.SelectedObjects.Count == 0)
                    return;
                string Tip = "Drawing configuration results layer";
                WaitShow(Tip);
                bool bGIS = true;
                bool bTable = true;
                bool bChart = true;
                int i = 0;
                int iOldGridType = CommonClass.GBenMAPGrid.GridDefinitionID;
                CRSelectFunctionCalculateValue crSelectFunctionCalculateValue = null;
                CRSelectFunctionCalculateValue crSelectFunctionCalculateValueForChart = null;
                for (int icro = 0; icro < CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; icro++)
                {
                    CRSelectFunctionCalculateValue cro = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[icro];
                    Configuration.ConfigurationCommonClass.ClearCRSelectFunctionCalculateValueLHS(ref cro);
                }
                List<CRSelectFunctionCalculateValue> lstCRSelectFunctionCalculateValue = new List<CRSelectFunctionCalculateValue>();
                foreach (CRSelectFunctionCalculateValue cr in olvCRFunctionResult.SelectedObjects)
                {
                    lstCRSelectFunctionCalculateValue.Add(cr);
                    if (crSelectFunctionCalculateValueForChart == null) crSelectFunctionCalculateValueForChart = cr;
                }
                if (lstCRSelectFunctionCalculateValue.Count != 0)
                {
                    if (cbCRAggregation.SelectedIndex != -1 && cbCRAggregation.SelectedIndex != 0)
                    {
                        DataRowView drv = cbCRAggregation.SelectedItem as DataRowView;
                        int iAggregationGridType = Convert.ToInt32(drv["GridDefinitionID"]);
                        if (iAggregationGridType != CommonClass.GBenMAPGrid.GridDefinitionID)
                        {
                            List<CRSelectFunctionCalculateValue> lstTemp = new List<CRSelectFunctionCalculateValue>();
                            foreach (CRSelectFunctionCalculateValue cr in lstCRSelectFunctionCalculateValue)
                            {
                                lstTemp.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(cr, CommonClass.GBenMAPGrid.GridDefinitionID, iAggregationGridType));
                            }
                            lstCRSelectFunctionCalculateValue = lstTemp;
                            CommonClass.GBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(iAggregationGridType);
                        }

                    }
                    crSelectFunctionCalculateValue = lstCRSelectFunctionCalculateValue.First();
                    if (lstCRSelectFunctionCalculateValue[0].CRCalculateValues.Count == 1 && !CommonClass.CRRunInPointMode)
                    {
                        lstCFGRforCDF = lstCRSelectFunctionCalculateValue;
                        canshowCDF = true;
                    }
                    else
                    {
                        lstCFGRforCDF = new List<CRSelectFunctionCalculateValue>();
                        canshowCDF = false;
                    }
                    iCDF = 0;
                    if (i == 0)
                    {
                        ClearMapTableChart();
                        if (rdbShowActiveCR.Checked)
                        {
                            if (tabCtlMain.SelectedIndex == 0)
                            {
                                bTable = false;
                                bChart = false;
                            }
                            else if (tabCtlMain.SelectedIndex == 1)
                            {
                                bGIS = false;
                                bChart = false;
                            }
                            else if (tabCtlMain.SelectedIndex == 2)
                            {
                                bGIS = false;
                                bTable = false;
                            }
                        }
                        if (bTable)
                        {
                            InitTableResult(lstCRSelectFunctionCalculateValue);
                        }
                        if (bChart)
                        {
                            InitChartResult(crSelectFunctionCalculateValueForChart, iOldGridType);
                        }
                        if (bGIS)
                        {
                            if (_tableObject == null)
                            {
                                InitTableResult(lstCRSelectFunctionCalculateValue);
                                if (!bTable)
                                {
                                    OLVResultsShow.SetObjects(null);
                                }
                            }
                            //Add Pollutants Mapgroup if it doesn't exist already -MCB
                            //MapGroup ResultsMapGroup = new MapGroup();
                            MapGroup ResultsMapGroup = AddMapGroup("Results", "Map Layers", false, false);
                            MapGroup HIFResultsMapGroup = AddMapGroup("Health Impacts", "Results", false, false);
                            
                            string author = lstCRSelectFunctionCalculateValue.First().CRSelectFunction.BenMAPHealthImpactFunction.Author;
                            if (author.IndexOf(" ") != -1)
                            {
                                author = author.Substring(0, author.IndexOf(" "));
                            }
                            string LayerNameText = author;
                            //Remove the old version of the layer if exists already
                            RemoveOldPolygonLayer(LayerNameText, HIFResultsMapGroup.Layers, false);


                            //set change projection text
                            string changeProjText = "change projection to setup projection";
                            if (!String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
                            {
                                changeProjText = changeProjText + " (" + CommonClass.MainSetup.SetupProjection + ")";
                            }
                            tsbChangeProjection.Text = changeProjText;

                            mainMap.ProjectionModeReproject = ActionMode.Never;
                            mainMap.ProjectionModeDefine = ActionMode.Never;
                            string shapeFileName = "";
                            //mainMap.Layers.Clear();
                            if (CommonClass.GBenMAPGrid is ShapefileGrid)
                            {                               
                                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                                {
                                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
                                }
                            }
                            else if (CommonClass.GBenMAPGrid is RegularGrid)
                            {
                                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp"))
                                {
                                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp";
                                }
                            }

                            // get unique beta variation count (such as number of seasons) and create layer for each
                            int bvCount = crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Variables.First().PollBetas.Count();
                            Dictionary<int, string> dicUniqueBV = new Dictionary<int, string>();
                            int bvUniqueCount = 0;

                            for (int ind = 0; ind < bvCount; ind++)
                            {
                                if (!dicUniqueBV.ContainsValue(crSelectFunctionCalculateValue.CRCalculateValues[ind].BetaName))
                                {
                                    dicUniqueBV.Add(ind, crSelectFunctionCalculateValue.CRCalculateValues[ind].BetaName);
                                    bvUniqueCount++;
                                }

                            }

                            for (int ind = 0; ind < bvUniqueCount; ind++)
                            {
                                MapPolygonLayer CRResultMapPolyLayer = (MapPolygonLayer)HIFResultsMapGroup.Layers.Add(shapeFileName);

                                DataTable dt = CRResultMapPolyLayer.DataSet.DataTable;
                                int j = 0;
                                int iCol = 0;
                                int iRow = 0;
                                List<string> lstRemoveName = new List<string>();
                                while (j < dt.Columns.Count)
                                {
                                    if (dt.Columns[j].ColumnName.ToLower() == "col") iCol = j;
                                    if (dt.Columns[j].ColumnName.ToLower() == "row") iRow = j;

                                    j++;
                                }
                                j = 0;

                                while (j < dt.Columns.Count)
                                {
                                    if (dt.Columns[j].ColumnName.ToLower() == "col" || dt.Columns[j].ColumnName.ToLower() == "row")
                                    { }
                                    else
                                        lstRemoveName.Add(dt.Columns[j].ColumnName);

                                    j++;
                                }
                                foreach (string s in lstRemoveName)
                                {
                                    dt.Columns.Remove(s);
                                }
                                // Set layer name with beta name to differentiate 
                                string layerName = "Incidence" + (dicUniqueBV[ind] == "" ? "" : " (" + dicUniqueBV[ind] + ")");

                                dt.Columns.Add(layerName, typeof(double));
                                j = 0;
                                while (j < dt.Columns.Count)
                                {
                                    if (dt.Columns[j].ColumnName.ToLower() == "col") iCol = j;
                                    if (dt.Columns[j].ColumnName.ToLower() == "row") iRow = j;

                                    j++;
                                }
                                j = 0;
                                CRCalculateValue crcv;
                                Dictionary<string, double> dicAll = new Dictionary<string, double>();
                                // Load values into dictionary according to beta variation offset
                                // If we have multiple seasons of the same name, we will sum the values 
                                for (j = 0; j < crSelectFunctionCalculateValue.CRCalculateValues.Count(); j++)
                                {
                                    crcv = crSelectFunctionCalculateValue.CRCalculateValues[j];
                                    if(crcv.BetaName.Equals(dicUniqueBV[ind]))
                                    {
                                        if (dicAll.ContainsKey(crcv.Col + "," + crcv.Row))
                                        {
                                            dicAll[crcv.Col + "," + crcv.Row] += crcv.PointEstimate;
                                        }
                                        else
                                        {
                                            dicAll.Add(crcv.Col + "," + crcv.Row, crcv.PointEstimate);
                                        }

                                    }

                                }

                                foreach (DataRow dr in dt.Rows)
                                {
                                    try
                                    {
                                        if (dicAll.ContainsKey(dr[iCol] + "," + dr[iRow]))
                                        {
                                            dr[layerName] = Math.Round(dicAll[dr[iCol] + "," + dr[iRow]], 10);
                                        }
                                        else
                                        {
                                            dr[layerName] = 0;
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                                if (File.Exists(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp");
                                CRResultMapPolyLayer.DataSet.SaveAs(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp", true);

                                MapPolygonLayer polLayer = CRResultMapPolyLayer;
                                polLayer.LegendText = author;
                                polLayer.Name = polLayer.LegendText;
                                string strValueField = polLayer.DataSet.DataTable.Columns[polLayer.DataSet.DataTable.Columns.Count - 1].ColumnName;
                                _columnName = strValueField;
                                polLayer.Symbology = CreateResultPolyScheme(ref polLayer, 6, "R"); //-MCB added

                                double dMinValue = 0.0;
                                double dMaxValue = 0.0;
                                dMinValue = crSelectFunctionCalculateValue.CRCalculateValues.Min(a => a.PointEstimate);
                                dMaxValue = crSelectFunctionCalculateValue.CRCalculateValues.Max(a => a.PointEstimate);

                                _dMinValue = dMinValue;
                                _dMaxValue = dMaxValue;
                                _CurrentIMapLayer = polLayer;
                                string pollutantUnit = string.Empty;
                                _columnName = strValueField;
                                _CurrentMapTitle = CommonClass.MainSetup.SetupName + " Setup: " + "Health Impacts- " + CRResultMapPolyLayer.LegendText;  //-MCB draft until better title

                                RenderMainMap(true, "H");   //"R"
                                addRegionLayerGroupToMainMap();
                            }
                        }
                    }
                    i++;
                    CommonClass.GBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(iOldGridType);
                }
                
                WaitClose();
                int result = EnforceLegendOrder();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                WaitClose();
                int result = EnforceLegendOrder();
            }

        }
        public List<FieldCheck> cflstColumnRow;
        public List<FieldCheck> cflstHealth;
        public List<FieldCheck> cflstResult;

        public List<FieldCheck> IncidencelstColumnRow;
        public List<FieldCheck> IncidencelstHealth;
        public List<FieldCheck> IncidencelstResult;

        public List<FieldCheck> apvlstColumnRow;
        public List<FieldCheck> apvlstHealth;
        public List<FieldCheck> apvlstResult;

        public List<FieldCheck> qalylstColumnRow;
        public List<FieldCheck> qalylstHealth;
        public List<FieldCheck> qalylstResult;

        public List<string> strHealthImpactPercentiles;
        public List<string> strPoolIncidencePercentiles;
        public List<string> strAPVPercentiles;
        public bool assignHealthImpactPercentile;
        public bool assignPoolIncidencePercentile;
        public bool assignAPVPercentile;

        private void btSelectAttribute_Click(object sender, EventArgs e)
        {
            try
            {
                ConfigurationResultsReport frm = new ConfigurationResultsReport();
                frm.lstColumnRow = cflstColumnRow;
                frm.lstHealth = cflstHealth;
                frm.lstResult = cflstResult;
                frm.sArrayPercentile = strHealthImpactPercentiles;
                frm.userAssignPercentile = assignHealthImpactPercentile;
                DialogResult rt = frm.ShowDialog();
                if (rt != System.Windows.Forms.DialogResult.OK) return;
                cflstColumnRow = frm.lstColumnRow;
                cflstHealth = frm.lstHealth;
                cflstResult = frm.lstResult;
                strHealthImpactPercentiles = frm.sArrayPercentile;
                assignHealthImpactPercentile = frm.userAssignPercentile;


                if (_tableObject != null)
                {
                    int pagecurrent = _pageCurrent;
                    InitTableResult(_tableObject);
                    _pageCurrent = pagecurrent;
                    UpdateTableResult(_tableObject);
                    bindingNavigatorPositionItem.Text = pagecurrent.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        private void btPoolingSelectAttribute_Click(object sender, EventArgs e)
        {
            try
            {
                ConfigurationResultsReport frm = new ConfigurationResultsReport();
                frm.lstColumnRow = IncidencelstColumnRow;
                frm.lstHealth = IncidencelstHealth;
                frm.lstResult = IncidencelstResult;
                frm.sArrayPercentile = strPoolIncidencePercentiles;
                frm.userAssignPercentile = assignPoolIncidencePercentile;
                frm.isPooledIncidence = true;
                DialogResult rt = frm.ShowDialog();
                if (rt != System.Windows.Forms.DialogResult.OK) return;
                IncidencelstColumnRow = frm.lstColumnRow;
                IncidencelstHealth = frm.lstHealth;
                IncidencelstResult = frm.lstResult;
                strPoolIncidencePercentiles = frm.sArrayPercentile;
                assignPoolIncidencePercentile = frm.userAssignPercentile;

                if (_tableObject != null)
                {
                    int pagecurrent = _pageCurrent;
                    InitTableResult(_tableObject);
                    _pageCurrent = pagecurrent;
                    UpdateTableResult(_tableObject);
                    bindingNavigatorPositionItem.Text = pagecurrent.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        private void cboRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataRowView drGrid = cboRegion.SelectedItem as DataRowView;
                string shapeFileName;

                if (isLoad)
                {
                    if (mainMap.GetAllLayers().Count > 1)
                    {
                        if (CommonClass.RBenMAPGrid == null)
                        {
                            mainMap.Layers.Clear();
                            CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));
                        }
                        else
                        {
                            if (CommonClass.RBenMAPGrid is ShapefileGrid)
                            {
                                shapeFileName = ((ShapefileGrid)CommonClass.RBenMAPGrid).GridDefinitionName;
                            }
                            else
                            {
                                shapeFileName = ((RegularGrid)CommonClass.RBenMAPGrid).GridDefinitionName;

                            }
                            for (int i = 0; i < mainMap.GetAllLayers().Count; i++)
                            {
                                if (mainMap.GetAllLayers()[i].LegendText == shapeFileName)
                                {
                                    mainMap.GetAllLayers().RemoveAt(i);
                                    break;
                                }
                            }

                            CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));
                            addRegionLayerGroupToMainMap();
                            int result = EnforceLegendOrder();
                        }

                    }
                    else
                    {
                        CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));
                    }
                }

                CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));
            }
            catch(Exception E)
            {
                Console.WriteLine("Error setting up grids: " + e.ToString());
            }
        }
        private void btAPVSelectAttribute_Click(object sender, EventArgs e)
        {
            try
            {
                APVResultsReport frm = new APVResultsReport();
                frm.lstColumnRow = apvlstColumnRow;
                frm.lstHealth = apvlstHealth;
                frm.lstResult = apvlstResult;
                frm.sArrayPercentile = strAPVPercentiles;
                frm.userAssignPercentile = assignAPVPercentile;
                DialogResult rt = frm.ShowDialog();
                if (rt != System.Windows.Forms.DialogResult.OK) return;
                apvlstColumnRow = frm.lstColumnRow;
                apvlstHealth = frm.lstHealth;
                apvlstResult = frm.lstResult;
                strAPVPercentiles = frm.sArrayPercentile;
                assignAPVPercentile = frm.userAssignPercentile;
                if (_tableObject != null)
                {
                    int pagecurrent = _pageCurrent;
                    InitTableResult(_tableObject);
                    _pageCurrent = pagecurrent;
                    UpdateTableResult(_tableObject);
                    bindingNavigatorPositionItem.Text = pagecurrent.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        private void btQALYSelectAttribute_Click(object sender, EventArgs e)
        {
            try
            {
                QALYResultsReport frm = new QALYResultsReport();
                frm.lstColumnRow = qalylstColumnRow;
                frm.lstHealth = qalylstHealth;
                frm.lstResult = qalylstResult;
                DialogResult rt = frm.ShowDialog();
                if (rt != System.Windows.Forms.DialogResult.OK) return;
                qalylstColumnRow = frm.lstColumnRow;
                qalylstHealth = frm.lstHealth;
                qalylstResult = frm.lstResult;
                if (_tableObject != null)
                {
                    int pagecurrent = _pageCurrent;
                    InitTableResult(_tableObject);
                    _pageCurrent = pagecurrent;
                    UpdateTableResult(_tableObject);
                    bindingNavigatorPositionItem.Text = pagecurrent.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void OLVResultsShow_BeforeSorting(object sender, BeforeSortingEventArgs e)
        {


        }
        private void OLVResultsShow_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            try
            {
                if (_tableObject == null || sender == null) return;
                int i = 0;
                if (_tableObject is List<CRSelectFunctionCalculateValue> || _tableObject is CRSelectFunctionCalculateValue)
                {

                    List<CRSelectFunctionCalculateValue> lstCRTable = new List<CRSelectFunctionCalculateValue>();
                    if (_tableObject is List<CRSelectFunctionCalculateValue>)
                        lstCRTable = (List<CRSelectFunctionCalculateValue>)_tableObject;
                    else
                        lstCRTable.Add(_tableObject as CRSelectFunctionCalculateValue);
                    foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                    {
                        cr.CRCalculateValues = cr.CRCalculateValues.Where(p => p.Population > 0).ToList();
                        if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
                        {

                            switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                            {
                                case "column":

                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.Col).ToList();

                                    break;
                                case "row":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.Row).ToList();
                                    break;
                                case "pointestimate":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.PointEstimate).ToList();
                                    break;
                                case "population":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.Population).ToList();
                                    break;
                                case "delta":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.Delta).ToList();
                                    break;
                                case "mean":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.Mean).ToList();
                                    break;
                                case "baseline":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.Baseline).ToList();
                                    break;
                                case "percentofbaseline":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.PercentOfBaseline).ToList();
                                    break;
                                case "standarddeviation":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.StandardDeviation).ToList();
                                    break;
                                case "variance":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.Variance).ToList();
                                    break;
                                default:
                                    if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10) == "Percentile")
                                    {
                                        cr.CRCalculateValues = cr.CRCalculateValues.OrderBy(p => p.LstPercentile[cr.CRCalculateValues.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();

                                    }
                                    break;

                            }
                        }
                        else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
                        {
                            switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                            {
                                case "column":

                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.Col).ToList();

                                    break;
                                case "row":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.Row).ToList();
                                    break;
                                case "pointestimate":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.PointEstimate).ToList();
                                    break;
                                case "population":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.Population).ToList();
                                    break;
                                case "delta":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.Delta).ToList();
                                    break;
                                case "mean":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.Mean).ToList();
                                    break;
                                case "baseline":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.Baseline).ToList();
                                    break;
                                case "percentofbaseline":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.PercentOfBaseline).ToList();
                                    break;
                                case "standarddeviation":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.StandardDeviation).ToList();
                                    break;
                                case "variance":
                                    cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.Variance).ToList();
                                    break;
                                default:
                                    if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10).ToLower() == "percentile")
                                    {
                                        cr.CRCalculateValues = cr.CRCalculateValues.OrderByDescending(p => p.LstPercentile[cr.CRCalculateValues.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();

                                    }
                                    break;

                            }
                        }
                    }
                    if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
                    {
                        switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                        {

                            case "dataset":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName).ToList();
                                break;
                            case "endpointgroup":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup).ToList();
                                break;
                            case "endpoint":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint).ToList();
                                break;
                            case "pollutant":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.PollutantGroup.PollutantGroupName).ToList();
                                break;
                            case "metric":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName).ToList();
                                break;
                            case "seasonalmetric":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric).ToList();
                                break;
                            case "metricstatistic":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic).ToList();
                                break;
                            case "author":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Author).ToList();
                                break;
                            case "year":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Year).ToList();
                                break;
                            case "geographicarea":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.GeographicAreaName).ToList();
                                break;
                            case "otherpollutants":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants).ToList();
                                break;
                            case "qualifier":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier).ToList();
                                break;
                            case "reference":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Reference).ToList();
                                break;
                            case "race":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.Race).ToList();
                                break;
                            case "ethnicity":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.Ethnicity).ToList();
                                break;
                            case "gender":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.Gender).ToList();
                                break;
                            case "startage":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.StartAge).ToList();
                                break;
                            case "endage":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.EndAge).ToList();
                                break;
                            case "function":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Function).ToList();
                                break;
                            case "incidencedataset":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.IncidenceDataSetName).ToList();
                                break;
                            case "prevalencedataset":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.PrevalenceDataSetName).ToList();
                                break;
                            case "beta":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Beta).ToList();
                                break;
                            case "disbeta":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BetaDistribution).ToList();
                                break;
                            case "p1beta":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter1).ToList();
                                break;
                            case "p2beta":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter2).ToList();
                                break;
                            case "a":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.AContantValue).ToList();
                                break;
                            case "namea":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.AContantDescription).ToList();
                                break;
                            case "b":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BContantValue).ToList();
                                break;
                            case "nameb":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BContantDescription).ToList();
                                break;
                            case "c":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.CContantValue).ToList();
                                break;
                            case "namec":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.CContantDescription).ToList();
                                break;

                        }
                    }
                    else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
                    {
                        switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                        {

                            case "dataset":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName).ToList();
                                break;
                            case "endpointgroup":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup).ToList();
                                break;
                            case "endpoint":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint).ToList();
                                break;
                            case "pollutant":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.PollutantGroup.PollutantGroupName).ToList();
                                break;
                            case "metric":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName).ToList();
                                break;
                            case "seasonalmetric":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric).ToList();
                                break;
                            case "metricstatistic":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic).ToList();
                                break;
                            case "author":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Author).ToList();
                                break;
                            case "year":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Year).ToList();
                                break;
                            case "geographicarea":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.GeographicAreaName).ToList();
                                break;
                            case "otherpollutants":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants).ToList();
                                break;
                            case "qualifier":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier).ToList();
                                break;
                            case "reference":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Reference).ToList();
                                break;
                            case "race":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.Race).ToList();
                                break;
                            case "ethnicity":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.Ethnicity).ToList();
                                break;
                            case "gender":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.Gender).ToList();
                                break;
                            case "startage":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.StartAge).ToList();
                                break;
                            case "endage":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.EndAge).ToList();
                                break;
                            case "function":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Function).ToList();
                                break;
                            case "incidencedataset":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.IncidenceDataSetName).ToList();
                                break;
                            case "prevalencedataset":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.PrevalenceDataSetName).ToList();
                                break;
                            case "beta":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Beta).ToList();
                                break;
                            case "disbeta":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BetaDistribution).ToList();
                                break;
                            case "p1beta":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter1).ToList();
                                break;
                            case "p2beta":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter2).ToList();
                                break;
                            case "a":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.AContantValue).ToList();
                                break;
                            case "namea":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.AContantDescription).ToList();
                                break;
                            case "b":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BContantValue).ToList();
                                break;
                            case "nameb":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.BContantDescription).ToList();
                                break;
                            case "c":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.CContantValue).ToList();
                                break;
                            case "namec":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.CContantDescription).ToList();
                                break;

                        }
                    }



                    _tableObject = lstCRTable;
                    UpdateTableResult(lstCRTable);
                }
                else if (_tableObject is List<AllSelectCRFunction> || _tableObject is AllSelectCRFunction)
                {

                    List<AllSelectCRFunction> lstCRTable = new List<AllSelectCRFunction>();
                    if (_tableObject is List<AllSelectCRFunction>)
                        lstCRTable = (List<AllSelectCRFunction>)_tableObject;
                    else
                        lstCRTable.Add(_tableObject as AllSelectCRFunction);
                    foreach (AllSelectCRFunction cr in lstCRTable)
                    {
                        cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.Where(p => p.Population > 0).ToList();
                        if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
                        {

                            switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                            {
                                case "column":

                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Col).ToList();

                                    break;
                                case "row":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Row).ToList();
                                    break;
                                case "pointestimate":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.PointEstimate).ToList();
                                    break;
                                case "population":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Population).ToList();
                                    break;
                                case "delta":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Delta).ToList();
                                    break;
                                case "mean":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Mean).ToList();
                                    break;
                                case "baseline":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Baseline).ToList();
                                    break;
                                case "percentofbaseline":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.PercentOfBaseline).ToList();
                                    break;
                                case "standarddeviation":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.StandardDeviation).ToList();
                                    break;
                                case "variance":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.Variance).ToList();
                                    break;
                                default:
                                    if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10) == "Percentile")
                                    {
                                        cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderBy(p => p.LstPercentile[cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();

                                    }
                                    break;

                            }
                        }
                        else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
                        {
                            switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                            {
                                case "column":

                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Col).ToList();

                                    break;
                                case "row":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Row).ToList();
                                    break;
                                case "pointestimate":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.PointEstimate).ToList();
                                    break;
                                case "population":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Population).ToList();
                                    break;
                                case "delta":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Delta).ToList();
                                    break;
                                case "mean":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Mean).ToList();
                                    break;
                                case "baseline":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Baseline).ToList();
                                    break;
                                case "percentofbaseline":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.PercentOfBaseline).ToList();
                                    break;
                                case "standarddeviation":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.StandardDeviation).ToList();
                                    break;
                                case "variance":
                                    cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.Variance).ToList();
                                    break;
                                default:
                                    if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10).ToLower() == "percentile")
                                    {
                                        cr.CRSelectFunctionCalculateValue.CRCalculateValues = cr.CRSelectFunctionCalculateValue.CRCalculateValues.OrderByDescending(p => p.LstPercentile[cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();

                                    }
                                    break;

                            }
                        }
                    }
                    if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
                    {
                        switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                        {

                            case "dataset":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName).ToList();
                                break;
                            case "endpointgroup":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup).ToList();
                                break;
                            case "endpoint":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint).ToList();
                                break;
                            case "pollutant":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.PollutantGroup.PollutantGroupName).ToList();
                                break;
                            case "metric":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName).ToList();
                                break;
                            case "seasonalmetric":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric).ToList();
                                break;
                            case "metricstatistic":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic).ToList();
                                break;
                            case "author":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author).ToList();
                                break;
                            case "year":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Year).ToList();
                                break;
                            case "geographicarea":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.GeographicAreaName).ToList();
                                break;
                            case "otherpollutants":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants).ToList();
                                break;
                            case "qualifier":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier).ToList();
                                break;
                            case "reference":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Reference).ToList();
                                break;
                            case "race":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Race).ToList();
                                break;
                            case "ethnicity":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Ethnicity).ToList();
                                break;
                            case "gender":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Gender).ToList();
                                break;
                            case "startage":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.StartAge).ToList();
                                break;
                            case "endage":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.EndAge).ToList();
                                break;
                            case "function":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Function).ToList();
                                break;
                            case "incidencedataset":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.IncidenceDataSetName).ToList();
                                break;
                            case "prevalencedataset":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.PrevalenceDataSetName).ToList();
                                break;
                            case "beta":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Beta).ToList();
                                break;
                            case "disbeta":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaDistribution).ToList();
                                break;
                            case "p1beta":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter1).ToList();
                                break;
                            case "p2beta":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter2).ToList();
                                break;
                            case "a":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantValue).ToList();
                                break;
                            case "namea":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantDescription).ToList();
                                break;
                            case "b":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantValue).ToList();
                                break;
                            case "nameb":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantDescription).ToList();
                                break;
                            case "c":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantValue).ToList();
                                break;
                            case "namec":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantDescription).ToList();
                                break;

                        }
                    }
                    else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
                    {
                        switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                        {

                            case "dataset":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName).ToList();
                                break;
                            case "endpointgroup":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup).ToList();
                                break;
                            case "endpoint":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint).ToList();
                                break;
                            case "pollutant":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.PollutantGroup.PollutantGroupName).ToList();
                                break;
                            case "metric":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName).ToList();
                                break;
                            case "seasonalmetric":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric).ToList();
                                break;
                            case "metricstatistic":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic).ToList();
                                break;
                            case "author":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author).ToList();
                                break;
                            case "year":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Year).ToList();
                                break;
                            case "geographicarea":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.GeographicAreaName).ToList();
                                break;
                            case "otherpollutants":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants).ToList();
                                break;
                            case "qualifier":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier).ToList();
                                break;
                            case "reference":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Reference).ToList();
                                break;
                            case "race":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Race).ToList();
                                break;
                            case "ethnicity":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Ethnicity).ToList();
                                break;
                            case "gender":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.Gender).ToList();
                                break;
                            case "startage":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.StartAge).ToList();
                                break;
                            case "endage":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.EndAge).ToList();
                                break;
                            case "function":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Function).ToList();
                                break;
                            case "incidencedataset":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.IncidenceDataSetName).ToList();
                                break;
                            case "prevalencedataset":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.PrevalenceDataSetName).ToList();
                                break;
                            case "beta":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Beta).ToList();
                                break;
                            case "disbeta":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaDistribution).ToList();
                                break;
                            case "p1beta":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter1).ToList();
                                break;
                            case "p2beta":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter2).ToList();
                                break;
                            case "a":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantValue).ToList();
                                break;
                            case "namea":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantDescription).ToList();
                                break;
                            case "b":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantValue).ToList();
                                break;
                            case "nameb":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantDescription).ToList();
                                break;
                            case "c":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantValue).ToList();
                                break;
                            case "namec":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantDescription).ToList();
                                break;

                        }
                    }
                    _tableObject = lstCRTable;
                    UpdateTableResult(lstCRTable);
                }
                else if (_tableObject is List<AllSelectValuationMethodAndValue> || _tableObject is AllSelectValuationMethodAndValue)
                {
                    List<AllSelectValuationMethodAndValue> lstallSelectValuationMethodAndValue = new List<AllSelectValuationMethodAndValue>();
                    if (_tableObject is List<AllSelectValuationMethodAndValue>)
                    {
                        lstallSelectValuationMethodAndValue = (List<AllSelectValuationMethodAndValue>)_tableObject;
                    }
                    else
                    {
                        lstallSelectValuationMethodAndValue.Add((AllSelectValuationMethodAndValue)_tableObject);
                    }
                    foreach (AllSelectValuationMethodAndValue avmav in lstallSelectValuationMethodAndValue)
                    {
                        if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
                        {
                            switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                            {
                                case "column":
                                    avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderBy(p => p.Col).ToList();
                                    break;
                                case "row":
                                    avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderBy(p => p.Row).ToList();
                                    break;
                                case "pointestimate":
                                    avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderBy(p => p.PointEstimate).ToList();
                                    break;

                                case "mean":
                                    avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderBy(p => p.Mean).ToList();
                                    break;

                                case "standarddeviation":
                                    avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderBy(p => p.StandardDeviation).ToList();
                                    break;
                                case "variance":
                                    avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderBy(p => p.Variance).ToList();
                                    break;
                                default:
                                    if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10).ToLower() == "percentile")
                                    {
                                        avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderBy(p => p.LstPercentile[avmav.lstAPVValueAttributes.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();

                                    }
                                    break;

                            }

                        }
                        else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
                        {
                            switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                            {
                                case "column":
                                    avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderByDescending(p => p.Col).ToList();
                                    break;
                                case "row":
                                    avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderByDescending(p => p.Row).ToList();
                                    break;
                                case "pointestimate":
                                    avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderByDescending(p => p.PointEstimate).ToList();
                                    break;

                                case "mean":
                                    avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderByDescending(p => p.Mean).ToList();
                                    break;

                                case "standarddeviation":
                                    avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderByDescending(p => p.StandardDeviation).ToList();
                                    break;
                                case "variance":
                                    avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderByDescending(p => p.Variance).ToList();
                                    break;
                                default:
                                    if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10).ToLower() == "percentile")
                                    {
                                        avmav.lstAPVValueAttributes = avmav.lstAPVValueAttributes.OrderByDescending(p => p.LstPercentile[avmav.lstAPVValueAttributes.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();

                                    }
                                    break;

                            }
                        }
                    }
                    if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
                    {
                        switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                        {

                            case "dataset":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.DataSet).ToList();
                                break;
                            case "endpointgroup":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.EndPointGroup).ToList();
                                break;
                            case "endpoint":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.EndPoint).ToList();
                                break;
                            case "pollutant":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.PollutantGroup).ToList();
                                break;

                            case "author":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Author).ToList();
                                break;
                            case "year":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Year).ToList();
                                break;
                            case "location":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Location).ToList();
                                break;
                            case "otherpollutants":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.OtherPollutants).ToList();
                                break;
                            case "qualifier":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Qualifier).ToList();
                                break;

                            case "race":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Race).ToList();
                                break;
                            case "ethnicity":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Ethnicity).ToList();
                                break;
                            case "gender":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Gender).ToList();
                                break;
                            case "startage":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.StartAge).ToList();
                                break;
                            case "endage":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.EndAge).ToList();
                                break;
                            case "function":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Function).ToList();
                                break;

                            case "version":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Version).ToList();
                                break;
                        }

                    }
                    else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
                    {
                        switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                        {

                            case "dataset":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.DataSet).ToList();
                                break;
                            case "endpointgroup":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.EndPointGroup).ToList();
                                break;
                            case "endpoint":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.EndPoint).ToList();
                                break;
                            case "pollutant":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.PollutantGroup).ToList();
                                break;

                            case "author":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Author).ToList();
                                break;
                            case "year":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Year).ToList();
                                break;
                            case "location":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Location).ToList();
                                break;
                            case "otherpollutants":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.OtherPollutants).ToList();
                                break;
                            case "qualifier":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Qualifier).ToList();
                                break;

                            case "race":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Race).ToList();
                                break;
                            case "ethnicity":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Ethnicity).ToList();
                                break;
                            case "gender":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Gender).ToList();
                                break;
                            case "startage":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.StartAge).ToList();
                                break;
                            case "endage":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.EndAge).ToList();
                                break;
                            case "function":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Function).ToList();
                                break;

                            case "version":
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Version).ToList();
                                break;
                        }
                    }
                    _tableObject = lstallSelectValuationMethodAndValue;
                    UpdateTableResult(lstallSelectValuationMethodAndValue);
                }

                else if (_tableObject is List<AllSelectQALYMethodAndValue> || _tableObject is AllSelectQALYMethodAndValue)
                {
                    List<AllSelectQALYMethodAndValue> lstallSelectQALYMethodAndValue = new List<AllSelectQALYMethodAndValue>();
                    if (_tableObject is List<AllSelectQALYMethodAndValue>)
                    {
                        lstallSelectQALYMethodAndValue = (List<AllSelectQALYMethodAndValue>)_tableObject;
                    }
                    else
                    {
                        lstallSelectQALYMethodAndValue.Add((AllSelectQALYMethodAndValue)_tableObject);
                    }
                    foreach (AllSelectQALYMethodAndValue avmav in lstallSelectQALYMethodAndValue)
                    {
                        if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
                        {
                            switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                            {
                                case "column":
                                    avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderBy(p => p.Col).ToList();
                                    break;
                                case "row":
                                    avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderBy(p => p.Row).ToList();
                                    break;
                                case "pointestimate":
                                    avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderBy(p => p.PointEstimate).ToList();
                                    break;

                                case "mean":
                                    avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderBy(p => p.Mean).ToList();
                                    break;

                                case "standarddeviation":
                                    avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderBy(p => p.StandardDeviation).ToList();
                                    break;
                                case "variance":
                                    avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderBy(p => p.Variance).ToList();
                                    break;
                                default:
                                    if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10).ToLower() == "percentile")
                                    {
                                        avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderBy(p => p.LstPercentile[avmav.lstQALYValueAttributes.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();

                                    }
                                    break;

                            }

                        }
                        else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
                        {
                            switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                            {
                                case "column":
                                    avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderByDescending(p => p.Col).ToList();
                                    break;
                                case "row":
                                    avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderByDescending(p => p.Row).ToList();
                                    break;
                                case "pointestimate":
                                    avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderByDescending(p => p.PointEstimate).ToList();
                                    break;

                                case "mean":
                                    avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderByDescending(p => p.Mean).ToList();
                                    break;

                                case "standarddeviation":
                                    avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderByDescending(p => p.StandardDeviation).ToList();
                                    break;
                                case "variance":
                                    avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderByDescending(p => p.Variance).ToList();
                                    break;
                                default:
                                    if ((sender as ObjectListView).Columns[e.Column].Text.Length >= 10 && (sender as ObjectListView).Columns[e.Column].Text.Substring(0, 10).ToLower() == "percentile")
                                    {
                                        avmav.lstQALYValueAttributes = avmav.lstQALYValueAttributes.OrderByDescending(p => p.LstPercentile[avmav.lstQALYValueAttributes.First().LstPercentile.Count - OLVResultsShow.Columns.Count + e.Column]).ToList();

                                    }
                                    break;

                            }
                        }
                    }
                    if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
                    {
                        switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                        {

                            case "dataset":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.DataSet).ToList();
                                break;
                            case "endpointgroup":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.EndPointGroup).ToList();
                                break;
                            case "endpoint":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.EndPoint).ToList();
                                break;
                            case "pollutant":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Pollutant).ToList();
                                break;

                            case "author":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Author).ToList();
                                break;
                            case "year":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Year).ToList();
                                break;
                            case "location":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Location).ToList();
                                break;
                            case "otherpollutants":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.OtherPollutants).ToList();
                                break;
                            case "qualifier":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Qualifier).ToList();
                                break;

                            case "race":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Race).ToList();
                                break;
                            case "ethnicity":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Ethnicity).ToList();
                                break;
                            case "gender":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Gender).ToList();
                                break;
                            case "startage":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.StartAge).ToList();
                                break;
                            case "endage":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.EndAge).ToList();
                                break;
                            case "function":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Function).ToList();
                                break;

                            case "version":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderBy(p => p.AllSelectQALYMethod.Version).ToList();
                                break;
                        }

                    }
                    else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
                    {
                        switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                        {

                            case "dataset":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.DataSet).ToList();
                                break;
                            case "endpointgroup":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.EndPointGroup).ToList();
                                break;
                            case "endpoint":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.EndPoint).ToList();
                                break;
                            case "pollutant":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Pollutant).ToList();
                                break;

                            case "author":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Author).ToList();
                                break;
                            case "year":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Year).ToList();
                                break;
                            case "location":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Location).ToList();
                                break;
                            case "otherpollutants":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.OtherPollutants).ToList();
                                break;
                            case "qualifier":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Qualifier).ToList();
                                break;

                            case "race":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Race).ToList();
                                break;
                            case "ethnicity":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Ethnicity).ToList();
                                break;
                            case "gender":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Gender).ToList();
                                break;
                            case "startage":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.StartAge).ToList();
                                break;
                            case "endage":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.EndAge).ToList();
                                break;
                            case "function":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Function).ToList();
                                break;

                            case "version":
                                lstallSelectQALYMethodAndValue = lstallSelectQALYMethodAndValue.OrderByDescending(p => p.AllSelectQALYMethod.Version).ToList();
                                break;
                        }
                    }
                    _tableObject = lstallSelectQALYMethodAndValue;
                    UpdateTableResult(lstallSelectQALYMethodAndValue);
                }
                else if (_tableObject is BenMAPLine)
                {
                    BenMAPLine crTable = (BenMAPLine)_tableObject;
                    if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
                    {
                        switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                        {
                            case "column":
                                crTable.ModelResultAttributes = crTable.ModelResultAttributes.OrderBy(p => p.Col).ToList();
                                break;
                            case "row":
                                crTable.ModelResultAttributes = crTable.ModelResultAttributes.OrderBy(p => p.Row).ToList();
                                break;
                        }
                    }
                    else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
                    {
                        switch ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "").ToLower())
                        {
                            case "column":
                                crTable.ModelResultAttributes = crTable.ModelResultAttributes.OrderByDescending(p => p.Col).ToList();
                                break;
                            case "row":
                                crTable.ModelResultAttributes = crTable.ModelResultAttributes.OrderByDescending(p => p.Row).ToList();
                                break;
                        }
                    }
                    List<string> lstAddField = new List<string>();
                    List<double[]> lstResultCopy = new List<double[]>();
                    foreach (Metric metric in crTable.Pollutant.Metrics)
                    {
                        lstAddField.Add(metric.MetricName);
                    }
                    if (crTable.Pollutant.SesonalMetrics != null)
                    {
                        foreach (SeasonalMetric sesonalMetric in crTable.Pollutant.SesonalMetrics)
                        {
                            lstAddField.Add(sesonalMetric.SeasonalMetricName);
                        }
                    }

                    i = 0;
                    while (i < lstAddField.Count())
                    {

                        if (OLVResultsShow.LastSortOrder == SortOrder.Ascending)
                        {
                            if ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "") == lstAddField[i])
                                crTable.ModelResultAttributes = crTable.ModelResultAttributes.OrderBy(p => p.Values[lstAddField[i]]).ToList();


                        }
                        else if (OLVResultsShow.LastSortOrder == SortOrder.Descending)
                        {
                            if ((sender as ObjectListView).Columns[e.Column].Text.Replace(" ", "") == lstAddField[i])
                                crTable.ModelResultAttributes = crTable.ModelResultAttributes.OrderByDescending(p => p.Values[lstAddField[i]]).ToList();
                        }
                        i++;
                    }

                    _tableObject = crTable;
                    UpdateTableResult(_tableObject);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void BenMAP_Shown(object sender, EventArgs e)
        {
            StartTip st = new StartTip();
            string iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
            string isShow = "T";
            if (System.IO.File.Exists(iniPath))
            {
                isShow = CommonClass.IniReadValue("appSettings", "IsShowStart", iniPath);
            }
            if (isShow == "T")
            { st.Show(); }
        }

        private void btnShowAPVResult_Click(object sender, EventArgs e)
        {
            tlvAPVResult_DoubleClick(sender, e);
        }


        private void trvSetting_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            Rectangle rec = e.Bounds;


            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString(e.Node.Text, trvSetting.Font, Brushes.Black, Rectangle.Inflate(e.Bounds, 2, 0), sf);


        }

        private void spTable_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnResultShow_RefreshItems(object sender, EventArgs e)
        {

        }
        private void RemoveOldPolygonLayer(string LayerName, IMapLayerCollection layerList, bool ShrinkOtherLayersInMapGroup = false)
        {
            MapGroup aMGLayer = new MapGroup();
            MapPolygonLayer aPolyLayer = new MapPolygonLayer();
            List<ILayer> layersToRemove = new List<ILayer>();
           
            //Remove the old version of the layer if exists already
            foreach (ILayer aLayer in layerList)
            {
                if (aLayer is MapGroup || aLayer is IMapGroup) //Look within Map groups
                {
                    aMGLayer = (MapGroup)aLayer;
                    RemoveOldPolygonLayer(LayerName, aMGLayer.Layers, ShrinkOtherLayersInMapGroup);
                   
                }
                else if (aLayer is FeatureLayer || aLayer is IFeatureLayer) // layer at root level(not in a mapgroup
                {
                    if (aLayer is MapPolygonLayer)
                    {
                        aPolyLayer = (MapPolygonLayer)aLayer;

                        if (aPolyLayer.Name == LayerName)
                        {
                            layersToRemove.Add(aLayer); //add to list of layers to remove                            
                        }
                        else if (ShrinkOtherLayersInMapGroup)  // Unexpand this layer to increase display room for new layer
                        {
                            aLayer.IsExpanded = false;
                        }
                    }
                }
            }

            //remove layers
            foreach (ILayer layer in layersToRemove)
            {
                layerList.Remove((IMapLayer)layer);
            }

           return;
        }
        private ILayer FindTopVisibleLayer(bool ignoreAdminMapGroup=false)
        {  //Loop through all of the layers and find the topmost visible one - used to update the map title
            //if ignoreAdminMapGroup = true then ignore the visible layers within that map group when finding the topvislayer.

            ILayer TopVisLayer = null;
            ILayer ThisLayer = null;
            List<ILayer> AllLayers = null;
            AllLayers = mainMap.GetAllLayers();
            AllLayers.Reverse(); //reverser list so Last added one is top visible
            TopVisLayer = null;
            for (int j=0; j <= AllLayers.Count-1; j++)
            {   
                ThisLayer = AllLayers[j];
                if (ThisLayer.IsVisible)
                {
                    if (!ignoreAdminMapGroup)
                    {
                        TopVisLayer = ThisLayer;
                        break;
                    }
                    else //make sure the layer is not in the admin group
                    {
                        foreach (MapGroup ThisMG in mainMap.GetAllGroups())
                        { 
                            if (ThisMG.LegendText == regionGroupLegendText & !ThisMG.Contains(ThisLayer))
                            {
                                TopVisLayer = ThisLayer;
                                return TopVisLayer;
                            }
                        }
                    }
                }
            }
            return TopVisLayer;
        }
        private MapGroup AddMapGroup(string mgName, string parentMGText,  bool ShrinkOtherMG = false, bool TurnOffNonReference = false)
        {
            if (mgName == null || mgName =="") return null;   //confirm map group name is valid

            bool mgFound = false;
            MapGroup NewMapGroup = new MapGroup();
            MapGroup ParentMapGroup = null;
            string parentText;
            
            //See if a map group with the Map group name already exists and find the name of the parent (if a map group)
            foreach (IMapLayer layer in mainMap.GetAllGroups())
            {
                if (layer is MapGroup || layer is IMapGroup)
                {
                    if (layer.LegendText == mgName) // && layer.GetParentItem().LegendText == parentMGText)
                    {
                        //Make sure the layer is in the same map group
                        ParentMapGroup = (MapGroup)mainMap.GetAllGroups().Find(m => m.Contains(layer));
                        if (ParentMapGroup == null)
                        {
                            //then assume top level
                            parentText = "Map Layers";           //default parent item- so top level map groups are detected correctly
                        }
                        else
                        {
                            parentText = ParentMapGroup.LegendText;
                        }

                        //if (layer.GetParentItem() != null)   //MCB--problem getting the parent map group of some map groups??????
                        //{
                        //    parentText = layer.GetParentItem().LegendText;
                        //}
                        
                        if (parentText == parentMGText)      //Map group already exists
                        {
                            NewMapGroup = (MapGroup)layer;    
                            mgFound = true;
                            //break;
                        }
                    }
                    else 
                    {
                        if (ShrinkOtherMG)
                        {
                            layer.IsExpanded = false;             // Unexpand other mapgroups to increase display room for new layer    
                        }
                        if (layer.LegendText != regionGroupLegendText && parentMGText != regionGroupLegendText)
                        {
                            if (TurnOffNonReference)
                            {
                                layer.IsVisible = false;        //turn off other layers
                            }
                        }
                    }
                    if (layer.LegendText == parentMGText) ParentMapGroup = (MapGroup)layer;
                }
            }

            if (!mgFound)  //New map group not found already, so add it
            {
                NewMapGroup.LegendText = mgName;
                if (parentMGText == "Map Layers")  //add map group at top level
                {
                    mainMap.Layers.Add(NewMapGroup);
                    //mainMap.Layers.Insert(mainMap.Layers.Count(),NewMapGroup);
                }
                else
                {
                    if (ParentMapGroup == null)
                    {
                        ParentMapGroup = (MapGroup)mainMap.GetAllGroups().Find(m => m.LegendText == parentMGText);
                    }
                    if (ParentMapGroup == null)  //If parent still null then we can't find a map group to put this layer in.
                    {
                        mainMap.Layers.Add(NewMapGroup);   //adding it to top level
                    }
                    else  //Add new group under it's parent Map Group
                    {
                        ParentMapGroup.Add(NewMapGroup);
                    }
                    NewMapGroup.SetParentItem(ParentMapGroup);
                }
               
            }
            //testing of index
            //int MGindex = mainMap.Layers.IndexOf(NewMapGroup);
            //int MGIndex2 = mainMap.GetAllLayers().Count - 1;
            //string MGname = NewMapGroup.LegendText;
            //MessageBox.Show("Index of new map group: " + MGname + " is " + MGindex + " or this: " + MGIndex2);
            return NewMapGroup;
        }

        private void tlvIncidence_DoubleClick(object sender, EventArgs e)
        {


            try
            {
                if (olvIncidence.SelectedObjects.Count == 0) return;
                ClearMapTableChart();
                bool bGIS = true;
                bool bTable = true;
                bool bChart = true;
                int i = 0;
                int iOldGridType = CommonClass.GBenMAPGrid.GridDefinitionID;
                CRSelectFunctionCalculateValue crSelectFunctionCalculateValue = null;
                BenMAPGrid incidenceGrid = CommonClass.GBenMAPGrid;
                if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
                              CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null)
                {
                    incidenceGrid = CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation;
                }
                List<AllSelectCRFunction> lstAllSelectCRFunction = new List<AllSelectCRFunction>();
                if ((sender is ObjectListView) || sender is Button)
                {
                    foreach (KeyValuePair<AllSelectCRFunction, string> keyValueCR in olvIncidence.SelectedObjects)
                    {
                        AllSelectCRFunction cr = keyValueCR.Key;

                        if (cr.CRSelectFunctionCalculateValue == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues.Count == 0)
                        {
                            continue;
                        }
                        else
                        {
                            if (crSelectFunctionCalculateValue == null)
                                crSelectFunctionCalculateValue = cr.CRSelectFunctionCalculateValue;
                            lstAllSelectCRFunction.Add(cr);
                        }

                    }
                    tabCtlMain.SelectedIndex = 1;
                    if (olvIncidence.SelectedObjects.Count > 1)
                    {
                        bGIS = false;
                        bChart = false;
                    }
                }
                else
                {
                    foreach (KeyValuePair<AllSelectCRFunction, string> keyValueCR in olvIncidence.Objects)
                    {
                        AllSelectCRFunction cr = keyValueCR.Key;

                        if (cr.CRSelectFunctionCalculateValue == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues.Count == 0)
                        {
                            continue;
                        }
                        else
                        {
                            if (crSelectFunctionCalculateValue == null)
                                crSelectFunctionCalculateValue = cr.CRSelectFunctionCalculateValue;
                            lstAllSelectCRFunction.Add(cr);
                        }

                    }

                    if (_MapAlreadyDisplayed && _IncidenceDragged)//MCB- a kluge: Need a better way to determine if sender was from map
                    {
                        bGIS = true;
                        tabCtlMain.SelectedIndex = 0;
                        bChart = false;
                    }
                    else
                    {
                        bGIS = false;
                        bChart = false;
                        tabCtlMain.SelectedIndex = 1;
                    }
                }
                if (lstAllSelectCRFunction.Count > 0) crSelectFunctionCalculateValue = lstAllSelectCRFunction.First().CRSelectFunctionCalculateValue;
                else return;
                if (lstAllSelectCRFunction[0].CRSelectFunctionCalculateValue.CRCalculateValues.Count == 1 && !CommonClass.CRRunInPointMode)
                {
                    lstCFGRpoolingforCDF = lstAllSelectCRFunction;
                    canshowCDF = true;
                    bChart = true;
                }
                else
                {
                    lstCFGRpoolingforCDF = new List<AllSelectCRFunction>();
                    canshowCDF = false;
                }
                iCDF = 1;
                string Tip = "Drawing pooled incidence results layer";
                WaitShow(Tip);
                if (crSelectFunctionCalculateValue != null)
                {
                    if (cbIncidenceAggregation.SelectedIndex != -1 && cbIncidenceAggregation.SelectedIndex != 0)
                    {
                        DataRowView drv = cbIncidenceAggregation.SelectedItem as DataRowView;
                        int iAggregationGridType = Convert.ToInt32(drv["GridDefinitionID"]);
                        if (iAggregationGridType != incidenceGrid.GridDefinitionID)
                        {
                            crSelectFunctionCalculateValue = APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crSelectFunctionCalculateValue, incidenceGrid.GridDefinitionID, iAggregationGridType);
                            incidenceGrid = Grid.GridCommon.getBenMAPGridFromID(iAggregationGridType);
                        }

                    }
                    if (i == 0)
                    {
                        ClearMapTableChart();
                        if (this.rbShowActiveIncidence.Checked)
                        {
                            if (tabCtlMain.SelectedIndex == 0)
                            {
                                bTable = false;
                                bChart = false;
                            }
                            else if (tabCtlMain.SelectedIndex == 1)
                            {
                                bGIS = false;
                                bChart = false;
                            }
                            else if (tabCtlMain.SelectedIndex == 2)
                            {
                                bGIS = false;
                                bTable = false;
                            }
                        }
                        if (bTable)
                        {
                            InitTableResult(lstAllSelectCRFunction);

                        }
                        if (bChart)
                        {

                            InitChartResult(crSelectFunctionCalculateValue, (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null) ? CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID : CommonClass.GBenMAPGrid.GridDefinitionID);
                        }
                        if (bGIS)
                        {
                            //set change projection text
                            string changeProjText = "change projection to setup projection";
                            if (!String.IsNullOrEmpty(CommonClass.MainSetup.SetupProjection))
                            {
                                changeProjText = changeProjText + " (" + CommonClass.MainSetup.SetupProjection + ")";
                            }
                            tsbChangeProjection.Text = changeProjText;

                            mainMap.ProjectionModeReproject = ActionMode.Never;
                            mainMap.ProjectionModeDefine = ActionMode.Never;
                            string shapeFileName = "";

                            MapGroup ResultsMG = AddMapGroup("Results", "Map Layers", false, false);
                            MapGroup PIResultsMapGroup = AddMapGroup("Pooled Incidence", "Results", false, false);
                            
                            //string LayerNameText = "Pooled Incidence";
                            string author = "Author Unknown";
                            if (crSelectFunctionCalculateValue.CRSelectFunction != null && crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction != null
                                                            && crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author != null)
                            {
                                author = crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author;
                                if (author.IndexOf(" ") > -1)
                                {
                                    author = author.Substring(0, author.IndexOf(" "));
                                }
                            }
                            string LayerNameText = "Pooled Incidence: " + author; 
                            RemoveOldPolygonLayer(LayerNameText, PIResultsMapGroup.Layers, false);

                            //mainMap.Layers.Clear();
                            if (incidenceGrid is ShapefileGrid)
                            {
                                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (incidenceGrid as ShapefileGrid).ShapefileName + ".shp"))
                                {
                                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (incidenceGrid as ShapefileGrid).ShapefileName + ".shp";
                                }
                            }
                            else if (incidenceGrid is RegularGrid)
                            {
                                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (incidenceGrid as RegularGrid).ShapefileName + ".shp"))
                                {
                                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (incidenceGrid as RegularGrid).ShapefileName + ".shp";
                                }
                            }


                            // get unique beta variation count (such as number of seasons) and create layer for each
                            //int bvCount = CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction[0].BenMAPHealthImpactFunction.Variables[0].PollBetas.Count();
                            Dictionary<int, string> dicUniqueBV = new Dictionary<int, string>();
                            int bvUniqueCount = 0;
                            for (int ind = 0; ind < crSelectFunctionCalculateValue.CRCalculateValues.Count(); ind++)
                            {
                                if (!dicUniqueBV.ContainsValue(crSelectFunctionCalculateValue.CRCalculateValues[ind].BetaName))
                                {
                                    dicUniqueBV.Add(bvUniqueCount++, crSelectFunctionCalculateValue.CRCalculateValues[ind].BetaName);
                                }

                            }

                            for (int ind = 0; ind < bvUniqueCount; ind++)
                            {

                                MapPolygonLayer tlvIPoolMapPolyLayer = (MapPolygonLayer)PIResultsMapGroup.Layers.Add(shapeFileName);

                                DataTable dt = tlvIPoolMapPolyLayer.DataSet.DataTable;
                                int j = 0;
                                int iCol = 0;
                                int iRow = 0;
                                List<string> lstRemoveName = new List<string>();
                                while (j < dt.Columns.Count)
                                {
                                    if (dt.Columns[j].ColumnName.ToLower() == "col") iCol = j;
                                    if (dt.Columns[j].ColumnName.ToLower() == "row") iRow = j;

                                    j++;
                                }
                                j = 0;

                                while (j < dt.Columns.Count)
                                {
                                    if (dt.Columns[j].ColumnName.ToLower() == "col" || dt.Columns[j].ColumnName.ToLower() == "row")
                                    { }
                                    else
                                        lstRemoveName.Add(dt.Columns[j].ColumnName);

                                    j++;
                                }
                                foreach (string s in lstRemoveName)
                                {
                                    dt.Columns.Remove(s);
                                }
                                string layerName = "Pooled Incidence" + (dicUniqueBV[ind] == "" ? "" : " (" + dicUniqueBV[ind] + ")");
                                dt.Columns.Add(layerName, typeof(double));
                                j = 0;
                                while (j < dt.Columns.Count)
                                {
                                    if (dt.Columns[j].ColumnName.ToLower() == "col") iCol = j;
                                    if (dt.Columns[j].ColumnName.ToLower() == "row") iRow = j;

                                    j++;
                                }
                                j = 0;
                                CRCalculateValue crcv;
                                Dictionary<string, double> dicAll = new Dictionary<string, double>();
                                for(j=0; j < crSelectFunctionCalculateValue.CRCalculateValues.Count(); j++)
                                {
                                    crcv = crSelectFunctionCalculateValue.CRCalculateValues[j];
                                    if (crcv.BetaName.Equals(dicUniqueBV[ind]))
                                    {
                                        if (dicAll.ContainsKey(crcv.Col + "," + crcv.Row))
                                        {
                                            dicAll[crcv.Col + "," + crcv.Row] += crcv.PointEstimate;
                                        }
                                        else
                                        {
                                            dicAll.Add(crcv.Col + "," + crcv.Row, crcv.PointEstimate);
                                        }
                                    }
                                }

                                foreach (DataRow dr in dt.Rows)
                                {
                                    try
                                    {
                                        if (dicAll.ContainsKey(dr[iCol] + "," + dr[iRow]))
                                        {
                                            dr[layerName] = Math.Round(dicAll[dr[iCol] + "," + dr[iRow]], 10);
                                        }
                                        else
                                        {
                                            dr[layerName] = 0;
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }

                                if (File.Exists(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp");
                                tlvIPoolMapPolyLayer.DataSet.SaveAs(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp", true);
                                // mainMap.Layers.Clear();  -MCB, will need to add code to clear the equivalent layer if it exists already

                                //tlvIPoolMapPolyLayer = (MapPolygonLayer)ResultsMG.Layers.Add(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp");
                               // tlvIPoolMapPolyLayer.DataSet.DataTable.Columns[tlvIPoolMapPolyLayer.DataSet.DataTable.Columns.Count - 1].ColumnName = "Pooled Incidence";

                                //if (crSelectFunctionCalculateValue.CRSelectFunction != null && crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction != null
                                //    && crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author != null)
                                //{
                                //    author = crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author;
                                //    if (author.IndexOf(" ") > -1)
                                //    {
                                //        author = author.Substring(0, author.IndexOf(" "));
                                //    }
                                //}

                                MapPolygonLayer polLayer = tlvIPoolMapPolyLayer;
                                string strValueField = polLayer.DataSet.DataTable.Columns[polLayer.DataSet.DataTable.Columns.Count - 1].ColumnName;

                                _columnName = strValueField;
                                polLayer.Symbology = CreateResultPolyScheme(ref polLayer, 6, "I"); //-MCB added

                                double dMinValue = 0.0;
                                double dMaxValue = 0.0;
                                dMinValue = crSelectFunctionCalculateValue.CRCalculateValues.Count == 0 ? 0 : crSelectFunctionCalculateValue.CRCalculateValues.Min(a => a.PointEstimate);
                                dMaxValue = crSelectFunctionCalculateValue.CRCalculateValues.Count == 0 ? 0 : crSelectFunctionCalculateValue.CRCalculateValues.Max(a => a.PointEstimate);

                                _dMinValue = dMinValue;
                                _dMaxValue = dMaxValue;
                                //_currentLayerIndex = mainMap.Layers.Count - 1;
                                polLayer.LegendText = author;
                                polLayer.Name = "Pooled Incidence:" + author; // "PIR_" + author;
                                _CurrentIMapLayer = polLayer;
                                string pollutantUnit = string.Empty;
                                _columnName = strValueField;
                                _CurrentMapTitle = CommonClass.MainSetup.SetupName + " Setup: Pooled Incidence-" + tlvIPoolMapPolyLayer.LegendText;
                                RenderMainMap(true, "I");


                            }
                            addRegionLayerGroupToMainMap();
                        }
                    }
                    i++;
                    CommonClass.GBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(iOldGridType);
                }
                int result = EnforceLegendOrder();
                WaitClose();
            }
            catch
            {
                WaitClose();
            }
        }

        private void btPoolingShowResult_Click(object sender, EventArgs e)
        {
            tlvIncidence_DoubleClick(sender, e);
        }
        int icbCRAggregationSelectIndexOld = -1;
        private void cbCRAggregation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCRAggregation.SelectedIndex != icbCRAggregationSelectIndexOld)
            {
                icbCRAggregationSelectIndexOld = cbCRAggregation.SelectedIndex;
                btShowCRResult_Click(sender, e);
            }
            tabCtlMain.Focus();
        }

        private void rbIncidenceOnlyOne_CheckedChanged(object sender, EventArgs e)
        {
            if (rbIncidenceOnlyOne.Checked)
            {
                lblIncidence.Enabled = true;
                cbPoolingWindowIncidence.Enabled = true;

            }
            else
            {
                lblIncidence.Enabled = false;
                cbPoolingWindowIncidence.Enabled = false;

            }
            cbPoolingWindowIncidence_SelectedIndexChanged(sender, e);
        }

        private void rbAPVOnlyOne_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAPVOnlyOne.Checked)
            {
                lblAPV.Enabled = true;
                cbPoolingWindowAPV.Enabled = true;
            }
            else
            {
                lblAPV.Enabled = false;
                cbPoolingWindowAPV.Enabled = false;

            }
            cbPoolingWindowAPV_SelectedIndexChanged(sender, e);

        }

        private void numericUpDownResult_ValueChanged(object sender, EventArgs e)
        {
            if (_tableObject == null) return;
            foreach (OLVColumn olvc in OLVResultsShow.Columns)
            {
                if (olvc.AspectToStringFormat != "" && olvc.AspectToStringFormat != null)
                {
                    olvc.AspectToStringFormat = "{0:N" + numericUpDownResult.Value + "}";
                }
            }
            UpdateTableResult(_tableObject);
        }

        private void btShowDetailIncidence_Click(object sender, EventArgs e)
        {
            if (btShowDetailIncidence.Text == "Show aggregated")
            {
                btShowDetailIncidence.Text = "Show pooled";
            }
            else
                btShowDetailIncidence.Text = "Show aggregated";
            cbPoolingWindowIncidence_SelectedIndexChanged(sender, e);
        }

        private void btShowDetailValuation_Click(object sender, EventArgs e)
        {
            if (btShowDetailValuation.Text == "Show aggregated")
            {
                btShowDetailValuation.Text = "Show pooled";
            }
            else
                btShowDetailValuation.Text = "Show aggregated";

            cbPoolingWindowAPV_SelectedIndexChanged(sender, e);


        }





        private void olvCRFunctionResult_Validated(object sender, EventArgs e)
        {

        }

        private void olvIncidence_Validated(object sender, EventArgs e)
        {
            foreach (OLVListItem item in (sender as ObjectListView).SelectedItems)
            {
                item.UseItemStyleForSubItems = true; item.BackColor = SystemColors.Highlight;
                item.ForeColor = Color.White;
            }
        }

        private void tlvAPVResult_Validated(object sender, EventArgs e)
        {
            foreach (OLVListItem item in (sender as ObjectListView).SelectedItems)
            {
                item.UseItemStyleForSubItems = true; item.BackColor = SystemColors.Highlight;
                item.ForeColor = Color.White;
            }
        }

        private void olvCRFunctionResult_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {


        }

        private void olvCRFunctionResult_MouseLeave(object sender, EventArgs e)
        {
            foreach (OLVListItem item in (sender as ObjectListView).SelectedItems)
            {
                item.UseItemStyleForSubItems = true; item.BackColor = SystemColors.Highlight;
                item.ForeColor = Color.White;
            }
        }

        private void cbGraph_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                zedGraphCtl.GraphPane = new GraphPane(new Rectangle(0, 0, zedGraphCtl.Width, zedGraphCtl.Height), "", "", "");
                switch (cbGraph.Text)
                {
                    case "Bar Graph":
                        olvRegions.Enabled = true;
                        groupBox1.Enabled = true;
                        groupBox9.Enabled = true;
                        btnApply.Enabled = true;
                        btnSelectAll.Enabled = true;
                        btnApply_Click(sender, e);
                        break;
                    case "Cumulative Distribution Functions":
                        olvRegions.Enabled = false;
                        groupBox1.Enabled = false;
                        groupBox9.Enabled = false;
                        btnSelectAll.Enabled = false;
                        btnApply.Enabled = false;
                        ShowCDFgraph();
                        break;
                }
            }
            catch
            { }
        }

        private void ShowCDFgraph()
        {
            try
            {
                GraphPane myPane = zedGraphCtl.GraphPane;
                switch (iCDF)
                {
                    case 0:
                        List<double> lstp = new List<double>();
                        int p = lstCFGRforCDF[0].CRCalculateValues[0].LstPercentile.Count;
                        double percentile = (double)50 / p * 0.01;
                        for (int i = 0; i < p; i++)
                        {
                            lstp.Add(percentile);
                            percentile = percentile + 100 / p * 0.01;
                        }
                        foreach (CRSelectFunctionCalculateValue cr in lstCFGRforCDF)
                        {
                            PointPairList list = new PointPairList();
                            for (int i = 0; i < p; i++)
                            {
                                list.Add(new PointPair(cr.CRCalculateValues[0].LstPercentile[i], lstp[i]));
                            }
                            myPane.AddCurve(cr.CRSelectFunction.BenMAPHealthImpactFunction.Author, list, GetRandomColor(), ZedGraph.SymbolType.None).Line.Width = 1.2F;
                        }

                        break;
                    case 1:
                        lstp = new List<double>();
                        p = lstCFGRpoolingforCDF[0].CRSelectFunctionCalculateValue.CRCalculateValues[0].LstPercentile.Count;
                        percentile = (double)50 / p * 0.01;
                        for (int i = 0; i < p; i++)
                        {
                            lstp.Add(percentile);
                            percentile = percentile + 100 / p * 0.01;
                        }
                        foreach (AllSelectCRFunction cr in lstCFGRpoolingforCDF)
                        {
                            PointPairList list = new PointPairList();
                            for (int i = 0; i < p; i++)
                            {
                                list.Add(new PointPair(cr.CRSelectFunctionCalculateValue.CRCalculateValues[0].LstPercentile[i], lstp[i]));
                            }
                            myPane.AddCurve(cr.Author, list, GetRandomColor(), ZedGraph.SymbolType.None).Line.Width = 1.2F;
                        }
                        break;
                    case 2:
                        lstp = new List<double>();
                        p = lstAPVRforCDF[0].lstAPVValueAttributes[0].LstPercentile.Count;
                        percentile = (double)50 / p * 0.01;
                        for (int i = 0; i < p; i++)
                        {
                            lstp.Add(percentile);
                            percentile = percentile + 100 / p * 0.01;
                        }
                        foreach (AllSelectValuationMethodAndValue value in lstAPVRforCDF)
                        {
                            PointPairList list = new PointPairList();
                            for (int i = 0; i < p; i++)
                            {
                                list.Add(new PointPair(value.lstAPVValueAttributes[0].LstPercentile[i], lstp[i]));
                            }
                            myPane.AddCurve(value.AllSelectValuationMethod.Author, list, GetRandomColor(), ZedGraph.SymbolType.None).Line.Width = 1.2F;
                        }
                        myPane.XAxis.Scale.Format = "$#0.####";
                        break;
                }
                myPane.IsFontsScaled = false; myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0F);
                myPane.YAxis.Scale.Format = "#0.####%";
                myPane.YAxis.Scale.Min = 0;
                myPane.YAxis.Scale.Max = 1;
                myPane.YAxis.MajorGrid.IsVisible = true;
                myPane.Title.Text = strCDFTitle;
                myPane.XAxis.Title.Text = strCDFX;
                myPane.YAxis.Title.Text = strCDFY;
                zedGraphCtl.AxisChange();
                zedGraphCtl.Refresh();
            }
            catch
            { }
        }

        public System.Drawing.Color GetRandomColor()
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            return Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255));
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            try
            {
                for (int j = 0; j < olvRegions.Items.Count; j++)
                {
                    OLVListItem olvi = olvRegions.Items[j] as OLVListItem;
                    olvi.Checked = true;
                }
                btnApply_Click(null, null);
            }
            catch
            { }
        }

        private Color[] GetColorRamp(string rampID, int numClasses)
        {
            // FYI: numClasses is just a stub for now- only 6 class color ramps have been created so far.-MCB

            //New empty color array to fill and return
            Color[] _colorArray = new Color[6];

            //Create a selection of color ramps to choose from
            //Note: Color ramps created using ColorBrewer 2.0: chose from color blind safe six class ramps. -MCB
            //Sequential- color ramps
            //single hue (light to dark)
            Color[] _oranges_Array = { Color.FromArgb(254, 237, 222), Color.FromArgb(254, 217, 118), Color.FromArgb(253, 174, 107), Color.FromArgb(253, 141, 60), Color.FromArgb(230, 85, 13), Color.FromArgb(166, 54, 3) };
            Color[] _purples_Array = { Color.FromArgb(242, 240, 247), Color.FromArgb(218, 218, 235), Color.FromArgb(188, 189, 220), Color.FromArgb(158, 154, 200), Color.FromArgb(117, 107, 177), Color.FromArgb(84, 39, 143) };
            Color[] _blues_Array = { Color.FromArgb(239, 243, 255), Color.FromArgb(198, 219, 239), Color.FromArgb(158, 202, 225), Color.FromArgb(107, 174, 214), Color.FromArgb(49, 130, 189), Color.FromArgb(8, 81, 156) };

            //multi-hue
            //pale_yellow_blue chosen as default ramp for main map (and default)
            Color[] _pale_yellow_blue_Array = { Color.FromArgb(240, 249, 232), Color.FromArgb(204, 235, 197), Color.FromArgb(168, 221, 181), Color.FromArgb(123, 204, 196), Color.FromArgb(67, 162, 202), Color.FromArgb(8,104,172) };        
            //Pale blue to green - an alternative mentioned by the client in an e-mail
            Color[] _pale_blue_green_Array = { Color.FromArgb(246, 239, 247), Color.FromArgb(208, 209, 230), Color.FromArgb(166, 189, 219), Color.FromArgb(103, 169, 207), Color.FromArgb(28, 144, 153), Color.FromArgb(1,108,89) };
            Color[] _yellow_red_Array = { Color.FromArgb(255, 255, 178), Color.FromArgb(254, 217, 118), Color.FromArgb(254, 178, 76), Color.FromArgb(253, 141, 60), Color.FromArgb(240, 59, 32), Color.FromArgb(189, 0, 38) };
            Color[] _yellow_blue_Array = { Color.FromArgb(255, 255, 204), Color.FromArgb(199, 233, 180), Color.FromArgb(127, 205, 187), Color.FromArgb(65, 182, 196), Color.FromArgb(44, 127, 184), Color.FromArgb(37, 52, 148) };
            Color[] _yellow_green_Array = { Color.FromArgb(255, 255, 204), Color.FromArgb(217, 240, 163), Color.FromArgb(173, 221, 142), Color.FromArgb(120, 198, 121), Color.FromArgb(49, 163, 84), Color.FromArgb(0, 104, 55) };

            //Diverging color ramps
            
            Color[] _brown_green_Array = { Color.FromArgb(140, 81, 10), Color.FromArgb(216, 179, 101), Color.FromArgb(246, 232, 195), Color.FromArgb(199, 234, 229), Color.FromArgb(90, 180, 172), Color.FromArgb(1, 102, 94) };
            Color[] _magenta_green_Array = { Color.FromArgb(197, 27, 125), Color.FromArgb(233, 163, 201), Color.FromArgb(253, 224, 239), Color.FromArgb(230, 245, 208), Color.FromArgb(161, 215, 106), Color.FromArgb(77, 146, 33) };
            //red_blue chosen as default by client for delta layers
            Color[] _red_blue_Array = { Color.FromArgb(215, 48, 39), Color.FromArgb(252, 141, 89), Color.FromArgb(254, 224, 144), Color.FromArgb(224, 243, 248), Color.FromArgb(145, 191, 219), Color.FromArgb(69, 117, 180) };
            Color[] _red_black_Array = { Color.FromArgb(178, 24, 43), Color.FromArgb(239, 138, 98), Color.FromArgb(253, 219, 199), Color.FromArgb(224, 224, 224), Color.FromArgb(153, 153, 153), Color.FromArgb(77, 77, 77) };
            Color[] _purple_green_Array = { Color.FromArgb(118, 42, 131), Color.FromArgb(175, 141, 195), Color.FromArgb(231, 212, 232), Color.FromArgb(217, 240, 211), Color.FromArgb(127, 191, 123), Color.FromArgb(27, 120, 55) };

            //Note: Could double the ramps by allowing the case hue names in reverse order and just reversing the array contents. 
            switch (rampID)
            {
                case "oranges":
                    _colorArray = _oranges_Array;
                    break;
                case "purples":
                    _colorArray = _purples_Array;
                    break;
                case "blues":
                    _colorArray = _blues_Array;
                    break;

                case "yellow_red":
                    _colorArray = _yellow_red_Array;
                    break;
                case "pale_yellow_blue":
                    _colorArray = _pale_yellow_blue_Array;
                    break;
                case "pale_blue_green":
                    _colorArray = _pale_blue_green_Array;
                    break;
                case "yellow_blue":
                    _colorArray = _yellow_blue_Array;
                    break;
                case "yellow_green":
                    _colorArray = _yellow_green_Array;
                    break;
               
                case "brown_green":
                    _colorArray = _brown_green_Array;
                    break;
                case "magenta_green":
                    _colorArray = _magenta_green_Array;
                    break;
                case "red_blue":
                    _colorArray = _red_blue_Array;
                    break;
                case "blue_red":
                    _colorArray = (System.Drawing.Color[])_red_blue_Array.Reverse();
                    break;
                case "red_black":
                    _colorArray = _red_black_Array;
                    break;
                case "purple_green":
                    _colorArray = _purple_green_Array;
                    break;
            }

            return _colorArray;
        }

        private void olvCRFunctionResult_DragLeave(object sender, EventArgs e)
        {
            Debug.WriteLine("olvCRFunctionResul_DragLeave");
            _HealthResultsDragged = true;
             return;
        }

        private void olvCRFunctionResult_DragDrop(object sender, DragEventArgs e)
        {
            Debug.WriteLine("olvCRFunctionResult_DragDrop");
            //
            //{
             //  btShowCRResult_Click(sender, e);
                 _HealthResultsDragged = false;
            //}
            return;
        }
        private void mainMap_DragEnter(object sender, DragEventArgs e)
        {
            Debug.WriteLine("mainMap_DragEnter");
            //_HealthResultsDragged = true;
            if (_HealthResultsDragged)
            {
                btShowCRResult_Click(sender, e);
                _HealthResultsDragged = false;
            }

            if (_IncidenceDragged)
            {
                tlvIncidence_DoubleClick(sender, e);
                _IncidenceDragged = false;
            }

            if (_APVdragged)
            {
                tlvAPVResult_DoubleClick(sender, e);
                _APVdragged = false;
            }
            return;
        }
        private void mainMap_DragLeave(object sender, EventArgs e)
        {
            Debug.WriteLine("mainMap_DragLeave"); 
            _HealthResultsDragged = false;
            _IncidenceDragged = false;
            _APVdragged = false;
            return;
        }

        private void mainMap_DragDrop(object sender, DragEventArgs e)
        {   
            //Debug.WriteLine("mainMap_DragDrop");
            //if (_HealthResultsDragged)
            //{  
            //    btShowCRResult_Click(sender, e);
            //    return;
            //}
        }

        private void picGIS_DragEnter(object sender, DragEventArgs e)
        {
            //Debug.WriteLine("picGIS_DragEnter");
            //_HealthResultsDragged = true;
            //if (_HealthResultsDragged)
            //{
            //    btShowCRResult_Click(sender, e);
            //    _HealthResultsDragged = false;
            //    return;
            //}
            //return;
        }

        private void tlvIncidence_DragLeave(object sender, EventArgs e)
        {
            Debug.WriteLine("tlvIncidence_DragLeave");
            _IncidenceDragged = true;
            return;
        }
		//MERGE Check
        private void textBoxFilterSimple_TextChanged(object sender, EventArgs e)
        {
            this.TimedFilter(this.treeListView, textBoxFilterSimple.Text);
        }
        private void tlvAPVResult_DragLeave(object sender, EventArgs e)
        {
            Debug.WriteLine("APVdragged_DragLeave");
            _APVdragged = true;
            return;
        }

        private void btnShowHideAttributeTable_Click(object sender, EventArgs e)
        {
            //User has to select a feature layer to see it's attribute table.  If none selected then show the attribute table of the top visible layer.  If none then don't show an attribute table.
            //If the selected layer is not a feature layer (point, line, polygon_ then don't show an attribute table.

            if (mainMap.GetAllLayers().Count > 0) //Only perform if any featurelayers present
            {
                ILayer SelLayer = null;
                string LayerType;
                MapPolygonLayer SelPolyMapLayer;
                MapPointLayer SelPointLayer;
                MapLineLayer SelLineMapLayer;

                //Get the selected layer
                SelLayer = mainMap.Layers.SelectedLayer;
                if (SelLayer == null)                    //Use the top visible layer
                {
                    SelLayer = FindTopVisibleLayer(false);
                    Debug.WriteLine("No layer selected, top visible layer used instead to dsplay the attribute table of");
                }

                if (SelLayer == null)                   //No layers are visible on the map
                {
                    Debug.WriteLine("User tried to show an attribute table When none of the layers are visible or selected.");
                    return;
                }

                //Get it's type
                LayerType = SelLayer.ToString();
                switch (LayerType.ToLower())
                {
                    case "dotspatial.controls.mappolygonlayer":
                        SelPolyMapLayer = (MapPolygonLayer)SelLayer;
                        SelPolyMapLayer.ShowAttributes();
                        break;
                    case "dotspatial.controls.mapmultipointlayer":
                    case "dotspatial.controls.mappointlayer":
                        SelPointLayer = (MapPointLayer)SelLayer;
                        SelPointLayer.ShowAttributes();
                        break;
                    case "dotspatial.controls.mappolylinelayer":
                    case "dotspatial.controls.maplinelayer":
                        SelLineMapLayer = (MapLineLayer)SelLayer;
                        SelLineMapLayer.ShowAttributes();
                        break;
                    default:
                        Debug.WriteLine("user tried to show the Attribute table for a non-feature layer.");
                        break;
                }
            }
            else
            {
                Debug.WriteLine("No Layers to display the attribute table of");
            }
            return;
            //-----------------OLD WAY-------------------------
            //if (dgvAttributeTable.Visible == false)
            //{
            //    WaitShow("Loading Table...");                                                               //Need to change data source to 1st selected layer (if none selected, then first feature layer)
            //    bool selLayerFound = false;
            //    List<ILayer> ILlist = mainMap.GetAllLayers();
                   
            //    MapLayerCollection FLlist = new MapLayerCollection();// Get just the feature layers, within map groups too)
            //    string strLayerType = null;
               
            //    foreach (IMapLayer aLayer in ILlist)
            //    {
            //        strLayerType = aLayer.ToString();
            //        strLayerType = strLayerType.Replace("DotSpatial.Controls.", "");
            //        if (strLayerType == "MapPointLayer" | strLayerType == "MapPolygonLayer" | strLayerType == "MapLineLayer")
            //        {
            //            FLlist.Add(aLayer);
            //        }
            //    }

            //    if (FLlist != null && FLlist.Count > 0)                      // if featurelayers are present 
            //    {
            //        foreach (IFeatureLayer fLayer in FLlist) // find the first selected feature layer
            //        {
            //            if (fLayer.IsSelected && !selLayerFound)
            //            {                          
            //                if (dgvAttributeTable.DataSource == (null) || !dgvAttributeTable.DataSource.Equals(fLayer.DataSet.DataTable))
            //                {   
            //                    if (!fLayer.DataSet.AttributesPopulated) fLayer.DataSet.FillAttributes();
            //                    dgvAttributeTable.DataSource = fLayer.DataSet.DataTable;
            //                    _CurrentMapTableTitle = fLayer.LegendText;
            //                }
            //                selLayerFound = true;
            //                break;
            //            }
            //        }
            //        if (!selLayerFound)                      //if no feature layers selected, then use the last feature layer added
            //        {
            //            IFeatureLayer fLayer = (IFeatureLayer)FLlist[0];
            //            if (dgvAttributeTable.DataSource == null || !dgvAttributeTable.DataSource.Equals(fLayer.DataSet.DataTable))
            //                {
            //                    if (!fLayer.DataSet.AttributesPopulated) fLayer.DataSet.FillAttributes();
            //                    dgvAttributeTable.DataSource = fLayer.DataSet.DataTable;
            //                    _CurrentMapTableTitle = fLayer.LegendText;
            //                }
            //        }
                    
            //                                                 // Make the table visible and change the button's text (button acts as a toggle between map and table)
            //        dgvAttributeTable.Visible = true;
            //        _SavedExtent = mainMap.Extent;
            //        tabMapLayoutPanel1.SetRow(mainMap, 2);
            //        tabMapLayoutPanel1.SetRow(dgvAttributeTable, 1);
                    
            //        tbMapTitle.Text = _CurrentMapTableTitle;
            //        btnShowHideAttributeTable.Text = "Map";  // button now allows the user to switch to the map
            //        Debug.WriteLine("Attribute table displayed");
            //    }
            //    else                                          // No feature layers present
            //    {
            //         // Notify user that no features are present or do nothing? MCB-
            //    }
            //    WaitClose();
            //}
            //else
            //{
            //    dgvAttributeTable.Visible = false;
            //    tabMapLayoutPanel1.SetRow(dgvAttributeTable, 2);
            //    tabMapLayoutPanel1.SetRow(mainMap, 1);
            //    mainMap.ViewExtents = _SavedExtent;
            //    btnShowHideAttributeTable.Text = "Attribute Table";  //button now allows the user to switch to the attribute table of the 1st selected layer
               
            //    tbMapTitle.Text = _CurrentMapTitle;
                
            //    Debug.WriteLine("Map displayed");
            //}
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void OLVResultsShow_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}