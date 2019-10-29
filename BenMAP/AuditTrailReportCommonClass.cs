using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESIL.DBUtility;
using System.Data;

namespace BenMAP
{
    class AuditTrailReportCommonClass
    {
        public static TreeNode getTreeNodeFromBenMAPPollutant(BenMAPPollutant benMAPPollutant)
        {
            TreeNode treeNode = new TreeNode();
            treeNode.Text = "Pollutant";
            DateTime dt = new DateTime(2011, 1, 1);
            try
            {
                treeNode.Nodes.Add("Name:" + benMAPPollutant.PollutantName);
                treeNode.Nodes.Add("Observation Type:" + ((benMAPPollutant.Observationtype == ObservationtypeEnum.Daily) ? "Daily" : "Hourly"));
                if (benMAPPollutant.Seasons != null && benMAPPollutant.Seasons.Count > 0)
                {
                    for (int i = 0; i < benMAPPollutant.Seasons.Count; i++)
                    {
                        dt = new DateTime(2011, 1, 1);
                        treeNode.Nodes.Add("Season" + i + ":" + dt.AddDays(benMAPPollutant.Seasons[i].StartDay).GetDateTimeFormats('M')[0].ToString() + "-" + dt.AddDays(benMAPPollutant.Seasons[i].EndDay).GetDateTimeFormats('M')[0].ToString());
                    }

                }
                if (benMAPPollutant.Metrics != null && benMAPPollutant.Metrics.Count > 0)
                {
                    for (int i = 0; i < benMAPPollutant.Metrics.Count; i++)
                    {
                        treeNode.Nodes.Add("Metric" + i + ":" + benMAPPollutant.Metrics[i].MetricName);
                    }

                }
                if (benMAPPollutant.SesonalMetrics != null && benMAPPollutant.SesonalMetrics.Count > 0)
                {
                    for (int i = 0; i < benMAPPollutant.SesonalMetrics.Count; i++)
                    {
                        treeNode.Nodes.Add("Seasonal Metric" + i + ":" + benMAPPollutant.SesonalMetrics[i].SeasonalMetricName);
                    }
                }

                return treeNode;
            }
            catch
            {
                return null;
            }
        }
        public static TreeNode getTreeNodeFromBenMAPLine(BenMAPLine benMAPLine)
        {
            TreeNode treeNode = new TreeNode();
            treeNode.Text = "Air Quality Surfaces";

            try
            {
                if (benMAPLine is ModelDataLine)
                {
                    treeNode = getTreeNodeFromModelDataLine(benMAPLine as ModelDataLine);
                }
                else if (benMAPLine is MonitorDataLine)
                {
                    treeNode = getTreeNodeFromMonitorDataLine(benMAPLine as MonitorDataLine);
                }
                treeNode.Nodes.Insert(0, "Create Datetime:" + benMAPLine.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                return treeNode;
            }
            catch
            {
                return null;
            }
        }
        public static TreeNode getTreeNodeFromModelDataLine(ModelDataLine modelDataLine)
        {
            TreeNode treeNode = new TreeNode();
            treeNode.Text = "Air Quality Surfaces";
            try
            {
                treeNode.Nodes.Add("Pollutant:" + modelDataLine.Pollutant.PollutantName);
                treeNode.Nodes.Add("Model Database File:" + modelDataLine.DatabaseFilePath);
                treeNode.Nodes.Add(getTreeNodeFromBenMAPGrid(modelDataLine.GridType));
                treeNode.Nodes.Add(getTreeNodeFromBenMAPPollutant(modelDataLine.Pollutant));
                return treeNode;
            }
            catch
            {
                return null;
            }

        }

        public static TreeNode getTreeNodeFromMonitorDataLine(MonitorDataLine monitorDataLine)
        {
            TreeNode treeNode = new TreeNode();
            treeNode.Text = "Air Quality Surfaces";
            try
            {
                treeNode.Nodes.Add("Pollutant:" + monitorDataLine.Pollutant.PollutantName);
                treeNode.Nodes.Add("Interpolation Method:" + monitorDataLine.InterpolationMethod);
                treeNode.Nodes.Add("Library Monitors:" + (monitorDataLine.MonitorDirectType == 0 ? "True" : "False"));
                FireBirdHelperBase fb = new ESILFireBirdHelper();
                string commandText = string.Format("select monitordatasetname from MonitorDatasets where monitordatasetid={0}", monitorDataLine.MonitorDataSetID);
                string datasetName = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                treeNode.Nodes.Add("Monitor Dataset Name:" + datasetName);
                treeNode.Nodes.Add("Monitor Year:" + monitorDataLine.MonitorLibraryYear);
                treeNode.Nodes.Add(getTreeNodeFromBenMAPGrid(monitorDataLine.GridType));
                treeNode.Nodes.Add(getTreeNodeFromBenMAPPollutant(monitorDataLine.Pollutant));
                if (monitorDataLine.MonitorAdvance != null)
                {
                    TreeNode treeNodeAdvance = new TreeNode();
                    treeNodeAdvance.Text = "Advanced";
                    treeNodeAdvance.Nodes.Add("Neighbor Scaling Type:" + (monitorDataLine.MonitorAdvance.WeightingApproach == WeightingApproachEnum.InverseDistance ? "Inverse Distance" : "Inverse Distance Squared"));
                    treeNode.Nodes.Add(treeNodeAdvance);
                    TreeNode treeNodeFiltering = new TreeNode();
                    treeNodeFiltering.Text = "Monitor Filtering";
                    treeNodeFiltering.Nodes.Add("Methods:" + monitorDataLine.MonitorAdvance.IncludeMethods);
                    treeNodeFiltering.Nodes.Add("Objectives:");
                    treeNodeFiltering.Nodes.Add("Maximum POC:" + monitorDataLine.MonitorAdvance.FilterMaximumPOC);
                    treeNodeFiltering.Nodes.Add("POC Preferences:" + monitorDataLine.MonitorAdvance.POCPreferenceOrder);
                    treeNodeFiltering.Nodes.Add("Minimum Lat, Long:" + monitorDataLine.MonitorAdvance.FilterMinLatitude + "," + monitorDataLine.MonitorAdvance.FilterMinLongitude);
                    treeNodeFiltering.Nodes.Add("Maximum Lat, Long:" + monitorDataLine.MonitorAdvance.FilterMaxLatitude + "," + monitorDataLine.MonitorAdvance.FilterMaxLongitude);
                    treeNodeFiltering.Nodes.Add("Number Required per Quarter:");
                    treeNodeFiltering.Nodes.Add("Types Used:" + monitorDataLine.MonitorAdvance.DataTypesToUse);
                    treeNodeFiltering.Nodes.Add("Type Preferred:" + monitorDataLine.MonitorAdvance.PreferredType);
                    treeNodeFiltering.Nodes.Add("Type Output:" + monitorDataLine.MonitorAdvance.OutputType);
                    treeNode.Nodes.Add(treeNodeFiltering);
                }
                return treeNode;
            }
            catch
            {
                return null;
            }
        }
        public static TreeNode getTreeNodeFromBenMAPGrid(BenMAPGrid benMAPGrid)
        {
            TreeNode treeNode = new TreeNode();
            treeNode.Text = "Grid Definition";
            try
            {
                treeNode.Nodes.Add("Name:" + benMAPGrid.GridDefinitionName);
                treeNode.Nodes.Add("ID:" + benMAPGrid.GridDefinitionID);
                treeNode.Nodes.Add("Columns:" + benMAPGrid.Columns);
                treeNode.Nodes.Add("Rows:" + benMAPGrid.RRows);
                treeNode.Nodes.Add("Grid Type:" + (benMAPGrid.TType == GridTypeEnum.Regular ? "Regular" : "Shapefile"));
                if (benMAPGrid is ShapefileGrid)
                {
                    treeNode.Nodes.Add("Shapefile Name:" + (benMAPGrid as ShapefileGrid).ShapefileName);
                }
                else if (benMAPGrid is RegularGrid)
                {
                    treeNode.Nodes.Add("Shapefile Name:" + (benMAPGrid as RegularGrid).ShapefileName);

                }

                return treeNode;
            }
            catch
            {
                return null;
            }
        }
        public static TreeNode getTreeNodeFromBaseControlGroup(BaseControlGroup baseControlGroup)
        {
            TreeNode treeNode = new TreeNode();
            treeNode.Text = "Baseline And Control Group";
            try
            {
                TreeNode tn = getTreeNodeFromBenMAPPollutant(baseControlGroup.Pollutant);
                treeNode.Nodes.Add(tn);
                tn = getTreeNodeFromBenMAPLine(baseControlGroup.Base);
                tn.Text = "Baseline : " + tn.Text;
                treeNode.Nodes.Add(tn);
                tn = getTreeNodeFromBenMAPLine(baseControlGroup.Control);
                tn.Text = "Control : " + tn.Text;
                treeNode.Nodes.Add(tn);

                return treeNode;
            }
            catch
            {
                return null;
            }
        }

        public static TreeNode getTreeNodeFromBaseControlCRSelectFunction(BaseControlCRSelectFunction baseControlCRSelectFunction)
        {
            TreeNode treeNode = new TreeNode();
            int i = 0;
            try
            {
                treeNode.Text = "Estimate Health Impacts";
                treeNode.Nodes.Add("Create Datetime:" + baseControlCRSelectFunction.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                treeNode.Nodes.Add("IsRunInPointMode:" + baseControlCRSelectFunction.CRRunInPointMode);
                treeNode.Nodes.Add("Default Monte Carlo Iterations:" + baseControlCRSelectFunction.CRDefaultMonteCarloIterations);
                if (!baseControlCRSelectFunction.CRRunInPointMode)
                {
                    treeNode.Nodes.Add("Latin Hypercube Points:" + baseControlCRSelectFunction.CRLatinHypercubePoints);
                }
                treeNode.Nodes.Add("Population Dataset:" + baseControlCRSelectFunction.BenMAPPopulation.DataSetName + "-" + baseControlCRSelectFunction.BenMAPPopulation.GridType.GridDefinitionName);
                treeNode.Nodes.Add("Year:" + baseControlCRSelectFunction.BenMAPPopulation.Year);
                treeNode.Nodes.Add("Threshold:" + baseControlCRSelectFunction.CRThreshold);

                String incidenceTxt = "";
                if (Configuration.ConfigurationCommonClass.indidenceAvgSelected == Configuration.ConfigurationCommonClass.incidenceAveraging.averageAll)
                {
                    incidenceTxt = "All";
                }
                else
                {
                    incidenceTxt = "Filtered";
                }
                treeNode.Nodes.Add("Incidence averaging:" + incidenceTxt);

                foreach (BaseControlGroup bcg in baseControlCRSelectFunction.BaseControlGroup)
                {
                    TreeNode tn = getTreeNodeFromBaseControlGroup(bcg);
                    tn.Text = tn.Text + i;
                    treeNode.Nodes.Add(tn);
                    i++;
                }
                TreeNode tnCR = new TreeNode();
                tnCR.Text = "Selected health impact functions";
                if (baseControlCRSelectFunction.lstCRSelectFunction != null)
                {
                    for (int iCR = 0; iCR < baseControlCRSelectFunction.lstCRSelectFunction.Count; iCR++)
                    {
                        TreeNode tnCROne = new TreeNode();
                        tnCROne.Text = "Health impact function " + iCR;
                        tnCROne.Nodes.Add("Health impact function dataset:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.DataSetName);
                        tnCROne.Nodes.Add("Endpoint group:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.EndPointGroup);
                        tnCROne.Nodes.Add("Endpoint:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.EndPoint);
                        if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].StartAge == -1)
                            tnCROne.Nodes.Add("Start age:");
                        else
                            tnCROne.Nodes.Add("Start age:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].StartAge);
                        if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].EndAge == -1)
                            tnCROne.Nodes.Add("End age:");
                        else
                            tnCROne.Nodes.Add("End age:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].EndAge);
                        tnCROne.Nodes.Add("Race:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].Race);
                        tnCROne.Nodes.Add("Ethnicity:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].Ethnicity);
                        tnCROne.Nodes.Add("Gender:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].Gender);
                        tnCROne.Nodes.Add("Pollutant:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Pollutant.PollutantName);
                        tnCROne.Nodes.Add("Metric:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Metric.MetricName);
                        tnCROne.Nodes.Add("Metric statistic:" + Enum.GetName(typeof(MetricStatic), baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.MetricStatistic));
                        if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.SeasonalMetric != null)
                            tnCROne.Nodes.Add("Seasonal metric:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName);

                        tnCROne.Nodes.Add("Author:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Author);
                        tnCROne.Nodes.Add("Qualifier:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Qualifier);
                        tnCROne.Nodes.Add("Function:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Function);
                        tnCROne.Nodes.Add("Year:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Year);
                        tnCROne.Nodes.Add("Geographic area:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].GeographicAreaName);
                        tnCROne.Nodes.Add("Other pollutants:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.OtherPollutants);

                        tnCROne.Nodes.Add("Reference:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Reference);

                        tnCROne.Nodes.Add("Baseline functional form:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.BaseLineIncidenceFunction);
                        tnCROne.Nodes.Add("Incidence dataset:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].IncidenceDataSetName);
                        tnCROne.Nodes.Add("Prevalence dataset:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].PrevalenceDataSetName);
                        tnCROne.Nodes.Add("Variable dataset:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].VariableDataSetName);
                        tnCROne.Nodes.Add("Beta:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Beta);
                        tnCROne.Nodes.Add("Beta distribution:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.BetaDistribution);
                        tnCROne.Nodes.Add("P1Beta:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.BetaParameter1);
                        tnCROne.Nodes.Add("P2Beta:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.BetaParameter2);
                        tnCROne.Nodes.Add("A:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.AContantValue);

                        tnCROne.Nodes.Add("NameA:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.AContantDescription);
                        tnCROne.Nodes.Add("B:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.BContantValue);
                        tnCROne.Nodes.Add("NameB:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.BContantDescription);
                        tnCROne.Nodes.Add("C:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.CContantValue);
                        tnCROne.Nodes.Add("NameC:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.CContantDescription);
                        tnCROne.Nodes.Add("Percentile:" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Percentile);
                        tnCR.Nodes.Add(tnCROne);
                    }
                }
                treeNode.Nodes.Add(tnCR);
                return treeNode;
            }
            catch
            {
                return null;
            }
        }

        public static TreeNode getTreeNodeFromBaseControlCRSelectFunctionCalculateValue(BaseControlCRSelectFunctionCalculateValue baseControlCRSelectFunctionCalculateValue)
        {
            TreeNode treeNode = new TreeNode();
            int i = 0;
            try
            {
                treeNode.Text = "Estimate Health Impacts";
                treeNode.Nodes.Add("Create Datetime:" + baseControlCRSelectFunctionCalculateValue.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                treeNode.Nodes.Add("IsRunInPointMode:" + baseControlCRSelectFunctionCalculateValue.CRRunInPointMode);
                treeNode.Nodes.Add("Default Monte Carlo Iterations:" + baseControlCRSelectFunctionCalculateValue.CRDefaultMonteCarloIterations);
                if (!baseControlCRSelectFunctionCalculateValue.CRRunInPointMode)
                {
                    treeNode.Nodes.Add("Latin Hypercube Points:" + baseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints);
                }
                treeNode.Nodes.Add("Population Dataset:" + baseControlCRSelectFunctionCalculateValue.BenMAPPopulation.DataSetName + "-" + baseControlCRSelectFunctionCalculateValue.BenMAPPopulation.GridType.GridDefinitionName);
                treeNode.Nodes.Add("Year:" + baseControlCRSelectFunctionCalculateValue.BenMAPPopulation.Year);
                treeNode.Nodes.Add("Threshold:" + baseControlCRSelectFunctionCalculateValue.CRThreshold);

                String incidenceTxt = "";
                if (Configuration.ConfigurationCommonClass.indidenceAvgSelected == Configuration.ConfigurationCommonClass.incidenceAveraging.averageAll)
                {
                    incidenceTxt = "All";
                }
                else
                {
                    incidenceTxt = "Filtered";
                }
                treeNode.Nodes.Add("Incidence averaging:" + incidenceTxt);

                foreach (BaseControlGroup bcg in baseControlCRSelectFunctionCalculateValue.BaseControlGroup)
                {
                    TreeNode tn = getTreeNodeFromBaseControlGroup(bcg);
                    tn.Text = tn.Text + i;
                    treeNode.Nodes.Add(tn);
                    i++;
                }
                TreeNode tnCR = new TreeNode();
                tnCR.Text = "Selected health impact functions";
                for (int iCR = 0; iCR < baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; iCR++)
                {
                    TreeNode tnCROne = new TreeNode();
                    tnCROne.Text = "Health impact function " + iCR;
                    tnCROne.Nodes.Add("Health impact function dataset:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.DataSetName);
                    tnCROne.Nodes.Add("Endpoint group:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup);
                    tnCROne.Nodes.Add("Endpoint:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.EndPoint);

                    if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.StartAge == -1)
                        tnCROne.Nodes.Add("Start age:");
                    else
                        tnCROne.Nodes.Add("Start age:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.StartAge);
                    if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.EndAge == -1)
                        tnCROne.Nodes.Add("End age:");
                    else
                        tnCROne.Nodes.Add("End age:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.EndAge);
                    tnCROne.Nodes.Add("Race:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.Race);
                    tnCROne.Nodes.Add("Ethnicity:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.Ethnicity);
                    tnCROne.Nodes.Add("Gender:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.Gender);
                    tnCROne.Nodes.Add("Pollutant:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName);
                    tnCROne.Nodes.Add("Metric:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName);
                    tnCROne.Nodes.Add("Metric statistic:" + Enum.GetName(typeof(MetricStatic), baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic));
                    if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
                        tnCROne.Nodes.Add("Seasonal metric:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName);

                    tnCROne.Nodes.Add("Author:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Author);
                    tnCROne.Nodes.Add("Qualifier:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Qualifier);
                    tnCROne.Nodes.Add("Function:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Function);
                    tnCROne.Nodes.Add("Year:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Year);
                    tnCROne.Nodes.Add("Geographic area:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.GeographicAreaName);
                    tnCROne.Nodes.Add("Other pollutants:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants);

                    tnCROne.Nodes.Add("Reference:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Reference);
                    tnCROne.Nodes.Add("Baseline functional form:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction);
                    tnCROne.Nodes.Add("Incidence dataset:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.IncidenceDataSetName);
                    tnCROne.Nodes.Add("Prevalence dataset:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.PrevalenceDataSetName);
                    tnCROne.Nodes.Add("Variable dataset:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.VariableDataSetName);
                    tnCROne.Nodes.Add("Beta:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Beta);
                    tnCROne.Nodes.Add("Beta distribution:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.BetaDistribution);
                    tnCROne.Nodes.Add("P1Beta:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter1);
                    tnCROne.Nodes.Add("P2Beta:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
                    tnCROne.Nodes.Add("A:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.AContantValue);

                    tnCROne.Nodes.Add("NameA:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.AContantDescription);
                    tnCROne.Nodes.Add("B:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.BContantValue);
                    tnCROne.Nodes.Add("NameB:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.BContantDescription);
                    tnCROne.Nodes.Add("C:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.CContantValue);
                    tnCROne.Nodes.Add("NameC:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.CContantDescription);
                    tnCROne.Nodes.Add("Percentile:" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Percentile);
                    tnCR.Nodes.Add(tnCROne);
                }
                treeNode.Nodes.Add(tnCR);
                if (baseControlCRSelectFunctionCalculateValue.lstLog != null && baseControlCRSelectFunctionCalculateValue.lstLog.Count > 0)
                {
                    TreeNode tnLog = new TreeNode();
                    tnLog.Text = "Log & Message";
                    for (int iCR = 0; iCR < baseControlCRSelectFunctionCalculateValue.lstLog.Count; iCR++)
                    {
                        tnLog.Nodes.Add(baseControlCRSelectFunctionCalculateValue.lstLog[iCR]);
                    }
                    treeNode.Nodes.Add(tnLog);
                }
                return treeNode;
            }
            catch
            {
                return null;
            }
        }

        public static TreeNode getTreeNodeFromIncidencePoolingAndAggregation(IncidencePoolingAndAggregation incidencePoolingAndAggregation)
        {
            TreeNode treeNode = new TreeNode();
            try
            {
                treeNode.Text = "Incidence Pooling And Aggregation ";


                TreeNode tn = new TreeNode();
                tn.Text = "Incidence Pooling Windows Name " + incidencePoolingAndAggregation.PoolingName;
                for (int iCR = 0; iCR < incidencePoolingAndAggregation.lstAllSelectCRFuntion.Count; iCR++)
                {
                    TreeNode tnCROne = new TreeNode();
                    getTreeNodeFromLstAllSelectCRFunction(incidencePoolingAndAggregation.lstAllSelectCRFuntion[iCR], incidencePoolingAndAggregation.lstAllSelectCRFuntion, ref tnCROne);
                    treeNode.Nodes.Add(tnCROne);
                }
                return treeNode;
            }
            catch
            {
                return null;
            }
        }

        public static TreeNode getTreeNodeFromIncidencePoolingAndAggregationAdvance(IncidencePoolingAndAggregationAdvance incidencePoolingAndAggregationAdvance)
        {
            TreeNode treeNode = new TreeNode();
            try
            {
                treeNode.Text = "Advanced";
                treeNode.Nodes.Add("Sort Incidence LHPs:" + incidencePoolingAndAggregationAdvance.SortIncidenceResults);
                treeNode.Nodes.Add("Default Advanced Pooling Method:" + Enum.GetName(typeof(IPAdvancePoolingMethodEnum), incidencePoolingAndAggregationAdvance.IPAdvancePoolingMethod));
                treeNode.Nodes.Add("Default Monte Carlo Iterations:" + incidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations);
                treeNode.Nodes.Add("Random Seed:" + incidencePoolingAndAggregationAdvance.RandomSeed);
                TreeNode trInflation = new TreeNode();
                trInflation.Text = "Inflation Adjustment";

                trInflation.Nodes.Add("Dataset:" + incidencePoolingAndAggregationAdvance.InflationDatasetName);
                trInflation.Nodes.Add("Year:" + incidencePoolingAndAggregationAdvance.CurrencyYear);
                treeNode.Nodes.Add(trInflation);
                TreeNode trIncome = new TreeNode();
                trIncome.Text = "Income Growth Adjustment";

                trIncome.Nodes.Add("Dataset:" + incidencePoolingAndAggregationAdvance.AdjustIncomeGrowthDatasetName);
                trIncome.Nodes.Add("Year : " + incidencePoolingAndAggregationAdvance.IncomeGrowthYear);

                if (incidencePoolingAndAggregationAdvance.EndpointGroups != null)
                {
                    try
                    {
                        Dictionary<string, double> dicIncome = APVX.APVCommonClass.getIncomeGrowthFactorsFromDataSetIDAndYear(incidencePoolingAndAggregationAdvance.AdjustIncomeGrowthDatasetID,
                           incidencePoolingAndAggregationAdvance.IncomeGrowthYear);
                        TreeNode tnEndpointGroups = new TreeNode();
                        tnEndpointGroups.Text = "Adjust Income Growth EndpointGroups";
                        foreach (string s in incidencePoolingAndAggregationAdvance.EndpointGroups)
                        {
                            if (dicIncome.ContainsKey(s))
                                tnEndpointGroups.Nodes.Add(s + ":" + dicIncome[s]);
                            else
                                tnEndpointGroups.Nodes.Add(s + ":1");
                        }
                        trIncome.Nodes.Add(tnEndpointGroups);
                    }
                    catch
                    {
                    }
                }
                treeNode.Nodes.Add(trIncome);

                if (incidencePoolingAndAggregationAdvance.IncidenceAggregation != null)
                {
                    TreeNode tn = new TreeNode();
                    tn.Text = "Incidence Aggregation";
                    tn.Nodes.Add("Name:" + incidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionName);
                    tn.Nodes.Add("ID:" + incidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID);
                    tn.Nodes.Add("Columns:" + incidencePoolingAndAggregationAdvance.IncidenceAggregation.Columns);
                    tn.Nodes.Add("Rows:" + incidencePoolingAndAggregationAdvance.IncidenceAggregation.RRows);
                    tn.Nodes.Add("Grid Type:" + Enum.GetName(typeof(GridTypeEnum), incidencePoolingAndAggregationAdvance.IncidenceAggregation.TType));
                    tn.Nodes.Add("Shapefile Name:" + ((incidencePoolingAndAggregationAdvance.IncidenceAggregation is ShapefileGrid) ? (incidencePoolingAndAggregationAdvance.IncidenceAggregation as ShapefileGrid).ShapefileName : (incidencePoolingAndAggregationAdvance.IncidenceAggregation as RegularGrid).ShapefileName));
                    treeNode.Nodes.Add(tn);


                }
                if (incidencePoolingAndAggregationAdvance.ValuationAggregation != null)
                {
                    TreeNode tn = new TreeNode();
                    tn.Text = "Valuation Aggregation";
                    tn.Nodes.Add("Name:" + incidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionName);
                    tn.Nodes.Add("ID:" + incidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID);
                    tn.Nodes.Add("Columns:" + incidencePoolingAndAggregationAdvance.ValuationAggregation.Columns);
                    tn.Nodes.Add("Rows:" + incidencePoolingAndAggregationAdvance.ValuationAggregation.RRows);
                    tn.Nodes.Add("Grid Type:" + Enum.GetName(typeof(GridTypeEnum), incidencePoolingAndAggregationAdvance.ValuationAggregation.TType));
                    tn.Nodes.Add("Shapefile Name:" + ((incidencePoolingAndAggregationAdvance.ValuationAggregation is ShapefileGrid) ? (incidencePoolingAndAggregationAdvance.ValuationAggregation as ShapefileGrid).ShapefileName : (incidencePoolingAndAggregationAdvance.ValuationAggregation as RegularGrid).ShapefileName));
                    treeNode.Nodes.Add(tn);
                }
                if (incidencePoolingAndAggregationAdvance.QALYAggregation != null)
                {
                    TreeNode tn = new TreeNode();
                    tn.Text = "QALYAggregation Aggregation";
                    tn.Nodes.Add("Name:" + incidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionName);
                    tn.Nodes.Add("ID:" + incidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID);
                    tn.Nodes.Add("Columns:" + incidencePoolingAndAggregationAdvance.QALYAggregation.Columns);
                    tn.Nodes.Add("Rows:" + incidencePoolingAndAggregationAdvance.QALYAggregation.RRows);
                    tn.Nodes.Add("Grid Type:" + Enum.GetName(typeof(GridTypeEnum), incidencePoolingAndAggregationAdvance.QALYAggregation.TType));
                    tn.Nodes.Add("Shapefile Name:" + ((incidencePoolingAndAggregationAdvance.QALYAggregation is ShapefileGrid) ? (incidencePoolingAndAggregationAdvance.QALYAggregation as ShapefileGrid).ShapefileName : (incidencePoolingAndAggregationAdvance.QALYAggregation as RegularGrid).ShapefileName));
                    treeNode.Nodes.Add(tn);
                }

                return treeNode;
            }
            catch
            {
                return null;
            }
        }

        public static void getTreeNodeFromLstAllSelectValuationMethod(AllSelectValuationMethod allSelectValuationMethodList, List<AllSelectValuationMethod> LstAllSelectValuationMethod, ref TreeNode treeNode)
        {
            List<AllSelectValuationMethod> lstOne = LstAllSelectValuationMethod.Where(p => p.PID == allSelectValuationMethodList.ID).ToList();

            if (lstOne != null && lstOne.Count > 0)
            {
                foreach (AllSelectValuationMethod asvm in lstOne)
                {

                    if (asvm.NodeType == 2000)
                    {
                        TreeNode tnvaluation = new TreeNode();
                        tnvaluation.Text = "Valuation Function";
                        tnvaluation.Nodes.Add("ID:" + asvm.BenMAPValuationFunction.ID);
                        tnvaluation.Nodes.Add("Dataset:" + asvm.BenMAPValuationFunction.DataSet);
                        tnvaluation.Nodes.Add("EndPointGroupID:" + asvm.BenMAPValuationFunction.EndPointGroupID);
                        tnvaluation.Nodes.Add("Endpoint group:" + asvm.BenMAPValuationFunction.EndPointGroup);
                        tnvaluation.Nodes.Add("EndPointID:" + asvm.BenMAPValuationFunction.EndPointID);
                        tnvaluation.Nodes.Add("Endpoint:" + asvm.BenMAPValuationFunction.EndPoint);

                        if (asvm.BenMAPValuationFunction.StartAge == -1)
                        {
                            tnvaluation.Nodes.Add("Start age:");
                        }
                        else
                        {
                            tnvaluation.Nodes.Add("Start age:" + asvm.BenMAPValuationFunction.StartAge);
                        }

                        if (asvm.BenMAPValuationFunction.EndAge == -1)
                        {
                            tnvaluation.Nodes.Add("End age:");
                        }
                        else {
                            tnvaluation.Nodes.Add("End age:" + asvm.BenMAPValuationFunction.EndAge);
                        }

                        tnvaluation.Nodes.Add("Qualifier:" + asvm.BenMAPValuationFunction.Qualifier);
                        tnvaluation.Nodes.Add("Reference:" + asvm.BenMAPValuationFunction.Reference);
                        tnvaluation.Nodes.Add("Function:" + asvm.BenMAPValuationFunction.Function);
                        tnvaluation.Nodes.Add("NameA:" + asvm.BenMAPValuationFunction.NameA);
                        tnvaluation.Nodes.Add("DistA:" + asvm.BenMAPValuationFunction.DistA);
                        tnvaluation.Nodes.Add("A:" + asvm.BenMAPValuationFunction.A);
                        tnvaluation.Nodes.Add("P1A:" + asvm.BenMAPValuationFunction.P1A);
                        tnvaluation.Nodes.Add("P2A:" + asvm.BenMAPValuationFunction.P2A);
                        tnvaluation.Nodes.Add("NameB:" + asvm.BenMAPValuationFunction.NameB);
                        tnvaluation.Nodes.Add("B:" + asvm.BenMAPValuationFunction.B);
                        tnvaluation.Nodes.Add("NameC:" + asvm.BenMAPValuationFunction.NameC);
                        tnvaluation.Nodes.Add("C:" + asvm.BenMAPValuationFunction.C);
                        tnvaluation.Nodes.Add("NameD:" + asvm.BenMAPValuationFunction.NameD);
                        tnvaluation.Nodes.Add("D:" + asvm.BenMAPValuationFunction.D);
                        tnvaluation.Nodes.Add("Weight:" + asvm.Weight);
                        treeNode.Nodes.Add(tnvaluation);
                    }
                    else
                    {
                        TreeNode tn = new TreeNode();
                        tn.Text = asvm.Name + ":Pooling Method Type:" + asvm.PoolingMethod;
                        getTreeNodeFromLstAllSelectValuationMethod(asvm, LstAllSelectValuationMethod, ref tn);
                        treeNode.Nodes.Add(tn);
                    }
                }
            }

        }

        public static void getTreeNodeFromLstAllSelectCRFunction(AllSelectCRFunction AllSelectCRFunctionList, List<AllSelectCRFunction> LstAllSelectCRFunction, ref TreeNode treeNode)
        {
            List<AllSelectCRFunction> lstOne = LstAllSelectCRFunction.Where(p => p.PID == AllSelectCRFunctionList.ID).ToList();

            if (lstOne != null && lstOne.Count > 0)
            {
                foreach (AllSelectCRFunction asvm in lstOne)
                {

                    if (asvm.NodeType == 100)
                    {
                        TreeNode tnvaluation = new TreeNode();
                        tnvaluation.Text = "Health impact function";
                        tnvaluation.Nodes.Add("Health impact function dataset:" + asvm.DataSet);
                        tnvaluation.Nodes.Add("Endpoint group:" + asvm.EndPointGroup);
                        tnvaluation.Nodes.Add("Endpoint:" + asvm.EndPoint);
                        tnvaluation.Nodes.Add("Pollutant:" + asvm.Pollutant);
                        tnvaluation.Nodes.Add("Metric:" + asvm.Metric); tnvaluation.Nodes.Add("Metric statistic:" + asvm.MetricStatistic);
                        tnvaluation.Nodes.Add("Author:" + asvm.Author);
                        tnvaluation.Nodes.Add("Year:" + asvm.Year);
                        tnvaluation.Nodes.Add("Location:" + asvm.Location);
                        tnvaluation.Nodes.Add("Other pollutants:" + asvm.OtherPollutants);
                        tnvaluation.Nodes.Add("Geographic area:" + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.GeographicAreaName);
                        try
                        {
                            tnvaluation.Nodes.Add("Reference:" + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Reference);
                            if (asvm.StartAge == "-1")
                                tnvaluation.Nodes.Add("Start age:");
                            else
                                tnvaluation.Nodes.Add("Start age:" + asvm.StartAge);
                            if (asvm.EndAge == "-1")
                                tnvaluation.Nodes.Add("End age:");
                            else
                                tnvaluation.Nodes.Add("End age:" + asvm.EndAge);
                            tnvaluation.Nodes.Add("Baseline functional form:" + asvm.OtherPollutants); tnvaluation.Nodes.Add("Incidence dataset:" + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.IncidenceDataSetName);
                            tnvaluation.Nodes.Add("Beta:" + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Beta);
                            tnvaluation.Nodes.Add("Beta distribution:" + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaDistribution);
                            tnvaluation.Nodes.Add("P1Beta:" + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter1);
                            tnvaluation.Nodes.Add("P2Beta:" + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
                            tnvaluation.Nodes.Add("A:" + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantValue);

                            tnvaluation.Nodes.Add("NameA:" + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantDescription);
                            tnvaluation.Nodes.Add("B:" + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantValue);
                            tnvaluation.Nodes.Add("NameB:" + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantDescription);
                            tnvaluation.Nodes.Add("C:" + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantValue);
                            tnvaluation.Nodes.Add("NameC:" + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantDescription);
                            tnvaluation.Nodes.Add("Percentile:" + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Percentile);
                        }
                        catch (Exception ex)
                        { }
                        tnvaluation.Nodes.Add("Weight:" + asvm.Weight);
                        treeNode.Nodes.Add(tnvaluation);
                    }
                    else
                    {
                        TreeNode tn = new TreeNode();
                        tn.Text = asvm.Name + ":Pooling Method Type:" + asvm.PoolingMethod;
                        getTreeNodeFromLstAllSelectCRFunction(asvm, LstAllSelectCRFunction, ref tn);
                        treeNode.Nodes.Add(tn);
                    }

                }
                treeNode.Text = AllSelectCRFunctionList.Name + ":Pooling Method Type:" + AllSelectCRFunctionList.PoolingMethod;
            }
            else
            {
                if (AllSelectCRFunctionList.NodeType == 100)
                {
                    treeNode.Text = "Health impact function";
                    try
                    {
                        treeNode.Nodes.Add("Health impact function dataset:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName);
                        treeNode.Nodes.Add("Endpoint group:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup);
                        treeNode.Nodes.Add("Endpoint:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint);
                        treeNode.Nodes.Add("Pollutant:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName);
                        treeNode.Nodes.Add("Metric:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName);
                        treeNode.Nodes.Add("Metric statistic:" + Enum.GetName(typeof(MetricStatic), AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic));
                        if (AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
                            treeNode.Nodes.Add("Seasonal metric:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName);

                        treeNode.Nodes.Add("Author:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author);
                        treeNode.Nodes.Add("Qualifier:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier);
                        treeNode.Nodes.Add("Function:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Function);
                        treeNode.Nodes.Add("Year:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Year);
                        treeNode.Nodes.Add("Geographic area:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.GeographicAreaName);
                        treeNode.Nodes.Add("Other pollutants:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants);

                        treeNode.Nodes.Add("Reference:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Reference);
                        if (AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.StartAge == -1)
                            treeNode.Nodes.Add("Start age:");
                        else
                            treeNode.Nodes.Add("Start age:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.StartAge);
                        if (AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndAge == -1)
                            treeNode.Nodes.Add("End age:");
                        else
                            treeNode.Nodes.Add("End age:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndAge);
                        treeNode.Nodes.Add("Baseline functional form:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction);
                        treeNode.Nodes.Add("Incidence dataset:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.IncidenceDataSetName);
                        treeNode.Nodes.Add("Beta:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Beta);
                        treeNode.Nodes.Add("Beta distribution:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaDistribution);
                        treeNode.Nodes.Add("P1Beta:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter1);
                        treeNode.Nodes.Add("P2Beta:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
                        treeNode.Nodes.Add("A:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantValue);

                        treeNode.Nodes.Add("NameA:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantDescription);
                        treeNode.Nodes.Add("B:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantValue);
                        treeNode.Nodes.Add("NameB:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantDescription);
                        treeNode.Nodes.Add("C:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantValue);
                        treeNode.Nodes.Add("NameC:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantDescription);
                        treeNode.Nodes.Add("Percentile:" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Percentile);
                        treeNode.Nodes.Add("Weight:" + AllSelectCRFunctionList.Weight);
                    }
                    catch
                    { }

                }
                else
                {
                    treeNode.Text = AllSelectCRFunctionList.Name + ":Pooling Method Type:" + AllSelectCRFunctionList.PoolingMethod + " Weight:" + AllSelectCRFunctionList.Weight;
                }


            }

        }

        public static void getTreeNodeFromAllDatasetsMetadata(List<MetadataClassObj> lstMetadataObjs, ref TreeNode treeNode)
        {



        }




        public static TreeNode getTreeNodeFromValuationMethodPoolingAndAggregationBase(ValuationMethodPoolingAndAggregationBase valuationMethodPoolingAndAggregationBase)
        {
            TreeNode treeNode = new TreeNode();
            try
            {
                treeNode.Text = valuationMethodPoolingAndAggregationBase.IncidencePoolingAndAggregation.PoolingName;

                treeNode.Nodes.Add(getTreeNodeFromIncidencePoolingAndAggregation(valuationMethodPoolingAndAggregationBase.IncidencePoolingAndAggregation));
                if (valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod != null && valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod.Count > 0)
                {
                    TreeNode tn = new TreeNode();
                    tn.Text = "Valuation Pooling Window Name " + valuationMethodPoolingAndAggregationBase.IncidencePoolingAndAggregation.PoolingName;
                    tn.Nodes.Add(valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod[0].Name);
                    getTreeNodeFromLstAllSelectValuationMethod(valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod[0], valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod, ref tn);
                    treeNode.Nodes.Add(tn);
                }


                return treeNode;
            }
            catch
            {
                return null;
            }
        }
        public static TreeNode getTreeNodeFromValuationMethodPoolingAndAggregation(ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation)
        {
            TreeNode treeNode = new TreeNode();
            try
            {
                treeNode.Text = "Aggregate, Pool & Value";
                treeNode.Nodes.Add("Create Datetime:" + valuationMethodPoolingAndAggregation.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                treeNode.Nodes.Add(getTreeNodeFromBaseControlCRSelectFunctionCalculateValue(valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue));
                if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null)
                {
                    treeNode.Nodes.Add(getTreeNodeFromIncidencePoolingAndAggregationAdvance(valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance));
                }
                foreach (ValuationMethodPoolingAndAggregationBase vb in valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                {
                    treeNode.Nodes.Add(getTreeNodeFromValuationMethodPoolingAndAggregationBase(vb));

                }
                if (valuationMethodPoolingAndAggregation.lstLog != null && valuationMethodPoolingAndAggregation.lstLog.Count > 0)
                {
                    TreeNode tnLog = new TreeNode();
                    tnLog.Text = "Log & Message";
                    for (int iCR = 0; iCR < valuationMethodPoolingAndAggregation.lstLog.Count; iCR++)
                    {
                        tnLog.Nodes.Add(valuationMethodPoolingAndAggregation.lstLog[iCR]);
                    }
                    treeNode.Nodes.Add(tnLog);
                }
                return treeNode;
            }
            catch
            {
                return null;
            }
        }


        private static StreamWriter sw;
        public static bool exportToTxt(TreeView tv, string filename)
        {
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Create);
                sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(tv.Nodes[0].Text);
                foreach (TreeNode node in tv.Nodes)
                {
                    saveNode(node.Nodes);
                }
                sw.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }
        public static bool exportToXml(TreeView tv, string filename)
        {
            try
            {
                sw = new StreamWriter(filename, false, System.Text.Encoding.UTF8);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                string txtWithoutSpace = tv.Nodes[0].Text;
                txtWithoutSpace = txtWithoutSpace.Replace(" ", ".");
                txtWithoutSpace = txtWithoutSpace.Replace("&", "And");
                txtWithoutSpace = txtWithoutSpace.Replace(":", "");
                txtWithoutSpace = txtWithoutSpace.Replace("..", ".");
                sw.WriteLine("<" + txtWithoutSpace + ">");

                foreach (TreeNode node in tv.Nodes)
                {
                    saveNode(node.Nodes);
                }
                sw.WriteLine("</" + txtWithoutSpace + ">");
                sw.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }
        }

        private static void saveNode(TreeNodeCollection tnc)
        {
            foreach (TreeNode node in tnc)
            {
                if (node.Nodes.Count > 0)
                {
                    string txtWithoutSpace = node.Text;
                    txtWithoutSpace = txtWithoutSpace.Replace(" ", ".");
                    txtWithoutSpace = txtWithoutSpace.Replace("&", "And");
                    txtWithoutSpace = txtWithoutSpace.Replace(":", "");
                    txtWithoutSpace = txtWithoutSpace.Replace("..", ".");

                    sw.WriteLine("<" + txtWithoutSpace + ">");
                    saveNode(node.Nodes);
                    sw.WriteLine("</" + txtWithoutSpace + ">");
                }
                else sw.WriteLine(node.Text);
            }
        }

        public static int generateAuditTrailReportTreeView(TreeView trv)
        {
            int retVal = 0;

            if (CommonClass.ValuationMethodPoolingAndAggregation != null)
            {
                ValuationMethodPoolingAndAggregation apvrVMPA = new ValuationMethodPoolingAndAggregation();
                apvrVMPA = CommonClass.ValuationMethodPoolingAndAggregation;
                TreeNode apvrTreeNode = new TreeNode();
                apvrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromValuationMethodPoolingAndAggregation(apvrVMPA);
                trv.Nodes.Clear();
                trv.Nodes.Add(apvrVMPA.Version == null ? "BenMAP-CE" : apvrVMPA.Version);
                trv.Nodes.Add(apvrTreeNode);
            }
            else if (CommonClass.BaseControlCRSelectFunctionCalculateValue != null)
            {
                BaseControlCRSelectFunctionCalculateValue cfgrFunctionCV = new BaseControlCRSelectFunctionCalculateValue();
                cfgrFunctionCV = CommonClass.BaseControlCRSelectFunctionCalculateValue;
                TreeNode cfgrTreeNode = new TreeNode();
                cfgrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunctionCalculateValue(cfgrFunctionCV);
                trv.Nodes.Clear();
                trv.Nodes.Add(cfgrFunctionCV.Version == null ? "BenMAP-CE" : cfgrFunctionCV.Version);
                trv.Nodes.Add(cfgrTreeNode);
            }
            else if (CommonClass.BaseControlCRSelectFunction != null)
            {
                BaseControlCRSelectFunction cfgFunction = new BaseControlCRSelectFunction();
                cfgFunction = CommonClass.BaseControlCRSelectFunction;
                TreeNode cfgTreeNode = new TreeNode();
                cfgTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunction(cfgFunction);
                trv.Nodes.Clear();
                trv.Nodes.Add(cfgFunction.Version == null ? "BenMAP-CE" : cfgFunction.Version);
                trv.Nodes.Add(cfgTreeNode);
            }
            else
            {
                //incomplete configuration
                //MessageBox.Show("Please finish your configuration first.");
                retVal = -1;
            }

            return retVal;
        }

        public static int exportToCtlx(string filePath)
        {
            int retVal = 0;

            if (CommonClass.ValuationMethodPoolingAndAggregation != null)
            {
                ValuationMethodPoolingAndAggregation apvrVMPA = new ValuationMethodPoolingAndAggregation();
                apvrVMPA = CommonClass.ValuationMethodPoolingAndAggregation;
                APVX.APVCommonClass.SaveAPVFile(filePath.Substring(0, filePath.Length - 4) + "apvx", apvrVMPA);
                BatchCommonClass.OutputAPV(apvrVMPA, filePath, filePath.Substring(0, filePath.Length - 4) + "apvx");
                //MessageBox.Show("Configuration file saved.", "File saved");
            }
            else if (CommonClass.BaseControlCRSelectFunction != null)
            {
                BaseControlCRSelectFunction cfgFunction = new BaseControlCRSelectFunction();
                cfgFunction = CommonClass.BaseControlCRSelectFunction;
                Configuration.ConfigurationCommonClass.SaveCFGFile(CommonClass.BaseControlCRSelectFunction, filePath.Substring(0, filePath.Length - 4) + "cfgx");
                BatchCommonClass.OutputCFG(cfgFunction, filePath, filePath.Substring(0, filePath.Length - 4) + "cfgx");
                //MessageBox.Show("Configuration file saved.", "File saved");
            }
            else
            {
                //incomplete configuration
                //MessageBox.Show("Please finish your configuration first.");
                retVal = -1;
            }                  

            return retVal;
        }




    }
}
