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
        private int selectedVarIdx;
        private int selectedSeasonIdx;
        private bool isSeasonal;

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
            selectedVarIdx = sel;
            selectedSeasonIdx = 0;

            // TODO: Temporary override for testing
            //if (false && _hif.BetaVariation == "Seasonal")
            if (_hif.BetaVariation == "Seasonal")
            {
                isSeasonal = true;
            }
            else
            {
                isSeasonal = false;
            }

        }
 
        private void EffectCoefficients_Load(object sender, EventArgs e)
        {
            try
            {
                CRFVariable selectedVariable = _hif.PollVariables.ElementAt(selectedVarIdx);
                txtVariable.Text = selectedVariable.VariableName;
                txtPollutant.Text = selectedVariable.PollutantName;
                txtModelSpec.Text = _hif.ModelSpec;
                txtSeasMetric.Text = _hif.SeasonalMetric;
                if(selectedVariable.Metric.MetricName != null)
                {
                    cboMetric.Text = selectedVariable.Metric.MetricName;
                }

                // multipollutant locked to normal per epa's request
                // check that function ID is there first (not there for new functions)
                string dataset = null;
                if (_hif.FunctionID != string.Empty || _hif.FunctionID.Length > 0)
                    dataset = Configuration.ConfigurationCommonClass.getDatasetNameFromFunctionID(Convert.ToInt32(_hif.FunctionID));

                if(dataset != null && dataset.ToLower().Contains("multi"))
                {
                    cboBetaDistribution.Items.Add("Normal");
                    cboBetaDistribution.SelectedText = "Normal";
                }
                else
                {
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    string commandText = "select DISTRIBUTIONNAME from DISTRIBUTIONTYPES order by DISTRIBUTIONNAME";
                    DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        cboBetaDistribution.Items.Add(dr["distributionname"].ToString());
                    }
                }

                cboBetaDistribution.SelectedIndex = 0;
                selectedSeasonIdx = 0;

                if (_hif.PollVariables.Count > 1)
                {
                    hideForMulti.Visible = false;
                    editVarBtn.Visible = true;
                    prevBtn.Visible = true;
                    nextBtn.Visible = true;

                    if (isSeasonal) { showForSeasonal.Visible = true; showForSeasonal2.Visible = true; }
                    else { showForSeasonal.Visible = false; showForSeasonal2.Visible = false; }
                }

                else if (_hif.PollVariables.Count == 1)
                {
                    hideForMulti.Visible = true;
                    editVarBtn.Visible = false;
                    prevBtn.Visible = false;
                    nextBtn.Visible = false;

                    if (isSeasonal) { showForSeasonal.Visible = true; showForSeasonal2.Visible = true; }
                    else { showForSeasonal.Visible = false; showForSeasonal2.Visible = false; }
                }

                else
                { 
                    MessageBox.Show("Pollutant variable list is empty");
                    return;
                }

                if (isSeasonal)
                {
                    cboSeason.Items.Clear();
                    foreach (var pb in selectedVariable.PollBetas)
                    {
                        cboSeason.Items.Add(pb.SeasNumName);
                    }
                }

                loadVariable();
                loadMetrics();

                if(selectedVariable.PollBetas[selectedSeasonIdx].Distribution == string.Empty || selectedVariable.PollBetas[selectedSeasonIdx].Distribution == "None")
                {
                    cboBetaDistribution.SelectedIndex = cboBetaDistribution.FindString("None");
                }
                else
                {
                    cboBetaDistribution.SelectedIndex = cboBetaDistribution.FindString(selectedVariable.PollBetas[selectedSeasonIdx].Distribution);
                }

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
            if (selectedVarIdx + 1 > _hif.PollVariables.Count() - 1) { selectedVarIdx = 0; }
            else { selectedVarIdx++; }

            selectedSeasonIdx = 0;
            loadVariable();
            loadMetrics();
            cboSeason.SelectedIndex = 0;
        }

        private void prevBtn_Click(object sender, EventArgs e)
        {
            saveCurrent(cboSeason.SelectedIndex);

            // Set form for previous
            if (selectedVarIdx - 1 < 0) selectedVarIdx = _hif.PollVariables.Count() - 1;
            else { selectedVarIdx--; }

            selectedSeasonIdx = 0;
            loadVariable();
            loadMetrics();
            cboSeason.SelectedIndex = 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            saveCurrent(selectedSeasonIdx);
            this.DialogResult = DialogResult.OK;
            cboSeason.Items.Clear();
        }

        private void saveCurrent(int seasonInd)
        {
            try
            {
                if (txtBeta.Text == string.Empty)
                {
                    MessageBox.Show("'Beta' can not be null. Please input a valid value.");
                    return;
                }

                if (txtAconstantValue.Text == string.Empty)
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
                    if (txtBetaParameter1.Text == string.Empty)
                    {
                        MessageBox.Show("'Beta Parameter 1' can not be null. Please input a valid value.");
                        return;
                    }

                    if (txtBetaParameter2.Text == string.Empty)
                    {
                        MessageBox.Show("'Beta Parameter 2' can not be null. Please input a valid value.");
                        return;
                    }

                    _hif.PollVariables.ElementAt(selectedVarIdx).PollBetas[seasonInd].P1Beta = Convert.ToDouble(txtBetaParameter1.Text);
                    _hif.PollVariables.ElementAt(selectedVarIdx).PollBetas[seasonInd].P2Beta = Convert.ToDouble(txtBetaParameter2.Text);
                }

                _hif.PollVariables.ElementAt(selectedVarIdx).PollBetas[seasonInd].Beta = Convert.ToDouble(txtBeta.Text);
                _hif.PollVariables.ElementAt(selectedVarIdx).PollBetas[seasonInd].AConstantName = txtAconstantDescription.Text.ToString();
                _hif.PollVariables.ElementAt(selectedVarIdx).PollBetas[seasonInd].BConstantName = txtBconstantDescription.Text.ToString();
                _hif.PollVariables.ElementAt(selectedVarIdx).PollBetas[seasonInd].CConstantName = txtCconstantDescription.Text.ToString();
                _hif.PollVariables.ElementAt(selectedVarIdx).PollBetas[seasonInd].AConstantValue = Convert.ToDouble(txtAconstantValue.Text);
                _hif.PollVariables.ElementAt(selectedVarIdx).PollBetas[seasonInd].BConstantValue = Convert.ToDouble(txtBconstantValue.Text);
                _hif.PollVariables.ElementAt(selectedVarIdx).PollBetas[seasonInd].CConstantValue = Convert.ToDouble(txtCconstantValue.Text);
                _hif.PollVariables.ElementAt(selectedVarIdx).PollBetas[seasonInd].Distribution = cboBetaDistribution.Text.ToString();

                // Save metrics
                saveMetric();
                saveDistribution();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void loadVariable()
        {
            try
            {
                CRFVariable selectedVariable = _hif.PollVariables.ElementAt(selectedVarIdx);
                txtVariable.Text = selectedVariable.VariableName;
                txtPollutant.Text = selectedVariable.PollutantName;
                txtModelSpec.Text = _hif.ModelSpec;

                if (isSeasonal)
                {


                    cboSeason.SelectedItem = cboSeason.Items[selectedSeasonIdx];
                    txtSeason.Text = selectedVariable.PollBetas[selectedSeasonIdx].SeasonName;
                    txtStart.Text = selectedVariable.PollBetas[selectedSeasonIdx].StartDate;
                    txtEnd.Text = selectedVariable.PollBetas[selectedSeasonIdx].EndDate;
                }

                txtAconstantDescription.Text = selectedVariable.PollBetas[selectedSeasonIdx].AConstantName;
                txtBconstantDescription.Text = selectedVariable.PollBetas[selectedSeasonIdx].BConstantName;
                txtCconstantDescription.Text = selectedVariable.PollBetas[selectedSeasonIdx].CConstantName;
                txtAconstantValue.Text = selectedVariable.PollBetas[selectedSeasonIdx].AConstantValue.ToString();
                txtBconstantValue.Text = selectedVariable.PollBetas[selectedSeasonIdx].BConstantValue.ToString();
                txtCconstantValue.Text = selectedVariable.PollBetas[selectedSeasonIdx].CConstantValue.ToString();
                txtBeta.Text = selectedVariable.PollBetas[selectedSeasonIdx].Beta.ToString();
                cboBetaDistribution.SelectedIndex = cboBetaDistribution.FindString(selectedVariable.PollBetas[selectedSeasonIdx].Distribution);

                if (txtBetaParameter1.Visible && txtBetaParameter2.Visible)
                {
                    txtBetaParameter1.Text = selectedVariable.PollBetas[selectedSeasonIdx].P1Beta.ToString();
                    txtBetaParameter2.Text = selectedVariable.PollBetas[selectedSeasonIdx].P2Beta.ToString();
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
                CRFVariable selectedVariable = _hif.PollVariables.ElementAt(selectedVarIdx);

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
                    cboMetric.Text = selectedVariable.Metric.MetricName;
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        // Saves the metric object for the current beta object 
        private void saveMetric()
        {
            try
            {
                CRFVariable selectedVariable = _hif.PollVariables.ElementAt(selectedVarIdx);

                if (selectedVariable.PollutantName.Contains("*")) return;

                string commandText = string.Format("select metricid, hourlymetricgeneration from metrics where pollutantid = {0} and metricname = '{1}'", selectedVariable.Pollutant1ID ,cboMetric.Text);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                selectedVariable.Metric = new Metric();
                selectedVariable.Metric.MetricName = cboMetric.Text;
                selectedVariable.Metric.PollutantID = selectedVariable.Pollutant1ID;
                selectedVariable.Metric.MetricID = Convert.ToInt32(ds.Tables[0].Rows[0]["metricid"]);
                selectedVariable.Metric.HourlyMetricGeneration = Convert.ToInt32(ds.Tables[0].Rows[0]["hourlymetricgeneration"]);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void saveDistribution()
        {
            try
            {
                CRFVariable selectedVariable = _hif.PollVariables.ElementAt(selectedVarIdx);
                selectedVariable.PollBetas[selectedSeasonIdx].Distribution = cboBetaDistribution.Text;

                string commandText = string.Format("select distributiontypeid from DISTRIBUTIONTYPES where distributionname = '{0}'", cboBetaDistribution.Text);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                object res = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                if (res != null)
                {
                    selectedVariable.PollBetas[selectedSeasonIdx].DistributionTypeID = Convert.ToInt32(res);
                }
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

        private void editVarBtn_Click(object sender, EventArgs e)
        {
            CRFBeta temp = new CRFBeta();
            temp = _hif.PollVariables[selectedVarIdx].PollBetas[selectedSeasonIdx].DeepCopy();

            VarianceMulti form = new VarianceMulti(_hif.ModelSpec, _hif.PollVariables[selectedVarIdx].PollutantName, temp);
            DialogResult res = form.ShowDialog();
            if(res == DialogResult.OK)
            {
                _hif.PollVariables[selectedVarIdx].PollBetas[selectedSeasonIdx] = form.Beta.DeepCopy();
            }
        }

        private void cboSeason_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cboMetric.SelectedIndex == -1 && _hif.PollVariables[selectedVarIdx].PollutantName.Contains("*") == false)
            {
                MessageBox.Show("Please select a metric before changing season.");
                cboSeason.SelectedIndex = selectedSeasonIdx;
                return;
            }
            saveCurrent(selectedSeasonIdx);
            selectedSeasonIdx = cboSeason.SelectedIndex;
            loadVariable();
        }

        // Adapted from HealthImpactFunctionDefinition.cs 
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
                if (cboBetaDistribution.SelectedItem.ToString().Trim() == "None") { return; }
                if (cboBetaDistribution.SelectedItem.ToString().Trim() == "Custom")
                {
                    list = _hif.PollVariables[selectedVarIdx].PollBetas[selectedSeasonIdx].CustomList;
                    if (list.Count == 0)
                    {
                        CustomDistributionEntries frm = new CustomDistributionEntries();
                        DialogResult rtn = frm.ShowDialog();
                        if (rtn != DialogResult.OK) { return; }
                        list = frm.list;
                        _hif.PollVariables[selectedVarIdx].PollBetas[selectedSeasonIdx].CustomList = frm.list;
                        txtBeta.Text = frm.Mean.ToString();
                        _hif.PollVariables[selectedVarIdx].PollBetas[selectedSeasonIdx].Beta = frm.Mean;
                        txtBetaParameter1.Text = frm.StandardDeviation.ToString();
                        _hif.PollVariables[selectedVarIdx].PollBetas[selectedSeasonIdx].P1Beta = frm.StandardDeviation;
                    }
                    else
                    {
                        CustomDistributionEntries frmCustom = new CustomDistributionEntries(list);
                        DialogResult rtnCustom = frmCustom.ShowDialog();
                        if (rtnCustom != DialogResult.OK) { return; }
                        list = frmCustom.list;
                        _hif.PollVariables[selectedVarIdx].PollBetas[selectedSeasonIdx].CustomList = frmCustom.list;
                        txtBeta.Text = frmCustom.Mean.ToString();
                        _hif.PollVariables[selectedVarIdx].PollBetas[selectedSeasonIdx].Beta = frmCustom.Mean;
                        txtBetaParameter1.Text = frmCustom.StandardDeviation.ToString();
                        _hif.PollVariables[selectedVarIdx].PollBetas[selectedSeasonIdx].P1Beta = frmCustom.StandardDeviation;
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

                    // Beta distribution is the same for each beta associated with that variable
                    CRFVariable selectedVar = _hif.PollVariables[selectedVarIdx];
                    foreach (CRFBeta b in selectedVar.PollBetas)
                    {
                        b.Distribution = cboBetaDistribution.SelectedItem.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void txtBeta_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46 || keyValue == 45)
            {
                if (e.KeyChar == 45 && (((TextBox)sender).SelectionStart == 0 && ((TextBox)sender).Text.IndexOf("-") >= 0))
                    e.Handled = true;
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtBetaParameter1_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46 || keyValue == 45)
            {
                if (e.KeyChar == 45 && (((TextBox)sender).SelectionStart == 0 && ((TextBox)sender).Text.IndexOf("-") >= 0))
                    e.Handled = true;
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtBetaParameter2_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46 || keyValue == 45)
            {
                if (e.KeyChar == 45 && (((TextBox)sender).SelectionStart == 0 && ((TextBox)sender).Text.IndexOf("-") >= 0))
                    e.Handled = true;
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtAconstantValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46 || keyValue == 45)
            {
                if (e.KeyChar == 45 && (((TextBox)sender).SelectionStart == 0 && ((TextBox)sender).Text.IndexOf("-") >= 0))
                    e.Handled = true;
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtBconstantValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46 || keyValue == 45)
            {
                if (e.KeyChar == 45 && (((TextBox)sender).SelectionStart == 0 && ((TextBox)sender).Text.IndexOf("-") >= 0))
                    e.Handled = true;
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtCconstantValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46 || keyValue == 45)
            {
                if (e.KeyChar == 45 && (((TextBox)sender).SelectionStart == 0 && ((TextBox)sender).Text.IndexOf("-") >= 0))
                    e.Handled = true;
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }
    }
}
 