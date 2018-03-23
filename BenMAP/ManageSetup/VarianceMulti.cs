using BrightIdeasSoftware;
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
    public partial class VarianceMulti : FormBase
    {
        /* private int betaIndex;
        private CRFVariable _poll;

        public CRFVariable Pollutant
        {
            get { return _poll; }
            set { _poll = value;  }
        } */
        private CRFBeta beta;

        public CRFBeta Beta
        {
            get { return beta; }
            set { beta = value; }
        }

        public VarianceMulti(string modelSpec, string pollName, CRFBeta b)
        {
            InitializeComponent();
            beta = b;

            tbModelSpec.Text = modelSpec;
            tbPollutant.Text = pollName;
            tbSeason.Text = beta.SeasonName;
            tbStart.Text = beta.StartDate;
            tbEnd.Text = beta.EndDate;
        }

        private void VarianceMulti_Load(object sender, EventArgs e)
        {
            olvVariance.SetObjects(beta.VarCovar);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        /*
        void txt_TextChanged_VarCov(object sender, EventArgs e)
        {
            try
            {
                TextBox txt = (TextBox)sender;

                int flag = 0;
                foreach (char c in txt.Text)
                {
                    if (! (char.IsNumber(c) || c.Equals('.') || c.Equals('E') || c.Equals('e') || c.Equals('-')) )
                    {
                        MessageBox.Show("The variance/covariance must be entered in decimal format or scientific notation.");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TextBox txt = (TextBox)sender;
                Logger.LogError(ex);
            }
        }
*/
        private void olvVariance_CellEditStarting(object sender, CellEditEventArgs e)
        {
            base.OnClick(e);
            TextBox txt = new TextBox();
            txt.Bounds = e.CellBounds;
            txt.Font = ((ObjectListView)sender).Font;
            if (e.Value != null)
            {
                txt.Text = string.Format("{0:0.##E+00}", e.Value );
                //txt.TextChanged += new EventHandler(txt_TextChanged_VarCov);
                txt.Tag = e.RowObject;
                e.Control = txt;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void olvVariance_CellEditFinishing(object sender, CellEditEventArgs e)
        {
            try
            {
                TextBox txt = (TextBox)e.Control;
                //((TextBox)e.Control).TextChanged -= new EventHandler(txt_TextChanged_VarCov);

                foreach (char c in txt.Text)
                {
                    if (!(char.IsNumber(c) || c.Equals('.') || c.Equals('E') || c.Equals('e') || c.Equals('-') || c.Equals('+')))
                    {
                        MessageBox.Show("The variance/covariance must be entered in decimal format or scientific notation.");
                        e.Cancel = true;
                        return;
                    }
                }

                ((CRFVarCov)txt.Tag).VarCov = Convert.ToDouble(txt.Text);

            }
            catch
            {
                MessageBox.Show("A problem occurred converting your input to a number. The variance/covariance must be entered in decimal format or scientific notation.");
                e.Cancel = true;
            }
        }
    }
}
