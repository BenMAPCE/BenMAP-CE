using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Data;
using LumenWorks.Framework.IO.Csv;


namespace BenMAP
{
    public partial class ModelData : FormBase
    {
        private MetadataClassObj _metadataObj = null;
        private BaseControlGroup _bgc = null;
        private string _currentStat = string.Empty;
        private string _isForceValidate = string.Empty;
        private string _iniPath = string.Empty;
        private string _strPath;
        private string _tabnameref = string.Empty;

        public string StrPath
        {
            get { return _strPath; }
            set { _strPath = value; }
        }
        public MetadataClassObj MetadataObj
        {
            get { return _metadataObj; }
        }

        public ModelData()
        {
            InitializeComponent();
        }

        public ModelData(BaseControlGroup currentPollutant, string currentStat)
        {
            InitializeComponent();
            _bgc = currentPollutant;
            _currentStat = currentStat;

            _iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
            _isForceValidate = CommonClass.IniReadValue("appSettings", "IsForceValidate", _iniPath);
            if (_isForceValidate == "T")
            {
                btnOK.Enabled = false;
            }
            else
            {
                btnOK.Enabled = true;
            }
        }

        private void ModelData_Load(object sender, EventArgs e)
        {
            try
            {
                if (CommonClass.GBenMAPGrid != null)
                {
                    txtGridType.Text = CommonClass.GBenMAPGrid.GridDefinitionName;
                    txtGridType.Enabled = false;
                }
                if (CommonClass.LstPollutant != null && CommonClass.LstPollutant.Count > 0)
                {
                    txtPollutant.Text = _bgc.Pollutant.PollutantName;
                    txtPollutant.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnBrowseDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = CommonClass.ResultFilePath;
                openFileDialog.Filter = "Supported File Types (*.csv, *.xls, *.xlsx)|*.csv; *.xls; *.xlsx|CSV files|*.csv|XLS files|*.xls|XLSX files|*.xlsx";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != DialogResult.OK) {
                    return;
                }
                txtModelDatabase.Text = openFileDialog.FileName;
                _tabnameref = string.Empty; //forget tab name if users select a different file after validation
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnBrowseFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = CommonClass.ResultFilePath;
                openFileDialog.Filter = "Supported File Types (*.csv, *.xls, *.xlsx)|*.csv; *.xls; *.xlsx|CSV files|*.csv|XLS files|*.xls|XLSX files|*.xlsx";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != DialogResult.OK) {
                    return;
                }
                txtModelFile.Text = openFileDialog.FileName;
                _tabnameref = string.Empty; //forget tab name if users select a different file after validation
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

        string saveAQGPath = string.Empty;
        BaseControlGroup saveBCG = new BaseControlGroup();
        private void btnOK_Click(object sender, EventArgs e)
        {
            LoadDatabase();
        }

        private void LoadDatabase()
        {
            string msg = string.Empty;
            bool bCreateShapeFile = false;
            try
            {
                if (tabControl1.SelectedIndex == 0 && string.IsNullOrEmpty(txtModelDatabase.Text)) { return; }
                if (tabControl1.SelectedIndex == 0 && !File.Exists(txtModelDatabase.Text)) { msg = string.Format("{0} not Exists !", "File"); return; }
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "AQG files (*.aqgx)|*.aqgx";
                sfd.FilterIndex = 2;
                sfd.RestoreDirectory = true;
                sfd.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    saveAQGPath = sfd.FileName;
                    if (tabControl1.SelectedIndex == 0)
                    {
                        if (!File.Exists(txtModelDatabase.Text)) { msg = string.Format("{0} does not exist.", "File"); return; }
                        _strPath = "Model Data: " + txtModelDatabase.Text;
                        foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
                        {
                            if (txtPollutant.Text == b.Pollutant.PollutantName)
                            {
                                saveBCG = b;
                                bCreateShapeFile = CreateShapeFile(b);
                                break;
                            }
                        }
                        this.DialogResult = DialogResult.OK;
                    }
                    else if (tabControl1.SelectedIndex == 1)
                    {
                        if (!File.Exists(txtModelFile.Text)) { msg = string.Format("{0} does not Exist.", "File"); return; }
                        if (!File.Exists(txtModelFile.Text)) { msg = string.Format("{0} does not Exist", txtModelFile.Text); return; }
                        _strPath = "Model Data: " + txtModelFile.Text;
                        foreach (BaseControlGroup b in CommonClass.LstBaseControlGroup)
                        {
                            if (txtPollutant.Text == b.Pollutant.PollutantName)
                            {
                                saveBCG = b;
                                bCreateShapeFile = CreateShapeFile(b);
                                break;
                            }
                        } if (bCreateShapeFile)
                        {
                            this.DialogResult = DialogResult.OK;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitClose();
                Logger.LogError(ex);
            }
            finally
            {
                if (msg != string.Empty)
                {
                    WaitClose();
                    { MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                }
            }
        }

        private bool CreateShapeFile(BaseControlGroup b)
        {
            string msg = string.Empty;
            System.Data.DataSet ds;
            string currentStat = string.Empty;
            try
            {
                currentStat = _currentStat;

                ModelDataLine modelDataLine = new ModelDataLine(); 
                if (tabControl1.SelectedIndex == 0)
                {
                    WaitShow("Loading model data file.");

                    modelDataLine.DatabaseFilePath = txtModelDatabase.Text;
                    //System.Data.DataTable dtModel = CommonClass.ExcelToDataTable(txtModelDatabase.Text);
                    DataTable dtModel = CommonClass.ExcelToDataTable(txtModelDatabase.Text, _tabnameref);
                    DataSourceCommonClass.UpdateModelDataLineFromDataSet(b.Pollutant, modelDataLine, dtModel);
                }
                else
                {
                    StreamReader sr = new StreamReader(txtModelFile.Text);
                    string csvDataLine;

                    csvDataLine = "";

                    string fileDataLine;

                    fileDataLine = sr.ReadLine();

                    string[] sMatchArray = fileDataLine.Split(new char[] { ',' });
                    if (!(sMatchArray[0].Replace("\"", "") == b.Pollutant.PollutantName && sMatchArray[1] == b.GridType.GridDefinitionName && sMatchArray[2].Replace("\"", "") == "Model"))
                    {
                        MessageBox.Show("The pollutant or grid definition do not match.");
                        return false;
                    }
                    WaitShow("Loading model data file...");
                    string strLine = "";
                    ds = new System.Data.DataSet();
                    DataTable dt = new DataTable();
                    DataRow dr = null;
                    string[] strArray;
                    strLine = sr.ReadLine();
                    if (strLine != null && strLine.Length > 0)
                    {
                        strArray = strLine.Split(',');
                        for (int i = 0; i < strArray.Count(); i++)
                        {
                            dt.Columns.Add(strArray[i]);
                        }

                    }
                    while (strLine != null)
                    {
                        strLine = sr.ReadLine();
                        if (strLine != null && strLine.Length > 0)
                        {
                            dr = dt.NewRow();
                            strArray = strLine.Split(',');
                            for (int i = 0; i < strArray.Count(); i++)
                            {
                                dr[i] = strArray[i];
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                    ds.Tables.Add(dt);
                    modelDataLine.DatabaseFilePath = txtModelFile.Text;
                    DataSourceCommonClass.UpdateModelDataLineFromDataSetNewFormat(b.Pollutant, ref modelDataLine, ds);
                    sr.Close();
                    switch (currentStat)
                    {
                        case "baseline":
                            b.Base = null;
                            b.Base = modelDataLine;
                            b.Base.GridType = b.GridType;
                            break;
                        case "control":
                            b.Control = null;
                            b.Control = modelDataLine;
                            b.Control.GridType = b.GridType;
                            break;
                    }
                }
                System.Threading.Thread.Sleep(100); WaitClose();
                switch (currentStat)
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
                if (modelDataLine.ModelAttributes.Count == 0 && modelDataLine.ModelResultAttributes.Count == 0)
                {
                    msg = "Error reading files.";
                    return false;
                }
                int threadId = -1;
                AsyncDelegate asyncD = new AsyncDelegate(AsyncCreateFile);
                IAsyncResult ar = asyncD.BeginInvoke(b, modelDataLine, currentStat, out threadId, null, null);
                return true;
            }
            catch (Exception ex)
            {
                WaitClose();
                Logger.LogError(ex);
                return false;
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
                { CommonClass.NodeAnscyStatus = string.Format("{0};{1};on", bcg.Pollutant.PollutantName.ToLower(), _currentStat); }

                DateTime dt = DateTime.Now;
                shapeFile = string.Format("{0}{1}{2}{3}{4}{5}.shp", new string[] { bcg.Pollutant.PollutantName.ToLower(), currentStat, dt.ToString("yyyyMMdd"), dt.Hour.ToString("00"), dt.Minute.ToString("00"), dt.Second.ToString("00") });
                Random random = new Random();
                shapeFile = string.Format("{0}{1}{2}.shp", new string[] { bcg.Pollutant.PollutantName.ToLower(), currentStat, random.Next(100).ToString() });
                shapeFile = bcg.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "B" + currentStat + ".shp";
                strShapePath = string.Format("{0}\\Tmp\\{1}", CommonClass.DataFilePath, shapeFile);
                Dictionary<int, string> dicSeasonStatics = new Dictionary<int, string>();

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
                { CommonClass.NodeAnscyStatus = string.Format("{0};{1};off", bcg.Pollutant.PollutantName.ToLower(), _currentStat); }
                switch (_currentStat)
                {
                    case "baseline":
                        DataSourceCommonClass.CreateAQGFromBenMAPLine(saveBCG.Base, saveAQGPath); saveBCG.Base.ShapeFile = "";
                        break;
                    case "control":
                        DataSourceCommonClass.CreateAQGFromBenMAPLine(saveBCG.Control, saveAQGPath);
                        saveBCG.Control.ShapeFile = "";
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
            try
            {
                if (waitMess.InvokeRequired)
                { waitMess.Invoke(new CloseFormDelegate(DoCloseJob)); }
                else
                { DoCloseJob(); }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void DoCloseJob()
        {
            try
            {
                if (!waitMess.IsDisposed)
                {
                    sFlog = true;
                    waitMess.Close();
                }
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            string datasetName = string.Empty;
            if(_currentStat.ToLower().Equals("control"))
            {
                datasetName = "Control";//this is how it is listed in the db
            }
            if(_currentStat.ToLower().Equals("baseline"))
            {
                datasetName = "Baseline";//this is how it is listed in the db
            }
            DataTable modelDT = new DataTable();
            //modelDT = CommonClass.ExcelToDataTable(txtModelDatabase.Text);
            modelDT = CommonClass.ExcelToDataTable(txtModelDatabase.Text, ref _tabnameref, null);
            ValidateDatabaseImport vdi = new ValidateDatabaseImport(modelDT, datasetName, txtModelDatabase.Text);
            DialogResult dlgR = vdi.ShowDialog();
            if (dlgR.Equals(DialogResult.OK))
            {
                if (vdi.PassedValidation && _isForceValidate == "T")
                {
                    LoadDatabase(); 
                }
            }

        }

        private void btnViewMetadata_Click(object sender, EventArgs e)
        {
            ViewEditMetadata viewEMdata = null;
            if (_metadataObj != null)
            {
                viewEMdata = new ViewEditMetadata(_strPath, _metadataObj);
            }
            else
            {
                viewEMdata = new ViewEditMetadata(_strPath);
            }
            DialogResult dr = viewEMdata.ShowDialog();
            if (dr.Equals(DialogResult.OK))
            {
                _metadataObj = viewEMdata.MetadataObj;
            }
        }

        private void txtModelDatabase_TextChanged(object sender, EventArgs e)
        {
            btnValidate.Enabled = !string.IsNullOrEmpty(txtModelDatabase.Text);
        }
    }
}