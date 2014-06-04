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
    public partial class GBDRollback : Form
    {
        public GBDRollback()
        {
            InitializeComponent();

            //set up locations,form size, visibility
            gbParameterSelection.Location = new Point(gbAreaSelection.Location.X, gbAreaSelection.Location.Y);
            gbAreaSelection.Visible = true;
            gbParameterSelection.Visible = false;
            Size = new Size(906, 902); //form size
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSelectAndContinue_Click(object sender, EventArgs e)
        {
            gbAreaSelection.Visible = false;
            gbParameterSelection.Visible = true;
            

        }

        private void btnAreaSelection_Click(object sender, EventArgs e)
        {
            gbAreaSelection.Visible = true;
            gbParameterSelection.Visible = false;
        }
    }
}
