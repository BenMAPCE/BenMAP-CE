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
        private string _msg = "Loading Data. Please wait.";
        /// <summary>
        /// 显示的文字提示
        /// </summary>
        public string Msg
        {
            get { return _msg; }
            set
            {
                _msg = value;
                this.lbTip.Text = _msg;
                //---------调整格式
                this.lbTip.Location =new Point( (this.Width - this.pictureBox1.Width - lbTip.Width) / 3 +this.pictureBox1.Width,this.lbTip.Location.Y);
                if (lbTip.Location.X + this.lbTip.Width > this.Width) this.Width = lbTip.Location.X + this.lbTip.Width + 10;
            }
        }

        private void TipFormGIF_Load(object sender, EventArgs e)
        {
            if (this.ParentForm == null)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(CommonClass.BenMAPForm.ParentForm.Location.X + CommonClass.BenMAPForm.ParentForm.Size.Width / 2 - this.Size.Width / 2, CommonClass.BenMAPForm.ParentForm.Location.Y + CommonClass.BenMAPForm.ParentForm.Size.Height / 2 - this.Size.Height / 2);
                
            }
            this.ControlBox = false;
        }
    }
}
