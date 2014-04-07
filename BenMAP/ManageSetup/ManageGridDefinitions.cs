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
        public ManageGridDefinetions()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
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
                commandText = "select IncidenceDataSetID from IncidenceDataSets where GridDefinitionID=" + _gridDefinitionID + " ";
                object obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                if (obj != null)
                {
                    MessageBox.Show("This grid definition is used in 'Incidence Datasets'. Please delete incidence datasets that use this grid definition first.");
                    return;
                }
                commandText = "select PopulationDataSetID from PopulationDataSets where GridDefinitionID=" + _gridDefinitionID + " ";
                obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                if (obj != null)
                {
                    MessageBox.Show("This grid definition is used in 'Population Datasets'. Please delete population datasets that use this grid definition first.");
                    return;
                }
                commandText = "select SetupVariableID from SetupVariables where GridDefinitionID=" + _gridDefinitionID + " ";
                obj = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
                if (obj != null)
                {
                    MessageBox.Show("This grid definition is used in 'Variable Datasets'. Please delete variable datasets that use this grid definition first.");
                    return;
                }
                string msg = string.Format("Delete '{0}' grid definition?", lstAvailableGrid.GetItemText(lstAvailableGrid.SelectedItem));
                DialogResult result = MessageBox.Show(msg, "Confirm Deletion", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        int gdID = 0; //Grid Definition ID
                        int dstID = 0;
                        commandText = string.Format("SELECT GRIDDEFINITIONID FROM GRIDDEFINITIONS WHERE GRIDDEFINITIONNAME = '{0}' and SETUPID = {1}", lstAvailableGrid.SelectedItem.ToString(), CommonClass.ManageSetup.SetupID);
                        gdID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));
                        commandText = "SELECT DATASETID FROM DATASETS WHERE DATASETNAME = 'GridDefinition'";
                        dstID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));

                        commandText = "select ttype from GridDefinitions where GridDefinitionID=" + _gridDefinitionID + "";
                        int ttype = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));
                        commandText = "select ShapeFileName from " + (ttype == 1 ? "ShapefileGriddefinitiondetails" : "Regulargriddefinitiondetails") + " where GridDefinitionID=" + _gridDefinitionID + "";
                        object shapefilename = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                        CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + shapefilename.ToString() + ".shp");
                        commandText = "delete from GridDefinitions where GridDefinitionID=" + _gridDefinitionID + "";
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        commandText = "delete from ShapefileGriddefinitiondetails where griddefinitionid=" + _gridDefinitionID + "";
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        commandText = "delete from Regulargriddefinitiondetails where griddefinitionid=" + _gridDefinitionID + "";
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

                        commandText = string.Format("DELETE FROM METADATAINFORMATION WHERE SETUPID = {0} AND DATASETID = {1} AND DATASETTYPEID = {2}", CommonClass.ManageSetup.SetupID, gdID, dstID);
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        
                        lstAvailableGrid.Items.Clear();
                        cboDefaultGridType.Items.Clear();
                        loadGrid();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }


        }

        private object _gridDefinitionID;
        private void lstAvailableGrid_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (lstAvailableGrid.Items.Count > 0)
                {
                    ListItem lst = lstAvailableGrid.SelectedItem as ListItem;
                    _gridDefinitionID = lst.ID;
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


    }
}
