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

        // Some of these fields will be filled dynamically
        // Hard coded values put in for tesing UI features 
        private void EffectCoefficients_Load(object sender, EventArgs e)
        {
            try
            {
                BindItems();
                CRFVariable selectedVariable = _hif.PollVariables.ElementAt(selected);
                txtVariable.Text = selectedVariable.VariableName;
                txtPollutant.Text = selectedVariable.PollutantName;
                txtModelSpec.Text = _hif.ModelSpec;

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

                if (_hif.PollVariables.Count > 1)
                {
                    panel2.Visible = false;
                    editVarBtn.Visible = true;
                    prevBtn.Visible = true;
                    nextBtn.Visible = true;

                    if (seasonal) showForSeasonal.Visible = true;
                    else showForSeasonal.Visible = false;
                }

                else if (_hif.PollVariables.Count == 1)
                {
                    panel2.Visible = true;
                    editVarBtn.Visible = false;
                    prevBtn.Visible = false;
                    nextBtn.Visible = false;

                    if (seasonal) showForSeasonal.Visible = true;
                    else showForSeasonal.Visible = false;
                }

                else
                { 
                    MessageBox.Show("Pollutant variable list is empty");
                    return;
                }

                cboBetaDistribution.SelectedValueChanged -= cboBetaDistribution_SelectedValueChanged;
                cboBetaDistribution.SelectedValueChanged += cboBetaDistribution_SelectedValueChanged;

                cboSeason.SelectedIndexChanged -= cboSeason_SelectedValueChanged;
                cboSeason.SelectedIndexChanged += cboSeason_SelectedValueChanged;
            }

            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        public void BindItems()
        {
            try
            {
                string commandText = string.Empty;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

                if (seasonal)
                {
                    int i, numSeasons;
                    commandText = string.Format("select seasonalmetricseasonname, startday, endday, seasonalmetricseasonid from seasonalmetricseasons as sms inner join seasonalmetrics as sm on sm.seasonalmetricid=sms.seasonalmetricid inner join crfunctions c on c.metricid=sm.metricid and c.seasonalmetricid=sm.seasonalmetricid and c.seasonalmetricid=sm.seasonalmetricid and crfunctionid={0} order by startday asc", Convert.ToInt32(_hif.FunctionID));
                    DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    DataRow dr = ds.Tables[0].Rows[0];
                    numSeasons = ds.Tables[0].Rows.Count;

                    txtSeason.Text = dr["seasonalmetricseasonname"].ToString();
                    txtStart.Text = dr["startday"].ToString();
                    txtEnd.Text = dr["endday"].ToString();

                    for (i = 1; i <= numSeasons; i++)
                    {
                        cboSeason.Items.Add(string.Format("Season {0}", i));
                    }
                     
                    i = 0;
                    foreach (var pv in _hif.PollVariables)
                    {
                        while (pv.PollBetas.Count != numSeasons)
                        {
                            _hif.PollVariables[i].PollBetas.Add(new CRFBeta());
                        }
                        i++;
                    }

                    selectedSeason = 0;
                    cboSeason.SelectedIndex = 0;

                    commandText = string.Format("select beta, p1beta, p2beta, a, namea, b, nameb, c, namec from crfvariables vars inner join crfbetas betas on betas.crfvariableid=vars.crfvariableid where crfunctionid={0} and seasonalmetricseasonid={1} and vars.pollutant1id={2}", Convert.ToInt32(_hif.FunctionID), Convert.ToInt32(dr["seasonalmetricseasonid"].ToString()), _hif.PollVariables.ElementAt(selected).PollutantID);
                    ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    dr = ds.Tables[0].Rows[0];

                    txtBeta.Text = dr["beta"].ToString();

                    if (dr["p1beta"].ToString() == string.Empty) txtBetaParameter1.Text = "0";
                    else txtBetaParameter1.Text = dr["p1beta"].ToString();

                    if (dr["p2beta"].ToString() == string.Empty) txtBetaParameter2.Text = "0";
                    else txtBetaParameter2.Text = dr["p2beta"].ToString();

                    if (dr["a"].ToString() == string.Empty) txtAconstantValue.Text = "0";
                    else txtAconstantValue.Text = dr["a"].ToString();
                    txtAconstantDescription.Text = dr["namea"].ToString();

                    if (dr["b"].ToString() == string.Empty) txtBconstantValue.Text = "0";
                    else txtBconstantValue.Text = dr["b"].ToString();
                    txtBconstantDescription.Text = dr["nameb"].ToString();

                    if (dr["c"].ToString() == string.Empty) txtCconstantValue.Text = "0";
                    else txtCconstantValue.Text = dr["c"].ToString();
                    txtCconstantDescription.Text = dr["namec"].ToString();

                    txtSeasMetric.Text = _hif.SeasonalMetric;
                }
                else
                {
                    // TBD

                    commandText = string.Format("select beta, p1beta, p2beta, a, namea, b, nameb, c, namec from crfvariables vars inner join crfbetas betas on betas.crfvariableid=vars.crfvariableid where crfunctionid={0} and vars.pollutant1id={2}", Convert.ToInt32(_hif.FunctionID), _hif.PollVariables.ElementAt(selected).PollutantID);
                    DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                    DataRow dr = ds.Tables[0].Rows[0];

                    txtBeta.Text = dr["beta"].ToString();

                    if (dr["p1beta"].ToString() == string.Empty) txtBetaParameter1.Text = "0";
                    else txtBetaParameter1.Text = dr["p1beta"].ToString();

                    if (dr["p2beta"].ToString() == string.Empty) txtBetaParameter2.Text = "0";
                    else txtBetaParameter2.Text = dr["p2beta"].ToString();

                    if (dr["a"].ToString() == string.Empty) txtAconstantValue.Text = "0";
                    else txtAconstantValue.Text = dr["a"].ToString();
                    txtAconstantDescription.Text = dr["namea"].ToString();

                    if (dr["b"].ToString() == string.Empty) txtBconstantValue.Text = "0";
                    else txtBconstantValue.Text = dr["b"].ToString();
                    txtBconstantDescription.Text = dr["nameb"].ToString();

                    if (dr["c"].ToString() == string.Empty) txtCconstantValue.Text = "0";
                    else txtCconstantValue.Text = dr["c"].ToString();
                    txtCconstantDescription.Text = dr["namec"].ToString();

                    txtSeasMetric.Text = "None";
                }
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

            loadVariable();
            cboSeason.SelectedIndex = 0;
        }

        private void prevBtn_Click(object sender, EventArgs e)
        {
            saveCurrent(cboSeason.SelectedIndex);

            // Set form for previous
            if (selected - 1 < 0) selected = _hif.PollVariables.Count() - 1;
            else { selected--; }

            loadVariable();
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
            CRFVariable selectedVariable = _hif.PollVariables.ElementAt(selected);
            txtVariable.Text = selectedVariable.VariableName;
            txtPollutant.Text = selectedVariable.PollutantName;
            txtModelSpec.Text = _hif.ModelSpec;

            // Need to fill object in HIF form
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult warn = MessageBox.Show("Pressing cancel will discard the changes on the current variable. Changes made on other variable values will still be saved.","Warning",MessageBoxButtons.OKCancel);
            if (warn == DialogResult.Cancel) return;

            this.DialogResult = DialogResult.Cancel;
        }

        private void editVarBtn_Click(object sender, EventArgs e)
        {
            VarianceMulti form = new VarianceMulti(txtModelSpec.Text, txtPollutant.Text);
            DialogResult res = form.ShowDialog();
        }

        private void cboSeason_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                saveCurrent(selectedSeason);
                loadVariable();
                selectedSeason = cboSeason.SelectedIndex;

                string commandText = string.Format("select seasonalmetricseasonname, startday, endday, seasonalmetricseasonid from seasonalmetricseasons as sms inner join seasonalmetrics as sm on sm.seasonalmetricid=sms.seasonalmetricid inner join crfunctions c on c.metricid=sm.metricid and c.seasonalmetricid=sm.seasonalmetricid and c.seasonalmetricid=sm.seasonalmetricid and crfunctionid={0} order by startday asc", Convert.ToInt32(_hif.FunctionID));
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                DataRow dr = ds.Tables[0].Rows[selectedSeason];

                txtSeason.Text = dr["seasonalmetricseasonname"].ToString();
                txtStart.Text = dr["startday"].ToString();
                txtEnd.Text = dr["endday"].ToString();

                commandText = string.Format("select beta, p1beta, p2beta, a, namea, b, nameb, c, namec from crfvariables vars inner join crfbetas betas on betas.crfvariableid=vars.crfvariableid where crfunctionid={0} and seasonalmetricseasonid={1} and vars.pollutant1id={2}", Convert.ToInt32(_hif.FunctionID), Convert.ToInt32(dr["seasonalmetricseasonid"].ToString()), _hif.PollVariables.ElementAt(selected).PollutantID);
                ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                dr = ds.Tables[0].Rows[0];

                txtBeta.Text = dr["beta"].ToString();

                if (dr["p1beta"].ToString() == string.Empty) txtBetaParameter1.Text = "0";
                else txtBetaParameter1.Text = dr["p1beta"].ToString();

                if (dr["p2beta"].ToString() == string.Empty) txtBetaParameter2.Text = "0";
                else txtBetaParameter2.Text = dr["p2beta"].ToString();

                if (dr["a"].ToString() == string.Empty) txtAconstantValue.Text = "0";
                else txtAconstantValue.Text = dr["a"].ToString();
                txtAconstantDescription.Text = dr["namea"].ToString();

                if (dr["b"].ToString() == string.Empty) txtBconstantValue.Text = "0";
                else txtBconstantValue.Text = dr["b"].ToString();
                txtBconstantDescription.Text = dr["nameb"].ToString();

                if (dr["c"].ToString() == string.Empty) txtCconstantValue.Text = "0";
                else txtCconstantValue.Text = dr["c"].ToString();
                txtCconstantDescription.Text = dr["namec"].ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
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
 