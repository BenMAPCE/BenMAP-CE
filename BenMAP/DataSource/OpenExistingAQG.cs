using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using DotSpatial.Data;
using ESIL.DBUtility;

namespace BenMAP
{
	public partial class OpenExistingAQG : FormBase
	{
		string pathBaseControl = "";
		private MetadataClassObj _metadataObj = null;

		public OpenExistingAQG()
		{
			InitializeComponent();
		}

		public OpenExistingAQG(BaseControlGroup bcg)
		{
			InitializeComponent();
			bcgOpenAQG = bcg;
		}
		public BaseControlGroup bcgOpenAQG;
		public string basePath = string.Empty;
		public string controlPath = string.Empty;
		string saveBasePath = string.Empty;
		string saveControlPath = string.Empty;

		private void OpenExistingAQG_Load(object sender, EventArgs e)
		{
			FireBirdHelperBase fb = new ESILFireBirdHelper();
			try
			{
				if (CommonClass.LstPollutant != null && CommonClass.LstPollutant.Count > 0)
				{
					txtPollutant.Text = bcgOpenAQG.Pollutant.PollutantName;

				}
				txtPollutant.Enabled = false;
				string commandText = string.Format("select * from GridDefinitions where setupid={0} order by GridDefinitionName asc", CommonClass.MainSetup.SetupID);
				System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				DataTable dtGrid = ds.Tables[0].Clone();
				dtGrid = ds.Tables[0].Copy();
				cboGrid.DataSource = dtGrid;
				cboGrid.DisplayMember = "GridDefinitionName";
				for (int i = 0; i < dtGrid.Rows.Count; i++)
				{
					if (dtGrid.Rows[i]["defaulttype"].ToString() == "1" && CommonClass.GBenMAPGrid == null)
					{
						cboGrid.SelectedIndex = i;
						break;
					}
					else if (CommonClass.GBenMAPGrid != null && Convert.ToInt32(dtGrid.Rows[i]["GridDefinitionID"]) == CommonClass.GBenMAPGrid.GridDefinitionID)
					{
						cboGrid.SelectedIndex = i;
						break;

					}

				}
				if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0)
				{
					cboGrid.Enabled = false;

				}

			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}

		}

		private void btnOpenBase_Click(object sender, EventArgs e)
		{
			try
			{
				// 2015 02 05 replaced load dataset code with old (previously commented out code) to remove validataion requirement
				// initial directory path is wrong, however
				//#region Dead Code
				//    // 2015 02 05 - stopped here
				//    LoadSelectedDataSet lmdataset = new LoadSelectedDataSet("Load Baseline Data", "Baseline","Baseline", "Baseline");


				//DialogResult dlgr = lmdataset.ShowDialog();
				//if(dlgr.Equals(DialogResult.OK))
				//{
				//    txtBase.Text = lmdataset.StrPath;
				//    _metadataObj = lmdataset.MetadataObj;
				//}
				//#endregion
				OpenFileDialog openFileDialog = new OpenFileDialog();
				if (string.IsNullOrEmpty(pathBaseControl) || !System.IO.Directory.Exists(pathBaseControl))	//BenMAP 381: Simplified logic to set initial directory and filter once. 
					openFileDialog.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG\";
				else
					openFileDialog.InitialDirectory = pathBaseControl;

				if (txtPollutant.Text.Trim() == "")		
					openFileDialog.Filter = "AQG files (*.aqgx)|*.aqgx";
				else
					openFileDialog.Filter = "Supported File Types(*.csv, *.aqgx, *.xls, *.xlsx)|*.csv; *.aqgx; *.xls; *.xlsx|CSV file(*.csv)|*.csv|AQG files (*.aqgx)|*.aqgx|Excel file (*.xls)|*.xls|Excel file (*.xlsx)|*.xlsx";

				if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }		//BenMAP 381: Removed the line setting the filter index to the default value
				txtBase.Text = openFileDialog.FileName;
				int pathIdx = openFileDialog.FileName.LastIndexOf("\\");
				pathBaseControl = openFileDialog.FileName.Substring(0, pathIdx);  //BenMAP 381: Remember the directory where the user selected the file & remove file name 
				openFileDialog.RestoreDirectory = true;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}


		}

		private void btnCreatBase_Click(object sender, EventArgs e)
		{
			GridCreationMethods frm = new GridCreationMethods(bcgOpenAQG, "baseline");
			frm.ShowDialog();
		}

		private void btnOpenControl_Click(object sender, EventArgs e)
		{
			try
			{
				// 2015 02 05 replaced load dataset code with old (previously commented out code) to remove validataion requirement
				// initial directory path is wrong, however
				//#region Dead Code
				//LoadSelectedDataSet lmdataset = new LoadSelectedDataSet("Load Control Data", "Control Data", "Control", "Control");

				//DialogResult dlgr = lmdataset.ShowDialog();
				//if (dlgr.Equals(DialogResult.OK))
				//{
				//    txtControl.Text = lmdataset.StrPath;
				//}
				//#endregion

				OpenFileDialog openFileDialog = new OpenFileDialog();
				if (string.IsNullOrEmpty(pathBaseControl) || !System.IO.Directory.Exists(pathBaseControl))		//BenMAP 381: Simplified logic to set initial directory and filter once. 
					openFileDialog.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG\";
				else
					openFileDialog.InitialDirectory = pathBaseControl;
				
				if (txtPollutant.Text.Trim() == "")
					openFileDialog.Filter = "AQG files (*.aqgx)|*.aqgx";
				else
					openFileDialog.Filter = "Supported File Types(*.csv, *.aqgx, *.xls, *.xlsx)|*.csv; *.aqgx; *.xls; *.xlsx|CSV file(*.csv)|*.csv|AQG files (*.aqgx)|*.aqgx|Excel file (*.xls)|*.xls|Excel file (*.xlsx)|*.xlsx";

				if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }		//BenMAP 381: Removed the line setting the filter index to the default value
				txtControl.Text = openFileDialog.FileName;
				int pathIdx = openFileDialog.FileName.LastIndexOf("\\");				
				pathBaseControl = openFileDialog.FileName.Substring(0, pathIdx);  //BenMAP 381: Remember the directory where the user selected the file & remove file name 
				openFileDialog.RestoreDirectory = true;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void btnCreatControl_Click(object sender, EventArgs e)
		{
			GridCreationMethods frm = new GridCreationMethods(bcgOpenAQG, "control");
			frm.ShowDialog();
		}
		public bool isGridTypeChanged = false;
		public BenMAPGrid benMAPGridOld = null;
		private void btnOK_Click(object sender, EventArgs e)
		{
			try
			{
				if (txtBase.Text == string.Empty || txtControl.Text == string.Empty)
				{
					MessageBox.Show("Please select both the baseline and control file.", "Error");
					return;
				}
				DataRowView drv = cboGrid.SelectedItem as DataRowView;
				BenMAPGrid benMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drv["GridDefinitionID"]));

				if (CommonClass.GBenMAPGrid == null) CommonClass.GBenMAPGrid = benMAPGrid;
				else if (CommonClass.GBenMAPGrid.GridDefinitionID != benMAPGrid.GridDefinitionID)
				{
					benMAPGridOld = Grid.GridCommon.getBenMAPGridFromID(CommonClass.GBenMAPGrid.GridDefinitionID);
					CommonClass.GBenMAPGrid = benMAPGrid;
					isGridTypeChanged = true;
				}
				bcgOpenAQG.GridType = CommonClass.GBenMAPGrid;

				string inputFileFormat = string.Empty;

				saveBasePath = string.Empty;
				saveControlPath = string.Empty;
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.Filter = "AQG files (*.aqgx)|*.aqgx";
				sfd.RestoreDirectory = true;
				sfd.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG";
				if (!txtBase.Text.ToLower().EndsWith("aqgx"))
				{
					sfd.Title = "Save the baseline Grid.";
					if (sfd.ShowDialog() == DialogResult.OK)
						saveBasePath = sfd.FileName;
					else
						return;
				}
				int pathIdx = sfd.FileName.LastIndexOf("\\");
				sfd.InitialDirectory = saveBasePath.Substring(0, pathIdx);
				sfd.FileName = "";
				if (!txtControl.Text.ToLower().EndsWith("aqgx"))
				{
					sfd.Title = "Save the control Grid.";
					if (sfd.ShowDialog() == DialogResult.OK)
						saveControlPath = sfd.FileName;
					else
						return;
				}

				//WaitShow("Loading AQ data file...");
				if (txtBase.Text != "")
				{
					basePath = "Model Data:" + txtBase.Text;
					inputFileFormat = basePath.Substring(basePath.LastIndexOf("."));
					switch (inputFileFormat)
					{
						case ".csv":
							if (!File.Exists(txtBase.Text)) { MessageBox.Show("File not found.", "Error"); return; }
							if (txtPollutant.Text == bcgOpenAQG.Pollutant.PollutantName)
							{
								bcgOpenAQG.GridType = CommonClass.GBenMAPGrid;
								CreateShapeFile(bcgOpenAQG, "baseline", txtBase.Text);
								bcgOpenAQG.Base.CreateTime = DateTime.Now;
							}
							break;
						case ".aqgx":
							openAQG(txtBase.Text, "baseline", bcgOpenAQG);
							break;
						case ".xls":
							if (!File.Exists(txtBase.Text)) { MessageBox.Show("File not found.", "Error"); return; }
							if (txtPollutant.Text == bcgOpenAQG.Pollutant.PollutantName)
							{
								bcgOpenAQG.GridType = CommonClass.GBenMAPGrid;
								CreateShapeFile(bcgOpenAQG, "baseline", txtBase.Text);
								bcgOpenAQG.Base.CreateTime = DateTime.Now;
							}
							break;
						case ".xlsx":
							if (!File.Exists(txtBase.Text)) { MessageBox.Show("File not found.", "Error"); return; }
							if (txtPollutant.Text == bcgOpenAQG.Pollutant.PollutantName)
							{
								bcgOpenAQG.GridType = CommonClass.GBenMAPGrid;
								CreateShapeFile(bcgOpenAQG, "baseline", txtBase.Text);
								bcgOpenAQG.Base.CreateTime = DateTime.Now;
							}
							break;

					}

				}
				if (txtControl.Text != "")
				{
					controlPath = "Model Data:" + txtControl.Text;
					inputFileFormat = controlPath.Substring(controlPath.LastIndexOf("."));
					switch (inputFileFormat)
					{
						case ".csv":
							if (!File.Exists(txtControl.Text)) { MessageBox.Show("File not found.", "Error"); return; }
							if (txtPollutant.Text == bcgOpenAQG.Pollutant.PollutantName)
							{
								CreateShapeFile(bcgOpenAQG, "control", txtControl.Text);
								bcgOpenAQG.Control.CreateTime = DateTime.Now;
							}
							break;
						case ".aqgx":
							openAQG(txtControl.Text, "control", bcgOpenAQG);
							break;
						case ".xls":
							if (!File.Exists(txtControl.Text)) { MessageBox.Show("File not found.", "Error"); return; }
							if (txtPollutant.Text == bcgOpenAQG.Pollutant.PollutantName)
							{
								CreateShapeFile(bcgOpenAQG, "control", txtControl.Text);
								bcgOpenAQG.Control.CreateTime = DateTime.Now;
							}
							break;
						case ".xlsx":
							if (!File.Exists(txtControl.Text)) { MessageBox.Show("File not found.", "Error"); return; }
							if (txtPollutant.Text == bcgOpenAQG.Pollutant.PollutantName)
							{
								CreateShapeFile(bcgOpenAQG, "control", txtControl.Text);
								bcgOpenAQG.Control.CreateTime = DateTime.Now;
							}
							break;
					}

				}
				//WaitClose();
				this.DialogResult = DialogResult.OK;
			}
			catch (Exception ex)
			{
				WaitClose();
				Logger.LogError(ex);
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			if (isGridTypeChanged)
			{
				CommonClass.GBenMAPGrid = benMAPGridOld;
				isGridTypeChanged = false;
			}
			this.DialogResult = DialogResult.Cancel;

		}

		private void openAQG(string sourceFilePath, string currentState, BaseControlGroup bcg)
		{
			try
			{
				string err = "";
				BenMAPLine benMapLine = DataSourceCommonClass.LoadAQGFile(sourceFilePath, ref err);
				if (benMapLine == null)
				{
					WaitClose();
					MessageBox.Show(err);
					return;
				}

				if (bcg.Pollutant != null && benMapLine.Pollutant.PollutantID != bcg.Pollutant.PollutantID)
				{
					WaitClose();
					MessageBox.Show("The AQG's pollutant does not match the selected pollutant. Please select another file.");
					if (isGridTypeChanged)
					{
						CommonClass.GBenMAPGrid = benMAPGridOld;
						isGridTypeChanged = false;
					}

					return;
				}
				else if (benMapLine.GridType.GridDefinitionID != bcg.GridType.GridDefinitionID)
				{
					WaitClose();
					MessageBox.Show("The AQG's grid definition does not match the selected grid definition. Please select another file.");
					if (isGridTypeChanged)
					{
						CommonClass.GBenMAPGrid = benMAPGridOld;
						isGridTypeChanged = false;
					}

					return;
				}
				if (bcg.Pollutant == null) bcg.Pollutant = benMapLine.Pollutant;
				if (benMapLine.ShapeFile != null && !benMapLine.ShapeFile.Contains(@"\"))
				{
					string AppPath = Application.StartupPath;
					string _filePath = sourceFilePath.Substring(0, sourceFilePath.LastIndexOf(@"\") + 1);
					string strShapePath = string.Format("{0}\\Tmp\\{1}", CommonClass.DataFilePath, benMapLine.ShapeFile);
					benMapLine.ShapeFile = _filePath + benMapLine.ShapeFile;
					DataSourceCommonClass.SaveBenMAPLineShapeFile(bcg.GridType, bcg.Pollutant, benMapLine, strShapePath);
				}
				else if (benMapLine.ShapeFile != null && benMapLine.ShapeFile.Contains(@"\"))
				{
					DataSourceCommonClass.SaveBenMAPLineShapeFile(bcg.GridType, bcg.Pollutant, benMapLine, benMapLine.ShapeFile);

				}
				switch (currentState)
				{
					case "baseline":
						if (bcg.Pollutant == null || bcg.Pollutant.PollutantID == benMapLine.Pollutant.PollutantID)
						{
							bcg.Base = benMapLine; if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0)
							{
								CommonClass.LstPollutant = new List<BenMAPPollutant>();
								CommonClass.LstPollutant.Add(benMapLine.Pollutant);
								bcg.Pollutant = benMapLine.Pollutant;

							}
						}

						break;
					case "control":
						if (bcg.Pollutant == null || bcg.Pollutant.PollutantID == benMapLine.Pollutant.PollutantID)
							bcg.Control = benMapLine; if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0)
						{
							CommonClass.LstPollutant = new List<BenMAPPollutant>();
							CommonClass.LstPollutant.Add(benMapLine.Pollutant);
							bcg.Pollutant = benMapLine.Pollutant;

						}
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
		private void CreateShapeFile(BaseControlGroup b, string state, string filePath)
		{
			
	 string msg = string.Empty;

			ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
			ModelDataLine modelDataLine = new ModelDataLine(); try
			{

				modelDataLine.DatabaseFilePath = filePath;
				System.Data.DataTable dtModel = CommonClass.ExcelToDataTable(filePath);
				DataSourceCommonClass.UpdateModelDataLineFromDataSet(b.Pollutant, modelDataLine, dtModel);

				switch (state)
				{
					case "baseline":
						b.Base = null;
						b.Base = modelDataLine;
						break;
					case "control":
						b.Control = null;
						b.Control = modelDataLine;
						break;
				}
				if (modelDataLine.ModelAttributes.Count == 0)
				{
					msg = "Error reading files.";
					return;
				}
				int threadId = -1;
				AsyncDelegate asyncD = new AsyncDelegate(AsyncCreateFile);
				IAsyncResult ar = asyncD.BeginInvoke(b, modelDataLine, state, out threadId, null, null);
				return;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				return;
			}
			finally
			{
				if (msg != string.Empty)
				{ MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
			}
		}

		private string AsyncCreateFile(BaseControlGroup bcg, ModelDataLine m, string currentStat, out int threadId)
		{
			threadId = -1;
			string shapeFile = string.Empty;
			string strShapePath = string.Empty;
			string AppPath = Application.StartupPath;
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
				{ CommonClass.NodeAnscyStatus = string.Format("{0};{1};on", bcg.Pollutant.PollutantName.ToLower(), currentStat); }

				DateTime dt = DateTime.Now;
				shapeFile = string.Format("{0}{1}{2}{3}{4}{5}.shp", new string[] { bcg.Pollutant.PollutantName.ToLower(), currentStat, dt.ToString("yyyyMMdd"), dt.Hour.ToString("00"), dt.Minute.ToString("00"), dt.Second.ToString("00") });
				Random random = new Random();
				shapeFile = string.Format("{0}{1}{2}.shp", new string[] { bcg.Pollutant.PollutantName.ToLower(), currentStat, random.Next(100).ToString() });
				shapeFile = bcg.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "B" + currentStat + ".shp";
				strShapePath = string.Format("{0}\\Tmp\\{1}", CommonClass.DataFilePath, shapeFile);

				DataSourceCommonClass.UpdateModelValuesModelData(DataSourceCommonClass.DicSeasonStaticsAll, bcg.GridType, bcg.Pollutant, m, strShapePath);
				lock (CommonClass.LstAsynchronizationStates)
				{
					CommonClass.LstAsynchronizationStates.Remove(str);
					if (CommonClass.LstAsynchronizationStates.Count == 0)
					{
						CommonClass.CurrentMainFormStat = "Current Setup: " + CommonClass.MainSetup.SetupName;
					}
				}
				lock (CommonClass.NodeAnscyStatus)
				{ CommonClass.NodeAnscyStatus = string.Format("{0};{1};off", bcg.Pollutant.PollutantName.ToLower(), currentStat); }
				switch (currentStat)
				{
					case "baseline":
						if (!string.IsNullOrEmpty(saveBasePath))
							DataSourceCommonClass.CreateAQGFromBenMAPLine(bcgOpenAQG.Base, saveBasePath);
						break;
					case "control":
						if (!string.IsNullOrEmpty(saveControlPath))
							DataSourceCommonClass.CreateAQGFromBenMAPLine(bcgOpenAQG.Control, saveControlPath);
						break;
				}

				return str;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				return string.Empty;
			}
		}
	}
}
