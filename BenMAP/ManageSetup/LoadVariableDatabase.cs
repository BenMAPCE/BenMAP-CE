using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace BenMAP
{
    public partial class LoadVariableDatabase : FormBase
    {
        public LoadVariableDatabase()
        {
            InitializeComponent();
        }

        private string _definitionID = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string DefinitionID
        {
            get { return _definitionID; }
            set { _definitionID = value; }
        }

        private string _dataPath = string.Empty;
        /// <summary>
        /// 返回路径
        /// </summary>
        public string DataPath
        { 
            get { return _dataPath; } 
            set { _dataPath = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
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
                string commandText = string.Format("select * from GRIDDEFINITIONS where SetupID={0} ORDER BY GRIDDEFINITIONNAME ASC",CommonClass.ManageSetup.SetupID);
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
    }
}
