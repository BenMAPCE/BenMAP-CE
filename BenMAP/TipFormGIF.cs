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
	public partial class TipFormGIF : FormBase
	{
		public TipFormGIF()
		{
			InitializeComponent();
		}

		//BenMAP-499: Added properties to class instead of within BenMAP and Main classes
		public bool sFlog;

		private delegate void CloseFormDelegate();

		private string _msg = "Loading Data. Please wait.";
		public string Msg
		{
			get { return _msg; }
			set
			{
				_msg = value;
				this.lbTip.Text = _msg;
				this.lbTip.Location = new Point((this.Width - this.pictureBox1.Width - lbTip.Width) / 3 + this.pictureBox1.Width, this.lbTip.Location.Y);
				if (lbTip.Location.X + this.lbTip.Width > this.Width) this.Width = lbTip.Location.X + this.lbTip.Width + 10;
			}
		}

		private void TipFormGIF_Load(object sender, EventArgs e)
		{
			if (this.ParentForm == null)
			{
				this.StartPosition = FormStartPosition.Manual;
				this.Location = new Point(CommonClass.BenMAPForm.ParentForm.Location.X + CommonClass.BenMAPForm.ParentForm.Size.Width / 2 - this.Size.Width / 2, CommonClass.BenMAPForm.ParentForm.Location.Y + CommonClass.BenMAPForm.ParentForm.Size.Height / 2 - this.Size.Height / 2);
				if (this.Top > 130)
				{
					this.Top = this.Top - 130;
				}
			}
			this.ControlBox = false;
		}
		//BenMAP-499: Added methods to the class instead of within the BenMAP and Main classes
		public void ShowWaitMess()
		{
			try
			{ 
				if (!this.IsDisposed)
				{
					this.ShowDialog();
				}
			}
			catch (System.Threading.ThreadAbortException Err)
			{
				MessageBox.Show(Err.Message);
			}
		}

		public void WaitShow(string msg)
		{
			try
			{
				if (sFlog == true)
				{
					sFlog = false;
					Msg = msg;
					System.Threading.Thread upgradeThread = null;
					upgradeThread = new System.Threading.Thread(() => ShowWaitMess());
					upgradeThread.Start();
				}
			}
			catch (System.Threading.ThreadAbortException Err)
			{
				MessageBox.Show(Err.Message);
			}
		}

		public void WaitClose()
		{
			if (this.InvokeRequired)
				this.Invoke(new CloseFormDelegate(DoCloseJob));
			else
				DoCloseJob();
		}

		private void DoCloseJob()
		{
			try
			{
				if (!this.IsDisposed)
				{
					if (this.Created)
					{
						sFlog = true;
						this.Close();
					}
				}
			}
			catch (System.Threading.ThreadAbortException Err)
			{
				MessageBox.Show(Err.Message);
			}
		}
	}
}
