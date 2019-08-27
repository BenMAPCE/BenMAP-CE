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
                if (string.IsNullOrEmpty(pathBaseControl) || !System.IO.Directory.Exists(pathBaseControl))
                {
                    openFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath + @"\Data\SampleData\";
                }
                if (txtPollutant.Text.Trim() == "")
                {
                    openFileDialog.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG\";
                    openFileDialog.Filter = "AQG files (*.aqgx)|*.aqgx";
                }
                else
                {
                    openFileDialog.Filter = "Supported File Types(*.csv, *.aqgx, *.xls, *.xlsx)|*.csv; *.aqgx; *.xls; *.xlsx|CSV file(*.csv)|*.csv|AQG files (*.aqgx)|*.aqgx|Excel file (*.xls)|*.xls|Excel file (*.xlsx)|*.xlsx";
                }
                openFileDialog.FilterIndex = 1;
                if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
                txtBase.Text = openFileDialog.FileName;
                pathBaseControl = System.IO.Directory.GetCurrentDirectory();
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
                if (string.IsNullOrEmpty(pathBaseControl) || !System.IO.Directory.Exists(pathBaseControl))
                {
                    openFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath + @"\Data\SampleData\";
                }
                if (txtPollutant.Text.Trim() == "")
                {
                    openFileDialog.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG\";
                    openFileDialog.Filter = "AQG files (*.aqgx)|*.aqgx";
                }
                else
                {
                    openFileDialog.Filter = "Supported File Types(*.csv, *.aqgx, *.xls, *.xlsx)|*.csv; *.aqgx; *.xls; *.xlsx|CSV file(*.csv)|*.csv|AQG files (*.aqgx)|*.aqgx|Excel file (*.xls)|*.xls|Excel file (*.xlsx)|*.xlsx";
                }
                openFileDialog.FilterIndex = 1;
                if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
                txtControl.Text = openFileDialog.FileName;
                pathBaseControl = System.IO.Directory.GetCurrentDirectory();
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
                sfd.FileName = "";
                if (!txtControl.Text.ToLower().EndsWith("aqgx"))
                {
                    sfd.Title = "Save the control Grid.";
                    if (sfd.ShowDialog() == DialogResult.OK)
                        saveControlPath = sfd.FileName;
                    else
                        return;
                }

                WaitShow("Loading AQ data file...");
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
                                QueueCreateShapeFile(bcgOpenAQG, "baseline", txtBase.Text, saveBasePath);
                                //bcgOpenAQG.Base.CreateTime = DateTime.Now;
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
                                QueueCreateShapeFile(bcgOpenAQG, "baseline", txtBase.Text, saveBasePath);
                                //bcgOpenAQG.Base.CreateTime = DateTime.Now;
                            }
                            break;
                        case ".xlsx":
                            if (!File.Exists(txtBase.Text)) { MessageBox.Show("File not found.", "Error"); return; }
                            if (txtPollutant.Text == bcgOpenAQG.Pollutant.PollutantName)
                            {
                                bcgOpenAQG.GridType = CommonClass.GBenMAPGrid;
                                QueueCreateShapeFile(bcgOpenAQG, "baseline", txtBase.Text, saveBasePath);
                                //bcgOpenAQG.Base.CreateTime = DateTime.Now;
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
                                QueueCreateShapeFile(bcgOpenAQG, "control", txtControl.Text, saveControlPath);
                                //bcgOpenAQG.Control.CreateTime = DateTime.Now;
                            }
                            break;
                        case ".aqgx":
                            openAQG(txtControl.Text, "control", bcgOpenAQG);
                            break;
                        case ".xls":
                            if (!File.Exists(txtControl.Text)) { MessageBox.Show("File not found.", "Error"); return; }
                            if (txtPollutant.Text == bcgOpenAQG.Pollutant.PollutantName)
                            {
                                QueueCreateShapeFile(bcgOpenAQG, "control", txtControl.Text, saveControlPath);
                                //bcgOpenAQG.Control.CreateTime = DateTime.Now;
                            }
                            break;
                        case ".xlsx":
                            if (!File.Exists(txtControl.Text)) { MessageBox.Show("File not found.", "Error"); return; }
                            if (txtPollutant.Text == bcgOpenAQG.Pollutant.PollutantName)
                            {
                                QueueCreateShapeFile(bcgOpenAQG, "control", txtControl.Text, saveControlPath);
                                //bcgOpenAQG.Control.CreateTime = DateTime.Now;
                            }
                            break;
                    }

                }
                WaitClose();
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

        private void QueueCreateShapeFile(BaseControlGroup b, string state, string filePath, string aqgxPath)
        {

            // Add the parameters of this call to globally accessible list
            CreateShapeFileParams p = new CreateShapeFileParams();
            p.b = b;
            p.state = state;
            p.filePath = filePath;
            p.aqgxPath = aqgxPath;

            // Just in case the user has updated a previous configured surface
            if(state == "baseline")
            {
                b.Base = null;
            } else if (state == "control")
            {
                b.Control = null;
            }


            Boolean isNew = true;
            if(CommonClass.LstCreateShapeFileParams == null)
            {
                CommonClass.LstCreateShapeFileParams = new List<CreateShapeFileParams>();
            }
            // See if the user updated an existing AQ surface
            for(int i=0; i < CommonClass.LstCreateShapeFileParams.Count; i++)
            {
                CreateShapeFileParams ParamsFromQueue = CommonClass.LstCreateShapeFileParams[i];
                if (ParamsFromQueue.b.Pollutant.PollutantID == p.b.Pollutant.PollutantID && ParamsFromQueue.state == p.state)
                {
                    // The user has updated an existing surface
                    CommonClass.LstCreateShapeFileParams[i] = p;
                    isNew = false;
                    break;
                }
            }
            // Or, if the user set up a new surface
            if(isNew)
            {
                CommonClass.LstCreateShapeFileParams.Add(p);
            }


            //Check to see if all BaseControlGroups are completely populated, or ready to create now.
            Boolean AllSurfacesAreReady = true;
            foreach (BaseControlGroup b1 in CommonClass.LstBaseControlGroup)
            {
                Boolean BaseIsGood = false;
                Boolean ControlIsGood = false;

                // Make sure this base surface is set up, or we have a parameter to do so...
                if (b1.Base == null)
                {
                    // Then look to see if we have params for it

                    for (int i = 0; i < CommonClass.LstCreateShapeFileParams.Count; i++)
                    {
                        CreateShapeFileParams ParamsFromQueue = CommonClass.LstCreateShapeFileParams[i];
                        if (ParamsFromQueue.b.Pollutant.PollutantID == b1.Pollutant.PollutantID && ParamsFromQueue.state == "baseline")
                        {
                            BaseIsGood = true;
                            break;
                        }
                    }
                } else
                {
                    BaseIsGood = true;
                }

                // Make sure this control surface is set up, or we have a parameter to do so...
                if (b1.Control == null)
                {
                    // Then look to see if we have params for it
                    for (int i = 0; i < CommonClass.LstCreateShapeFileParams.Count; i++)
                    {
                        CreateShapeFileParams ParamsFromQueue = CommonClass.LstCreateShapeFileParams[i];
                        if (ParamsFromQueue.b.Pollutant.PollutantID == b1.Pollutant.PollutantID && ParamsFromQueue.state == "control")
                        {
                            ControlIsGood = true;
                            break;
                        }
                    }
                }
                else
                {
                    ControlIsGood = true;
                }
                if(!BaseIsGood || !ControlIsGood)
                {
                    AllSurfacesAreReady = false;
                    break;
                }
            }

            if(AllSurfacesAreReady && state == "control") //A slight kludge, but we'll always hit the control after the baseline, so the second check avoids some rarely occurring issues
            {
                // Create the surfaces
                CreateShapeFileBatch(CommonClass.LstCreateShapeFileParams);
                CommonClass.LstCreateShapeFileParams.Clear();
            }

        }

        private void CreateShapeFile(BaseControlGroup b, string state, string filePath)
        {
            string msg = string.Empty;
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
                //AsyncDelegate asyncD = new AsyncDelegate(AsyncCreateFile);
                //IAsyncResult ar = asyncD.BeginInvoke(b, modelDataLine, state, out threadId, null, null);
                string ret = AsyncCreateFile(b, modelDataLine, state, out threadId); //, null, null);
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


        private void CreateShapeFileBatch(List<CreateShapeFileParams> LstParams)
        {
            string msg = String.Empty;

            // Load all the datasets           
            foreach (CreateShapeFileParams c in LstParams)
            {
                ModelDataLine modelDataLine = new ModelDataLine();
                try
                {
                    modelDataLine.DatabaseFilePath = c.filePath;
                    System.Data.DataTable dtModel = CommonClass.ExcelToDataTable(c.filePath);
                    DataSourceCommonClass.UpdateModelDataLineFromDataSet(c.b.Pollutant, modelDataLine, dtModel);

                    switch (c.state)
                    {
                        case "baseline":
                            c.b.Base = null;
                            c.b.Base = modelDataLine;
                            break;
                        case "control":
                            c.b.Control = null;
                            c.b.Control = modelDataLine;
                            break;
                    }
                    if (modelDataLine.ModelAttributes.Count == 0)
                    {
                        msg = "Error reading files.";
                        return;
                    }
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


            // Clean up missing days in model data
            // Assume all pollutants include the same number of values (days)
            int DataValueCount = CommonClass.LstBaseControlGroup[0].Base.ModelAttributes[0].Values.Count;
            // For each day
            for (int CurrentDayIdx = 0; CurrentDayIdx < DataValueCount; CurrentDayIdx++)
            {
                // For each cell
                for (int CurrentCellIdx = 0; CurrentCellIdx < CommonClass.LstBaseControlGroup[0].Base.ModelAttributes.Count; CurrentCellIdx++)
                {
                    // Check for missing value
                    Boolean isDayMissingBase = false;
                    Boolean isDayMissingControl = false;
                    foreach (BaseControlGroup bcg in CommonClass.LstBaseControlGroup)
                    {
                        if(bcg.Base.ModelAttributes.Count > CurrentCellIdx && bcg.Base.ModelAttributes[CurrentCellIdx].SeasonalMetric == null)
                        {
                            if (bcg.Base.ModelAttributes[CurrentCellIdx].Values[CurrentDayIdx] == float.MinValue)
                            {
                                isDayMissingBase = true;
                            }
                        }

                        if (bcg.Control.ModelAttributes.Count > CurrentCellIdx && bcg.Control.ModelAttributes[CurrentCellIdx].SeasonalMetric == null)
                        {
                            if (bcg.Control.ModelAttributes[CurrentCellIdx].Values[CurrentDayIdx] == float.MinValue)
                            {
                                isDayMissingControl = true;
                            }
                        }
                        if (isDayMissingBase && isDayMissingControl)
                        {
                            break;
                        }
                    }
                    // If we found a missing value in any pollutant for this day and cell, clear values for all pollutants for this day and cell
                    if (isDayMissingBase)
                    {
                        //Console.WriteLine("Clearing data for day={0}, cell={1}", CurrentDayIdx, CurrentCellIdx);
                        foreach (BaseControlGroup bcg in CommonClass.LstBaseControlGroup)
                        {
                            bcg.Base.ModelAttributes[CurrentCellIdx].Values[CurrentDayIdx] = float.MinValue;
                        }
                    }
                    if (isDayMissingControl)
                    {
                        //Console.WriteLine("Clearing data for day={0}, cell={1}", CurrentDayIdx, CurrentCellIdx);
                        foreach (BaseControlGroup bcg in CommonClass.LstBaseControlGroup)
                        {
                            bcg.Control.ModelAttributes[CurrentCellIdx].Values[CurrentDayIdx] = float.MinValue;
                        }
                    }
                }
            }

            // Create the shapefiles using cleaned up data
            foreach (CreateShapeFileParams c in LstParams)
            {
                int threadId = -1;
                //AsyncDelegate asyncD = new AsyncDelegate(AsyncCreateFile);
                //IAsyncResult ar = asyncD.BeginInvoke(b, modelDataLine, state, out threadId, null, null);

                if (c.state == "baseline")
                {
                    saveBasePath = c.aqgxPath;
                    string ret = AsyncCreateFile(c.b, (ModelDataLine)c.b.Base, c.state, out threadId); //, null, null);
                    c.b.Base.CreateTime = DateTime.Now;
                }
                else if (c.state == "control")
                {
                    saveControlPath = c.aqgxPath;
                    string ret = AsyncCreateFile(c.b, (ModelDataLine)c.b.Control, c.state, out threadId); //, null, null);
                    c.b.Control.CreateTime = DateTime.Now;
                }
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

                //TODO: Three different ways of computing shapeFile name?
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
                            DataSourceCommonClass.CreateAQGFromBenMAPLine(bcg.Base, saveBasePath);
                        break;
                    case "control":
                        if (!string.IsNullOrEmpty(saveControlPath))
                            DataSourceCommonClass.CreateAQGFromBenMAPLine(bcg.Control, saveControlPath);
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
