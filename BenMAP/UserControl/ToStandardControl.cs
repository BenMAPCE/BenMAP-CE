using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BenMAP.DataSource
{
    public partial class ToStandardControl : UserControl
    {
        #region Parameters
        private BenMAPPollutant _BenMAPPollutant;
        public StandardRollback _StandardRollback;
        private int _regionID;
        /// <summary>
        /// 控件的RegionID
        /// </summary>
        public int RegionID
        {
            get { return _regionID; }
            set { _regionID = value; }
        }
        #endregion

        public ToStandardControl(BenMAPPollutant BenMAPPollutant )
        {
            InitializeComponent();
            _BenMAPPollutant = BenMAPPollutant;
            //_StandardRollback = StandardRollback;
        }

        private void ToStandardControl_Load(object sender, EventArgs e)
        {
            //--给Metric赋值

            foreach (Metric m in _BenMAPPollutant.Metrics)
            {
                cboDailyMetric.Items.Add(m.MetricName);
            }
            cboDailyMetric.SelectedIndex = 0;
            //BindingSource bsMetric = new BindingSource();

            //// DicFilterGroup = (Dictionary<string, int>)DicFilterGroup.OrderBy(p => p.Value);
            //bsMetric.DataSource = _BenMAPPollutant.Metrics;
            //cboDailyMetric.DataSource = bsMetric;
            //cboDailyMetric.DisplayMember = "MetricName";
            //cboDailyMetric.ValueMember = "Metric.MetricID";
            //BindingSource bsSesonalMetrics = new BindingSource();

            //// DicFilterGroup = (Dictionary<string, int>)DicFilterGroup.OrderBy(p => p.Value);
            //bsSesonalMetrics.DataSource = _BenMAPPollutant.SesonalMetrics;
            //cboSeasonalMetric.DataSource = bsSesonalMetrics;
            //cboSeasonalMetric.DisplayMember = "SeasonalMetricName";
            //cboSeasonalMetric.ValueMember = "SeasonalMetricName";
            cboAnnualStatistic.Items.Add("");
            cboAnnualStatistic.Items.Add("Mean");
            cboAnnualStatistic.Items.Add("Median");
            cboAnnualStatistic.Items.Add("Max");
            cboAnnualStatistic.Items.Add("Min");
            cboAnnualStatistic.Items.Add("Sum");
            lblOrdinalityDestribution.Text = "Highest" + " value of " + cboDailyMetric.Text + "<=" + txtStandard.Text;
            cboInterday.Items.Add("Percentage");
            cboInterday.Items.Add("Incremental");
            //cboInterday.Items.Add("Quadratic");
            cboInterday.Items.Add("Peak Shaving");
            cboInterday.SelectedIndex = 0;
            cboIntraday.Items.Add("Percentage");
            cboIntraday.Items.Add("Incremental");
            //cboIntraday.Items.Add("Quadratic");
            //cboIntraday.Items.Add("Peak Shaving");
            cboIntraday.SelectedIndex = 0;
            if (_BenMAPPollutant.Observationtype == ObservationtypeEnum.Hourly)
            {
                lblIntradayRollbackMethod.Enabled = true;
                cboIntraday.Enabled = true;
                lblIntraBackground.Enabled = true;
                txtIntradayBackground.Enabled = true;
            }
            else
            {
                lblIntradayRollbackMethod.Enabled = false;
                cboIntraday.Enabled = false;
                lblIntraBackground.Enabled = false;
                txtIntradayBackground.Enabled = false;
            }

        }

        private void nudnOrdinality_ValueChanged(object sender, EventArgs e)
        {
            changelabel();
            _StandardRollback.Ordinality = Convert.ToInt16(nudnOrdinality.Value);
        }

        private void nudnOrdinality_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8)
            {
                e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void nudnOrdinality_KeyUp(object sender, KeyEventArgs e)
        {
            changelabel();
            _StandardRollback.Ordinality = Convert.ToInt16(nudnOrdinality.Value);
        }

        private void txtStandard_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Double.Parse(txtStandard.Text);
                changelabel();
                _StandardRollback.Standard = Double.Parse(txtStandard.Text);
            }
            catch
            {
                txtStandard.Text = "0.00";
                changelabel();
                _StandardRollback.Standard = 0;
            }
        }

        private void txtStandard_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46)
            {
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        void changelabel()
        {
            int number = Convert.ToInt16(nudnOrdinality.Value);
            if (cboAnnualStatistic.Text != "")
            {
                lblOrdinalityDestribution.Text = cboAnnualStatistic.Text + " value of ";
            }
            else if (number == 1)
            {
                lblOrdinalityDestribution.Text = "Highest" + " value of ";
            }
            else
            {
                switch (number.ToString().Substring(number.ToString().Length - 1, 1))
                {
                    case "2":
                        lblOrdinalityDestribution.Text = number.ToString() + "nd" + " Highest" + " value of ";
                        break;
                    case "3":
                        lblOrdinalityDestribution.Text = number.ToString() + "rd" + " Highest" + " value of ";
                        break;
                }
            }

            if (cboSeasonalMetric.Text != "")
            {
                lblOrdinalityDestribution.Text += cboSeasonalMetric.Text + " of ";
            }
            lblOrdinalityDestribution.Text += cboDailyMetric.Text + " <= " + txtStandard.Text;
        }

        private void cboInterday_SelectedIndexChanged(object sender, EventArgs e)
        {
            _StandardRollback.InterdayRollbackMethod = cboInterday.Text;
        }

        private void txtBackground_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46)
            {
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtBackground_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _StandardRollback.InterdayBackground = Double.Parse(txtInterdayBackground.Text);
            }
            catch
            {
                txtInterdayBackground.Text = "0.00";
                _StandardRollback.InterdayBackground = 0;
            }
        }

        private void cboDailyMetric_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                foreach (Metric metric in _BenMAPPollutant.Metrics)
                {
                    if (cboDailyMetric.SelectedItem.ToString() == metric.MetricName)
                        _StandardRollback.DailyMetric = metric;
                }
                cboSeasonalMetric.Items.Clear();
                cboSeasonalMetric.Items.Add("");
                if (_BenMAPPollutant.SesonalMetrics != null)
                {
                    foreach (SeasonalMetric s in _BenMAPPollutant.SesonalMetrics)
                    {
                        if (s.Metric.MetricName == _StandardRollback.DailyMetric.MetricName)
                            cboSeasonalMetric.Items.Add(s.SeasonalMetricName);
                    }
                }
                changelabel();
            }
            catch
            { }
        }

        private void cboSeasonalMetric_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSeasonalMetric.Text == "")
            {
                _StandardRollback.SeasonalMetric = null;
            }
            else
            {
                foreach (SeasonalMetric metric in _BenMAPPollutant.SesonalMetrics)
                {
                    if (cboSeasonalMetric.SelectedItem.ToString() == metric.SeasonalMetricName)
                        _StandardRollback.SeasonalMetric = metric;
                }
            }
            changelabel();
        }

        private void cboAnnualStatistic_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboAnnualStatistic.Text == "")
            {
                lblOrdinality.Enabled = true;
                nudnOrdinality.Enabled = true;
            }
            else
            {
                lblOrdinality.Enabled = false;
                nudnOrdinality.Value = 1;
                nudnOrdinality.Enabled = false;
            }

            switch (cboAnnualStatistic.Text)
            {
                case "Mean":
                    _StandardRollback.AnnualStatistic = MetricStatic.Mean;
                    break;
                case "Median":
                    _StandardRollback.AnnualStatistic = MetricStatic.Median;
                    break;
                case "Max":
                    _StandardRollback.AnnualStatistic = MetricStatic.Max;
                    break;
                case "Min":
                    _StandardRollback.AnnualStatistic = MetricStatic.Min;
                    break;
                case "Sum":
                    _StandardRollback.AnnualStatistic = MetricStatic.Sum;
                    break;
                case "":
                    _StandardRollback.AnnualStatistic = MetricStatic.None;
                    break;
            }
            changelabel();
        }

        private void cboIntraday_SelectedIndexChanged(object sender, EventArgs e)
        {
            _StandardRollback.IntradayRollbackMethod = cboIntraday.Text;
        }

        private void txtIntradayBackground_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46)
            {
                if (e.KeyChar == 46 && ((TextBox)sender).Text.IndexOf(".") == 0)
                    e.Handled = true;
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }

        private void txtIntradayBackground_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _StandardRollback.IntradayBackground = Double.Parse(txtIntradayBackground.Text);
            }
            catch
            { 
                txtInterdayBackground.Text = "0.00";
                _StandardRollback.IntradayBackground = 0;
            }
        }
    }
}


