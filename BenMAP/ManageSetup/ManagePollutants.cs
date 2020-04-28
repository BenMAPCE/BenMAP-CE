using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;

namespace BenMAP
{
	public partial class ManagePollutants : FormBase
	{
		public ManagePollutants()
		{
			InitializeComponent();
		}

		private void ManagePollutants_Load(object sender, EventArgs e)
		{
			try
			{
				BindPollutants();
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void BindPollutants()
		{
			try
			{
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				string commandText = string.Format("select pollutantname,pollutantID from pollutants  where setupid={0} order by pollutantname asc", CommonClass.ManageSetup.SetupID);
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
				lstAvailablePollutants.DataSource = ds.Tables[0];
				lstAvailablePollutants.DisplayMember = "pollutantname";
				if (lstAvailablePollutants.Items.Count > 0)
				{
					lstAvailablePollutants.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}
		private void lstAvailablePollutants_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				string commandText = "select metricname from metrics where pollutantid=(select pollutantid from pollutants where pollutantname='" + lstAvailablePollutants.GetItemText(lstAvailablePollutants.SelectedItem) + "' and SetUpID=" + CommonClass.ManageSetup.SetupID + ")";
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
				lstPollutantMetrics.DataSource = ds.Tables[0];
				lstPollutantMetrics.DisplayMember = "metricname";
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			PollutantDefinition frm = new PollutantDefinition();
			DialogResult rtn = frm.ShowDialog();
			if (rtn == DialogResult.OK)
			{
				BindPollutants();
			}
		}

		private void btnEdit_Click(object sender, EventArgs e)
		{
			try
			{
				if (lstAvailablePollutants.SelectedItem == null) return;
				PollutantDefinition frm = new PollutantDefinition();
				DataRowView dr = lstAvailablePollutants.SelectedItem as DataRowView;
				frm._pollutantIDName = dr.Row["pollutantname"].ToString();
				frm._pollutantID = dr.Row["pollutantID"];
				DialogResult rtn = frm.ShowDialog();
				if (rtn == DialogResult.OK)
				{
					BindPollutants();
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			if (lstAvailablePollutants.SelectedItem == null) return;
			ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
			string commandText = string.Empty;
			try
			{
				object pollutantID = (lstAvailablePollutants.SelectedItem as DataRowView).Row["pollutantID"];
				string pollutantName = (lstAvailablePollutants.SelectedItem as DataRowView).Row["pollutantName"].ToString();
				commandText = "select MonitorID from Monitors where PollutantID=" + pollutantID + "";
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
				if (ds.Tables[0].Rows.Count != 0)
				{
					MessageBox.Show("The " + pollutantName + " pollutant is used in 'Monitor Datasets'. Please delete monitor datasets that use this pollutant first.");
					return;
				}
				commandText = "select CRFunctionID from CRFunctions where PollutantID=" + pollutantID + "";
				ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
				if (ds.Tables[0].Rows.Count != 0)
				{
					MessageBox.Show("The " + pollutantName + " pollutant is used in 'Health Impact Function Datasets'. Please delete health impact functions that use this pollutant first.");
					return;
				}
				commandText = "select MetricID from Metrics where PollutantID=" + pollutantID + "";
				ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
				string warning = "";
				if (ds.Tables[0].Rows.Count != 0)
				{
					warning = "All metrics associated with this pollutant will be deleted. ";
				}
				DialogResult rtn = MessageBox.Show(warning + "Are you sure you want to delete " + pollutantName + "?", "Confirm Delete", MessageBoxButtons.OKCancel);
				if (rtn == DialogResult.OK)
				{
					commandText = "delete from SeasonalMetricSeasons where SeasonalMetricID in (select SeasonalMetricID from SeasonalMetrics where MetricID in (select MetricID from Metrics where PollutantID=" + pollutantID + "))";
					fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
					commandText = "delete from SeasonalMetrics where MetricID in (select MetricID from Metrics where PollutantID=" + pollutantID + ")";
					fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
					commandText = "delete from Metrics where PollutantID=" + pollutantID + "";
					fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
					commandText = "delete from PollutantSeasons where PollutantID=" + pollutantID + "";
					fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
					commandText = "delete from Pollutants where PollutantID=" + pollutantID + "";
					fb.ExecuteNonQuery(CommonClass.Connection, new CommandType(), commandText);
				}
				BindPollutants();
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

		private void btnCopy_Click(object sender, EventArgs e)
		{
			// get current list item for copy
			object pollutantID = (lstAvailablePollutants.SelectedItem as DataRowView).Row["pollutantID"];
			string pollutantName = (lstAvailablePollutants.SelectedItem as DataRowView).Row["pollutantName"].ToString();

			Tools.InputBox myBox = new Tools.InputBox("Copy Pollutant " + pollutantName, "Enter New Pollutant Name", pollutantName + "_copy");
			DialogResult inputResult = myBox.ShowDialog();
			if (inputResult == DialogResult.OK)
			{
				// copy routine goes here
				CopyPollutant cp = new CopyPollutant();
				cp.Copy(int.Parse(pollutantID.ToString()), CommonClass.ManageSetup.SetupID, myBox.InputText);
				MessageBox.Show("Pollutant " + pollutantName + " was copied as " + myBox.InputText);
			}
			else if (inputResult == DialogResult.Cancel)
			{
				MessageBox.Show("Copy cancelled by user");
			}
			// reflect changes in GUI
			BindPollutants();
		}
	}
}
