using System;
using System.Data;
using System.Windows.Forms;
using ESIL.DBUtility;

namespace BenMAP
{
    public partial class LoadIncomeGrowthDataSet : FormBase
    {
        public LoadIncomeGrowthDataSet()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtDatabase.Text == string.Empty)
                {
                    MessageBox.Show("Please select a datafile.");
                    return;
                }
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select IncomeGrowthadjdatasetid from IncomeGrowthadjdatasets where setupid={0} and IncomeGrowthadjdatasetname='{1}'", CommonClass.ManageSetup.SetupID, txtDataSetName.Text);
                object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                if (obj != null)
                {
                    MessageBox.Show("This income growth dataset name is already in use. Please enter a different name.");
                    return;
                }
                DataTable dt = new DataTable();
                string strfilename = string.Empty;
                string strtablename = string.Empty;
                commandText = string.Empty;

                dt = CommonClass.ExcelToDataTable(txtDatabase.Text);
                int iYear = -1;
                int iMean = -1;
                int iEndpointGroup = -1;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    switch (dt.Columns[i].ColumnName.ToLower().Replace(" ", ""))
                    {
                        case "year": iYear = i;
                            break;
                        case "mean": iMean = i;
                            break;
                        case "endpointgroup": iEndpointGroup = i;
                            break;
                    }
                }
                string warningtip = "";
                if (iYear < 0) warningtip = "'Year', ";
                if (iMean < 0) warningtip += "'Mean', ";
                if (iEndpointGroup < 0) warningtip += "'Endpoint Group', ";
                if (warningtip != "")
                {
                    warningtip = warningtip.Substring(0, warningtip.Length - 2);
                    warningtip = "Please check the column header of " + warningtip + ". It is incorrect or does not exist.";
                    MessageBox.Show(warningtip, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                commandText = "SELECT max(INCOMEGROWTHADJDATASETID) from INCOMEGROWTHADJDATASETS";
                int incomegrowthadjdatasetID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText)) + 1;
                commandText = string.Format("insert into INCOMEGROWTHADJDATASETS VALUES({0},{1},'{2}' )", incomegrowthadjdatasetID, CommonClass.ManageSetup.SetupID, txtDataSetName.Text);
                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                int currentDataSetID = incomegrowthadjdatasetID;

                if (dt == null) { return; }
                int rtn = 0;
                foreach (DataRow row in dt.Rows)
                {
                    if (row == null)
                    { continue; }
                    commandText = string.Format("insert into INCOMEGROWTHADJFACTORS(INCOMEGROWTHADJDATASETID,YYEAR,MEAN,ENDPOINTGROUPS) values({0},{1},{2},'{3}')", currentDataSetID, int.Parse(row[iYear].ToString()), row[iMean], row[iEndpointGroup]);
                    rtn = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                }
                if (rtn != 0)
                {
                    IncomeGrowthDataSetName = txtDataSetName.Text;
                }
            }

            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = Application.StartupPath + @"E:\";
                openFileDialog.Filter = "All Files|*.*|CSV files|*.csv|XLS files|*.xls|XLSX files|*.xlsx";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != DialogResult.OK) { return; }
                txtDatabase.Text = openFileDialog.FileName;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private string _incomeGrowthDataSetName;

        public string IncomeGrowthDataSetName
        {
            get { return _incomeGrowthDataSetName; }
            set { _incomeGrowthDataSetName = value; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void LoadIncomeGrowthDataSet_Load(object sender, EventArgs e)
        {
            try
            {
                FireBirdHelperBase fb = new ESILFireBirdHelper();
                if (_incomeGrowthDataSetName == null)
                {
                    int number = 0;
                    int incomeGrowthDatasetID = 0;
                    do
                    {
                        string comText = "select incomeGrowthADJDatasetID from incomeGrowthADJDataSets where incomeGrowthADJDatasetName=" + "'IncomeGrowthDataSet" + Convert.ToString(number) + "'";
                        incomeGrowthDatasetID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, comText));
                        number++;
                    } while (incomeGrowthDatasetID > 0);
                    txtDataSetName.Text = "IncomeGrowthDataSet" + Convert.ToString(number - 1);
                }
            }
            catch (Exception)
            { }
        }
    }
}