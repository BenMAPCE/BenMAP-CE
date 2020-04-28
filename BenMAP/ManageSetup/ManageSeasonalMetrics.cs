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
	public partial class ManageSeasonalMetrics : FormBase
	{
		int seasonalmetricidadd = 0;
		private static List<ManageSetupSeasonalMetric> _lstSMetrics = new List<ManageSetupSeasonalMetric>();
		public static List<ManageSetupSeasonalMetric> LstSMetrics
		{
			get { return _lstSMetrics; }
			set { _lstSMetrics = value; }
		}
		public List<SeasonalMetricSeason> _lstSMSeasons = new List<SeasonalMetricSeason>();
		public Metric _metric = new Metric();
		public ManageSetupSeasonalMetric _seasonalMetric;
		public SeasonalMetricSeason _seasonalMetricSeason;
		private bool _isAddPollutant;

		int newSeasonalMetricSeasonID = 0;

		int flagStartTime = 0;
		int flagEndTime = 0;

		public ManageSeasonalMetrics()
		{
			InitializeComponent();
		}


		public ManageSeasonalMetrics(List<ManageSetupSeasonalMetric> LstSMetric, Metric metric, bool isAdd)
		{
			InitializeComponent();
			_lstSMetrics = LstSMetric;
			_isAddPollutant = isAdd;
			if (_lstSMetrics.Count != 0) { _metric = _lstSMetrics[0].Metric; }
			else
			{
				_metric = metric;
			}
			dtpStartTime.CustomFormat = "MMMM dd";
			dtpEndTime.CustomFormat = "MMMM dd";
			dtpStartTime.MinDate = new DateTime(0001, 1, 1);
			dtpStartTime.MaxDate = new DateTime(9990, 12, 31);
			dtpEndTime.MinDate = new DateTime(0001, 1, 1);
			dtpEndTime.MaxDate = new DateTime(9990, 12, 31);
		}

		private void ManageSeasonalMetrics_Load(object sender, EventArgs e)
		{
			try
			{
				cboStatisticFunction.SelectedIndex = 0;

				txtMetricIDName.Text = _metric.MetricName;
				if (_lstSMetrics.Count != 0)
				{
					txtSeasonMetricName.Enabled = true;
					for (int i = 0; i < _lstSMetrics.Count; i++)
					{
						lstSeasonalMetrics.Items.Add(new ListItem(_lstSMetrics[i].SeasonalMetricID.ToString(), _lstSMetrics[i].SeasonalMetricName));
					}
					lstSeasonalMetrics.SelectedIndex = 0;
				}
				else
				{
					txtSeasonMetricName.Enabled = false;
					btnDeleteSeasonalMetricSeason.Enabled = false;
					dtpStartTime.Value = new DateTime(2011, 1, 1);
					dtpEndTime.Value = new DateTime(2011, 12, 31);
					dtpStartTime.Enabled = false;
					dtpEndTime.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		bool isRefrest = true;
		private void RefrestFileList()
		{
			for (int i = 0; i < lstSeasonalMetrics.Items.Count; i++)
				lstSeasonalMetrics.Items[i] = lstSeasonalMetrics.Items[i];
		}

		private void lstSeasonalMetrics_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (lstSeasonalMetrics.Items.Count == 0) return;
				if (!isRefrest) return;
				if (_seasonalMetric != null)
				{
					for (int i = 0; i < _lstSMetrics.Count; i++)
					{
						if (_lstSMetrics[i].SeasonalMetricID == _seasonalMetric.SeasonalMetricID)
						{
							_lstSMetrics[i] = updateSeasonalMetric(_seasonalMetric);
							break;
						}
					}
				}
				ListItem lt = (lstSeasonalMetrics.SelectedIndex < 0 ? lstSeasonalMetrics.Items[lstSeasonalMetrics.Items.Count - 1] : lstSeasonalMetrics.Items[lstSeasonalMetrics.SelectedIndex]) as ListItem;
				_seasonalMetric = (from p in _lstSMetrics where p.SeasonalMetricID.ToString() == lt.ID select p).ToList()[0];
				txtSeasonMetricName.Text = _seasonalMetric.SeasonalMetricName;
				_lstSMSeasons = _seasonalMetric.Seasons;
				lstSeasonalMetricSeasons.Items.Clear();
				if (_seasonalMetric.Seasons != null)
				{
					_seasonalMetric.Seasons.Sort((x, y) => x.StartDay < y.StartDay ? -1 : 0);
					for (int i = 1; i <= _seasonalMetric.Seasons.Count; i++)
					{
						lstSeasonalMetricSeasons.Items.Add(new ListItem(_seasonalMetric.Seasons[i - 1].SeasonalMetricSeasonID.ToString(), "Season " + i));
					}

					if (lstSeasonalMetricSeasons.Items.Count > 0)
					{
						lstSeasonalMetricSeasons.SelectedIndex = lstSeasonalMetricSeasons.Items.Count - 1;
					}
				}
				if (lstSeasonalMetricSeasons.Items.Count <= 0)
				{
					dtpStartTime.Value = new DateTime(2011, 1, 1);
					dtpEndTime.Value = new DateTime(2011, 12, 31);
					dtpStartTime.Enabled = false;
					dtpEndTime.Enabled = false;
					btnDeleteSeasonalMetricSeason.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}


		private void setMonthAndDay(SeasonalMetricSeason season)
		{
			DateTime dt = new DateTime(2011, 1, 1);
			dt = dt.AddDays(Convert.ToInt32(season.StartDay));
			dtpStartTime.Value = dt;
			DateTime dt2 = new DateTime(2011, 1, 1);
			dt2 = dt2.AddDays(Convert.ToInt32(season.EndDay));
			dtpEndTime.Value = dt2;
		}


		private ManageSetupSeasonalMetric updateSeasonalMetric(ManageSetupSeasonalMetric sMetric)
		{
			try
			{
				ManageSetupSeasonalMetric sMetric1 = sMetric;
				sMetric1.SeasonalMetricName = sMetric.SeasonalMetricName;
				sMetric1.Seasons = _lstSMSeasons;
				return sMetric1;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
				return null;
			}
		}

		private SeasonalMetricSeason updateSMSeason(SeasonalMetricSeason sMSeason)
		{
			try
			{
				SeasonalMetricSeason sMSeason1 = sMSeason;
				switch (tabMetricFunction.SelectedIndex)
				{
					case 0:
						sMSeason1.SeasonalMetricType = 0;
						sMSeason1.MetricFunction = cboStatisticFunction.GetItemText(cboStatisticFunction.SelectedItem);
						break;
					case 1:
						sMSeason1.SeasonalMetricType = 1;
						sMSeason1.MetricFunction = txtFunctionManage.Text;
						break;
				}
				return sMSeason1;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
				return null;
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			string commandText = string.Empty;
			try
			{
				if (_seasonalMetricSeason != null && _lstSMSeasons != null)
				{
					for (int i = 0; i < _lstSMSeasons.Count; i++)
					{
						if (_lstSMSeasons[i].SeasonalMetricSeasonID == _seasonalMetricSeason.SeasonalMetricSeasonID)
						{
							_lstSMSeasons[i] = updateSMSeason(_seasonalMetricSeason);
						}
					}
				}
				if (!isRefrest) return;
				if (_seasonalMetric != null)
				{
					for (int i = 0; i < _lstSMetrics.Count; i++)
					{
						if (_lstSMetrics[i].SeasonalMetricID == _seasonalMetric.SeasonalMetricID)
						{
							_lstSMetrics[i] = updateSeasonalMetric(_seasonalMetric);
						}
					}
				}

				if (!_isAddPollutant)
				{
					for (int i = 0; i < _lstSMetrics.Count; i++)
					{
						commandText = "select SeasonalMetricName from SeasonalMetrics where metricID=" + _metric.MetricID + "";
						DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
						if (ds.Tables[0].Rows.Count != 0)
						{
							for (int k = 0; k < ds.Tables[0].Rows.Count; k++)
							{
								if (ds.Tables[0].Rows[k].ToString() == _lstSMetrics[i].SeasonalMetricName)
								{ MessageBox.Show("This seasonal metric name is already in use. Please enter a different name."); return; }
							}
						}

						commandText = "select SeasonalMetricName from SeasonalMetrics where SeasonalMetricID=" + _lstSMetrics[i].SeasonalMetricID + " and MetricID=" + _lstSMetrics[i].Metric.MetricID + "";
						object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
						if (obj == null)
						{
							commandText = string.Format("insert into SeasonalMetrics values ({0},{1},'{2}')", _lstSMetrics[i].SeasonalMetricID, _lstSMetrics[i].Metric.MetricID, _lstSMetrics[i].SeasonalMetricName);
							fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
						}
						else
						{
							commandText = "update SeasonalMetrics set SeasonalMetricName='" + _lstSMetrics[i].SeasonalMetricName + "' where SeasonalMetricID=" + _lstSMetrics[i].SeasonalMetricID + " ";
							fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
						}
						if (_lstSMetrics[i].Seasons != null)
						{
							for (int j = 0; j < _lstSMetrics[i].Seasons.Count; j++)
							{
								commandText = "select startDay from SeasonalMetricSeasons where SeasonalMetricSeasonID=" + _lstSMetrics[i].Seasons[j].SeasonalMetricSeasonID + "";
								obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
								if (obj == null)
								{
									commandText = string.Format("insert into SeasonalMetricSeasons values ({0},{1},{2},{3},{4},'{5}',{6})", _lstSMetrics[i].Seasons[j].SeasonalMetricSeasonID, _lstSMetrics[i].Seasons[j].SeasonalMetricID, _lstSMetrics[i].Seasons[j].StartDay, _lstSMetrics[i].Seasons[j].EndDay, _lstSMetrics[i].Seasons[j].SeasonalMetricType, _lstSMetrics[i].Seasons[j].MetricFunction, _lstSMetrics[i].Seasons[j].PollutantSeasonID);
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
								}
								else
								{
									commandText = string.Format("update SeasonalMetricSeasons set Startday={0}, Endday={1}, Seasonalmetrictype={2}, MetricFunction = '{3}' where SeasonalMetricSeasonID={4}", _lstSMetrics[i].Seasons[j].StartDay, _lstSMetrics[i].Seasons[j].EndDay, _lstSMetrics[i].Seasons[j].SeasonalMetricType, _lstSMetrics[i].Seasons[j].MetricFunction, _lstSMetrics[i].Seasons[j].SeasonalMetricSeasonID);
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
								}
							}
						}
					}
				}
				else
				{
				}
				this.DialogResult = DialogResult.OK;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}


		private void lstSeasonalMetricSeasons_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (sender == null || lstSeasonalMetricSeasons.SelectedItem == null) return;
				ListItem lt = lstSeasonalMetricSeasons.SelectedItem as ListItem;
				_seasonalMetricSeason = (from p in _lstSMSeasons where p.SeasonalMetricSeasonID.ToString() == lt.ID select p).ToList()[0];
				setMonthAndDay(_seasonalMetricSeason);
				switch (_seasonalMetricSeason.SeasonalMetricType)
				{
					case 0:
						tabMetricFunction.SelectedTab = tbpStatistic;
						cboStatisticFunction.Text = _seasonalMetricSeason.MetricFunction;
						break;
					case 1:
						tabMetricFunction.SelectedTab = tbpCustomFunction;
						txtFunctionManage.Text = _seasonalMetricSeason.MetricFunction;
						break;
				}
				if (lstSeasonalMetricSeasons.SelectedIndex == lstSeasonalMetricSeasons.Items.Count - 1)
				{
					dtpStartTime.Enabled = true;
					dtpEndTime.Enabled = true;
					btnDeleteSeasonalMetricSeason.Enabled = true;
				}
				else
				{
					dtpStartTime.Enabled = false;
					dtpEndTime.Enabled = false;
					btnDeleteSeasonalMetricSeason.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		int _iSeasonalMetric = 0;
		private void btnAdd_Click(object sender, EventArgs e)
		{
			try
			{
				if (lstSeasonalMetrics.SelectedItem != null) { _seasonalMetric = updateSeasonalMetric(_seasonalMetric); }
				ManageSetupSeasonalMetric addSMetric = new ManageSetupSeasonalMetric();
				FireBirdHelperBase fb = new ESILFireBirdHelper();
				string commandText = "select max(SeasonalMetricID) from SeasonalMetrics";
				seasonalmetricidadd++;
				addSMetric.SeasonalMetricID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + seasonalmetricidadd;
				if (_lstSMetrics.Count == 0) { addSMetric.Metric = _metric; txtSeasonMetricName.Enabled = true; }
				else { addSMetric.Metric = _lstSMetrics[0].Metric; }
				bool checkExist = true;
				if (lstSeasonalMetrics.Items.Count > 0)
				{
					while (checkExist)
					{
						foreach (ListItem it in lstSeasonalMetrics.Items)
						{
							if (it.ToString() == "SeasonalMetric " + _iSeasonalMetric.ToString() + "")
							{
								_iSeasonalMetric++;
								checkExist = true;
								break;
							}
							else
							{
								checkExist = false;
							}
						}
					}
				}
				addSMetric.SeasonalMetricName = "SeasonalMetric " + _iSeasonalMetric;
				txtSeasonMetricName.Text = "SeasonalMetric " + _iSeasonalMetric;
				_lstSMetrics.Add(addSMetric);
				lstSeasonalMetrics.Items.Add(new ListItem(addSMetric.SeasonalMetricID.ToString(), addSMetric.SeasonalMetricName));
				_seasonalMetric = null;
				lstSeasonalMetrics.SelectedIndex = lstSeasonalMetrics.Items.Count - 1;
				_iSeasonalMetric++;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}







		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (lstSeasonalMetrics.SelectedItem == null) return;
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			string commandText = string.Empty;
			try
			{
				if (lstSeasonalMetrics.Items.Count == 0)
				{
					MessageBox.Show("There is no seasonal metric to delete."); return;
				}
				commandText = "select MonitorID from MonitorEntries where SeasonalMetricID=" + _seasonalMetric.SeasonalMetricID + "";
				object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
				if (obj != null)
				{
					MessageBox.Show("This seasonal metric is used in 'Monitor Datasets'. Please delete monitor datasets that use this seasonal metric first.");
					return;
				}
				DialogResult rtn = MessageBox.Show("All Seasons associated with this seasonal metric will be deleted. Are you sure you want to delete this seasonal metric?", "", MessageBoxButtons.OKCancel);
				if (rtn == DialogResult.OK)
				{
					for (int i = 0; i < _lstSMetrics.Count; i++)
					{
						if (_lstSMetrics[i].SeasonalMetricID == _seasonalMetric.SeasonalMetricID)
						{
							_lstSMetrics.RemoveAt(i);
						}
					}
					commandText = "delete from SeasonalMetrics where SeasonalMetricID=" + _seasonalMetric.SeasonalMetricID + "";
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
					commandText = "delete from SeasonalMetricSeasons where SeasonalMetricID=" + _seasonalMetric.SeasonalMetricID + "";
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
					_seasonalMetric = null;
					lstSeasonalMetrics.Items.RemoveAt(lstSeasonalMetrics.SelectedIndex);
					if (lstSeasonalMetrics.Items.Count == 0)
					{
						lstSeasonalMetricSeasons.Items.Clear();
						txtSeasonMetricName.Text = "";
						txtSeasonMetricName.Enabled = false;
					}
					else
					{
						lstSeasonalMetrics.SelectedIndex = lstSeasonalMetrics.Items.Count - 1;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}



		private void lstFunctions_DoubleClick(object sender, EventArgs e)
		{
			string msg = "";
			try
			{
				if (lstFunctions == null || lstFunctions.Items.Count == 0)
				{
					msg = "No function can be selected";
					return;
				}
				Dictionary<string, string> varDic = new Dictionary<string, string>();
				varDic.Add("ABS(x)", "ABS()");
				varDic.Add("EXP(x)", "EXP()");
				varDic.Add("IPOWER(x,y)", "IPOWER(,)");
				varDic.Add("LN(x)", "LN()");
				varDic.Add("POWER(x,y)", "POWER(,)");
				varDic.Add("SQR(x)", "SQR()");
				varDic.Add("SQRT(x)", "SQRT()");
				string insertedFun = string.Empty;
				string tag = lstFunctions.SelectedItem.ToString();
				insertedFun = varDic[tag];
				string fun = string.Empty;
				string oldFun = txtFunctionManage.Text;
				int oldInsertIndex = txtFunctionManage.SelectionStart;
				int index = lstFunctions.SelectedItem.ToString().IndexOf("(") + 1;
				if (txtFunctionManage.Text == string.Empty)
				{
					txtFunctionManage.Text = insertedFun;
					txtFunctionManage.SelectionStart = index;
					txtFunctionManage.Focus();
				}
				else
				{
					Console.WriteLine(oldInsertIndex + "\n" + index);
					string strFun = txtFunctionManage.Text.Insert(oldInsertIndex, insertedFun);
					txtFunctionManage.Text = strFun;
					index += oldInsertIndex;
					txtFunctionManage.SelectionStart = index;
					txtFunctionManage.Focus();
				}

			}
			catch (Exception ex)
			{
				CommonClass.Equals(ex, ex);
			}
			finally
			{
				if (msg != "")
				{
					MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
		}

		private void lstVariables_DoubleClick(object sender, EventArgs e)
		{
			string msg = "";
			try
			{
				if (lstVariables == null || lstVariables.Items.Count == 0)
				{
					msg = "No varible can be selected";
					return;
				}
				string lstVar = lstVariables.SelectedItem.ToString();
				int varindex = lstVar.IndexOf("[");
				int oldInsertIndex = txtFunctionManage.SelectionStart;
				if (txtFunctionManage.Text == string.Empty)
				{
					if (lstVar == "MetricValues[]" || lstVar == "SeasonalMetricValues[]" || lstVar == "SortedMetricValues[]")
					{
						txtFunctionManage.Text = lstVar;
						txtFunctionManage.SelectionStart = varindex + 1;
						txtFunctionManage.Focus();
					}
					else
					{
						txtFunctionManage.Text = lstVar;
						txtFunctionManage.SelectionStart = txtFunctionManage.Text.Length;
						txtFunctionManage.Focus();
					}
				}
				else
				{

					string lstVarFun = txtFunctionManage.Text.Insert(oldInsertIndex, lstVar);

					if (lstVar == "MetricValues[]" || lstVar == "SeasonalMetricValues[]" || lstVar == "SortedMetricValues[]")
					{
						txtFunctionManage.Text = lstVarFun;
						varindex += oldInsertIndex + 1;
						txtFunctionManage.SelectionStart = varindex;
						txtFunctionManage.Focus();
					}
					else
					{
						txtFunctionManage.Text = lstVarFun;
						oldInsertIndex += lstVariables.Text.Length;
						txtFunctionManage.SelectionStart = oldInsertIndex;
						txtFunctionManage.Focus();
					}
				}
			}
			catch (Exception ex)
			{
				CommonClass.Equals(ex, ex);
			}
			finally
			{
				if (msg != "")
				{
					MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
		}

		private void txtSeasonMetricName_Leave(object sender, EventArgs e)
		{
			if (txtSeasonMetricName.Text.Trim() == "")
			{
				if (_seasonalMetric != null)
					txtSeasonMetricName.Text = _seasonalMetric.SeasonalMetricName;
				return;
			}
			bool sameName = false;
			int j = 1;
			string seasonmetricName = txtSeasonMetricName.Text;
			do
			{
				sameName = false;
				for (int i = 0; i < _lstSMetrics.Count; i++)
				{
					if (_lstSMetrics[i].SeasonalMetricID != _seasonalMetric.SeasonalMetricID)
					{
						if (_lstSMetrics[i].SeasonalMetricName == seasonmetricName)
						{
							sameName = true;
							seasonmetricName = txtSeasonMetricName.Text + "_" + j++;
							break;
						}
					}
				}
			} while (sameName);
			txtSeasonMetricName.Text = seasonmetricName;
			_seasonalMetric.SeasonalMetricName = seasonmetricName;

			foreach (ListItem l in lstSeasonalMetrics.Items)
			{
				if (l.ID == _seasonalMetric.SeasonalMetricID.ToString())
				{
					l.Name = txtSeasonMetricName.Text;
					isRefrest = false;
					RefrestFileList();
					isRefrest = true;
					break;
				}
			}

		}

		private void txtSeasonMetricName_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				txtSeasonMetricName_Leave(sender, e);
			}
		}

		private void tabMetricFunction_Leave(object sender, EventArgs e)
		{
			try
			{
				if (_seasonalMetricSeason != null)
				{
					for (int i = 0; i < _lstSMSeasons.Count; i++)
					{
						if (_lstSMSeasons[i].SeasonalMetricSeasonID == _seasonalMetricSeason.SeasonalMetricSeasonID)
						{
							_lstSMSeasons[i] = updateSMSeason(_seasonalMetricSeason);
						}
					}
				}
			}
			catch
			{ }
		}

		private void tabMetricFunction_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (tabMetricFunction.SelectedIndex)
			{
				case 0:
					cboStatisticFunction.SelectedIndex = 0;
					break;
				case 1:
					txtFunctionManage.Text = "";
					break;
			}
		}

		private void btnAddSeasonalMetricSeason_Click(object sender, EventArgs e)
		{
			try
			{
				if (lstSeasonalMetrics.Items.Count <= 0)
				{
					MessageBox.Show("Please define the seasonal metric first.", "", MessageBoxButtons.OK);
					return;
				}
				lstSeasonalMetricSeasons.SelectedIndex = lstSeasonalMetricSeasons.Items.Count - 1;
				if (flagStartTime == 0) dtpStartTime_Leave(sender, e);
				if (flagEndTime == 0) dtpEndTime_Leave(sender, e);
				if (flagStartTime == 2 || flagEndTime == 2)
				{
					flagStartTime = 0;
					flagEndTime = 0;
					return;
				}
				flagStartTime = 0;
				flagEndTime = 0;
				DateTime endTime = dtpEndTime.Value;
				if (endTime.Month == 12 && endTime.Day == 31 && lstSeasonalMetricSeasons.Items.Count > 0)
				{
					MessageBox.Show("Can not add another season because the entire year has already been covered.");
					return;
				}
				FireBirdHelperBase fb = new ESILFireBirdHelper();
				if (lstSeasonalMetrics.SelectedItem == null) { return; }
				ListItem ltSMetric = lstSeasonalMetrics.SelectedItem as ListItem;
				if (lstSeasonalMetricSeasons.Items.Count == 0)
				{
					dtpStartTime.Enabled = true;
					dtpEndTime.Enabled = true;
				}
				SeasonalMetricSeason sms = new SeasonalMetricSeason();
				sms.SeasonalMetricID = Convert.ToInt32(ltSMetric.ID);
				string commandText = "select max(SeasonalMetricSeasonID) from SeasonalMetricSeasons";
				newSeasonalMetricSeasonID++;
				sms.SeasonalMetricSeasonID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + newSeasonalMetricSeasonID;
				int i = lstSeasonalMetricSeasons.Items.Count + 1;
				lstSeasonalMetricSeasons.Items.Add(new ListItem(sms.SeasonalMetricSeasonID.ToString(), "Season " + i));
				dtpStartTime.Value = endTime.AddDays(1);
				dtpEndTime.Value = new DateTime(2011, 12, 31);
				sms.StartDay = dtpStartTime.Value.DayOfYear - 1;
				sms.EndDay = 364;
				tabMetricFunction.SelectedIndex = 0;
				sms.SeasonalMetricType = tabMetricFunction.SelectedIndex;
				cboStatisticFunction.SelectedIndex = 0;
				sms.MetricFunction = cboStatisticFunction.Text;
				txtFunctionManage.Text = "";

				if (_lstSMSeasons == null) { _lstSMSeasons = new List<SeasonalMetricSeason>(); }
				_lstSMSeasons.Add(sms);
				_seasonalMetric.Seasons = _lstSMSeasons;
				lstSeasonalMetricSeasons.SelectedIndex = lstSeasonalMetricSeasons.Items.Count - 1;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void btnDeleteSeasonalMetricSeason_Click(object sender, EventArgs e)
		{
			if (lstSeasonalMetrics.Items.Count <= 0 || lstSeasonalMetricSeasons.SelectedItem == null) return;
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			string commandText = string.Empty;
			try
			{
				DialogResult result = MessageBox.Show("Are you sure you want to delete this season?", "", MessageBoxButtons.OKCancel);
				if (result == DialogResult.OK)
				{
					ListItem smsItem = lstSeasonalMetricSeasons.SelectedItem as ListItem;

					lstSeasonalMetricSeasons.Items.RemoveAt(lstSeasonalMetricSeasons.SelectedIndex);
					commandText = "delete from SeasonalMetricSeasons where SeasonalMetricSeasonID=" + smsItem.ID + "";
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
					for (int i = 0; i < _lstSMSeasons.Count; i++)
					{
						if (_lstSMSeasons[i].SeasonalMetricSeasonID.ToString() == smsItem.ID)
						{
							_lstSMSeasons.RemoveAt(i);
						}
					}

					_seasonalMetricSeason = null;
					if (lstSeasonalMetricSeasons.Items.Count > 0)
					{ lstSeasonalMetricSeasons.SelectedIndex = lstSeasonalMetricSeasons.Items.Count - 1; }
					else
					{
						dtpStartTime.Value = new DateTime(2011, 1, 1);
						dtpEndTime.Value = new DateTime(2011, 12, 31);
						dtpStartTime.Enabled = false;
						dtpEndTime.Enabled = false;
						btnDeleteSeasonalMetricSeason.Enabled = false;
					}
				}
				else { return; }

			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}

		}

		private void dtpStartTime_Leave(object sender, EventArgs e)
		{
			try
			{
				if (_seasonalMetricSeason == null || lstSeasonalMetricSeasons.Items.Count <= 0) return;
				tabMetricFunction.Focus(); if (dtpEndTime.Value.DayOfYear <= dtpStartTime.Value.DayOfYear)
				{
					MessageBox.Show("Start date must be earlier than end date. Please check the start and end dates.");
					dtpStartTime.Value = new DateTime(2011, 1, 1).AddDays(_seasonalMetricSeason.StartDay);
					flagStartTime = 2;
					return;
				}
				if (lstSeasonalMetricSeasons.Items.Count <= 1)
					_seasonalMetricSeason.StartDay = dtpStartTime.Value.DayOfYear - 1;
				else
				{
					ListItem lseason = lstSeasonalMetricSeasons.Items[lstSeasonalMetricSeasons.Items.Count - 2] as ListItem;
					SeasonalMetricSeason lastsms = (from p in _lstSMSeasons where p.SeasonalMetricSeasonID.ToString() == lseason.ID select p).ToList()[0];
					if (dtpStartTime.Value.DayOfYear <= lastsms.EndDay + 1)
					{
						MessageBox.Show("Seasons can not overlap.");
						dtpStartTime.Value = new DateTime(2011, 1, 1).AddDays(_seasonalMetricSeason.StartDay);
						return;
					}
					else
						_seasonalMetricSeason.StartDay = dtpStartTime.Value.DayOfYear - 1;
				}
				flagStartTime = 1;
			}
			catch
			{ }
		}

		private void dtpEndTime_Leave(object sender, EventArgs e)
		{
			try
			{
				if (_seasonalMetricSeason == null || lstSeasonalMetricSeasons.Items.Count <= 0) return;
				tabMetricFunction.Focus(); if (dtpEndTime.Value.DayOfYear <= dtpStartTime.Value.DayOfYear)
				{
					MessageBox.Show("Start date must be earlier than end date. Please check the start and end dates.");
					dtpEndTime.Value = new DateTime(2011, 1, 1).AddDays(_seasonalMetricSeason.EndDay);
					flagEndTime = 2;
					return;
				}
				_seasonalMetricSeason.EndDay = dtpEndTime.Value.DayOfYear - 1;
				flagEndTime = 1;
			}
			catch
			{ }
		}
	}
}
