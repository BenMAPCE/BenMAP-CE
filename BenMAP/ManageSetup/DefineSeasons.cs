using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace BenMAP
{
    public partial class DefineSeasons : FormBase
    {
        int newPollutantSeasonID = 0;

        public DefineSeasons()
        {
            InitializeComponent();
        }
        private bool _isAddPollutant;
        public DefineSeasons(bool isAdd)
        {
            InitializeComponent();
            _isAddPollutant = isAdd;
        }

        public object _pollutantID;        
        public  Dictionary<string, Season> dicSave = new Dictionary<string, Season>();
        private void DefineSeasons_Load(object sender, EventArgs e)
        {
            ESIL.DBUtility.ESILFireBirdHelper fb = new ESIL.DBUtility.ESILFireBirdHelper();
            try
            {
                //---------test----
                dtpStartTime.CustomFormat = "MMMM dd";
                dtpEndTime.CustomFormat = "MMMM dd";
                dtpStartTime.MinDate = new DateTime(0001, 1, 1);
                dtpStartTime.MaxDate = new DateTime(9990, 12, 31);
                dtpEndTime.MinDate = new DateTime(0001, 1, 1);
                dtpEndTime.MaxDate = new DateTime(9990, 12, 31);
                if (dicSave.Keys.Count != 0)
                {
                    foreach (string season in dicSave.Keys)
                    {                        
                        lstSeasons.Items.Add(season);
                    }
                    //lstSeasons.DataSource = dicSave.Keys;
                    lstSeasons.SelectedIndex = 0;
                }
                else
                {
                    this.dtpStartTime.Value = new DateTime(2011, 1, 1);
                    this.dtpEndTime.Value = new DateTime(2011, 12, 31);
                    nudownStartHour.Value = 0;
                    nudownEndHour.Value = 23;
                    nudownNumberofBins.Value = 0;
                }

                if (lstSeasons.Items.Count == 0)
                {
                    btnDelete.Enabled = false;
                    //nudownEndHour.Enabled = false;
                    //nudownNumberofBins.Enabled = false;
                    //nudownStartHour.Enabled = false;
                    dtpEndTime.Enabled = false;
                    dtpStartTime.Enabled = false;
                }
                else 
                {
                    if (lstSeasons.Items.Count == 1)
                    {
                        btnDelete.Enabled = true;
                    }
                    else
                    {
                        btnDelete.Enabled = false;
                    }
                }

                //nudownStartHour.Minimum = 0;
                //nudownStartHour.Maximum = 23;

                //nudownEndHour.Minimum = 0;
                //nudownEndHour.Maximum = 23;

                //nudownNumberofBins.Minimum = 0;
                //nudownNumberofBins.Maximum = 365;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            lstSeasons.SelectedIndex = lstSeasons.Items.Count - 1;
            bool ok;
            btnDelete.Enabled = true;
            //nudownEndHour.Enabled = true;
            //nudownNumberofBins.Enabled = true;
            //nudownStartHour.Enabled = true;
            dtpEndTime.Enabled = true;
            dtpStartTime.Enabled = true;
            try
            {
                int index = 1;
                if (lstSeasons.Items.Count == 0)
                {
                    string addSeason = "Season " + index.ToString();
                    //lstSeasons.Items.Add(new ListItem("", addSeason));
                    ok=saveSeasonDic(index);//保存season1 中的月份
                    if (!ok) { return; }
                    //lstSeasons.DataSource = null;
                    lstSeasons.Items.Add(addSeason);
                    //lstSeasons.DataSource = dicSave.Keys;
                    lstSeasons.SelectedIndex = lstSeasons.Items.Count - 1;
                    return;
                }
                else
                {
                    if (lstSeasons.Items.Count > 1)
                    {
                        if (dtpStartTime.Value.DayOfYear <= dicSave["Season " + Convert.ToString(lstSeasons.Items.Count - 1)].EndDay + 1)
                        {
                            dtpStartTime.Value = new DateTime(2011, 1, 1).AddDays(dicSave["Season " + Convert.ToString(lstSeasons.Items.Count - 1)].EndDay + 1);
                            MessageBox.Show("Seasons can not overlap.");
                            return;
                        }
                    }
                    string endSeason = "Season " + lstSeasons.Items.Count.ToString();
                    if (lstSeasons.SelectedIndex + 1 < lstSeasons.Items.Count)
                    {
                        DateTime dtEnd = new DateTime();
                        dtEnd.AddDays(dicSave[endSeason].EndDay);
                        if (dtEnd.Month == 12 && dtEnd.Day == 31)
                        {
                            MessageBox.Show("Can not add another season because the entire year has already been covered.");
                            return;
                        }
                    }
                    DateTime startTime = dtpStartTime.Value;
                    DateTime endTime = dtpEndTime.Value;
                    if (endTime.Month == 12 && endTime.Day == 31)//为12月31号
                    {
                        MessageBox.Show("Can not add another season because the entire year has already been covered.");
                        return;
                    }
                    else
                    {
                        //先比较endTime是否小于startTime
                        if (endTime < startTime)// 如果小于
                        {
                            MessageBox.Show("Start date must be earlier than end date. Please check the start and end dates.");
                            return;
                        }
                        //然后保存前一个season
                        ok= saveSeasonDic(lstSeasons.Items.Count);
                        if (!ok) { return; }
                        //saveSeasonDic(i - 1);
                        //再加一个season进去
                        string addSeason = "Season " + (lstSeasons.Items.Count + 1).ToString();
                        lstSeasons.Items.Add(addSeason);
                        //再变化开始日期，变为原来结束时间加一天
                        startTime = endTime.AddDays(1);
                        dtpStartTime.Value = startTime;
                        //结束时间变为十二月三十一号
                        dtpEndTime.Value = new DateTime(2011, 12, 31);
                        nudownStartHour.Value = 0;
                        nudownEndHour.Value = 23;
                        nudownNumberofBins.Value = 0;
                        //再保存新加进去的season
                        ok=saveSeasonDic(lstSeasons.Items.Count);
                        if (!ok) { return; }
                        //lstSeasons.DataSource = null;
                        //lstSeasons.Items.Clear();
                        //lstSeasons.DataSource = dicSave.Keys;
                        lstSeasons.SelectedIndex = lstSeasons.Items.Count - 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private bool saveSeasonDic(int index)
        {
            ESIL.DBUtility.ESILFireBirdHelper fb = new ESIL.DBUtility.ESILFireBirdHelper();
            try
            {
                Season sd = new Season();
                sd.StartDay = dtpStartTime.Value.DayOfYear-1;
                sd.EndDay = dtpEndTime.Value.DayOfYear-1;
                sd.StartHour = Convert.ToInt32(nudownStartHour.Value);
                if (Convert.ToInt32(nudownEndHour.Value) == 23)
                {
                    sd.EndHour = Convert.ToInt32(nudownEndHour.Value) + 1;
                }
                else
                {
                    sd.EndHour = Convert.ToInt32(nudownEndHour.Value);
                }
                if (sd.StartHour >= sd.EndHour)
                {
                    MessageBox.Show("Start hour must be earlier than end hour."); return false;
                }
                sd.Numbins = Convert.ToInt32(nudownNumberofBins.Value);
                string tmpkey = "Season " + index.ToString();
                if (dicSave.ContainsKey(tmpkey))
                {
                    sd.PollutantSeasonID=dicSave[tmpkey].PollutantSeasonID;
                    dicSave[tmpkey] = sd;
                }
                else
                {
                    string commandText = "select max(PollutantSeasonID) from PollutantSeasons";
                    newPollutantSeasonID++;
                    object pollutantSeasonID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    sd.PollutantSeasonID = Convert.ToInt32(pollutantSeasonID) + newPollutantSeasonID;
                    dicSave.Add(tmpkey, sd);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return false;
            }
        }

        private void getMonthAndDay(Season sourceSeason)
        {
            try
            {
                int startDay = sourceSeason.StartDay;
                DateTime dt = new DateTime(2011, 1, 1);
                dt = dt.AddDays(startDay);
                dtpStartTime.Value = dt;
                //nudStartMonth.Value = dt.Month;
                //nudStartDay.Value = dt.Day;
                int endDay = sourceSeason.EndDay;
                DateTime dt2 = new DateTime(2011, 1, 1);
                dt2 = dt2.AddDays(endDay);
                dtpEndTime.Value = dt2;
                //nudEndMonth.Value = dt2.Month;
                //nudEndDay.Value = dt2.Day;
                nudownStartHour.Value = sourceSeason.StartHour;
                nudownEndHour.Value = sourceSeason.EndHour;
                nudownNumberofBins.Value = sourceSeason.Numbins;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lstSeasons.Items.Count > 1 && lstSeasons.SelectedIndex == lstSeasons.Items.Count - 1)
            {
                if (dtpStartTime.Value.DayOfYear <= dicSave["Season " + Convert.ToString(lstSeasons.Items.Count - 1)].EndDay + 1)
                {
                    dtpStartTime.Value = new DateTime(2011, 1, 1).AddDays(dicSave["Season " + Convert.ToString(lstSeasons.Items.Count - 1)].EndDay + 1);
                    MessageBox.Show("Seasons can not overlap.");
                    return;
                }
            }
            if (dtpEndTime.Value.DayOfYear < dtpStartTime.Value.DayOfYear)
            {
                MessageBox.Show("Start date must be earlier than end date. Please check the start and end dates.");
                return;
            }
            ESIL.DBUtility.ESILFireBirdHelper fb = new ESIL.DBUtility.ESILFireBirdHelper();
            string commandText = String.Empty;
            try
            {
                if (lstSeasons.Items.Count != 0)
                {
                    lstSeasons.SelectedIndex = lstSeasons.Items.Count - 1;
                    //string lastseason=lstSeasons.GetItemText(lstSeasons.Items[lstSeasons.Items.Count-1]);
                   bool ok= saveSeasonDic(lstSeasons.Items.Count);
                   if (!ok) { return; }
                    //majie---------------1022
                   if (!_isAddPollutant)
                   {
                       foreach (string season in dicSave.Keys)
                       {
                           commandText = string.Format("select pollutantID from PollutantSeasons where PollutantSeasonID={0}", dicSave[season].PollutantSeasonID);
                           object checkSeason = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                           if (checkSeason != null)
                           {
                               commandText = string.Format("update  PollutantSeasons set PollutantID={0} , StartDay={1} , EndDay={2} , StartHour={3} , EndHour={4} , Numbins={5} where PollutantSeasonID ={6}", _pollutantID, dicSave[season].StartDay, dicSave[season].EndDay, dicSave[season].StartHour, dicSave[season].EndHour, dicSave[season].Numbins, dicSave[season].PollutantSeasonID);
                               fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                           }
                           else
                           {
                               commandText = string.Format("insert into PollutantSeasons values ({0},{1},{2},{3},{4},{5},{6})", dicSave[season].PollutantSeasonID, _pollutantID, dicSave[season].StartDay, dicSave[season].EndDay, dicSave[season].StartHour, dicSave[season].EndHour, dicSave[season].Numbins);
                               fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                           }
                       }
                   }

                    //由于season可能做过更改，重新对每个seasonalmetric的最后一个seasonalmetricseason的时间段赋值
                   foreach (ManageSetupSeasonalMetric mssm in ManageSeasonalMetrics.LstSMetrics)
                   {
                       if (mssm.Seasons != null)
                       {
                           if (mssm.Seasons.Count > 0)
                           {
                               foreach (string season in dicSave.Keys)
                               {
                                   if (mssm.Seasons[mssm.Seasons.Count - 1].PollutantSeasonID == dicSave[season].PollutantSeasonID)
                                   {
                                       mssm.Seasons[mssm.Seasons.Count - 1].StartDay = dicSave[season].StartDay;
                                       mssm.Seasons[mssm.Seasons.Count - 1].EndDay = dicSave[season].EndDay;
                                       break;
                                   }
                               }
                           }
                       }
                   }

                   foreach (string season in dicSave.Keys)
                   {
                       commandText = string.Format("select SeasonalMetricSeasonID from SeasonalMetricSeasons where PollutantSeasonID={0}", dicSave[season].PollutantSeasonID);
                       object checkSeason = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                       if (checkSeason != null)
                       {
                           commandText = string.Format("update SeasonalMetricSeasons set StartDay={0} , EndDay={1} where PollutantSeasonID ={2}", dicSave[season].StartDay, dicSave[season].EndDay, dicSave[season].PollutantSeasonID);
                           fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                       }
                   }

                    //---end
                    //foreach (ListItem lst in lstSeasons.Items)
                    //{
                    //    string item = lst.Name;
                    //    //int startMonth = dicSave[item].Month1;
                    //    //int startDay = (int)dicSave[item].Day1;
                    //    //DateTime dt = new DateTime(2011, startMonth, startDay);
                    //    //int startDayCount = dt.DayOfYear - 1;
                    //    //int endMonth = dicSave[item].Month2;
                    //    //int endDay = (int)dicSave[item].Day2;
                    //    //dt = new DateTime(2011, endMonth, endDay);
                    //    //int endDayCount = dt.DayOfYear - 1;
                    //    int startDayCount = dicSave[item].dtStart.DayOfYear - 1;
                    //    int endDayCount = dicSave[item].dtEnd.DayOfYear - 1;
                    //    if (lst.ID == string.Empty)
                    //    {
                    //        string commandText = "select next value for SEQ_POLLUTANTSEASONS FROM RDB$DATABASE";
                    //        object pollutantSeasonID = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    //        commandText = string.Format("insert into PollutantSeasons values ({0},{1},{2},{3},{4},{5},{6})", pollutantSeasonID, _pollutantID, startDayCount, endDayCount, dicSave[item].StartHour, dicSave[item].EndHour, dicSave[item].Numbins);
                    //        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    //    }
                    //    else
                    //    {
                    //        string commandText = string.Format("update  PollutantSeasons set PollutantID={0} , StartDay={1} , EndDay={2} , StartHour={3} , EndHour={4} , Numbins={5} where PollutantSeasonID ={6}", _pollutantID, startDayCount, endDayCount, dicSave[item].StartHour, dicSave[item].EndHour, dicSave[item].Numbins, lst.ID);
                    //        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    //    }
                    //}
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        object tmpitem;
        private void lstSeasons_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //如果上次选择的item是最后一个，就保存一下
                if (tmpitem == lstSeasons.Items[lstSeasons.Items.Count - 1])
                {
                    if (lstSeasons.Items.Count > 1)
                    {
                        if (dtpStartTime.Value.DayOfYear <= dicSave["Season " + Convert.ToString(lstSeasons.Items.Count - 1)].EndDay + 1)
                        {
                            lstSeasons.SelectedIndex = lstSeasons.Items.Count - 1;
                            dtpStartTime.Value = new DateTime(2011, 1, 1).AddDays(dicSave["Season " + Convert.ToString(lstSeasons.Items.Count - 1)].EndDay + 1);
                            return;
                        }
                    }
                    if (dtpEndTime.Value.DayOfYear < dtpStartTime.Value.DayOfYear)
                    {
                        return;
                    }
                    saveSeasonDic(lstSeasons.Items.Count);
                }
                tmpitem = lstSeasons.SelectedItem;
                if (lstSeasons.SelectedItem != null)
                {
                    string str = lstSeasons.GetItemText(lstSeasons.SelectedItem);
                    DateTime dtStart = new DateTime(2011,1,1);
                    DateTime dtEnd = new DateTime(2011,1,1);
                    dtStart=dtStart.AddDays(dicSave[str].StartDay);
                    dtEnd=dtEnd.AddDays(dicSave[str].EndDay);
                    dtpStartTime.Value = dtStart;
                    dtpEndTime.Value = dtEnd;
                    nudownStartHour.Value = dicSave[str].StartHour;
                    if (dicSave[str].EndHour == 24)
                    {
                        nudownEndHour.Value = dicSave[str].EndHour - 1;
                    }
                    else
                    {
                        nudownEndHour.Value = dicSave[str].EndHour;
                    }
                    nudownNumberofBins.Value = dicSave[str].Numbins;
                    if (lstSeasons.SelectedIndex != lstSeasons.Items.Count - 1)
                    {
                        //nudownEndHour.Enabled = false;
                        //nudownNumberofBins.Enabled = false;
                        //nudownStartHour.Enabled = false;
                        dtpEndTime.Enabled = false;
                        dtpStartTime.Enabled = false;
                        btnDelete.Enabled = false;
                    }
                    else
                    {
                        //nudownEndHour.Enabled = true;
                        //nudownNumberofBins.Enabled = true;
                        //nudownStartHour.Enabled = true;
                        dtpEndTime.Enabled = true;
                        dtpStartTime.Enabled = true;
                        btnDelete.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            try
            {
                string commandText = "";
                //if (lstSeasons.Items.Count == 0)
                //{ MessageBox.Show("No season to delete."); return; }
                if (lstSeasons.Items.Count > 0)
                {
                    string str = lstSeasons.GetItemText(lstSeasons.SelectedItem);
                    int checkPolSeasonID = dicSave[str].PollutantSeasonID;
                    //ListItem ltm = lstSeasons.SelectedItem as ListItem;
                    //if (ltm.ID == string.Empty)
                    //{
                    //    lstSeasons.Items.RemoveAt(lstSeasons.SelectedIndex);
                    //    dicSave.Remove(lstSeasons.GetItemText(lstSeasons.SelectedItem));
                    //}
                    //else
                    //{
                    //commandText = "select SeasonalMetricSeasonID from SeasonalMetricSeasons where PollutantSeasonID=" + checkPolSeasonID + "";
                    //object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                    //if (obj != null)
                    //{
                    //    DialogResult dr = MessageBox.Show("This season is used by a defined seasonal metric. Confirm deletion?", "Confirm deletion", MessageBoxButtons.OKCancel);
                    //    if (dr == DialogResult.Cancel)
                    //    {
                    //        return;
                    //    }
                    //    else
                    //    {
                    //        commandText = "delete from SeasonalMetricSeasons where PollutantSeasonID=" + checkPolSeasonID + "";
                    //        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    //    }
                    //}
                    //else
                    //{
                    //    bool confirmdelete = false;
                    //    foreach (ManageSetupSeasonalMetric mssm in ManageSeasonalMetrics.LstSMetrics)
                    //    {
                    //        if (mssm.Seasons != null)
                    //        {
                    //            foreach (SeasonalMetricSeason sms in mssm.Seasons)
                    //            {
                    //                if (sms.PollutantSeasonID == checkPolSeasonID)
                    //                {
                    //                    DialogResult dr = MessageBox.Show("This season is used by a defined seasonal metric. Confirm deletion?", "Confirm deletion", MessageBoxButtons.OKCancel);
                    //                    if (dr == DialogResult.Cancel)
                    //                    {
                    //                        return;
                    //                    }
                    //                    else
                    //                    {
                    //                        confirmdelete = true;
                    //                        break;
                    //                    }
                    //                }
                    //            }
                    //        }
                    //        if (confirmdelete) break;
                    //    }
                    //    //删掉所有跟这个season有关的seasonalmetricseason
                    //    foreach (ManageSetupSeasonalMetric mssm in ManageSeasonalMetrics.LstSMetrics)
                    //    {
                    //        if (mssm.Seasons != null)
                    //        {
                    //            for (int i = mssm.Seasons.Count - 1; i >= 0; i--)
                    //            {
                    //                if (mssm.Seasons[i].PollutantSeasonID == checkPolSeasonID)
                    //                {
                    //                    mssm.Seasons.RemoveAt(i);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //lstSeasons.Items.RemoveAt(lstSeasons.SelectedIndex);
                    //dicSave.Remove(lstSeasons.GetItemText(lstSeasons.SelectedItem));
                    dicSave.Remove(str);
                    lstSeasons.Items.Remove(str);
                    commandText = "Delete from PollutantSeasons where PollutantSeasonID=" + checkPolSeasonID + "";
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

                    if (lstSeasons.Items.Count == 0)
                    {
                        btnDelete.Enabled = false;
                        dtpStartTime.Enabled = false;
                        dtpEndTime.Enabled = false;
                        //nudownEndHour.Enabled = false;
                        //nudownNumberofBins.Enabled = false;
                        //nudownStartHour.Enabled = false;
                        dtpStartTime.Value = new DateTime(2011, 1, 1);
                        dtpEndTime.Value = new DateTime(2011, 12, 31);
                    }
                    else
                    {
                        //lstSeasons.DataSource = null;
                        //lstSeasons.Items.Clear();
                        //lstSeasons.DataSource = dicSave.Keys;
                        lstSeasons.SelectedIndex = lstSeasons.Items.Count - 1;
                    }
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

        private void lstSeasons_MouseClick(object sender, MouseEventArgs e)
        {
            if (dtpEndTime.Value.DayOfYear < dtpStartTime.Value.DayOfYear)
            {
                lstSeasons.SelectedItem = lstSeasons.Items[lstSeasons.Items.Count - 1];
                MessageBox.Show("Start date must be earlier than end date. Please check the start and end dates.");
                return;
            }
        }

        private void nudownStartHour_Leave(object sender, EventArgs e)
        {
            if (lstSeasons.SelectedItem == null) return;
            if (Convert.ToInt16(nudownStartHour.Value) >= Convert.ToInt16(nudownEndHour.Value))
            {
                MessageBox.Show("Start hour must be earlier than end hour.");
                nudownStartHour.Value = 0;
            }
            else
            {
                dicSave[lstSeasons.SelectedItem.ToString()].StartHour = Convert.ToInt16(nudownStartHour.Value);
            }
        }

        private void nudownEndHour_Leave(object sender, EventArgs e)
        {
            if (lstSeasons.SelectedItem == null) return;
            if (Convert.ToInt16(nudownStartHour.Value) >= Convert.ToInt16(nudownEndHour.Value))
            {
                MessageBox.Show("Start hour must be earlier than end hour.");
                nudownEndHour.Value = 23;
            }
            else
            {
                dicSave[lstSeasons.SelectedItem.ToString()].EndHour = Convert.ToInt16(nudownEndHour.Value);
            }
        }

        private void nudownNumberofBins_Leave(object sender, EventArgs e)
        {
            if (lstSeasons.SelectedItem == null) return;
            dicSave[lstSeasons.SelectedItem.ToString()].Numbins = Convert.ToInt16(nudownNumberofBins.Value);
        }
    }
}