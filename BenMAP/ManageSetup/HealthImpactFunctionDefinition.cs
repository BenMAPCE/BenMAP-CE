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

namespace BenMAP
{
    public partial class HealthImpactFunctionOfUser_defined : FormBase
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
        private List<double> _listCustom;
        public List<double> listCustom
        {
            get { return _listCustom; }
            set { _listCustom = value; }
        }
        public HealthImpactFunctionOfUser_defined()
        {
            InitializeComponent();
            _dataName = string.Empty;
            _healthImpacts = new HealthImpact();
        }

        public HealthImpactFunctionOfUser_defined(string dataName, HealthImpact healthImpact, List<double> listValue)
        {
            InitializeComponent();
            _healthImpacts = healthImpact;
            _dataName = dataName;
            _listCustom = listValue;
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

                if (txtBetaParameter1.Text == string.Empty)
                {
                    MessageBox.Show("'Beta Parameter 1' can not be null. Please input a valid value.");
                    return;
                }
                if (txtBetaParameter2.Text == string.Empty)
                {
                    MessageBox.Show("'Beta Parameter 2' can not be null. Please input a valid value.");
                    return;
                }
                if (txtBeta.Text == string.Empty)
                {
                    MessageBox.Show("'Beta' can not be null. Please input a valid value.");
                    return;
                }
                if (txtCconstantValue.Text == string.Empty)
                {
                    MessageBox.Show("'C' can not be null. Please input a valid value.");
                    return;
                }
                if (txtBconstantValue.Text == string.Empty)
                {
                    MessageBox.Show("'B' can not be null. Please input a valid value.");
                    return;
                }
                if (nudownStartAge.Value > nudownEndAge.Value)
                {
                    MessageBox.Show("The end age must be higher than start age.");
                    return;
                }
                if (txtAconstantValue.Text == string.Empty)
                {
                    MessageBox.Show("'A' can not be null. Please input a valid value.");
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
                _healthImpacts.BetaDistribution = cboBetaDistribution.Text;
                _healthImpacts.Beta = txtBeta.Text;
                _healthImpacts.BetaParameter1 = txtBetaParameter1.Text;
                _healthImpacts.BetaParameter2 = txtBetaParameter2.Text;
                _healthImpacts.AConstantDescription = txtAconstantDescription.Text;
                _healthImpacts.AConstantValue = txtAconstantValue.Text;
                _healthImpacts.BConstantDescription = txtBconstantDescription.Text;
                _healthImpacts.BConstantValue = txtBconstantValue.Text;
                _healthImpacts.CConstantDescription = txtCconstantDescription.Text;
                _healthImpacts.CConstantValue = txtCconstantValue.Text;
                _healthImpacts.Incidence = cboIncidenceDataSet.Text;
                _healthImpacts.Prevalence = cboPrevalenceDataSet.Text;
                _healthImpacts.Variable = cboVariableDataSet.Text;
                _listCustom = list;
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

        private void HealthImpactFunctionOfUser_defined_Load(object sender, EventArgs e)
        {
            try
            {

                if (_dataName != string.Empty)
                {

                    BindItems();
                    cboBetaDistribution.Items.Add("None");
                    cboBetaDistribution.Items.Add("Normal");
                    cboBetaDistribution.Items.Add("Triangular");
                    cboBetaDistribution.Items.Add("Poisson");
                    cboBetaDistribution.Items.Add("Binomial");
                    cboBetaDistribution.Items.Add("LogNormal");
                    cboBetaDistribution.Items.Add("Uniform");
                    cboBetaDistribution.Items.Add("Exponential");
                    cboBetaDistribution.Items.Add("Geometric");
                    cboBetaDistribution.Items.Add("Weibull");
                    cboBetaDistribution.Items.Add("Gamma");
                    cboBetaDistribution.Items.Add("Logistic");
                    cboBetaDistribution.Items.Add("Beta");
                    cboBetaDistribution.Items.Add("Pareto");
                    cboBetaDistribution.Items.Add("Cauchy");
                    cboBetaDistribution.Items.Add("Custom"); 
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
                    cboBetaDistribution.Text = _healthImpacts.BetaDistribution;
                    txtBeta.Text = _healthImpacts.Beta;
                    txtBetaParameter1.Text = _healthImpacts.BetaParameter1;
                    txtBetaParameter2.Text = _healthImpacts.BetaParameter2;
                    txtCconstantValue.Text = _healthImpacts.CConstantValue;
                    txtCconstantDescription.Text = _healthImpacts.CConstantDescription;
                    txtBconstantValue.Text = _healthImpacts.BConstantValue;
                    txtBconstantDescription.Text = _healthImpacts.BConstantDescription;
                    txtAconstantValue.Text = _healthImpacts.AConstantValue;
                    txtAconstantDescription.Text = _healthImpacts.AConstantDescription;
                    cboIncidenceDataSet.Text = _healthImpacts.Incidence;
                    cboPrevalenceDataSet.Text = _healthImpacts.Prevalence;
                    cboVariableDataSet.Text = _healthImpacts.Variable;
                    list = listCustom;

                }
                else
                {
                    BindItems();
                    cboBetaDistribution.Items.Add("None");
                    cboBetaDistribution.Items.Add("Normal");
                    cboBetaDistribution.Items.Add("Triangular");
                    cboBetaDistribution.Items.Add("Poisson");
                    cboBetaDistribution.Items.Add("Binomial");
                    cboBetaDistribution.Items.Add("LogNormal");
                    cboBetaDistribution.Items.Add("Uniform");
                    cboBetaDistribution.Items.Add("Exponential");
                    cboBetaDistribution.Items.Add("Geometric");
                    cboBetaDistribution.Items.Add("Weibull");
                    cboBetaDistribution.Items.Add("Gamma");
                    cboBetaDistribution.Items.Add("Logistic");
                    cboBetaDistribution.Items.Add("Beta");
                    cboBetaDistribution.Items.Add("Pareto");
                    cboBetaDistribution.Items.Add("Cauchy");
                    cboBetaDistribution.Items.Add("Custom");
                    cboBetaDistribution.SelectedIndex = 0; 

                }
                cboBetaDistribution.SelectedValueChanged -= cboBetaDistribution_SelectedValueChanged;
                cboBetaDistribution.SelectedValueChanged += cboBetaDistribution_SelectedValueChanged;
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
                if (cboEndpointGroup.Items.Count > 0)
                    cboEndpointGroup.SelectedIndex = 0;
                cboEndpointGroup.DropDownWidth = 250; commandText = "select POLLUTANTNAME from POLLUTANTS where setupID=" + CommonClass.ManageSetup.SetupID;
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboPollutant.DataSource = ds.Tables[0];
                cboPollutant.DisplayMember = "POLLUTANTNAME";
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
                string str = cboPollutant.Text;
                string commandText = string.Format("select * from METRICS where POLLUTANTID=(select POLLUTANTID from POLLUTANTS where POLLUTANTNAME='{0}'and setupID={1} )", str, CommonClass.ManageSetup.SetupID);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboMetric.DataSource = ds.Tables[0];
                cboMetric.DisplayMember = "METRICNAME";
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

                string str = cboMetric.Text;
                DataTable dt = (DataTable)cboMetric.DataSource;
                string metricID = "";
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["METRICNAME"].ToString() == str)
                    { metricID = dr["METRICID"].ToString(); }
                }
                if (string.IsNullOrEmpty(metricID)) return;
                string commandText = "select '' as SeasonalMetricName from SeasonalMetrics union select SeasonalMetricName from SeasonalMetrics where MetricID=" + metricID;
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

        public List<double> list = new List<double>();
        private void cboBetaDistribution_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                HealthImpact healthImpactValues = new HealthImpact();
                healthImpactValues.BetaDistribution = cboBetaDistribution.SelectedItem.ToString();
                healthImpactValues.Beta = txtBeta.Text;
                healthImpactValues.BetaParameter1 = txtBetaParameter1.Text;
                healthImpactValues.BetaParameter2 = txtBetaParameter2.Text;
                if (cboBetaDistribution.SelectedItem == "None") { return; }
                if (cboBetaDistribution.SelectedItem == "Custom")
                {
                    if (list.Count == 0)
                    {
                        CustomDistributionEntries frm = new CustomDistributionEntries();
                        DialogResult rtn = frm.ShowDialog();
                        if (rtn != DialogResult.OK) { return; }
                        list = frm.list;
                    }
                    else
                    {
                        CustomDistributionEntries frmCustom = new CustomDistributionEntries(list);
                        DialogResult rtnCustom = frmCustom.ShowDialog();
                        if (rtnCustom != DialogResult.OK) { return; }
                        list = frmCustom.list;
                    }
                }
                else
                {
                    EditDistributionValues frm = new EditDistributionValues(cboBetaDistribution.SelectedItem.ToString(), healthImpactValues);
                    DialogResult rtn = frm.ShowDialog();
                    if (rtn != DialogResult.OK) { return; }
                    txtBeta.Text = frm.MeanValue;
                    txtBetaParameter1.Text = frm.Parameter1;
                    txtBetaParameter2.Text = frm.Parameter2;
                    if (cboBetaDistribution.SelectedItem == "Normal" || cboBetaDistribution.SelectedItem == "Poisson" || cboBetaDistribution.SelectedItem == "Exponential" || cboBetaDistribution.SelectedItem == "Geometric")
                    {
                        txtBetaParameter2.Text = healthImpactValues.BetaParameter2;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

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
    }
}
