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
        string _dataName = string.Empty;

        public ManageSetup()
        {
            InitializeComponent();
        }
        private void btnGridDef_Click(object sender, EventArgs e)
        {
            ManageGridDefinetions frm = new ManageGridDefinetions();
            DialogResult rtn = frm.ShowDialog();
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
                {
                    string commandText = string.Format("select  VALUATIONFUNCTIONDATASETNAME from VALUATIONFUNCTIONDATASETS where setupID={0} ", CommonClass.ManageSetup.SetupID);
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
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
                {
                    string commandText = string.Format("select  INCOMEGROWTHADJDATASETNAME from INCOMEGROWTHADJDATASETS where setupID={0}", CommonClass.ManageSetup.SetupID);
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    lstIncome.DataSource = ds.Tables[0];
                    lstIncome.DisplayMember = "INCOMEGROWTHADJDATASETNAME";
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }


        private void btnPopulation_Click(object sender, EventArgs e)
        {
            ManagePopulationDataSets frm = new ManagePopulationDataSets();
            DialogResult rtn = frm.ShowDialog();
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
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            string commandText = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                commandText = string.Format("select SetupID,SetupName from Setups order by SetupID");
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboSetupName.DataSource = ds.Tables[0];
                cboSetupName.DisplayMember = "SetupName";
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
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            string commandText = "select max(SETUPID) from Setups";
            object objSetup = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
            int setupID = int.Parse(objSetup.ToString()) + 1;
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
                    WaitShow("Waiting for deleting the setup...");
                    commandText = string.Format("delete from T_populationDatasetIDYear where PopulationDatasetID in (select PopulationDatasetID from PopulationDatasets where setupid ={0})", CommonClass.ManageSetup.SetupID); fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    commandText = string.Format("delete from ShapefileGridDefinitiondetails where griddefinitionid in (select griddefinitionid from Griddefinitions where setupid={0})", CommonClass.ManageSetup.SetupID); fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
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



    }
}
