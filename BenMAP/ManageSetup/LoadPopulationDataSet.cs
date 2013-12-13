using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ESIL.DBUtility;
using System.Data.OleDb;
using System.Threading;
using LumenWorks.Framework.IO.Csv;
using System.Collections;
using Excel;

namespace BenMAP
{
    public partial class LoadPopulationDataSet : FormBase
    {
        public LoadPopulationDataSet()
        {
            InitializeComponent();
        }

        Dictionary<string, int> dicGender = null;
        Dictionary<string, int> dicRace = null;
        Dictionary<string, int> dicEthnicity = null;
        Dictionary<string, int> dicGenderAll = null;
        Dictionary<string, int> dicRaceAll = null;
        Dictionary<string, int> dicEthnicityAll = null;

        private string _popConfig = "";
        private object _gridDefinID;
        public object _popConfigID;
        private void LoadPopulationDataSet_Load(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                //automatically generated name-increase the number at the end of the name
                int number = 0;
                int PopulationDatasetID = 0;
                do
                {
                    string comText = "select PopulationDatasetID from PopulationDataSets where PopulationDatasetName=" + "'PopulationDataSet" + Convert.ToString(number) + "'";
                    PopulationDatasetID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, comText));
                    number++;
                } while (PopulationDatasetID > 0);
                txtDataSetName.Text = "PopulationDataSet" + Convert.ToString(number - 1);

                //绑定GridDefinitionName的内容

                string commandText = "select GridDefinitionName from GridDefinitions where SetupID=" + CommonClass.ManageSetup.SetupID + "";
                DataSet dt = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                cboGridDefinition.DataSource = dt.Tables[0];
                cboGridDefinition.DisplayMember = "GRIDDEFINITIONNAME";
                cboGridDefinition.SelectedIndex = -1;
                //绑定PopulationConfigurationName的内容
                commandText = "select PopulationConfigurationName from PopulationConfigurations";
                dt = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                cboConfiguration.DataSource = dt.Tables[0];
                cboConfiguration.DisplayMember = "POPULATIONCONFIGURATIONNAME";
                cboConfiguration.SelectedIndex = -1;
                btnBrowseGW.Enabled = false;
                progBarLoadPop.Value = 0;
                lblprogbar.Text = "";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                if (string.IsNullOrEmpty(cboConfiguration.Text)) return;
                PopulationConfigurationDefinition frm = new PopulationConfigurationDefinition();
                frm._configurationName = _popConfig;
                frm._configurationID = _popConfigID;
                DialogResult rtn = frm.ShowDialog();
                //if (rtn == DialogResult.OK)
                //{
                //    //绑定PopulationConfigurationName的内容
                //    string commandText = "select PopulationConfigurationName from PopulationConfigurations";
                //    DataSet dt = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                //    cboConfiguration.DataSource = dt.Tables[0];
                //    cboConfiguration.DisplayMember = "POPULATIONCONFIGURATIONNAME";
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void cboConfiguration_SelectedIndexChanged(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                _popConfig = cboConfiguration.GetItemText(cboConfiguration.SelectedItem).ToString();
                string commandText = string.Format("select PopulationConfigurationID from PopulationConfigurations where PopulationConfigurationName='{0}'", cboConfiguration.GetItemText(cboConfiguration.SelectedItem));
                _popConfigID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                if (CommonClass.ManageSetup.SetupID == 1 && Convert.ToInt16(_popConfigID) == 1)
                    chkUseWoodsPoole.Enabled = true;
                else
                {
                    chkUseWoodsPoole.Checked = false;
                    chkUseWoodsPoole.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                PopulationConfigurationDefinition frm = new PopulationConfigurationDefinition();
                DialogResult rtn = frm.ShowDialog();
                if (rtn == DialogResult.OK)
                {
                    //绑定PopulationConfigurationName的内容
                    string commandText = "select PopulationConfigurationName from PopulationConfigurations";
                    DataSet dt = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    cboConfiguration.DataSource = dt.Tables[0];
                    cboConfiguration.DisplayMember = "POPULATIONCONFIGURATIONNAME";
                    cboConfiguration.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            ESILFireBirdHelper fb = new ESILFireBirdHelper();
            try
            {
                if (cboConfiguration.SelectedItem == null)
                {
                    MessageBox.Show("Please select a population configuration from the drop-down menu.");
                }
                if (cboConfiguration.SelectedItem != null)
                {
                    string msg = "All population datasets associated with this configuration will be deleted. Are you sure you want to delete this population configuration?";
                    DialogResult result = MessageBox.Show(msg, "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        WaitShow("Waiting...");
                        string commandText = string.Format("select PopulationConfigurationID from PopulationConfigurations where PopulationConfigurationName='{0}'", cboConfiguration.GetItemText(cboConfiguration.SelectedItem));
                        object popConID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);

                        commandText = string.Format("delete from AgeRanges where PopulationConfigurationID={0}", popConID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from PopConfigRaceMap where PopulationConfigurationID={0}", popConID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from PopConfigGenderMap where PopulationConfigurationID={0}", popConID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from PopConfigEthnicityMap where PopulationConfigurationID={0}", popConID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("delete from PopulationConfigurations where PopulationConfigurationID={0}", popConID);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        WaitClose();
                        MessageBox.Show("Population configuration successfully deleted.");
                        commandText = "select PopulationConfigurationName from PopulationConfigurations";
                        DataSet dt = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                        cboConfiguration.DataSource = dt.Tables[0];
                        cboConfiguration.DisplayMember = "POPULATIONCONFIGURATIONNAME";
                    }
                }
            }
            catch (Exception ex)
            {
                WaitClose();
                Logger.LogError(ex.Message);
            }
        }

        private void btnBrowseDB_Click(object sender, EventArgs e)
        {

            try
            {
                // Open，获取文件所在的路径
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = CommonClass.ResultFilePath;
                openFileDialog.Filter = "CSV files|*.csv|XLS files|*.xls|XLSX files|*.xlsx";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                { return; }
                txtDataBase.Text = openFileDialog.FileName;//将路径填充到txt文本框中
                //DataParser dp = new DataParser();

                //_dtPopFile = Configuration.ConfigurationCommonClass.csvToDataTable(txtDataBase.Text, true);
                ////_dtPopFile = dp.ReadCSV2DataTable(txtDataBase.Text);
                //int i = _dtPopFile.Rows.Count;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void chkPopulationGrowth_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPopulationGrowth.Checked == true)
            {
                btnBrowseGW.Enabled = true;
            }
            else { btnBrowseGW.Enabled = false; }
        }

        private void btnBrowseGW_Click(object sender, EventArgs e)
        {
            try
            {
                // Open，获取文件所在的路径
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = CommonClass.ResultFilePath;
                openFileDialog.Filter = "CSV files|*.csv|XLS files|*.xls|XLSX files|*.xlsx";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                { return; }
                txtGrowthWeights.Text = openFileDialog.FileName;//将路径填充到txt文本框中
                //DataParser dp = new DataParser();
                //_dtPopGrowth = dp.ReadCSV2DataTable(txtGrowthWeights.Text);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void cboGridDefinition_SelectedIndexChanged(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                string commandText = string.Format("select GridDefinitionID from GridDefinitions where GridDefinitionName='{0}' and  SetupID={1}", cboGridDefinition.GetItemText(cboGridDefinition.SelectedItem), CommonClass.ManageSetup.SetupID);
                _gridDefinID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// 得到所有AgeRange
        /// </summary>
        /// <returns></returns>
        Dictionary<string, int> getAllAgeRange()
        {
            try
            {
                Dictionary<string, int> dicAgeRange = new Dictionary<string, int>();
                string commandText = "select AgeRangeID,AgeRangeName from AgeRanges where PopulationConfigurationID=" + _popConfigID + "";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (!dicAgeRange.Keys.Contains(dr["AgeRangeName"].ToString()))
                    {
                        dicAgeRange.Add(dr["AgeRangeName"].ToString(), Convert.ToInt32(dr["AgeRangeID"]));
                    }
                }

                return dicAgeRange;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        Dictionary<string, int> getAllRace()
        {
            try
            {
                Dictionary<string, int> dicRace = new Dictionary<string, int>();
                string commandText = "select RaceID,LOWER(RaceName) from Races where raceid in (select raceid from Popconfigracemap where PopulationConfigurationID=" + _popConfigID + ")";
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
                return new Dictionary<string, int>();
            }
        }

        Dictionary<string, int> getAllGender()
        {
            try
            {
                Dictionary<string, int> dicGender = new Dictionary<string, int>();
                string commandText = "select GenderID,LOWER(GenderName) from Genders where GenderID in (select GenderID from Popconfiggendermap where PopulationConfigurationID=" + _popConfigID + ")";
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
                return new Dictionary<string, int>();
            }
        }

        Dictionary<string, int> getAllEthnicity()
        {
            try
            {
                Dictionary<string, int> dicEthnicity = new Dictionary<string, int>();
                string commandText = "select EthnicityID,LOWER(EthnicityName) from Ethnicity where EthnicityID in (select EthnicityID from Popconfigethnicitymap where PopulationConfigurationID=" + _popConfigID + ")";
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
                return new Dictionary<string, int>();
            }
        }

        /// <summary>
        /// RaceName匹配ID
        /// </summary>
        /// <param name="RaceName"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        private int GetValueFromRaceID(string RaceName)
        {
            try
            {
                if (!dicRace.ContainsKey(RaceName.ToLower().Trim()))
                {
                    FireBirdHelperBase fb = new ESILFireBirdHelper();
                    if (dicRaceAll.ContainsKey(RaceName.ToLower().Trim()))
                    {
                        string commandText = string.Format("insert into Popconfigracemap values ({0},{1})", _popConfigID, dicRaceAll[RaceName.ToLower().Trim()]);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicRace.Add(RaceName.ToLower().Trim(), dicRaceAll[RaceName.ToLower().Trim()]);
                    }
                    else
                    {
                        string commandText = "select max(RaceID) from Races";
                        object obj = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                        commandText = string.Format("insert into Races values ({0},'{1}')", obj, RaceName.Trim());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicRaceAll.Add(RaceName.ToLower().Trim(), Convert.ToInt16(obj));
                        commandText = string.Format("insert into Popconfigracemap values ({0},{1})", _popConfigID, Convert.ToInt16(obj));
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicRace.Add(RaceName.ToLower().Trim(), Convert.ToInt16(obj));
                    }
                }
                return dicRace[RaceName.ToLower().Trim()];
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return 6;
            }
        }

        /// <summary>
        /// GenderName匹配ID
        /// </summary>
        /// <param name="GenderName"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        private int GetValueFromGenderID(string GenderName)
        {
            try
            {
                if (!dicGender.ContainsKey(GenderName.ToLower().Trim()))
                {
                    FireBirdHelperBase fb = new ESILFireBirdHelper();
                    if (dicGenderAll.ContainsKey(GenderName.ToLower().Trim()))
                    {
                        string commandText = string.Format("insert into Popconfiggendermap values ({0},{1})", _popConfigID, dicGenderAll[GenderName.ToLower().Trim()]);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicGender.Add(GenderName.ToLower().Trim(), dicGenderAll[GenderName.ToLower().Trim()]);
                    }
                    else
                    {
                        string commandText = "select max(GenderID) from Genders";
                        object genderID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                        commandText = string.Format("insert into Genders values ({0},'{1}')", genderID, GenderName.Trim());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicGenderAll.Add(GenderName.Trim().ToLower(), Convert.ToInt16(genderID));
                        commandText = string.Format("insert into Popconfiggendermap values ({0},{1})", _popConfigID, Convert.ToInt16(genderID));
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicGender.Add(GenderName.Trim().ToLower(), Convert.ToInt16(genderID));
                    }
                }
                return dicGender[GenderName.Trim().ToLower()];

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return 4;
            }
        }

        /// <summary>
        /// EthnicityName匹配ID
        /// </summary>
        /// <param name="EthnicityName"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        private int GetValueFromEthnicityID(string EthnicityName)
        {
            try
            {
                if (!dicEthnicity.ContainsKey(EthnicityName.ToLower().Trim()))
                {
                    FireBirdHelperBase fb = new ESILFireBirdHelper();
                    if (dicEthnicityAll.ContainsKey(EthnicityName.ToLower().Trim()))
                    {
                        string commandText = string.Format("insert into Popconfigethnicitymap values ({0},{1})", _popConfigID, dicEthnicityAll[EthnicityName.ToLower().Trim()]);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicEthnicity.Add(EthnicityName.ToLower().Trim(), dicEthnicityAll[EthnicityName.ToLower().Trim()]);
                    }
                    else
                    {
                        string commandText = "select max(EthnicityID) from Ethnicity";
                        object ethnicityID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                        commandText = string.Format("insert into Ethnicity values ({0},'{1}')", ethnicityID, EthnicityName.Trim());
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicEthnicityAll.Add(EthnicityName.Trim().ToLower(), Convert.ToInt16(ethnicityID));
                        commandText = string.Format("insert into Popconfigethnicitymap values ({0},{1})", _popConfigID, Convert.ToInt16(ethnicityID));
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        dicEthnicity.Add(EthnicityName.Trim().ToLower(), Convert.ToInt16(ethnicityID));
                    }
                }
                return dicEthnicity[EthnicityName.Trim().ToLower()];

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return 4;
            }
        }

        /// <summary>
        /// AgeRangeName匹配ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private int GetValueFromAgeRangeID(string AgeRangeName, Dictionary<string, int> dic)
        {
            try
            {
                if (dic.ContainsKey(AgeRangeName))
                    return dic[AgeRangeName];
                else
                    return -1;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return -1;//如果是空值，设为-1，不导进去
            }
        }
        public OleDbDataReader ReadExcel2Reader(string filePath)
        {
            string msg = string.Empty;
            DataSet ds = new DataSet();
            string constring = string.Empty;//连接字符串
            string strSql = string.Empty;// 查询语句
            DataTable dt = new DataTable();
            OleDbConnection conn = null;
            try
            {
                // 获取连接字符串
                constring = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=YES\"", filePath); ;
                if (string.IsNullOrEmpty(constring)) { return null; }
                conn = new OleDbConnection(constring);
                conn.Open();
                //返回Excel的架构，包括各个sheet表的名称,类型，创建时间和修改时间等
                DataTable dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });
                string[] tableNames = new string[dtSheetName.Rows.Count];
                for (int i = 0; i < dtSheetName.Rows.Count; i++)
                {
                    tableNames[i] = dtSheetName.Rows[i]["TABLE_NAME"].ToString();
                }
                if (tableNames == null || tableNames.Length == 0) { msg = string.Format("{0} is empty!\t", filePath); }
                string tableName = "";
                OleDbCommand command = null;
                //for (int i = 0; i < tableNames.Length; i++)
                //{
                //    tableName = tableNames[i];
                //    if (tableName.Contains("_")) { if (tableName.LastIndexOf('_') == tableName.Length - 1) { break; } }
                //    strSql = "select * from [" + tableName + "]";
                //    command = new OleDbDataAdapter(strSql, conn);
                //    //string key = tableName.Replace('\'', ' ');
                //    //key = key.Replace('$', ' ');
                //    //key = key.Trim();
                //    command.Fill(ds, tableName);
                //}
                OleDbDataReader dataReader = null;
                for (int i = 0; i < tableNames.Length; i++)
                {
                    tableName = tableNames[i];
                    if (tableName.Contains("_")) { if (tableName.LastIndexOf('_') == tableName.Length - 1) { break; } }
                    strSql = "select * from [" + tableName + "]";
                    command = new OleDbCommand(strSql, conn);
                    dataReader = command.ExecuteReader();
                    if (dataReader != null)
                    {
                        return dataReader;
                    }
                    //string key = tableName.Replace('\'', ' ');
                    //key = key.Replace('$', ' ');
                    //key = key.Trim();
                    //command.Fill(ds, tableName);
                }
                return dataReader;
                command.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                msg = ex.Message;
                return null;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed) { conn.Close(); }
                if (!string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }// method

        public OleDbDataReader ReadCSV2Reader(string fileName)
        {
            string msg = string.Empty;
            try
            {
                if (!File.Exists(fileName))
                {
                    msg = string.Format("{} does't exist!\t"); return null;
                }
                DataSet ds = new DataSet();
                string strDir, strName;
                strDir = fileName.Substring(0, fileName.LastIndexOf(@"\") + 1);
                strName = fileName.Substring(fileName.LastIndexOf(@"\") + 1);
                string strConn = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}\\;Extended Properties=\"Text;HDR=Yes;FMT=Delimited;IMEX=0\"", strDir);
                OleDbConnection oleCon = new OleDbConnection(strConn);
                oleCon.Open();
                OleDbCommand command = new OleDbCommand(string.Format("Select * from [{0}]", strName), oleCon);
                OleDbDataReader dataReader = null;
                dataReader = command.ExecuteReader();
                oleCon.Close();
                return dataReader;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                msg = ex.Message;
                return null;
            }
            finally
            {
                if (msg != "")
                {
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            string commandText = string.Empty;
            if (CommonClass.Connection.State != ConnectionState.Open)
            { CommonClass.Connection.Open(); }
            FirebirdSql.Data.FirebirdClient.FbConnection fbconnection = CommonClass.getNewConnection();
            fbconnection.Open();
            FirebirdSql.Data.FirebirdClient.FbTransaction fbtra = fbconnection.BeginTransaction();//开始事务
            FirebirdSql.Data.FirebirdClient.FbCommand fbCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
            fbCommand.Connection = fbconnection;
            fbCommand.CommandType = CommandType.Text;
            fbCommand.Transaction = fbtra;
            try
            {
                dicGender = getAllGender();
                dicRace = getAllRace();
                dicEthnicity = getAllEthnicity();
                dicGenderAll = IncidenceDatasetDefinition.getAllGender();
                dicRaceAll = IncidenceDatasetDefinition.getAllRace();
                dicEthnicityAll = IncidenceDatasetDefinition.getAllEthnicity();
                Dictionary<string, int> dicAgeRange = getAllAgeRange();
                if (string.IsNullOrEmpty(txtDataSetName.Text))
                {
                    MessageBox.Show("Please input dataset name first.");
                    return;
                }
                if (string.IsNullOrEmpty(cboGridDefinition.Text))
                {
                    MessageBox.Show("Please select grid definition first.");
                    return;
                }
                if (string.IsNullOrEmpty(cboConfiguration.Text))
                {
                    MessageBox.Show("Please select population configuration first.");
                    return;
                }
                if (string.IsNullOrEmpty(txtDataBase.Text))
                {
                    MessageBox.Show("Please select population data file.");
                    return;
                }
                commandText = "select SetupID,PopulationDataSetName,PopulationConfigurationID,GridDefinitionID from PopulationDataSets";
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                for (int t = 0; t < ds.Tables[0].Rows.Count; t++)
                {
                    if (CommonClass.ManageSetup.SetupID == Convert.ToInt16(ds.Tables[0].Rows[t]["SetupID"]) && txtDataSetName.Text == ds.Tables[0].Rows[t]["PopulationDataSetName"].ToString())
                    {
                        MessageBox.Show("This population dataset name is already in use. Please enter a different name."); return;
                    }
                    if (txtDataSetName.Text.ToString() == ds.Tables[0].Rows[t]["PopulationDataSetName"].ToString() && Convert.ToInt32(_popConfigID) == Convert.ToInt32(ds.Tables[0].Rows[t]["PopulationConfigurationID"]) && Convert.ToInt32(_gridDefinID) == Convert.ToInt32(ds.Tables[0].Rows[t]["GridDefinitionID"]))
                    {
                        MessageBox.Show("Failed to load population dataset. Please check the dataset name, grid definition and population configuration."); return;
                    }

                }
                Application.DoEvents();
                lblprogbar.Text = "Saving Population...";
                lblprogbar.Refresh();
                commandText = "select max(POPULATIONDATASETID) from POPULATIONDATASETS";
                object dataSetID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                int fileCount = 0;
                //string tip = "Saving Population...";
                //WaitShow(tip);
                //commandText = string.Format("insert into PopulationDataSets values ({0},{1},'{2}',{3},{4})", dataSetID, CommonClass.ManageSetup.SetupID, txtDataSetName.Text, _popConfigID, _gridDefinID);
                //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

                string fileName = txtDataBase.Text;
                string dataFormat = fileName.Substring(fileName.LastIndexOf(".") + 1, (fileName.Length - fileName.LastIndexOf(".") - 1)).ToLower();
                string constring = string.Empty;
                //OleDbConnection oleCon = null;
                //OleDbDataReader myReader = null;

                int i = 0;
                string commandTextSave = "execute block as declare i int;" + "BEGIN  ";
                int iRow = -1;
                int iColumn = -1;
                int iYear = -1;
                int iPopulation = -1;
                int iRace = -1;
                int iEthnicity = -1;
                int iGender = -1;
                int iAgeRange = -1;
                //int j = 0;
                bool wrongAgeRange = false;
                //bool correctyear = true;
                int icount = 0;
                int iClose = 1;
                List<string> lstYyear = new List<string>();
                //如果是美国Case，应用了人口增长，先找出baseyear,然后判断如果不符合不允许导入
                int baseYear = -1;
                if (CommonClass.ManageSetup.SetupID == 1 && chkUseWoodsPoole.Checked)
                {
                    commandText = "select first 1 yyear from populationentries where populationdatasetid=30";
                    DataRow dr = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText).Tables[0].Rows[0];
                    baseYear = Convert.ToInt16(dr[0]);
                }

                if (dataFormat == "csv")
                {
                    //string strDir, strName;
                    //strDir = fileName.Substring(0, fileName.LastIndexOf(@"\") + 1);
                    //strName = fileName.Substring(fileName.LastIndexOf(@"\") + 1);
                    //constring = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}\\;Extended Properties=\"Text;HDR=Yes;FMT=Delimited;IMEX=0\"", strDir);
                    //oleCon = new OleDbConnection(constring);
                    //if (oleCon.State != ConnectionState.Closed) { oleCon.Close(); }
                    //oleCon.Open();
                    //OleDbCommand command = new OleDbCommand(string.Format("Select * from [{0}]", strName), oleCon);
                    //myReader = command.ExecuteReader();
                    //command = new OleDbCommand(string.Format("Select count(*) from [{0}]", strName), oleCon);
                    //fileCount = Convert.ToInt32(command.ExecuteScalar());

                    using (CsvReader csv = new CsvReader(new StreamReader(fileName), true))
                    {
                        fileCount = 0;
                        while (csv.ReadNextRecord())
                        {
                            fileCount++;
                        }
                        progBarLoadPop.Maximum = fileCount;
                    }
                    using (CsvReader csv = new CsvReader(new StreamReader(fileName), true))
                    {
                        //Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                        //Microsoft.Office.Interop.Excel.Workbook wb = null;
                        //Microsoft.Office.Interop.Excel.Worksheet ws = null;
                        //object missing = System.Reflection.Missing.Value;
                        //wb = excel.Workbooks.Open(fileName, missing, true, missing, missing, missing, missing, missing, missing, true, missing, missing, missing, missing, missing);
                        //ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets.get_Item(1);
                        ////取得总记录行数(包括标题列) 
                        //fileCount = ws.UsedRange.Cells.Rows.Count; //得到行数 
                        ////int columnsint = ws.UsedRange.Cells.Columns.Count;//得到列数 
                        //excel.Quit();
                        //progBarLoadPop.Maximum = fileCount;
                        ////PopulationTipForm frm = new PopulationTipForm(fileCount, 0, "Saving Population... ");
                        //frm.Show(this);

                        int fieldCount = csv.FieldCount;
                        string[] headers = csv.GetFieldHeaders();
                        foreach (string s in headers)
                        {
                            if (s.ToLower().Replace(" ", "") == "row") iRow = csv.GetFieldIndex(s);
                            if (s.ToLower().Replace(" ", "") == "column") iColumn = csv.GetFieldIndex(s);
                            if (s.ToLower().Replace(" ", "") == "year") iYear = csv.GetFieldIndex(s);
                            if (s.ToLower().Replace(" ", "") == "population") iPopulation = csv.GetFieldIndex(s);
                            if (s.ToLower().Replace(" ", "") == "race") iRace = csv.GetFieldIndex(s);
                            if (s.ToLower().Replace(" ", "") == "ethnicity") iEthnicity = csv.GetFieldIndex(s);
                            if (s.ToLower().Replace(" ", "") == "gender") iGender = csv.GetFieldIndex(s);
                            if (s.ToLower().Replace(" ", "") == "agerange") iAgeRange = csv.GetFieldIndex(s);
                        }

                        string warningtip = "";
                        if (iRow < 0) warningtip = "'Row', ";
                        if (iColumn < 0) warningtip += "'Column', ";
                        if (iYear < 0) warningtip += "'Year', ";
                        if (iPopulation < 0) warningtip += "'Population', ";
                        if (iRace < 0) warningtip += "'Race', ";
                        if (iEthnicity < 0) warningtip += "'Ethnicity', ";
                        if (iGender < 0) warningtip += "'Gender', ";
                        if (iAgeRange < 0) warningtip += "'AgeRange', ";
                        if (warningtip != "")
                        {
                            warningtip = warningtip.Substring(0, warningtip.Length - 2);
                            warningtip = "Please check the column header of " + warningtip + ". It is incorrect or does not exist.";
                            MessageBox.Show(warningtip, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            lblprogbar.Text = "";
                            return;
                        }

                        commandText = string.Format("insert into PopulationDataSets values ({0},{1},'{2}',{3},{4},{5})", dataSetID, CommonClass.ManageSetup.SetupID, txtDataSetName.Text, _popConfigID, _gridDefinID, chkUseWoodsPoole.Checked ? 1 : 0);
                        //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        fbCommand.CommandText = commandText;
                        fbCommand.ExecuteNonQuery();

                        while (csv.ReadNextRecord())
                        {
                            if (baseYear > 0 && baseYear != Convert.ToInt16(csv[iYear]))
                            {
                                MessageBox.Show("Population year needs to be the default base year (" + baseYear + ").");
                                //string delete = "delete from PopulationDataSets where populationdatasetid=" + dataSetID + "'";
                                //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, delete);
                                //correctyear = false;
                                fbtra.Rollback();
                                lblprogbar.Text = "";
                                progBarLoadPop.Value = 0;
                                //progBarLoadPop.Visible = false;
                                return;
                            }

                            if (icount == 200 * 200 * iClose)
                            {
                                GC.Collect();
                                iClose++;
                            }

                            //如果AgeRange不符合就不导入
                            if (GetValueFromAgeRangeID(csv[iAgeRange], dicAgeRange) == -1)
                            {
                                wrongAgeRange = true;
                                progBarLoadPop.PerformStep();
                                lblprogbar.Text = "Saving Population..." + Convert.ToString((int)((double)progBarLoadPop.Value / fileCount * 100)) + "%";
                                lblprogbar.Refresh();
                                continue;
                            }

                            if (!lstYyear.Contains(csv[iYear]))
                            {
                                lstYyear.Add(csv[iYear]);
                            }

                            try
                            {
                                if (i < 200)
                                {
                                    //-------------------加批量的语句 
                                    commandTextSave = commandTextSave + string.Format("insert into PopulationEntries (PopulationDataSetID,RaceID,GenderID,AgeRangeID,CColumn,Row,YYear,VValue,EthnicityID)  values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, GetValueFromRaceID(csv[iRace]), GetValueFromGenderID(csv[iGender]), GetValueFromAgeRangeID(csv[iAgeRange], dicAgeRange), csv[iColumn], csv[iRow], csv[iYear], csv[iPopulation], GetValueFromEthnicityID(csv[iEthnicity]));
                                    //commandTextSave = commandTextSave + s;
                                }
                                else
                                {
                                    //-------------加批量尾，提交---------------
                                    commandTextSave = commandTextSave + "  END";
                                    // fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandTextSave);
                                    //--------modify by xiejp 2011929 ----to 
                                    fbCommand.CommandText = commandTextSave;
                                    fbCommand.ExecuteNonQuery();
                                    i = 0;
                                    //-------------
                                    commandTextSave = "execute block as declare i int;" + " BEGIN   ";
                                    //-----------加批量头-----------------------
                                    //-----------加该行的insert语句-------------
                                    commandTextSave = commandTextSave + string.Format("insert into PopulationEntries (PopulationDataSetID,RaceID,GenderID,AgeRangeID,CColumn,Row,YYear,VValue,EthnicityID)  values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, GetValueFromRaceID(csv[iRace]), GetValueFromGenderID(csv[iGender]), GetValueFromAgeRangeID(csv[iAgeRange], dicAgeRange), csv[iColumn], csv[iRow], csv[iYear], csv[iPopulation], GetValueFromEthnicityID(csv[iEthnicity]));
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(ex);
                            }
                            i++;
                            progBarLoadPop.PerformStep();
                            lblprogbar.Text = "Saving Population..." + Convert.ToString((int)((double)progBarLoadPop.Value / fileCount * 100)) + "%";
                            lblprogbar.Refresh();
                            icount++;
                            //frm.setPos(icount);

                        }
                        //-----------------最后 剩的-------------------------
                        if (commandTextSave.LastIndexOf(" END") <= 0)
                        {
                            //-------------加批量尾，提交---------------
                            commandTextSave = commandTextSave + "  END";
                            //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandTextSave);
                            fbCommand.CommandText = commandTextSave;
                            fbCommand.ExecuteNonQuery();
                        }
                        foreach (string year in lstYyear)
                        {
                            commandText = string.Format("insert into t_PopulationDataSetIDYear(PopulationDataSetID,Yyear) values({0},{1})", dataSetID, year);
                            //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                            fbCommand.CommandText = commandText;
                            fbCommand.ExecuteNonQuery();
                        }

                        //myReader.Close();
                        //oleCon.Close();
                        //commandText = "delete from t_PopulationDataSetIDYear";
                        //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        //commandText = "insert into  t_PopulationDataSetIDYear select distinct PopulationDataSetID,Yyear from PopulationEntries ";
                        //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        //frm.Close();
                    }

                }
                else
                {
                    ////判断有没有安装Excel
                    //if (Type.GetTypeFromProgID("Excel.Application") == null)
                    //{
                    //    MessageBox.Show("Please install Excel.", "Warning", MessageBoxButtons.OK);
                    //    return;
                    //}
                    //Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                    //Microsoft.Office.Interop.Excel.Workbook wb = null;
                    //Microsoft.Office.Interop.Excel.Worksheet ws = null;
                    //object missing = System.Reflection.Missing.Value;
                    //wb = excel.Workbooks.Open(fileName, missing, true, missing, missing, missing, missing, missing, missing, true, missing, missing, missing, missing, missing);
                    //ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets.get_Item(1);
                    ////取得总记录行数(包括标题列) 
                    //fileCount = ws.UsedRange.Cells.Rows.Count; //得到行数
                    //progBarLoadPop.Maximum = fileCount;
                    //int columnsint = ws.UsedRange.Cells.Columns.Count;//得到列数
                    //for (int k = 1; k <= columnsint; k++)
                    //{
                    //    if (((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, k]).Text.ToString().ToLower().Replace(" ", "") == "row") { iRow = k; }
                    //    if (((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, k]).Text.ToString().ToLower().Replace(" ", "") == "column") { iColumn = k; }
                    //    if (((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, k]).Text.ToString().ToLower().Replace(" ", "") == "year") { iYear = k; }
                    //    if (((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, k]).Text.ToString().ToLower().Replace(" ", "") == "population") { iPopulation = k; }
                    //    if (((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, k]).Text.ToString().ToLower().Replace(" ", "") == "race") { iRace = k; }
                    //    if (((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, k]).Text.ToString().ToLower().Replace(" ", "") == "ethnicity") { iEthnicity = k; }
                    //    if (((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, k]).Text.ToString().ToLower().Replace(" ", "") == "gender") { iGender = k; }
                    //    if (((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, k]).Text.ToString().ToLower().Replace(" ", "") == "agerange") { iAgeRange = k; }
                    //}
                    //wb.Close(false, Type.Missing, Type.Missing);
                    //CommonClass.Kill(excel);
                    //excel.Quit();
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
                    //FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    //IExcelDataReader excelReader = fileName.ToLower().EndsWith("xls")
                    //                           ? ExcelReaderFactory.CreateBinaryReader(stream)
                    //                           : ExcelReaderFactory.CreateOpenXmlReader(stream);

                    //System.Reflection.FieldInfo fRow = excelReader.GetType().GetField("_workbook", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    //Excel.Core.OpenXmlFormat.XlsxWorkbook wb = null;
                    //wb = fRow.GetValue(excelReader) as Excel.Core.OpenXmlFormat.XlsxWorkbook;
                    //List<string> lstsheets = new List<string>();
                    //int isheet = excelReader.ResultsCount;
                    //for (int k = 0; k < isheet; k++)
                    //{
                    //    string name = excelReader.GetName(k);
                    //    lstsheets.Add(name);
                    //}

                    DataTable dtpop = CommonClass.ExcelToDataTable(fileName);
                    fileCount = dtpop.Rows.Count;
                    progBarLoadPop.Maximum = fileCount;
                    progBarLoadPop.Value = 0;
                    for (int k = 0; k < dtpop.Columns.Count; k++)
                    {
                        if (dtpop.Columns[k].ColumnName.ToLower().Replace(" ", "") == "row") { iRow = k; }
                        if (dtpop.Columns[k].ColumnName.ToLower().Replace(" ", "") == "column") { iColumn = k; }
                        if (dtpop.Columns[k].ColumnName.ToLower().Replace(" ", "") == "year") { iYear = k; }
                        if (dtpop.Columns[k].ColumnName.ToLower().Replace(" ", "") == "population") { iPopulation = k; }
                        if (dtpop.Columns[k].ColumnName.ToLower().Replace(" ", "") == "race") { iRace = k; }
                        if (dtpop.Columns[k].ColumnName.ToLower().Replace(" ", "") == "ethnicity") { iEthnicity = k; }
                        if (dtpop.Columns[k].ColumnName.ToLower().Replace(" ", "") == "gender") { iGender = k; }
                        if (dtpop.Columns[k].ColumnName.ToLower().Replace(" ", "") == "agerange") { iAgeRange = k; }
                    }

                    string warningtip = "";
                    if (iRow < 0) warningtip = "'Row', ";
                    if (iColumn < 0) warningtip += "'Column', ";
                    if (iYear < 0) warningtip += "'Year', ";
                    if (iPopulation < 0) warningtip += "'Population', ";
                    if (iRace < 0) warningtip += "'Race', ";
                    if (iEthnicity < 0) warningtip += "'Ethnicity', ";
                    if (iGender < 0) warningtip += "'Gender', ";
                    if (iAgeRange < 0) warningtip += "'AgeRange', ";
                    if (warningtip != "")
                    {
                        warningtip = warningtip.Substring(0, warningtip.Length - 2);
                        warningtip = "Please check the column header of " + warningtip + ". It is incorrect or does not exist.";
                        MessageBox.Show(warningtip, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        lblprogbar.Text = "";
                        return;
                    }

                    commandText = string.Format("insert into PopulationDataSets values ({0},{1},'{2}',{3},{4},{5})", dataSetID, CommonClass.ManageSetup.SetupID, txtDataSetName.Text, _popConfigID, _gridDefinID, chkUseWoodsPoole.Checked ? 1 : 0);
                    //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    fbCommand.CommandText = commandText;
                    fbCommand.ExecuteNonQuery();

                    #region datatable 比excelReader.Read()快
                    for (int k = 0; k < dtpop.Rows.Count; k++)
                    {
                        try
                        {
                            if (baseYear > 0 && baseYear != Convert.ToInt16(dtpop.Rows[k][iYear]))//excelReader.GetString(iYear - 1)))
                            {
                                MessageBox.Show("Population year needs to be the default base year (" + baseYear + ").");
                                //string delete = "delete from PopulationDataSets where populationdatasetid=" + dataSetID + "'";
                                //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, delete);
                                //correctyear = false;
                                fbtra.Rollback();
                                lblprogbar.Text = "";
                                progBarLoadPop.Value = 0;
                                //progBarLoadPop.Visible = false;
                                return;
                            }

                            //如果AgeRange不符合就不导入
                            if (GetValueFromAgeRangeID(dtpop.Rows[k][iAgeRange].ToString(), dicAgeRange) == -1)//excelReader.GetString(iAgeRange - 1), dicAgeRange) == -1)
                            {
                                wrongAgeRange = true;
                                progBarLoadPop.PerformStep();
                                lblprogbar.Text = "Saving Population..." + Convert.ToString((int)((double)progBarLoadPop.Value / fileCount * 100)) + "%";
                                lblprogbar.Refresh();
                                continue;
                            }

                            if (!lstYyear.Contains(dtpop.Rows[k][iYear].ToString()))//excelReader.GetString(iYear - 1)))
                            {
                                lstYyear.Add(dtpop.Rows[k][iYear].ToString());//excelReader.GetString(iYear - 1));
                            }

                            if (icount == 200 * 200 * iClose)
                            {
                                GC.Collect();
                                //Application.DoEvents();
                                //this.Refresh();
                                iClose++;
                            }

                            if (i < 200)
                            {
                                //-------------------加批量的语句 
                                commandTextSave = commandTextSave + string.Format("insert into PopulationEntries (PopulationDataSetID,RaceID,GenderID,AgeRangeID,CColumn,Row,YYear,VValue,EthnicityID)  values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, GetValueFromRaceID(dtpop.Rows[k][iRace].ToString()), GetValueFromGenderID(dtpop.Rows[k][iGender].ToString()), GetValueFromAgeRangeID(dtpop.Rows[k][iAgeRange].ToString(), dicAgeRange), Convert.ToInt16(dtpop.Rows[k][iColumn]), Convert.ToInt16(dtpop.Rows[k][iRow]), Convert.ToInt16(dtpop.Rows[k][iYear]), Convert.ToDouble(dtpop.Rows[k][iPopulation]), GetValueFromEthnicityID(dtpop.Rows[k][iEthnicity].ToString()));//GetValueFromRaceID(excelReader.GetString(iRace - 1)), GetValueFromGenderID(excelReader.GetString(iGender - 1)), GetValueFromAgeRangeID(excelReader.GetString(iAgeRange - 1), dicAgeRange), excelReader.GetInt16(iColumn - 1), excelReader.GetInt16(iRow - 1), excelReader.GetInt16(iYear - 1), excelReader.GetDouble(iPopulation - 1), GetValueFromEthnicityID(excelReader.GetString(iEthnicity - 1)));
                            }
                            else
                            {
                                //-------------加批量尾，提交---------------
                                commandTextSave = commandTextSave + "  END";
                                fbCommand.CommandText = commandTextSave;
                                fbCommand.ExecuteNonQuery();
                                i = 0;
                                //-------------
                                commandTextSave = "execute block as declare i int;" + " BEGIN   ";
                                //-----------加批量头-----------------------
                                //-----------加该行的insert语句-------------
                                commandTextSave = commandTextSave + string.Format("insert into PopulationEntries (PopulationDataSetID,RaceID,GenderID,AgeRangeID,CColumn,Row,YYear,VValue,EthnicityID)  values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, GetValueFromRaceID(dtpop.Rows[k][iRace].ToString()), GetValueFromGenderID(dtpop.Rows[k][iGender].ToString()), GetValueFromAgeRangeID(dtpop.Rows[k][iAgeRange].ToString(), dicAgeRange), Convert.ToInt16(dtpop.Rows[k][iColumn]), Convert.ToInt16(dtpop.Rows[k][iRow]), Convert.ToInt16(dtpop.Rows[k][iYear]), Convert.ToDouble(dtpop.Rows[k][iPopulation]), GetValueFromEthnicityID(dtpop.Rows[k][iEthnicity].ToString()));
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex);
                        }
                        i++;
                        progBarLoadPop.PerformStep();
                        lblprogbar.Text = "Saving Population..." + Convert.ToString((int)((double)progBarLoadPop.Value / fileCount * 100)) + "%";
                        lblprogbar.Refresh();
                        icount++;
                    }
                    dtpop.Dispose();
                    #endregion

                    #region exceldatareader
                    //excelReader.Read();
                    //while (excelReader.Read())
                    //{
                    //    try
                    //    {
                    //        if (baseYear > 0 && baseYear != Convert.ToInt16(excelReader.GetString(iYear)))
                    //        {
                    //            MessageBox.Show("Population year needs to be the default base year (" + baseYear + ").");
                    //            //string delete = "delete from PopulationDataSets where populationdatasetid=" + dataSetID + "'";
                    //            //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, delete);
                    //            //correctyear = false;
                    //            fbtra.Rollback();
                    //            return;
                    //        }

                    //        //如果AgeRange不符合就不导入
                    //        if (GetValueFromAgeRangeID(excelReader.GetString(iAgeRange), dicAgeRange) == -1)
                    //        {
                    //            wrongAgeRange = true;
                    //            progBarLoadPop.PerformStep();
                    //            lblprogbar.Text = "Saving Population..." + Convert.ToString((int)((double)progBarLoadPop.Value / fileCount * 100)) + "%";
                    //            lblprogbar.Refresh();
                    //            continue;
                    //        }

                    //        if (!lstYyear.Contains(excelReader.GetString(iYear)))
                    //        {
                    //            lstYyear.Add(excelReader.GetString(iYear));
                    //        }

                    //        if (icount == 200 * 200 * iClose)
                    //        {
                    //            GC.Collect();
                    //            //Application.DoEvents();
                    //            //this.Refresh();
                    //            iClose++;
                    //        }

                    //        if (i < 200)
                    //        {
                    //            //-------------------加批量的语句 
                    //            commandTextSave = commandTextSave + string.Format("insert into PopulationEntries (PopulationDataSetID,RaceID,GenderID,AgeRangeID,CColumn,Row,YYear,VValue,EthnicityID)  values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, GetValueFromRaceID(excelReader.GetString(iRace)), GetValueFromGenderID(excelReader.GetString(iGender)), GetValueFromAgeRangeID(excelReader.GetString(iAgeRange), dicAgeRange), excelReader.GetInt16(iColumn), excelReader.GetInt16(iRow), excelReader.GetInt16(iYear), excelReader.GetDouble(iPopulation), GetValueFromEthnicityID(excelReader.GetString(iEthnicity)));
                    //        }
                    //        else
                    //        {
                    //            //-------------加批量尾，提交---------------
                    //            commandTextSave = commandTextSave + "  END";
                    //            fbCommand.CommandText = commandTextSave;
                    //            fbCommand.ExecuteNonQuery();
                    //            i = 0;
                    //            //-------------
                    //            commandTextSave = "execute block as declare i int;" + " BEGIN   ";
                    //            //-----------加批量头-----------------------
                    //            //-----------加该行的insert语句-------------
                    //            commandTextSave = commandTextSave + string.Format("insert into PopulationEntries (PopulationDataSetID,RaceID,GenderID,AgeRangeID,CColumn,Row,YYear,VValue,EthnicityID)  values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, GetValueFromRaceID(excelReader.GetString(iRace)), GetValueFromGenderID(excelReader.GetString(iGender)), GetValueFromAgeRangeID(excelReader.GetString(iAgeRange), dicAgeRange), excelReader.GetInt16(iColumn), excelReader.GetInt16(iRow), excelReader.GetInt16(iYear), excelReader.GetDouble(iPopulation), GetValueFromEthnicityID(excelReader.GetString(iEthnicity)));
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Logger.LogError(ex);
                    //    }
                    //    i++;
                    //    //Application.DoEvents();
                    //    progBarLoadPop.PerformStep();
                    //    //Application.DoEvents();
                    //    lblprogbar.Text = "Saving Population..." + Convert.ToString((int)((double)progBarLoadPop.Value / fileCount * 100)) + "%";
                    //    //Application.DoEvents();
                    //    lblprogbar.Refresh();
                    //    icount++;
                    //}
                    #endregion
                    //for (int k = 2; k <= fileCount; k++)
                    //{
                    //    try
                    //    {
                    //        if (!lstYyear.Contains(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iYear]).Text.ToString()))
                    //        {
                    //            lstYyear.Add(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iYear]).Text.ToString());
                    //        }

                    //        if (i < 200)
                    //        {
                    //            //-------------------加批量的语句 
                    //            commandTextSave = commandTextSave + string.Format("insert into PopulationEntries (PopulationDataSetID,RaceID,GenderID,AgeRangeID,CColumn,Row,YYear,VValue,EthnicityID)  values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, GetValueFromRaceID(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iRace]).Text.ToString(), dicRace), GetValueFromGenderID(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iGender]).Text.ToString(), dicGender), GetValueFromAgeRangeID(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iAgeRange]).Text.ToString(), dicAgeRange), Convert.ToInt16(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iColumn]).Text.ToString()), Convert.ToInt16(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iRow]).Text.ToString()), Convert.ToInt16(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iYear]).Text.ToString()), Convert.ToDouble(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iPopulation]).Text.ToString()), GetValueFromEthnicityID(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iEthnicity]).Text.ToString(), dicEthnicity));
                    //            //commandTextSave = commandTextSave + s;
                    //        }
                    //        else
                    //        {
                    //            //-------------加批量尾，提交---------------
                    //            commandTextSave = commandTextSave + "  END";
                    //            // fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandTextSave);
                    //            //--------modify by xiejp 2011929 ----to 
                    //            fbCommand.CommandText = commandTextSave;
                    //            fbCommand.ExecuteNonQuery();
                    //            i = 0;
                    //            //-------------
                    //            commandTextSave = "execute block as declare i int;" + " BEGIN   ";
                    //            //-----------加批量头-----------------------
                    //            //-----------加该行的insert语句-------------
                    //            commandTextSave = commandTextSave + string.Format("insert into PopulationEntries (PopulationDataSetID,RaceID,GenderID,AgeRangeID,CColumn,Row,YYear,VValue,EthnicityID)  values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, GetValueFromRaceID(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iRace]).Text.ToString(), dicRace), GetValueFromGenderID(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iGender]).Text.ToString(), dicGender), GetValueFromAgeRangeID(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iAgeRange]).Text.ToString(), dicAgeRange), Convert.ToInt16(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iColumn]).Text.ToString()), Convert.ToInt16(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iRow]).Text.ToString()), Convert.ToInt16(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iYear]).Text.ToString()), Convert.ToDouble(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iPopulation]).Text.ToString()), GetValueFromEthnicityID(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iEthnicity]).Text.ToString(), dicEthnicity));
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Logger.LogError(ex);
                    //    }
                    //    i++;
                    //    progBarLoadPop.Value++;
                    //    lblprogbar.Text = "Saving Population..." + Convert.ToString((int)((double)progBarLoadPop.Value / fileCount * 100)) + "%";
                    //    lblprogbar.Refresh();
                    //}
                    //-----------------最后 剩的-------------------------
                    if (commandTextSave.LastIndexOf(" END") <= 0)
                    {
                        //-------------加批量尾，提交---------------
                        commandTextSave = commandTextSave + "  END";
                        //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandTextSave);
                        fbCommand.CommandText = commandTextSave;
                        fbCommand.ExecuteNonQuery();
                    }
                    foreach (string year in lstYyear)
                    {
                        commandText = string.Format("insert into t_PopulationDataSetIDYear(PopulationDataSetID,Yyear) values({0},{1})", dataSetID, year);
                        //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        fbCommand.CommandText = commandText;
                        fbCommand.ExecuteNonQuery();
                    }
                    #region
                    //myReader.Close();
                    //oleCon.Close();
                    //commandText = "delete from t_PopulationDataSetIDYear";
                    //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    //commandText = "insert into  t_PopulationDataSetIDYear select distinct PopulationDataSetID,Yyear from PopulationEntries ";
                    //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

                    //constring = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=YES\"", fileName); ;
                    //oleCon = new OleDbConnection(constring);
                    //if (oleCon.State != ConnectionState.Closed) { oleCon.Close(); }
                    //oleCon.Open();
                    //返回Excel的架构，包括各个sheet表的名称,类型，创建时间和修改时间等
                    //DataTable dtSheetName = oleCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });
                    //string[] tableNames = new string[dtSheetName.Rows.Count];
                    //for (int k = 0; k < dtSheetName.Rows.Count; k++)
                    //{
                    //    tableNames[k] = dtSheetName.Rows[k]["TABLE_NAME"].ToString();
                    //}
                    //string tableName = "";
                    //OleDbCommand command = null;;
                    //for (int k = 0; k < tableNames.Length; k++)
                    //{
                    //    tableName = tableNames[k];
                    //    if (tableName.Contains("_")) { if (tableName.LastIndexOf('_') == tableName.Length - 1) { break; } }
                    //    string strSql = "select * from [" + tableName + "]";
                    //    command = new OleDbCommand(strSql, oleCon);
                    //    myReader = command.ExecuteReader();
                    //    strSql = "select count(*) from [" + tableName + "]";
                    //    command = new OleDbCommand(strSql, oleCon);
                    //    fileCount = Convert.ToInt32(command.ExecuteScalar());
                    //    if (myReader != null)
                    //    {
                    //        break;
                    //    }
                    //}
                    #endregion
                }
                #region
                //WaitClose();
                //PopulationTipForm frm = new PopulationTipForm(fileCount,0,"Saving Population... ");
                //frm.Show(this);
                //int i = 0;
                //string commandTextSave = "execute block as declare i int;" + "BEGIN  ";
                //--------------加批量头
                //.................
                //string s = "";
                //int iRow = -1;
                //int iColumn = -1;
                //int iYear = -1;
                //int iPopulation = -1;
                //int iRace = -1;
                //int iEthnicity = -1;
                //int iGender = -1;
                //int iAgeRange = -1;
                //int j = 0;
                //int icount = 0;
                //int iClose = 1;
                //FirebirdSql.Data.FirebirdClient.FbCommand fbCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
                //fbCommand.Connection = CommonClass.Connection;
                //fbCommand.CommandType = CommandType.Text;
                //if (CommonClass.Connection.State != ConnectionState.Open)
                //{ CommonClass.Connection.Open(); }
                //while (myReader.Read())
                //{                        
                //if (j == 0)
                //{
                //    for (int k = 0; k < myReader.FieldCount; k++)
                //    {
                //        if (myReader.GetName(k).ToLower().Replace(" ", "") == "row") { iRow = k; }
                //        if (myReader.GetName(k).ToLower().Replace(" ", "") == "column") { iColumn = k; }
                //        if (myReader.GetName(k).ToLower().Replace(" ", "") == "year") { iYear = k; }
                //        if (myReader.GetName(k).ToLower().Replace(" ", "") == "population") { iPopulation = k; }
                //        if (myReader.GetName(k).ToLower().Replace(" ", "") == "race") { iRace = k; }
                //        if (myReader.GetName(k).ToLower().Replace(" ", "") == "ethnicity") { iEthnicity = k; }
                //        if (myReader.GetName(k).ToLower().Replace(" ", "") == "gender") { iGender = k; }
                //        if (myReader.GetName(k).ToLower().Replace(" ", "") == "agerange") { iAgeRange = k; }
                //    }
                //    j++;
                //}
                //if (icount == 200 * 200 * iClose)
                //{
                //    //CommonClass.Connection.Close();
                //    //fbCommand.Dispose();
                //    // CommonClass.Connection.Dispose();

                //    GC.Collect();
                //    //fbCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
                //    //fbCommand.Connection = CommonClass.Connection;
                //    //fbCommand.CommandType = CommandType.Text;
                //    //CommonClass.Connection.Open();
                //    iClose++;
                //}
                //try
                //{
                //    if (i < 200)
                //    {
                //        //-------------------加批量的语句 
                //        commandTextSave = commandTextSave + string.Format("insert into PopulationEntries (PopulationDataSetID,RaceID,GenderID,AgeRangeID,CColumn,Row,YYear,VValue,EthnicityID)  values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, GetValueFromRaceID(myReader[iRace].ToString(), dicRace), GetValueFromGenderID(myReader[iGender].ToString(), dicGender), GetValueFromAgeRangeID(myReader[iAgeRange].ToString(), dicAgeRange), myReader[iColumn], myReader[iRow], myReader[iYear], myReader[iPopulation], GetValueFromEthnicityID(myReader[iEthnicity].ToString(), dicEthnicity));
                //        //commandTextSave = commandTextSave + s;
                //    }
                //    else
                //    {
                //        //-------------加批量尾，提交---------------
                //        commandTextSave = commandTextSave + "  END";
                //        // fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandTextSave);
                //        //--------modify by xiejp 2011929 ----to 
                //        fbCommand.CommandText = commandTextSave;
                //        fbCommand.ExecuteNonQuery();
                //        i = 0;
                //        //-------------
                //        commandTextSave = "execute block as declare i int;" + " BEGIN   ";
                //        //-----------加批量头-----------------------
                //        //-----------加该行的insert语句-------------
                //        commandTextSave = commandTextSave + string.Format("insert into PopulationEntries (PopulationDataSetID,RaceID,GenderID,AgeRangeID,CColumn,Row,YYear,VValue,EthnicityID)  values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, GetValueFromRaceID(myReader[iRace].ToString(), dicRace), GetValueFromGenderID(myReader[iGender].ToString(), dicGender), GetValueFromAgeRangeID(myReader[iAgeRange].ToString(), dicAgeRange), myReader[iColumn], myReader[iRow], myReader[iYear], myReader[iPopulation], GetValueFromEthnicityID(myReader[iEthnicity].ToString(), dicEthnicity));
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Logger.LogError(ex);
                //}
                //i++;
                //icount++;
                //frm.setPos(icount);
                //Thread.Sleep(100);                        
                //}
                ////-----------------最后 剩的-------------------------
                //if (commandTextSave.LastIndexOf(" END") <= 0)
                //{
                //    //-------------加批量尾，提交---------------
                //    commandTextSave = commandTextSave + "  END";
                //    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandTextSave);
                //}
                //myReader.Close();
                //oleCon.Close();
                //commandText = "delete from t_PopulationDataSetIDYear";
                //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                //commandText = "insert into  t_PopulationDataSetIDYear select distinct PopulationDataSetID,Yyear from PopulationEntries ";
                //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                //frm.Close();
                #endregion
                bool containYear = true;
                if (txtGrowthWeights.Text != string.Empty)
                {
                    //string tip = "Saving Populaiton Growth Weights...";
                    //WaitShow(tip);
                    lblprogbar.Text = "Saving Populaiton Growth Weights...";
                    progBarLoadPop.Value = 0;
                    lblprogbar.Refresh();
                    fileName = txtGrowthWeights.Text;
                    dataFormat = fileName.Substring(fileName.LastIndexOf(".") + 1, (fileName.Length - fileName.LastIndexOf(".") - 1));
                    constring = string.Empty;
                    //OleDbConnection oleCon = null;
                    //OleDbDataReader myReader = null;
                    i = 0;
                    commandTextSave = "execute block as declare i int;" + "BEGIN  ";
                    int iSourceCol = -1;
                    int iSourceRow = -1;
                    int iTargetCol = -1;
                    int iTargetRow = -1;
                    iRace = -1;
                    iEthnicity = -1;
                    int iValue = -1;
                    iYear = -1;
                    icount = 0;
                    iClose = 1;
                    //j = 0;
                    //FirebirdSql.Data.FirebirdClient.FbCommand fbCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
                    //fbCommand.Connection = CommonClass.Connection;
                    //fbCommand.CommandType = CommandType.Text;
                    //if (CommonClass.Connection.State != ConnectionState.Open)
                    //{ CommonClass.Connection.Open(); }

                    if (dataFormat == "csv")
                    {
                        //string strDir, strName;
                        //strDir = fileName.Substring(0, fileName.LastIndexOf(@"\") + 1);
                        //strName = fileName.Substring(fileName.LastIndexOf(@"\") + 1);
                        //constring = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}\\;Extended Properties=\"Text;HDR=Yes;FMT=Delimited;IMEX=0\"", strDir);
                        //oleCon = new OleDbConnection(constring);
                        //if (oleCon.State != ConnectionState.Closed) { oleCon.Close(); }
                        //oleCon.Open();
                        //OleDbCommand command = new OleDbCommand(string.Format("Select * from [{0}]", strName), oleCon);
                        //myReader = command.ExecuteReader();
                        //command = new OleDbCommand(string.Format("Select count(*) from [{0}]", strName), oleCon);
                        //fileCount = Convert.ToInt32(command.ExecuteScalar());
                        using (CsvReader csv = new CsvReader(new StreamReader(fileName), true))
                        {
                            fileCount = 0;
                            while (csv.ReadNextRecord())
                            {
                                fileCount++;
                            }
                            progBarLoadPop.Maximum = fileCount;
                        }
                        using (CsvReader csv = new CsvReader(new StreamReader(fileName), true))
                        {
                            //Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                            //Microsoft.Office.Interop.Excel.Workbook wb = null;
                            //Microsoft.Office.Interop.Excel.Worksheet ws = null;
                            //object missing = System.Reflection.Missing.Value;
                            //wb = excel.Workbooks.Open(fileName, missing, true, missing, missing, missing, missing, missing, missing, true, missing, missing, missing, missing, missing);
                            //ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets.get_Item(1);
                            ////取得总记录行数(包括标题列) 
                            //fileCount = ws.UsedRange.Cells.Rows.Count; //得到行数 
                            ////int columnsint = ws.UsedRange.Cells.Columns.Count;//得到列数 
                            //excel.Quit();
                            progBarLoadPop.Maximum = fileCount;
                            //PopulationTipForm frm = new PopulationTipForm(fileCount, 0, "Saving Population... ");
                            //frm.Show(this);

                            int fieldCount = csv.FieldCount;
                            string[] headers = csv.GetFieldHeaders();
                            foreach (string s in headers)
                            {
                                if (s.ToLower().Replace(" ", "") == "year") iYear = csv.GetFieldIndex(s);
                                if (s.ToLower().Replace(" ", "").Contains("sourcecol")) iSourceCol = csv.GetFieldIndex(s);
                                if (s.ToLower().Replace(" ", "") == "sourcerow") iSourceRow = csv.GetFieldIndex(s);
                                if (s.ToLower().Replace(" ", "").Contains("targetcol")) iTargetCol = csv.GetFieldIndex(s);
                                if (s.ToLower().Replace(" ", "") == "targetrow") iTargetRow = csv.GetFieldIndex(s);
                                if (s.ToLower().Replace(" ", "") == "race") iRace = csv.GetFieldIndex(s);
                                if (s.ToLower().Replace(" ", "") == "ethnicity") iEthnicity = csv.GetFieldIndex(s);
                                if (s.ToLower().Replace(" ", "") == "value") iValue = csv.GetFieldIndex(s);
                            }

                            string warningtip = "";
                            if (iYear < 0) warningtip = "'Year', ";
                            if (iSourceCol < 0) warningtip += "'SourceCol', ";
                            if (iSourceRow < 0) warningtip += "'SourceRow', ";
                            if (iTargetCol < 0) warningtip += "'TargetCol', ";
                            if (iTargetRow < 0) warningtip += "'TargetRow', ";
                            if (iRace < 0) warningtip += "'Race', ";
                            if (iEthnicity < 0) warningtip += "'Ethnicity', ";
                            if (iValue < 0) warningtip += "'Value', ";
                            if (warningtip != "")
                            {
                                warningtip = warningtip.Substring(0, warningtip.Length - 2);
                                warningtip = "Please check the column header of " + warningtip + " in Population Growth Weights file. It is incorrect or does not exist.";
                                MessageBox.Show(warningtip, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                fbtra.Rollback();
                                lblprogbar.Text = "";
                                progBarLoadPop.Value = 0;
                                return;
                            }

                            while (csv.ReadNextRecord())
                            {
                                try
                                {
                                    if (!lstYyear.Contains(csv[iYear].ToString().Trim()))
                                    {
                                        containYear = false;
                                        progBarLoadPop.PerformStep();
                                        lblprogbar.Text = "Saving Populaiton Growth Weights..." + Convert.ToString((int)((double)progBarLoadPop.Value / fileCount * 100)) + "%";
                                        lblprogbar.Refresh();
                                        continue;
                                    }

                                    if (icount == 200 * 200 * iClose)
                                    {
                                        GC.Collect();
                                        iClose++;
                                    }

                                    if (i < 200)
                                    {
                                        //-------------------加批量的语句 
                                        commandTextSave = commandTextSave + string.Format("insert into PopulationGrowthWeights (PopulationDataSetID,Yyear,SourceColumn,SourceRow,TargetColumn,TargetRow,RaceID,EthnicityID,Vvalue ) values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, csv[iYear], csv[iSourceCol], csv[iSourceRow], csv[iTargetCol], csv[iTargetRow], GetValueFromRaceID(csv[iRace].ToString()), GetValueFromEthnicityID(csv[iEthnicity].ToString()), csv[iValue]);
                                        //commandTextSave = commandTextSave + s;
                                    }
                                    else
                                    {
                                        //-------------加批量尾，提交---------------
                                        commandTextSave = commandTextSave + "  END";
                                        // fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandTextSave);
                                        //--------modify by xiejp 2011929 ----to 
                                        fbCommand.CommandText = commandTextSave;
                                        fbCommand.ExecuteNonQuery();
                                        i = 0;
                                        //-------------
                                        commandTextSave = "execute block as declare i int;" + " BEGIN   ";
                                        //-----------加批量头-----------------------
                                        //-----------加该行的insert语句-------------
                                        commandTextSave = commandTextSave + string.Format("insert into PopulationGrowthWeights (PopulationDataSetID,Yyear,SourceColumn,SourceRow,TargetColumn,TargetRow,RaceID,EthnicityID,Vvalue ) values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, csv[iYear], csv[iSourceCol], csv[iSourceRow], csv[iTargetCol], csv[iTargetRow], GetValueFromRaceID(csv[iRace].ToString()), GetValueFromEthnicityID(csv[iEthnicity].ToString()), csv[iValue]);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError(ex);
                                }
                                i++;
                                progBarLoadPop.PerformStep();
                                lblprogbar.Text = "Saving Populaiton Growth Weights..." + Convert.ToString((int)((double)progBarLoadPop.Value / fileCount * 100)) + "%";
                                lblprogbar.Refresh();
                                icount++;
                                //frm.setPos(icount);

                            }
                            //-----------------最后 剩的-------------------------
                            if (commandTextSave.LastIndexOf(" END") <= 0)
                            {
                                //-------------加批量尾，提交---------------
                                commandTextSave = commandTextSave + "  END";
                                fbCommand.CommandText = commandTextSave;
                                fbCommand.ExecuteNonQuery();
                                //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandTextSave);
                            }
                        }

                    }
                    else
                    {
                        #region
                        //constring = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=YES\"", fileName); ;
                        //oleCon = new OleDbConnection(constring);
                        //if (oleCon.State != ConnectionState.Closed) { oleCon.Close(); }
                        //oleCon.Open();
                        ////返回Excel的架构，包括各个sheet表的名称,类型，创建时间和修改时间等
                        //DataTable dtSheetName = oleCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });
                        //string[] tableNames = new string[dtSheetName.Rows.Count];
                        //for (int k = 0; k < dtSheetName.Rows.Count; k++)
                        //{
                        //    tableNames[k] = dtSheetName.Rows[k]["TABLE_NAME"].ToString();
                        //}
                        //string tableName = "";
                        //OleDbCommand command = null; ;
                        //for (int k = 0; k < tableNames.Length; k++)
                        //{
                        //    tableName = tableNames[k];
                        //    if (tableName.Contains("_")) { if (tableName.LastIndexOf('_') == tableName.Length - 1) { break; } }
                        //    string strSql = "select * from [" + tableName + "]";
                        //    command = new OleDbCommand(strSql, oleCon);
                        //    myReader = command.ExecuteReader();
                        //    strSql = "select count(*) from [" + tableName + "]";
                        //    command = new OleDbCommand(strSql, oleCon);
                        //    fileCount = Convert.ToInt32(command.ExecuteScalar());
                        //    if (myReader != null)
                        //    {
                        //        break;
                        //    }

                        //}
                        #endregion

                        //Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                        //Microsoft.Office.Interop.Excel.Workbook wb = null;
                        //Microsoft.Office.Interop.Excel.Worksheet ws = null;
                        //object missing = System.Reflection.Missing.Value;
                        //wb = excel.Workbooks.Open(fileName, missing, true, missing, missing, missing, missing, missing, missing, true, missing, missing, missing, missing, missing);
                        //ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets.get_Item(1);
                        ////取得总记录行数(包括标题列) 
                        //fileCount = ws.UsedRange.Cells.Rows.Count; //得到行数
                        //progBarLoadPop.Maximum = fileCount;
                        //int columnsint = ws.UsedRange.Cells.Columns.Count;//得到列数
                        //for (int k = 1; k <= columnsint; k++)
                        //{
                        //    if (((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, k]).Text.ToString().ToLower().Replace(" ", "") == "year") { iYear = k; }
                        //    if (((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, k]).Text.ToString().ToLower().Replace(" ", "") == "sourcecol") { iSourceCol = k; }
                        //    if (((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, k]).Text.ToString().ToLower().Replace(" ", "") == "sourcerow") { iSourceRow = k; }
                        //    if (((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, k]).Text.ToString().ToLower().Replace(" ", "") == "targetcol") { iTargetCol = k; }
                        //    if (((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, k]).Text.ToString().ToLower().Replace(" ", "") == "targetrow") { iTargetRow = k; }
                        //    if (((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, k]).Text.ToString().ToLower().Replace(" ", "") == "race") { iRace = k; }
                        //    if (((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, k]).Text.ToString().ToLower().Replace(" ", "") == "ethnicity") { iEthnicity = k; }
                        //    if (((Microsoft.Office.Interop.Excel.Range)ws.Cells[1, k]).Text.ToString().ToLower().Replace(" ", "") == "value") { iValue = k; }
                        //}
                        DataTable dtpopWeight = CommonClass.ExcelToDataTable(fileName);
                        fileCount = dtpopWeight.Rows.Count;
                        progBarLoadPop.Maximum = fileCount;
                        progBarLoadPop.Value = 0;
                        for (int k = 0; k < dtpopWeight.Columns.Count; k++)
                        {
                            if (dtpopWeight.Columns[k].ColumnName.ToLower().Replace(" ", "") == "year") { iYear = k; }
                            if (dtpopWeight.Columns[k].ColumnName.ToLower().Replace(" ", "") == "sourcecol") { iSourceCol = k; }
                            if (dtpopWeight.Columns[k].ColumnName.ToLower().Replace(" ", "") == "sourcerow") { iSourceRow = k; }
                            if (dtpopWeight.Columns[k].ColumnName.ToLower().Replace(" ", "") == "targetcol") { iTargetCol = k; }
                            if (dtpopWeight.Columns[k].ColumnName.ToLower().Replace(" ", "") == "targetrow") { iTargetRow = k; }
                            if (dtpopWeight.Columns[k].ColumnName.ToLower().Replace(" ", "") == "race") { iRace = k; }
                            if (dtpopWeight.Columns[k].ColumnName.ToLower().Replace(" ", "") == "ethnicity") { iEthnicity = k; }
                            if (dtpopWeight.Columns[k].ColumnName.ToLower().Replace(" ", "") == "value") { iValue = k; }
                        }

                        string warningtip = "";
                        if (iYear < 0) warningtip = "'Year', ";
                        if (iSourceCol < 0) warningtip += "'SourceCol', ";
                        if (iSourceRow < 0) warningtip += "'SourceRow', ";
                        if (iTargetCol < 0) warningtip += "'TargetCol', ";
                        if (iTargetRow < 0) warningtip += "'TargetRow', ";
                        if (iRace < 0) warningtip += "'Race', ";
                        if (iEthnicity < 0) warningtip += "'Ethnicity', ";
                        if (iValue < 0) warningtip += "'Value', ";
                        if (warningtip != "")
                        {
                            warningtip = warningtip.Substring(0, warningtip.Length - 2);
                            warningtip = "Please check the column header of " + warningtip + " in Population Growth Weights file. It is incorrect or does not exist.";
                            MessageBox.Show(warningtip, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            fbtra.Rollback();
                            lblprogbar.Text = "";
                            progBarLoadPop.Value = 0;
                            return;
                        }

                        for (int k = 0; k <= fileCount; k++)
                        {
                            try
                            {
                                if (!lstYyear.Contains(dtpopWeight.Rows[k][iYear].ToString().Trim()))
                                {
                                    containYear = false;
                                    progBarLoadPop.PerformStep();
                                    lblprogbar.Text = "Saving Populaiton Growth Weights..." + Convert.ToString((int)((double)progBarLoadPop.Value / fileCount * 100)) + "%";
                                    lblprogbar.Refresh();
                                    continue;
                                }

                                if (icount == 200 * 200 * iClose)
                                {
                                    GC.Collect();
                                    iClose++;
                                }

                                if (i < 200)
                                {
                                    //-------------------加批量的语句 
                                    commandTextSave = commandTextSave + string.Format("insert into PopulationGrowthWeights (PopulationDataSetID,Yyear,SourceColumn,SourceRow,TargetColumn,TargetRow,RaceID,EthnicityID,Vvalue ) values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, Convert.ToInt16(dtpopWeight.Rows[k][iYear]), Convert.ToInt16(dtpopWeight.Rows[k][iSourceCol]), Convert.ToInt16(dtpopWeight.Rows[k][iSourceRow]), Convert.ToInt16(dtpopWeight.Rows[k][iTargetCol]), Convert.ToInt16(dtpopWeight.Rows[k][iTargetRow]), GetValueFromRaceID(dtpopWeight.Rows[k][iRace].ToString()), GetValueFromEthnicityID(dtpopWeight.Rows[k][iEthnicity].ToString()), Convert.ToDouble(dtpopWeight.Rows[k][iValue]));
                                    //commandTextSave = commandTextSave + s;
                                }
                                else
                                {
                                    //-------------加批量尾，提交---------------
                                    commandTextSave = commandTextSave + "  END";
                                    // fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandTextSave);
                                    //--------modify by xiejp 2011929 ----to 
                                    fbCommand.CommandText = commandTextSave;
                                    fbCommand.ExecuteNonQuery();
                                    i = 0;
                                    //-------------
                                    commandTextSave = "execute block as declare i int;" + " BEGIN   ";
                                    //-----------加批量头-----------------------
                                    //-----------加该行的insert语句-------------
                                    commandTextSave = commandTextSave + string.Format("insert into PopulationGrowthWeights (PopulationDataSetID,Yyear,SourceColumn,SourceRow,TargetColumn,TargetRow,RaceID,EthnicityID,Vvalue ) values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, Convert.ToInt16(dtpopWeight.Rows[k][iYear]), Convert.ToInt16(dtpopWeight.Rows[k][iSourceCol]), Convert.ToInt16(dtpopWeight.Rows[k][iSourceRow]), Convert.ToInt16(dtpopWeight.Rows[k][iTargetCol]), Convert.ToInt16(dtpopWeight.Rows[k][iTargetRow]), GetValueFromRaceID(dtpopWeight.Rows[k][iRace].ToString()), GetValueFromEthnicityID(dtpopWeight.Rows[k][iEthnicity].ToString()), Convert.ToDouble(dtpopWeight.Rows[k][iValue]));
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(ex);
                            }
                            i++;
                            progBarLoadPop.PerformStep();
                            lblprogbar.Text = "Saving Populaiton Growth Weights..." + Convert.ToString((int)((double)progBarLoadPop.Value / fileCount * 100)) + "%";
                            lblprogbar.Refresh();
                            icount++;
                        }
                        //-----------------最后 剩的-------------------------
                        if (commandTextSave.LastIndexOf(" END") <= 0)
                        {
                            //-------------加批量尾，提交---------------
                            commandTextSave = commandTextSave + "  END";
                            fbCommand.CommandText = commandTextSave;
                            fbCommand.ExecuteNonQuery();
                            //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandTextSave);
                        }

                        #region MicrosoftExcel
                        //for (int k = 2; k <= fileCount; k++)
                        //{
                        //    try
                        //    {
                        //        if (icount == 200 * 200 * iClose)
                        //        {
                        //            GC.Collect();
                        //            iClose++;
                        //        }

                        //        if (i < 200)
                        //        {
                        //            //-------------------加批量的语句 
                        //            commandTextSave = commandTextSave + string.Format("insert into PopulationGrowthWeights (PopulationDataSetID,Yyear,SourceColumn,SourceRow,TargetColumn,TargetRow,RaceID,EthnicityID,Vvalue ) values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, ((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iYear]).Text.ToString(), Convert.ToInt16(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iSourceCol]).Text.ToString()), Convert.ToInt16(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iSourceRow]).Text.ToString()), Convert.ToInt16(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iTargetCol]).Text.ToString()), Convert.ToInt16(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iTargetRow]).Text.ToString()), GetValueFromRaceID(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iRace]).Text.ToString()), GetValueFromEthnicityID(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iEthnicity]).Text.ToString()), Convert.ToDouble(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iValue]).Text.ToString()));
                        //            //commandTextSave = commandTextSave + s;
                        //        }
                        //        else
                        //        {
                        //            //-------------加批量尾，提交---------------
                        //            commandTextSave = commandTextSave + "  END";
                        //            // fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandTextSave);
                        //            //--------modify by xiejp 2011929 ----to 
                        //            fbCommand.CommandText = commandTextSave;
                        //            fbCommand.ExecuteNonQuery();
                        //            i = 0;
                        //            //-------------
                        //            commandTextSave = "execute block as declare i int;" + " BEGIN   ";
                        //            //-----------加批量头-----------------------
                        //            //-----------加该行的insert语句-------------
                        //            commandTextSave = commandTextSave + string.Format("insert into PopulationGrowthWeights (PopulationDataSetID,Yyear,SourceColumn,SourceRow,TargetColumn,TargetRow,RaceID,EthnicityID,Vvalue ) values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, ((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iYear]).Text.ToString(), Convert.ToInt16(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iSourceCol]).Text.ToString()), Convert.ToInt16(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iSourceRow]).Text.ToString()), Convert.ToInt16(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iTargetCol]).Text.ToString()), Convert.ToInt16(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iTargetRow]).Text.ToString()), GetValueFromRaceID(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iRace]).Text.ToString()), GetValueFromEthnicityID(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iEthnicity]).Text.ToString()), Convert.ToDouble(((Microsoft.Office.Interop.Excel.Range)ws.Cells[k, iValue]).Text.ToString()));
                        //        }
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        Logger.LogError(ex);
                        //    }
                        //    i++;
                        //    progBarLoadPop.PerformStep();
                        //    lblprogbar.Text = "Saving Populaiton Growth Weights..." + Convert.ToString((int)((double)progBarLoadPop.Value / fileCount * 100)) + "%";
                        //    lblprogbar.Refresh();
                        //    icount++;
                        //}
                        ////-----------------最后 剩的-------------------------
                        //if (commandTextSave.LastIndexOf(" END") <= 0)
                        //{
                        //    //-------------加批量尾，提交---------------
                        //    commandTextSave = commandTextSave + "  END";
                        //    //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandTextSave);
                        //    fbCommand.CommandText = commandTextSave;
                        //    fbCommand.ExecuteNonQuery();
                        //}
                        #endregion
                    }
                    #region
                    //WaitClose();
                    //PopulationTipForm frm = new PopulationTipForm(fileCount, 0, "Saving Populaiton Growth Weights...");
                    //frm.Show(this);
                    //int i = 0;
                    //string commandTextSave = "execute block as declare i int;" + "BEGIN  ";
                    ////--------------加批量头
                    ////.................
                    //// string s = "";
                    //int iSourceCol = -1;
                    //int iSourceRow = -1;
                    //int iTargetCol = -1;
                    //int iTargetRow = -1;
                    //int iRace = -1;
                    //int iEthnicity = -1;
                    //int iValue = -1;
                    //int iYear = -1;
                    //int icount = 0;
                    //int iClose = 1;
                    //int j = 0;
                    //FirebirdSql.Data.FirebirdClient.FbCommand fbCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
                    //fbCommand.Connection = CommonClass.Connection;
                    //fbCommand.CommandType = CommandType.Text;
                    //CommonClass.Connection.Open();
                    //while (myReader.Read())
                    //{
                    //    if (j == 0)
                    //    {
                    //        for (int k = 0; k < myReader.FieldCount; k++)
                    //        {
                    //            if (myReader.GetName(k).ToLower().Replace(" ", "") == "sourcecol") { iSourceCol = k; }
                    //            if (myReader.GetName(k).ToLower().Replace(" ", "") == "sourcerow") { iSourceRow = k; }
                    //            if (myReader.GetName(k).ToLower().Replace(" ", "") == "targetcol") { iTargetCol = k; }
                    //            if (myReader.GetName(k).ToLower().Replace(" ", "") == "targetrow") { iTargetRow = k; }
                    //            if (myReader.GetName(k).ToLower().Replace(" ", "") == "race") { iRace = k; }
                    //            if (myReader.GetName(k).ToLower().Replace(" ", "") == "ethnicity") { iEthnicity = k; }
                    //            if (myReader.GetName(k).ToLower().Replace(" ", "") == "value") { iValue = k; }
                    //            if (myReader.GetName(k).ToLower().Replace(" ", "") == "year") { iYear = k; }
                    //        }
                    //        j++;
                    //    }
                    //    if (icount == 200 * 200 * iClose)
                    //    {
                    //        GC.Collect();
                    //        iClose++;
                    //    }
                    //    try
                    //    {
                    //        if (i < 200)
                    //        {
                    //            //-------------------加批量的语句 
                    //            commandTextSave = commandTextSave + string.Format("insert into PopulationGrowthWeights (PopulationDataSetID,Yyear,SourceColumn,SourceRow,TargetColumn,TargetRow,RaceID,EthnicityID,Vvalue ) values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, myReader[iYear], myReader[iSourceCol], myReader[iSourceRow], myReader[iTargetCol], myReader[iTargetRow], GetValueFromRaceID(myReader[iRace].ToString(), dicRace), GetValueFromEthnicityID(myReader[iEthnicity].ToString(), dicEthnicity), myReader[iValue]);
                    //            //commandTextSave = commandTextSave + s;                                
                    //        }
                    //        else
                    //        {
                    //            //-------------加批量尾，提交---------------
                    //            commandTextSave = commandTextSave + "  END";
                    //            //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandTextSave);
                    //            fbCommand.CommandText = commandTextSave;
                    //            fbCommand.ExecuteNonQuery();
                    //            i = 0;
                    //            //-------------
                    //            commandTextSave = "execute block as declare i int;" + "BEGIN  ";
                    //            //-----------加批量头-----------------------
                    //            //-----------加改行的insert语句-------------
                    //            commandTextSave = commandTextSave + string.Format("insert into PopulationGrowthWeights (PopulationDataSetID,Yyear,SourceColumn,SourceRow,TargetColumn,TargetRow,RaceID,EthnicityID,Vvalue ) values ({0},{1},{2},{3},{4},{5},{6},{7},{8});", dataSetID, myReader[iYear], myReader[iSourceCol], myReader[iSourceRow], myReader[iTargetCol], myReader[iTargetRow], GetValueFromRaceID(myReader[iRace].ToString(), dicRace), GetValueFromEthnicityID(myReader[iEthnicity].ToString(), dicEthnicity), myReader[iValue]);

                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Logger.LogError(ex.Message);
                    //    }
                    //    i++;
                    //    icount++;
                    //    frm.setPos(icount);
                    //}
                    ////-----------------最后 剩的-------------------------
                    //if (commandTextSave.LastIndexOf(" END") <= 0)
                    //{
                    //    //-------------加批量尾，提交---------------
                    //    commandTextSave = commandTextSave + "  END";

                    //    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandTextSave);
                    //}
                    //frm.Close();
                    //myReader.Close();
                    //oleCon.Close();
                    #endregion
                }
                if (wrongAgeRange)
                    MessageBox.Show("Population data with non-corresponding age range will not be imported into database.");
                if (!containYear)
                    MessageBox.Show("Population growth weights with non-corresponding year will not be imported into database.");

                fbtra.Commit();
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                fbtra.Rollback();
                lblprogbar.Text = "";
                WaitClose();
            }
        }

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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }


    }

}