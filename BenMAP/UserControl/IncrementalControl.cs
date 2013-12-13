using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace BenMAP.DataSource
{
    public partial class IncrementalControl : UserControl
    {
        #region Parameters
        ///// <summary>
        ///// incremental rollback参数Increment
        ///// </summary>
        //private double _increment;
        ///// <summary>
        ///// incremental rollback参数Increment
        ///// </summary>
        //public double Increment
        //{
        //    get { return _increment; }
        //    set { _increment = value; }
        //}

        ///// <summary>
        ///// incremental rollback参数Background
        ///// </summary>
        //private double _background;
        ///// <summary>
        ///// incremental rollback参数Background
        ///// </summary>
        //public double Background
        //{
        //    get { return _background; }
        //    set { _background = value; }
        //}

        ///// <summary>
        ///// incremental rollback参数Color
        ///// </summary>
        //private Color _color;
        ///// <summary>
        ///// incremental rollback参数Color
        ///// </summary>
        //public Color Color
        //{
        //    get { return _color; }
        //    set { _color = value; }
        //}

        //private string _regionName;
        ///// <summary>
        ///// incremental rollback参数RegionName
        ///// 和control.name相同
        ///// </summary>
        //public string RegionName
        //{
        //    get { return _regionName; }
        //    set { _regionName = value; }
        //}
        public IncrementalRollback _IncrementalRollback;

        private int _regionID;
        /// <summary>
        /// 控件的RegionID
        /// </summary>
        public int RegionID
        {
            get { return _regionID; }
            set { _regionID = value; }
        }
        #endregion

        public IncrementalControl()
        {
            InitializeComponent();

            ////获取当前选中的区域
            //Regex reg = new Regex(@"\D");      //找到所有非数字
            //string s = reg.Replace(_regionName.ToString(), "");    //把所有非数字替换成空
            //_regionID = int.Parse(s);
        }

        private void txtIncrement_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46 || keyValue == 45)
            {
                if (e.KeyChar == 45 && (((TextBox)sender).SelectionStart == 0 && ((TextBox)sender).Text.IndexOf("-") >= 0))
                    e.Handled = true;
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtBackground_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46 || keyValue == 45)
            {
                if (e.KeyChar == 45 && (((TextBox)sender).SelectionStart == 0 && ((TextBox)sender).Text.IndexOf("-") >= 0))
                    e.Handled = true;
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        /// <summary>
        /// 当Increment的text改变时,改变控件的Increment属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtIncrement_TextChanged(object sender, EventArgs e)
        {
            //this.Increment = Double.Parse(txtIncrement.Text);
            try
            {
                _IncrementalRollback.Increment = Double.Parse(txtIncrement.Text);
            }
            catch
            { txtIncrement.Text = "0.00"; }
        }

        /// <summary>
        /// 当Background的text改变时,改变控件的Background属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBackground_TextChanged(object sender, EventArgs e)
        {
            //this.Background = Double.Parse(txtBackground.Text);
            try
            {
                _IncrementalRollback.Background = Double.Parse(txtBackground.Text);
            }
            catch
            { txtBackground.Text = "0.00"; }
        }

    }
}
