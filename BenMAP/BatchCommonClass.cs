using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace BenMAP
{
	class BatchCommonClass
	{
		public static string getMonitorDataSetNameFromID(int MonitorDataSetID)
		{
			try
			{
				string strSQL = "select MonitorDatasetName from MonitorDataSets where MonitorDatasetID=" + MonitorDataSetID;
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				return fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, strSQL).ToString();
			}
			catch
			{
			}
			return "";
		}
		public static bool OutputAQG(BenMAPLine benMAPLine, string strFile, string strFileAQG)
		{
			try
			{
				FileStream fs = new FileStream(strFile, FileMode.Create);
				StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
				sw.WriteLine("## " + Path.GetFileName(strFile));
				sw.WriteLine("## " + DateTime.Now.ToString("d"));
				sw.WriteLine();
				sw.WriteLine("VARIABLES");
				sw.WriteLine();
				sw.WriteLine("%BENMAPDIR%     " + System.Windows.Forms.Application.StartupPath);
				sw.WriteLine("%AQGDIR%        " + CommonClass.ResultFilePath + @"\Result\AQG");
				sw.WriteLine();
				sw.WriteLine("COMMANDS");
				sw.WriteLine();
				sw.WriteLine("SETACTIVESETUP  -ActiveSetup    " + CommonClass.getBenMAPSetupFromID(benMAPLine.GridType.SetupID).SetupName);
				sw.WriteLine();
				if (benMAPLine is ModelDataLine)
				{
					sw.WriteLine("##");
					sw.WriteLine("##Source Apportionment modeling");
					sw.WriteLine("##");
					sw.WriteLine();
					sw.WriteLine("CREATE AQG");
					sw.WriteLine();
					sw.WriteLine("-Filename   " + strFileAQG.Replace(CommonClass.ResultFilePath + @"\Result\AQG\", @"%AQGDIR%\"));
					sw.WriteLine("-GridType   " + "\"" + benMAPLine.GridType.GridDefinitionName + "\"");
					sw.WriteLine("-Pollutant  " + benMAPLine.Pollutant.PollutantName);
					sw.WriteLine();
					sw.WriteLine("ModelDirect");
					sw.WriteLine();
					sw.WriteLine("-ModelFilename  " + (benMAPLine as ModelDataLine).DatabaseFilePath);
					sw.WriteLine("-DSNName        ");
					sw.WriteLine();
					sw.WriteLine("##");
					sw.WriteLine();
				}
				else if (benMAPLine is MonitorDataLine)
				{
					sw.WriteLine("##");
					sw.WriteLine("##Source Apportionment monitor");
					sw.WriteLine("##");
					sw.WriteLine();
					sw.WriteLine("CREATE AQG");
					sw.WriteLine();
					sw.WriteLine("-Filename   " + strFileAQG.Replace(CommonClass.ResultFilePath + @"\Result\AQG\", @"%AQGDIR%\"));
					sw.WriteLine("-GridType   " + "\"" + benMAPLine.GridType.GridDefinitionName + "\"");
					sw.WriteLine("-Pollutant  " + benMAPLine.Pollutant.PollutantName);
					sw.WriteLine();
					sw.WriteLine("MonitorDirect");
					sw.WriteLine();
					MonitorDataLine monitordataline = benMAPLine as MonitorDataLine;
					if (monitordataline.MonitorDirectType == 0)
						sw.WriteLine("-MonitorDataType      Library");
					else
						sw.WriteLine("-MonitorDataType      TextFile");
					if (monitordataline.InterpolationMethod != null)
						sw.WriteLine("-InterpolationMethod  " + Enum.GetName(typeof(InterpolationMethodEnum), monitordataline.InterpolationMethod));
					sw.WriteLine("-MonitorDataSet       " + getMonitorDataSetNameFromID(monitordataline.MonitorDataSetID));
					sw.WriteLine("-MonitorYear          " + monitordataline.MonitorLibraryYear);
					sw.WriteLine("-MonitorFile          " + monitordataline.MonitorDataFilePath);
					if (monitordataline.MonitorAdvance == null)
					{
						sw.WriteLine("-MaxDistance          -1");
						sw.WriteLine("-MaxRelativeDistance  -1");
						sw.WriteLine("-WeightingMethod      ");
					}
					else
					{
						sw.WriteLine("-MaxDistance          " + monitordataline.MonitorAdvance.MaxinumNeighborDistance.ToString());
						sw.WriteLine("-MaxRelativeDistance  " + monitordataline.MonitorAdvance.RelativeNeighborDistance.ToString());
						sw.WriteLine("-WeightingMethod      " + Enum.GetName(typeof(WeightingApproachEnum), monitordataline.MonitorAdvance.WeightingApproach));
					}
					if (monitordataline.InterpolationMethod == InterpolationMethodEnum.FixedRadius)
					{
						sw.Write("-FixRadius            " + monitordataline.FixedRadius);
					}
					sw.WriteLine();
					sw.WriteLine("##");
					sw.WriteLine();
				}
				sw.Flush();
				sw.Close();
				fs.Close();
			}
			catch
			{
				return false;
			}
			return true;
		}
		public static bool OutputCFG(BaseControlCRSelectFunction baseControlCRSelectFunction, string strFile, string strFileCFG)
		{
			try
			{
				if (strFileCFG == "") strFileCFG = strFile.Substring(0, strFile.Count() - 4) + "cfgx";
				FileStream fs = new FileStream(strFile, FileMode.Create);
				StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
				sw.WriteLine("## " + Path.GetFileName(strFile));
				sw.WriteLine("## " + DateTime.Now.ToString("d"));
				sw.WriteLine();
				sw.WriteLine("VARIABLES");
				sw.WriteLine();
				sw.WriteLine("%BENMAPDIR%     " + System.Windows.Forms.Application.StartupPath);
				sw.WriteLine("%CFGDIR%        " + CommonClass.ResultFilePath + @"\Result\CFG");
				sw.WriteLine("%CFGRDIR%       " + CommonClass.ResultFilePath + @"\Result\CFGR");
				sw.WriteLine();
				sw.WriteLine("COMMANDS");
				sw.WriteLine();
				sw.WriteLine("SETACTIVESETUP  -ActiveSetup    " + CommonClass.getBenMAPSetupFromID(baseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID).SetupName);
				sw.WriteLine();
				sw.WriteLine("##");
				sw.WriteLine("##");
				sw.WriteLine();
				sw.WriteLine("RUN CFG");
				sw.WriteLine();
				sw.WriteLine("-CFGFilename           " + strFileCFG.Replace(CommonClass.ResultFilePath + @"\Result\CFG\", @"%CFGDIR%\")
						.Replace(CommonClass.ResultFilePath + @"\Result\CFGR\", @"%CFGRDIR%\"));
				sw.WriteLine("-ResultsFilename       " + strFileCFG.Substring(0, strFileCFG.Count() - 4).Replace(CommonClass.ResultFilePath + @"\Result\CFG\", @"%CFGDIR%\")
						.Replace(CommonClass.ResultFilePath + @"\Result\CFGR\", @"%CFGRDIR%\") + "cfgrx");
				sw.WriteLine("-BaselineAQG           ");
				sw.WriteLine("-ControlAQG            ");
				sw.WriteLine("-Year                  " + baseControlCRSelectFunction.BenMAPPopulation.Year.ToString());
				sw.WriteLine("-LatinHypercubePoints  " + baseControlCRSelectFunction.CRLatinHypercubePoints);
				sw.WriteLine("-DefaultMonteCarlo     " + baseControlCRSelectFunction.CRDefaultMonteCarloIterations);
				sw.WriteLine("-Seeds                 " + baseControlCRSelectFunction.CRSeeds);
				sw.WriteLine("-Threshold             " + baseControlCRSelectFunction.CRThreshold);
				sw.Flush();
				sw.Close();
				fs.Close();
			}
			catch
			{
				return false;
			}
			return true;
		}
		public static bool OutputAPV(ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation, string strFile, string strAPVFile)
		{
			try
			{
				if (strAPVFile == "") strAPVFile = strFile.Substring(0, strFile.Count() - 4) + "apvx";
				FileStream fs = new FileStream(strFile, FileMode.Create);
				StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
				sw.WriteLine("## " + Path.GetFileName(strFile));
				sw.WriteLine("## " + DateTime.Now.ToString("d"));
				sw.WriteLine();
				sw.WriteLine("VARIABLES");
				sw.WriteLine();
				sw.WriteLine("%BENMAPDIR%     " + System.Windows.Forms.Application.StartupPath);
				sw.WriteLine("%APVDIR%        " + CommonClass.ResultFilePath + @"\Result\APV");
				sw.WriteLine("%APVRDIR%       " + CommonClass.ResultFilePath + @"\Result\APVR");
				sw.WriteLine();
				sw.WriteLine("COMMANDS");
				sw.WriteLine();
				sw.WriteLine("SETACTIVESETUP  -ActiveSetup    " + CommonClass.getBenMAPSetupFromID(valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.First().GridType.SetupID).SetupName);
				sw.WriteLine();
				sw.WriteLine("##");
				sw.WriteLine("##");
				sw.WriteLine();
				sw.WriteLine("RUN APV");
				sw.WriteLine();
				sw.WriteLine("-APVFilename           " + strAPVFile.Replace(CommonClass.ResultFilePath + @"\Result\APV\", @"%APVDIR%\")
						.Replace(CommonClass.ResultFilePath + @"\Result\APVR\", @"%APVRDIR%\"));
				sw.WriteLine("-ResultsFilename       " + strAPVFile.Replace("apvx", "apvrx").Replace(CommonClass.ResultFilePath + @"\Result\APV\", @"%APVDIR%\")
						.Replace(CommonClass.ResultFilePath + @"\Result\APVR\", @"%APVRDIR%\"));
				sw.WriteLine("-CFGRFilename          " + strAPVFile.Substring(0, strAPVFile.Length - 4).Replace(CommonClass.ResultFilePath + @"\Result\APV\", @"%APVRDIR%\")
						.Replace(CommonClass.ResultFilePath + @"\Result\APVR\", @"%APVRDIR%\") + "cfgrx");
				if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
						valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation != null)
				{
					sw.WriteLine("-IncidenceAggregation  " + valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionName);
				}
				else
				{
					sw.WriteLine("-IncidenceAggregation  ");
				}
				if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
						valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null)
				{
					sw.WriteLine("-ValuationAggregation  " + valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionName);
				}
				else
				{
					sw.WriteLine("-ValuationAggregation  ");
				}
				if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null)
				{
					sw.WriteLine("-RandomSeed            " + valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.RandomSeed);
					sw.WriteLine("-DollarYear            " + valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.CurrencyYear);
				}

				sw.Flush();
				sw.Close();
				fs.Close();
			}
			catch
			{
				return false;
			}
			return true;
		}
		public static void WriteBatchLogFile(string msg, string strFile)
		{
			try
			{
				if (!File.Exists(strFile))
				{
					FileStream fsCreate = File.Create(strFile);
					fsCreate.Close();
				}
				FileStream fs = new FileStream(strFile, FileMode.Append);
				StreamWriter streamWriter = new StreamWriter(fs, Encoding.UTF8);
				streamWriter.BaseStream.Seek(0, SeekOrigin.End);
				streamWriter.WriteLine(msg);
				streamWriter.Flush();
				fs.Close();
			}
			catch
			{
			}
		}
		public static bool RunBatch(string strFile)
		{
			DateTime dateTime = DateTime.Now;

			try
			{   //Count # of Batch Configs and Report One
				Console.Write("Reading Batch File...");
				bool checkLog = false;

				List<BatchBase> lstBatchBase = ReadBatchFile(strFile, ref checkLog);
				if (lstBatchBase == null)
				{
					Console.Write("Error (No Commands)");
					return false;
				}
				if (lstBatchBase.Count > 1)
					Console.WriteLine("Completed (Loaded " + lstBatchBase.Count + " Commands)");
				else
					Console.WriteLine("Completed (Loaded " + lstBatchBase.Count + " Command)");

				if (checkLog)
				{
					Console.WriteLine("Error Executing Batch File Command(s)" + Environment.NewLine + "Log Available at " + strFile + ".log");
					Console.WriteLine("You may also find additional information in " + CommonClass.ResultFilePath + "\\BenMAP.log");
					return false;
				}
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

				int batchCount = 0;
				foreach (BatchBase batchBase in lstBatchBase)
				{
					batchCount += 1;
					//Console.SetCursorPosition(0, Console.CursorTop);
					//Console.Write(new String(' ', Console.BufferWidth));
					//Console.SetCursorPosition(0, Console.CursorTop - 1);
					Console.WriteLine("---Running Command #" + batchCount + "---");
					//CommonClass.ClearAllObject(); //Clear all object so that each batch action runs independently.
					if (batchBase is BatchAQGBase)
					{
						try
						{
							if (batchBase is BatchModelDirect)
							{
								Console.Write("Creating Air Quality Grid from Model Data...");
								try
								{
									BatchModelDirect batchModelDirect = batchBase as BatchModelDirect;
									ModelDataLine modelDataLine = new ModelDataLine();
									modelDataLine.DatabaseFilePath = batchModelDirect.ModelFilename;
									System.Data.DataTable dtModel = CommonClass.ExcelToDataTable(batchModelDirect.ModelFilename, batchModelDirect.ModelTablename);
									CommonClass.MainSetup = getSetupFromName(batchBase.ActiveSetup);
									BenMAPPollutant benMAPPollutant = getPollutantFromName(batchModelDirect.Pollutant);
									BenMAPGrid benMAPGrid = getGridFromName(batchModelDirect.GridType);
									modelDataLine.Pollutant = benMAPPollutant;
									modelDataLine.GridType = benMAPGrid;
									DataSourceCommonClass.UpdateModelDataLineFromDataSet(benMAPPollutant, modelDataLine, dtModel);
									Dictionary<int, string> dicSeasonStatics = new Dictionary<int, string>();


									DataSourceCommonClass.UpdateModelValuesModelData(DataSourceCommonClass.DicSeasonStaticsAll, benMAPGrid, benMAPPollutant, modelDataLine, ""); modelDataLine.GridType = benMAPGrid;
									DataSourceCommonClass.CreateAQGFromBenMAPLine(modelDataLine, batchModelDirect.Filename);
									Console.WriteLine("Completed");
								}
								catch (Exception ex)
								{
									Console.WriteLine("AQG Error: " + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + "Log Available at " + strFile + ".log");
									Logger.LogError(ex);
								}
							}
							else if (batchBase is BatchMonitorDirect)
							{
								Console.Write("Creating Air Quality Grid from Monitor Data...");
								try
								{
									BatchMonitorDirect batchMonitorDirect = batchBase as BatchMonitorDirect;
									MonitorDataLine monitorDataLine = new MonitorDataLine();
									CommonClass.MainSetup = getSetupFromName(batchBase.ActiveSetup);
									BenMAPPollutant benMAPPollutant = getPollutantFromName(batchMonitorDirect.Pollutant);
									BenMAPGrid benMAPGrid = getGridFromName(batchMonitorDirect.GridType);
									monitorDataLine.Pollutant = benMAPPollutant;
									monitorDataLine.GridType = benMAPGrid;
									if (batchMonitorDirect.MonitorDataType == "Library")
									{
										monitorDataLine.MonitorDirectType = 0;
										monitorDataLine.MonitorLibraryYear = batchMonitorDirect.MonitorYear;
										string commandText = string.Format("select MonitorDataSetID from MonitorDataSets where SetupID={0} and MonitorDataSetName='{1}'", CommonClass.MainSetup.SetupID, batchMonitorDirect.MonitorDataSet);
										if (fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText) == null)
										{
											WriteBatchLogFile("Error (Unable to Locate Monitor Dataset) :", strFile + ".log");
											WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
											for (int j = 0; j < batchBase.BatchText.Count; j++)
											{
												WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
											}
											Console.WriteLine("Error (Unable to Locate Monitor Dataset)" + Environment.NewLine + "Log Available at " + strFile + ".log");
											continue;
										}
										monitorDataLine.MonitorDataSetID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
									}
									else if (batchMonitorDirect.MonitorDataType == "TextFile")
									{
										monitorDataLine.MonitorDirectType = 1;
										monitorDataLine.MonitorDataFilePath = batchMonitorDirect.MonitorFile;
									}
									monitorDataLine.InterpolationMethod = batchMonitorDirect.InterpolationMethod == "ClosestMonitor" ? InterpolationMethodEnum.ClosestMonitor : InterpolationMethodEnum.VoronoiNeighborhoodAveragin; if (batchMonitorDirect.InterpolationMethod == "FixedRadius")
									{
										monitorDataLine.InterpolationMethod = InterpolationMethodEnum.FixedRadius;
										monitorDataLine.FixedRadius = batchMonitorDirect.FixRadius;
									}
									monitorDataLine.MonitorAdvance = new MonitorAdvance()
									{
										MaxinumNeighborDistance = batchMonitorDirect.MaxDistance,
										RelativeNeighborDistance = batchMonitorDirect.MaxRelativeDistance,
									};

									if (CommonClass.MainSetup.SetupID == 1)
									{
										switch (monitorDataLine.Pollutant.PollutantName)
										{
											case "PM2.5":
												monitorDataLine.MonitorAdvance.FilterMaximumPOC = 4;
												monitorDataLine.MonitorAdvance.POCPreferenceOrder = "1,2,3,4";
												break;
											case "PM10":
												monitorDataLine.MonitorAdvance.FilterMaximumPOC = 4;
												monitorDataLine.MonitorAdvance.POCPreferenceOrder = "1,2,3,4";
												break;
											case "Ozone":
												monitorDataLine.MonitorAdvance.FilterMaximumPOC = 4;
												monitorDataLine.MonitorAdvance.POCPreferenceOrder = "1,2,3,4";
												break;
											case "NO2":
												monitorDataLine.MonitorAdvance.FilterMaximumPOC = 9;
												monitorDataLine.MonitorAdvance.POCPreferenceOrder = "1,2,3,4,5,6,7,8,9";
												break;
											case "SO2":
												monitorDataLine.MonitorAdvance.FilterMaximumPOC = 9;
												monitorDataLine.MonitorAdvance.POCPreferenceOrder = "1,2,3,4,5,6,7,8,9";
												break;
										}
									}
									if (batchMonitorDirect.WeightingMethod == "InverseDistanceSquared")
									{
										monitorDataLine.MonitorAdvance.WeightingApproach = WeightingApproachEnum.InverseDistanceSquared;
									}
									else
									{
										monitorDataLine.MonitorAdvance.WeightingApproach = WeightingApproachEnum.InverseDistance;
									}
									DataSourceCommonClass.UpdateModelValuesMonitorData(benMAPGrid, benMAPPollutant, ref monitorDataLine);
									monitorDataLine.GridType = benMAPGrid;
									DataSourceCommonClass.CreateAQGFromBenMAPLine(monitorDataLine, batchMonitorDirect.Filename);
									Console.Write("Completed");
									Console.WriteLine("Results Located At:");
									Console.WriteLine(batchMonitorDirect.Filename);
								}
								catch (Exception ex)
								{
									Console.WriteLine("AQG Error: " + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + "Log Available at " + strFile + ".log");
									Logger.LogError(ex);
								}

							}
						}
						catch (Exception ex)
						{
							WriteBatchLogFile("AQG Error :" + ex.Message, strFile + ".log");
							WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
							for (int j = 1; j < batchBase.BatchText.Count; j++)
							{
								WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
							}
							Console.WriteLine("AQG Error: " + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + "Log Available at " + strFile + ".log");
						}
					}
					else if (batchBase is BatchCFG)
					{
						try
						{
							Console.Write("Loading CFG File...");

							BatchCFG batchCFG = batchBase as BatchCFG;
							CommonClass.MainSetup = getSetupFromName(batchBase.ActiveSetup);
							string err = "";
							BaseControlCRSelectFunction baseControlCRSelectFunction = Configuration.ConfigurationCommonClass.loadCFGFile(batchCFG.CFGFilename, ref err);
							if (baseControlCRSelectFunction == null)
							{
								WriteBatchLogFile("Error (CFGX File-No Data) :" + err, strFile + ".log");
								WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
								for (int j = 1; j < batchBase.BatchText.Count; j++)
								{
									WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
								}
								Console.WriteLine("Error (CFGX File-No Data): " + err + Environment.NewLine + "Log Available at " + strFile + ".log" + Environment.NewLine);
								continue;
							}
							BenMAPLine benMAPLineBase = null, benMAPLineControl = null;
							string errB = "";
							string errC = "";
							if (batchCFG.BaselineAQG != null && batchCFG.BaselineAQG.Trim() != "")
							{
								benMAPLineBase = DataSourceCommonClass.LoadAQGFile(batchCFG.BaselineAQG, ref errB);
								if(errB == "")
                                {
									if (benMAPLineBase != null && benMAPLineBase.Pollutant.PollutantID != baseControlCRSelectFunction.BaseControlGroup.First().Pollutant.PollutantID)
									{
										//Benmap-562
										errB = "AQ pollutant (" + benMAPLineBase.Pollutant.PollutantName + ") does not match Function pollutant (" + baseControlCRSelectFunction.BaseControlGroup.First().Pollutant.PollutantName + ")";
									}
								}
								if (errB != "")
								{									
									WriteBatchLogFile("Error (Base File): " + errB, strFile + ".log");
									WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
									for (int j = 1; j < batchBase.BatchText.Count; j++)
									{
										WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
									}
									Console.WriteLine("Error (Base File): " + errB + Environment.NewLine + "Log Available at " + strFile + ".log" + Environment.NewLine);
									continue;
								}

							}
							if (batchCFG.ControlAQG != null && batchCFG.ControlAQG.Trim() != "")
							{
								benMAPLineControl = DataSourceCommonClass.LoadAQGFile(batchCFG.ControlAQG, ref errC);
                                if (errC == "")
                                {
									if (benMAPLineControl != null && benMAPLineControl.Pollutant.PollutantID != baseControlCRSelectFunction.BaseControlGroup.First().Pollutant.PollutantID)
									{
										errC = "AQ pollutant (" + benMAPLineControl.Pollutant.PollutantName + ") does not match Function pollutant (" + baseControlCRSelectFunction.BaseControlGroup.First().Pollutant.PollutantName + ")";										
									}
								}
                                if (errC != "")
                                {
									WriteBatchLogFile("Error (Control File): " + errC, strFile + ".log");
									WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
									for (int j = 1; j < batchBase.BatchText.Count; j++)
									{
										WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
									}
									Console.WriteLine("Error (Control File): " + errC + Environment.NewLine + "Log Available at " + strFile + ".log" + Environment.NewLine);
									continue;
								}
							}
							
							
							if (benMAPLineBase != null && benMAPLineControl != null && benMAPLineBase.GridType.GridDefinitionID != benMAPLineControl.GridType.GridDefinitionID)
							{
								WriteBatchLogFile("Error (Base or Control File): Baseline Grid Definition (" + benMAPLineBase.GridType.GridDefinitionName + ") does not match Control Grid Definition (" + benMAPLineControl.GridType.GridDefinitionName + ").", strFile + ".log");
								WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
								for (int j = 1; j < batchBase.BatchText.Count; j++)
								{
									WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
								}
								Console.WriteLine("Error (Base or Control File): Baseline Grid Definition (" + benMAPLineBase.GridType.GridDefinitionName + ") does not match Control Grid Definition (" + benMAPLineControl.GridType.GridDefinitionName + ")." + Environment.NewLine + "Log Available at " + strFile + ".log" + Environment.NewLine);
								continue;
							}
							if (benMAPLineBase != null && benMAPLineControl != null)
							{
								baseControlCRSelectFunction.BaseControlGroup.First().Base = benMAPLineBase;
								baseControlCRSelectFunction.BaseControlGroup.First().Control = benMAPLineControl;
								baseControlCRSelectFunction.BaseControlGroup.First().DeltaQ = null;
								baseControlCRSelectFunction.BaseControlGroup.First().GridType = benMAPLineBase.GridType;
							}
							if (batchCFG.Year != -1) baseControlCRSelectFunction.BenMAPPopulation.Year = batchCFG.Year;
							if (batchCFG.LatinHypercubePoints != -1 && (batchCFG.LatinHypercubePoints == 10 || batchCFG.LatinHypercubePoints == 20 || batchCFG.LatinHypercubePoints == 50 || batchCFG.LatinHypercubePoints == 100)) //Added option of 50 to match percentiles options listed in GUI ("LatinHypercubePoints.cs")--MP 07 Jan 2020
							{
								baseControlCRSelectFunction.CRLatinHypercubePoints = batchCFG.LatinHypercubePoints;

							}
							else if (batchCFG.LatinHypercubePoints == 0)
							{
								baseControlCRSelectFunction.CRRunInPointMode = true;
							}
							else if (batchCFG.LatinHypercubePoints != -1)
							{
								baseControlCRSelectFunction.CRRunInPointMode = false;

								WriteBatchLogFile("Error (Latin Hypercube Points Selection):", strFile + ".log");
								WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
								for (int j = 1; j < batchBase.BatchText.Count; j++)
								{
									WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
								}
								Console.WriteLine("Error (Latin Hypercube Points Selection)" + Environment.NewLine + "Log Available at " + strFile + ".log" + Environment.NewLine);
							}
							if (batchCFG.Threshold != -1)
								baseControlCRSelectFunction.CRThreshold = batchCFG.Threshold;
							if (batchCFG.Seeds != -1)
								baseControlCRSelectFunction.CRSeeds = batchCFG.Seeds;
							CommonClass.GBenMAPGrid = baseControlCRSelectFunction.BaseControlGroup.First().Base.GridType;
							CommonClass.LstBaseControlGroup = baseControlCRSelectFunction.BaseControlGroup;
							CommonClass.BaseControlCRSelectFunction = baseControlCRSelectFunction;
							HealthImpactFunctions healthImapctFuntion = new HealthImpactFunctions();
							healthImapctFuntion._filePath = batchCFG.ResultsFilename;

							Console.WriteLine("Completed");

							healthImapctFuntion.btnRun_Click(null, null);
						}
						catch (Exception ex)
						{
							WriteBatchLogFile("CFG Error :" + ex.Message, strFile + ".log");
							WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
							for (int j = 1; j < batchBase.BatchText.Count; j++)
							{
								WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
							}
							Console.WriteLine("CFG Error:" + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + "Log Available at " + strFile + ".log" + Environment.NewLine);
						}
					}
					else if (batchBase is BatchAPV)
					{
						try
						{
							Console.Write("Loading APV File...");

							BatchAPV batchAPV = batchBase as BatchAPV;
							if (!File.Exists(batchAPV.CFGRFilename))
							{
								WriteBatchLogFile("Error (CFGR File Doesn't Exist):", strFile + ".log");
								WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
								for (int j = 1; j < batchBase.BatchText.Count; j++)
								{
									WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
								}
								Console.WriteLine("Error (CFGR File Doesn't Exist)" + Environment.NewLine + "Log Available at " + strFile + ".log");
								continue;
							}
							string errAPV = "";
							ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation = APVX.APVCommonClass.loadAPVRFile(batchAPV.APVFilename, ref errAPV);
							if (valuationMethodPoolingAndAggregation == null)
							{
								WriteBatchLogFile("Error (APVX File-No Data):" + errAPV, strFile + ".log");
								WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
								for (int j = 1; j < batchBase.BatchText.Count; j++)
								{
									WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
								}
								Console.WriteLine("Error (APVX File-No Data): " + errAPV + Environment.NewLine + "Log Available at " + strFile + ".log");
								continue;
							}
							string err = "";
							if (batchAPV.CFGRFilename != null && batchAPV.CFGRFilename.Trim() != "")
							{
								valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue =
										Configuration.ConfigurationCommonClass.LoadCFGRFile(batchAPV.CFGRFilename, ref err);

								valuationMethodPoolingAndAggregation.CFGRPath = batchAPV.CFGRFilename;
							}
							else
							{
								valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue =
										Configuration.ConfigurationCommonClass.LoadCFGRFile(valuationMethodPoolingAndAggregation.CFGRPath, ref err);
							}
							CommonClass.BaseControlCRSelectFunctionCalculateValue = valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue;
							if (valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue == null)
							{
								WriteBatchLogFile("Error (CFGRX File-No Data) :" + err, strFile + ".log");
								WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
								for (int j = 1; j < batchBase.BatchText.Count; j++)
								{
									WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
								}
								Console.WriteLine("Error (CFGRX File-No Data): " + err + Environment.NewLine + "Log Available at " + strFile + ".log");
								continue;
							}
							CommonClass.CRSeeds = valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.CRSeeds;
							if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance == null)
								valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance = new IncidencePoolingAndAggregationAdvance();
							if (batchAPV.IncidenceAggregation != null && batchAPV.IncidenceAggregation.Trim() != "")
							{
								valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation = getGridFromName(batchAPV.IncidenceAggregation);
							}
							if (batchAPV.ValuationAggregation != null && batchAPV.ValuationAggregation.Trim() != "")
							{
								valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation = getGridFromName(batchAPV.ValuationAggregation);
							}
							if (batchAPV.DollarYear != null && batchAPV.DollarYear != "")
								//valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncomeGrowthYear = Convert.ToInt32(batchAPV.DollarYear);
								valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.CurrencyYear = Convert.ToInt32(batchAPV.DollarYear);
							//Batch parameter -DollarYear is Inflation Currency Year.
							//Income growth year is different from currency year. Income growth year should remain what is specified in APV (*.apvx) file. 
							if (batchAPV.RandomSeed != -1)
								valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.RandomSeed = batchAPV.RandomSeed.ToString();

							CommonClass.ValuationMethodPoolingAndAggregation = valuationMethodPoolingAndAggregation;
							CommonClass.IncidencePoolingAndAggregationAdvance = valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance;
							CommonClass.lstIncidencePoolingAndAggregation = CommonClassExtension.DeepClone(CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Select(a => a.IncidencePoolingAndAggregation).ToList()); //YY:
							SelectValuationMethods selectValuationMethods = new SelectValuationMethods();

							Console.Write("Completed" + Environment.NewLine);
							selectValuationMethods._filePath = batchAPV.ResultsFilename;
							selectValuationMethods.btnOK_Click(null, null);

						}
						catch (Exception ex)
						{
							WriteBatchLogFile("APV Error :" + ex.Message, strFile + ".log");
							WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
							for (int j = 1; j < batchBase.BatchText.Count; j++)
							{
								WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
							}
							Console.WriteLine("APV Error:" + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + "Log Available at " + strFile + ".log");
						}
					}
					else if (batchBase is BatchReport)
					{
						if (!File.Exists((batchBase as BatchReport).InputFile))
						{
							WriteBatchLogFile("Report Error (Input File Doesn't Exist):", strFile + ".log");
							WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
							for (int j = 1; j < batchBase.BatchText.Count; j++)
							{
								WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
							}
							Console.WriteLine("Report Error (Input File Doesn't Exist)" + Environment.NewLine + "Log Available at " + strFile + ".log");
							continue;
						}

						if (batchBase is BatchReportAuditTrail)
						{
							try
							{
								BatchReportAuditTrail batchReportAuditTrail = batchBase as BatchReportAuditTrail;
								string filePath = batchReportAuditTrail.InputFile;
								string fileType = Path.GetExtension(batchReportAuditTrail.InputFile);
								Console.Write("Generating Audit Trail...");
								switch (fileType)
								{
									case ".aqgx":
										BenMAPLine aqgBenMAPLine = new BenMAPLine();
										string err = "";
										aqgBenMAPLine = DataSourceCommonClass.LoadAQGFile(filePath, ref err);
										if (aqgBenMAPLine == null)
										{
											WriteBatchLogFile("Report Error (AGGX File-No Data) :" + err, strFile + ".log");
											WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
											for (int j = 1; j < batchBase.BatchText.Count; j++)
											{
												WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
											}
											Console.WriteLine("Report Error (AGGX File-No Data): " + err + Environment.NewLine + "Log Available at " + strFile + ".log");
											continue;
										}
										TreeNode aqgTreeNode = new TreeNode();
										aqgTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBenMAPLine(aqgBenMAPLine);
										exportToTxt(aqgTreeNode, batchReportAuditTrail.ReportFile);
										break;
									case ".cfgx":
										BaseControlCRSelectFunction cfgFunction = new BaseControlCRSelectFunction();
										err = "";
										cfgFunction = Configuration.ConfigurationCommonClass.loadCFGFile(filePath, ref err);
										if (cfgFunction == null)
										{
											WriteBatchLogFile("Report Error (CFGX File-No Data) :" + err, strFile + ".log");
											WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
											for (int j = 1; j < batchBase.BatchText.Count; j++)
											{
												WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
											}
											Console.WriteLine("Report Error (CFGX File-No Data): " + err + Environment.NewLine + "Log Available at " + strFile + ".log");
											continue;
										}
										TreeNode cfgTreeNode = new TreeNode();
										cfgTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunction(cfgFunction);
										exportToTxt(cfgTreeNode, batchReportAuditTrail.ReportFile);
										break;
									case ".cfgrx":
										BaseControlCRSelectFunctionCalculateValue cfgrFunctionCV = new BaseControlCRSelectFunctionCalculateValue();
										err = "";
										cfgrFunctionCV = Configuration.ConfigurationCommonClass.LoadCFGRFile(filePath, ref err);
										if (cfgrFunctionCV == null)
										{
											WriteBatchLogFile("Report Error (CFGRX File-No Data) :" + err, strFile + ".log");
											WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
											for (int j = 1; j < batchBase.BatchText.Count; j++)
											{
												WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
											}
											Console.WriteLine("Report Error (CFGRX File-No Data): " + err + Environment.NewLine + "Log Available at " + strFile + ".log");
											continue;
										}
										TreeNode cfgrTreeNode = new TreeNode();
										cfgrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromBaseControlCRSelectFunctionCalculateValue(cfgrFunctionCV);
										exportToTxt(cfgrTreeNode, batchReportAuditTrail.ReportFile);
										break;
									case ".apvx":
									case ".apvrx":
										err = "";
										ValuationMethodPoolingAndAggregation apvrVMPA = new ValuationMethodPoolingAndAggregation();
										apvrVMPA = APVX.APVCommonClass.loadAPVRFile(filePath, ref err);
										if (apvrVMPA == null)
										{
											WriteBatchLogFile("Report Error (APVR File-No Data) :" + err, strFile + ".log");
											WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
											for (int j = 1; j < batchBase.BatchText.Count; j++)
											{
												WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
											}
											Console.WriteLine("Report Error (APVR File-No Data): " + err + Environment.NewLine + "Log Available at " + strFile + ".log");
											continue;
										}
										TreeNode apvrTreeNode = new TreeNode();
										apvrTreeNode = AuditTrailReportCommonClass.getTreeNodeFromValuationMethodPoolingAndAggregation(apvrVMPA);
										exportToTxt(apvrTreeNode, batchReportAuditTrail.ReportFile);
										break;
								}
								Console.WriteLine("Completed" + Environment.NewLine);
								Console.WriteLine("Results Saved At:" + Environment.NewLine);
								Console.WriteLine(batchReportAuditTrail.ReportFile + Environment.NewLine);
							}
							catch (Exception ex)
							{
								WriteBatchLogFile("Report Error :" + ex.Message, strFile + ".log");
								WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
								for (int j = 1; j < batchBase.BatchText.Count; j++)
								{
									WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
								}
								Console.WriteLine("Report Error: " + ex.Message + Environment.NewLine + "Log Available at " + strFile + ".log");
							}
						}
						else if (batchBase is BatchReportCFGR)
						{
							try
							{
								int prevTop = Console.CursorTop;
								BenMAP benMAP = new BenMAP("");
								int currTop = Console.CursorTop;
								for (int i = currTop; i > prevTop - 1; i--)         //This code clears the command line of output text from appManager1.LoadExtensions(); in initialization of BenMAP
								{
									Console.SetCursorPosition(0, i);
									Console.Write(new String(' ', Console.BufferWidth));
								}
								Console.SetCursorPosition(0, prevTop);

								BatchReportCFGR batchReportCFGR = batchBase as BatchReportCFGR;
								Dictionary<string, string> dicBatchReportParameter = new Dictionary<string, string>();//BenMAP-552 allow passing AllPercentiles parameter to btnTableOutput_Click

								Console.Write("Generating Report (CFGR)...");
								benMAP._outputFileName = batchReportCFGR.ReportFile;
								string err = "";
								BaseControlCRSelectFunctionCalculateValue bControlCR = Configuration.ConfigurationCommonClass.LoadCFGRFile(batchReportCFGR.InputFile, ref err);
								if (bControlCR == null)
								{
									WriteBatchLogFile("Report Error (CFGRX File-No Data) :" + err, strFile + ".log");
									WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
									for (int j = 1; j < batchBase.BatchText.Count; j++)
									{
										WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
									}
									Console.WriteLine("Report Error (CFGRX File-No Data): " + err + Environment.NewLine + "Log Available at " + strFile + ".log");
									continue;
								}
								if (batchReportCFGR.GridFields != null && batchReportCFGR.GridFields.Trim() != "")
								{
									string[] strTemp = batchReportCFGR.GridFields.ToLower().Split(new char[] { ',' }).Select(p => p.Trim()).ToArray(); //BENMAP-552
									if (benMAP.cflstColumnRow == null)
									{
										benMAP.cflstColumnRow = new List<FieldCheck>();
										benMAP.cflstColumnRow.Add(new FieldCheck() { FieldName = "Column", isChecked = false });
										benMAP.cflstColumnRow.Add(new FieldCheck() { FieldName = "Row", isChecked = false });
									}
									if (strTemp.Contains("column"))
									{
										benMAP.cflstColumnRow.Where(p => p.FieldName == "Column").First().isChecked = true;
									}
									if (strTemp.Contains("row"))
									{
										benMAP.cflstColumnRow.Where(p => p.FieldName == "Row").First().isChecked = true;
									}
								}
								if (batchReportCFGR.CustomFields != null && batchReportCFGR.CustomFields.Trim() != "")
								{
									string[] strTemp = batchReportCFGR.CustomFields.ToLower().Split(new char[] { ',' }).Select(p => p.Trim()).ToArray(); //BENMAP-552;
									if (benMAP.cflstHealth == null)
									{
										benMAP.cflstHealth = new List<FieldCheck>();
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "DataSet", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Endpoint Group", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Endpoint", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Pollutant", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Metric", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Seasonal Metric", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Metric Statistic", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Author", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Year", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Location", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Other Pollutants", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Qualifier", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Reference", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Race", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Ethnicity", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Gender", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Start Age", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "End Age", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Function", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Incidence Dataset", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Prevalence Dataset", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Beta", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Disbeta", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "P1Beta", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "P2Beta", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "A", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "NameA", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "B", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "NameB", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "C", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "NameC", isChecked = false });
										benMAP.cflstHealth.Add(new FieldCheck() { FieldName = "Geographic Area", isChecked = false });

									}
									if (strTemp.Contains("dataset"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "DataSet").First().isChecked = true;
									}
									if (strTemp.Contains("endpoint group") || strTemp.Contains("endpointgroup"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Endpoint Group").First().isChecked = true;
									}
									if (strTemp.Contains("endpoint"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Endpoint").First().isChecked = true;
									}
									if (strTemp.Contains("pollutant"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Pollutant").First().isChecked = true;
									}
									if (strTemp.Contains("metric"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Metric").First().isChecked = true;
									}
									if (strTemp.Contains("seasonal metric") || strTemp.Contains("seasonalmetric"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Seasonal Metric").First().isChecked = true;
									}
									if (strTemp.Contains("metric statistic") || strTemp.Contains("metricstatistic"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Metric Statistic").First().isChecked = true;
									}
									if (strTemp.Contains("author"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Author").First().isChecked = true;
									}
									if (strTemp.Contains("dataSet"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "DataSet").First().isChecked = true;
									}
									if (strTemp.Contains("year"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Year").First().isChecked = true;
									}
									if (strTemp.Contains("location"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Location").First().isChecked = true;
									}
									if (strTemp.Contains("other pollutants") || strTemp.Contains("otherpollutants"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Other Pollutants").First().isChecked = true;
									}
									if (strTemp.Contains("qualifier"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Qualifier").First().isChecked = true;
									}
									if (strTemp.Contains("reference"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Reference").First().isChecked = true;
									}
									if (strTemp.Contains("race"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Race").First().isChecked = true;
									}
									if (strTemp.Contains("ethnicity"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Ethnicity").First().isChecked = true;
									}
									if (strTemp.Contains("gender"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Gender").First().isChecked = true;
									}
									if (strTemp.Contains("start age") || strTemp.Contains("startage"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Start Age").First().isChecked = true;
									}
									if (strTemp.Contains("end age") || strTemp.Contains("endage"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "End Age").First().isChecked = true;
									}
									if (strTemp.Contains("function"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Function").First().isChecked = true;
									}
									if (strTemp.Contains("incidence dataset") || strTemp.Contains("incidencedataset"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Incidence Dataset").First().isChecked = true;
									}
									if (strTemp.Contains("prevalence dataset") || strTemp.Contains("prevalencedataset"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Prevalence Dataset").First().isChecked = true;
									}
									if (strTemp.Contains("beta"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Beta").First().isChecked = true;
									}
									if (strTemp.Contains("disbeta"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Disbeta").First().isChecked = true;
									}
									if (strTemp.Contains("p1beta"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "P1Beta").First().isChecked = true;
									}
									if (strTemp.Contains("p2beta"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "P2Beta").First().isChecked = true;
									}
									if (strTemp.Contains("a"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "A").First().isChecked = true;
									}
									if (strTemp.Contains("namea"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "NameA").First().isChecked = true;
									}
									if (strTemp.Contains("b"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "B").First().isChecked = true;
									}
									if (strTemp.Contains("nameb"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "NameB").First().isChecked = true;
									}
									if (strTemp.Contains("c"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "C").First().isChecked = true;
									}
									if (strTemp.Contains("namec"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "NameC").First().isChecked = true;
									}
									if (strTemp.Contains("geographic area") || strTemp.Contains("geographicarea"))
									{
										benMAP.cflstHealth.Where(p => p.FieldName == "Geographic Area").First().isChecked = true;
									}
								}
								if (batchReportCFGR.ResultFields != null && batchReportCFGR.ResultFields.Trim() != "")
								{
									string[] strTemp = batchReportCFGR.ResultFields.ToLower().Split(new char[] { ',' }).Select(p => p.Trim()).ToArray(); //BENMAP-552
									if (benMAP.cflstResult == null)
									{
										benMAP.cflstResult = new List<FieldCheck>();
										benMAP.cflstResult.Add(new FieldCheck() { FieldName = "Point Estimate", isChecked = false });
										benMAP.cflstResult.Add(new FieldCheck() { FieldName = "Population", isChecked = false });
										benMAP.cflstResult.Add(new FieldCheck() { FieldName = "Delta", isChecked = false });
										benMAP.cflstResult.Add(new FieldCheck() { FieldName = "Mean", isChecked = false });
										benMAP.cflstResult.Add(new FieldCheck() { FieldName = "Baseline", isChecked = false });
										benMAP.cflstResult.Add(new FieldCheck() { FieldName = "Percent of Baseline", isChecked = false });
										benMAP.cflstResult.Add(new FieldCheck() { FieldName = "Standard Deviation", isChecked = false });
										benMAP.cflstResult.Add(new FieldCheck() { FieldName = "Variance", isChecked = false });
										benMAP.cflstResult.Add(new FieldCheck() { FieldName = "Percentiles", isChecked = false });

									}
									if (strTemp.Contains("pointestimate") || strTemp.Contains("point estimate"))
									{
										benMAP.cflstResult.Where(p => p.FieldName == "Point Estimate").First().isChecked = true;
									}
									if (strTemp.Contains("incidence"))
									{
										benMAP.cflstResult.Where(p => p.FieldName == "Incidence").First().isChecked = true;
									}
									if (strTemp.Contains("population"))
									{
										benMAP.cflstResult.Where(p => p.FieldName == "Population").First().isChecked = true;
									}
									if (strTemp.Contains("delta"))
									{
										benMAP.cflstResult.Where(p => p.FieldName == "Delta").First().isChecked = true;
									}
									if (strTemp.Contains("mean"))
									{
										benMAP.cflstResult.Where(p => p.FieldName == "Mean").First().isChecked = true;
									}
									if (strTemp.Contains("baseline"))
									{
										benMAP.cflstResult.Where(p => p.FieldName == "Baseline").First().isChecked = true;
									}
									if (strTemp.Contains("percent of baseline") || strTemp.Contains("percentofbaseline"))
									{
										benMAP.cflstResult.Where(p => p.FieldName == "Percent of Baseline").First().isChecked = true;
									}
									if (strTemp.Contains("standard deviation") || strTemp.Contains("standarddeviation"))
									{
										benMAP.cflstResult.Where(p => p.FieldName == "Standard Deviation").First().isChecked = true;
									}
									if (strTemp.Contains("variance"))
									{
										benMAP.cflstResult.Where(p => p.FieldName == "Variance").First().isChecked = true;
									}
									if (strTemp.Contains("percentiles"))
									{
										benMAP.cflstResult.Where(p => p.FieldName == "Percentiles").First().isChecked = true;
									}
									else
									{
										//BenMAP-543. Percentiles fields are required to generate "Formatted Results" fields, which were added to commandline GENERATE REPORT CFGR output since BENMAP530. 
										//Therefore, Percentiles fileds are always needed when using GENERATE REPORT CFGR command.
										//In the future, it is better to either update the user menu (v April 2021) to incidate that Percentiles will always be included in report, 
										//or update the code so that although "Formatted Results" fields are calculated using Percentiles fields, the output cvs will not include Percentiles fields. 
										benMAP.cflstResult.Where(p => p.FieldName == "Percentiles").First().isChecked = true;
									}
									if (strTemp.Contains("all percentiles"))
									{
										//BENMAP-552
										dicBatchReportParameter["percentiles"] = "all percentiles";
									}
								}
								benMAP._tableObject = bControlCR.lstCRSelectFunctionCalculateValue;
								benMAP.btnTableOutput_Click(dicBatchReportParameter, null); //BENMAP-552


								Console.Write("Completed" + Environment.NewLine);
								Console.Write("Results Located At:" + Environment.NewLine);
								Console.Write(batchReportCFGR.ReportFile);
							}
							catch (Exception ex)
							{
								WriteBatchLogFile("Report Error (CFGR):" + ex.Message, strFile + ".log");
								WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
								for (int j = 1; j < batchBase.BatchText.Count; j++)
								{
									WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
								}
								Console.WriteLine("Report Error (CFGR): " + ex.Message + Environment.NewLine + "Log Available at " + strFile + ".log");
							}

						}
						else if (batchBase is BatchReportAPVR)
						{
							try
							{
								int prevTop = Console.CursorTop;
								BenMAP benMAP = new BenMAP("");
								int currTop = Console.CursorTop;
								for (int i = currTop; i > prevTop - 1; i--)             //This code clears the command line of output text from appManager1.LoadExtensions(); in initialization of BenMAP
								{
									Console.SetCursorPosition(0, i);
									Console.Write(new String(' ', Console.BufferWidth));
								}
								Console.SetCursorPosition(0, prevTop);
								Console.Write("Generating Report (APVR)...");
								BatchReportAPVR batchReportAPVR = batchBase as BatchReportAPVR;
								Dictionary<string, string> dicBatchReportParameter = new Dictionary<string, string>();//BenMAP-552 allow passing AllPercentiles parameter to btnTableOutput_Click

								ValuationMethodPoolingAndAggregation apvrVMPA = new ValuationMethodPoolingAndAggregation();
								string err = "";
								apvrVMPA = APVX.APVCommonClass.loadAPVRFile(batchReportAPVR.InputFile, ref err);
								if (apvrVMPA == null)
								{
									WriteBatchLogFile("Report Error (APVRX File-No Data) :" + err, strFile + ".log");
									WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
									for (int j = 1; j < batchBase.BatchText.Count; j++)
									{
										WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
									}
									Console.WriteLine("Report Error (APVRX File-No Data): " + err + Environment.NewLine + "Log Available at " + strFile + ".log");
									continue;
								}
								CommonClass.GBenMAPGrid = apvrVMPA.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.First().GridType;
								CommonClass.ValuationMethodPoolingAndAggregation = apvrVMPA;
								CommonClass.IncidencePoolingAndAggregationAdvance = CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance;
								CommonClass.BaseControlCRSelectFunctionCalculateValue = CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue;
								CommonClass.BaseControlCRSelectFunction = null;
								CommonClass.BaseControlCRSelectFunction = new BaseControlCRSelectFunction();
								CommonClass.BaseControlCRSelectFunction.BaseControlGroup = CommonClass.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup;
								CommonClass.BaseControlCRSelectFunction.BenMAPPopulation = CommonClass.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
								CommonClass.BaseControlCRSelectFunction.CRLatinHypercubePoints = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints;
								CommonClass.BaseControlCRSelectFunction.CRDefaultMonteCarloIterations = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRDefaultMonteCarloIterations;
								CommonClass.BaseControlCRSelectFunction.CRRunInPointMode = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRRunInPointMode;
								CommonClass.BaseControlCRSelectFunction.CRSeeds = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRSeeds;
								CommonClass.BaseControlCRSelectFunction.CRThreshold = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRThreshold;
								CommonClass.BaseControlCRSelectFunction.RBenMapGrid = CommonClass.BaseControlCRSelectFunctionCalculateValue.RBenMapGrid;
								CommonClass.BaseControlCRSelectFunction.incidenceAvgSelected = CommonClass.BaseControlCRSelectFunctionCalculateValue.incidenceAvgSelected; //YY: 539
								CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction = new List<CRSelectFunction>();
								for (int i = 0; i < CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; i++)
								{
									CommonClass.BaseControlCRSelectFunction.lstCRSelectFunction.Add(CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[i].CRSelectFunction);
								}

								CommonClass.MainSetup = CommonClass.getBenMAPSetupFromID(CommonClass.BaseControlCRSelectFunction.BaseControlGroup.First().GridType.SetupID);
								if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null && CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null)
								{
									CommonClass.lstCRResultAggregation = new List<CRSelectFunctionCalculateValue>();
									CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation = new List<CRSelectFunctionCalculateValue>(); //YY: new added
									foreach (CRSelectFunctionCalculateValue crv in CommonClass.ValuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
									{
										//CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));
										//YY: calculate incidence aggregation
										CommonClass.lstCRResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID));
										//YY: calculate valuation aggregation in a seperate object
										CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation.Add(APVX.APVCommonClass.ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID));

									}
								}
								//for valuation pooling
								foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
								{
									bool bHavePooling = false;
									foreach (AllSelectCRFunction alsc in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(pa => pa.PoolingMethod == "").ToList())
									{
										if (alsc.PoolingMethod == "" && alsc.NodeType == 100)
										{
											try
											{
												if (bHavePooling == false && alsc.CRSelectFunctionCalculateValue != null && alsc.CRSelectFunctionCalculateValue.CRCalculateValues != null && alsc.CRSelectFunctionCalculateValue.CRCalculateValues.Count > 0)
												{
													bHavePooling = true;
												}
												if (CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation == null || CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation.Count == 0)
												{
													if ((CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
															&& CommonClass.lstCRResultAggregation != null
															&& CommonClass.lstCRResultAggregation.Count != 0)
													{
														alsc.CRSelectFunctionCalculateValue = CommonClass.lstCRResultAggregation.Where(pa => pa.CRSelectFunction.CRID == alsc.CRID).First();
													}
													else
													{
														alsc.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(pa => pa.CRSelectFunction.CRID == alsc.CRID).First();
													}

												}
												else
												{
													alsc.CRSelectFunctionCalculateValue = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation.Where(pa => pa.CRSelectFunction.CRID == alsc.CRID).First();
												}
											}
											catch
											{
											}
										}
										else
										{
											alsc.CRSelectFunctionCalculateValue = null;
										}
									}

									if (bHavePooling == false)
									{
										List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
										if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().PoolingMethod == "None" || (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().PoolingMethod == "" && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().NodeType != 100))
										{
											APVX.APVCommonClass.getAllChildCRNotNoneForPooling(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First(), vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);

										}
										lstCR.Insert(0, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First());
										if (lstCR.Count == 1 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID < 9999 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID > 0) { }
										else
										{
											APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(true, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(pa => pa.NodeType != 100).Max(pa => pa.NodeType), vb.IncidencePoolingAndAggregation.lstColumns);
										}
									}
								}

								//YY: new added for incidence pooling
								CommonClass.lstIncidencePoolingAndAggregation = CommonClassExtension.DeepClone(CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Select(a => a.IncidencePoolingAndAggregation).ToList()); //YY:
								foreach (IncidencePoolingAndAggregation ip in CommonClass.lstIncidencePoolingAndAggregation)
								{
									bool bHavePooling = false;
									foreach (AllSelectCRFunction ascr in ip.lstAllSelectCRFuntion.Where(p => p.PoolingMethod == "").ToList())
									{
										if (ascr.PoolingMethod == "" && ascr.NodeType == 100)
										{
											try
											{
												if (bHavePooling == false && ascr.CRSelectFunctionCalculateValue != null && ascr.CRSelectFunctionCalculateValue.CRCalculateValues != null && ascr.CRSelectFunctionCalculateValue.CRCalculateValues.Count > 0)
												{
													bHavePooling = true;
												}
												if (CommonClass.lstCRResultAggregation == null || CommonClass.lstCRResultAggregation.Count == 0)
												{
													ascr.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(pa => pa.CRSelectFunction.CRID == ascr.CRID).First();
												}
												else
												{
													ascr.CRSelectFunctionCalculateValue = CommonClass.lstCRResultAggregation.Where(pa => pa.CRSelectFunction.CRID == ascr.CRID).First();
												}
											}
											catch
											{

											}
										}
										else
										{
											ascr.CRSelectFunctionCalculateValue = null;
										}
									}

									if (bHavePooling == false)
									{
										List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
										if (ip.lstAllSelectCRFuntion.First().PoolingMethod == "None" || (ip.lstAllSelectCRFuntion.First().PoolingMethod == "" && ip.lstAllSelectCRFuntion.First().NodeType != 100))
										{
											APVX.APVCommonClass.getAllChildCRNotNoneForPooling(ip.lstAllSelectCRFuntion.First(), ip.lstAllSelectCRFuntion, ref lstCR);

										}
										lstCR.Insert(0, ip.lstAllSelectCRFuntion.First());
										if (lstCR.Count == 1 && ip.lstAllSelectCRFuntion.First().CRID < 9999 && ip.lstAllSelectCRFuntion.First().CRID > 0) { }
										else
										{
											APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(true, ref ip.lstAllSelectCRFuntion, ref ip.lstAllSelectCRFuntion, ip.lstAllSelectCRFuntion.Where(pa => pa.NodeType != 100).Max(pa => pa.NodeType), ip.lstColumns);
										}
									}
								}
								foreach (ValuationMethodPoolingAndAggregationBase vb in CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
								{
									bool bHavePooling = false;
									foreach (AllSelectCRFunction alsc in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(pa => pa.PoolingMethod == "").ToList())
									{
										if (alsc.PoolingMethod == "" && alsc.NodeType == 100)
										{
											try
											{
												if (bHavePooling == false && alsc.CRSelectFunctionCalculateValue != null && alsc.CRSelectFunctionCalculateValue.CRCalculateValues != null && alsc.CRSelectFunctionCalculateValue.CRCalculateValues.Count > 0)
												{
													bHavePooling = true;
												}
												if (CommonClass.lstCRResultAggregation == null || CommonClass.lstCRResultAggregation.Count == 0)
												{
													alsc.CRSelectFunctionCalculateValue = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(pa => pa.CRSelectFunction.CRID == alsc.CRID).First();
												}
												else
												{
													alsc.CRSelectFunctionCalculateValue = CommonClass.lstCRResultAggregation.Where(pa => pa.CRSelectFunction.CRID == alsc.CRID).First();
												}
											}
											catch
											{
											}
										}
										else
										{
											alsc.CRSelectFunctionCalculateValue = null;
										}
									}

									if (bHavePooling == false)
									{
										List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
										if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().PoolingMethod == "None")
										{
											APVX.APVCommonClass.getAllChildCRNotNoneForPooling(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First(), vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);

										}
										lstCR.Insert(0, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First());
										if (lstCR.Count == 1 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID < 9999 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID > 0) { }
										else
										{
											APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(true, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(pa => pa.NodeType != 100).Max(pa => pa.NodeType), vb.IncidencePoolingAndAggregation.lstColumns);
										}
									}
								}
								switch (batchReportAPVR.ResultType)
								{
									case "IncidenceResults":
									//same as PooledIncidence
									case "PooledIncidence":
										if (batchReportAPVR.GridFields != null && batchReportAPVR.GridFields.Trim() != "")
										{
											string[] strTemp = batchReportAPVR.GridFields.ToLower().Split(new char[] { ',' }).Select(p => p.Trim()).ToArray(); //BENMAP-552
											if (benMAP.IncidencelstColumnRow == null)
											{
												benMAP.IncidencelstColumnRow = new List<FieldCheck>();
												benMAP.IncidencelstColumnRow.Add(new FieldCheck() { FieldName = "Column", isChecked = false });
												benMAP.IncidencelstColumnRow.Add(new FieldCheck() { FieldName = "Row", isChecked = false });
											}
											if (strTemp.Contains("column"))
											{
												benMAP.IncidencelstColumnRow.Where(p => p.FieldName == "Column").First().isChecked = true;
											}
											if (strTemp.Contains("row"))
											{
												benMAP.IncidencelstColumnRow.Where(p => p.FieldName == "Row").First().isChecked = true;
											}
										}
										if (batchReportAPVR.CustomFields != null && batchReportAPVR.CustomFields.Trim() != "")
										{
											string[] strTemp = batchReportAPVR.CustomFields.ToLower().Split(new char[] { ',' }).Select(p => p.Trim()).ToArray(); //BENMAP-552
											if (benMAP.IncidencelstHealth == null)
											{
												benMAP.IncidencelstHealth = new List<FieldCheck>();
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "DataSet", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Endpoint Group", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Endpoint", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Pollutant", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Metric", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Seasonal Metric", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Metric Statistic", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Author", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Year", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Location", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Other Pollutants", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Qualifier", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Reference", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Race", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Ethnicity", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Gender", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Start Age", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "End Age", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Function", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Incidence Dataset", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Prevalence Dataset", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Beta", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Disbeta", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "P1Beta", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "P2Beta", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "A", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "NameA", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "B", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "NameB", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "C", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "NameC", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Version", isChecked = false });
												benMAP.IncidencelstHealth.Add(new FieldCheck() { FieldName = "Geographic Area", isChecked = false });
											}
											if (strTemp.Contains("dataset"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "DataSet").First().isChecked = true;
											}
											if (strTemp.Contains("endpoint group") || strTemp.Contains("endpointgroup"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Endpoint Group").First().isChecked = true;
											}
											if (strTemp.Contains("endpoint"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Endpoint").First().isChecked = true;
											}
											if (strTemp.Contains("pollutant"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Pollutant").First().isChecked = true;
											}
											if (strTemp.Contains("metric"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Metric").First().isChecked = true;
											}
											if (strTemp.Contains("seasonal metric") || strTemp.Contains("seasonalmetric"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Seasonal Metric").First().isChecked = true;
											}
											if (strTemp.Contains("metric statistic") || strTemp.Contains("metricstatistic"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Metric Statistic").First().isChecked = true;
											}
											if (strTemp.Contains("author"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Author").First().isChecked = true;
											}
											if (strTemp.Contains("dataset"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "DataSet").First().isChecked = true;
											}
											if (strTemp.Contains("year"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Year").First().isChecked = true;
											}
											if (strTemp.Contains("location"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Location").First().isChecked = true;
											}
											if (strTemp.Contains("other pollutants") || strTemp.Contains("otherpollutants"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Other Pollutants").First().isChecked = true;
											}
											if (strTemp.Contains("qualifier"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Qualifier").First().isChecked = true;
											}
											if (strTemp.Contains("reference"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Reference").First().isChecked = true;
											}
											if (strTemp.Contains("race"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Race").First().isChecked = true;
											}
											if (strTemp.Contains("ethnicity"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Ethnicity").First().isChecked = true;
											}
											if (strTemp.Contains("gender"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Gender").First().isChecked = true;
											}
											if (strTemp.Contains("start age") || strTemp.Contains("startage"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Start Age").First().isChecked = true;
											}
											if (strTemp.Contains("end age") || strTemp.Contains("endage"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "End Age").First().isChecked = true;
											}
											if (strTemp.Contains("function"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Function").First().isChecked = true;
											}
											if (strTemp.Contains("incidence dataset") || strTemp.Contains("incidencedataset"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Incidence Dataset").First().isChecked = true;
											}
											if (strTemp.Contains("prevalence dataset") || strTemp.Contains("prevalencedataset"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Prevalence Dataset").First().isChecked = true;
											}
											if (strTemp.Contains("beta"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Beta").First().isChecked = true;
											}
											if (strTemp.Contains("disbeta"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Disbeta").First().isChecked = true;
											}
											if (strTemp.Contains("p1beta"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "P1Beta").First().isChecked = true;
											}
											if (strTemp.Contains("p2beta"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "P2Beta").First().isChecked = true;
											}
											if (strTemp.Contains("a"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "A").First().isChecked = true;
											}
											if (strTemp.Contains("namea"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "NameA").First().isChecked = true;
											}
											if (strTemp.Contains("b"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "B").First().isChecked = true;
											}
											if (strTemp.Contains("nameb"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "NameB").First().isChecked = true;
											}
											if (strTemp.Contains("c"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "C").First().isChecked = true;
											}
											if (strTemp.Contains("namec"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "NameC").First().isChecked = true;
											}
											if (strTemp.Contains("version"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Version").First().isChecked = true;
											}
											if (strTemp.Contains("geographic area") || strTemp.Contains("geographicarea"))
											{
												benMAP.IncidencelstHealth.Where(p => p.FieldName == "Geographic Area").First().isChecked = true;
											}
										}
										if (batchReportAPVR.ResultFields != null && batchReportAPVR.ResultFields.Trim() != "")
										{
											string[] strTemp = batchReportAPVR.ResultFields.ToLower().Split(new char[] { ',' }).Select(p => p.Trim()).ToArray(); //BENMAP-552
											if (benMAP.IncidencelstResult == null)
											{
												benMAP.IncidencelstResult = new List<FieldCheck>();
												benMAP.IncidencelstResult.Add(new FieldCheck() { FieldName = "Point Estimate", isChecked = false });
												benMAP.IncidencelstResult.Add(new FieldCheck() { FieldName = "Population", isChecked = false });
												benMAP.IncidencelstResult.Add(new FieldCheck() { FieldName = "Delta", isChecked = false });
												benMAP.IncidencelstResult.Add(new FieldCheck() { FieldName = "Mean", isChecked = false });
												benMAP.IncidencelstResult.Add(new FieldCheck() { FieldName = "Baseline", isChecked = false });
												benMAP.IncidencelstResult.Add(new FieldCheck() { FieldName = "Percent of Baseline", isChecked = false });
												benMAP.IncidencelstResult.Add(new FieldCheck() { FieldName = "Standard Deviation", isChecked = false });
												benMAP.IncidencelstResult.Add(new FieldCheck() { FieldName = "Variance", isChecked = false });
												benMAP.IncidencelstResult.Add(new FieldCheck() { FieldName = "Percentiles", isChecked = false });

											}
											if (strTemp.Contains("pointestimate") || strTemp.Contains("point estimate"))
											{
												benMAP.IncidencelstResult.Where(p => p.FieldName == "Point Estimate").First().isChecked = true;
											}
											if (strTemp.Contains("incidence"))
											{
												benMAP.IncidencelstResult.Where(p => p.FieldName == "Incidence").First().isChecked = true;
											}
											if (strTemp.Contains("population"))
											{
												benMAP.IncidencelstResult.Where(p => p.FieldName == "Population").First().isChecked = true;
											}
											if (strTemp.Contains("delta"))
											{
												benMAP.IncidencelstResult.Where(p => p.FieldName == "Delta").First().isChecked = true;
											}
											if (strTemp.Contains("mean"))
											{
												benMAP.IncidencelstResult.Where(p => p.FieldName == "Mean").First().isChecked = true;
											}
											if (strTemp.Contains("baseline"))
											{
												benMAP.IncidencelstResult.Where(p => p.FieldName == "Baseline").First().isChecked = true;
											}
											if (strTemp.Contains("percent of baseline") || strTemp.Contains("percentofbaseline"))
											{
												benMAP.IncidencelstResult.Where(p => p.FieldName == "Percent of Baseline").First().isChecked = true;
											}
											if (strTemp.Contains("standard deviation") || strTemp.Contains("standarddeviation"))
											{
												benMAP.IncidencelstResult.Where(p => p.FieldName == "Standard Deviation").First().isChecked = true;
											}
											if (strTemp.Contains("variance"))
											{
												benMAP.IncidencelstResult.Where(p => p.FieldName == "Variance").First().isChecked = true;
											}
											if (strTemp.Contains("percentiles"))
											{
												benMAP.IncidencelstResult.Where(p => p.FieldName == "Percentiles").First().isChecked = true;
											}
											else
											{
												//BenMAP-543. Percentiles fields are USUALLY required to generate "Formatted Results" fields, which were added to commandline GENERATE REPORT CFGR output since BENMAP530. 
												//Currently for command GENERATE REPORT APVR with parameter -ResultType = PooledIncidence, Percentiles fields are always added to export no matter if it's listed in "-ResultFields" or not.
												//To be consistent with GENERATE REPORT APVR with parameter -ResultType = PooledValuation and GENERATE REPORT CFGR, we make this field always show in batch report. 
												benMAP.IncidencelstResult.Where(p => p.FieldName == "Percentiles").First().isChecked = true;
											}
                                            if (strTemp.Contains("all percentiles"))
                                            {
												//BENMAP-552
												dicBatchReportParameter["percentiles"] = "all percentiles";
											}
										}
										break;
									case "PooledValuation":
										if (batchReportAPVR.GridFields != null && batchReportAPVR.GridFields.Trim() != "")
										{
											string[] strTemp = batchReportAPVR.GridFields.ToLower().Split(new char[] { ',' }).Select(p => p.Trim()).ToArray(); //BENMAP-552
											if (benMAP.apvlstColumnRow == null)
											{
												benMAP.apvlstColumnRow = new List<FieldCheck>();
												benMAP.apvlstColumnRow.Add(new FieldCheck() { FieldName = "Column", isChecked = false });
												benMAP.apvlstColumnRow.Add(new FieldCheck() { FieldName = "Row", isChecked = false });
											}
											if (strTemp.Contains("column"))
											{
												benMAP.apvlstColumnRow.Where(p => p.FieldName == "Column").First().isChecked = true;
											}
											if (strTemp.Contains("row"))
											{
												benMAP.apvlstColumnRow.Where(p => p.FieldName == "Row").First().isChecked = true;
											}
										}
										if (batchReportAPVR.CustomFields != null && batchReportAPVR.CustomFields.ToLower().Trim() != "")
										{
											string[] strTemp = batchReportAPVR.CustomFields.ToLower().Split(new char[] { ',' }).Select(p => p.Trim()).ToArray();//BENMAP-552
											if (benMAP.apvlstHealth == null)
											{
												benMAP.apvlstHealth = new List<FieldCheck>();

												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Name", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Dataset", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Endpoint Group", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Endpoint", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Pollutant", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Metric", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Seasonal Metric", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Metric Statistic", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Author", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Year", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Location", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Other Pollutants", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Qualifier", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Race", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Ethnicity", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Gender", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Start Age", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "End Age", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Function", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Version", isChecked = false });
												benMAP.apvlstHealth.Add(new FieldCheck() { FieldName = "Geographic Area", isChecked = false });
											}
											if (strTemp.Contains("name"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Name").First().isChecked = true;
											}
											if (strTemp.Contains("dataset"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Dataset").First().isChecked = true;
											}
											if (strTemp.Contains("endpoint group") || strTemp.Contains("endpointgroup"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Endpoint Group").First().isChecked = true;
											}
											if (strTemp.Contains("endpoint"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Endpoint").First().isChecked = true;
											}
											if (strTemp.Contains("pollutant"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Pollutant").First().isChecked = true;
											}
											if (strTemp.Contains("metric"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Metric").First().isChecked = true;
											}
											if (strTemp.Contains("seasonal metric") || strTemp.Contains("seasonalmetric"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Seasonal Metric").First().isChecked = true;
											}
											if (strTemp.Contains("metric statistic") || strTemp.Contains("metricstatistic"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Metric Statistic").First().isChecked = true;
											}
											if (strTemp.Contains("author"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Author").First().isChecked = true;
											}

											if (strTemp.Contains("year"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Year").First().isChecked = true;
											}
											if (strTemp.Contains("location"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Location").First().isChecked = true;
											}
											if (strTemp.Contains("other pollutants") || strTemp.Contains("otherpollutants"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Other Pollutants").First().isChecked = true;
											}
											if (strTemp.Contains("qualifier"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Qualifier").First().isChecked = true;
											}
											if (strTemp.Contains("race"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Race").First().isChecked = true;
											}
											if (strTemp.Contains("ethnicity"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Ethnicity").First().isChecked = true;
											}
											if (strTemp.Contains("gender"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Gender").First().isChecked = true;
											}
											if (strTemp.Contains("start age") || strTemp.Contains("startage"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Start Age").First().isChecked = true;
											}
											if (strTemp.Contains("end age") || strTemp.Contains("endage"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "End Age").First().isChecked = true;
											}
											if (strTemp.Contains("function"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Function").First().isChecked = true;
											}
											if (strTemp.Contains("version"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Version").First().isChecked = true;
											}
											if (strTemp.Contains("geographic area") || strTemp.Contains("geopgrahicarea"))
											{
												benMAP.apvlstHealth.Where(p => p.FieldName == "Geographic Area").First().isChecked = true;
											}

										}
										if (batchReportAPVR.ResultFields != null && batchReportAPVR.ResultFields.Trim() != "")
										{
											string[] strTemp = batchReportAPVR.ResultFields.ToLower().Split(new char[] { ',' }).Select(p => p.Trim()).ToArray(); //BENMAP-552
											if (benMAP.apvlstResult == null)
											{
												benMAP.apvlstResult = new List<FieldCheck>();
												benMAP.apvlstResult.Add(new FieldCheck() { FieldName = "Point Estimate", isChecked = false });
												benMAP.apvlstResult.Add(new FieldCheck() { FieldName = "Mean", isChecked = false });
												benMAP.apvlstResult.Add(new FieldCheck() { FieldName = "Standard Deviation", isChecked = false });
												benMAP.apvlstResult.Add(new FieldCheck() { FieldName = "Variance", isChecked = false });
												benMAP.apvlstResult.Add(new FieldCheck() { FieldName = "Percentiles", isChecked = false });

											}
											if (strTemp.Contains("pointestimate") || strTemp.Contains("point estimate"))
											{
												benMAP.apvlstResult.Where(p => p.FieldName == "Point Estimate").First().isChecked = true;
											}
											if (strTemp.Contains("mean"))
											{
												benMAP.apvlstResult.Where(p => p.FieldName == "Mean").First().isChecked = true;
											}
											if (strTemp.Contains("standarddeviation") || strTemp.Contains("standard deviation"))
											{
												benMAP.apvlstResult.Where(p => p.FieldName == "Standard Deviation").First().isChecked = true;
											}
											if (strTemp.Contains("variance"))
											{
												benMAP.apvlstResult.Where(p => p.FieldName == "Variance").First().isChecked = true;
											}
											if (strTemp.Contains("percentiles"))
											{
												benMAP.apvlstResult.Where(p => p.FieldName == "Percentiles").First().isChecked = true;												
											}
											else
											{
												//BenMAP-543. Percentiles fields are required to generate "Formatted Results" fields, which were added to commandline GENERATE REPORT APVR output since BENMAP530. 
												//Therefore, Percentiles fileds are always needed when using GENERATE REPORT APVR command (with ResultType=PooledIncidence).
												//note that the user menu (v April 2021) does not have parameter "-ResultFields" for command GENERATE REPORT APVR, but the code has it. 
												//in the future, we should either update the user manual or ignore "-ResultFields" input for command GENERATE REPORT APVR
												benMAP.apvlstResult.Where(p => p.FieldName == "Percentiles").First().isChecked = true;
											}
											if (strTemp.Contains("all percentiles"))
											{
												//BENMAP-552
												dicBatchReportParameter["percentiles"] = "all percentiles";
											}
										}
										break;
								}



								benMAP._outputFileName = batchReportAPVR.ReportFile;
								switch (batchReportAPVR.ResultType)
								{
									case "IncidenceResults":

										benMAP._tableObject = apvrVMPA.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue;
										benMAP.tabCtlReport.SelectedIndex = 1;
										benMAP.btnTableOutput_Click(dicBatchReportParameter, null); //BENMAP-552

										Console.Write("Completed" + Environment.NewLine);
										Console.WriteLine("Results Located At:");
										Console.WriteLine(batchReportAPVR.ReportFile);
										break;
									case "PooledIncidence":
										List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>(); //In order to display version and dataset in batch mode, must pass the AllSelectCRFunction, not the CRSelectFunctionCalculateValue--[BenMAP 434, MP]
										Dictionary<AllSelectCRFunction, string> dicPooledIncidence = new Dictionary<AllSelectCRFunction, string>(); //BenMAP-543 use the new export function to include pooling name and grid definition.

										benMAP.LoadAllIncidencePooling(ref benMAP.dicIncidencePoolingAndAggregation, ref benMAP.dicIncidencePoolingAndAggregationUnPooled);
										//foreach (KeyValuePair<AllSelectCRFunction, string> keyValueCR in benMAP.dicIncidencePoolingAndAggregation)
										//{
										//	AllSelectCRFunction cr = keyValueCR.Key;

										//	if (cr.CRSelectFunctionCalculateValue == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues == null || cr.CRSelectFunctionCalculateValue.CRCalculateValues.Count == 0)
										//	{
										//		continue;
										//	}
										//	else
										//	{
										//		lstCR.Add(cr);  //In order to display version and dataset in batch mode, must pass the AllSelectCRFunction, not the CRSelectFunctionCalculateValue--[BenMAP 434, MP]
										//	}

										//}
										dicPooledIncidence = benMAP.dicIncidencePoolingAndAggregation; //BenMAP-543

										//benMAP._tableObject = lstCR;
										benMAP._tableObject = dicPooledIncidence; //BenMAP-543
										benMAP.tabCtlReport.SelectedIndex = 1;
										benMAP.btnTableOutput_Click(dicBatchReportParameter, null);//BENMAP-552


										Console.Write("Completed" + Environment.NewLine);
										Console.WriteLine("Results Located At:");
										Console.WriteLine(batchReportAPVR.ReportFile);
										break;
									case "PooledValuation":
										benMAP.loadAllAPVPooling();
										List<AllSelectValuationMethodAndValue> lstallSelectValuationMethodAndValue = new List<AllSelectValuationMethodAndValue>();
										foreach (KeyValuePair<AllSelectValuationMethod, string> keyValue in benMAP.dicAPVPoolingAndAggregation)
										{
											AllSelectValuationMethod allSelectValuationMethod = keyValue.Key;

											AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = null;
											if (allSelectValuationMethod.ID < 0) continue;

											ValuationMethodPoolingAndAggregationBase vb = CommonClass.ValuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Where(p => p.IncidencePoolingAndAggregation.PoolingName == keyValue.Value).First();

											try
											{


												allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue.Where(p => p.AllSelectValuationMethod.ID == allSelectValuationMethod.ID).First();

												if (allSelectValuationMethodAndValue != null && allSelectValuationMethodAndValue.lstAPVValueAttributes != null && allSelectValuationMethodAndValue.lstAPVValueAttributes.Count() > 0)
													lstallSelectValuationMethodAndValue.Add(allSelectValuationMethodAndValue);
											}
											catch
											{
											}
										}

										benMAP._tableObject = lstallSelectValuationMethodAndValue;

										benMAP.btnTableOutput_Click(dicBatchReportParameter, null); //BENMAP-552

										Console.Write("Completed" + Environment.NewLine);
										Console.WriteLine("Results Located At:");
										Console.WriteLine(batchReportAPVR.ReportFile);
										break;
									default:
										WriteBatchLogFile("APVR Report Error (Result Type Not Supported):", strFile + ".log");
										WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
										for (int j = 1; j < batchBase.BatchText.Count; j++)
										{
											WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
										}
										Console.WriteLine("Error (Result Type Not Supported)" + Environment.NewLine + "Log Available at " + strFile + ".log");

										break;
								}
							}
							catch (Exception ex)
							{
								WriteBatchLogFile("Report Error (APVR) :" + ex.Message, strFile + ".log");
								WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
								for (int j = 1; j < batchBase.BatchText.Count; j++)
								{
									WriteBatchLogFile("            " + batchBase.BatchText[j].ToString(), strFile + ".log");
								}
								Console.WriteLine("Report Error (APVR): " + ex.Message + Environment.NewLine + "Log Available at " + strFile + ".log");
							}
						}

					}
				}
			}
			catch
			{
				return false;
			}

			return true;
		}
		public static StreamWriter sw;
		public static bool exportToTxt(TreeNode tv, string filename)
		{
			try
			{
				FileStream fs = new FileStream(filename, FileMode.Create);

				sw = new StreamWriter(fs, Encoding.UTF8);
				sw.WriteLine(tv.Text);
				foreach (TreeNode node in tv.Nodes)
				{
					if (node.Nodes.Count >= 1) //updated to address BenMAP 258/246--printing the text of first-level parent node  (11/26/2019,MP)
					{
						sw.WriteLine("<" + node.Text + ">");
						saveNode(node.Nodes);
						sw.WriteLine("</" + node.Text + ">");
					}
					else
						sw.WriteLine(node.Text);
				}
				sw.WriteLine("</" + tv.Text + ">");

				sw.Close();
				fs.Close();

				return true;
			}

			catch (Exception ex)
			{
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
		public static BenMAPSetup getSetupFromName(string strName)
		{
			try
			{
				string commandText = string.Format("select SetupID,SetupName,SetupProjection from Setups order by SetupID");
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					if (dr["SetupName"].ToString().Trim().ToLower().Equals(strName.ToLower()))
					{
						BenMAPSetup benMAPSetup = new BenMAPSetup()
						{
							SetupID = Convert.ToInt32(dr["SetupID"]),
							SetupName = dr["SetupName"].ToString()
						};
						if (dr["SetupProjection"] != DBNull.Value)
						{
							benMAPSetup.SetupProjection = dr["SetupProjection"].ToString();
						}
						CommonClass.MainSetup = benMAPSetup;
						return benMAPSetup;
					}
				}
			}
			catch
			{ }
			return null;
		}
		public static BenMAPPollutant getPollutantFromName(string strName)
		{
			try
			{
				string commandText = string.Format("select PollutantName,ObservationType,PollutantID from Pollutants where SetupID={0} and PollutantName='{1}'", CommonClass.MainSetup.SetupID, strName);
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				DataRow dr = ds.Tables[0].Rows[0];
				BenMAPPollutant benMAPPollutant = new BenMAPPollutant();
				benMAPPollutant.PollutantID = Convert.ToInt32(dr["PollutantID"]);
				benMAPPollutant.PollutantName = dr["PollutantName"].ToString();
				benMAPPollutant.Observationtype = (ObservationtypeEnum)Convert.ToInt32(dr["ObservationType"]);
				benMAPPollutant.Metrics = Grid.GridCommon.getMetricsFromPollutantID(benMAPPollutant.PollutantID);
				benMAPPollutant.Seasons = Grid.GridCommon.getSeasonFromPollutantID(benMAPPollutant.PollutantID);
				List<SeasonalMetric> lstSeasonalMetric = new List<SeasonalMetric>();
				commandText = string.Format("select SeasonalMetricID from SeasonalMetrics where MetricID in (select MetricID from Metrics where PollutantID={0})", benMAPPollutant.PollutantID);
				ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				foreach (DataRow drSeasonalMetric in ds.Tables[0].Rows)
				{
					lstSeasonalMetric.Add(Grid.GridCommon.getSeasonalMetric(Convert.ToInt32(drSeasonalMetric["SeasonalMetricID"])));
				}
				benMAPPollutant.SesonalMetrics = lstSeasonalMetric;
				return benMAPPollutant;
			}
			catch
			{ }
			return null;
		}
		public static BenMAPGrid getGridFromName(string strName)
		{
			try
			{
				string commandText = string.Format("select GridDefinitionID,SetupID,GridDefinitionName,Columns,RRows,TType from GridDefinitions where SetupID={0} and GridDefinitionName ='{1}'", CommonClass.MainSetup.SetupID, strName);
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				int iGrid = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
				BenMAPGrid benMAPGrid = Grid.GridCommon.getBenMAPGridFromID(iGrid);
				return benMAPGrid;

			}
			catch
			{ }
			return null;
		}
		public static List<BatchBase> ReadBatchFile(string strFile, ref bool checkLog)
		{
			try
			{
				if (!File.Exists(strFile)) return null;
				StreamReader objReader = new StreamReader(strFile);
				string sLine = "";
				ArrayList LineList = new ArrayList();
				int iVARIABLES = -1, iCOMMANDS = -1, iSetup = -1, iSetupEnd = -1; ; Dictionary<string, string> dicVariables = new Dictionary<string, string>();
				string[] strTemp = null;
				while (sLine != null)
				{
					sLine = objReader.ReadLine();
					if (sLine != null && !sLine.Equals(""))
						LineList.Add(sLine);
				}
				objReader.Close();
				BatchBase batchBase = new BatchBase();

				for (int i = 0; i < LineList.Count; i++)
				{
					if (LineList[i].ToString().Trim() == "VARIABLES")
					{
						iVARIABLES = i;
						break;
					}
				}
				if (iVARIABLES > -1)
				{
					for (int i = iVARIABLES + 1; i < LineList.Count; i++)
					{
						if (LineList[i].ToString().Trim()[0] != '%')
						{
							break;
						}
						else
						{
							strTemp = LineList[i].ToString().Trim().Split(new char[] { '%' });
							if (strTemp.Count() == 3 && !dicVariables.ContainsKey(strTemp[1]))
							{
								dicVariables.Add("%" + strTemp[1] + "%", strTemp[2]);
							}

						}
					}
				}

				for (int i = 0; i < LineList.Count; i++)
				{
					if (LineList[i].ToString().Trim() == "COMMANDS")
					{
						iCOMMANDS = i;
						break;
					}
				}
				if (iCOMMANDS == -1) return null;

				for (int i = iCOMMANDS; i < LineList.Count; i++)
				{
					if (LineList[i].ToString().Trim().Contains("SETACTIVESETUP"))
					{
						iSetup = i;
						break;
					}
				}
				if (iSetup > -1)
				{
					for (int i = iSetup + 1; i < LineList.Count; i++)
					{
						if (LineList[i].ToString().Trim()[0] != '-')
						{
							iSetupEnd = i - 1;
							break;
						}
						else
						{
						}
					}
				}
				for (int i = iSetup; i < iSetupEnd + 1; i++)
				{
					if (LineList[i].ToString().Contains("-ActiveSetup"))
					{
						batchBase.ActiveSetup = LineList[i].ToString().Replace("-ActiveSetup", "").Replace("SETACTIVESETUP", "").Trim().Replace("\"", "");
					}
				}

				List<ArrayList> LineListPart = new List<ArrayList>(); int iStart = -1, iEnd = -1;
				for (int i = iCOMMANDS; i < LineList.Count; i++)
				{
					switch (LineList[i].ToString().Trim())
					{
						case "SETACTIVESETUP":
						case "CREATE AQG":
						case "RUN CFG":
						case "RUN APV":
						case "GENERATE REPORT AuditTrail":
						case "GENERATE REPORT CFGR":
						case "GENERATE REPORT APVR":
							if (iStart > -1)
							{
								iEnd = i - 1;
								{
									ArrayList arrayList = LineList.GetRange(iStart, iEnd - iStart + 1);
									LineListPart.Add(new ArrayList());
									foreach (string s in arrayList)
									{
										if (s.Trim().IndexOf("##") != 0)
											LineListPart[LineListPart.Count - 1].Add(s);
									}
								}
								iEnd = -1;
								iStart = i;
							}
							else
							{
								iStart = i;
							}
							break;
					}
				}
				if (iStart > -1 && iEnd == -1)
				{
					ArrayList arrayList = LineList.GetRange(iStart, LineList.Count - iStart);
					LineListPart.Add(new ArrayList());
					foreach (string s in arrayList)
					{
						if (s.Trim().IndexOf("##") != 0)
							LineListPart[LineListPart.Count - 1].Add(s);
					}

				}

				DateTime dateTime = DateTime.Now;
				string msgOut;
				List<BatchBase> lstBatchBase = new List<BatchBase>();
				foreach (ArrayList array in LineListPart)
				{
					switch (array[0].ToString().Trim())
					{
						case "SETACTIVESETUP":
							foreach (string s in array)
							{
								if (s.Contains("-ActiveSetup"))
								{
									strTemp = s.Split(new char[] { '"' });
									if (strTemp.Count() >= 2)
									{
										batchBase.ActiveSetup = strTemp[1];
									}
								}
							}
							break;
						case "CREATE AQG":
							int iMonitorOrModel = 0;
							BatchAQGBase batchAQGBase = new BatchAQGBase();
							for (int i = 1; i < array.Count; i++)
							{
								if (array[i].ToString().Contains("-"))
								{
									if (array[i].ToString().Contains("-Filename"))
									{
										batchAQGBase.Filename = array[i].ToString().Replace("-Filename", "").Replace("\"", "").Trim();
										foreach (KeyValuePair<string, string> k in dicVariables)
										{
											if (batchAQGBase.Filename.Contains(k.Key))
												batchAQGBase.Filename = batchAQGBase.Filename.Replace(k.Key, k.Value).Trim();
										}

									}
									else if (array[i].ToString().Contains("-GridType"))
									{
										batchAQGBase.GridType = array[i].ToString().Replace("-GridType", "").Replace("\"", "").Trim();
									}
									else if (array[i].ToString().Contains("-Pollutant"))
									{
										batchAQGBase.Pollutant = array[i].ToString().Replace("-Pollutant", "").Replace("\"", "").Trim();
									}
								}
								else
								{
									iMonitorOrModel = i;
									break;
								}
							}
							switch (array[iMonitorOrModel].ToString().Trim())
							{
								case "ModelDirect":
									BatchModelDirect batchModelDirect = new BatchModelDirect()
									{
										ActiveSetup = batchBase.ActiveSetup,
										Filename = batchAQGBase.Filename,
										Pollutant = batchAQGBase.Pollutant,
										GridType = batchAQGBase.GridType,
									};
									for (int i = iMonitorOrModel + 1; i < array.Count; i++)
									{
										if (array[i].ToString().Contains("-ModelFilename"))
										{
											batchModelDirect.ModelFilename = array[i].ToString().Replace("-ModelFilename", "").Replace("\"", "").Trim();
											foreach (KeyValuePair<string, string> k in dicVariables)
											{
												if (batchModelDirect.ModelFilename.Contains(k.Key))
													batchModelDirect.ModelFilename = batchModelDirect.ModelFilename.Replace(k.Key, k.Value).Trim();
											}
										}
										else if (array[i].ToString().Contains("-DSNName"))
										{
											batchModelDirect.DSNName = array[i].ToString().Replace("-DSNName", "").Replace("\"", "").Trim();
										}
										else if (array[i].ToString().Contains("-TableName")) //YY: Table Name
										{
											batchModelDirect.ModelTablename = array[i].ToString().Replace("-TableName", "").Replace("\"", "").Trim();
										}
									}
									if (!CheckBatch(batchModelDirect, out msgOut))
									{
										checkLog = true;
										WriteBatchLogFile("Error (Loading AQG Monitor Data): " + msgOut, strFile + ".log");
										WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
										for (int i = 0; i < array.Count; i++)
										{
											WriteBatchLogFile("            " + array[i], strFile + ".log");
										}
										continue;
									}
									else
									{

										batchModelDirect.BatchText = array;
										lstBatchBase.Add(batchModelDirect);
									}
									break;

								case "MonitorDirect":
									BatchMonitorDirect batchMonitorDirect = new BatchMonitorDirect()
									{
										ActiveSetup = batchBase.ActiveSetup,
										Filename = batchAQGBase.Filename,
										Pollutant = batchAQGBase.Pollutant,
										GridType = batchAQGBase.GridType,
									};
									for (int i = iMonitorOrModel + 1; i < array.Count; i++)
									{
										if (array[i].ToString().Contains("-MonitorDataType"))
										{
											batchMonitorDirect.MonitorDataType = array[i].ToString().Replace("-MonitorDataType", "").Replace("\"", "").Trim();
										}
										else if (array[i].ToString().Contains("-InterpolationMethod"))
										{
											batchMonitorDirect.InterpolationMethod = array[i].ToString().Replace("-InterpolationMethod", "").Replace("\"", "").Trim();
										}
										else if (array[i].ToString().Contains("-MonitorDataSet"))
										{
											batchMonitorDirect.MonitorDataSet = array[i].ToString().Replace("-MonitorDataSet", "").Replace("\"", "").Trim();
										}
										else if (array[i].ToString().Contains("-MonitorYear"))
										{
											try
											{
												batchMonitorDirect.MonitorYear = Convert.ToInt32(array[i].ToString().Replace("-MonitorYear", ""));
											}
											catch
											{
												checkLog = true;
												WriteBatchLogFile("Error AQG (Year):", strFile + ".log");
												WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
												for (int j = 0; j < array.Count; j++)
												{
													WriteBatchLogFile("            " + array[j], strFile + ".log");
												}
												continue;
											}
										}
										else if (array[i].ToString().Contains("-MonitorFile"))
										{
											batchMonitorDirect.MonitorFile = array[i].ToString().Replace("-MonitorFile", "").Replace("\"", "").Trim();
											foreach (KeyValuePair<string, string> k in dicVariables)
											{
												if (batchMonitorDirect.MonitorFile.Contains(k.Key))
													batchMonitorDirect.MonitorFile = batchMonitorDirect.MonitorFile.Replace(k.Key, k.Value).Trim();
											}
										}
										else if (array[i].ToString().Contains("-MaxDistance"))
										{
											try
											{
												batchMonitorDirect.MaxDistance = Convert.ToDouble(array[i].ToString().Replace("-MaxDistance", "").Replace("\"", "").Trim());
											}
											catch
											{
												checkLog = true;
												WriteBatchLogFile("Error AQG (MaxDistance):", strFile + ".log");
												WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
												for (int j = 0; j < array.Count; j++)
												{
													WriteBatchLogFile("            " + array[j], strFile + ".log");
												}
												continue;
											}
										}
										else if (array[i].ToString().Contains("-MaxRelativeDistance"))
										{
											try
											{
												batchMonitorDirect.MaxRelativeDistance = Convert.ToDouble(array[i].ToString().Replace("-MaxRelativeDistance", "").Replace("\"", "").Trim());
											}
											catch
											{
												checkLog = true;
												WriteBatchLogFile("Error AQG (MaxRelativeDistance):", strFile + ".log");
												WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
												for (int j = 0; j < array.Count; j++)
												{
													WriteBatchLogFile("            " + array[j], strFile + ".log");
												}
												continue;
											}
										}
										else if (array[i].ToString().Contains("-WeightingMethod"))
										{
											batchMonitorDirect.WeightingMethod = array[i].ToString().Replace("-WeightingMethod", "").Replace("\"", "").Trim();
										}
										else if (array[i].ToString().Contains("-FixRadius"))
										{
											batchMonitorDirect.FixRadius = Convert.ToDouble(array[i].ToString().Replace("-FixRadius", "").Replace("\"", "").Trim());
										}
									}


									if (!CheckBatch(batchMonitorDirect, out msgOut))
									{
										checkLog = true;
										WriteBatchLogFile("Error (Loading AQG Monitor Data): " + msgOut, strFile + ".log");
										WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
										for (int i = 0; i < array.Count; i++)
										{
											WriteBatchLogFile("            " + array[i], strFile + ".log");
										}
										continue;
									}
									else
									{
										batchMonitorDirect.BatchText = array;
										lstBatchBase.Add(batchMonitorDirect);
									}
									break;
							}
							break;
						case "RUN CFG":
							BatchCFG batchCFG = new BatchCFG()
							{
								ActiveSetup = batchBase.ActiveSetup,
							};
							for (int i = 1; i < array.Count; i++)
							{
								if (array[i].ToString().Contains("-CFGFilename"))
								{
									batchCFG.CFGFilename = array[i].ToString().Replace("-CFGFilename", "").Replace("\"", "").Trim();
									foreach (KeyValuePair<string, string> k in dicVariables)
									{
										if (batchCFG.CFGFilename.Contains(k.Key))
											batchCFG.CFGFilename = batchCFG.CFGFilename.Replace(k.Key, k.Value).Trim();
									}
								}
								else if (array[i].ToString().Contains("-ResultsFilename"))
								{
									batchCFG.ResultsFilename = array[i].ToString().Replace("-ResultsFilename", "").Replace("\"", "").Trim();
									foreach (KeyValuePair<string, string> k in dicVariables)
									{
										if (batchCFG.ResultsFilename.Contains(k.Key))
											batchCFG.ResultsFilename = batchCFG.ResultsFilename.Replace(k.Key, k.Value).Trim();
									}
								}
								else if (array[i].ToString().Contains("-BaselineAQG"))
								{
									batchCFG.BaselineAQG = array[i].ToString().Replace("-BaselineAQG", "").Replace("\"", "").Trim();
									foreach (KeyValuePair<string, string> k in dicVariables)
									{
										if (batchCFG.BaselineAQG.Contains(k.Key))
											batchCFG.BaselineAQG = batchCFG.BaselineAQG.Replace(k.Key, k.Value).Trim();
									}
								}
								else if (array[i].ToString().Contains("-ControlAQG"))
								{
									batchCFG.ControlAQG = array[i].ToString().Replace("-ControlAQG", "").Replace("\"", "").Trim();
									foreach (KeyValuePair<string, string> k in dicVariables)
									{
										if (batchCFG.ControlAQG.Contains(k.Key))
											batchCFG.ControlAQG = batchCFG.ControlAQG.Replace(k.Key, k.Value).Trim();
									}
								}
								else if (array[i].ToString().Contains("-Year"))
								{
									try
									{
										batchCFG.Year = Convert.ToInt32(array[i].ToString().Replace("-Year", "").Replace("\"", "").Trim());
									}
									catch
									{
										checkLog = true;
										WriteBatchLogFile("Error CFG (Year):", strFile + ".log");
										WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
										for (int j = 0; j < array.Count; j++)
										{
											WriteBatchLogFile("            " + array[j], strFile + ".log");
										}
										continue;
									}
								}
								else if (array[i].ToString().Contains("-LatinHypercubePoints"))
								{
									try
									{
										batchCFG.LatinHypercubePoints = Convert.ToInt32(array[i].ToString().Replace("-LatinHypercubePoints", "").Replace("\"", "").Trim());
									}
									catch
									{
										checkLog = true;
										WriteBatchLogFile("Error CFG (LatinHypercubePoints):", strFile + ".log");
										WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
										for (int j = 0; j < array.Count; j++)
										{
											WriteBatchLogFile("            " + array[j], strFile + ".log");
										}
										continue;
									}
								}
								else if (array[i].ToString().Contains("-Threshold"))
								{
									try
									{
										batchCFG.Threshold = Convert.ToDouble(array[i].ToString().Replace("-Threshold", "").Replace("\"", "").Trim());
									}
									catch
									{
										checkLog = true;
										WriteBatchLogFile("Error CFG (Threshold):", strFile + ".log");
										WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
										for (int j = 0; j < array.Count; j++)
										{
											WriteBatchLogFile("            " + array[j], strFile + ".log");
										}
										continue;
									}
								}
								else if (array[i].ToString().Contains("-Seeds"))
								{
									try
									{
										batchCFG.Seeds = Convert.ToInt32(array[i].ToString().Replace("-Seeds", "").Replace("\"", "").Trim());
									}
									catch
									{
										checkLog = true;
										WriteBatchLogFile("Error CFG (Seed):", strFile + ".log");
										WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
										for (int j = 0; j < array.Count; j++)
										{
											WriteBatchLogFile("            " + array[j], strFile + ".log");
										}
										continue;
									}
								}
							}
							if (!CheckBatch(batchCFG, out msgOut))
							{
								checkLog = true;
								WriteBatchLogFile("Error (Loading CFG Command): " + msgOut, strFile + ".log");
								WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
								for (int i = 0; i < array.Count; i++)
								{
									WriteBatchLogFile("            " + array[i], strFile + ".log");
								}
								continue;
							}
							else
							{
								batchCFG.BatchText = array;
								lstBatchBase.Add(batchCFG);
							}

							break;
						case "RUN APV":
							BatchAPV batchAPV = new BatchAPV()
							{
								ActiveSetup = batchBase.ActiveSetup,
							};
							for (int i = 1; i < array.Count; i++)
							{
								if (array[i].ToString().Contains("-APVFilename"))
								{
									batchAPV.APVFilename = array[i].ToString().Replace("-APVFilename", "").Replace("\"", "").Trim();
									foreach (KeyValuePair<string, string> k in dicVariables)
									{
										if (batchAPV.APVFilename.Contains(k.Key))
											batchAPV.APVFilename = batchAPV.APVFilename.Replace(k.Key, k.Value).Trim();
									}
								}
								else if (array[i].ToString().Contains("-ResultsFilename"))
								{
									batchAPV.ResultsFilename = array[i].ToString().Replace("-ResultsFilename", "").Replace("\"", "").Trim();
									foreach (KeyValuePair<string, string> k in dicVariables)
									{
										if (batchAPV.ResultsFilename.Contains(k.Key))
											batchAPV.ResultsFilename = batchAPV.ResultsFilename.Replace(k.Key, k.Value).Trim();
									}
								}
								else if (array[i].ToString().Contains("-CFGRFilename"))
								{
									batchAPV.CFGRFilename = array[i].ToString().Replace("-CFGRFilename", "").Replace("\"", "").Trim();
									foreach (KeyValuePair<string, string> k in dicVariables)
									{
										if (batchAPV.CFGRFilename.Contains(k.Key))
											batchAPV.CFGRFilename = batchAPV.CFGRFilename.Replace(k.Key, k.Value).Trim();
									}
								}
								else if (array[i].ToString().Contains("-IncidenceAggregation"))
								{
									batchAPV.IncidenceAggregation = array[i].ToString().Replace("-IncidenceAggregation", "").Replace("\"", "").Trim();
								}
								else if (array[i].ToString().Contains("-ValuationAggregation"))
								{
									batchAPV.ValuationAggregation = array[i].ToString().Replace("-ValuationAggregation", "").Replace("\"", "").Trim();
								}
								else if (array[i].ToString().Contains("-RandomSeed"))
								{
									try
									{
										batchAPV.RandomSeed = Convert.ToInt32(array[i].ToString().Replace("-RandomSeed", "").Replace("\"", "").Trim());
									}
									catch
									{
										checkLog = true;
										WriteBatchLogFile("Error APV (RandomSeed):", strFile + ".log");
										WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
										for (int j = 0; j < array.Count; j++)
										{
											WriteBatchLogFile("            " + array[j], strFile + ".log");
										}
										continue;
									}
								}
								else if (array[i].ToString().Contains("-DollarYear"))
								{
									try
									{
										batchAPV.DollarYear = array[i].ToString().Replace("-DollarYear", "").Replace("\"", "").Trim();
									}
									catch
									{
										checkLog = true;
										WriteBatchLogFile("Error APV (DollarYear):", strFile + ".log");
										WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
										for (int j = 0; j < array.Count; j++)
										{
											WriteBatchLogFile("            " + array[j], strFile + ".log");
										}
										continue;
									}
								}
							}
							if (!CheckBatch(batchAPV, out msgOut))
							{
								checkLog = true;
								WriteBatchLogFile("Error (Loading APV Command): " + msgOut, strFile + ".log");
								WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
								for (int i = 0; i < array.Count; i++)
								{
									WriteBatchLogFile("            " + array[i], strFile + ".log");
								}
								continue;
							}
							else
							{
								batchAPV.BatchText = array;
								lstBatchBase.Add(batchAPV);
							}

							break;
						case "GENERATE REPORT AuditTrail":
							BatchReportAuditTrail batchReportAuditTrail = new BatchReportAuditTrail();
							for (int i = 1; i < array.Count; i++)
							{

								if (array[i].ToString().Contains("-InputFile"))
								{
									batchReportAuditTrail.InputFile = array[i].ToString().Replace("-InputFile", "").Replace("\"", "").Trim();
									foreach (KeyValuePair<string, string> k in dicVariables)
									{
										if (batchReportAuditTrail.InputFile.Contains(k.Key))
											batchReportAuditTrail.InputFile = batchReportAuditTrail.InputFile.Replace(k.Key, k.Value).Trim();
									}
								}
								else if (array[i].ToString().Contains("-ReportFile"))
								{
									batchReportAuditTrail.ReportFile = array[i].ToString().Replace("-ReportFile", "").Replace("\"", "").Trim();
									foreach (KeyValuePair<string, string> k in dicVariables)
									{
										if (batchReportAuditTrail.ReportFile.Contains(k.Key))
											batchReportAuditTrail.ReportFile = batchReportAuditTrail.ReportFile.Replace(k.Key, k.Value).Trim();
									}
								}
							}
							batchReportAuditTrail.ActiveSetup = batchBase.ActiveSetup;
							if (!CheckBatch(batchReportAuditTrail, out msgOut))
							{
								checkLog = true;
								WriteBatchLogFile("Error (Loading Audit Trail Command): " + msgOut, strFile + ".log");
								WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
								for (int j = 1; j < array.Count; j++)
								{
									WriteBatchLogFile("            " + array[j], strFile + ".log");
								}
								continue;
							}
							else
							{
								batchReportAuditTrail.BatchText = array;
								lstBatchBase.Add(batchReportAuditTrail);
							}

							break;
						case "GENERATE REPORT CFGR":
							BatchReportCFGR batchReportCFGR = new BatchReportCFGR();
							for (int i = 1; i < array.Count; i++)
							{

								if (array[i].ToString().Contains("-InputFile"))
								{
									batchReportCFGR.InputFile = array[i].ToString().Replace("-InputFile", "").Replace("\"", "").Trim();
									foreach (KeyValuePair<string, string> k in dicVariables)
									{
										if (batchReportCFGR.InputFile.Contains(k.Key))
											batchReportCFGR.InputFile = batchReportCFGR.InputFile.Replace(k.Key, k.Value).Trim();
									}
								}
								else if (array[i].ToString().Contains("-ReportFile"))
								{
									batchReportCFGR.ReportFile = array[i].ToString().Replace("-ReportFile", "").Replace("\"", "").Trim();
									foreach (KeyValuePair<string, string> k in dicVariables)
									{
										if (batchReportCFGR.ReportFile.Contains(k.Key))
											batchReportCFGR.ReportFile = batchReportCFGR.ReportFile.Replace(k.Key, k.Value).Trim();
									}
								}
								else if (array[i].ToString().Contains("-GridFields"))
								{
									batchReportCFGR.GridFields = array[i].ToString().Replace("-GridFields", "").Replace("\"", "").Trim();
								}
								else if (array[i].ToString().Contains("-CustomFields"))
								{
									batchReportCFGR.CustomFields = array[i].ToString().Replace("-CustomFields", "").Replace("\"", "").Trim();
								}
								else if (array[i].ToString().Contains("-ResultFields"))
								{
									batchReportCFGR.ResultFields = array[i].ToString().Replace("-ResultFields", "").Replace("\"", "").Trim();
								}
								else if (array[i].ToString().Contains("-Grouping"))
								{
									batchReportCFGR.Grouping = array[i].ToString().Replace("-Grouping", "").Replace("\"", "").Trim();
								}
								else if (array[i].ToString().Contains("-DecimalDigits"))
								{
									batchReportCFGR.DecimalDigits = array[i].ToString().Replace("-DecimalDigits", "").Replace("\"", "").Trim();
								}
							}
							batchReportCFGR.ActiveSetup = batchBase.ActiveSetup;
							if (!CheckBatch(batchReportCFGR, out msgOut))
							{
								checkLog = true;
								WriteBatchLogFile("Error (Loading CFGR Report Command): " + msgOut, strFile + ".log");
								WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
								for (int j = 1; j < array.Count; j++)
								{
									WriteBatchLogFile("            " + array[j], strFile + ".log");
								}
								continue;
							}
							else
							{
								batchReportCFGR.BatchText = array;
								lstBatchBase.Add(batchReportCFGR);
							}

							break;
						case "GENERATE REPORT APVR":

							BatchReportAPVR batchReportAPVR = new BatchReportAPVR();
							for (int i = 1; i < array.Count; i++)
							{


								if (array[i].ToString().Contains("-InputFile"))
								{
									batchReportAPVR.InputFile = array[i].ToString().Replace("-InputFile", "").Replace("\"", "").Trim();
									foreach (KeyValuePair<string, string> k in dicVariables)
									{
										if (batchReportAPVR.InputFile.Contains(k.Key))
											batchReportAPVR.InputFile = batchReportAPVR.InputFile.Replace(k.Key, k.Value).Trim();
									}
								}
								else if (array[i].ToString().Contains("-ReportFile"))
								{
									batchReportAPVR.ReportFile = array[i].ToString().Replace("-ReportFile", "").Replace("\"", "").Trim();
									foreach (KeyValuePair<string, string> k in dicVariables)
									{
										if (batchReportAPVR.ReportFile.Contains(k.Key))
											batchReportAPVR.ReportFile = batchReportAPVR.ReportFile.Replace(k.Key, k.Value).Trim();
									}
								}
								else if (array[i].ToString().Contains("-GridFields"))
								{
									batchReportAPVR.GridFields = array[i].ToString().Replace("-GridFields", "").Replace("\"", "").Trim();
								}
								else if (array[i].ToString().Contains("-CustomFields"))
								{
									batchReportAPVR.CustomFields = array[i].ToString().Replace("-CustomFields", "").Replace("\"", "").Trim();
								}
								else if (array[i].ToString().Contains("-ResultFields"))
								{
									batchReportAPVR.ResultFields = array[i].ToString().Replace("-ResultFields", "").Replace("\"", "").Trim();
								}
								else if (array[i].ToString().Contains("-Totals"))
								{
									batchReportAPVR.Totals = array[i].ToString().Replace("-Totals", "").Replace("\"", "").Trim();
								}
								else if (array[i].ToString().Contains("-ResultType"))
								{
									batchReportAPVR.ResultType = array[i].ToString().Replace("-ResultType", "").Replace("\"", "").Trim();
								}
							}
							batchReportAPVR.ActiveSetup = batchBase.ActiveSetup;
							if (!CheckBatch(batchReportAPVR, out msgOut))
							{
								checkLog = true;
								WriteBatchLogFile("Error (Loading APVR Report Command): " + msgOut, strFile + ".log");
								WriteBatchLogFile("Occurred: " + dateTime, strFile + ".log");
								for (int j = 1; j < array.Count; j++)
								{
									WriteBatchLogFile("            " + array[j], strFile + ".log");
								}
								continue;
							}
							else
							{
								batchReportAPVR.BatchText = array;
								lstBatchBase.Add(batchReportAPVR);
							}

							break;


					}



				}
				return lstBatchBase;
			}
			catch
			{
				return null;
			}
		}
		public static bool CheckBatch(BatchBase batchBase, out string msgOut)
		{
			try
			{
				if (getSetupFromName(batchBase.ActiveSetup) == null)
				{
					msgOut = String.Format("Unable to get setup: {0}", batchBase.ActiveSetup);
					return false;
				}
				if (batchBase is BatchAQGBase)
				{
					if (batchBase is BatchModelDirect)
					{
						BatchModelDirect batchModelDirect = (batchBase as BatchModelDirect);
						if (!File.Exists(batchModelDirect.ModelFilename))
						{
							msgOut = String.Format("Unable to locate model file: {0}", batchModelDirect.ModelFilename);
							return false;
						}
						if (getPollutantFromName(batchModelDirect.Pollutant) == null)
						{
							msgOut = String.Format("Unable to get pollutant from setup: {0}", batchModelDirect.Pollutant);
							return false;
						}
						if (getGridFromName(batchModelDirect.GridType) == null)
						{
							msgOut = String.Format("Unable to get grid from setup: {0}", batchModelDirect.GridType);
							return false;
						}
					}
					else if (batchBase is BatchMonitorDirect)
					{
						BatchMonitorDirect batchMonitorDirect = batchBase as BatchMonitorDirect;
						if (getPollutantFromName(batchMonitorDirect.Pollutant) == null)
						{
							msgOut = String.Format("Unable to get pollutant from setup: {0}", batchMonitorDirect.Pollutant);
							return false;
						}
						if (getGridFromName(batchMonitorDirect.GridType) == null)
						{
							msgOut = String.Format("Unable to get grid from setup: {0}", batchMonitorDirect.GridType);
							return false;
						}
						if (batchMonitorDirect.MonitorDataType == "Library")
						{
							if (batchMonitorDirect.MonitorYear < 1980 || batchMonitorDirect.MonitorYear > 2300)
							{
								msgOut = String.Format("Monitor year {0} is outside the allowable range of 1980 - 2300, inclusive.", batchMonitorDirect.MonitorYear);
								return false;
							}
						}
						else if (batchMonitorDirect.MonitorDataType == "TextFile")
						{
							if (!File.Exists(batchMonitorDirect.MonitorFile))
							{
								msgOut = String.Format("Unable to locate monitor file: {0}", batchMonitorDirect.MonitorFile);
								return false;
							}
						}
						else
						{
							msgOut = String.Format("Unable to recognize monitor type: {0}. 'Library' and 'TextFile' are the supported values", batchMonitorDirect.MonitorDataType);
							return false;
						}
					}
					else
					{
						msgOut = String.Format("Unknown AQ batch type");
						return false;
					}
				}
				else if (batchBase is BatchCFG)
				{
					BatchCFG batchCFG = batchBase as BatchCFG;
					if (!File.Exists(batchCFG.CFGFilename))
					{
						msgOut = String.Format("Unable to locate CFG file: {0}", batchCFG.CFGFilename);
						return false; 
					}

					if (batchCFG.LatinHypercubePoints != -1 && batchCFG.LatinHypercubePoints != 0 && batchCFG.LatinHypercubePoints != 10 && batchCFG.LatinHypercubePoints != 20 && batchCFG.LatinHypercubePoints != 50 && batchCFG.LatinHypercubePoints != 100)
					{
						msgOut = String.Format("Unexpected Latin Hypercube Points value: {0}. Supported values are -1, 0, 10, 20, 50, 100.", batchCFG.LatinHypercubePoints);
						return false; //Added option of 50 to match percentiles options listed in GUI ("LatinHypercubePoints.cs")--MP 07 Jan 2020
					}
					if ((batchCFG.Year != -1) && (batchCFG.Year < 1980 || batchCFG.Year > 2300))
					{
						msgOut = String.Format("CFG year {0} is must be -1 or in the range of 1980 - 2300, inclusive.", batchCFG.Year);
						return false;
					}
				}
				else if (batchBase is BatchAPV)
				{
					BatchAPV batchAPV = batchBase as BatchAPV;
					if (!File.Exists(batchAPV.APVFilename))
					{
						msgOut = String.Format("Unable to locate APV file: {0}", batchAPV.APVFilename);
						return false;
					}
					if (batchAPV.CFGRFilename == null || batchAPV.CFGRFilename.Trim() == "")
					{
						msgOut = String.Format("CFGR Filename is missing.");
						return false;
					}
				}
				else if (batchBase is BatchReport)
				{
					BatchReport batchReport = batchBase as BatchReport;
					if (batchReport.InputFile == null || batchReport.InputFile.Trim() == "")
					{
						msgOut = String.Format("Report InputFile is missing.");
						return false;
					}
				}
			}
			catch(Exception e)
			{
				msgOut = String.Format("An exception occurred while validating the batch file: {0}.\n\n{1}", e.Message, e.StackTrace);
				return false;
			}
			msgOut = "";
			return true;
		}
	}
}
