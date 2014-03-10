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
        public IncrementalRollback _IncrementalRollback;

        private int _regionID;
        public int RegionID
        {
            get { return _regionID; }
            set { _regionID = value; }
        }

        public IncrementalControl()
        {
            InitializeComponent();

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

        private void txtIncrement_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _IncrementalRollback.Increment = Double.Parse(txtIncrement.Text);
            }
            catch
            { txtIncrement.Text = "0.00"; }
        }

        private void txtBackground_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _IncrementalRollback.Background = Double.Parse(txtBackground.Text);
            }
            catch
            { txtBackground.Text = "0.00"; }
        }

    }
}
