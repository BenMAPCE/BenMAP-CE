using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Linq;
using ESIL.DBUtility;
using FirebirdSql.Data.FirebirdClient;
using System.IO;
using System.Collections.Generic;

namespace BenMAP
{
    public partial class MonitorDataSetDefinition : FormBase
    {
        public MonitorDataSetDefinition()
        {
            InitializeComponent();
        }

        private string _dataSetName;
        private object _dataSetID;//used for when a new dataset is created.
        private object _newDataSetID = null;//used for copying an existing dataset that is locked. (the new datasetid)
        private object _oldDataSetID = null;//used for copying an existing dataset that is locked. (the locked datasetid)
        private DataTable _dtDataFile;
        private MetadataClassObj _metadataObj = null;
        private bool _isLocked = false;
        
        private bool _CopyingDataset = false;
        //private string _strPath;

        public MonitorDataSetDefinition(string name, object id, bool isLocked)
            : this(name, id)
        {
            _isLocked = isLocked;
            if(_isLocked)
            {
                txtDataSetName.Enabled = true;//false;
                _dataSetName  = name + "_Copy";
                txtDataSetName.Text = _dataSetName;
                _oldDataSetID = _dataSetID;
                _CopyingDataset = true;
            }
            else
            {
                txtDataSetName.Enabled = false;
            }
        }
        public MonitorDataSetDefinition(string name, object id)
        {
            InitializeComponent();
            _dataSetName = name;
            _dataSetID = id;
        }

        private void MonitorDataSetDefinition_Load(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();

            try
            {
                progressBar1.Visible = false;
                if (_dataSetName != null)
                {
                    txtDataSetName.Text = _dataSetName;
                    addGridView(_dataSetID);
                }
                else
                {
                    int number = 0;
                    int monitorDatasetID = 0;
                    do
                    {
                        string comText = "select monitorDatasetID from monitorDataSets where monitorDatasetName=" + "'MonitorDataSet" + Convert.ToString(number) + "'";
                        monitorDatasetID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, comText));
                        number++;
                    } while (monitorDatasetID > 0);
                    txtDataSetName.Text = "MonitorDataSet" + Convert.ToString(number - 1);
                }
                string cmdText = "select PollutantID,PollutantName from Pollutants where SetupID=" + CommonClass.ManageSetup.SetupID + "";
                DataSet dst = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), cmdText);
                cboPollutant.DataSource = dst.Tables[0];
                cboPollutant.DisplayMember = "PollutantName";
                if (dst.Tables[0].Rows.Count != 0)
                { cboPollutant.SelectedIndex = 0; }
                txtYear.Text = "1999";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void addGridView(object id)
        {
            try
            {
                FireBirdHelperBase fb = new ESILFireBirdHelper();
                string commandText = string.Format("select c.pollutantname,a.yyear,count(*) from monitorentries a,monitors b,pollutants c where a.monitorid=b.monitorid and b.pollutantid=c.pollutantid and b.monitordatasetID={0} group by c.pollutantname,a.yyear", id);
                //string commandText = string.Format("select distinct c.pollutantname,a.yyear, d.metadataid, count(*) " +
                //                                    "from monitorentries a, monitors b, pollutants c, METADATAINFORMATION d " +
                //                                    "where a.monitorid=b.monitorid and b.pollutantid=c.pollutantid and " +
                //                                    "d.DATASETID=b.monitordatasetID and b.monitordatasetID={0} and d.DATASETTYPEID={1} " +
                //                                    "group by c.pollutantname,a.yyear, b.monitordatasetID, d.metadataid", _metadataObj.DatasetId, _metadataObj.DatasetTypeId);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                olvMonitorDataSets.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void cboPollutant_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (_dataSetID != null && !_isLocked)
                {
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    string commandText = string.Format("update MonitorDataSets set MonitorDataSetName='{0}' where MonitorDataSetID={1} and SetUpID={2}", txtDataSetName.Text, _dataSetID, CommonClass.ManageSetup.SetupID);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                }
                if(_isLocked)//doing a copy
                {
                    CopyDatabase();
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void txtYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == (char)8)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnBrowse_Click_1(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog() { RestoreDirectory = true };
                openFileDialog.InitialDirectory = CommonClass.ResultFilePath;
                openFileDialog.Filter = "All Files|*.*|CSV files|*.csv|XLS files|*.xls|XLSX files|*.xlsx";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                { return; }
                txtMonitorDataFile.Text = openFileDialog.FileName;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadDatabase();
        }

        private void LoadDatabase()
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                string commandText = string.Empty;
                if (cboPollutant.Text == string.Empty)
                {
                    MessageBox.Show("Please select a pollutant."); 
                    return;
                }
                if (txtYear.Text.Length != 4)
                {
                    MessageBox.Show("Please input a valid year."); 
                    return;
                }
                string msg = string.Format("Save this file associated with {0} and {1} ?", cboPollutant.GetItemText(cboPollutant.SelectedItem), txtYear.Text);
                DialogResult result = MessageBox.Show(msg, "Confirm Edit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No) return;
                //_dtDataFile = CommonClass.ExcelToDataTable(txtMonitorDataFile.Text);
                int iMonitorName = -1;
                int iMonitorDescription = -1;
                int iLatitude = -1;
                int iLongitude = -1;
                int iMetric = -1;
                int iSeasonalMetric = -1;
                int iStatistic = -1;
                int iValue = -1;
                for (int i = 0; i < _dtDataFile.Columns.Count; i++)
                {
                    switch (_dtDataFile.Columns[i].ColumnName.ToLower().Replace(" ", ""))
                    {
                        case "monitorname": iMonitorName = i;
                            break;
                        case "monitordescription": iMonitorDescription = i;
                            break;
                        case "latitude": iLatitude = i;
                            break;
                        case "longitude": iLongitude = i;
                            break;
                        case "metric": iMetric = i;
                            break;
                        case "seasonalmetric": iSeasonalMetric = i;
                            break;
                        case "statistic": iStatistic = i;
                            break;
                        case "values": iValue = i;
                            break;
                    }
                }
                string warningtip = "";
                if (iMonitorName < 0) warningtip = "'Monitor Name', ";
                if (iMonitorDescription < 0) warningtip += "'Monitor Description', ";
                if (iLatitude < 0) warningtip += "'Latitude', ";
                if (iLongitude < 0) warningtip += "'Longitude', ";
                if (iMetric < 0) warningtip += "'Metric', ";
                if (iSeasonalMetric < 0) warningtip += "'Seasonal Metric', ";
                if (iStatistic < 0) warningtip += "'Statistic', ";
                if (iValue < 0) warningtip += "'Values', ";
                if (warningtip != "")
                {
                    warningtip = warningtip.Substring(0, warningtip.Length - 2);
                    warningtip = "Please check the column header of " + warningtip + ". It is incorrect or does not exist.\r\n";
                    warningtip += "\r\nFile failed to load, please validate the file for a more detail explanation of errors.";
                    MessageBox.Show(warningtip, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    progressBar1.Visible = false;
                    return;
                }

                if (_dataSetID == null)
                {
                    commandText = string.Format("select MonitorDataSetID from MonitorDataSets where MonitorDataSetName='{0}' and setupID={1}", txtDataSetName.Text, CommonClass.ManageSetup.SetupID);
                    _dataSetID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (_dataSetID == null)
                    {
                        commandText = "select max(MonitorDataSetID) from MonitorDataSets";
                        _dataSetID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                        //the 'F' is for the LOCKED column in MonitorDataSets.  This is being added and is not a predefined.
                        commandText = string.Format("insert into MonitorDataSets values ({0},{1},'{2}', 'F')", _dataSetID, CommonClass.ManageSetup.SetupID, txtDataSetName.Text);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }
                //If copying a predefined dataset, a new deataset id will be needed. The user can load additional data into the dataset before doing a copy of the 
                //Old dataset.  If that be the case, the new dataset that is needed can be grabed here.
                if(_CopyingDataset)
                {
                    _newDataSetID = _dataSetID;
                }
                if (_dtDataFile != null)
                {
                    progressBar1.Visible = true;
                    progressBar1.Step = 1;
                    progressBar1.Minimum = 1;
                    progressBar1.Maximum = _dtDataFile.Rows.Count;
                    progressBar1.Value = progressBar1.Minimum;
                    commandText = string.Format("select pollutantid from pollutants where pollutantname='{0}' and setupid={1}", cboPollutant.Text, CommonClass.ManageSetup.SetupID);
                    int pollutantID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                    int monitorID = 0;
                    for (int i = 0; i < _dtDataFile.Rows.Count; i++)
                    {
                        commandText = "select max(MonitorID) from MONITORS";
                        monitorID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                        FbParameter Parameter = new FbParameter("@Description", _dtDataFile.Rows[i][iMonitorDescription]);
                        commandText = string.Format("insert into Monitors(Monitorid,Monitordatasetid,Pollutantid,Latitude,Longitude,Monitorname,Monitordescription, Metadataid) values ({0},{1},{2},{3},{4},'{5}',@Description, {6})", monitorID, _dataSetID, pollutantID, _dtDataFile.Rows[i][iLatitude], _dtDataFile.Rows[i][iLongitude], _dtDataFile.Rows[i][iMonitorName], _metadataObj.MetadataEntryId);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText, Parameter);
                        commandText = "select max(MonitorEntryID) from MonitorEntries";
                        int monitorEntriesID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                        commandText = string.Format("select metricID from Metrics where lower(MetricName)='{0}' and pollutantid={1}", _dtDataFile.Rows[i][iMetric].ToString().Trim().ToLower(), pollutantID);
                        object metricID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = string.Format("select SeasonalMetricID from SeasonalMetrics where lower(SeasonalMetricName)='{0}'and metricid in (select metricid from Metrics where lower(MetricName)='{1}' and pollutantid={2})", _dtDataFile.Rows[i][iSeasonalMetric].ToString().Trim().ToLower(), _dtDataFile.Rows[i][iMetric].ToString().Trim().ToLower(), pollutantID);
                        object seaMetricID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        FbParameter fbParameter = new FbParameter("@VValues", FbDbType.Binary);
                        byte[] blob = System.Text.Encoding.UTF8.GetBytes(_dtDataFile.Rows[i][iValue].ToString());
                        fbParameter.Value = blob;
                        string strMetricID = "null";
                        if (metricID != null) strMetricID = metricID.ToString();
                        string strSeasonalMetricID = "null";
                        if (seaMetricID != null) strSeasonalMetricID = seaMetricID.ToString();
                        commandText = string.Format("insert into MonitorEntries(Monitorentryid,Monitorid,Yyear,Metricid,Seasonalmetricid,Statistic,Vvalues) values ({0},{1},{2},{3},{4},'{5}',@VValues)",
                         monitorEntriesID, monitorID, txtYear.Text, strMetricID, strSeasonalMetricID, _dtDataFile.Rows[i][iStatistic]);

                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText, fbParameter);
                        progressBar1.PerformStep();
                        lblProgress.Text = Convert.ToString((int)((double)progressBar1.Value / _dtDataFile.Rows.Count * 100)) + "%";
                        lblProgress.Refresh();
                    }

                    //insertMetadata(Convert.ToInt16(_dataSetID), pollutantID);
                    insertMetadata(Convert.ToInt16(_dataSetID));
                }
                progressBar1.Visible = false;
                lblProgress.Text = "";
                addGridView(_dataSetID); //return;
            }
            catch (Exception ex)
            {
                progressBar1.Visible = false;
                lblProgress.Text = "";
                addGridView(_dataSetID);
                Logger.LogError(ex.Message);
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
                if (cboPollutant.Text == string.Empty)
                {
                    MessageBox.Show("Please select a pollutant.");
                    return;
                }
                if (txtYear.Text.Length != 4)
                {
                    MessageBox.Show("Please input a valid year.");
                    return;
                }
                //check and see if name is used
                commandText = string.Format("Select MONITORDATASETNAME from MONITORDATASETS WHERE MONITORDATASETNAME = '{0}'",txtDataSetName.Text.Trim());
                rVal = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                if(rVal != null)
                {
                    MessageBox.Show("Name is already used.  Please select a new name.");
                    txtDataSetName.Focus();
                    return;
                }

                string msg = string.Format("Save this file associated with {0} and {1} ?", cboPollutant.GetItemText(cboPollutant.SelectedItem), txtYear.Text);
                DialogResult result = MessageBox.Show(msg, "Confirm Copy", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No) return;
                //getting a new dataset
                if(_newDataSetID == null)
                {
                    commandText = commandText = "select max(MonitorDataSetID) from MonitorDataSets";
                    _newDataSetID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                }
                //the 'F' is for the LOCKED column in MonitorDataSets.  This is being added and is not a predefined.
                commandText = string.Format("insert into MonitorDataSets values ({0},{1},'{2}', 'F')", _newDataSetID, CommonClass.ManageSetup.SetupID, txtDataSetName.Text);
                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                commandText = "select max(MonitorID) from MONITORS";
                maxID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                commandText = string.Format("select min(MonitorID) from MONITORS where MONITORDATASETID = {0}", _oldDataSetID);
                minID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                //inserting - copying the locked data to the new data set
                commandText = string.Format("insert into Monitors(Monitorid, Monitordatasetid, Pollutantid, Latitude, Longitude, Monitorname, Monitordescription, Metadataid) " +
                              "SELECT Monitorid + ({0} - {1}) + 1, " +
                              "{2}, Pollutantid, Latitude, Longitude, Monitorname,Monitordescription, " +
                              "Metadataid FROM Monitors WHERE MONITORDATASETID = {3}", maxID, minID, _newDataSetID, _oldDataSetID );
                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

                commandText = string.Format("insert into MonitorEntries (MONITORENTRYID,MONITORID,YYEAR,METRICID,SEASONALMETRICID,STATISTIC,VVALUES) " +
                                            "SELECT (SELECT MAX(MONITORENTRYID) FROM MONITORENTRIES)+OE.MONITORENTRYID - (SELECT MIN(MONITORENTRYID) FROM MONITORENTRIES A " +
                                            "INNER JOIN MONITORS B " +
                                            "ON A.MONITORID = B.MONITORID " +
                                            "WHERE B.MONITORDATASETID = {0}) +1 AS NEWMONITORENTRYID, " +
                                            "NM.MONITORID AS NEWMONITORID, OE.YYEAR,OE.METRICID, OE.SEASONALMETRICID, OE.STATISTIC, OE.VVALUES FROM MONITORENTRIES OE " +
                                            "INNER JOIN MONITORS OM " +
                                            "ON OE.MONITORID = OM.MONITORID " +
                                            "INNER JOIN MONITORS NM " +
                                            "ON NM.MONITORID = OM.Monitorid + ({2} - {3}) + 1 " +
                                            "WHERE OM.MONITORDATASETID = {0} " +
                                            "AND NM.MONITORDATASETID = {1}", _oldDataSetID, _newDataSetID, maxID, minID);
                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                _metadataObj = new MetadataClassObj();
                _metadataObj.DatasetId =  Convert.ToInt32(_newDataSetID);
                _metadataObj.FileName = txtDataSetName.Text;

            }
            catch (Exception ex)
            {
                progressBar1.Visible = false;
                lblProgress.Text = "";
                addGridView(_dataSetID);
                Logger.LogError(ex.Message);
            }
        }

        //private void insertMetadata(int dataSetID, int pollutantId)
        private void insertMetadata(int dataSetID)
        {
            _metadataObj.DatasetId = dataSetID;

            _metadataObj.DatasetTypeId = SQLStatementsCommonClass.getDatasetID("Monitor");
            if (!SQLStatementsCommonClass.insertMetadata(_metadataObj))
            {
                MessageBox.Show("Failed to save Metadata.");
            }
            //if(!SQLStatementsCommonClass.updateMonitorsTable(_metadataObj.MetadataId, dataSetID, pollutantId))
            //{
            //    MessageBox.Show("Failed to save metadataId to Monitors table.");
            //}
        }
       
        private static Dictionary<int, string> getMetric()
        {
            try
            {
                Dictionary<int, string> dicMetric = new Dictionary<int, string>();
                string commandText = "select MetricID,MetricName from Metrics "; ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (!dicMetric.Keys.Contains(Convert.ToInt32(dr["MetricID"])))
                        dicMetric.Add(Convert.ToInt32(dr["MetricID"]), dr["MetricName"].ToString());
                }
                return dicMetric;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return null;
            }
        }

        private static Dictionary<int, string> getSeasonalMetric()
        {
            try
            {
                Dictionary<int, string> dicSMetric = new Dictionary<int, string>();
                string commandText = "select SeasonalMetricID,SeasonalMetricName from SeasonalMetrics "; ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (!dicSMetric.Keys.Contains(Convert.ToInt32(dr["SeasonalMetricID"])))
                        dicSMetric.Add(Convert.ToInt32(dr["SeasonalMetricID"]), dr["SeasonalMetricName"].ToString());
                }
                return dicSMetric;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return null;
            }
        }

        private string getStringFromID(int id, Dictionary<int, string> dic)
        {
            try
            {
                string result = string.Empty;
                foreach (int s in dic.Keys)
                {
                    if (s == id)
                        result = dic[s];
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return null;
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
                dtOut.Columns.Add("Monitor Name", typeof(string));
                dtOut.Columns.Add("Monitor Description", typeof(string));
                dtOut.Columns.Add("Latitude", typeof(double));
                dtOut.Columns.Add("Longitude", typeof(double));
                dtOut.Columns.Add("Metric", typeof(string));
                dtOut.Columns.Add("Seasonal Metric", typeof(string));
                dtOut.Columns.Add("Statistic", typeof(string));
                dtOut.Columns.Add("Values", typeof(string));
                Dictionary<int, string> dicMetric = getMetric();
                Dictionary<int, string> dicSeasonalMetric = getSeasonalMetric();

                FireBirdHelperBase fb = new ESILFireBirdHelper();
                string commandText = string.Empty;
                int outputRowsNumber = 50;
                commandText = "select count(*) from Monitors";
                int count = (int)fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                if (count < outputRowsNumber) { outputRowsNumber = count; }
                commandText = string.Format("select first {0} MonitorName,MonitorDescription,Latitude,Longitude,MetricID,SeasonalMetricID,Statistic,Vvalues from Monitors a,MonitorEntries b where a.MonitorID=b.MonitorID", outputRowsNumber);
                FbDataReader fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, commandText);
                Byte[] blob = null;
                string str = string.Empty;
                while (fbDataReader.Read())
                {
                    DataRow dr = dtOut.NewRow();
                    blob = fbDataReader["Vvalues"] as byte[];
                    str = System.Text.Encoding.Default.GetString(blob);
                    dr["Monitor Name"] = fbDataReader["MonitorName"].ToString();
                    dr["Monitor Description"] = fbDataReader["MonitorDescription"].ToString();
                    dr["Latitude"] = Convert.ToDouble(fbDataReader["Latitude"]);
                    dr["Longitude"] = Convert.ToDouble(fbDataReader["Longitude"]);
                    if (!(fbDataReader["MetricID"] is DBNull))
                    {
                        dr["Metric"] = getStringFromID(Convert.ToInt32(fbDataReader["MetricID"]), dicMetric);
                    }
                    if (!(fbDataReader["SeasonalMetricID"] is DBNull))
                    {
                        dr["Seasonal Metric"] = getStringFromID(Convert.ToInt32(fbDataReader["SeasonalMetricID"]), dicSeasonalMetric);
                    }
                    dr["Statistic"] = fbDataReader["Statistic"].ToString();
                    dr["Values"] = str;
                    dtOut.Rows.Add(dr);
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
                    { data += "\"" + dt.Rows[i][j].ToString() + "\""; }
                    else
                    {
                        data += dt.Rows[i][j].ToString();
                    }
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

        private void btnBrowse1_Click(object sender, EventArgs e)
        {
            if(checkForDuplicate())
            {

                LoadSelectedDataSet lmdataset = new LoadSelectedDataSet("Load Monitor Dataset", "Monitor Dataset Name:", txtDataSetName.Text, "Monitor");

                DialogResult dlgr = lmdataset.ShowDialog();
                if(dlgr.Equals(DialogResult.OK))
                {
                    _dtDataFile = lmdataset.MonitorDataSet;
                    _metadataObj = lmdataset.MetadataObj;
                    olvMonitorDataSets.ClearObjects();
                    LoadDatabase();
                }
            }
            else
            {
                MessageBox.Show("Dublicate entry.  There is alrady an entry for the selected pollutant and year"); 
            }
        }
       
        private bool checkForDuplicate()
        {
            bool bPassed = true;
            string pollutant = cboPollutant.Text;
            string year = txtYear.Text;

            foreach(ListViewItem lvi in olvMonitorDataSets.Items)
            {
                if(pollutant.Equals(lvi.SubItems[0].Text) && year.Equals(lvi.SubItems[1].Text))
                {
                    bPassed = false;
                    break;
                }
            }

            return bPassed;
        }
    }
}