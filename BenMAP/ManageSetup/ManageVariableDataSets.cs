using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BenMAP
{
    public partial class ManageVariableDataSets : FormBase
    {
        public ManageVariableDataSets()
        {
            InitializeComponent();
        }
        string _dataName = string.Empty;
        private void btn_Click(object sender, EventArgs e)
        {
            try
            {
                VariableDataSetDefinition frm = new VariableDataSetDefinition();
                DialogResult rtn = frm.ShowDialog();
                if (rtn != DialogResult.OK) { return; }
                ExportDataForlistbox();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void ManageVariableDataSets_Load(object sender, EventArgs e)
        {
            try
            {

                ExportDataForlistbox();

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void ExportDataForlistbox()
        {
            try
            {
                string commandText = string.Format("select * from SETUPVARIABLEDATASETS where setupid={0}", CommonClass.ManageSetup.SetupID);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstAvailable.DataSource = ds.Tables[0];
                lstAvailable.DisplayMember = "SETUPVARIABLEDATASETNAME";
                if (lstAvailable.Items.Count > 0)
                {
                    lstAvailable.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        private void lstAvailable_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender == null) { return; }
                var lst = sender as ListBox;
                if (lst.SelectedItem == null) return;
                DataRowView dr = lst.SelectedItem as DataRowView;
                string str = dr.Row["SETUPVARIABLEDATASETNAME"].ToString();
                string commandText = string.Format("select SETUPVARIABLEID,SETUPVARIABLEDATASETID,SETUPVARIABLENAME,GRIDDEFINITIONID from SETUPVARIABLES WHERE SETUPVARIABLEDATASETID in (select SETUPVARIABLEDATASETID from SETUPVARIABLEDATASETS where SETUPVARIABLEDATASETNAME='{0}' and setupid={1})  ORDER BY SETUPVARIABLENAME ASC", str, CommonClass.ManageSetup.SetupID);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstVariables.DataSource = ds.Tables[0];
                lstVariables.DisplayMember = "SETUPVARIABLENAME";
                lstVariables.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstAvailable.SelectedItem == null) return;
                string str = lstAvailable.GetItemText(lstAvailable.SelectedItem);
                VariableDataSetDefinition frm = new VariableDataSetDefinition(str);
                DialogResult rth = frm.ShowDialog();
                ExportDataForlistbox();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstAvailable.SelectedItem == null) return;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Empty;
                int varDts = 0; //Variable Dataset
                int dstID = 0;//DataSetTypeID
                if (MessageBox.Show("Delete the selected variable dataset?", "Confirm Deletion", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    WaitShow("Deleting data...");

                    commandText = string.Format("SELECT VALUATIONFUNCTIONDATASETID FROM VALUATIONFUNCTIONDATASETS WHERE VALUATIONFUNCTIONDATASETNAME = '{0}' and SETUPID = {1}", lstAvailable.Text, CommonClass.ManageSetup.SetupID);
                    varDts = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));
                    commandText = "SELECT DATASETID FROM DATASETS WHERE DATASETNAME = 'VariableDataset'";
                    dstID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));

                    commandText = string.Format("delete from  SETUPVARIABLEDATASETS where SETUPVARIABLEDATASETNAME='{0}' and setupid={1}", lstAvailable.Text, CommonClass.ManageSetup.SetupID);
                    int i = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                    commandText = string.Format("DELETE FROM METADATAINFORMATION WHERE SETUPID = {0} AND DATASETID = {1} AND DATASETTYPEID = {2}", CommonClass.ManageSetup.SetupID, varDts, dstID);
                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                }
                commandText = string.Format("select * from SETUPVARIABLEDATASETS where SetupID={0} ", CommonClass.ManageSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstAvailable.DataSource = ds.Tables[0];
                lstAvailable.DisplayMember = "SETUPVARIABLEDATASETNAME";
                WaitClose();
            }
            catch (Exception ex)
            {
                WaitClose();
                Logger.LogError(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
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
        private delegate void ChangeDelegate(string msg);
        public void WaitClose()
        {
            if (waitMess.InvokeRequired)
                waitMess.Invoke(new CloseFormDelegate(DoCloseJob));
            else
                DoCloseJob();
        }

        public void WaitChangeMsg(string msg)
        {
            try
            {
                if (waitMess.InvokeRequired)
                    waitMess.Invoke(new ChangeDelegate(DoChange), msg);
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
        }
        private void DoChange(string msg)
        {
            try
            {
                if (!waitMess.IsDisposed)
                {
                    if (waitMess.Created)
                    {
                        waitMess.Msg = msg;
                    }
                }
            }
            catch (System.Threading.ThreadAbortException Err)
            {
                MessageBox.Show(Err.Message);
            }
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
