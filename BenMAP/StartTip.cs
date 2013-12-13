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
using System.Reflection;

namespace BenMAP
{
    public partial class StartTip : Form
    {
        public StartTip()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.Text = "BenMAP-CE " + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);
            btn_Click(button1, null);
        }

        private bool bol1 = false, bol2 = false, bol3 = false, bol4 = false;

        private void btn_Click(object sender, EventArgs e)
        {
            switch ((sender as PictureBox).Name)
            {
                case "button1":
                    bol1 = true; bol2 = false; bol3 = false; bol4 = false;
                    button1.Image = Properties.Resources.StartTip1_3;
                    button2.Image = Properties.Resources.StartTip2_1;
                    button3.Image = Properties.Resources.StartTip3_1;
                    button4.Image = Properties.Resources.StartTip4_1;
                    break;
                case "button2":
                    bol1 = false; bol2 = true; bol3 = false; bol4 = false;
                    button1.Image = Properties.Resources.StartTip1_1;
                    button2.Image = Properties.Resources.StartTip2_3;
                    button3.Image = Properties.Resources.StartTip3_1;
                    button4.Image = Properties.Resources.StartTip4_1;
                    break;
                case "button3":
                    bol1 = false; bol2 = false; bol3 = true; bol4 = false;
                    button1.Image = Properties.Resources.StartTip1_1;
                    button2.Image = Properties.Resources.StartTip2_1;
                    button3.Image = Properties.Resources.StartTip3_3;
                    button4.Image = Properties.Resources.StartTip4_1;
                    break;
                case "button4":
                    bol1 = false; bol2 = false; bol3 = false; bol4 = true;
                    button1.Image = Properties.Resources.StartTip1_1;
                    button2.Image = Properties.Resources.StartTip2_1;
                    button3.Image = Properties.Resources.StartTip3_1;
                    button4.Image = Properties.Resources.StartTip4_3;
                    break;
            }

            string str = (sender as PictureBox).Name.Replace("button", "");
            foreach (Control cl in this.Controls)
            {
                if (cl is Panel)
                {
                    if (cl.Name.Replace("panel", "") == str)
                        cl.Visible = true;
                    else
                        cl.Visible = false;
                }
            }
        }

        #region Mouse hover & leave

        private void btn_MouseEnter(object sender, EventArgs e)
        {
            switch ((sender as PictureBox).Name)
            {
                case "button1":
                    if (!bol1)
                        button1.Image = Properties.Resources.StartTip1_2;
                    break;
                case "button2":
                    if (!bol2)
                        button2.Image = Properties.Resources.StartTip2_2;
                    break;
                case "button3":
                    if (!bol3)
                        button3.Image = Properties.Resources.StartTip3_2;
                    break;
                case "button4":
                    if (!bol4)
                        button4.Image = Properties.Resources.StartTip4_2;
                    break;
            }
        }

        private void btn_MouseLeave(object sender, EventArgs e)
        {
            switch ((sender as PictureBox).Name)
            {
                case "button1":
                    if (!bol1)
                        button1.Image = Properties.Resources.StartTip1_1;
                    break;
                case "button2":
                    if (!bol2)
                        button2.Image = Properties.Resources.StartTip2_1;
                    break;
                case "button3":
                    if (!bol3)
                        button3.Image = Properties.Resources.StartTip3_1;
                    break;
                case "button4":
                    if (!bol4)
                        button4.Image = Properties.Resources.StartTip4_1;
                    break;
            }
        }

        #endregion

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.epa.gov/air/benmap/ce.html");
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
        private void UpdateConfig(string Xvalue)
        {
            //XmlDocument doc = new XmlDocument();
            //doc.Load(Application.ExecutablePath + ".config");
            //XmlNode node = doc.SelectSingleNode(@"//add[@key='IsShow']");
            //XmlElement ele = (XmlElement)node;
            //ele.SetAttribute("value", Xvalue);
            //doc.Save(Application.ExecutablePath + ".config");
            //ConfigurationManager.RefreshSection("appSettings");
            string iniPath = CommonClass.ResultFilePath + @"\BenMAP.ini";
            if (System.IO.File.Exists(iniPath))
            {
                CommonClass.IniWriteValue("appSettings", "IsShowStart", Xvalue, iniPath);
            }
        }

        private void StartTip_Load(object sender, EventArgs e)
        {
            button1.Location = new Point(0, 90);
            button2.Location = new Point(0, button1.Location.Y + 61);
            button3.Location = new Point(0, button2.Location.Y + 61);
            button4.Location = new Point(0, button3.Location.Y + 61);
            chkCloseTip.Location = new Point(chkCloseTip.Location.X, button4.Location.Y + 72);
            btnOK.Location = new Point(btnOK.Location.X, button4.Location.Y + 72);
            linkLabel1.Location = new Point(label5.Location.X + label5.Width, label5.Location.Y);
        }

        //private void StartTip_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    try
        //    {
        //        if (chkCloseTip.Checked)
        //        {
        //            UpdateConfig("F");
        //        }
        //        else
        //        {
        //            UpdateConfig("T");
        //        }
        //        this.DialogResult = DialogResult.OK;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex.Message);
        //    }
        //}
    }
}
