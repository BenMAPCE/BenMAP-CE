using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESIL.DBUtility;
using System.Text.RegularExpressions;
using BenMAP.Tools;
using System.IO;
using System.Text;

namespace BenMAP
{
    public partial class HIFDefinitionMulti : FormBase
    {
        private static Tools.CalculateFunctionString _pointEstimateEval;
        internal static Tools.CalculateFunctionString PointEstimateEval
        {
            get
            {
                if (_pointEstimateEval == null)
                    _pointEstimateEval = new Tools.CalculateFunctionString();
                return _pointEstimateEval;
            }

        }

        private string _dataName;

        private HealthImpact _healthImpacts;
        public HealthImpact HealthImpacts
        {
            get { return _healthImpacts; }
            set { _healthImpacts = value; }
        }

        /* private List<double> _listCustom;
        public List<double> listCustom
        {
            get { return _listCustom; }
            set { _listCustom = value; }
        } */

        public HIFDefinitionMulti()
        {
            InitializeComponent();
            _dataName = string.Empty;
            _healthImpacts = new HealthImpact();
            if (_healthImpacts.PollVariables == null) _healthImpacts.PollVariables = new List<CRFVariable>();
            foreach (var poll in _healthImpacts.PollVariables)
            {
                if (poll.PollBetas == null)
                {
                    poll.PollBetas = new List<CRFBeta>();
                    poll.PollBetas.Add(new CRFBeta());
                }
            }
        }

        public HIFDefinitionMulti(string dataName, HealthImpact healthImpact)// , List<double> listValue)
        {
            InitializeComponent();
            _healthImpacts = healthImpact.DeepCopy();
            _dataName = dataName;
            // _listCustom = listValue;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet dsFunctions = new DataSet();
                string commandText = "select * from FUNCTIONALFORMS";
                dsFunctions = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                List<string> listFunctions = new List<string>();
                int rowFunctionsCount = dsFunctions.Tables[0].Rows.Count;
                for (int i = 0; i < rowFunctionsCount; i++)
                {
                    listFunctions.Add(dsFunctions.Tables[0].Rows[i][1].ToString());
                }

                commandText = "select * from BASELINEFUNCTIONALFORMS";
                DataSet dsBaselineFunctions = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                List<string> listBaselineFunctions = new List<string>();
                int rowBaselineFunctionsCount = dsBaselineFunctions.Tables[0].Rows.Count;
                for (int j = 0; j < rowBaselineFunctionsCount; j++)
                {
                    listBaselineFunctions.Add(dsBaselineFunctions.Tables[0].Rows[j][1].ToString());
                }
                List<string> lstSystemVariableName = Configuration.ConfigurationCommonClass.getAllSystemVariableNameList();
                string functionText = Configuration.ConfigurationCommonClass.getFunctionStringFromDatabaseFunction(txtFunction.Text);
                Dictionary<string, double> dicVariable = new Dictionary<string, double>();
                Dictionary<string, string> dicEstimateVariables = new Dictionary<string, string>();
                string DatabaseFunction = functionText.Replace("prevalence", "").Replace("incidence", "").Replace("deltaq", "")
                   .Replace("pop", "").Replace("beta", "").Replace("q0", "").Replace("q1", "")
                   .Replace("abs", " ").Replace("acos", " ").Replace("asin", " ").Replace("atan", " ")
                   .Replace("atan2", " ").Replace("bigmul", " ").Replace("ceiling", " ").Replace("cos", " ")
                   .Replace("cosh", " ").Replace("divrem", " ").Replace("exp", " ").Replace("floor", " ")
                   .Replace("ieeeremainder", " ").Replace("log", " ").Replace("log10", " ").Replace("max", " ")
                   .Replace("min", " ").Replace("pow", " ").Replace("round", " ").Replace("sign", " ")
                   .Replace("sin", " ").Replace("sinh", " ").Replace("sqrt", " ").Replace("tan", " ")
                   .Replace("tanh", " ").Replace("truncate", " ");
                int crid = 1;
                foreach (string s in lstSystemVariableName)
                {
                    if (DatabaseFunction.ToLower().Contains(s.ToLower()))
                    {
                        dicVariable.Add(s, 1);
                        if (dicEstimateVariables.ContainsKey(crid.ToString()))
                        {
                            if (dicEstimateVariables[crid.ToString()] == "")
                                dicEstimateVariables[crid.ToString()] = " double " + s.ToLower();
                            else if (!dicEstimateVariables[crid.ToString()].Contains("double " + s.ToLower()))
                                dicEstimateVariables[crid.ToString()] += " , double " + s.ToLower();
                        }
                        else
                        {
                            dicEstimateVariables.Add(crid.ToString(), " double " + s.ToLower());
                        }
                    }
                }
                Dictionary<string, string> dicEstimate = new Dictionary<string, string>();
                dicEstimate.Add(crid.ToString(), functionText);

                Dictionary<string, List<string>> dicVariableList = new Dictionary<string, List<string>>();
                List<string> addNames = new List<string>();
                foreach (CRFVariable v in HealthImpacts.PollVariables)
                {
                    addNames.Add(v.VariableName);
                }
                dicVariableList.Add(crid.ToString(), addNames);

                CalculateFunctionString calculateFunctionString = new CalculateFunctionString();
                calculateFunctionString.CreateAllPointEstimateEvalObjects(dicEstimate, dicEstimateVariables, dicVariableList);
                Dictionary<string, double> dicTest = new Dictionary<string, double>();
                foreach (string varName in addNames)
                { 
                    dicTest.Add(varName, 1);
                }   
                object result = PointEstimateEval.PointEstimateEval(crid.ToString(), functionText, 1, 1, 1, dicTest, dicTest, dicTest, dicTest, 1, 1, 1, dicVariable);
                if (Tools.CalculateFunctionString.dicPointEstimateMethodInfo != null) Tools.CalculateFunctionString.dicPointEstimateMethodInfo.Clear();
                if (!(result is double) || double.IsNaN(Convert.ToDouble(result)) || Convert.ToDouble(result) == -999999999)
                {
                    MessageBox.Show("Please input a valid value for 'Function'.");
                    return;
                }
                else
                {
                    if (!listFunctions.Contains(txtFunction.Text))
                    {
                        commandText = "select max(FUNCTIONALFORMID) from FUNCTIONALFORMS";
                        object objFunction = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                        int functionsID = int.Parse(objFunction.ToString()) + 1;
                        commandText = string.Format("insert into FunctionalForms values ({0},'{1}')", functionsID, txtFunction.Text);
                        int rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    }
                }
                functionText = Configuration.ConfigurationCommonClass.getFunctionStringFromDatabaseFunction(txtBaselineIncidenceFunction.Text);
                dicVariable = new Dictionary<string, double>();
                dicEstimateVariables = new Dictionary<string, string>();
                DatabaseFunction = functionText.Replace("prevalence", "").Replace("incidence", "").Replace("deltaq", "")
                   .Replace("pop", "").Replace("beta", "").Replace("q0", "").Replace("q1", "")
                   .Replace("abs", " ").Replace("acos", " ").Replace("asin", " ").Replace("atan", " ")
                   .Replace("atan2", " ").Replace("bigmul", " ").Replace("ceiling", " ").Replace("cos", " ")
                   .Replace("cosh", " ").Replace("divrem", " ").Replace("exp", " ").Replace("floor", " ")
                   .Replace("ieeeremainder", " ").Replace("log", " ").Replace("log10", " ").Replace("max", " ")
                   .Replace("min", " ").Replace("pow", " ").Replace("round", " ").Replace("sign", " ")
                   .Replace("sin", " ").Replace("sinh", " ").Replace("sqrt", " ").Replace("tan", " ")
                   .Replace("tanh", " ").Replace("truncate", " ");
                foreach (string s in lstSystemVariableName)
                {
                    if (DatabaseFunction.ToLower().Contains(s.ToLower()))
                    {
                        dicVariable.Add(s, 1);
                        if (dicEstimateVariables.ContainsKey(crid.ToString()))
                        {
                            if (dicEstimateVariables[crid.ToString()] == "")
                                dicEstimateVariables[crid.ToString()] = " double " + s.ToLower();
                            else if (!dicEstimateVariables[crid.ToString()].Contains("double " + s.ToLower()))
                                dicEstimateVariables[crid.ToString()] += " , double " + s.ToLower();
                        }
                        else
                        {
                            dicEstimateVariables.Add(crid.ToString(), " double " + s.ToLower());
                        }
                    }
                }
                dicEstimate = new Dictionary<string, string>();
                dicEstimate.Add(crid.ToString(), functionText);

                calculateFunctionString = new CalculateFunctionString();
                calculateFunctionString.CreateAllBaselineEvalObjects(dicEstimate, dicEstimateVariables, dicVariableList);
                result = PointEstimateEval.BaseLineEval(crid.ToString(), functionText, 1, 1, 1, dicTest, dicTest, dicTest, dicTest, 1, 1, 1, dicVariable);
                if (Tools.CalculateFunctionString.dicPointEstimateMethodInfo != null) Tools.CalculateFunctionString.dicPointEstimateMethodInfo.Clear();
                if (!(result is double) || double.IsNaN(Convert.ToDouble(result)) || Convert.ToDouble(result) == -999999999)
                {
                    MessageBox.Show("Please input a valid value for 'Baseline Function'.");
                    return;
                }
                else
                {
                    if (!listBaselineFunctions.Contains(txtBaselineIncidenceFunction.Text))
                    {
                        commandText = "select max(FUNCTIONALFORMID) from BASELINEFUNCTIONALFORMS";
                        object objBaselineFunction = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                        int BaselienFunctionID = int.Parse(objBaselineFunction.ToString()) + 1;
                        commandText = string.Format("insert into BaselineFunctionalForms values ({0},'{1}')", BaselienFunctionID, txtBaselineIncidenceFunction.Text);
                        int rtn = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    }
                }

                bool ok = false;
                ok = IsValidDate(txtYear.Text);
                if (ok == false)
                {
                    MessageBox.Show("Please input a valid year.");
                    return;
                }

                if (nudownStartAge.Value > nudownEndAge.Value)
                {
                    MessageBox.Show("The end age must be higher than start age.");
                    return;
                }

                _healthImpacts.EndpointGroup = cboEndpointGroup.Text;
                _healthImpacts.Endpoint = cboEndpoint.Text;
                _healthImpacts.Pollutant = cboPollutant.Text;
                _healthImpacts.MetricStatistis = cboMetricStatistic.Text;
                _healthImpacts.SeasonalMetric = cboSeasonalMetric.Text;
                _healthImpacts.Race = cboRace.Text;
                _healthImpacts.Ethnicity = cboEthnicity.Text;
                _healthImpacts.Gender = cboGender.Text;
                _healthImpacts.StartAge = nudownStartAge.Value.ToString();
                _healthImpacts.EndAge = nudownEndAge.Value.ToString();
                _healthImpacts.Author = txtAnthor.Text;
                _healthImpacts.Year = txtYear.Text;
                _healthImpacts.Location = txtLocation.Text;
                _healthImpacts.LocationName = cboLocationName.Text;
                _healthImpacts.Qualifier = txtQualifier.Text;
                _healthImpacts.OtherPollutant = txtOtherPollutant.Text;
                _healthImpacts.Reference = txtReference.Text;
                _healthImpacts.Function = txtFunction.Text;
                _healthImpacts.BaselineIncidenceFunction = txtBaselineIncidenceFunction.Text;
                _healthImpacts.Incidence = cboIncidenceDataSet.Text;
                _healthImpacts.Prevalence = cboPrevalenceDataSet.Text;
                _healthImpacts.Variable = cboVariableDataSet.Text;
                // _listCustom = list;

                this.DialogResult = DialogResult.OK;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

        }

        public bool IsValidDate(string strIn)
        {

            return Regex.IsMatch(strIn, @"^[0-9][0-9][0-9][0-9]$");
        }

        private void HIFDefinitionMulti_Load(object sender, EventArgs e)
        {
            try
            {
                // Edit existing function
                if (_dataName != string.Empty)
                {
                    BindItems();
                    cboEndpointGroup.Text = _healthImpacts.EndpointGroup;
                    cboEndpoint.Text = _healthImpacts.Endpoint;
                    cboPollutant.Text = _healthImpacts.Pollutant;
                    cboMetricStatistic.Text = _healthImpacts.MetricStatistis;
                    if (_healthImpacts.SeasonalMetric == string.Empty) cboSeasonalMetric.SelectedIndex = -1;
                    else cboSeasonalMetric.Text = _healthImpacts.SeasonalMetric;
                    cboRace.Text = _healthImpacts.Race;
                    cboEthnicity.Text = _healthImpacts.Ethnicity;
                    cboGender.Text = _healthImpacts.Gender;
                    nudownStartAge.Value = int.Parse(_healthImpacts.StartAge);
                    nudownEndAge.Value = int.Parse(_healthImpacts.EndAge);
                    txtAnthor.Text = _healthImpacts.Author;
                    txtYear.Text = _healthImpacts.Year;
                    txtOtherPollutant.Text = _healthImpacts.OtherPollutant;
                    txtLocation.Text = _healthImpacts.Location;
                    cboLocationName.Text = _healthImpacts.LocationName;
                    txtQualifier.Text = _healthImpacts.Qualifier;
                    txtReference.Text = _healthImpacts.Reference;
                    txtFunction.Text = _healthImpacts.Function;
                    txtBaselineIncidenceFunction.Text = _healthImpacts.BaselineIncidenceFunction;
                    cboIncidenceDataSet.Text = _healthImpacts.Incidence;
                    cboPrevalenceDataSet.Text = _healthImpacts.Prevalence;
                    cboVariableDataSet.Text = _healthImpacts.Variable;
                    // list = listCustom;

                    if (_healthImpacts.BetaVariation == "Seasonal") bvSeasonal.Checked = true;
                    else bvFullYear.Checked = true;
                    
                }
                // Add new function
                else 
                {
                    BindItems();
                    cboPollutant.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        public void BindItems()
        {
            try
            {
                string commandText = "select ENDPOINTGROUPNAME from ENDPOINTGROUPS ";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboEndpointGroup.DataSource = ds.Tables[0];
                cboEndpointGroup.DisplayMember = "ENDPOINTGROUPNAME";
                if (cboEndpointGroup.Items.Count > 0) cboEndpointGroup.SelectedIndex = 0;
                cboEndpointGroup.DropDownWidth = 250;

                commandText = string.Format("select PGName, PollutantGroupID from PollutantGroups where setupid={0} order by PollutantGroupID asc", CommonClass.MainSetup.SetupID);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboPollutant.DataSource = ds.Tables[0];
                cboPollutant.DisplayMember = "PGNAME";

                // Load Model Specifications based on single or group
                string str = _healthImpacts.Pollutant;
                commandText = string.Format("select count(POLLUTANTID) from POLLUTANTGROUPPOLLUTANTS where POLLUTANTGROUPID in (select POLLUTANTGROUPID from POLLUTANTGROUPS where PGName ='{0}')", str);
                fb = new ESIL.DBUtility.ESILFireBirdHelper();
                int count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));

                if (count == 1) { commandText = "select MSDESCRIPTION from MODELSPECIFICATIONS where MSID=1"; }
                else { commandText = "select MSDESCRIPTION from MODELSPECIFICATIONS where MSID!=1"; }

                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    cboModelSpec.Items.Add(dr["MSDESCRIPTION"]);
                } 
                cboModelSpec.SelectedItem = _healthImpacts.ModelSpec;

                commandText = "select ETHNICITYNAME from ETHNICITY";
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);

                cboEthnicity.DataSource = ds.Tables[0];
                cboEthnicity.DisplayMember = "ETHNICITYNAME";
                cboEthnicity.SelectedIndex = -1;

                commandText = "select RACENAME from RACES";
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboRace.DataSource = ds.Tables[0];
                cboRace.DisplayMember = "RACENAME";
                cboRace.SelectedIndex = -1;

                commandText = "select GENDERNAME from GENDERS";
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboGender.DataSource = ds.Tables[0];
                cboGender.DisplayMember = "GENDERNAME"; 
                cboGender.SelectedIndex = -1;
                cboMetricStatistic.Items.Add("None");
                cboMetricStatistic.Items.Add("Mean");
                cboMetricStatistic.Items.Add("Median");
                cboMetricStatistic.Items.Add("Max");
                cboMetricStatistic.Items.Add("Min");
                cboMetricStatistic.Items.Add("Sum");
                cboMetricStatistic.SelectedIndex = 0;

                string groupSelected = cboPollutant.Text;
                commandText = string.Format("select distinct SEASONALMETRICNAME from SEASONALMETRICS inner join METRICS on SEASONALMETRICS.METRICID = METRICS.METRICID inner join POLLUTANTS on METRICS.POLLUTANTID = POLLUTANTS.POLLUTANTID inner join POLLUTANTGROUPPOLLUTANTS on POLLUTANTS.POLLUTANTID = POLLUTANTGROUPPOLLUTANTS.POLLUTANTID inner join POLLUTANTGROUPS on POLLUTANTGROUPS.POLLUTANTGROUPID = POLLUTANTGROUPPOLLUTANTS.POLLUTANTGROUPID and PGNAME='{0}'", groupSelected);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboSeasonalMetric.Items.Add("None");
                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    cboSeasonalMetric.Items.Add(dr["SeasonalMetricName"]);
                }
                cboSeasonalMetric.DisplayMember = "SeasonalMetricName";
                cboSeasonalMetric.SelectedIndex = -1;

                commandText = string.Format("select INCIDENCEDATASETNAME from INCIDENCEDATASETS where setupid={0} order by INCIDENCEDATASETNAME asc", CommonClass.ManageSetup.SetupID);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboIncidenceDataSet.DataSource = ds.Tables[0];
                cboIncidenceDataSet.DisplayMember = "INCIDENCEDATASETNAME";
                cboIncidenceDataSet.SelectedIndex = -1;

                commandText = string.Format("select INCIDENCEDATASETNAME from INCIDENCEDATASETS where setupid={0} order by INCIDENCEDATASETNAME asc", CommonClass.ManageSetup.SetupID);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboPrevalenceDataSet.DataSource = ds.Tables[0];
                cboPrevalenceDataSet.DisplayMember = "INCIDENCEDATASETNAME";
                cboPrevalenceDataSet.SelectedIndex = -1;

                commandText = string.Format("select SETUPVARIABLEDATASETNAME from SETUPVARIABLEDATASETS where setupid={0}", CommonClass.ManageSetup.SetupID);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboVariableDataSet.DataSource = ds.Tables[0];
                cboVariableDataSet.DisplayMember = "SETUPVARIABLEDATASETNAME";
                cboVariableDataSet.SelectedIndex = -1;
                string[] AvailableFunctions = new string[] { "ABS(x)", "EXP(x)", "LOG(x)", "POW(x,y)", "SQR(x)", "ACOS(x)", "ASIN(x)", "ATAN(x)", "ATAN2(x,y)", "BIGMUL(x,y)", "CEILING(x)", "COS(x)", "COSH(x)", "DIVREM(x,y,z)", "FLOOR(x)", "IEEEREMAINDER(x,y)", "LOG10(x)", "MAX(x,y)", "MIN(x,y)", "ROUND(x,y)", "SIGN(x)", "SIN(x)", "SINH(x)", "TAN(x)", "TANH(x)", "TRUNCATE(x)" };
                lstFuncAvailableFunctions.Items.AddRange(AvailableFunctions);

                commandText = "select * from COMMONFNFORMS";
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstCommonUsedFunctions.DataSource = ds.Tables[0];
                lstCommonUsedFunctions.DisplayMember = "FUNCTIONALFORMTEXT";
                lstCommonUsedFunctions.SelectedIndex = -1;
                string[] AvailableVariables = new string[] { "Beta", "DELTAQ", "POP", "Incidence", "Prevalence", "Q0", "Q1", "A", "B", "C" };
                lstFuncAvailableVariables.Items.AddRange(AvailableVariables);
                lstFuncAvailableVariables.SelectedIndex = -1;

                commandText = string.Format("select distinct lower(SetupVariableName) as SetupVariableName from SetupVariables where setupvariabledatasetid in (select setupvariabledatasetid from setupvariabledatasets where setupid={0})", CommonClass.ManageSetup.SetupID);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstFuncAvailableSetupVariables.DataSource = ds.Tables[0];
                lstFuncAvailableSetupVariables.DisplayMember = "SetupVariableName";
                lstFuncAvailableSetupVariables.SelectedIndex = -1;
                lstBaselineAvailableSetupVariables.DataSource = ds.Tables[0];
                lstBaselineAvailableSetupVariables.DisplayMember = "SetupVariableName";
                lstBaselineAvailableSetupVariables.SelectedIndex = -1;
                string[] BaselineAvailableFunctions = new string[] { "ABS(x)", "EXP(x)", "LOG(x)", "POW(x,y)", "SQR(x)", "ACOS(x)", "ASIN(x)", "ATAN(x)", "ATAN2(x,y)", "BIGMUL(x,y)", "CEILING(x)", "COS(x)", "COSH(x)", "DIVREM(x,y,z)", "FLOOR(x)", "IEEEREMAINDER(x,y)", "LOG10(x)", "MAX(x,y)", "MIN(x,y)", "ROUND(x,y)", "SIGN(x)", "SIN(x)", "SINH(x)", "TAN(x)", "TANH(x)", "TRUNCATE(x)" };
                lstBaselineAvailableFunctions.Items.AddRange(BaselineAvailableFunctions);
                lstBaselineAvailableFunctions.SelectedIndex = -1;

                commandText = "select FUNCTIONALFORMTEXT from COMMONBLFNFORMS";
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstBaselineCommonUsedFunctions.DataSource = ds.Tables[0];
                lstBaselineCommonUsedFunctions.DisplayMember = "FUNCTIONALFORMTEXT";
                lstBaselineCommonUsedFunctions.SelectedIndex = -1;
                string[] BaselineAvailableVariables = new string[] { "Beta", "DELTAQ", "POP", "Incidence", "Prevalence", "Q0", "Q1", "A", "B", "C" };
                lstBaselineAvailableVariables.Items.AddRange(BaselineAvailableVariables);
                lstBaselineAvailableVariables.SelectedIndex = -1;

                commandText = "select LocationTypeName from LocationType";
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboLocationName.DataSource = ds.Tables[0];
                cboLocationName.DisplayMember = "LocationTypeName";
                cboLocationName.SelectedIndex = -1;


                // Set up variable objects -- order by char_length and variable name to avoid 1, 10, 2 ordering
                commandText = string.Format("select distinct variablename, crv.crfvariableid, pollutantname, pollutant1id, pollutant2id, metricname, m.metricid, hourlymetricgeneration from crfunctions as crf left join crfvariables as crv on crf.crfunctionid = crv.crfunctionid join metrics as m on m.pollutantid = crv.pollutant1id where crf.crfunctionid = {0} and m.metricid in (select metricid from crfvariables as crv2 where crv.crfunctionid = crv2.crfunctionid) order by char_length(variablename), variablename", _healthImpacts.FunctionID);

                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);

                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    varList.Items.Add(dr["variablename"].ToString()).SubItems.Add(dr["pollutantname"].ToString());

                    // interaction
                    if (dr["pollutantname"].ToString().Contains("*"))
                    {
                        _healthImpacts.PollVariables.Add(new CRFVariable(dr["variablename"].ToString(), Convert.ToInt32(dr["crfvariableid"]), Convert.ToInt32(_healthImpacts.FunctionID), dr["pollutantname"].ToString(), Convert.ToInt32(dr["pollutant1id"]), Convert.ToInt32(dr["pollutant2id"])));
                    }
                    else
                    {
                        Metric newMetric = new Metric();
                        newMetric.MetricID = Convert.ToInt32(dr["metricid"]);
                        newMetric.MetricName = dr["metricname"].ToString();
                        newMetric.PollutantID = Convert.ToInt32(dr["pollutant1id"]);
                        newMetric.HourlyMetricGeneration = Convert.ToInt32(dr["hourlymetricgeneration"]);

                        _healthImpacts.PollVariables.Add(new CRFVariable(dr["variablename"].ToString(), Convert.ToInt32(dr["crfvariableid"]), Convert.ToInt32(_healthImpacts.FunctionID), dr["pollutantname"].ToString(), Convert.ToInt32(dr["pollutant1id"]), newMetric));
                    }
                }

                varList.Columns[1].Width = -1;
                if (varList.Columns[1].Width < 123) varList.Columns[1].Width = 123;

                // Set up beta objects
                int j = 0;
                int numSeasons = 1;
                string aDesc, bDesc, cDesc;
                double a, b, c, p1, p2;
                foreach (var pv in _healthImpacts.PollVariables)
                {
                    commandText = string.Format("select crfbetaid, beta, a, namea, b, nameb, c, namec, p1beta, p2beta, seasonalmetricseasonname, startday, endday, distributionname from crfvariables v left join crfbetas b on b.crfvariableid=v.crfvariableid join distributiontypes dt on b.distributiontypeid=dt.distributiontypeid left join seasonalmetricseasons s on s.seasonalmetricseasonid=b.seasonalmetricseasonid where crfunctionid={0} and pollutantname='{1}' order by startday", _healthImpacts.FunctionID, pv.PollutantName);
                    ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    numSeasons = ds.Tables[0].Rows.Count;

                    if (pv.PollBetas == null) pv.PollBetas = new List<CRFBeta>();

                    if(numSeasons > 1)
                    {
                        for(int i = 1; i <= numSeasons; i++)
                        {
                            DataRow dr = ds.Tables[0].Rows[i - 1];
                            str = string.Format("Season {0}", i);

                            CRFBeta newBeta = new CRFBeta();
                            newBeta.SeasNumName = str;
                            newBeta.BetaID = Convert.ToInt32(dr["crfbetaid"]);
                            newBeta.Beta = Convert.ToDouble(dr["beta"]);
                            newBeta.SeasonName = dr["seasonalmetricseasonname"].ToString();
                            newBeta.StartDate = dr["startday"].ToString();
                            newBeta.EndDate = dr["endday"].ToString();
                            newBeta.Distribution = dr["distributionname"].ToString();                            

                            if (dr["p1beta"].ToString() != string.Empty)
                                newBeta.P1Beta = Convert.ToDouble(dr["p1beta"]);

                            if (dr["p2beta"].ToString() != string.Empty) 
                                newBeta.P2Beta = Convert.ToDouble(dr["p2beta"]);

                            if (dr["a"].ToString() != string.Empty) 
                                newBeta.AConstantValue = Convert.ToDouble(dr["a"]);

                            if (dr["namea"].ToString() != string.Empty) 
                                newBeta.AConstantName = dr["namea"].ToString();

                            if (dr["b"].ToString() != string.Empty) 
                                newBeta.BConstantValue = Convert.ToDouble(dr["b"]);

                            if (dr["nameb"].ToString() != string.Empty) 
                                newBeta.BConstantName = dr["nameb"].ToString();

                            if (dr["c"].ToString() != string.Empty) 
                                newBeta.CConstantValue = Convert.ToDouble(dr["c"]);

                            if (dr["namec"].ToString() == string.Empty) 
                                newBeta.CConstantName = dr["namec"].ToString();

                            pv.PollBetas.Add(newBeta);
                        }
                    }

                    else if (numSeasons == 1)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        CRFBeta newBeta = new CRFBeta();
                        newBeta.BetaID = Convert.ToInt32(dr["crfbetaid"]);
                        newBeta.Beta = Convert.ToDouble(dr["beta"]);
                        newBeta.Distribution = dr["distributionname"].ToString();

                        if (dr["p1beta"].ToString() != string.Empty)
                            newBeta.P1Beta = Convert.ToDouble(dr["p1beta"]);

                        if (dr["p2beta"].ToString() != string.Empty)
                            newBeta.P2Beta = Convert.ToDouble(dr["p2beta"]);

                        if (dr["a"].ToString() != string.Empty)
                            newBeta.AConstantValue = Convert.ToDouble(dr["a"]);

                        if (dr["namea"].ToString() != string.Empty)
                            newBeta.AConstantName = dr["namea"].ToString();

                        if (dr["b"].ToString() != string.Empty)
                            newBeta.BConstantValue = Convert.ToDouble(dr["b"]);

                        if (dr["nameb"].ToString() != string.Empty)
                            newBeta.BConstantName = dr["nameb"].ToString();

                        if (dr["c"].ToString() != string.Empty)
                            newBeta.CConstantValue = Convert.ToDouble(dr["c"]);

                        if (dr["namec"].ToString() == string.Empty)
                            newBeta.CConstantName = dr["namec"].ToString();

                        pv.PollBetas.Add(newBeta);
                    }

                    else
                    {
                        MessageBox.Show("No pollutant/ variable data in the database for this function");
                        return;
                    }
                    j++;
                }

                // ToEdit -- Custom set up for now until database changes can occur
                if (_healthImpacts.PollVariables.Count() == 1 && _healthImpacts.PollVariables.First().PollBetas.Count == 1
                    && _healthImpacts.PollVariables.First().PollBetas.First().Distribution == "Custom")
                {
                    CRFBeta temp = _healthImpacts.PollVariables.First().PollBetas.First();
                    fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    ds = new DataSet();
                    commandText = string.Format("select Vvalue from CRFUNCTIONCUSTOMENTRIES where CRFUNCTIONID={0}", _healthImpacts.FunctionID);
                    ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);

                    foreach(DataRow d in ds.Tables[0].Rows)
                    {
                        temp.CustomList.Add(Convert.ToDouble(d["Vvalue"]));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void cboPollutant_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // Load Model Specifications based on single or group
                string str = cboPollutant.Text;
                string commandText = string.Format("select count(POLLUTANTID) from POLLUTANTGROUPPOLLUTANTS where POLLUTANTGROUPID in (select POLLUTANTGROUPID from POLLUTANTGROUPS where PGName ='{0}')", str);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                int count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));

                if (count == 1) { commandText = "select MSDESCRIPTION from MODELSPECIFICATIONS where MSID=1"; }
                else { commandText = "select MSDESCRIPTION from MODELSPECIFICATIONS where MSID!=1"; }

                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboModelSpec.DataSource = ds.Tables[0];
                cboModelSpec.DisplayMember = "MSDESCRIPTION";

                _healthImpacts.Pollutant = cboPollutant.Text;

                refreshVariableList();
                updateBetas_EditOrNew();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        // Set up for future editing of health impact functions 
        private void cboModelSpec_SelectedValueChanged(object sender, EventArgs e)
        {
            refreshVariableList();
        }

        private void refreshVariableList()
        {
            try
            {
                // Fill Variable List
                int i = 1;
                bool isFirstOrder = false;
                string str = cboPollutant.Text;
                string varName = string.Empty;
                varList.Invalidate();
                varList.Items.Clear();
                _healthImpacts.PollVariables.Clear();
                _healthImpacts.PollVariables = new List<CRFVariable>();

                if (cboModelSpec.Text.Contains("first order")) isFirstOrder = true;
                List<string> firstOrder = new List<string>();
                SortedSet<string> foHashSet = new SortedSet<string>();

                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select POLLUTANTNAME, POLLUTANTS.POLLUTANTID from POLLUTANTS inner join POLLUTANTGROUPPOLLUTANTS on POLLUTANTS.POLLUTANTID = POLLUTANTGROUPPOLLUTANTS.POLLUTANTID inner join POLLUTANTGROUPS on POLLUTANTGROUPS.POLLUTANTGROUPID = POLLUTANTGROUPPOLLUTANTS.POLLUTANTGROUPID and PGNAME='{0}' order by POLLUTANTNAME asc", str);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    varName = string.Format("P{0}", i);
                    varList.Items.Add(varName).SubItems.Add(dr["POLLUTANTNAME"].ToString());
                    _healthImpacts.PollVariables.Add(new CRFVariable(varName, dr["POLLUTANTNAME"].ToString(), Convert.ToInt32(dr["POLLUTANTID"])));
                    if (isFirstOrder) firstOrder.Add(dr["POLLUTANTNAME"].ToString());
                    i++;
                }

                if (isFirstOrder)
                {
                    // Get all combinations of the pollutants
                    // Sorted HashSet and item comparison used to avoid duplicates 
                    foreach (var item1 in firstOrder)
                    {
                        foreach (var item2 in firstOrder)
                        {
                            if (item1 != item2)
                            {
                                if (item1.CompareTo(item2) > 0) { foHashSet.Add(item2 + "*" + item1); }
                                else { foHashSet.Add(item1 + "*" + item2); }
                            }
                        }
                    }
                    foreach (var toAdd in foHashSet)
                    {
                        varName = string.Format("P{0}", i);
                        varList.Items.Add(varName).SubItems.Add(toAdd);
                        _healthImpacts.PollVariables.Add(new CRFVariable(varName, toAdd, -1));
                        i++;
                    }
                }

                foreach (var poll in _healthImpacts.PollVariables)
                {
                    if (poll.PollBetas == null)
                    {
                        poll.PollBetas = new List<CRFBeta>();
                        poll.PollBetas.Add(new CRFBeta());
                    }
                }

                varList.Columns[1].Width = -1;
                if (varList.Columns[1].Width < 123) varList.Columns[1].Width = 123;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void lstFuncAvailableCompiledFunctions_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                int index = txtFunction.SelectionStart;
                txtFunction.Text = lstCommonUsedFunctions.Text;
                txtFunction.SelectionStart = txtFunction.Text.Length;
                txtFunction.Focus();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void lstFuncAllAvailableFunctions_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                int index = txtFunction.SelectionStart;
                txtFunction.Text = lstCommonUsedFunctions.Text;
                txtFunction.SelectionStart = txtFunction.Text.Length;
                txtFunction.Focus();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void lstFuncAvailableVariables_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                int index = txtFunction.SelectionStart;
                txtFunction.Text = txtFunction.Text.Insert(index, lstFuncAvailableVariables.Text);
                txtFunction.SelectionStart = txtFunction.Text.Length;
                txtFunction.Focus();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void lstFuncAvailableSetupVariables_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                int index = txtFunction.SelectionStart;
                txtFunction.Text = txtFunction.Text.Insert(index, lstFuncAvailableSetupVariables.Text);
                txtFunction.SelectionStart = txtFunction.Text.Length;
                txtFunction.Focus();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void lstBaselineAvailableCompiledFunctions_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                int index = txtBaselineIncidenceFunction.SelectionStart;
                txtBaselineIncidenceFunction.Text = lstBaselineCommonUsedFunctions.Text;
                txtBaselineIncidenceFunction.SelectionStart = txtBaselineIncidenceFunction.Text.Length;
                txtBaselineIncidenceFunction.Focus();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void lstBaselineAllAvailableFunctions_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                int index = txtBaselineIncidenceFunction.SelectionStart;
                txtBaselineIncidenceFunction.Text = lstBaselineCommonUsedFunctions.Text;
                txtBaselineIncidenceFunction.SelectionStart = txtBaselineIncidenceFunction.Text.Length;
                txtBaselineIncidenceFunction.Focus();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void lstBaselineAvailableVariables_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                int index = txtBaselineIncidenceFunction.SelectionStart;
                txtBaselineIncidenceFunction.Text = txtBaselineIncidenceFunction.Text.Insert(index, lstBaselineAvailableVariables.Text);
                txtBaselineIncidenceFunction.SelectionStart = txtBaselineIncidenceFunction.Text.Length;
                txtBaselineIncidenceFunction.Focus();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void lstBaselineAvailableSetupVariables_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                int index = txtBaselineIncidenceFunction.SelectionStart;
                txtBaselineIncidenceFunction.Text = txtBaselineIncidenceFunction.Text.Insert(index, lstBaselineAvailableSetupVariables.Text);
                txtBaselineIncidenceFunction.SelectionStart = txtBaselineIncidenceFunction.Text.Length;
                txtBaselineIncidenceFunction.Focus();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void lstFuncAvailableFunctions_DoubleClick(object sender, EventArgs e)
        {
            Dictionary<string, string> dicFuncAvailableFunctions = new Dictionary<string, string>();
            dicFuncAvailableFunctions.Add("ABS(x)", "ABS()");
            dicFuncAvailableFunctions.Add("EXP(x)", "EXP()");
            dicFuncAvailableFunctions.Add("LOG(x)", "LOG()");
            dicFuncAvailableFunctions.Add("POW(x,y)", "POW(,)");
            dicFuncAvailableFunctions.Add("SQR(x)", "SQR()");
            dicFuncAvailableFunctions.Add("ACOS(x)", "ACOS()");
            dicFuncAvailableFunctions.Add("ASIN(x)", "ASIN()");
            dicFuncAvailableFunctions.Add("ATAN(x)", "ATAN()");
            dicFuncAvailableFunctions.Add("ATAN2(x,y)", "ATAN2(,)");
            dicFuncAvailableFunctions.Add("BIGMUL(x,y)", "BIGMUL(,)");
            dicFuncAvailableFunctions.Add("CEILING(x)", "CEILING()");
            dicFuncAvailableFunctions.Add("COS(x)", "COS()");
            dicFuncAvailableFunctions.Add("COSH(x)", "COSH()");
            dicFuncAvailableFunctions.Add("DIVREM(x,y,z)", "DIVREM(,,)");
            dicFuncAvailableFunctions.Add("FLOOR(x)", "FLOOR()");
            dicFuncAvailableFunctions.Add("IEEEREMAINDER(x,y)", "IEEEREMAINDER(,)");
            dicFuncAvailableFunctions.Add("LOG10(x)", "LOG10()");
            dicFuncAvailableFunctions.Add("MAX(x,y)", "MAX(,)");
            dicFuncAvailableFunctions.Add("MIN(x,y)", "MIN(,)");
            dicFuncAvailableFunctions.Add("ROUND(x,y)", "ROUND(,)");
            dicFuncAvailableFunctions.Add("SIGN(x)", "SIGN()");
            dicFuncAvailableFunctions.Add("SIN(x)", "SIN()");
            dicFuncAvailableFunctions.Add("SINH(x)", "SINH()");
            dicFuncAvailableFunctions.Add("TAN(x)", "TAN()");
            dicFuncAvailableFunctions.Add("TANH(x)", "TANH()");
            dicFuncAvailableFunctions.Add("TRUNCATE(x)", "TRUNCATE()");
            string insert = lstFuncAvailableFunctions.SelectedItem.ToString();
            string insertFunction = dicFuncAvailableFunctions[insert];
            int index = txtFunction.SelectionStart;
            txtFunction.Text = txtFunction.Text.Insert(index, insertFunction);
            txtFunction.SelectionStart = txtFunction.Text.Length;
        }

        private void lstBaselineAvailableFunctions_DoubleClick(object sender, EventArgs e)
        {
            Dictionary<string, string> dicBaselineAvailableFunctions = new Dictionary<string, string>();
            dicBaselineAvailableFunctions.Add("ABS(x)", "ABS()");
            dicBaselineAvailableFunctions.Add("EXP(x)", "EXP()");
            dicBaselineAvailableFunctions.Add("LOG(x)", "LOG()");
            dicBaselineAvailableFunctions.Add("POW(x,y)", "POW(,)");
            dicBaselineAvailableFunctions.Add("SQR(x)", "SQR()");
            dicBaselineAvailableFunctions.Add("ACOS(x)", "ACOS()");
            dicBaselineAvailableFunctions.Add("ASIN(x)", "ASIN()");
            dicBaselineAvailableFunctions.Add("ATAN(x)", "ATAN()");
            dicBaselineAvailableFunctions.Add("ATAN2(x,y)", "ATAN2(,)");
            dicBaselineAvailableFunctions.Add("BIGMUL(x,y)", "BIGMUL(,)");
            dicBaselineAvailableFunctions.Add("CEILING(x)", "CEILING()");
            dicBaselineAvailableFunctions.Add("COS(x)", "COS()");
            dicBaselineAvailableFunctions.Add("COSH(x)", "COSH()");
            dicBaselineAvailableFunctions.Add("DIVREM(x,y,z)", "DIVREM(,,)");
            dicBaselineAvailableFunctions.Add("FLOOR(x)", "FLOOR()");
            dicBaselineAvailableFunctions.Add("IEEEREMAINDER(x,y)", "IEEEREMAINDER(,)");
            dicBaselineAvailableFunctions.Add("LOG10(x)", "LOG10()");
            dicBaselineAvailableFunctions.Add("MAX(x,y)", "MAX(,)");
            dicBaselineAvailableFunctions.Add("MIN(x,y)", "MIN(,)");
            dicBaselineAvailableFunctions.Add("ROUND(x,y)", "ROUND(,)");
            dicBaselineAvailableFunctions.Add("SIGN(x)", "SIGN()");
            dicBaselineAvailableFunctions.Add("SIN(x)", "SIN()");
            dicBaselineAvailableFunctions.Add("SINH(x)", "SINH()");
            dicBaselineAvailableFunctions.Add("TAN(x)", "TAN()");
            dicBaselineAvailableFunctions.Add("TANH(x)", "TANH()");
            dicBaselineAvailableFunctions.Add("TRUNCATE(x)", "TRUNCATE()");
            string insert = lstBaselineAvailableFunctions.SelectedItem.ToString();
            string insertFunction = dicBaselineAvailableFunctions[insert];
            int index = txtBaselineIncidenceFunction.SelectionStart;
            txtBaselineIncidenceFunction.Text = txtBaselineIncidenceFunction.Text.Insert(index, insertFunction);
            txtBaselineIncidenceFunction.SelectionStart = txtBaselineIncidenceFunction.Text.Length;
        }

        private void txtYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46 || keyValue == 45)
            {
                if (e.KeyChar == 45 && (((TextBox)sender).SelectionStart == 0 && ((TextBox)sender).Text.IndexOf("-") >= 0))
                    e.Handled = true;
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }


        private void cboEndpointGroup_SelectedValueChanged_1(object sender, EventArgs e)
        {
            try
            {
                string str = cboEndpointGroup.Text;
                string commandText = string.Format("select * from ENDPOINTS where ENDPOINTGROUPID=(select ENDPOINTGROUPID from ENDPOINTGROUPS where ENDPOINTGROUPNAME='{0}' )", str);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboEndpoint.DataSource = ds.Tables[0];
                cboEndpoint.DisplayMember = "ENDPOINTNAME";
                int maxWidth = 177;
                int width = 177;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    using (Graphics g = this.CreateGraphics())
                    {
                        SizeF string_size = g.MeasureString(dr["ENDPOINTNAME"].ToString(), this.Font);
                        width = Convert.ToInt16(string_size.Width) + 50;
                    }
                    maxWidth = Math.Max(maxWidth, width);
                }
                cboEndpoint.DropDownWidth = maxWidth;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void editEffect_Click(object sender, EventArgs e)
        {
            try
            {
                if (varList.Items.Count == 0)
                {
                    MessageBox.Show("You must select Pollutant(s) and Model Specification before you can view Effect Coefficients.");
                    return;
                }

                // Set index for list if there is a variable selected or show first variable otherwise
                int selectedVar = 0;
                if (varList.SelectedItems.Count != 0) selectedVar = varList.SelectedItems[0].Index;

                updateObjectFromForm();

                // Make copy of HIF to pass to new form
                HealthImpact hifPass = new HealthImpact();
                hifPass = HealthImpacts.DeepCopy();

                EffectCoefficients form = new EffectCoefficients(hifPass, selectedVar);
                DialogResult res = form.ShowDialog();

                if (res != DialogResult.OK) return;

                // If user clicked OK, update object with beta values from Effect Coefficients form
                int i = 0;
                foreach (CRFVariable pv in _healthImpacts.PollVariables)
                {
                    if (pv.PollBetas == null) pv.PollBetas = new List<CRFBeta>();
                    else { pv.PollBetas.Clear(); pv.PollBetas = new List<CRFBeta>(); }

                    if(form.HIF.PollVariables[i].PollBetas.Count() > 0)
                        pv.PollBetas.AddRange(form.HIF.PollVariables[i].PollBetas);

                    i++;
                }
            }

            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        // Updates fields needed for Effect Coefficients from form
        private void updateObjectFromForm()
        {
            if (cboPollutant.SelectedItem != null)
                _healthImpacts.Pollutant = cboPollutant.SelectedItem.ToString();
            if (cboModelSpec.SelectedItem != null)
                _healthImpacts.ModelSpec = cboModelSpec.GetItemText(this.cboModelSpec.SelectedItem);
            if (cboMetricStatistic.SelectedItem != null)
                _healthImpacts.MetricStatistis = cboMetricStatistic.SelectedItem.ToString();
            if (cboSeasonalMetric.SelectedItem != null)
                _healthImpacts.SeasonalMetric = cboSeasonalMetric.SelectedItem.ToString();
            if (_healthImpacts.BetaVariation == "") cboSeasonalMetric_SelectedIndexChanged(null, null);
        }

        // Used to toggle Beta Variation
        private void cboSeasonalMetric_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSeasonalMetric.SelectedItem.ToString().Equals("None"))
            {
                bvFullYear.Checked = true;
                _healthImpacts.BetaVariation = "Full year";
                _healthImpacts.SeasonalMetric = cboSeasonalMetric.Text;
            }
            else
            {
                bvSeasonal.Checked = true;
                _healthImpacts.BetaVariation = "Seasonal";
                _healthImpacts.SeasonalMetric = cboSeasonalMetric.Text;
            }

            updateBetas_EditOrNew();
        }

        // Set up beta objects for editing or creating new functions
        // Set up with 0's for beta and constants 
        // If seasonal, the season name, start day, and end day will be defined in the database
        private void updateBetas_EditOrNew()
        {
            try
            {
                if (_healthImpacts.Pollutant == string.Empty || _healthImpacts.Pollutant == null || varList.Items.Count == 0) return;

                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

                if (bvSeasonal.Checked && _healthImpacts.SeasonalMetric != null)
                {
                    // CRFBeta temp = new CRFBeta();
                    foreach (CRFVariable pv in _healthImpacts.PollVariables)
                    {
                        int i = 1;
                        pv.PollBetas.Clear();

                        pv.Metric = new Metric();

                        string commandText = string.Format("select distinct startday, endday, seasonalmetricseasonname from crfvariables v left join crfbetas b on b.crfvariableid=v.crfvariableid left join seasonalmetricseasons s on s.seasonalmetricseasonid=b.seasonalmetricseasonid join seasonalmetrics sm on sm.SEASONALMETRICID = s.SEASONALMETRICID where seasonalmetricname='{0}' and pollutantname='{1}' order by startday", _healthImpacts.SeasonalMetric, pv.PollutantName);
                        DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);

                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            CRFBeta temp = new CRFBeta();
                            temp.StartDate = dr["startday"].ToString();
                            temp.EndDate = dr["endday"].ToString();
                            temp.SeasonName = dr["seasonalmetricseasonname"].ToString();
                            temp.SeasNumName = string.Format("Season {0}", i);
                            pv.PollBetas.Add(temp);

                            i++;
                        }
                    }

                }
                else
                {
                    // Set up one beta per variable with blank slate 
                    foreach (CRFVariable pv in _healthImpacts.PollVariables)
                    {
                        pv.PollBetas.Clear();
                        pv.PollBetas.Add(new CRFBeta());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}
