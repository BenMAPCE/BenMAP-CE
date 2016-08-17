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
    }
}
