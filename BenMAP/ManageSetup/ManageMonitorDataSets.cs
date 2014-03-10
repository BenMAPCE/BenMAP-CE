using System;
using System.Data;
using System.Windows.Forms;

namespace BenMAP
{
    public partial class ManageMonitorDataSets : FormBase
    {
        public ManageMonitorDataSets()
        {
            InitializeComponent();
        }

        string _dataName = string.Empty;
        private string _lstDataSetName;
        private object _lstDataSetID;

        private void ManageMonitorDataSets_Load(object sender, EventArgs e)
        {
            addLstBox();
        }

        private void lstAvailableDataSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender == null) { return; }
                var lst = sender as ListBox;
                DataRowView drv = lst.SelectedItem as DataRowView;
                _lstDataSetID = drv["MonitorDataSetID"];
                _lstDataSetName = drv["MonitorDataSetName"].ToString();
                addGridView();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void addLstBox()
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select MonitorDataSetID, MonitorDataSetName from MonitorDataSets where setupid={0} order  by MonitorDataSetName asc", CommonClass.ManageSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstAvailableDataSets.DataSource = ds.Tables[0];
                lstAvailableDataSets.DisplayMember = "MonitorDataSetName";
                if (ds.Tables[0].Rows.Count == 0)
                {
                    olvMonitorDataSets.ClearObjects();
                }
                else
                {
                    lstAvailableDataSets.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void addGridView()
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select c.pollutantname,a.yyear,count(*) from monitorentries a,monitors b,pollutants c where a.monitorid=b.monitorid and b.pollutantid=c.pollutantid and b.monitordatasetID={0} group by c.pollutantname,a.yyear", _lstDataSetID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                olvMonitorDataSets.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lstAvailableDataSets.Items.Count == 0)
            { return; }
            MonitorDataSetDefinition frm = new MonitorDataSetDefinition(_lstDataSetName, _lstDataSetID);
            DialogResult rtn = frm.ShowDialog();
            {
                addLstBox();
                addGridView();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            MonitorDataSetDefinition frm = new MonitorDataSetDefinition();
            DialogResult rtn = frm.ShowDialog();
            {
                addLstBox();
                addGridView();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstAvailableDataSets.SelectedItem == null) return;
                DialogResult rtn = MessageBox.Show("Delete the '" + _lstDataSetName + "' monitor dataset?", "Confirm Deletion", MessageBoxButtons.YesNo);
                if (rtn == DialogResult.Yes)
                {
                    ESIL.DBUtility.ESILFireBirdHelper fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    string commandText = string.Format("delete from MonitorDataSets where MonitorDataSetID={0}", _lstDataSetID); fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    addLstBox();
                    addGridView();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}