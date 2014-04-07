using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESIL.DBUtility;

namespace BenMAP
{
    public partial class frmIncomeGrowthAdjustmentDataSet : FormBase
    {
        public frmIncomeGrowthAdjustmentDataSet()
        {
            InitializeComponent();
        }
        string _dataName = string.Empty;
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                LoadIncomeGrowthDataSet frm = new LoadIncomeGrowthDataSet();
                frm.ShowDialog();
                ExportDataForlistbox();
                int count = -1;
                for (int i = 0; i < lstDataSetName.Items.Count; i++)
                {
                    DataRowView dr = lstDataSetName.Items[i] as DataRowView;
                    string datasetName = dr["INCOMEGROWTHADJDATASETNAME"].ToString();
                    if (datasetName == frm.IncomeGrowthDataSetName)
                    {
                        count = i;
                        break;
                    }
                }
                lstDataSetName.SelectedIndex = count;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void IncomeGrowthAdjustmentDataSet_Load(object sender, EventArgs e)
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
                string commandText = string.Format("select  * from INCOMEGROWTHADJDATASETS where setupid={0} ", CommonClass.ManageSetup.SetupID);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstDataSetName.DataSource = ds.Tables[0];
                lstDataSetName.DisplayMember = "INCOMEGROWTHADJDATASETNAME";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void lstDataSetName_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {

                if (sender == null) { return; }
                var lst = sender as ListBox;
                if (lst.SelectedItem == null) return;
                DataRowView dr = lst.SelectedItem as DataRowView;
                string str = dr.Row["INCOMEGROWTHADJDATASETNAME"].ToString();
                string commandText = string.Format("select YYEAR,MEAN,ENDPOINTGROUPS from INCOMEGROWTHADJFACTORS WHERE INCOMEGROWTHADJDATASETID in (select INCOMEGROWTHADJDATASETID from INCOMEGROWTHADJDATASETS where INCOMEGROWTHADJDATASETNAME='{0}' and setupid={1})  ORDER BY YYEAR ASC", str, CommonClass.ManageSetup.SetupID);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);

                olvData.DataSource = ds.Tables[0];
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstDataSetName.SelectedItem == null) return;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Empty;
                int igaDstID = 0; //Income Growth Adjustment Dataset ID
                int dstID = 0;//DataSetTypeID
                if (MessageBox.Show("Delete the selected income growth adjustment dataset?", "Confirm Deletion", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    commandText = string.Format("SELECT INCOMEGROWTHADJDATASETID FROM INCOMEGROWTHADJDATASETS WHERE INCOMEGROWTHADJDATASETNAME = '{0}' and SETUPID = {1}", lstDataSetName.Text, CommonClass.ManageSetup.SetupID);
                    igaDstID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));
                    commandText = "SELECT DATASETID FROM DATASETS WHERE DATASETNAME = 'Incomegrowth'";
                    dstID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));

                    commandText = string.Format("delete from INCOMEGROWTHADJDATASETS where INCOMEGROWTHADJDATASETNAME='{0}' and setupid={1}", lstDataSetName.Text, CommonClass.ManageSetup.SetupID);
                    int i = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                    commandText = string.Format("DELETE FROM METADATAINFORMATION WHERE SETUPID = {0} AND DATASETID = {1} AND DATASETTYPEID = {2}", CommonClass.ManageSetup.SetupID, igaDstID, dstID);
                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                }
                commandText = string.Format("select * from INCOMEGROWTHADJDATASETS where SetupID={0}", CommonClass.ManageSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstDataSetName.DataSource = ds.Tables[0];
                lstDataSetName.DisplayMember = "INCOMEGROWTHADJDATASETNAME";
                if (ds.Tables[0].Rows.Count == 0)
                {
                    olvData.ClearObjects();
                }
            }
            catch
            { }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "CSV File|*.CSV";
                saveFileDialog1.InitialDirectory = "C:\\";
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                { return; }
                string fileName = saveFileDialog1.FileName;
                DataTable dtOut = new DataTable();

                dtOut.Columns.Add("Year", typeof(int));
                dtOut.Columns.Add("Mean", typeof(double));
                dtOut.Columns.Add("EndpointGroup", typeof(string));
                FireBirdHelperBase fb = new ESILFireBirdHelper();
                string commandText = string.Empty;
                int outputRowsNumber = 50;
                commandText = "select count(*) from IncomeGrowthAdjFactors";
                int count = (int)fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                if (count < outputRowsNumber) { outputRowsNumber = count; }
                commandText = string.Format("select first {0} endpointGroups, YYear,Mean from IncomeGrowthAdjFactors", outputRowsNumber);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataRow newdr = dtOut.NewRow();
                    newdr["Year"] = Convert.ToInt32(dr["YYear"]);
                    newdr["Mean"] = Convert.ToDouble(dr["Mean"]);
                    newdr["EndpointGroup"] = dr["endpointGroups"].ToString();
                    dtOut.Rows.Add(newdr);
                }
                CommonClass.SaveCSV(dtOut, fileName);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
    }
}
