using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BenMAP.DataSource
{
    public partial class PercentageControl : UserControl
    {
        private int _regionID;
        public int RegionID
        {
            get { return _regionID; }
            set { _regionID = value; }
        }
        public PercentageRollback _PercentageRollback;
        public PercentageControl()
        {
            InitializeComponent();
        }

        private void txtPercent_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtPercent_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _PercentageRollback.Percent = Double.Parse(txtPercent.Text);
                if (_PercentageRollback.Percent < 0.0)
                {
                    txtPercent.Text = "0.00";
                    _PercentageRollback.Percent = 0.0;
                }
                if (_PercentageRollback.Percent > 100.0)
                {
                    txtPercent.Text = "100.00";
                    _PercentageRollback.Percent = 100.0;
                }
            }
            catch
            { txtPercent.Text = "0.00"; }
        }

        private void txtBackground_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _PercentageRollback.Background = Double.Parse(txtBackground.Text);
                if (_PercentageRollback.Background < 0.0)
                {
                    txtBackground.Text = "0.00";
                    _PercentageRollback.Background = 0.0;
                }
            }
            catch
            { txtBackground.Text = "0.00"; }
        }
    }
}
