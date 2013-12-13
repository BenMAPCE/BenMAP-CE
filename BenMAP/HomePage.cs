using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace BenMAP
{
    public partial class HomePage : Form
    {
        public HomePage()
        {
            InitializeComponent();
            lblversion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 4);
            this.Width = 800;
            Height = 600;
        }
        public string sPicName;
        private void picCreateProject_Click(object sender, EventArgs e)
        {
            sPicName = (sender as PictureBox).Name;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;

        }

        private void picUserGuide_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, Application.StartupPath + @"\Data\QuickStartGuide.chm");
        }

        private void picExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;   
        private void HomePage_MouseMove(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }
    }
}
