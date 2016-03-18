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
        private int selected;
        private int selectedSeason;
        private bool seasonal;

        private HealthImpact _hif;
        public HealthImpact HIF
        {
            get { return _hif; }
            set { _hif = value; }
        }

        public EffectCoefficients(HealthImpact hif, int sel)
        {
            InitializeComponent();
            _hif = hif.DeepCopy();
            selected = sel;
            
            if (_hif.BetaVariation == "Seasonal") seasonal = true;
            else seasonal = false;
        }
 
        private void EffectCoefficients_Load(object sender, EventArgs e)
        {
            try
            {
                // BindItems();
                CRFVariable selectedVariable = _hif.PollVariables.ElementAt(selected);
                txtVariable.Text = selectedVariable.VariableName;
                txtPollutant.Text = selectedVariable.PollutantName;
                txtModelSpec.Text = _hif.ModelSpec;
                txtSeasMetric.Text = _hif.SeasonalMetric;

                // cboBetaDistribution.Items.Add("None");
                cboBetaDistribution.Items.Add("Normal");
                /* cboBetaDistribution.Items.Add("Triangular");
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
                cboBetaDistribution.Items.Add("Custom"); */

                cboBetaDistribution.SelectedIndex = 0;
                selectedSeason = 0;

                if (_hif.PollVariables.Count > 1)
                {
                    hideForMulti.Visible = false;
                    editVarBtn.Visible = true;
                    prevBtn.Visible = true;
                    nextBtn.Visible = true;

                    if (seasonal) { showForSeasonal.Visible = true; showForSeasonal2.Visible = true; }
                    else { showForSeasonal.Visible = false; showForSeasonal2.Visible = false; }
                }

                else if (_hif.PollVariables.Count == 1)
                {
                    hideForMulti.Visible = true;
                    editVarBtn.Visible = false;
                    prevBtn.Visible = false;
                    nextBtn.Visible = false;

                    if (seasonal) { showForSeasonal.Visible = true; showForSeasonal2.Visible = true; }
                    else { showForSeasonal.Visible = false; showForSeasonal2.Visible = false; }
                }

                else
                { 
                    MessageBox.Show("Pollutant variable list is empty");
                    return;
                }

                if (seasonal)
                {
                    foreach (var pb in selectedVariable.PollBetas)
                    {
                        cboSeason.Items.Add(pb.SeasNumName);
                    }
                }

                loadVariable();
                loadMetrics();

                cboBetaDistribution.SelectedValueChanged -= cboBetaDistribution_SelectedValueChanged;
                cboBetaDistribution.SelectedValueChanged += cboBetaDistribution_SelectedValueChanged;

                cboSeason.SelectionChangeCommitted -= cboSeason_SelectedValueChanged;
                cboSeason.SelectionChangeCommitted += cboSeason_SelectedValueChanged;
            }

            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void nextBtn_Click(object sender, EventArgs e)
        {
            saveCurrent(cboSeason.SelectedIndex);

            // Set form for next 
            if (selected + 1 > _hif.PollVariables.Count() - 1) { selected = 0; }
            else { selected++; }

            selectedSeason = 0;
            loadVariable();
            loadMetrics();
            cboSeason.SelectedIndex = 0;
        }

        private void prevBtn_Click(object sender, EventArgs e)
        {
            saveCurrent(cboSeason.SelectedIndex);

            // Set form for previous
            if (selected - 1 < 0) selected = _hif.PollVariables.Count() - 1;
            else { selected--; }

            selectedSeason = 0;
            loadVariable();
            loadMetrics();
            cboSeason.SelectedIndex = 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            saveCurrent(cboSeason.SelectedIndex);
            this.DialogResult = DialogResult.OK;
        }

        private void saveCurrent(int seasonInd)
        {
            if (txtBeta.Text == string.Empty)
            {
                MessageBox.Show("'Beta' can not be null. Please input a valid value.");
                return;
            }

            if(txtAconstantValue.Text == string.Empty)
            {
                MessageBox.Show("'A' can not be null. Please input a valid value.");
                return;
            }

            if (txtBconstantValue.Text == string.Empty)
            {
                MessageBox.Show("'B' can not be null. Please input a valid value.");
                return;
            }

            if (txtCconstantValue.Text == string.Empty)
            {
                MessageBox.Show("'C' can not be null. Please input a valid value.");
                return;
            }

            if (txtBetaParameter1.Visible && txtBetaParameter2.Visible)
            {
                if(txtBetaParameter1.Text == string.Empty)
                {
                    MessageBox.Show("'Beta Parameter 1' can not be null. Please input a valid value.");
                    return;
                }

                if (txtBetaParameter2.Text == string.Empty)
                {
                    MessageBox.Show("'Beta Parameter 2' can not be null. Please input a valid value.");
                    return;
                }

                _hif.PollVariables.ElementAt(selected).PollBetas[seasonInd].P1Beta = Convert.ToDouble(txtBetaParameter1.Text);
                _hif.PollVariables.ElementAt(selected).PollBetas[seasonInd].P2Beta = Convert.ToDouble(txtBetaParameter2.Text);
            }

            _hif.PollVariables.ElementAt(selected).PollBetas[seasonInd].Beta = Convert.ToDouble(txtBeta.Text);
            _hif.PollVariables.ElementAt(selected).PollBetas[seasonInd].AConstantName = txtAconstantDescription.Text.ToString();
            _hif.PollVariables.ElementAt(selected).PollBetas[seasonInd].BConstantName = txtBconstantDescription.Text.ToString();
            _hif.PollVariables.ElementAt(selected).PollBetas[seasonInd].CConstantName = txtCconstantDescription.Text.ToString();
            _hif.PollVariables.ElementAt(selected).PollBetas[seasonInd].AConstantValue = Convert.ToDouble(txtAconstantValue.Text);
            _hif.PollVariables.ElementAt(selected).PollBetas[seasonInd].BConstantValue = Convert.ToDouble(txtBconstantValue.Text);
            _hif.PollVariables.ElementAt(selected).PollBetas[seasonInd].CConstantValue = Convert.ToDouble(txtCconstantValue.Text);
            _hif.PollVariables.ElementAt(selected).PollBetas[seasonInd].Distribution = cboBetaDistribution.Text.ToString();
        }

        private void loadVariable()
        {
            try
            {
                CRFVariable selectedVariable = _hif.PollVariables.ElementAt(selected);
                txtVariable.Text = selectedVariable.VariableName;
                txtPollutant.Text = selectedVariable.PollutantName;
                txtModelSpec.Text = _hif.ModelSpec;

                if (seasonal)
                {
                    cboSeason.SelectedIndex = selectedSeason;

                    txtSeason.Text = selectedVariable.PollBetas[selectedSeason].SeasonName;
                    txtStart.Text = selectedVariable.PollBetas[selectedSeason].StartDate;
                    txtEnd.Text = selectedVariable.PollBetas[selectedSeason].EndDate;
                }

                txtAconstantDescription.Text = selectedVariable.PollBetas[selectedSeason].AConstantName;
                txtBconstantDescription.Text = selectedVariable.PollBetas[selectedSeason].BConstantName;
                txtCconstantDescription.Text = selectedVariable.PollBetas[selectedSeason].CConstantName;
                txtAconstantValue.Text = selectedVariable.PollBetas[selectedSeason].AConstantValue.ToString();
                txtBconstantValue.Text = selectedVariable.PollBetas[selectedSeason].BConstantValue.ToString();
                txtCconstantValue.Text = selectedVariable.PollBetas[selectedSeason].CConstantValue.ToString();
                txtBeta.Text = selectedVariable.PollBetas[selectedSeason].Beta.ToString();

                if (txtBetaParameter1.Visible && txtBetaParameter2.Visible)
                {
                    txtBetaParameter1.Text = selectedVariable.PollBetas[selectedSeason].P1Beta.ToString();
                    txtBetaParameter2.Text = selectedVariable.PollBetas[selectedSeason].P2Beta.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void loadMetrics()
        {
            try
            {
                CRFVariable selectedVariable = _hif.PollVariables.ElementAt(selected);

                if(selectedVariable.PollutantName.Contains("*"))
                {
                    cboMetric.DataSource = null;
                    cboMetric.Items.Clear();
                    cboMetric.Enabled = false;
                }
                else
                {
                    cboMetric.Enabled = true;
                    string commandText = string.Format("select METRICNAME from METRICS where METRICS.POLLUTANTID={0}", selectedVariable.Pollutant1ID);
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    cboMetric.DataSource = ds.Tables[0];
                    cboMetric.DisplayMember = "METRICNAME";
                    cboMetric.Text = selectedVariable.Metric;
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Warning to be used once editing is enabled
            /* DialogResult warn = MessageBox.Show("Pressing cancel will discard the changes on the current variable. Changes made on other variable values will still be saved.","Warning",MessageBoxButtons.OKCancel);
            if (warn == DialogResult.Cancel) return; */

            this.DialogResult = DialogResult.Cancel;
        }

        private void editVarBtn_Click(object sender, EventArgs e)
        {
            VarianceMulti form = new VarianceMulti(_hif, _hif.PollVariables[selected], selectedSeason);
            DialogResult res = form.ShowDialog();
        }

        private void cboSeason_SelectedValueChanged(object sender, EventArgs e)
        {
            saveCurrent(selectedSeason);
            selectedSeason = cboSeason.SelectedIndex;
            loadVariable();
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
 