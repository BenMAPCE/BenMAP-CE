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
    public partial class APVConfigurationAdvancedSettings : FormBase
    {
        private bool isload = false;
        private bool isloadIncomeGrowth = false;
        private bool isloadInflation = false;
        public APVConfigurationAdvancedSettings()
        {
            InitializeComponent();
        }

        ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
        DataSet dsGrid = new DataSet();
        string commandText = string.Empty;
        private void APVConfigurationAdvancedSettings_Load(object sender, EventArgs e)
        {
            try
            {
                BindingGridType();
                cboIncidenceAggregation.DataSource = dsGrid.Tables[0];
                cboIncidenceAggregation.DisplayMember = "GridDefinitionName";
                cboIncidenceAggregation.SelectedIndex = -1;
                BindingGridType();
                cboValuationAggregation.DataSource = dsGrid.Tables[0];
                cboValuationAggregation.DisplayMember = "GridDefinitionName";
                cboValuationAggregation.SelectedIndex = -1;
                BindingGridType();
                cboQALYAggregation.DataSource = dsGrid.Tables[0];
                cboQALYAggregation.DisplayMember = "GridDefinitionName";
                cboQALYAggregation.SelectedIndex = -1;
                BindingGrid();
                if (_incidencePoolingAndAggregationAdvance == null) _incidencePoolingAndAggregationAdvance = new IncidencePoolingAndAggregationAdvance();
                if (_incidencePoolingAndAggregationAdvance.IncidenceAggregation != null)
                {
                    foreach (DataRowView drv in cboIncidenceAggregation.Items)
                    {
                        if (drv["GridDefinitionID"].ToString() == _incidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID.ToString())
                        {
                            cboIncidenceAggregation.SelectedItem = drv;
                        }
                    }
                }
                if (_incidencePoolingAndAggregationAdvance.ValuationAggregation != null)
                {
                    foreach (DataRowView drvValuation in cboValuationAggregation.Items)
                    {
                        if (drvValuation["GridDefinitionID"].ToString() == _incidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID.ToString())
                        {
                            cboValuationAggregation.SelectedItem = drvValuation;
                        }
                    }
                }
                if (_incidencePoolingAndAggregationAdvance.QALYAggregation != null)
                {
                    foreach (DataRowView drvQALY in cboQALYAggregation.Items)
                    {
                        if (drvQALY["GridDefinitionID"].ToString() == _incidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID.ToString())
                        {
                            cboQALYAggregation.SelectedItem = drvQALY;
                        }
                    }
                }
                if (_incidencePoolingAndAggregationAdvance.IPAdvancePoolingMethod != null)
                {
                    cboDefaultAdvancedPoolingMethod.SelectedIndex = (int)_incidencePoolingAndAggregationAdvance.IPAdvancePoolingMethod;
                }
                if (_incidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations != null)
                {
                    cboDefaultMonteCarloIterations.Text = _incidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations.ToString();
                }
                if (_incidencePoolingAndAggregationAdvance.RandomSeed != null)
                {
                    txtRandomSeed.Text = _incidencePoolingAndAggregationAdvance.RandomSeed;
                }
                else
                    txtRandomSeed.Text = "1";
                chbSortIncidenceResults.Checked = _incidencePoolingAndAggregationAdvance.SortIncidenceResults;

                if (CommonClass.IncidencePoolingAndAggregationAdvance == null)
                {
                    cboInflationDataset.SelectedIndex = 0;

                }

                if (_incidencePoolingAndAggregationAdvance.InflationDatasetID != null)
                {
                    foreach (DataRowView drvInflation in cboInflationDataset.Items)
                    {
                        if (drvInflation["InflationDataSetID"].ToString() == _incidencePoolingAndAggregationAdvance.InflationDatasetID.ToString())
                        {
                            cboInflationDataset.SelectedItem = drvInflation;
                        }
                    }
                }
                if (_incidencePoolingAndAggregationAdvance.CurrencyYear == -1 && CommonClass.BenMAPPopulation != null) _incidencePoolingAndAggregationAdvance.CurrencyYear = CommonClass.BenMAPPopulation.Year; else if (_incidencePoolingAndAggregationAdvance.CurrencyYear == -1) _incidencePoolingAndAggregationAdvance.CurrencyYear = 2010;
                if (_incidencePoolingAndAggregationAdvance.CurrencyYear != null)
                {
                    foreach (DataRowView drvCurrencyYear in cboCurrencyYear.Items)
                    {
                        if (drvCurrencyYear["Yyear"].ToString() == _incidencePoolingAndAggregationAdvance.CurrencyYear.ToString())
                        {
                            cboCurrencyYear.SelectedItem = drvCurrencyYear;
                        }
                    }
                }

                if (_incidencePoolingAndAggregationAdvance.AdjustIncomeGrowthDatasetID != null)
                {
                    foreach (DataRowView drvIncomeGrowth in cboIncomeGrowthDataset.Items)
                    {
                        if (drvIncomeGrowth["IncomeGrowthAdjDataSetID"].ToString() == _incidencePoolingAndAggregationAdvance.AdjustIncomeGrowthDatasetID.ToString())
                        {
                            cboIncomeGrowthDataset.SelectedItem = drvIncomeGrowth;
                        }
                    }
                }
                if (_incidencePoolingAndAggregationAdvance.IncomeGrowthYear != null)
                {
                    foreach (DataRowView drvIncomeGrowthYear in cboIncomeGrowthYear.Items)
                    {
                        if (drvIncomeGrowthYear["Yyear"].ToString() == _incidencePoolingAndAggregationAdvance.IncomeGrowthYear.ToString())
                        {
                            cboIncomeGrowthYear.SelectedItem = drvIncomeGrowthYear;
                        }
                    }
                }
                if (_incidencePoolingAndAggregationAdvance.IncomeGrowthYear != null)
                {
                    foreach (DataRowView drvIncomeGrowthYear in cboIncomeGrowthYear.Items)
                    {
                        if (drvIncomeGrowthYear["Yyear"] == _incidencePoolingAndAggregationAdvance.IncomeGrowthYear.ToString())
                        {
                            cboIncomeGrowthYear.SelectedItem = drvIncomeGrowthYear;
                        }
                    }
                }
                if (_incidencePoolingAndAggregationAdvance.EndpointGroups != null)
                {
                    if (lstEndpointGroups.SelectedItems != null && lstEndpointGroups.SelectedItems.Count > 0)
                    {
                        lstEndpointGroups.SelectedItems.Clear();
                    }
                    for (int i = lstEndpointGroups.Items.Count - 1; i >= 0; i--)
                    {
                        var item = lstEndpointGroups.Items[i] as DataRowView;
                        if (_incidencePoolingAndAggregationAdvance.EndpointGroups.Contains(item["EndpointGroups"].ToString()))
                        {
                            lstEndpointGroups.SelectedItems.Add(item);
                        }
                    }
                }
                else
                {
                    List<string> listEndpointGroups = new List<string>();
                    for (int i = lstEndpointGroups.Items.Count - 1; i >= 0; i--)
                    {
                        var item = lstEndpointGroups.Items[i] as DataRowView;

                        lstEndpointGroups.SelectedItems.Add(item);


                        DataRowView drv = lstEndpointGroups.Items[i] as DataRowView;
                        listEndpointGroups.Add(drv["EndpointGroups"].ToString());
                    }
                    _incidencePoolingAndAggregationAdvance.EndpointGroups = listEndpointGroups;

                }


            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

        }

        public void BindingGrid()
        {
            cboDefaultAdvancedPoolingMethod.Items.Add("Round weights to two digits");
            cboDefaultAdvancedPoolingMethod.Items.Add("Round weights to three digits");
            cboDefaultAdvancedPoolingMethod.Items.Add("Use exact weights for Monte Carlo");
            cboDefaultAdvancedPoolingMethod.SelectedIndex = 0;
            cboDefaultAdvancedPoolingMethod.DropDownWidth = 200;
            commandText = string.Format("select * from InflationDataSets where SetupID={0}", CommonClass.MainSetup.SetupID);
            dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);

            cboInflationDataset.DataSource = dsGrid.Tables[0];
            cboInflationDataset.DisplayMember = "InflationDataSetName";
            if(cboInflationDataset.Items.Count > 0)
            {
                cboInflationDataset.SelectedIndex = 0;
            }
            isloadInflation = true;
            commandText = string.Format("select * from IncomeGrowthAdjDataSets where SetupID={0}", CommonClass.MainSetup.SetupID);
            dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
            cboIncomeGrowthDataset.DataSource = dsGrid.Tables[0];
            cboIncomeGrowthDataset.DisplayMember = "IncomeGrowthAdjDatasetName";
            if (cboIncomeGrowthDataset.Items.Count > 0)
            {
                cboIncomeGrowthDataset.SelectedIndex = 0;
            }
            isloadIncomeGrowth = true;
        }
        public void BindingGridType()
        {
            commandText = string.Format("select * from GridDefinitions where SetupID={0} order by GridDefinitionName asc ", CommonClass.MainSetup.SetupID);
            dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);

        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void cboInflationDataset_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {

                commandText = string.Format("select InflationDataSetID,InflationDataSetName from InflationDataSets where SetupID={0}", CommonClass.MainSetup.SetupID);
                dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                Dictionary<string, int> dicInflationDataset = new Dictionary<string, int>();
                foreach (DataRow dr in dsGrid.Tables[0].Rows)
                {
                    dicInflationDataset.Add(dr["InflationDataSetName"].ToString(), Convert.ToInt32(dr["InflationDataSetID"].ToString()));
                }
                DataRowView drv = cboInflationDataset.SelectedItem as DataRowView;
                string str = drv["InflationDataSetName"].ToString();
                commandText = string.Format("select Yyear from InflationEntries where InflationDataSetID={0}", dicInflationDataset[str]);
                dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboCurrencyYear.DataSource = dsGrid.Tables[0];
                cboCurrencyYear.DisplayMember = "Yyear";
                cboCurrencyYear.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

        }

        private void cboIncomeGrowthYear_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {

                DataRowView drvYear = cboIncomeGrowthYear.SelectedItem as DataRowView;
                int Year = Convert.ToInt32(drvYear["Yyear"].ToString());
                DataRowView drvInComeGrowthAdjDataSetName = cboIncomeGrowthDataset.SelectedItem as DataRowView;
                string str = drvInComeGrowthAdjDataSetName["IncomeGrowthAdjDataSetName"].ToString();
                commandText = string.Format("select IncomeGrowthAdjDataSetID from IncomeGrowthAdjDataSets where IncomeGrowthAdjDataSetName='{0}'", str);
                object obj = fb.ExecuteScalar(CommonClass.Connection, new CommandType(), commandText);
                int incomeGrowthID = int.Parse(obj.ToString());
                // 2015 02 03 modified next line to add dataset to filter. otherwise all endpoints were shown, even if not in the income growth dataset 
                //commandText = string.Format("select EndpointGroups from IncomeGrowthAdjFactors where Yyear={0}", Year);
                commandText = string.Format("select EndpointGroups from IncomeGrowthAdjFactors where Yyear={0} AND IncomeGrowthAdjDataSetID={1}", Year, incomeGrowthID);
                dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                lstEndpointGroups.DataSource = dsGrid.Tables[0];
                lstEndpointGroups.DisplayMember = "EndpointGroups";
                lstEndpointGroups.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private void cboIncomeGrowthDataset_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                commandText = string.Format("select IncomeGrowthAdjDataSetName,IncomeGrowthAdjDataSetID from IncomeGrowthAdjDataSets where SetupID={0}", CommonClass.MainSetup.SetupID);
                dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                Dictionary<string, int> dicIncomeGrowthYear = new Dictionary<string, int>();
                foreach (DataRow dr in dsGrid.Tables[0].Rows)
                {
                    dicIncomeGrowthYear.Add(dr["IncomeGrowthAdjDataSetName"].ToString(), Convert.ToInt32(dr["IncomeGrowthAdjDataSetID"].ToString()));
                }
                DataRowView drv = cboIncomeGrowthDataset.SelectedItem as DataRowView;
                string str = drv["IncomeGrowthAdjDataSetName"].ToString();
                commandText = string.Format("select distinct Yyear from IncomeGrowthAdjFactors where IncomeGrowthAdjDataSetID={0}", dicIncomeGrowthYear[str]);
                dsGrid = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);
                cboIncomeGrowthYear.DataSource = dsGrid.Tables[0];
                cboIncomeGrowthYear.DisplayMember = "Yyear";
                int index = 0;
                foreach (DataRowView element in cboIncomeGrowthYear.Items)
                {
                    if (CommonClass.BenMAPPopulation.Year.ToString() == element["Yyear"].ToString())
                    {
                        cboIncomeGrowthYear.SelectedIndex = index; break;
                    }
                    index++;
                }
                isload = true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                int iMonte = -1;
                if (Int32.TryParse(cboDefaultMonteCarloIterations.Text, out iMonte) == false)
                {
                    MessageBox.Show("The default Monte Carlo iterations must be a number.");
                    return;
                }

                int iSeeds = -1; if (txtRandomSeed.Text != "Random Integer" && Int32.TryParse(txtRandomSeed.Text, out iSeeds) == false)
                {
                    MessageBox.Show("The random seed must be a number.");
                    return;
                }

                if (cboIncidenceAggregation.SelectedIndex != -1)
                {
                    DataRowView drvIncidence = cboIncidenceAggregation.SelectedItem as DataRowView;
                    _incidencePoolingAndAggregationAdvance.IncidenceAggregation = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drvIncidence["GridDefinitionID"].ToString()));
                }
                if (cboValuationAggregation.SelectedIndex != -1)
                {
                    DataRowView drvValuation = cboValuationAggregation.SelectedItem as DataRowView;
                    _incidencePoolingAndAggregationAdvance.ValuationAggregation = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drvValuation["GridDefinitionID"].ToString()));
                }
                if (cboQALYAggregation.SelectedIndex != -1)
                {
                    DataRowView drvQALY = cboQALYAggregation.SelectedItem as DataRowView;
                    _incidencePoolingAndAggregationAdvance.QALYAggregation = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(drvQALY["GridDefinitionID"].ToString()));
                }

                _incidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations = Convert.ToInt32(cboDefaultMonteCarloIterations.Text);
                _incidencePoolingAndAggregationAdvance.RandomSeed = txtRandomSeed.Text;
                _incidencePoolingAndAggregationAdvance.SortIncidenceResults = chbSortIncidenceResults.Checked;
                if (cboInflationDataset.SelectedIndex != -1)
                {
                    DataRowView drvInflation = cboInflationDataset.SelectedItem as DataRowView;
                    _incidencePoolingAndAggregationAdvance.InflationDatasetID = Convert.ToInt32(drvInflation["InflationDatasetID"]);
                    _incidencePoolingAndAggregationAdvance.InflationDatasetName = drvInflation["InflationDataSetName"].ToString();
                }
                if (cboCurrencyYear.SelectedIndex != -1)
                {
                    DataRowView drvCurrencyYear = cboCurrencyYear.SelectedItem as DataRowView;
                    _incidencePoolingAndAggregationAdvance.CurrencyYear = Convert.ToInt32(drvCurrencyYear["Yyear"]);
                }
                if (cboIncomeGrowthDataset.SelectedIndex != -1)
                {
                    DataRowView drvIncomeGrowth = cboIncomeGrowthDataset.SelectedItem as DataRowView;
                    _incidencePoolingAndAggregationAdvance.AdjustIncomeGrowthDatasetID = Convert.ToInt32(drvIncomeGrowth["IncomeGrowthAdjDataSetID"].ToString());
                    _incidencePoolingAndAggregationAdvance.AdjustIncomeGrowthDatasetName = drvIncomeGrowth["IncomeGrowthAdjDatasetName"].ToString();
                }
                if (cboIncomeGrowthYear.SelectedIndex != -1)
                {
                    DataRowView drvIncomeGrowthYear = cboIncomeGrowthYear.SelectedItem as DataRowView;
                    _incidencePoolingAndAggregationAdvance.IncomeGrowthYear = Convert.ToInt32(drvIncomeGrowthYear["Yyear"].ToString());
                }

                List<string> listEndpointGroups = new List<string>();
                for (int i = 0; i < lstEndpointGroups.SelectedItems.Count; i++)
                {
                    DataRowView drv = lstEndpointGroups.SelectedItems[i] as DataRowView;
                    listEndpointGroups.Add(drv["EndpointGroups"].ToString());
                }
                _incidencePoolingAndAggregationAdvance.EndpointGroups = listEndpointGroups;


                if (cboDefaultAdvancedPoolingMethod.SelectedIndex != -1)
                {
                    _incidencePoolingAndAggregationAdvance.IPAdvancePoolingMethod = (IPAdvancePoolingMethodEnum)cboDefaultAdvancedPoolingMethod.SelectedIndex;
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private IncidencePoolingAndAggregationAdvance _incidencePoolingAndAggregationAdvance;

        public IncidencePoolingAndAggregationAdvance IncidencePoolingAndAggregationAdvance
        {
            get { return _incidencePoolingAndAggregationAdvance; }
            set { _incidencePoolingAndAggregationAdvance = value; }
        }

        private void txtRandomSeed_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void cboIncomeGrowthDataset_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboIncomeGrowthYear_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public void AdvanceOptionType(int optionType)
        {
            if (optionType == 1)
            {
                if (this.tab.TabPages.Contains(tbpAggreationAndPooling))
                {
                    this.tab.TabPages.Remove(this.tbpCurrencyAndIncome);
                }
                else
                {
                    this.tab.TabPages.Add(this.tbpAggreationAndPooling);
                    this.tab.TabPages.Remove(this.tbpCurrencyAndIncome);
                }

                this.Text = "Advanced Pooling Settings:";
            }
            else
            {
                if (this.tab.TabPages.Contains(tbpCurrencyAndIncome))
                {
                    this.tab.TabPages.Remove(this.tbpAggreationAndPooling);
                }
                else
                {
                    this.tab.TabPages.Remove(this.tbpAggreationAndPooling);
                    this.tab.TabPages.Add(this.tbpCurrencyAndIncome);
                }
                this.Text = "Advanced Valuation Settings:";
            }
        }
    }
}
