using System;
using System.Data;
using System.Windows.Forms;

namespace BenMAP
{
    public partial class ManageMonitorDataSets : FormBase
    {
        public ManageMonitorDataSets()
        {
            InitializeComponent();
        }
        private bool bIsLocked = false;
        string _dataName = string.Empty;
        private string _lstDataSetName;
        private object _lstDataSetID;
        private int _dsMetadataID; //Metadata ID is stored in the olvMonitorDataSets - hidden column olvColumn3
        private int _dsSetupID;//Setup Id is stored in the olvMonitorDatasets - hidden column olvColumn4
        private int _dsDataSetId;//Dataset Id is stored in the olvMonitorDatasets - hidden column olvColumn5
        private int _dsDatasetTypeId;//Dataset Type Id is stored in the olvMonitorDataset - hidden column olvColumn6
        private MetadataClassObj _metadataObj = null;


        private void ManageMonitorDataSets_Load(object sender, EventArgs e)
        {
            addLstBox();
        }

        private void lstAvailableDataSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender == null) 
                { 
                    return; 
                }
                var lst = sender as ListBox;
                DataRowView drv = lst.SelectedItem as DataRowView;
                _lstDataSetID = drv["MonitorDataSetID"];
                _lstDataSetName = drv["MonitorDataSetName"].ToString();
                _dsSetupID = CommonClass.ManageSetup.SetupID;
                btnViewMetadata.Enabled = false;
                addGridView();
                bIsLocked = isLock();
                setEditControl();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
        private void setEditControl()
        {
            if (bIsLocked)
            {
                btnEdit.Text = "Copy1";
                btnEdit.Visible = false;
            }
            else
            {
                btnEdit.Text = "Edit";
                btnEdit.Visible = true;
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
                commandText = string.Format("SELECT LOCKED FROM MONITORDATASETS WHERE MONITORDATASETID = {0} AND SETUPID = {1}", _lstDataSetID, _dsSetupID);
                obtRtv = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                if(obtRtv.ToString().Equals("T"))
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

        private void addLstBox()
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select MonitorDataSetID, MonitorDataSetName from MonitorDataSets where setupid={0} order  by MonitorDataSetName asc", CommonClass.ManageSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstAvailableDataSets.DataSource = ds.Tables[0];
                lstAvailableDataSets.DisplayMember = "MonitorDataSetName";
                if (ds.Tables[0].Rows.Count == 0)
                {
                    olvMonitorDataSets.ClearObjects();
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

        private void addGridView()
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                //string commandText = string.Format("select c.pollutantname,a.yyear,count(*) from monitorentries a,monitors b,pollutants c " +
                //                                   "where a.monitorid=b.monitorid and b.pollutantid=c.pollutantid and b.monitordatasetID={0} " +
                //                                   "group by c.pollutantname,a.yyear", _lstDataSetID);

                string commandText = string.Format("select b.monitordatasetID, b.METADATAID, c.pollutantname,a.yyear,count(*) " + 
                                                   "from monitorentries a,monitors b,pollutants c " +
                                                   "where a.monitorid=b.monitorid and b.pollutantid=c.pollutantid " +
                                                   "and b.monitordatasetID={0} group by c.pollutantname,a.yyear , b.monitordatasetid, b.METADATAID", _lstDataSetID);

                //string commandText = string.Format("SELECT c.pollutantname,a.yyear,count(*), d.SETUPID, d.DATASETID, d.DATASETTYPEID " +
                //                                   "FROM monitorentries a,monitors b,pollutants c, metadatainformation d " +
                //                                   "WHERE a.monitorid=b.monitorid and b.pollutantid=c.pollutantid " +
                //                                   "AND b.monitordatasetID={0} " +
                //                                   "GROUP BY c.pollutantname,a.yyear, d.SETUPID, d.DATASETID, d.DATASETTYPEID", _lstDataSetID);
                
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                olvMonitorDataSets.DataSource = ds.Tables[0];
               
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lstAvailableDataSets.Items.Count == 0)
            { return; }
            MonitorDataSetDefinition frm = new MonitorDataSetDefinition(_lstDataSetName, _lstDataSetID, bIsLocked);
            DialogResult rtn = frm.ShowDialog();
            {
                addLstBox();
                addGridView();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            MonitorDataSetDefinition frm = new MonitorDataSetDefinition();
            DialogResult rtn = frm.ShowDialog();
            {
                addLstBox();
                addGridView();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstAvailableDataSets.SelectedItem == null) return;
                DialogResult rtn = MessageBox.Show("Delete the '" + _lstDataSetName + "' monitor dataset?", "Confirm Deletion", MessageBoxButtons.YesNo);
                if (rtn == DialogResult.Yes)
                {
                    string commandText = string.Empty;
                    ESIL.DBUtility.ESILFireBirdHelper fb = new ESIL.DBUtility.ESILFireBirdHelper();

                    commandText = "SELECT DATASETTYPEID FROM DATASETTYPES WHERE DATASETTYPENAME = 'Monitor'";
                    int datasetid = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));

                    commandText = string.Format("delete from MonitorDataSets where MonitorDataSetID={0}", _lstDataSetID);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                   
                    commandText = string.Format("DELETE FROM METADATAINFORMATION WHERE SETUPID ={0} AND DATASETID = {1} AND DATASETTYPEID = {2}", CommonClass.ManageSetup.SetupID, _lstDataSetID, datasetid);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    
                    addLstBox();
                    addGridView();
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

        private void btnViewMetadata_Click(object sender, EventArgs e)
        {
            try
            {
                _metadataObj = SQLStatementsCommonClass.getMetadata(_dsDataSetId, _dsSetupID, _dsDatasetTypeId, _dsMetadataID);
                if(_metadataObj.DatasetId == 0)
                {
                    _metadataObj.DatasetId = _dsDataSetId;
                }
                if(_metadataObj.SetupId == 0)
                {
                    _metadataObj.SetupId = _dsSetupID;
                }
                if(_metadataObj.DatasetTypeId == 0)
                {
                    _metadataObj.DatasetTypeId = _dsDatasetTypeId;
                }
                _metadataObj.SetupName = CommonClass.ManageSetup.SetupName;// _lstDataSetName;
                btnViewMetadata.Enabled = false;
                ViewEditMetadata viewEMdata = new ViewEditMetadata(_metadataObj);
                DialogResult dr = viewEMdata.ShowDialog();
                if (dr.Equals(DialogResult.OK))
                {
                    _metadataObj = viewEMdata.MetadataObj;
                    addLstBox();
                    addGridView();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void olvMonitorDataSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    
                    BrightIdeasSoftware.DataListView dlv = sender as BrightIdeasSoftware.DataListView;

                    if (dlv.SelectedItem != null)
                    {
                        btnViewMetadata.Enabled = true;

                        DataRowView drv = dlv.SelectedItem.RowObject as DataRowView;//dlv.SelectedItem.RowObject
                        if(!string.IsNullOrEmpty((drv["metadataid"].ToString())))
                        {
                            _dsMetadataID = Convert.ToInt32(drv["metadataid"]);
                        }
                        else
                        {
                            _dsMetadataID = 0;
                        }
                        //_dsSetupID = CommonClass.ManageSetup.SetupID;//Convert.ToInt32(drv["setupid"]);
                        _dsDataSetId = Convert.ToInt32(_lstDataSetID);//Convert.ToInt32(drv["datasetid"]);//Monitor Dataset Id
                        _dsDatasetTypeId = SQLStatementsCommonClass.getDatasetID("Monitor");//Convert.ToInt32(drv["datasettypeid"]);
                        _metadataObj = null;//clearing out the old metadata object.
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (lstAvailableDataSets.SelectedItem == null) return;
            string strSetName = lstAvailableDataSets.GetItemText(lstAvailableDataSets.SelectedItem);
            Tools.InputBox myBox = new Tools.InputBox("Copy Monitor Dataset " + strSetName, "Enter New Monitor Dataset Name", strSetName + "_copy");
            DialogResult inputResult = myBox.ShowDialog();
            if (inputResult == DialogResult.OK)
            {
                // copy routine goes here
                CopyMonitors cp = new CopyMonitors();

                cp.Copy(int.Parse(_lstDataSetID.ToString()), CommonClass.ManageSetup.SetupID, myBox.InputText);
                //MessageBox.Show("Pollutant " + pollutantName + " was copied as " + myBox.InputText);
            }
            else if (inputResult == DialogResult.Cancel)
            {
                MessageBox.Show("Copy cancelled by user");
            }
            // refresh form
            addLstBox();
            addGridView();
        }
    }
}