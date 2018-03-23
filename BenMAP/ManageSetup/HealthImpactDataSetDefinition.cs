using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using System.IO;
using ESIL.DBUtility;

namespace BenMAP
{
    public partial class HealthImpactDataSetDefinition : FormBase
    {
        private DataTable dt;//_dtDataFile;
        private MetadataClassObj _metadataObj = null;
        DataTable _dt = new DataTable();
        DataTable _dtEndpointGroup = new DataTable();
        DataTable _dtPollutant = new DataTable();
        private int _datasetID;
        private int crFunctionDataSetID = 0;
        private bool _isEdit = false;
        private const int HEALTHIMPACTDATASETID = 6; // BenMAP-322 - hardcoded to health impact dataset type
        List<int> lstdeleteCRFunctionid = new List<int>();
        // Dictionary<int, List<CRFVariable>> dicVariables = new Dictionary<int, List<CRFVariable>>(); 
        // CRFunctionId, List of CRFVariables for that function
        private Dictionary<int, List<CRFVariable>> variableLists;
        // Used to flag when variables have been changed/ 
        // inserts should be made instead of update
        private List<int> toInsert; 

        public HealthImpactDataSetDefinition()
        {
            InitializeComponent();
            getcrFunctionDatasetID();
            _datasetID = -1;

            variableLists = new Dictionary<int, List<CRFVariable>>();
            // initialize variable and beta lists??
            toInsert = new List<int>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthImpactDataSetDefinition"/> class.
        /// </summary>
        /// <param name="dataSetID">The data set identifier which is the current function dataset id (CrfunctiondatasetID).</param>
        public HealthImpactDataSetDefinition(int dataSetID, bool isEdit)
        {   //this function should be called when doing an edit
            InitializeComponent();
            _datasetID = dataSetID;
            crFunctionDataSetID = dataSetID;//when doing an edit I need to have the current funciton dataset ID
            _isEdit = isEdit;
            txtHealthImpactFunction.Enabled = false;

            variableLists = new Dictionary<int, List<CRFVariable>>();
            // initialize variable and beta lists??
            toInsert = new List<int>();
        }

        private void getcrFunctionDatasetID()
        {
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            //Getting an ID
            //Getting a new current Function Dataset Id - But I don't need a new one if I am add to or / editing a dataset
            string commandText = string.Format("select max(CRFUNCTIONDATASETID) from CRFunctionDatasets");
            object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
            //int crFunctionDataSetID = int.Parse(obj.ToString()) + 1;
            crFunctionDataSetID = int.Parse(obj.ToString()) + 1;
        }

        string _filePath = string.Empty;
        List<double> listCustomValue = new List<double>();
        Dictionary<int, List<double>> dicCustomValue = new Dictionary<int, List<double>>();
        int AddCount = 0;

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                listCustomValue.Clear();
                HIFDefinitionMulti frm = new HIFDefinitionMulti();
                DialogResult rth = frm.ShowDialog();
                if (rth != DialogResult.OK) { return; }
                AddCount--;
                DataRow dr = _dt.NewRow();
                dr[0] = frm.HealthImpacts.EndpointGroup;
                dr[1] = frm.HealthImpacts.Endpoint;
                dr[2] = frm.HealthImpacts.Pollutant;
                dr[3] = frm.HealthImpacts.SeasonalMetric;
                dr[5] = frm.HealthImpacts.MetricStatistic;
                dr[6] = frm.HealthImpacts.Author;
                dr[7] = frm.HealthImpacts.Year;
                dr[8] = frm.HealthImpacts.LocationName;
                dr[9] = frm.HealthImpacts.Location;
                dr[10] = frm.HealthImpacts.OtherPollutant;
                dr[11] = frm.HealthImpacts.Qualifier;
                dr[12] = frm.HealthImpacts.Reference;
                dr[13] = frm.HealthImpacts.Race;
                dr[14] = frm.HealthImpacts.Ethnicity;
                dr[15] = frm.HealthImpacts.Gender;
                dr[16] = frm.HealthImpacts.StartAge;
                dr[17] = frm.HealthImpacts.EndAge;
                dr[18] = frm.HealthImpacts.Function;
                dr[19] = frm.HealthImpacts.BaselineIncidenceFunction;
                dr[20] = frm.HealthImpacts.Incidence;
                dr[21] = frm.HealthImpacts.Prevalence;
                dr[22] = frm.HealthImpacts.Variable;
                dr[23] = frm.HealthImpacts.ModelSpec;
                dr[24] = frm.HealthImpacts.BetaVariation;
                dr[25] = AddCount;

                variableLists.Add(AddCount, frm.HealthImpacts.PollVariables);

                _dt.Rows.Add(dr);
                olvFunction.DataSource = _dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        DataTable dtLoad = new DataTable();
        // DataTable dtForLoading = new DataTable();
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            DataSet ds = new DataSet();
            string commandText = string.Empty;

            if (!_isEdit)
            {
                commandText = string.Format("select * from  CRFunctionDataSets where SetupID={0}", CommonClass.ManageSetup.SetupID);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                int dataSetNameCount = ds.Tables[0].Rows.Count;
                for (int dataSetNameRow = 0; dataSetNameRow < dataSetNameCount; dataSetNameRow++)
                {
                    if (txtHealthImpactFunction.Text == ds.Tables[0].Rows[dataSetNameRow]["CRFunctionDataSetName"].ToString())
                    {
                        MessageBox.Show("This health impact function dataset name is already in use. Please enter a different name.");
                        return;
                    }
                }
                if (txtHealthImpactFunction.Text == string.Empty)
                {
                    MessageBox.Show("Please input a name for the health impact function dataset.");
                    return;
                } 
            }
            LoadFromFile();
        }

        private void LoadFromFile()
        {
            LoadSelectedDataSet lmdataset = new LoadSelectedDataSet("Load Health Impact Dataset", "Health Impact Dataset Name:", txtHealthImpactFunction.Text, "Healthfunctions");
            DialogResult dlgr = lmdataset.ShowDialog();
            if (dlgr.Equals(DialogResult.OK))
            {
                dt = lmdataset.MonitorDataSet;
                // 2015 09 29 - BENMAP-353 
                _metadataObj = lmdataset.MetadataObj;
                
                olvFunction.ClearObjects();
                
                //LoadDatabase();
                LoadFunctionOLV();
                //after loading the datafile, the dataset is Edit flag should be reset to true.  
                //This will allow for additional files to be added.
                _isEdit = true;
            }
        }

        //this is populating the olvFunction
        private void LoadFunctionOLV()
        {
            //This load is to the gridview on the Health Impact Data Set Definition and not to the database.
            //The database load is done in the btnOK_Click_1 event.
            try
            {
                #region Dead code
                //DataTable dt = new DataTable();
                //OpenFileDialog openFileDialog = new OpenFileDialog();
                //openFileDialog.InitialDirectory = Application.StartupPath + @"E:\";
                //openFileDialog.Filter = "All Files|*.*|CSV files|*.csv|XLS files|*.xls|XLSX files|*.xlsx";
                //openFileDialog.FilterIndex = 2;
                //openFileDialog.RestoreDirectory = true;
                //if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
                //_filePath = openFileDialog.FileName;
                //WaitShow("Loading health impact functions...");
                //dt = CommonClass.ExcelToDataTable(_filePath); 
                #endregion

                if (dt == null) { return; }
                
                int rowCount = dt.Rows.Count;
                int colCount = dt.Columns.Count;

                int iEndpointGroup = -1;
                int iEndpoint = -1;
                int iPollutant = -1;
                int iMetric = -1;
                int iSeasonalMetric = -1;
                int iMetricStatistic = -1;
                int iAuthor = -1;
                int iYear = -1;
                int iLocationType = -1;
                int iLocation = -1;
                int iOtherPollutant = -1;
                int iQualifier = -1;
                int iReference = -1;
                int iRace = -1;
                int iEthnicity = -1;
                int iGender = -1;
                int iStartAge = -1;
                int iEndAge = -1;
                int iFunction = -1;
                int iBaselineFunction = -1;
                int iIncidenceDataset = -1;
                int iPrevalenceDataset = -1;
                int iVariableDataset = -1;
                int iModelSpecification = -1;
                int iBetaVariation = -1;

                for (int i = 0; i < colCount; i++)
                {
                    if (dt.Columns[i].ColumnName.ToLower().Contains("statistic"))
                    { iMetricStatistic = i; }
                    if (dt.Columns[i].ColumnName.ToLower().Contains("baseline") && dt.Columns[i].ColumnName.ToLower().Contains("function"))
                    { iBaselineFunction = i; }
                    if (dt.Columns[i].ColumnName.ToLower().Contains("author"))
                    { iAuthor = i; }
                    if (dt.Columns[i].ColumnName.ToLower().Contains("year"))
                    { iYear = i; }
                    if (dt.Columns[i].ColumnName.ToLower().Replace(" ", "").Contains("locationtype"))
                    { iLocationType = i; }
                    if (dt.Columns[i].ColumnName.ToLower().Replace(" ", "").Contains("location") && (!dt.Columns[i].ColumnName.ToLower().Replace(" ", "").Contains("type")))
                    { iLocation = i; }
                    switch (dt.Columns[i].ColumnName.ToLower().Replace(" ", ""))
                    {
                        case "endpointgroup": iEndpointGroup = i;
                            break;
                        case "endpoint": iEndpoint = i;
                            break;
                        case "pollutant": iPollutant = i;
                            break;
                        case "metric": iMetric = i;
                            break;
                        case "seasonalmetric": iSeasonalMetric = i;
                            break;
                        case "otherpollutants": iOtherPollutant = i;
                            break;
                        case "qualifier": iQualifier = i;
                            break;
                        case "reference": iReference = i;
                            break;
                        case "race": iRace = i;
                            break;
                        case "ethnicity": iEthnicity = i;
                            break;
                        case "gender": iGender = i;
                            break;
                        case "startage": iStartAge = i;
                            break;
                        case "endage": iEndAge = i;
                            break;
                        case "function": iFunction = i;
                            break;
                        case "incidencedataset": iIncidenceDataset = i;
                            break;
                        case "prevalencedataset": iPrevalenceDataset = i;
                            break;
                        case "variabledataset": iVariableDataset = i;
                            break;
                        case "modelspecification": iModelSpecification = i;
                            break;
                        case "betavariation": iBetaVariation = i;
                            break;
                    }
                }

                string warningtip = "";
                if (iEndpointGroup < 0) warningtip = "'Endpoint Group', ";
                if (iEndpoint < 0) warningtip += "'Endpoint', ";
                if (iPollutant < 0) warningtip += "'Pollutant', ";
                if (iMetric < 0) warningtip += "'Metric', ";
                if (iSeasonalMetric < 0) warningtip += "'Seasonal Metric', ";
                if (iMetricStatistic < 0) warningtip += "'Metric Statistic', ";
                if (iAuthor < 0) warningtip += "'Study Author', ";
                if (iYear < 0) warningtip += "'Study Year', ";
                if (iLocation < 0) warningtip += "'Study Location', ";
                if (iOtherPollutant < 0) warningtip += "'Other Pollutants', ";
                if (iQualifier < 0) warningtip += "'Qualifier', ";
                if (iReference < 0) warningtip += "'Reference', ";
                if (iRace < 0) warningtip += "'Race', ";
                if (iGender < 0) warningtip += "'Gender', ";
                if (iStartAge < 0) warningtip += "'StartAge', ";
                if (iEndAge < 0) warningtip += "'EndAge', ";
                if (iFunction < 0) warningtip += "'Function', ";
                if (iIncidenceDataset < 0) warningtip += "'Incidence Dataset', ";
                if (iPrevalenceDataset < 0) warningtip += "'Prevalence Dataset', ";
                if (iModelSpecification < 0) warningtip += "'Model Specification', ";
                if (iBetaVariation < 0) warningtip += "'Beta Variation', ";
                if (warningtip != "")
                {
                    WaitClose();
                    warningtip = warningtip.Substring(0, warningtip.Length - 2);
                    warningtip = "Please check the column header of " + warningtip + ". It is incorrect or does not exist.";
                    MessageBox.Show(warningtip, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //dtForLoading = _dt.Clone();
                for (int i = 0; i < rowCount; i++)
                {
                    DataRow dr = _dt.NewRow();
                    dr[0] = dt.Rows[i][iEndpointGroup];
                    dr[1] = dt.Rows[i][iEndpoint];
                    dr[2] = dt.Rows[i][iPollutant];
                    dr[3] = dt.Rows[i][iMetric];
                    dr[4] = dt.Rows[i][iSeasonalMetric];
                    dr[5] = dt.Rows[i][iMetricStatistic];
                    dr[6] = dt.Rows[i][iAuthor];
                    dr[7] = dt.Rows[i][iYear];
                    if (iLocationType < 0)
                    {
                        dr[8] = "NULL";
                    }
                    else
                    {
                        dr[8] = dt.Rows[i][iLocationType];
                    }
                    dr[9] = dt.Rows[i][iLocation];
                    dr[10] = dt.Rows[i][iOtherPollutant];
                    dr[11] = dt.Rows[i][iQualifier];
                    dr[12] = dt.Rows[i][iReference];
                    dr[13] = dt.Rows[i][iRace];
                    if (iEthnicity < 0)
                    {
                        dr[14] = "NULL";
                    }
                    else
                    {
                        dr[14] = dt.Rows[i][iEthnicity];
                    }
                    dr[15] = dt.Rows[i][iGender];
                    dr[16] = dt.Rows[i][iStartAge];
                    dr[17] = dt.Rows[i][iEndAge];
                    dr[18] = dt.Rows[i][iFunction];
                    if (iBaselineFunction < 0)
                    {
                        dr[19] = "";
                    }
                    else
                    {
                        dr[19] = dt.Rows[i][iBaselineFunction];
                    }
                    dr[20] = dt.Rows[i][iIncidenceDataset];
                    dr[21] = dt.Rows[i][iPrevalenceDataset];
                    if (iVariableDataset < 0)
                    {
                        dr[22] = "NULL";
                    }
                    else
                    {
                        dr[22] = dt.Rows[i][iVariableDataset];
                    }
                    if (iModelSpecification < 0)
                    {
                        dr[23] = "";
                    }
                    else
                    {
                        dr[23] = dt.Rows[i][iModelSpecification];
                    }
                    if (iBetaVariation < 0)
                    {
                        dr[24] = "";
                    }
                    else
                    {
                        dr[24] = dt.Rows[i][iBetaVariation];
                    }
                    dr[25] = --AddCount;
                    _dt.Rows.Add(dr);
                    //dtForLoading.ImportRow(dr);
                    // removed next row to avoid double importing of file records
                    //_dt.ImportRow(dr);
                }   
                    
                //dtLoad = _dt;
                int dtRow = _dt.Rows.Count;
                string strTableName = string.Empty;
                string strPolluantName = string.Empty;
                if (!cboFilterEndpointGroup.Items.Contains("All"))
                { cboFilterEndpointGroup.Items.Add("All"); }
                if (!cboFilterPollutants.Items.Contains("All"))
                { cboFilterPollutants.Items.Add("All"); }
                for (int i = 0; i < dtRow; i++)
                {
                    strTableName = _dt.Rows[i][0].ToString();
                    if (!cboFilterEndpointGroup.Items.Contains(strTableName))
                        cboFilterEndpointGroup.Items.Add(strTableName);
                    strPolluantName = _dt.Rows[i][2].ToString();
                    if (!cboFilterPollutants.Items.Contains(strPolluantName))
                        cboFilterPollutants.Items.Add(strPolluantName);
                }
                olvFunction.DataSource = _dt;
                WaitClose();
            }
            catch (Exception ex)
            {
                WaitClose();
                Logger.LogError(ex);
            }
        }

        private void LoadDatabase()
        {   //for this block only - replacing _dt with dtForLoading.  this is so that only new files that get loaded will/should get assoceated with the new metadata id.
            if (_dt.Rows.Count < 1)
            {
                MessageBox.Show("No dataset was selected for import or created.  Please select a dataset to import or 'Add' information to create a data set.");
                btnBrowse.Focus();
                return;
            }
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            DataSet ds = new DataSet();
            string commandText = string.Empty;
            object obj = null;
            //int crFunctionDataSetID = 0;

            try
            {
                string undefinePollutant = "";

                // Dictionaries used to match names and IDs for saving to database

                Dictionary<string, int> dicEndpointGroup = new Dictionary<string, int>();
                commandText = "select EndpointGroupID,LOWER(EndpointGroupName) from EndpointGroups ";
                DataSet dsEndpointGroup = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                foreach (DataRow drEndpointGroup in dsEndpointGroup.Tables[0].Rows)
                {
                    if (!dicEndpointGroup.ContainsKey(drEndpointGroup["LOWER"].ToString()))
                        dicEndpointGroup.Add(drEndpointGroup["LOWER"].ToString(), Convert.ToInt32(drEndpointGroup["EndpointGroupID"]));
                }

                Dictionary<string, int> dicPollutant = new Dictionary<string, int>();
                commandText = string.Format("select PollutantID,LOWER(PollutantName) from Pollutants where SetupID={0}", CommonClass.ManageSetup.SetupID);
                DataSet dsPollutant = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                foreach (DataRow drPollutant in dsPollutant.Tables[0].Rows)
                {
                    if (!dicPollutant.ContainsKey(drPollutant["LOWER"].ToString()))
                        dicPollutant.Add(drPollutant["LOWER"].ToString(), Convert.ToInt32(drPollutant["PollutantID"]));
                }

                Dictionary<string, int> dicPollutantGroup = new Dictionary<string, int>();
                commandText = string.Format("select PollutantGroupID,LOWER(PGName) from PollutantGroups where SetupID={0}", CommonClass.ManageSetup.SetupID);
                DataSet dsPollutantGroup = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                foreach (DataRow drPollutantGroup in dsPollutantGroup.Tables[0].Rows)
                {
                    if (!dicPollutantGroup.ContainsKey(drPollutantGroup["LOWER"].ToString()))
                        dicPollutantGroup.Add(drPollutantGroup["LOWER"].ToString(), Convert.ToInt32(drPollutantGroup["PollutantGroupID"]));
                }

                Dictionary<string, string> dicIncidence = new Dictionary<string, string>();
                commandText = string.Format("select IncidenceDataSetID,LOWER(IncidenceDataSetName) from IncidenceDataSets where SetupID={0}", CommonClass.ManageSetup.SetupID);
                DataSet dsIncidence = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                foreach (DataRow drIncidence in dsIncidence.Tables[0].Rows)
                {
                    if (!dicIncidence.ContainsKey(drIncidence["LOWER"].ToString()))
                        dicIncidence.Add(drIncidence["LOWER"].ToString(), drIncidence["IncidenceDataSetID"].ToString());
                }

                Dictionary<string, int> dicPrevalence = new Dictionary<string, int>();
                commandText = string.Format("select IncidenceDataSetID,LOWER(IncidenceDataSetName) from IncidenceDataSets where SetupID={0}", CommonClass.ManageSetup.SetupID);
                DataSet dsPrevalence = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                foreach (DataRow drPrevalence in dsPrevalence.Tables[0].Rows)
                {
                    if (!dicPrevalence.ContainsKey(drPrevalence["LOWER"].ToString()))
                        dicPrevalence.Add(drPrevalence["LOWER"].ToString(), Convert.ToInt32(drPrevalence["IncidenceDataSetID"].ToString()));
                }

                Dictionary<string, string> dicVariable = new Dictionary<string, string>();
                commandText = string.Format("select SetupVariableDataSetID,LOWER(SetupVariableDataSetName) from SetupVariableDataSets where SetupID={0}", CommonClass.ManageSetup.SetupID);
                DataSet dsVariable = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                foreach (DataRow drVarible in dsVariable.Tables[0].Rows)
                {
                    if (!dicVariable.ContainsKey(drVarible["LOWER"].ToString()))
                        dicVariable.Add(drVarible["LOWER"].ToString(), drVarible["SetupVariableDataSetID"].ToString());
                }

                Dictionary<string, int> dicBaselineFuntion = new Dictionary<string, int>();
                commandText = "select FunctionalFormID,LOWER(FunctionalFormText) from BaselineFunctionalForms";
                DataSet dsBaselineFunction = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                foreach (DataRow drBaselineFunction in dsBaselineFunction.Tables[0].Rows)
                {
                    if (!dicBaselineFuntion.ContainsKey(drBaselineFunction["LOWER"].ToString()))
                        dicBaselineFuntion.Add(drBaselineFunction["LOWER"].ToString(), Convert.ToInt32(drBaselineFunction["FunctionalFormID"]));
                }

                Dictionary<string, int> dicFunction = new Dictionary<string, int>();
                commandText = "select FunctionalFormID,FunctionalFormText from FunctionalForms";
                DataSet dsFunction = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                foreach (DataRow drFunction in dsFunction.Tables[0].Rows)
                {
                    if (!dicFunction.ContainsKey(drFunction["FunctionalFormText"].ToString()))
                        dicFunction.Add(drFunction["FunctionalFormText"].ToString(), Convert.ToInt32(drFunction["FunctionalFormID"].ToString()));
                }

                Dictionary<string, string> dicLocationTypeID = new Dictionary<string, string>();
                commandText = string.Format("select LocationTypeID,LOWER(LocationTypeName) from LocationType where SetupID={0}", CommonClass.ManageSetup.SetupID);
                DataSet dsLocationType = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                foreach (DataRow drLocationType in dsLocationType.Tables[0].Rows)
                {
                    dicLocationTypeID.Add(drLocationType["LOWER"].ToString(), drLocationType["LocationTypeID"].ToString());
                }

                Dictionary<string, string> dicMSID = new Dictionary<string, string>();
                commandText = "select MSID, LOWER(MSDESCRIPTION) from ModelSpecifications";
                DataSet dsMS = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                foreach (DataRow drMS in dsMS.Tables[0].Rows)
                {
                    if (!dicMSID.ContainsKey(drMS["LOWER"].ToString()))
                        dicMSID.Add(drMS["LOWER"].ToString().Trim(), drMS["MSID"].ToString());
                }

                Dictionary<string, string> dicBetaVarID = new Dictionary<string, string>();
                commandText = "select BETAVARIATIONID, LOWER(BETAVARIATIONNAME) from BetaVariations";
                DataSet dsBV = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                foreach (DataRow drBV in dsBV.Tables[0].Rows)
                {
                    if (!dicBetaVarID.ContainsKey(drBV["LOWER"].ToString()))
                        dicBetaVarID.Add(drBV["LOWER"].ToString(), drBV["BETAVARIATIONID"].ToString());
                }

                for (int m = 0; m < _dt.Rows.Count; m++)
                {
                    DataRow dr = _dt.Rows[m];
                    commandText = "select endpointGroupID from EndpointGroups where LOWER(EndpointGroupName)='" + dr[0].ToString().ToLower() + "' ";
                    obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                    if (obj == null)
                    {
                        commandText = "select max(ENDPOINTGROUPID) from ENDPOINTGROUPS";
                        object endPointGroupID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                        commandText = string.Format("insert into EndpointGroups values ({0},'{1}')", endPointGroupID, dr[0].ToString());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicEndpointGroup.Add(dr[0].ToString().ToLower(), Convert.ToInt32(endPointGroupID));
                    }

                }

                // new dataset
                #region if the _datasetID compairs to a -1
                if (_datasetID == -1)
                {
                    commandText = string.Format("select * from  CRFunctionDataSets where SetupID={0}", CommonClass.ManageSetup.SetupID);
                    ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    int dataSetNameCount = ds.Tables[0].Rows.Count;
                    int rth;

                    commandText = string.Format("select CRFUNCTIONDATASETID from CRFUNCTIONDATASETS where CRFUNCTIONDATASETID = {0} AND setupid = {1}", crFunctionDataSetID, CommonClass.ManageSetup.SetupID);
                    obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                    if (obj == null)
                    {
                        //this is inserting it to the CRFunctionDataSets table - check and see if it already exist, if it does then I am adding in an additional file
                        //The F is for the locked column in CRFunctionDataSet - this is being imported and not predefined.
                        commandText = string.Format("insert into CRFunctionDataSets values ({0},{1},'{2}','F', 'F')", crFunctionDataSetID, CommonClass.ManageSetup.SetupID, txtHealthImpactFunction.Text.Replace("'", "''"));
                        rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    }

                    int dgvRowCount = _dt.Rows.Count;
                    
                    for (int row = 0; row < dgvRowCount; row++)
                    {
                        CommonClass.Connection.Close();
                        commandText = string.Format("select max(CRFUNCTIONID) from CRFunctions");
                        obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                        int CRFunctionID = int.Parse(obj.ToString()) + 1;

                        int EndpointGroupID = dicEndpointGroup[_dt.Rows[row][0].ToString().ToLower()];

                        commandText = string.Format("select EndpointID from Endpoints where Replace(LOWER(EndpointName),' ','')='{0}' and EndpointGroupID={1}", _dt.Rows[row][1].ToString().ToLower().Replace(" ", ""), dicEndpointGroup[_dt.Rows[row][0].ToString().ToLower()]);
                        obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                        if (obj == null)
                        {
                            commandText = "select max(EndPointID) from EndPoints";
                            obj = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                            commandText = string.Format("insert into Endpoints values ({0},{1},'{2}')", obj, dicEndpointGroup[_dt.Rows[row][0].ToString().ToLower()], _dt.Rows[row][1].ToString());
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        int EndpointID = int.Parse(obj.ToString());

                        int PollutantID, PollutantGroupID;
                        // Used so that inserts will work -- one should be removed from database later and code updated -- ToEdit when database is updated
                        if (dicPollutant.ContainsKey(_dt.Rows[row][2].ToString().ToLower()))
                        {
                            PollutantID = dicPollutant[_dt.Rows[row][2].ToString().ToLower()];
                            commandText = string.Format("select pollutantgroupid from pollutants left join pollutantgrouppollutants on pollutants.pollutantid=pollutantgrouppollutants.pollutantid where setupid={0} and pollutants.pollutantid={1}", CommonClass.MainSetup.SetupID, PollutantID);
                            PollutantGroupID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                        }
                        else if (dicPollutantGroup.ContainsKey(_dt.Rows[row][2].ToString().ToLower()))
                        {
                            PollutantGroupID = dicPollutantGroup[_dt.Rows[row][2].ToString().ToLower()];
                            commandText = string.Format("select first 1 pollutantid from pollutantgrouppollutants where pollutantgroupid={0}", PollutantGroupID);
                            PollutantID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                        }
                        else
                        {
                            if (!undefinePollutant.Contains(_dt.Rows[row][2].ToString()))
                            {
                                undefinePollutant += "'" + _dt.Rows[row][2].ToString() + "', ";
                            }
                            continue;
                        }

                        int FunctionID = 0;

                        if (dicFunction.ContainsKey(_dt.Rows[row][18].ToString()))
                        {
                            FunctionID = dicFunction[_dt.Rows[row][18].ToString()];
                        }
                        else
                        {
                            commandText = string.Format("select max(FUNCTIONALFORMID) from FUNCTIONALFORMS");
                            obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                            FunctionID = int.Parse(obj.ToString()) + 1;
                            commandText = string.Format("insert into FUNCTIONALFORMS values ({0},'{1}')", FunctionID, _dt.Rows[row][18].ToString());
                            rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        }
                        int BaselineFunctionID = 0;

                        if (dicBaselineFuntion.ContainsKey(_dt.Rows[row][19].ToString().ToLower()))
                        {
                            BaselineFunctionID = dicBaselineFuntion[_dt.Rows[row][19].ToString().ToLower()];
                        }
                        else
                        {
                            commandText = string.Format("select max(FUNCTIONALFORMID) from BASELINEFUNCTIONALFORMS");
                            obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                            BaselineFunctionID = int.Parse(obj.ToString()) + 1;
                            commandText = string.Format("insert into BASELINEFUNCTIONALFORMS values ({0},'{1}')", BaselineFunctionID, _dt.Rows[row][19].ToString());
                            rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        }

                        // Metrics matched against pollutantid and pollutantgroupid to allow save -- ToEdit when database is updated
                        commandText = string.Format("select first 1 metricid from metrics join pollutantgrouppollutants p on pollutantgroupid={0} or p.pollutantid={1} and metrics.pollutantid=p.pollutantid and lower(metricname)='{2}'", PollutantGroupID, PollutantID, _dt.Rows[row][3].ToString().ToLower());
                        int MetricID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));

                        // Get SeasonalMetricID using SeasonalMetricName and ID from first pollutant 
                        string SeasonalMetricID = "";
                        List<CRFVariable> varList = new List<CRFVariable>();
                        if (_dt.Rows[row][3].ToString() != string.Empty)
                        {
                            commandText = string.Format("select sm.seasonalmetricid from seasonalmetrics sm left join metrics m on m.metricid = sm.metricid left join pollutants p on p.pollutantid = m.pollutantid where sm.seasonalmetricname = '{0}' and p.pollutantid = {1}", _dt.Rows[row][3].ToString(), PollutantID);
                            fb = new ESIL.DBUtility.ESILFireBirdHelper();
                            object res = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                            if (res != null)
                            {
                                SeasonalMetricID = Convert.ToInt32(res).ToString();
                            }
                        }
                        else
                        {
                            SeasonalMetricID = "NULL";
                        }

                        string PrevalenceID = string.Empty;

                        if (dicPrevalence.Keys.Contains(_dt.Rows[row][21].ToString().ToLower()))
                            PrevalenceID = dicPrevalence[_dt.Rows[row][21].ToString().ToLower()].ToString();
                        else
                            PrevalenceID = "NULL";

                        string IncidenceID = string.Empty;

                        if (dicIncidence.Keys.Contains(_dt.Rows[row][20].ToString().ToLower()))
                            IncidenceID = dicIncidence[_dt.Rows[row][20].ToString().ToLower()].ToString();
                        else IncidenceID = "NULL";

                        string VariableID = string.Empty;

                        if (dicVariable.Keys.Contains(_dt.Rows[row][22].ToString().ToLower()))
                            VariableID = dicVariable[_dt.Rows[row][22].ToString().ToLower()].ToString();
                        else VariableID = "NULL";

                        string LocationtypeID = string.Empty;

                        if (dicLocationTypeID.Keys.Contains(_dt.Rows[row][8].ToString().ToLower()))
                            LocationtypeID = dicLocationTypeID[_dt.Rows[row][8].ToString().ToLower()].ToString();
                        else LocationtypeID = "NULL";

                        string ModelSpecID = string.Empty;

                        if (dicMSID.Keys.Contains(_dt.Rows[row][23].ToString().ToLower().Trim()))
                            ModelSpecID = dicMSID[_dt.Rows[row][23].ToString().ToLower()].ToString();
                        else ModelSpecID = "NULL";

                        string BetaVarID = string.Empty;

                        if (dicBetaVarID.Keys.Contains(_dt.Rows[row][24].ToString().ToLower()))
                            BetaVarID = dicBetaVarID[_dt.Rows[row][24].ToString().ToLower()].ToString();
                        else BetaVarID = "NULL";

                        int MetricStatisticID = 0;

                        if (_dt.Rows[row][5].ToString().Trim() == "None")
                        {
                            MetricStatisticID = 0;
                        }

                        else if (_dt.Rows[row][5].ToString().Trim() == "Mean")
                        {
                            MetricStatisticID = 1;
                        }

                        else if (_dt.Rows[row][5].ToString().Trim() == "Median")
                        {
                            MetricStatisticID = 2;
                        }

                        else if (_dt.Rows[row][5].ToString().Trim() == "Max")
                        {
                            MetricStatisticID = 3;
                        }

                        else if (_dt.Rows[row][5].ToString().Trim() == "Min")
                        {
                            MetricStatisticID = 4;
                        }

                        else if (_dt.Rows[row][5].ToString().Trim() == "")
                        {
                            MetricStatisticID = 0;
                        }
                        else
                        {
                            MetricStatisticID = 5;
                        }

                        // Adding new function to new dataset

                        commandText = string.Format("insert into CRFunctions(CRFUNCTIONID, CRFUNCTIONDATASETID, ENDPOINTGROUPID, ENDPOINTID, "
                            + "SEASONALMETRICID, METRICSTATISTIC, AUTHOR, YYEAR, LOCATION, OTHERPOLLUTANTS, "
                            + "QUALIFIER, REFERENCE, RACE, GENDER, STARTAGE, ENDAGE, FUNCTIONALFORMID, INCIDENCEDATASETID, PREVALENCEDATASETID, "
                            + "VARIABLEDATASETID, BASELINEFUNCTIONALFORMID, ETHNICITY, PERCENTILE, LOCATIONTYPEID, POLLUTANTGROUPID, MSID, BETAVARIATIONID) "
                            + " values({0},{1},{2},{3},{4},{5},'{6}',{7},'{8}','{9}','{10}','{11}','{12}','{13}', " +
                            "{14},{15},{16},{17},{18},{19},{20},'{21}',{22},{23},{24},{25},{26})",
                            CRFunctionID, crFunctionDataSetID, EndpointGroupID, EndpointID, SeasonalMetricID, MetricStatisticID,
                            _dt.Rows[row][6].ToString().Replace("'", "''"), Convert.ToInt16(_dt.Rows[row][7].ToString()), _dt.Rows[row][9].ToString().Replace("'", "''"),
                            _dt.Rows[row][10].ToString().Replace("'", "''"), _dt.Rows[row][11].ToString().Replace("'", "''"), _dt.Rows[row][12].ToString().Replace("'", "''"),
                            _dt.Rows[row][13].ToString().Replace("'", "''"), _dt.Rows[row][15].ToString().Replace("'", "''"), _dt.Rows[row][16], _dt.Rows[row][17],
                            FunctionID, IncidenceID, PrevalenceID, VariableID, BaselineFunctionID, _dt.Rows[row][14].ToString().Replace("'", "''"), 0,
                            LocationtypeID, PollutantGroupID, ModelSpecID, BetaVarID);

                        rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);


                        varList = new List<CRFVariable>();
                        if (variableLists.TryGetValue(Convert.ToInt16(_dt.Rows[row][25]), out varList))
                        {

                            // Insert into CRFVariables table
                            foreach (CRFVariable v in varList)
                            {
                                // Get new CRFVariableID
                                commandText = string.Format("select max(CRFVARIABLEID) from CRFVariables");
                                obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                                int CRFVariableID = int.Parse(obj.ToString()) + 1;

                                if (v.Metric.MetricName != null)
                                {
                                    commandText = string.Format("insert into CRFVariables(CRFUNCTIONID,CRFVARIABLEID,VARIABLENAME,POLLUTANTNAME,POLLUTANT1ID,POLLUTANT2ID,METRICID) values ({0},{1},'{2}','{3}',{4},{5},{6})", CRFunctionID, CRFVariableID, v.VariableName, v.PollutantName, v.Pollutant1ID, v.Pollutant2ID, v.Metric.MetricID);
                                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                }
                                else
                                {
                                    commandText = string.Format("insert into CRFVariables(CRFUNCTIONID,CRFVARIABLEID,VARIABLENAME,POLLUTANTNAME,POLLUTANT1ID,POLLUTANT2ID) values ({0},{1},'{2}','{3}',{4},{5})", CRFunctionID, CRFVariableID, v.VariableName, v.PollutantName, v.Pollutant1ID, v.Pollutant2ID);
                                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                }

                                //Insert into CRFBetas table
                                foreach (CRFBeta b in v.PollBetas)
                                {
                                    // Get new CRFBetaID
                                    commandText = string.Format("select max(CRFBETAID) from CRFBetas");
                                    obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                                    int CRFBetaID = int.Parse(obj.ToString()) + 1;
                                    b.BetaID = CRFBetaID;

                                    commandText = string.Format("insert into CRFBetas(crfbetaid, crfvariableid, distributiontypeid, seasonalmetricseasonid, beta, p1beta, p2beta, a, namea, b, nameb, c, namec) values ({0},{1},{2},{3},{4},{5},{6},{7},'{8}',{9},'{10}',{11},'{12}')", CRFBetaID, CRFVariableID, b.DistributionTypeID, b.SeasonalMetricSeasonID, b.Beta, b.P1Beta, b.P2Beta, b.AConstantValue, b.AConstantName, b.BConstantValue, b.BConstantName, b.CConstantValue, b.CConstantName);
                                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                                    // Check for custom distribution
                                    if (b.Distribution == "Custom" && b.CustomList.Count() > 0)
                                    {
                                        FirebirdSql.Data.FirebirdClient.FbCommand fbCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
                                        fbCommand.Connection = CommonClass.Connection;
                                        fbCommand.CommandType = CommandType.Text;
                                        fbCommand.Connection.Open();
                                        commandText = "delete from crfunctioncustomentries where crfunctionid =" + CRFunctionID.ToString();
                                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                        DataTable dtCustomValue = new DataTable();
                                        DataColumn dc = new DataColumn();
                                        dc.ColumnName = "Value";
                                        dtCustomValue.Columns.Add(dc);

                                        foreach (double value in b.CustomList)
                                        {
                                            DataRow dr = dtCustomValue.NewRow();
                                            dr["Value"] = value;
                                            dtCustomValue.Rows.Add(dr);
                                        }
                                        int rowCount = dtCustomValue.Rows.Count;
                                        for (int k = 0; k < (rowCount / 125) + 1; k++)
                                        {
                                            commandText = "execute block as declare CRFUNCTIONID int;" + " BEGIN ";
                                            for (int t = 0; t < 125; t++)
                                            {
                                                if (k * 125 + t < rowCount)
                                                {
                                                    commandText = commandText + string.Format(" insert into CRFUNCTIONCUSTOMENTRIES values ({0},{1});", CRFunctionID, dtCustomValue.Rows[k * 125 + t][0]);
                                                }
                                                else
                                                    continue;
                                            }
                                            commandText = commandText + "END";
                                            fbCommand.CommandText = commandText;
                                            fbCommand.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }

                            // Insert into CRFVarCov table
                            // Have to do this after all betas have been set up so that they have ID's
                            int i = 0;
                            int CRFVarCovID = 0;
                            foreach (CRFVariable v in varList)
                            {
                                i = 0;
                                foreach (CRFBeta b in v.PollBetas)
                                {
                                    foreach (CRFVarCov cov in b.VarCovar)
                                    {
                                        // Get new CRFVarCovID
                                        commandText = string.Format("select max(CRFVARCOVID) from CRFVarCov");
                                        obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                                        CRFVarCovID = int.Parse(obj.ToString()) + 1;

                                        // Get beta ID's 
                                        if (cov.BetaID1 == 0 && cov.BetaID2 == 0)
                                        {
                                            cov.BetaID1 = b.BetaID;
                                            foreach (CRFVariable cv in varList)
                                            {
                                                if (cv.PollutantName == cov.InteractionPollutant)
                                                {
                                                    cov.BetaID2 = cv.PollBetas[i].BetaID;
                                                }
                                            }
                                        }

                                        commandText = string.Format("insert into CRFVARCOV (crfvarcovid, crfbetaid1, crfbetaid2, varcov) values ({0},{1},{2},{3})", CRFVarCovID, cov.BetaID1, cov.BetaID2, cov.VarCov);
                                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                    }
                                    i++;
                                }
                            }
                        }
                    }
                    if (undefinePollutant.Length > 2)
                    {
                        undefinePollutant = undefinePollutant.Substring(0, undefinePollutant.Length - 2);
                        MessageBox.Show("Please define " + undefinePollutant + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                } 

                // Editing existing function

                else
                {
                    commandText = string.Format("update CRFunctionDataSets set CRFunctionDataSetName='{0}' where CRFunctionDataSetID={1}", txtHealthImpactFunction.Text.Replace("'", "''"), _datasetID);
                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                    string deleteCRFunctions = "";
                    foreach (int i in lstdeleteCRFunctionid)
                    {
                        deleteCRFunctions += i.ToString() + ",";
                    }
                    if (deleteCRFunctions.Length > 1)
                    {
                        deleteCRFunctions = "(" + deleteCRFunctions.Substring(0, deleteCRFunctions.Length - 1) + ")";

                        // Delete values from VarCov table
                        commandText = string.Format("delete from crfvarcov where crfbetaid1 in "
                            + "(select crfbetaid from crfbetas where crfvariableid in "
                            + "(select crfvariableid from crfvariables where crfunctionid in {0})) "
                            + "or crfbetaid2 in (select crfbetaid from crfbetas where crfvariableid in ("
                            + "select crfvariableid from crfvariables where crfunctionid in {0}))", deleteCRFunctions);
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                        // Delete values from CRFBetas table
                        commandText = string.Format("delete from crfbetas where crfvariableid in "
                            + "(select crfvariableid from crfvariables where crfunctionid in {0})", deleteCRFunctions);
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                        // Delete values from CRFVariables table
                        commandText = string.Format("delete from crfvariables where crfunctionid in {0}", deleteCRFunctions);
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                        // Delete function from CRFunctions table 
                        commandText = string.Format("delete from CRFunctions where crfunctionid in {0}", deleteCRFunctions);
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    }

                    int dgvRowCount = _dt.Rows.Count;
                    for (int row = 0; row < dgvRowCount; row++)
                    {
                        CommonClass.Connection.Close();
                        commandText = string.Format("select max(CRFUNCTIONID) from CRFunctions");
                        obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                        int CRFunctionID = int.Parse(obj.ToString()) + 1;
                        int EndpointGroupID = dicEndpointGroup[_dt.Rows[row][0].ToString().ToLower()];

                        commandText = string.Format("select EndpointID from Endpoints where Replace(LOWER(EndpointName),' ','')='{0}' and EndpointGroupID={1}", _dt.Rows[row][1].ToString().ToLower().Replace(" ", ""), dicEndpointGroup[_dt.Rows[row][0].ToString().ToLower()]);
                        obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                        if (obj == null)
                        {
                            commandText = "select max(ENDPOINTID) from ENDPOINTS";
                            obj = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                            commandText = string.Format("insert into Endpoints values ({0},{1},'{2}')", obj, dicEndpointGroup[_dt.Rows[row][0].ToString().ToLower()], _dt.Rows[row][1].ToString());
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        int EndpointID = int.Parse(obj.ToString());

                        int PollutantID, PollutantGroupID;
                        // Used so that inserts will work -- one should be removed from database later and code updated -- ToEdit when database is updated
                        if (dicPollutant.ContainsKey(_dt.Rows[row][2].ToString().ToLower()))
                        {
                            PollutantID = dicPollutant[_dt.Rows[row][2].ToString().ToLower()];
                            commandText = string.Format("select pollutantgroupid from pollutants left join pollutantgrouppollutants on pollutants.pollutantid=pollutantgrouppollutants.pollutantid where setupid={0} and pollutants.pollutantid={1}", CommonClass.MainSetup.SetupID, PollutantID);
                            PollutantGroupID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                        }
                        else if (dicPollutantGroup.ContainsKey(_dt.Rows[row][2].ToString().ToLower()))
                        {
                            PollutantGroupID = dicPollutantGroup[_dt.Rows[row][2].ToString().ToLower()];
                            commandText = string.Format("select first 1 pollutantid from pollutantgrouppollutants where pollutantgroupid={0}", PollutantGroupID);
                            PollutantID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                        }
                        else
                        { 
                            if (!undefinePollutant.Contains(_dt.Rows[row][2].ToString()))
                            {
                                undefinePollutant += "'" + _dt.Rows[row][2].ToString() + "', ";
                            }
                            continue;
                        }

                        int FunctionID = 0;

                        if (dicFunction.ContainsKey(_dt.Rows[row][18].ToString()))
                        { FunctionID = dicFunction[_dt.Rows[row][18].ToString()]; }
                        else
                        {
                            commandText = string.Format("select max(FUNCTIONALFORMID) from FUNCTIONALFORMS");
                            obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                            FunctionID = int.Parse(obj.ToString()) + 1;
                            commandText = string.Format("insert into FUNCTIONALFORMS values ({0},'{1}')", FunctionID, _dt.Rows[row][18].ToString());
                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        }
                        int BaselineFunctionID = 0;

                        if (dicBaselineFuntion.ContainsKey(_dt.Rows[row][19].ToString().ToLower()))
                            {
                                BaselineFunctionID = dicBaselineFuntion[_dt.Rows[row][19].ToString().ToLower()];
                            }
                        else
                        {
                            commandText = string.Format("select max(FUNCTIONALFORMID) from BASELINEFUNCTIONALFORMS");
                            obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                            BaselineFunctionID = int.Parse(obj.ToString()) + 1;
                            commandText = string.Format("insert into BASELINEFUNCTIONALFORMS values ({0},'{1}')", BaselineFunctionID, _dt.Rows[row][19].ToString());
                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        }

                        // Metrics matched against pollutantid and pollutantgroupid to allow save -- ToEdit when database is updated
                        commandText = string.Format("select first 1 metricid from metrics join pollutantgrouppollutants p on pollutantgroupid={0} or p.pollutantid={1} and metrics.pollutantid=p.pollutantid and lower(metricname)='{2}'", PollutantGroupID, PollutantID, _dt.Rows[row][3].ToString().ToLower());
                        int MetricID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));

                        // Get SeasonalMetricID using SeasonalMetricName and ID from first pollutant 
                        string SeasonalMetricID = "";
                        List<CRFVariable> varList = new List<CRFVariable>();
                        if (_dt.Rows[row][3].ToString() != string.Empty)
                        {
                            commandText = string.Format("select sm.seasonalmetricid from seasonalmetrics sm left join metrics m on m.metricid = sm.metricid left join pollutants p on p.pollutantid = m.pollutantid where sm.seasonalmetricname = '{0}' and p.pollutantid = {1}", _dt.Rows[row][3].ToString(), PollutantID);
                            fb = new ESIL.DBUtility.ESILFireBirdHelper();
                            object res = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                            if (res != null)
                            {
                                SeasonalMetricID = Convert.ToInt32(res).ToString();
                            }
                        }
                        else
                        {
                            SeasonalMetricID = "NULL";
                        }

                        string PrevalenceID = string.Empty;

                        if (dicPrevalence.Keys.Contains(_dt.Rows[row][21].ToString().ToLower()))
                            PrevalenceID = dicPrevalence[_dt.Rows[row][21].ToString().ToLower()].ToString();
                        else
                            PrevalenceID = "NULL";

                        string IncidenceID = string.Empty;

                        if (dicIncidence.Keys.Contains(_dt.Rows[row][20].ToString().ToLower()))
                            IncidenceID = dicIncidence[_dt.Rows[row][20].ToString().ToLower()].ToString();
                        else IncidenceID = "NULL";

                        string VariableID = string.Empty;

                        if (dicVariable.Keys.Contains(_dt.Rows[row][22].ToString().ToLower()))
                            VariableID = dicVariable[_dt.Rows[row][22].ToString().ToLower()].ToString();
                        else VariableID = "NULL";

                        string LocationtypeID = string.Empty;

                        if (dicLocationTypeID.Keys.Contains(_dt.Rows[row][8].ToString().ToLower()))
                            LocationtypeID = dicLocationTypeID[_dt.Rows[row][8].ToString().ToLower()].ToString();
                        else LocationtypeID = "NULL";

                        string ModelSpecID = string.Empty;

                        if (dicMSID.Keys.Contains(_dt.Rows[row][23].ToString().ToLower()))
                            ModelSpecID = dicMSID[_dt.Rows[row][23].ToString().ToLower()].ToString();
                        else ModelSpecID = "NULL";

                        string BetaVarID = string.Empty;

                        if (dicBetaVarID.Keys.Contains(_dt.Rows[row][24].ToString().ToLower()))
                            BetaVarID = dicBetaVarID[_dt.Rows[row][24].ToString().ToLower()].ToString();
                        else BetaVarID = "NULL";

                        int MetricStatisticID = 0;

                        if (_dt.Rows[row][5].ToString().Trim() == "None")
                            MetricStatisticID = 0;
                        else if (_dt.Rows[row][5].ToString().Trim() == "Mean")
                            MetricStatisticID = 1;
                        else if (_dt.Rows[row][5].ToString().Trim() == "Median")
                            MetricStatisticID = 2;
                        else if (_dt.Rows[row][5].ToString().Trim() == "Max")
                            MetricStatisticID = 3;
                        else if (_dt.Rows[row][5].ToString().Trim() == "Min")
                            MetricStatisticID = 4;
                        else if (_dt.Rows[row][5].ToString().Trim() == "")
                            MetricStatisticID = 0;
                        else
                            MetricStatisticID = 5;

                        if (Convert.ToInt16(_dt.Rows[row][25]) > 0) // CRFunctionID
                        {
                            CRFunctionID = Convert.ToInt16(_dt.Rows[row][25]);

                            commandText = string.Format("update CRFunctions set CRFunctionDataSetID={0},EndpointGroupID={1},EndpointID={2},SeasonalMetricID={3},METRICSTATISTIC={4},AUTHOR='{5}',YYEAR={6},LOCATION='{7}',OTHERPOLLUTANTS='{8}',QUALIFIER='{9}',REFERENCE='{10}',RACE='{11}',GENDER='{12}',STARTAGE={13},ENDAGE={14},FUNCTIONALFORMID={15},INCIDENCEDATASETID={16},PREVALENCEDATASETID={17},VARIABLEDATASETID={18},BASELINEFUNCTIONALFORMID={19},ETHNICITY='{20}',PERCENTILE={21},LOCATIONTYPEID={22},MSID={23},BETAVARIATIONID={24},POLLUTANTGROUPID={25} where CRFunctionID={26}", _datasetID, EndpointGroupID, EndpointID, SeasonalMetricID, MetricStatisticID, _dt.Rows[row][6].ToString().Replace("'", "''"), Convert.ToInt16(_dt.Rows[row][7].ToString()), _dt.Rows[row][9].ToString().Replace("'", "''"), _dt.Rows[row][10].ToString().Replace("'", "''"), _dt.Rows[row][11].ToString().Replace("'", "''"), _dt.Rows[row][12].ToString().Replace("'", "''"), _dt.Rows[row][13].ToString().Replace("'", "''"), _dt.Rows[row][15].ToString().Replace("'", "''"), _dt.Rows[row][16], _dt.Rows[row][17], FunctionID, IncidenceID, PrevalenceID, VariableID, BaselineFunctionID, _dt.Rows[row][14].ToString().Replace("'", "''"), 0, LocationtypeID, ModelSpecID, BetaVarID, PollutantGroupID, CRFunctionID);
                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);


                            varList = new List<CRFVariable>();
                            if (variableLists.TryGetValue(CRFunctionID, out varList))
                            {
                                // Delete existing data and insert new
                                if(toInsert.Contains(CRFunctionID))
                                {
                                    // --- Deleting ---
                                    // Delete values from VarCov table
                                    commandText = string.Format("delete from crfvarcov where crfbetaid1 in "
                                        + "(select crfbetaid from crfbetas where crfvariableid in "
                                        + "(select crfvariableid from crfvariables where crfunctionid = {0})) "
                                        + "or crfbetaid2 in (select crfbetaid from crfbetas where crfvariableid in ("
                                        + "select crfvariableid from crfvariables where crfunctionid = {0}))", CRFunctionID);
                                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                                    // Delete values from CRFBetas table
                                    commandText = string.Format("delete from crfbetas where crfvariableid in "
                                        + "(select crfvariableid from crfvariables where crfunctionid = {0})", CRFunctionID);
                                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                                    // Delete values from CRFVariables table
                                    commandText = string.Format("delete from crfvariables where crfunctionid = {0}", CRFunctionID);
                                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                                    // --- Inserting ---
                                    // Insert into CRFVariables table
                                    foreach (CRFVariable v in varList)
                                    {
                                        // Get new CRFVariableID
                                        commandText = string.Format("select max(CRFVARIABLEID) from CRFVariables");
                                        obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                                        int CRFVariableID = int.Parse(obj.ToString()) + 1;

                                        if (v.Metric.MetricName != null)
                                        {
                                            commandText = string.Format("insert into CRFVariables(CRFUNCTIONID,CRFVARIABLEID,VARIABLENAME,POLLUTANTNAME,POLLUTANT1ID,POLLUTANT2ID,METRICID) values ({0},{1},'{2}','{3}',{4},{5},{6})", CRFunctionID, CRFVariableID, v.VariableName, v.PollutantName, v.Pollutant1ID, v.Pollutant2ID, v.Metric.MetricID);
                                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                        }
                                        else
                                        {
                                            commandText = string.Format("insert into CRFVariables(CRFUNCTIONID,CRFVARIABLEID,VARIABLENAME,POLLUTANTNAME,POLLUTANT1ID,POLLUTANT2ID) values ({0},{1},'{2}','{3}',{4},{5})", CRFunctionID, CRFVariableID, v.VariableName, v.PollutantName, v.Pollutant1ID, v.Pollutant2ID);
                                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                        }

                                        //Insert into CRFBetas table
                                        foreach (CRFBeta b in v.PollBetas)
                                        {
                                            // Get new CRFBetaID
                                            commandText = string.Format("select max(CRFBETAID) from CRFBetas");
                                            obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                                            int CRFBetaID = int.Parse(obj.ToString()) + 1;
                                            b.BetaID = CRFBetaID;

                                            commandText = string.Format("insert into CRFBetas(crfbetaid, crfvariableid, distributiontypeid, seasonalmetricseasonid, beta, p1beta, p2beta, a, namea, b, nameb, c, namec) values ({0},{1},{2},{3},{4},{5},{6},{7},'{8}',{9},'{10}',{11},'{12}')", CRFBetaID, CRFVariableID, b.DistributionTypeID, b.SeasonalMetricSeasonID, b.Beta, b.P1Beta, b.P2Beta, b.AConstantValue, b.AConstantName, b.BConstantValue, b.BConstantName, b.CConstantValue, b.CConstantName);
                                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                                            // Check for custom distribution
                                            if (b.Distribution == "Custom" && b.CustomList.Count() > 0)
                                            {
                                                FirebirdSql.Data.FirebirdClient.FbCommand fbCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
                                                fbCommand.Connection = CommonClass.Connection;
                                                fbCommand.CommandType = CommandType.Text;
                                                fbCommand.Connection.Open();
                                                commandText = "delete from crfunctioncustomentries where crfunctionid =" + CRFunctionID.ToString();
                                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                                DataTable dtCustomValue = new DataTable();
                                                DataColumn dc = new DataColumn();
                                                dc.ColumnName = "Value";
                                                dtCustomValue.Columns.Add(dc);

                                                foreach (double value in b.CustomList)
                                                {
                                                    DataRow dr = dtCustomValue.NewRow();
                                                    dr["Value"] = value;
                                                    dtCustomValue.Rows.Add(dr);
                                                }
                                                int rowCount = dtCustomValue.Rows.Count;
                                                for (int k = 0; k < (rowCount / 125) + 1; k++)
                                                {
                                                    commandText = "execute block as declare CRFUNCTIONID int;" + " BEGIN ";
                                                    for (int t = 0; t < 125; t++)
                                                    {
                                                        if (k * 125 + t < rowCount)
                                                        {
                                                            commandText = commandText + string.Format(" insert into CRFUNCTIONCUSTOMENTRIES values ({0},{1});", CRFunctionID, dtCustomValue.Rows[k * 125 + t][0]);
                                                        }
                                                        else
                                                            continue;
                                                    }
                                                    commandText = commandText + "END";
                                                    fbCommand.CommandText = commandText;
                                                    fbCommand.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                    }
                                    // Insert into CRFVarCov table
                                    // Have to do this after all betas have been set up so that they have ID's
                                    int i = 0;
                                    /*
                                    foreach (CRFVariable v in varList)
                                    {
                                        i = 0;
                                        foreach (CRFBeta b in v.PollBetas)
                                        {
                                            foreach (CRFVarCov cov in b.VarCovar)
                                            {
                                                foreach (CRFVariable cv in varList)
                                                {
                                                    // Get new CRFVarCovID
                                                    commandText = string.Format("select max(CRFVARCOVID) from CRFVarCov");
                                                    obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                                                    int CRFVarCovID = int.Parse(obj.ToString()) + 1;

                                                    if (cov.BetaID1 == 0 && cov.BetaID2 == 0)
                                                    {
                                                        cov.BetaID1 = b.BetaID;
                                                        cov.BetaID2 = cv.PollBetas[i].BetaID;
                                                    }

                                                    commandText = string.Format("insert into CRFVARCOV (crfvarcovid, crfbetaid1, crfbetaid2, varcov) values ({0},{1},{2},{3})", CRFVarCovID, cov.BetaID1, cov.BetaID2, cov.VarCov);
                                                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                                }
                                            }
                                            i++;
                                        }

                                    }
                                    */
                                    //JHA 3/6/2018 - Fixed the above loop since it was doubling the number of varcov entries added for new functions in existing datasets.  This function needs to be refactored to reduce code duplication.
                                    foreach (CRFVariable v in varList)
                                    {
                                        i = 0;
                                        foreach (CRFBeta b in v.PollBetas)
                                        {
                                            foreach (CRFVarCov cov in b.VarCovar)
                                            {
                                                // Get new CRFVarCovID
                                                commandText = string.Format("select max(CRFVARCOVID) from CRFVarCov");
                                                obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                                                int CRFVarCovID = int.Parse(obj.ToString()) + 1;

                                                // Get beta ID's 
                                                if (cov.BetaID1 == 0 && cov.BetaID2 == 0)
                                                {
                                                    cov.BetaID1 = b.BetaID;
                                                    foreach (CRFVariable cv in varList)
                                                    {
                                                        if (cv.PollutantName == cov.InteractionPollutant)
                                                        {
                                                            cov.BetaID2 = cv.PollBetas[i].BetaID;
                                                        }
                                                    }
                                                }

                                                commandText = string.Format("insert into CRFVARCOV (crfvarcovid, crfbetaid1, crfbetaid2, varcov) values ({0},{1},{2},{3})", CRFVarCovID, cov.BetaID1, cov.BetaID2, cov.VarCov);
                                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                            }
                                            i++;
                                        }
                                    }
                                }

                                // Can be updated instead of wiped out
                                else
                                {
                                    // Update CRFVariables table
                                    foreach (CRFVariable v in varList)
                                    {
                                        // MetricID should be null for first order interaction variables
                                        String metId = "null";
                                        if(v.Metric.MetricID > 0)
                                        {
                                            metId = v.Metric.MetricID.ToString();
                                        }
                                        commandText = string.Format("update CRFVariables set variablename='{0}',pollutantname='{1}',metricid={2} where crfvariableid={3} and crfunctionid={4}", v.VariableName, v.PollutantName, metId, v.VariableID, v.FunctionID);
                                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                                        // Update CRFBetas table
                                        foreach (CRFBeta b in v.PollBetas)
                                        {
                                            commandText = string.Format("update CRFBetas set beta={0},p1beta={1},p2beta={2},a={3},namea='{4}',b={5},nameb='{6}',c={7},namec='{8}',distributiontypeid={9},seasonalmetricseasonid={10} where crfbetaid={11} and crfvariableid={12}", b.Beta, b.P1Beta, b.P2Beta, b.AConstantValue, b.AConstantName, b.BConstantValue, b.BConstantName, b.CConstantValue, b.CConstantName, b.DistributionTypeID,b.SeasonalMetricSeasonID, b.BetaID, v.VariableID);
                                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                                            // Update CRFVarCov table
                                            foreach (CRFVarCov cov in b.VarCovar)
                                            {
                                                //commandText = string.Format("update CRFVarCov set crfbetaid1={0}, crfbetaid2={1}, varcov={2} where crfvarcovid={3} and crfbetaid1={4}", cov.BetaID1, cov.BetaID2, cov.VarCov, cov.VarCovID, b.BetaID);
                                                //JHA 3/6/2018 - Fixed key misalignment. Also, betaids should not be changing here.  Just varcov.
                                                commandText = string.Format("update CRFVarCov set varcov={0} where crfvarcovid={1}", cov.VarCov, cov.VarCovID);
                                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Adding new function to existing dataset

                        else if (Convert.ToInt16(_dt.Rows[row][25].ToString()) < 0)
                        {
                            commandText = string.Format("insert into CRFunctions(CRFUNCTIONID, CRFUNCTIONDATASETID, ENDPOINTGROUPID, ENDPOINTID, "
                            + "SEASONALMETRICID, METRICSTATISTIC, AUTHOR, YYEAR, LOCATION, OTHERPOLLUTANTS, "
                            + "QUALIFIER, REFERENCE, RACE, GENDER, STARTAGE, ENDAGE, FUNCTIONALFORMID, INCIDENCEDATASETID, PREVALENCEDATASETID, "
                            + "VARIABLEDATASETID, BASELINEFUNCTIONALFORMID, ETHNICITY, PERCENTILE, LOCATIONTYPEID, POLLUTANTGROUPID, MSID, BETAVARIATIONID) "
                            + " values({0},{1},{2},{3},{4},{5},'{6}',{7},'{8}','{9}','{10}','{11}','{12}','{13}', " +
                                                    "{14},{15},{16},{17},{18},{19},{20},'{21}',{22},{23},{24},{25},{26})",
                                                    CRFunctionID, _datasetID, EndpointGroupID, EndpointID, SeasonalMetricID, MetricStatisticID,
                                                    _dt.Rows[row][6].ToString().Replace("'", "''"), Convert.ToInt16(_dt.Rows[row][7].ToString()), _dt.Rows[row][9].ToString().Replace("'", "''"),
                                                    _dt.Rows[row][10].ToString().Replace("'", "''"), _dt.Rows[row][11].ToString().Replace("'", "''"), _dt.Rows[row][12].ToString().Replace("'", "''"),
                                                    _dt.Rows[row][13].ToString().Replace("'", "''"), _dt.Rows[row][15].ToString().Replace("'", "''"), _dt.Rows[row][16], _dt.Rows[row][17],
                                                    FunctionID, IncidenceID, PrevalenceID, VariableID, BaselineFunctionID, _dt.Rows[row][14].ToString().Replace("'", "''"), 0,
                                                    LocationtypeID, PollutantGroupID, ModelSpecID, BetaVarID);

                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                            varList = new List<CRFVariable>();
                            if (variableLists.TryGetValue(Convert.ToInt16(_dt.Rows[row][25]), out varList))
                            {

                                // Insert into CRFVariables table
                                foreach (CRFVariable v in varList)
                                {
                                    // Get new CRFVariableID
                                    commandText = string.Format("select max(CRFVARIABLEID) from CRFVariables");
                                    obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                                    int CRFVariableID = int.Parse(obj.ToString()) + 1;

                                    if(v.Metric != null && v.Metric.MetricName != null)
                                    {
                                        commandText = string.Format("insert into CRFVariables(CRFUNCTIONID,CRFVARIABLEID,VARIABLENAME,POLLUTANTNAME,POLLUTANT1ID,POLLUTANT2ID,METRICID) values ({0},{1},'{2}','{3}',{4},{5},{6})", CRFunctionID, CRFVariableID, v.VariableName, v.PollutantName, v.Pollutant1ID, v.Pollutant2ID, v.Metric.MetricID);
                                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                    }
                                    else
                                    {
                                        commandText = string.Format("insert into CRFVariables(CRFUNCTIONID,CRFVARIABLEID,VARIABLENAME,POLLUTANTNAME,POLLUTANT1ID,POLLUTANT2ID) values ({0},{1},'{2}','{3}',{4},{5})", CRFunctionID, CRFVariableID, v.VariableName, v.PollutantName, v.Pollutant1ID, v.Pollutant2ID);
                                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                    }

                                    //Insert into CRFBetas table
                                    foreach (CRFBeta b in v.PollBetas)
                                    {
                                        // Get new CRFBetaID
                                        commandText = string.Format("select max(CRFBETAID) from CRFBetas");
                                        obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                                        int CRFBetaID = int.Parse(obj.ToString()) + 1;
                                        b.BetaID = CRFBetaID;

                                        commandText = string.Format("insert into CRFBetas(crfbetaid, crfvariableid, distributiontypeid, seasonalmetricseasonid, beta, p1beta, p2beta, a, namea, b, nameb, c, namec) values ({0},{1},{2},{3},{4},{5},{6},{7},'{8}',{9},'{10}',{11},'{12}')", CRFBetaID, CRFVariableID, b.DistributionTypeID, b.SeasonalMetricSeasonID, b.Beta, b.P1Beta, b.P2Beta, b.AConstantValue, b.AConstantName, b.BConstantValue, b.BConstantName, b.CConstantValue, b.CConstantName);
                                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                                        // Check for custom distribution
                                        if(b.Distribution == "Custom" && b.CustomList.Count() > 0)
                                        {
                                            FirebirdSql.Data.FirebirdClient.FbCommand fbCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
                                            fbCommand.Connection = CommonClass.Connection;
                                            fbCommand.CommandType = CommandType.Text;
                                            fbCommand.Connection.Open();
                                            commandText = "delete from crfunctioncustomentries where crfunctionid =" + CRFunctionID.ToString();
                                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                            DataTable dtCustomValue = new DataTable();
                                            DataColumn dc = new DataColumn();
                                            dc.ColumnName = "Value";
                                            dtCustomValue.Columns.Add(dc);

                                            foreach (double value in b.CustomList)
                                            {
                                                DataRow dr = dtCustomValue.NewRow();
                                                dr["Value"] = value;
                                                dtCustomValue.Rows.Add(dr);
                                            }
                                            int rowCount = dtCustomValue.Rows.Count;
                                            for (int k = 0; k < (rowCount / 125) + 1; k++)
                                            {
                                                commandText = "execute block as declare CRFUNCTIONID int;" + " BEGIN ";
                                                for (int t = 0; t < 125; t++)
                                                {
                                                    if (k * 125 + t < rowCount)
                                                    {
                                                        commandText = commandText + string.Format(" insert into CRFUNCTIONCUSTOMENTRIES values ({0},{1});", CRFunctionID, dtCustomValue.Rows[k * 125 + t][0]);
                                                    }
                                                    else
                                                        continue;
                                                }
                                                commandText = commandText + "END";
                                                fbCommand.CommandText = commandText;
                                                fbCommand.ExecuteNonQuery();
                                            }
                                        }
                                    } 
                                }

                                // Insert into CRFVarCov table
                                // Have to do this after all betas have been set up so that they have ID's
                                int i = 0;
                                /*
                                foreach (CRFVariable v in varList)
                                {
                                    i = 0;
                                    foreach (CRFBeta b in v.PollBetas)
                                    {
                                        foreach (CRFVarCov cov in b.VarCovar)
                                        {
                                            foreach (CRFVariable cv in varList)
                                            {
                                                // Get new CRFVarCovID
                                                commandText = string.Format("select max(CRFVARCOVID) from CRFVarCov");
                                                obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                                                int CRFVarCovID = int.Parse(obj.ToString()) + 1;

                                                if(cov.BetaID1 == 0 && cov.BetaID2 == 0)
                                                {
                                                    cov.BetaID1 = b.BetaID;
                                                    cov.BetaID2 = cv.PollBetas[i].BetaID;
                                                }

                                                commandText = string.Format("insert into CRFVARCOV (crfvarcovid, crfbetaid1, crfbetaid2, varcov) values ({0},{1},{2},{3})", CRFVarCovID, cov.BetaID1, cov.BetaID2, cov.VarCov);
                                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                            }
                                        }
                                        i++;
                                    }
                                }
                                */

                                //JHA 3/6/2018 - Fixed the above loop since it was doubling the number of varcov entries added for new functions in existing datasets.  This function needs to be refactored to reduce code duplication.
                                foreach (CRFVariable v in varList)
                                {
                                    i = 0;
                                    foreach (CRFBeta b in v.PollBetas)
                                    {
                                        foreach (CRFVarCov cov in b.VarCovar)
                                        {
                                            // Get new CRFVarCovID
                                            commandText = string.Format("select max(CRFVARCOVID) from CRFVarCov");
                                            obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                                            int CRFVarCovID = int.Parse(obj.ToString()) + 1;

                                            // Get beta ID's 
                                            if (cov.BetaID1 == 0 && cov.BetaID2 == 0)
                                            {
                                                cov.BetaID1 = b.BetaID;
                                                foreach (CRFVariable cv in varList)
                                                {
                                                    if (cv.PollutantName == cov.InteractionPollutant)
                                                    {
                                                        cov.BetaID2 = cv.PollBetas[i].BetaID;
                                                    }
                                                }
                                            }

                                            commandText = string.Format("insert into CRFVARCOV (crfvarcovid, crfbetaid1, crfbetaid2, varcov) values ({0},{1},{2},{3})", CRFVarCovID, cov.BetaID1, cov.BetaID2, cov.VarCov);
                                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                        }
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                } 
                #endregion
                // 2015 09 29 - BENMAP-353 save metadataobject
                //SQLStatementsCommonClass.insertMetadata(_metadataObj);
                insertMetadata(crFunctionDataSetID); // insertMetadata throwing exception
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // 2015 09 08 - Add confirmation message BENMAP-332
                if (olvFunction.Items.Count == 0) { MessageBox.Show("There are no data to be deleted."); return; }
                if (olvFunction.SelectedObject == null) { MessageBox.Show("You must select a row to delete."); return; }
                
                DialogResult rtn = MessageBox.Show("Delete this function?", "Confirm Deletion", MessageBoxButtons.YesNo);
                if (rtn == DialogResult.Yes)
                {
                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        foreach (Object olv in olvFunction.SelectedObjects)
                        {
                            if (olvcCRFunction.GetValue(olv).ToString() == _dt.Rows[i][25].ToString())
                            {
                                if (dicCustomValue.ContainsKey(Convert.ToInt32(olvcCRFunction.GetValue(olv).ToString())))
                                    dicCustomValue.Remove(Convert.ToInt32(olvcCRFunction.GetValue(olv).ToString()));
                                lstdeleteCRFunctionid.Add(Convert.ToInt32(olvcCRFunction.GetValue(olv).ToString()));
                                _dt.Rows.Remove(_dt.Rows[i]);
                            }
                        }
                    }
                    olvFunction.DataSource = _dt;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (olvFunction.SelectedObject == null) return;
                HealthImpact healthImpact = new HealthImpact();
                healthImpact.EndpointGroup = olvcEndpointGroup.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.Endpoint = olvcEndpoint.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.Pollutant = olvcPollutant.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.Metric = "";
                healthImpact.SeasonalMetric = olvcSeasonalMetric.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.MetricStatistic = olvcMetricStat.GetValue(olvFunction.SelectedObject).ToString().TrimEnd();
                healthImpact.Author = olvcAuthor.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.Year = olvcYear.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.LocationName = olvcLocationType.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.Location = olvcLocation.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.OtherPollutant = olvcOtherPolls.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.Qualifier = olvcQualifer.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.Reference = olvcReference.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.Race = olvcRace.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.Ethnicity = olvcEthnicity.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.Gender = olvcGender.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.StartAge = olvcStartAge.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.EndAge = olvcEndAge.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.Function = olvcFunction.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.BaselineIncidenceFunction = olvcBaseline.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.Incidence = olvcIncidence.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.Prevalence = olvcPrevalence.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.Variable = olvcVariableDataset.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.ModelSpec = olvcModelSpec.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.BetaVariation = olvcBetaVariation.GetValue(olvFunction.SelectedObject).ToString();
                healthImpact.FunctionID = olvcCRFunction.GetValue(olvFunction.SelectedObject).ToString();

                healthImpact.PollVariables = new List<CRFVariable>();

                foreach(var poll in healthImpact.PollVariables)
                {
                    if(poll.PollBetas == null)
                    {
                        poll.PollBetas = new List<CRFBeta>();
                        poll.PollBetas.Add(new CRFBeta());
                    }
                }

                if (_dt.Rows.Count == 0) { return; }

                HIFDefinitionMulti frm = new HIFDefinitionMulti(txtHealthImpactFunction.Text, healthImpact);//, listCustomValue);
                DialogResult rth = frm.ShowDialog();
                if (rth != DialogResult.OK) { return; }
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    if (_dt.Rows[i][25].ToString() == olvcCRFunction.GetValue(olvFunction.SelectedObject).ToString())
                    {
                        _dt.Rows[i][0] = frm.HealthImpacts.EndpointGroup;
                        _dt.Rows[i][1] = frm.HealthImpacts.Endpoint;
                        _dt.Rows[i][2] = frm.HealthImpacts.Pollutant;
                        _dt.Rows[i][3] = frm.HealthImpacts.SeasonalMetric;
                        _dt.Rows[i][5] = frm.HealthImpacts.MetricStatistic;
                        _dt.Rows[i][6] = frm.HealthImpacts.Author;
                        _dt.Rows[i][7] = frm.HealthImpacts.Year;
                        _dt.Rows[i][8] = frm.HealthImpacts.LocationName;
                        _dt.Rows[i][9] = frm.HealthImpacts.Location;
                        _dt.Rows[i][10] = frm.HealthImpacts.OtherPollutant;
                        _dt.Rows[i][11] = frm.HealthImpacts.Qualifier;
                        _dt.Rows[i][12] = frm.HealthImpacts.Reference;
                        _dt.Rows[i][13] = frm.HealthImpacts.Race;
                        _dt.Rows[i][14] = frm.HealthImpacts.Ethnicity;
                        _dt.Rows[i][15] = frm.HealthImpacts.Gender;
                        _dt.Rows[i][16] = frm.HealthImpacts.StartAge;
                        _dt.Rows[i][17] = frm.HealthImpacts.EndAge;
                        _dt.Rows[i][18] = frm.HealthImpacts.Function;
                        _dt.Rows[i][19] = frm.HealthImpacts.BaselineIncidenceFunction;
                        _dt.Rows[i][20] = frm.HealthImpacts.Incidence;
                        _dt.Rows[i][21] = frm.HealthImpacts.Prevalence;
                        _dt.Rows[i][22] = frm.HealthImpacts.Variable;
                        _dt.Rows[i][23] = frm.HealthImpacts.ModelSpec;
                        _dt.Rows[i][24] = frm.HealthImpacts.BetaVariation;
                        _dt.Rows[i][25] = Convert.ToInt32(olvcCRFunction.GetValue(olvFunction.SelectedObject).ToString());

                        variableLists.Add(Convert.ToInt32(healthImpact.FunctionID), frm.HealthImpacts.PollVariables);

                        // Check if Pollutant, Model Spec, Seasonal Metric, or Beta Variation have changed
                        // If so, variables and betas will need to be deleted and new values inserted
                        if(healthImpact.Pollutant != frm.HealthImpacts.Pollutant || 
                            healthImpact.ModelSpec != frm.HealthImpacts.ModelSpec ||
                            healthImpact.SeasonalMetric != frm.HealthImpacts.SeasonalMetric ||
                            healthImpact.BetaVariation != frm.HealthImpacts.BetaVariation)
                        {

                            toInsert.Add(Convert.ToInt32(healthImpact.FunctionID));
                        }
                    }
                }
                olvFunction.DataSource = _dt;
                olvFunction.SelectedItem.ForeColor = Color.Red;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cleanup_WhenCanceled();
            this.DialogResult = DialogResult.Cancel;
        }


        private void HealthImpactDataSetDefinition_Load(object sender, EventArgs e)
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = new DataSet();
                string commandText = string.Empty;
                if (_datasetID != -1)
                {
                    commandText = string.Format("select crfunctiondatasetname from crfunctiondatasets where crfunctiondatasetid={0}", _datasetID);
                    txtHealthImpactFunction.Text = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));

                    commandText = string.Format("select b.endpointgroupname,c.endpointname,d.pgname,f.seasonalmetricname, a.metadataid, " +
                            "case when Metricstatistic = 0 then 'None'  when Metricstatistic = 1 then 'Mean' when Metricstatistic = 2 " +
                            "then 'Median' when Metricstatistic = 3 then 'Max' when Metricstatistic = 4 then 'Min' when Metricstatistic = 5 " +
                            "then 'Sum'  END as MetricstatisticName,author,yyear,g.locationtypename,location,otherpollutants,qualifier,reference, " +
                            "race,ethnicity,gender,startage,endage,h.functionalformtext,i.functionalformtext,j.incidencedatasetname, " +
                            "k.incidencedatasetname,l.setupvariabledatasetname as variabeldatasetname, ms.msdescription, bv.betavariationname, " +
                            "a.CRFUNCTIONID from crfunctions a " +
                            "join modelspecifications ms on (a.msid = ms.msid) " +
                            "join betavariations bv on (a.betavariationid = bv.betavariationid) " +
                            "join endpointgroups b on (a.ENDPOINTGROUPID = b.ENDPOINTGROUPID) " +
                            "join endpoints c on(a.endpointid = c.endpointid) " +
                            "join pollutantgroups d on(a.POLLUTANTGROUPID = d.POLLUTANTGROUPID) " +
                            "left join seasonalmetrics f on(a.seasonalmetricid = f.seasonalmetricid) " +
                            "left join locationtype g on(a.locationtypeid = g.locationtypeid) join functionalforms h on(a.functionalformid = h.functionalformid) " +
                            "left join baselinefunctionalforms i on(a.baselinefunctionalformid = i.functionalformid) " +
                            "left join incidencedatasets j on(a.incidencedatasetid = j.incidencedatasetid) " +
                            "left join incidencedatasets k on(a.prevalencedatasetid = k.incidencedatasetid) " +
                            "left join setupvariabledatasets l on(a.variabledatasetid = l.setupvariabledatasetid) " +
                            "where CRFUNCTIONDATASETID={0}", _datasetID);

                    ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    olvFunction.DataSource = ds.Tables[0];
                    _dt = ds.Tables[0];
                    cboFilterEndpointGroup.Items.Add("All");
                    cboFilterPollutants.Items.Add("All");
                    int dtRow = _dt.Rows.Count;
                    string strTableName = string.Empty;
                    string strPolluantName = string.Empty;
                    for (int i = 0; i < dtRow; i++)
                    {
                        strTableName = _dt.Rows[i][0].ToString();
                        if (!cboFilterEndpointGroup.Items.Contains(strTableName))
                        {
                            cboFilterEndpointGroup.Items.Add(strTableName);

                        }

                        strPolluantName = _dt.Rows[i][2].ToString();
                        if (!cboFilterPollutants.Items.Contains(strPolluantName))
                        {
                            cboFilterPollutants.Items.Add(strPolluantName);

                        }
                    }
                }
                else
                {
                    commandText = string.Format("select b.endpointgroupname,c.endpointname,d.pgname,f.seasonalmetricname, a.metadataid, " +
                        "case when Metricstatistic = 0 then 'None'  when Metricstatistic = 1 then 'Mean' when Metricstatistic = 2 " +
                        "then 'Median' when Metricstatistic = 3 then 'Max' when Metricstatistic = 4 then 'Min' when Metricstatistic = 5 " +
                        "then 'Sum'  END as MetricstatisticName,author,yyear,g.locationtypename,location,otherpollutants,qualifier,reference, " +
                        "race,ethnicity,gender,startage,endage,h.functionalformtext,i.functionalformtext,j.incidencedatasetname, " +
                        "k.incidencedatasetname,l.setupvariabledatasetname as variabeldatasetname, ms.msdescription, bv.betavariationname, " +
                        "a.CRFUNCTIONID from crfunctions a " +
                        "join modelspecifications ms on (a.msid = ms.msid) " +
                        "join betavariations bv on (a.betavariationid = bv.betavariationid) " +
                        "join endpointgroups b on (a.ENDPOINTGROUPID = b.ENDPOINTGROUPID) " +
                        "join endpoints c on(a.endpointid = c.endpointid) " +
                        "join pollutantgroups d on(a.POLLUTANTGROUPID = d.POLLUTANTGROUPID) " +
                        "left join seasonalmetrics f on(a.seasonalmetricid = f.seasonalmetricid) " +
                        "left join locationtype g on(a.locationtypeid = g.locationtypeid) join functionalforms h on(a.functionalformid = h.functionalformid) " +
                        "left join baselinefunctionalforms i on(a.baselinefunctionalformid = i.functionalformid) " +
                        "left join incidencedatasets j on(a.incidencedatasetid = j.incidencedatasetid) " +
                        "left join incidencedatasets k on(a.prevalencedatasetid = k.incidencedatasetid) " +
                        "left join setupvariabledatasets l on(a.variabledatasetid = l.setupvariabledatasetid) " +
                        "where CRFUNCTIONDATASETID=null");

                    ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    olvFunction.DataSource = ds.Tables[0];
                    _dt = ds.Tables[0];

                    int number = 0;
                    int HealthImpactFunctionDatasetID = 0;
                    do
                    {
                        string comText = "select crfunctionDatasetID from crfunctionDataSets where crfunctionDatasetName=" + "'HealthImpactFunctionDataSet" + Convert.ToString(number) + "'";
                        HealthImpactFunctionDatasetID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, comText));
                        number++;
                    } while (HealthImpactFunctionDatasetID > 0);
                    txtHealthImpactFunction.Text = "HealthImpactFunctionDataSet" + Convert.ToString(number - 1);
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnOK_Click_1(object sender, EventArgs e)
        {
            LoadDatabase();
            this.DialogResult = DialogResult.OK;
        }

       

        private void insertMetadata(int crFunctionDataSetID)
        {
            _metadataObj.DatasetId = crFunctionDataSetID;
            // 2015 09 28 BENMAP-353 set Dataset Type ID, to hardcoded value - was not getting previously set. This caused metadata to be unretrievable
            //_metadataObj.DatasetTypeId = SQLStatementsCommonClass.getDatasetID("Healthfunctions");
            _metadataObj.DatasetTypeId = HEALTHIMPACTDATASETID;
            
            if (!SQLStatementsCommonClass.insertMetadata(_metadataObj))
            {
                MessageBox.Show("Failed to save Metadata.");
            }
        }

        private void cboFilterEndpointGroup_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                ObjectListView olv = olvFunction;
                if (olv == null || olv.IsDisposed)
                    return;
                OLVColumn column = olv.GetColumn("olvcEndpointGroup");

                ArrayList chosenValues = new ArrayList();
                string selectEndpoint = cboFilterEndpointGroup.GetItemText(cboFilterEndpointGroup.SelectedItem);
                if (selectEndpoint == "All")
                {
                    olvcEndpointGroup.ValuesChosenForFiltering.Clear();
                    olv.UpdateColumnFiltering();
                }
                else
                {
                    chosenValues.Add(selectEndpoint);
                    olvcEndpointGroup.ValuesChosenForFiltering = chosenValues;
                    olv.UpdateColumnFiltering();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void cboFilterPollutants_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                ObjectListView olv = olvFunction;
                if (olv == null || olv.IsDisposed)
                    return;
                OLVColumn column = olv.GetColumn("olvcPollutant");

                ArrayList chosenValues = new ArrayList();
                string selectEndpoint = cboFilterPollutants.GetItemText(cboFilterPollutants.SelectedItem);
                if (selectEndpoint == "All")
                {
                    olvcPollutant.ValuesChosenForFiltering.Clear();
                    olv.UpdateColumnFiltering();
                }
                else
                {
                    chosenValues.Add(selectEndpoint);
                    olvcPollutant.ValuesChosenForFiltering = chosenValues;
                    olv.UpdateColumnFiltering();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
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

        private void txtFilter_TextChanged_1(object sender, EventArgs e)
        {
            this.TimedFilter(this.olvFunction, txtFilter.Text);
        }

        private void chbGroup_CheckedChanged(object sender, EventArgs e)
        {
            ShowGroupsChecked(this.olvFunction, (CheckBox)sender);
        }

        private void ShowGroupsChecked(ObjectListView olv, CheckBox cb)
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

        private void btnOutPut_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "CSV File|*.CSV";
                saveFileDialog1.InitialDirectory = "C:\\";
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                { return; }
                string fileName = saveFileDialog1.FileName;
                DataTable dtOut = new DataTable();
                dtOut.Columns.Add("Endpoint Group", typeof(string));
                dtOut.Columns.Add("Endpoint", typeof(string));
                dtOut.Columns.Add("Pollutant", typeof(string));
                dtOut.Columns.Add("Seasonal Metric", typeof(string));
                dtOut.Columns.Add("Metric Statistic", typeof(string));
                dtOut.Columns.Add("Study Author", typeof(string));
                dtOut.Columns.Add("Study Year", typeof(int));
                dtOut.Columns.Add("Study Location Type", typeof(string));
                dtOut.Columns.Add("Study Location", typeof(string));
                dtOut.Columns.Add("Other Pollutants", typeof(string));
                dtOut.Columns.Add("Qualifier", typeof(string));
                dtOut.Columns.Add("Reference", typeof(string));
                dtOut.Columns.Add("Race", typeof(string));
                dtOut.Columns.Add("Ethnicity", typeof(string));
                dtOut.Columns.Add("Gender", typeof(string));
                dtOut.Columns.Add("Start Age", typeof(int));
                dtOut.Columns.Add("End Age", typeof(int));
                dtOut.Columns.Add("Function", typeof(string));
                dtOut.Columns.Add("Baseline Function", typeof(string));
                dtOut.Columns.Add("Incidence DataSet", typeof(string));
                dtOut.Columns.Add("Prevalence DataSet", typeof(string));
                dtOut.Columns.Add("Variable DataSet", typeof(string));
                FireBirdHelperBase fb = new ESILFireBirdHelper();
                string commandText = string.Empty;
                int outputRowsNumber = 50;
                Dictionary<int, string> dicEndPointGroup = OutputCommonClass.getAllEndPointGroup();
                Dictionary<int, string> dicEndPoint = OutputCommonClass.getAllEndPoint();
                Dictionary<int, string> dicFunction = OutputCommonClass.getAllFunctions();
                Dictionary<int, string> dicPollutant = OutputCommonClass.getAllPollutants();
                Dictionary<int, string> dicSeasonalMetric = OutputCommonClass.getSeasonalMetric();
                Dictionary<int, string> dicIncidence = OutputCommonClass.getAllIncidenceDataset();
                Dictionary<int, string> dicPrevalence = OutputCommonClass.getAllPrevalence();
                Dictionary<int, string> dicVariable = OutputCommonClass.getAllVariableDatasets();
                Dictionary<int, string> dicBaselineFunction = OutputCommonClass.getAllBaselineFunctions();
                Dictionary<int, string> dicLocation = OutputCommonClass.getAllLocation();
                commandText = "select count(*) from CRFunctions";
                int count = (int)fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                if (count < outputRowsNumber) { outputRowsNumber = count; }
                commandText = string.Format("select first {0} Endpointgroupid,Endpointid,Pollutantid,Seasonalmetricid,Metricstatistic, Author, Yyear, Location, Otherpollutants, Qualifier, Reference,Race, Gender, Startage, Endage, Functionalformid,Incidencedatasetid,Prevalencedatasetid,Variabledatasetid,Baselinefunctionalformid, Ethnicity,Locationtypeid from CRFunctions where crfunctiondatasetid in (select crfunctiondatasetid from crFunctionDatasets where setupid=1)", outputRowsNumber);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataRow newdr = dtOut.NewRow();
                    newdr["Endpoint Group"] = OutputCommonClass.getStringFromID(Convert.ToInt32(dr["Endpointgroupid"]), dicEndPointGroup);
                    newdr["Endpoint"] = OutputCommonClass.getStringFromID(Convert.ToInt32(dr["Endpointid"]), dicEndPoint);
                    newdr["Pollutant"] = OutputCommonClass.getStringFromID(Convert.ToInt32(dr["Pollutantid"]), dicPollutant);
                    if (DBNull.Value.Equals(dr["Seasonalmetricid"]))
                    {
                        newdr["Seasonal Metric"] = string.Empty;

                    }
                    else { newdr["Seasonal Metric"] = OutputCommonClass.getStringFromID(Convert.ToInt32(dr["Seasonalmetricid"]), dicSeasonalMetric); }
                    newdr["Metric Statistic"] = OutputCommonClass.getMetricStastic(Convert.ToInt32(dr["Metricstatistic"]));
                    newdr["Study Author"] = dr["Author"].ToString();
                    newdr["Study Year"] = Convert.ToInt32(dr["Yyear"]);
                    if (DBNull.Value.Equals(dr["Locationtypeid"]))
                    {
                        newdr["Study Location Type"] = string.Empty;

                    }
                    else
                    {
                        newdr["Study Location Type"] = OutputCommonClass.getStringFromID(Convert.ToInt32(dr["Locationtypeid"]), dicLocation);
                    }
                    newdr["Study Location"] = dr["Location"].ToString();
                    newdr["Other Pollutants"] = dr["Otherpollutants"].ToString();
                    newdr["Qualifier"] = dr["Qualifier"].ToString();
                    newdr["Reference"] = dr["Reference"].ToString();
                    newdr["Race"] = dr["Race"].ToString();
                    newdr["Ethnicity"] = dr["Ethnicity"].ToString();
                    newdr["Gender"] = dr["Gender"].ToString();
                    newdr["Start Age"] = Convert.ToInt32(dr["StartAge"]);
                    newdr["End Age"] = Convert.ToInt32(dr["EndAge"]);
                    newdr["Function"] = OutputCommonClass.getStringFromID(Convert.ToInt32(dr["Functionalformid"]), dicFunction);
                    newdr["Baseline Function"] = OutputCommonClass.getStringFromID(Convert.ToInt32(dr["Baselinefunctionalformid"]), dicBaselineFunction);
                    if (DBNull.Value.Equals(dr["Incidencedatasetid"]))
                    {
                        newdr["Incidence DataSet"] = string.Empty;

                    }
                    else
                    {
                        newdr["Incidence DataSet"] = OutputCommonClass.getStringFromID(Convert.ToInt32(dr["Incidencedatasetid"]), dicIncidence);
                    }
                    if (DBNull.Value.Equals(dr["Prevalencedatasetid"]))
                    {
                        newdr["Prevalence DataSet"] = string.Empty;

                    }
                    else
                    {
                        newdr["Prevalence DataSet"] = OutputCommonClass.getStringFromID(Convert.ToInt32(dr["Prevalencedatasetid"]), dicPrevalence);
                    }
                    if (DBNull.Value.Equals(dr["Variabledatasetid"]))
                    {
                        newdr["Variable DataSet"] = string.Empty;

                    }
                    else
                    {
                        newdr["Variable DataSet"] = OutputCommonClass.getStringFromID(Convert.ToInt32(dr["Variabledatasetid"]), dicVariable);
                    }
                    dtOut.Rows.Add(newdr);
                }
                CommonClass.SaveCSV(dtOut, fileName);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        //private void btnViewMetadata_Click(object sender, EventArgs e)
        //{
        //    //_metadataObj = SQLStatementsCommonClass.getMetadata(_datasetID, CommonClass.ManageSetup.SetupID);
        //    //_metadataObj.SetupName = txtHealthImpactFunction.Text;
        //    //ViewEditMetadata viewEMdata = new ViewEditMetadata(_metadataObj);
        //    //DialogResult dr = viewEMdata.ShowDialog();
        //    //if (dr.Equals(DialogResult.OK))
        //    //{
        //    //    _metadataObj = viewEMdata.MetadataObj;
        //    //}
        //}

        private void Cleanup_WhenCanceled()
        {
            int crFunctionDatasetId = 0;
            int datasetid = 0;
            string commandText = string.Empty;
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            try
            {
                //NOTE THIS IS WRONG - NOT GOOD.  It will delete all for the current dataset.  Problem - editing and adding in a new file then click cancel 
                //will casue it to delete all references to the dataset.
                //
                //commandText = string.Format("SELECT CRFUNCTIONDATASETID FROM CRFUNCTIONDATASETS WHERE SETUPID = {0} AND CRFunctionDataSetName = '{1}'", CommonClass.ManageSetup.SetupID, txtHealthImpactFunction.Text);
                //crFunctionDatasetId = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                //commandText = string.Format("delete from CRFunctionDataSets where CRFunctionDataSetName='{0}' and setupid={1}", txtHealthImpactFunction.Text, CommonClass.ManageSetup.SetupID);
                //int i = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                //commandText = "select DATASETTYPEID FROM DATASETTYPES WHERE DATASETTYPENAME = 'Healthfunctions'";
                //datasetid = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                //commandText = string.Format("DELETE FROM METADATAINFORMATION WHERE SETUPID ={0} AND DATASETID = {1} AND DATASETTYPEID = {2}", CommonClass.ManageSetup.SetupID, crFunctionDatasetId, datasetid);
                //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}