using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using FirebirdSql.Data.FirebirdClient;

namespace BenMAP
{
    public partial class FilterMonitors : FormBase
    {
        private MonitorAdvance _MonitorAdvanceFilter;

        public MonitorAdvance MonitorAdvanceFilter
        {
            get { return _MonitorAdvanceFilter; }
            set { _MonitorAdvanceFilter = value; }
        }

        private MonitorAdvance MonitorAdvanceFilterTemp;

        public BaseControlGroup bcg = new BaseControlGroup();
        public MonitorDataLine mDataLine = new MonitorDataLine();
        bool settingcorrect = true;

        FilterMonitorsPM ucPM = new FilterMonitorsPM();
        FilterMonitorsOzone ucOzone = new FilterMonitorsOzone();

        public FilterMonitors()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void FilterMonitors_Load(object sender, EventArgs e)
        {
            try
            {
                if (_MonitorAdvanceFilter.FilterIncludeIDs != null)
                {
                    string excludeID = "";
                    foreach (string i in _MonitorAdvanceFilter.FilterIncludeIDs)
                    {
                        excludeID += i.ToString() + ",";
                    }
                    if (excludeID.Length > 0)
                    {
                        excludeID.Substring(0, excludeID.Length - 1);
                    }
                    txtExcludeID.Text = excludeID;
                }

                if (_MonitorAdvanceFilter.FilterExcludeIDs != null)
                {
                    string includeID = "";
                    foreach (string i in _MonitorAdvanceFilter.FilterExcludeIDs)
                    {
                        includeID += i.ToString() + ",";
                    }
                    if (includeID.Length > 0)
                    {
                        includeID.Substring(0, includeID.Length - 1);
                    }
                    txtIncludeID.Text = includeID;
                }

                if (_MonitorAdvanceFilter.FilterStates != null)
                {
                    string filterState = "";
                    foreach (Location l in _MonitorAdvanceFilter.FilterStates)
                    {
                        filterState += l.LocationName + ",";
                    }
                    if (filterState.Length > 0)
                    {
                        filterState.Substring(0, filterState.Length - 1);
                    }
                    txtStates.Text = filterState;
                }

                txtMinimumLongitude.Text = _MonitorAdvanceFilter.FilterMinLongitude.ToString();
                txtMaximumLongitude.Text = _MonitorAdvanceFilter.FilterMaxLongitude.ToString();
                txtMinimumLatitude.Text = _MonitorAdvanceFilter.FilterMinLatitude.ToString();
                txtMaximumLatitude.Text = _MonitorAdvanceFilter.FilterMaxLatitude.ToString();

                if (_MonitorAdvanceFilter.FilterMaximumPOC > 0)
                {
                    nudownMaximumPOC.Value = _MonitorAdvanceFilter.FilterMaximumPOC;
                }
                else
                {
                    switch (bcg.Pollutant.PollutantName)
                    {
                        case "PM2.5":
                            nudownMaximumPOC.Value = 4;
                            break;
                        case "PM10":
                            nudownMaximumPOC.Value = 4;
                            break;
                        case "Ozone":
                            nudownMaximumPOC.Value = 4;
                            break;
                        case "NO2":
                            nudownMaximumPOC.Value = 9;
                            break;
                        case "SO2":
                            nudownMaximumPOC.Value = 9;
                            break;
                        default:
                            nudownMaximumPOC.Value = 9;
                            break;
                    }
                }

                if (_MonitorAdvanceFilter.POCPreferenceOrder != null)
                {
                    txtPOCPreferenceOrder.Text = _MonitorAdvanceFilter.POCPreferenceOrder;
                }
                else
                {
                    switch (bcg.Pollutant.PollutantName)
                    {
                        case "PM2.5":
                        case "PM10":
                        case "Ozone":
                            txtPOCPreferenceOrder.Text = "1,2,3,4";
                            break;
                        case "NO2":
                        case "SO2":
                            txtPOCPreferenceOrder.Text = "1,2,3,4,5,6,7,8,9";
                            break;
                        default:
                            txtPOCPreferenceOrder.Text = "1,2,3,4,5,6,7,8,9";
                            break;
                    }
                }

                if (_MonitorAdvanceFilter.IncludeMethods != null)
                {
                    List<string> allmethod = FindAllMethod();
                    if (allmethod != null && allmethod.Count > 0)
                    {
                        foreach (string code in allmethod)
                        {
                            chkSelectMethods.Items.Add(code);
                        }
                        foreach (string s in _MonitorAdvanceFilter.IncludeMethods)
                        {
                            for (int i = 0; i < chkSelectMethods.Items.Count; i++)
                            {
                                if (s == chkSelectMethods.Items[i].ToString())
                                {
                                    chkSelectMethods.SetItemChecked(i, true);
                                }
                            }
                        }
                        chkSelectMethods.Sorted = true;
                    }
                }
                else
                {
                    Dictionary<string, bool> dicmethods = LoadMethod();
                    if (dicmethods != null && dicmethods.Count() > 0)
                    {
                        foreach (string code in dicmethods.Keys)
                        {
                            chkSelectMethods.Items.Add(code, dicmethods[code]);
                        }
                        chkSelectMethods.Sorted = true;
                    }
                }


                switch (bcg.Pollutant.PollutantName)
                {
                    case "PM2.5":
                    case "PM10":
                        lblNoOption.Visible = false;
                        ucPM = new FilterMonitorsPM();
                        pnlStep6.Controls.Add(ucPM);
                        ucPM.nudownNumberOfValidObservations.Value = _MonitorAdvanceFilter.NumberOfPerQuarter != 0 ? _MonitorAdvanceFilter.NumberOfPerQuarter : 11;
                        switch (_MonitorAdvanceFilter.DataTypesToUse)
                        {
                            case MonitorAdvanceDataTypeEnum.Local:
                                ucPM.rbtnLocal.Checked = true;
                                break;
                            case MonitorAdvanceDataTypeEnum.Standard:
                                ucPM.rbtnStandard.Checked = true;
                                break;
                            case MonitorAdvanceDataTypeEnum.Both:
                                ucPM.rbtnBoth.Checked = true;
                                break;
                        }

                        switch (_MonitorAdvanceFilter.PreferredType)
                        {
                            case MonitorAdvanceDataTypeEnum.Local:
                                ucPM.rbtnPreferredLocal.Checked = true;
                                break;
                            case MonitorAdvanceDataTypeEnum.Standard:
                                ucPM.rbtnStandardPreferred.Checked = true;
                                break;
                        }

                        switch (_MonitorAdvanceFilter.OutputType)
                        {
                            case MonitorAdvanceDataTypeEnum.Local:
                                ucPM.rbtnOutputLocal.Checked = true;
                                break;
                            case MonitorAdvanceDataTypeEnum.Standard:
                                ucPM.rbtnOutputStandard.Checked = true;
                                break;
                        }
                        break;
                    case "Ozone":
                        ucOzone = new FilterMonitorsOzone();
                        pnlStep6.Controls.Add(ucOzone);
                        ucOzone.numDownStartHour.Value = MonitorAdvanceFilter.StartHour;
                        ucOzone.numDownEndHour.Value = MonitorAdvanceFilter.EndHour;
                        ucOzone.nudownNumberOfValidHours.Value = MonitorAdvanceFilter.NumberOfValidHour;
                        ucOzone.nudownNumberOfValidDays.Value = MonitorAdvanceFilter.PercentOfValidDays;
                        ucOzone.dtpStartTime.CustomFormat = "MMMM dd";
                        ucOzone.dtpEndTime.CustomFormat = "MMMM dd";
                        ucOzone.dtpStartTime.MinDate = new DateTime(0001, 1, 1);
                        ucOzone.dtpStartTime.MaxDate = new DateTime(9990, 12, 31);
                        ucOzone.dtpEndTime.MinDate = new DateTime(0001, 1, 1);
                        ucOzone.dtpEndTime.MaxDate = new DateTime(9990, 12, 31);
                        ucOzone.dtpStartTime.Value = new DateTime(2011, 1, 1).AddDays(MonitorAdvanceFilter.StartDate);
                        ucOzone.dtpEndTime.Value = new DateTime(2011, 1, 1).AddDays(MonitorAdvanceFilter.EndDate);
                        break;
                    default:
                        lblNoOption.Visible = true;
                        break;
                }
            }
            catch
            { }
        }

        private void btnMap_Click(object sender, EventArgs e)
        {
            try
            {
                updataMonitorAdvance();
                if (!settingcorrect) return;
                WaitShow("Filtering monitors. Please wait.");
                mDataLine.MonitorAdvance = MonitorAdvanceFilterTemp;
                List<MonitorValue> lstMonitorValues = DataSourceCommonClass.GetMonitorData(bcg.GridType, bcg.Pollutant, mDataLine);
                DataSourceCommonClass.UpdateMonitorDicMetricValue(bcg.Pollutant, lstMonitorValues);
                string shapeFile = CommonClass.DataFilePath + @"\Data\Shapefiles\United States\State_epa2.shp";
                MonitorMap frm = new MonitorMap();
                frm.GridShapeFile = shapeFile;
                frm.LstMonitorPoints = lstMonitorValues;
                WaitClose();
                frm.ShowDialog();
            }
            catch
            {
                waitMess.Close();
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            updataMonitorAdvance();
            if (!settingcorrect) return;
            _MonitorAdvanceFilter = MonitorAdvanceFilterTemp;
            this.DialogResult = DialogResult.OK;
        }

        private void updataMonitorAdvance()
        {
            try
            {
                settingcorrect = true;
                MonitorAdvanceFilterTemp = new MonitorAdvance();
                if (!string.IsNullOrEmpty(txtIncludeID.Text))
                {
                    string[] sArray = txtIncludeID.Text.Split(',');
                    if (MonitorAdvanceFilterTemp.FilterIncludeIDs == null)
                        MonitorAdvanceFilterTemp.FilterIncludeIDs = new List<string>();
                    foreach (string s in sArray)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            try
                            {
                                MonitorAdvanceFilterTemp.FilterIncludeIDs.Add(s);
                            }
                            catch
                            {
                                MessageBox.Show("Please input valid value for monitor ID to be included.", "Error", MessageBoxButtons.OK);
                                settingcorrect = false;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(txtExcludeID.Text))
                {
                    string[] sArray = txtExcludeID.Text.Split(',');
                    if (MonitorAdvanceFilterTemp.FilterExcludeIDs == null)
                        MonitorAdvanceFilterTemp.FilterExcludeIDs = new List<string>();
                    foreach (string s in sArray)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            try
                            {
                                MonitorAdvanceFilterTemp.FilterExcludeIDs.Add(s);
                            }
                            catch
                            {
                                settingcorrect = false;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(txtStates.Text))
                {
                    string[] sArray = txtStates.Text.Split(',');
                    foreach (string s in sArray)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                        }
                    }
                }

                try
                {
                    Convert.ToDouble(txtMinimumLongitude.Text);
                    if (Convert.ToDouble(txtMinimumLongitude.Text) > 180 || Convert.ToDouble(txtMinimumLongitude.Text) < -180)
                    {
                        MessageBox.Show("Please input a valid value for minimum longitude.", "Error", MessageBoxButtons.OK);
                        settingcorrect = false;
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("Please input a valid value for minimum longitude.", "Error", MessageBoxButtons.OK);
                    settingcorrect = false;
                    return;
                }

                try
                {
                    Convert.ToDouble(txtMaximumLongitude.Text);
                    if (Convert.ToDouble(txtMaximumLongitude.Text) > 180 || Convert.ToDouble(txtMaximumLongitude.Text) < -180)
                    {
                        MessageBox.Show("Please input a valid value for maximum longitude.", "Error", MessageBoxButtons.OK);
                        settingcorrect = false;
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("Please input a valid value for maximum longitude.", "Error", MessageBoxButtons.OK);
                    settingcorrect = false;
                    return;
                }

                try
                {
                    Convert.ToDouble(txtMinimumLatitude.Text);
                    if (Convert.ToDouble(txtMinimumLatitude.Text) > 180 || Convert.ToDouble(txtMinimumLatitude.Text) < -180)
                    {
                        MessageBox.Show("Please input a valid value for minimum latitude.", "Error", MessageBoxButtons.OK);
                        settingcorrect = false;
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("Please input a valid value for minimum latitude.", "Error", MessageBoxButtons.OK);
                    settingcorrect = false;
                    return;
                }

                try
                {
                    Convert.ToDouble(txtMaximumLatitude.Text);
                    if (Convert.ToDouble(txtMaximumLatitude.Text) > 180 || Convert.ToDouble(txtMaximumLatitude.Text) < -180)
                    {
                        MessageBox.Show("Please input a valid value for maximum latitude.", "Error", MessageBoxButtons.OK);
                        settingcorrect = false;
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("Please input a valid value for maximum latitude.", "Error", MessageBoxButtons.OK);
                    settingcorrect = false;
                    return;
                }

                if (Convert.ToDouble(txtMinimumLatitude.Text) > Convert.ToDouble(txtMaximumLatitude.Text))
                {
                    MessageBox.Show("The minimum latitude must be smaller than the maximum latitude.", "Error", MessageBoxButtons.OK);
                    settingcorrect = false;
                    return;
                }
                if (Convert.ToDouble(txtMinimumLongitude.Text) > Convert.ToDouble(txtMaximumLongitude.Text))
                {
                    MessageBox.Show("The minimum longitude must be smaller than the maximum longitude.", "Error", MessageBoxButtons.OK);
                    settingcorrect = false;
                    return;
                }
                MonitorAdvanceFilterTemp.FilterMinLongitude = Convert.ToDouble(txtMinimumLongitude.Text);
                MonitorAdvanceFilterTemp.FilterMaxLongitude = Convert.ToDouble(txtMaximumLongitude.Text);
                MonitorAdvanceFilterTemp.FilterMinLatitude = Convert.ToDouble(txtMinimumLatitude.Text);
                MonitorAdvanceFilterTemp.FilterMaxLatitude = Convert.ToDouble(txtMaximumLatitude.Text);

                MonitorAdvanceFilterTemp.FilterMaximumPOC = Convert.ToInt16(nudownMaximumPOC.Value);
                string[] sPOC = txtPOCPreferenceOrder.Text.Split(',');
                foreach (string poc in sPOC)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(poc))
                        {
                            Convert.ToInt16(poc);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Please input valid values for POC preference order.", "Error", MessageBoxButtons.OK);
                        settingcorrect = false;
                        return;
                    }
                }
                MonitorAdvanceFilterTemp.POCPreferenceOrder = txtPOCPreferenceOrder.Text;

                MonitorAdvanceFilterTemp.IncludeMethods = new List<string>();
                foreach (string m in chkSelectMethods.CheckedItems)
                {
                    MonitorAdvanceFilterTemp.IncludeMethods.Add(m);
                }

                switch (bcg.Pollutant.PollutantName)
                {
                    case "PM2.5":
                    case "PM10":
                        MonitorAdvanceFilterTemp.NumberOfPerQuarter = Convert.ToInt16(ucPM.nudownNumberOfValidObservations.Value);
                        if (ucPM.rbtnLocal.Checked)
                        {
                            MonitorAdvanceFilterTemp.DataTypesToUse = MonitorAdvanceDataTypeEnum.Local;
                        }
                        if (ucPM.rbtnStandard.Checked)
                        {
                            MonitorAdvanceFilterTemp.DataTypesToUse = MonitorAdvanceDataTypeEnum.Standard;
                        }
                        if (ucPM.rbtnBoth.Checked)
                        {
                            MonitorAdvanceFilterTemp.DataTypesToUse = MonitorAdvanceDataTypeEnum.Both;
                        }

                        if (ucPM.rbtnPreferredLocal.Checked)
                        {
                            MonitorAdvanceFilterTemp.PreferredType = MonitorAdvanceDataTypeEnum.Local;
                        }
                        if (ucPM.rbtnStandardPreferred.Checked)
                        {
                            MonitorAdvanceFilterTemp.PreferredType = MonitorAdvanceDataTypeEnum.Standard;
                        }

                        if (ucPM.rbtnOutputLocal.Checked)
                        {
                            MonitorAdvanceFilterTemp.OutputType = MonitorAdvanceDataTypeEnum.Local;
                        }
                        if (ucPM.rbtnOutputStandard.Checked)
                        {
                            MonitorAdvanceFilterTemp.OutputType = MonitorAdvanceDataTypeEnum.Standard;
                        }
                        break;
                    case "Ozone":
                        if (ucOzone.numDownStartHour.Value >= ucOzone.numDownEndHour.Value)
                        {
                            MessageBox.Show("Start hour must be smaller than the end hour.", "Error", MessageBoxButtons.OK);
                            settingcorrect = false;
                            return;
                        }
                        if (ucOzone.dtpStartTime.Value.DayOfYear >= ucOzone.dtpEndTime.Value.DayOfYear)
                        {
                            MessageBox.Show("Start day must be less than the end day.", "Error", MessageBoxButtons.OK);
                            settingcorrect = false;
                            return;
                        }
                        MonitorAdvanceFilterTemp.StartHour = Convert.ToInt16(ucOzone.numDownStartHour.Value);
                        MonitorAdvanceFilterTemp.EndHour = Convert.ToInt16(ucOzone.numDownEndHour.Value);
                        MonitorAdvanceFilterTemp.NumberOfValidHour = Convert.ToInt16(ucOzone.nudownNumberOfValidHours.Value);
                        MonitorAdvanceFilterTemp.StartDate = ucOzone.dtpStartTime.Value.DayOfYear;
                        MonitorAdvanceFilterTemp.EndDate = ucOzone.dtpEndTime.Value.DayOfYear;
                        MonitorAdvanceFilterTemp.PercentOfValidDays = Convert.ToInt16(ucOzone.nudownNumberOfValidDays.Value);
                        break;
                }
            }
            catch
            {
                settingcorrect = false;
            }
        }

        private void txtMinimumLongitude_KeyPress(object sender, KeyPressEventArgs e)
        {
            keypress(sender, e);
        }

        void keypress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == (char)8)
            {
                e.Handled = false;
            }
            else
            {
                switch (e.KeyChar)
                {
                    case '-':
                        e.Handled = false;
                        break;
                    case '.':
                        if (((TextBox)sender).Text.Trim().IndexOf('.') > -1)
                        {
                            e.Handled = true;
                        }
                        break;
                    default: e.Handled = true; break;
                }
            }
        }

        private void txtMaximumLongitude_KeyPress(object sender, KeyPressEventArgs e)
        {
            keypress(sender, e);
        }

        private void txtMinimumLatitude_KeyPress(object sender, KeyPressEventArgs e)
        {
            keypress(sender, e);
        }

        private void txtMaximumLatitude_KeyPress(object sender, KeyPressEventArgs e)
        {
            keypress(sender, e);
        }

        private void nudownMaximumPOC_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtPOCPreferenceOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == (char)8 || e.KeyChar == ',')
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "CSV File|*.csv";
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                lblExport.Text = "Checking the monitor filter setting...";
                lblExport.Refresh();
                updataMonitorAdvance();
                if (!settingcorrect)
                {
                    lblExport.Text = "";
                    return;
                }
                lblExport.Text = "";
                lblExport.Refresh();
                prgBarExport.Visible = true;
                mDataLine.MonitorAdvance = MonitorAdvanceFilterTemp;
                List<MonitorValue> lstMonitorValues = DataSourceCommonClass.GetMonitorData(bcg.GridType, bcg.Pollutant, mDataLine);
                prgBarExport.Maximum = lstMonitorValues.Count();
                prgBarExport.Value = 0;
                FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine("Monitor Name,Monitor Description,Latitude,Longitude,Metric,Seasonal Metric,Statistic,Values");
                foreach (MonitorValue mv in lstMonitorValues)
                {
                    sw.Write(mv.MonitorName);
                    sw.Write(",");
                    sw.Write("\"" + mv.MonitorMethod + "\"");
                    sw.Write(",");
                    sw.Write(mv.Latitude);
                    sw.Write(",");
                    sw.Write(mv.Longitude);
                    sw.Write(",");
                    if (mv.Metric != null)
                    {
                        sw.Write(mv.Metric.MetricName);
                    }
                    sw.Write(",");
                    if (mv.SeasonalMetric != null)
                    {
                        sw.Write(mv.SeasonalMetric.SeasonalMetricName);
                    }
                    sw.Write(",");
                    sw.Write(mv.Statistic);
                    sw.Write(",");
                    string value = string.Empty;
                    string commandText = string.Format("select VValues from MonitorEntries where MonitorID={0}", mv.MonitorID);
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    byte[] blob = null;
                    object ob = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    blob = ob as byte[];
                    value = System.Text.Encoding.Default.GetString(blob);
                    sw.Write("\"" + value + "\"");
                    sw.WriteLine();
                    prgBarExport.Value++;
                    prgBarExport.Refresh();
                }
                sw.Flush();
                sw.Close();
                fs.Close();
                prgBarExport.Visible = false;
                MessageBox.Show("Export is complete.", "File saved", MessageBoxButtons.OK);
            }
            catch
            {
                prgBarExport.Visible = false;
            }
        }

        private Dictionary<string, bool> LoadMethod()
        {
            try
            {
                Dictionary<string, bool> dicmethod = new Dictionary<string, bool>();
                int maxPOC = 9;
                switch (bcg.Pollutant.PollutantName)
                {
                    case "PM2.5":
                    case "PM10":
                    case "Ozone":
                        maxPOC = 4;
                        break;
                    case "NO2":
                    case "SO2":
                        maxPOC = 9;
                        break;
                }
                if (mDataLine.MonitorDirectType == 0)
                {
                    string commandText = string.Format("select b.Latitude,b.Longitude,b.MonitorDescription from MonitorEntries a,Monitors b,MonitorDataSets c where a.MonitorID=b.MonitorID and b.MonitorDataSetID=c.MonitorDataSetID and b.PollutantID={0} and c.MonitorDataSetID={1} and a.YYear={2} ", bcg.Pollutant.PollutantID, mDataLine.MonitorDataSetID, mDataLine.MonitorLibraryYear);
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    FbDataReader fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, commandText);
                    while (fbDataReader.Read())
                    {
                        double Latitude = Convert.ToDouble(fbDataReader["Latitude"]);
                        double Longitude = Convert.ToDouble(fbDataReader["Longitude"]);
                        bool contain = true;
                        if (!((Convert.ToDouble(Latitude) >= 20) && (Convert.ToDouble(Latitude) <= 55) && (Convert.ToDouble(Longitude) <= -65) && (Longitude) >= -130))
                        { contain = false; }
                        string MonitorMethod = fbDataReader["MonitorDescription"].ToString();
                        if (!string.IsNullOrEmpty(MonitorMethod) && MonitorMethod.Contains("MethodCode") && MonitorMethod.Contains("POC"))
                        {
                            int iPOC = Convert.ToInt16(MonitorMethod.Substring(MonitorMethod.IndexOf("POC=") + 4, MonitorMethod.IndexOf('\'', MonitorMethod.IndexOf("POC=") + 4) - MonitorMethod.IndexOf("POC=") - 4));
                            string methodcode = MonitorMethod.Substring(12, MonitorMethod.IndexOf('\'', 12) - 12);
                            if (iPOC <= maxPOC && contain == true)
                            {
                                if (!dicmethod.ContainsKey(methodcode))
                                {
                                    dicmethod.Add(methodcode, true);
                                }
                                else
                                {
                                    dicmethod[methodcode] = true;
                                }
                            }
                            else
                            {
                                if (!dicmethod.ContainsKey(methodcode))
                                {
                                    dicmethod.Add(methodcode, false);
                                }
                            }
                        }
                    }
                    fbDataReader.Close();
                }
                return dicmethod;
            }
            catch
            {
                return null;
            }
        }

        private List<string> FindAllMethod()
        {
            try
            {
                List<string> method = new List<string>();
                if (mDataLine.MonitorDirectType == 0)
                {
                    string commandText = string.Format("select b.MonitorDescription from MonitorEntries a,Monitors b,MonitorDataSets c where a.MonitorID=b.MonitorID and b.MonitorDataSetID=c.MonitorDataSetID and b.PollutantID={0} and c.MonitorDataSetID={1} and a.YYear={2} ", bcg.Pollutant.PollutantID, mDataLine.MonitorDataSetID, mDataLine.MonitorLibraryYear);
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    FbDataReader fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, commandText);
                    while (fbDataReader.Read())
                    {
                        string MonitorMethod = fbDataReader["MonitorDescription"].ToString();
                        if (!string.IsNullOrEmpty(MonitorMethod) && MonitorMethod.Contains("MethodCode"))
                        {
                            string methodcode = MonitorMethod.Substring(12, MonitorMethod.IndexOf('\'', 12) - 12);
                            if (!method.Contains(methodcode))
                            {
                                method.Add(methodcode);
                            }
                        }
                    }
                    fbDataReader.Close();
                }
                return method;
            }
            catch
            {
                return null;
            }
        }
    }
}
