using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESIL.DBUtility;

namespace BenMAP
{
    public partial class GridType : FormBase
    {
        public GridType()
        {
            InitializeComponent();
        }
        bool ok = false;
        private void GridType_Load(object sender, EventArgs e)
        {
            FireBirdHelperBase fb = new ESILFireBirdHelper();
            try
            {
                string commandText = string.Format("select * from GridDefinitions where setupid={0} order by GridDefinitionName asc", CommonClass.MainSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                // 必须这样写，否则会出错


                DataTable dtGrid = ds.Tables[0].Clone();
                dtGrid = ds.Tables[0].Copy();
                cboGrid.DataSource = dtGrid;
                cboGrid.DisplayMember = "GridDefinitionName";
                for (int i = 0; i < dtGrid.Rows.Count; i++)
                {
                    if (dtGrid.Rows[i]["defaulttype"].ToString() == "1")
                    {
                        cboGrid.SelectedIndex = i;
                        break;
                    }

                }
                commandText = string.Format("select * from GridDefinitions where columns<=56 and setupid={0}  order by GridDefinitionName desc", CommonClass.MainSetup.SetupID);
                DataSet dsregion = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                DataTable dtRegion = dsregion.Tables[0];
                cboRegion.DataSource = dtRegion;
                cboRegion.DisplayMember = "GridDefinitionName";
                if (CommonClass.GBenMAPGrid != null) { cboGrid.Text = CommonClass.GBenMAPGrid.GridDefinitionName; }
                if (CommonClass.RBenMAPGrid != null) { cboRegion.Text = CommonClass.RBenMAPGrid.GridDefinitionName; }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                DataRowView drGrid = cboGrid.SelectedItem as DataRowView;
                string str = "";
                if (CommonClass.GBenMAPGrid != null && CommonClass.GBenMAPGrid.GridDefinitionID != Convert.ToInt32(drGrid["GridDefinitionID"]))// != Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"])))// GetBenMapGridDefinitions(drGrid);
                {
                    //------------keneng you wenti
                    if (CommonClass.LstAsynchronizationStates != null && CommonClass.LstAsynchronizationStates.Count > 0)
                    {

                        MessageBox.Show(string.Format("Grid type can not be changed while baseline or control air quality grids are beig generated."), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;

                    }
                    DialogResult result = MessageBox.Show(string.Format("Change the grid type from \'{0}\' to '{1}' ? ", CommonClass.GBenMAPGrid.GridDefinitionName, drGrid["GridDefinitionName"]), "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result != DialogResult.Yes) { return; }
                    CommonClass.GBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));// GetBenMapGridDefinitions(drGrid);
                    //----------
                    if (CommonClass.LstBaseControlGroup != null)
                    {
                        foreach (BaseControlGroup bcg in CommonClass.LstBaseControlGroup)
                        {
                            bcg.GridType = CommonClass.GBenMAPGrid;
                            bcg.Base = null;
                            bcg.Control = null;
                            //CommonClass.LstBaseControlGroup.Clear();
                            //CommonClass.BaseControlCRSelectFunction = null;
                            //CommonClass.BaseControlCRSelectFunctionCalculateValue = null;
                            //CommonClass.lstIncidencePoolingAndAggregation = null;
                            ////CommonClass.IncidencePoolingAndAggregationAdvance = null;
                            //CommonClass.IncidencePoolingResult = null;
                            //CommonClass.ValuationMethodPoolingAndAggregation = null;

                        }
                    }


                }
                CommonClass.GBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drGrid["GridDefinitionID"]));
                DataRowView drRegion = cboRegion.SelectedItem as DataRowView;
                CommonClass.RBenMAPGrid = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drRegion["GridDefinitionID"]));// GetBenMapGridDefinitions(drRegion);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            //this.Close();
        }

        //private BenMAPGrid GetBenMapGridDefinitions(DataRowView dr)
        //{
        //    BenMAPGrid gridDef;
        //    try
        //    {
        //        gridDef = new BenMAPGrid()
        //        {
        //            GridDefinitionID = int.Parse(dr["GridDefinitionID"].ToString()),
        //            GridDefinitionName = dr["GridDefinitionName"].ToString(),
        //            SetupID = int.Parse(dr["SetupID"].ToString()),
        //            Columns = int.Parse(dr["Columns"].ToString()),
        //            RRows = int.Parse(dr["RRows"].ToString()),
        //        };
        //        switch (int.Parse(dr["TType"].ToString()))
        //        {
        //            case 0:
        //                gridDef.TType = GridTypeEnum.Regular;
        //                break;
        //            case 1:
        //                gridDef.TType = GridTypeEnum.Shapefile;
        //                break;
        //        }
        //        return gridDef;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex);
        //        return null;
        //    }
        //}

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            //this.Close();
        }

        private void cboGrid_SelectedValueChanged(object sender, EventArgs e)
        {

        }

    }
}
