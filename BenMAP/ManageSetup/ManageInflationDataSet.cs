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
	public partial class ManageInflationDataSet : FormBase
	{
		public ManageInflationDataSet()
		{
			InitializeComponent();

		}
		string _dataName = string.Empty;
		private int _datasetID;
		private int _datasetTypeID;
		private MetadataClassObj _metadataObj = null;

		private void btnOK_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			try
			{
				LoadInflationDataSet frm = new LoadInflationDataSet();
				frm.ShowDialog();
				ExportDataForlistbox();
				int count = -1;
				for (int i = 0; i < lstAvailableDataSets.Items.Count; i++)
				{
					DataRowView drv = lstAvailableDataSets.Items[i] as DataRowView;
					string datasetName = drv["INFLATIONDATASETNAME"].ToString();
					if (datasetName == frm.InflationDataSetName)
					{
						count = i;
						break;
					}
				}
				lstAvailableDataSets.SelectedIndex = count;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void ManageInflationDataSet_Load(object sender, EventArgs e)
		{
			try
			{
				ExportDataForlistbox();
				lstAvailableDataSets.ClearSelected();
				lstAvailableDataSets.SelectedIndex = -1;
				btnViewMetadata.Enabled = false;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}

		}
		private void ExportDataForlistbox()
		{
			try
			{
				string commandText = string.Format("select * from INFLATIONDATASETS where setupid={0} ", CommonClass.ManageSetup.SetupID);
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
				lstAvailableDataSets.DataSource = ds.Tables[0];
				lstAvailableDataSets.DisplayMember = "INFLATIONDATASETNAME";
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void lstAvailableDataSets_SelectedValueChanged(object sender, EventArgs e)
		{
			try
			{
				if (sender == null) { return; }
				var lst = sender as ListBox;
				if (lst.SelectedItem == null) return;
				DataRowView dr = lst.SelectedItem as DataRowView;
				string str = dr.Row["INFLATIONDATASETNAME"].ToString();
				string commandText = string.Format("select YYEAR,ALLGOODSINDEX,MEDICALCOSTINDEX,WAGEINDEX from INFLATIONENTRIES WHERE INFLATIONDATASETID in (select INFLATIONDATASETID from INFLATIONDATASETS where INFLATIONDATASETNAME='{0}' and setupid={1})  ORDER BY YYEAR ASC", str, CommonClass.ManageSetup.SetupID);
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
				olvData.DataSource = ds.Tables[0];
				commandText = string.Format("select INFLATIONDATASETID from INFLATIONDATASETS where INFLATIONDATASETNAME='{0}' and setupid={1}", str, CommonClass.ManageSetup.SetupID);
				_datasetID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
				_dataName = str;
				btnViewMetadata.Enabled = false;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			try
			{
				if (lstAvailableDataSets.SelectedItem == null) return;
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				string commandText = string.Empty;
				int infDstID = 0; //inflation Dataset ID
				int dstID = 0;//DataSetTypeID
				if (MessageBox.Show("Delete the selected inflation dataset?", "Confirm Deletion", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					commandText = string.Format("SELECT INFLATIONDATASETID FROM INFLATIONDATASETS WHERE INFLATIONDATASETNAME = '{0}' and SETUPID = {1}", lstAvailableDataSets.Text, CommonClass.ManageSetup.SetupID);
					infDstID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));
					commandText = "SELECT DATASETTYPEID FROM DATASETTYPES WHERE DATASETTYPENAME = 'Inflation'";
					dstID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText));

					commandText = string.Format("delete from Inflationdatasets where Inflationdatasetname='{0}' and setupid={1}", lstAvailableDataSets.Text, CommonClass.ManageSetup.SetupID);
					int i = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);

					commandText = string.Format("DELETE FROM METADATAINFORMATION WHERE SETUPID = {0} AND DATASETID = {1} AND DATASETTYPEID = {2}", CommonClass.ManageSetup.SetupID, infDstID, dstID);
					i = fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
				}
				commandText = string.Format("select * from INFLATIONDATASETS where SetupID={0}", CommonClass.ManageSetup.SetupID);
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
				lstAvailableDataSets.DataSource = ds.Tables[0];
				lstAvailableDataSets.DisplayMember = "INCOMEGROWTHADJDATASETNAME";
				if (ds.Tables[0].Rows.Count == 0)
				{
					olvData.ClearObjects();
				}
			}
			catch
			{ }
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void btnOutput_Click(object sender, EventArgs e)
		{
			try
			{
				SaveFileDialog saveFileDialog1 = new SaveFileDialog();
				saveFileDialog1.Filter = "CSV File|*.CSV";
				saveFileDialog1.InitialDirectory = CommonClass.ResultFilePath + @"\Data Files";
				saveFileDialog1.RestoreDirectory = true;
				if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
				{ return; }
				string fileName = saveFileDialog1.FileName;
				DataTable dtOut = new DataTable();

				dtOut.Columns.Add("Year", typeof(int));
				dtOut.Columns.Add("AllGoodsIndex", typeof(double));
				dtOut.Columns.Add("MedicalCostIndex", typeof(double));
				dtOut.Columns.Add("WageIndex", typeof(double));
				FireBirdHelperBase fb = new ESILFireBirdHelper();
				string commandText = string.Empty;
				int outputRowsNumber = 50;
				commandText = "select count(*) from INFLATIONENTRIES";
				int count = (int)fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText);
				if (count < outputRowsNumber) { outputRowsNumber = count; }
				commandText = string.Format("select first {0} YYear, AllGoodsIndex,MedicalCostIndex,WageIndex from INFLATIONENTRIES where inflationdatasetid in (select inflationdatasetid from inflationdatasets where setupid = 1)", outputRowsNumber);
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					DataRow newdr = dtOut.NewRow();
					newdr["Year"] = Convert.ToInt32(dr["YYear"]);
					newdr["AllGoodsIndex"] = Convert.ToDouble(dr["AllGoodsIndex"]);
					newdr["MedicalCostIndex"] = Convert.ToDouble(dr["MedicalCostIndex"]);
					newdr["WageIndex"] = Convert.ToDouble(dr["WageIndex"]);
					dtOut.Rows.Add(newdr);
				}
				CommonClass.SaveCSV(dtOut, fileName);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void btnViewMetadata_Click(object sender, EventArgs e)
		{
			_datasetTypeID = 3; // hardcoded for inflation dataset type
			_metadataObj = SQLStatementsCommonClass.getMetadata(_datasetID, CommonClass.ManageSetup.SetupID, _datasetTypeID);
			_metadataObj.SetupName = CommonClass.ManageSetup.SetupName;//_dataName;//_lstDataSetName;
			btnViewMetadata.Enabled = false;
			BrightIdeasSoftware.DataListView dlv = olvData as BrightIdeasSoftware.DataListView;
			dlv.SelectedItems.Clear();
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
