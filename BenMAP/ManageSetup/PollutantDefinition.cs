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
	public partial class PollutantDefinition : FormBase
	{
		public PollutantDefinition()
		{
			InitializeComponent();
		}
		int metricidadd = 0;
		bool wrongStartEndHour = false;
		public string _pollutantIDName = "";
		public object _pollutantID;
		private int _addMetricNumber = 0;
		BenMAPPollutant _benMAPPollutant = new BenMAPPollutant();
		public Metric _metric;
		private bool _isAddPollutant;

		public List<Season> _lstSeason = new List<Season>();

		public List<Metric> _lstMetrics = new List<Metric>();

		public List<ManageSetupSeasonalMetric> _lstAllSeasonalMetric = new List<ManageSetupSeasonalMetric>();

		public List<ManageSetupSeasonalMetric> _lstSeasonalMetric = new List<ManageSetupSeasonalMetric>();

		Dictionary<string, Season> _dicSeasons = new Dictionary<string, Season>();
		public List<ManageSetupSeasonalMetric> getAllSeasonalMetrics()
		{
			try
			{
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				List<ManageSetupSeasonalMetric> lstSeasonalMetric = new List<ManageSetupSeasonalMetric>();
				string commandText = string.Format("select SeasonalMetricID from SeasonalMetrics where MetricID in (select MetricID from Metrics where PollutantID={0})", _pollutantID);
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				foreach (DataRow drSeasonalMetric in ds.Tables[0].Rows)
				{
					lstSeasonalMetric.Add(getManageSetupSeasonalMetric(Convert.ToInt32(drSeasonalMetric["SeasonalMetricID"])));
				}
				return lstSeasonalMetric;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
				return null;
			}
		}

		public static ManageSetupSeasonalMetric getManageSetupSeasonalMetric(int SeasonalMetricID)
		{
			try
			{
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				string commandText = string.Format("select MetricID,SeasonalMetricName from SeasonalMetrics where SeasonalMetricID={0}", SeasonalMetricID);
				System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				DataRow dr = ds.Tables[0].Rows[0];
				ManageSetupSeasonalMetric seasonalMetric = new ManageSetupSeasonalMetric();
				seasonalMetric.SeasonalMetricID = SeasonalMetricID;
				seasonalMetric.Metric = Grid.GridCommon.getMetricFromID(Convert.ToInt32(dr["MetricID"]));
				seasonalMetric.SeasonalMetricName = dr["SeasonalMetricName"].ToString();
				seasonalMetric.Seasons = new List<SeasonalMetricSeason>();
				commandText = string.Format("select -1 as PollutantSeasonID,StartDay,EndDay,SeasonalMetricSeasonID,SeasonalMetricID,SeasonalMetricType,MetricFunction from SeasonalMetricSeasons where SeasonalMetricID={0}", SeasonalMetricID);
				ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				foreach (DataRow drSeason in ds.Tables[0].Rows)
				{
					SeasonalMetricSeason season = new SeasonalMetricSeason()
					{
						PollutantSeasonID = Convert.ToInt32(drSeason["PollutantSeasonID"]),
						StartDay = Convert.ToInt32(drSeason["StartDay"]),
						EndDay = Convert.ToInt32(drSeason["EndDay"]),
						SeasonalMetricID = Convert.ToInt32(drSeason["SeasonalMetricID"]),
						SeasonalMetricSeasonID = Convert.ToInt32(drSeason["SeasonalMetricSeasonID"]),
						SeasonalMetricType = Convert.ToInt32(drSeason["SeasonalMetricType"]),
						MetricFunction = drSeason["MetricFunction"].ToString()
					};
					seasonalMetric.Seasons.Add(season);
				}
				return seasonalMetric;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				return null;
			}
		}

		private void PollutantDefinition_Load(object sender, EventArgs e)
		{
			try
			{
				cbohourlymetricgeneration.SelectedIndex = 0;
				if (_pollutantID != null)
				{
					_benMAPPollutant = Grid.GridCommon.getPollutantFromID(Convert.ToInt32(_pollutantID));
					txtPollutantID.Text = _benMAPPollutant.PollutantName;
					cboObservationType.SelectedIndex = (int)_benMAPPollutant.Observationtype;
					_lstAllSeasonalMetric = getAllSeasonalMetrics();
					for (int i = 0; i < _benMAPPollutant.Metrics.Count; i++)
					{
						ListItem lt = new ListItem(_benMAPPollutant.Metrics[i].MetricID.ToString(), _benMAPPollutant.Metrics[i].MetricName);
						lstMetrics.Items.Add(lt);
					}
					_lstMetrics = _benMAPPollutant.Metrics;
					if (lstMetrics.Items.Count != 0)
					{
						lstMetrics.SelectedIndex = 0;
					}
					_lstSeason = _benMAPPollutant.Seasons;
					if (_lstSeason.Count != 0)
					{
						for (int i = 0; i < _lstSeason.Count; i++)
						{
							_dicSeasons.Add("Season " + (i + 1).ToString(), _lstSeason[i]);
						}
					}
					_isAddPollutant = false;
				}
				else
				{
					_benMAPPollutant = new BenMAPPollutant();
					FireBirdHelperBase fb = new ESILFireBirdHelper();
					string commandText = "select max(POLLUTANTID) from POLLUTANTS";
					_pollutantID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;

					int number = 0;
					int PollutantID = 0;
					do
					{
						string comText = "select PollutantID from pollutants where pollutantName=" + "'Pollutant " + Convert.ToString(number) + "'";
						PollutantID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, comText));
						number++;
					} while (PollutantID > 0);
					txtPollutantID.Text = "Pollutant " + Convert.ToString(number - 1);

					_benMAPPollutant.PollutantID = Convert.ToInt32(_pollutantID);
					_benMAPPollutant.PollutantName = "Pollutant 0";
					cboObservationType.SelectedIndex = 0;
					_isAddPollutant = true;

					//For all new pollutant, add a whole year as the first pollutant season.
					Season defaultSeason = new Season();
					commandText = "select max(PollutantSeasonID) from PollutantSeasons";
					defaultSeason.PollutantSeasonID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
					defaultSeason.PollutantID = _benMAPPollutant.PollutantID;
					defaultSeason.StartDay = 0;
					defaultSeason.EndDay = 364;
					defaultSeason.StartHour = 0;
					defaultSeason.EndHour = 24;
					defaultSeason.Numbins = 0;
					_dicSeasons.Add("Season 1", defaultSeason);

				}


				if (lstMetrics.Items.Count == 0)
				{
					txtMetricName.Enabled = false;
					cbohourlymetricgeneration.Enabled = false;
					tabHourlyMetricGeneration.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void updateBenMAPPollutant(BenMAPPollutant benMapPollutant)
		{
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			string commandText = string.Empty;
			customFunctionInvalid = false;
			try
			{
				commandText = "select PollutantName from Pollutants where PollutantID=" + _benMAPPollutant.PollutantID + "";
				object checkName = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
				if (checkName != null)
				{
					commandText = string.Format("update Pollutants set PollutantName='{0}',ObservationType={1} where PollutantID={2}", txtPollutantID.Text, cboObservationType.SelectedIndex, _benMAPPollutant.PollutantID);
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
				}
				else
				{
					commandText = string.Format("select pollutantID from pollutants where PollutantName='{0}' and setupID={1}", txtPollutantID.Text, CommonClass.ManageSetup.SetupID);
					object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
					if (obj != null) { MessageBox.Show("This pollutant name is already is use. Please enter a different name."); PollutantExist = true; return; }
					else PollutantExist = false;
					commandText = string.Format("insert into Pollutants Values ({0},{1},'{2}',{3})", _benMAPPollutant.PollutantID, CommonClass.ManageSetup.SetupID, txtPollutantID.Text, cboObservationType.SelectedIndex);
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
				}
				if (_isAddPollutant)
				{
					foreach (string season in _dicSeasons.Keys)
					{
						commandText = "select pollutantID from PollutantSeasons where PollutantSeasonID=" + _dicSeasons[season].PollutantSeasonID + "";
						object checkSeason = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
						if (checkSeason != null)
						{
							commandText = string.Format("update  PollutantSeasons set PollutantID={0} , StartDay={1} , EndDay={2} , StartHour={3} , EndHour={4} , Numbins={5} where PollutantSeasonID ={6}", _pollutantID, _dicSeasons[season].StartDay, _dicSeasons[season].EndDay, _dicSeasons[season].StartHour, _dicSeasons[season].EndHour, _dicSeasons[season].Numbins, _dicSeasons[season].PollutantSeasonID);
							fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
						}
						else
						{
							commandText = string.Format("insert into PollutantSeasons values ({0},{1},{2},{3},{4},{5},{6})", _dicSeasons[season].PollutantSeasonID, _pollutantID, _dicSeasons[season].StartDay, _dicSeasons[season].EndDay, _dicSeasons[season].StartHour, _dicSeasons[season].EndHour, _dicSeasons[season].Numbins);
							fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
						}
					}
				}

				for (int i = 0; i < _lstMetrics.Count; i++)
				{
					commandText = "select MetricName from Metrics where MetricID=" + _lstMetrics[i].MetricID + "";
					object metricName = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);

					FixedWindowMetric fix = new FixedWindowMetric();
					MovingWindowMetric mov = new MovingWindowMetric();
					CustomerMetric cus = new CustomerMetric();
					if (_lstMetrics[i] is FixedWindowMetric)
					{
						fix = _lstMetrics[i] as FixedWindowMetric;
					}
					else if (_lstMetrics[i] is MovingWindowMetric)
					{
						mov = _lstMetrics[i] as MovingWindowMetric;
					}
					else if (_lstMetrics[i] is CustomerMetric)
					{
						cus = _lstMetrics[i] as CustomerMetric;
					}
					if (metricName == null)
					{
						if (cboObservationType.SelectedIndex == 1)
						{
							commandText = string.Format("Insert into Metrics Values ({0},{1},'{2}',0)", _lstMetrics[i].MetricID, _pollutantID, _lstMetrics[i].MetricName);
							fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
						}
						if (cboObservationType.SelectedIndex == 0)
						{
							switch (_lstMetrics[i].HourlyMetricGeneration)
							{
								case 0:
									commandText = string.Format("Insert into Metrics Values ({0},{1},'{2}',1)", _lstMetrics[i].MetricID, _pollutantID, _lstMetrics[i].MetricName);
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
									break;
								case 1:
									commandText = string.Format("Insert into Metrics Values ({0},{1},'{2}',1)", fix.MetricID, _pollutantID, fix.MetricName);
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
									commandText = string.Format("insert into FixedWindowMetrics values ({0},{1},{2},{3})", fix.MetricID, fix.StartHour, fix.EndHour, (int)fix.Statistic);
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
									break;
								case 2:
									commandText = string.Format("Insert into Metrics Values ({0},{1},'{2}',2)", mov.MetricID, _pollutantID, mov.MetricName);
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
									commandText = string.Format("insert into MovingWindowMetrics values ({0},{1},{2},{3})", mov.MetricID, mov.WindowSize, (int)mov.WindowStatistic, (int)mov.DailyStatistic);
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
									break;
								case 3:
									List<Tuple<string, int>> lstSystemVariableName = Configuration.ConfigurationCommonClass.getAllSystemVariableNameList();
									Dictionary<string, double> dicVariable = new Dictionary<string, double>();
									foreach (Tuple<string, int> tuple in lstSystemVariableName)
									{
										dicVariable.Add(tuple.Item1, 1);
									}
									string valueFunctionText = APVX.APVCommonClass.getFunctionStringFromDatabaseFunction(txtFunctionManage.Text);
									double valueFunctionResult = APVX.APVCommonClass.getValueFromValuationFunctionString(valueFunctionText, 1, 1, 1, 1, 1, 1, 1, 1, dicVariable);
									if (cus.MetricFunction == string.Empty || cus.MetricFunction.Trim() == "" || valueFunctionResult == -999999999.0)
									{
										MessageBox.Show("Please enter a valid value for 'Function' of " + _lstMetrics[i].MetricName + ".");
										customFunctionInvalid = true;
										return;
									}
									else customFunctionInvalid = false;
									commandText = string.Format("Insert into Metrics Values ({0},{1},'{2}',3)", cus.MetricID, _pollutantID, cus.MetricName);
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
									commandText = string.Format("insert into CustomMetrics values ({0},'{1}') ", cus.MetricID, cus.MetricFunction);
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
									break;
							}
						}
					}
					else
					{
						commandText = string.Format("update Metrics set MetricName='{0}' where MetricID={1}", _lstMetrics[i].MetricName, _lstMetrics[i].MetricID);
						fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

						if (cboObservationType.SelectedIndex == 0)
						{
							switch (_lstMetrics[i].HourlyMetricGeneration)
							{

								case 0:
									break;
								case 1:
									commandText = "select count(*) from FixedWindowMetrics where metricid=" + fix.MetricID + "";
									object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
									if (Convert.ToInt32(obj) == 0)
									{
										commandText = string.Format("insert into FixedWindowMetrics values ({0},{1},{2},{3})", fix.MetricID, fix.StartHour, fix.EndHour, (int)fix.Statistic);
									}
									else
									{
										commandText = string.Format("update FixedWindowMetrics set StartHour={0},EndHour={1},Statistic={2} where MetricID={3}", fix.StartHour, fix.EndHour, (int)fix.Statistic, fix.MetricID);
									}
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
									commandText = "update Metrics set HOURLYMETRICGENERATION=1 where metricID=" + fix.MetricID + "";
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
									break;
								case 2:
									commandText = "select count(*) from MovingWindowMetrics where metricid=" + mov.MetricID + "";
									obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
									if (Convert.ToInt32(obj) == 0)
									{
										commandText = string.Format("insert into MovingWindowMetrics values ({0},{1},{2},{3})", mov.MetricID, mov.WindowSize, (int)mov.WindowStatistic, (int)mov.DailyStatistic);
									}
									else
									{
										commandText = string.Format("update MovingWindowMetrics set WindowSize={0},WindowStatistic={1},DailyStatistic={2} where MetricID={3}", mov.WindowSize, (int)mov.WindowStatistic, (int)mov.DailyStatistic, mov.MetricID);
									}
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
									commandText = "update Metrics set HOURLYMETRICGENERATION=2 where metricID=" + mov.MetricID + "";
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
									break;
								case 3:
									List<Tuple<string, int>> lstSystemVariableName = Configuration.ConfigurationCommonClass.getAllSystemVariableNameList();
									Dictionary<string, double> dicVariable = new Dictionary<string, double>();
									foreach (Tuple<string, int> tuple in lstSystemVariableName)
									{
										dicVariable.Add(tuple.Item1, 1);
									}
									string valueFunctionText = APVX.APVCommonClass.getFunctionStringFromDatabaseFunction(txtFunctionManage.Text);
									double valueFunctionResult = APVX.APVCommonClass.getValueFromValuationFunctionString(valueFunctionText, 1, 1, 1, 1, 1, 1, 1, 1, dicVariable);
									if (cus.MetricFunction == string.Empty || cus.MetricFunction.Trim() == "" || valueFunctionResult == -999999999.0)
									{
										MessageBox.Show("Please input a valid value into 'Function' of " + _lstMetrics[i].MetricName + ".");
										customFunctionInvalid = true;
										return;
									}
									else customFunctionInvalid = false;
									commandText = "select count(*) from CustomMetrics where metricid=" + cus.MetricID + "";
									obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
									if (Convert.ToInt32(obj) == 0)
									{
										commandText = string.Format("insert into CustomMetrics values ({0},'{1}') ", cus.MetricID, cus.MetricFunction);
									}
									else
									{
										commandText = string.Format("update CustomMetrics set MetricFunction = '{0}' where MetricID={1}", cus.MetricFunction, cus.MetricID);
									}
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
									commandText = "update Metrics set HOURLYMETRICGENERATION=3 where metricID=" + cus.MetricID + "";
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
									break;
							}
						}
						else if (cboObservationType.SelectedIndex == 1)
						{
							commandText = string.Format("update Metrics set HOURLYMETRICGENERATION=0 where MetricID in ({0})", _lstMetrics[i].MetricID);
							fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
							commandText = string.Format("delete from FixedWindowMetrics where MetricID in ({0})", _lstMetrics[i].MetricID);
							fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
							commandText = string.Format("delete from MovingWindowMetrics where MetricID in ({0})", _lstMetrics[i].MetricID);
							fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
							commandText = string.Format("delete from CustomMetrics where MetricID in ({0})", _lstMetrics[i].MetricID);
							fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
						}
					}
				}
				if (_isAddPollutant)
				{
					for (int i = 0; i < _lstAllSeasonalMetric.Count; i++)
					{
						commandText = "select SeasonalMetricName from SeasonalMetrics where SeasonalMetricID=" + _lstAllSeasonalMetric[i].SeasonalMetricID + " and MetricID=" + _lstAllSeasonalMetric[i].Metric.MetricID + "";
						object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
						if (obj == null)
						{
							commandText = string.Format("insert into SeasonalMetrics values ({0},{1},'{2}')", _lstAllSeasonalMetric[i].SeasonalMetricID, _lstAllSeasonalMetric[i].Metric.MetricID, _lstAllSeasonalMetric[i].SeasonalMetricName);
							fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
						}
						else
						{
							commandText = "update SeasonalMetrics set SeasonalMetricName='" + _lstAllSeasonalMetric[i].SeasonalMetricName + "' where SeasonalMetricID=" + _lstAllSeasonalMetric[i].SeasonalMetricID + " ";
							fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
						}
						if (_lstAllSeasonalMetric[i].Seasons != null)
						{
							for (int j = 0; j < _lstSeasonalMetric[i].Seasons.Count; j++)
							{
								commandText = "select startDay from SeasonalMetricSeasons where SeasonalMetricSeasonID=" + _lstAllSeasonalMetric[i].Seasons[j].SeasonalMetricSeasonID + "";
								obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
								if (obj == null)
								{
									commandText = string.Format("insert into SeasonalMetricSeasons values ({0},{1},{2},{3},{4},'{5}',{6})", _lstAllSeasonalMetric[i].Seasons[j].SeasonalMetricSeasonID, _lstAllSeasonalMetric[i].Seasons[j].SeasonalMetricID, _lstAllSeasonalMetric[i].Seasons[j].StartDay, _lstAllSeasonalMetric[i].Seasons[j].EndDay, _lstAllSeasonalMetric[i].Seasons[j].SeasonalMetricType, _lstAllSeasonalMetric[i].Seasons[j].MetricFunction, _lstAllSeasonalMetric[i].Seasons[j].PollutantSeasonID);
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
								}
								else
								{
									commandText = string.Format("update SeasonalMetricSeasons set Startday={0}, Endday={1}, Seasonalmetrictype={2}, MetricFunction = '{3}' where SeasonalMetricSeasonID={4}", _lstAllSeasonalMetric[i].Seasons[j].StartDay, _lstAllSeasonalMetric[i].Seasons[j].EndDay, _lstAllSeasonalMetric[i].Seasons[j].SeasonalMetricType, _lstAllSeasonalMetric[i].Seasons[j].MetricFunction, _lstAllSeasonalMetric[i].Seasons[j].SeasonalMetricSeasonID);
									fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
								}
							}
						}
					}
				}

			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}

		}

		private Metric updateMetric(Metric metric)
		{
			try
			{
				wrongStartEndHour = false;
				if (cboObservationType.SelectedIndex == 0)
				{
					switch (cbohourlymetricgeneration.SelectedIndex)
					{
						case 0:
							FixedWindowMetric fix = new FixedWindowMetric();
							fix.MetricID = metric.MetricID;
							fix.MetricName = metric.MetricName;
							fix.PollutantID = Convert.ToInt32(_pollutantID);
							fix.HourlyMetricGeneration = 1;

							fix.StartHour = Convert.ToInt32(nudownStartHour.Value);
							fix.EndHour = Convert.ToInt32(nudownEndHour.Value);
							if (fix.StartHour >= fix.EndHour)
							{
								wrongStartEndHour = true;
								MessageBox.Show("The start hour must be earlier than end hour.");
								return metric;
							}
							else
							{
								wrongStartEndHour = false;
							}
							fix.Statistic = (MetricStatic)(cboFixStatistic.SelectedIndex + 1);
							return fix;
						case 1:
							MovingWindowMetric mov = new MovingWindowMetric();
							mov.MetricID = metric.MetricID;
							mov.MetricName = metric.MetricName;
							mov.PollutantID = Convert.ToInt32(_pollutantID);
							mov.HourlyMetricGeneration = 2;

							mov.WindowSize = Convert.ToInt32(nudownWindowSize.Value);
							mov.DailyStatistic = (MetricStatic)(cboMovingStatistic.SelectedIndex + 1);
							mov.WindowStatistic = (MetricStatic)(cboWindowStatistic.SelectedIndex + 1);
							return mov;
						case 2:
							CustomerMetric cus = new CustomerMetric();
							cus.MetricID = metric.MetricID;
							cus.MetricName = metric.MetricName;
							cus.PollutantID = Convert.ToInt32(_pollutantID);
							cus.HourlyMetricGeneration = 3;

							cus.MetricFunction = txtFunctionManage.Text;
							metric = (Metric)cus;
							return cus;
					}
				}
				else
				{
					FixedWindowMetric fix = new FixedWindowMetric();
					fix.MetricID = metric.MetricID;
					fix.MetricName = metric.MetricName;
					fix.PollutantID = Convert.ToInt32(_pollutantID);
					fix.HourlyMetricGeneration = 1;
					fix.StartHour = 0;
					fix.EndHour = 23;
					fix.Statistic = MetricStatic.Mean;
					return fix;
				}
				return metric;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
				return metric;
			}
		}

		bool isRefrest = true;
		private void RefrestFileList()
		{
			for (int i = 0; i < lstMetrics.Items.Count; i++)
				lstMetrics.Items[i] = lstMetrics.Items[i];
		}

		private void cboObservationType_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (cboObservationType.SelectedIndex == 0 && lstMetrics.Items.Count > 0)
				{
					cbohourlymetricgeneration.Enabled = true;
					tabHourlyMetricGeneration.Enabled = true;
				}
				else if (cboObservationType.SelectedIndex == 1)
				{
					cbohourlymetricgeneration.Enabled = false;
					tabHourlyMetricGeneration.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}
		private void lstMetrics_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (lstMetrics.Items.Count == 0) { txtMetricName.Text = ""; return; }
				if (!isRefrest) return;
				if (_metric != null)
				{
					for (int i = 0; i < _lstMetrics.Count; i++)
					{
						if (_lstMetrics[i].MetricID == _metric.MetricID)
						{
							_lstMetrics[i] = updateMetric(_metric);
							if (wrongStartEndHour)
							{
								lstMetrics.SelectedIndex = i;
								return;
							}
							break;
						}
					}
				}
				ListItem lt = (lstMetrics.SelectedIndex < 0 ? lstMetrics.Items[lstMetrics.Items.Count - 1] : lstMetrics.Items[lstMetrics.SelectedIndex]) as ListItem;
				var lst = (from p in _lstMetrics where p.MetricID.ToString() == lt.ID select p).ToList();
				if (lst.Count == 0) { return; }
				Metric metric = (Metric)lst[0];
				_metric = metric;
				txtMetricName.Text = metric.MetricName;

				FixedWindowMetric fix = new FixedWindowMetric();
				MovingWindowMetric mov = new MovingWindowMetric();
				CustomerMetric cus = new CustomerMetric();
				if (metric is FixedWindowMetric)
				{
					fix = metric as FixedWindowMetric;
				}
				else if (metric is MovingWindowMetric)
				{
					mov = metric as MovingWindowMetric;
				}
				else if (metric is CustomerMetric)
				{
					cus = metric as CustomerMetric;
				}
				switch (metric.HourlyMetricGeneration)
				{
					case 0:
						tabHourlyMetricGeneration.Controls.Clear();
						tabHourlyMetricGeneration.TabPages.Add(tpFix);
						break;
					case 1:
						cbohourlymetricgeneration.SelectedIndex = 0;
						tabHourlyMetricGeneration.Controls.Clear();
						tabHourlyMetricGeneration.TabPages.Add(tpFix);
						nudownStartHour.Value = fix.StartHour;
						nudownEndHour.Value = fix.EndHour;
						cboFixStatistic.SelectedIndex = (int)fix.Statistic - 1;
						break;
					case 2:
						cbohourlymetricgeneration.SelectedIndex = 1;
						tabHourlyMetricGeneration.Controls.Clear();
						tabHourlyMetricGeneration.TabPages.Add(tpMovingWindow);
						nudownWindowSize.Value = mov.WindowSize;
						cboMovingStatistic.SelectedIndex = (int)mov.DailyStatistic - 1;
						cboWindowStatistic.SelectedIndex = (int)mov.WindowStatistic - 1;
						break;
					case 3:
						cbohourlymetricgeneration.SelectedIndex = 2;
						tabHourlyMetricGeneration.Controls.Clear();
						tabHourlyMetricGeneration.TabPages.Add(tpCustom);
						txtFunctionManage.Text = cus.MetricFunction;
						break;
				}

				var sMetric = (from p in _lstAllSeasonalMetric where p.Metric.MetricID == metric.MetricID select p.SeasonalMetricName).ToList();
				lstSeasonalMetrics.DataSource = null; lstSeasonalMetrics.Items.Clear();
				for (int i = 0; i < sMetric.Count; i++)
				{
					lstSeasonalMetrics.Items.Add(sMetric[i].ToString());
				}
				_lstSeasonalMetric = (from p in _lstAllSeasonalMetric where p.Metric.MetricID == metric.MetricID select p).ToList();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}


		private void cbohourlymetricgeneration_SelectedValueChanged(object sender, EventArgs e)
		{
			try
			{
				switch (cbohourlymetricgeneration.SelectedIndex)
				{
					case 0:
						tabHourlyMetricGeneration.Controls.Clear();
						tabHourlyMetricGeneration.TabPages.Add(tpFix);
						cboFixStatistic.SelectedIndex = 0;
						break;
					case 1:
						tabHourlyMetricGeneration.Controls.Clear();
						tabHourlyMetricGeneration.TabPages.Add(tpMovingWindow);
						cboWindowStatistic.SelectedIndex = 0;
						cboMovingStatistic.SelectedIndex = 0;
						break;
					case 2:
						tabHourlyMetricGeneration.Controls.Clear();
						tabHourlyMetricGeneration.TabPages.Add(tpCustom);
						txtFunctionManage.Text = "";
						break;
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void btnMetricsAdd_Click(object sender, EventArgs e)
		{
			try
			{
				if (cboObservationType.SelectedIndex == 1)
				{
					cbohourlymetricgeneration.Enabled = false;
					tabHourlyMetricGeneration.Enabled = false;
				}
				else
				{
					cbohourlymetricgeneration.Enabled = true;
					tabHourlyMetricGeneration.Enabled = true;
				}
				if (lstMetrics.Items.Count == 0)
				{
					txtMetricName.Enabled = true;
				}
				else
				{
					if (!isRefrest) return;
					if (_metric != null)
					{
						for (int i = 0; i < _lstMetrics.Count; i++)
						{
							if (_lstMetrics[i].MetricID == _metric.MetricID)
							{
								_lstMetrics[i] = updateMetric(_metric);
								if (wrongStartEndHour) return;
								break;
							}
						}
					}
				}
				if (lstMetrics.SelectedItem != null) { _metric = updateMetric(_metric); }
				Metric addMetric = new Metric();
				FireBirdHelperBase fb = new ESILFireBirdHelper();
				string commandText = "select max(METRICID) from METRICS";
				metricidadd++;
				addMetric.MetricID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + metricidadd;
				bool checkExist = true;
				if (lstMetrics.Items.Count > 0)
				{
					while (checkExist)
					{
						foreach (ListItem it in lstMetrics.Items)
						{
							if (it.ToString() == "Metric " + _addMetricNumber.ToString() + "")
							{
								_addMetricNumber++;
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
				addMetric.MetricName = "Metric " + _addMetricNumber.ToString() + "";
				txtMetricName.Text = "Metric " + _addMetricNumber.ToString() + "";
				addMetric.PollutantID = Convert.ToInt32(_pollutantID);

				FixedWindowMetric newMetric = new FixedWindowMetric();
				newMetric.MetricID = addMetric.MetricID;
				newMetric.MetricName = txtMetricName.Text;
				newMetric.PollutantID = Convert.ToInt32(_pollutantID);
				newMetric.HourlyMetricGeneration = 1;

				newMetric.StartHour = 0;
				newMetric.EndHour = 23;
				newMetric.Statistic = (MetricStatic)1;

				_lstMetrics.Add(newMetric);
				lstMetrics.Items.Add(new ListItem(newMetric.MetricID.ToString(), newMetric.MetricName));
				_metric = null;
				lstMetrics.SelectedIndex = lstMetrics.Items.Count - 1;
				_addMetricNumber++;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void btnMetricsDelete_Click(object sender, EventArgs e)
		{
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			string commandText = string.Empty;
			try
			{
				if (lstMetrics.Items.Count == 0)
				{
					MessageBox.Show("There are no metrics to delete."); return;
				}
				if (lstMetrics.SelectedItem == null) { MessageBox.Show("Please select one Metric."); return; }
				ListItem lst = lstMetrics.SelectedItem as ListItem;
				int metricID = Convert.ToInt32(lst.ID);

				commandText = "select CRFunctionID from CRFunctions where MetricID=" + metricID + "";
				object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
				if (obj != null)
				{
					MessageBox.Show("This metric is used by functions in one or more health impact function datasets. Please delete health impact functions that use this metric first.");
					return;
				}
				commandText = "select MonitorID from MonitorEntries where MetricID=" + metricID + " ";
				obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
				if (obj != null)
				{
					MessageBox.Show("This metric is used by a monitor dataset. Please delete monitor datasets that use this metric first.");
					return;
				}
				commandText = "select SeasonalMetricID from SeasonalMetrics where MetricID=" + metricID + " ";
				obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
				string warning = "";
				if (obj != null)
				{
					warning = "All seasonal metrics associated with this metric will be deleted. ";
				}
				DialogResult drt = MessageBox.Show(warning + "Are you sure you want to delete this metric? ", "Confirm", MessageBoxButtons.YesNo);
				if (drt == DialogResult.Yes)
				{
					commandText = "delete from SeasonalMetricSeasons where SeasonalMetricID=" + Convert.ToInt16(obj) + "";
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

					lstSeasonalMetrics.DataSource = null;
					lstSeasonalMetrics.Items.Clear();
					int allSeasonalMetricCount = _lstAllSeasonalMetric.Count;
					for (int i = allSeasonalMetricCount - 1; i >= 0; i--)
					{
						if (_lstAllSeasonalMetric[i].Metric.MetricID == _metric.MetricID)
						{
							_lstAllSeasonalMetric.RemoveAt(i);
						}
					}
					commandText = "delete from SeasonalMetrics where SeasonalMetricID=" + Convert.ToInt16(obj) + "";
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

					commandText = string.Format("delete from Metrics where MetricID='{0}' and PollutantID={1}", metricID, _pollutantID);
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
					commandText = "delete from SeasonalMetrics where MetricID=" + metricID + "";
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
					commandText = "delete from CustomMetrics where MetricID=" + metricID + "";
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
					commandText = "delete from fixedwindowMetrics where metricID=" + metricID + "";
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
					commandText = "delete from MovingWindowMetrics where metricID=" + metricID + " ";
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
					for (int i = 0; i < _lstMetrics.Count; i++)
					{
						if (_lstMetrics[i].MetricID == metricID)
						{
							_lstMetrics.RemoveAt(i);
							break;
						}
					}
					lstMetrics.Items.Remove(lstMetrics.SelectedItem);
				}

				if (lstMetrics.Items.Count == 0)
				{
					cbohourlymetricgeneration.SelectedIndex = 0;
					nudownStartHour.Minimum = 0;
					nudownStartHour.Maximum = 23;
					nudownEndHour.Minimum = 0;
					nudownEndHour.Maximum = 23;
					cboFixStatistic.SelectedIndex = 0;
					txtMetricName.Text = "";
					txtMetricName.Enabled = false;
					cbohourlymetricgeneration.Enabled = false;
					tabHourlyMetricGeneration.Enabled = false;
				}
				else
				{
					lstMetrics.SelectedIndex = lstMetrics.Items.Count - 1;
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void btnAdvancedOptions_Click(object sender, EventArgs e)
		{

			try
			{
				if (!isRefrest) return;
				if (_metric != null)
				{
					for (int i = 0; i < _lstMetrics.Count; i++)
					{
						if (_lstMetrics[i].MetricID == _metric.MetricID)
						{
							_lstMetrics[i] = updateMetric(_metric);
							if (wrongStartEndHour) return;
							break;
						}
					}
				}
				DefineSeasons frm = new DefineSeasons(_isAddPollutant);
				frm._pollutantID = _pollutantID;
				frm._lstSeasonalMetric = _lstAllSeasonalMetric;		//pass seasonal metric information to the global pollutant window
				frm.dicSave = _dicSeasons;
				DialogResult rtn = frm.ShowDialog();
				{
					_dicSeasons = frm.dicSave;
					if (_dicSeasons.Count != 0)
					{

					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}
		bool PollutantExist = false;
		private void btnEdit_Click(object sender, EventArgs e)
		{
			try
			{
				if (lstMetrics.Items.Count <= 0)
				{
					MessageBox.Show("Please add a metric first.", "Error", MessageBoxButtons.OK);
					return;
				}
				if (!isRefrest) return;
				if (_metric != null)
				{
					for (int i = 0; i < _lstMetrics.Count; i++)
					{
						if (_lstMetrics[i].MetricID == _metric.MetricID)
						{
							_lstMetrics[i] = updateMetric(_metric);
							if (wrongStartEndHour) return;
							break;
						}
					}
				}
				updateBenMAPPollutant(_benMAPPollutant);
				if (PollutantExist || customFunctionInvalid) return;
				if (_dicSeasons.Count == 0) return;

				ManageSeasonalMetrics frm = new ManageSeasonalMetrics(_lstSeasonalMetric, _metric, _isAddPollutant, _dicSeasons);
				frm.ShowDialog();
				{
					if (ManageSeasonalMetrics.LstSMetrics.Count != 0)
					{
						_lstSeasonalMetric = ManageSeasonalMetrics.LstSMetrics;
						int allSeasonalMetricCount = _lstAllSeasonalMetric.Count;
						for (int i = allSeasonalMetricCount - 1; i >= 0; i--)
						{
							if (_lstAllSeasonalMetric[i].Metric.MetricID == _metric.MetricID)
							{
								_lstAllSeasonalMetric.RemoveAt(i);
							}
						}
						for (int i = 0; i < _lstSeasonalMetric.Count; i++)
						{
							_lstAllSeasonalMetric.Add(_lstSeasonalMetric[i]);
						}
						var sMetric = (from p in _lstAllSeasonalMetric where p.Metric.MetricID == _metric.MetricID select p.SeasonalMetricName).ToList();
						lstSeasonalMetrics.DataSource = null;
						lstSeasonalMetrics.Items.Clear();
						for (int i = 0; i < sMetric.Count; i++)
						{
							lstSeasonalMetrics.Items.Add(sMetric[i].ToString());
						}
						_lstSeasonalMetric = (from p in _lstAllSeasonalMetric where p.Metric.MetricID == _metric.MetricID select p).ToList();
					}
					else
					{
						lstSeasonalMetrics.DataSource = null;
						lstSeasonalMetrics.Items.Clear();
						int allSeasonalMetricCount = _lstAllSeasonalMetric.Count;
						for (int i = allSeasonalMetricCount - 1; i >= 0; i--)
						{
							if (_lstAllSeasonalMetric[i].Metric.MetricID == _metric.MetricID)
							{
								_lstAllSeasonalMetric.RemoveAt(i);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}
		bool customFunctionInvalid = false;
		private void btnOK_Click(object sender, EventArgs e)
		{
			try
			{
				if (!isRefrest) return;
				if (_metric != null)
				{
					for (int i = 0; i < _lstMetrics.Count; i++)
					{
						if (_lstMetrics[i].MetricID == _metric.MetricID)
						{
							_lstMetrics[i] = updateMetric(_metric);
							if (wrongStartEndHour) return;
							break;
						}
					}
				}

				//Check if pollutant seasons exist. 
				if (!(_dicSeasons.Count > 0))
				{
					MessageBox.Show("You must define seasons for this pollutant.", "Error");
					return;
				}

				updateBenMAPPollutant(_benMAPPollutant);
				if (PollutantExist || customFunctionInvalid) return;
				ManageSeasonalMetrics.LstSMetrics.Clear();
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

		private void txtMetricName_Leave(object sender, EventArgs e)
		{
			try
			{
				if (txtMetricName.Text.Trim() == "")
				{
					if (_metric != null)
						txtMetricName.Text = _metric.MetricName;
					return;
				}
				bool sameName = false;
				int j = 1;
				string metricName = txtMetricName.Text;
				do
				{
					sameName = false;
					for (int i = 0; i < _lstMetrics.Count; i++)
					{
						if (_lstMetrics[i].MetricID != _metric.MetricID)
						{
							if (_lstMetrics[i].MetricName == metricName)
							{
								sameName = true;
								metricName = txtMetricName.Text + "_" + j++;
								break;
							}
						}
					}
				} while (sameName);
				txtMetricName.Text = metricName;
				_metric.MetricName = metricName;

				foreach (ListItem l in lstMetrics.Items)
				{
					if (l.ID == _metric.MetricID.ToString())
					{
						l.Name = txtMetricName.Text;
						isRefrest = false;
						RefrestFileList();
						isRefrest = true;
						break;
					}
				}

			}
			catch
			{ }
		}

		private void txtMetricName_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				txtMetricName_Leave(sender, e);
			}
		}
		private void PollutantDefinition_FormClosing(object sender, FormClosingEventArgs e)
		{
			//Check if pollutant seasons exist.
			//No need to check for _isAddPollutant as the pollutant is not added into the database at form close event.
			if (!_isAddPollutant)
			{
				if (!(_dicSeasons.Count() > 0))
				{
					MessageBox.Show("You must define seasons for this pollutant.");
					e.Cancel = true;
				}
			}
		}

	}

	public class ManageSetupSeasonalMetric
	{
		public int SeasonalMetricID;
		public string SeasonalMetricName;
		public Metric Metric;
		public List<SeasonalMetricSeason> Seasons;
	}

	public class SeasonalMetricSeason
	{
		public int SeasonalMetricSeasonID;
		public int SeasonalMetricID;
		public int StartDay;
		public int EndDay;
		public int SeasonalMetricType;
		public string MetricFunction;
		public int PollutantSeasonID;
	}


}
