using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BenMAP.APVX
{

	public partial class MessageForm : FormBase
	{

		public MessageForm(int visibleButton)
		{
			InitializeComponent();
			btnOpenExist.ForeColor = Color.DarkRed;
			btnCreate.ForeColor = Color.DarkBlue;
			switch (visibleButton)
			{
				case 0:
					btnOpenExist.Visible = false;
					break;
				case 1:
					btnCreate.Visible = false;
					break;
				case 2:
					btnCancel.Visible = false;
					break;
			}
		}

		private string _message;
		public string Message
		{
			get
			{
				_message = lblMessage.Text;
				return _message;
			}
			set
			{
				_message = value;
				lblMessage.Text = _message;
			}
		}

		public bool SetFirstButton()
		{
			this.AcceptButton = btnOpenExist;
			return true;
		}

		private string _btnOneText;
		public string BTNOneText
		{
			get
			{
				_btnOneText = btnOpenExist.Text;
				return _btnOneText;
			}
			set
			{
				_btnOneText = value;
				btnOpenExist.Text = _btnOneText;
			}
		}

		private Point _labelLocation;
		public Point LabelLocation
		{
			get
			{
				_labelLocation = lblMessage.Location;
				return _labelLocation;
			}
			set
			{
				_labelLocation = value;
				lblMessage.Location = _labelLocation;
				lblMessage.AutoSize = false;
			}
		}
		public bool SetLabel(int x, int y)
		{
			lblMessage.Size = new System.Drawing.Size(x, y);
			return true;
		}

		private Point _firstButtonLocation;
		public Point FirstButtonLocation
		{
			get
			{
				_firstButtonLocation = btnOpenExist.Location;
				return _firstButtonLocation;
			}
			set
			{
				_firstButtonLocation = value;
				btnOpenExist.Location = _firstButtonLocation;
				btnOpenExist.AutoSize = false;
				btnOpenExist.Size = new Size(88, 27);
			}
		}

		public bool SetFirstButtonLocation(int x, int y)
		{
			try
			{
				lblMessage.Location = new Point(12, 29);
				btnOpenExist.Location = new Point(x, y);
				return true;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				return false;
			}
		}
		private string _btnSecondText;
		public string BTNSecondText
		{
			get
			{
				_btnSecondText = btnCreate.Text;
				return _btnSecondText;
			}
			set
			{
				_btnSecondText = value;
				btnCreate.Text = _btnSecondText;
			}
		}

		private string _btnThirdText;
		public string BTNThirdText
		{
			get
			{
				_btnThirdText = btnCancel.Text;
				return _btnThirdText;
			}
			set
			{
				_btnThirdText = value;
				btnCancel.Text = _btnThirdText;
			}
		}

		private void btnOpenExist_Click(object sender, EventArgs e)
		{
			try
			{
				this.DialogResult = DialogResult.Yes;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void btnCreate_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.No;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

	}
}
