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
    public partial class Renamefile : FormBase
    {
        public Renamefile()
        {
            InitializeComponent();
        }

        public string newfileName = "";
        public string manage = "";
        public int datasetID = -1;

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtfileName.Text.Trim() == "") return;
            switch (manage)
            {
                case "GridDefinition":
                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + txtfileName.Text + ".shp"))
                    {
                        MessageBox.Show("The shapefile name already exists.");
                        return;
                    }
                    else
                    {
                        newfileName = txtfileName.Text.Trim();
                        this.DialogResult = DialogResult.OK;
                    }
                    break;
                case "Variable":
                    string commandText = string.Format("select setupvariableID from setupvariables where setupvariablename='{0}' and setupvariabledatasetID='{1}'", txtfileName.Text, datasetID);
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                    if (obj != null || newfileName.Trim() == txtfileName.Text.Trim())
                    {
                        MessageBox.Show("The variable name already exists.");
                        return;
                    }
                    else
                    {
                        newfileName = txtfileName.Text.Trim();
                        this.DialogResult = DialogResult.OK;
                    }
                    break;
            }
        }

        private void RenameShapefile_Load(object sender, EventArgs e)
        {
            txtfileName.Text = newfileName;
        }
    }
}
