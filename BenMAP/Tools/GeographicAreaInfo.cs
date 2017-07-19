using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BenMAP.Tools
{
    public partial class GeographicAreaInfo : FormBase
    {
        public GeographicAreaInfo()
        {
            InitializeComponent();
        }

        private void UpdateConfig(string Xvalue)
        {
            string iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
            if (System.IO.File.Exists(iniPath))
            {
                CommonClass.IniWriteValue("appSettings", "IsShowGeographicAreaInfo", Xvalue, iniPath);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (chkCloseTip.Checked)
            {
                UpdateConfig("F");
            }
            else
            {
                UpdateConfig("T");
            }
            this.DialogResult = DialogResult.OK;
            this.Dispose();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void chkCloseTip_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
