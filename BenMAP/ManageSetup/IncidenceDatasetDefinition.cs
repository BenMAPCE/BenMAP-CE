using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using ESIL.DBUtility;
using System.IO;
using System.Drawing;

namespace BenMAP
{
    public partial class IncidenceDatasetDefinition : FormBase
    {
         // 2014 11 19 modified to support lock and copy (clone)
        private bool _isLocked = false;
        private bool _CopyingDataset = false;
        private string _dataSetName;
        private object _dataSetID;//used for when a new dataset is created.
        private object _newDataSetID = null;//used for copying an existing dataset that is locked. (the new datasetid)
        private object _oldDataSetID = null;//used for copying an existing dataset that is locked. (the locked datasetid)
        private DataTable _dtDataFile;
        private MetadataClassObj _metadataObj = null;

        // 2014 11 19 added new overloaded creation class to support lock and copy (clone)
        public IncidenceDatasetDefinition(string name, object id, bool isLocked)
            : this(name, id)
        {
            _isLocked = isLocked;
            if(_isLocked)
            {
                txtDataName.Enabled = true;//false;
                _dataSetName  = name + "_Copy";
                txtDataName.Text = _dataSetName;
                _oldDataSetID = _dataSetID;
                _CopyingDataset = true;
            }
            else
            {
                txtDataName.Enabled = false;
            }
        }
         public IncidenceDatasetDefinition(string name, object id)
        {
            InitializeComponent();
            _dataSetName = name;
            _dataSetID = id;
        }

        public IncidenceDatasetDefinition()
        {
            InitializeComponent();
            _dataSetName = string.Empty;
        }

        DataTable dtIncidence = new DataTable();

        

        public IncidenceDatasetDefinition(string dataSetName, int dataSetID)
        {
            InitializeComponent();
            _dataSetName = dataSetName;
            incidenceDatasetID = dataSetID;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        //private string _dataSetName = string.Empty;
        private int incidenceDatasetID;
        private int _grdiDefinitionID;

        private void IncidenceDatasetDefinition_Load(object sender, EventArgs e)
        {
            lblProgress.Visible = false;
            progressBar1.Visible = false;
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                if (_dataSetName != string.Empty)
                {
                    txtDataName.Text = _dataSetName;
                    BindDataGridView(null, null);
                    cboGridDefinition.Enabled = false;
                }
                else
                {

                    int number = 0;
                    int incidenceDatasetID = 0;
                    do
                    {
                        string comText = "select incidenceDatasetID from incidenceDataSets where incidenceDatasetName=" + "'IncidenceDataSet" + Convert.ToString(number) + "'";
                        incidenceDatasetID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, comText));
                        number++;
                    } while (incidenceDatasetID > 0);
                    txtDataName.Text = "IncidenceDataSet" + Convert.ToString(number - 1);
                    cboGridDefinition.Enabled = true;
                }

                string cmdText = "select GridDefinitionName,GridDefinitionID from GridDefinitions where SetupID=" + CommonClass.ManageSetup.SetupID + "";
                DataSet dts = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, cmdText);
                cboGridDefinition.DataSource = dts.Tables[0];
                cboGridDefinition.DisplayMember = "GRIDDEFINITIONNAME";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        public void BindDataGridView(string EndpointGroupName, string EndpointName)
        {
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            try
            {
                string commandText = string.Format("select IncidenceRates.IncidenceRateID, EndPointGroups.EndPointGroupName,EndPoints.EndPointName,IncidenceRates.Prevalence,Races.RaceName,Ethnicity.EthnicityName,Genders.GenderName,IncidenceRates.StartAge,IncidenceRates.EndAge from IncidenceRates,EndPointGroups,EndPoints,Races,Ethnicity,Genders ,IncidenceDataSets where (IncidenceDataSets.IncidenceDataSetID= IncidenceRates.IncidenceDataSetID) and (IncidenceRates.EndPointGroupID=EndPointGroups.EndPointGroupID) and (IncidenceRates.EndPointID=EndPoints.EndPointID) and (IncidenceRates.RaceID=Races.RaceID) and (IncidenceRates.GenderID=Genders.GenderID) and (IncidenceRates.EthnicityID=Ethnicity.EthnicityID) and IncidenceDataSets.IncidenceDataSetID='{0}'", incidenceDatasetID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                dtIncidence = ds.Tables[0];
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][3].ToString() == "F")
                    {
                        ds.Tables[0].Rows[i][3] = "Incidence";
                    }
                    else if (ds.Tables[0].Rows[i][3].ToString() == "T")
                    {
                        ds.Tables[0].Rows[i][3] = "Prevalence";
                    }
                    else
                    {
                        ds.Tables[0].Rows[i][3] = string.Empty;
                    }
                }
                olvIncidenceRates.DataSource = ds.Tables[0];
                cboEndpoint.Items.Clear();
                cboEndpointGroup.Items.Clear();
                string endpointGroupName = string.Empty;
                string endpointName = string.Empty;
                cboEndpointGroup.Items.Add("");
                cboEndpoint.Items.Add("");
                if (string.IsNullOrEmpty(EndpointGroupName) && string.IsNullOrEmpty(EndpointGroupName))
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        endpointGroupName = ds.Tables[0].Rows[i]["EndPointGroupName"].ToString();
                        if (!cboEndpointGroup.Items.Contains(endpointGroupName))
                            cboEndpointGroup.Items.Add(endpointGroupName);
                        endpointName = ds.Tables[0].Rows[i]["EndPointName"].ToString();
                        if (!cboEndpoint.Items.Contains(endpointName))
                            cboEndpoint.Items.Add(endpointName);
                    }
                    cboEndpointGroup.SelectedIndex = 0;
                    cboEndpoint.SelectedIndex = 0;
                    olvIncidenceRates.SelectedIndex = 0;
                }
                else
                {
                    int maxEndpointGroupWidth = 192;
                    int EndpointGroupWidth = 192;
                    commandText = string.Format("select distinct EndPointGroups.EndPointGroupName from IncidenceRates,EndPointGroups where (IncidenceRates.EndPointGroupID=EndPointGroups.EndPointGroupID) and IncidenceRates.IncidenceDataSetID='{0}'", incidenceDatasetID);
                    DataSet dsEndpointGroup = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    for (int i = 0; i < dsEndpointGroup.Tables[0].Rows.Count; i++)
                    {
                        if (!cboEndpointGroup.Items.Contains(dsEndpointGroup.Tables[0].Rows[i]["EndPointGroupName"].ToString()))
                        {
                            cboEndpointGroup.Items.Add(dsEndpointGroup.Tables[0].Rows[i]["EndPointGroupName"].ToString());
                            using (Graphics g = this.CreateGraphics())
                            {
                                SizeF string_size = g.MeasureString(dsEndpointGroup.Tables[0].Rows[i]["EndPointGroupName"].ToString(), this.Font);
                                EndpointGroupWidth = Convert.ToInt16(string_size.Width) + 50;
                            }
                            maxEndpointGroupWidth = Math.Max(maxEndpointGroupWidth, EndpointGroupWidth);
                        }
                        cboEndpointGroup.DropDownWidth = maxEndpointGroupWidth;
                    }
                    if (cboEndpointGroup.Items.Contains(EndpointGroupName))
                    {
                        cboEndpointGroup.Text = EndpointGroupName;
                        commandText = string.Format("select distinct EndPoints.EndPointName from IncidenceRates,EndPointGroups,EndPoints where (IncidenceRates.EndPointGroupID=EndPointGroups.EndPointGroupID) and (IncidenceRates.EndPointID=EndPoints.EndPointID) and IncidenceRates.IncidenceDataSetID='{0}' and EndPointGroups.EndPointGroupName='{1}'", incidenceDatasetID, EndpointGroupName);
                        DataSet dsEndpoint = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                        for (int i = 0; i < dsEndpoint.Tables[0].Rows.Count; i++)
                        {
                            if (!cboEndpoint.Items.Contains(dsEndpoint.Tables[0].Rows[i]["EndPointName"].ToString()))
                            {
                                cboEndpoint.Items.Add(dsEndpoint.Tables[0].Rows[i]["EndPointName"].ToString());
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            endpointName = ds.Tables[0].Rows[i]["EndPointName"].ToString();
                            if (!cboEndpoint.Items.Contains(endpointName))
                                cboEndpoint.Items.Add(endpointName);
                        }
                    }

                    if (string.IsNullOrEmpty(EndpointName))
                    {
                        cboEndpoint.SelectedIndex = 0;
                    }
                    else
                    {
                        if (cboEndpoint.Items.Contains(EndpointName))
                        {
                            cboEndpoint.Text = EndpointName;
                        }
                        else
                            cboEndpoint.SelectedIndex = 0;
                    }
                    olvIncidenceRates.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        string incidenceRateID;

        private DataTable _dtLoadTable;

        public static Dictionary<string, int> getAllRace()
        {
            try
            {
                Dictionary<string, int> dicRace = new Dictionary<string, int>();
                string commandText = "select RaceID,LOWER(RaceName) from Races";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicRace.Add(dr["LOWER"].ToString(), Convert.ToInt32(dr["RaceID"]));
                }

                return dicRace;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static Dictionary<string, int> getAllEthnicity()
        {
            try
            {
                Dictionary<string, int> dicEthnicity = new Dictionary<string, int>();
                string commandText = "select  EthnicityID,LOWER(EthnicityName) from Ethnicity ";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicEthnicity.Add(dr["LOWER"].ToString(), Convert.ToInt32(dr["EthnicityID"]));
                }

                return dicEthnicity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static Dictionary<string, int> getAllGender()
        {
            try
            {
                Dictionary<string, int> dicGender = new Dictionary<string, int>();
                string commandText = "select  GenderID,LOWER(GenderName) from Genders ";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicGender.Add(dr["LOWER"].ToString(), Convert.ToInt32(dr["GenderID"]));
                }


                return dicGender;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<string, int> getAllEndPointGroup()
        {
            try
            {
                Dictionary<string, int> dicEndPointGroup = new Dictionary<string, int>();
                string commandText = "select EndPointGroupID,LOWER(EndPointGroupName) from EndPointGroups";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicEndPointGroup.Add(dr["LOWER"].ToString(), Convert.ToInt32(dr["EndPointGroupID"]));
                }

                return dicEndPointGroup;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static Dictionary<string, int> getAllOrigEndPointGroup()
        {
            // this is a dictionary without case lowering
            // used by output example file routine
            try
            {
                Dictionary<string, int> dicEndPointGroup = new Dictionary<string, int>();
                string commandText = "select EndPointGroupID, EndPointGroupName from EndPointGroups";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicEndPointGroup.Add(dr["EndPointGroupName"].ToString(), Convert.ToInt32(dr["EndPointGroupID"]));
                }

                return dicEndPointGroup;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static Dictionary<int, List<string>> getEndPointID()
        {
            try
            {
                Dictionary<int, List<string>> dicEndPoint = new Dictionary<int, List<string>>();
                string commandText = "select EndPointID,EndPointGroupID,LOWER(EndPointName) from EndPoints "; ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    List<string> value = new List<string>();
                    value.Add(dr["EndPointGroupID"].ToString());
                    value.Add(dr["LOWER"].ToString());
                    dicEndPoint.Add(Convert.ToInt32(dr["EndPointID"]), value);
                }
                return dicEndPoint;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
       
        public static Dictionary<string, int> getAllEndPoint()
        {
            try
            {
                Dictionary<string, int> dicEndPoint = new Dictionary<string, int>();
                string commandText = "select EndPointID,LOWER(EndPointName) from EndPoints "; ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (!dicEndPoint.Keys.Contains(dr["LOWER"].ToString()))
                        dicEndPoint.Add(dr["LOWER"].ToString(), Convert.ToInt32(dr["EndPointID"]));
                }
                return dicEndPoint;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static Dictionary<string, int> getAllOrigEndPoint()
        {
            try
            {
                Dictionary<string, int> dicEndPoint = new Dictionary<string, int>();
                string commandText = "select EndPointID,EndPointName from EndPoints "; ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (!dicEndPoint.Keys.Contains(dr["EndPointName"].ToString()))
                        dicEndPoint.Add(dr["EndPointName"].ToString(), Convert.ToInt32(dr["EndPointID"]));
                }
                return dicEndPoint;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private int GetValueFromRaceID(string RaceName, Dictionary<string, int> dic)
        {
            try
            {
                return dic[RaceName];
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return 6;
            }
        }

        private int GetValueFromGenderID(string GenderName, Dictionary<string, int> dic)
        {
            try
            {
                return dic[GenderName];
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return 4;
            }
        }

        private int GetValueFromEthnicityID(string EthnicityName, Dictionary<string, int> dic)
        {
            try
            {
                return dic[EthnicityName];
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return 4;
            }
        }

        private string GetValueFromIncidenceID(string IncidenceName, Dictionary<string, string> dic)
        {
            try
            {
                return dic[IncidenceName];
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return "F";
            }
        }

        private static int GetEndpointIDFromDic(int endpointGroupID, string endpoint, Dictionary<int, List<string>> dicEndPoint)
        {
            int endpointID = 0;
            foreach (int key in dicEndPoint.Keys)
            {
                if (dicEndPoint[key][0] == endpointGroupID.ToString() && dicEndPoint[key][1] == endpoint)
                {
                    endpointID = key;
                }
            }
            return endpointID;
        }

        DataTable _dt = new DataTable();

        private void btnBrowse_Click(object sender, EventArgs e)
        {   // this button is labeled "Load from file" in the GUI
            // the input file is loaded into the database here
            if (txtDataName.Text == string.Empty || cboGridDefinition.SelectedItem == null)
            { MessageBox.Show("Please input a dataset name and select a grid definition."); return; }
            else
            {
                FireBirdHelperBase fb = new ESILFireBirdHelper();
                LoadIncidenceDatabase frm = new LoadIncidenceDatabase();
                frm.GridDefinitionID = _grdiDefinitionID;
                string commandText = string.Empty;
                string str = string.Empty;
                try
                {
                    DialogResult rtn = frm.ShowDialog();
                    if (rtn != DialogResult.OK) { return; }
                    _metadataObj = frm.MetadataObj;
                    str = frm.StrPath;
                    commandText = "select GridDefinitionName from GridDefinitions where GridDefinitionID=" + _grdiDefinitionID + "";                    
                    cboGridDefinition.Text = (fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)).ToString();
                    if(frm.IncidneceData != null)
                    {
                        _dtLoadTable = frm.IncidneceData;
                    }
                    else
                    {
                        _dtLoadTable = CommonClass.ExcelToDataTable(str); 
                        if (_dtLoadTable == null)
                            { MessageBox.Show("Failed to import data from CSV file."); return; }
                    }
                    
                    #region Validation has been moved to LoadIncidenceDatabase
                    //This section will be handled by the new validation window launched by LoadIncidenceDatabase window.
                    //A datatable will be available from the LoadIncidenceDatabase window on a true (passed) validation.


                    int iEndpointGroup = -1;
                    int iEndpoint = -1;
                    int iRace = -1;
                    int iGender = -1;
                    int iEthnicity = -1;
                    int iStartAge = -1;
                    int iEndAge = -1;
                    int iColumn = -1;
                    int iRow = -1;
                    int iType = -1;
                    int iValue = -1;
                    for (int i = 0; i < _dtLoadTable.Columns.Count; i++)
                    {
                        switch (_dtLoadTable.Columns[i].ColumnName.ToLower().Replace(" ", ""))
                        {
                            case "endpointgroup": iEndpointGroup = i;
                                break;
                            case "endpoint": iEndpoint = i;
                                break;
                            case "race": iRace = i;
                                break;
                            case "gender": iGender = i;
                                break;
                            case "ethnicity": iEthnicity = i;
                                break;
                            case "startage": iStartAge = i;
                                break;
                            case "endage": iEndAge = i;
                                break;
                            case "column": iColumn = i;
                                break;
                            case "row": iRow = i;
                                break;
                            case "type": iType = i;
                                break;
                            case "value": iValue = i;
                                break;
                        }
                    }
                    //string warningtip = "";
                    //if (iEndpointGroup < 0) warningtip = "'Endpoint Group', ";
                    //if (iEndpoint < 0) warningtip += "'Endpoint', ";
                    //if (iRace < 0) warningtip += "'Race', ";
                    //if (iGender < 0) warningtip += "'Gender', ";
                    //if (iEthnicity < 0) warningtip += "'Ethnicity', ";
                    //if (iStartAge < 0) warningtip += "'Start Age', ";
                    //if (iEndAge < 0) warningtip += "'End Age', ";
                    //if (iColumn < 0) warningtip += "'Column', ";
                    //if (iRow < 0) warningtip += "'Row', ";
                    //if (iType < 0) warningtip += "'Type', ";
                    //if (iValue < 0) warningtip += "'Value', ";
                    //if (warningtip != "")
                    //{
                    //    warningtip = warningtip.Substring(0, warningtip.Length - 2);
                    //    warningtip = "Please check the column header of " + warningtip + ". It is incorrect or does not exist.";
                    //    MessageBox.Show(warningtip, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    return;
                    //}
                    #endregion
                    
                    lblProgress.Visible = true;
                    progressBar1.Visible = true;

                    progressBar1.Step = 1;
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = _dtLoadTable.Rows.Count;
                    progressBar1.Value = progressBar1.Minimum;
                    if (_dataSetName != string.Empty)
                    {
                        commandText = string.Format("select IncidenceDataSetID from IncidenceDataSets where IncidenceDataSetName='{0}' and setupID={1}", _dataSetName, CommonClass.ManageSetup.SetupID);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        incidenceDatasetID = Convert.ToInt32(obj);
                        if (_dataSetName != txtDataName.Text)
                        {
                            commandText = string.Format("select INCIDENCEDATASETID from INCIDENCEDATASETS where INCIDENCEDATASETNAME='{0}' and setupID={1}", txtDataName.Text, CommonClass.ManageSetup.SetupID);
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            if (obj != null) { MessageBox.Show("The dataset name already exists. Please enter a different name."); return; }
                            commandText = string.Format("update INCIDENCEDATASETS set INCIDENCEDATASETNAME='{0}' where INCIDENCEDATASETID='{1}'", txtDataName.Text, incidenceDatasetID);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                    else
                    {
                        commandText = string.Format("select INCIDENCEDATASETID from INCIDENCEDATASETS where INCIDENCEDATASETNAME='{0}' and setupID={1}", txtDataName.Text, CommonClass.ManageSetup.SetupID);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            MessageBox.Show("The dataset name already exists. Please enter a different name.");
                            lblProgress.Visible = false;
                            progressBar1.Visible = false;
                            return;
                        }
                        else
                        {
                            commandText = "select max(IncidenceDatasetID) from INCIDENCEDATASETS";
                            obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                            incidenceDatasetID = Convert.ToInt32(obj) + 1;
                            //The 'F' is for the Locked column in INCIDENCEDATESTS - imported not predefined.
                            commandText = string.Format("insert into INCIDENCEDATASETS (IncidenceDatasetID,SetupID,IncidenceDatasetName,GridDefinitionID, LOCKED) values( {0},{1},'{2}',{3}, 'F')", incidenceDatasetID, CommonClass.ManageSetup.SetupID, txtDataName.Text, _grdiDefinitionID);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        _dataSetName = txtDataName.Text;
                    }
                    
                    // look for duplicate rows in imput file and make a copy in lstMustRemove
                    List<DataRow> lstMustRemove = new List<DataRow>();
                    Dictionary<string, int> dicDtLoadTable = new Dictionary<string, int>();
                    for (int i = 0; i < _dtLoadTable.Rows.Count; i++)
                    {
                        if (dicDtLoadTable.ContainsKey(_dtLoadTable.Rows[i][iEndpointGroup] + "," + _dtLoadTable.Rows[i][iEndpoint] + ","
                            + _dtLoadTable.Rows[i][iRace] + "," + _dtLoadTable.Rows[i][iGender] + ","
                            + _dtLoadTable.Rows[i][iStartAge] + "," + _dtLoadTable.Rows[i][iEndAge] + ","
                            + _dtLoadTable.Rows[i][iType] + "," + _dtLoadTable.Rows[i][iEthnicity] + ","
                            + _dtLoadTable.Rows[i][iColumn] + "," + _dtLoadTable.Rows[i][iRow] + "," + _dtLoadTable.Rows[i][iValue]))
                        {
                            lstMustRemove.Add(_dtLoadTable.Rows[dicDtLoadTable[_dtLoadTable.Rows[i][iEndpointGroup] + "," + _dtLoadTable.Rows[i][iEndpoint] + ","
                            + _dtLoadTable.Rows[i][iRace] + "," + _dtLoadTable.Rows[i][iGender] + ","
                            + _dtLoadTable.Rows[i][iStartAge] + "," + _dtLoadTable.Rows[i][iEndAge] + ","
                            + _dtLoadTable.Rows[i][iType] + "," + _dtLoadTable.Rows[i][iEthnicity] + ","
                            + _dtLoadTable.Rows[i][iColumn] + "," + _dtLoadTable.Rows[i][iRow] + "," + _dtLoadTable.Rows[i][iValue]]]);
                            dicDtLoadTable[_dtLoadTable.Rows[i][iEndpointGroup] + "," + _dtLoadTable.Rows[i][iEndpoint] + ","
                            + _dtLoadTable.Rows[i][iRace] + "," + _dtLoadTable.Rows[i][iGender] + ","
                            + _dtLoadTable.Rows[i][iStartAge] + "," + _dtLoadTable.Rows[i][iEndAge] + ","
                            + _dtLoadTable.Rows[i][iType] + "," + _dtLoadTable.Rows[i][iEthnicity] + ","
                            + _dtLoadTable.Rows[i][iColumn] + "," + _dtLoadTable.Rows[i][iRow] + "," + _dtLoadTable.Rows[i][iValue]] = i;
                        }
                        else
                        {
                            dicDtLoadTable.Add(_dtLoadTable.Rows[i][iEndpointGroup] + "," + _dtLoadTable.Rows[i][iEndpoint] + ","
                            + _dtLoadTable.Rows[i][iRace] + "," + _dtLoadTable.Rows[i][iGender] + ","
                            + _dtLoadTable.Rows[i][iStartAge] + "," + _dtLoadTable.Rows[i][iEndAge] + ","
                            + _dtLoadTable.Rows[i][iType] + "," + _dtLoadTable.Rows[i][iEthnicity] + ","
                            + _dtLoadTable.Rows[i][iColumn] + "," + _dtLoadTable.Rows[i][iRow] + "," + _dtLoadTable.Rows[i][iValue], i);

                        }
                    }

                    if (lstMustRemove.Count() > 0) {
                        // duplicates were found - ask user to overwrite or cancel
                        DialogResult dlgOverwrite = MessageBox.Show("Duplicate incidence data were found in the import file","Continue with load?",MessageBoxButtons.OKCancel);
                        if (dlgOverwrite != DialogResult.OK) {  // user selects cancel import
                            MessageBox.Show("Load cancelled by user");
                            return;
                        }
                    }
                    foreach (DataRow dr in lstMustRemove)
                    {
                        _dtLoadTable.Rows.Remove(dr);
                    }
                    lstMustRemove = new List<DataRow>();
                    dicDtLoadTable = new Dictionary<string, int>();
                    for (int i = 0; i < _dtLoadTable.Rows.Count; i++)
                    {
                        if (dicDtLoadTable.ContainsKey(_dtLoadTable.Rows[i][iEndpointGroup] + "," + _dtLoadTable.Rows[i][iEndpoint] + ","
                            + _dtLoadTable.Rows[i][iRace] + "," + _dtLoadTable.Rows[i][iGender] + ","
                            + _dtLoadTable.Rows[i][iStartAge] + "," + _dtLoadTable.Rows[i][iEndAge] + ","
                            + _dtLoadTable.Rows[i][iType] + "," + _dtLoadTable.Rows[i][iEthnicity] + ","
                            + _dtLoadTable.Rows[i][iColumn] + "," + _dtLoadTable.Rows[i][iRow]))
                        {
                            MessageBox.Show("File import failed. Please check the file for duplicates.");
                            progressBar1.Visible = false;
                            return;
                        }
                        else
                        {
                            dicDtLoadTable.Add(_dtLoadTable.Rows[i][iEndpointGroup] + "," + _dtLoadTable.Rows[i][iEndpoint] + ","
                            + _dtLoadTable.Rows[i][iRace] + "," + _dtLoadTable.Rows[i][iGender] + ","
                            + _dtLoadTable.Rows[i][iStartAge] + "," + _dtLoadTable.Rows[i][iEndAge] + ","
                            + _dtLoadTable.Rows[i][iType] + "," + _dtLoadTable.Rows[i][iEthnicity] + ","
                            + _dtLoadTable.Rows[i][iColumn] + "," + _dtLoadTable.Rows[i][iRow], i);

                        }

                    }
                    dicDtLoadTable.Clear();
                    GC.Collect();
                    
                    DataView dv = _dtLoadTable.DefaultView;
                    DataTable dtDistinct = dv.ToTable(true, dv.Table.Columns[iEndpointGroup].ColumnName, dv.Table.Columns[iEndpoint].ColumnName, dv.Table.Columns[iRace].ColumnName, dv.Table.Columns[iGender].ColumnName, dv.Table.Columns[iEthnicity].ColumnName, dv.Table.Columns[iStartAge].ColumnName, dv.Table.Columns[iEndAge].ColumnName, dv.Table.Columns[iType].ColumnName);
                    Dictionary<string, string> dicIncidence = new Dictionary<string, string>();
                    dicIncidence.Add("incidence", "F");
                    dicIncidence.Add("prevalence", "T");
                    dicIncidence.Add("", "0");
                    for (int j = 0; j < dtDistinct.Rows.Count; j++)
                    {
                        commandText = "select EndPointGroupID from EndPointGroups where lower(EndPointGroupName)='" + dtDistinct.Rows[j][0].ToString().ToLower() + "'";
                        object endPointGroupID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (endPointGroupID == null)
                        {
                            commandText = "select max(EndPointGroupID) from EndPointGroups";
                            endPointGroupID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                            commandText = string.Format("insert into EndpointGroups values ({0},'{1}')", endPointGroupID, dtDistinct.Rows[j][0].ToString());
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        commandText = "select EndPointID from EndPoints where lower(EndPointName)='" + dtDistinct.Rows[j][1].ToString().ToLower().Trim() + "' and EndpointGroupID=" + endPointGroupID + "";
                        object endPointID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (endPointID == null)
                        {
                            commandText = "select max(EndPointID) from EndPoints";
                            endPointID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                            commandText = string.Format("insert into Endpoints values ({0},{1},'{2}')", endPointID, endPointGroupID, dtDistinct.Rows[j][1].ToString());
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        commandText = "select RaceID from Races where lower(RaceName)='" + dtDistinct.Rows[j][2].ToString().ToLower().Trim() + "'";
                        object raceID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (raceID == null)
                        {
                            commandText = "select max(RaceID) from Races";
                            raceID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                            commandText = string.Format("insert into Races values ({0},'{1}')", raceID, dtDistinct.Rows[j][2].ToString());
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        commandText = "select GenderID from Genders where lower(GenderName)='" + dtDistinct.Rows[j][3].ToString().ToLower().Trim() + "'";
                        object genderID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (genderID == null)
                        {
                            commandText = "select max(GenderID) from Genders";
                            genderID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                            commandText = string.Format("insert into Genders values ({0},'{1}')", genderID, dtDistinct.Rows[j][3].ToString());
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        commandText = "select EthnicityID from Ethnicity where lower(ethnicityName)='" + dtDistinct.Rows[j][4].ToString().ToLower().Trim() + "'";
                        object ethnicityID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (ethnicityID == null)
                        {
                            commandText = "select max(EthnicityID) from Ethnicity";
                            ethnicityID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                            commandText = string.Format("insert into Ethnicity values ({0},'{1}')", ethnicityID, dtDistinct.Rows[j][4].ToString());
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        commandText = string.Format("select incidenceRateID from IncidenceRates where incidenceDataSetID={0} and GridDefinitionID={1} and EndPointGroupId={2} and EndPointID={3} and RaceID={4} and GenderID={5} and StartAge={6} and EndAge={7} and Prevalence='{8}' and EthnicityID={9} ", incidenceDatasetID, _grdiDefinitionID, endPointGroupID, endPointID, raceID, genderID, dtDistinct.Rows[j][5], dtDistinct.Rows[j][6], GetValueFromIncidenceID(dtDistinct.Rows[j][7].ToString().ToLower(), dicIncidence), ethnicityID);
                        object incidenceRateID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (incidenceRateID != null)
                        {
                            commandText = string.Format("delete from incidenceEntries where IncidenceRateID={0}", incidenceRateID);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        else
                        {
                            commandText = "select max(incidenceRateID) from IncidenceRates";
                            incidenceRateID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                            commandText = string.Format("insert into IncidenceRates values ({0},{1},{2},{3},{4},{5},{6},{7},{8},'{9}',{10})", incidenceRateID, incidenceDatasetID, _grdiDefinitionID, endPointGroupID, endPointID, raceID, genderID, dtDistinct.Rows[j][5], dtDistinct.Rows[j][6], GetValueFromIncidenceID(dtDistinct.Rows[j][7].ToString().ToLower(), dicIncidence), ethnicityID);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                    }
                    
                    Dictionary<string, int> dicEndPointGroup = getAllEndPointGroup();
                    Dictionary<int, List<string>> dicEndpointID = getEndPointID();
                    Dictionary<string, int> dicGender = getAllGender();
                    Dictionary<string, int> dicRace = getAllRace();
                    Dictionary<string, int> dicEthnicity = getAllEthnicity();
                    
                    /////////////////////////////////////////////////////////////
                    // STOPPED HERE
                    // add check for import rows that duplicate existing rows
                    /*
                    bool bDupRows = false;
                    int iRowCount = 0;
                    while ((iRowCount < _dtLoadTable.Rows.Count) && !bDupRows) {
                        // a duplicate has the same [EndpointGroup], [Endpoint], [Race], [Gender], [StartAge], [EndAge], [Type], [Ethnicity], [Column], [Row]
                        // because the column and row are a property of the gridpoint definitions, the gridpoint id must also match for the row-column match to be match
                        //dicDtLoadTable.Add(_dtLoadTable.Rows[i][iEndpointGroup] + "," + _dtLoadTable.Rows[i][iEndpoint] + ","
                        //    + _dtLoadTable.Rows[i][iRace] + "," + _dtLoadTable.Rows[i][iGender] + ","
                        //    + _dtLoadTable.Rows[i][iStartAge] + "," + _dtLoadTable.Rows[i][iEndAge] + ","
                        //    + _dtLoadTable.Rows[i][iType] + "," + _dtLoadTable.Rows[i][iEthnicity] + ","
                        //    + _dtLoadTable.Rows[i][iColumn] + "," + _dtLoadTable.Rows[i][iRow], i);
                      
                        commandText = "Select INCIDENCERATEID from INCIDENCERATES as R Inner Join IncidenceEntries as E "
                                + " on R.IncidenceRateID = E.IncidenceRateID "
                                + "where R.EndpointGroupID =" + _dtLoadTable.Rows[iRowCount][iEndpointGroup] 
                                + ", and R.EndpointID=" + _dtLoadTable.Rows[iRowCount][iEndpoint] 
                                + ", and R.RaceID=" + _dtLoadTable.Rows[iRowCount][iRace] 
                                + ", and R.GenderID=" + _dtLoadTable.Rows[iRowCount][iGender] 
                                + ", and R.StartAge=" + _dtLoadTable.Rows[iRowCount][iStartAge] 
                                + ", and R.EndAge=" + _dtLoadTable.Rows[iRowCount][iEndAge] 
                                + ", and R.EthnicityID= " + _dtLoadTable.Rows[iRowCount][iEthnicity] 
                                + ", and R.GridDefinition =" + _grdiDefinitionID.ToString()
                                + ", and E.Column =" + _dtLoadTable.Rows[iRowCount][iColumn] 
                                + ", and E.Row=" + _dtLoadTable.Rows[iRowCount][iRow] + ") ";
                        FirebirdSql.Data.FirebirdClient.FbDataReader drDups = fb.ExecuteReader(CommonClass.Connection,CommandType.Text,commandText);
                        if (drDups.HasRows)
                        {
                        }
                        iRowCount++;
                    }
                   
                    if (bDupRows)   // duplicates have been found
                    {
                        DialogResult dlgOverwrite = MessageBox.Show("Imput file contains incident data already in BenMAP ","Load with duplicates?",MessageBoxButtons.OKCancel);
                        if (dlgOverwrite != DialogResult.OK)
                        {  // user selects cancel import
                            MessageBox.Show("Load cancelled by user");
                            return;
                        }
                    }
                    /////////////////////////////////////////////////////////////
                    */
                    progressBar1.Maximum = _dtLoadTable.Rows.Count;
                    for (int i = 0; i < (_dtLoadTable.Rows.Count / 125) + 1; i++)
                    {
                        commandText = "execute block as declare incidenceRateID int;" + " BEGIN ";

                        for (int k = 0; k < 125; k++)
                        {
                            if (i * 125 + k < _dtLoadTable.Rows.Count)
                            {
                                commandText = commandText + string.Format("select incidenceRateID from IncidenceRates  where incidenceDataSetID={0} and GridDefinitionID={1} and EndPointGroupId={2} and EndPointID={3} and RaceID={4} and GenderID={5} and StartAge={6} and EndAge={7} and Prevalence='{8}' and EthnicityID={9}  into :incidenceRateID;", incidenceDatasetID, _grdiDefinitionID, dicEndPointGroup[_dtLoadTable.Rows[i * 125 + k][iEndpointGroup].ToString().ToLower()], GetEndpointIDFromDic(dicEndPointGroup[_dtLoadTable.Rows[i * 125 + k][iEndpointGroup].ToString().ToLower()], _dtLoadTable.Rows[i * 125 + k][iEndpoint].ToString().ToLower().Trim(), dicEndpointID), dicRace[_dtLoadTable.Rows[i * 125 + k][iRace].ToString().ToLower().Trim()], dicGender[_dtLoadTable.Rows[i * 125 + k][iGender].ToString().ToLower().Trim()], _dtLoadTable.Rows[i * 125 + k][iStartAge], _dtLoadTable.Rows[i * 125 + k][iEndAge], dicIncidence[_dtLoadTable.Rows[i * 125 + k][iType].ToString().ToLower()], dicEthnicity[_dtLoadTable.Rows[i * 125 + k][iEthnicity].ToString().ToLower().Trim()]);
                            }
                            else
                            {
                                continue;
                            }
                            commandText = commandText + string.Format(" insert into incidenceEntries values (:incidenceRateID,{0},{1},{2});", _dtLoadTable.Rows[i * 125 + k][iColumn], _dtLoadTable.Rows[i * 125 + k][iRow], _dtLoadTable.Rows[i * 125 + k][iValue].ToString().Trim() == "." ? 0 : _dtLoadTable.Rows[i * 125 + k][iValue]);
                            progressBar1.PerformStep();
                            lblProgress.Text = Convert.ToString((int)((double)progressBar1.Value / progressBar1.Maximum * 100)) + "%";
                            lblProgress.Refresh();

                        }
                        commandText = commandText + "END";
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    } 
                    insertMetadata(incidenceDatasetID);
                    progressBar1.Visible = false;
                    lblProgress.Text = "";
                    lblProgress.Visible = false;
                    BindDataGridView(null, null);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("File import failed. Please check the file for errors.", "Error", MessageBoxButtons.OK);
                    MessageBox.Show("File import failed. Please validate file for more detailed informaton.", "Error", MessageBoxButtons.OK);
                    progressBar1.Visible = false;
                    lblProgress.Text = "";
                    lblProgress.Visible = false;
                    Logger.LogError(ex.Message);
                }
            }
        }
        private void insertMetadata(int dataSetID)
        {

            _metadataObj.DatasetId = dataSetID;

            _metadataObj.DatasetTypeId = SQLStatementsCommonClass.getDatasetID("Incidence");
            if (!SQLStatementsCommonClass.insertMetadata(_metadataObj))
            {
                MessageBox.Show("Failed to save Metadata.");
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {   // 2014 11 19 modified to deal with copy (clone)
                if (string.IsNullOrEmpty(txtDataName.Text) && !_isLocked)
                {
                    MessageBox.Show("Please input dataset name.");
                    return;
                }
                if (_isLocked)//doing a copy
                {
                    CopyDatabase();
                }
                else if (_dataSetName != txtDataName.Text)
                {
                    string commandText = string.Format("select INCIDENCEDATASETID from INCIDENCEDATASETS where INCIDENCEDATASETNAME='{0}' and setupID={1} and incidencedatasetid <> {2}", txtDataName.Text, CommonClass.ManageSetup.SetupID, incidenceDatasetID);
                    FireBirdHelperBase fb = new ESILFireBirdHelper();
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null) { MessageBox.Show("The dataset name already exists. Please enter a different name."); return; }
                    else
                    {
                        commandText = string.Format("update INCIDENCEDATASETS set INCIDENCEDATASETNAME='{0}' where INCIDENCEDATASETID='{1}'", txtDataName.Text, incidenceDatasetID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 2014 11 19 - don't allow user to delete locked set
            if (_isLocked)
            {
                MessageBox.Show("Default (locked) datasets may not be deleted");   
                return;
            }
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            string commandText = string.Empty;
            string msg = string.Empty;
            try
            {
                if (olvIncidenceRates.Items.Count == 0) { msg = "There are no data to be deleted."; return; }
                if (olvIncidenceRates.SelectedObject == null) return;
                if (olvIncidenceRates.SelectedObject is DataRow)
                {
                    DataRow drv = olvIncidenceRates.SelectedObject as DataRow;
                    incidenceRateID = drv["INCIDENCERATEID"].ToString();
                }
                else if (olvIncidenceRates.SelectedObject is DataRowView)
                {
                    DataRowView drv = olvIncidenceRates.SelectedObject as DataRowView;
                    incidenceRateID = drv["INCIDENCERATEID"].ToString();
                }
                msg = string.Format("Delete this incidence or prevalence rate?");
                DialogResult result = MessageBox.Show(msg, "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    commandText = string.Format("delete from INCIDENCEENTRIES where IncidenceRateID={0}", incidenceRateID);
                    int deleteRates = fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    commandText = string.Format("delete from INCIDENCERATES where IncidenceRateID={0}", incidenceRateID);
                    deleteRates = fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    BindDataGridView(cboEndpointGroup.Text, cboEndpoint.Text);
                    if (olvIncidenceRates.Items.Count == 0)
                    {
                        cboEndpointGroup.SelectedIndex = 0;
                        if (olvIncidenceRates.Items.Count == 0)
                        {
                            DataTable dt = new DataTable();
                            olvValues.DataSource = dt;
                        }
                        else
                        {
                            olvIncidenceRates.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void cboGridDefinition_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                _grdiDefinitionID = Convert.ToInt32(((cboGridDefinition.SelectedItem) as DataRowView)["GridDefinitionID"]);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void bdnInfo_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                if (e.ClickedItem == null) { return; }
                string tag = e.ClickedItem.Tag.ToString();
                Console.WriteLine(tag);
                switch (tag)
                {
                    case "first":
                        _pageCurrent = 1;
                        _currentRow = 0;
                        LoadData();
                        break;
                    case "previous":
                        _pageCurrent--;
                        if (_pageCurrent <= 0)
                        {
                            return;
                        }
                        else
                        {
                            _currentRow = _pageSize * (_pageCurrent - 1);
                        }
                        LoadData();
                        break;
                    case "next":
                        _pageCurrent++;
                        if (_pageCurrent > _pageCount)
                        {
                            return;
                        }
                        else
                        {
                            _currentRow = _pageSize * (_pageCurrent - 1);
                        }
                        LoadData();
                        break;
                    case "last":
                        _pageCurrent = _pageCount;
                        _currentRow = _pageSize * (_pageCurrent - 1);
                        LoadData();
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void txtCurrentPage_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var vars = sender as ToolStripTextBox;
                    if (vars == null) { return; }
                    int currentPage = 0;
                    bool ok = int.TryParse(vars.Text, out currentPage);
                    if (!ok) { return; }
                    if (_pageCurrent == currentPage) { return; }
                    _pageCurrent = currentPage;
                    _currentRow = _pageSize * (_pageCurrent - 1);
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void olvIncidenceRates_SelectionChanged(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {

                DataRowView drv = olvIncidenceRates.SelectedObject as DataRowView;

                incidenceRateID = drv["INCIDENCERATEID"].ToString();
                string commandText = "select GridDefinitionName,GridDefinitions.GridDefinitionID as GridDefinitionID from GridDefinitions,IncidenceRates where (GridDefinitions.GridDefinitionID=IncidenceRates.GridDefinitionID )and IncidenceRates.IncidenceRateID=" + incidenceRateID + "";
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);

                cboGridDefinition.Text = ds.Tables[0].Rows[0]["GridDefinitionName"].ToString();
                _grdiDefinitionID = Convert.ToInt32(ds.Tables[0].Rows[0]["GridDefinitionID"]);
                commandText = "select  CColumn,Row,VValue from IncidenceEntries where IncidenceRateID=" + incidenceRateID + "  ";
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                _dtColRowValue = ds.Tables[0];
                InitDataSet();

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void cboEndpointGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ObjectListView olv = olvIncidenceRates;
                if (olv == null || olv.IsDisposed)
                    return;
                OLVColumn column = olv.GetColumn("olvcEndpointGroup");

                ArrayList chosenValues = new ArrayList();
                olvcEndpoint.ValuesChosenForFiltering.Clear();
                if (!string.IsNullOrEmpty(cboEndpointGroup.Text))
                {
                    chosenValues.Add(cboEndpointGroup.Text);
                    olvcEndpointGroup.ValuesChosenForFiltering = chosenValues;
                    olv.UpdateColumnFiltering();
                }
                else
                {
                    olvcEndpointGroup.ValuesChosenForFiltering.Clear();
                    olv.UpdateColumnFiltering();
                }
                if (!string.IsNullOrEmpty(cboEndpointGroup.Text))
                {
                    DataRow[] _dt = dtIncidence.Select("EndPointGroupName='" + cboEndpointGroup.Text + "'");
                    olvIncidenceRates.DataSource = _dt;
                }
                else
                    olvIncidenceRates.DataSource = dtIncidence;
                string commandText = string.Format("select distinct EndPoints.EndPointName from IncidenceRates,EndPointGroups,EndPoints where (IncidenceRates.EndPointGroupID=EndPointGroups.EndPointGroupID) and (IncidenceRates.EndPointID=EndPoints.EndPointID) and IncidenceRates.IncidenceDataSetID='{0}'", incidenceDatasetID);
                if (!string.IsNullOrEmpty(cboEndpointGroup.Text))
                {
                    commandText += " and EndPointGroups.EndPointGroupName='" + cboEndpointGroup.Text + "'";
                }
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet dsEndpoint = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboEndpoint.Items.Clear();
                cboEndpoint.Items.Add("");
                int maxEndpointWidth = 136;
                int EndpointWidth = 136;
                for (int i = 0; i < dsEndpoint.Tables[0].Rows.Count; i++)
                {
                    if (!cboEndpoint.Items.Contains(dsEndpoint.Tables[0].Rows[i]["EndPointName"].ToString()))
                    {
                        cboEndpoint.Items.Add(dsEndpoint.Tables[0].Rows[i]["EndPointName"].ToString());
                    }
                    using (Graphics g = this.CreateGraphics())
                    {
                        SizeF string_size = g.MeasureString(dsEndpoint.Tables[0].Rows[i]["EndPointName"].ToString(), this.Font);
                        EndpointWidth = Convert.ToInt16(string_size.Width) + 50;
                    }
                    maxEndpointWidth = Math.Max(maxEndpointWidth, EndpointWidth);
                }
                cboEndpoint.DropDownWidth = maxEndpointWidth;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void cboEndpoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(cboEndpointGroup.Text) && !string.IsNullOrEmpty(cboEndpoint.Text))
                {
                    DataRow[] _dt = dtIncidence.Select("EndPointGroupName='" + cboEndpointGroup.Text + "' and EndPointName='" + cboEndpoint.Text + "'");
                    olvIncidenceRates.DataSource = _dt;
                }
                else if (string.IsNullOrEmpty(cboEndpointGroup.Text) && !string.IsNullOrEmpty(cboEndpoint.Text))
                {
                    DataRow[] _dt = dtIncidence.Select("EndPointName='" + cboEndpoint.Text + "'");
                    olvIncidenceRates.DataSource = _dt;
                }
                else if (!string.IsNullOrEmpty(cboEndpointGroup.Text) && string.IsNullOrEmpty(cboEndpoint.Text))
                {
                    DataRow[] _dt = dtIncidence.Select("EndPointGroupName='" + cboEndpointGroup.Text + "'");
                    olvIncidenceRates.DataSource = _dt;
                }
                else
                    olvIncidenceRates.DataSource = dtIncidence;
                ObjectListView olv = olvIncidenceRates;
                if (olv == null || olv.IsDisposed)
                    return;
                OLVColumn column = olv.GetColumn("olvcEndpoint");

                ArrayList chosenValues = new ArrayList();
                string selectEndpoint = cboEndpoint.GetItemText(cboEndpoint.SelectedItem);
                if (!string.IsNullOrEmpty(selectEndpoint))
                {
                    chosenValues.Add(selectEndpoint);
                    olvcEndpoint.ValuesChosenForFiltering = chosenValues;
                    olv.UpdateColumnFiltering();
                }
                else
                {
                    olvcEndpoint.ValuesChosenForFiltering.Clear();
                    olv.UpdateColumnFiltering();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            this.TimedFilter(this.olvIncidenceRates, txtFilter.Text);
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

            System.Diagnostics.Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            olv.ModelFilter = filter;
            stopWatch.Stop();
        }

        private void chbGroup_CheckedChanged(object sender, EventArgs e)
        {
            ShowGroupsChecked(this.olvIncidenceRates, (CheckBox)sender);
        }

        private void ShowGroupsChecked(ObjectListView olv, CheckBox cb)
        {
            if (cb.Checked && olv.View == View.List)
            {
                cb.Checked = false;
                MessageBox.Show("ListView's cannot show groups when in List view.", "Incidence ListView", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                olv.ShowGroups = cb.Checked;
                olv.BuildList();
            }
        }

        private string getTypeFromPrevalence(string prevalence)
        {
            if (prevalence == "F") { return "Incidence"; }
            else if (prevalence == "T") { return "Prevalence"; }
            else return null;
        }

        private string getStringFromID(int id, Dictionary<string, int> dic)
        {
            try
            {
                string result = string.Empty;
                foreach (string s in dic.Keys)
                {
                    if (dic[s] == id)
                        result = s;
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return null;
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
                dtOut.Columns.Add("Race", typeof(string));
                dtOut.Columns.Add("Gender", typeof(string));
                dtOut.Columns.Add("Ethnicity", typeof(string));
                dtOut.Columns.Add("Start Age", typeof(int));
                dtOut.Columns.Add("End Age", typeof(int));
                dtOut.Columns.Add("Type", typeof(string));
                dtOut.Columns.Add("Column", typeof(int));
                dtOut.Columns.Add("Row", typeof(int));
                dtOut.Columns.Add("Value", typeof(double));
                FireBirdHelperBase fb = new ESILFireBirdHelper();
                string commandText = string.Empty;
                int outputRowsNumber = 50;
                Dictionary<string, int> dicGender = getAllGender();
                Dictionary<string, int> dicRace = getAllRace();
                Dictionary<string, int> dicEthnicity = getAllEthnicity();
                // changed to mixed case version of endpoint group to prevent case folding - example will not import if lower case
                Dictionary<string, int> dicEndPointGroup = getAllOrigEndPointGroup();
                Dictionary<string, int> dicEndPoint = getAllOrigEndPoint();
                commandText = "select count(*) from IncidenceRates";
                int count = (int)fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                if (count < outputRowsNumber) { outputRowsNumber = count; }
                commandText = string.Format("select first {0} endpointGroupID, endpointID,RaceID,GenderID,EthnicityID,StartAge,EndAge,Prevalence,Ccolumn,Row,Vvalue from IncidenceRates a,IncidenceEntries b where a.IncidenceRateId=b.IncidenceRateId", outputRowsNumber);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataRow newdr = dtOut.NewRow();
                    newdr["Endpoint Group"] = getStringFromID(Convert.ToInt32(dr["endpointGroupID"]), dicEndPointGroup);
                    newdr["Endpoint"] = getStringFromID(Convert.ToInt32(dr["endpointID"]), dicEndPoint);
                    newdr["Race"] = getStringFromID(Convert.ToInt32(dr["RaceID"]), dicRace);
                    newdr["Gender"] = getStringFromID(Convert.ToInt32(dr["GenderID"]), dicGender);
                    newdr["Ethnicity"] = getStringFromID(Convert.ToInt32(dr["EthnicityID"]), dicEthnicity);
                    newdr["Start Age"] = Convert.ToInt32(dr["StartAge"]);
                    newdr["End Age"] = Convert.ToInt32(dr["EndAge"]);
                    newdr["Type"] = getTypeFromPrevalence(dr["Prevalence"].ToString());
                    newdr["Column"] = Convert.ToInt32(dr["Ccolumn"]);
                    newdr["Row"] = Convert.ToInt32(dr["Row"]);
                    newdr["Value"] = Convert.ToDouble(dr["Vvalue"]);
                    dtOut.Rows.Add(newdr);
                }
                CommonClass.SaveCSV(dtOut, fileName);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        public void SaveCSV(DataTable dt, string fileName)
        {
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string data = "";

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j].ToString().Contains(","))
                    {
                        data += "\"" + dt.Rows[i][j].ToString() + "\"";
                    }
                    else
                        data += dt.Rows[i][j].ToString();
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }

            sw.Close();
            fs.Close();
            MessageBox.Show("CSV file saved.", "File saved");
        }


        struct ColRowValue
        {
            public int col, row;
            public double value;
        }
        private void olvValues_ColumnClick(object sender, ColumnClickEventArgs e)
        {
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
                commandText = string.Format("Select INCIDENCEDATASETNAME from INCIDENCEDATASETS WHERE INCIDENCEDATASETNAME = '{0}'", txtDataName.Text.Trim());
                rVal = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                if (rVal != null)
                {
                    MessageBox.Show("Name is already used.  Please select a new name.");
                    txtDataName.Focus();
                    return;
                }

                // string msg = string.Format("Save this file associated with {0} and {1} ?", cboPollutant.GetItemText(cboPollutant.SelectedItem), txtYear.Text);
                string msg = "Copy Incidence Data Set";
                DialogResult result = MessageBox.Show(msg, "Confirm Copy", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No) return;
                //getting a new dataset id
                if (_newDataSetID == null)
                {
                    commandText = commandText = "select max(IncidenceDataSetID) from IncidenceDataSets";
                    _newDataSetID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                }
                // first, create a new incidence data set
                //the 'F' is for the LOCKED column in IncidenceDataSets.  This is being added and is not a predefined.
                commandText = string.Format("Select GridDefinitionID from INCIDENCEDATASETS WHERE INCIDENCEDATASETID = {0}", _oldDataSetID);
                rVal = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                if (rVal != null)   // no Grid Definition
                {
                    commandText = string.Format("insert into IncidenceDataSets(INCIDENCEDATASETID, SETUPID, INCIDENCEDATASETNAME, LOCKED) "
                         + " values ({0},{1},'{2}', 'F')", _newDataSetID, CommonClass.ManageSetup.SetupID, txtDataName.Text);
                }
                else
                { // insert with grid definition id

                    commandText = string.Format("insert into IncidenceDataSets(INCIDENCEDATASETID, SETUPID, INCIDENCEDATASETNAME, GRIDDEFINITIONID, LOCKED) "
                            + " values ({0},{1},'{2}',{3}, 'F')", _newDataSetID, CommonClass.ManageSetup.SetupID, rVal.ToString(), txtDataName.Text);
                }
                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                // then, fill the incidence rates table
                commandText = "select max(IncidenceRateID) from INCIDENCERATES";

                maxID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                commandText = string.Format("select min(INCIDENCERATEID) from INCIDENCERATES where INCIDENCEDATASETID = {0}", _oldDataSetID);
                minID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                //inserting - copying the locked data to the new data set
                commandText = string.Format("insert into IncidenceRates(IncidenceRateid, IncidenceDataSetID, GridDefinitionID, EndpointGroupID, EndpointID, RaceID, "  +
                               "GenderID, StartAge, EndAge, Prevalence, EthnicityID) " +
                              "SELECT IncidenceRateID + ({0} - {1}) + 1, " +
                              "{2}, GridDefinitionID, EndpointGroupID, EndpointID, RaceID, GenderID, StartAge, EndAge, Prevalence, EthnicityID " +
                              "FROM IncidenceRates WHERE IncidenceDataSetID = {3}", maxID, minID, _newDataSetID, _oldDataSetID);
                
                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                // now copy the old values to the new set
                commandText = string.Format("INSERT INTO INCIDENCEENTRIES(INCIDENCERATEID, CCOLUMN, \"ROW\", VVALUE) "
                                + "SELECT {0} + 1 + IR.INCIDENCERATEID - {1} 	AS NEWINCIDENCERATEID, CCOLUMN, \"ROW\", VVALUE "
                                + "from INCIDENCEENTRIES as IE INNER JOIN INCIDENCERATES AS IR 	ON IE.INCIDENCERATEID = IR.INCIDENCERATEID "
                                + "WHERE IR.INCIDENCEDATASETID = {2} ", maxID, minID, _oldDataSetID);
                
                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                _metadataObj = new MetadataClassObj();
                _metadataObj.DatasetId = Convert.ToInt32(_newDataSetID);
                _metadataObj.FileName = txtDataName.Text;

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