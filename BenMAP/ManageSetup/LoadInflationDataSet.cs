using System;
using System.Data;
using System.Windows.Forms;
using ESIL.DBUtility;
//TODO:
//make sure there is a name in the txtInflationDataSetName text box
namespace BenMAP
{
    public partial class LoadInflationDataSet : FormBase
    {
        private MetadataClassObj _metadataObj = null;
        private DataTable _inflationData;
        public DataTable InflationData
        {
            get { return _inflationData; }
        }
        private string _strPath;
        private string _inflationDataSetName;
        private string _isForceValidate = string.Empty;
        private string _iniPath = string.Empty;
        public string InflationDataSetName
        {
            get { return _inflationDataSetName; }
            set { _inflationDataSetName = value; }
        }
        
        public LoadInflationDataSet()
        {
            InitializeComponent();
            _iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
            _isForceValidate = CommonClass.IniReadValue("appSettings", "IsForceValidate", _iniPath);
            if (_isForceValidate == "T")
                btnOK.Enabled = false;
            else
                btnOK.Enabled = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
           LoadDatabase();
        }
        private void GetMetadata()
        {
            _metadataObj = new MetadataClassObj();
            Metadata metadata = new Metadata(_strPath);
            _metadataObj = metadata.GetMetadata();
        }
        private void LoadDatabase()
        {
            try
            {
                if (txtDatabase.Text == string.Empty)
                {
                    MessageBox.Show("Please select a datafile.");
                    return;
                }
                if(string.IsNullOrEmpty(txtInflationDataSetName.Text))
                {
                    MessageBox.Show("Please enter a dataset name.");
                    txtInflationDataSetName.Focus();
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
                    warningtip = "Please check the column header of " + warningtip + ". It is incorrect or does not exist.\r\n";
                    warningtip += "\r\nFile failed to load, please validate the file for a more detail explanation of errors.";
                    MessageBox.Show(warningtip, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                commandText = "SELECT max(INFLATIONDATASETID) from INFLATIONDATASETS";
                int inflationdatasetid = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText)) + 1;
                commandText = string.Format("insert into INFLATIONDATASETS VALUES({0},{1},'{2}' )", inflationdatasetid, CommonClass.ManageSetup.SetupID, txtInflationDataSetName.Text);
                int rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                int currentDataSetID = inflationdatasetid;
                int rtn = 0;

                if (dt == null) 
                { 
                    return; 
                }
                
                foreach (DataRow row in dt.Rows)
                {
                    if (row == null)
                    { continue; }
                    commandText = string.Format("insert into INFLATIONENTRIES values({0},{1},{2},{3},{4})", currentDataSetID, int.Parse(row[iYear].ToString()), row[iAllGoodsIndex], row[iMedicalCostIndex], row[iWageIndex]);
                    rtn = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                }

                if (rtn != 0)
                {
                    InflationDataSetName = txtInflationDataSetName.Text;
                }

                commandText = "SELECT DATASETID FROM DATASETS WHERE DATASETNAME = 'Inflation'";
                _metadataObj.DatasetTypeId = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));
                rtn = 0;//reseting the return number
                commandText = string.Format("INSERT INTO METADATAINFORMATION " +
                                            "(SETUPID, DATASETID, DATASETTYPEID, FILENAME, " +
                                            "EXTENSION, DATAREFERENCE, FILEDATE, IMPORTDATE, DESCRIPTION, " + 
                                            "PROJECTION, GEONAME, DATUMNAME, DATUMTYPE, SPHEROIDNAME, " +
                                            "MERIDIANNAME, UNITNAME, PROJ4STRING, NUMBEROFFEATURES) " + 
                                            "VALUES('{0}', '{1}', '{2}', '{3}', '{4}','{5}', '{6}', '{7}', '{8}', '{9}', " +
                                            "'{10}', '{11}', '{12}', '{13}', '{14}','{15}', '{16}', '{17}')",
                                            _metadataObj.SetupId, inflationdatasetid, _metadataObj.DatasetTypeId, _metadataObj.FileName,
                                            _metadataObj.Extension, _metadataObj.DataReference, _metadataObj.FileDate, _metadataObj.ImportDate,
                                            _metadataObj.Description, _metadataObj.Projection, _metadataObj.GeoName, _metadataObj.DatumName,
                                            _metadataObj.DatumType, _metadataObj.SpheroidName, _metadataObj.MeridianName, _metadataObj.UnitName,
                                            _metadataObj.Proj4String, _metadataObj.NumberOfFeatures);
                rtn = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
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
                if (openFileDialog.ShowDialog() != DialogResult.OK) 
                { 
                    return; 
                }
                txtDatabase.Text = openFileDialog.FileName;
                GetMetadata();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
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

        private void txtDatabase_TextChanged(object sender, EventArgs e)
        {
            btnValidate.Enabled = !string.IsNullOrEmpty(txtDatabase.Text);
            btnViewMetadata.Enabled = !string.IsNullOrEmpty(txtDatabase.Text);
            _strPath = txtDatabase.Text;
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            _inflationData = CommonClass.ExcelToDataTable(_strPath);
            ValidateDatabaseImport vdi = new ValidateDatabaseImport(_inflationData, "Inflation", _strPath);

            DialogResult dlgR = vdi.ShowDialog();
            if (dlgR.Equals(DialogResult.OK))
            {
                if (vdi.PassedValidation && _isForceValidate == "T")
                    LoadDatabase();
            }
        }

        private void btnViewMetadata_Click(object sender, EventArgs e)
        {
            ViewEditMetadata viewEMdata = null;
            if (_metadataObj != null)
            {
                viewEMdata = new ViewEditMetadata(_strPath, _metadataObj);
            }
            else
            {
                viewEMdata = new ViewEditMetadata(_strPath);
            }
            viewEMdata.ShowDialog();
            _metadataObj = viewEMdata.MetadataObj;
        }
    }
}