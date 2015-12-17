using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESIL.DBUtility;
using BenMAP.Grid;

namespace BenMAP
{
    public partial class PollutantMulti : FormBase
    {
        public PollutantMulti()
        {
            InitializeComponent();
        }
        
        // Used to store pollutant info since TreeView doesn't bind to DataSource 
        public struct PollInfo
        {
            public int groupID;
            public string groupName;
            public int pollID;
            public string pollName;
            public int setupID;
            public int obsID;
        };

        public PollInfo setPollInfo(int pgID, string pgName, int pID, string pName, int sID, int oID)
        {
            PollInfo setPoll = new PollInfo();
            setPoll.groupID = pgID;
            setPoll.groupName = pgName;
            setPoll.pollID = pID;
            setPoll.pollName = pName;
            setPoll.setupID = sID;
            setPoll.obsID = oID;
            return setPoll;
        }

        private void PollutantMulti_Load(object sender, EventArgs e)
        {
            try
            {
                FireBirdHelperBase fb = new ESILFireBirdHelper();
                string commandText = string.Empty;
                TreeNode[] newNode;

                commandText = string.Format("select PGName from PollutantGroups where setupid={0} order by PGName asc", CommonClass.MainSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

                // Load pollutant groups as parent nodes for tree
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    pollTreeView.Nodes.Add(dr["pgname"].ToString(), dr["pgname"].ToString());
                }

                commandText = string.Format("select PollutantGroupPollutants.PollutantGroupID, PGName, Pollutants.PollutantID, PollutantName, Pollutants.SetupID, ObservationType from Pollutants inner join PollutantGroupPollutants on Pollutants.PollutantID=PollutantGroupPollutants.PollutantID inner join PollutantGroups on PollutantGroups.PollutantGroupID=PollutantGroupPollutants.PollutantGroupID and PollutantGroups.SetupID={0} order by PollutantName asc", CommonClass.MainSetup.SetupID);
                ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

                // Add pollutants as child nodes to their groups
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    newNode = pollTreeView.Nodes.Find(dr["PGName"].ToString(), false);
                    if (newNode[0] == null || newNode == null) break;
                    newNode[0].Nodes.Add(dr["pollutantname"].ToString(), dr["pollutantname"].ToString());
                    newNode[0].Collapse();

                    newNode = newNode[0].Nodes.Find(dr["pollutantname"].ToString(), false);
                    newNode[0].Tag = setPollInfo(Convert.ToInt32(dr["pollutantgroupid"]), dr["pgname"].ToString(), Convert.ToInt32(dr["pollutantid"]), dr["pollutantname"].ToString(), Convert.ToInt32(dr["setupid"]), Convert.ToInt32(dr["observationtype"]));
                } 

                // Initialize pollutant objects if not done
                if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0)
                {
                    CommonClass.PollutantGroup = new BenMAPPollutantGroup();
                    CommonClass.LstPollutant = new List<BenMAPPollutant>();
                    return;
                }

                // If a pollutant has already been selected, show in TreeView 
                TreeNode[] selectedNode = pollTreeView.Nodes.Find(CommonClass.PollutantGroup.PollutantGroupName, false);

                if (selectedNode[0] != null || selectedNode != null) 
                {
                    selectTreeView.Nodes.Add((TreeNode)selectedNode[0].Clone());
                } 
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void saveSelected(string name)
        {
            try
            {
                TreeNode[] findParent = pollTreeView.Nodes.Find(name, false);

                if (selectTreeView.Nodes.Count == 0 || findParent[0] == null || findParent == null) return;

                PollInfo tag = (PollInfo)findParent[0].Nodes[0].Tag;
                BenMAPPollutant poll = null;

                if (CommonClass.PollutantGroup == null) CommonClass.PollutantGroup = new BenMAPPollutantGroup();
                CommonClass.PollutantGroup.PollutantGroupID = tag.groupID;
                CommonClass.PollutantGroup.PollutantGroupName = tag.groupName;

                foreach (TreeNode n in findParent[0].Nodes)
                {
                    tag = (PollInfo)n.Tag;
                    poll = GridCommon.getPollutantFromID(tag.pollID);
                    CommonClass.LstPollutant.Add(poll);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void deleteSelected()
        {
            CommonClass.PollutantGroup = null;
            CommonClass.LstPollutant.Clear();
        }

        private void groupTreeView_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void selectedTree_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        private void selectedTree_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
                {
                    TreeView send = (TreeView)sender;
                    TreeNode NewNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
                    if (NewNode.Nodes.Count == 0) NewNode = NewNode.Parent;

                    if (!send.Nodes.ContainsKey(NewNode.Name))
                    {
                        if (send.GetNodeCount(false) > 0) { selectTreeView.Nodes.Clear(); deleteSelected(); }
                        send.Nodes.Add((TreeNode)NewNode.Clone());
                        saveSelected(NewNode.Text);
                    }
                    else { MessageBox.Show(string.Format("{0} has already been selected.", NewNode.Text)); }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void showDetails (PollInfo pInfo) 
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            string commandText = string.Empty;
            try
            {
                txtPollutantName.Text = pInfo.pollName; 
                int obserID = pInfo.obsID;
                switch (obserID)
                {
                    case 0:
                        txtObservationType.Text = "Hourly";
                        tclFixed.Enabled = true;
                        if (cbShowDetails.Checked)
                        {
                            detailGroup.Height = 258;
                            this.Height = 652; 
                        }
                        else
                        {
                            this.Height = 386;
                        }
                        break;
                    case 1:
                        txtObservationType.Text = "Daily";
                        if (cbShowDetails.Checked)
                        {
                            detailGroup.Height = 85;
                            this.Height = 481; 
                        }
                        else
                        {
                            this.Height = 386;
                        }
                        break;
                }
                commandText = string.Format("select MetricName,MetricID,HourlyMetricGeneration from metrics where pollutantid={0}", pInfo.pollID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                DataTable dtMetric = ds.Tables[0].Clone();
                dtMetric = ds.Tables[0].Copy();
                cmbMetric.DataSource = dtMetric;
                cmbMetric.DisplayMember = "MetricName";
                if (dtMetric.Rows.Count != 0)
                { cmbMetric.SelectedIndex = 0; }
                DataRowView drvMetric = cmbMetric.SelectedItem as DataRowView;
                if (drvMetric == null) { return; }
                commandText = string.Format("select SeasonalMetricName,SeasonalMetricID from SeasonalMetrics where MetricID={0}", drvMetric["metricID"]);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cmbSeasonalMetric.DataSource = ds.Tables[0];
                cmbSeasonalMetric.DisplayMember = "SeasonalMetricName";

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void cbShowDetails_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == null || pollTreeView.SelectedNode == null) { return; }
            if (pollTreeView.SelectedNode.Tag == null) { this.Height = 386; return; }
            PollInfo pInfo = (PollInfo)pollTreeView.SelectedNode.Tag;
            showDetails(pInfo);
        }

        public void loadMetric(DataRowView drvMetric)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            string commandText = string.Empty;
            try
            {
                switch (Convert.ToInt32(drvMetric["HourlyMetricGeneration"]))
                {
                    case 1:
                        commandText = string.Format("select starthour,endhour,statistic from FixedWindowMetrics where metricid={0}", drvMetric["metricid"]);
                        DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                        if (ds.Tables[0] == null) { return; }
                        DataRow dr = ds.Tables[0].Rows[0];
                        txtStartHour.Text = dr["starthour"].ToString();
                        txtEndHour.Text = dr["endhour"].ToString();
                        cmbStatistic.SelectedIndex = Convert.ToInt32(dr["statistic"]) - 1;
                        cmbStatistic.Enabled = false;
                        tclFixed.Controls.Clear();
                        tclFixed.TabPages.Add(tabFixed);
                        tclFixed.Visible = true;
                        break;
                    case 2:
                        commandText = string.Format("select WindowSize,WindowStatistic,DailyStatistic from MovingWindowMetrics where metricid={0}", drvMetric["metricid"]);
                        ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                        if (ds.Tables[0].Rows.Count == 0) { return; }
                        dr = ds.Tables[0].Rows[0];
                        txtMovingWindowSize.Text = dr["WindowSize"].ToString();
                        cmbWStatistic.SelectedIndex = Convert.ToInt32(dr["WindowStatistic"]) - 1;
                        cmbWStatistic.Enabled = false;
                        cmbDaily.SelectedIndex = Convert.ToInt32(dr["DailyStatistic"]) - 1;
                        cmbDaily.Enabled = false;
                        tclFixed.Controls.Clear();
                        tclFixed.TabPages.Add(tabMoving);
                        tclFixed.Visible = true;
                        break;
                    case 0:
                        commandText = string.Format("select MetricFunction from CustomMetrics where MetricID={0}", drvMetric["metricid"]);
                        object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                        if (obj != null)
                        {
                            txtFunction.Text = obj.ToString();
                            tclFixed.Controls.Clear();
                            tclFixed.TabPages.Add(tabCustom);
                            tclFixed.Visible = true;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void selectedTree_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                TreeNode newNode = selectTreeView.SelectedNode;
                if (newNode != null)
                {
                    if (newNode.Nodes.Count == 0) newNode = newNode.Parent;

                    string delPollutant = string.Empty;
                    string str = string.Empty;
                    string baseString = string.Empty;
                    string contString = string.Empty;

                    if (CommonClass.LstPollutant != null && CommonClass.LstPollutant.Count > 0)
                    {
                        foreach (BenMAPPollutant p in CommonClass.LstPollutant)
                        {
                            delPollutant = p.PollutantName;
                            str = string.Format("{0}baseline", delPollutant);
                            if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Contains(str.ToLower()))
                            {
                                baseString += delPollutant;
                                baseString += " ";
                            }
                            str = string.Format("{0}control", delPollutant);
                            if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Contains(str.ToLower()))
                            {
                                contString += delPollutant;
                                contString += " ";
                            }
                        }

                        if(baseString.Length != 0)
                        {
                            MessageBox.Show(string.Format("{0} baseline air quality grid is being created. ", delPollutant), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if(contString.Length != 0)
                        {
                            MessageBox.Show(string.Format("{0} control air quality grid is being created. ", delPollutant), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                    }

                    DialogResult result = MessageBox.Show(string.Format("Delete the selected pollutant(s) \'{0}\'? ", newNode.Text), "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result != DialogResult.Yes) { return; }

                    selectTreeView.Nodes.Remove(newNode);
                    deleteSelected();
                }
                else
                {
                    MessageBox.Show("There is no pollutant to delete.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectTreeView.Nodes.Count == 0)
                {
                    MessageBox.Show("Please select a pollutant.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (selectTreeView.Nodes.Count > 0) { selectTreeView.Nodes.Clear(); }
            this.DialogResult = DialogResult.Cancel;
        }

        private void cmbMetric_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataRowView drvMetric = cmbMetric.SelectedItem as DataRowView;
                if (drvMetric == null) { return; }
                loadMetric(drvMetric);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
   

        private void pollTreeView_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender == null) { return; }
                if (pollTreeView.SelectedNode.Tag == null) { this.Height = 386; return; }
                PollInfo pInfo = (PollInfo)pollTreeView.SelectedNode.Tag;
                showDetails(pInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
    }
}
