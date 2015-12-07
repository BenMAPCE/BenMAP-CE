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

        public PollutantMulti()
        {
            InitializeComponent();
        }

        private void PollutantMulti_Load(object sender, EventArgs e)
        {
            lstSPollutant.Items.Clear();
            lstSPollutant.Invalidate();
            try
            {
                FireBirdHelperBase fb = new ESILFireBirdHelper();
                string commandText = string.Empty;

                commandText = string.Format("select PGName from PollutantGroups where setupid={0} order by PollutantGroupID asc", CommonClass.MainSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    pollTreeView.Nodes.Add(dr["pgname"].ToString(), dr["pgname"].ToString());
                }

                commandText = string.Format("select PollutantGroupPollutants.PollutantGroupID, PGName, Pollutants.PollutantID, PollutantName, Pollutants.SetupID, ObservationType from Pollutants inner join PollutantGroupPollutants on Pollutants.PollutantID=PollutantGroupPollutants.PollutantID inner join PollutantGroups on PollutantGroups.PollutantGroupID=PollutantGroupPollutants.PollutantGroupID and PollutantGroups.SetupID={0} order by PollutantGroupPollutants.PollutantGroupID asc", CommonClass.MainSetup.SetupID);
                ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    TreeNode[] newNode = pollTreeView.Nodes.Find(dr["PGName"].ToString(), false);
                    if (newNode[0] == null | newNode == null) break;
                    newNode[0].Nodes.Add(dr["pollutantname"].ToString(), dr["pollutantname"].ToString());
                    newNode[0].Collapse();

                    newNode = newNode[0].Nodes.Find(dr["pollutantname"].ToString(), false);
                    newNode[0].Tag = setPollInfo(Convert.ToInt32(dr["pollutantgroupid"]), dr["pgname"].ToString(), Convert.ToInt32(dr["pollutantid"]), dr["pollutantname"].ToString(), Convert.ToInt32(dr["setupid"]), Convert.ToInt32(dr["observationtype"]));
                } 

                /* from Pollutant.cs to reference for saving groups to global list 
                lstPollutant.DataSource = ds.Tables[0];
                lstPollutant.DisplayMember = "PollutantName";
                if (CommonClass.LstPollutant == null || CommonClass.LstPollutant.Count == 0) { CommonClass.LstPollutant = new List<BenMAPPollutant>(); return; }
                int selectedCount = CommonClass.LstPollutant.Count;
                string str = string.Empty; 
                for (int i = 0; i < selectedCount; i++)
                {
                    str = CommonClass.LstPollutant[i].PollutantName;
                    lstSPollutant.Items.Add(str);
                }*/
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

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
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
            {
                TreeView send = (TreeView)sender;
                TreeNode NewNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
 
                if(!send.Nodes.ContainsKey(NewNode.Name))
                {
                    send.Nodes.Add((TreeNode)NewNode.Clone());
                }
                else
                {
                    MessageBox.Show(string.Format("{0} has already been selected.", NewNode.Text));
                } 
            }
        }

        private void showDetails (PollInfo drv) 
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            string commandText = string.Empty;
            try
            {
                txtPollutantName.Text = drv.pollName; 
                int obserID = drv.obsID;
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
                commandText = string.Format("select MetricName,MetricID,HourlyMetricGeneration from metrics where pollutantid={0}", drv.pollID);
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
            if (sender == null) { return; }
            if (pollTreeView.SelectedNode.Tag == null) return;
            PollInfo drv = (PollInfo)pollTreeView.SelectedNode.Tag;
            showDetails(drv);
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

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                DataRowView drv = lstPollutant.SelectedItem as DataRowView;
                if (drv == null) { return; }
                BenMAPPollutant pollutant = null;
                string type = string.Empty;
                if (lstSPollutant.Items.Count == 0)
                {
                    lstSPollutant.Items.Add(lstPollutant.GetItemText(lstPollutant.SelectedItem));
                    pollutant = GridCommon.getPollutantFromID(int.Parse(drv["PollutantID"].ToString()));
                    CommonClass.LstPollutant.Add(pollutant);
                }
                else if (lstSPollutant.Items.Count != 0)
                {
                    if (!lstSPollutant.Items.Contains(lstPollutant.GetItemText(lstPollutant.SelectedItem)))
                    {
                        lstSPollutant.Items.Add(lstPollutant.GetItemText(lstPollutant.SelectedItem));
                        pollutant = GridCommon.getPollutantFromID(int.Parse(drv["PollutantID"].ToString()));
                        CommonClass.LstPollutant.Add(pollutant);
                    }
                    else
                    { MessageBox.Show(string.Format("{0} has already been selected.", lstPollutant.GetItemText(lstPollutant.SelectedItem))); }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstSPollutant.SelectedItem != null)
                {
                    string delPollutant = lstSPollutant.GetItemText(lstSPollutant.SelectedItem);
                    string str = string.Empty;
                    if (CommonClass.LstPollutant != null && CommonClass.LstPollutant.Count > 0)
                    {
                        str = string.Format("{0}baseline", delPollutant);
                        if (CommonClass.LstAsynchronizationStates != null &&
                            CommonClass.LstAsynchronizationStates.Contains(str.ToLower()))
                        {
                            MessageBox.Show(string.Format("{0} baseline air quality grid is being created. ", delPollutant), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        str = string.Format("{0}control", delPollutant);
                        if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Contains(str.ToLower()))
                        {
                            MessageBox.Show(string.Format("{0} control air quality grid is being created. ", delPollutant), "Please wait", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    DialogResult result = MessageBox.Show(string.Format("Delete the selected pollutant \'{0}\'? ", delPollutant), "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result != DialogResult.Yes) { return; }

                    lstSPollutant.Items.RemoveAt(lstSPollutant.SelectedIndex);
                    int pCount = CommonClass.LstPollutant.Count;
                    BenMAPPollutant p;
                    for (int i = pCount - 1; i > -1; i--)
                    {
                        p = CommonClass.LstPollutant[i];
                        if (p == null || p.PollutantName != delPollutant) { continue; }
                        CommonClass.LstPollutant.Remove(p);
                    }
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

        private void selectedTree_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (selectTreeView.SelectedNode != null)
                {
                    string delPollutant = selectTreeView.SelectedNode.Text;
                    DialogResult result = MessageBox.Show(string.Format("Delete the selected pollutant \'{0}\'? ", delPollutant), "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result != DialogResult.Yes) { return; }

                    selectTreeView.Nodes.Remove(selectTreeView.SelectedNode);
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
                if (lstSPollutant.Items.Count == 0)
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
            if (lstSPollutant.Items.Count > 0)
            { lstSPollutant.Items.Clear(); }
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
   
        private void lstPollutant_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender == null) { return; }
                PollInfo drv = (PollInfo)pollTreeView.SelectedNode.Tag;
                showDetails(drv);
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
                PollInfo drv = (PollInfo)pollTreeView.SelectedNode.Tag;
                showDetails(drv);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void lstSPollutant_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender == null) { return; }
                PollInfo drv = (PollInfo)pollTreeView.SelectedNode.Tag;
                showDetails(drv);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void lstPollutant_DoubleClick(object sender, EventArgs e)
        {
            btnSelect_Click(sender, e);
        }

        private void lstSPollutant_DoubleClick(object sender, EventArgs e)
        {
            btnDelete_Click(sender, e);
        }

    }
}
