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
        public VarianceMulti(string modelSpec, string poll)
        {
            InitializeComponent();
            tbModelSpec.Text = modelSpec;
            tbPollutant.Text = poll;
        }

        private void VarianceMulti_Load(object sender, EventArgs e)
        {

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
