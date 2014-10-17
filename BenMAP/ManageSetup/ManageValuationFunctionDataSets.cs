using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using System.Collections;

namespace BenMAP
{
    public partial class ManageValuationFunctionDataSets : FormBase
    {
        public ManageValuationFunctionDataSets()
        {
            InitializeComponent();
        }
        private MetadataClassObj _metadataObj = null;
        private int _dsMetadataID;
        private int _dsSetupID;
        private int _dataSetID = -1;
        //private int _dsDataSetId;
        private int _dsDatasetTypeId;
        private string _dataName = string.Empty;

        private string commandText = string.Empty;
        ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
        DataSet ds;
        DataTable _dt = new DataTable();
        private bool isload = false;
        
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ValuationFunctionDataSetDefinition frm = new ValuationFunctionDataSetDefinition();
                DialogResult rth = frm.ShowDialog();
                if (rth != DialogResult.OK) 
                { 
                    return; 
                }
                commandText = string.Format("select * from VALUATIONFUNCTIONDATASETS where SetupID={0}", CommonClass.ManageSetup.SetupID);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstAvailableDataSets.DataSource = ds.Tables[0];
                lstAvailableDataSets.DisplayMember = "VALUATIONFUNCTIONDATASETNAME";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void ManageValuationFunctionDataSets_Load(object sender, EventArgs e)
        {
            try
            {
                commandText = string.Format("select VALUATIONFUNCTIONDATASETID,VALUATIONFUNCTIONDATASETNAME from VALUATIONFUNCTIONDATASETS where setupid={0} ", CommonClass.ManageSetup.SetupID);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstAvailableDataSets.DataSource = ds.Tables[0];
                lstAvailableDataSets.DisplayMember = "VALUATIONFUNCTIONDATASETNAME";
                isload = true;
                lstAvailableDataSets_SelectedValueChanged(sender, e);
                int dtRow = _dt.Rows.Count;
                string strTableName = string.Empty;
                string strEndpointName = string.Empty;
                for (int i = 0; i < dtRow; i++)
                {
                    strTableName = _dt.Rows[i][0].ToString();
                    if (!cboEndpointGroup.Items.Contains(strTableName))
                        cboEndpointGroup.Items.Add(strTableName);
                    strEndpointName = _dt.Rows[i][1].ToString();
                    if (!cboEndpoint.Items.Contains(strEndpointName))
                        cboEndpoint.Items.Add(strEndpointName);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        
        private void lstAvailableDataSets_SelectedValueChanged(object sender, EventArgs e)
        {
            if (isload)
                try
                {
                    cboEndpointGroup.Items.Clear();
                    cboEndpointGroup.Text = string.Empty;
                    cboEndpoint.Items.Clear();
                    cboEndpoint.Text = string.Empty;
                    txtFilter.Text = "";
                    toolStripStatusLabel1.Text = "";
                    if (sender == null || lstAvailableDataSets.SelectedItem == null) { return; }
                    DataRowView drv = lstAvailableDataSets.SelectedItem as DataRowView;
                    _dataSetID = Convert.ToInt16(drv["VALUATIONFUNCTIONDATASETID"]);
                    
                    commandText = string.Format("select endpointgroups.endpointgroupname,endpoints.endpointname, valuationfunctions.qualifier, " +
                                                "valuationfunctions.reference,valuationfunctions.startage,valuationfunctions.endage, " +
                                                "valuationfunctionalforms.functionalformtext,valuationfunctions.a,valuationfunctions.namea, " +
                                                "valuationfunctions.dista,valuationfunctions.p1a,valuationfunctions.p2a,valuationfunctions.b, " +
                                                "valuationfunctions.nameb,valuationfunctions.c,valuationfunctions.namec,valuationfunctions.d, " +
                                                "valuationfunctions.named, valuationfunctions.metadataid " +
                                                "from valuationfunctions , endpoints , endpointgroups,valuationfunctionalforms " +
                                                "where valuationfunctions.endpointid=endpoints.endpointid " +
                                                "and endpointgroups.endpointgroupid=valuationfunctions.endpointgroupid " +
                                                "and valuationfunctions.functionalformid=valuationfunctionalforms.functionalformid " +
                                                "and valuationfunctiondatasetid={0} order by valuationfunctions.metadataid, endpointgroupname asc", _dataSetID);//drv["VALUATIONFUNCTIONDATASETID"]);

                    ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    olvData.DataSource = ds.Tables[0];
                    _dt = ds.Tables[0];
                    int dtRow = _dt.Rows.Count;
                    string strTableName = string.Empty;
                    string strEndpointName = string.Empty;
                    cboEndpointGroup.Items.Add("");
                    cboEndpoint.Items.Add("");
                    int maxEndpointGroupWidth = 194;
                    int EndpointGroupWidth = 194;
                    int maxEndpointWidth = 180;
                    int EndpointWidth = 180;
                    for (int i = 0; i < dtRow; i++)
                    {
                        strTableName = _dt.Rows[i][0].ToString();
                        if (!cboEndpointGroup.Items.Contains(strTableName))
                        {
                            cboEndpointGroup.Items.Add(strTableName);
                            using (Graphics g = this.CreateGraphics())
                            {
                                SizeF string_size = g.MeasureString(strTableName, this.Font);
                                EndpointGroupWidth = Convert.ToInt16(string_size.Width) + 50;
                            }
                            maxEndpointGroupWidth = Math.Max(maxEndpointGroupWidth, EndpointGroupWidth);
                        }
                        strEndpointName = _dt.Rows[i][1].ToString();
                        if (!cboEndpoint.Items.Contains(strEndpointName))
                        {
                            cboEndpoint.Items.Add(strEndpointName);
                            using (Graphics g = this.CreateGraphics())
                            {
                                SizeF string_size = g.MeasureString(strEndpointName, this.Font);
                                EndpointWidth = Convert.ToInt16(string_size.Width) + 50;
                            }
                            maxEndpointWidth = Math.Max(maxEndpointWidth, EndpointWidth);
                        }
                    }
                    cboEndpointGroup.DropDownWidth = maxEndpointGroupWidth; cboEndpoint.DropDownWidth = maxEndpointWidth;
                    cboEndpointGroup.SelectedIndex = 0;
                    cboEndpoint.SelectedIndex = 0;
                    btnViewMetadata.Enabled = false;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
        }
        private void InitTree(DataTable dt, TreeView tv)
        {
            int Rows = dt.Rows.Count;
            TreeNode[] curNode = new TreeNode[Rows];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];
                for (int j = 1; j <= Rows; j++)
                {
                    if (j == 1 && dr[j].ToString() != string.Empty)
                    {
                        TreeNode tn = new TreeNode(dr[j].ToString());

                        curNode[0] = tn;
                        break;
                    }
                    else if (j >= 2)
                    {
                        if (dr[j].ToString() != string.Empty)
                        {
                            TreeNode tn = new TreeNode(dr[j].ToString());
                            curNode[j - 2].Nodes.Add(tn);
                            curNode[j - 1] = tn;
                            break;
                        }
                    }
                }
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string str = lstAvailableDataSets.GetItemText(lstAvailableDataSets.SelectedItem);
            try
            {
                ValuationFunctionDataSetDefinition frm = new ValuationFunctionDataSetDefinition(str, _dataSetID, true);
                DialogResult rth = frm.ShowDialog();
                if (rth != DialogResult.OK) { return; }
                commandText = string.Format("select * from VALUATIONFUNCTIONDATASETS where SetupID={0}", CommonClass.ManageSetup.SetupID);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstAvailableDataSets.DataSource = ds.Tables[0];
                lstAvailableDataSets.DisplayMember = "VALUATIONFUNCTIONDATASETNAME";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstAvailableDataSets.SelectedItem == null) return;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Empty;
                int vfdID = 0;//ValuationFunctionDatasetID
                int dstID = 0;//DataSetTypeID
                if (MessageBox.Show("Delete the selected valuation function dataset?", "Confirm Deletion", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    commandText = string.Format("SELECT VALUATIONFUNCTIONDATASETID FROM VALUATIONFUNCTIONDATASETS WHERE ValuationFunctionDataSetName = '{0}' and SETUPID = {1}", lstAvailableDataSets.Text, CommonClass.ManageSetup.SetupID);
                    vfdID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));
                    commandText = "SELECT DATASETTYPEID FROM DATASETTYPES WHERE DATASETTYPENAME = 'Valuationfunction'";
                    dstID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));
                    
                    commandText = string.Format("delete from ValuationFunctionDataSets where ValuationFunctionDataSetName='{0}' and setupid={1} and VALUATIONFUNCTIONDATASETID = {2}", lstAvailableDataSets.Text, CommonClass.ManageSetup.SetupID, vfdID);
                    int i = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    
                    commandText = string.Format("DELETE FROM METADATAINFORMATION WHERE SETUPID = {0} AND DATASETID = {1} AND DATASETTYPEID = {2}",CommonClass.ManageSetup.SetupID,vfdID, dstID);
                    i = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                }
                commandText = string.Format("select * from ValuationFunctionDataSets where SetupID={0}", CommonClass.ManageSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstAvailableDataSets.DataSource = ds.Tables[0];
                lstAvailableDataSets.DisplayMember = "ValuationFunctionDataSetName";
                if (ds.Tables[0].Rows.Count == 0)
                {
                    olvData.ClearObjects();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        DataTable _dtEndpointGroup = new DataTable();
        DataTable _dtEndpoint = new DataTable();

        private void cboEndpointGroup_SelectedValueChanged(object sender, EventArgs e)
        {
            if (isload)
            {
                try
                {
                    ObjectListView olv = olvData;
                    if (olv == null || olv.IsDisposed)
                        return;
                    OLVColumn column = olv.GetColumn("olvcEndpointGroup");

                    ArrayList chosenValues = new ArrayList();
                    string selectEndpointGroup = cboEndpointGroup.GetItemText(cboEndpointGroup.SelectedItem);
                    if (selectEndpointGroup == "")
                    {
                        olvcEndpointGroup.ValuesChosenForFiltering.Clear();
                        olv.UpdateColumnFiltering();
                    }
                    else
                    {
                        chosenValues.Add(selectEndpointGroup);
                        olvcEndpointGroup.ValuesChosenForFiltering = chosenValues;
                        olv.UpdateColumnFiltering();
                    }
                    cboEndpoint.Text = "";
                    string commandText = "";
                    if (cboEndpointGroup.Text == "")
                    {
                        commandText = string.Format("select endpointgroups.endpointgroupname,endpoints.endpointname, valuationfunctions.qualifier, " +
                                                    "valuationfunctions.reference,valuationfunctions.startage,valuationfunctions.endage," +
                                                    "valuationfunctionalforms.functionalformtext,valuationfunctions.a,valuationfunctions.namea, " + 
                                                    "valuationfunctions.dista,valuationfunctions.p1a,valuationfunctions.p2a,valuationfunctions.b, " +
                                                    "valuationfunctions.nameb,valuationfunctions.c,valuationfunctions.namec,valuationfunctions.d, " +
                                                    "valuationfunctions.named, valuationfunctions.metadataid " +
                                                    "from valuationfunctions , endpoints , endpointgroups,valuationfunctionalforms " +
                                                    "where valuationfunctions.endpointid=endpoints.endpointid and endpointgroups.endpointgroupid=valuationfunctions.endpointgroupid " +
                                                    " and valuationfunctions.functionalformid=valuationfunctionalforms.functionalformid and " +
                                                    "valuationfunctiondatasetid={0} order by endpointgroupname asc", _dataSetID);
                    }
                    else
                    {
                        commandText = string.Format("select endpointgroups.endpointgroupname,endpoints.endpointname, valuationfunctions.qualifier, " +
                                                    "valuationfunctions.reference,valuationfunctions.startage,valuationfunctions.endage, " +
                                                    "valuationfunctionalforms.functionalformtext,valuationfunctions.a,valuationfunctions.namea, " +
                                                    "valuationfunctions.dista,valuationfunctions.p1a,valuationfunctions.p2a,valuationfunctions.b, " +
                                                    "valuationfunctions.nameb,valuationfunctions.c,valuationfunctions.namec,valuationfunctions.d, " +
                                                    "valuationfunctions.named, valuationfunctions.metadataid " +
                                                    "from valuationfunctions , endpoints , endpointgroups,valuationfunctionalforms " +
                                                    "where valuationfunctions.endpointid=endpoints.endpointid and " +
                                                    "endpointgroups.endpointgroupid=valuationfunctions.endpointgroupid and " +
                                                    "valuationfunctions.functionalformid=valuationfunctionalforms.functionalformid " +
                                                    "and valuationfunctiondatasetid={0} and endpointGroups.endpointGroupName='{1}' " +
                                                    "order by endpointgroupname asc", _dataSetID, cboEndpointGroup.Text);
                    }
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    olvData.DataSource = ds.Tables[0];
                    cboEndpoint.Items.Clear();
                    cboEndpoint.Items.Add("");
                    int maxEndpointWidth = 180;
                    int EndpointWidth = 180;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        if (!cboEndpoint.Items.Contains(dr[1].ToString()))
                        {
                            cboEndpoint.Items.Add(dr[1].ToString());
                        }
                        using (Graphics g = this.CreateGraphics())
                        {
                            SizeF string_size = g.MeasureString(dr[1].ToString(), this.Font);
                            EndpointWidth = Convert.ToInt16(string_size.Width) + 50;
                        }
                        maxEndpointWidth = Math.Max(maxEndpointWidth, EndpointWidth);
                    }
                    cboEndpoint.DropDownWidth = maxEndpointWidth;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        private void cboEndpoint_SelectedValueChanged(object sender, EventArgs e)
        {
            if (isload)
            {
                try
                {
                    ObjectListView olv = olvData;
                    if (olv == null || olv.IsDisposed)
                        return;
                    OLVColumn column = olv.GetColumn("olvcEndpoint");

                    ArrayList chosenValues = new ArrayList();
                    string selectEndpoint = cboEndpoint.GetItemText(cboEndpoint.SelectedItem);
                    if (selectEndpoint == "")
                    {
                        olvcEndpoint.ValuesChosenForFiltering.Clear();
                        olv.UpdateColumnFiltering();
                    }
                    else
                    {
                        chosenValues.Add(selectEndpoint);
                        olvcEndpoint.ValuesChosenForFiltering = chosenValues;
                        olv.UpdateColumnFiltering();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        private void txtFilter_TextChanged_1(object sender, EventArgs e)
        {
            if (txtFilter.Text != "")
            {
                this.TimedFilter(this.olvData, txtFilter.Text);
            }
            else
            {
                toolStripStatusLabel1.Text = "";
                olvData.ModelFilter = null;
                olvData.DefaultRenderer = null;
            }
        }

        void TimedFilter(ObjectListView olv, string txt)
        {
            this.TimedFilter(olv, txt, 0);
        }

        void TimedFilter(ObjectListView olv, string txt, int matchKind)
        {
            TextMatchFilter filter = null;
            if (!String.IsNullOrEmpty(txt))
            {
                switch (matchKind)
                {
                    case 0:
                    default:
                        filter = TextMatchFilter.Contains(olv, txt);
                        break;
                    case 1:
                        filter = TextMatchFilter.Prefix(olv, txt);
                        break;
                    case 2:
                        filter = TextMatchFilter.Regex(olv, txt);
                        break;
                }
            }
            if (filter == null)
                olv.DefaultRenderer = null;
            else
            {
                olv.DefaultRenderer = new HighlightTextRenderer(filter);

                olv.DefaultRenderer = new HighlightTextRenderer { Filter = filter, UseGdiTextRendering = true };
            }

            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            olv.ModelFilter = filter;
            stopWatch.Stop();

            IList objects = olv.Objects as IList;
            if (olv.Items.Count == 0)
                this.toolStripStatusLabel1.Text =
                    String.Format("Filtered in {0}ms", stopWatch.ElapsedMilliseconds);
            else
                this.toolStripStatusLabel1.Text =
                    String.Format("Filtered {0} items down to {1} items in {2}ms",
                                  objects.Count,
                                  olv.Items.Count,
                                  stopWatch.ElapsedMilliseconds);
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ShowGroupsChecked(this.olvData, (CheckBox)sender);
        }
        void ShowGroupsChecked(ObjectListView olv, CheckBox cb)
        {
            if (cb.Checked && olv.View == View.List)
            {
                cb.Checked = false;
                MessageBox.Show("ListView's cannot show groups when in List view.", "Object List View Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                olv.ShowGroups = cb.Checked;
                olv.BuildList();
            }
        }

        private void btnViewMetadata_Click(object sender, EventArgs e)
        {
            if(lstAvailableDataSets.SelectedItem != null)
            {//_dataSetID = Convert.ToInt16(drv["VALUATIONFUNCTIONDATASETID"]);

                DataRowView drv = lstAvailableDataSets.SelectedItem as DataRowView;
                _metadataObj = SQLStatementsCommonClass.getMetadata(_dataSetID, _dsSetupID, _dsDatasetTypeId, _dsMetadataID);
                _metadataObj.DatasetId = _dataSetID;//Convert.ToInt32(drv["VALUATIONFUNCTIONDATASETID"]);
                _metadataObj.SetupName = CommonClass.ManageSetup.SetupName;//drv["VALUATIONFUNCTIONDATASETNAME"].ToString();//_dataName
                btnViewMetadata.Enabled = false;
                ViewEditMetadata viewEMdata = new ViewEditMetadata(_metadataObj);
                DialogResult dr = viewEMdata.ShowDialog();
                if (dr.Equals(DialogResult.OK))
                {
                    _metadataObj = viewEMdata.MetadataObj;
                    olvData.ClearHotItem();
                    olvData.SelectedIndex = -1;
                }
            }
        }

        private void olvData_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if(sender != null)
                {
                    BrightIdeasSoftware.DataListView dlv = sender as BrightIdeasSoftware.DataListView;

                    if (dlv.SelectedItem != null)
                    {
                        btnViewMetadata.Enabled = true;

                        DataRowView drv = dlv.SelectedItem.RowObject as DataRowView;

                        _dsMetadataID = Convert.ToInt32(drv["metadataid"]);
                        _dsSetupID = CommonClass.ManageSetup.SetupID;
                        //_dataSetID = Convert.ToInt32(drv["VALUATIONFUNCTIONDATASETID"]);
                        _dsDatasetTypeId = SQLStatementsCommonClass.getDatasetID("Valuationfunction");
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO:  FIX THIS.
                //do nothing for now until I can get the metadta to run correctly
                //throw new Exception(ex.Message);
            }
        }

    }
}
