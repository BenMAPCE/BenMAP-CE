using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Xml.Serialization;
using ProtoBuf;
using Meta.Numerics;
using System.Reflection;

namespace BenMAP.APVX
{
    public class APVCommonClass
    {
        public static BaseControlCRSelectFunctionCalculateValue getNoResultBaseControlCRSelectFunctionCalculateValue(BaseControlCRSelectFunctionCalculateValue BaseControlCRSelectFunctionCalculateValue)
        {
            try
            {
                BaseControlCRSelectFunctionCalculateValue copy = new BaseControlCRSelectFunctionCalculateValue();
                copy.CreateTime = BaseControlCRSelectFunctionCalculateValue.CreateTime;
                copy.BaseControlGroup = new List<BaseControlGroup>();
                foreach (BaseControlGroup bcg in BaseControlCRSelectFunctionCalculateValue.BaseControlGroup)
                {
                    BaseControlGroup bcgcopy = new BaseControlGroup();
                    bcgcopy.GridType = bcg.GridType;
                    bcgcopy.Pollutant = bcg.Pollutant;
                    bcgcopy.DeltaQ = bcg.DeltaQ;
                    bcgcopy.Base = bcg.Base; bcgcopy.Control = bcg.Control; copy.BaseControlGroup.Add(bcgcopy);
                }
                copy.BenMAPPopulation = CommonClass.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
                copy.CRLatinHypercubePoints = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints;
                copy.CRDefaultMonteCarloIterations = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRDefaultMonteCarloIterations;
                copy.CRRunInPointMode = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRRunInPointMode;
                copy.CRThreshold = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRThreshold;
                copy.RBenMapGrid = CommonClass.BaseControlCRSelectFunctionCalculateValue.RBenMapGrid;
                copy.lstLog = CommonClass.BaseControlCRSelectFunctionCalculateValue.lstLog;
                copy.lstCRSelectFunctionCalculateValue = new List<CRSelectFunctionCalculateValue>();
                List<float> lstd = new List<float>();
                foreach (CRSelectFunctionCalculateValue crr in BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
                {
                    CRSelectFunctionCalculateValue crrcopy = new CRSelectFunctionCalculateValue();
                    crrcopy.CRSelectFunction = crr.CRSelectFunction;
                    copy.lstCRSelectFunctionCalculateValue.Add(crrcopy);
                }
                return copy;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static ValuationMethodPoolingAndAggregation getNoResultValuationMethodPoolingAndAggregation(ValuationMethodPoolingAndAggregation ValuationMethodPoolingAndAggregationFrom)
        {
            try
            {
                ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation = new ValuationMethodPoolingAndAggregation();
                valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance = ValuationMethodPoolingAndAggregationFrom.IncidencePoolingAndAggregationAdvance;
                BaseControlCRSelectFunctionCalculateValue copy = getNoResultBaseControlCRSelectFunctionCalculateValue(ValuationMethodPoolingAndAggregationFrom.BaseControlCRSelectFunctionCalculateValue);
                valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue = copy;
                valuationMethodPoolingAndAggregation.lstLog = ValuationMethodPoolingAndAggregationFrom.lstLog;
                valuationMethodPoolingAndAggregation.VariableDatasetID = ValuationMethodPoolingAndAggregationFrom.VariableDatasetID;
                valuationMethodPoolingAndAggregation.VariableDatasetName = ValuationMethodPoolingAndAggregationFrom.VariableDatasetName;
                valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase = new List<ValuationMethodPoolingAndAggregationBase>();
                foreach (ValuationMethodPoolingAndAggregationBase vb in ValuationMethodPoolingAndAggregationFrom.lstValuationMethodPoolingAndAggregationBase)
                {
                    ValuationMethodPoolingAndAggregationBase vbOut = new ValuationMethodPoolingAndAggregationBase();

                    vbOut.lstValuationColumns = vb.lstValuationColumns;
                    vbOut.lstQALYColumns = vb.lstQALYColumns;
                    vbOut.lstAllSelectQALYMethodAndValue = null; vbOut.LstAllSelectValuationMethodAndValue = null;
                    vbOut.IncidencePoolingAndAggregation = new IncidencePoolingAndAggregation(); 
                    vbOut.IncidencePoolingAndAggregation.ConfigurationResultsFilePath = vb.IncidencePoolingAndAggregation.ConfigurationResultsFilePath;
                    vbOut.IncidencePoolingAndAggregation.PoolingName = vb.IncidencePoolingAndAggregation.PoolingName;
                    vbOut.IncidencePoolingAndAggregation.lstColumns = vb.IncidencePoolingAndAggregation.lstColumns;
                    vbOut.IncidencePoolingAndAggregation.VariableDataset = vb.IncidencePoolingAndAggregation.VariableDataset;
                    vbOut.IncidencePoolingAndAggregation.Weights = vb.IncidencePoolingAndAggregation.Weights;
                    vbOut.IncidencePoolingAndAggregation.lstAllSelectCRFuntion = new List<AllSelectCRFunction>();
                    vbOut.lstAllSelectCRFunctionIncidenceAggregation = new List<AllSelectCRFunction>(); //YY:
                    vbOut.IncidencePoolingAndAggregation.PoolLevel = vb.IncidencePoolingAndAggregation.PoolLevel;
                    foreach (AllSelectCRFunction alcr in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion)
                    {
                        vbOut.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Add(new AllSelectCRFunction()
                        {
                            Author = alcr.Author,
                            CRID = alcr.CRID,
                            CRIndex = alcr.CRIndex,
                            DataSet = alcr.DataSet,
                            EndAge = alcr.EndAge,
                            EndPoint = alcr.EndPoint,
                            EndPointGroup = alcr.EndPointGroup,
                            EndPointGroupID = alcr.EndPointGroupID,
                            EndPointID = alcr.EndPointID,
                            Ethnicity = alcr.Ethnicity,
                            Function = alcr.Function,
                            Gender = alcr.Gender,
                            ID = alcr.ID,
                            Location = alcr.Location,
                            GeographicArea = alcr.GeographicArea,
                            Metric = alcr.Metric,
                            MetricStatistic = alcr.MetricStatistic,
                            Name = alcr.Name,
                            NodeType = alcr.NodeType,
                            OtherPollutants = alcr.OtherPollutants,
                            PID = alcr.PID,
                            Pollutant = alcr.Pollutant,
                            PoolingMethod = alcr.PoolingMethod,
                            Qualifier = alcr.Qualifier,
                            Race = alcr.Race,
                            SeasonalMetric = alcr.SeasonalMetric,
                            StartAge = alcr.StartAge,
                            Version = alcr.Version,
                            Weight = alcr.Weight,
                            Year = alcr.Year,
                            //YY:new added Nov 2019
                            ChildCount = alcr.ChildCount,
                            CountStudies = alcr.CountStudies,
                            AgeRange = alcr.AgeRange,
                            Nickname = alcr.Nickname
                        });
                        if (alcr.CRSelectFunctionCalculateValue != null)
                        {
                            vbOut.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[vbOut.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Count - 1].CRSelectFunctionCalculateValue = new CRSelectFunctionCalculateValue()
                            {
                                CRSelectFunction = alcr.CRSelectFunctionCalculateValue.CRSelectFunction
                            };
                        }
                        else
                        {
                            vbOut.IncidencePoolingAndAggregation.lstAllSelectCRFuntion[vbOut.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Count - 1].CRSelectFunctionCalculateValue = alcr.CRSelectFunctionCalculateValue;
                        }

                    }
                    //YY: add to copy lstlstAllSelectFunctionIncidenceAggregation

                    foreach (AllSelectCRFunction alcr in vb.lstAllSelectCRFunctionIncidenceAggregation)
                    {
                        vbOut.lstAllSelectCRFunctionIncidenceAggregation.Add(new AllSelectCRFunction()
                        {
                            Author = alcr.Author,
                            CRID = alcr.CRID,
                            CRIndex = alcr.CRIndex,
                            DataSet = alcr.DataSet,
                            EndAge = alcr.EndAge,
                            EndPoint = alcr.EndPoint,
                            EndPointGroup = alcr.EndPointGroup,
                            EndPointGroupID = alcr.EndPointGroupID,
                            EndPointID = alcr.EndPointID,
                            Ethnicity = alcr.Ethnicity,
                            Function = alcr.Function,
                            Gender = alcr.Gender,
                            ID = alcr.ID,
                            Location = alcr.Location,
                            GeographicArea = alcr.GeographicArea,
                            Metric = alcr.Metric,
                            MetricStatistic = alcr.MetricStatistic,
                            Name = alcr.Name,
                            NodeType = alcr.NodeType,
                            OtherPollutants = alcr.OtherPollutants,
                            PID = alcr.PID,
                            Pollutant = alcr.Pollutant,
                            PoolingMethod = alcr.PoolingMethod,
                            Qualifier = alcr.Qualifier,
                            Race = alcr.Race,
                            SeasonalMetric = alcr.SeasonalMetric,
                            StartAge = alcr.StartAge,
                            Version = alcr.Version,
                            Weight = alcr.Weight,
                            Year = alcr.Year,
                            //YY:new added Nov 2019
                            ChildCount = alcr.ChildCount,
                            CountStudies = alcr.CountStudies,
                            AgeRange = alcr.AgeRange,
                            Nickname = alcr.Nickname
                        });
                        if (alcr.CRSelectFunctionCalculateValue != null)
                        {
                            vbOut.lstAllSelectCRFunctionIncidenceAggregation[vbOut.lstAllSelectCRFunctionIncidenceAggregation.Count - 1].CRSelectFunctionCalculateValue = new CRSelectFunctionCalculateValue()
                            {
                                CRSelectFunction = alcr.CRSelectFunctionCalculateValue.CRSelectFunction
                            };
                        }
                        else
                        {
                            vbOut.lstAllSelectCRFunctionIncidenceAggregation[vbOut.lstAllSelectCRFunctionIncidenceAggregation.Count - 1].CRSelectFunctionCalculateValue = alcr.CRSelectFunctionCalculateValue;
                        }

                    }

                    vbOut.IncidencePoolingResult = vb.IncidencePoolingResult;
                    vbOut.lstAllSelectQALYMethod = vb.lstAllSelectQALYMethod;
                    vbOut.LstAllSelectValuationMethod = vb.LstAllSelectValuationMethod;
                    valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Add(vbOut);
                }
                return valuationMethodPoolingAndAggregation;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static bool SaveAPVFile(string strFile, ValuationMethodPoolingAndAggregation ValuationMethodPoolingAndAggregationFrom)
        {
            if (File.Exists(strFile))
                File.Delete(strFile);

            ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation = new ValuationMethodPoolingAndAggregation();
            try
            {
                valuationMethodPoolingAndAggregation = getNoResultValuationMethodPoolingAndAggregation(ValuationMethodPoolingAndAggregationFrom);
                valuationMethodPoolingAndAggregation.CFGRPath = strFile.Substring(0, strFile.Length - 5) + ".cfgrx";
                Configuration.ConfigurationCommonClass.SaveCRFRFile(ValuationMethodPoolingAndAggregationFrom.BaseControlCRSelectFunctionCalculateValue, valuationMethodPoolingAndAggregation.CFGRPath);


                if (File.Exists(strFile))
                    File.Delete(strFile);
                using (FileStream fs = new FileStream(strFile, FileMode.OpenOrCreate))
                {

                    try
                    {
                        valuationMethodPoolingAndAggregation.Version = "BenMAP-CE " + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);
                        valuationMethodPoolingAndAggregation.CreateTime = DateTime.Now;
                        Serializer.Serialize<ValuationMethodPoolingAndAggregation>(fs, valuationMethodPoolingAndAggregation);
                        fs.Close();
                        fs.Dispose();
                        return true;
                    }
                    catch
                    {
                        fs.Close();
                        fs.Dispose();
                        valuationMethodPoolingAndAggregation = null;
                        GC.Collect();
                        return false;

                    }
                }
            }
            catch
            {
                return false;
            }

        }


        public static bool SaveAPVRFile(string strFile, ValuationMethodPoolingAndAggregation ValuationMethodPoolingAndAggregationFrom)
        {

            if (File.Exists(strFile))
                File.Delete(strFile);

            ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation = new ValuationMethodPoolingAndAggregation();
            try
            {
                valuationMethodPoolingAndAggregation = getNoResultValuationMethodPoolingAndAggregation(ValuationMethodPoolingAndAggregationFrom);
                for (int i = 0; i < valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Count; i++)
                {
                    valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[i].IncidencePoolingAndAggregation = ValuationMethodPoolingAndAggregationFrom.lstValuationMethodPoolingAndAggregationBase[i].IncidencePoolingAndAggregation;
                    valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[i].lstAllSelectQALYMethodAndValue = ValuationMethodPoolingAndAggregationFrom.lstValuationMethodPoolingAndAggregationBase[i].lstAllSelectQALYMethodAndValue;
                    valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[i].lstAllSelectQALYMethodAndValueAggregation = ValuationMethodPoolingAndAggregationFrom.lstValuationMethodPoolingAndAggregationBase[i].lstAllSelectQALYMethodAndValueAggregation;
                    valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[i].LstAllSelectValuationMethodAndValue = ValuationMethodPoolingAndAggregationFrom.lstValuationMethodPoolingAndAggregationBase[i].LstAllSelectValuationMethodAndValue;
                    valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[i].LstAllSelectValuationMethodAndValueAggregation = ValuationMethodPoolingAndAggregationFrom.lstValuationMethodPoolingAndAggregationBase[i].LstAllSelectValuationMethodAndValueAggregation;
                    //YY:
                    valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[i].lstAllSelectCRFunctionIncidenceAggregation = ValuationMethodPoolingAndAggregationFrom.lstValuationMethodPoolingAndAggregationBase[i].lstAllSelectCRFunctionIncidenceAggregation;
                }
                valuationMethodPoolingAndAggregation.CreateTime = ValuationMethodPoolingAndAggregationFrom.CreateTime;
                valuationMethodPoolingAndAggregation.CFGRPath = strFile.Substring(0, strFile.Length - 6) + ".cfgrx";
                CommonClass.lstCRResultAggregation.Clear();
                CommonClass.ValuationMethodPoolingAndAggregation.lstValuationResultAggregation.Clear();//YY:

                GC.Collect();

                if (File.Exists(strFile))
                    File.Delete(strFile);
                using (FileStream fs = new FileStream(strFile, FileMode.OpenOrCreate))
                {

                    try
                    {
                        valuationMethodPoolingAndAggregation.Version = "BenMAP-CE " + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);
                        Serializer.Serialize<ValuationMethodPoolingAndAggregation>(fs, valuationMethodPoolingAndAggregation);
                        fs.Close();
                        fs.Dispose();
                        valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase = null;
                        valuationMethodPoolingAndAggregation = null;
                        GC.Collect();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        fs.Close();
                        fs.Dispose();
                        valuationMethodPoolingAndAggregation = null;

                        GC.Collect();
                        return false;

                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public static ValuationMethodPoolingAndAggregation loadAPVRFile(string strFile, ref string err)
        {
            ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation = null;

            try
            {
                using (FileStream fs = new FileStream(strFile, FileMode.Open))
                {
                    try
                    {
                        valuationMethodPoolingAndAggregation = Serializer.Deserialize<ValuationMethodPoolingAndAggregation>(fs);
                    }
                    catch
                    {
                        fs.Close();
                        fs.Dispose();
                        FileStream fsSec = new FileStream(strFile, FileMode.Open);
                        valuationMethodPoolingAndAggregation = Serializer.DeserializeWithLengthPrefix<ValuationMethodPoolingAndAggregation>(fsSec, PrefixStyle.Fixed32);
                        fsSec.Close();
                        fsSec.Dispose();
                    }

                    fs.Close();
                    fs.Dispose();
                }

                // For backward compatability, assume "everywhere" if we don't have an area name set
                foreach (CRSelectFunctionCalculateValue c in valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue)
                {
                    if (string.IsNullOrEmpty(c.CRSelectFunction.GeographicAreaName))
                    {
                        c.CRSelectFunction.GeographicAreaName = Configuration.ConfigurationCommonClass.GEOGRAPHIC_AREA_EVERYWHERE;
                    }
                }
                
                

                //YY: for backward compatability, if vb.lstAllSelectCRFunctionIncidenceAggregation == null the apv file is in old format.
                if (valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.First().lstAllSelectCRFunctionIncidenceAggregation == null)
                {
                    bool isBatch = false;
                    if (CommonClass.InputParams != null && CommonClass.InputParams.Count() > 0 && CommonClass.InputParams[0].ToLower().Contains(".ctlx"))
                    {
                        isBatch = true;
                    }
                    if (!isBatch)
                    {
                        StackTrace stackTrace = new StackTrace();
                        if (stackTrace.GetFrame(1).GetMethod().Name != "trvSetting_NodeMouseDoubleClick")
                        {
                            MessageBox.Show("The apv file you are importing will be converted to the new BenMAP strucure. Please review it after conversion. ", "Older Format Detected",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);
                        }
                    }
                    ConvertOldPoolingTree(valuationMethodPoolingAndAggregation);
                }
                

                //YY: For backward compatability, copy pooled incidence for valuation to pooled incidence for incidence (which is not available before)
                foreach (ValuationMethodPoolingAndAggregationBase vb in valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                {
                    if (vb.lstAllSelectCRFunctionIncidenceAggregation == null)
                    {
                        vb.lstAllSelectCRFunctionIncidenceAggregation = CommonClassExtension.DeepClone(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion);
                    }
                }

                BenMAPSetup benMAPSetup = null;
                if (valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType != null)
                {
                    benMAPSetup = CommonClass.getBenMAPSetupFromName(valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType.SetupName);
                }
                if (benMAPSetup == null)
                {
                    err = "The setup name \"" + valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType.SetupName + "\" can't be found in the database.";
                    return null;
                }
                valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.Setup = benMAPSetup;

                BenMAPGrid benMAPGrid = Grid.GridCommon.getBenMAPGridFromName(valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType.GridDefinitionName, benMAPSetup);
                if (benMAPGrid == null)
                {
                    err = "The grid definition name \"" + valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType.GridDefinitionName + "\" can't be found in the setup \"" + benMAPSetup.SetupName + "\".";
                    return null;
                }

                foreach (BaseControlGroup bcg in valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup)
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

                BenMAPPopulation population = Configuration.ConfigurationCommonClass.getPopulationFromName(valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation.DataSetName, benMAPSetup.SetupID, valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation.Year);
                if (population == null)
                {
                    err = "The population name \"" + valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation.DataSetName + "\" can't be found in the setup \"" + benMAPSetup.SetupName + "\".";
                    return null;
                }
                valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation = population;

                int variableDatasetID = -1;
                if (valuationMethodPoolingAndAggregation.VariableDatasetName != "")
                {
                    ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                    string commandText = string.Format("select setupvariabledatasetid from setupvariabledatasets where setupvariabledatasetname ='{0}' and SetupID={1}", valuationMethodPoolingAndAggregation.VariableDatasetName, benMAPSetup.SetupID);
                    variableDatasetID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                }
                if (variableDatasetID <= 0)
                {
                    valuationMethodPoolingAndAggregation.VariableDatasetID = -1;
                    valuationMethodPoolingAndAggregation.VariableDatasetName = "";
                }

                bool isAPV = false;
                if (strFile.Substring(strFile.Length - 5, 5) == ".apvx")
                {
                    isAPV = true;
                    valuationMethodPoolingAndAggregation.CFGRPath = strFile.Substring(0, strFile.Length - 5) + ".cfgrx";
                }
                else
                {
                    valuationMethodPoolingAndAggregation.CFGRPath = strFile.Substring(0, strFile.Length - 6) + ".cfgrx";
                    string errCFGR = "";
                    valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue = Configuration.ConfigurationCommonClass.LoadCFGRFile(valuationMethodPoolingAndAggregation.CFGRPath, ref errCFGR);
                    if (valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue == null)
                    {
                        return null;
                    }
                    valuationMethodPoolingAndAggregation.CFGRPath = "";
                }


                if (!isAPV)
                {
                    foreach (ValuationMethodPoolingAndAggregationBase vb in valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                    {
                        foreach (AllSelectCRFunction alsr in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion)
                        {
                            if (alsr.CRSelectFunctionCalculateValue != null && alsr.NodeType == 2000)
                            {
                                try
                                {
                                    alsr.CRSelectFunctionCalculateValue = valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(p => p.CRSelectFunction.CRID == alsr.CRID).First();
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
                foreach (ValuationMethodPoolingAndAggregationBase vb in valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                {
                    foreach (AllSelectCRFunction acr in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion)
                    {
                        if (acr.PoolingMethod != "")
                        {
                            List<AllSelectCRFunction> lst = new List<AllSelectCRFunction>();
                            IncidencePoolingandAggregation.getAllChildFromAllSelectCRFunction(acr, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lst);
                            if (lst.Count > 0)
                            {
                                if (acr.CRSelectFunctionCalculateValue == null) acr.CRSelectFunctionCalculateValue = new CRSelectFunctionCalculateValue()
                                {
                                    CRSelectFunction = new CRSelectFunction()
                                    {

                                        StartAge = Convert.ToInt32(lst.Min(p => p.StartAge)),
                                        EndAge = Convert.ToInt32(lst.Max(p => p.EndAge)),
                                    }
                                };
                                if (acr.CRSelectFunctionCalculateValue.CRSelectFunction == null)
                                {
                                    acr.CRSelectFunctionCalculateValue.CRSelectFunction = new CRSelectFunction();
                                }
                                acr.CRSelectFunctionCalculateValue.CRSelectFunction.StartAge = Convert.ToInt32(lst.Min(p => p.StartAge));
                                acr.CRSelectFunctionCalculateValue.CRSelectFunction.EndAge = Convert.ToInt32(lst.Max(p => p.EndAge));
                                acr.StartAge = lst.Min(p => p.StartAge);
                                acr.EndAge = lst.Max(p => p.EndAge);
                            }

                        }
                    }
                }





                GC.Collect();
                CommonClass.GBenMAPGrid = valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.First().GridType;

                return valuationMethodPoolingAndAggregation;

            }
            catch (Exception ex)
            {
                err = "BenMAP-CE was unable to open the file. The file may be corrupt, or it may have been created using a previous incompatible version of BenMAP-CE.";
                return null;
            }
        }

        public static string getGroupAgeRange(object ascr, object lstAll)
        {
            //YY: calculate age ranges (works for both 
            string strStartAge = "";
            string strEndAge = "";
            string strAgeRange = "";
            List<Tuple<int, int>> lstAgeRange = new List<Tuple<int, int>>();

            if (ascr is AllSelectCRFunction && lstAll is List<AllSelectCRFunction>)
            {
                foreach (AllSelectCRFunction cr in (List<AllSelectCRFunction>)lstAll)
                {
                    Tuple<int, int> range = new Tuple<int, int>(Convert.ToInt32(cr.StartAge), Convert.ToInt32(cr.EndAge));
                    if (!lstAgeRange.Contains(range))
                    {
                        lstAgeRange.Add(range);
                    }
                    strStartAge = ((AllSelectCRFunction)ascr).StartAge;
                    strEndAge = ((AllSelectCRFunction)ascr).EndAge;
                }
            }
            else if (ascr is AllSelectValuationMethod && lstAll is List<AllSelectValuationMethod>)
            {
                foreach (AllSelectValuationMethod vm in (List<AllSelectValuationMethod>)lstAll)
                {
                    Tuple<int, int> range = new Tuple<int, int>(Convert.ToInt32(vm.StartAge), Convert.ToInt32(vm.EndAge));
                    if (!lstAgeRange.Contains(range))
                    {
                        lstAgeRange.Add(range);
                    }
                    strStartAge = ((AllSelectValuationMethod)ascr).StartAge;
                    strEndAge = ((AllSelectValuationMethod)ascr).EndAge;
                }
            }
            else
            {
                return "";
            }

            if (lstAgeRange.Count() == 1)
            {
                strAgeRange = strStartAge + "-" + strEndAge;
            }
            else
            {
                lstAgeRange.Sort();
                int i = 0;
                int startAge = 0;
                int endAge = 0;
                foreach (Tuple<int, int> range in lstAgeRange)
                {
                    if (i == 0)
                    {
                        startAge = range.Item1;
                        endAge = range.Item2;
                    }
                    else
                    {
                        if (range.Item1 <= endAge + 1)
                        {
                            if (endAge < range.Item2) endAge = range.Item2;
                        }
                        else
                        {
                            strAgeRange = strAgeRange + startAge.ToString() + "-" + endAge.ToString() + ";";
                            startAge = range.Item1;
                            endAge = range.Item2;
                        }
                    }
                    i++;
                }
                strAgeRange = strAgeRange + startAge.ToString() + "-" + endAge.ToString();
            }
            return strAgeRange;
        }

        public static string getCRPropertyByColumnn(AllSelectCRFunction ascr, List<string> lstColumns, int col)
        {
            //this one is for AllSelectCRFunction (incidence pooling)
            string v = "";
            try
            {
                switch (lstColumns[col].Replace(" ", "").ToLower())
                {
                    case "version": v = Convert.ToString(ascr.Version); break;
                    case "endpoint": v = Convert.ToString(ascr.EndPoint); break;
                    case "author": v = Convert.ToString(ascr.Author); break;
                    case "qualifier": v = Convert.ToString(ascr.Qualifier); break;
                    case "location": v = Convert.ToString(ascr.Location); break;
                    case "startage": v = Convert.ToString(ascr.StartAge); break;
                    case "endage": v = Convert.ToString(ascr.EndAge); break;
                    case "year": v = Convert.ToString(ascr.Year); break;
                    case "otherpollutants": v = Convert.ToString(ascr.OtherPollutants); break;
                    case "race": v = Convert.ToString(ascr.Race); break;
                    case "ethnicity": Convert.ToString(v = ascr.Ethnicity); break;
                    case "gender": v = Convert.ToString(ascr.Gender); break;
                    case "function": v = Convert.ToString(ascr.Function); break;
                    case "pollutant": v = Convert.ToString(ascr.Pollutant); break;
                    case "metric": v = Convert.ToString(ascr.Metric); break;
                    case "seasonalmetric": Convert.ToString(v = ascr.SeasonalMetric); break;
                    case "metricstatistic": v = Convert.ToString(ascr.MetricStatistic); break;
                    case "dataSet": v = Convert.ToString(ascr.DataSet); break;
                    case "endpointid": v = Convert.ToString(ascr.EndPointID); break;
                    case "crselectfunctioncalculatevalue": v = Convert.ToString(ascr.CRSelectFunctionCalculateValue); break;
                    case "weight": v = Convert.ToString(ascr.Weight); break;
                    case "studylocation": v = Convert.ToString(ascr.GeographicArea); break;
                    default: v = ""; break;
                }
            }
            catch (Exception ex)
            {

            }
            
            return v;
        }

        public static string getCRPropertyByColumnn(AllSelectValuationMethod asvm, List<string> lstColumns, int col)
        {
            //this one is for AllSelectValuationMethod (valuation pooling)
            string v = "";
            try
            {
                switch (lstColumns[col].Replace(" ", "").ToLower())
                {
                    case "version": v = Convert.ToString(asvm.Version); break;
                    case "endpoint": v = Convert.ToString(asvm.EndPoint); break;
                    case "author": v = Convert.ToString(asvm.Author); break;
                    case "qualifier": v = Convert.ToString(asvm.Qualifier); break;
                    case "location": v = Convert.ToString(asvm.Location); break;
                    case "startage": v = Convert.ToString(asvm.StartAge); break;
                    case "endage": v = Convert.ToString(asvm.EndAge); break;
                    case "year": v = Convert.ToString(asvm.Year); break;
                    case "otherpollutants": v = Convert.ToString(asvm.OtherPollutants); break;
                    case "race": v = Convert.ToString(asvm.Race); break;
                    case "ethnicity": Convert.ToString(v = asvm.Ethnicity); break;
                    case "gender": v = Convert.ToString(asvm.Gender); break;
                    case "function": v = Convert.ToString(asvm.Function); break;
                    case "pollutant": v = Convert.ToString(asvm.Pollutant); break;
                    case "metric": v = Convert.ToString(asvm.Metric); break;
                    case "seasonalmetric": Convert.ToString(v = asvm.SeasonalMetric); break;
                    case "metricstatistic": v = Convert.ToString(asvm.MetricStatistic); break;
                    case "dataSet": v = Convert.ToString(asvm.DataSet); break;
                    case "endpointid": v = Convert.ToString(asvm.EndPointID); break;
                    case "weight": v = Convert.ToString(asvm.Weight); break;
                    case "studylocation": v = Convert.ToString(asvm.GeographicArea); break;
                    default: v = ""; break;
                }
            }
            catch (Exception ex)
            {

            }

            return v;
        }

        public static void AddMissingCRGroup(List<AllSelectCRFunction> lstAllSelectCRFunction, List<string> lstColumns, int poolLevel)
        {
          
            int maxId = lstAllSelectCRFunction.Max(x => x.ID);
            foreach (AllSelectCRFunction ascr in lstAllSelectCRFunction.ToList())
            {
                //update new fields ChildCount, CountStudies, AgeRange, Nickname
                if (ascr.AgeRange == null || ascr.AgeRange=="")
                {
                    if(ascr.NodeType == 100)
                    {
                        ascr.ChildCount = 0;
                        ascr.CountStudies = 0;
                        ascr.AgeRange = ascr.StartAge + "-" + ascr.EndAge;
                        ascr.Nickname = ascr.Name;
                    }
                    else
                    {
                        ascr.ChildCount = lstAllSelectCRFunction.Where(x => x.PID == ascr.ID).Count();
                        List<AllSelectCRFunction> lstAllChildCR = new List<AllSelectCRFunction>(); //including sub children
                        getAllChildCR(ascr, lstAllSelectCRFunction, ref lstAllChildCR);
                        ascr.CountStudies = lstAllChildCR.Where(x=>x.NodeType==100).Count();
                        ascr.AgeRange = getGroupAgeRange(ascr, lstAllSelectCRFunction);
                        ascr.Nickname = ascr.Name;
                    }
                }
                

                if (ascr.NodeType < poolLevel)
                {
                    var query = lstAllSelectCRFunction.Where(x => x.PID == ascr.ID && x.NodeType > ascr.NodeType+1 ).ToList(); //&& x.NodeType == 100
                    List<AllSelectCRFunction> lstChildCR = query.ToList();
                    if (lstChildCR.Count() > 0)
                    {
                        //only item with NodeType == poolLevel can have functions (NodeType==100) as direct children
                        //otherwise add an item between this item and direct function children
                        //It's also possible that a group is missing between 2 groups when x.NodeType > ascr.NodeType+1
                        List<string> lstString = new List<string>();
                        lstString = getLstStringFromColumnName(lstColumns[ascr.NodeType].Replace(" ", "").ToLower(), lstChildCR); //possible names of ascr's new subgroup
            int countDirectChildren = lstAllSelectCRFunction.Where(x => x.PID == ascr.ID).Count();
                        foreach (string newName in lstString)
                        {
                            AllSelectCRFunction newCr = new AllSelectCRFunction()
                            {
                                CRIndex = -1,
                                Version = ascr.Version,
                                EndPointGroupID = ascr.EndPointGroupID,
                                Name = newName,
                                PoolingMethod = lstString.Count()==1 && countDirectChildren ==1? ascr.PoolingMethod : "", //If it's the only newCR for the parent and this newCR doesn't have any existing syblings , get parent's pooling method
                              EndPointGroup = ascr.EndPointGroup,
                                EndPoint = ascr.EndPoint,
                                Author = ascr.Author,
                                Qualifier = ascr.Qualifier,
                                Location = ascr.Location,
                                StartAge = ascr.StartAge,
                                EndAge = ascr.EndAge,
                                Year = ascr.Year,
                                OtherPollutants = ascr.OtherPollutants,
                                Race = ascr.Race,
                                Ethnicity = ascr.Ethnicity,
                                Gender = ascr.Gender,
                                Function = ascr.Function,
                                Pollutant = ascr.Pollutant,
                                Metric = ascr.Metric,
                                SeasonalMetric = ascr.SeasonalMetric,
                                MetricStatistic = ascr.MetricStatistic,
                                DataSet = ascr.DataSet,
                                NodeType = ascr.NodeType + 1,
                                ID = ++maxId,
                                PID = ascr.ID,
                                EndPointID = ascr.EndPointID,
                                CRID = -1,
                                CRSelectFunctionCalculateValue = ascr.CRSelectFunctionCalculateValue,
                                Weight = 0,
                                GeographicArea = ascr.GeographicArea,
                                GeographicAreaFeatureId = ascr.GeographicAreaFeatureId,
                                ChildCount = 0,
                                CountStudies = ascr.CountStudies,
                                AgeRange = "",
                                Nickname = newName,
                            };

                            //make newCR parent of lstChildCR, calculate ChildCount for newCR
                            foreach (AllSelectCRFunction cr in lstChildCR)
                            {
                                if (getCRPropertyByColumnn(cr, lstColumns, ascr.NodeType) == newName)
                                { 
                                    cr.PID = newCr.ID;
                                    if (cr.NodeType < 100) cr.NodeType = newCr.NodeType + 1;
                                    newCr.ChildCount++;
                                }
                            }

                            //add newCR
                            lstAllSelectCRFunction.Add(newCr);
                        }
                        //allow endpoint group always have a pooling method.
                        ascr.ChildCount = lstString.Count();
                        if (lstString.Count() == 1 && countDirectChildren==1)
                        {
                            //If this parent only need to add one child, parent's pooling method is passed to the child.
                            if (ascr.NodeType == 0) ascr.PoolingMethod = "None"; else ascr.PoolingMethod = "";
                        }

                        AddMissingCRGroup(lstAllSelectCRFunction, lstColumns, poolLevel);
                    }

                }
            }
        }
        public static void AddMissingCRGroupValuation(List<AllSelectValuationMethod> lstAllSelectValuationMethods, List<string> lstColumns, int poolLevel)
        {

            int maxId = lstAllSelectValuationMethods.Max(x => x.ID);
            foreach (AllSelectValuationMethod asvm in lstAllSelectValuationMethods.ToList())
            {
                //update new fields ChildCount, CountStudies, AgeRange, Nickname
                if (asvm.AgeRange == null)
                {
                    if (asvm.NodeType == 2000)
                    {
                        asvm.ChildCount = 0;
                        asvm.CountStudies = 0;
                        asvm.AgeRange = asvm.StartAge + "-" + asvm.EndAge;
                        asvm.Nickname = asvm.Name;
                    }
                    else
                    {
                        asvm.ChildCount = lstAllSelectValuationMethods.Where(x => x.PID == asvm.ID).Count();
                        List<AllSelectValuationMethod> lstAllChildVM = new List<AllSelectValuationMethod>(); //including sub children
                        //getAllChildVM(asvm, lstAllSelectValuationMethods, ref lstAllChildVM);
                        //asvm.CountStudies = lstAllChildVM.Count();
                        //if (asvm.NodeType == 100)
                        //{
                        //    asvm.AgeRange = asvm.StartAge + "-" + asvm.EndAge;
                        //    asvm.Nickname = asvm.Name;
                        //}
                        //else
                        //{
                        //    asvm.AgeRange = getGroupAgeRange(asvm, lstAllSelectValuationMethods);
                            
                        //}
                    }
                }

                if (asvm.NodeType < poolLevel)
                {
                    var query = lstAllSelectValuationMethods.Where(x => x.PID == asvm.ID && x.NodeType > asvm.NodeType + 1 && x.NodeType != 2000).ToList();//&&x.NodeType == 100
                    List<AllSelectValuationMethod> lstChildVM = query.ToList();
                    if (lstChildVM.Count() > 0)
                    {
                        //only item with NodeType == poolLevel can have functions (NodeType==100) as direct children
                        //otherwise add an item between this item and direct function children
                        List<string> lstString = new List<string>();
                        lstString = getLstStringFromColumnName(lstColumns[asvm.NodeType].Replace(" ", "").ToLower(), lstChildVM); //possible names of asvm's new subgroup
            int countDirectChildren = lstAllSelectValuationMethods.Where(x => x.PID == asvm.ID).Count();
            foreach (string newName in lstString)
                        {
                            AllSelectValuationMethod newCr = new AllSelectValuationMethod()
                            {
                                CRIndex = -1,
                                Version = asvm.Version,
                                Name = newName,
                                PoolingMethod = lstString.Count() == 1 && countDirectChildren ==1 ? asvm.PoolingMethod : "", //If it's the only newCR for the parent, get parent's pooling method
                                EndPointGroup = asvm.EndPointGroup,
                                EndPoint = asvm.EndPoint,
                                Author = asvm.Author,
                                Qualifier = asvm.Qualifier,
                                Location = asvm.Location,
                                StartAge = asvm.StartAge,
                                EndAge = asvm.EndAge,
                                Year = asvm.Year,
                                OtherPollutants = asvm.OtherPollutants,
                                Race = asvm.Race,
                                Ethnicity = asvm.Ethnicity,
                                Gender = asvm.Gender,
                                Function = asvm.Function,
                                Pollutant = asvm.Pollutant,
                                Metric = asvm.Metric,
                                SeasonalMetric = asvm.SeasonalMetric,
                                MetricStatistic = asvm.MetricStatistic,
                                DataSet = asvm.DataSet,
                                NodeType = asvm.NodeType + 1,
                                ID = ++maxId,
                                PID = asvm.ID,
                                EndPointID = asvm.EndPointID,
                                CRID = -1,
                                APVID = asvm.APVID, //YY: need to confirm.
                                BenMAPValuationFunction = asvm.BenMAPValuationFunction,
                                lstMonte = asvm.lstMonte,
                                Weight = 0,
                                GeographicArea = asvm.GeographicArea,
                                GeographicAreaFeatureId = asvm.GeographicAreaFeatureId,
                                ChildCount = 0,
                                CountStudies = asvm.CountStudies,
                                AgeRange = asvm.AgeRange,
                                Nickname = newName,
                            };

                            //make newCR parent of lstChildCR and calculate ChildCount for newCR
                            foreach (AllSelectValuationMethod vm in lstChildVM)
                            {
                                if (getCRPropertyByColumnn(vm, lstColumns, asvm.NodeType) == newName)
                                {
                                    vm.PID = newCr.ID;
                                    newCr.ChildCount++;
                                    if (vm.NodeType <100) vm.NodeType = newCr.NodeType + 1;
                                }
                                    
                            }

                            //add newCR
                            lstAllSelectValuationMethods.Add(newCr);

                        }
                        //update parent cr. Endpoint Group always have pooling method. 
                        asvm.ChildCount = lstString.Count();
                        if (lstString.Count() == 1 && countDirectChildren == 1)
                        {
                            if (asvm.NodeType == 0) asvm.PoolingMethod = "None"; else asvm.PoolingMethod = "";
                        }

                        AddMissingCRGroupValuation(lstAllSelectValuationMethods, lstColumns, poolLevel);
                    }

                }
            }
        }
        public static List<string> getLstStringFromColumnName(string columName, object lstAllCR)
        {
            List<string> lstString = new List<string>();
            if (lstAllCR is List<AllSelectCRFunction>)
            {
                List<AllSelectCRFunction> lstCR = (List<AllSelectCRFunction>)lstAllCR;
                switch (columName)
                {
                    case "endpoint":
                        lstString = lstCR.Select(p => p.EndPoint).Distinct().ToList();
                        break;
                    case "author":
                        lstString = lstCR.Select(p => p.Author).Distinct().ToList();
                        break;
                    case "qualifier":
                        lstString = lstCR.Select(p => p.Qualifier).Distinct().ToList();
                        break;
                    case "location":
                        lstString = lstCR.Select(p => p.Location).Distinct().ToList();
                        break;
                    case "startage":
                        lstString = lstCR.Select(p => p.StartAge.ToString()).Distinct().ToList();
                        break;
                    case "endage":
                        lstString = lstCR.Select(p => p.EndAge.ToString()).Distinct().ToList();
                        break;
                    case "year":
                        lstString = lstCR.Select(p => p.Year.ToString()).Distinct().ToList();
                        break;
                    case "otherpollutants":
                        lstString = lstCR.Select(p => p.OtherPollutants).Distinct().ToList();
                        break;
                    case "race":
                        lstString = lstCR.Select(p => p.Race).Distinct().ToList();
                        break;
                    case "ethnicity":
                        lstString = lstCR.Select(p => p.Ethnicity).Distinct().ToList();
                        break;
                    case "gender":
                        lstString = lstCR.Select(p => p.Gender).Distinct().ToList();
                        break;
                    case "function":
                        lstString = lstCR.Select(p => p.Function).Distinct().ToList();
                        break;
                    case "pollutant":
                        lstString = lstCR.Select(p => p.Pollutant).Distinct().ToList();
                        break;
                    case "metric":
                        lstString = lstCR.Select(p => p.Metric).Distinct().ToList();
                        break;
                    case "seasonalmetric":
                        lstString = lstCR.Select(p => p.SeasonalMetric).Distinct().ToList();
                        break;
                    case "metricstatistic":
                        lstString = lstCR.Select(p => p.MetricStatistic).Distinct().ToList();
                        break;
                    case "dataset":
                        lstString = lstCR.Select(p => p.DataSet).Distinct().ToList();
                        break;
                    case "studylocation":
                        lstString = lstCR.Select(p => p.GeographicArea).Distinct().ToList();
                        break;
                    case "version":

                        lstString = lstCR.Select(p => p.Version).Distinct().ToList();
                        break;
                }
            }
            else if(lstAllCR is List<AllSelectValuationMethod>)
            {
                List<AllSelectValuationMethod> lstCR = (List<AllSelectValuationMethod>)lstAllCR;
                switch (columName)
                {
                    case "endpoint":
                        lstString = lstCR.Select(p => p.EndPoint).Distinct().ToList();
                        break;
                    case "author":
                        lstString = lstCR.Select(p => p.Author).Distinct().ToList();
                        break;
                    case "qualifier":
                        lstString = lstCR.Select(p => p.Qualifier).Distinct().ToList();
                        break;
                    case "location":
                        lstString = lstCR.Select(p => p.Location).Distinct().ToList();
                        break;
                    case "startage":
                        lstString = lstCR.Select(p => p.StartAge.ToString()).Distinct().ToList();
                        break;
                    case "endage":
                        lstString = lstCR.Select(p => p.EndAge.ToString()).Distinct().ToList();
                        break;
                    case "year":
                        lstString = lstCR.Select(p => p.Year.ToString()).Distinct().ToList();
                        break;
                    case "otherpollutants":
                        lstString = lstCR.Select(p => p.OtherPollutants).Distinct().ToList();
                        break;
                    case "race":
                        lstString = lstCR.Select(p => p.Race).Distinct().ToList();
                        break;
                    case "ethnicity":
                        lstString = lstCR.Select(p => p.Ethnicity).Distinct().ToList();
                        break;
                    case "gender":
                        lstString = lstCR.Select(p => p.Gender).Distinct().ToList();
                        break;
                    case "function":
                        lstString = lstCR.Select(p => p.Function).Distinct().ToList();
                        break;
                    case "pollutant":
                        lstString = lstCR.Select(p => p.Pollutant).Distinct().ToList();
                        break;
                    case "metric":
                        lstString = lstCR.Select(p => p.Metric).Distinct().ToList();
                        break;
                    case "seasonalmetric":
                        lstString = lstCR.Select(p => p.SeasonalMetric).Distinct().ToList();
                        break;
                    case "metricstatistic":
                        lstString = lstCR.Select(p => p.MetricStatistic).Distinct().ToList();
                        break;
                    case "dataset":
                        lstString = lstCR.Select(p => p.DataSet).Distinct().ToList();
                        break;
                    case "studylocation":
                        lstString = lstCR.Select(p => p.GeographicArea).Distinct().ToList();
                        break;
                    case "version":

                        lstString = lstCR.Select(p => p.Version).Distinct().ToList();
                        break;
                }
            }
            return lstString;
        }

        public static void ConvertOldPoolingTree(ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation)
        {
            //Pooling trees in old apvrx files do not show all level of pooling. Convert them to new trees. 
            //old apvrx files do not have PoolLevel and no values in vb.lstAllSelectCRFunctionIncidenceAggregation
            if (valuationMethodPoolingAndAggregation == null) return;
            //foreach(ValuationMethodPoolingAndAggregationBase vb in vmpa.lstValuationMethodPoolingAndAggregationBase) get pooling level
            //

            foreach (ValuationMethodPoolingAndAggregationBase vb in valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
            {
                //incidence pooling
                List<AllSelectCRFunction> lstAllSelectCRFunction = null;
                if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion != null && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Count() > 0)
                {
                    lstAllSelectCRFunction = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion;
                    if (lstAllSelectCRFunction.Count() == 1) continue;
                    List<string> lstColumns = vb.IncidencePoolingAndAggregation.lstColumns;
                    //update lstColumn to move columns with same values to the end.
                    List<string> lstNewColumns = new List<string>();
                    List<string> lstWaitList = new List<string>();
                    for(int i = 0; i < lstColumns.Count(); i++)
                    {
                        if(getLstStringFromColumnName(lstColumns[i].Replace(" ", "").ToLower(), lstAllSelectCRFunction.Where(x => x.NodeType == 100).ToList()).Count() <= 1) //move columns no need to pool to the end
                        {
                            lstWaitList.Add(lstColumns[i]);
                        }
                        else
                        {
                            lstNewColumns.Add(lstColumns[i]);
                        }
                    }
                    lstNewColumns.AddRange(lstWaitList);
                    vb.IncidencePoolingAndAggregation.lstColumns = lstNewColumns;


                    //update NodeType as some endpoint crgroups are renamed to endpoint group names
                    foreach (AllSelectCRFunction ascrP in lstAllSelectCRFunction)
                    {
                        if (ascrP.NodeType == 100) continue;
                        AllSelectCRFunction ascrC = lstAllSelectCRFunction.Where(x => x.PID == ascrP.ID).First();
                        if (ascrP.Name == ascrP.EndPointGroup)
                        {
                            ascrP.NodeType = 0;
                        }
                        else
                        {
                            
                            for (int i = 0; i < lstNewColumns.Count(); i++)
                            {
                                if(ascrP.Name == getCRPropertyByColumnn(ascrC, lstNewColumns, i))
                                {
                                    ascrP.NodeType = i + 1;
                                    break;
                                }
                            }
                        }
                    }

                    int poolLevel = lstAllSelectCRFunction.Where(x => x.NodeType != 100).Max(y => y.NodeType);

                    vb.IncidencePoolingAndAggregation.PoolLevel = poolLevel;
                    AddMissingCRGroup(lstAllSelectCRFunction, lstNewColumns, poolLevel);
                }

                //valution pooling
                List<AllSelectValuationMethod> allSelectValuationMethods = null;
                if (vb.LstAllSelectValuationMethod != null && vb.LstAllSelectValuationMethod.Count() > 0)
                {
                    allSelectValuationMethods = vb.LstAllSelectValuationMethod;
                    if (allSelectValuationMethods.Count() == 1) continue;
                    List<string> lstColumns = vb.lstValuationColumns;
                    //update lstColumn to move columns with same values to the end.
                    List<string> lstNewColumns = new List<string>();
                    //use same columns as incidence pooling
                    lstNewColumns = vb.IncidencePoolingAndAggregation.lstColumns;
                    //update NodeType as some endpoint crgroups are renamed to endpoint group names
                    //also update ChildCount, CountStudies, AgeRange and NickName from incidence pooling.
                    foreach (AllSelectValuationMethod asvmP in allSelectValuationMethods)
                    {
                        if (asvmP.NodeType == 100 || asvmP.NodeType == 2000) continue;
            asvmP.CountStudies = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.ID == asvmP.ID).Select(p => p.CountStudies).First();
            asvmP.AgeRange = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.ID == asvmP.ID).Select(p => p.AgeRange).First();
            asvmP.Nickname = vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.ID == asvmP.ID).Select(p => p.Nickname).First();
            AllSelectValuationMethod asvmC = allSelectValuationMethods.Where(x => x.PID == asvmP.ID).First();
                        if (asvmP.Name == asvmC.EndPointGroup)
                        {
                            asvmP.NodeType = 0;
                        }
                        else
                        {
                            for (int i = 0; i < lstNewColumns.Count(); i++)
                            {
                                if (asvmP.Name == getCRPropertyByColumnn(asvmC, lstNewColumns, i))
                                {
                                    asvmP.NodeType = i + 1;
                                    break;
                                }
                            }
                        }
                    }

                    //int poolLevel = allSelectValuationMethods.Where(x => x.NodeType != 100 && x.NodeType != 2000).Max(y => y.NodeType);
                    int poolLevel = vb.IncidencePoolingAndAggregation.PoolLevel;
                    if (poolLevel != vb.IncidencePoolingAndAggregation.PoolLevel)
                    {
                        //pool level here should match pool level in pooling
                        System.Windows.Forms.MessageBox.Show("Something is wrong");
                        return;
                    }
                    //vb.IncidencePoolingAndAggregation.PoolLevel = poolLevel;
                    AddMissingCRGroupValuation(allSelectValuationMethods, lstNewColumns, poolLevel);
                }

            }

        }
        public static void getAllSelectValuationMethodAndValueFromResultCopy(ref AllSelectValuationMethodAndValue allSelectValuationMethodAndValue)
        {


        }



        private static Tools.CalculateFunctionString _valuationEval;
        internal static Tools.CalculateFunctionString ValuationEval
        {
            get
            {
                if (_valuationEval == null)
                    _valuationEval = new Tools.CalculateFunctionString();
                return APVCommonClass._valuationEval;
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
      .Replace("sqr(", "Math.Sqrt(").Replace("sqr (", "Math.Sqrt(")
     .Replace("sqrt(", "Math.Sqrt(").Replace("sqrt (", "Math.Sqrt(")
     .Replace("tan(", "Math.Tan(").Replace("tan (", "Math.Tan(")
     .Replace("tanh(", "Math.Tanh(").Replace("tanh (", "Math.Tanh(")
     .Replace("truncate(", "Math.Truncate(").Replace("truncate (", "Math.Truncate(");


                if (result.Contains("if") && result.Contains(":="))
                {

                    result = result.Replace(" and", " && ").Replace(")and", ")&&").Replace(" or", " || ").Replace(")or", ")||").Replace(":=", " return ")
                        .Replace("result", " ").Replace("else", ";else").Replace("then", " ").Replace("<>", "!=");
                    result = result + "; return -999999999;";

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













        public static double getValueFromValuationFunctionString(string FunctionString, double A, double B, double C, double D, double AllGoodsIndex, double MedicalCostIndex, double WageIndex, double LagAdjustment, Dictionary<string, double> dicSetupVariables)
        {

            object result = ValuationEval.ValuationEval(FunctionString, A, B, C, D, AllGoodsIndex, MedicalCostIndex, WageIndex, LagAdjustment, dicSetupVariables);
            if (result is double)
            {
                if (double.IsNaN(Convert.ToDouble(result))) return 0;
                return Convert.ToDouble(result);
            }
            else
            {
                result = ValuationEval.ValuationEval(FunctionString, A, B, C, D, AllGoodsIndex, MedicalCostIndex, WageIndex, LagAdjustment, dicSetupVariables);
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

        public static void getInflationFromDataSetIDAndYear(int InflationDataSetID, int Year, ref double AllGoodsIndex, ref double MedicalCostIndex, ref double WageIndex)
        {
            try
            {
                string commandText = string.Format("select    AllGoodsIndex    ,   MedicalCostIndex ,   WageIndex        from InflationEntries where InflationDatasetID={0} and YYear={1}   ", InflationDataSetID, Year);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];

                    AllGoodsIndex = Convert.ToDouble(dr["AllGoodsIndex"]);
                    MedicalCostIndex = Convert.ToDouble(dr["MedicalCostIndex"]);
                    WageIndex = Convert.ToDouble(dr["WageIndex"]);
                }
                else
                {

                    AllGoodsIndex = 1; MedicalCostIndex = 1; WageIndex = 1;
                }
            }
            catch (Exception ex)
            {

            }


        }
        public static Dictionary<string, double> getIncomeGrowthFactorsFromDataSetIDAndYear(int DataSetID, int Year)
        {
            try
            {
                Dictionary<string, double> dicIncomeGrowthFactors = new Dictionary<string, double>();
                string commandText = string.Format(" select IncomeGrowthAdjdatasetID,YYear,Mean,EndPointGroups from IncomeGrowthAdjfactors where IncomeGrowthAdjdatasetID={0} and YYear={1}", DataSetID, Year);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    dicIncomeGrowthFactors.Add(dr["EndPointGroups"].ToString(), Convert.ToDouble(dr["Mean"]));


                }

                return dicIncomeGrowthFactors;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static List<BenMAPValuationFunction> getLstBenMAPValuationFuncitonFromEndPointGroupID(int EndPointGroupID)
        {
            try
            {
                List<BenMAPValuationFunction> lstBenMapValuationFunction = new List<BenMAPValuationFunction>();
                string commandText = string.Format("  select a.ValuationFunctionID,a.ValuationFunctionDatasetID,e.ValuationFunctionDatasetName,a.EndpointGroupID,b.EndPointGroupName,                     " +
    " a.EndpointID,c.EndPointName,a.Qualifier,a.Reference,a.StartAge,a.EndAge,a.FunctionalFormID,d.FunctionalFormText,a.A,  " +
    " a.NameA,a.DistA,a.P1A,a.P2A,a.B,a.NameB,a.C,a.NameC,a.D,a.NameD                                                       " +
    " from valuationfunctions a ,EndPointGroups b,EndPoints c,ValuationFunctionalForms d ,      ValuationFunctionDatasets e                             " +
    " where a.EndPointGroupID=b.EndPointGroupID and a.EndPointID=c.EndPointID and a.FunctionalFormID=d.FunctionalFormID and a.ValuationFunctionDatasetID=e.ValuationFunctionDatasetID and a.EndPointGroupID={0}  and e.SetupID={1} ", EndPointGroupID, CommonClass.MainSetup.SetupID);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    BenMAPValuationFunction bvf = new BenMAPValuationFunction();
                    bvf.DistA = Convert.ToString(dr["DistA"]);
                    bvf.A = Convert.ToDouble(dr["A"]);
                    bvf.NameA = Convert.ToString(dr["NameA"]);
                    bvf.P1A = Convert.ToDouble(dr["P1A"]);
                    bvf.P2A = Convert.ToDouble(dr["P2A"]);
                    bvf.NameB = Convert.ToString(dr["NameB"]);
                    bvf.B = Convert.ToDouble(dr["B"]);
                    bvf.NameC = Convert.ToString(dr["NameC"]);
                    bvf.C = Convert.ToDouble(dr["C"]);
                    bvf.NameD = Convert.ToString(dr["NameD"]);
                    bvf.D = Convert.ToDouble(dr["D"]);
                    bvf.DataSet = Convert.ToString(dr["ValuationFunctionDatasetName"]);
                    bvf.EndAge = Convert.ToInt32(dr["EndAge"]);
                    bvf.EndPoint = Convert.ToString(dr["EndPointName"]);
                    bvf.EndPointGroup = Convert.ToString(dr["EndPointGroupName"]);
                    bvf.EndPointID = Convert.ToInt32(dr["EndPointID"]);
                    bvf.EndPointGroupID = Convert.ToInt32(dr["EndPointGroupID"]);
                    bvf.Function = Convert.ToString(dr["FunctionalFormText"]);
                    bvf.ID = Convert.ToInt32(dr["ValuationFunctionID"]);
                    bvf.Qualifier = Convert.ToString(dr["Qualifier"]);
                    bvf.Reference = Convert.ToString(dr["Reference"]);
                    bvf.StartAge = Convert.ToInt32(dr["StartAge"]);
                    lstBenMapValuationFunction.Add(bvf);





                }
                return lstBenMapValuationFunction;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<BenMAPValuationFunction> getLstBenMAPValuationFuncitonFromEndPointID(int EndPointID)
        {
            try
            {
                List<BenMAPValuationFunction> lstBenMapValuationFunction = new List<BenMAPValuationFunction>();
                string commandText = string.Format("  select a.ValuationFunctionID,a.ValuationFunctionDatasetID,e.ValuationFunctionDatasetName,a.EndpointGroupID,b.EndPointGroupName,                     " +
    " a.EndpointID,c.EndPointName,a.Qualifier,a.Reference,a.StartAge,a.EndAge,a.FunctionalFormID,d.FunctionalFormText,a.A,  " +
    " a.NameA,a.DistA,a.P1A,a.P2A,a.B,a.NameB,a.C,a.NameC,a.D,a.NameD                                                       " +
    " from valuationfunctions a ,EndPointGroups b,EndPoints c,ValuationFunctionalForms d ,      ValuationFunctionDatasets e                             " +
    " where a.EndPointGroupID=b.EndPointGroupID and a.EndPointID=c.EndPointID and a.FunctionalFormID=d.FunctionalFormID and a.ValuationFunctionDatasetID=e.ValuationFunctionDatasetID and a.EndPointID={0}    ", EndPointID);
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    BenMAPValuationFunction bvf = new BenMAPValuationFunction();
                    bvf.DistA = Convert.ToString(dr["DistA"]);
                    bvf.A = Convert.ToDouble(dr["A"]);
                    bvf.NameA = Convert.ToString(dr["NameA"]);
                    bvf.P1A = Convert.ToDouble(dr["P1A"]);
                    bvf.P2A = Convert.ToDouble(dr["P2A"]);
                    bvf.NameB = Convert.ToString(dr["NameB"]);
                    bvf.B = Convert.ToDouble(dr["B"]);
                    bvf.NameC = Convert.ToString(dr["NameC"]);
                    bvf.C = Convert.ToDouble(dr["C"]);
                    bvf.NameD = Convert.ToString(dr["NameD"]);
                    bvf.D = Convert.ToDouble(dr["D"]);
                    bvf.DataSet = Convert.ToString(dr["ValuationFunctionDatasetName"]);
                    bvf.EndAge = Convert.ToInt32(dr["EndAge"]);
                    bvf.EndPoint = Convert.ToString(dr["EndPointName"]);
                    bvf.EndPointGroup = Convert.ToString(dr["EndPointGroupName"]);
                    bvf.EndPointID = Convert.ToInt32(dr["EndPointID"]);
                    bvf.EndPointGroupID = Convert.ToInt32(dr["EndPointGroupID"]);
                    bvf.Function = Convert.ToString(dr["FunctionalFormText"]);
                    bvf.ID = Convert.ToInt32(dr["ValuationFunctionID"]);
                    bvf.Qualifier = Convert.ToString(dr["Qualifier"]);
                    bvf.Reference = Convert.ToString(dr["Reference"]);
                    bvf.StartAge = Convert.ToInt32(dr["StartAge"]);
                    lstBenMapValuationFunction.Add(bvf);





                }
                return lstBenMapValuationFunction;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static List<RowCol> getAllRowColFromGridType(BenMAPGrid benMAPGrid)
        {
            try
            {
                List<RowCol> lstRowCol = new List<RowCol>();
                DotSpatial.Data.IFeatureSet fs = new DotSpatial.Data.FeatureSet();
                int iCol = -1;
                int iRow = -1;
                if (benMAPGrid is ShapefileGrid)
                {

                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (benMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                    {
                        fs = DotSpatial.Data.FeatureSet.Open(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (benMAPGrid as ShapefileGrid).ShapefileName + ".shp");
                        int i = 0;
                        foreach (DataColumn dc in fs.DataTable.Columns)
                        {
                            if (dc.ColumnName.ToLower() == "col")
                                iCol = i;
                            if (dc.ColumnName.ToLower() == "row")
                                iRow = i;

                            i++;
                        }
                        foreach (DataRow dr in fs.DataTable.Rows)
                        {
                            lstRowCol.Add(new RowCol() { Col = Convert.ToInt32(dr[iCol]), Row = Convert.ToInt32(dr[iRow]) });
                        }
                        fs.Close();
                    }
                }
                else if (benMAPGrid is RegularGrid)
                {

                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (benMAPGrid as RegularGrid).ShapefileName + ".shp"))
                    {
                        fs = DotSpatial.Data.FeatureSet.Open(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (benMAPGrid as RegularGrid).ShapefileName + ".shp");
                        int i = 0;
                        foreach (DataColumn dc in fs.DataTable.Columns)
                        {
                            if (dc.ColumnName.ToLower() == "col")
                                iCol = i;
                            if (dc.ColumnName.ToLower() == "row")
                                iRow = i;

                            i++;
                        }
                        foreach (DataRow dr in fs.DataTable.Rows)
                        {
                            lstRowCol.Add(new RowCol() { Col = Convert.ToInt32(dr[iCol]), Row = Convert.ToInt32(dr[iRow]) });
                        }
                        fs.Close();
                    }

                }
                return lstRowCol;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static Dictionary<string, double> getOneAllSelectValuationMethodAPVValue(Dictionary<int, Dictionary<string, double>> dicAllCRResultValues, AllSelectValuationMethod allSelectValuationMethod, List<RowCol> lstRowCol, CRSelectFunctionCalculateValue crSelectFunctionCalculateValue, double AllGoodsIndex, double MedicalCostIndex, double WageIndex, double Income, double[] lhsDesignResult, Dictionary<string, double> dicSetupVariables)
        {
            try
            {
                double PointEstimate = 0.0;
                double PointEstimateOut = 0.0;
                Dictionary<string, double> dicOut = new Dictionary<string, double>();
                List<APVValueAttribute> lstAPVValueAttribute = new List<APVValueAttribute>();
                Dictionary<string, double> dicValues = dicAllCRResultValues[crSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.ID];

                foreach (RowCol rowcol in lstRowCol)
                {
                    try
                    {
                        PointEstimate = dicValues[rowcol.Col + "," + rowcol.Row];

                    }
                    catch (Exception ex)
                    { }
                    PointEstimateOut = getValueFromValuationFunctionString(allSelectValuationMethod.Function, allSelectValuationMethod.BenMAPValuationFunction.A, allSelectValuationMethod.BenMAPValuationFunction.B,
                        allSelectValuationMethod.BenMAPValuationFunction.C, allSelectValuationMethod.BenMAPValuationFunction.D, AllGoodsIndex, MedicalCostIndex, WageIndex, 0, dicSetupVariables);
                    dicOut.Add(rowcol.Col + "," + rowcol.Row, PointEstimateOut);

                }


                return dicOut;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<int, Dictionary<string, List<float>>> getAllCrCalculateValues(List<CRSelectFunctionCalculateValue> lstCRSelectFunctionCalculateValue)
        {
            Dictionary<int, Dictionary<string, List<float>>> dicAll = new Dictionary<int, Dictionary<string, List<float>>>();
            int i = 0;
            foreach (CRSelectFunctionCalculateValue crfcv in lstCRSelectFunctionCalculateValue)
            {
                Dictionary<string, List<float>> dicValues = new Dictionary<string, List<float>>();
                List<float> dArray = new List<float>();
                foreach (CRCalculateValue crcv in crfcv.CRCalculateValues)
                {
                    dArray = new List<float>();
                    dArray.Add(crcv.PointEstimate);
                    if (crcv.LstPercentile != null)
                        dArray.AddRange(crcv.LstPercentile);
                    dicValues.Add(crcv.Col + "," + crcv.Row, dArray);
                }

                dicAll.Add(i, dicValues);
                i++;
            }

            return dicAll;
        }
        public static void getPoolingMethodCRFromAllSelectCRFunction(bool isCalulate, ref List<AllSelectCRFunction> lstAllSelectCRFunctionNone, ref List<AllSelectCRFunction> lstAllSelectCRFunctionAll, int nodetype, List<string> lstColumns, Boolean calculatePop = true)
        {
            try
            {
                int j = 0;
                int iCRIDMax = lstAllSelectCRFunctionAll.Max(p => p.CRID) + 1;
                if (iCRIDMax < 9999) iCRIDMax = 9999;
                for (int i = nodetype; i >= 0; i--)
                {
                    var query = lstAllSelectCRFunctionNone.Where(p => p.NodeType == i).ToList();

                    foreach (AllSelectCRFunction alsc in query)
                    {
                        if (alsc.PoolingMethod != "" && alsc.PoolingMethod != "None")
                        {
                            List<AllSelectCRFunction> lstSec = new List<AllSelectCRFunction>();
                            getAllChildCRNotNoneCalulate(alsc, lstAllSelectCRFunctionAll, ref lstSec);
                            List<AllSelectCRFunction> lstTemp = lstSec.Where(p => p.CRSelectFunctionCalculateValue != null && p.CRSelectFunctionCalculateValue.CRCalculateValues != null).ToList();
                            if (isCalulate)
                            {
                                if (alsc.CRSelectFunctionCalculateValue != null)
                                {
                                    alsc.CRSelectFunctionCalculateValue.CRCalculateValues = null;
                                    alsc.CRSelectFunctionCalculateValue = null;
                                }
                                List<double> lstWeight = lstTemp.Select(a => a.Weight).ToList();
                                alsc.CRSelectFunctionCalculateValue = getPoolingMethodCRSelectFunctionCalculateValue(lstTemp.Select(a => a.CRSelectFunctionCalculateValue).ToList(), getPoolingMethodTypeEnumFromString(alsc.PoolingMethod), ref lstWeight);
                                for (int iTemp = 0; iTemp < lstTemp.Count; iTemp++)
                                {
                                    lstTemp[iTemp].Weight = lstWeight[iTemp];
                                }
                            }
                            else
                                alsc.CRSelectFunctionCalculateValue = new CRSelectFunctionCalculateValue();
                            if (alsc.CRSelectFunctionCalculateValue == null || alsc.CRSelectFunctionCalculateValue.CRCalculateValues == null)
                            {

                            }
                            alsc.EndPointGroup = lstSec.First().EndPointGroup;

                            alsc.CRSelectFunctionCalculateValue.CRSelectFunction = new CRSelectFunction();
                            if (alsc.CRID == -1)
                            {
                                alsc.CRSelectFunctionCalculateValue.CRSelectFunction.CRID = 9999 + j;
                                alsc.CRID = iCRIDMax + j;
                                j++;
                            }
                            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.StartAge = alsc.StartAge == "" ? lstSec.Min(a => a.CRSelectFunctionCalculateValue.CRSelectFunction.StartAge) : Convert.ToInt32(alsc.StartAge);
                            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.EndAge = alsc.EndAge == "" ? lstSec.Max(a => a.CRSelectFunctionCalculateValue.CRSelectFunction.EndAge) : Convert.ToInt32(alsc.EndAge);

                            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction = new BenMAPHealthImpactFunction()
                            {
                                ID = alsc.CRID == -1 ? iCRIDMax + j : alsc.CRID,
                                EndPointGroup = lstSec.First().EndPointGroup,
                                EndPointGroupID = lstSec.First().EndPointGroupID,
                                EndPoint = alsc.EndPoint,
                                Author = alsc.Author,
                                Qualifier = alsc.Qualifier,
                                strLocations = alsc.Location,
                                GeographicAreaName = alsc.GeographicArea,
                                StartAge = alsc.StartAge == "" ? lstSec.Min(a => a.CRSelectFunctionCalculateValue.CRSelectFunction.StartAge) : Convert.ToInt32(alsc.StartAge),
                                EndAge = alsc.EndAge == "" ? lstSec.Max(a => a.CRSelectFunctionCalculateValue.CRSelectFunction.EndAge) : Convert.ToInt32(alsc.EndAge),
                                Year = alsc.Year == "" ? lstSec.First().CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Year : Convert.ToInt32(alsc.Year),
                                OtherPollutants = alsc.OtherPollutants,
                                Race = alsc.Race,
                                Ethnicity = alsc.Ethnicity,
                                Gender = alsc.Gender,
                                Function = alsc.Function,
                                Pollutant = new BenMAPPollutant() { PollutantName = alsc.Pollutant },
                                Metric = new Metric() { MetricName = alsc.Metric },
                                SeasonalMetric = new SeasonalMetric() { SeasonalMetricName = alsc.SeasonalMetric },
                                MetricStatistic = MetricStatic.Mean,
                                DataSetName = alsc.DataSet
                            };

                            var Parent = alsc;

                            //YY: re-calculate population
                            if (calculatePop 
                                && alsc.CRSelectFunctionCalculateValue != null 
                                && alsc.CRSelectFunctionCalculateValue.CRCalculateValues != null
                                && CommonClass.IncidencePoolingAndAggregationAdvance.CalculatePooledPopulationYN)
                            {
                                List<AllSelectCRFunction> lstChildCR = new List<AllSelectCRFunction>();
                                getAllChildCRFunctions(alsc, lstAllSelectCRFunctionAll, ref lstChildCR);
                                int gridDefinitionID = CommonClass.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID;
                                CalculatePooledPopulation(alsc, lstChildCR, CommonClass.BenMAPPopulation, gridDefinitionID);
                            }
                            
                            

                        }
                        else
                        {
                            alsc.CRID = -1;
                            alsc.CRSelectFunctionCalculateValue = null;
                        }
                    }
                }

                
                

                }
            catch (Exception ex)
            {
            }

        }

        private static void CalculatePooledPopulation(AllSelectCRFunction ascrParent, List<AllSelectCRFunction> lstChildCR, BenMAPPopulation benMAPPopulation, int gridDefinitionID)
        {
            //calculate population for each ascrParent.CRSelectFunctionCalculateValue.CRCalculateValues from benMAPPopulation dataset
            //use lstChildCR to list discrete population groups
            //gridDefinitionID is the grid definition ID of the pooling
            try
            {
                List<PopulationGroup> lstPopGroupOrigin = new List<PopulationGroup>(); // list of unique population groups from selected functions
                List<PopulationGroup> lstPopGroupTmp1 = new List<PopulationGroup>(); // 
                List<PopulationGroup> lstPopGroupTmp2 = new List<PopulationGroup>();
                List<Int32> lstAge = new List<int>();

                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = "";

                int popDatasetID = benMAPPopulation.DataSetID;

                //Prepare lstPopGroupOrigin
                foreach (AllSelectCRFunction ascr in lstChildCR)
                {
                    int startAge = Convert.ToInt32(ascr.StartAge);
                    int endAge = Convert.ToInt32(ascr.EndAge);

                    if (!lstPopGroupOrigin.Any(x => x.StartAge == startAge
                    && x.EndAge == endAge
                    && x.Race == (ascr.Race == "" ? "ALL" : ascr.Race)
                    && x.Ethnicity == (ascr.Ethnicity == "" ? "ALL" : ascr.Ethnicity)
                    && x.Gender == (ascr.Gender == "" ? "ALL" : ascr.Gender)))
                    {
                        lstPopGroupOrigin.Add(new PopulationGroup
                        {
                            StartAge = startAge,
                            EndAge = endAge,
                            Race = (ascr.Race == "" ? "ALL" : ascr.Race),
                            Ethnicity = (ascr.Ethnicity == "" ? "ALL" : ascr.Ethnicity),
                            Gender = (ascr.Gender == "" ? "ALL" : ascr.Gender),
                        });

                        if (!lstAge.Contains(startAge))
                        {
                            lstAge.Add(startAge);
                        }
                        if (!lstAge.Contains(endAge))
                        {
                            lstAge.Add(endAge);
                        }
                    }
                }

                //if there is only one unique population group, we can directly use the population from each child function result values
                if (lstPopGroupOrigin.Count() == 1)
                {
                    return;
                }

                lstAge.Sort();

                //expend age ranges to be discrete age ranges
                for (int i = 0; i<lstAge.Count-1; i++)
                {
                    int startAge = 0;
                    if (i == 0)
                    {
                        startAge = lstAge[i];
                    }
                    else
                    {
                        startAge = lstAge[i] + 1;
                    }
                    int endAge = lstAge[i + 1];

                    foreach (PopulationGroup popGroup in lstPopGroupOrigin)
                    {
                        if (popGroup.StartAge <= startAge && popGroup.EndAge >= endAge)
                        {
                            lstPopGroupTmp1.Add(new PopulationGroup
                            {
                                StartAge = startAge,
                                EndAge = endAge,
                                Race = popGroup.Race,
                                Ethnicity = popGroup.Ethnicity,
                                Gender = popGroup.Gender,
                            });
                        }
                    }
                }
                lstPopGroupTmp2 = new List<PopulationGroup>() ;
                foreach (PopulationGroup popGroup in lstPopGroupTmp1)
                {
                    if (!lstPopGroupTmp2.Any(x => x.StartAge <= popGroup.StartAge
                    && x.EndAge >= popGroup.EndAge
                    && (x.Race == popGroup.Race || x.Race == "ALL")
                    && (x.Ethnicity == popGroup.Ethnicity || x.Ethnicity == "ALL")
                    && (x.Gender == popGroup.Gender || x.Gender == "ALL")))
                    {
                        lstPopGroupTmp2.Add(new PopulationGroup
                        {
                            StartAge = popGroup.StartAge,
                            EndAge = popGroup.EndAge,
                            Race = popGroup.Race,
                            Ethnicity = popGroup.Ethnicity,
                            Gender = popGroup.Gender,
                        });
                    }
                }
                lstPopGroupTmp1 = new List<PopulationGroup>();

                //Check and expand races,  if race values overlaps (contain both all and non-all)
                int countRace = lstPopGroupTmp2.Where(x => x.Race.ToLower() == "all" || x.Race == "").Count();
                if (countRace> 0 && countRace < lstPopGroupTmp2.Count())
                {
                    foreach (PopulationGroup popGroup in lstPopGroupTmp2)
                    {
                        if (popGroup.Race.ToLower() == "all" || popGroup.Race == "")
                        {
                            commandText = string.Format(@"SELECT distinct R.RACENAME from 
POPULATIONENTRIES pop INNER JOIN RACES r ON pop.RACEID = r.RACEID
WHERE pop.POPULATIONDATASETID = {0}",popDatasetID);
                            fb = new ESIL.DBUtility.ESILFireBirdHelper();
                            DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                lstPopGroupTmp1.Add(new PopulationGroup
                                {
                                    StartAge = popGroup.StartAge,
                                    EndAge = popGroup.EndAge,
                                    Race = dr["RaceName"].ToString(),
                                    Ethnicity = popGroup.Ethnicity,
                                    Gender = popGroup.Gender,
                                });
                            }
                        }
                        else
                        {
                            lstPopGroupTmp1.Add(new PopulationGroup
                            {
                                StartAge = popGroup.StartAge,
                                EndAge = popGroup.EndAge,
                                Race = popGroup.Race,
                                Ethnicity = popGroup.Ethnicity,
                                Gender = popGroup.Gender,
                            });
                        }
                    }

                    lstPopGroupTmp2 = lstPopGroupTmp1;
                    lstPopGroupTmp1 = new List<PopulationGroup>();
                }
                else
                {
                    // no need to update tmp1 and tmp2
                }

                //Check and expand gender,  if gender values overlaps (contain both all and non-all)
                int countGender = lstPopGroupTmp2.Where(x => x.Gender.ToLower() == "all" || x.Gender == "").Count();
                if (countGender > 0 && countGender < lstPopGroupTmp2.Count())
                {
                    foreach (PopulationGroup popGroup in lstPopGroupTmp2)
                    {
                        if (popGroup.Gender.ToLower() == "all" || popGroup.Gender == "")
                        {
                            commandText = string.Format(@"SELECT distinct g.GenderName from 
POPULATIONENTRIES pop INNER JOIN genders g ON pop.GENDERID = g.GENDERID
WHERE pop.POPULATIONDATASETID = {0}", popDatasetID);
                            fb = new ESIL.DBUtility.ESILFireBirdHelper();
                            DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                lstPopGroupTmp1.Add(new PopulationGroup
                                {
                                    StartAge = popGroup.StartAge,
                                    EndAge = popGroup.EndAge,
                                    Race = popGroup.Race,
                                    Ethnicity = popGroup.Ethnicity,
                                    Gender = dr["GenderName"].ToString(),
                                });
                            }
                        }
                        else
                        {
                            lstPopGroupTmp1.Add(new PopulationGroup
                            {
                                StartAge = popGroup.StartAge,
                                EndAge = popGroup.EndAge,
                                Race = popGroup.Race,
                                Ethnicity = popGroup.Ethnicity,
                                Gender = popGroup.Gender,
                            });
                        }
                    }

                    lstPopGroupTmp2 = lstPopGroupTmp1;
                    lstPopGroupTmp1 = new List<PopulationGroup>();
                }
                else
                {
                    // no need to update tmp1 and tmp2
                }

                //Check and expand ethnicity,  if ethnicity values overlaps (contain both all and non-all)
                int countEthnicity = lstPopGroupTmp2.Where(x => x.Ethnicity.ToLower() == "all" || x.Ethnicity == "").Count();
                if (countEthnicity > 0 && countEthnicity < lstPopGroupTmp2.Count())
                {
                    foreach (PopulationGroup popGroup in lstPopGroupTmp2)
                    {
                        if (popGroup.Ethnicity.ToLower() == "all" || popGroup.Ethnicity == "")
                        {
                            commandText = "select distinct EthnicityName from Ethnicity where upper(EthnicityName) <>'ALL' and EthnicityName<>''";
                            commandText = string.Format(@"SELECT distinct e.EthnicityName from 
POPULATIONENTRIES pop INNER JOIN Ethnicity e ON pop.ETHNICITYID = e.ETHNICITYID
WHERE pop.POPULATIONDATASETID = {0}", popDatasetID);
                            fb = new ESIL.DBUtility.ESILFireBirdHelper();
                            DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                lstPopGroupTmp1.Add(new PopulationGroup
                                {
                                    StartAge = popGroup.StartAge,
                                    EndAge = popGroup.EndAge,
                                    Race = popGroup.Race,
                                    Ethnicity = dr["EthnicityName"].ToString(),
                                    Gender = popGroup.Gender,
                                });
                            }
                        }
                        else
                        {
                            lstPopGroupTmp1.Add(new PopulationGroup
                            {
                                StartAge = popGroup.StartAge,
                                EndAge = popGroup.EndAge,
                                Race = popGroup.Race,
                                Ethnicity = popGroup.Ethnicity,
                                Gender = popGroup.Gender,
                            });
                        }
                    }

                    lstPopGroupTmp2 = new List<PopulationGroup>();
                    foreach (PopulationGroup popGroup in lstPopGroupTmp1)
                    {
                        if (!lstPopGroupTmp2.Any(x => x.StartAge <= popGroup.StartAge
                        && x.EndAge >= popGroup.EndAge
                        && x.Race == popGroup.Race
                        && x.Ethnicity == popGroup.Ethnicity
                        && x.Gender == popGroup.Gender))
                        {
                            lstPopGroupTmp2.Add(new PopulationGroup
                            {
                                StartAge = popGroup.StartAge,
                                EndAge = popGroup.EndAge,
                                Race = popGroup.Race,
                                Ethnicity = popGroup.Ethnicity,
                                Gender = popGroup.Gender,
                            });
                        }
                    }
                    lstPopGroupTmp1 = new List<PopulationGroup>();
                }
                else
                {
                    // no need to update tmp1 and tmp2
                }

                //start calculating population for grid cell of pooled result
                
                int popYear = benMAPPopulation.Year;
                commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", benMAPPopulation.DataSetID);
                popYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
                //YY: How to handle when popYear <> benMAPPopulation.Year?
                commandText = string.Format(@"SELECT PERCENTAGEID FROM GRIDDEFINITIONPERCENTAGES p
INNER JOIN POPULATIONDATASETS pd
ON p.SOURCEGRIDDEFINITIONID = pd.GRIDDEFINITIONID
WHERE p.TARGETGRIDDEFINITIONID = {0}
AND pd.POPULATIONDATASETID = {1}", gridDefinitionID, popDatasetID);
                int percentageID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
                commandText = string.Format("SELECT POPULATIONCONFIGURATIONID FROM POPULATIONDATASETS WHERE POPULATIONDATASETID = {0} ", benMAPPopulation.DataSetID);
                int popConfigurationID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));

                foreach (CRCalculateValue crvp in ascrParent.CRSelectFunctionCalculateValue.CRCalculateValues)
                {
                    int col = crvp.Col;
                    int row = crvp.Row;
                    double popTmp = 0; //crvp.Population;

                    foreach (PopulationGroup popGroup in lstPopGroupTmp2)
                    {
                        string gender = popGroup.Gender == "" ? "ALL" : popGroup.Gender;
                        string race = popGroup.Race == "" ? "ALL" : popGroup.Race;
                        string ethnicity = popGroup.Ethnicity == "" ? "ALL" : popGroup.Ethnicity;
                        int startAge = popGroup.StartAge;
                        int endAge = popGroup.EndAge;

                        commandText = string.Format(@"SELECT sum( iif(cast(a.STARTAGE as float) > {5} OR cast(a.ENDAGE as float) < {4} ,0
    ,iif(cast(a.STARTAGE as float) >={4} AND cast(a.ENDAGE as float)<={5},1
    ,iif({4} <=cast(a.ENDAGE as float) AND {5}>=cast(a.ENDAGE as float), (cast(a.ENDAGE as float) - {4} + 1)/(cast(a.ENDAGE as float)-cast(a.STARTAGE as float) + 1)
    ,({5} - cast(a.STARTAGE as float) + 1)/(cast(a.ENDAGE as float)-cast(a.STARTAGE as float) + 1)))) * pop.VVALUE * pct.PERCENTAGE) as newVValue

FROM POPULATIONENTRIES pop
INNER JOIN GENDERS g ON pop.GENDERID = g.GENDERID
INNER JOIN RACES r ON pop.RACEID = r.RACEID
INNER JOIN ETHNICITY e ON pop.ETHNICITYID = e.ETHNICITYID
INNER JOIN AGERANGES a ON pop.AGERANGEID = a.AGERANGEID
INNER JOIN GRIDDEFINITIONPERCENTAGEENTRIES pct ON pop.CCOLUMN = pct.SOURCECOLUMN AND pop.ROW = pct.SOURCEROW

WHERE 
pop.POPULATIONDATASETID = {0}
AND pop.YYEAR = {1} 
AND pct.TARGETCOLUMN = {2}
AND pct.TARGETROW= {3}
AND pct.PERCENTAGEID = {6} 
AND a.POPULATIONCONFIGURATIONID = {7}", popDatasetID, popYear, col, row, startAge, endAge, percentageID, popConfigurationID);

                        if (race != "ALL")
                        {
                            commandText += String.Format(" AND r.RACENAME='{0}'",race);
                        }
                        if (gender != "ALL")
                        {
                            commandText += String.Format(" AND g.GENDERNAME='{0}'", gender);
                        }
                        if (ethnicity != "ALL")
                        {
                            commandText += String.Format(" AND e.ETHNICITYNAME='{0}'", ethnicity);
                        }
                        object obj = fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText);
                        popTmp += Convert.ToDouble(obj.ToString()== "" ? 0:obj);
                    }
                    crvp.Population = Convert.ToSingle(popTmp);

                }
               
            }
            catch(Exception ex)
            {
            }

        }

        public static List<float> getMedianSample(List<float> listInput, int Points)
        {
            listInput.Sort();
            if (listInput.Count() == Points)
                return listInput;
            List<float> lstOut = new List<float>();
            for (int i = 0; i < Points; i++)
            {
                lstOut.Add(listInput.GetRange(i * (listInput.Count / Points), (listInput.Count / Points)).Median());
            }
            return lstOut;
        }
        public static List<double> getMedianSample(List<double> listInput, int Points)
        {
            listInput.Sort();
            if (listInput.Count() == Points)
                return listInput;
            List<double> lstOut = new List<double>();

            for (int i = 0; i < Points; i++)
            {
                lstOut.Add(listInput.GetRange(i * (listInput.Count / Points), (listInput.Count / Points)).Median());
            }
            return lstOut;
        }
        public static double getChidist(Dictionary<int, Dictionary<string, List<float>>> dicall, ref List<double> lstWeight)
        {

            double dChidist = 0;

            double dSum = 0, dRandom = 0;
            int idSum = 0;
            Dictionary<int, List<float>> dicAggregation = new Dictionary<int, List<float>>();
            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
            {
                List<float> lst = new List<float>();
                foreach (float f in k.Value.First().Value)
                {
                    lst.Add(0);
                }
                foreach (List<float> lstF in k.Value.Values)
                {
                    for (int ilstF = 0; ilstF < lstF.Count; ilstF++)
                    {
                        lst[ilstF] += lstF[ilstF];
                    }
                }
                dicAggregation.Add(k.Key, lst);

            }
            double sumVarianceA = 0;
            double dPointElementA = 0;
            foreach (KeyValuePair<int, List<float>> k in dicAggregation)
            {


                if (Configuration.ConfigurationCommonClass.getVariance(k.Value.GetRange(1, k.Value.Count - 1), k.Value[0]) != 0)
                    sumVarianceA += 1.00000 / Configuration.ConfigurationCommonClass.getVariance(k.Value.GetRange(1, k.Value.Count - 1), k.Value[0]);

            }
            lstWeight = new List<double>();

            foreach (KeyValuePair<int, List<float>> k in dicAggregation)
            {
                if (sumVarianceA != 0 && Configuration.ConfigurationCommonClass.getVariance(k.Value.GetRange(1, k.Value.Count - 1), k.Value[0]) != 0)
                    lstWeight.Add((1.00000 / Configuration.ConfigurationCommonClass.getVariance(k.Value.GetRange(1, k.Value.Count - 1), k.Value[0])) / sumVarianceA);
                else
                    lstWeight.Add(0);

            }
            idSum = 0;

            foreach (KeyValuePair<int, List<float>> k in dicAggregation)
            {
                dPointElementA += k.Value[0] * lstWeight[idSum];
                idSum++;
            }
            foreach (KeyValuePair<int, List<float>> k in dicAggregation)
            {


                dSum += (1 / Configuration.ConfigurationCommonClass.getVariance(k.Value.GetRange(1, k.Value.Count - 1), k.Value[0])) * Math.Pow((dPointElementA - k.Value[0]), 2);


            }

            idSum = 0;
            try
            {
                dChidist = 0;
                Meta.Numerics.Statistics.Distributions.ChiSquaredDistribution chi = new Meta.Numerics.Statistics.Distributions.ChiSquaredDistribution(lstWeight.Count - 1);
                dChidist = chi.RightProbability(dSum);

            }
            catch
            {
                dChidist = 1;
            }

            return dChidist;


        }
        public static CRSelectFunctionCalculateValue getPoolingMethodCRSelectFunctionCalculateValue(List<CRSelectFunctionCalculateValue> lstCRSelectFunctionCalculateValue, PoolingMethodTypeEnum poolingMethod, ref List<double> lstWeight)
        {
            try
            {

                CRSelectFunctionCalculateValue crv = new CRSelectFunctionCalculateValue();
                crv.CRCalculateValues = new List<CRCalculateValue>();
                Dictionary<int, Dictionary<string, List<float>>> dicall = getAllCrCalculateValues(lstCRSelectFunctionCalculateValue);
                Dictionary<int, Dictionary<string, CRCalculateValue>> dicAllCR = new Dictionary<int, Dictionary<string, CRCalculateValue>>();
                int iDicAllCR = 0;
                foreach (CRSelectFunctionCalculateValue crfcv in lstCRSelectFunctionCalculateValue)
                {
                    Dictionary<string, CRCalculateValue> dicValues = new Dictionary<string, CRCalculateValue>();
                    List<float> dArray = new List<float>();
                    foreach (CRCalculateValue crcv in crfcv.CRCalculateValues)
                    {
                        if (!dicValues.ContainsKey(crcv.Col + "," + crcv.Row))
                            dicValues.Add(crcv.Col + "," + crcv.Row, crcv);
                    }

                    dicAllCR.Add(iDicAllCR, dicValues);
                    iDicAllCR++;
                }

                List<string> lstKey = new List<string>();
                Dictionary<string, List<float>> DicIE = new Dictionary<string, List<float>>();

                foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
                {
                    DicIE.Union(k.Value);
                }
                List<string> ie = DicIE.Keys.Distinct().ToList();
                foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
                {
                    ie.AddRange(k.Value.Keys.ToList());
                }
                ie = ie.Distinct().ToList();
                double dChidist = 0;
                List<double> lstRandomWeight = new List<double>();
                if (poolingMethod == PoolingMethodTypeEnum.RandomOrFixedEffects)
                {
                    dChidist = getChidist(dicall, ref lstRandomWeight);
                }



                string[] sin = null;
                CRCalculateValue crCalculateValue = new CRCalculateValue();
                int i = 0;
                int j = 0;
                double dlatin = 0.0;
                List<List<float>> lstPercentile = new List<List<float>>(); List<float> lstPooling = new List<float>();
                Random randomPooling = new Random();

                switch (poolingMethod)
                {

                    case PoolingMethodTypeEnum.SumDependent:
                        foreach (string s in ie)
                        {
                            crCalculateValue = new CRCalculateValue();
                            sin = s.Split(new char[] { ',' });
                            crCalculateValue.Col = Convert.ToInt32(sin[0]);
                            crCalculateValue.Row = Convert.ToInt32(sin[1]);
                            crCalculateValue.PointEstimate = 0;
                            iDicAllCR = 0;
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
                            {

                                try
                                {
                                    if (k.Value.ContainsKey(s))
                                    {
                                        crCalculateValue.PointEstimate += k.Value[s][0];
                                        crCalculateValue.Baseline += dicAllCR[iDicAllCR][s].Baseline;
                                        crCalculateValue.Population += dicAllCR[iDicAllCR][s].Population;
                                        crCalculateValue.Incidence += dicAllCR[iDicAllCR][s].Incidence;
                                        crCalculateValue.Delta += dicAllCR[iDicAllCR][s].Delta;
                                    }

                                    if ((crCalculateValue.LstPercentile == null || crCalculateValue.LstPercentile.Count == 0) && (k.Value.ContainsKey(s)))
                                    {
                                        crCalculateValue.LstPercentile = k.Value[s];
                                        crCalculateValue.LstPercentile.RemoveAt(0);
                                    }
                                    else if (k.Value.ContainsKey(s))
                                    {
                                        i = 0;
                                        while (i < crCalculateValue.LstPercentile.Count)
                                        {
                                            crCalculateValue.LstPercentile[i] += k.Value[s][i + 1];
                                            i++;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                                iDicAllCR++;
                            }
                            crCalculateValue.Delta = crCalculateValue.Delta / dicAllCR.Count;
                            crCalculateValue.Population = crCalculateValue.Population / dicAllCR.Count;
                            if (crCalculateValue.LstPercentile != null && crCalculateValue.LstPercentile.Count > 0)
                            {
                                crCalculateValue.Mean = Configuration.ConfigurationCommonClass.getMean(crCalculateValue.LstPercentile);
                                crCalculateValue.Variance = Configuration.ConfigurationCommonClass.getVariance(crCalculateValue.LstPercentile, crCalculateValue.PointEstimate);
                                crCalculateValue.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(crCalculateValue.LstPercentile, crCalculateValue.PointEstimate);
                                crCalculateValue.PercentOfBaseline = crCalculateValue.Baseline == 0 ? 0 : Convert.ToSingle((crCalculateValue.Mean / crCalculateValue.Baseline) * 100);
                            }
                            crv.CRCalculateValues.Add(crCalculateValue);

                        }
                        break;

                    case PoolingMethodTypeEnum.SumIndependent:
                        foreach (string s in ie)
                        {
                            crCalculateValue = new CRCalculateValue();
                            sin = s.Split(new char[] { ',' });
                            crCalculateValue.Col = Convert.ToInt32(sin[0]);
                            crCalculateValue.Row = Convert.ToInt32(sin[1]);
                            crCalculateValue.PointEstimate = 0;
                            lstPercentile = new List<List<float>>();
                            iDicAllCR = 0;
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
                            {

                                try
                                {
                                    if (k.Value.ContainsKey(s))
                                    {
                                        crCalculateValue.PointEstimate += k.Value[s][0];
                                        crCalculateValue.Baseline += dicAllCR[iDicAllCR][s].Baseline;
                                        crCalculateValue.Population += dicAllCR[iDicAllCR][s].Population;
                                        crCalculateValue.Incidence += dicAllCR[iDicAllCR][s].Incidence;
                                        crCalculateValue.Delta += dicAllCR[iDicAllCR][s].Delta;
                                    }

                                    if ((crCalculateValue.LstPercentile == null || crCalculateValue.LstPercentile.Count == 0) && k.Value.ContainsKey(s))
                                    {
                                        lstPercentile.Add(k.Value[s].GetRange(1, CommonClass.CRLatinHypercubePoints));
                                    }
                                }
                                catch
                                {
                                }
                                iDicAllCR++;
                            }
                            lstPooling = new List<float>();

                            for (int iPer = 0; iPer < 5000; iPer++)
                            {
                                float d = lstPercentile[0][randomPooling.Next(0, CommonClass.CRLatinHypercubePoints - 1)];
                                for (int iPerSec = 1; iPerSec < lstPercentile.Count; iPerSec++)
                                {
                                    d = d + lstPercentile[iPerSec][randomPooling.Next(0, CommonClass.CRLatinHypercubePoints - 1)];
                                }
                                lstPooling.Add(d);

                            }
                            crCalculateValue.LstPercentile = getMedianSample(lstPooling, CommonClass.CRLatinHypercubePoints);
                            crCalculateValue.Delta = crCalculateValue.Delta / dicAllCR.Count;
                            crCalculateValue.Population = crCalculateValue.Population / dicAllCR.Count;
                            if (crCalculateValue.LstPercentile != null && crCalculateValue.LstPercentile.Count > 0)
                            {
                                crCalculateValue.Mean = Configuration.ConfigurationCommonClass.getMean(crCalculateValue.LstPercentile);
                                crCalculateValue.Variance = Configuration.ConfigurationCommonClass.getVariance(crCalculateValue.LstPercentile, crCalculateValue.PointEstimate);
                                crCalculateValue.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(crCalculateValue.LstPercentile, crCalculateValue.PointEstimate);
                                crCalculateValue.PercentOfBaseline = crCalculateValue.Baseline == 0 ? 0 : Convert.ToSingle((crCalculateValue.Mean / crCalculateValue.Baseline) * 100);
                            }
                            crv.CRCalculateValues.Add(crCalculateValue);

                        }
                        break;
                    case PoolingMethodTypeEnum.SubtractionDependent:
                        int iSubtractionDependent = 0;
                        foreach (string s in ie)
                        {
                            crCalculateValue = new CRCalculateValue();
                            sin = s.Split(new char[] { ',' });
                            crCalculateValue.Col = Convert.ToInt32(sin[0]);
                            crCalculateValue.Row = Convert.ToInt32(sin[1]);
                            crCalculateValue.PointEstimate = 0;

                            iSubtractionDependent = 0;
                            iDicAllCR = 0;
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
                            {

                                try
                                {
                                    if (iSubtractionDependent == 0)
                                    {
                                        if (k.Value.ContainsKey(s))
                                        {
                                            crCalculateValue.PointEstimate = k.Value[s][0];
                                        }
                                        else
                                            crCalculateValue.PointEstimate = 0;
                                    }
                                    else if (k.Value.ContainsKey(s))
                                    {
                                        crCalculateValue.PointEstimate -= k.Value[s][0];
                                        crCalculateValue.Baseline -= dicAllCR[iDicAllCR][s].Baseline;
                                        crCalculateValue.Population += dicAllCR[iDicAllCR][s].Population;
                                        crCalculateValue.Incidence -= dicAllCR[iDicAllCR][s].Incidence;
                                        crCalculateValue.Delta += dicAllCR[iDicAllCR][s].Delta;
                                    }
                                    iSubtractionDependent++;
                                    if (crCalculateValue.LstPercentile == null && (k.Value.ContainsKey(s)))
                                    {

                                        crCalculateValue.LstPercentile = k.Value[s];
                                        crCalculateValue.LstPercentile.RemoveAt(0);
                                    }
                                    else if (k.Value.ContainsKey(s))
                                    {
                                        i = 0;
                                        while (i < crCalculateValue.LstPercentile.Count)
                                        {
                                            crCalculateValue.LstPercentile[i] -= k.Value[s][i + 1];
                                            i++;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                                iDicAllCR++;
                            }
                            crCalculateValue.Delta = crCalculateValue.Delta / dicAllCR.Count;
                            crCalculateValue.Population = crCalculateValue.Population / dicAllCR.Count;
                            if (crCalculateValue.LstPercentile != null && crCalculateValue.LstPercentile.Count > 0)
                            {
                                crCalculateValue.Mean = Configuration.ConfigurationCommonClass.getMean(crCalculateValue.LstPercentile);
                                crCalculateValue.Variance = Configuration.ConfigurationCommonClass.getVariance(crCalculateValue.LstPercentile, crCalculateValue.PointEstimate);
                                crCalculateValue.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(crCalculateValue.LstPercentile, crCalculateValue.PointEstimate);
                                crCalculateValue.PercentOfBaseline = crCalculateValue.Baseline == 0 ? 0 : Convert.ToSingle((crCalculateValue.Mean / crCalculateValue.Baseline) * 100);
                            }
                            crv.CRCalculateValues.Add(crCalculateValue);


                        }
                        break;
                    case PoolingMethodTypeEnum.SubtractionIndependent:
                        int iSubtractionIndependent = 0;
                        foreach (string s in ie)
                        {
                            crCalculateValue = new CRCalculateValue();
                            sin = s.Split(new char[] { ',' });
                            crCalculateValue.Col = Convert.ToInt32(sin[0]);
                            crCalculateValue.Row = Convert.ToInt32(sin[1]);
                            crCalculateValue.PointEstimate = 0;

                            iSubtractionIndependent = 0;
                            lstPercentile = new List<List<float>>();
                            iDicAllCR = 0;
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
                            {

                                if (iSubtractionIndependent == 0)
                                {
                                    if (k.Value.ContainsKey(s))
                                    {
                                        crCalculateValue.PointEstimate = k.Value[s][0];
                                    }
                                    else
                                        crCalculateValue.PointEstimate = 0;
                                }
                                else if (k.Value.ContainsKey(s))
                                {
                                    crCalculateValue.PointEstimate -= k.Value[s][0];
                                    crCalculateValue.Baseline -= dicAllCR[iDicAllCR][s].Baseline;
                                    crCalculateValue.Population += dicAllCR[iDicAllCR][s].Population;
                                    crCalculateValue.Incidence -= dicAllCR[iDicAllCR][s].Incidence;
                                    crCalculateValue.Delta += dicAllCR[iDicAllCR][s].Delta;
                                }

                                iSubtractionIndependent++;
                                if (k.Value.ContainsKey(s))
                                {
                                    try
                                    {
                                        lstPercentile.Add(k.Value[s].GetRange(1, CommonClass.CRLatinHypercubePoints));


                                    }
                                    catch
                                    {
                                    }
                                }
                                iDicAllCR++;
                            }
                            lstPooling = new List<float>();

                            for (int iPer = 0; iPer < 5000; iPer++)
                            {
                                float d = lstPercentile[0][randomPooling.Next(0, CommonClass.CRLatinHypercubePoints - 1)];
                                for (int iPerSec = 1; iPerSec < lstPercentile.Count; iPerSec++)
                                {
                                    d = d - lstPercentile[iPerSec][randomPooling.Next(0, CommonClass.CRLatinHypercubePoints - 1)];
                                }
                                lstPooling.Add(d);

                            }
                            crCalculateValue.LstPercentile = getMedianSample(lstPooling, CommonClass.CRLatinHypercubePoints);
                            crCalculateValue.Delta = crCalculateValue.Delta / dicAllCR.Count;
                            crCalculateValue.Population = crCalculateValue.Population / dicAllCR.Count;
                            if (crCalculateValue.LstPercentile != null && crCalculateValue.LstPercentile.Count > 0)
                            {
                                crCalculateValue.Mean = Configuration.ConfigurationCommonClass.getMean(crCalculateValue.LstPercentile);
                                crCalculateValue.Variance = Configuration.ConfigurationCommonClass.getVariance(crCalculateValue.LstPercentile, crCalculateValue.PointEstimate);
                                crCalculateValue.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(crCalculateValue.LstPercentile, crCalculateValue.PointEstimate);
                                crCalculateValue.PercentOfBaseline = crCalculateValue.Baseline == 0 ? 0 : Convert.ToSingle((crCalculateValue.Mean / crCalculateValue.Baseline) * 100);
                            }
                            crv.CRCalculateValues.Add(crCalculateValue);


                        }
                        break;
                    case PoolingMethodTypeEnum.SubjectiveWeights:
                        if (lstWeight != null && lstWeight.Count > 0 && lstWeight.Sum() != 1)
                        {
                            double dWeightSum = lstWeight.Sum();
                            for (int iWeightSum = 0; iWeightSum < lstWeight.Count; iWeightSum++)
                            {
                                lstWeight[iWeightSum] = lstWeight[iWeightSum] / dWeightSum;
                            }
                        }
                        foreach (string s in ie)
                        {
                            crCalculateValue = new CRCalculateValue();
                            sin = s.Split(new char[] { ',' });
                            crCalculateValue.Col = Convert.ToInt32(sin[0]);
                            crCalculateValue.Row = Convert.ToInt32(sin[1]);
                            crCalculateValue.PointEstimate = 0;

                            j = 0;
                            lstPooling = new List<float>();
                            iDicAllCR = 0;
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
                            {

                                if (k.Value.ContainsKey(s))
                                {
                                    try
                                    {
                                        crCalculateValue.PointEstimate += Convert.ToSingle(k.Value[s][0] * lstWeight[j]);
                                        crCalculateValue.Baseline += Convert.ToSingle(dicAllCR[iDicAllCR][s].Baseline * lstWeight[j]);
                                        crCalculateValue.Population += Convert.ToSingle(dicAllCR[iDicAllCR][s].Population * lstWeight[j]);
                                        crCalculateValue.Incidence += Convert.ToSingle(dicAllCR[iDicAllCR][s].Incidence * lstWeight[j]);
                                        crCalculateValue.Delta += Convert.ToSingle(dicAllCR[iDicAllCR][s].Delta * lstWeight[j]);
                                        if (crCalculateValue.LstPercentile == null)
                                        {
                                            for (int iPer = 0; iPer < Convert.ToInt32(Math.Round(lstWeight[j] * 100)); iPer++)
                                            {
                                                lstPooling.AddRange(k.Value[s].GetRange(1, CommonClass.CRLatinHypercubePoints));
                                            }

                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                                iDicAllCR++;
                                j++;

                            }
                            if (lstPooling.Count > 0)
                            {
                                crCalculateValue.LstPercentile = getMedianSample(lstPooling, CommonClass.CRLatinHypercubePoints);
                            }
                            else
                            {
                                crCalculateValue.LstPercentile = new List<float>();
                                for (int iPer = 0; iPer < CommonClass.CRLatinHypercubePoints; iPer++)
                                {
                                    crCalculateValue.LstPercentile.Add(0);
                                }
                            }
                            if (crCalculateValue.LstPercentile != null && crCalculateValue.LstPercentile.Count > 0)
                            {
                                crCalculateValue.Mean = Configuration.ConfigurationCommonClass.getMean(crCalculateValue.LstPercentile);
                                crCalculateValue.Variance = Configuration.ConfigurationCommonClass.getVariance(crCalculateValue.LstPercentile, crCalculateValue.PointEstimate);
                                crCalculateValue.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(crCalculateValue.LstPercentile, crCalculateValue.PointEstimate);
                                crCalculateValue.PercentOfBaseline = crCalculateValue.Baseline == 0 ? 0 : Convert.ToSingle((crCalculateValue.Mean / crCalculateValue.Baseline) * 100);
                            }
                            crv.CRCalculateValues.Add(crCalculateValue);

                        }
                        break;
                    case PoolingMethodTypeEnum.RandomOrFixedEffects:

                    case PoolingMethodTypeEnum.FixedEffects:
                        lstWeight = new List<double>();
                        foreach (string s in ie)
                        {
                            if (s == "16,71")
                            {

                            }
                            double sumVariance = 0;
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
                            {

                                if (k.Value.ContainsKey(s))
                                {
                                    if (Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]) != 0)
                                        sumVariance += 1.00000 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]);
                                }
                            }
                            lstWeight = new List<double>();
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
                            {
                                if (sumVariance != 0 && k.Value.ContainsKey(s) && Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]) != 0)
                                    lstWeight.Add((1.00000 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0])) / sumVariance);
                                else
                                {
                                    lstWeight.Add(1.0000 / Convert.ToDouble(dicall.Count));
                                }
                            }
                            for (int iw = 0; iw < lstWeight.Count; iw++)
                            {
                                if (double.IsNaN(lstWeight[iw]))
                                {
                                    lstWeight[iw] = 1.0000 / Convert.ToDouble(dicall.Count);

                                }
                            }
                            crCalculateValue = new CRCalculateValue();
                            sin = s.Split(new char[] { ',' });
                            crCalculateValue.Col = Convert.ToInt32(sin[0]);
                            crCalculateValue.Row = Convert.ToInt32(sin[1]);
                            crCalculateValue.PointEstimate = 0;

                            j = 0;
                            iDicAllCR = 0;
                            lstPooling = new List<float>();
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
                            {

                                if (k.Value.ContainsKey(s))
                                {
                                    try
                                    {
                                        crCalculateValue.PointEstimate += Convert.ToSingle(k.Value[s][0] * lstWeight[j]);
                                        crCalculateValue.Baseline += Convert.ToSingle(dicAllCR[iDicAllCR][s].Baseline * lstWeight[j]);
                                        crCalculateValue.Population += Convert.ToSingle(dicAllCR[iDicAllCR][s].Population * lstWeight[j]);
                                        crCalculateValue.Incidence += Convert.ToSingle(dicAllCR[iDicAllCR][s].Incidence * lstWeight[j]);
                                        crCalculateValue.Delta += Convert.ToSingle(dicAllCR[iDicAllCR][s].Delta * lstWeight[j]);

                                        if (crCalculateValue.LstPercentile == null)
                                        {
                                            for (int iPer = 0; iPer < Convert.ToInt32(Math.Round(lstWeight[j] * 100)); iPer++)
                                            {
                                                lstPooling.AddRange(k.Value[s].GetRange(1, CommonClass.CRLatinHypercubePoints));
                                            }

                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                                iDicAllCR++;
                                j++;

                            }
                            if (poolingMethod == PoolingMethodTypeEnum.RandomOrFixedEffects)
                            {
                                double dSum = 0, dRandom = 0;
                                int idSum = 0;


                                if (dChidist < 0.05)
                                {
                                    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
                                    {

                                        if (k.Value.ContainsKey(s))
                                        {
                                            dSum += (1 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0])) * Math.Pow((crCalculateValue.PointEstimate - k.Value[s][0]), 2);
                                        }
                                        idSum++;
                                    }
                                    idSum = 0;
                                    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
                                    {

                                        if (k.Value.ContainsKey(s))
                                        {
                                            dRandom += Math.Pow(1 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]), 2);

                                        }
                                        idSum++;
                                    }

                                    dRandom = (dSum - 1) / (sumVariance - dRandom / sumVariance);
                                    lstWeight.Clear();
                                    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
                                    {

                                        if (k.Value.ContainsKey(s))
                                        {
                                            lstWeight.Add(1 / (Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]) + dRandom));
                                        }
                                        else
                                        {
                                            lstWeight.Add(1.0000 / Convert.ToDouble(dicall.Count));
                                        }
                                    }
                                    dSum = lstWeight.Sum();
                                    for (int iw = 0; iw < lstWeight.Count; iw++)
                                    {
                                        lstWeight[iw] = Convert.ToDouble(lstWeight[iw]) / dSum;
                                    }
                                    for (int iw = 0; iw < lstWeight.Count; iw++)
                                    {
                                        if (double.IsNaN(lstWeight[iw]))
                                        {
                                            lstWeight[iw] = 1.0000 / Convert.ToDouble(dicall.Count);

                                        }
                                    }
                                    j = 0;
                                    crCalculateValue.PointEstimate = 0;
                                    crCalculateValue.Baseline = 0;
                                    crCalculateValue.Population = 0;
                                    crCalculateValue.Incidence = 0;
                                    crCalculateValue.Delta = 0;
                                    lstPooling = new List<float>();
                                    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
                                    {

                                        if (k.Value.ContainsKey(s))
                                        {
                                            try
                                            {
                                                crCalculateValue.PointEstimate += Convert.ToSingle(k.Value[s][0] * lstWeight[j]);

                                                crCalculateValue.Baseline += Convert.ToSingle(dicAllCR[j][s].Baseline * lstWeight[j]);
                                                crCalculateValue.Population += Convert.ToSingle(dicAllCR[j][s].Population * lstWeight[j]);
                                                crCalculateValue.Incidence += Convert.ToSingle(dicAllCR[j][s].Incidence * lstWeight[j]);
                                                crCalculateValue.Delta += Convert.ToSingle(dicAllCR[j][s].Delta * lstWeight[j]);
                                                if (crCalculateValue.LstPercentile == null)
                                                {
                                                    for (int iPer = 0; iPer < Convert.ToInt32(Math.Round(lstWeight[j] * 100)); iPer++)
                                                    {
                                                        lstPooling.AddRange(k.Value[s].GetRange(1, CommonClass.CRLatinHypercubePoints));
                                                    }

                                                }

                                            }
                                            catch
                                            {
                                            }
                                        }
                                        j++;

                                    }

                                }
                                else
                                {
                                    for (int iw = 0; iw < lstRandomWeight.Count; iw++)
                                    {

                                        if (double.IsNaN(lstRandomWeight[iw]))
                                        {
                                            lstRandomWeight[iw] = 1.0000 / Convert.ToDouble(dicall.Count);

                                        }
                                    }
                                    j = 0;
                                    crCalculateValue.PointEstimate = 0;
                                    crCalculateValue.Baseline = 0;
                                    crCalculateValue.Population = 0;
                                    crCalculateValue.Incidence = 0;
                                    crCalculateValue.Delta = 0;
                                    lstPooling = new List<float>();
                                    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
                                    {

                                        if (k.Value.ContainsKey(s))
                                        {
                                            try
                                            {
                                                crCalculateValue.PointEstimate += Convert.ToSingle(k.Value[s][0] * lstRandomWeight[j]);

                                                crCalculateValue.Baseline += Convert.ToSingle(dicAllCR[j][s].Baseline * lstRandomWeight[j]);
                                                crCalculateValue.Population += Convert.ToSingle(dicAllCR[j][s].Population * lstRandomWeight[j]);
                                                crCalculateValue.Incidence += Convert.ToSingle(dicAllCR[j][s].Incidence * lstRandomWeight[j]);
                                                crCalculateValue.Delta += Convert.ToSingle(dicAllCR[j][s].Delta * lstRandomWeight[j]);
                                                if (crCalculateValue.LstPercentile == null)
                                                {
                                                    for (int iPer = 0; iPer < Convert.ToInt32(Math.Round(lstRandomWeight[j] * 100)); iPer++)
                                                    {
                                                        lstPooling.AddRange(k.Value[s].GetRange(1, CommonClass.CRLatinHypercubePoints));
                                                    }

                                                }

                                            }
                                            catch
                                            {
                                            }
                                        }
                                        j++;

                                    }
                                }
                            }
                            if (lstPooling.Count > 0)
                            {
                                crCalculateValue.LstPercentile = getMedianSample(lstPooling, CommonClass.CRLatinHypercubePoints);
                            }
                            else
                            {
                                crCalculateValue.LstPercentile = new List<float>();
                                for (int iPer = 0; iPer < CommonClass.CRLatinHypercubePoints; iPer++)
                                {
                                    crCalculateValue.LstPercentile.Add(0);
                                }
                            }
                            if (crCalculateValue.LstPercentile != null && crCalculateValue.LstPercentile.Count > 0)
                            {
                                crCalculateValue.Mean = Configuration.ConfigurationCommonClass.getMean(crCalculateValue.LstPercentile);
                                crCalculateValue.Variance = Configuration.ConfigurationCommonClass.getVariance(crCalculateValue.LstPercentile, crCalculateValue.PointEstimate);
                                crCalculateValue.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(crCalculateValue.LstPercentile, crCalculateValue.PointEstimate);
                                crCalculateValue.PercentOfBaseline = crCalculateValue.Baseline == 0 ? 0 : Convert.ToSingle((crCalculateValue.Mean / crCalculateValue.Baseline) * 100);
                            }
                            crv.CRCalculateValues.Add(crCalculateValue);


                        }
                        break;
                }
                dicall.Clear();
                dicAllCR.Clear();
                GC.Collect();
                return crv;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static AllSelectValuationMethodAndValue getOneAllSelectValuationMethodCRSelectFunctionCalculateValue(CRSelectFunctionCalculateValue crSelectFunctionCalculateValue, ref AllSelectValuationMethod allSelectValuationMethod, double AllGoodsIndex, double MedicalCostIndex, double WageIndex, Dictionary<string, double> dicIncome)
        {
            try
            {
                AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = new AllSelectValuationMethodAndValue();
                allSelectValuationMethodAndValue.AllSelectValuationMethod = allSelectValuationMethod;
                List<Tuple<string,int>> lstSystemVariableName = Configuration.ConfigurationCommonClass.getAllSystemVariableNameList();
                List<SetupVariableJoinAllValues> lstSetupVariable = new List<SetupVariableJoinAllValues>();
                int iGridID = CommonClass.GBenMAPGrid.GridDefinitionID;
                if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null
                                       && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
                {
                    iGridID = CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID;
                }
                Configuration.ConfigurationCommonClass.getSetupVariableNameListFromDatabaseFunction(CommonClass.ValuationMethodPoolingAndAggregation.VariableDatasetID, iGridID, allSelectValuationMethod.Function, lstSystemVariableName, ref lstSetupVariable);
                Dictionary<string, Dictionary<string, float>> DicAll = new Dictionary<string, Dictionary<string, float>>();
                foreach (SetupVariableJoinAllValues sv in lstSetupVariable)
                {
                    Dictionary<string, float> dic = new Dictionary<string, float>();
                    dic = sv.lstValues.ToDictionary(p => p.Col + "," + p.Row, p => p.Value);


                    DicAll.Add(sv.SetupVariableName.ToLower(), dic);
                }
                List<GridRelationship> lstGridRelationshipAll = CommonClass.LstGridRelationshipAll;
                allSelectValuationMethodAndValue.lstAPVValueAttributes = new List<APVValueAttribute>();
                string sFunction = getFunctionStringFromDatabaseFunction(allSelectValuationMethod.Function);
                double dValuation = 0;
                double dv = 0;
                double income = 1;
                try
                {
                    if (dicIncome != null && dicIncome.ContainsKey(allSelectValuationMethod.BenMAPValuationFunction.EndPointGroup))
                        income = dicIncome[allSelectValuationMethod.BenMAPValuationFunction.EndPointGroup];
                    if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null && CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.EndpointGroups != null
                        && !CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.EndpointGroups.Contains(allSelectValuationMethod.BenMAPValuationFunction.EndPointGroup))
                    {
                        income = 1;
                    }
                }
                catch (Exception ex)
                { }
                int i = 0;
                int idicAllCount = DicAll.Count;
                allSelectValuationMethod.lstMonte = new List<LatinPoints>(); int iRandomSeed = Convert.ToInt32(DateTime.Now.Hour + "" + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond);
                if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
                CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.RandomSeed != "" &&
                CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.RandomSeed != "Random Integer" && CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.RandomSeed != null)
                    iRandomSeed = Convert.ToInt32(CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.RandomSeed);
                // calculates percentiles here
                allSelectValuationMethod.lstMonte.Add(new LatinPoints() { values = getLHSArrayValuationFunctionSeed(100, allSelectValuationMethod.BenMAPValuationFunction, iRandomSeed).ToList() });
                int iSeed = 0;
                Dictionary<string, double> dicVariable = new Dictionary<string, double>();
                foreach (CRCalculateValue crCalculateValue in crSelectFunctionCalculateValue.CRCalculateValues)
                {
                    APVValueAttribute apvValueAttribute = new APVValueAttribute();
                    if (crCalculateValue.PointEstimate != 0)
                    {
                        dicVariable.Clear();
                        i = 0;
                        while (i < idicAllCount)
                        {
                            dv = 0;

                            try
                            {
                                if (DicAll.ToArray()[i].Value.ContainsKey(crCalculateValue.Col + "," + crCalculateValue.Row))
                                    dv = DicAll.ToArray()[i].Value[crCalculateValue.Col + "," + crCalculateValue.Row];


                            }
                            catch
                            { }
                            dicVariable.Add(DicAll.Keys.ToArray()[i], dv);
                            i++;

                        }


                        apvValueAttribute.Col = crCalculateValue.Col;
                        apvValueAttribute.Row = crCalculateValue.Row;





                        dValuation = getValueFromValuationFunctionString(sFunction, allSelectValuationMethod.BenMAPValuationFunction.A,
                             allSelectValuationMethod.BenMAPValuationFunction.B, allSelectValuationMethod.BenMAPValuationFunction.C,
                             allSelectValuationMethod.BenMAPValuationFunction.D, AllGoodsIndex, MedicalCostIndex, WageIndex, 0, dicVariable);
                        if (dValuation == double.NaN) dValuation = 0.0;
                    }
                    apvValueAttribute.Col = crCalculateValue.Col;
                    apvValueAttribute.Row = crCalculateValue.Row;
                    apvValueAttribute.PointEstimate = Convert.ToSingle(income * crCalculateValue.PointEstimate * dValuation);
                    apvValueAttribute.LstPercentile = new List<float>();

                    if (crCalculateValue.LstPercentile != null && crCalculateValue.LstPercentile.Count > 0)
                    {

                        if (crCalculateValue.PointEstimate == 0)
                        {
                            for (int iapv = 0; iapv < 100; iapv++)
                            {
                                apvValueAttribute.LstPercentile.Add(0);
                            }
                        }
                        else
                        {

                            List<float> lstValue = new List<float>();
                            foreach (float d in crCalculateValue.LstPercentile)
                            {
                                foreach (double dMonte in allSelectValuationMethod.lstMonte[iSeed].values)
                                {
                                    if (allSelectValuationMethod.BenMAPValuationFunction.A == 0.0)
                                        lstValue.Add(Convert.ToSingle(apvValueAttribute.PointEstimate * d / (crCalculateValue.PointEstimate)));
                                    else
                                        lstValue.Add(Convert.ToSingle(apvValueAttribute.PointEstimate * d * dMonte / (crCalculateValue.PointEstimate * allSelectValuationMethod.BenMAPValuationFunction.A)));

                                }
                            }
                            lstValue.Sort();

                            apvValueAttribute.LstPercentile = getMedianSample(lstValue, 100);


                        }

                    }
                    if (apvValueAttribute.PointEstimate != 0)
                    {

                        apvValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(apvValueAttribute.LstPercentile);
                        apvValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(apvValueAttribute.LstPercentile, apvValueAttribute.PointEstimate);
                        apvValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(apvValueAttribute.LstPercentile, apvValueAttribute.PointEstimate);
                    }
                    if (allSelectValuationMethodAndValue.lstAPVValueAttributes == null) allSelectValuationMethodAndValue.lstAPVValueAttributes = new List<APVValueAttribute>();
                    allSelectValuationMethodAndValue.lstAPVValueAttributes.Add(apvValueAttribute);


                }



                DicAll = null;
                return allSelectValuationMethodAndValue;


            }
            catch
            {
                return null;
            }
        }
        public static AllSelectValuationMethodAndValue getPoolingLstAllSelectValuationMethodAndValue(List<AllSelectValuationMethodAndValue> lstAllSelectValuationMethodAndValue, PoolingMethodTypeEnum poolingMethod, List<double> lstWeight)
        {
            try
            {
                AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = new AllSelectValuationMethodAndValue();
                allSelectValuationMethodAndValue.lstAPVValueAttributes = new List<APVValueAttribute>();

                Dictionary<int, Dictionary<string, List<float>>> dicAll = new Dictionary<int, Dictionary<string, List<float>>>();
                Dictionary<string, List<float>> dicValue = new Dictionary<string, List<float>>();
                int i = 0;
                List<string> lstAllColRow = new List<string>();
                APVValueAttribute aValueAttribute = new APVValueAttribute();
                string[] strColRow;
                float dPoint = 0;

                while (i < lstAllSelectValuationMethodAndValue.Count)
                {
                    dicValue = new Dictionary<string, List<float>>();

                    foreach (APVValueAttribute apvValueAttribute in lstAllSelectValuationMethodAndValue[i].lstAPVValueAttributes)
                    {
                        List<float> lstDouble = new List<float>();
                        lstDouble.Add(apvValueAttribute.PointEstimate);
                        if (apvValueAttribute.LstPercentile != null)
                            lstDouble.AddRange(apvValueAttribute.LstPercentile);
                        dicValue.Add(apvValueAttribute.Col + "," + apvValueAttribute.Row, lstDouble);
                    }
                    dicAll.Add(i, dicValue);
                    lstAllColRow = lstAllColRow.Union(dicValue.Keys.ToList()).ToList();
                    i++;
                }
                List<List<float>> lstPercentile = new List<List<float>>(); List<float> lstPooling = new List<float>();
                Random randomPooling = new Random();

                double dChidist = 0;
                List<double> lstRandomWeight = new List<double>();
                if (poolingMethod == PoolingMethodTypeEnum.RandomOrFixedEffects)
                {
                    dChidist = getChidist(dicAll, ref lstRandomWeight);
                }
                int j = 0;
                switch (poolingMethod)
                {

                    case PoolingMethodTypeEnum.SumDependent:
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new APVValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstPooling = new List<float>();
                            dPoint = 0;
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {
                                try
                                {
                                    dPoint += k.Value[s].First();
                                    if (lstPooling.Count == 0)
                                    {
                                        lstPooling = k.Value[s].GetRange(1, k.Value[s].Count - 1);
                                    }
                                    else
                                    {
                                        i = 1;
                                        while (i < k.Value[s].Count)
                                        {
                                            lstPooling[i - 1] += k.Value[s][i];
                                            i++;
                                        }

                                    }

                                }
                                catch
                                { }
                            }
                            aValueAttribute.PointEstimate = Convert.ToSingle(dPoint);
                            aValueAttribute.LstPercentile = lstPooling;
                            if (aValueAttribute.PointEstimate != 0)
                            {
                                aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(lstPooling);
                                aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(lstPooling, dPoint);
                                aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(lstPooling, dPoint);
                            }
                            allSelectValuationMethodAndValue.lstAPVValueAttributes.Add(aValueAttribute);

                        }
                        break;

                    case PoolingMethodTypeEnum.SumIndependent:
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new APVValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstPooling = new List<float>();
                            dPoint = 0;
                            lstPercentile = new List<List<float>>();
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {
                                try
                                {
                                    dPoint += k.Value[s].First();
                                    lstPercentile.Add(k.Value[s].GetRange(1, 100));


                                }
                                catch
                                { }
                            }
                            lstPooling = new List<float>();

                            for (int iPer = 0; iPer < 5000; iPer++)
                            {
                                float d = lstPercentile[0][randomPooling.Next(0, 100 - 1)];
                                for (int iPerSec = 1; iPerSec < lstPercentile.Count; iPerSec++)
                                {
                                    d = d + lstPercentile[iPerSec][randomPooling.Next(0, 100 - 1)];
                                }
                                lstPooling.Add(d);

                            }
                            aValueAttribute.LstPercentile = getMedianSample(lstPooling, 100);
                            aValueAttribute.PointEstimate = dPoint;
                            if (aValueAttribute.PointEstimate != 0)
                            {
                                aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(aValueAttribute.LstPercentile);
                                aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(aValueAttribute.LstPercentile, dPoint);
                                aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(aValueAttribute.LstPercentile, dPoint);
                            }
                            allSelectValuationMethodAndValue.lstAPVValueAttributes.Add(aValueAttribute);

                        }

                        break;
                    case PoolingMethodTypeEnum.SubtractionDependent:
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new APVValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstPooling = new List<float>();
                            dPoint = 0;
                            int iSubtractionDependent = 0;
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {
                                try
                                {
                                    if (iSubtractionDependent == 0)
                                        dPoint += k.Value[s].First();
                                    else
                                        dPoint -= k.Value[s].First();
                                    iSubtractionDependent++;
                                    if (lstPooling.Count == 0)
                                    {

                                        lstPooling = k.Value[s].GetRange(1, k.Value[s].Count - 1);
                                    }
                                    else
                                    {

                                        i = 1;
                                        while (i < k.Value[s].Count)
                                        {
                                            lstPooling[i - 1] -= k.Value[s][i];
                                            i++;
                                        }

                                    }

                                }
                                catch
                                { }
                            }
                            aValueAttribute.PointEstimate = dPoint;
                            aValueAttribute.LstPercentile = lstPooling;
                            if (aValueAttribute.PointEstimate != 0)
                            {
                                aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(lstPooling);
                                aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(lstPooling, dPoint);
                                aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(lstPooling, dPoint);
                            }
                            allSelectValuationMethodAndValue.lstAPVValueAttributes.Add(aValueAttribute);

                        }

                        break;
                    case PoolingMethodTypeEnum.SubtractionIndependent:
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new APVValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstPooling = new List<float>();
                            dPoint = 0;
                            int iSubtractionIndependent = 0;
                            lstPercentile = new List<List<float>>();
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {
                                try
                                {
                                    if (iSubtractionIndependent == 0)
                                        dPoint += k.Value[s].First();
                                    else
                                        dPoint -= k.Value[s].First();
                                    iSubtractionIndependent++;
                                    lstPercentile.Add(k.Value[s].GetRange(1, 100));




                                }
                                catch
                                { }
                            }
                            lstPooling = new List<float>();

                            for (int iPer = 0; iPer < 5000; iPer++)
                            {
                                float d = lstPercentile[0][randomPooling.Next(0, 100 - 1)];
                                for (int iPerSec = 1; iPerSec < lstPercentile.Count; iPerSec++)
                                {
                                    d = d - lstPercentile[iPerSec][randomPooling.Next(0, 100 - 1)];
                                }
                                lstPooling.Add(d);

                            }
                            aValueAttribute.LstPercentile = getMedianSample(lstPooling, 100);
                            aValueAttribute.PointEstimate = dPoint;
                            if (aValueAttribute.PointEstimate != 0)
                            {
                                aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(aValueAttribute.LstPercentile);
                                aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(aValueAttribute.LstPercentile, dPoint);
                                aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(aValueAttribute.LstPercentile, dPoint);
                            }
                            allSelectValuationMethodAndValue.lstAPVValueAttributes.Add(aValueAttribute);

                        }
                        break;
                    case PoolingMethodTypeEnum.SubjectiveWeights:
                        if (lstWeight != null && lstWeight.Count > 0 && lstWeight.Sum() != 1)
                        {
                            double dWeightSum = lstWeight.Sum();
                            for (int iWeightSum = 0; iWeightSum < lstWeight.Count; iWeightSum++)
                            {
                                lstWeight[iWeightSum] = lstWeight[iWeightSum] / dWeightSum;
                            }
                        }
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new APVValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstPooling = new List<float>();
                            dPoint = 0;
                            lstPercentile = new List<List<float>>();
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {
                                try
                                {
                                    dPoint += Convert.ToSingle(k.Value[s].First() * lstWeight[k.Key]);
                                    for (int iPer = 0; iPer < Convert.ToInt32(Math.Round(lstWeight[k.Key] * 100)); iPer++)
                                    {
                                        lstPooling.AddRange(k.Value[s].GetRange(1, 100));
                                    }


                                }
                                catch
                                { }
                            }
                            if (lstPooling.Count > 0)
                                aValueAttribute.LstPercentile = getMedianSample(lstPooling, 100);
                            else
                            {
                                aValueAttribute.LstPercentile = new List<float>();
                                for (int iPer = 0; iPer < 100; iPer++)
                                {
                                    aValueAttribute.LstPercentile.Add(0);
                                }
                            }
                            aValueAttribute.PointEstimate = dPoint;
                            if (aValueAttribute.PointEstimate != 0)
                            {
                                aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(aValueAttribute.LstPercentile);
                                aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(aValueAttribute.LstPercentile, dPoint);
                                aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(aValueAttribute.LstPercentile, dPoint);
                            }
                            allSelectValuationMethodAndValue.lstAPVValueAttributes.Add(aValueAttribute);

                        }

                        break;
                    case PoolingMethodTypeEnum.RandomOrFixedEffects:

                    case PoolingMethodTypeEnum.FixedEffects:
                        i = 0;
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new APVValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstPooling = new List<float>();
                            dPoint = 0;
                            double sumVariance = 0;
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {

                                if (k.Value.ContainsKey(s) && Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]) != 0)
                                {
                                    sumVariance += 1.00000 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]);
                                }
                            }
                            lstWeight = new List<double>();
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {
                                if (sumVariance != 0 && Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]) != 0)
                                {
                                    lstWeight.Add((1.00000 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0])) / sumVariance);
                                }
                                else
                                {
                                    lstWeight.Add(0);
                                }

                            }
                            for (int iw = 0; iw < lstWeight.Count; iw++)
                            {
                                if (double.IsNaN(lstWeight[iw]))
                                {
                                    lstWeight[iw] = 0;

                                }
                            }
                            lstPercentile = new List<List<float>>();
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {
                                try
                                {
                                    dPoint += Convert.ToSingle(k.Value[s].First() * lstWeight[k.Key]);
                                    for (int iPer = 0; iPer < Convert.ToInt32(Math.Round(lstWeight[k.Key] * 100)); iPer++)
                                    {
                                        lstPooling.AddRange(k.Value[s].GetRange(1, 100));
                                    }


                                }
                                catch
                                { }
                            }
                            if (poolingMethod == PoolingMethodTypeEnum.RandomOrFixedEffects)
                            {
                                if (dChidist < 0.05)
                                {
                                    double dSum = 0, dRandom = 0;
                                    int idSum = 0;
                                    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                                    {

                                        if (k.Value.ContainsKey(s))
                                        {
                                            dSum += (1 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0])) * Math.Pow((dPoint - k.Value[s][0]), 2);
                                        }
                                        idSum++;
                                    }
                                    idSum = 0;
                                    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                                    {

                                        if (k.Value.ContainsKey(s))
                                        {
                                            dRandom += Math.Pow(1 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]), 2);

                                        }
                                        idSum++;
                                    }

                                    dRandom = (dSum - 1) / (sumVariance - dRandom / sumVariance);
                                    lstWeight.Clear();
                                    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                                    {

                                        if (k.Value.ContainsKey(s))
                                        {
                                            lstWeight.Add(1 / (Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]) + dRandom));
                                        }
                                        else
                                        {
                                            lstWeight.Add(1.0000 / Convert.ToDouble(dicAll.Count));
                                        }
                                    }
                                    dSum = lstWeight.Sum();
                                    for (int iw = 0; iw < lstWeight.Count; iw++)
                                    {
                                        lstWeight[iw] = lstWeight[iw] / dSum;
                                    }
                                    for (int iw = 0; iw < lstWeight.Count; iw++)
                                    {
                                        if (double.IsNaN(lstWeight[iw]))
                                        {
                                            lstWeight[iw] = 0;

                                        }
                                    }
                                    j = 0;
                                    dPoint = 0;
                                    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                                    {

                                        if (k.Value.ContainsKey(s))
                                        {
                                            try
                                            {
                                                dPoint += Convert.ToSingle(k.Value[s][0] * lstWeight[j]);


                                            }
                                            catch
                                            {
                                            }
                                        }
                                        j++;

                                    }

                                }
                                else
                                {
                                    for (int iw = 0; iw < lstRandomWeight.Count; iw++)
                                    {
                                        if (double.IsNaN(lstRandomWeight[iw]))
                                        {
                                            lstRandomWeight[iw] = 0;

                                        }
                                    }
                                    j = 0;
                                    dPoint = 0;
                                    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                                    {

                                        if (k.Value.ContainsKey(s))
                                        {
                                            try
                                            {
                                                dPoint += Convert.ToSingle(k.Value[s][0] * lstRandomWeight[j]);


                                            }
                                            catch
                                            {
                                            }
                                        }
                                        j++;

                                    }
                                }
                            }
                            if (lstPooling.Count > 0)
                                aValueAttribute.LstPercentile = getMedianSample(lstPooling, 100);
                            else
                            {
                                aValueAttribute.LstPercentile = new List<float>();
                                for (int iPer = 0; iPer < 100; iPer++)
                                {
                                    aValueAttribute.LstPercentile.Add(0);
                                }
                            }
                            aValueAttribute.PointEstimate = dPoint;
                            if (aValueAttribute.PointEstimate != 0)
                            {
                                aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(aValueAttribute.LstPercentile);
                                aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(aValueAttribute.LstPercentile, dPoint);
                                aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(aValueAttribute.LstPercentile, dPoint);
                            }
                            allSelectValuationMethodAndValue.lstAPVValueAttributes.Add(aValueAttribute);

                        }

                        break;
                }
                return allSelectValuationMethodAndValue;
            }
            catch
            {
                return null;
            }

        }

        public static AllSelectQALYMethodAndValue getPoolingLstAllSelectQALYMethodAndValue(List<AllSelectQALYMethodAndValue> lstAllSelectQALYMethodAndValue, PoolingMethodTypeEnum poolingMethod, List<double> lstWeight)
        {
            try
            {
                AllSelectQALYMethodAndValue allSelectQALYMethodAndValue = new AllSelectQALYMethodAndValue();
                allSelectQALYMethodAndValue.lstQALYValueAttributes = new List<QALYValueAttribute>();

                Dictionary<int, Dictionary<string, List<float>>> dicAll = new Dictionary<int, Dictionary<string, List<float>>>();
                Dictionary<string, List<float>> dicValue = new Dictionary<string, List<float>>();
                int i = 0;
                List<string> lstAllColRow = new List<string>();
                QALYValueAttribute aValueAttribute = new QALYValueAttribute();
                string[] strColRow;
                float dPoint = 0;
                double dChidist = 0;
                List<double> lstRandomWeight = new List<double>();
                if (poolingMethod == PoolingMethodTypeEnum.RandomOrFixedEffects)
                {
                    dChidist = getChidist(dicAll, ref lstRandomWeight);
                }
                int j = 0;
                while (i < lstAllSelectQALYMethodAndValue.Count)
                {
                    dicValue = new Dictionary<string, List<float>>();

                    foreach (QALYValueAttribute apvValueAttribute in lstAllSelectQALYMethodAndValue[i].lstQALYValueAttributes)
                    {
                        List<float> lstDouble = new List<float>();
                        lstDouble.Add(apvValueAttribute.PointEstimate);
                        if (apvValueAttribute.LstPercentile != null)
                            lstDouble.AddRange(apvValueAttribute.LstPercentile);
                        dicValue.Add(apvValueAttribute.Col + "," + apvValueAttribute.Row, lstDouble);
                    }
                    dicAll.Add(i, dicValue);
                    lstAllColRow = lstAllColRow.Union(dicValue.Keys.ToList()).ToList();
                    i++;
                }
                List<List<float>> lstPercentile = new List<List<float>>(); List<float> lstPooling = new List<float>();
                Random randomPooling = new Random();
                switch (poolingMethod)
                {

                    case PoolingMethodTypeEnum.SumDependent:
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new QALYValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstPooling = new List<float>();
                            dPoint = 0;
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {
                                try
                                {
                                    dPoint += k.Value[s].First();
                                    if (lstPooling.Count == 0)
                                    {
                                        lstPooling = k.Value[s].GetRange(1, k.Value[s].Count - 1);
                                    }
                                    else
                                    {
                                        i = 1;
                                        while (i < k.Value[s].Count)
                                        {
                                            lstPooling[i - 1] += k.Value[s][i];
                                            i++;
                                        }

                                    }

                                }
                                catch
                                { }
                            }
                            aValueAttribute.PointEstimate = Convert.ToSingle(dPoint);
                            aValueAttribute.LstPercentile = lstPooling;
                            aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(lstPooling);
                            aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(lstPooling, dPoint);
                            aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(lstPooling, dPoint);
                            allSelectQALYMethodAndValue.lstQALYValueAttributes.Add(aValueAttribute);

                        }
                        break;

                    case PoolingMethodTypeEnum.SumIndependent:
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new QALYValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstPooling = new List<float>();
                            dPoint = 0;
                            lstPercentile = new List<List<float>>();
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {
                                try
                                {
                                    dPoint += k.Value[s].First();
                                    lstPercentile.Add(k.Value[s].GetRange(1, 100));


                                }
                                catch
                                { }
                            }
                            lstPooling = new List<float>();

                            for (int iPer = 0; iPer < 5000; iPer++)
                            {
                                float d = lstPercentile[0][randomPooling.Next(0, 100 - 1)];
                                for (int iPerSec = 1; iPerSec < lstPercentile.Count; iPerSec++)
                                {
                                    d = d + lstPercentile[iPerSec][randomPooling.Next(0, 100 - 1)];
                                }
                                lstPooling.Add(d);

                            }
                            aValueAttribute.LstPercentile = getMedianSample(lstPooling, 100);
                            aValueAttribute.PointEstimate = dPoint;
                            aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(aValueAttribute.LstPercentile);
                            aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(aValueAttribute.LstPercentile, dPoint);
                            aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(aValueAttribute.LstPercentile, dPoint);
                            allSelectQALYMethodAndValue.lstQALYValueAttributes.Add(aValueAttribute);

                        }

                        break;
                    case PoolingMethodTypeEnum.SubtractionDependent:
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new QALYValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstPooling = new List<float>();
                            dPoint = 0;
                            int iSubtractionDependent = 0;
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {
                                try
                                {
                                    if (iSubtractionDependent == 0)
                                        dPoint += k.Value[s].First();
                                    else
                                        dPoint -= k.Value[s].First();
                                    iSubtractionDependent++;
                                    if (lstPooling.Count == 0)
                                    {

                                        lstPooling = k.Value[s].GetRange(1, k.Value[s].Count - 1);
                                    }
                                    else
                                    {

                                        i = 1;
                                        while (i < k.Value[s].Count)
                                        {
                                            lstPooling[i - 1] -= k.Value[s][i];
                                            i++;
                                        }

                                    }

                                }
                                catch
                                { }
                            }
                            aValueAttribute.PointEstimate = dPoint;
                            aValueAttribute.LstPercentile = lstPooling;
                            aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(lstPooling);
                            aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(lstPooling, dPoint);
                            aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(lstPooling, dPoint);
                            allSelectQALYMethodAndValue.lstQALYValueAttributes.Add(aValueAttribute);

                        }

                        break;
                    case PoolingMethodTypeEnum.SubtractionIndependent:
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new QALYValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstPooling = new List<float>();
                            dPoint = 0;
                            int iSubtractionIndependent = 0;
                            lstPercentile = new List<List<float>>();
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {
                                try
                                {
                                    if (iSubtractionIndependent == 0)
                                        dPoint += k.Value[s].First();
                                    else
                                        dPoint -= k.Value[s].First();
                                    iSubtractionIndependent++;
                                    lstPercentile.Add(k.Value[s].GetRange(1, 100));




                                }
                                catch
                                { }
                            }
                            lstPooling = new List<float>();

                            for (int iPer = 0; iPer < 5000; iPer++)
                            {
                                float d = lstPercentile[0][randomPooling.Next(0, 100 - 1)];
                                for (int iPerSec = 1; iPerSec < lstPercentile.Count; iPerSec++)
                                {
                                    d = d - lstPercentile[iPerSec][randomPooling.Next(0, 100 - 1)];
                                }
                                lstPooling.Add(d);

                            }
                            aValueAttribute.LstPercentile = getMedianSample(lstPooling, 100);
                            aValueAttribute.PointEstimate = dPoint;
                            aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(aValueAttribute.LstPercentile);
                            aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(aValueAttribute.LstPercentile, dPoint);
                            aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(aValueAttribute.LstPercentile, dPoint);
                            allSelectQALYMethodAndValue.lstQALYValueAttributes.Add(aValueAttribute);

                        }
                        break;
                    case PoolingMethodTypeEnum.SubjectiveWeights:
                        if (lstWeight != null && lstWeight.Count > 0 && lstWeight.Sum() != 1)
                        {
                            double dWeightSum = lstWeight.Sum();
                            for (int iWeightSum = 0; iWeightSum < lstWeight.Count; iWeightSum++)
                            {
                                lstWeight[iWeightSum] = lstWeight[iWeightSum] / dWeightSum;
                            }
                        }
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new QALYValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstPooling = new List<float>();
                            dPoint = 0;
                            lstPercentile = new List<List<float>>();
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {
                                try
                                {
                                    dPoint += Convert.ToSingle(k.Value[s].First() * lstWeight[k.Key]);
                                    for (int iPer = 0; iPer < Convert.ToInt32(Math.Round(lstWeight[k.Key] * 100)); iPer++)
                                    {
                                        lstPooling.AddRange(k.Value[s].GetRange(1, 100));
                                    }


                                }
                                catch
                                { }
                            }
                            if (lstPooling.Count > 0)
                                aValueAttribute.LstPercentile = getMedianSample(lstPooling, 100);
                            else
                            {
                                aValueAttribute.LstPercentile = new List<float>();
                                for (int iPer = 0; iPer < 100; iPer++)
                                {
                                    aValueAttribute.LstPercentile.Add(0);
                                }
                            }
                            aValueAttribute.PointEstimate = dPoint;
                            aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(lstPooling);
                            aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(lstPooling, dPoint);
                            aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(lstPooling, dPoint);
                            allSelectQALYMethodAndValue.lstQALYValueAttributes.Add(aValueAttribute);

                        }

                        break;
                    case PoolingMethodTypeEnum.RandomOrFixedEffects:

                    case PoolingMethodTypeEnum.FixedEffects:
                        i = 0;
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new QALYValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstPooling = new List<float>();
                            dPoint = 0;
                            double sumVariance = 0;
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {

                                if (k.Value.ContainsKey(s) && Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]) != 0)
                                {
                                    sumVariance += 1.00000 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]);
                                }
                            }
                            lstWeight = new List<double>();
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {
                                if (sumVariance != 0 && Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]) != 0)
                                {
                                    lstWeight.Add((1.00000 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0])) / sumVariance);
                                }
                                else
                                {
                                    lstWeight.Add(0);
                                }

                            }
                            for (int iw = 0; iw < lstWeight.Count; iw++)
                            {
                                if (double.IsNaN(lstWeight[iw]))
                                {
                                    lstWeight[iw] = 0;

                                }
                            }
                            lstPercentile = new List<List<float>>();
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {
                                try
                                {
                                    dPoint += Convert.ToSingle(k.Value[s].First() * lstWeight[k.Key]);
                                    for (int iPer = 0; iPer < Convert.ToInt32(Math.Round(lstWeight[k.Key] * 100)); iPer++)
                                    {
                                        lstPooling.AddRange(k.Value[s].GetRange(1, 100));
                                    }


                                }
                                catch
                                { }
                            }
                            if (poolingMethod == PoolingMethodTypeEnum.RandomOrFixedEffects)
                            {
                                if (dChidist < 0.05)
                                {
                                    double dSum = 0, dRandom = 0;
                                    int idSum = 0;
                                    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                                    {

                                        if (k.Value.ContainsKey(s))
                                        {
                                            dSum += (1 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0])) * Math.Pow((dPoint - k.Value[s][0]), 2);
                                        }
                                        idSum++;
                                    }
                                    idSum = 0;
                                    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                                    {

                                        if (k.Value.ContainsKey(s))
                                        {
                                            dRandom += Math.Pow(1 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]), 2);

                                        }
                                        idSum++;
                                    }

                                    dRandom = (dSum - 1) / (sumVariance - dRandom / sumVariance);
                                    lstWeight.Clear();
                                    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                                    {

                                        if (k.Value.ContainsKey(s))
                                        {
                                            lstWeight.Add(1 / (Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]) + dRandom));
                                        }
                                        else
                                        {
                                            lstWeight.Add(1.0000 / Convert.ToDouble(dicAll.Count));
                                        }
                                    }
                                    dSum = lstWeight.Sum();
                                    for (int iw = 0; iw < lstWeight.Count; iw++)
                                    {
                                        lstWeight[iw] = lstWeight[iw] / dSum;
                                    }
                                    for (int iw = 0; iw < lstWeight.Count; iw++)
                                    {
                                        if (double.IsNaN(lstWeight[iw]))
                                        {
                                            lstWeight[iw] = 0;

                                        }
                                    }
                                    j = 0;
                                    dPoint = 0;
                                    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                                    {

                                        if (k.Value.ContainsKey(s))
                                        {
                                            try
                                            {
                                                dPoint += Convert.ToSingle(k.Value[s][0] * lstWeight[j]);


                                            }
                                            catch
                                            {
                                            }
                                        }
                                        j++;

                                    }

                                }
                                else
                                {
                                    for (int iw = 0; iw < lstRandomWeight.Count; iw++)
                                    {
                                        if (double.IsNaN(lstRandomWeight[iw]))
                                        {
                                            lstRandomWeight[iw] = 0;

                                        }
                                    }
                                    j = 0;
                                    dPoint = 0;
                                    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                                    {

                                        if (k.Value.ContainsKey(s))
                                        {
                                            try
                                            {
                                                dPoint += Convert.ToSingle(k.Value[s][0] * lstRandomWeight[j]);


                                            }
                                            catch
                                            {
                                            }
                                        }
                                        j++;

                                    }
                                }
                            }
                            if (lstPooling.Count > 0)
                                aValueAttribute.LstPercentile = getMedianSample(lstPooling, 100);
                            else
                            {
                                aValueAttribute.LstPercentile = new List<float>();
                                for (int iPer = 0; iPer < 100; iPer++)
                                {
                                    aValueAttribute.LstPercentile.Add(0);
                                }
                            }
                            aValueAttribute.PointEstimate = dPoint;
                            aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(aValueAttribute.LstPercentile);
                            aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(aValueAttribute.LstPercentile, dPoint);
                            aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(aValueAttribute.LstPercentile, dPoint);
                            allSelectQALYMethodAndValue.lstQALYValueAttributes.Add(aValueAttribute);

                        }

                        break;
                }
                return allSelectQALYMethodAndValue;
            }
            catch
            {
                return null;
            }

        }

        public static AllSelectValuationMethodAndValue ApplyAllSelectValuationMethodAndValueAggregation(GridRelationship gridRelationship, BenMAPGrid benMAPGrid, AllSelectValuationMethodAndValue asvv)
        {
            AllSelectValuationMethodAndValue asvvnew = new AllSelectValuationMethodAndValue();
            if (gridRelationship == null) return asvv;
            int icount = 0;
            try
            {
                asvvnew.AllSelectValuationMethod = asvv.AllSelectValuationMethod;
                asvvnew.lstAPVValueAttributes = new List<APVValueAttribute>(); 
                string str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", gridRelationship.smallGridID, gridRelationship.bigGridID);
                if (gridRelationship.smallGridID == benMAPGrid.GridDefinitionID)
                {
                    str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", benMAPGrid.GridDefinitionID, gridRelationship.bigGridID);
                }
                if (benMAPGrid.GridDefinitionID == 28 || gridRelationship.smallGridID == 28 || benMAPGrid.GridDefinitionID == 27 || gridRelationship.smallGridID == 27)
                {
                    str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", 27, gridRelationship.bigGridID);
                }
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    Dictionary<string, Dictionary<string, double>> dicRelationShip = new Dictionary<string, Dictionary<string, double>>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (dicRelationShip.ContainsKey(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()))
                        {
                            if (!dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
                                dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                        }
                        else
                        {
                            dicRelationShip.Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), new Dictionary<string, double>());
                            dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                        }

                    }
                    ds.Dispose();
                    if (gridRelationship.bigGridID == benMAPGrid.GridDefinitionID)
                    {
                        Dictionary<string, APVValueAttribute> dicAggregation = new Dictionary<string, APVValueAttribute>();
                        foreach (APVValueAttribute ava in asvv.lstAPVValueAttributes)
                        {
                            if (dicRelationShip.ContainsKey(ava.Col + "," + ava.Row))
                            {
                                double d = dicRelationShip[ava.Col + "," + ava.Row].Sum(p => p.Value);
                                foreach (KeyValuePair<string, double> k in dicRelationShip[ava.Col + "," + ava.Row])
                                {
                                    if (dicAggregation.ContainsKey(k.Key))
                                    {
                                        APVValueAttribute apv = dicAggregation[k.Key];
                                        apv.PointEstimate += ava.PointEstimate * Convert.ToSingle(k.Value) / Convert.ToSingle(d);
                                        if (ava.LstPercentile != null)
                                        {
                                            int iAggregation = 0;
                                            foreach (float dLstPercentile in ava.LstPercentile)
                                            {
                                                apv.LstPercentile[iAggregation] += dLstPercentile * Convert.ToSingle(k.Value) / Convert.ToSingle(d);
                                                iAggregation++;
                                            }
                                        }
                                        if (apv.LstPercentile != null && apv.LstPercentile.Count > 0)
                                        {
                                            apv.Mean = Configuration.ConfigurationCommonClass.getMean(apv.LstPercentile);
                                            apv.Variance = Configuration.ConfigurationCommonClass.getVariance(apv.LstPercentile, apv.PointEstimate);
                                            apv.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(apv.LstPercentile, apv.PointEstimate);
                                        }
                                    }
                                    else
                                    {
                                        asvvnew.lstAPVValueAttributes.Add(new APVValueAttribute()
                                        {

                                            Col = Convert.ToInt32(k.Key.Split(new char[] { ',' }).ToArray()[0]),
                                            Row = Convert.ToInt32(k.Key.Split(new char[] { ',' }).ToArray()[1]),
                                            PointEstimate = ava.PointEstimate * Convert.ToSingle(k.Value) / Convert.ToSingle(d),

                                        }
                                         );
                                        if (ava.LstPercentile != null)
                                        {
                                            asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile = new List<float>();
                                            foreach (float dLstPercentile in ava.LstPercentile)
                                            {
                                                asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile.Add(dLstPercentile * Convert.ToSingle(k.Value) / Convert.ToSingle(d));
                                            }
                                        }
                                        if (asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile != null && asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile.Count > 0)
                                        {
                                            asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].Mean = Configuration.ConfigurationCommonClass.getMean(asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile);
                                            asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].Variance = Configuration.ConfigurationCommonClass.getVariance(asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile, asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].PointEstimate);
                                            asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile, asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].PointEstimate);
                                        }
                                        dicAggregation.Add(k.Key, asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1]);
                                    }


                                }
                            }
                        }
                        dicAggregation.Clear();
                        dicAggregation = null;
                    }
                    else
                    {
                        Dictionary<string, APVValueAttribute> dicAPVValueAttribute = new Dictionary<string, APVValueAttribute>();
                        APVValueAttribute anewfirst = new APVValueAttribute();
                        anewfirst.LstPercentile = new List<float>(); if (asvv.lstAPVValueAttributes.First().LstPercentile != null)
                        {
                            for (int iPercentile = 0; iPercentile < asvv.lstAPVValueAttributes.First().LstPercentile.Count; iPercentile++)
                            {
                                anewfirst.LstPercentile.Add(0);
                            }
                        }
                        Dictionary<string, APVValueAttribute> dicValuationValueFrom = new Dictionary<string, APVValueAttribute>();
                        foreach (APVValueAttribute ava in asvv.lstAPVValueAttributes)
                        {
                            if (!dicValuationValueFrom.ContainsKey(ava.Col + "," + ava.Row))
                                dicValuationValueFrom.Add(ava.Col + "," + ava.Row, ava);
                        }
                        foreach (KeyValuePair<string, Dictionary<string, double>> gra in dicRelationShip)
                        {
                            APVValueAttribute anew = new APVValueAttribute();
                            if (anewfirst.LstPercentile != null)
                            {
                                anew.LstPercentile = new List<float>();
                                foreach (float d in anewfirst.LstPercentile)
                                {
                                    anew.LstPercentile.Add(d);
                                }
                            }
                            anew.Col = Convert.ToInt32(gra.Key.Split(new char[] { ',' }).ToArray()[0]);
                            anew.Row = Convert.ToInt32(gra.Key.Split(new char[] { ',' }).ToArray()[1]);

                            foreach (KeyValuePair<string, double> k in gra.Value)
                            {
                                if (dicValuationValueFrom.ContainsKey(k.Key))
                                {
                                    APVValueAttribute ValuationValue = dicValuationValueFrom[k.Key];
                                    anew.PointEstimate += ValuationValue.PointEstimate * Convert.ToSingle(k.Value);

                                    if (ValuationValue.LstPercentile != null)
                                    {
                                        if (anew.LstPercentile == null) anew.LstPercentile = new List<float>();
                                        for (int iPercentile = 0; iPercentile < ValuationValue.LstPercentile.Count(); iPercentile++)
                                        {
                                            anew.LstPercentile[iPercentile] += ValuationValue.LstPercentile[iPercentile] * Convert.ToSingle(k.Value);
                                        }
                                    }
                                }
                            }
                            if (anew.LstPercentile != null && anew.LstPercentile.Count > 0)
                            {
                                anew.Mean = Configuration.ConfigurationCommonClass.getMean(anew.LstPercentile);
                                anew.Variance = Configuration.ConfigurationCommonClass.getVariance(anew.LstPercentile, anew.PointEstimate);
                                anew.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(anew.LstPercentile, anew.PointEstimate);
                            }

                            dicAPVValueAttribute.Add(gra.Key, anew);
                        }

                        asvvnew.lstAPVValueAttributes = dicAPVValueAttribute.Values.Distinct().ToList();
                    }
                    if (dicRelationShip != null)
                    {
                        for (int iDic = 0; iDic < dicRelationShip.Count; iDic++)
                        {
                            dicRelationShip[dicRelationShip.Keys.ToArray()[iDic]].Clear();
                        }
                    }

                    dicRelationShip.Clear();
                    GC.Collect();
                }
                else
                {
                    asvvnew.AllSelectValuationMethod = asvv.AllSelectValuationMethod;
                    asvvnew.lstAPVValueAttributes = new List<APVValueAttribute>();

                    if (gridRelationship.bigGridID == benMAPGrid.GridDefinitionID)
                    {
                        foreach (APVValueAttribute ava in asvv.lstAPVValueAttributes)
                        {
                            icount = gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).Count();
                            if (icount > 0)
                            {
                                icount = gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).First().smallGridRowCol.Count();
                                foreach (RowCol rc in gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).First().smallGridRowCol)
                                {
                                    asvvnew.lstAPVValueAttributes.Add(new APVValueAttribute()
                                    {
                                        Row = rc.Row,
                                        Col = rc.Col,
                                        PointEstimate = ava.PointEstimate / Convert.ToSingle(icount),
                                        Mean = ava.Mean / Convert.ToSingle(icount),
                                        StandardDeviation = ava.StandardDeviation / Convert.ToSingle(icount),
                                        Variance = ava.Variance / Convert.ToSingle(icount),
                                    }
                                    );
                                    if (ava.LstPercentile != null)
                                    {
                                        asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile = new List<float>();
                                        foreach (float dLstPercentile in ava.LstPercentile)
                                        {
                                            asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile.Add(dLstPercentile / Convert.ToSingle(icount));
                                        }
                                    }
                                }
                            }



                        }
                    }
                    else
                    {
                        RowColComparer rowColComparer = new RowColComparer();
                        Dictionary<string, string> dicRowCol = new Dictionary<string, string>();
                        foreach (GridRelationshipAttribute gra in gridRelationship.lstGridRelationshipAttribute)
                        {
                            foreach (RowCol rc in gra.smallGridRowCol)
                            {
                                if (!dicRowCol.Keys.Contains(rc.Col + "," + rc.Row))
                                    dicRowCol.Add(rc.Col + "," + rc.Row, gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row);

                            }
                        }
                        Dictionary<string, APVValueAttribute> dicAPVValueAttribute = new Dictionary<string, APVValueAttribute>();
                        APVValueAttribute anewfirst = new APVValueAttribute();
                        anewfirst.LstPercentile = new List<float>(); if (asvv.lstAPVValueAttributes.First().LstPercentile != null)
                        {
                            for (int iPercentile = 0; iPercentile < asvv.lstAPVValueAttributes.First().LstPercentile.Count; iPercentile++)
                            {
                                anewfirst.LstPercentile.Add(0);
                            }
                        }
                        foreach (GridRelationshipAttribute gra in gridRelationship.lstGridRelationshipAttribute)
                        {
                            APVValueAttribute anew = new APVValueAttribute();
                            if (anewfirst.LstPercentile != null)
                            {
                                anew.LstPercentile = new List<float>();
                                foreach (float d in anewfirst.LstPercentile)
                                {
                                    anew.LstPercentile.Add(d);
                                }
                            }
                            anew.Col = gra.bigGridRowCol.Col;
                            anew.Row = gra.bigGridRowCol.Row;

                            dicAPVValueAttribute.Add(gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row, anew);
                        }
                        foreach (APVValueAttribute ava in asvv.lstAPVValueAttributes)
                        {
                            if (dicRowCol.Keys.Contains(ava.Col + "," + ava.Row))
                            {
                                APVValueAttribute apvValueAttribute = dicAPVValueAttribute[dicRowCol[ava.Col + "," + ava.Row]];
                                apvValueAttribute.Mean += ava.Mean;
                                apvValueAttribute.PointEstimate += ava.PointEstimate;
                                apvValueAttribute.StandardDeviation += ava.StandardDeviation;
                                apvValueAttribute.Variance += ava.Variance;
                                if (ava.LstPercentile != null)
                                {
                                    if (apvValueAttribute.LstPercentile == null)
                                        apvValueAttribute.LstPercentile = ava.LstPercentile;
                                    else
                                    {
                                        for (int iPercentile = 0; iPercentile < ava.LstPercentile.Count(); iPercentile++)
                                        {
                                            apvValueAttribute.LstPercentile[iPercentile] += ava.LstPercentile[iPercentile];
                                        }
                                    }

                                }
                            }

                        }
                        asvvnew.lstAPVValueAttributes = dicAPVValueAttribute.Values.ToList();








                    }
                }
            }
            catch
            { }
            return asvvnew;
        }
        public static AllSelectQALYMethodAndValue ApplyAllSelectQALYMethodAndValueAggregation(GridRelationship gridRelationship, BenMAPGrid benMAPGrid, AllSelectQALYMethodAndValue asvv)
        {
            AllSelectQALYMethodAndValue asvvnew = new AllSelectQALYMethodAndValue();
            int icount = 0;
            if (gridRelationship == null) return asvv;
            try
            {
                asvvnew.AllSelectQALYMethod = asvv.AllSelectQALYMethod;
                asvvnew.lstQALYValueAttributes = new List<QALYValueAttribute>(); string str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", gridRelationship.smallGridID, gridRelationship.bigGridID);
                if (gridRelationship.smallGridID == benMAPGrid.GridDefinitionID)
                {
                    str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", benMAPGrid.GridDefinitionID, gridRelationship.bigGridID);
                }
                if (benMAPGrid.GridDefinitionID == 28 || gridRelationship.smallGridID == 28 || benMAPGrid.GridDefinitionID == 27 || gridRelationship.smallGridID == 27)
                {
                    str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", 27, gridRelationship.bigGridID);
                }
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    Dictionary<string, Dictionary<string, double>> dicRelationShip = new Dictionary<string, Dictionary<string, double>>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (dicRelationShip.ContainsKey(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()))
                        {
                            if (!dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
                                dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                        }
                        else
                        {
                            dicRelationShip.Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), new Dictionary<string, double>());
                            dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                        }

                    }
                    if (gridRelationship.bigGridID == benMAPGrid.GridDefinitionID)
                    {
                        Dictionary<string, QALYValueAttribute> dicAggregation = new Dictionary<string, QALYValueAttribute>();
                        foreach (QALYValueAttribute ava in asvv.lstQALYValueAttributes)
                        {
                            if (dicRelationShip.ContainsKey(ava.Col + "," + ava.Row))
                            {

                                double d = dicRelationShip[ava.Col + "," + ava.Row].Sum(p => p.Value);
                                foreach (KeyValuePair<string, double> k in dicRelationShip[ava.Col + "," + ava.Row])
                                {
                                    if (dicAggregation.ContainsKey(k.Key))
                                    {
                                        QALYValueAttribute qaly = dicAggregation[k.Key];
                                        qaly.PointEstimate += ava.PointEstimate * Convert.ToSingle(k.Value) / Convert.ToSingle(d);
                                        if (ava.LstPercentile != null)
                                        {
                                            int iAggregation = 0;
                                            foreach (float dLstPercentile in ava.LstPercentile)
                                            {
                                                qaly.LstPercentile[iAggregation] += dLstPercentile * Convert.ToSingle(k.Value) / Convert.ToSingle(d);
                                                iAggregation++;
                                            }
                                        }
                                        if (qaly.LstPercentile != null && qaly.LstPercentile.Count > 0)
                                        {
                                            qaly.Mean = Configuration.ConfigurationCommonClass.getMean(qaly.LstPercentile);
                                            qaly.Variance = Configuration.ConfigurationCommonClass.getVariance(qaly.LstPercentile, qaly.PointEstimate);
                                            qaly.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(qaly.LstPercentile, qaly.PointEstimate);
                                        }
                                    }
                                    else
                                    {
                                        asvvnew.lstQALYValueAttributes.Add(new QALYValueAttribute()
                                        {

                                            Col = Convert.ToInt32(k.Key.Split(new char[] { ',' }).ToArray()[0]),
                                            Row = Convert.ToInt32(k.Key.Split(new char[] { ',' }).ToArray()[1]),
                                            PointEstimate = ava.PointEstimate * Convert.ToSingle(k.Value) / Convert.ToSingle(d),

                                        }
                                         );
                                        if (ava.LstPercentile != null)
                                        {
                                            asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].LstPercentile = new List<float>();
                                            foreach (float dLstPercentile in ava.LstPercentile)
                                            {
                                                asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].LstPercentile.Add(dLstPercentile * Convert.ToSingle(k.Value) / Convert.ToSingle(d));
                                            }
                                        }
                                        if (asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].LstPercentile != null && asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].LstPercentile.Count > 0)
                                        {
                                            asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].Mean = Configuration.ConfigurationCommonClass.getMean(asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].LstPercentile);
                                            asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].Variance = Configuration.ConfigurationCommonClass.getVariance(asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].LstPercentile, asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].PointEstimate);
                                            asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].LstPercentile, asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].PointEstimate);
                                        }
                                        dicAggregation.Add(k.Key, asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1]);
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        Dictionary<string, QALYValueAttribute> dicQALYValueAttribute = new Dictionary<string, QALYValueAttribute>();
                        QALYValueAttribute anewfirst = new QALYValueAttribute();
                        anewfirst.LstPercentile = new List<float>(); if (asvv.lstQALYValueAttributes.First().LstPercentile != null)
                        {
                            for (int iPercentile = 0; iPercentile < asvv.lstQALYValueAttributes.First().LstPercentile.Count; iPercentile++)
                            {
                                anewfirst.LstPercentile.Add(0);
                            }
                        }
                        Dictionary<string, QALYValueAttribute> dicQALYValueFrom = new Dictionary<string, QALYValueAttribute>();
                        foreach (QALYValueAttribute ava in asvv.lstQALYValueAttributes)
                        {
                            if (!dicQALYValueFrom.ContainsKey(ava.Col + "," + ava.Row))
                                dicQALYValueFrom.Add(ava.Col + "," + ava.Row, ava);
                        }
                        foreach (KeyValuePair<string, Dictionary<string, double>> gra in dicRelationShip)
                        {
                            QALYValueAttribute anew = new QALYValueAttribute();
                            if (anewfirst.LstPercentile != null)
                            {
                                anew.LstPercentile = new List<float>();
                                foreach (float d in anewfirst.LstPercentile)
                                {
                                    anew.LstPercentile.Add(d);
                                }
                            }
                            anew.Col = Convert.ToInt32(gra.Key.Split(new char[] { ',' }).ToArray()[0]);
                            anew.Row = Convert.ToInt32(gra.Key.Split(new char[] { ',' }).ToArray()[1]);

                            foreach (KeyValuePair<string, double> k in gra.Value)
                            {
                                if (dicQALYValueFrom.ContainsKey(k.Key))
                                {
                                    QALYValueAttribute QALYValue = dicQALYValueFrom[k.Key];
                                    anew.PointEstimate += QALYValue.PointEstimate * Convert.ToSingle(k.Value);

                                    if (QALYValue.LstPercentile != null)
                                    {
                                        if (anew.LstPercentile == null) anew.LstPercentile = new List<float>();
                                        for (int iPercentile = 0; iPercentile < QALYValue.LstPercentile.Count(); iPercentile++)
                                        {
                                            anew.LstPercentile[iPercentile] += QALYValue.LstPercentile[iPercentile] * Convert.ToSingle(k.Value);
                                        }
                                    }
                                }
                            }
                            if (anew.LstPercentile != null && anew.LstPercentile.Count > 0)
                            {
                                anew.Mean = Configuration.ConfigurationCommonClass.getMean(anew.LstPercentile);
                                anew.Variance = Configuration.ConfigurationCommonClass.getVariance(anew.LstPercentile, anew.PointEstimate);
                                anew.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(anew.LstPercentile, anew.PointEstimate);
                            }

                            dicQALYValueAttribute.Add(gra.Key, anew);
                        }

                        asvvnew.lstQALYValueAttributes = dicQALYValueAttribute.Values.Distinct().ToList();
                    }
                }
                else
                {
                    asvvnew.AllSelectQALYMethod = asvv.AllSelectQALYMethod;
                    asvvnew.lstQALYValueAttributes = new List<QALYValueAttribute>();
                    if (gridRelationship.bigGridID == benMAPGrid.GridDefinitionID)
                    {
                        foreach (QALYValueAttribute ava in asvv.lstQALYValueAttributes)
                        {
                            icount = gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).Count();
                            if (icount > 0)
                            {
                                icount = gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).First().smallGridRowCol.Count();
                                foreach (RowCol rc in gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).First().smallGridRowCol)
                                {
                                    QALYValueAttribute qTemp = new QALYValueAttribute();
                                    qTemp.Col = rc.Col;
                                    qTemp.Row = rc.Row;
                                    qTemp.PointEstimate = ava.PointEstimate / Convert.ToSingle(icount);
                                    qTemp.StandardDeviation = ava.StandardDeviation / Convert.ToSingle(icount);
                                    qTemp.Mean = ava.Mean / Convert.ToSingle(icount);
                                    qTemp.Variance = ava.Variance / Convert.ToSingle(icount);
                                    if (ava.LstPercentile != null)
                                    {
                                        qTemp.LstPercentile = new List<float>();
                                        foreach (float f in ava.LstPercentile)
                                        {
                                            qTemp.LstPercentile.Add(f / Convert.ToSingle(icount));
                                        }

                                    }
                                    asvvnew.lstQALYValueAttributes.Add(qTemp);

                                }
                            }



                        }
                    }
                    else
                    {
                        if (asvvnew == null)
                        {
                            asvvnew = new AllSelectQALYMethodAndValue();
                            asvvnew.lstQALYValueAttributes = new List<QALYValueAttribute>();
                            asvvnew.AllSelectQALYMethod = asvv.AllSelectQALYMethod;
                        }
                        RowColComparer rowColComparer = new RowColComparer();
                        Dictionary<string, string> dicRowCol = new Dictionary<string, string>();
                        foreach (GridRelationshipAttribute gra in gridRelationship.lstGridRelationshipAttribute)
                        {
                            foreach (RowCol rc in gra.smallGridRowCol)
                            {
                                if (!dicRowCol.Keys.Contains(rc.Col + "," + rc.Row))
                                    dicRowCol.Add(rc.Col + "," + rc.Row, gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row);

                            }
                        }
                        Dictionary<string, QALYValueAttribute> dicQALYValueAttribute = new Dictionary<string, QALYValueAttribute>();

                        foreach (GridRelationshipAttribute gra in gridRelationship.lstGridRelationshipAttribute)
                        {
                            QALYValueAttribute anew = new QALYValueAttribute();
                            anew.Col = gra.bigGridRowCol.Col;
                            anew.Row = gra.bigGridRowCol.Row;

                            dicQALYValueAttribute.Add(gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row, anew);
                        }
                        foreach (QALYValueAttribute ava in asvv.lstQALYValueAttributes)
                        {
                            if (dicRowCol.Keys.Contains(ava.Col + "," + ava.Row))
                            {
                                QALYValueAttribute QALYValueAttribute = dicQALYValueAttribute[dicRowCol[ava.Col + "," + ava.Row]];
                                QALYValueAttribute.PointEstimate += ava.PointEstimate;
                                QALYValueAttribute.Mean += ava.Mean;
                                QALYValueAttribute.StandardDeviation += ava.StandardDeviation;
                                QALYValueAttribute.Variance += ava.Variance;
                                if (ava.LstPercentile != null)
                                {
                                    if (QALYValueAttribute.LstPercentile == null) QALYValueAttribute.LstPercentile = ava.LstPercentile;
                                    else
                                    {
                                        for (int iPercentile = 0; iPercentile < ava.LstPercentile.Count(); iPercentile++)
                                        {
                                            QALYValueAttribute.LstPercentile[iPercentile] += ava.LstPercentile[iPercentile];
                                        }
                                    }

                                }
                            }

                        }
                        asvvnew.lstQALYValueAttributes = dicQALYValueAttribute.Values.ToList();

                    }
                }


            }
            catch
            { }
            return asvvnew;
        }
        public static CRSelectFunctionCalculateValue ApplyCRSelectFunctionCalculateValueAggregation(GridRelationship gridRelationship, BenMAPGrid benMAPGrid, CRSelectFunctionCalculateValue cRSelectFunctionCalculateValue)
        {
            CRSelectFunctionCalculateValue asvvnew = new CRSelectFunctionCalculateValue();
            int icount = 0;
            try
            {
                asvvnew.CRCalculateValues = new List<CRCalculateValue>();

                string str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", benMAPGrid.GridDefinitionID, gridRelationship.smallGridID);
                if (gridRelationship.smallGridID == benMAPGrid.GridDefinitionID)
                {
                    str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", benMAPGrid.GridDefinitionID, gridRelationship.bigGridID);
                }
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    Dictionary<string, Dictionary<string, double>> dicRelationShip = new Dictionary<string, Dictionary<string, double>>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (dicRelationShip.ContainsKey(dr["targetcolumn"].ToString() + dr["targetrow"].ToString()))
                        {
                            if (!dicRelationShip[dr["targetcolumn"].ToString() + dr["targetrow"].ToString()].ContainsKey(dr["sourcecolumn"].ToString() + dr["sourcerow"].ToString()))
                                dicRelationShip[dr["targetcolumn"].ToString() + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                        }
                        else
                        {
                            dicRelationShip.Add(dr["targetcolumn"].ToString() + dr["targetrow"].ToString(), new Dictionary<string, double>());
                            dicRelationShip[dr["targetcolumn"].ToString() + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                        }

                    }
                    if (gridRelationship.bigGridID == benMAPGrid.GridDefinitionID)
                    {
                        foreach (CRCalculateValue ava in cRSelectFunctionCalculateValue.CRCalculateValues)
                        {
                            if (dicRelationShip.ContainsKey(ava.Col + "," + ava.Row))
                            {
                                double d = dicRelationShip[ava.Col + "," + ava.Row].Sum(p => p.Value);
                                foreach (KeyValuePair<string, double> k in dicRelationShip[ava.Col + "," + ava.Row])
                                {
                                    CRCalculateValue anew = new CRCalculateValue();
                                    asvvnew.CRCalculateValues.Add(new CRCalculateValue()
                                    {
                                        Row = Convert.ToInt32(k.Key.Split(new char[] { ',' }).ToArray()[0]),
                                        Col = Convert.ToInt32(k.Key.Split(new char[] { ',' }).ToArray()[0]),
                                        PointEstimate = ava.PointEstimate * Convert.ToSingle(k.Value) / Convert.ToSingle(d),
                                        Mean = ava.Mean * Convert.ToSingle(k.Value) / Convert.ToSingle(d),
                                        StandardDeviation = ava.StandardDeviation * Convert.ToSingle(k.Value) / Convert.ToSingle(d),
                                        Variance = ava.Variance * Convert.ToSingle(k.Value) / Convert.ToSingle(d),
                                    }
                                     );
                                    if (ava.LstPercentile != null)
                                    {
                                        asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].LstPercentile = new List<float>();
                                        foreach (float dLstPercentile in ava.LstPercentile)
                                        {
                                            asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].LstPercentile.Add(dLstPercentile * Convert.ToSingle(k.Value) / Convert.ToSingle(d));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Dictionary<string, CRCalculateValue> dicCRCalculateValue = new Dictionary<string, CRCalculateValue>();
                        CRCalculateValue anewfirst = new CRCalculateValue();
                        anewfirst.LstPercentile = new List<float>(); if (cRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile != null)
                        {
                            for (int iPercentile = 0; iPercentile < cRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count; iPercentile++)
                            {
                                anewfirst.LstPercentile.Add(0);
                            }
                        }
                        Dictionary<string, CRCalculateValue> dicCRCalculateValueFrom = new Dictionary<string, CRCalculateValue>();
                        foreach (CRCalculateValue ava in cRSelectFunctionCalculateValue.CRCalculateValues)
                        {
                            dicCRCalculateValueFrom.Add(ava.Col + "," + ava.Row, ava);
                        }
                        foreach (KeyValuePair<string, Dictionary<string, double>> gra in dicRelationShip)
                        {
                            CRCalculateValue anew = new CRCalculateValue();
                            if (anewfirst.LstPercentile != null)
                            {
                                anew.LstPercentile = new List<float>();
                                foreach (float d in anewfirst.LstPercentile)
                                {
                                    anew.LstPercentile.Add(d);
                                }
                            }
                            anew.Col = Convert.ToInt32(gra.Key.Split(new char[] { ',' }).ToArray()[0]);
                            anew.Row = Convert.ToInt32(gra.Key.Split(new char[] { ',' }).ToArray()[1]);

                            foreach (KeyValuePair<string, double> k in gra.Value)
                            {
                                if (dicCRCalculateValueFrom.ContainsKey(k.Key))
                                {
                                    CRCalculateValue CRCalculateValue = dicCRCalculateValueFrom[k.Key];
                                    anew.PointEstimate += CRCalculateValue.PointEstimate;
                                    anew.Mean += CRCalculateValue.Mean;
                                    anew.Variance += CRCalculateValue.Variance;
                                    anew.StandardDeviation += CRCalculateValue.StandardDeviation;
                                    if (CRCalculateValue.LstPercentile != null)
                                    {
                                        if (anew.LstPercentile == null) anew.LstPercentile = new List<float>();
                                        for (int iPercentile = 0; iPercentile < CRCalculateValue.LstPercentile.Count(); iPercentile++)
                                        {
                                            anew.LstPercentile[iPercentile] += CRCalculateValue.LstPercentile[iPercentile];
                                        }
                                    }
                                }
                            }
                            dicCRCalculateValue.Add(gra.Key, anew);
                        }

                        asvvnew.CRCalculateValues = dicCRCalculateValue.Values.ToList();
                    }
                }
                else
                {
                    if (gridRelationship.bigGridID == benMAPGrid.GridDefinitionID)
                    {


                        foreach (CRCalculateValue ava in cRSelectFunctionCalculateValue.CRCalculateValues)
                        {
                            icount = gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).Count();
                            if (icount > 0)
                            {
                                icount = gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).First().smallGridRowCol.Count();
                                foreach (RowCol rc in gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).First().smallGridRowCol)
                                {
                                    asvvnew.CRCalculateValues.Add(new CRCalculateValue()
                                    {
                                        Row = rc.Row,
                                        Col = rc.Col,
                                        PointEstimate = ava.PointEstimate / Convert.ToSingle(icount),
                                        Baseline = ava.Baseline / Convert.ToSingle(icount),
                                        Delta = ava.Delta / Convert.ToSingle(icount),
                                        Incidence = ava.Incidence / Convert.ToSingle(icount),
                                        PercentOfBaseline = ava.PercentOfBaseline,
                                        Population = ava.Population / Convert.ToSingle(icount),
                                        Mean = ava.Mean / Convert.ToSingle(icount),
                                        StandardDeviation = ava.StandardDeviation / Convert.ToSingle(icount),
                                        Variance = ava.Variance / Convert.ToSingle(icount),
                                    }
                                    );
                                    if (ava.LstPercentile != null)
                                    {
                                        asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].LstPercentile = new List<float>();
                                        foreach (float dLstPercentile in ava.LstPercentile)
                                        {
                                            asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].LstPercentile.Add(dLstPercentile / Convert.ToSingle(icount));
                                        }
                                    }
                                }
                            }



                        }


                    }
                    else
                    {
                        RowColComparer rowColComparer = new RowColComparer();
                        Dictionary<string, string> dicRowCol = new Dictionary<string, string>();
                        foreach (GridRelationshipAttribute gra in gridRelationship.lstGridRelationshipAttribute)
                        {
                            foreach (RowCol rc in gra.smallGridRowCol)
                            {
                                if (!dicRowCol.Keys.Contains(rc.Col + "," + rc.Row))
                                    dicRowCol.Add(rc.Col + "," + rc.Row, gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row);

                            }
                        }
                        Dictionary<string, CRCalculateValue> dicCRCalculateValue = new Dictionary<string, CRCalculateValue>();
                        CRCalculateValue anewfirst = new CRCalculateValue();
                        anewfirst.LstPercentile = new List<float>(); if (cRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile != null)
                        {
                            for (int iPercentile = 0; iPercentile < cRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count; iPercentile++)
                            {
                                anewfirst.LstPercentile.Add(0);
                            }
                        }
                        foreach (GridRelationshipAttribute gra in gridRelationship.lstGridRelationshipAttribute)
                        {
                            CRCalculateValue anew = new CRCalculateValue();
                            if (anewfirst.LstPercentile != null)
                            {
                                anew.LstPercentile = new List<float>();
                                foreach (float d in anewfirst.LstPercentile)
                                {
                                    anew.LstPercentile.Add(d);
                                }
                            }
                            anew.Col = gra.bigGridRowCol.Col;
                            anew.Row = gra.bigGridRowCol.Row;

                            dicCRCalculateValue.Add(gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row, anew);
                        }
                        foreach (CRCalculateValue ava in cRSelectFunctionCalculateValue.CRCalculateValues)
                        {
                            if (dicRowCol.Keys.Contains(ava.Col + "," + ava.Row))
                            {
                                CRCalculateValue CRCalculateValue = dicCRCalculateValue[dicRowCol[ava.Col + "," + ava.Row]];
                                CRCalculateValue.Baseline += ava.Baseline;
                                CRCalculateValue.Delta += ava.Delta;
                                CRCalculateValue.Incidence = Convert.ToSingle((CRCalculateValue.Incidence + ava.Incidence) / 2.0000);
                                CRCalculateValue.Population += ava.Population;
                                CRCalculateValue.Mean += ava.Mean;
                                CRCalculateValue.PointEstimate += ava.PointEstimate;
                                CRCalculateValue.StandardDeviation += ava.StandardDeviation;
                                CRCalculateValue.Variance += ava.Variance;
                                CRCalculateValue.PercentOfBaseline = CRCalculateValue.Baseline == 0 ? 0 : Convert.ToSingle((CRCalculateValue.Mean / CRCalculateValue.Baseline) * 100);
                                if (ava.LstPercentile != null)
                                {
                                    if (CRCalculateValue.LstPercentile == null)
                                        CRCalculateValue.LstPercentile = ava.LstPercentile;
                                    else
                                    {
                                        for (int iPercentile = 0; iPercentile < ava.LstPercentile.Count(); iPercentile++)
                                        {
                                            CRCalculateValue.LstPercentile[iPercentile] += ava.LstPercentile[iPercentile];
                                        }
                                    }

                                }
                            }

                        }
                        asvvnew.CRCalculateValues = dicCRCalculateValue.Values.ToList();







                    }
                }


            }
            catch
            { }
            return asvvnew;
        }


























































        public static void ApplyAggregationFromValuationMethodPoolingAndAggregation(List<GridRelationship> lstGridRelationshipAll, BenMAPGrid gBenMAPGrid, ref ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation)
        {
            try
            {
                int icount = 0;
                double d = 0;
                int idAggregation = -1;
                GridRelationship gridRelationship = null;
                GridRelationship gridRelationshipIncidence = null;
                if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
                       valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null &&
                       valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != gBenMAPGrid.GridDefinitionID && valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase != null)
                {
                    idAggregation = valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID;

                    if (lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).Count() > 0)
                    {
                        gridRelationship = lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).First();
                    }
                    else
                    {
                        gridRelationship = lstGridRelationshipAll.Where(p => p.bigGridID == idAggregation && p.smallGridID == gBenMAPGrid.GridDefinitionID).First();

                    }
                    gridRelationshipIncidence = null;
                    if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation != null)
                    {
                        idAggregation = valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID;
                        if (lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).Count() > 0)
                        {
                            gridRelationshipIncidence = lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).First();
                        }
                        else
                        {
                            gridRelationshipIncidence = lstGridRelationshipAll.Where(p => p.bigGridID == idAggregation && p.smallGridID == gBenMAPGrid.GridDefinitionID).First();

                        }
                    }

                    for (int ivb = 0; ivb < valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Count; ivb++)
                    {
                        ValuationMethodPoolingAndAggregationBase vb = valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[ivb];
                        if (vb.LstAllSelectValuationMethodAndValue != null)
                        {
                            vb.LstAllSelectValuationMethodAndValueAggregation = new List<AllSelectValuationMethodAndValue>();

                            foreach (AllSelectValuationMethodAndValue asvv in vb.LstAllSelectValuationMethodAndValue)
                            {

                                vb.LstAllSelectValuationMethodAndValueAggregation.Add(APVX.APVCommonClass.ApplyAllSelectValuationMethodAndValueAggregation(gridRelationship, CommonClass.GBenMAPGrid, asvv));

                            }

                        }
                        if (vb.IncidencePoolingResult != null && gridRelationshipIncidence != null)
                        {
                            vb.IncidencePoolingResultAggregation = APVX.APVCommonClass.ApplyCRSelectFunctionCalculateValueAggregation(gridRelationshipIncidence, CommonClass.GBenMAPGrid, vb.IncidencePoolingResult);






                        }

                    }
                }
                if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
                        valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation != null &&
                        valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID != gBenMAPGrid.GridDefinitionID && valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase != null)
                {
                    idAggregation = valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID;
                    if (lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).Count() > 0)
                    {
                        gridRelationship = lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).First();
                    }
                    else
                    {
                        gridRelationship = lstGridRelationshipAll.Where(p => p.bigGridID == idAggregation && p.smallGridID == gBenMAPGrid.GridDefinitionID).First();

                    }
                    for (int ivb = 0; ivb < valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase.Count; ivb++)
                    {
                        ValuationMethodPoolingAndAggregationBase vb = valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[ivb];
                        if (vb.lstAllSelectQALYMethodAndValue != null)
                        {
                            vb.lstAllSelectQALYMethodAndValueAggregation = new List<AllSelectQALYMethodAndValue>();

                            foreach (AllSelectQALYMethodAndValue asvv in vb.lstAllSelectQALYMethodAndValue)
                            {
                                vb.lstAllSelectQALYMethodAndValueAggregation.Add(APVX.APVCommonClass.ApplyAllSelectQALYMethodAndValueAggregation(gridRelationship, CommonClass.GBenMAPGrid, asvv));



                            }

                        }
                    }
                }



            }
            catch
            { }
        }











        // Distributions
        public static double[] getLHSArrayValuationFunctionSeed(int LatinHypercubePoints, BenMAPValuationFunction benMAPValuationFunction, int Seed)
        { 
            try
            {
                Meta.Numerics.Statistics.Sample sample = null;
                double[] lhsResultArray = new double[LatinHypercubePoints];
                switch (benMAPValuationFunction.DistA)
                {
                    case "None":
                        for (int i = 0; i < LatinHypercubePoints; i++)
                        {
                            lhsResultArray[i] = benMAPValuationFunction.A;

                        }
                        return lhsResultArray;
                        break;
                    case "Normal":

                        Meta.Numerics.Statistics.Distributions.Distribution Normal_distribution =
   new Meta.Numerics.Statistics.Distributions.NormalDistribution(benMAPValuationFunction.A, benMAPValuationFunction.P1A);
                        sample = CreateSample(Normal_distribution, CommonClass.IncidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations, Seed);
                        break;
                    case "Triangular":
                        Meta.Numerics.Statistics.Distributions.TriangularDistribution triangularDistribution =
    new Meta.Numerics.Statistics.Distributions.TriangularDistribution(benMAPValuationFunction.P1A, benMAPValuationFunction.P2A, benMAPValuationFunction.A);
                        sample = CreateSample(triangularDistribution, CommonClass.IncidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations, Seed);
                        break;
                    case "Poisson":
                        Meta.Numerics.Statistics.Distributions.PoissonDistribution poissonDistribution =
    new Meta.Numerics.Statistics.Distributions.PoissonDistribution(benMAPValuationFunction.P1A);
                        sample = CreateSample(poissonDistribution, CommonClass.IncidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations, Seed);
                        break;
                    case "Binomial":
                        Meta.Numerics.Statistics.Distributions.BinomialDistribution binomialDistribution =
    new Meta.Numerics.Statistics.Distributions.BinomialDistribution(benMAPValuationFunction.P1A, Convert.ToInt32(benMAPValuationFunction.P2A));
                        sample = CreateSample(binomialDistribution, CommonClass.IncidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations, Seed);
                        break;
                    case "LogNormal":
                        Meta.Numerics.Statistics.Distributions.LognormalDistribution lognormalDistribution =
    new Meta.Numerics.Statistics.Distributions.LognormalDistribution(benMAPValuationFunction.P1A, benMAPValuationFunction.P2A);
                        sample = CreateSample(lognormalDistribution, CommonClass.IncidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations, Seed);
                        break;
                    case "Uniform":
                        Interval interval = Interval.FromEndpoints(benMAPValuationFunction.P1A,
benMAPValuationFunction.P2A);

                        Meta.Numerics.Statistics.Distributions.UniformDistribution uniformDistribution =
                            new Meta.Numerics.Statistics.Distributions.UniformDistribution(interval);
                        sample = CreateSample(uniformDistribution, CommonClass.IncidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations, Seed);
                        break;
                    case "Exponential":
                        Meta.Numerics.Statistics.Distributions.ExponentialDistribution exponentialDistribution =
    new Meta.Numerics.Statistics.Distributions.ExponentialDistribution(benMAPValuationFunction.P1A);
                        sample = CreateSample(exponentialDistribution, CommonClass.IncidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations, Seed);
                        break;
                    case "Geometric":
                        Meta.Numerics.Statistics.Distributions.GeometricDistribution GeometricDistribution =
    new Meta.Numerics.Statistics.Distributions.GeometricDistribution(benMAPValuationFunction.P1A);
                        sample = CreateSample(GeometricDistribution, CommonClass.IncidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations, Seed);
                        break;
                    case "Weibull":
                        Meta.Numerics.Statistics.Distributions.WeibullDistribution WeibullDistribution =
    new Meta.Numerics.Statistics.Distributions.WeibullDistribution(benMAPValuationFunction.P1A, benMAPValuationFunction.P2A);
                        sample = CreateSample(WeibullDistribution, CommonClass.IncidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations, Seed);
                        break;
                    case "Gamma":
                        Meta.Numerics.Statistics.Distributions.GammaDistribution GammaDistribution =
    new Meta.Numerics.Statistics.Distributions.GammaDistribution(benMAPValuationFunction.P1A, benMAPValuationFunction.P2A);
                        sample = CreateSample(GammaDistribution, CommonClass.IncidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations, Seed);
                        break;
                    case "Logistic":
                        Meta.Numerics.Statistics.Distributions.Distribution logistic_distribution = new Meta.Numerics.Statistics.Distributions.LogisticDistribution(benMAPValuationFunction.P1A, benMAPValuationFunction.P2A);
                        sample = CreateSample(logistic_distribution, CommonClass.IncidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations);

                        break;
                    case "Beta":
                        Meta.Numerics.Statistics.Distributions.BetaDistribution BetaDistribution =
    new Meta.Numerics.Statistics.Distributions.BetaDistribution(benMAPValuationFunction.P1A, benMAPValuationFunction.P2A);
                        sample = CreateSample(BetaDistribution, CommonClass.IncidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations, Seed);
                        break;
                    case "Pareto":
                        Meta.Numerics.Statistics.Distributions.ParetoDistribution ParetoDistribution =
    new Meta.Numerics.Statistics.Distributions.ParetoDistribution(benMAPValuationFunction.P1A, benMAPValuationFunction.P2A);
                        sample = CreateSample(ParetoDistribution, CommonClass.IncidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations, Seed);
                        break;
                    case "Cauchy":
                        Meta.Numerics.Statistics.Distributions.CauchyDistribution CauchyDistribution =
    new Meta.Numerics.Statistics.Distributions.CauchyDistribution(benMAPValuationFunction.P1A, benMAPValuationFunction.P2A);
                        sample = CreateSample(CauchyDistribution, CommonClass.IncidencePoolingAndAggregationAdvance.DefaultMonteCarloIterations, Seed);
                        break;
                    case "Custom":
                        string commandText = string.Format("select   VValue  from ValuationFunctionCustomEntries where ValuationFunctionID={0} ", benMAPValuationFunction.ID);
                        ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

                        DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                        List<float> lstCustom = new List<float>();
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            lstCustom.Add(Convert.ToSingle(dr[0]));

                        }
                        lstCustom.Sort();
                        for (int i = 0; i < LatinHypercubePoints; i++)
                        {
                            lhsResultArray[i] = lstCustom.GetRange(i * (lstCustom.Count / LatinHypercubePoints), (lstCustom.Count / LatinHypercubePoints)).Median();
                        }
                        return lhsResultArray;


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
        public static PoolingMethodTypeEnum getPoolingMethodTypeEnumFromString(string PoolingMethod)
        {
            PoolingMethod = PoolingMethod.Replace(" ", "");
            PoolingMethodTypeEnum pme = new PoolingMethodTypeEnum();
            switch (PoolingMethod)
            {
                case "FixedEffects":
                    pme = PoolingMethodTypeEnum.FixedEffects;
                    break;
                case "RandomOrFixedEffects":
                    pme = PoolingMethodTypeEnum.RandomOrFixedEffects;
                    break;
                case "UserDefinedWeights":
                    pme = PoolingMethodTypeEnum.SubjectiveWeights;
                    break;
                case "SubtractionDependent":
                    pme = PoolingMethodTypeEnum.SubtractionDependent;
                    break;
                case "SubtractionIndependent":
                    pme = PoolingMethodTypeEnum.SubtractionIndependent;
                    break;
                case "SumDependent":
                    pme = PoolingMethodTypeEnum.SumDependent;
                    break;
                case "SumIndependent":
                    pme = PoolingMethodTypeEnum.SumIndependent;
                    break;

            }
            return pme;
        }
        public static void getAllChildMethodNotNone(AllSelectValuationMethod allSelectValueMethod, List<AllSelectValuationMethod> lstAll, ref List<AllSelectValuationMethod> lstReturn)
        {
            //Note that there is another similar function in IncidencePoolingandAggregation.cs called getAllChildMethodNotNone.
            //This function is only used for valuation
            List<AllSelectValuationMethod> lstOne = lstAll.Where(p => p.PID == allSelectValueMethod.ID && (p.PoolingMethod != "None" || p.NodeType == 100)).ToList();
            lstReturn.AddRange(lstOne);
            List<AllSelectValuationMethod> lstSec = lstAll.Where(p => p.PID == allSelectValueMethod.ID && (p.PoolingMethod == "None")).ToList();

            foreach (AllSelectValuationMethod asvm in lstSec)
            {
                getAllChildMethodNotNone(asvm, lstAll, ref lstReturn);

            }
        }

        public static void getAllChildCR(AllSelectCRFunction allSelectCRFunction, List<AllSelectCRFunction> lstAll, ref List<AllSelectCRFunction> lstReturn)
        {
            List<AllSelectCRFunction> lstOne = lstAll.Where(p => p.PID == allSelectCRFunction.ID).ToList();
            lstReturn.AddRange(lstOne);
            foreach (AllSelectCRFunction ascr in lstOne)
            {
                getAllChildCR(ascr, lstAll, ref lstReturn);

            }
        }
        public static void getAllChildVM(AllSelectValuationMethod allSelectValuationMethod, List<AllSelectValuationMethod> lstAll, ref List<AllSelectValuationMethod> lstReturn)
        {
            List<AllSelectValuationMethod> lstOne = lstAll.Where(p => p.PID == allSelectValuationMethod.ID).ToList();
            lstReturn.AddRange(lstOne);
            foreach (AllSelectValuationMethod asvm in lstOne)
            {
                getAllChildVM(asvm, lstAll, ref lstReturn);

            }
        }

        public static void getAllChildCRNotNone(AllSelectCRFunction allSelectCRFunction, List<AllSelectCRFunction> lstAll, ref List<AllSelectCRFunction> lstReturn)
        {


            //YY: get all child and subchild items which  either (1) are not pooled individuals or (2) are pooled groups.
            List<AllSelectCRFunction> lstOne = lstAll.Where(p => p.PID == allSelectCRFunction.ID).ToList();
            if (allSelectCRFunction.PoolingMethod == "None" || allSelectCRFunction.PoolingMethod == "") //YY: new
            {
                lstReturn.AddRange(lstOne);

                foreach (AllSelectCRFunction asvm in lstOne)
                {
                    getAllChildCRNotNone(asvm, lstAll, ref lstReturn);

                }
            }
            else
            {

            }
        }

        public static void getAllChildCRNotNoneForPooling(AllSelectCRFunction allSelectCRFunction, List<AllSelectCRFunction> lstAll, ref List<AllSelectCRFunction> lstReturn)
        {

            List<AllSelectCRFunction> lstOne = lstAll.Where(p => p.PID == allSelectCRFunction.ID).ToList();
            if (allSelectCRFunction.PoolingMethod == "None")
            {
                lstReturn.AddRange(lstOne);

                foreach (AllSelectCRFunction asvm in lstOne)
                {
                    getAllChildCRNotNoneForPooling(asvm, lstAll, ref lstReturn);

                }
            }
            else
            {
                lstReturn.Add(allSelectCRFunction);

            }
        }

        public static void getAllChildCRFunctions(AllSelectCRFunction allSelectCRFunction, List<AllSelectCRFunction> lstAll, ref List<AllSelectCRFunction> lstReturn)
        {
            //YY: get all child CR functions. Added to calculate pooled population.
            List<AllSelectCRFunction> lstOne = lstAll.Where(p => p.PID == allSelectCRFunction.ID).ToList();
            if (allSelectCRFunction.NodeType != 100)
            {
                foreach (AllSelectCRFunction asvm in lstOne)
                {
                    getAllChildCRFunctions(asvm, lstAll, ref lstReturn);

                }
            }
            else
            {
                lstReturn.Add(allSelectCRFunction);

            }
        }
        public static void getAllChildCRNotNoneCalulate(AllSelectCRFunction allSelectCRFunction, List<AllSelectCRFunction> lstAll, ref List<AllSelectCRFunction> lstReturn)
        {
            //return all child and sub-child items of this allSelectCRFunction, these items are either pooled groups or individual studies. 

            //List<AllSelectCRFunction> lstOne = lstAll.Where(p => p.PID == allSelectCRFunction.ID && (p.PoolingMethod != "None" || p.NodeType == 100)).ToList();
            List<AllSelectCRFunction> lstOne = lstAll.Where(p => p.PID == allSelectCRFunction.ID && ((p.PoolingMethod != "None" && p.PoolingMethod != "") || p.NodeType == 100)).ToList(); //YY:
            lstReturn.AddRange(lstOne);
            List<AllSelectCRFunction> lstSec = lstAll.Where(p => p.PID == allSelectCRFunction.ID).ToList();


            foreach (AllSelectCRFunction asvm in lstSec)
            {
                if (asvm.PoolingMethod == "None" || asvm.PoolingMethod == "") //YY:
                    getAllChildCRNotNoneCalulate(asvm, lstAll, ref lstReturn);

            }

        }

        private static void getAllChildQALYMethodNotNone(AllSelectQALYMethod allSelectValueMethod, List<AllSelectQALYMethod> lstAll, ref List<AllSelectQALYMethod> lstReturn)
        {
            List<AllSelectQALYMethod> lstOne = lstAll.Where(p => p.PID == allSelectValueMethod.ID && (p.PoolingMethod != "None" || p.NodeType == 5)).ToList();
            lstReturn.AddRange(lstOne);
            List<AllSelectQALYMethod> lstSec = lstAll.Where(p => p.PID == allSelectValueMethod.ID && (p.PoolingMethod == "None")).ToList();

            foreach (AllSelectQALYMethod asvm in lstSec)
            {
                getAllChildQALYMethodNotNone(asvm, lstAll, ref lstReturn);

            }
        }
        public static void CalculateValuationMethodPoolingAndAggregation(ref ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation)
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commondText = "";
                double AllGoodsIndex = 0;
                double MedicalCostIndex = 0;
                double WageIndex = 0;
                System.Data.DataSet ds = null;
                List<double> lstWeight = new List<double>();
                int i = 0;
                List<RowCol> lstAllRowCol = getAllRowColFromGridType(CommonClass.GBenMAPGrid);
                Dictionary<string, double> dicIncome = new Dictionary<string, double>();
                if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
                    valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.InflationDatasetID != -1
                    && valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.CurrencyYear != -1)
                {
                    getInflationFromDataSetIDAndYear(valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.InflationDatasetID,
                        valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.CurrencyYear, ref AllGoodsIndex, ref MedicalCostIndex, ref WageIndex);
                }
                else
                {
                    commondText = string.Format("select InflationDataSetID from InflationDataSets where SetupID={0} order by InflationDataSetID", CommonClass.MainSetup.SetupID);
                    ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commondText);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        getInflationFromDataSetIDAndYear(Convert.ToInt32(ds.Tables[0].Rows[0][0]),
                      CommonClass.BenMAPPopulation.Year, ref AllGoodsIndex, ref MedicalCostIndex, ref WageIndex);


                    }

                }
                if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
    valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.AdjustIncomeGrowthDatasetID != -1
    && valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncomeGrowthYear != -1)
                {
                    dicIncome = getIncomeGrowthFactorsFromDataSetIDAndYear(valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.AdjustIncomeGrowthDatasetID,
                        valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncomeGrowthYear);
                }




                foreach (ValuationMethodPoolingAndAggregationBase valuationMethodPoolingAndAggregationBase in valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                {
                    valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue = new List<AllSelectValuationMethodAndValue>();


                    List<AllSelectValuationMethod> queryFirst = valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod.Where(a => valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod.Where(c => c.NodeType == 2000).Select(p => p.PID).Contains(a.ID)).ToList();
                    if (queryFirst.Count() > 0)
                    {
                        foreach (AllSelectValuationMethod avmFirst in queryFirst)
                        {
                            try
                            {
                                AllSelectCRFunction allsCRFunction = valuationMethodPoolingAndAggregationBase.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.CRID == avmFirst.CRID).First();
                                CRSelectFunctionCalculateValue crv = allsCRFunction.CRSelectFunctionCalculateValue;




















                                var queryLeaf = valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod.Where(a => a.PID == avmFirst.ID).ToList();
                                List<AllSelectValuationMethodAndValue> lstTemp = new List<AllSelectValuationMethodAndValue>();

                                for (int iqueryLeaf = 0; iqueryLeaf < queryLeaf.Count(); iqueryLeaf++)
                                {
                                    AllSelectValuationMethod avmLeaf = queryLeaf[iqueryLeaf];
                                    CRSelectFunctionCalculateValue crvCal = null;





                                    crvCal = crv;

                                    if (crvCal != null)
                                    {
                                        AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = getOneAllSelectValuationMethodCRSelectFunctionCalculateValue(crvCal,
                                             ref avmLeaf, AllGoodsIndex, MedicalCostIndex, WageIndex, dicIncome);
                                        GC.Collect();
                                        allSelectValuationMethodAndValue.AllSelectValuationMethod = avmLeaf;
                                        if (avmFirst.PoolingMethod != "None")
                                        {
                                            lstTemp.Add(allSelectValuationMethodAndValue);
                                        }
                                        valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Add(allSelectValuationMethodAndValue);

                                    }


                                }
                                if (lstTemp.Count > 0)
                                {
                                    AllSelectValuationMethodAndValue allSelectValuationMethodAndValueFirst = getPoolingLstAllSelectValuationMethodAndValue(lstTemp, getPoolingMethodTypeEnumFromString(avmFirst.PoolingMethod), queryLeaf.Select(p => p.Weight).ToList());
                                    allSelectValuationMethodAndValueFirst.AllSelectValuationMethod = avmFirst;
                                    valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Add(allSelectValuationMethodAndValueFirst);
                                    GC.Collect();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message+"\n"+ex.StackTrace);
                            }


                        }

                    }
                    var querySecond = valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod.Where(a => queryFirst.Select(p => p.PID).Contains(a.ID));
                    if (querySecond.Count() > 0)
                    {
                        foreach (AllSelectValuationMethod avmSecond in querySecond)
                        {
                            if (avmSecond.PoolingMethod != "None")
                            {
                                List<AllSelectValuationMethod> lstChild = new List<AllSelectValuationMethod>();
                                getAllChildMethodNotNone(avmSecond, valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod, ref lstChild);
                                List<int> lstID = lstChild.Select(p => p.ID).ToList();
                                AllSelectValuationMethodAndValue allSelectValuationMethodAndValueSec = getPoolingLstAllSelectValuationMethodAndValue(valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Where(p => lstID.Contains(p.AllSelectValuationMethod.ID)).ToList(), getPoolingMethodTypeEnumFromString(avmSecond.PoolingMethod), valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Where(p => lstID.Contains(p.AllSelectValuationMethod.ID)).ToList().Select(p => p.AllSelectValuationMethod.Weight).ToList());
                                var query = valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Where(p => lstID.Contains(p.AllSelectValuationMethod.ID)).ToList();
                                foreach (AllSelectValuationMethodAndValue alsv in query)
                                {
                                    valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Remove(alsv);

                                }
                                allSelectValuationMethodAndValueSec.AllSelectValuationMethod = avmSecond;
                                valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Add(allSelectValuationMethodAndValueSec);
                                GC.Collect();
                            }
                        }
                    }
                    List<AllSelectValuationMethod> queryThree = valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod.Where(a => querySecond.Select(p => p.PID).Contains(a.ID)).ToList();
                    while (queryThree.Count() > 0)
                    {
                        foreach (AllSelectValuationMethod avmThree in queryThree)
                        {

                            if (avmThree.PoolingMethod != "None")
                            {
                                List<AllSelectValuationMethod> lstChild = new List<AllSelectValuationMethod>();
                                getAllChildMethodNotNone(avmThree, valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod, ref lstChild);
                                List<int> lstID = lstChild.Select(p => p.ID).ToList();
                                AllSelectValuationMethodAndValue allSelectValuationMethodAndValueSec = getPoolingLstAllSelectValuationMethodAndValue(valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Where(p => lstID.Contains(p.AllSelectValuationMethod.ID)).ToList(), getPoolingMethodTypeEnumFromString(avmThree.PoolingMethod), valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Where(p => lstID.Contains(p.AllSelectValuationMethod.ID)).ToList().Select(p => p.AllSelectValuationMethod.Weight).ToList());
                                allSelectValuationMethodAndValueSec.AllSelectValuationMethod = avmThree;
                                var query = valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Where(p => lstID.Contains(p.AllSelectValuationMethod.ID)).ToList();
                                foreach (AllSelectValuationMethodAndValue alsv in query)
                                {
                                    valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Remove(alsv);

                                }
                                valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Add(allSelectValuationMethodAndValueSec);
                            }
                            GC.Collect();
                        }
                        queryThree = valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod.Where(a => queryThree.Select(p => p.PID).Contains(a.ID)).ToList();
                    }



                }
                dicIncome = null;
                lstAllRowCol = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

        }
        public static Dictionary<string, Dictionary<string, double>> getRelationFromDicRelationShipAll(GridRelationship gridRelationShipPopulation)
        {

            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            Dictionary<string, Dictionary<string, double>> dicRelationShip = new Dictionary<string, Dictionary<string, double>>();
            try
            {
                string str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", gridRelationShipPopulation.smallGridID, gridRelationShipPopulation.bigGridID);

                if (gridRelationShipPopulation.smallGridID == 28)
                {
                    str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", 27, gridRelationShipPopulation.bigGridID);
                }
                if (APVX.APVCommonClass.DicRelationShipAll.ContainsKey(gridRelationShipPopulation.smallGridID + "," + gridRelationShipPopulation.bigGridID))
                {
                    dicRelationShip = APVX.APVCommonClass.DicRelationShipAll[gridRelationShipPopulation.smallGridID + "," + gridRelationShipPopulation.bigGridID];
                }
                else
                {
                    DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        Configuration.ConfigurationCommonClass.creatPercentageToDatabase(gridRelationShipPopulation.bigGridID, gridRelationShipPopulation.smallGridID == 28 ? 27 : gridRelationShipPopulation.smallGridID,null);
                        ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
                    }
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (dicRelationShip.ContainsKey(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()))
                        {
                            if (!dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
                                dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                        }
                        else
                        {
                            dicRelationShip.Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), new Dictionary<string, double>());
                            dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                        }

                    }
                    APVX.APVCommonClass.DicRelationShipAll.Add(gridRelationShipPopulation.smallGridID + "," + gridRelationShipPopulation.bigGridID, dicRelationShip);
                    ds.Dispose();
                }
            }
            catch
            {
            }
            return dicRelationShip;
        }
        public static Dictionary<string, Dictionary<string, Dictionary<string, double>>> DicRelationShipAll = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();

        public static CRSelectFunctionCalculateValue ApplyAggregationCRSelectFunctionCalculateValue(CRSelectFunctionCalculateValue crSelectFunctionCalculateValueFrom, int GridFrom, int GridTo)
        {
            CRSelectFunctionCalculateValue crOut = new CRSelectFunctionCalculateValue();
            crOut.CRSelectFunction = crSelectFunctionCalculateValueFrom.CRSelectFunction;
            crOut.CRCalculateValues = new List<CRCalculateValue>();
            if (GridFrom == GridTo) return crSelectFunctionCalculateValueFrom;
            try
            {
                GridRelationship gridRelationship = null;
                try
                {
                    if (CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == GridTo && p.smallGridID == GridFrom).Count() > 0)
                    {
                        gridRelationship = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == GridTo && p.smallGridID == GridFrom).First();
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

                if (gridRelationship == null)
                {
                    Configuration.ConfigurationCommonClass.creatPercentageToDatabase(GridTo, GridFrom,null);
                    if (CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == GridTo && p.smallGridID == GridFrom).Count() > 0)
                    {
                        gridRelationship = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == GridTo && p.smallGridID == GridFrom).First();
                    }
                }

                string str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", gridRelationship.smallGridID, gridRelationship.bigGridID);
                if (gridRelationship.smallGridID == GridFrom)
                {
                    str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", GridFrom, gridRelationship.bigGridID);
                }
                if ((GridFrom == 28 || gridRelationship.smallGridID == 28) || (GridFrom == 27 || gridRelationship.smallGridID == 27))
                {
                    str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", 27, gridRelationship.bigGridID);
                    if (GridFrom == 28) GridFrom = 27;
                    if (GridTo == 28) GridTo = 27;
                }
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds; int iCount = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, "select count(*) from (" + str + " ) a"));
                if (iCount == 0)
                {

                    Configuration.ConfigurationCommonClass.creatPercentageToDatabase(GridTo, GridFrom,null);
                    iCount = 1;

                }
                if (iCount != 0)       
                {
                    Dictionary<string, Dictionary<string, double>> dicRelationShip = new Dictionary<string, Dictionary<string, double>>();
                    if (DicRelationShipAll.ContainsKey(GridFrom + "," + GridTo))
                    {
                        dicRelationShip = DicRelationShipAll[GridFrom + "," + GridTo];
                    }
                    else
                    {
                        ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);

                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            if (dicRelationShip.ContainsKey(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()))
                            {
                                if (!dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
                                    dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                            }
                            else
                            {
                                dicRelationShip.Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), new Dictionary<string, double>());
                                dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                            }

                        }
                        DicRelationShipAll.Add(GridFrom + "," + GridTo, dicRelationShip);
                        ds.Dispose();
                    }

                    if (gridRelationship.bigGridID == GridFrom)
                    {
                        Dictionary<string, CRCalculateValue> dicAggregation = new Dictionary<string, CRCalculateValue>();
                        foreach (CRCalculateValue ava in crSelectFunctionCalculateValueFrom.CRCalculateValues)
                        {
                            if (dicRelationShip.ContainsKey(ava.Col + "," + ava.Row))
                            {
                                double d = dicRelationShip[ava.Col + "," + ava.Row].Sum(p => p.Value);
                                foreach (KeyValuePair<string, double> k in dicRelationShip[ava.Col + "," + ava.Row])
                                {
                                    CRCalculateValue anew = new CRCalculateValue();
                                    if (dicAggregation.ContainsKey(k.Key))
                                    {
                                        anew = dicAggregation[k.Key];
                                        anew = crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1];
                                        anew.PointEstimate += ava.PointEstimate * Convert.ToSingle(k.Value) / Convert.ToSingle(d);
                                        anew.Population += ava.Population * Convert.ToSingle(k.Value) / Convert.ToSingle(d);
                                        anew.Incidence += ava.Incidence * Convert.ToSingle(k.Value);
                                        anew.Baseline += ava.Baseline * Convert.ToSingle(k.Value) / Convert.ToSingle(d);
                                        anew.Delta += ava.Delta * Convert.ToSingle(k.Value);
                                        if (ava.LstPercentile != null)
                                        {
                                            for (int iavaLst = 0; iavaLst < ava.LstPercentile.Count; iavaLst++)
                                            {
                                                float dLstPercentile = ava.LstPercentile[iavaLst];
                                                anew.LstPercentile[iavaLst] += dLstPercentile * Convert.ToSingle(k.Value) / Convert.ToSingle(d);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        crOut.CRCalculateValues.Add(new CRCalculateValue()
                                        {

                                            Col = Convert.ToInt32(k.Key.Split(new char[] { ',' }).ToArray()[0]),
                                            Row = Convert.ToInt32(k.Key.Split(new char[] { ',' }).ToArray()[1]),
                                            PointEstimate = ava.PointEstimate * Convert.ToSingle(k.Value) / Convert.ToSingle(d),
                                            Population = ava.Population * Convert.ToSingle(k.Value) / Convert.ToSingle(d),
                                            Incidence = ava.Incidence,
                                            Baseline = ava.Baseline * Convert.ToSingle(k.Value) / Convert.ToSingle(d),
                                            Delta = ava.Delta,


                                        }

                                         );
                                        dicAggregation.Add(k.Key, crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1]);
                                        anew = crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1];
                                        if (ava.LstPercentile != null)
                                        {
                                            anew.LstPercentile = new List<float>();
                                            foreach (float dLstPercentile in ava.LstPercentile)
                                            {
                                                anew.LstPercentile.Add(dLstPercentile * Convert.ToSingle(k.Value) / Convert.ToSingle(d));
                                            }
                                        }
                                    }

                                    if (anew.LstPercentile != null && anew.LstPercentile.Count > 0)
                                    {
                                        anew.Mean = Configuration.ConfigurationCommonClass.getMean(anew.LstPercentile);
                                        anew.Variance = Configuration.ConfigurationCommonClass.getVariance(anew.LstPercentile, anew.PointEstimate);
                                        anew.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(anew.LstPercentile, anew.PointEstimate);
                                    }

                                    anew.PercentOfBaseline = anew.Baseline == 0 ? 0 : Convert.ToSingle(Math.Round((anew.Mean / anew.Baseline) * 100, 4));
                                }
                            }
                        }
                        dicAggregation.Clear();
                        dicAggregation = null;
                    }
                    else       
                    {
                        Dictionary<string, CRCalculateValue> dicCRCalculateValue = new Dictionary<string, CRCalculateValue>();
                        CRCalculateValue anewfirst = new CRCalculateValue();
                        anewfirst.LstPercentile = new List<float>(); if (crSelectFunctionCalculateValueFrom.CRCalculateValues.First().LstPercentile != null)
                        {
                            for (int iPercentile = 0; iPercentile < crSelectFunctionCalculateValueFrom.CRCalculateValues.First().LstPercentile.Count; iPercentile++)
                            {
                                anewfirst.LstPercentile.Add(0);
                            }
                        }
                        Dictionary<string, CRCalculateValue> dicCRCalculateValueFrom = new Dictionary<string, CRCalculateValue>();
                        foreach (CRCalculateValue ava in crSelectFunctionCalculateValueFrom.CRCalculateValues)
                        {
                            dicCRCalculateValueFrom.Add(ava.Col + "," + ava.Row, ava);
                        }
                        foreach (KeyValuePair<string, Dictionary<string, double>> gra in dicRelationShip)
                        {
                            CRCalculateValue anew = new CRCalculateValue();
                            if (anewfirst.LstPercentile != null)
                            {
                                anew.LstPercentile = new List<float>();
                                foreach (float d in anewfirst.LstPercentile)
                                {
                                    anew.LstPercentile.Add(d);
                                }
                            }
                            anew.Col = Convert.ToInt32(gra.Key.Split(new char[] { ',' }).ToArray()[0]);
                            anew.Row = Convert.ToInt32(gra.Key.Split(new char[] { ',' }).ToArray()[1]);

                            foreach (KeyValuePair<string, double> k in gra.Value)
                            {
                                if (dicCRCalculateValueFrom.ContainsKey(k.Key))
                                {
                                    CRCalculateValue CRCalculateValue = dicCRCalculateValueFrom[k.Key];
                                    anew.PointEstimate += CRCalculateValue.PointEstimate * Convert.ToSingle(k.Value);
                                    anew.Population += CRCalculateValue.Population * Convert.ToSingle(k.Value);

                                    if (!float.IsNaN(CRCalculateValue.Incidence))
                                    {
                                        anew.Incidence = (anew.Incidence + CRCalculateValue.Incidence * Convert.ToSingle(k.Value));

                                    }
                                    anew.Baseline += CRCalculateValue.Baseline * Convert.ToSingle(k.Value);
                                    anew.Delta = (anew.Delta + CRCalculateValue.Delta * Convert.ToSingle(k.Value) * CRCalculateValue.Population);
                                    if (float.IsNaN(anew.Delta)) anew.Delta = 0;
                                    if (CRCalculateValue.LstPercentile != null && CRCalculateValue.LstPercentile.Count()!=0)
                                    {
                                        if (anew.LstPercentile == null) anew.LstPercentile = new List<float>();
                                        for (int iPercentile = 0; iPercentile < CRCalculateValue.LstPercentile.Count(); iPercentile++)
                                        {
                                            anew.LstPercentile[iPercentile] += CRCalculateValue.LstPercentile[iPercentile] * Convert.ToSingle(k.Value);
                                        }
                                    }
                                }
                            }
                            if (anew.LstPercentile != null && anew.LstPercentile.Count > 0)
                            {
                                anew.Mean = Configuration.ConfigurationCommonClass.getMean(anew.LstPercentile);
                                anew.Variance = Configuration.ConfigurationCommonClass.getVariance(anew.LstPercentile, anew.PointEstimate);
                                anew.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(anew.LstPercentile, anew.PointEstimate);
                            }
                            else
                            {
                                anew.Mean = float.NaN;
                                anew.Variance = float.NaN;
                                anew.StandardDeviation = float.NaN;
                            }
                            anew.Incidence = anew.Incidence / gra.Value.Count;
                            anew.Delta = anew.Delta / anew.Population;
                            if (float.IsNaN(anew.Delta)) anew.Delta = 0;
                            anew.PercentOfBaseline = anew.Baseline == 0 ? 0 : Convert.ToSingle(Math.Round((float.IsNaN(anew.Mean) ? anew.PointEstimate : anew.Mean / anew.Baseline) * 100, 4));
                            dicCRCalculateValue.Add(gra.Key, anew);
                        }

                        crOut.CRCalculateValues = dicCRCalculateValue.Values.Distinct().ToList();
                        dicCRCalculateValue.Clear();
                        dicCRCalculateValue = null;
                        dicCRCalculateValueFrom.Clear();
                        dicCRCalculateValueFrom = null;

                    }

                }
                else    
                {
                    Dictionary<string, CRCalculateValue> dicCRCalculateValue = new Dictionary<string, CRCalculateValue>();
                    List<string> lstSmallColRow = new List<string>();
                    crOut.CRCalculateValues = new List<CRCalculateValue>();
                    crOut.CRSelectFunction = crSelectFunctionCalculateValueFrom.CRSelectFunction;
                    foreach (CRCalculateValue cr in crSelectFunctionCalculateValueFrom.CRCalculateValues)
                    {
                        if (!dicCRCalculateValue.ContainsKey(cr.Col + "," + cr.Row))
                        {
                            dicCRCalculateValue.Add(cr.Col + "," + cr.Row, cr);
                        }
                    }
                    if (CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == GridFrom && p.smallGridID == GridTo).Count() > 0)
                    {
                        foreach (GridRelationshipAttribute gr in gridRelationship.lstGridRelationshipAttribute)
                        {
                            if (gr.smallGridRowCol != null && gr.smallGridRowCol.Count > 0 && dicCRCalculateValue.ContainsKey(gr.bigGridRowCol.Col + "," + gr.bigGridRowCol.Row))
                            {
                                CRCalculateValue crin = dicCRCalculateValue[gr.bigGridRowCol.Col + "," + gr.bigGridRowCol.Row];
                                foreach (RowCol rc in gr.smallGridRowCol)
                                {
                                    if (!lstSmallColRow.Contains(rc.Col + "," + rc.Row))
                                    {
                                        lstSmallColRow.Add(rc.Col + "," + rc.Row);
                                        crOut.CRCalculateValues.Add(new CRCalculateValue()
                                        {
                                            Col = rc.Col,
                                            Row = rc.Row,
                                            Baseline = crin.Baseline / gr.smallGridRowCol.Count,
                                            Delta = crin.Delta,
                                            Incidence = crin.Incidence,
                                            Mean = crin.Mean / gr.smallGridRowCol.Count,
                                            PercentOfBaseline = crin.PercentOfBaseline,
                                            PointEstimate = crin.PointEstimate / gr.smallGridRowCol.Count,
                                            Population = crin.Population / gr.smallGridRowCol.Count,
                                            StandardDeviation = crin.StandardDeviation / gr.smallGridRowCol.Count,
                                            Variance = crin.Variance / gr.smallGridRowCol.Count,
                                        });
                                        if (crin.LstPercentile != null && crin.LstPercentile.Count > 0)
                                        {
                                            crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].LstPercentile = new List<float>();
                                            foreach (float f in crin.LstPercentile)
                                            {
                                                crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].LstPercentile.Add(f / gr.smallGridRowCol.Count);

                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        foreach (GridRelationshipAttribute gr in gridRelationship.lstGridRelationshipAttribute)
                        {
                            if (gr.smallGridRowCol != null && gr.smallGridRowCol.Count > 0)
                            {
                                crOut.CRCalculateValues.Add(new CRCalculateValue()
                                {
                                    Col = gr.bigGridRowCol.Col,
                                    Row = gr.bigGridRowCol.Row,

                                }
                                );
                                foreach (RowCol rc in gr.smallGridRowCol)
                                {

                                    if (dicCRCalculateValue.ContainsKey(rc.Col + "," + rc.Row))
                                    {
                                        CRCalculateValue crsmall = dicCRCalculateValue[rc.Col + "," + rc.Row];
                                        crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].Baseline += crsmall.Baseline;
                                        crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].Delta = Convert.ToSingle((crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].Delta + crsmall.Delta) / 2.000);
                                        crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].Incidence += Convert.ToSingle((crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].Delta + crsmall.Incidence) / 2.000);
                                        crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].Mean += crsmall.Mean;
                                        crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].PointEstimate += crsmall.PointEstimate;
                                        crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].Population += crsmall.Population;
                                        crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].StandardDeviation += crsmall.StandardDeviation;
                                        if (crsmall.LstPercentile != null && crsmall.LstPercentile.Count > 0)
                                        {
                                            if (crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].LstPercentile == null)
                                            {
                                                crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].LstPercentile = new List<float>();
                                                foreach (float f in crsmall.LstPercentile)
                                                {
                                                    crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].LstPercentile.Add(f);
                                                }

                                            }
                                            else
                                            {
                                                for (int i = 0; i < crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].LstPercentile.Count; i++)
                                                {
                                                    crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].LstPercentile[i] += crsmall.LstPercentile[i];

                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return crOut;
        }
        public static void CalculateQALYMethodPoolingAndAggregation(ref ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation)
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commondText = "";

                List<double> lstWeight = new List<double>();
                int i = 0;
                List<RowCol> lstAllRowCol = getAllRowColFromGridType(CommonClass.GBenMAPGrid);
                foreach (ValuationMethodPoolingAndAggregationBase valuationMethodPoolingAndAggregationBase in valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                {
                    if (valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethod == null || valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethod.Count == 0)
                        continue;
                    valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue = new List<AllSelectQALYMethodAndValue>();
                    var queryFirst = valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethod.Where(a => valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethod.Where(c => c.NodeType == 3000).Select(p => p.PID).Contains(a.ID));
                    if (queryFirst.Count() > 0)
                    {
                        foreach (AllSelectQALYMethod avmFirst in queryFirst)
                        {
                            AllSelectCRFunction allsCRFunction = valuationMethodPoolingAndAggregationBase.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.CRID == avmFirst.CRID).First();
                            CRSelectFunctionCalculateValue crv = allsCRFunction.CRSelectFunctionCalculateValue;
                            CRSelectFunctionCalculateValue crvCal = crv;






















                            var queryLeaf = valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethod.Where(a => a.PID == avmFirst.ID).ToList();
                            List<AllSelectQALYMethodAndValue> lstTemp = new List<AllSelectQALYMethodAndValue>();
                            for (int iqueryLeaf = 0; iqueryLeaf < queryLeaf.Count(); iqueryLeaf++)
                            {
                                AllSelectQALYMethod avmLeaf = queryLeaf[iqueryLeaf];
                                if (crvCal != null)
                                {
                                    AllSelectQALYMethodAndValue AllSelectQALYMethodAndValue = getOneAllSelectQALYMethodCRSelectFunctionCalculateValue(crvCal,
                                        ref  avmLeaf);
                                    AllSelectQALYMethodAndValue.AllSelectQALYMethod = avmLeaf;
                                    if (avmFirst.PoolingMethod != "None")
                                    {
                                        lstTemp.Add(AllSelectQALYMethodAndValue);
                                    }
                                    valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Add(AllSelectQALYMethodAndValue);

                                }


                            }
                            if (lstTemp.Count > 0)
                            {
                                AllSelectQALYMethodAndValue AllSelectQALYMethodAndValueFirst = getPoolingLstAllSelectQALYMethodAndValue(lstTemp, getPoolingMethodTypeEnumFromString(avmFirst.PoolingMethod), queryLeaf.Select(p => p.Weight).ToList());
                                AllSelectQALYMethodAndValueFirst.AllSelectQALYMethod = avmFirst;
                                valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Add(AllSelectQALYMethodAndValueFirst);
                            }


                        }
                    }
                    var querySecond = valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethod.Where(a => queryFirst.Select(p => p.PID).Contains(a.ID));
                    if (querySecond.Count() > 0)
                    {
                        foreach (AllSelectQALYMethod avmSecond in querySecond)
                        {
                            if (avmSecond.PoolingMethod != "None")
                            {
                                List<AllSelectQALYMethod> lstChild = new List<AllSelectQALYMethod>();
                                getAllChildQALYMethodNotNone(avmSecond, valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethod, ref lstChild);
                                List<int> lstID = lstChild.Select(p => p.ID).ToList();
                                AllSelectQALYMethodAndValue AllSelectQALYMethodAndValueSec = getPoolingLstAllSelectQALYMethodAndValue(valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Where(p => lstID.Contains(p.AllSelectQALYMethod.ID)).ToList(), getPoolingMethodTypeEnumFromString(avmSecond.PoolingMethod), valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Where(p => lstID.Contains(p.AllSelectQALYMethod.ID)).ToList().Select(p => p.AllSelectQALYMethod.Weight).ToList());
                                AllSelectQALYMethodAndValueSec.AllSelectQALYMethod = avmSecond;
                                valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Add(AllSelectQALYMethodAndValueSec);
                            }
                        }
                    }

                    List<AllSelectQALYMethod> queryThree = valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethod.Where(a => querySecond.Select(p => p.PID).Contains(a.ID)).ToList();
                    while (queryThree.Count() > 0)
                    {
                        foreach (AllSelectQALYMethod avmThree in queryThree)
                        {

                            if (avmThree.PoolingMethod != "None")
                            {
                                List<AllSelectQALYMethod> lstChild = new List<AllSelectQALYMethod>();
                                getAllChildQALYMethodNotNone(avmThree, valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethod, ref lstChild);
                                List<int> lstID = lstChild.Select(p => p.ID).ToList();
                                AllSelectQALYMethodAndValue AllSelectQALYMethodAndValueSec = getPoolingLstAllSelectQALYMethodAndValue(valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Where(p => lstID.Contains(p.AllSelectQALYMethod.ID)).ToList(), getPoolingMethodTypeEnumFromString(avmThree.PoolingMethod), valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Where(p => lstID.Contains(p.AllSelectQALYMethod.ID)).ToList().Select(p => p.AllSelectQALYMethod.Weight).ToList());
                                AllSelectQALYMethodAndValueSec.AllSelectQALYMethod = avmThree;
                                valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Add(AllSelectQALYMethodAndValueSec);
                            }
                        }
                        queryThree = valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethod.Where(a => queryThree.Select(p => p.PID).Contains(a.ID)).ToList();
                    }






                }
                GC.Collect();
            }
            catch (Exception ex)
            {

            }

        }


        public static AllSelectQALYMethodAndValue getOneAllSelectQALYMethodCRSelectFunctionCalculateValue(CRSelectFunctionCalculateValue crSelectFunctionCalculateValue, ref AllSelectQALYMethod allSelectQALYMethod)
        {
            try
            {
                AllSelectQALYMethodAndValue allSelectQALYMethodAndValue = new AllSelectQALYMethodAndValue();
                allSelectQALYMethodAndValue.AllSelectQALYMethod = allSelectQALYMethod;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = "";

                List<GridRelationship> lstGridRelationshipAll = CommonClass.LstGridRelationshipAll;
                double dQALY = 0.0;
                List<float> lstQALY = new List<float>();

                List<float> lstQALYTemp = new List<float>();
                if (allSelectQALYMethod.lstQALYLast == null || allSelectQALYMethod.lstQALYLast.Count == 0)
                {
                    allSelectQALYMethod.lstQALYLast = new List<float>();

                    commandText = string.Format("select qaly from QALYENTRIES where QALYDataSetID={0} and StartAge={1} and EndAge={2}", allSelectQALYMethod.BenMAPQALY.QalyDatasetID, allSelectQALYMethod.BenMAPQALY.StartAge, allSelectQALYMethod.BenMAPQALY.EndAge);
                    DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        lstQALY.Add(Convert.ToSingle(dr["qaly"]));
                    }
                    ds = null;
                    GC.Collect();
                    allSelectQALYMethod.fQALYFirst = Convert.ToSingle(lstQALY.First());

                    lstQALY.Sort();
                    allSelectQALYMethod.lstQALYLast = lstQALY;



                }
                int iLstPercentile = 0; MontoCarlo montoCarlo = new MontoCarlo();
                List<int> query = null;
                if (crSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile != null && crSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count > 0)
                {



                }
                List<QALYValueAttribute> lstQALYValueAttribute = new List<QALYValueAttribute>();
                foreach (CRCalculateValue crCalculateValue in crSelectFunctionCalculateValue.CRCalculateValues)
                {
                    QALYValueAttribute qalyValueAttribute = new QALYValueAttribute();

                    if (crCalculateValue.PointEstimate != 0)
                    {


                        qalyValueAttribute.Col = crCalculateValue.Col;
                        qalyValueAttribute.Row = crCalculateValue.Row;





                    }
                    qalyValueAttribute.Col = crCalculateValue.Col;
                    qalyValueAttribute.Row = crCalculateValue.Row;

                    qalyValueAttribute.PointEstimate = Convert.ToSingle(Math.Round(crCalculateValue.PointEstimate * allSelectQALYMethod.fQALYFirst, 4));
                    qalyValueAttribute.LstPercentile = new List<float>();
                    if (crCalculateValue.LstPercentile != null && crCalculateValue.LstPercentile.Count > 0)
                    {
                        iLstPercentile = crCalculateValue.LstPercentile.Count;
                        if (crCalculateValue.PointEstimate == 0)
                        {
                            for (int iMonto = 0; iMonto < 100; iMonto++)
                            {
                                qalyValueAttribute.LstPercentile.Add(0);
                            }
                        }
                        else
                        {
                            lstQALYTemp = new List<float>();
                            foreach (float dMonto in allSelectQALYMethod.lstQALYLast)
                            {
                                foreach (float d in crCalculateValue.LstPercentile)
                                {

                                    lstQALYTemp.Add(dMonto * d);
                                }
                            }
                            lstQALYTemp.Sort();
                            qalyValueAttribute.LstPercentile = getMedianSample(lstQALYTemp, 100);






                        }

                        qalyValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(qalyValueAttribute.LstPercentile);
                        qalyValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(qalyValueAttribute.LstPercentile, qalyValueAttribute.PointEstimate);
                        qalyValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(qalyValueAttribute.LstPercentile, qalyValueAttribute.PointEstimate);


                    }





                    lstQALYValueAttribute.Add(qalyValueAttribute);


                }
                allSelectQALYMethodAndValue.lstQALYValueAttributes = lstQALYValueAttribute;
                GC.Collect();
                return allSelectQALYMethodAndValue;


            }
            catch
            {
                return null;
            }
        }
        public static AllSelectQALYMethodAndValue getPoolingLstAllSelectQALYMethodAndValue_old(List<AllSelectQALYMethodAndValue> lstAllSelectQALYMethodAndValue, PoolingMethodTypeEnum poolingMethod, List<double> lstWeight)
        {
            try
            {
                AllSelectQALYMethodAndValue allSelectQALYMethodAndValue = new AllSelectQALYMethodAndValue();
                allSelectQALYMethodAndValue.lstQALYValueAttributes = new List<QALYValueAttribute>();
                Dictionary<int, double> dicWeight = new Dictionary<int, double>();

                Dictionary<int, Dictionary<string, QALYValueAttribute>> dicAll = new Dictionary<int, Dictionary<string, QALYValueAttribute>>();
                Dictionary<string, QALYValueAttribute> dicValue = new Dictionary<string, QALYValueAttribute>();
                int i = 0;
                List<string> lstAllColRow = new List<string>();
                QALYValueAttribute aValueAttribute = new QALYValueAttribute();
                string[] strColRow;
                float dPoint = 0;
                List<float> lstLHS = new List<float>();
                List<QALYValueAttribute> lstQALYValueAttributes = new List<QALYValueAttribute>();
                while (i < lstAllSelectQALYMethodAndValue.Count)
                {
                    dicValue = new Dictionary<string, QALYValueAttribute>();

                    foreach (QALYValueAttribute apvValueAttribute in lstAllSelectQALYMethodAndValue[i].lstQALYValueAttributes)
                    {
                        dicValue.Add(apvValueAttribute.Col + "," + apvValueAttribute.Row, apvValueAttribute);
                    }
                    dicAll.Add(i, dicValue);
                    lstAllColRow = lstAllColRow.Union(dicValue.Keys.ToList()).ToList();
                    i++;
                }
                switch (poolingMethod)
                {

                    case PoolingMethodTypeEnum.SumDependent:
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new QALYValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstLHS = new List<float>();
                            dPoint = 0;
                            foreach (KeyValuePair<int, Dictionary<string, QALYValueAttribute>> k in dicAll)
                            {
                                try
                                {
                                    dPoint += k.Value[s].PointEstimate;
                                    if (k.Value[s].LstPercentile != null)
                                    {
                                        if (lstLHS.Count == 0)
                                        {
                                            lstLHS = k.Value[s].LstPercentile;
                                        }
                                        else
                                        {
                                            i = 0;
                                            while (i < k.Value[s].LstPercentile.Count)
                                            {
                                                lstLHS[i - 1] += k.Value[s].LstPercentile[i];
                                                i++;
                                            }

                                        }
                                    }

                                }
                                catch
                                { }
                            }
                            aValueAttribute.PointEstimate = dPoint;
                            aValueAttribute.LstPercentile = lstLHS;
                            aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(lstLHS);
                            aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(lstLHS, dPoint);
                            aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(lstLHS, dPoint);
                            lstQALYValueAttributes.Add(aValueAttribute);

                        }
                        break;

                    case PoolingMethodTypeEnum.SumIndependent:
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new QALYValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstLHS = new List<float>();
                            dPoint = 0;
                            foreach (KeyValuePair<int, Dictionary<string, QALYValueAttribute>> k in dicAll)
                            {
                                try
                                {
                                    dPoint += k.Value[s].PointEstimate;
                                    if (k.Value[s].LstPercentile != null)
                                    {
                                        if (lstLHS.Count == 0)
                                        {
                                            lstLHS = k.Value[s].LstPercentile;
                                        }
                                        else
                                        {
                                            i = 0;
                                            while (i < k.Value[s].LstPercentile.Count)
                                            {
                                                lstLHS[i - 1] += k.Value[s].LstPercentile[i];
                                                i++;
                                            }

                                        }
                                    }

                                }
                                catch
                                { }
                            }
                            aValueAttribute.PointEstimate = dPoint;
                            aValueAttribute.LstPercentile = lstLHS;
                            aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(lstLHS);
                            aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(lstLHS, dPoint);
                            aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(lstLHS, dPoint);
                            lstQALYValueAttributes.Add(aValueAttribute);

                        }

                        break;
                    case PoolingMethodTypeEnum.SubtractionDependent:
                        int iSubtractionDependent = 0;
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new QALYValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstLHS = new List<float>();
                            dPoint = 0;
                            iSubtractionDependent = 0;
                            foreach (KeyValuePair<int, Dictionary<string, QALYValueAttribute>> k in dicAll)
                            {
                                try
                                {
                                    if (iSubtractionDependent == 0)
                                        dPoint += k.Value[s].PointEstimate;
                                    else
                                        dPoint -= k.Value[s].PointEstimate;
                                    iSubtractionDependent++;
                                    if (k.Value[s].LstPercentile != null)
                                    {
                                        if (lstLHS.Count == 0)
                                        {

                                            lstLHS = k.Value[s].LstPercentile;
                                        }
                                        else
                                        {

                                            i = 0;
                                            while (i < k.Value[s].LstPercentile.Count())
                                            {
                                                lstLHS[i - 1] -= k.Value[s].LstPercentile[i];
                                                i++;
                                            }

                                        }
                                    }

                                }
                                catch
                                { }
                            }
                            aValueAttribute.PointEstimate = dPoint;
                            aValueAttribute.LstPercentile = lstLHS;
                            aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(lstLHS);
                            aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(lstLHS, dPoint);
                            aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(lstLHS, dPoint);
                            lstQALYValueAttributes.Add(aValueAttribute);

                        }

                        break;
                    case PoolingMethodTypeEnum.SubtractionIndependent:
                        int iSubtractionIndependent = 0;
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new QALYValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstLHS = new List<float>();
                            dPoint = 0;
                            iSubtractionIndependent = 0;
                            foreach (KeyValuePair<int, Dictionary<string, QALYValueAttribute>> k in dicAll)
                            {
                                try
                                {
                                    if (iSubtractionIndependent == 0)
                                        dPoint += k.Value[s].PointEstimate;
                                    else
                                        dPoint -= k.Value[s].PointEstimate;
                                    iSubtractionIndependent++;
                                    if (k.Value[s].LstPercentile != null)
                                    {
                                        if (lstLHS.Count == 0)
                                        {

                                            lstLHS = k.Value[s].LstPercentile;
                                        }
                                        else
                                        {

                                            i = 0;
                                            while (i < k.Value[s].LstPercentile.Count())
                                            {
                                                lstLHS[i - 1] -= k.Value[s].LstPercentile[i];
                                                i++;
                                            }

                                        }
                                    }

                                }
                                catch
                                { }
                            }
                            aValueAttribute.PointEstimate = dPoint;
                            aValueAttribute.LstPercentile = lstLHS;
                            aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(lstLHS);
                            aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(lstLHS, dPoint);
                            aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(lstLHS, dPoint);
                            lstQALYValueAttributes.Add(aValueAttribute);

                        }
                        break;
                    case PoolingMethodTypeEnum.SubjectiveWeights:
                        if (lstWeight != null && lstWeight.Count > 0 && lstWeight.Sum() != 1)
                        {
                            double dWeightSum = lstWeight.Sum();
                            for (int iWeightSum = 0; iWeightSum < lstWeight.Count; iWeightSum++)
                            {
                                lstWeight[iWeightSum] = lstWeight[iWeightSum] / dWeightSum;
                            }
                        }
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new QALYValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstLHS = new List<float>();
                            dPoint = 0;
                            foreach (KeyValuePair<int, Dictionary<string, QALYValueAttribute>> k in dicAll)
                            {
                                try
                                {
                                    dPoint += Convert.ToSingle(k.Value[s].PointEstimate * lstAllSelectQALYMethodAndValue[k.Key].AllSelectQALYMethod.Weight);
                                    if (k.Value[s].LstPercentile != null)
                                    {
                                        if (lstLHS.Count == 0)
                                        {
                                            i = 0;
                                            while (i < k.Value[s].LstPercentile.Count())
                                            {
                                                lstLHS.Add(Convert.ToSingle(k.Value[s].LstPercentile[i] * lstAllSelectQALYMethodAndValue[k.Key].AllSelectQALYMethod.Weight));
                                                i++;
                                            }
                                        }
                                        else
                                        {
                                            i = 0;
                                            while (i < k.Value[s].LstPercentile.Count)
                                            {
                                                lstLHS[i] += Convert.ToSingle(k.Value[s].LstPercentile[i] * lstAllSelectQALYMethodAndValue[k.Key].AllSelectQALYMethod.Weight);
                                                i++;
                                            }

                                        }
                                    }

                                }
                                catch
                                { }
                            }
                            aValueAttribute.PointEstimate = dPoint;
                            aValueAttribute.LstPercentile = lstLHS;
                            aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(lstLHS);
                            aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(lstLHS, dPoint);
                            aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(lstLHS, dPoint);
                            lstQALYValueAttributes.Add(aValueAttribute);

                        }

                        break;
                    case PoolingMethodTypeEnum.RandomOrFixedEffects:
                        List<int> lstRandom = new List<int>();
                        Random random = new Random();
                        i = 0;
                        while (i < dicAll.Count)
                        {
                            lstRandom.Add(random.Next(10));
                            i++;
                        }
                        lstWeight = new List<double>();
                        i = 0;
                        int iSum = lstRandom.Sum();
                        while (i < dicAll.Count)
                        {
                            lstWeight.Add(Convert.ToDouble(lstRandom[i]) / Convert.ToDouble(iSum));
                            i++;
                        }
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new QALYValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstLHS = new List<float>();
                            dPoint = 0;
                            double sumVariance = 0;
                            foreach (KeyValuePair<int, Dictionary<string, QALYValueAttribute>> k in dicAll)
                            {

                                if (k.Value.ContainsKey(s))
                                {
                                    sumVariance += k.Value[s].Variance;
                                }
                            }
                            lstWeight = new List<double>();
                            foreach (KeyValuePair<int, Dictionary<string, QALYValueAttribute>> k in dicAll)
                            {
                                lstWeight.Add(k.Value[s].Variance / sumVariance);

                            }
                            foreach (KeyValuePair<int, Dictionary<string, QALYValueAttribute>> k in dicAll)
                            {
                                try
                                {
                                    dPoint += Convert.ToSingle(k.Value[s].PointEstimate * lstWeight[k.Key]);
                                    if (k.Value[s].LstPercentile != null)
                                    {
                                        if (lstLHS.Count == 0)
                                        {
                                            i = 0;
                                            while (i < k.Value[s].LstPercentile.Count)
                                            {
                                                lstLHS.Add(Convert.ToSingle(k.Value[s].LstPercentile[i] * lstWeight[k.Key]));
                                                i++;
                                            }
                                        }
                                        else
                                        {
                                            i = 0;
                                            while (i < k.Value[s].LstPercentile.Count)
                                            {
                                                lstLHS[i - 1] += Convert.ToSingle(k.Value[s].LstPercentile[i] * lstWeight[k.Key]);
                                                i++;
                                            }

                                        }
                                    }

                                }
                                catch
                                { }
                            }
                            aValueAttribute.PointEstimate = dPoint;
                            aValueAttribute.LstPercentile = lstLHS;
                            aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(lstLHS);
                            aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(lstLHS, dPoint);
                            aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(lstLHS, dPoint);
                            lstQALYValueAttributes.Add(aValueAttribute);

                        }
                        break;
                    case PoolingMethodTypeEnum.FixedEffects:
                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new QALYValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstLHS = new List<float>();
                            dPoint = 0;
                            foreach (KeyValuePair<int, Dictionary<string, QALYValueAttribute>> k in dicAll)
                            {
                                try
                                {
                                    dPoint += k.Value[s].PointEstimate;
                                    if (k.Value[s].LstPercentile != null)
                                    {
                                        if (lstLHS.Count == 0)
                                        {
                                            lstLHS = k.Value[s].LstPercentile;
                                        }
                                        else
                                        {
                                            i = 0;
                                            while (i < k.Value[s].LstPercentile.Count)
                                            {
                                                lstLHS[i - 1] += k.Value[s].LstPercentile[i];
                                                i++;
                                            }

                                        }
                                    }

                                }
                                catch
                                { }
                            }
                            aValueAttribute.PointEstimate = dPoint / dicAll.Count;
                            i = 0;
                            while (i < lstLHS.Count - 1)
                            {
                                lstLHS[i] = lstLHS[i] / dicAll.Count;
                                i++;
                            }
                            aValueAttribute.LstPercentile = lstLHS;
                            aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(lstLHS);
                            aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(lstLHS, dPoint);
                            aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(lstLHS, dPoint);
                            lstQALYValueAttributes.Add(aValueAttribute);

                        }

                        break;
                }

                allSelectQALYMethodAndValue.lstQALYValueAttributes = lstQALYValueAttributes;
                GC.Collect();
                return allSelectQALYMethodAndValue;
            }
            catch
            {
                return null;
            }

        }
        public static bool getIsExistQALYFromEndPointGroup(string EndPointGroup)
        {
            try
            {
                string commandText = string.Format("  select  count(0)" +
" from QALYDataSets a, QALYEntries b  where a.QALYDataSetID=b.QALYDataSetID and a.SetupID={0} and EndPointGroup='{1}'   ", CommonClass.MainSetup.SetupID, EndPointGroup);

                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                int count = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
                if (count > 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        public static List<BenMAPQALY> getLstBenMAPQALYFromEndPointGroup(string EndPointGroup)
        {
            try
            {
                List<BenMAPQALY> lstBenMAPQALY = new List<BenMAPQALY>();
                string commandText = string.Format("  select  a.QALYDataSetID,SetupID,QALYDatasetName,EndPointGroup,EndPoint,Qualifier,Description,StartAge,EndAge " +
" from QALYDataSets a,(select distinct QALYDataSetID,StartAge,EndAge from QALYEntries) b where a.QALYDataSetID=b.QALYDataSetID and a.SetupID={0} and EndPointGroup='{1}'   ", CommonClass.MainSetup.SetupID, EndPointGroup);

                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    BenMAPQALY bvf = new BenMAPQALY();
                    bvf.Description = Convert.ToString(dr["Description"]);
                    bvf.EndAge = Convert.ToInt32(dr["EndAge"]);
                    bvf.EndPoint = dr["EndPoint"].ToString();
                    bvf.EndPointGroup = dr["EndPointGroup"].ToString(); bvf.QalyDatasetID = Convert.ToInt32(dr["QalyDatasetID"]);
                    bvf.QalyDatasetName = Convert.ToString(dr["QalyDatasetName"]);
                    bvf.Qualifier = Convert.ToString(dr["Qualifier"]); bvf.SetupID = Convert.ToInt32(dr["SetupID"]);
                    bvf.StartAge = Convert.ToInt32(dr["StartAge"]);

                    lstBenMAPQALY.Add(bvf);





                }
                return lstBenMAPQALY;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
