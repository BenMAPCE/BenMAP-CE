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
        /// <summary>
        /// 提示信息
        /// </summary>
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
        }// Message


        public bool SetFirstButton()
        {
            this.AcceptButton = btnOpenExist;
            return true;
        }// FirstButton


        private string _btnOneText;
        /// <summary>
        /// 第一个Button的Text内容
        /// </summary>
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
                //this.AcceptButton = btnOpenExist;
            }
        }// BTNOneText


        private Point _labelLocation;
        /// <summary>
        /// 设置Lable的位置
        /// </summary>
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
        }// LabelLocation

        public bool SetLabel(int x, int y)
        {// 679, 31
            lblMessage.Size = new System.Drawing.Size(x, y);
            return true;
        }

        private Point _firstButtonLocation;
        /// <summary>
        /// 控制第一个按钮的位置
        /// </summary>
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
        }// var


        public bool SetFirstButtonLocation(int x, int y)
        {
            try
            {
                //Coordinates
                lblMessage.Location = new Point(12, 29);
                btnOpenExist.Location = new Point(x, y);
                //btnOpenExist.Location= pt;
                //btnOpenExist.Location.Y = pt.Y;
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }
        private string _btnSecondText;
        /// <summary>
        /// 第二个Buntton的Text内容
        /// </summary>
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
        }// BTNSecondText


        private string _btnThirdText;
        /// <summary>
        /// 第三个Button的text内容
        /// </summary>
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
        }// BTNThirdText


    }
}
