using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using BenMAP.Tools;

namespace BenMAP
{
    public partial class HealthImpactFunctions : FormBase
    {
        private bool isload = false;
        //public bool isHIFChanged = false;
        ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
        private List<BenMAPHealthImpactFunction> lstBenMAPHealthImpactFunction = new List<BenMAPHealthImpactFunction>();
        private List<BenMAPHealthImpactFunction> lstBenMAPHealthImpactFunctionSelected = new List<BenMAPHealthImpactFunction>();
        private List<CRSelectFunction> lstCRSelectFunction = new List<CRSelectFunction>();
        private Dictionary<string, int> DicRace =new Dictionary<string,int>();// = CommonClass.DicAllRace;// Configuration.ConfigurationCommonClass.getAllRace();
        private Dictionary<string, int> DicGender =new Dictionary<string,int>();// = CommonClass.DicAllGender;// Configuration.ConfigurationCommonClass.getAllGender();
        private Dictionary<string, int> DicEthnicity = new Dictionary<string, int>();// = CommonClass.DicAllEthnicity;// Configuration.ConfigurationCommonClass.getAllEthnicity();
        private Dictionary<string, int> DicIncidenceDataSet = Configuration.ConfigurationCommonClass.getAllIncidenceDataSet(CommonClass.MainSetup.SetupID);
        private Dictionary<string, int> DicVariableDataSet = Configuration.ConfigurationCommonClass.getAllVariableDataSet(CommonClass.MainSetup.SetupID);
        private static int _maxCRID;

        public static int MaxCRID
        {
            get { return HealthImpactFunctions._maxCRID; }
            set { HealthImpactFunctions._maxCRID = value; }
        }
        /// <summary>
        /// 记录HealthImpactFunctions中新增或者修改CRFunction的ID
        /// 删除：del+";"+ID
        /// 新增：in+";"+ID
        /// </summary>
        private List<string> _lstUpdateCRFunction = new List<string>();
        public HealthImpactFunctions()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CommonClass.BaseControlCRSelectFunction = BaseControlCRSelectFunctionOld;
            this.DialogResult = DialogResult.Cancel;
        }
        public BaseControlCRSelectFunction BaseControlCRSelectFunctionOld;//所有BaseControlAndCRSelectFunciton
        private void HealthImpactFunctions_Load(object sender, EventArgs e)
        {
            //DataSetID
            try
            {
                CommonClass.Connection.Close();
            }
            catch
            {
            }
            CommonClass.Connection = CommonClass.getNewConnection();
            olvSelected.CheckBoxes = false;
            string commandText = string.Format("select CRFunctionDatasetID,SetupID,CRFunctionDatasetName,ReadOnly from CRFunctionDatasets where SetupID={0} ", CommonClass.MainSetup.SetupID);

            System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
            cbDataSet.DataSource = ds.Tables[0];
            cbDataSet.DisplayMember = "CRFunctionDatasetName";
            isload = true;
            if (ds.Tables[0].Rows.Count > 0)
            {
                cbDataSet.SelectedIndex = 0;

                cbDataSet_SelectedIndexChanged(sender, e);
            }
            //cbEndPointGroup_SelectedIndexChanged(sender, e);
            olvSimple.CheckBoxes = false;
            if (CommonClass.BaseControlCRSelectFunction != null && CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction != null)
            {
                BaseControlCRSelectFunctionOld = new BaseControlCRSelectFunction();
                BaseControlCRSelectFunctionOld.BaseControlGroup = CommonClass.BaseControlCRSelectFunction.BaseControlGroup;
                BaseControlCRSelectFunctionOld.BenMAPPopulation = CommonClass.BaseControlCRSelectFunction.BenMAPPopulation;
                BaseControlCRSelectFunctionOld.CRLatinHypercubePoints = CommonClass.BaseControlCRSelectFunction.CRLatinHypercubePoints;
                BaseControlCRSelectFunctionOld.CRRunInPointMode = CommonClass.BaseControlCRSelectFunction.CRRunInPointMode;
                BaseControlCRSelectFunctionOld.CRThreshold = CommonClass.BaseControlCRSelectFunction.CRThreshold;
                BaseControlCRSelectFunctionOld.RBenMapGrid = CommonClass.BaseControlCRSelectFunction.RBenMapGrid;
                BaseControlCRSelectFunctionOld.lstCRSelectFunction = new List<CRSelectFunction>();
                foreach (CRSelectFunction cr in CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction)
                {
                    BaseControlCRSelectFunctionOld.lstCRSelectFunction.Add(new CRSelectFunction()
                    {
                        BenMAPHealthImpactFunction = cr.BenMAPHealthImpactFunction,
                        CRID = cr.CRID,
                        EndAge = cr.EndAge,
                        Ethnicity = cr.Ethnicity,
                        Gender = cr.Gender,
                        IncidenceDataSetID = cr.IncidenceDataSetID,
                        IncidenceDataSetName = cr.IncidenceDataSetName,
                        Locations = cr.Locations,
                        PrevalenceDataSetID = cr.PrevalenceDataSetID,
                        PrevalenceDataSetName = cr.PrevalenceDataSetName,
                        Race = cr.Race,
                        StartAge = cr.StartAge,
                        VariableDataSetID = cr.VariableDataSetID,
                        VariableDataSetName = cr.VariableDataSetName
                    });

                }

                //List<int> lstInt = CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Select(p => p.BenMAPHealthImpactFunction.ID).ToList();
                this.olvSelected.Objects = CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction;
                gBSelectedHealthImpactFuntion.Text = "Selected Health Impact Functions (" + CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count + ")";
                lstCRSelectFunction = CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction;
            }
            olvSelected.IsSimpleDropSink = true;
            olvSimple.IsSimpleDropSink = true;
            olvSelected.DropSink = new HealthImapctDropSink(true, this);
            olvSimple.DropSink = new HealthImapctDropSink(true, this);
            olvSimple.Sort(1);
            // load ---EndPoint Group
            //race gender ethnicity 应该显示的是对应的population config里的
            commandText = string.Format("select RaceID,RaceName from Races where (raceid in (select raceid from Popconfigracemap where populationconfigurationid = {0})) or racename='' or lower(racename)='all'",CommonClass.BenMAPPopulation.PopulationConfiguration);
            ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DicRace.Add(dr["RaceName"].ToString(), Convert.ToInt32(dr["RaceID"]));
            }
            commandText = string.Format("select EthnicityID,EthnicityName from Ethnicity where (ethnicityid in (select ethnicityid from Popconfigethnicitymap where populationconfigurationid ={0})) or EthnicityName='' or lower(EthnicityName)='all'", CommonClass.BenMAPPopulation.PopulationConfiguration);
            ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DicEthnicity.Add(dr["EthnicityName"].ToString(), Convert.ToInt32(dr["EthnicityID"]));
            }
            commandText = string.Format("select GenderID,GenderName from Genders where (genderid in (select genderid from Popconfiggendermap where populationconfigurationid={0})) or GenderName='' or lower(GenderName)='all'", CommonClass.BenMAPPopulation.PopulationConfiguration);
            ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DicGender.Add(dr["GenderName"].ToString(), Convert.ToInt32(dr["GenderID"]));
            }
        }
        private void cbEndPointGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isload)
            {
                try
                {
                    DataRowView drv = cbEndPointGroup.SelectedItem as DataRowView;
                    DataRowView drvDataSet = this.cbDataSet.SelectedItem as DataRowView;

                    string commandText = "";
                    string strPollutants = "";
                    foreach (BenMAPPollutant benMAPPollutant in CommonClass.LstPollutant)
                    {
                        strPollutants = strPollutants + "," + benMAPPollutant.PollutantID;
                    }
                    strPollutants = strPollutants.Substring(1);
                    if (Convert.ToInt32(drv["EndPointGroupID"]) == -1)
                    {


                        //System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                        //cbEndPointGroup.DataSource = ds.Tables[0];
                        //cbEndPointGroup.DisplayMember = "EndPointGroupName";
                        commandText = string.Format(" select CRFunctionID from CRFunctions where CRFunctionDataSetID={0} and PollutantID in ({1})", drvDataSet["CRFunctionDatasetID"], strPollutants);
                        System.Data.DataSet dsCRFunction = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                        lstBenMAPHealthImpactFunction.Clear();
                        foreach (DataRow dr in dsCRFunction.Tables[0].Rows)
                        {
                            BenMAPHealthImpactFunction benMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(Convert.ToInt32(dr["CRFunctionID"]));
                            lstBenMAPHealthImpactFunction.Add(benMAPHealthImpactFunction);
                        }
                        this.olvSimple.SetObjects(lstBenMAPHealthImpactFunction);
                    }
                    else
                    {
                        commandText = string.Format(" select CRFunctionID from CRFunctions where CRFunctionDataSetID={0} and PollutantID in ({1}) and EndPointGroupID={2}", drvDataSet["CRFunctionDatasetID"], strPollutants, Convert.ToInt32(Convert.ToInt32(drv["EndPointGroupID"])));
                        System.Data.DataSet dsCRFunction = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                        lstBenMAPHealthImpactFunction.Clear();
                        foreach (DataRow dr in dsCRFunction.Tables[0].Rows)
                        {
                            BenMAPHealthImpactFunction benMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(Convert.ToInt32(dr["CRFunctionID"]));
                            lstBenMAPHealthImpactFunction.Add(benMAPHealthImpactFunction);
                        }
                        this.olvSimple.SetObjects(lstBenMAPHealthImpactFunction);

                    }
                }
                catch (Exception ex)
                {

                }

            }

        }
        private void cbDataSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isload)
            {
                try
                {
                    DataRowView drv = cbDataSet.SelectedItem as DataRowView;
                    string commandText = string.Format("select -1 as EndPointGroupID,'All' as EndPointGroupName from EndPointGroups union select distinct a.EndPointGroupID,b.EndPointGroupName from CRFunctions a,EndPointGroups b where CRFunctionDataSetID={0} and a.EndPointGroupID=b.EndPointGroupID  ", drv["CRFunctionDatasetID"]);
                    string strPollutants = "";
                    foreach (BenMAPPollutant benMAPPollutant in CommonClass.LstPollutant)
                    {
                        strPollutants = strPollutants + "," + benMAPPollutant.PollutantID;
                    }
                    strPollutants = strPollutants.Substring(1);

                    System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    cbEndPointGroup.DataSource = ds.Tables[0];
                    cbEndPointGroup.DisplayMember = "EndPointGroupName";
                    //commandText = string.Format(" select CRFunctionID from CRFunctions where CRFunctionDataSetID={0} and PollutantID in ({1})", drv["CRFunctionDatasetID"], strPollutants);
                    //System.Data.DataSet dsCRFunction = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    //lstBenMAPHealthImpactFunction.Clear();
                    //foreach (DataRow dr in dsCRFunction.Tables[0].Rows)
                    //{
                    //    BenMAPHealthImpactFunction benMAPHealthImpactFunction = Configuration.ConfigurationCommonClass.getBenMAPHealthImpactFunctionFromID(Convert.ToInt32(dr["CRFunctionID"]));
                    //    lstBenMAPHealthImpactFunction.Add(benMAPHealthImpactFunction);
                    //}
                    //this.olvSimple.SetObjects(lstBenMAPHealthImpactFunction);
                }
                catch (Exception ex)
                {

                }

            }

        }

        private void cbGroups_CheckedChanged(object sender, EventArgs e)
        {
            ShowGroupsChecked(this.olvSimple, (CheckBox)sender);
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
        
        // Todo:陈志润20120115新增
        public void btAddCRFunctions_Click(object sender, EventArgs e)
        {
            try
            {

                // List<BenMAPHealthImpactFunction> lstBenMAPHealthImpactFunction = olvSimple.SelectedObjects as List<BenMAPHealthImpactFunction>;
                foreach (BenMAPHealthImpactFunction benMAPHealthImpactFunction in olvSimple.SelectedObjects)
                {
                    //isHIFChanged = true;
                    //------modify by xiejp 支持多个相同的CR
                    //if (isInlstBenMAPHealthImpactFunctionSelected(benMAPHealthImpactFunction))
                    //{
                    //}
                    //else
                    // lstBenMAPHealthImpactFunctionSelected.Add(benMAPHealthImpactFunction);
                    CRSelectFunction crSelectFunction = new CRSelectFunction();
                    crSelectFunction.BenMAPHealthImpactFunction = benMAPHealthImpactFunction;
                    string commandText = "";
                    //---------------判断function直接default----
                    DataSet ds = null;
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    if (crSelectFunction.BenMAPHealthImpactFunction.Function.ToLower().Contains("incidence") || crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction.ToLower().Contains("incidence"))
                    {
                        //default incidence
                        commandText = string.Format("select distinct a.IncidenceDataSetID,IncidenceDataSetName from IncidenceDataSets a,IncidenceRates b where a.IncidenceDataSetID=b.IncidenceDataSetID and  SetupID={0} and b.EndPointGroupID={1} and Prevalence='F' and (b.EndPointID={2} or b.EndPointID=99 or b.EndPointID=100 or b.EndPointID=102)", CommonClass.MainSetup.SetupID, crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID, crSelectFunction.BenMAPHealthImpactFunction.EndPointID);
                        ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                        //---------获得最近的IncidenceDataSet------------
                        int drNextYear,drYear=0;
                        try
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                if (string.IsNullOrEmpty(crSelectFunction.IncidenceDataSetName))
                                {
                                    crSelectFunction.IncidenceDataSetID = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                                    crSelectFunction.IncidenceDataSetName = ds.Tables[0].Rows[0][1].ToString();
                                    drYear = int.Parse(dr["IncidenceDataSetName"].ToString().Substring(dr["IncidenceDataSetName"].ToString().IndexOf("(") + 1, dr["IncidenceDataSetName"].ToString().IndexOf(")") - dr["IncidenceDataSetName"].ToString().IndexOf("(") - 1));

                                }
                                else
                                {
                                    //--得到年份 规则在()内
                                    if (dr["IncidenceDataSetName"].ToString().Contains("(") && dr["IncidenceDataSetName"].ToString().Contains(")"))
                                    {
                                        drNextYear = int.Parse(dr["IncidenceDataSetName"].ToString().Substring(dr["IncidenceDataSetName"].ToString().IndexOf("(") + 1, dr["IncidenceDataSetName"].ToString().IndexOf(")") - dr["IncidenceDataSetName"].ToString().IndexOf("(") - 1));
                                        if (drNextYear > 0 && Math.Abs(drNextYear - CommonClass.BenMAPPopulation.Year) < Math.Abs(drYear - CommonClass.BenMAPPopulation.Year))
                                        {
                                            drYear = drNextYear;
                                            crSelectFunction.IncidenceDataSetID = Convert.ToInt32(dr[0]);
                                            crSelectFunction.IncidenceDataSetName = dr[1].ToString();

                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            crSelectFunction.IncidenceDataSetID = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                            crSelectFunction.IncidenceDataSetName = ds.Tables[0].Rows[0][1].ToString();
                        }
                    }
                    if (crSelectFunction.BenMAPHealthImpactFunction.Function.ToLower().Contains("prevalence") || crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction.ToLower().Contains("prevalence"))
                    {
                        commandText = string.Format("select distinct a.IncidenceDataSetID,IncidenceDataSetName from IncidenceDataSets a,IncidenceRates b where a.IncidenceDataSetID=b.IncidenceDataSetID and  SetupID={0} and b.EndPointGroupID={1} and Prevalence='T'", CommonClass.MainSetup.SetupID, crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID);
                        // ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                        ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            crSelectFunction.PrevalenceDataSetID = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                            crSelectFunction.PrevalenceDataSetName = ds.Tables[0].Rows[0][1].ToString();
                        }
                    }

                    string DatabaseFunction = crSelectFunction.BenMAPHealthImpactFunction.Function.Replace("prevalence", "").Replace("incidence", "").Replace("deltaq", "")
                    .Replace("pop", "").Replace("beta", "").Replace("q0", "").Replace("q1", "").Replace("allgoodsindex", "").Replace("medicalcostindex", "").Replace("wageindex", "")
                   .Replace("abs", " ")
     .Replace("acos", " ")
     .Replace("asin", " ")
     .Replace("atan", " ")
     .Replace("atan2", " ")
     .Replace("bigmul", " ")
     .Replace("ceiling", " ")
     .Replace("cos", " ")
     .Replace("cosh", " ")
     .Replace("divrem", " ")
     .Replace("exp", " ")
     .Replace("floor", " ")
     .Replace("ieeeremainder", " ")
     .Replace("log", " ")
     .Replace("log10", " ")
     .Replace("max", " ")
     .Replace("min", " ")
     .Replace("pow", " ")
     .Replace("round", " ")
     .Replace("sign", " ")
     .Replace("sin", " ")
     .Replace("sinh", " ")
     .Replace("sqrt", " ")
     .Replace("tan", " ")
     .Replace("tanh", " ")
     .Replace("truncate", " ").ToLower();
                    bool inLst = false;
                    foreach (string s in lstVariable)
                    {
                        if (DatabaseFunction.Contains(s.ToLower()))
                        {
                            inLst = true;
                        }
                    }
                    DatabaseFunction = crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction.Replace("prevalence", "").Replace("incidence", "").Replace("deltaq", "")
                .Replace("pop", "").Replace("beta", "").Replace("q0", "").Replace("q1", "").Replace("allgoodsindex", "").Replace("medicalcostindex", "").Replace("wageindex", "")
               .Replace("abs", " ")
 .Replace("acos", " ")
 .Replace("asin", " ")
 .Replace("atan", " ")
 .Replace("atan2", " ")
 .Replace("bigmul", " ")
 .Replace("ceiling", " ")
 .Replace("cos", " ")
 .Replace("cosh", " ")
 .Replace("divrem", " ")
 .Replace("exp", " ")
 .Replace("floor", " ")
 .Replace("ieeeremainder", " ")
 .Replace("log", " ")
 .Replace("log10", " ")
 .Replace("max", " ")
 .Replace("min", " ")
 .Replace("pow", " ")
 .Replace("round", " ")
 .Replace("sign", " ")
 .Replace("sin", " ")
 .Replace("sinh", " ")
 .Replace("sqrt", " ")
 .Replace("tan", " ")
 .Replace("tanh", " ")
 .Replace("truncate", " ").ToLower();
                    foreach (string s in lstVariable)
                    {
                        if (DatabaseFunction.Contains(s.ToLower()))
                        {
                            inLst = true;
                        }
                    }
                    if (inLst)
                    {
                        crSelectFunction.VariableDataSetID = DicVariableDataSet.First().Value;
                        crSelectFunction.VariableDataSetName = DicVariableDataSet.First().Key;
                    }
                    crSelectFunction.StartAge = benMAPHealthImpactFunction.StartAge;
                    crSelectFunction.EndAge = benMAPHealthImpactFunction.EndAge;
                    crSelectFunction.Locations = benMAPHealthImpactFunction.Locations;
                    //----------修正，如果不包含则选择最合适的一个
                    if (DicRace.ContainsKey(benMAPHealthImpactFunction.Race))
                        crSelectFunction.Race = benMAPHealthImpactFunction.Race;
                    else
                    {
                        foreach (string s in DicRace.Keys.ToList())
                        {
                            if (s.ToLower() == benMAPHealthImpactFunction.Race.ToLower())
                                crSelectFunction.Race = s;
                        }
                    }
                    if (DicGender.ContainsKey(benMAPHealthImpactFunction.Gender))
                        crSelectFunction.Gender = benMAPHealthImpactFunction.Gender;
                    else
                    {
                        foreach (string s in DicGender.Keys.ToList())
                        {
                            if (s.ToLower() == benMAPHealthImpactFunction.Gender.ToLower())
                                crSelectFunction.Gender = s;
                        }
                    }
                    if (DicEthnicity.ContainsKey(benMAPHealthImpactFunction.Ethnicity))
                        crSelectFunction.Ethnicity = benMAPHealthImpactFunction.Ethnicity;
                    else
                    {
                        foreach (string s in DicEthnicity.Keys.ToList())
                        {
                            if (s.ToLower() == benMAPHealthImpactFunction.Ethnicity.ToLower())
                                crSelectFunction.Ethnicity = s;
                        }
                    }
                    
                    crSelectFunction.CRID = 1;
                    //if(_maxCRID
                    if (_maxCRID > 0)
                    {
                        crSelectFunction.CRID = _maxCRID + 1;// lstCRSelectFunction.Max(p => p.CRID) + 1;
                        _maxCRID = _maxCRID + 1;
                    }
                    else
                    {
                        _maxCRID = 1;
                    }
                    lstCRSelectFunction.Add(crSelectFunction);

                    if (CommonClass.lstIncidencePoolingAndAggregation != null && CommonClass.lstIncidencePoolingAndAggregation.Count > 0)
                    {

                        if (CommonClass.LstUpdateCRFunction == null || CommonClass.LstUpdateCRFunction.Count == 0)
                        {
                            CommonClass.LstUpdateCRFunction = new List<CRSelectFunction>();
                            CommonClass.LstUpdateCRFunction.Add(crSelectFunction);
                        }
                        else
                        {
                            foreach (CRSelectFunction crFunction in CommonClass.LstUpdateCRFunction)
                            {
                                if ((crSelectFunction.CRID == crFunction.CRID) && (crSelectFunction.BenMAPHealthImpactFunction.ID == crFunction.BenMAPHealthImpactFunction.ID)) { continue; }
                                CommonClass.LstUpdateCRFunction.Add(crSelectFunction);
                                break;
                            }
                        }
                    }
                }
                olvSelected.SetObjects(lstCRSelectFunction);
                gBSelectedHealthImpactFuntion.Text = "Selected Health Impact Functions (" + lstCRSelectFunction.Count + ")";
                olvSelected.CheckBoxes = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

        }


        public bool isInlstBenMAPHealthImpactFunctionSelected(BenMAPHealthImpactFunction benMAPHealthImpactFunction)
        {
            foreach (CRSelectFunction b in lstCRSelectFunction)
            {
                if (b.BenMAPHealthImpactFunction.ID == benMAPHealthImpactFunction.ID) return true;
            }
            return false;
        }

        private void olvSelected_CellEditStarting(object sender, CellEditEventArgs e)
        { // Todo:陈志润
            base.OnClick(e);
            if (e.Column == null || (lstAsyns != null && lstAsyns.Count > 0))
            {
                e.Cancel = true;
                return; }
            string commandText = "";
            DataSet ds = null;
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            if (e.Column.Text == "Race")
            {

                ComboBox cb = new ComboBox();
                cb.Bounds = e.CellBounds;
                cb.Font = ((ObjectListView)sender).Font;
                cb.DropDownStyle = ComboBoxStyle.DropDownList;
                cb.Items.AddRange(DicRace.Keys.ToArray());
                if (e.Value != null)
                    cb.SelectedText = e.Value.ToString();// Math.Max(0, Math.Min(cb.Items.Count - 1, ((int)e.Value) / 10));
                cb.SelectedIndexChanged += new EventHandler(cbRace_SelectedIndexChanged);
                cb.Tag = e.RowObject; // remember which person we are editing
                e.Control = cb;
            }
            else if (e.Column.Text == "Gender")
            {

                ComboBox cb = new ComboBox();
                cb.Bounds = e.CellBounds;
                cb.Font = ((ObjectListView)sender).Font;
                cb.DropDownStyle = ComboBoxStyle.DropDownList;
                cb.Items.AddRange(DicGender.Keys.ToArray());
                if (e.Value != null)
                    cb.SelectedText = e.Value.ToString();// Math.Max(0, Math.Min(cb.Items.Count - 1, ((int)e.Value) / 10));
                cb.SelectedIndexChanged += new EventHandler(cbGender_SelectedIndexChanged);
                cb.Tag = e.RowObject; // remember which person we are editing
                e.Control = cb;
            }
            else if (e.Column.Text == "Ethnicity")
            {

                ComboBox cb = new ComboBox();
                cb.Bounds = e.CellBounds;
                cb.Font = ((ObjectListView)sender).Font;
                cb.DropDownStyle = ComboBoxStyle.DropDownList;
                cb.Items.AddRange(DicEthnicity.Keys.ToArray());
                if (e.Value != null)
                    cb.SelectedText = e.Value.ToString();// Math.Max(0, Math.Min(cb.Items.Count - 1, ((int)e.Value) / 10));
                cb.SelectedIndexChanged += new EventHandler(cbEthnicity_SelectedIndexChanged);
                cb.Tag = e.RowObject; // remember which person we are editing
                e.Control = cb;
            }
            else if (e.Column.Text == "Incidence Dataset")
            {
                //判断是否需要Incidence--------------------------
                CRSelectFunction crSelectFunction = e.RowObject as CRSelectFunction;
                if (crSelectFunction.BenMAPHealthImpactFunction.Function.ToLower().Contains("incidence") || crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction.ToLower().Contains("incidence"))
                {


                    ComboBox cb = new ComboBox();
                    cb.Bounds = e.CellBounds;
                    cb.Font = ((ObjectListView)sender).Font;
                    cb.DropDownStyle = ComboBoxStyle.DropDownList;

                    //default incidence
                    commandText = string.Format("select distinct a.IncidenceDataSetID,IncidenceDataSetName from IncidenceDataSets a,IncidenceRates b where a.IncidenceDataSetID=b.IncidenceDataSetID and  SetupID={0} and b.EndPointGroupID={1} and Prevalence='F' and (b.EndPointID={2} or b.EndPointID=99 or b.EndPointID=100 or b.EndPointID=102)", CommonClass.MainSetup.SetupID, crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID, crSelectFunction.BenMAPHealthImpactFunction.EndPointID);
                    ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    cb.DataSource = ds.Tables[0];
                    cb.DisplayMember = "IncidenceDataSetName";


                    //cb.Items.AddRange(DicIncidenceDataSet.Keys.ToArray());
                    if (e.Value != null)
                        cb.SelectedText = e.Value.ToString();// Math.Max(0, Math.Min(cb.Items.Count - 1, ((int)e.Value) / 10));
                    cb.SelectedIndexChanged += new EventHandler(cbIncidenceDataSet_SelectedIndexChanged);
                    cb.Tag = e.RowObject; // remember which person we are editing
                    e.Control = cb;
                }
                else
                {
                    e.Cancel = true;
                }

            }
            else if (e.Column.Text == "Prevalence Dataset")
            {
                //判断是否需要Prevalence--------------------------
                CRSelectFunction cr = e.RowObject as CRSelectFunction;
                if (cr.BenMAPHealthImpactFunction.Function.ToLower().Contains("prevalence") || cr.BenMAPHealthImpactFunction.BaseLineIncidenceFunction.ToLower().Contains("prevalence"))
                {
                    ComboBox cb = new ComboBox();
                    cb.Bounds = e.CellBounds;
                    cb.Font = ((ObjectListView)sender).Font;
                    cb.DropDownStyle = ComboBoxStyle.DropDownList;
                    commandText = string.Format("select distinct a.IncidenceDataSetID,IncidenceDataSetName from IncidenceDataSets a,IncidenceRates b where a.IncidenceDataSetID=b.IncidenceDataSetID and  SetupID={0} and b.EndPointGroupID={1} and Prevalence='T'", CommonClass.MainSetup.SetupID, cr.BenMAPHealthImpactFunction.EndPointGroupID);
                    ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    cb.DataSource = ds.Tables[0];
                    cb.DisplayMember = "IncidenceDataSetName";
                    //cb.Items.AddRange(DicIncidenceDataSet.Keys.ToArray());
                    if (e.Value != null)
                        cb.SelectedText = e.Value.ToString();// Math.Max(0, Math.Min(cb.Items.Count - 1, ((int)e.Value) / 10));
                    cb.SelectedIndexChanged += new EventHandler(cbPrevalenceDataSet_SelectedIndexChanged);
                    cb.Tag = e.RowObject; // remember which person we are editing
                    e.Control = cb;
                }
                else
                {
                    e.Cancel = true;
                }

            }
            else if (e.Column.Text == "Variable Dataset")
            {
                CRSelectFunction cr = e.RowObject as CRSelectFunction;
                string DatabaseFunction = cr.BenMAPHealthImpactFunction.Function.Replace("prevalence", "").Replace("incidence", "").Replace("deltaq", "")
                 .Replace("pop", "").Replace("beta", "").Replace("q0", "").Replace("q1", "").Replace("allgoodsindex", "").Replace("medicalcostindex", "").Replace("wageindex", "")
                .Replace("abs", " ")
  .Replace("acos", " ")
  .Replace("asin", " ")
  .Replace("atan", " ")
  .Replace("atan2", " ")
  .Replace("bigmul", " ")
  .Replace("ceiling", " ")
  .Replace("cos", " ")
  .Replace("cosh", " ")
  .Replace("divrem", " ")
  .Replace("exp", " ")
  .Replace("floor", " ")
  .Replace("ieeeremainder", " ")
  .Replace("log", " ")
  .Replace("log10", " ")
  .Replace("max", " ")
  .Replace("min", " ")
  .Replace("pow", " ")
  .Replace("round", " ")
  .Replace("sign", " ")
  .Replace("sin", " ")
  .Replace("sinh", " ")
  .Replace("sqrt", " ")
  .Replace("tan", " ")
  .Replace("tanh", " ")
  .Replace("truncate", " ").ToLower();
                bool inLst = false;
                foreach (string s in lstVariable)
                {
                    if (DatabaseFunction.Contains(s.ToLower()))
                    {
                        inLst = true;
                    }
                }
                DatabaseFunction = cr.BenMAPHealthImpactFunction.BaseLineIncidenceFunction.Replace("prevalence", "").Replace("incidence", "").Replace("deltaq", "")
                .Replace("pop", "").Replace("beta", "").Replace("q0", "").Replace("q1", "").Replace("allgoodsindex", "").Replace("medicalcostindex", "").Replace("wageindex", "")
               .Replace("abs", " ")
 .Replace("acos", " ")
 .Replace("asin", " ")
 .Replace("atan", " ")
 .Replace("atan2", " ")
 .Replace("bigmul", " ")
 .Replace("ceiling", " ")
 .Replace("cos", " ")
 .Replace("cosh", " ")
 .Replace("divrem", " ")
 .Replace("exp", " ")
 .Replace("floor", " ")
 .Replace("ieeeremainder", " ")
 .Replace("log", " ")
 .Replace("log10", " ")
 .Replace("max", " ")
 .Replace("min", " ")
 .Replace("pow", " ")
 .Replace("round", " ")
 .Replace("sign", " ")
 .Replace("sin", " ")
 .Replace("sinh", " ")
 .Replace("sqrt", " ")
 .Replace("tan", " ")
 .Replace("tanh", " ")
 .Replace("truncate", " ").ToLower();
                // bool inLst = false;
                foreach (string s in lstVariable)
                {
                    if (DatabaseFunction.Contains(s.ToLower()))
                    {
                        inLst = true;
                    }
                }
                if (inLst)
                {
                    //判断是否需要Variable--------------------------
                    ComboBox cb = new ComboBox();
                    cb.Bounds = e.CellBounds;
                    cb.Font = ((ObjectListView)sender).Font;
                    cb.DropDownStyle = ComboBoxStyle.DropDownList;
                    cb.Items.AddRange(this.DicVariableDataSet.Keys.ToArray());
                    if (e.Value != null)
                        cb.SelectedText = e.Value.ToString();// Math.Max(0, Math.Min(cb.Items.Count - 1, ((int)e.Value) / 10));
                    cb.SelectedIndexChanged += new EventHandler(cbVariableDataSet_SelectedIndexChanged);
                    cb.Tag = e.RowObject; // remember which person we are editing
                    e.Control = cb;

                }
                else
                {
                    e.Cancel = true;
                }

            }
            else if (e.Column.Text == "Start Age")
            {
                TextBox txt = new TextBox();
                txt.Bounds = e.CellBounds;
                txt.Font = ((ObjectListView)sender).Font;
                if (e.Value != null)
                {
                    txt.Text = e.Value.ToString();
                    txt.TextChanged += new EventHandler(txt_TextChanged_StartAge);
                    txt.Tag = e.RowObject;
                    e.Control = txt;
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else if (e.Column.Text == "End Age")
            {
                TextBox txt = new TextBox();
                txt.Bounds = e.CellBounds;
                txt.Font = ((ObjectListView)sender).Font;
                if (e.Value != null)
                {
                    txt.Text = e.Value.ToString();
                    txt.TextChanged += new EventHandler(txt_TextChanged_EndAge);
                    txt.Tag = e.RowObject;
                    e.Control = txt;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        void txt_TextChanged_StartAge(object sender, EventArgs e)
        {
            try
            {
                TextBox txt = (TextBox)sender;

                int flag = 0;
                foreach (char c in txt.Text)
                {
                    if (!char.IsNumber(c))
                    {
                        MessageBox.Show("Please input a number.");
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    if (Convert.ToInt32(txt.Text) < 0 || Convert.ToInt32(txt.Text) > 99)
                    {
                        MessageBox.Show("The Age range is 0-99."); txt.Text = ((CRSelectFunction)txt.Tag).StartAge.ToString();
                    }
                }
                //if (((CRSelectFunction)txt.Tag).EndAge < Convert.ToInt32(txt.Text))
                //{
                //    MessageBox.Show("End Age must be more than or equal to Start Age."); txt.Text = ((CRSelectFunction)txt.Tag).StartAge.ToString();
 
                //}
               // ((CRSelectFunction)txt.Tag).StartAge = Convert.ToInt32(txt.Text);
            }
            catch (Exception ex)
            {
                TextBox txt = (TextBox)sender;

                Logger.LogError(ex);
            }
        }

        void txt_TextChanged_EndAge(object sender, EventArgs e)
        {
            try
            {
                TextBox txt = (TextBox)sender;

                int flag = 0;
                foreach (char c in txt.Text)
                {
                    if (!char.IsNumber(c))
                    {
                        MessageBox.Show("Please input a number.");
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    if (Convert.ToInt32(txt.Text) < 0 || Convert.ToInt32(txt.Text) > 99)
                    {
                        MessageBox.Show("The Age range is 0-99."); txt.Text = ((CRSelectFunction)txt.Tag).EndAge.ToString();
                    }
                }
                //if (((CRSelectFunction)txt.Tag).StartAge > Convert.ToInt32(txt.Text))
                //{
                //    MessageBox.Show("End Age must be more than or equal to Start Age."); txt.Text = ((CRSelectFunction)txt.Tag).StartAge.ToString();

                //}
               // ((CRSelectFunction)txt.Tag).EndAge = Convert.ToInt32(txt.Text);
            }
            catch (Exception ex)
            {
                TextBox txt = (TextBox)sender;

                Logger.LogError(ex);
            }
        }

        void cbVariableDataSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            //isHIFChanged = true;
            ComboBox cb = (ComboBox)sender;
            ((CRSelectFunction)cb.Tag).VariableDataSetName = cb.Text;
            ((CRSelectFunction)cb.Tag).VariableDataSetID = DicVariableDataSet[cb.Text];
        }

        void cbPrevalenceDataSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            // isHIFChanged = true;
            ComboBox cb = (ComboBox)sender;
            ((CRSelectFunction)cb.Tag).PrevalenceDataSetName = cb.Text;
            ((CRSelectFunction)cb.Tag).PrevalenceDataSetID = DicIncidenceDataSet[cb.Text];
        }

        void cbIncidenceDataSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            // isHIFChanged = true;
            ComboBox cb = (ComboBox)sender;
            ((CRSelectFunction)cb.Tag).IncidenceDataSetName = cb.Text;
            ((CRSelectFunction)cb.Tag).IncidenceDataSetID = DicIncidenceDataSet[cb.Text];
        }
        void cbRace_SelectedIndexChanged(object sender, EventArgs e)
        {
            //isHIFChanged = true;
            ComboBox cb = (ComboBox)sender;
            ((CRSelectFunction)cb.Tag).Race = cb.Text;
        }
        void cbGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            //isHIFChanged = true;
            ComboBox cb = (ComboBox)sender;
            ((CRSelectFunction)cb.Tag).Gender = cb.Text;
        }
        void cbEthnicity_SelectedIndexChanged(object sender, EventArgs e)
        { // Todo:陈志润
            // isHIFChanged = true;
            ComboBox cb = (ComboBox)sender;
            ((CRSelectFunction)cb.Tag).Ethnicity = cb.Text;
        }

        private void olvSelected_CellEditFinishing(object sender, CellEditEventArgs e)
        {
            try
            {
                if (e.Column.Text == "Start Age")
                {
                    TextBox txt = (TextBox)e.Control;
                    ((TextBox)e.Control).TextChanged -= new EventHandler(txt_TextChanged_StartAge);

                    if (((CRSelectFunction)txt.Tag).EndAge < Convert.ToInt32(txt.Text))
                    {
                        MessageBox.Show("End age must be more than or equal to start age."); //txt.Text = ((CRSelectFunction)txt.Tag).StartAge.ToString();                    
                        e.Cancel = true;
                    }
                    else
                        ((CRSelectFunction)txt.Tag).StartAge = Convert.ToInt32(txt.Text);
                    ((ObjectListView)sender).RefreshItem(e.ListViewItem);

                    e.Cancel = true;
                }
                else if (e.Column.Text == "End Age")
                {
                    TextBox txt = (TextBox)e.Control;
                    ((TextBox)e.Control).TextChanged -= new EventHandler(txt_TextChanged_EndAge);

                    if (((CRSelectFunction)txt.Tag).StartAge > Convert.ToInt32(txt.Text))
                    {
                        MessageBox.Show("End age must be more than or equal to start age."); //txt.Text = ((CRSelectFunction)txt.Tag).StartAge.ToString();                    
                        e.Cancel = true;
                    }
                    else
                        ((CRSelectFunction)txt.Tag).EndAge = Convert.ToInt32(txt.Text);
                    ((ObjectListView)sender).RefreshItem(e.ListViewItem);
                    e.Cancel = true;
                }
                else if (e.Column.Text == "Race")
                {

                    // Stop listening for change events
                    ((ComboBox)e.Control).SelectedIndexChanged -= new EventHandler(cbRace_SelectedIndexChanged);

                    // Any updating will have been down in the SelectedIndexChanged event handler
                    // Here we simply make the list redraw the involved ListViewItem
                    ((ObjectListView)sender).RefreshItem(e.ListViewItem);

                    // We have updated the model object, so we cancel the auto update
                    e.Cancel = true;
                }
                else if (e.Column.Text == "Gender")
                {

                    // Stop listening for change events
                    ((ComboBox)e.Control).SelectedIndexChanged -= new EventHandler(cbGender_SelectedIndexChanged);

                    // Any updating will have been down in the SelectedIndexChanged event handler
                    // Here we simply make the list redraw the involved ListViewItem
                    ((ObjectListView)sender).RefreshItem(e.ListViewItem);

                    // We have updated the model object, so we cancel the auto update
                    e.Cancel = true;
                }
                else if (e.Column.Text == "Ethnicity")
                {

                    // Stop listening for change events
                    ((ComboBox)e.Control).SelectedIndexChanged -= new EventHandler(cbEthnicity_SelectedIndexChanged);

                    // Any updating will have been down in the SelectedIndexChanged event handler
                    // Here we simply make the list redraw the involved ListViewItem
                    ((ObjectListView)sender).RefreshItem(e.ListViewItem);

                    // We have updated the model object, so we cancel the auto update
                    e.Cancel = true;
                }
                else if (e.Column.Text == "Incidence Dataset")
                {
                    ((ComboBox)e.Control).SelectedIndexChanged -= new EventHandler(cbIncidenceDataSet_SelectedIndexChanged);

                    // Any updating will have been down in the SelectedIndexChanged event handler
                    // Here we simply make the list redraw the involved ListViewItem
                    ((ObjectListView)sender).RefreshItem(e.ListViewItem);

                    // We have updated the model object, so we cancel the auto update
                    e.Cancel = true;

                }
                else if (e.Column.Text == "Prevalence Dataset")
                {
                    ((ComboBox)e.Control).SelectedIndexChanged -= new EventHandler(cbPrevalenceDataSet_SelectedIndexChanged);

                    // Any updating will have been down in the SelectedIndexChanged event handler
                    // Here we simply make the list redraw the involved ListViewItem
                    ((ObjectListView)sender).RefreshItem(e.ListViewItem);

                    // We have updated the model object, so we cancel the auto update
                    e.Cancel = true;

                }
                else if (e.Column.Text == "Variable Dataset")
                {
                    ((ComboBox)e.Control).SelectedIndexChanged -= new EventHandler(cbVariableDataSet_SelectedIndexChanged);

                    // Any updating will have been down in the SelectedIndexChanged event handler
                    // Here we simply make the list redraw the involved ListViewItem
                    ((ObjectListView)sender).RefreshItem(e.ListViewItem);

                    // We have updated the model object, so we cancel the auto update
                    e.Cancel = true;
                }
            }
            catch
            {
                e.Cancel = true;
            }
        }
        List<string> lstVariable = Configuration.ConfigurationCommonClass.getAllSystemVariableNameList();
        Dictionary<string, Dictionary<string, double>> DicAllSetupVariableValues = new Dictionary<string, Dictionary<string, double>>();
        Dictionary<string, Dictionary<string, double>> dicAllIncidence = new Dictionary<string, Dictionary<string, double>>();
        Dictionary<string, Dictionary<string, double>> dicAllPrevalence = new Dictionary<string, Dictionary<string, double>>();
        Dictionary<string, Dictionary<int, float>> dicALlPopulation = new Dictionary<string, Dictionary<int, float>>();
        Dictionary<string, Dictionary<int, float>> dicALlPopulation12 = new Dictionary<string, Dictionary<int, float>>();
        Dictionary<string, Dictionary<string, float>> dicALlPopulationAge = new Dictionary<string, Dictionary<string, float>>();
        public string _filePath = "";
        DateTime dtRunStart;
       
        public void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                //---------------------------开始运算---------------------------------------------------------------------
                //------判断是否Batch--------
                bool isBatch = false;
                string tip = "Creating health impact function result. Please wait.";
                string sProgressBar = "";
                if (CommonClass.InputParams != null && CommonClass.InputParams.Count() > 0 && CommonClass.InputParams[0].ToLower().Contains(".ctlx"))
                {
                    isBatch = true;
                }
                if (!isBatch)
                {
                    if (olvSelected.Items.Count == 0)
                    {
                        MessageBox.Show("Please select studies first.");
                        return;
                    }
                    _filePath = "";

                    

                    //-----------------------majie----------------
                    DialogResult rtn = MessageBox.Show("Run and save the CFG results file (*.cfgrx)?", "Run and Save", MessageBoxButtons.YesNo);
                    if (rtn == System.Windows.Forms.DialogResult.No) { return; }
                    //----------------------提示是否SaveToFile-----------------------
                    //DialogResult rtn = MessageBox.Show("Save your results to a file?", "Tip", MessageBoxButtons.YesNoCancel);
                    //if (rtn == System.Windows.Forms.DialogResult.Cancel) { return; }
                    if (rtn == System.Windows.Forms.DialogResult.Yes)
                    {
                        //弹出窗口让其保存---------------------------------------------------
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = "cfgrx files (*.cfgrx)|*.cfgrx";
                        // sfd.FilterIndex = 2;
                        sfd.RestoreDirectory = true;
                        sfd.InitialDirectory = CommonClass.ResultFilePath + @"\Result\CFGR";
                        if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            _filePath = sfd.FileName;
                        }
                        else { return; }
                        //_filePath = sfd.FileName.Substring(0, sfd.FileName.LastIndexOf(@"\") + 1);
                    }
                    //开线程Run And 赋值 CommonClass.BaseControlCRFunction 
                    //WaitShow(tip);
                    //this.Enabled = false;
                    //----------把所有的button enabled=false;

                    btAddCRFunctions.Enabled = false;
                    btAdvanced.Enabled = false;
                    btDelSelectMethod.Enabled = false;
                    btnCancel.Enabled = false;
                    btnRun.Enabled = false;
                    btnSave.Enabled = false;
                    cbDataSet.Enabled = false;
                    cbEndPointGroup.Enabled = false;
                    cbGroups.Enabled = false;

                    lstAsyns = new List<string>();
                    lbProgressBar.Text = tip;
                    if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null && CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue != null)
                    {
                        foreach (CRSelectFunctionCalculateValue crv in CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
                        {
                            crv.CRCalculateValues = null;
                        }
                        CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue = null;
                        GC.Collect();
                    }
                    if (CommonClass.lstCRResultAggregation != null)
                    {
                        foreach (CRSelectFunctionCalculateValue crv in CommonClass.lstCRResultAggregation)
                        {
                            if(crv!=null)
                            crv.CRCalculateValues = null;
                        }
                        CommonClass.lstCRResultAggregation = null;
                        GC.Collect();
                    }
                    CommonClass.BaseControlCRSelectFunction = new BaseControlCRSelectFunction();
                    //foreach (BaseControlGroup bcg in CommonClass.LstBaseControlGroup)
                    //{
                    //    if (bcg.Base is MonitorDataLine )
                    //    {
                    //        if((bcg.Base as MonitorDataLine)!=null && (bcg.Base as MonitorDataLine).MonitorNeighbors!=null)
                    //        (bcg.Base as MonitorDataLine).MonitorNeighbors.Clear();
                    //    }
                    //    if (bcg.Control is MonitorDataLine)
                    //    {
                    //        if ((bcg.Control as MonitorDataLine) != null && (bcg.Control as MonitorDataLine).MonitorNeighbors != null)
                    //        (bcg.Control as MonitorDataLine).MonitorNeighbors.Clear();
                    //    }
                    //}
                    GC.Collect();
                    CommonClass.BaseControlCRSelectFunction.BaseControlGroup = CommonClass.LstBaseControlGroup;
                    CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction = olvSelected.Objects as List<CRSelectFunction>;
                    CommonClass.BaseControlCRSelectFunction.CRLatinHypercubePoints = CommonClass.CRLatinHypercubePoints;
                    CommonClass.BaseControlCRSelectFunction.CRRunInPointMode = CommonClass.CRRunInPointMode;
                    CommonClass.BaseControlCRSelectFunction.CRThreshold = CommonClass.CRThreshold;
                    CommonClass.BaseControlCRSelectFunction.RBenMapGrid = CommonClass.RBenMAPGrid;
                    CommonClass.BaseControlCRSelectFunction.BenMAPPopulation = CommonClass.BenMAPPopulation;
                }
                dtRunStart = DateTime.Now;
                //----------------del all valuation
                //CommonClass.ValuationMethodPoolingAndAggregation = null;
                Tools.CalculateFunctionString.dicPointEstimateMethodInfo.Clear();
                 

                List<GridRelationship> lstGridRelationshipAll = CommonClass.LstGridRelationshipAll;
                //暂时忽略Region
                string str = DateTime.Now.ToString();
                //List<CRSelectFunctionCalculateValue> lstCRSelectFunctionCalculateValue = Configuration.ConfigurationCommonClass.CalculateCRSelectFunctions(CommonClass.CRThreshold, CommonClass.CRLatinHypercubePoints,
                //   CommonClass.CRRunInPointMode, lstGridRelationshipAll, CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction, CommonClass.BaseControlCRSelectFunction.BaseControlGroup, null, CommonClass.BenMAPPopulation);
                //CommonClass.BaseControlCRSelectFunctionCalculateValue = new BaseControlCRSelectFunctionCalculateValue();
                //CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup = CommonClass.BaseControlCRSelectFunction.BaseControlGroup;
                //CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue = lstCRSelectFunctionCalculateValue;
                //// str = str + ":" + DateTime.Now.ToString();
                //WaitClose();
                //this.DialogResult = DialogResult.OK;
                //return;
                //--------modify by xiejp 2011927用多线程的方式来计算所有的CRFunction---------------------------------------------------------------
                //  List<CRSelectFunctionCalculateValue> lstCRSelectFunctionCalculateValue = new List<CRSelectFunctionCalculateValue>();
                Dictionary<string, int> dicRace = Configuration.ConfigurationCommonClass.getAllRace();// new Dictionary<string, int>();
                Dictionary<string, int> dicEthnicity = Configuration.ConfigurationCommonClass.getAllEthnicity();// new Dictionary<string, int>();
                Dictionary<string, int> dicGender = Configuration.ConfigurationCommonClass.getAllGender();// new Dictionary<string, int>(); 
                foreach (CRSelectFunction crSelectFunction in CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction)
                {
                    lstAsyns.Add(crSelectFunction.BenMAPHealthImpactFunction.ID.ToString());
                }
                this.pBarCR.Maximum = lstAsyns.Count()+2;
                this.pBarCR.Minimum = 0;
                if (isBatch)
                {
                    CommonClass.BenMAPPopulation = CommonClass.BaseControlCRSelectFunction.BenMAPPopulation;
                    CommonClass.CRLatinHypercubePoints = CommonClass.BaseControlCRSelectFunction.CRLatinHypercubePoints;
                    CommonClass.CRRunInPointMode = CommonClass.BaseControlCRSelectFunction.CRRunInPointMode;
                    CommonClass.CRThreshold= CommonClass.BaseControlCRSelectFunction.CRThreshold;
                    CommonClass.GBenMAPGrid = CommonClass.BaseControlCRSelectFunction.BaseControlGroup.First().GridType;
                    
                }
                CommonClass.BaseControlCRSelectFunctionCalculateValue = new BaseControlCRSelectFunctionCalculateValue();
                CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup = CommonClass.BaseControlCRSelectFunction.BaseControlGroup;
                CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue = new List<CRSelectFunctionCalculateValue>();
                CommonClass.BaseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints = CommonClass.CRLatinHypercubePoints;
                CommonClass.BaseControlCRSelectFunctionCalculateValue.CRRunInPointMode = CommonClass.CRRunInPointMode;
                CommonClass.BaseControlCRSelectFunctionCalculateValue.CRThreshold = CommonClass.CRThreshold;
                CommonClass.BaseControlCRSelectFunctionCalculateValue.RBenMapGrid = CommonClass.RBenMAPGrid;
                CommonClass.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation = CommonClass.BenMAPPopulation;
                CommonClass.BaseControlCRSelectFunctionCalculateValue.Version = "BenMAP-CE " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);

                int i = 0;
                List<CRSelectFunction> lstCRSelectFunction = new List<CRSelectFunction>();
                //------------------------修正，得到所有的Incidence数据
                dicAllIncidence = new Dictionary<string, Dictionary<string, double>>();
                dicAllPrevalence = new Dictionary<string, Dictionary<string, double>>();
                dicALlPopulation = new Dictionary<string, Dictionary<int, float>>();
                dicALlPopulationAge = new Dictionary<string, Dictionary<string, float>>();
                GridRelationship gridRelationShipIncidence = null;
                Dictionary<string, double> dicIncidenceRateAttribute = null;
                GridRelationship gridRelationShipPrevalence = null;
                Dictionary<string, double> dicPrevalenceRateAttribute = null;
                //Dictionary<int, float> dicPopulation = null;
                Dictionary<int, float> dicPopulation12 = null;
                //Dictionary<string, float> dicPopulationAge = null;
                
                GridRelationship gridPopulation = new GridRelationship() {
                    bigGridID = CommonClass.BenMAPPopulation.GridType.GridDefinitionID ==1?1: CommonClass.GBenMAPGrid.GridDefinitionID,
                    smallGridID = CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 1 ? CommonClass.GBenMAPGrid.GridDefinitionID : CommonClass.BenMAPPopulation.GridType.GridDefinitionID
                };
                //List<IncidenceRateAttribute> lstIncidenceRateAttribute = null;
                //List<IncidenceRateAttribute> lstPrevalenceRateAttribute = null;
                //List<PopulationAttribute> lstPopulation;
                //lbProgressBar.Text = "Read ";
                sProgressBar = "Loading Air Quality Data.";
                while (sProgressBar.Length < 57)
                {
                    sProgressBar = sProgressBar + " ";
                }
                this.lbProgressBar.Text = sProgressBar;
                this.pBarCR.Value++;
                Application.DoEvents();
                lbProgressBar.Refresh();
                //-------clearCalculateFunctionString
                if (Tools.CalculateFunctionString.dicBaselineMethodInfo != null) Tools.CalculateFunctionString.dicBaselineMethodInfo.Clear();
                if (Tools.CalculateFunctionString.dicPointEstimateMethodInfo != null) Tools.CalculateFunctionString.dicPointEstimateMethodInfo.Clear();
                if (Tools.CalculateFunctionString.dicValuationMethodInfo != null) Tools.CalculateFunctionString.dicValuationMethodInfo.Clear();

                List<SetupVariableJoinAllValues> lstSetupVariable = new List<SetupVariableJoinAllValues>();
                Dictionary<int, Dictionary<string, ModelResultAttribute>> DicControlAll = new Dictionary<int, Dictionary<string, ModelResultAttribute>>();
                Dictionary<int,Dictionary<string, Dictionary<string, float>>> dicAllMetricDataBase=new Dictionary<int,Dictionary<string,Dictionary<string,float>>>();
                Dictionary<int,Dictionary<string, Dictionary<string, float>>> dicAllMetricDataControl=new Dictionary<int,Dictionary<string,Dictionary<string,float>>>();
                Dictionary<int,Dictionary<string,Dictionary<string,List<float>>>> dicAll365Base=new Dictionary<int,Dictionary<string,Dictionary<string,List<float>>>>();
                Dictionary<int,Dictionary<string,Dictionary<string,List<float>>>> dicAll365Control=new Dictionary<int,Dictionary<string,Dictionary<string,List<float>>>>();
                foreach (BaseControlGroup baseControlGroup in CommonClass.LstBaseControlGroup)
                {
                    Dictionary<string, ModelResultAttribute> dicControl = new Dictionary<string, ModelResultAttribute>();
                    foreach (ModelResultAttribute mr in baseControlGroup.Control.ModelResultAttributes)
                    {
                        if (!dicControl.Keys.Contains(mr.Col + "," + mr.Row))
                            dicControl.Add(mr.Col + "," + mr.Row, mr);
                    }
                    DicControlAll.Add(baseControlGroup.Pollutant.PollutantID, dicControl);
                    Dictionary<string,Dictionary<string,List<float>>> dic365Base=new Dictionary<string,Dictionary<string,List<float>>>();
                    dicAllMetricDataBase.Add(baseControlGroup.Pollutant.PollutantID, Configuration.ConfigurationCommonClass.getAllMetricDataFromBaseControlGroup(baseControlGroup, true, ref dic365Base));
                    dicAll365Base.Add(baseControlGroup.Pollutant.PollutantID, dic365Base);

                    Dictionary<string, Dictionary<string, List<float>>> dic365Control = new Dictionary<string, Dictionary<string, List<float>>>();
                    dicAllMetricDataControl.Add(baseControlGroup.Pollutant.PollutantID, Configuration.ConfigurationCommonClass.getAllMetricDataFromBaseControlGroup(baseControlGroup, false, ref dic365Control));
                    dicAll365Control.Add(baseControlGroup.Pollutant.PollutantID, dic365Control);
                }
                foreach (GridRelationship gRelationship in lstGridRelationshipAll)
                {
                    if ((gRelationship.bigGridID == CommonClass.BenMAPPopulation.GridType.GridDefinitionID && gRelationship.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID) || (gRelationship.smallGridID == CommonClass.BenMAPPopulation.GridType.GridDefinitionID && gRelationship.bigGridID == CommonClass.GBenMAPGrid.GridDefinitionID))
                    {
                        gridPopulation = gRelationship;
                    }
                }
                int crid = 1, crCount = CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction==null?0:CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count();
                Tools.CalculateFunctionString.dicPointEstimateMethodInfo.Clear();
                Tools.CalculateFunctionString.dicPointEstimateMethodInfo = new Dictionary<string, object>();
                //try
                //{
                //    System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController("FirebirdServerDefaultInstance");
                //    service.Stop();
                //    service.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped);
                //    service.Start();
                //    service.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running);
                //}
                //catch
                //{ 
                //}
                Application.DoEvents();
                //lbProgressBar.Text = "Loading Population data ...";// +crid + " of " + crCount + "...";
                sProgressBar = "Loading Population data.";
                while (sProgressBar.Length < 57)
                {
                    sProgressBar = sProgressBar + " ";
                }
                this.lbProgressBar.Text = sProgressBar;
                this.pBarCR.Value++;
                lbProgressBar.Refresh();
                //---------------------------提速度想法，清理所有的Population,根据不同的Race,Ethithty,Gender,求出所有年龄组合的Age,然后再拼出Population，然后再计算Incidence,然后再算值
                Dictionary<string, string> dicAllRaceEthnicityGenderAge = new Dictionary<string, string>();
                foreach (CRSelectFunction crSelectFunction in CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction)
                {
                    if (dicAllRaceEthnicityGenderAge.ContainsKey(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender))
                    {
                        string[] strAgeArray = dicAllRaceEthnicityGenderAge[crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender].Split(new char[] { ',' });
                        if (Convert.ToInt32(strAgeArray[0]) > crSelectFunction.StartAge)
                        {
                            dicAllRaceEthnicityGenderAge[crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender] =
                                crSelectFunction.StartAge + "," + strAgeArray[1];
                        }
                        if (Convert.ToInt32(strAgeArray[1]) < crSelectFunction.EndAge)
                        {
                            dicAllRaceEthnicityGenderAge[crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender] =
                                 strAgeArray[0] + "," + crSelectFunction.EndAge;
                        }


                    }
                    else
                        dicAllRaceEthnicityGenderAge.Add(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender, crSelectFunction.StartAge + "," + crSelectFunction.EndAge);
                }
                dicALlPopulationAge = new Dictionary<string, Dictionary<string, float>>();
                foreach (KeyValuePair<string, string> kAge in dicAllRaceEthnicityGenderAge)
                {
                    string[] skAgeArray = kAge.Value.Split(new char[] { ',' });
                    string[] skAgeArrayRaceGenderEthnicity = kAge.Key.Split(new char[] { ',' });
                    Dictionary<string, float> dicPopulationAgeIn = new Dictionary<string, float>();
                    CRSelectFunction crSelectFunction = CommonClass.getCRSelectFunctionClone(CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.First());
                    crSelectFunction.StartAge = Convert.ToInt32(skAgeArray[0]);
                    crSelectFunction.EndAge = Convert.ToInt32(skAgeArray[1]);
                    crSelectFunction.Race = skAgeArrayRaceGenderEthnicity[0];
                    crSelectFunction.Ethnicity = skAgeArrayRaceGenderEthnicity[1];
                    crSelectFunction.Gender = skAgeArrayRaceGenderEthnicity[2];
                    Configuration.ConfigurationCommonClass.getPopulationDataSetFromCRSelectFunction(ref dicPopulationAgeIn, ref dicPopulation12, crSelectFunction, CommonClass.BenMAPPopulation, dicRace, dicEthnicity,
                            dicGender, CommonClass.GBenMAPGrid.GridDefinitionID, gridPopulation);
                    dicALlPopulationAge.Add(kAge.Key, dicPopulationAgeIn);
 
                }
                //-------------修正：需要得到所有的AgeID-----------------------------
                List<string> lstAllAgeID = Configuration.ConfigurationCommonClass.getAllAgeID();
                Dictionary<string,string> dicBaseLine=new Dictionary<string,string>();
                Dictionary<string,string> dicEstimate=new Dictionary<string,string>();
                 Dictionary<string,string> dicBaseLineVariables=new Dictionary<string,string>();
                 Dictionary<string,string> dicEstimateVariables=new Dictionary<string,string>();
                foreach (CRSelectFunction crSelectFunction in CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction)
                {
                    dicBaseLine.Add(crid.ToString(),Configuration.ConfigurationCommonClass.getFunctionStringFromDatabaseFunction(crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction));
                    //------------Get Variables in Baseline
                    string DatabaseFunction = crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction.Replace("prevalence", "").Replace("incidence", "").Replace("deltaq", "")
                    .Replace("pop", "").Replace("beta", "").Replace("q0", "").Replace("q1", "")
                   .Replace("abs", " ")
     .Replace("acos", " ")
     .Replace("asin", " ")
     .Replace("atan", " ")
     .Replace("atan2", " ")
     .Replace("bigmul", " ")
     .Replace("ceiling", " ")
     .Replace("cos", " ")
     .Replace("cosh", " ")
     .Replace("divrem", " ")
     .Replace("exp", " ")
     .Replace("floor", " ")
     .Replace("ieeeremainder", " ")
     .Replace("log", " ")
     .Replace("log10", " ")
     .Replace("max", " ")
     .Replace("min", " ")
     .Replace("pow", " ")
     .Replace("round", " ")
     .Replace("sign", " ")
     .Replace("sin", " ")
     .Replace("sinh", " ")
     .Replace("sqrt", " ")
     .Replace("tan", " ")
     .Replace("tanh", " ")
     .Replace("truncate", " ");
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
List<string> SystemVariableNameList = Configuration.ConfigurationCommonClass.getAllSystemVariableNameList();
               foreach (string s in SystemVariableNameList)
               {
                   if (DatabaseFunction.ToLower().Contains(str.ToLower()))
                   {
                      if(dicBaseLineVariables.ContainsKey(crid.ToString()))
                      {
                         if( dicBaseLineVariables[crid.ToString()]=="")
                             dicBaseLineVariables[crid.ToString()]=s;
                          else
                             dicBaseLineVariables[crid.ToString()]+=","+s;
                      }
                       else
                      {
                          dicBaseLineVariables.Add(crid.ToString(),s);
                      }
                   }
               }
                    dicEstimate.Add(crid.ToString(),Configuration.ConfigurationCommonClass.getFunctionStringFromDatabaseFunction(crSelectFunction.BenMAPHealthImpactFunction.Function));
                    DatabaseFunction = crSelectFunction.BenMAPHealthImpactFunction.Function.Replace("prevalence", "").Replace("incidence", "").Replace("deltaq", "")
                    .Replace("pop", "").Replace("beta", "").Replace("q0", "").Replace("q1", "")
                   .Replace("abs", " ")
     .Replace("acos", " ")
     .Replace("asin", " ")
     .Replace("atan", " ")
     .Replace("atan2", " ")
     .Replace("bigmul", " ")
     .Replace("ceiling", " ")
     .Replace("cos", " ")
     .Replace("cosh", " ")
     .Replace("divrem", " ")
     .Replace("exp", " ")
     .Replace("floor", " ")
     .Replace("ieeeremainder", " ")
     .Replace("log", " ")
     .Replace("log10", " ")
     .Replace("max", " ")
     .Replace("min", " ")
     .Replace("pow", " ")
     .Replace("round", " ")
     .Replace("sign", " ")
     .Replace("sin", " ")
     .Replace("sinh", " ")
     .Replace("sqrt", " ")
     .Replace("tan", " ")
     .Replace("tanh", " ")
     .Replace("truncate", " ");
              
               foreach (string s in SystemVariableNameList)
               {
                   if (DatabaseFunction.ToLower().Contains(str.ToLower()))
                   {
                      if(dicEstimateVariables .ContainsKey(crid.ToString()))
                      {
                         if( dicEstimateVariables[crid.ToString()]=="")
                             dicEstimateVariables[crid.ToString()]="double " +s.ToLower();
                          else
                             dicEstimateVariables[crid.ToString()] += ",double " + s.ToLower();
                      }
                       else
                      {
                          dicEstimateVariables.Add(crid.ToString(), "double " + s.ToLower());
                      }
                   }
               }

                    crid = crid + 1;
                }
                CalculateFunctionString calculateFunctionString = new CalculateFunctionString();
                calculateFunctionString.CreateAllBaselineEvalObjects(dicBaseLine,dicBaseLineVariables);
                calculateFunctionString.CreateAllPointEstimateEvalObjects(dicEstimate,dicEstimateVariables);
                crid = 1;
                foreach (CRSelectFunction crSelectFunction in CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction)
                {
                    //Configuration.ConfigurationCommonClass.CalculateOneCRSelectFunction(dicRace, dicEthnicity, dicGender, CommonClass.CRThreshold, CommonClass.CRLatinHypercubePoints,
                    //    CommonClass.CRRunInPointMode, lstGridRelationshipAll, crSelectFunction, CommonClass.BaseControlCRSelectFunction.BaseControlGroup, null, CommonClass.BenMAPPopulation);
                    //if (i < 3 && i < CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count - 1)
                    //{
                    //    lstCRSelectFunction.Add(crSelectFunction);

                    //}
                    //else
                    //{
                   
                    string strAuthorCount = crid.ToString() + " of " + CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count.ToString()+".";// +"...";// crSelectFunction.BenMAPHealthImpactFunction.Author + " " + crid.ToString() + " of " + CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count.ToString() + "...";
                    while (strAuthorCount.Length < CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count.ToString().Length * 2 + 4)
                    {
                        strAuthorCount = " " + strAuthorCount;
 
                    }

                    if (crSelectFunction.StartAge == -1 && crSelectFunction.EndAge == -1)
                    {
                        crSelectFunction.StartAge = 0;
                        crSelectFunction.EndAge = 0;
                    }
                    //--------求Population--------------
                    Dictionary<string, float>  dicPopulationAge = dicALlPopulationAge[crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender];
                    //Dictionary<int, float> dicPopulation   = new Dictionary<int, float>();
                     
                    Dictionary<string, double> dicAge = Configuration.ConfigurationCommonClass.getDicAge(crSelectFunction);
                    //foreach (KeyValuePair<string, float> k in dicPopulationAge)
                    //{
                    //    string[] s = k.Key.Split(new char[] { ',' });
                    //    if (!dicAge.ContainsKey(s[2])) continue;
                    //    if (dicPopulation.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                    //    {
                    //        dicPopulation[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])] += k.Value * Convert.ToSingle(dicAge[s[2]]);
                    //    }
                    //    else
                    //    {
                    //        dicPopulation.Add(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1]), k.Value * Convert.ToSingle(dicAge[s[2]]));
                    //    }
                    //}

                    //---------求Incidence和
                     
                    //if (dicALlPopulation.Keys.Contains(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID))
                    //{
                    //    dicPopulation = dicALlPopulation[crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID];
                    //    if (dicALlPopulation12.ContainsKey(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID))
                    //        dicPopulation12 = dicALlPopulation12[crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID];
                    //    dicPopulationAge = dicALlPopulationAge[crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID];
                    //}
                    //else
                    //{
                    //    dicPopulationAge = new Dictionary<string, float>();
                    //    dicPopulation12 = new Dictionary<int, float>();
                    //    dicPopulation = Configuration.ConfigurationCommonClass.getPopulationDataSetFromCRSelectFunction(ref dicPopulationAge, ref dicPopulation12, crSelectFunction, CommonClass.BenMAPPopulation, dicRace, dicEthnicity,
                    //        dicGender, CommonClass.GBenMAPGrid.GridDefinitionID, gridPopulation);
                    //    //dicPopulation = new Dictionary<int, double>();
                    //    //foreach (PopulationAttribute pa in lstPopulation)
                    //    //{
                    //    //    if (!dicPopulation.Keys.Contains(Convert.ToInt32(pa.Col) * 10000 + Convert.ToInt32(pa.Row)))
                    //    //    {
                    //    //        dicPopulation.Add(Convert.ToInt32(pa.Col) * 10000 + Convert.ToInt32(pa.Row), pa.Value);

                    //    //    }

                    //    //}
                    //    //lstPopulation = null;
                    //    dicALlPopulationAge.Add(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID, dicPopulationAge);
                    //    dicALlPopulation.Add(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID, dicPopulation);
                    //    if (dicALlPopulation12.ContainsKey(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID))
                    //        dicALlPopulation12.Remove(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID);
                    //    dicALlPopulation12.Add(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID, dicPopulation12);
                    //}
                    string commandText = string.Format("select GriddefinitionID from IncidenceDatasets where IncidenceDatasetID={0}", crSelectFunction.IncidenceDataSetID);
                    int incidenceDataSetGridType = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                    commandText = string.Format("select GriddefinitionID from IncidenceDatasets where IncidenceDatasetID={0}", crSelectFunction.PrevalenceDataSetID);
                    int PrevalenceDataSetGridType = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));

                    if (crSelectFunction.IncidenceDataSetID > -1)
                    {
                        //lbProgressBar.Text = "Loading Incidence data for Study " + strAuthorCount;//+ crid+" of "+ crCount + "...";
                        //sProgressBar = "Loading Incidence data for Study  " + strAuthorCount;
                        //while (sProgressBar.Length < 57)
                        //{
                        //    sProgressBar = sProgressBar + " ";
                        //}
                        //this.lbProgressBar.Text = sProgressBar;
                        //Application.DoEvents();
                        //lbProgressBar.Refresh();
                        if (dicAllIncidence.Keys.Contains(crSelectFunction.IncidenceDataSetID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge))
                        {
                            dicIncidenceRateAttribute = dicAllIncidence[crSelectFunction.IncidenceDataSetID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge];
                        }
                        else
                        {
                            foreach (GridRelationship gRelationship in lstGridRelationshipAll)
                            {
                                if ((gRelationship.bigGridID == incidenceDataSetGridType && gRelationship.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID) || (gRelationship.smallGridID == incidenceDataSetGridType && gRelationship.bigGridID == CommonClass.GBenMAPGrid.GridDefinitionID))
                                {
                                    gridRelationShipIncidence = gRelationship;
                                }
                            }
                            if (gridRelationShipIncidence == null)
                            {
                                gridRelationShipIncidence = new GridRelationship()
                                {
                                    bigGridID = incidenceDataSetGridType == 1 ? 1 : CommonClass.GBenMAPGrid.GridDefinitionID,
                                    smallGridID = incidenceDataSetGridType == 1 ? CommonClass.GBenMAPGrid.GridDefinitionID : incidenceDataSetGridType
                                };
                            }
                            //lstIncidenceRateAttribute =Configuration.ConfigurationCommonClass. getIncidenceDataSetFromCRSelectFuntion(crSelectFunction, false, dicRace, dicEthnicity, dicGender, CommonClass.GBenMAPGrid.GridDefinitionID, gridRelationShipIncidence);
                            //dicIncidenceRateAttribute = new Dictionary<string, double>();
                            //foreach (IncidenceRateAttribute inc in lstIncidenceRateAttribute)
                            //{
                            //    if (!dicIncidenceRateAttribute.Keys.Contains(inc.Col + "," + inc.Row))
                            //    {
                            //        dicIncidenceRateAttribute.Add(inc.Col + "," + inc.Row, inc.Value);

                            //    }
                            //}
                            dicIncidenceRateAttribute = Configuration.ConfigurationCommonClass.getIncidenceDataSetFromCRSelectFuntionDicAllAge(dicAge, dicPopulationAge, dicPopulation12, crSelectFunction, false, dicRace, dicEthnicity, dicGender, CommonClass.GBenMAPGrid.GridDefinitionID, gridRelationShipIncidence);
                            dicAllIncidence.Add(crSelectFunction.IncidenceDataSetID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge, dicIncidenceRateAttribute);
                        }
                    }
                    if (crSelectFunction.PrevalenceDataSetID > -1)
                    {
                        //lbProgressBar.Text = "Loading Prevalence data for Study " + strAuthorCount;//+ crid+" of "+ crCount + "...";
                        //sProgressBar = "Loading Prevalence data for Study " + strAuthorCount;
                        //while (sProgressBar.Length < 57)
                        //{
                        //    sProgressBar = sProgressBar + " ";
                        //}
                        //this.lbProgressBar.Text = sProgressBar;
                        //Application.DoEvents();
                        //lbProgressBar.Refresh();
                        if (dicAllPrevalence.Keys.Contains(crSelectFunction.PrevalenceDataSetID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge))
                        {
                            dicPrevalenceRateAttribute = dicAllPrevalence[crSelectFunction.PrevalenceDataSetID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge];
                        }
                        else
                        {
                            //PrevalenceDataSetGridType = Convert.ToInt32(fb.ExecuteScalar(_connection, CommandType.Text, commandText));
                            foreach (GridRelationship gRelationship in lstGridRelationshipAll)
                            {
                                if ((gRelationship.bigGridID == PrevalenceDataSetGridType && gRelationship.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID) || (gRelationship.smallGridID == PrevalenceDataSetGridType && gRelationship.bigGridID == CommonClass.GBenMAPGrid.GridDefinitionID))
                                {
                                    gridRelationShipPrevalence = gRelationship;
                                }
                            }
                            if (gridRelationShipPrevalence == null)
                            {
                                gridRelationShipPrevalence = new GridRelationship()
                                {
                                    bigGridID = PrevalenceDataSetGridType == 1 ? 1 : CommonClass.GBenMAPGrid.GridDefinitionID,
                                    smallGridID = PrevalenceDataSetGridType == 1 ? CommonClass.GBenMAPGrid.GridDefinitionID : PrevalenceDataSetGridType
                                };
                            }

                            //lstPrevalenceRateAttribute = Configuration.ConfigurationCommonClass.getIncidenceDataSetFromCRSelectFuntion(crSelectFunction, true, dicRace, dicEthnicity, dicGender, CommonClass.GBenMAPGrid.GridDefinitionID, gridRelationShipPrevalence);
                            //dicPrevalenceRateAttribute = new Dictionary<string, double>();
                            //foreach (IncidenceRateAttribute inc in lstPrevalenceRateAttribute)
                            //{
                            //    if (!dicPrevalenceRateAttribute.Keys.Contains(inc.Col + "," + inc.Row))
                            //    {
                            //        dicPrevalenceRateAttribute.Add(inc.Col + "," + inc.Row, inc.Value);

                            //    }
                            //}
                            dicPrevalenceRateAttribute = Configuration.ConfigurationCommonClass.getIncidenceDataSetFromCRSelectFuntionDicAllAge(dicAge,  dicPopulationAge, dicPopulation12, crSelectFunction, true, dicRace, dicEthnicity, dicGender, CommonClass.GBenMAPGrid.GridDefinitionID, gridRelationShipPrevalence);
                            dicAllPrevalence.Add(crSelectFunction.PrevalenceDataSetID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge, dicPrevalenceRateAttribute);

                        }
                    }
                    lstSetupVariable = null;
                    DicAllSetupVariableValues = new Dictionary<string, Dictionary<string, double>>();
                    if (crSelectFunction.VariableDataSetID > -1)
                    {
                        //lbProgressBar.Text = "Loading Variable data for Study " + strAuthorCount;//+ crid+" of "+ crCount + "...";
                        //sProgressBar = "Loading Variable data for Study   " + strAuthorCount;
                        //while (sProgressBar.Length < 57)
                        //{
                        //    sProgressBar = sProgressBar + " ";
                        //}
                        //this.lbProgressBar.Text = sProgressBar;
                        //Application.DoEvents();
                        //lbProgressBar.Refresh();
                        lstSetupVariable = new List<SetupVariableJoinAllValues>();
                        Configuration.ConfigurationCommonClass.getSetupVariableNameListFromDatabaseFunction(crSelectFunction.VariableDataSetID,CommonClass.GBenMAPGrid.GridDefinitionID, crSelectFunction.BenMAPHealthImpactFunction.Function, Configuration.ConfigurationCommonClass.LstSystemVariableName, ref lstSetupVariable);
                        Configuration.ConfigurationCommonClass.getSetupVariableNameListFromDatabaseFunction(crSelectFunction.VariableDataSetID,CommonClass.GBenMAPGrid.GridDefinitionID, crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction, Configuration.ConfigurationCommonClass.LstSystemVariableName, ref lstSetupVariable);
                        //算系统变量

                        if (lstSetupVariable != null)
                        {
                            foreach (SetupVariableJoinAllValues sv in lstSetupVariable)
                            {
                                //dicSetupVariable.Add(sv..Col + "," + sv.Row, sv.Value);
                                Dictionary<string, double> dic = new Dictionary<string, double>();
                                foreach (SetupVariableValues svv in sv.lstValues)
                                {
                                    if (!dic.Keys.Contains(svv.Col + "," + svv.Row))
                                        dic.Add(svv.Col + "," + svv.Row, svv.Value);
                                }
                                DicAllSetupVariableValues.Add(sv.SetupVariableName, dic);
                            }
                            lstSetupVariable = null;
                        }
                    }

                    var query = from a in CommonClass.LstBaseControlGroup where a.Pollutant.PollutantID == crSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantID select a;
                    if (query == null || query.Count() == 0)
                    {
                        return;
                    }
                    BaseControlGroup baseControlGroup = query.First();
                    AsyncDelegateCalculateOneCRSelectFunction dlgt = new AsyncDelegateCalculateOneCRSelectFunction(Configuration.ConfigurationCommonClass.CalculateOneCRSelectFunction);
                    //AsyncDelegateCalculateLstCRSelectFunction dlgt = new AsyncDelegateCalculateLstCRSelectFunction(CalculateLstCRSelectFunction);
                    //    //lstAsyns.Add("one");
                    //    ////string strFileName = CommonClass._modelBase.ModelPath.Substring(CommonClass._modelBase.ModelPath.LastIndexOf(@"\"), CommonClass._modelBase.ModelPath.LastIndexOf(".") - CommonClass._modelBase.ModelPath.LastIndexOf(@"\")-1);
                    //    ////GridLayerName = strFileName.Replace(@"\", "");
                    //    //// Initiate the asychronous call.   
                    //IAsyncResult ar = dlgt.BeginInvoke(lstCRSelectFunction, dicRace, dicEthnicity, dicGender,
                    //lstGridRelationshipAll, new AsyncCallback(outPut), dlgt);
                    double[] lhsResultArray = null;
                    if (!CommonClass.CRRunInPointMode)
                    {
                        int iRandomSeed = Convert.ToInt32(DateTime.Now.Hour + "" + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond);
                        if (CommonClass.CRSeeds != null && CommonClass.CRSeeds != -1)
                            iRandomSeed = Convert.ToInt32(CommonClass.CRSeeds);

                        //Console.WriteLine(DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Millisecond);
                        //if (crSelectFunction.lstLatinPoints != null && crSelectFunction.lstLatinPoints.Count() >=CommonClass.CRSeeds)
                        //{
                        //    lhsResultArray = crSelectFunction.lstLatinPoints[CommonClass.CRSeeds-1].values.ToArray();
                        //}
                        //else
                        //{
                        //    lhsResultArray = Configuration.ConfigurationCommonClass.getLHSArrayCRFunction(CommonClass.CRLatinHypercubePoints, crSelectFunction);
                        //    crSelectFunction.lstLatinPoints =new List<LatinPoints>();
                        //    crSelectFunction.lstLatinPoints.Add(new LatinPoints() { values = lhsResultArray.ToList(), });
                        //    for (int iSeed = 1; iSeed <10; iSeed++)
                        //    {

                        //        crSelectFunction.lstLatinPoints.Add(new LatinPoints() { values = Configuration.ConfigurationCommonClass.getLHSArrayCRFunction(CommonClass.CRLatinHypercubePoints, crSelectFunction).ToList() }); 
                        //    }
                        //}
                        //Console.WriteLine(DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Millisecond);
                        //crSelectFunctionCalculateValue.lstLatin = lhsResultArray.ToList();
                        lhsResultArray = Configuration.ConfigurationCommonClass.getLHSArrayCRFunctionSeed(CommonClass.CRLatinHypercubePoints, crSelectFunction, iRandomSeed);
                        crSelectFunction.lstLatinPoints = new List<LatinPoints>();
                        crSelectFunction.lstLatinPoints.Add(new LatinPoints() { values = lhsResultArray.ToList() });
                    }
                    string iRunCRID =crid.ToString();
                    if (!isBatch)
                    {
                        IAsyncResult ar = dlgt.BeginInvoke(iRunCRID, lstAllAgeID, dicAge, dicAllMetricDataBase[baseControlGroup.Pollutant.PollutantID], dicAllMetricDataControl[baseControlGroup.Pollutant.PollutantID]
                            , dicAll365Base[baseControlGroup.Pollutant.PollutantID], dicAll365Control[baseControlGroup.Pollutant.PollutantID], DicControlAll[baseControlGroup.Pollutant.PollutantID],
                            DicAllSetupVariableValues,  dicPopulationAge, dicIncidenceRateAttribute, dicPrevalenceRateAttribute, incidenceDataSetGridType, PrevalenceDataSetGridType, dicRace, dicEthnicity, dicGender, CommonClass.CRThreshold, CommonClass.CRLatinHypercubePoints,
                            CommonClass.CRRunInPointMode, lstGridRelationshipAll, crSelectFunction, baseControlGroup, null, CommonClass.BenMAPPopulation, lhsResultArray, new AsyncCallback(outPut), dlgt);
                    }
                    else
                    {
                        Configuration.ConfigurationCommonClass.CalculateOneCRSelectFunction(iRunCRID, lstAllAgeID, dicAge, dicAllMetricDataBase[baseControlGroup.Pollutant.PollutantID], dicAllMetricDataControl[baseControlGroup.Pollutant.PollutantID]
                            , dicAll365Base[baseControlGroup.Pollutant.PollutantID], dicAll365Control[baseControlGroup.Pollutant.PollutantID], DicControlAll[baseControlGroup.Pollutant.PollutantID],
                            DicAllSetupVariableValues, dicPopulationAge, dicIncidenceRateAttribute, dicPrevalenceRateAttribute, incidenceDataSetGridType, PrevalenceDataSetGridType, dicRace, dicEthnicity, dicGender, CommonClass.CRThreshold, CommonClass.CRLatinHypercubePoints,
                            CommonClass.CRRunInPointMode, lstGridRelationshipAll, crSelectFunction, baseControlGroup, null, CommonClass.BenMAPPopulation, lhsResultArray);

                    }
                    crid = crid + 1; ;
                    //this.pBarCR.Value++;
                    //    i = 0;
                    //}
                    //i++;
                    //CRSelectFunctionCalculateValue crv = dlgt.EndInvoke(ar);
                    //lstCRSelectFunctionCalculateValue.Add(crv);
                }
                /*
                foreach (CRSelectFunction crSelectFunction in CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction)
                {
                    //Configuration.ConfigurationCommonClass.CalculateOneCRSelectFunction(dicRace, dicEthnicity, dicGender, CommonClass.CRThreshold, CommonClass.CRLatinHypercubePoints,
                    //    CommonClass.CRRunInPointMode, lstGridRelationshipAll, crSelectFunction, CommonClass.BaseControlCRSelectFunction.BaseControlGroup, null, CommonClass.BenMAPPopulation);
                    //if (i < 3 && i < CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count - 1)
                    //{
                    //    lstCRSelectFunction.Add(crSelectFunction);

                    //}
                    //else
                    //{
                    try
                    {
                     CommonClass.Connection.Close();
                    }
                    catch
                    { 
                    }
                    
                    CommonClass.Connection = CommonClass.getNewConnection();
                    string strAuthorCount = crSelectFunction.BenMAPHealthImpactFunction.Author + " " + crid.ToString() + " of " + CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count.ToString() + "...";
                    Application.DoEvents();
                    lbProgressBar.Text = "Loading Population data for Study " + strAuthorCount;// +crid + " of " + crCount + "...";
                    
                    lbProgressBar.Refresh();
                    
                    if (crSelectFunction.StartAge == -1 && crSelectFunction.EndAge == -1)
                    {
                        crSelectFunction.StartAge = 0;
                        crSelectFunction.EndAge = 0;
                    }
                    if (dicALlPopulation.Keys.Contains(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID))
                    {
                        dicPopulation = dicALlPopulation[crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID];
                        if (dicALlPopulation12.ContainsKey(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID))
                            dicPopulation12 = dicALlPopulation12[crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID];
                        dicPopulationAge = dicALlPopulationAge[crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID];
                    }
                    else
                    {
                        dicPopulationAge = new Dictionary<string, float>();
                        dicPopulation12 = new Dictionary<int, float>();
                        dicPopulation = Configuration.ConfigurationCommonClass.getPopulationDataSetFromCRSelectFunction(ref dicPopulationAge, ref dicPopulation12, crSelectFunction, CommonClass.BenMAPPopulation, dicRace, dicEthnicity,
                            dicGender, CommonClass.GBenMAPGrid.GridDefinitionID, gridPopulation);
                        //dicPopulation = new Dictionary<int, double>();
                        //foreach (PopulationAttribute pa in lstPopulation)
                        //{
                        //    if (!dicPopulation.Keys.Contains(Convert.ToInt32(pa.Col) * 10000 + Convert.ToInt32(pa.Row)))
                        //    {
                        //        dicPopulation.Add(Convert.ToInt32(pa.Col) * 10000 + Convert.ToInt32(pa.Row), pa.Value);

                        //    }

                        //}
                        //lstPopulation = null;
                        dicALlPopulationAge.Add(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID, dicPopulationAge);
                        dicALlPopulation.Add(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID, dicPopulation);
                        if (dicALlPopulation12.ContainsKey(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID))
                            dicALlPopulation12.Remove(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID);
                        dicALlPopulation12.Add(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge + "," + CommonClass.BenMAPPopulation.DataSetID, dicPopulation12);
                    }
                    string commandText = string.Format("select GriddefinitionID from IncidenceDatasets where IncidenceDatasetID={0}", crSelectFunction.IncidenceDataSetID);
                    int incidenceDataSetGridType = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                    commandText = string.Format("select GriddefinitionID from IncidenceDatasets where IncidenceDatasetID={0}", crSelectFunction.PrevalenceDataSetID);
                    int PrevalenceDataSetGridType = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));

                    if (crSelectFunction.IncidenceDataSetID > -1)
                    {
                        lbProgressBar.Text = "Loading Incidence data for Study " + strAuthorCount;//+ crid+" of "+ crCount + "...";
                    Application.DoEvents();
                    lbProgressBar.Refresh();
                    if (dicAllIncidence.Keys.Contains(crSelectFunction.IncidenceDataSetID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointID  + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge))
                        {
                            dicIncidenceRateAttribute = dicAllIncidence[crSelectFunction.IncidenceDataSetID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + ","  + crSelectFunction.StartAge + "," + crSelectFunction.EndAge];
                        }
                        else
                        {
                            foreach (GridRelationship gRelationship in lstGridRelationshipAll)
                            {
                                if ((gRelationship.bigGridID == incidenceDataSetGridType && gRelationship.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID) || (gRelationship.smallGridID == incidenceDataSetGridType && gRelationship.bigGridID == CommonClass.GBenMAPGrid.GridDefinitionID))
                                {
                                    gridRelationShipIncidence = gRelationship;
                                }
                            }
                            if (gridRelationShipIncidence == null)
                            {
                                gridRelationShipIncidence = new GridRelationship()
                                {
                                    bigGridID = incidenceDataSetGridType == 1 ? 1 : CommonClass.GBenMAPGrid.GridDefinitionID,
                                    smallGridID = incidenceDataSetGridType == 1 ? CommonClass.GBenMAPGrid.GridDefinitionID : incidenceDataSetGridType
                                };
                            }
                            //lstIncidenceRateAttribute =Configuration.ConfigurationCommonClass. getIncidenceDataSetFromCRSelectFuntion(crSelectFunction, false, dicRace, dicEthnicity, dicGender, CommonClass.GBenMAPGrid.GridDefinitionID, gridRelationShipIncidence);
                            //dicIncidenceRateAttribute = new Dictionary<string, double>();
                            //foreach (IncidenceRateAttribute inc in lstIncidenceRateAttribute)
                            //{
                            //    if (!dicIncidenceRateAttribute.Keys.Contains(inc.Col + "," + inc.Row))
                            //    {
                            //        dicIncidenceRateAttribute.Add(inc.Col + "," + inc.Row, inc.Value);

                            //    }
                            //}
                            dicIncidenceRateAttribute = Configuration.ConfigurationCommonClass.getIncidenceDataSetFromCRSelectFuntionDicAllAge(dicPopulation,dicPopulationAge,dicPopulation12, crSelectFunction, false, dicRace, dicEthnicity, dicGender, CommonClass.GBenMAPGrid.GridDefinitionID, gridRelationShipIncidence);
                            dicAllIncidence.Add(crSelectFunction.IncidenceDataSetID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + ","  + crSelectFunction.StartAge + "," + crSelectFunction.EndAge, dicIncidenceRateAttribute);
                        }
                    }
                    if (crSelectFunction.PrevalenceDataSetID > -1)
                    {
                        lbProgressBar.Text = "Loading Prevalence data for Study " + strAuthorCount;//+ crid+" of "+ crCount + "...";
                    Application.DoEvents();
                    lbProgressBar.Refresh();
                    if (dicAllPrevalence.Keys.Contains(crSelectFunction.PrevalenceDataSetID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointID +  "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge))
                        {
                            dicPrevalenceRateAttribute = dicAllPrevalence[crSelectFunction.PrevalenceDataSetID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + ","  + crSelectFunction.StartAge + "," + crSelectFunction.EndAge];
                        }
                        else
                        {
                            //PrevalenceDataSetGridType = Convert.ToInt32(fb.ExecuteScalar(_connection, CommandType.Text, commandText));
                            foreach (GridRelationship gRelationship in lstGridRelationshipAll)
                            {
                                if ((gRelationship.bigGridID == PrevalenceDataSetGridType && gRelationship.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID) || (gRelationship.smallGridID == PrevalenceDataSetGridType && gRelationship.bigGridID == CommonClass.GBenMAPGrid.GridDefinitionID))
                                {
                                    gridRelationShipPrevalence = gRelationship;
                                }
                            }
                            if (gridRelationShipPrevalence == null)
                            {
                                gridRelationShipPrevalence = new GridRelationship()
                                {
                                    bigGridID = PrevalenceDataSetGridType == 1 ? 1 : CommonClass.GBenMAPGrid.GridDefinitionID,
                                    smallGridID = PrevalenceDataSetGridType == 1 ? CommonClass.GBenMAPGrid.GridDefinitionID : PrevalenceDataSetGridType
                                };
                            }

                            //lstPrevalenceRateAttribute = Configuration.ConfigurationCommonClass.getIncidenceDataSetFromCRSelectFuntion(crSelectFunction, true, dicRace, dicEthnicity, dicGender, CommonClass.GBenMAPGrid.GridDefinitionID, gridRelationShipPrevalence);
                            //dicPrevalenceRateAttribute = new Dictionary<string, double>();
                            //foreach (IncidenceRateAttribute inc in lstPrevalenceRateAttribute)
                            //{
                            //    if (!dicPrevalenceRateAttribute.Keys.Contains(inc.Col + "," + inc.Row))
                            //    {
                            //        dicPrevalenceRateAttribute.Add(inc.Col + "," + inc.Row, inc.Value);

                            //    }
                            //}
                            dicPrevalenceRateAttribute = Configuration.ConfigurationCommonClass.getIncidenceDataSetFromCRSelectFuntionDicAllAge(dicPopulation,dicPopulationAge,dicPopulation12, crSelectFunction, true, dicRace, dicEthnicity, dicGender, CommonClass.GBenMAPGrid.GridDefinitionID, gridRelationShipPrevalence);
                            dicAllPrevalence.Add(crSelectFunction.PrevalenceDataSetID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "," +  crSelectFunction.StartAge + "," + crSelectFunction.EndAge, dicPrevalenceRateAttribute);

                        }
                    }
                    lstSetupVariable = null;
                     DicAllSetupVariableValues = new Dictionary<string, Dictionary<string, double>>();
                    if (crSelectFunction.VariableDataSetID > -1)
                    {
                        lbProgressBar.Text = "Loading Variable data for Study " + strAuthorCount;//+ crid+" of "+ crCount + "...";
                    Application.DoEvents();
                    lbProgressBar.Refresh();
                        lstSetupVariable = new List<SetupVariableJoinAllValues>();
                        Configuration.ConfigurationCommonClass.getSetupVariableNameListFromDatabaseFunction(CommonClass.GBenMAPGrid.GridDefinitionID, crSelectFunction.BenMAPHealthImpactFunction.Function, Configuration.ConfigurationCommonClass.LstSystemVariableName, ref lstSetupVariable);
                        Configuration.ConfigurationCommonClass.getSetupVariableNameListFromDatabaseFunction(CommonClass.GBenMAPGrid.GridDefinitionID, crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction, Configuration.ConfigurationCommonClass.LstSystemVariableName, ref lstSetupVariable);
                        //算系统变量

                        if (lstSetupVariable != null)
                        {
                            foreach (SetupVariableJoinAllValues sv in lstSetupVariable)
                            {
                                //dicSetupVariable.Add(sv..Col + "," + sv.Row, sv.Value);
                                Dictionary<string, double> dic = new Dictionary<string, double>();
                                foreach (SetupVariableValues svv in sv.lstValues)
                                {
                                    if (!dic.Keys.Contains(svv.Col + "," + svv.Row))
                                        dic.Add(svv.Col + "," + svv.Row, svv.Value);
                                }
                                DicAllSetupVariableValues.Add(sv.SetupVariableName, dic);
                            }
                            lstSetupVariable = null;
                        }
                    }
                   
                    var query = from a in CommonClass.LstBaseControlGroup where a.Pollutant.PollutantID == crSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantID select a;
                    if (query == null || query.Count() == 0)
                    {
                        return;
                    }
                    BaseControlGroup baseControlGroup = query.First();
                    AsyncDelegateCalculateOneCRSelectFunction dlgt = new AsyncDelegateCalculateOneCRSelectFunction(Configuration.ConfigurationCommonClass.CalculateOneCRSelectFunction);
                    //AsyncDelegateCalculateLstCRSelectFunction dlgt = new AsyncDelegateCalculateLstCRSelectFunction(CalculateLstCRSelectFunction);
                    //    //lstAsyns.Add("one");
                    //    ////string strFileName = CommonClass._modelBase.ModelPath.Substring(CommonClass._modelBase.ModelPath.LastIndexOf(@"\"), CommonClass._modelBase.ModelPath.LastIndexOf(".") - CommonClass._modelBase.ModelPath.LastIndexOf(@"\")-1);
                    //    ////GridLayerName = strFileName.Replace(@"\", "");
                    //    //// Initiate the asychronous call.   
                    //IAsyncResult ar = dlgt.BeginInvoke(lstCRSelectFunction, dicRace, dicEthnicity, dicGender,
                    //lstGridRelationshipAll, new AsyncCallback(outPut), dlgt);
                    double[] lhsResultArray = null;
                    if (!CommonClass.CRRunInPointMode)
                    {
                        int iRandomSeed = Convert.ToInt32(DateTime.Now.Hour + "" + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond);
                        if (CommonClass.CRSeeds != null && CommonClass.CRSeeds!=-1 )
                            iRandomSeed = Convert.ToInt32(CommonClass.CRSeeds);

                        //Console.WriteLine(DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Millisecond);
                        //if (crSelectFunction.lstLatinPoints != null && crSelectFunction.lstLatinPoints.Count() >=CommonClass.CRSeeds)
                        //{
                        //    lhsResultArray = crSelectFunction.lstLatinPoints[CommonClass.CRSeeds-1].values.ToArray();
                        //}
                        //else
                        //{
                        //    lhsResultArray = Configuration.ConfigurationCommonClass.getLHSArrayCRFunction(CommonClass.CRLatinHypercubePoints, crSelectFunction);
                        //    crSelectFunction.lstLatinPoints =new List<LatinPoints>();
                        //    crSelectFunction.lstLatinPoints.Add(new LatinPoints() { values = lhsResultArray.ToList(), });
                        //    for (int iSeed = 1; iSeed <10; iSeed++)
                        //    {

                        //        crSelectFunction.lstLatinPoints.Add(new LatinPoints() { values = Configuration.ConfigurationCommonClass.getLHSArrayCRFunction(CommonClass.CRLatinHypercubePoints, crSelectFunction).ToList() }); 
                        //    }
                        //}
                        //Console.WriteLine(DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Millisecond);
                        //crSelectFunctionCalculateValue.lstLatin = lhsResultArray.ToList();
                        lhsResultArray = Configuration.ConfigurationCommonClass.getLHSArrayCRFunctionSeed(CommonClass.CRLatinHypercubePoints, crSelectFunction,iRandomSeed);
                        crSelectFunction.lstLatinPoints = new List<LatinPoints>();
                        crSelectFunction.lstLatinPoints.Add(new LatinPoints() { values = lhsResultArray.ToList() }); 
                    }
                    if (!isBatch)
                    {
                        IAsyncResult ar = dlgt.BeginInvoke(crid, dicAllMetricDataBase[baseControlGroup.Pollutant.PollutantID], dicAllMetricDataControl[baseControlGroup.Pollutant.PollutantID]
                            , dicAll365Base[baseControlGroup.Pollutant.PollutantID], dicAll365Control[baseControlGroup.Pollutant.PollutantID], DicControlAll[baseControlGroup.Pollutant.PollutantID],
                            DicAllSetupVariableValues, dicPopulation, dicPopulationAge, dicIncidenceRateAttribute, dicPrevalenceRateAttribute, incidenceDataSetGridType, PrevalenceDataSetGridType, dicRace, dicEthnicity, dicGender, CommonClass.CRThreshold, CommonClass.CRLatinHypercubePoints,
                            CommonClass.CRRunInPointMode, lstGridRelationshipAll, crSelectFunction, baseControlGroup, null, CommonClass.BenMAPPopulation, lhsResultArray, new AsyncCallback(outPut), dlgt);
                    }
                    else
                    {
                        Configuration.ConfigurationCommonClass.CalculateOneCRSelectFunction(crid, dicAllMetricDataBase[baseControlGroup.Pollutant.PollutantID], dicAllMetricDataControl[baseControlGroup.Pollutant.PollutantID]
                            , dicAll365Base[baseControlGroup.Pollutant.PollutantID], dicAll365Control[baseControlGroup.Pollutant.PollutantID], DicControlAll[baseControlGroup.Pollutant.PollutantID],
                            DicAllSetupVariableValues, dicPopulation, dicPopulationAge, dicIncidenceRateAttribute, dicPrevalenceRateAttribute, incidenceDataSetGridType, PrevalenceDataSetGridType, dicRace, dicEthnicity, dicGender, CommonClass.CRThreshold, CommonClass.CRLatinHypercubePoints,
                            CommonClass.CRRunInPointMode, lstGridRelationshipAll, crSelectFunction, baseControlGroup, null, CommonClass.BenMAPPopulation, lhsResultArray);                    
 
                    }
                        crid = crid + 1; ;
                    this.pBarCR.Value++;
                    //    i = 0;
                    //}
                    //i++;
                    //CRSelectFunctionCalculateValue crv = dlgt.EndInvoke(ar);
                    //lstCRSelectFunctionCalculateValue.Add(crv);
                }
                 * */
                if (isBatch)
                {
                    afterOutput();
                }

            }
            catch (Exception ex)
            {
                btAddCRFunctions.Enabled = true;
                btAdvanced.Enabled = true;
                btDelSelectMethod.Enabled = true;
                btnCancel.Enabled = true;
                btnRun.Enabled = true;
                btnSave.Enabled = true;
                cbDataSet.Enabled = true;
                cbEndPointGroup.Enabled = true;
                cbGroups.Enabled = true;

                GC.Collect();
                //WaitClose();
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                //WaitClose();
                //this.DialogResult = System.Windows.Forms.DialogResult.OK;
                // btnRun.Enabled = true;
            }
        }
        //private void CalculateLstCRSelectFunction(int incidenceDataSetGridType, int PrevalenceDataSetGridType, List<CRSelectFunction> lstCRSelectFunction, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, List<GridRelationship> lstGridRelationshipAll)
        //{
        //    foreach (CRSelectFunction crSelectFunction in lstCRSelectFunction)
        //    {
        //        Configuration.ConfigurationCommonClass.CalculateOneCRSelectFunction( incidenceDataSetGridType, PrevalenceDataSetGridType,dicRace, dicEthnicity, dicGender, CommonClass.CRThreshold, CommonClass.CRLatinHypercubePoints,
        //           CommonClass.CRRunInPointMode, lstGridRelationshipAll, crSelectFunction, CommonClass.BaseControlCRSelectFunction.BaseControlGroup, null, CommonClass.BenMAPPopulation);
        //    }

        //}
        List<CRSelectFunctionCalculateValue> lstCRSelectFunctionCalculateValue = new List<CRSelectFunctionCalculateValue>();
        private List<string> lstAsyns = new List<string>();
        private void afterOutput()
        {

            //CommonClass.BaseControlCRSelectFunctionCalculateValue = new BaseControlCRSelectFunctionCalculateValue();
            //CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup = CommonClass.BaseControlCRSelectFunction.BaseControlGroup;
            //CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue = lstCRSelectFunctionCalculateValue;
            // str = str + ":" + DateTime.Now.ToString();
            if (DicAllSetupVariableValues != null)
            {
                for (int iKey = 0; iKey < DicAllSetupVariableValues.Count; iKey++)
                {
                    //string s = ;
                    if (DicAllSetupVariableValues[DicAllSetupVariableValues.Keys.ToArray()[iKey]] != null)
                        DicAllSetupVariableValues[DicAllSetupVariableValues.Keys.ToArray()[iKey]].Clear();
                    DicAllSetupVariableValues[DicAllSetupVariableValues.Keys.ToArray()[iKey]] = null;
                }
            }
            DicAllSetupVariableValues = null;
            if (dicAllIncidence != null)
            {
                for (int iKey = 0; iKey < dicAllIncidence.Count; iKey++)
                {
                    if (dicAllIncidence[dicAllIncidence.Keys.ToArray()[iKey]] != null)
                        dicAllIncidence[dicAllIncidence.Keys.ToArray()[iKey]].Clear();
                    dicAllIncidence[dicAllIncidence.Keys.ToArray()[iKey]] = null;
                }
            }
            dicAllIncidence = null;
            if (dicAllPrevalence != null)
            {
                for (int iKey = 0; iKey < dicAllPrevalence.Count; iKey++)
                {
                    if (dicAllPrevalence[dicAllPrevalence.Keys.ToArray()[iKey]] != null)
                        dicAllPrevalence[dicAllPrevalence.Keys.ToArray()[iKey]].Clear();
                    dicAllPrevalence[dicAllPrevalence.Keys.ToArray()[iKey]] = null;
                }
            }
            dicAllPrevalence = null;
            if (dicALlPopulation != null)
            {
                for (int iKey = 0; iKey < dicALlPopulation.Count; iKey++)
                {
                    if (dicALlPopulation[dicALlPopulation.Keys.ToArray()[iKey]] != null)
                        dicALlPopulation[dicALlPopulation.Keys.ToArray()[iKey]].Clear();
                    dicALlPopulation[dicALlPopulation.Keys.ToArray()[iKey]] = null;
                }
            }
            dicALlPopulation = null;
            if (dicALlPopulationAge != null)
            {
                for (int iKey = 0; iKey < dicALlPopulationAge.Count; iKey++)
                {
                    if (dicALlPopulationAge[dicALlPopulationAge.Keys.ToArray()[iKey]] != null)
                        dicALlPopulationAge[dicALlPopulationAge.Keys.ToArray()[iKey]].Clear();
                    dicALlPopulationAge[dicALlPopulationAge.Keys.ToArray()[iKey]] = null;
                }
            }
            if (Configuration.ConfigurationCommonClass.DicGrowth != null)
                Configuration.ConfigurationCommonClass.DicGrowth.Clear();
            Configuration.ConfigurationCommonClass.DicGrowth = null;
            if (Configuration.ConfigurationCommonClass.DicWeight != null)
                Configuration.ConfigurationCommonClass.DicWeight.Clear();
            Configuration.ConfigurationCommonClass.DicWeight = null;
            dicALlPopulationAge = null;
            GC.Collect();

            //WaitClose();
            try
            {
                this.pBarCR.Value = this.pBarCR.Maximum;
                //this.lbProgressBar.Text = "Finished!";
                DateTime dtNow = DateTime.Now;
                TimeSpan ts = dtNow.Subtract(dtRunStart);
                //MessageBox.Show("HIF running time: " + ts.Hours + " hours " + ts.Minutes + " minutes " + ts.Seconds + " seconds!");
                this.lbProgressBar.Text = "Processing complete. HIF processing time: " + ts.Hours + " hours " + ts.Minutes + " minutes " + ts.Seconds + " seconds.";
                CommonClass.BaseControlCRSelectFunctionCalculateValue.lstLog = new List<string>();
                CommonClass.BaseControlCRSelectFunctionCalculateValue.lstLog.Add(this.lbProgressBar.Text);
                //-----赋值CreateTime
                CommonClass.BaseControlCRSelectFunctionCalculateValue.CreateTime = DateTime.Now;
                if (_filePath != "")
                {
                    Configuration.ConfigurationCommonClass.SaveCRFRFile(CommonClass.BaseControlCRSelectFunctionCalculateValue, _filePath);
                }
                Thread.Sleep(3000);
                GC.Collect();
                //CommonClass.Connection.Close();
            }
            catch
            {
            }

            //CommonClass.Connection = CommonClass.getNewConnection();

        }
        private void outPut(IAsyncResult ar)
        {
            try
            {
                lock (this)
                {

                    //CRSelectFunctionCalculateValue crv = (ar.AsyncState as AsyncDelegateCalculateOneCRSelectFunction).EndInvoke(ar);
                    //if (ar != null)
                    //    lstCRSelectFunctionCalculateValue.Add(crv);
                    //lstAsyns.Remove(crv.CRSelectFunction.BenMAPHealthImpactFunction.ID.ToString());
                    //object o= (ar.AsyncState as AsyncDelegateCalculateOneCRSelectFunction).Method.GetParameters()[0].DefaultValue;
                    lstAsyns.RemoveAt(0);
                    int icount = CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count - lstAsyns.Count;
                    this.pBarCR.Value++;
                    //lock (waitMess)
                    //{
                    //    waitMess.Msg = "Finish process cells for study " + icount + " of " + CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count + "...";
                    //}
                    GC.Collect();
                    //WaitChangeMsg("Finish process cells for study " + icount + " of " + CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count + "...");
                    Application.DoEvents();
                    for (int iSelect = 0; iSelect < icount; iSelect++)
                    {
                        olvSelected.GetItem(iSelect).UseItemStyleForSubItems = true;// olvSimple.HighlightBackgroundColor;
                        olvSelected.GetItem(iSelect).Font = new Font(olvSelected.GetItem(iSelect).Font, FontStyle.Bold);
                    }
                    olvSelected.Refresh();
                    string strAuthorCount = icount.ToString() + " of " + CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count.ToString();// + ".";// +"...";// crSelectFunction.BenMAPHealthImpactFunction.Author + " " + crid.ToString() + " of " + CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count.ToString() + "...";
                    while (strAuthorCount.Length < CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count.ToString().Length * 2 + 4)
                    {
                        strAuthorCount = " " + strAuthorCount;

                    }
                    // olvSelected.RedrawItems(0, olvSelected.Items.Count - 1, false); 
                    // this.lbProgressBar.Text = "Finishing processing cells for study " + CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction[icount - 1].BenMAPHealthImpactFunction.Author + " " + icount.ToString() + " of " + CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count.ToString() + "...";
                    string sProgressBar = String.Format("Processing. Completed {0} Health Impact Functions.", strAuthorCount);
                    while (sProgressBar.Length < 57)
                    {
                        sProgressBar = sProgressBar + " ";
                    }
                    this.lbProgressBar.Text = sProgressBar;
                    this.lbProgressBar.Text = sProgressBar;
                    Application.DoEvents();
                    lbProgressBar.Refresh();
                    if (lstAsyns.Count == 0)
                    {
                        //CommonClass.BaseControlCRSelectFunctionCalculateValue = new BaseControlCRSelectFunctionCalculateValue();
                        //CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup = CommonClass.BaseControlCRSelectFunction.BaseControlGroup;
                        //CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue = lstCRSelectFunctionCalculateValue;
                        // str = str + ":" + DateTime.Now.ToString();
                        //if (DicAllSetupVariableValues != null)
                        //{
                        //   for(int iKey=0;iKey<DicAllSetupVariableValues.Count;iKey++)
                        //    {
                        //        //string s = ;
                        //       if(DicAllSetupVariableValues[DicAllSetupVariableValues.Keys.ToArray()[iKey]]!=null)
                        //        DicAllSetupVariableValues[DicAllSetupVariableValues.Keys.ToArray()[iKey]].Clear();
                        //        DicAllSetupVariableValues[DicAllSetupVariableValues.Keys.ToArray()[iKey]] = null;
                        //    }
                        //}
                        // DicAllSetupVariableValues =null;
                        // if (dicAllIncidence != null)
                        // {
                        //     for (int iKey = 0; iKey < dicAllIncidence.Count; iKey++)
                        //     {
                        //         if(dicAllIncidence[dicAllIncidence.Keys.ToArray()[iKey]]!=null)
                        //         dicAllIncidence[dicAllIncidence.Keys.ToArray()[iKey]].Clear();
                        //         dicAllIncidence[dicAllIncidence.Keys.ToArray()[iKey]] = null;
                        //     }
                        // }
                        // dicAllIncidence = null;
                        // if (dicAllPrevalence != null)
                        // {
                        //     for (int iKey = 0; iKey < dicAllPrevalence.Count; iKey++)
                        //     {
                        //         if (dicAllPrevalence[dicAllPrevalence.Keys.ToArray()[iKey]] != null)
                        //         dicAllPrevalence[dicAllPrevalence.Keys.ToArray()[iKey]].Clear();
                        //         dicAllPrevalence[dicAllPrevalence.Keys.ToArray()[iKey]] = null;
                        //     }
                        // }
                        // dicAllPrevalence = null;
                        // if (dicALlPopulation != null)
                        // {
                        //     for (int iKey = 0; iKey < dicALlPopulation.Count; iKey++)
                        //     {
                        //         if (dicALlPopulation[dicALlPopulation.Keys.ToArray()[iKey]] != null)
                        //         dicALlPopulation[dicALlPopulation.Keys.ToArray()[iKey]].Clear();
                        //         dicALlPopulation[dicALlPopulation.Keys.ToArray()[iKey]] = null;
                        //     }
                        // }
                        // dicALlPopulation = null;
                        // if (dicALlPopulationAge != null)
                        // {
                        //     for (int iKey = 0; iKey < dicALlPopulationAge.Count; iKey++)
                        //     {
                        //         if (dicALlPopulationAge[dicALlPopulationAge.Keys.ToArray()[iKey]] != null)
                        //         dicALlPopulationAge[dicALlPopulationAge.Keys.ToArray()[iKey]].Clear();
                        //         dicALlPopulationAge[dicALlPopulationAge.Keys.ToArray()[iKey]] = null;
                        //     }
                        // }
                        //if(Configuration.ConfigurationCommonClass.DicGrowth!=null)
                        // Configuration.ConfigurationCommonClass.DicGrowth.Clear();
                        // Configuration.ConfigurationCommonClass.DicGrowth = null;
                        // if (Configuration.ConfigurationCommonClass.DicWeight != null)
                        // Configuration.ConfigurationCommonClass.DicWeight.Clear();
                        // Configuration.ConfigurationCommonClass.DicWeight = null;
                        // dicALlPopulationAge = null;
                        // GC.Collect();

                        ////WaitClose();
                        //try
                        //{
                        //    this.pBarCR.Value = this.pBarCR.Maximum;
                        //    //this.lbProgressBar.Text = "Finished!";
                        //    DateTime dtNow = DateTime.Now;
                        //    TimeSpan ts = dtNow - dtRunStart;
                        //    //MessageBox.Show("HIF running time: " + ts.Hours + " hours " + ts.Minutes + " minutes " + ts.Seconds + " seconds!");
                        //    this.lbProgressBar.Text = "Finished!HIF process time: " + ts.Hours + " hours " + ts.Minutes + " minutes " + ts.Seconds + " seconds!";
                        //    CommonClass.BaseControlCRSelectFunctionCalculateValue.lstLog = new List<string>();
                        //    CommonClass.BaseControlCRSelectFunctionCalculateValue.lstLog.Add(this.lbProgressBar.Text);
                        //    if (_filePath != "")
                        //    {
                        //        Configuration.ConfigurationCommonClass.SaveCRFRFile(CommonClass.BaseControlCRSelectFunctionCalculateValue, _filePath);
                        //    }
                        //    Thread.Sleep(3000);
                        //    GC.Collect();
                        //    CommonClass.Connection.Close();
                        //}
                        //catch
                        //{ 
                        //}

                        //CommonClass.Connection = CommonClass.getNewConnection();
                        afterOutput();
                        this.DialogResult = DialogResult.OK;

                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                // this.DialogResult = DialogResult.;
            }

        }
        public delegate void AsyncDelegateCalculateLstCRSelectFunction(List<CRSelectFunction> lstCRSelectFunction, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, List<GridRelationship> lstGridRelationshipAll);
        public delegate void AsyncDelegateCalculateOneCRSelectFunction(string crid,List<string> lstAllAgeID, Dictionary<string, double> dicAge, Dictionary<string, Dictionary<string, float>> dicBaseMetricData, Dictionary<string, Dictionary<string, float>> dicControlMetricData,
           Dictionary<string, Dictionary<string, List<float>>> dicBase365, Dictionary<string, Dictionary<string, List<float>>> dicControl365,
           Dictionary<string, ModelResultAttribute> dicControl, Dictionary<string, Dictionary<string, double>> DicAllSetupVariableValues, Dictionary<string, float> dicPopulationAllAge, Dictionary<string, double> dicIncidenceRateAttribute, Dictionary<string, double> dicPrevalenceRateAttribute, int incidenceDataSetGridType, int PrevalenceDataSetGridType, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, double Threshold, int LatinHypercubePoints, bool RunInPointMode, List<GridRelationship> lstGridRelationship, CRSelectFunction crSelectFunction, BaseControlGroup baseControlGroup, List<RegionTypeGrid> lstRegionTypeGrid, BenMAPPopulation benMAPPopulation, double[] lhsResultArray);

        private void textBoxFilterSimple_TextChanged(object sender, EventArgs e)
        {
            this.TimedFilter(this.olvSimple, textBoxFilterSimple.Text);
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
                olv.DefaultRenderer = new HighlightTextRenderer { Filter = filter, UseGdiTextRendering = true };
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
            if (objects == null)
                this.toolStripStatusLabel1.Text =
                    String.Format("Filtered in {0}ms", stopWatch.ElapsedMilliseconds);
            else
                this.toolStripStatusLabel1.Text =
                    String.Format("Filtered {0} items down to {1} items in {2}ms",
                                  objects.Count,
                                  olv.Items.Count,
                                  stopWatch.ElapsedMilliseconds);
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
                    //ShowWaitMess();
                    System.Threading.Thread upgradeThread = null;
                    upgradeThread = new System.Threading.Thread(new System.Threading.ThreadStart(ShowWaitMess));
                    upgradeThread.Start();
                    //upgradeThread.IsBackground = true;
                }
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }
        private delegate void CloseFormDelegate();
        private delegate void ChangeDelegate(string msg);
        //--关闭等待窗体 
        public void WaitClose()
        {
            //同步到主线程上
            if (waitMess.InvokeRequired)
                waitMess.Invoke(new CloseFormDelegate(DoCloseJob));
            else
                DoCloseJob();
        }

        public void WaitChangeMsg(string msg)
        {
            try
            {
                if (waitMess.InvokeRequired)
                    waitMess.Invoke(new ChangeDelegate(DoChange), msg);
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }
        private void DoChange(string msg)
        {
            try
            {
                if (!waitMess.IsDisposed)
                {
                    if (waitMess.Created)
                    {
                        //sFlog = true;
                        waitMess.Msg = msg;
                    }
                }
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
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

        public void btDelSelectMethod_Click(object sender, EventArgs e)
        {// Todo:陈志润20120115删除
            try
            {
                //foreach (CRSelectFunction cr in olvSelected.SelectedObjects)
                //{
                //    //isHIFChanged = true;
                //    lstCRSelectFunction.Remove(cr);
                //    if ((CommonClass.lstIncidencePoolingAndAggregation != null) && (CommonClass.lstIncidencePoolingAndAggregation.Count > 0))
                //    {
                //        if (CommonClass.LstDelCRFunction == null || CommonClass.LstDelCRFunction.Count == 0)
                //        {
                //            CommonClass.LstDelCRFunction = new List<CRSelectFunction>();
                //            CommonClass.LstDelCRFunction.Add(cr);
                //        }
                //        else
                //        {
                //            foreach (CRSelectFunction crFunction in CommonClass.LstDelCRFunction)
                //            {
                //                if ((cr.CRID == crFunction.CRID) && (cr.BenMAPHealthImpactFunction.ID == crFunction.BenMAPHealthImpactFunction.ID)) { continue; }
                //                CommonClass.LstDelCRFunction.Add(cr);
                //            }
                //        }
                //    }
                //}
                for (int i = olvSelected.SelectedObjects.Count - 1; i >= 0; i--)
                {
                    CRSelectFunction cr = olvSelected.SelectedObjects[i] as CRSelectFunction;
                    if (cr != null) { lstCRSelectFunction.Remove(cr); }
                    if ((CommonClass.lstIncidencePoolingAndAggregation != null) && (CommonClass.lstIncidencePoolingAndAggregation.Count > 0))
                    {
                        if (CommonClass.LstDelCRFunction == null || CommonClass.LstDelCRFunction.Count == 0)
                        {
                            CommonClass.LstDelCRFunction = new List<CRSelectFunction>();
                            CommonClass.LstDelCRFunction.Add(cr);
                        }
                        else
                        {
                            foreach (CRSelectFunction crFunction in CommonClass.LstDelCRFunction)
                            {
                                if ((cr.CRID == crFunction.CRID) && (cr.BenMAPHealthImpactFunction.ID == crFunction.BenMAPHealthImpactFunction.ID)) { continue; }
                                CommonClass.LstDelCRFunction.Add(cr);
                                break;
                            }
                        }
                    }
                }
                this.olvSelected.SetObjects(lstCRSelectFunction);
                gBSelectedHealthImpactFuntion.Text = "Selected Health Impact Functions (" + lstCRSelectFunction.Count + ")";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string _filePath = "";

            //弹出窗口让其保存---------------------------------------------------
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CFG files (*.cfgx)|*.cfgx";
            //sfd.FilterIndex = 2;
            sfd.RestoreDirectory = true;
            sfd.InitialDirectory = CommonClass.ResultFilePath + @"\Result\CFG";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _filePath = sfd.FileName;
                //-----------do save---------------
                BaseControlCRSelectFunction bc = new BaseControlCRSelectFunction();
                bc.CRLatinHypercubePoints = CommonClass.CRLatinHypercubePoints;
                bc.CRRunInPointMode = CommonClass.CRRunInPointMode;
                bc.CRThreshold = CommonClass.CRThreshold;
                bc.CRSeeds = CommonClass.CRSeeds;
                bc.RBenMapGrid = CommonClass.RBenMAPGrid;

                bc.BenMAPPopulation = CommonClass.BenMAPPopulation;
                bc.BaseControlGroup = CommonClass.LstBaseControlGroup;
                bc.lstCRSelectFunction = olvSelected.Objects as List<CRSelectFunction>;
                bc.CreateTime = DateTime.Now;
                Configuration.ConfigurationCommonClass.SaveCFGFile(bc, _filePath);
                MessageBox.Show("File saved.", "File saved");
            }

            //_filePath = sfd.FileName.Substring(0, sfd.FileName.LastIndexOf(@"\") + 1);

        }

        private void btLoadCR_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Filter = "CFG files (*.cfgx)|*.cfgx ";
            sfd.FilterIndex = 2;
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string _filePath = sfd.FileName;
                //-----------do load---------------
                BaseControlCRSelectFunction bc = new BaseControlCRSelectFunction();
                bc.BaseControlGroup = CommonClass.LstBaseControlGroup;
                bc.lstCRSelectFunction = olvSelected.Objects as List<CRSelectFunction>;
            }

        }

        private void olvSimple_IsHyperlink(object sender, IsHyperlinkEventArgs e)
        {
            //  Help.ShowHelp(new Control(), Application.StartupPath + @"\Data\QuickStartGuide.chm", "Select Health Impact Function"); 

        }

        private void olvSimple_CellClick(object sender, CellClickEventArgs e)
        {
            //Todo:陈志润
            base.OnClick(e);
            //base.Click(sender, e);
            if (e.Column != null && e.Column.Hyperlink)
            {
                switch (e.Column.Text)
                {
                    case "DataSet":
                        Help.ShowHelp(this, Application.StartupPath + @"\Data\QuickStartGuide.chm", "select_health_impact_function.htm");
                        break;
                }
            }
        }

        private void btAdvanced_Click(object sender, EventArgs e)
        {
            Form frm = new LatinHypercubePoints();
            (frm as LatinHypercubePoints).LatinHypercubePointsCount = CommonClass.CRLatinHypercubePoints;
            (frm as LatinHypercubePoints).IsRunInPointMode = CommonClass.CRRunInPointMode;
            (frm as LatinHypercubePoints).Threshold = CommonClass.CRThreshold;
            DialogResult rtn = frm.ShowDialog();
            if (rtn != DialogResult.OK) { return; }
            CommonClass.CRLatinHypercubePoints = (frm as LatinHypercubePoints).LatinHypercubePointsCount;
            CommonClass.CRRunInPointMode = (frm as LatinHypercubePoints).IsRunInPointMode;
            CommonClass.CRThreshold = (frm as LatinHypercubePoints).Threshold;
        }
        //------------majie------------
        private void olvSelected_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
            {
                btDelSelectMethod_Click(sender, e);
            }
        }

        private void olvSelected_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            int icount = CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count - lstAsyns.Count;
            if (e.ItemIndex < icount)
            {
                e.Item.ForeColor = Color.Black;
                e.Item.Font = new Font(e.Item.Font, FontStyle.Bold);
 
            }

        }
    }

    public class HealthImapctDropSink : SimpleDropSink
    {
        public HealthImpactFunctions myHealthImpactFunctions;
        /// <summary>
        /// Create a RearrangingDropSink
        /// </summary>
        public HealthImapctDropSink()
        {
            this.CanDropBetween = true;
            this.CanDropOnBackground = true;
            this.CanDropOnItem = false;
        }

        /// <summary>
        /// Create a RearrangingDropSink
        /// </summary>
        /// <param name="acceptDropsFromOtherLists"></param>
        public HealthImapctDropSink(bool acceptDropsFromOtherLists, HealthImpactFunctions healthImpactFunctions)
            : this()
        {
            this.AcceptExternal = acceptDropsFromOtherLists;
            myHealthImpactFunctions = healthImpactFunctions;
        }

        /// <summary>
        /// Trigger OnModelCanDrop
        /// </summary>
        /// <param name="args"></param>
        protected override void OnModelCanDrop(ModelDropEventArgs args)
        {
            base.OnModelCanDrop(args);
            //Todo:陈志润
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
            // Todo:陈志润
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
                    //if(this.ListView.Name!="olvSelected")
                    //this.ListView.MoveObjects(args.DropTargetIndex, args.SourceModels);
                    if (this.ListView.Name == "olvSelected" && args.SourceListView.Name != "olvSelected")
                    {
                        //this.ListView.AddObjects(args.SourceModels);
                        myHealthImpactFunctions.btAddCRFunctions_Click(null, null);
                        //foreach (BenMAPHealthImpactFunction cr in args.SourceModels)
                        //{
                        //    myHealthImpactFunctions.olvSimple.AddObject(cr );
                        //}

                    }
                    else
                    {
                        //myHealthImpactFunctions.btDelSelectMethod_Click(null, null);


                    }
                    break;
                case DropTargetLocation.BelowItem:
                    //if (this.ListView.Name != "olvSelected")
                    //this.ListView.MoveObjects(args.DropTargetIndex + 1, args.SourceModels);
                    if (this.ListView.Name == "olvSelected" && args.SourceListView.Name != "olvSelected")
                    {
                        //this.ListView.AddObjects(args.SourceModels);
                        myHealthImpactFunctions.btAddCRFunctions_Click(null, null);
                        //foreach (BenMAPHealthImpactFunction cr in args.SourceModels)
                        //{
                        //    myHealthImpactFunctions.olvSimple.AddObject(cr);
                        //}
                    }
                    else
                    {
                        //myHealthImpactFunctions.btDelSelectMethod_Click(null, null);

                    }
                    break;
                case DropTargetLocation.Background:
                    if (this.ListView.Name == "olvSelected" && args.SourceListView.Name != "olvSelected")
                    {
                        //this.ListView.AddObjects(args.SourceModels);
                        myHealthImpactFunctions.btAddCRFunctions_Click(null, null);
                        //foreach (BenMAPHealthImpactFunction cr in args.SourceModels)
                        //{
                        //    myHealthImpactFunctions.olvSimple.AddObject(cr);
                        //}
                    }
                    else
                    {
                        //myHealthImpactFunctions.btDelSelectMethod_Click(null, null);

                    }
                    break;
                default:
                    return;
            }


        }
    }
}
