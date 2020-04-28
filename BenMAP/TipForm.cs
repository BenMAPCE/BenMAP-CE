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
	public partial class TipForm : FormBase
	{
		public TipForm()
		{
			InitializeComponent();
			this.TopMost = false; this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ControlBox = false;
		}

		public TipForm(string tipStr)
		{
			InitializeComponent();
		}

		private string _msg = "Loading Data. Please wait.";
		public string Msg
		{
			get { return _msg; }
			set
			{
				_msg = value;
				this.Text = _msg;
				timer1 = new Timer();
				timer1.Tick += new EventHandler(timer1_Tick);

			}
		}

		private void TipForm_Load(object sender, EventArgs e)
		{
			progressBarTip.Maximum = 100; progressBarTip.Value = 0; progressBarTip.Step = 1; timer1.Enabled = true;
			if (this.ParentForm == null)
			{
				this.StartPosition = FormStartPosition.Manual;
				this.Location = new Point(CommonClass.BenMAPForm.ParentForm.Location.X + CommonClass.BenMAPForm.ParentForm.Size.Width / 2 - this.Size.Width / 2, CommonClass.BenMAPForm.ParentForm.Location.Y + CommonClass.BenMAPForm.ParentForm.Size.Height / 2 - this.Size.Height / 2);
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (progressBarTip.Value == 100)
			{
				progressBarTip.Value = 0;
			}
			else
				progressBarTip.Value += progressBarTip.Step;
		}
	}
}
