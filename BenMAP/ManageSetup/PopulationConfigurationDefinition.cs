using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESIL.DBUtility;

namespace BenMAP
{
    public partial class PopulationConfigurationDefinition : FormBase
    {
        public PopulationConfigurationDefinition()
        {
            InitializeComponent();
            _dtAgeRange = new DataTable();
        }

        private DataTable _dtAgeRange;
        private DataTable _dtRaces;
        private DataTable _dtGenders;
        private DataTable _dtEthnicity;
        public string _configurationName;
        public object _configurationID;
        private bool isAdd = true;
        private Dictionary<object, string> dicRaces = new Dictionary<object, string>();
        private Dictionary<object, string> dicGenders = new Dictionary<object, string>();
        private Dictionary<object, string> dicEthnicitys = new Dictionary<object, string>();
        private void PopulationConfigurationDefinition_Load(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                string commandText = "select RaceName from Races";
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                lstAvailableRaces.Items.Clear();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    lstAvailableRaces.Items.Add(dr["RaceName"].ToString());
                }
                commandText = "select  GenderName from Genders";
                ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                lstAvailableGrnders.Items.Clear();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    lstAvailableGrnders.Items.Add(dr["GENDERNAME"].ToString());
                }
                commandText = "select  EthnicityName from Ethnicity";
                ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                lstAvailableEthnicity.Items.Clear();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    lstAvailableEthnicity.Items.Add(dr["ETHNICITYNAME"].ToString());
                }
                if (_configurationID != null)
                {
                    isAdd = false;
                    txtConfigName.Text = _configurationName;
                    commandText = string.Format("select Genders.GenderID,GenderName from Genders,PopConfigGenderMap,PopulationConfigurations where (PopulationConfigurations.PopulationConfigurationID=PopConfigGenderMap.PopulationConfigurationID) and (PopConfigGenderMap.GenderID=Genders.GenderID) and PopulationConfigurations.PopulationConfigurationID={0}", _configurationID);
                    ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    _dtGenders = ds.Tables[0];
                    lstGenders.Items.Clear();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        lstGenders.Items.Add(dr["GENDERNAME"].ToString());
                        dicGenders.Add(dr["GenderID"], dr["GENDERNAME"].ToString());
                    }
                    commandText = string.Format("select Races.RaceID,RaceName from Races,PopConfigRaceMap,PopulationConfigurations where (PopulationConfigurations.PopulationConfigurationID=PopConfigRaceMap.PopulationConfigurationID) and (PopConfigRaceMap.RaceID=Races.RaceID) and PopulationConfigurations.PopulationConfigurationID={0}", _configurationID);
                    ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    _dtRaces = ds.Tables[0];
                    lstRaces.Items.Clear();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        lstRaces.Items.Add(dr["RaceName"].ToString());
                        dicRaces.Add(dr["RaceID"], dr["RaceName"].ToString());
                    }
                    commandText = string.Format("select Ethnicity.EthnicityID,EthnicityName from Ethnicity,PopConfigEthnicityMap,PopulationConfigurations where (PopulationConfigurations.PopulationConfigurationID=PopConfigEthnicityMap.PopulationConfigurationID) and (PopConfigEthnicityMap.EthnicityID=Ethnicity.EthnicityID) and PopulationConfigurations.PopulationConfigurationID={0}", _configurationID);
                    ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    _dtEthnicity = ds.Tables[0];
                    lstEthnicities.Items.Clear();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        lstEthnicities.Items.Add(dr["ETHNICITYNAME"].ToString());
                        dicEthnicitys.Add(dr["EthnicityID"], dr["ETHNICITYNAME"].ToString());
                    }
                    commandText = string.Format("select AgeRangeName,StartAge,EndAge from AgeRanges,PopulationConfigurations where (PopulationConfigurations.PopulationConfigurationID=AgeRanges.PopulationConfigurationID) and PopulationConfigurations.PopulationConfigurationID={0}", _configurationID);
                    ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    dgvAgeRangs.DataSource = ds.Tables[0];
                    dgvAgeRangs.Columns[0].HeaderText = "Age Range";
                    dgvAgeRangs.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvAgeRangs.Columns[1].HeaderText = "Start Age";
                    dgvAgeRangs.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvAgeRangs.Columns[2].HeaderText = "End Age";
                    dgvAgeRangs.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvAgeRangs.RowHeadersVisible = false;
                    _dtAgeRange = ds.Tables[0];
                }
                else
                {
                    int number = 0;
                    int PopulationConfigurationID = 0;
                    do
                    {
                        string comText = "select PopulationConfigurationID from PopulationConfigurations where PopulationConfigurationName=" + "'PopulationConfiguration" + Convert.ToString(number) + "'";
                        PopulationConfigurationID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, comText));
                        number++;
                    } while (PopulationConfigurationID > 0);
                    txtConfigName.Text = "PopulationConfiguration" + Convert.ToString(number - 1);
                    commandText = "select max(POPULATIONCONFIGURATIONID) from POPULATIONCONFIGURATIONS";
                    _configurationID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                    commandText = string.Format("select AgeRangeName,StartAge,EndAge from AgeRanges,PopulationConfigurations where (PopulationConfigurations.PopulationConfigurationID=AgeRanges.PopulationConfigurationID) and PopulationConfigurations.PopulationConfigurationName='null'");
                    ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    dgvAgeRangs.DataSource = ds.Tables[0];
                    dgvAgeRangs.Columns[0].HeaderText = "Age Range";
                    dgvAgeRangs.Columns[1].HeaderText = "Start Age";
                    dgvAgeRangs.Columns[2].HeaderText = "End Age";
                    dgvAgeRangs.RowHeadersVisible = false;
                    _dtAgeRange = ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnAddRace_Click(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();

            try
            {
                if (lstAvailableRaces.Items.Count == 0) { return; }
                if (!lstRaces.Items.Contains(lstAvailableRaces.SelectedItem))
                {
                    lstRaces.Items.Add(lstAvailableRaces.SelectedItem);
                    string commandText = string.Format("select RaceID from  Races where RaceName='{0}'", lstAvailableRaces.SelectedItem);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    dicRaces.Add(obj, lstAvailableRaces.SelectedItem.ToString());
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnAddGender_Click(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                if (lstAvailableGrnders.Items.Count == 0) { return; }
                if (!lstGenders.Items.Contains(lstAvailableGrnders.SelectedItem))
                {
                    lstGenders.Items.Add(lstAvailableGrnders.SelectedItem);
                    string commandText = string.Format("select GenderID from  Genders where GenderName='{0}'", lstAvailableGrnders.SelectedItem);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    dicGenders.Add(obj, lstAvailableGrnders.SelectedItem.ToString());
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnAddEthnicity_Click(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                if (lstAvailableEthnicity.Items.Count == 0) { return; }
                if (!lstEthnicities.Items.Contains(lstAvailableEthnicity.SelectedItem))
                {
                    lstEthnicities.Items.Add(lstAvailableEthnicity.SelectedItem);
                    string commandText = string.Format("select EthnicityID from  Ethnicity where EthnicityName='{0}'", lstAvailableEthnicity.SelectedItem);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    dicEthnicitys.Add(obj, lstAvailableEthnicity.SelectedItem.ToString());
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private string _newRaceName;
        private void btnRaceNew_Click(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                NewRace frm = new NewRace();
                DialogResult rtn = frm.ShowDialog();
                if (rtn == DialogResult.OK)
                {
                    _newRaceName = frm._newRaceName;
                    lstRaces.Items.Add(_newRaceName);
                    lstAvailableRaces.Items.Add(_newRaceName);
                    string commandText = string.Format("select RaceID from  Races where RaceName='{0}'", _newRaceName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    dicRaces.Add(obj, _newRaceName);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private string _newGenderName;
        private void btnGendersNew_Click(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                NewGender frm = new NewGender();
                DialogResult rtn = frm.ShowDialog();
                if (rtn == DialogResult.OK)
                {
                    _newGenderName = frm._newGenderName;
                    lstGenders.Items.Add(_newGenderName);
                    lstAvailableGrnders.Items.Add(_newGenderName);
                    string commandText = string.Format("select GenderID from  Genders where GenderName='{0}'", _newGenderName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    dicGenders.Add(obj, _newGenderName);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private string _newEthnicityName;
        private void btnEthnicitiesNew_Click(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                NewEthnicity frm = new NewEthnicity();
                DialogResult rtn = frm.ShowDialog();
                if (rtn == DialogResult.OK)
                {
                    _newEthnicityName = frm._newEthnicityName;
                    lstEthnicities.Items.Add(_newEthnicityName);
                    lstAvailableEthnicity.Items.Add(_newEthnicityName);
                    string commandText = string.Format("select EthnicityID from  Ethnicity where EthnicityName='{0}'", _newEthnicityName);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    dicEthnicitys.Add(obj, _newEthnicityName);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnRaceRemove_Click(object sender, EventArgs e)
        {
            ESILFireBirdHelper fb = new ESILFireBirdHelper();
            try
            {
                if (lstRaces.SelectedItem == null) return;
                string commandText = string.Format("select RaceID from  Races where RaceName='{0}'", lstRaces.SelectedItem);
                object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                lstRaces.Items.Remove(lstRaces.SelectedItem);
                dicRaces.Remove(obj);
                if (_configurationID != null)
                {
                    commandText = "delete from PopConfigRaceMap where RaceID=" + obj + " and PopulationConfigurationID=" + _configurationID + "";
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnGendersRemove_Click(object sender, EventArgs e)
        {
            ESILFireBirdHelper fb = new ESILFireBirdHelper();
            try
            {
                if (lstGenders.SelectedItem == null) return;
                string commandText = string.Format("select GenderID from  Genders where GenderName='{0}'", lstGenders.SelectedItem);
                object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                lstGenders.Items.Remove(lstGenders.SelectedItem);
                dicGenders.Remove(obj);
                if (_configurationID != null)
                {
                    commandText = "delete from PopConfigGenderMap where GenderID=" + obj + " and PopulationConfigurationID=" + _configurationID + "";
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnEthnicitiesRemove_Click(object sender, EventArgs e)
        {
            ESILFireBirdHelper fb = new ESILFireBirdHelper();
            try
            {
                if (lstEthnicities.SelectedItem == null) return;
                string commandText = string.Format("select EthnicityID from  Ethnicity where EthnicityName='{0}'", lstEthnicities.SelectedItem);
                object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                lstEthnicities.Items.Remove(lstEthnicities.SelectedItem);
                dicEthnicitys.Remove(obj);
                if (_configurationID != null)
                {
                    commandText = "delete from PopConfigEthnicityMap where EthnicityID=" + obj + " and PopulationConfigurationID=" + _configurationID + "";
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private int originalHighAge;
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                AgeRangeDefinition frm = new AgeRangeDefinition(_configurationID);

                if (dgvAgeRangs.RowCount == 0)
                {
                    originalHighAge = -1;
                }
                if (dgvAgeRangs.RowCount != 0)
                {
                    int rowCount = _dtAgeRange.Rows.Count - 1;
                    originalHighAge = Convert.ToInt32(_dtAgeRange.Rows[rowCount][2].ToString());
                }

                frm._lowAge = originalHighAge;
                frm._highAge = originalHighAge;
                frm._rowCount = dgvAgeRangs.RowCount;
                DialogResult rtn = frm.ShowDialog();
                if (rtn == DialogResult.OK)
                {
                    foreach (DataRow d in _dtAgeRange.Rows)
                    {
                        if (d[0].ToString() == frm._newAgeRangID.ToString())
                        { MessageBox.Show("This age range name is already in use. Please enter a different name."); return; }
                    }

                    if (dgvAgeRangs.RowCount > 0)
                    {
                        WaitShow("Adding " + frm._newAgeRangID);
                        ESILFireBirdHelper fb = new ESILFireBirdHelper();

                        string commandText = "select AgeRangeID from AgeRanges where AgeRangeName='" + frm._newAgeRangID + "' and POPULATIONCONFIGURATIONID = " + _configurationID + "";
                        object ageRangeID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (ageRangeID != null) //remove from database, if user is deleting after initial entry
                        {
                            commandText = "delete from AgeRanges where AgeRangeID='" + ageRangeID + "' and POPULATIONCONFIGURATIONID = " + _configurationID + "";
                            fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        WaitClose();
                        _dtAgeRange.Rows[dgvAgeRangs.RowCount - 1][0] = _dtAgeRange.Rows[dgvAgeRangs.RowCount - 1][1].ToString() + "TO" + _dtAgeRange.Rows[dgvAgeRangs.RowCount - 1][2].ToString();
                    }

                    DataRow dr = _dtAgeRange.NewRow();
                    dr[0] = frm._newAgeRangID;
                    dr[1] = frm._lowAge;
                    dr[2] = frm._highAge;
                    _dtAgeRange.Rows.Add(dr);
                    dgvAgeRangs.DataSource = _dtAgeRange;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (this.dgvAgeRangs.SelectedRows.Count == 0)
            {
                MessageBox.Show("There is no age range selected to edit.");
            }
            else
            {
                int selection = Convert.ToInt32(this.dgvAgeRangs.SelectedRows[0].Index);    //this section retrieves the current values
                string currName = _dtAgeRange.Rows[selection][0].ToString();
                int currLowAge = Convert.ToInt32(_dtAgeRange.Rows[selection][1]);
                int currHighAge = Convert.ToInt32(_dtAgeRange.Rows[selection][2]);

                AgeRangeDefinition frm = new AgeRangeDefinition(_configurationID);          //a new age definition window is populated with current values
                frm._isEdit = true;
                frm._lowAge = currLowAge;
                frm._highAge = currHighAge;
                frm._rangeName = currName;
                DialogResult rtn = frm.ShowDialog();
                if (rtn == DialogResult.OK)
                {
                    _dtAgeRange.Rows[selection][0] = frm._newAgeRangID;
                    _dtAgeRange.Rows[selection][1] = frm._lowAge;
                    _dtAgeRange.Rows[selection][2] = frm._highAge;
                    this.dgvAgeRangs.DataSource = _dtAgeRange;

                    WaitShow("Editing " + currName);
                    ESILFireBirdHelper fb = new ESILFireBirdHelper();
                    string commandText = "select AgeRangeID from AgeRanges where AgeRangeName='" + currName + "' and POPULATIONCONFIGURATIONID = " + _configurationID + "";
                    object ageRangeID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);

                    if (ageRangeID != null) //remove from database, if user is editing after initial entry
                    {
                        commandText = "delete from AgeRanges where AgeRangeID='" + ageRangeID + "' and POPULATIONCONFIGURATIONID = " + _configurationID + "";
                        fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    WaitClose();
                }
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            ESILFireBirdHelper fb = new ESILFireBirdHelper();
            try
            {
                if (dgvAgeRangs.RowCount == 0)
                {
                    MessageBox.Show("There is no age range to delete.");
                }
                else
                {
                    int selection = Convert.ToInt32(this.dgvAgeRangs.SelectedRows[0].Index);
                    string currName = _dtAgeRange.Rows[selection][0].ToString();
                    string msg = "Delete " + currName + " Age Group?";
                    DialogResult result = MessageBox.Show(msg, "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        WaitShow("Deleting " + currName);

                        string commandText = "select AgeRangeID from AgeRanges where AgeRangeName='" + currName + "' and POPULATIONCONFIGURATIONID = " + _configurationID + "";
                        object ageRangeID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);

                        if (ageRangeID != null) //remove from database, if user is deleting after initial entry
                        {
                            commandText = "delete from AgeRanges where AgeRangeID='" + ageRangeID + "' and POPULATIONCONFIGURATIONID = " + _configurationID + "";
                            fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        }

                        _dtAgeRange.Rows.RemoveAt(selection);
                        WaitClose();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitClose();
                Logger.LogError(ex.Message);
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
            ESILFireBirdHelper fb = new ESILFireBirdHelper();
            string commandText = string.Empty;
            try
            {
                if (txtConfigName.Text == string.Empty) { MessageBox.Show("Please input the population configuration name."); return; }
                if (isAdd)
                {
                    commandText = string.Format("select PopulationConfigurationID from PopulationConfigurations where PopulationConfigurationName='{0}'", txtConfigName.Text);
                    object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (obj != null)
                    {
                        MessageBox.Show("This population configuration name is already in use. Please enter a different name.");
                        return;
                    }
                    //The 'F' is for the locked column in POPULATIONCONFIGURATIONS - this is imported and not predefined
                    commandText = "insert into PopulationConfigurations values (" + _configurationID + ",'" + txtConfigName.Text + "', 'F')";
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                }
                else
                {
                    if (txtConfigName.Text != _configurationName)
                    {
                        commandText = string.Format("select PopulationConfigurationID from PopulationConfigurations where PopulationConfigurationName='{0}'", txtConfigName.Text);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            MessageBox.Show("Please rename the Configuration name.");
                            return;
                        }
                        commandText = "update PopulationConfigurations set PopulationConfigurationName='" + txtConfigName.Text + "' where PopulationConfigurationID=" + _configurationID + "";
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }
                if (_dtRaces != null)
                {
                    for (int i = 0; i < _dtRaces.Rows.Count; i++)
                    {
                        dicRaces.Remove(_dtRaces.Rows[i][0]);

                    }
                }
                foreach (object raceId in dicRaces.Keys)
                {
                    commandText = string.Format("insert into PopConfigRaceMap values ({0},{1})", _configurationID, raceId);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                }

                if (_dtGenders != null)
                {
                    for (int i = 0; i < _dtGenders.Rows.Count; i++)
                    {
                        dicGenders.Remove(_dtGenders.Rows[i][0]);
                    }
                }
                foreach (object genderID in dicGenders.Keys)
                {
                    commandText = string.Format("insert into PopConfigGenderMap values ({0},{1})", _configurationID, genderID);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                }

                if (_dtEthnicity != null)
                {
                    for (int i = 0; i < _dtEthnicity.Rows.Count; i++)
                    {
                        dicEthnicitys.Remove(_dtEthnicity.Rows[i][0]);
                    }
                }
                foreach (object ethnicityID in dicEthnicitys.Keys)
                {
                    commandText = string.Format("insert into PopConfigEthnicityMap values ({0},{1})", _configurationID, ethnicityID);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                }

                for (int i = 1; i < _dtAgeRange.Rows.Count; i++)        // Validation section to check for gaps/overlapping age ranges
                {
                    try
                    {
                        int previous = Convert.ToInt32(_dtAgeRange.Rows[i - 1][2]) + 1;
                        int current = Convert.ToInt32(_dtAgeRange.Rows[i][1]);

                        bool compare = (previous == current);

                        if (!compare)
                        {
                            MessageBox.Show("Please check " + _dtAgeRange.Rows[i - 1][0] + " & " + _dtAgeRange.Rows[i][0] + " for an error in age range data.");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.Message);
                    }
                }

                for (int i = 0; i < _dtAgeRange.Rows.Count; i++)        //Looks for a RangeID based on RangeName and ConfigurationID--if none, adds; otherwise, updates the age values (due to editing)
                {
                    commandText = "select AgeRangeID from AgeRanges where AgeRangeName='" + _dtAgeRange.Rows[i][0].ToString() + "' and POPULATIONCONFIGURATIONID = " + _configurationID + "";
                    object ageRangeID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    if (ageRangeID == null)
                    {
                        commandText = "select max(AGERANGEID) from AGERANGES";
                        ageRangeID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                        commandText = string.Format("insert into AgeRanges values ({0},{1},'{2}',{3},{4})", ageRangeID, _configurationID, _dtAgeRange.Rows[i][0], _dtAgeRange.Rows[i][1], _dtAgeRange.Rows[i][2]);
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                    else
                    {
                        commandText = "update AgeRanges set StartAge='" + _dtAgeRange.Rows[i][1] + "' where AgeRangeID=" + ageRangeID + "";
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        commandText = "update AgeRanges set EndAge='" + _dtAgeRange.Rows[i][2] + "' where AgeRangeID=" + ageRangeID + "";
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

    }
}
