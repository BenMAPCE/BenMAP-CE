using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESIL.DBUtility;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Configuration;
using FirebirdSql.Data.FirebirdClient;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Meta.Numerics;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using ProtoBuf;
using System.Reflection;
using BenMAP.Crosswalks;

namespace BenMAP.Configuration
{
	public class ConfigurationCommonClass
	{

		public const string GEOGRAPHIC_AREA_EVERYWHERE = "Everywhere";
		public const string GEOGRAPHIC_AREA_ELSEWHERE = "Elsewhere";

		public enum geographicAreaAnalysisMode
		{
			allUnconstrained = 1,
			allConstrained = 2,
			mixedConstraints = 3
		}

		public enum incidenceAveraging  // incidence averaging choices
		{
			averageAll = 0, // use the average incidence rate across all races/ethnicities/genders
			averageFiltered = 1, // filter the incidence rate to match the one(s) selected on the health impact form (HealthImpactFunctions.cs)
		}


		// global variable to hold user selection of averaging type
		public static incidenceAveraging incidenceAvgSelected = incidenceAveraging.averageAll;

		public static void ClearCRSelectFunctionCalculateValueLHS(ref CRSelectFunctionCalculateValue cRSelectFunctionCalculateValue)
		{

		}
		public static void SaveCRFRFile(BaseControlCRSelectFunctionCalculateValue baseControlCRSelectFunctionCalculateValue, string strCRFPath)
		{
			try
			{
				if (File.Exists(strCRFPath))
					File.Delete(strCRFPath);

				using (FileStream fs = new FileStream(strCRFPath, FileMode.OpenOrCreate))
				{
					try
					{
						baseControlCRSelectFunctionCalculateValue.Version = "BenMAP-CE " + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);
						Serializer.Serialize<BaseControlCRSelectFunctionCalculateValue>(fs, baseControlCRSelectFunctionCalculateValue);

						fs.Dispose();
					}
					catch (Exception ex)
					{
						fs.Close();
						fs.Dispose();
					}
				}
				return;
				BaseControlCRSelectFunctionCalculateValue copy = new BaseControlCRSelectFunctionCalculateValue();
				copy.BaseControlGroup = new List<BaseControlGroup>();
				foreach (BaseControlGroup bcg in baseControlCRSelectFunctionCalculateValue.BaseControlGroup)
				{
					BaseControlGroup bcgcopy = new BaseControlGroup();
					bcgcopy.GridType = bcg.GridType;
					bcgcopy.Pollutant = bcg.Pollutant;
					bcgcopy.DeltaQ = bcg.DeltaQ;
					bcgcopy.Base = DataSourceCommonClass.getBenMapLineCopyOnlyResultCopy(bcg.Base);
					bcgcopy.Control = DataSourceCommonClass.getBenMapLineCopyOnlyResultCopy(bcg.Control);
					copy.BaseControlGroup.Add(bcgcopy);
				}
				copy.BenMAPPopulation = baseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
				copy.CRLatinHypercubePoints = baseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints;
				copy.CRDefaultMonteCarloIterations = baseControlCRSelectFunctionCalculateValue.CRDefaultMonteCarloIterations;
				copy.CRRunInPointMode = baseControlCRSelectFunctionCalculateValue.CRRunInPointMode;
				copy.CRThreshold = baseControlCRSelectFunctionCalculateValue.CRThreshold;
				copy.RBenMapGrid = baseControlCRSelectFunctionCalculateValue.RBenMapGrid;
				copy.incidenceAvgSelected = baseControlCRSelectFunctionCalculateValue.incidenceAvgSelected; //YY: 539
				copy.lstCRSelectFunctionCalculateValue = new List<CRSelectFunctionCalculateValue>();
				List<float> lstd = new List<float>();
				foreach (CRSelectFunctionCalculateValue crr in baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
				{
					CRSelectFunctionCalculateValue crrcopy = new CRSelectFunctionCalculateValue();
					crrcopy.CRSelectFunction = crr.CRSelectFunction;
					copy.lstCRSelectFunctionCalculateValue.Add(crrcopy);
				}

				GC.Collect();
				if (File.Exists(strCRFPath))
					File.Delete(strCRFPath);



				using (FileStream fs = new FileStream(strCRFPath, FileMode.OpenOrCreate))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					try
					{
						formatter.Serialize(fs, copy);
					}
					catch (Exception ex)
					{
					}
					fs.Close();
					fs.Dispose();
					copy = null;
					formatter = null;
					GC.Collect();
				}


				GC.Collect();

			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}
		public static BaseControlCRSelectFunctionCalculateValue LoadCFGRFile(string strCFGRPath, ref string err)
		{

			using (FileStream fs = new FileStream(strCFGRPath, FileMode.Open))
			{
				try
				{
					BaseControlCRSelectFunctionCalculateValue baseControlCRSelectFunctionCalculateValue = Serializer.Deserialize<BaseControlCRSelectFunctionCalculateValue>(fs);

					// For backward compatability, assume "everywhere" if we don't have an area name set
					foreach (CRSelectFunctionCalculateValue c in baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
					{
						if (string.IsNullOrEmpty(c.CRSelectFunction.GeographicAreaName))
						{
							c.CRSelectFunction.GeographicAreaName = GEOGRAPHIC_AREA_EVERYWHERE;
						}
					}

					BenMAPSetup benMAPSetup = null;
					if (baseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType != null)
					{
						benMAPSetup = CommonClass.getBenMAPSetupFromName(baseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType.SetupName);
					}
					if (benMAPSetup == null)
					{
						err = "The setup name \"" + baseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType.SetupName + "\" can't be found in the database.";
						return null;
					}
					baseControlCRSelectFunctionCalculateValue.Setup = benMAPSetup;

					BenMAPGrid benMAPGrid = Grid.GridCommon.getBenMAPGridFromName(baseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType.GridDefinitionName, benMAPSetup);
					if (benMAPGrid == null)
					{
						err = "The grid definition name \"" + baseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType.GridDefinitionName + "\" can't be found in the setup \"" + benMAPSetup.SetupName + "\".";
						return null;
					}

					foreach (BaseControlGroup bcg in baseControlCRSelectFunctionCalculateValue.BaseControlGroup)
					{
						bcg.GridType = benMAPGrid;
						BenMAPPollutant pollutant = Grid.GridCommon.getPollutantFromName(bcg.Pollutant.PollutantName, benMAPSetup.SetupID);
						if (pollutant == null)
						{
							err = "The pollutant name \"" + bcg.Pollutant.PollutantName + "\" can't be found in the setup \"" + benMAPSetup.SetupName + "\".";
							return null;
						}
						bcg.Pollutant = pollutant;
					}

					BenMAPPopulation population = getPopulationFromName(baseControlCRSelectFunctionCalculateValue.BenMAPPopulation.DataSetName, benMAPSetup.SetupID, baseControlCRSelectFunctionCalculateValue.BenMAPPopulation.Year);
					if (population == null)
					{
						err = "The population name \"" + baseControlCRSelectFunctionCalculateValue.BenMAPPopulation.DataSetName + "\" can't be found in the setup \"" + benMAPSetup.SetupName + "\".";
						return null;
					}
					baseControlCRSelectFunctionCalculateValue.BenMAPPopulation = population;

					foreach (CRSelectFunctionCalculateValue fn in baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
					{
						if (fn.CRSelectFunction.IncidenceDataSetID != -1) //HIF contains incidence dataset info
						{
							int incidenceID = getIncidenceDatasetID(baseControlCRSelectFunctionCalculateValue.Setup.SetupID, fn.CRSelectFunction.IncidenceDataSetName);

							if (incidenceID == 0)   //If no ID found, notify user and exit
							{
								MessageBox.Show(String.Format("No database entry for Setup ID: {0}, Incidence Dataset: {1} in {2} (ID: {3})", baseControlCRSelectFunctionCalculateValue.Setup.SetupID, fn.CRSelectFunction.IncidenceDataSetName, fn.CRSelectFunction.BenMAPHealthImpactFunction.Author, fn.CRSelectFunction.BenMAPHealthImpactFunction.ID));
								baseControlCRSelectFunctionCalculateValue = null;
								break;
							}

							if (incidenceID != fn.CRSelectFunction.IncidenceDataSetID)   //If dataset ID doesn't match ID in HIF, change HIF to dataset value and notify user of change.
							{
								fn.CRSelectFunction.IncidenceDataSetID = incidenceID;
								MessageBox.Show(String.Format("Updated the Incidence Dataset ID for Health Impact Function ({0}--ID: {1}) to match the entry in the database.", fn.CRSelectFunction.BenMAPHealthImpactFunction.Author, fn.CRSelectFunction.BenMAPHealthImpactFunction.ID));
							}
						}

						if (fn.CRSelectFunction.PrevalenceDataSetID != -1) //HIF contains prevalence dataset info
						{
							int prevalenceID = getIncidenceDatasetID(baseControlCRSelectFunctionCalculateValue.Setup.SetupID, fn.CRSelectFunction.PrevalenceDataSetName);

							if (prevalenceID == 0)   //If no ID found, notify user and exit
							{
								MessageBox.Show(String.Format("No database entry for Setup ID: {0}, Prevalence Dataset: {1} in {2} (ID: {3})", baseControlCRSelectFunctionCalculateValue.Setup.SetupID, fn.CRSelectFunction.PrevalenceDataSetName, fn.CRSelectFunction.BenMAPHealthImpactFunction.Author, fn.CRSelectFunction.BenMAPHealthImpactFunction.ID));
								baseControlCRSelectFunctionCalculateValue = null;
								break;
							}

							if (prevalenceID != fn.CRSelectFunction.PrevalenceDataSetID)   //If dataset ID doesn't match ID in HIF, change HIF to dataset value and notify user of change.
							{
								fn.CRSelectFunction.PrevalenceDataSetID = prevalenceID;
								MessageBox.Show(String.Format("Updated the Prevalence Dataset ID for Health Impact Function ({0}--ID: {1}) to match the entry in the database.", fn.CRSelectFunction.BenMAPHealthImpactFunction.Author, fn.CRSelectFunction.BenMAPHealthImpactFunction.ID));
							}
						}
					}

					fs.Close();
					fs.Dispose();
					baseControlCRSelectFunctionCalculateValue.RBenMapGrid = null;
					return baseControlCRSelectFunctionCalculateValue;
				}
				catch (Exception ex)
				{
					fs.Close();
					fs.Dispose();
					err = "BenMAP-CE was unable to open the file. The file may be corrupt, or it may have been created using a previous incompatible version of BenMAP-CE.";
					return null;
				}
			}

			try
			{
				BaseControlCRSelectFunctionCalculateValue baseControlCRSelectFunctionCalculateValue = null; try
				{
					using (FileStream fs = new FileStream(strCFGRPath, FileMode.Open))
					{
						BinaryFormatter formatter = new BinaryFormatter();
						baseControlCRSelectFunctionCalculateValue = (BaseControlCRSelectFunctionCalculateValue)formatter.Deserialize(fs); fs.Close();
						fs.Dispose();
						formatter = null;
					}

					foreach (BaseControlGroup bcg in baseControlCRSelectFunctionCalculateValue.BaseControlGroup)
					{
						if (bcg.Base != null)
						{
							DataSourceCommonClass.getModelValuesFromResultCopy(ref bcg.Base);
							bcg.Base.ShapeFile = null;
						}
						if (bcg.Control != null)
						{
							DataSourceCommonClass.getModelValuesFromResultCopy(ref bcg.Control);
							bcg.Control.ShapeFile = null;
						}
					}
				}
				catch (Exception ex)
				{
				}
				for (int i = 0; i < baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; i++)
				{
					CRSelectFunctionCalculateValue crclv = baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[i];
					getCalculateValueFromResultCopy(ref crclv);
					baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[i] = crclv;

				}
				GC.Collect();


				return baseControlCRSelectFunctionCalculateValue;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);

				return null;
			}

		}
		public static void SaveCFGFile(BaseControlCRSelectFunction baseControlCRSelectFunction, string strFile)
		{
			try
			{
				if (File.Exists(strFile))
					File.Delete(strFile);
				using (FileStream fs = new FileStream(strFile, FileMode.OpenOrCreate))
				{
					baseControlCRSelectFunction.Version = "BenMAP-CE " + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);
					Serializer.Serialize<BaseControlCRSelectFunction>(fs, baseControlCRSelectFunction);

					fs.Close();
					fs.Dispose();
				}
				GC.Collect();
				return;
				BaseControlCRSelectFunction copy = new BaseControlCRSelectFunction();
				copy.BaseControlGroup = new List<BaseControlGroup>();
				foreach (BaseControlGroup bcg in baseControlCRSelectFunction.BaseControlGroup)
				{
					BaseControlGroup bcgcopy = new BaseControlGroup();
					bcgcopy.GridType = bcg.GridType;
					bcgcopy.Pollutant = bcg.Pollutant;
					bcgcopy.DeltaQ = bcg.DeltaQ;
					bcgcopy.Base = DataSourceCommonClass.getBenMapLineCopyOnlyResultCopy(bcg.Base);
					bcgcopy.Control = DataSourceCommonClass.getBenMapLineCopyOnlyResultCopy(bcg.Control);
					copy.BaseControlGroup.Add(bcgcopy);
				}
				copy.BenMAPPopulation = baseControlCRSelectFunction.BenMAPPopulation;
				copy.CRLatinHypercubePoints = baseControlCRSelectFunction.CRLatinHypercubePoints;
				copy.CRDefaultMonteCarloIterations = baseControlCRSelectFunction.CRDefaultMonteCarloIterations;
				copy.CRRunInPointMode = baseControlCRSelectFunction.CRRunInPointMode;
				copy.CRThreshold = baseControlCRSelectFunction.CRThreshold;
				copy.RBenMapGrid = baseControlCRSelectFunction.RBenMapGrid;
				copy.lstCRSelectFunction = baseControlCRSelectFunction.lstCRSelectFunction;
				copy.incidenceAvgSelected = baseControlCRSelectFunction.incidenceAvgSelected; //YY: 539

				if (File.Exists(strFile))
					File.Delete(strFile);
				using (FileStream fs = new FileStream(strFile, FileMode.OpenOrCreate))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					formatter.Serialize(fs, copy);
					fs.Close();
					fs.Dispose();
					copy = null;
					formatter = null;
				}
				GC.Collect();


			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}
		}

		public static BenMAPPopulation getPopulationFromName(string PopulationName, int SetupID, int year)
		{
			try
			{
				BenMAPPopulation Population = new BenMAPPopulation();
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				string commandText = string.Format("select PopulationdatasetID,PopulationconfigurationID,GriddefinitionID from Populationdatasets where populationdatasetname ='{0}' and SetupID={1}", PopulationName, SetupID);
				System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				DataRow dr = ds.Tables[0].Rows[0];
				Population.DataSetID = Convert.ToInt32(dr["PopulationdatasetID"].ToString());
				Population.DataSetName = PopulationName;
				Population.Year = year;
				Population.PopulationConfiguration = Convert.ToInt32(dr["PopulationconfigurationID"].ToString());
				Population.GridType = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(dr["GriddefinitionID"].ToString()));
				return Population;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				return null;
			}
		}

		public static BaseControlCRSelectFunction loadCFGFile(string strFile, ref string err)
		{
			bool isBatch = false;

			if (CommonClass.InputParams != null && CommonClass.InputParams.Count() > 0 && CommonClass.InputParams[0].ToLower().Contains(".ctlx"))
			{
				isBatch = true;
			}

			BaseControlCRSelectFunction baseControlCRSelectFunction = null;
			using (FileStream fs = new FileStream(strFile, FileMode.Open))
			{
				try
				{
					baseControlCRSelectFunction = Serializer.Deserialize<BaseControlCRSelectFunction>(fs);

					// For backward compatability, assume "everywhere" if we don't have an area name set
					foreach (CRSelectFunction c in baseControlCRSelectFunction.lstCRSelectFunction)
					{
						if (string.IsNullOrEmpty(c.GeographicAreaName))
						{
							c.GeographicAreaName = GEOGRAPHIC_AREA_EVERYWHERE;
						}
					}


					BenMAPSetup benMAPSetup = null;
					if (baseControlCRSelectFunction.BaseControlGroup[0].GridType != null)
					{
						benMAPSetup = CommonClass.getBenMAPSetupFromName(baseControlCRSelectFunction.BaseControlGroup[0].GridType.SetupName);
					}
					if (benMAPSetup == null)
					{
						err = "The setup name \"" + baseControlCRSelectFunction.BaseControlGroup[0].GridType.SetupName + "\" can't be found in the database.";
						return null;
					}
					baseControlCRSelectFunction.Setup = benMAPSetup;

					BenMAPGrid benMAPGrid = Grid.GridCommon.getBenMAPGridFromName(baseControlCRSelectFunction.BaseControlGroup[0].GridType.GridDefinitionName, benMAPSetup);
					if (benMAPGrid == null)
					{
						err = "The grid definition name \"" + baseControlCRSelectFunction.BaseControlGroup[0].GridType.GridDefinitionName + "\" can't be found in the setup \"" + benMAPSetup.SetupName + "\".";
						return null;
					}

					foreach (BaseControlGroup bcg in baseControlCRSelectFunction.BaseControlGroup)
					{
						bcg.GridType = benMAPGrid;
						BenMAPPollutant pollutant = Grid.GridCommon.getPollutantFromName(bcg.Pollutant.PollutantName, benMAPSetup.SetupID);
						if (pollutant == null)
						{
							err = "The pollutant name \"" + bcg.Pollutant.PollutantName + "\" can't be found in the setup \"" + benMAPSetup.SetupName + "\".";
							return null;
						}
						bcg.Pollutant = pollutant;
					}

					BenMAPPopulation population = getPopulationFromName(baseControlCRSelectFunction.BenMAPPopulation.DataSetName, benMAPSetup.SetupID, baseControlCRSelectFunction.BenMAPPopulation.Year);
					if (population == null)
					{
						err = "The population name \"" + baseControlCRSelectFunction.BenMAPPopulation.DataSetName + "\" can't be found in the setup \"" + benMAPSetup.SetupName + "\".";
						return null;
					}
					baseControlCRSelectFunction.BenMAPPopulation = population;

					foreach (CRSelectFunction fn in baseControlCRSelectFunction.lstCRSelectFunction)
					{
						if (fn.IncidenceDataSetID != -1) //HIF contains incidence dataset info
						{
							int incidenceID = getIncidenceDatasetID(baseControlCRSelectFunction.Setup.SetupID, fn.IncidenceDataSetName);

							if (incidenceID == 0)   //If no ID found, notify user and exit
							{
								MessageBox.Show(String.Format("No database entry for Setup ID: {0}, Incidence Dataset: {1} in {2} (ID: {3})", baseControlCRSelectFunction.Setup.SetupID, fn.IncidenceDataSetName, fn.BenMAPHealthImpactFunction.Author, fn.BenMAPHealthImpactFunction.ID));
								baseControlCRSelectFunction = null;
								break;
							}

							if (incidenceID != fn.IncidenceDataSetID)   //If dataset ID doesn't match ID in HIF, change HIF to dataset value and notify user of change.
							{
								fn.IncidenceDataSetID = incidenceID;
								if (!isBatch)
								{
									MessageBox.Show(String.Format("Updated the Incidence Dataset ID for Health Impact Function ({0}--ID: {1}) to match the entry in the database.", fn.BenMAPHealthImpactFunction.Author, fn.BenMAPHealthImpactFunction.ID));
								}
							}
						}

						if (fn.PrevalenceDataSetID != -1) //HIF contains prevalence dataset info
						{
							int prevalenceID = getIncidenceDatasetID(baseControlCRSelectFunction.Setup.SetupID, fn.PrevalenceDataSetName);

							if (prevalenceID == 0)   //If no ID found, notify user and exit
							{
								MessageBox.Show(String.Format("No database entry for Setup ID: {0}, Prevalence Dataset: {1} in {2} (ID: {3})", baseControlCRSelectFunction.Setup.SetupID, fn.PrevalenceDataSetName, fn.BenMAPHealthImpactFunction.Author, fn.BenMAPHealthImpactFunction.ID));
								baseControlCRSelectFunction = null;
								break;
							}

							if (prevalenceID != fn.PrevalenceDataSetID)   //If dataset ID doesn't match ID in HIF, change HIF to dataset value and notify user of change.
							{
								fn.PrevalenceDataSetID = prevalenceID;
								if (!isBatch)
								{
									MessageBox.Show(String.Format("Updated the Prevalence Dataset ID for Health Impact Function ({0}--ID: {1}) to match the entry in the database.", fn.BenMAPHealthImpactFunction.Author, fn.BenMAPHealthImpactFunction.ID));
								}
							}
						}
					}

					fs.Close();
					fs.Dispose();
					baseControlCRSelectFunction.RBenMapGrid = null;
					return baseControlCRSelectFunction;
				}
				catch (Exception ex)
				{
					fs.Close();
					fs.Dispose();
					err = "BenMAP-CE was unable to open the file. The file may be corrupt, or it may have been created using a previous incompatible version of BenMAP-CE.";
					return null;
				}
			}

			try
			{
				using (FileStream fs = new FileStream(strFile, FileMode.Open))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					baseControlCRSelectFunction = (BaseControlCRSelectFunction)formatter.Deserialize(fs); fs.Close();
					fs.Dispose();
					formatter = null;
					GC.Collect();
				}
				foreach (BaseControlGroup bcg in baseControlCRSelectFunction.BaseControlGroup)
				{
					DataSourceCommonClass.getModelValuesFromResultCopy(ref bcg.Base);
					DataSourceCommonClass.getModelValuesFromResultCopy(ref bcg.Control);
				}


			}
			catch (Exception ex)
			{
			}
			return baseControlCRSelectFunction;
		}

		public static int getIncidenceDatasetID(int setupID, string datasetName) //BenMAP 435--Ensure backward compatibility of datasets (incidence and prevalence) [2020 01 03, MP]
		{
			string commandText = String.Format("select a.INCIDENCEDATASETID from INCIDENCEDATASETS a where a.SETUPID={0} and a.INCIDENCEDATASETNAME='{1}'", setupID, datasetName);
			ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
			int incidenceID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText)); //Find ID of dataset in the HIF

			return incidenceID;
		}

		public static Dictionary<string, int> getAllRace()
		{
			try
			{
				Dictionary<string, int> dicRace = new Dictionary<string, int>();
				string commandText = "select RaceID,RaceName from Races";
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					dicRace.Add(dr["RaceName"].ToString(), Convert.ToInt32(dr["RaceID"]));
				}

				return dicRace;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
		public static Dictionary<string, int> getAllEthnicity()
		{
			try
			{
				Dictionary<string, int> dicEthnicity = new Dictionary<string, int>();
				string commandText = "select  EthnicityID,EthnicityName from Ethnicity ";
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					dicEthnicity.Add(dr["EthnicityName"].ToString(), Convert.ToInt32(dr["EthnicityID"]));
				}

				return dicEthnicity;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
		public static Dictionary<string, int> getAllGender()
		{
			try
			{
				Dictionary<string, int> dicGender = new Dictionary<string, int>();
				string commandText = "select  GenderID,GenderName from Genders ";
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					dicGender.Add(dr["GenderName"].ToString(), Convert.ToInt32(dr["GenderID"]));
				}


				return dicGender;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public static Dictionary<string, int> getAllVariableDataSet(int SetupID)
		{
			try
			{
				Dictionary<string, int> dicAllIncidenceDataSet = new Dictionary<string, int>();
				string commandText = string.Format("select SetupVariableDatasetID,SetupVariableDatasetName from SetupVariableDatasets where SetupID={0} ", SetupID);
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					dicAllIncidenceDataSet.Add(dr["SetupVariableDatasetName"].ToString(), Convert.ToInt32(dr["SetupVariableDatasetID"]));
				}


				return dicAllIncidenceDataSet;
			}
			catch (Exception ex)
			{
				return null;
			}

		}

		public static Dictionary<string, int> getAllIncidenceDataSet(int SetupID)
		{
			try
			{
				Dictionary<string, int> dicAllIncidenceDataSet = new Dictionary<string, int>();
				string commandText = string.Format("select IncidenceDataSetID,IncidenceDataSetName from IncidenceDataSets where SetupID={0} ", SetupID);
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					dicAllIncidenceDataSet.Add(dr["IncidenceDataSetName"].ToString(), Convert.ToInt32(dr["IncidenceDataSetID"]));
				}


				return dicAllIncidenceDataSet;
			}
			catch (Exception ex)
			{
				return null;
			}

		}
		public static GeographicArea getGeographicArea(int GeographicAreaId)
		{
			try
			{
				string commandText = string.Format("select geographicareaname, entiregriddefinition, griddefinitionid, GeographicAreaFeatureIdField from geographicareas where geographicareaid={0}", GeographicAreaId);
				GeographicArea geographicArea = new GeographicArea();
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				if (ds.Tables[0].Rows.Count == 0) return null;
				DataRow dr = ds.Tables[0].Rows[0];

				geographicArea.GeographicAreaID = GeographicAreaId;
				geographicArea.GeographicAreaName = dr["GeographicAreaName"].ToString();
				geographicArea.GridDefinitionID = Convert.ToInt32(dr["GridDefinitionID"]);
				geographicArea.GeographicAreaFeatureIdField = dr["GeographicAreaFeatureIdField"].ToString();
				return geographicArea;
			}
			catch (Exception ex)
			{
				return null;
			}

		}
		public static BenMAPHealthImpactFunction getBenMAPHealthImpactFunctionFromID(int ID)
		{
			try
			{
				string commandText = string.Format("select CRFunctionID,a.CRFunctionDatasetID,f.CRFunctionDataSetName,a.EndpointGroupID,b.EndPointGroupName,a.EndpointID,c.EndPointName,PollutantID,"
+ " MetricID,SeasonalMetricID,MetricStatistic,Author,YYear,Location,OtherPollutants,Qualifier,Reference,Race,Gender,Startage,Endage,a.FunctionalFormid,d.FunctionalFormText,"
+ " a.IncidenceDatasetID,a.PrevalenceDatasetID,a.VariableDatasetID,Beta,DistBeta,P1Beta,P2Beta,A,NameA,B,NameB,C,NameC,a.BaselineFunctionalFormID,"
+ " e.FunctionalFormText as BaselineFunctionalFormText,Ethnicity,Percentile,GeographicAreaId, GeographicAreaFeatureId, g.IncidenceDataSetName,i.IncidenceDataSetName as PrevalenceDataSetName,"
+ " h.SetupVariableDataSetName as VariableDatasetName from crFunctions a join CRFunctionDataSets f on a.CRFunctionDatasetID=f.CRFunctionDatasetID"
+ " join EndPointGroups b on a.EndPointGroupID=b.EndPointGroupID join EndPoints c on a.EndPointID=c.EndPointID join FunctionalForms d on a.FunctionalFormid=d.FunctionalFormID"
+ " left join BaselineFunctionalForms e on a.BaselineFunctionalFormID=e.FunctionalFormID left join IncidenceDataSets g on a.IncidenceDatasetID=g.IncidenceDatasetID"
+ " left join IncidenceDataSets i on a.PrevalenceDatasetID=i.IncidenceDatasetID left join SetupVariableDataSets h on a.VariableDatasetID=h.SetupVariableDataSetID"
+ " where CRFunctionID={0}", ID);
				BenMAPHealthImpactFunction benMapHealthImpactFunction = new BenMAPHealthImpactFunction();
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				if (ds.Tables[0].Rows.Count == 0) return null;
				DataRow dr = ds.Tables[0].Rows[0];
				if ((dr["IncidenceDatasetID"] is DBNull) == false)
					benMapHealthImpactFunction.IncidenceDataSetID = Convert.ToInt32(dr["IncidenceDatasetID"]);
				if ((dr["PrevalenceDatasetID"] is DBNull) == false)
					benMapHealthImpactFunction.PrevalenceDataSetID = Convert.ToInt32(dr["PrevalenceDatasetID"]);
				if ((dr["VariableDatasetID"] is DBNull) == false)
					benMapHealthImpactFunction.VariableDataSetID = Convert.ToInt32(dr["VariableDatasetID"]);
				if ((dr["IncidenceDatasetName"] is DBNull) == false)
					benMapHealthImpactFunction.IncidenceDataSetName = dr["IncidenceDatasetName"].ToString();
				if ((dr["PrevalenceDatasetName"] is DBNull) == false)
					benMapHealthImpactFunction.PrevalenceDataSetName = dr["PrevalenceDatasetName"].ToString();
				if ((dr["VariableDatasetName"] is DBNull) == false)
					benMapHealthImpactFunction.VariableDataSetName = dr["VariableDatasetName"].ToString();

				benMapHealthImpactFunction.DataSetID = Convert.ToInt32(dr["CRFunctionDatasetID"]);
				benMapHealthImpactFunction.DataSetName = dr["CRFunctionDataSetName"].ToString();


				benMapHealthImpactFunction.Beta = Convert.ToDouble(dr["Beta"]);
				benMapHealthImpactFunction.BetaDistribution = dr["DistBeta"].ToString();
				benMapHealthImpactFunction.BetaParameter1 = Convert.ToDouble(dr["P1Beta"]);
				benMapHealthImpactFunction.BetaParameter2 = Convert.ToDouble(dr["P2Beta"]);
				if ((dr["A"] is DBNull) == false)
					benMapHealthImpactFunction.AContantValue = Convert.ToDouble(dr["A"]);
				if ((dr["NameA"] is DBNull) == false)
					benMapHealthImpactFunction.AContantDescription = dr["NameA"].ToString();
				if ((dr["B"] is DBNull) == false)
					benMapHealthImpactFunction.BContantValue = Convert.ToDouble(dr["B"]);
				if ((dr["NameB"] is DBNull) == false)
					benMapHealthImpactFunction.BContantDescription = dr["NameB"].ToString();
				if ((dr["C"] is DBNull) == false)
					benMapHealthImpactFunction.CContantValue = Convert.ToDouble(dr["C"]);
				if ((dr["NameC"] is DBNull) == false)
					benMapHealthImpactFunction.CContantDescription = dr["NameC"].ToString();


				benMapHealthImpactFunction.ID = Convert.ToInt32(dr["CRFunctionID"]);
				benMapHealthImpactFunction.EndPointGroup = dr["EndPointGroupName"].ToString();
				benMapHealthImpactFunction.EndPointGroupID = Convert.ToInt32(dr["EndpointGroupID"]);
				benMapHealthImpactFunction.EndPoint = dr["EndPointName"].ToString();
				benMapHealthImpactFunction.EndPointID = Convert.ToInt32(dr["EndPointID"]);
				benMapHealthImpactFunction.Pollutant = Grid.GridCommon.getPollutantFromID(Convert.ToInt32(dr["PollutantID"]), CommonClass.lstPollutantAll);
				benMapHealthImpactFunction.Metric = Grid.GridCommon.getMetricFromPollutantAndID(benMapHealthImpactFunction.Pollutant, Convert.ToInt32(dr["MetricID"]));
				benMapHealthImpactFunction.SeasonalMetric = null;
				if ((dr["SeasonalMetricID"] is DBNull) == false)
				{
					benMapHealthImpactFunction.SeasonalMetric = Grid.GridCommon.getSeasonalMetricFromPollutantAndID(benMapHealthImpactFunction.Pollutant, Convert.ToInt32(dr["SeasonalMetricID"]));
				}
				benMapHealthImpactFunction.MetricStatistic = (MetricStatic)Convert.ToInt32(dr["MetricStatistic"]);
				benMapHealthImpactFunction.Author = dr["Author"].ToString();
				benMapHealthImpactFunction.Year = Convert.ToInt32(dr["YYear"]);
				if ((dr["GeographicAreaId"] is DBNull) == false)
				{
					benMapHealthImpactFunction.GeographicAreaID = Convert.ToInt32(dr["GeographicAreaId"]);
					benMapHealthImpactFunction.GeographicAreaName = getGeographicArea(Convert.ToInt32(dr["GeographicAreaId"])).GeographicAreaName;
				}
				if ((dr["GeographicAreaFeatureId"] is DBNull) == false)
				{
					benMapHealthImpactFunction.GeographicAreaFeatureID = dr["GeographicAreaFeatureId"].ToString();
					benMapHealthImpactFunction.GeographicAreaName = benMapHealthImpactFunction.GeographicAreaName + ": " + benMapHealthImpactFunction.GeographicAreaFeatureID;
				}
				if (dr["Location"] is DBNull == false)
				{
					benMapHealthImpactFunction.strLocations = dr["Location"].ToString();
				}
				benMapHealthImpactFunction.OtherPollutants = dr["OtherPollutants"].ToString();
				benMapHealthImpactFunction.Qualifier = dr["Qualifier"].ToString();
				benMapHealthImpactFunction.Reference = dr["Reference"].ToString();
				if ((dr["Race"] is DBNull) == false)
					benMapHealthImpactFunction.Race = dr["Race"].ToString();
				if ((dr["Gender"] is DBNull) == false)
					benMapHealthImpactFunction.Gender = dr["Gender"].ToString();
				if ((dr["Startage"] is DBNull) == false)
					benMapHealthImpactFunction.StartAge = Convert.ToInt32(dr["Startage"]);
				if ((dr["Endage"] is DBNull) == false)
					benMapHealthImpactFunction.EndAge = Convert.ToInt32(dr["Endage"]);
				benMapHealthImpactFunction.Function = dr["FunctionalFormText"].ToString();
				benMapHealthImpactFunction.BaseLineIncidenceFunction = dr["BaselineFunctionalFormText"].ToString();
				if ((dr["Ethnicity"] is DBNull) == false)
					benMapHealthImpactFunction.Ethnicity = dr["Ethnicity"].ToString();
				if ((dr["Percentile"] is DBNull) == false)
					benMapHealthImpactFunction.Percentile = Convert.ToInt32(dr["Percentile"]);
				return benMapHealthImpactFunction;
			}
			catch (Exception ex)
			{
				return null;
			}

		}
		public static double Normal(double x, double miu, double sigma)
		{
			return 1.0 / (x * Math.Sqrt(2 * Math.PI) * sigma) * Math.Exp(-1 * (Math.Log(x) - miu) * (Math.Log(x) - miu) / (2 * sigma * sigma));
		}
		public static double triangular(double Min, double Mode, double Max)
		{
			double R = 0.0;
			Random r = new Random();
			R = r.NextDouble();
			if (R == ((Mode - Min) / (Max - Min)))
			{
				return Mode;
			}
			else if (R < ((Mode - Min) / (Max - Min)))
			{
				return Min + Math.Sqrt(R * (Max - Min) * (Mode - Min));
			}
			else
			{
				return Max - Math.Sqrt((1 - R) * (Max - Min) * (Max - Mode));
			}
		}
		public static double[] simulate(int Total, double[] Tmin, double[] Tmod, double[] Tmax)
		{
			int mlngEvals = 10000;
			int i = 0, i1 = 0, i2 = 0;
			double[] TMin = new double[Total];
			double[] TMod = new double[Total];
			double[] TMax = new double[Total];
			double[] mlngResults = new double[Total];
			double Time = 0.0;
			long lngWinner = 0;
			double Winner = 0;
			for (i = 0; i < Total; i++)
			{
				TMin[i] = Tmin[i];
				TMod[i] = Tmod[i];
				TMax[i] = Tmax[i];
				mlngResults[i] = 0;
			}
			for (i1 = 1; i1 <= mlngEvals; i1++)
			{
				lngWinner = 0;
				Winner = triangular(TMin[0], TMod[0], TMax[0]);
				for (i2 = 1; i2 < Total; i2++)
				{
					Time = triangular(TMin[i2], TMod[i2], TMax[i2]);
					if (Time < Winner)
					{
						Winner = Time;
						lngWinner = i2;
					}
				}
				mlngResults[lngWinner]++;
			}
			return mlngResults;
		}

		public static double[] getLHSArrayCRFunctionSeed(int LatinHypercubePoints, CRSelectFunction crSelectFunction, int Seed, int MonteCarlo)
		{
			try
			{
				//    List<int> lstInt = new List<int>();
				//    for (int i = 0; i < LatinHypercubePoints; i++)
				//    {
				//        lstInt.Add(Convert.ToInt16(Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(LatinHypercubePoints) - (100.00 / (2 * Convert.ToDouble(LatinHypercubePoints)))));
				//    }
				double[] lhsResultArray = new double[LatinHypercubePoints];
				Meta.Numerics.Statistics.Sample sample = null;
				// distribution switch statement
				// System.Console.WriteLine("Distribution " + crSelectFunction.BenMAPHealthImpactFunction.BetaDistribution);  --Unsure why this was being written to the Command Line, Commented out for Ben-MAP 229
				switch (crSelectFunction.BenMAPHealthImpactFunction.BetaDistribution)
				{
					case "None":
						for (int i = 0; i < LatinHypercubePoints; i++)
						{
							lhsResultArray[i] = crSelectFunction.BenMAPHealthImpactFunction.Beta;

						}
						return lhsResultArray;
						break;
					case "Normal":

						Meta.Numerics.Statistics.Distributions.Distribution Normal_distribution =
new Meta.Numerics.Statistics.Distributions.NormalDistribution(crSelectFunction.BenMAPHealthImpactFunction.Beta, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1);
						sample = CreateSample(Normal_distribution, MonteCarlo, Seed);
						break;
					case "Triangular":
						Meta.Numerics.Statistics.Distributions.Distribution Triangular_distribution =
new Meta.Numerics.Statistics.Distributions.TriangularDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2, crSelectFunction.BenMAPHealthImpactFunction.Beta);
						sample = CreateSample(Triangular_distribution, MonteCarlo, Seed);
						break;
					case "Poisson":
						Meta.Numerics.Statistics.Distributions.PoissonDistribution Poisson_distribution =
new Meta.Numerics.Statistics.Distributions.PoissonDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1);
						sample = CreateSample(Poisson_distribution, MonteCarlo, Seed);
						break;
					case "Binomial":
						Meta.Numerics.Statistics.Distributions.BinomialDistribution Binomial_distribution =
new Meta.Numerics.Statistics.Distributions.BinomialDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Convert.ToInt32(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2));
						sample = CreateSample(Binomial_distribution, MonteCarlo, Seed);
						break;
					case "LogNormal":
						Meta.Numerics.Statistics.Distributions.LognormalDistribution Lognormal_distribution =
new Meta.Numerics.Statistics.Distributions.LognormalDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
						sample = CreateSample(Lognormal_distribution, MonteCarlo, Seed);
						break;
					case "Uniform":
						Interval interval = Interval.FromEndpoints(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1,
crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);

						Meta.Numerics.Statistics.Distributions.UniformDistribution Uniform_distribution =
new Meta.Numerics.Statistics.Distributions.UniformDistribution(interval);
						sample = CreateSample(Uniform_distribution, MonteCarlo, Seed);
						break;
					case "Exponential":
						Meta.Numerics.Statistics.Distributions.ExponentialDistribution Exponential_distribution =
new Meta.Numerics.Statistics.Distributions.ExponentialDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1);
						sample = CreateSample(Exponential_distribution, MonteCarlo, Seed);
						break;
					case "Geometric":
						Meta.Numerics.Statistics.Distributions.ExponentialDistribution Geometric_distribution =
new Meta.Numerics.Statistics.Distributions.ExponentialDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1);
						sample = CreateSample(Geometric_distribution, MonteCarlo, Seed);
						break;
					case "Weibull":
						Meta.Numerics.Statistics.Distributions.WeibullDistribution Weibull_distribution =
new Meta.Numerics.Statistics.Distributions.WeibullDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
						sample = CreateSample(Weibull_distribution, MonteCarlo, Seed);
						break;
					case "Gamma":
						Meta.Numerics.Statistics.Distributions.GammaDistribution Gamma_distribution =
new Meta.Numerics.Statistics.Distributions.GammaDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
						sample = CreateSample(Gamma_distribution, MonteCarlo, Seed);
						break;
					case "Logistic":
						Meta.Numerics.Statistics.Distributions.Distribution logistic_distribution =
new Meta.Numerics.Statistics.Distributions.LogisticDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
						sample = CreateSample(logistic_distribution, MonteCarlo, Seed);

						break;
					case "Beta":

						Meta.Numerics.Statistics.Distributions.BetaDistribution Beta_distribution =
								new Meta.Numerics.Statistics.Distributions.BetaDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
						sample = CreateSample(Beta_distribution, MonteCarlo, Seed);
						break;
					case "Pareto":
						Meta.Numerics.Statistics.Distributions.ParetoDistribution Pareto_distribution =
new Meta.Numerics.Statistics.Distributions.ParetoDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
						sample = CreateSample(Pareto_distribution, MonteCarlo, Seed);
						break;
					case "Cauchy":
						Meta.Numerics.Statistics.Distributions.CauchyDistribution Cauchy_distribution =
new Meta.Numerics.Statistics.Distributions.CauchyDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
						sample = CreateSample(Cauchy_distribution, MonteCarlo, Seed);



						break;
					case "Custom":
						string commandText = string.Format("select   VValue  from CRFunctionCustomEntries where CRFunctionID={0} order by vvalue", crSelectFunction.BenMAPHealthImpactFunction.ID);
						ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

						DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
						List<double> lstCustom = new List<double>();
						foreach (DataRow dr in ds.Tables[0].Rows)
						{
							lstCustom.Add(Convert.ToDouble(dr[0]));

						}
						lstCustom.Sort();
						//calculate percentile range
						for (int i = 0; i < LatinHypercubePoints; i++)
						{
							lhsResultArray[i] = lstCustom.GetRange(i * (lstCustom.Count / LatinHypercubePoints), (lstCustom.Count / LatinHypercubePoints)).Median();
						}
						return lhsResultArray;
						break;


				}
				List<double> lstlogistic = sample.ToList();
				lstlogistic.Sort();

				for (int i = 0; i < LatinHypercubePoints; i++)
				{
					lhsResultArray[i] = lstlogistic.GetRange(i * (lstlogistic.Count / LatinHypercubePoints), (lstlogistic.Count / LatinHypercubePoints)).Median();
				}
				return lhsResultArray;


			}
			catch
			{
				return null;
			}
		}
		private static Meta.Numerics.Statistics.Sample CreateSample(Meta.Numerics.Statistics.Distributions.Distribution distribution, int count)
		{
			return (CreateSample(distribution, count, 1));
		}
		private static Meta.Numerics.Statistics.Sample CreateSample(Meta.Numerics.Statistics.Distributions.DiscreteDistribution distribution, int count, int seed)
		{

			Meta.Numerics.Statistics.Sample sample = new Meta.Numerics.Statistics.Sample();

			System.Random rng = new System.Random(seed);
			for (int i = 0; i < count; i++)
			{
				double x = distribution.InverseLeftProbability(rng.NextDouble());
				sample.Add(x);
			}

			return (sample);
		}
		private static Meta.Numerics.Statistics.Sample CreateSample(Meta.Numerics.Statistics.Distributions.Distribution distribution, int count, int seed)
		{

			Meta.Numerics.Statistics.Sample sample = new Meta.Numerics.Statistics.Sample();

			System.Random rng = new System.Random(seed);
			for (int i = 0; i < count; i++)
			{
				double x = distribution.InverseLeftProbability(rng.NextDouble());
				sample.Add(x);
			}

			return (sample);
		}

		public static double getPrevalenceValueFromColRow(int Col, int Row, List<IncidenceRateAttribute> lstPrevalenceRateAttribute, int PrevalenceDataSetGridType, int GridDefinitionID, GridRelationship gridRelationShipPrevalence)
		{
			try
			{
				double prevalenceValue = 0;
				if (lstPrevalenceRateAttribute.Count > 0)
				{

					if (PrevalenceDataSetGridType == GridDefinitionID)
					{
						var queryPrevalence = from a in lstPrevalenceRateAttribute where a.Col == Col && a.Row == Row select a;
						double values = 0;
						foreach (IncidenceRateAttribute iRateAttributes in queryPrevalence)
						{
							values += iRateAttributes.Value;

						}
						if (queryPrevalence.Count() > 0) prevalenceValue = values / Convert.ToDouble(queryPrevalence.Count());
					}
					else
					{
						RowCol rowColPrevalence = new RowCol() { Col = Col, Row = Row };
						List<RowCol> lstPrevalenceRowCol;
						if (PrevalenceDataSetGridType == gridRelationShipPrevalence.bigGridID)
						{

							var queryrowCol = from a in gridRelationShipPrevalence.lstGridRelationshipAttribute where a.smallGridRowCol.Contains(rowColPrevalence, new RowColComparer()) select new RowCol() { Col = a.bigGridRowCol.Col, Row = a.bigGridRowCol.Row };
							lstPrevalenceRowCol = queryrowCol.ToList();
							var queryPrevalence = from a in lstPrevalenceRateAttribute where lstPrevalenceRowCol.Contains(new RowCol() { Col = a.Col, Row = a.Row }, new RowColComparer()) select new { Values = lstPrevalenceRateAttribute.Average(c => c.Value) };

							if (queryPrevalence != null && queryPrevalence.Count() > 0)
								prevalenceValue = queryPrevalence.First().Values;

						}
						else
						{
							var queryrowCol = from a in gridRelationShipPrevalence.lstGridRelationshipAttribute where a.bigGridRowCol.Col == rowColPrevalence.Col && a.bigGridRowCol.Row == rowColPrevalence.Row select a;
							if (queryrowCol != null && queryrowCol.Count() > 0)
							{
								lstPrevalenceRowCol = queryrowCol.First().smallGridRowCol;
								List<IncidenceRateAttribute> lstQueryPrevalence = new List<IncidenceRateAttribute>();
								var queryPrevalence = from a in lstPrevalenceRateAttribute where lstPrevalenceRowCol.Contains(new RowCol() { Row = a.Row, Col = a.Col }, new RowColComparer()) select new { Values = lstPrevalenceRateAttribute.Average(c => c.Value) };



								if (queryPrevalence != null && queryPrevalence.Count() > 0)
									prevalenceValue = queryPrevalence.First().Values;
							}
						}
					}
				}
				return prevalenceValue;
			}
			catch (Exception ex)
			{
				return 0;
			}
		}
		public static double getIncidenceValueFromColRow(int Col, int Row, List<IncidenceRateAttribute> lstIncidenceRateAttribute, int incidenceDataSetGridType, int GridDefinitionID, GridRelationship gridRelationShipIncidence)
		{
			try
			{
				double incidenceValue = 0;
				if (lstIncidenceRateAttribute.Count > 0)
				{
					if (incidenceDataSetGridType == GridDefinitionID)
					{
						var queryIncidence = from a in lstIncidenceRateAttribute where a.Col == Col && a.Row == Row select a;
						double values = 0;
						foreach (IncidenceRateAttribute iRateAttributes in queryIncidence)
						{
							values += iRateAttributes.Value;

						}
						if (queryIncidence.Count() > 0) incidenceValue = values / Convert.ToDouble(queryIncidence.Count());
					}
					else
					{

						List<RowCol> lstIncidenceRowCol;
						if (incidenceDataSetGridType == gridRelationShipIncidence.bigGridID)
						{
							RowCol rowColIncidence = new RowCol() { Col = Col, Row = Row };
							var queryrowCol = from a in gridRelationShipIncidence.lstGridRelationshipAttribute where a.smallGridRowCol.Contains(rowColIncidence, new RowColComparer()) select new RowCol() { Col = a.bigGridRowCol.Col, Row = a.bigGridRowCol.Row };
							lstIncidenceRowCol = queryrowCol.ToList();

							var queryIncidence = from a in lstIncidenceRateAttribute where lstIncidenceRowCol.Contains(new RowCol() { Col = a.Col, Row = a.Row }, new RowColComparer()) select new { Values = lstIncidenceRateAttribute.Average(c => c.Value) };

							if (queryIncidence != null && queryIncidence.Count() > 0)
								incidenceValue = queryIncidence.First().Values;
						}
						else
						{
							var queryrowCol = from a in gridRelationShipIncidence.lstGridRelationshipAttribute where a.bigGridRowCol.Col == Col && a.bigGridRowCol.Row == Row select a;
							if (queryrowCol != null && queryrowCol.Count() > 0)
							{
								lstIncidenceRowCol = queryrowCol.First().smallGridRowCol;
								var queryIncidence = from a in lstIncidenceRateAttribute where lstIncidenceRowCol.Contains(new RowCol() { Row = a.Row, Col = a.Col }, new RowColComparer()) select new { Values = lstIncidenceRateAttribute.Average(c => c.Value) };

								if (queryIncidence != null && queryIncidence.Count() > 0)
									incidenceValue = queryIncidence.First().Values;
							}

						}
					}
				}
				return incidenceValue;
			}
			catch (Exception ex)
			{
				return 0;
			}
		}
		public static double getPopulationValueFromColRow(int Col, int Row, BenMAPPopulation benMAPPopulation, List<PopulationAttribute> lstPopulationAttribute, int GridDefinitionID, GridRelationship gridRelationShipPopulation)
		{
			try
			{
				double PopulationValue = 0;
				if (lstPopulationAttribute.Count > 0)
				{
					if (benMAPPopulation.GridType.GridDefinitionID == GridDefinitionID)
					{
						var queryPopulation = from a in lstPopulationAttribute where a.Col == Col && a.Row == Row select a;
						double values = 0;
						foreach (PopulationAttribute iPopulationAttributes in queryPopulation)
						{
							values += iPopulationAttributes.Value;

						}
						if (queryPopulation.Count() > 0) PopulationValue = values;
					}
					else
					{
						RowCol rowColPopulation = new RowCol() { Col = Col, Row = Row };
						List<RowCol> lstPopulationRowCol;
						if (benMAPPopulation.GridType.GridDefinitionID == gridRelationShipPopulation.bigGridID)
						{

							var queryrowCol = from a in gridRelationShipPopulation.lstGridRelationshipAttribute where a.smallGridRowCol.Contains(rowColPopulation, new RowColComparer()) select new RowCol() { Col = a.bigGridRowCol.Col, Row = a.bigGridRowCol.Row };
							lstPopulationRowCol = queryrowCol.ToList();

							var queryPopulation = from a in lstPopulationAttribute where lstPopulationRowCol.Contains(new RowCol() { Col = a.Col, Row = a.Row }, new RowColComparer()) select new { Values = lstPopulationAttribute.Sum(c => c.Value) };

							if (queryPopulation != null && queryPopulation.Count() > 0)
								PopulationValue = queryPopulation.First().Values / lstPopulationRowCol.Count;

						}
						else
						{
							var queryrowCol = from a in gridRelationShipPopulation.lstGridRelationshipAttribute where a.bigGridRowCol.Col == rowColPopulation.Col && a.bigGridRowCol.Row == rowColPopulation.Row select a;
							if (queryrowCol != null && queryrowCol.Count() > 0)
							{
								lstPopulationRowCol = queryrowCol.First().smallGridRowCol;
								List<PopulationAttribute> lstQueryPopulation = new List<PopulationAttribute>();
								var queryPopulation = from a in lstPopulationAttribute where lstPopulationRowCol.Contains(new RowCol() { Row = a.Row, Col = a.Col }, new RowColComparer()) select new { Values = lstPopulationAttribute.Sum(c => c.Value) };



								if (queryPopulation != null && queryPopulation.Count() > 0)
									PopulationValue = queryPopulation.First().Values;
							}
						}
					}
				}
				return PopulationValue;
			}
			catch (Exception ex)
			{
				return 0;
			}

		}












		public static Dictionary<string, float> getPopulationGrowthFromCommandText(string commandText, int GridDefinitionID, BenMAPPopulation benMAPPopulation)
		{
			try
			{
				Dictionary<string, float> dicPopulation = new Dictionary<string, float>();
				List<PopulationAttribute> lstPopulationAttribute = new List<PopulationAttribute>();
				List<PopulationAttribute> lstResult = new List<PopulationAttribute>();
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				Dictionary<string, float> diclstPopulationAttribute = new Dictionary<string, float>();
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					PopulationAttribute pAttribute = new PopulationAttribute()
					{
						Col = Convert.ToInt32(dr["CColumn"]),
						Row = Convert.ToInt32(dr["Row"]),
						Value = Convert.ToSingle(dr["VValue"])
					};
					lstPopulationAttribute.Add(pAttribute);
					if (!diclstPopulationAttribute.Keys.Contains(pAttribute.Col + "," + pAttribute.Row))
					{
						diclstPopulationAttribute.Add(pAttribute.Col + "," + pAttribute.Row, pAttribute.Value);
					}



				}
				float PopulationValue = 0;
				List<float> lstTemp = null;
				if (benMAPPopulation.GridType.GridDefinitionID == GridDefinitionID)
				{
					lstResult = lstPopulationAttribute;
				}
				else
				{
					GridRelationship gridRelationShipPopulation = new GridRelationship();

					foreach (GridRelationship gRelationship in CommonClass.LstGridRelationshipAll)
					{
						if ((gRelationship.bigGridID == benMAPPopulation.GridType.GridDefinitionID && gRelationship.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID) || (gRelationship.smallGridID == benMAPPopulation.GridType.GridDefinitionID && gRelationship.bigGridID == CommonClass.GBenMAPGrid.GridDefinitionID))
						{
							gridRelationShipPopulation = gRelationship;
						}
					}
					if (benMAPPopulation.GridType.GridDefinitionID == gridRelationShipPopulation.bigGridID)
					{
						foreach (GridRelationshipAttribute gra in gridRelationShipPopulation.lstGridRelationshipAttribute)
						{
							if (diclstPopulationAttribute.Keys.Contains(gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row))
							{
								foreach (RowCol rc in gra.smallGridRowCol)
								{
									lstResult.Add(new PopulationAttribute()
									{
										Col = rc.Col,
										Row = rc.Row,
										Value = diclstPopulationAttribute[gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row] / Convert.ToSingle(gra.smallGridRowCol.Count())
									});
								}
							}

						}
					}
					else
					{
						foreach (GridRelationshipAttribute gra in gridRelationShipPopulation.lstGridRelationshipAttribute)
						{


							lstResult.Add(new PopulationAttribute()
							{
								Col = gra.bigGridRowCol.Col,
								Row = gra.bigGridRowCol.Row,
								Value = 0
							});
							foreach (RowCol rc in gra.smallGridRowCol)
							{
								if (diclstPopulationAttribute.Keys.Contains(rc.Col + "," + rc.Row))
								{
									lstResult[lstResult.Count - 1].Value += diclstPopulationAttribute[rc.Col + "," + rc.Row];
								}
							}


						}
					}

				}
				foreach (PopulationAttribute pa in lstResult)
				{
					if (!dicPopulation.Keys.Contains(pa.Col + "," + pa.Row))
					{
						dicPopulation.Add(pa.Col + "," + pa.Row, pa.Value);
					}
				}
				ds.Dispose();
				return dicPopulation;

			}
			catch
			{
				return null;
			}
		}

		public static string getPopulationComandTextFrom12kmToCounty(CRSelectFunction crSelectFunction, BenMAPPopulation benMAPPopulation, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender)
		{
			ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
			int benMAPPopulationDataSetID = benMAPPopulation.DataSetID;
			string commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", benMAPPopulation.DataSetID); int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
			if (CommonClass.MainSetup.SetupID != 1) commonYear = benMAPPopulation.Year;
			commandText = "";
			try
			{
				fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, "select count(0) from POP12kmToCounty");
			}
			catch (Exception ex)
			{
				fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, "CREATE TABLE POP12kmToCounty (RACEID  SMALLINT NOT NULL,GENDERID  SMALLINT NOT NULL," +
"  AGERANGEID           SMALLINT NOT NULL,  CCOLUMN              INTEGER NOT NULL," +
"  ROW                  INTEGER NOT NULL,  VVALUE               FLOAT NOT NULL," +
"  ETHNICITYID          SMALLINT NOT NULL); ");
				fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, "insert into POP12kmToCounty select a.RaceID,a.GenderID,a.Agerangeid,b.TargetColumn, b.TargetRow,sum(a.VValue*b.Percentage) as VValue,a.Ethnicityid " +
"  from PopulationEntries a, (select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentageEntries where percentageid=22 and normalizationstate in (0,1)) b " +
" where a.row=b.sourcerow and a.Ccolumn=b.sourcecolumn and a.PopulationDataSetID=4 " +
" group by b.TargetColumn, b.TargetRow,a.RaceID,a.GenderID,a.Agerangeid,a.Ethnicityid;");
			}

			string strwhere = "";
			if (CommonClass.MainSetup.SetupID == 1)
				strwhere = "where AGERANGEID!=42";
			else
				strwhere = " where 1=1 ";
			string ageCommandText = string.Format("select * from Ageranges b   " + strwhere);
			if (crSelectFunction.StartAge != -1)
			{
				ageCommandText = string.Format(ageCommandText + " and b.EndAge>={0} ", crSelectFunction.StartAge);
			}
			if (crSelectFunction.EndAge != -1)
			{
				ageCommandText = string.Format(ageCommandText + " and b.StartAge<={0} ", crSelectFunction.EndAge);
			}
			DataSet dsage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, ageCommandText);
			string strsumage = "";
			string strsumageGrowth = "";
			foreach (DataRow dr in dsage.Tables[0].Rows)
			{
				if (strsumageGrowth == "")
					strsumageGrowth = dr["AgerangeID"].ToString();
				else
					strsumageGrowth = strsumageGrowth + "," + dr["AgerangeID"].ToString();
				if ((Convert.ToInt32(dr["StartAge"]) >= crSelectFunction.StartAge || crSelectFunction.StartAge == -1) && (Convert.ToInt32(dr["EndAge"]) <= crSelectFunction.EndAge || crSelectFunction.EndAge == -1))
				{
					if (strsumage == "")
						strsumage = dr["AgerangeID"].ToString();
					else
						strsumage = strsumage + "," + dr["AgerangeID"].ToString();
				}
				else
				{
					double dDiv = 1;
					if (Convert.ToInt32(dr["StartAge"]) < crSelectFunction.StartAge)
					{
						dDiv = Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - crSelectFunction.StartAge + 1) / Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]) + 1);
						if (Convert.ToInt32(dr["EndAge"]) > crSelectFunction.EndAge)
						{
							dDiv = Convert.ToDouble(crSelectFunction.EndAge - crSelectFunction.StartAge + 1) / Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]) + 1);

						}
					}
					else if (Convert.ToInt32(dr["EndAge"]) > crSelectFunction.EndAge)
					{
						dDiv = Convert.ToDouble(crSelectFunction.EndAge - Convert.ToInt32(dr["StartAge"]) + 1) / Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]) + 1);


					}

					if (commandText != "") commandText = commandText + " union ";

					commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue)*" + dDiv + " as VValue   from POP12kmToCounty a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
							"  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID ");


					commandText = string.Format(commandText + " and a.AgerangeID={0}", Convert.ToInt32(dr["AgerangeID"]));
					if (!string.IsNullOrEmpty(crSelectFunction.Race) && crSelectFunction.Race.ToLower() != "all")
					{
						if (dicRace[crSelectFunction.Race] != null)
						{
							commandText = string.Format(commandText + " and (a.RaceID={0} or a.RaceID=6)", dicRace[crSelectFunction.Race]);
						}
					}
					if (!string.IsNullOrEmpty(crSelectFunction.Ethnicity) && crSelectFunction.Ethnicity.ToLower() != "all")
					{
						if (dicEthnicity[crSelectFunction.Ethnicity] != null)
						{
							commandText = string.Format(commandText + " and (a.EthnicityID={0} or a.EthnicityID=4)", dicEthnicity[crSelectFunction.Ethnicity]);

						}
					}
					if (!string.IsNullOrEmpty(crSelectFunction.Gender) && crSelectFunction.Gender.ToLower() != "all")
					{
						if (dicGender[crSelectFunction.Gender] != null)
						{
							commandText = string.Format(commandText + " and (a.GenderID={0} or a.GenderID=4)", dicGender[crSelectFunction.Gender]);
						}
					}
					commandText = commandText + " group by a.CColumn,a.Row";
				}
			}
			if (commandText != "" && strsumage != "") commandText = commandText + " union ";
			if (strsumage != "")
			{

				commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.VValue) as VValue   from POP12kmToCounty a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
						"  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID ");

				commandText = string.Format(commandText + " and a.AgerangeID in ({0}) ", strsumage);

				if (!string.IsNullOrEmpty(crSelectFunction.Race))
				{
					if (dicRace[crSelectFunction.Race].ToString() != "")
					{
						commandText = string.Format(commandText + " and (a.RaceID={0} or a.RaceID=6)", dicRace[crSelectFunction.Race]);
					}
				}
				if (!string.IsNullOrEmpty(crSelectFunction.Ethnicity))
				{
					if (dicEthnicity[crSelectFunction.Ethnicity].ToString() != "")
					{
						commandText = string.Format(commandText + " and (a.EthnicityID={0} or a.EthnicityID=4)", dicEthnicity[crSelectFunction.Ethnicity]);

					}
				}
				if (!string.IsNullOrEmpty(crSelectFunction.Gender))
				{
					if (dicGender[crSelectFunction.Gender].ToString() != "")
					{
						commandText = string.Format(commandText + " and (a.GenderID={0} or a.GenderID=4)", dicGender[crSelectFunction.Gender]);
					}
				}
				commandText = commandText + " group by a.CColumn,a.Row";
			}
			if (commandText != "")
			{
				commandText = "select   a.CColumn,a.Row,sum(a.vvalue) as VValue  from ( " + commandText + " ) a group by a.CColumn,a.Row";
			}
			return commandText;
		}
		public static string getPopulationComandTextFromCRSelectFunctionForPop(CRSelectFunction crSelectFunction, BenMAPPopulation benMAPPopulation, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender)
		{
			ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
			int benMAPPopulationDataSetID = benMAPPopulation.DataSetID;
			string commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", benMAPPopulation.DataSetID); int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
			if (CommonClass.MainSetup.SetupID != 1) commonYear = benMAPPopulation.Year;
			commandText = "";
			string strwhere = "";
			if (CommonClass.MainSetup.SetupID == 1)
				strwhere = "where AGERANGEID!=42";
			else
				strwhere = " where 1=1 ";
			string ageCommandText = string.Format("select * from Ageranges b   " + strwhere);
			if (crSelectFunction.StartAge != -1)
			{
				ageCommandText = string.Format(ageCommandText + " and b.EndAge>={0} ", crSelectFunction.StartAge);
			}
			if (crSelectFunction.EndAge != -1)
			{
				ageCommandText = string.Format(ageCommandText + " and b.StartAge<={0} ", crSelectFunction.EndAge);
			}
			DataSet dsage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, ageCommandText);
			string strsumage = "";
			string strsumageGrowth = "";
			foreach (DataRow dr in dsage.Tables[0].Rows)
			{
				if (strsumageGrowth == "")
					strsumageGrowth = dr["AgerangeID"].ToString();
				else
					strsumageGrowth = strsumageGrowth + "," + dr["AgerangeID"].ToString();
				if ((Convert.ToInt32(dr["StartAge"]) >= crSelectFunction.StartAge || crSelectFunction.StartAge == -1) && (Convert.ToInt32(dr["EndAge"]) <= crSelectFunction.EndAge || crSelectFunction.EndAge == -1))
				{
					if (strsumage == "")
						strsumage = dr["AgerangeID"].ToString();
					else
						strsumage = strsumage + "," + dr["AgerangeID"].ToString();
				}
				else
				{
					double dDiv = 1;
					if (Convert.ToInt32(dr["StartAge"]) < crSelectFunction.StartAge)
					{
						dDiv = Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - crSelectFunction.StartAge + 1) / Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]) + 1);
						if (Convert.ToInt32(dr["EndAge"]) > crSelectFunction.EndAge)
						{
							dDiv = Convert.ToDouble(crSelectFunction.EndAge - crSelectFunction.StartAge + 1) / Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]) + 1);

						}
					}
					else if (Convert.ToInt32(dr["EndAge"]) > crSelectFunction.EndAge)
					{
						dDiv = Convert.ToDouble(crSelectFunction.EndAge - Convert.ToInt32(dr["StartAge"]) + 1) / Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]) + 1);


					}

					if (commandText != "") commandText = commandText + " union ";
					if (benMAPPopulation.GridType.GridDefinitionID == 1 && CommonClass.MainSetup.SetupID == 1 && commonYear != benMAPPopulation.Year)
					{
						commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue*" + dDiv + ") as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
								"  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);

					}
					else if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && commonYear != benMAPPopulation.Year)
					{
						commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue*" + dDiv + ") as VValue   from PopulationEntries a," +
								"(select b.SourceColumn,b.SourceRow,a.VValue*b.VValue as VValue,a.AgerangeID,a.RaceID,a.EthnicityID,a.GenderID from PopulationEntries a,populationgrowthweights b where a.PopulationDatasetID=2 and a.YYear=" + benMAPPopulation.Year + " and a.CColumn=b.targetcolumn and a.Row =b.TargetRow and a.EthnicityID=b.EthnicityID and a.RaceID=b.RaceID) b " +
									 "  where  a.CColumn=b.sourcecolumn and a.Row=b.sourcerow  and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);






					}
					else
					{
						commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue)*" + dDiv + " as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);
					}
					commandText = string.Format(commandText + " and a.AgerangeID={0}", Convert.ToInt32(dr["AgerangeID"]));
					if (!string.IsNullOrEmpty(crSelectFunction.Race) && crSelectFunction.Race.ToLower() != "all")
					{
						if (dicRace[crSelectFunction.Race] != null)
						{
							commandText = string.Format(commandText + " and (a.RaceID={0} or a.RaceID=6)", dicRace[crSelectFunction.Race]);
						}
					}
					if (!string.IsNullOrEmpty(crSelectFunction.Ethnicity) && crSelectFunction.Ethnicity.ToLower() != "all")
					{
						if (dicEthnicity[crSelectFunction.Ethnicity] != null)
						{
							commandText = string.Format(commandText + " and (a.EthnicityID={0} or a.EthnicityID=4)", dicEthnicity[crSelectFunction.Ethnicity]);

						}
					}
					if (!string.IsNullOrEmpty(crSelectFunction.Gender) && crSelectFunction.Gender.ToLower() != "all")
					{
						if (dicGender[crSelectFunction.Gender] != null)
						{
							commandText = string.Format(commandText + " and (a.GenderID={0} or a.GenderID=4)", dicGender[crSelectFunction.Gender]);
						}
					}
					commandText = commandText + " group by a.CColumn,a.Row,a.AgeRangeID";
				}
			}
			if (commandText != "" && strsumage != "") commandText = commandText + " union ";
			if (strsumage != "")
			{
				if (benMAPPopulation.GridType.GridDefinitionID == 1 && CommonClass.MainSetup.SetupID == 1 && commonYear != benMAPPopulation.Year)
				{
					commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.VValue) as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
							"  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);

				}
				else if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && commonYear != benMAPPopulation.Year)
				{


					commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue) as VValue   from PopulationEntries a," +
									"(select b.TargetColumn,b.TargetRow,a.VValue*b.VValue as VValue,a.AgerangeID,a.RaceID,a.EthnicityID,a.GenderID from PopulationEntries a,populationgrowthweights b where a.PopulationDatasetID=2 and a.YYear=" + benMAPPopulation.Year + " and a.CColumn=b.targetcolumn and a.Row =b.TargetRow and a.EthnicityID=b.EthnicityID and a.RaceID=b.RaceID) b " +
										 "  where  a.CColumn=b.Targetcolumn and a.Row=b.TargetColumn  and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);

				}
				else
				{
					commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue) as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);
				}
				commandText = string.Format(commandText + " and a.AgerangeID in ({0}) ", strsumage);

				if (!string.IsNullOrEmpty(crSelectFunction.Race))
				{
					if (dicRace[crSelectFunction.Race].ToString() != "")
					{
						commandText = string.Format(commandText + " and (a.RaceID={0} or a.RaceID=6)", dicRace[crSelectFunction.Race]);
					}
				}
				if (!string.IsNullOrEmpty(crSelectFunction.Ethnicity))
				{
					if (dicEthnicity[crSelectFunction.Ethnicity].ToString() != "")
					{
						commandText = string.Format(commandText + " and (a.EthnicityID={0} or a.EthnicityID=4)", dicEthnicity[crSelectFunction.Ethnicity]);

					}
				}
				if (!string.IsNullOrEmpty(crSelectFunction.Gender))
				{
					if (dicGender[crSelectFunction.Gender].ToString() != "")
					{
						commandText = string.Format(commandText + " and (a.GenderID={0} or a.GenderID=4)", dicGender[crSelectFunction.Gender]);
					}
				}
				commandText = commandText + " group by a.CColumn,a.Row";
			}
			if (commandText != "")
			{
				commandText = "select   a.CColumn,a.Row,sum(a.vvalue) as VValue  from ( " + commandText + " ) a group by a.CColumn,a.Row";
			}
			return commandText;
		}
		public static Dictionary<string, Dictionary<string, WeightAttribute>> DicWeight;
		public static Dictionary<string, double> DicGrowth;
		public static int Year;
		public static void updatePercentageToDatabase(KeyValuePair<string, List<GridRelationshipAttributePercentage>> dicAllGridPercentage)
		{
			string commandText = "select max(PercentageID) from GridDefinitionPercentages";
			ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
			int iMax = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
			commandText = string.Format("insert into GridDefinitionPercentages values({0},{1})", iMax, dicAllGridPercentage.Key);
			fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
			int i = 1;
			commandText = "execute block as declare incidenceRateID int;" + " BEGIN ";
			FirebirdSql.Data.FirebirdClient.FbCommand fbCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
			fbCommand.Connection = CommonClass.Connection;
			fbCommand.CommandType = CommandType.Text;
			if (fbCommand.Connection.State != ConnectionState.Open)
			{ fbCommand.Connection.Open(); }
			int j = 0;
			foreach (GridRelationshipAttributePercentage grp in dicAllGridPercentage.Value)
			{

				if (i < 250 && j < dicAllGridPercentage.Value.Count - 1)
				{
					commandText = commandText + string.Format(" insert into GridDefinitionPercentageEntries values({0},{1},{2},{3},{4},{5},{6});",
	iMax, grp.sourceCol, grp.sourceRow, grp.targetCol, grp.targetRow, grp.percentage, 0);


				}
				else
				{
					commandText = commandText + string.Format(" insert into GridDefinitionPercentageEntries values({0},{1},{2},{3},{4},{5},{6});",
					iMax, grp.sourceCol, grp.sourceRow, grp.targetCol, grp.targetRow, grp.percentage, 0);

					commandText = commandText + "END";
					fbCommand.CommandText = commandText;
					fbCommand.ExecuteNonQuery();
					commandText = "execute block as declare incidenceRateID int;" + " BEGIN ";

					i = 1;

				}
				i++;
				j++;

			}
		}
		public static void creatPercentageToDatabase(int big, int small, String popRasterLoc)
		{
			/*dpa 1/28/2017 - taking a chance here. Let's just comment out this code and instead call our new Crosswalk form.
			GridDefinition grd = new GridDefinition();
			Dictionary<string, List<GridRelationshipAttributePercentage>> dicAllGridPercentage = grd.getRelationshipFromBenMAPGridPercentage(big, small, popRasterLoc);
			updatePercentageToDatabase(dicAllGridPercentage.ToArray()[0]);
			 */
			CrosswalksConfiguration f = new CrosswalksConfiguration();
			f.StartPosition = FormStartPosition.CenterParent;
			f.Top = f.Top - 100; //shift it up a bit in case the "drawing layers" dialog is still showing. 
			f.RunCompact(big, small, null);
			CommonClass.IsAddPercentage = true;
			//dpa 1/28/2017 deleted unreachable code
		}

		public static void creatPercentageToDatabaseForSetup(int big, int small, String setupName)
		{
			CrosswalksConfiguration f = new CrosswalksConfiguration();
			f.StartPosition = FormStartPosition.CenterParent;
			f.Top = f.Top - 100; //shift it up a bit in case the "drawing layers" dialog is still showing. 
			f.RunCompact(big, small, setupName);
			CommonClass.IsAddPercentage = true;
		}

		public static List<string> getAllAgeID()
		{
			try
			{
				List<string> lstAgeID = new List<string>();
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				string commandText = string.Format("select * from AgeRanges");
				DataSet dsage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				foreach (DataRow dr in dsage.Tables[0].Rows)
				{
					lstAgeID.Add(Convert.ToString(dr["AgeRangeID"]));
				}
				return lstAgeID;
			}
			catch
			{
			}
			return null;
		}
		/// <summary>
		/// returns a population dictionary for the CR (Health Effects) Function
		/// </summary>
		/// <param name="diclstPopulationAttributeAge"></param>
		/// <param name="dicPop12"></param>
		/// <param name="crSelectFunction"></param>
		/// <param name="benMAPPopulation"></param>
		/// <param name="dicRace"></param>
		/// <param name="dicEthnicity"></param>
		/// <param name="dicGender"></param>
		/// <param name="GridDefinitionID"></param>
		/// <param name="gridRelationShipPopulation"></param>
		/// <returns></returns>
		public static Dictionary<int, float> getPopulationDataSetFromCRSelectFunction(ref Dictionary<string, float> diclstPopulationAttributeAge, ref Dictionary<int, float> dicPop12, CRSelectFunction crSelectFunction, BenMAPPopulation benMAPPopulation, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, int GridDefinitionID, GridRelationship gridRelationShipPopulation)
		{
			try
			{

				Dictionary<int, float> dicPopulationAttribute = new Dictionary<int, float>();
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				Dictionary<string, float> diclstPopulationAttribute = new Dictionary<string, float>();
				Dictionary<string, Dictionary<string, double>> dicPopweightfromPercentage = new Dictionary<string, Dictionary<string, double>>();

				string commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", benMAPPopulation.DataSetID); int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
				// HARDCODED SetupID != 1 (US)
				// sets the common year to the population year except for Setup 1 (US)
				if (CommonClass.MainSetup.SetupID != 1) commonYear = benMAPPopulation.Year;
				commandText = "";
				// HARDCODED SetupID = 1 (US) and AgeRangeID != 42 (0 to 99) 
				// exclude age range 0 to 99 from US setup (ONLY)
				//if (CommonClass.MainSetup.SetupID == 1)
				//    strwhere = "where AGERANGEID!=42";
				//else
				//    strwhere = " where 1=1 ";
				string ageCommandText = string.Format("select b.* from PopulationConfigurations a, Ageranges b   where a.PopulationConfigurationID=b.PopulationConfigurationID and a.PopulationConfigurationID=(select PopulationConfigurationID from PopulationDatasets where PopulationDataSetID=" + benMAPPopulation.DataSetID + ")");
				DataSet dsage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, ageCommandText);
				// next part of string appears to handle population growth ???
				string strsumage = "";
				string strsumageGrowth = "";
				foreach (DataRow dr in dsage.Tables[0].Rows)
				{
					if (strsumageGrowth == "")
						strsumageGrowth = dr["AgerangeID"].ToString();
					else
						strsumageGrowth = strsumageGrowth + "," + dr["AgerangeID"].ToString();
					if ((Convert.ToInt32(dr["StartAge"]) >= crSelectFunction.StartAge || crSelectFunction.StartAge == -1) && (Convert.ToInt32(dr["EndAge"]) <= crSelectFunction.EndAge || crSelectFunction.EndAge == -1))
					{
						if (strsumage == "")
							strsumage = dr["AgerangeID"].ToString();
						else
							strsumage = strsumage + "," + dr["AgerangeID"].ToString();
					}
					else
					{
						double dDiv = 1;
						if (Convert.ToInt32(dr["StartAge"]) < crSelectFunction.StartAge)
						{
							dDiv = Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - crSelectFunction.StartAge + 1) / Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]) + 1);
							if (Convert.ToInt32(dr["EndAge"]) > crSelectFunction.EndAge)
							{
								dDiv = Convert.ToDouble(crSelectFunction.EndAge - crSelectFunction.StartAge + 1) / Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]) + 1);

							}
						}
						else if (Convert.ToInt32(dr["EndAge"]) > crSelectFunction.EndAge)
						{
							dDiv = Convert.ToDouble(crSelectFunction.EndAge - Convert.ToInt32(dr["StartAge"]) + 1) / Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]) + 1);


						}

						if (commandText != "") commandText = commandText + " union ";
						// HARDCODED  - setupID = 1 (US) GridDefinitionID = 1 (no longer exists!)
						// DEADCODE - next if sttement can never be executed because no GridDefinitionID=1 exists in the BenMap-CE database
						if (benMAPPopulation.GridType.GridDefinitionID == 1 && CommonClass.MainSetup.SetupID == 1 && commonYear != benMAPPopulation.Year)
						{
							commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue)*" + dDiv + " as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
									"  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);


						}
						// HARDCODED - SetupID= 1 (US) and GridDefinitioID in 27, 28 (CMAQ 12km and clipped)
						else if ((benMAPPopulation.GridType.GridDefinitionID == 28 || benMAPPopulation.GridType.GridDefinitionID == 27) && CommonClass.MainSetup.SetupID == 1 && commonYear != benMAPPopulation.Year)
						{

							commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue*c.VValue*" + dDiv + ") as VValue   from PopulationEntries a,PopulationEntries b ," +
									" PopulationGrowthWeights c   where  b.PopulationDatasetID=2 and b.YYear={2} and a.RaceID=c.RaceID and  a.EthnicityID=c.EthnicityID and a.CColumn= c.TargetColumn  " +
" and a.Row=c.Targetrow  and b.CColumn= c.SourceColumn and b.Row= c.SourceRow and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear, CommonClass.BenMAPPopulation.Year);
						}
						// this case will run for all non US and US not using CMAQ grid definition
						else
						{
							commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue)*" + dDiv + " as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and YYear={1}", benMAPPopulation.DataSetID, commonYear);
						}
						// add filter for age range id
						commandText = string.Format(commandText + " and a.AgerangeID={0}", Convert.ToInt32(dr["AgerangeID"]));
						// add filter for 
						if (!string.IsNullOrEmpty(crSelectFunction.Race) && crSelectFunction.Race.ToLower() != "all")
						{
							if (dicRace.ContainsKey(crSelectFunction.Race))
							{ // HARDCODED - raceID=6 (empty string)
								// note that raceID=5 (ALL) is not included here
								commandText = string.Format(commandText + " and (a.RaceID={0} or a.RaceID=6)", dicRace[crSelectFunction.Race]);
							}
						}
						if (!string.IsNullOrEmpty(crSelectFunction.Ethnicity) && crSelectFunction.Ethnicity.ToLower() != "all")
						{
							if (dicEthnicity.ContainsKey(crSelectFunction.Ethnicity))
							{
								commandText = string.Format(commandText + " and (a.EthnicityID={0} or a.EthnicityID=4)", dicEthnicity[crSelectFunction.Ethnicity]);

							}
						}
						if (!string.IsNullOrEmpty(crSelectFunction.Gender) && crSelectFunction.Gender.ToLower() != "all")
						{
							if (dicGender.ContainsKey(crSelectFunction.Gender))
							{
								commandText = string.Format(commandText + " and (a.GenderID={0} or a.GenderID=4)", dicGender[crSelectFunction.Gender]);
							}
						}
						commandText = commandText + " group by a.CColumn,a.Row";
					}
				}
				if (commandText != "" && strsumage != "") commandText = commandText + " union ";
				if (strsumage != "")
				{
					if (benMAPPopulation.GridType.GridDefinitionID == 1 && CommonClass.MainSetup.SetupID == 1 && commonYear != benMAPPopulation.Year)
					{
						commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.VValue) as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
								"  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);

					}
					else if ((benMAPPopulation.GridType.GridDefinitionID == 28 || benMAPPopulation.GridType.GridDefinitionID == 27) && CommonClass.MainSetup.SetupID == 1 && commonYear != benMAPPopulation.Year)
					{
						commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue*c.VValue) as VValue   from PopulationEntries a,PopulationEntries b ," +
									" PopulationGrowthWeights c   where  b.PopulationDatasetID=2 and b.YYear={2} and a.RaceID=c.RaceID and  a.EthnicityID=c.EthnicityID and a.CColumn= c.TargetColumn  " +
" and a.Row=c.Targetrow  and b.CColumn= c.SourceColumn and b.Row= c.SourceRow and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear, CommonClass.BenMAPPopulation.Year);
					}
					else
					{
						commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue) as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and YYear={1}", benMAPPopulation.DataSetID, commonYear);
					}
					commandText = string.Format(commandText + " and a.AgerangeID in ({0}) ", strsumage);

					if (!string.IsNullOrEmpty(crSelectFunction.Race) && crSelectFunction.Race.Trim().ToLower() != "all")
					{
						if (dicRace[crSelectFunction.Race].ToString() != "")
						{
							commandText = string.Format(commandText + " and (a.RaceID={0} or a.RaceID=6)", dicRace[crSelectFunction.Race]);
						}
					}
					if (!string.IsNullOrEmpty(crSelectFunction.Ethnicity) && crSelectFunction.Ethnicity.Trim().ToLower() != "all")
					{
						if (dicEthnicity[crSelectFunction.Ethnicity].ToString() != "")
						{
							commandText = string.Format(commandText + " and (a.EthnicityID={0} or a.EthnicityID=4)", dicEthnicity[crSelectFunction.Ethnicity]);

						}
					}
					if (!string.IsNullOrEmpty(crSelectFunction.Gender) && crSelectFunction.Gender.Trim().ToLower() != "all")
					{
						if (dicGender[crSelectFunction.Gender].ToString() != "")
						{
							commandText = string.Format(commandText + " and (a.GenderID={0} or a.GenderID=4)", dicGender[crSelectFunction.Gender]);
						}
					}
					commandText = commandText + " group by a.CColumn,a.Row";
				}
				if (commandText != "")
				{
					commandText = "select   a.CColumn,a.Row,sum(a.vvalue) as VValue  from ( " + commandText + " ) a group by a.CColumn,a.Row";
				}
				int RaceID = -1;
				int EthnicityID = -1;
				int GenderID = -1;


				if (1 == 1)
				{
					Year = CommonClass.BenMAPPopulation.Year;

					if (!string.IsNullOrEmpty(crSelectFunction.Race))
					{
						if (dicRace.ContainsKey(crSelectFunction.Race) && dicRace[crSelectFunction.Race].ToString() != "" && crSelectFunction.Race.Trim().ToLower() != "all")
						{
							RaceID = dicRace[crSelectFunction.Race];

						}
					}
					if (!string.IsNullOrEmpty(crSelectFunction.Ethnicity))
					{
						if (dicEthnicity.ContainsKey(crSelectFunction.Ethnicity) && dicEthnicity[crSelectFunction.Ethnicity].ToString() != "" && crSelectFunction.Ethnicity.Trim().ToLower() != "all")
						{
							EthnicityID = dicEthnicity[crSelectFunction.Ethnicity];

						}
					}
					if (!string.IsNullOrEmpty(crSelectFunction.Gender))
					{
						if (dicGender.ContainsKey(crSelectFunction.Gender) && dicGender[crSelectFunction.Gender].ToString() != "" && crSelectFunction.Gender.Trim().ToLower() != "all")
						{
							GenderID = dicGender[crSelectFunction.Gender];
						}
					}
					FbDataReader fbDataReader = null;
					if (CommonClass.MainSetup.SetupID == 1)
					{
						string strGrowth = "select * from PopulationEntries where PopulationDataSetID=37 and YYear=" + CommonClass.BenMAPPopulation.Year + "  ";

						if (DicGrowth == null && CommonClass.BenMAPPopulation.Year != commonYear)
						{
							fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, strGrowth);
							DicGrowth = new Dictionary<string, double>();
							while (fbDataReader.Read())
							{
								if (!DicGrowth.ContainsKey(fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString() + "," + fbDataReader["EthnicityID"].ToString() + "," +
									 fbDataReader["RaceID"].ToString() + "," + fbDataReader["GenderID"].ToString() + "," + fbDataReader["AgeRangeID"].ToString()))
									DicGrowth.Add(fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString() + "," + fbDataReader["EthnicityID"].ToString() + "," +
											fbDataReader["RaceID"].ToString() + "," + fbDataReader["GenderID"].ToString() + "," + fbDataReader["AgeRangeID"].ToString(), Convert.ToDouble(fbDataReader["VValue"]));
								else
									DicGrowth[fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString() + "," + fbDataReader["EthnicityID"].ToString() + "," +
									fbDataReader["RaceID"].ToString() + "," + fbDataReader["GenderID"].ToString() + "," + fbDataReader["AgeRangeID"].ToString()] = DicGrowth[fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString() + "," + fbDataReader["EthnicityID"].ToString() + "," +
									fbDataReader["RaceID"].ToString() + "," + fbDataReader["GenderID"].ToString() + "," + fbDataReader["AgeRangeID"].ToString()] + Convert.ToDouble(fbDataReader["VValue"]);
							}
							fbDataReader.Dispose();
						}
						string strWeight = "select * from PopulationGrowthWeights where PopulationDataSetID=" + benMAPPopulation.DataSetID + " and YYear=" + commonYear;


						if (DicWeight == null && CommonClass.BenMAPPopulation.Year != commonYear && benMAPPopulation.GridType.GridDefinitionID != 18)
						{
							string strWeightCount = "select count(*) from PopulationGrowthWeights where PopulationDataSetID=" + benMAPPopulation.DataSetID + " and YYear=" + commonYear;
							int weightCount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, strWeightCount));
							if (weightCount > 0)
							{
								fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, strWeight);
								DicWeight = new Dictionary<string, Dictionary<string, WeightAttribute>>();
								while (fbDataReader.Read())
								{
									if (DicWeight.ContainsKey(fbDataReader["TargetColumn"].ToString() + "," + fbDataReader["TargetRow"].ToString()))
									{
										DicWeight[fbDataReader["TargetColumn"].ToString() + "," + fbDataReader["TargetRow"].ToString()].Add(fbDataReader["SourceColumn"].ToString() + "," + fbDataReader["SourceRow"].ToString() + "," + fbDataReader["EthnicityID"].ToString() + "," +
															fbDataReader["RaceID"].ToString(), new WeightAttribute() { RaceID = fbDataReader["RaceID"].ToString(), EthnicityID = fbDataReader["EthnicityID"].ToString(), Value = Convert.ToDouble(fbDataReader["VValue"]) });
									}
									else
									{
										DicWeight.Add(fbDataReader["TargetColumn"].ToString() + "," + fbDataReader["TargetRow"].ToString(), new Dictionary<string, WeightAttribute>());
										DicWeight[fbDataReader["TargetColumn"].ToString() + "," + fbDataReader["TargetRow"].ToString()].Add(fbDataReader["SourceColumn"].ToString() + "," + fbDataReader["SourceRow"].ToString() + "," + fbDataReader["EthnicityID"].ToString() + "," +
															fbDataReader["RaceID"].ToString(), new WeightAttribute() { RaceID = fbDataReader["RaceID"].ToString(), EthnicityID = fbDataReader["EthnicityID"].ToString(), Value = Convert.ToDouble(fbDataReader["VValue"]) });
									}


								}
								fbDataReader.Dispose();
							}
							else
							{
								string str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + benMAPPopulation.GridType.GridDefinitionID + " and  targetgriddefinitionid =18 ) and normalizationstate in (0,1)";
								DataSet dsPercentage = null;
								try
								{
									dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
									if (dsPercentage.Tables[0].Rows.Count == 0)
									{
										Configuration.ConfigurationCommonClass.creatPercentageToDatabase(18, benMAPPopulation.GridType.GridDefinitionID, null);
										dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
									}
									foreach (DataRow dr in dsPercentage.Tables[0].Rows)
									{
										if (dicPopweightfromPercentage.ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
										{
											if (!dicPopweightfromPercentage[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].ContainsKey(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()))
												dicPopweightfromPercentage[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["Percentage"]));
										}
										else
										{
											dicPopweightfromPercentage.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), new Dictionary<string, double>());
											dicPopweightfromPercentage[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["Percentage"]));
										}
									}
									dsPercentage.Dispose();

								}
								catch
								{ }
							}
						}
					}
					Dictionary<string, double> dicAge = new Dictionary<string, double>();
					string sAge = "";
					foreach (DataRow dr in dsage.Tables[0].Rows)
					{
						sAge += sAge == "" ? dr["AgeRangeID"].ToString() : "," + dr["AgeRangeID"].ToString();
						if ((Convert.ToInt32(dr["StartAge"]) >= crSelectFunction.StartAge || crSelectFunction.StartAge == -1) && (Convert.ToInt32(dr["EndAge"]) <= crSelectFunction.EndAge || crSelectFunction.EndAge == -1))
						{
							dicAge.Add(dr["AgeRangeID"].ToString(), 1);
						}
						else
						{
							double dDiv = 1;
							if (Convert.ToInt32(dr["StartAge"]) < crSelectFunction.StartAge)
							{
								dDiv = Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - crSelectFunction.StartAge + 1) / Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]) + 1);
								if (Convert.ToInt32(dr["EndAge"]) > crSelectFunction.EndAge)
								{
									dDiv = Convert.ToDouble(crSelectFunction.EndAge - crSelectFunction.StartAge + 1) / Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]) + 1);

								}
							}
							else if (Convert.ToInt32(dr["EndAge"]) > crSelectFunction.EndAge)
							{
								dDiv = Convert.ToDouble(crSelectFunction.EndAge - Convert.ToInt32(dr["StartAge"]) + 1) / Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]) + 1);


							}
							dicAge.Add(dr["AgeRangeID"].ToString(), 1);
							//YY: Proposed change: should use calculated weight instead of 1. Also, dDiv should always >=0
							//if (dDiv < 0) dDiv = 0;
							//dicAge.Add(dr["AgeRangeID"].ToString(), dDiv);
						}
					}
					dsage.Dispose();

					string strPop = "select * from PopulationEntries where PopulationDataSetID=" + benMAPPopulation.DataSetID + " and YYear=" + commonYear + " and AgeRangeID in (" + sAge + ")";
					fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, strPop);
					double d = 0;
					while (fbDataReader.Read())
					{
						d = 0; char[] c = new char[] { ',' };
						if (DicWeight != null && DicWeight.ContainsKey(fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString()) && dicAge.ContainsKey(fbDataReader["AgeRangeID"].ToString()) && DicGrowth != null && DicGrowth.Count > 0)
						{
							string se = fbDataReader["EthnicityID"].ToString(), sr = fbDataReader["RaceID"].ToString(), sg = fbDataReader["GenderID"].ToString(),
							 sga = fbDataReader["GenderID"].ToString() + "," + fbDataReader["AgeRangeID"], sa = fbDataReader["AgeRangeID"].ToString();
							foreach (KeyValuePair<string, WeightAttribute> k in DicWeight[fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString()])
							{
								if (k.Value.EthnicityID == se && k.Value.RaceID == sr && DicGrowth.ContainsKey(k.Key + "," + sga)
&& (RaceID == -1 || RaceID.ToString() == sr)
&& (GenderID == -1 || GenderID.ToString() == sg)
&& (EthnicityID == -1 || EthnicityID.ToString() == se)
)
									d += Convert.ToDouble(fbDataReader["VValue"]) * DicGrowth[k.Key + "," + sga] * k.Value.Value * dicAge[sa];


							}

						}
						else if (dicAge.ContainsKey(fbDataReader["AgeRangeID"].ToString()) && benMAPPopulation.GridType.GridDefinitionID == 18 && DicGrowth != null && DicGrowth.Count > 0)
						{
							if (DicGrowth.ContainsKey(fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString() + "," + fbDataReader["EthnicityID"].ToString() + "," +
										 fbDataReader["RaceID"].ToString() + "," + fbDataReader["GenderID"].ToString() + "," + fbDataReader["AgeRangeID"].ToString())
												 && (RaceID == -1 || RaceID.ToString() == fbDataReader["RaceID"].ToString())
												 && (GenderID == -1 || GenderID.ToString() == fbDataReader["GenderID"].ToString())
												 && (EthnicityID == -1 || EthnicityID.ToString() == fbDataReader["EthnicityID"].ToString()))
							{
								d = Convert.ToDouble(fbDataReader["VValue"]) * dicAge[fbDataReader["AgeRangeID"].ToString()] * DicGrowth[fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString() + "," + fbDataReader["EthnicityID"].ToString() + "," +
										fbDataReader["RaceID"].ToString() + "," + fbDataReader["GenderID"].ToString() + "," + fbDataReader["AgeRangeID"].ToString()];
							}
						}
						else if (dicPopweightfromPercentage != null && dicPopweightfromPercentage.Count > 0 && dicPopweightfromPercentage.ContainsKey(fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString()))
						{
							foreach (KeyValuePair<string, double> k in dicPopweightfromPercentage[fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString()])
							{
								if (DicGrowth.ContainsKey(k.Key + "," + fbDataReader["EthnicityID"] + "," + fbDataReader["RaceID"] + "," + fbDataReader["GenderID"] + "," + fbDataReader["AgeRangeID"].ToString())
										&& (RaceID == -1 || RaceID.ToString() == fbDataReader["RaceID"].ToString())
										&& (GenderID == -1 || GenderID.ToString() == fbDataReader["GenderID"].ToString())
										&& (EthnicityID == -1 || EthnicityID.ToString() == fbDataReader["EthnicityID"].ToString())
										)
									d += Convert.ToDouble(fbDataReader["VValue"]) * DicGrowth[k.Key + "," + fbDataReader["EthnicityID"] + "," + fbDataReader["RaceID"] + "," + fbDataReader["GenderID"].ToString() + "," + fbDataReader["AgeRangeID"].ToString()] * k.Value;
							}
						}
						else
						{
							if ((RaceID == -1 || RaceID.ToString() == fbDataReader["RaceID"].ToString())
												 && (GenderID == -1 || GenderID.ToString() == fbDataReader["GenderID"].ToString())
												 && (EthnicityID == -1 || EthnicityID.ToString() == fbDataReader["EthnicityID"].ToString()))
							{

								d = Convert.ToDouble(fbDataReader["VValue"]) * dicAge[fbDataReader["AgeRangeID"].ToString()];
							}

						}

						if (!diclstPopulationAttributeAge.ContainsKey(fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString() + "," + fbDataReader["AgeRangeID"].ToString()))
						{
							diclstPopulationAttributeAge.Add(fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString() + "," + fbDataReader["AgeRangeID"].ToString(), Convert.ToSingle(d));
						}
						else
						{
							diclstPopulationAttributeAge[fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString() + "," + fbDataReader["AgeRangeID"].ToString()] += Convert.ToSingle(d);
						}
						if (!diclstPopulationAttribute.ContainsKey(fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString()))
						{
							diclstPopulationAttribute.Add(fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString(), Convert.ToSingle(d));
						}
						else
						{
							diclstPopulationAttribute[fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString()] += Convert.ToSingle(d);
						}
					}// end while
					fbDataReader.Dispose();
					foreach (KeyValuePair<string, float> k in diclstPopulationAttribute)
					{
						string[] s = k.Key.Split(new char[] { ',' });
						dicPopulationAttribute.Add(Convert.ToInt32(s[0]) * 1000000 + Convert.ToInt32(s[1]), k.Value);
					}
					dicPop12 = dicPopulationAttribute;
					diclstPopulationAttribute = null;


				}
				else
				{

					FbDataReader fbDataReader2 = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, commandText);

					while (fbDataReader2.Read())
					{
						diclstPopulationAttribute.Add(fbDataReader2["CColumn"].ToString() + "," + fbDataReader2["Row"], Convert.ToSingle(fbDataReader2["VValue"]));
						dicPopulationAttribute.Add(Convert.ToInt32(fbDataReader2["CColumn"]) * 1000000 + Convert.ToInt32(fbDataReader2["Row"]), Convert.ToSingle(fbDataReader2["VValue"]));


					}
					dicPop12 = dicPopulationAttribute;
				}



				if (benMAPPopulation.GridType.GridDefinitionID == CommonClass.GBenMAPGrid.GridDefinitionID || ((benMAPPopulation.GridType.GridDefinitionID == 27 && CommonClass.GBenMAPGrid.GridDefinitionID == 28) || (benMAPPopulation.GridType.GridDefinitionID == 28 && CommonClass.GBenMAPGrid.GridDefinitionID == 27)))
				{ }
				else
				{
					string str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + (benMAPPopulation.GridType.GridDefinitionID == 28 ? 27 : benMAPPopulation.GridType.GridDefinitionID) + " and  targetgriddefinitionid = " + CommonClass.GBenMAPGrid.GridDefinitionID + " ) and normalizationstate in (0,1)";
					DataSet dsPercentage = null;
					Dictionary<string, Dictionary<string, double>> dicRelationShipForAggregation = new Dictionary<string, Dictionary<string, double>>();
					try
					{
						dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
						if (dsPercentage.Tables[0].Rows.Count == 0)
						{
							creatPercentageToDatabase(CommonClass.GBenMAPGrid.GridDefinitionID, (benMAPPopulation.GridType.GridDefinitionID == 28 ? 27 : benMAPPopulation.GridType.GridDefinitionID), null);
							dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
						}
						foreach (DataRow dr in dsPercentage.Tables[0].Rows)
						{
							if (dicRelationShipForAggregation.ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
							{
								if (!dicRelationShipForAggregation[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].ContainsKey(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()))
									dicRelationShipForAggregation[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["Percentage"]));
							}
							else
							{
								dicRelationShipForAggregation.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), new Dictionary<string, double>());
								dicRelationShipForAggregation[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["Percentage"]));
							}

						}

						dsPercentage.Dispose();


						Dictionary<string, float> dicPopulationAgeAggregation = new Dictionary<string, float>();
						foreach (KeyValuePair<string, float> k in diclstPopulationAttributeAge)
						{
							string[] s = k.Key.Split(new char[] { ',' });
							if (dicRelationShipForAggregation.ContainsKey(s[0] + "," + s[1]))
							{
								double dPop = 0;
								foreach (KeyValuePair<string, double> kin in dicRelationShipForAggregation[s[0] + "," + s[1]])
								{
									if (dicPopulationAgeAggregation.ContainsKey(kin.Key + "," + s[2]))
									{
										dicPopulationAgeAggregation[kin.Key + "," + s[2]] += Convert.ToSingle(k.Value * kin.Value);
									}
									else
									{
										dicPopulationAgeAggregation.Add(kin.Key + "," + s[2], Convert.ToSingle(k.Value * kin.Value));
									}

								}

							}
						}
						diclstPopulationAttributeAge.Clear();
						diclstPopulationAttributeAge = dicPopulationAgeAggregation;
					}
					catch
					{ }
				}
				if (benMAPPopulation.GridType.GridDefinitionID == GridDefinitionID || ((benMAPPopulation.GridType.GridDefinitionID == 28 || benMAPPopulation.GridType.GridDefinitionID == 27) && (GridDefinitionID == 27 || GridDefinitionID == 28)))

				{
					return dicPopulationAttribute;
				}
				else
				{
					Dictionary<int, float> dicPopulationAttributeReturn = new Dictionary<int, float>();
					Dictionary<string, Dictionary<string, double>> dicRelationShip = APVX.APVCommonClass.getRelationFromDicRelationShipAll(gridRelationShipPopulation);
					if (benMAPPopulation.GridType.GridDefinitionID == gridRelationShipPopulation.bigGridID)
					{
						if (dicRelationShip != null && dicRelationShip.Count > 0)
						{
							foreach (KeyValuePair<string, Dictionary<string, double>> k in dicRelationShip)
							{
								string[] s = k.Key.Split(new char[] { ',' });

								if (dicPopulationAttribute.Keys.Contains(Convert.ToInt32(s[0]) * 1000000 + Convert.ToInt32(s[1])))
								{
									double d = k.Value.Sum(p => p.Value);
									foreach (KeyValuePair<string, double> rc in k.Value)
									{
										string[] sin = rc.Key.Split(new char[] { ',' });
										if (!dicPopulationAttributeReturn.ContainsKey(Convert.ToInt32(sin[0]) * 1000000 + Convert.ToInt32(sin[1])))
											dicPopulationAttributeReturn.Add(Convert.ToInt32(sin[0]) * 1000000 + Convert.ToInt32(sin[1]), Convert.ToSingle(dicPopulationAttribute[Convert.ToInt32(s[0]) * 1000000 + Convert.ToInt32(s[1])] * rc.Value / d));
										else
											dicPopulationAttributeReturn[Convert.ToInt32(sin[0]) * 1000000 + Convert.ToInt32(sin[1])] += Convert.ToSingle(dicPopulationAttribute[Convert.ToInt32(s[0]) * 1000000 + Convert.ToInt32(s[1])] * rc.Value / d);
									}
								}
							}
						}
						else
						{
							foreach (GridRelationshipAttribute gra in gridRelationShipPopulation.lstGridRelationshipAttribute)
							{


								if (diclstPopulationAttribute.Keys.Contains(gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row))
								{
									foreach (RowCol rc in gra.smallGridRowCol)
									{
										dicPopulationAttributeReturn.Add(rc.Col * 1000000 + rc.Row, dicPopulationAttribute[gra.bigGridRowCol.Col * 1000000 + gra.bigGridRowCol.Row] / Convert.ToSingle(gra.smallGridRowCol.Count));
									}
								}

							}
						}
					}
					else
					{
						if (dicRelationShip != null && dicRelationShip.Count > 0)
						{
							foreach (KeyValuePair<string, Dictionary<string, double>> k in dicRelationShip)
							{
								string[] s = k.Key.Split(new char[] { ',' });
								double d = 0;

								foreach (KeyValuePair<string, double> rc in k.Value)
								{
									string[] sin = rc.Key.Split(new char[] { ',' });
									if (dicPopulationAttribute.ContainsKey(Convert.ToInt32(sin[0]) * 1000000 + Convert.ToInt32(sin[1])))
										d += dicPopulationAttribute[Convert.ToInt32(sin[0]) * 1000000 + Convert.ToInt32(sin[1])] * rc.Value;

								}
								if (d > 0)
								{
									dicPopulationAttributeReturn.Add(Convert.ToInt32(s[0]) * 1000000 + Convert.ToInt32(s[1]), Convert.ToSingle(d));
								}

							}
						}
						else
						{
							foreach (GridRelationshipAttribute gra in gridRelationShipPopulation.lstGridRelationshipAttribute)
							{

								if (!dicPopulationAttributeReturn.ContainsKey(gra.bigGridRowCol.Col * 1000000 + gra.bigGridRowCol.Row))
									dicPopulationAttributeReturn.Add(gra.bigGridRowCol.Col * 1000000 + gra.bigGridRowCol.Row, 0);
								foreach (RowCol rc in gra.smallGridRowCol)
								{
									if (gra.bigGridRowCol.Col == 13 && gra.bigGridRowCol.Row == 69)
									{
									}
									if (dicPopulationAttribute.Keys.Contains(rc.Col * 1000000 + rc.Row))
										dicPopulationAttributeReturn[gra.bigGridRowCol.Col * 1000000 + gra.bigGridRowCol.Row] += dicPopulationAttribute[rc.Col * 1000000 + rc.Row];
								}
							}
						}
					}// end else
					dicPopulationAttribute = dicPopulationAttributeReturn.Where(p => p.Value != 0).ToDictionary(p => p.Key, p => p.Value);
				}// end dumb else


				diclstPopulationAttribute = null;
				dsage.Dispose();
				return dicPopulationAttribute;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
		// what is going on here???????????????????????
		///<summary>returns a dictionary containing the age bins and their start and end ages</summary> 
		public static Dictionary<string, double> getDicAge(CRSelectFunction crSelectFunction)
		{
			ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
			string strwhere;
			// HARDCODED - SetupID=1, AgeRangeID = 42, 0 to 99
			// don't use age 0 to 99 for US Setup (SetupID = 1)
			if (CommonClass.MainSetup.SetupID == 1)
				strwhere = "where AGERANGEID!=42";
			else
				strwhere = " where 1=1 ";
			string ageCommandText = string.Format("select b.* from PopulationConfigurations a, Ageranges b   where a.PopulationConfigurationID=b.PopulationConfigurationID and a.PopulationConfigurationID=(select PopulationConfigurationID from PopulationDatasets where PopulationDataSetID=" + CommonClass.BenMAPPopulation.DataSetID + ")");
			if (crSelectFunction.StartAge != -1)
			{
				ageCommandText = string.Format(ageCommandText + " and b.EndAge>={0} ", crSelectFunction.StartAge);
			}
			if (crSelectFunction.EndAge != -1)
			{
				ageCommandText = string.Format(ageCommandText + " and b.StartAge<={0} ", crSelectFunction.EndAge);
			}
			DataSet dsage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, ageCommandText);
			Dictionary<string, double> dicAge = new Dictionary<string, double>();
			string sAge = "";
			foreach (DataRow dr in dsage.Tables[0].Rows)
			{
				sAge += sAge == "" ? dr["AgeRangeID"].ToString() : "," + dr["AgeRangeID"].ToString();
				if ((Convert.ToInt32(dr["StartAge"]) >= crSelectFunction.StartAge || crSelectFunction.StartAge == -1) && (Convert.ToInt32(dr["EndAge"]) <= crSelectFunction.EndAge || crSelectFunction.EndAge == -1))
				{
					dicAge.Add(dr["AgeRangeID"].ToString(), 1);
				}
				else
				{
					double dDiv = 1;
					if (Convert.ToInt32(dr["StartAge"]) < crSelectFunction.StartAge)
					{
						dDiv = Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - crSelectFunction.StartAge + 1) / Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]) + 1);
						if (Convert.ToInt32(dr["EndAge"]) > crSelectFunction.EndAge)
						{
							dDiv = Convert.ToDouble(crSelectFunction.EndAge - crSelectFunction.StartAge + 1) / Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]) + 1);

						}
					}
					else if (Convert.ToInt32(dr["EndAge"]) > crSelectFunction.EndAge)
					{
						dDiv = Convert.ToDouble(crSelectFunction.EndAge - Convert.ToInt32(dr["StartAge"]) + 1) / Convert.ToDouble(Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]) + 1);


					}
					dicAge.Add(dr["AgeRangeID"].ToString(), dDiv);
				}
			}
			dsage.Dispose();
			return dicAge;
		}
		public static Dictionary<string, double> getIncidenceDataSetFromCRSelectFuntionDicAllAge(Dictionary<string, double> dicAge, Dictionary<string, float> dicPopulationAge, Dictionary<int, float> dicPopulation12, CRSelectFunction crSelectFunction, bool bPrevalence, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, int GridDefinitionID, GridRelationship gridRelationShipPopulation)
		{
			try
			{

				Dictionary<int, double> dicIncidenceRateAttribute = new Dictionary<int, double>();
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				// choose incidence or prevalence data set
				string strbPrevalence = "F";
				int iid = crSelectFunction.IncidenceDataSetID;
				if (bPrevalence)
				{
					strbPrevalence = "T";
					iid = crSelectFunction.PrevalenceDataSetID;
				}
				string commandText = "";

				int iIncidenceDataSetGridID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, string.Format("select GridDefinitionID from IncidenceDataSets where IncidenceDataSetID={1} ", CommonClass.MainSetup.SetupID, iid)));

				commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", CommonClass.BenMAPPopulation.DataSetID); int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
				commandText = "";
				// HARDCODED - use Empty string for race
				string strRace = "";
				string strEthnicity = "";
				string strGender = "";
				// this is performing a mapping from incidence age bins to population age bins -AS
				// BF-531 - check to see if user wants to use average or filtered incidence rates
				if (ConfigurationCommonClass.incidenceAvgSelected != incidenceAveraging.averageAll)
				{
					// add filter for race
					//strRace = " and (b.RaceID=6)";
					if (!string.IsNullOrEmpty(crSelectFunction.Race) && (crSelectFunction.Race.ToLower() != "all"))
					{
						if (dicRace.ContainsKey(crSelectFunction.Race))
						{
							int raceID = dicRace[crSelectFunction.Race];

							//string raceSQL = String.Format("SELECT count(*) FROM INCIDENCERATES where incidencedatasetid = {0} and raceid = {1}", iid, raceID);
							//int raceCount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, raceSQL));
							//if (raceCount > 0)
							//{
							//if incidence exists, then use it
							strRace = string.Format(" and (b.RaceID={0})", raceID);
							//}
						}
					}
					else //If advanced setting (exact match subgroup) is selected and the function has "ALL" or blank selected
					{
						//Check if subgroups overlap each other. If yes, treat "ALL" or blank as subgroup name, otherwise, average all.
						int raceIDall = 0;
						int raceIDblank = 0;
						if (dicRace.ContainsKey("")) raceIDblank = dicRace[""];
						if (dicRace.ContainsKey("ALL")) raceIDall = dicRace["ALL"];
						if (raceIDblank > 0 || raceIDall > 0)
						{
							commandText = string.Format(@"select count(*) from (with tmp as (SELECT a.ENDPOINTGROUPID, a.ENDPOINTID, a.STARTAGE, a.ENDAGE, b.RACENAME 
FROM INCIDENCERATES a inner join RACES b ON a.RACEID = b.RACEID 
WHERE(lower(b.RACENAME) = 'all' or b.RACENAME = '') and a.INCIDENCEDATASETID = {0}) 
SELECT a.ENDPOINTGROUPID, a.ENDPOINTID, a.STARTAGE, a.ENDAGE, b.RACENAME 
FROM INCIDENCERATES a inner join RACES b ON a.RACEID = b.RACEID inner join tmp 
on a.ENDPOINTGROUPID = tmp.ENDPOINTGROUPID AND a.ENDPOINTID = tmp.ENDPOINTID and a.STARTAGE = tmp.STARTAGE and a.ENDAGE = tmp.ENDAGE 
WHERE(lower(b.RACENAME) != 'all' and b.RACENAME != '') and a.INCIDENCEDATASETID = {0})", iid);

							if (Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) > 0)

							{
								if (raceIDall == 0)
								{
									strRace = string.Format(" and b.RaceID={0})", raceIDblank);
								}
								else if (raceIDblank == 0)
								{
									strRace = string.Format(" and b.RaceID={0})", raceIDall);
								}
								else
								{
									strRace = string.Format(" and (b.RaceID={0} or b.RaceID={1})", raceIDall, raceIDblank);
								}
							}

						}
					}
					//add filter for ethnicity
					//strEthnicity = " and (b.EthnicityID=4)";
					if (!string.IsNullOrEmpty(crSelectFunction.Ethnicity) && (crSelectFunction.Ethnicity.ToLower() != "all"))
					{
						if (dicEthnicity.ContainsKey(crSelectFunction.Ethnicity))
						{
							int ethnicityID = dicEthnicity[crSelectFunction.Ethnicity];

							//if incidence exists, then use it
							//string ethnicitySQL = String.Format("SELECT count(*) FROM INCIDENCERATES where incidencedatasetid = {0} and ethnicityid = {1}", iid, ethnicityID);
							//int ethnicityCount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, ethnicitySQL));
							//if (ethnicityCount > 0)
							//{
							strEthnicity = string.Format(" and (b.EthnicityID={0})", ethnicityID);
							//}
						}
					}
					else //ALL or blank selected for function
					{
						int ethnicityIDall = 0;
						int ethnicityIDblank = 0;
						if (dicEthnicity.ContainsKey("")) ethnicityIDblank = dicEthnicity[""];
						if (dicEthnicity.ContainsKey("ALL")) ethnicityIDall = dicEthnicity["ALL"];
						//if all or blank is in ethnicity dictionary, check if ethnicity has overlapping groups. If yes, filter by all or blank, otherwise, use average.
						if (ethnicityIDblank > 0 || ethnicityIDall > 0)
						{
							commandText = string.Format(@"select count(*) from (with tmp as (SELECT a.ENDPOINTGROUPID, a.ENDPOINTID, a.STARTAGE, a.ENDAGE, b.ETHNICITYNAME 
FROM INCIDENCERATES a inner join ETHNICITY b ON a.ETHNICITYID = b.ETHNICITYID 
WHERE(lower(b.ETHNICITYNAME) = 'all' or b.ETHNICITYNAME = '') and a.INCIDENCEDATASETID = {0}) 
SELECT a.ENDPOINTGROUPID, a.ENDPOINTID, a.STARTAGE, a.ENDAGE, b.ETHNICITYNAME 
FROM INCIDENCERATES a inner join ETHNICITY b ON a.ETHNICITYID = b.ETHNICITYID inner join tmp 
on a.ENDPOINTGROUPID = tmp.ENDPOINTGROUPID AND a.ENDPOINTID = tmp.ENDPOINTID and a.STARTAGE = tmp.STARTAGE and a.ENDAGE = tmp.ENDAGE 
WHERE(lower(b.ETHNICITYNAME) != 'all' and b.ETHNICITYNAME != '') and a.INCIDENCEDATASETID = {0})", iid);

							if (Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) > 0)
							{
								if (ethnicityIDall == 0)
								{
									strEthnicity = string.Format(" and b.EthnicityID={0})", ethnicityIDblank);
								}
								else if (ethnicityIDblank == 0)
								{
									strEthnicity = string.Format(" and b.EthnicityID={0})", ethnicityIDall);
								}
								else
								{
									strEthnicity = string.Format(" and (b.EthnicityID={0} or b.EthnicityID={1})", ethnicityIDall, ethnicityIDblank);
								}
							}

						}
					}

					if (!string.IsNullOrEmpty(crSelectFunction.Gender) && (crSelectFunction.Gender.ToLower() != "all"))
					{
						if (dicGender.ContainsKey(crSelectFunction.Gender))
						{
							int genderID = dicGender[crSelectFunction.Gender];

							//if incidence exists, then use it
							//string genderSQL = String.Format("SELECT count(*) FROM INCIDENCERATES where incidencedatasetid = {0} and genderid = {1}", iid, genderID);
							//int genderCount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, genderSQL));
							//if (genderCount > 0)
							//{
							strGender = string.Format(" and (b.GenderID={0})", genderID);
							//}
						}
					}
					else //ALL or blank selected for function
					{
						int genderIDall = 0;
						int genderIDblank = 0;
						if (dicGender.ContainsKey("")) genderIDblank = dicGender[""];
						if (dicGender.ContainsKey("ALL")) genderIDall = dicGender["ALL"];
						//if all or blank is in gender dictionary, check if gender has overlapping groups. If yes, filter by all or blank, otherwise, use average.
						if (genderIDblank > 0 || genderIDall > 0)
						{
							commandText = string.Format(@"select count(*) from (with tmp as (SELECT a.ENDPOINTGROUPID, a.ENDPOINTID, a.STARTAGE, a.ENDAGE, b.GENDERNAME 
FROM INCIDENCERATES a inner join GENDERS b ON a.GENDERID = b.GENDERID 
WHERE(lower(b.GENDERNAME) = 'all' or b.GENDERNAME = '') and a.INCIDENCEDATASETID = {0}) 
SELECT a.ENDPOINTGROUPID, a.ENDPOINTID, a.STARTAGE, a.ENDAGE, b.GENDERNAME 
FROM INCIDENCERATES a inner join GENDERS b ON a.GENDERID = b.GENDERID inner join tmp 
on a.ENDPOINTGROUPID = tmp.ENDPOINTGROUPID AND a.ENDPOINTID = tmp.ENDPOINTID and a.STARTAGE = tmp.STARTAGE and a.ENDAGE = tmp.ENDAGE 
WHERE(lower(b.GENDERNAME) != 'all' and b.GENDERNAME != '') and a.INCIDENCEDATASETID = {0})", iid);

							if (Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) > 0)

							{
								if (genderIDall == 0)
								{
									strGender = string.Format(" and b.GenderID={0})", genderIDblank);
								}
								else if (genderIDblank == 0)
								{
									strGender = string.Format(" and b.GenderID={0})", genderIDall);
								}
								else
								{
									strGender = string.Format(" and (b.GenderID={0} or b.GenderID={1})", genderIDall, genderIDblank);
								}
							}

						}
					}
				}


				// add filters to restrict age ranges
				string strbEndAgeOri = " CASE" +
							 " WHEN (b.EndAge> " + crSelectFunction.EndAge + ") THEN " + crSelectFunction.EndAge + " ELSE b.EndAge END ";
				string strbStartAgeOri = " CASE" +
						" WHEN (b.StartAge< " + crSelectFunction.StartAge + ") THEN " + crSelectFunction.StartAge + " ELSE b.StartAge END ";
				string straEndAgeOri = " CASE" +
							" WHEN (a.EndAge> " + crSelectFunction.EndAge + ") THEN " + crSelectFunction.EndAge + " ELSE a.EndAge END ";
				string straStartAgeOri = " CASE" +
						" WHEN (a.StartAge< " + crSelectFunction.StartAge + ") THEN " + crSelectFunction.StartAge + " ELSE a.StartAge END ";
				// ratio the overlap of the age range as a weight
				string strAgeID = string.Format(" select a.startAge,a.EndAge,b.AgeRangeid, " +
" CASE" +
" WHEN (b.startAge>=a.StartAge and b.EndAge<=a.EndAge) THEN 1" +
" WHEN (b.startAge<a.StartAge and b.EndAge<=a.EndAge and ({3}-{2}+1)>0) THEN  Cast(({3}-{0}+1) as float)/({3}-{2}+1)" + " WHEN (b.startAge<a.StartAge and b.EndAge>a.EndAge and ({3}-{2}+1)>0) THEN Cast(({1}-{0}+1) as float)/({3}-{2}+1)" +
"  WHEN (b.startAge>=a.StartAge and b.EndAge>a.EndAge and ({3}-{2}+1)>0) THEN Cast(({1}-{2}+1) as float)/({3}-{2}+1)" +
" WHEN ({3}-{2}+1)<=0 THEN 0 " +
" ELSE 1" +
" END as weight,b.StartAge as sourceStartAge,b.EndAge as SourceEndAge" +
"  from ( select distinct startage,endage from Incidencerates  where IncidenceDataSetID=" + iid + ")a,ageranges b" +
" where b.EndAge>=a.StartAge and b.StartAge<=a.EndAge and b.PopulationConfigurationID={4}", straStartAgeOri, straEndAgeOri, strbStartAgeOri, strbEndAgeOri, CommonClass.BenMAPPopulation.PopulationConfiguration);
				// average incidence rates if there are multiple demographic groups.
				string strIncAvg = string.Format("select  a.CColumn,a.Row,b.STARTAGE, b.ENDAGE, b.ENDPOINTGROUPID, b.ENDPOINTID, b.PREVALENCE, avg(a.VValue) as VValue " +
"from IncidenceEntries a inner " +
"join IncidenceRates b on a.INCIDENCERATEID = b.INCIDENCERATEID " +
"inner join IncidenceDatasets c on b.INCIDENCEDATASETID = c.INCIDENCEDATASETID " +
"where c.INCIDENCEDATASETID = {0} " + strRace + strEthnicity + strGender +
" group by a.CColumn, a.Row, b.STARTAGE, b.ENDAGE, b.ENDPOINTGROUPID, b.ENDPOINTID, b.PREVALENCE ", iid);


				// HARDCODED - b.EndPointID=99 (Empty String in Endpoint GroupID 4, "Asthma Exacerbation") or b.EndPointID=100 (Empty String in Endpoint Group 6, "Chronic Bronchitis") or b.EndPointID=102 (Empty String in Endpoint Group 14, "Upper Respiratory Symptoms")
				//updated query strInc so that it uses incidence data which is already grouped by age range (strIncAvg).
				string strInc = string.Format("select  IncAvg.CColumn,IncAvg.Row,sum(IncAvg.VValue*d.Weight) as VValue,d.AgeRangeID  from (" + strIncAvg + ") IncAvg ,(" + strAgeID +
						 ") d where   IncAvg.StartAge=d.StartAge and IncAvg.EndAge=d.EndAge and " +
		 "  IncAvg.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (IncAvg.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or IncAvg.EndPointID=99 or IncAvg.EndPointID=100 or IncAvg.EndPointID=102)" + " and IncAvg.Prevalence='" + strbPrevalence + "' " +
		 "  and IncAvg.StartAge<={2} and IncAvg.EndAge>={1} group by IncAvg.CColumn,IncAvg.Row ,d.AgeRangeID", iid, crSelectFunction.StartAge, crSelectFunction.EndAge);
				DataSet dsInc = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, strInc);

				Dictionary<string, double> dicInc = new Dictionary<string, double>();
				foreach (DataRow dr in dsInc.Tables[0].Rows)
				{   // HARDCODED - multiply column by 1000000 and add row to convert row column to a single number for hash
					if (!dicInc.ContainsKey((Convert.ToInt32(dr["CColumn"]) * 1000000 + Convert.ToInt32(dr["Row"])).ToString() + "," + dr["AgeRangeID"]))
					{
						dicInc.Add((Convert.ToInt32(dr["CColumn"]) * 1000000 + Convert.ToInt32(dr["Row"])).ToString() + "," + dr["AgeRangeID"].ToString(), Convert.ToDouble(dr["VValue"]));
					}
				}
				dsInc.Dispose();
				if (iIncidenceDataSetGridID == CommonClass.GBenMAPGrid.GridDefinitionID) return dicInc;
				Dictionary<string, Dictionary<string, double>> dicPercentageForAggregationInc = new Dictionary<string, Dictionary<string, double>>();
				try
				{
					// HARDCODED - requires normalization state to be in 0, 1 
					// HARDCODED - source grid definition ID in 27 (CMAQ 12km Nation - Clipped) or 28 (CMAQ 12km Nation ???

					string str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + (CommonClass.GBenMAPGrid.GridDefinitionID == 28 ? 27 : CommonClass.GBenMAPGrid.GridDefinitionID) + " and  targetgriddefinitionid = " + iIncidenceDataSetGridID + " ) and normalizationstate in (0,1)";

					DataSet dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
					if (dsPercentage.Tables[0].Rows.Count == 0) // no crosswalk rows found - need to generate them 
					{
						creatPercentageToDatabase(iIncidenceDataSetGridID, CommonClass.GBenMAPGrid.GridDefinitionID, null);
						int iPercentageID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, "select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + CommonClass.GBenMAPGrid.GridDefinitionID + " and  targetgriddefinitionid = " + iIncidenceDataSetGridID));
						str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + (CommonClass.GBenMAPGrid.GridDefinitionID == 28 ? 27 : CommonClass.GBenMAPGrid.GridDefinitionID) + " and  targetgriddefinitionid = " + iIncidenceDataSetGridID + " ) and normalizationstate in (0,1)";
						dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
					}
					foreach (DataRow dr in dsPercentage.Tables[0].Rows)
					{
						if (dicPercentageForAggregationInc.ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
						{
							if (!dicPercentageForAggregationInc[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].ContainsKey(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()))
								dicPercentageForAggregationInc[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["Percentage"]));
						}
						else
						{
							dicPercentageForAggregationInc.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), new Dictionary<string, double>());
							dicPercentageForAggregationInc[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["Percentage"]));
						}

					}

					Dictionary<string, double> dicReturn = new Dictionary<string, double>();
					//A few notes:
					//string[] sin:                  [target col, target row, pct] 
					//KeyValuePair<string, float> k: k.key="source col, source row, ageId" --- k.value = population
					//string[] s:                    [source col, source row, ageId] 
					//double dsin:                   target grid ID
					foreach (KeyValuePair<string, float> k in dicPopulationAge)
					{
						string[] s = k.Key.Split(new char[] { ',' });
						if (!dicAge.ContainsKey(s[2])) continue;
						if (dicPercentageForAggregationInc.ContainsKey(s[0] + "," + s[1]))
						{
							foreach (KeyValuePair<string, double> kin in dicPercentageForAggregationInc[s[0] + "," + s[1]])
							{
								string[] sin = kin.Key.Split(new char[] { ',' });
								double dsin = Convert.ToInt32(sin[0]) * 1000000 + Convert.ToInt32(sin[1]);
								if (!dicInc.ContainsKey(dsin + "," + s[2])) continue;
								if (dicReturn.ContainsKey((Convert.ToInt32(s[0]) * 1000000 + Convert.ToInt32(s[1])).ToString() + "," + s[2]))
									dicReturn[(Convert.ToInt32(s[0]) * 1000000 + Convert.ToInt32(s[1])).ToString() + "," + s[2]] += dicInc[dsin + "," + s[2]] * kin.Value;
								else
									dicReturn.Add((Convert.ToInt32(s[0]) * 1000000 + Convert.ToInt32(s[1])).ToString() + "," + s[2], dicInc[dsin + "," + s[2]] * kin.Value);
							}
						}

					}
					return dicReturn;

				}
				catch (Exception ex)
				{
					return null;
				}
			}
			catch (Exception ex)
			{
				return null;
			}

		}

		public static List<Tuple<string, int>> getAllSystemVariableNameList() //Expanded to include Variable Dataset ID to avoid issues with similar/duplicate entries
		{
			try
			{
				List<Tuple<string, int>> lstResult = new List<Tuple<string, int>>();
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
				string commandText = "select distinct SetupVariableName, SetupVariableDatasetID from SetupVariables where setupvariabledatasetid in(select setupvariabledatasetid from setupvariabledatasets where setupid = " + CommonClass.MainSetup.SetupID + ")";
				DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					lstResult.Add(new Tuple<string, int>(dr[0].ToString(), Convert.ToInt32(dr[1])));
				}
				return lstResult;
			}
			catch (Exception ex)
			{
				return null;
			}

		}


		private static List<Tuple<string, int>> lstSystemVariableName;
		public static List<Tuple<string, int>> LstSystemVariableName
		{
			get
			{
				if (lstSystemVariableName == null)
				{
					lstSystemVariableName = getAllSystemVariableNameList();
				}
				return lstSystemVariableName;
			}
		}
		public static Dictionary<string, Dictionary<string, float>> getAllMetricDataFromBaseControlGroup(BaseControlGroup baseControlGroup, bool isBase, ref Dictionary<string, Dictionary<string, List<float>>> dicAll365)
		{
			Dictionary<string, Dictionary<string, float>> dicReturn = new Dictionary<string, Dictionary<string, float>>();
			List<float> lstTemp = new List<float>();
			try
			{
				BenMAPLine benMapLine = null;
				if (isBase)
				{
					benMapLine = baseControlGroup.Base;
				}
				else
				{
					benMapLine = baseControlGroup.Control;
				}
				Dictionary<string, string> dicMetricAll = new Dictionary<string, string>();
				if (baseControlGroup.Pollutant.Metrics != null)
				{
					foreach (Metric m in baseControlGroup.Pollutant.Metrics)
					{
						dicMetricAll.Add(m.MetricName, Enum.GetName(typeof(MetricStatic), m is MovingWindowMetric ? (m as MovingWindowMetric).WindowStatistic : m is FixedWindowMetric ? (m as FixedWindowMetric).Statistic : MetricStatic.Mean));
					}
				}
				if (baseControlGroup.Pollutant.SesonalMetrics != null)
				{
					foreach (SeasonalMetric s in baseControlGroup.Pollutant.SesonalMetrics)
					{
						dicMetricAll.Add(s.SeasonalMetricName, Enum.GetName(typeof(MetricStatic), MetricStatic.Mean));
					}
				}
				foreach (ModelResultAttribute m in benMapLine.ModelResultAttributes)
				{
					dicReturn.Add(m.Col + "," + m.Row, m.Values);
					Dictionary<string, float> dicAdd = new Dictionary<string, float>();
					foreach (KeyValuePair<string, float> k in m.Values)
					{
						if (!k.Key.Contains(",") && !m.Values.ContainsKey(k.Key + "," + dicMetricAll[k.Key]))
						{
							dicAdd.Add(k.Key + "," + dicMetricAll[k.Key], k.Value);
						}
					}
					foreach (KeyValuePair<string, float> k in dicAdd)
					{
						dicReturn[m.Col + "," + m.Row].Add(k.Key, k.Value);
					}
				}
				if (benMapLine.ModelAttributes != null)
				{
					int globalSeasonStartDay = 365;
					int globalSeasonEndDay = 0;
					foreach (Season pollutantSeason in benMapLine.Pollutant.Seasons)
					{
						if (pollutantSeason.StartDay < globalSeasonStartDay)
							globalSeasonStartDay = pollutantSeason.StartDay;
						if (pollutantSeason.EndDay > globalSeasonEndDay)
							globalSeasonEndDay = pollutantSeason.EndDay;
					}

					foreach (ModelAttribute m in benMapLine.ModelAttributes)
					{
						if (m.Values.Count.Equals(366))
						{
							if (globalSeasonStartDay > 59) //if the start of the season occurs after Feb 29
								globalSeasonStartDay = benMapLine.Pollutant.Seasons.First().StartDay + 1; //increase start day by 1
							if (globalSeasonEndDay > 59) //if the end of the season occurs after Feb 29
								globalSeasonEndDay = benMapLine.Pollutant.Seasons.Last().EndDay + 1; //increase end day by 1
						}
						if (m.SeasonalMetric != null)
						{
							if (!dicAll365.ContainsKey(m.Col + "," + m.Row))
							{
								dicAll365.Add(m.Col + "," + m.Row, new Dictionary<string, List<float>>());
							}
							if (!dicAll365[m.Col + "," + m.Row].ContainsKey(m.SeasonalMetric.SeasonalMetricName))
							{
								dicAll365[m.Col + "," + m.Row].Add(m.SeasonalMetric.SeasonalMetricName, m.Values);
							}
							if (dicReturn.ContainsKey(m.Col + "," + m.Row))
							{
								lstTemp = m.Values
										.GetRange(globalSeasonStartDay, globalSeasonEndDay - globalSeasonStartDay + 1)
										.Where(p => p != float.MinValue)
										.ToList();
								if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.SeasonalMetric.SeasonalMetricName + "," + "Mean"))
								{

									dicReturn[m.Col + "," + m.Row].Add(m.SeasonalMetric.SeasonalMetricName + "," + "Mean", lstTemp.Count == 0 ? float.MinValue : lstTemp.Average());
								}
								else
								{
									dicReturn[m.Col + "," + m.Row][m.SeasonalMetric.SeasonalMetricName + "," + "Mean"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.Average();
								}
								if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.SeasonalMetric.SeasonalMetricName + "," + "Median"))
								{
									dicReturn[m.Col + "," + m.Row].Add(m.SeasonalMetric.SeasonalMetricName + "," + "Median", lstTemp.Count == 0 ? float.MinValue : lstTemp.OrderBy(p => p).Median());
								}
								else
								{
									dicReturn[m.Col + "," + m.Row][m.SeasonalMetric.SeasonalMetricName + "," + "Median"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.OrderBy(p => p).Median();
								}
								if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.SeasonalMetric.SeasonalMetricName + "," + "Max"))
								{
									dicReturn[m.Col + "," + m.Row].Add(m.SeasonalMetric.SeasonalMetricName + "," + "Max", lstTemp.Count == 0 ? float.MinValue : lstTemp.Max());
								}
								else
									dicReturn[m.Col + "," + m.Row][m.SeasonalMetric.SeasonalMetricName + "," + "Max"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.Max();
								if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.SeasonalMetric.SeasonalMetricName + "," + "Min"))
								{
									dicReturn[m.Col + "," + m.Row].Add(m.SeasonalMetric.SeasonalMetricName + "," + "Min", lstTemp.Count == 0 ? float.MinValue : lstTemp.Min());
								}
								else
									dicReturn[m.Col + "," + m.Row][m.SeasonalMetric.SeasonalMetricName + "," + "Min"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.Min();
								if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.SeasonalMetric.SeasonalMetricName + "," + "Sum"))
								{
									dicReturn[m.Col + "," + m.Row].Add(m.SeasonalMetric.SeasonalMetricName + "," + "Sum", lstTemp.Count == 0 ? float.MinValue : lstTemp.Sum());
								}
								else
									dicReturn[m.Col + "," + m.Row][m.SeasonalMetric.SeasonalMetricName + "," + "Sum"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.Sum();
							}

						}
						else if (m.Metric != null)
						{
							if (!dicAll365.ContainsKey(m.Col + "," + m.Row))
							{
								dicAll365.Add(m.Col + "," + m.Row, new Dictionary<string, List<float>>());
							}
							if (!dicAll365[m.Col + "," + m.Row].ContainsKey(m.Metric.MetricName))
							{
								dicAll365[m.Col + "," + m.Row].Add(m.Metric.MetricName, m.Values);
							}
							lstTemp = m.Values
										.GetRange(globalSeasonStartDay, globalSeasonEndDay - globalSeasonStartDay + 1)
										.Where(p => p != float.NaN && p != float.MinValue)
										.ToList();
							if (dicReturn.ContainsKey(m.Col + "," + m.Row))
							{
								if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.Metric.MetricName + "," + "Mean"))
								{
									dicReturn[m.Col + "," + m.Row].Add(m.Metric.MetricName + "," + "Mean", lstTemp.Count == 0 ? float.MinValue : lstTemp.Average());
								}
								else
									dicReturn[m.Col + "," + m.Row][m.Metric.MetricName + "," + "Mean"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.Average();
								if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.Metric.MetricName + "," + "Median"))
								{
									dicReturn[m.Col + "," + m.Row].Add(m.Metric.MetricName + "," + "Median", lstTemp.Count == 0 ? float.MinValue : lstTemp.OrderBy(p => p).Median());
								}
								else
									dicReturn[m.Col + "," + m.Row][m.Metric.MetricName + "," + "Median"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.OrderBy(p => p).Median();
								if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.Metric.MetricName + "," + "Max"))
								{
									dicReturn[m.Col + "," + m.Row].Add(m.Metric.MetricName + "," + "Max", lstTemp.Count == 0 ? float.MinValue : lstTemp.Max());
								}
								else
									dicReturn[m.Col + "," + m.Row][m.Metric.MetricName + "," + "Max"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.Max();
								if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.Metric.MetricName + "," + "Min"))
								{
									dicReturn[m.Col + "," + m.Row].Add(m.Metric.MetricName + "," + "Min", lstTemp.Count == 0 ? float.MinValue : lstTemp.Min());
								}
								else
									dicReturn[m.Col + "," + m.Row][m.Metric.MetricName + "," + "Min"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.Min();
								if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.Metric.MetricName + "," + "Sum"))
								{
									dicReturn[m.Col + "," + m.Row].Add(m.Metric.MetricName + "," + "Sum", lstTemp.Count == 0 ? float.MinValue : lstTemp.Sum());
								}
								else
									dicReturn[m.Col + "," + m.Row][m.Metric.MetricName + "," + "Sum"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.Sum();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
			return dicReturn;
		}
		public static void CalculateOneCRSelectFunction(string sCRID, List<string> lstAllAgeID, Dictionary<string, double> dicAge, Dictionary<string, Dictionary<string, float>> dicBaseMetricData, Dictionary<string, Dictionary<string, float>> dicControlMetricData,
 Dictionary<string, Dictionary<string, List<float>>> dicBase365, Dictionary<string, Dictionary<string, List<float>>> dicControl365,
 Dictionary<string, ModelResultAttribute> dicControl, Dictionary<string, Dictionary<string, double>> DicAllSetupVariableValues, Dictionary<string, float> dicPopulationAllAge, Dictionary<string, double> dicIncidenceRateAttribute,
 Dictionary<string, double> dicPrevalenceRateAttribute, int incidenceDataSetGridType, int PrevalenceDataSetGridType, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, double Threshold, int LatinHypercubePoints,
 bool RunInPointMode, List<GridRelationship> lstGridRelationship, CRSelectFunction crSelectFunction, Dictionary<string, double> dicGeoAreaPercentages, BaseControlGroup baseControlGroup, List<RegionTypeGrid> lstRegionTypeGrid, BenMAPPopulation benMAPPopulation, double[] lhsResultArray)
		{
			try
			{

				CRSelectFunctionCalculateValue crSelectFunctionCalculateValue = new CRSelectFunctionCalculateValue() { CRSelectFunction = crSelectFunction, CRCalculateValues = new List<CRCalculateValue>() };

				try
				{
					if (benMAPPopulation.GridType.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
						lstGridRelationship.Where(p => (p.bigGridID == benMAPPopulation.GridType.GridDefinitionID && p.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID) || (p.smallGridID == benMAPPopulation.GridType.GridDefinitionID && p.bigGridID == CommonClass.GBenMAPGrid.GridDefinitionID)).First();
				}
				catch
				{
				}
				//debug file
				if (CommonClass.getDebugValue())
				{
					Logger.debuggingOut.Append("Column,Row,AGERANGEID,Type,a,b,c,beta,delta,control,baseline,incidence,population,prevelance,result\n");
				}


				double baseValue = 0;
				double controlValue = 0;
				double populationValue = 0;
				double incidenceValue = 0;
				double prevalenceValue = 0;

				Dictionary<string, double> dicPopValue = new Dictionary<string, double>();
				Dictionary<string, double> dicIncidenceValue = new Dictionary<string, double>();
				Dictionary<string, double> dicPrevalenceValue = new Dictionary<string, double>();

				double deltaQValue = 0;
				float i365 = 1;
				int iStartDay = 365, iEndDay = 0;

				//Determined that, regardless of whether there is or is not a seasonal metric, if the user designates no Metric Statistic, then the function is daily--calculated over the duration of the global pollutant season
				if (crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic == MetricStatic.None)
				{
					//Need to find the start and end day of the global pollutant season to handle missing values outside the global season correctly and to correctly calculate seasonal metrics of pollutant
					//Previously, logic implemented looked at the seasonal metrics of the pollutant and used the last season in that list to define the start and end day
					//This is incorrect as the seasonal metric is some subset of the global season
					//Now, the global season predominates in determining the start and end

					i365 = 365;

						if (crSelectFunction.BenMAPHealthImpactFunction.Pollutant.Seasons != null && crSelectFunction.BenMAPHealthImpactFunction.Pollutant.Seasons.Count != 0)
						{
							i365 = 0;
							foreach (Season season in crSelectFunction.BenMAPHealthImpactFunction.Pollutant.Seasons)
							{
								
								i365 = i365 + season.EndDay - season.StartDay + 1;
								if (season.StartDay < iStartDay) iStartDay = season.StartDay;
								if (season.EndDay > iEndDay) iEndDay = season.EndDay + 1;
							}

						}
				}

				Dictionary<string, double> dicVariable = null;
				double d = 0;
				CRCalculateValue crCalculateValue = new CRCalculateValue();

				string strBaseLineFunction = ConfigurationCommonClass.getFunctionStringFromDatabaseFunction(crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction);
				bool hasPopInstrBaseLineFunction = crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction.Contains("POP");
				string strPointEstimateFunction = ConfigurationCommonClass.getFunctionStringFromDatabaseFunction(crSelectFunction.BenMAPHealthImpactFunction.Function);
				Dictionary<string, MonitorValue> dicBaseMonitor = new Dictionary<string, MonitorValue>();
				Dictionary<string, MonitorValue> dicControlMonitor = new Dictionary<string, MonitorValue>();
				Dictionary<string, List<MonitorNeighborAttribute>> dicAllMonitorNeighborControl = new Dictionary<string, List<MonitorNeighborAttribute>>();
				Dictionary<string, List<MonitorNeighborAttribute>> dicAllMonitorNeighborBase = new Dictionary<string, List<MonitorNeighborAttribute>>();


				bool hasGeographicArea = false;
				if (crSelectFunction.GeographicAreaName != GEOGRAPHIC_AREA_EVERYWHERE)
				{
					hasGeographicArea = true;
				}

				if (baseControlGroup.Base is MonitorDataLine && baseControlGroup.Control is MonitorDataLine && crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic == MetricStatic.None)
				{
					if ((baseControlGroup.Base as MonitorDataLine).MonitorValues != null)
					{
						foreach (MonitorValue m in (baseControlGroup.Base as MonitorDataLine).MonitorValues)
						{
							dicBaseMonitor.Add(m.MonitorName, m);
						}
					}
					if ((baseControlGroup.Control as MonitorDataLine).MonitorValues != null)
					{
						foreach (MonitorValue m in (baseControlGroup.Control as MonitorDataLine).MonitorValues)
						{
							dicControlMonitor.Add(m.MonitorName, m);
						}
					}
					if ((baseControlGroup.Base as MonitorDataLine).MonitorNeighbors != null)
					{
						foreach (MonitorNeighborAttribute m in (baseControlGroup.Base as MonitorDataLine).MonitorNeighbors)
						{
							if (!dicAllMonitorNeighborBase.ContainsKey(m.Col + "," + m.Row))
								dicAllMonitorNeighborBase.Add(m.Col + "," + m.Row, new List<MonitorNeighborAttribute>() { m });
							else
								dicAllMonitorNeighborBase[m.Col + "," + m.Row].Add(m);
						}
					}
					if ((baseControlGroup.Control as MonitorDataLine).MonitorNeighbors != null)
					{
						foreach (MonitorNeighborAttribute m in (baseControlGroup.Control as MonitorDataLine).MonitorNeighbors)
						{
							if (!dicAllMonitorNeighborControl.ContainsKey(m.Col + "," + m.Row))
								dicAllMonitorNeighborControl.Add(m.Col + "," + m.Row, new List<MonitorNeighborAttribute>() { m });
							else
								dicAllMonitorNeighborControl[m.Col + "," + m.Row].Add(m);
						}
					}
				}

				foreach (ModelResultAttribute modelResultAttribute in baseControlGroup.Base.ModelResultAttributes)
				{
					bool debug = false;
					if (modelResultAttribute.Col == 312 && modelResultAttribute.Row == 97)
					{
						debug = true;
					}

					// If a HIF has an assigned Geographic Area, only run it if it intersects with this grid cell
					if (hasGeographicArea)
					{
						if (crSelectFunction.GeographicAreaName == GEOGRAPHIC_AREA_ELSEWHERE)
						{
							if (dicGeoAreaPercentages.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row) == true)
							{
								// We had an interesction with at least one of the geographic areas. Skip to next grid cell
								continue;
							}
						}
						else
						{
							if (dicGeoAreaPercentages.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row) == false)
							{
								// No interesction with geographic area. Skip to next grid cell
								continue;
							}
						}

					}

					populationValue = 0;
					incidenceValue = 0;
					prevalenceValue = 0;

					if (dicPopulationAllAge != null)
					{
						foreach (KeyValuePair<string, double> s in dicAge)
						{
							if (dicPopulationAllAge.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row + "," + s.Key))
								populationValue += dicPopulationAllAge[modelResultAttribute.Col + "," + modelResultAttribute.Row + "," + s.Key] * s.Value;
						}

					}
					if (populationValue == 0)
						continue;
					dicIncidenceValue = new Dictionary<string, double>();
					dicPrevalenceValue = new Dictionary<string, double>();
					dicPopValue = new Dictionary<string, double>();
					if (dicIncidenceRateAttribute != null)
					{
						foreach (string s in lstAllAgeID)
						{
							if (dicIncidenceRateAttribute.Keys.Contains((Convert.ToInt32(modelResultAttribute.Col) * 1000000 + Convert.ToInt32(modelResultAttribute.Row)).ToString() + "," + s))
							{
								dicIncidenceValue.Add(s, dicIncidenceRateAttribute[(Convert.ToInt32(modelResultAttribute.Col) * 1000000 + Convert.ToInt32(modelResultAttribute.Row)).ToString() + "," + s]);

							}
						}
					}
					if (dicPrevalenceRateAttribute != null)
					{
						foreach (string s in lstAllAgeID)
						{
							if (dicPrevalenceRateAttribute.Keys.Contains((Convert.ToInt32(modelResultAttribute.Col) * 1000000 + Convert.ToInt32(modelResultAttribute.Row)).ToString() + "," + s))
							{
								dicPrevalenceValue.Add(s, dicPrevalenceRateAttribute[(Convert.ToInt32(modelResultAttribute.Col) * 1000000 + Convert.ToInt32(modelResultAttribute.Row)).ToString() + "," + s]);
							}
						}

					}
					if (dicPopulationAllAge != null)
					{
						foreach (string s in lstAllAgeID)
						{
							if (!dicAge.ContainsKey(s)) continue;
							if (dicPopulationAllAge.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row + "," + s))
							{
								dicPopValue.Add(s, dicPopulationAllAge[modelResultAttribute.Col + "," + modelResultAttribute.Row + "," + s] * dicAge[s]);
							}
						}
					}
					if (DicAllSetupVariableValues != null && DicAllSetupVariableValues.Count > 0)
					{
						dicVariable = new Dictionary<string, double>();
						d = 0;
						foreach (KeyValuePair<string, Dictionary<string, double>> k in DicAllSetupVariableValues)
						{
							d = 0;
							if (k.Value.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row))
								d = k.Value[modelResultAttribute.Col + "," + modelResultAttribute.Row];


							dicVariable.Add(k.Key, d);

						}
					}

					//Debug switch: true to export daily baseline and control per grid. Only affect debug mode.
					bool exportSwitch = false;

					//If there is a seasonal metric in the HIF
					if (crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
					{
						//If there is a metric statistic other than "None"
						if (crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic != MetricStatic.None)
						{	
							//Retrieve the "Seasonal Metric, Metric Statistic" value from the dictionary--this is a calculation performed over the entire range of the seasonal metric
							if (dicBaseMetricData.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row) && dicBaseMetricData[modelResultAttribute.Col + "," + modelResultAttribute.Row].ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName + ","
									+ Enum.GetName(typeof(MetricStatic), crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic))
									&&
									dicControlMetricData.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row) && dicControlMetricData[modelResultAttribute.Col + "," + modelResultAttribute.Row].ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName + ","
									+ Enum.GetName(typeof(MetricStatic), crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic))
									)
							{
								baseValue = dicBaseMetricData[modelResultAttribute.Col + "," + modelResultAttribute.Row][crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName + ","
								+ Enum.GetName(typeof(MetricStatic), crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic)];
								controlValue = dicControlMetricData[modelResultAttribute.Col + "," + modelResultAttribute.Row][crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName + ","
								+ Enum.GetName(typeof(MetricStatic), crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic)];
							}
							else
							{
								crCalculateValue = new CRCalculateValue()
								{
									Baseline = 0,
									Col = modelResultAttribute.Col,
									Row = modelResultAttribute.Row,
									Delta = 0,
									Incidence = Convert.ToSingle(incidenceValue),
									Population = Convert.ToSingle(populationValue),
									LstPercentile = new List<float>(),
									Mean = 0,
									PercentOfBaseline = 0,
									PointEstimate = 0,
									StandardDeviation = 0,
									Variance = 0

								};
								if (lhsResultArray != null)
								{
									foreach (double dlhs in lhsResultArray)
									{
										crCalculateValue.LstPercentile.Add(0);
									}
								}
								crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);
								continue;
							}
						}
						else //If the metric statistic is "None"
						{ 
							//Check if the dictionaries contain values for that col/row and seasonal metric
							if (dicBase365.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row) &&
dicBase365[modelResultAttribute.Col + "," + modelResultAttribute.Row].ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName)
&& dicControl365.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row) &&
dicControl365[modelResultAttribute.Col + "," + modelResultAttribute.Row].ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
							{

								float fPSum = 0, fBaselineSum = 0;
								List<float> lstFPSum = new List<float>();
								if (lhsResultArray != null)
								{
									foreach (double dlhs in lhsResultArray)
									{
										lstFPSum.Add(0);
									}
								}

								//Accessing the daily values of the seasonal metric--limit to start and end of global pollutant
								for (int iBase = iStartDay; iBase < iEndDay; iBase++)
								{
									double fBase, fControl, fDelta;
									fBase = dicBase365[modelResultAttribute.Col + "," + modelResultAttribute.Row][crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName][iBase];
									fControl = dicControl365[modelResultAttribute.Col + "," + modelResultAttribute.Row][crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName][iBase];
									if (fBase != float.MinValue && fControl != float.MinValue)
									{
										if (Threshold != 0 && fBase < Threshold)
											fBase = Threshold;
										if (fControl != 0 && fControl < Threshold)
											fControl = Threshold;
										fDelta = fBase - fControl;
										{
											CRCalculateValue cr = CalculateCRSelectFunctionsOneCel(sCRID, hasPopInstrBaseLineFunction, 1, crSelectFunction, strBaseLineFunction, strPointEstimateFunction, modelResultAttribute.Col, modelResultAttribute.Row, fBase, fControl, dicPopValue, dicIncidenceValue, dicPrevalenceValue, dicVariable, lhsResultArray);
											fPSum += cr.PointEstimate;
											fBaselineSum += cr.Baseline;
											if (lhsResultArray != null)
											{
												for (int dlhs = 0; dlhs < lhsResultArray.Count(); dlhs++)
												{
													lstFPSum[dlhs] += cr.LstPercentile[dlhs];
												}
											}
										}
									}

								}
								crCalculateValue = new CRCalculateValue()
								{
									Col = modelResultAttribute.Col,
									Row = modelResultAttribute.Row,
									Delta = 0,
									Incidence = Convert.ToSingle(incidenceValue),
									PointEstimate = fPSum,
									LstPercentile = lstFPSum,
									Population = Convert.ToSingle(populationValue),
									Mean = lstFPSum.Count() == 0 ? float.NaN : getMean(lstFPSum),
									Variance = lstFPSum.Count() == 0 ? float.NaN : getVariance(lstFPSum, fPSum),
									Baseline = fBaselineSum,
								};
								crCalculateValue.StandardDeviation = lstFPSum.Count() == 0 ? float.NaN : Convert.ToSingle(Math.Sqrt(crCalculateValue.Variance));
								//crCalculateValue.PercentOfBaseline = crCalculateValue.Baseline == 0 ? 0 : Convert.ToSingle(Math.Round((crCalculateValue.Mean / crCalculateValue.Baseline) * 100, 4)); 
								//Use point estimate to calculate pct baseline when in point mode.
								if (crCalculateValue.Baseline == 0)
									crCalculateValue.PercentOfBaseline = 0;
								else if (float.IsNaN(crCalculateValue.Mean))
									crCalculateValue.PercentOfBaseline = Convert.ToSingle(Math.Round((crCalculateValue.PointEstimate / crCalculateValue.Baseline) * 100, 4));
								else
									crCalculateValue.PercentOfBaseline = Convert.ToSingle(Math.Round((crCalculateValue.Mean / crCalculateValue.Baseline) * 100, 4));
								double baseValueForDelta = modelResultAttribute.Values[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName];
								double controlValueForDelta = baseValueForDelta;

								if (dicControl.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row))
								{

									if (dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values.Keys.Contains(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
										controlValueForDelta = dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName];

								}
								if (Threshold != 0 && baseValueForDelta < Threshold)
									baseValueForDelta = Threshold;

								if (Threshold != 0 && controlValueForDelta < Threshold)
									controlValueForDelta = Threshold;
								crCalculateValue.Delta = Convert.ToSingle(baseValueForDelta - controlValueForDelta);
								crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);
								continue;

							}
							else
							{
								if (modelResultAttribute.Values.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
									baseValue = modelResultAttribute.Values[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName];
								controlValue = baseValue;

								if (dicControl.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row))
								{

									if (dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values.Keys.Contains(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
										controlValue = dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName];

								}
								//                                if (baseControlGroup.Base is MonitorDataLine && baseControlGroup.Control is MonitorDataLine && baseValue != controlValue && dicAllMonitorNeighborBase != null
								//&& dicAllMonitorNeighborBase.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row)
								//&& dicAllMonitorNeighborControl != null && dicAllMonitorNeighborBase.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row))
								//In versions 1.4.3 and earlier BenMAP skips calculating HI for the grid if baseValue == controlValue
								if (baseControlGroup.Base is MonitorDataLine && baseControlGroup.Control is MonitorDataLine && dicAllMonitorNeighborBase != null
								&& dicAllMonitorNeighborBase.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row)
								&& dicAllMonitorNeighborControl != null && dicAllMonitorNeighborBase.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row))
								{
									i365 = crSelectFunction.BenMAPHealthImpactFunction.Pollutant.Seasons.Count();
									iStartDay = 0;
									iEndDay = crSelectFunction.BenMAPHealthImpactFunction.Pollutant.Seasons.Count();
									bool is365 = false;
									int dayCount = 4;
									foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborBase[modelResultAttribute.Col + "," + modelResultAttribute.Row])
									{
										if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
										{
											is365 = true;
											dayCount = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName].Count;
											break;
										}
									}
									if (!is365)
									{
										foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborControl[modelResultAttribute.Col + "," + modelResultAttribute.Row])
										{
											if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
											{
												is365 = true;
												dayCount = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName].Count;
												break;
											}
										}
									}

									if (is365)
									{
										List<float> lstdfmBase = new List<float>();

										foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborBase[modelResultAttribute.Col + "," + modelResultAttribute.Row])
										{
											if (lstdfmBase.Count == 0) //if it's the first monitor for this grid
											{
												if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
												{
													//lstdfmBase = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName].Select(p => p == float.MinValue ? 0 : Convert.ToSingle(p * mnAttribute.Weight)).ToList();
													lstdfmBase = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName].Select(p => p == float.MinValue ? float.MinValue : Convert.ToSingle(p * mnAttribute.Weight)).ToList();

												}
												else if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
												{
													//float value = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] == float.MinValue ? 0 : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] * Convert.ToSingle(mnAttribute.Weight);
													float value = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] == float.MinValue ? float.MinValue : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] * Convert.ToSingle(mnAttribute.Weight);
													for (int i = 0; i < dayCount; i++)
													{
														lstdfmBase.Add(value);
													}
												}
											}
											else // if it's not the first monitor for this grid
											{
												if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
												{
													for (int idfm = 0; idfm < lstdfmBase.Count; idfm++)
													{
														//lstdfmBase[idfm] += dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName][idfm] == float.MinValue ? 0 : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName][idfm] * Convert.ToSingle(mnAttribute.Weight);
														//Only add monitor values which are not float.MinValue. If all monitors have values as float.MinValue, this grid have float.MinValue instead of 0
														float value = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName][idfm] == float.MinValue ? float.MinValue : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName][idfm] * Convert.ToSingle(mnAttribute.Weight);
														if (value == float.MinValue)
														{
															//Skip this value
														}
														else if (lstdfmBase[idfm] == float.MinValue)
														{
															lstdfmBase[idfm] = value;
														}
														else
														{
															lstdfmBase[idfm] += value;
														}
													}
												}
												else if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
												{
													//float value = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] == float.MinValue ? 0 : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] * Convert.ToSingle(mnAttribute.Weight);
													//Only add monitor values which are not float.MinValue. If all monitors have values as float.MinValue, this grid should have float.MinValue instead of 0
													float value = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] == float.MinValue ? float.MinValue : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] * Convert.ToSingle(mnAttribute.Weight);
													for (int idfm = 0; idfm < lstdfmBase.Count; idfm++)
													{
														//lstdfmBase[idfm] += value;
														if (value == float.MinValue)
														{
															//Skip this value
														}
														else if (lstdfmBase[idfm] == float.MinValue)
														{
															lstdfmBase[idfm] = value;
														}
														else
														{
															lstdfmBase[idfm] += value;
														}
													}
												}
											}
										}

										List<float> lstdfmControl = new List<float>();


										foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborControl[modelResultAttribute.Col + "," + modelResultAttribute.Row])
										{
											if (lstdfmControl.Count == 0)
											{
												if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
												{
													//lstdfmControl = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName].Select(p => p == float.MinValue ? 0 : Convert.ToSingle(p * mnAttribute.Weight)).ToList();
													lstdfmControl = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName].Select(p => p == float.MinValue ? float.MinValue : Convert.ToSingle(p * mnAttribute.Weight)).ToList();
												}
												else if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
												{
													//float value = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] == float.MinValue ? 0 : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] * Convert.ToSingle(mnAttribute.Weight);
													float value = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] == float.MinValue ? float.MinValue : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] * Convert.ToSingle(mnAttribute.Weight);
													for (int i = 0; i < dayCount; i++)
													{
														lstdfmControl.Add(value);
													}
												}
											}
											else
											{
												if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
												{
													for (int idfm = 0; idfm < lstdfmControl.Count; idfm++)
													{
														//lstdfmControl[idfm] += dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName][idfm] == float.MinValue ? 0 : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName][idfm] * Convert.ToSingle(mnAttribute.Weight);
														float value = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName][idfm] == float.MinValue ? float.MinValue : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName][idfm] * Convert.ToSingle(mnAttribute.Weight);
														if (value == float.MinValue)
														{
															//Skip this value
														}
														else if (lstdfmControl[idfm] == float.MinValue)
														{
															lstdfmControl[idfm] = value;
														}
														else
														{
															lstdfmControl[idfm] += value;
														}
													}
												}
												else if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
												{
													//float value = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] == float.MinValue ? 0 : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] * Convert.ToSingle(mnAttribute.Weight);
													float value = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] == float.MinValue ? float.MinValue : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] * Convert.ToSingle(mnAttribute.Weight);
													//Only add monitor values which are not float.MinValue. If all monitors have values as float.MinValue, this grid should have float.MinValue instead of 0
													for (int idfm = 0; idfm < lstdfmControl.Count; idfm++)
													{
														//lstdfmControl[idfm] += value;
														if (value == float.MinValue)
														{
															//Skip this value
														}
														else if (lstdfmControl[idfm] == float.MinValue)
														{
															lstdfmControl[idfm] = value;
														}
														else
														{
															lstdfmControl[idfm] += value;
														}
													}
												}
											}
										}

										float fPSum = 0, fBaselineSum = 0;
										List<float> lstFPSum = new List<float>();
										if (lhsResultArray != null)
										{
											for (int dlhs = 0; dlhs < lhsResultArray.Count(); dlhs++)
											{
												lstFPSum.Add(0);
											}
										}
										if (lstdfmBase.Count > 0 && lstdfmControl.Count > 0)
										{
#if DEBUG
											//Debug mode only: export daily baseline and control to a csv
											if (exportSwitch == true)
											{
												try
												{
													string path = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + @"\My BenMAP-CE Files\";
													string baseDailyValue = String.Join(",", lstdfmBase);
													StreamWriter baseWriter = new StreamWriter(path + "debug_baselinse.csv", true);
													string baseMsg = string.Format("{0}_{1:yyyyMMddhhmmss}.bin", sCRID, DateTime.Now) + "," + modelResultAttribute.Col + "," + modelResultAttribute.Row + "," + baseDailyValue;
													baseWriter.WriteLine(baseMsg);
													baseWriter.Close();

													string controlDailyValue = String.Join(",", lstdfmControl);
													StreamWriter controlWriter = new StreamWriter(path + "debug_control.csv", true);
													string controlMsg = string.Format("{0}_{1:yyyyMMddhhmmss}.bin", sCRID, DateTime.Now) + "," + modelResultAttribute.Col + "," + modelResultAttribute.Row + "," + controlDailyValue;
													controlWriter.WriteLine(controlMsg);
													controlWriter.Close();
												}
												catch (Exception myEx)
												{
												}
											}

#endif

											for (int iBase = iStartDay; iBase < iEndDay; iBase++)
											{
												double fBase, fControl, fDelta;
												fBase = lstdfmBase[iBase];
												fControl = lstdfmControl[iBase];
												if (fBase != float.MinValue && fControl != float.MinValue) //changed 0 = to float.MinValue
												{
													if (Threshold != 0 && fBase < Threshold)
														fBase = Threshold;
													if (fControl != 0 && fControl < Threshold)
														fControl = Threshold;
													fDelta = fBase - fControl;
													{
														CRCalculateValue cr = CalculateCRSelectFunctionsOneCel(sCRID, hasPopInstrBaseLineFunction, 1, crSelectFunction, strBaseLineFunction, strPointEstimateFunction, modelResultAttribute.Col, modelResultAttribute.Row, fBase, fControl, dicPopValue, dicIncidenceValue, dicPrevalenceValue, dicVariable, lhsResultArray);
														fPSum += cr.PointEstimate;
														fBaselineSum += cr.Baseline;
														if (lhsResultArray != null)
														{
															for (int dlhs = 0; dlhs < lhsResultArray.Count(); dlhs++)
															{
																lstFPSum[dlhs] += cr.LstPercentile[dlhs];
															}
														}
													}
												}

											}
										}
										crCalculateValue = new CRCalculateValue()
										{
											Col = modelResultAttribute.Col,
											Row = modelResultAttribute.Row,
											Delta = 0,
											Incidence = Convert.ToSingle(incidenceValue),
											PointEstimate = fPSum,
											LstPercentile = lstFPSum,
											Population = Convert.ToSingle(populationValue),
											Mean = lstFPSum.Count() == 0 ? float.NaN : getMean(lstFPSum),
											Variance = lstFPSum.Count() == 0 ? float.NaN : getVariance(lstFPSum, fPSum),
											Baseline = fBaselineSum,
										};
										crCalculateValue.StandardDeviation = lstFPSum.Count() == 0 ? float.NaN : Convert.ToSingle(Math.Sqrt(crCalculateValue.Variance));
									}
									else
									{
										double fBase = 0;
										foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborBase[modelResultAttribute.Col + "," + modelResultAttribute.Row])
										{
											if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
											{
												//fBase += dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] == float.MinValue ? 0 : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] * Convert.ToSingle(mnAttribute.Weight);
												//Only add monitor values which are not float.MinValue. 
												//If all monitors have values == float.MinValue, this grid should have float.MinValue instead of 0
												float value = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] == float.MinValue ? float.MinValue : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] * Convert.ToSingle(mnAttribute.Weight);
												if (value == float.MinValue)
												{
													//skip
												}
												else if (fBase == float.MinValue)
												{
													fBase = value;
												}
												else
												{
													fBase += value;
												}
											}
										}

										double fControl = 0;
										foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborControl[modelResultAttribute.Col + "," + modelResultAttribute.Row])
										{
											if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
											{
												//fControl += dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] == float.MinValue ? 0 : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] * Convert.ToSingle(mnAttribute.Weight);
												//Only add monitor values which are not float.MinValue. 
												//If all monitors have values == float.MinValue, this grid should have float.MinValue instead of 0
												float value = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] == float.MinValue ? float.MinValue : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] * Convert.ToSingle(mnAttribute.Weight);
												if (value == float.MinValue)
												{
													//skip
												}
												else if (fControl == float.MinValue)
												{
													fControl = value;
												}
												else
												{
													fControl += value;
												}
											}

										}

										float fPSum = 0, fBaselineSum = 0;
										List<float> lstFPSum = new List<float>();
										if (lhsResultArray != null)
										{
											for (int dlhs = 0; dlhs < lhsResultArray.Count(); dlhs++)
											{
												lstFPSum.Add(0);
											}
										}
										double fDelta;
										if (fBase != float.MinValue && fControl != float.MinValue)//changed 0 = to float.MinValue
										{
											if (Threshold != 0 && fBase < Threshold)
												fBase = Threshold;
											if (fControl != 0 && fControl < Threshold)
												fControl = Threshold;
											fDelta = fBase - fControl;
											{
												CRCalculateValue cr = CalculateCRSelectFunctionsOneCel(sCRID, hasPopInstrBaseLineFunction, 1, crSelectFunction, strBaseLineFunction, strPointEstimateFunction, modelResultAttribute.Col, modelResultAttribute.Row, fBase, fControl, dicPopValue, dicIncidenceValue, dicPrevalenceValue, dicVariable, lhsResultArray);
												fPSum += cr.PointEstimate * i365;
												fBaselineSum += cr.Baseline * i365;
												if (lhsResultArray != null)
												{
													for (int dlhs = 0; dlhs < lhsResultArray.Count(); dlhs++)
													{
														lstFPSum[dlhs] += cr.LstPercentile[dlhs];
													}
												}
											}
										}
										crCalculateValue = new CRCalculateValue()
										{
											Col = modelResultAttribute.Col,
											Row = modelResultAttribute.Row,
											Delta = 0,
											Incidence = Convert.ToSingle(incidenceValue),
											PointEstimate = fPSum,
											LstPercentile = lstFPSum,
											Population = Convert.ToSingle(populationValue),
											Mean = lstFPSum.Count() == 0 ? float.NaN : getMean(lstFPSum),
											Variance = lstFPSum.Count() == 0 ? float.NaN : getVariance(lstFPSum, fPSum),
											Baseline = fBaselineSum,
										};
										crCalculateValue.StandardDeviation = lstFPSum.Count() == 0 ? float.NaN : Convert.ToSingle(Math.Sqrt(crCalculateValue.Variance));
									}
									//Use point estimate instead of mean when in point mode
									if (crCalculateValue.Baseline == 0)
										crCalculateValue.PercentOfBaseline = 0;
									else if (float.IsNaN(crCalculateValue.Mean))
										crCalculateValue.PercentOfBaseline = Convert.ToSingle(Math.Round((crCalculateValue.PointEstimate / crCalculateValue.Baseline) * 100, 4));
									else
										crCalculateValue.PercentOfBaseline = Convert.ToSingle(Math.Round((crCalculateValue.Mean / crCalculateValue.Baseline) * 100, 4));
									double baseValueForDelta = modelResultAttribute.Values[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName];
									double controlValueForDelta = baseValueForDelta;

									if (dicControl.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row))
									{

										if (dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values.Keys.Contains(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
											controlValueForDelta = dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName];

									}
									if (Threshold != 0 && baseValueForDelta < Threshold)
										baseValueForDelta = Threshold;

									if (Threshold != 0 && controlValueForDelta < Threshold)
										controlValueForDelta = Threshold;
									crCalculateValue.Delta = Convert.ToSingle(baseValueForDelta - controlValueForDelta);
									crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);


									continue;
								}
								baseValue = modelResultAttribute.Values[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName];
								controlValue = baseValue;

								if (dicControl.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row))
								{

									if (dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values.Keys.Contains(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
										controlValue = dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName];

								}
								i365 = crSelectFunction.BenMAPHealthImpactFunction.Pollutant.Seasons.Count();
							}
						}

					}
					else //if the function doesn't have a seasonal metric...
					{
						//if the function has something other than "None" for the metric statistic...
						if (crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic != MetricStatic.None)
						{
							if (dicBaseMetricData.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row) && dicBaseMetricData[modelResultAttribute.Col + "," + modelResultAttribute.Row].ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName + ","
									+ Enum.GetName(typeof(MetricStatic), crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic))
									&&
									dicControlMetricData.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row) && dicControlMetricData[modelResultAttribute.Col + "," + modelResultAttribute.Row].ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName + ","
									+ Enum.GetName(typeof(MetricStatic), crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic))
									)
							{
								baseValue = dicBaseMetricData[modelResultAttribute.Col + "," + modelResultAttribute.Row][crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName + ","
								+ Enum.GetName(typeof(MetricStatic), crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic)];
								controlValue = dicControlMetricData[modelResultAttribute.Col + "," + modelResultAttribute.Row][crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName + ","
								+ Enum.GetName(typeof(MetricStatic), crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic)];
							}
							else //No match. Use 0 as result for this function and append this result to final result. 
							{
								crCalculateValue = new CRCalculateValue()
								{
									Baseline = 0,
									Col = modelResultAttribute.Col,
									Row = modelResultAttribute.Row,
									Delta = 0,
									Incidence = Convert.ToSingle(incidenceValue),
									Population = Convert.ToSingle(populationValue),
									LstPercentile = new List<float>(),
									Mean = 0,
									PercentOfBaseline = 0,
									PointEstimate = 0,
									StandardDeviation = 0,
									Variance = 0

								};
								if (lhsResultArray != null)
								{
									foreach (double dlhs in lhsResultArray)
									{
										crCalculateValue.LstPercentile.Add(0);
									}
								}
								crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);
								continue;
							}
						}
						else //else if crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic == MetricStatic.None
						{
							//If both baseline and control layer have consecutive daily data
							//loop through each day and CalculateCRSelectFunctionsOneCel result of this grid for each day. 
							if (dicBase365.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row) &&
dicBase365[modelResultAttribute.Col + "," + modelResultAttribute.Row].ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName)
&& dicControl365.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row) &&
dicControl365[modelResultAttribute.Col + "," + modelResultAttribute.Row].ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
							{

								float fPSum = 0, fBaselineSum = 0;
								List<float> lstFPSum = new List<float>();
								if (lhsResultArray != null)
								{
									foreach (double dlhs in lhsResultArray)
									{
										lstFPSum.Add(0);
									}
								}
								//updated to run HIF calculation from start to end of global pollutant, rather than from start to end (Jan 1 to Dec 31) of AQ data input
								for (int iBase = iStartDay; iBase < iEndDay; iBase++)
								{
									double fBase, fControl, fDelta;
									fBase = dicBase365[modelResultAttribute.Col + "," + modelResultAttribute.Row][crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName][iBase];
									fControl = dicControl365[modelResultAttribute.Col + "," + modelResultAttribute.Row][crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName][iBase];
									if (fBase != float.MinValue && fControl != float.MinValue)
									{
										if (Threshold != 0 && fBase < Threshold)
											fBase = Threshold;
										if (fControl != 0 && fControl < Threshold)
											fControl = Threshold;
										fDelta = fBase - fControl;
										{
											CRCalculateValue cr = CalculateCRSelectFunctionsOneCel(sCRID, hasPopInstrBaseLineFunction, 1, crSelectFunction, strBaseLineFunction, strPointEstimateFunction, modelResultAttribute.Col, modelResultAttribute.Row, fBase, fControl, dicPopValue, dicIncidenceValue, dicPrevalenceValue, dicVariable, lhsResultArray);
											fPSum += cr.PointEstimate;
											fBaselineSum += cr.Baseline;
											if (lhsResultArray != null)
											{
												for (int dlhs = 0; dlhs < lhsResultArray.Count(); dlhs++)
												{
													lstFPSum[dlhs] += cr.LstPercentile[dlhs];
												}
											}
										}
									}

								}
								crCalculateValue = new CRCalculateValue()
								{
									Col = modelResultAttribute.Col,
									Row = modelResultAttribute.Row,
									Delta = 0,
									Incidence = Convert.ToSingle(incidenceValue),
									PointEstimate = fPSum,
									LstPercentile = lstFPSum,
									Population = Convert.ToSingle(populationValue),
									Mean = lstFPSum.Count() == 0 ? float.NaN : getMean(lstFPSum),
									Variance = lstFPSum.Count() == 0 ? float.NaN : getVariance(lstFPSum, fPSum),
									Baseline = fBaselineSum,
								};
								crCalculateValue.StandardDeviation = lstFPSum.Count() == 0 ? float.NaN : Convert.ToSingle(Math.Sqrt(crCalculateValue.Variance));
								//Use point estimate instead of mean when in point mode
								if (crCalculateValue.Baseline == 0)
									crCalculateValue.PercentOfBaseline = 0;
								else if (float.IsNaN(crCalculateValue.Mean))
									crCalculateValue.PercentOfBaseline = Convert.ToSingle(Math.Round((crCalculateValue.PointEstimate / crCalculateValue.Baseline) * 100, 4));
								else
									crCalculateValue.PercentOfBaseline = Convert.ToSingle(Math.Round((crCalculateValue.Mean / crCalculateValue.Baseline) * 100, 4));
								double baseValueForDelta = modelResultAttribute.Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];
								double controlValueForDelta = baseValueForDelta;

								if (dicControl.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row))
								{

									if (dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values.Keys.Contains(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
										controlValueForDelta = dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];

								}
								if (Threshold != 0 && baseValueForDelta < Threshold)
									baseValueForDelta = Threshold;

								if (Threshold != 0 && controlValueForDelta < Threshold)
									controlValueForDelta = Threshold;
								crCalculateValue.Delta = Convert.ToSingle(baseValueForDelta - controlValueForDelta);
								crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);
								continue;

							}
							else

							{
								if (modelResultAttribute.Values.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
									baseValue = modelResultAttribute.Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];
								controlValue = baseValue;

								if (dicControl.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row))
								{

									if (dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values.Keys.Contains(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
										controlValue = dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];

								}
								//if (baseControlGroup.Base is MonitorDataLine && baseControlGroup.Control is MonitorDataLine && baseValue != controlValue && dicAllMonitorNeighborBase != null
								//&& dicAllMonitorNeighborBase.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row)
								//&& dicAllMonitorNeighborControl != null && dicAllMonitorNeighborBase.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row))
								//In version 1.4.3 and earlier, BenMAP used to skip calculating daily HIF when annual metric baseValue == controlValue
								if (baseControlGroup.Base is MonitorDataLine && baseControlGroup.Control is MonitorDataLine && dicAllMonitorNeighborBase != null
								&& dicAllMonitorNeighborBase.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row)
								&& dicAllMonitorNeighborControl != null && dicAllMonitorNeighborBase.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row))
								{
									bool is365 = false;
									int dayCount = 365;
									foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborBase[modelResultAttribute.Col + "," + modelResultAttribute.Row])
									{
										if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
										{
											is365 = true;
											dayCount = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName].Count;
											break;
										}
									}
									if (!is365)
									{
										foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborControl[modelResultAttribute.Col + "," + modelResultAttribute.Row])
										{
											if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
											{
												is365 = true;
												dayCount = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName].Count;
												break;
											}
										}
									}

									if (is365)
									{
										List<float> lstdfmBase = new List<float>();
										foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborBase[modelResultAttribute.Col + "," + modelResultAttribute.Row])
										{
											if (lstdfmBase.Count == 0)
											{
												if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
												{
													//lstdfmBase = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName].Select(p => p == float.MinValue ? 0 : Convert.ToSingle(p * mnAttribute.Weight)).ToList();
													lstdfmBase = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName].Select(p => p == float.MinValue ? float.MinValue : Convert.ToSingle(p * mnAttribute.Weight)).ToList();

												}
												else if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
												{
													//float value = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] == float.MinValue ? 0 : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] * Convert.ToSingle(mnAttribute.Weight);
													float value = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] == float.MinValue ? float.MinValue : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] * Convert.ToSingle(mnAttribute.Weight);
													for (int i = 0; i < dayCount; i++)
													{
														lstdfmBase.Add(value);
													}
												}
											}
											else
											{
												if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
												{
													for (int idfm = 0; idfm < lstdfmBase.Count; idfm++)
													{
														//lstdfmBase[idfm] += dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName][idfm] == float.MinValue ? 0 : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName][idfm] * Convert.ToSingle(mnAttribute.Weight);
														//Only add monitor values which are not float.MinValue. 
														//If all monitors have values as float.MinValue, this grid should have float.MinValue instead of 0
														float value = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName][idfm] == float.MinValue ? float.MinValue : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName][idfm] * Convert.ToSingle(mnAttribute.Weight);
														if (value == float.MinValue)
														{ //skip
														}
														else if (lstdfmBase[idfm] == float.MinValue)
														{
															lstdfmBase[idfm] = value;
														}
														else
														{
															lstdfmBase[idfm] += value;
														}

													}
												}
												else if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
												{
													float value = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] == float.MinValue ? 0 : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] * Convert.ToSingle(mnAttribute.Weight);
													for (int idfm = 0; idfm < lstdfmBase.Count; idfm++)
													{
														//lstdfmBase[idfm] += value;
														if (value == float.MinValue)
														{ //skip
														}
														else if (lstdfmBase[idfm] == float.MinValue)
														{
															lstdfmBase[idfm] = value;
														}
														else
														{
															lstdfmBase[idfm] += value;
														}
													}
												}
											}
										}

										List<float> lstdfmControl = new List<float>();

										foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborControl[modelResultAttribute.Col + "," + modelResultAttribute.Row])
										{
											if (lstdfmControl.Count == 0)
											{
												if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
												{
													//lstdfmControl = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName].Select(p => p == float.MinValue ? 0 : Convert.ToSingle(p * mnAttribute.Weight)).ToList();
													lstdfmControl = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName].Select(p => p == float.MinValue ? float.MinValue : Convert.ToSingle(p * mnAttribute.Weight)).ToList();

												}
												else if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
												{
													//float value = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] == float.MinValue ? 0 : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] * Convert.ToSingle(mnAttribute.Weight);
													float value = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] == float.MinValue ? float.MinValue : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] * Convert.ToSingle(mnAttribute.Weight);

													for (int i = 0; i < dayCount; i++)
													{
														lstdfmControl.Add(value);
													}
												}
											}
											else
											{
												if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
												{
													for (int idfm = 0; idfm < lstdfmControl.Count; idfm++)
													{
														//lstdfmControl[idfm] += dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName][idfm] == float.MinValue ? 0 : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName][idfm] * Convert.ToSingle(mnAttribute.Weight);
														//Only add monitor values which are not float.MinValue. 
														//If all monitors have values as float.MinValue, this grid should have float.MinValue instead of 0
														float value = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName][idfm] == float.MinValue ? float.MinValue : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName][idfm] * Convert.ToSingle(mnAttribute.Weight);
														if (value == float.MinValue)
														{
															//skip
														}
														else if (lstdfmControl[idfm] == float.MinValue)
														{
															lstdfmControl[idfm] = value;
														}
														else
														{
															lstdfmControl[idfm] += value;
														}
													}
												}
												else if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
												{
													//float value = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] == float.MinValue ? 0 : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] * Convert.ToSingle(mnAttribute.Weight);
													float value = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] == float.MinValue ? float.MinValue : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] * Convert.ToSingle(mnAttribute.Weight);
													for (int idfm = 0; idfm < lstdfmBase.Count; idfm++)
													{
														//lstdfmControl[idfm] += value;
														if (value == float.MinValue)
														{
															//skip
														}
														else if (lstdfmControl[idfm] == float.MinValue)
														{
															lstdfmControl[idfm] = value;
														}
														else
														{
															lstdfmControl[idfm] += value;
														}
													}
												}
											}
										}
										float fPSum = 0, fBaselineSum = 0;
										//Prepare for pencentile
										List<float> lstFPSum = new List<float>();
										if (lhsResultArray != null)
										{
											for (int dlhs = 0; dlhs < lhsResultArray.Count(); dlhs++)
											{
												lstFPSum.Add(0);
											}
										}
										if (lstdfmBase.Count > 0 && lstdfmControl.Count > 0)
										{
#if DEBUG
											//Debug mode only: export daily baseline and control per grid to a csv
											if (exportSwitch == true)
											{
												try
												{
													string path = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + @"\My BenMAP-CE Files\";
													string baseDailyValue = String.Join(",", lstdfmBase);
													StreamWriter baseWriter = new StreamWriter(path + "debug_baselinse.csv", true);
													string baseMsg = string.Format("{0}_{1:yyyyMMddhhmmss}.bin", sCRID, DateTime.Now) + "," + modelResultAttribute.Col + "," + modelResultAttribute.Row + "," + baseDailyValue;
													baseWriter.WriteLine(baseMsg);
													baseWriter.Close();

													string controlDailyValue = String.Join(",", lstdfmControl);
													StreamWriter controlWriter = new StreamWriter(path + "debug_control.csv", true);
													string controlMsg = string.Format("{0}_{1:yyyyMMddhhmmss}.bin", sCRID, DateTime.Now) + "," + modelResultAttribute.Col + "," + modelResultAttribute.Row + "," + controlDailyValue;
													controlWriter.WriteLine(controlMsg);
													controlWriter.Close();
												}
												catch (Exception myEx)
												{
												}
											}

#endif


											for (int iBase = iStartDay; iBase < iEndDay; iBase++)
											{
												double fBase, fControl, fDelta;
												fBase = lstdfmBase[iBase];
												fControl = lstdfmControl[iBase];
												if (fBase != float.MinValue && fControl != float.MinValue) //Changed 0 = to float.MinValue as we do want to continue calculationg when concentration is 0 instead of missing.
												{
													if (Threshold != 0 && fBase < Threshold)
														fBase = Threshold;
													if (fControl != 0 && fControl < Threshold)
														fControl = Threshold;
													fDelta = fBase - fControl;
													if (fDelta != 0)
													{
														CRCalculateValue cr = CalculateCRSelectFunctionsOneCel(sCRID, hasPopInstrBaseLineFunction, 1, crSelectFunction, strBaseLineFunction, strPointEstimateFunction, modelResultAttribute.Col, modelResultAttribute.Row, fBase, fControl, dicPopValue, dicIncidenceValue, dicPrevalenceValue, dicVariable, lhsResultArray);
														fPSum += cr.PointEstimate;
														fBaselineSum += cr.Baseline;
														if (lhsResultArray != null)
														{
															for (int dlhs = 0; dlhs < lhsResultArray.Count(); dlhs++)
															{
																lstFPSum[dlhs] += cr.LstPercentile[dlhs];
															}
														}
													}
												}

											}
										}
										crCalculateValue = new CRCalculateValue()
										{
											Col = modelResultAttribute.Col,
											Row = modelResultAttribute.Row,
											Delta = 0,
											Incidence = Convert.ToSingle(incidenceValue),
											PointEstimate = fPSum,
											LstPercentile = lstFPSum,
											Population = Convert.ToSingle(populationValue),
											Mean = lstFPSum.Count() == 0 ? float.NaN : getMean(lstFPSum),
											Variance = lstFPSum.Count() == 0 ? float.NaN : getVariance(lstFPSum, fPSum),
											Baseline = fBaselineSum,
										};
										crCalculateValue.StandardDeviation = lstFPSum.Count() == 0 ? float.NaN : Convert.ToSingle(Math.Sqrt(crCalculateValue.Variance));
									}
									else
									{
										double fBase = 0;
										foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborBase[modelResultAttribute.Col + "," + modelResultAttribute.Row])
										{
											if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
											{
												//fBase += dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] == float.MinValue ? 0 : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] * Convert.ToSingle(mnAttribute.Weight);
												float value = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] == float.MinValue ? float.MinValue : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] * Convert.ToSingle(mnAttribute.Weight);
												if (value == float.MinValue)
												{
													//skip
												}
												else if (fBase == float.MinValue)
												{
													fBase = value;
												}
												else
												{
													fBase += value;
												}
											}
										}

										double fControl = 0;
										foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborControl[modelResultAttribute.Col + "," + modelResultAttribute.Row])
										{
											if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
											{
												//fControl += dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] == float.MinValue ? 0 : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] * Convert.ToSingle(mnAttribute.Weight);
												float value = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] == float.MinValue ? float.MinValue : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] * Convert.ToSingle(mnAttribute.Weight);
												if (value == float.MinValue)
												{
													//skip
												}
												else if (fControl == float.MinValue)
												{
													fControl = value;
												}
												else
												{
													fControl += value;
												}
											}

											float fPSum = 0, fBaselineSum = 0;
											List<float> lstFPSum = new List<float>();
											if (lhsResultArray != null)
											{
												for (int dlhs = 0; dlhs < lhsResultArray.Count(); dlhs++)
												{
													lstFPSum.Add(0);
												}
											}
											double fDelta;
											if (fBase != float.MinValue && fControl != float.MinValue) //Changed 0 = to float.MinValue as we do want to continue calculationg when concentration is 0 instead of missing.
											{
												if (Threshold != 0 && fBase < Threshold)
													fBase = Threshold;
												if (fControl != 0 && fControl < Threshold)
													fControl = Threshold;
												fDelta = fBase - fControl;
												{
													CRCalculateValue cr = CalculateCRSelectFunctionsOneCel(sCRID, hasPopInstrBaseLineFunction, 1, crSelectFunction, strBaseLineFunction, strPointEstimateFunction, modelResultAttribute.Col, modelResultAttribute.Row, fBase, fControl, dicPopValue, dicIncidenceValue, dicPrevalenceValue, dicVariable, lhsResultArray);
													fPSum += cr.PointEstimate * i365;
													fBaselineSum += cr.Baseline * i365;
													if (lhsResultArray != null)
													{
														for (int dlhs = 0; dlhs < lhsResultArray.Count(); dlhs++)
														{
															lstFPSum[dlhs] += cr.LstPercentile[dlhs] * i365;
														}
													}
												}
											}
											crCalculateValue = new CRCalculateValue()
											{
												Col = modelResultAttribute.Col,
												Row = modelResultAttribute.Row,
												Delta = 0,
												Incidence = Convert.ToSingle(incidenceValue),
												PointEstimate = fPSum,
												LstPercentile = lstFPSum,
												Population = Convert.ToSingle(populationValue),
												Mean = lstFPSum.Count() == 0 ? float.NaN : getMean(lstFPSum),
												Variance = lstFPSum.Count() == 0 ? float.NaN : getVariance(lstFPSum, fPSum),
												Baseline = fBaselineSum,
											};
											crCalculateValue.StandardDeviation = lstFPSum.Count() == 0 ? float.NaN : Convert.ToSingle(Math.Sqrt(crCalculateValue.Variance));
										}
									}
									//Use point estimate instead of mean when in point mode
									if (crCalculateValue.Baseline == 0)
										crCalculateValue.PercentOfBaseline = 0;
									else if (float.IsNaN(crCalculateValue.Mean))
										crCalculateValue.PercentOfBaseline = Convert.ToSingle(Math.Round((crCalculateValue.PointEstimate / crCalculateValue.Baseline) * 100, 4));
									else
										crCalculateValue.PercentOfBaseline = Convert.ToSingle(Math.Round((crCalculateValue.Mean / crCalculateValue.Baseline) * 100, 4));
									double baseValueForDelta = modelResultAttribute.Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];
									double controlValueForDelta = baseValueForDelta;

									if (dicControl.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row))
									{

										if (dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values.Keys.Contains(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
											controlValueForDelta = dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];

									}
									if (Threshold != 0 && baseValueForDelta < Threshold)
										baseValueForDelta = Threshold;

									if (Threshold != 0 && controlValueForDelta < Threshold)
										controlValueForDelta = Threshold;
									crCalculateValue.Delta = Convert.ToSingle(baseValueForDelta - controlValueForDelta);
									crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);


									continue;
								}

							}
						}

					}
					if (Threshold != 0 && baseValue < Threshold)
						baseValue = Threshold;



					if (Threshold != 0 && controlValue < Threshold)
						controlValue = Threshold;
					deltaQValue = baseValue - controlValue;




					{
						crCalculateValue = CalculateCRSelectFunctionsOneCel(sCRID, hasPopInstrBaseLineFunction, i365, crSelectFunction, strBaseLineFunction, strPointEstimateFunction, modelResultAttribute.Col, modelResultAttribute.Row, baseValue, controlValue, dicPopValue, dicIncidenceValue, dicPrevalenceValue, dicVariable, lhsResultArray);

						crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);
					}
					dicVariable = null;
				}

				CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Add(crSelectFunctionCalculateValue);
				DicAllSetupVariableValues = null;
				dicControl = null;
				dicVariable = null;
				GC.Collect();

			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
				return;
			}
		}
		public static void getCalculateValueFromResultCopy(ref CRSelectFunctionCalculateValue crSelectFunctionCalculateValue)
		{

		}

		public static CRCalculateValue CalculateCRSelectFunctionsOneCel(string iCRID, bool hasPopInstrBaseLineFunction, float i365, CRSelectFunction crSelectFunction, string strBaseLineFunction, string strPointEstimateFunction, int col, int row, double baseValue, double controlValue, Dictionary<string, double> dicPopulationValue, Dictionary<string, double> dicIncidenceValue, Dictionary<string, double> dicPrevalenceValue, Dictionary<string, double> dicSetupVariables, double[] lhsDesignResult)
		{
			try
			{
				double incidenceValue, prevalenceValue, PopValue, avgIncidence, avgPrevalence;

				CRCalculateValue crCalculateValue = new CRCalculateValue()
				{
					Col = col,
					Row = row,
					Population = Convert.ToSingle(dicPopulationValue != null ? dicPopulationValue.Sum(p => p.Value) : 0),
					Incidence = 0,
					Delta = Convert.ToSingle(baseValue - controlValue)


				};
				//Console.WriteLine("processing column : " + col + " row : " + row);

				if (dicPopulationValue == null || dicPopulationValue.Count == 0 || dicPopulationValue.Sum(p => p.Value) == 0)
				{
					crCalculateValue.PointEstimate = 0;
					////if (CommonClass.getDebugValue() && (CommonClass.debugGridCell = (CommonClass.debugRow == row && CommonClass.debugCol == col)))
					////    Logger.debuggingOut.Append(crCalculateValue.Col + "," + crCalculateValue.Row + ",");
					////file.Write(crCalculateValue.Col + "," + crCalculateValue.Row + ",");

				}
				else
				{
					if (strPointEstimateFunction.ToLower().Contains("pop"))
					{
						foreach (KeyValuePair<string, double> k in dicPopulationValue)
						{
							incidenceValue = dicIncidenceValue != null && dicIncidenceValue.Count > 0 && dicIncidenceValue.ContainsKey(k.Key) ? dicIncidenceValue[k.Key] : 0;
							prevalenceValue = dicPrevalenceValue != null && dicPrevalenceValue.Count > 0 && dicPrevalenceValue.ContainsKey(k.Key) ? dicPrevalenceValue[k.Key] : 0;
							if (CommonClass.getDebugValue() && (CommonClass.debugGridCell = (CommonClass.debugRow == row && CommonClass.debugCol == col)))
								Logger.debuggingOut.Append(crCalculateValue.Col + "," + crCalculateValue.Row + "," + k.Key + ",");
							crCalculateValue.PointEstimate += ConfigurationCommonClass.getValueFromPointEstimateFunctionString(iCRID, strPointEstimateFunction, crSelectFunction.BenMAPHealthImpactFunction.AContantValue,
									crSelectFunction.BenMAPHealthImpactFunction.BContantValue, crSelectFunction.BenMAPHealthImpactFunction.CContantValue,
									crSelectFunction.BenMAPHealthImpactFunction.Beta, baseValue - controlValue, controlValue, baseValue, incidenceValue, k.Value, prevalenceValue, dicSetupVariables) * i365;
						}
					}
					else
					{   //BenMAP 410--When no population is used, average the rates of incidence and prevalence and pass 0 pouplation, instead of iterating over population bins.
						if (dicIncidenceValue.Count > 0)
							avgIncidence = dicIncidenceValue.Values.Average();
						else
							avgIncidence = 0;
						if (dicPrevalenceValue.Count > 0)
							avgPrevalence = dicPrevalenceValue.Values.Average();
						else
							avgPrevalence = 0;

						if (CommonClass.getDebugValue() && (CommonClass.debugGridCell = (CommonClass.debugRow == row && CommonClass.debugCol == col)))
							Logger.debuggingOut.Append(crCalculateValue.Col + "," + crCalculateValue.Row + ",");

						crCalculateValue.PointEstimate = ConfigurationCommonClass.getValueFromPointEstimateFunctionString(iCRID, strPointEstimateFunction, crSelectFunction.BenMAPHealthImpactFunction.AContantValue,
								crSelectFunction.BenMAPHealthImpactFunction.BContantValue, crSelectFunction.BenMAPHealthImpactFunction.CContantValue,
								crSelectFunction.BenMAPHealthImpactFunction.Beta, baseValue - controlValue, controlValue, baseValue, avgIncidence, 0, avgPrevalence, dicSetupVariables) * i365;
					}
				}
				if (strBaseLineFunction != " return  ;")
				{
					if (hasPopInstrBaseLineFunction && crCalculateValue.Population == 0)
					{
						crCalculateValue.Baseline = 0;
						if (CommonClass.getDebugValue() && (CommonClass.debugGridCell = (CommonClass.debugRow == row && CommonClass.debugCol == col)))
							Logger.debuggingOut.Append(crCalculateValue.Col + "," + crCalculateValue.Row + ",");

					}
					else
					{
						if (strBaseLineFunction.ToLower().Contains("pop"))
						{
							foreach (KeyValuePair<string, double> k in dicPopulationValue)
							{
								incidenceValue = dicIncidenceValue != null && dicIncidenceValue.Count > 0 && dicIncidenceValue.ContainsKey(k.Key) ? dicIncidenceValue[k.Key] : 0;
								prevalenceValue = dicPrevalenceValue != null && dicPrevalenceValue.Count > 0 && dicPrevalenceValue.ContainsKey(k.Key) ? dicPrevalenceValue[k.Key] : 0;
								if (CommonClass.getDebugValue() && (CommonClass.debugGridCell = (CommonClass.debugRow == row && CommonClass.debugCol == col)))
									Logger.debuggingOut.Append(crCalculateValue.Col + "," + crCalculateValue.Row + "," + k.Key + ",");
								crCalculateValue.Baseline += ConfigurationCommonClass.getValueFromBaseFunctionString(iCRID, strBaseLineFunction, crSelectFunction.BenMAPHealthImpactFunction.AContantValue,
										crSelectFunction.BenMAPHealthImpactFunction.BContantValue, crSelectFunction.BenMAPHealthImpactFunction.CContantValue,
										crSelectFunction.BenMAPHealthImpactFunction.Beta, baseValue - controlValue, controlValue, baseValue, incidenceValue, k.Value, prevalenceValue, dicSetupVariables) * i365;
							}
						}
						else
						{   //BenMAP 410--When no population is used, average the rates of incidence and prevalence and pass 0 pouplation, instead of iterating over population bins.
							if (dicIncidenceValue.Count > 0)
								avgIncidence = dicIncidenceValue.Values.Average();
							else
								avgIncidence = 0;
							if (dicPrevalenceValue.Count > 0)
								avgPrevalence = dicPrevalenceValue.Values.Average();
							else
								avgPrevalence = 0;
							if (CommonClass.getDebugValue() && (CommonClass.debugGridCell = (CommonClass.debugRow == row && CommonClass.debugCol == col)))
								Logger.debuggingOut.Append(crCalculateValue.Col + "," + crCalculateValue.Row + ",");

							crCalculateValue.Baseline = ConfigurationCommonClass.getValueFromBaseFunctionString(iCRID, strBaseLineFunction, crSelectFunction.BenMAPHealthImpactFunction.AContantValue,
									crSelectFunction.BenMAPHealthImpactFunction.BContantValue, crSelectFunction.BenMAPHealthImpactFunction.CContantValue,
									crSelectFunction.BenMAPHealthImpactFunction.Beta, baseValue - controlValue, controlValue, baseValue, avgIncidence, 0, avgPrevalence, dicSetupVariables) * i365;
						}
					}
				}
				else
				{
					crCalculateValue.Baseline = crCalculateValue.PointEstimate;
					//if (CommonClass.getDebugValue() && (CommonClass.debugGridCell = (CommonClass.debugRow == row && CommonClass.debugCol == col)))
					//    Logger.debuggingOut.Append(crCalculateValue.Col + "," + crCalculateValue.Row + ",");
				}
				crCalculateValue.LstPercentile = new List<float>();
				if (lhsDesignResult != null)
				{
					foreach (double dlhs in lhsDesignResult)
					{
						crCalculateValue.LstPercentile.Add(0);
					}
					if (crCalculateValue.Population != 0)
					{
						for (int idlhs = 0; idlhs < lhsDesignResult.Count(); idlhs++)
						{
							double dlhs = lhsDesignResult[idlhs];
							if (strPointEstimateFunction.ToLower().Contains("pop"))          //BenMAP 410--When no population is used, average the rates of incidence and prevalence and pass 0 pouplation, instead of iterating over population bins.
							{
								foreach (KeyValuePair<string, double> k in dicPopulationValue)
								{
									incidenceValue = dicIncidenceValue != null && dicIncidenceValue.Count > 0 && dicIncidenceValue.ContainsKey(k.Key) ? dicIncidenceValue[k.Key] : 0;
									prevalenceValue = dicPrevalenceValue != null && dicPrevalenceValue.Count > 0 && dicPrevalenceValue.ContainsKey(k.Key) ? dicPrevalenceValue[k.Key] : 0;
									if (CommonClass.getDebugValue() && (CommonClass.debugGridCell = (CommonClass.debugRow == row && CommonClass.debugCol == col)))
										Logger.debuggingOut.Append(crCalculateValue.Col + "," + crCalculateValue.Row + "," + k.Key + ",");
									crCalculateValue.LstPercentile[idlhs] += (ConfigurationCommonClass.getValueFromPointEstimateFunctionString(iCRID, strPointEstimateFunction,
											crSelectFunction.BenMAPHealthImpactFunction.AContantValue,
									crSelectFunction.BenMAPHealthImpactFunction.BContantValue, crSelectFunction.BenMAPHealthImpactFunction.CContantValue,
										dlhs, baseValue - controlValue, controlValue, baseValue, incidenceValue, k.Value, prevalenceValue, dicSetupVariables) * i365);
								}
							}
							else
							{   //BenMAP 410--When no population is used, average the rates of incidence and prevalence and pass 0 pouplation, instead of iterating over population bins.
								if (dicIncidenceValue.Count > 0)
									avgIncidence = dicIncidenceValue.Values.Average();
								else
									avgIncidence = 0;
								if (dicPrevalenceValue.Count > 0)
									avgPrevalence = dicPrevalenceValue.Values.Average();
								else
									avgPrevalence = 0;
								if (CommonClass.getDebugValue() && (CommonClass.debugGridCell = (CommonClass.debugRow == row && CommonClass.debugCol == col)))
									Logger.debuggingOut.Append(crCalculateValue.Col + "," + crCalculateValue.Row + ",");

								crCalculateValue.LstPercentile[idlhs] += (ConfigurationCommonClass.getValueFromPointEstimateFunctionString(iCRID, strPointEstimateFunction,
										crSelectFunction.BenMAPHealthImpactFunction.AContantValue,
								crSelectFunction.BenMAPHealthImpactFunction.BContantValue, crSelectFunction.BenMAPHealthImpactFunction.CContantValue,
									dlhs, baseValue - controlValue, controlValue, baseValue, avgIncidence, 0, avgPrevalence, dicSetupVariables) * i365);
							}
						}
					}
				}

				//Use NaN instead of 0 for the following results when in point mode.
				crCalculateValue.Mean = crCalculateValue.LstPercentile.Count() == 0 ? float.NaN : getMean(crCalculateValue.LstPercentile);
				crCalculateValue.Variance = crCalculateValue.LstPercentile.Count() == 0 ? float.NaN : getVariance(crCalculateValue.LstPercentile, crCalculateValue.PointEstimate);
				crCalculateValue.StandardDeviation = crCalculateValue.LstPercentile.Count() == 0 ? float.NaN : Convert.ToSingle(Math.Sqrt(crCalculateValue.Variance));

				if (CommonClass.getDebugValue() && (CommonClass.debugGridCell = (CommonClass.debugRow == row && CommonClass.debugCol == col)))
					Logger.Log(Logger.Level.DEBUG, null, null, "Debugging Grid Cell Row " + CommonClass.debugRow + ", Grid Cell Column " + CommonClass.debugCol);

				if (crCalculateValue.Baseline == 0)
					crCalculateValue.PercentOfBaseline = 0;
				else if (float.IsNaN(crCalculateValue.Mean))
					crCalculateValue.PercentOfBaseline = Convert.ToSingle(Math.Round((crCalculateValue.PointEstimate / crCalculateValue.Baseline) * 100, 4));
				else
					crCalculateValue.PercentOfBaseline = Convert.ToSingle(Math.Round((crCalculateValue.Mean / crCalculateValue.Baseline) * 100, 4));
				return crCalculateValue;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
		public static string getFunctionStringFromDatabaseFunction(string DatabaseFunction)
		{
			try
			{
				string result = DatabaseFunction;

				result = result.ToLower();
				result = result.Replace("abs(", "Math.Abs(").Replace("abs (", "Math.Abs(")
.Replace("acos(", "Math.Acos(").Replace("acos (", "Math.Acos(")
.Replace("asin(", "Math.Asin(").Replace("asin (", "Math.Asin(")
.Replace("atan(", "Math.Atan(").Replace("atan (", "Math.Atan(")
.Replace("atan2(", "Math.Atan2(").Replace("atan2 (", "Math.Atan2(")
.Replace("bigmul(", "Math.BigMul(").Replace("bigmul (", "Math.BigMul(")
.Replace("ceiling(", "Math.Ceiling(").Replace("ceiling (", "Math.Ceiling(")
.Replace("cos(", "Math.Cos(").Replace("cos (", "Math.Cos(")
.Replace("Math.AMath.Cos(", "Math.Acos(")
.Replace("cosh(", "Math.Cosh(").Replace("cosh (", "Math.Cosh(")
.Replace("divrem(", "Math.DivRem(").Replace("divrem (", "Math.DivRem(")
.Replace("exp(", "Math.Exp(").Replace("exp (", "Math.Exp(")
.Replace("floor(", "Math.Floor(").Replace("floor (", "Math.Floor(")
.Replace("ieeeremainder(", "Math.IEEERemainder(").Replace("ieeeremainder (", "Math.IEEERemainder(")
.Replace("log(", "Math.Log(").Replace("log (", "Math.Log(")
.Replace("log10(", "Math.Log10(").Replace("log10 (", "Math.Log10(")
.Replace("max(", "Math.Max(").Replace("max (", "Math.Max(")
.Replace("min(", "Math.Min(").Replace("min (", "Math.Min(")
.Replace("pow(", "Math.Pow(").Replace("pow (", "Math.Pow(")
.Replace("round(", "Math.Round(").Replace("round (", "Math.Round(")
.Replace("sign(", "Math.Sign(").Replace("sign (", "Math.Sign(")
.Replace("sin(", "Math.Sin(").Replace("sin (", "Math.Sin(")
.Replace("sinh(", "Math.Sinh(").Replace("sinh (", "Math.Sinh(")
.Replace("sqr(", "myPow(").Replace("sqr (", "myPow(")
.Replace("sqrt(", "Math.Sqrt(").Replace("sqrt (", "Math.Sqrt(")
.Replace("tan(", "Math.Tan(").Replace("tan (", "Math.Tan(")
.Replace("tanh(", "Math.Tanh(").Replace("tanh (", "Math.Tanh(")
.Replace("truncate(", "Math.Truncate(").Replace("truncate (", "Math.Truncate(");


				if (result.Contains("if") && result.Contains(":="))
				{

					result = result.Replace(" and", " && ").Replace(")and", ")&&").Replace(" or", " || ").Replace(")or", ")||").Replace(":=", " return ")
							.Replace("result", " ").Replace("else", ";else").Replace("then", " ").Replace("<>", "!=");
					result = result + ";";
					string tmp = result.Replace("else if", "").Replace("else  if", "").Replace("else   if", "");
					if (!tmp.Contains("else"))
						result += " else return -999999999;";

				}
				else
				{
					result = " return " + result + " ;";
				}


				return result;
			}
			catch (Exception ex)
			{
				return null;
			}
		}




		public static void getSetupVariableNameListFromDatabaseFunction(int VariableDatasetID, int GridDefinitionID, string DatabaseFunction, List<Tuple<string, int>> SystemVariableNameList, ref List<SetupVariableJoinAllValues> lstFunctionVariables)
		{
			try
			{
				if (lstFunctionVariables == null) lstFunctionVariables = new List<SetupVariableJoinAllValues>();
				DatabaseFunction = DatabaseFunction.Replace("prevalence", "").Replace("incidence", "").Replace("deltaq", "")
						 .Replace("pop", "").Replace("beta", "").Replace("q0", "").Replace("q1", "")
						.Replace("abs", " ")
.Replace("acos", " ")
.Replace("asin", " ")
.Replace("atan", " ")
.Replace("atan2", " ")
.Replace("bigmul", " ")
.Replace("ceiling", " ")
.Replace("cos", " ")
.Replace("cosh", " ")
.Replace("divrem", " ")
.Replace("exp", " ")
.Replace("floor", " ")
.Replace("ieeeremainder", " ")
.Replace("log", " ")
.Replace("log10", " ")
.Replace("max", " ")
.Replace("min", " ")
.Replace("pow", " ")
.Replace("round", " ")
.Replace("sign", " ")
.Replace("sin", " ")
.Replace("sinh", " ")
.Replace("sqrt", " ")
.Replace("tan", " ")
.Replace("tanh", " ")
.Replace("truncate", " ");
				ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

				foreach (Tuple<string, int> tuple in SystemVariableNameList)
				{
					if (DatabaseFunction.ToLower().Contains(tuple.Item1.ToLower()))
					{
						string cleanFunction = Regex.Replace(DatabaseFunction, @"[^\w]+", ",");
						string[] checkFunction = cleanFunction.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

						foreach (string temp in checkFunction)
						{
							if (temp.Equals(tuple.Item1.ToLower()))
							{
								bool inLst = false;
								foreach (SetupVariableJoinAllValues sv in lstFunctionVariables)
								{
									if (sv.SetupVariableName.ToLower() == tuple.Item1.ToLower())
									{
										inLst = true;
									}
								}
								if (!inLst)
								{
									SetupVariableJoinAllValues setupVariableJoinAllValues = new SetupVariableJoinAllValues();
									setupVariableJoinAllValues.SetupVariableName = tuple.Item1;
									string commandText = string.Format("select a.SetupVariableID,a.GridDefinitionID from SetupVariables a,SetupVariableDatasets b where a.SetupVariableDatasetID=b.SetupVariableDatasetID and a.SetupVariableName='{0}' and a.SetupVariableDatasetID={1}", tuple.Item1, VariableDatasetID);
									DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
									DataRow dr = ds.Tables[0].Rows[0];
									setupVariableJoinAllValues.SetupVariableID = Convert.ToInt32(dr["SetupVariableID"]);
									setupVariableJoinAllValues.SetupVariableGridType = Convert.ToInt32(dr["GridDefinitionID"]);
									commandText = string.Format(" select SetupVariableID,CColumn,Row,VValue from SetupGeographicVariables where SetupVariableID={0}", setupVariableJoinAllValues.SetupVariableID);
									ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
									setupVariableJoinAllValues.lstValues = new List<SetupVariableValues>();
									foreach (DataRow drVariable in ds.Tables[0].Rows)
									{
										setupVariableJoinAllValues.lstValues.Add(new SetupVariableValues()
										{
											Col = Convert.ToInt32(drVariable["CColumn"]),
											Row = Convert.ToInt32(drVariable["Row"]),
											Value = Convert.ToSingle(drVariable["VValue"])
										});


									}
									SetupVariableJoinAllValues setupVariableJoinAllValuesReturn = new SetupVariableJoinAllValues();
									setupVariableJoinAllValuesReturn.lstValues = new List<SetupVariableValues>();
									IEnumerable<SetupVariableValues> ies = null;

									GridRelationship gridRelationShipPopulation = new GridRelationship();

									foreach (GridRelationship gRelationship in CommonClass.LstGridRelationshipAll)
									{
										if ((gRelationship.bigGridID == setupVariableJoinAllValues.SetupVariableGridType && gRelationship.smallGridID == GridDefinitionID) || (gRelationship.smallGridID == setupVariableJoinAllValues.SetupVariableGridType && gRelationship.bigGridID == GridDefinitionID))
										{
											gridRelationShipPopulation = gRelationship;
										}
									}
									float d = 0;
									if (setupVariableJoinAllValues.SetupVariableGridType == GridDefinitionID)
									{
										setupVariableJoinAllValuesReturn = setupVariableJoinAllValues;
									}
									else
									{
										GridRelationship gridRelationShip = new GridRelationship() { smallGridID = GridDefinitionID, bigGridID = setupVariableJoinAllValues.SetupVariableGridType };
										Dictionary<string, Dictionary<string, double>> dicRelationShip = APVX.APVCommonClass.getRelationFromDicRelationShipAll(gridRelationShip);

										Dictionary<string, float> dicOld = new Dictionary<string, float>();
										Dictionary<string, float> dicNew12 = new Dictionary<string, float>();
										Dictionary<string, float> dicNew = new Dictionary<string, float>();
										foreach (SetupVariableValues sv in setupVariableJoinAllValues.lstValues)
										{
											if (!dicOld.ContainsKey(sv.Col + "," + sv.Row))
											{
												dicOld.Add(sv.Col + "," + sv.Row, sv.Value);
											}
										}
										if (1 == 2)
										{
											Dictionary<string, Dictionary<string, double>> dicRelationShipTo12 = APVX.APVCommonClass.getRelationFromDicRelationShipAll(new GridRelationship() { smallGridID = 27, bigGridID = setupVariableJoinAllValues.SetupVariableGridType });
											foreach (KeyValuePair<string, Dictionary<string, double>> k in dicRelationShipTo12)
											{
												string[] s = k.Key.Split(new char[] { ',' });
												if (dicOld.ContainsKey(k.Key))
												{
													d = dicOld[k.Key]; if (k.Value != null && k.Value.Count > 0)
													{
														foreach (KeyValuePair<string, double> kin in k.Value)
														{

															if (dicNew12.ContainsKey(kin.Key))
															{
																dicNew12[kin.Key] += Convert.ToSingle(d * kin.Value);
															}
															else
																dicNew12.Add(kin.Key, Convert.ToSingle(d * kin.Value));


														}
													}
												}
											}
											dicRelationShipTo12 = APVX.APVCommonClass.getRelationFromDicRelationShipAll(new GridRelationship() { smallGridID = 27, bigGridID = GridDefinitionID });
											foreach (KeyValuePair<string, Dictionary<string, double>> k in dicRelationShipTo12)
											{
												d = 0;
												if (k.Value != null && k.Value.Count > 0)
												{
													foreach (KeyValuePair<string, double> kin in k.Value)
													{
														if (dicNew12.ContainsKey(kin.Key))
															d += Convert.ToSingle(dicNew12[kin.Key] * kin.Value);
													}
													d = d / Convert.ToSingle(k.Value.Sum(p => p.Value));
												}
												if (!dicNew.ContainsKey(k.Key))
												{
													dicNew.Add(k.Key, d);
												}
											}
											foreach (KeyValuePair<string, float> k in dicNew)
											{
												string[] s = k.Key.Split(new char[] { ',' });
												setupVariableJoinAllValuesReturn.lstValues.Add(new SetupVariableValues()
												{
													Col = Convert.ToInt32(s[0]),
													Row = Convert.ToInt32(s[1]),
													Value = k.Value
												});
											}


										}
										else
										{
											if (dicRelationShip != null && dicRelationShip.Count != 0)
											{
												foreach (KeyValuePair<string, Dictionary<string, double>> kO in dicRelationShip)
												{
													foreach (KeyValuePair<string, double> k in kO.Value)
													{
														string[] s = k.Key.Split(new char[] { ',' });
														if (dicOld.ContainsKey(kO.Key))
														{
															d = Convert.ToSingle(dicOld[kO.Key] * k.Value);
															if (dicNew.ContainsKey(k.Key))
															{
																dicNew[k.Key] += d;
															}
															else
															{
																dicNew.Add(k.Key, d);
															}
														}
													}

												}
												foreach (KeyValuePair<string, float> k in dicNew)
												{
													string[] s = k.Key.Split(new char[] { ',' });
													setupVariableJoinAllValuesReturn.lstValues.Add(new SetupVariableValues()
													{
														Col = Convert.ToInt32(s[0]),
														Row = Convert.ToInt32(s[1]),
														Value = k.Value
													});
												}

											}
											else
											{
												APVX.APVCommonClass.getRelationFromDicRelationShipAll(gridRelationShipPopulation);

												if (setupVariableJoinAllValues.SetupVariableGridType == gridRelationShipPopulation.bigGridID)
												{
													if (dicRelationShip != null && dicRelationShip.Count > 0)
													{
														foreach (KeyValuePair<string, Dictionary<string, double>> k in dicRelationShip)
														{
															string[] s = k.Key.Split(new char[] { ',' });
															d = setupVariableJoinAllValues.lstValues.Where(p => p.Col == Convert.ToInt32(s[0]) && p.Row == Convert.ToInt32(s[1])).Average(p => p.Value);
															if (k.Value != null && k.Value.Count > 0)
															{
																foreach (KeyValuePair<string, double> kin in k.Value)
																{
																	string[] sin = kin.Key.Split(new char[] { ',' });
																	ies = setupVariableJoinAllValuesReturn.lstValues.Where(p => p.Col == Convert.ToInt32(sin[0]) && p.Row == Convert.ToInt32(sin[1]));
																	if (ies != null && ies.Count() > 0)
																	{ }
																	else
																	{
																		setupVariableJoinAllValuesReturn.lstValues.Add(new SetupVariableValues()
																		{
																			Col = Convert.ToInt32(sin[0]),
																			Row = Convert.ToInt32(sin[1]),
																			Value = d
																		});
																	}
																}
															}
														}
													}
													else
													{
														foreach (GridRelationshipAttribute gra in gridRelationShipPopulation.lstGridRelationshipAttribute)
														{
															var queryPopulation = from a in setupVariableJoinAllValues.lstValues where gra.bigGridRowCol.Col == a.Col && gra.bigGridRowCol.Row == a.Row select new { Values = setupVariableJoinAllValues.lstValues.Average(c => c.Value) };

															if (queryPopulation != null && queryPopulation.Count() > 0 && gra.smallGridRowCol.Count > 0)
															{
																d = queryPopulation.First().Values;
																foreach (RowCol rc in gra.smallGridRowCol)
																{
																	ies = setupVariableJoinAllValuesReturn.lstValues.Where(p => p.Col == rc.Col && p.Row == rc.Row);
																	if (ies != null && ies.Count() > 0)
																	{ }
																	else
																	{
																		setupVariableJoinAllValuesReturn.lstValues.Add(new SetupVariableValues()
																		{
																			Col = rc.Col,
																			Row = rc.Row,
																			Value = d
																		});
																	}
																}
															}

														}
													}
												}
												else
												{
													if (dicRelationShip != null && dicRelationShip.Count > 0)
													{
														foreach (KeyValuePair<string, Dictionary<string, double>> k in dicRelationShip)
														{
															string[] s = k.Key.Split(new char[] { ',' });
															d = Convert.ToSingle(setupVariableJoinAllValues.lstValues.Where(p => k.Value.ContainsKey(p.Col + "," + p.Row)).Sum(p => p.Value * k.Value[p.Col + "," + p.Row]) / k.Value.Sum(p => p.Value));

															setupVariableJoinAllValuesReturn.lstValues.Add(new SetupVariableValues()
															{
																Col = Convert.ToInt32(s[0]),
																Row = Convert.ToInt32(s[1]),
																Value = d
															});




														}
													}
													else
													{
														foreach (GridRelationshipAttribute gra in gridRelationShipPopulation.lstGridRelationshipAttribute)
														{
															var queryPopulation = from a in setupVariableJoinAllValues.lstValues where gra.smallGridRowCol.Contains(new RowCol() { Row = a.Row, Col = a.Col }, new RowColComparer()) select new { Values = setupVariableJoinAllValues.lstValues.Average(c => c.Value) };

															if (queryPopulation != null && queryPopulation.Count() > 0)
															{
																d = queryPopulation.First().Values;
																ies = setupVariableJoinAllValuesReturn.lstValues.Where(p => p.Col == gra.bigGridRowCol.Col && p.Row == gra.bigGridRowCol.Row);
																if (ies != null && ies.Count() > 0)
																{ }
																else
																{
																	setupVariableJoinAllValuesReturn.lstValues.Add(new SetupVariableValues()
																	{
																		Col = gra.bigGridRowCol.Col,
																		Row = gra.bigGridRowCol.Row,
																		Value = d
																	});
																}
															}


														}
													}
												}
											}
										}
										setupVariableJoinAllValuesReturn.SetupVariableGridType = GridDefinitionID;
										setupVariableJoinAllValuesReturn.SetupVariableID = setupVariableJoinAllValues.SetupVariableID;
										setupVariableJoinAllValuesReturn.SetupVariableName = setupVariableJoinAllValues.SetupVariableName;

									}

									lstFunctionVariables.Add(setupVariableJoinAllValuesReturn);
									ds.Dispose();
								}

							}
						}
					}

				}
			}
			catch (Exception ex)
			{
			}
		}

		private static Tools.CalculateFunctionString _baseeval;
		internal static Tools.CalculateFunctionString BaseEval
		{
			get
			{
				if (_baseeval == null)
					_baseeval = new Tools.CalculateFunctionString();
				return ConfigurationCommonClass._baseeval;
			}

		}
		private static Tools.CalculateFunctionString _pointEstimateEval;
		internal static Tools.CalculateFunctionString PointEstimateEval
		{
			get
			{
				if (_pointEstimateEval == null)
					_pointEstimateEval = new Tools.CalculateFunctionString();
				return ConfigurationCommonClass._pointEstimateEval;
			}

		}

		public static float getValueFromBaseFunctionString(string crid, string FunctionString, double A, double B, double C, double Beta, double DeltaQ, double Q0, double Q1, double Incidence, double POP, double Prevalence, Dictionary<string, double> dicSetupVariables)
		{
			try
			{
				object result = BaseEval.BaseLineEval(crid, FunctionString, A, B, C, Beta, DeltaQ, Q0, Q1, Incidence, POP, Prevalence, dicSetupVariables);
				if (result is double)
				{
					if (double.IsNaN(Convert.ToDouble(result))) return 0;
					return Convert.ToSingle(Convert.ToDouble(result));
				}
				else
				{
					result = BaseEval.BaseLineEval(crid, FunctionString, A, B, C, Beta, DeltaQ, Q0, Q1, Incidence, POP, Prevalence, dicSetupVariables);
					if (result is double)
					{
						if (double.IsNaN(Convert.ToDouble(result))) return 0;
						return Convert.ToSingle(Convert.ToDouble(result));
					}
					else
					{
						return 0;
					}
				}
			}
			catch (Exception ex)
			{
				return 0;
			}
		}
		public static float getValueFromPointEstimateFunctionString(string crid, string FunctionString, double A, double B, double C, double Beta, double DeltaQ, double Q0, double Q1, double Incidence, double POP, double Prevalence, Dictionary<string, double> dicSetupVariables)
		{
			try
			{
				object result = PointEstimateEval.PointEstimateEval(crid, FunctionString, A, B, C, Beta, DeltaQ, Q0, Q1, Incidence, POP, Prevalence, dicSetupVariables);
				if (result is double)
				{
					if (double.IsNaN(Convert.ToDouble(result))) return 0;
					return Convert.ToSingle(Convert.ToDouble(result));
				}
				else
				{   //whats the point of this code??????
						//if we already know its not a double why recalculate  - Adam S.
					result = PointEstimateEval.PointEstimateEval(crid, FunctionString, A, B, C, Beta, DeltaQ, Q0, Q1, Incidence, POP, Prevalence, dicSetupVariables);
					if (result is double)
					{
						if (double.IsNaN(Convert.ToDouble(result))) return 0;
						return Convert.ToSingle(Convert.ToDouble(result));
					}
					else
					{

						return 0;
					}
				}
			}
			catch (Exception ex)
			{
				return 0;

			}

		}

		public static float getMean(List<float> values)
		{
			if (values == null || values.Count == 0) return 0;
			double sumd = 0;
			foreach (float di in values)
			{
				sumd = sumd + di;
			}
			sumd = sumd / values.Count; return Convert.ToSingle(sumd);
		}
		public static float getStandardDeviation(List<float> values, float PointEstimate)
		{
			return Convert.ToSingle(Math.Sqrt(getVariance(values, PointEstimate)));

		}
		public static float getVariance(List<float> values, float PointEstimate)
		{

			if (values == null || values.Count == 0) return 0;
			List<float> lstValuesForStandardDeviation = new List<float>();
			foreach (float f in values)
			{
				lstValuesForStandardDeviation.Add(f);
			}
			lstValuesForStandardDeviation.Add(PointEstimate);
			double avg = lstValuesForStandardDeviation.Average();
			double dResult = lstValuesForStandardDeviation.Sum(v => Math.Pow(v - avg, 2)) / Convert.ToDouble(lstValuesForStandardDeviation.Count() - 1);
			return Convert.ToSingle(dResult);
		}

		public static void dumpSetupVariableJoinAllValueToDebugFile(ref List<SetupVariableJoinAllValues> lstSetupVariable)
		{   // dump information about the current lstVariables to a debugging file
				// this can be used to find out what veriables are being used in the HealthImpactFunctions run and what grid definitions they use
				// output file name is HARDCODED!
			System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\outputfunctionlist" + ".txt", true);
			file.Write("Variable_Name:\tVariable_Grid_Type:\tCell_Count\tCell_Col\tCell_Row\tCell_Value\n");
			// write out variables to file
			foreach (SetupVariableJoinAllValues sv in lstSetupVariable)
			{
				int gridCount = 0;
				// ignore error if there is no grid
				try
				{

					gridCount = sv.lstValues.Count();
				}
				catch { }

				if (gridCount > 0)
				{
					foreach (SetupVariableValues cell in sv.lstValues)
					{
						file.Write("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\n", sv.SetupVariableName.ToString(), sv.SetupVariableGridType.ToString(), gridCount,
										cell.Col.ToString(), cell.Row.ToString(), cell.Value.ToString());
					}

				}
				else
				{
					file.Write("{0}\t{1}\t{2}\n", sv.SetupVariableName.ToString(), sv.SetupVariableGridType.ToString(), gridCount);

				}
				file.Flush();
			}

			file.Close();
		}
		public static void resizeListBoxHorizontalExtent(ListBox listBox, string displayMember)
		{
			//Add horizontal extent to the listbox's width plus the offset to make sure long items show in full.
			try
			{
				int width = 0;
				for (int i = 0; i < listBox.Items.Count; i++)
				{
					DataRowView drvItem = (DataRowView)listBox.Items[i];
					string text = drvItem.Row[displayMember].ToString();

					int hzSize = TextRenderer.MeasureText(text, listBox.Font, listBox.ClientSize, TextFormatFlags.NoPrefix).Width;
					width = Math.Max(width, hzSize);
				}
				listBox.HorizontalExtent = width + 2;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex);
			}

		}



	}
}
