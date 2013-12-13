using System;
using System.Data;
using System.Windows.Forms;

namespace BenMAP
{
    public partial class MonitorRollback : FormBase
    {
        #region Variables

        private BaseControlGroup _bgc = null;
        private string _currentStat = string.Empty;

        ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

        /// <summary>
        /// MonitorRollback的整套流程
        /// </summary>
        public MonitorModelRollbackLine _monitorRollbackLine;

        /// <summary>
        /// MonitorRollback的Grid
        /// </summary>
        BenMAPGrid _monitorRollbackGrid = new BenMAPGrid();

        private string _strPath;

        /// <summary>
        /// 选择的数据路径；
        /// </summary>
        public string StrPath
        {
            get { return _strPath; }
            set { _strPath = value; }
        }

        #endregion Variables

        public MonitorRollback(BaseControlGroup currentPollutant, string currentStat)
        {
            InitializeComponent();
            _bgc = currentPollutant;
            _currentStat = currentStat;
            _monitorRollbackLine = new MonitorModelRollbackLine();
        }

        private void MonitorRollback_Load(object sender, EventArgs e)
        {
            string commandText = string.Empty;
            DataSet ds;
            DataSet dsGrid;
            try
            {
                if (CommonClass.LstPollutant != null && CommonClass.LstPollutant.Count > 0)
                {
                    txtPollutant.Text = _bgc.Pollutant.PollutantName;
                    txtPollutant.Enabled = false;
                }
                commandText = string.Format("select MonitorDataSetID, MonitorDataSetName from MonitorDataSets where SetupID={0}  and MonitorDataSetID in (select distinct MonitorDataSetID from monitors where pollutantID={1}) order by MonitorDataSetName asc", CommonClass.MainSetup.SetupID, _bgc.Pollutant.PollutantID);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboMonitorDataSet.DataSource = ds.Tables[0];
                cboMonitorDataSet.DisplayMember = "MonitorDataSetName";
                cboMonitorDataSet.SelectedIndex = 0;
                //cboMonitorDataSet.Enabled = false;

                fb = new ESIL.DBUtility.ESILFireBirdHelper();
                commandText = string.Format("select * from GridDefinitions where SetupID={0} order by GridDefinitionName asc ", CommonClass.MainSetup.SetupID);
                dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboRollbackGridType.DataSource = dsGrid.Tables[0];
                cboRollbackGridType.DisplayMember = "GridDefinitionName";
                cboRollbackGridType.SelectedIndex = -1;
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

        ///// <summary>
        ///// 绑定GridType
        ///// </summary>
        //public void BindingGridType()
        //{
        //    fb = new ESIL.DBUtility.ESILFireBirdHelper();
        //    commandText = string.Format("select * from GridDefinitions where SetupID={0} order by GridDefinitionID asc ", CommonClass.ManageSetup.SetupID);
        //    dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
        //}

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
                txtMonitorDataFile.Text = openFileDialog.FileName;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnAdvanced_Click(object sender, EventArgs e)
        {
            //FilterMonitors frm = new FilterMonitors();
            //DialogResult rtn = frm.ShowDialog();
            //if (rtn != DialogResult.OK) { return; }
            if (cboRollbackGridType.SelectedIndex == -1)
            {
                MessageBox.Show("Please select rollback grid type first.");
                return;
            }
            if (tabControl1.SelectedIndex == 0)
            {
                if (cboMonitorDataSet.SelectedIndex == -1 || cboMonitorLibraryYear.SelectedIndex == -1) { MessageBox.Show("Please select monitor library year."); return; }
                _monitorRollbackLine.MonitorDirectType = 0;
                _monitorRollbackLine.MonitorDataSetID = Convert.ToInt32((cboMonitorDataSet.SelectedItem as DataRowView)["MonitorDataSetID"]);
                _monitorRollbackLine.MonitorLibraryYear = Convert.ToInt32((cboMonitorLibraryYear.SelectedItem as DataRowView)["Yyear"]);
                _strPath = "Monitor Rollback: libary";
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                if (txtMonitorDataFile.Text == "") { MessageBox.Show("Please select monitor data file."); return; }
                _monitorRollbackLine.MonitorDirectType = 1;
                _monitorRollbackLine.MonitorDataFilePath = txtMonitorDataFile.Text;
                _strPath = "Monitor Rollback: " + txtMonitorDataFile.Text;
            }
            _monitorRollbackLine.RollbackGrid = _monitorRollbackGrid;
            _monitorRollbackLine.GridType = CommonClass.GBenMAPGrid;
            _monitorRollbackLine.Pollutant = _bgc.Pollutant;
            MonitorAdvance MyMonitorAdvance = new MonitorAdvance()
            {
            };
            if (_monitorRollbackLine.MonitorAdvance != null)
            {
                MyMonitorAdvance = new MonitorAdvance()
                {
                    DataTypesToUse = _monitorRollbackLine.MonitorAdvance.DataTypesToUse,
                    EndDate = _monitorRollbackLine.MonitorAdvance.EndDate,
                    EndHour = _monitorRollbackLine.MonitorAdvance.EndHour,
                    FilterExcludeIDs = _monitorRollbackLine.MonitorAdvance.FilterExcludeIDs,
                    FilterIncludeIDs = _monitorRollbackLine.MonitorAdvance.FilterIncludeIDs,
                    FilterMaximumPOC = _monitorRollbackLine.MonitorAdvance.FilterMaximumPOC,
                    FilterMaxLatitude = _monitorRollbackLine.MonitorAdvance.FilterMaxLatitude,
                    FilterMaxLongitude = _monitorRollbackLine.MonitorAdvance.FilterMaxLongitude,
                    FilterMinLatitude = _monitorRollbackLine.MonitorAdvance.FilterMinLatitude,
                    FilterMinLongitude = _monitorRollbackLine.MonitorAdvance.FilterMinLongitude,
                    FilterStates = _monitorRollbackLine.MonitorAdvance.FilterStates,
                    GetClosedIfNoneWithinRadius = _monitorRollbackLine.MonitorAdvance.GetClosedIfNoneWithinRadius,
                    IncludeMethods = _monitorRollbackLine.MonitorAdvance.IncludeMethods,
                    MaxinumNeighborDistance = _monitorRollbackLine.MonitorAdvance.MaxinumNeighborDistance,
                    NumberOfPerQuarter = _monitorRollbackLine.MonitorAdvance.NumberOfPerQuarter,
                    NumberOfValidHour = _monitorRollbackLine.MonitorAdvance.NumberOfValidHour,
                    OutputType = _monitorRollbackLine.MonitorAdvance.OutputType,
                    PercentOfValidDays = _monitorRollbackLine.MonitorAdvance.PercentOfValidDays,
                    POCPreferenceOrder = _monitorRollbackLine.MonitorAdvance.POCPreferenceOrder,
                    PreferredType = _monitorRollbackLine.MonitorAdvance.PreferredType,
                    RelativeNeighborDistance = _monitorRollbackLine.MonitorAdvance.RelativeNeighborDistance,
                    StartDate = _monitorRollbackLine.MonitorAdvance.StartDate,
                    StartHour = _monitorRollbackLine.MonitorAdvance.StartHour,
                    WeightingApproach = _monitorRollbackLine.MonitorAdvance.WeightingApproach

                };
            }
            FilterMonitors frm = new FilterMonitors();
            frm.MonitorAdvanceFilter = MyMonitorAdvance;
            frm.bcg = this._bgc;
            frm.mDataLine = _monitorRollbackLine;
            DialogResult rtn = frm.ShowDialog();
            if (rtn == DialogResult.OK) 
            {
                MyMonitorAdvance = frm.MonitorAdvanceFilter;
                _monitorRollbackLine.MonitorAdvance = MyMonitorAdvance;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboRollbackGridType.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select rollback grid type first.");
                    return;
                }
                if (tabControl1.SelectedIndex == 0)
                {
                    if (cboMonitorDataSet.SelectedIndex == -1 || cboMonitorLibraryYear.SelectedIndex == -1) { MessageBox.Show("Please select monitor library year."); return; }
                    _monitorRollbackLine.MonitorDirectType = 0;
                    _monitorRollbackLine.MonitorDataSetID = Convert.ToInt32((cboMonitorDataSet.SelectedItem as DataRowView)["MonitorDataSetID"]);
                    _monitorRollbackLine.MonitorLibraryYear = Convert.ToInt32((cboMonitorLibraryYear.SelectedItem as DataRowView)["Yyear"]);
                    _strPath = "Monitor Rollback: libary";
                }
                else if (tabControl1.SelectedIndex == 1)
                {
                    if (txtMonitorDataFile.Text == "") { MessageBox.Show("Please select monitor data file."); return; }
                    _monitorRollbackLine.MonitorDirectType = 1;
                    _monitorRollbackLine.MonitorDataFilePath = txtMonitorDataFile.Text;
                    _strPath = "Monitor Rollback: " + txtMonitorDataFile.Text;
                }
                _monitorRollbackLine.RollbackGrid = _monitorRollbackGrid;
                _monitorRollbackLine.GridType = CommonClass.GBenMAPGrid;
                _monitorRollbackLine.Pollutant = _bgc.Pollutant;
                //_monitorRollbackLine.GridType = _monitorRollbackGrid;

                MonitorRollbackSettings2 frm = new MonitorRollbackSettings2(_currentStat, _monitorRollbackLine);
                frm.Bgc = _bgc;
                DialogResult rtn = frm.ShowDialog();
                if (rtn != DialogResult.OK) { return; }
                _monitorRollbackLine = frm._monitorRollbackLine;
                //-------------------需要update数据！！！！
                if (MonitorRollbackSettings3.MakeBaselineGrid == "F")
                {
                    switch (_currentStat)
                    {
                        case "baseline":
                            _bgc.Base = _monitorRollbackLine;//DataSourceCommonClass.LoadAQGFile(txtExistingAQG.Text);

                            break;
                        case "control":
                            _bgc.Control = _monitorRollbackLine;// DataSourceCommonClass.LoadAQGFile(txtExistingAQG.Text);
                            break;
                    }
                }
                else
                {
                    //string err = "";
                    //_bgc.Base = DataSourceCommonClass.LoadAQGFile(MonitorRollbackSettings3.MakeBaselineGrid.Substring(1, MonitorRollbackSettings3.MakeBaselineGrid.Length - 1), ref err);
                    _bgc.Control = _monitorRollbackLine;
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void cboMonitorDataSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            string commandText = string.Empty;
            try
            {
                cboMonitorLibraryYear.Text = null;
                if (cboMonitorDataSet == null || cboMonitorDataSet.SelectedItem == null) { return; }
                DataRowView drv = cboMonitorDataSet.SelectedItem as DataRowView;
                int MonitorDataSetID = Convert.ToInt32(drv["MonitorDataSetID"]);
                commandText = string.Format("select distinct Yyear from MonitorEntries a,Monitors b where a.MonitorID =b.MonitorID and MonitorDataSetID={0} and PollutantID={1}", MonitorDataSetID, _bgc.Pollutant.PollutantID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboMonitorLibraryYear.DataSource = ds.Tables[0];
                cboMonitorLibraryYear.DisplayMember = "Yyear";
                cboMonitorLibraryYear.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        /// <summary>
        /// 选择不同的RollbackGridType
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboRollbackGridType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboRollbackGridType.SelectedIndex == -1) return;
            DataRowView drv = (cboRollbackGridType.SelectedItem as DataRowView);
            _monitorRollbackGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drv["GridDefinitionID"]));
        }
    }
}