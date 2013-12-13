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
                //加入Race，Gender，Ethnicity的items
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
                //查看和编辑
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
                    dgvAgeRangs.Columns[0].HeaderText = "AgeRange";
                    dgvAgeRangs.Columns[1].HeaderText = "Start Age";
                    dgvAgeRangs.Columns[2].HeaderText = "End Age";
                    dgvAgeRangs.RowHeadersVisible = false;
                    //绑定_dtAgeRange
                    _dtAgeRange = ds.Tables[0];
                }
                else
                {
                    //automatically generated name-increase the number at the end of the name
                    int number = 0;
                    int PopulationConfigurationID = 0;
                    do
                    {
                        string comText = "select PopulationConfigurationID from PopulationConfigurations where PopulationConfigurationName=" + "'PopulationConfiguration" + Convert.ToString(number) + "'";
                        PopulationConfigurationID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, comText));
                        number++;
                    } while (PopulationConfigurationID > 0);
                    txtConfigName.Text = "PopulationConfiguration" + Convert.ToString(number - 1);
                    //txtConfigName.Text = "PopulationConfiguration 0";
                    commandText = "select max(POPULATIONCONFIGURATIONID) from POPULATIONCONFIGURATIONS";
                    _configurationID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                    commandText = string.Format("select AgeRangeName,StartAge,EndAge from AgeRanges,PopulationConfigurations where (PopulationConfigurations.PopulationConfigurationID=AgeRanges.PopulationConfigurationID) and PopulationConfigurations.PopulationConfigurationName='null'");
                    ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    dgvAgeRangs.DataSource = ds.Tables[0];
                    dgvAgeRangs.Columns[0].HeaderText = "AgeRange";
                    dgvAgeRangs.Columns[1].HeaderText = "Start Age";
                    dgvAgeRangs.Columns[2].HeaderText = "End Age";
                    dgvAgeRangs.RowHeadersVisible = false;
                    //绑定_dtAgeRange
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
                //DataRowView drv = lstAvailableRaces.SelectedItem as DataRowView;
                //System.Data.DataRowView drview = (System.Data.DataRowView)lstAvailableRaces.SelectedItem;
                // if (!lstRaces.Items.Contains(drview["RaceName"]))
                // {
                //     lstRaces.Items.Add(drview["RaceName"]);
                // }

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
            //ESILFireBirdHelper fb = new ESILFireBirdHelper();
            //string commandText = string.Empty;
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
                DialogResult rtn = frm.ShowDialog();
                if (rtn == DialogResult.OK)
                {
                    foreach (DataRow d in _dtAgeRange.Rows)
                    {
                        if (d[0].ToString() == frm._newAgeRangID.ToString())
                        { MessageBox.Show("This age range name is already in use. Please enter a different name."); return; }
                    }
                    DataRow dr = _dtAgeRange.NewRow();
                    dr[0] = frm._newAgeRangID;
                    dr[1] = frm._lowAge + 1;
                    dr[2] = frm._highAge;
                    _dtAgeRange.Rows.Add(dr);
                    dgvAgeRangs.DataSource = _dtAgeRange;
                    //commandText = string.Format("select PopulationConfigurationID from PopulationConfigurations where PopulationConfigurationName='{0}'", _configurationName);
                    //object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    //if (obj != null)
                    //{
                    //    _configurationID = obj;
                    //}
                    //if (obj == null)
                    //{
                    //    commandText = "select next value for SEQ_POPULATIONCONFIGURATIONS FROM RDB$DATABASE";
                    //    obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    //    _configurationID = obj;
                    //    commandText = string.Format("insert into PopulationConfigurations ( PopulationConfigurationID,PopulationConfigurationName) values ({0},'{1}')",_configurationID,txtConfigName.Text);
                    //    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    //}
                    //commandText = "select next value for SEQ_AGERANGES FROM RDB$DATABASE";
                    //obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    //commandText = string.Format("insert into AgeRanges values ({0},{1},'{2}',{3},{4})", obj, _configurationID, dr[0], dr[1], dr[2]);
                    //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
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
                    string msg = "Delete the last age range?";
                    DialogResult result = MessageBox.Show(msg, "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        //int rowCount = _dtAgeRange.Rows.Count-1;
                        //originalHighAge = Convert.ToInt32(_dtAgeRange.Rows[rowCount][2].ToString());
                        WaitShow("Deleting the last age range.");
                        dgvAgeRangs.DataSource = _dtAgeRange;
                        if (_configurationID != null)
                        {
                            string commandText = string.Format("delete from AgeRanges where StartAge={0} and PopulationConfigurationID={1} ", _dtAgeRange.Rows[_dtAgeRange.Rows.Count - 1][1], _configurationID);
                            fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                        }
                        _dtAgeRange.Rows.RemoveAt(_dtAgeRange.Rows.Count - 1);
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

        #region 等待窗口
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
        #endregion 等待窗口

        private void btnOK_Click(object sender, EventArgs e)
        {
            ESILFireBirdHelper fb = new ESILFireBirdHelper();
            string commandText = string.Empty;
            //object[] saveID;
            //List<object> saveIDlst = new List<object>();
            try
            {
                //commandText = string.Format("select PopulationConfigurationID from PopulationConfigurations where PopulationConfigurationName='{0}'", _configurationName);
                //object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                //if (obj != null)
                //{
                //    _configurationID = obj;
                //}
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
                    //commandText = "select next value for SEQ_POPULATIONCONFIGURATIONS FROM RDB$DATABASE";
                    //_configurationID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    //obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    //_configurationID = obj;
                    commandText = "insert into PopulationConfigurations values (" + _configurationID + ",'" + txtConfigName.Text + "')";
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
                        commandText = "update PopulationConfigurations set PopulationConfigurationName='" + txtConfigName.Text + "' where PopulationConfigurationID="+_configurationID+"";
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    }
                }
                //save Race
                if (_dtRaces != null)
                {
                    for (int i = 0; i < _dtRaces.Rows.Count; i++)
                    {
                        //if (dicRaces.ContainsKey(_dtRaces.Rows[i][0]))
                        //{
                        //    dicRaces.Remove(_dtRaces.Rows[i][0]);
                        //}
                        dicRaces.Remove(_dtRaces.Rows[i][0]);

                    }
                }
                foreach (object raceId in dicRaces.Keys)
                {
                    //saveIDlst=dicRaces.Keys.ToList<>;
                    commandText = string.Format("insert into PopConfigRaceMap values ({0},{1})", _configurationID, raceId);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                }

                //save Gender
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

                //save Ethnicity
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

                //save AgeRanges
                for (int i = 0; i < _dtAgeRange.Rows.Count; i++)
                {
                    commandText = string.Format("select AgeRangeID from AgeRanges where PopulationConfigurationID={0} and StartAge={1}", _configurationID, _dtAgeRange.Rows[i][1]);
                    object ageRangeID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    //commandText = string.Format("select AgeRangeID from AgeRanges where AgeRangeName='{0}'",);
                    if (ageRangeID == null)
                    {
                        commandText = "select max(AGERANGEID) from AGERANGES";
                        ageRangeID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                        commandText = string.Format("insert into AgeRanges values ({0},{1},'{2}',{3},{4})", ageRangeID, _configurationID, _dtAgeRange.Rows[i][0], _dtAgeRange.Rows[i][1], _dtAgeRange.Rows[i][2]);
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
