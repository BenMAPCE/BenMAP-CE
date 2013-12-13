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
//using DataWorker;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using ZedGraph;
using ESIL.DBUtility;
using System.Configuration;

namespace BenMAP
{
    public partial class BenMAP : FormBase
    {
        #region fields
        BenMAPGrid chartGrid;//when show pooled incidence and valuation -> show aggregate grid in chart

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
        /// <summary>
        /// 表示已经有设置的图标key,参数文件里的设置必须与这里保持一致
        /// </summary>
        private const string _readyImageKey = "ready";

        /// <summary>
        /// 表示尚未设置的图标key,参数文件里的设置必须与这里保持一致
        /// </summary>
        private const string _unreadyImageKey = "unready";

        private const string _yibuImageKey = "yibu";

        private const string _errorImageKey = "error";

        /// <summary>
        /// 基本的窗体标题,包括应用程序名称和版本
        /// </summary>
        private string _baseFormTitle = "";
        /// <summary>
        /// 控制主窗口
        /// </summary>
        public Main mainFrm = null;

        private List<string> _listAddGridTo36km = new List<string>();
        private string _reportTableFileName = "";

        private FeatureSet _fsregion = new FeatureSet();
        private bool isLegendHide = false;
        private object LayerObject = null;//存储图层对象
        private string _currentNode = string.Empty;// 当前节点
        private string _homePageName;

        public string HomePageName
        {
            get { return _homePageName; }
            set { _homePageName = value; }
        }

        string chartXAxis = "";//cbChartXAxis
        string strchartTitle = "";//chart的标题
        string strchartX = "";//chart的横坐标轴标题
        string strchartY = "";//chart的纵坐标轴标题
        string strCDFTitle = "";
        string strCDFX = "";
        string strCDFY = "";
        int iCDF = -1;//指示画的是cfgr,incidence pooing,还是apvr
        bool canshowCDF = false;
        List<CRSelectFunctionCalculateValue> lstCFGRforCDF = new List<CRSelectFunctionCalculateValue>();
        List<AllSelectCRFunction> lstCFGRpoolingforCDF = new List<AllSelectCRFunction>();
        List<AllSelectValuationMethodAndValue> lstAPVRforCDF = new List<AllSelectValuationMethodAndValue>();
        #endregion fields

        public BenMAP(string homePageName)
        {
            try
            {
                InitializeComponent();
                //Meta.Numerics.Statistics.Distributions.ChiSquaredDistribution chi = new Meta.Numerics.Statistics.Distributions.ChiSquaredDistribution(5);
                //double d= chi.RightProbability(16.3);
                Control.CheckForIllegalCrossThreadCalls = false;
                _homePageName = homePageName;
                //初始化的时候所有控件不显示
                splitContainer1.Visible = false;
                zedGraphCtl.Visible = false;
                this.tabCtlReport.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
                this.tabCtlReport.DrawItem += new DrawItemEventHandler(DrawTabControlItems);

                // 
                CommonClass.NodeAnscy -= ChangeNodeStatus;
                CommonClass.NodeAnscy += ChangeNodeStatus;

                //初始化GIS左上角Panel的图片
                //System.Drawing.Image backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\Chart.jpg");
                //pnlSpatial.BackgroundImage = backImg;
                ////初始化RSM Tab里panel的图片
                //if (CommonClass.ActiveSetup == "China")
                //{
                //    backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\RSM_PM2.5_China.jpg");
                //    // pnlRSM.BackgroundImage = backImg;
                //}
                //else
                //{
                //    backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\RSM_PM2.5.png");
                //    // pnlRSM.BackgroundImage = backImg;
                //}
                //// 刷新报表列表
                //ResetReportList();
                //CommonClass.CurrentMainFormStat = "Current Setup:" + CommonClass.MainSetup.SetupName;
                mainMap.LayerAdded += new EventHandler<LayerEventArgs>(mainMap_LayerAdded);
                //mainMap.
                //mainMap.l
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
                //Debug.WriteLine(_count++);
                lock (CommonClass.NodeAnscyStatus)
                {
                    if (CommonClass.NodeAnscyStatus != string.Empty)
                    {
                        ansy = CommonClass.NodeAnscyStatus;
                        string[] tmps = ansy.Split(new char[] { ';' });// 0 是污染物；1是节点；2是开始还是结束的状态：on/off
                        _currentPollutant = tmps[0].ToLower();
                        _currentAnsyNode = tmps[1];
                        if (tmps[2] == "on") { _currentImage = _yibuImageKey; }
                        else if (tmps[2] == "off") { _currentImage = _readyImageKey; }
                        foreach (TreeNode node in trvSetting.Nodes)
                        {
                            RecursiveQuery(node);
                        }
                    }
                }
                //if (CommonClass.NodeAnscyStatus != string.Empty && _currentNode != string.Empty)
                //{

                //    lock (CommonClass.LstAsynchronizationStates)
                //    {
                //        if (CommonClass.LstAsynchronizationStates.Count > 0)
                //        {
                //            //foreach (string str in CommonClass.LstAsynchronizationStates)
                //            //{
                //            //    lock (CommonClass.NodeAnscyStatus) { ansy = CommonClass.NodeAnscyStatus; }
                //            //    string[] tmps = ansy.Split(new char[]{';'});
                //            //    if(tmps[0]==_currentNode)
                //            //    _currentImage = _yibuImageKey;
                //            //    foreach (TreeNode node in trvSetting.Nodes)
                //            //    {
                //            //        RecursiveQuery(node);
                //            //    }
                //            //}// LstAsynchronizationStates
                //            lock (CommonClass.NodeAnscyStatus) { ansy = CommonClass.NodeAnscyStatus; }
                //            string[] tmps = ansy.Split(new char[] { ';' });
                //            _currentAnsyNode = tmps[0];
                //            if (tmps[1] == "on") { _currentImage = _yibuImageKey; }
                //            else if (tmps[1] == "off") { _currentImage = _readyImageKey; }
                //            foreach (TreeNode node in trvSetting.Nodes)
                //            {
                //                RecursiveQuery(node);
                //            }
                //        }
                //        else
                //        {
                //            _currentImage = _readyImageKey;
                //            foreach (TreeNode node in trvSetting.Nodes)
                //            {
                //                RecursiveQuery(node);
                //            }
                //        }
                //    }//Lock
                //}

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        /// <summary>
        /// 递归遍历树的节点
        /// </summary>
        /// <param name="node"></param>
        private void RecursiveQuery(TreeNode node)
        {
            try
            {
                //var b = node.Tag as BenMAPLine;


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
                    // tabCtlReport.SelectedIndex = i;
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
                //tabFont.si
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
                //else if ((tag == "qaly") && (cbPoolingWindowQALY != null && cbPoolingWindowQALY.Items.Count > 0))
                //{
                //    bForeColor = Brushes.Black;
                //    if (tag == selectedPage)
                //    {
                //        bBackColor = new SolidBrush(Color.White);
                //    }
                //    else
                //    {
                //        bBackColor = new LinearGradientBrush(e.Bounds, SystemColors.Control, SystemColors.Control, System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal);
                //    }
                //    iImageKey = 13;
                //}
                else if (tag == "audit")
                {
                    // && ((CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count > 0) ||
                    //  (cbPoolingWindowAPV != null && cbPoolingWindowAPV.Items.Count > 0) || (cbPoolingWindowQALY != null && cbPoolingWindowQALY.Items.Count > 0))
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
                //画样式
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
            //throw new NotImplementedException();
        }

        #region menus

        /// <summary>
        ///
        /// </summary>
        /// <param name="tree">TreeView控件</param>
        /// <returns></returns>
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

        /// <summary>
        /// 新建BenMAP
        /// </summary>
        public void NewFile()
        {
            splitContainer1.Visible = true;
            CommonClass.ClearAllObject();
            //清空上次内容
            ClearAll();
            //初始化treeView，装载treeView控件根节点（必须数据）
            // ResetParamsTree(Application.StartupPath + @"\Configs\ParamsTreeForNew.xml");
            ResetParamsTree(Application.StartupPath + @"\Configs\ParamsTree_USA.xml");
        }

        /// <summary>
        /// 清楚上次打开或新建的内容
        /// </summary>
        public void ClearAll()
        {
            try
            {
                // axMap2.RemoveAllLayers();   //去除GIS控件的全部图层
                this.picGIS.Visible = true;
                mainMap.Layers.Clear();
                pnlChart.BackgroundImage = null;   //panel的背景图标清空
                //dgvData = new DataGridView();
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
                CommonClass.LstPollutant = null;//污染物列表
                CommonClass.RBenMAPGrid = null;//系统选择的Grid

                CommonClass.GBenMAPGrid = null;//系统选择的Region
                CommonClass.LstBaseControlGroup = null;//系统设置的DataSource Base and Control
                //public  List<CRSelectFunction> LstCRSelectFunction;//系统选择的所有Health Impact Function
                CommonClass.CRThreshold = 0;//阈值
                CommonClass.CRLatinHypercubePoints = 10;//拉丁立体方采样点数
                CommonClass.CRRunInPointMode = false;//是否使用拉丁立体方方采样

                CommonClass.BenMAPPopulation = null;//系统设置的Population

                CommonClass.BaseControlCRSelectFunction = null;//所有BaseControlAndCRSelectFunciton
                CommonClass.BaseControlCRSelectFunctionCalculateValue = null;//所有BaseControlAndCRSelectFunciton以及Value

                //-------------------APVX-------------------------------
                //CommonClass.IncidencePoolingAndAggregationAdvance = null;//Advance
                CommonClass.lstIncidencePoolingAndAggregation = null;
                //public  List<ValuationMethodPoolingAndAggregation> lstValuationMethodPoolingAndAggregation;
                //public  IncidencePoolingAndAggregation IncidencePoolingAndAggregation;//IncidencePooling;

                CommonClass.IncidencePoolingResult = null;
                CommonClass.ValuationMethodPoolingAndAggregation = null;
                //GC.Collect();
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
                    //cbPoolingWindowQALY.Items.Clear();
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

                            //CommonClass.LstPollutant = CommonClass.BaseControlCRSelectFunction.BaseControlGroup.Select(pa => pa.Pollutant).ToList();
                            //CommonClass.lst
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        Logger.LogError(ex);
                    }
                    //-------------modify by xiejp因为project过于大，现只保存类似APVX的设置去掉APVX,QALY,CR的显示，并把它置红----------------------------------

                    errorNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);

                    errorNodeImage(trvSetting.Nodes[2].Nodes[1]);
                    errorNodeImage(trvSetting.Nodes[2].Nodes[2]);

                    //foreach (ValuationMethodPoolingAndAggregationBase vbAPVFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                    //{
                    //    cbPoolingWindowAPV.Items.Add(vbAPVFrom.IncidencePoolingAndAggregation.PoolingName);
                    //}
                    //cbPoolingWindowAPV.SelectedIndex = 0;

                    ////------------------------------显示QALY--------------------------------------------------------------

                    //if (CommonClass.ValuationMethodPoolingAndAggregation != null && CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase != null && CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(a => a.lstAllSelectQALYMethodAndValue != null).Count() > 0)
                    //{
                    //    cbPoolingWindowQALY.Items.Clear();
                    //    foreach (ValuationMethodPoolingAndAggregationBase vbQALYFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(a => a.lstAllSelectQALYMethodAndValue != null))
                    //    {
                    //        cbPoolingWindowQALY.Items.Add(vbQALYFrom.IncidencePoolingAndAggregation.PoolingName);
                    //    }
                    //    cbPoolingWindowQALY.SelectedIndex = 0;
                    //}

                    //foreach (TreeNode trnd in this.trvSetting.Nodes["aggregationpoolingvaluation"].Nodes)
                    //{
                    //    changeNodeImage(trnd);
                    //}
                    //olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
                }
                else if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null)
                {
                    //olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
                    //if (CommonClass.IncidencePoolingAndAggregationAdvance != null)
                    //{
                    //    changeNodeImage(trvSetting.Nodes["aggregationpoolingvaluation"].Nodes["aggregation"]);
                    //}
                    //if (CommonClass.lstIncidencePoolingAndAggregation != null && CommonClass.lstIncidencePoolingAndAggregation.Count > 0)
                    //{
                    //    changeNodeImage(trvSetting.Nodes["aggregationpoolingvaluation"].Nodes["poolingmethod"]);
                    //}

                    showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, this.trvSetting.Nodes["aggregationpoolingvaluation"]);
                    errorNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
                }
                else if (CommonClass.BaseControlCRSelectFunction != null)
                {
                    errorNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
                    showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, this.trvSetting.Nodes["aggregationpoolingvaluation"]);

                    //CommonClass.LstPollutant = CommonClass.BaseControlCRSelectFunction.BaseControlGroup.Select(pa => pa.Pollutant).ToList();
                    //CommonClass.lst
                }
                else if (CommonClass.LstBaseControlGroup != null && CommonClass.LstBaseControlGroup.Count > 0)
                {
                    int nodesCount = 0;
                    //--------------变绿并且加节点----------------------------------------------------------------------
                    // TreeNode tr = currentNode.Parent;
                    foreach (TreeNode trchild in trvSetting.Nodes)
                    {
                        if (trchild.Name == "airqualitygridgroup")
                        {
                            nodesCount = trchild.Nodes.Count;

                            //currentNode.Tag = CommonClass.LstPollutant;

                            for (int i = nodesCount - 1; i > -1; i--)
                            {
                                TreeNode node = trchild.Nodes[i];
                                if (trchild.Nodes[i].Name == "datasource") { trchild.Nodes.RemoveAt(i); }
                            }
                            // CommonClass.LstBaseControlGroup = new List<BaseControlGroup>(CommonClass.LstPollutant.Count);
                            // for (int i = 0; i < CommonClass.LstPollutant.Count; i++)
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

                            // //Todo:设置灰色 陈志润
                            //tabCRFunctionResultGISShow.BackColor = Color.Gray;
                            //tabAPVResultGISShow.BackColor = Color.Gray;
                            //tabQLAYResultShow.BackColor = Color.Gray;
                            //tabAuditTrialReport.BackColor = Color.Gray;
                            olvCRFunctionResult.SetObjects(null);
                            olvIncidence.SetObjects(null);
                            tlvAPVResult.SetObjects(null);
                            //tlvQALYResult.SetObjects(null);
                            cbPoolingWindowIncidence.Items.Clear();
                            cbPoolingWindowAPV.Items.Clear();
                            //cbPoolingWindowQALY.Items.Clear();
                            //----------变黄-----------
                            ClearMapTableChart();
                            initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);

                            initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
                            initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
                            return;
                        }

                        trvSetting.Nodes["pollutant"].Parent.ExpandAll();
                    }
                }
                //------------modify by xiejp ---Population和Aggregation可以单独出现---需要分别变绿---
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
            //-------------重新设置Aggregation--------------------------
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
            //cbQALYAggregation.DataSource = dsQALYAggregationGridType.Tables[0];
            //cbQALYAggregation.DisplayMember = "GridDefinitionName";
            //cbQALYAggregation.SelectedIndex = 0;
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
            //---------end --------------------------------
        }
        /// <summary>
        /// 打开一个BenMAP
        /// </summary>
        public void OpenFile()
        {
            try
            {
                //--------------提醒他是否保存project---------------

                splitContainer1.Visible = true;
                CommonClass.ClearAllObject();
                CommonClass.CRSeeds = 1;
                //清空上次内容
                ClearAll();
                //初始化treeView，装载treeView控件根节点（必须数据）
                //InitTreeView(trvSetting);
                ResetParamsTree("");
                //全面清空CommonClass
                //CommonClass.ManageSetup = null;
                //CommonClass.MainSetup = null;// 当前活动区域

                ClearMapTableChart();
                initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);

                initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
                initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
                CommonClass.IncidencePoolingAndAggregationAdvance = null;
                if (CommonClass.IncidencePoolingAndAggregationAdvance != null)
                {
                    changeNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);

                }
                ////Todo:设置灰色 陈志润
                //tabCRFunctionResultGISShow.BackColor = Color.Gray;
                //tabAPVResultGISShow.BackColor = Color.Gray;
                //tabQLAYResultShow.BackColor = Color.Gray;
                //tabAuditTrialReport.BackColor = Color.Gray;
                olvCRFunctionResult.SetObjects(null);
                olvIncidence.SetObjects(null);
                tlvAPVResult.SetObjects(null);
                //tlvQALYResult.SetObjects(null);

                cbPoolingWindowIncidence.Items.Clear();
                cbPoolingWindowAPV.Items.Clear();
                //cbPoolingWindowQALY.Items.Clear();
                ClearMapTableChart();
                //Todo:陈志润
                SetTabControl(tabCtlReport);
                HealthImpactFunctions.MaxCRID = 0;
                BenMAP_Load(this, null);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        /// <summary>
        /// 打开最近文件（Usa case, China case）
        /// </summary>
        /// <param name="filePath"></param>
        public void OpenFile(string filePath)
        {
            try
            {
                splitContainer1.Visible = true;
                //清空上次内容
                ClearAll();
                //初始化treeView，装载treeView控件根节点（必须数据）
                ResetParamsTree(filePath);
                //把已有的数据内容添加到CommonClass的公共属性
                string chinaOrUSA = System.IO.Path.GetFileNameWithoutExtension(filePath);
                chinaOrUSA = chinaOrUSA.Substring(chinaOrUSA.LastIndexOf('_') + 1, chinaOrUSA.Length - chinaOrUSA.LastIndexOf('_') - 1);
                switch (chinaOrUSA)
                {
                    case "China":
                        //更改pnlRSM的背景
                        System.Drawing.Image backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\RSM_PM2.5_China.jpg");
                        // pnlRSM.BackgroundImage = backImg;
                        //加载中国区图层
                        string shapeRoot = string.Format(@"{0}\Data\ChinaData\ShapeFile\", Application.StartupPath);
                        string shapeFile = shapeRoot + "China_Region.shp";
                        //LoadDotSpatialGIS(mainMap, shapeFile);
                        break;
                    case "USA":
                        //更改pnlRSM的背景
                        backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\RSM_PM2.5.png");
                        // pnlRSM.BackgroundImage = backImg;
                        shapeRoot = string.Format(@"{0}\Data\USAData\ShapeFile\", Application.StartupPath);
                        shapeFile = shapeRoot + "US_Region.shp";
                        //LoadDotSpatialGIS(mainMap, shapeFile);
                        break;
                    default:
                        //更改pnlRSM的背景
                        backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\RSM_PM2.5_China.jpg");
                        //  pnlRSM.BackgroundImage = backImg;
                        //加载地图
                        // LoadGIS(axMap2);
                        backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\RSM_PM2.5.png");
                        //pnlRSM.BackgroundImage = backImg;
                        shapeRoot = string.Format(@"{0}\Data\USAData\ShapeFile\", Application.StartupPath);
                        shapeFile = shapeRoot + "US_Region.shp";
                        //LoadDotSpatialGIS(mainMap, shapeFile);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        #endregion menus

        /// <summary>
        /// 从当前节点及其子节点中,查找Text为指定值的节点
        /// </summary>
        /// <param name="root">父节点</param>
        /// <param name="nodeText">要查找的字段</param>
        /// <returns></returns>
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

                    //currentNode.Tag = CommonClass.LstPollutant;

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

                            if (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0)
                            {
                                //------------change base ico---------
                                node.Nodes[0].Nodes[0].ImageKey = "doc";
                                node.Nodes[0].Nodes[0].SelectedImageKey = "doc";
                            }
                            else if (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count == 0)
                            {
                                errorNodeImage(node);
                                //errorNodeImage(node.Nodes[1]);

                            }
                            if (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count > 0)
                            {
                                //------------change base ico---------
                                node.Nodes[1].Nodes[0].ImageKey = "doc";
                                node.Nodes[1].Nodes[0].SelectedImageKey = "doc";
                            }
                            else if (bcg.Control != null && bcg.Control.ModelResultAttributes != null && bcg.Control.ModelResultAttributes.Count == 0)
                            {
                                //errorNodeImage(node.Nodes[1]);
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

        /// <summary>
        /// Add Data Source Node
        /// </summary>
        /// <param name="pollutantName">选择污染物的名称</param>
        /// <returns></returns>
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
                Extent et = mainMap.Extent;
                splitContainer2.Panel1.Hide();
                splitContainer2.SplitterDistance = 0;
                splitContainer2.BorderStyle = BorderStyle.None;
                isLegendHide = true;
                mainMap.ViewExtents = et;
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }
        private int iGridTypeOld = -1;
        /// <summary>
        /// 双击相应的节点，可以调出设置页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trvSetting_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                iGridTypeOld = CommonClass.MainSetup.SetupID;
                TreeNode currentNode = trvSetting.SelectedNode;// (sender as TreeNode);// trvSetting.SelectedNode;
                if (currentNode == null) { currentNode = (sender as TreeNode); }
                string nodeName = currentNode.Name.ToLower();
                string nodeTag = string.Empty;
                TreeNode parentNode = currentNode.Parent as TreeNode;
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
                        //改变该节点的图标为“ready”
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
                                    // currentNode.Parent.Nodes[i].Text = "Data Source";
                                    // currentNode.Parent.Nodes[i].Nodes[0].Tag = null;

                                    currentNode.Parent.Nodes[i].Nodes[0].Nodes.Clear();
                                    initNodeImage(currentNode.Parent.Nodes[i].Nodes[0]);
                                    //  currentNode.Parent.Nodes[i].Nodes[1].Tag = null;
                                    currentNode.Parent.Nodes[i].Nodes[1].Nodes.Clear();
                                    initNodeImage(currentNode.Parent.Nodes[i].Nodes[1]);
                                    initNodeImage(currentNode.Parent.Nodes[i]);
                                }
                            }

                            //if (CommonClass.IncidencePoolingResult == null)
                            //{
                            //    olvCRFunctionResult.SetObjects(null);
                            //    tlvAPVResult.SetObjects(null);
                            //}

                            //CommonClass.BaseControlCRSelectFunction = null;

                            if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null)
                            {
                                CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue = null;
                            }
                            //CommonClass.lstIncidencePoolingAndAggregation = null;
                            //CommonClass.IncidencePoolingAndAggregationAdvance = null;
                            //CommonClass.IncidencePoolingResult = null;
                            //CommonClass.ValuationMethodPoolingAndAggregation = null;
                            //Todo:设置灰色 陈志润
                            //tabCRFunctionResultGISShow.BackColor = Color.Gray;
                            //tabAPVResultGISShow.BackColor = Color.Gray;
                            //tabQLAYResultShow.BackColor = Color.Gray;
                            //tabAuditTrialReport.BackColor = Color.Gray;
                            olvCRFunctionResult.SetObjects(null);
                            olvIncidence.SetObjects(null);
                            tlvAPVResult.SetObjects(null);
                            //tlvQALYResult.SetObjects(null);

                            cbPoolingWindowIncidence.Items.Clear();
                            cbPoolingWindowAPV.Items.Clear();
                            //cbPoolingWindowQALY.Items.Clear();
                            ClearMapTableChart();
                            //----------变黄-----------
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
                        // 画边界图网格图层
                        mainMap.Layers.Clear();
                        HideSplitContainer2();
                        mainMap.ProjectionModeReproject = ActionMode.Never;
                        mainMap.ProjectionModeDefine = ActionMode.Never;
                        tabCtlMain.SelectedIndex = 0;
                        if (currentNode.Tag is ShapefileGrid)
                        {
                            mainMap.Layers.Clear();
                            if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (currentNode.Tag as ShapefileGrid).ShapefileName + ".shp"))
                            {
                                mainMap.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (currentNode.Tag as ShapefileGrid).ShapefileName + ".shp");
                            }
                        }
                        else if (currentNode.Tag is RegularGrid)
                        {
                            mainMap.Layers.Clear();
                            if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (currentNode.Tag as RegularGrid).ShapefileName + ".shp"))
                            {
                                mainMap.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (currentNode.Tag as RegularGrid).ShapefileName + ".shp");
                            }
                        }
                        //HideSplitContainer2();
                        PolygonLayer player = mainMap.Layers[0] as PolygonLayer;
                        float f = (float)0.9;
                        Color c = Color.Transparent;
                        PolygonSymbolizer Transparent = new PolygonSymbolizer(c);

                        //Transparent.OutlineSymbolizer = new LineSymbolizer(Color.Black, 1);
                        Transparent.OutlineSymbolizer = new LineSymbolizer(Color.DarkBlue, 1);     //设置region图层outline宽度为2
                        player.Symbolizer = Transparent;
                        LayerObject = null;
                        break;
                    case "region":
                        _currentNode = "region";
                        HideSplitContainer2();
                        mainMap.Layers.Clear();
                        // 画区域图层
                        tabCtlMain.SelectedIndex = 0;
                        addRegionLayerToMainMap();
                        LayerObject = null;
                        break;
                    case "pollutant":
                        _currentNode = "pollutant";
                        if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0)
                        {
                            MessageBox.Show(string.Format("BenMAP is still creating the air quality surface map."), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        BenMAPPollutant[] benMAPPollutantArray = null;//
                        if (CommonClass.LstPollutant != null)
                        {
                            benMAPPollutantArray = CommonClass.LstPollutant.ToArray();
                        }
                        frm = new Pollutant();
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
                        //改变该节点的图标为“ready”
                        changeNodeImage(currentNode);
                        //ChangeNodeStatus();
                        int nodesCount = currentNode.Parent.Nodes.Count;
                        if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0)
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
                            CommonClass.BaseControlCRSelectFunction = null;
                            CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
                            CommonClass.lstIncidencePoolingAndAggregation = null;
                            //CommonClass.IncidencePoolingAndAggregationAdvance = null;
                            CommonClass.IncidencePoolingResult = null;
                            CommonClass.ValuationMethodPoolingAndAggregation = null;
                            ////Todo:设置灰色 陈志润
                            //tabCRFunctionResultGISShow.BackColor = Color.Gray;
                            //tabAPVResultGISShow.BackColor = Color.Gray;
                            //tabQLAYResultShow.BackColor = Color.Gray;
                            //tabAuditTrialReport.BackColor = Color.Gray;
                            olvCRFunctionResult.SetObjects(null);
                            olvIncidence.SetObjects(null);
                            tlvAPVResult.SetObjects(null);
                            //tlvQALYResult.SetObjects(null);

                            cbPoolingWindowIncidence.Items.Clear();
                            cbPoolingWindowAPV.Items.Clear();
                            //cbPoolingWindowQALY.Items.Clear();
                            ClearMapTableChart();
                            //----------变黄-----------
                            ClearMapTableChart();
                            initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);
                            foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes)
                            {
                                initNodeImage(tn);
                            }
                            foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes)
                            {
                                initNodeImage(tn);
                            }

                            return;
                        }
                        else if (benMAPPollutantArray == null || (benMAPPollutantArray != null && CommonClass.lstPollutantAll.Count != benMAPPollutantArray.Count()) ||
                            (benMAPPollutantArray != null && benMAPPollutantArray.ToList().Select(pp => pp.PollutantID).ToList() != CommonClass.lstPollutantAll.Select(ppp => ppp.PollutantID).ToList()))
                        {
                            currentNode.Tag = CommonClass.LstPollutant;

                            for (int i = nodesCount - 1; i > -1; i--)
                            {
                                TreeNode node = currentNode.Parent.Nodes[i];
                                if (currentNode.Parent.Nodes[i].Name == "datasource") { currentNode.Parent.Nodes.RemoveAt(i); }
                            }
                            CommonClass.LstBaseControlGroup = null;
                            GC.Collect();
                            CommonClass.LstBaseControlGroup = new List<BaseControlGroup>(CommonClass.LstPollutant.Count);
                            // for (int i = 0; i < CommonClass.LstPollutant.Count; i++)
                            for (int i = CommonClass.LstPollutant.Count - 1; i > -1; i--)
                            {
                                p = CommonClass.LstPollutant[i];
                                bcg = new BaseControlGroup() { GridType = CommonClass.GBenMAPGrid, Pollutant = p };
                                CommonClass.LstBaseControlGroup.Add(bcg);
                                AddDataSourceNode(bcg, currentNode.Parent);
                            }
                            CommonClass.BaseControlCRSelectFunction = null;
                            CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
                            CommonClass.lstIncidencePoolingAndAggregation = null;
                            //CommonClass.IncidencePoolingAndAggregationAdvance = null;
                            CommonClass.IncidencePoolingResult = null;
                            CommonClass.ValuationMethodPoolingAndAggregation = null;
                            //----------变黄-----------
                            ClearMapTableChart();
                            initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);

                            foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes)
                            {
                                initNodeImage(tn);
                            }
                            foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 2].Nodes)
                            {
                                initNodeImage(tn);
                            }
                            CommonClass.BenMAPPopulation = null;
                            CommonClass.IncidencePoolingAndAggregationAdvance = null;

                            ////Todo:设置灰色 陈志润
                            //tabCRFunctionResultGISShow.BackColor = Color.Gray;
                            //tabAPVResultGISShow.BackColor = Color.Gray;
                            //tabQLAYResultShow.BackColor = Color.Gray;
                            //tabAuditTrialReport.BackColor = Color.Gray;
                            olvCRFunctionResult.SetObjects(null);
                            olvIncidence.SetObjects(null);
                            tlvAPVResult.SetObjects(null);
                            //tlvQALYResult.SetObjects(null);

                            cbPoolingWindowIncidence.Items.Clear();
                            cbPoolingWindowAPV.Items.Clear();
                            //cbPoolingWindowQALY.Items.Clear();
                            ClearMapTableChart();
                        }
                        currentNode.Parent.ExpandAll();

                        break;
                    case "datasource":
                        _currentNode = "datasource";
                        TreeNode pNode = currentNode.Parent;
                        BaseControlGroup bcgLoadAQG = new BaseControlGroup();
                        //if (CommonClass.GBenMAPGrid == null) { MessageBox.Show("Grid Type not set up!"); return; }
                        //if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0) { MessageBox.Show("Please select pollutants!"); return; }
                        //if (CommonClass.LstBaseControlGroup == null || CommonClass.LstBaseControlGroup.Count == 0) { MessageBox.Show("Please select pollutants!"); return; }
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
                        if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0 && bcgLoadAQG.Pollutant!=null)
                        {
                            lock (CommonClass.LstAsynchronizationStates)
                            {
                                if (CommonClass.LstAsynchronizationStates.Contains(string.Format("{0}{1}", bcgLoadAQG.Pollutant.PollutantName.ToLower(), "baseline"))
                                    || CommonClass.LstAsynchronizationStates.Contains(string.Format("{0}{1}", bcgLoadAQG.Pollutant.PollutantName.ToLower(), "control")))
                                {
                                    msg = " BenMAP is still creating the air quality surface map.";
                                    MessageBox.Show(msg);
                                    return  ;
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
                                    // currentNode.Parent.Nodes[i].Text = "Data Source";
                                    // currentNode.Parent.Nodes[i].Nodes[0].Tag = null;
                                    if (currentNodeTag != null && currentNodeTag.ToString() != "" && int.Parse(currentNodeTag) != int.Parse(currentNode.Parent.Nodes[i].Tag.ToString()))
                                    {
                                        currentNode.Parent.Nodes[i].Nodes[0].Nodes.Clear();
                                        initNodeImage(currentNode.Parent.Nodes[i].Nodes[0]);
                                        //  currentNode.Parent.Nodes[i].Nodes[1].Tag = null;
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
                            //tlvQALYResult.SetObjects(null);

                            cbPoolingWindowIncidence.Items.Clear();
                            cbPoolingWindowAPV.Items.Clear();
                            //cbPoolingWindowQALY.Items.Clear();
                            //----------变黄-----------
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
                        bcgLoadAQG = openAQG.bcgOpenAQG;
                        int index = currentNode.Index;
                        BrushBaseControl(ref pNode, bcgLoadAQG, index);

                        break;
                    case "baseline":
                        _currentNode = "baseline";
                        currStat = "baseline";

                        //nodeTag = currentNode.Parent.Tag.ToString();
                        //BaseControlGroup baseline = currentNode.Tag as BaseControlGroup;
                        //    foreach (BaseControlGroup bc in CommonClass.LstBaseControlGroup)
                        //    {
                        //        if (bc.Pollutant.PollutantID == baseline.Pollutant.PollutantID)
                        //        { baseline = bc; }
                        //    }
                        //    currentNode.Tag = baseline;
                        BaseControlOP(currStat, ref currentNode);
                        //AddChildNodes(ref currentNode, currStat, currStat, bml);
                        //改变该节点的图标为“ready”
                        //changeNodeImage(currentNode);
                        //currentNode.ExpandAll();
                        //ChangeNodeStatus();
                        break;
                    case "basedata":
                        _currentNode = "basedata";
                        str = string.Format("{0}baseline", (currentNode.Tag as BenMAPLine).Pollutant.PollutantName);
                        if (CommonClass.LstAsynchronizationStates != null &&
                            CommonClass.LstAsynchronizationStates.Contains(str.ToLower()))
                        {
                            MessageBox.Show(string.Format("BenMAP is still creating the air quality surface map.", (currentNode.Tag as BenMAPLine).Pollutant.PollutantName), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        //str = string.Format("{0}control", (currentNode.Tag as BenMAPLine).Pollutant.PollutantName);
                        //if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Contains(str.ToLower()))
                        //{
                        //    MessageBox.Show(string.Format("{0} Control is asynchronously generating a file ", (currentNode.Tag as BenMAPLine).Pollutant.PollutantID), "Tip", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //    return;
                        //}
                        // 画base图层
                        // string tip = "Loading Layer.\r\nPlease wait!";
                        //开线程Run And 赋值 CommonClass.BaseControlCRFunction
                        WaitShow("Drawing layer...");
                        try
                        {
                            tabCtlMain.SelectedIndex = 0;
                            mainMap.Layers.Clear();
                            tsbChangeProjection.Text = "change projection to Albers";
                            BenMAPLine b = currentNode.Tag as BenMAPLine;
                            foreach (BaseControlGroup bc in CommonClass.LstBaseControlGroup)
                            {
                                if (bc.Pollutant.PollutantID == b.Pollutant.PollutantID)
                                { b = bc.Base; }
                            }
                            currentNode.Tag = b;
                            addBenMAPLineToMainMap(b, "B");
                            //mainMap.ProjectionModeReproject = ActionMode.Never;
                            //mainMap.ProjectionModeDefine = ActionMode.Never;
                            //if (currentNode.Tag is BenMAPLine)
                            //{
                            //    if (File.Exists((currentNode.Tag as BenMAPLine).ShapeFile))
                            //    {
                            //        mainMap.Layers.Add((currentNode.Tag as BenMAPLine).ShapeFile);
                            //    }
                            //}
                            addRegionLayerToMainMap();
                            LayerObject = currentNode.Tag as BenMAPLine;
                            InitTableResult(currentNode.Tag as BenMAPLine);
                        }
                        catch
                        {
                        }
                        WaitClose();
                        break;
                    case "delta":
                        _currentNode = "delta";
                        //WaitShow("Loading Delta Layer");
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
                            MessageBox.Show(string.Format("BenMAP is still creating the air quality surface map. ", bcgDelta.Pollutant.PollutantName), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        str = string.Format("{0}control", bcgDelta.Pollutant.PollutantName);
                        if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Contains(str.ToLower()))
                        {
                            MessageBox.Show(string.Format("BenMAP is still creating the air quality surface map. ", bcgDelta.Pollutant.PollutantName), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        WaitShow("Drawing layer...");
                        tsbChangeProjection.Text = "change projection to Albers";
                        if (bcgDelta.DeltaQ == null)
                        {
                            bcgDelta.DeltaQ = new BenMAPLine();
                            bcgDelta.DeltaQ.Pollutant = bcgDelta.Base.Pollutant;
                            bcgDelta.DeltaQ.GridType = bcgDelta.Base.GridType;
                            bcgDelta.DeltaQ.ModelResultAttributes = new List<ModelResultAttribute>();
                            //bcgDelta.DeltaQ.ResultCopy = new List<double[]>();
                            //foreach (double[] ld in bcgDelta.Base.ResultCopy)
                            //{
                            //    IEnumerable<double[]> ldIenumerable = bcgDelta.Control.ResultCopy.Where(a => a[0] == ld[0] && a[1] == ld[1]);
                            //    foreach (double[] da in ldIenumerable)
                            //    {
                            //        bcgDelta.DeltaQ.ResultCopy.Add(ld);

                            //        for (int idelta = 2; idelta < ld.Length; idelta++)
                            //        {
                            //            bcgDelta.DeltaQ.ResultCopy[bcgDelta.DeltaQ.ResultCopy.Count - 1][idelta] = ld[idelta] - da[idelta];

                            //        }
                            //    }

                            //}
                            //DataSourceCommonClass.getModelValuesFromResultCopy(ref bcgDelta.DeltaQ);
                            //return;
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
                                        foreach (KeyValuePair<string, float> k in mra.Values)
                                        {
                                            if (dicControl[mra.Col + "," + mra.Row].ContainsKey(k.Key))
                                                bcgDelta.DeltaQ.ModelResultAttributes[bcgDelta.DeltaQ.ModelResultAttributes.Count - 1].Values.Add(k.Key, k.Value - (dicControl[mra.Col + "," + mra.Row][k.Key]));
                                            else
                                                bcgDelta.DeltaQ.ModelResultAttributes[bcgDelta.DeltaQ.ModelResultAttributes.Count - 1].Values.Add(k.Key, Convert.ToSingle(0.0));
                                        }
                                    }
                                    //IEnumerable<ModelResultAttribute> mrcIEnumerable = bcgDelta.Control.ModelResultAttributes.Where(a => a.Col == mra.Col && a.Row == mra.Row);
                                    //foreach (ModelResultAttribute mrc in mrcIEnumerable)
                                    //{
                                    //    bcgDelta.DeltaQ.ModelResultAttributes.Add(new ModelResultAttribute()
                                    //    {
                                    //        Col = mra.Col,
                                    //        Row = mra.Row
                                    //    });
                                    //    bcgDelta.DeltaQ.ModelResultAttributes[bcgDelta.DeltaQ.ModelResultAttributes.Count - 1].Values = new Dictionary<string, float>();
                                    //    foreach (KeyValuePair<string, float> k in mra.Values)
                                    //    {
                                    //        if (mrc.Values.ContainsKey(k.Key))
                                    //            bcgDelta.DeltaQ.ModelResultAttributes[bcgDelta.DeltaQ.ModelResultAttributes.Count - 1].Values.Add(k.Key, k.Value - mrc.Values[k.Key]);
                                    //        else
                                    //            bcgDelta.DeltaQ.ModelResultAttributes[bcgDelta.DeltaQ.ModelResultAttributes.Count - 1].Values.Add(k.Key, Convert.ToSingle( 0.0));
                                    //    }
                                    //}
                                }
                                catch
                                { }
                            }
                        }
                        //-----------do-------------

                        try
                        {
                            tabCtlMain.SelectedIndex = 0;
                            mainMap.Layers.Clear();
                            addBenMAPLineToMainMap(bcgDelta.DeltaQ, "D");
                            //mainMap.ProjectionModeReproject = ActionMode.Never;
                            //mainMap.ProjectionModeDefine = ActionMode.Never;
                            //if (currentNode.Tag is BenMAPLine)
                            //{
                            //    if (File.Exists((currentNode.Tag as BenMAPLine).ShapeFile))
                            //    {
                            //        mainMap.Layers.Add((currentNode.Tag as BenMAPLine).ShapeFile);
                            //    }
                            //}
                            addRegionLayerToMainMap();
                            LayerObject = bcgDelta.DeltaQ;
                            InitTableResult(bcgDelta.DeltaQ);
                        }
                        catch
                        {
                        }
                        WaitClose();
                        break;
                    //changeNodeImage(currentNode);
                    case "control":
                        _currentNode = "control";
                        currStat = "control";
                        //nodeTag = currentNode.Parent.Tag.ToString();
                        BaseControlOP(currStat, ref currentNode);
                        //changeNodeImage(currentNode);
                        //currentNode.ExpandAll();
                        break;
                    case "controldata":
                        _currentNode = "controldata";
                        //str = string.Format("{0}baseline", (currentNode.Tag as BenMAPLine).Pollutant.PollutantID);
                        //if (CommonClass.LstAsynchronizationStates != null &&
                        //    CommonClass.LstAsynchronizationStates.Contains(str.ToLower()))
                        //{
                        //    MessageBox.Show(string.Format("{0} Base Line is asynchronously generating a file ", (currentNode.Tag as BenMAPLine).Pollutant.PollutantID), "Tip", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //    return;
                        //}
                        str = string.Format("{0}control", (currentNode.Tag as BenMAPLine).Pollutant.PollutantName);
                        if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Contains(str.ToLower()))
                        {
                            MessageBox.Show(string.Format("BenMAP is still creating the air quality surface map. ", (currentNode.Tag as BenMAPLine).Pollutant.PollutantName), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        WaitShow("Drawing layer...");
                        try
                        {
                            tabCtlMain.SelectedIndex = 0;
                            mainMap.Layers.Clear();
                            //// Todo:陈志润20111128
                            //addBenMAPLineToMainMap(currentNode.Tag as BenMAPLine, "C");
                            BenMAPLine cc = currentNode.Tag as BenMAPLine;
                            foreach (BaseControlGroup bc in CommonClass.LstBaseControlGroup)
                            {
                                if (bc.Pollutant.PollutantID == cc.Pollutant.PollutantID)
                                { cc = bc.Control; }
                            }
                            currentNode.Tag = cc;
                            addBenMAPLineToMainMap(cc, "C");
                            //mainMap.ProjectionModeReproject = ActionMode.Never;
                            //mainMap.ProjectionModeDefine = ActionMode.Never;
                            //if (currentNode.Tag is BenMAPLine)
                            //{
                            //    if (File.Exists((currentNode.Tag as BenMAPLine).ShapeFile))
                            //    {
                            //        mainMap.Layers.Add((currentNode.Tag as BenMAPLine).ShapeFile);
                            //    }
                            //}
                            addRegionLayerToMainMap();
                            LayerObject = currentNode.Tag as BenMAPLine;
                            InitTableResult(currentNode.Tag as BenMAPLine);
                        }
                        catch
                        {
                        }
                        WaitClose();
                        // 画control图层
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
                                CommonClass.BaseControlCRSelectFunctionCalculateValue = Configuration.ConfigurationCommonClass.LoadCFGRFile((frm as OpenExistingConfiguration).strCRPath,ref err);
                                if (CommonClass.BaseControlCRSelectFunctionCalculateValue == null)
                                {
                                    System.Threading.Thread.Sleep(300);
                                    WaitClose();
                                    MessageBox.Show(err);
                                    return;
                                }

                                if (CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue == null)
                                {
                                    System.Threading.Thread.Sleep(300);//直接waitclose,有时候关不了，在这停留0.3s，似乎可以了
                                    WaitClose();
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
                                        //CommonClass.LstPollutant = CommonClass.BaseControlCRSelectFunction.BaseControlGroup.Select(pa => pa.Pollutant).ToList();
                                        //CommonClass.lst
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    Logger.LogError(ex);
                                }
                                
                                //Todo:设置灰色 陈志润
                                //tabCRFunctionResultGISShow.BackColor = Color.White;
                                olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
                                CommonClass.ValuationMethodPoolingAndAggregation = null;
                                CommonClass.lstIncidencePoolingAndAggregation = null;
                                //CommonClass.IncidencePoolingAndAggregationAdvance = null;
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
                                //tlvQALYResult.SetObjects(null);
                                cbPoolingWindowIncidence.Items.Clear();
                                cbPoolingWindowAPV.Items.Clear();
                                //cbPoolingWindowQALY.Items.Clear();
                                ClearMapTableChart();
                                //Todo:设置灰色 陈志润
                                //tabAPVResultGISShow.BackColor = Color.Gray;
                                //tabQLAYResultShow.BackColor = Color.Gray;
                                //tabAuditTrialReport.BackColor = Color.Gray;
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(ex);
                            }
                            WaitClose();
                        }
                        else
                        {
                            //--------------------更新所有的Health以及之前的所有节点-----------------------------
                            try
                            {
                                if (CommonClass.BaseControlCRSelectFunction != null)
                                {
                                    //Todo:设置灰色 陈志润
                                    //tabCRFunctionResultGISShow.BackColor = Color.Gray;
                                    //tabAPVResultGISShow.BackColor = Color.Gray;
                                    //tabQLAYResultShow.BackColor = Color.Gray;
                                    //tabAuditTrialReport.BackColor = Color.Gray;
                                    CommonClass.MainSetup = CommonClass.getBenMAPSetupFromID(CommonClass.BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID);
                                   
                                    showExistBaseControlCRSelectFunction(CommonClass.BaseControlCRSelectFunction, currentNode);
                                    olvIncidence.SetObjects(null);
                                    tlvAPVResult.SetObjects(null);
                                    //tlvQALYResult.SetObjects(null);
                                    cbPoolingWindowIncidence.Items.Clear();
                                    cbPoolingWindowAPV.Items.Clear();
                                    //cbPoolingWindowQALY.Items.Clear();
                                    ClearMapTableChart();
                                    CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
                                    CommonClass.lstIncidencePoolingAndAggregation = null;
                                    //CommonClass.IncidencePoolingAndAggregationAdvance = null;
                                    CommonClass.IncidencePoolingResult = null;
                                    CommonClass.ValuationMethodPoolingAndAggregation = null;

                                    GC.Collect();

                                    ////Todo:设置灰色 陈志润
                                    //tabCRFunctionResultGISShow.BackColor = Color.Gray;
                                    //tabAPVResultGISShow.BackColor = Color.Gray;
                                    //tabQLAYResultShow.BackColor = Color.Gray;
                                    //tabAuditTrialReport.BackColor = Color.Gray;
                                    //tlvQALYResult.SetObjects(null);
                                    olvCRFunctionResult.SetObjects(null);
                                    //initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1]);
                                    foreach (TreeNode tn in trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes)
                                    {
                                        initNodeImage(tn);
                                    }
                                    //CommonClass.LstPollutant = CommonClass.BaseControlCRSelectFunction.BaseControlGroup.Select(pa => pa.Pollutant).ToList();
                                    //CommonClass.lst
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        //Todo:陈志润
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
                            //---变黄HealthImpact
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
                            //if ((frm as HealthImpactFunctions).isHIFChanged)
                            //{
                            //    errorNodeImage(currentNode);
                            //    errorNodeImage(currentNode.Nodes[1]);
                            //    errorNodeImage(currentNode.Nodes[2]);
                            //}
                            return;
                        }
                        ConfigurationResultsReport frmReport = new ConfigurationResultsReport();
                        frmReport.userAssignPercentile = false;
                        strHealthImpactPercentiles = null;

                        //---------------显示列表到CR Result GIS Show里面-------------------------
                        //Todo:设置灰色 陈志润
                        //tabCRFunctionResultGISShow.BackColor = Color.White;
                        //tabAPVResultGISShow.BackColor = Color.Gray;
                        //tabQLAYResultShow.BackColor = Color.Gray;
                        //tabAuditTrialReport.BackColor = Color.Gray;
                        // CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue
                        olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
                        changeNodeImage(currentNode);
                        //--------------------Modify by xiejp考虑变化较少的情况---------------------------------
                        //                       CommonClass.ValuationMethodPoolingAndAggregation = null;
                        //                       CommonClass.lstIncidencePoolingAndAggregation = null;
                        //                       CommonClass.IncidencePoolingAndAggregationAdvance = null;
                        //                       CommonClass.IncidencePoolingResult = null;
                        //initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
                        //                       initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 2]);
                        //                       initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
                        //Todo:陈志润 CRResultChangeVPV为当HealthImpactFunction有删除的时候所用
                        if (CommonClass.ValuationMethodPoolingAndAggregation != null)
                        {
                            //CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue;
                            CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue;
                            CommonClass.ValuationMethodPoolingAndAggregation.CFGRPath = "";
                            //----------------do pooling-------
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
                                        //alsc.CRID = -1;
                                        alsc.CRSelectFunctionCalculateValue = null;
                                    }

                                }
                            }
                            ////----------------do pooling-------
                            //foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                            //{
                            //    //foreach (AllSelectCRFunction alsc in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.PoolingMethod != "" && p.PoolingMethod != "None").ToList())
                            //    //{
                            //    //    if (alsc.PoolingMethod != "" && alsc.PoolingMethod != "None")
                            //    //    {
                            //    //        List<AllSelectCRFunction> lstSec = new List<AllSelectCRFunction>();
                            //    //        APVX.APVCommonClass.getAllChildCRNotNoneCalulate(alsc, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstSec);
                            //    //        //List<AllSelectCRFunction> lstSecResult = new List<AllSelectCRFunction>();
                            //    //        //foreach (AllSelectCRFunction alcr in lstSec)
                            //    //        //{
                            //    //        //    if (lstAllSelectCRFunctionNone.Where(p => p.ID == alcr.ID).Count() > 0)
                            //    //        //    {
                            //    //        //        lstSecResult.Add(lstAllSelectCRFunctionNone.Where(p => p.ID == alcr.ID).First());
                            //    //        //    }
                            //    //        //}
                            //    //        alsc.CRSelectFunctionCalculateValue = APVX.APVCommonClass.getPoolingMethodCRSelectFunctionCalculateValue(lstSec.Where(p => p.CRSelectFunctionCalculateValue != null).Select(a => a.CRSelectFunctionCalculateValue).ToList(), APVX.APVCommonClass.getPoolingMethodTypeEnumFromString(alsc.PoolingMethod), lstSec.Where(p => p.CRSelectFunctionCalculateValue != null).Select(a => a.Weight).ToList());
                            //    //    }
                            //    //}
                            //    //-----------------------首先得到Pooling--------------------------------------

                            //    List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
                            //    if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().PoolingMethod == "None")
                            //    {
                            //        APVX.APVCommonClass.getAllChildCRNotNone(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First(), vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);

                            //    }
                            //    lstCR.Insert(0, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First());
                            //    if (lstCR.Count == 1 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID < 9999 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID > 0)//.PoolingMethod == "")
                            //    { }
                            //    else
                            //    {
                            //        APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(a => a.NodeType != 100).Max(a => a.NodeType), vb.IncidencePoolingAndAggregation.lstColumns);
                            //    }
                            //    //---------------------------------------------------
                            //}
                            //---------如果最后一个为绿色则变成红色
                            if (trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 2].ImageKey == _readyImageKey)
                            {
                                errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 2]);
                            }
                            if (trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1].ImageKey == _readyImageKey)
                            {
                                errorNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
                            }

                        }
                        //this.CRResultChangeVPV();
                        //----------------------------------------------------------------------------------------
                        //Todo:设置灰色 陈志润
                        //tabCRFunctionResultGISShow.BackColor = Color.Gray;
                        //tabAPVResultGISShow.BackColor = Color.Gray;
                        //tabQLAYResultShow.BackColor = Color.Gray;
                        //tabAuditTrialReport.BackColor = Color.Gray;
                        //tlvQALYResult.SetObjects(null);
                        olvIncidence.SetObjects(null);
                        tlvAPVResult.SetObjects(null);

                        cbPoolingWindowIncidence.Items.Clear();
                        cbPoolingWindowAPV.Items.Clear();
                        //cbPoolingWindowQALY.Items.Clear();
                        ClearMapTableChart();
                        changeNodeImage(currentNode);
                        //initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1]);
                        //Todo:陈志润
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
                                CommonClass.BaseControlCRSelectFunctionCalculateValue = Configuration.ConfigurationCommonClass.LoadCFGRFile((frm as OpenExistingAPVConfiguration).strCRPath,ref err);
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
                                        //CommonClass.LstPollutant = CommonClass.BaseControlCRSelectFunction.BaseControlGroup.Select(pa => pa.Pollutant).ToList();
                                        //CommonClass.lst
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    Logger.LogError(ex);
                                }
                                
                                //Todo:设置灰色 陈志润
                                //tabCRFunctionResultGISShow.BackColor = Color.White;
                                //tabAPVResultGISShow.BackColor = Color.Gray;
                                //tabQLAYResultShow.BackColor = Color.Gray;
                                //tabAuditTrialReport.BackColor = Color.Gray;
                                olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
                                CommonClass.ValuationMethodPoolingAndAggregation = null;
                                CommonClass.lstIncidencePoolingAndAggregation = null;
                                //CommonClass.IncidencePoolingAndAggregationAdvance = null;
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
                                //tlvQALYResult.SetObjects(null);

                                cbPoolingWindowIncidence.Items.Clear();
                                cbPoolingWindowAPV.Items.Clear();
                                //cbPoolingWindowQALY.Items.Clear();
                                ClearMapTableChart();
                                //Todo:设置灰色 陈志润
                                //tabCRFunctionResultGISShow.BackColor = Color.Gray;
                                //tabAPVResultGISShow.BackColor = Color.Gray;
                                //tabQLAYResultShow.BackColor = Color.Gray;
                                //tabAuditTrialReport.BackColor = Color.Gray;
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
                            CommonClass.BaseControlCRSelectFunction = null;
                            GC.Collect();
                            CommonClass.ClearAllObject();
                            string err = "";
                            CommonClass.ValuationMethodPoolingAndAggregation = APVX.APVCommonClass.loadAPVRFile((frm as OpenExistingAPVConfiguration).strAPVPath,ref err);
                            if (CommonClass.ValuationMethodPoolingAndAggregation == null)
                            {
                                WaitClose();
                                MessageBox.Show(err);
                                return;
                            }
                            //------------兼容原来的版本如果EndpointGroup为空，则置第一个
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

                                    //CommonClass.LstPollutant = CommonClass.BaseControlCRSelectFunction.BaseControlGroup.Select(pa => pa.Pollutant).ToList();
                                    //CommonClass.lst
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                            if (((frm as OpenExistingAPVConfiguration).strAPVPath.Substring(((frm as OpenExistingAPVConfiguration).strAPVPath.Length - 5), 5)) == "apvrx")
                            {
                                olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);

                                //Todo:设置灰色 陈志润
                                //tabCRFunctionResultGISShow.BackColor = Color.White;
                                //tabAPVResultGISShow.BackColor = Color.Gray;
                                //tabQLAYResultShow.BackColor = Color.Gray;
                                //tabAuditTrialReport.BackColor = Color.Gray;
                            }
                            else
                            {
                                olvCRFunctionResult.SetObjects(null);//CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);

                                //Todo:设置灰色 陈志润
                                //tabCRFunctionResultGISShow.BackColor = Color.Gray;
                                //tabAPVResultGISShow.BackColor = Color.Gray;
                                //tabQLAYResultShow.BackColor = Color.Gray;
                                //tabAuditTrialReport.BackColor = Color.Gray;
                            }
                            //CommonClass.ValuationMethodPoolingAndAggregation = null;
                            //CommonClass.IncidencePoolingAndAggregation = null;
                            //CommonClass.IncidencePoolingAndAggregationAdvance = null;
                            //CommonClass.IncidencePoolingResult = null;

                            //tlvAPVResult.SetObjects(null);
                            CommonClass.lstIncidencePoolingAndAggregation = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Select(a => a.IncidencePoolingAndAggregation).ToList();
                            CommonClass.IncidencePoolingAndAggregationAdvance = CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance;
                            cbPoolingWindowAPV.Items.Clear();

                            //Todo:设置灰色 陈志润
                            //tabCRFunctionResultGISShow.BackColor = Color.Gray;
                            //tabAPVResultGISShow.BackColor = Color.Gray;
                            //tabQLAYResultShow.BackColor = Color.Gray;
                            //tabAuditTrialReport.BackColor = Color.Gray;
                            olvIncidence.SetObjects(null);
                            tlvAPVResult.SetObjects(null);
                            //tlvQALYResult.SetObjects(null);

                            cbPoolingWindowIncidence.Items.Clear();
                            cbPoolingWindowAPV.Items.Clear();
                            //cbPoolingWindowQALY.Items.Clear();
                            ClearMapTableChart();
                            if ((frm as OpenExistingAPVConfiguration).strAPVPath.Substring((frm as OpenExistingAPVConfiguration).strAPVPath.Count() - 5, 5).ToLower() == "apvrx")
                            {
                                //-------------重新计算----
                                //APVX.APVCommonClass.CalculateQALYMethodPoolingAndAggregation(ref CommonClass.ValuationMethodPoolingAndAggregation);
                                //APVX.APVCommonClass.CalculateValuationMethodPoolingAndAggregation(ref CommonClass.ValuationMethodPoolingAndAggregation);
                                //---------------重新计算Aggregation及Pooling---------------
                                if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null
                                   && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID && (CommonClass.lstCRResultAggregation == null || CommonClass.lstCRResultAggregation.Count == 0))
                                {
                                    //--------------首先Aggregation--------------
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

                                    //-----------------------首先得到Pooling--------------------------------------
                                    if (bHavePooling == false)
                                    {
                                        List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
                                        if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().PoolingMethod == "None")
                                        {
                                            APVX.APVCommonClass.getAllChildCRNotNoneForPooling(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First(), vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);

                                        }
                                        lstCR.Insert(0, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First());
                                        if (lstCR.Count == 1 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID < 9999 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID > 0)//.PoolingMethod == "")
                                        { }
                                        else
                                        {
                                            APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(true,ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(pa => pa.NodeType != 100).Max(pa => pa.NodeType), vb.IncidencePoolingAndAggregation.lstColumns);
                                        }
                                    }
                                }
                                foreach (ValuationMethodPoolingAndAggregationBase vbAPVFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                                {
                                    cbPoolingWindowAPV.Items.Add(vbAPVFrom.IncidencePoolingAndAggregation.PoolingName);
                                }
                                cbPoolingWindowAPV.SelectedIndex = 0;
                                ////---------------------显示Pooled Incidence-------------------------
                                cbPoolingWindowIncidence.Items.Clear();
                                foreach (ValuationMethodPoolingAndAggregationBase vbAPVFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                                {
                                    this.cbPoolingWindowIncidence.Items.Add(vbAPVFrom.IncidencePoolingAndAggregation.PoolingName);
                                }
                                cbPoolingWindowIncidence.SelectedIndex = 0;
                                ////------------------------------显示QALY--------------------------------------------------------------


                                //if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(a => a.lstAllSelectQALYMethodAndValue != null).Count() > 0 ||
                                //       CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(a => a.lstAllSelectQALYMethodAndValueAggregation != null).Count() > 0)
                                //{
                                //    cbPoolingWindowQALY.Items.Clear();
                                //    if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(a => a.lstAllSelectQALYMethodAndValue != null).Count() > 0)
                                //    {
                                //        foreach (ValuationMethodPoolingAndAggregationBase vbQALYFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(a => a.lstAllSelectQALYMethodAndValue != null))
                                //        {
                                //            cbPoolingWindowQALY.Items.Add(vbQALYFrom.IncidencePoolingAndAggregation.PoolingName);
                                //        }
                                //    }
                                //    else
                                //    {
                                //        foreach (ValuationMethodPoolingAndAggregationBase vbQALYFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(a => a.lstAllSelectQALYMethodAndValueAggregation != null))
                                //        {
                                //            cbPoolingWindowQALY.Items.Add(vbQALYFrom.IncidencePoolingAndAggregation.PoolingName);
                                //        }
                                //    }
                                //    cbPoolingWindowQALY.SelectedIndex = 0;
                                //}
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
                            //CommonClass.IncidencePoolingResult = CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingResult;
                            //List<AllSelectValuationMethod> lstRoot = new List<AllSelectValuationMethod>();
                            //lstRoot.Add(CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.First().LstAllSelectValuationMethod.First());
                            ////---------------显示列表到APVX Result GIS Show里面-------------------------

                            //tlvAPVResult.Roots = lstRoot;
                            //this.tlvAPVResult.CanExpandGetter = delegate(object x)
                            //{
                            //    AllSelectValuationMethod dir = (AllSelectValuationMethod)x;
                            //    return (dir.NodeType != 3);
                            //};
                            //this.tlvAPVResult.ChildrenGetter = delegate(object x)
                            //{
                            //    AllSelectValuationMethod dir = (AllSelectValuationMethod)x;

                            //    try
                            //    {
                            //        return getChildFromAllSelectValuationMethod(dir);

                            //    }
                            //    catch (UnauthorizedAccessException ex)
                            //    {
                            //        MessageBox.Show(this, ex.Message, "ObjectListViewDemo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //        return new List<AllSelectValuationMethod>();
                            //        //return new ArrayList();
                            //    }
                            //};
                            //tlvAPVResult.ExpandAll();
                            ////------------------------------显示QALY--------------------------------------------------------------
                            //if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.First().lstAllSelectQALYMethod != null && CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.First().lstAllSelectQALYMethodAndValue != null)
                            //{
                            //    List<AllSelectQALYMethod> lstQALYRoot = new List<AllSelectQALYMethod>();
                            //    lstQALYRoot.Add(CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.First().lstAllSelectQALYMethod.First());
                            //    tlvQALYResult.Roots = lstQALYRoot;
                            //    this.tlvQALYResult.CanExpandGetter = delegate(object x)
                            //    {
                            //        AllSelectQALYMethod dir = (AllSelectQALYMethod)x;
                            //        return (dir.NodeType != 3);
                            //    };
                            //    this.tlvQALYResult.ChildrenGetter = delegate(object x)
                            //    {
                            //        AllSelectQALYMethod dir = (AllSelectQALYMethod)x;
                            //        try
                            //        {
                            //            return getChildFromAllSelectQALYMethod(dir);

                            //        }
                            //        catch (UnauthorizedAccessException ex)
                            //        {
                            //            MessageBox.Show(this, ex.Message, "ObjectListViewDemo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //            return new List<AllSelectValuationMethod>();
                            //            //return new ArrayList();
                            //        }
                            //    };
                            //    tlvQALYResult.ExpandAll();
                            //}

                            //Todo:陈志润
                            SetTabControl(tabCtlReport);
                            //this.trvSetting.ExpandAll();
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
                                //CommonClass.l = new List<CRSelectFunctionCalculateValue>();
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
                            ////---------------------显示Pooled Incidence-------------------------
                            cbPoolingWindowIncidence.Items.Clear();
                            foreach (ValuationMethodPoolingAndAggregationBase vbAPVFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                            {
                                this.cbPoolingWindowIncidence.Items.Add(vbAPVFrom.IncidencePoolingAndAggregation.PoolingName);
                            }
                            cbPoolingWindowIncidence.SelectedIndex = 0;
                            //------------------------------显示QALY--------------------------------------------------------------

                            //cbPoolingWindowQALY.Items.Clear();
                            //if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(a => a.lstAllSelectQALYMethodAndValue != null).Count() > 0 ||
                            //       CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(a => a.lstAllSelectQALYMethodAndValueAggregation != null).Count() > 0)
                            //{
                            //    cbPoolingWindowQALY.Items.Clear();
                            //    if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(a => a.lstAllSelectQALYMethodAndValue != null).Count() > 0)
                            //    {
                            //        foreach (ValuationMethodPoolingAndAggregationBase vbQALYFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(a => a.lstAllSelectQALYMethodAndValue != null))
                            //        {
                            //            cbPoolingWindowQALY.Items.Add(vbQALYFrom.IncidencePoolingAndAggregation.PoolingName);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        foreach (ValuationMethodPoolingAndAggregationBase vbQALYFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(a => a.lstAllSelectQALYMethodAndValueAggregation != null))
                            //        {
                            //            cbPoolingWindowQALY.Items.Add(vbQALYFrom.IncidencePoolingAndAggregation.PoolingName);
                            //        }
                            //    }
                            //    cbPoolingWindowQALY.SelectedIndex = 0;
                            //}
                            //           List<AllSelectValuationMethod> lstRoot = new List<AllSelectValuationMethod>();
                            //           lstRoot.Add(CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.First().LstAllSelectValuationMethod.First());
                            //           //---------------显示列表到APVX Result GIS Show里面-------------------------

                            //           tlvAPVResult.Roots = lstRoot;
                            //           this.tlvAPVResult.CanExpandGetter = delegate(object x)
                            //{
                            //    AllSelectValuationMethod dir = (AllSelectValuationMethod)x;
                            //    return (dir.NodeType != 3);
                            //};
                            //           this.tlvAPVResult.ChildrenGetter = delegate(object x)
                            //{
                            //    AllSelectValuationMethod dir = (AllSelectValuationMethod)x;

                            //    try
                            //    {
                            //        return getChildFromAllSelectValuationMethod(dir);

                            //    }
                            //    catch (UnauthorizedAccessException ex)
                            //    {
                            //        MessageBox.Show(this, ex.Message, "ObjectListViewDemo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //        return new List<AllSelectValuationMethod>();
                            //        //return new ArrayList();
                            //    }
                            //};

                            //           tlvAPVResult.ExpandAll();
                            //           //------------------------------显示QALY--------------------------------------------------------------
                            //           if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.First().lstAllSelectQALYMethod != null && CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.First().lstAllSelectQALYMethodAndValue != null)
                            //           {
                            //               List<AllSelectQALYMethod> lstQALYRoot = new List<AllSelectQALYMethod>();
                            //               lstQALYRoot.Add(CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.First().lstAllSelectQALYMethod.First());
                            //               tlvQALYResult.Roots = lstQALYRoot;
                            //               this.tlvQALYResult.CanExpandGetter = delegate(object x)
                            //               {
                            //                   AllSelectQALYMethod dir = (AllSelectQALYMethod)x;
                            //                   return (dir.NodeType != 3);
                            //               };
                            //               this.tlvQALYResult.ChildrenGetter = delegate(object x)
                            //               {
                            //                   AllSelectQALYMethod dir = (AllSelectQALYMethod)x;
                            //                   try
                            //                   {
                            //                       return getChildFromAllSelectQALYMethod(dir);

                            //                   }
                            //                   catch (UnauthorizedAccessException ex)
                            //                   {
                            //                       MessageBox.Show(this, ex.Message, "ObjectListViewDemo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //                       return new List<AllSelectValuationMethod>();
                            //                       //return new ArrayList();
                            //                   }
                            //               };
                            //               tlvQALYResult.ExpandAll();
                            //           }
                            //----------------------------------------------------------------------------------------------------
                            //----------------------显示CRResult--------------------------------
                            //Todo:设置灰色 陈志润
                            //tabCRFunctionResultGISShow.BackColor = Color.White;
                            //tabAPVResultGISShow.BackColor = Color.Gray;
                            //tabQLAYResultShow.BackColor = Color.Gray;
                            //tabAuditTrialReport.BackColor = Color.Gray;
                            olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
                            changeNodeImage(currentNode.Parent.Nodes[currentNode.Parent.Nodes.Count - 2]);
                            changeNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
                            changeNodeImage(currentNode);
                            changeNodeImage(currentNode);
                            //changeNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1]);
                            changeNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
                        }
                        //Todo:陈志润
                        SetTabControl(tabCtlReport);
                        break;
                    case "valuationmethod":
                        _currentNode = "valuationmethod";
                        if (CommonClass.ValuationMethodPoolingAndAggregation == null)
                            return;
                        if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.First().LstAllSelectValuationMethod == null)
                            return;
                        //if (CommonClass.ValuationMethodPoolingAndAggregation.lstAllSelectQALYMethod.Count == 0)
                        //    return;
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
                        ////---------------------显示Pooled Incidence-------------------------
                        cbPoolingWindowIncidence.Items.Clear();
                        foreach (ValuationMethodPoolingAndAggregationBase vbAPVFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                        {
                            this.cbPoolingWindowIncidence.Items.Add(vbAPVFrom.IncidencePoolingAndAggregation.PoolingName);
                        }
                        cbPoolingWindowIncidence.SelectedIndex = 0;
                        //------------------------------显示QALY--------------------------------------------------------------
                        //cbPoolingWindowQALY.Items.Clear();
                        //if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(a => a.lstAllSelectQALYMethodAndValue != null).Count() > 0 ||
                        //       CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(a => a.lstAllSelectQALYMethodAndValueAggregation != null).Count() > 0)
                        //{

                        //    if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(a => a.lstAllSelectQALYMethodAndValue != null).Count() > 0)
                        //    {
                        //        foreach (ValuationMethodPoolingAndAggregationBase vbQALYFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(a => a.lstAllSelectQALYMethodAndValue != null))
                        //        {
                        //            cbPoolingWindowQALY.Items.Add(vbQALYFrom.IncidencePoolingAndAggregation.PoolingName);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        foreach (ValuationMethodPoolingAndAggregationBase vbQALYFrom in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(a => a.lstAllSelectQALYMethodAndValueAggregation != null))
                        //        {
                        //            cbPoolingWindowQALY.Items.Add(vbQALYFrom.IncidencePoolingAndAggregation.PoolingName);
                        //        }
                        //    }
                        //    cbPoolingWindowQALY.SelectedIndex = 0;
                        //}

                        //----------------------显示CRResult--------------------------------
                        //Todo:设置灰色 陈志润
                        //tabCRFunctionResultGISShow.BackColor = Color.White;
                        //tabAPVResultGISShow.BackColor = Color.Gray;
                        //tabQLAYResultShow.BackColor = Color.Gray;
                        //tabAuditTrialReport.BackColor = Color.Gray;
                        olvCRFunctionResult.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
                        changeNodeImage(currentNode.Parent.Nodes[currentNode.Parent.Nodes.Count - 2]);
                        changeNodeImage(trvSetting.Nodes[1].Nodes[trvSetting.Nodes[1].Nodes.Count - 1]);
                        changeNodeImage(currentNode);
                        //Todo:陈志润
                        SetTabControl(tabCtlReport);
                        break;
                    //default: break;
                }//swith
                //if (CommonClass.IncidencePoolingAndAggregation != null)
                //{
                //    btShowIncidencePooling.Tag = "Poolint Method Type:" + Enum.GetName(typeof(PoolingMethodTypeEnum), CommonClass.IncidencePoolingAndAggregation.PoolingMethodType);
                //}
                //else
                //{
                //    btShowIncidencePooling.Tag = "";
                //}
                //-------------重新设置Aggregation--------------------------
                if (iGridTypeOld != CommonClass.MainSetup.SetupID)
                {
                    ChangeAllAggregationCombox();
                }
                //---------end --------------------------------
                if (CommonClass.IncidencePoolingAndAggregationAdvance != null)
                {
                    changeNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
                    //----------------------Aggregation
                    FireBirdHelperBase fb = new ESILFireBirdHelper();
                    string commandText = "";
                    //if (CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null)
                    //{
                    //    commandText = string.Format("select -1 as  GridDefinitionID, '' as GridDefinitionName from GridDefinitions union select distinct GridDefinitionID,GridDefinitionName from GridDefinitions where GridDefinitionID<>7 and GridDefinitionID<>13 and columns*RRows<(select columns*RRows from GridDefinitions where GridDefinitionID={1} )  and setupid={0} ", CommonClass.MainSetup.SetupID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID);
                    //    System.Data.DataSet dsRegion = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    //    cbAPVAggregation.DataSource = dsRegion.Tables[0];
                    //    cbAPVAggregation.DisplayMember = "GridDefinitionName";

                    //}
                    //if (CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation != null)
                    //{
                    //    commandText = string.Format("select -1 as  GridDefinitionID, '' as GridDefinitionName from GridDefinitions union select distinct GridDefinitionID,GridDefinitionName from GridDefinitions where GridDefinitionID<>7 and GridDefinitionID<>13 and columns*RRows<(select columns*RRows from GridDefinitions where GridDefinitionID={1} )  and setupid={0} ", CommonClass.MainSetup.SetupID, CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID);
                    //    System.Data.DataSet dsQALY = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    //    cbQALYAggregation.DataSource = dsQALY.Tables[0];
                    //    cbQALYAggregation.DisplayMember = "GridDefinitionName";

                    //}

                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void CRResultChangeVPV()
        {
            try
            {
                if (CommonClass.lstIncidencePoolingAndAggregation == null) return;
                //------------update lstIncidencePoolingAndAggregation---------------
                List<string> lstCRID = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Select(p => p.CRSelectFunction.CRID + "," + p.CRSelectFunction.BenMAPHealthImpactFunction.ID).ToList();
                foreach (IncidencePoolingAndAggregation ipa in CommonClass.lstIncidencePoolingAndAggregation)
                {
                    List<AllSelectCRFunction> lstRemove = new List<AllSelectCRFunction>();
                    foreach (AllSelectCRFunction asc in ipa.lstAllSelectCRFuntion)
                    {
                        if (asc.NodeType != 100) continue;
                        if (!lstCRID.Contains(asc.CRSelectFunctionCalculateValue.CRSelectFunction.CRID + "," + asc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.ID))
                        {
                            //--则需要删除---
                            lstRemove.Add(asc);
                        }
                    }
                    foreach (AllSelectCRFunction ascremove in lstRemove)
                    {

                        ipa.lstAllSelectCRFuntion.Remove(ascremove);
                    }
                    //------------删掉nodetype=3的，如果该节点下面有则不能删掉---------------------

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
                        //remove----------------
                        lstVBRemove.Add(vb);
                    }
                    else
                    {
                        //---------------重新生成Pooling
                        //foreach (AllSelectCRFunction alsc in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion)
                        //{
                        //    //------------Pooling-------------
                        //    if (alsc.PoolingMethod != "" && alsc.PoolingMethod != "None")
                        //    {
                        //        List<AllSelectCRFunction> lstSec = new List<AllSelectCRFunction>();
                        //        APVX.APVCommonClass.getAllChildCRNotNoneCalulate(alsc, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstSec);
                        //        //List<AllSelectCRFunction> lstSecResult = new List<AllSelectCRFunction>();
                        //        //foreach (AllSelectCRFunction alcr in lstSec)
                        //        //{
                        //        //    if (lstAllSelectCRFunctionNone.Where(p => p.ID == alcr.ID).Count() > 0)
                        //        //    {
                        //        //        lstSecResult.Add(lstAllSelectCRFunctionNone.Where(p => p.ID == alcr.ID).First());
                        //        //    }
                        //        //}
                        //        alsc.CRSelectFunctionCalculateValue = APVX.APVCommonClass.getPoolingMethodCRSelectFunctionCalculateValue(lstSec.Where(p => p.CRSelectFunctionCalculateValue != null).Select(a => a.CRSelectFunctionCalculateValue).ToList(), APVX.APVCommonClass.getPoolingMethodTypeEnumFromString(alsc.PoolingMethod), lstSec.Where(p => p.CRSelectFunctionCalculateValue != null).Select(a => a.Weight).ToList());

                        //    }

                        //}
                        //-----------------------首先得到Pooling--------------------------------------

                        List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
                        if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().PoolingMethod == "None")
                        {
                            APVX.APVCommonClass.getAllChildCRNotNone(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First(), vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);

                        }
                        lstCR.Insert(0, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First());
                        if (lstCR.Count == 1 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID < 9999 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID > 0)//.PoolingMethod == "")
                        { }
                        else
                        {
                            APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(true,ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.NodeType != 100).Max(p => p.NodeType), vb.IncidencePoolingAndAggregation.lstColumns);
                        }
                        //---------------------------------------------------
                        //-----------------如果只是加载APVX则要重新匹配--------------
                        //清理ValuationMethod,QALYMethod
                        //------如果id不在lstAllSelectCRFunction中--则删除---
                        //------如果nodetype==5的父节点没有，则删除
                        List<AllSelectValuationMethod> lstASVMRemove = new List<AllSelectValuationMethod>();
                        List<int> lstAVM = new List<int>();
                        if (vb.LstAllSelectValuationMethod != null && vb.LstAllSelectValuationMethod.Count > 0)
                        {
                            lstAVM = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.CRSelectFunctionCalculateValue != null).Select(p => p.CRID).ToList();
                            List<AllSelectValuationMethod> lstVTemp = vb.LstAllSelectValuationMethod.Where(p => vb.LstAllSelectValuationMethod.Where(a => a.PID == p.ID).Where(c => c.NodeType != 2000).Count() == 0 && p.NodeType != 2000).ToList();
                            foreach (AllSelectValuationMethod asvm in lstVTemp)// vb.LstAllSelectValuationMethod.Where(p => vb.LstAllSelectValuationMethod.Where(a=>a.PID==p.ID).Where(c=>c.NodeType!=2000).Count()==0))
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
                        //------如果id不在lstAllSelectCRFunction中--则删除---QALY
                        //------如果nodetype==5的父节点没有，则删除
                        List<AllSelectQALYMethod> lstQALYRemove = new List<AllSelectQALYMethod>();
                        if (vb.lstAllSelectQALYMethod != null && vb.lstAllSelectQALYMethod.Count > 0)
                        {
                            lstAVM = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.CRSelectFunctionCalculateValue != null).Select(p => p.CRID).ToList();
                            List<AllSelectQALYMethod> lstQTemp = vb.lstAllSelectQALYMethod.Where(p => vb.lstAllSelectQALYMethod.Where(a => a.PID == p.ID).Where(c => c.NodeType != 3000).Count() == 0 && p.NodeType != 3000).ToList();
                            foreach (AllSelectQALYMethod QALY in lstQTemp)//vb.lstAllSelectQALYMethod.Where(p => vb.lstAllSelectQALYMethod.Where(a => a.PID == p.ID).Where(c => c.NodeType != 3000).Count() == 0))
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
                            //------如果id不在lstAllSelectCRFunction中--则删除---
                            //------如果nodetype==5的父节点没有，则删除
                        }
                    }
                }
                lstVBRemove = new List<ValuationMethodPoolingAndAggregationBase>();
                foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                {
                    if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion == null || vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Count == 0)
                    {
                        //remove----------------
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

        /// <summary>
        /// load cfgx后的操作
        /// </summary>
        /// <param name="baseControlCRSelectFunction"></param>
        /// <param name="currentNode"></param>
        private void showExistBaseControlCRSelectFunction(BaseControlCRSelectFunction baseControlCRSelectFunction, TreeNode currentNode)
        {
            //----------Set maxcrid----
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
            //--------------变绿并且加节点----------------------------------------------------------------------
            // TreeNode tr = currentNode.Parent;
            foreach (TreeNode trchild in trvSetting.Nodes)
            {
                if (trchild.Name == "airqualitygridgroup")
                {
                    nodesCount = trchild.Nodes.Count;

                    //currentNode.Tag = CommonClass.LstPollutant;

                    for (int i = nodesCount - 1; i > -1; i--)
                    {
                        TreeNode node = trchild.Nodes[i];
                        if (trchild.Nodes[i].Name == "datasource") { trchild.Nodes.RemoveAt(i); }
                    }
                    // CommonClass.LstBaseControlGroup = new List<BaseControlGroup>(CommonClass.LstPollutant.Count);
                    // for (int i = 0; i < CommonClass.LstPollutant.Count; i++)
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
            //CommonClass.lst
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
            //mainMap.Layers.Clear();
            mainMap.ProjectionModeReproject = ActionMode.Never;
            mainMap.ProjectionModeDefine = ActionMode.Never;

            string s = isBase;
            //if (isBase) s = "B";
            try
            {
                if (File.Exists(benMAPLine.ShapeFile))
                {
                    try
                    {
                        mainMap.Layers.Add(benMAPLine.ShapeFile);
                    }
                    catch (Exception ex)
                    {
                        DataSourceCommonClass.SaveBenMAPLineShapeFile(CommonClass.GBenMAPGrid, benMAPLine.Pollutant, benMAPLine, benMAPLine.ShapeFile);
                        mainMap.Layers.Add(benMAPLine.ShapeFile);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(benMAPLine.ShapeFile))
                    {
                        benMAPLine.ShapeFile = benMAPLine.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "B" + s + ".shp";
                        benMAPLine.ShapeFile = string.Format("{0}\\Tmp\\{1}", CommonClass.DataFilePath, benMAPLine.ShapeFile);
                    }
                    DataSourceCommonClass.SaveBenMAPLineShapeFile(CommonClass.GBenMAPGrid, benMAPLine.Pollutant, benMAPLine, benMAPLine.ShapeFile);
                    mainMap.Layers.Add(benMAPLine.ShapeFile);
                    // return;
                }
                MapPolygonLayer polLayer = mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer;
                //-----修正by xiejp20120207--把略掉的字段名补全----------------
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
                for (int iAddField = 2; iAddField < 2 + lstAddField.Count; iAddField++)
                {
                    polLayer.DataSet.DataTable.Columns[iAddField].ColumnName = lstAddField[iAddField - 2];
                }
                if (isBase == "B") polLayer.LegendText = "Baseline";
                if (isBase == "D") polLayer.LegendText = "Delta";
                if (isBase == "C") polLayer.LegendText = "Control";
                string strValueField = polLayer.DataSet.DataTable.Columns[2].ColumnName;
                PolygonScheme myScheme1 = new PolygonScheme();
                float fl = (float)0.1;
                myScheme1.EditorSettings.StartColor = Color.Blue;
                myScheme1.EditorSettings.StartColor.ToTransparent(fl);
                myScheme1.EditorSettings.EndColor = Color.Red;
                myScheme1.EditorSettings.EndColor.ToTransparent(fl);

                myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
                //myScheme1.EditorSettings.ClassificationType = ClassificationType.UniqueValues ;
                myScheme1.EditorSettings.NumBreaks = 6;
                myScheme1.EditorSettings.FieldName = strValueField;// "Value";
                myScheme1.EditorSettings.UseGradient = false;
                myScheme1.CreateCategories(polLayer.DataSet.DataTable);

                //PolygonCategory pc = new PolygonCategory(Color.LightBlue, Color.DarkBlue, 1);
                //pc.FilterExpression = "[Value] < 6.5 ";
                //pc.LegendText = "0-6.5";
                //myScheme1.AddCategory(pc);
                //----------
                //UniqueValues+半透明
                double dMinValue = 0.0;
                double dMaxValue = 0.0;
                //Base或者Control，默认都按Base的区间来画，以保证Base和Control都是同一区间
                dMinValue = benMAPLine.ModelResultAttributes.Min(a => a.Values[strValueField]);
                dMaxValue = benMAPLine.ModelResultAttributes.Max(a => a.Values[strValueField]);

                if (double.IsNaN(dMinValue)) dMinValue = 0;
                if (double.IsNaN(dMaxValue)) dMaxValue = 0;
                if (isBase == "C")
                {
                    try
                    {
                        foreach (BaseControlGroup baseControlGroup in CommonClass.LstBaseControlGroup)
                        {

                            if (baseControlGroup.GridType.GridDefinitionID == benMAPLine.GridType.GridDefinitionID && baseControlGroup.Pollutant.PollutantID == benMAPLine.Pollutant.PollutantID)
                            {
                                if (baseControlGroup.Base != null && baseControlGroup.Base.ModelResultAttributes != null && baseControlGroup.Base.ModelResultAttributes.Count > 0)
                                {
                                    dMinValue = baseControlGroup.Base.ModelResultAttributes.Min(a => a.Values.ToArray()[0].Value);
                                    dMaxValue = baseControlGroup.Base.ModelResultAttributes.Max(a => a.Values.ToArray()[0].Value);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                }

                //Color[] colors = new Color[] { Color.Blue, Color.FromArgb(0, 255, 255), Color.FromArgb(0, 255, 0), Color.Yellow, Color.Red, Color.FromArgb(255, 0, 255) };
                ////Quantities+半透明
                //PolygonCategoryCollection pcc = new PolygonCategoryCollection();
                //int iColor = 0;

                //foreach (PolygonCategory pc in myScheme1.Categories)
                //{
                //    //pc.Symbolizer.SetOutlineWidth(0);
                //    PolygonCategory pcin = pc;
                //    double dnow =  dMinValue + ((dMaxValue - dMinValue) / 6.00) * Convert.ToDouble(iColor) ;
                //    double dnowUp =  dMinValue + ((dMaxValue - dMinValue) / 6.00) * Convert.ToDouble(iColor + 1) ;

                //    //if (iColor == myScheme1.Categories.Count - 1) dnowUp = dnowUp + 1.000000;
                //    //dnow = Math.Round(dnow, 2);
                //    //dnowUp = Math.Round(dnowUp, 2);
                //    pcin.FilterExpression = string.Format("[{0}]>=" + dnow+ " and [{0}] <" + dnowUp, strValueField);
                //    pcin.LegendText=string.Format("[{0}]>=" + dnow.ToString("E2") + " and [{0}] <" + dnowUp.ToString("E2"), strValueField);
                //    if (iColor == 0)
                //    {
                //        pcin.FilterExpression = string.Format(" [{0}] <" + dnowUp, strValueField);
                //        pcin.LegendText = string.Format(" [{0}] <" + dnowUp.ToString("E2"), strValueField);

                //    }
                //    if (iColor == myScheme1.Categories.Count - 1)
                //    {
                //        pcin.FilterExpression = string.Format(" [{0}] >=" + dnow, strValueField);
                //        pcin.LegendText = string.Format(" [{0}] >=" + dnow.ToString("E2"), strValueField);

                //    }

                //    //pcin.LegendText = pcin.FilterExpression;// string.Format("{0} >= " + dnow + " and {0} < " + dnowUp, strValueField);

                //    pcin.Symbolizer.SetOutline(Color.Transparent, 0);
                //    Color ctemp = pcin.Symbolizer.GetFillColor();
                //    float fColor = (float)0.2;
                //    ctemp.ToTransparent(fColor);
                //    pcin.Symbolizer.SetFillColor(colors[iColor]);
                //    pcc.Add(pcin);
                //    iColor++;
                //}
                //myScheme1.ClearCategories();
                //foreach (PolygonCategory pct in pcc)
                //{
                //    myScheme1.Categories.Add(pct);
                //}
                ////player.Symbology = myScheme1;

                //polLayer.Symbology = myScheme1;
                _currentLayerIndex = mainMap.Layers.Count - 1;
                _dMinValue = dMinValue;
                _dMaxValue = dMaxValue;
                _columnName = strValueField;
                RenderMainMap(true);
            }
            catch (Exception ex)
            {
            }
        }

        private string _drawStatus = string.Empty;//判定GIS图片是哪张GIS图片：例如：base_pm2.5;
        private double _dMinValue = 0.0;//最小值
        private double _dMaxValue = 0.0;// 最大值
        private int _currentLayerIndex = 1;
        private string _columnName = string.Empty;
        private Color[] _blendColors;

        /// <summary>
        /// 图例改变后的颜色
        /// </summary>
        public Color[] BlendColors
        {
            get { return _blendColors; }
            set { _blendColors = value; }
        }
        /// <summary>
        /// 重新Render
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetGisMap(object sender, EventArgs e)
        {
            try
            {

                _blendColors = colorBlend.ColorArray;
                _dMaxValue = colorBlend.MaxValue;
                _dMinValue = colorBlend.MinValue;
                //colorBlend.s
                colorBlend.SetValueRange(_dMinValue, _dMaxValue, false);
                //colorBlend._minPlotValue = _dMinValue;
                //colorBlend._maxPlotValue = _dMaxValue;
                Color[] colors = new Color[_blendColors.Length];
                _blendColors.CopyTo(colors, 0);
                //Quantities+半透明
                PolygonCategoryCollection pcc = new PolygonCategoryCollection();
                int iColor = 0;
                //PolygonScheme pgs = (mainMap.Layers[_currentLayerIndex] as IFeatureLayer).Symbology as PolygonScheme;
                string ColumnName = _columnName;
                PolygonScheme myScheme1 = new PolygonScheme();
                float fl = (float)0.1;
                //myScheme1.EditorSettings.StartColor = Color.Blue;
                //myScheme1.EditorSettings.StartColor.ToTransparent(fl);
                //myScheme1.EditorSettings.EndColor = Color.Red;
                //myScheme1.EditorSettings.EndColor.ToTransparent(fl);
                float fColor = (float)0.2;
                Color ctemp = new Color();
                //myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
                ////myScheme1.EditorSettings.ClassificationType = ClassificationType.UniqueValues ;
                //myScheme1.EditorSettings.NumBreaks = 6;
                //myScheme1.EditorSettings.FieldName = _columnName;// "Value";
                //myScheme1.EditorSettings.UseGradient = false;
                //myScheme1.CreateCategories((mainMap.Layers[_currentLayerIndex] as IFeatureLayer).DataSet.DataTable);
                //if (myScheme1.Categories.Count == 1)
                //{

                //    //pc.Symbolizer.SetOutlineWidth(0);
                //    PolygonSymbolizer ps = new PolygonSymbolizer();
                //    ps.SetFillColor(colors[iColor]);
                //    ps.SetOutline(Color.Transparent, 0);
                //    //player.Symbology = myScheme1;

                //    (mainMap.Layers[_currentLayerIndex] as IFeatureLayer).Symbolizer = ps;
                //    return;

                //}
                iColor = 0;
                //foreach (PolygonCategory pc in myScheme1.Categories)
                for (int iBlend = 0; iBlend < 6; iBlend++)
                {
                    //pc.Symbolizer.SetOutlineWidth(0);
                    PolygonCategory pcin = new PolygonCategory();
                    double dnow = 0;// Math.Round(_dMinValue + ((_dMaxValue - _dMinValue) / 6.00) * Convert.ToDouble(iColor), 3);
                    double dnowUp = 0;// Math.Round(_dMinValue + ((_dMaxValue - _dMinValue) / 6.00) * Convert.ToDouble(iColor + 1), 3);
                    //------------实现value--
                    dnow = colorBlend.ValueArray[iBlend];
                    if (iBlend < 5)
                        dnowUp = colorBlend.ValueArray[iBlend + 1];
                    pcin.FilterExpression = string.Format("[{0}]>=" + dnow + " and [{0}] <" + dnowUp, ColumnName);
                    pcin.LegendText = ">=" + dnow.ToString() + " and <" + dnowUp.ToString();// string.Format("[{0}]>=" + dnow.ToString("E2") + " and [{0}] <" + dnowUp.ToString("E2"), ColumnName);
                    if (iBlend == 0)
                    {
                        pcin.FilterExpression = string.Format(" [{0}] <" + dnowUp, ColumnName);
                        //pcin.LegendText = string.Format(" [{0}] <" + dnowUp.ToString("E2"), ColumnName);
                        pcin.LegendText = "<" + dnowUp.ToString();
                    }
                    if (iBlend == 5)
                    {
                        pcin.FilterExpression = string.Format(" [{0}] >=" + dnow, ColumnName);
                        //pcin.LegendText = string.Format(" [{0}] >=" + dnow.ToString("E2"), ColumnName);
                        pcin.LegendText = ">=" + dnow.ToString();
                    }

                    //pcin.LegendText = pcin.FilterExpression;// string.Format("{0} >= " + dnow + " and {0} < " + dnowUp, strValueField);

                    pcin.Symbolizer.SetOutline(Color.Transparent, 0);
                    ctemp = pcin.Symbolizer.GetFillColor();
                    pcin.Symbolizer.SetFillColor(ctemp.ToTransparent(fColor));
                    ctemp.ToTransparent(fColor);
                    pcin.Symbolizer.SetFillColor(colors[iColor]);
                    pcc.Add(pcin);
                    iColor++;
                }
                myScheme1.ClearCategories();
                foreach (PolygonCategory pct in pcc)
                {
                    myScheme1.Categories.Add(pct);
                }
                //player.Symbology = myScheme1;
                //if (myScheme1.LegendText == "Pooled Inci") myScheme1.LegendText = "Pooled Incidence";
                //if (myScheme1.LegendText == "Pooled Valu") myScheme1.LegendText = "Pooled Valuation";
                myScheme1.AppearsInLegend = true;
                myScheme1.LegendText = ColumnName;
                myScheme1.EditorSettings.ClassificationType = ClassificationType.Custom;
                (mainMap.Layers[_currentLayerIndex] as IFeatureLayer).Symbology = myScheme1;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        /// <summary>
        /// Render 图层(包括base control CR APV QALY),以_dMinValue,_dMaxValue 作为Render的最小最大数值，分6个颜色段
        /// </summary>
        /// <param name="isCone"></param>
        private void RenderMainMap(bool isCone)
        {
            //----------try
            double min = _dMinValue;
            double max = _dMaxValue;
            colorBlend.SetValueRange(min, max, true);
            colorBlend._minPlotValue = _dMinValue;
            colorBlend._maxPlotValue = _dMaxValue;
            ResetGisMap(null, null);
            return;
            //-------end ----------
            Color[] colors = new Color[] { Color.Blue, Color.FromArgb(0, 255, 255), Color.FromArgb(0, 255, 0), Color.Yellow, Color.Red, Color.FromArgb(255, 0, 255) };
            // 初始化ColorBlendControl
            //ColorBlendControl cb = new ColorBlendControl();
            //cb.SetValueRange(_dMinValue, _dMaxValue, true);
            //colorBlend = cb;
            //colors = colorBlend.ColorArray;
            //_blendColors = colorBlend.ColorArray;
            //_dMinValue = colorBlend.MinValue;
            //_dMaxValue = colorBlend.MaxValue;
            //colorBlend = new ColorBlendControl();
            //double min = _dMinValue;
            //double max = _dMaxValue;
            colorBlend.SetValueRange(min, max, true);
            _blendColors = colorBlend.ColorArray;
            _dMinValue = colorBlend.MinValue;
            _dMaxValue = colorBlend.MaxValue;

            //colorBlend.ValueUnit=CommonClass.
            //Quantities+半透明
            PolygonCategoryCollection pcc = new PolygonCategoryCollection();
            int iColor = 0;
            PolygonScheme myScheme1 = new PolygonScheme();
            float fl = (float)0.1;
            myScheme1.EditorSettings.StartColor = Color.Blue;
            //myScheme1.EditorSettings.StartColor.ToTransparent(fl);
            myScheme1.EditorSettings.EndColor = Color.FromArgb(255, 0, 255);
            //myScheme1.EditorSettings.EndColor.ToTransparent(fl);
            float fColor = (float)0.2;
            Color ctemp = new Color();
            if (isCone)
            {
                myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
                //myScheme1.EditorSettings.ClassificationType = ClassificationType.UniqueValues ;
                myScheme1.EditorSettings.NumBreaks = 6;
                myScheme1.EditorSettings.FieldName = _columnName;// "Value";
                myScheme1.EditorSettings.UseGradient = false;
                myScheme1.CreateCategories((mainMap.Layers[_currentLayerIndex] as IFeatureLayer).DataSet.DataTable);
                if (myScheme1.Categories.Count == 1)
                {

                    //pc.Symbolizer.SetOutlineWidth(0);
                    PolygonSymbolizer ps = new PolygonSymbolizer();
                    ps.SetFillColor(colors[iColor]);
                    ps.SetOutline(Color.Transparent, 0);
                    //player.Symbology = myScheme1;

                    (mainMap.Layers[_currentLayerIndex] as IFeatureLayer).Symbolizer = ps;
                    return;

                }
                else
                {
                    foreach (PolygonCategory pc in myScheme1.Categories)
                    {
                        //pc.Symbolizer.SetOutlineWidth(0);
                        PolygonCategory pcin = pc;
                        double dnow = Math.Round(_dMinValue + ((_dMaxValue - _dMinValue) / 6.0000) * Convert.ToDouble(iColor), 3);
                        double dnowUp = Math.Round(_dMinValue + ((_dMaxValue - _dMinValue) / 6.0000) * Convert.ToDouble(iColor + 1), 3);

                        //if (iColor == myScheme1.Categories.Count - 1) dnowUp = dnowUp + 1.000000;
                        //dnow = Math.Round(dnow, 2);
                        //dnowUp = Math.Round(dnowUp, 2);
                        pcin.FilterExpression = string.Format("[{0}]>=" + dnow + " and [{0}] <" + dnowUp, _columnName);
                        // pcin.LegendText = string.Format("[{0}]>=" + dnow.ToString("E2") + " and [{0}] <" + dnowUp.ToString("E2"), _columnName);
                        pcin.LegendText = string.Format(">=" + dnow.ToString("E2") + " and  <" + dnowUp.ToString("E2"), _columnName);
                        if (iColor == 0)
                        {
                            pcin.FilterExpression = string.Format(" [{0}] <" + dnowUp, _columnName);
                            // pcin.LegendText = string.Format(" [{0}] <" + dnowUp.ToString("E2"), _columnName);
                            pcin.LegendText = string.Format("<" + dnowUp.ToString("E2"), _columnName);
                        }
                        if (iColor == myScheme1.Categories.Count - 1)
                        {
                            pcin.FilterExpression = string.Format(" [{0}] >=" + dnow, _columnName);
                            // pcin.LegendText = string.Format(" [{0}] <" + dnowUp.ToString("E2"), _columnName);
                            pcin.LegendText = string.Format("<" + dnowUp.ToString("E2"), _columnName);

                        }

                        //pcin.LegendText = pcin.FilterExpression;// string.Format("{0} >= " + dnow + " and {0} < " + dnowUp, strValueField);

                        pcin.Symbolizer.SetOutline(Color.Transparent, 0);
                        ctemp = pcin.Symbolizer.GetFillColor();
                        pcin.Symbolizer.SetFillColor(ctemp.ToTransparent(fColor));
                        ctemp.ToTransparent(fColor);
                        pcin.Symbolizer.SetFillColor(colors[iColor]);
                        pcc.Add(pcin);
                        iColor++;
                    }
                }
                myScheme1.ClearCategories();
                foreach (PolygonCategory pct in pcc)
                {
                    myScheme1.Categories.Add(pct);
                }
                //player.Symbology = myScheme1;
                //if (myScheme1.LegendText == "Pooled Inci") myScheme1.LegendText = "Pooled Incidence";
                //if (myScheme1.LegendText == "Pooled Valu") myScheme1.LegendText = "Pooled Valuation";
                (mainMap.Layers[_currentLayerIndex] as IFeatureLayer).Symbology = myScheme1;
            }
            else
            {
                pcc = new PolygonCategoryCollection();
                myScheme1.EditorSettings.ClassificationType = ClassificationType.UniqueValues;
                //myScheme1.EditorSettings.ClassificationType = ClassificationType.UniqueValues ;
                //myScheme1.EditorSettings.NumBreaks = 6;
                myScheme1.EditorSettings.FieldName = _columnName;// "Value";
                myScheme1.EditorSettings.UseGradient = false;

                //myScheme1.EditorSettings.GradientAngle = -45;
                // myScheme1.EditorSettings.
                myScheme1.CreateCategories((mainMap.Layers[_currentLayerIndex] as IFeatureLayer).DataSet.DataTable);
                foreach (PolygonCategory pc in myScheme1.Categories)
                {
                    PolygonCategory pcin = pc;
                    //pcin.SelectionSymbolizer.ou
                    pcin.Symbolizer.SetOutline(Color.Transparent, 0);
                    //pcin.Symbolizer.SetFillColor(pc.Symbolizer.GetFillColor());
                    //pcin.Symbolizer.OutlineSymbolizer.IsVisible = false;
                    //(pcin.Symbolizer.OutlineSymbolizer as IOutlinedSymbol).UseOutline = false;
                    //pcin.Symbolizer.SetOutlineWidth(0.000000000001);
                    //ctemp = pcin.Symbolizer.GetFillColor();
                    //pcin.Symbolizer.SetFillColor(ctemp.ToTransparent(fColor));
                    //ctemp.ToTransparent(fColor);
                    pcc.Add(pcin);
                }
                myScheme1.ClearCategories();
                foreach (PolygonCategory pct in pcc)
                {
                    myScheme1.Categories.Add(pct);
                }
                if (myScheme1.LegendText == "Pooled Inci") myScheme1.LegendText = "Pooled Incidence";
                if (myScheme1.LegendText == "Pooled Valu") myScheme1.LegendText = "Pooled Valuation";
                (mainMap.Layers[_currentLayerIndex] as IFeatureLayer).Symbology = myScheme1;
                //((mainMap.Layers[layerIndex] as IFeatureLayer).Symbolizer as PolygonSymbolizer).SetOutline(Color.Transparent, 0);
            }
        }
        /// <summary>
        /// 加背景图层--如US默认加State图层
        /// </summary>
        private void addRegionLayerToMainMap()
        {
            try
            {
                if (CommonClass.RBenMAPGrid == null)
                {
                    try
                    {
                        DataRowView drGrid = cboRegion.SelectedItem as DataRowView;
                        CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));// GetBenMapGridDefinitions(drGrid);
                    }
                    catch
                    {
 
                    }
                }
                bool isWGS83 = true;
                if (tsbChangeProjection.Text == "change projection to GCS/NAD 83")
                {
                    tsbChangeProjection_Click(null, null);
                    isWGS83 = false;
                }
                if (CommonClass.RBenMAPGrid == null)
                {
                    cboRegion.SelectedIndex = 0;
                };
                mainMap.ProjectionModeReproject = ActionMode.Never;
                mainMap.ProjectionModeDefine = ActionMode.Never;
                if (CommonClass.RBenMAPGrid is ShapefileGrid)
                {
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                    {
                        mainMap.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as ShapefileGrid).ShapefileName + ".shp");
                    }
                }
                else if (CommonClass.RBenMAPGrid is RegularGrid)
                {
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as RegularGrid).ShapefileName + ".shp"))
                    {
                        mainMap.Layers.Add(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.RBenMAPGrid as RegularGrid).ShapefileName + ".shp");
                    }
                }
                //modify name to GridDefinitionName
                mainMap.Layers[mainMap.Layers.Count() - 1].LegendText = CommonClass.RBenMAPGrid.GridDefinitionName;
                PolygonLayer playerRegion = mainMap.Layers[mainMap.Layers.Count - 1] as PolygonLayer;
                // float f = (float)0.9;
                Color cRegion = Color.Transparent;
                PolygonSymbolizer TransparentRegion = new PolygonSymbolizer(cRegion);

                //Transparent.OutlineSymbolizer = new LineSymbolizer(Color.Black, 1);
                TransparentRegion.OutlineSymbolizer = new LineSymbolizer(Color.DarkBlue, 1);    //设置region图层outline宽度为2
                playerRegion.Symbolizer = TransparentRegion;
                if (isWGS83 == false)
                {
                    tsbChangeProjection_Click(null, null);
                    //isWGS83 = false;
                }
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 点击Base/Control引发函数，相应弹出
        /// </summary>
        /// <param name="currStat">状态Base/Control</param>
        /// <param name="currentNode">当前节点</param>
        /// <returns></returns>
        private bool BaseControlOP(string currStat, ref TreeNode currentNode)
        {
            string msg = string.Empty;
            DialogResult rtn;
            BaseControlGroup bcg = new BaseControlGroup();
            BenMAPLine bml = new BenMAPLine();
            //MonitorDataLine monitorDataLine =new MonitorDataLine();
            try
            {
                //if (CommonClass.GBenMAPGrid == null) { msg = "Grid Type not set up!"; return false; }
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
                //foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
                //{
                //    if (int.Parse(nodeTag) == b.Pollutant.PollutantID) { bcg = b; break; }
                //}
                bool isGridTypeChanged = false;
                bool removeNode = true;//remove node when change gridtype - (if : rollback- save baseline- don't remove note) 
                switch (currStat)
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
                            //monitorDataLine = frm.MDataLine;
                            //DataSourceCommonClass.UpdateModelValuesMonitorData(bcg.GridType, bcg.Pollutant, ref monitorDataLine);
                            //bcg.Base = monitorDataLine;
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
                                        //CommonClass.LstBaseControlGroup[i].Control = null;
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
                            //monitorDataLine = frm.MDataLine;
                            //DataSourceCommonClass.UpdateModelValuesMonitorData(bcg.GridType, bcg.Pollutant, ref monitorDataLine);
                            //bcg.Base = monitorDataLine;
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
                                        //CommonClass.LstBaseControlGroup[i].Base = null;
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
                //--------------如果有GridType的改动-----------------------
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
                            // e.Node.Parent.Nodes[i].Text = "Data Source";
                            // e.Node.Parent.Nodes[i].Nodes[0].Tag = null;
                            if (bcg.Pollutant.PollutantID != int.Parse(currentNode.Parent.Parent.Nodes[i].Tag.ToString()))
                            {
                                currentNode.Parent.Parent.Nodes[i].Nodes[0].Nodes.Clear();
                                initNodeImage(currentNode.Parent.Parent.Nodes[i].Nodes[0]);
                                //  e.Node.Parent.Nodes[i].Nodes[1].Tag = null;
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
                    //tlvQALYResult.SetObjects(null);

                    cbPoolingWindowIncidence.Items.Clear();
                    cbPoolingWindowAPV.Items.Clear();
                    //cbPoolingWindowQALY.Items.Clear();
                    //----------变黄-----------
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
                //---------------------------------------------------------
                string strName = "";
                try
                {
                    strName = frm.StrPath.Substring(frm.StrPath.LastIndexOf(@"\") + 1);
                }
                catch
                { }
                AddChildNodes(ref currentNode, frm.PageStat, strName, bml);
                bcg.DeltaQ = null;
                //
                //ChangeNodeStatus();
                //changeNodeImage(currentNode);
                currentNode.ExpandAll();
                //--------------modify by xiejp1209修改base和control时必须清理掉CRResult-APVX一级APVResult--并把下面的变黄------
                if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null)
                {
                    CommonClass.BaseControlCRSelectFunctionCalculateValue = null;//.lstCRSelectFunctionCalculateValue
                }
                //CommonClass.lstIncidencePoolingAndAggregation = null;
                //CommonClass.ValuationMethodPoolingAndAggregation = null;
                //--变黄----------
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

        /// <summary>
        /// 刷新树的DataSoure节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="currStat"></param>
        /// <param name="txt"></param>
        /// <param name="bml"></param>
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
                if (CommonClass.LstPollutant != null && CommonClass.LstPollutant.Count > 0)
                {
                    changeNodeImage(trvSetting.Nodes[0].Nodes[0]);
                }
                node.ExpandAll();
                pnode.Nodes.RemoveAt(index);
                pnode.Nodes.Insert(index, node);
                pnode.ExpandAll();
                //--------------modify by xiejp1209修改base和control时必须清理掉CRResult-APVX一级APVResult--并把下面的变黄------
                if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null)
                {
                    CommonClass.BaseControlCRSelectFunctionCalculateValue = null;//.lstCRSelectFunctionCalculateValue
                }
                //--变黄----------
                //if (bcg.Base != null && bcg.Control != null) 
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

        //
        /// <summary>
        /// 为树节点添加子节点（当增加Pollutant时发生)
        /// </summary>
        /// <param name="node"></param>
        /// <param name="currStat">base/control</param>
        /// <param name="txt"></param>
        /// <param name="bml"></param>
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
                        node.Nodes.Clear();//清空现有的所有子节点
                        newNode = CreateNewNode(CommonClass.GBenMAPGrid, "Grid");
                        if (newNode != null) { node.Nodes.Add(newNode); }
                        if (CommonClass.RBenMAPGrid != null)
                        {
                            //newNode = CreateNewNode(CommonClass.RBenMAPGrid, "Aggregation");
                            string target = "Domain";
                            BenMAPGrid grid = CommonClass.RBenMAPGrid;
                            newNode = new TreeNode
                            {
                                Name = "region",
                                Text = string.Format("{0}: {1}", target, grid.GridDefinitionName),
                                Tag = grid,
                                ToolTipText = "Double-click domain data file to display ",// string.Format("{0}: {1}", target, grid.GridDefinitionName),
                                ImageKey = "doc",
                                SelectedImageKey = "doc",
                            };
                            if (newNode != null) { node.Nodes.Add(newNode); }
                        }
                        break;
                    case "baseline":
                        node.Nodes.Clear();//清空现有的所有子节点
                        //tag = node.Tag.ToString();

                        foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
                        {
                            if (int.Parse(node.Parent.Tag.ToString()) == b.Pollutant.PollutantID) { bcg = b; break; }
                        }
                        if (string.IsNullOrEmpty(txt)) { txt = "data from library"; }
                        newNode = new TreeNode()
                        {
                            Name = "basedata",
                            Text = txt,//string.Format("{0}: {1}", currStat, txt),
                            Tag = bml,
                            ToolTipText = "Double-click AQ data file to display ",//string.Format("{0}:{1}", tag, shapeFile),
                            ImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
                            SelectedImageKey = (bcg.Base != null && bcg.Base.ModelResultAttributes != null && bcg.Base.ModelResultAttributes.Count > 0) ? "doc" : "docgrey",
                        };
                        node.Nodes.Add(newNode);
                        break;
                    case "control":
                        node.Nodes.Clear();//清空现有的所有子节点
                        //tag = node.Tag.ToString();
                        foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
                        {
                            if (int.Parse(node.Parent.Tag.ToString()) == b.Pollutant.PollutantID) { bcg = b; break; }
                        }
                        newNode = new TreeNode()
                        {
                            Name = "controldata",
                            Text = txt, //string.Format("{0}: {1}", currStat, txt),
                            Tag = bml,
                            ToolTipText = "Double-click AQ data file to display ", //string.Format("{0}:{1}", tag, shapeFile),
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

        /// <summary>
        /// 添加GridTYPE下的节点
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="target"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 把节点图片显示为初始图片：黄色
        /// </summary>
        /// <param name="rootNode"></param>
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
        /// <summary>
        /// 把节点图片显示为错误图片：红色
        /// </summary>
        /// <param name="rootNode"></param>
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
        /// <summary>
        /// 标记已经选择数据的treeView节点的图标为“ready”
        /// </summary>
        /// <param name="rootNode">已经选择数据的节点</param>
        private void changeNodeImage(TreeNode rootNode)
        {
            bool hasUnRead = false;
            try
            {
                hasUnRead = JudgeStatus(rootNode);
                if (hasUnRead) { return; }
                rootNode.ImageKey = _readyImageKey;
                rootNode.SelectedImageKey = _readyImageKey;
                // rootNode.SelectedImageKey = _readyImageKey;
                //TreeNode node = rootNode.Parent;
                //foreach (TreeNode child in node.Nodes)
                //{
                //    if (child.SelectedImageKey == "unready" && child.ImageKey == "unready") { hasUnRead = true; break; }
                //}
                //hasUnRead = JudgeStatus(node);

                //if (!hasUnRead) { node.ImageKey = _readyImageKey; node.SelectedImageKey = _readyImageKey; }
                //rootNode.Tag = _readyImageKey;
                //foreach (TreeNode childNode in rootNode.Nodes)
                //{
                //    childNode.ImageKey = _readyImageKey;
                //    childNode.SelectedImageKey = _readyImageKey;
                //}
                rootNode.ExpandAll();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        /// <summary>
        /// 判断状态
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
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
                    // case "healthimpactfunctions":
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
            //if (pnlSpatial.Visible == true)
            //{
            //    pnlSpatial.Visible = false;
            //}
            //else
            //{
            //    pnlSpatial.Visible = true;
            //}
        }

        #region ListView设置

        /// <summary>
        /// Configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void btnRawGenerate_Click(object sender, EventArgs e)
        //{
        //    //if (lvwRawForm.FocusedItem == null)
        //    //{
        //    //    MessageBox.Show("Please select a report form.", "Select Form", MessageBoxButtons.OK);
        //    //}
        //    //else
        //    //{
        //    //    switch (lvwRawForm.FocusedItem.Text.ToString())
        //    //    {
        //    //        case "Table":
        //    //            //ColumnSetting colSettingFrm = new ColumnSetting();
        //    //            //DialogResult rtn = colSettingFrm.ShowDialog();
        //    //            //if (rtn != DialogResult.OK)
        //    //            //{ return; }
        //    //            //else
        //    //            //{
        //    //            //if (string.IsNullOrEmpty(CommonClass.ConfigurationResultPath))
        //    //            //{
        //    //            //    MessageBox.Show("Please finish Configuration process first!");
        //    //            //}
        //    //            //else
        //    //            //{
        //    //            //    ShowTable(CommonClass.ConfigurationResultPath.ToString());
        //    //            //}

        //    //            //}
        //    //            break;
        //    //        case "Box Plot":
        //    //            ShowBoxPlot();
        //    //            break;
        //    //        case "Cumulative Distribution":
        //    //            ShowCumulative();
        //    //            break;
        //    //        case "Bar Chart":
        //    //            //if (CommonClass.ConfigurationResultPath == null)
        //    //            //{
        //    //            //    MessageBox.Show("Please finish Configuration process first!");
        //    //            //}
        //    //            //else
        //    //            //{
        //    //            //    ShowChart(CommonClass.ConfigurationResultPath.ToString());
        //    //            //}

        //    //            break;
        //    //        default:
        //    //            break;
        //    //    }
        //    //}
        //}

        /// <summary>
        /// 在tabMain显示默认表格
        /// </summary>
        private void ShowTable(string file)
        {
            //string file = Application.StartupPath + @"\Data\USAData\Valuation_Result.csv";

            //DataWorker.DataParser dp = new DataWorker.DataParser();
            //dgvData .DataSource = dp.ReadCSV2DataSet(file,"Pooled Valuation");
            _reportTableFileName = file;
            //DataReader dp = new DataReader();
            //_dtInfo = dp.ReadCSV2DataTable(file);
            //InitDataSet();
            //ConverCSV2DataSet(file);
            tabCtlMain.SelectTab(tabData);
        }

        // 4、菜单响应事件：

        /// <summary>
        /// 在tabMain显示默认Bar Chart
        /// </summary>
        private void ShowChart(string resultFile)
        {
            zedGraphCtl.Visible = true;
            ZedGraphResult(zedGraphCtl, resultFile);
            zedGraphCtl.AxisChange();
            zedGraphCtl.Refresh();
            //System.Drawing.Image backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\Chart.jpg");
            //pnlChart.BackgroundImage = backImg;
            tabCtlMain.SelectTab(tabChart);
        }

        private void ZedGraphResult(ZedGraphControl zgc, string file)
        {
            //string[] strValuations = { "Woodruff,T.J.J,J.D.Patker and K.C.Schoendorf.2006.Fine particulate matter (PM2.5) air pollution and selected causes of postneonatal infant mortality in California,Environmental Health Perspectives.Vol.114:186-90.", "Laden,F.,J.Schwartz,F.E. Speizer and D.W.Dockery.2006.Reduction in Fine Particulate Air Pollution and Mortalidy: Extended follow-up of the Harvard Six Cities Study.Am J Respir Crit Care Med.", "Pope,C.A.,3rd,R.T. Burnett,M.J.Thun,E.E.Calle,D.Krewski,K.Ito and G.D. Thurston.2002. Lung cancer,cardiopulmonary motralitym,and long-term exposure to fine particulate air pollution.Jama.Vol.287(9):1132-41." };
            try
            {
                //string strSavePath = Application.StartupPath + @"\Data\USAData\Valuation_Result.csv";
                //DataWorker.DataReader dp = new DataWorker.DataReader();
                //DataTable dt = dp.ReadCSV2DataTable(Application.StartupPath + @"\Data\USAData\pointABincidence.csv");
                System.Data.DataTable dt = DataSourceCommonClass.getDataSetFromCSV(file).Tables[0];// (dp.GetDataFromFile(file)).Tables[0];
                //System.Data.DataTable dt = CommonClass.DSValuationResult.Tables[0];
                // DataSet dszed = new System.Data.DataSet();
                System.Data.DataSet dsOut = new System.Data.DataSet();

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
                    // i++;
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

                //CommonClass.ListRegionRowCol

                string[] strValuations = { "Woodruff,etc.", "Laden,etc.", "Pope,etc." };
                List<string> strValuationsNow = new List<string>();
                //foreach (int iv in CommonClass.ListIncidenceResult)
                //{
                //    strValuationsNow.Add(strValuations[iv].ToString());
                //    //TreeNode tnnn = new TreeNode();
                //    //tnnn.Text = strValuations[iv].ToString();
                //    //tnn.Nodes.Add(tnnn);
                //}
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
                    //  myPane.AddBar(myCurve);

                    i++;
                }
                myPane.Chart.Fill = new Fill(Color.White,
                 Color.FromArgb(255, 255, 166), 45.0F);

                // expand the range of the Y axis slightly to accommodate the labels
                //myPane.YAxis.Scale.Max += myPane.YAxis.Scale.MajorStep;
                // myPane.YAxis.Scale.TextLabels = strValuationsNow.ToArray();
                myPane.XAxis.Scale.TextLabels = new string[] { dicRegionValues[0].Keys.ToArray()[0], dicRegionValues[0].Keys.ToArray()[1], dicRegionValues[0].Keys.ToArray()[2], dicRegionValues[0].Keys.ToArray()[3], dicRegionValues[0].Keys.ToArray()[4] };
                //myPane.YAxis.Type= AxisType.
                myPane.XAxis.Type = AxisType.Text;
                myPane.XAxis.Scale.FontSpec.Angle = 65;
                myPane.XAxis.Scale.FontSpec.IsBold = true;
                myPane.XAxis.Scale.FontSpec.Size = 12;
                // Create TextObj's to provide labels for each bar
                //  BarItem.CreateBarLabels(myPane, false, "f0");
                // zgc.Controls.Add(myPane);
                //zgc.GraphPane = myPane;
                zgc.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;

                zgc.AxisChange();
                zgc.Refresh();
                //dicRegionValues[i] = addRegionValue(dsOut.Tables[i]);

                //int j = 0;
                //Dictionary<string, double> dicRegionOne = dicRegionValues[0];
                //while (j < dicRegionOne.Count)
                //{
                //    PointPairList list = new PointPairList();
                //    i = 0;
                //    while (i < irepeat)
                //    {
                //        Dictionary<string, double> dicRegioni = dicRegionValues[i];
                //        list.Add(new PointPair(Convert.ToDouble(j), dicRegioni.ToArray()[j].Value));

                //        i++;
                //    }
                //    j++;
                //}
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

            //统计区域的网格sum和avg
            i = 0;
            List<ModelAttribute> lsdt = new List<ModelAttribute>();
            //foreach (DataRow dr in dt.Rows)
            //{
            //    ModelAttribute ma = new ModelAttribute()
            //    {
            //        Col = Convert.ToInt32(dr[0]),
            //        Row = Convert.ToInt32(dr[1]),
            //        Values = Convert.ToDouble(dr[2])

            //    };
            //    lsdt.Add(ma);
            //}
            ////Model值统计网格平均值
            //while (i < fsregion.DataTable.Rows.Count)
            //{
            //    int fid = Convert.ToInt32(fsregion.DataTable.Rows[i]["RegionID"]);
            //    var rc = from c in CommonClass.ListRegionRowCol where c.Fid == fid select c;
            //    List<regionrowcol> lsrc = rc.ToList();
            //    double sumvalue = 0;
            //    double avgvalue = 0;
            //    int sumindex = 0;
            //    var test =
            //   from a in lsdt
            //   join
            //       b in lsrc on a.Col equals b.Col
            //   join c in lsrc on a.Row equals c.Row
            //   select a;
            //    sumvalue = test.ToList().Sum(a => a.Values);
            //    string statename = fsregion.DataTable.Rows[i]["STATE_NAME"].ToString();
            //    DicRegionValue.Add(statename, sumvalue);
            //    //sumindex = test.Count();

            //    //if (sumindex == 0) avgvalue = 0;
            //    //else avgvalue = sumvalue / Convert.ToDouble(sumindex);
            //    //mpRegionLayer.DataSet.DataTable.Rows[i][fsregion.DataTable.Columns.Count - 1] = avgvalue;
            //    //ModelRegionValue mrv = new ModelRegionValue()
            //    //{
            //    //    Regionid = fid,
            //    //    Value = avgvalue
            //    //};
            //    //CommonClass.ListBaseRegion.Add(mrv);
            //    i++;
            //}
            return DicRegionValue;
        }

        private void ZedGraphDemo(ZedGraphControl zgc)
        {
            //= new ZedGraphControl();
            GraphPane myPane = zgc.GraphPane;
            string[] str = { "North", "South", "West", "East", "Central" };

            // Set the title and axis labels
            myPane.Title.Text = "Vertical Bars with Value Labels Above Each Bar";
            myPane.XAxis.Title.Text = "Position Number";
            myPane.YAxis.Title.Text = "Some Random Thing";

            PointPairList list = new PointPairList();
            PointPairList list2 = new PointPairList();
            PointPairList list3 = new PointPairList();
            Random rand = new Random();

            // Generate random data for three curves

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

            // create the curves
            BarItem myCurve = myPane.AddBar("curve 1", list, Color.Blue);
            BarItem myCurve2 = myPane.AddBar("curve 2", list2, Color.Red);
            BarItem myCurve3 = myPane.AddBar("curve 3", list3, Color.Green);

            // Fill the axis background with a color gradient
            myPane.Chart.Fill = new Fill(Color.White,
                Color.FromArgb(255, 255, 166), 45.0F);

            // expand the range of the Y axis slightly to accommodate the labels
            //myPane.YAxis.Scale.Max += myPane.YAxis.Scale.MajorStep;
            myPane.XAxis.Scale.TextLabels = str;
            myPane.XAxis.Type = AxisType.Text;
            // Create TextObj's to provide labels for each bar
            BarItem.CreateBarLabels(myPane, false, "f0");
            // zgc.Controls.Add(myPane);
            //zgc.GraphPane = myPane;
            zgc.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;

            zgc.AxisChange();
            zgc.Refresh();

            //return zgc;
        }

        /// <summary>
        /// 在tabMain显示默认Cumulative Distribution
        /// </summary>
        private void ShowCumulative()
        {
            zedGraphCtl.Visible = false;
            System.Drawing.Image backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\Cumulative Distributions.JPG");
            pnlChart.BackgroundImage = backImg;
            tabCtlMain.SelectTab(tabChart);
        }

        /// <summary>
        /// 在tabMain显示默认BoxPlot
        /// </summary>
        private void ShowBoxPlot()
        {
            zedGraphCtl.Visible = false;
            System.Drawing.Image backImg = System.Drawing.Image.FromFile(Application.StartupPath + @"\Data\Image\BoxPlot.jpg");
            pnlChart.BackgroundImage = backImg;
            tabCtlMain.SelectTab(tabChart);
        }

        //private void btnProcessGenerate_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        //SaveFileDialog sfd = new SaveFileDialog();
        //        //sfd.Title = "Please select a path to save report:";
        //        //sfd.Filter = "CSV File(*.csv)|*.csv";
        //        //sfd.InitialDirectory = Application.StartupPath + @"\Data";
        //        //sfd.RestoreDirectory = false;

        //        //if (sfd.ShowDialog() != DialogResult.OK)
        //        //{ return; }

        //        //DataWorker.DataSaver dSaver = new DataWorker.DataSaver();
        //        //bool saveOK = dSaver.Create2CSV(ds,"Valuation_Result",true,strPath);
        //        //bool saveOK = ExportDataset2CSV(ds, strPath);

        //        //XmlDocument xmlDoc = new XmlDocument();
        //        //xmlDoc.Load(Application.StartupPath + @"\Data\USAData\ValuationResult.csv");
        //        //xmlDoc.Save(strPath);
        //        //if (!saveOK) return;

        //        //MessageBox.Show("Successfully saved!", "Tip", MessageBoxButtons.OK);

        //        //if (lvwProcessForm.FocusedItem == null)
        //        //{
        //        //    MessageBox.Show("Please select a report form.", "Select Form", MessageBoxButtons.OK);
        //        //}
        //        //else
        //        //{
        //        //    switch (lvwProcessForm.FocusedItem.Text.ToString())
        //        //    {
        //        //        case "Table":
        //        //            //ColumnSetting colSettingFrm = new ColumnSetting();
        //        //            //DialogResult rtn = colSettingFrm.ShowDialog();
        //        //            //if (rtn != DialogResult.OK)
        //        //            //{ return; }
        //        //            //else
        //        //            //{
        //        //            //    ShowTable();
        //        //            //}
        //        //            //if (CommonClass.ValuationResultPath == string.Empty || CommonClass.ValuationResultPath == null)
        //        //            //{
        //        //            //    MessageBox.Show("Please finish APVX process first!");
        //        //            //}
        //        //            //else
        //        //            //{
        //        //            //    string strPath = CommonClass.ValuationResultPath.ToString(); // sfd.FileName;
        //        //            //    //把保存在默认文件夹下的ValuationResult.csv复制一份到用户指定路径
        //        //            //    DataWorker.DataParser dParser = new DataWorker.DataParser();
        //        //            //    System.Data.DataSet ds = dParser.ReadCSV2DataSet(strPath, "Valuation_Result") as System.Data.DataSet;

        //        //            //    ShowTable(CommonClass.ValuationResultPath.ToString());
        //        //            //}
        //        //            break;
        //        //        case "Box Plot":
        //        //            ShowBoxPlot();
        //        //            break;
        //        //        case "Cumulative Distribution":
        //        //            ShowCumulative();
        //        //            break;
        //        //        case "Bar Chart":
        //        //            //if (CommonClass.ValuationResultPath == string.Empty || CommonClass.ValuationResultPath == null)
        //        //            //{
        //        //            //    MessageBox.Show("Please finish APVX process first!");
        //        //            //}
        //        //            //else
        //        //            //{
        //        //            //    string strPath = CommonClass.ValuationResultPath.ToString(); // sfd.FileName;

        //        //            //    //把保存在默认文件夹下的ValuationResult.csv复制一份到用户指定路径
        //        //            //    DataWorker.DataParser dParser = new DataWorker.DataParser();
        //        //            //    System.Data.DataSet ds = dParser.ReadCSV2DataSet(strPath, "Valuation_Result") as System.Data.DataSet;
        //        //            //    ShowChart(CommonClass.ValuationResultPath.ToString());
        //        //            //}

        //        //            break;
        //        //        default:
        //        //            break;
        //        //    }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex);
        //    }
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool ExportDataset2CSV(System.Data.DataSet ds, string fileName)
        {
            try
            {
                StreamWriter swriter = new StreamWriter(fileName, false, Encoding.Default);
                //表头
                string str = string.Empty;
                str = string.Format("Column,Row,ValuationResult");
                swriter.WriteLine(str);

                //----------------按网格的顺序存---------------
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

                //-----------------------------------------------

                swriter.Flush();//将缓冲区的数据写入流
                swriter.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }

        #endregion ListView设置

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// 将DataTable中数据写入到CSV文件中
        /// </summary>
        /// <param name="dt">提供保存数据的DataTable</param>
        /// <param name="fileName">CSV的文件路径</param>
        //public void CommonClass.SaveCSV(DataTable dt, string fileName)
        //{
        //    FileStream fs = new FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
        //    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
        //    string data = "";

        //    //写出列名称
        //    for (int i = 0; i < dt.Columns.Count; i++)
        //    {
        //        data += dt.Columns[i].ColumnName.ToString();
        //        if (i < dt.Columns.Count - 1)
        //        {
        //            data += ",";
        //        }
        //    }
        //    sw.WriteLine(data);

        //    //写出各行数据
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        data = "";

        //        for (int j = 0; j < dt.Columns.Count; j++)
        //        {
        //            if (dt.Rows[i][j].ToString().Contains(","))
        //            {
        //                data += "\"" + dt.Rows[i][j].ToString() + "\"";
        //            }
        //            else
        //                data += dt.Rows[i][j].ToString();
        //            if (j < dt.Columns.Count - 1)
        //            {
        //                data += ",";
        //            }
        //        }
        //        sw.WriteLine(data);
        //    }

        //    sw.Close();
        //    fs.Close();
        //    MessageBox.Show("Success in saving CSV file! ");
        //}
        public string _outputFileName;
        /// <summary>
        /// 输出表格形式的报告,保存为csv文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        if (_tableObject is CRSelectFunctionCalculateValue || _tableObject is Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>||_tableObject is List<CRSelectFunctionCalculateValue>)
                            saveFileDialog1.InitialDirectory = CommonClass.ResultFilePath + @"\Result\CFGR";
                        else if (_tableObject is BenMAPLine)
                            saveFileDialog1.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG";
                        else if (_tableObject is List<AllSelectValuationMethodAndValue> || _tableObject is AllSelectValuationMethodAndValue)
                            saveFileDialog1.InitialDirectory = CommonClass.ResultFilePath + @"\Result\APVR";
                        else if(_tableObject is List<AllSelectCRFunction>)
                            saveFileDialog1.InitialDirectory = CommonClass.ResultFilePath + @"\Result\APVR";

                        if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                        {
                            return;
                        }

                        _outputFileName = saveFileDialog1.FileName;
                    }
                    //-------首先生成一个DataTable---------
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
                                    //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
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
                        if (IncidencelstResult == null)
                        {
                            //BrightIdeasSoftware.OLVColumn olvColumnPointEstimate = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PointEstimate", AspectToStringFormat = "{0:N4}", Width = "PointEstimate".Length * 8, Text = "Point Estimate", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPointEstimate);
                            //BrightIdeasSoftware.OLVColumn olvColumnPopulation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Population", AspectToStringFormat = "{0:N4}", Text = "Population", Width = "Population".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPopulation);
                            ////BrightIdeasSoftware.OLVColumn olvColumnIncidence = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Incidence", AspectToStringFormat = "{0:N4}", Text = "Incidence", Width = "Incidence".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnIncidence);
                            //BrightIdeasSoftware.OLVColumn olvColumnDelta = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Delta", AspectToStringFormat = "{0:N4}", Text = "Delta", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnDelta);
                            //BrightIdeasSoftware.OLVColumn olvColumnMean = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Mean", AspectToStringFormat = "{0:N4}", Text = "Mean", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnMean);
                            //BrightIdeasSoftware.OLVColumn olvColumnBaseline = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Baseline", AspectToStringFormat = "{0:N4}", Text = "Baseline", Width = "Baseline2".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnBaseline);
                            //BrightIdeasSoftware.OLVColumn olvColumnPercentOfBaseline = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PercentOfBaseline", AspectToStringFormat = "{0:N4}", Width = "Percent Of Baseline".Length * 8, Text = "Percent Of Baseline", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPercentOfBaseline);
                            //BrightIdeasSoftware.OLVColumn olvColumnStandardDeviation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.StandardDeviation", AspectToStringFormat = "{0:N4}", Width = "Standard Deviation".Length * 8, Text = "Standard Deviation", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnStandardDeviation);
                            //BrightIdeasSoftware.OLVColumn olvColumnVariance = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Variance", Text = "Variance", AspectToStringFormat = "{0:N4}", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnVariance);
                            dt.Columns.Add("Point Estimate", typeof(double));
                            dt.Columns.Add("Population", typeof(double));
                            dt.Columns.Add("Delta", typeof(double));
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
                                //Percentile +  i* count/100[" + i + "]

                                dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()))))), typeof(double));//"Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), typeof(double));// "LstPercentile[" + i + "]", Text = "Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

                                i++;
                            }
                        }
                        //if (lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile != null)
                        //{
                        //    if (strPoolIncidencePercentiles != null && strPoolIncidencePercentiles.Count > 0)
                        //    {
                        //        double interval = -1;
                        //        switch (CommonClass.CRLatinHypercubePoints)
                        //        {
                        //            case 10:
                        //                interval = 5;
                        //                break;
                        //            case 20:
                        //                interval = 2.5;
                        //                break;
                        //            case 50:
                        //                interval = 1;
                        //                break;
                        //            case 100:
                        //                interval = 0.5;
                        //                break;
                        //        }
                        //        i = 0;
                        //        while (i < strPoolIncidencePercentiles.Count)
                        //        {
                        //            BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + (int)(Convert.ToDouble(strPoolIncidencePercentiles[i]) / interval - 1) / 2 + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + strPoolIncidencePercentiles[i].ToString(), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
                        //            i++;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        i = 0;
                        //        while (i < lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count())
                        //        {
                        //            //Percentile +  i* count/100[" + i + "]
                        //            if (IncidencelstResult == null || IncidencelstResult.Last().isChecked)
                        //            {
                        //                BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + i + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()))))), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
                        //            }
                        //            i++;
                        //        }
                        //    }
                        //}
                        //Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction> dicAPV = new Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>();// new Dictionary<CRCalculateValue, CRSelectFunction>();
                        //int iLstCRTable = 0;
                        //Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();

                        foreach (AllSelectCRFunction cr in lstAllSelectCRFuntion)
                        {
                            //foreach (CRCalculateValue crv in cr.CRSelectFunctionCalculateValue.CRCalculateValues)
                            //{
                            //    dicKey = null;
                            //    dicKey = new Dictionary<CRCalculateValue, int>();
                            //    dicKey.Add(crv, iLstCRTable);
                            //    dicAPV.Add(dicKey.ToList()[0], cr);
                            //}
                            //iLstCRTable++;
                            foreach (CRCalculateValue crcv in cr.CRSelectFunctionCalculateValue.CRCalculateValues)
                            {
                                DataRow dr = dt.NewRow();
                                if(dt.Columns.Contains("Col"))  dr["Col"] = crcv.Col;
                                if (dt.Columns.Contains("Row")) dr["Row"] = crcv.Row;
                                if (IncidencelstHealth != null)
                                {
                                    foreach (FieldCheck fieldCheck in IncidencelstHealth)
                                    {
                                        if (fieldCheck.FieldName.ToLower() == "version" && fieldCheck.isChecked)
                                        {
                                            dr["Version"]= cr.Version;

                                        }
                                        else if (fieldCheck.isChecked)
                                        {
                                            dr[fieldCheck.FieldName]=getFieldNameFromlstHealthObject(fieldCheck.FieldName,crcv,cr.CRSelectFunctionCalculateValue.CRSelectFunction);
                                        }
                                    }
                                }
                                if (dt.Columns.Contains("Point Estimate")) dr["Point Estimate"] = crcv.PointEstimate;
                                if (dt.Columns.Contains("Population")) dr["Population"] = crcv.Population;
                                //dr["Incidence"] = crcv.Incidence;
                                if (dt.Columns.Contains("Delta")) dr["Delta"] = crcv.Delta;
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
                                        //Percentile +  i* count/100[" + i + "]

                                        dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(cr.CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count())))))] = crcv.LstPercentile[i];// "LstPercentile[" + i + "]", Text = "Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

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
                        //first 50 init columns
                        CRSelectFunctionCalculateValue crTable = (CRSelectFunctionCalculateValue)_tableObject;
                        dt.Columns.Add("Col", typeof(int));
                        dt.Columns.Add("Row", typeof(int));
                        dt.Columns.Add("Point Estimate", typeof(double));
                        dt.Columns.Add("Population", typeof(double));
                        //dt.Columns.Add("Incidence", typeof(double));
                        dt.Columns.Add("Delta", typeof(double));
                        dt.Columns.Add("Mean", typeof(double));
                        dt.Columns.Add("Baseline", typeof(double));
                        dt.Columns.Add("Percent Of Baseline", typeof(double));
                        dt.Columns.Add("Standard Deviation", typeof(double));
                        dt.Columns.Add("Variance", typeof(double));
                        i = 0;
                        while (i < crTable.CRCalculateValues.First().LstPercentile.Count())
                        {
                            //Percentile +  i* count/100[" + i + "]

                            dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()))))), typeof(double));//"Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), typeof(double));// "LstPercentile[" + i + "]", Text = "Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

                            i++;
                        }

                        foreach (CRCalculateValue crcv in crTable.CRCalculateValues)
                        {
                            DataRow dr = dt.NewRow();
                            dr["Col"] = crcv.Col;
                            dr["Row"] = crcv.Row;
                            dr["Point Estimate"] = crcv.PointEstimate;
                            dr["Population"] = crcv.Population;
                            //dr["Incidence"] = crcv.Incidence;
                            dr["Delta"] = crcv.Delta;
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
                                    //Percentile +  i* count/100[" + i + "]

                                    dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count())))))] = crcv.LstPercentile[i];// "LstPercentile[" + i + "]", Text = "Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

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
                                    //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
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
                                    //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                }
                            }
                        }
                        if (cflstResult == null)
                        {

                            dt.Columns.Add("Point Estimate", typeof(double));
                            dt.Columns.Add("Population", typeof(double));
                            //dt.Columns.Add("Incidence", typeof(double));
                            dt.Columns.Add("Delta", typeof(double));
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
                                    //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                }
                            }
                        }
                        if (dicAPV.First().Key.Key.LstPercentile != null)
                        {
                            i = 0;
                            while (i < dicAPV.First().Key.Key.LstPercentile.Count())
                            {
                                //Percentile +  i* count/100[" + i + "]
                                if (cflstResult == null || cflstResult.Last().isChecked)
                                {
                                    dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(dicAPV.First().Key.Key.LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(dicAPV.First().Key.Key.LstPercentile.Count()))))), typeof(double));
                                    // BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.LstPercentile[" + i + "]", Width = "Percentile100".Length * 8, Text = "Percentile" + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()))))), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
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
                                        //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
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
                                        //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                    }
                                }
                            }
                            if (cflstResult == null)
                            {

                                dr["Point Estimate"] = crcv.PointEstimate;
                                dr["Population"] = crcv.Population;
                                //dr["Incidence"] = crcv.Incidence;
                                dr["Delta"] = crcv.Delta;
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
                                        //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                    }
                                }
                            }


                            i = 0;
                            if ((cflstResult == null || cflstResult.Last().isChecked) && crcv.LstPercentile != null)
                            {
                                while (i < crcv.LstPercentile.Count())
                                {
                                    //Percentile +  i* count/100[" + i + "]

                                    dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crcv.LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(crcv.LstPercentile.Count())))))] = crcv.LstPercentile[i];// "LstPercentile[" + i + "]", Text = "Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

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
                        //CRSelectFunctionCalculateValue crTable = (CRSelectFunctionCalculateValue)_tableObject;
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
                                        //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
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
                                        //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                    }
                                }
                            }
                            if (IncidencelstResult == null)
                            {

                                dt.Columns.Add("Point Estimate", typeof(double));
                                dt.Columns.Add("Population", typeof(double));
                                //dt.Columns.Add("Incidence", typeof(double));
                                dt.Columns.Add("Delta", typeof(double));
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
                                        //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                    }
                                }
                            }

                            if (lstCRTable.First().CRCalculateValues.First().LstPercentile != null)
                            {
                                i = 0;
                                while (i < lstCRTable.First().CRCalculateValues.First().LstPercentile.Count())
                                {
                                    //Percentile +  i* count/100[" + i + "]
                                    if (IncidencelstResult == null || IncidencelstResult.Last().isChecked)
                                    {
                                        dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()))))), typeof(double));
                                        // BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.LstPercentile[" + i + "]", Width = "Percentile100".Length * 8, Text = "Percentile" + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()))))), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
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
                                        //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
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
                                        //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                    }
                                }
                            }
                            if (cflstResult == null)
                            {

                                dt.Columns.Add("Point Estimate", typeof(double));
                                dt.Columns.Add("Population", typeof(double));
                                //dt.Columns.Add("Incidence", typeof(double));
                                dt.Columns.Add("Delta", typeof(double));
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
                                        //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                    }
                                }
                            }

                            if (lstCRTable.First().CRCalculateValues.First().LstPercentile != null)
                            {
                                i = 0;
                                while (i < lstCRTable.First().CRCalculateValues.First().LstPercentile.Count())
                                {
                                    //Percentile +  i* count/100[" + i + "]
                                    if (cflstResult == null || cflstResult.Last().isChecked)
                                    {
                                        dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()))))), typeof(double));
                                        // BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.LstPercentile[" + i + "]", Width = "Percentile100".Length * 8, Text = "Percentile" + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()))))), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
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
                                    //---------得到Base------
                                    BenMAPLine benMAPLineBase = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == crNew.BenMAPHealthImpactFunction.Pollutant.PollutantID).First().Base;
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
                                    dicKey.Add(crv,dicAPV.Count);
                                    dicAPV.Add(dicKey.First(), crNew);
                                }
                            }
                            if (cflstResult.Where(p => p.FieldName == "Population Weighted Control").First().isChecked == true)
                            {
                                foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                                {
                                    
                                    CRCalculateValue crv = new CRCalculateValue();
                                    CRSelectFunction crNew = CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
                                    //---------得到Base------
                                    BenMAPLine benMAPLineControl = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == crNew.BenMAPHealthImpactFunction.Pollutant.PollutantID).First().Control;
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
                                    dicKey.Add(crv,dicAPV.Count);
                                    dicAPV.Add(dicKey.First(), crNew);
                                }
                            }
                        }

                        foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                        {
                            foreach (CRCalculateValue crcv in cr.CRCalculateValues)
                            {
                                Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();
                                    dicKey.Add(crcv,dicAPV.Count);
                                    dicAPV.Add(dicKey.First(), cr.CRSelectFunction);
                                
                            }
                        }
                        foreach (KeyValuePair<KeyValuePair<CRCalculateValue,int>,CRSelectFunction> k in dicAPV)
                        {
                            
                                DataRow dr = dt.NewRow();
                                if (this.tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Tag.ToString() == "incidence")
                                {
                                    if ( IncidencelstColumnRow == null)
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
                                                //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
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
                                                //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                            }
                                        }
                                    }
                                    if (IncidencelstResult == null)
                                    {

                                        dr["Point Estimate"] = k.Key.Key.PointEstimate;
                                        dr["Population"] = k.Key.Key.Population;
                                        //dr["Incidence"] = crcv.Incidence;
                                        dr["Delta"] = k.Key.Key.Delta;
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
                                                //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                            }
                                        }
                                    }


                                    i = 0;
                                    if ((IncidencelstResult == null || IncidencelstResult.Last().isChecked) && k.Key.Key.LstPercentile != null)
                                    {
                                        while (i < k.Key.Key.LstPercentile.Count())
                                        {
                                            //Percentile +  i* count/100[" + i + "]

                                            dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(k.Key.Key.LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(k.Key.Key.LstPercentile.Count())))))] = k.Key.Key.LstPercentile[i];// "LstPercentile[" + i + "]", Text = "Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

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
                                                //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
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
                                                //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                            }
                                        }
                                    }
                                    if (cflstResult == null)
                                    {

                                        dr["Point Estimate"] = k.Key.Key.PointEstimate;
                                        dr["Population"] = k.Key.Key.Population;
                                        //dr["Incidence"] = crcv.Incidence;
                                        dr["Delta"] = k.Key.Key.Delta;
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
                                                //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                            }
                                        }
                                    }


                                    i = 0;
                                    if ((cflstResult == null || cflstResult.Last().isChecked) && k.Key.Key.LstPercentile != null)
                                    {
                                        while (i < k.Key.Key.LstPercentile.Count())
                                        {
                                            //Percentile +  i* count/100[" + i + "]

                                            dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(k.Key.Key.LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(k.Key.Key.LstPercentile.Count())))))] = k.Key.Key.LstPercentile[i];// "LstPercentile[" + i + "]", Text = "Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

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
                        //first 50 init columns
                        BenMAPLine crTable = (BenMAPLine)_tableObject;
                        DataSourceCommonClass.SaveModelDataLineToNewFormatCSV(crTable, _outputFileName);
                    }
                    //else if (_tableObject is AllSelectValuationMethodAndValue)
                    //{
                    //    AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = (AllSelectValuationMethodAndValue)_tableObject;
                    //    dt.Columns.Add("Col", typeof(int));
                    //    dt.Columns.Add("Row", typeof(int));
                    //    dt.Columns.Add("PointEstimate", typeof(double));

                    //    dt.Columns.Add("Mean", typeof(double));

                    //    dt.Columns.Add("StandardDeviation", typeof(double));
                    //    dt.Columns.Add("Variance", typeof(double));

                    //    i = 0;
                    //    while (i < allSelectValuationMethodAndValue.lstAPVValueAttributes.First().LstPercentile.Count())
                    //    {
                    //        dt.Columns.Add("Percentile" + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(allSelectValuationMethodAndValue.lstAPVValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(allSelectValuationMethodAndValue.lstAPVValueAttributes.First().LstPercentile.Count()))))), typeof(double));// "LstPercentile[" + i + "]", Text = "Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

                    //        i++;
                    //    }

                    //    dt.Columns.Add("Name", typeof(string));
                    //    dt.Columns.Add("PoolingMethod", typeof(string));
                    //    dt.Columns.Add("Qualifier", typeof(string));
                    //    dt.Columns.Add("StartAge", typeof(string));
                    //    dt.Columns.Add("EndAge", typeof(string));
                    //    dt.Columns.Add("Function", typeof(string));

                    //    foreach (APVValueAttribute apvx in allSelectValuationMethodAndValue.lstAPVValueAttributes)
                    //    {
                    //        DataRow dr = dt.NewRow();
                    //        dr["Col"] = apvx.Col;
                    //        dr["Row"] = apvx.Row;
                    //        dr["PointEstimate"] = apvx.PointEstimate;

                    //        dr["Mean"] = apvx.Mean;

                    //        dr["StandardDeviation"] = apvx.StandardDeviation;
                    //        dr["Variance"] = apvx.Variance;

                    //        dr["Name"] = allSelectValuationMethodAndValue.AllSelectValuationMethod.Name;
                    //        dr["PoolingMethod"] = allSelectValuationMethodAndValue.AllSelectValuationMethod.PoolingMethod;
                    //        dr["Qualifier"] = allSelectValuationMethodAndValue.AllSelectValuationMethod.Qualifier;
                    //        dr["StartAge"] = allSelectValuationMethodAndValue.AllSelectValuationMethod.StartAge;
                    //        dr["EndAge"] = allSelectValuationMethodAndValue.AllSelectValuationMethod.EndAge;
                    //        dr["Function"] = allSelectValuationMethodAndValue.AllSelectValuationMethod.Function;

                    //        i = 0;
                    //        while (i < allSelectValuationMethodAndValue.lstAPVValueAttributes.First().LstPercentile.Count())
                    //        {
                    //            //Percentile +  i* count/100[" + i + "]

                    //            dr["Percentile" + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(allSelectValuationMethodAndValue.lstAPVValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(allSelectValuationMethodAndValue.lstAPVValueAttributes.First().LstPercentile.Count())))))] = apvx.LstPercentile[i];// "LstPercentile[" + i + "]", Text = "Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

                    //            i++;
                    //        }
                    //        dt.Rows.Add(dr);
                    //    }
                    //    CommonClass.SaveCSV(dt, fileName);

                    //    //-----------------------------------初始化table--------------------------------------------------------
                    //}
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
                        //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value", Text = "MethodID", Width = "MethodID".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
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
                                    //BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
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
                                    // BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
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
                                    // BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
                                }
                            }

                        }
                        if ((apvlstResult == null || apvlstResult.Last().isChecked) && lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile != null)
                        {
                            i = 0;
                            while (i < lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count())
                            {
                                //Percentile +  i* count/100[" + i + "]

                                dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count()))))), typeof(double));// "LstPercentile[" + i + "]", Text = "Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

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
                                            //BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
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
                                            // BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
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
                                            // BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
                                        }
                                    }

                                }
                                if ((apvlstResult == null || apvlstResult.Last().isChecked) && lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile != null)
                                {
                                    i = 0;
                                    while (i < lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count())
                                    {
                                        //Percentile +  i* count/100[" + i + "]
                                        dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.First().LstPercentile.Count())))))] = apvx.LstPercentile[i];// "LstPercentile[" + i + "]", Text = "Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

                                        i++;
                                    }
                                }


                                dt.Rows.Add(dr);
                            }
                        }
                        CommonClass.SaveCSV(dt, _outputFileName);

                        //-----------------------------------初始化table--------------------------------------------------------
                    }

                        //----------------------------------QALY---------------------------
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
                        //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value", Text = "MethodID", Width = "MethodID".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
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
                                    //BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
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
                                    // BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
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
                                    // BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
                                }
                            }

                        }
                        if ((qalylstResult == null || qalylstResult.Last().isChecked) && lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile != null)
                        {
                            i = 0;
                            while (i < lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count())
                            {
                                //Percentile +  i* count/100[" + i + "]

                                dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()))))), typeof(double));// "LstPercentile[" + i + "]", Text = "Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

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
                                            //BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
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
                                            // BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
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
                                            // BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length +2)* 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
                                        }
                                    }

                                }
                                if ((qalylstResult == null || qalylstResult.Last().isChecked) && lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile != null)
                                {
                                    i = 0;
                                    while (i < lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count())
                                    {
                                        //Percentile +  i* count/100[" + i + "]
                                        dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count())))))] = apvx.LstPercentile[i];// "LstPercentile[" + i + "]", Text = "Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

                                        i++;
                                    }
                                }


                                dt.Rows.Add(dr);
                            }
                        }
                        CommonClass.SaveCSV(dt, _outputFileName);

                        //-----------------------------------初始化table--------------------------------------------------------
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
                            dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count()))))), typeof(double));// "LstPercentile[" + i + "]", Text = "Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

                            i++;
                        }

                        dt.Columns.Add("Name", typeof(string));
                        dt.Columns.Add("Pooling Method", typeof(string));
                        dt.Columns.Add("Qualifier", typeof(string));
                        dt.Columns.Add("Start Age", typeof(string));
                        dt.Columns.Add("End Age", typeof(string));
                        //dt.Columns.Add("Function", typeof(string));


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
                            //dr["Function"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.Function;

                            i = 0;
                            while (i < allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count())
                            {
                                //Percentile +  i* count/100[" + i + "]

                                dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count())))))] = apvx.LstPercentile[i];// "LstPercentile[" + i + "]", Text = "Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

                                i++;
                            }
                            dt.Rows.Add(dr);

                        }
                        CommonClass.SaveCSV(dt, _outputFileName);

                        //-----------------------------------初始化table--------------------------------------------------------
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
                            dt.Columns.Add("Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()))))), typeof(double));// "LstPercentile[" + i + "]", Text = "Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

                            i++;
                        }

                        dt.Columns.Add("Name", typeof(string));
                        dt.Columns.Add("Pooling Method", typeof(string));
                        dt.Columns.Add("Qualifier", typeof(string));
                        dt.Columns.Add("Start Age", typeof(string));
                        dt.Columns.Add("End Age", typeof(string));
                        //dt.Columns.Add("Function", typeof(string));

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
                                //dr["Function"] = allSelectQALYMethodAndValue.AllSelectQALYMethod.Function;

                                i = 0;
                                while (i < allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count())
                                {
                                    //Percentile +  i* count/100[" + i + "]

                                    dr["Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count())))))] = apvx.LstPercentile[i];// "LstPercentile[" + i + "]", Text = "Percentile" + Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

                                    i++;
                                }
                                dt.Rows.Add(dr);

                            }
                        }
                        CommonClass.SaveCSV(dt, _outputFileName);

                        //-----------------------------------初始化table--------------------------------------------------------
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        /// <summary>
        /// 输出Chart形式的报告，并保存为图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        #region ListView的选择改变

        /// <summary>
        /// 根据传进来的listView控件，判断里面有没有选中的item。
        /// 选中的item，底色为兰色；没有选中的，底色none
        /// </summary>
        /// <param name="list">目标listView控件</param>
        private void ListviewSelectionChange(ListView list)
        {
            //if (list.SelectedItems.Count > 0)
            //{
            foreach (ListViewItem item in list.Items)
            {
                //选中的，背景为蓝色；没选中的，没有背景色
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

            //}
        }

        /// <summary>
        /// ListView:lvwRawData
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvwRawData_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            //if (e.IsSelected)
            //{
            //    lvwRawForm.Items.Clear();
            //    foreach (var item in _dicReports[e.Item.Tag.ToString()].Forms)
            //    {
            //        lvwRawForm.Items.Add(item, "item");
            //    }
            //}
            //ListViewItem lvi = lvwRawData.Items[0] as ListViewItem;

            //ListviewSelectionChange(lvwRawData);
        }

        private void lvwRawData_MouseClick(object sender, MouseEventArgs e)
        {
            //ListviewSelectionChange(lvwRawData);
        }

        /// <summary>
        /// ListView:lvwRawForm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvwRawForm_MouseClick(object sender, MouseEventArgs e)
        {
            //ListviewSelectionChange(lvwRawForm);
            //ListviewSelectionChange(lvwRawData);
        }

        /// <summary>
        /// ListView:lvwResultType
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvwResultType_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                //lvwProcessForm.Items.Clear();

                ////Form
                //foreach (var item in _dicReports[e.Item.Tag.ToString()].Forms)
                //{
                //    lvwProcessForm.Items.Add(item, "item");
                //}
            }
        }

        private void lvwResultType_MouseClick(object sender, MouseEventArgs e)
        {
            //ListviewSelectionChange(lvwResultType);
        }

        /// <summary>
        /// ListView:lvwProcessType
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvwProcessType_MouseClick(object sender, MouseEventArgs e)
        {
            //ListviewSelectionChange(lvwProcessType);
        }

        /// <summary>
        /// ListView:lvwProcessForm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvwProcessForm_MouseClick(object sender, MouseEventArgs e)
        {
            //ListviewSelectionChange(lvwProcessForm);
        }

        #endregion ListView的选择改变

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

        /// <summary>
        /// Audit Trail Report
        /// 暂时链接到OneStepSetup界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRawAudit_Click(object sender, EventArgs e)
        {
            //string layerName = mainMap.Layers[0];// axMap2.get_LayerName(0);
            //layerName = axMap2.MapState;
            IMapFeatureLayer ilayer = mainMap.Layers[0] as IMapFeatureLayer;
            string layerName = ilayer.LegendText;
            if (mainMap.Layers.Count == 0)
            {
                MessageBox.Show("Please set up necessary data.", "Error", MessageBoxButtons.OK);
                return;
            }
            //if (layerName.Contains("US_Region"))
            //{
            //    layerName = "USA";
            //    OneStepSetup oneFrm = new OneStepSetup("USA", layerName);
            //    oneFrm.ShowDialog();
            //}
            //else
            //{
            //    layerName = "China";
            //    OneStepSetup oneFrm = new OneStepSetup("China", layerName);
            //    oneFrm.ShowDialog();
            //}
        }



        #region 多线程显示tip窗口

        TipFormGIF waitMess = new TipFormGIF();//等待窗体
        bool sFlog = true;

        //--显示等待窗体
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

        //--新开辟一个线程调用
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

        //--关闭等待窗体
        public void WaitClose()
        {
            //同步到主线程上
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

        #endregion 多线程显示tip窗口

        private void lvwRawData_MouseLeave(object sender, EventArgs e)
        {
            //ListviewSelectionChange(lvwRawData);
        }

        private void lvwRawData_Click(object sender, EventArgs e)
        {
            //ListviewSelectionChange(lvwRawData);
        }

        private void lvwRawData_MouseUp(object sender, MouseEventArgs e)
        {
            //ListviewSelectionChange(lvwRawData);
        }
        /// <summary>
        /// Load事件------其中包括对所有Aggregation的初始化,RegionLayer的初始化,结果页面的初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BenMAP_Load(object sender, EventArgs e)
        {
            olvCRFunctionResult.EmptyListMsg = "After results are generated here, double-click the selected study to display map/data/chart below." + Environment.NewLine + " Ctrl- or shift-click to select multiple studies and then click \"Show result\" to display data for multiple studies.";
            //this.splitContainer2.Panel1.Hide();
            //splitContainer2.Panel1MinSize = 0;
            mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
            //splitContainer2.SplitterDistance = 0;
            //--------------AddFilefold
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

            //if (!File.Exists(CommonClass.DataFilePath + @"\BenMAP.ini"))
            //    File.Copy(Application.StartupPath + @"\Data\BenMAP.ini", CommonClass.DataFilePath + @"\BenMAP.ini");
            
            //resolve in installshield--do not need to copy file here--
            //if (!Directory.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles"))
            //{
            //    CopyDir(Application.StartupPath + @"\Data\Shapefiles", CommonClass.DataFilePath + @"\Data\Shapefiles");
            //}

            //string batchcfg = Application.StartupPath + @"\Data\Batch32\PM25_36km_AsthmaExacerbation.cfgx";
            //string batchcfgCOPY = CommonClass.DataFilePath + @"\PM25_36km_AsthmaExacerbation.cfgx";
            //if (File.Exists(batchcfg))
            //{
            //    if (!File.Exists(batchcfgCOPY))
            //        File.Copy(batchcfg, batchcfgCOPY);
            //    else
            //    {
            //        FileInfo f = new FileInfo(batchcfg);
            //        FileInfo fCOPY = new FileInfo(batchcfgCOPY);
            //        if (f.LastWriteTime > fCOPY.LastWriteTime)//文件的最后修改时间
            //        {
            //            File.Delete(batchcfgCOPY);
            //            File.Copy(batchcfg, batchcfgCOPY);
            //        }
            //    }
            //}

            //batchcfg = Application.StartupPath + @"\Data\Batch32\PM25_36km_AsthmaExacerbation_aggregation_state.apvx";
            //batchcfgCOPY = CommonClass.DataFilePath + @"\PM25_36km_AsthmaExacerbation_aggregation_state.apvx";
            //if (File.Exists(batchcfg))
            //{
            //    if (!File.Exists(batchcfgCOPY))
            //        File.Copy(batchcfg, batchcfgCOPY);
            //    else
            //    {
            //        FileInfo f = new FileInfo(batchcfg);
            //        FileInfo fCOPY = new FileInfo(batchcfgCOPY);
            //        if (f.LastWriteTime > fCOPY.LastWriteTime)
            //        {
            //            File.Delete(batchcfgCOPY);
            //            File.Copy(batchcfg, batchcfgCOPY);
            //        }
            //    }
            //}
            //if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + @"\Result\Reports"))
            //    System.IO.Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + @"\Result\Reports");

            //trvSetting.Height = 30;
            //-------------------------------
            //bindingNavigatorAddNewItem.Enabled = false;
            bindingNavigatorCountItem.Enabled = true;
            bindingNavigatorMoveFirstItem.Enabled = true;
            bindingNavigatorMoveNextItem.Enabled = true;
            bindingNavigatorMoveLastItem.Enabled = true;
            bindingNavigatorMovePreviousItem.Enabled = true;
            bindingNavigatorPositionItem.Enabled = true;
            //-------------把前面两个删掉------------
            //if (tabCtlReport.TabPages.Count == 7)
            //{
            //    tabCtlReport.TabPages.RemoveAt(0);
            //    tabCtlReport.TabPages.RemoveAt(0);
            //    tabCtlReport.TabPages.RemoveAt(tabCtlReport.TabPages.Count - 2);
            //    //tabCtlReport.TabPages[3].Parent = null;
            //}
            //ToolTip ttTip = new ToolTip();
            //ttTip.SetToolTip(tsbSavePic, "Hello");//例如显示在textBox1的正下方toolTip1.Show("Hello", textBox1, 0/*textBox1.Width*/,textBox1.Height);
            //------------把Audit Report先删掉-------------
            // tabCtlReport.TabPages.RemoveAt(tabCtlReport.TabPages.Count - 1);
            colorBlend.CustomizeValueRange -= ResetGisMap;
            colorBlend.CustomizeValueRange += ResetGisMap;
            //if (CommonClass.MainSetup != null && CommonClass.MainSetup.SetupID == 1)
            //{
            //    CommonClass.IncidencePoolingAndAggregationAdvance = new IncidencePoolingAndAggregationAdvance()
            //    {
            //        IncidenceAggregation = CommonClass.MainSetup != null && CommonClass.MainSetup.SetupID == 1 ? Grid.GridCommon.getBenMAPGridFromID(2) : null,
            //        ValuationAggregation = CommonClass.MainSetup != null && CommonClass.MainSetup.SetupID == 1 ? Grid.GridCommon.getBenMAPGridFromID(2) : null,
            //        QALYAggregation = CommonClass.MainSetup != null && CommonClass.MainSetup.SetupID == 1 ? Grid.GridCommon.getBenMAPGridFromID(2) : null
            //    };
            //}
            InitAggregationAndRegionList();
            //--------
            Dictionary<string, string> dicSeasonStaticsAll = DataSourceCommonClass.DicSeasonStaticsAll;
            InitColumnsShowSet();
            isLoad = true;
        }

        /// <summary>
        /// 将整个文件夹复制到目标文件夹中。
        /// </summary>
        /// <param name="srcPath">源文件夹</param>
        /// <param name="aimPath">目标文件夹</param>
        /// <returns></returns>
        public void CopyDir(string srcPath, string aimPath)
        {
            try
            {
                // 检查目标目录是否以目录分割字符结束如果不是则添加之
                if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
                    aimPath += Path.DirectorySeparatorChar;

                // 判断目标目录是否存在如果不存在则新建之
                if (!Directory.Exists(aimPath))
                    Directory.CreateDirectory(aimPath);

                // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                // 如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法

                string[] fileList = Directory.GetFileSystemEntries(srcPath);

                // 遍历所有的文件和目录
                foreach (string file in fileList)
                {
                    // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                    if (Directory.Exists(file))
                    {
                        CopyDir(file, aimPath + Path.GetFileName(file));
                    }
                    // 否则直接Copy文件
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
            //------------------初始化Aggregation列表
            try
            {
                System.Data.DataSet dsCRAggregationGridType = BindGridtype();
                cbCRAggregation.DataSource = dsCRAggregationGridType.Tables[0];
                cbCRAggregation.DisplayMember = "GridDefinitionName";
                cbCRAggregation.SelectedIndex = 0;
                //System.Data.DataSet dsAPVAggregationGridType = BindGridtype();
                //cbAPVAggregation.DataSource = dsAPVAggregationGridType.Tables[0];
                //cbAPVAggregation.DisplayMember = "GridDefinitionName";
                //cbAPVAggregation.SelectedIndex = 0;
                //System.Data.DataSet dsQALYAggregationGridType = BindGridtype();
                //cbQALYAggregation.DataSource = dsQALYAggregationGridType.Tables[0];
                //cbQALYAggregation.DisplayMember = "GridDefinitionName";
                //cbQALYAggregation.SelectedIndex = 0;
                FireBirdHelperBase fb = new ESILFireBirdHelper();
                string commandText = string.Format("select distinct GridDefinitionID,GridDefinitionName from GridDefinitions where columns<=56 and setupid={0}  order by GridDefinitionName desc", CommonClass.MainSetup.SetupID);
                System.Data.DataSet dsRegion = BindGridtypeDomain();// fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                cboRegion.DisplayMember = "GridDefinitionName";
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
                    //-------------------------------
                    //----一行行读取ctlx分类到一个数组-----首先设计出每种AQG,CFG,APV三种不同的ctlx对象,然后根据读出来的文档到所有对象

                    //----一个一个的Run,Run后填入日志

                    //----最后output日志到同一目录的和ctlx同一文件名 *.log中
                }
                else if (CommonClass.InputParams != null && CommonClass.InputParams.Length > 0 && CommonClass.InputParams[0].ToLower().IndexOf("projx") > 0)
                {
                    //-------load project--------------
                    splitContainer1.Visible = true;
                    CommonClass.ClearAllObject();
                    //清空上次内容
                    ClearAll();
                    //初始化treeView，装载treeView控件根节点（必须数据）
                    //InitTreeView(trvSetting);
                    ResetParamsTree("");
                    //全面清空CommonClass
                    //CommonClass.ManageSetup = null;
                    //CommonClass.MainSetup = null;// 当前活动区域

                    ClearMapTableChart();
                    initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);

                    initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
                    initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
                    CommonClass.IncidencePoolingAndAggregationAdvance = null;
                    if (CommonClass.IncidencePoolingAndAggregationAdvance != null)
                    {
                        changeNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);

                    }
                    ////Todo:设置灰色 陈志润
                    //tabCRFunctionResultGISShow.BackColor = Color.Gray;
                    //tabAPVResultGISShow.BackColor = Color.Gray;
                    //tabQLAYResultShow.BackColor = Color.Gray;
                    //tabAuditTrialReport.BackColor = Color.Gray;
                    olvCRFunctionResult.SetObjects(null);
                    olvIncidence.SetObjects(null);
                    tlvAPVResult.SetObjects(null);
                    //tlvQALYResult.SetObjects(null);

                    cbPoolingWindowIncidence.Items.Clear();
                    cbPoolingWindowAPV.Items.Clear();
                    //cbPoolingWindowQALY.Items.Clear();
                    ClearMapTableChart();
                    //Todo:陈志润
                    SetTabControl(tabCtlReport);
                    CommonClass.LstPollutant = null;//污染物列表
                    CommonClass.RBenMAPGrid = null;//系统选择的Grid

                    CommonClass.GBenMAPGrid = null;//系统选择的Region
                    CommonClass.LstBaseControlGroup = null;//系统设置的DataSource Base and Control
                    //public  List<CRSelectFunction> LstCRSelectFunction;//系统选择的所有Health Impact Function
                    CommonClass.CRThreshold = 0;//阈值
                    CommonClass.CRLatinHypercubePoints = 10;//拉丁立体方采样点数
                    CommonClass.CRRunInPointMode = false;//是否使用拉丁立体方方采样

                    CommonClass.BenMAPPopulation = null;//系统设置的Population

                    CommonClass.BaseControlCRSelectFunction = null;//所有BaseControlAndCRSelectFunciton
                    CommonClass.BaseControlCRSelectFunctionCalculateValue = null;//所有BaseControlAndCRSelectFunciton以及Value

                    //-------------------APVX-------------------------------
                    //CommonClass.IncidencePoolingAndAggregationAdvance = null;//Advance
                    CommonClass.lstIncidencePoolingAndAggregation = null;
                    //public  List<ValuationMethodPoolingAndAggregation> lstValuationMethodPoolingAndAggregation;
                    //public  IncidencePoolingAndAggregation IncidencePoolingAndAggregation;//IncidencePooling;

                    CommonClass.IncidencePoolingResult = null;
                    CommonClass.ValuationMethodPoolingAndAggregation = null;
                    //GC.Collect();
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
                        //cbPoolingWindowQALY.Items.Clear();
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

                                //CommonClass.LstPollutant = CommonClass.BaseControlCRSelectFunction.BaseControlGroup.Select(pa => pa.Pollutant).ToList();
                                //CommonClass.lst
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            Logger.LogError(ex);
                        }
                        //-------------modify by xiejp因为project过于大，现只保存类似APVX的设置去掉APVX,QALY,CR的显示，并把它置红----------------------------------

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
                        //--------------变绿并且加节点----------------------------------------------------------------------
                        // TreeNode tr = currentNode.Parent;
                        foreach (TreeNode trchild in trvSetting.Nodes)
                        {
                            if (trchild.Name == "airqualitygridgroup")
                            {
                                nodesCount = trchild.Nodes.Count;

                                //currentNode.Tag = CommonClass.LstPollutant;

                                for (int i = nodesCount - 1; i > -1; i--)
                                {
                                    TreeNode node = trchild.Nodes[i];
                                    if (trchild.Nodes[i].Name == "datasource") { trchild.Nodes.RemoveAt(i); }
                                }
                                // CommonClass.LstBaseControlGroup = new List<BaseControlGroup>(CommonClass.LstPollutant.Count);
                                // for (int i = 0; i < CommonClass.LstPollutant.Count; i++)
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
                                //tlvQALYResult.SetObjects(null);
                                cbPoolingWindowIncidence.Items.Clear();
                                cbPoolingWindowAPV.Items.Clear();
                                //cbPoolingWindowQALY.Items.Clear();
                                //----------变黄-----------
                                ClearMapTableChart();
                                initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 3].Nodes.Count - 1]);

                                initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes.Count - 1]);
                                initNodeImage(trvSetting.Nodes[trvSetting.Nodes.Count - 1].Nodes[0]);
                                return;
                            }

                            trvSetting.Nodes["pollutant"].Parent.ExpandAll();
                        }
                    }
                    //------------modify by xiejp ---Population和Aggregation可以单独出现---需要分别变绿---
                    if (CommonClass.BenMAPPopulation != null)
                    {
                        changeNodeImage(trvSetting.Nodes[1].Nodes[0]);
                    }
                    if (CommonClass.IncidencePoolingAndAggregationAdvance != null)
                    {
                        changeNodeImage(trvSetting.Nodes[2].Nodes[0]);
                    }


                }
            }
            catch (Exception ex)
            {

            }
        }
        private bool isLoad = false;//是否已登陆
        /// <summary>
        /// 用来绑定Aggregation的Combox
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 用来绑定Aggregation的Combox
        /// </summary>
        /// <returns></returns>
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
        #region MapWindow控件功能
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbSavePic_Click(object sender, EventArgs e)
        {
            try
            {
                //Thread.Sleep(new TimeSpan(0, 0, 2));
                string s = tsbSavePic.ToolTipText;
                tsbSavePic.ToolTipText = "";
                Image i = new Bitmap(mainMap.Width, mainMap.Height);
                Graphics g = Graphics.FromImage(i);
                tsbSavePic.ToolTipText = s;
                g.CopyFromScreen(this.PointToScreen(new Point(splitContainer1.Width - splitContainer2.Panel2.Width - 6, this.tabCtlMain.Parent.Location.Y + 27)), new Point(0, 0), new Size(this.Width, this.Height));

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

        private void saveFileDialog1_Disposed(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 改变坐标系--现只支持美国区和中国区
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbChangeProjection_Click(object sender, EventArgs e)
        {
            try
            {
                if (mainMap.Layers.Count == 0) return;
                if (CommonClass.MainSetup.SetupName.ToLower() == "china")
                {
                    if (mainMap.Projection != DotSpatial.Projections.KnownCoordinateSystems.Projected.Asia.AsiaLambertConformalConic)
                    {
                        mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Projected.Asia.AsiaLambertConformalConic;
                        //mainMap.Projection.LatitudeOfOrigin = 34;
                        //mainMap.Projection.LongitudeOfCenter = 110;
                        foreach (FeatureLayer layer in mainMap.Layers)
                        {
                            //if (layer.Projection == null || (layer.Projection as DotSpatial.Projections.ProjDescriptor).ToString() == "Nothing.")
                            layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                            layer.Reproject(mainMap.Projection);
                        }
                        tsbChangeProjection.Text = "change projection to GCS/NAD 83";
                    }
                    else
                    {
                        mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                        foreach (FeatureLayer layer in mainMap.Layers)
                        {
                            //if (layer.Projection == null || (layer.Projection as DotSpatial.Projections.ProjDescriptor).ToString() == "Nothing.")
                            layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Projected.Asia.AsiaLambertConformalConic;
                            layer.Reproject(mainMap.Projection);
                        }
                        tsbChangeProjection.Text = "change projection to Albers";
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
                    return;
                }


                if (mainMap.Projection != DotSpatial.Projections.KnownCoordinateSystems.Projected.NorthAmerica.USAContiguousLambertConformalConic)
                {
                    mainMap.Projection = DotSpatial.Projections.KnownCoordinateSystems.Projected.NorthAmerica.USAContiguousLambertConformalConic;
                    foreach (FeatureLayer layer in mainMap.Layers)
                    {
                        //if (layer.Projection == null || (layer.Projection as DotSpatial.Projections.ProjDescriptor).ToString() == "Nothing.")
                        layer.Projection = DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984;
                        layer.Reproject(mainMap.Projection);
                    }
                    tsbChangeProjection.Text = "change projection to GCS/NAD 83";
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
                    tsbChangeProjection.Text = "change projection to Albers";
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

        /// <summary>
        /// 加图层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbAddLayer_Click(object sender, EventArgs e)
        {
            mainMap.AddLayer();


            //----------------change 36km------------------
            //FeatureLayer fl = mainMap.Layers[mainMap.Layers.Count - 1] as FeatureLayer;

            //int i = 0;
            //fl.DataSet.DataTable.Rows[fl.DataSet.DataTable.Rows.Count - 1]["Row"] = 45;
            ////fl.DataSet.DataTable.Columns[fl.DataSet.DataTable.Columns.Count - 1].DataType = typeof(Int32);
            //FeatureSet fsOut = new FeatureSet();
            //foreach (DataColumn dc in fl.DataSet.DataTable.Columns)
            //{
            //    fsOut.DataTable.Columns.Add(dc.ColumnName);
            //}
            //fsOut.DataTable.Columns["Row"].DataType = typeof(int);
            //fsOut.DataTable.Columns["Col"].DataType = typeof(int);

            //foreach (Feature f in fl.DataSet.Features)
            //{
            //    //------------如果是Row=1 => Row=98 ;其他Row=Row-1
            //    //if (f.DataRow["Row"].ToString() == "1")
            //    //{
            //    //    fl.DataSet.DataTable.Rows[i]["Row"] = 98;
            //    //}
            //    //else
            //    //    fl.DataSet.DataTable.Rows[i]["Row"] = Convert.ToInt16(f.DataRow["Row"]) - 1; ;
            //    fsOut.Features.Add(f.BasicGeometry);
            //    foreach (DataColumn dc in fl.DataSet.DataTable.Columns)
            //    {
            //        if (dc.ColumnName != "Row" && dc.ColumnName != "Col")
            //            fsOut.DataTable.Rows[fsOut.DataTable.Rows.Count - 1][dc.ColumnName] = f.DataRow[dc.ColumnName].ToString().Trim();
            //        else
            //            fsOut.DataTable.Rows[fsOut.DataTable.Rows.Count - 1][dc.ColumnName] = Convert.ToInt32(f.DataRow[dc.ColumnName].ToString());
            //    }
            //    //fl.DataSet.DataTable.Rows[fl.DataSet.DataTable.Rows.Count - 1]["Row"] = Convert.ToInt32(fl.DataSet.DataTable.Rows[fl.DataSet.DataTable.Rows.Count - 1]["Row"].ToString());
            //    i++;
            //}
            //fsOut.Projection = fl.Projection;

            //fsOut.SaveAs(@"D:\County_epa2.shp", true);
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
                tsbChangeProjection_Click(sender, e);
                tsbChangeProjection_Click(sender, e);

                string fileName = saveFileDialog1.FileName;

                FeatureLayer fl = mainMap.Layers[0] as FeatureLayer;
                fl.DataSet.SaveAs(fileName, true);
                MessageBox.Show("Shapefile saved.", "File saved");
            }
            catch
            {
            }
        }
        /// <summary>
        /// 未用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbChangeCone_Click(object sender, EventArgs e)
        {
            //
            if (mainMap.Layers.Count < 2)
                return;
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
        /// <summary>
        /// Identify
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIdentify_Click(object sender, EventArgs e)
        {
            mainMap.FunctionMode = FunctionMode.Info;
        }
        /// <summary>
        /// 根据状态是否打开Legend设置面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLayerSet_Click(object sender, EventArgs e)
        {
            if (isLegendHide)
            {
                if (_currentNode == "grid" || _currentNode == "region") { return; }
                this.splitContainer2.BorderStyle = BorderStyle.FixedSingle;
                this.splitContainer2.Panel1.Show();
                //Todo:陈志润 20111124修改
                splitContainer2.SplitterDistance = 204;
                isLegendHide = false;
            }
            else
            {
                Extent et = mainMap.Extent;
                splitContainer2.Panel1.Hide();
                // mainMap.ZoomToMaxExtent();
                //mainMap.ZoomToPrevious();
                //mainMap.ZoomToPrevious();
                splitContainer2.SplitterDistance = 0;
                this.splitContainer2.BorderStyle = BorderStyle.None;
                isLegendHide = true;
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
        }
        /// <summary>
        /// 生成饼状图，柱状图图层
        /// </summary>
        /// <param name="iValue"></param>
        /// <param name="MinValue"></param>
        /// <param name="MaxValue"></param>
        /// <returns></returns>
        private FeatureSet getThemeFeatureSet(int iValue, ref double MinValue, ref double MaxValue)
        {
            try
            {
                FeatureSet fsReturn = new FeatureSet();
                MinValue = 0;
                MaxValue = 0;

                //得到Dic<col,row,value>
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

                //--------------------modify by xiejp---------------------------- CR APV QALY
                int idCboAPV = Convert.ToInt32((cbAPVAggregation.SelectedItem as DataRowView)["GridDefinitionID"]);
                int idCboCFGR = Convert.ToInt32((this.cbCRAggregation.SelectedItem as DataRowView)["GridDefinitionID"]);
                int idCboQALY = -1;// Convert.ToInt32((cbQALYAggregation.SelectedItem as DataRowView)["GridDefinitionID"]);
                int idTo = Convert.ToInt32((cboRegion.SelectedItem as DataRowView)["GridDefinitionID"]);
                IFeatureSet fsValue = (mainMap.Layers[0] as FeatureLayer).DataSet;
                IFeatureSet fsRegion = (mainMap.Layers[1] as FeatureLayer).DataSet;
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
                //首先获得Region的中心点加入Col ,Row
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
                    //BenMAPGrid benMAPGrid = Grid.GridCommon.getBenMAPGridFromID(idCboQALY == -1 ? CommonClass.GBenMAPGrid.GridDefinitionID : idCboQALY);
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
                                    fsReturn.DataTable.Rows[i]["ThemeValue"] = 0.000;// Math.Round(dicValue[rc.Col + "," + rc.Row], 4);
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

                //得到value填入
                return fsReturn;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 饼状图/柱状图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPieTheme_Click(object sender, EventArgs e)
        {
            //默认为MainMap有两个时以第一个图层的最后一个字段做饼状图
            try
            {
                double MinValue = 0;
                double MaxValue = 0;
                IMapLayer removeLayer = null;
                if (mainMap.Layers.Count == 3)
                {
                    foreach (IMapLayer layer in mainMap.Layers)
                    {
                        if (layer is MapPointLayer)
                            removeLayer = layer;
                    }
                }
                if (removeLayer != null)
                    mainMap.Layers.Remove(removeLayer);
                if (mainMap.Layers.Count != 2)
                {
                    MessageBox.Show("No available layer to generate pie theme.");
                    return;
                }
                else
                {
                    //判断用来做图层的对象ow
                    IFeatureSet fs = (mainMap.Layers[1] as PolygonLayer).DataSet;
                    if (fs.DataTable.Rows.Count > 5000)
                    {
                        MessageBox.Show("Too many features to be displayed in this aggregation layer.");
                        return;
                    }
                    WaitShow("Loading theme layer... ");
                    FeatureSet fsValue = null;
                    if (LayerObject != null && LayerObject is BenMAPLine)
                    {
                        //-----------------------------未写--------弹出窗口以哪个字段统计做饼图--------------------
                    }

                    //以第二个图层的最后一个字段做饼图
                    fsValue = getThemeFeatureSet((mainMap.Layers[0] as PolygonLayer).DataSet.DataTable.Columns.Count - 1, ref MinValue, ref MaxValue);

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
                    //增加图层并且渲染
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
                            Bitmap ig = null;// getonly3DPie(150);
                            if (Convert.ToDouble(dr["ThemeValue"]) < 0)
                            {
                                ig = getonly3DPie(150, Color.Green);
                            }
                            else
                            {
                                ig = getonly3DPie(150, Color.Red);
                            }
                            //Bitmap ig = DrawCell(Color.Red, 0, 15, 30, 100, 15);
                            if (Convert.ToInt32(gridvalue) == 0) continue;
                            //----把饼图和柱状图合并
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
                            // Dim pc1 As New PointCategory(NONcommonSymboliser)
                            pc1.FilterExpression = "[ThemeValue] = " + dr["ThemeValue"].ToString();
                            pc1.LegendText = "[ThemeValue] = " + dr["ThemeValue"].ToString();
                            pc1.DisplayExpression();

                            ps.AddCategory(pc1);
                        }
                    }
                    (mainMap.Layers[2] as PointLayer).Symbology = ps;
                    //il.LegendText = NowGridLayer.LayerStyle + "ThemePie";
                }
                WaitClose();
            }
            catch
            {
                WaitClose();
            }
        }

        #region Theme

        /// 画-三▂维?柱ù图?
        /// </summary>
        /// <param name="myColor">颜?色?，?不?透?明÷之?前°</param>
        /// <param name="x">长¤方?体?的?前°面?一?个?面?的?左哩?上?角?的?坐?标括簒点?</param>
        /// <param name="y">长¤方?体?的?前°面?一?个?面?的?左哩?上?角?的?坐?标括簓点?</param>
        /// <param name="width">长¤方?体?的?前°面?一?个?面?的?宽í</param>
        /// <param name="height">长¤方?体?的?前°面?一?个?面?的?高?</param>
        /// <param name="iDeep">长¤方?体?的?深?，?后ó面?一?个?面?的?X坐?标括?为a 前°面?一?个?面?的?x + iDeep </param>
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

            // Create points for polygon.
            PointF[] ptsArray1 =   //left
      {
          p1, p2, p6, p5
      };

            PointF[] ptsArray2 =    //bottom
      {
          p2, p3, p7, p6
      };

            PointF[] ptsArray3 =   //right
      {
          p4, p3, p7, p8
      };

            PointF[] ptsArray4 =   //top
      {
          p1, p4, p8, p5
      };
            //
            //myColor = Color.FromArgb(164,164,251);
            //颜?色?的?问ê题琣正y在ú测a试?中D。￡。￡。￡。￡。￡颜?色?太?难?看′了?。￡。￡。￡。￡：阰（辍?

            SolidBrush defaultBrush = //164,164,251
                new SolidBrush(Color.FromArgb(128, myColor.R, myColor.G, myColor.B));
            int r, g, b;
            r = myColor.R - 37;
            g = myColor.G - 37;
            b = myColor.B - 45;
            if (r < 0) r = 0;
            if (g < 0) g = 0;
            if (b < 0) b = 0;
            SolidBrush topBrush =  // 127,127,206  37,37,45
                new SolidBrush(Color.FromArgb(188, r, g, b));
            int r1, g1, b1;

            r1 = myColor.R - 52;
            g1 = myColor.G - 52;
            b1 = myColor.B - 88;
            if (r1 < 0) r1 = 0;
            if (g1 < 0) g1 = 0;
            if (b1 < 0) b1 = 0;
            SolidBrush rightBrush = //112,112,163  52,52,88
                new SolidBrush(Color.FromArgb(220, r1, g1, b1));

            SolidBrush invisibleBrush =
                new SolidBrush(Color.FromArgb(111, myColor.R, myColor.G, myColor.B));
            SolidBrush visibleBrush =
                new SolidBrush(Color.FromArgb(188, myColor.R, myColor.G, myColor.B));
            SolidBrush FrectBrush =
                new SolidBrush(Color.FromArgb(220, myColor.R, myColor.G, myColor.B));

            // Fill recntagle1
            //objGraphics.FillRectangle(invisibleBrush, Brect);
            objGraphics.FillRectangle(defaultBrush, Brect);
            objGraphics.DrawRectangle(Pens.Black, Brect);

            // Fill Polygon1
            //objGraphics.FillPolygon(invisibleBrush, ptsArray1);
            objGraphics.FillPolygon(defaultBrush, ptsArray1);
            objGraphics.DrawPolygon(Pens.Black, ptsArray1);
            // Fill Polygon2
            //objGraphics.FillPolygon(invisibleBrush, ptsArray2);
            objGraphics.FillPolygon(defaultBrush, ptsArray2);
            objGraphics.DrawPolygon(Pens.Black, ptsArray2);
            // Fill Polygon3
            //objGraphics.FillPolygon(visibleBrush, ptsArray3);
            objGraphics.FillPolygon(rightBrush, ptsArray3);
            objGraphics.DrawPolygon(Pens.Black, ptsArray3);
            // Fill Polygon4
            //objGraphics.FillPolygon(visibleBrush, ptsArray4);
            objGraphics.FillPolygon(topBrush, ptsArray4);
            objGraphics.DrawPolygon(Pens.Black, ptsArray4);

            // Fill recntagle2
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
            //objGraphics.DrawRectangle(new Pen(Color.Black), 0, 0, width, height);
            //objGraphics.FillRectangle(new SolidBrush(Color.Transparent), 1, 1, width - 2, height - 2);
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
            //objGraphics.Dispose();
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
            //objGraphics.DrawRectangle(new Pen(Color.Black), 0, 0, width, height);
            //objGraphics.FillRectangle(new SolidBrush(Color.Transparent), 1, 1, width - 2, height - 2);
            SolidBrush objBrush = new SolidBrush(Color.Blue);
            objGraphics.SmoothingMode = SmoothingMode.AntiAlias;

            int iCurrentPos = 0;

            // Color[] arrColor = { Color.Red, Color.Red, Color.Red, Color.Red, Color.Red, Color.Red };
            Color[] arrColor = { c, c, c, c, c, c };
            for (int i = arrVote.Length - 1; i >= 0; i--)
            {
                arrColor[i] = c;// Color.FromArgb(oRan.Next(255), oRan.Next(255), oRan.Next(255));
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
            //objGraphics.Dispose();
            return objBitmap;
        }

        #endregion Theme

        #endregion MapWindow控件功能
        /// <summary>
        /// 清除所有结果
        /// </summary>
        private void ClearMapTableChart()
        {
            //---------------------------清空原有数据 Map/Table/Chart--------------------------------------------
            mainMap.Layers.Clear();
            OLVResultsShow.SetObjects(null);
            _tableObject = null;
            zedGraphCtl.Visible = false;
            //lblRegion.Visible = false;
            btnApply.Visible = false;
            olvRegions.Visible = false;
            cbGraph.Visible = false;
            groupBox9.Visible = false;
            groupBox1.Visible = false;
            btnSelectAll.Visible = false;
            picGIS.Visible = true;
        }
        //-------------majie存放CRSelectFunctionCalculateValue的属性--------

        /// <summary>
        /// CR结果列表双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void olvCRFunctionResult_DoubleClick(object sender, EventArgs e)
        {
            //Selected--> To GIS Show
            btShowCRResult_Click(sender, e);
            return;
            if (olvCRFunctionResult.Objects == null) return;
            string Tip = "Drawing health impact function result layer";
            WaitShow(Tip);
            bool bGIS = true;
            bool bTable = true;
            bool bChart = true;
            int i = 0;
            int iOldGridType = CommonClass.GBenMAPGrid.GridDefinitionID;
            CRSelectFunctionCalculateValue crSelectFunctionCalculateValue = null;
            for (int icro = 0; icro < CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; icro++)
            {
                CRSelectFunctionCalculateValue cro = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[icro];
                //Configuration.ConfigurationCommonClass.ClearCRSelectFunctionCalculateValueLHS(ref cro);
            }
            foreach (CRSelectFunctionCalculateValue cr in olvCRFunctionResult.SelectedObjects)
            {
                crSelectFunctionCalculateValue = cr;
            }
            if (crSelectFunctionCalculateValue != null)
            {
                //--------------做Aggregation----------------
                if (cbCRAggregation.SelectedIndex != -1 && cbCRAggregation.SelectedIndex != 0)
                {
                    DataRowView drv = cbCRAggregation.SelectedItem as DataRowView;
                    int iAggregationGridType = Convert.ToInt32(drv["GridDefinitionID"]);
                    if (iAggregationGridType != CommonClass.GBenMAPGrid.GridDefinitionID)
                    {
                        crSelectFunctionCalculateValue = APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crSelectFunctionCalculateValue, CommonClass.GBenMAPGrid.GridDefinitionID, iAggregationGridType);
                        CommonClass.GBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(iAggregationGridType);
                    }

                }
                //--------------重新生成拉丁立体方-------------
                //Configuration.ConfigurationCommonClass.UpdateCRSelectFunctionCalculateValueLHS(ref crSelectFunctionCalculateValue);
                if (i == 0)
                {
                    //初始化table数据！
                    //---------------------------清空原有数据 Map/Table/Chart--------------------------------------------
                    ClearMapTableChart();
                    //-----------------------chart未写---------------------------------------------
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
                        InitTableResult(crSelectFunctionCalculateValue);

                    }
                    if (bChart)
                    {
                        foreach (CRSelectFunctionCalculateValue cr in olvCRFunctionResult.SelectedObjects)
                        {
                            InitChartResult(cr, iOldGridType);
                            break;
                        }
                    }
                    if (bGIS)
                    {
                        mainMap.ProjectionModeReproject = ActionMode.Never;
                        mainMap.ProjectionModeDefine = ActionMode.Never;
                        string shapeFileName = "";
                        if (CommonClass.GBenMAPGrid is ShapefileGrid)
                        {
                            mainMap.Layers.Clear();
                            if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                            {
                                shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
                                //mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp");
                            }
                        }
                        else if (CommonClass.GBenMAPGrid is RegularGrid)
                        {
                            mainMap.Layers.Clear();
                            if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp"))
                            {
                                shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp";
                                // mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp");
                            }
                        }
                        tsbChangeProjection.Text = "change projection to Albers";
                        //FeatureSet fs = new FeatureSet();
                        //fs.Open(shapeFileName);
                        mainMap.Layers.Add(shapeFileName);

                        DataTable dt = (mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).DataSet.DataTable;
                        (mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).LegendText = "CRResult";
                        (mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).Name = "CRResult";
                        //加值
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
                        dt.Columns.Add("Value", typeof(double));
                        j = 0;
                        while (j < dt.Columns.Count)
                        {
                            if (dt.Columns[j].ColumnName.ToLower() == "col") iCol = j;
                            if (dt.Columns[j].ColumnName.ToLower() == "row") iRow = j;

                            j++;
                        }
                        j = 0;
                        Dictionary<string, double> dicAll = new Dictionary<string, double>();
                        foreach (CRCalculateValue crcv in crSelectFunctionCalculateValue.CRCalculateValues)
                        {
                            if (!dicAll.ContainsKey(crcv.Col + "," + crcv.Row))
                                dicAll.Add(crcv.Col + "," + crcv.Row, crcv.PointEstimate);
                        }
                        //匹配
                        foreach (DataRow dr in dt.Rows)
                        {
                            try
                            {
                                //dr["Value"] = crSelectFunctionCalculateValue.CRCalculateValues.Where(p => p.Col == Convert.ToInt32(dr[iCol])
                                //    && p.Row == Convert.ToInt32(dr[iRow])).Select(p => p.PointEstimate).First();
                                if (dicAll.ContainsKey(dr[iCol] + "," + dr[iRow]))
                                    dr["Value"] = dicAll[dr[iCol] + "," + dr[iRow]];
                                else
                                    dr["Value"] = 0;
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        if (File.Exists(CommonClass.DataFilePath + @"\Tmp\Incidence.shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Tmp\Incidence.shp");
                        (mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).DataSet.SaveAs(CommonClass.DataFilePath + @"\Tmp\Incidence.shp", true);
                        mainMap.Layers.Clear();
                        mainMap.Layers.Add(CommonClass.DataFilePath + @"\Tmp\Incidence.shp");
                        MapPolygonLayer polLayer = mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer;
                        string strValueField = polLayer.DataSet.DataTable.Columns[polLayer.DataSet.DataTable.Columns.Count - 1].ColumnName;
                        PolygonScheme myScheme1 = new PolygonScheme();
                        float fl = (float)0.1;
                        myScheme1.EditorSettings.StartColor = Color.Blue;
                        myScheme1.EditorSettings.StartColor.ToTransparent(fl);
                        myScheme1.EditorSettings.EndColor = Color.Red;
                        myScheme1.EditorSettings.EndColor.ToTransparent(fl);

                        myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
                        //myScheme1.EditorSettings.ClassificationType = ClassificationType.UniqueValues ;
                        myScheme1.EditorSettings.NumBreaks = 6;
                        myScheme1.EditorSettings.FieldName = strValueField;// "Value";
                        myScheme1.EditorSettings.UseGradient = false;
                        myScheme1.CreateCategories(polLayer.DataSet.DataTable);

                        //PolygonCategory pc = new PolygonCategory(Color.LightBlue, Color.DarkBlue, 1);
                        //pc.FilterExpression = "[Value] < 6.5 ";
                        //pc.LegendText = "0-6.5";
                        //myScheme1.AddCategory(pc);
                        //----------
                        //UniqueValues+半透明
                        double dMinValue = 0.0;
                        double dMaxValue = 0.0;
                        //Base或者Control，默认都按Base的区间来画，以保证Base和Control都是同一区间
                        dMinValue = crSelectFunctionCalculateValue.CRCalculateValues.Count == 0 ? 0 : crSelectFunctionCalculateValue.CRCalculateValues.Min(a => a.PointEstimate);
                        dMaxValue = crSelectFunctionCalculateValue.CRCalculateValues.Count == 0 ? 0 : crSelectFunctionCalculateValue.CRCalculateValues.Max(a => a.PointEstimate);

                        //Todo:陈志润 20111124修改
                        _dMinValue = dMinValue;
                        _dMaxValue = dMaxValue;
                        _currentLayerIndex = mainMap.Layers.Count - 1;
                        string pollutantUnit = string.Empty;//benMAPLine
                        _columnName = strValueField;
                        RenderMainMap(true);

                        //Color[] colors = new Color[] { Color.Blue, Color.FromArgb(0, 255, 255), Color.FromArgb(0, 255, 0), Color.Yellow, Color.Red, Color.FromArgb(255, 0, 255) };
                        ////Quantities+半透明
                        //PolygonCategoryCollection pcc = new PolygonCategoryCollection();
                        //int iColor = 0;

                        //foreach (PolygonCategory pc in myScheme1.Categories)
                        //{
                        //    //pc.Symbolizer.SetOutlineWidth(0);
                        //    PolygonCategory pcin = pc;
                        //    double dnow = dMinValue + ((dMaxValue - dMinValue) / 6.00) * Convert.ToDouble(iColor);
                        //    double dnowUp = dMinValue + ((dMaxValue - dMinValue) / 6.00) * Convert.ToDouble(iColor + 1);
                        //    pcin.FilterExpression = string.Format("[{0}]>=" + dnow + " and [{0}] <" + dnowUp, strValueField);
                        //    pcin.LegendText = string.Format("[{0}]>=" + dnow.ToString("E2") + " and [{0}] <" + dnowUp.ToString("E2"), strValueField);//.ToString("E2")
                        //    if (iColor == 0)
                        //    {
                        //        pcin.FilterExpression = string.Format(" [{0}] <" + dnowUp.ToString("E2"), strValueField);
                        //        pcin.LegendText = string.Format(" [{0}] <" + dnowUp.ToString("E2"), strValueField);

                        //    }
                        //    if (iColor == myScheme1.Categories.Count - 1)
                        //    {
                        //        pcin.FilterExpression = string.Format(" [{0}] >=" + dnow.ToString("E2"), strValueField);
                        //        pcin.LegendText = string.Format(" [{0}] >=" + dnow.ToString("E2"), strValueField);

                        //    }

                        //    //pcin.LegendText = pcin.FilterExpression;

                        //    pcin.Symbolizer.SetOutline(Color.Transparent, 0);
                        //    Color ctemp = pcin.Symbolizer.GetFillColor();
                        //    float fColor = (float)0.2;
                        //    ctemp.ToTransparent(fColor);
                        //    pcin.Symbolizer.SetFillColor(colors[iColor]);
                        //    pcc.Add(pcin);
                        //    iColor++;
                        //}
                        //myScheme1.ClearCategories();
                        //foreach (PolygonCategory pct in pcc)
                        //{
                        //    myScheme1.Categories.Add(pct);
                        //}
                        ////player.Symbology = myScheme1;

                        //polLayer.Symbology = myScheme1;
                        addRegionLayerToMainMap();
                    }
                }
                i++;
                CommonClass.GBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(iOldGridType);
            }
            WaitClose();
        }
        /// <summary>
        /// APV结果列表双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                //----------modify by xiejp 20120618--for multi-Pooling Window-- multi-Select

                if (sender is ObjectListView || sender is Button)
                {
                    foreach (KeyValuePair<AllSelectValuationMethod, string> keyValue in tlvAPVResult.SelectedObjects)
                    {
                        AllSelectValuationMethod allSelectValuationMethod = keyValue.Key;
                        if (rbAPVOnlyOne.Checked)//--Only one Pooling Window
                        {
                            //-----------------首先判断是否有结果-------------
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
                            //----------find Pooling Window first! ----
                            AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = null;
                            if (allSelectValuationMethod.ID < 0) continue;
                            //int iMaxID = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Max(p => p.LstAllSelectValuationMethod.Max(a => a.ID)) + 1;
                            //int iPoolingWindow = allSelectValuationMethod.ID / iMaxID;

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
                    bGIS = false;
                    bChart = false;
                    tabCtlMain.SelectedIndex = 1;
                    foreach (KeyValuePair<AllSelectValuationMethod, string> keyValue in tlvAPVResult.Objects)
                    {
                        AllSelectValuationMethod allSelectValuationMethod = keyValue.Key;
                        if (rbAPVOnlyOne.Checked)//--Only one Pooling Window
                        {
                            //-----------------首先判断是否有结果-------------
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
                            //----------find Pooling Window first! ----
                            AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = null;
                            //if (allSelectValuationMethod.ID < 0) continue;
                            //int iMaxID = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Max(p => p.LstAllSelectValuationMethod.Max(a => a.ID)) + 1;
                            //int iPoolingWindow = allSelectValuationMethod.ID / iMaxID;

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
                //---------------------------清空原有数据 Map/Table/Chart--------------------------------------------
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
                //-----------------------chart未写---------------------------------------------
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
                //------------modify by xiejp 2012 0302 加入Aggregation---------

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
                    //do
                    tsbChangeProjection.Text = "Change projection to Albers";
                    mainMap.ProjectionModeReproject = ActionMode.Never;
                    mainMap.ProjectionModeDefine = ActionMode.Never;
                    string shapeFileName = "";
                    if (!chbAPVAggregation.Checked)
                    {
                        if (CommonClass.GBenMAPGrid is ShapefileGrid)
                        {
                            mainMap.Layers.Clear();
                            if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                            {
                                shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
                                //mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp");
                            }
                        }
                        else if (CommonClass.GBenMAPGrid is RegularGrid)
                        {
                            mainMap.Layers.Clear();
                            if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp"))
                            {
                                shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp";
                                // mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp");
                            }
                        }
                    }
                    else
                    {
                        if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation is ShapefileGrid)
                        {
                            mainMap.Layers.Clear();
                            if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation as ShapefileGrid).ShapefileName + ".shp"))
                            {
                                shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation as ShapefileGrid).ShapefileName + ".shp";
                                //mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation as ShapefileGrid).ShapefileName + ".shp");
                            }
                        }
                        else if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation is RegularGrid)
                        {
                            mainMap.Layers.Clear();
                            if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation as RegularGrid).ShapefileName + ".shp"))
                            {
                                shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation as RegularGrid).ShapefileName + ".shp";
                                // mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation as RegularGrid).ShapefileName + ".shp");
                            }
                        }
                    }
                    if (benMapGridShow != null)
                    {
                        shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + ((benMapGridShow is ShapefileGrid) ? (benMapGridShow as ShapefileGrid).ShapefileName : (benMapGridShow as RegularGrid).ShapefileName) + ".shp";
                    }
                    mainMap.Layers.Add(shapeFileName);
                    IFeatureSet fs = (mainMap.Layers[0] as MapPolygonLayer).DataSet;
                    //fs.Open(shapeFileName);
                    (mainMap.Layers[0] as MapPolygonLayer).Name = "APVResult";
                    (mainMap.Layers[0] as MapPolygonLayer).LegendText = "APVResult";

                    //加值
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
                    fs.DataTable.Columns.Add("Pooled Valuation", typeof(double));
                    j = 0;
                    while (j < fs.DataTable.Columns.Count)
                    {
                        if (fs.DataTable.Columns[j].ColumnName.ToLower() == "col") iCol = j;
                        if (fs.DataTable.Columns[j].ColumnName.ToLower() == "row") iRow = j;

                        j++;
                    }
                    j = 0;
                    Dictionary<string, double> dicAll = new Dictionary<string, double>();
                    foreach (APVValueAttribute crcv in lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes)
                    {
                        if (!dicAll.ContainsKey(crcv.Col + "," + crcv.Row))
                            dicAll.Add(crcv.Col + "," + crcv.Row, crcv.PointEstimate);
                    }
                    //匹配
                    foreach (DataRow dr in fs.DataTable.Rows)
                    {
                        try
                        {
                            if (dicAll.ContainsKey(dr[iCol] + "," + dr[iRow]))
                                dr["Pooled Valuation"] = dicAll[dr[iCol] + "," + dr[iRow]];
                            else
                                dr["Pooled Valuation"] = 0;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    if (File.Exists(CommonClass.DataFilePath + @"\Tmp\APVTemp.shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Tmp\APVTemp.shp");
                    fs.SaveAs(CommonClass.DataFilePath + @"\Tmp\APVTemp.shp", true);
                    //fs.Dispose();
                    mainMap.Layers.Clear();
                    mainMap.Layers.Add(CommonClass.DataFilePath + @"\Tmp\APVTemp.shp");
                    (mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).DataSet.DataTable.Columns[(mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).DataSet.DataTable.Columns.Count - 1].ColumnName = "Pooled Valuation";                           
                    string author = "Pooled Valuation";
                    if (lstallSelectValuationMethodAndValue.First().AllSelectValuationMethod != null
                        && lstallSelectValuationMethodAndValue.First().AllSelectValuationMethod.Author != null)
                    {
                        author = lstallSelectValuationMethodAndValue.First().AllSelectValuationMethod.Author;
                        if (author.IndexOf(" ") > -1)
                        {
                            author = author.Substring(0, author.IndexOf(" "));
                        }
                    }
                    mainMap.Layers[mainMap.Layers.Count() - 1].LegendText = author;
                    MapPolygonLayer polLayer = mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer;
                    string strValueField = polLayer.DataSet.DataTable.Columns[polLayer.DataSet.DataTable.Columns.Count - 1].ColumnName;
                    PolygonScheme myScheme1 = new PolygonScheme();
                    float fl = (float)0.1;
                    myScheme1.EditorSettings.StartColor = Color.Blue;
                    myScheme1.EditorSettings.StartColor.ToTransparent(fl);
                    myScheme1.EditorSettings.EndColor = Color.Red;
                    myScheme1.EditorSettings.EndColor.ToTransparent(fl);

                    myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
                    //myScheme1.EditorSettings.ClassificationType = ClassificationType.UniqueValues ;
                    myScheme1.EditorSettings.NumBreaks = 6;
                    myScheme1.EditorSettings.FieldName = strValueField;// "Value";
                    myScheme1.EditorSettings.UseGradient = false;
                    myScheme1.CreateCategories(polLayer.DataSet.DataTable);

                    //PolygonCategory pc = new PolygonCategory(Color.LightBlue, Color.DarkBlue, 1);
                    //pc.FilterExpression = "[Value] < 6.5 ";
                    //pc.LegendText = "0-6.5";
                    //myScheme1.AddCategory(pc);
                    //----------
                    //UniqueValues+半透明
                    double dMinValue = 0.0;
                    double dMaxValue = 0.0;
                    //Base或者Control，默认都按Base的区间来画，以保证Base和Control都是同一区间
                    dMinValue = lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.Min(a => a.PointEstimate);
                    dMaxValue = lstallSelectValuationMethodAndValue.First().lstAPVValueAttributes.Max(a => a.PointEstimate);

                    //Todo:陈志润 20111124修改
                    _dMinValue = dMinValue;
                    _dMaxValue = dMaxValue;
                    _currentLayerIndex = mainMap.Layers.Count - 1;
                    //RenderMainMap(mainMap.Layers.Count - 1, strValueField, true, dMinValue, dMaxValue);
                    //string pollutantUnit = string.Empty;//benMAPLine
                    _columnName = strValueField;
                    RenderMainMap(true);


                    addRegionLayerToMainMap();

                }
                WaitClose();
            }
            catch (Exception ex)
            { }
            finally
            {
                WaitClose();
            }
        }

        /// <summary>
        /// 递归得到所有非None的Method
        /// </summary>
        /// <param name="allSelectValueMethod"></param>
        /// <param name="lstAll"></param>
        /// <param name="lstReturn"></param>
        private void getAllChildMethodNotNone(AllSelectValuationMethod allSelectValueMethod, List<AllSelectValuationMethod> lstAll, ref List<AllSelectValuationMethod> lstReturn)
        {
            List<AllSelectValuationMethod> lstOne = lstAll.Where(p => p.PID == allSelectValueMethod.ID).ToList();// && (p.PoolingMethod != "None" || p.NodeType == 3)).ToList();
            lstReturn.AddRange(lstOne.Where(p => p.PoolingMethod != "None" || p.NodeType == 3).ToList());
            foreach (AllSelectValuationMethod asvm in lstOne)
            {
                getAllChildMethodNotNone(asvm, lstAll, ref lstReturn);
            }
        }

        /// <summary>
        /// 递归得到所有非None的Method
        /// </summary>
        /// <param name="allSelectValueMethod"></param>
        /// <param name="lstAll"></param>
        /// <param name="lstReturn"></param>
        private void getAllChildQALYMethodNotNone(AllSelectQALYMethod allSelectQALYMethod, List<AllSelectQALYMethod> lstAll, ref List<AllSelectQALYMethod> lstReturn)
        {
            List<AllSelectQALYMethod> lstOne = lstAll.Where(p => p.PID == allSelectQALYMethod.ID).ToList();// && (p.PoolingMethod != "None" || p.NodeType == 3)).ToList();
            lstReturn.AddRange(lstOne.Where(p => p.PoolingMethod != "None" || p.NodeType == 3).ToList());
            foreach (AllSelectQALYMethod asvm in lstOne)
            {
                getAllChildQALYMethodNotNone(asvm, lstAll, ref lstReturn);
            }
        }

        #region table Result show

        private int _pageCurrent;
        private int _currentRow;
        private int _pageSize;
        private int _pageCount;
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
                    fieldName = "BenMAPHealthImpactFunction.Pollutant.PollutantName";
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
                case "Location":
                    fieldName = "BenMAPHealthImpactFunction.strLocations";
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
                    fieldName = crf.BenMAPHealthImpactFunction.Pollutant.PollutantName;
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
                case "Location":
                    fieldName = crf.BenMAPHealthImpactFunction.strLocations;
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
                case "Location":
                    fieldName = "Location";
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
                    fieldName = allSelectValuationMethod.Pollutant;
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
                case "Location":
                    fieldName = allSelectValuationMethod.Location;
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
                IncidencelstHealth.Add(new FieldCheck() { FieldName = "Location", isChecked = false });
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
                cflstHealth.Add(new FieldCheck() { FieldName = "Location", isChecked = false });
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
                apvlstHealth.Add(new FieldCheck() { FieldName = "Location", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Other Pollutants", isChecked = false });
                apvlstHealth.Add(new FieldCheck() { FieldName = "Qualifier", isChecked = false });
                //apvlstHealth.Add(new FieldCheck(){FieldName="Reference", isChecked=false});
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
                qalylstHealth.Add(new FieldCheck() { FieldName = "Location", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Other Pollutants", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Qualifier", isChecked = false });
                //qalylstHealth.Add(new FieldCheck(){FieldName="Reference", isChecked=false});
                qalylstHealth.Add(new FieldCheck() { FieldName = "Race", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Ethnicity", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Gender", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Start Age", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "End Age", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Function", isChecked = false });
                qalylstHealth.Add(new FieldCheck() { FieldName = "Version", isChecked = true });
            }
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
                //if (oTable is CRSelectFunctionCalculateValue)
                //{
                //    //first 50 init columns
                //    CRSelectFunctionCalculateValue crTable = (CRSelectFunctionCalculateValue)oTable;
                //    crTable.CRCalculateValues = crTable.CRCalculateValues.Where(p => p.Population > 0).ToList();
                //    BrightIdeasSoftware.OLVColumn olvColumnCol = new BrightIdeasSoftware.OLVColumn() { AspectName = "Col", Text = "Col", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnCol);
                //    BrightIdeasSoftware.OLVColumn olvColumnRow = new BrightIdeasSoftware.OLVColumn() { AspectName = "Row", Text = "Row", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnRow);
                //    BrightIdeasSoftware.OLVColumn olvColumnPointEstimate = new BrightIdeasSoftware.OLVColumn() { AspectName = "PointEstimate", Width = "PointEstimate".Length * 8, Text = "PointEstimate", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPointEstimate);
                //    BrightIdeasSoftware.OLVColumn olvColumnPopulation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Population", Text = "Population", Width = "Population".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPopulation);
                //    BrightIdeasSoftware.OLVColumn olvColumnIncidence = new BrightIdeasSoftware.OLVColumn() { AspectName = "Incidence", Text = "Incidence", Width = "Incidence".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnIncidence);
                //    BrightIdeasSoftware.OLVColumn olvColumnDelta = new BrightIdeasSoftware.OLVColumn() { AspectName = "Delta", Text = "Delta", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnDelta);
                //    BrightIdeasSoftware.OLVColumn olvColumnMean = new BrightIdeasSoftware.OLVColumn() { AspectName = "Mean", Text = "Mean", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnMean);
                //    BrightIdeasSoftware.OLVColumn olvColumnBaseline = new BrightIdeasSoftware.OLVColumn() { AspectName = "Baseline", Text = "Baseline", Width = "Baseline2".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnBaseline);
                //    BrightIdeasSoftware.OLVColumn olvColumnPercentOfBaseline = new BrightIdeasSoftware.OLVColumn() { AspectName = "PercentOfBaseline", Width = "PercentOfBaseline".Length * 8, Text = "PercentOfBaseline", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPercentOfBaseline);
                //    BrightIdeasSoftware.OLVColumn olvColumnStandardDeviation = new BrightIdeasSoftware.OLVColumn() { AspectName = "StandardDeviation", Width = "PercentOfBaseline".Length * 8, Text = "StandardDeviation", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnStandardDeviation);
                //    BrightIdeasSoftware.OLVColumn olvColumnVariance = new BrightIdeasSoftware.OLVColumn() { AspectName = "Variance", Text = "Variance", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnVariance);
                //    List<float> lstTemp = new List<float>();
                //    if (crTable.CRCalculateValues.First().LstPercentile != null)
                //    {
                //        i = 0;
                //        while (i < crTable.CRCalculateValues.First().LstPercentile.Count())
                //        {
                //            //Percentile +  i* count/100[" + i + "]

                //            BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "LstPercentile[" + i + "]", Width = "Percentile100".Length * 8, Text = "Percentile" + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(crTable.CRCalculateValues.First().LstPercentile.Count()))))), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
                //            lstTemp.Add(0);
                //            i++;
                //        }
                //    }
                //    crTable.CRCalculateValues.Insert(0, new CRCalculateValue() { Col = 0, Row = 0, PointEstimate = crTable.CRCalculateValues.Average(p => p.PointEstimate), LstPercentile = lstTemp });
                //    _tableObject = crTable;
                //    OLVResultsShow.SetObjects(crTable.CRCalculateValues.GetRange(0, crTable.CRCalculateValues.Count() > 50 ? 50 : crTable.CRCalculateValues.Count()));
                //    _pageSize = 50;
                //    _currentRow = 0;
                //    _pageCount = crTable.CRCalculateValues.Count / 50 + 1;
                //    _pageCurrent = 1;
                //    bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
                //    bindingNavigatorCountItem.Text = _pageCount.ToString();
                //}
                //else
                if (oTable is Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> )
                {
                    List<CRSelectFunctionCalculateValue> lstCRSelectFunctionCalculateValue = new List<CRSelectFunctionCalculateValue>();
                    if (this.tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Tag.ToString() == "incidence")
                    {
                        foreach (KeyValuePair<AllSelectCRFunction, string> keyValueCR in olvIncidence.SelectedObjects)
                        {
                            AllSelectCRFunction cr = keyValueCR.Key;

                            if (cr.CRSelectFunctionCalculateValue == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues.Count == 0)
                            {
                                //WaitClose();
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
                            //crSelectFunctionCalculateValue = cr;
                        }
                        //--------------做Aggregation----------------
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
                                    //crSelectFunctionCalculateValue = APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crSelectFunctionCalculateValue, CommonClass.GBenMAPGrid.GridDefinitionID, iAggregationGridType);
                                }
                                lstCRSelectFunctionCalculateValue = lstTemp;
                            }

                        }
                    }
                    oTable = lstCRSelectFunctionCalculateValue;
                }
                if (oTable is List<AllSelectCRFunction>)
                {
                    List<AllSelectCRFunction> lstAllSelectCRFuntion = (List<AllSelectCRFunction>)oTable;
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
                            if (fieldCheck.FieldName.ToLower() == "version" && fieldCheck.isChecked)
                            {
                                BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value.Version", Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                           
                            }
                            else if (fieldCheck.isChecked)
                            {
                                BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value.CRSelectFunctionCalculateValue.CRSelectFunction." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                            }
                        }
                    }
                    if (IncidencelstResult == null)
                    {
                        BrightIdeasSoftware.OLVColumn olvColumnPointEstimate = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PointEstimate", AspectToStringFormat = "{0:N4}", Width = "Point Estimate".Length * 8, Text = "Point Estimate", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPointEstimate);
                        BrightIdeasSoftware.OLVColumn olvColumnPopulation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Population", AspectToStringFormat = "{0:N4}", Text = "Population", Width = "Population".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPopulation);
                        //BrightIdeasSoftware.OLVColumn olvColumnIncidence = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Incidence", AspectToStringFormat = "{0:N4}", Text = "Incidence", Width = "Incidence".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnIncidence);
                        BrightIdeasSoftware.OLVColumn olvColumnDelta = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Delta", AspectToStringFormat = "{0:N4}", Text = "Delta", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnDelta);
                        BrightIdeasSoftware.OLVColumn olvColumnMean = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Mean", AspectToStringFormat = "{0:N4}", Text = "Mean", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnMean);
                        BrightIdeasSoftware.OLVColumn olvColumnBaseline = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Baseline", AspectToStringFormat = "{0:N4}", Text = "Baseline", Width = "Baseline2".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnBaseline);
                        BrightIdeasSoftware.OLVColumn olvColumnPercentOfBaseline = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PercentOfBaseline", AspectToStringFormat = "{0:N4}", Width = "Percent Of Baseline".Length * 8, Text = "Percent Of Baseline", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPercentOfBaseline);
                        BrightIdeasSoftware.OLVColumn olvColumnStandardDeviation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.StandardDeviation", AspectToStringFormat = "{0:N4}", Width = "Standard Deviation".Length * 8, Text = "Standard Deviation", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnStandardDeviation);
                        BrightIdeasSoftware.OLVColumn olvColumnVariance = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Variance", Text = "Variance", AspectToStringFormat = "{0:N4}", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnVariance);
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
                                //Percentile +  i* count/100[" + i + "]
                                if (IncidencelstResult == null || IncidencelstResult.Last().isChecked)
                                {
                                    BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + i + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectCRFuntion.First().CRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count()))))), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
                                }
                                i++;
                            }
                        }
                    }
                    Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction> dicAPV = new Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>();// new Dictionary<CRCalculateValue, CRSelectFunction>();
                    int iLstCRTable = 0;
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
                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(0, dicAPV.Count > 50 ? 50 : dicAPV.Count));
                    _pageSize = 50;
                    _currentRow = 0;
                    _pageCount = dicAPV.Count / 50 + 1;//.Count / 50 + 1;
                    _pageCurrent = 1;
                    bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
                    bindingNavigatorCountItem.Text = _pageCount.ToString();
                }
                if (oTable is List<CRSelectFunctionCalculateValue> || oTable is CRSelectFunctionCalculateValue)
                {
                    //first 50 init columns

                    List<CRSelectFunctionCalculateValue> lstCRTable = new List<CRSelectFunctionCalculateValue>();
                    if (oTable is List<CRSelectFunctionCalculateValue>)
                        lstCRTable = (List<CRSelectFunctionCalculateValue>)oTable;
                    else
                        lstCRTable.Add(oTable as CRSelectFunctionCalculateValue);
                    //-----------modify by xiejp 20120518 first orderby collumn-------------
                    for (int iCR = 0; iCR < lstCRTable.Count; iCR++)
                    {
                        CRSelectFunctionCalculateValue cr = lstCRTable[iCR];
                        //cr.CRCalculateValues.Sort(p => p.Col);
                        cr.CRCalculateValues = cr.CRCalculateValues.Where(p => p != null).OrderBy(p => p.Col).ToList();
                    }
                    //lstCRTable.CRCalculateValues = lstCRTable.CRCalculateValues.Where(p => p.Population > 0).ToList();
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
                            BrightIdeasSoftware.OLVColumn olvColumnPointEstimate = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PointEstimate", AspectToStringFormat = "{0:N4}", Width = "PointEstimate".Length * 8, Text = "Point Estimate", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPointEstimate);
                            BrightIdeasSoftware.OLVColumn olvColumnPopulation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Population", AspectToStringFormat = "{0:N4}", Text = "Population", Width = "Population".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPopulation);
                            //BrightIdeasSoftware.OLVColumn olvColumnIncidence = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Incidence", AspectToStringFormat = "{0:N4}", Text = "Incidence", Width = "Incidence".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnIncidence);
                            BrightIdeasSoftware.OLVColumn olvColumnDelta = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Delta", AspectToStringFormat = "{0:N4}", Text = "Delta", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnDelta);
                            BrightIdeasSoftware.OLVColumn olvColumnMean = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Mean", AspectToStringFormat = "{0:N4}", Text = "Mean", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnMean);
                            BrightIdeasSoftware.OLVColumn olvColumnBaseline = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Baseline", AspectToStringFormat = "{0:N4}", Text = "Baseline", Width = "Baseline2".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnBaseline);
                            BrightIdeasSoftware.OLVColumn olvColumnPercentOfBaseline = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PercentOfBaseline", AspectToStringFormat = "{0:N4}", Width = "Percent Of Baseline".Length * 8, Text = "Percent Of Baseline", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPercentOfBaseline);
                            BrightIdeasSoftware.OLVColumn olvColumnStandardDeviation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.StandardDeviation", AspectToStringFormat = "{0:N4}", Width = "Standard Deviation".Length * 8, Text = "Standard Deviation", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnStandardDeviation);
                            BrightIdeasSoftware.OLVColumn olvColumnVariance = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Variance", Text = "Variance", AspectToStringFormat = "{0:N4}", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnVariance);
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
                                    //Percentile +  i* count/100[" + i + "]
                                    if (IncidencelstResult == null || IncidencelstResult.Last().isChecked)
                                    {
                                        BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + i + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()))))), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
                                    }
                                    i++;
                                }
                            }
                        }
                    }
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

                                if (fieldCheck.isChecked)
                                {
                                    BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstHealth(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
                                }
                            }
                        }
                        if (cflstResult == null)
                        {
                            BrightIdeasSoftware.OLVColumn olvColumnPointEstimate = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PointEstimate", AspectToStringFormat = "{0:N4}", Width = "PointEstimate".Length * 8, Text = "Point Estimate", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPointEstimate);
                            BrightIdeasSoftware.OLVColumn olvColumnPopulation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Population", AspectToStringFormat = "{0:N4}", Text = "Population", Width = "Population".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPopulation);
                            //BrightIdeasSoftware.OLVColumn olvColumnIncidence = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Incidence", AspectToStringFormat = "{0:N4}", Text = "Incidence", Width = "Incidence".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnIncidence);
                            BrightIdeasSoftware.OLVColumn olvColumnDelta = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Delta", AspectToStringFormat = "{0:N4}", Text = "Delta", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnDelta);
                            BrightIdeasSoftware.OLVColumn olvColumnMean = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Mean", AspectToStringFormat = "{0:N4}", Text = "Mean", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnMean);
                            BrightIdeasSoftware.OLVColumn olvColumnBaseline = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Baseline", Text = "Baseline", AspectToStringFormat = "{0:N4}", Width = "Baseline2".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnBaseline);
                            BrightIdeasSoftware.OLVColumn olvColumnPercentOfBaseline = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.PercentOfBaseline", AspectToStringFormat = "{0:N4}", Width = "Percent Of Baseline".Length * 8, Text = "Percent Of Baseline", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPercentOfBaseline);
                            BrightIdeasSoftware.OLVColumn olvColumnStandardDeviation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.StandardDeviation", AspectToStringFormat = "{0:N4}", Width = "Standard Deviation".Length * 8, Text = "Standard Deviation", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnStandardDeviation);
                            BrightIdeasSoftware.OLVColumn olvColumnVariance = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.Variance", AspectToStringFormat = "{0:N4}", Text = "Variance", IsEditable = false, Width = "Variance".Length * 8 }; OLVResultsShow.Columns.Add(olvColumnVariance);
                        }
                        else
                        {
                            foreach (FieldCheck fieldCheck in cflstResult)
                            {

                                if (fieldCheck.isChecked && fieldCheck.FieldName != cflstResult.Last().FieldName && fieldCheck.FieldName != "Population Weighted Delta"
                                    && fieldCheck.FieldName != "Population Weighted Base" && fieldCheck.FieldName != "Population Weighted Control")
                                {
                                    BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key." + getFieldNameFromlstHealth(fieldCheck.FieldName), AspectToStringFormat = "{0:N4}", Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
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
                                    //Percentile +  i* count/100[" + i + "]
                                    if (cflstResult == null || cflstResult.Last().isChecked)
                                    {
                                        BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Key.LstPercentile[" + i + "]", AspectToStringFormat = "{0:N4}", Width = "Percentile100".Length * 8, Text = "Percentile " + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstCRTable.First().CRCalculateValues.First().LstPercentile.Count()))))), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
                                    }
                                    i++;
                                }
                            }
                        }
                    }
                    Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicAPV = new Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>();// new Dictionary<CRCalculateValue, CRSelectFunction>();
                    int iLstCRTable = 0;
                    Dictionary<CRCalculateValue, int> dicKey = new Dictionary<CRCalculateValue, int>();
                    //------------------加入对Population Weight Delta 等的考虑------
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
                                CRSelectFunction crNew=CommonClass.getCRSelectFunctionClone(cr.CRSelectFunction);
                                crNew.BenMAPHealthImpactFunction.EndPoint = "Population Weighted Delta";
                                dicAPV.Add(dicKey.ToList()[0], crNew );
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
                                //---------得到Base------
                                BenMAPLine benMAPLineBase = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == crNew.BenMAPHealthImpactFunction.Pollutant.PollutantID).First().Base;
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
                                    if(dicBase.ContainsKey(crvForEstimate.Col+","+crvForEstimate.Row))
                                    {
                                        dPointEstimate = dPointEstimate + dicBase[crvForEstimate.Col + "," + crvForEstimate.Row] * crvForEstimate.Population;
                                    }
                                }
                                crv.PointEstimate = dPointEstimate/cr.CRCalculateValues.Sum(p => p.Population);
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
                                //---------得到Base------
                                BenMAPLine benMAPLineControl = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == crNew.BenMAPHealthImpactFunction.Pollutant.PollutantID).First().Control;
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
                    foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                    {
                        foreach (CRCalculateValue crv in cr.CRCalculateValues)
                        {
                            dicKey = null;
                            dicKey = new Dictionary<CRCalculateValue, int>();
                            dicKey.Add(crv, iLstCRTable);
                            dicAPV.Add(dicKey.ToList()[0], cr.CRSelectFunction);
                        }
                        iLstCRTable++;
                    }
                    _tableObject = lstCRTable;
                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(0, dicAPV.Count > 50 ? 50 : dicAPV.Count));
                    _pageSize = 50;
                    _currentRow = 0;
                    _pageCount = dicAPV.Count / 50 + 1;//.Count / 50 + 1;
                    _pageCurrent = 1;
                    bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
                    bindingNavigatorCountItem.Text = _pageCount.ToString();
                }
                //else if (oTable is AllSelectValuationMethodAndValue)
                //{
                //    AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = (AllSelectValuationMethodAndValue)oTable;
                //    BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value", Text = "MethodID", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);

                //    BrightIdeasSoftware.OLVColumn olvColumnCol = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Col", Text = "Col", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnCol);
                //    BrightIdeasSoftware.OLVColumn olvColumnRow = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Row", Text = "Row", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnRow);
                //    BrightIdeasSoftware.OLVColumn olvColumnPointEstimate = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.PointEstimate", Width = "PointEstimate".Length * 8, Text = "PointEstimate", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPointEstimate);
                //    BrightIdeasSoftware.OLVColumn olvColumnMean = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Mean", Text = "Mean", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnMean);
                //    BrightIdeasSoftware.OLVColumn olvColumnStandardDeviation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.StandardDeviation", Width = "StandardDeviation".Length * 8, Text = "StandardDeviation", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnStandardDeviation);
                //    BrightIdeasSoftware.OLVColumn olvColumnVariance = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Variance", Text = "Variance", Width = "Variance".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnVariance);
                //    if (allSelectValuationMethodAndValue.lstAPVValueAttributes.First().LstPercentile != null)
                //    {
                //        i = 0;
                //        while (i < allSelectValuationMethodAndValue.lstAPVValueAttributes.First().LstPercentile.Count())
                //        {
                //            //Percentile +  i* count/100[" + i + "]

                //            BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.LstPercentile[" + i + "]", Width = "LstPercentile".Length * 8, Text = "Percentile" + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(allSelectValuationMethodAndValue.lstAPVValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(allSelectValuationMethodAndValue.lstAPVValueAttributes.First().LstPercentile.Count()))))), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

                //            i++;
                //        }
                //    }
                //    _tableObject = allSelectValuationMethodAndValue;
                //    Dictionary<APVValueAttribute, int> dicAPV = new Dictionary<APVValueAttribute, int>();
                //    foreach (APVValueAttribute apvx in allSelectValuationMethodAndValue.lstAPVValueAttributes)
                //    {
                //        dicAPV.Add(apvx, allSelectValuationMethodAndValue.AllSelectValuationMethod.ID);
                //    }
                //    //if(dicAPV.Count
                //    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(0, dicAPV.Count > 50 ? 50 : dicAPV.Count));
                //    _pageSize = 50;
                //    _currentRow = 0;
                //    _pageCount = dicAPV.Count / 50 + 1;
                //    _pageCurrent = 1;
                //    bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
                //    bindingNavigatorCountItem.Text = _pageCount.ToString();
                //    //-----------------------------------初始化table--------------------------------------------------------
                //}
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
                    //---------order by column first!
                    for (int iValuation = 0; iValuation < lstallSelectValuationMethodAndValue.Count; iValuation++)
                    {
                        lstallSelectValuationMethodAndValue[iValuation].lstAPVValueAttributes = lstallSelectValuationMethodAndValue[iValuation].lstAPVValueAttributes.OrderBy(p => p.Col).ToList();

                    }
                    //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value", Text = "MethodID", Width = "MethodID".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
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

                            if (fieldCheck.isChecked)
                            {
                                BrightIdeasSoftware.OLVColumn olvColumnAPVID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value." + getFieldNameFromlstAPV(fieldCheck.FieldName), Text = fieldCheck.FieldName, Width = (fieldCheck.FieldName.Length + 2) * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnAPVID);
                            }
                        }

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
                                    //Percentile +  i* count/100[" + i + "]

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
                    //-----------------------------------初始化table--------------------------------------------------------
                }


                else if (oTable is List<AllSelectQALYMethodAndValue> || oTable is AllSelectQALYMethodAndValue)
                {
                    List<AllSelectQALYMethodAndValue> lstallSelectQALYMethodAndValue = new List<AllSelectQALYMethodAndValue>();
                    if (oTable is List<AllSelectQALYMethodAndValue>)
                    {
                        lstallSelectQALYMethodAndValue = (List<AllSelectQALYMethodAndValue>)oTable;
                    }
                    else
                    {
                        lstallSelectQALYMethodAndValue.Add((AllSelectQALYMethodAndValue)oTable);
                    }
                    //-----order by column first!
                    for (int iQALY = 0; iQALY < lstallSelectQALYMethodAndValue.Count; iQALY++)
                    {
                        lstallSelectQALYMethodAndValue[iQALY].lstQALYValueAttributes = lstallSelectQALYMethodAndValue[iQALY].lstQALYValueAttributes.OrderBy(p => p.Col).ToList();
                    }
                    //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value", Text = "MethodID", Width = "MethodID".Length * 8, IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);
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
                                //Percentile +  i* count/100[" + i + "]

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
                    //-----------------------------------初始化table--------------------------------------------------------
                }
                //else if (oTable is AllSelectQALYMethodAndValue)
                //{
                //    AllSelectQALYMethodAndValue allSelectQALYMethodAndValue = (AllSelectQALYMethodAndValue)oTable;
                //    BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value", Text = "MethodID", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);

                //    BrightIdeasSoftware.OLVColumn olvColumnCol = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Col", Text = "Col", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnCol);
                //    BrightIdeasSoftware.OLVColumn olvColumnRow = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Row", Text = "Row", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnRow);
                //    BrightIdeasSoftware.OLVColumn olvColumnPointEstimate = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.PointEstimate", Text = "PointEstimate", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPointEstimate);
                //    BrightIdeasSoftware.OLVColumn olvColumnMean = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Mean", Text = "Mean", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnMean);
                //    BrightIdeasSoftware.OLVColumn olvColumnStandardDeviation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.StandardDeviation", Text = "StandardDeviation", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnStandardDeviation);
                //    BrightIdeasSoftware.OLVColumn olvColumnVariance = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Variance", Text = "Variance", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnVariance);
                //    if (allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile != null)
                //    {
                //        i = 0;
                //        while (i < allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count())
                //        {
                //            //Percentile +  i* count/100[" + i + "]

                //            BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.LstPercentile[" + i + "]", Text = "Percentile" + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(allSelectQALYMethodAndValue.lstQALYValueAttributes.First().LstPercentile.Count()))))), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

                //            i++;
                //        }
                //    }
                //    _tableObject = allSelectQALYMethodAndValue;
                //    Dictionary<QALYValueAttribute, int> dicAPV = new Dictionary<QALYValueAttribute, int>();
                //    foreach (QALYValueAttribute apvx in allSelectQALYMethodAndValue.lstQALYValueAttributes)
                //    {
                //        dicAPV.Add(apvx, allSelectQALYMethodAndValue.AllSelectQALYMethod.ID);

                //    }
                //    OLVResultsShow.SetObjects(dicAPV.ToList().Count >= 50 ? dicAPV.ToList().GetRange(0, 50) : dicAPV.ToList());
                //    _pageSize = 50;
                //    _currentRow = 0;
                //    _pageCount = dicAPV.Count / 50 + 1;
                //    _pageCurrent = 1;
                //    bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
                //    bindingNavigatorCountItem.Text = _pageCount.ToString();
                //}
                //else if (oTable is List<AllSelectQALYMethodAndValue>)
                //{
                //    List<AllSelectQALYMethodAndValue> lstAllSelectQALYMethodAndValue = (List<AllSelectQALYMethodAndValue>)oTable;
                //    BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Value", Text = "MethodID", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnID);

                //    BrightIdeasSoftware.OLVColumn olvColumnCol = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Col", Text = "Col", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnCol);
                //    BrightIdeasSoftware.OLVColumn olvColumnRow = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Row", Text = "Row", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnRow);
                //    BrightIdeasSoftware.OLVColumn olvColumnPointEstimate = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.PointEstimate", Text = "PointEstimate", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnPointEstimate);
                //    BrightIdeasSoftware.OLVColumn olvColumnMean = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Mean", Text = "Mean", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnMean);
                //    BrightIdeasSoftware.OLVColumn olvColumnStandardDeviation = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.StandardDeviation", Text = "StandardDeviation", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnStandardDeviation);
                //    BrightIdeasSoftware.OLVColumn olvColumnVariance = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.Variance", Text = "Variance", IsEditable = false }; OLVResultsShow.Columns.Add(olvColumnVariance);
                //    if (lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile != null)
                //    {
                //        i = 0;
                //        while (i < lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count())
                //        {
                //            //Percentile +  i* count/100[" + i + "]

                //            BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key.LstPercentile[" + i + "]", Text = "Percentile" + ((Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()) - (100.00 / (2 * Convert.ToDouble(lstAllSelectQALYMethodAndValue.First().lstQALYValueAttributes.First().LstPercentile.Count()))))), IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);

                //            i++;
                //        }
                //    }
                //    _tableObject = lstAllSelectQALYMethodAndValue;
                //    Dictionary<QALYValueAttribute, int> dicAPV = new Dictionary<QALYValueAttribute, int>();
                //    foreach (AllSelectQALYMethodAndValue asqav in lstAllSelectQALYMethodAndValue)
                //    {
                //        foreach (QALYValueAttribute apvx in asqav.lstQALYValueAttributes)
                //        {
                //            dicAPV.Add(apvx, asqav.AllSelectQALYMethod.ID);

                //        }
                //    }
                //    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(0, 50));
                //    _pageSize = 50;
                //    _currentRow = 0;
                //    _pageCount = dicAPV.Count / 50 + 1;
                //    _pageCurrent = 1;
                //    bindingNavigatorPositionItem.Text = _pageCurrent.ToString();
                //    bindingNavigatorCountItem.Text = _pageCount.ToString();
                //}
                else if (oTable is BenMAPLine)
                {
                    BenMAPLine crTable = (BenMAPLine)oTable;
                    //----------order by column first!
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
                        //Percentile +  i* count/100[" + i + "]

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
            }
        }

        //private DataTable dtPane = new DataTable();
        private void InitChartResult(object oTable, int GridID)
        {
            try
            {
                //马杰把整个设置成true---------------
                zedGraphCtl.Visible = true;
                //lblRegion.Visible = true;
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

                        CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));// GetBenMapGridDefinitions(drGrid);
                    }
                    if (GridID != CommonClass.RBenMAPGrid.GridDefinitionID)
                    {
                        foreach (GridRelationship gr in CommonClass.LstGridRelationshipAll)
                        {
                            if (gr.bigGridID == CommonClass.RBenMAPGrid.GridDefinitionID && gr.smallGridID == GridID)// CommonClass.GBenMAPGrid.GridDefinitionID)
                            {
                                gRegionGridRelationship = gr;
                            }
                            else if (gr.smallGridID == CommonClass.RBenMAPGrid.GridDefinitionID && gr.bigGridID == GridID)// CommonClass.GBenMAPGrid.GridDefinitionID)
                            {
                                gRegionGridRelationship = gr;
                            }
                        }
                    }
                }
                if (oTable is CRSelectFunctionCalculateValue)
                {
                    //first 50 init columns
                    CRSelectFunctionCalculateValue crTable = (CRSelectFunctionCalculateValue)oTable;
                    //----------------Aggregation------------------------------
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
                    //if (CommonClass.RBenMAPGrid.GridDefinitionID != GridID)
                    //{
                    //    allSelectValuationMethodAndValue = APVX.APVCommonClass.ApplyAllSelectValuationMethodAndValueAggregation(gRegionGridRelationship, Grid.GridCommon.getBenMAPGridFromID(GridID), allSelectValuationMethodAndValue);
                    //}
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
                    //-----------------------------------初始化table--------------------------------------------------------
                }
                else if (oTable is List<AllSelectValuationMethodAndValue>)
                {
                    List<AllSelectValuationMethodAndValue> lstallSelectValuationMethodAndValue = (List<AllSelectValuationMethodAndValue>)oTable;
                    AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.First();// new AllSelectValuationMethodAndValue();
                    //if (CommonClass.RBenMAPGrid.GridDefinitionID != GridID)
                    //{
                    //    allSelectValuationMethodAndValue = APVX.APVCommonClass.ApplyAllSelectValuationMethodAndValueAggregation(gRegionGridRelationship, Grid.GridCommon.getBenMAPGridFromID(GridID), lstallSelectValuationMethodAndValue.First());
                    //}
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
                    //-----------------------------------初始化table--------------------------------------------------------
                }
                #region qaly
                //else if (oTable is AllSelectQALYMethodAndValue)
                //{
                //    AllSelectQALYMethodAndValue allSelectQALYMethodAndValue = (AllSelectQALYMethodAndValue)oTable;
                //    if (CommonClass.RBenMAPGrid.GridDefinitionID != GridID)
                //    {
                //        allSelectQALYMethodAndValue = APVX.APVCommonClass.ApplyAllSelectQALYMethodAndValueAggregation(gRegionGridRelationship, Grid.GridCommon.getBenMAPGridFromID(GridID), allSelectQALYMethodAndValue);
                //    }
                //    foreach (QALYValueAttribute crv in allSelectQALYMethodAndValue.lstQALYValueAttributes)
                //    {
                //        if (!dicValue.Keys.Contains(crv.Col + "," + crv.Row))
                //            dicValue.Add(crv.Col + "," + crv.Row, crv.PointEstimate);
                //    }
                //    strchartTitle = "QALY Result";
                //    strchartX = "Region";
                //    strchartY = "QALY";
                //}
                //else if (oTable is List<AllSelectQALYMethodAndValue>)
                //{
                //    AllSelectQALYMethodAndValue allSelectQALYMethodAndValue = ((List<AllSelectQALYMethodAndValue>)oTable).First();
                //    if (CommonClass.RBenMAPGrid.GridDefinitionID != GridID)
                //    {
                //        allSelectQALYMethodAndValue = APVX.APVCommonClass.ApplyAllSelectQALYMethodAndValueAggregation(gRegionGridRelationship, Grid.GridCommon.getBenMAPGridFromID(GridID), allSelectQALYMethodAndValue);
                //    }
                //    foreach (QALYValueAttribute crv in allSelectQALYMethodAndValue.lstQALYValueAttributes)
                //    {
                //        if (!dicValue.Keys.Contains(crv.Col + "," + crv.Row))
                //            dicValue.Add(crv.Col + "," + crv.Row, crv.PointEstimate);
                //    }
                //    strchartTitle = "QALY Result";
                //    strchartX = "Region";
                //    strchartY = "QALY";
                //}
                #endregion
                //---------------------首先统计成Region需要的数据---------------------
                //i = 0;
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
                //switch (cbGraph.Text)
                //{
                //    case "Bar Graph":
                        #region Bar graph
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
                        if (1 == 1)//CommonClass.RBenMAPGrid.GridDefinitionID == CommonClass.GBenMAPGrid.GridDefinitionID)
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
                            //Dictionary<string, double> dicBigGrid = new Dictionary<string, double>();
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
                                    //lstString = gridRelationshipAttribute.smallGridRowCol.Select(a => a.Col + "," + a.Row).ToList();
                                    ////var v=
                                    ////d = dicValue.ToList().Where(p => lstString.Contains(p.Key)).Sum(a => a.Value);
                                    ////if (d != 0)
                                    ////{
                                    ////    d = d / Convert.ToDouble(lstString.Count);
                                    ////}
                                    ////  IDicValue=dicValue.Where(p => lstString.Contains(p.Key));
                                    ////  crResult = CommonClass.lstChartResult.Where(p => p.Col == gridRelationshipAttribute.bigGridRowCol.Col
                                    ////    && p.Row == gridRelationshipAttribute.bigGridRowCol.Row).First();
                                    ////if (IDicValue.Count() > 0)
                                    ////{
                                    ////    foreach (KeyValuePair<string, double> k in IDicValue)
                                    ////    {
                                    ////        crResult.RegionValue += k.Value;
                                    ////    }
                                    ////}
                                    //CommonClass.lstChartResult.Where(p => p.Col == gridRelationshipAttribute.bigGridRowCol.Col
                                    //    && p.Row == gridRelationshipAttribute.bigGridRowCol.Row).First().RegionValue = dicValue.Where(p => lstString.Contains(p.Key)).Sum(a => a.Value);
                                    //dicBigGrid.Add(gridRelationshipAttribute.bigGridRowCol.Col + "," + gridRelationshipAttribute.bigGridRowCol.Row, d);
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
                                            CommonClass.lstChartResult[i].RegionValue = 0.00;// Math.Round(dicValue[rc.Col + "," + rc.Row], 4);
                                        }
                                    }
                                    catch (Exception ex)
                                    { }
                                }
                                i++;
                            }
                        }

                        //-----------------马杰 新加一个List<string> 生成所有的Name字符List;循环list-->新增checkbox-->Region Panel-->top left; >5 checked=true
                        //Dictionary<int, string> DicAllRegions = new Dictionary<int, string>();
                        List<string> lstPane = new List<string>();
                        //for (int l = 0; l < dt.Rows.Count; l++)
                        //{
                        //    if (!DicAllRegions.Values.Contains(dt.Rows[l]["Name"].ToString()))
                        //    {
                        //        DicAllRegions.Add(l, dt.Rows[l]["Name"].ToString());
                        //    }
                        //}
                        //lvRegions.Items.Clear();
                        //i = 0;
                        //foreach (KeyValuePair<int,string> region in DicAllRegions)
                        //{
                        //    ListViewItem item = new ListViewItem() { Text = region.Value, Checked = false };
                        //    if (i < iRowCount)
                        //    {
                        //        item.Checked = true;
                        //    }
                        //   // lvRegions.Items.Add(item);
                        //    i++;

                        //}
                        //var vRegion = CommonClass.lstChartResult.Select(p => new { RegionName = p.RegionName });
                        olvRegions.SetObjects(CommonClass.lstChartResult);

                        //-----------------有 一个 变量 dt------------------------
                        //dtPane = dt;
                        olvRegions.MultiSelect = true;
                        if (CommonClass.lstChartResult.Count > 5)
                        {
                            //for (int k = 0; k < 5; k++)
                            //{
                            //    //lvRegions.Items[k].Checked = true;
                            //    lstPane.Add(  DicAllRegions.Values.ToList()[k]);
                            //}
                            lstPane = CommonClass.lstChartResult.Select(p => p.RegionName).ToList().GetRange(0, 5);
                            //olvRegions.SelectedObjects = DicAllRegions.ToList().GetRange(0, 5);
                            //for (int k = 0; k < 5; k++)
                            //{
                            //    //olvRegions.CheckObject(olvRegions.Items[k]);
                            //    olvRegions.Items[k].Checked = true;

                            //}
                            //olvRegions.CheckBoxes = true;
                            //olvRegions.CheckedObjects = CommonClass.lstChartResult.GetRange(0, 5);
                            for (int j = 0; j < 5; j++)
                            {
                                OLVListItem olvi = olvRegions.Items[j] as OLVListItem;
                                olvi.Checked = true;

                            }
                        }
                        else
                        {
                            lstPane = CommonClass.lstChartResult.Select(p => p.RegionName).ToList();
                            //for (int k = 0; k < olvRegions.Items.Count; k++)
                            //{
                            //    //olvRegions.Items[k].Checked = true;
                            //    olvRegions.CheckObject(olvRegions.Items[k]);

                            //}
                            //olvRegions.CheckedObjects = CommonClass.lstChartResult;
                            for (int j = 0; j < CommonClass.lstChartResult.Count; j++)
                            {
                                OLVListItem olvi = olvRegions.Items[j] as OLVListItem;
                                olvi.Checked = true;

                            }
                            // olvRegions.CheckedObjects = CommonClass.lstChartResult.Select(p => new { RegionName = p.RegionName }).ToList();
                        }
                        #endregion

                        #region 画bar--不用写在这里
                        ////画图-----------------------------------------------------------------------------------

                        //myPane.CurveList.Clear();
                        //i = 0;
                        //Color[] colorArray = new Color[] { Color.Blue, Color.Red, Color.Green };
                        //while (i < 1)
                        //{
                        //    PointPairList list = new PointPairList();
                        //    int j = 0;
                        //    while (j < iRowCount)
                        //    {
                        //        list.Add(new PointPair(Convert.ToInt32(j), CommonClass.lstChartResult[j].RegionValue));
                        //        j++;
                        //    }

                        //    BarItem myCurve = myPane.AddBar("Result", list, colorArray[i]);
                        //    //  myPane.AddBar(myCurve);

                        //    i++;
                        //}
                        //myPane.Chart.Fill = new Fill(Color.White,
                        // Color.FromArgb(255, 255, 166), 45.0F);

                        //// expand the range of the Y axis slightly to accommodate the labels
                        ////myPane.YAxis.Scale.Max += myPane.YAxis.Scale.MajorStep;
                        //// myPane.YAxis.Scale.TextLabels = strValuationsNow.ToArray();
                        ////List<string> lstTextLabels = new List<string>();
                        ////for (int ij = 0; ij < iRowCount; ij++)
                        ////{
                        ////    lstTextLabels.Add(dt.Rows[ij]["Name"].ToString());
                        ////}
                        //myPane.YAxis.Scale.MinAuto = true;//用数据最小最大值重新设置下横纵坐标轴的显示范围
                        //myPane.YAxis.Scale.MaxAuto = true;
                        //myPane.XAxis.Scale.MinAuto = true;
                        //myPane.XAxis.Scale.MaxAuto = true;
                        //myPane.YAxis.Scale.Format = "#,##0.####";//y轴的数字格式加上千位分隔逗号
                        //myPane.XAxis.Scale.TextLabels = lstPane.ToArray(); //lstTextLabels.ToArray();
                        //// myPane.XAxis.Scale.TextLabels = new string[] { dt.Rows[0]["Name"].ToString(), dt.Rows[1]["Name"].ToString(), dt.Rows[2]["Name"].ToString(), dt.Rows[3]["Name"].ToString(), dt.Rows[4]["Name"].ToString(), };
                        ////myPane.YAxis.Type= AxisType.
                        //myPane.XAxis.Type = AxisType.Text;
                        //myPane.XAxis.Scale.FontSpec.Angle = 65;
                        //myPane.XAxis.Scale.FontSpec.IsBold = true;
                        //myPane.XAxis.Scale.FontSpec.Size = 12;
                        //// Create TextObj's to provide labels for each bar
                        ////  BarItem.CreateBarLabels(myPane, false, "f0");
                        //// zgc.Controls.Add(myPane);
                        ////zgc.GraphPane = myPane;
                        //zedGraphCtl.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;

                        //zedGraphCtl.AxisChange();
                        //zedGraphCtl.Refresh();
                        #endregion

                        #region 把shapefile里面每个字段加到cbChartXAxis
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
                        #endregion

                        //    break;
                    //case "Cumulative Distribution Functions":
                if(cbGraph.Text=="Cumulative Distribution Functions")
                        cbGraph_SelectedIndexChanged(null, null);
                        //break;
                //}

            }
            catch (Exception ex)
            {
            }
        }


        private void UpdateTableResult(object oTable)
        {

            //if (oTable is CRSelectFunctionCalculateValue)
            //{
            //    //first 50 init columns
            //    CRSelectFunctionCalculateValue crTable = (CRSelectFunctionCalculateValue)oTable;
            //    if (_pageCurrent == _pageCount)

            //        OLVResultsShow.SetObjects(crTable.CRCalculateValues.GetRange(_pageCurrent * 50 - 50, crTable.CRCalculateValues.Count - (_pageCurrent - 1) * 50));
            //    else
            //        OLVResultsShow.SetObjects(crTable.CRCalculateValues.GetRange(_pageCurrent * 50 - 50, 50));
            //}
            //else 
            if (oTable is List<AllSelectCRFunction>)
            {
                List<AllSelectCRFunction> lstAllSelectCRFuntion = (List<AllSelectCRFunction>)oTable;
                 
                Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction> dicAPV = new Dictionary<KeyValuePair<CRCalculateValue, int>, AllSelectCRFunction>();// new Dictionary<CRCalculateValue, CRSelectFunction>();
                int iLstCRTable = 0;
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
                //first 50 init columns

                List<CRSelectFunctionCalculateValue> lstCRTable = new List<CRSelectFunctionCalculateValue>();
                if (oTable is List<CRSelectFunctionCalculateValue>)
                    lstCRTable = (List<CRSelectFunctionCalculateValue>)oTable;
                else
                    lstCRTable.Add(oTable as CRSelectFunctionCalculateValue);
                Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicAPV = new Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>();// new Dictionary<CRCalculateValue, CRSelectFunction>();
                int iLstCRTable = 0;
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
                            //---------得到Base------
                            BenMAPLine benMAPLineBase = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == crNew.BenMAPHealthImpactFunction.Pollutant.PollutantID).First().Base;
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
                            //---------得到Base------
                            BenMAPLine benMAPLineControl = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.Where(p => p.Pollutant.PollutantID == crNew.BenMAPHealthImpactFunction.Pollutant.PollutantID).First().Control;
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
                foreach (CRSelectFunctionCalculateValue cr in lstCRTable)
                {
                    foreach (CRCalculateValue crv in cr.CRCalculateValues)
                    {
                        dicKey = null;
                        dicKey = new Dictionary<CRCalculateValue, int>();
                        dicKey.Add(crv, iLstCRTable);
                        dicAPV.Add(dicKey.ToList()[0], cr.CRSelectFunction);
                    }
                    iLstCRTable++;
                }



                if (_pageCurrent == _pageCount)

                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, dicAPV.Count - (_pageCurrent - 1) * 50));
                else
                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, 50));

            }
            else if (oTable is Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>)
            {
                Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction> dicAPV = oTable as Dictionary<KeyValuePair<CRCalculateValue, int>, CRSelectFunction>;
                if (_pageCurrent == _pageCount)

                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, dicAPV.Count - (_pageCurrent - 1) * 50));
                else
                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, 50));
            }
            //else if (oTable is AllSelectValuationMethodAndValue)
            //{
            //    AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = (AllSelectValuationMethodAndValue)oTable;
            //    //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key", Text = "ID" }; OLVResultsShow.Columns.Add(olvColumnID);

        //    Dictionary<APVValueAttribute, int> dicAPV = new Dictionary<APVValueAttribute, int>();
            //    foreach (APVValueAttribute apvx in allSelectValuationMethodAndValue.lstAPVValueAttributes)
            //    {
            //        dicAPV.Add(apvx, allSelectValuationMethodAndValue.AllSelectValuationMethod.ID);
            //    }
            //    if (_pageCurrent == _pageCount)

        //        OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, dicAPV.ToList().Count - (_pageCurrent - 1) * 50));
            //    else
            //        OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, 50));
            //    //-----------------------------------初始化table--------------------------------------------------------
            //}
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
                //BrightIdeasSoftware.OLVColumn olvColumnID = new BrightIdeasSoftware.OLVColumn() { AspectName = "Key", Text = "ID" }; OLVResultsShow.Columns.Add(olvColumnID);


                Dictionary<QALYValueAttribute, AllSelectQALYMethod> dicAPV = new Dictionary<QALYValueAttribute, AllSelectQALYMethod>();

                foreach (QALYValueAttribute apvx in allSelectQALYMethodAndValue.lstQALYValueAttributes)
                {
                    dicAPV.Add(apvx, allSelectQALYMethodAndValue.AllSelectQALYMethod);
                }


                if (_pageCurrent == _pageCount)

                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, dicAPV.ToList().Count - (_pageCurrent - 1) * 50));
                else
                    OLVResultsShow.SetObjects(dicAPV.ToList().GetRange(_pageCurrent * 50 - 50, 50));
                //-----------------------------------初始化table--------------------------------------------------------
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
                            //MessageBox.Show("It's the first page, click 'Move to Next Page' to view!\t", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            //MessageBox.Show("It's the last page, click 'Move to Previous Page' to view!\t", "Tip", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                catch
                {
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
                    //if (vb.IncidencePoolingAndAggregation.PoolingMethodType == PoolingMethodTypeEnum.None)
                    //{
                    //    MessageBox.Show("The Pooling Method Type can not be null!");
                    //    return;
                    //}
                }
                else
                {
                    if (vb.IncidencePoolingAndAggregation == null)
                    {
                        MessageBox.Show("No result.");
                        return;
                    }
                    //if (vb.IncidencePoolingAndAggregation.PoolingMethodType == PoolingMethodTypeEnum.None)
                    //{
                    //    MessageBox.Show("The Pooling Method Type can not be null!");
                    //    return;
                    //}
                }
                //----------------------加载----

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
                        tsbChangeProjection.Text = "Change projection to Albers";
                        mainMap.ProjectionModeReproject = ActionMode.Never;
                        mainMap.ProjectionModeDefine = ActionMode.Never;
                        string shapeFileName = "";
                        if (chbAPVAggregation.Checked)
                        {
                            if (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation is ShapefileGrid)
                            {
                                mainMap.Layers.Clear();
                                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation as ShapefileGrid).ShapefileName + ".shp"))
                                {
                                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation as ShapefileGrid).ShapefileName + ".shp";
                                    //mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation as ShapefileGrid).ShapefileName + ".shp");
                                }
                            }
                            else if (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation is RegularGrid)
                            {
                                mainMap.Layers.Clear();
                                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation as RegularGrid).ShapefileName + ".shp"))
                                {
                                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation as RegularGrid).ShapefileName + ".shp";
                                    // mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation as RegularGrid).ShapefileName + ".shp");
                                }
                            }
                        }
                        else
                        {
                            if (CommonClass.GBenMAPGrid is ShapefileGrid)
                            {
                                mainMap.Layers.Clear();
                                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                                {
                                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
                                    //mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp");
                                }
                            }
                            else if (CommonClass.GBenMAPGrid is RegularGrid)
                            {
                                mainMap.Layers.Clear();
                                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp"))
                                {
                                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp";
                                    // mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp");
                                }
                            }
                        }

                        IFeatureSet fs = FeatureSet.Open(shapeFileName);

                        //加值
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
                        //匹配
                        foreach (DataRow dr in fs.DataTable.Rows)
                        {
                            try
                            {
                                //dr["Value"] = crSelectFunctionCalculateValue.CRCalculateValues.Where(p => p.Col == Convert.ToInt32(dr[iCol])
                                //    && p.Row == Convert.ToInt32(dr[iRow])).Select(p => p.PointEstimate).First();
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
                        mainMap.Layers.Clear();
                        mainMap.Layers.Add(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp");
                        MapPolygonLayer polLayer = mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer;
                        string strValueField = polLayer.DataSet.DataTable.Columns[polLayer.DataSet.DataTable.Columns.Count - 1].ColumnName;
                        PolygonScheme myScheme1 = new PolygonScheme();
                        float fl = (float)0.1;
                        myScheme1.EditorSettings.StartColor = Color.Blue;
                        myScheme1.EditorSettings.StartColor.ToTransparent(fl);
                        myScheme1.EditorSettings.EndColor = Color.Red;
                        myScheme1.EditorSettings.EndColor.ToTransparent(fl);

                        myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
                        //myScheme1.EditorSettings.ClassificationType = ClassificationType.UniqueValues ;
                        myScheme1.EditorSettings.NumBreaks = 6;
                        myScheme1.EditorSettings.FieldName = strValueField;// "Value";
                        myScheme1.EditorSettings.UseGradient = false;
                        myScheme1.CreateCategories(polLayer.DataSet.DataTable);

                        //PolygonCategory pc = new PolygonCategory(Color.LightBlue, Color.DarkBlue, 1);
                        //pc.FilterExpression = "[Value] < 6.5 ";
                        //pc.LegendText = "0-6.5";
                        //myScheme1.AddCategory(pc);
                        //----------
                        //UniqueValues+半透明
                        double dMinValue = 0.0;
                        double dMaxValue = 0.0;
                        //Base或者Control，默认都按Base的区间来画，以保证Base和Control都是同一区间
                        dMinValue = crSelectFunctionCalculateValue.CRCalculateValues.Min(a => a.PointEstimate);
                        dMaxValue = crSelectFunctionCalculateValue.CRCalculateValues.Max(a => a.PointEstimate);
                        _dMinValue = dMinValue;
                        _dMaxValue = dMaxValue;
                        _currentLayerIndex = mainMap.Layers.Count - 1;
                        _columnName = strValueField;
                        RenderMainMap(true);

                        //Color[] colors = new Color[] { Color.Blue, Color.FromArgb(0, 255, 255), Color.FromArgb(0, 255, 0), Color.Yellow, Color.Red, Color.FromArgb(255, 0, 255) };
                        ////Quantities+半透明
                        //PolygonCategoryCollection pcc = new PolygonCategoryCollection();
                        //int iColor = 0;

                        //foreach (PolygonCategory pc in myScheme1.Categories)
                        //{
                        //    //pc.Symbolizer.SetOutlineWidth(0);
                        //    PolygonCategory pcin = pc;
                        //    double dnow = dMinValue + ((dMaxValue - dMinValue) / 6.00) * Convert.ToDouble(iColor);
                        //    double dnowUp = dMinValue + ((dMaxValue - dMinValue) / 6.00) * Convert.ToDouble(iColor + 1);
                        //    if (iColor == 0)
                        //    {
                        //     //   dnow = dnow - 1.000000;
                        //        pcin.FilterExpression = string.Format("[{0}]<" + dnowUp, strValueField);
                        //        pcin.LegendText = string.Format("[{0}]<" + dnowUp.ToString("E2"), strValueField);// string.Format("{0} >= " + dnow.ToString("E2") + " and {0} < " + dnowUp.ToString("E2"), strValueField);
                        //    }
                        //    else if (iColor == myScheme1.Categories.Count - 1)
                        //    {
                        //        //dnowUp = dnowUp + 1.000000;
                        //        pcin.FilterExpression = string.Format("[{0}]>" + dnow, strValueField);
                        //        pcin.LegendText = string.Format("[{0}]>" + dnow.ToString("E2"), strValueField);// string.Format("{0} >= " + dnow.ToString("E2") + " and {0} < " + dnowUp.ToString("E2"), strValueField);

                        //    }
                        //    else
                        //    {
                        //        pcin.FilterExpression = string.Format("[{0}]>=" + dnow + " and [{0}] <" + dnowUp, strValueField);
                        //        pcin.LegendText = string.Format("{0} >= " + dnow.ToString("E2") + " and {0} < " + dnowUp.ToString("E2"), strValueField);
                        //    }

                        //    pcin.Symbolizer.SetOutline(Color.Transparent, 0);
                        //    Color ctemp = pcin.Symbolizer.GetFillColor();
                        //    float fColor = (float)0.2;
                        //    ctemp.ToTransparent(fColor);
                        //    pcin.Symbolizer.SetFillColor(colors[iColor]);
                        //    pcc.Add(pcin);
                        //    iColor++;
                        //}
                        //myScheme1.ClearCategories();
                        //foreach (PolygonCategory pct in pcc)
                        //{
                        //    myScheme1.Categories.Add(pct);
                        //}
                        ////player.Symbology = myScheme1;

                        //polLayer.Symbology = myScheme1;
                        addRegionLayerToMainMap();
                    }
                    WaitClose();
                }
            }
            catch
            {
                WaitClose();
            }
        }

        #endregion table Result show
        /// <summary>
        /// 未用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabCtlReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabCtlMain.SelectedIndex == 3 && tabCtlReport.SelectedIndex < 5)
            {
                tabCtlMain.SelectedIndex = 0;
            }
            if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabPoolingIncidence")
            {
                //-------------改成All---
                // rbIncidenceAll.Checked = true;
                olvIncidence.SelectAll();
                tlvIncidence_DoubleClick(sender, e);
            }
            if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabAPVResultGISShow")
            {
                //-------------改成All---
                // rbIncidenceAll.Checked = true;
                tlvAPVResult.SelectAll();
                tlvAPVResult_DoubleClick(sender, e);
            }
            //if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabQLAYResultShow")
            //{
            //    //-------------改成All---
            //    // rbIncidenceAll.Checked = true;
            //    //tlvQALYResult.SelectAll();
            //    this.tlvQALYResult_DoubleClick(sender, e);
            //}
        }
        /// <summary>
        /// QALY结果列表双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void tlvQALYResult_DoubleClick(object sender, EventArgs e)
        //{

        //    try
        //    {
        //        ClearMapTableChart();
        //        if (tlvQALYResult.SelectedObjects.Count == 0) return;

        //        string Tip = "Drawing pooled QALY results layer";
        //        WaitShow(Tip);
        //        bool bGIS = true;
        //        bool bTable = true;
        //        bool bChart = true;
        //        List<AllSelectQALYMethodAndValue> lstallSelectQALYMethodAndValue = new List<AllSelectQALYMethodAndValue>();
        //        if (CommonClass.IncidencePoolingAndAggregationAdvance == null || CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation == null)
        //        {
        //            chbQALYAggregation.Checked = false;

        //        }
        //        else
        //        {
        //            chbQALYAggregation.Checked = true;

        //        }
        //        //----------modify by xiejp 20120618--for multi-Pooling Window-- multi-Select

        //        if (sender is ObjectListView || sender is Button)
        //        {
        //            foreach (KeyValuePair<AllSelectQALYMethod, string> keyValue in tlvQALYResult.SelectedObjects)
        //            {
        //                AllSelectQALYMethod allSelectQALYMethod = keyValue.Key;
        //                if (rbQALYOnlyOne.Checked)//--Only one Pooling Window
        //                {
        //                    //-----------------首先判断是否有结果-------------
        //                    ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == keyValue.Value).First();

        //                    AllSelectQALYMethodAndValue allSelectQALYMethodAndValue = null;

        //                    try
        //                    {
        //                        if (vb.lstAllSelectQALYMethodAndValueAggregation == null || vb.lstAllSelectQALYMethodAndValueAggregation.Count == 0)
        //                        {

        //                            allSelectQALYMethodAndValue = vb.lstAllSelectQALYMethodAndValue.Where(p => p.AllSelectQALYMethod.ID == allSelectQALYMethod.ID).First();

        //                        }
        //                        else
        //                        {

        //                            allSelectQALYMethodAndValue = vb.lstAllSelectQALYMethodAndValueAggregation.Where(p => p.AllSelectQALYMethod.ID == allSelectQALYMethod.ID).First();
        //                        }
        //                        if (allSelectQALYMethodAndValue != null && allSelectQALYMethodAndValue.lstQALYValueAttributes != null && allSelectQALYMethodAndValue.lstQALYValueAttributes.Count() > 0)
        //                            lstallSelectQALYMethodAndValue.Add(allSelectQALYMethodAndValue);
        //                    }
        //                    catch
        //                    { }
        //                }
        //                else
        //                {
        //                    //----------find Pooling Window first! ----
        //                    AllSelectQALYMethodAndValue allSelectQALYMethodAndValue = null;
        //                    if (allSelectQALYMethod.ID < 0) continue;
        //                    //int iMaxID = CommonClass.QALYMethodPoolingAndAggregation.lstQALYMethodPoolingAndAggregationBase.Max(p => p.LstAllSelectQALYMethod.Max(a => a.ID)) + 1;
        //                    //int iPoolingWindow = allSelectQALYMethod.ID / iMaxID;

        //                    ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == keyValue.Value).First();

        //                    try
        //                    {


        //                        allSelectQALYMethodAndValue = vb.lstAllSelectQALYMethodAndValue.Where(p => p.AllSelectQALYMethod.ID == allSelectQALYMethod.ID).First();

        //                        if (allSelectQALYMethodAndValue != null && allSelectQALYMethodAndValue.lstQALYValueAttributes != null && allSelectQALYMethodAndValue.lstQALYValueAttributes.Count() > 0)
        //                            lstallSelectQALYMethodAndValue.Add(allSelectQALYMethodAndValue);
        //                    }
        //                    catch
        //                    {
        //                    }
        //                }
        //            }
        //            tabCtlMain.SelectedIndex = 1;
        //            if (tlvQALYResult.SelectedObjects.Count > 1)
        //            {

        //                bGIS = false;
        //                bChart = false;
        //            }
        //        }
        //        else
        //        {
        //            bGIS = false;
        //            bChart = false;
        //            tabCtlMain.SelectedIndex = 1;
        //            foreach (KeyValuePair<AllSelectQALYMethod, string> keyValue in tlvQALYResult.Objects)
        //            {
        //                AllSelectQALYMethod allSelectQALYMethod = keyValue.Key;
        //                if (rbQALYOnlyOne.Checked)//--Only one Pooling Window
        //                {
        //                    //-----------------首先判断是否有结果-------------
        //                    ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == keyValue.Value).First();

        //                    AllSelectQALYMethodAndValue allSelectQALYMethodAndValue = null;

        //                    try
        //                    {
        //                        if (vb.lstAllSelectQALYMethodAndValueAggregation == null || vb.lstAllSelectQALYMethodAndValueAggregation.Count == 0)
        //                        {

        //                            allSelectQALYMethodAndValue = vb.lstAllSelectQALYMethodAndValue.Where(p => p.AllSelectQALYMethod.ID == allSelectQALYMethod.ID).First();

        //                        }
        //                        else
        //                        {

        //                            allSelectQALYMethodAndValue = vb.lstAllSelectQALYMethodAndValueAggregation.Where(p => p.AllSelectQALYMethod.ID == allSelectQALYMethod.ID).First();
        //                        }
        //                        if (allSelectQALYMethodAndValue != null && allSelectQALYMethodAndValue.lstQALYValueAttributes != null && allSelectQALYMethodAndValue.lstQALYValueAttributes.Count() > 0)
        //                            lstallSelectQALYMethodAndValue.Add(allSelectQALYMethodAndValue);
        //                    }
        //                    catch
        //                    { }
        //                }
        //                else
        //                {
        //                    //----------find Pooling Window first! ----
        //                    AllSelectQALYMethodAndValue allSelectQALYMethodAndValue = null;
        //                    //if (allSelectQALYMethod.ID < 0) continue;
        //                    //int iMaxID = CommonClass.QALYMethodPoolingAndAggregation.lstQALYMethodPoolingAndAggregationBase.Max(p => p.LstAllSelectQALYMethod.Max(a => a.ID)) + 1;
        //                    //int iPoolingWindow = allSelectQALYMethod.ID / iMaxID;

        //                    ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == keyValue.Value).First();
        //                    try
        //                    {


        //                        allSelectQALYMethodAndValue = vb.lstAllSelectQALYMethodAndValue.Where(p => p.AllSelectQALYMethod.ID == allSelectQALYMethod.ID).First();

        //                        if (allSelectQALYMethodAndValue != null && allSelectQALYMethodAndValue.lstQALYValueAttributes != null && allSelectQALYMethodAndValue.lstQALYValueAttributes.Count() > 0)
        //                            lstallSelectQALYMethodAndValue.Add(allSelectQALYMethodAndValue);
        //                    }
        //                    catch
        //                    {
        //                    }
        //                }
        //            }
        //        }
        //        //---------------------------清空原有数据 Map/Table/Chart--------------------------------------------
        //        ClearMapTableChart();
        //        if (this.rbShowActiveQALY.Checked)
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
        //        //-----------------------chart未写---------------------------------------------
        //        if (lstallSelectQALYMethodAndValue == null)
        //        {
        //            WaitClose();
        //            MessageBox.Show("No result in this method. It might have been pooled before!");
        //            return;
        //        }

        //        else if (lstallSelectQALYMethodAndValue.Count == 0)
        //        {
        //            WaitClose();
        //            MessageBox.Show("No result in this method. It might have been pooled before!");
        //            return;
        //        }
        //        BenMAPGrid benMapGridShow = null;

        //        if (bChart && lstallSelectQALYMethodAndValue != null)
        //        {

        //            InitChartResult(lstallSelectQALYMethodAndValue, CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation != null ? CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID : CommonClass.GBenMAPGrid.GridDefinitionID);
        //        }
        //        //------------modify by xiejp 2012 0302 加入Aggregation---------

        //        if (lstallSelectQALYMethodAndValue != null)
        //        {
        //            if (this.cbQALYAggregation.SelectedIndex != -1 && cbQALYAggregation.SelectedIndex != 0)
        //            {
        //                int idCbo = Convert.ToInt32((cbQALYAggregation.SelectedItem as DataRowView)["GridDefinitionID"]);
        //                int idAggregation = -1;
        //                GridRelationship gridRelationship = null;
        //                if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation != null)
        //                {
        //                    idAggregation = CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID;

        //                    if (CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idCbo && p.smallGridID == idAggregation).Count() > 0)
        //                    {
        //                        gridRelationship = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idCbo && p.smallGridID == idAggregation).First();
        //                    }
        //                    else
        //                    {
        //                        CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idAggregation && p.smallGridID == idCbo).First();

        //                    }
        //                }
        //                else
        //                {
        //                    if (CommonClass.GBenMAPGrid.GridDefinitionID == idCbo)
        //                    {
        //                    }
        //                    else if (CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idCbo && p.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID).Count() > 0)
        //                    {
        //                        gridRelationship = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == idCbo && p.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID).First();
        //                    }
        //                    else
        //                    {
        //                        gridRelationship = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == CommonClass.GBenMAPGrid.GridDefinitionID && p.smallGridID == idCbo).First();

        //                    }
        //                }

        //                if (idCbo != idAggregation)
        //                {
        //                    List<AllSelectQALYMethodAndValue> lstTmp = new List<AllSelectQALYMethodAndValue>();
        //                    foreach (AllSelectQALYMethodAndValue asvm in lstallSelectQALYMethodAndValue)
        //                    {
        //                        lstTmp.Add(APVX.APVCommonClass.ApplyAllSelectQALYMethodAndValueAggregation(gridRelationship, idAggregation == -1 ? CommonClass.GBenMAPGrid : CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation, asvm));
        //                    }
        //                    lstallSelectQALYMethodAndValue = lstTmp;
        //                    benMapGridShow = Grid.GridCommon.getBenMAPGridFromID(idCbo);
        //                }

        //            }

        //        }

        //        if (bTable && lstallSelectQALYMethodAndValue != null)
        //        {
        //            InitTableResult(lstallSelectQALYMethodAndValue);
        //        }

        //        if (bGIS)
        //        {
        //            //do
        //            tsbChangeProjection.Text = "change projection to Albers";
        //            mainMap.ProjectionModeReproject = ActionMode.Never;
        //            mainMap.ProjectionModeDefine = ActionMode.Never;
        //            string shapeFileName = "";
        //            if (!chbQALYAggregation.Checked)
        //            {
        //                if (CommonClass.GBenMAPGrid is ShapefileGrid)
        //                {
        //                    mainMap.Layers.Clear();
        //                    if (File.Exists(Application.StartupPath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
        //                    {
        //                        shapeFileName = Application.StartupPath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
        //                        //mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp");
        //                    }
        //                }
        //                else if (CommonClass.GBenMAPGrid is RegularGrid)
        //                {
        //                    mainMap.Layers.Clear();
        //                    if (File.Exists(Application.StartupPath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp"))
        //                    {
        //                        shapeFileName = Application.StartupPath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp";
        //                        // mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp");
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation is ShapefileGrid)
        //                {
        //                    mainMap.Layers.Clear();
        //                    if (File.Exists(Application.StartupPath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation as ShapefileGrid).ShapefileName + ".shp"))
        //                    {
        //                        shapeFileName = Application.StartupPath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation as ShapefileGrid).ShapefileName + ".shp";
        //                        //mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.QALYMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation as ShapefileGrid).ShapefileName + ".shp");
        //                    }
        //                }
        //                else if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation is RegularGrid)
        //                {
        //                    mainMap.Layers.Clear();
        //                    if (File.Exists(Application.StartupPath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation as RegularGrid).ShapefileName + ".shp"))
        //                    {
        //                        shapeFileName = Application.StartupPath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation as RegularGrid).ShapefileName + ".shp";
        //                        // mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.QALYMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation as RegularGrid).ShapefileName + ".shp");
        //                    }
        //                }
        //            }
        //            if (benMapGridShow != null)
        //            {
        //                shapeFileName = Application.StartupPath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + ((benMapGridShow is ShapefileGrid) ? (benMapGridShow as ShapefileGrid).ShapefileName : (benMapGridShow as RegularGrid).ShapefileName) + ".shp";
        //            }
        //            mainMap.Layers.Add(shapeFileName);
        //            IFeatureSet fs = (mainMap.Layers[0] as MapPolygonLayer).DataSet;
        //            //fs.Open(shapeFileName);
        //            (mainMap.Layers[0] as MapPolygonLayer).Name = "QALYResult";
        //            (mainMap.Layers[0] as MapPolygonLayer).LegendText = "QALYResult";

        //            //加值
        //            int j = 0;
        //            int iCol = 0;
        //            int iRow = 0;
        //            List<string> lstRemoveName = new List<string>();
        //            while (j < fs.DataTable.Columns.Count)
        //            {
        //                if (fs.DataTable.Columns[j].ColumnName.ToLower() == "col") iCol = j;
        //                if (fs.DataTable.Columns[j].ColumnName.ToLower() == "row") iRow = j;

        //                j++;
        //            }
        //            j = 0;

        //            while (j < fs.DataTable.Columns.Count)
        //            {
        //                if (fs.DataTable.Columns[j].ColumnName.ToLower() == "col" || fs.DataTable.Columns[j].ColumnName.ToLower() == "row")
        //                { }
        //                else
        //                    lstRemoveName.Add(fs.DataTable.Columns[j].ColumnName);

        //                j++;
        //            }
        //            foreach (string s in lstRemoveName)
        //            {
        //                fs.DataTable.Columns.Remove(s);
        //            }
        //            fs.DataTable.Columns.Add("Value", typeof(double));
        //            j = 0;
        //            while (j < fs.DataTable.Columns.Count)
        //            {
        //                if (fs.DataTable.Columns[j].ColumnName.ToLower() == "col") iCol = j;
        //                if (fs.DataTable.Columns[j].ColumnName.ToLower() == "row") iRow = j;

        //                j++;
        //            }
        //            j = 0;
        //            Dictionary<string, double> dicAll = new Dictionary<string, double>();
        //            foreach (QALYValueAttribute crcv in lstallSelectQALYMethodAndValue.First().lstQALYValueAttributes)
        //            {
        //                if (!dicAll.ContainsKey(crcv.Col + "," + crcv.Row))
        //                    dicAll.Add(crcv.Col + "," + crcv.Row, crcv.PointEstimate);
        //            }
        //            //匹配
        //            foreach (DataRow dr in fs.DataTable.Rows)
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
        //            fs.SaveAs(Application.StartupPath + @"\Result\QALYTemp.shp", true);
        //            //fs.Dispose();
        //            mainMap.Layers.Clear();
        //            mainMap.Layers.Add(Application.StartupPath + @"\Result\QALYTemp.shp");
        //            MapPolygonLayer polLayer = mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer;
        //            string strValueField = polLayer.DataSet.DataTable.Columns[polLayer.DataSet.DataTable.Columns.Count - 1].ColumnName;
        //            PolygonScheme myScheme1 = new PolygonScheme();
        //            float fl = (float)0.1;
        //            myScheme1.EditorSettings.StartColor = Color.Blue;
        //            myScheme1.EditorSettings.StartColor.ToTransparent(fl);
        //            myScheme1.EditorSettings.EndColor = Color.Red;
        //            myScheme1.EditorSettings.EndColor.ToTransparent(fl);

        //            myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
        //            //myScheme1.EditorSettings.ClassificationType = ClassificationType.UniqueValues ;
        //            myScheme1.EditorSettings.NumBreaks = 6;
        //            myScheme1.EditorSettings.FieldName = strValueField;// "Value";
        //            myScheme1.EditorSettings.UseGradient = false;
        //            myScheme1.CreateCategories(polLayer.DataSet.DataTable);

        //            //PolygonCategory pc = new PolygonCategory(Color.LightBlue, Color.DarkBlue, 1);
        //            //pc.FilterExpression = "[Value] < 6.5 ";
        //            //pc.LegendText = "0-6.5";
        //            //myScheme1.AddCategory(pc);
        //            //----------
        //            //UniqueValues+半透明
        //            double dMinValue = 0.0;
        //            double dMaxValue = 0.0;
        //            //Base或者Control，默认都按Base的区间来画，以保证Base和Control都是同一区间
        //            dMinValue = lstallSelectQALYMethodAndValue.First().lstQALYValueAttributes.Min(a => a.PointEstimate);
        //            dMaxValue = lstallSelectQALYMethodAndValue.First().lstQALYValueAttributes.Max(a => a.PointEstimate);

        //            //Todo:陈志润 20111124修改
        //            _dMinValue = dMinValue;
        //            _dMaxValue = dMaxValue;
        //            _currentLayerIndex = mainMap.Layers.Count - 1;
        //            //RenderMainMap(mainMap.Layers.Count - 1, strValueField, true, dMinValue, dMaxValue);
        //            //string pollutantUnit = string.Empty;//benMAPLine
        //            _columnName = strValueField;
        //            RenderMainMap(true);


        //            addRegionLayerToMainMap();

        //        }
        //        WaitClose();
        //    }
        //    catch (Exception ex)
        //    { }
        //    finally
        //    {
        //        WaitClose();
        //    }
        //}
        /// <summary>
        /// 打开Audit Trail Report的结果文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            { }
        }
        /// <summary>
        /// 生成Audit Trail Report按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btShowAudit_Click(object sender, EventArgs e)
        {
            try
            {
                //--显示tab
                About about=new About();
                tabCtlMain.SelectedIndex = 3;
                if (rbAuditFile.Checked)
                {
                    string filePath = txtExistingConfiguration.Text;
                    string fileType = Path.GetExtension(txtExistingConfiguration.Text);
                    switch (fileType)
                    {
                        case ".aqgx":
                            BenMAPLine aqgBenMAPLine = new BenMAPLine();
                            string err = "";
                            aqgBenMAPLine = DataSourceCommonClass.LoadAQGFile(filePath,ref err);
                            if (aqgBenMAPLine == null)
                            {
                                MessageBox.Show(err);
                                return;
                            }
                            TreeNode aqgTreeNode = new TreeNode();
                            aqgTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBenMAPLine(aqgBenMAPLine);
                            trvAuditTrialReport.Nodes.Clear();
                            trvAuditTrialReport.Nodes.Add(aqgBenMAPLine.Version == null ? "BenMAP-CE" : aqgBenMAPLine.Version);
                            trvAuditTrialReport.Nodes.Add(aqgTreeNode);
                            break;
                        case ".cfgx":
                            BaseControlCRSelectFunction cfgFunction = new BaseControlCRSelectFunction();
                            err = "";
                            cfgFunction = ConfigurationCommonClass.loadCFGFile(filePath,ref err);
                            if (cfgFunction == null)
                            {
                                MessageBox.Show(err);
                                return;
                            }
                            TreeNode cfgTreeNode = new TreeNode();
                            cfgTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunction(cfgFunction);
                            trvAuditTrialReport.Nodes.Clear();
                            trvAuditTrialReport.Nodes.Add(cfgFunction.Version == null ? "BenMAP-CE" : cfgFunction.Version);
                            trvAuditTrialReport.Nodes.Add(cfgTreeNode);
                            break;
                        case ".cfgrx":
                            BaseControlCRSelectFunctionCalculateValue cfgrFunctionCV = new BaseControlCRSelectFunctionCalculateValue();
                            err = "";
                            cfgrFunctionCV = ConfigurationCommonClass.LoadCFGRFile(filePath,ref err);
                            if (cfgrFunctionCV == null)
                            {
                                MessageBox.Show(err);
                                return;
                            }
                            TreeNode cfgrTreeNode = new TreeNode();
                            cfgrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunctionCalculateValue(cfgrFunctionCV);
                            trvAuditTrialReport.Nodes.Clear();
                            trvAuditTrialReport.Nodes.Add(cfgrFunctionCV.Version == null ? "BenMAP-CE" :cfgrFunctionCV.Version);
                            trvAuditTrialReport.Nodes.Add(cfgrTreeNode);
                            break;
                        case ".apvx":
                            ValuationMethodPoolingAndAggregation apvVMPA = new ValuationMethodPoolingAndAggregation();
                            err = "";
                            apvVMPA = APVX.APVCommonClass.loadAPVRFile(filePath,ref err);
                            if (apvVMPA == null)
                            {
                                MessageBox.Show(err);
                                return;
                            }
                            TreeNode apvTreeNode = new TreeNode();
                            apvTreeNode = AuditTrailReportCommonClass.getTreeNodeFromValuationMethodPoolingAndAggregation(apvVMPA);
                            trvAuditTrialReport.Nodes.Clear();
                            trvAuditTrialReport.Nodes.Add(apvVMPA.Version == null ? "BenMAP-CE" :apvVMPA.Version);
                            trvAuditTrialReport.Nodes.Add(apvTreeNode);
                            break;
                        case ".apvrx":
                            ValuationMethodPoolingAndAggregation apvrVMPA = new ValuationMethodPoolingAndAggregation();
                            err = "";
                            apvrVMPA = APVX.APVCommonClass.loadAPVRFile(filePath,ref err);
                            if (apvrVMPA == null)
                            {
                                MessageBox.Show(err);
                                return;
                            }
                            TreeNode apvrTreeNode = new TreeNode();
                            apvrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromValuationMethodPoolingAndAggregation(apvrVMPA);
                            trvAuditTrialReport.Nodes.Clear();
                            trvAuditTrialReport.Nodes.Add(apvrVMPA.Version == null ? "BenMAP-CE" :apvrVMPA.Version);
                            trvAuditTrialReport.Nodes.Add(apvrTreeNode);
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
                        trvAuditTrialReport.Nodes.Clear();
                        trvAuditTrialReport.Nodes.Add(apvrVMPA.Version == null ? "BenMAP-CE" :apvrVMPA.Version);
                        trvAuditTrialReport.Nodes.Add(apvrTreeNode);
                    }
                    else if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null)
                    {
                        BaseControlCRSelectFunctionCalculateValue cfgrFunctionCV = new BaseControlCRSelectFunctionCalculateValue();
                        cfgrFunctionCV = CommonClass.BaseControlCRSelectFunctionCalculateValue;
                        TreeNode cfgrTreeNode = new TreeNode();
                        cfgrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunctionCalculateValue(cfgrFunctionCV);
                        trvAuditTrialReport.Nodes.Clear();
                        trvAuditTrialReport.Nodes.Add(cfgrFunctionCV.Version == null ? "BenMAP-CE" :cfgrFunctionCV.Version);
                        trvAuditTrialReport.Nodes.Add(cfgrTreeNode);
                    }
                    else if (CommonClass.BaseControlCRSelectFunction != null)
                    {
                        BaseControlCRSelectFunction cfgFunction = new BaseControlCRSelectFunction();
                        cfgFunction = CommonClass.BaseControlCRSelectFunction;
                        TreeNode cfgTreeNode = new TreeNode();
                        cfgTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunction(cfgFunction);
                        trvAuditTrialReport.Nodes.Clear();
                        trvAuditTrialReport.Nodes.Add(cfgFunction.Version == null ? "BenMAP-CE" :cfgFunction.Version);
                        trvAuditTrialReport.Nodes.Add(cfgTreeNode);
                    }
                    else
                    {
                        MessageBox.Show("Please finish your configuration first.");
                    }
                }
                if (trvAuditTrialReport.Nodes.Count > 0)
                {
                    trvAuditTrialReport.ExpandAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("BenMAP-CE was unable to open the file. The file may be corrupt, or it may have been created using a previous incompatible version of BenMAP-CE.");
                Logger.LogError(ex.Message);
            }
        }
        /// <summary>
        /// 选择Audit File的Raio事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// 选择Current Audit File的Raio事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// 未用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                //----------------- 使用 dtPanel
                //画图-----------------------------------------------------------------------------------
                GraphPane myPane = this.zedGraphCtl.GraphPane;
                List<string> lstPane = new List<string>();
                //-------
                myPane.CurveList.Clear();
                int i = 0;
                Color[] colorArray = new Color[] { Color.Blue, Color.Red, Color.Green };
                while (i < 1)
                {
                    PointPairList list = new PointPairList();
                    int j = 0;
                    //Dictionary<int,string> dicChecked=olvRegions.CheckedObjects as Dictionary<int,string>;
                    //dicChecked = new Dictionary<int, string>();
                    //foreach (KeyValuePair<int, string> k in olvRegions.CheckedObjects)
                    //{
                    //    dicChecked.Add(k.Key, k.Value);
                    //}
                    //while (j < olvRegions.Items.Count)
                    //{
                    //    if (olvRegions.Items[j].Checked)
                    //    {
                    //        list.Add(new PointPair(Convert.ToInt32(j), CommonClass.lstChartResult[j].RegionValue));
                    //        lstPane.Add(CommonClass.lstChartResult[j].RegionName);
                    //    }
                    //    j++;
                    //}
                    foreach (ChartResult cr in olvRegions.CheckedObjects)
                    {
                        list.Add(new PointPair(Convert.ToInt32(j), cr.RegionValue));
                        lstPane.Add(cr.RegionName);
                        j++;
                    }
                    BarItem myCurve = myPane.AddBar("Result", list, colorArray[i]);
                    //  myPane.AddBar(myCurve);

                    i++;
                }
                myPane.Chart.Fill = new Fill(Color.White,
                 Color.FromArgb(255, 255, 166), 45.0F);

                // expand the range of the Y axis slightly to accommodate the labels
                //myPane.YAxis.Scale.Max += myPane.YAxis.Scale.MajorStep;
                // myPane.YAxis.Scale.TextLabels = strValuationsNow.ToArray();
                //List<string> lstTextLabels = new List<string>();
                //for (int ij = 0; ij < iRowCount; ij++)
                //{
                //    lstTextLabels.Add(dt.Rows[ij]["Name"].ToString());
                //}

                myPane.XAxis.Scale.TextLabels = lstPane.ToArray(); //lstTextLabels.ToArray();
                // myPane.XAxis.Scale.TextLabels = new string[] { dt.Rows[0]["Name"].ToString(), dt.Rows[1]["Name"].ToString(), dt.Rows[2]["Name"].ToString(), dt.Rows[3]["Name"].ToString(), dt.Rows[4]["Name"].ToString(), };
                //myPane.YAxis.Type= AxisType.
                myPane.XAxis.Type = AxisType.Text;
                myPane.XAxis.Scale.FontSpec.Angle = 65;
                myPane.XAxis.Scale.FontSpec.IsBold = true;
                myPane.XAxis.Scale.FontSpec.Size = 12;
                // Create TextObj's to provide labels for each bar
                //  BarItem.CreateBarLabels(myPane, false, "f0");
                // zgc.Controls.Add(myPane);
                //zgc.GraphPane = myPane;
                zedGraphCtl.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
                myPane.IsFontsScaled = false;//图比例变化时候图表上的文字不跟着自动缩放
                myPane.YAxis.Scale.MinAuto = true;//用数据最小最大值重新设置下横纵坐标轴的显示范围
                myPane.YAxis.Scale.MaxAuto = true;
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
                                for (int iDt =0;iDt<fs.DataTable.Rows.Count;iDt++)
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
                                        //DataRow dr = fs.DataTable.Select("convert(COL,'System.Int32')='" + CommonClass.lstChartResult[j].Col + "' and convert(ROW,'System.Int32')='" + CommonClass.lstChartResult[j].Row + "'").First();//too slow
                                        CommonClass.lstChartResult[j].RegionName = dr[cbChartXAxis.Text].ToString();
                                    }
                                    break;
                                }
                            }
                            fs.Close();
                            fs.Dispose();
                        }
                    }
                    //textChartFilter.Text = "";
                    chartXAxis = cbChartXAxis.Text;
                    List<int> lstChecked = new List<int>();
                    for (int j = 0; j < olvRegions.Items.Count; j++)
                    {
                        if (olvRegions.Items[j].Checked)
                            lstChecked.Add(j);
                    }
                    olvRegions.SetObjects(CommonClass.lstChartResult);
                    //olvRegions.RefreshObjects(CommonClass.lstChartResult);//太慢了
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
            catch
            { }
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
            // Setup a default renderer to draw the filter matches
            if (filter == null)
                olv.DefaultRenderer = null;
            else
            {
                olv.DefaultRenderer = new HighlightTextRenderer(filter);

                // Uncomment this line to see how the GDI+ rendering looks
                olv.DefaultRenderer = new HighlightTextRenderer { Filter = filter, UseGdiTextRendering = true };
            }

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
                //ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == cbPoolingWindowAPV.Items[cbPoolingWindowIncidence.SelectedIndex].ToString()).First();
                //if (vb == null || (vb.LstAllSelectValuationMethodAndValue == null && vb.LstAllSelectValuationMethodAndValueAggregation == null))
                //    return;
                //List<AllSelectCRFunction> lstRoot = new List<AllSelectCRFunction>();
                ////--------------------
                //lstIncidencePoolingAndAggregationAll = new List<AllSelectCRFunction>();
                //int i = 0;
                //int iMaxID = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Max(p => p.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Max(a=>a.ID))+1;
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
                    //lstShow.Add(vb.LstAllSelectValuationMethod.First());
                   
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
                //lstAPVPoolingAndAggregationAll = new List<AllSelectValuationMethod>();
                //List<AllSelectValuationMethod> lstRootValuation = new List<AllSelectValuationMethod>();
                //int i = 0;
                //int iMaxID = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Max(p => p.LstAllSelectValuationMethod.Max(a => a.ID))+1;
                //foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                //{
                //}
                //    lstRootValuation.Add(new AllSelectValuationMethod()
                //    {
                //        Name = "Pooling Window:" + vb.IncidencePoolingAndAggregation.PoolingName,
                //        NodeType = -999,
                //        PID = -20000,
                //        ID = -999 - i
                //    });

                //    lstAPVPoolingAndAggregationAll.Add(lstRootValuation.Last());
                //    //-----foreach
                //    foreach (AllSelectValuationMethod avm in vb.LstAllSelectValuationMethod)
                //    {
                //        lstAPVPoolingAndAggregationAll.Add(new AllSelectValuationMethod()
                //        {
                //            APVID = avm.APVID,
                //            Author = avm.Author,
                //            BenMAPValuationFunction = avm.BenMAPValuationFunction,
                //            CRID = avm.CRID,
                //            CRIndex = avm.CRIndex,
                //            DataSet = avm.DataSet,
                //            EndAge = avm.EndAge,
                //            EndPoint = avm.EndPoint,
                //            EndPointGroup = avm.EndPointGroup,
                //            EndPointID = avm.EndPointID,
                //            Ethnicity = avm.Ethnicity,
                //            Function = avm.Function,
                //            Gender = avm.Gender,
                //            ID = avm.ID + iMaxID * i,
                //            lstMonte = avm.lstMonte,
                //            Location = avm.Location,
                //            Metric = avm.Metric,
                //            MetricStatistic = avm.MetricStatistic,
                //            Name = avm.Name,
                //            NodeType = avm.NodeType,
                //            OtherPollutants = avm.OtherPollutants,
                //            PID = avm.PID + iMaxID * i,
                //            Pollutant = avm.Pollutant,
                //            PoolingMethod = avm.PoolingMethod,
                //            Qualifier = avm.Qualifier,
                //            Race = avm.Race,
                //            SeasonalMetric = avm.SeasonalMetric,
                //            StartAge = avm.StartAge,
                //            Version = avm.Version,
                //            Weight = avm.Weight,
                //            Year = avm.Year,

                //        }
                //        );
                //        if (avm.ID == vb.LstAllSelectValuationMethod.First().ID)
                //        {
                //            lstAPVPoolingAndAggregationAll.Last().PID = -999 - i;

                //        }
                //    }

                //    i++;
                //}


                //tlvAPVResult.Roots = lstRootValuation;
                //this.tlvAPVResult.CanExpandGetter = delegate(object x)
                //{
                //    AllSelectValuationMethod dir = (AllSelectValuationMethod)x;
                //    if(btShowDetailValuation.Text=="Show Brief")
                //        return (dir.NodeType != 5);
                //    else
                //        return (dir.NodeType != 5 && (dir.PoolingMethod == "" || dir.PoolingMethod == "None" || dir.PoolingMethod == null));
                //};
                //this.tlvAPVResult.ChildrenGetter = delegate(object x)
                //{
                //    AllSelectValuationMethod dir = (AllSelectValuationMethod)x;

                //    try
                //    {
                //        return getChildFromAllSelectValuationMethod(dir, null);
                //    }
                //    catch (UnauthorizedAccessException ex)
                //    {
                //        MessageBox.Show(this, ex.Message, "ObjectListViewDemo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //        return new List<AllSelectValuationMethod>();
                //        //return new ArrayList();
                //    }
                //};
                //tlvAPVResult.RebuildAll(true);
                //tlvAPVResult.ExpandAll();
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// APV PoolingWindow的SelectIndexChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbPoolingWindowAPV_SelectedIndexChanged(object sender, EventArgs e)
        {
            //------------if cbPoolingWindows is Enable-----------
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
            }// return;
            if (CommonClass.ValuationMethodPoolingAndAggregation == null || CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase == null) return;
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
            //lstShow.Add(vb.LstAllSelectValuationMethod.First());

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
            //if (vb == null || (vb.LstAllSelectValuationMethodAndValue == null && vb.LstAllSelectValuationMethodAndValueAggregation == null))
            //    return;
            //List<AllSelectValuationMethod> lstRootValuation = new List<AllSelectValuationMethod>();
            //lstRootValuation.Add(vb.LstAllSelectValuationMethod.First());
            ////---------------显示列表到APVX Result GIS Show里面-------------------------

            //tlvAPVResult.Roots = lstRootValuation;
            //this.tlvAPVResult.CanExpandGetter = delegate(object x)
            //{
            //    AllSelectValuationMethod dir = (AllSelectValuationMethod)x;
            //    if (btShowDetailValuation.Text == "Show Brief")
            //        return (dir.NodeType != 5);
            //    else
            //        return (dir.NodeType != 5 && (dir.PoolingMethod == "" || dir.PoolingMethod == "None" || dir.PoolingMethod == null));
            //};
            //this.tlvAPVResult.ChildrenGetter = delegate(object x)
            //{
            //    AllSelectValuationMethod dir = (AllSelectValuationMethod)x;

            //    try
            //    {
            //        return getChildFromAllSelectValuationMethod(dir, vb);
            //    }
            //    catch (UnauthorizedAccessException ex)
            //    {
            //        MessageBox.Show(this, ex.Message, "ObjectListViewDemo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //        return new List<AllSelectValuationMethod>();
            //        //return new ArrayList();
            //    }
            //};
            //tlvAPVResult.RebuildAll(true);
            //tlvAPVResult.ExpandAll();
        }
        //private List<AllSelectCRFunction> lstIncidencePoolingAndAggregationAll;
        public Dictionary<AllSelectCRFunction, string> dicIncidencePoolingAndAggregation;
        public Dictionary<AllSelectCRFunction, string> dicIncidencePoolingAndAggregationUnPooled;
        public void LoadAllIncidencePooling(ref Dictionary<AllSelectCRFunction, string> Pooled,ref Dictionary<AllSelectCRFunction, string> UnPooled)
        {
            if (!rbIncidenceAll.Checked) return;
            try
            {
                Pooled = new Dictionary<AllSelectCRFunction, string>();
                UnPooled = new Dictionary<AllSelectCRFunction, string>();
                //ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == cbPoolingWindowAPV.Items[cbPoolingWindowIncidence.SelectedIndex].ToString()).First();
                //if (vb == null || (vb.LstAllSelectValuationMethodAndValue == null && vb.LstAllSelectValuationMethodAndValueAggregation == null))
                //    return;
                //List<AllSelectCRFunction> lstRoot = new List<AllSelectCRFunction>();
                ////--------------------
                //lstIncidencePoolingAndAggregationAll = new List<AllSelectCRFunction>();
                //int i = 0;
                //int iMaxID = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Max(p => p.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Max(a=>a.ID))+1;
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
                            //if (acr.PoolingMethod != "" && acr.PoolingMethod != "None")//modify by xiejp 20120921
                            if (acr.PoolingMethod != "None")
                            {
                                Pooled.Add(acr, vb.IncidencePoolingAndAggregation.PoolingName);
                            }
                            //else if (acr.PoolingMethod == "")
                            //{
                            //    dicIncidencePoolingAndAggregationUnPooled.Add(acr, vb.IncidencePoolingAndAggregation.PoolingName);
                            //}
                        }
                    }
                    foreach (AllSelectCRFunction acr in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion)
                    {
                        if (acr.PoolingMethod == "")
                        {
                            UnPooled.Add(acr, vb.IncidencePoolingAndAggregation.PoolingName);
                        }

                    }

                    //lstRoot.Add(new AllSelectCRFunction()
                    //{
                    //    Name = "Pooling Window:" + vb.IncidencePoolingAndAggregation.PoolingName,
                    //    NodeType = -999,
                    //    PID = -20000,
                    //    ID = -999 - i
                    //});

                    //lstIncidencePoolingAndAggregationAll.Add(lstRoot.Last());
                    ////vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().PID = -999 - i;
                    ////---------foreach
                    //foreach (AllSelectCRFunction acr in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion)
                    //{
                    //    lstIncidencePoolingAndAggregationAll.Add(new AllSelectCRFunction
                    //    ()
                    //    {
                    //        Author = acr.Author,
                    //        EndAge = acr.EndAge,
                    //        CRID = acr.CRID,
                    //        CRIndex = acr.CRIndex,
                    //        DataSet = acr.DataSet,
                    //        EndPoint = acr.EndPoint,
                    //        EndPointGroup = acr.EndPointGroup,
                    //        EndPointGroupID = acr.EndPointGroupID,
                    //        EndPointID = acr.EndPointID,
                    //        Ethnicity = acr.Ethnicity,
                    //        Function = acr.Function,
                    //        Gender = acr.Gender,
                    //        ID = Convert.ToInt32(acr.ID + (i * iMaxID)),
                    //        Location = acr.Location,
                    //        Metric = acr.Metric,
                    //        MetricStatistic = acr.MetricStatistic,
                    //        Name = acr.Name,
                    //        NodeType = acr.NodeType,
                    //        OtherPollutants = acr.OtherPollutants,
                    //        PID = Convert.ToInt32(acr.PID + (i * iMaxID)),
                    //        Pollutant = acr.Pollutant,
                    //        PoolingMethod = acr.PoolingMethod,
                    //        Qualifier = acr.Qualifier,
                    //        Race = acr.Race,
                    //        SeasonalMetric = acr.SeasonalMetric,
                    //        StartAge = acr.StartAge,
                    //        Version = acr.Version,
                    //        Weight = acr.Weight,
                    //        Year = acr.Year,
                    //        CRSelectFunctionCalculateValue= acr.CRSelectFunctionCalculateValue,

                    //    });
                    //    if (acr.ID == vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().ID)
                    //        lstIncidencePoolingAndAggregationAll.Last().PID = -999 - i;

                    //}
                    //i++;
                }
                //---------------显示列表到Incidence Result GIS Show里面-------------------------

                //this.olvIncidence.Roots = lstRoot;
                //this.olvIncidence.CanExpandGetter = delegate(object x)
                //{
                //    AllSelectCRFunction dir = (AllSelectCRFunction)x;
                //    if (btShowDetailIncidence.Text == "Show Brief")
                //        return (dir.NodeType != 5);
                //    else
                //        return (dir.NodeType != 5 && (dir.PoolingMethod == "" || dir.PoolingMethod == "None" || dir.PoolingMethod==null));
                //};
                //this.olvIncidence.ChildrenGetter = delegate(object x)
                //{
                //    AllSelectCRFunction dir = (AllSelectCRFunction)x;



                //    try
                //    {
                //        return getChildFromAllSelectCRFunction(dir, null);
                //    }
                //    catch (UnauthorizedAccessException ex)
                //    {
                //        MessageBox.Show(this, ex.Message, "ObjectListViewDemo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //        return new List<AllSelectValuationMethod>();
                //        //return new ArrayList();
                //    }
                //};
                //olvIncidence.RebuildAll(true);
                //olvIncidence.ExpandAll();
            }
            catch (Exception ex)
            {
            }

        }
        /// <summary>
        /// Incidence PoolingWindow的SelectIndexChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    //-------------改成All---
                    // rbIncidenceAll.Checked = true;
                    //tlvIncidence_DoubleClick(sender, e);

                    tlvIncidence_DoubleClick(sender, e);
                }
                //if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabAPVResultGISShow")
                //{
                //    //-------------改成All---
                //    // rbIncidenceAll.Checked = true;
                //    tlvAPVResult_DoubleClick(sender, e);
                //}
                //if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabQLAYResultShow")
                //{
                //    //-------------改成All---
                //    // rbIncidenceAll.Checked = true;
                //    this.tlvQALYResult_DoubleClick(sender, e);
                //}

                return;
            }
            if (CommonClass.ValuationMethodPoolingAndAggregation == null || CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase == null) return;
            ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == cbPoolingWindowAPV.Items[cbPoolingWindowIncidence.SelectedIndex].ToString()).First();
            //-----------
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
                    //if (acr.PoolingMethod != "" && acr.PoolingMethod != "None")//modify by xiejp 20120921
                    if (acr.PoolingMethod != "None")
                    {
                        dicIncidencePoolingAndAggregation.Add(acr, vb.IncidencePoolingAndAggregation.PoolingName);
                    }
                    //else if (acr.PoolingMethod == "")
                    //{
                    //    dicIncidencePoolingAndAggregationUnPooled.Add(acr, vb.IncidencePoolingAndAggregation.PoolingName);
                    //}
                }
            }
            foreach (AllSelectCRFunction acr in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion)
            {
                if (acr.PoolingMethod == "")
                {
                    dicIncidencePoolingAndAggregationUnPooled.Add(acr, vb.IncidencePoolingAndAggregation.PoolingName);
                }

            }
            //--------Set objects to olvIncidence------------------
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
            //tlvIncidence_DoubleClick(sender, e);
            //if (vb == null || (vb.LstAllSelectValuationMethodAndValue == null && vb.LstAllSelectValuationMethodAndValueAggregation == null))
            //    return;
            //List<AllSelectCRFunction> lstRoot = new List<AllSelectCRFunction>();
            //lstRoot.Add(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First());
            ////---------------显示列表到Incidence Result GIS Show里面-------------------------

            //this.olvIncidence.Roots = lstRoot;
            //this.olvIncidence.CanExpandGetter = delegate(object x)
            //{
            //    AllSelectCRFunction dir = (AllSelectCRFunction)x;
            //    if (btShowDetailIncidence.Text == "Show Brief")
            //        return (dir.NodeType != 5);
            //    else
            //        return (dir.NodeType != 5 && (dir.PoolingMethod == "" || dir.PoolingMethod == "None" || dir.PoolingMethod == null));
            //};
            //this.olvIncidence.ChildrenGetter = delegate(object x)
            //{
            //    AllSelectCRFunction dir = (AllSelectCRFunction)x;

            //    try
            //    {
            //        return getChildFromAllSelectCRFunction(dir, vb);
            //    }
            //    catch (UnauthorizedAccessException ex)
            //    {
            //        MessageBox.Show(this, ex.Message, "ObjectListViewDemo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //        return new List<AllSelectValuationMethod>();
            //        //return new ArrayList();
            //    }
            //};
            //olvIncidence.RebuildAll(true);
            //olvIncidence.ExpandAll();
        }
        Dictionary<AllSelectQALYMethod, string> dicQALYPoolingAndAggregation;
        Dictionary<AllSelectQALYMethod, string> dicQALYPoolingAndAggregationUnPooled;
        /// <summary>
        /// QALY PoolingWindow的SelectIndexChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void cbPoolingWindowQALY_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    dicQALYPoolingAndAggregation = new Dictionary<AllSelectQALYMethod, string>();
        //    dicQALYPoolingAndAggregationUnPooled = new Dictionary<AllSelectQALYMethod, string>();
        //    AllSelectQALYMethodAndValue allSelectQALYMethodAndValue = null;
        //    if (rbQALYAll.Checked)
        //    {

        //        foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
        //        {
        //            if (vb.lstAllSelectQALYMethod == null) continue;
        //            List<AllSelectQALYMethod> lstShow = new List<AllSelectQALYMethod>();
        //            lstShow.Add(vb.lstAllSelectQALYMethod.First());
        //            getChildFromAllSelectQALYMethodUnPooled(vb.lstAllSelectQALYMethod.First(), vb, ref lstShow);

        //            foreach (AllSelectQALYMethod avm in lstShow)
        //            {
        //                if (avm.PoolingMethod != "None")
        //                {
        //                    try
        //                    {
        //                        allSelectQALYMethodAndValue = null;
        //                        if (vb.LstAllSelectValuationMethodAndValueAggregation == null || vb.LstAllSelectValuationMethodAndValueAggregation.Count == 0)
        //                        {

        //                            allSelectQALYMethodAndValue = vb.lstAllSelectQALYMethodAndValue.Where(p => p.AllSelectQALYMethod.ID == avm.ID).First();

        //                        }
        //                        else
        //                        {

        //                            allSelectQALYMethodAndValue = vb.lstAllSelectQALYMethodAndValueAggregation.Where(p => p.AllSelectQALYMethod.ID == avm.ID).First();
        //                        }
        //                        if (allSelectQALYMethodAndValue != null && allSelectQALYMethodAndValue.lstQALYValueAttributes != null && allSelectQALYMethodAndValue.lstQALYValueAttributes.Count() > 0)
        //                            dicQALYPoolingAndAggregation.Add(avm, vb.IncidencePoolingAndAggregation.PoolingName);
        //                    }
        //                    catch
        //                    {
        //                    }

        //                }
        //            }
        //            foreach (AllSelectQALYMethod avm in vb.lstAllSelectQALYMethod)
        //            {
        //                if (avm.PoolingMethod == "" || avm.PoolingMethod == null)
        //                {
        //                    try
        //                    {
        //                        if (vb.lstAllSelectQALYMethodAndValueAggregation == null || vb.lstAllSelectQALYMethodAndValueAggregation.Count == 0)
        //                        {

        //                            allSelectQALYMethodAndValue = vb.lstAllSelectQALYMethodAndValue.Where(p => p.AllSelectQALYMethod.ID == avm.ID).First();

        //                        }
        //                        else
        //                        {

        //                            allSelectQALYMethodAndValue = vb.lstAllSelectQALYMethodAndValueAggregation.Where(p => p.AllSelectQALYMethod.ID == avm.ID).First();
        //                        }
        //                        if (allSelectQALYMethodAndValue != null && allSelectQALYMethodAndValue.lstQALYValueAttributes != null && allSelectQALYMethodAndValue.lstQALYValueAttributes.Count() > 0)
        //                            dicQALYPoolingAndAggregationUnPooled.Add(avm, vb.IncidencePoolingAndAggregation.PoolingName);
        //                    }
        //                    catch
        //                    {
        //                    }

        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == cbPoolingWindowQALY.Items[cbPoolingWindowQALY.SelectedIndex].ToString()).First();
        //        if (vb == null || (vb.lstAllSelectQALYMethodAndValue == null && vb.lstAllSelectQALYMethodAndValueAggregation == null))
        //            return;
        //        List<AllSelectQALYMethod> lstShow = new List<AllSelectQALYMethod>();
        //        lstShow.Add(vb.lstAllSelectQALYMethod.First());
        //        getChildFromAllSelectQALYMethodUnPooled(vb.lstAllSelectQALYMethod.First(), vb, ref lstShow);
        //        foreach (AllSelectQALYMethod avm in lstShow)
        //        {
        //            if (avm.PoolingMethod != "None")
        //            {
        //                try
        //                {
        //                    allSelectQALYMethodAndValue = null;
        //                    if (vb.LstAllSelectValuationMethodAndValueAggregation == null || vb.LstAllSelectValuationMethodAndValueAggregation.Count == 0)
        //                    {

        //                        allSelectQALYMethodAndValue = vb.lstAllSelectQALYMethodAndValue.Where(p => p.AllSelectQALYMethod.ID == avm.ID).First();

        //                    }
        //                    else
        //                    {

        //                        allSelectQALYMethodAndValue = vb.lstAllSelectQALYMethodAndValueAggregation.Where(p => p.AllSelectQALYMethod.ID == avm.ID).First();
        //                    }
        //                    if (allSelectQALYMethodAndValue != null && allSelectQALYMethodAndValue.lstQALYValueAttributes != null && allSelectQALYMethodAndValue.lstQALYValueAttributes.Count() > 0)
        //                        dicQALYPoolingAndAggregation.Add(avm, vb.IncidencePoolingAndAggregation.PoolingName);
        //                }
        //                catch
        //                {
        //                }

        //            }
        //            else if (avm.PoolingMethod == "" || avm.PoolingMethod == null)
        //            {
        //                try
        //                {
        //                    if (vb.lstAllSelectQALYMethodAndValueAggregation == null || vb.lstAllSelectQALYMethodAndValueAggregation.Count == 0)
        //                    {

        //                        allSelectQALYMethodAndValue = vb.lstAllSelectQALYMethodAndValue.Where(p => p.AllSelectQALYMethod.ID == avm.ID).First();

        //                    }
        //                    else
        //                    {

        //                        allSelectQALYMethodAndValue = vb.lstAllSelectQALYMethodAndValueAggregation.Where(p => p.AllSelectQALYMethod.ID == avm.ID).First();
        //                    }
        //                    if (allSelectQALYMethodAndValue != null && allSelectQALYMethodAndValue.lstQALYValueAttributes != null && allSelectQALYMethodAndValue.lstQALYValueAttributes.Count() > 0)
        //                        dicQALYPoolingAndAggregationUnPooled.Add(avm, vb.IncidencePoolingAndAggregation.PoolingName);
        //                }
        //                catch
        //                {
        //                }

        //            }
        //        }

        //    }
        //    if (CommonClass.ValuationMethodPoolingAndAggregation == null || CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase == null) return;
        //    if (btQALYPooled.Text == "Show aggregated")
        //        tlvQALYResult.SetObjects(dicQALYPoolingAndAggregation);
        //    else
        //        tlvQALYResult.SetObjects(dicQALYPoolingAndAggregationUnPooled);
        //    tlvQALYResult.SelectAll();
        //    if (tabCtlReport.TabPages[tabCtlReport.SelectedIndex].Name == "tabAPVResultGISShow")
        //    {
        //        tlvQALYResult_DoubleClick(sender, e);
        //    }
        //    //List<AllSelectQALYMethod> lstQALYRoot = new List<AllSelectQALYMethod>();
        //    //lstQALYRoot.Add(vb.lstAllSelectQALYMethod.First());
        //    //tlvQALYResult.Roots = lstQALYRoot;

        //    //this.tlvQALYResult.CanExpandGetter = delegate(object x)
        //    //{
        //    //    AllSelectQALYMethod dir = (AllSelectQALYMethod)x;
        //    //    return (dir.NodeType != 5);
        //    //};
        //    //this.tlvQALYResult.ChildrenGetter = delegate(object x)
        //    //{
        //    //    AllSelectQALYMethod dir = (AllSelectQALYMethod)x;
        //    //    try
        //    //    {
        //    //        return getChildFromAllSelectQALYMethod(dir, vb);
        //    //    }
        //    //    catch (UnauthorizedAccessException ex)
        //    //    {
        //    //        MessageBox.Show(this, ex.Message, "ObjectListViewDemo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //    //        return new List<AllSelectValuationMethod>();
        //    //        //return new ArrayList();
        //    //    }
        //    //};
        //    //tlvQALYResult.RebuildAll(true);
        //    //tlvQALYResult.ExpandAll();
        //}
        /// <summary>
        /// 问号的鼠标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picCRHelp_Click(object sender, EventArgs e)
        {
            this.toolTip1.Show("Double click datagrid to create result.\r\nIf you choose \'Create map,data and chart \',GIS Map/Table" +
                    "/Chart results will be created.\r\nIf you choose \'Create data (table) for multiple studies\',Only one acti" +
                    "ve result will be created.", picCRHelp, 5000);
        }

        /// <summary>
        /// 输出AuditTrailReport
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAuditTrailOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sDlg = new SaveFileDialog();
                sDlg.Title = "Save Audit Trail Report to XML Document or Text";
                sDlg.Filter = "TXT Files (*.txt)|*.txt|CTLX Files (*.ctlx)|*.ctlx|XML Files (*.xml)|*.xml";
                sDlg.AddExtension = true;
                bool saveOk = false;
                //-----如果没有内容，则返回
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
                            //----------Current--如果是APVX 则保存APVX CFGR --如果有CFGR 则保存 CFG -如果是AQG，则AQG--
                            //----------如果是有文件则按文件----
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
                                        aqgBenMAPLine = DataSourceCommonClass.LoadAQGFile(filePath,ref err);
                                        if (aqgBenMAPLine == null)
                                        {
                                            MessageBox.Show(err);
                                            return;
                                        }
                                        BatchCommonClass.OutputAQG(aqgBenMAPLine, sDlg.FileName,filePath);
                                        break;
                                    case ".cfgx":
                                        BaseControlCRSelectFunction cfgFunction = new BaseControlCRSelectFunction();
                                        err = "";
                                        cfgFunction = ConfigurationCommonClass.loadCFGFile(filePath,ref err);
                                        if (cfgFunction == null)
                                        {
                                            MessageBox.Show(err);
                                            return;
                                        }
                                        BatchCommonClass.OutputCFG(cfgFunction, sDlg.FileName,filePath);
                                        break;
                                    case ".cfgrx":
                                        BaseControlCRSelectFunctionCalculateValue cfgrFunctionCV = new BaseControlCRSelectFunctionCalculateValue();
                                        err = "";
                                        cfgrFunctionCV = ConfigurationCommonClass.LoadCFGRFile(filePath,ref err);
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
                                        List<CRSelectFunction> lstCRSelectFunction=new List<CRSelectFunction>();
                                        foreach(CRSelectFunctionCalculateValue crfc in cfgrFunctionCV.lstCRSelectFunctionCalculateValue)
                                        {
                                            lstCRSelectFunction.Add(crfc.CRSelectFunction);
                                        }
                                        bc.lstCRSelectFunction = lstCRSelectFunction;
                                        BatchCommonClass.OutputCFG(bc, sDlg.FileName,"");
                                        ConfigurationCommonClass.SaveCFGFile(bc, sDlg.FileName.Substring(0, sDlg.FileName.Length - 4) + "cfgx");
                                        break;
                                    case ".apvx":
                                        ValuationMethodPoolingAndAggregation apvVMPA = new ValuationMethodPoolingAndAggregation();
                                        err = "";
                                        apvVMPA = APVX.APVCommonClass.loadAPVRFile(filePath,ref err);
                                        if (apvVMPA == null)
                                        {
                                            MessageBox.Show(err);
                                            return;
                                        }
                                        BatchCommonClass.OutputAPV(apvVMPA, sDlg.FileName,filePath);
                                        break;
                                    case ".apvrx":
                                        ValuationMethodPoolingAndAggregation apvrVMPA = new ValuationMethodPoolingAndAggregation();
                                        err = "";
                                        apvrVMPA = APVX.APVCommonClass.loadAPVRFile(filePath,ref err);
                                        if (apvrVMPA == null)
                                        {
                                            MessageBox.Show(err);
                                            return;
                                        }
                                        APVX.APVCommonClass.SaveAPVFile(sDlg.FileName.Substring(0, sDlg.FileName.Length - 4) + "apvx", apvrVMPA);
                                        BatchCommonClass.OutputAPV(apvrVMPA, sDlg.FileName,"");
                                        break;
                                }
                                MessageBox.Show("Configuration file saved.", "File saved");
                            }
                            else if (rbAuditCurrent.Checked)
                            {
                                if (CommonClass.ValuationMethodPoolingAndAggregation != null)
                                {
                                    ValuationMethodPoolingAndAggregation apvrVMPA = new ValuationMethodPoolingAndAggregation();
                                    apvrVMPA = CommonClass.ValuationMethodPoolingAndAggregation;
                                    APVX.APVCommonClass.SaveAPVFile(sDlg.FileName.Substring(0, sDlg.FileName.Length - 4) + "apvx", apvrVMPA);
                                    BatchCommonClass.OutputAPV(apvrVMPA, sDlg.FileName, sDlg.FileName.Substring(0, sDlg.FileName.Length - 4) + "apvx");
                                    MessageBox.Show("Configuration file saved.", "File saved");
                                }                                
                                else if (CommonClass.BaseControlCRSelectFunction != null)
                                {
                                    BaseControlCRSelectFunction cfgFunction = new BaseControlCRSelectFunction();
                                    cfgFunction = CommonClass.BaseControlCRSelectFunction;
                                    Configuration.ConfigurationCommonClass.SaveCFGFile(CommonClass.BaseControlCRSelectFunction, sDlg.FileName.Substring(0, sDlg.FileName.Length - 4) + "cfgx");
                                    BatchCommonClass.OutputCFG(cfgFunction, sDlg.FileName, sDlg.FileName.Substring(0, sDlg.FileName.Length - 4) + "cfgx");
                                    MessageBox.Show("Configuration file saved.", "File saved");
                                }
                                else
                                {
                                    MessageBox.Show("Please finish your configuration first.");
                                }
                            }
                            break;
                        case ".txt":
                            saveOk = exportToTxt(trvAuditTrialReport, sDlg.FileName);
                            break;
                        case ".xml":
                            saveOk = exportToXml(trvAuditTrialReport, sDlg.FileName);
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
        /// <summary>
        /// 输出AuditTrailReport Text
        /// </summary>
        /// <param name="tv"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool exportToTxt(TreeView tv, string filename)
        {
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Create);
                sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(tv.Nodes[0].Text);
                foreach (TreeNode node in tv.Nodes)
                {
                    saveNode(node.Nodes);
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
        /// <summary>
        /// 输出AuditTrailReport XML
        /// </summary>
        /// <param name="tv"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool exportToXml(TreeView tv, string filename)
        {
            try
            {
                sw = new StreamWriter(filename, false, System.Text.Encoding.UTF8);
                //Write the header
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                //Write our root node
                string txtWithoutSpace = tv.Nodes[0].Text;
                txtWithoutSpace = txtWithoutSpace.Replace(" ", ".");
                txtWithoutSpace = txtWithoutSpace.Replace("&", "And");
                txtWithoutSpace = txtWithoutSpace.Replace(":", "");
                txtWithoutSpace = txtWithoutSpace.Replace("..", ".");
                sw.WriteLine("<" + txtWithoutSpace + ">");

                foreach (TreeNode node in tv.Nodes)
                {
                    saveNode(node.Nodes);
                }
                //Close the root node
                sw.WriteLine("</" + txtWithoutSpace + ">");
                sw.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// 保存node到文本
        /// </summary>
        /// <param name="tnc"></param>
        private void saveNode(TreeNodeCollection tnc)
        {
            foreach (TreeNode node in tnc)
            {
                //If we have child nodes, we'll write 
                //a parent node, then iterrate through
                //the children
                if (node.Nodes.Count > 0)
                {
                    string txtWithoutSpace = node.Text;
                    txtWithoutSpace = txtWithoutSpace.Replace(" ", ".");
                    txtWithoutSpace = txtWithoutSpace.Replace("&", "And");
                    txtWithoutSpace = txtWithoutSpace.Replace(":", "");
                    txtWithoutSpace = txtWithoutSpace.Replace("..", ".");

                    sw.WriteLine("<" + txtWithoutSpace + ">");
                    saveNode(node.Nodes);
                    sw.WriteLine("</" + txtWithoutSpace + ">");
                }
                else //No child nodes, so we just write the text
                    sw.WriteLine(node.Text);
            }
        }
        /// <summary>
        /// 点击CR 的Show Result按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btShowCRResult_Click(object sender, EventArgs e)
        {
            //Selected--> To GIS Show
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
                    //crSelectFunctionCalculateValue = cr;
                }
                if (lstCRSelectFunctionCalculateValue.Count != 0)
                {
                    //--------------做Aggregation----------------
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
                                //crSelectFunctionCalculateValue = APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crSelectFunctionCalculateValue, CommonClass.GBenMAPGrid.GridDefinitionID, iAggregationGridType);
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
                    //--------------重新生成拉丁立体方-------------
                    //Configuration.ConfigurationCommonClass.UpdateCRSelectFunctionCalculateValueLHS(ref crSelectFunctionCalculateValue);
                    if (i == 0)
                    {
                        //初始化table数据！
                        //---------------------------清空原有数据 Map/Table/Chart--------------------------------------------
                        ClearMapTableChart();
                        //-----------------------chart未写---------------------------------------------
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
                            tsbChangeProjection.Text = "change projection to Albers";
                            mainMap.ProjectionModeReproject = ActionMode.Never;
                            mainMap.ProjectionModeDefine = ActionMode.Never;
                            string shapeFileName = "";
                            if (CommonClass.GBenMAPGrid is ShapefileGrid)
                            {
                                mainMap.Layers.Clear();
                                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                                {
                                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp";
                                    //mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp");
                                }
                            }
                            else if (CommonClass.GBenMAPGrid is RegularGrid)
                            {
                                mainMap.Layers.Clear();
                                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp"))
                                {
                                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp";
                                    // mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp");
                                }
                            }

                            //FeatureSet fs = new FeatureSet();
                            //fs.Open(shapeFileName);
                            mainMap.Layers.Add(shapeFileName);

                            DataTable dt = (mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).DataSet.DataTable;
                            string author = lstCRSelectFunctionCalculateValue.First().CRSelectFunction.BenMAPHealthImpactFunction.Author;//
                            if (author.IndexOf(" ") != -1)
                            {
                                author = author.Substring(0, author.IndexOf(" "));
                            }
                            (mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).LegendText = author;// lstCRSelectFunctionCalculateValue.First().CRSelectFunction.BenMAPHealthImpactFunction.Author;// "CRResult";
                            (mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).Name = author;// lstCRSelectFunctionCalculateValue.First().CRSelectFunction.BenMAPHealthImpactFunction.Author;//"CRResult";
                            //加值
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
                            dt.Columns.Add("Incidence", typeof(double));
                            j = 0;
                            while (j < dt.Columns.Count)
                            {
                                if (dt.Columns[j].ColumnName.ToLower() == "col") iCol = j;
                                if (dt.Columns[j].ColumnName.ToLower() == "row") iRow = j;

                                j++;
                            }
                            j = 0;
                            Dictionary<string, double> dicAll = new Dictionary<string, double>();
                            foreach (CRCalculateValue crcv in crSelectFunctionCalculateValue.CRCalculateValues)
                            {
                                dicAll.Add(crcv.Col + "," + crcv.Row, crcv.PointEstimate);
                            }
                            //匹配
                            foreach (DataRow dr in dt.Rows)
                            {
                                try
                                {
                                    //dr["Value"] = crSelectFunctionCalculateValue.CRCalculateValues.Where(p => p.Col == Convert.ToInt32(dr[iCol])
                                    //    && p.Row == Convert.ToInt32(dr[iRow])).Select(p => p.PointEstimate).First();
                                    if (dicAll.ContainsKey(dr[iCol] + "," + dr[iRow]))
                                        dr["Incidence"] = dicAll[dr[iCol] + "," + dr[iRow]];
                                    else
                                        dr["Incidence"] = 0;
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                            if (File.Exists(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp");
                            (mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).DataSet.SaveAs(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp", true);
                            mainMap.Layers.Clear();
                            mainMap.Layers.Add(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp");
                            MapPolygonLayer polLayer = mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer;
                            string strValueField = polLayer.DataSet.DataTable.Columns[polLayer.DataSet.DataTable.Columns.Count - 1].ColumnName;
                            PolygonScheme myScheme1 = new PolygonScheme();
                            float fl = (float)0.1;
                            myScheme1.EditorSettings.StartColor = Color.Blue;
                            myScheme1.EditorSettings.StartColor.ToTransparent(fl);
                            myScheme1.EditorSettings.EndColor = Color.Red;
                            myScheme1.EditorSettings.EndColor.ToTransparent(fl);

                            myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
                            //myScheme1.EditorSettings.ClassificationType = ClassificationType.UniqueValues ;
                            myScheme1.EditorSettings.NumBreaks = 6;
                            myScheme1.EditorSettings.FieldName = strValueField;// "Value";
                            myScheme1.EditorSettings.UseGradient = false;
                            myScheme1.CreateCategories(polLayer.DataSet.DataTable);

                            //PolygonCategory pc = new PolygonCategory(Color.LightBlue, Color.DarkBlue, 1);
                            //pc.FilterExpression = "[Value] < 6.5 ";
                            //pc.LegendText = "0-6.5";
                            //myScheme1.AddCategory(pc);
                            //----------
                            //UniqueValues+半透明
                            double dMinValue = 0.0;
                            double dMaxValue = 0.0;
                            //Base或者Control，默认都按Base的区间来画，以保证Base和Control都是同一区间
                            dMinValue = crSelectFunctionCalculateValue.CRCalculateValues.Min(a => a.PointEstimate);
                            dMaxValue = crSelectFunctionCalculateValue.CRCalculateValues.Max(a => a.PointEstimate);

                            //Todo:陈志润 20111124修改
                            _dMinValue = dMinValue;
                            _dMaxValue = dMaxValue;
                            _currentLayerIndex = mainMap.Layers.Count - 1;
                            string pollutantUnit = string.Empty;//benMAPLine
                            _columnName = strValueField;
                            RenderMainMap(true);
                            (mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).LegendText = author;// lstCRSelectFunctionCalculateValue.First().CRSelectFunction.BenMAPHealthImpactFunction.Author;// "CRResult";
                            (mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).Name = author;// lstCRSelectFunctionCalculateValue.First().CRSelectFunction.BenMAPHealthImpactFunction.Author;//"CRResult";

                            //Color[] colors = new Color[] { Color.Blue, Color.FromArgb(0, 255, 255), Color.FromArgb(0, 255, 0), Color.Yellow, Color.Red, Color.FromArgb(255, 0, 255) };
                            ////Quantities+半透明
                            //PolygonCategoryCollection pcc = new PolygonCategoryCollection();
                            //int iColor = 0;

                            //foreach (PolygonCategory pc in myScheme1.Categories)
                            //{
                            //    //pc.Symbolizer.SetOutlineWidth(0);
                            //    PolygonCategory pcin = pc;
                            //    double dnow = dMinValue + ((dMaxValue - dMinValue) / 6.00) * Convert.ToDouble(iColor);
                            //    double dnowUp = dMinValue + ((dMaxValue - dMinValue) / 6.00) * Convert.ToDouble(iColor + 1);
                            //    pcin.FilterExpression = string.Format("[{0}]>=" + dnow + " and [{0}] <" + dnowUp, strValueField);
                            //    pcin.LegendText = string.Format("[{0}]>=" + dnow.ToString("E2") + " and [{0}] <" + dnowUp.ToString("E2"), strValueField);//.ToString("E2")
                            //    if (iColor == 0)
                            //    {
                            //        pcin.FilterExpression = string.Format(" [{0}] <" + dnowUp.ToString("E2"), strValueField);
                            //        pcin.LegendText = string.Format(" [{0}] <" + dnowUp.ToString("E2"), strValueField);

                            //    }
                            //    if (iColor == myScheme1.Categories.Count - 1)
                            //    {
                            //        pcin.FilterExpression = string.Format(" [{0}] >=" + dnow.ToString("E2"), strValueField);
                            //        pcin.LegendText = string.Format(" [{0}] >=" + dnow.ToString("E2"), strValueField);

                            //    }

                            //    //pcin.LegendText = pcin.FilterExpression;

                            //    pcin.Symbolizer.SetOutline(Color.Transparent, 0);
                            //    Color ctemp = pcin.Symbolizer.GetFillColor();
                            //    float fColor = (float)0.2;
                            //    ctemp.ToTransparent(fColor);
                            //    pcin.Symbolizer.SetFillColor(colors[iColor]);
                            //    pcc.Add(pcin);
                            //    iColor++;
                            //}
                            //myScheme1.ClearCategories();
                            //foreach (PolygonCategory pct in pcc)
                            //{
                            //    myScheme1.Categories.Add(pct);
                            //}
                            ////player.Symbology = myScheme1;

                            //polLayer.Symbology = myScheme1;
                            addRegionLayerToMainMap();
                        }
                    }
                    i++;
                    CommonClass.GBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(iOldGridType);
                }
                WaitClose();
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.Message);
                WaitClose();
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

        /// <summary>
        /// 点击选择 Select Attribute按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                //--------add update modify by xiejp 20120910---------

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

                //--------add update modify by xiejp 20120910---------
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
        /// <summary>
        /// Region的选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataRowView drGrid = cboRegion.SelectedItem as DataRowView;
                string shapeFileName;
                
                if (isLoad)
                {
                    if (mainMap.Layers.Count > 1)
                    {
                        if (CommonClass.RBenMAPGrid == null)
                        {
                            mainMap.Layers.Clear();
                            CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));// GetBenMapGridDefinitions(drGrid);
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
                            for (int i = 0; i < mainMap.Layers.Count; i++)
                            {
                                if (mainMap.Layers[i].LegendText == shapeFileName)
                                {
                                    mainMap.Layers.RemoveAt(i);
                                    break;
                                }
                            }

                            CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));// GetBenMapGridDefinitions(drGrid);
                            addRegionLayerToMainMap();
                        }

                    }
                    else
                    {
                        CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));
                    }
                }

                CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));// GetBenMapGridDefinitions(drGrid);
            }
            catch
            {
            }
        }
        /// <summary>
        /// APV的 SelectAttribute按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                //--------add update modify by xiejp 20120910---------
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
        /// <summary>
        /// QALY的Select Attribute按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                //--------add update modify by xiejp 20120910---------
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
        /// <summary>
        /// 结果排序列的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OLVResultsShow_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            try
            {
                //----------根据tableObject直接LastSortOrder,然后updateTable---------------
                if (_tableObject == null || sender == null) return;
                int i = 0;
                #region CR
                if (_tableObject is List<CRSelectFunctionCalculateValue> || _tableObject is CRSelectFunctionCalculateValue)
                {
                    //first 50 init columns

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
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName).ToList();
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
                            case "location":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Locations).ToList();
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
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName).ToList();
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
                            case "location":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Locations).ToList();
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
                    //lstCRTable.CRCalculateValues = lstCRTable.CRCalculateValues.Where(p => p.Population > 0).ToList();



                    _tableObject = lstCRTable;
                    UpdateTableResult(lstCRTable);
                }
                #endregion
                #region pool incidence
                else if (_tableObject is List<AllSelectCRFunction> || _tableObject is AllSelectCRFunction)
                {
                    //first 50 init columns

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
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName).ToList();
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
                            case "location":
                                lstCRTable = lstCRTable.OrderBy(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Locations).ToList();
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
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName).ToList();
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
                            case "location":
                                lstCRTable = lstCRTable.OrderByDescending(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Locations).ToList();
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
                #endregion
                #region APV
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
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderBy(p => p.AllSelectValuationMethod.Pollutant).ToList();
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
                                lstallSelectValuationMethodAndValue = lstallSelectValuationMethodAndValue.OrderByDescending(p => p.AllSelectValuationMethod.Pollutant).ToList();
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
                    //-----------------------------------初始化table--------------------------------------------------------
                }

                # endregion
                #region QALY
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
                    //-----------------------------------初始化table--------------------------------------------------------
                }
                #endregion
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
                        //Percentile +  i* count/100[" + i + "]

                        // BrightIdeasSoftware.OLVColumn olvPercentile = new BrightIdeasSoftware.OLVColumn() { AspectName = "Values[" + lstAddField[i] + "]", Width = lstAddField[i].Length * 9, Text = lstAddField[i], IsEditable = false }; OLVResultsShow.Columns.Add(olvPercentile);
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
            //string isShow = ConfigurationManager.AppSettings["IsShow"];
            string iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
            string isShow = "T";
            if (System.IO.File.Exists(iniPath))
            {
                isShow = CommonClass.IniReadValue("appSettings", "IsShowStart", iniPath);
            }
            if (isShow == "T")
            { st.Show(); }
            //this.Visible = false;
            //_mainForm.Visible = false;
            //HomePage homePage = new HomePage();
            //homePage.ShowDialog();
            //this.Visible = true;
            //_mainForm.Visible = true;
            //_homePageName = homePage.sPicName;
            //loadHomePageFunction();
        }

        private void btnShowAPVResult_Click(object sender, EventArgs e)
        {
            tlvAPVResult_DoubleClick(sender, e);
        }

        //private void btnShowQALYResult_Click(object sender, EventArgs e)
        //{
        //    tlvQALYResult_DoubleClick(sender, e);
        //}

        private void trvSetting_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            // Draw the node text. 
            Rectangle rec = e.Bounds;
            //rec.Height = 30;
            //SizeF sf = e.Graphics.MeasureString(e.Node.Text, trvSetting.Font);
            //double width = rect.getWidth(); 
            //e.Node.Text = e.Node.Text + Environment.NewLine + Environment.NewLine + "Incomplete"; 


            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            //sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(e.Node.Text, trvSetting.Font, Brushes.Black, Rectangle.Inflate(e.Bounds, 2, 0), sf);

            //e.Graphics.FillRectangle(Brushes, new Rectangle(e.Bounds.Location, new Size(this.trvSetting.Width - e.Bounds.X, e.Bounds.Height)));
            //e.Graphics.DrawString(e.Node.Text, this.trvSetting.Font, Brushes.Black, rec);

        }

        private void spTable_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnResultShow_RefreshItems(object sender, EventArgs e)
        {

        }

        private void tlvIncidence_DoubleClick(object sender, EventArgs e)
        {
            //Selected--> To GIS Show


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
                //List<CRSelectFunctionCalculateValue> lstCRSelectFunctionCalculateValue = new List<CRSelectFunctionCalculateValue>();
                List<AllSelectCRFunction> lstAllSelectCRFunction = new List<AllSelectCRFunction>();
                if ((sender is ObjectListView) || sender is Button)
                {
                    foreach (KeyValuePair<AllSelectCRFunction, string> keyValueCR in olvIncidence.SelectedObjects)
                    {
                        AllSelectCRFunction cr = keyValueCR.Key;

                        if (cr.CRSelectFunctionCalculateValue == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues.Count == 0)
                        {
                            //WaitClose();
                            continue;
                        }
                        else
                        {
                            if(crSelectFunctionCalculateValue==null)
                                crSelectFunctionCalculateValue = cr.CRSelectFunctionCalculateValue;
                            //lstCRSelectFunctionCalculateValue.Add(cr.CRSelectFunctionCalculateValue);
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
                            //WaitClose();
                            continue;
                        }
                        else
                        {
                            if (crSelectFunctionCalculateValue == null)
                                crSelectFunctionCalculateValue = cr.CRSelectFunctionCalculateValue;
                            lstAllSelectCRFunction.Add(cr);
                            //lstCRSelectFunctionCalculateValue.Add(cr.CRSelectFunctionCalculateValue);
                        }

                    }
                    bGIS = false;
                    bChart = false;
                    tabCtlMain.SelectedIndex = 1;
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
                    //--------------做Aggregation----------------
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
                        //初始化table数据！
                        //---------------------------清空原有数据 Map/Table/Chart--------------------------------------------
                        ClearMapTableChart();
                        //-----------------------chart未写---------------------------------------------
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
                            //if (.Count() == 0)
                            //    InitTableResult(crSelectFunctionCalculateValue);
                            //else
                                InitTableResult(lstAllSelectCRFunction);

                        }
                        if (bChart)
                        {
                            //foreach (AllSelectCRFunction cr in olvIncidence.SelectedObjects)
                            //{

                            InitChartResult(crSelectFunctionCalculateValue, (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null) ? CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID : CommonClass.GBenMAPGrid.GridDefinitionID);
                            //}
                        }
                        if (bGIS)
                        {
                            tsbChangeProjection.Text = "change projection to Albers";
                            mainMap.ProjectionModeReproject = ActionMode.Never;
                            mainMap.ProjectionModeDefine = ActionMode.Never;
                            string shapeFileName = "";

                            //if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
                            //    CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null)
                            //{
                            //    incidenceGrid = CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation;
                            //}
                            if (incidenceGrid is ShapefileGrid)
                            {
                                mainMap.Layers.Clear();
                                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (incidenceGrid as ShapefileGrid).ShapefileName + ".shp"))
                                {
                                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (incidenceGrid as ShapefileGrid).ShapefileName + ".shp";
                                    //mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.GBenMAPGrid as ShapefileGrid).ShapefileName + ".shp");
                                }
                            }
                            else if (incidenceGrid is RegularGrid)
                            {
                                mainMap.Layers.Clear();
                                if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (incidenceGrid as RegularGrid).ShapefileName + ".shp"))
                                {
                                    shapeFileName = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (incidenceGrid as RegularGrid).ShapefileName + ".shp";
                                    // mainMap.Layers.Add(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.GBenMAPGrid as RegularGrid).ShapefileName + ".shp");
                                }
                            }

                            //FeatureSet fs = new FeatureSet();
                            //fs.Open(shapeFileName);
                            mainMap.Layers.Add(shapeFileName);

                            DataTable dt = (mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).DataSet.DataTable;
                            (mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).LegendText = "CRResult";
                            (mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).Name = "CRResult";
                            //加值
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
                            dt.Columns.Add("Pooled Incidence", typeof(double));
                            j = 0;
                            while (j < dt.Columns.Count)
                            {
                                if (dt.Columns[j].ColumnName.ToLower() == "col") iCol = j;
                                if (dt.Columns[j].ColumnName.ToLower() == "row") iRow = j;

                                j++;
                            }
                            j = 0;
                            Dictionary<string, double> dicAll = new Dictionary<string, double>();
                            foreach (CRCalculateValue crcv in crSelectFunctionCalculateValue.CRCalculateValues)
                            {
                                if (!dicAll.ContainsKey(crcv.Col + "," + crcv.Row))
                                    dicAll.Add(crcv.Col + "," + crcv.Row, crcv.PointEstimate);
                            }
                            //匹配
                            foreach (DataRow dr in dt.Rows)
                            {
                                try
                                {
                                    //dr["Value"] = crSelectFunctionCalculateValue.CRCalculateValues.Where(p => p.Col == Convert.ToInt32(dr[iCol])
                                    //    && p.Row == Convert.ToInt32(dr[iRow])).Select(p => p.PointEstimate).First();
                                    if (dicAll.ContainsKey(dr[iCol] + "," + dr[iRow]))
                                        dr["Pooled Incidence"] = dicAll[dr[iCol] + "," + dr[iRow]];
                                    else
                                        dr["Pooled Incidence"] = 0;
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                            if (File.Exists(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp")) CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp");
                            (mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).DataSet.SaveAs(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp", true);
                            mainMap.Layers.Clear();
                            mainMap.Layers.Add(CommonClass.DataFilePath + @"\Tmp\CRTemp.shp");
                            (mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).DataSet.DataTable.Columns[(mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer).DataSet.DataTable.Columns.Count - 1].ColumnName = "Pooled Incidence";
                            string author = "Pooled Incidence";
                            if (crSelectFunctionCalculateValue.CRSelectFunction != null && crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction != null
                                && crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author != null)
                            {
                                author = crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author;
                                if (author.IndexOf(" ") > -1)
                                {
                                    author = author.Substring(0, author.IndexOf(" "));
                                }
                            }

                            mainMap.Layers[mainMap.Layers.Count() - 1].LegendText = author;
                            MapPolygonLayer polLayer = mainMap.Layers[mainMap.Layers.Count - 1] as MapPolygonLayer;
                            string strValueField = polLayer.DataSet.DataTable.Columns[polLayer.DataSet.DataTable.Columns.Count - 1].ColumnName;
                            PolygonScheme myScheme1 = new PolygonScheme();
                            float fl = (float)0.1;
                            myScheme1.EditorSettings.StartColor = Color.Blue;
                            myScheme1.EditorSettings.StartColor.ToTransparent(fl);
                            myScheme1.EditorSettings.EndColor = Color.Red;
                            myScheme1.EditorSettings.EndColor.ToTransparent(fl);

                            myScheme1.EditorSettings.ClassificationType = ClassificationType.Quantities;
                            //myScheme1.EditorSettings.ClassificationType = ClassificationType.UniqueValues ;
                            myScheme1.EditorSettings.NumBreaks = 6;
                            myScheme1.EditorSettings.FieldName = strValueField;// "Value";
                            myScheme1.EditorSettings.UseGradient = false;
                            myScheme1.CreateCategories(polLayer.DataSet.DataTable);


                            //----------
                            //UniqueValues+半透明
                            double dMinValue = 0.0;
                            double dMaxValue = 0.0;
                            //Base或者Control，默认都按Base的区间来画，以保证Base和Control都是同一区间
                            dMinValue = crSelectFunctionCalculateValue.CRCalculateValues.Count == 0 ? 0 : crSelectFunctionCalculateValue.CRCalculateValues.Min(a => a.PointEstimate);
                            dMaxValue = crSelectFunctionCalculateValue.CRCalculateValues.Count == 0 ? 0 : crSelectFunctionCalculateValue.CRCalculateValues.Max(a => a.PointEstimate);

                            //Todo:陈志润 20111124修改
                            _dMinValue = dMinValue;
                            _dMaxValue = dMaxValue;
                            _currentLayerIndex = mainMap.Layers.Count - 1;
                            string pollutantUnit = string.Empty;//benMAPLine
                            _columnName = strValueField;
                            RenderMainMap(true);


                            addRegionLayerToMainMap();
                        }
                    }
                    i++;
                    CommonClass.GBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(iOldGridType);
                }
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
            //if (olvIncidence.Objects != null)
            //{
            cbPoolingWindowIncidence_SelectedIndexChanged(sender, e);
            //}
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

        //private void btQALYPooled_Click(object sender, EventArgs e)
        //{
        //    if (btQALYPooled.Text == "Show aggregated")
        //    {
        //        btQALYPooled.Text = "Show pooled";
        //    }
        //    else
        //        btQALYPooled.Text = "Show aggregated";

        //    cbPoolingWindowQALY_SelectedIndexChanged(sender, e);
        //}

        //private void rbQALYOnlyOne_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (rbQALYOnlyOne.Checked)
        //    {
        //        lbPoolingWindowQALY.Enabled = true;
        //        cbPoolingWindowQALY.Enabled = true;
        //    }
        //    else
        //    {
        //        lbPoolingWindowQALY.Enabled = false;
        //        cbPoolingWindowQALY.Enabled = false;

        //    }
        //    if (this.tlvQALYResult.Objects != null)
        //    {
        //        cbPoolingWindowQALY_SelectedIndexChanged(sender, e);
        //    }
        //}

        private void olvCRFunctionResult_Validated(object sender, EventArgs e)
        {

        }

        private void olvIncidence_Validated(object sender, EventArgs e)
        {
            //-----------重画
            foreach (OLVListItem item in (sender as ObjectListView).SelectedItems)
            {
                item.UseItemStyleForSubItems = true;// olvSimple.HighlightBackgroundColor;
                item.BackColor = SystemColors.Highlight;
                item.ForeColor = Color.White;
            }
        }

        private void tlvAPVResult_Validated(object sender, EventArgs e)
        {
            //-----------重画
            foreach (OLVListItem item in (sender as ObjectListView).SelectedItems)
            {
                item.UseItemStyleForSubItems = true;// olvSimple.HighlightBackgroundColor;
                item.BackColor = SystemColors.Highlight;
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
                item.UseItemStyleForSubItems = true;// olvSimple.HighlightBackgroundColor;
                item.BackColor = SystemColors.Highlight;
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
                //double pMin = 0, pMax = 1;
                switch (iCDF)//0-cfgr;1-pooled incidence;2-apvr
                {
                    case 0:
                        List<double> lstp = new List<double>();
                        int p = lstCFGRforCDF[0].CRCalculateValues[0].LstPercentile.Count;
                        double percentile = (double)50 / p * 0.01;
                        //pMin = (double)50 / p * 0.01;
                        for (int i = 0; i < p; i++)
                        {
                            lstp.Add(percentile);
                            percentile = percentile + 100 / p * 0.01;
                        }
                        //pMax = 1 - pMin;
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
                        //pMin = (double)50 / p * 0.01;
                        for (int i = 0; i < p; i++)
                        {
                            lstp.Add(percentile);
                            percentile = percentile + 100 / p * 0.01;
                        }
                        //pMax = 1 - pMin;
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
                        //pMin = (double)50 / p * 0.01;
                        for (int i = 0; i < p; i++)
                        {
                            lstp.Add(percentile);
                            percentile = percentile + 100 / p * 0.01;
                        }
                        //pMax = 1 - pMin;
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
                myPane.IsFontsScaled = false;//图比例变化时候图表上的文字不跟着自动缩放
                myPane.Chart.Fill = new Fill(Color.White, Color.LightGoldenrodYellow, 45.0F);
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


    }//class
}