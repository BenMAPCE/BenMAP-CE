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
            gbAreaSelection.Location = new Point(gbName.Location.X, gbName.Location.Y);
            gbParameterSelection.Location = new Point(gbName.Location.X, gbName.Location.Y);
            gbName.Visible = true;
            gbAreaSelection.Visible = false;
            gbParameterSelection.Visible = false;
            Size = new Size(906, 794); //form size

            //parameter options in gbParameterSelection
            gbOptionsPercentage.Location = new Point(gbOptionsIncremental.Location.X, gbOptionsIncremental.Location.Y);
            gbParameterSelection.Controls.Add(gbOptionsPercentage);
            gbOptionsStandard.Location = new Point(gbOptionsIncremental.Location.X, gbOptionsIncremental.Location.Y);
            gbParameterSelection.Controls.Add(gbOptionsStandard);            
            cboRollbackType.SelectedIndex = 0;
            gbOptionsPercentage.Visible = true;
            gbOptionsIncremental.Visible = false;
            gbOptionsStandard.Visible = false;

        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();           
        }

        


        private void cboRollbackType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cboRollbackType.SelectedIndex)
            {
                case 0:
                    gbOptionsIncremental.Visible = false;
                    gbOptionsPercentage.Visible = true;
                    gbOptionsStandard.Visible = false;
                    break;
                case 1:
                    gbOptionsIncremental.Visible = true;
                    gbOptionsPercentage.Visible = false;
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

        private void GBDRollback_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you wish to close?", "Confirm Close", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            gbName.Visible = false;
            gbAreaSelection.Visible = true;
            gbParameterSelection.Visible = false;
            
        }

        private void btnNext2_Click(object sender, EventArgs e)
        {
            gbName.Visible = false;
            gbAreaSelection.Visible = false;
            gbParameterSelection.Visible = true;
            //cboRollbackType.SelectedIndex = -1;            

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            gbName.Visible = true;
            gbAreaSelection.Visible = false;
            gbParameterSelection.Visible = false;
        }

        private void btnBack2_Click(object sender, EventArgs e)
        {
            gbName.Visible = false;
            gbAreaSelection.Visible = true;
            gbParameterSelection.Visible = false;
        }

        private void btnNext2_Click_1(object sender, EventArgs e)
        {

        }


        

       
    }
}
