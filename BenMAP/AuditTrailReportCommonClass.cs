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
				treeNode.Text = "Pollutant";
				treeNode.Nodes.Add("Observation Type: " + ((benMAPPollutant.Observationtype == ObservationtypeEnum.Daily) ? "Daily" : "Hourly"));
				if (benMAPPollutant.Seasons != null && benMAPPollutant.Seasons.Count > 0)
				{
					for (int i = 1; i < benMAPPollutant.Seasons.Count + 1; i++)
					{
						dt = new DateTime(2011, 1, 1);
						treeNode.Nodes.Add("Season " + i + ": " + dt.AddDays(benMAPPollutant.Seasons[i - 1].StartDay).GetDateTimeFormats('M')[0].ToString() + "-" + dt.AddDays(benMAPPollutant.Seasons[i - 1].EndDay).GetDateTimeFormats('M')[0].ToString());
					}

				}
				if (benMAPPollutant.Metrics != null && benMAPPollutant.Metrics.Count > 0)
				{
					for (int i = 1; i < benMAPPollutant.Metrics.Count + 1; i++)
					{
						treeNode.Nodes.Add("Metric " + i + ": " + benMAPPollutant.Metrics[i - 1].MetricName);
					}

				}
				if (benMAPPollutant.SesonalMetrics != null && benMAPPollutant.SesonalMetrics.Count > 0)
				{
					for (int i = 1; i < benMAPPollutant.SesonalMetrics.Count + 1; i++)
					{
						treeNode.Nodes.Add("Seasonal Metric " + i + ": " + benMAPPollutant.SesonalMetrics[i - 1].SeasonalMetricName);
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
				treeNode.Nodes.Insert(0, "Create Datetime: " + benMAPLine.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
				treeNode.Expand();
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
			treeNode.Text = "Air Quality Surfaces";         //For multi-pollutant, discussed possibility of desiginating the group under which the pollutants fall--i.e. "Air Quality Surfaces (Traffic Group)"
			try
			{
				treeNode.Nodes.Add("Model Database File: " + modelDataLine.DatabaseFilePath);
				treeNode.Expand();
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
			treeNode.Text = "Air Quality Surfaces";         //For multi-pollutant, discussed possibility of desiginating the group under which the pollutants fall--i.e. "Air Quality Surfaces (Traffic Group)"
			try
			{
				FireBirdHelperBase fb = new ESILFireBirdHelper();
				string commandText = string.Format("select monitordatasetname from MonitorDatasets where monitordatasetid={0}", monitorDataLine.MonitorDataSetID);
				string datasetName = Convert.ToString(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
				treeNode.Nodes.Add("Monitor Dataset Name: " + datasetName);
				treeNode.Nodes.Add("Monitor Year: " + monitorDataLine.MonitorLibraryYear);
				treeNode.Nodes.Add("Interpolation Method: " + monitorDataLine.InterpolationMethod);
				treeNode.Nodes.Add("Monitor Data Source: " + (monitorDataLine.MonitorDirectType == 0 ? "Library" : "Text File"));
				if (monitorDataLine is MonitorModelRollbackLine)
				{
					treeNode.Nodes.Add(getTreeNodeFromBenMAPRollback(monitorDataLine));
				}

				if (monitorDataLine.MonitorAdvance != null)
				{
					TreeNode treeNodeAdvance = new TreeNode();
					treeNodeAdvance.Text = "Advanced";
					treeNodeAdvance.Nodes.Add("Neighbor Scaling Type: " + (monitorDataLine.MonitorAdvance.WeightingApproach == WeightingApproachEnum.InverseDistance ? "Inverse Distance" : "Inverse Distance Squared"));
					treeNode.Nodes.Add(treeNodeAdvance);
					TreeNode treeNodeFiltering = new TreeNode();
					treeNodeFiltering.Text = "Monitor Filtering";
					treeNodeFiltering.Nodes.Add("Methods: " + monitorDataLine.MonitorAdvance.IncludeMethods);
					treeNodeFiltering.Nodes.Add("Objectives: ");
					treeNodeFiltering.Nodes.Add("Maximum POC: " + monitorDataLine.MonitorAdvance.FilterMaximumPOC);
					treeNodeFiltering.Nodes.Add("POC Preferences: " + monitorDataLine.MonitorAdvance.POCPreferenceOrder);
					treeNodeFiltering.Nodes.Add("Minimum Lat, Long: " + monitorDataLine.MonitorAdvance.FilterMinLatitude + "," + monitorDataLine.MonitorAdvance.FilterMinLongitude);
					treeNodeFiltering.Nodes.Add("Maximum Lat, Long: " + monitorDataLine.MonitorAdvance.FilterMaxLatitude + "," + monitorDataLine.MonitorAdvance.FilterMaxLongitude);
					treeNodeFiltering.Nodes.Add("Number Required per Quarter: ");
					treeNodeFiltering.Nodes.Add("Types Used: " + monitorDataLine.MonitorAdvance.DataTypesToUse);
					treeNodeFiltering.Nodes.Add("Type Preferred: " + monitorDataLine.MonitorAdvance.PreferredType);
					treeNodeFiltering.Nodes.Add("Type Output: " + monitorDataLine.MonitorAdvance.OutputType);
					treeNode.Nodes.Add(treeNodeFiltering);
				}
				treeNode.Expand();
				return treeNode;
			}
			catch
			{
				return null;
			}
		}
		public static TreeNode getTreeNodeFromBenMAPRollback(MonitorDataLine monitorDataLine)
		{
			try
			{
				TreeNode treeNode = new TreeNode();

				treeNode.Text = "Rollback";
				List<BenMAPRollback> lstBenMapRollback = (monitorDataLine as MonitorModelRollbackLine).BenMAPRollbacks;

				foreach (BenMAPRollback brb in lstBenMapRollback)
				{
					TreeNode treeNodebrb = new TreeNode();                                              //Options for Rollback: Percentage, Incremental, Standard
					if (brb is PercentageRollback)
					{
						treeNodebrb.Text = ("Rollback Type: Percentage");                           //hard-coded rollback type because the .RollbackType was all lower case
						treeNodebrb.Nodes.Add("Rollback Amount: " + (brb as PercentageRollback).Percent + "%");
						treeNodebrb.Nodes.Add("Background Concentration: " + (brb as PercentageRollback).Background);
					}
					else if (brb is IncrementalRollback)
					{
						treeNodebrb.Text = ("Rollback Type: Incremental");
						treeNodebrb.Nodes.Add("Rollback Amount: " + (brb as IncrementalRollback).Increment);
						treeNodebrb.Nodes.Add("Background Concentration: " + (brb as IncrementalRollback).Background);
					}
					else if (brb is StandardRollback)
					{
						treeNodebrb.Text = ("Rollback Type: Standard");
						treeNodebrb.Nodes.Add("Daily Metric: " + (brb as StandardRollback).DailyMetric.MetricName);
						if ((brb as StandardRollback).SeasonalMetric != null)
						{
							treeNodebrb.Nodes.Add("Seasonal Metric: " + (brb as StandardRollback).SeasonalMetric);
						}
						if ((brb as StandardRollback).AnnualStatistic != null)
						{
							treeNodebrb.Nodes.Add("Annual Statistic: " + (brb as StandardRollback).AnnualStatistic);
						}
						treeNodebrb.Nodes.Add("Rollback Ordinality: " + (brb as StandardRollback).Ordinality);
						treeNodebrb.Nodes.Add("Rollback Standard: " + (brb as StandardRollback).Standard);
						treeNodebrb.Nodes.Add("Rollback Background: " + (brb as StandardRollback).Background);
						treeNodebrb.Nodes.Add("Interday Rollback Method: " + (brb as StandardRollback).InterdayRollbackMethod);
						treeNodebrb.Nodes.Add("Interday Background Concentration: " + (brb as StandardRollback).InterdayBackground);
						treeNodebrb.Nodes.Add("Intraday Rollback Method: " + (brb as StandardRollback).IntradayRollbackMethod);
						treeNodebrb.Nodes.Add("Intraday Background Concentration: " + (brb as StandardRollback).IntradayBackground);
					}
					//The commented-out section below will provide the row and column of selected regions. 
					//string tempRowCol = "";
					//foreach (RowCol rowCol in brb.SelectRegions)
					//{
					//    tempRowCol = String.Concat(tempRowCol, "[", rowCol.Col.ToString(), ", ", rowCol.Row.ToString(), "] ");
					//}
					//treeNodebrb.Nodes.Add("Grid Region #: " + tempRowCol);
					treeNodebrb.Collapse();
					treeNode.Nodes.Add(treeNodebrb);
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
				treeNode.Nodes.Add("Name: " + benMAPGrid.GridDefinitionName);
				// treeNode.Nodes.Add("ID: " + benMAPGrid.GridDefinitionID);
				treeNode.Nodes.Add("Columns: " + benMAPGrid.Columns);
				treeNode.Nodes.Add("Rows: " + benMAPGrid.RRows);
				treeNode.Nodes.Add("Grid Type: " + (benMAPGrid.TType == GridTypeEnum.Regular ? "Regular" : "Shapefile"));
				if (benMAPGrid is ShapefileGrid)
				{
					treeNode.Nodes.Add("Shapefile Name: " + (benMAPGrid as ShapefileGrid).ShapefileName);
				}
				else if (benMAPGrid is RegularGrid)
				{
					treeNode.Nodes.Add("Shapefile Name: " + (benMAPGrid as RegularGrid).ShapefileName);

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

			try
			{
				treeNode.Text = "Baseline and Control (" + baseControlGroup.Pollutant.PollutantName + ")";
				TreeNode tn = getTreeNodeFromBenMAPPollutant(baseControlGroup.Pollutant);
				treeNode.Nodes.Add(tn);
				tn = getTreeNodeFromBenMAPLine(baseControlGroup.Base);
				tn.Text = "Baseline";
				treeNode.Nodes.Add(tn);
				tn = getTreeNodeFromBenMAPLine(baseControlGroup.Control);
				tn.Text = "Control";
				treeNode.Nodes.Add(tn);
				TreeNode shpGrid = getTreeNodeFromBenMAPGrid(baseControlGroup.GridType);
				treeNode.Nodes.Add(shpGrid);

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
			int i = 1;
			try
			{
				treeNode.Text = "Estimate Health Impacts";
				treeNode.Nodes.Add("Create Datetime: " + baseControlCRSelectFunction.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
				TreeNode popNode = new TreeNode();
				popNode.Text = "Population Dataset";
				popNode.Nodes.Add("Population Dataset Name: " + baseControlCRSelectFunction.BenMAPPopulation.DataSetName);
				popNode.Nodes.Add("Population Dataset Year: " + baseControlCRSelectFunction.BenMAPPopulation.Year);
				popNode.Nodes.Add("Grid Definition: " + baseControlCRSelectFunction.BenMAPPopulation.GridType.GridDefinitionName);
				treeNode.Nodes.Add(popNode);

				TreeNode tnCR = new TreeNode();
				tnCR.Text = "Selected Health Impact Functions";
				if (baseControlCRSelectFunction.lstCRSelectFunction != null)
				{
					for (int iCR = 0; iCR < baseControlCRSelectFunction.lstCRSelectFunction.Count; iCR++)
					{
						TreeNode tnCROne = new TreeNode();
						tnCROne.Text = "Health Impact Function (" + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Author + ", " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Year + ", " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.EndPointGroup + ")";
						tnCROne.Nodes.Add("Health Impact Function Dataset: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.DataSetName);
						tnCROne.Nodes.Add("Endpoint: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.EndPoint);
						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].StartAge == -1)
							tnCROne.Nodes.Add("Start Age: ");
						else
							tnCROne.Nodes.Add("Start Age: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].StartAge);
						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].EndAge == -1)
							tnCROne.Nodes.Add("End Age: ");
						else
							tnCROne.Nodes.Add("End Age: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].EndAge);

						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].Race != "")
							tnCROne.Nodes.Add("Race: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].Race);
						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].Ethnicity != "")
							tnCROne.Nodes.Add("Ethnicity: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].Ethnicity);
						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].Gender != "")
							tnCROne.Nodes.Add("Gender: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].Gender);

						tnCROne.Nodes.Add("Pollutant: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Pollutant.PollutantName);
						tnCROne.Nodes.Add("Metric: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Metric.MetricName);
						tnCROne.Nodes.Add("Metric Statistic: " + Enum.GetName(typeof(MetricStatic), baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.MetricStatistic));
						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.SeasonalMetric != null)
							tnCROne.Nodes.Add("Seasonal Metric: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName);

						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Qualifier != "")
							tnCROne.Nodes.Add("Qualifier: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Qualifier);

						tnCROne.Nodes.Add("Function: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Function);
						tnCROne.Nodes.Add("Geographic Area: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].GeographicAreaName);
						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.OtherPollutants != "")
							tnCROne.Nodes.Add("Other Pollutants: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.OtherPollutants);

						tnCROne.Nodes.Add("Reference: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Reference);

						tnCROne.Nodes.Add("Baseline Functional Form: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.BaseLineIncidenceFunction);

						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].IncidenceDataSetName != null)
							tnCROne.Nodes.Add("Incidence Dataset: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].IncidenceDataSetName);
						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].PrevalenceDataSetName != null)
							tnCROne.Nodes.Add("Prevalence Dataset: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].PrevalenceDataSetName);
						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].VariableDataSetName != null)
							tnCROne.Nodes.Add("Variable Dataset: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].VariableDataSetName);

						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Beta != 0)
							tnCROne.Nodes.Add("Beta: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Beta);
						tnCROne.Nodes.Add("Beta distribution: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.BetaDistribution);
						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.BetaParameter1 != 0)
							tnCROne.Nodes.Add("P1Beta: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.BetaParameter1);
						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.BetaParameter2 != 0)
							tnCROne.Nodes.Add("P2Beta: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.BetaParameter2);

						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.AContantValue != 0)
							tnCROne.Nodes.Add("A: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.AContantValue);
						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.AContantDescription != "")
							tnCROne.Nodes.Add("Name A: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.AContantDescription);
						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.BContantValue != 0)
							tnCROne.Nodes.Add("B: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.BContantValue);
						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.BContantDescription != "")
							tnCROne.Nodes.Add("Name B: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.BContantDescription);
						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.CContantValue != 0)
							tnCROne.Nodes.Add("C: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.CContantValue);
						if (baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.CContantDescription != "")
							tnCROne.Nodes.Add("Name C: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.CContantDescription);
						tnCROne.Nodes.Add("Percentile: " + baseControlCRSelectFunction.lstCRSelectFunction[iCR].BenMAPHealthImpactFunction.Percentile);
						tnCR.Nodes.Add(tnCROne);
					}
				}
				treeNode.Nodes.Add(tnCR);
				TreeNode advNode = new TreeNode();
				advNode.Text = "Advanced Settings";
				advNode.Nodes.Add("Is Run In Point Mode: " + baseControlCRSelectFunction.CRRunInPointMode);
				if (!baseControlCRSelectFunction.CRRunInPointMode)
				{
					advNode.Nodes.Add("Latin Hypercube Points: " + baseControlCRSelectFunction.CRLatinHypercubePoints);
				}
				advNode.Nodes.Add("Default Monte Carlo Iterations: " + String.Format("{0:n0}", baseControlCRSelectFunction.CRDefaultMonteCarloIterations));
				advNode.Nodes.Add("Random Seed: " + baseControlCRSelectFunction.CRSeeds);
				advNode.Nodes.Add("Threshold: " + baseControlCRSelectFunction.CRThreshold);

				String incidenceTxt = "";
				if (Configuration.ConfigurationCommonClass.incidenceAvgSelected == Configuration.ConfigurationCommonClass.incidenceAveraging.averageAll)
				{
					incidenceTxt = "All";
				}
				else
				{
					incidenceTxt = "Filtered";
				}
				advNode.Nodes.Add("Incidence averaging: " + incidenceTxt);
				treeNode.Nodes.Add(advNode);
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
			int i = 1;
			try
			{
				treeNode.Text = "Estimate Health Impacts";
				treeNode.Nodes.Add("Create Datetime: " + baseControlCRSelectFunctionCalculateValue.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
				TreeNode popNode = new TreeNode();
				popNode.Text = "Population Dataset";
				popNode.Nodes.Add("Population Dataset Name: " + baseControlCRSelectFunctionCalculateValue.BenMAPPopulation.DataSetName);
				popNode.Nodes.Add("Population Dataset Year: " + baseControlCRSelectFunctionCalculateValue.BenMAPPopulation.Year);
				popNode.Nodes.Add("Grid Definition: " + baseControlCRSelectFunctionCalculateValue.BenMAPPopulation.GridType.GridDefinitionName);
				popNode.Nodes.Add("Population Configuration: " + baseControlCRSelectFunctionCalculateValue.BenMAPPopulation.GridType.SetupName);
				treeNode.Nodes.Add(popNode);

				TreeNode tnCR = new TreeNode();
				tnCR.Text = "Selected Health Impact Functions";
				for (int iCR = 0; iCR < baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; iCR++)
				{
					TreeNode tnCROne = new TreeNode();
					tnCROne.Text = "Health Impact Function (" + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Author + ", " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Year + ", " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup + ")";
					tnCROne.Nodes.Add("Health Impact Function Dataset: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.DataSetName);
					tnCROne.Nodes.Add("Endpoint: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.EndPoint);

					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.StartAge == -1)
						tnCROne.Nodes.Add("Start Age: ");
					else
						tnCROne.Nodes.Add("Start Age: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.StartAge);
					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.EndAge == -1)
						tnCROne.Nodes.Add("End Age: ");
					else
						tnCROne.Nodes.Add("End Age: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.EndAge);

					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.Race != "")
						tnCROne.Nodes.Add("Race: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.Race);
					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.Ethnicity != "")
						tnCROne.Nodes.Add("Ethnicity: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.Ethnicity);
					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.Gender != "")
						tnCROne.Nodes.Add("Gender: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.Gender);


					tnCROne.Nodes.Add("Pollutant: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName);
					tnCROne.Nodes.Add("Metric: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName);
					tnCROne.Nodes.Add("Metric Statistic: " + Enum.GetName(typeof(MetricStatic), baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic));
					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
						tnCROne.Nodes.Add("Seasonal Metric: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName);

					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Qualifier != "")
						tnCROne.Nodes.Add("Qualifier: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Qualifier);

					tnCROne.Nodes.Add("Function: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Function);
					tnCROne.Nodes.Add("Geographic Area: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.GeographicAreaName);

					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants != "")
						tnCROne.Nodes.Add("Other Pollutants: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants);

					tnCROne.Nodes.Add("Reference: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Reference);
					tnCROne.Nodes.Add("Baseline Functional Form: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction);

					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.IncidenceDataSetName != null)
						tnCROne.Nodes.Add("Incidence Dataset: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.IncidenceDataSetName);
					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.PrevalenceDataSetName != null)
						tnCROne.Nodes.Add("Prevalence Dataset: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.PrevalenceDataSetName);
					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.VariableDataSetName != null)
						tnCROne.Nodes.Add("Variable Dataset: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.VariableDataSetName);

					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Beta != 0)
						tnCROne.Nodes.Add("Beta: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Beta);
					tnCROne.Nodes.Add("Beta distribution: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.BetaDistribution);
					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter1 != 0)
						tnCROne.Nodes.Add("P1Beta: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter1);
					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter2 != 0)
						tnCROne.Nodes.Add("P2Beta: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);

					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.AContantValue != 0)
						tnCROne.Nodes.Add("A: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.AContantValue);
					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.AContantDescription != "")
						tnCROne.Nodes.Add("Name A: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.AContantDescription);
					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.BContantValue != 0)
						tnCROne.Nodes.Add("B: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.BContantValue);
					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.BContantDescription != "")
						tnCROne.Nodes.Add("Name B: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.BContantDescription);
					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.CContantValue != 0)
						tnCROne.Nodes.Add("C: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.CContantValue);
					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.CContantDescription != "")
						tnCROne.Nodes.Add("Name C: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.CContantDescription);
					if (baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Percentile != 0)
						tnCROne.Nodes.Add("Percentile: " + baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[iCR].CRSelectFunction.BenMAPHealthImpactFunction.Percentile);
					tnCR.Nodes.Add(tnCROne);
				}
				treeNode.Nodes.Add(tnCR);
				TreeNode advNode = new TreeNode();
				advNode.Text = "Advanced Settings";
				advNode.Nodes.Add("Is Run In Point Mode: " + baseControlCRSelectFunctionCalculateValue.CRRunInPointMode);
				if (!baseControlCRSelectFunctionCalculateValue.CRRunInPointMode)
				{
					advNode.Nodes.Add("Latin Hypercube Points: " + baseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints);
				}
				advNode.Nodes.Add("Default Monte Carlo Iterations: " + String.Format("{0:n0}", baseControlCRSelectFunctionCalculateValue.CRDefaultMonteCarloIterations));
				advNode.Nodes.Add("Random Seed: " + baseControlCRSelectFunctionCalculateValue.CRSeeds);
				advNode.Nodes.Add("Threshold: " + baseControlCRSelectFunctionCalculateValue.CRThreshold);

				String incidenceTxt = "";
				if (Configuration.ConfigurationCommonClass.incidenceAvgSelected == Configuration.ConfigurationCommonClass.incidenceAveraging.averageAll)
				{
					incidenceTxt = "All";
				}
				else
				{
					incidenceTxt = "Filtered";
				}
				advNode.Nodes.Add("Incidence averaging: " + incidenceTxt);
				treeNode.Nodes.Add(advNode);

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

		public static void getTreeNodeFromIncidencePoolingAndAggregation(IncidencePoolingAndAggregation incidencePoolingAndAggregation, ref TreeNode tn)
		{
			try
			{
				//Find all unique endpoint groups within a given pooling window
				List<string> uniqueEndpointGroups = incidencePoolingAndAggregation.lstAllSelectCRFuntion
						.GroupBy(x => x.EndPointGroup)
						.Select(g => g.First().EndPointGroup)
						.ToList();

				//Iterate through each endpoint group
				foreach (string endpointGroup in uniqueEndpointGroups)
				{
					//Find all the entries that match the endpoint group, ordering by the parent ID
					List<AllSelectCRFunction> endpointEntries = incidencePoolingAndAggregation.lstAllSelectCRFuntion
						.Where(x => x.EndPointGroup.Equals(endpointGroup))
						.OrderBy(x => x.PID)
						.ToList();

					//Add the nickname of the first entry (which is the parent/root note)
					TreeNode newNode = new TreeNode();
					newNode.Text = endpointEntries[0].Nickname;

					if (endpointEntries.Count() > (1))
					{
						//if there are more than one entries, recursively find children based on parent id--provide pooling/weight info as necessary
						int parentID = endpointEntries[0].ID;
						if (endpointEntries[0].PoolingMethod != "")
							newNode.Nodes.Add("Pooling Method: " + endpointEntries[0].PoolingMethod);
						if (endpointEntries[0].Weight != 0)
							newNode.Nodes.Add("Weight: " + endpointEntries[0].Weight);

						getChildNodeFromParentNode(parentID, endpointEntries, ref newNode);
					}

					tn.Nodes.Add(newNode);
				}
			}
			catch
			{
			}
		}

		public static void getChildNodeFromParentNode(int parentID, List<AllSelectCRFunction> endpointEntries, ref TreeNode childNodes)
		{
			List<AllSelectCRFunction> childEntries = endpointEntries
					.Where(x => x.PID.Equals(parentID))
					.ToList();

			foreach (AllSelectCRFunction entry in childEntries)
			{
				string entryString;
				if (string.IsNullOrEmpty(entry.Nickname))
					entryString = entry.Author;
				else
					entryString = entry.Nickname;

				TreeNode newNode = childNodes.Nodes.Add(entryString);
				if (entry.PoolingMethod != "")
					newNode.Nodes.Add("Pooling Method: " + entry.PoolingMethod);
				if (entry.Weight != 0)
					newNode.Nodes.Add("Weight: " + entry.Weight.ToString());

				//if we are at the last child provide the actual HIF record
				if (entry.NodeType == 100)
					newNode.Nodes.Add("Health Impact Function (" + entry.Author + ", " + entry.Year + ", " + entry.EndPointGroup + ")");
				getChildNodeFromParentNode(entry.ID, endpointEntries, ref newNode);
			}
		}
		public static TreeNode getTreeNodeFromIncidencePoolingAndAggregationAdvance(IncidencePoolingAndAggregationAdvance incidencePoolingAndAggregationAdvance)
		{
			TreeNode treeNode = new TreeNode();
			try
			{
				treeNode.Text = "Advanced Settings";
				TreeNode advAgPoolNode = new TreeNode();
				advAgPoolNode.Text = "Aggregation and Pooling";
				advAgPoolNode.Nodes.Add("Sort Incidence LHPs: " + incidencePoolingAndAggregationAdvance.SortIncidenceResults);
				advAgPoolNode.Nodes.Add("Default Advanced Pooling Method: " + Enum.GetName(typeof(IPAdvancePoolingMethodEnum), incidencePoolingAndAggregationAdvance.IPAdvancePoolingMethod));
				advAgPoolNode.Nodes.Add("Default Monte Carlo Iterations: " + String.Format("{0:n0}", incidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations));
				advAgPoolNode.Nodes.Add("Random Seed: " + incidencePoolingAndAggregationAdvance.RandomSeed);
				treeNode.Nodes.Add(advAgPoolNode);

				TreeNode advValNode = new TreeNode();
				advValNode.Text = "Valuation";
				TreeNode trInflation = new TreeNode();

				if (incidencePoolingAndAggregationAdvance.InflationDatasetName != "")
				{
					trInflation.Text = "Inflation Adjustment";
					trInflation.Nodes.Add("Dataset: " + incidencePoolingAndAggregationAdvance.InflationDatasetName);
					trInflation.Nodes.Add("Year: " + incidencePoolingAndAggregationAdvance.CurrencyYear);
				}
				else
					trInflation.Text = "No Inflation Adjustment";

				advValNode.Nodes.Add(trInflation);
				TreeNode trIncome = new TreeNode();
				if (incidencePoolingAndAggregationAdvance.AdjustIncomeGrowthDatasetName != "")
				{
					trIncome.Text = "Income Growth Adjustment";
					trIncome.Nodes.Add("Dataset: " + incidencePoolingAndAggregationAdvance.AdjustIncomeGrowthDatasetName);
					trIncome.Nodes.Add("Year: " + incidencePoolingAndAggregationAdvance.IncomeGrowthYear);
					if (incidencePoolingAndAggregationAdvance.EndpointGroups != null)
					{
						try
						{
							Dictionary<string, double> dicIncome = APVX.APVCommonClass.getIncomeGrowthFactorsFromDataSetIDAndYear(incidencePoolingAndAggregationAdvance.AdjustIncomeGrowthDatasetID,
								 incidencePoolingAndAggregationAdvance.IncomeGrowthYear);
							TreeNode tnEndpointGroups = new TreeNode();
							tnEndpointGroups.Text = "Adjust Income Growth Endpoint Groups";
							foreach (string s in incidencePoolingAndAggregationAdvance.EndpointGroups)
							{
								if (dicIncome.ContainsKey(s))
									tnEndpointGroups.Nodes.Add(s + ": " + dicIncome[s]);
								else
									tnEndpointGroups.Nodes.Add(s + ": 1");
							}
							trIncome.Nodes.Add(tnEndpointGroups);
						}
						catch
						{
						}
					}
				}
				else
					trIncome.Text = ("No Income Growth Adjustment");

				advValNode.Nodes.Add(trIncome);
				treeNode.Nodes.Add(advValNode);

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
				int valCount = 1;
				foreach (AllSelectValuationMethod asvm in lstOne)
				{

					if (asvm.NodeType == 2000)
					{
						TreeNode tnvaluation = new TreeNode();
						tnvaluation.Text = "Valuation Function #" + valCount + " (" + asvm.BenMAPValuationFunction.Qualifier + ")";
						//tnvaluation.Nodes.Add("ID: " + asvm.BenMAPValuationFunction.ID);
						tnvaluation.Nodes.Add("Dataset: " + asvm.BenMAPValuationFunction.DataSet);
						tnvaluation.Nodes.Add("Endpoint group: " + asvm.BenMAPValuationFunction.EndPointGroup);
						tnvaluation.Nodes.Add("Endpoint: " + asvm.BenMAPValuationFunction.EndPoint);

						if (asvm.BenMAPValuationFunction.StartAge == -1)
						{
							tnvaluation.Nodes.Add("Start Age: " + asvm.StartAge);
						}
						else
						{
							tnvaluation.Nodes.Add("Start Age: " + asvm.BenMAPValuationFunction.StartAge);
						}

						if (asvm.BenMAPValuationFunction.EndAge == -1)
						{
							tnvaluation.Nodes.Add("End age: ");
						}
						else
						{
							tnvaluation.Nodes.Add("End age: " + asvm.BenMAPValuationFunction.EndAge);
						}

						if (asvm.BenMAPValuationFunction.Reference != "")
							tnvaluation.Nodes.Add("Reference: " + asvm.BenMAPValuationFunction.Reference);

						tnvaluation.Nodes.Add("Function: " + asvm.BenMAPValuationFunction.Function);
						tnvaluation.Nodes.Add("Name A: " + asvm.BenMAPValuationFunction.NameA);
						tnvaluation.Nodes.Add("A: " + asvm.BenMAPValuationFunction.A);
						if (asvm.BenMAPValuationFunction.DistA != "None")
						{
							tnvaluation.Nodes.Add("Dist A: " + asvm.BenMAPValuationFunction.DistA);
							tnvaluation.Nodes.Add("P1A: " + asvm.BenMAPValuationFunction.P1A);
							tnvaluation.Nodes.Add("P2A: " + asvm.BenMAPValuationFunction.P2A);
						}
						if (asvm.BenMAPValuationFunction.NameB != "")
						{
							tnvaluation.Nodes.Add("Name B: " + asvm.BenMAPValuationFunction.NameB);
							tnvaluation.Nodes.Add("B: " + asvm.BenMAPValuationFunction.B);
						}
						if (asvm.BenMAPValuationFunction.NameC != "")
						{
							tnvaluation.Nodes.Add("Name C: " + asvm.BenMAPValuationFunction.NameC);
							tnvaluation.Nodes.Add("C: " + asvm.BenMAPValuationFunction.C);
						}
						if (asvm.BenMAPValuationFunction.NameD != "")
						{
							tnvaluation.Nodes.Add("Name D: " + asvm.BenMAPValuationFunction.NameD);
							tnvaluation.Nodes.Add("D: " + asvm.BenMAPValuationFunction.D);
						}
						if (asvm.Weight != 0)
							tnvaluation.Nodes.Add("Weight: " + asvm.Weight);
						treeNode.Nodes.Add(tnvaluation);
						valCount++;
					}
					else
					{
						TreeNode tn = new TreeNode();
						tn.Text = "Health Impact Function (" + asvm.Author + ")";
						getTreeNodeFromLstAllSelectValuationMethod(asvm, LstAllSelectValuationMethod, ref tn);
						treeNode.Nodes.Add(tn);
					}
				}
			}

		}

		public static void getTreeNodeFromLstAllSelectCRFunction(AllSelectCRFunction AllSelectCRFunctionList, List<AllSelectCRFunction> LstAllSelectCRFunction, ref TreeNode treeNode)
		{
			List<AllSelectCRFunction> lstOne = LstAllSelectCRFunction.Where(p => p.PID == AllSelectCRFunctionList.ID).ToList();
			TreeNode newNode = new TreeNode();

			if (lstOne != null && lstOne.Count > 0)
			{
				foreach (AllSelectCRFunction asvm in lstOne)
				{
					if (asvm.NodeType == 100)
					{
						//    TreeNode tnvaluation = new TreeNode();
						//    tnvaluation.Text = "Health Impact Function1 (" + asvm.Author + ", " + asvm.Year + ", " + asvm.EndPointGroup + ")";
						//    tnvaluation.Nodes.Add("Health Impact Function Dataset: " + asvm.DataSet);
						//    tnvaluation.Nodes.Add("Endpoint: " + asvm.EndPoint);
						//    tnvaluation.Nodes.Add("Pollutant: " + asvm.Pollutant);
						//    tnvaluation.Nodes.Add("Metric: " + asvm.Metric); tnvaluation.Nodes.Add("Metric statistic: " + asvm.MetricStatistic);
						//    tnvaluation.Nodes.Add("Location: " + asvm.Location);
						//    tnvaluation.Nodes.Add("Other pollutants: " + asvm.OtherPollutants);
						//    tnvaluation.Nodes.Add("Geographic area: " + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.GeographicAreaName);
						//    try
						//    {
						//        tnvaluation.Nodes.Add("Reference: " + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Reference);
						//        if (asvm.StartAge == "-1")
						//            tnvaluation.Nodes.Add("Start Age: ");
						//        else
						//            tnvaluation.Nodes.Add("Start Age: " + asvm.StartAge);
						//        if (asvm.EndAge == "-1")
						//            tnvaluation.Nodes.Add("End age: ");
						//        else
						//            tnvaluation.Nodes.Add("End age: " + asvm.EndAge);
						//        tnvaluation.Nodes.Add("Baseline functional form: " + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction); tnvaluation.Nodes.Add("Incidence dataset: " + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.IncidenceDataSetName);
						//        tnvaluation.Nodes.Add("Beta: " + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Beta);
						//        tnvaluation.Nodes.Add("Beta distribution: " + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaDistribution);
						//        tnvaluation.Nodes.Add("P1Beta: " + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter1);
						//        tnvaluation.Nodes.Add("P2Beta: " + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
						//        tnvaluation.Nodes.Add("A: " + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantValue);

						//        tnvaluation.Nodes.Add("Name A: " + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantDescription);
						//        tnvaluation.Nodes.Add("B: " + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantValue);
						//        tnvaluation.Nodes.Add("Name B: " + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantDescription);
						//        tnvaluation.Nodes.Add("C: " + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantValue);
						//        tnvaluation.Nodes.Add("Name C: " + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantDescription);
						//        tnvaluation.Nodes.Add("Percentile: " + asvm.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Percentile);
						//}
						//catch (Exception ex)
						//{ }
						//tnvaluation.Nodes.Add("Weight: " + asvm.Weight);
						//    treeNode.Nodes.Add(tnvaluation);
					}
					else
					{
						//TreeNode tn = new TreeNode();
						//tn.Text = "Pooling Method Type: " + asvm.PoolingMethod;
						//getTreeNodeFromLstAllSelectCRFunction(asvm, LstAllSelectCRFunction, ref tn);
						//treeNode.Nodes.Add(tn);
					}

				}
				//treeNode.Text = "Pooling Method Type: " + AllSelectCRFunctionList.PoolingMethod;
			}
			else
			{
				if (AllSelectCRFunctionList.NodeType == 100)
				{
					newNode.Text = "Health Impact Function (" + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author + " ," + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Year + " ," + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroup + ")";

					if (AllSelectCRFunctionList.Weight != 0)
						treeNode.Nodes.Add("Weight: " + AllSelectCRFunctionList.Weight);
					//try
					//{
					//    treeNode.Nodes.Add("Health Impact Function Dataset: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName);
					//    treeNode.Nodes.Add("Endpoint: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint);
					//    treeNode.Nodes.Add("Pollutant: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName);
					//    treeNode.Nodes.Add("Metric: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName);
					//    treeNode.Nodes.Add("Metric statistic: " + Enum.GetName(typeof(MetricStatic), AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic));
					//    if (AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
					//        treeNode.Nodes.Add("Seasonal metric: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName);

					//    treeNode.Nodes.Add("Qualifier: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier);
					//    treeNode.Nodes.Add("Function: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Function);
					//    treeNode.Nodes.Add("Geographic area: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.GeographicAreaName);
					//    treeNode.Nodes.Add("Other pollutants: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants);

					//    treeNode.Nodes.Add("Reference: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Reference);
					//    if (AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.StartAge == -1)
					//        treeNode.Nodes.Add("Start Age: ");
					//    else
					//        treeNode.Nodes.Add("Start Age: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.StartAge);
					//    if (AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndAge == -1)
					//        treeNode.Nodes.Add("End age: ");
					//    else
					//        treeNode.Nodes.Add("End age: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndAge);
					//    treeNode.Nodes.Add("Baseline functional form : " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction);
					//    treeNode.Nodes.Add("Incidence dataset: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.IncidenceDataSetName);
					//    treeNode.Nodes.Add("Beta: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Beta);
					//    treeNode.Nodes.Add("Beta distribution: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaDistribution);
					//    treeNode.Nodes.Add("P1Beta: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter1);
					//    treeNode.Nodes.Add("P2Beta: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
					//    treeNode.Nodes.Add("A: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantValue);

					//    treeNode.Nodes.Add("Name A: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.AContantDescription);
					//    treeNode.Nodes.Add("B: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantValue);
					//    treeNode.Nodes.Add("Name B: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.BContantDescription);
					//    treeNode.Nodes.Add("C: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantValue);
					//    treeNode.Nodes.Add("Name C: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.CContantDescription);
					//    treeNode.Nodes.Add("Percentile: " + AllSelectCRFunctionList.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Percentile);
					//}
					//catch
					//{ }

				}
				else
				{
					newNode.Text = AllSelectCRFunctionList.Name + ":Pooling Method Type: " + AllSelectCRFunctionList.PoolingMethod + " Weight: " + AllSelectCRFunctionList.Weight;
				}
			}
			treeNode.Nodes.Add(newNode);
		}

		public static void getTreeNodeFromAllDatasetsMetadata(List<MetadataClassObj> lstMetadataObjs, ref TreeNode treeNode)
		{



		}

		public static TreeNode getTreeNodeFromValuationMethodPoolingAndAggregationBase(ValuationMethodPoolingAndAggregationBase valuationMethodPoolingAndAggregationBase)
		{
			TreeNode treeNode = new TreeNode();
			try
			{

				//Build a tree node by adding relevant info about pooling and valuation
				TreeNode tn = new TreeNode();
				tn.Text = "Pooling";
				getTreeNodeFromIncidencePoolingAndAggregation(valuationMethodPoolingAndAggregationBase.IncidencePoolingAndAggregation, ref tn);
				treeNode.Nodes.Add(tn);

				if (valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue != null && valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod.Count > 1)
				{
					tn = new TreeNode();
					tn.Text = "Valuation Method Type: " + valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod[0].PoolingMethod;
					getTreeNodeFromLstAllSelectValuationMethod(valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod[0], valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod, ref tn);
					treeNode.Nodes.Add(tn);
				}
				else
				{
					tn = new TreeNode();
					tn.Text = "No Valuation";
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
				treeNode.Nodes.Add("Create Datetime: " + valuationMethodPoolingAndAggregation.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));

				TreeNode agNode = new TreeNode();
				agNode.Text = "Aggregation";
				if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation != null)
				{
					TreeNode tn = new TreeNode();
					tn.Text = "Incidence";
					tn.Nodes.Add("Name: " + valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionName);
					//tn.Nodes.Add("ID: " + valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID);
					tn.Nodes.Add("Columns: " + valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.Columns);
					tn.Nodes.Add("Rows: " + valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.RRows);
					tn.Nodes.Add("Grid Type: " + Enum.GetName(typeof(GridTypeEnum), valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.TType));
					tn.Nodes.Add("Shapefile Name: " + ((valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation is ShapefileGrid) ? (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation as ShapefileGrid).ShapefileName : (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation as RegularGrid).ShapefileName));
					agNode.Nodes.Add(tn);
				}
				if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null)
				{
					TreeNode tn = new TreeNode();
					tn.Text = "Valuation";
					tn.Nodes.Add("Name: " + valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionName);
					//tn.Nodes.Add("ID: " + valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID);
					tn.Nodes.Add("Columns: " + valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.Columns);
					tn.Nodes.Add("Rows: " + valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.RRows);
					tn.Nodes.Add("Grid Type: " + Enum.GetName(typeof(GridTypeEnum), valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.TType));
					tn.Nodes.Add("Shapefile Name: " + ((valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation is ShapefileGrid) ? (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation as ShapefileGrid).ShapefileName : (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation as RegularGrid).ShapefileName));
					agNode.Nodes.Add(tn);
				}
				if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation != null)
				{
					TreeNode tn = new TreeNode();
					tn.Text = "QALYAggregation Aggregation";
					tn.Nodes.Add("Name: " + valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionName);
					//tn.Nodes.Add("ID: " + valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID);
					tn.Nodes.Add("Columns: " + valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation.Columns);
					tn.Nodes.Add("Rows: " + valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation.RRows);
					tn.Nodes.Add("Grid Type: " + Enum.GetName(typeof(GridTypeEnum), valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation.TType));
					tn.Nodes.Add("Shapefile Name: " + ((valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation is ShapefileGrid) ? (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation as ShapefileGrid).ShapefileName : (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation as RegularGrid).ShapefileName));
					agNode.Nodes.Add(tn);
				}
				treeNode.Nodes.Add(agNode);
				int poolCount = 1;
				foreach (ValuationMethodPoolingAndAggregationBase vb in valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
				{
					TreeNode poolNode = new TreeNode();
					poolNode = getTreeNodeFromValuationMethodPoolingAndAggregationBase(vb);
					poolNode.Text = "Pooling Window #" + poolCount + " (" + vb.IncidencePoolingAndAggregation.PoolingName + ")";
					treeNode.Nodes.Add(poolNode);
					poolCount++;
				}
				if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null)
				{
					treeNode.Nodes.Add(getTreeNodeFromIncidencePoolingAndAggregationAdvance(valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance));
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

		public static TreeNode getRuntimeTreeNode(List<string> lstLog)
		{
			TreeNode treeNode = new TreeNode();
			try
			{
				foreach (string entry in lstLog)
				{
					if (entry.Contains("Processing complete."))
					{
						int idxEntry = entry.IndexOf('.') + 1;
						string subEntry = entry.Substring(idxEntry).Trim();
						treeNode.Text = subEntry;
					}
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
				txtWithoutSpace = txtWithoutSpace.Replace(": ", "");
				txtWithoutSpace = txtWithoutSpace.Replace("..", ".");
				sw.WriteLine("<" + txtWithoutSpace + ">");

				foreach (TreeNode node in tv.Nodes)
				{
					string nodeText = node.Text.Replace(" ", ".");
					nodeText = nodeText.Replace(",", ".");
					nodeText = nodeText.Replace("&", "And");
					nodeText = nodeText.Replace(":", "");
					nodeText = nodeText.Replace("..", ".");
					nodeText = nodeText.Replace("#", "");

					if (node.Nodes.Count > 1) //updated to address BenMAP 258/246--printing the text of first-level parent node (11/26/2019,MP) 
					{
						sw.WriteLine("<" + nodeText + ">");
						saveNode(node.Nodes);
						sw.WriteLine("</" + nodeText + ">");
					}
					else
						sw.WriteLine(nodeText);
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
					txtWithoutSpace = txtWithoutSpace.Replace(": ", "");
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
