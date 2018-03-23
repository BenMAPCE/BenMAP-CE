using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;

namespace BenMAP
{
    public partial class ManagePollutants : FormBase
    {

        bool _showGroups = false;

        public ManagePollutants()
        {
            InitializeComponent();
        }

        private void ManagePollutants_Load(object sender, EventArgs e)
        {
            try
            {
                //BindPollutants();
                BindPollutants2(); //Start with a flat list of pollutants
                btnAddgroup.Visible = _showGroups;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        /*
        private void BindPollutants()
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select pollutantname,pollutantID from pollutants  where setupid={0} order by pollutantname asc", CommonClass.ManageSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstAvailablePollutants.DataSource = ds.Tables[0];
                lstAvailablePollutants.DisplayMember = "pollutantname";
                if (lstAvailablePollutants.Items.Count > 0)
                {
                    lstAvailablePollutants.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        */
        // Populate with pollutants categorized by pollutant group if showGroups = true
        //  else populate with flat list of pollutants
        private void BindPollutants2()
        {
            try
            {

                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Empty;


                // Clear the list before we rebuild it
                tvAvailablePollutants.Nodes.Clear();
                if (_showGroups)
                {
                    tvAvailablePollutants.ShowLines = true;
                    tvAvailablePollutants.ShowPlusMinus = true;
                    tvAvailablePollutants.Indent = 15;
                    commandText = string.Format("select pgname, PollutantGroupID, pgdescription from PollutantGroups where setupid={0} order by PGName asc", CommonClass.ManageSetup.SetupID);
                    DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

                    // Load pollutant groups as parent nodes for tree
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TreeNode node = new TreeNode(dr["pgname"].ToString());
                        node.Name = node.Text;
                        node.Tag = new List<String> { dr["PollutantGroupID"].ToString(), dr["pgdescription"].ToString() };
                        tvAvailablePollutants.Nodes.Add(node);
                    }

                    commandText = string.Format(@"select c.PollutantGroupID, c.PGName, a.PollutantID, a.PollutantName 
                        from Pollutants a
                        inner join PollutantGroupPollutants b on a.PollutantID = b.PollutantID
                        inner join PollutantGroups c on c.PollutantGroupID = b.PollutantGroupID and c.SetupID = {0}
                        order by a.PollutantName asc", CommonClass.ManageSetup.SetupID);

                    ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

                    TreeNode[] parentNode;
                    // Add pollutants as child nodes to their groups
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        // Search the top level to find this pollutant's group and then add this pollutant as its child
                        parentNode = tvAvailablePollutants.Nodes.Find(dr["PGName"].ToString(), false);
                        if (parentNode != null && parentNode[0] != null)
                        {
                            TreeNode node = new TreeNode(dr["pollutantname"].ToString());
                            node.Name = node.Text;
                            node.Tag = Convert.ToInt32(dr["pollutantid"]);
                            parentNode[0].Nodes.Add(node);
                            parentNode[0].Collapse();
                            //newNode = newNode[0].Nodes.Find(dr["pollutantname"].ToString(), false);
                        }
                    }
                }
                else
                {
                    // Load a flat list of pollutants
                    tvAvailablePollutants.ShowLines = false;
                    tvAvailablePollutants.ShowPlusMinus = false;
                    tvAvailablePollutants.Indent = 1;
                    tvAvailablePollutants.FullRowSelect = true;
                    commandText = string.Format("select pollutantname,pollutantID from pollutants where setupid={0} order by pollutantname asc", CommonClass.ManageSetup.SetupID);
                    DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TreeNode node = new TreeNode(dr["pollutantname"].ToString());
                        node.Name = node.Text;
                        node.Tag = Convert.ToInt32(dr["pollutantID"]);
                        tvAvailablePollutants.Nodes.Add(node);
                    }
                }


                // Initialize pollutant objects if not done
                if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0)
                {
                    CommonClass.PollutantGroup = new BenMAPPollutantGroup();
                    CommonClass.LstPollutant = new List<BenMAPPollutant>();

                }

        }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
}
        /*
        private void lstAvailablePollutants_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = "select metricname from metrics where pollutantid=(select pollutantid from pollutants where pollutantname='" + lstAvailablePollutants.GetItemText(lstAvailablePollutants.SelectedItem) + "' and SetUpID=" + CommonClass.ManageSetup.SetupID + ")";
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstPollutantMetrics.DataSource = ds.Tables[0];
                lstPollutantMetrics.DisplayMember = "metricname";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
*/
        private void btnAdd_Click(object sender, EventArgs e)
        {
            PollutantDefinition frm = new PollutantDefinition();
            DialogResult rtn = frm.ShowDialog();
            if (rtn == DialogResult.OK)
            {
                BindPollutants2();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvAvailablePollutants.SelectedNode == null) return;

                TreeNode sel = tvAvailablePollutants.SelectedNode;
                string nodeName = sel.Name;
                if (_showGroups == true && sel.Level == 0)
                {
                    // Edit the selected pollutant group
                    PollutantGroupDefinition frm = new PollutantGroupDefinition();
                    frm._pollutantGroupID = Convert.ToInt32(((List<String>) sel.Tag)[0]);
                    frm._pollutantGroupDesc = ((List<String>) sel.Tag)[1];
                    frm._pollutantGroupName = nodeName;

                    // Pass the list of pollutants for this group
                    foreach (TreeNode n in sel.Nodes)
                    {
                        frm._pollutantList.Add(Convert.ToInt32(n.Tag));
                    }

                    DialogResult rtn = frm.ShowDialog();
                    if (rtn == DialogResult.OK)
                    {
                        BindPollutants2();
                    }
                } else
                {
                    // Edit the selected pollutant
                    PollutantDefinition frm = new PollutantDefinition();
                    frm._pollutantID = sel.Tag;
                    frm._pollutantIDName = nodeName;
                    DialogResult rtn = frm.ShowDialog();
                    if (rtn == DialogResult.OK)
                    {
                        BindPollutants2();
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        // Deletes the selected record: a pollutant, or a pollutant group
        // Note: deleting a pollutant group deletes only the group, not the pollutants themselves
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (tvAvailablePollutants.SelectedNode == null) return;

            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            string commandText = string.Empty;
            TreeNode sel = tvAvailablePollutants.SelectedNode;
            string nodeName = sel.Name;
            if (_showGroups == true && sel.Level == 0)
            {
                // Delete a pollutant group
                int id = Convert.ToInt32(((List<String>)sel.Tag)[0]);
                commandText = "delete from pollutantgrouppollutants where pollutantgroupid=" + id + "";
                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                commandText = "delete from pollutantgroups where pollutantgroupid=" + id + "";
                fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
            }
            else
            {
                // Delete a pollutant
                try
                {
                    object pollutantID = sel.Tag;
                    string pollutantName = nodeName;
                    commandText = "select MonitorID from Monitors where PollutantID=" + pollutantID + "";
                    DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        MessageBox.Show("The " + pollutantName + " pollutant is used in 'Monitor Datasets'. Please delete monitor datasets that use this pollutant first.");
                        return;
                    }
                    commandText = "select CRFunctionID from CRFunctions where PollutantID=" + pollutantID + "";
                    ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        MessageBox.Show("The " + pollutantName + " pollutant is used in 'Health Impact Function Datasets'. Please delete health impact functions that use this pollutant first.");
                        return;
                    }
                    commandText = "select MetricID from Metrics where PollutantID=" + pollutantID + "";
                    ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);

                    string warning = String.Empty;
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        warning = "All metrics associated with this pollutant will be deleted. ";
                    }
                    DialogResult rtn = MessageBox.Show(warning + "Are you sure you want to delete " + pollutantName + "?", "Confirm Delete", MessageBoxButtons.OKCancel);
                    if (rtn == DialogResult.OK)
                    {
                        commandText = "delete from SeasonalMetricSeasons where SeasonalMetricID in (select SeasonalMetricID from SeasonalMetrics where MetricID in (select MetricID from Metrics where PollutantID=" + pollutantID + "))";
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        commandText = "delete from SeasonalMetrics where MetricID in (select MetricID from Metrics where PollutantID=" + pollutantID + ")";
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        commandText = "delete from Metrics where PollutantID=" + pollutantID + "";
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        commandText = "delete from PollutantSeasons where PollutantID=" + pollutantID + "";
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        commandText = "delete from Pollutants where PollutantID=" + pollutantID + "";
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    }

                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message);
                }
            }

            BindPollutants2();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnAddgroup_Click(object sender, EventArgs e)
        {
            PollutantGroupDefinition frm = new PollutantGroupDefinition();
            DialogResult rtn = frm.ShowDialog();
            if (rtn == DialogResult.OK)
            {
                BindPollutants2();
            }
        }

        private void chkShowGroups_CheckedChanged(object sender, EventArgs e)
        {
            _showGroups = chkShowGroups.Checked;
            BindPollutants2();
            btnAddgroup.Visible = _showGroups;
        }

        private void tvAvailablePollutants_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                TreeNode sel = tvAvailablePollutants.SelectedNode;
                string polName = sel.Name;
                if (_showGroups == true && sel.Level == 0) {
                    polName = null; //Make sure the lookup returns nothing when the user picks a group
                }

                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = "select metricname from metrics where pollutantid=(select pollutantid from pollutants where pollutantname='" + polName + "' and SetUpID=" + CommonClass.ManageSetup.SetupID + ")";
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstPollutantMetrics.DataSource = ds.Tables[0];
                lstPollutantMetrics.DisplayMember = "metricname";

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        /* This hidden button was removed
private void btnCopy_Click(object sender, EventArgs e)
{
   // get current list item for copy
   object pollutantID = (lstAvailablePollutants.SelectedItem as DataRowView).Row["pollutantID"];
   string pollutantName = (lstAvailablePollutants.SelectedItem as DataRowView).Row["pollutantName"].ToString();

   Tools.InputBox myBox = new Tools.InputBox("Copy Pollutant " + pollutantName, "Enter New Pollutant Name", pollutantName + "_copy");
    DialogResult inputResult = myBox.ShowDialog();
   if (inputResult == DialogResult.OK)
   {
       // copy routine goes here
       CopyPollutant cp = new CopyPollutant();
       cp.Copy(int.Parse(pollutantID.ToString()), CommonClass.ManageSetup.SetupID, myBox.InputText);
       MessageBox.Show("Pollutant " + pollutantName + " was copied as " + myBox.InputText);
   }
   else if (inputResult == DialogResult.Cancel)
   {
       MessageBox.Show("Copy cancelled by user");
   }
   // reflect changes in GUI
   BindPollutants();
}
*/
    }
}
