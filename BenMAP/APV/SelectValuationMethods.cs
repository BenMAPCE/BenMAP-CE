using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using BrightIdeasSoftware;
using System.Windows.Forms;
using BenMAP.APVX;

namespace BenMAP
{
    public partial class SelectValuationMethods : FormBase
    {
        public SelectValuationMethods()
        {
            InitializeComponent();
            this.tabControlSelection.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlSelection.DrawItem += new DrawItemEventHandler(tabControlSelection_DrawItem);
        }
        List<BenMAPQALY> lstBenMAPQALY = null;// APVX.APVCommonClass.getLstBenMAPQALYFromEndPointGroup(CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethods.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup);

        private bool bIncidenceCanSelected = true;
        private List<AllSelectValuationMethod> lstAllSelectValuationMethod;
        private bool bLoad = false;
        private void SelectValuationMethods_Load(object sender, EventArgs e)
        {
           
            labDisplay.Text = "Click and select the valuation methods on the left panel," + "\n" + "and then drag them to the right panel under the desired Endpoint";
            if (CommonClass.lstIncidencePoolingAndAggregation == null) this.Close();
            try
            {
               // WaitShow("Pooling health impact function results. Please wait.");
                //-----------------------majie---------------------
                if ((CommonClass.LstDelCRFunction == null || CommonClass.LstDelCRFunction.Count == 0) && (CommonClass.LstUpdateCRFunction == null || CommonClass.LstUpdateCRFunction.Count == 0))
                {
                    //btnShowChanges.BackColor = Color.LightBlue;
                    btnShowChanges.Enabled = false;
                }
                else
                {
                    btnShowChanges.Enabled = true;
                    this.toolTip1.SetToolTip(btnShowChanges, "Please resolve conflict between pooling and configuration Files.");
                    //btnShowChanges.BackColor = Color.LightBlue;
                }
                bIncidenceCanSelected = true;
                if (CommonClass.ValuationMethodPoolingAndAggregation == null)
                    CommonClass.ValuationMethodPoolingAndAggregation = new ValuationMethodPoolingAndAggregation();
                CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue;
                CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance = CommonClass.IncidencePoolingAndAggregationAdvance;

                //--------------------------------------

                if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count >0 &&
                 CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues != null &&
                  CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues.Count >= 0)
                {
                    //---------------如果是APVX存入必须先loadcfgr---------但是要判断是否已经load过，如果已经load则不需要load了--

                    CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue;
                    if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null
                                      && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
                    {
                        //--------------首先Aggregation--------------
                        CommonClass.lstCRResultAggregation = new List<CRSelectFunctionCalculateValue>();
                        foreach (CRSelectFunctionCalculateValue crv in CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
                        {
                            CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
                        }
                    }
                }

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
                if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase == null)
                {
                    CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase = new List<ValuationMethodPoolingAndAggregationBase>();
                    foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
                    {
                        CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Add(new ValuationMethodPoolingAndAggregationBase() { IncidencePoolingAndAggregation = ip });
                    }

                }
                else
                {
                    var query = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => !CommonClass.lstIncidencePoolingAndAggregation.Select(a => a.PoolingName).Contains(p.IncidencePoolingAndAggregation.PoolingName));
                    if (query != null && query.Count() > 0)
                    {
                        foreach (ValuationMethodPoolingAndAggregationBase vb in query)
                        {
                            CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Remove(vb);
                        }
                    }
                    foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
                    {
                        if (!CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Select(a => a.IncidencePoolingAndAggregation.PoolingName).Contains(ip.PoolingName))
                        CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Add(new ValuationMethodPoolingAndAggregationBase() { IncidencePoolingAndAggregation = ip });
                    }
                }
                int i = 0;
                foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
                {
                    if (i == 0)
                    {
                        this.tabControlSelection.TabPages[0].Text = ip.PoolingName;
                    }
                    else
                        this.tabControlSelection.TabPages.Add(ip.PoolingName);
                    i++;
                }
                //----------初始化VariableDataset
                DataSet dsVariableDataset = BindVariableDataset();
                cbVariableDataset.DataSource = dsVariableDataset.Tables[0];
                cbVariableDataset.DisplayMember = "SetupVariableDataSetName";
                //cbVariableDataset.SelectedIndex = 0;
                if (CommonClass.ValuationMethodPoolingAndAggregation != null )
                {
                    int iVariableDataset = 0;
                    foreach (DataRow dr in dsVariableDataset.Tables[0].Rows)
                    {
                        if (Convert.ToInt32(dr["SetupVariableDataSetID"]) == CommonClass.ValuationMethodPoolingAndAggregation.VariableDatasetID)
                        {
                            cbVariableDataset.SelectedIndex = iVariableDataset;
                            break;
                        }
                        iVariableDataset++;
                    }
                }
                initTreeView(CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.First());
                //--------------------------------------
                //if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase == null)
                //{
                //    CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase = new List<ValuationMethodPoolingAndAggregationBase>();
                //    foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
                //    {
                //        CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Add(new ValuationMethodPoolingAndAggregationBase() { IncidencePoolingAndAggregation = ip });
                //        foreach (AllSelectCRFunction alsc in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Last().IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.PoolingMethod == "").ToList())
                //        {
                //            if (alsc.PoolingMethod == "")
                //            {
                //                try
                //                {
                //                    if (CommonClass.lstCRResultAggregation == null || CommonClass.lstCRResultAggregation.Count == 0)
                //                    {
                //                        alsc.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(p => p.CRSelectFunction.CRID == alsc.CRID).First();
                //                    }
                //                    else
                //                    {
                //                        alsc.CRSelectFunctionCalculateValue = CommonClass.lstCRResultAggregation.Where(p => p.CRSelectFunction.CRID == alsc.CRID).First();
                //                    }
                //                }
                //                catch
                //                {
                //                }
                //            }
                //        }
                //        //   cbPoolingWindow.Items.Add(ip.PoolingName);
                //        //-----------------------首先得到Pooling--------------------------------------

                //        List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
                //        if (ip.lstAllSelectCRFuntion.First().PoolingMethod == "None")
                //        {
                //            APVX.APVCommonClass.getAllChildCRNotNone(ip.lstAllSelectCRFuntion.First(), ip.lstAllSelectCRFuntion, ref lstCR);

                //        }
                //        lstCR.Insert(0, ip.lstAllSelectCRFuntion.First());
                //        if (lstCR.Count == 1 && ip.lstAllSelectCRFuntion.First().CRID < 9999 && ip.lstAllSelectCRFuntion.First().CRID > 0)//.PoolingMethod == "")
                //        { }
                //        else
                //        {
                //            APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(ref lstCR, ref ip.lstAllSelectCRFuntion, lstCR.Where(p => p.NodeType != 100).Max(p => p.NodeType), ip.lstColumns);
                //        }
                //        //-----------------------每一个生成一棵初始树---------------------------------
                //        List<AllSelectValuationMethod> lstValuationMethod = new List<AllSelectValuationMethod>();
                //        foreach (AllSelectCRFunction allSelectCRFunction in lstCR)
                //        {
                //            AllSelectValuationMethod alv = new AllSelectValuationMethod()
                //            {
                //                CRID = allSelectCRFunction.CRID,
                //                ID = allSelectCRFunction.ID,
                //                PID = allSelectCRFunction.PID,
                //                NodeType = allSelectCRFunction.NodeType,
                //                Name = allSelectCRFunction.Name,
                //                PoolingMethod ="None",// allSelectCRFunction.PoolingMethod == "" ? "None" : allSelectCRFunction.PoolingMethod,
                //                //------add for display
                //                EndPointGroup =ip.lstAllSelectCRFuntion.First().EndPointGroup, //allSelectCRFunction.EndPointGroup,
                //                EndPoint = allSelectCRFunction.EndPoint,
                //                Author = allSelectCRFunction.Author,
                //                Qualifier = allSelectCRFunction.Qualifier,
                //                Location = allSelectCRFunction.Location,
                //                StartAge = allSelectCRFunction.StartAge,
                //                EndAge = allSelectCRFunction.EndAge,
                //                Year = allSelectCRFunction.Year,
                //                OtherPollutants = allSelectCRFunction.OtherPollutants,
                //                Race = allSelectCRFunction.Race,
                //                Ethnicity = allSelectCRFunction.Ethnicity,
                //                Gender = allSelectCRFunction.Gender,
                //                Function = allSelectCRFunction.Function,
                //                Pollutant = allSelectCRFunction.Pollutant,
                //                Metric = allSelectCRFunction.Metric,
                //                SeasonalMetric = allSelectCRFunction.SeasonalMetric,
                //                MetricStatistic = allSelectCRFunction.MetricStatistic,
                //                DataSet = allSelectCRFunction.DataSet,
                //                Version = allSelectCRFunction.Version == 0 ? "" : allSelectCRFunction.Version.ToString(),

                //                //----- add for display
                //            };
                //            //if (lstValuationMethod.Count == 0)
                //            //    alv.PoolingMethod = "None";
                //            lstValuationMethod.Add(alv);
                //        }
                //        CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Count - 1].LstAllSelectValuationMethod = lstValuationMethod;
                //        if (lstCR.Count == 1)
                //        {
                //            CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Count - 1].lstValuationColumns = new List<string>();
                //        }
                //        else
                //            CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Count - 1].lstValuationColumns = ip.lstColumns.GetRange(0, ip.lstAllSelectCRFuntion.Where(p => p.NodeType != 100).Max(p => p.NodeType));


                //    }
                //}
                //else
                //{
                //    foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                //    {
                //        foreach (AllSelectCRFunction alsc in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.PoolingMethod == "").ToList())
                //        {
                //            if (alsc.PoolingMethod == "")
                //            {
                //                try
                //                {
                //                    if (CommonClass.lstCRResultAggregation == null || CommonClass.lstCRResultAggregation.Count == 0)
                //                    {
                //                        alsc.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(p => p.CRSelectFunction.CRID == alsc.CRID).First();
                //                    }
                //                    else
                //                    {
                //                        alsc.CRSelectFunctionCalculateValue = CommonClass.lstCRResultAggregation.Where(p => p.CRSelectFunction.CRID == alsc.CRID).First();
                //                    }
                //                }
                //                catch
                //                {
                //                }
                //            }
                //            else
                //            {
                //                alsc.CRSelectFunctionCalculateValue = null;
                //            }
                //        }

                //        //-----------------------首先得到Pooling--------------------------------------

                //        List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
                //        if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().PoolingMethod == "None")
                //        {
                //            APVX.APVCommonClass.getAllChildCRNotNoneForPooling(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First(), vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);

                //        }
                //        lstCR.Insert(0, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First());
                //        if (lstCR.Count == 1 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID < 9999 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID > 0)//.PoolingMethod == "")
                //        { }
                //        else
                //        {
                //          //  APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.NodeType != 100).Max(p => p.NodeType), vb.IncidencePoolingAndAggregation.lstColumns);
                //        }


                //        //-------------修正某些endpointgroup==null的错误
                //        if (vb.LstAllSelectValuationMethod != null && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion!=null && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Count>0)
                //        {
                //            for (int iavm = 0; iavm < vb.LstAllSelectValuationMethod.Count; iavm++)
                //            {
                //                vb.LstAllSelectValuationMethod[iavm].EndPointGroup = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().EndPointGroup;
                //            }
                //        }
                //    }
                //}

                //lstBenMAPQALY = APVX.APVCommonClass.getLstBenMAPQALYFromEndPointGroup(CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethods.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup);
                //ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                //string commandText = string.Format("select SetupVariableDatasetName from SetupVariableDatasets where SetupID={0}", CommonClass.ManageSetup.SetupID);
                //DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                //cboVariableDataset.DataSource = ds.Tables[0];
                //cboVariableDataset.DisplayMember = "SetupVariableDatasetName";
                //grpAvailableIncidenceResults.Text += " PoolingMethodType : " + Enum.GetNames(typeof(PoolingMethodTypeEnum))[Convert.ToInt32(CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethodType)];
                //lbPoolingWindowName.Text = CommonClass.lstIncidencePoolingAndAggregation.First().PoolingName;
                GC.Collect();
                this.treeListView.OwnerDraw = true;
                this.olvValuationMethods.OwnerDraw = true;
                this.olvValuationMethods.IsSimpleDropSink = true;
                //cbSView.SelectedIndex = 0;
                this.olvValuationMethods.DropSink = new ValuationDropSink(true, this);
                this.treeListView.DropSink = new ValuationDropSink(true, this);

                //olvValuationMethods.SetObjects(APVX.APVCommonClass.getLstBenMAPValuationFuncitonFromEndPointGroupID(CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethods.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID));
                //----------------------初始化treeviewlist---------------------------
                // treeListView.Roots = CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethods;
                //-----------------------
                //chbSkipQALYWeights.Checked = true;
                btnNext.Enabled = false;
                this.treeListView.CanExpandGetter = delegate(object x)
                {
                    AllSelectValuationMethod dir = (AllSelectValuationMethod)x;
                    return (dir.NodeType != 2000);
                };
                this.treeListView.ChildrenGetter = delegate(object x)
                {
                    AllSelectValuationMethod dir = (AllSelectValuationMethod)x;
                    try
                    {
                        return getChildFromAllSelectValuationMethod(dir);


                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return new List<AllSelectValuationMethod>();
                        //return new ArrayList();
                    }
                };


                
                //tabControlSelection.TabPages.Clear();
                
                bLoad = true;
                //如果存在QALY则不选SkipQALY
                //if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.lstAllSelectQALYMethod != null).Count() > 0)
                //    chbSkipQALYWeights.Checked = false;
                tabControlSelection.SelectedIndex = 0;
                updateTreeView();
                treeListView.ExpandAll();
               // WaitClose();
            }
            catch (Exception ex)
            {
            }

        }
        /// <summary>
        /// 用来绑定VariableDataset的Combox
        /// </summary>
        /// <returns></returns>
        private System.Data.DataSet BindVariableDataset()
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select -1 as SetupVariableDatasetid,'' as SetupVariableDataSetName from setupvariabledatasets union select SetupVariableDatasetid,SetupVariableDataSetName from setupvariabledatasets  where SetupID={0}  ", CommonClass.MainSetup.SetupID);
                System.Data.DataSet dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                return dsGrid;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return null;
            }
        }
        private List<AllSelectValuationMethod> getChildFromAllSelectValuationMethod(AllSelectValuationMethod allSelectValuationMethod)
        {
            List<AllSelectValuationMethod> lstAll = new List<AllSelectValuationMethod>();
            ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Text).First();
            
            var query = from a in vb.LstAllSelectValuationMethod where a.PID == allSelectValuationMethod.ID select a;
            lstAll = query.ToList();
            return lstAll;

        }

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            APVConfigurationAdvancedSettings frm = new APVConfigurationAdvancedSettings();
            frm.IncidencePoolingAndAggregationAdvance = CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance;
            DialogResult rtn = frm.ShowDialog();
            if (rtn != DialogResult.OK) { return; }

            CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance = frm.IncidencePoolingAndAggregationAdvance;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        //private void chbSkipQALYWeights_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (chbSkipQALYWeights.Checked)
        //    {
        //        //if (CommonClass.ValuationMethodPoolingAndAggregation != null && CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase != null)
        //        //{
        //        //    foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
        //        //    {
        //        //        vb.lstAllSelectQALYMethod = null;
        //        //    }
        //        //}
        //        btnNext.Enabled = false;
        //    }
        //    else
        //    {

        //        //if (lstBenMAPQALY == null)
        //        // {
        //        //     MessageBox.Show("The EndPointGroup can't do QALY!");
        //        //     this.chbSkipQALYWeights.Checked = true;
        //        //     return;

        //        // }
        //        // else if (lstBenMAPQALY.Count == 0)
        //        // {

        //        //     MessageBox.Show("The EndPointGroup can't do QALY!");
        //        //     this.chbSkipQALYWeights.Checked = true;
        //        //     return;
        //        // }
        //        //  btnNext.Enabled = true;
        //    }
        //}

        private void btnNext_Click(object sender, EventArgs e)
        {
            bool bWeight = false;
            foreach (AllSelectValuationMethod allSelectValuationMethod in lstAllSelectValuationMethod)
            {
                if (allSelectValuationMethod.PoolingMethod == "User Defined Weights")
                {
                    bWeight = true;
                }

            }
            if (bWeight)
            {
                //出来Weight的设置界面，设置完毕返回---------
            }

            //DataRowView drv = cboVariableDataset.SelectedItem as DataRowView;
            //CommonClass.lstIncidencePoolingAndAggregation.First().VariableDataset = drv["SetupVariableDatasetName"].ToString();
            //if (!chbSkipQALYWeights.Checked)
            //{
            //    SelectQALYMethods frm = new SelectQALYMethods();
            //    DialogResult rtn = frm.ShowDialog();
            //    if (rtn != DialogResult.OK) { return; }
            //}
        }
        public void addColumnsToTree(List<string> lstColumns)
        {
            //if (treeListView.Columns.Count > 4) return;
            while (treeListView.Columns.Count > 5)
            {
                treeListView.Columns.RemoveAt(5);
            }
            //-------------根据排序加载!-------------
            if (lstColumns == null || lstColumns.Count == 0) return;
            foreach (string s in lstColumns)
            {
                OLVColumn olvc = new OLVColumn();

                switch (s.Replace(" ", "").ToLower())
                {
                    case "version":
                        olvc.Text = "Version";
                        olvc.AspectName = "Version";
                        break;
                    case "endpoint":
                        olvc.Text = "End Point";
                        olvc.AspectName = "EndPoint";
                        break;
                    case "author":
                        olvc.Text = "Author";
                        olvc.AspectName = "Author";
                        break;
                    case "qualifier":
                        olvc.Text = "Qualifier";
                        olvc.AspectName = "Qualifier";
                        break;
                    case "location":
                        olvc.Text = "Location";
                        olvc.AspectName = "Location";
                        break;
                    case "startage":
                        olvc.Text = "Start Age";
                        olvc.AspectName = "StartAge";
                        break;
                    case "endage":
                        olvc.Text = "End Age";
                        olvc.AspectName = "EndAge";
                        break;
                    case "year":
                        olvc.Text = "Year";
                        olvc.AspectName = "Year";
                        break;
                    case "otherpollutants":
                        olvc.Text = "Other Pollutants";
                        olvc.AspectName = "OtherPollutants";
                        break;
                    case "race":
                        olvc.Text = "Race";
                        olvc.AspectName = "Race";
                        break;
                    case "ethnicity":
                        olvc.Text = "Ethnicity";
                        olvc.AspectName = "Ethnicity";
                        break;
                    case "gender":
                        olvc.Text = "Gender";
                        olvc.AspectName = "Gender";
                        break;
                    case "function":
                        olvc.Text = "Function";
                        olvc.AspectName = "Function";
                        break;
                    case "pollutant":
                        olvc.Text = "Pollutant";
                        olvc.AspectName = "Pollutant";
                        break;
                    case "metric":
                        olvc.Text = "Metric";
                        olvc.AspectName = "Metric";
                        break;
                    case "seasonalmetric":
                        olvc.Text = "Seasonal Metric";
                        olvc.AspectName = "SeasonalMetric";
                        break;
                    case "metricstatistic":
                        olvc.Text = "Metric Statistic";
                        olvc.AspectName = "MetricStatistic";
                        break;
                    case "dataset":
                        olvc.Text = "Data Set";
                        olvc.AspectName = "DataSet";
                        break;

                }
                olvc.Name = "Modify" + olvc.AspectName;

                olvc.HeaderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
                treeListView.Columns.Add(olvc);
            }
            treeListView.Refresh();

            //treeListView.ColumnReordered -= new System.Windows.Forms.ColumnReorderedEventHandler(this.treeListView_ColumnReordered);
            //treeListView.ColumnReordered += new System.Windows.Forms.ColumnReorderedEventHandler(this.treeListView_ColumnReordered);
        }

        private void initTreeView(ValuationMethodPoolingAndAggregationBase vb)
        {
            try
            {
                IncidencePoolingAndAggregation incidencePoolingAndAggregation = vb.IncidencePoolingAndAggregation;
                List<BenMAPValuationFunction> lstBenMAPValuationFunction = new List<BenMAPValuationFunction>();
                lstAllSelectValuationMethod = vb.LstAllSelectValuationMethod;
                List<AllSelectValuationMethod> lstRoot = new List<AllSelectValuationMethod>();
                if (lstAllSelectValuationMethod == null) lstAllSelectValuationMethod = new List<AllSelectValuationMethod>();
                //if (incidencePoolingAndAggregation.PoolingMethodType != PoolingMethodTypeEnum.None)
                //{

                lstBenMAPValuationFunction = APVX.APVCommonClass.getLstBenMAPValuationFuncitonFromEndPointGroupID(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().EndPointGroupID);//.Where(p => p.CRSelectFunctionCalculateValue != null).First().CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID);
                for (int iEndPointGroup = 1; iEndPointGroup < vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Count; iEndPointGroup++)
                {
                    if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[iEndPointGroup].EndPointGroup != vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[iEndPointGroup - 1].EndPointGroup)
                    {
                        List<BenMAPValuationFunction> lstTemp = APVX.APVCommonClass.getLstBenMAPValuationFuncitonFromEndPointGroupID(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[iEndPointGroup].EndPointGroupID);//.Where(p => p.CRSelectFunctionCalculateValue != null).First().CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID);
                        if (lstTemp != null && lstTemp.Count > 0) lstBenMAPValuationFunction.AddRange(lstTemp);

                    }
                }
                //if(olvValuationMethods.Objects==null)
                olvValuationMethods.SetObjects(lstBenMAPValuationFunction);
                //}
                //lstAllSelectValuationMethod=incidencePoolingAndAggregation.

                if (lstAllSelectValuationMethod.Count == 0)
                {

                    //-----------------------首先得到Pooling--------------------------------------
                    //-modify by xiejp 支持多个EndPointGroup----------
                    foreach (AllSelectCRFunction ascr in incidencePoolingAndAggregation.lstAllSelectCRFuntion)
                    {
                        if (ascr.CRID >= 9999) ascr.CRID = -1;
                    }
                    var query = incidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.PID == -1);
                    foreach (AllSelectCRFunction allSelectCRFunctionFirst in query)
                    {
                        List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
                        //if (allSelectCRFunctionFirst.PoolingMethod == "None")
                        //{
                        APVX.APVCommonClass.getAllChildCR(allSelectCRFunctionFirst, incidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);

                        //}
                        lstCR.Insert(0, allSelectCRFunctionFirst);
                        if (lstCR.Count == 1 && lstCR.First().CRID < 9999 && lstCR.First().CRID > 0)//.PoolingMethod == "")
                        { }
                        else
                        {
                            APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(false,ref lstCR, ref incidencePoolingAndAggregation.lstAllSelectCRFuntion, lstCR.Where(p => p.NodeType != 100).Max(p => p.NodeType), incidencePoolingAndAggregation.lstColumns);
                        }
                        //-重新生成LstCR------
                        lstCR = new List<AllSelectCRFunction>();
                        if (allSelectCRFunctionFirst.PoolingMethod == "None")
                        {
                            APVX.APVCommonClass.getAllChildCRNotNone(allSelectCRFunctionFirst, incidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);

                        }
                        lstCR.Insert(0, allSelectCRFunctionFirst);
                        //-----------------------每一个生成一棵初始树---------------------------------
                        //List<AllSelectValuationMethod> lstValuationMethod = new List<AllSelectValuationMethod>();
                        vb.LstAllSelectValuationMethod = new List<AllSelectValuationMethod>();
                        foreach (AllSelectCRFunction allSelectCRFunction in lstCR)
                        {
                            AllSelectValuationMethod alv = new AllSelectValuationMethod()
                            {
                                CRID = allSelectCRFunction.CRID,
                                ID = allSelectCRFunction.ID,
                                PID = allSelectCRFunction.PID,
                                NodeType = allSelectCRFunction.NodeType,
                                Name = allSelectCRFunction.Name,
                                PoolingMethod = "None", //modify by xiejp 全部为None allSelectCRFunction.PoolingMethod == "" ? "None" : allSelectCRFunction.PoolingMethod,
                                //------add for display
                                EndPointGroup = allSelectCRFunctionFirst.EndPointGroup,// allSelectCRFunction.EndPointGroup,
                                EndPoint = allSelectCRFunction.EndPoint,
                                Author = allSelectCRFunction.Author,
                                Qualifier = allSelectCRFunction.Qualifier,
                                Location = allSelectCRFunction.Location,
                                StartAge = allSelectCRFunction.StartAge,
                                EndAge = allSelectCRFunction.EndAge,
                                Year = allSelectCRFunction.Year,
                                OtherPollutants = allSelectCRFunction.OtherPollutants,
                                Race = allSelectCRFunction.Race,
                                Ethnicity = allSelectCRFunction.Ethnicity,
                                Gender = allSelectCRFunction.Gender,
                                Function = allSelectCRFunction.Function,
                                Pollutant = allSelectCRFunction.Pollutant,
                                Metric = allSelectCRFunction.Metric,
                                SeasonalMetric = allSelectCRFunction.SeasonalMetric,
                                MetricStatistic = allSelectCRFunction.MetricStatistic,
                                DataSet = allSelectCRFunction.DataSet,
                                Version =  allSelectCRFunction.Version.ToString(),

                                //----- add for display
                            };
                            //if (lstValuationMethod.Count == 0)
                            //    alv.PoolingMethod = "None";
                            lstAllSelectValuationMethod.Add(alv);
                        }
                        //---------Add所有的---------
                        if (vb.LstAllSelectValuationMethod.Count() > 0)
                        {
                            for (int iTemp = 0; iTemp < lstAllSelectValuationMethod.Count; iTemp++)
                            {
                                lstAllSelectValuationMethod[iTemp].ID = lstAllSelectValuationMethod[iTemp].ID + vb.LstAllSelectValuationMethod.Max(p => p.ID) + 1;
                                if (lstAllSelectValuationMethod[iTemp].PID != -1)
                                    lstAllSelectValuationMethod[iTemp].PID = lstAllSelectValuationMethod[iTemp].PID + vb.LstAllSelectValuationMethod.Max(p => p.ID) + 1;
                            }
                        }
                        vb.LstAllSelectValuationMethod.AddRange(lstAllSelectValuationMethod);

                        if (lstCR.Count == 1) //-------有些问题----
                        {
                            //vb.lstValuationColumns = new List<string>();
                        }
                        else
                        {
                            if (vb.lstValuationColumns == null || vb.lstValuationColumns.Count == 0)
                                vb.lstValuationColumns = incidencePoolingAndAggregation.lstColumns.GetRange(0, incidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.NodeType != 100).Max(p => p.NodeType));
                        }
                    }
                    if (tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Text != vb.IncidencePoolingAndAggregation.PoolingName) return;
                    //vb.LstAllSelectValuationMethod = lstAllSelectValuationMethod;
                    lstRoot = new List<AllSelectValuationMethod>();
                    lstRoot.Add(vb.LstAllSelectValuationMethod.First());
                    for (int iRoot = 1; iRoot < vb.LstAllSelectValuationMethod.Count; iRoot++)
                    {
                        if (lstRoot.Where(p => p.EndPointGroup == vb.LstAllSelectValuationMethod[iRoot].EndPointGroup).Count() == 0
                           )
                            lstRoot.Add(vb.LstAllSelectValuationMethod[iRoot]);
                    }
                    treeListView.Roots = lstRoot;// lstAllSelectValuationMethod.GetRange(0, 1);
                    //------------majie-------------
                    this.treeColumnName.ImageGetter = delegate(object x)
                    {
                        if (((AllSelectValuationMethod)x).NodeType == 100)
                            return 1;
                        else if (((AllSelectValuationMethod)x).NodeType == 2000)
                            return 2;
                        else
                            return 0;
                    };

                    treeListView.RebuildAll(true);
                    treeListView.ExpandAll();
                }
                else
                {
                    // vb.LstAllSelectValuationMethod = lstAllSelectValuationMethod;
                    lstRoot = new List<AllSelectValuationMethod>();
                    lstRoot.Add(vb.LstAllSelectValuationMethod.First());
                    for (int iRoot = 1; iRoot < vb.LstAllSelectValuationMethod.Count; iRoot++)
                    {
                        if (lstRoot.Where(p => p.EndPointGroup == vb.LstAllSelectValuationMethod[iRoot].EndPointGroup).Count() == 0
                            )
                            lstRoot.Add(vb.LstAllSelectValuationMethod[iRoot]);
                    }
                    treeListView.Roots = lstRoot;// lstAllSelectValuationMethod.GetRange(0, 1);
                    this.treeColumnName.ImageGetter = delegate(object x)
                    {
                        if (((AllSelectValuationMethod)x).NodeType == 100)
                            return 1;
                        else if (((AllSelectValuationMethod)x).NodeType == 2000)
                            return 2;
                        else
                            return 0;
                    };
                    treeListView.RebuildAll(true);
                    treeListView.ExpandAll();


                }
                //treeListView.ExpandAll();
                if (vb.LstAllSelectValuationMethod.Count > 1 && vb.lstValuationColumns != null && vb.lstValuationColumns.Count > 0 && treeListView.Columns.Count == 5)
                    addColumnsToTree(vb.lstValuationColumns);
            }
            catch
            { 
            }
        }

        private void updateTreeView()
        {
            treeListView.RebuildAll(true);
            treeListView.ExpandAll();
            
        }



        private void olvIncidenceResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethodType == PoolingMethodTypeEnum.None)
            //{
            //    try
            //    {
            //        // List<CRSelectFunctionCalculateValue> lstbvf = (List<CRSelectFunctionCalculateValue>)olvIncidenceResults.SelectedObjects;
            //        CRSelectFunctionCalculateValue crfcv = (CRSelectFunctionCalculateValue)olvIncidenceResults.SelectedObjects[0];
            //        List<BenMAPValuationFunction> lstBenMAPValuationFunction = APVX.APVCommonClass.getLstBenMAPValuationFuncitonFromEndPointGroupID(crfcv.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID);
            //        olvValuationMethods.SetObjects(lstBenMAPValuationFunction);
            //    }
            //    catch (Exception ex)
            //    {
            //    }

            //}

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
                // (e.RowObject as AllSelectQALYMethod).PoolingMethod=
                // We have updated the model object, so we cancel the auto update
                e.Cancel = true;
            }
        }

        private void treeListView_CellEditStarting(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            base.OnClick(e);
            if (e.Column == null) return;
            AllSelectValuationMethod asvm = (AllSelectValuationMethod)e.RowObject;

            if (e.Column.Text == "Pooling Method" && asvm.NodeType != 2000)
            {

                ComboBox cb = new ComboBox();
                cb.Bounds = e.CellBounds;
                cb.Font = ((TreeListView)sender).Font;
                cb.DropDownStyle = ComboBoxStyle.DropDownList;
                //cb.Items.AddRange(Enum.GetNames(typeof(PoolingMethodTypeEnum)));
                //------------暂时去掉Weight因为没有做--------------------------------------------------------
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
        void cbPoolingMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBox cb = (ComboBox)sender;
                ((AllSelectValuationMethod)cb.Tag).PoolingMethod = cb.Text;
                lstAllSelectValuationMethod.Where(p => p.ID == ((AllSelectValuationMethod)cb.Tag).ID).First().PoolingMethod = cb.Text;
                //if (cb.Text == "SubjectiveWeights")
                //{
                //    btnOK.Enabled = false;
                //    btnSave.Enabled = false;
                //    btnNext.Enabled = true;

                //}
            }
            catch
            { }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //---------------------保存到CommonClass底下并且Save----------------------------------------
            SaveFileDialog sfdAPV = new SaveFileDialog();
            sfdAPV.Filter = "APV files (*.apvx)|*.apvx";
            sfdAPV.FilterIndex = 2;
            sfdAPV.RestoreDirectory = true;
            sfdAPV.InitialDirectory = CommonClass.ResultFilePath + @"\Result\APV";
            if (sfdAPV.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string _filePathAPV = sfdAPV.FileName;
                CommonClass.ValuationMethodPoolingAndAggregation.VariableDatasetID = Convert.ToInt32((cbVariableDataset.SelectedItem as DataRowView)["SetupVariableDatasetID"].ToString());
                CommonClass.ValuationMethodPoolingAndAggregation.VariableDatasetName = cbVariableDataset.Text;
                CommonClass.ValuationMethodPoolingAndAggregation.Version = "BenMAP-CE " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);
                if (APVX.APVCommonClass.SaveAPVFile(_filePathAPV, CommonClass.ValuationMethodPoolingAndAggregation))
                    MessageBox.Show("APV file has been saved.", "File saved");
                else
                    MessageBox.Show("Out of memory.", "Error");
            }
            //CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregation = CommonClass.lstIncidencePoolingAndAggregation.First();
            //CommonClass.ValuationMethodPoolingAndAggregation.LstAllSelectValuationMethod = lstAllSelectValuationMethod;
            //---------------Save-------------------------------另外写
        }
        public string _filePath = "";
        public void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult rtn;
                bool isBatch = false;
                if (CommonClass.InputParams != null && CommonClass.InputParams.Count() > 0 && CommonClass.InputParams[0].ToLower().Contains(".ctlx"))
                {
                    isBatch = true;
                    CommonClass.BenMAPPopulation = CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
                    CommonClass.GBenMAPGrid = CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.First().GridType;
                }
                if (CommonClass.ValuationMethodPoolingAndAggregation == null) { CommonClass.ValuationMethodPoolingAndAggregation = new ValuationMethodPoolingAndAggregation(); }
                //ValuationMethodPoolingAndAggregationBase vbold = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == this.tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Text).First();
                //vbold.LstAllSelectValuationMethod = lstAllSelectValuationMethod;
                //--------必须选择VariableDataset如果有的话---------
                if (cbVariableDataset.Items.Count > 1 && cbVariableDataset.SelectedIndex==0)
                {
                    MessageBox.Show("Select Variable Dataset first!");
                    return;
                }
                if (!isBatch)
                {
                    CommonClass.ValuationMethodPoolingAndAggregation.VariableDatasetID = Convert.ToInt32((cbVariableDataset.SelectedItem as DataRowView)["SetupVariableDatasetID"].ToString());
                    CommonClass.ValuationMethodPoolingAndAggregation.VariableDatasetName = cbVariableDataset.Text;
                }
                CommonClass.ValuationMethodPoolingAndAggregation.Version = "BenMAP-CE " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);

                foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                {
                    if (vb.LstAllSelectValuationMethod == null || vb.LstAllSelectValuationMethod.Count == 0)
                    {
                        //MessageBox.Show("Please set Valuation method first.");
                        //return;
                        initTreeView( vb);

                    }   
                    else if (vb.LstAllSelectValuationMethod.Where(p => p.NodeType == 2000).Count() == 0)
                    {
                        //-------------------不判断直接往下--2012222
                        //MessageBox.Show("Please set Valuation method first.");
                        //return;
                    }

                }
                //-------clearCalculateFunctionString
                if (Tools.CalculateFunctionString.dicValuationMethodInfo != null) Tools.CalculateFunctionString.dicValuationMethodInfo.Clear();
                //CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregation = CommonClass.lstIncidencePoolingAndAggregation.First();
                //CommonClass.ValuationMethodPoolingAndAggregation.LstAllSelectValuationMethod = lstAllSelectValuationMethod;
                //CommonClass.IncidencePoolingResult = null;
                //if (CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethodType != PoolingMethodTypeEnum.None)
                //{
                //    CommonClass.IncidencePoolingResult = APVX.APVCommonClass.getPoolingMethodCRSelectFunctionCalculateValue(CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethods, CommonClass.lstIncidencePoolingAndAggregation.First().PoolingMethodType, CommonClass.lstIncidencePoolingAndAggregation.First().Weights);
                //}
                //---------------------------首先判断是否有Weight---------------------------------------------------------
                foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                {
                    bool bWeight = false;
                    foreach (AllSelectValuationMethod allSelectValuationMethod in vb.LstAllSelectValuationMethod)
                    {
                        if (allSelectValuationMethod.PoolingMethod == "User Defined Weights")
                        {
                            bWeight = true;
                        }

                    }
                    if (bWeight)
                    {
                        //出来Weight的设置界面，设置完毕返回---------
                        APVX.SelectValuationWeight selectfrm = new APVX.SelectValuationWeight();
                        selectfrm.txtPoolingWindowName.Text = vb.IncidencePoolingAndAggregation.PoolingName;
                        selectfrm.lstAllSelectValuationMethod = vb.LstAllSelectValuationMethod;
                        DialogResult rtnWeight = selectfrm.ShowDialog();
                        if (rtnWeight != DialogResult.OK)
                        {
                            return;
                        }
                        lstAllSelectValuationMethod = selectfrm.lstAllSelectValuationMethod;
                    }
                }
                //----------------modify by xiejp 20120206 首先Aggregation 所有的CR，然后计算结果--------------------------------------------------------------------
                if (CommonClass.lstCRResultAggregation == null || CommonClass.lstCRResultAggregation.Count == 0)
                {
                    CommonClass.lstCRResultAggregation = new List<CRSelectFunctionCalculateValue>();
                    if (CommonClass.BaseControlCRSelectFunctionCalculateValue == null || CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count == 0 ||
                    CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues == null ||
                     CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues.Count == 0)
                    {
                    }
                    else
                    {
                        if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null
                                               && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
                        {
                            foreach (CRSelectFunctionCalculateValue crv in CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
                            {
                                CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
                            }
                        }
                    }
                }
                //---------------------------第二判断是否需要QALY---------------------------------------------------------
                //if (!chbSkipQALYWeights.Checked &&1==2)
                //{
                //    SelectQALYMethods frm = new SelectQALYMethods();
                //    rtn = frm.ShowDialog();
                //    if (rtn == System.Windows.Forms.DialogResult.Abort)
                //        chbSkipQALYWeights.Checked = true;
                //    else if (rtn != DialogResult.OK) { return; }
                //}
                //if (chbSkipQALYWeights.Checked || 1==1)
                //{
                    //清空QALY
                    foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                    {
                        vb.lstAllSelectQALYMethod = null;
                        vb.lstAllSelectQALYMethodAndValue = null;
                        vb.lstAllSelectQALYMethodAndValueAggregation = null;
                    }
                //}
                if (CommonClass.BaseControlCRSelectFunctionCalculateValue == null || CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count == 0 ||
                  CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues == null ||
                   CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues.Count == 0)
                {
                    //---------------如果是APVX存入必须先loadcfgr---------但是要判断是否已经load过，如果已经load则不需要load了--
                    if (CommonClass.ValuationMethodPoolingAndAggregation.CFGRPath == "")
                    {
                        MessageBox.Show("Please run Health Impact Functions first.");

                        return;
                    }
                }
                //---------------------------开始运算---------------------------------------------------------------------
                string _filePathAPV = "";
                if (!isBatch)
                {
                    _filePath = "";
                    
                    //----------------------提示是否SaveToFile-----------------------
                    rtn = MessageBox.Show("Save the APV results file (*.apvrx)?", "Confirm Save", MessageBoxButtons.YesNo);
                    if (rtn == DialogResult.No) { return; }
                    if (rtn == DialogResult.Yes)
                    {
                        //SaveFileDialog sfdAPV = new SaveFileDialog();
                        //sfdAPV.Filter = "APVX files (*.apvx)|*.apvx";
                        //sfdAPV.FilterIndex = 2;
                        //sfdAPV.RestoreDirectory = true;

                        //if (sfdAPV.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        //{
                        //    _filePathAPV = sfdAPV.FileName;
                        //}
                        //弹出窗口让其保存---------------------------------------------------
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = "APVR files (*.apvrx)|*.apvrx";
                        sfd.FilterIndex = 2;
                        sfd.RestoreDirectory = true;
                        sfd.InitialDirectory = CommonClass.ResultFilePath + @"\Result\APVR";
                        if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }
                        _filePath = sfd.FileName;


                        //_filePath = sfd.FileName.Substring(0, sfd.FileName.LastIndexOf(@"\") + 1);
                    }
                }
                string tip = "Running to create pooled results file. Please wait.";
                //----------------Save To APVX File--------------------------------------
                if (_filePathAPV != "")
                {
                    //-------------save result------------------
                    if (APVX.APVCommonClass.SaveAPVFile(_filePathAPV, CommonClass.ValuationMethodPoolingAndAggregation))
                        MessageBox.Show("APV file saved.", "File saved");
                    else
                        MessageBox.Show("Out of memory.", "Error");

                }
                //if (_filePath != "")
                //{
                //    //-------------save result------------------
                //    //valuationMethodPoolingAndAggregation.CFGRPath = strFile.Substring(0, strFile.Length - 6) + ".cfgrx";
                //    Configuration.ConfigurationCommonClass.SaveCRFRFile(CommonClass.BaseControlCRSelectFunctionCalculateValue, _filePath.Substring(0, _filePath.Length - 6) + ".cfgrx");

                //    //if (APVX.APVCommonClass.SaveAPVRFile(_filePath, CommonClass.ValuationMethodPoolingAndAggregation))
                //    //{
                //    //    if (!isBatch) MessageBox.Show("File saved.");
                //    //}
                //    //else
                //    //{
                //    //    if (!isBatch) MessageBox.Show("Out of memory.");
                //    //}

                //}
                
                //-----------------------------------------------------------------------
                //开线程Run And 赋值 CommonClass.BaseControlCRFunction 
                if(!isBatch) WaitShow(tip);
                DateTime dtRunStart = DateTime.Now;
                if (CommonClass.BaseControlCRSelectFunctionCalculateValue == null || CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count == 0 ||
                 CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues == null ||
                  CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.First().CRCalculateValues.Count == 0)
                {
                    //---------------如果是APVX存入必须先loadcfgr---------但是要判断是否已经load过，如果已经load则不需要load了--
                    string err = "";
                    CommonClass.BaseControlCRSelectFunctionCalculateValue = Configuration.ConfigurationCommonClass.LoadCFGRFile(CommonClass.ValuationMethodPoolingAndAggregation.CFGRPath,ref err);
                    if (CommonClass.BaseControlCRSelectFunctionCalculateValue == null)
                    {
                        MessageBox.Show(err);
                        return;
                    }
                    CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue;
                    if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null
                                      && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
                    {
                        //--------------首先Aggregation--------------
                        CommonClass.lstCRResultAggregation = new List<CRSelectFunctionCalculateValue>();
                        foreach (CRSelectFunctionCalculateValue crv in CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
                        {
                            CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
                        }
                    }
                }
                //----------------do pooling-------
                foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                {
                    foreach (AllSelectCRFunction alsc in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.PoolingMethod == "").ToList())
                    {
                        if (alsc.PoolingMethod == "")
                        {
                            try
                            {
                                if (CommonClass.lstCRResultAggregation.Count == 0)
                                {
                                    alsc.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(p => p.CRSelectFunction.CRID == alsc.CRID).First();
                                }
                                else
                                {
                                    alsc.CRSelectFunctionCalculateValue = CommonClass.lstCRResultAggregation.Where(p => p.CRSelectFunction.CRID == alsc.CRID).First();
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                //----------------do pooling-------
                foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                {
                    //foreach (AllSelectCRFunction alsc in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.PoolingMethod != "" && p.PoolingMethod != "None").ToList())
                    //{
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
                    //        alsc.CRSelectFunctionCalculateValue = APVX.APVCommonClass.getPoolingMethodCRSelectFunctionCalculateValue(lstSec.Where(p => p.CRSelectFunctionCalculateValue != null && p.CRSelectFunctionCalculateValue.CRCalculateValues != null).Select(a => a.CRSelectFunctionCalculateValue).ToList(), APVX.APVCommonClass.getPoolingMethodTypeEnumFromString(alsc.PoolingMethod), lstSec.Where(p => p.CRSelectFunctionCalculateValue != null).Select(a => a.Weight).ToList());
                    //    }
                    //}
                    //-----------------------首先得到Pooling--------------------------------------
                    var query = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.PID == -1);
                    foreach (AllSelectCRFunction allSelectCRFunctionFirst in query)
                    {
                        List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
                        //if (allSelectCRFunctionFirst.PoolingMethod == "None")
                        //{
                            APVX.APVCommonClass.getAllChildCR(allSelectCRFunctionFirst, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);

                        //}
                        lstCR.Insert(0, allSelectCRFunctionFirst);
                        if (lstCR.Count == 1 && lstCR.First().CRID < 9999 && lstCR.First().CRID > 0)//.PoolingMethod == "")
                        { }
                        else
                        {
                            APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(true,ref lstCR, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, lstCR.Where(p => p.NodeType != 100).Max(p => p.NodeType), vb.IncidencePoolingAndAggregation.lstColumns);
                        }
                    }
                    //List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
                    //if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().PoolingMethod == "None")
                    //{
                    //    APVX.APVCommonClass.getAllChildCRNotNone(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First(), vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);

                    //}
                    //lstCR.Insert(0, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First());
                    //if (lstCR.Count == 1 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID < 9999 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID > 0)//.PoolingMethod == "")
                    //{ }
                    //else
                    //{
                    //    //if(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p=>p.PoolingMethod!="" && p.PoolingMethod!=null &&p.CRID<9999 ).Count()>0)
                    //    APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.NodeType != 100).Max(p => p.NodeType), vb.IncidencePoolingAndAggregation.lstColumns);
                    //}
                    //---------------------------------------------------
                }

                //----------------------生成Result--------------------------------------
                CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance = CommonClass.IncidencePoolingAndAggregationAdvance;
                APVX.APVCommonClass.CalculateValuationMethodPoolingAndAggregation(ref CommonClass.ValuationMethodPoolingAndAggregation);
                //ApplyAggregationFromValuationMethodPoolingAndAggregation(CommonClass.LstGridRelationshipAll, CommonClass.GBenMAPGrid, ref  CommonClass.ValuationMethodPoolingAndAggregation);
                //--------------适应aggregation----------
                if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
                    CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null &&
                    CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
                {

                }
                //------------Add log to apvr
                DateTime dtNow = DateTime.Now;
                TimeSpan ts = dtNow - dtRunStart;
                //MessageBox.Show("HIF running time: " + ts.Hours + " hours " + ts.Minutes + " minutes " + ts.Seconds + " seconds!");
                CommonClass.ValuationMethodPoolingAndAggregation.lstLog = new List<string>();
                CommonClass.ValuationMethodPoolingAndAggregation.lstLog.Add("Processing complete. Valuation processing time: " + ts.Hours + " hours " + ts.Minutes + " minutes " + ts.Seconds + " seconds.");
                if (!isBatch) WaitClose();
                //-----赋值CreateTime
                CommonClass.ValuationMethodPoolingAndAggregation.CreateTime = DateTime.Now;
                //----------------------Save To File----------------------------
                if (_filePath != "")
                {
                    //-------------save result------------------
                    Configuration.ConfigurationCommonClass.SaveCRFRFile(CommonClass.BaseControlCRSelectFunctionCalculateValue, _filePath.Substring(0, _filePath.Length - 6) + ".cfgrx");

                    if (APVX.APVCommonClass.SaveAPVRFile(_filePath, CommonClass.ValuationMethodPoolingAndAggregation))
                    {
                        if (!isBatch) MessageBox.Show("File saved.", "File saved");
                    }
                    else
                    {
                        if (!isBatch) MessageBox.Show("Out of memory.", "Error");
                    }

                }
                if (!isBatch)
                {
                    GC.Collect();
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }

            }

            catch (Exception ex)
            {

            }
        }
        public delegate void AsyncValuation(GridRelationship gridRelationship, BenMAPGrid benMAPGrid, AllSelectValuationMethodAndValue asvv, ref ValuationMethodPoolingAndAggregationBase vb);
        public delegate void AsyncQALY(GridRelationship gridRelationship, BenMAPGrid benMAPGrid, AllSelectQALYMethodAndValue asvv, ref ValuationMethodPoolingAndAggregationBase vb);
        public delegate void AsyncIncidence(GridRelationship gridRelationship, BenMAPGrid benMAPGrid, CRSelectFunctionCalculateValue asvv, ref ValuationMethodPoolingAndAggregationBase vb);

        public void ApplyAggregationFromValuationMethodPoolingAndAggregation(List<GridRelationship> lstGridRelationshipAll, BenMAPGrid gBenMAPGrid, ref ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation)
        {
            try
            {
                int icount = 0;
                double d = 0;
                int idAggregation = -1;
                GridRelationship gridRelationship = null;
                GridRelationship gridRelationshipIncidence = null;
                if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
                       valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null &&
                        valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase != null)
                {
                    //----修正所有的Valuation
                    if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != gBenMAPGrid.GridDefinitionID)
                    {
                        idAggregation = valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID;

                        if (lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).Count() > 0)
                        {
                            gridRelationship = lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).First();
                        }
                        else
                        {
                            gridRelationship = lstGridRelationshipAll.Where(p => p.bigGridID == idAggregation && p.smallGridID == gBenMAPGrid.GridDefinitionID).First();

                        }


                        for (int ivb = 0; ivb < valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Count; ivb++)
                        {
                            ValuationMethodPoolingAndAggregationBase vb = valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[ivb];
                            if (vb.LstAllSelectValuationMethodAndValue != null)
                            {
                                vb.LstAllSelectValuationMethodAndValueAggregation = new List<AllSelectValuationMethodAndValue>();

                                foreach (AllSelectValuationMethodAndValue asvv in vb.LstAllSelectValuationMethodAndValue)
                                {
                                    //agregation
                                    //AllSelectValuationMethodAndValue asvvnew = new AllSelectValuationMethodAndValue();
                                    //asvvnew.AllSelectValuationMethod = asvv.AllSelectValuationMethod;
                                    //asvvnew.lstAPVValueAttributes = new List<APVValueAttribute>();

                                    //asvv.ResultCopy = null;
                                    vb.LstAllSelectValuationMethodAndValueAggregation.Add(APVX.APVCommonClass.ApplyAllSelectValuationMethodAndValueAggregation(gridRelationship, CommonClass.GBenMAPGrid, asvv));
                                    //lstAsyns.Add("a");
                                    //AsyncValuation dlgt = new AsyncValuation(APVX.APVCommonClass.ApplyAllSelectValuationMethodAndValueAggregation);
                                    //dlgt.BeginInvoke(gridRelationship, CommonClass.GBenMAPGrid, asvv, ref vb, new AsyncCallback(outPut),dlgt);
                                    //if (asvvnew != null)
                                    //{
                                    //    vb.LstAllSelectValuationMethodAndValueAggregation.Add(asvvnew);
                                    //}

                                }
                                vb.LstAllSelectValuationMethodAndValue = null;

                            }
                            if (vb.IncidencePoolingResult != null)
                            {
                               // vb.IncidencePoolingResultAggregation = APVX.APVCommonClass.ApplyCRSelectFunctionCalculateValueAggregation(gridRelationship, CommonClass.GBenMAPGrid, vb.IncidencePoolingResult);
                                //lstAsyns.Add("a");
                                //AsyncIncidence dlgt = new AsyncIncidence(APVX.APVCommonClass.ApplyCRSelectFunctionCalculateValueAggregation);
                                //dlgt.BeginInvoke(gridRelationship, CommonClass.GBenMAPGrid, vb.IncidencePoolingResult, ref vb, new AsyncCallback(outPut), dlgt);
                                //CRSelectFunctionCalculateValue asvvnew = new CRSelectFunctionCalculateValue();
                                ////asvvnew.AllSelectValuationMethod = vb.IncidencePoolingResult.AllSelectValuationMethod;
                                //asvvnew.CRCalculateValues = new List<CRCalculateValue>();

                                //asvvnew.ResultCopy = null;
                                //if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID != -1 && valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID != gBenMAPGrid.GridDefinitionID)
                                //{
                                //    asvvnew = APVX.APVCommonClass.ApplyCRSelectFunctionCalculateValueAggregation(gridRelationship, CommonClass.GBenMAPGrid, vb.IncidencePoolingResult);


                                //        if (asvvnew != null)
                                //        {
                                //            vb.IncidencePoolingResultAggregation = asvvnew;
                                //        }



                                //}
                            }


                        }
                    }
                    else
                    {

                    }
                }
                if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
                        valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation != null &&
                        valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID != gBenMAPGrid.GridDefinitionID && valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase != null)
                {
                    //----修正所有的QALY
                    idAggregation = valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID;
                    if (lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).Count() > 0)
                    {
                        gridRelationship = lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).First();
                    }
                    else
                    {
                        gridRelationship = lstGridRelationshipAll.Where(p => p.bigGridID == idAggregation && p.smallGridID == gBenMAPGrid.GridDefinitionID).First();

                    }
                    for (int ivb = 0; ivb < valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Count; ivb++)
                    //foreach (ValuationMethodPoolingAndAggregationBase vb in valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                    {
                        ValuationMethodPoolingAndAggregationBase vb = valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[ivb];
                        if (vb.lstAllSelectQALYMethodAndValue != null)
                        {
                            vb.lstAllSelectQALYMethodAndValueAggregation = new List<AllSelectQALYMethodAndValue>();

                            foreach (AllSelectQALYMethodAndValue asvv in vb.lstAllSelectQALYMethodAndValue)
                            {
                                vb.lstAllSelectQALYMethodAndValueAggregation.Add(APVX.APVCommonClass.ApplyAllSelectQALYMethodAndValueAggregation(gridRelationship, CommonClass.GBenMAPGrid, asvv));
                                //lstAsyns.Add("a");
                                //AsyncQALY dlgt = new AsyncQALY(APVX.APVCommonClass.ApplyAllSelectQALYMethodAndValueAggregation);
                                //dlgt.BeginInvoke(gridRelationship, CommonClass.GBenMAPGrid,asvv, ref vb, new AsyncCallback(outPut), dlgt);
                                //agregation
                                //AllSelectQALYMethodAndValue asvvnew = new AllSelectQALYMethodAndValue();
                                //asvvnew.AllSelectQALYMethod = asvv.AllSelectQALYMethod;
                                //asvvnew.lstQALYValueAttributes = new List<QALYValueAttribute>();

                                //asvvnew = APVX.APVCommonClass.ApplyAllSelectQALYMethodAndValueAggregation(gridRelationship, CommonClass.GBenMAPGrid, asvv);
                                //    if (asvvnew != null)
                                //    {
                                //        vb.lstAllSelectQALYMethodAndValueAggregation.Add(asvvnew);
                                //    }


                            }
                            vb.lstAllSelectQALYMethodAndValue = null;

                        }
                    }
                }
                //if (lstAsyns.Count == 0)
                //{
                //    WaitClose();
                //    //----------------------Save To File----------------------------
                //    if (_filePath != "")
                //    {
                //        //-------------save result------------------
                //        APVX.APVCommonClass.SaveAPVRFile(_filePath, CommonClass.ValuationMethodPoolingAndAggregation);
                //        MessageBox.Show("Save has been completed!");

                //    }
                //    this.DialogResult = System.Windows.Forms.DialogResult.OK;

                //}

            }
            catch
            { }
        }
        private List<string> lstAsyns = new List<string>();
        private void outPut(IAsyncResult ar)
        {
            try
            {
                //CRSelectFunctionCalculateValue crv = (ar.AsyncState as AsyncDelegateCalculateOneCRSelectFunction).EndInvoke(ar);
                //if (ar != null)
                //    lstCRSelectFunctionCalculateValue.Add(crv);
                //lstAsyns.Remove(crv.CRSelectFunction.BenMAPHealthImpactFunction.ID.ToString());
                lstAsyns.RemoveAt(0);
                if (lstAsyns.Count == 0)
                {
                    //CommonClass.BaseControlCRSelectFunctionCalculateValue = new BaseControlCRSelectFunctionCalculateValue();
                    //CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup = CommonClass.BaseControlCRSelectFunction.BaseControlGroup;
                    //CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue = lstCRSelectFunctionCalculateValue;
                    // str = str + ":" + DateTime.Now.ToString();

                    //WaitClose();
                    WaitClose();
                    //----------------------Save To File----------------------------
                    if (_filePath != "")
                    {
                        //-------------save result------------------
                        APVX.APVCommonClass.SaveAPVFile(_filePath, CommonClass.ValuationMethodPoolingAndAggregation);
                        MessageBox.Show("File saved.", "File saved");

                    }
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    //this.DialogResult = DialogResult.OK;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                // this.DialogResult = DialogResult.;
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
        //private int cbPoolingWindow_IndexOld = -1;
        private void cbPoolingWindow_SelectedIndexChanged(object sender, EventArgs e)
        {
            //----------



        }

        private void tabControlSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!bLoad) return;
            //if (cbPoolingWindow_IndexOld != -1)
            //{
            //    ValuationMethodPoolingAndAggregationBase vbold = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == tabControlSelection.TabPages[cbPoolingWindow_IndexOld].Text).First();
            //    vbold.LstAllSelectValuationMethod = lstAllSelectValuationMethod;


            //}
            //cbPoolingWindow_IndexOld = tabControlSelection.SelectedIndex;

            //--------------change
            ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Text).First();
            //if (vb.LstAllSelectValuationMethod == null)
            //{
            addColumnsToTree(vb.lstValuationColumns);
            initTreeView(vb);
            //}


            //List<AllSelectValuationMethod> lstRoot = new List<AllSelectValuationMethod>();
            //lstRoot.Add(vb.LstAllSelectValuationMethod.First());
            //for (int iRoot = 1; iRoot < vb.LstAllSelectValuationMethod.Count; iRoot++)
            //{
            //    if (vb.LstAllSelectValuationMethod[iRoot].EndPointGroup != vb.LstAllSelectValuationMethod[iRoot - 1].EndPointGroup)
            //        lstRoot.Add(vb.LstAllSelectValuationMethod[iRoot]);
            //}
            //treeListView.Roots = lstRoot;// lstAllSelectValuationMethod.GetRange(0, 1);
            //lstAllSelectValuationMethod = vb.LstAllSelectValuationMethod;
            ////this.treeListView.CanExpandGetter = delegate(object x)
            ////{
            ////    AllSelectValuationMethod dir = (AllSelectValuationMethod)x;
            ////    return (dir.NodeType != 5);
            ////};
            ////this.treeListView.ChildrenGetter = delegate(object x)
            ////{
            ////    AllSelectValuationMethod dir = (AllSelectValuationMethod)x;

            ////    try
            ////    {
            ////        return getChildFromAllSelectValuationMethod(dir);


            ////    }
            ////    catch (UnauthorizedAccessException ex)
            ////    {
            ////        MessageBox.Show(this, ex.Message, "ObjectListViewDemo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            ////        return new List<AllSelectValuationMethod>();
            ////        //return new ArrayList();
            ////    }
            ////};
            //lstAllSelectValuationMethod = vb.LstAllSelectValuationMethod;
            updateTreeView();
            treeListView.ExpandAll();
            tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Controls.Add(treeListView);

        }

        public void updateDrop(List<BenMAPValuationFunction> lstBenMAPValuationFunction, AllSelectValuationMethod Target,int iTargetIndex,bool isAbove)
        {
            try
            {
                ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Text).First();
                if (isAbove && vb.LstAllSelectValuationMethod.Count > 1)
                {
                    Target = treeListView.GetModelObject(iTargetIndex - 1) as AllSelectValuationMethod;
                }
                int maxid = vb.LstAllSelectValuationMethod.Max(p => p.ID) + 1;
                if (Target == null) return;
                if (Target.NodeType == 2000)
                {
                    //加在父节点上面
                    var query = vb.LstAllSelectValuationMethod.Where(p => p.ID == Target.PID);
                    if (query.Count() > 0)
                    {
                        AllSelectValuationMethod av5 = query.First();
                        maxid = vb.LstAllSelectValuationMethod.Max(p => p.ID) + 1;
                        //------------只能加同一个EndPointGroup的-------
                        foreach (BenMAPValuationFunction bmv in lstBenMAPValuationFunction)
                        {
                            //-------如果有则不加--------
                            //if (vb.LstAllSelectValuationMethod.Where(p => p.PID == av5.ID).Where(p => p.BenMAPValuationFunction.ID == bmv.ID).Count() == 0)
                            //{
                            if (bmv.EndPointGroup != av5.EndPointGroup) continue;
                            vb.LstAllSelectValuationMethod.Add(new AllSelectValuationMethod()
                            {
                                BenMAPValuationFunction = bmv,
                                ID = maxid,
                                PID = Target.PID,
                                Name = bmv.Qualifier + "|" + bmv.DistA + "|" + bmv.StartAge + "-" + bmv.EndAge,
                                Function = bmv.Function,
                                StartAge = av5.StartAge,// bmv.StartAge.ToString(),
                                EndAge = av5.EndAge,// bmv.EndAge.ToString(),
                                NodeType = 2000,
                                Qualifier = bmv.Qualifier,
                                Author = av5.Author,
                                EndPointGroup = bmv.EndPointGroup,
                                EndPoint = av5.EndPoint,
                                EndPointID = av5.EndPointID,
                                Version = av5.Version,
                                DataSet = av5.DataSet,
                                Ethnicity = av5.Ethnicity,
                                Gender = av5.Gender,
                                Location = av5.Location,
                                Race = av5.Race,
                                Year = av5.Year,
                                Metric = av5.Metric,
                                MetricStatistic = av5.MetricStatistic,
                                OtherPollutants = av5.OtherPollutants,
                                Pollutant = av5.Pollutant,
                                SeasonalMetric = av5.SeasonalMetric,

                            });
                            maxid++;
                            //}

                        }
                    }
                }
                else
                {
                    //-----------如果Target的字节点有不等于200--
                    var query = vb.LstAllSelectValuationMethod.Where(p => p.PID == Target.ID).ToList();
                    maxid = vb.LstAllSelectValuationMethod.Max(p => p.ID) + 1;
                    if (query.Count() == 0 || query.First().NodeType == 2000)
                    {
                        //加在子节点上面
                        foreach (BenMAPValuationFunction bmv in lstBenMAPValuationFunction)
                        {
                            //-------如果有则不加--------
                            if (bmv.EndPointGroup != Target.EndPointGroup) continue;
                            //if (vb.LstAllSelectValuationMethod.Where(p => p.PID == Target.ID).Where(p => p.BenMAPValuationFunction.ID == bmv.ID).Count() == 0)
                            //{
                                vb.LstAllSelectValuationMethod.Add(new AllSelectValuationMethod()
                                {
                                    BenMAPValuationFunction = bmv,
                                    ID = maxid,
                                    PID = Target.ID,
                                    Name = bmv.Qualifier + "|" + bmv.DistA + "|" + bmv.StartAge + "-" + bmv.EndAge,

                                    Function = bmv.Function,
                                    StartAge = Target.StartAge,// bmv.StartAge.ToString(),
                                    EndAge = Target.EndAge,//bmv.EndAge.ToString(),
                                    NodeType = 2000,
                                    Qualifier = bmv.Qualifier,
                                    Author = Target.Author,
                                    EndPointGroup = bmv.EndPointGroup,
                                    EndPoint = bmv.EndPoint,
                                    EndPointID = bmv.EndPointID,
                                    Version = Target.Version,
                                    DataSet = Target.DataSet,
                                    Ethnicity = Target.Ethnicity,
                                    Gender = Target.Gender,
                                    Location = Target.Location,
                                    Race = Target.Race,
                                    Year = Target.Year,
                                    Metric = Target.Metric,
                                    MetricStatistic = Target.MetricStatistic,
                                    OtherPollutants = Target.OtherPollutants,
                                    Pollutant = Target.Pollutant,
                                    SeasonalMetric = Target.SeasonalMetric,
                                });
                                maxid++;
                            //}

                        }
                    }
                    else
                    {
                        //---------判断在IncidencePooling中的PoolingMethod是否为None;为None 不加，否则加在子节点上面
                        //if (vb.LstAllSelectValuationMethod.Where(p => p.PID == Target.ID).Count() > 0 && vb.LstAllSelectValuationMethod.Where(p => p.PID == Target.ID).Select(a => a.NodeType).First() < 5)
                        //{ }
                        //else
                        //{
                        //    //加在子节点上面
                        //    foreach (BenMAPValuationFunction bmv in lstBenMAPValuationFunction)
                        //    {
                        //        //-------如果有则不加--------
                        //        if (vb.LstAllSelectValuationMethod.Where(p => p.PID == Target.ID).Where(p => p.BenMAPValuationFunction.ID == bmv.ID).Count() == 0)
                        //        {
                        //            vb.LstAllSelectValuationMethod.Add(new AllSelectValuationMethod()
                        //            {
                        //                BenMAPValuationFunction = bmv,
                        //                ID = maxid,
                        //                PID = Target.ID,
                        //                Name = bmv.ID.ToString(),

                        //                Function = bmv.Function,
                        //                StartAge = bmv.StartAge.ToString(),
                        //                EndAge = bmv.EndAge.ToString(),
                        //                NodeType = 5,
                        //                Qualifier = bmv.Qualifier
                        //            });
                        //            maxid++;
                        //        }

                        //    }

                        //}
                    }
                }
                updateTreeView();
            }
            catch
            { 
            }

        }

        private void btDelSelection_Click(object sender, EventArgs e)
        {
            //------------删除所选择的NodeType=5的所有值
            ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Text).First();

            foreach (AllSelectValuationMethod av in treeListView.SelectedObjects)
            {
                if (av.NodeType == 2000)
                {
                    vb.LstAllSelectValuationMethod.Remove(av);

                }
            }
            updateTreeView();
        }

        private void treeListView_ColumnReordered(object sender, ColumnReorderedEventArgs e)
        {
            try
            {//----------
                if (e.NewDisplayIndex == 0 || e.NewDisplayIndex == 1 || e.OldDisplayIndex == 0 || e.OldDisplayIndex == 1 ||
                    e.NewDisplayIndex == 2 || e.OldDisplayIndex == 2 || e.NewDisplayIndex == 3 || e.OldDisplayIndex == 3
                    ||e.NewDisplayIndex == 4 || e.NewDisplayIndex == 4 )
                {
                    e.Cancel = true;
                    return;
                }
                //------------------根据顺序生成一棵树------------------------
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
                lstOLVColumns = lstOLVColumns.Where(p => p.DisplayIndex != 0 && p.DisplayIndex != 1 && p.DisplayIndex != 2 && p.DisplayIndex != 3 && p.DisplayIndex != 4).ToList();


                //(treeListView.Columns[e.NewDisplayIndex] as OLVColumn).DisplayIndex = e.OldDisplayIndex;
                //(treeListView.Columns[e.OldDisplayIndex] as OLVColumn).DisplayIndex = e.NewDisplayIndex;
                ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == tabControlSelection.TabPages[tabControlSelection.SelectedIndex].Text).First();

                vb.lstValuationColumns = lstOLVColumns.Select(p => p.Text).ToList();
                //initTreeView(vb);
                //return;
                vb.LstAllSelectValuationMethod = new List<AllSelectValuationMethod>();
                //initTreeView(vb);
                //return;
                //---------------------得重新生成一棵树!-------------------------------------------
                //首先编一个List<CR>---------
                List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
                APVX.APVCommonClass.getAllChildCRNotNone(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First(), vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);
                lstCR.Insert(0, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First());
                List<CRSelectFunctionCalculateValue> lstCRValue = new List<CRSelectFunctionCalculateValue>();
                foreach (AllSelectCRFunction acr in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion)
                {
                    if (acr.CRSelectFunctionCalculateValue != null && acr.CRSelectFunctionCalculateValue.CRSelectFunction == null)
                    {
                        //CRSelectFunctionCalculateValue crv = new CRSelectFunctionCalculateValue();
                        //crv.CRCalculateValues = acr.CRSelectFunctionCalculateValue.CRCalculateValues;
                        //crv.CRSelectFunction = new CRSelectFunction()
                        //{
                        //     //CRID=9999+ acr.ID,
                        //    BenMAPHealthImpactFunction = new BenMAPHealthImpactFunction()
                        //    {
                        //        EndPoint = acr.EndPoint,
                        //        Author = acr.Author,
                        //        Qualifier = acr.Qualifier,
                        //        strLocations = acr.Location,
                        //        StartAge = acr.StartAge == "" ? 0 : Convert.ToInt32(acr.StartAge),
                        //        EndAge = acr.EndAge == "" ? 0 : Convert.ToInt32(acr.EndAge),
                        //        Year = acr.Year == "" ? 0 : Convert.ToInt32(acr.Year),
                        //        OtherPollutants = acr.OtherPollutants,
                        //        Race = acr.Race,
                        //        Ethnicity = acr.Ethnicity,
                        //        Gender = acr.Gender,
                        //        Function = acr.Function,
                        //        Pollutant = new BenMAPPollutant() { PollutantName = acr.Pollutant },
                        //        Metric = new Metric() { MetricName = acr.Metric },
                        //        SeasonalMetric = new SeasonalMetric() { SeasonalMetricName = acr.SeasonalMetric },
                        //        MetricStatistic = MetricStatic.Mean,
                        //        DataSetName = acr.DataSet



                        //    }
                        //};
                        //lstCRValue.Add(acr.CRSelectFunctionCalculateValue);

                    }
                    else
                    {
                        if (acr.CRSelectFunctionCalculateValue != null && (acr.PoolingMethod == "" || (acr.PoolingMethod != "None" && acr.PoolingMethod != "")))
                        {
                            //----------------如果它的父节点或者父节点的父节点 有acr.PoolingMethod != "None" && acr.PoolingMethod != "" 则不选择
                            bool isCanChoose = true;
                            if (acr.PID != -1)
                            {
                                var query = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.ID == acr.PID);
                                while (query != null && query.Count() > 0)
                                {
                                    AllSelectCRFunction acrParent = query.First();
                                    if (acrParent.PoolingMethod != "None" && acrParent.PoolingMethod != "")
                                    {
                                        isCanChoose = false;
                                        break;
                                    }
                                    query = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.ID == acrParent.PID);
                                }
                            }
                            if(isCanChoose)
                                lstCRValue.Add(acr.CRSelectFunctionCalculateValue);
                        }
                    }
                }
                //lstCRValue = lstCR.Where(p => p.CRSelectFunctionCalculateValue != null).Select(a => a.CRSelectFunctionCalculateValue).ToList();
                List<AllSelectCRFunction> lstAllSelectCRFunction = new List<AllSelectCRFunction>();//getLstAllSelectCRFunction(dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text], lstOLVColumns.Select(p => p.Text).ToList(), dicTabCR[tabControlSelected.TabPages[tabControlSelected.SelectedIndex].Text].First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup,-1);
                Dictionary<string, List<CRSelectFunctionCalculateValue>> dicEndPointGroupCR = new Dictionary<string, List<CRSelectFunctionCalculateValue>>();
                foreach (CRSelectFunctionCalculateValue cr in lstCRValue)
                {
                    if (dicEndPointGroupCR.ContainsKey(cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup))
                        dicEndPointGroupCR[cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup].Add(cr);
                    else
                    {
                        dicEndPointGroupCR.Add(cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup, new List<CRSelectFunctionCalculateValue>());
                        dicEndPointGroupCR[cr.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup].Add(cr);
                    }
                }
                foreach (KeyValuePair<string, List<CRSelectFunctionCalculateValue>> k in dicEndPointGroupCR)
                {
                    List<AllSelectCRFunction> lstTemp = IncidencePoolingandAggregation.getLstAllSelectCRFunction(k.Value, vb.lstValuationColumns, k.Key, -1);
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
               // List<AllSelectCRFunction> lstResult = IncidencePoolingandAggregation.getLstAllSelectCRFunction(lstCRValue, vb.lstValuationColumns, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().EndPointGroup,vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p=>p.NodeType!=100).Max(p=>p.NodeType),0);
                List<AllSelectValuationMethod> lstValuationMethod = new List<AllSelectValuationMethod>();
                foreach (AllSelectCRFunction allSelectCRFunction in lstAllSelectCRFunction)
                {
                    AllSelectValuationMethod alv = new AllSelectValuationMethod()
                    {
                        CRID = allSelectCRFunction.CRID,
                        ID = allSelectCRFunction.ID,
                        PID = allSelectCRFunction.PID,
                        NodeType = allSelectCRFunction.NodeType,
                        Name = allSelectCRFunction.Name,
                        PoolingMethod = "None",
                        //------add for display
                        EndPointGroup = allSelectCRFunction.EndPointGroup,
                        EndPoint = allSelectCRFunction.EndPoint,
                        Author = allSelectCRFunction.Author,
                        Qualifier = allSelectCRFunction.Qualifier,
                        Location = allSelectCRFunction.Location,
                        StartAge = allSelectCRFunction.StartAge,
                        EndAge = allSelectCRFunction.EndAge,
                        Year = allSelectCRFunction.Year,
                        OtherPollutants = allSelectCRFunction.OtherPollutants,
                        Race = allSelectCRFunction.Race,
                        Ethnicity = allSelectCRFunction.Ethnicity,
                        Gender = allSelectCRFunction.Gender,
                        Function = allSelectCRFunction.Function,
                        Pollutant = allSelectCRFunction.Pollutant,
                        Metric = allSelectCRFunction.Metric,
                        SeasonalMetric = allSelectCRFunction.SeasonalMetric,
                        MetricStatistic = allSelectCRFunction.MetricStatistic,
                        DataSet = allSelectCRFunction.DataSet,
                        Version=allSelectCRFunction.Version.ToString(),
                        //----- add for display
                    };
                    lstValuationMethod.Add(alv);
                }
                //----------如果vb已经有method则加入相应的------
                int iMax = lstValuationMethod.Max(p => p.ID) + 1;
                List<AllSelectValuationMethod> lstExistLeaf = vb.LstAllSelectValuationMethod.Where(p => p.NodeType == 2000).ToList();
                foreach (AllSelectValuationMethod avmExist in lstExistLeaf)
                {
                    //------首先找到他的父节点!------------得到和父节点一致的CR
                    var parent = vb.LstAllSelectValuationMethod.Where(p => p.ID == avmExist.PID).First();
                    avmExist.PID = lstValuationMethod.Where(p => p.CRID == parent.CRID).First().ID;
                    avmExist.ID = iMax;
                    lstValuationMethod.Add(avmExist);
                    iMax++;
                }
                vb.LstAllSelectValuationMethod = lstValuationMethod;
                //cbPoolingWindow_SelectedIndexOld = tabControlSelected.SelectedIndex;
                initTreeView(vb);
                updateTreeView();
                treeListView.ExpandAll();
            }
            catch (Exception)
            {
            }
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
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.StartAge.ToString() == lstParent[i - k - 1]).ToList();
                        break;
                    case "endage":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndAge.ToString() == lstParent[i - k - 1]).ToList();
                        break;
                    case "year":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Year.ToString() == lstParent[i - k - 1]).ToList();
                        break;
                    case "otherpollutants":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants == lstParent[i - k - 1]).ToList();
                        break;
                    case "race":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Race == lstParent[i - k - 1]).ToList();
                        break;
                    case "ethnicity":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Ethnicity == lstParent[i - k - 1]).ToList();
                        break;
                    case "gender":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Gender == lstParent[i - k - 1]).ToList();
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
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName == lstParent[i - k - 1]).ToList();
                        break;
                    case "metricstatistic":
                        lstSecond = lstSecond.Where(p => Enum.GetName(typeof(MetricStatic), p.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic) == lstParent[i - k - 1]).ToList();
                        break;
                    case "dataset":
                        lstSecond = lstSecond.Where(p => p.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName == lstParent[i - k - 1]).ToList();
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
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.StartAge.ToString()).Distinct().ToList();
                    break;
                case "endage":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.EndAge.ToString()).Distinct().ToList();
                    break;
                case "year":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Year.ToString()).Distinct().ToList();
                    break;
                case "otherpollutants":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants).Distinct().ToList();
                    break;
                case "race":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Race).Distinct().ToList();
                    break;
                case "ethnicity":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Ethnicity).Distinct().ToList();
                    break;
                case "gender":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.Gender).Distinct().ToList();
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
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName).Distinct().ToList();
                    break;
                case "metricstatistic":
                    lstString = lstCR.Select(p => Enum.GetName(typeof(MetricStatic), p.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic)).Distinct().ToList();
                    break;
                case "dataset":
                    lstString = lstCR.Select(p => p.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName).Distinct().ToList();
                    break;
            }

            return lstString;
        }
        public static List<AllSelectCRFunction> getLstAllSelectCRFunction(List<CRSelectFunctionCalculateValue> lstCR, List<string> lstOLVColumns, string EndPointGroup)
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
                        NodeType = 0,
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
                        Pollutant = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName.ToString(),
                        Metric = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName,
                        SeasonalMetric = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName,
                        MetricStatistic = Enum.GetName(typeof(MetricStatic), lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic),
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
                        NodeType = 0,
                        PID = -1,
                        PoolingMethod = "None",


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
                                        ID = j + 1,
                                        Name = lstString[j],//----------endpoint---
                                        NodeType = i + 1,
                                        PID = 0,
                                        PoolingMethod = "None",

                                    });
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
                                            if (lstString.Count > 1)
                                            {
                                                for (int k = 0; k < lstString.Count(); k++)
                                                {
                                                    lstReturn.Add(new AllSelectCRFunction()
                                                    {
                                                        EndPointGroupID = lstCR.First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
                                                        ID = lstReturn.Count(),
                                                        Name = lstString[k],//----------endpoint---
                                                        NodeType = i + 1,
                                                        PID = query[j].ID,
                                                        PoolingMethod = "None",

                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                        }
                        else
                        {
                            //退出循环！-------------加子节点------------

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
                        foreach (CRSelectFunctionCalculateValue crc in lstSecond)
                        {
                            string strAuthor = crc.CRSelectFunction.BenMAPHealthImpactFunction.Author;
                            if (strAuthor != null && strAuthor != "" && strAuthor.Contains(" "))
                            {
                                strAuthor = strAuthor.Substring(0, strAuthor.IndexOf(" "));
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
                                Version = "1",//------------------需要修改!------------------------

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


                }
                return lstReturn;
            }
            catch
            {
                return null;
            }
        }
        //-------------------majie------------------
        private void tabControlSelection_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                Font fntTab;
                Brush bshBack;
                Brush bshFore;
                if (e.Index == this.tabControlSelection.SelectedIndex)    //当前Tab页的样式
                {
                    fntTab = new Font(e.Font, FontStyle.Bold);
                    bshBack = new SolidBrush(Color.White);
                    //bshBack = Brushes.LemonChiffon;
                    bshFore = Brushes.Black;
                }
                else    //其余Tab页的样式
                {
                    fntTab = e.Font;
                    bshBack = new System.Drawing.Drawing2D.LinearGradientBrush(e.Bounds, SystemColors.Control, SystemColors.Control, System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal);
                    bshFore = new SolidBrush(Color.Black);
                }
                //画样式
                string tabName = this.tabControlSelection.TabPages[e.Index].Text;
                StringFormat sftTab = new StringFormat();
                e.Graphics.FillRectangle(bshBack, e.Bounds);
                Rectangle recTab = e.Bounds;
                recTab = new Rectangle(recTab.X, recTab.Y + 4, recTab.Width, recTab.Height - 4);
                e.Graphics.DrawString(tabName, fntTab, bshFore, recTab, sftTab);
            }

            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnShowChanges_Click(object sender, EventArgs e)
        {
            try
            {
                ChangedCRFunctions crfunctions = new ChangedCRFunctions();
                crfunctions.Show();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void treeListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
            {
                btDelSelection_Click(sender, e);
            }
        }
    }



    public class ValuationDropSink : SimpleDropSink
    {
        public SelectValuationMethods mySelectValuationMethods;
        /// <summary>
        /// Create a RearrangingDropSink
        /// </summary>
        public ValuationDropSink()
        {
            this.CanDropBetween = true;
            this.CanDropOnBackground = true;
            this.CanDropOnItem = false;
        }

        /// <summary>
        /// Create a RearrangingDropSink
        /// </summary>
        /// <param name="acceptDropsFromOtherLists"></param>
        public ValuationDropSink(bool acceptDropsFromOtherLists, SelectValuationMethods selectValuationMethods)
            : this()
        {
            this.AcceptExternal = acceptDropsFromOtherLists;
            mySelectValuationMethods = selectValuationMethods;
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
            int iTop = Convert.ToInt32(this.ListView.TopItemIndex.ToString());
            switch (args.DropTargetLocation)
            {
                case DropTargetLocation.AboveItem:
                    if (this.ListView.Name == "treeListView" && args.SourceListView.Name != "treeListView")
                    {
                        List<BenMAPValuationFunction> lst = new List<BenMAPValuationFunction>();

                        foreach (BenMAPValuationFunction bm in args.SourceModels)
                        {
                            lst.Add(bm);
                        }
                        
                        mySelectValuationMethods.updateDrop(lst, args.TargetModel as AllSelectValuationMethod,args.DropTargetIndex,true);
                        this.ListView.TopItemIndex = iTop;
                        //  mySelectValuationMethods.btAddCRFunctions_Click(null, null);
                        //this.tar
                    }
                    else
                    {
                        //   mySelectValuationMethods.btDelSelectMethod_Click(null, null);


                    }
                    break;
                case DropTargetLocation.BelowItem:
                    if (this.ListView.Name == "treeListView" && args.SourceListView.Name != "treeListView")
                    {
                        List<BenMAPValuationFunction> lst = new List<BenMAPValuationFunction>();
                        foreach (BenMAPValuationFunction bm in args.SourceModels)
                        {
                            lst.Add(bm);
                        }
                        mySelectValuationMethods.updateDrop(lst, args.TargetModel as AllSelectValuationMethod, args.DropTargetIndex, false);
                        //this.ListView.EnsureVisible(args.DropTargetIndex);
                        this.ListView.TopItemIndex = iTop;
                        //this.ListView.AddObjects(args.SourceModels);
                        // mySelectValuationMethods.btAddCRFunctions_Click(null, null);
                    }
                    else
                    {
                        //   mySelectValuationMethods.btDelSelectMethod_Click(null, null);

                    }
                    break;
                case DropTargetLocation.Background:
                    if (this.ListView.Name == "treeListView")
                    {
                        //base.
                        //this.ListView.AddObjects(args.SourceModels);
                        //   mySelectValuationMethods.btAddCRFunctions_Click(null, null);

                    }
                    else
                    {
                        //   mySelectValuationMethods.btDelSelectMethod_Click(null, null);

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
