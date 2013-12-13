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
        /// <summary>
        /// 显示提示窗口
        /// </summary>
        public TipForm()
        {
            InitializeComponent();
            //lblTip.Text = Msg;  //在label显示提示
            this.TopMost = false;   //窗口置前
            //去掉右上角最大、最小化和关闭按钮
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
           
            
        }

        /// <summary>
        /// 根据传进来的参数不同，显示不同的提示窗口
        /// </summary>
        /// <param name="tipStr"></param>
        public TipForm(string tipStr)
        {
            InitializeComponent();

            //lblTip.Text = Msg;
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
                this.Text = _msg;
                timer1 = new Timer();
                timer1.Tick += new EventHandler(timer1_Tick);

            }
        }

        private void TipForm_Load(object sender, EventArgs e)
        {
            progressBarTip.Maximum = 100;//设置最大长度值
            progressBarTip.Value = 0;//设置当前值
            progressBarTip.Step = 1;//设置没次增长多少
            timer1.Enabled = true;
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
                progressBarTip.Value += progressBarTip.Step;//让进度条增加一次
            //if (progressBarTip.Value == 100)
            //{
            //    progressBarTip.Maximum = 100;//设置最大长度值
            //    progressBarTip.Value = 0;//设置当前值
            //    progressBarTip.Step = 5;//设置没次增长多少
            //    timer1.Enabled = true;
            //    progressBarTip.Value = 0;
            //}
        }
    }
}
