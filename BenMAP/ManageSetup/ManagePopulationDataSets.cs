using System;
using System.Data;
using System.Windows.Forms;

namespace BenMAP
{
    public partial class ManagePopulationDataSets : FormBase
    {
        string _dataName = string.Empty;

        public ManagePopulationDataSets()
        {
            InitializeComponent();
        }

        private int dataSetID;

        private void btnAdd_Click(object sender, EventArgs e)
        {
            LoadPopulationDataSet frm = new LoadPopulationDataSet();
            DialogResult rtn = frm.ShowDialog();
            if (rtn == DialogResult.OK)
            {
                BindControls();
            }
        }

        private void ManagePopulationDataSets_Load(object sender, EventArgs e)
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
                string commandText = string.Format("select PopulationDataSetName ,PopulationDataSetID from PopulationDataSets where setupid={0} and populationdatasetid<>37 order  by PopulationDataSetName asc", CommonClass.ManageSetup.SetupID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstAvailableDataSetsName.DataSource = ds.Tables[0];
                lstAvailableDataSetsName.DisplayMember = "PopulationDataSetName";
                if (ds.Tables[0].Rows.Count == 0)
                {
                    olvPopulationValues.ClearObjects();
                }
                else
                {
                    lstAvailableDataSetsName.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        private void lstAvailableDataSetsName_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender == null) { return; }
                var lst = sender as ListBox;
                if (lst.SelectedItem == null) return;
                DataRowView drv = lst.SelectedItem as DataRowView;
                dataSetID = Convert.ToInt32(drv[1]);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select GridDefinitionName from GridDefinitions as GD join PopulationDataSets as PD on (GD.GridDefinitionID=PD.GridDefinitionID) where PopulationDataSetID={0}", dataSetID);
                txtGridDefinition.Text = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText).ToString();
                commandText = string.Format("select PopulationConfigurationName from PopulationConfigurations as PC join PopulationDataSets as PD on (PC.PopulationConfigurationID=PD.PopulationConfigurationID) where PopulationDataSetID={0}", dataSetID);
                txtPopulationConfig.Text = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText).ToString();
                commandText = string.Format("select first 100 Races.RaceName,Ethnicity.EthnicityName,Genders.GenderName,AgeRanges.AgeRangeName,PopulationEntries.CColumn,PopulationEntries.Row,PopulationEntries.VValue from Races,Ethnicity,Genders,AgeRanges,PopulationEntries,PopulationDataSets where (PopulationEntries.RaceID=Races.RaceID) and (PopulationEntries.EthnicityID=Ethnicity.EthnicityID) and (PopulationEntries.GenderID=Genders.GenderID) and (PopulationEntries.AgeRangeID=AgeRanges.AgeRangeID) and (PopulationEntries.PopulationDataSetID=PopulationDataSets.PopulationDataSetID) and PopulationDataSets.PopulationDataSetID={0} ", dataSetID);
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                olvPopulationValues.DataSource = ds.Tables[0];

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
                if (lstAvailableDataSetsName.SelectedItem == null) return;
                string msg = string.Format("Delete the selected population dataset?", lstAvailableDataSetsName.GetItemText(lstAvailableDataSetsName.SelectedItem));
                DialogResult result = MessageBox.Show(msg, "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    string commandText = "delete from PopulationEntries where PopulationDataSetID=" + dataSetID + "";
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    commandText = "delete from PopulationGrowthWeights where PopulationDataSetID=" + dataSetID + "";
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    commandText = "delete from PopulationDataSets where PopulationDataSetID=" + dataSetID + "";
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                    commandText = "delete from t_populationDatasetIDYear where PopulationDataSetID=" + dataSetID + "";
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

                    BindControls();
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
    }
}