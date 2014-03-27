using System;
using System.Data;
using System.Windows.Forms;



namespace BenMAP
{
    public partial class LoadIncidenceDatabase : FormBase
    {

        private DataTable _incidneceData;
        public DataTable IncidneceData
        {
            get { return _incidneceData; }
        }

        public LoadIncidenceDatabase()
        {
            InitializeComponent();
            _iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
            _isForceValidate = CommonClass.IniReadValue("appSettings", "IsForceValidate", _iniPath);
            if (_isForceValidate == "T")
                btnOK.Enabled = false;
            else
                btnOK.Enabled = true;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = CommonClass.ResultFilePath;
            openFileDialog.Filter = "All Files|*.*|CSV files|*.csv|XLS files|*.xls|XLSX files|*.xlsx";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            { return; }
            txtDatabase.Text = openFileDialog.FileName;
        }

        private void LoadIncidenceDatabase_Load(object sender, EventArgs e)
        {
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            string commandText = string.Empty;
            try
            {
                if (GridDefinitionID != 0)
                {
                    System.Data.DataSet dsCRAggregationGridType = BindGridtype();
                    cboGridDefinition.DataSource = dsCRAggregationGridType.Tables[0];
                    cboGridDefinition.DisplayMember = "GridDefinitionName";

                    commandText = "select GridDefinitionName from GridDefinitions where GridDefinitionID=" + GridDefinitionID + "";
                    cboGridDefinition.Text = (fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)).ToString();
                    cboGridDefinition.Enabled = false;
                }
                else
                {
                    cboGridDefinition.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        private System.Data.DataSet BindGridtype()
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select -1 as  GridDefinitionID, '' as GridDefinitionName from GridDefinitions union select distinct GridDefinitionID,GridDefinitionName from griddefinitions where SetupID={0}  ", CommonClass.ManageSetup.SetupID);
                System.Data.DataSet dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                return dsGrid;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return null;
            }
        }
        private int gridDefinitionID;

        public int GridDefinitionID
        {
            get { return gridDefinitionID; }
            set { gridDefinitionID = value; }
        }
        private string _isForceValidate = string.Empty;
        private string _iniPath = string.Empty;
        private string _strPath;
        public string StrPath { get { return _strPath; } set { _strPath = value; } }

        private void btnOK_Click(object sender, EventArgs e)
        {
            LoadDatabase();
        }

        private void LoadDatabase()
        {
            string msg = string.Empty;
            try
            {
                if (txtDatabase.Text == string.Empty)
                {
                    msg = "Please select the file you want to load.";
                    return;
                }
                if (cboGridDefinition.Text == string.Empty)
                {
                    msg = "Please select the grid definition type.";
                    return;
                }
                _strPath = txtDatabase.Text;
                DialogResult rtn = MessageBox.Show("Do you want to load this database?", "Confirm", MessageBoxButtons.YesNo);
                if (rtn == DialogResult.Yes)
                { this.DialogResult = DialogResult.OK; }
                else { return; }
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

        private void btnValidate_Click(object sender, EventArgs e)
        {
            _incidneceData = CommonClass.ExcelToDataTable(_strPath);
            ValidateDatabaseImport vdi = new ValidateDatabaseImport(_incidneceData, "Incidence", _strPath);

            DialogResult dlgR = vdi.ShowDialog();
            if(dlgR.Equals(DialogResult.OK))
            {
                if (vdi.PassedValidation && _isForceValidate == "T")
                    LoadDatabase(); ;
            }
        }

        private void txtDatabase_TextChanged(object sender, EventArgs e)
        {
                btnValidate.Enabled = !string.IsNullOrEmpty(txtDatabase.Text);
                btnViewMetadata.Enabled = !string.IsNullOrEmpty(txtDatabase.Text);
                _strPath = txtDatabase.Text;
        }

        private void btnViewMetadata_Click(object sender, EventArgs e)
        {
            ViewEditMetadata vem = new ViewEditMetadata(_strPath);

            vem.ShowDialog();
        }
    }
}
