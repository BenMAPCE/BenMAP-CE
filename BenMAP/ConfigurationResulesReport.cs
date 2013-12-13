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
        //CRSelectFunctionCalculateValue crSelectFunctionCalculateValue = null;
        //public ConfigurationResultsReport(CRSelectFunctionCalculateValue cr)
        //{
        //    InitializeComponent();
        //    crSelectFunctionCalculateValue = cr;
        //}

        private void ConfigurationResultsReport_Load(object sender, EventArgs e)
        {
            try
            {
                //chklstGrid.Items.Add("Column");
                //chklstGrid.Items.Add("Row");
                //chklstHealth.Items.Add("DataSet"); chklstHealth.Items.Add("Endpoint Group"); chklstHealth.Items.Add("Endpoint");
                //chklstHealth.Items.Add("Pollutant"); chklstHealth.Items.Add("Metric"); chklstHealth.Items.Add("Seasonal Metric");
                //chklstHealth.Items.Add("Metric Statistic"); chklstHealth.Items.Add("Author"); chklstHealth.Items.Add("Year");
                //chklstHealth.Items.Add("Location"); chklstHealth.Items.Add("Other Pollutants"); chklstHealth.Items.Add("Qualifier");
                //chklstHealth.Items.Add("Reference"); chklstHealth.Items.Add("Race"); chklstHealth.Items.Add("Ethnicity");
                //chklstHealth.Items.Add("Gender"); chklstHealth.Items.Add("Start Age"); chklstHealth.Items.Add("End Age");
                //chklstHealth.Items.Add("Function"); chklstHealth.Items.Add("Incidence DataSet"); chklstHealth.Items.Add("Prevalence DataSet");
                //chklstHealth.Items.Add("Beta"); chklstHealth.Items.Add("Disbeta"); chklstHealth.Items.Add("P1Beta");
                //chklstHealth.Items.Add("P2Beta"); chklstHealth.Items.Add("A"); chklstHealth.Items.Add("NameA");
                //chklstHealth.Items.Add("B"); chklstHealth.Items.Add("NameB"); chklstHealth.Items.Add("C");
                //chklstHealth.Items.Add("NameC"); 
                ////chklstHealth.Items.Add("EJGroup"); chklstHealth.Items.Add("FunctionGroup");
                //chklstResult.Items.Add("Point Estimate");
                //chklstResult.Items.Add("Population");
                //chklstResult.Items.Add("Delta");
                //chklstResult.Items.Add("Mean");
                //chklstResult.Items.Add("Baseline");
                //chklstResult.Items.Add("Percent of Baseline");
                //chklstResult.Items.Add("Standard Deviation");
                //chklstResult.Items.Add("Variance");
                //chklstResult.Items.Add("Latin Hypercube Points");
                if (lstColumnRow == null)
                {
                    lstColumnRow = new List<FieldCheck>();
                    lstColumnRow.Add(new FieldCheck() { FieldName = "Column", isChecked = true });
                    lstColumnRow.Add(new FieldCheck() { FieldName = "Row", isChecked = true });
                    //dicColumnRow.Add("Row", true);

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
                    if (isPooledIncidence)
                    {
                        lstHealth.Add(new FieldCheck() { FieldName = "Version", isChecked = false });
                    }

                }
                if (lstResult == null)
                {
                    lstResult = new List<FieldCheck>();
                    lstResult.Add(new FieldCheck() { FieldName = "Point Estimate", isChecked = true });
                    //lstResult.Add(new FieldCheck() { FieldName = "Incidence", isChecked = true });
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
                    if (userAssignPercentile && sArrayPercentile!=null && sArrayPercentile.Count>0)
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
        //public ConfigurationResultsReportSet configurationResultsReportSet;
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
                        for (int i = 0; i < sArrayPercentiles.Count(); i++)//check
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

    //public class ConfigurationResultsReportSet
    //{
    //    bool Column=true;
    //    bool Row = true;
    //    bool DataSet;
    //    bool EndpointGroup;
    //    bool Endpoint;
    //    bool Pollutant;
    //    bool Metric;
    //    bool SeasonalMetric;
    //    bool MetricStatistic;
    //    bool Author;
    //    bool Year;
    //    bool Location;
    //    bool OtherPollutants;
    //    bool Qualifier;
    //    bool Reference;
    //    bool Race;
    //    bool Ethnicity;
    //    bool Gender;
    //    bool StartAge;
    //    bool EndAge;
    //    bool Function;
    //    bool IncidenceDataSet;
    //    bool PrevalenceDataSet;
    //    bool Beta;
    //    bool Disbeta;
    //    bool P1Beta;
    //    bool P2Beta;
    //    bool A;
    //    bool NameA;
    //    bool B;
    //    bool NameB;
    //    bool C;
    //    bool NameC;
    //    //bool EJGroup;
    //    //bool FunctionGroup;
    //    bool PointEstimate = true;
    //    bool Population = true;
    //    bool Delta = true;
    //    bool Mean = true;
    //    bool Baseline = true;
    //    bool PercentofBaseline = true;
    //    bool StandardDeviation = true;
    //    bool Variance = true;
    //    bool LatinHypercubePoints = true;

    //}
}
