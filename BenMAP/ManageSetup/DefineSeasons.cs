using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using ESIL.DBUtility;

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
		public List<ManageSetupSeasonalMetric> _lstSeasonalMetric;

		public Dictionary<string, Season> dicSave = new Dictionary<string, Season>();
		private void DefineSeasons_Load(object sender, EventArgs e)
		{
			ESIL.DBUtility.ESILFireBirdHelper fb = new ESIL.DBUtility.ESILFireBirdHelper();
			try
			{
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
			dtpEndTime.Enabled = true;
			dtpStartTime.Enabled = true;
			try
			{
				int index = 1;
				if (lstSeasons.Items.Count == 0)
				{
					string addSeason = "Season " + index.ToString();
					ok = saveSeasonDic(index); if (!ok) { return; }
					lstSeasons.Items.Add(addSeason);
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
					if (endTime.Month == 12 && endTime.Day == 31)
					{
						MessageBox.Show("Can not add another season because the entire year has already been covered.");
						return;
					}
					else
					{
						if (endTime < startTime)
						{
							MessageBox.Show("Start date must be earlier than end date. Please check the start and end dates.");
							return;
						}
						ok = saveSeasonDic(lstSeasons.Items.Count);
						if (!ok) { return; }
						string addSeason = "Season " + (lstSeasons.Items.Count + 1).ToString();
						lstSeasons.Items.Add(addSeason);
						startTime = endTime.AddDays(1);
						dtpStartTime.Value = startTime;
						dtpEndTime.Value = new DateTime(2011, 12, 31);
						nudownStartHour.Value = 0;
						nudownEndHour.Value = 23;
						nudownNumberofBins.Value = 0;
						ok = saveSeasonDic(lstSeasons.Items.Count);
						if (!ok) { return; }
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
				sd.StartDay = dtpStartTime.Value.DayOfYear - 1;
				sd.EndDay = dtpEndTime.Value.DayOfYear - 1;
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
					sd.PollutantSeasonID = dicSave[tmpkey].PollutantSeasonID;
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
				int endDay = sourceSeason.EndDay;
				DateTime dt2 = new DateTime(2011, 1, 1);
				dt2 = dt2.AddDays(endDay);
				dtpEndTime.Value = dt2;
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
			ESILFireBirdHelper fb = new ESILFireBirdHelper();
			string commandText;

			//Check start/end of the global season against the seasonal metric seasons to ensure that seasonal metrics lie within global season
			int pollutantStart = 366;
			int pollutantEnd = 0;

			foreach (KeyValuePair<string,Season> globalSeason in dicSave)
			{
				if (globalSeason.Value.StartDay < pollutantStart)
					pollutantStart = globalSeason.Value.StartDay;

				if (globalSeason.Value.EndDay > pollutantEnd)
					pollutantEnd = globalSeason.Value.EndDay;
			}

			if (_lstSeasonalMetric.Count > 0)
			{
				foreach (ManageSetupSeasonalMetric seasonalMetric in _lstSeasonalMetric)
				{
					if (seasonalMetric.Seasons.Count > 0)
					{
						int seasonStart = 366;
						int seasonEnd = 0;

						//check each season to find the start/end date of the seasonal metric
						foreach (SeasonalMetricSeason season in seasonalMetric.Seasons)
						{
							if (season.StartDay < seasonStart)
								seasonStart = season.StartDay;
							if (season.EndDay > seasonEnd)
								seasonEnd = season.EndDay;
						}

						//if the start or end falls outside the global pollutant season, allow user to delete the seasonal metric seasons or to reset the global pollutant season
						if (pollutantStart > seasonStart || pollutantEnd < seasonEnd)
						{
							DialogResult dr = MessageBox.Show(string.Format("The date range for the {0} seasonal metric falls outside the current pollutant season. Continuing with this change will delete all seasons currently set for this seasonal metric.\n\nDo you want to proceed?", seasonalMetric.SeasonalMetricName), "Seasonal Metric Season", MessageBoxButtons.YesNo);

							if (dr == DialogResult.Yes)
							{
								foreach (SeasonalMetricSeason season in seasonalMetric.Seasons)
								{
									commandText = "delete from SeasonalMetricSeasons where SeasonalMetricSeasonID=" + season.SeasonalMetricSeasonID + "";
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
								}

								seasonalMetric.Seasons = new List<SeasonalMetricSeason>();
							}
							else
							{
								if (pollutantStart > seasonStart)
									dtpStartTime.Value = new DateTime(2011, 1, 1).AddDays(seasonStart);
								if (pollutantEnd < seasonEnd)
									dtpEndTime.Value = new DateTime(2011, 1, 1).AddDays(seasonEnd);

								return;
							}
						}
					}
				}

			}
			try
			{
				if (lstSeasons.Items.Count != 0)
				{
					lstSeasons.SelectedIndex = lstSeasons.Items.Count - 1;
					bool ok = saveSeasonDic(lstSeasons.Items.Count);
					if (!ok) { return; }
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
										//These codes seem never excecute because mssm.Seasons[mssm.Seasons.Count - 1].PollutantSeasonID is either -1 or null
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
					DateTime dtStart = new DateTime(2011, 1, 1);
					DateTime dtEnd = new DateTime(2011, 1, 1);
					dtStart = dtStart.AddDays(dicSave[str].StartDay);
					dtEnd = dtEnd.AddDays(dicSave[str].EndDay);
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
						dtpEndTime.Enabled = false;
						dtpStartTime.Enabled = false;
						btnDelete.Enabled = false;
					}
					else
					{
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
				if (lstSeasons.Items.Count > 0)
				{
					if (lstSeasons.Items.Count == 1)
					{
						MessageBox.Show("There must be at least one season defined for each pollutant", "Error", MessageBoxButtons.OK);
						return;
					}
					string str = lstSeasons.GetItemText(lstSeasons.SelectedItem);
					int checkPolSeasonID = dicSave[str].PollutantSeasonID;
					dicSave.Remove(str);
					lstSeasons.Items.Remove(str);
					commandText = "Delete from PollutantSeasons where PollutantSeasonID=" + checkPolSeasonID + "";
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

					if (lstSeasons.Items.Count == 0)
					{
						btnDelete.Enabled = false;
						dtpStartTime.Enabled = false;
						dtpEndTime.Enabled = false;
						dtpStartTime.Value = new DateTime(2011, 1, 1);
						dtpEndTime.Value = new DateTime(2011, 12, 31);
					}
					else
					{
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