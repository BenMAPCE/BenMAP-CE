using System;
using System.IO;
using System.Windows.Forms;
using DotSpatial.Data;
using System.Data;
using ESIL.DBUtility;

namespace BenMAP
{
    public partial class GridCreationMethods : FormBase
    {
        public GridCreationMethods()
        {
            InitializeComponent();
        }

        private BaseControlGroup _bgc = null;

        public BaseControlGroup BGC
        {
            get { return _bgc; }
            set { _bgc = value; }
        }

        private MonitorDataLine _mDataLine = new MonitorDataLine();

        public MonitorDataLine MDataLine
        {
            get { return _mDataLine; }
            set { _mDataLine = value; }
        }

        private string _currentStat = string.Empty;

        public GridCreationMethods(BaseControlGroup bgc, string currentStat)
        {
            InitializeComponent();
            _bgc = bgc;
            _currentStat = currentStat;
            switch (currentStat)
            {
                case "baseline":
                    if (bgc.Base != null && bgc.Base.ModelResultAttributes != null && bgc.Base.ModelResultAttributes.Count > 0)
                    {
                        this.btnSave.Enabled = true;
                        this.btnSaveNewFormat.Enabled = true;
                    }
                    break;
                case "control":
                    if (bgc.Control != null && bgc.Control.ModelResultAttributes != null && bgc.Control.ModelResultAttributes.Count > 0)
                    {
                        this.btnSave.Enabled = true;
                        this.btnSaveNewFormat.Enabled = true;
                    }
                    break;
            }
        }

        private string _pageStat = "model";

        public string PageStat
        {
            get { return _pageStat; }
            set { _pageStat = value; }
        }

        private string _strPath;

        public string StrPath
        {
            get { return _strPath; }
            set { _strPath = value; }
        }

        private void GridCreationMethods_Load(object sender, EventArgs e)
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESILFireBirdHelper();
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
            { }
        }
        public bool isGridTypeChanged = false;
        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                DataRowView drv = cboGrid.SelectedItem as DataRowView;
                BenMAPGrid benMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drv["GridDefinitionID"]));

                if (CommonClass.GBenMAPGrid == null) CommonClass.GBenMAPGrid = benMAPGrid;
                else if (CommonClass.GBenMAPGrid.GridDefinitionID != benMAPGrid.GridDefinitionID)
                {
                    CommonClass.GBenMAPGrid = benMAPGrid;
                    isGridTypeChanged = true;
                }
                _bgc.GridType = CommonClass.GBenMAPGrid;
                string tip = "Reading and checking the data file.";
                if (rbtnModelData.Checked)
                {
                    ModelData frm = new ModelData(_bgc, _currentStat);
                    DialogResult rtn = frm.ShowDialog();
                    if (rtn == DialogResult.Cancel) { return; }
                    _strPath = frm.StrPath;
                    this.DialogResult = rtn;
                }
                else if (rbtnMonitorData.Checked)
                {
                    MonitorData frm = new MonitorData(_bgc, _currentStat);
                    DialogResult rtn = frm.ShowDialog();
                    if (rtn == DialogResult.Cancel) { return; }
                    _mDataLine = frm.MDataLine;
                    _strPath = frm.StrPath;
                    this.DialogResult = rtn;

                }
                else if (rbtnMonitorRollback.Checked)
                {
                    MonitorRollback frm = new MonitorRollback(_bgc, _currentStat);
                    DialogResult rtn = frm.ShowDialog();
                    if (rtn == DialogResult.Cancel) { return; }
                    _mDataLine = frm._monitorRollbackLine;
                    _strPath = frm.StrPath;
                    this.DialogResult = rtn;
                }
                else if (this.rbtnOpenFile.Checked)
                {
                    if (txtExistingAQG.Text != "")
                    {
                        ParaserAQG(txtExistingAQG.Text);

                    } return;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        public void ParaserAQG(string strPath)
        {
            try
            {
                string tip = "Reading and checking the data file.";
                _strPath = strPath;
                WaitShow(tip);
                string err = "";
                BenMAPLine benMapLine = DataSourceCommonClass.LoadAQGFile(txtExistingAQG.Text, ref err);
                System.Threading.Thread.Sleep(100);
                if (benMapLine == null)
                {
                    WaitClose();
                    MessageBox.Show(err);
                    return;
                }
                WaitClose();
                if (benMapLine.Pollutant.PollutantID != _bgc.Pollutant.PollutantID)
                {
                    MessageBox.Show("The AQG's pollutant does not match the selected pollutant. Please select another file.");
                    return;
                }
                else if (benMapLine.GridType.GridDefinitionID != _bgc.GridType.GridDefinitionID)
                {
                    MessageBox.Show("The AQG's grid definition does not match the selected grid definition. Please select another file.");
                    return;
                }
                //YY: why saving aqgx again in temp folder?
                if (benMapLine.ShapeFile != null && !benMapLine.ShapeFile.Contains(@"\"))
                {
                    string AppPath = Application.StartupPath;
                    string _filePath = txtExistingAQG.Text.Substring(0, txtExistingAQG.Text.LastIndexOf(@"\") + 1);
                    string strShapePath = string.Format("{0}\\Result\\Tmp\\{1}", CommonClass.DataFilePath, benMapLine.ShapeFile);
                    benMapLine.ShapeFile = _filePath + benMapLine.ShapeFile;
                    DataSourceCommonClass.SaveBenMAPLineShapeFile(_bgc.GridType, _bgc.Pollutant, benMapLine, strShapePath);
                }
                else if (benMapLine.ShapeFile != null)
                {
                    DataSourceCommonClass.SaveBenMAPLineShapeFile(_bgc.GridType, _bgc.Pollutant, benMapLine, benMapLine.ShapeFile);
                }
                switch (_currentStat)
                {
                    case "baseline":
                        _bgc.Base = benMapLine;
#if DEBUG
                        //YY: If in debug mode, export weight table. 
                        if (benMapLine is MonitorDataLine)
                        {
                            MonitorDataLine mdl = (MonitorDataLine)benMapLine;
                            string filePath = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + @"\My BenMAP-CE Files\debug_weightBase" + string.Format("{0:yyyyMMddhhmmss}", DateTime.Now) + ".csv";
                            System.IO.StreamWriter baseWriter = new System.IO.StreamWriter(filePath, true);
                            string baseMsg = "Col,Row,Distance,MonitorName,Weight";
                            baseWriter.WriteLine(baseMsg);
                            foreach (MonitorNeighborAttribute mna in mdl.MonitorNeighbors)
                            {
                                baseMsg = mna.Col + "," + mna.Row + "," + mna.Distance + "," + mna.MonitorName + "," + mna.Weight;
                                baseWriter.WriteLine(baseMsg);
                            }
                            baseWriter.Close();
                        }
                        
#endif
                        break;
                    case "control":
                        _bgc.Control = benMapLine; break;
                } this.DialogResult = System.Windows.Forms.DialogResult.OK;
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

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender == null) { return; }
                RadioButton rbn = sender as RadioButton;
                if (rbn.Checked) { _pageStat = rbn.Tag.ToString(); }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG";
                openFileDialog.Filter = "Air Quality Surface(*.aqgx)|*.aqgx";
                openFileDialog.FilterIndex = 3;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
                this.txtExistingAQG.Text = openFileDialog.FileName;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void rbtnOpenFile_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnOpenFile.Checked)
            {
                btnBrowse.Enabled = true;
            }
            else
            {
                btnBrowse.Enabled = false;
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "aqg files (*.aqgx)|*.aqgx";
            sfd.FilterIndex = 2;
            sfd.RestoreDirectory = true;
            sfd.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG";
            string _fileName = "";
            string _filePath = "";
            string shpFile = "";
            _filePath = sfd.FileName.Substring(0, sfd.FileName.LastIndexOf(@"\") + 1);
            FeatureSet fs = new FeatureSet();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                switch (_currentStat)
                {
                    case "baseline":
                        _filePath = sfd.FileName.Substring(0, sfd.FileName.LastIndexOf(@"\") + 1);
                        _fileName = sfd.FileName.Substring(sfd.FileName.LastIndexOf(@"\") + 1).Replace("aqgx", "shp");



                        DataSourceCommonClass.CreateAQGFromBenMAPLine(_bgc.Base, sfd.FileName); _bgc.Base.ShapeFile = shpFile;
                        break;
                    case "control":

                        string _filePathControl = sfd.FileName.Substring(0, sfd.FileName.LastIndexOf(@"\") + 1);
                        _fileName = sfd.FileName.Substring(sfd.FileName.LastIndexOf(@"\") + 1).Replace("aqgx", "shp");

                        DataSourceCommonClass.CreateAQGFromBenMAPLine(_bgc.Control, sfd.FileName); _bgc.Control.ShapeFile = shpFile;
                        break;
                }
                MessageBox.Show("AQG saved.", "File saved");
            }
        }

        private void SaveNewFormat_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "csv files (*.csv)|*.csv";
            sfd.FilterIndex = 2;
            sfd.RestoreDirectory = true;
            string _fileName = "";
            string _filePath = "";
            string shpFile = "";
            _filePath = sfd.FileName.Substring(0, sfd.FileName.LastIndexOf(@"\") + 1);
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                switch (_currentStat)
                {
                    case "baseline":
                        _filePath = sfd.FileName.Substring(0, sfd.FileName.LastIndexOf(@"\") + 1);
                        _fileName = sfd.FileName.Substring(sfd.FileName.LastIndexOf(@"\") + 1).Replace("aqg", "shp");

                        DataSourceCommonClass.SaveModelDataLineToNewFormatCSV(_bgc.Base, sfd.FileName); break;
                    case "control":

                        string _filePathControl = sfd.FileName.Substring(0, sfd.FileName.LastIndexOf(@"\") + 1);
                        DataSourceCommonClass.SaveModelDataLineToNewFormatCSV(_bgc.Control, sfd.FileName);
                        break;
                }
            }
        }

        private void picGTHelp_Click(object sender, EventArgs e)
        {
            this.toolTip1.Show(
                "Grid Type determines the spatial scale of the air quality grids " +
                "\r\nfor your analysis. If you are using model data, Grid Type should " +
                "\r\nmatch the column and row index in your model files. If you are " +
                "\r\nusing monitor data (including a rollback), Grid Type is the " +
                "\r\nspatial scale at which the final air quality layer will be reported.",
                picGTHelp, 32700);
        }

        private void picGTHelp_MouseHover(object sender, EventArgs e)
        {
            this.toolTip1.Show(
                "Grid Type determines the spatial scale of the air quality grids " +
                "\r\nfor your analysis. If you are using model data, Grid Type should " +
                "\r\nmatch the column and row index in your model files. If you are " +
                "\r\nusing monitor data (including a rollback), Grid Type is the " +
                "\r\nspatial scale at which the final air quality layer will be reported.",
                picGTHelp, 32700);
        }
    }
}