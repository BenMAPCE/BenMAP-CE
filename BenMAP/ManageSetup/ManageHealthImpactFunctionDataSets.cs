using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;
using System.Diagnostics;
using System.Collections;


namespace BenMAP
{
    public partial class ManageHealthImpactFunctionDataSets : FormBase
    {
        public ManageHealthImpactFunctionDataSets()
        {
            InitializeComponent();
        }
        private MetadataClassObj _metadataObj = null;
        private int _datasetID;
        string commandText = string.Empty;
        string _dataName = string.Empty;
        private int _dsMetadataID;
        private int _dsSetupID;
        private int _dsDataSetId;
        private int _dsDatasetTypeId;
        ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
        DataSet ds;
        DataTable _dt = new DataTable();
        private bool isload = false;
        private List<CRSelectFunction> lstCRSelectFunction = new List<CRSelectFunction>();
        private List<BenMAPHealthImpactFunction> lstBenMAPHealthImpactFunction = new List<BenMAPHealthImpactFunction>();
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                //this is calling the constructor to create a NEW dataset - get the ID in the constructor
                HealthImpactDataSetDefinition frm = new HealthImpactDataSetDefinition();
                DialogResult rth = frm.ShowDialog();
                if (rth != DialogResult.OK) { return; }
                commandText = string.Format("select * from CRFunctionDataSets where SetupID={0}", CommonClass.ManageSetup.SetupID);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstAvailableDataSets.DataSource = ds.Tables[0];
                lstAvailableDataSets.DisplayMember = "CRFunctionDataSetName";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            string str = lstAvailableDataSets.GetItemText(lstAvailableDataSets.SelectedItem);
            try
            {
                DataRowView drv = lstAvailableDataSets.SelectedItem as DataRowView;
                HealthImpactDataSetDefinition frm = new HealthImpactDataSetDefinition(Convert.ToInt16(drv["CrfunctiondatasetID"]), true);//doing an edit
                DialogResult rth = frm.ShowDialog();
                if (rth != DialogResult.OK) 
                { 
                    return;
                }
                commandText = string.Format("select * from CRFunctionDataSets where SetupID={0}", CommonClass.ManageSetup.SetupID);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstAvailableDataSets.DataSource = ds.Tables[0];
                lstAvailableDataSets.DisplayMember = "CRFunctionDataSetName";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void ManageHealthImpactFunctionDataSets_Load(object sender, EventArgs e)
        {
            try
            {
                commandText = string.Format("select CRfunctionDataSetID,CRfunctionDataSetName from CRfunctionDataSets where setupid={0}", CommonClass.ManageSetup.SetupID);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstAvailableDataSets.DataSource = ds.Tables[0];
                lstAvailableDataSets.DisplayMember = "CRfunctionDataSetName";
                isload = true;
                lstAvailableDataSets_SelectedValueChanged(sender, e);


            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void lstAvailableDataSets_SelectedValueChanged(object sender, EventArgs e)
        {
            //_metadataObj = SQLStatementsCommonClass.getMetadata(_datasetID, CommonClass.ManageSetup.SetupID);
            if (isload)
            {
                try
                {
                    cboEndpointGroup.Items.Clear();
                    cboEndpointGroup.Text = string.Empty;
                    cboPollutant.Items.Clear();
                    cboPollutant.Text = string.Empty;
                    if (lstAvailableDataSets.SelectedItem == null)
                    {
                        return;
                    }
                    DataRowView drv = lstAvailableDataSets.SelectedItem as DataRowView;
                    _dataName = drv["CRFUNCTIONDATASETNAME"].ToString();
                    _datasetID = Convert.ToInt32( drv["CRFunctionDataSetID"]);
                    btnViewMetadata.Enabled = false;
                    //commandText = string.Format("select b.endpointgroupname,c.endpointname,d.pollutantname,e.metricname,f.seasonalmetricname,case when Metricstatistic=0 then 'None'  when Metricstatistic=1 then 'Mean' when Metricstatistic=2 then 'Median' when Metricstatistic=3 then 'Max' when Metricstatistic=4 then 'Min' when Metricstatistic=5 then 'Sum'  END as MetricstatisticName,author,yyear,g.locationtypename,location,otherpollutants,qualifier,reference,race,ethnicity,gender,startage,endage,h.functionalformtext,i.functionalformtext,beta,distbeta,p1beta,p2beta,a,namea,b,nameb,c,namec,j.incidencedatasetname,k.incidencedatasetname,l.setupvariabledatasetname as variabeldatasetname,CRFUNCTIONID from crfunctions a join endpointgroups b on (a.ENDPOINTGROUPID=b.ENDPOINTGROUPID) join endpoints c on (a.endpointid=c.endpointid) join pollutants d on (a.pollutantid=d.pollutantid)join metrics e on (a.metricid=e.metricid) left join seasonalmetrics f on (a.seasonalmetricid=f.seasonalmetricid) left join locationtype g on (a.locationtypeid=g.locationtypeid) join functionalforms h on (a.functionalformid=h.functionalformid) join baselinefunctionalforms i on (a.baselinefunctionalformid=i.functionalformid) left join incidencedatasets j on (a.incidencedatasetid=j.incidencedatasetid) left join incidencedatasets k on (a.prevalencedatasetid=k.incidencedatasetid) left join setupvariabledatasets l on (a.variabledatasetid=l.setupvariabledatasetid) where CRFUNCTIONDATASETID={0}", drv["CRFunctionDataSetID"]);
                    commandText = string.Format("select b.endpointgroupname,c.endpointname,d.pollutantname,e.metricname,f.seasonalmetricname, a.metadataid, " +
                                                "case when Metricstatistic=0 then 'None'  when Metricstatistic=1 then 'Mean' when Metricstatistic=2 " +
                                                "then 'Median' when Metricstatistic=3 then 'Max' when Metricstatistic=4 then 'Min' when Metricstatistic=5 " +
                                                "then 'Sum'  END as MetricstatisticName,author,yyear,g.geographicareaname,location,otherpollutants,qualifier,reference, " +
                                                "race,ethnicity,gender,startage,endage,h.functionalformtext,i.functionalformtext,beta,distbeta,p1beta,p2beta,a,namea,b, " +
                                                "nameb,c,namec,j.incidencedatasetname,k.incidencedatasetname,l.setupvariabledatasetname as variabeldatasetname,CRFUNCTIONID " +
                                                "from crfunctions a join endpointgroups b on (a.ENDPOINTGROUPID=b.ENDPOINTGROUPID) " +
                                                "join endpoints c on (a.endpointid=c.endpointid) " +
                                                "join pollutants d on (a.pollutantid=d.pollutantid) " +
                                                "join metrics e on (a.metricid=e.metricid) left join seasonalmetrics f on (a.seasonalmetricid=f.seasonalmetricid) " +
                                                "left join geographicareas g on (a.geographicareaid=g.geographicareaid) join functionalforms h on (a.functionalformid=h.functionalformid) " +
                                                "join baselinefunctionalforms i on (a.baselinefunctionalformid=i.functionalformid) " + 
                                                "left join incidencedatasets j on (a.incidencedatasetid=j.incidencedatasetid) " +
                                                "left join incidencedatasets k on (a.prevalencedatasetid=k.incidencedatasetid) " +
                                                "left join setupvariabledatasets l on (a.variabledatasetid=l.setupvariabledatasetid) " +
                                                "where CRFUNCTIONDATASETID={0}", drv["CRFunctionDataSetID"]);
                    
                    ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    olvData.DataSource = ds.Tables[0];
                    _dt = ds.Tables[0];
                    int dtRow = _dt.Rows.Count;
                    string strTableName = string.Empty;
                    string strPolluantName = string.Empty;
                    cboEndpointGroup.Items.Add("");
                    cboPollutant.Items.Add("");
                    for (int i = 0; i < dtRow; i++)
                    {
                        strTableName = _dt.Rows[i][0].ToString();
                        if (!cboEndpointGroup.Items.Contains(strTableName))
                            cboEndpointGroup.Items.Add(strTableName);
                        strPolluantName = _dt.Rows[i][2].ToString();
                        if (!cboPollutant.Items.Contains(strPolluantName))
                            cboPollutant.Items.Add(strPolluantName);
                    }
                    cboEndpointGroup.SelectedIndex = 0;
                    cboPollutant.SelectedIndex = 0;
                    txtFilter.Text = "";
                    toolStripStatusLabel1.Text = "";
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {   
            int crFunctionDatasetId = 0;
            int datasetid = 0;
            try
            {
                if (lstAvailableDataSets.SelectedItem == null) return;
                if (MessageBox.Show("Delete the selected health impact function dataset?", "Confirm Deletion", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    commandText = string.Format("SELECT CRFUNCTIONDATASETID FROM CRFUNCTIONDATASETS WHERE SETUPID = {0} AND CRFunctionDataSetName = '{1}'", CommonClass.ManageSetup.SetupID, lstAvailableDataSets.Text);
                    crFunctionDatasetId = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text,commandText));
                    commandText = string.Format("delete from CRFunctionDataSets where CRFunctionDataSetName='{0}' and setupid={1}", lstAvailableDataSets.Text, CommonClass.ManageSetup.SetupID);
                    int i = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                    commandText = "select DATASETTYPEID FROM DATASETTYPES WHERE DATASETTYPENAME = 'Healthfunctions'";
                    datasetid = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                    commandText = string.Format("DELETE FROM METADATAINFORMATION WHERE SETUPID ={0} AND DATASETID = {1} AND DATASETTYPEID = {2}", CommonClass.ManageSetup.SetupID, crFunctionDatasetId, datasetid);
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                }
                DataRowView drv = lstAvailableDataSets.SelectedItem as DataRowView;
                lstAvailableDataSets.Items.Remove(drv["CRFunctionDataSetID"]);
                commandText = string.Format("select  CRfunctionDataSetID,CRFunctionDataSetName from CRFunctionDataSets where SetupID={0}", CommonClass.ManageSetup.SetupID);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstAvailableDataSets.DataSource = ds.Tables[0];
                lstAvailableDataSets.DisplayMember = "CRFunctionDataSetName";
                if (lstAvailableDataSets.Items.Count != 0)
                {
                    lstAvailableDataSets.SelectedIndex = 0;
                    lstAvailableDataSets_SelectedValueChanged(sender, e);
                }
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        

        DataTable _dtEndpointGroup = new DataTable();
        DataTable _dtPollutant = new DataTable();
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
                    string selectEndpoint = cboEndpointGroup.GetItemText(cboEndpointGroup.SelectedItem);
                    if (selectEndpoint == "")
                    {
                        olvcEndpointGroup.ValuesChosenForFiltering.Clear();
                        olv.UpdateColumnFiltering();
                    }
                    else
                    {
                        chosenValues.Add(selectEndpoint);
                        olvcEndpointGroup.ValuesChosenForFiltering = chosenValues;
                        olv.UpdateColumnFiltering();
                    }
                    DataRowView drv = lstAvailableDataSets.SelectedItem as DataRowView;
                    if (cboEndpointGroup.Text == "")
                    {
                        commandText = string.Format("select b.endpointgroupname,c.endpointname,d.pollutantname,e.metricname,f.seasonalmetricname, a.metadataid,case " +
                                                    "when Metricstatistic=0 then 'None'  when Metricstatistic=1 then 'Mean' when Metricstatistic=2 " +
                                                    "then 'Median' when Metricstatistic=3 then 'Max' when Metricstatistic=4 then 'Min' " +
                                                    "when Metricstatistic=5 then 'Sum'  " +
                                                    "END as MetricstatisticName,author,yyear,g.geographicareaname,location,otherpollutants,qualifier,reference, " +
                                                    "race,ethnicity,gender,startage,endage,h.functionalformtext,i.functionalformtext,beta,distbeta,p1beta,p2beta,a," +
                                                    "namea,b,nameb,c,namec,j.incidencedatasetname,k.incidencedatasetname,l.setupvariabledatasetname " +
                                                    "as variabeldatasetname,CRFUNCTIONID from crfunctions a join endpointgroups b " + 
                                                    "on (a.ENDPOINTGROUPID=b.ENDPOINTGROUPID) join endpoints c on (a.endpointid=c.endpointid) " +
                                                    "join pollutants d on (a.pollutantid=d.pollutantid)join metrics e on (a.metricid=e.metricid) " +
                                                    "left join seasonalmetrics f on (a.seasonalmetricid=f.seasonalmetricid) left join geographicareas g " +
                                                    "on (a.geographicareaid=g.geographicareaid) join functionalforms h on (a.functionalformid=h.functionalformid) " +
                                                    "join baselinefunctionalforms i on (a.baselinefunctionalformid=i.functionalformid) " +
                                                    "left join incidencedatasets j on (a.incidencedatasetid=j.incidencedatasetid) " +
                                                    "left join incidencedatasets k on (a.prevalencedatasetid=k.incidencedatasetid) " +
                                                    "left join setupvariabledatasets l on (a.variabledatasetid=l.setupvariabledatasetid) " + 
                                                    "where CRFUNCTIONDATASETID={0}", drv["CRFunctionDataSetID"]);
                    }
                    else
                    {
                        commandText = string.Format("select b.endpointgroupname,c.endpointname,d.pollutantname,e.metricname,f.seasonalmetricname, a.metadataid,case " +
                                                    "when Metricstatistic=0 then 'None'  when Metricstatistic=1 then 'Mean' when Metricstatistic=2 " +
                                                    "then 'Median' when Metricstatistic=3 then 'Max' when Metricstatistic=4 then 'Min' when Metricstatistic=5 " +
                                                    "then 'Sum'  END as MetricstatisticName,author,yyear,g.geographicareaname,location,otherpollutants,qualifier, " +
                                                    "reference,race,ethnicity,gender,startage,endage,h.functionalformtext,i.functionalformtext,beta,distbeta,p1beta, " +
                                                    "p2beta,a,namea,b,nameb,c,namec,j.incidencedatasetname,k.incidencedatasetname,l.setupvariabledatasetname " +
                                                    "as variabeldatasetname,CRFUNCTIONID from crfunctions a join endpointgroups b on  " +
                                                    "(a.ENDPOINTGROUPID=b.ENDPOINTGROUPID) join endpoints c on (a.endpointid=c.endpointid)  " +
                                                    "join pollutants d on (a.pollutantid=d.pollutantid)join metrics e on (a.metricid=e.metricid)  " +
                                                    "left join seasonalmetrics f on (a.seasonalmetricid=f.seasonalmetricid) left join geographicareas g  " +
                                                    "on (a.geographicareaid=g.geographicareaid) join functionalforms h on (a.functionalformid=h.functionalformid)  " +
                                                    "join baselinefunctionalforms i on (a.baselinefunctionalformid=i.functionalformid)  " +
                                                    "left join incidencedatasets j on (a.incidencedatasetid=j.incidencedatasetid)  " +
                                                    "left join incidencedatasets k on (a.prevalencedatasetid=k.incidencedatasetid)  " +
                                                    "left join setupvariabledatasets l on (a.variabledatasetid=l.setupvariabledatasetid)  " +
                                                    "where CRFUNCTIONDATASETID={0} and b.endpointgroupname='{1}'", drv["CRFunctionDataSetID"], cboEndpointGroup.Text);
                    }
                    ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    olvData.DataSource = ds.Tables[0];
                    string pollutant = cboPollutant.Text;
                    cboPollutant.Items.Clear();
                    cboPollutant.Items.Add("");
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        if (!cboPollutant.Items.Contains(dr[2].ToString()))
                        {
                            cboPollutant.Items.Add(dr[2].ToString());
                        }
                    }
                    if (!cboPollutant.Items.Contains(pollutant))
                    {
                        cboPollutant.SelectedIndex = 0;
                        cboPollutant.Text = "";
                    }
                    else
                    {
                        cboPollutant.Text = pollutant;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        private void cboPollutant_SelectedValueChanged(object sender, EventArgs e)
        {
            if (isload)
            {
                try
                {
                    ObjectListView olv = olvData;
                    if (olv == null || olv.IsDisposed)
                        return;
                    OLVColumn column = olv.GetColumn("olvcPollutant");

                    ArrayList chosenValues = new ArrayList();
                    string selectPollutant = cboPollutant.GetItemText(cboPollutant.SelectedItem);
                    if (selectPollutant == "")
                    {
                        olvcPollutant.ValuesChosenForFiltering.Clear();
                        olv.UpdateColumnFiltering();
                    }
                    else
                    {
                        chosenValues.Add(selectPollutant);
                        olvcPollutant.ValuesChosenForFiltering = chosenValues;
                        olv.UpdateColumnFiltering();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
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

            System.Diagnostics.Stopwatch stopWatch = new Stopwatch();
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

        private void chbGroup_CheckedChanged(object sender, EventArgs e)
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
            // 2015 09 10 - BENMAP335 - modified to get health impact metadata (was setting _dsDatasetTypeID = 0, which is invalid - hard coded to 6 for health impact functions
            //_metadataObj = SQLStatementsCommonClass.getMetadata(_dsDataSetId, _dsSetupID, _dsDatasetTypeId, _dsMetadataID);//(_datasetID, CommonClass.ManageSetup.SetupID);
            _metadataObj = SQLStatementsCommonClass.getMetadata(_dsDataSetId, _dsSetupID, 6, _dsMetadataID);//(_datasetID, CommonClass.ManageSetup.SetupID);
            _metadataObj.SetupName = CommonClass.ManageSetup.SetupName;//_dataName;
            btnViewMetadata.Enabled = false;
            ViewEditMetadata viewEMdata = new ViewEditMetadata(_metadataObj);
            DialogResult dr = viewEMdata.ShowDialog();
            if (dr.Equals(DialogResult.OK))
            {
                _metadataObj = viewEMdata.MetadataObj;
            }
        }

        private void olvData_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if(sender != null)
                {
                    BrightIdeasSoftware.DataListView dlv = sender as BrightIdeasSoftware.DataListView;

                    if(dlv.SelectedItem != null)
                    {
                        btnViewMetadata.Enabled = true;

                        DataRowView drv = dlv.SelectedItem.RowObject as DataRowView;//dlv.SelectedItem.RowObject

                        _dsMetadataID = Convert.ToInt32(drv["metadataid"]);
                        _dsSetupID = CommonClass.ManageSetup.SetupID;//Convert.ToInt32(drv["setupid"]);
                        _dsDataSetId = Convert.ToInt32(_datasetID);
                        // _dsDataSetId = Convert.ToInt32(drv["datasetid"]);//Monitor Dataset Id
                        // 2015 09 14 - BENMAP 335 - hard code dataset id to avoid problems with future name change
                        _dsDatasetTypeId = 6;
                        // _dsDatasetTypeId = SQLStatementsCommonClass.getDatasetID("Healthfunctions");//Convert.ToInt32(drv["datasettypeid"]);
                    }
                }
            }
            catch(Exception ex)
            {
                // 2015 09 14 - BENMAP 335 - modified catch to log error message
                Logger.LogError(ex.Message);
            }
        }


    }
}
