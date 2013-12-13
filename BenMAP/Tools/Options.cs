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

        //private void UpdateConfig(string appSettings, string Xvalue)
        //{
        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(Application.ExecutablePath + ".config");
        //    XmlNode node = doc.SelectSingleNode(@"//add[@key='" + appSettings + "']");
        //    XmlElement ele = (XmlElement)node;
        //    ele.SetAttribute("value", Xvalue);
        //    doc.Save(Application.ExecutablePath + ".config");
        //    ConfigurationManager.RefreshSection("appSettings");
        //}

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

                //XmlDocument doc = new XmlDocument();
                //doc.Load(Application.ExecutablePath + ".config");
                //XmlNode node = doc.SelectSingleNode(@"//add[@key='IsShow']");
                //XmlElement ele = (XmlElement)node;
                if (isShow == "T")
                    cboStart.Checked = true;
                else
                    cboStart.Checked = false;

                //node = doc.SelectSingleNode(@"//add[@key='IsShowExit']");
                //ele = (XmlElement)node;
                if (isShowExit == "T")
                    cboExit.Checked = true;
                else
                    cboExit.Checked = false;

                //node = doc.SelectSingleNode(@"//add[@key='DefaultSetup']");
                //ele = (XmlElement)node;
                //string defaultSetup = ele.GetAttribute("value");
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

                //if (cboStart.Checked)
                //    UpdateConfig("IsShow", "T");
                //else
                //    UpdateConfig("IsShow", "F");

                //if (cboExit.Checked)
                //    UpdateConfig("IsShowExit", "T");
                //else
                //    UpdateConfig("IsShowExit", "F");

                //UpdateConfig("DefaultSetup", cboDefaultSetup.Text);
                this.DialogResult = DialogResult.OK;
            }
            catch
            { }
        }
    }
}
