using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using BrightIdeasSoftware;
using System.Drawing;
using System.Collections.Generic;

namespace BenMAP
{
    public partial class ManageIncidenceDataSets : FormBase
    {
        public ManageIncidenceDataSets()
        {
            InitializeComponent();
        }

        string _dataName = string.Empty;
        private object _dataSetID;
        private MetadataClassObj _metadataObj = null;
        private bool bIsLocked = false;
        // 2014 11 20 - added to support locking and copying (cloning)
        //private object _lstDataSetID;
        private int _dsSetupID;//Setup Id is stored in the olvMonitorDatasets - hidden column olvColumn4
        
        
        private void ManageIncidenceDataSets_Load(object sender, EventArgs e)
        {
            try
            {
                BindControls();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void BindControls()
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select IncidenceDataSetName,incidenceDataSetID from IncidenceDataSets where setupid={0} order  by IncidenceDataSetName asc", CommonClass.ManageSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstAvailableDataSets.DataSource = ds.Tables[0];
                lstAvailableDataSets.DisplayMember = "IncidenceDataSetName";
                if (ds.Tables[0].Rows.Count == 0)
                {
                    olvIncidenceRates.ClearObjects();
                }
                else
                {
                    lstAvailableDataSets.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void lstAvailableDataSets_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender == null) { return; }
                var lst = sender as ListBox;
                if (lst.SelectedItem == null) return;
                DataRowView drv = lst.SelectedItem as DataRowView;
                _dataSetID = drv[1];
                _dataName = drv[0].ToString();
                // 2014 11 20 added next line to support lock/copy (cloning)
                _dsSetupID = CommonClass.ManageSetup.SetupID;
                string commandText = string.Format("select EndPointGroups.EndPointGroupName,EndPoints.EndPointName,IncidenceRates.Prevalence,Races.RaceName,Ethnicity.EthnicityName,Genders.GenderName,IncidenceRates.StartAge,IncidenceRates.EndAge from IncidenceRates,EndPointGroups,EndPoints,Races,Ethnicity,Genders ,IncidenceDataSets where (IncidenceDataSets.IncidenceDataSetID= IncidenceRates.IncidenceDataSetID) and (IncidenceRates.EndPointGroupID=EndPointGroups.EndPointGroupID) and (IncidenceRates.EndPointID=EndPoints.EndPointID) and (IncidenceRates.RaceID=Races.RaceID) and (IncidenceRates.GenderID=Genders.GenderID) and (IncidenceRates.EthnicityID=Ethnicity.EthnicityID) and IncidenceDataSets.IncidenceDataSetID='{0}'", _dataSetID);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][2].ToString() == "F")
                    {
                        ds.Tables[0].Rows[i][2] = "Incidence";
                    }
                    else if (ds.Tables[0].Rows[i][2].ToString() == "T")
                    {
                        ds.Tables[0].Rows[i][2] = "Prevalence";
                    }
                    else
                    { ds.Tables[0].Rows[i][2] = ""; }
                }
                olvIncidenceRates.DataSource = ds.Tables[0];

                cboEndpoint.Items.Clear();
                cboEndpointGroup.Items.Clear();
                string endpointGroupName = string.Empty;
                string endpointName = string.Empty;
                cboEndpointGroup.Items.Add("");
                cboEndpoint.Items.Add("");
                int maxEndpointGroupWidth = 188;
                int EndpointGroupWidth = 188;
                int maxEndpointWidth = 148;
                int EndpointWidth = 148;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    endpointGroupName = ds.Tables[0].Rows[i]["EndPointGroupName"].ToString();
                    if (!cboEndpointGroup.Items.Contains(endpointGroupName))
                    {
                        cboEndpointGroup.Items.Add(endpointGroupName);
                        using (Graphics g = this.CreateGraphics())
                        {
                            SizeF string_size = g.MeasureString(endpointGroupName, this.Font);
                            EndpointGroupWidth = Convert.ToInt16(string_size.Width) + 50;
                        }
                        maxEndpointGroupWidth = Math.Max(maxEndpointGroupWidth, EndpointGroupWidth);
                    }
                    endpointName = ds.Tables[0].Rows[i]["EndPointName"].ToString();
                    if (!cboEndpoint.Items.Contains(endpointName))
                    {
                        cboEndpoint.Items.Add(endpointName);
                        using (Graphics g = this.CreateGraphics())
                        {
                            SizeF string_size = g.MeasureString(endpointName, this.Font);
                            EndpointWidth = Convert.ToInt16(string_size.Width) + 50;
                        }
                        maxEndpointWidth = Math.Max(maxEndpointWidth, EndpointWidth);
                    }
                }
                cboEndpointGroup.DropDownWidth = maxEndpointGroupWidth; cboEndpoint.DropDownWidth = maxEndpointWidth;
                cboEndpointGroup.SelectedIndex = 0;
                cboEndpoint.SelectedIndex = 0;
                btnViewMetadata.Enabled = false;
                // 2014 11 20 - lock control for default data sets
                bIsLocked = isLock();
                setEditControl();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            string dataSetName = lstAvailableDataSets.GetItemText(lstAvailableDataSets.SelectedItem);
            try
            {
                if (dataSetName == string.Empty)
                    return;
                // 2014 11 20 - added bIsLocked to support copy (clone)
                IncidenceDatasetDefinition frm = new IncidenceDatasetDefinition(dataSetName, Convert.ToInt32(_dataSetID),bIsLocked);
                DialogResult rtn = frm.ShowDialog();
                {
                    BindControls();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                IncidenceDatasetDefinition frm = new IncidenceDatasetDefinition();
                DialogResult rtn = frm.ShowDialog();
                BindControls();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            try
            {
                if (lstAvailableDataSets.SelectedItem == null) return;
                string dstName = lstAvailableDataSets.GetItemText(lstAvailableDataSets.SelectedItem);
                string msg = string.Format("Delete the selected incidence or prevalence dataset?", dstName);//lstAvailableDataSets.GetItemText(lstAvailableDataSets.SelectedItem));
                DialogResult result = MessageBox.Show(msg, "Confirm Deletion", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    string commandText = string.Empty;
                    int iprDstID = 0; //Incidence Prevalence Rate Dataset ID
                    int dstID = 0;//DataSetTypeID

                    commandText = string.Format("SELECT INCIDENCEDATASETID FROM INCIDENCEDATASETS WHERE INCIDENCEDATASETNAME = '{0}' and SETUPID = {1}", dstName, CommonClass.ManageSetup.SetupID);
                    iprDstID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));
                    commandText = "SELECT DATASETTYPEID FROM DATASETTYPES WHERE DATASETTYPENAME = 'Incidence'";
                    dstID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));

                    commandText = string.Format("delete from IncidenceDataSets where IncidenceDataSetID='{0}'", _dataSetID);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

                    commandText = string.Format("DELETE FROM METADATAINFORMATION WHERE SETUPID = {0} AND DATASETID = {1} AND DATASETTYPEID = {2}", CommonClass.ManageSetup.SetupID, iprDstID, dstID);
                    fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                    BindControls();
                    if (lstAvailableDataSets.Items.Count == 0)
                    {
                        olvIncidenceRates.DataSource = null;
                        cboEndpointGroup.Items.Clear();
                        cboEndpoint.Items.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            this.TimedFilter(this.olvIncidenceRates, txtFilter.Text);
        }

        private void TimedFilter(ObjectListView olv, string txt)
        {
            this.TimedFilter(olv, txt, 0);
        }

        private void TimedFilter(ObjectListView olv, string txt, int matchKind)
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

            System.Diagnostics.Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            olv.ModelFilter = filter;
            stopWatch.Stop();

        }

        private void chbGroup_CheckedChanged(object sender, EventArgs e)
        {
            ShowGroupsChecked(this.olvIncidenceRates, (CheckBox)sender);
        }

        private void ShowGroupsChecked(ObjectListView olv, CheckBox cb)
        {
            if (cb.Checked && olv.View == View.List)
            {
                cb.Checked = false;
                MessageBox.Show("ListView's cannot show groups when in List view.", "Incidence ListView", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                olv.ShowGroups = cb.Checked;
                olv.BuildList();
            }
        }

        private void cboEndpointGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ObjectListView olv = olvIncidenceRates;
                if (olv == null || olv.IsDisposed)
                    return;
                OLVColumn column = olv.GetColumn("olvcEndpointGroup");

                ArrayList chosenValues = new ArrayList();
                string selectEGroup = cboEndpointGroup.GetItemText(cboEndpointGroup.SelectedItem);
                if (!string.IsNullOrEmpty(selectEGroup))
                {
                    chosenValues.Add(selectEGroup);
                    olvcEndpointGroup.ValuesChosenForFiltering = chosenValues;
                    olv.UpdateColumnFiltering();
                }
                else
                {
                    olvcEndpointGroup.ValuesChosenForFiltering.Clear();
                    olv.UpdateColumnFiltering();
                }

                cboEndpoint.Text = "";
                string commandText = "";
                if (cboEndpointGroup.Text == "")
                {
                    commandText = string.Format("select EndPointGroups.EndPointGroupName,EndPoints.EndPointName,IncidenceRates.Prevalence,Races.RaceName,Ethnicity.EthnicityName,Genders.GenderName,IncidenceRates.StartAge,IncidenceRates.EndAge from IncidenceRates,EndPointGroups,EndPoints,Races,Ethnicity,Genders ,IncidenceDataSets where (IncidenceDataSets.IncidenceDataSetID= IncidenceRates.IncidenceDataSetID) and (IncidenceRates.EndPointGroupID=EndPointGroups.EndPointGroupID) and (IncidenceRates.EndPointID=EndPoints.EndPointID) and (IncidenceRates.RaceID=Races.RaceID) and (IncidenceRates.GenderID=Genders.GenderID) and (IncidenceRates.EthnicityID=Ethnicity.EthnicityID) and IncidenceDataSets.IncidenceDataSetID='{0}'", _dataSetID);
                }
                else
                {
                    commandText = string.Format("select EndPointGroups.EndPointGroupName,EndPoints.EndPointName,IncidenceRates.Prevalence,Races.RaceName,Ethnicity.EthnicityName,Genders.GenderName,IncidenceRates.StartAge,IncidenceRates.EndAge from IncidenceRates,EndPointGroups,EndPoints,Races,Ethnicity,Genders ,IncidenceDataSets where (IncidenceDataSets.IncidenceDataSetID= IncidenceRates.IncidenceDataSetID) and (IncidenceRates.EndPointGroupID=EndPointGroups.EndPointGroupID) and (IncidenceRates.EndPointID=EndPoints.EndPointID) and (IncidenceRates.RaceID=Races.RaceID) and (IncidenceRates.GenderID=Genders.GenderID) and (IncidenceRates.EthnicityID=Ethnicity.EthnicityID) and IncidenceDataSets.IncidenceDataSetID='{0}' and endpointGroups.endpointGroupName='{1}'", _dataSetID, cboEndpointGroup.Text);
                }
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                olvIncidenceRates.DataSource = ds.Tables[0];
                cboEndpoint.Items.Clear();
                cboEndpoint.Items.Add("");
                int maxEndpointWidth = 148;
                int EndpointWidth = 148;
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
                Logger.LogError(ex.Message);
            }
        }

        private void cboEndpoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ObjectListView olv = olvIncidenceRates;
                if (olv == null || olv.IsDisposed)
                    return;
                OLVColumn column = olv.GetColumn("olvcEndpoint");

                ArrayList chosenValues = new ArrayList();
                string selectEndpoint = cboEndpoint.GetItemText(cboEndpoint.SelectedItem);
                if (!string.IsNullOrEmpty(selectEndpoint))
                {
                    chosenValues.Add(selectEndpoint);
                    olvcEndpoint.ValuesChosenForFiltering = chosenValues;
                    olv.UpdateColumnFiltering();
                }
                else
                {
                    olvcEndpoint.ValuesChosenForFiltering.Clear();
                    olv.UpdateColumnFiltering();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnViewMetadata_Click(object sender, EventArgs e)
        {
            _metadataObj = SQLStatementsCommonClass.getMetadata(Convert.ToInt32(_dataSetID), CommonClass.ManageSetup.SetupID);
            _metadataObj.SetupName = CommonClass.ManageSetup.SetupName;//_dataName;//_lstDataSetName;
            btnViewMetadata.Enabled = false;
            ViewEditMetadata viewEMdata = new ViewEditMetadata(_metadataObj);
            DialogResult dr = viewEMdata.ShowDialog();
            if (dr.Equals(DialogResult.OK))
            {
                _metadataObj = viewEMdata.MetadataObj;
            }
        }

        private void olvIncidenceRates_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender != null)
                {

                    BrightIdeasSoftware.DataListView dlv = sender as BrightIdeasSoftware.DataListView;

                    if (dlv.SelectedItem != null)
                    {
                        btnViewMetadata.Enabled = true;                        
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        // 2014 11 20 added for copy (clone)
        private void setEditControl()
        {
            if (bIsLocked)
            {
                btnEdit.Text = "Copy";
            }
            else
            {
                btnEdit.Text = "Edit";
            }
        }
        private bool isLock()
        {
            bool isLocked = false;
            string commandText = string.Empty;
            object obtRtv = null;
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            try
            {
                commandText = string.Format("SELECT LOCKED FROM INCIDENCEDATASETS WHERE INCIDENCEDATASETID = {0} AND SETUPID = {1}", _dataSetID, _dsSetupID);
                obtRtv = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                if (obtRtv.ToString().Equals("T"))
                {
                    isLocked = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

            return isLocked;
        }

    }
}