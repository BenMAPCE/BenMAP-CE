using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
//TODO:
//1 on the LoadVariableDatabase dialog add a validate button
//2 make it disabled
//3 make the OK button disabled
//4 After selecting a database to load (a csv file or excel file)
//  enabled the validate button.
//5 on a positive validation enable the OK button
//
namespace BenMAP
{
    public partial class LoadVariableDatabase : FormBase
    {
        private MetadataClassObj _metadataObj = null;

        public MetadataClassObj MetadataObj
        {
            get { return _metadataObj; }
        }
        private DataTable _variableDatabase;
        public DataTable VariableDatabase
        {
            get {return _variableDatabase; }
        }
        private string _strPath;
        private string _definitionID = string.Empty;
        private string _isForceValidate = string.Empty;
        private string _iniPath = string.Empty;
        public string DefinitionID
        {
            get { return _definitionID; }
            set { _definitionID = value; }
        }
        
        public LoadVariableDatabase()
        {
            InitializeComponent();
            _iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
            _isForceValidate = CommonClass.IniReadValue("appSettings", "IsForceValidate", _iniPath);
            if (_isForceValidate == "T")
            {
                btnOK.Enabled = false;
            }
            else
            {
                btnOK.Enabled = true;
            }
        }

        private string _dataPath = string.Empty;
        public string DataPath
        {
            get { return _dataPath; }
            set { _dataPath = value; }
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
            if (cboGridDefinition.Text == string.Empty || txtDatabase.Text == string.Empty)
            {
                MessageBox.Show("Please input the 'QALY Dataset Name' and 'Database'.");
                return;
            }
            string msg = string.Empty;
            try
            {
                if (!File.Exists(txtDatabase.Text)) { msg = "Please select a valid database path. "; return; }
                _dataPath = txtDatabase.Text;
                if (cboGridDefinition.Text == string.Empty)
                {
                    msg = "Please select a grid definition from the drop-down menu."; return;
                }
                _definitionID = cboGridDefinition.Text;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void LoadVariableDatabase_Load(object sender, EventArgs e)
        {
            try
            {
                string commandText = string.Format("select * from GRIDDEFINITIONS where SetupID={0} ORDER BY GRIDDEFINITIONNAME ASC", CommonClass.ManageSetup.SetupID);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboGridDefinition.DataSource = ds.Tables[0];
                cboGridDefinition.DisplayMember = "GRIDDEFINITIONNAME";
                cboGridDefinition.Text = DefinitionID;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
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

        private void btnValidate_Click(object sender, EventArgs e)
        {
            _variableDatabase = CommonClass.ExcelToDataTable(_strPath);
            ValidateDatabaseImport vdi = new ValidateDatabaseImport(_variableDatabase, "VariableDataset", _strPath);

            DialogResult dlgR = vdi.ShowDialog();
            if (dlgR.Equals(DialogResult.OK))
            {
                if (vdi.PassedValidation && _isForceValidate == "T")
                {
                    LoadDatabase();
                }
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
