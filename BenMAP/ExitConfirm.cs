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
	public partial class ExitConfirm : FormBase
	{
		public ExitConfirm()
		{
			InitializeComponent();
		}

		private void UpdateConfig(string Xvalue)
		{
			string iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
			if (System.IO.File.Exists(iniPath))
			{
				CommonClass.IniWriteValue("appSettings", "IsShowExit", Xvalue, iniPath);
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

		private void btnNO_Click(object sender, EventArgs e)
		{
			if (chkCloseTip.Checked)
			{
				UpdateConfig("F");
			}
			else
			{
				UpdateConfig("T");
			}
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}
