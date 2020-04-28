using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BenMAP
{
	public partial class APVResultsReport : FormBase
	{
		public APVResultsReport()
		{
			InitializeComponent();
		}

		private void APVResultsReport_Load(object sender, EventArgs e)
		{
			try
			{
				if (lstColumnRow == null)
				{
					lstColumnRow = new List<FieldCheck>();
					lstColumnRow.Add(new FieldCheck() { FieldName = "Column", isChecked = true });
					lstColumnRow.Add(new FieldCheck() { FieldName = "Row", isChecked = true });
				}
				if (lstHealth == null)
				{
					lstHealth = new List<FieldCheck>();
					lstHealth.Add(new FieldCheck() { FieldName = "Name", isChecked = false });
					lstHealth.Add(new FieldCheck() { FieldName = "DataSet", isChecked = false });
					lstHealth.Add(new FieldCheck() { FieldName = "Endpoint Group", isChecked = false });
					lstHealth.Add(new FieldCheck() { FieldName = "Endpoint", isChecked = true });
					lstHealth.Add(new FieldCheck() { FieldName = "Pollutant", isChecked = false });
					lstHealth.Add(new FieldCheck() { FieldName = "Metric", isChecked = false });
					lstHealth.Add(new FieldCheck() { FieldName = "Seasonal Metric", isChecked = false });
					lstHealth.Add(new FieldCheck() { FieldName = "Metric Statistic", isChecked = false });
					lstHealth.Add(new FieldCheck() { FieldName = "Author", isChecked = true });
					lstHealth.Add(new FieldCheck() { FieldName = "Year", isChecked = false });
					lstHealth.Add(new FieldCheck() { FieldName = "Location", isChecked = false });
					lstHealth.Add(new FieldCheck() { FieldName = "Other Pollutants", isChecked = false });
					lstHealth.Add(new FieldCheck() { FieldName = "Qualifier", isChecked = false });
					lstHealth.Add(new FieldCheck() { FieldName = "Race", isChecked = false });
					lstHealth.Add(new FieldCheck() { FieldName = "Ethnicity", isChecked = false });
					lstHealth.Add(new FieldCheck() { FieldName = "Gender", isChecked = false });
					lstHealth.Add(new FieldCheck() { FieldName = "Start Age", isChecked = true });
					lstHealth.Add(new FieldCheck() { FieldName = "End Age", isChecked = true });
					lstHealth.Add(new FieldCheck() { FieldName = "Function", isChecked = false });
					lstHealth.Add(new FieldCheck() { FieldName = "Version", isChecked = false });

				}
				if (lstResult == null)
				{
					lstResult = new List<FieldCheck>();
					lstResult.Add(new FieldCheck() { FieldName = "Point Estimate", isChecked = true });
					lstResult.Add(new FieldCheck() { FieldName = "Mean", isChecked = true });
					lstResult.Add(new FieldCheck() { FieldName = "Standard Deviation", isChecked = true });
					lstResult.Add(new FieldCheck() { FieldName = "Variance", isChecked = true });
					lstResult.Add(new FieldCheck() { FieldName = "Percentiles", isChecked = true });

				}
				olvColumnRow.SetObjects(lstColumnRow);
				olvHealth.SetObjects(lstHealth);
				olvResult.SetObjects(lstResult);
				if (olvResult.Items[olvResult.Items.Count - 1].Checked && !CommonClass.CRRunInPointMode)
				{
					if (userAssignPercentile && sArrayPercentile != null && sArrayPercentile.Count > 0)
					{
						cbPercentile.Checked = true;
						string strp = "";
						for (int i = 0; i < sArrayPercentile.Count(); i++)
						{
							strp += sArrayPercentile[i] + ",";
						}
						tbPercentiles.Text = strp.Substring(0, strp.Length - 1);
					}
				}
				else
					cbPercentile.Enabled = false;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}
		public List<FieldCheck> lstColumnRow;
		public List<FieldCheck> lstHealth;
		public List<FieldCheck> lstResult;
		public List<string> sArrayPercentile;
		public bool userAssignPercentile;

		private void btnOK_Click(object sender, EventArgs e)
		{
			userAssignPercentile = cbPercentile.Checked;
			if (lstResult.Last().isChecked && cbPercentile.Checked && !CommonClass.CRRunInPointMode)
			{
				if (string.IsNullOrEmpty(tbPercentiles.Text))
				{
					sArrayPercentile = null;
				}
				else
				{
					string[] sArrayPercentiles = tbPercentiles.Text.Split(',');
					bool valid = true;
					for (int i = 0; i < sArrayPercentiles.Count(); i++)
					{
						if (sArrayPercentiles[i].ToString() == "") continue;
						try
						{
							if ((Convert.ToDouble(sArrayPercentiles[i].ToString()) - 0.5) % 1 != 0)
							{
								valid = false;
							}
						}
						catch
						{
							valid = false;
						}
						if (!valid || Convert.ToDouble(sArrayPercentiles[i].ToString()) <= 0 || Convert.ToDouble(sArrayPercentiles[i].ToString()) >= 100)
						{
							MessageBox.Show("Please input valid percentiles.", "Error", MessageBoxButtons.OK);
							return;
						}
					}

					sArrayPercentile = new List<string>();

					for (int i = 0; i < sArrayPercentiles.Count(); i++)
					{
						if (sArrayPercentiles[i].ToString() == "") continue;
						sArrayPercentile.Add(sArrayPercentiles[i].ToString());
					}
				}
			}
			else
			{
				sArrayPercentile = null;
			}

			this.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void cbPercentile_CheckedChanged(object sender, EventArgs e)
		{
			if (!lstResult.Last().isChecked)
			{
				cbPercentile.Checked = false;
				return;
			}
			if (lstResult.Last().isChecked && cbPercentile.Checked)
			{
				this.tbPercentiles.Visible = true;
				this.lblInputTip.Visible = true;
			}
			else
			{
				this.tbPercentiles.Visible = false;
				this.lblInputTip.Visible = false;
			}
		}

		private void olvResult_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			if (olvResult.Items[olvResult.Items.Count - 1].Checked)
			{
				cbPercentile.Enabled = true;
			}
			else if (!olvResult.Items[olvResult.Items.Count - 1].Checked)
			{
				cbPercentile.Checked = false;
				cbPercentile.Enabled = false;
				tbPercentiles.Visible = false;
				lblInputTip.Visible = false;
				sArrayPercentile = null;
				tbPercentiles.Text = string.Empty;
			}
		}

	}


}
