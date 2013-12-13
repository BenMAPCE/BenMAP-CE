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
                // 必须这样写，否则会出错
                DataTable dtGrid = ds.Tables[0].Clone();
                dtGrid = ds.Tables[0].Copy();
                cboGrid.DataSource = dtGrid;
                cboGrid.DisplayMember = "GridDefinitionName";
                for (int i = 0; i < dtGrid.Rows.Count; i++)
                {
                    if (dtGrid.Rows[i]["defaulttype"].ToString() == "1" &&CommonClass.GBenMAPGrid == null)
                    {
                        cboGrid.SelectedIndex = i;
                        break;
                    }
                    else if(CommonClass.GBenMAPGrid!=null && Convert.ToInt32(dtGrid.Rows[i]["GridDefinitionID"])==CommonClass.GBenMAPGrid.GridDefinitionID)
                    {
                            cboGrid.SelectedIndex = i;
                        break;
                    
                    }

                }
                //---------------------判断是否在异步，在异步不能修改GridType-------------------
                if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0)
                {
                    cboGrid.Enabled = false;

                }
                //if (CommonClass.GBenMAPGrid != null) { cboGrid.Text = CommonClass.GBenMAPGrid.GridDefinitionName; }
                //else
                //{
                //    DataRowView drv = cboGrid.SelectedItem as DataRowView;
                //    CommonClass.GBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drv["GridDefinitionID"]));
                //}

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
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (string.IsNullOrEmpty(pathBaseControl) || !System.IO.Directory.Exists(pathBaseControl))
                {
                    openFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath + @"\Data\SampleData\";
                }
                //openFileDialog.InitialDirectory = Application.StartupPath + @"\Result\AQG";
                if (txtPollutant.Text.Trim() == "")
                {
                    openFileDialog.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG\";
                    openFileDialog.Filter = "AQG files (*.aqgx)|*.aqgx";
                }
                else
                {
                    openFileDialog.Filter = "CSV file(*.csv)|*.csv|AQG files (*.aqgx)|*.aqgx|Excel file (*.xls)|*.xls|Excel file (*.xlsx)|*.xlsx";
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
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (string.IsNullOrEmpty(pathBaseControl) || !System.IO.Directory.Exists(pathBaseControl))
                {
                    openFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath + @"\Data\SampleData\";
                }
                //openFileDialog.InitialDirectory = Application.StartupPath + @"\Result\AQG";
                if (txtPollutant.Text.Trim() == "")
                {
                    openFileDialog.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG\";
                    openFileDialog.Filter = "AQG files (*.aqgx)|*.aqgx";
                }
                else
                {
                    openFileDialog.Filter = "CSV file(*.csv)|*.csv|AQG files (*.aqgx)|*.aqgx|Excel file (*.xls)|*.xls|Excel file (*.xlsx)|*.xlsx";
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
        public  bool isGridTypeChanged = false;
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
                //-----------------set Grid--------------
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
                //-------open base.aqg ------majie-----

                //如果是csv或者excel文件，就存aqg
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
                //-------open control.aqg------majie-----            
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
                //WaitShow(tip);
                string err = "";
                BenMAPLine benMapLine = DataSourceCommonClass.LoadAQGFile(sourceFilePath,ref err);
                if (benMapLine == null)
                {
                    //--------------丁点加提示
                    WaitClose();
                    MessageBox.Show(err);
                    return;
                }
                //WaitClose();

                if (bcg.Pollutant != null&&benMapLine.Pollutant.PollutantID != bcg.Pollutant.PollutantID)
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
                    //if (File.Exists(_filePath + benMapLine.ShapeFile))
                    //{
                    //    // File.Copy(_filePath + @"\" + benMapLine.ShapeFile, strShapePath);
                    benMapLine.ShapeFile = _filePath + benMapLine.ShapeFile;
                    //}
                    //else
                    //{
                    //---------------------
                    DataSourceCommonClass.SaveBenMAPLineShapeFile(bcg.GridType, bcg.Pollutant, benMapLine, strShapePath);
                    //}
                }
                else if (benMapLine.ShapeFile != null && benMapLine.ShapeFile.Contains(@"\"))
                {
                    DataSourceCommonClass.SaveBenMAPLineShapeFile(bcg.GridType, bcg.Pollutant, benMapLine, benMapLine.ShapeFile);

                }
                switch (currentState)
                {
                    case "baseline":
                        //-----如果有pollutant，判断是否一致!
                        if (bcg.Pollutant == null || bcg.Pollutant.PollutantID == benMapLine.Pollutant.PollutantID)
                        {
                            bcg.Base = benMapLine;//DataSourceCommonClass.LoadAQGFile(txtExistingAQG.Text);
                            if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0)
                            {
                                CommonClass.LstPollutant = new List<BenMAPPollutant>();
                                CommonClass.LstPollutant.Add(benMapLine.Pollutant);
                                bcg.Pollutant = benMapLine.Pollutant;
                                
                            }
                        }

                        break;
                    case "control":
                        //-----如果有pollutant，判断是否一致!
                        if (bcg.Pollutant == null || bcg.Pollutant.PollutantID == benMapLine.Pollutant.PollutantID)
                        bcg.Control = benMapLine;// DataSourceCommonClass.LoadAQGFile(txtExistingAQG.Text);
                        if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0)
                        {
                            CommonClass.LstPollutant = new List<BenMAPPollutant>();
                            CommonClass.LstPollutant.Add(benMapLine.Pollutant);
                            bcg.Pollutant = benMapLine.Pollutant;
                                
                        }
                        break;
                }//swith
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
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
        //Dictionary<int, string> dicSeasonStatics = new Dictionary<int, string>();
        private void CreateShapeFile(BaseControlGroup b, string state, string filePath)
        {
            string msg = string.Empty;
            
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            //if (dicSeasonStatics.Count == 0)
            //{
            //    string commandText = string.Format("select * from SEASONALMETRICSEASONS where POLLUTANTSEASONID in (select POLLUTANTSEASONID from POLLUTANTSEASONS where pollutantid= " + b.Pollutant.PollutantID + ")");
            //    System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
            //    foreach (DataRow dr in ds.Tables[0].Rows)
            //    {
            //        dicSeasonStatics.Add(Convert.ToInt32(dr["PollutantSeasonID"]), dr["METRICFUNCTION"].ToString());
            //    }
            //}
            ModelDataLine modelDataLine = new ModelDataLine(); // TODO: 初始化为适当的值
            try
            {
                //WaitShow("Loading model data file!");

                modelDataLine.DatabaseFilePath = filePath;
                System.Data.DataTable dtModel = CommonClass.ExcelToDataTable(filePath);
                DataSourceCommonClass.UpdateModelDataLineFromDataSet(b.Pollutant, modelDataLine, dtModel);

                //WaitClose();
                switch (state)
                {
                    case "baseline":
                        b.Base = null;
                        b.Base = modelDataLine;
                        break;
                    //changeNodeImage(currentNode);
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
                //------------updateModelValues--------异步
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

        /// <summary>
        /// 异步调用生成ShapeFile
        /// </summary>
        /// <param name="bcg">DataSource对应的BaseLine和Control的对应内容</param>
        /// <param name="m">上一步的模型属性</param>
        /// <param name="currentStat">当前状态：baseline/control</param>
        /// <param name="threadId">线程ID</param>
        /// <returns></returns>
        private string AsyncCreateFile(BaseControlGroup bcg, ModelDataLine m, string currentStat, out int threadId)
        {
            threadId = -1;
            // ShapeFile的命名规范：污染物+baseline/control+当前做操时间+.shp,例如：pm10baseline20110921090622.shp
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
                        //CommonClass.NodeAnscyStatus = string.Format("{0};on", _currentStat);
                    }
                }
                lock (CommonClass.NodeAnscyStatus)
                { CommonClass.NodeAnscyStatus = string.Format("{0};{1};on", bcg.Pollutant.PollutantName.ToLower(), currentStat); }

                DateTime dt = DateTime.Now;
                shapeFile = string.Format("{0}{1}{2}{3}{4}{5}.shp", new string[] { bcg.Pollutant.PollutantName.ToLower(), currentStat, dt.ToString("yyyyMMdd"), dt.Hour.ToString("00"), dt.Minute.ToString("00"), dt.Second.ToString("00") });
                //----------------------modify by xiejp 20110927---为了不产生太多的shp，把临时文件的名称变为污染物名称+base(control)+.shp
                Random random = new Random();
                shapeFile = string.Format("{0}{1}{2}.shp", new string[] { bcg.Pollutant.PollutantName.ToLower(), currentStat, random.Next(100).ToString() });
                //Todo:陈志润 20111128
                //shapeFile = bcg.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "B" + currentStat.Substring(0,1)+".shp";
                shapeFile = bcg.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "B" + currentStat + ".shp";
                strShapePath = string.Format("{0}\\Tmp\\{1}", CommonClass.DataFilePath, shapeFile);

                DataSourceCommonClass.UpdateModelValuesModelData(DataSourceCommonClass.DicSeasonStaticsAll,bcg.GridType, bcg.Pollutant, m, strShapePath);
                lock (CommonClass.LstAsynchronizationStates)
                {
                    CommonClass.LstAsynchronizationStates.Remove(str);
                    if (CommonClass.LstAsynchronizationStates.Count == 0)
                    {
                        CommonClass.CurrentMainFormStat = "Current Setup: " + CommonClass.MainSetup.SetupName;
                        //CommonClass.NodeAnscyStatus = string.Format("{0};off", _currentStat);
                    }
                }//lock

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
