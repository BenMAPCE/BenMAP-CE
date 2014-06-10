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

        List<String> checkedCountries = new List<String>();
        List<GBDRollbackItem> rollbacks = new List<GBDRollbackItem>();


        public GBDRollback()
        {
            InitializeComponent();

            //set up locations,form size, visibility
            gbCountrySelection.Location = new Point(gbName.Location.X, gbName.Location.Y);
            gbParameterSelection.Location = new Point(gbName.Location.X, gbName.Location.Y);
            gbName.Visible = true;
            gbCountrySelection.Visible = false;
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

            LoadCountryTreeView();

        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();           
        }

        private void LoadCountryTreeView()
        {
            tvCountries.BeginUpdate();
            tvCountries.Nodes.Add("North America", "North America");
            tvCountries.Nodes["North America"].Nodes.Add("United States", "United States");
            tvCountries.Nodes["North America"].Nodes.Add("Canada", "Canada");
            tvCountries.Nodes["North America"].Nodes.Add("Mexico", "Mexico");
            tvCountries.EndUpdate();
        
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
            if (String.IsNullOrEmpty(txtName.Text.Trim()))
            {
                MessageBox.Show("Name is required.");
                txtName.Focus();
                return;
            }


            gbName.Visible = false;
            gbCountrySelection.Visible = true;
            gbParameterSelection.Visible = false;
            
        }

        private void btnNext2_Click(object sender, EventArgs e)
        {
            //check for country
            if (checkedCountries.Count == 0)
            {
                MessageBox.Show("You must select at least one country.");
                tvCountries.Focus();
                return;
            }

            gbName.Visible = false;
            gbCountrySelection.Visible = false;
            gbParameterSelection.Visible = true;
            //cboRollbackType.SelectedIndex = -1;     
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            gbName.Visible = true;
            gbCountrySelection.Visible = false;
            gbParameterSelection.Visible = false;
        }

        private void btnBack2_Click(object sender, EventArgs e)
        {
            gbName.Visible = false;
            gbCountrySelection.Visible = true;
            gbParameterSelection.Visible = false;
        }

        private void tvCountries_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckTreeViewNode(e.Node);

            //if this is checked AND a has no children)
            //then, it is country and we add to list
            if ((e.Node.Checked) && (e.Node.Nodes.Count == 0))
            {
                if (!checkedCountries.Exists(s => s.Equals(e.Node.Text, StringComparison.OrdinalIgnoreCase)))
                {
                    checkedCountries.Add(e.Node.Text);
                }
            }
            else
            {
                checkedCountries.Remove(e.Node.Text);               
            }
        }

        private void CheckTreeViewNode(TreeNode node)
        {
            tvCountries.BeginUpdate();
            foreach (TreeNode item in node.Nodes)
            {
                item.Checked = node.Checked;

                if (item.Nodes.Count > 0)
                {
                    this.CheckTreeViewNode(item);
                }
            }
            tvCountries.EndUpdate();
        }

        private void btnSaveRollback_Click(object sender, EventArgs e)
        {

            switch (cboRollbackType.SelectedIndex)
            {
                case 0: //percentage
                    if (String.IsNullOrEmpty(txtPercentage.Text.Trim()))
                    {
                        MessageBox.Show("Percentage is required.");
                        txtPercentage.Focus();
                        return;
                    }
                    break;
                case 1: //incremental
                    if (String.IsNullOrEmpty(txtIncrement.Text.Trim()))
                    {
                        MessageBox.Show("Increment is required.");
                        txtIncrement.Focus();
                        return;
                    }
                    break;
                case 2: //standard
                    if (cboStandard.SelectedIndex < 0)
                    {
                        MessageBox.Show("Standard is required.");
                        cboStandard.Focus();
                        return;
                    }
                    break;
            }

            GBDRollbackItem rollback = new GBDRollbackItem();
            rollback.Name = txtName.Text.Trim();
            rollback.Description = txtDescription.Text.Trim();
            rollback.Countries = checkedCountries;
            switch (cboRollbackType.SelectedIndex)
            {
                case 0: //percentage
                    rollback.Type = GBDRollbackItem.RollbackType.PERCENTAGE;
                    rollback.Percentage = Double.Parse(txtPercentage.Text.Trim());
                    rollback.Background = Double.Parse(txtPercentageBackground.Text.Trim());
                    break;
                case 1: //incremental
                    rollback.Type = GBDRollbackItem.RollbackType.INCREMENTAL;
                    rollback.Increment = Double.Parse(txtIncrement.Text.Trim());
                    rollback.Background = Double.Parse(txtIncrementBackground.Text.Trim());
                    break;
                case 2: //standard
                    rollback.Type = GBDRollbackItem.RollbackType.STANDARD;
                    rollback.Standard = (GBDRollbackItem.StandardType)cboStandard.SelectedIndex;
                    break;
            }

            rollbacks.Add(rollback);

            


           
        }



       
    }
}
