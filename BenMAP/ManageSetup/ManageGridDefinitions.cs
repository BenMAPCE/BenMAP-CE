using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using FirebirdSql.Data.FirebirdClient;
using DotSpatial.Controls;
using DotSpatial.Data;
using ESIL.DBUtility;
using System.IO;

namespace BenMAP
{
    public partial class ManageGridDefinetions : FormBase
    {
        string _dataName = string.Empty;
        private int _dsDataSetID;
        private int _dsSetupID;
        private int _dsDatasetTypeId;
        private int _dsMetadataID;
        private int _dsMetadataEntryID;
        private object _gridDefinitionID;
        private MetadataClassObj _metadataObj = null;


        public ManageGridDefinetions()
        {
            InitializeComponent();            
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            if (!SaveProjection()) return;

            GridDefinition frm = new GridDefinition();

            try
            {
                DialogResult rtn = frm.ShowDialog();
                if (rtn != DialogResult.OK) { return; }
                else
                {
                    lstAvailableGrid.Items.Clear();
                    cboDefaultGridType.Items.Clear();
                    loadGrid();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

            if (!SaveProjection()) return;

            GridDefinition frm = new GridDefinition();
            try
            {
                if (lstAvailableGrid.SelectedItem == null) return;
                ListItem lst = lstAvailableGrid.SelectedItem as ListItem;
                frm._gridIDName = lst.Name; frm._gridID = Convert.ToInt32(_gridDefinitionID);
                string str = lst.Name; DialogResult rtn = frm.ShowDialog();
                if (rtn != DialogResult.OK) { return; }
                else
                {
                    lstAvailableGrid.Items.Clear();
                    cboDefaultGridType.Items.Clear();
                    loadGrid();

                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

        }

        private void ManageGridDefinetions_Load(object sender, EventArgs e)
        {
            try
            {
                loadGrid();
                LoadProjections(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ManageGridDefinetions_Activated(object sender, EventArgs e)
        {
            try
            {
                SetSavedProjection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        Dictionary<int, int> dicShapeOrRegular;
        private void loadGrid()
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commondtext = string.Format("select  GridDefinitionName,GridDefinitionID,Ttype from GridDefinitions where setupid={0} order  by GridDefinitionName asc", CommonClass.ManageSetup.SetupID);
                System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commondtext);
                Dictionary<int, string> dicGridType = new Dictionary<int, string>();
                dicShapeOrRegular = new Dictionary<int, int>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicGridType.Add(Convert.ToInt32(dr["GridDefinitionID"]), dr["GridDefinitionName"].ToString());
                    dicShapeOrRegular.Add(Convert.ToInt16(dr["GridDefinitionID"]), Convert.ToInt16(dr["Ttype"]));
                }
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstAvailableGrid.Items.Add(new ListItem(ds.Tables[0].Rows[i]["GridDefinitionID"].ToString(), ds.Tables[0].Rows[i]["GridDefinitionName"].ToString()));
                    cboDefaultGridType.Items.Add(ds.Tables[0].Rows[i]["GridDefinitionName"].ToString());
                }
                commondtext = string.Format("select  GridDefinitionName from GridDefinitions where setupid={0} and defaultType=1", CommonClass.ManageSetup.SetupID);
                object defaultGrid = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commondtext);
                if (defaultGrid != null)
                {
                    cboDefaultGridType.Text = defaultGrid.ToString();
                }
                if (lstAvailableGrid.Items.Count != 0)
                {
                    lstAvailableGrid.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!SaveProjection()) return;

            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            string commandText = string.Empty;
            if (lstAvailableGrid.SelectedItem != null)
            {
                commandText = "select IncidenceDataSetName from IncidenceDataSets where GridDefinitionID=" + _gridDefinitionID + " ORDER BY IncidenceDataSetName";
                object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                String associatedIncidenceDatasetName = "";
                if (obj != null)
                {
                    System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];

                        associatedIncidenceDatasetName = associatedIncidenceDatasetName + Convert.ToString(dr[0]) + "\n                  ";

                    }
                }
                commandText = "select PopulationDataSetName from PopulationDataSets where GridDefinitionID=" + _gridDefinitionID + " ORDER BY PopulationDataSetName";
                obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                String associatedPopDatasetName = "";
                if (obj != null)
                {
                    System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];

                        associatedPopDatasetName = associatedPopDatasetName + Convert.ToString(dr[0]) + "\n                  ";

                    }
                }
                commandText = "select SetupVariableID from SetupVariables where GridDefinitionID=" + _gridDefinitionID + " ";
                obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                String associatedVariableDatasetName = "";
                if (obj != null)
                {
                    commandText = "SELECT DISTINCT b.SETUPVARIABLEDATASETNAME FROM SETUPVARIABLES a INNER JOIN SETUPVARIABLEDATASETS b ON a.SETUPVARIABLEDATASETID = b.SETUPVARIABLEDATASETID where a.GridDefinitionID =" + _gridDefinitionID + " ORDER BY b.SETUPVARIABLEDATASETNAME";
                    System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];

                        associatedVariableDatasetName = associatedVariableDatasetName + Convert.ToString(dr[0]) + "\n                  ";
                    }
                }

                String deleteWarningText = "";
                if (associatedIncidenceDatasetName != "")
                {
                    deleteWarningText = "Incidence Datasets:"+ "\n                  " + associatedIncidenceDatasetName +"\n";
                }
                if (associatedPopDatasetName != "")
                {
                    deleteWarningText = deleteWarningText  + "Population Datasets:" + "\n                  " + associatedPopDatasetName + "\n";
                }
                if (associatedVariableDatasetName != "")
                {
                    deleteWarningText = deleteWarningText  + "Variable Datasets:" + "\n                  " + associatedVariableDatasetName;
                } 

                DialogResult rtn;
                if (deleteWarningText == "")
                {
                    string msg = string.Format("Delete '{0}' grid definition?", lstAvailableGrid.GetItemText(lstAvailableGrid.SelectedItem));
                    rtn = MessageBox.Show(msg, "Confirm Deletion", MessageBoxButtons.YesNo);
                }
                else
                {
                    DeleteWarningForm deleteWarningForm = new DeleteWarningForm(2);
                    deleteWarningForm.WarningText = deleteWarningText;
                    deleteWarningForm.Message = "If you remove Grid Definition \"" + lstAvailableGrid.GetItemText(lstAvailableGrid.SelectedItem) + "\", the following associated datasets will also be permanently removed as well:";
                    rtn = deleteWarningForm.ShowDialog();
                }
                if (rtn == DialogResult.Cancel || rtn == DialogResult.No)
                { return; }

                if (rtn == DialogResult.Yes)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        int dstID = 0;
                        commandText = "select ttype from GridDefinitions where GridDefinitionID=" + _gridDefinitionID + "";
                        int ttype = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));
                        commandText = "select ShapeFileName from " + (ttype == 1 ? "ShapefileGriddefinitiondetails" : "Regulargriddefinitiondetails") + " where GridDefinitionID=" + _gridDefinitionID + "";
                        object shapefilename = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                        CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + shapefilename.ToString() + ".shp");
                        commandText = "delete from GridDefinitions where GridDefinitionID=" + _gridDefinitionID + "";
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        commandText = "delete from ShapefileGriddefinitiondetails where griddefinitionid=" + _gridDefinitionID + "";
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        //commandText = "delete from Regulargriddefinitiondetails where griddefinitionid=" + _gridDefinitionID + "";
                        //fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                        if (associatedIncidenceDatasetName != "")
                        {
                            commandText = "delete from INCIDENCEDATASETS where griddefinitionid=" + _gridDefinitionID + "";
                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                            //commandText = "delete from INCIDENCERATES where griddefinitionid=" + _gridDefinitionID + "";
                            //fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                            //commandText = "delete from INCIDENCEENTRIES WHERE INCIDENCEENTRIES.INCIDENCERATEID not in (SELECT INCIDENCERATEID FROM INCIDENCERATES)";
                            //fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        }

                        if (associatedPopDatasetName != "")
                        {
                            //commandText = "delete from POPULATIONDATASETS where griddefinitionid=" + _gridDefinitionID + "";
                            //fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                            //commandText = "delete from POPULATIONENTRIES WHERE POPULATIONENTRIES.POPULATIONDATASEID NOT IN (SELECT distinct POPULATIONDATASETID FROM POPULATIONDATASETS)";
                            //fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                            //commandText = "delete from POPULATIONGROWTHWEIGHTS WHERE POPULATIONGROWTHWEIGHTS.POPULATIONDATASETID NOT IN (SELECT distinct POPULATIONDATASETID FROM POPULATIONDATASETS)";
                            //fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                            commandText = "delete from T_POPULATIONDATASETIDYEAR WHERE T_POPULATIONDATASETIDYEAR.POPULATIONDATASETID NOT IN (SELECT distinct POPULATIONDATASETID FROM POPULATIONDATASETS)";
                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        }

                        if (associatedVariableDatasetName != "")
                        {
                            commandText = "delete from SETUPVARIABLEDATASETS WHERE SETUPVARIABLEDATASETS.SETUPVARIABLEDATASETID NOT IN (SELECT distinct SETUPVARIABLEDATASETID FROM SETUPVARIABLES)";
                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                            commandText = "delete from SETUPVARIABLES where griddefinitionid=" + _gridDefinitionID + "";
                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                            commandText = "delete from SETUPGEOGRAPHICVARIABLES WHERE SETUPGEOGRAPHICVARIABLES.SETUPVARIABLEID NOT IN (SELECT distinct SETUPVARIABLEID FROM SETUPVARIABLES)";
                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                            commandText = "delete from SETUPGLOBALVARIABLES WHERE SETUPGLOBALVARIABLES.SETUPVARIABLEID NOT IN (SELECT distinct SETUPVARIABLEID FROM SETUPVARIABLES)";
                            fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        }

                        string datasetIDCriteria = "";

                        commandText = "SELECT DATASETTYPEID FROM DATASETTYPES WHERE DATASETTYPENAME = 'GridDefinition'";
                        dstID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));
                        datasetIDCriteria = "NOT IN (SELECT GRIDDEFINITIONID FROM GRIDDEFINITIONS)";
                        commandText = string.Format("DELETE FROM METADATAINFORMATION WHERE SETUPID = {0} AND DATASETID {1} AND DATASETTYPEID = {2}", CommonClass.ManageSetup.SetupID, datasetIDCriteria, dstID);
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                        commandText = "SELECT DATASETTYPEID FROM DATASETTYPES WHERE DATASETTYPENAME = 'Incidence'";
                        dstID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));
                        datasetIDCriteria = "NOT IN (SELECT INCIDENCEDATASETID FROM INCIDENCEDATASETS)";
                        commandText = string.Format("DELETE FROM METADATAINFORMATION WHERE SETUPID = {0} AND DATASETID {1} AND DATASETTYPEID = {2}", CommonClass.ManageSetup.SetupID, datasetIDCriteria, dstID);
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                        commandText = "SELECT DATASETTYPEID FROM DATASETTYPES WHERE DATASETTYPENAME = 'Population'";
                        dstID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));
                        datasetIDCriteria = "NOT IN (SELECT POPULATIONDATASETID FROM POPULATIONDATASETS)";
                        commandText = string.Format("DELETE FROM METADATAINFORMATION WHERE SETUPID = {0} AND DATASETID {1} AND DATASETTYPEID = {2}", CommonClass.ManageSetup.SetupID, datasetIDCriteria, dstID);
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                        commandText = "SELECT DATASETTYPEID FROM DATASETTYPES WHERE DATASETTYPENAME = 'VariableDataset'";
                        dstID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));
                        datasetIDCriteria = "NOT IN (SELECT SETUPVARIABLEDATASETID FROM SETUPVARIABLEDATASETS)";
                        commandText = string.Format("DELETE FROM METADATAINFORMATION WHERE SETUPID = {0} AND DATASETID {1} AND DATASETTYPEID = {2}", CommonClass.ManageSetup.SetupID, datasetIDCriteria, dstID);
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);


                        lstAvailableGrid.Items.Clear();
                        cboDefaultGridType.Items.Clear();
                        loadGrid();
                        Cursor.Current = Cursors.Default;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Cursor.Current = Cursors.Default;
                    }
                }
            }


        }

        
        private void lstAvailableGrid_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (lstAvailableGrid.Items.Count > 0)
                {
                    ListItem lst = lstAvailableGrid.SelectedItem as ListItem;
                    _gridDefinitionID = lst.ID;
                    _dsDataSetID = Convert.ToInt32(lst.ID);
                    _dsSetupID = CommonClass.ManageSetup.SetupID;
                    _dsDatasetTypeId = SQLStatementsCommonClass.getDatasetID("GridDefinition");
                    _dataName = lst.Name;
                }
                if (dicShapeOrRegular.ContainsKey(Convert.ToInt16(_gridDefinitionID)))
                {
                    if (dicShapeOrRegular[Convert.ToInt16(_gridDefinitionID)] == 1)
                        txtGridType.Text = "Shapefile";
                    else
                        txtGridType.Text = "Regular";
                }
                else
                    txtGridType.Text = "Shapefile";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void cboDefaultGridType_SelectedValueChanged(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            string commandText = string.Empty;
            commandText = string.Format("update GRIDDEFINITIONS set DEFAULTTYPE=0 where setupid={0}", CommonClass.ManageSetup.SetupID);
            int rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
            commandText = string.Format("update GRIDDEFINITIONS set DEFAULTTYPE=1 where GRIDDEFINITIONNAME='{0}' and setupid={1}", cboDefaultGridType.SelectedItem, CommonClass.ManageSetup.SetupID);
            rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
        }


        private void btnViewMetadata_Click(object sender, EventArgs e)
        {

            //_dsMetadataID = SQLStatementsCommonClass.getMetadataID(_dsSetupID, _dsDataSetID, _dsDatasetTypeId);
            _dsMetadataEntryID = SQLStatementsCommonClass.getMetadataEntryID(_dsSetupID, _dsDataSetID, _dsDatasetTypeId);
            //_metadataObj = SQLStatementsCommonClass.getMetadata(_dsDataSetID, _dsSetupID, _dsDatasetTypeId, _dsMetadataID);
            _metadataObj = SQLStatementsCommonClass.getMetadata(_dsDataSetID, _dsSetupID, _dsDatasetTypeId, _dsMetadataEntryID);
            _metadataObj.SetupName = CommonClass.ManageSetup.SetupName;// _dataName;//_lstDataSetName;
            //_metadataObj.MetadataId = _dsMetadataID;
            if(string.IsNullOrEmpty(_metadataObj.FileName))
            {   //if there is no file name, using the data name that is displayed in the list of datasets.
                _metadataObj.FileName = _dataName;
            }
            if (_metadataObj.DatasetId == 0)
            {
                _metadataObj.DatasetId = _dsDataSetID;
            }
            if (_metadataObj.SetupId == 0)
            {
                _metadataObj.SetupId = _dsSetupID;
            }
            if (_metadataObj.DatasetTypeId == 0)
            {
                _metadataObj.DatasetTypeId = _dsDatasetTypeId;
            }
            ViewEditMetadata viewEMdata = new ViewEditMetadata(_metadataObj);
            DialogResult dr = viewEMdata.ShowDialog();
            if (dr.Equals(DialogResult.OK))
            {
                _metadataObj = viewEMdata.MetadataObj;
            }
        }

        private bool SaveProjection()
        {
            if (cboProjections.SelectedItem != null)
            {
                //save selected projection
                string projection = ((DataRowView)cboProjections.SelectedItem).Row["VALUE"].ToString();
                FireBirdHelperBase fb = new ESILFireBirdHelper();
                string commandText = string.Empty;
                commandText = string.Format("update SETUPS set SETUPPROJECTION='{0}' where setupid={1}", projection, CommonClass.ManageSetup.SetupID);
                int rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                //update MainSetup global var
                CommonClass.MainSetup = CommonClass.getBenMAPSetupFromID(CommonClass.MainSetup.SetupID);
                return true;
            }
            else
            {
                MessageBox.Show("You must select a GIS Projection for this setup.", "GIS Projection");
                return false;
            }
        }

        private void SetSavedProjection()
        {
            //select saved projection?
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            string commandText = string.Format("select SETUPPROJECTION from SETUPS where setupid={0}", CommonClass.ManageSetup.SetupID);
            object rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
            if (rtv != DBNull.Value)
            {

                //set selected item
                string projection = Convert.ToString(rtv);
                cboProjections.SelectedValue = projection;
                //if we still don't have a selected value,
                //show full list and reset value
                if (cboProjections.SelectedValue == null)
                {
                    chkShowAll.Checked = true;
                    cboProjections.SelectedValue = projection;
                }

                //disable control if any crosswalks exist for this setup
                commandText = string.Format("select count(*) from GRIDDEFINITIONS gd inner join GRIDDEFINITIONPERCENTAGES gdp " + 
                                            "on gd.GRIDDEFINITIONID = gdp.SOURCEGRIDDEFINITIONID where gd.setupid = {0}", CommonClass.ManageSetup.SetupID);
                rtv = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                if (rtv != DBNull.Value)
                {
                    int count = Convert.ToInt32(rtv);
                    if (count > 0)
                    {
                        cboProjections.Enabled = false;
                        chkShowAll.Enabled = false;
                    }
                }

            }
            else
            {
                cboProjections.SelectedItem = null;
            }
        }

        private void LoadProjections(bool showAll)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("DISPLAY", typeof(string));
            dt.Columns.Add("VALUE", typeof(string));

            System.Collections.Generic.List<string> groups = DotSpatial.Projections.KnownCoordinateSystems.Projected.Names.ToList<string>();
            foreach (string group in groups)
            {
                System.Collections.Generic.List<string> names = DotSpatial.Projections.KnownCoordinateSystems.Projected.GetCategory(group).Names.ToList<string>();
                foreach (string name in names)
                {
                    string display = group + " - " + name;
                    string value = display;
                    dt.Rows.Add(display, value);
                }
            
            }                     

            //filter results
            DataRow [] rows;
            if (!showAll)
            {
                List<string> filtered = new List<string>();
                filtered.Add("'Africa - AfricaAlbersEqualAreaConic'");
                filtered.Add("'Asia - AsiaNorthAlbersEqualAreaConic'");
                filtered.Add("'Asia - AsiaSouthAlbersEqualAreaConic'");
                filtered.Add("'Europe - EuropeAlbersEqualAreaConic'");
                filtered.Add("'NorthAmerica - AlaskaAlbersEqualAreaConic'");
                filtered.Add("'NorthAmerica - CanadaAlbersEqualAreaConic'");
                filtered.Add("'NorthAmerica - HawaiiAlbersEqualAreaConic'");
                filtered.Add("'NorthAmerica - NorthAmericaAlbersEqualAreaConic'");
                filtered.Add("'NorthAmerica - USAContiguousAlbersEqualAreaConicUSGS'");
                filtered.Add("'SouthAmerica - SouthAmericaAlbersEqualAreaConic'");

                rows = dt.Select("VALUE in (" + String.Join(",", filtered.ToArray()) + ")");

                DataTable dtFiltered;
                dtFiltered = rows.CopyToDataTable();
                dtFiltered.DefaultView.Sort = "DISPLAY ASC";

                cboProjections.DataSource = dtFiltered;
            }
            else
            {
                cboProjections.DataSource = dt;
            }
            
            cboProjections.DisplayMember = "DISPLAY";
            cboProjections.ValueMember = "VALUE";        


        }

        private void chkShowAll_CheckedChanged(object sender, EventArgs e)
        {
            LoadProjections(chkShowAll.Checked);
        }


        

    }
}
