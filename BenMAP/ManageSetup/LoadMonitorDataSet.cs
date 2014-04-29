using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//LoadMonitorDataSet files should have been named LoadSelectedDataset.  Hind site is 20/20
//this is being used for any of the datasets that did not have a load dialog.  It was
//implamented to make most all manageing datasets simaler in the way they worked and
//to make the validation process as simaler as posible.
namespace BenMAP
{
    public partial class LoadSelectedDataSet : Form
    {
        private string _title = string.Empty;
        private string _datasetnamelabel = string.Empty;
        private string _datasetName = string.Empty;
        private string _dataset = string.Empty;
        private string _strPath;
        private string _isForceValidate = string.Empty;
        private string _iniPath = string.Empty;
        private DataTable _monitorDataset;
        private MetadataClassObj _metadataObj = null;

        internal MetadataClassObj MetadataObj
        {
            get { return _metadataObj; }
        }
        public DataTable MonitorDataSet
        {
            get { return _monitorDataset; }
        }
        public string DatasetName
        {
            get{return _datasetName;}
        }
        public string DatasetNameLabel
        {
            get{ return _datasetnamelabel;}
        }
        public string Title
        {
            get{ return _title;}
        }
        public string StrPath
        {
            get { return _strPath; }
        }
        
        public LoadSelectedDataSet()
        {
            InitializeComponent();
            _iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
            _isForceValidate = CommonClass.IniReadValue("appSettings", "IsForceValidate", _iniPath);
            if(_isForceValidate == "T")
            {
                btnOK.Enabled = false;
            }
            else
            {
                btnOK.Enabled = true;
            }
                
        }
        public LoadSelectedDataSet(string title, string datasetNamelabel, string datasetName, string dataset):this()
        {
            _title = title;
            _datasetnamelabel = datasetNamelabel;
            _datasetName = datasetName;
            _dataset = dataset;
            this.Text= title;
            this.lblDataSetName.Text = datasetNamelabel;
            this.txtDataSetName.Text = datasetName;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(_strPath))
            {
                _monitorDataset = CommonClass.ExcelToDataTable(_strPath);
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Please select a file");
                btnBrowse.Focus();
                return;
            }
        }
        private void GetMetadata()
        {
            _metadataObj = new MetadataClassObj();
            Metadata metadata = new Metadata(_strPath);
            _metadataObj = metadata.GetMetadata();
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            _monitorDataset = CommonClass.ExcelToDataTable(_strPath);

            ValidateDatabaseImport vdi = new ValidateDatabaseImport(_monitorDataset, _dataset, _strPath);//  (_monitorDataset, "Monitor", _strPath);

            DialogResult dlgR = vdi.ShowDialog();
            if (dlgR.Equals(DialogResult.OK))
            {
                if(vdi.PassedValidation && _isForceValidate == "T")
                    this.DialogResult = DialogResult.OK;

            }
        }

        private void txtDatabase_TextChanged(object sender, EventArgs e)
        {
            btnValidate.Enabled = !string.IsNullOrEmpty(txtDatabase.Text);
            btnViewMetadata.Enabled = !string.IsNullOrEmpty(txtDatabase.Text);
            _strPath = txtDatabase.Text;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog() { RestoreDirectory = true };
                openFileDialog.InitialDirectory = CommonClass.ResultFilePath;
                openFileDialog.Filter = "All Files|*.*|CSV files|*.csv|XLS files|*.xls|XLSX files|*.xlsx";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                { return; }
                txtDatabase.Text = openFileDialog.FileName;
                GetMetadata();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnViewMetadata_Click(object sender, EventArgs e)
        {
            ViewEditMetadata viewEMdata = null;
            //_metadataObj.SetupName = txtDataSetName.Text;
            if(_metadataObj != null)
            {
                viewEMdata = new ViewEditMetadata(_strPath, _metadataObj);
            }
            else
            {
                viewEMdata = new ViewEditMetadata(_strPath);
            }
            DialogResult dr = viewEMdata.ShowDialog();
            if(dr.Equals(DialogResult.OK))
            {
                _metadataObj = viewEMdata.MetadataObj;
            }
        }
    }
}
