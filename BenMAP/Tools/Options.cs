using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Configuration;

namespace BenMAP
{
    public partial class Options : FormBase
    {
        public Options()
        {
            InitializeComponent();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            try
            {
                string isShow = "T";
                string isShowExit = "T";
                string defaultSetup = "United States";

                string iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
                if (System.IO.File.Exists(iniPath))
                {
                    isShow = CommonClass.IniReadValue("appSettings", "IsShowStart", iniPath);
                    isShowExit = CommonClass.IniReadValue("appSettings", "IsShowExit", iniPath);
                    defaultSetup = CommonClass.IniReadValue("appSettings", "DefaultSetup", iniPath);
                }

                if (isShow == "T")
                    cboStart.Checked = true;
                else
                    cboStart.Checked = false;

                if (isShowExit == "T")
                    cboExit.Checked = true;
                else
                    cboExit.Checked = false;

                string commandText = "select SetupID,SetupName from Setups order by SetupID";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                System.Data.DataTable dt = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText).Tables[0];
                cboDefaultSetup.DataSource = dt;
                cboDefaultSetup.DisplayMember = "SetupName";
                cboDefaultSetup.SelectedIndex = 0;
                cboDefaultSetup.Text = defaultSetup;
            }
            catch
            { }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                string iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
                if (System.IO.File.Exists(iniPath))
                {
                    CommonClass.IniWriteValue("appSettings", "IsShowStart", cboStart.Checked ? "T" : "F", iniPath);
                    CommonClass.IniWriteValue("appSettings", "IsShowExit", cboExit.Checked ? "T" : "F", iniPath);
                    CommonClass.IniWriteValue("appSettings", "DefaultSetup", cboDefaultSetup.Text, iniPath);
                }



                this.DialogResult = DialogResult.OK;
            }
            catch
            { }
        }
    }
}
