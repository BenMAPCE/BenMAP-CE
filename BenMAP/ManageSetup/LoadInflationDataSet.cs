using System;
using System.Data;
using System.Windows.Forms;
using ESIL.DBUtility;

namespace BenMAP
{
    public partial class LoadInflationDataSet : FormBase
    {
        public LoadInflationDataSet()
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
                string commandText = string.Format("select inflationdatasetid from inflationdatasets where setupid={0} and inflationdatasetname='{1}'", CommonClass.ManageSetup.SetupID, txtInflationDataSetName.Text);
                object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                if (obj != null)
                {
                    MessageBox.Show("This inflation dataset name is already in use. Please enter a different name.");
                    return;
                }
                DataTable dt = new DataTable();
                string strfilename = string.Empty;
                string strtablename = string.Empty;
                commandText = string.Empty;
                // Todo:陈志润
                //int sheetIndex = 1;
                //if (txtDatabase.Text.Substring(txtDatabase.Text.Length - 3, 3).ToLower() != "csv")
                //{
                //    //判断有没有安装Excel
                //    if (Type.GetTypeFromProgID("Excel.Application") == null)
                //    {
                //        MessageBox.Show("Please install Excel.", "Warning", MessageBoxButtons.OK);
                //        return;
                //    }
                //    sheetIndex = CommonClass.SelectedSheetIndex(txtDatabase.Text);
                //}

                dt = CommonClass.ExcelToDataTable(txtDatabase.Text);
                int iYear = -1;
                int iAllGoodsIndex = -1;
                int iMedicalCostIndex = -1;
                int iWageIndex = -1;

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    switch (dt.Columns[i].ColumnName.ToLower().Replace(" ", ""))
                    {
                        case "year": iYear = i;
                            break;
                        case "allgoodsindex": iAllGoodsIndex = i;
                            break;
                        case "medicalcostindex": iMedicalCostIndex = i;
                            break;
                        case "wageindex": iWageIndex = i;
                            break;
                    }
                }
                string warningtip = "";
                if (iYear < 0) warningtip = "'Year', ";
                if (iAllGoodsIndex < 0) warningtip += "'AllGoodsIndex', ";
                if (iMedicalCostIndex < 0) warningtip += "'MedicalCostIndex', ";
                if (iWageIndex < 0) warningtip += "'WageIndex', ";
                if (warningtip != "")
                {
                    warningtip = warningtip.Substring(0, warningtip.Length - 2);
                    warningtip = "Please check the column header of " + warningtip + ". It is incorrect or does not exist.";
                    MessageBox.Show(warningtip, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                commandText = "SELECT max(INFLATIONDATASETID) from INFLATIONDATASETS";
                int inflationdatasetid = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText)) + 1;
                commandText = string.Format("insert into INFLATIONDATASETS VALUES({0},{1},'{2}' )",inflationdatasetid, CommonClass.ManageSetup.SetupID, txtInflationDataSetName.Text);
                int rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                //commandText = string.Format("select next value for SEQ_INFLATIONDATASETS FROM RDB$DATABASE");
                //obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                //if (obj == null) { return; }
                int currentDataSetID = inflationdatasetid;
                //ds = dp.GetDataFromFile(txtDatabase.Text);
                //string strfilepath = System.IO.Path.GetExtension(txtDatabase.Text);
                //switch (strfilepath.ToLower())
                //{
                //    case ".xls":
                //        ds = dp.ReadExcel2DataSet(txtDatabase.Text);
                //        break;
                //    case ".xlsx":
                //        ds = dp.ReadExcel2DataSet(txtDatabase.Text);
                //        break;
                //    case ".csv":
                //        ds = dp.ReadCSV2DataSet(txtDatabase.Text, "table");
                //        break;
                //    default: break;
                //}
                if (dt == null) { return; }
                int rtn = 0;
                foreach (DataRow row in dt.Rows)
                {
                    if (row == null)
                    { continue; }
                    commandText = string.Format("insert into INFLATIONENTRIES values({0},{1},{2},{3},{4})", currentDataSetID, int.Parse(row[iYear].ToString()), row[iAllGoodsIndex], row[iMedicalCostIndex], row[iWageIndex]);
                    //string commandText = "insert into INCOMEGROWTHADJFACTORS values(15,2036,0.1111111111,'opip',0.11111,0.211111,'Acute Brochitis')";
                    rtn = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                }
                if (rtn != 0)
                {
                    InflationDataSetName = txtInflationDataSetName.Text;
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

        private string _inflationDataSetName;

        public string InflationDataSetName
        {
            get { return _inflationDataSetName; }
            set { _inflationDataSetName = value; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void LoadInflationDataSet_Load(object sender, EventArgs e)
        {
            string commandText = string.Empty;
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                if (_inflationDataSetName == null)
                {
                    int number = 0;
                    int InflationDatasetID = 0;
                    do
                    {
                        string comText = "select inflationDatasetID from inflationDatasets where inflationDatasetName=" + "'InflationDataSet" + Convert.ToString(number) + "'";
                        InflationDatasetID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, comText));
                        number++;
                    } while (InflationDatasetID > 0);
                    txtInflationDataSetName.Text = "InflationDataSet" + Convert.ToString(number - 1);
                }
            }
            catch (Exception)
            { }
        }
    }
}