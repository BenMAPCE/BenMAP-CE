using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Collections;

namespace BenMAP
{
    public partial class ManageSetup : FormBase
    {
        ////设定程序中存储自适应窗口变化初始相关私有变量
        //private ArrayList InitialCrl = new ArrayList();//用以存储窗体中所有的控件名称     
        //private ArrayList CrlLocationX = new ArrayList();//用以存储窗体中所有的控件原始位置
        //private ArrayList CrlLocationY = new ArrayList();//用以存储窗体中所有的控件原始位置
        //private ArrayList CrlSizeWidth = new ArrayList();//用以存储窗体中所有的控件原始的水平尺寸
        //private ArrayList CrlSizeHeight = new ArrayList();//用以存储窗体中所有的控件原始的垂直尺寸        
        //private int FormSizeWidth;//用以存储窗体原始的水平尺寸        
        //private int FormSizeHeight;//用以存储窗体原始的垂直尺寸       
        //private double FormSizeChangedX;//用以存储相关父窗体/容器的水平变化量 
        //private double FormSizeChangedY;//用以存储相关父窗体/容器的垂直变化量  
        //private int Wcounter = 0;//为防止递归遍历控件时产生混乱，故专门设定一个全局计数器
        ////end
        string _dataName = string.Empty;
        public ManageSetup()
        {
            InitializeComponent();
        }
        private void btnGridDef_Click(object sender, EventArgs e)
        {
            ManageGridDefinetions frm = new ManageGridDefinetions();
            DialogResult rtn = frm.ShowDialog();
            //if (rtn == DialogResult.OK)
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select GridDefinitionName from GridDefinitions where setupID={0}", CommonClass.ManageSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstGridDefinitions.DataSource = ds.Tables[0];
                lstGridDefinitions.DisplayMember = "GridDefinitionName";
            }
        }

        private void btnPollutants_Click(object sender, EventArgs e)
        {
            ManagePollutants frm = new ManagePollutants();
            DialogResult rtn = frm.ShowDialog();
            //if (rtn == DialogResult.OK)
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select PollutantName from Pollutants where setupID={0}", CommonClass.ManageSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstPollutans.DataSource = ds.Tables[0];
                lstPollutans.DisplayMember = "PollutantName";
            }

        }

        private void btnMonitor_Click(object sender, EventArgs e)
        {
            ManageMonitorDataSets frm = new ManageMonitorDataSets();
            DialogResult rtn = frm.ShowDialog();
            //if (rtn == DialogResult.OK)
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select MonitorDataSetName from MonitorDataSets where setupID={0}", CommonClass.ManageSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstMonitor.DataSource = ds.Tables[0];
                lstMonitor.DisplayMember = "MonitorDataSetName";
            }
        }

        private void btnIncidence_Click(object sender, EventArgs e)
        {
            ManageIncidenceDataSets frm = new ManageIncidenceDataSets();
            DialogResult rtn = frm.ShowDialog();
            //if (rtn == DialogResult.OK)
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select IncidenceDataSetName from IncidenceDataSets where setupID={0}", CommonClass.ManageSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstIncidenceOrPrevalence.DataSource = ds.Tables[0];
                lstIncidenceOrPrevalence.DisplayMember = "IncidenceDataSetName";
            }
        }

        private void btnHealthImpact_Click(object sender, EventArgs e)
        {
            ManageHealthImpactFunctionDataSets frm = new ManageHealthImpactFunctionDataSets();
            DialogResult rtn = frm.ShowDialog();
            //if (rtn == DialogResult.OK)
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select CRfunctionDataSetName from CRfunctionDataSets where setupID={0}", CommonClass.ManageSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstHealthImpact.DataSource = ds.Tables[0];
                lstHealthImpact.DisplayMember = "CRfunctionDataSetName";
            }
        }

        private void btnVariable_Click(object sender, EventArgs e)
        {
            ManageVariableDataSets frm = new ManageVariableDataSets();
            DialogResult rtn = frm.ShowDialog();
            //if (rtn == DialogResult.OK)
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select  SETUPVARIABLEDATASETNAME from SETUPVARIABLEDATASETS where setupID={0}", CommonClass.ManageSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstVariable.DataSource = ds.Tables[0];
                lstVariable.DisplayMember = "SETUPVARIABLEDATASETNAME";
            }
        }

        private void btnInflation_Click(object sender, EventArgs e)
        {
            ManageInflationDataSet frm = new ManageInflationDataSet();
            DialogResult rtn = frm.ShowDialog();
            //if (rtn == DialogResult.OK)
            {
                string commandText = string.Format("select  INFLATIONDATASETNAME from INFLATIONDATASETS where setupID={0}", CommonClass.ManageSetup.SetupID);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstInflation.DataSource = ds.Tables[0];
                lstInflation.DisplayMember = "INFLATIONDATASETNAME";
            }
        }

        private void btnValuation_Click(object sender, EventArgs e)
        {
            try
            {
                ManageValuationFunctionDataSets frm = new ManageValuationFunctionDataSets();
                DialogResult rtn = frm.ShowDialog();
                //if (rtn == DialogResult.OK)
                {
                    string commandText = string.Format("select  VALUATIONFUNCTIONDATASETNAME from VALUATIONFUNCTIONDATASETS where setupID={0} ", CommonClass.ManageSetup.SetupID);
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    //bind table to lstDatasetName and display QALYDATASETNAME field
                    lstValuation.DataSource = ds.Tables[0];
                    lstValuation.DisplayMember = "VALUATIONFUNCTIONDATASETNAME";
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnIncome_Click(object sender, EventArgs e)
        {
            try
            {
                frmIncomeGrowthAdjustmentDataSet frm = new frmIncomeGrowthAdjustmentDataSet();
                DialogResult rtn = frm.ShowDialog();
                //if (rtn == DialogResult.OK)
                {
                    string commandText = string.Format("select  INCOMEGROWTHADJDATASETNAME from INCOMEGROWTHADJDATASETS where setupID={0}", CommonClass.ManageSetup.SetupID);
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    //bind table to lstDatasetName and display QALYDATASETNAME field
                    lstIncome.DataSource = ds.Tables[0];
                    lstIncome.DisplayMember = "INCOMEGROWTHADJDATASETNAME";
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        //private void btnQALY_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        frmQALYDataSetManager frm = new frmQALYDataSetManager();
        //        DialogResult rtn = frm.ShowDialog();
        //        if (rtn == DialogResult.OK)
        //        {
        //            string commandText = string.Format("select  QALYDATASETNAME from QALYDATASETS where setupID={0}", CommonClass.ManageSetup.SetupID);
        //            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
        //            DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
        //            //bind table to lstDatasetName and display QALYDATASETNAME field
        //            //DatasetName1 = lstDatasetName.Text;
        //            lstQALY.DataSource = ds.Tables[0];
        //            lstQALY.DisplayMember = "QALYDATASETNAME";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex);
        //    }
        //}

        private void btnPopulation_Click(object sender, EventArgs e)
        {
            ManagePopulationDataSets frm = new ManagePopulationDataSets();
            DialogResult rtn = frm.ShowDialog();
            //if (rtn == DialogResult.OK)
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select PopulationDataSetname from PopulationDataSets where setupID={0} and populationdatasetid<>37", CommonClass.ManageSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);

                lstPopulation.DataSource = ds.Tables[0];
                lstPopulation.DisplayMember = "PopulationDataSetname";
            }
        }

        private void frmManageSetup_Load(object sender, EventArgs e)
        {
            ////自适应
            //GetInitialFormSize();
            //GetAllCrlLocation(this);
            //GetAllCrlSize(this);
            ////end 
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            string commandText = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                commandText = string.Format("select SetupID,SetupName from Setups order by SetupID");
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboSetupName.DataSource = ds.Tables[0];
                cboSetupName.DisplayMember = "SetupName";
                //cboSetupName.SelectedIndex = 0;
                //DataRow dr = ds.Tables[0].Rows[0];
                //CommonClass.ManageSetup = new BenMAPSetup()
                //{
                //    SetupID = Convert.ToInt32(dr["SetupID"]),
                //    SetupName = dr["SetupName"].ToString()
                //};
                //-------modified by majie------
                cboSetupName.Text = CommonClass.MainSetup.SetupName;
                CommonClass.ManageSetup = new BenMAPSetup()
                {
                    SetupID = CommonClass.MainSetup.SetupID,
                    SetupName = CommonClass.MainSetup.SetupName
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //绑定选择区域的各类值
        private void LoadItems()
        {
            int setupID = CommonClass.ManageSetup.SetupID;
            string commandText = string.Empty;
            string bindFieldName = string.Empty;
            commandText = string.Format("select  GRIDDEFINITIONNAME from GRIDDEFINITIONS where SetupID={0} order  by GRIDDEFINITIONNAME asc", setupID);
            bindFieldName = "GRIDDEFINITIONNAME";
            BindingListBox(commandText, lstGridDefinitions, bindFieldName);

            commandText = string.Format("select  POLLUTANTNAME from POLLUTANTS where SetupID={0} order  by POLLUTANTNAME asc", setupID);
            bindFieldName = "POLLUTANTNAME";
            BindingListBox(commandText, lstPollutans, bindFieldName);

            commandText = string.Format("select  MONITORDATASETNAME from MONITORDATASETS where SetupID={0} order  by MONITORDATASETNAME asc", setupID);
            bindFieldName = "MONITORDATASETNAME";
            BindingListBox(commandText, lstMonitor, bindFieldName);

            commandText = string.Format("select  INCIDENCEDATASETNAME from INCIDENCEDATASETS where SetupID={0} order  by INCIDENCEDATASETNAME asc", setupID);
            bindFieldName = "INCIDENCEDATASETNAME";
            BindingListBox(commandText, lstIncidenceOrPrevalence, bindFieldName);

            commandText = string.Format("select  POPULATIONDATASETNAME from POPULATIONDATASETS where SetupID={0} and populationdatasetid<>37 order  by POPULATIONDATASETNAME asc", setupID);
            bindFieldName = "POPULATIONDATASETNAME";
            BindingListBox(commandText, lstPopulation, bindFieldName);

            //commandText = string.Format("select  QALYDATASETNAME from QALYDATASETS where SetupID={0} order  by QALYDATASETNAME asc", setupID);
            //bindFieldName = "QALYDATASETNAME";
            //BindingListBox(commandText, lstQALY, bindFieldName);

            commandText = string.Format("select  CRFUNCTIONDATASETNAME from CRFUNCTIONDATASETS where SetupID={0} order  by CRFUNCTIONDATASETNAME asc", setupID);
            bindFieldName = "CRFUNCTIONDATASETNAME";
            BindingListBox(commandText, lstHealthImpact, bindFieldName);

            commandText = string.Format("select  SETUPVARIABLEDATASETNAME from SETUPVARIABLEDATASETS where SetupID={0} order  by SETUPVARIABLEDATASETNAME asc", setupID);
            bindFieldName = "SETUPVARIABLEDATASETNAME";
            BindingListBox(commandText, lstVariable, bindFieldName);

            commandText = string.Format("select  INFLATIONDATASETNAME from INFLATIONDATASETS where SetupID={0} order  by INFLATIONDATASETNAME asc", setupID);
            bindFieldName = "INFLATIONDATASETNAME";
            BindingListBox(commandText, lstInflation, bindFieldName);

            commandText = string.Format("select  VALUATIONFUNCTIONDATASETNAME from VALUATIONFUNCTIONDATASETS where SetupID={0} order  by VALUATIONFUNCTIONDATASETNAME asc", setupID);
            bindFieldName = "VALUATIONFUNCTIONDATASETNAME";
            BindingListBox(commandText, lstValuation, bindFieldName);

            commandText = string.Format("select  INCOMEGROWTHADJDATASETNAME from INCOMEGROWTHADJDATASETS where SetupID={0} order  by INCOMEGROWTHADJDATASETNAME asc", setupID);
            bindFieldName = "INCOMEGROWTHADJDATASETNAME";
            BindingListBox(commandText, lstIncome, bindFieldName);
        }
        //Bind the fields of tables from Firebirddatabase to listbox
        private bool BindingListBox(string commandText, ListBox lstBox, string bindFieldName)
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstBox.DataSource = ds.Tables[0];
                lstBox.DisplayMember = bindFieldName;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Bind the fields of tables from Firebirddatabase to combobox
        private bool BindingComboBox(string commondText, ComboBox cboBox, string bindFieldName)
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commondText);
                cboBox.DataSource = ds.Tables[0];
                cboBox.DisplayMember = bindFieldName;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            NewSetUp frm = new NewSetUp();
            DialogResult rth = frm.ShowDialog();
            if (rth != DialogResult.OK) { return; }
            //cboSetupName.Items.Add(frm.NewSetupName);
            //查询下一个SetupID
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            string commandText = "select max(SETUPID) from Setups";
            object objSetup = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
            int setupID = int.Parse(objSetup.ToString()) + 1;
            //插入新的区域和SetupID
            commandText = string.Format("insert into Setups values({0},'{1}')", setupID, frm.NewSetupName);
            int rht = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
            commandText = string.Format("select SetupID,SetupName from Setups order by SetupID");
            DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
            commandText = string.Format("select SetupID,SetupName from Setups order by SetupID");
            ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
            cboSetupName.DataSource = ds.Tables[0];
            cboSetupName.DisplayMember = "SetupName";
            cboSetupName.SelectedIndex = cboSetupName.Items.Count - 1;

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void cboSetupName_SelectedValueChanged(object sender, EventArgs e)
        {
            DataRowView dgv = cboSetupName.SelectedItem as DataRowView;
            CommonClass.ManageSetup = new BenMAPSetup()
            {
                SetupID = Convert.ToInt32(dgv["SetupID"]),
                SetupName = dgv["SetupName"].ToString()
            };
            LoadItems();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (CommonClass.MainSetup.SetupID == CommonClass.ManageSetup.SetupID)
            {
                MessageBox.Show("Can not delete the current setup.");
                return;
            }
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            string commandText = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                if (CommonClass.ManageSetup.SetupID == 1)
                {
                    MessageBox.Show("Can not delete the setup of '" + CommonClass.ManageSetup.SetupName + "'.");
                    return;
                }
                if (MessageBox.Show("Delete the selected setup?", "Confirm Deletion", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    //if (lstGridDefinitions.Items.Count == 0 && lstHealthImpact.Items.Count == 0 && lstIncidenceOrPrevalence.Items.Count == 0 && lstIncome.Items.Count == 0 && lstInflation.Items.Count == 0 && lstMonitor.Items.Count == 0 && lstPollutans.Items.Count == 0 && lstPopulation.Items.Count == 0 && lstQALY.Items.Count == 0 && lstValuation.Items.Count == 0 && lstVariable.Items.Count == 0)
                    //{
                    WaitShow("Waiting for deleting the setup...");
                    commandText = string.Format("delete from T_populationDatasetIDYear where PopulationDatasetID in (select PopulationDatasetID from PopulationDatasets where setupid ={0})", CommonClass.ManageSetup.SetupID);//T_populationDatasetIDYear没有和表PopulationDatasets关联，需要加这句删除，或改动数据库
                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    commandText = string.Format("delete from ShapefileGridDefinitiondetails where griddefinitionid in (select griddefinitionid from Griddefinitions where setupid={0})", CommonClass.ManageSetup.SetupID);//ShapefileGridDefinitiondetails没有和表Griddefinitions关联
                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    if (System.IO.Directory.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName))
                    {
                        System.IO.Directory.Delete(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName, true);
                    }
                    commandText = string.Format("delete from Setups where SetupName='{0}'", cboSetupName.Text);
                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    WaitClose();
                }
                commandText = string.Format("select SetupID,SetupName from Setups order by SetupID");
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboSetupName.DataSource = ds.Tables[0];
                cboSetupName.DisplayMember = "SetupName";
                if (cboSetupName.Items.Count > 0)
                    cboSetupName.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        #region 多线程显示tip窗口

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
            try
            {
                if (waitMess.InvokeRequired)
                    waitMess.Invoke(new CloseFormDelegate(DoCloseJob));
                else
                    DoCloseJob();
            }
            catch (Exception ex)
            { }
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

        #endregion 多线程显示tip窗口

        //private void ManageSetup_SizeChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // MessageBox.Show("窗体尺寸改变");           
        //        Wcounter = 0;
        //        int counter = 0;
        //        if (this.Size.Width < FormSizeWidth || this.Size.Height < FormSizeHeight)                //如果窗体的大小在改变过程中小于窗体尺寸的初始值，则窗体中的各个控件自动重置为初始尺寸，且窗体自动添加滚动条             
        //        {
        //            foreach (Control iniCrl in InitialCrl)
        //            {
        //                iniCrl.Width = (int)CrlSizeWidth[counter];
        //                iniCrl.Height = (int)CrlSizeHeight[counter];
        //                Point point = new Point();
        //                point.X = (int)CrlLocationX[counter];
        //                point.Y = (int)CrlLocationY[counter];
        //                iniCrl.Bounds = new Rectangle(point, iniCrl.Size);
        //                counter++;
        //            }
        //            this.AutoScroll = true;
        //        }
        //        else                //否则，重新设定窗体中所有控件的大小（窗体内所有控件的大小随窗体大小的变化而变化） 
        //        {
        //            this.AutoScroll = false;
        //            ResetAllCrlState(this);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex.Message);
        //    }
        //}

        //public void GetAllCrlLocation(Control CrlContainer)//获得并存储窗体中各控件的初始位置      
        //{
        //    foreach (Control iCrl in CrlContainer.Controls)
        //    {
        //        if (iCrl.Controls.Count > 0)
        //            GetAllCrlLocation(iCrl);
        //        InitialCrl.Add(iCrl);
        //        CrlLocationX.Add(iCrl.Location.X);
        //        CrlLocationY.Add(iCrl.Location.Y);
        //    }
        //}
        //public void GetAllCrlSize(Control CrlContainer)//获得并存储窗体中各控件的初始尺寸
        //{
        //    foreach (Control iCrl in CrlContainer.Controls)
        //    {
        //        if (iCrl.Controls.Count > 0)
        //            GetAllCrlSize(iCrl);
        //        CrlSizeWidth.Add(iCrl.Width);
        //        CrlSizeHeight.Add(iCrl.Height);
        //    }
        //}
        //public void GetInitialFormSize()//获得并存储窗体的初始尺寸    
        //{
        //    FormSizeWidth = this.Size.Width;
        //    FormSizeHeight = this.Size.Height;
        //}
        //public void ResetAllCrlState(Control CrlContainer)//重新设定窗体中各控件的状态（在与原状态的对比中计算而来）      
        //{
        //    FormSizeChangedX = (double)this.Size.Width / (double)FormSizeWidth;
        //    FormSizeChangedY = (double)this.Size.Height / (double)FormSizeHeight;
        //    foreach (Control kCrl in CrlContainer.Controls)
        //    {                 /*string name = kCrl.Name.ToString();          
        //                       * MessageBox.Show(name);              
        //                       * MessageBox.Show(Wcounter.ToString());*/
        //        if (kCrl.Controls.Count > 0)
        //        {
        //            ResetAllCrlState(kCrl);
        //        } Point point = new Point();
        //        point.X = (int)((int)CrlLocationX[Wcounter] * FormSizeChangedX);
        //        point.Y = (int)((int)CrlLocationY[Wcounter] * FormSizeChangedY);
        //        kCrl.Width = (int)((int)CrlSizeWidth[Wcounter] * FormSizeChangedX);
        //        kCrl.Height = (int)((int)CrlSizeHeight[Wcounter] * FormSizeChangedY);
        //        kCrl.Bounds = new Rectangle(point, kCrl.Size);
        //        Wcounter++;
        //    }
        //}
    }
}
