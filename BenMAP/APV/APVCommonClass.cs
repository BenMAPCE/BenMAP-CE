using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
//using DotSpatial.Data;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
//using Troschuetz.Random;
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
                copy.BaseControlGroup = new List<BaseControlGroup>();
                foreach (BaseControlGroup bcg in BaseControlCRSelectFunctionCalculateValue.BaseControlGroup)
                {
                    BaseControlGroup bcgcopy = new BaseControlGroup();
                    bcgcopy.GridType = bcg.GridType;
                    bcgcopy.Pollutant = bcg.Pollutant;
                    bcgcopy.DeltaQ = bcg.DeltaQ;
                    bcgcopy.Base = bcg.Base;// DataSourceCommonClass.getBenMapLineCopyOnlyResultCopy(bcg.Base);
                    bcgcopy.Control = bcg.Control;// DataSourceCommonClass.getBenMapLineCopyOnlyResultCopy(bcg.Control);
                    copy.BaseControlGroup.Add(bcgcopy);
                }
                copy.BenMAPPopulation = CommonClass.BaseControlCRSelectFunctionCalculateValue.BenMAPPopulation;
                copy.CRLatinHypercubePoints = CommonClass.BaseControlCRSelectFunctionCalculateValue.CRLatinHypercubePoints;
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
                    //crrcopy.ResultCopy = null;
                    //crrcopy.ResultCopy = new List<float[]>();
                    //foreach (CRCalculateValue crCalculateValue in crr.CRCalculateValues)
                    //{
                    //    lstd = null;
                    //    lstd = new List<float>();
                    //    lstd.Add(crCalculateValue.Col);
                    //    lstd.Add(crCalculateValue.Row);
                    //    lstd.Add(crCalculateValue.PointEstimate);
                    //    lstd.Add(crCalculateValue.Population);
                    //    lstd.Add(crCalculateValue.Incidence);
                    //    lstd.Add(crCalculateValue.Delta);
                    //    lstd.Add(crCalculateValue.Mean);
                    //    lstd.Add(crCalculateValue.Baseline);
                    //    lstd.Add(crCalculateValue.PercentOfBaseline);
                    //    lstd.Add(crCalculateValue.StandardDeviation);
                    //    lstd.Add(crCalculateValue.Variance);
                    //    if (crCalculateValue.LstPercentile != null)
                    //        lstd.AddRange(crCalculateValue.LstPercentile);
                    //    crrcopy.ResultCopy.Add(lstd.ToArray());
                    //}
                    //crSelectFunctionCalculateValue.ResultCopy.Add(lstd.ToArray());
                    //crrcopy.ResultCopy = crr.ResultCopy;
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
                //valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregation = ValuationMethodPoolingAndAggregationFrom.IncidencePoolingAndAggregation;
                //valuationMethodPoolingAndAggregation.lstAllSelectQALYMethod = ValuationMethodPoolingAndAggregationFrom.lstAllSelectQALYMethod;
                //valuationMethodPoolingAndAggregation.LstAllSelectValuationMethod = ValuationMethodPoolingAndAggregationFrom.LstAllSelectValuationMethod;
                //valuationMethodPoolingAndAggregation.IncidencePoolingResult = CommonClass.IncidencePoolingResult;
                //valuationMethodPoolingAndAggregation.lstAllSelectQALYMethodAndValue = new List<AllSelectQALYMethodAndValue>();
                foreach (ValuationMethodPoolingAndAggregationBase vb in ValuationMethodPoolingAndAggregationFrom.lstValuationMethodPoolingAndAggregationBase)
                {
                    ValuationMethodPoolingAndAggregationBase vbOut = new ValuationMethodPoolingAndAggregationBase();

                    //if (vb.lstAllSelectQALYMethodAndValue != null)
                    //{
                    //    vbOut.lstAllSelectQALYMethodAndValue = new List<AllSelectQALYMethodAndValue>();
                    //    foreach (AllSelectQALYMethodAndValue allSelectQALYMethodAndValue in vb.lstAllSelectQALYMethodAndValue)
                    //    {
                    //        AllSelectQALYMethodAndValue avcopy = new AllSelectQALYMethodAndValue();
                    //        avcopy.AllSelectQALYMethod = allSelectQALYMethodAndValue.AllSelectQALYMethod;
                    //        avcopy.ResultCopy = null;
                    //        avcopy.ResultCopy = new List<double[]>();
                    //        //avcopy.lstQALYMonteCarlo = allSelectQALYMethodAndValue.lstQALYMonteCarlo;
                    //        foreach (QALYValueAttribute qalyValueAttribute in allSelectQALYMethodAndValue.lstQALYValueAttributes)
                    //        {
                    //            lstd = null;
                    //            lstd = new List<double>();
                    //            lstd.Add(qalyValueAttribute.Col);
                    //            lstd.Add(qalyValueAttribute.Row);
                    //            lstd.Add(qalyValueAttribute.PointEstimate);
                    //            lstd.Add(qalyValueAttribute.Mean);
                    //            lstd.Add(qalyValueAttribute.StandardDeviation);
                    //            lstd.Add(qalyValueAttribute.Variance);
                    //            if(qalyValueAttribute.LstPercentile!=null)
                    //            lstd.AddRange(qalyValueAttribute.LstPercentile);
                    //            avcopy.ResultCopy.Add(lstd.ToArray());
                    //        }
                    //        vbOut.lstAllSelectQALYMethodAndValue.Add(avcopy);
                    //    }
                    //}
                    vbOut.lstValuationColumns = vb.lstValuationColumns;
                    vbOut.lstQALYColumns = vb.lstQALYColumns;
                    vbOut.lstAllSelectQALYMethodAndValue = null;// vb.lstAllSelectQALYMethodAndValue;
                    vbOut.LstAllSelectValuationMethodAndValue = null;// vb.LstAllSelectValuationMethodAndValue;
                    //vbOut.LstAllSelectValuationMethodAndValue = new List<AllSelectValuationMethodAndValue>();
                    //foreach (AllSelectValuationMethodAndValue allSelectValuationMethodAndValue in vb.LstAllSelectValuationMethodAndValue)
                    //{
                    //    AllSelectValuationMethodAndValue avcopyv = new AllSelectValuationMethodAndValue();
                    //    avcopyv.AllSelectValuationMethod = allSelectValuationMethodAndValue.AllSelectValuationMethod;
                    //    avcopyv.ResultCopy = null;
                    //    avcopyv.ResultCopy = new List<float[]>();

                    //    foreach (APVValueAttribute apvValueAttribute in allSelectValuationMethodAndValue.lstAPVValueAttributes)
                    //    {
                    //        lstd = null;
                    //        lstd = new List<float>();
                    //        lstd.Add(apvValueAttribute.Col);
                    //        lstd.Add(apvValueAttribute.Row);
                    //        lstd.Add(apvValueAttribute.PointEstimate);
                    //        lstd.Add(apvValueAttribute.Mean);
                    //        lstd.Add(apvValueAttribute.StandardDeviation);
                    //        lstd.Add(apvValueAttribute.Variance);
                    //        if(apvValueAttribute.LstPercentile!=null)
                    //        lstd.AddRange(apvValueAttribute.LstPercentile);
                    //        avcopyv.ResultCopy.Add(lstd.ToArray());
                    //    }
                    //    vbOut.LstAllSelectValuationMethodAndValue.Add(avcopyv);
                    //}
                    vbOut.IncidencePoolingAndAggregation = new IncidencePoolingAndAggregation();// vb.IncidencePoolingAndAggregation;
                    vbOut.IncidencePoolingAndAggregation.ConfigurationResultsFilePath = vb.IncidencePoolingAndAggregation.ConfigurationResultsFilePath;
                    vbOut.IncidencePoolingAndAggregation.PoolingName = vb.IncidencePoolingAndAggregation.PoolingName;
                    vbOut.IncidencePoolingAndAggregation.lstColumns = vb.IncidencePoolingAndAggregation.lstColumns;
                    vbOut.IncidencePoolingAndAggregation.VariableDataset = vb.IncidencePoolingAndAggregation.VariableDataset;
                    vbOut.IncidencePoolingAndAggregation.Weights = vb.IncidencePoolingAndAggregation.Weights;
                    vbOut.IncidencePoolingAndAggregation.lstAllSelectCRFuntion = new List<AllSelectCRFunction>();
                    foreach (AllSelectCRFunction alcr in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion)
                    {
                        vbOut.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Add(new AllSelectCRFunction()
                        {
                            Author = alcr.Author,
                            //  CRSelectFunctionCalculateValue=alcr.CRSelectFunctionCalculateValue==null?n new CRSelectFunctionCalculateValue(){ CRSelectFunction=alcr.CRSelectFunctionCalculateValue.CRSelectFunction},
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
                            Year = alcr.Year
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
                        //formatter = null;
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

                }
                valuationMethodPoolingAndAggregation.CreateTime = ValuationMethodPoolingAndAggregationFrom.CreateTime;
                valuationMethodPoolingAndAggregation.CFGRPath = strFile.Substring(0, strFile.Length - 6) + ".cfgrx";
                //Configuration.ConfigurationCommonClass.SaveCRFRFile(ValuationMethodPoolingAndAggregationFrom.BaseControlCRSelectFunctionCalculateValue, valuationMethodPoolingAndAggregation.CFGRPath);
                //if(ValuationMethodPoolingAndAggregationFrom.BaseControlCRSelectFunctionCalculateValue!=null)
                //ValuationMethodPoolingAndAggregationFrom.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue = null;
                //ValuationMethodPoolingAndAggregationFrom.BaseControlCRSelectFunctionCalculateValue = null;
                CommonClass.lstCRResultAggregation.Clear();

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
                        //ValuationMethodPoolingAndAggregationFrom.BaseControlCRSelectFunctionCalculateValue = Configuration.ConfigurationCommonClass.LoadCRFRFile(valuationMethodPoolingAndAggregation.CFGRPath);
                        //CommonClass.BaseControlCRSelectFunctionCalculateValue = ValuationMethodPoolingAndAggregationFrom.BaseControlCRSelectFunctionCalculateValue;
                        valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase = null;
                        valuationMethodPoolingAndAggregation = null;
                        GC.Collect();
                        return true;
                    }
                    catch
                    {
                        fs.Close();
                        fs.Dispose();
                        valuationMethodPoolingAndAggregation = null;
                        //ValuationMethodPoolingAndAggregationFrom.BaseControlCRSelectFunctionCalculateValue = Configuration.ConfigurationCommonClass.LoadCRFRFile(valuationMethodPoolingAndAggregation.CFGRPath);
                        //CommonClass.BaseControlCRSelectFunctionCalculateValue = ValuationMethodPoolingAndAggregationFrom.BaseControlCRSelectFunctionCalculateValue;
                        
                        //formatter = null;
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
        public static ValuationMethodPoolingAndAggregation loadAPVRFile(string strFile, ref string err)
        {
            //CommonClass.ClearAllObject();
            ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation = null;
            //CommonClass.ClearAllObject();

            try
            { 
                //ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation=null;
                 // BaseControlCRSelectFunctionCalculateValue baseControlCRSelectFunctionCalculateValue = null;// (BaseControlCRSelectFunctionCalculateValue)serializer.ReadObject(mStream);
               using (FileStream fs = new FileStream(strFile, FileMode.Open))
               {
                   //BinaryFormatter formatter = new BinaryFormatter();
                   //valuationMethodPoolingAndAggregation = (ValuationMethodPoolingAndAggregation)formatter.Deserialize(fs);//在这里大家要注意咯,他的返回值是object
                   //fs.Close();
                   //fs.Dispose();
                   //formatter = null;
                   //GC.Collect();
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
                   //Serializer.Serialize<List<BaseControlGroup>>(fs, baseControlCRSelectFunctionCalculateValue.BaseControlGroup);
                   //Serializer.Serialize<BaseControlCRSelectFunctionCalculateValue>(fs, baseControlCRSelectFunctionCalculateValue);
                   //fs.Flush();
                   //fs.Position = 0;

                   //TestObject obj2 = Serializer.Deserialize<TestObject>(fs);
                   //Console.WriteLine(obj2);  
                   fs.Close();
                   fs.Dispose();
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
                       //MessageBox.Show(errCFGR);
                       return null;
                   }
                   valuationMethodPoolingAndAggregation.CFGRPath = "";
               }
               //------------update数据-------------------------
               //foreach (BaseControlGroup bcg in valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup)
               //{
               //      DataSourceCommonClass.getModelValuesFromResultCopy(ref bcg.Base);
               //      DataSourceCommonClass.getModelValuesFromResultCopy(ref bcg.Control);
               //}
               //for (int i=0; i < valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; i++)
               //{
               //    CRSelectFunctionCalculateValue crclv = valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[i];
               //    //if(crclv.ResultCopy!=null && crclv.ResultCopy.Count>0)
               //    //Configuration.ConfigurationCommonClass. getCalculateValueFromResultCopy(ref crclv);
               //    //crclv.ResultCopy = null;
               //    valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[i] = crclv;
 
               //}
                
               // ---------------update QALY Valuation数据-------------------
               if (!isAPV)
               {
                   foreach (ValuationMethodPoolingAndAggregationBase vb in valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                   {
                       //修正incidence----------
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
               //-----------------让StartAge,EndAge真实的表达所选择的年龄段。StartAge,Min,EndAge,Max
               foreach (ValuationMethodPoolingAndAggregationBase vb in valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
               {
                   foreach (AllSelectCRFunction acr in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion)
                   {
                       if (acr.PoolingMethod != "")
                       {
                           //--------------得到所有NoOne---------
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
               //        //-----------必须getpooling-------------
               //        //foreach (AllSelectCRFunction alsr in vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion)
               //        //{
               //        //    if (alsr.CRSelectFunctionCalculateValue != null && alsr.NodeType != 100)
               //        //    {
               //        //        List<AllSelectCRFunction> lstSec = new List<AllSelectCRFunction>();
               //        //        getAllChildCRNotNoneCalulate(alsr, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstSec);
               //        //        //List<AllSelectCRFunction> lstSecResult = new List<AllSelectCRFunction>();
               //        //        //foreach (AllSelectCRFunction alcr in lstSec)
               //        //        //{
               //        //        //    if (lstAllSelectCRFunctionNone.Where(p => p.ID == alcr.ID).Count() > 0)
               //        //        //    {
               //        //        //        lstSecResult.Add(lstAllSelectCRFunctionNone.Where(p => p.ID == alcr.ID).First());
               //        //        //    }
               //        //        //}
               //        //        alsr.CRSelectFunctionCalculateValue = getPoolingMethodCRSelectFunctionCalculateValue(lstSec.Where(p => p.CRSelectFunctionCalculateValue != null).Select(a => a.CRSelectFunctionCalculateValue).ToList(), getPoolingMethodTypeEnumFromString(alsr.PoolingMethod), lstSec.Where(p => p.CRSelectFunctionCalculateValue != null).Select(a => a.Weight).ToList());

               //        //        //alsr.CRSelectFunctionCalculateValue = valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Where(p => p.CRSelectFunction.CRID == alsr.CRID).First();
               //        //    }
               //        //}
               //        //------------modify by xiejp--------------
               //        //-----------------------首先得到Pooling--------------------------------------

               //        List<AllSelectCRFunction> lstCR = new List<AllSelectCRFunction>();
               //        if (vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().PoolingMethod == "None")
               //        {
               //            APVX.APVCommonClass.getAllChildCRNotNone(vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First(), vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstCR);

               //        }
               //        lstCR.Insert(0, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First());
               //        if (lstCR.Count == 1 && vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.First().CRID < 9999)//.PoolingMethod == "")
               //        { }
               //        else
               //        {
               //            APVX.APVCommonClass.getPoolingMethodCRFromAllSelectCRFunction(ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, vb.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.NodeType != 100).Max(p => p.NodeType), vb.IncidencePoolingAndAggregation.lstColumns);
               //        }
               //        //---------------------------------------------------
               //        //if (vb.lstAllSelectQALYMethodAndValue != null)
               //        //{
               //        //    for (int i = 0; i < vb.lstAllSelectQALYMethodAndValue.Count; i++)
               //        //    {
               //        //        AllSelectQALYMethodAndValue allSelectQALYMethodAndValue = vb.lstAllSelectQALYMethodAndValue[i];
               //        //        //getAllSelectQALYMethodAndValueFromResultCopy(ref allSelectQALYMethodAndValue);

               //        //    }
               //        //}
               //        //if (vb.LstAllSelectValuationMethodAndValue != null)
               //        //{
               //        //    for (int i = 0; i < vb.LstAllSelectValuationMethodAndValue.Count; i++)
               //        //    {
               //        //        AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = vb.LstAllSelectValuationMethodAndValue[i];
               //        //        getAllSelectValuationMethodAndValueFromResultCopy(ref allSelectValuationMethodAndValue);
               //        //    }
               //        //}
               //    }
                  
               //}
               GC.Collect();
               CommonClass.GBenMAPGrid = valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.First().GridType;
               //ApplyAggregationFromValuationMethodPoolingAndAggregation(CommonClass.LstGridRelationshipAll, valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue.BaseControlGroup.First().GridType, ref valuationMethodPoolingAndAggregation);

               //mStream.Close();
               //fs.Close();
               return valuationMethodPoolingAndAggregation;
            
            }
            catch
            {
                err = "BenMAP-CE was unable to open the file. The file may be corrupt, or it may have been created using a previous incompatible version of BenMAP-CE.";
                return null;
            } 
        }
        /// <summary>
        /// 从ResultCopy中获取值到ValuationValue
        /// </summary>
        /// <param name="allSelectValuationMethodAndValue"></param>
        public static void getAllSelectValuationMethodAndValueFromResultCopy(ref AllSelectValuationMethodAndValue allSelectValuationMethodAndValue)
        {
            //try
            //{
            //    allSelectValuationMethodAndValue.lstAPVValueAttributes = new List<APVValueAttribute>();
            //    bool islhs=false;
            //    int ilength=0;
            //    if(allSelectValuationMethodAndValue.ResultCopy!=null && allSelectValuationMethodAndValue.ResultCopy.Count>0)
            //    {
            //        ilength=allSelectValuationMethodAndValue.ResultCopy.First().Length;
            //        if(allSelectValuationMethodAndValue.ResultCopy.First().Length>6)
            //        {
            //            islhs=true;
            //        }
            //    }
            //    for (int i = 0; i < allSelectValuationMethodAndValue.ResultCopy.Count; i++)
            //    {
            //        allSelectValuationMethodAndValue.lstAPVValueAttributes.Add(new APVValueAttribute()
            //        {
            //            Col = Convert.ToInt32(allSelectValuationMethodAndValue.ResultCopy[i][0]),
            //            Row = Convert.ToInt32(allSelectValuationMethodAndValue.ResultCopy[i][1]),
            //            PointEstimate = allSelectValuationMethodAndValue.ResultCopy[i][2],
            //            Mean = allSelectValuationMethodAndValue.ResultCopy[i][3],
            //            StandardDeviation = allSelectValuationMethodAndValue.ResultCopy[i][4],
            //            Variance = allSelectValuationMethodAndValue.ResultCopy[i][5],
            //             LstPercentile=(islhs)?allSelectValuationMethodAndValue.ResultCopy[i].ToList().GetRange(6,ilength-6):null
            //        });

            //    }
 
            //}
            //catch
            //{
            //}
            //finally
            //{
            //    allSelectValuationMethodAndValue.ResultCopy = null;
            //    //valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue = null;
            //}
        }
        /// <summary>
        /// 从ResultCopy中获取值到QALY
        /// </summary>
        /// <param name="allSelectQALYMethodAndValue"></param>
        //public static void getAllSelectQALYMethodAndValueFromResultCopy(ref AllSelectQALYMethodAndValue allSelectQALYMethodAndValue)
        //{
        //    try
        //    {
        //        allSelectQALYMethodAndValue.lstQALYValueAttributes = new List<QALYValueAttribute>();
        //        bool islhs = false;
        //        int ilength = 0;
        //        if (allSelectQALYMethodAndValue.ResultCopy != null && allSelectQALYMethodAndValue.ResultCopy.Count > 0)
        //        {
        //            ilength = allSelectQALYMethodAndValue.ResultCopy.First().Length;
        //            if (allSelectQALYMethodAndValue.ResultCopy.First().Length > 6)
        //            {
        //                islhs = true;
        //            }
        //        }
        //        for (int i = 0; i < allSelectQALYMethodAndValue.ResultCopy.Count; i++)
        //        {
        //            allSelectQALYMethodAndValue.lstQALYValueAttributes.Add(new QALYValueAttribute()
        //            {
        //                Col = Convert.ToInt32(allSelectQALYMethodAndValue.ResultCopy[i][0]),
        //                Row = Convert.ToInt32(allSelectQALYMethodAndValue.ResultCopy[i][1]),
        //                PointEstimate = allSelectQALYMethodAndValue.ResultCopy[i][2],
        //                Mean = allSelectQALYMethodAndValue.ResultCopy[i][3],
        //                StandardDeviation = allSelectQALYMethodAndValue.ResultCopy[i][4],
        //                Variance = allSelectQALYMethodAndValue.ResultCopy[i][5],
        //                LstPercentile = (islhs) ? allSelectQALYMethodAndValue.ResultCopy[i].ToList().GetRange(6, ilength - 6) : null
        //            });

        //        }

        //    }
        //    catch
        //    {
        //    }
        //    finally
        //    {
        //        allSelectQALYMethodAndValue.ResultCopy = null;
        //        //valuationMethodPoolingAndAggregation.BaseControlCRSelectFunctionCalculateValue = null;
        //    }
        //}

        private static Tools.CalculateFunctionString _valuationEval;
        /// <summary>
        /// 用来计算Valuation的Eval对象----用于在循环网格时不再生成编译对象
        /// </summary>
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
                    //    case "if ((Q1<A) or (Q1>B)) then Result :=0 else if ((Q1>=0)and(Q1<1)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0 else if ((Q1>=1) and (Q1<2)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.2 else if ((Q1>=2) and (Q1<3)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.40 else if ((Q1>=3) and (Q1<4)) then Result :=(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.6 else if ((Q1>=4) and (Q1<5)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.8 else if (Q1>=5) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP else Result := 0":

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
        /// <summary>
        /// 从函数字符串中得到使用到的系统变量的列表
        /// </summary>
        /// <param name="DatabaseFunction"></param>
        /// <param name="SystemVariableNameList"></param>
        /// <param name="lstFunctionVariables"></param>
      //  public static void getSetupVariableNameListFromDatabaseFunction(string DatabaseFunction, List<string> SystemVariableNameList, ref List<SetupVariableJoinAllValues> lstFunctionVariables)
      //  {
      //      try
      //      {
      //          // List<string> lstResult=new List<string>();
      //          if (lstFunctionVariables == null) lstFunctionVariables = new List<SetupVariableJoinAllValues>();
      //          DatabaseFunction = DatabaseFunction.Replace("prevalence", "").Replace("incidence", "").Replace("deltaq", "")
      //               .Replace("pop", "").Replace("beta", "").Replace("q0", "").Replace("q1", "")
      //              .Replace("abs", " ")
      //.Replace("acos", " ")
      //.Replace("asin", " ")
      //.Replace("atan", " ")
      //.Replace("atan2", " ")
      //.Replace("bigmul", " ")
      //.Replace("ceiling", " ")
      //.Replace("cos", " ")
      //.Replace("cosh", " ")
      //.Replace("divrem", " ")
      //.Replace("exp", " ")
      //.Replace("floor", " ")
      //.Replace("ieeeremainder", " ")
      //.Replace("log", " ")
      //.Replace("log10", " ")
      //.Replace("max", " ")
      //.Replace("min", " ")
      //.Replace("pow", " ")
      //.Replace("round", " ")
      //.Replace("sign", " ")
      //.Replace("sin", " ")
      //.Replace("sinh", " ")
      //.Replace("sqrt", " ")
      //.Replace("tan", " ")
      //.Replace("tanh", " ")
      //.Replace("truncate", " ");
      //          ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

      //          foreach (string str in SystemVariableNameList)
      //          {
      //              if (DatabaseFunction.ToLower().Contains(str.ToLower()))
      //              {
      //                  bool inLst = false;
      //                  foreach (SetupVariableJoinAllValues sv in lstFunctionVariables)
      //                  {
      //                      if (sv.SetupVariableName.ToLower() == str.ToLower())
      //                      {
      //                          inLst = true;
      //                      }
      //                  }
      //                  if (!inLst)
      //                  {
      //                      SetupVariableJoinAllValues setupVariableJoinAllValues = new SetupVariableJoinAllValues();
      //                      setupVariableJoinAllValues.SetupVariableName = str;
      //                      string commandText = string.Format("select SetupVariableID,GridDefinitionID from SetupVariables where SetupVariableName='{0}'", str);
      //                      DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
      //                      DataRow dr = ds.Tables[0].Rows[0];
      //                      setupVariableJoinAllValues.SetupVariableID = Convert.ToInt32(dr["SetupVariableID"]);
      //                      setupVariableJoinAllValues.SetupVariableGridType = Convert.ToInt32(dr["GridDefinitionID"]);
      //                      //add all value to lstvalues
      //                      commandText = string.Format(" select SetupVariableID,CColumn,Row,VValue from SetupGeographicVariables where SetupVariableID={0}", setupVariableJoinAllValues.SetupVariableID);
      //                      ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
      //                      setupVariableJoinAllValues.lstValues = new List<SetupVariableValues>();
      //                      foreach (DataRow drVariable in ds.Tables[0].Rows)
      //                      {
      //                          setupVariableJoinAllValues.lstValues.Add(new SetupVariableValues()
      //                          {
      //                              Col = Convert.ToInt32(drVariable["CColumn"]),
      //                              Row = Convert.ToInt32(drVariable["Row"]),
      //                              Value = Convert.ToDouble(drVariable["VValue"])
      //                          });


      //                      }
      //                      int GridDefinitionID = CommonClass.GBenMAPGrid.GridDefinitionID;
      //                      SetupVariableJoinAllValues setupVariableJoinAllValuesReturn = new SetupVariableJoinAllValues();
      //                      //-----------------------------------直接计算成现在的GridType------------------------------------
      //                      GridRelationship gridRelationShipPopulation = new GridRelationship();

      //                      foreach (GridRelationship gRelationship in CommonClass.LstGridRelationshipAll)
      //                      {
      //                          if ((gRelationship.bigGridID == setupVariableJoinAllValues.SetupVariableGridType && gRelationship.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID) || (gRelationship.smallGridID == setupVariableJoinAllValues.SetupVariableGridType && gRelationship.bigGridID == CommonClass.GBenMAPGrid.GridDefinitionID))
      //                          {
      //                              gridRelationShipPopulation = gRelationship;
      //                          }
      //                      }
      //                      double d = 0;
      //                      if (setupVariableJoinAllValues.SetupVariableGridType == GridDefinitionID)
      //                      {
      //                          setupVariableJoinAllValuesReturn = setupVariableJoinAllValues;
      //                      }
      //                      else
      //                      {
      //                          if (setupVariableJoinAllValues.SetupVariableGridType == gridRelationShipPopulation.bigGridID)//Population比较大
      //                          {
      //                              foreach (GridRelationshipAttribute gra in gridRelationShipPopulation.lstGridRelationshipAttribute)
      //                              {
      //                                  var queryPopulation = from a in setupVariableJoinAllValues.lstValues where gra.bigGridRowCol.Col == a.Col && gra.bigGridRowCol.Row == a.Row select new { Values = setupVariableJoinAllValues.lstValues.Average(c => c.Value) };

      //                                  if (queryPopulation != null && queryPopulation.Count() > 0 && gra.smallGridRowCol.Count > 0)
      //                                  {
      //                                      d = queryPopulation.First().Values;
      //                                      foreach (RowCol rc in gra.smallGridRowCol)
      //                                      {
      //                                          setupVariableJoinAllValuesReturn.lstValues.Add(new SetupVariableValues()
      //                                          {
      //                                              Col = rc.Col,
      //                                              Row = rc.Row,
      //                                              Value = d
      //                                          });
      //                                      }
      //                                  }

      //                              }
      //                          }
      //                          else//网格类型比较大
      //                          {
      //                              foreach (GridRelationshipAttribute gra in gridRelationShipPopulation.lstGridRelationshipAttribute)
      //                              {
      //                                  var queryPopulation = from a in setupVariableJoinAllValues.lstValues where gra.smallGridRowCol.Contains(new RowCol() { Row = a.Row, Col = a.Col }, new RowColComparer()) select new { Values = setupVariableJoinAllValues.lstValues.Average(c => c.Value) };

      //                                  if (queryPopulation != null && queryPopulation.Count() > 0)
      //                                  {
      //                                      d = queryPopulation.First().Values;
      //                                      setupVariableJoinAllValuesReturn.lstValues.Add(new SetupVariableValues()
      //                                      {
      //                                          Col = gra.bigGridRowCol.Col,
      //                                          Row = gra.bigGridRowCol.Row,
      //                                          Value = d
      //                                      });
      //                                  }


      //                              }
      //                          }
      //                          setupVariableJoinAllValuesReturn.SetupVariableGridType = CommonClass.GBenMAPGrid.GridDefinitionID;
      //                          setupVariableJoinAllValuesReturn.SetupVariableID = setupVariableJoinAllValues.SetupVariableID;
      //                          setupVariableJoinAllValuesReturn.SetupVariableName = setupVariableJoinAllValues.SetupVariableName;

      //                      }

      //                      //-----------------------------------------------------------------------------------------------
      //                      lstFunctionVariables.Add(setupVariableJoinAllValuesReturn);
      //                  }
      //                  //   lstFunctionVariables.Add(str);
      //              }

      //          }
      //      }
      //      catch (Exception ex)
      //      {
      //          //  return null;
      //      }
      //      //return lstResult;
      //  }
      
        /// <summary>
        /// 得到Valuation的值从ValuationFunction的Function里面
        /// </summary>
        /// <param name="FunctionString"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="Beta"></param>
        /// <param name="DeltaQ"></param>
        /// <param name="Q0"></param>
        /// <param name="Q1"></param>
        /// <param name="Incidence"></param>
        /// <param name="POP"></param>
        /// <param name="Prevalence"></param>
        /// <param name="dicSetupVariables"></param>
        /// <returns></returns>
        public static double getValueFromValuationFunctionString(string FunctionString, double A, double B, double C, double D, double AllGoodsIndex, double MedicalCostIndex, double WageIndex, double LagAdjustment, Dictionary<string, double> dicSetupVariables)
        {

            object result = ValuationEval.ValuationEval(FunctionString, A, B, C, D,AllGoodsIndex,MedicalCostIndex,WageIndex,LagAdjustment, dicSetupVariables);
            if (result is double)
            {
                if (double.IsNaN(Convert.ToDouble(result))) return 0;
                return Convert.ToDouble(result);
            }
            else
            {
                //return 0;
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

        /// <summary>
        /// 得到通货膨胀率
        /// </summary>
        /// <param name="InflationDataSetID"></param>
        /// <param name="Year"></param>
        /// <param name="AllGoodsIndex"></param>
        /// <param name="MedicalCostIndex"></param>
        /// <param name="WageIndex"></param>
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
                //    commandText = string.Format("select    AllGoodsIndex    ,   MedicalCostIndex ,   WageIndex        from InflationEntries where InflationDatasetID={0}  order by YYear desc", InflationDataSetID);
                //     fb = new ESIL.DBUtility.ESILFireBirdHelper();
                //      ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                //      DataRow dr = ds.Tables[0].Rows[0];

                    AllGoodsIndex =1;// Convert.ToDouble(dr["AllGoodsIndex"]);
                    MedicalCostIndex =1;// Convert.ToDouble(dr["MedicalCostIndex"]);
                    WageIndex =1;// Convert.ToDouble(dr["WageIndex"]);
                }
            }
            catch (Exception ex)
            {
                 
            }

            
        }
        /// <summary>
        /// 得到经济增长率From DataSetID,Year
        /// </summary>
        /// <param name="DataSetID"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        public static Dictionary<string, double> getIncomeGrowthFactorsFromDataSetIDAndYear(int DataSetID, int Year)
        {
            try
            {
                Dictionary<string, double> dicIncomeGrowthFactors=new Dictionary<string,double>();
                string commandText = string.Format(" select IncomeGrowthAdjdatasetID,YYear,Mean,EndPointGroups from IncomeGrowthAdjfactors where IncomeGrowthAdjdatasetID={0} and YYear={1}",DataSetID,Year);
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
    " where a.EndPointGroupID=b.EndPointGroupID and a.EndPointID=c.EndPointID and a.FunctionalFormID=d.FunctionalFormID and a.ValuationFunctionDatasetID=e.ValuationFunctionDatasetID and a.EndPointGroupID={0}  and e.SetupID={1} ", EndPointGroupID,CommonClass.MainSetup.SetupID);
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
                int iCol=-1;
                int iRow=-1;
                if (benMAPGrid is ShapefileGrid)
                {

                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (benMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                    {
                        fs = DotSpatial.Data.FeatureSet.Open(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (benMAPGrid as ShapefileGrid).ShapefileName + ".shp");
                        int i=0;
                        foreach (DataColumn dc in fs.DataTable.Columns)
                        {
                            if(dc.ColumnName.ToLower()=="col")
                                iCol=i;
                            if(dc.ColumnName.ToLower()=="row")
                                iRow=i;

                            i++;
                        }
                       foreach (DataRow dr in fs.DataTable.Rows)
                       {
                           lstRowCol.Add(new RowCol(){ Col= Convert.ToInt32(dr[iCol]), Row=Convert.ToInt32(dr[iRow])});
                       }
                        fs.Close();
                    }
                }
                else if (benMAPGrid is RegularGrid)
                {

                    if (File.Exists(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (benMAPGrid as RegularGrid).ShapefileName + ".shp"))
                    {
                        fs = DotSpatial.Data.FeatureSet.Open(CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\" + (benMAPGrid as RegularGrid).ShapefileName + ".shp");
                        int i=0;
                        foreach (DataColumn dc in fs.DataTable.Columns)
                        {
                            if(dc.ColumnName.ToLower()=="col")
                                iCol=i;
                            if(dc.ColumnName.ToLower()=="row")
                                iRow=i;

                            i++;
                        }
                       foreach (DataRow dr in fs.DataTable.Rows)
                       {
                           lstRowCol.Add(new RowCol(){ Col= Convert.ToInt32(dr[iCol]), Row=Convert.ToInt32(dr[iRow])});
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

        /// <summary>
        /// 得到一个网格的APVX值
        /// </summary>
        /// <param name="Row"></param>
        /// <param name="Col"></param>
        /// <param name="lstCRSelectFunctionCalculateValue"></param>
        /// <param name="AllGoodsIndex"></param>
        /// <param name="MedicalCostIndex"></param>
        /// <param name="WageIndex"></param>
        /// <param name="Income"></param>
        /// <param name="lhsDesignResult"></param>
        /// <param name="dicSetupVariables"></param>
        /// <returns></returns>
        public static Dictionary<string, double> getOneAllSelectValuationMethodAPVValue(Dictionary<int, Dictionary<string, double>> dicAllCRResultValues, AllSelectValuationMethod allSelectValuationMethod, List<RowCol> lstRowCol, CRSelectFunctionCalculateValue crSelectFunctionCalculateValue, double AllGoodsIndex, double MedicalCostIndex, double WageIndex, double Income, double[] lhsDesignResult, Dictionary<string, double> dicSetupVariables)
        {
             try
            {
                double PointEstimate = 0.0;
                double PointEstimateOut = 0.0;
                Dictionary<string, double> dicOut = new Dictionary<string, double>();
                List<APVValueAttribute> lstAPVValueAttribute = new List<APVValueAttribute>();
                 //to dic---
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
                        //得到了

                    }
                 
                //switch (allSelectValuationMethod.PoolingMethod)
                //{

                //    //case "None":
                //    //    break;
                //    case "SumDependent":
                //        break;
                //    case "SumIndependent":
                //        break;
                //    case "SubtractionDependent":
                //        break;
                //    case "SubtractionIndependent":
                //        break;
                //    case "SubjectiveWeights":
                //        break;
                //    case "RandomOrFixedEffects":
                //        break;
                //    case "FixedEffects":
                //        break;
                //}
                    return dicOut;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
        /// <summary>
        /// 得到所有CRResult 以便如计算
        /// </summary>
        /// <param name="lstCRSelectFunctionCalculateValue"></param>
        /// <returns></returns>
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
                
                //dicAll.Add(crfcv.CRSelectFunction.BenMAPHealthImpactFunction.ID, dicValues);
                dicAll.Add(i, dicValues);
                i++;
            }

            return dicAll;
        }
        public static void getPoolingMethodCRFromAllSelectCRFunction(bool isCalulate, ref List<AllSelectCRFunction> lstAllSelectCRFunctionNone,ref List<AllSelectCRFunction> lstAllSelectCRFunctionAll,int nodetype,List<string> lstColumns)
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
                        //------------Pooling-------------
                        if (alsc.PoolingMethod != "" && alsc.PoolingMethod != "None")
                        {
                            List<AllSelectCRFunction> lstSec = new List<AllSelectCRFunction>();
                            getAllChildCRNotNoneCalulate(alsc, lstAllSelectCRFunctionAll, ref lstSec);
                            //List<AllSelectCRFunction> lstSecResult = new List<AllSelectCRFunction>();
                            //foreach (AllSelectCRFunction alcr in lstSec)
                            //{
                            //    if (lstAllSelectCRFunctionNone.Where(p => p.ID == alcr.ID).Count() > 0)
                            //    {
                            //        lstSecResult.Add(lstAllSelectCRFunctionNone.Where(p => p.ID == alcr.ID).First());
                            //    }
                            //}
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

                            //CRID=9999+ acr.ID,
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
                                EndPointGroupID = lstSec.First().EndPointGroupID,// lstSec.Where(p => p.CRSelectFunctionCalculateValue != null).Select(a => a.CRSelectFunctionCalculateValue).ToList().First().CRSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID,
                                EndPoint = alsc.EndPoint,
                                Author = alsc.Author,
                                Qualifier = alsc.Qualifier,
                                strLocations = alsc.Location,
                                StartAge = alsc.StartAge == "" ? lstSec.Min(a=>a.CRSelectFunctionCalculateValue.CRSelectFunction.StartAge) : Convert.ToInt32(alsc.StartAge),
                                EndAge = alsc.EndAge == "" ? lstSec.Max(a=>a.CRSelectFunctionCalculateValue.CRSelectFunction.EndAge)  : Convert.ToInt32(alsc.EndAge),
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





                            //j++;
                            // -------------填入父值!-------------------
                            var Parent = alsc;
                            //for (int iParent = alsc.NodeType; iParent > 0; iParent--)
                            //{
                            //    if (iParent != alsc.NodeType)
                            //    {
                            //        Parent = lstAllSelectCRFunctionAll.Where(p => p.ID == Parent.PID).First();
                            //    }

                            //    switch (lstColumns[iParent - 1].Replace(" ", "").ToLower())
                            //    {
                            //        case "endpoint":
                            //            alsc.EndPoint = Parent.Name;
                            //            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndPoint = Parent.Name;
                            //            break;
                            //        case "author":
                            //            alsc.Author = Parent.Name;
                            //            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Author = Parent.Name;
                            //            break;
                            //        case "qualifier":
                            //            alsc.Qualifier = Parent.Name;
                            //            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Qualifier = Parent.Name;
                            //            break;
                            //        case "location":
                            //            alsc.Location = Parent.Name;
                            //            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.strLocations = Parent.Name;
                            //            break;
                            //        case "startage":
                            //            alsc.StartAge = Parent.Name;
                            //            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.StartAge = Convert.ToInt32(Parent.Name);
                            //            break;
                            //        case "endage":
                            //            alsc.EndAge = Parent.Name;
                            //            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.EndAge = Convert.ToInt32(Parent.Name);
                            //            break;
                            //        case "year":
                            //            alsc.Year = Parent.Name;
                            //            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Year = Convert.ToInt32(Parent.Name);
                            //            break;
                            //        case "otherpollutants":
                            //            alsc.OtherPollutants = Parent.Name;
                            //            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.OtherPollutants = Parent.Name;
                            //            break;
                            //        case "race":
                            //            alsc.Race = Parent.Name;
                            //            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Race = Parent.Name;
                            //            break;
                            //        case "ethnicity":
                            //            alsc.Ethnicity = Parent.Name;
                            //            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Ethnicity = Parent.Name;
                            //            break;
                            //        case "gender":
                            //            alsc.Gender = Parent.Name;
                            //            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Gender = Parent.Name;
                            //            break;
                            //        case "function":
                            //            alsc.Function = Parent.Name;
                            //            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Function = Parent.Name;
                            //            break;
                            //        case "pollutant":
                            //            alsc.Pollutant = Parent.Name;
                            //            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantName = Parent.Name;
                            //            break;
                            //        case "metric":
                            //            alsc.Metric = Parent.Name;
                            //            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.Metric = new Metric() { MetricName = Parent.Name };
                            //            break;
                            //        case "seasonalmetric":
                            //            alsc.SeasonalMetric = Parent.Name;
                            //            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric = new SeasonalMetric() { SeasonalMetricName = Parent.Name };
                            //            break;
                            //        case "metricstatistic":
                            //            alsc.MetricStatistic = Parent.Name;
                            //            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.MetricStatistic = MetricStatic.Mean;//"Parent.Name;
                            //            break;
                            //        case "dataset":
                            //            alsc.DataSet = Parent.Name;
                            //            alsc.CRSelectFunctionCalculateValue.CRSelectFunction.BenMAPHealthImpactFunction.DataSetName = Parent.Name;
                            //            break;
                            //        case "version":
                            //            alsc.Version = Convert.ToInt32(Parent.Name);
                            //            break;
                            //        //alsc. Version == Convert.ToInt32(Parent.Name);

                            //    }
                            //}
                            // alsc.NodeType = 100; //--最后重新设置他的nodetype-------modify by xiejp还是不要，保证不修改原来的架构!

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
        /// <summary>
        /// 得到是否运用Random--if PoolingMethod=RandomOrFixedEffects
        /// </summary>
        /// <param name="dicall"></param>
        /// <returns></returns>
        public static double getChidist(Dictionary<int, Dictionary<string, List<float>>> dicall, ref List<double> lstWeight)
        {
            //List<double> lstWeight = new List<double>();
            //Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            
            double dChidist = 0;
            //----------------判断是Random 还是Fix---------------------------------
            //---------首先Aggregation To Nation计算是否<0.05?Random:Fix------

            double dSum = 0, dRandom = 0;
            int idSum = 0;
            Dictionary<int, List<float>> dicAggregation = new Dictionary<int, List<float>>();
            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
            {
                List<float> lst = new List<float>();
                //List<float> lstF =  k.Value.First().Value;
                foreach (float f in k.Value.First().Value)
                {
                    lst.Add(0);
                }
                //--------To One--------------
                foreach (List<float> lstF in k.Value.Values)
                {
                    for (int ilstF = 0; ilstF < lstF.Count; ilstF++)
                    {
                        lst[ilstF] += lstF[ilstF];
                    }
                }
                dicAggregation.Add(k.Key, lst);

            }
            //------------------首先按Fix求出PointElement------
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
            //----------------End首先按Fix求出PointElement------------------------------

            idSum = 0;
            try
            {
                dChidist = 0;
                Meta.Numerics.Statistics.Distributions.ChiSquaredDistribution chi = new Meta.Numerics.Statistics.Distributions.ChiSquaredDistribution(lstWeight.Count - 1);
                dChidist = chi.RightProbability(dSum);
                //dChidist = app.WorksheetFunction.ChiDist(dSum, lstWeight.Count - 1);

            }
            catch
            {
                dChidist = 1;
            }

            return dChidist;


            //-------------End 判断是Random 还是Fix---------------------------------
        }
        public static CRSelectFunctionCalculateValue getPoolingMethodCRSelectFunctionCalculateValue(List<CRSelectFunctionCalculateValue> lstCRSelectFunctionCalculateValue, PoolingMethodTypeEnum poolingMethod,ref List<double> lstWeight)
        {
            try
            {
                
                CRSelectFunctionCalculateValue crv = new CRSelectFunctionCalculateValue();
                crv.CRCalculateValues = new List<CRCalculateValue>();
                Dictionary<int, Dictionary<string, List<float>>> dicall = getAllCrCalculateValues(lstCRSelectFunctionCalculateValue);
                //-------------------modify by xiejp 20120512因为IncidencePooling要在界面上显示出来必须求出Pooling后的Delta-Varance-Incidence-Baseline等--所有首先Dictionary
                Dictionary<int, Dictionary<string, CRCalculateValue>> dicAllCR = new Dictionary<int, Dictionary<string, CRCalculateValue>>();
                int iDicAllCR = 0;
                foreach (CRSelectFunctionCalculateValue crfcv in lstCRSelectFunctionCalculateValue)
                {
                    Dictionary<string, CRCalculateValue> dicValues = new Dictionary<string, CRCalculateValue>();
                    List<float> dArray = new List<float>();
                    foreach (CRCalculateValue crcv in crfcv.CRCalculateValues)
                    {
                        if(!dicValues.ContainsKey(crcv.Col + "," + crcv.Row))
                        dicValues.Add(crcv.Col + "," + crcv.Row, crcv);
                    }

                    //dicAll.Add(crfcv.CRSelectFunction.BenMAPHealthImpactFunction.ID, dicValues);
                    dicAllCR.Add(iDicAllCR, dicValues);
                    iDicAllCR++;
                }

                List<string> lstKey = new List<string>();
                Dictionary<string, List<float>> DicIE = new Dictionary<string, List<float>>();
               // Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
               
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
                //----------------判断是Random 还是Fix---------------------------------
                //---------首先Aggregation To Nation计算是否<0.05?Random:Fix------
                double dChidist = 0;
                List<double> lstRandomWeight = new List<double>();
                if (poolingMethod == PoolingMethodTypeEnum.RandomOrFixedEffects)
                {
                    dChidist = getChidist(dicall,ref lstRandomWeight);
                }



                //-------------End 判断是Random 还是Fix---------------------------------
                //  IEnumerable<string> ie =new IEnumerable<string>();// (IEnumerable<string>)query.Distinct();
                string[] sin = null;
                CRCalculateValue crCalculateValue = new CRCalculateValue();
                int i = 0;
                int j = 0;
                double dlatin = 0.0;
                List<List<float>> lstPercentile = new List<List<float>>();//--作为特殊Pooling方法的临时变量
                List<float> lstPooling = new List<float>();
                Random randomPooling = new Random();
               
                switch (poolingMethod)
                {

                    case PoolingMethodTypeEnum.SumDependent://简单的加
                        //--------只计算拉丁立体方的数字---去掉不对!
                        foreach (string s in ie)
                        {
                            crCalculateValue = new CRCalculateValue();
                            sin = s.Split(new char[] { ',' });
                            crCalculateValue.Col = Convert.ToInt32(sin[0]);
                            crCalculateValue.Row = Convert.ToInt32(sin[1]);
                            crCalculateValue.PointEstimate = 0;
                            //crCalculateValue.LstPercentile = new List<double>();
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
                                crCalculateValue.PercentOfBaseline =crCalculateValue.Baseline==0?0: Convert.ToSingle((crCalculateValue.Mean / crCalculateValue.Baseline) * 100);
                            }
                            crv.CRCalculateValues.Add(crCalculateValue);

                        }
                        break;

                    case PoolingMethodTypeEnum.SumIndependent:
                        //Results are summed assuming that they are independent.  A Monte Carlo simulation is used.  
                        //At each iteration, a random point is chosen from the Latin Hypercube of each result, 
                        //and the sum of these values is put in a holding container.  After some number of iterations, 
                        //the holding container is sorted low to high and binned down to the appropriate number of Latin Hypercube points. 
                        foreach (string s in ie)
                        {
                            crCalculateValue = new CRCalculateValue();
                            sin = s.Split(new char[] { ',' });
                            crCalculateValue.Col = Convert.ToInt32(sin[0]);
                            crCalculateValue.Row = Convert.ToInt32(sin[1]);
                            crCalculateValue.PointEstimate = 0;
                            //crCalculateValue.LstPercentile = new List<double>();
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
                                        //    crCalculateValue.LstPercentile = k.Value[s];
                                        //    crCalculateValue.LstPercentile.RemoveAt(0);
                                        //}
                                        //else if (k.Value.ContainsKey(s))
                                        //{
                                        //    i = 0;
                                        //    while (i < crCalculateValue.LstPercentile.Count)
                                        //    {
                                        //        crCalculateValue.LstPercentile[i] += k.Value[s][i + 1];
                                        //        i++;
                                        //    }
                                    }
                                }
                                catch
                                {
                                }
                                iDicAllCR++;
                            }
                            //---------------modify by xiejp 20120217 --修正Pooling的算法-----------------------------
                            //                            for (i in 1:5000)
                            //{laden<-sample(pctile[,1],size=1,replace=TRUE)
                            //krews<-sample(pctile[,2],size=1,replace=TRUE)
                            //s<-laden+krews
                            //indpsum[i,1]<-laden
                            //indpsum[i,2]<-krews
                            //indpsum[i,3]<-s
                            //}
                            lstPooling = new List<float>();

                            for (int iPer = 0; iPer < 5000; iPer++)
                            {
                                float d = lstPercentile[0][randomPooling.Next(0, CommonClass.CRLatinHypercubePoints - 1)];
                                for (int iPerSec = 1; iPerSec < lstPercentile.Count; iPerSec++)
                                {
                                    d =d+ lstPercentile[iPerSec][randomPooling.Next(0, CommonClass.CRLatinHypercubePoints - 1)];
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
                                crCalculateValue.PercentOfBaseline =crCalculateValue.Baseline==0?0: Convert.ToSingle((crCalculateValue.Mean / crCalculateValue.Baseline) * 100);
                            }
                            crv.CRCalculateValues.Add(crCalculateValue);

                        }
                        break;
                    case PoolingMethodTypeEnum.SubtractionDependent:
                        //--------只计算拉丁立体方的数字
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
                                        //crCalculateValue.PointEstimate -= k.Value[s].First();
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
                                crCalculateValue.PercentOfBaseline =crCalculateValue.Baseline==0?0: Convert.ToSingle((crCalculateValue.Mean / crCalculateValue.Baseline) * 100);
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

                                        //if (crCalculateValue.LstPercentile == null)
                                        //{

                                        //    //crCalculateValue.LstPercentile = k.Value[s];
                                        //    //crCalculateValue.LstPercentile.RemoveAt(0);
                                        //    lstPercentile.Add(k.Value[s].GetRange(1, CommonClass.CRLatinHypercubePoints));
                                        //}
                                        //else
                                        //{
                                        //    //crCalculateValue.PointEstimate -= k.Value[s].First();
                                        //    i = 0;
                                        //    while (i < crCalculateValue.LstPercentile.Count)
                                        //    {
                                        //        crCalculateValue.LstPercentile[i] -= k.Value[s][i + 1];
                                        //        i++;
                                        //    }
                                        //}
                                    }
                                    catch
                                    {
                                    }
                                }
                                iDicAllCR++;
                            }
                            //---------------modify by xiejp 20120217 --修正Pooling的算法-----------------------------
                            lstPooling = new List<float>();

                            for (int iPer = 0; iPer < 5000; iPer++)
                            {
                                float d = lstPercentile[0][randomPooling.Next(0, CommonClass.CRLatinHypercubePoints - 1)];
                                for (int iPerSec = 1; iPerSec < lstPercentile.Count; iPerSec++)
                                {
                                    d = d-lstPercentile[iPerSec][randomPooling.Next(0, CommonClass.CRLatinHypercubePoints - 1)];
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
                                crCalculateValue.PercentOfBaseline =crCalculateValue.Baseline==0?0: Convert.ToSingle((crCalculateValue.Mean / crCalculateValue.Baseline) * 100);
                            }
                            crv.CRCalculateValues.Add(crCalculateValue);


                        }
                        break;
                    case PoolingMethodTypeEnum.SubjectiveWeights:
                        //-----------如果加起来不等于1，首先变成=1
                        if (lstWeight!=null && lstWeight.Count>0 &&lstWeight.Sum() != 1)
                        {
                            double dWeightSum = lstWeight.Sum();
                            for (int iWeightSum = 0; iWeightSum < lstWeight.Count; iWeightSum++)
                            {
                                lstWeight[iWeightSum] = lstWeight[iWeightSum] / dWeightSum;
                            }
                        }
                        //--------只计算拉丁立体方的数字
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
                                        crCalculateValue.Baseline += Convert.ToSingle( dicAllCR[iDicAllCR][s].Baseline * lstWeight[j]);
                                        crCalculateValue.Population += Convert.ToSingle( dicAllCR[iDicAllCR][s].Population * lstWeight[j]);
                                        crCalculateValue.Incidence += Convert.ToSingle( dicAllCR[iDicAllCR][s].Incidence * lstWeight[j]);
                                        crCalculateValue.Delta += Convert.ToSingle( dicAllCR[iDicAllCR][s].Delta * lstWeight[j]);
                                        if (crCalculateValue.LstPercentile == null)
                                        {
                                            //------modify by xiejp 20120217-----------
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
                                crCalculateValue.PercentOfBaseline =crCalculateValue.Baseline==0?0: Convert.ToSingle((crCalculateValue.Mean / crCalculateValue.Baseline) * 100);
                            }
                            crv.CRCalculateValues.Add(crCalculateValue);

                        }
                        break;
                    case PoolingMethodTypeEnum.RandomOrFixedEffects://weight 随机数 加起来为1
                      
                    case PoolingMethodTypeEnum.FixedEffects:
                        lstWeight = new List<double>();
                        foreach (string s in ie)
                        {
                            //-------------modify by xiejp (1/Variance)/sum(1/Variance)
                            if (s == "16,71")
                            {
 
                            }
                            double sumVariance = 0;
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
                            {

                                if (k.Value.ContainsKey(s))
                                {
                                    if(Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0])!=0)
                                    sumVariance += 1.00000 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]);
                                }
                            }
                            lstWeight = new List<double>();
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicall)
                            {
                                if (sumVariance != 0 &&k.Value.ContainsKey(s)&& Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]) != 0)
                                    lstWeight.Add((1.00000 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0])) / sumVariance);
                                else
                                {
                                    //lstWeight.Add(0);
                                    lstWeight.Add(1.0000 /Convert.ToDouble( dicall.Count));
                                }
                            }
                            //-------如果lstWeight为NaN----则置为0----------
                            for (int iw = 0; iw < lstWeight.Count; iw++)
                            {
                                if (double.IsNaN(lstWeight[iw]))
                                {
                                    lstWeight[iw] = 1.0000 / Convert.ToDouble(dicall.Count);

                                }
                            }
                            //double sumVariance= lstCRSelectFunctionCalculateValue.Sum(p=>p.c
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
                                            //------modify by xiejp 20120217-----------
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
                            //----------如果是Random,则采用Random的用法---------------------
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
                                    //------------do Random------------------
                                    //dSum = 0;
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
                                            //lstWeight.Add(0);
                                            lstWeight.Add(1.0000 / Convert.ToDouble(dicall.Count));
                                        }
                                    }
                                    dSum = lstWeight.Sum();
                                    for (int iw = 0; iw < lstWeight.Count; iw++)
                                    {
                                        lstWeight[iw] = Convert.ToDouble(lstWeight[iw]) / dSum;
                                    }
                                    //-------如果lstWeight为NaN----则置为0----------
                                    for (int iw = 0; iw < lstWeight.Count; iw++)
                                    {
                                        if (double.IsNaN(lstWeight[iw]))
                                        {
                                            //lstWeight[iw] = 0;
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
                                                    //------modify by xiejp 20120217-----------
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
                                    //------------使用lstRandomWeight来算---------------
                                    //-------如果lstWeight为NaN----则置为0----------
                                    for (int iw = 0; iw < lstRandomWeight .Count; iw++)
                                    {

                                        if (double.IsNaN(lstRandomWeight[iw]))
                                        {
                                            //lstWeight[iw] = 0;
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
                                                    //------modify by xiejp 20120217-----------
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
                            //------------------------------------------------------------end Random----------
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
                                crCalculateValue.PercentOfBaseline =crCalculateValue.Baseline==0?0: Convert.ToSingle((crCalculateValue.Mean / crCalculateValue.Baseline) * 100);
                            }
                            crv.CRCalculateValues.Add(crCalculateValue);
                             

                        }
                        break;
                }
                dicall.Clear();
                dicAllCR.Clear();
                GC.Collect();
                //if (crv.CRCalculateValues[0].PointEstimate.ToString() == "NaN")
                //{ 
                //}
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
                //是否有Setup变量--如果有，则得到------
                List<string> lstSystemVariableName = Configuration.ConfigurationCommonClass.getAllSystemVariableNameList();
                List<SetupVariableJoinAllValues> lstSetupVariable = new List<SetupVariableJoinAllValues>();
                int iGridID = CommonClass.GBenMAPGrid.GridDefinitionID;
                if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null
                                       && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
                {
                    iGridID = CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID;
                }
                Configuration.ConfigurationCommonClass.getSetupVariableNameListFromDatabaseFunction(CommonClass.ValuationMethodPoolingAndAggregation.VariableDatasetID,iGridID, allSelectValuationMethod.Function, lstSystemVariableName, ref lstSetupVariable);
                //Dictionary<string, double> dicSetupVariable = new Dictionary<string, double>();
                //foreach (SetupVariableValues sv in lstSetupVariable)
                //{
                //    dicSetupVariable.Add(sv.Col + "," + sv.Row, sv.Value);
                //}
                // Dictionary<string, double> dicVariable = getDicSetupVariableColRow(col, row, lstSetupVariable, baseControlGroup.GridType.GridDefinitionID, lstGridRelationship);
                Dictionary<string, Dictionary<string, float>> DicAll = new Dictionary<string, Dictionary<string, float>>();
                foreach (SetupVariableJoinAllValues sv in lstSetupVariable)
                {
                    //dicSetupVariable.Add(sv..Col + "," + sv.Row, sv.Value);
                    Dictionary<string, float> dic = new Dictionary<string, float>();
                    dic = sv.lstValues.ToDictionary(p => p.Col + "," + p.Row, p => p.Value);
                    //foreach (SetupVariableValues svv in sv.lstValues)
                    //{

                    //        dic.Add(svv.Col + "," + svv.Row, svv.Value);

                    //}
                    DicAll.Add(sv.SetupVariableName.ToLower(), dic);
                }
                List<GridRelationship> lstGridRelationshipAll = CommonClass.LstGridRelationshipAll;
                allSelectValuationMethodAndValue.lstAPVValueAttributes = new List<APVValueAttribute>();
                //生成value-------------------------
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
                //List<double> lstMonte = getLHSArrayValuationFunction(100, allSelectValuationMethod.BenMAPValuationFunction).ToList();
                //if (allSelectValuationMethod.lstMonte == null || allSelectValuationMethod.lstMonte.Count == 0)
                //{
                allSelectValuationMethod.lstMonte = new List<LatinPoints>();//
                //for (int iMonto = 0; iMonto < 10; iMonto++)
                //{
                int iRandomSeed = Convert.ToInt32(DateTime.Now.Hour+""+DateTime.Now.Minute+DateTime.Now.Second+DateTime.Now.Millisecond);
                if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
                CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.RandomSeed != "" &&
                CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.RandomSeed != "Random Integer" && CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.RandomSeed != null)
                    iRandomSeed = Convert.ToInt32(CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.RandomSeed);

                allSelectValuationMethod.lstMonte.Add(new LatinPoints() { values = getLHSArrayValuationFunctionSeed(100, allSelectValuationMethod.BenMAPValuationFunction, iRandomSeed).ToList() });
                //}
                //}
                int iSeed = 0;//
                //if (CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
                //    CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.RandomSeed != "" &&CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.RandomSeed!=null)
                //    iSeed = Convert.ToInt16(CommonClass.ValuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.RandomSeed) - 1;

                Dictionary<string, double> dicVariable = new Dictionary<string, double>();
                foreach (CRCalculateValue crCalculateValue in crSelectFunctionCalculateValue.CRCalculateValues)
                {
                    APVValueAttribute apvValueAttribute = new APVValueAttribute();
                    //income = 1;
                    if (crCalculateValue.PointEstimate != 0)
                    {
                        //Dictionary<string, double> dicVariable = Configuration.ConfigurationCommonClass.getDicSetupVariableColRow(crCalculateValue.Col, crCalculateValue.Row, lstSetupVariable, CommonClass.GBenMAPGrid.GridDefinitionID, lstGridRelationshipAll);
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

                        //----------------------
                        //--------------modify by xiejp 20111215 每个计算100次Monte Carlo--然后排序，最后得到100
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
                            //------------排序------得到100
                            lstValue.Sort();
                            //foreach (double dMonte in allSelectValuationMethod.lstMonte)
                            //{
                            //    if (allSelectValuationMethod.BenMAPValuationFunction.A == 0.0)
                            //        lstValue.Add(0);
                            //    else
                            //        lstValue.Add(Convert.ToSingle(Math.Round(apvValueAttribute.PointEstimate * d * dMonte / (crCalculateValue.PointEstimate * allSelectValuationMethod.BenMAPValuationFunction.A), 4)));

                            //}
                            apvValueAttribute.LstPercentile = getMedianSample(lstValue, 100);
                            //for (int ilstValue = 0; ilstValue < 100; ilstValue++)
                            //{
                            //    apvValueAttribute.LstPercentile.Add(lstValue.GetRange(ilstValue * (lstValue.Count / 100), (lstValue.Count / 100)).Median());
                            //    //lhsResultArray[LatinHypercubePoints - i-1] = lstsamples[(LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints) > lstsamples.Count-1 ? lstsamples.Count-1 : (LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints)];
                            //    //i++;
                            //}

                            //------------------修正算法----------

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

                //--------------------Aggregation-------------------------------


                //--------------------------------------------------------------
                DicAll = null;
                return allSelectValuationMethodAndValue;


            }
            catch
            {
                return null;
            }
        }
        public static AllSelectValuationMethodAndValue getPoolingLstAllSelectValuationMethodAndValue(List<AllSelectValuationMethodAndValue> lstAllSelectValuationMethodAndValue, PoolingMethodTypeEnum poolingMethod,List<double> lstWeight)
        { 
            try
            {
                AllSelectValuationMethodAndValue allSelectValuationMethodAndValue = new AllSelectValuationMethodAndValue();
                allSelectValuationMethodAndValue.lstAPVValueAttributes = new List<APVValueAttribute>();

                //首先得到一个dic
                Dictionary<int, Dictionary<string, List<float>>> dicAll = new Dictionary<int, Dictionary<string, List<float>>>();
                Dictionary<string, List<float>> dicValue = new Dictionary<string, List<float>>();
                int i = 0;
                List<string> lstAllColRow = new List<string>();
                APVValueAttribute aValueAttribute = new APVValueAttribute();
                string[] strColRow;
                float dPoint=0;
               
                while (i < lstAllSelectValuationMethodAndValue.Count)
                {
                    dicValue = new Dictionary<string, List<float>>();

                    foreach (APVValueAttribute apvValueAttribute in lstAllSelectValuationMethodAndValue[i].lstAPVValueAttributes)
                    {
                        List<float> lstDouble = new List<float>();
                        lstDouble.Add(apvValueAttribute.PointEstimate);
                        if(apvValueAttribute.LstPercentile!=null)
                        lstDouble.AddRange(apvValueAttribute.LstPercentile);
                        dicValue.Add(apvValueAttribute.Col + ","+ apvValueAttribute.Row, lstDouble);
                    }
                    dicAll.Add(i, dicValue);
                    lstAllColRow=lstAllColRow.Union(dicValue.Keys.ToList()).ToList();
                    i++;
                }
                List<List<float>> lstPercentile = new List<List<float>>();//--作为特殊Pooling方法的临时变量 
                List<float> lstPooling = new List<float>();
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

                    case PoolingMethodTypeEnum.SumDependent://简单的加
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
                                        lstPooling = k.Value[s].GetRange(1, k.Value[s].Count-1 );
                                    }
                                    else
                                    {
                                        i = 1;
                                        while (i < k.Value[s].Count )
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
                                    //if (lstPooling.Count == 0)
                                    //{
                                    //    lstPooling = k.Value[s].GetRange(1, k.Value[s].Count-1 );
                                    //}
                                    //else
                                    //{
                                    //    i = 1;
                                    //    while (i < k.Value[s].Count )
                                    //    {
                                    //        lstPooling[i - 1] += k.Value[s][i];
                                    //        i++;
                                    //    }

                                    //}

                                }
                                catch
                                { }
                            }
                            //-------doPercent---
                            lstPooling = new List<float>();

                            for (int iPer = 0; iPer < 5000; iPer++)
                            {
                                float d = lstPercentile[0][randomPooling.Next(0, 100 - 1)];
                                for (int iPerSec = 1; iPerSec < lstPercentile.Count; iPerSec++)
                                {
                                    d = d+lstPercentile[iPerSec][randomPooling.Next(0, 100 - 1)];
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
                        //Results are summed assuming that they are independent.  A Monte Carlo simulation is used.  
                        //At each iteration, a random point is chosen from the Latin Hypercube of each result, 
                        //and the sum of these values is put in a holding container.  After some number of iterations, 
                        //the holding container is sorted low to high and binned down to the appropriate number of Latin Hypercube points. 

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
                                    if(iSubtractionDependent==0)
                                        dPoint += k.Value[s].First();
                                    else
                                        dPoint -= k.Value[s].First();
                                    iSubtractionDependent++;
                                    if (lstPooling.Count == 0)
                                    {
                                        
                                        lstPooling = k.Value[s].GetRange(1, k.Value[s].Count-1 );
                                    }
                                    else
                                    {
                                       
                                        i = 1;
                                        while (i < k.Value[s].Count )
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
                                    //if (lstPooling.Count == 0)
                                    //{

                                    //    lstPooling = k.Value[s].GetRange(1, k.Value[s].Count - 1);
                                    //}
                                    //else
                                    //{

                                    //    i = 1;
                                    //    while (i < k.Value[s].Count)
                                    //    {
                                    //        lstPooling[i - 1] -= k.Value[s][i];
                                    //        i++;
                                    //    }

                                    //}

                                }
                                catch
                                { }
                            }
                            //-------doPercent---
                            lstPooling = new List<float>();

                            for (int iPer = 0; iPer < 5000; iPer++)
                            {
                                float d = lstPercentile[0][randomPooling.Next(0, 100 - 1)];
                                for (int iPerSec = 1; iPerSec < lstPercentile.Count; iPerSec++)
                                {
                                    d = d-lstPercentile[iPerSec][randomPooling.Next(0, 100 - 1)];
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
                        //-----------如果加起来不等于1，首先变成=1
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
                                    //------modify by xiejp 20120217-----------
                                    for (int iPer = 0; iPer < Convert.ToInt32(Math.Round(lstWeight[k.Key] * 100)); iPer++)
                                    {
                                        lstPooling.AddRange(k.Value[s].GetRange(1, 100));
                                    }
                                    //if (lstPooling.Count == 0)
                                    //{
                                    //    i = 1;
                                    //    while (i < k.Value[s].Count )
                                    //    {
                                    //        lstPooling.Add(Convert.ToSingle(k.Value[s][i] * lstWeight[k.Key]));
                                    //        i++;
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    i = 1;
                                    //    while (i < k.Value[s].Count )
                                    //    {
                                    //        lstPooling[i - 1] += Convert.ToSingle(k.Value[s][i] * lstWeight[k.Key]);
                                    //        i++;
                                    //    }

                                    //}

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
                            //aValueAttribute.LstPercentile = lstPooling;
                            if (aValueAttribute.PointEstimate != 0)
                            {
                                aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(aValueAttribute.LstPercentile);
                                aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(aValueAttribute.LstPercentile, dPoint);
                                aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(aValueAttribute.LstPercentile, dPoint);
                            }
                            allSelectValuationMethodAndValue.lstAPVValueAttributes.Add(aValueAttribute);

                        }

                        break;
                    case PoolingMethodTypeEnum.RandomOrFixedEffects://weight 随机数 加起来为1
                        //List<int> lstRandom = new List<int>();
                        //Random random=new Random();
                        // i = 0;
                        // while (i < dicAll.Count )
                        // {
                        //     lstRandom.Add(random.Next(10));
                        //     i++;
                        // }
                        // lstWeight = new List<double>();
                        // i = 0;
                        //int iSum= lstRandom.Sum();
                        // while (i < dicAll.Count )
                        // {
                        //     lstWeight.Add(Convert.ToDouble(lstRandom[i])/Convert.ToDouble(iSum));
                        //     i++;
                        // }
                        // foreach (string s in lstAllColRow)
                        // {

                        //     strColRow = s.Split(new char[] { ',' });
                        //     aValueAttribute = new APVValueAttribute();
                        //     aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                        //     aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                        //     lstPooling = new List<float>();
                        //     dPoint = 0;
                        //     //-------------modify by xiejp (1/Variance)/sum(1/Variance)
                        //     double sumVariance = 0;
                        //     foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                        //     {

                        //         if (k.Value.ContainsKey(s))
                        //         {
                        //             sumVariance += 1.00000/Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]);
                        //         }
                        //     }
                        //     lstWeight = new List<double>();
                        //     foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                        //     {
                        //         if (sumVariance != 0)
                        //         {
                        //             lstWeight.Add((1.00000/Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0])) / sumVariance);
                        //         }
                        //         else
                        //         {
                        //             lstWeight.Add(0);
                        //         }

                        //     }
                        //     lstPercentile = new List<List<float>>();
                        //     foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                        //     {
                        //         try
                        //         {
                        //             dPoint += Convert.ToSingle(k.Value[s].First() * lstWeight[k.Key]);
                        //             //------modify by xiejp 20120217-----------
                        //             for (int iPer = 0; iPer < Convert.ToInt32(Math.Round(lstWeight[k.Key] * 100)); iPer++)
                        //             {
                        //                 lstPooling.AddRange(k.Value[s].GetRange(1, 100));
                        //             }
                        //             //if (lstPooling.Count == 0)
                        //             //{
                        //             //    i = 1;
                        //             //    while (i < k.Value[s].Count )
                        //             //    {
                        //             //        lstPooling.Add(Convert.ToSingle(k.Value[s][i] * lstWeight[k.Key]));
                        //             //        i++;
                        //             //    }
                        //             //}
                        //             //else
                        //             //{
                        //             //    i = 1;
                        //             //    while (i < k.Value[s].Count )
                        //             //    {
                        //             //        lstPooling[i - 1] += Convert.ToSingle(k.Value[s][i] * lstWeight[k.Key]);
                        //             //        i++;
                        //             //    }

                        //             //}

                        //         }
                        //         catch
                        //         { }
                        //     }
                        //     if (lstPooling.Count > 0)
                        //         aValueAttribute.LstPercentile = getMedianSample(lstPooling, 100);
                        //     else
                        //     {
                        //         aValueAttribute.LstPercentile = new List<float>();
                        //         for (int iPer = 0; iPer < 100; iPer++)
                        //         {
                        //             aValueAttribute.LstPercentile.Add(0);
                        //         }
                        //     }
                        //     aValueAttribute.PointEstimate = dPoint;
                        //     aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(aValueAttribute.LstPercentile);
                        //     aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(aValueAttribute.LstPercentile, dPoint);
                        //     aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(aValueAttribute.LstPercentile, dPoint);
                        //     allSelectValuationMethodAndValue.lstAPVValueAttributes.Add(aValueAttribute);

                        // }
                        //break;
                    case PoolingMethodTypeEnum.FixedEffects://weight 自动为1/count
                          
                         i = 0;
                          
                         foreach (string s in lstAllColRow)
                         {
                             strColRow = s.Split(new char[] { ',' });
                             aValueAttribute = new APVValueAttribute();
                             aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                             aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                             lstPooling = new List<float>();
                             dPoint = 0;
                             //-------------modify by xiejp (1/Variance)/sum(1/Variance)
                             double sumVariance = 0;
                             foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                             {

                                 if (k.Value.ContainsKey(s) && Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0])!=0)
                                 {
                                     sumVariance += 1.00000 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]);
                                 }
                             }
                             lstWeight = new List<double>();
                             foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                             {
                                 if (sumVariance != 0 && Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0])!=0)
                                 {
                                     lstWeight.Add((1.00000/Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0])) / sumVariance);
                                 }
                                 else
                                 {
                                     lstWeight.Add(0);
                                 }

                             }
                             //-------如果lstWeight为NaN----则置为0----------
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
                                     //------modify by xiejp 20120217-----------
                                     for (int iPer = 0; iPer < Convert.ToInt32(Math.Round(lstWeight[k.Key] * 100)); iPer++)
                                     {
                                         lstPooling.AddRange(k.Value[s].GetRange(1, 100));
                                     }
                                     //if (lstPooling.Count == 0)
                                     //{
                                     //    i = 1;
                                     //    while (i < k.Value[s].Count )
                                     //    {
                                     //        lstPooling.Add(Convert.ToSingle(k.Value[s][i] * lstWeight[k.Key]));
                                     //        i++;
                                     //    }
                                     //}
                                     //else
                                     //{
                                     //    i = 1;
                                     //    while (i < k.Value[s].Count )
                                     //    {
                                     //        lstPooling[i - 1] += Convert.ToSingle(k.Value[s][i] * lstWeight[k.Key]);
                                     //        i++;
                                     //    }

                                     //}

                                 }
                                 catch
                                 { }
                             }
                             //----------如果是Random,则采用Random的用法---------------------
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
                                     //------------do Random------------------
                                     //dSum = 0;
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
                                     //-------如果lstWeight为NaN----则置为0----------
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
                                     //-------如果lstWeight为NaN----则置为0----------
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
                             //------------------------------------------------------------end Random----------
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

                //首先得到一个dic
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
                List<List<float>> lstPercentile = new List<List<float>>();//--作为特殊Pooling方法的临时变量 
                List<float> lstPooling = new List<float>();
                Random randomPooling = new Random();
                switch (poolingMethod)
                {

                    case PoolingMethodTypeEnum.SumDependent://简单的加
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
                                    //if (lstPooling.Count == 0)
                                    //{
                                    //    lstPooling = k.Value[s].GetRange(1, k.Value[s].Count-1 );
                                    //}
                                    //else
                                    //{
                                    //    i = 1;
                                    //    while (i < k.Value[s].Count )
                                    //    {
                                    //        lstPooling[i - 1] += k.Value[s][i];
                                    //        i++;
                                    //    }

                                    //}

                                }
                                catch
                                { }
                            }
                            //-------doPercent---
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
                        //Results are summed assuming that they are independent.  A Monte Carlo simulation is used.  
                        //At each iteration, a random point is chosen from the Latin Hypercube of each result, 
                        //and the sum of these values is put in a holding container.  After some number of iterations, 
                        //the holding container is sorted low to high and binned down to the appropriate number of Latin Hypercube points. 

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
                                    //if (lstPooling.Count == 0)
                                    //{

                                    //    lstPooling = k.Value[s].GetRange(1, k.Value[s].Count - 1);
                                    //}
                                    //else
                                    //{

                                    //    i = 1;
                                    //    while (i < k.Value[s].Count)
                                    //    {
                                    //        lstPooling[i - 1] -= k.Value[s][i];
                                    //        i++;
                                    //    }

                                    //}

                                }
                                catch
                                { }
                            }
                            //-------doPercent---
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
                        //-----------如果加起来不等于1，首先变成=1
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
                                    //------modify by xiejp 20120217-----------
                                    for (int iPer = 0; iPer < Convert.ToInt32(Math.Round(lstWeight[k.Key] * 100)); iPer++)
                                    {
                                        lstPooling.AddRange(k.Value[s].GetRange(1, 100));
                                    }
                                    //if (lstPooling.Count == 0)
                                    //{
                                    //    i = 1;
                                    //    while (i < k.Value[s].Count )
                                    //    {
                                    //        lstPooling.Add(Convert.ToSingle(k.Value[s][i] * lstWeight[k.Key]));
                                    //        i++;
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    i = 1;
                                    //    while (i < k.Value[s].Count )
                                    //    {
                                    //        lstPooling[i - 1] += Convert.ToSingle(k.Value[s][i] * lstWeight[k.Key]);
                                    //        i++;
                                    //    }

                                    //}

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
                    case PoolingMethodTypeEnum.RandomOrFixedEffects://weight 随机数 加起来为1
                        //List<int> lstRandom = new List<int>();
                        //Random random=new Random();
                        // i = 0;
                        // while (i < dicAll.Count )
                        // {
                        //     lstRandom.Add(random.Next(10));
                        //     i++;
                        // }
                        // lstWeight = new List<double>();
                        // i = 0;
                        //int iSum= lstRandom.Sum();
                        // while (i < dicAll.Count )
                        // {
                        //     lstWeight.Add(Convert.ToDouble(lstRandom[i])/Convert.ToDouble(iSum));
                        //     i++;
                        // }
                        //foreach (string s in lstAllColRow)
                        //{

                        //    strColRow = s.Split(new char[] { ',' });
                        //    aValueAttribute = new QALYValueAttribute();
                        //    aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                        //    aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                        //    lstPooling = new List<float>();
                        //    dPoint = 0;
                        //    //-------------modify by xiejp (1/Variance)/sum(1/Variance)
                        //    double sumVariance = 0;
                        //    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                        //    {

                        //        if (k.Value.ContainsKey(s))
                        //        {
                        //            sumVariance += 1.00000 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]);
                        //        }
                        //    }
                        //    lstWeight = new List<double>();
                        //    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                        //    {
                        //        if (sumVariance != 0)
                        //        {
                        //            lstWeight.Add((1.00000 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0])) / sumVariance);
                        //        }
                        //        else
                        //        {
                        //            lstWeight.Add(0);
                        //        }

                        //    }
                        //    lstPercentile = new List<List<float>>();
                        //    foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                        //    {
                        //        try
                        //        {
                        //            dPoint += Convert.ToSingle(k.Value[s].First() * lstWeight[k.Key]);
                        //            //------modify by xiejp 20120217-----------
                        //            for (int iPer = 0; iPer < Convert.ToInt32(Math.Round(lstWeight[k.Key] * 100)); iPer++)
                        //            {
                        //                lstPooling.AddRange(k.Value[s].GetRange(1, 100));
                        //            }
                        //            //if (lstPooling.Count == 0)
                        //            //{
                        //            //    i = 1;
                        //            //    while (i < k.Value[s].Count )
                        //            //    {
                        //            //        lstPooling.Add(Convert.ToSingle(k.Value[s][i] * lstWeight[k.Key]));
                        //            //        i++;
                        //            //    }
                        //            //}
                        //            //else
                        //            //{
                        //            //    i = 1;
                        //            //    while (i < k.Value[s].Count )
                        //            //    {
                        //            //        lstPooling[i - 1] += Convert.ToSingle(k.Value[s][i] * lstWeight[k.Key]);
                        //            //        i++;
                        //            //    }

                        //            //}

                        //        }
                        //        catch
                        //        { }
                        //    }
                        //    if (lstPooling.Count > 0)
                        //        aValueAttribute.LstPercentile = getMedianSample(lstPooling, 100);
                        //    else
                        //    {
                        //        aValueAttribute.LstPercentile = new List<float>();
                        //        for (int iPer = 0; iPer < 100; iPer++)
                        //        {
                        //            aValueAttribute.LstPercentile.Add(0);
                        //        }
                        //    }
                        //    aValueAttribute.PointEstimate = dPoint;
                        //    aValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(aValueAttribute.LstPercentile);
                        //    aValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(aValueAttribute.LstPercentile, dPoint);
                        //    aValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(aValueAttribute.LstPercentile, dPoint);
                        //    allSelectQALYMethodAndValue.lstQALYValueAttributes.Add(aValueAttribute);

                        //}
                        //break;
                    case PoolingMethodTypeEnum.FixedEffects://weight 自动为1/count

                        i = 0;

                        foreach (string s in lstAllColRow)
                        {
                            strColRow = s.Split(new char[] { ',' });
                            aValueAttribute = new QALYValueAttribute();
                            aValueAttribute.Col = Convert.ToInt32(strColRow[0]);
                            aValueAttribute.Row = Convert.ToInt32(strColRow[1]);
                            lstPooling = new List<float>();
                            dPoint = 0;
                            //-------------modify by xiejp (1/Variance)/sum(1/Variance)
                            double sumVariance = 0;
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {

                                if (k.Value.ContainsKey(s) && Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0])!=0)
                                {
                                    sumVariance += 1.00000 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0]);
                                }
                            }
                            lstWeight = new List<double>();
                            foreach (KeyValuePair<int, Dictionary<string, List<float>>> k in dicAll)
                            {
                                if (sumVariance != 0 && Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0])!=0)
                                {
                                    lstWeight.Add((1.00000 / Configuration.ConfigurationCommonClass.getVariance(k.Value[s].GetRange(1, k.Value[s].Count - 1), k.Value[s][0])) / sumVariance);
                                }
                                else
                                {
                                    lstWeight.Add(0);
                                }

                            }
                            //-------如果lstWeight为NaN----则置为0----------
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
                                    //------modify by xiejp 20120217-----------
                                    for (int iPer = 0; iPer < Convert.ToInt32(Math.Round(lstWeight[k.Key] * 100)); iPer++)
                                    {
                                        lstPooling.AddRange(k.Value[s].GetRange(1, 100));
                                    }
                                    //if (lstPooling.Count == 0)
                                    //{
                                    //    i = 1;
                                    //    while (i < k.Value[s].Count )
                                    //    {
                                    //        lstPooling.Add(Convert.ToSingle(k.Value[s][i] * lstWeight[k.Key]));
                                    //        i++;
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    i = 1;
                                    //    while (i < k.Value[s].Count )
                                    //    {
                                    //        lstPooling[i - 1] += Convert.ToSingle(k.Value[s][i] * lstWeight[k.Key]);
                                    //        i++;
                                    //    }

                                    //}

                                }
                                catch
                                { }
                            }
                            //----------如果是Random,则采用Random的用法---------------------
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
                                    //------------do Random------------------
                                    //dSum = 0;
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
                                    //-------如果lstWeight为NaN----则置为0----------
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
                                    //-------如果lstWeight为NaN----则置为0----------
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
                            //------------------------------------------------------------end Random----------
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

        public static AllSelectValuationMethodAndValue  ApplyAllSelectValuationMethodAndValueAggregation(GridRelationship gridRelationship, BenMAPGrid benMAPGrid, AllSelectValuationMethodAndValue asvv)//, ref ValuationMethodPoolingAndAggregationBase vb)
        {
            AllSelectValuationMethodAndValue asvvnew = new AllSelectValuationMethodAndValue();
            if (gridRelationship == null) return asvv;
            int icount = 0;
            try
            {
                 asvvnew.AllSelectValuationMethod = asvv.AllSelectValuationMethod;
                asvvnew.lstAPVValueAttributes =  new List<APVValueAttribute>();// new List<float[]>();
                string str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", gridRelationship.smallGridID, gridRelationship.bigGridID);
                 if (gridRelationship.smallGridID == benMAPGrid.GridDefinitionID)
                {
                    str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", benMAPGrid.GridDefinitionID, gridRelationship.bigGridID);
                }
                 if (benMAPGrid.GridDefinitionID == 28 || gridRelationship.smallGridID == 28 || benMAPGrid.GridDefinitionID == 27 || gridRelationship.smallGridID == 27)
                  {
                      str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", 27, gridRelationship.bigGridID);
                  }
                //--------如果有的话则可以用这个----------------
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
                                    //CRCalculateValue anew = new CRCalculateValue();
                                    if (dicAggregation.ContainsKey(k.Key))
                                    {
                                        APVValueAttribute apv = dicAggregation[k.Key];
                                        apv.PointEstimate += ava.PointEstimate * Convert.ToSingle(k.Value) / Convert.ToSingle(d);
                                        if (ava.LstPercentile != null)
                                        {
                                            int iAggregation = 0;
                                            foreach (float dLstPercentile in ava.LstPercentile)
                                            {
                                                //asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile.Add(dLstPercentile * Convert.ToSingle(k.Value) / Convert.ToSingle(d));
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
                                        dicAggregation.Add(k.Key,asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1]);
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
                        anewfirst.LstPercentile = new List<float>();// asvv.lstAPVValueAttributes.First().LstPercentile;
                        if (asvv.lstAPVValueAttributes.First().LstPercentile != null)
                        {
                            for (int iPercentile = 0; iPercentile < asvv.lstAPVValueAttributes.First().LstPercentile.Count; iPercentile++)
                            {
                                anewfirst.LstPercentile.Add(0);//[iPercentile] = 0;
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
                                    //anew.Mean += CRCalculateValue.Mean * Convert.ToSingle(k.Value);
                                    //anew.Variance += CRCalculateValue.Variance * Convert.ToSingle(k.Value);
                                    //anew.StandardDeviation += CRCalculateValue.StandardDeviation * Convert.ToSingle(k.Value);

                                    //anew.PercentOfBaseline = (anew.PercentOfBaseline + CRCalculateValue.PercentOfBaseline * Convert.ToSingle(k.Value)) / 2;
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

                    //asvv.ResultCopy = null;
                    if (gridRelationship.bigGridID == benMAPGrid.GridDefinitionID)
                    {
                        foreach (APVValueAttribute ava in asvv.lstAPVValueAttributes)
                        {
                            //-------/count
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
                                        //LstPercentile=ava.LstPercentile==null?null:ava.LstPercentile.Aggregate(
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
                        // vb.LstAllSelectValuationMethodAndValueAggregation.Add(asvvnew);
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
                                //RowCol rr = new RowCol() { Col = 103, Row = 107 };

                            }
                        }
                        Dictionary<string, APVValueAttribute> dicAPVValueAttribute = new Dictionary<string, APVValueAttribute>();
                        APVValueAttribute anewfirst = new APVValueAttribute();
                        anewfirst.LstPercentile = new List<float>();// asvv.lstAPVValueAttributes.First().LstPercentile;
                        if (asvv.lstAPVValueAttributes.First().LstPercentile != null)
                        {
                            for (int iPercentile = 0; iPercentile < asvv.lstAPVValueAttributes.First().LstPercentile.Count; iPercentile++)
                            {
                                anewfirst.LstPercentile.Add(0);//[iPercentile] = 0;
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
                            //anew.LstPercentile = anewfirst.LstPercentile;
                            anew.Col = gra.bigGridRowCol.Col;
                            anew.Row = gra.bigGridRowCol.Row;
                            //asvvnew.lstAPVValueAttributes.Add(new APVValueAttribute()
                            //{
                            //    Col = gra.bigGridRowCol.Col,
                            //    Row = gra.bigGridRowCol.Row,
                            //}

                            //);
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
                                    //asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile = new List<double>();
                                    //for (int iPercentile = 0; iPercentile < iAPVValueAttribute.First().LstPercentile.Count(); iPercentile++)
                                    //{
                                    //   apvValueAttribute.LstPercentile.Add(
                                    //        iAPVValueAttribute.Average(p => p.LstPercentile[iPercentile]));

                                    //}
                                }
                            }

                        }
                        asvvnew.lstAPVValueAttributes = dicAPVValueAttribute.Values.ToList();

                        //foreach (GridRelationshipAttribute gra in gridRelationship.lstGridRelationshipAttribute)
                        //{

                        //    asvvnew.lstAPVValueAttributes.Add(new APVValueAttribute()
                        //    {
                        //        Col = gra.bigGridRowCol.Col,
                        //        Row = gra.bigGridRowCol.Row,
                        //    }

                        //    );
                        //    IEnumerable<APVValueAttribute> iAPVValueAttribute = asvv.lstAPVValueAttributes.Where(p => gra.smallGridRowCol.Contains(new RowCol() { Col = p.Col, Row = p.Row }, rowColComparer));
                        //    if (iAPVValueAttribute.Count() > 0)
                        //    {
                        //        foreach (APVValueAttribute apvx in iAPVValueAttribute)
                        //        {
                        //            asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].Mean += apvx.Mean;
                        //            asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].PointEstimate += apvx.PointEstimate;
                        //            asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].StandardDeviation += apvx.StandardDeviation;
                        //            asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].Variance += apvx.Variance;
                        //            if (apvx.LstPercentile != null)
                        //            {
                        //                if (asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile == null)
                        //                    asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile = apvx.LstPercentile;
                        //                else
                        //                {
                        //                    for (int iPercentile = 0; iPercentile < apvx.LstPercentile.Count(); iPercentile++)
                        //                    {
                        //                        asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile[iPercentile] += apvx.LstPercentile[iPercentile];
                        //                    }
                        //                }
                        //                //asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile = new List<double>();
                        //                //for (int iPercentile = 0; iPercentile < iAPVValueAttribute.First().LstPercentile.Count(); iPercentile++)
                        //                //{
                        //                //    asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile.Add(
                        //                //        iAPVValueAttribute.Average(p => p.LstPercentile[iPercentile]));

                        //                //}
                        //            }
                        //        }
                        //        //var query= from a in iAPVValueAttribute select new {Mean= iAPVValueAttribute.Sum(}
                        //        //asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].Mean = iAPVValueAttribute.Average(p => p.Mean);
                        //        //asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].PointEstimate = iAPVValueAttribute.Average(p => p.PointEstimate);
                        //        //asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].StandardDeviation = iAPVValueAttribute.Average(p => p.StandardDeviation);
                        //        //asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].Variance = iAPVValueAttribute.Average(p => p.Variance);



                        //    }
                        //    else
                        //    {
                        //        //asvvnew = null;

                        //    }
                        //}
                    }
                }
                //vb.LstAllSelectValuationMethodAndValue.Add(asvvnew);
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
                asvvnew.lstQALYValueAttributes = new List<QALYValueAttribute>();// new List<float[]>();
                string str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", gridRelationship.smallGridID, gridRelationship.bigGridID);
                 if (gridRelationship.smallGridID == benMAPGrid.GridDefinitionID)
                {
                    str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", benMAPGrid.GridDefinitionID, gridRelationship.bigGridID);
                }
                 if (benMAPGrid.GridDefinitionID == 28 || gridRelationship.smallGridID == 28 || benMAPGrid.GridDefinitionID == 27 || gridRelationship.smallGridID == 27)
                  {
                      str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", 27, gridRelationship.bigGridID);
                  }
                //--------如果有的话则可以用这个----------------
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
                                    //CRCalculateValue anew = new CRCalculateValue();
                                    if (dicAggregation.ContainsKey(k.Key))
                                    {
                                        QALYValueAttribute qaly = dicAggregation[k.Key];
                                        qaly.PointEstimate += ava.PointEstimate * Convert.ToSingle(k.Value) / Convert.ToSingle(d);
                                        if (ava.LstPercentile != null)
                                        {
                                            int iAggregation = 0;
                                            foreach (float dLstPercentile in ava.LstPercentile)
                                            {
                                                qaly.LstPercentile[iAggregation]+=dLstPercentile * Convert.ToSingle(k.Value) / Convert.ToSingle(d);
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
                        anewfirst.LstPercentile = new List<float>();// asvv.lstQALYValueAttributes.First().LstPercentile;
                        if (asvv.lstQALYValueAttributes.First().LstPercentile != null)
                        {
                            for (int iPercentile = 0; iPercentile < asvv.lstQALYValueAttributes.First().LstPercentile.Count; iPercentile++)
                            {
                                anewfirst.LstPercentile.Add(0);//[iPercentile] = 0;
                            }
                        }
                        Dictionary<string, QALYValueAttribute> dicQALYValueFrom = new Dictionary<string, QALYValueAttribute>();
                        foreach (QALYValueAttribute ava in asvv.lstQALYValueAttributes)
                        {
                            if(!dicQALYValueFrom.ContainsKey(ava.Col + "," + ava.Row))
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
                                    //anew.Mean += CRCalculateValue.Mean * Convert.ToSingle(k.Value);
                                    //anew.Variance += CRCalculateValue.Variance * Convert.ToSingle(k.Value);
                                    //anew.StandardDeviation += CRCalculateValue.StandardDeviation * Convert.ToSingle(k.Value);

                                    //anew.PercentOfBaseline = (anew.PercentOfBaseline + CRCalculateValue.PercentOfBaseline * Convert.ToSingle(k.Value)) / 2;
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
                    asvvnew.lstQALYValueAttributes = new List<QALYValueAttribute>();// new List<float[]>();

                    //asvv.ResultCopy = null;
                    if (gridRelationship.bigGridID == benMAPGrid.GridDefinitionID)
                    {
                        foreach (QALYValueAttribute ava in asvv.lstQALYValueAttributes)
                        {
                            //-------/count
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
                        //vb.lstAllSelectQALYMethodAndValueAggregation.Add(asvvnew);
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
                                //RowCol rr = new RowCol() { Col = 103, Row = 107 };

                            }
                        }
                        Dictionary<string, QALYValueAttribute> dicQALYValueAttribute = new Dictionary<string, QALYValueAttribute>();

                        foreach (GridRelationshipAttribute gra in gridRelationship.lstGridRelationshipAttribute)
                        {
                            //float[] anew = new float[asvv.arrayQALYValueAttributes.First().Length];
                            QALYValueAttribute anew = new QALYValueAttribute();
                            //anew.LstPercentile = anewfirst.LstPercentile;
                            anew.Col = gra.bigGridRowCol.Col;
                            anew.Row = gra.bigGridRowCol.Row;
                            //asvvnew.lstQALYValueAttributes.Add(new QALYValueAttribute()
                            //{
                            //    Col = gra.bigGridRowCol.Col,
                            //    Row = gra.bigGridRowCol.Row,
                            //}

                            //);
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
                                    if (QALYValueAttribute.LstPercentile == null)//----------------------------------------------------------------------
                                        QALYValueAttribute.LstPercentile = ava.LstPercentile;
                                    else
                                    {
                                        //QALYValueAttribute.LstPercentile = new List<float>();
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
                //vb.lstAllSelectQALYMethodAndValueAggregation.Add(asvvnew);
            
 
            }
            catch
            { }
            return asvvnew;
        }
        public static CRSelectFunctionCalculateValue ApplyCRSelectFunctionCalculateValueAggregation(GridRelationship gridRelationship, BenMAPGrid benMAPGrid, CRSelectFunctionCalculateValue cRSelectFunctionCalculateValue)
        {
            //return ApplyAggregationCRSelectFunctionCalculateValue(
            //return ApplyAggregationCRSelectFunctionCalculateValue
            CRSelectFunctionCalculateValue asvvnew = new CRSelectFunctionCalculateValue();
            int icount=0;
            try
            {
                asvvnew.CRCalculateValues = new List<CRCalculateValue>();

                //asvvnew.ResultCopy = null;
                string str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", benMAPGrid.GridDefinitionID, gridRelationship.smallGridID);
                if (gridRelationship.smallGridID == benMAPGrid.GridDefinitionID)
                {
                    str = string.Format("select sourcecolumn, sourcerow, targetcolumn, targetrow,Percentage from GridDefinitionPercentages a,GridDefinitionPercentageEntries b where a.PercentageID=b.PercentageID and a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1} and normalizationstate in (0,1)", benMAPGrid.GridDefinitionID, gridRelationship.bigGridID);
                }
                //--------如果有的话则可以用这个----------------
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
                                        //LstPercentile=ava.LstPercentile==null?null:ava.LstPercentile.Aggregate(
                                    }
                                     );
                                    if (ava.LstPercentile != null)
                                    {
                                        asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].LstPercentile = new List<float>();
                                        foreach (float dLstPercentile in ava.LstPercentile)
                                        {
                                            asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].LstPercentile.Add(dLstPercentile*Convert.ToSingle(k.Value) / Convert.ToSingle(d));
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
                        anewfirst.LstPercentile = new List<float>();// cRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile;
                        if (cRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile != null)
                        {
                            for (int iPercentile = 0; iPercentile < cRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count; iPercentile++)
                            {
                                anewfirst.LstPercentile.Add(0);//[iPercentile] = 0;
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
                            anew.Col =Convert.ToInt32( gra.Key.Split(new char[]{','}).ToArray()[0]);
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
                            //-------/count
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
                                         PercentOfBaseline=ava.PercentOfBaseline,
                                        Population = ava.Population / Convert.ToSingle(icount),
                                        Mean = ava.Mean / Convert.ToSingle(icount),
                                        StandardDeviation = ava.StandardDeviation / Convert.ToSingle(icount),
                                        Variance = ava.Variance / Convert.ToSingle(icount),
                                        //LstPercentile=ava.LstPercentile==null?null:ava.LstPercentile.Aggregate(
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

                        //vb.IncidencePoolingResultAggregation = asvvnew;

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
                                //RowCol rr = new RowCol() { Col = 103, Row = 107 };

                            }
                        }
                        Dictionary<string, CRCalculateValue> dicCRCalculateValue = new Dictionary<string, CRCalculateValue>();
                        CRCalculateValue anewfirst = new CRCalculateValue();
                        anewfirst.LstPercentile = new List<float>();// cRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile;
                        if (cRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile != null)
                        {
                            for (int iPercentile = 0; iPercentile < cRSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count; iPercentile++)
                            {
                                anewfirst.LstPercentile.Add(0);//[iPercentile] = 0;
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
                            //anew.LstPercentile = anewfirst.LstPercentile;
                            anew.Col = gra.bigGridRowCol.Col;
                            anew.Row = gra.bigGridRowCol.Row;
                            //asvvnew.lstCRCalculateValues.Add(new CRCalculateValue()
                            //{
                            //    Col = gra.bigGridRowCol.Col,
                            //    Row = gra.bigGridRowCol.Row,
                            //}

                            //);
                            dicCRCalculateValue.Add(gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row, anew);
                        }
                        foreach (CRCalculateValue ava in cRSelectFunctionCalculateValue.CRCalculateValues)
                        {
                            if (dicRowCol.Keys.Contains(ava.Col + "," + ava.Row))
                            {
                                CRCalculateValue CRCalculateValue = dicCRCalculateValue[dicRowCol[ava.Col + "," + ava.Row]];
                                CRCalculateValue.Baseline += ava.Baseline;
                                CRCalculateValue.Delta += ava.Delta;
                                CRCalculateValue.Incidence =Convert.ToSingle( (CRCalculateValue.Incidence + ava.Incidence)/2.0000);
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
                                    //asvvnew.lstCRCalculateValues[asvvnew.lstCRCalculateValues.Count - 1].LstPercentile = new List<double>();
                                    //for (int iPercentile = 0; iPercentile < iCRCalculateValue.First().LstPercentile.Count(); iPercentile++)
                                    //{
                                    //   CRCalculateValue.LstPercentile.Add(
                                    //        iCRCalculateValue.Average(p => p.LstPercentile[iPercentile]));

                                    //}
                                }
                            }

                        }
                        asvvnew.CRCalculateValues = dicCRCalculateValue.Values.ToList();
                        //foreach (GridRelationshipAttribute gra in gridRelationship.lstGridRelationshipAttribute)
                        //{

                        //    asvvnew.CRCalculateValues.Add(new CRCalculateValue()
                        //    {
                        //        Col = gra.bigGridRowCol.Col,
                        //        Row = gra.bigGridRowCol.Row,
                        //    }

                        //    );
                        //    IEnumerable<CRCalculateValue> iCRCalculateValue = cRSelectFunctionCalculateValue.CRCalculateValues.Where(p => gra.smallGridRowCol.Contains(new RowCol() { Col = p.Col, Row = p.Row }, new RowColComparer()));
                        //    if (iCRCalculateValue.Count() > 0)
                        //    {
                        //        foreach (CRCalculateValue crv in iCRCalculateValue)
                        //        {
                        //            asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].Mean += crv.Mean;
                        //            asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].PointEstimate += crv.PointEstimate;
                        //            asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].StandardDeviation += crv.StandardDeviation;
                        //            asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].Variance += crv.Variance;
                        //            if (asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].LstPercentile == null && crv.LstPercentile != null)
                        //            {
                        //                asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].LstPercentile = crv.LstPercentile;
                        //            }
                        //            else if (asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].LstPercentile != null && crv.LstPercentile != null)
                        //            {
                        //                for (int iPercentile = 0; iPercentile < crv.LstPercentile.Count(); iPercentile++)
                        //                {
                        //                    asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].LstPercentile[iPercentile] += crv.LstPercentile[iPercentile];
                        //                }
                        //            }


                        //        }


                        //    }
                        //    else
                        //    {
                        //        //asvvnew = null;

                        //    }
                        //}
                    }
                }
                
                //vb.IncidencePoolingResultAggregation=asvvnew;

            }
            catch
            { }
            return asvvnew;
        }
        /// <summary>
        /// Aggre
        /// </summary>
        /// <param name="gridRelationship"></param>
        /// <param name="gBenMAPGrid"></param>
        /// <param name="valuationMethodPoolingAndAggregation"></param>
        //public static void ApplyAggregationFromValuationMethodPoolingAndAggregation(List<GridRelationship> lstGridRelationshipAll, BenMAPGrid gBenMAPGrid, ref ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation)
        //{
        //    try
        //    {
        //        int icount = 0;
        //        double d = 0;
        //        int idAggregation = -1;
        //        GridRelationship gridRelationship = null;
        //        GridRelationship gridRelationshipIncidence = null;
        //        if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
        //               valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null &&
        //               valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != gBenMAPGrid.GridDefinitionID && valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase != null)
        //        {
        //            //----修正所有的Valuation
        //            idAggregation = valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID;

        //            if (lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).Count() > 0)
        //            {
        //                gridRelationship = lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).First();
        //            }
        //            else
        //            {
        //                gridRelationship = lstGridRelationshipAll.Where(p => p.bigGridID == idAggregation && p.smallGridID == gBenMAPGrid.GridDefinitionID).First();

        //            }

                   

        //            foreach (ValuationMethodPoolingAndAggregationBase vb in valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
        //            {
        //                if (vb.LstAllSelectValuationMethodAndValue != null)
        //                {
        //                    vb.LstAllSelectValuationMethodAndValueAggregation = new List<AllSelectValuationMethodAndValue>();

        //                    foreach (AllSelectValuationMethodAndValue asvv in vb.LstAllSelectValuationMethodAndValue)
        //                    {
        //                        //agregation
        //                        AllSelectValuationMethodAndValue asvvnew = new AllSelectValuationMethodAndValue();
        //                        asvvnew.AllSelectValuationMethod = asvv.AllSelectValuationMethod;
        //                        asvvnew.lstAPVValueAttributes = new List<APVValueAttribute>();

        //                        asvv.ResultCopy = null;
        //                        if (gridRelationship.smallGridID == valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID)
        //                        {
        //                            foreach (APVValueAttribute ava in asvv.lstAPVValueAttributes)
        //                            {
        //                                //-------/count
        //                                icount = gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).Count();
        //                                if (icount > 0)
        //                                {
        //                                    icount = gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).First().smallGridRowCol.Count();
        //                                    foreach (RowCol rc in gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).First().smallGridRowCol)
        //                                    {
        //                                        asvvnew.lstAPVValueAttributes.Add(new APVValueAttribute()
        //                                        {
        //                                            Row = rc.Row,
        //                                            Col = rc.Col,
        //                                            PointEstimate = ava.PointEstimate / icount,
        //                                            Mean = ava.Mean / icount,
        //                                            StandardDeviation = ava.StandardDeviation / icount,
        //                                            Variance = ava.Variance / icount,
        //                                            //LstPercentile=ava.LstPercentile==null?null:ava.LstPercentile.Aggregate(
        //                                        }
        //                                        );
        //                                        if (ava.LstPercentile != null)
        //                                        {
        //                                            asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile = new List<double>();
        //                                            foreach (double dLstPercentile in ava.LstPercentile)
        //                                            {
        //                                                asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile.Add(dLstPercentile / icount);
        //                                            }
        //                                        }
        //                                    }
        //                                }



        //                            }
        //                            vb.LstAllSelectValuationMethodAndValueAggregation.Add(asvvnew);
        //                        }
        //                        else
        //                        {

        //                            foreach (GridRelationshipAttribute gra in gridRelationship.lstGridRelationshipAttribute)
        //                            {

        //                                asvvnew.lstAPVValueAttributes.Add(new APVValueAttribute()
        //                                {
        //                                    Col = gra.bigGridRowCol.Col,
        //                                    Row = gra.bigGridRowCol.Row,
        //                                }

        //                                );
        //                                IEnumerable<APVValueAttribute> iAPVValueAttribute = asvv.lstAPVValueAttributes.Where(p => gra.smallGridRowCol.Contains(new RowCol() { Col = p.Col, Row = p.Row }, new RowColComparer()));
        //                                if (iAPVValueAttribute.Count() > 0  )
        //                                {
        //                                    foreach (APVValueAttribute apvx in iAPVValueAttribute)
        //                                    {
        //                                        asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].Mean += apvx.Mean;
        //                                        asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].PointEstimate += apvx.PointEstimate;
        //                                        asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].StandardDeviation += apvx.StandardDeviation;
        //                                        asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].Variance += apvx.Variance;
        //                                        if (apvx.LstPercentile != null)
        //                                        {
        //                                            if (asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile == null)
        //                                                asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile = apvx.LstPercentile;
        //                                            else
        //                                            {
        //                                                for (int iPercentile = 0; iPercentile < apvx.LstPercentile.Count(); iPercentile++)
        //                                                {
        //                                                    asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile[iPercentile] += apvx.LstPercentile[iPercentile];
        //                                                }
        //                                            }
        //                                            //asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile = new List<double>();
        //                                            //for (int iPercentile = 0; iPercentile < iAPVValueAttribute.First().LstPercentile.Count(); iPercentile++)
        //                                            //{
        //                                            //    asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].LstPercentile.Add(
        //                                            //        iAPVValueAttribute.Average(p => p.LstPercentile[iPercentile]));

        //                                            //}
        //                                        }
        //                                    }
        //                                    //var query= from a in iAPVValueAttribute select new {Mean= iAPVValueAttribute.Sum(}
        //                                    //asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].Mean = iAPVValueAttribute.Average(p => p.Mean);
        //                                    //asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].PointEstimate = iAPVValueAttribute.Average(p => p.PointEstimate);
        //                                    //asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].StandardDeviation = iAPVValueAttribute.Average(p => p.StandardDeviation);
        //                                    //asvvnew.lstAPVValueAttributes[asvvnew.lstAPVValueAttributes.Count - 1].Variance = iAPVValueAttribute.Average(p => p.Variance);

                                            

        //                                }
        //                                else
        //                                {
        //                                    //asvvnew = null;

        //                                }


        //                            }
        //                            if (asvvnew != null)
        //                            {
        //                                vb.LstAllSelectValuationMethodAndValueAggregation.Add(asvvnew);
        //                            }
        //                        }
        //                    }

        //                }
        //                if (vb.IncidencePoolingResult != null)
        //                {
        //                    CRSelectFunctionCalculateValue asvvnew = new CRSelectFunctionCalculateValue();
        //                    //asvvnew.AllSelectValuationMethod = vb.IncidencePoolingResult.AllSelectValuationMethod;
        //                    asvvnew.CRCalculateValues = new List<CRCalculateValue>();

        //                    asvvnew.ResultCopy = null;
        //                    if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID != -1 && valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID != gBenMAPGrid.GridDefinitionID)
        //                    {
        //                        idAggregation = valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID;
        //                        if (lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).Count() > 0)
        //                        {
        //                            gridRelationshipIncidence = lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).First();
        //                        }
        //                        else
        //                        {
        //                            gridRelationshipIncidence = lstGridRelationshipAll.Where(p => p.bigGridID == idAggregation && p.smallGridID == gBenMAPGrid.GridDefinitionID).First();

        //                        }
        //                        gridRelationship = gridRelationshipIncidence;

        //                        if (gridRelationship.smallGridID == valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID)
        //                        {
        //                            foreach (CRCalculateValue ava in vb.IncidencePoolingResult.CRCalculateValues)
        //                            {
        //                                //-------/count
        //                                icount = gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).Count();
        //                                if (icount > 0)
        //                                {
        //                                    icount = gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).First().smallGridRowCol.Count();
        //                                    foreach (RowCol rc in gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).First().smallGridRowCol)
        //                                    {
        //                                        asvvnew.CRCalculateValues.Add(new CRCalculateValue()
        //                                        {
        //                                            Row = rc.Row,
        //                                            Col = rc.Col,
        //                                            PointEstimate = ava.PointEstimate / icount,
        //                                            Mean = ava.Mean / icount,
        //                                            StandardDeviation = ava.StandardDeviation / icount,
        //                                            Variance = ava.Variance / icount,
        //                                            //LstPercentile=ava.LstPercentile==null?null:ava.LstPercentile.Aggregate(
        //                                        }
        //                                        );
        //                                        if (ava.LstPercentile != null)
        //                                        {
        //                                            asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].LstPercentile = new List<double>();
        //                                            foreach (double dLstPercentile in ava.LstPercentile)
        //                                            {
        //                                                asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].LstPercentile.Add(dLstPercentile / icount);
        //                                            }
        //                                        }
        //                                    }
        //                                }



        //                            }
        //                            vb.IncidencePoolingResultAggregation = asvvnew;

        //                        }
        //                        else
        //                        {

        //                            foreach (GridRelationshipAttribute gra in gridRelationship.lstGridRelationshipAttribute)
        //                            {

        //                                asvvnew.CRCalculateValues.Add(new CRCalculateValue()
        //                                {
        //                                    Col = gra.bigGridRowCol.Col,
        //                                    Row = gra.bigGridRowCol.Row,
        //                                }

        //                                );
        //                                IEnumerable<CRCalculateValue> iCRCalculateValue = vb.IncidencePoolingResult.CRCalculateValues.Where(p => gra.smallGridRowCol.Contains(new RowCol() { Col = p.Col, Row = p.Row }, new RowColComparer()));
        //                                if (iCRCalculateValue.Count() > 0)
        //                                {
        //                                    foreach (CRCalculateValue crv in iCRCalculateValue)
        //                                    {
        //                                        asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].Mean += crv.Mean;
        //                                        asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].PointEstimate += crv.PointEstimate;
        //                                        asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].StandardDeviation += crv.StandardDeviation;
        //                                        asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].Variance += crv.Variance;
        //                                        if (asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].LstPercentile == null && crv.LstPercentile != null)
        //                                        {
        //                                            asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].LstPercentile = crv.LstPercentile;
        //                                        }
        //                                        else if (asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].LstPercentile != null && crv.LstPercentile != null)
        //                                        {
        //                                            for (int iPercentile = 0; iPercentile < crv.LstPercentile.Count(); iPercentile++)
        //                                            {
        //                                                asvvnew.CRCalculateValues[asvvnew.CRCalculateValues.Count - 1].LstPercentile[iPercentile] += crv.LstPercentile[iPercentile];
        //                                            }
        //                                        }


        //                                    }
                                           

        //                                }
        //                                else
        //                                {
        //                                    //asvvnew = null;

        //                                }


        //                            }
        //                            if (asvvnew != null)
        //                            {
        //                                vb.IncidencePoolingResultAggregation = asvvnew;
        //                            }


        //                        }
        //                    }
        //                }

        //            }
        //        }
        //        if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
        //                valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation != null &&
        //                valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID != gBenMAPGrid.GridDefinitionID && valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase != null)
        //        {
        //            //----修正所有的QALY
        //            idAggregation = valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID;
        //            if (lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).Count() > 0)
        //            {
        //                gridRelationship = lstGridRelationshipAll.Where(p => p.bigGridID == gBenMAPGrid.GridDefinitionID && p.smallGridID == idAggregation).First();
        //            }
        //            else
        //            {
        //                gridRelationship = lstGridRelationshipAll.Where(p => p.bigGridID == idAggregation && p.smallGridID == gBenMAPGrid.GridDefinitionID).First();

        //            }
        //            foreach (ValuationMethodPoolingAndAggregationBase vb in valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
        //            {
        //                if (vb.lstAllSelectQALYMethodAndValue != null)
        //                {
        //                    vb.lstAllSelectQALYMethodAndValueAggregation = new List<AllSelectQALYMethodAndValue>();

        //                    foreach (AllSelectQALYMethodAndValue asvv in vb.lstAllSelectQALYMethodAndValue)
        //                    {
        //                        //agregation
        //                        AllSelectQALYMethodAndValue asvvnew = new AllSelectQALYMethodAndValue();
        //                        asvvnew.AllSelectQALYMethod = asvv.AllSelectQALYMethod;
        //                        asvvnew.lstQALYValueAttributes = new List<QALYValueAttribute>();

        //                        asvv.ResultCopy = null;
        //                        if (gridRelationship.smallGridID == valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID)
        //                        {
        //                            foreach (QALYValueAttribute ava in asvv.lstQALYValueAttributes)
        //                            {
        //                                //-------/count
        //                                icount = gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).Count();
        //                                if (icount > 0)
        //                                {
        //                                    icount = gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).First().smallGridRowCol.Count();
        //                                    foreach (RowCol rc in gridRelationship.lstGridRelationshipAttribute.Where(p => p.bigGridRowCol.Col == ava.Col && p.bigGridRowCol.Row == ava.Row).First().smallGridRowCol)
        //                                    {
        //                                        asvvnew.lstQALYValueAttributes.Add(new QALYValueAttribute()
        //                                        {
        //                                            Row = rc.Row,
        //                                            Col = rc.Col,
        //                                            PointEstimate = ava.PointEstimate / icount,
        //                                            Mean = ava.Mean / icount,
        //                                            StandardDeviation = ava.StandardDeviation / icount,
        //                                            Variance = ava.Variance / icount,
        //                                            //LstPercentile=ava.LstPercentile==null?null:ava.LstPercentile.Aggregate(
        //                                        }
        //                                        );
        //                                        if (ava.LstPercentile != null)
        //                                        {
        //                                            asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].LstPercentile = new List<double>();
        //                                            foreach (double dLstPercentile in ava.LstPercentile)
        //                                            {
        //                                                asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].LstPercentile.Add(dLstPercentile / icount);
        //                                            }
        //                                        }
        //                                    }
        //                                }



        //                            }
        //                            vb.lstAllSelectQALYMethodAndValueAggregation.Add(asvvnew);
        //                        }
        //                        else
        //                        {
        //                            if (asvvnew == null)
        //                            {
        //                                asvvnew = new AllSelectQALYMethodAndValue();
        //                                asvvnew.lstQALYValueAttributes = new List<QALYValueAttribute>();
        //                                asvvnew.AllSelectQALYMethod = asvv.AllSelectQALYMethod;
        //                            }
        //                            foreach (GridRelationshipAttribute gra in gridRelationship.lstGridRelationshipAttribute)
        //                            {

        //                                asvvnew.lstQALYValueAttributes.Add(new QALYValueAttribute()
        //                                {
        //                                    Col = gra.bigGridRowCol.Col,
        //                                    Row = gra.bigGridRowCol.Row,
        //                                }

        //                                );
        //                                IEnumerable<QALYValueAttribute> iQALYValueAttribute = asvv.lstQALYValueAttributes.Where(p => gra.smallGridRowCol.Contains(new RowCol() { Col = p.Col, Row = p.Row }, new RowColComparer()));
        //                                foreach (QALYValueAttribute qlya in iQALYValueAttribute)
        //                                {
        //                                    asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].Mean += qlya.Mean;
        //                                    asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].PointEstimate += qlya.PointEstimate;
        //                                    asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].StandardDeviation += qlya.StandardDeviation;
        //                                    asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].Variance += qlya.Variance;
        //                                    if (asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].LstPercentile == null && qlya.LstPercentile != null)
        //                                    {
        //                                        asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].LstPercentile = qlya.LstPercentile;
        //                                    }
        //                                    else if (asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].LstPercentile != null && qlya.LstPercentile != null)
        //                                    {
        //                                        for (int iPercentile = 0; iPercentile < qlya.LstPercentile.Count(); iPercentile++)
        //                                        {
        //                                            asvvnew.lstQALYValueAttributes[asvvnew.lstQALYValueAttributes.Count - 1].LstPercentile[iPercentile] += qlya.LstPercentile[iPercentile];
        //                                        }
        //                                    }

 
        //                                }
                                         


        //                            }
        //                            if (asvvnew != null)
        //                            {
        //                                vb.lstAllSelectQALYMethodAndValueAggregation.Add(asvvnew);
        //                            }

        //                        }
        //                    }

        //                }
        //            }
        //        }

        //    }
        //    catch
        //    { }
        //}

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
                    //----修正所有的Valuation
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
                                //agregation
                                //AllSelectValuationMethodAndValue asvvnew = new AllSelectValuationMethodAndValue();
                                //asvvnew.AllSelectValuationMethod = asvv.AllSelectValuationMethod;
                                //asvvnew.lstAPVValueAttributes = new List<APVValueAttribute>();

                                //asvv.ResultCopy = null;
                                vb.LstAllSelectValuationMethodAndValueAggregation.Add(APVX.APVCommonClass.ApplyAllSelectValuationMethodAndValueAggregation(gridRelationship, CommonClass.GBenMAPGrid, asvv));
                                //lstAsyns.Add("a");
                                //AsyncValuation dlgt = new AsyncValuation(APVX.APVCommonClass.ApplyAllSelectValuationMethodAndValueAggregation);
                                //dlgt.BeginInvoke(gridRelationship, CommonClass.GBenMAPGrid, asvv, ref vb, new AsyncCallback(outPut),dlgt);
                                //if (asvvnew != null)
                                //{
                                //    vb.LstAllSelectValuationMethodAndValueAggregation.Add(asvvnew);
                                //}

                            }

                        }
                        if (vb.IncidencePoolingResult != null &&gridRelationshipIncidence!=null)
                        {
                            vb.IncidencePoolingResultAggregation = APVX.APVCommonClass.ApplyCRSelectFunctionCalculateValueAggregation(gridRelationshipIncidence, CommonClass.GBenMAPGrid, vb.IncidencePoolingResult);
                            //lstAsyns.Add("a");
                            //AsyncIncidence dlgt = new AsyncIncidence(APVX.APVCommonClass.ApplyCRSelectFunctionCalculateValueAggregation);
                            //dlgt.BeginInvoke(gridRelationship, CommonClass.GBenMAPGrid, vb.IncidencePoolingResult, ref vb, new AsyncCallback(outPut), dlgt);
                            //CRSelectFunctionCalculateValue asvvnew = new CRSelectFunctionCalculateValue();
                            ////asvvnew.AllSelectValuationMethod = vb.IncidencePoolingResult.AllSelectValuationMethod;
                            //asvvnew.CRCalculateValues = new List<CRCalculateValue>();

                            //asvvnew.ResultCopy = null;
                            //if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID != -1 && valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncidenceAggregation.GridDefinitionID != gBenMAPGrid.GridDefinitionID)
                            //{
                            //    asvvnew = APVX.APVCommonClass.ApplyCRSelectFunctionCalculateValueAggregation(gridRelationship, CommonClass.GBenMAPGrid, vb.IncidencePoolingResult);


                            //        if (asvvnew != null)
                            //        {
                            //            vb.IncidencePoolingResultAggregation = asvvnew;
                            //        }



                            //}
                        }

                    }
                }
                if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
                        valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation != null &&
                        valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID != gBenMAPGrid.GridDefinitionID && valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase != null)
                {
                    //----修正所有的QALY
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
                    //foreach (ValuationMethodPoolingAndAggregationBase vb in valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                    {
                        ValuationMethodPoolingAndAggregationBase vb = valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase[ivb];
                        if (vb.lstAllSelectQALYMethodAndValue != null)
                        {
                            vb.lstAllSelectQALYMethodAndValueAggregation = new List<AllSelectQALYMethodAndValue>();

                            foreach (AllSelectQALYMethodAndValue asvv in vb.lstAllSelectQALYMethodAndValue)
                            {
                                vb.lstAllSelectQALYMethodAndValueAggregation.Add(APVX.APVCommonClass.ApplyAllSelectQALYMethodAndValueAggregation(gridRelationship, CommonClass.GBenMAPGrid, asvv));
                                //lstAsyns.Add("a");
                                //AsyncQALY dlgt = new AsyncQALY(APVX.APVCommonClass.ApplyAllSelectQALYMethodAndValueAggregation);
                                //dlgt.BeginInvoke(gridRelationship, CommonClass.GBenMAPGrid,asvv, ref vb, new AsyncCallback(outPut), dlgt);
                                //agregation
                                //AllSelectQALYMethodAndValue asvvnew = new AllSelectQALYMethodAndValue();
                                //asvvnew.AllSelectQALYMethod = asvv.AllSelectQALYMethod;
                                //asvvnew.lstQALYValueAttributes = new List<QALYValueAttribute>();

                                //asvvnew = APVX.APVCommonClass.ApplyAllSelectQALYMethodAndValueAggregation(gridRelationship, CommonClass.GBenMAPGrid, asvv);
                                //    if (asvvnew != null)
                                //    {
                                //        vb.lstAllSelectQALYMethodAndValueAggregation.Add(asvvnew);
                                //    }


                            }

                        }
                    }
                }
                //if (lstAsyns.Count == 0)
                //{
                //    WaitClose();
                //    //----------------------Save To File----------------------------
                //    if (_filePath != "")
                //    {
                //        //-------------save result------------------
                //        APVX.APVCommonClass.SaveAPVRFile(_filePath, CommonClass.ValuationMethodPoolingAndAggregation);
                //        MessageBox.Show("Save has been completed!");

                //    }
                //    this.DialogResult = System.Windows.Forms.DialogResult.OK;

                //}

            }
            catch
            { }
        }
        /// <summary>
        /// 获取拉丁立方体采样
        /// </summary>
        /// <param name="LatinHypercubePoints"></param>
        /// <returns></returns>
        //public static double[] getLHSArrayValuationFunction(int LatinHypercubePoints, BenMAPValuationFunction benMAPValuationFunction)
        //{
        //    try
        //    {
        //        Distribution distribution = null;// (Distribution)this.currentDistribution;
        //        double[] lhsResultArray = new double[LatinHypercubePoints];
        //        switch (benMAPValuationFunction.DistA)
        //        {
        //            case "None"://还是Beta
        //                for (int i = 0; i < LatinHypercubePoints; i++)
        //                {
        //                    lhsResultArray[i] = benMAPValuationFunction.A;

        //                }
        //                return lhsResultArray;
        //                break;
        //            case "Normal":
        //                distribution = new NormalDistribution()
        //                {
        //                    Mu = benMAPValuationFunction.A,
        //                    Sigma = benMAPValuationFunction.P1A

        //                };
        //                break;
        //            case "Triangular":
        //                distribution = new TriangularDistribution() { Alpha = benMAPValuationFunction.P1A, Beta = benMAPValuationFunction.P2A, Gamma = benMAPValuationFunction.A };
        //                break;
        //            case "Poisson":
        //                distribution = new PoissonDistribution() { Lambda = benMAPValuationFunction.P1A };
        //                break;
        //            case "Binomial":
        //                distribution = new BinomialDistribution() { Alpha = benMAPValuationFunction.P1A, Beta = Convert.ToInt32(benMAPValuationFunction.P2A) };
        //                break;
        //            case "LogNormal":
        //                distribution = new LognormalDistribution() { Mu = benMAPValuationFunction.P1A, Sigma = benMAPValuationFunction.P2A };
        //                break;
        //            case "Uniform":
        //                distribution = new ContinuousUniformDistribution() { Alpha = benMAPValuationFunction.P1A, Beta = benMAPValuationFunction.P2A };
        //                break;
        //            case "Exponential":
        //                distribution = new ExponentialDistribution() { Lambda = benMAPValuationFunction.P1A };
        //                break;
        //            case "Geometric":
        //                distribution = new GeometricDistribution() { Alpha = benMAPValuationFunction.P1A };
        //                break;
        //            case "Weibull":
        //                distribution = new WeibullDistribution() { Alpha = benMAPValuationFunction.P2A, Lambda = benMAPValuationFunction.P1A };
        //                break;
        //            case "Gamma":
        //                distribution = new GammaDistribution() { Alpha = benMAPValuationFunction.P1A, Theta = benMAPValuationFunction.P2A };
        //                break;
        //            case "Logistic":
        //                //distribution=new  Troschuetz.Random.
        //                Meta.Numerics.Statistics.Distributions.Distribution logistic_distribution = new Meta.Numerics.Statistics.Distributions.LogisticDistribution(benMAPValuationFunction.P1A, benMAPValuationFunction.P2A);
        //               Meta.Numerics.Statistics.Sample logistic_sample = CreateSample(logistic_distribution, 1000000);
        //               List<double> lstlogistic = logistic_sample.ToList();
        //               lstlogistic.Sort();

        //        for (int i = 0; i < LatinHypercubePoints; i++)
        //        {
        //            lhsResultArray[i] = lstlogistic.GetRange(i * (lstlogistic.Count / LatinHypercubePoints), (lstlogistic.Count / LatinHypercubePoints)).Median();
        //            //lhsResultArray[LatinHypercubePoints - i-1] = lstsamples[(LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints) > lstsamples.Count-1 ? lstsamples.Count-1 : (LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints)];
        //            //i++;
        //        }
        //        return lhsResultArray;
        //                break;
        //            case "Beta":
        //                distribution = new BetaDistribution() { Alpha = benMAPValuationFunction.P1A, Beta = benMAPValuationFunction.P2A };
        //                break;
        //            case "Pareto":
        //                distribution = new ParetoDistribution() { Alpha = benMAPValuationFunction.P1A, Beta = benMAPValuationFunction.P2A };
        //                break;
        //            case "Cauchy":
        //                distribution = new CauchyDistribution() { Alpha = benMAPValuationFunction.P1A, Gamma = benMAPValuationFunction.P2A };
        //                break;
        //            case "Custom":
        //                //distribution=new CustomDistributionEntries(){ 
        //                //------------首先得到Custom的实例---------------然后计算均值-------------------
        //                string commandText = string.Format("select   VValue  from ValuationFunctionCustomEntries where ValuationFunctionID={0} ", benMAPValuationFunction.ID);
        //                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

        //                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
        //                List<float> lstCustom = new List<float>();
        //                foreach (DataRow dr in ds.Tables[0].Rows)
        //                {
        //                    lstCustom.Add(Convert.ToSingle(dr[0]));

        //                }
        //                lstCustom.Sort();
        //                for (int i = 0; i < LatinHypercubePoints; i++)
        //                {
        //                    //switch (benMAPValuationFunction.DiscA)
        //                    //{
        //                    //    case "Uniform":
        //                    //    case "Triangular":
        //                    //        lhsResultArray[i] = lstCustom.GetRange(i * (lstCustom.Count / LatinHypercubePoints), (lstCustom.Count / LatinHypercubePoints)).Average();
        //                    //        break;
        //                    //    default:
        //                    //        lhsResultArray[i] = lstCustom.GetRange(i * (lstCustom.Count / LatinHypercubePoints), (lstCustom.Count / LatinHypercubePoints)).Median();
        //                    //        break;
        //                    //}
        //                    lhsResultArray[i] = lstCustom.GetRange(i * (lstCustom.Count / LatinHypercubePoints), (lstCustom.Count / LatinHypercubePoints)).Median();
        //                    //lhsResultArray[LatinHypercubePoints - i-1] = lstsamples[(LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints) > lstsamples.Count-1 ? lstsamples.Count-1 : (LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints)];
        //                    //i++;
        //                }
        //                return lhsResultArray;
        //                //break;


        //        }

        //        double[] samples = new double[1000000];
        //        Stopwatch watch = new Stopwatch();
        //        watch.Start();
        //        for (int index = 0; index < samples.Length; index++)
        //        {
        //            samples[index] = distribution.NextDouble();
        //        }
        //        watch.Stop();
        //        double duration = (double)watch.ElapsedTicks / (double)Stopwatch.Frequency;

        //        //Determine sum, minimum, maximum and display the last two together with a computed mean value.
        //        double sum = 0, minimum = double.MaxValue, maximum = double.MinValue;
        //        for (int index = 0; index < samples.Length; index++)
        //        {
        //            sum += samples[index];
        //            if (samples[index] > maximum)
        //                maximum = samples[index];
        //            if (samples[index] < minimum)
        //                minimum = samples[index];
        //        }
        //        double mean = sum / samples.Length;
        //        double variance = 0.0;
        //        for (int index = 0; index < samples.Length; index++)
        //        {
        //            variance += Math.Pow(samples[index] - mean, 2);
        //        }
        //        variance /= samples.Length;
        //        List<double> lstsamples = samples.ToList();
        //        lstsamples.Sort();

        //        for (int i = 0; i < LatinHypercubePoints; i++)
        //        {
        //            //switch (benMAPValuationFunction.DiscA)
        //            //        {
        //            //            case "Uniform":
        //            //            case "Triangular":
        //            //                lhsResultArray[i] = lstsamples.GetRange(i * (lstsamples.Count / LatinHypercubePoints), (lstsamples.Count / LatinHypercubePoints)).Average();
        //            //                break;
        //            //            default:
        //            //                lhsResultArray[i] = lstsamples.GetRange(i * (lstsamples.Count / LatinHypercubePoints), (lstsamples.Count / LatinHypercubePoints)).Median();
        //            //                break;
        //            //        }
        //            lhsResultArray[i] = lstsamples.GetRange(i * (lstsamples.Count / LatinHypercubePoints), (lstsamples.Count / LatinHypercubePoints)).Median();
        //            //lhsResultArray[LatinHypercubePoints - i-1] = lstsamples[(LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints) > lstsamples.Count-1 ? lstsamples.Count-1 : (LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints)];
        //            //i++;
        //        }
        //        return lhsResultArray;

        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        /// <summary>
        /// 获取拉丁立方体采样
        /// </summary>
        /// <param name="LatinHypercubePoints"></param>
        /// <returns></returns>
        public static double[] getLHSArrayValuationFunctionSeed(int LatinHypercubePoints, BenMAPValuationFunction benMAPValuationFunction,int Seed)
        {
            try
            {
                //Distribution distribution = null;// (Distribution)this.currentDistribution;
                Meta.Numerics.Statistics.Sample sample = null;
                double[] lhsResultArray = new double[LatinHypercubePoints];
                switch (benMAPValuationFunction.DistA)
                {
                    case "None"://还是Beta
                        for (int i = 0; i < LatinHypercubePoints; i++)
                        {
                            lhsResultArray[i] = benMAPValuationFunction.A;

                        }
                        return lhsResultArray;
                        break;
                    case "Normal":
                        //distribution = new NormalDistribution()
                        //{
                        //    Mu = benMAPValuationFunction.A,
                        //    Sigma = benMAPValuationFunction.P1A

                        //};
                         Meta.Numerics.Statistics.Distributions.Distribution Normal_distribution =
                            new Meta.Numerics.Statistics.Distributions.NormalDistribution(benMAPValuationFunction.A, benMAPValuationFunction.P1A);
                        sample = CreateSample(Normal_distribution, 1000000, Seed);
                        break;
                    case "Triangular":
                        //distribution = new TriangularDistribution() { Alpha = benMAPValuationFunction.P1A, Beta = benMAPValuationFunction.P2A, Gamma = benMAPValuationFunction.A };
                        Meta.Numerics.Statistics.Distributions.TriangularDistribution triangularDistribution =
                            new Meta.Numerics.Statistics.Distributions.TriangularDistribution(benMAPValuationFunction.P1A, benMAPValuationFunction.P2A,benMAPValuationFunction.A);
                        sample = CreateSample(triangularDistribution, 1000000, Seed);
                        break;
                    case "Poisson":
                        //distribution = new PoissonDistribution() { Lambda = benMAPValuationFunction.P1A };
                        Meta.Numerics.Statistics.Distributions.PoissonDistribution poissonDistribution =
                            new Meta.Numerics.Statistics.Distributions.PoissonDistribution(benMAPValuationFunction.P1A);
                        sample = CreateSample(poissonDistribution, 1000000, Seed);
                        break;
                    case "Binomial":
                        //distribution = new BinomialDistribution() { Alpha = benMAPValuationFunction.P1A, Beta = Convert.ToInt32(benMAPValuationFunction.P2A) };
                        Meta.Numerics.Statistics.Distributions.BinomialDistribution binomialDistribution =
                            new Meta.Numerics.Statistics.Distributions.BinomialDistribution(benMAPValuationFunction.P1A, Convert.ToInt32(benMAPValuationFunction.P2A));
                        sample = CreateSample(binomialDistribution, 1000000, Seed);
                        break;
                    case "LogNormal":
                        //distribution = new LognormalDistribution() { Mu = benMAPValuationFunction.P1A, Sigma = benMAPValuationFunction.P2A };
                        Meta.Numerics.Statistics.Distributions.LognormalDistribution lognormalDistribution =
                            new Meta.Numerics.Statistics.Distributions.LognormalDistribution(benMAPValuationFunction.P1A, benMAPValuationFunction.P2A);
                        sample = CreateSample(lognormalDistribution, 1000000, Seed);
                        break;
                    case "Uniform":
                        //distribution = new ContinuousUniformDistribution() { Alpha = benMAPValuationFunction.P1A, Beta = benMAPValuationFunction.P2A };
                        //Meta.Numerics.Statistics.Distributions.UniformDistribution uniformDistribution =
                        //    new Meta.Numerics.Statistics.Distributions.UniformDistribution();//benMAPValuationFunction.P1A, benMAPValuationFunction.P2A);
                        Interval interval = Interval.FromEndpoints(benMAPValuationFunction.P1A,
                            benMAPValuationFunction.P2A);

                        Meta.Numerics.Statistics.Distributions.UniformDistribution uniformDistribution =
                            new Meta.Numerics.Statistics.Distributions.UniformDistribution(interval );
                        sample = CreateSample(uniformDistribution, 1000000, Seed);
                        break;
                    case "Exponential":
                        //distribution = new ExponentialDistribution() { Lambda = benMAPValuationFunction.P1A };
                        Meta.Numerics.Statistics.Distributions.ExponentialDistribution exponentialDistribution =
                            new Meta.Numerics.Statistics.Distributions.ExponentialDistribution(benMAPValuationFunction.P1A);
                        sample = CreateSample(exponentialDistribution, 1000000, Seed);
                        break;
                    case "Geometric":
                        //distribution = new GeometricDistribution() { Alpha = benMAPValuationFunction.P1A };
                        Meta.Numerics.Statistics.Distributions.GeometricDistribution GeometricDistribution =
                            new Meta.Numerics.Statistics.Distributions.GeometricDistribution(benMAPValuationFunction.P1A);
                        sample = CreateSample(GeometricDistribution, 1000000, Seed);
                        break;
                    case "Weibull":
                        //distribution = new WeibullDistribution() { Alpha = benMAPValuationFunction.P2A, Lambda = benMAPValuationFunction.P1A };
                        Meta.Numerics.Statistics.Distributions.WeibullDistribution WeibullDistribution =
                            new Meta.Numerics.Statistics.Distributions.WeibullDistribution(benMAPValuationFunction.P1A, benMAPValuationFunction.P2A);
                        sample = CreateSample(WeibullDistribution, 1000000, Seed);
                        break;
                    case "Gamma":
                        //distribution = new GammaDistribution() { Alpha = benMAPValuationFunction.P1A, Theta = benMAPValuationFunction.P2A };
                        Meta.Numerics.Statistics.Distributions.GammaDistribution GammaDistribution =
                            new Meta.Numerics.Statistics.Distributions.GammaDistribution(benMAPValuationFunction.P1A, benMAPValuationFunction.P2A);
                        sample = CreateSample(GammaDistribution, 1000000, Seed);
                        break;
                    case "Logistic":
                        //distribution=new  Troschuetz.Random.
                        Meta.Numerics.Statistics.Distributions.Distribution logistic_distribution = new Meta.Numerics.Statistics.Distributions.LogisticDistribution(benMAPValuationFunction.P1A, benMAPValuationFunction.P2A);
                        sample = CreateSample(logistic_distribution, 1000000);
                        
                        break;
                    case "Beta":
                        //distribution = new BetaDistribution() { Alpha = benMAPValuationFunction.P1A, Beta = benMAPValuationFunction.P2A };
                        Meta.Numerics.Statistics.Distributions.BetaDistribution BetaDistribution =
                            new Meta.Numerics.Statistics.Distributions.BetaDistribution(benMAPValuationFunction.P1A, benMAPValuationFunction.P2A);
                        sample = CreateSample(BetaDistribution, 1000000, Seed);
                        break;
                    case "Pareto":
                        //distribution = new ParetoDistribution() { Alpha = benMAPValuationFunction.P1A, Beta = benMAPValuationFunction.P2A };
                        Meta.Numerics.Statistics.Distributions.ParetoDistribution ParetoDistribution =
                            new Meta.Numerics.Statistics.Distributions.ParetoDistribution(benMAPValuationFunction.P1A, benMAPValuationFunction.P2A);
                        sample = CreateSample(ParetoDistribution, 1000000, Seed);
                        break;
                    case "Cauchy":
                        //distribution = new CauchyDistribution() { Alpha = benMAPValuationFunction.P1A, Gamma = benMAPValuationFunction.P2A };
                        Meta.Numerics.Statistics.Distributions.CauchyDistribution CauchyDistribution =
                            new Meta.Numerics.Statistics.Distributions.CauchyDistribution(benMAPValuationFunction.P1A, benMAPValuationFunction.P2A);
                        sample = CreateSample(CauchyDistribution, 1000000, Seed);
                        break;
                    case "Custom":
                        //distribution=new CustomDistributionEntries(){ 
                        //------------首先得到Custom的实例---------------然后计算均值-------------------
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
                            //switch (benMAPValuationFunction.DiscA)
                            //{
                            //    case "Uniform":
                            //    case "Triangular":
                            //        lhsResultArray[i] = lstCustom.GetRange(i * (lstCustom.Count / LatinHypercubePoints), (lstCustom.Count / LatinHypercubePoints)).Average();
                            //        break;
                            //    default:
                            //        lhsResultArray[i] = lstCustom.GetRange(i * (lstCustom.Count / LatinHypercubePoints), (lstCustom.Count / LatinHypercubePoints)).Median();
                            //        break;
                            //}
                            lhsResultArray[i] = lstCustom.GetRange(i * (lstCustom.Count / LatinHypercubePoints), (lstCustom.Count / LatinHypercubePoints)).Median();
                            //lhsResultArray[LatinHypercubePoints - i-1] = lstsamples[(LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints) > lstsamples.Count-1 ? lstsamples.Count-1 : (LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints)];
                            //i++;
                        }
                        return lhsResultArray;
                    //break;


                }

                List<double> lstlogistic = sample.ToList();
                lstlogistic.Sort();

                for (int i = 0; i < LatinHypercubePoints; i++)
                {
                    lhsResultArray[i] = lstlogistic.GetRange(i * (lstlogistic.Count / LatinHypercubePoints), (lstlogistic.Count / LatinHypercubePoints)).Median();
                    //lhsResultArray[LatinHypercubePoints - i-1] = lstsamples[(LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints) > lstsamples.Count-1 ? lstsamples.Count-1 : (LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints)];
                    //i++;
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
            switch ( PoolingMethod)
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
        /// <summary>
        /// 递归得到所有非None的Method
        /// </summary>
        /// <param name="allSelectValueMethod"></param>
        /// <param name="lstAll"></param>
        /// <param name="lstReturn"></param>
        public static void getAllChildMethodNotNone(AllSelectValuationMethod allSelectValueMethod, List<AllSelectValuationMethod> lstAll, ref List<AllSelectValuationMethod> lstReturn)
        {
            List<AllSelectValuationMethod> lstOne = lstAll.Where(p => p.PID == allSelectValueMethod.ID  && (p.PoolingMethod != "None" || p.NodeType == 100)).ToList();
            lstReturn.AddRange(lstOne);
            List<AllSelectValuationMethod> lstSec = lstAll.Where(p => p.PID == allSelectValueMethod.ID && (p.PoolingMethod == "None" )).ToList();

            foreach (AllSelectValuationMethod asvm in lstSec)
            {
                getAllChildMethodNotNone(asvm, lstAll, ref lstReturn);

            }
        }

        /// <summary>
        /// 递归得到所有非None的Method
        /// </summary>
        /// <param name="allSelectCRFunction"></param>
        /// <param name="lstAll"></param>
        /// <param name="lstReturn"></param>
        public static void getAllChildCR(AllSelectCRFunction allSelectCRFunction, List<AllSelectCRFunction> lstAll, ref List<AllSelectCRFunction> lstReturn)
        {
            List<AllSelectCRFunction> lstOne = lstAll.Where(p => p.PID == allSelectCRFunction.ID).ToList();//&& (p.PoolingMethod != "None" || p.NodeType == 4)).ToList();
            lstReturn.AddRange(lstOne);
            foreach (AllSelectCRFunction asvm in lstOne)
            {
                getAllChildCR(asvm, lstAll, ref lstReturn);

            }
        }

        /// <summary>
        /// 递归得到所有非None的Method
        /// </summary>
        /// <param name="allSelectCRFunction"></param>
        /// <param name="lstAll"></param>
        /// <param name="lstReturn"></param>
        public static void getAllChildCRNotNone(AllSelectCRFunction allSelectCRFunction, List<AllSelectCRFunction> lstAll, ref List<AllSelectCRFunction> lstReturn)
        {
            
            List<AllSelectCRFunction> lstOne = lstAll.Where(p => p.PID == allSelectCRFunction.ID).ToList();//&& (p.PoolingMethod != "None" || p.NodeType == 4)).ToList();
            //if(allSelectCRFunction.PoolingMethod!="None" && allSelectCRFunction.PoolingMethod!="")
            
            if (allSelectCRFunction.PoolingMethod == "None")
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

        /// <summary>
        /// 递归得到所有非None的MethodForPooling
        /// </summary>
        /// <param name="allSelectCRFunction"></param>
        /// <param name="lstAll"></param>
        /// <param name="lstReturn"></param>
        public static void getAllChildCRNotNoneForPooling(AllSelectCRFunction allSelectCRFunction, List<AllSelectCRFunction> lstAll, ref List<AllSelectCRFunction> lstReturn)
        {

            List<AllSelectCRFunction> lstOne = lstAll.Where(p => p.PID == allSelectCRFunction.ID).ToList();//&& (p.PoolingMethod != "None" || p.NodeType == 4)).ToList();
            //if(allSelectCRFunction.PoolingMethod!="None" && allSelectCRFunction.PoolingMethod!="")

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
        /// <summary>
        /// 递归得到所有非None的Method
        /// </summary>
        /// <param name="allSelectCRFunction"></param>
        /// <param name="lstAll"></param>
        /// <param name="lstReturn"></param>
        public static void getAllChildCRNotNoneCalulate(AllSelectCRFunction allSelectCRFunction, List<AllSelectCRFunction> lstAll, ref List<AllSelectCRFunction> lstReturn)
        {
            List<AllSelectCRFunction> lstOne = lstAll.Where(p => p.PID == allSelectCRFunction.ID && (p.PoolingMethod != "None" || p.NodeType == 100)).ToList();
            lstReturn.AddRange(lstOne);
            List<AllSelectCRFunction> lstSec = lstAll.Where(p => p.PID == allSelectCRFunction.ID).ToList();//&& (p.PoolingMethod != "None" || p.NodeType == 4)).ToList();



            foreach (AllSelectCRFunction asvm in lstSec)
            {
                if(asvm.PoolingMethod=="None")
                getAllChildCRNotNoneCalulate(asvm, lstAll, ref lstReturn);

            }

        }

        /// <summary>
        /// 递归得到所有非None的Method
        /// </summary>
        /// <param name="allSelectValueMethod"></param>
        /// <param name="lstAll"></param>
        /// <param name="lstReturn"></param>
        private static void getAllChildQALYMethodNotNone(AllSelectQALYMethod allSelectValueMethod, List<AllSelectQALYMethod> lstAll, ref List<AllSelectQALYMethod> lstReturn)
        {
            List<AllSelectQALYMethod> lstOne = lstAll.Where(p => p.PID == allSelectValueMethod.ID && (p.PoolingMethod != "None" || p.NodeType == 5)).ToList();
            lstReturn.AddRange(lstOne);
            List<AllSelectQALYMethod> lstSec = lstAll.Where(p => p.PID == allSelectValueMethod.ID && (p.PoolingMethod == "None"  )).ToList();

            foreach (AllSelectQALYMethod asvm in lstSec)
            {
                getAllChildQALYMethodNotNone(asvm, lstAll, ref lstReturn);

            }
        }
        ///// <summary>
        /// 计算Valuation
        /// </summary>
        /// <param name="ValuationMethodPoolingAndAggregation"></param>
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
                    //根据Population的year获取，DataSetID取setup的第一个
                    commondText = string.Format("select InflationDataSetID from InflationDataSets where SetupID={0} order by InflationDataSetID", CommonClass.MainSetup.SetupID);
                    ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commondText);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        getInflationFromDataSetIDAndYear(Convert.ToInt32(ds.Tables[0].Rows[0][0]),
                      CommonClass.BenMAPPopulation.Year, ref AllGoodsIndex, ref MedicalCostIndex, ref WageIndex);


                    }

                }
                //根据advance得到通货膨胀率，经济增长率
                if (valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance != null &&
                    valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.AdjustIncomeGrowthDatasetID != -1
                    && valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncomeGrowthYear != -1)
                {
                    dicIncome = getIncomeGrowthFactorsFromDataSetIDAndYear(valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.AdjustIncomeGrowthDatasetID,
                        valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregationAdvance.IncomeGrowthYear);
                }
                //else
                //{
                //    //根据Population的year获取，DataSetID取setup的第一个
                //    commondText = string.Format("select   IncomeGrowthAdjDatasetID  from IncomeGrowthAdjDatasets  where SetupID={0}   order by  IncomeGrowthAdjDatasetID    ", CommonClass.MainSetup.SetupID);
                //    ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commondText);
                //    if (ds.Tables[0].Rows.Count > 0)
                //    {
                //        dicIncome = getIncomeGrowthFactorsFromDataSetIDAndYear(Convert.ToInt32(ds.Tables[0].Rows[0][0]),
                //      CommonClass.BenMAPPopulation.Year);


                //    }

                //}
                //暂时忽略Advance里面的各种GridType调整
               
                //----------------------------------------------------------------------------------------------------------------------------------------------------
                foreach (ValuationMethodPoolingAndAggregationBase valuationMethodPoolingAndAggregationBase in valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                {
                    valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue = new List<AllSelectValuationMethodAndValue>();
                    
                    //从GridType得到所有网格
                    // Dictionary<int, Dictionary<string, double>> dicAllCRResult = getAllCrCalculateValues(valuationMethodPoolingAndAggregation.IncidencePoolingAndAggregation.PoolingMethods);
                    //----------如果---代表只要计算一次 Aggregation的值--------------
                    //--------------修正算法 12.6 ----
                    //--首先我们求出所有的第一级父节点----
                    //------判断weight等获得第二级父节点--依次获得所有级别，最多4级

                    List<AllSelectValuationMethod> queryFirst = valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod.Where(a => valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod.Where(c => c.NodeType == 2000).Select(p => p.PID).Contains(a.ID)).ToList();
                    if (queryFirst.Count() > 0)
                    {
                        foreach (AllSelectValuationMethod avmFirst in queryFirst)
                        {
                            //-----------首先应该集合所有的IncidenceValue-------------
                            try
                            {
                                AllSelectCRFunction allsCRFunction = valuationMethodPoolingAndAggregationBase.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.CRID == avmFirst.CRID).First();
                                CRSelectFunctionCalculateValue crv = allsCRFunction.CRSelectFunctionCalculateValue;
                                //CRSelectFunctionCalculateValue crv = null;
                                //try
                                //{
                                //    crv = lstCRAggregation.Where(p => p.CRSelectFunction.CRID == avmFirst.CRID).First();
                                //}
                                //catch(Exception ex)
                                //{}
                                //if (crv == null)
                                //{
                                //    AllSelectCRFunction allsCRFunction = valuationMethodPoolingAndAggregationBase.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.CRID == avmFirst.CRID).First();
                                //    crv = allsCRFunction.CRSelectFunctionCalculateValue;
                                //}
                                #region oldcode
                                //if (crv == null)
                                //{

                                //    List<AllSelectCRFunction> lstAllSelectCRFunction = new List<AllSelectCRFunction>();
                                //    getAllChildCR(valuationMethodPoolingAndAggregationBase.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.ID == avmFirst.ID).First(), valuationMethodPoolingAndAggregationBase.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstAllSelectCRFunction);
                                //    //-----------------首先从中得到NodeType=4--------Pooling-------
                                //    List<AllSelectCRFunction> lst4 = lstAllSelectCRFunction.Where(p => p.NodeType == 4).ToList();
                                //    List<AllSelectCRFunction> lst4Child = null;// lstAllSelectCRFunction.Where(p => p.NodeType == 4).ToList();
                                //    foreach (AllSelectCRFunction ac4 in lst4)
                                //    {
                                //        if (ac4.PoolingMethod == "None")
                                //        {
                                //            //--去掉
                                //            //lstAllSelectCRFunction.Remove(ac4);
                                //        }
                                //        else
                                //        {
                                //            AllSelectCRFunction ac4Add = new AllSelectCRFunction()
                                //            {
                                //                ID = ac4.ID,
                                //                PID = ac4.PID,
                                //                CRID = ac4.CRID,
                                //                PoolingMethod = ac4.PoolingMethod,
                                //                NodeType = ac4.NodeType,
                                //                EndPointGroupID = ac4.EndPointGroupID,
                                //                EndPointID = ac4.EndPointID,
                                //                Name = ac4.Name,
                                //                Weight = ac4.Weight


                                //            };

                                //            lst4Child = lstAllSelectCRFunction.Where(p => p.PID == ac4.ID).ToList();
                                //            if (lst4Child.Count > 0)
                                //            {
                                //                //---Pooling
                                //                crv = getPoolingMethodCRSelectFunctionCalculateValue(lst4Child.Select(a => a.CRSelectFunctionCalculateValue).ToList(), getPoolingMethodTypeEnumFromString(ac4.PoolingMethod), lst4Child.Select(p => p.Weight).ToList());
                                //                ac4Add.CRSelectFunctionCalculateValue = crv;
                                //                lstAllSelectCRFunction.Remove(ac4);
                                //                lstAllSelectCRFunction.Add(ac4Add);
                                //                foreach (AllSelectCRFunction ac4child in lst4Child)
                                //                {
                                //                    lstAllSelectCRFunction.Remove(ac4child);
                                //                }
                                //            }
                                //        }
                                //    }
                                //    //-----------------首先从中得到NodeType=3--------Pooling-------
                                //    List<AllSelectCRFunction> lst3 = lstAllSelectCRFunction.Where(p => p.NodeType == 3).ToList();
                                //    List<AllSelectCRFunction> lst3Child = null;// lstAllSelectCRFunction.Where(p => p.NodeType == 3).ToList();
                                //    foreach (AllSelectCRFunction ac3 in lst3)
                                //    {
                                //        if (ac3.PoolingMethod == "None")
                                //        {
                                //            //--去掉
                                //            //lstAllSelectCRFunction.Remove(ac3);
                                //        }
                                //        else
                                //        {
                                //            AllSelectCRFunction ac3Add = new AllSelectCRFunction()
                                //            {
                                //                ID = ac3.ID,
                                //                PID = ac3.PID,
                                //                CRID = ac3.CRID,
                                //                PoolingMethod = ac3.PoolingMethod,
                                //                NodeType = ac3.NodeType,
                                //                EndPointGroupID = ac3.EndPointGroupID,
                                //                EndPointID = ac3.EndPointID,
                                //                Name = ac3.Name,
                                //                Weight = ac3.Weight


                                //            };

                                //            lst3Child = lstAllSelectCRFunction.Where(p => p.PID == ac3.ID).ToList();

                                //            //---Pooling
                                //            crv = getPoolingMethodCRSelectFunctionCalculateValue(lst3Child.Select(a => a.CRSelectFunctionCalculateValue).ToList(), getPoolingMethodTypeEnumFromString(ac3.PoolingMethod), lst3Child.Select(p => p.Weight).ToList());
                                //            ac3Add.CRSelectFunctionCalculateValue = crv;
                                //            lstAllSelectCRFunction.Remove(ac3);
                                //            lstAllSelectCRFunction.Add(ac3Add);
                                //            foreach (AllSelectCRFunction ac3child in lst3Child)
                                //            {
                                //                lstAllSelectCRFunction.Remove(ac3child);
                                //            }
                                //        }
                                //    }
                                //    //------------------NodeType=2--------------Pooling------------
                                //    List<AllSelectCRFunction> lst2 = lstAllSelectCRFunction.Where(p => p.NodeType == 2).ToList();
                                //    List<AllSelectCRFunction> lst2Child = null;// lstAllSelectCRFunction.Where(p => p.NodeType == 2).ToList();
                                //    foreach (AllSelectCRFunction ac2 in lst2)
                                //    {
                                //        if (ac2.PoolingMethod == "None")
                                //        {
                                //            //--去掉
                                //            //lstAllSelectCRFunction.Remove(ac2);
                                //        }
                                //        else
                                //        {
                                //            AllSelectCRFunction ac2Add = new AllSelectCRFunction()
                                //            {
                                //                ID = ac2.ID,
                                //                PID = ac2.PID,
                                //                CRID = ac2.CRID,
                                //                PoolingMethod = ac2.PoolingMethod,
                                //                NodeType = ac2.NodeType,
                                //                EndPointGroupID = ac2.EndPointGroupID,
                                //                EndPointID = ac2.EndPointID,
                                //                Name = ac2.Name,
                                //                Weight = ac2.Weight


                                //            };

                                //            lst2Child = lstAllSelectCRFunction.Where(p => p.PID == ac2.ID).ToList();

                                //            //---Pooling
                                //            crv = getPoolingMethodCRSelectFunctionCalculateValue(lst2Child.Select(a => a.CRSelectFunctionCalculateValue).ToList(), getPoolingMethodTypeEnumFromString(ac2.PoolingMethod), lst2Child.Select(p => p.Weight).ToList());
                                //            ac2Add.CRSelectFunctionCalculateValue = crv;
                                //            lstAllSelectCRFunction.Remove(ac2);
                                //            lstAllSelectCRFunction.Add(ac2Add);
                                //            foreach (AllSelectCRFunction ac2child in lst2Child)
                                //            {
                                //                lstAllSelectCRFunction.Remove(ac2child);
                                //            }
                                //        }
                                //    }
                                //    //------------------NodeType=1--------------Pooling------------
                                //    List<AllSelectCRFunction> lst1 = lstAllSelectCRFunction.Where(p => p.NodeType == 1).ToList();
                                //    List<AllSelectCRFunction> lst1Child = null;// lstAllSelectCRFunction.Where(p => p.NodeType == 1).ToList();
                                //    foreach (AllSelectCRFunction ac1 in lst1)
                                //    {
                                //        if (ac1.PoolingMethod == "None")
                                //        {
                                //            //--去掉
                                //            //lstAllSelectCRFunction.Remove(ac1);
                                //        }
                                //        else
                                //        {
                                //            AllSelectCRFunction ac1Add = new AllSelectCRFunction()
                                //            {
                                //                ID = ac1.ID,
                                //                PID = ac1.PID,
                                //                CRID = ac1.CRID,
                                //                PoolingMethod = ac1.PoolingMethod,
                                //                NodeType = ac1.NodeType,
                                //                EndPointGroupID = ac1.EndPointGroupID,
                                //                EndPointID = ac1.EndPointID,
                                //                Name = ac1.Name,
                                //                Weight = ac1.Weight


                                //            };

                                //            lst1Child = lstAllSelectCRFunction.Where(p => p.PID == ac1.ID).ToList();

                                //            //---Pooling
                                //            crv = getPoolingMethodCRSelectFunctionCalculateValue(lst1Child.Select(a => a.CRSelectFunctionCalculateValue).ToList(), getPoolingMethodTypeEnumFromString(ac1.PoolingMethod), lst1Child.Select(p => p.Weight).ToList());
                                //            ac1Add.CRSelectFunctionCalculateValue = crv;
                                //            lstAllSelectCRFunction.Remove(ac1);
                                //            lstAllSelectCRFunction.Add(ac1Add);
                                //            foreach (AllSelectCRFunction ac1child in lst1Child)
                                //            {
                                //                lstAllSelectCRFunction.Remove(ac1child);
                                //            }
                                //        }
                                //    }
                                //    //------------------NodeType=0--------------Pooling-------------
                                //    List<AllSelectCRFunction> lst0 = lstAllSelectCRFunction.Where(p => p.NodeType == 0).ToList();
                                //    List<AllSelectCRFunction> lst0Child = null;// lstAllSelectCRFunction.Where(p => p.NodeType == 0).ToList();
                                //    //if(allsCRFunction.NodeType==0)
                                //    foreach (AllSelectCRFunction ac0 in lst0)
                                //    {
                                //        if (ac0.PoolingMethod == "None")
                                //        {
                                //            //--去掉
                                //            //lstAllSelectCRFunction.Remove(ac0);
                                //        }
                                //        else
                                //        {
                                //            AllSelectCRFunction ac0Add = new AllSelectCRFunction()
                                //            {
                                //                ID = ac0.ID,
                                //                PID = ac0.PID,
                                //                CRID = ac0.CRID,
                                //                PoolingMethod = ac0.PoolingMethod,
                                //                NodeType = ac0.NodeType,
                                //                EndPointGroupID = ac0.EndPointGroupID,
                                //                EndPointID = ac0.EndPointID,
                                //                Name = ac0.Name,
                                //                Weight = ac0.Weight


                                //            };

                                //            lst0Child = lstAllSelectCRFunction.Where(p => p.PID == ac0.ID).ToList();

                                //            //---Pooling
                                //            crv = getPoolingMethodCRSelectFunctionCalculateValue(lst0Child.Select(a => a.CRSelectFunctionCalculateValue).ToList(), getPoolingMethodTypeEnumFromString(ac0.PoolingMethod), lst0Child.Select(p => p.Weight).ToList());
                                //            ac0Add.CRSelectFunctionCalculateValue = crv;
                                //            lstAllSelectCRFunction.Remove(ac0);
                                //            lstAllSelectCRFunction.Add(ac0Add);
                                //            foreach (AllSelectCRFunction ac0child in lst0Child)
                                //            {
                                //                lstAllSelectCRFunction.Remove(ac0child);
                                //            }
                                //        }
                                //    }
                                //    crv = getPoolingMethodCRSelectFunctionCalculateValue(lstAllSelectCRFunction.Where(p => p.CRSelectFunctionCalculateValue != null).Select(a => a.CRSelectFunctionCalculateValue).ToList(), getPoolingMethodTypeEnumFromString(allsCRFunction.PoolingMethod), lstAllSelectCRFunction.Where(p => p.CRSelectFunctionCalculateValue != null).Select(p => p.Weight).ToList());
                                //}
                                #endregion
                                var queryLeaf = valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod.Where(a => a.PID == avmFirst.ID).ToList();
                                List<AllSelectValuationMethodAndValue> lstTemp = new List<AllSelectValuationMethodAndValue>();
                                //foreach (AllSelectValuationMethod avmLeaf in queryLeaf)
                              
                                for (int iqueryLeaf = 0; iqueryLeaf < queryLeaf.Count(); iqueryLeaf++)
                                {
                                    AllSelectValuationMethod avmLeaf = queryLeaf[iqueryLeaf];
                                    //----------分别计算每一个Function---------------------
                                    //------------------------------------------------------首先Aggregation---------------------
                                    CRSelectFunctionCalculateValue crvCal = null;
                                    //if (crv == null)
                                    //{
                                    //    AllSelectCRFunction allsCRFunction = valuationMethodPoolingAndAggregationBase.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.CRID == avmFirst.CRID).First();
                                    //    crv = allsCRFunction.CRSelectFunctionCalculateValue;

                                    //    if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation != null
                                    //        && CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
                                    //    {
                                    //        //------------做Aggration--------

                                    //        crv = ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.ValuationAggregation.GridDefinitionID);
                                    //    }
                                        
                                    //}
                                    

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
                                        //else
                                        //{
                                            valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Add(allSelectValuationMethodAndValue);

                                        //}
                                    }


                                }
                                if (lstTemp.Count > 0)
                                {
                                    //----------集合-----------------------------
                                    AllSelectValuationMethodAndValue allSelectValuationMethodAndValueFirst = getPoolingLstAllSelectValuationMethodAndValue(lstTemp, getPoolingMethodTypeEnumFromString(avmFirst.PoolingMethod), queryLeaf.Select(p => p.Weight).ToList());
                                    allSelectValuationMethodAndValueFirst.AllSelectValuationMethod = avmFirst;
                                    //-------去掉子节点数据!
                                    //foreach (AllSelectValuationMethodAndValue alsv in lstTemp)
                                    //{
                                    //    valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Remove(alsv);
                                    //}
                                    valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Add(allSelectValuationMethodAndValueFirst);
                                    GC.Collect();
                                }
                            }
                            catch (Exception ex)
                            { }


                        }

                    }
                    //------------算第二个层次-----------------------------
                    var querySecond = valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod.Where(a => queryFirst.Select(p => p.PID).Contains(a.ID));
                    if (querySecond.Count() > 0)
                    {
                        foreach (AllSelectValuationMethod avmSecond in querySecond)
                        {
                            if (avmSecond.PoolingMethod != "None")
                            {
                                List<AllSelectValuationMethod> lstChild = new List<AllSelectValuationMethod>();
                                getAllChildMethodNotNone(avmSecond, valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod, ref lstChild);
                                List<int> lstID=lstChild.Select(p=>p.ID).ToList();
                                //----------集合-----------------------------
                                AllSelectValuationMethodAndValue allSelectValuationMethodAndValueSec = getPoolingLstAllSelectValuationMethodAndValue(valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Where(p => lstID.Contains(p.AllSelectValuationMethod.ID)).ToList(), getPoolingMethodTypeEnumFromString(avmSecond.PoolingMethod), valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Where(p => lstID.Contains(p.AllSelectValuationMethod.ID)).ToList().Select(p => p.AllSelectValuationMethod.Weight).ToList());
                                var query=valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Where(p => lstID.Contains(p.AllSelectValuationMethod.ID)).ToList();
                                foreach (AllSelectValuationMethodAndValue alsv in query)
                                {
                                    valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Remove(alsv);
 
                                }
                                allSelectValuationMethodAndValueSec.AllSelectValuationMethod = avmSecond;
                                valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Add(allSelectValuationMethodAndValueSec);
                                //---删掉不需要的------------------
                                //List<AllSelectValuationMethodAndValue> lstRemove = valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Where(p => lstID.Contains(p.AllSelectValuationMethod.ID)).ToList();
                                //foreach (AllSelectValuationMethodAndValue acRemove in lstRemove)
                                //{
                                //    valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Remove(acRemove);
                                //}
                                //lstRemove = null;
                                GC.Collect();
                            }
                            //----------集合-----------------------------
                        }
                    }
                    //------------算第三个层次----------------------------------20120101 ---修改成循环一直往上循环知道没有queryThree为止！
                    List<AllSelectValuationMethod> queryThree = valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod.Where(a => querySecond.Select(p => p.PID).Contains(a.ID)).ToList();
                    while (queryThree.Count() > 0)
                    {
                        foreach (AllSelectValuationMethod avmThree in queryThree)
                        {

                            //----------集合-----------------------------
                            if (avmThree.PoolingMethod != "None")
                            {
                                List<AllSelectValuationMethod> lstChild = new List<AllSelectValuationMethod>();
                                getAllChildMethodNotNone(avmThree, valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod, ref lstChild);
                                List<int> lstID = lstChild.Select(p => p.ID).ToList();
                                //----------集合-----------------------------
                                AllSelectValuationMethodAndValue allSelectValuationMethodAndValueSec = getPoolingLstAllSelectValuationMethodAndValue(valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Where(p => lstID.Contains(p.AllSelectValuationMethod.ID)).ToList(), getPoolingMethodTypeEnumFromString(avmThree.PoolingMethod), valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Where(p => lstID.Contains(p.AllSelectValuationMethod.ID)).ToList().Select(p => p.AllSelectValuationMethod.Weight).ToList());
                                allSelectValuationMethodAndValueSec.AllSelectValuationMethod = avmThree;
                                var query = valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Where(p => lstID.Contains(p.AllSelectValuationMethod.ID)).ToList();
                                foreach (AllSelectValuationMethodAndValue alsv in query)
                                {
                                    valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Remove(alsv);

                                }
                                valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Add(allSelectValuationMethodAndValueSec);
                                //---删掉不需要的------------------ modify by xiejp 20120919 --不删掉!
                                //List<AllSelectValuationMethodAndValue> lstRemove = valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Where(p => lstID.Contains(p.AllSelectValuationMethod.ID)).ToList();
                                //foreach (AllSelectValuationMethodAndValue acRemove in lstRemove)
                                //{
                                //    valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethodAndValue.Remove(acRemove);
                                //}
                                //lstRemove = null;
                            }
                            GC.Collect();
                        }
                        queryThree = valuationMethodPoolingAndAggregationBase.LstAllSelectValuationMethod.Where(a => queryThree.Select(p => p.PID).Contains(a.ID)).ToList();
                    }

                    //--------------算第四个层次------------------------------
                   
                     
                }
                dicIncome = null;
                lstAllRowCol = null;
                //lstCRAggregation = null;
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
                    //------如果没有则重造
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        //int iCount= fb.ExecuteScalar(string.Format("select count(*) from GridDefinitionPercentages a where a.SourceGridDefinitionID={0} and a.TargetGridDefinitionID={1}"
                        Configuration.ConfigurationCommonClass.creatPercentageToDatabase(gridRelationShipPopulation.bigGridID, gridRelationShipPopulation.smallGridID==28?27:gridRelationShipPopulation.smallGridID);
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
       public static Dictionary<string, Dictionary<string, Dictionary<string, double>>> DicRelationShipAll=new Dictionary<string,Dictionary<string,Dictionary<string,double>>>();
        public static CRSelectFunctionCalculateValue ApplyAggregationCRSelectFunctionCalculateValue(CRSelectFunctionCalculateValue crSelectFunctionCalculateValueFrom,int GridFrom ,int GridTo)
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
                    //else
                    //{
                    //    gridRelationship = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == GridFrom && p.smallGridID == GridTo).First();



                    //}
                }
                catch
                { 
                }
                if (gridRelationship == null)
                {
                    //return crSelectFunctionCalculateValueFrom;
                    Configuration.ConfigurationCommonClass.creatPercentageToDatabase(GridTo, GridFrom);
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
                //--------如果有的话则可以用这个----------------
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds;//
                int iCount=Convert.ToInt32( fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, "select count(*) from ("+str+" ) a"));
                if (iCount == 0)
                {

                    Configuration.ConfigurationCommonClass.creatPercentageToDatabase(GridTo, GridFrom);
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
                                                anew.LstPercentile[iavaLst]+=dLstPercentile * Convert.ToSingle(k.Value) / Convert.ToSingle(d);
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
                                            //Mean = ava.Mean * Convert.ToSingle(k.Value) / Convert.ToSingle(d),
                                            //StandardDeviation = ava.StandardDeviation * Convert.ToSingle(k.Value) / Convert.ToSingle(d),
                                            //Variance = ava.Variance * Convert.ToSingle(k.Value) / Convert.ToSingle(d),
                                            Population = ava.Population * Convert.ToSingle(k.Value) / Convert.ToSingle(d),
                                            Incidence = ava.Incidence,
                                            Baseline = ava.Baseline * Convert.ToSingle(k.Value) / Convert.ToSingle(d),
                                            Delta = ava.Delta,
                                            //PercentOfBaseline= ava.PercentOfBaseline


                                            //LstPercentile=ava.LstPercentile==null?null:ava.LstPercentile.Aggregate(
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
                                    //crOut.CRCalculateValues.Last().PercentOfBaseline= crOut.CRCalculateValues.Last().
                                    
                                    if (anew.LstPercentile != null && anew.LstPercentile.Count > 0)
                                    {
                                        anew.Mean = Configuration.ConfigurationCommonClass.getMean(anew.LstPercentile);
                                        anew.Variance = Configuration.ConfigurationCommonClass.getVariance(anew.LstPercentile, anew.PointEstimate);
                                        anew.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(anew.LstPercentile, anew.PointEstimate);
                                    }

                                    anew.PercentOfBaseline =anew.Baseline==0?0: Convert.ToSingle(Math.Round((anew.Mean / anew.Baseline) * 100, 4));
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
                        anewfirst.LstPercentile = new List<float>();// crSelectFunctionCalculateValueFrom.CRCalculateValues.First().LstPercentile;
                        if (crSelectFunctionCalculateValueFrom.CRCalculateValues.First().LstPercentile != null)
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
                                    anew.PointEstimate += CRCalculateValue.PointEstimate*Convert.ToSingle(k.Value);
                                    //anew.Mean += CRCalculateValue.Mean * Convert.ToSingle(k.Value);
                                    //anew.Variance += CRCalculateValue.Variance * Convert.ToSingle(k.Value);
                                    //anew.StandardDeviation += CRCalculateValue.StandardDeviation * Convert.ToSingle(k.Value);
                                    anew.Population += CRCalculateValue.Population * Convert.ToSingle(k.Value);
                                    
                                    if (!float.IsNaN(CRCalculateValue.Incidence))
                                    {
                                        anew.Incidence = (anew.Incidence + CRCalculateValue.Incidence * Convert.ToSingle(k.Value));
 
                                    }
                                    anew.Baseline += CRCalculateValue.Baseline * Convert.ToSingle(k.Value);
                                    anew.Delta = (anew.Delta + CRCalculateValue.Delta * Convert.ToSingle(k.Value) * CRCalculateValue.Population);
                                    if (float.IsNaN(anew.Delta )) anew.Delta = 0;
                                    //anew.PercentOfBaseline = (anew.PercentOfBaseline + CRCalculateValue.PercentOfBaseline * Convert.ToSingle(k.Value)) / 2;
                                    if (CRCalculateValue.LstPercentile != null)
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
                            anew.Incidence = anew.Incidence / gra.Value.Count;
                            anew.Delta = anew.Delta / anew.Population;
                            if (float.IsNaN(anew.Delta)) anew.Delta = 0;
                            anew.PercentOfBaseline =anew.Baseline==0?0: Convert.ToSingle(Math.Round((anew.Mean / anew.Baseline) * 100, 4));
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
                    //----------------Dic-------------------------
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
                        //gridRelationship = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == GridFrom && p.smallGridID == GridTo).First();
                        //----------To更加小----------------------------
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
                                            //LstPercentile=crin.LstPercentile==null||crin.LstPercentile.Count==0?crin.LstPercen
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
                        // gridRelationship = CommonClass.LstGridRelationshipAll.Where(p => p.bigGridID == GridTo && p.smallGridID == GridFrom).First();
                        //To 大----------
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
                                        crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].Delta =Convert.ToSingle(( crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].Delta + crsmall.Delta)/2.000);
                                        crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].Incidence += Convert.ToSingle((crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].Delta + crsmall.Incidence) / 2.000);
                                        crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].Mean += crsmall.Mean;
                                        //crOut.CRCalculateValues[crOut.CRCalculateValues.Count - 1].PercentOfBaseline += crsmall.PercentOfBaseline;
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
            catch
            {
                return null;
            }
            return crOut;
        }
        /// <summary>
        /// 计算QALY
        /// </summary>
        /// <param name="valuationMethodPoolingAndAggregationBase"></param>
        public static void CalculateQALYMethodPoolingAndAggregation(ref ValuationMethodPoolingAndAggregation valuationMethodPoolingAndAggregation)
        {
            try
            {
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commondText = "";

                //System.Data.DataSet ds = null;
                List<double> lstWeight = new List<double>();
                int i = 0;
                List<RowCol> lstAllRowCol = getAllRowColFromGridType(CommonClass.GBenMAPGrid);
                foreach (ValuationMethodPoolingAndAggregationBase valuationMethodPoolingAndAggregationBase in valuationMethodPoolingAndAggregation.lstValuationMethodPoolingAndAggregationBase)
                {
                    //暂时忽略Advance里面的各种GridType调整
                    if (valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethod == null || valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethod.Count == 0)
                        continue;
                    valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue = new List<AllSelectQALYMethodAndValue>();
                    var queryFirst = valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethod.Where(a => valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethod.Where(c => c.NodeType == 3000).Select(p => p.PID).Contains(a.ID));
                    if (queryFirst.Count() > 0)
                    {
                        foreach (AllSelectQALYMethod avmFirst in queryFirst)
                        {
                            //-----------首先应该集合所有的IncidenceValue-------------
                            AllSelectCRFunction allsCRFunction = valuationMethodPoolingAndAggregationBase.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.CRID == avmFirst.CRID).First();
                            CRSelectFunctionCalculateValue crv = allsCRFunction.CRSelectFunctionCalculateValue;
                            CRSelectFunctionCalculateValue crvCal = crv;

                            #region oldcode
                            //if (crv == null)
                            //{
                            //    List<AllSelectCRFunction> lstAllSelectCRFunction =new List<AllSelectCRFunction>();
                            //    getAllChildCR(valuationMethodPoolingAndAggregationBase.IncidencePoolingAndAggregation.lstAllSelectCRFuntion.Where(p => p.CRID == avmFirst.CRID).First(), valuationMethodPoolingAndAggregationBase.IncidencePoolingAndAggregation.lstAllSelectCRFuntion, ref lstAllSelectCRFunction);

                            //    //-----------------首先从中得到NodeType=4--------Pooling-------
                            //    List<AllSelectCRFunction> lst4 = lstAllSelectCRFunction.Where(p => p.NodeType == 4).ToList();
                            //    List<AllSelectCRFunction> lst4Child = null;// lstAllSelectCRFunction.Where(p => p.NodeType == 4).ToList();
                            //    foreach (AllSelectCRFunction ac4 in lst4)
                            //    {
                            //        if (ac4.PoolingMethod == "None")
                            //        {
                            //            //--去掉
                            //            //lstAllSelectCRFunction.Remove(ac4);
                            //        }
                            //        else
                            //        {
                            //            AllSelectCRFunction ac4Add = new AllSelectCRFunction()
                            //            {
                            //                ID = ac4.ID,
                            //                PID = ac4.PID,
                            //                CRID = ac4.CRID,
                            //                PoolingMethod = ac4.PoolingMethod,
                            //                NodeType = ac4.NodeType,
                            //                EndPointGroupID = ac4.EndPointGroupID,
                            //                EndPointID = ac4.EndPointID,
                            //                Name = ac4.Name,
                            //                Weight = ac4.Weight


                            //            };

                            //            lst4Child = lstAllSelectCRFunction.Where(p => p.PID == ac4.ID).ToList();
                            //            if (lst4Child.Count > 0)
                            //            {
                            //                //---Pooling
                            //                crv = getPoolingMethodCRSelectFunctionCalculateValue(lst4Child.Select(a => a.CRSelectFunctionCalculateValue).ToList(), getPoolingMethodTypeEnumFromString(ac4.PoolingMethod), lst4Child.Select(p => p.Weight).ToList());
                            //                ac4Add.CRSelectFunctionCalculateValue = crv;
                            //                lstAllSelectCRFunction.Remove(ac4);
                            //                lstAllSelectCRFunction.Add(ac4Add);
                            //                foreach (AllSelectCRFunction ac4child in lst4Child)
                            //                {
                            //                    lstAllSelectCRFunction.Remove(ac4child);
                            //                }
                            //            }
                            //        }
                            //    }
                                
                            //    //-----------------首先从中得到NodeType=3--------Pooling-------
                            //    List<AllSelectCRFunction> lst3= lstAllSelectCRFunction.Where(p => p.NodeType == 3).ToList();
                            //    List<AllSelectCRFunction> lst3Child = null;// lstAllSelectCRFunction.Where(p => p.NodeType == 3).ToList();
                            //    foreach (AllSelectCRFunction ac3 in lst3)
                            //    {
                            //        if (ac3.PoolingMethod == "None")
                            //        {
                            //            //--去掉
                            //            //lstAllSelectCRFunction.Remove(ac3);
                            //        }
                            //        else
                            //        {
                            //            AllSelectCRFunction ac3Add = new AllSelectCRFunction()
                            //            {
                            //                ID = ac3.ID,
                            //                PID = ac3.PID,
                            //                CRID = ac3.CRID,
                            //                PoolingMethod = ac3.PoolingMethod,
                            //                NodeType = ac3.NodeType,
                            //                EndPointGroupID = ac3.EndPointGroupID,
                            //                EndPointID = ac3.EndPointID,
                            //                Name = ac3.Name,
                            //                Weight = ac3.Weight


                            //            };
                                        
                            //            lst3Child = lstAllSelectCRFunction.Where(p => p.PID == ac3.ID).ToList();

                            //            //---Pooling
                            //            crv = getPoolingMethodCRSelectFunctionCalculateValue(lst3Child.Select(a => a.CRSelectFunctionCalculateValue).ToList(), getPoolingMethodTypeEnumFromString(ac3.PoolingMethod), lst3Child.Select(p => p.Weight).ToList());
                            //            ac3Add.CRSelectFunctionCalculateValue = crv;
                            //            lstAllSelectCRFunction.Remove(ac3);
                            //            lstAllSelectCRFunction.Add(ac3Add);
                            //            foreach (AllSelectCRFunction ac3child in lst3Child)
                            //            {
                            //                lstAllSelectCRFunction.Remove(ac3child);
                            //            }
                            //        }
                            //    }
                            //    //------------------NodeType=2--------------Pooling------------
                            //    List<AllSelectCRFunction> lst2 = lstAllSelectCRFunction.Where(p => p.NodeType == 2).ToList();
                            //    List<AllSelectCRFunction> lst2Child = null;// lstAllSelectCRFunction.Where(p => p.NodeType == 2).ToList();
                            //    foreach (AllSelectCRFunction ac2 in lst2)
                            //    {
                            //        if (ac2.PoolingMethod == "None")
                            //        {
                            //            //--去掉
                            //            //lstAllSelectCRFunction.Remove(ac2);
                            //        }
                            //        else
                            //        {
                            //            AllSelectCRFunction ac2Add = new AllSelectCRFunction()
                            //            {
                            //                ID = ac2.ID,
                            //                PID = ac2.PID,
                            //                CRID = ac2.CRID,
                            //                PoolingMethod = ac2.PoolingMethod,
                            //                NodeType = ac2.NodeType,
                            //                EndPointGroupID = ac2.EndPointGroupID,
                            //                EndPointID = ac2.EndPointID,
                            //                Name = ac2.Name,
                            //                Weight = ac2.Weight


                            //            };

                            //            lst2Child = lstAllSelectCRFunction.Where(p => p.PID == ac2.ID).ToList();

                            //            //---Pooling
                            //            crv = getPoolingMethodCRSelectFunctionCalculateValue(lst2Child.Select(a => a.CRSelectFunctionCalculateValue).ToList(), getPoolingMethodTypeEnumFromString(ac2.PoolingMethod), lst2Child.Select(p => p.Weight).ToList());
                            //            ac2Add.CRSelectFunctionCalculateValue = crv;
                            //            lstAllSelectCRFunction.Remove(ac2);
                            //            lstAllSelectCRFunction.Add(ac2Add);
                            //            foreach (AllSelectCRFunction ac2child in lst2Child)
                            //            {
                            //                lstAllSelectCRFunction.Remove(ac2child);
                            //            }
                            //        }
                            //    }
                            //    //------------------NodeType=1--------------Pooling------------
                            //    List<AllSelectCRFunction> lst1 = lstAllSelectCRFunction.Where(p => p.NodeType == 1).ToList();
                            //    List<AllSelectCRFunction> lst1Child = null;// lstAllSelectCRFunction.Where(p => p.NodeType == 1).ToList();
                            //    foreach (AllSelectCRFunction ac1 in lst1)
                            //    {
                            //        if (ac1.PoolingMethod == "None")
                            //        {
                            //            //--去掉
                            //            //lstAllSelectCRFunction.Remove(ac1);
                            //        }
                            //        else
                            //        {
                            //            AllSelectCRFunction ac1Add = new AllSelectCRFunction()
                            //            {
                            //                ID = ac1.ID,
                            //                PID = ac1.PID,
                            //                CRID = ac1.CRID,
                            //                PoolingMethod = ac1.PoolingMethod,
                            //                NodeType = ac1.NodeType,
                            //                EndPointGroupID = ac1.EndPointGroupID,
                            //                EndPointID = ac1.EndPointID,
                            //                Name = ac1.Name,
                            //                Weight = ac1.Weight


                            //            };

                            //            lst1Child = lstAllSelectCRFunction.Where(p => p.PID == ac1.ID).ToList();

                            //            //---Pooling
                            //            crv = getPoolingMethodCRSelectFunctionCalculateValue(lst1Child.Select(a => a.CRSelectFunctionCalculateValue).ToList(), getPoolingMethodTypeEnumFromString(ac1.PoolingMethod), lst1Child.Select(p => p.Weight).ToList());
                            //            ac1Add.CRSelectFunctionCalculateValue = crv;
                            //            lstAllSelectCRFunction.Remove(ac1);
                            //            lstAllSelectCRFunction.Add(ac1Add);
                            //            foreach (AllSelectCRFunction ac1child in lst1Child)
                            //            {
                            //                lstAllSelectCRFunction.Remove(ac1child);
                            //            }
                            //        }
                            //    }
                            //    //------------------NodeType=0--------------Pooling-------------
                            //    List<AllSelectCRFunction> lst0 = lstAllSelectCRFunction.Where(p => p.NodeType == 0).ToList();
                            //    List<AllSelectCRFunction> lst0Child = null;// lstAllSelectCRFunction.Where(p => p.NodeType == 0).ToList();
                            //    foreach (AllSelectCRFunction ac0 in lst0)
                            //    {
                            //        if (ac0.PoolingMethod == "None")
                            //        {
                            //            //--去掉
                            //            //lstAllSelectCRFunction.Remove(ac0);
                            //        }
                            //        else
                            //        {
                            //            AllSelectCRFunction ac0Add = new AllSelectCRFunction()
                            //            {
                            //                ID = ac0.ID,
                            //                PID = ac0.PID,
                            //                CRID = ac0.CRID,
                            //                PoolingMethod = ac0.PoolingMethod,
                            //                NodeType = ac0.NodeType,
                            //                EndPointGroupID = ac0.EndPointGroupID,
                            //                EndPointID = ac0.EndPointID,
                            //                Name = ac0.Name,
                            //                Weight = ac0.Weight


                            //            };

                            //            lst0Child = lstAllSelectCRFunction.Where(p => p.PID == ac0.ID).ToList();

                            //            //---Pooling
                            //            crv = getPoolingMethodCRSelectFunctionCalculateValue(lst0Child.Select(a => a.CRSelectFunctionCalculateValue).ToList(), getPoolingMethodTypeEnumFromString(ac0.PoolingMethod), lst0Child.Select(p => p.Weight).ToList());
                            //            ac0Add.CRSelectFunctionCalculateValue = crv;
                            //            lstAllSelectCRFunction.Remove(ac0);
                            //            lstAllSelectCRFunction.Add(ac0Add);
                            //            foreach (AllSelectCRFunction ac0child in lst0Child)
                            //            {
                            //                lstAllSelectCRFunction.Remove(ac0child);
                            //            }
                            //        }
                            //    }
                            //    crv = getPoolingMethodCRSelectFunctionCalculateValue(lstAllSelectCRFunction.Where(p => p.CRSelectFunctionCalculateValue != null).Select(a => a.CRSelectFunctionCalculateValue).ToList(), getPoolingMethodTypeEnumFromString(allsCRFunction.PoolingMethod), lstAllSelectCRFunction.Where(p => p.CRSelectFunctionCalculateValue != null).Select(p => p.Weight).ToList());
                            //}
                            #endregion
                            var queryLeaf = valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethod.Where(a => a.PID == avmFirst.ID).ToList();
                            List<AllSelectQALYMethodAndValue> lstTemp = new List<AllSelectQALYMethodAndValue>();
                            //foreach (AllSelectQALYMethod avmLeaf in queryLeaf)
                            for(int iqueryLeaf=0;iqueryLeaf<queryLeaf.Count();iqueryLeaf++)
                            {
                                //----------分别计算每一个Function---------------------
                                AllSelectQALYMethod avmLeaf = queryLeaf[iqueryLeaf];
                                //------------------------------------------------------首先Aggregation---------------------
                                //if (CommonClass.IncidencePoolingAndAggregationAdvance != null && CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation != null
                                //    && CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
                                //{
                                //    //------------做Aggration--------
                                //    crvCal = ApplyAggregationCRSelectFunctionCalculateValue(crv, CommonClass.GBenMAPGrid.GridDefinitionID, CommonClass.IncidencePoolingAndAggregationAdvance.QALYAggregation.GridDefinitionID);
                                //}
                                //else
                                //{
                                //    crvCal = crv;
                                //}
                                if (crvCal != null)
                                {
                                    AllSelectQALYMethodAndValue AllSelectQALYMethodAndValue = getOneAllSelectQALYMethodCRSelectFunctionCalculateValue(crvCal,
                                        ref  avmLeaf);
                                    AllSelectQALYMethodAndValue.AllSelectQALYMethod = avmLeaf;
                                    if (avmFirst.PoolingMethod != "None")
                                    {
                                        lstTemp.Add(AllSelectQALYMethodAndValue);
                                    }
                                    //else
                                    //{
                                        valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Add(AllSelectQALYMethodAndValue);

                                    //}
                                }


                            }
                            if (lstTemp.Count > 0)
                            {
                                //----------集合-----------------------------
                                AllSelectQALYMethodAndValue AllSelectQALYMethodAndValueFirst = getPoolingLstAllSelectQALYMethodAndValue(lstTemp, getPoolingMethodTypeEnumFromString(avmFirst.PoolingMethod), queryLeaf.Select(p => p.Weight).ToList());
                                AllSelectQALYMethodAndValueFirst.AllSelectQALYMethod = avmFirst;
                                valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Add(AllSelectQALYMethodAndValueFirst);
                            }


                        }
                    }
                    //------------算第二个层次-----------------------------
                    //---------------  modify by xiejp ----------------修改为先第二层次，然后根据！
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
                                //----------集合-----------------------------
                                AllSelectQALYMethodAndValue AllSelectQALYMethodAndValueSec = getPoolingLstAllSelectQALYMethodAndValue(valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Where(p => lstID.Contains(p.AllSelectQALYMethod.ID)).ToList(), getPoolingMethodTypeEnumFromString(avmSecond.PoolingMethod), valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Where(p => lstID.Contains(p.AllSelectQALYMethod.ID)).ToList().Select(p => p.AllSelectQALYMethod.Weight).ToList());
                                AllSelectQALYMethodAndValueSec.AllSelectQALYMethod = avmSecond;
                                valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Add(AllSelectQALYMethodAndValueSec);
                                //---删掉不需要的------------------
                                //List<AllSelectQALYMethodAndValue> lstRemove = valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Where(p => lstID.Contains(p.AllSelectQALYMethod.ID)).ToList();
                                //foreach (AllSelectQALYMethodAndValue acRemove in lstRemove)
                                //{
                                //    valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Remove(acRemove);
                                //}
                            }
                            //----------集合-----------------------------
                        }
                    }
                    //------------算第三个层次进行 循环如果再找不到父节点，则跳出循环----------------------------------120101

                    List<AllSelectQALYMethod> queryThree = valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethod.Where(a => querySecond.Select(p => p.PID).Contains(a.ID)).ToList();
                    while (queryThree.Count() > 0)
                    {
                        foreach (AllSelectQALYMethod avmThree in queryThree)
                        {

                            //----------集合-----------------------------
                            if (avmThree.PoolingMethod != "None")
                            {
                                List<AllSelectQALYMethod> lstChild = new List<AllSelectQALYMethod>();
                                getAllChildQALYMethodNotNone(avmThree, valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethod, ref lstChild);
                                List<int> lstID = lstChild.Select(p => p.ID).ToList();
                                //----------集合-----------------------------
                                AllSelectQALYMethodAndValue AllSelectQALYMethodAndValueSec = getPoolingLstAllSelectQALYMethodAndValue(valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Where(p => lstID.Contains(p.AllSelectQALYMethod.ID)).ToList(), getPoolingMethodTypeEnumFromString(avmThree.PoolingMethod), valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Where(p => lstID.Contains(p.AllSelectQALYMethod.ID)).ToList().Select(p => p.AllSelectQALYMethod.Weight).ToList());
                                AllSelectQALYMethodAndValueSec.AllSelectQALYMethod = avmThree;
                                valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Add(AllSelectQALYMethodAndValueSec);
                                //---删掉不需要的------------------
                                //List<AllSelectQALYMethodAndValue> lstRemove = valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Where(p => lstID.Contains(p.AllSelectQALYMethod.ID)).ToList();
                                //foreach (AllSelectQALYMethodAndValue acRemove in lstRemove)
                                //{
                                //    valuationMethodPoolingAndAggregationBase.lstAllSelectQALYMethodAndValue.Remove(acRemove);
                                //}
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="allSelectQALYMethodAndValue"></param>
        
        /// <summary>
        /// 生成QALY-------已去掉拉丁立体方。在显示结果时生成
        /// </summary>
        /// <param name="crSelectFunctionCalculateValue"></param>
        /// <param name="allSelectQALYMethod"></param>
        /// <returns></returns>
        public static AllSelectQALYMethodAndValue getOneAllSelectQALYMethodCRSelectFunctionCalculateValue(CRSelectFunctionCalculateValue crSelectFunctionCalculateValue,ref AllSelectQALYMethod allSelectQALYMethod)
        {
            try
            {
                AllSelectQALYMethodAndValue allSelectQALYMethodAndValue = new AllSelectQALYMethodAndValue();
                allSelectQALYMethodAndValue.AllSelectQALYMethod = allSelectQALYMethod;
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = "";
                //是否有Setup变量--如果有，则得到------
                            //Dictionary<string, double> dicSetupVariable = new Dictionary<string, double>();
                //foreach (SetupVariableValues sv in lstSetupVariable)
                //{
                //    dicSetupVariable.Add(sv.Col + "," + sv.Row, sv.Value);
                //}
                // Dictionary<string, double> dicVariable = getDicSetupVariableColRow(col, row, lstSetupVariable, baseControlGroup.GridType.GridDefinitionID, lstGridRelationship);
                
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
                    allSelectQALYMethod.lstQALYLast = lstQALY;// getMedianSample(lstQALY, 500);
                    //    if (lstQALY.Count > 100)
                    //    {
                    //        for (int iMonto = 0; iMonto < 100; iMonto++)
                    //        {
                    //            //for(int iTemp=0;iTemp<crCalculateValue.LstPercentile.Count;iTemp++)
                    //            //{

                    //            allSelectQALYMethod.lstQALYLast.Add(Convert.ToSingle(Math.Round(lstQALY.GetRange(iMonto * lstQALY.Count / 100, lstQALY.Count / 100).Median(), 4)));

                    //            //}

                    //        }
                    //    }
                    //    else
                    //    {
                    //        for (int iMonto = 0; iMonto < 100; iMonto++)
                    //        {
                    //            if (lstQALY.Count > iMonto)
                    //                allSelectQALYMethod.lstQALYLast.Add(Convert.ToSingle(lstQALY[iMonto]));
                    //            else
                    //                allSelectQALYMethod.lstQALYLast.Add(Convert.ToSingle(lstQALY[0]));
                    //        }

                    //    }
                    //}
                }
                int iLstPercentile = 0;// crSelectFunctionCalculateValue.lstLatin.Count;
                MontoCarlo montoCarlo = new MontoCarlo();
                List<int> query = null;
                //-----------为了更加加快速度，只做100次的随机数-------------------------------
                //---modify by xiejp-----直接计算所有的可能，然后分成100的中间值--------------20111215-----暂不考虑随机种子------
                if (crSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile != null && crSelectFunctionCalculateValue.CRCalculateValues.First().LstPercentile.Count > 0)
                {
                     
                       
                    
                }
                List<QALYValueAttribute> lstQALYValueAttribute = new List<QALYValueAttribute>();
                foreach (CRCalculateValue crCalculateValue in crSelectFunctionCalculateValue.CRCalculateValues)
                {
                    QALYValueAttribute qalyValueAttribute = new QALYValueAttribute();
                   
                    if (crCalculateValue.PointEstimate != 0)
                    {
                        //Dictionary<string, double> dicVariable = Configuration.ConfigurationCommonClass.getDicSetupVariableColRow(crCalculateValue.Col, crCalculateValue.Row, lstSetupVariable, CommonClass.GBenMAPGrid.GridDefinitionID, lstGridRelationshipAll);
                        

                        qalyValueAttribute.Col = crCalculateValue.Col;
                        qalyValueAttribute.Row = crCalculateValue.Row;
                       



                        
                    }
                    qalyValueAttribute.Col = crCalculateValue.Col;
                    qalyValueAttribute.Row = crCalculateValue.Row;
                    
                    qalyValueAttribute.PointEstimate = Convert.ToSingle(Math.Round(crCalculateValue.PointEstimate *allSelectQALYMethod. fQALYFirst, 4));
                    qalyValueAttribute.LstPercentile = new List<float>();
                    if (crCalculateValue.LstPercentile != null && crCalculateValue.LstPercentile.Count > 0)
                    {
                        //-------------------------考虑蒙特卡罗算法-------------------------------------
                        //iMontoCarlo = 100 / crCalculateValue.LstPercentile.Count;
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

                                   lstQALYTemp .Add( dMonto * d  );
                                }
                            }
                            //排序
                            lstQALYTemp.Sort();
                           // lstTemp = new List<float>();
                             qalyValueAttribute.LstPercentile=getMedianSample(lstQALYTemp,100);
                             
                                //for (int iMonto = 0; iMonto < 100; iMonto++)
                                //{
                                //    //for(int iTemp=0;iTemp<crCalculateValue.LstPercentile.Count;iTemp++)
                                //    //{

                                //    qalyValueAttribute.LstPercentile.Add(lstQALYTemp.GetRange(iMonto * lstQALYTemp.Count / 100, lstQALYTemp.Count / 100).Median());

                                //    //}

                                //}
                                //qalyValueAttribute.LstPercentile = lstTemp;
                             
                            //只取100个

                        }
                        //--------------------------With Latin Hypercube Points, on the other hand, BenMAP uses a 5,000 draw Monte Carlo approach to estimate QALYs. 
                        //That is, BenMAP draws once from the QALY database and once from the incidence Latin Hypercube, and multiplies the two draws together,
                        //    repeating this process 5,000 times and putting the results into a holding container. Finally, the holding container is sorted low to 
                        //high and binned back down to 100 Latin Hypercube points (representing the 0.5th percentile to the 99.5th percentile of the estimated QALY distribution

                        qalyValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(qalyValueAttribute.LstPercentile);
                        qalyValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(qalyValueAttribute.LstPercentile, qalyValueAttribute.PointEstimate);
                        qalyValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(qalyValueAttribute.LstPercentile, qalyValueAttribute.PointEstimate);


                    }
                    //if (crSelectFunctionCalculateValue.lstLatin != null && crSelectFunctionCalculateValue.lstLatin.Count > 0)
                    //{
                    //    //-------------------------考虑蒙特卡罗算法-------------------------------------
                    //    //iMontoCarlo = 100 / crCalculateValue.LstPercentile.Count;
                    //    if (crCalculateValue.PointEstimate == 0)
                    //    {
                    //        for (int iMonto = 0; iMonto < 100; iMonto++)
                    //        {
                    //            qalyValueAttribute.LstPercentile.Add(0.0);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        foreach (double dMonto in lstQALYMontoCarlo)
                    //        {
                    //            foreach (double d in crSelectFunctionCalculateValue.lstLatin)
                    //            {

                    //                qalyValueAttribute.LstPercentile.Add( dMonto * d *crCalculateValue.PointEstimate );
                    //            }
                    //        }
                    //        //排序
                    //        qalyValueAttribute.LstPercentile.Sort();
                    //        lstTemp = new List<double>();
                    //        if (iMontoCarlo > 0)
                    //        {
                    //            for (int iMonto = 0; iMonto < 100; iMonto++)
                    //            {
                    //                //for(int iTemp=0;iTemp<crCalculateValue.LstPercentile.Count;iTemp++)
                    //                //{

                    //                lstTemp.Add(qalyValueAttribute.LstPercentile[iMonto * iLstPercentile]);

                    //                //}

                    //            }
                    //            qalyValueAttribute.LstPercentile = lstTemp;
                    //        }
                    //        //只取100个

                    //    }
                    //    //--------------------------With Latin Hypercube Points, on the other hand, BenMAP uses a 5,000 draw Monte Carlo approach to estimate QALYs. 
                    //    //That is, BenMAP draws once from the QALY database and once from the incidence Latin Hypercube, and multiplies the two draws together,
                    //    //    repeating this process 5,000 times and putting the results into a holding container. Finally, the holding container is sorted low to 
                    //    //high and binned back down to 100 Latin Hypercube points (representing the 0.5th percentile to the 99.5th percentile of the estimated QALY distribution
                    //}
                    //qalyValueAttribute.Mean = Configuration.ConfigurationCommonClass.getMean(qalyValueAttribute.LstPercentile);
                    //qalyValueAttribute.Variance = Configuration.ConfigurationCommonClass.getVariance(qalyValueAttribute.LstPercentile, qalyValueAttribute.PointEstimate);
                    //qalyValueAttribute.StandardDeviation = Configuration.ConfigurationCommonClass.getStandardDeviation(qalyValueAttribute.LstPercentile, qalyValueAttribute.PointEstimate);
                    //if (allSelectQALYMethodAndValue.lstQALYValueAttributes == null) allSelectQALYMethodAndValue.lstQALYValueAttributes = new List<QALYValueAttribute>();
                    lstQALYValueAttribute.Add(qalyValueAttribute);


                }
                allSelectQALYMethodAndValue.lstQALYValueAttributes = lstQALYValueAttribute;// new List<float[]>();
                //List<float> lstTempDouble = new List<float>();
                //foreach (QALYValueAttribute qa in lstQALYValueAttribute)
                //{
                //    lstTempDouble = new List<float>();
                //    lstTempDouble.Add( qa.Col);
                //    lstTempDouble.Add(qa.Row);
                //    lstTempDouble.Add(qa.PointEstimate  );
                //    lstTempDouble.Add(qa.Mean);
                //    lstTempDouble.Add(qa.Variance);
                //    lstTempDouble.Add(qa.StandardDeviation);
                //    if (qa.LstPercentile != null && qa.LstPercentile.Count() > 0)
                //    {
                //        lstTempDouble.AddRange(qa.LstPercentile);
                //    }

                //    allSelectQALYMethodAndValue.arrayQALYValueAttributes.Add(lstTempDouble.ToArray());
                //}
                //lstQALYValueAttribute = null;
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
                allSelectQALYMethodAndValue.lstQALYValueAttributes =   new List<QALYValueAttribute>();
                Dictionary<int, double> dicWeight = new Dictionary<int, double>();

                //首先得到一个dic
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
                        //List<float> lstDouble = new List<float>();
                        //lstDouble.Add(apvValueAttribute[2]);
                        //if(apvValueAttribute.Length>6)
                        //lstDouble.AddRange(apvValueAttribute.ToList().GetRange(6,apvValueAttribute.Length-6));
                        dicValue.Add(apvValueAttribute.Col + "," + apvValueAttribute.Row, apvValueAttribute);
                    }
                    dicAll.Add(i, dicValue);
                    lstAllColRow = lstAllColRow.Union(dicValue.Keys.ToList()).ToList();
                    i++;
                }
                switch (poolingMethod)
                {

                    case PoolingMethodTypeEnum.SumDependent://简单的加
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
                        //Results are summed assuming that they are independent.  A Monte Carlo simulation is used.  
                        //At each iteration, a random point is chosen from the Latin Hypercube of each result, 
                        //and the sum of these values is put in a holding container.  After some number of iterations, 
                        //the holding container is sorted low to high and binned down to the appropriate number of Latin Hypercube points. 

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
                                    if(iSubtractionDependent==0)
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
                        //-----------如果加起来不等于1，首先变成=1
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
                                                lstLHS[i ] += Convert.ToSingle(k.Value[s].LstPercentile[i] * lstAllSelectQALYMethodAndValue[k.Key].AllSelectQALYMethod.Weight);
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
                    case PoolingMethodTypeEnum.RandomOrFixedEffects://weight 随机数 加起来为1
                        List<int> lstRandom = new List<int>();
                        Random random = new Random();
                        i = 0;
                        while (i < dicAll.Count  )
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
                            //-------------modify by xiejp (1/Variance)/sum(1/Variance)
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
                    case PoolingMethodTypeEnum.FixedEffects://weight 自动为1/count
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
                                            lstLHS = k.Value[s].LstPercentile;//.GetRange(1, k.Value[s].Count - 1);
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
                //allSelectQALYMethodAndValue.arrayQALYValueAttributes = new List<float[]>();
                //List<float> lstTempDouble = new List<float>();
                //foreach (QALYValueAttribute qa in lstQALYValueAttributes)
                //{
                //    lstTempDouble = new List<float>();
                //    lstTempDouble.Add(qa.Col);
                //    lstTempDouble.Add(qa.Row);
                //    lstTempDouble.Add(qa.PointEstimate);
                //    lstTempDouble.Add(qa.Mean);
                //    lstTempDouble.Add(qa.Variance);
                //    lstTempDouble.Add(qa.StandardDeviation);
                //    if (qa.LstPercentile != null && qa.LstPercentile.Count() > 0)
                //    {
                //        lstTempDouble.AddRange(qa.LstPercentile);
                //    }

                //    allSelectQALYMethodAndValue.arrayQALYValueAttributes.Add(lstTempDouble.ToArray());
                //}
                //lstQALYValueAttributes = null;
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
               // DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
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
                string commandText = string.Format("  select  a.QALYDataSetID,SetupID,QALYDatasetName,EndPointGroup,EndPoint,Qualifier,Description,StartAge,EndAge "+
" from QALYDataSets a,(select distinct QALYDataSetID,StartAge,EndAge from QALYEntries) b where a.QALYDataSetID=b.QALYDataSetID and a.SetupID={0} and EndPointGroup='{1}'   ",CommonClass.MainSetup.SetupID, EndPointGroup);

                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    BenMAPQALY bvf = new BenMAPQALY();
                    bvf.Description = Convert.ToString(dr["Description"]);
                    bvf.EndAge = Convert.ToInt32(dr["EndAge"]);
                    bvf.EndPoint = dr["EndPoint"].ToString();
                    bvf.EndPointGroup = dr["EndPointGroup"].ToString();// Convert.ToDouble(dr["P1A"]);
                    bvf.QalyDatasetID = Convert.ToInt32(dr["QalyDatasetID"]);
                    bvf.QalyDatasetName = Convert.ToString(dr["QalyDatasetName"]);
                    bvf.Qualifier = Convert.ToString(dr["Qualifier"]);// Convert.ToDouble(dr["B"]);
                    bvf.SetupID = Convert.ToInt32(dr["SetupID"]);
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
