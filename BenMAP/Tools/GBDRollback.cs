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

            //parameter options
            gbOptionsPercentage.Location = new Point(gbOptionsIncremental.Location.X, gbOptionsIncremental.Location.Y);
            gbParameterSelection.Controls.Add(gbOptionsPercentage);
            gbOptionsStandard.Location = new Point(gbOptionsIncremental.Location.X, gbOptionsIncremental.Location.Y);
            gbParameterSelection.Controls.Add(gbOptionsStandard);

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSelectAndContinue_Click(object sender, EventArgs e)
        {
            gbAreaSelection.Visible = false;
            gbParameterSelection.Visible = true;
            cboRollbackType.SelectedIndex = -1;            

        }

        private void btnAreaSelection_Click(object sender, EventArgs e)
        {
            gbAreaSelection.Visible = true;
            gbParameterSelection.Visible = false;
        }

        private void cboRollbackType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cboRollbackType.SelectedIndex)
            {
                case 0:
                    gbOptionsIncremental.Visible = true;
                    gbOptionsPercentage.Visible = false;
                    gbOptionsStandard.Visible = false;
                    break;
                case 1:
                    gbOptionsIncremental.Visible = false;
                    gbOptionsPercentage.Visible = true;
                    gbOptionsStandard.Visible = false;
                    break;
                case 2:
                    gbOptionsIncremental.Visible = false;
                    gbOptionsPercentage.Visible = false;
                    gbOptionsStandard.Visible = true;
                    break;
                default:
                    gbOptionsIncremental.Visible = false;
                    gbOptionsPercentage.Visible = false;
                    gbOptionsStandard.Visible = false;
                    break;
            }

        }
    }
}
