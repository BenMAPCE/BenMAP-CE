using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Collections;
using BenMAP.APVX;
using System.IO;

namespace BenMAP
{
    public partial class IncidencePoolingandAggregation : FormBase
    {

        private int _operationStatus;
        /// <summary>
        /// 当前操作状态：0->登入界面后没做任何操作；1->登入后做了新增操作；2->登入后做了修改界面参数；3->登入后做了删除操作
        /// </summary>
        public int OperationStatus
        {
            get { return _operationStatus; }
            set
            {
                _operationStatus = value;
            }
        }// Var
        public Dictionary<string, int> _dicPoolingWindowOperation=new Dictionary<string,int>();//-----如果有删除或者增加的操作，则记录
        public IncidencePoolingandAggregation()
        {
            InitializeComponent();
            this.tabControlSelected.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlSelected.DrawItem += new DrawItemEventHandler(tabControlSelected_DrawItem);

            this.tabControlSelected.AllowDrop = true;
        }
        private List<CRSelectFunctionCalculateValue> lstSelectCRSelectFunctionCalculateValue = new List<CRSelectFunctionCalculateValue>();
        //private IncidencePoolingAndAggregationAdvance incidencePoolingAndAggregationAdvance = new IncidencePoolingAndAggregationAdvance();
        private Dictionary<string, List<CRSelectFunctionCalculateValue>> dicTabCR = new Dictionary<string, List<CRSelectFunctionCalculateValue>>();
        private bool isSelectTile = false;
        public List<IncidencePoolingAndAggregation> lstIncidencePoolingAndAggregationOld;
        private Tools.IncidenceBusinessCardRenderer incidenceBusinessCardRenderer = new Tools.IncidenceBusinessCardRenderer();
        private List<int> lstExists = new List<int>();
        private void IncidencePoolingandAggregation_Load(object sender, EventArgs e)
        {
            try
            {
                _operationStatus = 0; //没做任何修改
                if ((CommonClass.LstDelCRFunction == null || CommonClass.LstDelCRFunction.Count == 0) && (CommonClass.LstUpdateCRFunction == null || CommonClass.LstUpdateCRFunction.Count == 0))
                {
                    //btnShowChanges.BackColor = Color.LightBlue;
                    btnShowChanges.Enabled = false;
                }
                else
                {
                    //-----------majie-------
                    btnShowChanges.Enabled = true;
                    this.toolTip1.SetToolTip(btnShowChanges, "Please resolve conflicts between the pooling and configuration files.");
                    //btnShowChanges.BackColor = Color.LightBlue;
                }
                if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null)
                {
                    if (CommonClass.ValuationMethodPoolingAndAggregation != null)
                    {
                        txtOpenExistingCFGR.Text = CommonClass.ValuationMethodPoolingAndAggregation.CFGRPath;
                        ////--------------异步loadCFGR--------------
                        //AsyncCFGRDelegate asyncD = new AsyncCFGRDelegate(AsyncLoadCFGR);
                        //IAsyncResult ar = asyncD.BeginInvoke(txtOpenExistingCFGR.Text, null, null);
                    }
                    //this.Close();
                    //return;
                    //--------------考虑选择后的Tile问题-----------------20120104--点击某节点，如果是叶子结点出来所有平级的Tile，如果是父节点出来所有的叶子结点-----
                    splitContainerTile.Panel2.Hide();
                    splitContainerTile.SplitterDistance = splitContainerTile.Width;
                    int iColumns = 0;
                    foreach (OLVColumn olvc in treeListView.Columns)
                    {
                        BrightIdeasSoftware.OLVColumn olvcTileColumn = new OLVColumn();
                        olvcTileColumn.Text = olvc.Text;
                        olvcTileColumn.AspectName = olvc.AspectName;
                        if (iColumns <= 5)
                        {
                            olvcTileColumn.IsTileViewColumn = true;
                        }
                        else
                            olvcTileColumn.IsVisible = false;
                        olvTile.AllColumns.Add(olvcTileColumn);

                        iColumns++;
                    }
                    //---------------------------------------------------------------------
                    tabControlSelected.TabPages.Clear();
                    tabControlSelected.TabPages.Add("PoolingWindow0", "PoolingWindow0");
                    tabControlSelected.TabPages[0].Controls.Add(this.treeListView);
                    this.olvAvailable.SetObjects(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue);
                    TypedObjectListView<CRSelectFunctionCalculateValue> tlist = new TypedObjectListView<CRSelectFunctionCalculateValue>(this.olvAvailable);
                    tlist.GenerateAspectGetters();
                    this.olvAvailable.TileSize = new Size(120, 90);
                    this.olvTile.TileSize = new Size(120, 110);
                    //this.olvAvailable.ItemRenderer = new Tools.BusinessCardRenderer();
                    //TypedObjectListView<CRSelectFunctionCalculateValue> tlist = new TypedObjectListView<CRSelectFunctionCalculateValue>(this.olvAvailable);
                    //tlist.GenerateAspectGetters();
                    // this.olvAvailable.TileSize = new Size(250, 120);
                    this.olvAvailable.ItemRenderer = incidenceBusinessCardRenderer;// new Tools.BusinessCardRenderer();
                    this.olvTile.ItemRenderer = new Tools.IncidenceBusinessCardRenderer();
                    this.olvTile.OwnerDraw = true;
                    olvAvailable.OwnerDraw = true;
                    cbView.SelectedIndex = 0;
                    //cbSView.SelectedIndex = 0;
                    this.olvAvailable.DropSink = new IncidenceDropSink(true, this);
                    this.treeListView.DropSink = new IncidenceDropSink(true, this);
                    //-------------------------加载已经保存过的-----------------
                    if (CommonClass.lstIncidencePoolingAndAggregation != null &&
                        CommonClass.lstIncidencePoolingAndAggregation.Count > 0)
                    {
                        //--------------赋值原来的lstIncidencePoolingAndAggregationOld
                        lstIncidencePoolingAndAggregationOld = new List<IncidencePoolingAndAggregation>();
                        foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
                        {
                            IncidencePoolingAndAggregation ipold = new IncidencePoolingAndAggregation();
                            ipold.ConfigurationResultsFilePath = ip.ConfigurationResultsFilePath;
                            if (ip.lstAllSelectCRFuntion != null)
                            {
                                ipold.lstAllSelectCRFuntion = new List<AllSelectCRFunction>();
                                foreach (AllSelectCRFunction ascr in ip.lstAllSelectCRFuntion)
                                {
                                    ipold.lstAllSelectCRFuntion.Add(new AllSelectCRFunction()
                                    {
                                        Author = ascr.Author,
                                        CRID = ascr.CRID,
                                        CRIndex = ascr.CRIndex,
                                        CRSelectFunctionCalculateValue = ascr.CRSelectFunctionCalculateValue,
                                        DataSet = ascr.DataSet,
                                        EndAge = ascr.EndAge,
                                        EndPoint = ascr.EndPoint,
                                        EndPointGroup = ascr.EndPointGroup,
                                        EndPointGroupID = ascr.EndPointGroupID,
                                        EndPointID = ascr.EndPointID,
                                        Ethnicity = ascr.Ethnicity,
                                        Function = ascr.Function,
                                        Gender = ascr.Gender,
                                        ID = ascr.ID,
                                        Location = ascr.Location,
                                        Metric = ascr.Metric,
                                        MetricStatistic = ascr.MetricStatistic,
                                        Name = ascr.Name,
                                        NodeType = ascr.NodeType,
                                        OtherPollutants = ascr.OtherPollutants,
                                        PID = ascr.PID,
                                        Pollutant = ascr.Pollutant,
                                        PoolingMethod = ascr.PoolingMethod,
                                        Qualifier = ascr.Qualifier,
                                        Race = ascr.Race,
                                        SeasonalMetric = ascr.SeasonalMetric,
                                        StartAge = ascr.StartAge,
                                        Version = ascr.Version,
                                        Weight = ascr.Weight,
                                        Year = ascr.Year
                                    });
                                }

                            }
                            if (ip.lstColumns != null)
                            {
                                ipold.lstColumns = new List<string>();
                                foreach (string s in ip.lstColumns)
                                {
                                    ipold.lstColumns.Add(s);
                                }
                            }
                            ipold.VariableDataset = ip.VariableDataset;
                            if (ip.Weights != null)
                            {
                                ipold.Weights = new List<double>();
                                ipold.Weights.AddRange(ip.Weights);
                            }
                            ipold.PoolingName = ip.PoolingName;
                            lstIncidencePoolingAndAggregationOld.Add(ipold);

                        }
                        dicTabCR = new Dictionary<string, List<CRSelectFunctionCalculateValue>>();
                        if (!CommonClass.lstIncidencePoolingAndAggregation.Select(p => p.PoolingName).Contains("PoolingWindow0"))
                        {
                            tabControlSelected.TabPages.Clear();
                            tabControlSelected.TabPages.Add(CommonClass.lstIncidencePoolingAndAggregation.First().PoolingName, CommonClass.lstIncidencePoolingAndAggregation.First().PoolingName);
                            tabControlSelected.TabPages[0].Controls.Add(this.treeListView);

                        }
                        foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
                        {
                            if (!istabControlSelectedContainText(ip.PoolingName))
                            {
                                tabControlSelected.TabPages.Add(ip.PoolingName);

                            }
                            if (ip.lstAllSelectCRFuntion != null && ip.lstAllSelectCRFuntion.Count > 0)
                                dicTabCR.Add(ip.PoolingName, ip.lstAllSelectCRFuntion.Where(p => p.CRSelectFunctionCalculateValue != null && p.CRID<9999).Select(a => a.CRSelectFunctionCalculateValue).ToList());
                            else
                                dicTabCR.Add(ip.PoolingName, null);

                        }
                        if (CommonClass.ValuationMethodPoolingAndAggregation != null && CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase != null
                            && CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Count > 0)
                        {
                            foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                            {
                                try
                                {
                                    IncidencePoolingAndAggregation ip = CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == vb.IncidencePoolingAndAggregation.PoolingName).First();
                                    vb.IncidencePoolingAndAggregation = ip;
                                }
                                catch
                                { 
                                }
                            }
                        }

                    }
                    else
                    {
                        CommonClass.lstIncidencePoolingAndAggregation = new List<IncidencePoolingAndAggregation>();
                        if (!istabControlSelectedContainText("PoolingWindow0"))
                            tabControlSelected.TabPages.Add("PoolingWindow0");
                        CommonClass.lstIncidencePoolingAndAggregation.Add(new IncidencePoolingAndAggregation()
                        {
                            PoolingName = "PoolingWindow0"
                        });
                    }
                    tabControlSelected.SelectedIndex = 0;
                    tabControlSelected_SelectedIndexChanged(sender, e);
                    tbPoolingName.Text = tabControlSelected.TabPages[0].Text;
                    //cbPoolingWindow_SelectedIndexOld = 0;
                    //CommonClass.lstIncidencePoolingAndAggregation.First().PoolingName = tbPoolingWindowName.Text;
                    //CommonClass.lstIncidencePoolingAndAggregation.First()Advance = incidencePoolingAndAggregationAdvance;
                    //CommonClass.lstIncidencePoolingAndAggregation.First().IncidencePoolingAndAggregationAdvance = incidencePoolingAndAggregationAdvance;
                    ////CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethods = lstSelected;
                    //tbPoolingWindowName.Text = CommonClass.lstIncidencePoolingAndAggregation.First().PoolingName;
                    //treeListView.SetObjects(CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethods);

                }

                this.treeListView.CheckBoxes = false;
                List<CRSelectFunctionCalculateValue> lstAvailable = (List<CRSelectFunctionCalculateValue>)this.olvAvailable.Objects;
                //-----------------------------绑定DataSet---------------------------
                Dictionary<string, int> DicFilterDataSet = new Dictionary<string, int>();
                DicFilterDataSet.Add("", -1);
                var query = from a in lstAvailable select new { a.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName, a.CRSelectFunction.BenMAPHealthImpactFunction.DataSetID };
                if (query != null && query.Count() > 0)
                {
                    List<KeyValuePair<string, int>> lstFilterDataSet = DicFilterDataSet.ToList();
                    lstFilterDataSet.AddRange(query.Distinct().ToDictionary(p => p.DataSetName, p => p.DataSetID));
                    DicFilterDataSet = lstFilterDataSet.ToDictionary(p => p.Key, p => p.Value);
                    // DicFilterDataSet = query.Distinct().ToDictionary(p => p.DataSetName, p => p.DataSetID);
                }
                BindingSource bs = new BindingSource();

                // DicFilterDataSet = (Dictionary<string, int>)DicFilterDataSet.OrderBy(p => p.Value);
                bs.DataSource = DicFilterDataSet;
                this.cbDataSet.DataSource = bs;
                cbDataSet.DisplayMember = "Key";
                cbDataSet.ValueMember = "Value";
                int maxDatasetWidth = 166;
                int DatasetWidth = 166;
                foreach (var q in DicFilterDataSet)
                {
                    using (Graphics g = this.CreateGraphics())
                    {
                        SizeF string_size = g.MeasureString(q.Key.ToString(), this.Font);
                        DatasetWidth = Convert.ToInt16(string_size.Width) + 50;
                    }
                    maxDatasetWidth = Math.Max(maxDatasetWidth, DatasetWidth);
                }
                cbDataSet.DropDownWidth = maxDatasetWidth;

                Dictionary<string, int> DicFilterGroup = new Dictionary<string, int>();
                DicFilterGroup.Add("", -1);
                var queryGroup = from a in lstAvailable select new { a.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup, a.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID };
                if (queryGroup != null && queryGroup.Count() > 0)
                {
                    List<KeyValuePair<string, int>> lstGroup = DicFilterGroup.ToList();
                    lstGroup.AddRange(queryGroup.Distinct().ToDictionary(p => p.EndPointGroup, p => p.EndPointGroupID));
                    DicFilterGroup = lstGroup.ToDictionary(p => p.Key, p => p.Value);
                }
                BindingSource bsqueryGroup = new BindingSource();

                // DicFilterGroup = (Dictionary<string, int>)DicFilterGroup.OrderBy(p => p.Value);
                bsqueryGroup.DataSource = DicFilterGroup;
                cbEndPointGroup.DataSource = bsqueryGroup;
                cbEndPointGroup.DisplayMember = "Key";
                cbEndPointGroup.ValueMember = "Value";
                int maxEndpointGroupWidth = 154;
                int EndpointGroupWidth = 154;
                foreach (var q in DicFilterGroup)
                {
                    using (Graphics g = this.CreateGraphics())
                    {
                        SizeF string_size = g.MeasureString(q.Key.ToString(), this.Font);
                        EndpointGroupWidth = Convert.ToInt16(string_size.Width) + 50;
                    }
                    maxEndpointGroupWidth = Math.Max(maxEndpointGroupWidth, EndpointGroupWidth);
                }
                cbEndPointGroup.DropDownWidth = maxEndpointGroupWidth;


                //------------bingdingcboPoolingMethod----------------------------------------
                //this.cboPoolingMethod.Items.Add(Enum.GetValues(PoolingMethodTypeEnum));


                //-------------bingding GridType---------------------------
                if (CommonClass.GBenMAPGrid != null)
                {
                    this.txtTargetGridType.Text = CommonClass.GBenMAPGrid.GridDefinitionName;
                }

                //----------------------init treelistview events------------------
                this.treeListView.CanExpandGetter = delegate(object x)
                {
                    try
                    {
                        AllSelectCRFunction dir = (AllSelectCRFunction)x;
                        IncidencePoolingAndAggregation ip = CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text).First();
                        if (ip.lstAllSelectCRFuntion.Where(p => p.PID == dir.ID).Count() > 0)
                            return true;//(dir.NodeType != 4);
                        else
                            return false;
                    }
                    catch
                    {
                        return false;
                    }
                };
                this.treeListView.ChildrenGetter = delegate(object x)
                {
                    AllSelectCRFunction dir = (AllSelectCRFunction)x;
                    try
                    {
                        return getChildFromAllSelectCRFunction(dir);


                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return new List<AllSelectCRFunction>();
                        //return new ArrayList();
                    }
                };

                if (CommonClass.lstIncidencePoolingAndAggregation.First() != null && CommonClass.lstIncidencePoolingAndAggregation.First().lstAllSelectCRFuntion != null)
                {
                    //    treeListView.SetObjects(CommonClass.lstIncidencePoolingAndAggregation.First().lstAllSelectCRFuntion);
                    if (CommonClass.lstIncidencePoolingAndAggregation.First().lstColumns != null && CommonClass.lstIncidencePoolingAndAggregation.First().lstColumns.Count > 0)
                    {
                        updateTreeColumns(ref CommonClass.lstIncidencePoolingAndAggregation.First().lstColumns);
                    }
                    else
                    {
                        updateTreeColumns(ref CommonClass.lstIncidencePoolingAndAggregation.First().lstColumns);

                    }
                    initTreeView(CommonClass.lstIncidencePoolingAndAggregation.First());
                }

               
                //cboPoolingMethod.SelectedIndex = Convert.ToInt32(CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethodType); 
            }
            catch (Exception ex)
            {

            }


        }
        private List<AllSelectCRFunction> getChildFromAllSelectCRFunction(AllSelectCRFunction allSelectValuationMethod)
        {
            try
            {
                List<AllSelectCRFunction> lstAll = new List<AllSelectCRFunction>();
                IncidencePoolingAndAggregation incidencePoolingAndAggregation = CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text).First();

                var query = from a in incidencePoolingAndAggregation.lstAllSelectCRFuntion where a.PID == allSelectValuationMethod.ID select a;
                lstAll = query.ToList();
                if (btShowDetail.Text == "Detailed View" && allSelectValuationMethod.PoolingMethod != "" && allSelectValuationMethod.PoolingMethod != "None")
                {
                    lstAll = new List<AllSelectCRFunction>();
                    lstAll.Add(new AllSelectCRFunction()
                    {
                        PoolingMethod = "",
                        Author = allSelectValuationMethod.Author,
                        ID = incidencePoolingAndAggregation.lstAllSelectCRFuntion.Max(p=>p.ID)+1,
                        CRID = allSelectValuationMethod.CRID,
                        CRIndex = allSelectValuationMethod.CRIndex,
                        DataSet = allSelectValuationMethod.DataSet,
                        EndAge = allSelectValuationMethod.EndAge,
                        EndPoint = allSelectValuationMethod.EndPoint,
                        EndPointGroup = allSelectValuationMethod.EndPointGroup,
                        EndPointGroupID = allSelectValuationMethod.EndPointGroupID,
                        EndPointID = allSelectValuationMethod.EndPointID,
                        Ethnicity = allSelectValuationMethod.Ethnicity,
                        Function = allSelectValuationMethod.Function,
                        Gender = allSelectValuationMethod.Gender,
                        Location = allSelectValuationMethod.Location,
                        Metric = allSelectValuationMethod.Metric,
                        MetricStatistic = allSelectValuationMethod.MetricStatistic,
                        Name = allSelectValuationMethod.Name,
                        NodeType = 100,
                        OtherPollutants = allSelectValuationMethod.OtherPollutants,
                        PID = allSelectValuationMethod.ID,
                        Pollutant = allSelectValuationMethod.Pollutant,
                        Qualifier = allSelectValuationMethod.Qualifier,
                        Race = allSelectValuationMethod.Race,
                        SeasonalMetric = allSelectValuationMethod.SeasonalMetric,
                        StartAge = allSelectValuationMethod.StartAge,
                        Version = allSelectValuationMethod.Version,
                        Year = allSelectValuationMethod.Year,
                        CRSelectFunctionCalculateValue=allSelectValuationMethod.CRSelectFunctionCalculateValue
                    });
                }
                return lstAll;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static void getAllChildFromAllSelectCRFunction(AllSelectCRFunction allSelectValuationMethod,List<AllSelectCRFunction> lstAll, ref List<AllSelectCRFunction> lstAllChild)
        {
            try
            {

                if (lstAllChild == null) lstAllChild = new List<AllSelectCRFunction>();
                List<AllSelectCRFunction> lst = lstAll.Where(p => p.PID == allSelectValuationMethod.ID).ToList();
                if (lst.Count() > 0) lstAllChild.AddRange(lst);
                foreach (AllSelectCRFunction ac in lst)
                {
                    if (ac.PoolingMethod != "")
                    {
                        getAllChildFromAllSelectCRFunction(ac, lstAll, ref lstAllChild);
 
                    }
                }

                 
            }
            catch (Exception ex)
            {
                
            }
        }

        /// <summary>
        /// 初始化TreeViewList
        /// </summary>
        /// <param name="incidencePoolingAndAggregation"></param>
        private void initTreeView(IncidencePoolingAndAggregation incidencePoolingAndAggregation)
        {


            List<AllSelectCRFunction> lstRoot = new List<AllSelectCRFunction>();
            if (incidencePoolingAndAggregation.lstAllSelectCRFuntion == null || incidencePoolingAndAggregation.lstAllSelectCRFuntion.Count == 0)
            {
                incidencePoolingAndAggregation.lstAllSelectCRFuntion = new List<AllSelectCRFunction>();
                treeListView.Objects = null;
            }
            else
            {
                lstRoot.Add(incidencePoolingAndAggregation.lstAllSelectCRFuntion.First());
                for (int i = 1; i < incidencePoolingAndAggregation.lstAllSelectCRFuntion.Count(); i++)
                {
                    if(incidencePoolingAndAggregation.lstAllSelectCRFuntion[i].EndPointGroup!= incidencePoolingAndAggregation.lstAllSelectCRFuntion[i-1].EndPointGroup)
                        lstRoot.Add(incidencePoolingAndAggregation.lstAllSelectCRFuntion[i]);
                }
                treeListView.Roots =lstRoot;// incidencePoolingAndAggregation.lstAllSelectCRFuntion.GetRange(0, 1);
                this.treeColumnName.ImageGetter = delegate(object x)
                {
                    if (((AllSelectCRFunction)x).NodeType==100)
                        return 1;
                    else
                        return 0;
                };
                treeListView.ExpandAll();
                treeListView.RebuildAll(true);
            }
            treeListView.RebuildAll(true);
            treeListView.ExpandAll();


        }
        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            try
            {
                if (CommonClass.IncidencePoolingAndAggregationAdvance == null) CommonClass.IncidencePoolingAndAggregationAdvance = new IncidencePoolingAndAggregationAdvance();
                APVConfigurationAdvancedSettings frm = new APVConfigurationAdvancedSettings();
                frm.IncidencePoolingAndAggregationAdvance = CommonClass.IncidencePoolingAndAggregationAdvance;// incidencePoolingAndAggregationAdvance;
                DialogResult rtn = frm.ShowDialog();
                if (rtn != DialogResult.OK) { return; }

                CommonClass.IncidencePoolingAndAggregationAdvance = frm.IncidencePoolingAndAggregationAdvance;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CommonClass.lstIncidencePoolingAndAggregation = lstIncidencePoolingAndAggregationOld;
            this.DialogResult = DialogResult.Cancel;
        }

        //private void btnBrowse_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        OpenFileDialog openFileDialog = new OpenFileDialog();
        //        openFileDialog.InitialDirectory = Application.StartupPath + @"E:\";
        //        openFileDialog.Filter = "Configuration Results|*.cfgrx";
        //        openFileDialog.FilterIndex = 3;
        //        openFileDialog.RestoreDirectory = true;
        //        if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
        //        cboConfigurationResultFileName.Text = openFileDialog.FileName;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex);
        //    }
        //}
        private string AsyncLoadCFGR(string strFile)
        {

            try
            {
                if (CommonClass.BaseControlCRSelectFunctionCalculateValue == null || CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count == 0 ||
                 CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues == null ||
                  CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues.Count == 0)
                {
                    //---------------如果是APVX存入必须先loadcfgr---------但是要判断是否已经load过，如果已经load则不需要load了--
                    string tip = "Creating Incidence Pooling And Aggregation. Please wait.";
                    WaitShow(tip);
                    //AsyncLoadCFGR(txtOpenExistingCFGR.Text);
                    string err = "";
                    CommonClass.BaseControlCRSelectFunctionCalculateValue = Configuration.ConfigurationCommonClass.LoadCFGRFile(strFile,ref err);//CommonClass.ValuationMethodPoolingAndAggregation.CFGRPath);
                    if (CommonClass.BaseControlCRSelectFunctionCalculateValue == null)
                    {
                        MessageBox.Show(err);
                        return "";
                    }
                    CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue;
                    //----------------do pooling-------
                    foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                    {
                        foreach (AllSelectCRFunction alsc in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.PoolingMethod == "").ToList())
                        {
                            if (alsc.PoolingMethod == "")
                            {
                                alsc.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(p => p.CRSelectFunction.CRID == alsc.CRID).First();
                            }
                        }
                    }
                    //----------------do pooling-------
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
                    //        APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.NodeType != 100).Max(p => p.NodeType), vb.IncidencePoolingAndAggregation.lstColumns);
                    //    }
                    //    //---------------------------------------------------
                    //}
                }
                WaitClose();
                return "";
            }
            catch (Exception ex)
            {
                WaitClose();
                Logger.LogError(ex);
                return "";

            }
        }
        public delegate string AsyncCFGRDelegate(string strFile);

        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                //if (string.IsNullOrEmpty(tbPoolingWindowName.Text))
                //{
                //    MessageBox.Show("Please enter a PoolingWindowName!");
                //    return;
                //}
                //---------------如果是APVX存入必须先loadcfgr---------但是要判断是否已经load过，如果已经load则不需要load了--
                string msg = string.Empty;
                string tip = string.Empty;
                DialogResult rtn;
                MessageForm messageBox;
                if ((CommonClass.LstDelCRFunction != null && CommonClass.LstDelCRFunction.Count > 0) || (CommonClass.LstUpdateCRFunction != null && CommonClass.LstUpdateCRFunction.Count > 0))
                {
                    messageBox = new MessageForm(1);
                    messageBox.Message = "This APV pools studies not found in the CFGRX. BenMAP may not produce correctly pooled results.";// "The HIF has been changed. The system may not work if the APVX is not consistent with the changed HIF!";
                    messageBox.Text = "Continue or back?";
                    messageBox.BTNOneText = "Continue";
                    messageBox.BTNThirdText = "Back to check";
                    messageBox.SetFirstButton();
                    messageBox.SetLabel(697, 40);
                    //messageBox.AcceptButton
                    messageBox.Size = new System.Drawing.Size(710, 139);
                    messageBox.FirstButtonLocation = new Point(490, messageBox.FirstButtonLocation.Y);
                    messageBox.LabelLocation = new Point(7, 20);
                    //messageBox.SetFirstButtonLocation(330, 60);
                    rtn = messageBox.ShowDialog();//Yes No Cancel
                    //msg = "The HIF has been changed. The system may not work if the APVX is not consistent with the changed HIF!";
                    //tip = "Are you sure to continue? ";
                    //rtn = MessageBox.Show(msg, tip, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (rtn == DialogResult.Cancel)
                    { return; }

                }
                if (treeListView.Objects == null)
                {
                    MessageBox.Show("No pooling method is available.");
                    return;
                }
                else if (tabControlSelected.SelectedIndex > -1)
                {
                    IncidencePoolingAndAggregation incidencePoolingAndAggregation = CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text).First();
                    if (incidencePoolingAndAggregation.lstColumns == null || incidencePoolingAndAggregation.lstColumns.Count() == 0)
                    {
                        updateTreeColumns(ref incidencePoolingAndAggregation.lstColumns);
                    }
                    //incidencePoolingAndAggregation.PoolingMethods = treeListView.Objects as List<CRSelectFunctionCalculateValue>;
                    //switch (cboPoolingMethod.SelectedItem.ToString())
                    //{
                    //    case "None":
                    //        incidencePoolingAndAggregation.PoolingMethodType = PoolingMethodTypeEnum.None;
                    //        break;
                    //    case "SumDependent":
                    //        incidencePoolingAndAggregation.PoolingMethodType = PoolingMethodTypeEnum.SumDependent;
                    //        break;
                    //    case "SumIndependent":
                    //        incidencePoolingAndAggregation.PoolingMethodType = PoolingMethodTypeEnum.SumIndependent;
                    //        break;
                    //    case "SubtractionDependent":
                    //        incidencePoolingAndAggregation.PoolingMethodType = PoolingMethodTypeEnum.SubtractionDependent;
                    //        break;
                    //    case "SubtractionIndependent":
                    //        incidencePoolingAndAggregation.PoolingMethodType = PoolingMethodTypeEnum.SubtractionIndependent;
                    //        break;
                    //    case "SubjectiveWeights":
                    //        incidencePoolingAndAggregation.PoolingMethodType = PoolingMethodTypeEnum.SubjectiveWeights;
                    //        break;
                    //    case "RandomOrFixedEffects":
                    //        incidencePoolingAndAggregation.PoolingMethodType = PoolingMethodTypeEnum.RandomOrFixedEffects;
                    //        break;
                    //    case "FixedEffects":
                    //        incidencePoolingAndAggregation.PoolingMethodType = PoolingMethodTypeEnum.FixedEffects;
                    //        break;
                    //}

                }
                foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
                {
                    if (ip.lstAllSelectCRFuntion == null || ip.lstAllSelectCRFuntion.Count == 0)
                    {
                        MessageBox.Show("Please set up all pooling windows first.");

                        return;

                    }

                }
                //lstIncidencePoolingAndAggregationOld = CommonClass.lstIncidencePoolingAndAggregation;
                //--------------赋值原来的lstIncidencePoolingAndAggregationOld
                lstIncidencePoolingAndAggregationOld = new List<IncidencePoolingAndAggregation>();
                foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
                {
                    IncidencePoolingAndAggregation ipold = new IncidencePoolingAndAggregation();
                    ipold.ConfigurationResultsFilePath = ip.ConfigurationResultsFilePath;
                    if (ip.lstAllSelectCRFuntion != null)
                    {
                        ipold.lstAllSelectCRFuntion = new List<AllSelectCRFunction>();
                        foreach (AllSelectCRFunction ascr in ip.lstAllSelectCRFuntion)
                        {
                            ipold.lstAllSelectCRFuntion.Add(new AllSelectCRFunction()
                            {
                                Author = ascr.Author,
                                CRID = ascr.CRID,
                                CRIndex = ascr.CRIndex,
                                CRSelectFunctionCalculateValue = ascr.CRSelectFunctionCalculateValue,
                                DataSet = ascr.DataSet,
                                EndAge = ascr.EndAge,
                                EndPoint = ascr.EndPoint,
                                EndPointGroup = ascr.EndPointGroup,
                                EndPointGroupID = ascr.EndPointGroupID,
                                EndPointID = ascr.EndPointID,
                                Ethnicity = ascr.Ethnicity,
                                Function = ascr.Function,
                                Gender = ascr.Gender,
                                ID = ascr.ID,
                                Location = ascr.Location,
                                Metric = ascr.Metric,
                                MetricStatistic = ascr.MetricStatistic,
                                Name = ascr.Name,
                                NodeType = ascr.NodeType,
                                OtherPollutants = ascr.OtherPollutants,
                                PID = ascr.PID,
                                Pollutant = ascr.Pollutant,
                                PoolingMethod = ascr.PoolingMethod,
                                Qualifier = ascr.Qualifier,
                                Race = ascr.Race,
                                SeasonalMetric = ascr.SeasonalMetric,
                                StartAge = ascr.StartAge,
                                Version = ascr.Version,
                                Weight = ascr.Weight,
                                Year = ascr.Year,


                            });
                        }

                    }
                    if (ip.lstColumns != null)
                    {
                        ipold.lstColumns = new List<string>();
                        foreach (string s in ip.lstColumns)
                        {
                            ipold.lstColumns.Add(s);
                        }
                    }
                    ipold.VariableDataset = ip.VariableDataset;
                    if (ip.Weights != null)
                    {
                        ipold.Weights = new List<double>();
                        ipold.Weights.AddRange(ip.Weights);
                    }
                    ipold.PoolingName = ip.PoolingName;
                    lstIncidencePoolingAndAggregationOld.Add(ipold);

                }
                foreach (IncidencePoolingAndAggregation ipTmp in CommonClass.lstIncidencePoolingAndAggregation)
                {
                    if (ipTmp.lstColumns == null || ipTmp.lstColumns.Count() == 0)
                    {
                        updateTreeColumns(ref ipTmp.lstColumns);
                    }
                    if (ipTmp.lstAllSelectCRFuntion.Select(p => p.PoolingMethod).Contains("User Defined Weights"))
                    {
                        //--------------------加载weight-------------------------------------
                        SelectSubjectiveWeight frmAPV = new SelectSubjectiveWeight(ipTmp);
                        DialogResult rtnAPV = frmAPV.ShowDialog();
                        if (rtnAPV != DialogResult.OK) { return; }
                        ipTmp.Weights = frmAPV.dicAllWeight.Values.ToList();
                        //CommonClass.lstIncidencePoolingAndAggregation.First().Weights = frmAPV.dicAllWeight.Values.ToList();
                        //-------------------得到结果----------------------------------------
                    }
                }
                //CommonClass.IncidencePoolingAndAggregationAdvance = incidencePoolingAndAggregationAdvance;
                if (txtOpenExistingCFGR.Text != "")
                {
                    //string tip = "Creating Incidence Pooling And Aggregation.\r\nPlease wait!";
                    //WaitShow(tip);
                    AsyncLoadCFGR(txtOpenExistingCFGR.Text);
                    //WaitClose();
                }
                else
                {
                    //----------------do pooling-------
                    //foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                    //{
                    //    foreach (AllSelectCRFunction alsc in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.PoolingMethod == "").ToList())
                    //    {
                    //        if (alsc.PoolingMethod == "")
                    //        {
                    //            try
                    //            {
                    //                alsc.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(p => p.CRSelectFunction.CRID == alsc.CRID).First();
                    //            }
                    //            catch
                    //            {

                    //            }
                    //        }
                    //    }
                    //}
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
                    //        APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.NodeType != 100).Max(p => p.NodeType), vb.IncidencePoolingAndAggregation.lstColumns);
                    //    }
                    //    //---------------------------------------------------
                    //}
                }

                // Todo:------------陈志润--------
                msg = "The HIF studies or pooling methods may have been changed. If the HIF studies are not consistent with the pooling setup, incorrect results may be generated.";
                tip = "Load the previous valuation settings? ";
                if (CommonClass.ValuationMethodPoolingAndAggregation == null) { CommonClass.ValuationMethodPoolingAndAggregation = new ValuationMethodPoolingAndAggregation(); }
                else if (_operationStatus != 0)
                {

                    messageBox = new MessageForm(-1);
                    messageBox.Message = msg;
                    messageBox.Size = new System.Drawing.Size(700, 139);
                    messageBox.LabelLocation = new Point(7, 11);
                    messageBox.Text = "Load the previous valuation settings?";
                    messageBox.BTNOneText = "Open previous valuation settings";
                    messageBox.BTNSecondText = "Reset valuation settings";
                    messageBox.BTNThirdText = "Cancel";
                    messageBox.SetFirstButton();

                    rtn = messageBox.ShowDialog();//Yes No Cancel
                    //rtn = MessageBox.Show(msg, tip, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    if (rtn == DialogResult.No)
                    {
                        if (CommonClass.ValuationMethodPoolingAndAggregation != null && _dicPoolingWindowOperation != null)
                        {
                            foreach (KeyValuePair<string, int> k in _dicPoolingWindowOperation)
                            {
                                if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == k.Key).Count() > 0)
                                {
                                    CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == k.Key).First().IncidencePoolingAndAggregation = CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == k.Key).First();
                                    CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == k.Key).First().LstAllSelectValuationMethod = null;
                                    CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == k.Key).First().lstAllSelectQALYMethod = null;
                                    CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == k.Key).First().lstAllSelectQALYMethodAndValue = null;
                                    CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == k.Key).First().LstAllSelectValuationMethodAndValue = null;


                                }
                            }
                        }
                        //switch (_operationStatus)
                        //{
                        //    case 0:// 没做任何操作
                        //        if (CommonClass.ValuationMethodPoolingAndAggregation == null)
                        //        {
                        //            // CommonClass.ValuationMethodPoolingAndAggregation = new ValuationMethodPoolingAndAggregation();
                        //        }
                        //        break;
                        //    case 1:// 新增
                        //    case 2:// 修改
                        //    case 3:// 删除
                        //        //------------modify by xiejp--只有有修改的才重新reset!
                        //       // CommonClass.ValuationMethodPoolingAndAggregation = null;// new ValuationMethodPoolingAndAggregation();
                        //        if (CommonClass.ValuationMethodPoolingAndAggregation != null&&_dicPoolingWindowOperation!=null)
                        //        {
                        //            foreach (KeyValuePair<string, int> k in _dicPoolingWindowOperation)
                        //            {
                        //                if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == k.Key).Count() >= 0)
                        //                {
                        //                    CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == k.Key).First().IncidencePoolingAndAggregation = CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == k.Key).First();
                        //                    CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == k.Key).First().LstAllSelectValuationMethod = null;
                        //                    CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == k.Key).First().lstAllSelectQALYMethod = null;
                        //                    CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == k.Key).First().lstAllSelectQALYMethodAndValue = null;
                        //                    CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == k.Key).First().LstAllSelectValuationMethodAndValue = null;

 
                        //                }
                        //            }
                        //        }
                        //        break;
                        //}
                    }//if_rtn
                    else if (rtn == DialogResult.Cancel)
                    { return; }
                }//else not null


                SelectValuationMethods frm = new SelectValuationMethods();
                rtn = frm.ShowDialog();
                if (rtn != DialogResult.OK) { return; }
                else
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                return;

                //----------------------------------------------
                List<CRSelectFunctionCalculateValue> lstSelected = treeListView.Objects as List<CRSelectFunctionCalculateValue>;
                //if (CommonClass.lstIncidencePoolingAndAggregation.First() == null)
                //{
                //    CommonClass.lstIncidencePoolingAndAggregation.First() = new IncidencePoolingAndAggregation();

                //}
                //CommonClass.lstIncidencePoolingAndAggregation.First().PoolingName = tbPoolingWindowName.Text;
                //CommonClass.IncidencePoolingAndAggregationAdvance = incidencePoolingAndAggregationAdvance;
                //CommonClass.lstIncidencePoolingAndAggregation.First().IncidencePoolingAndAggregationAdvance = incidencePoolingAndAggregationAdvance;
                //CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethods = lstSelected;
                //switch (cboPoolingMethod.SelectedItem.ToString())
                //{
                //    case "None":
                //        CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethodType = PoolingMethodTypeEnum.None;
                //        break;
                //    case "SumDependent":
                //        CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethodType = PoolingMethodTypeEnum.SumDependent;
                //        break;
                //    case "SumIndependent":
                //        CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethodType = PoolingMethodTypeEnum.SumIndependent;
                //        break;
                //    case "SubtractionDependent":
                //        CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethodType = PoolingMethodTypeEnum.SubtractionDependent;
                //        break;
                //    case "SubtractionIndependent":
                //        CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethodType = PoolingMethodTypeEnum.SubtractionIndependent;
                //        break;
                //    case "SubjectiveWeights":
                //        CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethodType = PoolingMethodTypeEnum.SubjectiveWeights;
                //        break;
                //    case "RandomOrFixedEffects":
                //        CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethodType = PoolingMethodTypeEnum.RandomOrFixedEffects;
                //        break;
                //    case "FixedEffects":
                //        CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethodType = PoolingMethodTypeEnum.FixedEffects;
                //        break;
                //}
                // CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethodType =  cboPoolingMethod.SelectedItem.ToString()  ;
                foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
                {
                    if (ip.lstAllSelectCRFuntion.Select(p => p.PoolingMethod).Contains("User Defined Weights"))
                    {
                        //--------------------加载weight-------------------------------------
                        SelectSubjectiveWeight frmAPV = new SelectSubjectiveWeight(ip);
                        DialogResult rtnAPV = frmAPV.ShowDialog();
                        if (rtnAPV != DialogResult.OK) { return; }
                        CommonClass.lstIncidencePoolingAndAggregation.First().Weights = frmAPV.dicAllWeight.Values.ToList();
                        //-------------------得到结果----------------------------------------
                    }
                }
                CommonClass.lstIncidencePoolingAndAggregation.First().ConfigurationResultsFilePath = "";//--------------------------现在没有用到。
                CommonClass.ValuationMethodPoolingAndAggregation = null;
                SelectValuationMethods frm2 = new SelectValuationMethods();
                DialogResult rtn2 = frm2.ShowDialog();
                if (rtn2 != DialogResult.OK) { return; }
                else
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
            }
            catch (Exception ex)
            {

            }
        }
        #region 等待窗口
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
            if (waitMess.InvokeRequired)
                waitMess.Invoke(new CloseFormDelegate(DoCloseJob));
            else
                DoCloseJob();
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
        #endregion 等待窗口
        private void cbView_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (cbView.SelectedIndex == 0 && this.olvAvailable.OwnerDraw)
            //    this.olvAvailable.TileSize = new Size(300, 130);

            this.ChangeView(this.olvAvailable, (ComboBox)sender);
        }
        private void ChangeView(ObjectListView listview, ComboBox comboBox)
        {
            // Handle restrictions on Tile view
            if (comboBox.SelectedIndex == 0)
            {
                if (listview.VirtualMode)
                {
                    MessageBox.Show("Sorry, Virtual lists can't use Tile view under Microsoft framework.", "Object List View Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (listview.CheckBoxes)
                {
                    MessageBox.Show("Tile view can't have checkboxes under Microsoft framework., so CheckBoxes have been turned off.", "Object List View Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    listview.CheckBoxes = false;
                }
            }

            switch (comboBox.SelectedIndex)
            {
                //case 0:
                //    listview.View = View.SmallIcon;
                //    break;
                //case 1:
                //    listview.View = View.LargeIcon;
                //    break;
                //case 2:
                //    listview.View = View.List;
                //    break;
                case 0:
                    listview.View = View.Tile;

                    break;
                case 1:
                    listview.View = View.Details;
                    break;
            }
            // listview.Refresh();
        }

        private void textBoxFilterSimple_TextChanged(object sender, EventArgs e)
        {
            this.olvcDataSet.ValuesChosenForFiltering.Clear();
            this.olvcEndPointGroup.ValuesChosenForFiltering.Clear();
            this.TimedFilter(this.olvAvailable, textBoxFilterSimple.Text);
        }
        void TimedFilter(ObjectListView olv, string txt)
        {
            this.TimedFilter(olv, txt, 0);
        }

        void TimedFilter(ObjectListView olv, string txt, int matchKind)
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
                //olv.DefaultRenderer = new HighlightTextRenderer { Filter = filter, UseGdiTextRendering = false };
            }

            // Some lists have renderers already installed
            //HighlightTextRenderer highlightingRenderer = olv.GetColumn(0).Renderer as HighlightTextRenderer;
            //if (highlightingRenderer != null)
            //    highlightingRenderer.Filter = filter;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            olv.ModelFilter = filter;
            stopWatch.Stop();

            IList objects = olv.Objects as IList;
            //if (objects == null)
            //    this.toolStripStatusLabel1.Text =
            //        String.Format("Filtered in {0}ms", stopWatch.ElapsedMilliseconds);
            //else
            //    this.toolStripStatusLabel1.Text =
            //        String.Format("Filtered {0} items down to {1} items in {2}ms",
            //                      objects.Count,
            //                      olv.Items.Count,
            //                      stopWatch.ElapsedMilliseconds);
        }

        private void cbGroups_CheckedChanged(object sender, EventArgs e)
        {
            ShowGroupsChecked(this.olvAvailable, (CheckBox)sender);
        }
        void ShowGroupsChecked(ObjectListView olv, CheckBox cb)
        {
            if (cb.Checked && olv.View == View.List)
            {
                cb.Checked = false;
                MessageBox.Show("ListView's cannot show groups when in List view.", "Object List View Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                olv.ShowGroups = cb.Checked;
                olv.BuildList();
            }

        }

        private void cbDataSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            // this.olvAvailable.
            ObjectListView olv = olvAvailable;
            if (olv == null || olv.IsDisposed)
                return;
            OLVColumn column = olv.GetColumn("olvcDataSet");

            // Collect all the checked values
            ArrayList chosenValues = new ArrayList();
            KeyValuePair<string, int> kvp = (KeyValuePair<string, int>)cbDataSet.SelectedItem;
            if (!string.IsNullOrEmpty(kvp.Key))
            {
                chosenValues.Add(kvp.Key);
                //foreach (object x in checkedList.CheckedItems)
                //{
                //    ICluster cluster = x as ICluster;
                //    if (cluster != null)
                //    {
                //        chosenValues.Add(cluster.ClusterKey);
                //    }
                //}
                olvcDataSet.ValuesChosenForFiltering = chosenValues;
                if (olvcDataSet.IsVisible == false)
                {
                    olvcDataSet.IsVisible = true;
                    olv.RebuildColumns();
                    olv.UpdateColumnFiltering();
                    olvcDataSet.IsVisible = false;
                    olv.RebuildColumns();
                }
                else
                olv.UpdateColumnFiltering();
            }
            else
            {
                olvcDataSet.ValuesChosenForFiltering.Clear();
                if (olvcDataSet.IsVisible == false)
                {
                    olvcDataSet.IsVisible = true;
                    olv.RebuildColumns();
                    olv.UpdateColumnFiltering();
                    olvcDataSet.IsVisible = false;
                    olv.RebuildColumns();
                }
                else
                olv.UpdateColumnFiltering();
            }
            //olv.Refresh();

        }

        private void cbEndPointGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            // this.olvAvailable.
            ObjectListView olv = olvAvailable;
            if (olv == null || olv.IsDisposed)
                return;
            OLVColumn column = olv.GetColumn("olvcEndPointGroup");

            // Collect all the checked values
            ArrayList chosenValues = new ArrayList();
            KeyValuePair<string, int> kvp = (KeyValuePair<string, int>)cbEndPointGroup.SelectedItem;
            if (!string.IsNullOrEmpty(kvp.Key))
            {
                chosenValues.Add(kvp.Key);
                //foreach (object x in checkedList.CheckedItems)
                //{
                //    ICluster cluster = x as ICluster;
                //    if (cluster != null)
                //    {
                //        chosenValues.Add(cluster.ClusterKey);
                //    }
                //}
                olvcEndPointGroup.ValuesChosenForFiltering = chosenValues;
                olv.UpdateColumnFiltering();

            }
            else
            {
                olvcEndPointGroup.ValuesChosenForFiltering.Clear();
                olv.UpdateColumnFiltering();

            }

        }
        private void treeListView_CellEditFinishing(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            if (e.Column.Text == "Pooling Method")
            {

                // Stop listening for change events
                ((ComboBox)e.Control).SelectedIndexChanged -= new EventHandler(cbPoolingMethod_SelectedIndexChanged);

                // Any updating will have been down in the SelectedIndexChanged event handler
                // Here we simply make the list redraw the involved ListViewItem
                ((TreeListView)sender).RefreshItem(e.ListViewItem);
                ((ComboBox)e.Control).Dispose();
                // We have updated the model object, so we cancel the auto update
                e.Cancel = true;
                // Todo:陈志润 20120116
               // _operationStatus = 2;//修改
            }
        }
        private void treeListView_CellEditStarting(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            base.OnClick(e);
            if (e.Column == null) return;
            AllSelectCRFunction asvm = (AllSelectCRFunction)e.RowObject;

            if (e.Column.Text == "Pooling Method" && asvm.NodeType != 100)
            {

                ComboBox cb = new ComboBox();
                cb.Bounds = e.CellBounds;
                cb.Font = ((TreeListView)sender).Font;
                cb.DropDownStyle = ComboBoxStyle.DropDownList;
                if (!CommonClass.CRRunInPointMode)
                {
                    //   cb.Items.AddRange(Enum.GetNames(typeof(PoolingMethodTypeEnum)));
                    cb.Items.Add("None");
                    cb.Items.Add("Sum Dependent");
                    cb.Items.Add("Sum Independent");
                    cb.Items.Add("Subtraction Dependent");
                    cb.Items.Add("Subtraction Independent");
                    cb.Items.Add("User Defined Weights");
                    cb.Items.Add("Random Or Fixed Effects");
                    cb.Items.Add("Fixed Effects");
                }
                else
                {
                    //  None = 0, SumDependent = 1, SumIndependent = 2, SubtractionDependent = 3, SubtractionIndependent = 4, SubjectiveWeights = 5, RandomOrFixedEffects = 6, FixedEffects = 7
                    cb.Items.Add("None");
                    cb.Items.Add("Sum Dependent");
                    cb.Items.Add("Subtraction Dependent");
                    cb.Items.Add("User Defined Weights");


                }
                //------------暂时去掉Weight因为没有做--------------------------------------------------------

                //cb.Items.Remove("SubjectiveWeights");
                //----------------------------------------xjp---------------------------------
                if (e.Value != null)
                    cb.SelectedText = e.Value.ToString();// Math.Max(0, Math.Min(cb.Items.Count - 1, ((int)e.Value) / 10));
               
                cb.SelectedIndexChanged += new EventHandler(cbPoolingMethod_SelectedIndexChanged);
                cb.Tag = e.RowObject; // remember which person we are editing
                e.Control = cb;
            }
            else
            {
                e.Cancel = true;
            }
        }

        void cb_MouseLeave(object sender, EventArgs e)
        {
            treeListView.FinishCellEdit();
        }


        void cbPoolingMethod_SelectedIndexChanged(object sender, EventArgs e)
        {


            ComboBox cb = (ComboBox)sender;
            if (((AllSelectCRFunction)cb.Tag).PoolingMethod == cb.Text) return;
            //Todo:陈志润20120115 提示
            _operationStatus = 2;// ?
            ((AllSelectCRFunction)cb.Tag).PoolingMethod = cb.Text;
            IncidencePoolingAndAggregation ip = CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text).First();
            ip.lstAllSelectCRFuntion.Where(p => p.ID == ((AllSelectCRFunction)cb.Tag).ID).First().PoolingMethod = cb.Text;
            if (_dicPoolingWindowOperation.ContainsKey(ip.PoolingName))
            {
                _dicPoolingWindowOperation[ip.PoolingName] = 3;
            }
            else
            {
                _dicPoolingWindowOperation.Add(ip.PoolingName, 3);
            }
            int iTop = Convert.ToInt32(treeListView.TopItemIndex.ToString());
            if (btShowDetail.Text == "Detailed View")
            {
                treeListView.RebuildAll(true);
                treeListView.ExpandAll();
            }
            treeListView.TopItemIndex = iTop;
            //lstAllSelectQALYMethod.Where(p => p.ID == ((AllSelectQALYMethod)cb.Tag).ID).First().PoolingMethod = cb.Text;
            //if (cb.Text == "SubjectiveWeights")
            //{
            //    btnOK.Enabled = false;
            //    btnSave.Enabled = false;
            //    btnNext.Enabled = true;

            //}
        }
        public void btAddCRFunctions_Click(object sender, EventArgs e)
        {
            try
            {
                if (btShowDetail.Text == "Detailed View")
                {
                    //MessageBox.Show("Detail status first!");
                    //btShowDetail.Text = "Show Brief";
                    btShowDetail_Click(sender, e);
                    return;
 
                }
                IncidencePoolingAndAggregation ip = CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text).First();
                if (!dicTabCR.ContainsKey(tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text))
                {
                    dicTabCR.Add(tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text, new List<CRSelectFunctionCalculateValue>());
                }
                List<CRSelectFunctionCalculateValue> lstAvailable = new List<CRSelectFunctionCalculateValue>();
                List<string> lstAvalilableEndPointGroup = new List<string>();
                foreach (CRSelectFunctionCalculateValue cr in olvAvailable.SelectedObjects)
                {
                    lstAvailable.Add(cr);
                    if(!lstAvalilableEndPointGroup.Contains(cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup))
                    {
                        lstAvalilableEndPointGroup.Add(cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup);
                    }
                }
                //--modify by xiejp 修正允许多个EndPointGroup在一个PoolingWindow里面20130109
                //if (lstAvailable.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID).Distinct().Count() > 1)
                //{
                //    MessageBox.Show("The Pooling needs the same EndPointGroup!");
                //    return;
                //}
                //-----------------------To-----------------
                if (dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text] == null) dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text] = new List<CRSelectFunctionCalculateValue>();
                if (ip.lstAllSelectCRFuntion != null && ip.lstAllSelectCRFuntion.Count > 0)
                {
                    dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text] = ip.lstAllSelectCRFuntion.Where(p => p.NodeType == 100 && p.CRSelectFunctionCalculateValue != null && p.CRSelectFunctionCalculateValue.CRSelectFunction != null).Select(p => p.CRSelectFunctionCalculateValue).ToList();
                }
                else
                {
                    dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text] = new List<CRSelectFunctionCalculateValue>();

                }

                if (dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text] != null && dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text].Count > 0)
                {
                    //--modify by xiejp 修正允许多个EndPointGroup在一个PoolingWindow里面20130109
                    //if (dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text].First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID != lstAvailable.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID)
                    //{
                    //    MessageBox.Show("The Pooling needs the same EndPointGroup!");
                    //    return;
                    //}
                }

                dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text].AddRange(lstAvailable);
                List<BrightIdeasSoftware.OLVColumn> lstOLVColumns = new List<OLVColumn>();
                foreach (BrightIdeasSoftware.OLVColumn olvc2 in this.treeListView.Columns)
                {
                    lstOLVColumns.Add(olvc2);
                }
                lstOLVColumns = lstOLVColumns.OrderBy(p => p.DisplayIndex).ToList();
                lstOLVColumns = lstOLVColumns.Where(p => p.DisplayIndex != 0 && p.DisplayIndex != 1).OrderBy(p => p.DisplayIndex).ToList();

                List<AllSelectCRFunction> lstAllSelectCRFunction = new List<AllSelectCRFunction>();// getLstAllSelectCRFunction(dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text], lstOLVColumns.Select(p => p.Text).ToList(), dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text].First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup, -1);
                //------首先对所有的CR分组--然后各自计算不同的EndPointGroup的list--然后把他们拼在一块
                Dictionary<string, List<CRSelectFunctionCalculateValue>> dicEndPointGroupCR = new Dictionary<string, List<CRSelectFunctionCalculateValue>>();
                foreach (CRSelectFunctionCalculateValue cr in dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text])
                {
                    if (dicEndPointGroupCR.ContainsKey(cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup))
                        dicEndPointGroupCR[cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup].Add(cr);
                    else
                    {
                        dicEndPointGroupCR.Add(cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup, new List<CRSelectFunctionCalculateValue>());
                        dicEndPointGroupCR[cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup].Add(cr);
                    }
                }
                lstAllSelectCRFunction = ip.lstAllSelectCRFuntion;
                if (lstAllSelectCRFunction == null) lstAllSelectCRFunction = new List<AllSelectCRFunction>();
                List<AllSelectCRFunction> lstRemoveAllSelectCRFuntion = ip.lstAllSelectCRFuntion.Where(p => lstAvalilableEndPointGroup.Contains(p.EndPointGroup)).ToList();
                foreach (AllSelectCRFunction ascr in lstRemoveAllSelectCRFuntion)
                {
                    lstAllSelectCRFunction.Remove(ascr);
                }
                foreach (KeyValuePair<string, List<CRSelectFunctionCalculateValue>> k in dicEndPointGroupCR)
                {
                    if (!lstAvalilableEndPointGroup.Contains(k.Key)) continue;
                    List<AllSelectCRFunction> lstTemp = getLstAllSelectCRFunction(k.Value,lstOLVColumns.Select(p => p.Text).ToList(),k.Key, -1);
                    //------把所有id增加lstAllSelectCRFunction.count;
                    if (lstAllSelectCRFunction.Count() > 0)
                    {
                        for (int iTemp = 0; iTemp < lstTemp.Count; iTemp++)
                        {
                            lstTemp[iTemp].ID = lstTemp[iTemp].ID + lstAllSelectCRFunction.Max(p => p.ID) + 1;
                            if(lstTemp[iTemp].PID!=-1)
                            lstTemp[iTemp].PID = lstTemp[iTemp].PID + lstAllSelectCRFunction.Max(p => p.ID) + 1;
                        }
                    }
                    if (lstTemp != null && lstTemp.Count > 0) lstAllSelectCRFunction.AddRange(lstTemp);
                }
                _operationStatus = 1;//新增
                if (_dicPoolingWindowOperation.ContainsKey(ip.PoolingName))
                {
                    _dicPoolingWindowOperation[ip.PoolingName] = 1;
                }
                else
                {
                    _dicPoolingWindowOperation.Add(ip.PoolingName, 1);
                }
                ip.lstAllSelectCRFuntion = lstAllSelectCRFunction;
                if (dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text] != null && dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text].Count > 0)
                    incidenceBusinessCardRenderer.lstExists = dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text].Select(p => p.CRSelectFunction.CRID).ToList();
                else
                    incidenceBusinessCardRenderer.lstExists = new List<int>();
                olvAvailable.Refresh();
                initTreeView(ip);
            }
            catch
            { }
            return;
            try
            {
                List<CRSelectFunctionCalculateValue> lstAvailable = new List<CRSelectFunctionCalculateValue>();// (List<CRSelectFunctionCalculateValue>)this.olvAvailable.SelectedObjects;

                //if (treeListView.Items.Count > 0 &&lstSelectCRSelectFunctionCalculateValue.Count==0)
                //{
                //    foreach (CRSelectFunctionCalculateValue crc in treeListView.Objects)
                //    {
                //        lstSelectCRSelectFunctionCalculateValue.Add(crc);
                //    }
                //}
                foreach (CRSelectFunctionCalculateValue cr in olvAvailable.SelectedObjects)
                {
                    lstAvailable.Add(cr);
                    //if (lstSelectCRSelectFunctionCalculateValue != null && lstSelectCRSelectFunctionCalculateValue.Count != 0 && cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup != lstSelectCRSelectFunctionCalculateValue.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup)
                    //{
                    //    MessageBox.Show("The Pooling needs the same EndPointGroup!");
                    //    return;
                    //}
                    //else if (lstAvailable.Count > 1 && lstAvailable[lstAvailable.Count - 2].CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup != cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup)
                    //{
                    //    MessageBox.Show("The Pooling needs the same EndPointGroup!");
                    //    return;

                    //}
                }
                if (lstAvailable.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID).Distinct().Count() > 1)
                {
                    MessageBox.Show("Pooling requires that functions have the same endpoint group.");
                    return;
                }
                //------------------首先更新对应的Incidence------------------
                IncidencePoolingAndAggregation ip = CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text).First();
                if (ip.lstAllSelectCRFuntion != null && ip.lstAllSelectCRFuntion.Count > 0)
                {
                    if (lstAvailable.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID != ip.lstAllSelectCRFuntion.Where(a => a.NodeType == 4).First().CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID)
                    {
                        MessageBox.Show("Pooling requires that functions have the same endpoint group.");
                        return;

                    }
                    //------------重造
                    lstAvailable = new List<CRSelectFunctionCalculateValue>();
                    var queryCRID = ip.lstAllSelectCRFuntion.Where(a => a.NodeType == 4).Select(p => p.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.ID);
                    foreach (CRSelectFunctionCalculateValue cr in olvAvailable.SelectedObjects)
                    {
                        //if(!queryCRID.Contains(cr.CRSelectFunction.BenMAPHealthImpactFunction.ID))
                        lstAvailable.Add(cr);

                    }

                    var query = lstAvailable.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint).ToList().Distinct().ToList();
                    int i = ip.lstAllSelectCRFuntion.Max(p => p.ID) + 1;
                    int iEndPoint = 0, iAuthor = 0, iQualifier = 0;
                    string strTemp = "";
                    for (int iquery = 0; iquery < query.Count(); iquery++)
                    {
                        strTemp = query[iquery];
                        var queryEndPoint = ip.lstAllSelectCRFuntion.Where(p => p.NodeType == 1 && p.Name == strTemp).ToList();
                        if (queryEndPoint.Count() == 0)
                        {
                            ip.lstAllSelectCRFuntion.Add(new AllSelectCRFunction()
                            {
                                NodeType = 1,
                                // EndPointGroupID = lstAvailable.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
                                ID = i,
                                Name = query[iquery],
                                PID = 0,
                                PoolingMethod = "None",


                            });
                            iEndPoint = i;
                            i++;
                        }
                        else
                        {
                            iEndPoint = queryEndPoint.First().ID;
                        }
                        //---------Add Author------------
                        var author = lstAvailable.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint == query[iquery]).Select(a => a.CRSelectFunction.BenMAPHealthImpactFunction.Author).Distinct().ToList();
                        for (int iauthor = 0; iauthor < author.Count; iauthor++)
                        {
                            var queryAuthor = ip.lstAllSelectCRFuntion.Where(p => p.NodeType == 2 && p.Name == author[iauthor] && p.PID == iEndPoint);
                            if (queryAuthor.Count() == 0)
                            {

                                ip.lstAllSelectCRFuntion.Add(new AllSelectCRFunction()
                                {
                                    NodeType = 2,
                                    // EndPointGroupID = lstAvailable.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
                                    ID = i,
                                    Name = author[iauthor],
                                    PID = iEndPoint,
                                    PoolingMethod = "None",


                                });
                                iAuthor = i;
                                i++;
                            }
                            else
                            {
                                iAuthor = queryAuthor.First().ID;

                            }
                            var Qualifier = lstAvailable.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Author == author[iauthor] && p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint == query[iquery]).Select(a => a.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier).Distinct().ToList();
                            for (int iQ = 0; iQ < Qualifier.Count; iQ++)
                            {
                                var queryQualifier = ip.lstAllSelectCRFuntion.Where(p => p.NodeType == 3 && p.Name == Qualifier[iQ] && p.PID == iAuthor);
                                if (queryQualifier.Count() == 0)
                                {
                                    ip.lstAllSelectCRFuntion.Add(new AllSelectCRFunction()
                                    {
                                        NodeType = 3,
                                        // EndPointGroupID = lstAvailable.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
                                        ID = i,
                                        Name = Qualifier[iQ],
                                        PID = iAuthor,
                                        PoolingMethod = "None",
                                        //CRSelectFunctionCalculateValue= lstAvailable[ifunction]


                                    });
                                    iQualifier = i;
                                    i++;

                                }
                                else
                                {
                                    iQualifier = queryQualifier.First().ID;
                                }
                                //------------function一级
                                var funtion = lstAvailable.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Author == author[iauthor] && p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint == query[iquery] && p.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier == Qualifier[iQ]).ToList();
                                for (int ifunction = 0; ifunction < funtion.Count; ifunction++)
                                {
                                    ip.lstAllSelectCRFuntion.Add(new AllSelectCRFunction()
                                    {
                                        NodeType = 4,
                                        // EndPointGroupID = lstAvailable.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
                                        ID = i,
                                        Name = funtion[ifunction].CRSelectFunction.BenMAPHealthImpactFunction.strLocations,
                                        PID = iQualifier,
                                        //PoolingMethod =null,
                                        CRSelectFunctionCalculateValue = funtion[ifunction]


                                    });
                                    i++;
                                }
                            }

                        }


                    }



                    //initTreeView(ip);

                }
                else if (ip.lstAllSelectCRFuntion == null || ip.lstAllSelectCRFuntion.Count == 0)
                {
                    ip.lstAllSelectCRFuntion = new List<AllSelectCRFunction>();
                    ip.lstAllSelectCRFuntion.Add(new AllSelectCRFunction()
                    {
                        NodeType = 0,
                        EndPointGroupID = lstAvailable.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
                        ID = 0,
                        Name = lstAvailable.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup,
                        PID = -1,
                        PoolingMethod = "None",


                    });
                    var query = lstAvailable.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint).ToList().Distinct().ToList();
                    int i = 1;
                    int iEndPoint = 0, iAuthor = 0, iQualifier = 0;
                    for (int iquery = 0; iquery < query.Count(); iquery++)
                    {
                        ip.lstAllSelectCRFuntion.Add(new AllSelectCRFunction()
                        {
                            NodeType = 1,
                            // EndPointGroupID = lstAvailable.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
                            ID = i,
                            Name = query[iquery],
                            PID = 0,
                            PoolingMethod = "None",


                        });
                        iEndPoint = i;
                        i++;
                        //---------Add Author------------
                        var author = lstAvailable.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint == query[iquery]).Select(a => a.CRSelectFunction.BenMAPHealthImpactFunction.Author).Distinct().ToList();
                        for (int iauthor = 0; iauthor < author.Count; iauthor++)
                        {
                            ip.lstAllSelectCRFuntion.Add(new AllSelectCRFunction()
                            {
                                NodeType = 2,
                                // EndPointGroupID = lstAvailable.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
                                ID = i,
                                Name = author[iauthor],
                                PID = iEndPoint,
                                PoolingMethod = "None",


                            });
                            iAuthor = i;
                            i++;
                            var Qualifier = lstAvailable.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint == query[iquery] && p.CRSelectFunction.BenMAPHealthImpactFunction.Author == author[iauthor]).Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier).Distinct().ToList();
                            for (int iqualifier = 0; iqualifier < Qualifier.Count; iqualifier++)
                            {
                                ip.lstAllSelectCRFuntion.Add(new AllSelectCRFunction()
                                {
                                    NodeType = 3,
                                    // EndPointGroupID = lstAvailable.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
                                    ID = i,
                                    Name = Qualifier[iqualifier],
                                    PID = iAuthor,
                                    PoolingMethod = "None",
                                    //CRSelectFunctionCalculateValue= lstAvailable[ifunction]


                                });
                                iQualifier = i;
                                i++;
                                var function = lstAvailable.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint == query[iquery] && p.CRSelectFunction.BenMAPHealthImpactFunction.Author == author[iauthor] && p.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier == Qualifier[iqualifier]).ToList();
                                for (int ifunction = 0; ifunction < function.Count(); ifunction++)
                                {
                                    ip.lstAllSelectCRFuntion.Add(new AllSelectCRFunction()
                                    {
                                        NodeType = 4,
                                        // EndPointGroupID = lstAvailable.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
                                        ID = i,
                                        Name = function[ifunction].CRSelectFunction.BenMAPHealthImpactFunction.strLocations,
                                        PID = iQualifier,
                                        //PoolingMethod =null,
                                        CRSelectFunctionCalculateValue = function[ifunction]


                                    });

                                    i++;

                                }
                            }

                        }


                    }


                }
                initTreeView(ip);
                //lstSelectCRSelectFunctionCalculateValue = lstSelectCRSelectFunctionCalculateValue.Union(lstAvailable).ToList();
                //this.treeListView.SetObjects(lstSelectCRSelectFunctionCalculateValue);
            }
            catch (Exception ex)
            {

            }

        }

        public void updateDrop(AllSelectCRFunction source, AllSelectCRFunction targe)
        {
            IncidencePoolingAndAggregation ip = CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text).First();

            if (source.NodeType == 4 && targe.NodeType == 3)
            {
                //------把4加到3上面
                var cr3 = ip.lstAllSelectCRFuntion.Where(p => p.ID == source.PID).First();

                source.PID = targe.ID;
                if (ip.lstAllSelectCRFuntion.Where(p => p.PID == cr3.ID).Count() == 0)
                {
                    ip.lstAllSelectCRFuntion.Remove(cr3);

                }
                var cr2 = ip.lstAllSelectCRFuntion.Where(p => p.ID == cr3.PID).First();
                if (ip.lstAllSelectCRFuntion.Where(p => p.PID == cr2.ID).Count() == 0)
                {
                    ip.lstAllSelectCRFuntion.Remove(cr2);

                }
                var cr1 = ip.lstAllSelectCRFuntion.Where(p => p.ID == cr2.PID).First();
                if (ip.lstAllSelectCRFuntion.Where(p => p.PID == cr1.ID).Count() == 0)
                {
                    ip.lstAllSelectCRFuntion.Remove(cr1);

                }
                var cr0 = ip.lstAllSelectCRFuntion.Where(p => p.ID == cr1.PID).First();
                if (ip.lstAllSelectCRFuntion.Where(p => p.PID == cr0.ID).Count() == 0)
                {
                    ip.lstAllSelectCRFuntion.Remove(cr0);

                }

            }
            else if (source.NodeType == 4 && targe.NodeType == 4)
            {

                var cr3 = ip.lstAllSelectCRFuntion.Where(p => p.ID == source.PID).First();

                source.PID = targe.PID;
                if (ip.lstAllSelectCRFuntion.Where(p => p.PID == cr3.ID).Count() == 0)
                {
                    ip.lstAllSelectCRFuntion.Remove(cr3);

                }
                var cr2 = ip.lstAllSelectCRFuntion.Where(p => p.ID == cr3.PID).First();
                if (ip.lstAllSelectCRFuntion.Where(p => p.PID == cr2.ID).Count() == 0)
                {
                    ip.lstAllSelectCRFuntion.Remove(cr2);

                }
                var cr1 = ip.lstAllSelectCRFuntion.Where(p => p.ID == cr2.PID).First();
                if (ip.lstAllSelectCRFuntion.Where(p => p.PID == cr1.ID).Count() == 0)
                {
                    ip.lstAllSelectCRFuntion.Remove(cr1);

                }
                var cr0 = ip.lstAllSelectCRFuntion.Where(p => p.ID == cr1.PID).First();
                if (ip.lstAllSelectCRFuntion.Where(p => p.PID == cr0.ID).Count() == 0)
                {
                    ip.lstAllSelectCRFuntion.Remove(cr0);

                }

            }



            treeListView.RebuildAll(true);
            treeListView.ExpandAll();

        }
        public void btDelSelectMethod_Click(object sender, EventArgs e)
        {
            if (btShowDetail.Text == "Detailed View")
            {
                MessageBox.Show("Please change to detailed view first.");
                return;

            }
            IncidencePoolingAndAggregation ip = CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text).First();
            _operationStatus = 3;//删除
            if (_dicPoolingWindowOperation.ContainsKey(ip.PoolingName))
            {
                _dicPoolingWindowOperation[ip.PoolingName] = 3;
            }
            else
            {
                _dicPoolingWindowOperation.Add(ip.PoolingName, 3);
            }
            //foreach (CRSelectFunctionCalculateValue cr in treeListView.SelectedObjects)
            //{
            //    lstSelectCRSelectFunctionCalculateValue.Remove(cr);

            //}
            //this.treeListView.SetObjects(lstSelectCRSelectFunctionCalculateValue);
            //------------首先可删掉nodetype=4的所有的------------------
            foreach (AllSelectCRFunction cr in treeListView.SelectedObjects)
            {
                if (cr.NodeType == 100)
                {
                    ip.lstAllSelectCRFuntion.Remove(cr);
                    dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text].Remove(cr.CRSelectFunctionCalculateValue);

                }
                //  lstSelectCRSelectFunctionCalculateValue.Remove(cr);

            }
            //----删除掉所有没有叶子结点的------
            List<AllSelectCRFunction> lstRemove = new List<AllSelectCRFunction>();
            foreach (AllSelectCRFunction allSelectCRFunction in ip.lstAllSelectCRFuntion)
            {
                List<AllSelectCRFunction> lstTmp = new List<AllSelectCRFunction>();
                APVX.APVCommonClass.getAllChildCR(allSelectCRFunction, ip.lstAllSelectCRFuntion, ref lstTmp);
                if (lstTmp.Where(p => p.NodeType == 100).Count() == 0)
                    lstRemove.Add(allSelectCRFunction);
                if (lstTmp.Where(p => p.NodeType == 100).Count() == 1)
                {
                    lstRemove.Add(allSelectCRFunction);
                    lstTmp.First().PID = allSelectCRFunction.PID;
                    var query = ip.lstAllSelectCRFuntion.Where(p => p.ID == allSelectCRFunction.PID).ToList();
                    while (query.Count > 0)
                    {
                        APVX.APVCommonClass.getAllChildCR(query.First(), ip.lstAllSelectCRFuntion, ref lstTmp);
                        if (lstTmp.Where(p => p.NodeType == 100).Count() == 1)
                        {
                            lstRemove.Add(query.First());
                            lstTmp.First().PID = query.First().PID;
                        }
                        else
                            break;
                    }
                }

            }
            lstRemove = lstRemove.Where(p => p.NodeType != 100).ToList();
            foreach (AllSelectCRFunction allSelectCRFunction in lstRemove)
            {
                ip.lstAllSelectCRFuntion.Remove(allSelectCRFunction);
            }

            ////------------删掉nodetype=3的，如果该节点下面有则不能删掉---------------------
            //foreach (AllSelectCRFunction cr in treeListView.SelectedObjects)
            //{
            //    if (cr.NodeType == 100)
            //    {
            //        if (ip.lstAllSelectCRFuntion.Where(p => p.PID == cr.ID).Count() == 0)
            //            ip.lstAllSelectCRFuntion.Remove(cr);

            //    }
            //    //  lstSelectCRSelectFunctionCalculateValue.Remove(cr);

            //}
            ////------------删掉nodetype=2的，如果该节点下面有则不能删掉---------------------
            //foreach (AllSelectCRFunction cr in treeListView.SelectedObjects)
            //{
            //    if (cr.NodeType == 2)
            //    {
            //        if (ip.lstAllSelectCRFuntion.Where(p => p.PID == cr.ID).Count() == 0)
            //            ip.lstAllSelectCRFuntion.Remove(cr);

            //    }
            //    //  lstSelectCRSelectFunctionCalculateValue.Remove(cr);

            //}

            ////------------删掉nodetype=1的，如果该节点下面有则不能删掉--------------------
            //foreach (AllSelectCRFunction cr in treeListView.SelectedObjects)
            //{
            //    if (cr.NodeType == 1)
            //    {
            //        if (ip.lstAllSelectCRFuntion.Where(p => p.PID == cr.ID).Count() == 0)
            //            ip.lstAllSelectCRFuntion.Remove(cr);

            //    }
            //    //  lstSelectCRSelectFunctionCalculateValue.Remove(cr);

            //}

            ////------------删掉nodetype=0的，如果该节点下面有则不能删掉--------------------
            //foreach (AllSelectCRFunction cr in treeListView.SelectedObjects)
            //{
            //    if (cr.NodeType == 0)
            //    {
            //        if (ip.lstAllSelectCRFuntion.Where(p => p.PID == cr.ID).Count() == 0)
            //            ip.lstAllSelectCRFuntion.Remove(cr);

            //    }
            //    //  lstSelectCRSelectFunctionCalculateValue.Remove(cr);

            //}
            //-----------清理---
            //var lstTemp = ip.lstAllSelectCRFuntion.Where(p => p.NodeType != 4);
            //List<AllSelectCRFunction> lstRemove = new List<AllSelectCRFunction>();
            //foreach (AllSelectCRFunction ac in lstTemp)
            //{
            //    if (ip.lstAllSelectCRFuntion.Where(p => p.PID == ac.ID).Count() == 0)
            //    {
            //        //ip.lstAllSelectCRFuntion.Remove(ac);
            //        lstRemove.Add(ac);
            //        var query2 = ip.lstAllSelectCRFuntion.Where(p => p.ID == ac.PID).ToList();
            //        foreach (AllSelectCRFunction ac2 in query2)
            //        {
            //            if (ip.lstAllSelectCRFuntion.Where(p => p.PID == ac2.ID && p.ID != ac.ID).Count() == 0)
            //            {
            //                lstRemove.Add(ac2);
            //                var query1 = ip.lstAllSelectCRFuntion.Where(p => p.ID == ac2.PID);

            //                foreach (AllSelectCRFunction ac1 in query1)
            //                {
            //                    if (ip.lstAllSelectCRFuntion.Where(p => p.PID == ac1.ID && p.ID != ac2.ID).Count() == 0)
            //                    {
            //                        lstRemove.Add(ac1);
            //                        var query0 = ip.lstAllSelectCRFuntion.Where(p => p.ID == ac1.PID);
            //                        if (query0.Count() > 0)
            //                        {
            //                            lstRemove.AddRange(query0.ToList());
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //    }
            //}
            //foreach (AllSelectCRFunction acRemove in lstRemove)
            //{
            //    ip.lstAllSelectCRFuntion.Remove(acRemove);
            //}
            initTreeView(ip);
            if (dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text] == null) dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text] = new List<CRSelectFunctionCalculateValue>();
            if (dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text] != null && dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text].Count > 0)
                incidenceBusinessCardRenderer.lstExists = dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text].Select(p => p.CRSelectFunction.CRID).ToList();
            else
                incidenceBusinessCardRenderer.lstExists = new List<int>();
            olvAvailable.Refresh();
            //dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text].AddRange(lstAvailable);
            //foreach (AllSelectCRFunction allSelectCRFunction in lstRemove)
            //{
            //    ip.lstAllSelectCRFuntion.Remove(allSelectCRFunction);
            //}

        }
        private void updateTreeColumns(ref List<string> lstString)
        {
            if (lstString == null)
            {
                lstString = new List<string>();
                for (int i = 2; i < treeListView.Columns.Count; i++)
                {
                    OLVColumn olvc = treeListView.Columns[i] as OLVColumn;
                    //if (olvc.Text.Replace(" ", "").ToLower() == lstString[i])
                    lstString.Add(olvc.Text);
                }
            }
            int j = 0;
            foreach (OLVColumn olvc in treeListView.Columns)
            {
                
                    olvc.DisplayIndex = j;
                j++;
            }
            for (int i = 0; i < lstString.Count(); i++)
            {
                foreach (OLVColumn olvc in treeListView.Columns)
                {
                    if (olvc.Text == lstString[i])
                        olvc.DisplayIndex = i + 2;
                }
            }
        }
        private void btTileSet_Click(object sender, EventArgs e)
        {
            try
            {
                APVX.TileSet tileSet = new APVX.TileSet();
                tileSet.olv = this.olvAvailable;
                DialogResult dr = tileSet.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.Cancel)
                    return;
                int count = 0;
                for (int i = 0; i < this.olvAvailable.AllColumns.Count; i++)
                {
                    if ((this.olvAvailable.AllColumns[i] as BrightIdeasSoftware.OLVColumn).IsTileViewColumn)
                    {
                        count++;
                        (this.olvAvailable.AllColumns[i] as BrightIdeasSoftware.OLVColumn).IsVisible = true;
                    }
                    else
                    {
                        (this.olvAvailable.AllColumns[i] as BrightIdeasSoftware.OLVColumn).IsVisible = false;
                    }
                }
                this.olvAvailable.TileSize = new Size(120, Convert.ToInt32(count * 15.2 + 10));
                this.olvAvailable.RebuildColumns();
                this.olvAvailable.Refresh();
            }
            catch
            { 
            }
        }

        private void btAddPoolingWindow_Click(object sender, EventArgs e)
        {
            int i = tabControlSelected.TabCount;// cbPoolingWindow.Items.Count;
            while (istabControlSelectedContainText("PoolingWindow" + i))
            {
                i++;
            }
            tabControlSelected.TabPages.Add("PoolingWindow" + i, "PoolingWindow" + i);
            IncidencePoolingAndAggregation ip = new IncidencePoolingAndAggregation() { PoolingName = "PoolingWindow" + i };
            CommonClass.lstIncidencePoolingAndAggregation.Add(ip);
            if (CommonClass.ValuationMethodPoolingAndAggregation != null && CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase != null)
            {
                try
                {
                    CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Add(new ValuationMethodPoolingAndAggregationBase() { IncidencePoolingAndAggregation = CommonClass.lstIncidencePoolingAndAggregation.Last(), LstAllSelectValuationMethod = new List<AllSelectValuationMethod>() });

                }
                catch
                {
                }
            }
            _operationStatus = 1;
            if (_dicPoolingWindowOperation.ContainsKey(ip.PoolingName))
            {
                _dicPoolingWindowOperation[ip.PoolingName] = 1;
            }
            else
            {
                _dicPoolingWindowOperation.Add(ip.PoolingName, 1);
            }
            //tabControlSelected.SelectedIndex = 0;
            tabControlSelected.SelectedIndex = tabControlSelected.TabCount - 1;

        }

        private bool _hasDelPoolingWindows = false;// 是否做了删除Pooling Windows的操作，如果删除了pooling windows-》true，否则-》false
        private void btDelPoolingWindow_Click(object sender, EventArgs e)
        {
            if (tabControlSelected.TabCount == 1)
            {
                MessageBox.Show("You can not delete the last pooling window.");
                return;
            }
            //----------去掉错误Valuation------------

            if (CommonClass.ValuationMethodPoolingAndAggregation != null && CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase != null)
            {
                try
                {
                    CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Remove(CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text).First());

                }
                catch
                {
                }

            }
            if (dicTabCR.ContainsKey(tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text))
            {
                dicTabCR.Remove(tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text);
            }
            CommonClass.lstIncidencePoolingAndAggregation.Remove(CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text).First());

            _operationStatus = 3;

            //tabControlSelected.TabPages[0].Controls.Add(treeListView);
            //this.cbPoolingWindow_SelectedIndexOld = -1;
            tabControlSelected.TabPages.RemoveAt(tabControlSelected.SelectedIndex);
            _hasDelPoolingWindows = true;
            if (tabControlSelected.TabPages.Count > 0)
            {
                tabControlSelected.SelectedIndex = 0;

            }
            else
                tabControlSelected.SelectedIndex = -1;
        }

        //private void cbPoolingWindow_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        try
        //        {
        //            //_pageCurrent = Convert.ToInt32(bindingNavigatorPositionItem.Text);
        //            //_currentRow = _pageSize * (_pageCurrent - 1);
        //            //UpdateTableResult(_tableObject);
        //            if (cbPoolingWindow.SelectedIndex != -1 && !cbPoolingWindow.Items.Contains(cbPoolingWindow.Text))
        //            {
        //                CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == cbPoolingWindow.Items[cbPoolingWindow.SelectedIndex].ToString()).First().PoolingName = cbPoolingWindow.Text;
        //                cbPoolingWindow.Items[cbPoolingWindow.SelectedIndex] = cbPoolingWindow.Text;
        //            }
        //            else
        //            {
        //                MessageBox.Show("The pooling window name is existed");
        //            }
        //        }
        //        catch
        //        {

        //        }
        //    }
        //}
        //private int cbPoolingWindow_SelectedIndexOld = -1;
        private void cbPoolingWindow_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



        private void btnSTileSet_Click(object sender, EventArgs e)
        {
            APVX.TileSet tileSet = new APVX.TileSet();
            tileSet.olv = this.treeListView;
            DialogResult dr = tileSet.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.Cancel)
                return;
            int count = 0;
            for (int i = 0; i < this.treeListView.Columns.Count; i++)
            {
                if ((this.treeListView.Columns[i] as BrightIdeasSoftware.OLVColumn).IsTileViewColumn)
                    count++;
            }
            this.treeListView.TileSize = new Size(300, Convert.ToInt32(count * 14.3 + 47));
            this.treeListView.Refresh();
        }

        private void tabControlSelected_SelectedIndexChanged(object sender, EventArgs e)
        {
            //--------------判断Pooling-->
            try
            {
                if (tabControlSelected.SelectedIndex == -1) return;
                IncidencePoolingAndAggregation ip = CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text).First();
                //if (ip.lstAllSelectCRFuntion == null || ip.lstAllSelectCRFuntion.Count == 0) btShowDetail.Text = "Show Brief";
                //else
                    //btShowDetail.Text = "Show Detail";
                if (tabControlSelected.SelectedIndex > -1)
                {
                    tbPoolingName.Text = tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text;
                    updateTreeColumns(ref ip.lstColumns);
                    //cbPoolingWindow_SelectedIndexOld = tabControlSelected.SelectedIndex;
                    initTreeView(ip);
                    treeListView.Dock = DockStyle.Fill;
                    treeListView.TabIndex = tabControlSelected.SelectedIndex;
                    treeListView.Refresh();
                    tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Controls.Add(treeListView);
                    //cboPoolingMethod.SelectedItem = Enum.GetName(typeof(PoolingMethodTypeEnum), ip.PoolingMethodType);


                }
                treeListView.Visible = true;
                //-----------majie-------
                if (tabControlSelected.TabCount == 0)
                { lblPoolingWinNum.Text = "1"; }
                else
                {
                    lblPoolingWinNum.Text = tabControlSelected.TabCount.ToString();
                }

                //----
                if (!dicTabCR.ContainsKey(tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text))
                {
                    dicTabCR.Add(tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text, new List<CRSelectFunctionCalculateValue>());


                    //-----------------------To-----------------
                    if (dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text] == null) dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text] = new List<CRSelectFunctionCalculateValue>();
                    if (ip.lstAllSelectCRFuntion != null && ip.lstAllSelectCRFuntion.Count > 0)
                    {
                        dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text] = ip.lstAllSelectCRFuntion.Where(p => p.NodeType == 100 && p.CRSelectFunctionCalculateValue != null && p.CRSelectFunctionCalculateValue.CRSelectFunction != null).Select(p => p.CRSelectFunctionCalculateValue).ToList();
                    }
                    else
                    {
                        dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text] = new List<CRSelectFunctionCalculateValue>();

                    }
                }
                if (dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text] != null && dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text].Count > 0)
                    incidenceBusinessCardRenderer.lstExists = dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text].Select(p => p.CRSelectFunction.CRID).ToList();
                else
                    incidenceBusinessCardRenderer.lstExists = new List<int>();
                olvAvailable.Refresh();
                //------------end---------

            }
            catch
            {
                treeListView.Objects = null;

            }

        }

        private void tbPoolingName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btChangeName_Click(sender, e);
            }

        }

        private bool istabControlSelectedContainText(string text)
        {
            bool btp = false;
            foreach (TabPage tp in tabControlSelected.TabPages)
            {
                if (tp.Text == text) btp = true;
            }
            return btp;

        }
        private void btChangeName_Click(object sender, EventArgs e)
        {
            try
            {
                IncidencePoolingAndAggregation ip = CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text).First();
                if (ip.PoolingName == tbPoolingName.Text)
                    return;
                if (tbPoolingName.Text == "")
                {
                    tbPoolingName.Text = ip.PoolingName;
                    return;
                }
                if (!istabControlSelectedContainText(tbPoolingName.Text) && tbPoolingName.Text != "")
                {
                    if (dicTabCR.ContainsKey(ip.PoolingName))
                    {
                        dicTabCR.Keys.ToList()[dicTabCR.Keys.ToList().IndexOf(ip.PoolingName)] = tbPoolingName.Text;
                    }
                    if (_dicPoolingWindowOperation.ContainsKey(ip.PoolingName))
                    {
                        _dicPoolingWindowOperation.Add(tbPoolingName.Text, _dicPoolingWindowOperation[ip.PoolingName]);
                        _dicPoolingWindowOperation.Remove(ip.PoolingName);
                    }
                    
                    ip.PoolingName = tbPoolingName.Text;
                    _operationStatus = 2;
                    
                    tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text = tbPoolingName.Text;


                }
                else
                {
                    tbPoolingName.Text = ip.PoolingName;
                    MessageBox.Show("This pooling window name is already defined. Please use a different name.");
                }
            }
            catch
            { }
        }

        private void treeListView_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            try
            {
                if (e.NewDisplayIndex == 0 || e.NewDisplayIndex == 1 || e.OldDisplayIndex == 0 || e.OldDisplayIndex == 1)//|| e.OldDisplayIndex == treeListView.Columns.Count - 1 || e.NewDisplayIndex == treeListView.Columns.Count - 1)
                {
                    e.Cancel = true;
                    return;
                }
                //Todo:陈志润20120115 提示
                _operationStatus = 1;

                //------------------根据顺序生成一棵树------------------------
                OLVColumn olvc = treeListView.Columns[e.OldDisplayIndex] as OLVColumn;
                //treeListView.Columns.Remove(olvc);
                //treeListView.Columns.Insert(e.NewDisplayIndex, olvc);
                List<BrightIdeasSoftware.OLVColumn> lstOLVColumns = new List<OLVColumn>();
                foreach (BrightIdeasSoftware.OLVColumn olvc2 in this.treeListView.Columns)
                {
                    lstOLVColumns.Add(olvc2);
                }
                olvc = lstOLVColumns.Where(p => p.DisplayIndex == e.OldDisplayIndex).First();
                lstOLVColumns = lstOLVColumns.OrderBy(p => p.DisplayIndex).ToList();
                lstOLVColumns.Remove(olvc);
                lstOLVColumns.Insert(e.NewDisplayIndex, olvc);
                lstOLVColumns = lstOLVColumns.Where(p => p.DisplayIndex != 0 && p.DisplayIndex != 1).ToList();

                IncidencePoolingAndAggregation ip = CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text).First();
                ip.lstColumns = lstOLVColumns.Select(p => p.Text).ToList();
                if (_dicPoolingWindowOperation.ContainsKey(ip.PoolingName))
                {
                    _dicPoolingWindowOperation[ip.PoolingName] = 1;
                }
                else
                {
                    _dicPoolingWindowOperation.Add(ip.PoolingName, 1);
                }
                //(treeListView.Columns[e.NewDisplayIndex] as OLVColumn).DisplayIndex = e.OldDisplayIndex;
                //(treeListView.Columns[e.OldDisplayIndex] as OLVColumn).DisplayIndex = e.NewDisplayIndex;
               // List<AllSelectCRFunction> lstAllSelectCRFunction = getLstAllSelectCRFunction(dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text], lstOLVColumns.Select(p => p.Text).ToList(), dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text].First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup,-1);
                List<AllSelectCRFunction> lstAllSelectCRFunction = new List<AllSelectCRFunction>();// getLstAllSelectCRFunction(dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text], lstOLVColumns.Select(p => p.Text).ToList(), dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text].First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup, -1);
                //------首先对所有的CR分组--然后各自计算不同的EndPointGroup的list--然后把他们拼在一块
                Dictionary<string, List<CRSelectFunctionCalculateValue>> dicEndPointGroupCR = new Dictionary<string, List<CRSelectFunctionCalculateValue>>();
                foreach (CRSelectFunctionCalculateValue cr in dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text])
                {
                    try
                    {
                        if (dicEndPointGroupCR.ContainsKey(cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup))
                            dicEndPointGroupCR[cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup].Add(cr);
                        else
                        {
                            dicEndPointGroupCR.Add(cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup, new List<CRSelectFunctionCalculateValue>());
                            dicEndPointGroupCR[cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup].Add(cr);
                        }
                    }
                    catch
                    { }
                }
                foreach (KeyValuePair<string, List<CRSelectFunctionCalculateValue>> k in dicEndPointGroupCR)
                {
                    List<AllSelectCRFunction> lstTemp = getLstAllSelectCRFunction(k.Value, lstOLVColumns.Select(p => p.Text).ToList(), k.Key, -1);
                    //------把所有id增加lstAllSelectCRFunction.count;
                    if (lstAllSelectCRFunction.Count() > 0)
                    {
                        for (int iTemp = 0; iTemp < lstTemp.Count; iTemp++)
                        {
                            lstTemp[iTemp].ID = lstTemp[iTemp].ID + lstAllSelectCRFunction.Max(p => p.ID) + 1;
                            if (lstTemp[iTemp].PID != -1)
                                lstTemp[iTemp].PID = lstTemp[iTemp].PID + lstAllSelectCRFunction.Max(p => p.ID) + 1;
                        }
                    }
                    if (lstTemp != null && lstTemp.Count > 0) lstAllSelectCRFunction.AddRange(lstTemp);
                }
                ip.lstAllSelectCRFuntion = lstAllSelectCRFunction;
                //cbPoolingWindow_SelectedIndexOld = tabControlSelected.SelectedIndex;
                initTreeView(ip);
            }
            catch (Exception ex)
            {
            }

            //-----------------pooling-------清掉!-------------------------
        }
        private string getNameFromOLVColumn(string column, CRSelectFunctionCalculateValue cr)
        {
            string sReturn = "";
            switch (column)
            {
                case "endpoint":
                    sReturn = cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint;
                    break;
                case "author":
                    sReturn = cr.CRSelectFunction.BenMAPHealthImpactFunction.Author;
                    break;
                case "qualifier":
                    sReturn = cr.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier;
                    break;
                case "location":
                    sReturn = cr.CRSelectFunction.BenMAPHealthImpactFunction.strLocations;
                    break;
                case "startage":
                    sReturn = cr.CRSelectFunction.StartAge.ToString();
                    break;
                case "endage":
                    sReturn = cr.CRSelectFunction.EndAge.ToString();
                    break;
                case "year":
                    sReturn = cr.CRSelectFunction.BenMAPHealthImpactFunction.Year.ToString();
                    break;
                case "otherpollutants":
                    sReturn = cr.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants;
                    break;
                case "race":
                    sReturn = cr.CRSelectFunction.Race;
                    break;
                case "ethnicity":
                    sReturn = cr.CRSelectFunction.Ethnicity;
                    break;
                case "gender":
                    sReturn = cr.CRSelectFunction.Gender;
                    break;
                case "function":
                    sReturn = cr.CRSelectFunction.BenMAPHealthImpactFunction.Function;
                    break;
                case "pollutant":
                    sReturn = cr.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName;
                    break;
                case "metric":
                    sReturn = cr.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName;
                    break;
                case "seasonalmetric":
                    sReturn = cr.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName;
                    break;
                case "metricstatistic":
                    sReturn = Enum.GetName(typeof(MetricStatic), cr.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic);
                    break;
                case "dataset":
                    sReturn = cr.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName;
                    break;

            }
            return sReturn;
        }
        public static void getSecond(ref List<CRSelectFunctionCalculateValue> lstSecond, List<string> lstOLVColumns, int i, List<string> lstParent)
        {
            //------------查出有两个则往下加!-------此处需用到递归-------------
            for (int k = 0; k < i; k++)
            {
                switch (lstOLVColumns[k].Replace(" ", "").ToLower())
                {
                    case "endpoint":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint == lstParent[i - k - 1]).ToList();
                        break;

                    case "author":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Author == lstParent[i - k - 1]).ToList();
                        break;
                    case "qualifier":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier == lstParent[i - k - 1]).ToList();
                        break;
                    case "location":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.strLocations == lstParent[i - k - 1]).ToList();
                        break;
                    case "startage":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.StartAge.ToString() == lstParent[i - k - 1]).ToList();
                        break;
                    case "endage":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.EndAge.ToString() == lstParent[i - k - 1]).ToList();
                        break;
                    case "year":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Year.ToString() == lstParent[i - k - 1]).ToList();
                        break;
                    case "otherpollutants":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants == lstParent[i - k - 1]).ToList();
                        break;
                    case "race":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.Race == lstParent[i - k - 1]).ToList();
                        break;
                    case "ethnicity":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.Ethnicity == lstParent[i - k - 1]).ToList();
                        break;
                    case "gender":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.Gender == lstParent[i - k - 1]).ToList();
                        break;
                    case "function":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Function == lstParent[i - k - 1]).ToList();
                        break;
                    case "pollutant":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName == lstParent[i - k - 1]).ToList();
                        break;
                    case "metric":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName == lstParent[i - k - 1]).ToList();
                        break;
                    case "seasonalmetric":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric == null ? "" == lstParent[i - k - 1] : p.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName == lstParent[i - k - 1]).ToList();
                        break;
                    case "metricstatistic":
                        lstSecond = lstSecond.Where(p => Enum.GetName(typeof(MetricStatic), p.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic) == lstParent[i - k - 1]).ToList();
                        break;
                    case "dataset":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName == lstParent[i - k - 1]).ToList();
                        break;
                    case "version":
                        //int iversion = lstCR.GroupBy(p => p.CRSelectFunction.CRID).Max(p => p.Count());
                        //lstString = new List<string>();
                        //for (int i = 1; i <= iversion; i++)
                        //{
                        //    lstString.Add(i.ToString());
                        //}
                        //--------直接--------------
                        var query = lstSecond.GroupBy(p => p.CRSelectFunction.CRID);
                        List<CRSelectFunctionCalculateValue> lstVersion = new List<CRSelectFunctionCalculateValue>();
                        foreach (IGrouping<int, CRSelectFunctionCalculateValue> ig in query)
                        {
                            int iCount = Convert.ToInt32(lstParent[i - k - 1]);
                            if (ig.Count() >= iCount)
                            {
                                lstVersion.Add(ig.ElementAt(iCount - 1));

                            }
                        }
                        lstSecond = lstVersion;
                        break;
                }
            }
        }
        public static List<string> getLstStringFromColumnName(string columName, List<CRSelectFunctionCalculateValue> lstCR)
        {
            List<string> lstString = new List<string>();
            switch (columName)
            {
                case "endpoint":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint).Distinct().ToList();
                    break;
                case "author":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Author).Distinct().ToList();
                    break;
                case "qualifier":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier).Distinct().ToList();
                    break;
                case "location":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.strLocations).Distinct().ToList();
                    break;
                case "startage":
                    lstString = lstCR.Select(p => p.CRSelectFunction.StartAge.ToString()).Distinct().ToList();
                    break;
                case "endage":
                    lstString = lstCR.Select(p => p.CRSelectFunction.EndAge.ToString()).Distinct().ToList();
                    break;
                case "year":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Year.ToString()).Distinct().ToList();
                    break;
                case "otherpollutants":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants).Distinct().ToList();
                    break;
                case "race":
                    lstString = lstCR.Select(p => p.CRSelectFunction.Race).Distinct().ToList();
                    break;
                case "ethnicity":
                    lstString = lstCR.Select(p => p.CRSelectFunction.Ethnicity).Distinct().ToList();
                    break;
                case "gender":
                    lstString = lstCR.Select(p => p.CRSelectFunction.Gender).Distinct().ToList();
                    break;
                case "function":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Function).Distinct().ToList();
                    break;
                case "pollutant":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName).Distinct().ToList();
                    break;
                case "metric":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName).Distinct().ToList();
                    break;
                case "seasonalmetric":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric == null ? "" : p.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName).Distinct().ToList();
                    break;
                case "metricstatistic":
                    lstString = lstCR.Select(p => Enum.GetName(typeof(MetricStatic), p.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic)).Distinct().ToList();
                    break;
                case "dataset":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName).Distinct().ToList();
                    break;
                case "version":
                    int iversion = lstCR.GroupBy(p => p.CRSelectFunction.CRID).Max(p => p.Count());
                    lstString = new List<string>();
                    for (int i = 1; i <= iversion; i++)
                    {
                        lstString.Add(i.ToString());
                    }
                    break;
            }

            return lstString;
        }
        public static List<AllSelectCRFunction> getLstAllSelectCRFunction(List<CRSelectFunctionCalculateValue> lstCR, List<string> lstOLVColumns, string EndPointGroup,int iMaxNodeType)
        {
            try
            {
                List<AllSelectCRFunction> lstReturn = new List<AllSelectCRFunction>();
                if (lstCR == null) return null;
                //---------从第一种情况往下循环，如果只有一个则break!
                if (lstCR.Count == 1)
                {
                    lstReturn.Add(new AllSelectCRFunction()
                    {
                        CRID = lstCR.First().CRSelectFunction.CRID,
                        CRSelectFunctionCalculateValue = lstCR.First(),
                        EndPointGroupID = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
                        EndPointID = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointID,
                        ID = 0,
                        Name = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup,
                        NodeType = 100,
                        PID = -1,
                        PoolingMethod = "",
                        Version = "1",

                        //------add for display
                        EndPointGroup = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup,
                        EndPoint = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPoint,
                        Author = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.Author,
                        Qualifier = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.Qualifier,
                        Location = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.strLocations,
                        StartAge = lstCR.First().CRSelectFunction.StartAge.ToString(),
                        EndAge = lstCR.First().CRSelectFunction.EndAge.ToString(),
                        Year = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.Year.ToString(),
                        OtherPollutants = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants,
                        Race = lstCR.First().CRSelectFunction.Race,
                        Ethnicity = lstCR.First().CRSelectFunction.Ethnicity,
                        Gender = lstCR.First().CRSelectFunction.Gender,
                        Function = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.Function,
                        Pollutant = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.Pollutant == null ? "" : lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName.ToString(),
                        Metric = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.Metric == null ? "" : lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName,
                        SeasonalMetric = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric == null ? "" : lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName,
                        MetricStatistic = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic == null ? "" : Enum.GetName(typeof(MetricStatic), lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic),
                        DataSet = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.DataSetName,
                        //----- add for display


                    });
                    return lstReturn;
                }
                else
                {
                    //-----------首先建立一个EndPointGroup的节点
                    lstReturn.Add(new AllSelectCRFunction()
                    {

                        EndPointGroupID = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,

                        ID = 0,
                        Name = EndPointGroup,
                        EndPointGroup = EndPointGroup,
                        //Name = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup,
                        //EndPointGroup = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup,
                        NodeType = 0,
                        PID = -1,
                        PoolingMethod = "None",
                         Version="",

                    });

                    //-------------根据排序组合树！---------------------
                    List<string> lstColumns = new List<string>();
                    //List<BrightIdeasSoftware.OLVColumn> lstOLVColumns = new List<OLVColumn>();
                    //foreach (BrightIdeasSoftware.OLVColumn olvc in this.treeListView.Columns)
                    //{
                    //    lstOLVColumns.Add(olvc);
                    //}
                    //lstOLVColumns = lstOLVColumns.Where(p => p.DisplayIndex != 0 && p.DisplayIndex != 1 && p.DisplayIndex != lstOLVColumns.Count - 1).OrderBy(p => p.DisplayIndex).ToList();

                    for (int i = 0; i < lstOLVColumns.Count; i++)
                    {

                        //BrightIdeasSoftware.OLVColumn olvc = lstOLVColumns[i];
                        List<string> lstString = new List<string>();
                        int iParent = 0;

                        //------直接加在EndPointGroup下面------如果不为0--加在0下面------首先判断是否有两个不同的



                        lstString = getLstStringFromColumnName(lstOLVColumns[i].Replace(" ", "").ToLower(), lstCR);//lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint).Distinct().ToList();
                        if (lstString.Count() > 0)
                        {
                            if (i == 0) //------直接加在EndPointGroup下面------如果不为0--加在0下面------首先判断是否有两个不同的
                            {
                                iParent = 0;


                                for (int j = 0; j < lstString.Count(); j++)
                                {
                                    lstReturn.Add(new AllSelectCRFunction()
                                    {
                                        EndPointGroupID = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
                                        EndPointGroup = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup,
                                        ID = j + 1,
                                        Name = lstString[j],//----------endpoint---
                                        NodeType = i + 1,
                                        PID = 0,
                                        PoolingMethod = "None",
                                        Version = "",

                                    });
                                    //------------modify version----------
                                    if (lstOLVColumns[i].Replace(" ", "").ToLower() == "version")
                                    {
                                        lstReturn.Last().Version = lstReturn.Last().Name;
                                    }
                                }
                            }
                            else
                            {
                                //-------得根据上一层---如果有则轮循下一层
                                List<AllSelectCRFunction> query = lstReturn.Where(p => p.NodeType == i).ToList();
                                if (query.Count() == 0) { i = lstOLVColumns.Count - 1; }
                                else
                                {
                                    for (int j = 0; j < query.Count(); j++)
                                    {
                                        //------首先求出它所有的Parent的string---
                                        List<string> lstParent = new List<string>();
                                        lstParent.Add(query[j].Name);
                                        var parent = lstReturn.Where(p => p.ID == query[j].PID).First();
                                        lstParent.Add(parent.Name);
                                        for (int k = i; k > 1; k--)
                                        {
                                            parent = lstReturn.Where(p => p.ID == parent.PID).First();
                                            lstParent.Add(parent.Name);

                                        }
                                        List<CRSelectFunctionCalculateValue> lstSecond = lstCR;
                                        getSecond(ref lstSecond, lstOLVColumns, i, lstParent);
                                        if (lstSecond.Count() > 1)
                                        {
                                            //----------add-------------
                                            lstString = getLstStringFromColumnName(lstOLVColumns[i].Replace(" ", "").ToLower(), lstSecond);// lstSecond.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint).Distinct().ToList();
                                            if (lstString.Count > 0)//modify by xiejp 20121115 解决一直往下的问题
                                            {
                                                for (int k = 0; k < lstString.Count(); k++)
                                                {
                                                    lstReturn.Add(new AllSelectCRFunction()
                                                    {
                                                        EndPointGroupID = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
                                                        EndPointGroup = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup,
                                                        ID = lstReturn.Count(),
                                                        Name = lstString[k],//----------endpoint---
                                                        NodeType = i + 1,
                                                        PID = query[j].ID,
                                                        PoolingMethod = "None",
                                                        Version = "",

                                                    });
                                                    //------------modify version----------
                                                    if (lstOLVColumns[i].Replace(" ", "").ToLower() == "version")
                                                    {
                                                        lstReturn.Last().Version = lstReturn.Last().Name;
                                                    }
                                                }
                                            }
                                            else
                                            {
 
                                            }
                                        }
                                    }
                                }
                            }


                        }
                        else
                        {
                            //退出循环！-------------加子节点------------
                            //--------------如果==1-------------
                            i = lstOLVColumns.Count - 1;
                        }



                    }
                    //---------------------根据生成的一棵树来生成整个的树---首先求出没有叶子结点的所有节点列表，然后轮循!----------
                    var queryTree = lstReturn.Where(p => lstReturn.Where(a => a.PID == p.ID).Count() == 0).ToList();
                    foreach (AllSelectCRFunction acf in queryTree)
                    {
                        List<string> lstParent = new List<string>();
                        lstParent.Add(acf.Name);
                        if (lstReturn.Where(p => p.ID == acf.PID).Count() > 0)
                        {
                            var parent = lstReturn.Where(p => p.ID == acf.PID).First();
                            lstParent.Add(parent.Name);
                            for (int k = acf.NodeType; k > 1; k--)
                            {
                                parent = lstReturn.Where(p => p.ID == parent.PID).First();
                                lstParent.Add(parent.Name);

                            }
                        }
                        List<CRSelectFunctionCalculateValue> lstSecond = lstCR;
                        getSecond(ref lstSecond, lstOLVColumns, acf.NodeType, lstParent);
                        int iVersion = 1;
                        int iVersionNodeType = 0;
                        for (int io = 0; io < lstOLVColumns.Count(); io++)
                        {
                            if (lstOLVColumns[io].ToLower() == "version")
                            {
                                iVersionNodeType = io + 1;

                            }
                        }
                        foreach (CRSelectFunctionCalculateValue crc in lstSecond)
                        {
                            iVersion = lstReturn.Where(p => p.CRID == crc.CRSelectFunction.CRID).Count() + 1;
                            //if (lstOLVColumns[lstOLVColumns.Count - 1].ToLower() == "version")
                            //    //if (lstOLVColumns[0].ToLower() != "version")
                            //    iVersion = lstReturn.Where(p => p.CRID == crc.CRSelectFunction.CRID).Count() + 1;
                            //else
                            //{
                            //    //--------直接得到
                            //    if (acf.NodeType == iVersionNodeType)
                            //        iVersion = Convert.ToInt32(acf.Name);
                            //    else
                            //    {
                            //        List<AllSelectCRFunction> versionParent = lstReturn.Where(p => p.ID == acf.PID).ToList();
                            //        while (versionParent!=null && versionParent.Count() > 0)
                            //        {
                            //            if (versionParent.First().NodeType == iVersionNodeType)
                            //            {
                            //                iVersion = Convert.ToInt32(versionParent.First().Name);
                            //            }
                            //            versionParent = lstReturn.Where(p => p.ID == versionParent.First().PID).ToList();
                            //        }
                            //    }

                            //}
                            string strAuthor=crc.CRSelectFunction.BenMAPHealthImpactFunction.Author;
                            if(strAuthor!=null && strAuthor!="" && strAuthor.Contains(" "))
                            {
                                strAuthor= strAuthor.Substring(0,strAuthor.IndexOf(" "));
                            }
                            lstReturn.Add(new AllSelectCRFunction()
                            {
                                CRID = crc.CRSelectFunction.CRID,
                                CRSelectFunctionCalculateValue = crc,
                                EndPointGroupID = crc.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
                                EndPointID = crc.CRSelectFunction.BenMAPHealthImpactFunction.EndPointID,
                                ID = lstReturn.Count(),
                                Name = strAuthor,// crc.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup,
                                NodeType = 100,
                                PID = acf.ID,
                                PoolingMethod = "",
                                Version = iVersion.ToString(),//------------------需要修改!------------------------

                                //------add for display
                                EndPointGroup = crc.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup,
                                EndPoint = crc.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint,
                                Author = crc.CRSelectFunction.BenMAPHealthImpactFunction.Author,
                                Qualifier = crc.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier,
                                Location = crc.CRSelectFunction.BenMAPHealthImpactFunction.strLocations,
                                StartAge = crc.CRSelectFunction.StartAge.ToString(),
                                EndAge = crc.CRSelectFunction.EndAge.ToString(),
                                Year = crc.CRSelectFunction.BenMAPHealthImpactFunction.Year.ToString(),
                                OtherPollutants = crc.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants,
                                Race = crc.CRSelectFunction.Race,
                                Ethnicity = crc.CRSelectFunction.Ethnicity,
                                Gender = crc.CRSelectFunction.Gender,
                                Function = crc.CRSelectFunction.BenMAPHealthImpactFunction.Function,
                                Pollutant = crc.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName,
                                Metric = crc.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName,
                                SeasonalMetric = crc.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric == null ? "" : crc.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName,
                                MetricStatistic = Enum.GetName(typeof(MetricStatic), crc.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic),
                                DataSet = crc.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName,
                                //----- add for display
                            });
                        }

                    }
                    //----------------------modify by xiejp--修正树，如果叶子结点只有一个的根节点，去掉---
                    if (lstReturn.Count > 0)
                    {
                        int iMaxLstReturnNodeType =iMaxNodeType!=-1?iMaxNodeType: lstReturn.Where(p => p.NodeType != 100).Max(p => p.NodeType);
                        //---------------------modify by xiejp--如果version已作为树的根节点---则要重新整理树----------
                        if (lstOLVColumns.GetRange(0, iMaxNodeType != -1 ? iMaxNodeType : lstReturn.Where(p => p.NodeType != 100).Max(p => p.NodeType)).Contains("Version"))
                        {
                            var lstTemp = lstReturn.Where(p => p.NodeType == lstOLVColumns.IndexOf("Version") + 1).ToList();
                            //---得到它的所有叶子结点，把叶子结点的version 修改成它!
                            for (int ilstTemp = 0; ilstTemp < lstTemp.Count; ilstTemp++)
                            {
                                List<AllSelectCRFunction> lstTempReturn = new List<AllSelectCRFunction>();
                                APVX.APVCommonClass.getAllChildCR(lstTemp[ilstTemp], lstReturn, ref lstTempReturn);
                                //lstTempReturn = lstTempReturn.Where(p => p.CRSelectFunctionCalculateValue != null).ToList();
                                foreach (AllSelectCRFunction alsc in lstTempReturn)
                                {
                                    if(alsc.Version=="")
                                    alsc.Version = lstTemp[ilstTemp].Name;
                                }
                            }

                        }
                        for (int iMax = iMaxLstReturnNodeType; iMax > 0; iMax--)
                        {
                            var lstTemp = lstReturn.Where(p => p.NodeType == iMax).ToList();
                            foreach (AllSelectCRFunction alcr in lstTemp)
                            {
                                var lstTempSec = lstReturn.Where(p => p.PID == alcr.ID).ToList();
                                var lstTempSec2 = lstReturn.Where(p => p.PID == alcr.PID).ToList();
                                if (lstTempSec2.Count == 1 || (lstTempSec.Count==1 && lstTempSec[0].NodeType==100))
                                {

                                    foreach (AllSelectCRFunction alcrSec in lstTempSec)
                                    {
                                        alcrSec.PID = alcr.PID;

                                    }
                                    lstReturn.Remove(alcr);
                                }
                            }
                        }
                        lstReturn.Where(p => p.NodeType != 100).Last().NodeType = iMaxLstReturnNodeType ;
                        //for (int iTemp = 0; iTemp < iMaxLstReturnNodeType; iTemp++)
                        //{
                        //    for (int iMax = iMaxLstReturnNodeType; iMax > 0; iMax--)
                        //    {
                        //        var lstTemp = lstReturn.Where(p => p.NodeType == iMax).ToList();
                        //        foreach (AllSelectCRFunction alcr in lstTemp)
                        //        {
                        //            var lstTempSec = lstReturn.Where(p => p.PID == alcr.ID).ToList();

                        //            if (lstTempSec.Count == 1)// &&lstTempSec[0].NodeType==100)//modify by xiejp 20121115解决一直往下的问题
                        //            {

                        //                foreach (AllSelectCRFunction alcrSec in lstTempSec)
                        //                {
                        //                    alcrSec.PID = alcr.PID;
                        //                    if (alcrSec.PID == -1) alcrSec.PID = 0;

                        //                }
                        //                try
                        //                {
                        //                    lstReturn.Where(p => p.ID == alcr.PID).First().NodeType = alcr.NodeType;
                        //                    //lstReturn.Where(p => p.ID == alcr.PID).First().Name += " " + alcr.Name;
                        //                    lstReturn.Remove(alcr);
                        //                }
                        //                catch
                        //                { 
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                    }



                }
                //-----------------让StartAge,EndAge真实的表达所选择的年龄段。StartAge,Min,EndAge,Max
                foreach (AllSelectCRFunction acr in lstReturn)
                {
                    if (acr.PoolingMethod != "")
                    {
                        //--------------得到所有NoOne---------
                        List<AllSelectCRFunction> lst = new List<AllSelectCRFunction>();
                        getAllChildFromAllSelectCRFunction(acr, lstReturn, ref lst);
                        if (lst.Count > 0)
                        {
                            if (acr.CRSelectFunctionCalculateValue == null ) acr.CRSelectFunctionCalculateValue = new CRSelectFunctionCalculateValue()
                            {
                                CRSelectFunction = new CRSelectFunction()
                                {

                                    StartAge = Convert.ToInt32(lst.Min(p => p.StartAge)),
                                    EndAge = Convert.ToInt32(lst.Max(p => p.EndAge)),
                                }
                            };
                            if (acr.CRSelectFunctionCalculateValue.CRSelectFunction == null)
                            {
                                acr.CRSelectFunctionCalculateValue.CRSelectFunction = new CRSelectFunction();
                                if (acr.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction == null)
                                {
                                    acr.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction = new BenMAPHealthImpactFunction();
                                }
                            }
                            acr.CRSelectFunctionCalculateValue.CRSelectFunction.StartAge = Convert.ToInt32(lst.Min(p => p.StartAge));
                            acr.CRSelectFunctionCalculateValue.CRSelectFunction.EndAge = Convert.ToInt32(lst.Max(p => p.EndAge));
                            acr.StartAge = lst.Min(p => p.StartAge);
                            acr.EndAge = lst.Max(p => p.EndAge);
                            List<string> lstTemp = lst.Select(p => p.Pollutant).Distinct().ToList();
                            acr.Pollutant = "";
                            foreach (string s in lstTemp)
                            {
                                acr.Pollutant += s+" ";
                            }
                            
                            //lstTemp = lst.Select(p => p.Author).Distinct().ToList();
                            //foreach (string s in lstTemp)
                            //{
                            //    acr.Author += s + " ";
                            //}
                            
                            //lstTemp = lst.Select(p => p.EndPoint).Distinct().ToList();
                            //foreach (string s in lstTemp)
                            //{
                            //    acr.EndPoint += s + " ";
                            //}
                            acr.Author = "";
                            acr.EndPoint = "";
                            List<string> lstAuthor = new List<string>();
                            List<string> lstEndPoint = new List<string>();
                            
                            foreach (AllSelectCRFunction alcr in lst)
                            {
                                if (alcr.CRSelectFunctionCalculateValue == null || alcr.CRSelectFunctionCalculateValue.CRSelectFunction == null || alcr.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction == null) continue;
                                if (alcr.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author != "" && !lstAuthor.Contains(alcr.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author))
                                {
                                    lstAuthor.Add(alcr.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author);
                                }
                                if (alcr.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint != "" && !lstEndPoint.Contains(alcr.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint))
                                {
                                    lstEndPoint.Add(alcr.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint);
                                }
                            }
                            if (acr.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction == null)
                            {
                                acr.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction = new BenMAPHealthImpactFunction();
                                
                            }
                            foreach (string s in lstAuthor)
                            {
                               
                                acr.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author += acr.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author==""?s:" "+s;
                                acr.Author +=acr.Author==""?s:" "+ s;
                            }
                            foreach (string s in lstEndPoint)
                            {
                               
                                acr.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint +=acr.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint==""?s: " "+s;
                                acr.EndPoint +=acr.EndPoint==""?s: " " + s;
                            }
                        }

                    }
                }
                return lstReturn;
            }
            catch
            {
                return null;
            }
        }

        private void btShowTile_Click(object sender, EventArgs e)
        {
            if (!isSelectTile)
            {
                //-----------majie-------
                btShowTile.Text = "Hide Tile";
                //-----------end----------
                splitContainerTile.Panel2.Show();
                splitContainerTile.SplitterDistance = splitContainerTile.Width / 3;
                isSelectTile = true;
            }
            else
            {
                //---------majie-------
                btShowTile.Text = "Show Tile";
                //----------end---------
                splitContainerTile.Panel2.Hide();
                splitContainerTile.SplitterDistance = splitContainerTile.Width;
                isSelectTile = false;

            }

        }

        private void btOLVTileSet_Click(object sender, EventArgs e)
        {
            try
            {
                APVX.TileSet tileSet = new APVX.TileSet();
                tileSet.olv = this.olvTile;
                DialogResult dr = tileSet.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.Cancel)
                    return;
                int count = 0;
                for (int i = 0; i < this.olvTile.AllColumns.Count; i++)
                {
                    if ((this.olvTile.AllColumns[i] as BrightIdeasSoftware.OLVColumn).IsTileViewColumn)
                    {
                        count++;
                        (this.olvTile.AllColumns[i] as BrightIdeasSoftware.OLVColumn).IsVisible = true;
                    }
                    else
                    {
                        (this.olvTile.AllColumns[i] as BrightIdeasSoftware.OLVColumn).IsVisible = false;
                    }
                }
                this.olvTile.TileSize = new Size(120, Convert.ToInt32(count * 15.2 + 10));
                this.olvTile.RebuildColumns();
                this.olvTile.Refresh();
            }
            catch
            { 
            }
        }

        private void SetTileAllSelectCRFunction(AllSelectCRFunction allSelectCRFunction)
        {
            try
            {
                IncidencePoolingAndAggregation ip = CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text).First();
                List<AllSelectCRFunction> lstAllSelectCRFunction = new List<AllSelectCRFunction>();
                if (ip.lstAllSelectCRFuntion.Count == 1) //只有自己
                {

                    lstAllSelectCRFunction.Add(allSelectCRFunction);
                    olvTile.SetObjects(lstAllSelectCRFunction);

                }
                else if (allSelectCRFunction.NodeType == 100)
                {

                    if (allSelectCRFunction.PID == -1)
                    {
                        lstAllSelectCRFunction.Add(allSelectCRFunction);
                        olvTile.SetObjects(lstAllSelectCRFunction);
                    }
                    else
                    {
                        AllSelectCRFunction parent = ip.lstAllSelectCRFuntion.Where(p => p.ID == allSelectCRFunction.PID).First();
                        lstAllSelectCRFunction = ip.lstAllSelectCRFuntion.Where(p => p.PID == parent.ID).ToList();
                        olvTile.SetObjects(lstAllSelectCRFunction);
                    }


                }
                else
                {
                    //---------求所有的子节点----------------
                    APVX.APVCommonClass.getAllChildCR(allSelectCRFunction, ip.lstAllSelectCRFuntion, ref lstAllSelectCRFunction);
                    olvTile.SetObjects(lstAllSelectCRFunction.Where(p => p.NodeType == 100).ToList());
                }
                olvTile.RebuildColumns();
            }
            catch
            { 
            }
        }
        private void treeListView_DoubleClick(object sender, EventArgs e)
        {
            //--------------//--------------考虑选择后的Tile问题-----------------20120104--点击某节点，如果是叶子结点出来所有平级的Tile，如果是父节点出来所有的叶子结点-----
            if (treeListView.SelectedObjects.Count == 0)
                return;
            AllSelectCRFunction allSelectCRFunction = treeListView.SelectedObjects[0] as AllSelectCRFunction;

            SetTileAllSelectCRFunction(allSelectCRFunction);
            //-----------------------------------
        }

        private void btBrowseCR_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = Application.StartupPath + @"E:\";
                openFileDialog.Filter = "CFGR files (*.cfgrx)|*.cfgrx";
                openFileDialog.FilterIndex = 3;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
                //txtOpenExistingCFGR.Text = openFileDialog.FileName;
                if (openFileDialog.FileName != CommonClass.ValuationMethodPoolingAndAggregation.CFGRPath)
                {
                    MessageBox.Show("This is a different .cfgrx file. Choose again.");

                }
                else
                {
                    txtOpenExistingCFGR.Text = openFileDialog.FileName;
                    //-----异步loadCFGR--------------
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("This is a different .cfgrx file. Choose again.");
            }
        }

        private void tabControlSelected_DrawItem(object sender, DrawItemEventArgs e)
        {
            Font fntTab;            
            Brush bshBack;
            Brush bshFore;
            if (e.Index == this.tabControlSelected.SelectedIndex)    //当前Tab页的样式
            {
                //tabControlSelected.TabPages[e.Index].ForeColor = Color.Orange;
                fntTab = new Font(e.Font, FontStyle.Bold);
                bshBack = new SolidBrush(Color.White);
                //bshBack = Brushes.LemonChiffon;
                bshFore = Brushes.Black; 
                //bshFore = new SolidBrush(Color.FromArgb(255,128,0));
            }
            else    //其余Tab页的样式
            {
                fntTab = e.Font;
                bshBack = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, SystemColors.Control, SystemColors.Control, System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal);
                bshFore = new SolidBrush(Color.Black);
            }
            //画样式
            string tabName = this.tabControlSelected.TabPages[e.Index].Text;
            StringFormat sftTab = new StringFormat();
            e.Graphics.FillRectangle(bshBack, e.Bounds);
            Rectangle recTab = e.Bounds;
            recTab = new Rectangle(recTab.X, recTab.Y + 4, recTab.Width, recTab.Height - 4);
            e.Graphics.DrawString(tabName, fntTab, bshFore, recTab, sftTab);
        }

        private void btnShowChanges_Click(object sender, EventArgs e)
        {
            try
            {
                ChangedCRFunctions ccrf = new ChangedCRFunctions();
                ccrf.Show();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void treeListView_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyValue == 46)
                {
                    btDelSelectMethod_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void tbPoolingName_Leave(object sender, EventArgs e)
        {
            btChangeName_Click(sender, e);
        }
    //--------------majie------------
        private void tabControlSelected_MouseDown(object sender, MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Point pt = new Point(e.X, e.Y);
            TabPage tp = GetTabPageByTab(pt);

            if (tp != null)
            {
                DoDragDrop(tp, DragDropEffects.All);
            } 
        }

        private void tabControlSelected_DragOver(object sender, DragEventArgs e)
        {
            
            Point pt = new Point(e.X, e.Y);
            //We need client coordinates.
            pt = tabControlSelected.PointToClient(pt);

            //Get the tab we are hovering over.
            TabPage hover_tab = GetTabPageByTab(pt);

            //Make sure we are on a tab.
            if (hover_tab != null)
            {
                //Make sure there is a TabPage being dragged.
                if (e.Data.GetDataPresent(typeof(TabPage)))
                {
                    e.Effect = DragDropEffects.Move;
                    TabPage drag_tab = (TabPage)e.Data.GetData(typeof(TabPage));

                    int item_drag_index = FindIndex(drag_tab);
                    int drop_location_index = FindIndex(hover_tab);

                    //Don't do anything if we are hovering over ourself.
                    if (item_drag_index != drop_location_index)
                    {
                        ArrayList pages = new ArrayList();
                        List<IncidencePoolingAndAggregation> lstTemp=new List<IncidencePoolingAndAggregation>();
                        //Put all tab pages into an array.
                        for (int i = 0; i < tabControlSelected.TabPages.Count; i++)
                        {
                            //Except the one we are dragging.
                            if (i != item_drag_index)
                            {
                                pages.Add(tabControlSelected.TabPages[i]);
                                lstTemp.Add(CommonClass.lstIncidencePoolingAndAggregation[i]);
                            }
                        }

                        //Now put the one we are dragging it at the proper location.
                        pages.Insert(drop_location_index, drag_tab);
                        lstTemp.Insert(drop_location_index,  CommonClass.lstIncidencePoolingAndAggregation[item_drag_index]);
                        CommonClass.lstIncidencePoolingAndAggregation = lstTemp;
                        //--------------调整Valuation&QALY
                        if (CommonClass.ValuationMethodPoolingAndAggregation != null && CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase != null)
                        {
                            List<ValuationMethodPoolingAndAggregationBase> lstTempVB = new List<ValuationMethodPoolingAndAggregationBase>();
                            //Put all tab pages into an array.
                            for (int i = 0; i < tabControlSelected.TabPages.Count; i++)
                            {
                                //Except the one we are dragging.
                                if (i != item_drag_index)
                                {
                                    lstTempVB.Add(CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[i]);
                                }
                            }

                            //Now put the one we are dragging it at the proper location.

                            lstTempVB.Insert(drop_location_index, CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[item_drag_index]);
                            CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase = lstTempVB;
                        }
                        //----------------------------------------------------------------------
                        //Make them all go away for a nanosec.
                        tabControlSelected.TabPages.Clear();

                        //Add them all back in.
                        tabControlSelected.TabPages.AddRange((TabPage[])pages.ToArray(typeof(TabPage)));

                        //Make sure the drag tab is selected.
                        tabControlSelected.SelectedTab = drag_tab;
                        tabControlSelected_SelectedIndexChanged(sender, e);

                        ////---------------------------modify by xiejp-----changedindex from 
                        //IncidencePoolingAndAggregation ip = CommonClass.lstIncidencePoolingAndAggregation[item_drag_index];
                        //CommonClass.lstIncidencePoolingAndAggregation.RemoveAt(item_drag_index);
                        //CommonClass.lstIncidencePoolingAndAggregation.Insert(
                    }
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Finds the TabPage whose tab is contains the given point.
        /// </summary>
        /// <param name="pt">The point (given in client coordinates) to look for a TabPage.</param>
        /// <returns>The TabPage whose tab is at the given point (null if there isn't one).</returns>
        private TabPage GetTabPageByTab(Point pt)
        {
            TabPage tp = null;

            for (int i = 0; i < tabControlSelected.TabPages.Count; i++)
            {
                if (tabControlSelected.GetTabRect(i).Contains(pt))
                {
                    tp = tabControlSelected.TabPages[i];
                    break;
                }
            }

            return tp;
        }

        /// <summary>
        /// Loops over all the TabPages to find the index of the given TabPage.
        /// </summary>
        /// <param name="page">The TabPage we want the index for.</param>
        /// <returns>The index of the given TabPage(-1 if it isn't found.)</returns>
        private int FindIndex(TabPage page)
        {
            for (int i = 0; i < tabControlSelected.TabPages.Count; i++)
            {
                if (tabControlSelected.TabPages[i] == page)
                    return i;
            }

            return -1;
        }

        private void btShowDetail_Click(object sender, EventArgs e)
        {
            //---------------------修改状态-----------------修改列表里面的树
            switch (btShowDetail.Text)
            {
                case "Detailed View":
                    btShowDetail.Text = "Condensed View";
                    break;
                case "Condensed View":
                    btShowDetail.Text = "Detailed View";
                    break;
                     
            }
            treeListView.RebuildAll(true);
            treeListView.ExpandAll();
        }

        private void treeListView_FormatRow(object sender, FormatRowEventArgs e)
        {
            //e.RowIndex
            //e.Model

            if (btShowDetail.Text == "Detailed View")
            {
                IncidencePoolingAndAggregation ip = CommonClass.lstIncidencePoolingAndAggregation.Where(p => p.PoolingName == tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text).First();
                
                AllSelectCRFunction allSelectCRFuntion = e.Model as AllSelectCRFunction;
                if(ip.lstAllSelectCRFuntion.Count>0 &&allSelectCRFuntion.ID==ip.lstAllSelectCRFuntion.Max(p=>p.ID)+1)
                {
                    e.Item.ForeColor = Color.Black;
                    e.Item.Font = new Font(e.Item.Font, FontStyle.Bold);
                }
            }
        }

        private void treeListView_Freezing(object sender, FreezeEventArgs e)
        {

        }

      
        
        //---------------end-------------
    }

    public class IncidenceDropSink : SimpleDropSink
    {
        public IncidencePoolingandAggregation myIncidencePoolingandAggregation;
        /// <summary>
        /// Create a RearrangingDropSink
        /// </summary>
        public IncidenceDropSink()
        {
            this.CanDropBetween = true;
            this.CanDropOnBackground = true;
            this.CanDropOnItem = false;
        }

        /// <summary>
        /// Create a RearrangingDropSink
        /// </summary>
        /// <param name="acceptDropsFromOtherLists"></param>
        public IncidenceDropSink(bool acceptDropsFromOtherLists, IncidencePoolingandAggregation incidencePoolingandAggregation)
            : this()
        {
            this.AcceptExternal = acceptDropsFromOtherLists;
            myIncidencePoolingandAggregation = incidencePoolingandAggregation;
        }

        /// <summary>
        /// Trigger OnModelCanDrop
        /// </summary>
        /// <param name="args"></param>
        protected override void OnModelCanDrop(ModelDropEventArgs args)
        {
            base.OnModelCanDrop(args);

            if (args.Handled)
                return;

            args.Effect = DragDropEffects.Move;

            // Don't allow drops from other list, if that's what's configured
            if (!this.AcceptExternal && args.SourceListView != this.ListView)
            {
                args.Effect = DragDropEffects.None;
                args.DropTargetLocation = DropTargetLocation.None;
                args.InfoMessage = "This list doesn't accept drops from other lists";
            }

            // If we are rearranging a list, don't allow drops on the background
            if (args.DropTargetLocation == DropTargetLocation.Background && args.SourceListView == this.ListView)
            {
                args.Effect = DragDropEffects.None;
                args.DropTargetLocation = DropTargetLocation.None;
            }
        }

        /// <summary>
        /// Trigger OnModelDropped
        /// </summary>
        /// <param name="args"></param>
        protected override void OnModelDropped(ModelDropEventArgs args)
        {
            //base.OnModelDropped(args);

            if (!args.Handled)
                this.RearrangeModels(args);
        }

        /// <summary>
        /// Do the work of processing the dropped items
        /// </summary>
        /// <param name="args"></param>
        public virtual void RearrangeModels(ModelDropEventArgs args)
        {
            switch (args.DropTargetLocation)
            {
                case DropTargetLocation.AboveItem:
                    if (this.ListView.Name == "treeListView" && args.SourceListView.Name != "treeListView")
                    {
                        myIncidencePoolingandAggregation.btAddCRFunctions_Click(null, null);

                    }
                    else if (this.ListView.Name == "treeListView" && args.SourceListView.Name == "treeListView")
                    {
                        //myIncidencePoolingandAggregation.btDelSelectMethod_Click(null, null);
                        //
                        //  myIncidencePoolingandAggregation.updateDrop(args.SourceModels[0] as AllSelectCRFunction, args.TargetModel as AllSelectCRFunction);

                    }
                    break;
                case DropTargetLocation.BelowItem:
                    if (this.ListView.Name == "treeListView" && args.SourceListView.Name != "treeListView")
                    {
                        //this.ListView.AddObjects(args.SourceModels);
                        myIncidencePoolingandAggregation.btAddCRFunctions_Click(null, null);
                    }
                    else if (this.ListView.Name == "treeListView" && args.SourceListView.Name == "treeListView")
                    {
                        //myIncidencePoolingandAggregation.btDelSelectMethod_Click(null, null);
                        //  myIncidencePoolingandAggregation.updateDrop(args.SourceModels[0] as AllSelectCRFunction, args.TargetModel as AllSelectCRFunction);

                    }
                    break;
                case DropTargetLocation.Background:
                    if (this.ListView.Name == "treeListView" && args.SourceListView.Name != "treeListView")
                    {
                        //this.ListView.AddObjects(args.SourceModels);
                        myIncidencePoolingandAggregation.btAddCRFunctions_Click(null, null);

                    }
                    else
                    {
                        //myIncidencePoolingandAggregation.btDelSelectMethod_Click(null, null);

                    }
                    break;
                default:
                    return;
            }

            if (args.SourceListView != this.ListView && args.SourceListView.Name == "treeListView")
            {
                args.SourceListView.RemoveObjects(args.SourceModels);
            }
        }
    }

}
