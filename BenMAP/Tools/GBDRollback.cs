using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;



namespace BenMAP
{
    public partial class GBDRollback : Form
    {

        List<String> checkedCountries = new List<String>();
        List<GBDRollbackItem> rollbacks = new List<GBDRollbackItem>();
        System.Data.DataTable dtCountries;


        public GBDRollback()
        {
            InitializeComponent();

            //set up locations,form size, visibility
            gbCountrySelection.Location = new System.Drawing.Point(gbName.Location.X, gbName.Location.Y);
            gbParameterSelection.Location = new System.Drawing.Point(gbName.Location.X, gbName.Location.Y);
            SetActivePanel(0);
            Size = new Size(906, 777); //form size

            //parameter options in gbParameterSelection
            gbOptionsPercentage.Location = new System.Drawing.Point(gbOptionsIncremental.Location.X, gbOptionsIncremental.Location.Y);
            gbParameterSelection.Controls.Add(gbOptionsPercentage);
            gbOptionsStandard.Location = new System.Drawing.Point(gbOptionsIncremental.Location.X, gbOptionsIncremental.Location.Y);
            gbParameterSelection.Controls.Add(gbOptionsStandard);            
            cboRollbackType.SelectedIndex = 0;
            SetActiveOptionsPanel(0);

            LoadCountries();
            LoadTreeView();

        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();           
        }

        private void LoadCountries()
        {
            DataSet ds = GBDRollbackDataSource.GetRegionCountryList();
            dtCountries = ds.Tables[0].Copy();//new DataTable();
        }

        private void LoadTreeView()
        {
            if (dtCountries != null)
            {
                string region = String.Empty;
                string country = String.Empty;
                tvCountries.BeginUpdate();
                foreach (DataRow dr in dtCountries.Rows)
                {
                    //new region?
                    if (!region.Equals(dr["REGION"].ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        region = dr["REGION"].ToString();
                        tvCountries.Nodes.Add(region, region);
                    }

                    country = dr["COUNTRYNAME"].ToString();
                    tvCountries.Nodes[region].Nodes.Add(country, country);
                }
                tvCountries.EndUpdate();
            }
        
        }

        


        private void cboRollbackType_SelectedIndexChanged(object sender, EventArgs e)
        {

            SetActiveOptionsPanel(cboRollbackType.SelectedIndex);
            switch (cboRollbackType.SelectedIndex)
            {
                case 0:
                   
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
            if (rollbacks.Exists(x => x.Name.Equals(txtName.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                DialogResult result = MessageBox.Show("A rollback with the name " + txtName.Text.Trim() + " already exists.  Do you wish to overwrite it?","", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    txtName.Focus();
                    return;
                }            
            }

            SetActivePanel(1);
            
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

            SetActivePanel(2);
            //cboRollbackType.SelectedIndex = -1;     
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            SetActivePanel(0);
        }

        private void btnBack2_Click(object sender, EventArgs e)
        {
            SetActivePanel(1);
        }

        private void tvCountries_AfterCheck(object sender, TreeViewEventArgs e)
        {
            CheckChildNodes(e.Node);

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
                checkedCountries.RemoveAll(x => x.Equals(e.Node.Text,StringComparison.OrdinalIgnoreCase));               
            }
        }

        private void CheckChildNodes(TreeNode node)
        {

            //this will set child nodes, if any, to 
            //same status as parent, checked or unchecked
            tvCountries.BeginUpdate();
            foreach (TreeNode item in node.Nodes)
            {
                item.Checked = node.Checked;

                if (item.Nodes.Count > 0)
                {
                    this.CheckChildNodes(item);
                }
            }
            tvCountries.EndUpdate();
        }



        private void btnSaveRollback_Click(object sender, EventArgs e)
        {
            double d;

            //clean text boxes for numerics
            txtPercentage.Text = txtPercentage.Text.Trim();
            txtPercentageBackground.Text = txtPercentageBackground.Text.Trim();
            txtIncrement.Text = txtIncrement.Text.Trim();
            txtIncrementBackground.Text = txtIncrementBackground.Text.Trim();

            switch (cboRollbackType.SelectedIndex)
            {
                case 0: //percentage
                    if (String.IsNullOrEmpty(txtPercentage.Text))
                    {
                        MessageBox.Show("Percentage is required.");
                        txtPercentage.Focus();
                        return;
                    }
                    if (!Double.TryParse(txtPercentage.Text, out d))
                    {
                        MessageBox.Show("Percentage must be numeric.");
                        txtPercentage.Focus();
                        return;                        
                    }
                    if (!String.IsNullOrEmpty(txtPercentageBackground.Text))
                    {
                        if (!Double.TryParse(txtPercentageBackground.Text, out d))
                        {
                            MessageBox.Show("Background must be numeric.");
                            txtPercentageBackground.Focus();
                            return;
                        }
                    }
                    break;
                case 1: //incremental
                    if (String.IsNullOrEmpty(txtIncrement.Text))
                    {
                        MessageBox.Show("Increment is required.");
                        txtIncrement.Focus();
                        return;
                    }
                    if (!Double.TryParse(txtIncrement.Text, out d))
                    {
                        MessageBox.Show("Increment must be numeric.");
                        txtIncrement.Focus();
                        return;
                    }
                    if (!String.IsNullOrEmpty(txtIncrementBackground.Text))
                    {
                        if (!Double.TryParse(txtIncrementBackground.Text, out d))
                        {
                            MessageBox.Show("Background must be numeric.");
                            txtIncrementBackground.Focus();
                            return;
                        }
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
            rollback.Name = txtName.Text;
            rollback.Description = txtDescription.Text;
            rollback.Countries = new List<string>(checkedCountries);
            switch (cboRollbackType.SelectedIndex)
            {
                case 0: //percentage
                    rollback.Type = GBDRollbackItem.RollbackType.Percentage;
                    rollback.Percentage = Double.Parse(txtPercentage.Text);
                    if (!String.IsNullOrEmpty(txtPercentageBackground.Text))
                    {
                        rollback.Background = Double.Parse(txtPercentageBackground.Text);
                    }
                    break;
                case 1: //incremental
                    rollback.Type = GBDRollbackItem.RollbackType.Incremental;
                    rollback.Increment = Double.Parse(txtIncrement.Text);
                    if (!String.IsNullOrEmpty(txtIncrementBackground.Text))
                    {
                        rollback.Background = Double.Parse(txtIncrementBackground.Text);
                    }
                    break;
                case 2: //standard
                    rollback.Type = GBDRollbackItem.RollbackType.Standard;
                    rollback.Standard = (GBDRollbackItem.StandardType)cboStandard.SelectedIndex;
                    break;
            }

            //remove rollback if it already exists
            rollbacks.RemoveAll(x => x.Name.Equals(rollback.Name, StringComparison.OrdinalIgnoreCase));

            //add to rollbacks
            rollbacks.Add(rollback);

            //add to grid
            dgvRollbacks.Rows.Clear();
            foreach (GBDRollbackItem item in rollbacks)
            { 
                DataGridViewRow row = new DataGridViewRow();
                int i = dgvRollbacks.Rows.Add(row);
                dgvRollbacks.Rows[i].Cells["colName"].Value = item.Name;
                dgvRollbacks.Rows[i].Cells["colColor"].Style.BackColor = item.Color;
                item.Countries.Sort();
                dgvRollbacks.Rows[i].Cells["colRollbackType"].Value = GetRollbackTypeSummary(item);         
            }

            ClearFields();
            SetActivePanel(0);
           
        }

        private string GetRollbackTypeSummary(GBDRollbackItem rollback)
        {
            string summary = String.Empty;

            switch (rollback.Type)
            {
                case GBDRollbackItem.RollbackType.Percentage: //percentage
                    summary = rollback.Percentage.ToString() + "% Rollback";
                    break;
                case GBDRollbackItem.RollbackType.Incremental: //incremental
                    char micrograms = '\u00B5';
                    char super3 = '\u00B3';
                    summary = rollback.Increment.ToString() + micrograms.ToString() + "g/m" + super3.ToString() + " Rollback";
                    break;
                case GBDRollbackItem.RollbackType.Standard:
                    summary = "Rollback to " + rollback.Standard.ToString() + " Standard";
                    break;
            }


            return summary;
        }

        private void ClearFields() 
        {
            //clear fields
            txtName.Text = String.Empty;
            txtDescription.Text = String.Empty;
            foreach (TreeNode node in tvCountries.Nodes)
            {
                node.Checked = false;
            }
            cboRollbackType.SelectedIndex = (int)GBDRollbackItem.RollbackType.Percentage; 
            txtPercentage.Text = String.Empty;
            txtPercentageBackground.Text = String.Empty;
            txtIncrement.Text = String.Empty;
            txtIncrementBackground.Text = String.Empty;
            cboStandard.SelectedIndex = -1;       

        }

        private void LoadRollback(GBDRollbackItem item)
        {            
            txtName.Text = item.Name;
            txtDescription.Text = item.Description;
            foreach (string country in item.Countries)
            {
                TreeNode[] nodes = tvCountries.Nodes.Find(country,true);
                foreach (TreeNode node in nodes)
                {
                    node.Checked = true;
                }
            }
            cboRollbackType.SelectedIndex = (int)item.Type;
            txtPercentage.Text = item.Percentage.ToString();
            txtPercentageBackground.Text = item.Background.ToString();
            txtIncrement.Text = item.Increment.ToString();
            txtIncrementBackground.Text = item.Background.ToString();
            cboStandard.SelectedIndex = (int)item.Standard;

        }

        private void SetActivePanel(int index)
        {
            switch (index)
            {
                case 0:
                    gbName.Visible = true;
                    gbCountrySelection.Visible = false;
                    gbParameterSelection.Visible = false;
                    break;
                case 1:
                    gbName.Visible = false;
                    gbCountrySelection.Visible = true;
                    gbParameterSelection.Visible = false;
                    break;
                case 2:
                    gbName.Visible = false;
                    gbCountrySelection.Visible = false;
                    gbParameterSelection.Visible = true;
                    break;
            }
        }

        private void SetActiveOptionsPanel(int index)
        {
            switch (index)
            {
                case 0:
                    gbOptionsPercentage.Visible = true;
                    gbOptionsIncremental.Visible = false;                    
                    gbOptionsStandard.Visible = false;
                    break;
                case 1:
                    gbOptionsPercentage.Visible = false;
                    gbOptionsIncremental.Visible = true;                    
                    gbOptionsStandard.Visible = false;
                    break;
                case 2:
                    gbOptionsPercentage.Visible = false;
                    gbOptionsIncremental.Visible = false;                    
                    gbOptionsStandard.Visible = true;
                    break;
            }
        }


        private void btnDeleteRollback_Click(object sender, EventArgs e)
        {
            if (dgvRollbacks.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you wish to delete the selected scenario?","", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    DataGridViewRow row = dgvRollbacks.SelectedRows[0];
                    string name = row.Cells["colName"].Value.ToString();
                    //delete rollback
                    rollbacks.RemoveAll(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    //delete row
                    dgvRollbacks.Rows.Remove(row);
                }
            
            }
        }

        private void btnEditRollback_Click(object sender, EventArgs e)
        {
            if (dgvRollbacks.SelectedRows.Count > 0)
            { 
                DataGridViewRow row = dgvRollbacks.SelectedRows[0];
                string name = row.Cells["colName"].Value.ToString();
                //get rollback
                GBDRollbackItem item = rollbacks.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                ClearFields();
                LoadRollback(item);
                SetActivePanel(0);
            
            }

        }

        private void dgvRollbacks_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex != -1) && (e.ColumnIndex != -1))
            {
                string columnName = dgvRollbacks.Columns[e.ColumnIndex].Name;
                
                if ((columnName.Equals("colTotalCountries", StringComparison.OrdinalIgnoreCase)) ||
                    (columnName.Equals("colTotalPopulation", StringComparison.OrdinalIgnoreCase)))
                {
                    string name = dgvRollbacks.Rows[e.RowIndex].Cells["colName"].Value.ToString();
                    //get rollback
                    GBDRollbackItem item = rollbacks.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    GBDRollbackCountriesPopulations frm = new GBDRollbackCountriesPopulations();

                    //build selected list of countries, pops
                    string[] countries = item.Countries.ToArray();
                    for (int i = 0; i < countries.Length; i++)
                    {
                        countries[i] = "'" + countries[i] + "'";
                    }
                    string expression = "COUNTRYNAME in (" + String.Join(",", countries) +")";
                    DataRow[] rows = dtCountries.Select(expression);
                    System.Data.DataTable dt = rows.CopyToDataTable<DataRow>();
                    frm.CountryPop = dt.Copy();
                    frm.ShowDialog();
                }               
            }

        }

        private void btnExecuteRollbacks_Click(object sender, EventArgs e)
        {

            foreach (GBDRollbackItem rollback in rollbacks)
            {

                //save report
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
                Microsoft.Office.Interop.Excel.Workbook xlBook = xlApp.Workbooks.Add();
                //get timestamp
                string timeStamp = DateTime.Now.ToString("yyyyMMddHHmm");
                //get application path
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = appPath + "GBDRollback_" + rollback.Name + "_" + timeStamp + ".xlsx";
                xlBook.SaveAs(filePath,
                              FileFormat: XlFileFormat.xlOpenXMLWorkbook, 
                              AccessMode:XlSaveAsAccessMode.xlShared);
                xlBook.Close();
                xlApp.Quit();

            }

        }      



       
    }
}
