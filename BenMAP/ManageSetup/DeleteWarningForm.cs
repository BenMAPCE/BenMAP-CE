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

    public partial class DeleteWarningForm : FormBase
    {

        public DeleteWarningForm(int visibleButton)
        {
            InitializeComponent();
            btnContinue.ForeColor = Color.DarkRed;
            ///btnNo.ForeColor = Color.DarkBlue;
            switch (visibleButton)
            {
                case 0:
                    btnContinue.Visible = false;
                    break;
                case 1:
                    btnCancel.Visible = false;
                    break;
                case 2:
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
            this.AcceptButton = btnContinue;
            return true;
        }

        private string _btnOneText;
        public string BTNOneText
        {
            get
            {
                _btnOneText = btnContinue.Text;
                return _btnOneText;
            }
            set
            {
                _btnOneText = value;
                btnContinue.Text = _btnOneText;
            }
        }

        private string _warningText;
        public string WarningText
        {
            get
            {
                _warningText = richTextBox1.Text;
                return _warningText;
            }
            set
            {
                _warningText = value;
                richTextBox1.Text = _warningText;
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
                _firstButtonLocation = btnContinue.Location;
                return _firstButtonLocation;
            }
            set
            {
                _firstButtonLocation = value;
                btnContinue.Location = _firstButtonLocation;
                btnContinue.AutoSize = false;
                btnContinue.Size = new Size(88, 27);
            }
        }

        public bool SetFirstButtonLocation(int x, int y)
        {
            try
            {
                lblMessage.Location = new Point(12, 29);
                btnContinue.Location = new Point(x, y);
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
                _btnSecondText = btnCancel.Text;
                return _btnSecondText;
            }
            set
            {
                _btnSecondText = value;
                btnCancel.Text = _btnSecondText;
            }
        }


        private void btnContinue_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lblMessage_Click(object sender, EventArgs e)
        {

        }
    }
}
