using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESIL.DBUtility;
using System.IO;

namespace BenMAP
{
    public partial class LoadAirQualityData : FormBase
    {
        public BaseControlGroup bcgOpenAQG;
        string _setupName = "";
        string _gridType = "";
        string _pollutantName = "";
        string _baseFile = "";
        string _controlFile = "";
        int _setupID = 0;
        int _pollutantID = 0;
        int _gridID = 0;
        string saveBasePath = string.Empty;
        string saveControlPath = string.Empty;

        public LoadAirQualityData(string info)
        {
            InitializeComponent();
            _setupName = info.Split('*')[1];
            _gridType = info.Split('*')[2];
            _pollutantName = info.Split('*')[3];
            _baseFile = info.Split('*')[4];
            _controlFile = info.Split('*')[5];
        }

        private void LoadAirQualityData_Load(object sender, EventArgs e)
        {
            try
            {
                FireBirdHelperBase fb = new ESILFireBirdHelper();
                string commandText = string.Empty;
                commandText = string.Format("select SetupID,SetupName from Setups order by SetupID");
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboSetup.DataSource = ds.Tables[0];
                cboSetup.DisplayMember = "SetupName";
                cboSetup.Text = _setupName;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void cboSetup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataRowView drv = cboSetup.SelectedItem as DataRowView;
                int SetupID = Convert.ToInt32(drv["SetupID"]);

                FireBirdHelperBase fb = new ESILFireBirdHelper();
                string commandText = string.Empty;
                CommonClass.MainSetup.SetupID = 0;
                commandText = string.Format("select PollutantID,PollutantName from Pollutants where setupid={0} order by PollutantName asc", SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                cboPollutant.DataSource = ds.Tables[0];
                cboPollutant.DisplayMember = "PollutantName";
                cboPollutant.Text = _pollutantName;

                fb = new ESIL.DBUtility.ESILFireBirdHelper();
                commandText = string.Format("select GridDefinitionID,GridDefinitionName from GridDefinitions where setupID={0}", SetupID);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboGridType.DataSource = ds.Tables[0];
                cboGridType.DisplayMember = "GridDefinitionName";
                cboGridType.Text = _gridType;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                DataRowView drv = cboSetup.SelectedItem as DataRowView;
                _setupID = Convert.ToInt32(drv["SetupID"]);
                drv = cboPollutant.SelectedItem as DataRowView;
                _pollutantID = Convert.ToInt32(drv["PollutantID"]);
                drv = cboGridType.SelectedItem as DataRowView;
                _gridID = Convert.ToInt32(drv["GridDefinitionID"]);

                CommonClass.MainSetup = CommonClass.getBenMAPSetupFromID(_setupID);
                CommonClass.ManageSetup = CommonClass.getBenMAPSetupFromID(_setupID);
                bcgOpenAQG = new BaseControlGroup();
                BenMAPPollutant pollutant = Grid.GridCommon.getPollutantFromID(_pollutantID);
                if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0)
                {
                    CommonClass.LstBaseControlGroup = new List<BaseControlGroup>();
                    CommonClass.LstBaseControlGroup.Add(new BaseControlGroup());
                    bcgOpenAQG = CommonClass.LstBaseControlGroup[0];
                    CommonClass.LstPollutant = new List<BenMAPPollutant>();
                    CommonClass.LstPollutant.Add(pollutant);
                }
                CommonClass.GBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(_gridID);

                saveBasePath = string.Empty;
                saveControlPath = string.Empty;
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "AQG files (*.aqgx)|*.aqgx";
                sfd.RestoreDirectory = true;
                sfd.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG";
                sfd.Title = "Save the baseline Grid.";
                if (sfd.ShowDialog() == DialogResult.OK)
                    saveBasePath = sfd.FileName;
                else
                    return;
                sfd.FileName = "";
                sfd.Title = "Save the control Grid.";
                if (sfd.ShowDialog() == DialogResult.OK)
                    saveControlPath = sfd.FileName;
                else
                    return;

                if (!File.Exists(_baseFile)) { MessageBox.Show("Baseline File not found.", "Error"); return; }
                bcgOpenAQG.Pollutant = pollutant;
                bcgOpenAQG.GridType = CommonClass.GBenMAPGrid;
                CreateShapeFile(bcgOpenAQG, "baseline", _baseFile);
                bcgOpenAQG.Base.CreateTime = DateTime.Now;

                if (!File.Exists(_controlFile)) { MessageBox.Show("Control File not found.", "Error"); return; }
                CreateShapeFile(bcgOpenAQG, "control", _controlFile);
                bcgOpenAQG.Control.CreateTime = DateTime.Now;

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void CreateShapeFile(BaseControlGroup b, string state, string filePath)
        {
            string msg = string.Empty;

            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            ModelDataLine modelDataLine = new ModelDataLine();
            try
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
