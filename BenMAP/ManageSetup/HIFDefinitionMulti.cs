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
                CalculateFunctionString calculateFunctionString = new CalculateFunctionString();
                calculateFunctionString.CreateAllPointEstimateEvalObjects(dicEstimate, dicEstimateVariables);
                object result = PointEstimateEval.PointEstimateEval(crid.ToString(), functionText, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, dicVariable);
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
                calculateFunctionString.CreateAllPointEstimateEvalObjects(dicEstimate, dicEstimateVariables);
                result = PointEstimateEval.PointEstimateEval(crid.ToString(), functionText, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, dicVariable);
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
                _healthImpacts.Metric = cboMetric.Text;
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
                    cboMetric.Text = _healthImpacts.Metric;
                    cboMetricStatistic.Text = _healthImpacts.MetricStatistis;
                    cboSeasonalMetric.Text = _healthImpacts.SeasonalMetric;
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

                // Load Metrics 
                commandText = string.Format("select METRICNAME, COUNT(*) as occur from METRICS inner join POLLUTANTS on METRICS.POLLUTANTID = POLLUTANTS.POLLUTANTID inner join POLLUTANTGROUPPOLLUTANTS on POLLUTANTS.POLLUTANTID = POLLUTANTGROUPPOLLUTANTS.POLLUTANTID inner join POLLUTANTGROUPS on POLLUTANTGROUPS.POLLUTANTGROUPID = POLLUTANTGROUPPOLLUTANTS.POLLUTANTGROUPID and PGNAME='{0}' group by METRICNAME order by COUNT(*) desc", str);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                DataSet intersect = ds.Copy();

                int i = 0;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (Convert.ToInt32(dr["occur"]) != count)
                    {
                        intersect.Tables[0].Rows.RemoveAt(i);
                        i--;
                    }
                    i++;
                }
                cboMetric.DataSource = intersect.Tables[0];
                cboMetric.DisplayMember = "METRICNAME";

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void cboModelSpec_SelectedValueChanged(object sender, EventArgs e)
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

        private void cboMetric_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                // Load Seasonal Metric according to selected Metric 
                string metricSelected = cboMetric.Text;
                string groupSelected = cboPollutant.Text;
                string commandText = string.Format("select distinct SEASONALMETRICNAME from SEASONALMETRICS inner join METRICS on SEASONALMETRICS.METRICID = METRICS.METRICID and METRICNAME='{0}' inner join POLLUTANTS on METRICS.POLLUTANTID = POLLUTANTS.POLLUTANTID inner join POLLUTANTGROUPPOLLUTANTS on POLLUTANTS.POLLUTANTID = POLLUTANTGROUPPOLLUTANTS.POLLUTANTID inner join POLLUTANTGROUPS on POLLUTANTGROUPS.POLLUTANTGROUPID = POLLUTANTGROUPPOLLUTANTS.POLLUTANTGROUPID and PGNAME='{1}'", metricSelected, groupSelected);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboSeasonalMetric.DataSource = ds.Tables[0];
                cboSeasonalMetric.DisplayMember = "SeasonalMetricName";
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

        private void txtBeta_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtBetaParameter1_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtBetaParameter2_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtAconstantValue_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtBconstantValue_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtCconstantValue_KeyPress(object sender, KeyPressEventArgs e)
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
                // Set index for list if there is a variable selected or show first variable otherwise
                int selectedVar = 0;
                if (varList.SelectedItems.Count != 0) selectedVar = varList.SelectedItems[0].Index;

                // Make sure object is updated
                if (_healthImpacts.BetaVariation == "") betaVarGroup_SelectedValueChanged(sender, e);
                DataRowView selectedModel = (DataRowView)cboModelSpec.SelectedItem;
                _healthImpacts.ModelSpec = selectedModel[0].ToString();

                // Make copy of HIF to pass to new form
                HealthImpact hifPass = new HealthImpact();
                hifPass = HealthImpacts.DeepCopy();

                EffectCoefficients form = new EffectCoefficients(hifPass, selectedVar);
                DialogResult res = form.ShowDialog();

                if (res != DialogResult.OK) return;

                // If user clicked OK, update object with beta values from Effect Coefficients form
                int i = 0;
                foreach (var pv in _healthImpacts.PollVariables)
                {
                    if (pv.PollBetas == null) pv.PollBetas = new List<CRFBeta>();
                    pv.PollBetas.AddRange(form.HIF.PollVariables[i].PollBetas);
                    i++;
                }
            }

            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void betaVarGroup_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                /* Once betas are in database, list of CRFBetas will be updated here
                   based on the toggle to reflect full year or seasons */
                if (bvFullYear.Checked) _healthImpacts.BetaVariation = "Full year";
                else _healthImpacts.BetaVariation = "Seasonal";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}
