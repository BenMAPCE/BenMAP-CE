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
    public partial class ConfigurationResultsReport : FormBase
    {
        public ConfigurationResultsReport()
        {
            InitializeComponent();
        }

        private void ConfigurationResultsReport_Load(object sender, EventArgs e)
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
                    lstHealth.Add(new FieldCheck() { FieldName = "Reference", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Race", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Ethnicity", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Gender", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Start Age", isChecked = true });
                    lstHealth.Add(new FieldCheck() { FieldName = "End Age", isChecked = true });
                    lstHealth.Add(new FieldCheck() { FieldName = "Function", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Incidence DataSet", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Prevalence DataSet", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Beta", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Disbeta", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "P1Beta", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "P2Beta", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "A", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "NameA", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "B", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "NameB", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "C", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "NameC", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Beta Variation", isChecked = false });
                    lstHealth.Add(new FieldCheck() { FieldName = "Model Specification", isChecked = false });

                    if (isPooledIncidence)
                    {
                        lstHealth.Add(new FieldCheck() { FieldName = "Version", isChecked = false });
                    }

                }
                if (lstResult == null)
                {
                    lstResult = new List<FieldCheck>();
                    lstResult.Add(new FieldCheck() { FieldName = "Point Estimate", isChecked = true });
                    lstResult.Add(new FieldCheck() { FieldName = "Population", isChecked = true });
                    lstResult.Add(new FieldCheck() { FieldName = "Delta", isChecked = true });
                    lstResult.Add(new FieldCheck() { FieldName = "Mean", isChecked = true });
                    lstResult.Add(new FieldCheck() { FieldName = "Baseline", isChecked = true });
                    lstResult.Add(new FieldCheck() { FieldName = "Percent of Baseline", isChecked = true });
                    lstResult.Add(new FieldCheck() { FieldName = "Standard Deviation", isChecked = true });
                    lstResult.Add(new FieldCheck() { FieldName = "Variance", isChecked = true });

                    if (!isPooledIncidence)
                    {
                        lstResult.Add(new FieldCheck() { FieldName = "Population Weighted Delta", isChecked = false });
                        lstResult.Add(new FieldCheck() { FieldName = "Population Weighted Base", isChecked = false });
                        lstResult.Add(new FieldCheck() { FieldName = "Population Weighted Control", isChecked = false });
                    }
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
                        if (CommonClass.CRLatinHypercubePoints == 10)
                        {
                            for (int i = 0; i < sArrayPercentile.Count(); i++)
                            {
                                for (int j = 0; j < chkLstPercentile10.Items.Count; j++)
                                {
                                    if (sArrayPercentile[i] == chkLstPercentile10.Items[j].ToString().Substring(10, chkLstPercentile10.Items[j].ToString().Length - 10))
                                    {
                                        chkLstPercentile10.SetItemChecked(j, true);
                                    }
                                }
                            }
                        }
                        else
                        {
                            string strp = "";
                            for (int i = 0; i < sArrayPercentile.Count(); i++)
                            {
                                strp += sArrayPercentile[i] + ",";
                            }
                            tbPercentiles.Text = strp.Substring(0, strp.Length - 1);
                        }
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
        CheckedListBox.CheckedItemCollection lstPercentile10;
        public List<string> sArrayPercentile;
        public bool userAssignPercentile;
        public bool isPooledIncidence;
        private void btnOK_Click(object sender, EventArgs e)
        {
            userAssignPercentile = cbPercentile.Checked;
            if (lstResult.Last().isChecked && cbPercentile.Checked && !CommonClass.CRRunInPointMode)
            {
                if (CommonClass.CRLatinHypercubePoints == 10)
                {
                    sArrayPercentile = new List<string>();
                    lstPercentile10 = chkLstPercentile10.CheckedItems;
                    for (int i = 0; i < lstPercentile10.Count; i++)
                    {
                        sArrayPercentile.Add(lstPercentile10[i].ToString().Substring(10, lstPercentile10[i].ToString().Length - 10));
                    }
                }
                else
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
                                switch (CommonClass.CRLatinHypercubePoints)
                                {
                                    case 10:
                                        if ((Convert.ToInt16(sArrayPercentiles[i].ToString()) - 5) % 10 != 0)
                                        {
                                            valid = false;
                                        }
                                        break;
                                    case 20:
                                        if ((Convert.ToDouble(sArrayPercentiles[i].ToString()) - 2.5) % 5 != 0)
                                        {
                                            valid = false;
                                        }
                                        break;
                                    case 50:
                                        if ((Convert.ToInt16(sArrayPercentiles[i].ToString()) - 1) % 2 != 0)
                                        {
                                            valid = false;
                                        }
                                        break;
                                    case 100:
                                        if ((Convert.ToDouble(sArrayPercentiles[i].ToString()) - 0.5) % 1 != 0)
                                        {
                                            valid = false;
                                        }
                                        break;
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
            }
            else
            {
                lstPercentile10 = null;
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
                if (CommonClass.CRLatinHypercubePoints == 10)
                {
                    this.chkLstPercentile10.Visible = true;
                    this.tbPercentiles.Visible = false;
                    this.lblInputTip.Visible = false;
                }
                else
                {
                    this.chkLstPercentile10.Visible = false;
                    this.tbPercentiles.Visible = true;
                    this.lblInputTip.Visible = true;
                }
            }
            else
            {
                this.tbPercentiles.Visible = false;
                this.lblInputTip.Visible = false;
                this.chkLstPercentile10.Visible = false;

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
                chkLstPercentile10.Visible = false;
                for (int i = 0; i < chkLstPercentile10.Items.Count; i++)
                {
                    chkLstPercentile10.SetItemChecked(i, false);
                }
                lstPercentile10 = null;
                sArrayPercentile = null;
                tbPercentiles.Text = string.Empty;
            }
        }

        private void tbPercentiles_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0' && e.KeyChar <= '9') || e.KeyChar == (char)8 || e.KeyChar == '.' || e.KeyChar == ',')
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }


}
