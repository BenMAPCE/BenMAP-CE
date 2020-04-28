using System;
using System.Data;
using System.Windows.Forms;

namespace BenMAP
{
	public partial class ManagePopulationDataSets : FormBase
	{
		string _dataName = string.Empty;
		private MetadataClassObj _metadataObj = null;
		private int _dsMetadataID;
		private int _dsSetupID;
		private int _dataSetID;
		private int _dsDatasetTypeId;

		public ManagePopulationDataSets()
		{
			InitializeComponent();
		}



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
				Configuration.ConfigurationCommonClass.resizeListBoxHorizontalExtent(lstAvailableDataSetsName, "PopulationDataSetName");
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
				_dataSetID = Convert.ToInt32(drv[1]);
				_dataName = drv[0].ToString();
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				string commandText = string.Format("select GridDefinitionName from GridDefinitions as GD join PopulationDataSets as PD on (GD.GridDefinitionID=PD.GridDefinitionID) where PopulationDataSetID={0}", _dataSetID);
				txtGridDefinition.Text = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText).ToString();
				commandText = string.Format("select PopulationConfigurationName from PopulationConfigurations as PC join PopulationDataSets as PD on (PC.PopulationConfigurationID=PD.PopulationConfigurationID) where PopulationDataSetID={0}", _dataSetID);
				txtPopulationConfig.Text = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText).ToString();
				commandText = string.Format("select first 100 Races.RaceName,Ethnicity.EthnicityName,Genders.GenderName,AgeRanges.AgeRangeName,PopulationEntries.CColumn,PopulationEntries.Row,PopulationEntries.VValue from Races,Ethnicity,Genders,AgeRanges,PopulationEntries,PopulationDataSets where (PopulationEntries.RaceID=Races.RaceID) and (PopulationEntries.EthnicityID=Ethnicity.EthnicityID) and (PopulationEntries.GenderID=Genders.GenderID) and (PopulationEntries.AgeRangeID=AgeRanges.AgeRangeID) and (PopulationEntries.PopulationDataSetID=PopulationDataSets.PopulationDataSetID) and PopulationDataSets.PopulationDataSetID={0} ", _dataSetID);
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);


				commandText = string.Format("select POPULATIONDATASETID from POPULATIONDATASETS where POPULATIONDATASETNAME='{0}' and SETUPID={1}", _dataName, CommonClass.ManageSetup.SetupID);
				_dataSetID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
				_dsSetupID = CommonClass.ManageSetup.SetupID;
				_dsDatasetTypeId = SQLStatementsCommonClass.getDatasetID("Population");
				commandText = string.Format("Select METADATAENTRYID from METADATAINFORMATION where DATASETID = {0} and SETUPID = {1} and DATASETTYPEID = {2}", _dataSetID, _dsSetupID, _dsDatasetTypeId);
				_dsMetadataID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));


				olvPopulationValues.DataSource = ds.Tables[0];

				btnViewMetadata.Enabled = false;

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
				string populationDatasetName = lstAvailableDataSetsName.GetItemText(lstAvailableDataSetsName.SelectedItem).ToString();
				string msg = string.Format("Delete the selected population dataset?", populationDatasetName);//lstAvailableDataSetsName.GetItemText(lstAvailableDataSetsName.SelectedItem));
				int popDstID = 0; //Population Dataset ID
				int dstID = 0;//DataSetTypeID
				string commandText = string.Empty;
				DialogResult result = MessageBox.Show(msg, "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (result == DialogResult.Yes)
				{
					commandText = string.Format("SELECT POPULATIONDATASETID FROM POPULATIONDATASETS WHERE POPULATIONDATASETNAME = '{0}' and SETUPID = {1}", populationDatasetName, CommonClass.ManageSetup.SetupID);
					popDstID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));
					commandText = "SELECT DATASETTYPEID FROM DATASETTYPES WHERE DATASETTYPENAME = 'Population'";
					dstID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));

					commandText = "delete from PopulationEntries where PopulationDataSetID=" + _dataSetID + "";
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
					commandText = "delete from PopulationGrowthWeights where PopulationDataSetID=" + _dataSetID + "";
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
					commandText = "delete from PopulationDataSets where PopulationDataSetID=" + _dataSetID + "";
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
					commandText = "delete from t_populationDatasetIDYear where PopulationDataSetID=" + _dataSetID + "";
					fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

					commandText = string.Format("DELETE FROM METADATAINFORMATION WHERE SETUPID = {0} AND DATASETID = {1} AND DATASETTYPEID = {2}", CommonClass.ManageSetup.SetupID, popDstID, dstID);
					fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

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

		private void btnViewMetadata_Click(object sender, EventArgs e)
		{
			_metadataObj = SQLStatementsCommonClass.getMetadata(_dataSetID, _dsSetupID, _dsDatasetTypeId, _dsMetadataID);
			_metadataObj.SetupName = CommonClass.ManageSetup.SetupName;
			btnViewMetadata.Enabled = false;
			ViewEditMetadata viewEMdata = new ViewEditMetadata(_metadataObj);
			DialogResult dr = viewEMdata.ShowDialog();
			if (dr.Equals(DialogResult.OK))
			{
				_metadataObj = viewEMdata.MetadataObj;
			}
		}

		private void olvPopulationValues_SelectedIndexChanged(object sender, EventArgs e)
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
	}
}