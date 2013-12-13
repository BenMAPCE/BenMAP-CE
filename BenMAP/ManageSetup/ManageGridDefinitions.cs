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
                //返回属性
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
                //DataRowView dr = lstAvailableGrid.SelectedItem as DataRowView;
                ListItem lst = lstAvailableGrid.SelectedItem as ListItem;
                frm._gridIDName = lst.Name;//dr.Row["GRIDDEFINITIONNAME"].ToString();
                frm._gridID = Convert.ToInt32( _gridDefinitionID);
                string str = lst.Name; //dr.Row["GRIDDEFINITIONNAME"].ToString();
                DialogResult rtn = frm.ShowDialog();
                if (rtn != DialogResult.OK) { return; }
                //返回属性
                else
                {
                    lstAvailableGrid.Items.Clear();
                    cboDefaultGridType.Items.Clear();
                    loadGrid();
                   
                }//if else
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

        }//method

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
                    lstAvailableGrid.Items.Add(new ListItem(ds.Tables[0].Rows[i]["GridDefinitionID"].ToString(),ds.Tables[0].Rows[i]["GridDefinitionName"].ToString()));
                    //cboDefaultGridType.Items.Add(new ListItem(ds.Tables[0].Rows[i]["GridDefinitionID"].ToString(), ds.Tables[0].Rows[i]["GridDefinitionName"].ToString()));
                    cboDefaultGridType.Items.Add(ds.Tables[0].Rows[i]["GridDefinitionName"].ToString());
                }
                commondtext = string.Format("select  GridDefinitionName from GridDefinitions where setupid={0} and defaultType=1", CommonClass.ManageSetup.SetupID);
                object defaultGrid = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commondtext);
                if (defaultGrid!= null)
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

            //if (IsEditor)
            //{
            //    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            //    string commandText = string.Format("select GRIDDEFINITIONID FROM SHAPEFILEGERIDEFINITIONDETAILS WHERE SHAPEFILENAME='{0}'", saveShapefilename);
            //    int currentDataSetID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));

            //    //System.Data.DataSet ds = new System.Data.DataSet();
            //    //DataWorker.DataParser dp = new DataWorker.DataParser();

            //    commandText = string.Format("Update GridDefinitions set GridDefinitionName='{0}',COLUMNS={1},RROWS={2},TTYPE={3} WHERE GRIDDFEINITIONID={4}", gridIDName, saveColumns, saveRRows, gridType, currentDataSetID);
            //    int i = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);//保存到QALYDATASETS

            //    switch (gridType)
            //    {
            //        case 1://shapefile
            //            //add to shapefilegriddefinitondetails
            //            commandText = string.Format("Update ShapefileGriddefinitiondetails set shapefilename='{0}' where griddefinitionid={1}", saveShapefilename, currentDataSetID);
            //            int j = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
            //            //move shapefile
            //            FeatureSet fs = new FeatureSet();
            //            fs.Open(saveShapefilepath);
            //            try
            //            {
            //                fs.SaveAs(Application.StartupPath + @"\Data\Shapefiles\" + saveShapefilename + ".shp", true);
            //            }
            //            finally
            //            {
            //                fs.Close();
            //            }
            //            break;
            //        case 0://regular
            //            //add to regulargriddefinitiondetail
            //            commandText = string.Format("Update regulargriddefinitiondetails set minimumlatitude={0},minimumlongitude={1},columnsperlongitude={2},rowsperlatitude={3} where griddefinitionid={4}", saveminLatitude,saveminLongitude,savecolPerLongitude,saverowPerLatitude, currentDataSetID);
            //            int t = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
            //            //move shapefile
            //            //FeatureSet fts = new FeatureSet();
            //            //fts.Open(saveShapefilepath);
            //            //try
            //            //{
            //            //    fts.SaveAs(Application.StartupPath + @"\Data\Shapefiles\" + saveShapefilename + ".shp", true);
            //            //}
            //            //finally
            //            //{
            //            //    fts.Close();
            //            //}

            //            break;
            //    }


            //}

            //else
            //{
            //    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            //    string commandText = string.Format("select next value for SEQ_GRIDDEFINITIONS FROM RDB$DATABASE");
            //    int currentDataSetID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));

            //    //System.Data.DataSet ds = new System.Data.DataSet();
            //    //DataWorker.DataParser dp = new DataWorker.DataParser();

            //    commandText = string.Format("INSERT INTO GRIDDEFINITIONS VALUES(" + currentDataSetID + ",{0},'{1}',{2},{3},{4})",CommonClass.ManageSetup.SetupID, gridIDName, saveColumns, saveRRows, gridType);
            //    int i = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);//保存到QALYDATASETS

            //    switch (gridType)
            //    {
            //        case 1://shapefile
            //            //add to shapefilegriddefinitondetails
            //            commandText = string.Format("INSERT INTO SHAPEFILEGRIDDEFINITIONDETAILS VALUES(" + currentDataSetID + ",'{0}')", saveShapefilename);
            //            int j = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
            //            //move shapefile
            //            FeatureSet fs = new FeatureSet();
            //            fs.Open(saveShapefilepath);
            //            try
            //            {
            //                fs.SaveAs(Application.StartupPath + @"\Data\Shapefiles\" + saveShapefilename + ".shp", true);
            //            }
            //            finally
            //            {
            //                fs.Close();
            //            }
            //            break;
            //        case 0://regular
            //            //add to regulargriddefinitiondetail
            //            commandText = string.Format("INSERT INTO regulargriddefinitiondetails VALUES({0},{1},{2},{3},{4})", currentDataSetID,saveminLatitude,saveminLongitude,savecolPerLongitude,saverowPerLatitude);
            //            int t = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
            //            break;
            //    }


            //}
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
                commandText = "select IncidenceDataSetID from IncidenceDataSets where GridDefinitionID="+_gridDefinitionID+" ";
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
                if (result ==DialogResult.Yes)
                {
                    try
                    {
                        
                        //commandText = "delete from GRIDDEFINITIONS where GRIDDEFINITIONNAME=@del";
                        ////string del = lstAvailableGrid.SelectedItem.ToString();
                        //FbParameter[] commandParameters = new FbParameter[] { new FbParameter("@del", lstAvailableGrid.GetItemText(lstAvailableGrid.SelectedItem)) };
                        //System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText, commandParameters);
                        commandText = "select ttype from GridDefinitions where GridDefinitionID=" + _gridDefinitionID + "";
                        int ttype = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));
                        commandText = "select ShapeFileName from " + (ttype == 1 ? "ShapefileGriddefinitiondetails" : "Regulargriddefinitiondetails") + " where GridDefinitionID=" + _gridDefinitionID + "";
                        object shapefilename = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                        CommonClass.DeleteShapeFileName(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.ManageSetup.SetupName + "\\" + shapefilename.ToString() + ".shp");
                        commandText = "delete from GridDefinitions where GridDefinitionID=" + _gridDefinitionID + "";
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        commandText = "delete from ShapefileGriddefinitiondetails where griddefinitionid=" + _gridDefinitionID + "";
                        fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
                        commandText = "delete from Regulargriddefinitiondetails where griddefinitionid="+_gridDefinitionID+"";
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
                    //_gridDefinitionID = ((lstAvailableGrid.SelectedItem) as DataRowView)["GridDefinitionID"];
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
            // 先全部改为0
            commandText = string.Format("update GRIDDEFINITIONS set DEFAULTTYPE=0 where setupid={0}",CommonClass.ManageSetup.SetupID);
            int rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
            //将设定的改为1
            commandText = string.Format("update GRIDDEFINITIONS set DEFAULTTYPE=1 where GRIDDEFINITIONNAME='{0}' and setupid={1}", cboDefaultGridType.SelectedItem,CommonClass.ManageSetup.SetupID);
            rth = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
        }

       
    }
}
