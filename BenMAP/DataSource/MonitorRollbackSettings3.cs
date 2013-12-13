using System;
using System.Windows.Forms;
using BenMAP.DataSource;
using System.Collections.Generic;
using System.Data;

namespace BenMAP
{
    public partial class MonitorRollbackSettings3 : FormBase
    {
        private InterpolationMethodEnum _var;

        private string _currentStat = string.Empty;

        public string CurrentStat
        {
            get { return _currentStat; }
            set { _currentStat = value; }
        }

        public MonitorModelRollbackLine _monitorRollbackLine;

        string saveAQGPath = string.Empty;

        private BaseControlGroup _bgc;

        public BaseControlGroup Bgc
        {
            get { return _bgc; }
            set { _bgc = value; }
        }

        static string makeBaselineGrid = "";

        public static string MakeBaselineGrid
        {
            get { return MonitorRollbackSettings3.makeBaselineGrid; }
            set { MonitorRollbackSettings3.makeBaselineGrid = value; }
        }

        /// <summary>
        /// 网格的差值方法
        /// </summary>
        public InterpolationMethodEnum Interplotion
        {
            get { return _var; }
            set
            {
                _var = value;
            }
        }// Interplotion

        private double _fixRadio = 0;

        /// <summary>
        ///
        /// </summary>
        public double FixRadio
        {
            get { return _fixRadio; }
            set
            {
                _fixRadio = value;
            }
        }// FixRadio

        public MonitorRollbackSettings3()
        {
            InitializeComponent();
        }

        private void MonitorRollbackSettings3_Load(object sender, EventArgs e)
        {
            try
            {
                rbtnNone.Checked = true;
                cboGridType.Text = CommonClass.GBenMAPGrid.GridDefinitionName;
                cboGridType.Enabled = false;
                rbtnVoronoiNeighborhood.Checked = true;
                //txtAdjustment.Enabled = false;
                //btnBrowse.Enabled = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void rbtnSpatialOnly_Click(object sender, EventArgs e)
        {
            txtAdjustment.Enabled = true;
            btnBrowse.Enabled = true;
        }

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            try
            {
                string method = string.Empty;
                foreach (var c in grpInterpolation.Controls)
                {
                    RadioButton r = c as RadioButton;
                    if (r == null) { continue; }
                    if (r.Checked) { method = r.Tag.ToString(); break; }
                }
                if (_monitorRollbackLine.MonitorAdvance == null)
                {
                    _monitorRollbackLine.MonitorAdvance = new MonitorAdvance();
                }
                AdvancedOptions frm = new AdvancedOptions(method, _monitorRollbackLine.MonitorAdvance);
                frm.bcg = _bgc;
                frm.mDataLine = _monitorRollbackLine;
                DialogResult rtn = frm.ShowDialog();
                if (rtn == DialogResult.OK) { _monitorRollbackLine.MonitorAdvance = frm.MyMonitorAdvance; }

                //MonitorAdvance MyMonitorAdvance = new MonitorAdvance()
                //{
                //};
                //if (_monitorRollbackLine.MonitorAdvance != null)
                //{
                //    MyMonitorAdvance = new MonitorAdvance()
                //    {
                //        DataTypesToUse = _monitorRollbackLine.MonitorAdvance.DataTypesToUse,
                //        EndDate = _monitorRollbackLine.MonitorAdvance.EndDate,
                //        EndHour = _monitorRollbackLine.MonitorAdvance.EndHour,
                //        FilterExcludeIDs = _monitorRollbackLine.MonitorAdvance.FilterExcludeIDs,
                //        FilterIncludeIDs = _monitorRollbackLine.MonitorAdvance.FilterIncludeIDs,
                //        FilterMaximumPOC = _monitorRollbackLine.MonitorAdvance.FilterMaximumPOC,
                //        FilterMaxLatitude = _monitorRollbackLine.MonitorAdvance.FilterMaxLatitude,
                //        FilterMaxLongitude = _monitorRollbackLine.MonitorAdvance.FilterMaxLongitude,
                //        FilterMinLatitude = _monitorRollbackLine.MonitorAdvance.FilterMinLatitude,
                //        FilterMinLongitude = _monitorRollbackLine.MonitorAdvance.FilterMinLongitude,
                //        FilterStates = _monitorRollbackLine.MonitorAdvance.FilterStates,
                //        GetClosedIfNoneWithinRadius = _monitorRollbackLine.MonitorAdvance.GetClosedIfNoneWithinRadius,
                //        IncludeMethods = _monitorRollbackLine.MonitorAdvance.IncludeMethods,
                //        MaxinumNeighborDistance = _monitorRollbackLine.MonitorAdvance.MaxinumNeighborDistance,
                //        NumberOfPerQuarter = _monitorRollbackLine.MonitorAdvance.NumberOfPerQuarter,
                //        NumberOfValidHour = _monitorRollbackLine.MonitorAdvance.NumberOfValidHour,
                //        OutputType = _monitorRollbackLine.MonitorAdvance.OutputType,
                //        PercentOfValidDays = _monitorRollbackLine.MonitorAdvance.PercentOfValidDays,
                //        POCPreferenceOrder = _monitorRollbackLine.MonitorAdvance.POCPreferenceOrder,
                //        PreferredType = _monitorRollbackLine.MonitorAdvance.PreferredType,
                //        RelativeNeighborDistance = _monitorRollbackLine.MonitorAdvance.RelativeNeighborDistance,
                //        StartDate = _monitorRollbackLine.MonitorAdvance.StartDate,
                //        StartHour = _monitorRollbackLine.MonitorAdvance.StartHour,
                //        WeightingApproach = _monitorRollbackLine.MonitorAdvance.WeightingApproach

                //    };
                //}
                //FilterMonitors frm = new FilterMonitors();
                //frm.MonitorAdvanceFilter = MyMonitorAdvance;
                //frm.bcg = this._bgc;
                //frm.mDataLine = _monitorRollbackLine;
                //DialogResult rtn = frm.ShowDialog();
                //if (rtn == DialogResult.OK)
                //{
                //    MyMonitorAdvance = frm.MonitorAdvanceFilter;
                //    _monitorRollbackLine.MonitorAdvance = MyMonitorAdvance;
                //}

                //string str = string.Empty;
                //foreach (var c in grpInterpolation.Controls)
                //{
                //    RadioButton r = c as RadioButton;
                //    if (r == null) { continue; }
                //    if (r.Checked) { str = r.Text; break; }
                //}
                //AdvancedOptions frm = new AdvancedOptions(str,);
                //frm.ShowDialog();
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
                openFileDialog.InitialDirectory = Application.StartupPath + @"E:\";
                openFileDialog.Filter = "All Files|*.*|CSV files|*.csv";
                openFileDialog.FilterIndex = 3;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
                txtAdjustment.Text = openFileDialog.FileName;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                bool ok = false;
                double value = 0.0;
                if (rbtnClosestMonitor.Checked) { _monitorRollbackLine.InterpolationMethod = InterpolationMethodEnum.ClosestMonitor; }
                else if (rbtnVoronoiNeighborhood.Checked) { _monitorRollbackLine.InterpolationMethod = InterpolationMethodEnum.VoronoiNeighborhoodAveragin; }
                else if (rbtnFixedRadius.Checked)
                {
                    _monitorRollbackLine.InterpolationMethod = InterpolationMethodEnum.FixedRadius;
                    ok = double.TryParse(txtFixRadio.Text, out value);
                    if (!ok || Convert.ToDouble(txtFixRadio.Text) <= 0)
                    {
                        MessageBox.Show("To select fixed radius interpolation you must provide a radius in kilometers.");
                        txtFixRadio.Text = string.Empty;
                        return;
                    }
                    _monitorRollbackLine.FixedRadius = value;
                }

                //_monitorRollbackLine.InterpolationMethod = Interplotion;
                //_monitorRollbackLine.FixedRadius = FixRadio;
                MonitorDataLine baselinegrid = new MonitorDataLine();
                SaveFileDialog sfd = new SaveFileDialog();
                if (chbMakeBaselineGrid.Checked)
                {
                    baselinegrid = new MonitorDataLine()
                    {
                        CreateTime = _monitorRollbackLine.CreateTime,
                        GridType = _monitorRollbackLine.GridType,
                        FixedRadius = _monitorRollbackLine.FixedRadius,
                        InterpolationMethod = _monitorRollbackLine.InterpolationMethod,
                        MonitorAdvance = _monitorRollbackLine.MonitorAdvance,
                        MonitorDataFilePath = _monitorRollbackLine.MonitorDataFilePath,
                        MonitorDataSetID = _monitorRollbackLine.MonitorDataSetID,
                        MonitorDefinitionFile = _monitorRollbackLine.MonitorDefinitionFile,
                        MonitorDirectType = _monitorRollbackLine.MonitorDirectType,
                        MonitorLibraryYear = _monitorRollbackLine.MonitorLibraryYear,
                        MonitorNeighbors = _monitorRollbackLine.MonitorNeighbors,
                        Pollutant = _monitorRollbackLine.Pollutant,
                        ShapeFile = _monitorRollbackLine.ShapeFile
                    };
                    sfd.Title = "Save the baseline Grid.";
                    sfd.Filter = "AGQ files (*.aqgx)|*.aqgx";
                    sfd.RestoreDirectory = true;
                    sfd.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG";
                    if (sfd.ShowDialog() != DialogResult.OK)
                    { return; }
                }
                SaveFileDialog sfd2 = new SaveFileDialog();
                sfd2.Title = "Save the rolled back Grid.";
                sfd2.Filter = "AGQ files (*.aqgx)|*.aqgx";
                sfd2.RestoreDirectory = true;
                sfd2.InitialDirectory = CommonClass.ResultFilePath + @"\Result\AQG";
                if (sfd2.ShowDialog() != DialogResult.OK)
                { return; }
                //save the baseline grid
                if (chbMakeBaselineGrid.Checked)
                {
                    WaitShow("Saving the baseline grid...");
                    DataSourceCommonClass.UpdateModelValuesMonitorData(baselinegrid.GridType, baselinegrid.Pollutant, ref baselinegrid);
                    DataSourceCommonClass.CreateAQGFromBenMAPLine(baselinegrid, sfd.FileName);
                    WaitClose();
                    makeBaselineGrid = "T" + sfd.FileName;
                }
                else
                    makeBaselineGrid = "F";
                //save the rolled back grid
                saveAQGPath = sfd2.FileName;
                int threadId = -1;
                AsynDelegateRollBack asyncD = new AsynDelegateRollBack(AsyncUpdateMonitorRollbackData);
                IAsyncResult ar = asyncD.BeginInvoke(_currentStat, _monitorRollbackLine, out threadId, null, null); 
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                WaitClose();
                Logger.LogError(ex);
            }
        }

        #region 等待窗口
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
        #endregion 等待窗口

        private string AsyncUpdateMonitorRollbackData(string currentStat, MonitorModelRollbackLine monitorRollbackLine, out int threadId)
        {
            threadId = -1;
            string str = string.Empty;
            try
            {
                _currentStat = currentStat;
                if (makeBaselineGrid.Length > 1 && makeBaselineGrid.Substring(0, 1) == "T")
                {
                    _currentStat = "control";
                }

                if (CommonClass.LstAsynchronizationStates == null) { CommonClass.LstAsynchronizationStates = new List<string>(); }
                lock (CommonClass.LstAsynchronizationStates)
                {
                    str = string.Format("{0}{1}", monitorRollbackLine.Pollutant.PollutantName.ToLower(), _currentStat);
                    CommonClass.LstAsynchronizationStates.Add(str);
                    if (_currentStat != "")
                    {
                        CommonClass.CurrentMainFormStat = _currentStat.Substring(0, 1).ToUpper() + _currentStat.Substring(1) + " is being created.";
                        //CommonClass.NodeAnscyStatus = string.Format("{0};on", _currentStat);
                    }
                }// lock

                lock (CommonClass.NodeAnscyStatus)
                { CommonClass.NodeAnscyStatus = string.Format("{0};{1};on", monitorRollbackLine.Pollutant.PollutantName.ToLower(), _currentStat); }
                switch (_currentStat)
                {
                    case "baseline":
                        RollBackDalgorithm.UpdateMonitorDataRollBack(ref _monitorRollbackLine);
                        lock (CommonClass.LstBaseControlGroup)
                        {
                            foreach (BaseControlGroup bc in CommonClass.LstBaseControlGroup)
                            {
                                if (bc.Pollutant.PollutantID == _monitorRollbackLine.Pollutant.PollutantID)
                                {
                                    _monitorRollbackLine.ShapeFile = _monitorRollbackLine.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "B" + _currentStat + ".shp";
                                    string shipFile = string.Format("{0}\\Tmp\\{1}", CommonClass.DataFilePath, _monitorRollbackLine.ShapeFile);
                                    bc.Base = _monitorRollbackLine;
                                    DataSourceCommonClass.SaveBenMAPLineShapeFile(_monitorRollbackLine.GridType, _monitorRollbackLine.Pollutant, _monitorRollbackLine, shipFile);
                                    //---------------majie save aqg file-----------------
                                    DataSourceCommonClass.CreateAQGFromBenMAPLine(bc.Base, saveAQGPath);//DataSourceCommonClass.LoadAQGFile(txtExistingAQG.Text);
                                    bc.Base.ShapeFile = "";
                                }
                            }
                        }
                        break;
                    case "control":
                        RollBackDalgorithm.UpdateMonitorDataRollBack(ref _monitorRollbackLine);
                        lock (CommonClass.LstBaseControlGroup)
                        {
                            foreach (BaseControlGroup bc in CommonClass.LstBaseControlGroup)
                            {
                                if (bc.Pollutant.PollutantID == _monitorRollbackLine.Pollutant.PollutantID)
                                {
                                    _monitorRollbackLine.ShapeFile = _monitorRollbackLine.Pollutant.PollutantID + "G" + CommonClass.GBenMAPGrid.GridDefinitionID + "C" + _currentStat + ".shp";
                                    string shipFile = string.Format("{0}\\Tmp\\{1}", CommonClass.DataFilePath, _monitorRollbackLine.ShapeFile);
                                    bc.Control = _monitorRollbackLine;
                                    DataSourceCommonClass.SaveBenMAPLineShapeFile(_monitorRollbackLine.GridType, _monitorRollbackLine.Pollutant, _monitorRollbackLine, shipFile);
                                    //--------------------majie save aqg file------------------
                                    DataSourceCommonClass.CreateAQGFromBenMAPLine(bc.Control, saveAQGPath);//DataSourceCommonClass.LoadAQGFile(txtExistingAQG.Text);
                                    bc.Control.ShapeFile = "";
                                }
                            }
                        }
                        break;
                }
                lock (CommonClass.LstAsynchronizationStates)
                {
                    CommonClass.LstAsynchronizationStates.Remove(str);
                    if (CommonClass.LstAsynchronizationStates.Count == 0)
                    {
                        CommonClass.CurrentMainFormStat = "Current Setup: " + CommonClass.MainSetup.SetupName;
                        //CommonClass.NodeAnscyStatus = string.Format("{0};off", _currentStat);
                    }
                }
                lock (CommonClass.NodeAnscyStatus)
                { CommonClass.NodeAnscyStatus = string.Format("{0};{1};off", monitorRollbackLine.Pollutant.PollutantName.ToLower(), _currentStat); }
                return str;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return string.Empty;
            }
        }

        private void chbMakeBaselineGrid_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (sender == null) { return; }
            //    var chk = sender as CheckBox;
            //    if (chk.Checked) { txtAdjustment.Enabled = false; btnBrowse.Enabled = false; }
            //    else
            //    { txtAdjustment.Enabled = false; btnBrowse.Enabled = false; }
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogError(ex);
            //}
        }

        private void txtFixRadio_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (sender == null) { return; }
            //    float radio = 0;
            //    bool ok = false;
            //    if (string.IsNullOrEmpty(txtFixRadio.Text.Trim()))
            //    {
            //        ok = float.TryParse(txtFixRadio.Text, out radio);
            //    }
            //    if (!ok) { MessageBox.Show("Please enter numbers!\t", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogError(ex);
            //}
        }

        private void rbtnClosestMonitor_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender == null) { return; }
                RadioButton rbtn = sender as RadioButton;
                string tag = rbtn.Tag.ToString();
                switch (tag)
                {
                    case "closest":
                        _var = InterpolationMethodEnum.ClosestMonitor;
                        break;
                    case "voronoi":
                        _var = InterpolationMethodEnum.VoronoiNeighborhoodAveragin;
                        break;
                    case "fixed":
                        _var = InterpolationMethodEnum.FixedRadius;
                        if (string.IsNullOrEmpty(txtFixRadio.Text.Trim())) { MessageBox.Show("Fixed radius", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                        _fixRadio = float.Parse(txtFixRadio.Text);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void rbtnNone_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender == null) { return; }
                RadioButton rbtn = sender as RadioButton;
                string tag = rbtn.Tag.ToString().ToLower();
                switch (tag)
                {
                    case "none":
                        txtAdjustment.Enabled = false;
                        btnBrowse.Enabled = false;
                        break;
                    case "spatial":
                        txtAdjustment.Enabled = true;
                        btnBrowse.Enabled = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void txtFixRadio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == (char)8)
            {
                e.Handled = false;
            }
            else
            {
                if (e.KeyChar == '.')
                {
                    if (((TextBox)sender).Text.Trim().IndexOf('.') > -1)
                        e.Handled = true;
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void rbtnFixedRadius_CheckedChanged(object sender, EventArgs e)
        {
            txtFixRadio.Focus();
        }

        private void txtFixRadio_Click(object sender, EventArgs e)
        {
            rbtnFixedRadius.Checked = true;
        }
    }
}