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
using System.Xml.Serialization;
using ProtoBuf;
using System.Reflection;

namespace BenMAP.Configuration
{
    public class ConfigurationCommonClass
    {
        public static void ClearCRSelectFunctionCalculateValueLHS(ref CRSelectFunctionCalculateValue cRSelectFunctionCalculateValue)
        {

        }
        public static void UpdateCRSelectFunctionCalculateValueLHS(ref CRSelectFunctionCalculateValue cRSelectFunctionCalculateValue)
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
                copy.CRRunInPointMode = baseControlCRSelectFunctionCalculateValue.CRRunInPointMode;
                copy.CRThreshold = baseControlCRSelectFunctionCalculateValue.CRThreshold;
                copy.RBenMapGrid = baseControlCRSelectFunctionCalculateValue.RBenMapGrid;

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
                copy.CRRunInPointMode = baseControlCRSelectFunction.CRRunInPointMode;
                copy.CRThreshold = baseControlCRSelectFunction.CRThreshold;
                copy.RBenMapGrid = baseControlCRSelectFunction.RBenMapGrid;
                copy.lstCRSelectFunction = baseControlCRSelectFunction.lstCRSelectFunction;


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

            BaseControlCRSelectFunction baseControlCRSelectFunction = null;
            using (FileStream fs = new FileStream(strFile, FileMode.Open))
            {
                try
                {
                    baseControlCRSelectFunction = Serializer.Deserialize<BaseControlCRSelectFunction>(fs);

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
        public static List<Location> getLocationFromIDAndType(int LocationType, string Locations)
        {
            return null;

        }
        public static BenMAPHealthImpactFunction getBenMAPHealthImpactFunctionFromID(int ID)
        {
            try
            {
                //           string commandText = string.Format("select CRFunctionID,a.CRFunctionDatasetID,f.CRFunctionDataSetName,a.EndpointGroupID,b.EndPointGroupName,a.EndpointID,c.EndPointName,PollutantID,"
                //+ " MetricID,SeasonalMetricID,MetricStatistic,Author,YYear,Location,OtherPollutants,Qualifier,Reference,Race,Gender,Startage,Endage,a.FunctionalFormid,d.FunctionalFormText,"
                //+ " a.IncidenceDatasetID,a.PrevalenceDatasetID,a.VariableDatasetID,Beta,DistBeta,P1Beta,P2Beta,A,NameA,B,NameB,C,NameC,a.BaselineFunctionalFormID,"
                //+ " e.FunctionalFormText as BaselineFunctionalFormText,Ethnicity,Percentile,Locationtypeid, g.IncidenceDataSetName,i.IncidenceDataSetName as PrevalenceDataSetName,"
                //+ " h.SetupVariableDataSetName as VariableDatasetName from crFunctions a join CRFunctionDataSets f on a.CRFunctionDatasetID=f.CRFunctionDatasetID"
                //+ " join EndPointGroups b on a.EndPointGroupID=b.EndPointGroupID join EndPoints c on a.EndPointID=c.EndPointID join FunctionalForms d on a.FunctionalFormid=d.FunctionalFormID"
                //+ " left join BaselineFunctionalForms e on a.BaselineFunctionalFormID=e.FunctionalFormID left join IncidenceDataSets g on a.IncidenceDatasetID=g.IncidenceDatasetID"
                //+ " left join IncidenceDataSets i on a.PrevalenceDatasetID=i.IncidenceDatasetID left join SetupVariableDataSets h on a.VariableDatasetID=h.SetupVariableDataSetID"
                //+ " where CRFunctionID={0}", ID);

                string commandText = string.Format("select a.CRFunctionID,a.CRFunctionDatasetID,f.CRFunctionDataSetName,a.EndpointGroupID,b.EndPointGroupName,a.EndpointID,c.EndPointName,PollutantGroupID,"
    + " MetricID,SeasonalMetricID,MetricStatistic,Author,YYear,Location,OtherPollutants,Qualifier,Reference,Race,Gender,Startage,Endage,a.FunctionalFormid,d.FunctionalFormText,"
    + " a.IncidenceDatasetID,a.PrevalenceDatasetID,a.VariableDatasetID,betas.Beta,dt.DistributionName as DistBeta,betas.P1Beta,betas.P2Beta,betas.A,betas.NameA,betas.B,betas.NameB,betas.C,betas.NameC,"
    + " a.BaselineFunctionalFormID,e.FunctionalFormText as BaselineFunctionalFormText,Ethnicity,Percentile,Locationtypeid, g.IncidenceDataSetName,i.IncidenceDataSetName as PrevalenceDataSetName,"
    + " h.SetupVariableDataSetName as VariableDatasetName, a.MSID, a.BetaVariationID"
    + " from crFunctions a"
    + " join CRFVariables vars on a.CRFunctionID = vars.CRFunctionID"
    + " join CRFBetas betas on vars.CRFVariableID = betas.CRFVariableID"
    + " join DistributionTypes dt on betas.DistributionTypeID = dt.DistributionTypeID"
    + " join CRFunctionDataSets f on a.CRFunctionDatasetID=f.CRFunctionDatasetID"
    + " join EndPointGroups b on a.EndPointGroupID=b.EndPointGroupID"
    + " join EndPoints c on a.EndPointID=c.EndPointID"
    + " join FunctionalForms d on a.FunctionalFormid=d.FunctionalFormID"
    + " left join BaselineFunctionalForms e on a.BaselineFunctionalFormID=e.FunctionalFormID"
    + " left join IncidenceDataSets g on a.IncidenceDatasetID=g.IncidenceDatasetID"
    + " left join IncidenceDataSets i on a.PrevalenceDatasetID=i.IncidenceDatasetID"
    + " left join SetupVariableDataSets h on a.VariableDatasetID=h.SetupVariableDataSetID"
    + " where a.CRFunctionID={0}", ID);

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
                if (dr["P1Beta"] != DBNull.Value)
                {
                    benMapHealthImpactFunction.BetaParameter1 = Convert.ToDouble(dr["P1Beta"]);
                }
                if (dr["P2Beta"] != DBNull.Value)
                {
                    benMapHealthImpactFunction.BetaParameter2 = Convert.ToDouble(dr["P2Beta"]);
                }
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
                benMapHealthImpactFunction.PollutantGroup = Grid.GridCommon.getPollutantGroupFromID(Convert.ToInt32(dr["PollutantGroupID"]));
                //use first pollutant in group, this assumes all pollutants in group have the same metrics
                benMapHealthImpactFunction.Metric = Grid.GridCommon.getMetricFromPollutantAndID(benMapHealthImpactFunction.PollutantGroup.Pollutants.First(), Convert.ToInt32(dr["MetricID"]));
                benMapHealthImpactFunction.SeasonalMetric = null;
                if ((dr["SeasonalMetricID"] is DBNull) == false)
                {
                    //use first pollutant in group, this assumes all pollutants in group have the same seasonal metrics
                    benMapHealthImpactFunction.SeasonalMetric = Grid.GridCommon.getSeasonalMetricFromPollutantAndID(benMapHealthImpactFunction.PollutantGroup.Pollutants.First(), Convert.ToInt32(dr["SeasonalMetricID"]));
                }
                benMapHealthImpactFunction.MetricStatistic = (MetricStatic)Convert.ToInt32(dr["MetricStatistic"]);
                benMapHealthImpactFunction.Author = dr["Author"].ToString();
                benMapHealthImpactFunction.Year = Convert.ToInt32(dr["YYear"]);
                if ((dr["Locationtypeid"] is DBNull || dr["Location"] is DBNull) == false)
                {
                    benMapHealthImpactFunction.Locations = getLocationFromIDAndType(Convert.ToInt32(dr["Locationtypeid"]), dr["Location"].ToString());
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

                benMapHealthImpactFunction.ModelSpecification = Grid.GridCommon.getModelSpecificationFromID(Convert.ToInt32(dr["MSID"]));
                benMapHealthImpactFunction.BetaVariation = Grid.GridCommon.getBetaVariationFromID(Convert.ToInt32(dr["BetaVariationID"]));
                benMapHealthImpactFunction.Variables = Grid.GridCommon.getVariableListFromID(benMapHealthImpactFunction.ID);
                Grid.GridCommon.getBetaListFromPollutantAndID(benMapHealthImpactFunction.Variables);

                return benMapHealthImpactFunction;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static double[] getLHSArray(int LatinHypercubePoints)
        {
            try
            {
                double[,] lhsResult = null;
                double[] lhsResultArray = null;

                lhsResult = ESIL.Kriging.LHSDesign.LhsDesign(1, LatinHypercubePoints);
                lhsResultArray = new double[LatinHypercubePoints];
                int ilhsResult = 0;
                while (ilhsResult < LatinHypercubePoints)
                {
                    lhsResultArray[ilhsResult] = lhsResult[0, ilhsResult] + 0.5; ilhsResult++;
                }

                var q = lhsResultArray.OrderBy(s => s);
                lhsResultArray = q.ToArray();
                return lhsResultArray;
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

        public static double[] getLHSArrayCRFunctionSeed(int LatinHypercubePoints, CRSelectFunction crSelectFunction, int Seed, int pollutantID, int betaIndex, double standardDeviation)
        {
            try
            {

                //get beta for pollutant and index
                CRFVariable crfVariable = getVariableFromPollutantID(crSelectFunction.BenMAPHealthImpactFunction, pollutantID);
                CRFBeta crfBeta = crfVariable.PollBetas[betaIndex];

                List<int> lstInt = new List<int>();
                for (int i = 0; i < LatinHypercubePoints; i++)
                {
                    lstInt.Add(Convert.ToInt16(Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(LatinHypercubePoints) - (100.00 / (2 * Convert.ToDouble(LatinHypercubePoints)))));
                }
                double[] lhsResultArray = new double[LatinHypercubePoints];
                Meta.Numerics.Statistics.Sample sample = null;
                switch (crfBeta.Distribution)
                {
                    case "None":
                        for (int i = 0; i < LatinHypercubePoints; i++)
                        {
                            lhsResultArray[i] = crfBeta.Beta;

                        }
                        return lhsResultArray;
                        break;
                    case "Normal":
                        Meta.Numerics.Statistics.Distributions.Distribution Normal_distribution =
    new Meta.Numerics.Statistics.Distributions.NormalDistribution(crfBeta.Beta, standardDeviation); // crfBeta.P1Beta);
                        sample = CreateSample(Normal_distribution, 1000000, Seed);
                        break;
                    case "Triangular":
                        Meta.Numerics.Statistics.Distributions.Distribution Triangular_distribution =
    new Meta.Numerics.Statistics.Distributions.TriangularDistribution(crfBeta.P1Beta, crfBeta.P2Beta, crfBeta.Beta);
                        sample = CreateSample(Triangular_distribution, 1000000, Seed);
                        break;
                    case "Poisson":
                        Meta.Numerics.Statistics.Distributions.PoissonDistribution Poisson_distribution =
    new Meta.Numerics.Statistics.Distributions.PoissonDistribution(crfBeta.P1Beta);
                        sample = CreateSample(Poisson_distribution, 1000000, Seed);
                        break;
                    case "Binomial":
                        Meta.Numerics.Statistics.Distributions.BinomialDistribution Binomial_distribution =
    new Meta.Numerics.Statistics.Distributions.BinomialDistribution(crfBeta.P1Beta, Convert.ToInt32(crfBeta.P2Beta));
                        sample = CreateSample(Binomial_distribution, 1000000, Seed);
                        break;
                    case "LogNormal":
                        Meta.Numerics.Statistics.Distributions.LognormalDistribution Lognormal_distribution =
    new Meta.Numerics.Statistics.Distributions.LognormalDistribution(crfBeta.P1Beta, crfBeta.P2Beta);
                        sample = CreateSample(Lognormal_distribution, 1000000, Seed);
                        break;
                    case "Uniform":
                        Interval interval = Interval.FromEndpoints(crfBeta.P1Beta, crfBeta.P2Beta);
                        Meta.Numerics.Statistics.Distributions.UniformDistribution Uniform_distribution =
                            new Meta.Numerics.Statistics.Distributions.UniformDistribution(interval); sample = CreateSample(Uniform_distribution, 1000000, Seed);
                        break;
                    case "Exponential":
                        Meta.Numerics.Statistics.Distributions.ExponentialDistribution Exponential_distribution =
    new Meta.Numerics.Statistics.Distributions.ExponentialDistribution(crfBeta.P1Beta);
                        sample = CreateSample(Exponential_distribution, 1000000, Seed);
                        break;
                    case "Geometric":
                        Meta.Numerics.Statistics.Distributions.ExponentialDistribution Geometric_distribution =
    new Meta.Numerics.Statistics.Distributions.ExponentialDistribution(crfBeta.P1Beta);
                        sample = CreateSample(Geometric_distribution, 1000000, Seed);
                        break;
                    case "Weibull":
                        Meta.Numerics.Statistics.Distributions.WeibullDistribution Weibull_distribution =
    new Meta.Numerics.Statistics.Distributions.WeibullDistribution(crfBeta.P1Beta, crfBeta.P2Beta);
                        sample = CreateSample(Weibull_distribution, 1000000, Seed);
                        break;
                    case "Gamma":
                        Meta.Numerics.Statistics.Distributions.GammaDistribution Gamma_distribution =
    new Meta.Numerics.Statistics.Distributions.GammaDistribution(crfBeta.P1Beta, crfBeta.P2Beta);
                        sample = CreateSample(Gamma_distribution, 1000000, Seed);
                        break;
                    case "Logistic":
                        Meta.Numerics.Statistics.Distributions.Distribution logistic_distribution = new Meta.Numerics.Statistics.Distributions.LogisticDistribution(crfBeta.P1Beta, crfBeta.P2Beta);
                        sample = CreateSample(logistic_distribution, 1000000, Seed);

                        break;
                    case "Beta":
                        Meta.Numerics.Statistics.Distributions.BetaDistribution Beta_distribution =
                            new Meta.Numerics.Statistics.Distributions.BetaDistribution(crfBeta.P1Beta, crfBeta.P2Beta);
                        sample = CreateSample(Beta_distribution, 1000000, Seed);
                        break;
                    case "Pareto":
                        Meta.Numerics.Statistics.Distributions.ParetoDistribution Pareto_distribution =
    new Meta.Numerics.Statistics.Distributions.ParetoDistribution(crfBeta.P1Beta, crfBeta.P2Beta);
                        sample = CreateSample(Pareto_distribution, 1000000, Seed);
                        break;
                    case "Cauchy":
                        Meta.Numerics.Statistics.Distributions.CauchyDistribution Cauchy_distribution =
    new Meta.Numerics.Statistics.Distributions.CauchyDistribution(crfBeta.P1Beta, crfBeta.P2Beta);
                        sample = CreateSample(Cauchy_distribution, 1000000, Seed);
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
        public static string getPopulationComandTextFromCRSelectFunction(CRSelectFunction crSelectFunction, BenMAPPopulation benMAPPopulation, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender)
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
                        commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue)*" + dDiv + " as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
                            "  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);

                    }
                    else if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && commonYear != benMAPPopulation.Year)
                    {
                        commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue*c.percentage)*" + dDiv + " as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b ," +
                                  " (select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=22 and normalizationstate in (0,1)) c " +
                                  "  where a.CColumn=c.sourcecolumn and a.Row=c.sourcerow  and b.CColumn= c.TargetColumn and b.Row= c.TargetRow and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);





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
                else if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && commonYear != benMAPPopulation.Year)
                {
                    commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.VValue*c.percentage) as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b ," +
                       " (select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=22 and normalizationstate in (0,1)) c " +
                               "  where a.CColumn=c.sourcecolumn and a.Row=c.sourcerow  and b.CColumn= c.TargetColumn and b.Row= c.TargetRow and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);



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

        public static string getPopulationComandTextFromCRSelectFunctionForInc(CRSelectFunction crSelectFunction, BenMAPPopulation benMAPPopulation, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender)
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
                        commandText += string.Format("select   a.CColumn,a.Row,a.AgeRangeID,sum(a.vvalue*b.vvalue)*" + dDiv + " as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
                            "  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);

                    }
                    else if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && commonYear != benMAPPopulation.Year)
                    {
                        commandText += string.Format("select   a.CColumn,a.Row,a.AgeRangeID,sum(a.vvalue*b.vvalue*c.VValue)*" + dDiv + " as VValue   from PopulationEntries a, PopulationEntries  b ," +
                                  " PopulationGrowthWeights   c   where PopulationDatasetID=2 and YYear=" + CommonClass.BenMAPPopulation.Year + " and a.RaceID=c.RaceID and  a.EthnicityID=c.EthnicityID and a.CColumn= c.TargetColumn  " +
 " and a.Row=c.Targetrow  and b.CColumn= c.SourceColumn and b.Row= c.SourceRow and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);





                    }
                    else
                    {
                        commandText += string.Format("select   a.CColumn,a.Row,a.AgeRangeID,sum(a.vvalue)*" + dDiv + " as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);
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
                    commandText += string.Format("select   a.CColumn,a.Row,a.AgeRangeID,sum(a.vvalue*b.VValue) as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
                        "  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);

                }
                else if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && commonYear != benMAPPopulation.Year)
                {
                    commandText += string.Format("select   a.CColumn,a.Row,a.AgeRangeID,sum(a.vvalue*b.vvalue*c.VValue) as VValue   from PopulationEntries a, PopulationEntries  b ," +
                                   " PopulationGrowthWeights   c   where PopulationDatasetID=2 and YYear=" + CommonClass.BenMAPPopulation.Year + " and a.RaceID=c.RaceID and  a.EthnicityID=c.EthnicityID and a.CColumn= c.TargetColumn  " +
  " and a.Row=c.Targetrow  and b.CColumn= c.SourceColumn and b.Row= c.SourceRow and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);



                }
                else
                {
                    commandText += string.Format("select   a.CColumn,a.Row,a.AgeRangeID,sum(a.vvalue) as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);
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
                commandText = commandText + " group by a.CColumn,a.Row,a.AgeRangeID";
            }
            if (commandText != "")
            {
                commandText = "select   a.CColumn,a.Row,a.AgeRangeID,sum(a.vvalue) as VValue  from ( " + commandText + " ) a group by a.CColumn,a.Row,a.AgeRangeID";
            }
            return commandText;
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
            GridDefinition grd = new GridDefinition();
            Dictionary<string, List<GridRelationshipAttributePercentage>> dicAllGridPercentage = grd.getRelationshipFromBenMAPGridPercentage(big, small, popRasterLoc);
            updatePercentageToDatabase(dicAllGridPercentage.ToArray()[0]);
            CommonClass.IsAddPercentage = true;
            return;
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            foreach (KeyValuePair<string, List<GridRelationshipAttributePercentage>> k in dicAllGridPercentage)
            {
                string commandText = "select max(PercentageID) from GridDefinitionPercentages";

                int iMax = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;
                commandText = string.Format("insert into GridDefinitionPercentages(PERCENTAGEID,SOURCEGRIDDEFINITIONID, TARGETGRIDDEFINITIONID) "
                    + "values({0},{1})", iMax, k.Key);
                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                int i = 1;
                commandText = "execute block as declare incidenceRateID int;" + " BEGIN ";
                FirebirdSql.Data.FirebirdClient.FbCommand fbCommand = new FirebirdSql.Data.FirebirdClient.FbCommand();
                fbCommand.Connection = CommonClass.Connection;
                fbCommand.CommandType = CommandType.Text;
                if (fbCommand.Connection.State != ConnectionState.Open)
                { fbCommand.Connection.Open(); }
                int j = 0;
                foreach (GridRelationshipAttributePercentage grp in k.Value)
                {

                    if (i < 250 && j < k.Value.Count - 1)
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
        public static Dictionary<int, float> getPopulationDataSetFromCRSelectFunction(ref Dictionary<string, float> diclstPopulationAttributeAge, ref Dictionary<int, float> dicPop12, CRSelectFunction crSelectFunction, BenMAPPopulation benMAPPopulation, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, int GridDefinitionID, GridRelationship gridRelationShipPopulation)
        {
            try
            {

                Dictionary<int, float> dicPopulationAttribute = new Dictionary<int, float>();
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                Dictionary<string, float> diclstPopulationAttribute = new Dictionary<string, float>();
                Dictionary<string, Dictionary<string, double>> dicPopweightfromPercentage = new Dictionary<string, Dictionary<string, double>>();

                string commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", benMAPPopulation.DataSetID); int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
                if (CommonClass.MainSetup.SetupID != 1) commonYear = benMAPPopulation.Year;
                commandText = "";
                string strwhere = "";
                if (CommonClass.MainSetup.SetupID == 1)
                    strwhere = "where AGERANGEID!=42";
                else
                    strwhere = " where 1=1 ";
                string ageCommandText = string.Format("select b.* from PopulationConfigurations a, Ageranges b   where a.PopulationConfigurationID=b.PopulationConfigurationID and a.PopulationConfigurationID=(select PopulationConfigurationID from PopulationDatasets where PopulationDataSetID=" + benMAPPopulation.DataSetID + ")"); DataSet dsage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, ageCommandText);
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
                            commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue)*" + dDiv + " as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
                                "  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);


                        }
                        else if ((benMAPPopulation.GridType.GridDefinitionID == 28 || benMAPPopulation.GridType.GridDefinitionID == 27) && CommonClass.MainSetup.SetupID == 1 && commonYear != benMAPPopulation.Year)
                        {

                            commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue*c.VValue*" + dDiv + ") as VValue   from PopulationEntries a,PopulationEntries b ," +
                                " PopulationGrowthWeights c   where  b.PopulationDatasetID=2 and b.YYear={2} and a.RaceID=c.RaceID and  a.EthnicityID=c.EthnicityID and a.CColumn= c.TargetColumn  " +
 " and a.Row=c.Targetrow  and b.CColumn= c.SourceColumn and b.Row= c.SourceRow and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear, CommonClass.BenMAPPopulation.Year);
                        }
                        else
                        {
                            commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue)*" + dDiv + " as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and YYear={1}", benMAPPopulation.DataSetID, commonYear);
                        }
                        commandText = string.Format(commandText + " and a.AgerangeID={0}", Convert.ToInt32(dr["AgerangeID"]));
                        if (!string.IsNullOrEmpty(crSelectFunction.Race) && crSelectFunction.Race.ToLower() != "all")
                        {
                            if (dicRace.ContainsKey(crSelectFunction.Race))
                            {
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
                    }
                    fbDataReader.Dispose();
                    foreach (KeyValuePair<string, float> k in diclstPopulationAttribute)
                    {
                        string[] s = k.Key.Split(new char[] { ',' });
                        dicPopulationAttribute.Add(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1]), k.Value);
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
                        dicPopulationAttribute.Add(Convert.ToInt32(fbDataReader2["CColumn"]) * 10000 + Convert.ToInt32(fbDataReader2["Row"]), Convert.ToSingle(fbDataReader2["VValue"]));


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

                                if (dicPopulationAttribute.Keys.Contains(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                                {
                                    double d = k.Value.Sum(p => p.Value);
                                    foreach (KeyValuePair<string, double> rc in k.Value)
                                    {
                                        string[] sin = rc.Key.Split(new char[] { ',' });
                                        if (!dicPopulationAttributeReturn.ContainsKey(Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])))
                                            dicPopulationAttributeReturn.Add(Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1]), Convert.ToSingle(dicPopulationAttribute[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])] * rc.Value / d));
                                        else
                                            dicPopulationAttributeReturn[Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])] += Convert.ToSingle(dicPopulationAttribute[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])] * rc.Value / d);
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
                                        dicPopulationAttributeReturn.Add(rc.Col * 10000 + rc.Row, dicPopulationAttribute[gra.bigGridRowCol.Col * 10000 + gra.bigGridRowCol.Row] / Convert.ToSingle(gra.smallGridRowCol.Count));
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
                                    if (dicPopulationAttribute.ContainsKey(Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])))
                                        d += dicPopulationAttribute[Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])] * rc.Value;

                                }
                                if (d > 0)
                                {
                                    dicPopulationAttributeReturn.Add(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1]), Convert.ToSingle(d));
                                }

                            }
                        }
                        else
                        {
                            foreach (GridRelationshipAttribute gra in gridRelationShipPopulation.lstGridRelationshipAttribute)
                            {

                                if (!dicPopulationAttributeReturn.ContainsKey(gra.bigGridRowCol.Col * 10000 + gra.bigGridRowCol.Row))
                                    dicPopulationAttributeReturn.Add(gra.bigGridRowCol.Col * 10000 + gra.bigGridRowCol.Row, 0);
                                foreach (RowCol rc in gra.smallGridRowCol)
                                {
                                    if (gra.bigGridRowCol.Col == 13 && gra.bigGridRowCol.Row == 69)
                                    {
                                    }
                                    if (dicPopulationAttribute.Keys.Contains(rc.Col * 10000 + rc.Row))
                                        dicPopulationAttributeReturn[gra.bigGridRowCol.Col * 10000 + gra.bigGridRowCol.Row] += dicPopulationAttribute[rc.Col * 10000 + rc.Row];
                                }
                            }
                        }
                    }
                    dicPopulationAttribute = dicPopulationAttributeReturn.Where(p => p.Value != 0).ToDictionary(p => p.Key, p => p.Value);
                }


                diclstPopulationAttribute = null;
                dsage.Dispose();
                return dicPopulationAttribute;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void getIncidenceLevelFromDatabase()
        {
            try
            {
                string commandTextLevel = "select * from t_poplevel";

                Dictionary<string, float> dicReturn = new Dictionary<string, float>();
                Dictionary<RowCol, double> dicPop = new Dictionary<RowCol, double>();


                dicPop.Add(new RowCol() { Row = 0, Col = 0 }, 0.0136931743472815);
                dicPop.Add(new RowCol() { Row = 1, Col = 4 }, 0.0544440671801567);
                dicPop.Add(new RowCol() { Row = 5, Col = 9 }, 0.0730041638016701);
                dicPop.Add(new RowCol() { Row = 10, Col = 14 }, 0.072923868894577);
                dicPop.Add(new RowCol() { Row = 15, Col = 19 }, 0.0718525871634483);
                dicPop.Add(new RowCol() { Row = 20, Col = 24 }, 0.0673884674906731);
                dicPop.Add(new RowCol() { Row = 25, Col = 29 }, 0.068867988884449);
                dicPop.Add(new RowCol() { Row = 30, Col = 34 }, 0.0728825107216835);
                dicPop.Add(new RowCol() { Row = 35, Col = 39 }, 0.0806736126542091);
                dicPop.Add(new RowCol() { Row = 40, Col = 44 }, 0.0797196552157402);
                dicPop.Add(new RowCol() { Row = 45, Col = 49 }, 0.0713507384061813);
                dicPop.Add(new RowCol() { Row = 50, Col = 54 }, 0.0624626986682415);
                dicPop.Add(new RowCol() { Row = 55, Col = 59 }, 0.0478613935410976);
                dicPop.Add(new RowCol() { Row = 60, Col = 64 }, 0.0384204462170601);
                dicPop.Add(new RowCol() { Row = 65, Col = 69 }, 0.0339006930589676);
                dicPop.Add(new RowCol() { Row = 70, Col = 74 }, 0.0314938016235828);
                dicPop.Add(new RowCol() { Row = 75, Col = 79 }, 0.0263733938336372);
                dicPop.Add(new RowCol() { Row = 80, Col = 84 }, 0.0175950452685356);
                dicPop.Add(new RowCol() { Row = 85, Col = 99 }, 0.0150916986167431);
                string commandText = "select distinct StartAge,EndAge from IncidenceRates";
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                try
                {
                    fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandTextLevel);

                }
                catch
                {
                    commandTextLevel = "create table t_poplevel (   AgeRangeID SMALLINT,   StartAge SMALLINT,   EndAge   SMALLINT,   VValue   FLOAT)";
                    fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandTextLevel);
                    int i = 1;
                    foreach (KeyValuePair<RowCol, double> k in dicPop)
                    {
                        commandTextLevel = "insert into t_poplevel values(" + i + "," + k.Key.Row + "," + k.Key.Col + "," + k.Value + ")";
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandTextLevel);
                        i++;
                    }

                    DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        int iStartAge = Convert.ToInt32(dr["StartAge"]);
                        int iEndAge = Convert.ToInt32(dr["EndAge"]);
                        List<KeyValuePair<RowCol, double>> lstPopDR = dicPop.Where(p => p.Key.Col <= iEndAge && p.Key.Row >= iStartAge).ToList();
                        double dpop = 0;
                        foreach (KeyValuePair<RowCol, double> k in lstPopDR)
                        {
                            if (k.Key.Row >= iStartAge && k.Key.Col <= iEndAge)
                            {
                                dpop += k.Value;
                            }
                            else if (k.Key.Row >= iStartAge && k.Key.Col >= iEndAge)
                            {
                                dpop += (iEndAge - k.Key.Row + 1) * k.Value / (k.Key.Col - k.Key.Row + 1);
                            }
                            else if (k.Key.Row <= iStartAge && k.Key.Col <= iEndAge)
                            {
                                dpop += (k.Key.Col - iStartAge + 1) * k.Value / (k.Key.Col - k.Key.Row + 1);
                            }
                            else if (k.Key.Row <= iStartAge && k.Key.Col >= iEndAge)
                            {
                                dpop += (iEndAge - iStartAge + 1) * k.Value / (k.Key.Col - k.Key.Row + 1);
                            }
                        }
                        fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, "insert into t_poplevel values(-1," + iStartAge + "," + iEndAge + "," + dpop + ")");
                        dicReturn.Add(iStartAge + "," + iEndAge, Convert.ToSingle(dpop));
                    }
                }

            }
            catch
            {
            }
        }
        public static float getPopLevelFromCR(CRSelectFunction crSelectFunction)
        {
            Dictionary<RowCol, double> dicPop = new Dictionary<RowCol, double>();


            dicPop.Add(new RowCol() { Row = 0, Col = 0 }, 0.0136931743472815);
            dicPop.Add(new RowCol() { Row = 1, Col = 4 }, 0.0544440671801567);
            dicPop.Add(new RowCol() { Row = 5, Col = 9 }, 0.0730041638016701);
            dicPop.Add(new RowCol() { Row = 10, Col = 14 }, 0.072923868894577);
            dicPop.Add(new RowCol() { Row = 15, Col = 19 }, 0.0718525871634483);
            dicPop.Add(new RowCol() { Row = 20, Col = 24 }, 0.0673884674906731);
            dicPop.Add(new RowCol() { Row = 25, Col = 29 }, 0.068867988884449);
            dicPop.Add(new RowCol() { Row = 30, Col = 34 }, 0.0728825107216835);
            dicPop.Add(new RowCol() { Row = 35, Col = 39 }, 0.0806736126542091);
            dicPop.Add(new RowCol() { Row = 40, Col = 44 }, 0.0797196552157402);
            dicPop.Add(new RowCol() { Row = 45, Col = 49 }, 0.0713507384061813);
            dicPop.Add(new RowCol() { Row = 50, Col = 54 }, 0.0624626986682415);
            dicPop.Add(new RowCol() { Row = 55, Col = 59 }, 0.0478613935410976);
            dicPop.Add(new RowCol() { Row = 60, Col = 64 }, 0.0384204462170601);
            dicPop.Add(new RowCol() { Row = 65, Col = 69 }, 0.0339006930589676);
            dicPop.Add(new RowCol() { Row = 70, Col = 74 }, 0.0314938016235828);
            dicPop.Add(new RowCol() { Row = 75, Col = 79 }, 0.0263733938336372);
            dicPop.Add(new RowCol() { Row = 80, Col = 84 }, 0.0175950452685356);
            dicPop.Add(new RowCol() { Row = 85, Col = 99 }, 0.0150916986167431);

            int iStartAge = crSelectFunction.StartAge;
            if (iStartAge == -1) iStartAge = 0;
            int iEndAge = crSelectFunction.EndAge;
            if (iEndAge == -1) iEndAge = 99;
            List<KeyValuePair<RowCol, double>> lstPopDR = dicPop.Where(p => p.Key.Col <= iEndAge && p.Key.Row >= iStartAge).ToList();
            double dpop = 0;
            foreach (KeyValuePair<RowCol, double> k in lstPopDR)
            {
                if (k.Key.Row >= iStartAge && k.Key.Col <= iEndAge)
                {
                    dpop += k.Value;
                }
                else if (k.Key.Row >= iStartAge && k.Key.Col >= iEndAge)
                {
                    dpop += (iEndAge - k.Key.Row + 1) * k.Value / (k.Key.Col - k.Key.Row + 1);
                }
                else if (k.Key.Row <= iStartAge && k.Key.Col <= iEndAge)
                {
                    dpop += (k.Key.Col - iStartAge + 1) * k.Value / (k.Key.Col - k.Key.Row + 1);
                }
                else if (k.Key.Row <= iStartAge && k.Key.Col >= iEndAge)
                {
                    dpop += (iEndAge - iStartAge + 1) * k.Value / (k.Key.Col - k.Key.Row + 1);
                }
            }
            return Convert.ToSingle(dpop);
        }
        public static Dictionary<int, double> getIncidenceDataSetFromCRSelectFuntionDicold(Dictionary<int, double> dicPopulation, Dictionary<string, double> dicPopulationAge, Dictionary<int, double> dicPopulation12, CRSelectFunction crSelectFunction, bool bPrevalence, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, int GridDefinitionID, GridRelationship gridRelationShipPopulation)
        {
            try
            {

                Dictionary<int, double> dicIncidenceRateAttribute = new Dictionary<int, double>();
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet dsIncidence = null;
                DataSet dsPrevalence = null;
                string strbPrevalence = "F";
                int iid = crSelectFunction.IncidenceDataSetID;
                if (bPrevalence)
                {
                    strbPrevalence = "T";
                    iid = crSelectFunction.PrevalenceDataSetID;
                }
                string commandText = "";

                int iPopulationDataSetID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, string.Format("select PopulationDataSetID from PopulationDataSets where SetupID={0} and GridDefinitionID= (select GridDefinitionID from IncidenceDataSets where IncidenceDataSetID={1} )", CommonClass.MainSetup.SetupID, iid)));
                int iPopulationDataSetGridID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, string.Format("select GridDefinitionID from IncidenceDataSets where IncidenceDataSetID={1} ", CommonClass.MainSetup.SetupID, iid)));

                BenMAPPopulation benMAPPopulation = new BenMAPPopulation() { DataSetID = iPopulationDataSetID, GridType = new BenMAPGrid() { GridDefinitionID = iPopulationDataSetGridID }, Year = CommonClass.BenMAPPopulation.Year };
                commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", iPopulationDataSetID); int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
                string populationCommandText = getPopulationComandTextFromCRSelectFunction(crSelectFunction, benMAPPopulation, dicRace, dicEthnicity, dicGender);
                commandText = "";

                string commandTextAge = string.Format("select  distinct b.StartAge,b.EndAge    from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c where " +
   " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and b.Prevalence='" + strbPrevalence + "' " +

  " and c.IncidenceDatasetID={0}  ", iid);
                if (crSelectFunction.StartAge != -1)
                {
                    commandTextAge = string.Format(commandTextAge + " and b.EndAge>={0} ", crSelectFunction.StartAge);
                }
                if (crSelectFunction.EndAge != -1)
                {

                    commandTextAge = string.Format(commandTextAge + " and b.StartAge<={0} ", crSelectFunction.EndAge);
                }

                DataSet dsAge = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandTextAge);
                foreach (DataRow dr in dsAge.Tables[0].Rows)
                {
                    if (commandText != "") commandText = commandText + " union ";
                    if ((crSelectFunction.StartAge < Convert.ToInt32(dr["StartAge"]) || crSelectFunction.StartAge == -1) && (crSelectFunction.EndAge > Convert.ToInt32(dr["EndAge"]) || crSelectFunction.EndAge == -1))
                    {
                        CRSelectFunction cr = new CRSelectFunction() { StartAge = Convert.ToInt32(dr["StartAge"]), EndAge = Convert.ToInt32(dr["EndAge"]), Ethnicity = crSelectFunction.Ethnicity, Gender = crSelectFunction.Gender, Race = crSelectFunction.Race };
                        if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4)
                        {
                            populationCommandText = getPopulationComandTextFromCRSelectFunction(cr, CommonClass.BenMAPPopulation, dicRace, dicEthnicity, dicGender);
                            commandText = commandText + string.Format("select  e.SourceColumn as CColumn,e.SourceRow as Row,a.VValue*d.VValue*e.Percentage as VValue  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c,(" + populationCommandText + ") d ," +
                                " (select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentageEntries where percentageid=77 and normalizationstate in (0,1)) e" +
                                " where  a.CColumn=e.TargetColumn and a.Row=e.TargetRow and  d.CColumn= e.SourceColumn and d.Row= e.SourceRow and " +
       " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100)" + " and b.Prevalence='" + strbPrevalence + "' " +
       "  and c.IncidenceDatasetID={0} and b.StartAge={1} and b.EndAge={2} ", iid, Convert.ToInt32(dr["StartAge"]), Convert.ToInt32(dr["EndAge"]));

                        }
                        else
                        {
                            populationCommandText = getPopulationComandTextFromCRSelectFunction(cr, benMAPPopulation, dicRace, dicEthnicity, dicGender);
                            commandText = commandText + string.Format("select  a.IncidenceRateID,a.CColumn,a.Row,a.VValue*d.VValue as VValue  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c,(" + populationCommandText + ") d  where  d.CColumn=a.CColumn and a.Row=d.Row and " +
       " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100)" + " and b.Prevalence='" + strbPrevalence + "' " +
       "  and c.IncidenceDatasetID={0} and b.StartAge={1} and b.EndAge={2} ", iid, Convert.ToInt32(dr["StartAge"]), Convert.ToInt32(dr["EndAge"]));
                        }
                    }
                    else if (crSelectFunction.StartAge >= Convert.ToInt32(dr["StartAge"]) && crSelectFunction.EndAge <= Convert.ToInt32(dr["EndAge"]))
                    {
                        CRSelectFunction cr = new CRSelectFunction() { StartAge = crSelectFunction.StartAge, EndAge = crSelectFunction.EndAge, Ethnicity = crSelectFunction.Ethnicity, Gender = crSelectFunction.Gender, Race = crSelectFunction.Race };
                        if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4)
                        {
                            populationCommandText = getPopulationComandTextFromCRSelectFunction(cr, CommonClass.BenMAPPopulation, dicRace, dicEthnicity, dicGender);
                            commandText = commandText + string.Format("select  e.SourceColumn as CColumn,e.SourceRow as Row,a.VValue*d.VValue*e.Percentage as VValue  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c,(" + populationCommandText + ") d ," +
                                " (select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentageEntries where percentageid=77 and normalizationstate in (0,1)) e" +
                                " where  a.CColumn=e.TargetColumn and a.Row=e.TargetRow and  d.CColumn= e.SourceColumn and d.Row= e.SourceRow and " +
       " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100)" + " and b.Prevalence='" + strbPrevalence + "' " +
       "  and c.IncidenceDatasetID={0} and b.StartAge={1} and b.EndAge={2} ", iid, Convert.ToInt32(dr["StartAge"]), Convert.ToInt32(dr["EndAge"]));

                        }
                        else
                        {
                            populationCommandText = getPopulationComandTextFromCRSelectFunction(cr, benMAPPopulation, dicRace, dicEthnicity, dicGender);
                            commandText = commandText + string.Format("select  a.IncidenceRateID,a.CColumn,a.Row,a.VValue*d.VValue as VValue  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c,(" + populationCommandText + ") d  where  d.CColumn=a.CColumn and a.Row=d.Row and " +
        " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100)" + " and b.Prevalence='" + strbPrevalence + "' " +
        "  and c.IncidenceDatasetID={0} and b.StartAge={1} and b.EndAge={2} ", iid, Convert.ToInt32(dr["StartAge"]), Convert.ToInt32(dr["EndAge"]));
                        }
                    }
                    else if (crSelectFunction.StartAge >= Convert.ToInt32(dr["StartAge"]) && (crSelectFunction.EndAge > Convert.ToInt32(dr["EndAge"]) || crSelectFunction.EndAge == -1))
                    {
                        CRSelectFunction cr = new CRSelectFunction() { StartAge = crSelectFunction.StartAge, EndAge = Convert.ToInt32(dr["EndAge"]), Ethnicity = crSelectFunction.Ethnicity, Gender = crSelectFunction.Gender, Race = crSelectFunction.Race };
                        if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4)
                        {
                            populationCommandText = getPopulationComandTextFromCRSelectFunction(cr, CommonClass.BenMAPPopulation, dicRace, dicEthnicity, dicGender);
                            commandText = commandText + string.Format("select  e.SourceColumn as CColumn,e.SourceRow as Row,a.VValue*d.VValue*e.Percentage as VValue  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c,(" + populationCommandText + ") d ," +
                                " (select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentageEntries where percentageid=77 and normalizationstate in (0,1)) e" +
                                " where  a.CColumn=e.TargetColumn and a.Row=e.TargetRow and  d.CColumn= e.SourceColumn and d.Row= e.SourceRow and " +
       " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100)" + " and b.Prevalence='" + strbPrevalence + "' " +
       "  and c.IncidenceDatasetID={0} and b.StartAge={1} and b.EndAge={2} ", iid, Convert.ToInt32(dr["StartAge"]), Convert.ToInt32(dr["EndAge"]));

                        }
                        else
                        {
                            populationCommandText = getPopulationComandTextFromCRSelectFunction(cr, benMAPPopulation, dicRace, dicEthnicity, dicGender);
                            commandText = commandText + string.Format("select  a.IncidenceRateID,a.CColumn,a.Row,a.VValue*d.VValue as VValue  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c,(" + populationCommandText + ") d  where  d.CColumn=a.CColumn and a.Row=d.Row and " +
       " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100)" + " and b.Prevalence='" + strbPrevalence + "' " +
       "  and c.IncidenceDatasetID={0} and b.StartAge={1} and b.EndAge={2} ", iid, Convert.ToInt32(dr["StartAge"]), Convert.ToInt32(dr["EndAge"]));
                        }
                    }
                    else if ((crSelectFunction.StartAge < Convert.ToInt32(dr["StartAge"]) || crSelectFunction.StartAge == -1) && crSelectFunction.EndAge <= Convert.ToInt32(dr["EndAge"]))
                    {
                        CRSelectFunction cr = new CRSelectFunction() { StartAge = Convert.ToInt32(dr["StartAge"]), EndAge = crSelectFunction.EndAge, Ethnicity = crSelectFunction.Ethnicity, Gender = crSelectFunction.Gender, Race = crSelectFunction.Race };
                        if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4)
                        {
                            populationCommandText = getPopulationComandTextFromCRSelectFunction(cr, CommonClass.BenMAPPopulation, dicRace, dicEthnicity, dicGender);
                            commandText = commandText + string.Format("select  e.SourceColumn as CColumn,e.SourceRow as Row,a.VValue*d.VValue*e.Percentage as VValue  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c,(" + populationCommandText + ") d ," +
                                " (select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentageEntries where percentageid=77 and normalizationstate in (0,1)) e" +
                                " where  a.CColumn=e.TargetColumn and a.Row=e.TargetRow and  d.CColumn= e.SourceColumn and d.Row= e.SourceRow and " +
       " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100)" + " and b.Prevalence='" + strbPrevalence + "' " +
       "  and c.IncidenceDatasetID={0} and b.StartAge={1} and b.EndAge={2} ", iid, Convert.ToInt32(dr["StartAge"]), Convert.ToInt32(dr["EndAge"]));

                        }
                        else
                        {
                            populationCommandText = getPopulationComandTextFromCRSelectFunction(cr, benMAPPopulation, dicRace, dicEthnicity, dicGender);
                            commandText = commandText + string.Format("select  a.IncidenceRateID,a.CColumn,a.Row,a.VValue*d.VValue as VValue  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c,(" + populationCommandText + ") d  where  d.CColumn=a.CColumn and a.Row=d.Row and " +
       " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100)" + " and b.Prevalence='" + strbPrevalence + "' " +
       "  and c.IncidenceDatasetID={0} and b.StartAge={1} and b.EndAge={2} ", iid, Convert.ToInt32(dr["StartAge"]), Convert.ToInt32(dr["EndAge"]));
                        }
                    }
                    if (!string.IsNullOrEmpty(crSelectFunction.Race))
                    {
                        if (dicRace.Keys.Contains(crSelectFunction.Race))
                        {
                            commandText = string.Format(commandText + " and (b.RaceID={0} or b.RaceID=6)", dicRace[crSelectFunction.Race]);
                        }
                    }
                    if (!string.IsNullOrEmpty(crSelectFunction.Ethnicity))
                    {
                        if (dicEthnicity.Keys.Contains(crSelectFunction.Ethnicity))
                        {
                            commandText = string.Format(commandText + " and (b.EthnicityID={0} or b.EthnicityID=4)", dicEthnicity[crSelectFunction.Ethnicity]);

                        }
                    }
                    if (!string.IsNullOrEmpty(crSelectFunction.Gender))
                    {
                        if (dicGender.Keys.Contains(crSelectFunction.Gender))
                        {
                            commandText = string.Format(commandText + " and (b.GenderID={0} or b.GenderID=4)", dicGender[crSelectFunction.Gender]);
                        }
                    }
                }






                if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4)
                {
                    populationCommandText = getPopulationComandTextFromCRSelectFunction(crSelectFunction, CommonClass.BenMAPPopulation, dicRace, dicEthnicity, dicGender);
                }
                else
                {
                    populationCommandText = getPopulationComandTextFromCRSelectFunction(crSelectFunction, benMAPPopulation, dicRace, dicEthnicity, dicGender);
                }
                commandText = "select a.CColumn,a.Row,sum(a.VValue/b.VValue)  as VValue from ( " + commandText + " ) a,(" + populationCommandText + ") b where a.CColumn=b.CColumn and a.Row=b.Row group by a.CColumn,a.Row";
                Dictionary<string, double> dicPercentage = new Dictionary<string, double>();
                Dictionary<string, Dictionary<string, double>> dicRelationShip = new Dictionary<string, Dictionary<string, double>>();
                if ((CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 28 || CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 27) && CommonClass.MainSetup.SetupID == 1)
                {
                    string str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=77 and normalizationstate in (0,1)";
                    DataSet dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);

                    foreach (DataRow dr in dsPercentage.Tables[0].Rows)
                    {
                        if (dicRelationShip.ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
                        {
                            if (!dicRelationShip[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].ContainsKey(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()))
                                dicRelationShip[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add((Convert.ToInt32(dr["targetcolumn"]) * 10000 + Convert.ToInt32(dr["targetrow"].ToString())).ToString(), Convert.ToDouble(dr["Percentage"]));
                        }
                        else
                        {
                            dicRelationShip.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), new Dictionary<string, double>());
                            dicRelationShip[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add((Convert.ToInt32(dr["targetcolumn"]) * 10000 + Convert.ToInt32(dr["targetrow"].ToString())).ToString(), Convert.ToDouble(dr["Percentage"]));
                        }

                    }
                    foreach (DataRow dr in dsPercentage.Tables[0].Rows)
                    {
                        dicPercentage.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString() + "," + dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["percentage"]));
                    }
                    dsPercentage.Dispose();
                }
                if (dsAge != null)
                    dsAge.Dispose();
                if (dsIncidence != null)
                    dsIncidence.Dispose();
                if (dsPrevalence != null)
                    dsPrevalence.Dispose();
                if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && commonYear != benMAPPopulation.Year && dicPopulationAge != null && dicPopulationAge.Count > 0)
                {
                    string strEndAgeOri = " CASE" +
                       " WHEN (b.EndAge> " + crSelectFunction.EndAge + ") THEN " + crSelectFunction.EndAge + " ELSE b.EndAge END ";
                    string strStartAgeOri = " CASE" +
                        " WHEN (b.StartAge< " + crSelectFunction.StartAge + ") THEN " + crSelectFunction.StartAge + " ELSE b.StartAge END ";
                    string strAgeID = string.Format(" select a.startAge,a.EndAge,b.AgeRangeid, " +
  " CASE" +
  " WHEN (b.startAge>=a.StartAge and b.EndAge<=a.EndAge) THEN 1" +
  " WHEN (b.startAge<a.StartAge and b.EndAge<=a.EndAge) THEN  Cast(({1}-a.StartAge+1) as float)/({1}-{0}+1)" + " WHEN (b.startAge<a.StartAge and b.EndAge>a.EndAge) THEN Cast(({1}-{0}+1) as float)/({1}-{0}+1)" +
  "  WHEN (b.startAge>=a.StartAge and b.EndAge>a.EndAge) THEN Cast((a.EndAge-{0}+1) as float)/({1}-{0}+1)" +
  " ELSE 1" +
  " END as weight,b.StartAge as sourceStartAge,b.EndAge as SourceEndAge" +
  "  from ( select distinct startage,endage from Incidencerates )a,ageranges b" +
  " where b.EndAge>=a.StartAge and b.StartAge<=a.EndAge", strStartAgeOri, strEndAgeOri);



                    string strInc = string.Format("select  a.CColumn,a.Row,sum(a.VValue*d.Weight) as VValue,d.AgeRangeID  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c ,(" + strAgeID +
                       ") d where   b.StartAge=d.StartAge and b.EndAge=d.EndAge and " +
               " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and b.RaceID=6 and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100)" + " and b.Prevalence='" + strbPrevalence + "' " +
               "  and c.IncidenceDatasetID={0} and b.StartAge<={2} and b.EndAge>={1} group by a.CColumn,a.Row ,d.AgeRangeID", iid, crSelectFunction.StartAge, crSelectFunction.EndAge);
                    DataSet dsInc = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, strInc);

                    Dictionary<string, double> dicInc = new Dictionary<string, double>();
                    foreach (DataRow dr in dsInc.Tables[0].Rows)
                    {
                        if (!dicInc.ContainsKey((Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString() + "," + dr["AgeRangeID"]))
                        {
                            dicInc.Add((Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString() + "," + dr["AgeRangeID"].ToString(), Convert.ToDouble(dr["VValue"]));
                        }
                    }
                    dsInc.Dispose();
                    Dictionary<int, double> dicPopInc = new Dictionary<int, double>();

                    foreach (KeyValuePair<string, double> k in dicPopulationAge)
                    {
                        string[] s = k.Key.Split(new char[] { ',' });
                        double dp = 0;

                        if (dicRelationShip.ContainsKey(s[0] + "," + s[1]))
                        {
                            foreach (KeyValuePair<string, double> kin in dicRelationShip[s[0] + "," + s[1]])
                            {
                                dp += dicInc[kin.Key + "," + s[2]] * kin.Value;
                            }
                        }


                        if (dicPopInc.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])) && dicPopulation12.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                        {

                            dicPopInc[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])] += dp * Convert.ToDouble(k.Value) / dicPopulation12[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])];

                        }
                        else
                        {
                            dicPopInc.Add(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1]), dp * Convert.ToDouble(k.Value) / dicPopulation12[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])]);
                        }

                    }
                    dicIncidenceRateAttribute = dicPopInc;
                }
                else
                {
                    string strPopInc = getPopulationComandTextFromCRSelectFunctionForInc(crSelectFunction, CommonClass.BenMAPPopulation, dicRace, dicEthnicity, dicGender);

                    string strAgeID = " select a.startAge,a.EndAge,b.AgeRangeid, " +
 " CASE" +
 " WHEN (b.startAge>=a.StartAge and b.EndAge<=a.EndAge) THEN 1" +
 " WHEN (b.startAge<a.StartAge and b.EndAge<=a.EndAge) THEN Cast((b.EndAge-a.StartAge+1) as float)/(b.EndAge-b.StartAge+1)" +
 " WHEN (b.startAge<a.StartAge and b.EndAge>a.EndAge) THEN Cast((b.EndAge-b.StartAge+1) as float)/(b.EndAge-b.StartAge+1)" +
 "  WHEN (b.startAge>=a.StartAge and b.EndAge>a.EndAge) THEN Cast((a.EndAge-b.StartAge+1) as float)/(b.EndAge-b.StartAge+1)" +
 " ELSE 1" +
 " END as weight,b.StartAge as sourceStartAge,b.EndAge as SourceEndAge" +
 "  from ( select distinct startage,endage from Incidencerates )a,ageranges b" +
 " where b.EndAge>=a.StartAge and b.StartAge<=a.EndAge";
                    string strInc = string.Format("select  a.CColumn,a.Row,sum(a.VValue*d.weight) as VValue,d.AgeRangeID  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c ,(" + strAgeID +
     ") d where   b.StartAge=d.StartAge and b.EndAge=d.EndAge and " +
" a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100)" + " and b.Prevalence='" + strbPrevalence + "' " +
"  and c.IncidenceDatasetID={0} and b.StartAge<={2} and b.EndAge>={1} group by a.CColumn,a.Row,d.AgeRangeID", iid, crSelectFunction.StartAge, crSelectFunction.EndAge);

                    string strTemp = "select a.CColumn,a.Row,sum(a.VValue*b.VValue) as VValue from (" + strPopInc + ") a,(" + strInc + ") b  ," +
                         "(select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=77 and normalizationstate in (0,1)) c" +
                         " where a.CColumn= c.sourcecolumn and a.Row=c.targetcolumn and b.CColumn=c.targetcolumn and b.Row=c.targetrow and a.agerangeid=b.agerangeid group by a.ccolumn,a.row";
                    DataSet dsInc = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, strInc);
                    DataSet dsPopInc = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, strPopInc);
                    Dictionary<string, double> dicInc = new Dictionary<string, double>();
                    foreach (DataRow dr in dsInc.Tables[0].Rows)
                    {
                        if (!dicInc.ContainsKey((Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString() + "," + dr["AgeRangeID"]))
                        {
                            dicInc.Add((Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString() + "," + dr["AgeRangeID"].ToString(), Convert.ToDouble(dr["VValue"]));
                        }
                    }

                    double dp = 0;
                    Dictionary<int, double> dicPopInc = new Dictionary<int, double>();
                    foreach (DataRow dr in dsPopInc.Tables[0].Rows)
                    {
                        dp = 0;
                        if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4)
                        {
                            if (dicRelationShip.ContainsKey(dr["CColumn"].ToString() + "," + dr["Row"]))
                            {
                                foreach (KeyValuePair<string, double> k in dicRelationShip[dr["CColumn"].ToString() + "," + dr["Row"]])
                                {
                                    dp += dicInc[k.Key + "," + dr["AgeRangeID"]] * k.Value;
                                }
                            }
                        }
                        else
                        {
                            dp = dicInc[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"]) + "," + dr["AgeRangeID"]];
                        }

                        if (dicPopInc.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && dicPopulation.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])))
                        {
                            if (CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 27 || CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 28)
                            {
                                if (dicPopulation12.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && dicPopInc.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && dicPopulation12[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])] != 0)
                                    dicPopInc[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])] += dp * Convert.ToDouble(dr["VValue"]) / dicPopulation12[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])];
                            }
                            else if (dicPopInc.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])))
                                dicPopInc[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])] += dp * Convert.ToDouble(dr["VValue"]) / dicPopulation[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])];

                        }
                        else
                        {
                            if (CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 27 || CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 28)
                            {
                                if (dicPopulation12.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && !dicPopInc.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && dicPopulation12[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])] != 0)
                                    dicPopInc.Add(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"]), dp * Convert.ToDouble(dr["VValue"]) / dicPopulation12[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])]);
                            }
                            else if (dicPopulation.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && !dicPopInc.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])))
                                dicPopInc.Add(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"]), dp * Convert.ToDouble(dr["VValue"]) / dicPopulation[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])]);
                        }
                        if (dicPopInc.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && double.IsNaN(dicPopInc[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])]))
                        {
                        }
                    }
                    dicIncidenceRateAttribute = dicPopInc;
                }






                if (!bPrevalence)
                    commandText = string.Format("select GriddefinitionID from IncidenceDatasets where IncidenceDatasetID={0}", crSelectFunction.IncidenceDataSetID);
                else
                    commandText = string.Format("select GriddefinitionID from IncidenceDatasets where IncidenceDatasetID={0}", crSelectFunction.PrevalenceDataSetID);
                int incidenceDataSetGridType = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4)
                {
                    incidenceDataSetGridType = CommonClass.BenMAPPopulation.GridType.GridDefinitionID;
                    foreach (GridRelationship gRelationship in CommonClass.LstGridRelationshipAll)
                    {
                        if ((gRelationship.bigGridID == incidenceDataSetGridType && gRelationship.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID) || (gRelationship.smallGridID == incidenceDataSetGridType && gRelationship.bigGridID == CommonClass.GBenMAPGrid.GridDefinitionID))
                        {
                            gridRelationShipPopulation = gRelationship;
                        }
                    }
                }
                Dictionary<int, double> dicResult = new Dictionary<int, double>();
                double IncidenceValue = 0;
                if (incidenceDataSetGridType == GridDefinitionID)
                {
                    dicResult = dicIncidenceRateAttribute;
                }
                else if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && (CommonClass.GBenMAPGrid.GridDefinitionID == CommonClass.BenMAPPopulation.GridType.GridDefinitionID || (CommonClass.GBenMAPGrid.GridDefinitionID == 27 && CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 28) || (CommonClass.GBenMAPGrid.GridDefinitionID == 28 && CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 27)))
                {
                    dicResult = dicIncidenceRateAttribute;
                }
                else
                {
                    dicRelationShip = APVX.APVCommonClass.getRelationFromDicRelationShipAll(gridRelationShipPopulation);
                    if (incidenceDataSetGridType == gridRelationShipPopulation.bigGridID)
                    {
                        if (dicRelationShip != null && dicRelationShip.Count > 0)
                        {
                            foreach (KeyValuePair<string, Dictionary<string, double>> k in dicRelationShip)
                            {
                                string[] s = k.Key.Split(new char[] { ',' });
                                if (dicIncidenceRateAttribute.Keys.Contains(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                                {
                                    foreach (KeyValuePair<string, double> rc in k.Value)
                                    {
                                        string[] sin = rc.Key.Split(new char[] { ',' });
                                        if (!dicResult.Keys.Contains(Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])))
                                        {

                                            dicResult.Add(Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1]), dicIncidenceRateAttribute[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])] * rc.Value);

                                        }
                                        else
                                        {

                                            dicResult[Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])] += dicIncidenceRateAttribute[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])] * rc.Value;

                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            foreach (GridRelationshipAttribute gra in gridRelationShipPopulation.lstGridRelationshipAttribute)
                            {
                                if (dicIncidenceRateAttribute.Keys.Contains(Convert.ToInt32(gra.bigGridRowCol.Col) * 10000 + Convert.ToInt32(gra.bigGridRowCol.Row)))
                                {
                                    foreach (RowCol rc in gra.smallGridRowCol)
                                    {
                                        if (!dicResult.Keys.Contains(Convert.ToInt32(rc.Col) * 10000 + Convert.ToInt32(rc.Row)))
                                        {
                                            if (!dicPercentage.ContainsKey(rc.Col + "," + rc.Row + "," + gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row))
                                                dicResult.Add(Convert.ToInt32(rc.Col) * 10000 + Convert.ToInt32(rc.Row), dicIncidenceRateAttribute[Convert.ToInt32(gra.bigGridRowCol.Col) * 10000 + Convert.ToInt32(gra.bigGridRowCol.Row)]);
                                            else
                                            {
                                                dicResult.Add(Convert.ToInt32(rc.Col) * 10000 + Convert.ToInt32(rc.Row), dicIncidenceRateAttribute[Convert.ToInt32(gra.bigGridRowCol.Col) * 10000 + Convert.ToInt32(gra.bigGridRowCol.Row)] * dicPercentage[rc.Col + "," + rc.Row + "," + gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row]);
                                            }
                                        }
                                        else
                                        {
                                            if (dicPercentage.ContainsKey(rc.Col + "," + rc.Row + "," + gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row))
                                                dicResult[Convert.ToInt32(rc.Col) * 10000 + Convert.ToInt32(rc.Row)] += dicIncidenceRateAttribute[Convert.ToInt32(gra.bigGridRowCol.Col) * 10000 + Convert.ToInt32(gra.bigGridRowCol.Row)] * dicPercentage[rc.Col + "," + rc.Row + "," + gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row];

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
                                double d = 0;
                                if (k.Value != null && k.Value.Count > 0)
                                {
                                    foreach (KeyValuePair<string, double> rc in k.Value)
                                    {
                                        string[] sin = rc.Key.Split(new char[] { ',' });
                                        if (dicIncidenceRateAttribute.Keys.Contains(Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])) && !double.IsNaN(dicIncidenceRateAttribute[Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])]))
                                        {
                                            d = (d + dicIncidenceRateAttribute[Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])]);
                                        }
                                    }
                                    d = d / k.Value.Count;
                                    if (!dicResult.Keys.Contains(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                                        dicResult.Add(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1]), d);

                                }
                            }
                        }
                        else
                        {
                            foreach (GridRelationshipAttribute gra in gridRelationShipPopulation.lstGridRelationshipAttribute)
                            {
                                if (gra.bigGridRowCol.Col == 75 && gra.bigGridRowCol.Row == 19)
                                {
                                }
                                double d = 0;
                                foreach (RowCol rc in gra.smallGridRowCol)
                                {
                                    if (dicIncidenceRateAttribute.Keys.Contains(Convert.ToInt32(rc.Col) * 10000 + Convert.ToInt32(rc.Row)) && !double.IsNaN(dicIncidenceRateAttribute[Convert.ToInt32(rc.Col) * 10000 + Convert.ToInt32(rc.Row)]))
                                    {
                                        d = (d + dicIncidenceRateAttribute[Convert.ToInt32(rc.Col) * 10000 + Convert.ToInt32(rc.Row)]);
                                    }

                                }
                                d = d / gra.smallGridRowCol.Count;
                                if (!dicResult.Keys.Contains(Convert.ToInt32(gra.bigGridRowCol.Col) * 10000 + Convert.ToInt32(gra.bigGridRowCol.Row)))
                                    dicResult.Add(Convert.ToInt32(gra.bigGridRowCol.Col) * 10000 + Convert.ToInt32(gra.bigGridRowCol.Row), d);



                            }
                        }
                    }

                }
                if (dsIncidence != null)
                    dsIncidence.Dispose();
                return dicResult;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static Dictionary<int, double> getIncidenceDataSetFromCRSelectFuntionDic(Dictionary<int, double> dicPopulation, Dictionary<string, double> dicPopulationAge, Dictionary<int, double> dicPopulation12, CRSelectFunction crSelectFunction, bool bPrevalence, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, int GridDefinitionID, GridRelationship gridRelationShipPopulation)
        {
            try
            {

                Dictionary<int, double> dicIncidenceRateAttribute = new Dictionary<int, double>();
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet dsIncidence = null;
                DataSet dsPrevalence = null;
                string strbPrevalence = "F";
                int iid = crSelectFunction.IncidenceDataSetID;
                if (bPrevalence)
                {
                    strbPrevalence = "T";
                    iid = crSelectFunction.PrevalenceDataSetID;
                }
                string commandText = "";

                int iPopulationDataSetID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, string.Format("select PopulationDataSetID from PopulationDataSets where SetupID={0} and GridDefinitionID= (select GridDefinitionID from IncidenceDataSets where IncidenceDataSetID={1} )", CommonClass.MainSetup.SetupID, iid)));
                int iPopulationDataSetGridID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, string.Format("select GridDefinitionID from IncidenceDataSets where IncidenceDataSetID={1} ", CommonClass.MainSetup.SetupID, iid)));

                BenMAPPopulation benMAPPopulation = new BenMAPPopulation() { DataSetID = iPopulationDataSetID, GridType = new BenMAPGrid() { GridDefinitionID = iPopulationDataSetGridID }, Year = CommonClass.BenMAPPopulation.Year };
                commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", iPopulationDataSetID); int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
                string populationCommandText = getPopulationComandTextFromCRSelectFunction(crSelectFunction, benMAPPopulation, dicRace, dicEthnicity, dicGender);
                commandText = "";
                string strRace = "";
                if (CommonClass.MainSetup.SetupID == 1) strRace = " and b.RaceID=6";
                string strbEndAgeOri = " CASE" +
                       " WHEN (b.EndAge> " + crSelectFunction.EndAge + ") THEN " + crSelectFunction.EndAge + " ELSE b.EndAge END ";
                string strbStartAgeOri = " CASE" +
                    " WHEN (b.StartAge< " + crSelectFunction.StartAge + ") THEN " + crSelectFunction.StartAge + " ELSE b.StartAge END ";
                string straEndAgeOri = " CASE" +
                      " WHEN (a.EndAge> " + crSelectFunction.EndAge + ") THEN " + crSelectFunction.EndAge + " ELSE a.EndAge END ";
                string straStartAgeOri = " CASE" +
                    " WHEN (a.StartAge< " + crSelectFunction.StartAge + ") THEN " + crSelectFunction.StartAge + " ELSE a.StartAge END ";
                string strAgeID = string.Format(" select a.startAge,a.EndAge,b.AgeRangeid, " +
  " CASE" +
  " WHEN (b.startAge>=a.StartAge and b.EndAge<=a.EndAge) THEN 1" +
  " WHEN (b.startAge<a.StartAge and b.EndAge<=a.EndAge) THEN  Cast(({3}-{0}+1) as float)/({3}-{2}+1)" + " WHEN (b.startAge<a.StartAge and b.EndAge>a.EndAge) THEN Cast(({1}-{0}+1) as float)/({3}-{2}+1)" +
  "  WHEN (b.startAge>=a.StartAge and b.EndAge>a.EndAge) THEN Cast(({1}-{2}+1) as float)/({3}-{2}+1)" +
  " ELSE 1" +
  " END as weight,b.StartAge as sourceStartAge,b.EndAge as SourceEndAge" +
  "  from ( select distinct startage,endage from Incidencerates  where IncidenceDataSetID=" + iid + ")a,ageranges b" +
  " where b.EndAge>=a.StartAge and b.StartAge<=a.EndAge", straStartAgeOri, straEndAgeOri, strbStartAgeOri, strbEndAgeOri);

                string strInc = string.Format("select  a.CColumn,a.Row,sum(a.VValue*d.Weight) as VValue,d.AgeRangeID  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c ,(" + strAgeID +
                     ") d where   b.StartAge=d.StartAge and b.EndAge=d.EndAge and " +
             " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + strRace + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100)" + " and b.Prevalence='" + strbPrevalence + "' " +
             "  and c.IncidenceDatasetID={0} and b.StartAge<={2} and b.EndAge>={1} group by a.CColumn,a.Row ,d.AgeRangeID", iid, crSelectFunction.StartAge, crSelectFunction.EndAge);
                Dictionary<string, double> dicPercentage = new Dictionary<string, double>();
                Dictionary<string, Dictionary<string, double>> dicRelationShip = new Dictionary<string, Dictionary<string, double>>();
                if ((CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 28 || CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 27) && CommonClass.MainSetup.SetupID == 1)
                {
                    string str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=77 and normalizationstate in (0,1)";
                    DataSet dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);

                    foreach (DataRow dr in dsPercentage.Tables[0].Rows)
                    {
                        if (dicRelationShip.ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
                        {
                            if (!dicRelationShip[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].ContainsKey(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()))
                                dicRelationShip[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add((Convert.ToInt32(dr["targetcolumn"]) * 10000 + Convert.ToInt32(dr["targetrow"].ToString())).ToString(), Convert.ToDouble(dr["Percentage"]));
                        }
                        else
                        {
                            dicRelationShip.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), new Dictionary<string, double>());
                            dicRelationShip[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add((Convert.ToInt32(dr["targetcolumn"]) * 10000 + Convert.ToInt32(dr["targetrow"].ToString())).ToString(), Convert.ToDouble(dr["Percentage"]));
                        }

                    }
                    foreach (DataRow dr in dsPercentage.Tables[0].Rows)
                    {
                        dicPercentage.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString() + "," + dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["percentage"]));
                    }
                    dsPercentage.Dispose();
                }

                if (dsIncidence != null)
                    dsIncidence.Dispose();
                if (dsPrevalence != null)
                    dsPrevalence.Dispose();
                if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && dicPopulationAge != null && dicPopulationAge.Count > 0)
                {

                    DataSet dsInc = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, strInc);

                    Dictionary<string, double> dicInc = new Dictionary<string, double>();
                    foreach (DataRow dr in dsInc.Tables[0].Rows)
                    {
                        if (!dicInc.ContainsKey((Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString() + "," + dr["AgeRangeID"]))
                        {
                            dicInc.Add((Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString() + "," + dr["AgeRangeID"].ToString(), Convert.ToDouble(dr["VValue"]));
                        }
                    }
                    dsInc.Dispose();
                    if (CommonClass.GBenMAPGrid.GridDefinitionID == 27 || CommonClass.GBenMAPGrid.GridDefinitionID == 28)
                    {
                        Dictionary<int, double> dicPopInc = new Dictionary<int, double>();

                        foreach (KeyValuePair<string, double> k in dicPopulationAge)
                        {
                            string[] s = k.Key.Split(new char[] { ',' });
                            double dp = 0;

                            if (dicRelationShip.ContainsKey(s[0] + "," + s[1]))
                            {
                                foreach (KeyValuePair<string, double> kin in dicRelationShip[s[0] + "," + s[1]])
                                {
                                    dp += dicInc[kin.Key + "," + s[2]] * kin.Value;
                                }
                            }


                            if (dicPopInc.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])) && dicPopulation12.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                            {

                                dicPopInc[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])] += dp * Convert.ToDouble(k.Value) / dicPopulation12[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])];

                            }
                            else
                            {
                                dicPopInc.Add(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1]), dp * Convert.ToDouble(k.Value) / dicPopulation12[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])]);
                            }

                        }
                        dicIncidenceRateAttribute = dicPopInc;
                    }
                    else
                    {
                        string str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =27 and  targetgriddefinitionid = " + CommonClass.GBenMAPGrid.GridDefinitionID + " ) and normalizationstate in (0,1)";
                        DataSet dsPercentage = null;
                        Dictionary<string, Dictionary<string, double>> dicRelationShipForAggregation = new Dictionary<string, Dictionary<string, double>>();
                        try
                        {
                            dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);

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




                            int iPercentageID = 0; Dictionary<string, Dictionary<string, double>> dicPercentageForAggregationInc = new Dictionary<string, Dictionary<string, double>>();
                            try
                            {
                                iPercentageID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, "select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + CommonClass.GBenMAPGrid.GridDefinitionID + " and  targetgriddefinitionid = " + iPopulationDataSetGridID));
                                str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( " + iPercentageID + " ) and normalizationstate in (0,1)";
                                dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
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


                                dsPercentage.Dispose();
                                Dictionary<int, double> dicPopInc = new Dictionary<int, double>();

                                foreach (KeyValuePair<string, double> k in dicPopulationAge)
                                {
                                    string[] s = k.Key.Split(new char[] { ',' });
                                    double dp = 0;
                                    if (s[0] == "36" && s[1] == "60")
                                    {

                                    }
                                    if (dicPercentageForAggregationInc.ContainsKey(s[0] + "," + s[1]))
                                    {
                                        foreach (KeyValuePair<string, double> kin in dicPercentageForAggregationInc[s[0] + "," + s[1]])
                                        {
                                            string[] sin = kin.Key.Split(new char[] { ',' });
                                            double dsin = Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1]);
                                            if (dicInc.ContainsKey(dsin + "," + s[2]))
                                                dp += dicInc[dsin + "," + s[2]] * kin.Value;
                                        }
                                        dp = dp / dicPercentageForAggregationInc[s[0] + "," + s[1]].Sum(p => p.Value);
                                    }


                                    if (dicPopInc.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])) && dicPopulation.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                                    {

                                        dicPopInc[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])] += dp * Convert.ToDouble(k.Value) / dicPopulation[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])];

                                    }
                                    else if (dicPopulation.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                                    {
                                        dicPopInc.Add(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1]), dp * Convert.ToDouble(k.Value) / dicPopulation[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])]);
                                    }

                                }
                                dicIncidenceRateAttribute = dicPopInc;
                                return dicIncidenceRateAttribute;

                            }
                            catch
                            {
                                try
                                {
                                    iPercentageID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, "select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + CommonClass.GBenMAPGrid.GridDefinitionID + " and  targetgriddefinitionid = " + iPopulationDataSetGridID));
                                    dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
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


                                    dsPercentage.Dispose();
                                    Dictionary<int, double> dicPopInc = new Dictionary<int, double>();

                                    foreach (KeyValuePair<string, double> k in dicPopulationAge)
                                    {
                                        string[] s = k.Key.Split(new char[] { ',' });
                                        double dp = 0, dsum = 0;

                                        if (dicPercentageForAggregationInc.ContainsKey(s[0] + "," + s[1]))
                                        {
                                            foreach (KeyValuePair<string, double> kin in dicPercentageForAggregationInc[s[0] + "," + s[1]])
                                            {

                                                string[] sin = kin.Key.Split(new char[] { ',' });
                                                double dsin = Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1]);
                                                if (dicInc.ContainsKey(dsin + "," + s[2]))
                                                    dp += dicInc[dsin + "," + s[2]] * kin.Value;
                                                dsum += kin.Value;
                                            }
                                            dp = dp / dsum;
                                        }


                                        if (dicPopInc.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])) && dicPopulation.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                                        {

                                            dicPopInc[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])] += dp * Convert.ToDouble(k.Value) / dicPopulation[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])];

                                        }
                                        else if (dicPopulation.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                                        {
                                            dicPopInc.Add(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1]), dp * Convert.ToDouble(k.Value) / dicPopulation[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])]);
                                        }

                                    }
                                    dicIncidenceRateAttribute = dicPopInc;
                                    return dicIncidenceRateAttribute;

                                }
                                catch
                                {
                                }

                            }



                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                else
                {
                    string strPopInc = getPopulationComandTextFromCRSelectFunctionForInc(crSelectFunction, CommonClass.BenMAPPopulation, dicRace, dicEthnicity, dicGender);


                    string strTemp = "select a.CColumn,a.Row,sum(a.VValue*b.VValue) as VValue from (" + strPopInc + ") a,(" + strInc + ") b  ," +
                         "(select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=77 and normalizationstate in (0,1)) c" +
                         " where a.CColumn= c.sourcecolumn and a.Row=c.targetcolumn and b.CColumn=c.targetcolumn and b.Row=c.targetrow and a.agerangeid=b.agerangeid group by a.ccolumn,a.row";
                    DataSet dsInc = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, strInc);
                    DataSet dsPopInc = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, strPopInc);
                    Dictionary<string, double> dicInc = new Dictionary<string, double>();
                    foreach (DataRow dr in dsInc.Tables[0].Rows)
                    {
                        if (!dicInc.ContainsKey((Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString() + "," + dr["AgeRangeID"]))
                        {
                            dicInc.Add((Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString() + "," + dr["AgeRangeID"].ToString(), Convert.ToDouble(dr["VValue"]));
                        }
                    }


                    double dp = 0;
                    Dictionary<int, double> dicPopInc = new Dictionary<int, double>();
                    foreach (DataRow dr in dsPopInc.Tables[0].Rows)
                    {
                        dp = 0;
                        if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4)
                        {
                            if (dicRelationShip.ContainsKey(dr["CColumn"].ToString() + "," + dr["Row"]))
                            {
                                foreach (KeyValuePair<string, double> k in dicRelationShip[dr["CColumn"].ToString() + "," + dr["Row"]])
                                {
                                    dp += dicInc[k.Key + "," + dr["AgeRangeID"]] * k.Value;
                                }
                            }
                        }
                        else
                        {
                            if (dicInc.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"]) + "," + dr["AgeRangeID"]))
                                dp = dicInc[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"]) + "," + dr["AgeRangeID"]];
                        }


                        if (dicPopInc.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && dicPopulation12.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])))
                        {
                            dicPopInc[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])] += dp * Convert.ToDouble(dr["VValue"]) / dicPopulation12[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])];

                        }
                        else
                        {

                            if (dicPopulation12.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && !dicPopInc.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && dicPopulation12[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])] != 0)
                                dicPopInc.Add(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"]), dp * Convert.ToDouble(dr["VValue"]) / dicPopulation12[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])]);
                        }
                        if (dicPopInc.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && double.IsNaN(dicPopInc[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])]))
                        {
                        }

                    }
                    dicIncidenceRateAttribute = dicPopInc;
                }



                if (!bPrevalence)
                    commandText = string.Format("select GriddefinitionID from IncidenceDatasets where IncidenceDatasetID={0}", crSelectFunction.IncidenceDataSetID);
                else
                    commandText = string.Format("select GriddefinitionID from IncidenceDatasets where IncidenceDatasetID={0}", crSelectFunction.PrevalenceDataSetID);
                int incidenceDataSetGridType = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4)
                {
                    incidenceDataSetGridType = CommonClass.BenMAPPopulation.GridType.GridDefinitionID;
                    gridRelationShipPopulation = null;
                    foreach (GridRelationship gRelationship in CommonClass.LstGridRelationshipAll)
                    {
                        if ((gRelationship.bigGridID == incidenceDataSetGridType && gRelationship.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID) || (gRelationship.smallGridID == incidenceDataSetGridType && gRelationship.bigGridID == CommonClass.GBenMAPGrid.GridDefinitionID))
                        {
                            gridRelationShipPopulation = gRelationship;
                        }
                    }
                    if (gridRelationShipPopulation == null)
                    {
                        gridRelationShipPopulation = new GridRelationship()
                        {
                            bigGridID = incidenceDataSetGridType == 1 ? 1 : CommonClass.GBenMAPGrid.GridDefinitionID,
                            smallGridID = incidenceDataSetGridType == 1 ? CommonClass.GBenMAPGrid.GridDefinitionID : incidenceDataSetGridType
                        };
                    }
                }
                Dictionary<int, double> dicResult = new Dictionary<int, double>();
                double IncidenceValue = 0;
                if (incidenceDataSetGridType == GridDefinitionID)
                {
                    dicResult = dicIncidenceRateAttribute;
                }
                else if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && (CommonClass.GBenMAPGrid.GridDefinitionID == CommonClass.BenMAPPopulation.GridType.GridDefinitionID || (CommonClass.GBenMAPGrid.GridDefinitionID == 27 && CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 28) || (CommonClass.GBenMAPGrid.GridDefinitionID == 28 && CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 27)))
                {
                    dicResult = dicIncidenceRateAttribute;
                }
                else
                {
                    dicRelationShip = APVX.APVCommonClass.getRelationFromDicRelationShipAll(gridRelationShipPopulation);
                    if (incidenceDataSetGridType == gridRelationShipPopulation.bigGridID)
                    {
                        if (dicRelationShip != null && dicRelationShip.Count > 0)
                        {
                            foreach (KeyValuePair<string, Dictionary<string, double>> k in dicRelationShip)
                            {
                                string[] s = k.Key.Split(new char[] { ',' });
                                if (dicIncidenceRateAttribute.Keys.Contains(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                                {
                                    foreach (KeyValuePair<string, double> rc in k.Value)
                                    {
                                        string[] sin = rc.Key.Split(new char[] { ',' });
                                        if (!dicResult.Keys.Contains(Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])))
                                        {

                                            dicResult.Add(Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1]), dicIncidenceRateAttribute[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])] * rc.Value);

                                        }
                                        else
                                        {

                                            dicResult[Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])] += dicIncidenceRateAttribute[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])] * rc.Value;

                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            foreach (GridRelationshipAttribute gra in gridRelationShipPopulation.lstGridRelationshipAttribute)
                            {
                                if (dicIncidenceRateAttribute.Keys.Contains(Convert.ToInt32(gra.bigGridRowCol.Col) * 10000 + Convert.ToInt32(gra.bigGridRowCol.Row)))
                                {
                                    foreach (RowCol rc in gra.smallGridRowCol)
                                    {
                                        if (!dicResult.Keys.Contains(Convert.ToInt32(rc.Col) * 10000 + Convert.ToInt32(rc.Row)))
                                        {
                                            if (!dicPercentage.ContainsKey(rc.Col + "," + rc.Row + "," + gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row))
                                                dicResult.Add(Convert.ToInt32(rc.Col) * 10000 + Convert.ToInt32(rc.Row), dicIncidenceRateAttribute[Convert.ToInt32(gra.bigGridRowCol.Col) * 10000 + Convert.ToInt32(gra.bigGridRowCol.Row)]);
                                            else
                                            {
                                                dicResult.Add(Convert.ToInt32(rc.Col) * 10000 + Convert.ToInt32(rc.Row), dicIncidenceRateAttribute[Convert.ToInt32(gra.bigGridRowCol.Col) * 10000 + Convert.ToInt32(gra.bigGridRowCol.Row)] * dicPercentage[rc.Col + "," + rc.Row + "," + gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row]);
                                            }
                                        }
                                        else
                                        {
                                            if (dicPercentage.ContainsKey(rc.Col + "," + rc.Row + "," + gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row))
                                                dicResult[Convert.ToInt32(rc.Col) * 10000 + Convert.ToInt32(rc.Row)] += dicIncidenceRateAttribute[Convert.ToInt32(gra.bigGridRowCol.Col) * 10000 + Convert.ToInt32(gra.bigGridRowCol.Row)] * dicPercentage[rc.Col + "," + rc.Row + "," + gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row];

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
                                if (k.Key == "36,60")
                                {

                                }
                                string[] s = k.Key.Split(new char[] { ',' });
                                double d = 0;
                                double dISum = 0;
                                if (k.Value != null && k.Value.Count > 0)
                                {
                                    foreach (KeyValuePair<string, double> rc in k.Value)
                                    {
                                        string[] sin = rc.Key.Split(new char[] { ',' });
                                        if (dicIncidenceRateAttribute.Keys.Contains(Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])) && !double.IsNaN(dicIncidenceRateAttribute[Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])]))
                                        {
                                            try
                                            {

                                                d = (d + dicIncidenceRateAttribute[Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])] * rc.Value);
                                                dISum += rc.Value;
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                    if (dicPopulation.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                                    {
                                        d = d / dISum; if (!dicResult.Keys.Contains(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                                            dicResult.Add(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1]), d);
                                    }

                                }
                            }
                        }
                        else
                        {
                            foreach (GridRelationshipAttribute gra in gridRelationShipPopulation.lstGridRelationshipAttribute)
                            {
                                if (gra.bigGridRowCol.Col == 75 && gra.bigGridRowCol.Row == 19)
                                {
                                }
                                double d = 0;
                                foreach (RowCol rc in gra.smallGridRowCol)
                                {
                                    if (dicIncidenceRateAttribute.Keys.Contains(Convert.ToInt32(rc.Col) * 10000 + Convert.ToInt32(rc.Row)) && !double.IsNaN(dicIncidenceRateAttribute[Convert.ToInt32(rc.Col) * 10000 + Convert.ToInt32(rc.Row)]))
                                    {
                                        d = (d + dicIncidenceRateAttribute[Convert.ToInt32(rc.Col) * 10000 + Convert.ToInt32(rc.Row)]);
                                    }

                                }
                                d = d / gra.smallGridRowCol.Count;
                                if (!dicResult.Keys.Contains(Convert.ToInt32(gra.bigGridRowCol.Col) * 10000 + Convert.ToInt32(gra.bigGridRowCol.Row)))
                                    dicResult.Add(Convert.ToInt32(gra.bigGridRowCol.Col) * 10000 + Convert.ToInt32(gra.bigGridRowCol.Row), d);



                            }
                        }
                    }

                }
                if (dsIncidence != null)
                    dsIncidence.Dispose();
                return dicResult;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static Dictionary<string, double> getDicAge(CRSelectFunction crSelectFunction)
        {
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

            string strwhere = "";
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
                string strbPrevalence = "F";
                int iid = crSelectFunction.IncidenceDataSetID;
                if (bPrevalence)
                {
                    strbPrevalence = "T";
                    iid = crSelectFunction.PrevalenceDataSetID;
                }
                string commandText = "";

                int iPopulationDataSetGridID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, string.Format("select GridDefinitionID from IncidenceDataSets where IncidenceDataSetID={1} ", CommonClass.MainSetup.SetupID, iid)));

                commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", CommonClass.BenMAPPopulation.DataSetID); int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
                commandText = "";
                string strRace = "";
                if (CommonClass.MainSetup.SetupID == 1) strRace = " and (b.RaceID=6 or b.RaceID=5)";
                string strbEndAgeOri = " CASE" +
                       " WHEN (b.EndAge> " + crSelectFunction.EndAge + ") THEN " + crSelectFunction.EndAge + " ELSE b.EndAge END ";
                string strbStartAgeOri = " CASE" +
                    " WHEN (b.StartAge< " + crSelectFunction.StartAge + ") THEN " + crSelectFunction.StartAge + " ELSE b.StartAge END ";
                string straEndAgeOri = " CASE" +
                      " WHEN (a.EndAge> " + crSelectFunction.EndAge + ") THEN " + crSelectFunction.EndAge + " ELSE a.EndAge END ";
                string straStartAgeOri = " CASE" +
                    " WHEN (a.StartAge< " + crSelectFunction.StartAge + ") THEN " + crSelectFunction.StartAge + " ELSE a.StartAge END ";
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

                string strInc = string.Format("select  a.CColumn,a.Row,sum(a.VValue*d.Weight) as VValue,d.AgeRangeID  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c ,(" + strAgeID +
                     ") d where   b.StartAge=d.StartAge and b.EndAge=d.EndAge and " +
             " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + strRace + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100 or b.EndPointID=102)" + " and b.Prevalence='" + strbPrevalence + "' " +
             "  and c.IncidenceDatasetID={0} and b.StartAge<={2} and b.EndAge>={1} group by a.CColumn,a.Row ,d.AgeRangeID", iid, crSelectFunction.StartAge, crSelectFunction.EndAge);
                DataSet dsInc = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, strInc);

                Dictionary<string, double> dicInc = new Dictionary<string, double>();
                foreach (DataRow dr in dsInc.Tables[0].Rows)
                {
                    if (!dicInc.ContainsKey((Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString() + "," + dr["AgeRangeID"]))
                    {
                        dicInc.Add((Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString() + "," + dr["AgeRangeID"].ToString(), Convert.ToDouble(dr["VValue"]));
                    }
                }
                dsInc.Dispose();
                if (iPopulationDataSetGridID == CommonClass.GBenMAPGrid.GridDefinitionID) return dicInc;
                Dictionary<string, Dictionary<string, double>> dicPercentageForAggregationInc = new Dictionary<string, Dictionary<string, double>>();
                try
                {

                    string str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + (CommonClass.GBenMAPGrid.GridDefinitionID == 28 ? 27 : CommonClass.GBenMAPGrid.GridDefinitionID) + " and  targetgriddefinitionid = " + iPopulationDataSetGridID + " ) and normalizationstate in (0,1)";

                    DataSet dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
                    if (dsPercentage.Tables[0].Rows.Count == 0)
                    {
                        creatPercentageToDatabase(iPopulationDataSetGridID, CommonClass.GBenMAPGrid.GridDefinitionID, null);
                        int iPercentageID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, "select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + CommonClass.GBenMAPGrid.GridDefinitionID + " and  targetgriddefinitionid = " + iPopulationDataSetGridID));
                        str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + (CommonClass.GBenMAPGrid.GridDefinitionID == 28 ? 27 : CommonClass.GBenMAPGrid.GridDefinitionID) + " and  targetgriddefinitionid = " + iPopulationDataSetGridID + " ) and normalizationstate in (0,1)";
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
                    foreach (KeyValuePair<string, float> k in dicPopulationAge)
                    {
                        string[] s = k.Key.Split(new char[] { ',' });
                        if (!dicAge.ContainsKey(s[2])) continue;
                        if (dicPercentageForAggregationInc.ContainsKey(s[0] + "," + s[1]))
                        {
                            foreach (KeyValuePair<string, double> kin in dicPercentageForAggregationInc[s[0] + "," + s[1]])
                            {
                                string[] sin = kin.Key.Split(new char[] { ',' });
                                double dsin = Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1]);
                                if (!dicInc.ContainsKey(dsin + "," + s[2])) continue;
                                if (dicReturn.ContainsKey((Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])).ToString() + "," + s[2]))
                                    dicReturn[(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])).ToString() + "," + s[2]] += dicInc[dsin + "," + s[2]] * kin.Value;
                                else
                                    dicReturn.Add((Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])).ToString() + "," + s[2], dicInc[dsin + "," + s[2]] * kin.Value);
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
        public static Dictionary<string, double> getIncidenceDataSetFromCRSelectFuntionDicAllAgeOld(Dictionary<int, float> dicPopulation, Dictionary<string, float> dicPopulationAge, Dictionary<int, float> dicPopulation12, CRSelectFunction crSelectFunction, bool bPrevalence, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, int GridDefinitionID, GridRelationship gridRelationShipPopulation)
        {
            try
            {

                Dictionary<int, double> dicIncidenceRateAttribute = new Dictionary<int, double>();
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string strbPrevalence = "F";
                int iid = crSelectFunction.IncidenceDataSetID;
                if (bPrevalence)
                {
                    strbPrevalence = "T";
                    iid = crSelectFunction.PrevalenceDataSetID;
                }
                string commandText = "";

                int iPopulationDataSetID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, string.Format("select PopulationDataSetID from PopulationDataSets where SetupID={0} and GridDefinitionID= (select GridDefinitionID from IncidenceDataSets where IncidenceDataSetID={1} )", CommonClass.MainSetup.SetupID, iid)));
                int iPopulationDataSetGridID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, string.Format("select GridDefinitionID from IncidenceDataSets where IncidenceDataSetID={1} ", CommonClass.MainSetup.SetupID, iid)));

                BenMAPPopulation benMAPPopulation = new BenMAPPopulation() { DataSetID = iPopulationDataSetID, GridType = new BenMAPGrid() { GridDefinitionID = iPopulationDataSetGridID }, Year = CommonClass.BenMAPPopulation.Year };
                commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", iPopulationDataSetID); int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
                string populationCommandText = getPopulationComandTextFromCRSelectFunction(crSelectFunction, benMAPPopulation, dicRace, dicEthnicity, dicGender);
                commandText = "";
                string strRace = "";
                if (CommonClass.MainSetup.SetupID == 1) strRace = " and (b.RaceID=6 or b.RaceID=5)";
                string strbEndAgeOri = " CASE" +
                       " WHEN (b.EndAge> " + crSelectFunction.EndAge + ") THEN " + crSelectFunction.EndAge + " ELSE b.EndAge END ";
                string strbStartAgeOri = " CASE" +
                    " WHEN (b.StartAge< " + crSelectFunction.StartAge + ") THEN " + crSelectFunction.StartAge + " ELSE b.StartAge END ";
                string straEndAgeOri = " CASE" +
                      " WHEN (a.EndAge> " + crSelectFunction.EndAge + ") THEN " + crSelectFunction.EndAge + " ELSE a.EndAge END ";
                string straStartAgeOri = " CASE" +
                    " WHEN (a.StartAge< " + crSelectFunction.StartAge + ") THEN " + crSelectFunction.StartAge + " ELSE a.StartAge END ";
                string strAgeID = string.Format(" select a.startAge,a.EndAge,b.AgeRangeid, " +
  " CASE" +
  " WHEN (b.startAge>=a.StartAge and b.EndAge<=a.EndAge) THEN 1" +
  " WHEN (b.startAge<a.StartAge and b.EndAge<=a.EndAge) THEN  Cast(({3}-{0}+1) as float)/({3}-{2}+1)" + " WHEN (b.startAge<a.StartAge and b.EndAge>a.EndAge) THEN Cast(({1}-{0}+1) as float)/({3}-{2}+1)" +
  "  WHEN (b.startAge>=a.StartAge and b.EndAge>a.EndAge) THEN Cast(({1}-{2}+1) as float)/({3}-{2}+1)" +
  " ELSE 1" +
  " END as weight,b.StartAge as sourceStartAge,b.EndAge as SourceEndAge" +
  "  from ( select distinct startage,endage from Incidencerates  where IncidenceDataSetID=" + iid + ")a,ageranges b" +
  " where b.EndAge>=a.StartAge and b.StartAge<=a.EndAge", straStartAgeOri, straEndAgeOri, strbStartAgeOri, strbEndAgeOri);

                string strInc = string.Format("select  a.CColumn,a.Row,sum(a.VValue*d.Weight) as VValue,d.AgeRangeID  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c ,(" + strAgeID +
                     ") d where   b.StartAge=d.StartAge and b.EndAge=d.EndAge and " +
             " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + strRace + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100 or b.EndPointID=102)" + " and b.Prevalence='" + strbPrevalence + "' " +
             "  and c.IncidenceDatasetID={0} and b.StartAge<={2} and b.EndAge>={1} group by a.CColumn,a.Row ,d.AgeRangeID", iid, crSelectFunction.StartAge, crSelectFunction.EndAge);
                DataSet dsInc = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, strInc);

                Dictionary<string, double> dicInc = new Dictionary<string, double>();
                foreach (DataRow dr in dsInc.Tables[0].Rows)
                {
                    if (!dicInc.ContainsKey((Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString() + "," + dr["AgeRangeID"]))
                    {
                        dicInc.Add((Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString() + "," + dr["AgeRangeID"].ToString(), Convert.ToDouble(dr["VValue"]));
                    }
                }
                dsInc.Dispose();
                if (iPopulationDataSetGridID == CommonClass.GBenMAPGrid.GridDefinitionID) return dicInc;
                Dictionary<string, Dictionary<string, double>> dicPercentageForAggregationInc = new Dictionary<string, Dictionary<string, double>>();
                try
                {

                    string str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + (CommonClass.GBenMAPGrid.GridDefinitionID == 28 ? 27 : CommonClass.GBenMAPGrid.GridDefinitionID) + " and  targetgriddefinitionid = " + iPopulationDataSetGridID + " ) and normalizationstate in (0,1)";

                    DataSet dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
                    if (dsPercentage.Tables[0].Rows.Count == 0)
                    {
                        creatPercentageToDatabase(iPopulationDataSetGridID, CommonClass.GBenMAPGrid.GridDefinitionID, null);
                        int iPercentageID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, "select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + CommonClass.GBenMAPGrid.GridDefinitionID + " and  targetgriddefinitionid = " + iPopulationDataSetGridID));
                        str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + (CommonClass.GBenMAPGrid.GridDefinitionID == 28 ? 27 : CommonClass.GBenMAPGrid.GridDefinitionID) + " and  targetgriddefinitionid = " + iPopulationDataSetGridID + " ) and normalizationstate in (0,1)";
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
                    foreach (KeyValuePair<string, float> k in dicPopulationAge)
                    {
                        string[] s = k.Key.Split(new char[] { ',' });
                        if (dicPercentageForAggregationInc.ContainsKey(s[0] + "," + s[1]))
                        {
                            foreach (KeyValuePair<string, double> kin in dicPercentageForAggregationInc[s[0] + "," + s[1]])
                            {
                                string[] sin = kin.Key.Split(new char[] { ',' });
                                double dsin = Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1]);
                                if (!dicInc.ContainsKey(dsin + "," + s[2])) continue;
                                if (dicReturn.ContainsKey((Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])).ToString() + "," + s[2]))
                                    dicReturn[(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])).ToString() + "," + s[2]] += dicInc[dsin + "," + s[2]] * kin.Value;
                                else
                                    dicReturn.Add((Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])).ToString() + "," + s[2], dicInc[dsin + "," + s[2]] * kin.Value);
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

        public static List<IncidenceRateAttribute> getIncidenceDataSetFromCRSelectFuntion(CRSelectFunction crSelectFunction, bool bPrevalence, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, int GridDefinitionID, GridRelationship gridRelationShipPopulation)
        {
            try
            {

                List<IncidenceRateAttribute> lstIncidenceRateAttribute = new List<IncidenceRateAttribute>();
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet dsIncidence = null;
                DataSet dsPrevalence = null;
                string strbPrevalence = "F";
                int iid = crSelectFunction.IncidenceDataSetID;
                if (bPrevalence)
                {
                    strbPrevalence = "T";
                    iid = crSelectFunction.PrevalenceDataSetID;
                }

                string commandText = string.Format("select distinct a.IncidenceRateID,a.CColumn,a.Row,a.VValue,b.StartAge,b.EndAge,b.RaceID,b.EthnicityID,b.GenderID from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c where" +
      " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100) and b.Prevalence='" + strbPrevalence + "' " +
      " and c.IncidenceDatasetID={0} ", iid); if (crSelectFunction.StartAge != -1)
                {
                    commandText = string.Format(commandText + " and b.EndAge>={0} ", crSelectFunction.StartAge);
                }
                if (crSelectFunction.EndAge != -1)
                {
                    commandText = string.Format(commandText + " and b.StartAge<={0} ", crSelectFunction.EndAge);
                }
                if (!string.IsNullOrEmpty(crSelectFunction.Race))
                {
                    if (dicRace[crSelectFunction.Race] != null)
                    {
                        commandText = string.Format(commandText + " and (b.RaceID={0} or b.RaceID=6)", dicRace[crSelectFunction.Race]);
                    }
                }
                if (!string.IsNullOrEmpty(crSelectFunction.Ethnicity))
                {
                    if (dicEthnicity[crSelectFunction.Ethnicity] != null)
                    {
                        commandText = string.Format(commandText + " and (b.EthnicityID={0} or b.EthnicityID=4)", dicEthnicity[crSelectFunction.Ethnicity]);

                    }
                }
                if (!string.IsNullOrEmpty(crSelectFunction.Gender))
                {
                    if (dicGender[crSelectFunction.Gender] != null)
                    {
                        commandText = string.Format(commandText + " and (b.GenderID={0} or b.GenderID=4)", dicGender[crSelectFunction.Gender]);
                    }
                }
                commandText = "select a.CColumn,a.Row,sum(a.VValue) as VValue from ( " + commandText + " ) a group by a.CColumn,a.Row";
                dsIncidence = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in dsIncidence.Tables[0].Rows)
                {
                    lstIncidenceRateAttribute.Add(new IncidenceRateAttribute()
                    {
                        Col = Convert.ToInt32(dr["CColumn"]),
                        Row = Convert.ToInt32(dr["Row"]),
                        Value = Convert.ToSingle(dr["VValue"])
                    });

                }

                if (!bPrevalence)
                    commandText = string.Format("select GriddefinitionID from IncidenceDatasets where IncidenceDatasetID={0}", crSelectFunction.IncidenceDataSetID);
                else
                    commandText = string.Format("select GriddefinitionID from IncidenceDatasets where IncidenceDatasetID={0}", crSelectFunction.PrevalenceDataSetID);
                int incidenceDataSetGridType = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                List<IncidenceRateAttribute> lstResult = new List<IncidenceRateAttribute>();
                float IncidenceValue = 0;
                if (incidenceDataSetGridType == GridDefinitionID)
                {
                    lstResult = lstIncidenceRateAttribute;
                }
                else
                {
                    if (incidenceDataSetGridType == gridRelationShipPopulation.bigGridID)
                    {
                        foreach (GridRelationshipAttribute gra in gridRelationShipPopulation.lstGridRelationshipAttribute)
                        {
                            var queryPopulation = from a in lstIncidenceRateAttribute where gra.bigGridRowCol.Col == a.Col && gra.bigGridRowCol.Row == a.Row select new { Values = lstIncidenceRateAttribute.Average(c => c.Value) };

                            if (queryPopulation != null && queryPopulation.Count() > 0 && gra.smallGridRowCol.Count > 0)
                            {
                                IncidenceValue = queryPopulation.First().Values;
                                foreach (RowCol rc in gra.smallGridRowCol)
                                {
                                    lstResult.Add(new IncidenceRateAttribute()
                                    {
                                        Col = rc.Col,
                                        Row = rc.Row,
                                        Value = IncidenceValue
                                    });
                                }
                            }

                        }
                    }
                    else
                    {
                        foreach (GridRelationshipAttribute gra in gridRelationShipPopulation.lstGridRelationshipAttribute)
                        {
                            var queryPopulation = from a in lstIncidenceRateAttribute where gra.smallGridRowCol.Contains(new RowCol() { Row = a.Row, Col = a.Col }, new RowColComparer()) select new { Values = lstIncidenceRateAttribute.Average(c => c.Value) };

                            if (queryPopulation != null && queryPopulation.Count() > 0)
                            {
                                IncidenceValue = queryPopulation.First().Values;
                                lstResult.Add(new IncidenceRateAttribute()
                                {
                                    Col = gra.bigGridRowCol.Col,
                                    Row = gra.bigGridRowCol.Row,
                                    Value = IncidenceValue
                                });
                            }


                        }
                    }

                }
                return lstResult;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<IncidenceRateAttribute> getIncidenceDataSetFromCRSelectFuntion(CRSelectFunction crSelectFunction, bool bPrevalence, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender)
        {
            try
            {
                List<IncidenceRateAttribute> lstIncidenceRateAttribute = new List<IncidenceRateAttribute>();
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet dsIncidence = null;
                DataSet dsPrevalence = null;
                string strbPrevalence = "F";
                if (bPrevalence) strbPrevalence = "T";
                string commandText = string.Format("select distinct a.IncidenceRateID,a.CColumn,a.Row,a.VValue,b.StartAge,b.EndAge,b.RaceID,b.EthnicityID,b.GenderID from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c where" +
      " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100) and b.Prevalence='" + strbPrevalence + "' " +
      " and c.IncidenceDatasetID={0} ", crSelectFunction.IncidenceDataSetID); if (crSelectFunction.StartAge != -1)
                {
                    commandText = string.Format(commandText + " and b.EndAge>={0} ", crSelectFunction.StartAge);
                }
                if (crSelectFunction.EndAge != -1)
                {
                    commandText = string.Format(commandText + " and b.StartAge<={0} ", crSelectFunction.EndAge);
                }
                if (!string.IsNullOrEmpty(crSelectFunction.Race))
                {
                    if (dicRace[crSelectFunction.Race] != null)
                    {
                        commandText = string.Format(commandText + " and (b.RaceID={0} or b.RaceID=6)", dicRace[crSelectFunction.Race]);
                    }
                }
                if (!string.IsNullOrEmpty(crSelectFunction.Ethnicity))
                {
                    if (dicEthnicity[crSelectFunction.Ethnicity] != null)
                    {
                        commandText = string.Format(commandText + " and (b.EthnicityID={0} or b.EthnicityID=4)", dicEthnicity[crSelectFunction.Ethnicity]);

                    }
                }
                if (!string.IsNullOrEmpty(crSelectFunction.Gender))
                {
                    if (dicGender[crSelectFunction.Gender] != null)
                    {
                        commandText = string.Format(commandText + " and (b.GenderID={0} or b.GenderID=4)", dicGender[crSelectFunction.Gender]);
                    }
                }
                commandText = "select a.CColumn,a.Row,sum(a.VValue) as VValue from ( " + commandText + " ) a group by a.CColumn,a.Row";
                dsIncidence = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in dsIncidence.Tables[0].Rows)
                {
                    lstIncidenceRateAttribute.Add(new IncidenceRateAttribute()
                    {
                        Col = Convert.ToInt32(dr["CColumn"]),
                        Row = Convert.ToInt32(dr["Row"]),
                        Value = Convert.ToSingle(dr["VValue"])
                    });

                }
                return lstIncidenceRateAttribute;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static List<RegionTypeGrid> InitRegionTypeGrid(BenMAPGrid benMAPGrid)
        {
            return null;
        }
        public static List<string> getAllSystemVariableNameList()
        {
            try
            {
                List<string> lstResult = new List<string>();
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = "select distinct SetupVariableName from SetupVariables where setupvariabledatasetid in(select setupvariabledatasetid from setupvariabledatasets where setupid = " + CommonClass.MainSetup.SetupID + ")";
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    lstResult.Add(dr[0].ToString());

                }
                return lstResult;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public static BenMAPPopulation getBenMapPopulationFromDataSetIDAndYear(int DataSetID, int Year)
        {
            try
            {
                BenMAPPopulation benMAPPopulation = new BenMAPPopulation() { DataSetID = DataSetID, Year = Year };
                string commandText = string.Format("select PopulationDatasetID,SetupID,PopulationDatasetName,PopulationConfigurationID,GridDefinitionID from   PopulationDatasets where PopulationDatasetID={0}", DataSetID);



                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                DataRow dr = ds.Tables[0].Rows[0];
                benMAPPopulation.GridType = Grid.GridCommon.getBenMAPGridFromID(Convert.ToInt32(dr["GridDefinitionID"]));
                benMAPPopulation.PopulationConfiguration = Convert.ToInt32(dr["PopulationConfigurationID"]);
                benMAPPopulation.DataSetName = dr["PopulationDatasetName"].ToString();


                ds.Dispose();
                return benMAPPopulation;
            }
            catch (Exception ex)
            {
                return null;
            }
        }






























        private static List<string> lstSystemVariableName;
        public static List<string> LstSystemVariableName
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
        public static Dictionary<string, float> getMetricValueFromDic(List<MetricValueAttributes> lstMetricValueAttributes)
        {
            try
            {
                Dictionary<string, float> dicReturn = new Dictionary<string, float>();
                foreach (MetricValueAttributes mv in lstMetricValueAttributes)
                {
                    dicReturn.Add(mv.MetricName, mv.MetricValue);
                }
                return dicReturn;
            }
            catch
            {
                return new Dictionary<string, float>();
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
                    foreach (ModelAttribute m in benMapLine.ModelAttributes)
                    {
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
                                lstTemp = m.Values.Where(p => p != float.MinValue).ToList();
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
                            lstTemp = m.Values.Where(p => p != float.NaN && p != float.MinValue).ToList();
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
        public static void CalculateOneCRSelectFunction(string sCRID, List<string> lstAllAgeID, Dictionary<string, double> dicAge,
                                        Dictionary<int, Dictionary<string, Dictionary<string, float>>> dicAllMetricDataBase,
                                        Dictionary<int, Dictionary<string, Dictionary<string, float>>> dicAllMetricDataControl,
                                        Dictionary<int, Dictionary<string, Dictionary<string, List<float>>>> dicAll365Base,
                                        Dictionary<int, Dictionary<string, Dictionary<string, List<float>>>> dicAll365Control,
                                        Dictionary<int, Dictionary<string, ModelResultAttribute>> DicControlAll,
                                        Dictionary<string, Dictionary<string, double>> DicAllSetupVariableValues, Dictionary<string, float> dicPopulationAllAge, Dictionary<string, double> dicIncidenceRateAttribute,
                                        Dictionary<string, double> dicPrevalenceRateAttribute, int incidenceDataSetGridType, int PrevalenceDataSetGridType,
                                        Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, double Threshold, int LatinHypercubePoints, bool RunInPointMode,
                                        List<GridRelationship> lstGridRelationship, CRSelectFunction crSelectFunction, List<RegionTypeGrid> lstRegionTypeGrid, BenMAPPopulation benMAPPopulation)
        {
            try
            {
                try
                {
                    if (benMAPPopulation.GridType.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
                        lstGridRelationship.Where(p => (p.bigGridID == benMAPPopulation.GridType.GridDefinitionID && p.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID) || (p.smallGridID == benMAPPopulation.GridType.GridDefinitionID && p.bigGridID == CommonClass.GBenMAPGrid.GridDefinitionID)).First();
                }
                catch
                {
                }


                //dictionaries to hold values for a single pollutant
                Dictionary<string, Dictionary<string, float>> dicBaseMetricData;
                Dictionary<string, Dictionary<string, float>> dicControlMetricData;
                Dictionary<string, Dictionary<string, List<float>>> dicBase365;
                Dictionary<string, Dictionary<string, List<float>>> dicControl365;
                Dictionary<string, ModelResultAttribute> dicControl;
                BaseControlGroup baseControlGroup;

                //to get dictionaries for a single pollutant, for example
                baseControlGroup = CommonClass.LstBaseControlGroup.First();
                dicBaseMetricData = dicAllMetricDataBase[baseControlGroup.Pollutant.PollutantID];
                dicControlMetricData = dicAllMetricDataControl[baseControlGroup.Pollutant.PollutantID];
                dicBase365 = dicAll365Base[baseControlGroup.Pollutant.PollutantID];
                dicControl365 = dicAll365Control[baseControlGroup.Pollutant.PollutantID];
                dicControl = DicControlAll[baseControlGroup.Pollutant.PollutantID];

                double baseValue = 0;
                double controlValue = 0;
                double deltaQValue = 0;

                Dictionary<int, double> dicBaseValues = new Dictionary<int, double>();
                Dictionary<int, double> dicControlValues = new Dictionary<int, double>();
                Dictionary<int, List<float>> dicBase365Values = new Dictionary<int, List<float>>();
                Dictionary<int, List<float>> dicControl365Values = new Dictionary<int, List<float>>();
                Dictionary<int, double> dicDeltaQValues = new Dictionary<int, double>();


                double populationValue = 0;
                double incidenceValue = 0;
                double prevalenceValue = 0;

                Dictionary<string, double> dicPopValue = new Dictionary<string, double>();
                Dictionary<string, double> dicIncidenceValue = new Dictionary<string, double>();
                Dictionary<string, double> dicPrevalenceValue = new Dictionary<string, double>();


                float i365 = 1;
                int iStartDay = 365, iEndDay = 0;
                //set startday, endday vars if no seasonal metric exists and metric statistic (i.e., annual statistic) is set to none for health impact function
                //this startday, endday will serve as global bounds for the pollutant(s) data
                if (crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric == null && crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic == MetricStatic.None)
                {
                    i365 = 365;
                    //get metric/seasonal metrics for pollutant metric that matches the metric specified in the health impact function
                    //we assume that all pollutants have the same metrics, so we can just get the first basecontrol group
                    baseControlGroup = CommonClass.LstBaseControlGroup.First();

                    //List<SeasonalMetric> lstseasonalMetric = baseControlGroup.Pollutant.SesonalMetrics.Where(p => p.Metric.MetricID == crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricID).ToList();

                    //in a multipollutant scenario, we have to match on metric name instead of id since metrics are tied to pollutants (old code which matched on ID is above)
                    //so here we are looking for pollutant seasonal metrics for metrics that have the same name as the seasonal metric for the health impact function
                    List<SeasonalMetric> lstseasonalMetric = baseControlGroup.Pollutant.SesonalMetrics.Where(p => String.Equals(p.Metric.MetricName, crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName, StringComparison.OrdinalIgnoreCase)).ToList();

                    SeasonalMetric seasonalMetric = null;
                    //if we have seasonal metrics for this metric, then take the last one (JCM 2016-01-25, why not the first one?)
                    if (lstseasonalMetric.Count > 0)
                        seasonalMetric = lstseasonalMetric.Last();
                    //if we have a seasonal metric
                    if (seasonalMetric != null && seasonalMetric.Seasons.Count > 0)
                    {
                        i365 = 0;
                        foreach (Season season in seasonalMetric.Seasons)
                        {
                            i365 = i365 + season.EndDay - season.StartDay + 1;
                            if (season.StartDay < iStartDay) iStartDay = season.StartDay;
                            if (season.EndDay > iEndDay) iEndDay = season.EndDay + 1;
                        }
                    }
                    else //if we don't have a seasonal metric, then check to see if pollutant itself has seasons
                    {
                        if (crSelectFunction.BenMAPHealthImpactFunction.PollutantGroup.Pollutants.First().Seasons != null && crSelectFunction.BenMAPHealthImpactFunction.PollutantGroup.Pollutants.First().Seasons.Count != 0)
                        {
                            i365 = 0;
                            foreach (Season season in crSelectFunction.BenMAPHealthImpactFunction.PollutantGroup.Pollutants.First().Seasons)
                            {
                                i365 = i365 + season.EndDay - season.StartDay + 1;
                                if (season.StartDay < iStartDay) iStartDay = season.StartDay;
                                if (season.EndDay > iEndDay) iEndDay = season.EndDay + 1;
                            }

                        }
                    }
                }



                Dictionary<string, double> dicVariable = null;
                double d = 0;
                CRCalculateValue crCalculateValue = new CRCalculateValue();

                //get health impact function strings
                string strBaseLineFunction = ConfigurationCommonClass.getFunctionStringFromDatabaseFunction(crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction);
                bool hasPopInstrBaseLineFunction = crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction.Contains("POP");
                string strPointEstimateFunction = ConfigurationCommonClass.getFunctionStringFromDatabaseFunction(crSelectFunction.BenMAPHealthImpactFunction.Function);

                //set monitor value dictionaries for each pollutant, if we are using monitor data
                Dictionary<int, Dictionary<string, MonitorValue>> dicBaseMonitorAll = new Dictionary<int, Dictionary<string, MonitorValue>>();
                Dictionary<int, Dictionary<string, MonitorValue>> dicControlMonitorAll = new Dictionary<int, Dictionary<string, MonitorValue>>();
                Dictionary<int, Dictionary<string, List<MonitorNeighborAttribute>>> dicAllMonitorNeighborControlAll = new Dictionary<int, Dictionary<string, List<MonitorNeighborAttribute>>>();
                Dictionary<int, Dictionary<string, List<MonitorNeighborAttribute>>> dicAllMonitorNeighborBaseAll = new Dictionary<int, Dictionary<string, List<MonitorNeighborAttribute>>>();

                //initialize dictionaries for monitor values
                Dictionary<string, MonitorValue> dicBaseMonitor = new Dictionary<string, MonitorValue>();
                Dictionary<string, MonitorValue> dicControlMonitor = new Dictionary<string, MonitorValue>();
                Dictionary<string, List<MonitorNeighborAttribute>> dicAllMonitorNeighborControl = new Dictionary<string, List<MonitorNeighborAttribute>>();
                Dictionary<string, List<MonitorNeighborAttribute>> dicAllMonitorNeighborBase = new Dictionary<string, List<MonitorNeighborAttribute>>();

                foreach (BaseControlGroup bcg in CommonClass.LstBaseControlGroup)
                {
                    //initialize dictionaries for monitor values
                    dicBaseMonitor = new Dictionary<string, MonitorValue>();
                    dicControlMonitor = new Dictionary<string, MonitorValue>();
                    dicAllMonitorNeighborControl = new Dictionary<string, List<MonitorNeighborAttribute>>();
                    dicAllMonitorNeighborBase = new Dictionary<string, List<MonitorNeighborAttribute>>();


                    if (bcg.Base is MonitorDataLine && bcg.Control is MonitorDataLine && crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic == MetricStatic.None)
                    {
                        if ((bcg.Base as MonitorDataLine).MonitorValues != null)
                        {
                            foreach (MonitorValue m in (bcg.Base as MonitorDataLine).MonitorValues)
                            {
                                dicBaseMonitor.Add(m.MonitorName, m);
                            }
                        }


                        if ((bcg.Control as MonitorDataLine).MonitorValues != null)
                        {
                            foreach (MonitorValue m in (bcg.Control as MonitorDataLine).MonitorValues)
                            {
                                dicControlMonitor.Add(m.MonitorName, m);
                            }
                        }


                        if ((bcg.Base as MonitorDataLine).MonitorNeighbors != null)
                        {
                            foreach (MonitorNeighborAttribute m in (bcg.Base as MonitorDataLine).MonitorNeighbors)
                            {
                                if (!dicAllMonitorNeighborBase.ContainsKey(m.Col + "," + m.Row))
                                    dicAllMonitorNeighborBase.Add(m.Col + "," + m.Row, new List<MonitorNeighborAttribute>() { m });
                                else
                                    dicAllMonitorNeighborBase[m.Col + "," + m.Row].Add(m);
                            }
                        }


                        if ((bcg.Control as MonitorDataLine).MonitorNeighbors != null)
                        {
                            foreach (MonitorNeighborAttribute m in (bcg.Control as MonitorDataLine).MonitorNeighbors)
                            {
                                if (!dicAllMonitorNeighborControl.ContainsKey(m.Col + "," + m.Row))
                                    dicAllMonitorNeighborControl.Add(m.Col + "," + m.Row, new List<MonitorNeighborAttribute>() { m });
                                else
                                    dicAllMonitorNeighborControl[m.Col + "," + m.Row].Add(m);
                            }
                        }
                    }

                    dicBaseMonitorAll.Add(bcg.Pollutant.PollutantID, dicBaseMonitor);
                    dicControlMonitorAll.Add(bcg.Pollutant.PollutantID, dicControlMonitor);
                    dicAllMonitorNeighborControlAll.Add(bcg.Pollutant.PollutantID, dicAllMonitorNeighborControl);
                    dicAllMonitorNeighborBaseAll.Add(bcg.Pollutant.PollutantID, dicAllMonitorNeighborBase);

                }

                CRSelectFunctionCalculateValue crSelectFunctionCalculateValue = new CRSelectFunctionCalculateValue() { CRSelectFunction = crSelectFunction, CRCalculateValues = new List<CRCalculateValue>() };

                // get number of beta variations (use first variable)
                List<CRFBeta> lstBetas = crSelectFunction.BenMAPHealthImpactFunction.Variables.First().PollBetas.OrderBy(beta => Convert.ToInt32(beta.StartDate)).ToList();

                #region foreach (ModelResultAttribute modelResultAttribute in baseControlGroup.Base.ModelResultAttributes)
                foreach (ModelResultAttribute modelResultAttribute in baseControlGroup.Base.ModelResultAttributes)
                {
                    //clear base, control, and deltaq values for this grid cell
                    dicBaseValues.Clear();
                    dicControlValues.Clear();
                    dicBase365Values.Clear();
                    dicControl365Values.Clear();
                    dicDeltaQValues.Clear();                   

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
                    dicIncidenceValue = null; dicPrevalenceValue = null; dicPopValue = null; dicIncidenceValue = new Dictionary<string, double>();
                    dicPrevalenceValue = new Dictionary<string, double>();
                    dicPopValue = new Dictionary<string, double>();
                    if (dicIncidenceRateAttribute != null)
                    {
                        foreach (string s in lstAllAgeID)
                        {
                            if (dicIncidenceRateAttribute.Keys.Contains((Convert.ToInt32(modelResultAttribute.Col) * 10000 + Convert.ToInt32(modelResultAttribute.Row)).ToString() + "," + s))
                            {
                                dicIncidenceValue.Add(s, dicIncidenceRateAttribute[(Convert.ToInt32(modelResultAttribute.Col) * 10000 + Convert.ToInt32(modelResultAttribute.Row)).ToString() + "," + s]);
                            }
                        }
                    }
                    if (dicPrevalenceRateAttribute != null)
                    {
                        foreach (string s in lstAllAgeID)
                        {
                            if (dicPrevalenceRateAttribute.Keys.Contains((Convert.ToInt32(modelResultAttribute.Col) * 10000 + Convert.ToInt32(modelResultAttribute.Row)).ToString() + "," + s))
                            {
                                dicPrevalenceValue.Add(s, dicPrevalenceRateAttribute[(Convert.ToInt32(modelResultAttribute.Col) * 10000 + Convert.ToInt32(modelResultAttribute.Row)).ToString() + "," + s]);
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

                    //build colrow key
                    string colRowKey = modelResultAttribute.Col + "," + modelResultAttribute.Row;

                    //build metric key
                    string metricKey = String.Empty;
                    //use seasonal metric name or metric name ?
                    if (crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
                    {
                        metricKey = crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName;
                    }
                    else
                    {
                        metricKey = crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName;
                    }
                    //add metric statistic name ?
                    if (crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic != MetricStatic.None)
                    {
                        metricKey = metricKey + "," + Enum.GetName(typeof(MetricStatic), crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic);
                    }

                    //do we have a metric statistic?               
                    if (crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic != MetricStatic.None)
                    {
                        #region if we have a metric statistic in health impact function
                        //get metric data for base and control values for all pollutants
                        //if we don't have metric data, create "blank" result and continue to next model result attribute (i.e. grid cell)
                        if ((!getAllMetricData(dicAllMetricDataBase, colRowKey, metricKey, dicBaseValues)) ||
                                (!getAllMetricData(dicAllMetricDataControl, colRowKey, metricKey, dicControlValues)))
                        {
                            //for each beta variation
                            for (int betaIndex = 0; betaIndex < lstBetas.Count; betaIndex++)
                            {
                                //add a result of 0 "zero"
                                crCalculateValue = new CRCalculateValue()
                                {
                                    Baseline = 0,
                                    Col = modelResultAttribute.Col,
                                    Row = modelResultAttribute.Row,
                                    Deltas = getDeltaQValuesZeros(),
                                    Incidence = Convert.ToSingle(incidenceValue),
                                    Population = Convert.ToSingle(populationValue),
                                    LstPercentile = new List<float>(),
                                    Mean = 0,
                                    PercentOfBaseline = 0,
                                    PointEstimate = 0,
                                    StandardDeviation = 0,
                                    Variance = 0

                                };

                                //add 0 percentile for each latin hypercube point (number latin hypercube points = number percentiles = number items in lhsResultArray)
                                for (int i = 0; i < CommonClass.CRLatinHypercubePoints; i++)
                                {
                                    crCalculateValue.LstPercentile.Add(0);
                                }
                                
                                //set beta variation fields
                                crCalculateValue.BetaVariationName = crSelectFunction.BenMAPHealthImpactFunction.BetaVariation.BetaVariationName;
                                crCalculateValue.BetaName = lstBetas[betaIndex].SeasonName;

                                crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);
                            }

                            //skip to next modelResultAttribute (i.e. grid cell)
                            continue;
                        }
                        #endregion
                    }
                    else
                    {

                        #region if we do not have a metric statistic in health impact function
                        //do we have 365 data?
                       if (getAll365Data(dicAll365Base, colRowKey, metricKey, dicBase365Values) &&
                                getAll365Data(dicAll365Control, colRowKey, metricKey, dicControl365Values))                       
                       {
                            #region if we have 365 data

                            //for each beta variation
                            for (int betaIndex = 0; betaIndex < lstBetas.Count; betaIndex++)
                            {

                                float fPSum = 0, fBaselineSum = 0;
                                List<float> lstFPSum = new List<float>();
                                //initialize percentile list
                                if (!CommonClass.CRRunInPointMode)
                                {
                                    for (int i = 0; i < CommonClass.CRLatinHypercubePoints; i++)
                                    {
                                        lstFPSum.Add(0);
                                    }
                                }

                                //loop over each day of the year for this row/col and metric                                
                                for (int iDay = 0; iDay < dicBase365Values.First().Value.Count; iDay++)
                                {

                                    Dictionary<int, double> fdicBaseValues = new Dictionary<int, double>();
                                    Dictionary<int, double> fdicControlValues = new Dictionary<int, double>();                                    
                                    Dictionary<int, double> fdicDeltaQValues = new Dictionary<int, double>();

                                    //double fBase, fControl, fDelta;
                                    fdicBaseValues = getValuesFrom365Values(dicBase365Values, iDay);
                                    fdicControlValues = getValuesFrom365Values(dicControl365Values, iDay);
                                    if ((!CheckValuesAgainstMinimum(fdicBaseValues)) && (!CheckValuesAgainstMinimum(fdicControlValues)))
                                    {
                                        CheckValuesAgainstThreshold(fdicBaseValues, Threshold);
                                        CheckValuesAgainstThreshold(fdicControlValues, Threshold);

                                        //get deltaQ values
                                        fdicDeltaQValues = getDeltaQValues(fdicBaseValues, fdicControlValues);

                                        {
                                            CRCalculateValue cr = CalculateCRSelectFunctionsOneCel(sCRID, hasPopInstrBaseLineFunction, 1, crSelectFunction, strBaseLineFunction, strPointEstimateFunction, modelResultAttribute.Col, modelResultAttribute.Row, fdicBaseValues, fdicControlValues, dicPopValue, dicIncidenceValue, dicPrevalenceValue, dicVariable, betaIndex);
                                            fPSum += cr.PointEstimate;
                                            fBaselineSum += cr.Baseline;
                                            if (!CommonClass.CRRunInPointMode)
                                            {
                                                for (int i = 0; i < CommonClass.CRLatinHypercubePoints; i++)
                                                {
                                                    lstFPSum[i] += cr.LstPercentile[i];
                                                }
                                            }
                                        }
                                    }

                                }

                                //build result value object
                                crCalculateValue = new CRCalculateValue()
                                {
                                    Col = modelResultAttribute.Col,
                                    Row = modelResultAttribute.Row,
                                    Deltas = getDeltaQValuesZeros(),
                                    Incidence = Convert.ToSingle(incidenceValue),
                                    PointEstimate = fPSum,
                                    LstPercentile = lstFPSum,
                                    Population = Convert.ToSingle(populationValue),
                                    Mean = lstFPSum.Count() == 0 ? float.NaN : getMean(lstFPSum),
                                    Variance = lstFPSum.Count() == 0 ? float.NaN : getVariance(lstFPSum, fPSum),
                                    Baseline = fBaselineSum,
                                };

                                crCalculateValue.StandardDeviation = lstFPSum.Count() == 0 ? float.NaN : Convert.ToSingle(Math.Sqrt(crCalculateValue.Variance));
                                crCalculateValue.PercentOfBaseline = crCalculateValue.Baseline == 0 ? 0 : Convert.ToSingle(Math.Round((crCalculateValue.Mean / crCalculateValue.Baseline) * 100, 4));

                                //calculate delta
                                Dictionary<int, double> baseValuesForDelta = getBaseValuesFromModelResultAttributes(colRowKey, metricKey);
                                Dictionary<int, double> controlValuesForDelta = new Dictionary<int, double>(baseValuesForDelta);
                                
                                //if (dicControl.Keys.Contains(colRowKey))
                                //{
                                //    if (dicControl[colRowKey].Values.Keys.Contains(metricKey))
                                //        controlValueForDelta = dicControl[colRowKey].Values[metricKey];
                                //}

                                CheckValuesAgainstThreshold(baseValuesForDelta, Threshold);
                                CheckValuesAgainstThreshold(controlValuesForDelta, Threshold);

                                //set delta
                                crCalculateValue.Deltas = getDeltaQValues(baseValuesForDelta, controlValuesForDelta);

                                //set beta variation fields
                                crCalculateValue.BetaVariationName = crSelectFunction.BenMAPHealthImpactFunction.BetaVariation.BetaVariationName;
                                crCalculateValue.BetaName = lstBetas[betaIndex].SeasonName;

                                crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);

                            }

                            //skip to next modelresultattribute (i.e. grid cell)
                            continue;

                            #endregion

                        }
                        else
                        {
                            #region if we do not have 365 data
                            if (modelResultAttribute.Values.ContainsKey(metricKey))
                                baseValue = modelResultAttribute.Values[metricKey];

                            controlValue = baseValue;

                            if (dicControl.Keys.Contains(colRowKey))
                            {

                                if (dicControl[colRowKey].Values.Keys.Contains(metricKey))
                                    controlValue = dicControl[colRowKey].Values[metricKey];

                            }

                            //are we using monitor data?
                            if (baseControlGroup.Base is MonitorDataLine && baseControlGroup.Control is MonitorDataLine && baseValue != controlValue && dicAllMonitorNeighborBase != null
                                && dicAllMonitorNeighborBase.ContainsKey(colRowKey)
                                && dicAllMonitorNeighborControl != null && dicAllMonitorNeighborBase.ContainsKey(colRowKey))
                            {
                                #region if we are using monitor data
                                bool is365 = false;
                                int dayCount = 365;

                                //adjust day counts if using seasonal metric
                                if (crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
                                {
                                    i365 = crSelectFunction.BenMAPHealthImpactFunction.PollutantGroup.Pollutants.First().Seasons.Count();
                                    iStartDay = 0;
                                    iEndDay = crSelectFunction.BenMAPHealthImpactFunction.PollutantGroup.Pollutants.First().Seasons.Count();
                                    dayCount = 4;
                                }

                                foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborBase[colRowKey])
                                {
                                    if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(metricKey))
                                    {
                                        is365 = true;
                                        dayCount = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[metricKey].Count;
                                        break;
                                    }
                                }

                                if (!is365)
                                {
                                    foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborControl[colRowKey])
                                    {
                                        if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(metricKey))
                                        {
                                            is365 = true;
                                            dayCount = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[metricKey].Count;
                                            break;
                                        }
                                    }
                                }

                                //for each beta variation
                                for (int betaIndex = 0; betaIndex < lstBetas.Count; betaIndex++)
                                {

                                    //is 365?                                
                                    if (is365)
                                    {
                                        #region if is365 = true
                                        List<float> lstdfmBase = new List<float>();

                                        foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborBase[colRowKey])
                                        {
                                            if (lstdfmBase.Count == 0)
                                            {
                                                if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(metricKey))
                                                {
                                                    lstdfmBase = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[metricKey].Select(p => p == float.MinValue ? 0 : Convert.ToSingle(p * mnAttribute.Weight)).ToList();
                                                }
                                                else if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
                                                {
                                                    float value = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[metricKey] == float.MinValue ? 0 : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[metricKey] * Convert.ToSingle(mnAttribute.Weight);
                                                    for (int i = 0; i < dayCount; i++)
                                                    {
                                                        lstdfmBase.Add(value);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(metricKey))
                                                {
                                                    for (int idfm = 0; idfm < lstdfmBase.Count; idfm++)
                                                    {
                                                        lstdfmBase[idfm] += dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[metricKey][idfm] == float.MinValue ? 0 : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[metricKey][idfm] * Convert.ToSingle(mnAttribute.Weight);
                                                    }
                                                }
                                                else if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
                                                {
                                                    float value = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[metricKey] == float.MinValue ? 0 : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[metricKey] * Convert.ToSingle(mnAttribute.Weight);
                                                    for (int idfm = 0; idfm < lstdfmBase.Count; idfm++)
                                                    {
                                                        lstdfmBase[idfm] += value;
                                                    }
                                                }
                                            }
                                        }

                                        List<float> lstdfmControl = new List<float>();


                                        foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborControl[colRowKey])
                                        {
                                            if (lstdfmControl.Count == 0)
                                            {
                                                if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(metricKey))
                                                {
                                                    lstdfmControl = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[metricKey].Select(p => p == float.MinValue ? 0 : Convert.ToSingle(p * mnAttribute.Weight)).ToList();

                                                }
                                                else if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
                                                {
                                                    float value = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[metricKey] == float.MinValue ? 0 : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[metricKey] * Convert.ToSingle(mnAttribute.Weight);
                                                    for (int i = 0; i < dayCount; i++)
                                                    {
                                                        lstdfmControl.Add(value);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(metricKey))
                                                {
                                                    for (int idfm = 0; idfm < lstdfmControl.Count; idfm++)
                                                    {
                                                        lstdfmControl[idfm] += dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[metricKey][idfm] == float.MinValue ? 0 : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[metricKey][idfm] * Convert.ToSingle(mnAttribute.Weight);

                                                    }
                                                }
                                                else if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
                                                {
                                                    float value = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[metricKey] == float.MinValue ? 0 : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[metricKey] * Convert.ToSingle(mnAttribute.Weight);
                                                    for (int idfm = 0; idfm < lstdfmBase.Count; idfm++)
                                                    {
                                                        lstdfmControl[idfm] += value;
                                                    }
                                                }
                                            }
                                        }

                                        float fPSum = 0, fBaselineSum = 0;
                                        List<float> lstFPSum = new List<float>();
                                        if (!CommonClass.CRRunInPointMode)
                                        {
                                            for (int i = 0; i < CommonClass.CRLatinHypercubePoints; i++)
                                            {
                                                lstFPSum.Add(0);
                                            }
                                        }
                                        if (lstdfmBase.Count > 0 && lstdfmControl.Count > 0)
                                        {
                                            for (int iBase = iStartDay; iBase < iEndDay; iBase++)
                                            {
                                                double fBase, fControl, fDelta;
                                                fBase = lstdfmBase[iBase];
                                                fControl = lstdfmControl[iBase];
                                                if (fBase != 0 && fControl != 0)
                                                {
                                                    if (Threshold != 0 && fBase < Threshold)
                                                        fBase = Threshold;
                                                    if (fControl != 0 && fControl < Threshold)
                                                        fControl = Threshold;
                                                    fDelta = fBase - fControl;

                                                    //if no seasonal metric, i.e. we are using metric name, and delta = 0 then skip to next day
                                                    if (crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric == null)
                                                    {
                                                        if (fDelta == 0)
                                                        {
                                                            continue;
                                                        }
                                                    }

                                                    {
                                                        CRCalculateValue cr = new CRCalculateValue(); //= CalculateCRSelectFunctionsOneCel(sCRID, hasPopInstrBaseLineFunction, 1, crSelectFunction, strBaseLineFunction, strPointEstimateFunction, modelResultAttribute.Col, modelResultAttribute.Row, fBase, fControl, dicPopValue, dicIncidenceValue, dicPrevalenceValue, dicVariable, betaIndex);
                                                        fPSum += cr.PointEstimate;
                                                        fBaselineSum += cr.Baseline;
                                                        if (!CommonClass.CRRunInPointMode)
                                                        {
                                                            for (int i = 0; i < CommonClass.CRLatinHypercubePoints; i++)
                                                            {
                                                                lstFPSum[i] += cr.LstPercentile[i];
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
                                        #endregion

                                    }
                                    else
                                    {
                                        #region if is365 = false
                                        double fBase = 0;
                                        foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborBase[colRowKey])
                                        {
                                            if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
                                            {
                                                fBase += dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[metricKey] == float.MinValue ? 0 : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[metricKey] * Convert.ToSingle(mnAttribute.Weight);
                                            }
                                        }

                                        double fControl = 0;
                                        foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborControl[colRowKey])
                                        {
                                            if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
                                            {
                                                fControl += dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[metricKey] == float.MinValue ? 0 : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[metricKey] * Convert.ToSingle(mnAttribute.Weight);
                                            }
                                        }

                                        float fPSum = 0, fBaselineSum = 0;
                                        List<float> lstFPSum = new List<float>();
                                        if (!CommonClass.CRRunInPointMode)
                                        {
                                            for (int i = 0; i < CommonClass.CRLatinHypercubePoints; i++)
                                            {
                                                lstFPSum.Add(0);
                                            }
                                        }
                                        double fDelta;
                                        if (fBase != 0 && fControl != 0)
                                        {
                                            if (Threshold != 0 && fBase < Threshold)
                                                fBase = Threshold;
                                            if (fControl != 0 && fControl < Threshold)
                                                fControl = Threshold;
                                            fDelta = fBase - fControl;
                                            {
                                                CRCalculateValue cr = new CRCalculateValue(); //CalculateCRSelectFunctionsOneCel(sCRID, hasPopInstrBaseLineFunction, 1, crSelectFunction, strBaseLineFunction, strPointEstimateFunction, modelResultAttribute.Col, modelResultAttribute.Row, fBase, fControl, dicPopValue, dicIncidenceValue, dicPrevalenceValue, dicVariable, betaIndex);
                                                fPSum += cr.PointEstimate * i365;
                                                fBaselineSum += cr.Baseline * i365;
                                                if (!CommonClass.CRRunInPointMode)
                                                {
                                                    for (int i = 0; i < CommonClass.CRLatinHypercubePoints; i++)
                                                    {
                                                        lstFPSum[i] += cr.LstPercentile[i];
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

                                        #endregion
                                    }


                                    crCalculateValue.PercentOfBaseline = crCalculateValue.Baseline == 0 ? 0 : Convert.ToSingle(Math.Round((crCalculateValue.Mean / crCalculateValue.Baseline) * 100, 4));

                                    //calculate Delta
                                    double baseValueForDelta = modelResultAttribute.Values[metricKey];
                                    double controlValueForDelta = baseValueForDelta;

                                    if (dicControl.Keys.Contains(colRowKey))
                                    {
                                        if (dicControl[colRowKey].Values.Keys.Contains(metricKey))
                                            controlValueForDelta = dicControl[colRowKey].Values[metricKey];
                                    }

                                    if (Threshold != 0 && baseValueForDelta < Threshold)
                                        baseValueForDelta = Threshold;

                                    if (Threshold != 0 && controlValueForDelta < Threshold)
                                        controlValueForDelta = Threshold;

                                    //set delta
                                    crCalculateValue.Delta = Convert.ToSingle(baseValueForDelta - controlValueForDelta);

                                    //set beta variation fields
                                    crCalculateValue.BetaVariationName = crSelectFunction.BenMAPHealthImpactFunction.BetaVariation.BetaVariationName;
                                    crCalculateValue.BetaName = lstBetas[betaIndex].SeasonName;

                                    //add calculated value to list of calculated values
                                    crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);
                                }

                                //skip to next modelResultAttribute (i.e., grid cell)
                                continue;


                                #endregion
                            }


                            if (crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
                            {
                                baseValue = modelResultAttribute.Values[metricKey];
                                controlValue = baseValue;

                                if (dicControl.Keys.Contains(colRowKey))
                                {
                                    if (dicControl[colRowKey].Values.Keys.Contains(metricKey))
                                        controlValue = dicControl[colRowKey].Values[metricKey];
                                }
                                i365 = crSelectFunction.BenMAPHealthImpactFunction.PollutantGroup.Pollutants.First().Seasons.Count();

                            }

                            #endregion
                        }

                        #endregion
                    }


                    //check base and control values against threshold
                    CheckValuesAgainstThreshold(dicBaseValues, Threshold);
                    CheckValuesAgainstThreshold(dicControlValues, Threshold);

                    //get deltaQ values
                    dicDeltaQValues = getDeltaQValues(dicBaseValues, dicControlValues);

                    //for each beta variation
                    for (int betaIndex = 0; betaIndex < lstBetas.Count; betaIndex++)
                    {
                        //calculate one cell                    
                        crCalculateValue = CalculateCRSelectFunctionsOneCel(sCRID, hasPopInstrBaseLineFunction, i365, crSelectFunction, strBaseLineFunction, strPointEstimateFunction, modelResultAttribute.Col, modelResultAttribute.Row, dicBaseValues, dicControlValues, dicPopValue, dicIncidenceValue, dicPrevalenceValue, dicVariable, betaIndex);

                        //set beta variation fields
                        crCalculateValue.BetaVariationName = crSelectFunction.BenMAPHealthImpactFunction.BetaVariation.BetaVariationName;
                        crCalculateValue.BetaName = lstBetas[betaIndex].SeasonName;

                        //add calculated value to list of calculated values
                        crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);
                    }


                    dicVariable = null;
                }
                #endregion //end foreach (ModelResultAttribute modelResultAttribute in baseControlGroup.Base.ModelResultAttributes)

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

        public static Dictionary<string, double> getDicSetupVariableColRow(int Col, int Row, List<SetupVariableJoinAllValues> lstVariableJoin, int GridDefinitionID, List<GridRelationship> lstGridRelationship)
        {
            try
            {
                Dictionary<string, double> dicResult = new Dictionary<string, double>();
                foreach (SetupVariableJoinAllValues setupVariableJoinAllValue in lstVariableJoin)
                {
                    if (setupVariableJoinAllValue.SetupVariableGridType == GridDefinitionID)
                    {
                        var queryVariable = from a in setupVariableJoinAllValue.lstValues where a.Col == Col && a.Row == Row select a;
                        double values = 0;
                        foreach (SetupVariableValues iRateAttributes in queryVariable)
                        {
                            values += iRateAttributes.Value;

                        }
                        if (queryVariable.Count() > 0) values = values / Convert.ToDouble(queryVariable.Count());
                        dicResult.Add(setupVariableJoinAllValue.SetupVariableName, values);
                    }
                    else
                    {
                        RowCol rowCol = new RowCol() { Col = Col, Row = Row };
                        List<RowCol> lstRowCol;
                        GridRelationship gridRelationShipVariable = new GridRelationship();
                        foreach (GridRelationship gRelationship in lstGridRelationship)
                        {
                            if (gRelationship.bigGridID == setupVariableJoinAllValue.SetupVariableGridType || gRelationship.smallGridID == setupVariableJoinAllValue.SetupVariableGridType)
                            {
                                gridRelationShipVariable = gRelationship;
                            }
                        }
                        if (setupVariableJoinAllValue.SetupVariableGridType == gridRelationShipVariable.bigGridID)
                        {

                            var queryrowCol = from a in gridRelationShipVariable.lstGridRelationshipAttribute where a.smallGridRowCol.Contains(rowCol, new RowColComparer()) select new RowCol() { Col = a.bigGridRowCol.Col, Row = a.bigGridRowCol.Row };
                            lstRowCol = queryrowCol.ToList();
                            var queryVariable = from a in setupVariableJoinAllValue.lstValues where lstRowCol.Contains(new RowCol() { Col = a.Col, Row = a.Row }, new RowColComparer()) select a;
                            double values = 0;
                            foreach (SetupVariableValues iAttributes in queryVariable)
                            {
                                values += iAttributes.Value;

                            }
                            if (queryVariable.Count() > 0) values = values / Convert.ToDouble(queryVariable.Count());
                            dicResult.Add(setupVariableJoinAllValue.SetupVariableName, values);

                        }
                        else
                        {
                            var queryrowCol = from a in gridRelationShipVariable.lstGridRelationshipAttribute where a.bigGridRowCol.Col == rowCol.Col && a.bigGridRowCol.Row == rowCol.Row select a;
                            if (queryrowCol != null)
                            {
                                lstRowCol = queryrowCol.First().smallGridRowCol;
                                List<SetupVariableValues> lstQueryVariable = new List<SetupVariableValues>();
                                foreach (RowCol rc in lstRowCol)
                                {
                                    var queryVariable = from a in setupVariableJoinAllValue.lstValues where a.Col == rc.Col && a.Row == rc.Row select a;
                                    IEnumerable<SetupVariableValues> iqueryIncidence = queryVariable.ToList();
                                    lstQueryVariable.AddRange(iqueryIncidence);

                                }
                                double values = 0;

                                foreach (SetupVariableValues iRateAttributes in lstQueryVariable)
                                {
                                    values += iRateAttributes.Value;

                                }
                                if (lstQueryVariable.Count() > 0) values = values / Convert.ToDouble(lstQueryVariable.Count());
                                dicResult.Add(setupVariableJoinAllValue.SetupVariableName, values);
                            }
                        }
                    }

                }

                return dicResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static CRCalculateValue CalculateCRSelectFunctionsOneCel(string iCRID, bool hasPopInstrBaseLineFunction, float i365, CRSelectFunction crSelectFunction, string strBaseLineFunction, string strPointEstimateFunction, int col, int row, Dictionary<int, double> dicBaseValues, Dictionary<int, double> dicControlValues, Dictionary<string, double> dicPopulationValue, Dictionary<string, double> dicIncidenceValue, Dictionary<string, double> dicPrevalenceValue, Dictionary<string, double> dicSetupVariables, int betaIndex)
        {
            try
            {

                double incidenceValue, prevalenceValue, PopValue;

                Dictionary<int, double> dicDeltaQValues = getDeltaQValues(dicBaseValues, dicControlValues);

                //create dictionary to hold all percentile betas for each pollutant
                Dictionary<int, List<double>> dicPollutantBetaValues = new Dictionary<int, List<double>>();

                List<Dictionary<string, double>> lstPercentileBetas = new List<Dictionary<string, double>>();
                Dictionary<int, double> dicBetaValues = new Dictionary<int, double>();
                Dictionary<string, double> dicBetaValuesVarName = new Dictionary<string, double>();

                //get Betas
                if (!CommonClass.CRRunInPointMode)
                {
                    //get standard deviation
                    double standardDeviation = CalculateCRSelectFunctionsOneCelStandardError(crSelectFunction, dicDeltaQValues, betaIndex);

                    //get random seed
                    int iRandomSeed = Convert.ToInt32(DateTime.Now.Hour + "" + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond);
                    if (CommonClass.CRSeeds != null && CommonClass.CRSeeds != -1)
                        iRandomSeed = Convert.ToInt32(CommonClass.CRSeeds);

                    //get beta distribution for each pollutant
                    foreach (KeyValuePair<int, double> kvp in dicDeltaQValues)
                    {
                        //get pollutant id
                        int pollutantID = kvp.Key;
                        double[] arrBetas = Configuration.ConfigurationCommonClass.getLHSArrayCRFunctionSeed(CommonClass.CRLatinHypercubePoints, crSelectFunction, iRandomSeed, pollutantID, betaIndex, standardDeviation);                        
                        //arrBetas = new double[] { 0.000952070623302594, 0.00210438313847891, 0.00274330081530712, 0.00322388206768484, 0.00362222578583882, 0.00397556823480331, 0.00429312210515043, 0.00459255842643554, 0.00487883373418946, 0.00515806309883299, 0.00543530163423869, 0.00571575251933468, 0.00600086948338013, 0.00630106910383155, 0.00661827327976483, 0.00696570641344888, 0.00736310219575063, 0.00783988157577035, 0.00847429570471552, 0.00963051160274645 };
                        List<double> lstBetas = new List<double>(arrBetas);
                        dicPollutantBetaValues.Add(pollutantID, lstBetas);                       

                    }

                    //now build list of percentiles with dictionaries with containing pollutant betas
                    for (int iPercentile = 0; iPercentile < CommonClass.CRLatinHypercubePoints; iPercentile++)
                    {

                        dicBetaValues = new Dictionary<int, double>();
                        dicBetaValuesVarName = new Dictionary<string, double>();

                        //loop over beta values for each pollutant
                        foreach (KeyValuePair<int, List<double>> kvp in dicPollutantBetaValues)
                        {
                            //add pollutant id and beta value for that pollutant at the specified percentile
                            dicBetaValues.Add(kvp.Key, kvp.Value[iPercentile]);
                        }

                        //convert from pollutant id-based dictionary to variable name-based dictionary
                        dicBetaValuesVarName = getVariableNameDictionaryFromPollutantIDDictionary(dicBetaValues, crSelectFunction.BenMAPHealthImpactFunction);

                        //add to list
                        lstPercentileBetas.Add(dicBetaValuesVarName);
                    }

                }

                //get "baseline" betas for each pollutant;  these are the values specified in the "Beta" fields for each pollutant in the health impact function definition
                dicBetaValues = getBetaValues(crSelectFunction.BenMAPHealthImpactFunction, betaIndex);
                dicBetaValuesVarName = getVariableNameDictionaryFromPollutantIDDictionary(dicBetaValues, crSelectFunction.BenMAPHealthImpactFunction);

                CRCalculateValue crCalculateValue = new CRCalculateValue()
                {
                    Col = col,
                    Row = row,
                    Population = Convert.ToSingle(dicPopulationValue != null ? dicPopulationValue.Sum(p => p.Value) : 0),
                    Incidence = 0,
                    Deltas = dicDeltaQValues
                };

                // set up DeltaList with delta values in order by pollutant name alphabetically for displaying results 
                if (crCalculateValue.DeltaList == null) crCalculateValue.DeltaList = new List<double>();
                crCalculateValue.DeltaList = getSortedDeltaListFromDictionaryandObject(crSelectFunction.BenMAPHealthImpactFunction, dicDeltaQValues);

                //convert pollutant id-based dictionaries to variable name dictionaries
                Dictionary<string, double> dicBaseValuesVarName = getVariableNameDictionaryFromPollutantIDDictionary(dicBaseValues, crSelectFunction.BenMAPHealthImpactFunction);
                Dictionary<string, double> dicControlValuesVarName = getVariableNameDictionaryFromPollutantIDDictionary(dicControlValues, crSelectFunction.BenMAPHealthImpactFunction);
                Dictionary<string, double> dicDeltaQValuesVarName = getVariableNameDictionaryFromPollutantIDDictionary(dicDeltaQValues, crSelectFunction.BenMAPHealthImpactFunction);           


                if (dicPopulationValue == null || dicPopulationValue.Count == 0 || dicPopulationValue.Sum(p => p.Value) == 0)
                    crCalculateValue.PointEstimate = 0;
                else
                {
                    if (strPointEstimateFunction.ToLower().Contains("pop"))
                    {
                        foreach (KeyValuePair<string, double> k in dicPopulationValue)
                        {
                            incidenceValue = dicIncidenceValue != null && dicIncidenceValue.Count > 0 && dicIncidenceValue.ContainsKey(k.Key) ? dicIncidenceValue[k.Key] : 0;
                            prevalenceValue = dicPrevalenceValue != null && dicPrevalenceValue.Count > 0 && dicPrevalenceValue.ContainsKey(k.Key) ? dicPrevalenceValue[k.Key] : 0;
                            crCalculateValue.PointEstimate += ConfigurationCommonClass.getValueFromPointEstimateFunctionString(iCRID, strPointEstimateFunction, crSelectFunction.BenMAPHealthImpactFunction.AContantValue,
                                crSelectFunction.BenMAPHealthImpactFunction.BContantValue, crSelectFunction.BenMAPHealthImpactFunction.CContantValue,
                                dicBetaValuesVarName, dicDeltaQValuesVarName, dicControlValuesVarName, dicBaseValuesVarName, incidenceValue, k.Value, prevalenceValue, dicSetupVariables) * i365;
                        }
                    }
                    else
                    {
                        foreach (KeyValuePair<string, double> k in dicPopulationValue)
                        {
                            incidenceValue = dicIncidenceValue != null && dicIncidenceValue.Count > 0 && dicIncidenceValue.ContainsKey(k.Key) ? dicIncidenceValue[k.Key] : 0;
                            prevalenceValue = dicPrevalenceValue != null && dicPrevalenceValue.Count > 0 && dicPrevalenceValue.ContainsKey(k.Key) ? dicPrevalenceValue[k.Key] : 0;
                            crCalculateValue.PointEstimate = ConfigurationCommonClass.getValueFromPointEstimateFunctionString(iCRID, strPointEstimateFunction, crSelectFunction.BenMAPHealthImpactFunction.AContantValue,
                                crSelectFunction.BenMAPHealthImpactFunction.BContantValue, crSelectFunction.BenMAPHealthImpactFunction.CContantValue,
                                dicBetaValuesVarName, dicDeltaQValuesVarName, dicControlValuesVarName, dicBaseValuesVarName, incidenceValue, k.Value, prevalenceValue, dicSetupVariables) * i365;
                        }
                    }
                }
                if (strBaseLineFunction != " return  ;")
                {
                    if (hasPopInstrBaseLineFunction && crCalculateValue.Population == 0)
                    {
                        crCalculateValue.Baseline = 0;

                    }
                    else
                    {
                        if (strBaseLineFunction.ToLower().Contains("pop"))
                        {
                            foreach (KeyValuePair<string, double> k in dicPopulationValue)
                            {
                                incidenceValue = dicIncidenceValue != null && dicIncidenceValue.Count > 0 && dicIncidenceValue.ContainsKey(k.Key) ? dicIncidenceValue[k.Key] : 0;
                                prevalenceValue = dicPrevalenceValue != null && dicPrevalenceValue.Count > 0 && dicPrevalenceValue.ContainsKey(k.Key) ? dicPrevalenceValue[k.Key] : 0;
                                crCalculateValue.Baseline += ConfigurationCommonClass.getValueFromBaseFunctionString(iCRID, strBaseLineFunction, crSelectFunction.BenMAPHealthImpactFunction.AContantValue,
                                    crSelectFunction.BenMAPHealthImpactFunction.BContantValue, crSelectFunction.BenMAPHealthImpactFunction.CContantValue,
                                    dicBetaValuesVarName, dicDeltaQValuesVarName, dicControlValuesVarName, dicBaseValuesVarName, incidenceValue, k.Value, prevalenceValue, dicSetupVariables) * i365;
                            }
                        }
                        else
                        {
                            foreach (KeyValuePair<string, double> k in dicPopulationValue)
                            {
                                incidenceValue = dicIncidenceValue != null && dicIncidenceValue.Count > 0 && dicIncidenceValue.ContainsKey(k.Key) ? dicIncidenceValue[k.Key] : 0;
                                prevalenceValue = dicPrevalenceValue != null && dicPrevalenceValue.Count > 0 && dicPrevalenceValue.ContainsKey(k.Key) ? dicPrevalenceValue[k.Key] : 0;
                                crCalculateValue.Baseline = ConfigurationCommonClass.getValueFromBaseFunctionString(iCRID, strBaseLineFunction, crSelectFunction.BenMAPHealthImpactFunction.AContantValue,
                                    crSelectFunction.BenMAPHealthImpactFunction.BContantValue, crSelectFunction.BenMAPHealthImpactFunction.CContantValue,
                                    dicBetaValuesVarName, dicDeltaQValuesVarName, dicControlValuesVarName, dicBaseValuesVarName, incidenceValue, k.Value, prevalenceValue, dicSetupVariables) * i365;
                            }
                        }
                    }
                }
                else
                {
                    crCalculateValue.Baseline = crCalculateValue.PointEstimate;
                }


                crCalculateValue.LstPercentile = new List<float>();

                if (lstPercentileBetas.Count > 0)
                {

                    for (int i = 0; i < CommonClass.CRLatinHypercubePoints; i++)
                    {
                        crCalculateValue.LstPercentile.Add(0);
                    }

                    if (crCalculateValue.Population != 0)
                    {
                        for (int iPercentile = 0; iPercentile < CommonClass.CRLatinHypercubePoints; iPercentile++)
                        {
                            //get betas for each pollutant at this percentile
                            Dictionary <string, double> dicBetas = lstPercentileBetas[iPercentile];

                            foreach (KeyValuePair<string, double> k in dicPopulationValue)
                            {
                                incidenceValue = dicIncidenceValue != null && dicIncidenceValue.Count > 0 && dicIncidenceValue.ContainsKey(k.Key) ? dicIncidenceValue[k.Key] : 0;
                                prevalenceValue = dicPrevalenceValue != null && dicPrevalenceValue.Count > 0 && dicPrevalenceValue.ContainsKey(k.Key) ? dicPrevalenceValue[k.Key] : 0;
                                crCalculateValue.LstPercentile[iPercentile] += (ConfigurationCommonClass.getValueFromPointEstimateFunctionString(iCRID, strPointEstimateFunction,
                                    crSelectFunction.BenMAPHealthImpactFunction.AContantValue,
                                crSelectFunction.BenMAPHealthImpactFunction.BContantValue, crSelectFunction.BenMAPHealthImpactFunction.CContantValue,
                                  dicBetas, dicDeltaQValuesVarName, dicControlValuesVarName, dicBaseValuesVarName, incidenceValue, k.Value, prevalenceValue, dicSetupVariables) * i365);
                            }


                        }
                    }
                }

                crCalculateValue.Mean = getMean(crCalculateValue.LstPercentile);
                crCalculateValue.Variance = crCalculateValue.LstPercentile.Count() == 0 ? float.NaN : getVariance(crCalculateValue.LstPercentile, crCalculateValue.PointEstimate);
                crCalculateValue.StandardDeviation = crCalculateValue.LstPercentile.Count() == 0 ? float.NaN : Convert.ToSingle(Math.Sqrt(crCalculateValue.Variance));


                if (crCalculateValue.Baseline == 0)
                    crCalculateValue.PercentOfBaseline = 0;
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




        public static void getSetupVariableNameListFromDatabaseFunction(int VariableDatasetID, int GridDefinitionID, string DatabaseFunction, List<string> SystemVariableNameList, ref List<SetupVariableJoinAllValues> lstFunctionVariables)
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

                foreach (string str in SystemVariableNameList)
                {
                    if (DatabaseFunction.ToLower().Contains(str.ToLower()))
                    {
                        bool inLst = false;
                        foreach (SetupVariableJoinAllValues sv in lstFunctionVariables)
                        {
                            if (sv.SetupVariableName.ToLower() == str.ToLower())
                            {
                                inLst = true;
                            }
                        }
                        if (!inLst)
                        {
                            SetupVariableJoinAllValues setupVariableJoinAllValues = new SetupVariableJoinAllValues();
                            setupVariableJoinAllValues.SetupVariableName = str;
                            string commandText = string.Format("select a.SetupVariableID,a.GridDefinitionID from SetupVariables a,SetupVariableDatasets b where a.SetupVariableDatasetID=b.SetupVariableDatasetID and a.SetupVariableName='{0}' and a.SetupVariableDatasetID={1}", str, VariableDatasetID);
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
            catch (Exception ex)
            {
            }
        }

        public static DataTable csvToDataTable(string file, bool isRowOneHeader)
        {

            DataTable csvDataTable = new DataTable();

            String[] csvData = File.ReadAllLines(file);

            if (csvData.Length == 0)
            {
                throw new Exception("CSV File Appears to be Empty");
            }

            String[] headings = csvData[0].Split(',');
            int index = 0;
            if (isRowOneHeader)
            {
                index = 1;
                for (int i = 0; i < headings.Length; i++)
                {
                    headings[i] = headings[i].Replace(" ", "_");

                    csvDataTable.Columns.Add(headings[i], typeof(string));
                }
            }
            else
            {
                for (int i = 0; i < headings.Length; i++)
                {
                    csvDataTable.Columns.Add("col" + (i + 1).ToString(), typeof(string));
                }
            }

            for (int i = index; i < csvData.Length; i++)
            {
                DataRow row = csvDataTable.NewRow();

                for (int j = 0; j < headings.Length; j++)
                {
                    row[j] = csvData[i].Split(',')[j];
                }

                csvDataTable.Rows.Add(row);
            }


            return csvDataTable;

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

        public static float getValueFromBaseFunctionString(string crid, string FunctionString, double A, double B, double C, Dictionary<string, double> dicBetas,
                Dictionary<string, double> dicDeltas, Dictionary<string, double> dicQZeros, Dictionary<string, double> dicQOnes,
                double Incidence, double POP, double Prevalence, Dictionary<string, double> dicSetupVariables)
        {
            try
            {

                object result = BaseEval.BaseLineEval(crid, FunctionString, A, B, C, dicBetas, dicDeltas, dicQZeros, dicQOnes, Incidence, POP, Prevalence, dicSetupVariables);
                if (result is double)
                {
                    if (double.IsNaN(Convert.ToDouble(result))) return 0;
                    return Convert.ToSingle(Convert.ToDouble(result));
                }
                else
                {
                    result = BaseEval.BaseLineEval(crid, FunctionString, A, B, C, dicBetas, dicDeltas, dicQZeros, dicQOnes, Incidence, POP, Prevalence, dicSetupVariables);
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
        public static float getValueFromPointEstimateFunctionString(string crid, string FunctionString, double A, double B, double C, Dictionary<string, double> dicBetas,
                Dictionary<string, double> dicDeltas, Dictionary<string, double> dicQZeros, Dictionary<string, double> dicQOnes,
                double Incidence, double POP, double Prevalence, Dictionary<string, double> dicSetupVariables)
        {
            try
            {

                object result = PointEstimateEval.PointEstimateEval(crid, FunctionString, A, B, C, dicBetas, dicDeltas, dicQZeros, dicQOnes, Incidence, POP, Prevalence, dicSetupVariables);
                if (result is double)
                {
                    if (double.IsNaN(Convert.ToDouble(result))) return 0;
                    return Convert.ToSingle(Convert.ToDouble(result));
                }
                else
                {
                    result = PointEstimateEval.PointEstimateEval(crid, FunctionString, A, B, C, dicBetas, dicDeltas, dicQZeros, dicQOnes, Incidence, POP, Prevalence, dicSetupVariables);
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

        public static bool getAllMetricData(Dictionary<int, Dictionary<string, Dictionary<string, float>>> dicAllMetricData, string colRowKey, string metricKey, Dictionary<int, double> dicValues)
        {
            //clear values
            double value = 0;
            dicValues.Clear();

            //loop over dictionary containing data for all pollutants
            //the key is the pollutant ID
            foreach (KeyValuePair<int, Dictionary<string, Dictionary<string, float>>> kvp in dicAllMetricData)
            {
                //if we have a value for this pollutant, then add it to our list of values
                if (getMetricData(kvp.Value, colRowKey, metricKey, out value))
                {
                    dicValues.Add(kvp.Key, value);
                }
                else
                {
                    //if we don't have a value for a pollutant, then clear any values we have added and return
                    dicValues.Clear();
                    return false;
                }
            }

            return true;
        }

        public static bool getAll365Data(Dictionary<int, Dictionary<string, Dictionary<string, List<float>>>> dicAll365Data, string colRowKey, string metricKey, Dictionary<int, List<float>> dicValues)
        {
            //clear values
            List<float> values = new List<float>();
            dicValues.Clear();

            //loop over dictionary containing data for all pollutants
            //the key is the pollutant ID
            foreach (KeyValuePair<int, Dictionary<string, Dictionary<string, List<float>>>> kvp in dicAll365Data)
            {
                //if we have a value for this pollutant, then add it to our list of values
                if (get365Data(kvp.Value, colRowKey, metricKey, values))
                {
                    dicValues.Add(kvp.Key, values);
                }
                else
                {
                    //if we don't have a value for a pollutant, then clear any values we have added and return
                    dicValues.Clear();
                    return false;
                }
            }

            return true;
        }

        public static bool getMetricData(Dictionary<string, Dictionary<string, float>> dicMetricData, string colRowKey, string metricKey, out double value)
        {
            value = 0;

            if (!dicMetricData.ContainsKey(colRowKey))
            {
                return false;
            }

            if (!dicMetricData[colRowKey].ContainsKey(metricKey))
            {
                return false;
            }

            value = dicMetricData[colRowKey][metricKey];

            return true;
        }

        public static bool get365Data(Dictionary<string, Dictionary<string, List<float>>> dic365Data, string colRowKey, string metricKey, List<float> values)
        {
            values.Clear();

            if (!dic365Data.ContainsKey(colRowKey))
            {
                return false;
            }

            if (!dic365Data[colRowKey].ContainsKey(metricKey))
            {
                return false;
            }

            values = dic365Data[colRowKey][metricKey];

            return true;
        }

        public static Dictionary<int, double> getValuesFrom365Values(Dictionary<int, List<float>> dic365Values, int iDay)
        {
            Dictionary<int, double> dicValues = new Dictionary<int, double>();

            foreach (KeyValuePair<int, List<float>> kvp in dic365Values)
            {
                double value = kvp.Value[iDay];
                dicValues.Add(kvp.Key, value);
            }

            return dicValues;
        }

        public static Dictionary<int, double> getBaseValuesFromModelResultAttributes(string colRowKey, string metricKey)
        {
            Dictionary<int, double> dicValues = new Dictionary<int, double>();

            string[] colRow = colRowKey.Split(',');

            int col = Int32.Parse(colRow[0]);
            int row = Int32.Parse(colRow[1]);

            foreach (BaseControlGroup bcg in CommonClass.LstBaseControlGroup)
            {
                foreach (ModelResultAttribute modelResultAttribute in bcg.Base.ModelResultAttributes)
                {
                    if ((modelResultAttribute.Col == col) && (modelResultAttribute.Row == row))
                    {
                        int pollutantID = bcg.Pollutant.PollutantID;
                        double value = modelResultAttribute.Values[metricKey];

                        dicValues.Add(pollutantID, value);
                        break;

                    }
                }
            }

            return dicValues;
        }

        public static void CheckValuesAgainstThreshold(Dictionary<int, double> dicValues, double Threshold)
        {

            foreach (KeyValuePair<int, double> kvp in dicValues)
            {
                if (Threshold != 0 && kvp.Value < Threshold)
                {
                    dicValues[kvp.Key] = Threshold;
                }
            }

        }

        public static bool CheckValuesAgainstMinimum(Dictionary<int, double> dicValues)
        {
            foreach (KeyValuePair<int, double> kvp in dicValues)
            {
                if (kvp.Value == double.MinValue)
                {
                    return true;
                }
            }

            return false;
        }

        public static Dictionary<int, double> getDeltaQValues(Dictionary<int, double> dicBaseValues, Dictionary<int, double> dicControlValues)
        {
            Dictionary<int, double> dicDeltaQValues = new Dictionary<int, double>();

            foreach (KeyValuePair<int, double> kvp in dicBaseValues)
            {
                double deltaQ = kvp.Value - dicControlValues[kvp.Key];
                dicDeltaQValues.Add(kvp.Key, deltaQ);
            }

            return dicDeltaQValues;
        }

        public static Dictionary<int, double> getDeltaQValuesZeros()
        {
            Dictionary<int, double> dicDeltaQValues = new Dictionary<int, double>();

            foreach (BenMAPPollutant pollutant in CommonClass.LstPollutant)
            {
                dicDeltaQValues.Add(pollutant.PollutantID, 0);
            }

            return dicDeltaQValues;
        }

        public static Dictionary<int, double> getBetaValues(BenMAPHealthImpactFunction hif, int betaIndex)
        {
            Dictionary<int, double> dicBetaValues = new Dictionary<int, double>();
            int pollutantID;
            double beta;

            foreach (CRFVariable variable in hif.Variables)
            {
                pollutantID = variable.Pollutant1ID;
                beta = variable.PollBetas[betaIndex].Beta;
                dicBetaValues.Add(pollutantID, beta);
            }

            return dicBetaValues;
        }

        public static Dictionary<string, double> getVariableNameDictionaryFromPollutantIDDictionary(Dictionary<int, double> dicPollutantID, BenMAPHealthImpactFunction hif)
        {
            Dictionary<string, double> dicVariableName = new Dictionary<string, double>();

            //loop over pollutant id dictionary
            foreach (KeyValuePair<int, double> kvp in dicPollutantID)
            {
                string varName = String.Empty;

                //find matching variable name for pollutant id in health impact function variable list
                foreach (CRFVariable variable in hif.Variables)
                {
                    //do pollutant ids match?
                    if (variable.Pollutant1ID == kvp.Key)
                    {
                        varName = variable.VariableName;
                        dicVariableName.Add(varName, kvp.Value);
                        break;
                    }
                }


            }

            return dicVariableName;
        }

        public static int getPollutantIDFromPollutantNameAndObject(BenMAPHealthImpactFunction hif, string pollName)
        {
            int ID = 0;

            foreach (CRFVariable v in hif.Variables)
            {
                if (String.Equals(v.PollutantName, pollName ,StringComparison.OrdinalIgnoreCase))
                {
                    ID = v.Pollutant1ID;
                    break;
                }
            }

            return ID;
        }

        public static string getPollutantNameFromPollutantIDAndObject(BenMAPHealthImpactFunction hif, int pollID)
        {
            string name = string.Empty;

            foreach (CRFVariable v in hif.Variables)
            {
                if (v.Pollutant1ID == pollID)
                {
                    name = v.PollutantName;
                    break;
                }
            }

            return name;
        }

        public static CRFVariable getVariableFromPollutantID(BenMAPHealthImpactFunction hif, int pollutantID)
        {
            CRFVariable crfVariable = null;

            foreach (CRFVariable v in hif.Variables)
            {
                if (v.Pollutant1ID == pollutantID)
                {
                    crfVariable = v;
                    break;
                }
            }

            return crfVariable;
        }

        public static List<double> getSortedDeltaListFromDictionaryandObject(BenMAPHealthImpactFunction hif, Dictionary<int, double> dicDelta)
        {
            SortedList<string, double> sorted = new SortedList<string, double>();
            List<double> inOrder = new List<double>();

            foreach (KeyValuePair<int, double> p in dicDelta)
            {
                string toAdd = getPollutantNameFromPollutantIDAndObject(hif, p.Key);
                sorted.Add(toAdd, p.Value);
            }

            foreach (KeyValuePair<string, double> s in sorted)
            {
                inOrder.Add(s.Value);
            }

            return inOrder;
        }

        public static double[,] multiplyMatrices(double[,] matrix1, double[,] matrix2)
        {
            int m1rows = matrix1.GetLength(0);
            int m1cols = matrix1.GetLength(1);
            int m2cols = matrix2.GetLength(1);
            double[,] result = new double[m1rows, m2cols];

            for (int row = 0; row < m1rows; row++)
            {
                for (int col = 0; col < m2cols; col++)
                {
                    for (int i = 0; i < m1cols; i++)
                    {
                        result[row, col] += matrix1[row, i] * matrix2[i, col];
                    }
                }
            }

            return result;
        }

        public static double[,] transposeMatrix(double[,] matrix)
        {
            int newRowCount = matrix.GetLength(1);
            int newColCount = matrix.GetLength(0);
            double[,] result = new double[newRowCount, newColCount];

            for (int row = 0; row < newColCount; row++)
            {
                for (int col = 0; col < newRowCount; col++)
                {
                    result[col, row] = matrix[row, col];
                }
            }

            return result;
        }

        public static double CalculateCRSelectFunctionsOneCelStandardError(CRSelectFunction hif, Dictionary<int, double> dicAQDeltas, int betaIndex)
         {
            try
            {
                int m1Width = dicAQDeltas.Count();
                double[,] m1 = new double[1, m1Width];
                double[,] m2 = new double[m1Width, m1Width];

                Dictionary<string, double> dicDeltasWithVar = getVariableNameDictionaryFromPollutantIDDictionary(dicAQDeltas, hif.BenMAPHealthImpactFunction);
                
                for(int i = 1; i <= m1Width; i++)
                {
                    string key = string.Format("P{0}", i);
                    m1[0, i-1] = dicDeltasWithVar[key];
                }

                // set up var/covar matrix from db
                System.Data.DataSet ds = null;
                string commandText = string.Empty;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

                // since beta seasons are added in order, we can use betaInd to index and find betaIDs for the query
                string varName = string.Empty;
                List<int> betaIDs = new List<int>();

                foreach (CRFVariable v in hif.BenMAPHealthImpactFunction.Variables) { betaIDs.Add(v.PollBetas[betaIndex].BetaID); }

                for (int row = 0; row < m1Width; row++)
                {
                    varName = string.Format("P{0}", row+1); // hif.BenMAPHealthImpactFunction.Variables[row].VariableName;
                    commandText = string.Format("select varcov from CRFVARIABLES as crv join CRFBETAS as crb on crb.crfvariableid=crv.crfvariableid join CRFVARCOV as crvc on crvc.crfbetaID1=crb.crfbetaid or crvc.crfbetaID2=crb.crfbetaid where((crfbetaid2={0} and variablename!='{1}') or (crfbetaid1={0} and crfbetaid2={0})) order by crv.crfvariableid", betaIDs[row], varName);
                    ds = fb.ExecuteDataset(CommonClass.Connection, new CommandType(), commandText);

                    int col = 0;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        m2[row, col] = Convert.ToDouble(dr["varcov"]);
                        col++;
                    }
                }

                // standard error calculations
                double[,] result1 = multiplyMatrices(m1, m2);
                double[,] resultT = transposeMatrix(result1);
                double[,] resultSE = multiplyMatrices(m1, resultT);

                if (resultSE.GetLength(0) != 1 || resultSE.GetLength(1) != 1) { throw new Exception("Standard Error not correctly calculated"); }
                double SE = Math.Sqrt(resultSE[0, 0]);

                return SE;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return -99999;
            }
        }
    }
}
