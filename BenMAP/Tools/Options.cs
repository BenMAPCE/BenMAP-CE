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
                string isForceValidate = "T";
                string isShowGeographicAreaInfo = "T";
                string strNumDaysToDelete = "30";
                string defaultSetup = "United States";

                string iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
                if (System.IO.File.Exists(iniPath))
                {
                    isShow = CommonClass.IniReadValue("appSettings", "IsShowStart", iniPath);
                    isShowExit = CommonClass.IniReadValue("appSettings", "IsShowExit", iniPath);
                    isForceValidate = CommonClass.IniReadValue("appSettings", "IsForceValidate", iniPath);

                    string temp = CommonClass.IniReadValue("appSettings", "IsShowGeographicAreaInfo", iniPath);
                    if (!string.IsNullOrEmpty(temp))
                    {
                        isShowGeographicAreaInfo = temp;
                    }

                    temp = CommonClass.IniReadValue("appSettings", "NumDaysToDelete", iniPath);
                    if(!string.IsNullOrEmpty(temp))
                    {
                        strNumDaysToDelete = temp;
                    }
                    defaultSetup = CommonClass.IniReadValue("appSettings", "DefaultSetup", iniPath);
                }

                if (isShow == "T")
                {
                    cboStart.Checked = true;
                }
                else
                {
                    cboStart.Checked = false;
                }

                if (isShowExit == "T")
                {
                    cboExit.Checked = true;
                }
                else
                {
                    cboExit.Checked = false;
                }

                if (isForceValidate == "T")
                {
                    cboRequireValidation.Checked = true;
                }
                else
                {
                    cboRequireValidation.Checked = false;
                }

                if (isShowGeographicAreaInfo == "T")
                {
                    cboGeographicAreaInfo.Checked = true;
                }
                else
                {
                    cboGeographicAreaInfo.Checked = false;
                }

                txtNumDays.Text = strNumDaysToDelete;

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
                    CommonClass.IniWriteValue("appSettings", "IsForceValidate", cboRequireValidation.Checked ? "T" : "F", iniPath);
                    CommonClass.IniWriteValue("appSettings", "IsShowGeographicAreaInfo", cboGeographicAreaInfo.Checked ? "T" : "F", iniPath);
                    CommonClass.IniWriteValue("appSettings", "NumDaysToDelete", txtNumDays.Text, iniPath);
                    CommonClass.IniWriteValue("appSettings", "DefaultSetup", cboDefaultSetup.Text, iniPath);
                }


                this.DialogResult = DialogResult.OK;
            }
            catch
            { }
        }

        private void txtNumDays_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-')
            {
                e.Handled = true;
            }
        }

        private void btnDeleteNow_Click(object sender, EventArgs e)
        {
            deleteValidationLogFiles();
        }
        //this is for deleting the validation log files NOW
        private void deleteValidationLogFiles()
        {
            string validationResultsPath = CommonClass.ResultFilePath + @"\ValidationResults";
            string[] strFiles = System.IO.Directory.GetFiles(validationResultsPath, "*rtf");
            //string iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
            //int NumDaysToDelete = Convert.ToInt32( CommonClass.IniReadValue("appSettings", "NumDaysToDelete", iniPath));
            //DateTime createDate;
            System.IO.FileInfo fInfo = null;
            foreach(string s in strFiles)
            {
                fInfo = new System.IO.FileInfo(s);
                //createDate = fInfo.CreationTime;

                //if (createDate.Date < DateTime.Now.Date.Subtract(TimeSpan.FromDays(NumDaysToDelete)))
                //{
                System.IO.File.Delete(s);
                //}
            }
        }

    }
}
