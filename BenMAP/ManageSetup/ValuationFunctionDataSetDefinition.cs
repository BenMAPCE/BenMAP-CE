using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BenMAP;
using BrightIdeasSoftware;
using System.Diagnostics;
using System.Collections;
using ESIL.DBUtility;

namespace BenMAP
{
    public partial class ValuationFunctionDataSetDefinition : FormBase
    {
        List<int> lstdeleteValuationid = new List<int>();
        private object _dataSetID = -1;
        private string _dataSetName;
        private object _newDataSetID = null;//used for copying an existing dataset that is locked. (the new datasetid)
        private object _oldDataSetID = null;//used for copying an existing dataset that is locked. (the locked datasetid)
        
        private bool _isEdit = false;
        //DataTable dtForLoading = new DataTable();
        int valuationFunctionDataSetID = -1;
        private bool _isLocked = false;
        private bool _CopyingDataset = false;
        private const int VALUATIONDATASETTYPEID = 7;
        

        /// <summary>
        /// Initializes a new instance of the <see cref="ValuationFunctionDataSetDefinition"/> class.
        /// </summary>
        public ValuationFunctionDataSetDefinition()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValuationFunctionDataSetDefinition"/> class.
        /// The this statement calls the constructor with the same param.
        /// </summary>
        /// <param name="dataSetName">Name of the data set.</param>
        public ValuationFunctionDataSetDefinition(string dataSetName): this()//calls the constructor that will initializae components
        {
            _dataName = dataSetName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValuationFunctionDataSetDefinition"/> class.
        /// The this statement calls the constructor with the same param.
        /// </summary>
        /// <param name="dataSetName">Name of the data set.</param>
        /// <param name="datasetId">The dataset identifier.</param>
        public ValuationFunctionDataSetDefinition(string dataSetName, object datasetId, bool isLocked): this(dataSetName)//calls the constructor that sets the data set name
        {
            _dataSetID = datasetId;
            _isEdit  = !isLocked;
            valuationFunctionDataSetID = Convert.ToInt32(datasetId.ToString());//when doing an edit I need to have the current funciton dataset ID
            _isLocked = isLocked;
            if (_isLocked)
            {
                txtValuationFunctionDataSetName.Enabled = true;//false;
                _dataSetName = dataSetName + "_Copy";
                _dataName = _dataSetName;
                txtValuationFunctionDataSetName.Text = _dataSetName;
                _oldDataSetID = _dataSetID;
                _CopyingDataset = true;
            }
            else
            {
                txtValuationFunctionDataSetName.Enabled = false;
            }
        }

        DataTable dt = new DataTable();
        DataTable _dt = new DataTable();
        List<double> listCustomValue = new List<double>();
        Dictionary<int, List<double>> dicCustomValue = new Dictionary<int, List<double>>();
        int AddCount = 0;
        private MetadataClassObj _metadataObj = null;

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                listCustomValue.Clear();
                ValuationFunctionDefinition frm = new ValuationFunctionDefinition();
                DialogResult rht = frm.ShowDialog();
                if (rht != DialogResult.OK) { return; }
                AddCount--;
                DataRow dr = _dt.NewRow();
                dr[0] = frm.EndpointGroup;
                dr[1] = frm.Endpoint;
                dr[2] = frm.Qualifier;
                dr[3] = frm.Reference;
                dr[4] = frm.StartAge;
                dr[5] = frm.EndAge;
                dr[6] = frm.Function;
                dr[7] = frm.A;
                dr[8] = frm.ADescription;
                dr[9] = frm.ADistribution;
                dr[10] = frm.AParameter1;
                dr[11] = frm.AParameter2;
                dr[12] = frm.B;
                dr[13] = frm.BName;
                dr[14] = frm.C;
                dr[15] = frm.CName;
                dr[16] = frm.D;
                dr[17] = frm.DName;
                dr[18] = AddCount;
                if (frm.ADistribution == "Custom" && frm.listCustom.Count > 0)
                    dicCustomValue.Add(AddCount, frm.listCustom);
                _dt.Rows.Add(dr);
                olvData.DataSource = _dt;
                LoadEndPointGroupEndPointName();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        string _filePath = string.Empty;

        private void btnLoadFromDatabase_Click(object sender, EventArgs e)
        {
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            DataSet ds = new DataSet();
            if(!_isEdit)
            {
                string commandText = string.Format("select * from  ValuationFunctionDataSets where SetupID={0}", CommonClass.ManageSetup.SetupID);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                int dataSetNameCount = ds.Tables[0].Rows.Count;
                for (int dataSetNameRow = 0; dataSetNameRow < dataSetNameCount; dataSetNameRow++)
                {
                    if (txtValuationFunctionDataSetName.Text == ds.Tables[0].Rows[dataSetNameRow]["ValuationFunctionDataSetName"].ToString())
                    {
                        MessageBox.Show("The valuation dataset name is already in use. Please enter a different name.");
                        return; 
                    }
                }
                if (txtValuationFunctionDataSetName.Text == string.Empty)
                {
                    MessageBox.Show("Please input a valid dataset name.");
                    return; 
                }
             }
             LoadFromFile();
        }

        private void LoadFromFile()
        {
            LoadSelectedDataSet lmdataset = new LoadSelectedDataSet("Load Valuation Function Dataset", "Valuation Function Dataset Name:", txtValuationFunctionDataSetName.Text, "Valuationfunction");
            DialogResult dlgr = lmdataset.ShowDialog();
            if (dlgr.Equals(DialogResult.OK))
            {
                dt = lmdataset.MonitorDataSet;
                _metadataObj = lmdataset.MetadataObj;
                olvData.ClearObjects();
                LoadValuationFunctionOLV();
                // commented out next line to avoid double loading
                // LoadDatabase();
                //after loading the datafile, the dataset is Edit flag should be reset to true.  
                //This will allow for additional files to be added.
                _isEdit = true;
            }
        }

        private void LoadValuationFunctionOLV()
        {
            try
            {
                //OpenFileDialog openFileDialog = new OpenFileDialog();
                //openFileDialog.InitialDirectory = Application.StartupPath + @"E:\";
                //openFileDialog.Filter = "All Files|*.*|CSV files|*.csv|XLS files|*.xls|XLSX files|*.xlsx";
                //openFileDialog.FilterIndex = 2;
                //openFileDialog.RestoreDirectory = true;
                //if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
                //_filePath = openFileDialog.FileName;
                //string strfilepath = System.IO.Path.GetExtension(_filePath);
                //dt = CommonClass.ExcelToDataTable(_filePath);
                if (dt == null) { return; }
                int rowCount = dt.Rows.Count;
                int colCount = dt.Columns.Count;
                int iEndpointGroup = -1;
                int iEndpoint = -1;
                int iQualifier = -1;
                int iReference = -1;
                int iStartAge = -1;
                int iEndAge = -1;
                int iFunction = -1;
                int iA = -1;
                int iNameA = -1;
                int iDistrubutionA = -1;
                int iP1A = -1;
                int iP2A = -1;
                int iB = -1;
                int iNameB = -1;
                int iC = -1;
                int iNameC = -1;
                int iD = -1;
                int iNameD = -1;
                for (int i = 0; i < colCount; i++)
                {
                    if (dt.Columns[i].ColumnName.ToLower().Contains("dist") && dt.Columns[i].ColumnName.ToLower().Contains("a"))
                    {
                        iDistrubutionA = i;
                    }
                    switch (dt.Columns[i].ColumnName.ToLower().Replace(" ", ""))
                    {
                        case "endpointgroup": iEndpointGroup = i;
                            break;
                        case "endpoint": iEndpoint = i;
                            break;
                        case "qualifier": iQualifier = i;
                            break;
                        case "reference": iReference = i;
                            break;
                        case "startage": iStartAge = i;
                            break;
                        case "endage": iEndAge = i;
                            break;
                        case "function": iFunction = i;
                            break;
                        case "a": iA = i;
                            break;
                        case "parameter1a": iP1A = i;
                            break;
                        case "p1a": iP1A = i;
                            break;
                        case "parameter2a": iP2A = i;
                            break;
                        case "p2a": iP2A = i;
                            break;
                        case "namea": iNameA = i;
                            break;
                        case "b": iB = i;
                            break;
                        case "nameb": iNameB = i;
                            break;
                        case "c": iC = i;
                            break;
                        case "namec": iNameC = i;
                            break;
                        case "d": iD = i;
                            break;
                        case "named": iNameD = i;
                            break;
                    }
                }

                string warningtip = "";
                if (iEndpointGroup < 0) warningtip = "'Endpoint Group', ";
                if (iEndpoint < 0) warningtip += "'Endpoint', ";
                if (iQualifier < 0) warningtip += "'Qualifier', ";
                if (iReference < 0) warningtip += "'Reference', ";
                if (iStartAge < 0) warningtip += "'StartAge', ";
                if (iEndAge < 0) warningtip += "'EndAge', ";
                if (iFunction < 0) warningtip += "'Function', ";
                if (iA < 0) warningtip += "'A', ";
                if (iNameA < 0) warningtip += "'Name A', ";
                if (iDistrubutionA < 0) warningtip += "'Distrubution A', ";
                if (iP1A < 0) warningtip += "'Parameter 1 A', ";
                if (iP2A < 0) warningtip += "'Parameter 2 A', ";
                if (iB < 0) warningtip += "'B', ";
                if (iNameB < 0) warningtip += "'Name B', ";
                if (iC < 0) warningtip += "'C', ";
                if (iNameC < 0) warningtip += "'Name C', ";
                if (iD < 0) warningtip += "'D', ";
                if (iNameD < 0) warningtip += "'Name D', ";
                if (warningtip != "")
                {
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
                    dr[2] = dt.Rows[i][iQualifier];
                    dr[3] = dt.Rows[i][iReference];
                    dr[4] = dt.Rows[i][iStartAge];
                    dr[5] = dt.Rows[i][iEndAge];
                    dr[6] = dt.Rows[i][iFunction];
                    dr[7] = dt.Rows[i][iA];
                    dr[8] = dt.Rows[i][iNameA];
                    dr[9] = dt.Rows[i][iDistrubutionA];
                    dr[10] = dt.Rows[i][iP1A];
                    dr[11] = dt.Rows[i][iP2A];
                    dr[12] = dt.Rows[i][iB];
                    dr[13] = dt.Rows[i][iNameB];
                    dr[14] = dt.Rows[i][iC];
                    dr[15] = dt.Rows[i][iNameC];
                    dr[16] = dt.Rows[i][iD];
                    dr[17] = dt.Rows[i][iNameD];
                    dr[18] = --AddCount;

                    _dt.Rows.Add(dr);
                    // commented out next line to avoid double import
                    //_dt.ImportRow(dr);
                    //dtForLoading.ImportRow(dr);
                }

                olvData.DataSource = _dt;
                LoadEndPointGroupEndPointName();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        
        private void LoadDatabase()
        {
            //if (dtForLoading.Rows.Count < 1)
            if (_dt.Rows.Count < 1)
                {
                MessageBox.Show("No dataset was selected for import or created.  Please select a dataset to import or 'Add' information to careate a data set.");
                btnLoadFromDatabase.Focus();
                return;
            }
            
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            DataSet ds = new DataSet();
            string commandText = string.Empty;
            object obj = null;
            //int valuationFunctionDataSetID = 0;
            try
            {
                //if (_dataName == string.Empty)
                if(Convert.ToInt32(_dataSetID) == -1)
                {
                    commandText = string.Format("select * from  ValuationFunctionDataSets where SetupID={0}", CommonClass.ManageSetup.SetupID);
                    ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    int dataSetNameCount = ds.Tables[0].Rows.Count;
                    #region Moved to btnLoadFromDatabase_Click event starting at line 110. doing the check before user has a chance to select file
                    //for (int dataSetNameRow = 0; dataSetNameRow < dataSetNameCount; dataSetNameRow++)
                    //{
                    //    if (txtValuationFunctionDataSetName.Text == ds.Tables[0].Rows[dataSetNameRow]["ValuationFunctionDataSetName"].ToString())
                    //    {
                    //        MessageBox.Show("The valuation dataset name is already in use. Please enter a different name.");
                    //        //return; I am not going to do anything for now until I can have more than one file loaded and have Metadata for each file loaded to a single dataset
                    //    }
                    //} 
                    //if (txtValuationFunctionDataSetName.Text == string.Empty)
                    //{
                    //    MessageBox.Show("Please input a valid dataset name.");
                    //    //return; I am not going to do anything for now until I can have more than one file loaded and have Metadata for each file loaded to a single dataset
                    //}
                    #endregion
                    //if(valuationFunctionDataSetID == -1)
                    if(Convert.ToInt32(_dataSetID) == -1)
                    {
                        commandText = string.Format("select max(VALUATIONFUNCTIONDATASETID) from ValuationFunctionDataSets");
                        obj = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText)) + 1;
                        valuationFunctionDataSetID = int.Parse(obj.ToString());
                        _dataSetID = valuationFunctionDataSetID;
                    }
                    int rth;
                    #region Dead code
                    //commandText = string.Format("select max(VALUATIONFUNCTIONDATASETID) from ValuationFunctionDataSets");
                    //object obj = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText)) + 1;
                    //valuationFunctionDataSetID = int.Parse(obj.ToString());
                    //commandText = string.Format("SELECT VALUATIONFUNCTIONDATASETID from ValuationFunctionDataSets WHERE VALUATIONFUNCTIONDATASETID = {0} AND SETUPID = {1}", _dataSetID, CommonClass.ManageSetup.SetupID);
                    
                    #endregion
                    
                    commandText = string.Format("SELECT VALUATIONFUNCTIONDATASETID from ValuationFunctionDataSets WHERE VALUATIONFUNCTIONDATASETID = {0} AND SETUPID = {1}", valuationFunctionDataSetID, CommonClass.ManageSetup.SetupID);
                    obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                    if (obj == null)
                    {
                        //The 'F' is for the locked column in ValuationFunctionDataSets - this is imported and is not predefined. the first 'F' is read only, the second 'F' is locked or not
                        commandText = string.Format("insert into ValuationFunctionDataSets values ({0},{1},'{2}','F', 'F')", valuationFunctionDataSetID, CommonClass.ManageSetup.SetupID, txtValuationFunctionDataSetName.Text.Replace("'", "''"));
                        rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    }
                    //commandText = string.Format("insert into ValuationFunctionDataSets values ({0},{1},'{2}','F')", valuationFunctionDataSetID, CommonClass.ManageSetup.SetupID, txtValuationFunctionDataSetName.Text.Replace("'", "''"));
                    //int rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    Dictionary<string, int> dicEndpointGroup = new Dictionary<string, int>();
                    commandText = "select EndpointGroupID,LOWER(EndpointGroupName) from EndpointGroups ";
                    DataSet dsEndpointGroup = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    foreach (DataRow drEndpointGroup in dsEndpointGroup.Tables[0].Rows)
                    {
                        dicEndpointGroup.Add(drEndpointGroup["LOWER"].ToString(), Convert.ToInt32(drEndpointGroup["EndpointGroupID"]));
                    }
                    Dictionary<string, int> dicValuationFunction = new Dictionary<string, int>();
                    commandText = "select FunctionalFormID,FunctionalFormText from VALUATIONFUNCTIONALFORMS";
                    DataSet dsValuationFunction = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    foreach (DataRow drFunction in dsValuationFunction.Tables[0].Rows)
                    {
                        if (!dicValuationFunction.ContainsKey(drFunction["FunctionalFormText"].ToString()))
                            dicValuationFunction.Add(drFunction["FunctionalFormText"].ToString(), Convert.ToInt32(drFunction["FunctionalFormID"]));
                    }
                    int dgvRowCount = _dt.Rows.Count;
                    //int dgvRowCount = dtForLoading.Rows.Count;
                    try
                    {
                        for (int row = 0; row < dgvRowCount; row++)
                        {
                            CommonClass.Connection.Close();
                            commandText = string.Format("select max(VALUATIONFUNCTIONID) from ValuationFunctions");
                            obj = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText)) + 1;
                            int valuationFunctionID = int.Parse(obj.ToString());
                            commandText = string.Format("select ValuationFunctionDataSetID from ValuationFunctionDataSets where ValuationFunctionDataSetName='{0}' and SetupID={1}", txtValuationFunctionDataSetName.Text, CommonClass.ManageSetup.SetupID);
                            obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                            int VValuationFunctionDataSetID = int.Parse(obj.ToString());
                            int EndpointGroupID = 0;
                            //if (dicEndpointGroup.ContainsKey(_dt.Rows[row][0].ToString().ToLower()))
                            if (dicEndpointGroup.ContainsKey(_dt.Rows[row][0].ToString().ToLower()))
                            {
                                //EndpointGroupID = dicEndpointGroup[_dt.Rows[row][0].ToString().ToLower()];
                                EndpointGroupID = dicEndpointGroup[_dt.Rows[row][0].ToString().ToLower()];
                            }
                            else
                            {
                                commandText = string.Format("select max(EndpointGroupid) from EndpointGroups");
                                obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                                EndpointGroupID = int.Parse(obj.ToString()) + 1;
                                //commandText = string.Format("insert into EndpointGroups values({0},'{1}')", EndpointGroupID, _dt.Rows[row][0].ToString());
                                commandText = string.Format("insert into EndpointGroups values({0},'{1}')", EndpointGroupID, _dt.Rows[row][0].ToString());
                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                dicEndpointGroup.Add(_dt.Rows[row][0].ToString().ToLower(), EndpointGroupID);
                                //dicEndpointGroup.Add(dtForLoading.Rows[row][0].ToString().ToLower(), EndpointGroupID);
                            }
                            int EndpointID = 0;             //BenMAP 441/442/444--Address error created when passing single quote to database
                            commandText = string.Format("select EndpointID from Endpoints where EndpointGroupID={0} and LOWER(EndpointName)='" + _dt.Rows[row][1].ToString().ToLower().Replace("'", "''") + "' ", EndpointGroupID);
                            //commandText = string.Format("select EndpointID from Endpoints where EndpointGroupID={0} and LOWER(EndpointName)='" +                                                    dtForLoading.Rows[row][1].ToString().ToLower() + "' ", EndpointGroupID);
                            obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                            if (obj == null)
                            {
                                commandText = string.Format("select max(EndpointID) from Endpoints");
                                obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                                EndpointID = int.Parse(obj.ToString()) + 1;
                                commandText = string.Format("insert into Endpoints values({0},{1},'{2}')", EndpointID, EndpointGroupID, _dt.Rows[row][1].ToString().Replace("'", "''"));
                                //commandText = string.Format("insert into Endpoints values({0},{1},'{2}')", EndpointID, EndpointGroupID, dtForLoading.Rows[row][1].ToString());
                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                            }
                            else
                            {
                                EndpointID = int.Parse(obj.ToString());
                            }
                            int FunctionalFormID = 0;
                            if (dicValuationFunction.ContainsKey(_dt.Rows[row][6].ToString()))
                            //if (dicValuationFunction.ContainsKey(dtForLoading.Rows[row][6].ToString()))
                            {
                                FunctionalFormID = dicValuationFunction[_dt.Rows[row][6].ToString()];
                                //FunctionalFormID = dicValuationFunction[dtForLoading.Rows[row][6].ToString()];
                            }
                            else
                            {
                                commandText = string.Format("select max(FUNCTIONALFORMID) from VALUATIONFUNCTIONALFORMS");
                                obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                                FunctionalFormID = int.Parse(obj.ToString()) + 1;
                                commandText = string.Format("insert into VALUATIONFUNCTIONALFORMS values({0},'{1}')", FunctionalFormID, _dt.Rows[row][6].ToString());
                                //commandText = string.Format("insert into VALUATIONFUNCTIONALFORMS values({0},'{1}')", FunctionalFormID, dtForLoading.Rows[row][6].ToString());
                                rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                dicValuationFunction.Add(_dt.Rows[row][6].ToString(), FunctionalFormID);
                                //dicValuationFunction.Add(dtForLoading.Rows[row][6].ToString(), FunctionalFormID);
                            }

                            //commandText = string.Format("insert into ValuationFunctions values({0},{1},{2},{3},'{4}','{5}',{6},{7},{8},{9},'{10}','{11}',{12},{13},{14},'{15}',{16},'{17}',{18},'{19}', {20})",
                            //                            valuationFunctionID, VValuationFunctionDataSetID, EndpointGroupID, EndpointID, _dt.Rows[row][2].ToString().Replace("'", "''"), _dt.Rows[row][3].ToString().Replace("'", "''"),
                            //                                _dt.Rows[row][4], _dt.Rows[row][5], FunctionalFormID, _dt.Rows[row][7], _dt.Rows[row][8].ToString().Replace("'", "''"), _dt.Rows[row][9].ToString().Replace("'", "''"),
                            //                                _dt.Rows[row][10], _dt.Rows[row][11], _dt.Rows[row][12], _dt.Rows[row][13].ToString().Replace("'", "''"), _dt.Rows[row][14], _dt.Rows[row][15].ToString().Replace("'", "''"),
                            //                                _dt.Rows[row][16], _dt.Rows[row][17].ToString().Replace("'", "''"), _metadataObj.MetadataEntryId);
                            // 2015 09 23 - BENMAP-357 replaced metadata objext in insert
                            /// // removed metadata object from insert -
                            /// 
                            // 2017 05 29 IEc BENMAP-225 Address issue with adding entries to newly created datasets (e.g. HIF, Valuation, ...)
                            // Issue caused by _metadataObj not set to an instance while manually enter data instead of importing from a file.
                            // Please note that [CRFunctions].[METADATAID] links to [METADATAINFORMATION].[METADATAENTRYID].
                            if (_metadataObj == null)
                            {
                                _metadataObj = new MetadataClassObj();
                                _metadataObj.SetupId = CommonClass.ManageSetup.SetupID;
                                _metadataObj.DatasetId = valuationFunctionDataSetID;
                                _metadataObj.DatasetTypeId = VALUATIONDATASETTYPEID; //according to BENMAP-353's notes, datasettypeid for Valuation Functions has been hardcoded to 7.
                                _metadataObj.ImportDate = DateTime.Today.ToString("d");
                                _metadataObj.Description = "Health impact functions manually entered through Health Impact Function Definition form.";

                                commandText = "select max(METADATAENTRYID) from METADATAINFORMATION";
                                _metadataObj.MetadataEntryId = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                            }

                            commandText = string.Format("insert into ValuationFunctions(VALUATIONFUNCTIONID, VALUATIONFUNCTIONDATASETID, ENDPOINTGROUPID, ENDPOINTID, "
                                        + "QUALIFIER, REFERENCE, STARTAGE, ENDAGE, FUNCTIONALFORMID, A, NAMEA, DISTA, P1A, P2A, B, NAMEB, C, NAMEC, D, NAMED, METADATAID) "
                                        + "values({0},{1},{2},{3},'{4}','{5}',{6},{7},{8},{9},'{10}','{11}',{12},{13},{14},'{15}',{16},'{17}',{18},'{19}', {20})",
                                                        valuationFunctionID, VValuationFunctionDataSetID, EndpointGroupID, EndpointID, _dt.Rows[row][2].ToString().Replace("'", "''"),
                                                        _dt.Rows[row][3].ToString().Replace("'", "''"), _dt.Rows[row][4], _dt.Rows[row][5], FunctionalFormID, _dt.Rows[row][7],
                                                        _dt.Rows[row][8].ToString().Replace("'", "''"), _dt.Rows[row][9].ToString().Replace("'", "''"),
                                                        _dt.Rows[row][10], _dt.Rows[row][11], _dt.Rows[row][12], _dt.Rows[row][13].ToString().Replace("'", "''"),
                                                        _dt.Rows[row][14], _dt.Rows[row][15].ToString().Replace("'", "''"),
                                                       _dt.Rows[row][16], _dt.Rows[row][17].ToString().Replace("'", "''"), _metadataObj.MetadataEntryId);

                            //commandText = string.Format("insert into ValuationFunctions values({0},{1},{2},{3},'{4}','{5}',{6},{7},{8},{9},'{10}','{11}',{12},{13},{14},'{15}',{16},'{17}',{18},'{19}', {20})",
                            //                            valuationFunctionID, VValuationFunctionDataSetID, EndpointGroupID, EndpointID, dtForLoading.Rows[row][2].ToString().Replace("'", "''"),
                            //                            dtForLoading.Rows[row][3].ToString().Replace("'", "''"), dtForLoading.Rows[row][4], dtForLoading.Rows[row][5], FunctionalFormID, dtForLoading.Rows[row][7],
                            //                            dtForLoading.Rows[row][8].ToString().Replace("'", "''"), dtForLoading.Rows[row][9].ToString().Replace("'", "''"),
                            //                            dtForLoading.Rows[row][10], dtForLoading.Rows[row][11], dtForLoading.Rows[row][12], dtForLoading.Rows[row][13].ToString().Replace("'", "''"),
                            //                            dtForLoading.Rows[row][14], dtForLoading.Rows[row][15].ToString().Replace("'", "''"),
                            //                           dtForLoading.Rows[row][16], dtForLoading.Rows[row][17].ToString().Replace("'", "''"), _metadataObj.MetadataEntryId);

                            rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                            if (_dt.Rows[row][9].ToString() == "Custom" && dicCustomValue.ContainsKey(Convert.ToInt32(_dt.Rows[row][18])) && dicCustomValue[Convert.ToInt32(_dt.Rows[row][18])].Count > 0)
                            //if (dtForLoading.Rows[row][9].ToString() == "Custom" && dicCustomValue.ContainsKey(Convert.ToInt32(dtForLoading.Rows[row][18])) && dicCustomValue[Convert.ToInt32(dtForLoading.Rows[row][18])].Count > 0)
                            {
                                FirebirdSql.Data.FirebirdClient.FbCommand fbCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
                                fbCommand.Connection = CommonClass.Connection;
                                fbCommand.CommandType = CommandType.Text;
                                fbCommand.Connection.Open();
                                DataTable dtCustomValue = new DataTable();
                                DataColumn dc = new DataColumn();
                                dc.ColumnName = "Value";
                                dtCustomValue.Columns.Add(dc);
                                foreach (double value in dicCustomValue[Convert.ToInt32(_dt.Rows[row][18])])
                                //foreach (double value in dicCustomValue[Convert.ToInt32(dtForLoading.Rows[row][18])])
                                {
                                    DataRow dr = dtCustomValue.NewRow();
                                    dr["Value"] = value;
                                    dtCustomValue.Rows.Add(dr);
                                }
                                if (dtCustomValue.Rows.Count != 0)
                                {
                                    int rowCount = dtCustomValue.Rows.Count;
                                    for (int j = 0; j < (rowCount / 125) + 1; j++)
                                    {
                                        commandText = "execute block as declare ValuationFunctionID int;" + " BEGIN ";
                                        for (int k = 0; k < 125; k++)
                                        {
                                            commandText = commandText + string.Format(" insert into VALUATIONFUNCTIONCUSTOMENTRIES values ({0},{1});", valuationFunctionID, dtCustomValue.Rows[j * 125 + k][0]);
                                        }
                                        commandText = commandText + "END";
                                        fbCommand.CommandText = commandText;
                                        fbCommand.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {       //BenMAP 441/442/444--Provide user with information on what caused import of Valuation Function to fail
                        Logger.LogError(ex);
                        MessageBox.Show("Error Loading Valuation Function(s)" + Environment.NewLine + Environment.NewLine + commandText + Environment.NewLine + ex.StackTrace, "Database Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    valuationFunctionDataSetID = Convert.ToInt32(_dataSetID);
                    commandText = string.Format("select ValuationFunctionDataSetID from ValuationFunctionDataSets where ValuationFunctionDataSetName='{0}'", _dataName.Replace("'", "''"));
                    //commandText = string.Format("select ValuationFunctionDataSetID from ValuationFunctionDataSets where ValuationFunctionDataSetName='{0}'", txtValuationFunctionDataSetName.Text.Replace("'", "''"));
                    obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                    int currentValuationFunctionDataSetID = int.Parse(obj.ToString());
                    commandText = string.Format("update ValuationFunctionDataSets set ValuationFunctionDataSetName='{0}' where ValuationFunctionDataSetID={1}", txtValuationFunctionDataSetName.Text.Replace("'", "''"), currentValuationFunctionDataSetID);
                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                    string deleteValuation = "";
                    foreach (int i in lstdeleteValuationid)
                    {
                        deleteValuation += i.ToString() + ",";
                    }
                    if (deleteValuation.Length > 1)
                    {
                        deleteValuation = "(" + deleteValuation.Substring(0, deleteValuation.Length - 1) + ")";
                        commandText = string.Format("delete from ValuationFunctions where ValuationFunctionid in {0}", deleteValuation);
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    }
                    
                    for (int row = 0; row < _dt.Rows.Count; row++)
                    //for (int row = 0; row < dtForLoading.Rows.Count; row++)
                    {
                        commandText = string.Format("select max(VALUATIONFUNCTIONID) from ValuationFunctions");
                        obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                        int valuationFunctionID = int.Parse(obj.ToString()) + 1;
                        commandText = string.Format("select ValuationFunctionDataSetID from ValuationFunctionDataSets where ValuationFunctionDataSetName='{0}'", txtValuationFunctionDataSetName.Text);
                        obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                        int VValuationFunctionDataSetID = int.Parse(obj.ToString());
                        commandText = string.Format("select EndpointGroupID from EndpointGroups where EndpointGroupName='{0}'", _dt.Rows[row][0].ToString());
                        //commandText = string.Format("select EndpointGroupID from EndpointGroups where EndpointGroupName='{0}'", dtForLoading.Rows[row][0].ToString());
                        obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                        int EndpointGroupID = int.Parse(obj.ToString());
                        commandText = string.Format("select EndpointID from Endpoints where EndpointGroupID={0} and EndpointName='{1}'", EndpointGroupID, _dt.Rows[row][1].ToString());
                        //commandText = string.Format("select EndpointID from Endpoints where EndpointGroupID={0} and EndpointName='{1}'", EndpointGroupID, dtForLoading.Rows[row][1].ToString());
                        obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                        int EndpointID = int.Parse(obj.ToString());
                        commandText = string.Format("select FunctionalFormID from ValuationFunctionalForms where FunctionalFormText='{0}'", _dt.Rows[row][6].ToString());
                        //commandText = string.Format("select FunctionalFormID from ValuationFunctionalForms where FunctionalFormText='{0}'", dtForLoading.Rows[row][6].ToString());
                        obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                        int FunctionalFormID = int.Parse(obj.ToString());
                        if (Convert.ToInt16(_dt.Rows[row][18].ToString()) > 0)
                        //if (Convert.ToInt16(dtForLoading.Rows[row][18].ToString()) > 0)
                        {
                            //commandText = string.Format("update ValuationFunctions set VALUATIONFUNCTIONDATASETID={0},ENDPOINTGROUPID={1},ENDPOINTID={2},QUALIFIER='{3}',REFERENCE='{4}',STARTAGE={5},ENDAGE={6},FUNCTIONALFORMID={7},A={8},NAMEA='{9}',DISTA='{10}',P1A={11},P2A={12},B={13},NAMEB='{14}',C={15},NAMEC='{16}',D={17},NAMED='{18}' where valuationfunctionid={19}", VValuationFunctionDataSetID, EndpointGroupID, EndpointID, _dt.Rows[row][2].ToString().Replace("'", "''"), _dt.Rows[row][3].ToString().Replace("'", "''"), _dt.Rows[row][4], _dt.Rows[row][5], FunctionalFormID, _dt.Rows[row][7], _dt.Rows[row][8].ToString().Replace("'", "''"), _dt.Rows[row][9].ToString().Replace("'", "''"), _dt.Rows[row][10], _dt.Rows[row][11], _dt.Rows[row][12], _dt.Rows[row][13].ToString().Replace("'", "''"), _dt.Rows[row][14], _dt.Rows[row][15].ToString().Replace("'", "''"), _dt.Rows[row][16], _dt.Rows[row][17].ToString().Replace("'", "''"), Convert.ToInt16(_dt.Rows[row][18].ToString()));
                            //commandText = string.Format("update ValuationFunctions set VALUATIONFUNCTIONDATASETID={0},ENDPOINTGROUPID={1},ENDPOINTID={2},QUALIFIER='{3}',REFERENCE='{4}', " +
                            //                            "STARTAGE={5},ENDAGE={6},FUNCTIONALFORMID={7},A={8},NAMEA='{9}',DISTA='{10}',P1A={11},P2A={12},B={13},NAMEB='{14}',C={15},NAMEC='{16}', " +
                            //                           "D={17},NAMED='{18}' where valuationfunctionid={19}", VValuationFunctionDataSetID, EndpointGroupID, EndpointID,
                            //                            dtForLoading.Rows[row][2].ToString().Replace("'", "''"), dtForLoading.Rows[row][3].ToString().Replace("'", "''"),
                            //                            dtForLoading.Rows[row][4], dtForLoading.Rows[row][5], FunctionalFormID, dtForLoading.Rows[row][7], dtForLoading.Rows[row][8].ToString().Replace("'", "''"),
                            //                            dtForLoading.Rows[row][9].ToString().Replace("'", "''"), dtForLoading.Rows[row][10], dtForLoading.Rows[row][11], dtForLoading.Rows[row][12],
                            //                            dtForLoading.Rows[row][13].ToString().Replace("'", "''"), dtForLoading.Rows[row][14], dtForLoading.Rows[row][15].ToString().Replace("'", "''"),
                            //                            dtForLoading.Rows[row][16], dtForLoading.Rows[row][17].ToString().Replace("'", "''"), Convert.ToInt16(dtForLoading.Rows[row][18].ToString()));
                            commandText = string.Format("update ValuationFunctions set VALUATIONFUNCTIONDATASETID={0},ENDPOINTGROUPID={1},ENDPOINTID={2},QUALIFIER='{3}',REFERENCE='{4}', " +
                                                        "STARTAGE={5},ENDAGE={6},FUNCTIONALFORMID={7},A={8},NAMEA='{9}',DISTA='{10}',P1A={11},P2A={12},B={13},NAMEB='{14}',C={15},NAMEC='{16}', " +
                                                       "D={17},NAMED='{18}' where valuationfunctionid={19}", VValuationFunctionDataSetID, EndpointGroupID, EndpointID,
                                                        _dt.Rows[row][2].ToString().Replace("'", "''"), _dt.Rows[row][3].ToString().Replace("'", "''"),
                                                        _dt.Rows[row][4], _dt.Rows[row][5], FunctionalFormID, _dt.Rows[row][7], _dt.Rows[row][8].ToString().Replace("'", "''"),
                                                        _dt.Rows[row][9].ToString().Replace("'", "''"), _dt.Rows[row][10], _dt.Rows[row][11], _dt.Rows[row][12],
                                                        _dt.Rows[row][13].ToString().Replace("'", "''"), _dt.Rows[row][14], _dt.Rows[row][15].ToString().Replace("'", "''"),
                                                        _dt.Rows[row][16], _dt.Rows[row][17].ToString().Replace("'", "''"), Convert.ToInt16(_dt.Rows[row][18].ToString()));

                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                            if (_dt.Rows[row][9].ToString() == "Custom" && dicCustomValue.ContainsKey(Convert.ToInt32(_dt.Rows[row][18])) && dicCustomValue[Convert.ToInt32(_dt.Rows[row][18])].Count > 0)
                            //if (dtForLoading.Rows[row][9].ToString() == "Custom" &&
                            //    dicCustomValue.ContainsKey(Convert.ToInt32(dtForLoading.Rows[row][18])) &&
                            //    dicCustomValue[Convert.ToInt32(dtForLoading.Rows[row][18])].Count > 0)
                            {
                                FirebirdSql.Data.FirebirdClient.FbCommand fbCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
                                fbCommand.Connection = CommonClass.Connection;
                                fbCommand.CommandType = CommandType.Text;
                                fbCommand.Connection.Open();
                                commandText = "delete from valuationfunctioncustomentries where valuationfunctionid =" + _dt.Rows[row][18].ToString();
                                //commandText = "delete from valuationfunctioncustomentries where valuationfunctionid =" + dtForLoading.Rows[row][18].ToString();
                                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                                DataTable dtCustomValue = new DataTable();
                                DataColumn dc = new DataColumn();
                                dc.ColumnName = "Value";
                                dtCustomValue.Columns.Add(dc);
                                foreach (double value in dicCustomValue[Convert.ToInt32(_dt.Rows[row][18])])
                                //foreach (double value in dicCustomValue[Convert.ToInt32(dtForLoading.Rows[row][18])])
                                {
                                    DataRow dr = dtCustomValue.NewRow();
                                    dr["Value"] = value;
                                    dtCustomValue.Rows.Add(dr);
                                }
                                int rowCount = dtCustomValue.Rows.Count;
                                for (int l = 0; l < (rowCount / 125) + 1; l++)
                                {
                                    commandText = "execute block as declare ValuationFunctionID int;" + " BEGIN ";
                                    for (int k = 0; k < 125; k++)
                                    {
                                        if (l * 125 + k < rowCount)
                                        {
                                            //commandText = commandText + string.Format(" insert into VALUATIONFUNCTIONCUSTOMENTRIES values ({0},{1});", Convert.ToInt16(_dt.Rows[row][18].ToString()), dtCustomValue.Rows[l * 125 + k][0]);
                                            //commandText = commandText + string.Format(" insert into VALUATIONFUNCTIONCUSTOMENTRIES values ({0},{1});",
                                            //              Convert.ToInt16(dtForLoading.Rows[row][18].ToString()), dtCustomValue.Rows[l * 125 + k][0]);
                                            commandText = commandText + string.Format(" insert into VALUATIONFUNCTIONCUSTOMENTRIES values ({0},{1});",
                                                          Convert.ToInt16(_dt.Rows[row][18].ToString()), dtCustomValue.Rows[l * 125 + k][0]);
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
                        else if (Convert.ToInt16(_dt.Rows[row][18].ToString()) < 0)
                        //else if (Convert.ToInt16(dtForLoading.Rows[row][18].ToString()) < 0)
                        {
                            
                            commandText = string.Format("insert into ValuationFunctions(VALUATIONFUNCTIONID, VALUATIONFUNCTIONDATASETID, ENDPOINTGROUPID, ENDPOINTID, "
                                    + "QUALIFIER, REFERENCE, STARTAGE, ENDAGE, FUNCTIONALFORMID, A, NAMEA, DISTA, P1A, P2A, B, NAMEB, C, NAMEC, D, NAMED) " 
                                    + "values({0},{1},{2},{3},'{4}','{5}',{6},{7},{8},{9},'{10}','{11}',{12},{13},{14},'{15}',{16},'{17}',{18},'{19}' )",
                                                        valuationFunctionID, VValuationFunctionDataSetID, EndpointGroupID, EndpointID, _dt.Rows[row][2].ToString().Replace("'", "''"),
                                                        _dt.Rows[row][3].ToString().Replace("'", "''"), _dt.Rows[row][4], _dt.Rows[row][5], FunctionalFormID, _dt.Rows[row][7],
                                                        _dt.Rows[row][8].ToString().Replace("'", "''"), _dt.Rows[row][9].ToString().Replace("'", "''"), _dt.Rows[row][10], _dt.Rows[row][11],
                                                        _dt.Rows[row][12], _dt.Rows[row][13].ToString().Replace("'", "''"), _dt.Rows[row][14], _dt.Rows[row][15].ToString().Replace("'", "''"),
                                                        _dt.Rows[row][16], _dt.Rows[row][17].ToString().Replace("'", "''"));
                            //commandText = string.Format("insert into ValuationFunctions values({0},{1},{2},{3},'{4}','{5}',{6},{7},{8},{9},'{10}','{11}',{12},{13},{14},'{15}',{16},'{17}',{18},'{19}', {20})",
                            //                            valuationFunctionID, VValuationFunctionDataSetID, EndpointGroupID, EndpointID, dtForLoading.Rows[row][2].ToString().Replace("'", "''"),
                            //                            dtForLoading.Rows[row][3].ToString().Replace("'", "''"), dtForLoading.Rows[row][4], dtForLoading.Rows[row][5], FunctionalFormID, dtForLoading.Rows[row][7],
                            //                            dtForLoading.Rows[row][8].ToString().Replace("'", "''"), dtForLoading.Rows[row][9].ToString().Replace("'", "''"), dtForLoading.Rows[row][10], dtForLoading.Rows[row][11],
                            //                            dtForLoading.Rows[row][12], dtForLoading.Rows[row][13].ToString().Replace("'", "''"), dtForLoading.Rows[row][14], dtForLoading.Rows[row][15].ToString().Replace("'", "''"),
                            //                            dtForLoading.Rows[row][16], dtForLoading.Rows[row][17].ToString().Replace("'", "''"), _metadataObj.MetadataEntryId);
                            
                            int rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                            if (_dt.Rows[row][9].ToString() == "Custom" && dicCustomValue.ContainsKey(Convert.ToInt32(_dt.Rows[row][18])) && dicCustomValue[Convert.ToInt32(_dt.Rows[row][18])].Count > 0)
                            //if (dtForLoading.Rows[row][9].ToString() == "Custom" && 
                            //    dicCustomValue.ContainsKey(Convert.ToInt32(dtForLoading.Rows[row][18])) && 
                            //    dicCustomValue[Convert.ToInt32(dtForLoading.Rows[row][18])].Count > 0)
                            {
                                FirebirdSql.Data.FirebirdClient.FbCommand fbCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
                                fbCommand.Connection = CommonClass.Connection;
                                fbCommand.CommandType = CommandType.Text;
                                fbCommand.Connection.Open();
                                DataTable dtCustomValue = new DataTable();
                                DataColumn dc = new DataColumn();
                                dc.ColumnName = "Value";
                                dtCustomValue.Columns.Add(dc);
                                foreach (double value in dicCustomValue[Convert.ToInt32(_dt.Rows[row][18])])
                                //foreach (double value in dicCustomValue[Convert.ToInt32(dtForLoading.Rows[row][18])])
                                {
                                    DataRow dr = dtCustomValue.NewRow();
                                    dr["Value"] = value;
                                    dtCustomValue.Rows.Add(dr);
                                }
                                int rowCount = dtCustomValue.Rows.Count;
                                for (int l = 0; l < (rowCount / 125) + 1; l++)
                                {
                                    commandText = "execute block as declare ValuationFunctionID int;" + " BEGIN ";
                                    for (int k = 0; k < 125; k++)
                                    {
                                        if (l * 125 + k < rowCount)
                                        {
                                            commandText = commandText + string.Format(" insert into VALUATIONFUNCTIONCUSTOMENTRIES values ({0},{1});", valuationFunctionID, dtCustomValue.Rows[l * 125 + k][0]);
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
                }
                insertMetadata(valuationFunctionDataSetID);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // 2015 09 09 - Add confirmation message BENMAP-332
                if (olvData.Items.Count == 0) { MessageBox.Show("There are no data to be deleted."); return; }
                if (olvData.SelectedObject == null) { MessageBox.Show("You must select a row to delete."); return; }
               
                //if (olvData.SelectedObjects == null || olvData.Items.Count == 0)
                //{ return; }
                DialogResult rtn = MessageBox.Show("Delete this function?", "Confirm Deletion", MessageBoxButtons.YesNo);
                if (rtn == DialogResult.Yes)
                {
                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        foreach (Object olv in olvData.SelectedObjects)
                        {
                            if (olvColumn19.GetValue(olv).ToString() == _dt.Rows[i][18].ToString())
                            {
                                if (dicCustomValue.ContainsKey(Convert.ToInt16(olvColumn19.GetValue(olv).ToString())))
                                    dicCustomValue.Remove(Convert.ToInt16(olvColumn19.GetValue(olv).ToString()));
                                lstdeleteValuationid.Add(Convert.ToInt16(olvColumn19.GetValue(olv).ToString()));
                                _dt.Rows.Remove(_dt.Rows[i]);
                            }
                        }
                    }
                    olvData.DataSource = _dt;
                    LoadEndPointGroupEndPointName();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_isLocked)//doing a copy
            {
                CopyDatabase();
                this.DialogResult = DialogResult.OK;
                return;
            }
            else
            {
                LoadDatabase();
                this.DialogResult = DialogResult.OK;
            }
        }

        private void insertMetadata(int valuationFunctionDataSetID)
        {
            _metadataObj.DatasetId = valuationFunctionDataSetID;

            // BENMAP-346 - hardcoded datasettypeid as it was not getting set by common class getDatasetID call
            _metadataObj.DatasetTypeId = VALUATIONDATASETTYPEID;
            // _metadataObj.DatasetTypeId = SQLStatementsCommonClass.getDatasetID("Valuationfunction");
            if (!SQLStatementsCommonClass.insertMetadata(_metadataObj))
            {
                MessageBox.Show("Failed to save Metadata.");
            }

        }

        private string _dataName = string.Empty;
        
        private void ValuationFunctionDataSetDefinition_Load(object sender, EventArgs e)
        {
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            CommonClass.SetupOLVEmptyListOverlay(this.olvData.EmptyListMsgOverlay as BrightIdeasSoftware.TextOverlay);
            string commandText = string.Empty;
            try
            {
                if (_dataName != string.Empty)
                {
                    txtValuationFunctionDataSetName.Text = _dataName;
                    commandText = string.Format("select ValuationFunctionDataSetID from ValuationFunctionDataSets where ValuationFunctionDataSetName='{0}'", _dataName.Replace("'", "''"));
                    object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                    commandText = string.Format("select endpointgroups.endpointgroupname,endpoints.endpointname, valuationfunctions.qualifier, valuationfunctions.reference,valuationfunctions.startage,valuationfunctions.endage,valuationfunctionalforms.functionalformtext,valuationfunctions.a,valuationfunctions.namea,valuationfunctions.dista,valuationfunctions.p1a,valuationfunctions.p2a,valuationfunctions.b,valuationfunctions.nameb,valuationfunctions.c,valuationfunctions.namec,valuationfunctions.d,valuationfunctions.named,valuationfunctions.valuationfunctionid from valuationfunctions , endpoints , endpointgroups,valuationfunctionalforms where valuationfunctions.endpointid=endpoints.endpointid and endpointgroups.endpointgroupid=valuationfunctions.endpointgroupid and valuationfunctions.functionalformid=valuationfunctionalforms.functionalformid and valuationfunctiondatasetid={0} order by endpointgroupname asc", obj);
                    dt = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText).Tables[0];
                    if (dt != null)
                    {
                        olvData.DataSource = dt;
                        _dt = dt;
                        cboEndpointGroup.Items.Add("");
                        cboEndpoint.Items.Add("");
                    }
                }
                else
                {
                    int number = 0;
                    int ValuationFunctionatasetID = 0;
                    do
                    {
                        string comText = "select ValuationFunctionDatasetID from ValuationFunctionDatasets where ValuationFunctionDatasetName=" + "'ValuationFunctionDataSet" + Convert.ToString(number) + "'";
                        ValuationFunctionatasetID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, comText));
                        number++;
                    } while (ValuationFunctionatasetID > 0);
                    txtValuationFunctionDataSetName.Text = "ValuationFunctionDataSet" + Convert.ToString(number - 1);

                    commandText = string.Format("select  endpointgroups.endpointgroupname,endpoints.endpointname, valuationfunctions.qualifier, valuationfunctions.reference,valuationfunctions.startage,valuationfunctions.endage,valuationfunctionalforms.functionalformtext,valuationfunctions.a,valuationfunctions.namea,valuationfunctions.dista,valuationfunctions.p1a,valuationfunctions.p2a,valuationfunctions.b,valuationfunctions.nameb,valuationfunctions.c,valuationfunctions.namec,valuationfunctions.d,valuationfunctions.named, valuationfunctions.valuationfunctionid from valuationfunctions , endpoints , endpointgroups,valuationfunctionalforms where valuationfunctions.endpointid=endpoints.endpointid and endpointgroups.endpointgroupid=valuationfunctions.endpointgroupid and valuationfunctions.functionalformid=valuationfunctionalforms.functionalformid and valuationfunctions.valuationfunctiondatasetid=null");
                    dt = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText).Tables[0];
                    olvData.DataSource = dt;
                    _dt = dt;
                }
                LoadEndPointGroupEndPointName();
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
                if (olvData.SelectedItem == null) return;
                string EndpointGroup = olvcEndpointGroup.GetValue(olvData.SelectedObject).ToString();
                string Endpoint = olvcEndpoint.GetValue(olvData.SelectedObject).ToString();
                string Qualifier = olvColumn3.GetValue(olvData.SelectedObject).ToString();
                string Reference = olvColumn4.GetValue(olvData.SelectedObject).ToString();
                int StartAge = int.Parse(olvColumn5.GetValue(olvData.SelectedObject).ToString());
                int EndAge = int.Parse(olvColumn6.GetValue(olvData.SelectedObject).ToString());
                string Function = olvColumn7.GetValue(olvData.SelectedObject).ToString();
                float A = float.Parse(olvColumn8.GetValue(olvData.SelectedObject).ToString());
                string NameA = olvColumn9.GetValue(olvData.SelectedObject).ToString();
                string ADistribution = olvColumn10.GetValue(olvData.SelectedObject).ToString();
                float P1A = float.Parse(olvColumn11.GetValue(olvData.SelectedObject).ToString());
                float P2A = float.Parse(olvColumn12.GetValue(olvData.SelectedObject).ToString());
                float B = float.Parse(olvColumn13.GetValue(olvData.SelectedObject).ToString());
                string NameB = olvColumn14.GetValue(olvData.SelectedObject).ToString();
                float C = float.Parse(olvColumn15.GetValue(olvData.SelectedObject).ToString());
                string NameC = olvColumn16.GetValue(olvData.SelectedObject).ToString();
                float D = float.Parse(olvColumn17.GetValue(olvData.SelectedObject).ToString());
                string NameD = olvColumn18.GetValue(olvData.SelectedObject).ToString();
                if (olvColumn10.GetValue(olvData.SelectedObject).ToString() == "Custom" && Convert.ToInt32(olvColumn19.GetValue(olvData.SelectedObject).ToString()) > 0)
                {
                    if (dicCustomValue.ContainsKey(Convert.ToInt32(olvColumn19.GetValue(olvData.SelectedObject).ToString())))
                        listCustomValue = dicCustomValue[Convert.ToInt32(olvColumn19.GetValue(olvData.SelectedObject).ToString())];
                    else
                    {
                        ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                        DataSet ds = new DataSet();
                        string commandText = string.Format("select Vvalue from VALUATIONFUNCTIONCUSTOMENTRIES where VALUATIONFUNCTIONID={0}", Convert.ToInt32(olvColumn19.GetValue(olvData.SelectedObject).ToString()));
                        ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                        List<double> listCustom = new List<double>();
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            listCustom.Add(Convert.ToDouble(ds.Tables[0].Rows[i][0]));
                        }
                        listCustomValue = listCustom;
                    }
                }
                else if (olvColumn10.GetValue(olvData.SelectedObject).ToString() == "Custom" && Convert.ToInt32(olvColumn19.GetValue(olvData.SelectedObject).ToString()) < 0)
                {
                    if (dicCustomValue.ContainsKey(Convert.ToInt32(olvColumn19.GetValue(olvData.SelectedObject).ToString())))
                        listCustomValue = dicCustomValue[Convert.ToInt32(olvColumn19.GetValue(olvData.SelectedObject).ToString())];
                }
                else
                    listCustomValue = new List<double>();
                if (_dt.Rows.Count == 0) { return; }
                ValuationFunctionDefinition frm = new ValuationFunctionDefinition(EndpointGroup, Endpoint, Qualifier, Reference, StartAge, EndAge, Function, A, NameA, ADistribution, P1A, P2A, B, NameB, C, NameC, D, NameD, listCustomValue);
                DialogResult rth = frm.ShowDialog();
                if (rth != DialogResult.OK) { return; }
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    if (_dt.Rows[i][18].ToString() == olvColumn19.GetValue(olvData.SelectedObject).ToString())
                    {
                        _dt.Rows[i][0] = frm.EndpointGroup;
                        _dt.Rows[i][1] = frm.Endpoint;
                        _dt.Rows[i][2] = frm.Qualifier;
                        _dt.Rows[i][3] = frm.Reference;
                        _dt.Rows[i][4] = frm.StartAge;
                        _dt.Rows[i][5] = frm.EndAge;
                        _dt.Rows[i][6] = frm.Function;
                        _dt.Rows[i][7] = frm.A;
                        _dt.Rows[i][8] = frm.ADescription;
                        _dt.Rows[i][9] = frm.ADistribution;
                        _dt.Rows[i][10] = frm.AParameter1;
                        _dt.Rows[i][11] = frm.AParameter2;
                        _dt.Rows[i][12] = frm.B;
                        _dt.Rows[i][13] = frm.BName;
                        _dt.Rows[i][14] = frm.C;
                        _dt.Rows[i][15] = frm.CName;
                        _dt.Rows[i][16] = frm.D;
                        _dt.Rows[i][17] = frm.DName;
                        _dt.Rows[i][18] = Convert.ToInt32(olvColumn19.GetValue(olvData.SelectedObject).ToString());
                        if (frm.ADistribution == "Custom" && frm.listCustom.Count > 0)
                        {
                            if (dicCustomValue.ContainsKey(Convert.ToInt32(olvColumn19.GetValue(olvData.SelectedObject).ToString())))
                                dicCustomValue.Remove(Convert.ToInt32(olvColumn19.GetValue(olvData.SelectedObject).ToString()));
                            dicCustomValue.Add(Convert.ToInt32(olvColumn19.GetValue(olvData.SelectedObject).ToString()), frm.listCustom);
                        }
                    }
                }
                LoadEndPointGroupEndPointName();
                olvData.DataSource = _dt;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        DataTable _dtEndpointGroup = new DataTable();
        DataTable _dtEndpoint = new DataTable();
        private void cboEndpointGroup_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {

                int maxEndpointWidth = 223;
                int EndpointWidth = 223;
                if (cboEndpointGroup.SelectedIndex == 0)
                {
                    cboEndpoint.Items.Clear();
                    cboEndpoint.Items.Add("");
                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        if (!cboEndpoint.Items.Contains(_dt.Rows[i][1].ToString()))
                        { cboEndpoint.Items.Add(_dt.Rows[i][1].ToString()); }
                        using (Graphics g = this.CreateGraphics())
                        {
                            SizeF string_size = g.MeasureString(_dt.Rows[i][1].ToString(), this.Font);
                            EndpointWidth = Convert.ToInt16(string_size.Width) + 50;
                        }
                        maxEndpointWidth = Math.Max(maxEndpointWidth, EndpointWidth);
                    }
                    cboEndpoint.DropDownWidth = maxEndpointWidth;
                    cboEndpoint.SelectedIndex = 0;
                    olvData.DataSource = _dt;
                }
                else
                {
                    string endpoint = cboEndpoint.Text;
                    olvcEndpoint.ValuesChosenForFiltering.Clear();
                    DataRow[] drEndpoint = _dt.Select("ENDPOINTGROUPNAME='" + cboEndpointGroup.Text + "'");
                    olvData.DataSource = drEndpoint;
                    cboEndpoint.Items.Clear();
                    cboEndpoint.Items.Add("");
                    foreach (DataRow dr in drEndpoint)
                    {
                        if (!cboEndpoint.Items.Contains(dr[1].ToString()))
                        { cboEndpoint.Items.Add(dr[1].ToString()); }
                        using (Graphics g = this.CreateGraphics())
                        {
                            SizeF string_size = g.MeasureString(dr[1].ToString(), this.Font);
                            EndpointWidth = Convert.ToInt16(string_size.Width) + 50;
                        }
                        maxEndpointWidth = Math.Max(maxEndpointWidth, EndpointWidth);
                    }
                    cboEndpoint.DropDownWidth = maxEndpointWidth;
                    if (cboEndpoint.Items.Contains(endpoint))
                    {
                        cboEndpoint.Text = endpoint;
                    }
                    else
                    {
                        cboEndpoint.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void cboEndpoint_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboEndpointGroup.SelectedIndex == 0 && cboEndpoint.SelectedIndex == 0)
                {
                    olvData.DataSource = _dt;
                }
                else if (cboEndpointGroup.SelectedIndex > 0 && cboEndpoint.SelectedIndex == 0)
                {
                    DataRow[] drnewfilter = _dt.Select("ENDPOINTGROUPNAME='" + cboEndpointGroup.Text + "'");
                    olvData.DataSource = drnewfilter;
                }
                else if (cboEndpointGroup.SelectedIndex == 0 && cboEndpoint.SelectedIndex > 0)
                {
                    DataRow[] drnewfilter = _dt.Select("ENDPOINTNAME='" + cboEndpoint.Text + "'");
                    olvData.DataSource = drnewfilter;
                }
                else if (cboEndpointGroup.SelectedIndex > 0 && cboEndpoint.SelectedIndex > 0)
                {
                    DataRow[] drnewfilter = _dt.Select("ENDPOINTGROUPNAME='" + cboEndpointGroup.Text + "' and " + "ENDPOINTNAME='" + cboEndpoint.Text + "'");
                    olvData.DataSource = drnewfilter;
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
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

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            this.TimedFilter(this.olvData, txtFilter.Text);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ShowGroupsChecked(this.olvData, (CheckBox)sender);
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

        private void btnOutput_Click(object sender, EventArgs e)
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
                dtOut.Columns.Add("Qualifier", typeof(string));
                dtOut.Columns.Add("Reference", typeof(string));
                dtOut.Columns.Add("Start Age", typeof(int));
                dtOut.Columns.Add("End Age", typeof(int));
                dtOut.Columns.Add("Function", typeof(string));
                dtOut.Columns.Add("A", typeof(double));
                dtOut.Columns.Add("Name A", typeof(string));
                dtOut.Columns.Add("Distribution A", typeof(string));
                dtOut.Columns.Add("Parameter 1 A", typeof(double));
                dtOut.Columns.Add("Parameter 2 A", typeof(double));
                dtOut.Columns.Add("B", typeof(double));
                dtOut.Columns.Add("Name B", typeof(string));
                dtOut.Columns.Add("C", typeof(double));
                dtOut.Columns.Add("Name C", typeof(string));
                dtOut.Columns.Add("D", typeof(double));
                dtOut.Columns.Add("Name D", typeof(string));

                FireBirdHelperBase fb = new ESILFireBirdHelper();
                string commandText = string.Empty;
                int outputRowsNumber = 50;
                Dictionary<int, string> dicEndPointGroup = OutputCommonClass.getAllEndPointGroup();
                Dictionary<int, string> dicEndPoint = OutputCommonClass.getAllEndPoint();
                Dictionary<int, string> dicFunction = OutputCommonClass.getAllValuationFunctions();
                commandText = "select count(*) from ValuationFunctions";
                int count = (int)fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                if (count < outputRowsNumber) { outputRowsNumber = count; }
                commandText = string.Format("select first {0} Valuationfunctionid,Valuationfunctiondatasetid,Endpointgroupid,Endpointid,Qualifier,Reference,Startage,Endage,Functionalformid, A, Namea, Dista,  P1A,P2A, B, Nameb, C, Namec, D, Named from VALUATIONFUNCTIONS", outputRowsNumber);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataRow newdr = dtOut.NewRow();
                    newdr["Endpoint Group"] = OutputCommonClass.getStringFromID(Convert.ToInt32(dr["endpointGroupID"]), dicEndPointGroup);
                    newdr["Endpoint"] = OutputCommonClass.getStringFromID(Convert.ToInt32(dr["endpointID"]), dicEndPoint);
                    newdr["Qualifier"] = dr["Qualifier"].ToString();
                    newdr["Reference"] = dr["Reference"].ToString();
                    newdr["Start Age"] = Convert.ToInt32(dr["StartAge"]);
                    newdr["End Age"] = Convert.ToInt32(dr["EndAge"]);
                    newdr["Function"] = OutputCommonClass.getStringFromID(Convert.ToInt32(dr["Functionalformid"]), dicFunction);
                    newdr["A"] = Convert.ToDouble(dr["A"]);
                    newdr["Name A"] = dr["Namea"].ToString();
                    newdr["Distribution A"] = dr["Dista"].ToString();
                    newdr["Parameter 1 A"] = Convert.ToDouble(dr["P1A"]);
                    newdr["Parameter 2 A"] = Convert.ToDouble(dr["P2A"]);
                    newdr["B"] = Convert.ToDouble(dr["B"]);
                    newdr["Name B"] = dr["Nameb"].ToString();
                    newdr["C"] = Convert.ToDouble(dr["C"]);
                    newdr["Name C"] = dr["Namec"].ToString();
                    newdr["D"] = Convert.ToDouble(dr["D"]);
                    newdr["Name D"] = dr["Named"].ToString();
                    dtOut.Rows.Add(newdr);
                }
                CommonClass.SaveCSV(dtOut, fileName);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        void LoadEndPointGroupEndPointName()
        {
            if (_dt.Rows.Count == 0)
            //if (dtForLoading.Rows.Count < 1)
            {
                cboEndpointGroup.Items.Clear();
                cboEndpoint.Items.Clear();
            }
            else
            {
                cboEndpointGroup.Items.Clear();
                cboEndpointGroup.Items.Add("");
                cboEndpoint.Items.Clear();
                cboEndpoint.Items.Add("");
                int maxEndpointGroupWidth = 223;
                int EndpointGroupWidth = 223;
                int maxEndpointWidth = 223;
                int EndpointWidth = 223;
                for (int i = 0; i < _dt.Rows.Count; i++)
                //for (int i = 0; i < dtForLoading.Rows.Count; i++)
                {
                    /*if (!cboEndpointGroup.Items.Contains(dtForLoading.Rows[i][0].ToString()))
                    {
                        cboEndpointGroup.Items.Add(dtForLoading.Rows[i][0].ToString());
                    }
                    if (!cboEndpoint.Items.Contains(dtForLoading.Rows[i][1].ToString()))
                    {
                        cboEndpoint.Items.Add(dtForLoading.Rows[i][1].ToString());
                    }*/
                    if (!cboEndpointGroup.Items.Contains(_dt.Rows[i][0].ToString()))
                    {
                        cboEndpointGroup.Items.Add(_dt.Rows[i][0].ToString());
                    }
                    if (!cboEndpoint.Items.Contains(_dt.Rows[i][1].ToString()))
                    {
                        cboEndpoint.Items.Add(_dt.Rows[i][1].ToString());
                    }
                    using (Graphics g = this.CreateGraphics())
                    {
                        //SizeF string_size = g.MeasureString(dtForLoading.Rows[i][0].ToString(), this.Font);
                        SizeF string_size = g.MeasureString(_dt.Rows[i][0].ToString(), this.Font);
                        EndpointGroupWidth = Convert.ToInt16(string_size.Width) + 50;
                    }
                    maxEndpointGroupWidth = Math.Max(maxEndpointGroupWidth, EndpointGroupWidth);
                    using (Graphics g = this.CreateGraphics())
                    {
                        //SizeF string_size = g.MeasureString(dtForLoading.Rows[i][1].ToString(), this.Font);
                        SizeF string_size = g.MeasureString(_dt.Rows[i][1].ToString(), this.Font);
                        EndpointWidth = Convert.ToInt16(string_size.Width) + 50;
                    }
                    maxEndpointWidth = Math.Max(maxEndpointWidth, EndpointWidth);
                }
                cboEndpointGroup.DropDownWidth = maxEndpointGroupWidth;
                cboEndpoint.DropDownWidth = maxEndpointWidth;
            }
        }
        private void CopyDatabase()
        {
            
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                string commandText = string.Empty;
                int maxID = 0;
                int minID = 0;
                object rVal = null;
               //check and see if name is used
                commandText = string.Format("Select ValuationFunctionDATASETNAME from ValuationFunctionDATASETS WHERE ValuationFunctionDATASETNAME = '{0}'", txtValuationFunctionDataSetName.Text.Trim());
                rVal = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                if (rVal != null)
                {
                    MessageBox.Show("Name is already used.  Please select a new name.");
                    txtValuationFunctionDataSetName.Focus();
                    return;
                }

                // string msg = string.Format("Save this file associated with {0} and {1} ?", cboPollutant.GetItemText(cboPollutant.SelectedItem), txtYear.Text);
                string msg = "Copy Valuation Function  Data Set";
                DialogResult result = MessageBox.Show(msg, "Confirm Copy", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No) return;
                //getting a new dataset id
                if (_newDataSetID == null)
                {
                    commandText = commandText = "select max(valuationFunctionDataSetID) from ValuationFunctionDataSets";
                    _newDataSetID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                }
                // first, create a new valuation function data set
                //the 'F's are for the READONLY and LOCKED columns. No idea why we have both. Locked is 'F' for the copy.
                commandText = string.Format("insert into ValuationFunctionDataSets(ValuationFunctionDATASETID, SETUPID, " 
                         + "ValuationFunctionDATASETNAME, READONLY, LOCKED) "
                         + " values ({0},{1},'{2}', 'F', 'F' )", _newDataSetID, CommonClass.ManageSetup.SetupID, txtValuationFunctionDataSetName.Text);
                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
               
                // then, fill the valuation functions table
                commandText = "select max(ValuationFunctionID) from ValuationFunctions";

                maxID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                commandText = string.Format("select min(ValuationFunctionID) from ValuationFunctions where ValuationFunctionDATASETID = {0}", _oldDataSetID);
                minID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                
                //inserting - copying the locked data to the new data set
                commandText = string.Format("insert into ValuationFunctions(VALUATIONFUNCTIONID, VALUATIONFUNCTIONDATASETID, ENDPOINTGROUPID, " +
                              "ENDPOINTID, QUALIFIER, REFERENCE, STARTAGE, ENDAGE, FUNCTIONALFORMID, A, NAMEA, DISTA, P1A, P2A, B, NAMEB, " +
                              "C, NAMEC, D, NAMED, METADATAID) " +
                              "SELECT ValuationFunctionID + ({0} - {1}) + 1, " +
                              "{2}, ENDPOINTGROUPID, ENDPOINTID, QUALIFIER, REFERENCE, STARTAGE, ENDAGE, FUNCTIONALFORMID, A, NAMEA, DISTA, " + 
                              "P1A, P2A, B, NAMEB, C, NAMEC, D, NAMED, METADATAID " +
                              "FROM ValuationFunctions WHERE ValuationFunctionDataSetID= {3}", maxID, minID, _newDataSetID, _oldDataSetID);
                
                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                // now copy the valuation custom entries
                commandText = string.Format("INSERT INTO ValuationFunctionCustomEntries(ValuationFunctionID, VVALUE) "
                                + "SELECT {0} + 1 + C.ValuationFunctionID - {1} AS NEWValuationFunctionID, VVALUE "
                                + "from ValuationFunctionCustomEntries as C INNER JOIN ValuationFunctions AS P "
                                + "ON C.ValuationFunctionID = P.ValuationFunctionID "
                                + "WHERE P.ValuationFunctionDATASETID = {2} ", maxID, minID, _oldDataSetID);
                
                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                

                _metadataObj = new MetadataClassObj();
                _metadataObj.DatasetId = Convert.ToInt32(_newDataSetID);
                _metadataObj.FileName = txtValuationFunctionDataSetName.Text;

            }
            catch (Exception ex)
            {
                progressBar1.Visible = false;
                lblProgress.Text = "";
                //addGridView(_dataSetID);
                Logger.LogError(ex.Message);
            }
        }


    }
}
