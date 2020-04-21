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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BenMAP.Tools;

namespace BenMAP
{
    public partial class HealthImpactFunctions : FormBase
    {
        private bool isload = false;
        ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
        private List<BenMAPHealthImpactFunction> lstBenMAPHealthImpactFunction = new List<BenMAPHealthImpactFunction>();
        private List<BenMAPHealthImpactFunction> lstBenMAPHealthImpactFunctionSelected = new List<BenMAPHealthImpactFunction>();
        private List<CRSelectFunction> lstCRSelectFunction = new List<CRSelectFunction>();
        private Dictionary<string, int> DicRace = new Dictionary<string, int>(); private Dictionary<string, int> DicGender = new Dictionary<string, int>(); private Dictionary<string, int> DicEthnicity = new Dictionary<string, int>(); private Dictionary<string, int> DicIncidenceDataSet = Configuration.ConfigurationCommonClass.getAllIncidenceDataSet(CommonClass.MainSetup.SetupID);
        private Dictionary<string, int> DicVariableDataSet = Configuration.ConfigurationCommonClass.getAllVariableDataSet(CommonClass.MainSetup.SetupID);
        private static int _maxCRID;
        private bool hasGeoAreaInfoShown = false;
        private static bool _dailyAQmissing;

        private Configuration.ConfigurationCommonClass.geographicAreaAnalysisMode geoMode = Configuration.ConfigurationCommonClass.geographicAreaAnalysisMode.allUnconstrained;

        public static bool dailyAQmissing => _dailyAQmissing;

        public static int MaxCRID
        {
            get { return HealthImpactFunctions._maxCRID; }
            set { HealthImpactFunctions._maxCRID = value; }
        }
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
        public BaseControlCRSelectFunction BaseControlCRSelectFunctionOld;
        private void HealthImpactFunctions_Load(object sender, EventArgs e)
        {
            CommonClass.SetupOLVEmptyListOverlay(this.olvSimple.EmptyListMsgOverlay as BrightIdeasSoftware.TextOverlay);
            CommonClass.SetupOLVEmptyListOverlay(this.olvSelected.EmptyListMsgOverlay as BrightIdeasSoftware.TextOverlay);
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
            olvSimple.CheckBoxes = false;
            if (CommonClass.BaseControlCRSelectFunction != null && CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction != null)
            {
                BaseControlCRSelectFunctionOld = new BaseControlCRSelectFunction();
                BaseControlCRSelectFunctionOld.BaseControlGroup = CommonClass.BaseControlCRSelectFunction.BaseControlGroup;
                BaseControlCRSelectFunctionOld.BenMAPPopulation = CommonClass.BaseControlCRSelectFunction.BenMAPPopulation;
                BaseControlCRSelectFunctionOld.CRLatinHypercubePoints = CommonClass.BaseControlCRSelectFunction.CRLatinHypercubePoints;
                BaseControlCRSelectFunctionOld.CRDefaultMonteCarloIterations = CommonClass.BaseControlCRSelectFunction.CRDefaultMonteCarloIterations;
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
                        GeographicAreaName = cr.GeographicAreaName,
                        GeographicAreaID = cr.GeographicAreaID,
                        GeographicAreaFeatureID = cr.GeographicAreaFeatureID,
                        PrevalenceDataSetID = cr.PrevalenceDataSetID,
                        PrevalenceDataSetName = cr.PrevalenceDataSetName,
                        Race = cr.Race,
                        StartAge = cr.StartAge,
                        VariableDataSetID = cr.VariableDataSetID,
                        VariableDataSetName = cr.VariableDataSetName
                    });

                }
                setGeoAreaMode(CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction);
                this.olvSelected.Objects = CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction;
                gBSelectedHealthImpactFuntion.Text = "Selected Health Impact Functions (" + CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count + ")";
                lstCRSelectFunction = CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction;
            }
            olvSelected.IsSimpleDropSink = true;
            olvSimple.IsSimpleDropSink = true;
            olvSelected.DropSink = new HealthImapctDropSink(true, this);
            olvSimple.DropSink = new HealthImapctDropSink(true, this);
            olvSimple.Sort(1);
            commandText = string.Format("select RaceID,RaceName from Races where (raceid in (select raceid from Popconfigracemap where populationconfigurationid = {0})) or racename='' or lower(racename)='all'", CommonClass.BenMAPPopulation.PopulationConfiguration);
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
                    string strMetric = "";
                    foreach (BenMAPPollutant benMAPPollutant in CommonClass.LstPollutant)
                    {
                        strPollutants = strPollutants + "," + benMAPPollutant.PollutantID;
                    }
                    strPollutants = strPollutants.Substring(1);
                    foreach (BaseControlGroup bcg in CommonClass.LstBaseControlGroup)
                    {
                        int pollutantID = bcg.Pollutant.PollutantID;
                        List<int> lstMetricID = new List<int>();
                        if (bcg.Base.ModelResultAttributes.Count > 0)
                        {
                            foreach (string m in bcg.Base.ModelResultAttributes[0].Values.Keys)
                            {
                                string metric = m.Split(',')[0];
                                commandText = string.Format("select metricid from metrics where pollutantid={0} and metricname='{1}'", pollutantID, metric);
                                int metricid = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                                if (metricid > 0 && !lstMetricID.Contains(metricid))
                                {
                                    strMetric = strMetric + "," + metricid;
                                    lstMetricID.Add(metricid);
                                }
                            }
                        }
                    }
                    strMetric = strMetric.Substring(1);
                    if (Convert.ToInt32(drv["EndPointGroupID"]) == -1)
                    {
                        commandText = string.Format(" select CRFunctionID from CRFunctions where CRFunctionDataSetID={0} and PollutantID in ({1}) and MetricID in ({2})", drvDataSet["CRFunctionDatasetID"], strPollutants, strMetric);
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
                        commandText = string.Format(" select CRFunctionID from CRFunctions where CRFunctionDataSetID={0} and PollutantID in ({1}) and EndPointGroupID={2} and MetricID in ({3})", drvDataSet["CRFunctionDatasetID"], strPollutants, Convert.ToInt32(Convert.ToInt32(drv["EndPointGroupID"])), strMetric);
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

        public void btAddCRFunctions_Click(object sender, EventArgs e)
        {
            try
            {

                Boolean missingIncData = false; //If selected functions are missing incidence/prevalence data
                _dailyAQmissing = false; //If AQ data is is compatible with HIF (e.g. annual vs daily). 
                Boolean noPopHIF = false;

                foreach (BenMAPHealthImpactFunction benMAPHealthImpactFunction in olvSimple.SelectedObjects)
                {
                    CRSelectFunction crSelectFunction = new CRSelectFunction();
                    crSelectFunction.BenMAPHealthImpactFunction = benMAPHealthImpactFunction;
                    string commandText = "";
                    DataSet ds = null;
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    if (!crSelectFunction.BenMAPHealthImpactFunction.Function.ToLower().Contains("pop") || !crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction.ToLower().Contains("pop"))
                    {
                        noPopHIF = true;
                    }
                    if (crSelectFunction.BenMAPHealthImpactFunction.Function.ToLower().Contains("incidence") || crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction.ToLower().Contains("incidence"))
                    {
                        commandText = string.Format("select distinct a.IncidenceDataSetID,IncidenceDataSetName from IncidenceDataSets a,IncidenceRates b where a.IncidenceDataSetID=b.IncidenceDataSetID and  SetupID={0} and b.EndPointGroupID={1} and Prevalence='F' and (b.EndPointID={2} or b.EndPointID=99 or b.EndPointID=100 or b.EndPointID=102)", CommonClass.MainSetup.SetupID, crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID, crSelectFunction.BenMAPHealthImpactFunction.EndPointID);
                        ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                        int drNextYear, drYear = 0;
                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            missingIncData = true; //One or more functions call missing incidence/prevalence data.
                        }
                        else
                            try
                            {
                                foreach (DataRow dr in ds.Tables[0].Rows)
                                {
                                    // This logic selects the nearest year's incidence dataset to the selected population data
                                    if (string.IsNullOrEmpty(crSelectFunction.IncidenceDataSetName))
                                    {
                                        crSelectFunction.IncidenceDataSetID = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                                        crSelectFunction.IncidenceDataSetName = ds.Tables[0].Rows[0][1].ToString();
                                        drYear = int.Parse(dr["IncidenceDataSetName"].ToString().Substring(dr["IncidenceDataSetName"].ToString().IndexOf("(") + 1, dr["IncidenceDataSetName"].ToString().IndexOf(")") - dr["IncidenceDataSetName"].ToString().IndexOf("(") - 1));

                                    }
                                    else
                                    {
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
                        ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            crSelectFunction.PrevalenceDataSetID = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                            crSelectFunction.PrevalenceDataSetName = ds.Tables[0].Rows[0][1].ToString();
                        }
                        else
                        {
                            missingIncData = true; //One or more functions call missing incidence/prevalence data.
                        }
                    }
                    //Check if daily data is required and available
                    if (benMAPHealthImpactFunction.MetricStatistic == MetricStatic.None) //Daily or seasonal function
                    {
                        Metric metric = benMAPHealthImpactFunction.Metric;
                        SeasonalMetric seasonalMetric = benMAPHealthImpactFunction.SeasonalMetric;
                        foreach (BaseControlGroup bcg in CommonClass.LstBaseControlGroup)
                        {
                            try
                            {
                                if (bcg.Base is MonitorDataLine) // monitor data
                                {
                                    MonitorDataLine bcgMonitorBase = bcg.Base as MonitorDataLine;
                                    if (seasonalMetric != null && !bcgMonitorBase.MonitorValues[0].dicMetricValues365.ContainsKey(seasonalMetric.SeasonalMetricName)) //Seasonal function, missing seasonal AQ
                                    {
                                        _dailyAQmissing = true;
                                    }
                                    else if (!bcgMonitorBase.MonitorValues[0].dicMetricValues365.ContainsKey(metric.MetricName))//Daily function, missing daily AQ 
                                    {
                                        _dailyAQmissing = true;
                                    }
                                    else if (bcgMonitorBase.MonitorValues[0].dicMetricValues365[metric.MetricName].Count() < 365) //Daily function, daily AQ calculated but less than 365. 
                                    {
                                        _dailyAQmissing = true;
                                    }
                                }
                                else if (bcg.Base is ModelDataLine)// model data
                                {
                                    if (bcg.Base.ModelAttributes == null)
                                    {
                                        _dailyAQmissing = true;
                                    }
                                    else if (bcg.Base.ModelAttributes.Count() == 0)
                                    {
                                        _dailyAQmissing = true;
                                    }
                                    else
                                    {
                                        ModelDataLine bcgModelBase = bcg.Base as ModelDataLine;
                                        if (seasonalMetric != null) // function has seasonal metric --- seasonal function
                                        {
                                            _dailyAQmissing = true;
                                            foreach (ModelAttribute ma in bcgModelBase.ModelAttributes)
                                            {

                                                if (ma.SeasonalMetric != null && ma.SeasonalMetric.SeasonalMetricName == benMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName)
                                                {
                                                    _dailyAQmissing = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else //Daily function
                                        {
                                            _dailyAQmissing = true;
                                            foreach (ModelAttribute ma in bcgModelBase.ModelAttributes) //check metric matches
                                            {
                                                if (ma.Metric.MetricName == benMAPHealthImpactFunction.Metric.MetricName)
                                                {
                                                    _dailyAQmissing = false;
                                                    break;
                                                }
                                            }
                                            foreach (ModelAttribute ma in bcgModelBase.ModelAttributes) //check counts matches
                                            {
                                                if (ma.Values.Count() < 365)
                                                {
                                                    _dailyAQmissing = true;
                                                    break;
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _dailyAQmissing = false;
                                Logger.LogError(ex);
                            }


                        }
                    }


                    crSelectFunction.VariableDataSetID = benMAPHealthImpactFunction.VariableDataSetID;      //Code previously generated a dictionary of all variable dataset options, then selected the first entry. 
                    crSelectFunction.VariableDataSetName = benMAPHealthImpactFunction.VariableDataSetName;  //What is needed, is to copy over the dataset name/ID associated with the selected HIF.
                    crSelectFunction.StartAge = benMAPHealthImpactFunction.StartAge;
                    crSelectFunction.EndAge = benMAPHealthImpactFunction.EndAge;
                    crSelectFunction.GeographicAreaName = benMAPHealthImpactFunction.GeographicAreaName;
                    crSelectFunction.GeographicAreaID = benMAPHealthImpactFunction.GeographicAreaID;
                    crSelectFunction.GeographicAreaFeatureID = benMAPHealthImpactFunction.GeographicAreaFeatureID;
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
                    if (_maxCRID > 0)
                    {
                        crSelectFunction.CRID = _maxCRID + 1; _maxCRID = _maxCRID + 1;
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
                    // Default unconstrained functions to Everywhere
                    if (String.IsNullOrEmpty(crSelectFunction.GeographicAreaName))
                    {
                        crSelectFunction.GeographicAreaName = Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_EVERYWHERE;
                    }
                }
                //Now that we have added the new function(s), let's see what Geo Area Mode we are running in now
                setGeoAreaMode(lstCRSelectFunction);
                if (geoMode == Configuration.ConfigurationCommonClass.geographicAreaAnalysisMode.allConstrained || geoMode == Configuration.ConfigurationCommonClass.geographicAreaAnalysisMode.mixedConstraints)
                {
                    string iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
                    string isShow = "T";
                    if (System.IO.File.Exists(iniPath))
                    {
                        isShow = CommonClass.IniReadValue("appSettings", "IsShowGeographicAreaInfo", iniPath);
                    }
                    if (hasGeoAreaInfoShown == false && (string.IsNullOrEmpty(isShow) || isShow == "T"))
                    {
                        hasGeoAreaInfoShown = true;
                        GeographicAreaInfo gi = new GeographicAreaInfo();
                        gi.ShowDialog();
                    }
                }

                if (noPopHIF) //BenMAP-410: Provide message box to user if a HIF contains a function that does not use population
                {
                    MessageBox.Show("The selection contains a Health Impact Function that does not use population in calculating either baseline or point estimate values. In those cases, BenMAP will use the average rates of incidence and prevalence.");
                }
                olvSelected.SetObjects(lstCRSelectFunction);
                gBSelectedHealthImpactFuntion.Text = "Selected Health Impact Functions (" + lstCRSelectFunction.Count + ")";
                olvSelected.CheckBoxes = false;

                if (_dailyAQmissing == true)
                {
                    MessageBox.Show("One or more selected health impact functions are configured to use daily or seasonal metrics "
                        + "that are not available in the current air quality surfaces. If you do not revise your air quality "
                        + "surfaces to include daily data, the function will use the annual metric instead, which may produce "
                        + "incorrect results.  You may check the metric associated with each health impact function through the "
                        + "Manage Datasets Screen."
                        , "Warning - Air Quality Data Compatibility", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                if (missingIncData == true)
                {
                    MessageBox.Show("One or more selected  health impact functions do not have corresponded incidence/prevalence data. "
                        + "Please check if the endpoint names for your incidence/prevalence data through the Manage Datasets Screen. "
                        + "\nIf you continue, the output of this function may be 0."
                        , "Warning - Incidence/Prevalence Data Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

        }

        private void setGeoAreaMode(List<CRSelectFunction> lstCRSelectFunction)
        {

            int constrainedCount = 0;
            int everywhereCount = 0;
            int elsewhereCount = 0;
            foreach (CRSelectFunction crSelectFunction in lstCRSelectFunction)
            {
                if (crSelectFunction.GeographicAreaName == Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_EVERYWHERE)
                {
                    everywhereCount++;
                }
                else if (crSelectFunction.GeographicAreaName == Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_ELSEWHERE)
                {
                    elsewhereCount++;
                }
                else
                {
                    constrainedCount++;
                }
            }
            // Set the global geographic area analysis mode on the form
            if (constrainedCount == 0)
            {
                geoMode = Configuration.ConfigurationCommonClass.geographicAreaAnalysisMode.allUnconstrained;
            }
            else if (everywhereCount == 0 && elsewhereCount == 0)
            {
                geoMode = Configuration.ConfigurationCommonClass.geographicAreaAnalysisMode.allConstrained;
            }
            else
            {
                geoMode = Configuration.ConfigurationCommonClass.geographicAreaAnalysisMode.mixedConstraints;
            }

            // Set the column header to indicate whether we have editable items or not
            if (geoMode == Configuration.ConfigurationCommonClass.geographicAreaAnalysisMode.mixedConstraints)
            {
                HeaderFormatStyle hf = new HeaderFormatStyle();
                hf.SetForeColor(System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192))))));
                olvSelected.GetColumn("Geographic Area").HeaderFormatStyle = hf;
            }
            else
            {
                HeaderFormatStyle hf = new HeaderFormatStyle();
                hf.SetForeColor(System.Drawing.Color.Black);
                olvSelected.GetColumn("Geographic Area").HeaderFormatStyle = hf;
            }
        }

        private void olvSelected_CellEditStarting(object sender, CellEditEventArgs e)
        {
            base.OnClick(e);
            if (e.Column == null || (lstAsyns != null && lstAsyns.Count > 0))
            {
                e.Cancel = true;
                return;
            }
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
                    cb.SelectedText = e.Value.ToString(); cb.SelectedIndexChanged += new EventHandler(cbRace_SelectedIndexChanged);
                cb.Tag = e.RowObject; e.Control = cb;
            }
            else if (e.Column.Text == "Gender")
            {

                ComboBox cb = new ComboBox();
                cb.Bounds = e.CellBounds;
                cb.Font = ((ObjectListView)sender).Font;
                cb.DropDownStyle = ComboBoxStyle.DropDownList;
                cb.Items.AddRange(DicGender.Keys.ToArray());
                if (e.Value != null)
                    cb.SelectedText = e.Value.ToString(); cb.SelectedIndexChanged += new EventHandler(cbGender_SelectedIndexChanged);
                cb.Tag = e.RowObject; e.Control = cb;
            }
            else if (e.Column.Text == "Ethnicity")
            {

                ComboBox cb = new ComboBox();
                cb.Bounds = e.CellBounds;
                cb.Font = ((ObjectListView)sender).Font;
                cb.DropDownStyle = ComboBoxStyle.DropDownList;
                cb.Items.AddRange(DicEthnicity.Keys.ToArray());
                if (e.Value != null)
                    cb.SelectedText = e.Value.ToString(); cb.SelectedIndexChanged += new EventHandler(cbEthnicity_SelectedIndexChanged);
                cb.Tag = e.RowObject; e.Control = cb;
            }
            else if (e.Column.Text == "Incidence Dataset")
            {
                CRSelectFunction crSelectFunction = e.RowObject as CRSelectFunction;
                if (crSelectFunction.BenMAPHealthImpactFunction.Function.ToLower().Contains("incidence") || crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction.ToLower().Contains("incidence"))
                {


                    ComboBox cb = new ComboBox();
                    cb.Bounds = e.CellBounds;
                    cb.Font = ((ObjectListView)sender).Font;
                    cb.DropDownStyle = ComboBoxStyle.DropDownList;

                    commandText = string.Format("select distinct a.IncidenceDataSetID,IncidenceDataSetName from IncidenceDataSets a,IncidenceRates b where a.IncidenceDataSetID=b.IncidenceDataSetID and  SetupID={0} and b.EndPointGroupID={1} and Prevalence='F' and (b.EndPointID={2} or b.EndPointID=99 or b.EndPointID=100 or b.EndPointID=102)", CommonClass.MainSetup.SetupID, crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID, crSelectFunction.BenMAPHealthImpactFunction.EndPointID);
                    ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    cb.DataSource = ds.Tables[0];
                    cb.DisplayMember = "IncidenceDataSetName";


                    if (e.Value != null)
                        cb.SelectedText = e.Value.ToString(); cb.SelectedIndexChanged += new EventHandler(cbIncidenceDataSet_SelectedIndexChanged);
                    cb.Tag = e.RowObject; e.Control = cb;
                }
                else
                {
                    e.Cancel = true;
                }

            }
            else if (e.Column.Text == "Prevalence Dataset")
            {
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
                    if (e.Value != null)
                        cb.SelectedText = e.Value.ToString(); cb.SelectedIndexChanged += new EventHandler(cbPrevalenceDataSet_SelectedIndexChanged);
                    cb.Tag = e.RowObject; e.Control = cb;
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
                foreach (string s in dicVariable.Keys)
                {
                    if (DatabaseFunction.Contains(s.ToLower()))
                    {
                        inLst = true;
                        break;
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
                foreach (string s in dicVariable.Keys)
                {
                    if (DatabaseFunction.Contains(s.ToLower()))
                    {
                        inLst = true;
                        break;
                    }
                }
                if (inLst)
                {
                    ComboBox cb = new ComboBox();
                    cb.Bounds = e.CellBounds;
                    cb.Font = ((ObjectListView)sender).Font;
                    cb.DropDownStyle = ComboBoxStyle.DropDownList;
                    cb.Items.AddRange(this.DicVariableDataSet.Keys.ToArray());
                    if (e.Value != null)
                        cb.SelectedText = e.Value.ToString(); cb.SelectedIndexChanged += new EventHandler(cbVariableDataSet_SelectedIndexChanged);
                    cb.Tag = e.RowObject; e.Control = cb;

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
            else if (e.Column.Text == "Geographic Area" &&
                ((string)e.Value == Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_EVERYWHERE || (string)e.Value == Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_ELSEWHERE) &&
                geoMode == Configuration.ConfigurationCommonClass.geographicAreaAnalysisMode.mixedConstraints)
            {
                ComboBox cb = new ComboBox();
                cb.Bounds = e.CellBounds;
                cb.Font = ((ObjectListView)sender).Font;
                cb.DropDownStyle = ComboBoxStyle.DropDownList;
                cb.Items.Add(Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_EVERYWHERE);
                cb.Items.Add(Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_ELSEWHERE);

                if (e.Value != null)
                {
                    cb.Text = e.Value.ToString();
                }


                cb.SelectedIndexChanged += new EventHandler(cbGeographicArea_SelectedIndexChanged);
                cb.Tag = e.RowObject;
                e.Control = cb;
            }
            else
            {
                e.Cancel = true;
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

            }
            catch (Exception ex)
            {
                TextBox txt = (TextBox)sender;

                Logger.LogError(ex);
            }
        }

        void cbVariableDataSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            ((CRSelectFunction)cb.Tag).VariableDataSetName = cb.Text;
            ((CRSelectFunction)cb.Tag).VariableDataSetID = DicVariableDataSet[cb.Text];
        }

        void cbPrevalenceDataSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            ((CRSelectFunction)cb.Tag).PrevalenceDataSetName = cb.Text;
            ((CRSelectFunction)cb.Tag).PrevalenceDataSetID = DicIncidenceDataSet[cb.Text];
        }

        void cbIncidenceDataSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            ((CRSelectFunction)cb.Tag).IncidenceDataSetName = cb.Text;
            ((CRSelectFunction)cb.Tag).IncidenceDataSetID = DicIncidenceDataSet[cb.Text];
        }
        void cbRace_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            ((CRSelectFunction)cb.Tag).Race = cb.Text;
        }
        void cbGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            ((CRSelectFunction)cb.Tag).Gender = cb.Text;
        }
        void cbEthnicity_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            ((CRSelectFunction)cb.Tag).Ethnicity = cb.Text;
        }
        void cbGeographicArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            ((CRSelectFunction)cb.Tag).GeographicAreaName = cb.Text;
            // TODO: Encode GeographicAreaID somehow here to be more robust
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
                        MessageBox.Show("End age must be more than or equal to start age."); e.Cancel = true;
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
                        MessageBox.Show("End age must be more than or equal to start age."); e.Cancel = true;
                    }
                    else
                        ((CRSelectFunction)txt.Tag).EndAge = Convert.ToInt32(txt.Text);
                    ((ObjectListView)sender).RefreshItem(e.ListViewItem);
                    e.Cancel = true;
                }
                else if (e.Column.Text == "Race")
                {

                    ((ComboBox)e.Control).SelectedIndexChanged -= new EventHandler(cbRace_SelectedIndexChanged);

                    ((ObjectListView)sender).RefreshItem(e.ListViewItem);

                    e.Cancel = true;
                }
                else if (e.Column.Text == "Gender")
                {

                    ((ComboBox)e.Control).SelectedIndexChanged -= new EventHandler(cbGender_SelectedIndexChanged);

                    ((ObjectListView)sender).RefreshItem(e.ListViewItem);

                    e.Cancel = true;
                }
                else if (e.Column.Text == "Ethnicity")
                {

                    ((ComboBox)e.Control).SelectedIndexChanged -= new EventHandler(cbEthnicity_SelectedIndexChanged);

                    ((ObjectListView)sender).RefreshItem(e.ListViewItem);

                    e.Cancel = true;
                }
                else if (e.Column.Text == "Incidence Dataset")
                {
                    ((ComboBox)e.Control).SelectedIndexChanged -= new EventHandler(cbIncidenceDataSet_SelectedIndexChanged);

                    ((ObjectListView)sender).RefreshItem(e.ListViewItem);

                    e.Cancel = true;

                }
                else if (e.Column.Text == "Prevalence Dataset")
                {
                    ((ComboBox)e.Control).SelectedIndexChanged -= new EventHandler(cbPrevalenceDataSet_SelectedIndexChanged);

                    ((ObjectListView)sender).RefreshItem(e.ListViewItem);

                    e.Cancel = true;

                }
                else if (e.Column.Text == "Variable Dataset")
                {
                    ((ComboBox)e.Control).SelectedIndexChanged -= new EventHandler(cbVariableDataSet_SelectedIndexChanged);

                    ((ObjectListView)sender).RefreshItem(e.ListViewItem);

                    e.Cancel = true;
                }
                else if (e.Column.Text == "Geographic Area")
                {
                    ((ComboBox)e.Control).SelectedIndexChanged -= new EventHandler(cbGeographicArea_SelectedIndexChanged);
                    ((ObjectListView)sender).RefreshItem(e.ListViewItem);
                    ((ComboBox)e.Control).Dispose();
                    e.Cancel = true;
                }
            }
            catch
            {
                e.Cancel = true;
            }
        }
        Dictionary<string, List<string>> dicVariable = getDicVariableNameList();
        Dictionary<string, Dictionary<string, double>> DicAllSetupVariableValues = new Dictionary<string, Dictionary<string, double>>();
        Dictionary<string, Dictionary<string, double>> dicAllIncidence = new Dictionary<string, Dictionary<string, double>>();
        Dictionary<string, Dictionary<string, double>> dicAllPrevalence = new Dictionary<string, Dictionary<string, double>>();
        Dictionary<string, Dictionary<int, float>> dicALlPopulation = new Dictionary<string, Dictionary<int, float>>();
        Dictionary<string, Dictionary<int, float>> dicALlPopulation12 = new Dictionary<string, Dictionary<int, float>>();
        Dictionary<string, Dictionary<string, float>> dicALlPopulationAge = new Dictionary<string, Dictionary<string, float>>();
        public string _filePath = "";
        DateTime dtRunStart;

        /// <summary>
        /// return a dictionary containing the variables and their variable data set names for the currently selected setup
        /// </summary>
        /// <returns></returns>
        static Dictionary<string, List<string>> getDicVariableNameList()
        {
            Dictionary<string, List<string>> dicVariable = new Dictionary<string, List<string>>();
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = "select setupvariablename, setupvariabledatasetname from  setupvariables a join setupvariabledatasets b on a.setupvariabledatasetid=b.setupvariabledatasetid where b.setupid = " + CommonClass.MainSetup.SetupID + " ";
                DataTable dt = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    if (!dicVariable.ContainsKey(dr["setupvariablename"].ToString()))
                        dicVariable.Add(dr["setupvariablename"].ToString(), new List<string>());
                    dicVariable[dr["setupvariablename"].ToString()].Add(dr["setupvariabledatasetname"].ToString());
                }
                return dicVariable;
            }
            catch
            {
                return dicVariable;
            }
        }

        public async void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
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

                    HealthImpactConfirmation hifConfirm = new HealthImpactConfirmation(olvSelected.Objects as List<CRSelectFunction>);
                    DialogResult rtn = hifConfirm.ShowDialog();
                    //DialogResult rtn = MessageBox.Show("Run and save the CFG results file (*.cfgrx)?", "Run and Save", MessageBoxButtons.YesNo);
                    if (rtn != System.Windows.Forms.DialogResult.Yes) { return; }
                    else
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = "cfgrx files (*.cfgrx)|*.cfgrx";
                        sfd.RestoreDirectory = true;
                        sfd.InitialDirectory = CommonClass.ResultFilePath + @"\Result\CFGR";
                        if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            _filePath = sfd.FileName;
                        }
                        else { return; }
                    }

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
                            if (crv != null)
                                crv.CRCalculateValues = null;
                        }
                        CommonClass.lstCRResultAggregation = null;
                        GC.Collect();
                    }
                    if (CommonClass.ValuationMethodPoolingAndAggregation !=null && CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation != null) //YY:
                    {
                        foreach (CRSelectFunctionCalculateValue crv in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation)
                        {
                            if (crv != null)
                                crv.CRCalculateValues = null;
                        }
                        CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation = null;
                        GC.Collect();
                    }
                    CommonClass.BaseControlCRSelectFunction = new BaseControlCRSelectFunction();
                    GC.Collect();
                    CommonClass.BaseControlCRSelectFunction.BaseControlGroup = CommonClass.LstBaseControlGroup;
                    CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction = olvSelected.Objects as List<CRSelectFunction>;
                    CommonClass.BaseControlCRSelectFunction.CRLatinHypercubePoints = CommonClass.CRLatinHypercubePoints;
                    CommonClass.BaseControlCRSelectFunction.CRDefaultMonteCarloIterations = CommonClass.CRDefaultMonteCarloIterations;
                    CommonClass.BaseControlCRSelectFunction.CRRunInPointMode = CommonClass.CRRunInPointMode;
                    CommonClass.BaseControlCRSelectFunction.CRThreshold = CommonClass.CRThreshold;
                    CommonClass.BaseControlCRSelectFunction.RBenMapGrid = CommonClass.RBenMAPGrid;
                    CommonClass.BaseControlCRSelectFunction.BenMAPPopulation = CommonClass.BenMAPPopulation;
                }
                dtRunStart = DateTime.Now;
                Tools.CalculateFunctionString.dicPointEstimateMethodInfo.Clear();


                List<GridRelationship> lstGridRelationshipAll = CommonClass.LstGridRelationshipAll;
                string str = DateTime.Now.ToString();
                Dictionary<string, int> dicRace = Configuration.ConfigurationCommonClass.getAllRace();
                Dictionary<string, int> dicEthnicity = Configuration.ConfigurationCommonClass.getAllEthnicity();
                Dictionary<string, int> dicGender = Configuration.ConfigurationCommonClass.getAllGender();
                foreach (CRSelectFunction crSelectFunction in CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction)
                {
                    lstAsyns.Add(crSelectFunction.BenMAPHealthImpactFunction.ID.ToString());
                }
                this.pBarCR.Maximum = lstAsyns.Count() + 2;
                this.pBarCR.Minimum = 0;
                if (isBatch)
                {
                    CommonClass.BenMAPPopulation = CommonClass.BaseControlCRSelectFunction.BenMAPPopulation;
                    CommonClass.CRLatinHypercubePoints = CommonClass.BaseControlCRSelectFunction.CRLatinHypercubePoints;
                    CommonClass.CRDefaultMonteCarloIterations = CommonClass.BaseControlCRSelectFunction.CRDefaultMonteCarloIterations;
                    CommonClass.CRRunInPointMode = CommonClass.BaseControlCRSelectFunction.CRRunInPointMode;
                    CommonClass.CRThreshold = CommonClass.BaseControlCRSelectFunction.CRThreshold;
                    CommonClass.GBenMAPGrid = CommonClass.BaseControlCRSelectFunction.BaseControlGroup.First().GridType;

                }
                CommonClass.BaseControlCRSelectFunctionCalculateValue = new BaseControlCRSelectFunctionCalculateValue();
                CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup = CommonClass.BaseControlCRSelectFunction.BaseControlGroup;
                CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue = new List<CRSelectFunctionCalculateValue>();
                CommonClass.BaseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints = CommonClass.CRLatinHypercubePoints;
                CommonClass.BaseControlCRSelectFunctionCalculateValue.CRDefaultMonteCarloIterations = CommonClass.CRDefaultMonteCarloIterations;
                CommonClass.BaseControlCRSelectFunctionCalculateValue.CRRunInPointMode = CommonClass.CRRunInPointMode;
                CommonClass.BaseControlCRSelectFunctionCalculateValue.CRThreshold = CommonClass.CRThreshold;
                CommonClass.BaseControlCRSelectFunctionCalculateValue.RBenMapGrid = CommonClass.RBenMAPGrid;
                CommonClass.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation = CommonClass.BenMAPPopulation;
                CommonClass.BaseControlCRSelectFunctionCalculateValue.Version = "BenMAP-CE " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);

                int i = 0;
                List<CRSelectFunction> lstCRSelectFunction = new List<CRSelectFunction>();
                dicAllIncidence = new Dictionary<string, Dictionary<string, double>>();
                dicAllPrevalence = new Dictionary<string, Dictionary<string, double>>();
                dicALlPopulation = new Dictionary<string, Dictionary<int, float>>();
                dicALlPopulationAge = new Dictionary<string, Dictionary<string, float>>();

                Dictionary<int, float> dicPopulation12 = null;

                GridRelationship gridPopulation = new GridRelationship()
                {
                    bigGridID = CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 1 ? 1 : CommonClass.GBenMAPGrid.GridDefinitionID,
                    smallGridID = CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 1 ? CommonClass.GBenMAPGrid.GridDefinitionID : CommonClass.BenMAPPopulation.GridType.GridDefinitionID
                };
                sProgressBar = "Loading Air Quality Data.";
                while (sProgressBar.Length < 57)
                {
                    sProgressBar = sProgressBar + " ";
                }
                this.lbProgressBar.Text = sProgressBar;
                this.pBarCR.Value++;
                Application.DoEvents();
                lbProgressBar.Refresh();
                if (Tools.CalculateFunctionString.dicBaselineMethodInfo != null) Tools.CalculateFunctionString.dicBaselineMethodInfo.Clear();
                if (Tools.CalculateFunctionString.dicPointEstimateMethodInfo != null) Tools.CalculateFunctionString.dicPointEstimateMethodInfo.Clear();
                if (Tools.CalculateFunctionString.dicValuationMethodInfo != null) Tools.CalculateFunctionString.dicValuationMethodInfo.Clear();

                List<SetupVariableJoinAllValues> lstSetupVariable = new List<SetupVariableJoinAllValues>();
                Dictionary<int, Dictionary<string, ModelResultAttribute>> DicControlAll = new Dictionary<int, Dictionary<string, ModelResultAttribute>>();
                Dictionary<int, Dictionary<string, Dictionary<string, float>>> dicAllMetricDataBase = new Dictionary<int, Dictionary<string, Dictionary<string, float>>>();
                Dictionary<int, Dictionary<string, Dictionary<string, float>>> dicAllMetricDataControl = new Dictionary<int, Dictionary<string, Dictionary<string, float>>>();
                Dictionary<int, Dictionary<string, Dictionary<string, List<float>>>> dicAll365Base = new Dictionary<int, Dictionary<string, Dictionary<string, List<float>>>>();
                Dictionary<int, Dictionary<string, Dictionary<string, List<float>>>> dicAll365Control = new Dictionary<int, Dictionary<string, Dictionary<string, List<float>>>>();
                foreach (BaseControlGroup baseControlGroup in CommonClass.LstBaseControlGroup)
                {
                    Dictionary<string, ModelResultAttribute> dicControl = new Dictionary<string, ModelResultAttribute>();
                    foreach (ModelResultAttribute mr in baseControlGroup.Control.ModelResultAttributes)
                    {
                        if (!dicControl.Keys.Contains(mr.Col + "," + mr.Row))
                            dicControl.Add(mr.Col + "," + mr.Row, mr);
                    }
                    DicControlAll.Add(baseControlGroup.Pollutant.PollutantID, dicControl);
                    Dictionary<string, Dictionary<string, List<float>>> dic365Base = new Dictionary<string, Dictionary<string, List<float>>>();
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
                int crid = 1, crCount = CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction == null ? 0 : CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count();
                Tools.CalculateFunctionString.dicPointEstimateMethodInfo.Clear();
                Tools.CalculateFunctionString.dicPointEstimateMethodInfo = new Dictionary<string, object>();


                //Build cell intersection lists for each geographic area that is being used
                bool elsewhereExists = false;
                Dictionary<string, Dictionary<string, double>> dicAllGeoAreaPercentages = new Dictionary<string, Dictionary<string, double>>();
                dicAllGeoAreaPercentages[Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_EVERYWHERE] = null;

                foreach (CRSelectFunction crSelectFunction in CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction)
                {
                    // For backward compatability, assume "everywhere" if we don't have an area name set
                    if (string.IsNullOrEmpty(crSelectFunction.GeographicAreaName))
                    {
                        crSelectFunction.GeographicAreaName = Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_EVERYWHERE;
                    }

                    if (crSelectFunction.GeographicAreaName == Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_ELSEWHERE)
                    {
                        elsewhereExists = true;
                    }

                    if (!string.IsNullOrEmpty(crSelectFunction.GeographicAreaName)
                        && crSelectFunction.GeographicAreaName != Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_ELSEWHERE
                        && crSelectFunction.GeographicAreaName != Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_EVERYWHERE)
                    {
                        // Get the crosswalk for the Geographic Area
                        int geoId = crSelectFunction.GeographicAreaID;
                        try
                        {
                            if (dicAllGeoAreaPercentages.ContainsKey(crSelectFunction.GeographicAreaName) == false)
                            {
                                Dictionary<string, double> dicGeoAreaPercentages = CommonClass.IntersectionsWithGeographicArea(CommonClass.GBenMAPGrid.GridDefinitionID, geoId, crSelectFunction.GeographicAreaFeatureID);
                                dicAllGeoAreaPercentages.Add(crSelectFunction.GeographicAreaName, dicGeoAreaPercentages);
                            }
                        }
                        catch
                        {
                            // TODO: Add error handling
                        }
                    }
                }

        //BENMAP-448: Clean up cells that intersect with more than one named geo areas. Pick the winners with tha largest intersection. 
        //We only want once result per cell
        CommonClass.PruneGeographicAreaIntersections(dicAllGeoAreaPercentages);

                if (elsewhereExists)
                {
                    Dictionary<string, double> dicElsewherePercentages = new Dictionary<string, double>();
                    foreach (KeyValuePair<string, Dictionary<string, double>> dicGeoAreaPercentages in dicAllGeoAreaPercentages)
                    {
                        if (dicGeoAreaPercentages.Value != null)
                        {
                            foreach (KeyValuePair<string, double> pct in dicGeoAreaPercentages.Value)
                            {
                                dicElsewherePercentages[pct.Key] = pct.Value;
                            }
                        }
                    }
                    dicAllGeoAreaPercentages.Add(Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_ELSEWHERE, dicElsewherePercentages);
                }

                string populationStatus = "Loading Population Data...";
                if (isBatch)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(new String(' ', Console.BufferWidth));
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write(populationStatus);
                }
                else
                {
                    Application.DoEvents();
                    sProgressBar = "Loading Population data.";
                    while (sProgressBar.Length < 57)
                    {
                        sProgressBar = sProgressBar + " ";
                    }
                    this.lbProgressBar.Text = sProgressBar;
                    this.pBarCR.Value++;
                    lbProgressBar.Refresh();
                }

                //loop over health impact functions to find all race/gender/ethnicity groups and age ranges for which we need populations
                Dictionary<string, string> dicAllRaceEthnicityGenderAge = new Dictionary<string, string>();
                foreach (CRSelectFunction crSelectFunction in CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction)
                {
                    if (dicAllRaceEthnicityGenderAge.ContainsKey(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender))
                    {
                        string[] strAgeArray = dicAllRaceEthnicityGenderAge[crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender].Split(new char[] { ',' });
                        if (Convert.ToInt32(strAgeArray[0]) > crSelectFunction.StartAge)
                        {
                            strAgeArray[0] = crSelectFunction.StartAge.ToString();
                        }
                        if (Convert.ToInt32(strAgeArray[1]) < crSelectFunction.EndAge)
                        {
                            strAgeArray[1] = crSelectFunction.EndAge.ToString();
                        }

                        dicAllRaceEthnicityGenderAge[crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender] = strAgeArray[0] + "," + strAgeArray[1];


                    }
                    else
                        dicAllRaceEthnicityGenderAge.Add(crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender, crSelectFunction.StartAge + "," + crSelectFunction.EndAge);
                }

                // create a dictionary with keys of race+ethnicity+gender and a dictionary of population dictionaries for the start and end ages 
                dicALlPopulationAge = new Dictionary<string, Dictionary<string, float>>();
                try
                {
                    foreach (KeyValuePair<string, string> kAge in dicAllRaceEthnicityGenderAge)
                    {
                        string[] skAgeArray = kAge.Value.Split(new char[] { ',' });
                        string[] skAgeArrayRaceGenderEthnicity = kAge.Key.Split(new char[] { ',' });

                        //build cache key. key is race,ethnicity,gender,start age,end age,CommonClass.GBenMAPGrid.GridDefinitionID,CommonClass.BenMAPPopulation.GridType.GridDefinitionID
                        //added year of population dataset to cache key
                        string cacheKey = String.Format("{0},{1},{2},{3},{4}",
                                                        kAge.Key, kAge.Value,
                                                        CommonClass.GBenMAPGrid.GridDefinitionID.ToString(),
                                                        CommonClass.BenMAPPopulation.GridType.GridDefinitionID.ToString(),
                                                        CommonClass.BenMAPPopulation.Year.ToString());

                        //check cache
                        Dictionary<string, float> dicPopulationAgeIn;


                        if (CommonClass.DicPopulationAgeInCache.Keys.Contains(cacheKey))
                        {
                            //if in cache, retrieve a copy
                            dicPopulationAgeIn = new Dictionary<string, float>(CommonClass.DicPopulationAgeInCache[cacheKey]);

                            //this.lbProgressBar.Text = String.Format("Loading Cached Population data for Race = {0}, Ethnicity = {1}, Gender = {2}, Start Age = {3}, End Age = {4}",
                            //                                        skAgeArrayRaceGenderEthnicity[0], skAgeArrayRaceGenderEthnicity[1], skAgeArrayRaceGenderEthnicity[2],
                            //                                        skAgeArray[0], skAgeArray[1]);
                            if (!isBatch)
                                this.lbProgressBar.Text = "Loading Cached Population data.";
                            else
                            {
                                populationStatus = "Loading Population Data (Cached)...";
                                Console.SetCursorPosition(0, Console.CursorTop);
                                Console.Write(new String(' ', Console.BufferWidth));
                                Console.SetCursorPosition(0, Console.CursorTop - 1);
                                Console.Write(populationStatus);
                            }
                        }
                        else
                        {
                            //if not in cache, retreive population                                          

                            CRSelectFunction crSelectFunction = CommonClass.getCRSelectFunctionClone(CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.First());
                            crSelectFunction.StartAge = Convert.ToInt32(skAgeArray[0]);
                            crSelectFunction.EndAge = Convert.ToInt32(skAgeArray[1]);
                            crSelectFunction.Race = skAgeArrayRaceGenderEthnicity[0];
                            crSelectFunction.Ethnicity = skAgeArrayRaceGenderEthnicity[1];
                            crSelectFunction.Gender = skAgeArrayRaceGenderEthnicity[2];

                            //build population
                            dicPopulationAgeIn = new Dictionary<string, float>();
                            Configuration.ConfigurationCommonClass.getPopulationDataSetFromCRSelectFunction(ref dicPopulationAgeIn, ref dicPopulation12, crSelectFunction, CommonClass.BenMAPPopulation, dicRace, dicEthnicity,
                                    dicGender, CommonClass.GBenMAPGrid.GridDefinitionID, gridPopulation);

                            //add copy of dicPopulationAgeIn to cache
                            CommonClass.DicPopulationAgeInCache.Add(cacheKey, new Dictionary<string, float>(dicPopulationAgeIn));

                        }

                        //set dicPopulationAgeIn
                        dicALlPopulationAge.Add(kAge.Key, dicPopulationAgeIn);

                    }

                    if (isBatch)
                    {
                        Console.SetCursorPosition(populationStatus.Length, Console.CursorTop);
                        Console.Write(new String(' ', Console.BufferWidth));
                        Console.SetCursorPosition(populationStatus.Length, Console.CursorTop - 1);
                        Console.Write("Completed" + Environment.NewLine);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    if (isBatch)
                    {
                        Console.WriteLine("Error Loading Population Data");
                        return;
                    }
                }

                List<string> lstAllAgeID = Configuration.ConfigurationCommonClass.getAllAgeID();
                Dictionary<string, string> dicBaseLine = new Dictionary<string, string>();
                Dictionary<string, string> dicEstimate = new Dictionary<string, string>();
                Dictionary<string, string> dicBaseLineVariables = new Dictionary<string, string>();
                Dictionary<string, string> dicEstimateVariables = new Dictionary<string, string>();
                foreach (CRSelectFunction crSelectFunction in CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction)
                {

                    dicBaseLine.Add(crid.ToString(), Configuration.ConfigurationCommonClass.getFunctionStringFromDatabaseFunction(crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction));
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
                    List<Tuple<string, int>> SystemVariableNameList = Configuration.ConfigurationCommonClass.getAllSystemVariableNameList();
                    foreach (Tuple<string, int> tuple in SystemVariableNameList)
                    {
                        if (tuple.Item2 == crSelectFunction.VariableDataSetID)
                        {
                            if (DatabaseFunction.ToLower().Contains(tuple.Item1.ToLower()))
                            {
                                string cleanFunction = Regex.Replace(DatabaseFunction, @"[^\w]+", ",");
                                string[] checkFunction = cleanFunction.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                                foreach (string temp in checkFunction)
                                {
                                    if (temp.Equals(tuple.Item1.ToLower()))
                                    {
                                        if (dicEstimateVariables.ContainsKey(crid.ToString()))
                                        {
                                            if (dicEstimateVariables[crid.ToString()] == "")
                                                dicEstimateVariables[crid.ToString()] = " double " + tuple.Item1.ToLower();
                                            else if (!dicEstimateVariables[crid.ToString()].Contains("double " + tuple.Item1.ToLower()))
                                                dicEstimateVariables[crid.ToString()] += " , double " + tuple.Item1.ToLower();
                                        }
                                        else
                                        {
                                            dicEstimateVariables.Add(crid.ToString(), " double " + tuple.Item1.ToLower());
                                        }
                                    }
                                }
                            }
                        }
                    }
                    dicEstimate.Add(crid.ToString(), Configuration.ConfigurationCommonClass.getFunctionStringFromDatabaseFunction(crSelectFunction.BenMAPHealthImpactFunction.Function));
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

                    foreach (Tuple<string, int> tuple in SystemVariableNameList)
                    {
                        if (DatabaseFunction.ToLower().Contains(tuple.Item1.ToLower()))
                        {
                            string cleanFunction = Regex.Replace(DatabaseFunction, @"[^\w]+", ",");
                            string[] checkFunction = cleanFunction.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                            foreach (string temp in checkFunction)
                            {
                                if (temp.Equals(tuple.Item1.ToLower()))
                                {
                                    if (dicEstimateVariables.ContainsKey(crid.ToString()))
                                    {
                                        if (dicEstimateVariables[crid.ToString()] == "")
                                            dicEstimateVariables[crid.ToString()] = " double " + tuple.Item1.ToLower();
                                        else if (!dicEstimateVariables[crid.ToString()].Contains("double " + tuple.Item1.ToLower()))
                                            dicEstimateVariables[crid.ToString()] += " , double " + tuple.Item1.ToLower();
                                    }
                                    else
                                    {
                                        dicEstimateVariables.Add(crid.ToString(), " double " + tuple.Item1.ToLower());
                                    }
                                }
                            }
                        }
                    }

                    crid = crid + 1;
                }
                CalculateFunctionString calculateFunctionString = new CalculateFunctionString();
                calculateFunctionString.CreateAllBaselineEvalObjects(dicBaseLine, dicEstimateVariables);
                calculateFunctionString.CreateAllPointEstimateEvalObjects(dicEstimate, dicEstimateVariables);
                crid = 1;

                var tasksHIF = new List<Task>();
                if (isBatch)
                {
                    Console.Write("Loading Health Impact Functions...");
                }

                try
                {
                    foreach (CRSelectFunction crSelectFunction in CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction)
                    {

                        GridRelationship gridRelationShipIncidence = null;
                        Dictionary<string, double> dicIncidenceRateAttribute = null;
                        GridRelationship gridRelationShipPrevalence = null;
                        Dictionary<string, double> dicPrevalenceRateAttribute = null;
                        //additionaly keys for looking up incidence values
                        int funRaceID = dicRace.ContainsKey(crSelectFunction.Race) ? dicRace[crSelectFunction.Race] : 0;
                        int funEthnicityID = dicEthnicity.ContainsKey(crSelectFunction.Ethnicity) ? dicEthnicity[crSelectFunction.Ethnicity] : 0;
                        int funGenderID = dicGender.ContainsKey(crSelectFunction.Gender) ? dicGender[crSelectFunction.Gender] : 0;

                        string strAuthorCount = crid.ToString() + " of " + CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count.ToString() + "."; while (strAuthorCount.Length < CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count.ToString().Length * 2 + 4)
                        {
                            strAuthorCount = " " + strAuthorCount;

                        }

                        if (crSelectFunction.StartAge == -1 && crSelectFunction.EndAge == -1)
                        {
                            crSelectFunction.StartAge = 0;
                            crSelectFunction.EndAge = 0;
                        }
                        Dictionary<string, float> dicPopulationAge = dicALlPopulationAge[crSelectFunction.Race + "," + crSelectFunction.Ethnicity + "," + crSelectFunction.Gender];

                        Dictionary<string, double> dicAge = Configuration.ConfigurationCommonClass.getDicAge(crSelectFunction);




                        string commandText = string.Format("select GriddefinitionID from IncidenceDatasets where IncidenceDatasetID={0}", crSelectFunction.IncidenceDataSetID);
                        int incidenceDataSetGridType = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                        commandText = string.Format("select GriddefinitionID from IncidenceDatasets where IncidenceDatasetID={0}", crSelectFunction.PrevalenceDataSetID);
                        int PrevalenceDataSetGridType = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));

                        if (crSelectFunction.IncidenceDataSetID > -1)
                        {
                            //if (dicAllIncidence.Keys.Contains(crSelectFunction.IncidenceDataSetID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + "," + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "," + crSelectFunction.StartAge + "," + crSelectFunction.EndAge))
                            // dicAllIncidence.Keys includes (1)Incidence dataset ID, (2)endpoint group ID, (3) endpoint ID, (4)start age, (5)end age, as well as (6)race, (7) ethnicity and (8) gender
                            if (dicAllIncidence.Keys.Contains(crSelectFunction.IncidenceDataSetID + ","
                                + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + ","
                                + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + ","
                                + crSelectFunction.StartAge + ","
                                + crSelectFunction.EndAge + ","
                                + funRaceID + ","
                                + funEthnicityID + ","
                                + funGenderID))
                            {
                                dicIncidenceRateAttribute = dicAllIncidence[crSelectFunction.IncidenceDataSetID + ","
                                    + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + ","
                                    + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + ","
                                    + crSelectFunction.StartAge + ","
                                    + crSelectFunction.EndAge + ","
                                    + funRaceID + ","
                                    + funEthnicityID + ","
                                    + funGenderID];
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

                                dicIncidenceRateAttribute = Configuration.ConfigurationCommonClass.getIncidenceDataSetFromCRSelectFuntionDicAllAge(dicAge, dicPopulationAge, dicPopulation12, crSelectFunction, false, dicRace, dicEthnicity, dicGender, CommonClass.GBenMAPGrid.GridDefinitionID, gridRelationShipIncidence);
                                bool debug = true;
                            if(debug && dicIncidenceRateAttribute.ContainsKey("3120097,8")){
                                    Console.WriteLine("grid : 3120097 " + " " + dicIncidenceRateAttribute["3120097,8"]);
                                }
                                dicAllIncidence.Add(crSelectFunction.IncidenceDataSetID + ","
                                    + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + ","
                                    + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + ","
                                    + crSelectFunction.StartAge + ","
                                    + crSelectFunction.EndAge + ","
                                    + funRaceID + ","
                                    + funEthnicityID + ","
                                    + funGenderID, dicIncidenceRateAttribute);
                            }
                        }
                        if (crSelectFunction.PrevalenceDataSetID > -1)
                        {
                            if (dicAllPrevalence.Keys.Contains(crSelectFunction.PrevalenceDataSetID + ","
                                + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + ","
                                + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + ","
                                + crSelectFunction.StartAge + ","
                                + crSelectFunction.EndAge + ","
                                + funRaceID + ","
                                + funEthnicityID + ","
                                + funGenderID))
                            {
                                dicPrevalenceRateAttribute = dicAllPrevalence[crSelectFunction.PrevalenceDataSetID + ","
                                    + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + ","
                                    + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + ","
                                    + crSelectFunction.StartAge + ","
                                    + crSelectFunction.EndAge + ","
                                    + funRaceID + ","
                                    + funEthnicityID + ","
                                    + funGenderID];
                            }
                            else
                            {
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


                                dicPrevalenceRateAttribute = Configuration.ConfigurationCommonClass.getIncidenceDataSetFromCRSelectFuntionDicAllAge(dicAge, dicPopulationAge, dicPopulation12, crSelectFunction, true, dicRace, dicEthnicity, dicGender, CommonClass.GBenMAPGrid.GridDefinitionID, gridRelationShipPrevalence);
                                dicAllPrevalence.Add(crSelectFunction.PrevalenceDataSetID + ","
                                    + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + ","
                                    + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + ","
                                    + crSelectFunction.StartAge + ","
                                    + crSelectFunction.EndAge + ","
                                    + funRaceID + ","
                                    + funEthnicityID + ","
                                    + funGenderID, dicPrevalenceRateAttribute);

                            }
                        }
                        lstSetupVariable = null;
                        DicAllSetupVariableValues = new Dictionary<string, Dictionary<string, double>>();
                        if (crSelectFunction.VariableDataSetID > -1)
                        {
                            lstSetupVariable = new List<SetupVariableJoinAllValues>();
                            Configuration.ConfigurationCommonClass.getSetupVariableNameListFromDatabaseFunction(crSelectFunction.VariableDataSetID, CommonClass.GBenMAPGrid.GridDefinitionID, crSelectFunction.BenMAPHealthImpactFunction.Function, Configuration.ConfigurationCommonClass.LstSystemVariableName, ref lstSetupVariable);
                            Configuration.ConfigurationCommonClass.getSetupVariableNameListFromDatabaseFunction(crSelectFunction.VariableDataSetID, CommonClass.GBenMAPGrid.GridDefinitionID, crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction, Configuration.ConfigurationCommonClass.LstSystemVariableName, ref lstSetupVariable);
                            // dump variable list to debug file after adding baseline function
                            Configuration.ConfigurationCommonClass.dumpSetupVariableJoinAllValueToDebugFile(ref lstSetupVariable);
                            if (lstSetupVariable != null)
                            {
                                foreach (SetupVariableJoinAllValues sv in lstSetupVariable)
                                {
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
                        double[] lhsResultArray = null;
                        // if not running in point mode
                        if (!CommonClass.CRRunInPointMode)
                        {
                            int iRandomSeed = Convert.ToInt32(DateTime.Now.Hour + "" + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond);
                            if (CommonClass.CRSeeds != null && CommonClass.CRSeeds != -1)
                                iRandomSeed = Convert.ToInt32(CommonClass.CRSeeds);


                            lhsResultArray = Configuration.ConfigurationCommonClass.getLHSArrayCRFunctionSeed(CommonClass.CRLatinHypercubePoints, crSelectFunction, iRandomSeed, CommonClass.CRDefaultMonteCarloIterations);
                            crSelectFunction.lstLatinPoints = new List<LatinPoints>();
                            crSelectFunction.lstLatinPoints.Add(new LatinPoints() { values = lhsResultArray.ToList() });
                        }
                        string iRunCRID = crid.ToString();
                        // if not batch perform asynch call 
                        if (!isBatch)
                        {
                            IAsyncResult ar = dlgt.BeginInvoke(iRunCRID, lstAllAgeID, dicAge, dicAllMetricDataBase[baseControlGroup.Pollutant.PollutantID], dicAllMetricDataControl[baseControlGroup.Pollutant.PollutantID]
                                , dicAll365Base[baseControlGroup.Pollutant.PollutantID], dicAll365Control[baseControlGroup.Pollutant.PollutantID], DicControlAll[baseControlGroup.Pollutant.PollutantID],
                                DicAllSetupVariableValues, dicPopulationAge, dicIncidenceRateAttribute, dicPrevalenceRateAttribute, incidenceDataSetGridType, PrevalenceDataSetGridType, dicRace, dicEthnicity, dicGender, CommonClass.CRThreshold, CommonClass.CRLatinHypercubePoints,
                                CommonClass.CRRunInPointMode, lstGridRelationshipAll, crSelectFunction, dicAllGeoAreaPercentages[crSelectFunction.GeographicAreaName], baseControlGroup, null, CommonClass.BenMAPPopulation, lhsResultArray, new AsyncCallback(outPut), dlgt);
                        }
                        else  //In Batch Mode, add the calculation of each HIF to a list of tasks
                        {
                            tasksHIF.Add(Task.Run(() =>
                            Configuration.ConfigurationCommonClass.CalculateOneCRSelectFunction(iRunCRID, lstAllAgeID, dicAge, dicAllMetricDataBase[baseControlGroup.Pollutant.PollutantID], dicAllMetricDataControl[baseControlGroup.Pollutant.PollutantID]
                                , dicAll365Base[baseControlGroup.Pollutant.PollutantID], dicAll365Control[baseControlGroup.Pollutant.PollutantID], DicControlAll[baseControlGroup.Pollutant.PollutantID],
                                DicAllSetupVariableValues, dicPopulationAge, dicIncidenceRateAttribute, dicPrevalenceRateAttribute, incidenceDataSetGridType, PrevalenceDataSetGridType, dicRace, dicEthnicity, dicGender, CommonClass.CRThreshold, CommonClass.CRLatinHypercubePoints,
                                CommonClass.CRRunInPointMode, lstGridRelationshipAll, crSelectFunction, dicAllGeoAreaPercentages[crSelectFunction.GeographicAreaName], baseControlGroup, null, CommonClass.BenMAPPopulation, lhsResultArray)));
                        }
                        crid = crid + 1;
                    }
                    Console.WriteLine("Completed (" + CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count + " HIFs)");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }

                if (isBatch)
                {
                    Task finishHIF = Task.WhenAll(tasksHIF);    //Begin a task that is waiting for the completion of all HIF tasks
                    try
                    {
                        CancellationTokenSource tokenSource = new CancellationTokenSource();
                        CancellationToken token = tokenSource.Token;

                        Task progressTask = Task.Run(() =>      //While the HIF tasks are not finished, begin a task to indicate progress to user (repeats 3 dots)
                        {
                            string progressString = "Processing Health Impact Functions";
                            Console.Write(progressString);

                            while (true)
                            {
                                if (!token.IsCancellationRequested) //As long as the task is not canceled, continue to print to the console, resetting the position after printing
                                {
                                    for (int progressSpace = 1; progressSpace < 4; progressSpace++)
                                    {
                                        Thread.Sleep(1000);
                                        Console.Write(".");
                                        Thread.Sleep(1000);
                                    }

                                    Console.SetCursorPosition(progressString.Length, Console.CursorTop);
                                    Console.Write(new String(' ', Console.BufferWidth));
                                    Console.SetCursorPosition(progressString.Length, Console.CursorTop - 1);
                                }
                                else
                                    break;
                            }
                        }, tokenSource.Token);

                        finishHIF.Wait();           //Wait for the completion of all HIF tasks
                        tokenSource.Cancel();       //Send cancellation request to the progress task when all have finished
                        progressTask.Wait();        //Wait for the progress task to cancel, then continue

                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex);
                    }

                    if (finishHIF.Exception != null)        //Check on status of the tasks running HIF, clear the console and provide final update
                        throw finishHIF.Exception;

                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(new String(' ', Console.BufferWidth));
                    Console.SetCursorPosition(0, Console.CursorTop - 1);

                    if (finishHIF.Status == TaskStatus.RanToCompletion)
                        Console.Write("Processing Health Impact Functions...Completed" + Environment.NewLine);
                    else if (finishHIF.Status == TaskStatus.Faulted)
                        Console.Write("Processing Health Impact Functions...Error" + Environment.NewLine);
                    else
                        return;

                    afterOutput(isBatch);
                }
            }
            catch (Exception ex)
            {
                // 2016 06 10 - added logging and message box to show stack trace if error occurs, otherwise there is no way to know that it's failed.
                // log stack trace for debug use
                Logger.LogError(ex);
                // show message to user, as well
                MessageBox.Show(ex.StackTrace);
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
            }
            finally
            {
            }
        }

        List<CRSelectFunctionCalculateValue> lstCRSelectFunctionCalculateValue = new List<CRSelectFunctionCalculateValue>();
        private List<string> lstAsyns = new List<string>();
        private void afterOutput(bool isBatch)
        {

            if (DicAllSetupVariableValues != null)
            {
                for (int iKey = 0; iKey < DicAllSetupVariableValues.Count; iKey++)
                {
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

            try
            {
                if (!isBatch)
                {
                    GC.Collect();
                    this.pBarCR.Value = this.pBarCR.Maximum;
                    DateTime dtNow = DateTime.Now;
                    TimeSpan ts = dtNow.Subtract(dtRunStart);
                    this.lbProgressBar.Text = "Processing complete. HIF processing time: " + ts.Hours + " hours " + ts.Minutes + " minutes " + ts.Seconds + " seconds.";
                    CommonClass.BaseControlCRSelectFunctionCalculateValue.lstLog = new List<string>();
                    CommonClass.BaseControlCRSelectFunctionCalculateValue.lstLog.Add(this.lbProgressBar.Text);
                    CommonClass.BaseControlCRSelectFunctionCalculateValue.CreateTime = DateTime.Now;
                    if (_filePath != "")
                    {
                        Configuration.ConfigurationCommonClass.SaveCRFRFile(CommonClass.BaseControlCRSelectFunctionCalculateValue, _filePath);
                    }
                    Thread.Sleep(3000);
                    GC.Collect();
                }
                else
                {
                    Configuration.ConfigurationCommonClass.SaveCRFRFile(CommonClass.BaseControlCRSelectFunctionCalculateValue, _filePath);
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(new String(' ', Console.BufferWidth));
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.WriteLine("Results Saved At:" + Environment.NewLine + _filePath);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }
        }
        private void outPut(IAsyncResult ar)
        {
            try
            {
                lock (this)
                {

                    lstAsyns.RemoveAt(0);
                    int icount = CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count - lstAsyns.Count;
                    this.pBarCR.Value++;
                    GC.Collect();
                    Application.DoEvents();
                    for (int iSelect = 0; iSelect < icount; iSelect++)
                    {
                        olvSelected.GetItem(iSelect).UseItemStyleForSubItems = true; olvSelected.GetItem(iSelect).Font = new Font(olvSelected.GetItem(iSelect).Font, FontStyle.Bold);
                    }
                    olvSelected.Refresh();
                    string strAuthorCount = icount.ToString() + " of " + CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count.ToString();
                    while (strAuthorCount.Length < CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Count.ToString().Length * 2 + 4)
                    {
                        strAuthorCount = " " + strAuthorCount;

                    }
                    //  string sProgressBar = String.Format("Processing {0} Health Impact Functions. Currently calculating {1}", strAuthorCount, CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.ElementAt(icount).VariableDataSetName.ToString());
                    string sProgressBar = String.Format("Processing {0} Health Impact Functions.", strAuthorCount);
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
                        afterOutput(false);     //The encapsulating method (outPut) is only called when not in batch mode, sending false to differentiate behavior in batch mode for afterOutput [08 Jan 2020, MP]
                        this.DialogResult = DialogResult.OK;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        public delegate void AsyncDelegateCalculateLstCRSelectFunction(List<CRSelectFunction> lstCRSelectFunction, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, List<GridRelationship> lstGridRelationshipAll);
        public delegate void AsyncDelegateCalculateOneCRSelectFunction(string crid, List<string> lstAllAgeID, Dictionary<string, double> dicAge, Dictionary<string, Dictionary<string, float>> dicBaseMetricData, Dictionary<string, Dictionary<string, float>> dicControlMetricData,
           Dictionary<string, Dictionary<string, List<float>>> dicBase365, Dictionary<string, Dictionary<string, List<float>>> dicControl365,
           Dictionary<string, ModelResultAttribute> dicControl, Dictionary<string, Dictionary<string, double>> DicAllSetupVariableValues, Dictionary<string, float> dicPopulationAllAge, Dictionary<string, double> dicIncidenceRateAttribute,
           Dictionary<string, double> dicPrevalenceRateAttribute, int incidenceDataSetGridType, int PrevalenceDataSetGridType, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, double Threshold, int LatinHypercubePoints,
           bool RunInPointMode, List<GridRelationship> lstGridRelationship, CRSelectFunction crSelectFunction, Dictionary<string, double> dicGeoAreaIntersections, BaseControlGroup baseControlGroup, List<RegionTypeGrid> lstRegionTypeGrid, BenMAPPopulation benMAPPopulation, double[] lhsResultArray);

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
            if (filter == null)
                olv.DefaultRenderer = null;
            else
            {
                olv.DefaultRenderer = new HighlightTextRenderer(filter);

                olv.DefaultRenderer = new HighlightTextRenderer { Filter = filter, UseGdiTextRendering = true };
            }


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
        TipFormGIF waitMess = new TipFormGIF(); bool sFlog = true;
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

        private delegate void CloseFormDelegate();
        private delegate void ChangeDelegate(string msg);
        public void WaitClose()
        {
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

        public void btDelSelectMethod_Click(object sender, EventArgs e)
        {
            try
            {
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
                //Now that we have remove function(s), let's see what Geo Area Mode we are running in now
                setGeoAreaMode(lstCRSelectFunction);

                //If geoAreaMode not mixed, then make sure everything is set to everywhere.  Elsewhere wouldn't be valid.
                foreach (CRSelectFunction cr in lstCRSelectFunction)
                {
                    if (cr.GeographicAreaName == Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_ELSEWHERE)
                    {
                        cr.GeographicAreaName = Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_EVERYWHERE;
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

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CFG files (*.cfgx)|*.cfgx";
            sfd.RestoreDirectory = true;
            sfd.InitialDirectory = CommonClass.ResultFilePath + @"\Result\CFG";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _filePath = sfd.FileName;
                BaseControlCRSelectFunction bc = new BaseControlCRSelectFunction();
                bc.CRLatinHypercubePoints = CommonClass.CRLatinHypercubePoints;
                bc.CRDefaultMonteCarloIterations = CommonClass.CRDefaultMonteCarloIterations;
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
                BaseControlCRSelectFunction bc = new BaseControlCRSelectFunction();
                bc.BaseControlGroup = CommonClass.LstBaseControlGroup;
                bc.lstCRSelectFunction = olvSelected.Objects as List<CRSelectFunction>;
            }

        }
        // EMPTY FUNCTION - this function contain no code and does nothing.
        private void olvSimple_IsHyperlink(object sender, IsHyperlinkEventArgs e)
        {

        }

        // DEADCODE - this callback no longer supplies any functionality.
        private void olvSimple_CellClick(object sender, CellClickEventArgs e)
        {
            base.OnClick(e);
            if (e.Column != null && e.Column.Hyperlink)
            {
                switch (e.Column.Text)
                {
                    case "DataSet":
                        // HARDCODED - help file location and page to open
                        // DEADCODE - commented this out as there is no help file and this is the only active place where it could be called
                        //Help.ShowHelp(this, Application.StartupPath + @"\Data\QuickStartGuide.chm", "select_health_impact_function.htm");
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
            (frm as LatinHypercubePoints).DefaultMonteCarloIterations = CommonClass.CRDefaultMonteCarloIterations;
            DialogResult rtn = frm.ShowDialog();
            if (rtn != DialogResult.OK) { return; }
            CommonClass.CRLatinHypercubePoints = (frm as LatinHypercubePoints).LatinHypercubePointsCount;
            CommonClass.CRRunInPointMode = (frm as LatinHypercubePoints).IsRunInPointMode;
            CommonClass.CRThreshold = (frm as LatinHypercubePoints).Threshold;
            CommonClass.CRDefaultMonteCarloIterations = (frm as LatinHypercubePoints).DefaultMonteCarloIterations;
        }
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

        private void olvSimple_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void olvSelected_FormatCell(object sender, FormatCellEventArgs e)
        {
            if (e.Column.Text == "Geographic Area" &&
                ((string)e.CellValue == Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_EVERYWHERE || (string)e.CellValue == Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_ELSEWHERE) &&
                geoMode == Configuration.ConfigurationCommonClass.geographicAreaAnalysisMode.mixedConstraints)
            {
                CellBorderDecoration cbd = new CellBorderDecoration();
                cbd.BorderPen = new Pen(Color.LightGray);
                cbd.FillBrush = null;
                cbd.BoundsPadding = new Size(0, -1);
                cbd.CornerRounding = 0.0f;
                e.SubItem.Decorations.Add(cbd);

                Image imgDD = global::BenMAP.Properties.Resources.dropdown_hint;
                e.SubItem.Decorations.Add(new ImageDecoration(imgDD, ContentAlignment.MiddleRight));

            }
            else if (e.Column.Text == "Race" || e.Column.Text == "Ethnicity" || e.Column.Text == "Gender")
            {
                CellBorderDecoration cbd = new CellBorderDecoration();
                cbd.BorderPen = new Pen(Color.LightGray);
                cbd.FillBrush = null;
                cbd.BoundsPadding = new Size(0, -1);
                cbd.CornerRounding = 0.0f;
                e.SubItem.Decorations.Add(cbd);

                Image imgDD = global::BenMAP.Properties.Resources.dropdown_hint;
                e.SubItem.Decorations.Add(new ImageDecoration(imgDD, ContentAlignment.MiddleRight));
            }
            else if (e.Column.Text == "Incidence Dataset" && !String.IsNullOrEmpty((string)e.CellValue))
            {
                CellBorderDecoration cbd = new CellBorderDecoration();
                cbd.BorderPen = new Pen(Color.LightGray);
                cbd.FillBrush = null;
                cbd.BoundsPadding = new Size(0, -1);
                cbd.CornerRounding = 0.0f;
                e.SubItem.Decorations.Add(cbd);

                Image imgDD = global::BenMAP.Properties.Resources.dropdown_hint;
                e.SubItem.Decorations.Add(new ImageDecoration(imgDD, ContentAlignment.MiddleRight));
            }
            else if (e.Column.Text == "Prevalence Dataset" && !String.IsNullOrEmpty((string)e.CellValue))
            {
                CellBorderDecoration cbd = new CellBorderDecoration();
                cbd.BorderPen = new Pen(Color.LightGray);
                cbd.FillBrush = null;
                cbd.BoundsPadding = new Size(0, -1);
                cbd.CornerRounding = 0.0f;
                e.SubItem.Decorations.Add(cbd);

                Image imgDD = global::BenMAP.Properties.Resources.dropdown_hint;
                e.SubItem.Decorations.Add(new ImageDecoration(imgDD, ContentAlignment.MiddleRight));
            }
            else if (e.Column.Text == "Variable Dataset" && !String.IsNullOrEmpty((string)e.CellValue))
            {
                CellBorderDecoration cbd = new CellBorderDecoration();
                cbd.BorderPen = new Pen(Color.LightGray);
                cbd.FillBrush = null;
                cbd.BoundsPadding = new Size(0, -1);
                cbd.CornerRounding = 0.0f;
                e.SubItem.Decorations.Add(cbd);

                Image imgDD = global::BenMAP.Properties.Resources.dropdown_hint;
                e.SubItem.Decorations.Add(new ImageDecoration(imgDD, ContentAlignment.MiddleRight));
            }
            else if (e.Column.Text == "Start Age" || e.Column.Text == "End Age")
            {
                CellBorderDecoration cbd = new CellBorderDecoration();
                cbd.BorderPen = new Pen(Color.LightGray);
                cbd.FillBrush = null;
                cbd.BoundsPadding = new Size(0, -1);
                cbd.CornerRounding = 0.0f;
                e.SubItem.Decorations.Add(cbd);
            }

        }
    }

    public class HealthImapctDropSink : SimpleDropSink
    {
        public HealthImpactFunctions myHealthImpactFunctions;
        public HealthImapctDropSink()
        {
            this.CanDropBetween = true;
            this.CanDropOnBackground = true;
            this.CanDropOnItem = false;
        }

        public HealthImapctDropSink(bool acceptDropsFromOtherLists, HealthImpactFunctions healthImpactFunctions)
            : this()
        {
            this.AcceptExternal = acceptDropsFromOtherLists;
            myHealthImpactFunctions = healthImpactFunctions;
        }

        protected override void OnModelCanDrop(ModelDropEventArgs args)
        {
            base.OnModelCanDrop(args);
            if (args.Handled)
                return;

            args.Effect = DragDropEffects.Move;

            if (!this.AcceptExternal && args.SourceListView != this.ListView)
            {
                args.Effect = DragDropEffects.None;
                args.DropTargetLocation = DropTargetLocation.None;
                args.InfoMessage = "This list doesn't accept drops from other lists";
            }

            if (args.DropTargetLocation == DropTargetLocation.Background && args.SourceListView == this.ListView)
            {
                args.Effect = DragDropEffects.None;
                args.DropTargetLocation = DropTargetLocation.None;
            }
        }

        protected override void OnModelDropped(ModelDropEventArgs args)
        {
            if (!args.Handled)
                this.RearrangeModels(args);
        }

        public virtual void RearrangeModels(ModelDropEventArgs args)
        {
            switch (args.DropTargetLocation)
            {
                case DropTargetLocation.AboveItem:
                    if (this.ListView.Name == "olvSelected" && args.SourceListView.Name != "olvSelected")
                    {
                        myHealthImpactFunctions.btAddCRFunctions_Click(null, null);

                    }
                    else
                    {


                    }
                    break;
                case DropTargetLocation.BelowItem:
                    if (this.ListView.Name == "olvSelected" && args.SourceListView.Name != "olvSelected")
                    {
                        myHealthImpactFunctions.btAddCRFunctions_Click(null, null);
                    }
                    else
                    {

                    }
                    break;
                case DropTargetLocation.Background:
                    if (this.ListView.Name == "olvSelected" && args.SourceListView.Name != "olvSelected")
                    {
                        myHealthImpactFunctions.btAddCRFunctions_Click(null, null);
                    }
                    else
                    {

                    }
                    break;
                default:
                    return;
            }


        }
    }
}
