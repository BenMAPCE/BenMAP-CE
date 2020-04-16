using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using BenMAP.DataSource;

namespace BenMAP
{
	public partial class MonitorData : FormBase
	{
		private BaseControlGroup _bgcMonitor = null;

		private MonitorDataLine _mDataLine = new MonitorDataLine();
		private string _SaveAQGPath = "";
		public MonitorDataLine MDataLine
		{
			get { return _mDataLine; }
			set { _mDataLine = value; }
		}

		public string SaveAQGPath
		{
			get { return _SaveAQGPath; }
			set { _SaveAQGPath = value; }
		}

		private string _currentStat = string.Empty;

		public MonitorData(BaseControlGroup currentPollutant, string currentStat, string SaveAQGPath)
		{
			InitializeComponent();
			_bgcMonitor = currentPollutant;
			_currentStat = currentStat;
			_SaveAQGPath = SaveAQGPath;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			try
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.InitialDirectory = CommonClass.ResultFilePath;
				openFileDialog.Filter = "Supported File Types (*.csv, *.xls, *.xlsx)|*.csv; *.xls; *.xlsx|CSV files|*.csv|XLS files|*.xls|XLSX files|*.xlsx";
				openFileDialog.FilterIndex = 1;
				openFileDialog.RestoreDirectory = true;
				if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
				txtMonitorDataFile.Text = openFileDialog.FileName;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void MonitorData_Load(object sender, EventArgs e)
		{
			ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
			string commandText = string.Empty;
			DataSet ds;
			try
			{
				cboMonitorType.Items.Add("Library");
				cboMonitorType.Items.Add("Text File");
				cboMonitorType.SelectedIndex = 0;
				if (CommonClass.GBenMAPGrid != null)
				{
					txtGridType.Text = CommonClass.GBenMAPGrid.GridDefinitionName;
					txtGridType.Enabled = false;
				}
				if (CommonClass.LstPollutant != null && CommonClass.LstPollutant.Count > 0)
				{
					txtPollutant.Text = _bgcMonitor.Pollutant.PollutantName;
					txtPollutant.Enabled = false;
				}
				rbtnVoronoi.Checked = true;
				commandText = string.Format("select MonitorDataSetID,MonitorDataSetName from MonitorDataSets where SetupID={0} and MonitorDataSetID in (select distinct MonitorDataSetID from monitors where pollutantID={1}) order by MonitorDataSetName asc", CommonClass.MainSetup.SetupID, _bgcMonitor.Pollutant.PollutantID);

				ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
				cboMonitorDataSet.DataSource = ds.Tables[0];
				cboMonitorDataSet.DisplayMember = "MonitorDataSetName";
				if (cboMonitorDataSet.Items.Count != 0)
				{
					cboMonitorDataSet.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private MonitorAdvance _monitorAdvance = null;

		private void btnAdvanced_Click(object sender, EventArgs e)
		{
			string msg = string.Empty;
			try
			{
				if (!((cboMonitorDataSet.Text != string.Empty && cboMonitorLibraryYear.Text != string.Empty) || txtMonitorDataFile.Text != string.Empty))
				{
					msg = "Please select monitor dataset.";
					MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
				string method = string.Empty;
				foreach (var c in grpInterpolationMethod.Controls)
				{
					RadioButton r = c as RadioButton;
					if (r == null) { continue; }
					if (r.Checked) { method = r.Tag.ToString(); break; }
				}
				if (_monitorAdvance == null)
				{
					_monitorAdvance = new MonitorAdvance();
				}
				switch (cboMonitorType.SelectedIndex)
				{
					case 0:
						_mDataLine.MonitorDirectType = 0;
						if (cboMonitorDataSet.Text != string.Empty && cboMonitorLibraryYear.Text != string.Empty)
						{
							DataRowView drv = cboMonitorDataSet.SelectedItem as DataRowView;
							int dsID = int.Parse(drv["MonitorDataSetID"].ToString());
							_mDataLine.MonitorDataSetID = dsID;
							drv = cboMonitorLibraryYear.SelectedItem as DataRowView;
							_mDataLine.MonitorLibraryYear = int.Parse(drv["Yyear"].ToString());
						}
						else
						{ msg = string.Format("Settings are not complete."); return; }
						_strPath = "Monitor data: library";
						break;
					case 1:
						_mDataLine.MonitorDirectType = 1;
						if (txtMonitorDataFile.Text != string.Empty) { _mDataLine.MonitorDataFilePath = txtMonitorDataFile.Text; }
						else
						{ msg = string.Format("Settings are not complete."); return; }
						_strPath = "Monitor data: " + txtMonitorDataFile.Text;
						break;
				}
				AdvancedOptions frm = new AdvancedOptions(method, _monitorAdvance);
				frm.bcg = _bgcMonitor;
				frm.mDataLine = _mDataLine;
				DialogResult rtn = frm.ShowDialog();
				if (rtn == DialogResult.OK) { _monitorAdvance = frm.MyMonitorAdvance; }
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void cboMonitorDataSet_SelectedValueChanged(object sender, EventArgs e)
		{
			ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
			string commandText = string.Empty;
			DataSet ds;
			try
			{
				cboMonitorLibraryYear.Text = null;
				if (cboMonitorDataSet == null || cboMonitorDataSet.SelectedItem == null) { return; }
				DataRowView drv = cboMonitorDataSet.SelectedItem as DataRowView;

				int MonitorDataSetID = Convert.ToInt32(drv["MonitorDataSetID"]);
				commandText = string.Format("select distinct Yyear from MonitorEntries a,Monitors b where a.MonitorID =b.MonitorID and MonitorDataSetID={0} and PollutantID={1}", MonitorDataSetID, _bgcMonitor.Pollutant.PollutantID);
				ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);

				cboMonitorLibraryYear.DataSource = ds.Tables[0];
				cboMonitorLibraryYear.DisplayMember = "Yyear";
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}
		string saveAQGPath = string.Empty;
		private void btnGo_Click(object sender, EventArgs e)
		{
			string msg = string.Empty;
			bool ok = false;
			double value = 0.0;
			try
			{
				if (rbtnClosestMonitor.Checked) { _mDataLine.InterpolationMethod = InterpolationMethodEnum.ClosestMonitor; }
				else if (rbtnVoronoi.Checked) { _mDataLine.InterpolationMethod = InterpolationMethodEnum.VoronoiNeighborhoodAveragin; }
				else if (rbtnFixedRadiums.Checked)
				{
					_mDataLine.InterpolationMethod = InterpolationMethodEnum.FixedRadius;
					ok = double.TryParse(txtRadiums.Text, out value);
					if (!ok || Convert.ToDouble(txtRadiums.Text) <= 0)
					{
						msg = "To select fixed radius interpolation you must provide a radius in kilometers.";
						txtRadiums.Text = string.Empty;
						return;
					}
					_mDataLine.FixedRadius = value;
				}
				int selectedIndex = cboMonitorType.SelectedIndex;
				switch (selectedIndex)
				{
					case 0:
						_mDataLine.MonitorDirectType = selectedIndex;
						if (cboMonitorDataSet.Text != string.Empty && cboMonitorLibraryYear.Text != string.Empty)
						{
							DataRowView drv = cboMonitorDataSet.SelectedItem as DataRowView;
							int dsID = int.Parse(drv["MonitorDataSetID"].ToString());
							_mDataLine.MonitorDataSetID = dsID;
							drv = cboMonitorLibraryYear.SelectedItem as DataRowView;
							_mDataLine.MonitorLibraryYear = int.Parse(drv["Yyear"].ToString());
						}
						else
						{ msg = string.Format("Settings are not completed."); return; }
						_strPath = "Monitor data: library";
						break;
					case 1:
						_mDataLine.MonitorDirectType = selectedIndex;
						if (txtMonitorDataFile.Text != string.Empty) { _mDataLine.MonitorDataFilePath = txtMonitorDataFile.Text; }
						else
						{ msg = string.Format("Settings are not completed."); return; }
						_strPath = "Monitor data: " + txtMonitorDataFile.Text;
						break;
				}
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.Filter = "AQGX files (*.aqgx)|*.aqgx";
				sfd.FilterIndex = 2;
				sfd.RestoreDirectory = true;
				if (String.IsNullOrEmpty(_SaveAQGPath))
					sfd.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG";
				if (sfd.ShowDialog() != DialogResult.OK)
				{ return; }
				saveAQGPath = sfd.FileName;
				_SaveAQGPath = sfd.FileName;
				if (_monitorAdvance != null)
				{
					_mDataLine.MonitorAdvance = _monitorAdvance;
				}
				ModelDataLine m = new ModelDataLine();
				int threadId = -1;
				AsyncDelegate asyncD = new AsyncDelegate(AsyncUpdateMonitorData);
				IAsyncResult ar = asyncD.BeginInvoke(_bgcMonitor, m, _currentStat, out threadId, null, null);

				this.DialogResult = DialogResult.OK;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
			finally
			{
				if (msg != string.Empty)
				{
					MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}

		private string _strPath;

		public string StrPath
		{
			get { return _strPath; }
			set { _strPath = value; }
		}

		/// <summary>
		/// Asynchronouses the update monitor data.
		/// </summary>
		/// <param name="bcg">The BCG.</param>
		/// <param name="m">The m.</param>
		/// <param name="currentStat">The current stat.</param>
		/// <param name="threadId">The thread identifier.</param>
		/// <returns>System.String.</returns>
		private string AsyncUpdateMonitorData(BaseControlGroup bcg, ModelDataLine m, string currentStat, out int threadId)
		{
			threadId = -1;
			string str = string.Empty;
			try
			{
				if (CommonClass.LstAsynchronizationStates == null) { CommonClass.LstAsynchronizationStates = new List<string>(); }
				lock (CommonClass.LstAsynchronizationStates)
				{
					str = string.Format("{0}{1}", bcg.Pollutant.PollutantName.ToLower(), currentStat);
					CommonClass.LstAsynchronizationStates.Add(str);
					if (currentStat != "")
					{
						CommonClass.CurrentMainFormStat = currentStat.Substring(0, 1).ToUpper() + currentStat.Substring(1) + " is being created.";
					}
				}
				lock (CommonClass.NodeAnscyStatus)
				{ CommonClass.NodeAnscyStatus = string.Format("{0};{1};on", bcg.Pollutant.PollutantName.ToLower(), _currentStat); }
				switch (_currentStat)
				{
					case "baseline":
						DataSourceCommonClass.UpdateModelValuesMonitorData(_bgcMonitor.GridType, _bgcMonitor.Pollutant, ref _mDataLine);
						lock (CommonClass.LstBaseControlGroup)
						{
							foreach (BaseControlGroup bc in CommonClass.LstBaseControlGroup)
							{
								if (bc.Pollutant.PollutantID == _bgcMonitor.Pollutant.PollutantID)
								{
									_mDataLine.GridType = _bgcMonitor.GridType;
									_mDataLine.Pollutant = _bgcMonitor.Pollutant;
									_mDataLine.ShapeFile = _mDataLine.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "B" + _currentStat + ".shp";
									string shipFile = string.Format("{0}\\Tmp\\{1}", CommonClass.DataFilePath, _mDataLine.ShapeFile);
									bc.Base = _mDataLine;
									DataSourceCommonClass.SaveBenMAPLineShapeFile(_mDataLine.GridType, _mDataLine.Pollutant, _mDataLine, shipFile);
									DataSourceCommonClass.CreateAQGFromBenMAPLine(bc.Base, saveAQGPath); bc.Base.ShapeFile = "";
								}
							}
						}
						_bgcMonitor.Base = _mDataLine;
						break;
					case "control":
						DataSourceCommonClass.UpdateModelValuesMonitorData(_bgcMonitor.GridType, _bgcMonitor.Pollutant, ref _mDataLine);
						lock (CommonClass.LstBaseControlGroup)
						{
							foreach (BaseControlGroup bc in CommonClass.LstBaseControlGroup)
							{
								if (bc.Pollutant.PollutantID == _bgcMonitor.Pollutant.PollutantID)
								{
									_mDataLine.GridType = _bgcMonitor.GridType;
									_mDataLine.Pollutant = _bgcMonitor.Pollutant;
									_mDataLine.ShapeFile = _mDataLine.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "C" + _currentStat + ".shp";
									string shipFile = string.Format("{0}\\Tmp\\{1}", CommonClass.DataFilePath, _mDataLine.ShapeFile);
									bc.Control = _mDataLine;
									DataSourceCommonClass.SaveBenMAPLineShapeFile(_mDataLine.GridType, _mDataLine.Pollutant, _mDataLine, shipFile);
									DataSourceCommonClass.CreateAQGFromBenMAPLine(bc.Control, saveAQGPath); bc.Control.ShapeFile = "";
								}
							}
						}
						_bgcMonitor.Control = _mDataLine;
						break;
				}
				List<ModelResultAttribute> lstRemove = new List<ModelResultAttribute>();
				foreach (ModelResultAttribute model in _mDataLine.ModelResultAttributes)
				{
					if (model.Values == null || model.Values.Count == 0)
					{
						lstRemove.Add(model);
					}
				}
				foreach (ModelResultAttribute model in lstRemove)
				{
					_mDataLine.ModelResultAttributes.Remove(model);
				}
				lock (CommonClass.LstAsynchronizationStates)
				{
					CommonClass.LstAsynchronizationStates.Remove(str);
					if (CommonClass.LstAsynchronizationStates.Count == 0)
					{
						CommonClass.CurrentMainFormStat = "Current Setup: " + CommonClass.MainSetup.SetupName;
					}
				}
				lock (CommonClass.NodeAnscyStatus)
				{ CommonClass.NodeAnscyStatus = string.Format("{0};{1};off", bcg.Pollutant.PollutantName.ToLower(), _currentStat); }
				return str;
			}
			catch (Exception ex)
			{
				lock (CommonClass.LstAsynchronizationStates)
				{
					CommonClass.LstAsynchronizationStates.Remove(str);
					if (CommonClass.LstAsynchronizationStates.Count == 0)
					{
						CommonClass.CurrentMainFormStat = "Current Setup: " + CommonClass.MainSetup.SetupName;
					}
				}
				lock (CommonClass.NodeAnscyStatus)
				{ CommonClass.NodeAnscyStatus = string.Format("{0};{1};off", bcg.Pollutant.PollutantName.ToLower(), _currentStat); }
				Logger.LogError(ex);
				return string.Empty;
			}
		}

		private void cboMonitorType_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (sender == null) { return; }
				int currentPageIndex = cboMonitorType.SelectedIndex;
				switch (currentPageIndex)
				{
					case 0:
						tcMonitorData.Controls.Clear();
						tcMonitorData.TabPages.Add(tpLibrary);
						break;
					case 1:
						tcMonitorData.Controls.Clear();
						tcMonitorData.TabPages.Add(tpText);
						break;
					case 2:
						break;
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
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

		private void btnMap_Click(object sender, EventArgs e)
		{
			bool ok = false;
			double value = 0;
			string msg = string.Empty;
			try
			{
				if (rbtnClosestMonitor.Checked) { _mDataLine.InterpolationMethod = InterpolationMethodEnum.ClosestMonitor; }
				else if (rbtnVoronoi.Checked) { _mDataLine.InterpolationMethod = InterpolationMethodEnum.VoronoiNeighborhoodAveragin; }
				else if (rbtnFixedRadiums.Checked)
				{
					_mDataLine.InterpolationMethod = InterpolationMethodEnum.FixedRadius;
					ok = double.TryParse(txtRadiums.Text, out value);
					if (!ok)
					{
						msg = "To select fixed radius interpolation you must provide a radius in kilometers.";
						txtRadiums.Text = string.Empty;
						return;
					}
					_mDataLine.FixedRadius = value;
				}
				int selectedIndex = cboMonitorType.SelectedIndex;
				switch (selectedIndex)
				{
					case 0:
						_mDataLine.MonitorDirectType = selectedIndex;
						if (cboMonitorDataSet.Text != string.Empty && cboMonitorLibraryYear.Text != string.Empty)
						{
							DataRowView drv = cboMonitorDataSet.SelectedItem as DataRowView;
							int dsID = int.Parse(drv["MonitorDataSetID"].ToString());
							_mDataLine.MonitorDataSetID = dsID;
							drv = cboMonitorLibraryYear.SelectedItem as DataRowView;
							_mDataLine.MonitorLibraryYear = int.Parse(drv["Yyear"].ToString());
						}
						else
						{ msg = string.Format("Settings are not complete."); return; }
						_strPath = "Monitor data: library";
						break;
					case 1:
						_mDataLine.MonitorDirectType = selectedIndex;
						if (txtMonitorDataFile.Text != string.Empty) { _mDataLine.MonitorDataFilePath = txtMonitorDataFile.Text; }
						else
						{ msg = string.Format("Settings are not complete."); return; }
						_strPath = "Monitor data: " + txtMonitorDataFile.Text;
						break;
				}
				if (_monitorAdvance != null)
				{
					_mDataLine.MonitorAdvance = _monitorAdvance;
				}
				_mDataLine.GridType = _bgcMonitor.GridType;
				_mDataLine.Pollutant = _bgcMonitor.Pollutant;
				_mDataLine.ShapeFile = _mDataLine.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "M" + _currentStat + ".shp";
				string shapeFile = string.Format("{0}\\Tmp\\{1}", CommonClass.DataFilePath, _mDataLine.ShapeFile);
				List<MonitorValue> lstMonitorValues = DataSourceCommonClass.GetMonitorData(_bgcMonitor.GridType, _bgcMonitor.Pollutant, _mDataLine);
				DataSourceCommonClass.UpdateMonitorDicMetricValue(_bgcMonitor.Pollutant, lstMonitorValues);
				if (_bgcMonitor.GridType is ShapefileGrid)
				{
					shapeFile = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (_bgcMonitor.GridType as ShapefileGrid).ShapefileName + ".shp";
				}
				else if (_bgcMonitor.GridType is RegularGrid)
				{
					shapeFile = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (_bgcMonitor.GridType as RegularGrid).ShapefileName + ".shp";
				}

				MonitorMap frm = new MonitorMap();
				frm.GridShapeFile = shapeFile;
				frm.LstMonitorPoints = lstMonitorValues;
				frm.ShowDialog();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}
		private bool DrawMointorMap()
		{
			bool ok = false;
			double value = 0.0;
			string msg = string.Empty;
			try
			{
				if (rbtnClosestMonitor.Checked) { _mDataLine.InterpolationMethod = InterpolationMethodEnum.ClosestMonitor; }
				else if (rbtnVoronoi.Checked) { _mDataLine.InterpolationMethod = InterpolationMethodEnum.VoronoiNeighborhoodAveragin; }
				else if (rbtnFixedRadiums.Checked)
				{
					_mDataLine.InterpolationMethod = InterpolationMethodEnum.FixedRadius;
					ok = double.TryParse(txtRadiums.Text, out value);
					if (!ok)
					{
						msg = "To select fixed radius interpolation you must provide a radius in kilometers.";
						txtRadiums.Text = string.Empty;
						return false;
					}
					_mDataLine.FixedRadius = value;
				}
				int selectedIndex = cboMonitorType.SelectedIndex;
				switch (selectedIndex)
				{
					case 0:
						_mDataLine.MonitorDirectType = selectedIndex;
						if (cboMonitorDataSet.Text != string.Empty && cboMonitorLibraryYear.Text != string.Empty)
						{
							DataRowView drv = cboMonitorDataSet.SelectedItem as DataRowView;
							int dsID = int.Parse(drv["MonitorDataSetID"].ToString());
							_mDataLine.MonitorDataSetID = dsID;
							drv = cboMonitorLibraryYear.SelectedItem as DataRowView;
							_mDataLine.MonitorLibraryYear = int.Parse(drv["Yyear"].ToString());
						}
						else
						{ msg = string.Format("Settings are not complete."); return false; }
						_strPath = "Monitor data: library";
						break;
					case 1:
						_mDataLine.MonitorDirectType = selectedIndex;
						if (txtMonitorDataFile.Text != string.Empty) { _mDataLine.MonitorDataFilePath = txtMonitorDataFile.Text; }
						else
						{ msg = string.Format("Settings are not complete."); return false; }
						_strPath = "Monitor data: " + txtMonitorDataFile.Text;
						break;
				}
				if (_monitorAdvance != null)
				{
					_mDataLine.MonitorAdvance = _monitorAdvance;
				}
				ModelDataLine m = new ModelDataLine();

				switch (_currentStat)
				{
					case "baseline":
						DataSourceCommonClass.UpdateModelValuesMonitorData(_bgcMonitor.GridType, _bgcMonitor.Pollutant, ref _mDataLine);
						lock (CommonClass.LstBaseControlGroup)
						{
							foreach (BaseControlGroup bc in CommonClass.LstBaseControlGroup)
							{
								if (bc.Pollutant.PollutantID == _bgcMonitor.Pollutant.PollutantID)
								{
									_mDataLine.GridType = _bgcMonitor.GridType;
									_mDataLine.Pollutant = _bgcMonitor.Pollutant;
									_mDataLine.ShapeFile = _mDataLine.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "B" + _currentStat + ".shp";
									string shipFile = string.Format("{0}\\Tmp\\{1}", CommonClass.DataFilePath, _mDataLine.ShapeFile);
									bc.Base = _mDataLine;
									DataSourceCommonClass.SaveBenMAPLineShapeFile(_mDataLine.GridType, _mDataLine.Pollutant, _mDataLine, shipFile);
									bc.Base.ShapeFile = "";
								}
							}
						}
						_bgcMonitor.Base = _mDataLine;
						break;
					case "control":
						DataSourceCommonClass.UpdateModelValuesMonitorData(_bgcMonitor.GridType, _bgcMonitor.Pollutant, ref _mDataLine);
						lock (CommonClass.LstBaseControlGroup)
						{
							foreach (BaseControlGroup bc in CommonClass.LstBaseControlGroup)
							{
								if (bc.Pollutant.PollutantID == _bgcMonitor.Pollutant.PollutantID)
								{
									_mDataLine.GridType = _bgcMonitor.GridType;
									_mDataLine.Pollutant = _bgcMonitor.Pollutant;
									_mDataLine.ShapeFile = _mDataLine.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "C" + _currentStat + ".shp";
									string shipFile = string.Format("{0}\\Tmp\\{1}", CommonClass.DataFilePath, _mDataLine.ShapeFile);
									bc.Control = _mDataLine;
									DataSourceCommonClass.SaveBenMAPLineShapeFile(_mDataLine.GridType, _mDataLine.Pollutant, _mDataLine, shipFile);
									bc.Control.ShapeFile = "";
								}
							}
						}
						_bgcMonitor.Control = _mDataLine;
						break;
				}
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				return false;
			}
		}

		private void txtRadiums_KeyPress(object sender, KeyPressEventArgs e)
		{
			if ((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == (char)8)
			{
				e.Handled = false;
			}
			else if (e.KeyChar == '.')
			{
				if (((TextBox)sender).Text.Trim().IndexOf('.') > -1)
					e.Handled = true;
			}
			else
			{
				e.Handled = true;
			}
		}

		private void rbtnFixedRadiums_Click(object sender, EventArgs e)
		{
			txtRadiums.Focus();
		}

		private void txtRadiums_Click(object sender, EventArgs e)
		{
			rbtnFixedRadiums.Checked = true;
		}

	}
}