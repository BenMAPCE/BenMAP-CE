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
    public partial class EffectCoefficients : FormBase
    {
        private String betaVariation;
        public EffectCoefficients(String varSelected, List<CRFVariable> varList)
        {
            InitializeComponent();
            betaVariation = varSelected;
        }

        // Some of these fields will be filled dynamically
        // Hard coded values put in for tesing UI features 
        private void EffectCoefficients_Load(object sender, EventArgs e)
        {
            try
            {
                cboBetaDistribution.Items.Add("None");
                cboBetaDistribution.Items.Add("Normal");
                cboBetaDistribution.Items.Add("Triangular");
                cboBetaDistribution.Items.Add("Poisson");
                cboBetaDistribution.Items.Add("Binomial");
                cboBetaDistribution.Items.Add("LogNormal");
                cboBetaDistribution.Items.Add("Uniform");
                cboBetaDistribution.Items.Add("Exponential");
                cboBetaDistribution.Items.Add("Geometric");
                cboBetaDistribution.Items.Add("Weibull");
                cboBetaDistribution.Items.Add("Gamma");
                cboBetaDistribution.Items.Add("Logistic");
                cboBetaDistribution.Items.Add("Beta");
                cboBetaDistribution.Items.Add("Pareto");
                cboBetaDistribution.Items.Add("Cauchy");
                cboBetaDistribution.Items.Add("Custom");

                if (betaVariation == "Full Year")
                {
                    groupBox1.Text = "Full Year";
                    tbSeasMetric.Text = "None";
                    showForSeasonal.Visible = false;
                    panel2.Visible = true;
                    panel1.Visible = true;

                }

                else if (betaVariation == "Seasonal")
                {
                    groupBox1.Text = "Season 1";
                    tbSeasMetric.Text = "ColdWarm";
                    showForSeasonal.Visible = true;
                    panel2.Visible = false;
                    panel1.Visible = true;
                }

                else // Geographic
                {
                    panel1.Visible = false;
                }

                cboBetaDistribution.SelectedValueChanged -= cboBetaDistribution_SelectedValueChanged;
                cboBetaDistribution.SelectedValueChanged += cboBetaDistribution_SelectedValueChanged;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
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

        private void editVarBtn_Click(object sender, EventArgs e)
        {
            VarianceMulti form = new VarianceMulti();
            DialogResult res = form.ShowDialog();
        }

        // From HealthImpactFunctionDefinition.cs 
        public List<double> list = new List<double>();
        private void cboBetaDistribution_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                HealthImpact healthImpactValues = new HealthImpact();
                healthImpactValues.BetaDistribution = cboBetaDistribution.SelectedItem.ToString();
                healthImpactValues.Beta = txtBeta.Text;
                healthImpactValues.BetaParameter1 = txtBetaParameter1.Text;
                healthImpactValues.BetaParameter2 = txtBetaParameter2.Text;
                if (cboBetaDistribution.SelectedItem.ToString() == "None") { return; }
                if (cboBetaDistribution.SelectedItem.ToString() == "Custom")
                {
                    if (list.Count == 0)
                    {
                        CustomDistributionEntries frm = new CustomDistributionEntries();
                        DialogResult rtn = frm.ShowDialog();
                        if (rtn != DialogResult.OK) { return; }
                        list = frm.list;
                    }
                    else
                    {
                        CustomDistributionEntries frmCustom = new CustomDistributionEntries(list);
                        DialogResult rtnCustom = frmCustom.ShowDialog();
                        if (rtnCustom != DialogResult.OK) { return; }
                        list = frmCustom.list;
                    }
                }
                else
                {
                    EditDistributionValues frm = new EditDistributionValues(cboBetaDistribution.SelectedItem.ToString(), healthImpactValues);
                    DialogResult rtn = frm.ShowDialog();
                    if (rtn != DialogResult.OK) { return; }
                    txtBeta.Text = frm.MeanValue;
                    txtBetaParameter1.Text = frm.Parameter1;
                    txtBetaParameter2.Text = frm.Parameter2;
                    if (cboBetaDistribution.SelectedItem.ToString() == "Normal" || cboBetaDistribution.SelectedItem.ToString() == "Poisson" 
                        || cboBetaDistribution.SelectedItem.ToString() == "Exponential" || cboBetaDistribution.SelectedItem.ToString() == "Geometric")
                    {
                        txtBetaParameter2.Text = healthImpactValues.BetaParameter2;
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