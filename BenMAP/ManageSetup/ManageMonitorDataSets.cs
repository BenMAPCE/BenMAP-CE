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
                btnViewMetadata.Enabled = false;
                addGridView();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
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
            MonitorDataSetDefinition frm = new MonitorDataSetDefinition(_lstDataSetName, _lstDataSetID, true);
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

                    commandText = "select DATASETID FROM DATASETS WHERE DATASETNAME = 'Monitor'";
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

                        _dsMetadataID = Convert.ToInt32(drv["metadataid"]);
                        _dsSetupID = CommonClass.ManageSetup.SetupID;//Convert.ToInt32(drv["setupid"]);
                        _dsDataSetId = Convert.ToInt32(_lstDataSetID);//Convert.ToInt32(drv["datasetid"]);//Monitor Dataset Id
                        _dsDatasetTypeId = SQLStatementsCommonClass.getDatasetID("Monitor");//Convert.ToInt32(drv["datasettypeid"]);
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