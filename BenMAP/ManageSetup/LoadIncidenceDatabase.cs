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
    public partial class LoadIncidenceDatabase : FormBase
    {
        public LoadIncidenceDatabase()
        {
            InitializeComponent();
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

        private string _strPath;
        public string StrPath { get { return _strPath; } set { _strPath = value; } }

        private void btnOK_Click(object sender, EventArgs e)
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

    }
}
