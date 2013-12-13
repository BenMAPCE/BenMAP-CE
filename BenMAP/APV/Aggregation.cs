using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BenMAP.APVX
{
    public partial class Aggregation : FormBase
    {
        public Aggregation()
        {
            InitializeComponent();
        }
        private DataSet dsGrid;
        private void Aggregation_Load(object sender, EventArgs e)
        {
            try
            {

                BindGridtype();
                cboIncidenceAggregation.DataSource = dsGrid.Tables[0];
                cboIncidenceAggregation.DisplayMember = "GridDefinitionName";
                cboIncidenceAggregation.SelectedIndex = 0;
                BindGridtype();
                cboValuationAggregation.DataSource = dsGrid.Tables[0];
                cboValuationAggregation.DisplayMember = "GridDefinitionName";
                cboValuationAggregation.SelectedIndex = 0;
                BindGridtype();
                cboQALYAggregation.DataSource = dsGrid.Tables[0];
                cboQALYAggregation.DisplayMember = "GridDefinitionName";
                cboQALYAggregation.SelectedIndex = 0;
                int i = 0;
                if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation != null)
                {
                    i = 0;
                    foreach (DataRowView drv in cboIncidenceAggregation.Items)
                    {
                        if (Convert.ToInt32(drv["GridDefinitionID"]) == CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID)
                        {
                            cboIncidenceAggregation.SelectedIndex = i;
                        }
                        i++;
                    }
                }
                if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null)
                {
                    i = 0;
                    foreach (DataRowView drv in cboValuationAggregation.Items)
                    {
                        if (Convert.ToInt32(drv["GridDefinitionID"]) == CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID)
                        {
                            cboValuationAggregation.SelectedIndex = i;
                        }
                        i++;
                    }
                }
                if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation != null)
                {
                    i = 0;
                    foreach (DataRowView drv in cboQALYAggregation.Items)
                    {
                        if (Convert.ToInt32(drv["GridDefinitionID"]) == CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID)
                        {
                            cboQALYAggregation.SelectedIndex = i;
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            
        }
        private void BindGridtype()
        {
            try
            {
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               string commandText = string.Format("select -1 as  GridDefinitionID, '' as GridDefinitionName from GridDefinitions union select distinct GridDefinitionID,GridDefinitionName from griddefinitions where SetupID={0} ", CommonClass.MainSetup.SetupID);
                dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
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
    }
}
