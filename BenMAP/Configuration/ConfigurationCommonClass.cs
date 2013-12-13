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
//using Troschuetz.Random;
using System.Diagnostics;
using System.Xml.Serialization;
using ProtoBuf;
using System.Reflection;
//using Polenter.Serialization;
//using AltSerialize;

namespace BenMAP.Configuration
{
   public class ConfigurationCommonClass
    {
       /// <summary>
       /// 清除拉丁立体方
       /// </summary>
       /// <param name="cRSelectFunctionCalculateValue"></param>
       public static void ClearCRSelectFunctionCalculateValueLHS(ref CRSelectFunctionCalculateValue cRSelectFunctionCalculateValue)
       {
           //if (cRSelectFunctionCalculateValue.lstLatin != null && cRSelectFunctionCalculateValue.lstLatin.Count > 0)
           //{
           //    foreach (CRCalculateValue crv in cRSelectFunctionCalculateValue.CRCalculateValues)
           //    {
           //        crv.LstPercentile = null;
           //    }
           //}
 
       }
       /// <summary>
       /// 重新生成拉丁立体方
       /// </summary>
       /// <param name="cRSelectFunctionCalculateValue"></param>
       public static void UpdateCRSelectFunctionCalculateValueLHS(ref CRSelectFunctionCalculateValue cRSelectFunctionCalculateValue)
       {
           //if (cRSelectFunctionCalculateValue.lstLatin != null && cRSelectFunctionCalculateValue.lstLatin.Count > 0)
           //{
           //    foreach (CRCalculateValue crv in cRSelectFunctionCalculateValue.CRCalculateValues)
           //    {
           //        crv.LstPercentile=new List<double>();
           //        foreach (double d in cRSelectFunctionCalculateValue.lstLatin)
           //        {
           //            crv.LstPercentile.Add(crv.PointEstimate * d);
           //        }
           //        //Mean
           //        crv.Mean = getMean(crv.LstPercentile);
           //        //标准差
           //        crv.StandardDeviation = getStandardDeviation(crv.LstPercentile, crv.PointEstimate);
           //        //方差
           //        crv.Variance = getVariance(crv.LstPercentile, crv.PointEstimate);

           //    }
           //}
 
       }

       ///// <summary>
       ///// 重新生成拉丁立体方
       ///// </summary>
       ///// <param name="allSelectValuationMethodAndValue"></param>
       //public static void UpdateAllSelectValuationMethodAndValueLHS(ref AllSelectValuationMethodAndValue allSelectValuationMethodAndValue)
       //{
       //    if (allSelectValuationMethodAndValue.AllSelectValuationMethod. != null && allSelectValuationMethodAndValue.lstLatin.Count > 0)
       //    {
       //        foreach (CRCalculateValue crv in allSelectValuationMethodAndValue.CRCalculateValues)
       //        {
       //            crv.LstPercentile = new List<double>();
       //            foreach (double d in allSelectValuationMethodAndValue.lstLatin)
       //            {
       //                crv.LstPercentile.Add(crv.PointEstimate * d);
       //            }
       //            //Mean
       //            crv.Mean = getMean(crv.LstPercentile);
       //            //标准差
       //            crv.StandardDeviation = getStandardDeviation(crv.LstPercentile, crv.PointEstimate);
       //            //方差
       //            crv.Variance = getVariance(crv.LstPercentile, crv.PointEstimate);

       //        }
       //    }

       //}
      

       /// <summary>
       /// 从文件中获取List CRSelectFunctionCalculateValue
       /// </summary>
       /// <param name="lstCRSelectFuntion"></param>
       /// <param name="strCRFPath"></param>
       public static void SaveCRFRFile(BaseControlCRSelectFunctionCalculateValue baseControlCRSelectFunctionCalculateValue, string strCRFPath)
       {

           try
           {
               ////----------清理valuationMethodPoolingAndAggregation
               //XmlSerializer SerializerObj = new XmlSerializer(typeof(BaseControlCRSelectFunctionCalculateValue),
               //                    new Type[]{typeof(GridTypeEnum),typeof(RegularGrid),typeof(ShapefileGrid), 
               //                        typeof(ModelAttribute),  typeof(List<float[]>),typeof(ModelResultAttribute),typeof(CRSelectFunctionCalculateValue),typeof(BenMAPPopulation),typeof(BenMAPGrid),     
               //                        typeof(ObservationtypeEnum),typeof(BenMAPPollutant),typeof(SeasonalMetric),typeof(Season),typeof(Metric),
               //                        typeof(FixedWindowMetric),typeof(MovingWindowMetric),typeof(CustomerMetric),typeof(BenMAPSetup),
               //typeof(BenMAPPollutant),typeof(BenMAPLine),typeof(ModelDataLine),typeof(MonitorDataLine),typeof(InterpolationMethodEnum),
               //typeof(MonitorAdvance),typeof(WeightingApproachEnum),typeof(MonitorAdvanceDataTypeEnum),typeof(RowCol),          
               //typeof(RollbackType),typeof(RollbackMethod),typeof(BenMAPRollback),typeof(PercentageRollback),                   
               //typeof(IncrementalRollback),typeof(StandardRollback),typeof(Metric),typeof(SeasonalMetric),typeof(MetricStatic), 
               //typeof(MonitorModelRollbackLine),typeof(Monitor),typeof(MonitorValue),typeof(Location),typeof(PopulationAttribute),
               //typeof(RegionTypeGrid),typeof(GridRelationshipAttribute),typeof(IncidenceRateAttribute),typeof(BaseControlGroup),typeof(SetupVariableValues),
               //typeof(SetupVariableJoinAllValues),typeof(BenMAPHealthImpactFunction),typeof(CRSelectFunction),                  
               //typeof(CRSelectFunctionCalculateValue),typeof(CRCalculateValue),typeof(BaseControlCRSelectFunction),             
               //typeof(BaseControlCRSelectFunctionCalculateValue),typeof(BenMAPQALY),typeof(AllSelectQALYMethod),                
               //typeof(QALYValueAttribute),typeof(AllSelectQALYMethodAndValue),typeof(AllSelectValuationMethod),                 
               //typeof(BenMAPValuationFunction),typeof(AllSelectCRFunction),typeof(AllSelectValuationMethodAndValue),            
               //typeof(PoolingMethodTypeEnum),typeof(IPAdvancePoolingMethodEnum),typeof(IncidencePoolingAndAggregationAdvance),  
               //typeof(IncidencePoolingAndAggregation),typeof(APVValueAttribute),typeof(BenMAPValuationFunction),                
               //typeof(ValuationMethodPoolingAndAggregation),typeof(ValuationMethodPoolingAndAggregationBase),typeof(ChartResult)
               //});

               //                // Create a new file stream to write the serialized object to a file
               //                TextWriter WriteFileStream = new StreamWriter(strCRFPath);//@"D:\test.xml");
               //                SerializerObj.Serialize(WriteFileStream, baseControlCRSelectFunctionCalculateValue);

               //                // Cleanup
               //                WriteFileStream.Close();

               //                // Create a new file stream for reading the XML file
               //               // FileStream ReadFileStream = new FileStream(strCRFPath, FileMode.Open, FileAccess.Read, FileShare.Read);// new FileStream(@"D:\test.xml", FileMode.Open, FileAccess.Read, FileShare.Read);

               //                return;
               //GC.Collect();
               if (File.Exists(strCRFPath))
                   File.Delete(strCRFPath);

               using (FileStream fs = new FileStream(strCRFPath, FileMode.OpenOrCreate))
               {
                   try
                   {
                       baseControlCRSelectFunctionCalculateValue.Version = "BenMAP-CE " + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);
                       //baseControlCRSelectFunctionCalculateValue.RBenMapGrid = CommonClass.rBenMAPGrid;
                       //baseControlCRSelectFunctionCalculateValue.BaseControlGroup[0].GridType = CommonClass.GBenMAPGrid;
                       //Serializer.Serialize<List<BaseControlGroup>>(fs, baseControlCRSelectFunctionCalculateValue.BaseControlGroup);
                       Serializer.Serialize<BaseControlCRSelectFunctionCalculateValue>(fs, baseControlCRSelectFunctionCalculateValue);
                       //fs.Flush();
                       //fs.Position = 0;

                       //TestObject obj2 = Serializer.Deserialize<TestObject>(fs);
                       //Console.WriteLine(obj2);  
                       //fs.Close();
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
                   //crrcopy.lstLatin = crr.lstLatin;
                   //crrcopy.ResultCopy = new List<float[]>();
                   //foreach (CRCalculateValue crCalculateValue in crr.CRCalculateValues)
                   //{
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
                   //    lstd.AddRange(crCalculateValue.LstPercentile);
                   //    crrcopy.ResultCopy.Add(lstd.ToArray());
                   //}
                   // crSelectFunctionCalculateValue.ResultCopy.Add(lstd.ToArray());
                   //crrcopy.ResultCopy=crr.ResultCopy;
                   copy.lstCRSelectFunctionCalculateValue.Add(crrcopy);
               }
               //List<Type> knownTypes = new List<Type>();
               //knownTypes.Add(typeof(List<CustomerMetric>));
               //knownTypes.Add(typeof(List<FixedWindowMetric>));
               //knownTypes.Add(typeof(List<MovingWindowMetric>));
               //knownTypes.Add(typeof(ModelDataLine));
               //knownTypes.Add(typeof(MonitorDataLine));
               //knownTypes.Add(typeof(MonitorModelRelativeLine));
               //knownTypes.Add(typeof(MonitorModelRollbackLine));
               //knownTypes.Add(typeof(ShapefileGrid));
               //knownTypes.Add(typeof(RegularGrid));
               //knownTypes.Add(typeof(CRSelectFunction));
               //knownTypes.Add(typeof(BenMAPHealthImpactFunction));
               //knownTypes.Add(typeof(List<CRCalculateValue>));
               //var serializer = new DataContractJsonSerializer(typeof(BaseControlCRSelectFunctionCalculateValue), knownTypes);
               //MemoryStream stream = new MemoryStream();

               GC.Collect();
               if (File.Exists(strCRFPath))
                   File.Delete(strCRFPath);

               

               //return;
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
               //serializer.WriteObject(stream, copy);

               //FileStream fs = new FileStream(strCRFPath, FileMode.OpenOrCreate);
               ////BinaryWriter w = new BinaryWriter(fs);
               ////w.Write(stream.ToArray());//dataBytes);
               ////w.Close();
               //fs.Write(stream.ToArray(), 0, stream.ToArray().Count());
               //copy = null;
               //stream.Close();
               //stream.Dispose();
              
               GC.Collect();

           }
           catch (Exception ex)
           {
               Logger.LogError(ex);
               // return null;
           }
       }
       /// <summary>
       /// load CR的结果文件生成对象
       /// </summary>
       /// <param name="strCFGRPath"></param>
       /// <returns></returns>
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

                   //Serializer.Serialize<List<BaseControlGroup>>(fs, baseControlCRSelectFunctionCalculateValue.BaseControlGroup);
                   //Serializer.Serialize<BaseControlCRSelectFunctionCalculateValue>(fs, baseControlCRSelectFunctionCalculateValue);
                   //fs.Flush();
                   //fs.Position = 0;

                   //TestObject obj2 = Serializer.Deserialize<TestObject>(fs);
                   //Console.WriteLine(obj2);  
                   fs.Close();
                   fs.Dispose();
                   //----------为了不要混淆RBenMAPGrid直接置空
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
              // List<Type> knownTypes = new List<Type>();
              // knownTypes.Add(typeof(List<CustomerMetric>));
              // knownTypes.Add(typeof(List<FixedWindowMetric>));
              // knownTypes.Add(typeof(List<MovingWindowMetric>));
              // knownTypes.Add(typeof(ModelDataLine));
              // knownTypes.Add(typeof(MonitorDataLine));
              // knownTypes.Add(typeof(MonitorModelRelativeLine));
              // knownTypes.Add(typeof(MonitorModelRollbackLine));
              // knownTypes.Add(typeof(ShapefileGrid));
              // knownTypes.Add(typeof(RegularGrid));
              // knownTypes.Add(typeof(CRSelectFunction));
              // knownTypes.Add(typeof(BenMAPHealthImpactFunction));
              // knownTypes.Add(typeof(List<CRCalculateValue>));
              // var serializer = new DataContractJsonSerializer(typeof(List<CRSelectFunctionCalculateValue>), knownTypes);
              // FileStream fs = new FileStream(strCRFRPath, FileMode.Open);
              // // BinaryReader brtest = new BinaryReader(fstest);
              // byte[] data = new byte[fs.Length];
              // fs.Read(data, 0, data.Length);
              //// var serializertest = new DataContractJsonSerializer(typeof(BenMAPLine), knownTypes);
              // var mStream = new MemoryStream(data);
               //BinaryReader br = new BinaryReader(fs);
               //MemoryStream memoryFile = GridCommon.DecryptToMemoryStream(strFile, "&%#@?,:*");
               BaseControlCRSelectFunctionCalculateValue baseControlCRSelectFunctionCalculateValue = null;// (BaseControlCRSelectFunctionCalculateValue)serializer.ReadObject(mStream);
               try
               {
                   using (FileStream fs = new FileStream(strCFGRPath, FileMode.Open))
                   {
                       BinaryFormatter formatter = new BinaryFormatter();
                       baseControlCRSelectFunctionCalculateValue = (BaseControlCRSelectFunctionCalculateValue)formatter.Deserialize(fs);//在这里大家要注意咯,他的返回值是object
                       fs.Close();
                       fs.Dispose();
                       formatter = null;
                       //GC.Collect();
                   }
               
               //------------update数据-------------------------
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
                 //  MessageBox.Show(ex.Message);
               }
               for (int i=0; i < baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Count; i++)
               {
                   CRSelectFunctionCalculateValue crclv = baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[i];
                   getCalculateValueFromResultCopy(ref crclv);
                   //crclv.ResultCopy = null;
                   baseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue[i] = crclv;
 
               }
               GC.Collect();
                

               //mStream.Close();
               //fs.Close();
               return baseControlCRSelectFunctionCalculateValue;
           }
           catch (Exception ex)
           {
               Logger.LogError(ex);
               
               return null;
           }

       }
       /// <summary>
       /// save cfgx文件
       /// </summary>
       /// <param name="baseControlCRSelectFunction"></param>
       /// <param name="strFile"></param>
       public static void SaveCFGFile(BaseControlCRSelectFunction baseControlCRSelectFunction,string strFile)
       {
           try
           {
               if (File.Exists(strFile))
                   File.Delete(strFile);
               //foreach (BaseControlGroup bcg in baseControlCRSelectFunction.BaseControlGroup)
               //{
               //    if (bcg.Base is MonitorDataLine)
               //    {
               //        if ((bcg.Base as MonitorDataLine).MonitorNeighbors != null)
               //            (bcg.Base as MonitorDataLine).MonitorNeighbors.Clear();
               //    }
               //    if (bcg.Control is MonitorDataLine)
               //    {
               //        if ((bcg.Control as MonitorDataLine).MonitorNeighbors != null)
               //            (bcg.Control as MonitorDataLine).MonitorNeighbors.Clear();
               //    }
               //}
               //GC.Collect();
               using (FileStream fs = new FileStream(strFile, FileMode.OpenOrCreate))
               {
                   baseControlCRSelectFunction.Version = "BenMAP-CE " + Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, Assembly.GetExecutingAssembly().GetName().Version.ToString().Count() - 2);
                   Serializer.Serialize<BaseControlCRSelectFunction>(fs, baseControlCRSelectFunction);
                   //fs.Flush();
                   //fs.Position = 0;

                   //TestObject obj2 = Serializer.Deserialize<TestObject>(fs);
                   //Console.WriteLine(obj2);  
                   fs.Close();
                   fs.Dispose();
               }
               //FileStream fs = new FileStream(strFile, FileMode.OpenOrCreate);
               //fs.Write(stream.ToArray(), 0, stream.ToArray().Count());
               //fs.Close();
               //copy = null;
               //stream.Dispose();
               //stream.Close();
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
                
               //List<Type> knownTypes = new List<Type>();
               //knownTypes.Add(typeof(List<CustomerMetric>));
               //knownTypes.Add(typeof(List<FixedWindowMetric>));
               //knownTypes.Add(typeof(List<MovingWindowMetric>));
               //knownTypes.Add(typeof(ModelDataLine));
               //knownTypes.Add(typeof(MonitorDataLine));
               //knownTypes.Add(typeof(MonitorModelRelativeLine));
               //knownTypes.Add(typeof(MonitorModelRollbackLine));
               //knownTypes.Add(typeof(ShapefileGrid));
               //knownTypes.Add(typeof(RegularGrid));
               //knownTypes.Add(typeof(CRSelectFunction));
               //knownTypes.Add(typeof(BenMAPHealthImpactFunction));
               //knownTypes.Add(typeof(BaseControlCRSelectFunction));
               //var serializer = new DataContractJsonSerializer(typeof(BaseControlCRSelectFunction), knownTypes);
               //MemoryStream stream = new MemoryStream();

               //serializer.WriteObject(stream, copy);
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
               //FileStream fs = new FileStream(strFile, FileMode.OpenOrCreate);
               //fs.Write(stream.ToArray(), 0, stream.ToArray().Count());
               //fs.Close();
               //copy = null;
               //stream.Dispose();
               //stream.Close();
               GC.Collect();
               

           }
           catch (Exception ex)
           {
               Logger.LogError(ex);
               // return null;
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
                   //Serializer.Serialize<List<BaseControlGroup>>(fs, baseControlCRSelectFunctionCalculateValue.BaseControlGroup);
                   //Serializer.Serialize<BaseControlCRSelectFunctionCalculateValue>(fs, baseControlCRSelectFunctionCalculateValue);
                   //fs.Flush();
                   //fs.Position = 0;

                   //TestObject obj2 = Serializer.Deserialize<TestObject>(fs);
                   //Console.WriteLine(obj2);  
                   fs.Close();
                   fs.Dispose();
                   //----------为了不要混淆RBenMAPGrid直接置空
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
               //List<Type> knownTypes = new List<Type>();
               //knownTypes.Add(typeof(List<CustomerMetric>));
               //knownTypes.Add(typeof(List<FixedWindowMetric>));
               //knownTypes.Add(typeof(List<MovingWindowMetric>));
               //knownTypes.Add(typeof(ModelDataLine));
               //knownTypes.Add(typeof(MonitorDataLine));
               //knownTypes.Add(typeof(MonitorModelRelativeLine));
               //knownTypes.Add(typeof(MonitorModelRollbackLine));
               //knownTypes.Add(typeof(ShapefileGrid));
               //knownTypes.Add(typeof(RegularGrid));
               //knownTypes.Add(typeof(CRSelectFunction));
               //knownTypes.Add(typeof(BenMAPHealthImpactFunction));
               //knownTypes.Add(typeof(List<CRCalculateValue>));
               //var serializer = new DataContractJsonSerializer(typeof(BaseControlCRSelectFunction), knownTypes);
               //FileStream fs = new FileStream(strFile, FileMode.Open);
               //// BinaryReader brtest = new BinaryReader(fstest);
               //byte[] data = new byte[fs.Length];
               //fs.Read(data, 0, data.Length);
               //// var serializertest = new DataContractJsonSerializer(typeof(BenMAPLine), knownTypes);
               //var mStream = new MemoryStream(data);
               ////BinaryReader br = new BinaryReader(fs);
               ////MemoryStream memoryFile = GridCommon.DecryptToMemoryStream(strFile, "&%#@?,:*");
               ////List<CRSelectFunctionCalculateValue> lstCRSelectFunctionCalculateValue = (List<CRSelectFunctionCalculateValue>)serializer.ReadObject(mStream);
               //baseControlCRSelectFunction = (BaseControlCRSelectFunction)serializer.ReadObject(mStream);
               using (FileStream fs = new FileStream(strFile, FileMode.Open))
               {
                   BinaryFormatter formatter = new BinaryFormatter();
                   baseControlCRSelectFunction = (BaseControlCRSelectFunction)formatter.Deserialize(fs);//在这里大家要注意咯,他的返回值是object
                   fs.Close();
                   fs.Dispose();
                   formatter = null;
                   GC.Collect();
               }
               foreach (BaseControlGroup bcg in baseControlCRSelectFunction.BaseControlGroup)
               {
                   DataSourceCommonClass.getModelValuesFromResultCopy(ref bcg.Base);
                   DataSourceCommonClass.getModelValuesFromResultCopy(ref bcg.Control);
               }
               //mStream.Close();
               //fs.Close();
               
 
           }
           catch (Exception ex)
           { 
           }
           return baseControlCRSelectFunction;
       }

       /// <summary>
       /// 得到所有Race种族
       /// </summary>
       /// <returns></returns>
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
       /// <summary>
       /// 得到所有Ethnicity-宗教信仰
       /// </summary>
       /// <returns></returns>
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
       /// <summary>
       /// 得到所有Gender;性别
       /// </summary>
       /// <returns></returns>
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
               string commandText =string.Format( "select IncidenceDataSetID,IncidenceDataSetName from IncidenceDataSets where SetupID={0} ",SetupID);
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
           //--------------------------修改数据库 add FIPS表
           return null;
 
       }
       /// <summary>
       /// 从ID中获取某个HealthImactFunction
       /// </summary>
       /// <param name="ID"></param>
       /// <returns></returns>
       public static BenMAPHealthImpactFunction getBenMAPHealthImpactFunctionFromID(int ID)
       {
           try
           {
               string commandText = string.Format("select CRFunctionID,a.CRFunctionDatasetID,f.CRFunctionDataSetName,a.EndpointGroupID,b.EndPointGroupName,a.EndpointID,c.EndPointName,PollutantID,"
    + " MetricID,SeasonalMetricID,MetricStatistic,Author,YYear,Location,OtherPollutants,Qualifier,Reference,Race,Gender,Startage,Endage,a.FunctionalFormid,d.FunctionalFormText,"
    + " a.IncidenceDatasetID,a.PrevalenceDatasetID,a.VariableDatasetID,Beta,DistBeta,P1Beta,P2Beta,A,NameA,B,NameB,C,NameC,a.BaselineFunctionalFormID,"
    + " e.FunctionalFormText as BaselineFunctionalFormText,Ethnicity,Percentile,Locationtypeid, g.IncidenceDataSetName,i.IncidenceDataSetName as PrevalenceDataSetName,"
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
               benMapHealthImpactFunction.Pollutant = Grid.GridCommon.getPollutantFromID(Convert.ToInt32(dr["PollutantID"]),CommonClass.lstPollutantAll);
               benMapHealthImpactFunction.Metric = Grid.GridCommon.getMetricFromPollutantAndID(benMapHealthImpactFunction.Pollutant ,Convert.ToInt32(dr["MetricID"]));
               benMapHealthImpactFunction.SeasonalMetric = null;
               if ((dr["SeasonalMetricID"] is DBNull) == false)
               {
                   benMapHealthImpactFunction.SeasonalMetric = Grid.GridCommon.getSeasonalMetricFromPollutantAndID(benMapHealthImpactFunction.Pollutant ,Convert.ToInt32(dr["SeasonalMetricID"]));
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
               return benMapHealthImpactFunction;
           }
           catch (Exception ex)
           {
               return null;
           }

       }
       /// <summary>
       /// 获取拉丁立方体采样
       /// </summary>
       /// <param name="LatinHypercubePoints"></param>
       /// <returns></returns>
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
                   lhsResultArray[ilhsResult] = lhsResult[0, ilhsResult] + 0.5;//让数据在0.5-1.5的范围内------------可能需要修正
                   ilhsResult++;
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
       public static double Normal(double x, double miu, double sigma) //正态分布概率密度函数
       {
           return 1.0 / (x * Math.Sqrt(2 * Math.PI) * sigma) * Math.Exp(-1 * (Math.Log(x) - miu) * (Math.Log(x) - miu) / (2 * sigma * sigma));
       }
               public static double triangular(double  Min,double  Mode,double  Max)
        {                        
            //   Declarations        
                 double  R=0.0;                                   
            //   Initialise     
            Random r = new Random();
             R = r.NextDouble();            
            //    Triangular                        
            if ( R == (( Mode -  Min) / ( Max -  Min))) {
              return  Mode;
            }
            else if ( R < (( Mode -  Min) / ( Max -  Min))) {
                return  Min + Math.Sqrt( R * ( Max -  Min) * ( Mode -  Min));
            }
            else {
              return  Max - Math.Sqrt((1 -  R) * ( Max -  Min) * ( Max -  Mode));
            }
        }
        public static double[] simulate(int Total, double[] Tmin, double[] Tmod, double[] Tmax)
        {
            // Declarations
                int mlngEvals=10000;
               int i=0, i1=0, i2=0;             
               double[]   TMin = new double[Total];
               double[]  TMod = new double[Total];
               double[]  TMax = new double[Total];
               double[] mlngResults = new double[Total];
               double  Time=0.0;
               long lngWinner=0;
               double  Winner=0;               
            // Initialise            
                 for (i=0; i<Total; i++) {                 
            //     distribution parameters
                    TMin[i]=Tmin[i];
                    TMod[i]=Tmod[i];
                    TMax[i]=Tmax[i];              
            //     Results Array              
                   mlngResults[i]=0; 
                 } 
            // The Tournament           
                 for (i1=1; i1<=mlngEvals; i1++) {         
            //     Seed               
                   lngWinner = 0;
                    Winner = triangular( TMin[0],  TMod[0],  TMax[0]);
            //     And the Rest
                     for (i2=1; i2<Total; i2++) {
                        Time = triangular( TMin[i2],  TMod[i2],  TMax[i2]);
                         if ( Time <  Winner) {
                            Winner =  Time;
                           lngWinner = i2;
                         }
                     }
            //     Bin
                   mlngResults[lngWinner]++;
               }
               return mlngResults;
        }
    
        /// <summary>
       /// 获取拉丁立方体采样
       /// </summary>
       /// <param name="LatinHypercubePoints"></param>
       /// <returns></returns>
        //public static double[] getLHSArrayCRFunction(int LatinHypercubePoints, CRSelectFunction crSelectFunction)
        //{
        //    try
        //    {
        //         List<int> lstInt = new List<int>();
        //         for (int i = 0; i < LatinHypercubePoints; i++)
        //         {
        //             //lstInt.Add((100 * (i+1)) / LatinHypercubePoints);
        //             lstInt.Add(Convert.ToInt16(Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(LatinHypercubePoints) - (100.00 / (2 * Convert.ToDouble(LatinHypercubePoints)))));
        //         }
        //        Distribution distribution = null;// (Distribution)this.currentDistribution;
        //        double[] lhsResultArray = new double[LatinHypercubePoints];
        //        switch (crSelectFunction.BenMAPHealthImpactFunction.BetaDistribution)
        //        {
        //            case "None"://还是Beta
        //                for (int i = 0; i < LatinHypercubePoints; i++)
        //                {
        //                    lhsResultArray[i] = crSelectFunction.BenMAPHealthImpactFunction.Beta;

        //                }
        //                return lhsResultArray;
        //                break;
        //            case "Normal":
        //                distribution = new NormalDistribution()
        //                {
        //                    Mu = crSelectFunction.BenMAPHealthImpactFunction.Beta,
        //                    Sigma = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1

        //                };
        //                break;
        //            case "Triangular":
        //                distribution = new TriangularDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Beta = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2, Gamma = crSelectFunction.BenMAPHealthImpactFunction.Beta };
        //                break;
        //            case "Poisson":
        //                distribution = new PoissonDistribution() { Lambda = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1 };
        //                break;
        //            case "Binomial":
        //                distribution = new BinomialDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Beta = Convert.ToInt32(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2) };
        //                break;
        //            case "LogNormal":
        //                distribution = new LognormalDistribution() { Mu = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Sigma = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2 };
        //                break;
        //            case "Uniform":
        //                distribution = new ContinuousUniformDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Beta = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2 };
        //                break;
        //            case "Exponential":
        //                distribution = new ExponentialDistribution() { Lambda = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1 };
        //                break;
        //            case "Geometric":
        //                distribution = new GeometricDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1 };
        //                break;
        //            case "Weibull":
        //                distribution = new WeibullDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Lambda = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2 };
        //                break;
        //            case "Gamma":
        //                distribution = new GammaDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Theta = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2 };
        //                break;
        //            case "Logistic":
        //                //distribution=new  Troschuetz.Random.
        //                Meta.Numerics.Statistics.Distributions.Distribution logistic_distribution = new Meta.Numerics.Statistics.Distributions.LogisticDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
        //                Meta.Numerics.Statistics.Sample logistic_sample = CreateSample(logistic_distribution, 1000000);
        //                List<double> lstlogistic = logistic_sample.ToList();
        //                lstlogistic.Sort();

        //                for (int i = 0; i < LatinHypercubePoints; i++)
        //                {
        //                    lhsResultArray[i] = lstlogistic.GetRange(i * (lstlogistic.Count / LatinHypercubePoints), (lstlogistic.Count / LatinHypercubePoints)).Median();
        //                    //lhsResultArray[LatinHypercubePoints - i-1] = lstsamples[(LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints) > lstsamples.Count-1 ? lstsamples.Count-1 : (LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints)];
        //                    //i++;
        //                }
        //                return lhsResultArray;
        //                break;
        //            case "Beta":
        //                distribution = new BetaDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Beta = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2 };
        //                break;
        //            case "Pareto":
        //                distribution = new ParetoDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Beta = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2 };
        //                break;
        //            case "Cauchy":
        //                distribution = new CauchyDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Gamma = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2 };
        //                break;
        //            case "Custom":
        //                //distribution=new CustomDistributionEntries(){ 
        //                //------------首先得到Custom的实例---------------然后计算均值-------------------
        //                string commandText = string.Format("select   VValue  from CRFunctionCustomEntries where CRFunctionID={0} order by vvalue", crSelectFunction.BenMAPHealthImpactFunction.ID);
        //                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

        //                DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
        //                List<double> lstCustom = new List<double>();
        //                foreach (DataRow dr in ds.Tables[0].Rows)
        //                {
        //                    lstCustom.Add(Convert.ToDouble(dr[0]));

        //                }
        //                lstCustom.Sort();
        //                //List<int> lstInt = new List<int>();
        //                for (int i = 0; i < LatinHypercubePoints; i++)
        //                {
        //                    //lstInt.Add(100 * i / LatinHypercubePoints);
        //                    lhsResultArray[i] = lstCustom.GetRange(i * (lstCustom.Count / LatinHypercubePoints), (lstCustom.Count / LatinHypercubePoints)).Median();
        //                    //lhsResultArray[LatinHypercubePoints - i-1] = lstsamples[(LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints) > lstsamples.Count-1 ? lstsamples.Count-1 : (LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints)];
        //                    //i++;
        //                }
        //                //lhsResultArray = Extreme.Statistics.Stats.Percentiles(lstCustom.ToArray(), lstInt.ToArray());
        //                return lhsResultArray;
        //                break;


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

        public static double[] getLHSArrayCRFunctionSeed(int LatinHypercubePoints, CRSelectFunction crSelectFunction,int Seed)
        {
            try
            {
                List<int> lstInt = new List<int>();
                for (int i = 0; i < LatinHypercubePoints; i++)
                {
                    //lstInt.Add((100 * (i+1)) / LatinHypercubePoints);
                    lstInt.Add(Convert.ToInt16(Convert.ToDouble(i + 1) * 100.00 / Convert.ToDouble(LatinHypercubePoints) - (100.00 / (2 * Convert.ToDouble(LatinHypercubePoints)))));
                }
                //Distribution distribution = null;// (Distribution)this.currentDistribution;
                double[] lhsResultArray = new double[LatinHypercubePoints];
                Meta.Numerics.Statistics.Sample sample = null;
                switch (crSelectFunction.BenMAPHealthImpactFunction.BetaDistribution)
                {
                    case "None"://还是Beta
                        for (int i = 0; i < LatinHypercubePoints; i++)
                        {
                            lhsResultArray[i] = crSelectFunction.BenMAPHealthImpactFunction.Beta;

                        }
                        return lhsResultArray;
                        break;
                    case "Normal":
                        //distribution = new NormalDistribution()
                        //{
                        //    Mu = crSelectFunction.BenMAPHealthImpactFunction.Beta,
                        //    Sigma = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1

                        //};
                        Meta.Numerics.Statistics.Distributions.Distribution Normal_distribution =
                            new Meta.Numerics.Statistics.Distributions.NormalDistribution(crSelectFunction.BenMAPHealthImpactFunction.Beta, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1);
                        sample = CreateSample(Normal_distribution, 1000000, Seed);
                        break;
                    case "Triangular":
                        //distribution = new TriangularDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Beta = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2, Gamma = crSelectFunction.BenMAPHealthImpactFunction.Beta };
                        Meta.Numerics.Statistics.Distributions.Distribution Triangular_distribution =
                            new Meta.Numerics.Statistics.Distributions.TriangularDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2, crSelectFunction.BenMAPHealthImpactFunction.Beta);
                        sample = CreateSample(Triangular_distribution, 1000000, Seed);
                        break;
                    case "Poisson":
                        //distribution = new PoissonDistribution() { Lambda = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1 };
                        Meta.Numerics.Statistics.Distributions.PoissonDistribution Poisson_distribution =
                            new Meta.Numerics.Statistics.Distributions.PoissonDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1);
                        sample = CreateSample(Poisson_distribution, 1000000, Seed);
                        break;
                    case "Binomial":
                        //distribution = new BinomialDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Beta = Convert.ToInt32(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2) };
                        Meta.Numerics.Statistics.Distributions.BinomialDistribution Binomial_distribution =
                            new Meta.Numerics.Statistics.Distributions.BinomialDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Convert.ToInt32(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2));
                        sample = CreateSample(Binomial_distribution, 1000000, Seed);
                        break;
                    case "LogNormal":
                        //distribution = new LognormalDistribution() { Mu = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Sigma = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2 };
                        Meta.Numerics.Statistics.Distributions.LognormalDistribution Lognormal_distribution =
                            new Meta.Numerics.Statistics.Distributions.LognormalDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
                        sample = CreateSample(Lognormal_distribution, 1000000, Seed);
                        break;
                    case "Uniform":
                        //distribution = new ContinuousUniformDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Beta = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2 };
                        Interval interval= Interval.FromEndpoints(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1,
                            crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
                        
                        Meta.Numerics.Statistics.Distributions.UniformDistribution Uniform_distribution =
                            new Meta.Numerics.Statistics.Distributions.UniformDistribution(interval );//crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
                        //Uniform_distribution.
                        sample = CreateSample(Uniform_distribution, 1000000, Seed);
                        break;
                    case "Exponential":
                        //distribution = new ExponentialDistribution() { Lambda = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1 };
                        Meta.Numerics.Statistics.Distributions.ExponentialDistribution Exponential_distribution =
                            new Meta.Numerics.Statistics.Distributions.ExponentialDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1);
                        sample = CreateSample(Exponential_distribution, 1000000, Seed);
                        break;
                    case "Geometric":
                        //distribution = new GeometricDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1 };
                        Meta.Numerics.Statistics.Distributions.ExponentialDistribution Geometric_distribution =
                            new Meta.Numerics.Statistics.Distributions.ExponentialDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1);
                        sample = CreateSample(Geometric_distribution, 1000000, Seed);
                        break;
                    case "Weibull":
                        //distribution = new WeibullDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Lambda = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2 };
                        Meta.Numerics.Statistics.Distributions.WeibullDistribution Weibull_distribution =
                            new Meta.Numerics.Statistics.Distributions.WeibullDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
                        sample = CreateSample(Weibull_distribution, 1000000, Seed);
                        break;
                    case "Gamma":
                        //distribution = new GammaDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Theta = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2 };
                        Meta.Numerics.Statistics.Distributions.GammaDistribution Gamma_distribution =
                            new Meta.Numerics.Statistics.Distributions.GammaDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
                        sample = CreateSample(Gamma_distribution, 1000000, Seed);
                        break;
                    case "Logistic":
                        //distribution=new  Troschuetz.Random.
                        Meta.Numerics.Statistics.Distributions.Distribution logistic_distribution = new Meta.Numerics.Statistics.Distributions.LogisticDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
                        sample = CreateSample(logistic_distribution, 1000000, Seed);

                        break;
                    case "Beta":
                        //distribution = new BetaDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Beta = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2 };

                        Meta.Numerics.Statistics.Distributions.BetaDistribution Beta_distribution =
                            new Meta.Numerics.Statistics.Distributions.BetaDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
                        sample = CreateSample(Beta_distribution, 1000000, Seed);
                        break;
                    case "Pareto":
                        //distribution = new ParetoDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Beta = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2 };
                        Meta.Numerics.Statistics.Distributions.ParetoDistribution Pareto_distribution =
                            new Meta.Numerics.Statistics.Distributions.ParetoDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
                        sample = CreateSample(Pareto_distribution, 1000000, Seed);
                        break;
                    case "Cauchy":
                        //Troschuetz.Random.Distribution distribution = new Troschuetz.Random.CauchyDistribution() { Alpha = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, Gamma = crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2 };
                        Meta.Numerics.Statistics.Distributions.CauchyDistribution Cauchy_distribution =
                            new Meta.Numerics.Statistics.Distributions.CauchyDistribution(crSelectFunction.BenMAPHealthImpactFunction.BetaParameter1, crSelectFunction.BenMAPHealthImpactFunction.BetaParameter2);
                        sample = CreateSample(Cauchy_distribution, 1000000, Seed);
                        //double[] samples = new double[1000000];
                        //Stopwatch watch = new Stopwatch();
                        //watch.Start();
                        //for (int index = 0; index < samples.Length; index++)
                        //{
                        //    samples[index] = distribution.NextDouble();
                        //}
                        //watch.Stop();
                        //double duration = (double)watch.ElapsedTicks / (double)Stopwatch.Frequency;

                        ////Determine sum, minimum, maximum and display the last two together with a computed mean value.
                        //double sum = 0, minimum = double.MaxValue, maximum = double.MinValue;
                        //for (int index = 0; index < samples.Length; index++)
                        //{
                        //    sum += samples[index];
                        //    if (samples[index] > maximum)
                        //        maximum = samples[index];
                        //    if (samples[index] < minimum)
                        //        minimum = samples[index];
                        //}
                        //double mean = sum / samples.Length;
                        //double variance = 0.0;
                        //for (int index = 0; index < samples.Length; index++)
                        //{
                        //    variance += Math.Pow(samples[index] - mean, 2);
                        //}
                        //variance /= samples.Length;
                        //List<double> lstsamples = samples.ToList();
                        //lstsamples.Sort();

                        //for (int i = 0; i < LatinHypercubePoints; i++)
                        //{
                        //    lhsResultArray[i] = lstsamples.GetRange(i * (lstsamples.Count / LatinHypercubePoints), (lstsamples.Count / LatinHypercubePoints)).Median();
                        //    //lhsResultArray[LatinHypercubePoints - i-1] = lstsamples[(LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints) > lstsamples.Count-1 ? lstsamples.Count-1 : (LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints)];
                        //    //i++;
                        //}
                        //return lhsResultArray;

                        break;
                    case "Custom":
                        //distribution=new CustomDistributionEntries(){ 
                        //------------首先得到Custom的实例---------------然后计算均值-------------------
                        string commandText = string.Format("select   VValue  from CRFunctionCustomEntries where CRFunctionID={0} order by vvalue", crSelectFunction.BenMAPHealthImpactFunction.ID);
                        ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();

                        DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                        List<double> lstCustom = new List<double>();
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            lstCustom.Add(Convert.ToDouble(dr[0]));

                        }
                        lstCustom.Sort();
                        //List<int> lstInt = new List<int>();
                        for (int i = 0; i < LatinHypercubePoints; i++)
                        {
                            //lstInt.Add(100 * i / LatinHypercubePoints);
                            lhsResultArray[i] = lstCustom.GetRange(i * (lstCustom.Count / LatinHypercubePoints), (lstCustom.Count / LatinHypercubePoints)).Median();
                            //lhsResultArray[LatinHypercubePoints - i-1] = lstsamples[(LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints) > lstsamples.Count-1 ? lstsamples.Count-1 : (LatinHypercubePoints - i) * (lstsamples.Count / LatinHypercubePoints)];
                            //i++;
                        }
                        //lhsResultArray = Extreme.Statistics.Stats.Percentiles(lstCustom.ToArray(), lstInt.ToArray());
                        return lhsResultArray;
                        break;


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

       /// <summary>
       /// 获取某网格Prevalence值，已被替换-速度慢
       /// </summary>
       /// <param name="Col"></param>
       /// <param name="Row"></param>
       /// <param name="lstPrevalenceRateAttribute"></param>
       /// <param name="PrevalenceDataSetGridType"></param>
       /// <param name="GridDefinitionID"></param>
       /// <param name="gridRelationShipPrevalence"></param>
       /// <returns></returns>
       public static double getPrevalenceValueFromColRow(int Col, int Row, List<IncidenceRateAttribute> lstPrevalenceRateAttribute, int PrevalenceDataSetGridType, int GridDefinitionID, GridRelationship gridRelationShipPrevalence)
       {
           try
           {
               double prevalenceValue = 0;
               if (lstPrevalenceRateAttribute.Count > 0)
               {
                   //求prevalenceValue

                   if (PrevalenceDataSetGridType == GridDefinitionID)
                   {
                       var queryPrevalence = from a in lstPrevalenceRateAttribute where a.Col == Col && a.Row == Row select a;
                       //暂不考虑startAge,endAge
                       double values = 0;
                       foreach (IncidenceRateAttribute iRateAttributes in queryPrevalence)
                       {
                           values += iRateAttributes.Value;

                       }
                       if (queryPrevalence.Count() > 0) prevalenceValue = values / Convert.ToDouble(queryPrevalence.Count());
                   }
                   else//如果不是一个Grid，则需要查询用哪些RowCol------------------------------------------------------------
                   {
                       //--------------如果Grid比PrevalenceDataSetGridType大，则使用incidence平均，如果比PrevalenceDataSetGridType小，则直接选大的那个
                       RowCol rowColPrevalence = new RowCol() { Col = Col, Row = Row };
                       List<RowCol> lstPrevalenceRowCol;
                       //--------------如果Grid比incidenceDataSetGridType大，则使用incidence平均，如果比incidenceDataSetGridType小，则直接选大的那个
                       if (PrevalenceDataSetGridType == gridRelationShipPrevalence.bigGridID)
                       {
                           //region的网格类型比较大

                           var queryrowCol = from a in gridRelationShipPrevalence.lstGridRelationshipAttribute where a.smallGridRowCol.Contains(rowColPrevalence, new RowColComparer()) select new RowCol() { Col = a.bigGridRowCol.Col, Row = a.bigGridRowCol.Row };
                           lstPrevalenceRowCol = queryrowCol.ToList();
                           var queryPrevalence = from a in lstPrevalenceRateAttribute where lstPrevalenceRowCol.Contains(new RowCol() { Col = a.Col, Row = a.Row }, new RowColComparer()) select new { Values = lstPrevalenceRateAttribute.Average(c => c.Value) };
                           ////暂不考虑startAge,endAge
                           //double values = 0;
                           //foreach (IncidenceRateAttribute iRateAttributes in queryPrevalence)
                           //{
                           //    values += iRateAttributes.Value;

                           //}
                           //if (queryPrevalence.Count() > 0) prevalenceValue = values / Convert.ToDouble(queryPrevalence.Count());
                           if (queryPrevalence != null && queryPrevalence.Count() > 0)
                               prevalenceValue = queryPrevalence.First().Values;

                       }
                       else
                       {
                           //RowCol rowCol = new RowCol() { Col = col, Row = row };
                           var queryrowCol = from a in gridRelationShipPrevalence.lstGridRelationshipAttribute where a.bigGridRowCol.Col == rowColPrevalence.Col && a.bigGridRowCol.Row == rowColPrevalence.Row select a;
                           if (queryrowCol != null && queryrowCol.Count() > 0)
                           {
                               lstPrevalenceRowCol = queryrowCol.First().smallGridRowCol;
                               List<IncidenceRateAttribute> lstQueryPrevalence = new List<IncidenceRateAttribute>();
                               var queryPrevalence = from a in lstPrevalenceRateAttribute where lstPrevalenceRowCol.Contains(new RowCol() { Row = a.Row, Col = a.Col }, new RowColComparer()) select new { Values = lstPrevalenceRateAttribute.Average(c => c.Value) };
                               //lstQueryPrevalence = queryPrevalence.ToList();
                               ////foreach (RowCol rc in lstPrevalenceRowCol)
                               ////{
                               ////    var queryPrevalence = from a in lstPrevalenceRateAttribute where a.Col == rc.Col && a.Row == rc.Row select a;
                               ////    IEnumerable<IncidenceRateAttribute> iqueryPrevalence = queryPrevalence.ToList();
                               ////    lstQueryPrevalence.AddRange(iqueryPrevalence);

                               ////}
                               ////暂不考虑startAge,endAge
                               //double values = 0;

                               //foreach (IncidenceRateAttribute iRateAttributes in lstQueryPrevalence)
                               //{
                               //    values += iRateAttributes.Value;

                               //}
                               //if (lstQueryPrevalence.Count() > 0) prevalenceValue = values / Convert.ToDouble(lstQueryPrevalence.Count());
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
       /// <summary>
       /// 获取某网格Incidence----已被替换，速度慢
       /// </summary>
       /// <param name="Col"></param>
       /// <param name="Row"></param>
       /// <param name="lstIncidenceRateAttribute"></param>
       /// <param name="incidenceDataSetGridType"></param>
       /// <param name="GridDefinitionID"></param>
       /// <param name="gridRelationShipIncidence"></param>
       /// <returns></returns>
       public static double getIncidenceValueFromColRow(int Col, int Row, List<IncidenceRateAttribute> lstIncidenceRateAttribute, int incidenceDataSetGridType, int GridDefinitionID, GridRelationship gridRelationShipIncidence)
       {
           try
           {
               double incidenceValue = 0;
               if (lstIncidenceRateAttribute.Count > 0)//考虑Col,Row的问题
               {
                   //求incidenceValue
                   //如果网格类型相同则取同样的Row Col的数据
                   if (incidenceDataSetGridType == GridDefinitionID)
                   {
                       var queryIncidence = from a in lstIncidenceRateAttribute where a.Col == Col && a.Row == Row select a;
                       //暂不考虑startAge,endAge
                       double values = 0;
                       foreach (IncidenceRateAttribute iRateAttributes in queryIncidence)
                       {
                           values += iRateAttributes.Value;

                       }
                       if (queryIncidence.Count() > 0) incidenceValue = values / Convert.ToDouble(queryIncidence.Count());
                   }
                   else//如果不是一个Grid，则需要查询用哪些RowCol------------------------------------------------------------
                   {

                       List<RowCol> lstIncidenceRowCol;
                       //--------------如果Grid比incidenceDataSetGridType大，则使用incidence平均，如果比incidenceDataSetGridType小，则直接选大的那个
                       if (incidenceDataSetGridType == gridRelationShipIncidence.bigGridID)
                       {
                           //region的网格类型比较大
                           RowCol rowColIncidence = new RowCol() { Col = Col, Row = Row };
                           var queryrowCol = from a in gridRelationShipIncidence.lstGridRelationshipAttribute where a.smallGridRowCol.Contains(rowColIncidence, new RowColComparer()) select new RowCol() { Col = a.bigGridRowCol.Col, Row = a.bigGridRowCol.Row };
                           lstIncidenceRowCol = queryrowCol.ToList();

                           var queryIncidence = from a in lstIncidenceRateAttribute where lstIncidenceRowCol.Contains(new RowCol() { Col = a.Col, Row = a.Row }, new RowColComparer()) select new { Values = lstIncidenceRateAttribute.Average(c => c.Value) };
                           //暂不考虑startAge,endAge
                           //double values = 0;
                           //foreach (IncidenceRateAttribute iRateAttributes in queryIncidence)
                           //{
                           //    values += iRateAttributes.Value;

                           //}
                           //if (queryIncidence.Count() > 0) 
                           if (queryIncidence != null && queryIncidence.Count() > 0)
                               incidenceValue = queryIncidence.First().Values;// values / Convert.ToDouble(queryIncidence.Count());

                       }
                       else
                       {
                           //RowCol rowCol = new RowCol() { Col = col, Row = row };
                           var queryrowCol = from a in gridRelationShipIncidence.lstGridRelationshipAttribute where a.bigGridRowCol.Col == Col && a.bigGridRowCol.Row == Row select a;
                           if (queryrowCol != null && queryrowCol.Count() > 0)
                           {
                               lstIncidenceRowCol = queryrowCol.First().smallGridRowCol;
                               var queryIncidence = from a in lstIncidenceRateAttribute where lstIncidenceRowCol.Contains(new RowCol() { Row = a.Row, Col = a.Col }, new RowColComparer()) select new { Values = lstIncidenceRateAttribute.Average(c => c.Value) };

                               if (queryIncidence != null && queryIncidence.Count() > 0)
                                   incidenceValue = queryIncidence.First().Values;// values / Convert.ToDouble(lstQueryIncidence.Count());
                           }

                           //------------------join list-----------------------
                           //var queryIncidenceJoin= 
                           //    from a in lstIncidenceRateAttribute  join b in gridRelationShipIncidence.lstGridRelationshipAttribute on  b.bigGridRowCol.Col equals rowColIncidence.Col && b.bigGridRowCol.Row == rowColIncidence.Row && b.smallGridRowCol.Contains(new RowCol() { Row = a.Row, Col = a.Col }, new RowColComparer())
                           //                                         select new {  Values = lstIncidenceRateAttribute.Average(c => c.Value) };
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
       /// <summary>
       /// 获取某网格Population---已被替换，速度慢
       /// </summary>
       /// <param name="Col"></param>
       /// <param name="Row"></param>
       /// <param name="benMAPPopulation"></param>
       /// <param name="lstPopulationAttribute"></param>
       /// <param name="GridDefinitionID"></param>
       /// <param name="gridRelationShipPopulation"></param>
       /// <returns></returns>
       public static double getPopulationValueFromColRow(int Col, int Row, BenMAPPopulation benMAPPopulation, List<PopulationAttribute> lstPopulationAttribute, int GridDefinitionID, GridRelationship gridRelationShipPopulation)
       {
           try
           {
               double PopulationValue = 0;
               if (lstPopulationAttribute.Count > 0)//考虑Col,Row的问题
               {
                   //求incidenceValue
                   //如果网格类型相同则取同样的Row Col的数据
                   if (benMAPPopulation.GridType.GridDefinitionID == GridDefinitionID)
                   {
                       var queryPopulation = from a in lstPopulationAttribute where a.Col == Col && a.Row == Row select a;
                       //暂不考虑startAge,endAge
                       double values = 0;
                       foreach (PopulationAttribute iPopulationAttributes in queryPopulation)
                       {
                           values += iPopulationAttributes.Value;

                       }
                       if (queryPopulation.Count() > 0) PopulationValue = values;// Convert.ToDouble(queryPopulation.Count());
                   }
                   else//如果不是一个Grid，则需要查询用哪些RowCol------------------------------------------------------------
                   {
                       RowCol rowColPopulation = new RowCol() { Col = Col, Row = Row };
                       List<RowCol> lstPopulationRowCol;
                       //--------------如果Grid比incidenceDataSetGridType大，则使用incidence平均，如果比incidenceDataSetGridType小，则直接选大的那个
                       if (benMAPPopulation.GridType.GridDefinitionID == gridRelationShipPopulation.bigGridID)
                       {
                           //region的网格类型比较大

                           var queryrowCol = from a in gridRelationShipPopulation.lstGridRelationshipAttribute where a.smallGridRowCol.Contains(rowColPopulation, new RowColComparer()) select new RowCol() { Col = a.bigGridRowCol.Col, Row = a.bigGridRowCol.Row };
                           lstPopulationRowCol = queryrowCol.ToList();

                           var queryPopulation = from a in lstPopulationAttribute where lstPopulationRowCol.Contains(new RowCol() { Col = a.Col, Row = a.Row }, new RowColComparer()) select new { Values = lstPopulationAttribute.Sum(c => c.Value) };
                           //暂不考虑startAge,endAge
                           //double values = 0;
                           //foreach (PopulationAttribute iPopulationAttributes in queryPopulation)
                           //{
                           //    values += iPopulationAttributes.Value;

                           //}
                           if (queryPopulation != null && queryPopulation.Count() > 0)
                               PopulationValue = queryPopulation.First().Values / lstPopulationRowCol.Count;

                       }
                       else
                       {
                           //RowCol rowCol = new RowCol() { Col = col, Row = row };
                           var queryrowCol = from a in gridRelationShipPopulation.lstGridRelationshipAttribute where a.bigGridRowCol.Col == rowColPopulation.Col && a.bigGridRowCol.Row == rowColPopulation.Row select a;
                           if (queryrowCol != null && queryrowCol.Count() > 0)
                           {
                               lstPopulationRowCol = queryrowCol.First().smallGridRowCol;
                               List<PopulationAttribute> lstQueryPopulation = new List<PopulationAttribute>();
                               var queryPopulation = from a in lstPopulationAttribute where lstPopulationRowCol.Contains(new RowCol() { Row = a.Row, Col = a.Col }, new RowColComparer()) select new { Values = lstPopulationAttribute.Sum(c => c.Value) };
                               //lstQueryPopulation = queryPopulation.ToList();
                               ////foreach (RowCol rc in lstPopulationRowCol)
                               ////{
                               ////    var queryPopulation = from a in benMAPPopulation.PopulationAttributes where a.Col == rc.Col && a.Row == rc.Row select a;
                               ////    IEnumerable<PopulationAttribute> iqueryIncidence = queryPopulation.ToList();
                               ////    lstQueryPopulation.AddRange(iqueryIncidence);

                               ////}
                               ////暂不考虑startAge,endAge
                               //double values = 0;

                               //foreach (PopulationAttribute iRateAttributes in lstQueryPopulation)
                               //{
                               //    values += iRateAttributes.Value;

                               //}
                               //if (lstQueryPopulation.Count() > 0) PopulationValue = values ;
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

       //public static double getPopulationValueFromColRow(int Col, int Row, int DataSetID, int Year, int GridDefinitionID,int PopulationGridID,int RaceID,int GenderID,int EthnicityID,int StartAge,int EndAge, GridRelationship gridRelationShipPopulation)
       //{
       //    double PopulationValue = 0;
       //    //string commandText = string.Format("select a.PopulationDatasetID,a.RaceID,a.GenderID,a.AgeRangeID,a.CColumn,a.Row,a.YYear,a.VValue,a.EthnicityID  ,b.StartAge,b.EndAge      from PopulationEntries a,Ageranges b   where a.AgerangeID=b.AgerangeID and a.PopulationDatasetID={0} and YYear={1}", DataSetID, Year);
       //    string commandText = string.Format("select sum(a.VValue)      from PopulationEntries a,Ageranges b   where a.AgerangeID=b.AgerangeID and a.PopulationDatasetID={0} and YYear={1}", DataSetID, Year);
       //    //commandText = string.Format("select a.PopulationDatasetID,a.RaceID,a.GenderID,a.AgeRangeID,a.CColumn,a.Row,a.YYear,a.VValue,a.EthnicityID  ,b.StartAge,b.EndAge      from PopulationEntries a,Ageranges b   where a.AgerangeID=b.AgerangeID and a.PopulationDatasetID={0} and YYear={1}", DataSetID, Year);
       //    if (RaceID >= 0)
       //    {
       //        commandText = commandText + string.Format(" and a.RaceID={0}",RaceID);
       //    }
       //    if (GenderID > 0)
       //    {
       //        commandText = commandText + string.Format(" and a.GenderID={0}", GenderID);
       //    }
       //    if (EthnicityID > 0)
       //    {
       //        commandText = commandText + string.Format(" and a.EthnicityID={0}", EthnicityID);
       //    }
       //    if (StartAge > 0)
       //    {
       //        commandText = commandText + string.Format(" and b.StartAge>={0}", StartAge);
       //    }
       //    if (EndAge > 0)
       //    {
       //        commandText = commandText + string.Format(" and b.EndAge<={0}", EndAge);
       //    }
           
       //        //求incidenceValue
       //        //如果网格类型相同则取同样的Row Col的数据
       //    if (PopulationGridID == GridDefinitionID)
       //        {
       //            commandText = commandText + string.Format(" and a.CColumn={0} and a.Row={1}", Col,Row);
           
       //        }
       //        else//如果不是一个Grid，则需要查询用哪些RowCol------------------------------------------------------------
       //        {
       //            RowCol rowColPopulation = new RowCol() { Col = Col, Row = Row };
       //            List<RowCol> lstPopulationRowCol;
       //            //--------------如果Grid比incidenceDataSetGridType大，则使用incidence平均，如果比incidenceDataSetGridType小，则直接选大的那个
       //            if (PopulationGridID == gridRelationShipPopulation.bigGridID)
       //            {
       //                //region的网格类型比较大

       //                var queryrowCol = from a in gridRelationShipPopulation.lstGridRelationshipAttribute where a.smallGridRowCol.Contains(rowColPopulation, new RowColComparer()) select new RowCol() { Col = a.bigGridRowCol.Col, Row = a.bigGridRowCol.Row };
       //                lstPopulationRowCol = queryrowCol.ToList();

       //                var queryPopulation = from a in benMAPPopulation.PopulationAttributes where lstPopulationRowCol.Contains(new RowCol() { Col = a.Col, Row = a.Row }, new RowColComparer()) select new { Values = benMAPPopulation.PopulationAttributes.Sum(c => c.Value) };
       //                //暂不考虑startAge,endAge
       //                //double values = 0;
       //                //foreach (PopulationAttribute iPopulationAttributes in queryPopulation)
       //                //{
       //                //    values += iPopulationAttributes.Value;

       //                //}
       //                if (queryPopulation != null && queryPopulation.Count() > 0)
       //                    PopulationValue = queryPopulation.First().Values;

       //            }
       //            else
       //            {
       //                //RowCol rowCol = new RowCol() { Col = col, Row = row };
       //                var queryrowCol = from a in gridRelationShipPopulation.lstGridRelationshipAttribute where a.bigGridRowCol.Col == rowColPopulation.Col && a.bigGridRowCol.Row == rowColPopulation.Row select a;
       //                if (queryrowCol != null && queryrowCol.Count() > 0)
       //                {
       //                    lstPopulationRowCol = queryrowCol.First().smallGridRowCol;
       //                    List<PopulationAttribute> lstQueryPopulation = new List<PopulationAttribute>();
       //                    var queryPopulation = from a in benMAPPopulation.PopulationAttributes where lstPopulationRowCol.Contains(new RowCol() { Row = a.Row, Col = a.Col }, new RowColComparer()) select new { Values = benMAPPopulation.PopulationAttributes.Sum(c => c.Value) };
       //                    //lstQueryPopulation = queryPopulation.ToList();
       //                    ////foreach (RowCol rc in lstPopulationRowCol)
       //                    ////{
       //                    ////    var queryPopulation = from a in benMAPPopulation.PopulationAttributes where a.Col == rc.Col && a.Row == rc.Row select a;
       //                    ////    IEnumerable<PopulationAttribute> iqueryIncidence = queryPopulation.ToList();
       //                    ////    lstQueryPopulation.AddRange(iqueryIncidence);

       //                    ////}
       //                    ////暂不考虑startAge,endAge
       //                    //double values = 0;

       //                    //foreach (PopulationAttribute iRateAttributes in lstQueryPopulation)
       //                    //{
       //                    //    values += iRateAttributes.Value;

       //                    //}
       //                    //if (lstQueryPopulation.Count() > 0) PopulationValue = values ;
       //                    if (queryPopulation != null && queryPopulation.Count() > 0)
       //                        PopulationValue = queryPopulation.First().Values;
       //                }
       //            }
       //        }
            
       //    return PopulationValue;

       //}
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


                   //------------

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
                   if (benMAPPopulation.GridType.GridDefinitionID == gridRelationShipPopulation.bigGridID)//Population比较大
                   {
                       foreach (GridRelationshipAttribute gra in gridRelationShipPopulation.lstGridRelationshipAttribute)
                       {
                           //var queryPopulation = from a in lstPopulationAttribute where gra.bigGridRowCol.Col == a.Col && gra.bigGridRowCol.Row == a.Row  select new { Values = lstPopulationAttribute.Average(c => c.Value) };
                           //if (queryPopulation != null && queryPopulation.Count() > 0 && gra.smallGridRowCol.Count > 0)
                           //{
                           //    PopulationValue = queryPopulation.First().Values;// / Convert.ToDouble(gra.smallGridRowCol.Count);
                           //    foreach (RowCol rc in gra.smallGridRowCol)
                           //    {
                           //        lstResult.Add(new PopulationAttribute()
                           //        {
                           //            Col = rc.Col,
                           //            Row = rc.Row,
                           //            Value = PopulationValue
                           //        });
                           //    }
                           //}
                           //lstTemp = lstPopulationAttribute.Where(p => p.Col == gra.bigGridRowCol.Col && p.Row == gra.bigGridRowCol.Row).Select(a => a.Value).ToList();
                           //if (lstTemp != null && lstTemp.Count > 0)
                           //{
                           if(diclstPopulationAttribute.Keys.Contains(gra.bigGridRowCol.Col+","+gra.bigGridRowCol.Row))
                           {
                               foreach (RowCol rc in gra.smallGridRowCol)
                               {
                                   lstResult.Add(new PopulationAttribute()
                                   {
                                       Col = rc.Col,
                                       Row = rc.Row,
                                       Value =diclstPopulationAttribute[gra.bigGridRowCol.Col+","+gra.bigGridRowCol.Row]/Convert.ToSingle(gra.smallGridRowCol.Count())// lstPopulationAttribute.Where(p => p.Col == gra.bigGridRowCol.Col && p.Row == gra.bigGridRowCol.Row).Select(a => a.Value).Average()
                                   });
                               }
                           }
                           //}

                       }
                   }
                   else//网格类型比较大
                   {
                       foreach (GridRelationshipAttribute gra in gridRelationShipPopulation.lstGridRelationshipAttribute)
                       {
                           //var queryPopulation = from a in lstPopulationAttribute where gra.smallGridRowCol.Contains(new RowCol() { Row = a.Row, Col = a.Col }, new RowColComparer()) select new { Values = lstPopulationAttribute.Average(c => c.Value) };

                           //if (queryPopulation != null && queryPopulation.Count() > 0)
                           //{
                           //    PopulationValue = queryPopulation.First().Values;
                           //    lstResult.Add(new PopulationAttribute()
                           //    {
                           //        Col = gra.bigGridRowCol.Col,
                           //        Row = gra.bigGridRowCol.Row,
                           //        Value = PopulationValue
                           //    });
                           //}

                         //lstTemp=  lstPopulationAttribute.Where(p => gra.smallGridRowCol.Contains(new RowCol() { Row = p.Row, Col = p.Col }, new RowColComparer())).Select(a => a.Value).ToList();
                         //if (lstTemp != null && lstTemp.Count > 0)
                         //{
                             lstResult.Add(new PopulationAttribute()
                             {
                                 Col = gra.bigGridRowCol.Col,
                                 Row = gra.bigGridRowCol.Row,
                                 Value =0// lstPopulationAttribute.Where(p => p.Col == gra.bigGridRowCol.Col && p.Row == gra.bigGridRowCol.Row).Select(a => a.Value).Average()
                             });
                             foreach (RowCol rc in gra.smallGridRowCol)
                             {
                                 if (diclstPopulationAttribute.Keys.Contains(rc.Col + "," + rc.Row))
                                 {
                                     lstResult[lstResult.Count - 1].Value += diclstPopulationAttribute[rc.Col + "," + rc.Row];
                                 }
                             }
                              
                         //}

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
       public static string getPopulationComandTextFromCRSelectFunction(CRSelectFunction crSelectFunction, BenMAPPopulation benMAPPopulation,  Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender)
       {
           ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
           int benMAPPopulationDataSetID = benMAPPopulation.DataSetID;
           //string commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", benMAPPopulationDataSetID);// benMAPPopulation.Year);
           //int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
           string commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", benMAPPopulation.DataSetID);// benMAPPopulation.Year);
           int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
           if (CommonClass.MainSetup.SetupID != 1) commonYear = benMAPPopulation.Year;
           commandText = "";
           //string commandText = "";
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
               //-------如果StartAge在区间内--，则sum---不然--考虑-1的情况------------
               if (strsumageGrowth == "")
                   strsumageGrowth = dr["AgerangeID"].ToString();
               else
                   strsumageGrowth = strsumageGrowth + "," + dr["AgerangeID"].ToString();
               if ((Convert.ToInt32(dr["StartAge"]) >= crSelectFunction.StartAge || crSelectFunction.StartAge == -1) && (Convert.ToInt32(dr["EndAge"]) <= crSelectFunction.EndAge || crSelectFunction.EndAge == -1))
               {
                   //sum
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
                   //-------------------修正---如果setup=1 ,且gridtype=county，则直接和Growth关联，如果Setup!=1,commonYear=benMapPopulationYear--------------
                   if (benMAPPopulation.GridType.GridDefinitionID == 1 && CommonClass.MainSetup.SetupID == 1 && commonYear != benMAPPopulation.Year)
                   {
                       commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue)*" + dDiv + " as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
                           "  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);


                   }
                   else if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && commonYear != benMAPPopulation.Year)
                   {
                       commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue*c.percentage)*" + dDiv + " as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b ," +
                                 " (select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=22 and normalizationstate in (0,1)) c " +
                                 "  where a.CColumn=c.sourcecolumn and a.Row=c.sourcerow  and b.CColumn= c.TargetColumn and b.Row= c.TargetRow and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);



                       //commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue)*" + dDiv + " as VValue   from PopulationEntries a,(select sourcecolumn as CColumn, sourcerow as row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries a," +
                       //    " (select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=22 and normalizationstate in (0,1)) b "
                       //    + " where a.CColumn= b.TargetColumn and a.Row= b.TargetRow and PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
                                
                       //         "  where a.CColumn=b.CColumn and a.Row=b.Row    and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);


                   }
                   else
                   {
                       commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue)*" + dDiv + " as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);
                   }
                   //commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue)*" + dDiv + " as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and YYear={1}", benMAPPopulationDataSetID, commonYear);// benMAPPopulation.Year);
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
              // commandText = commandText + string.Format("select   a.CColumn,a.Row,sum(a.vvalue) as VValue   from PopulationEntries a   where       a.PopulationDatasetID={0} and YYear={1}", benMAPPopulationDataSetID, commonYear);// benMAPPopulation.Year);
               //-------------------修正---如果setup=1 ,且gridtype=county，则直接和Growth关联，如果Setup!=1,commonYear=benMapPopulationYear--------------
               if (benMAPPopulation.GridType.GridDefinitionID == 1 && CommonClass.MainSetup.SetupID == 1 && commonYear != benMAPPopulation.Year)
               {
                   commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.VValue) as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
                       "  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);


               }
               else if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && commonYear != benMAPPopulation.Year)
               {
                   commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.VValue*c.percentage) as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b ," +
                      " (select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=22 and normalizationstate in (0,1)) c " +
                              "  where a.CColumn=c.sourcecolumn and a.Row=c.sourcerow  and b.CColumn= c.TargetColumn and b.Row= c.TargetRow and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);
                   //commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue) as VValue   from PopulationEntries a,(select sourcecolumn as CColumn, sourcerow as row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries a," +
                   //      " (select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=22 and normalizationstate in (0,1)) b "
                   //      + " where a.CColumn= b.TargetColumn and a.Row= b.TargetRow and PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +

                   //           "  where a.CColumn=b.CColumn and a.Row=b.Row    and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);



               }
               else
               {
                   commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue) as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);
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
           //string commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", benMAPPopulationDataSetID);// benMAPPopulation.Year);
           //int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
           string commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", benMAPPopulation.DataSetID);// benMAPPopulation.Year);
           int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
           if (CommonClass.MainSetup.SetupID != 1) commonYear = benMAPPopulation.Year;
           commandText = "";
           //string commandText = "";
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
               //-------如果StartAge在区间内--，则sum---不然--考虑-1的情况------------
               if (strsumageGrowth == "")
                   strsumageGrowth = dr["AgerangeID"].ToString();
               else
                   strsumageGrowth = strsumageGrowth + "," + dr["AgerangeID"].ToString();
               if ((Convert.ToInt32(dr["StartAge"]) >= crSelectFunction.StartAge || crSelectFunction.StartAge == -1) && (Convert.ToInt32(dr["EndAge"]) <= crSelectFunction.EndAge || crSelectFunction.EndAge == -1))
               {
                   //sum
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
                   //-------------------修正---如果setup=1 ,且gridtype=county，则直接和Growth关联，如果Setup!=1,commonYear=benMapPopulationYear--------------
                   if (benMAPPopulation.GridType.GridDefinitionID == 1 && CommonClass.MainSetup.SetupID == 1 && commonYear != benMAPPopulation.Year)
                   {
                       commandText += string.Format("select   a.CColumn,a.Row,a.AgeRangeID,sum(a.vvalue*b.vvalue)*" + dDiv + " as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
                           "  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);


                   }
                   else if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && commonYear != benMAPPopulation.Year)
                   {
                       commandText += string.Format("select   a.CColumn,a.Row,a.AgeRangeID,sum(a.vvalue*b.vvalue*c.VValue)*" + dDiv + " as VValue   from PopulationEntries a, PopulationEntries  b ," +
                                 " PopulationGrowthWeights   c   where PopulationDatasetID=2 and YYear="+CommonClass.BenMAPPopulation.Year+" and a.RaceID=c.RaceID and  a.EthnicityID=c.EthnicityID and a.CColumn= c.TargetColumn  " +
" and a.Row=c.Targetrow  and b.CColumn= c.SourceColumn and b.Row= c.SourceRow and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);



                       //commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue)*" + dDiv + " as VValue   from PopulationEntries a,(select sourcecolumn as CColumn, sourcerow as row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries a," +
                       //    " (select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=22 and normalizationstate in (0,1)) b "
                       //    + " where a.CColumn= b.TargetColumn and a.Row= b.TargetRow and PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +

                       //         "  where a.CColumn=b.CColumn and a.Row=b.Row    and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);


                   }
                   else
                   {
                       commandText += string.Format("select   a.CColumn,a.Row,a.AgeRangeID,sum(a.vvalue)*" + dDiv + " as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);
                   }
                   //commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue)*" + dDiv + " as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and YYear={1}", benMAPPopulationDataSetID, commonYear);// benMAPPopulation.Year);
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
               // commandText = commandText + string.Format("select   a.CColumn,a.Row,sum(a.vvalue) as VValue   from PopulationEntries a   where       a.PopulationDatasetID={0} and YYear={1}", benMAPPopulationDataSetID, commonYear);// benMAPPopulation.Year);
               //-------------------修正---如果setup=1 ,且gridtype=county，则直接和Growth关联，如果Setup!=1,commonYear=benMapPopulationYear--------------
               if (benMAPPopulation.GridType.GridDefinitionID == 1 && CommonClass.MainSetup.SetupID == 1 && commonYear != benMAPPopulation.Year)
               {
                   commandText += string.Format("select   a.CColumn,a.Row,a.AgeRangeID,sum(a.vvalue*b.VValue) as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
                       "  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);


               }
               else if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && commonYear != benMAPPopulation.Year)
               {
                   commandText += string.Format("select   a.CColumn,a.Row,a.AgeRangeID,sum(a.vvalue*b.vvalue*c.VValue) as VValue   from PopulationEntries a, PopulationEntries  b ," +
                                  " PopulationGrowthWeights   c   where PopulationDatasetID=2 and YYear=" + CommonClass.BenMAPPopulation.Year + " and a.RaceID=c.RaceID and  a.EthnicityID=c.EthnicityID and a.CColumn= c.TargetColumn  " +
 " and a.Row=c.Targetrow  and b.CColumn= c.SourceColumn and b.Row= c.SourceRow and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);
                   //commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue) as VValue   from PopulationEntries a,(select sourcecolumn as CColumn, sourcerow as row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries a," +
                   //      " (select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=22 and normalizationstate in (0,1)) b "
                   //      + " where a.CColumn= b.TargetColumn and a.Row= b.TargetRow and PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +

                   //           "  where a.CColumn=b.CColumn and a.Row=b.Row    and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);



               }
               else
               {
                   commandText += string.Format("select   a.CColumn,a.Row,a.AgeRangeID,sum(a.vvalue) as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);
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
           //string commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", benMAPPopulationDataSetID);// benMAPPopulation.Year);
           //int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
           string commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", benMAPPopulation.DataSetID);// benMAPPopulation.Year);
           int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
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

           //string commandText = "";
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
               //-------如果StartAge在区间内--，则sum---不然--考虑-1的情况------------
               if (strsumageGrowth == "")
                   strsumageGrowth = dr["AgerangeID"].ToString();
               else
                   strsumageGrowth = strsumageGrowth + "," + dr["AgerangeID"].ToString();
               if ((Convert.ToInt32(dr["StartAge"]) >= crSelectFunction.StartAge || crSelectFunction.StartAge == -1) && (Convert.ToInt32(dr["EndAge"]) <= crSelectFunction.EndAge || crSelectFunction.EndAge == -1))
               {
                   //sum
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
                   //-------------------修正---如果setup=1 ,且gridtype=county，则直接和Growth关联，如果Setup!=1,commonYear=benMapPopulationYear--------------
                   
                       commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue)*" + dDiv + " as VValue   from POP12kmToCounty a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
                           "  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID ");// benMAPPopulation.Year);


                   
                   //commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue)*" + dDiv + " as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and YYear={1}", benMAPPopulationDataSetID, commonYear);// benMAPPopulation.Year);
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
               // commandText = commandText + string.Format("select   a.CColumn,a.Row,sum(a.vvalue) as VValue   from PopulationEntries a   where       a.PopulationDatasetID={0} and YYear={1}", benMAPPopulationDataSetID, commonYear);// benMAPPopulation.Year);
               //-------------------修正---如果setup=1 ,且gridtype=county，则直接和Growth关联，如果Setup!=1,commonYear=benMapPopulationYear--------------
              
                   commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.VValue) as VValue   from POP12kmToCounty a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
                       "  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID ");// benMAPPopulation.Year);

 
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
           //string commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", benMAPPopulationDataSetID);// benMAPPopulation.Year);
           //int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
           string commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", benMAPPopulation.DataSetID);// benMAPPopulation.Year);
           int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
           if (CommonClass.MainSetup.SetupID != 1) commonYear = benMAPPopulation.Year;
           commandText = "";
           //string commandText = "";
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
               //-------如果StartAge在区间内--，则sum---不然--考虑-1的情况------------
               if (strsumageGrowth == "")
                   strsumageGrowth = dr["AgerangeID"].ToString();
               else
                   strsumageGrowth = strsumageGrowth + "," + dr["AgerangeID"].ToString();
               if ((Convert.ToInt32(dr["StartAge"]) >= crSelectFunction.StartAge || crSelectFunction.StartAge == -1) && (Convert.ToInt32(dr["EndAge"]) <= crSelectFunction.EndAge || crSelectFunction.EndAge == -1))
               {
                   //sum
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
                   //-------------------修正---如果setup=1 ,且gridtype=county，则直接和Growth关联，如果Setup!=1,commonYear=benMapPopulationYear--------------
                   if (benMAPPopulation.GridType.GridDefinitionID == 1 && CommonClass.MainSetup.SetupID == 1 && commonYear != benMAPPopulation.Year)
                   {
                       commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue*" + dDiv + ") as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
                           "  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);


                   }
                   else if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && commonYear != benMAPPopulation.Year)
                   {
                       commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue*" + dDiv + ") as VValue   from PopulationEntries a,"+
                           "(select b.SourceColumn,b.SourceRow,a.VValue*b.VValue as VValue,a.AgerangeID,a.RaceID,a.EthnicityID,a.GenderID from PopulationEntries a,populationgrowthweights b where a.PopulationDatasetID=2 and a.YYear=" + benMAPPopulation.Year + " and a.CColumn=b.targetcolumn and a.Row =b.TargetRow and a.EthnicityID=b.EthnicityID and a.RaceID=b.RaceID) b " +
                              "  where  a.CColumn=b.sourcecolumn and a.Row=b.sourcerow  and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);
                 
                              //   "  where a.CColumn=c.sourcecolumn and a.Row=c.sourcerow  and b.CColumn= c.TargetColumn and b.Row= c.TargetRow and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);



                       //commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue)*" + dDiv + " as VValue   from PopulationEntries a,(select sourcecolumn as CColumn, sourcerow as row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries a," +
                       //    " (select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=22 and normalizationstate in (0,1)) b "
                       //    + " where a.CColumn= b.TargetColumn and a.Row= b.TargetRow and PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +

                       //         "  where a.CColumn=b.CColumn and a.Row=b.Row    and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);


                   }
                   else
                   {
                       commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue)*" + dDiv + " as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);
                   }
                   //commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue)*" + dDiv + " as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and YYear={1}", benMAPPopulationDataSetID, commonYear);// benMAPPopulation.Year);
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
               // commandText = commandText + string.Format("select   a.CColumn,a.Row,sum(a.vvalue) as VValue   from PopulationEntries a   where       a.PopulationDatasetID={0} and YYear={1}", benMAPPopulationDataSetID, commonYear);// benMAPPopulation.Year);
               //-------------------修正---如果setup=1 ,且gridtype=county，则直接和Growth关联，如果Setup!=1,commonYear=benMapPopulationYear--------------
               if (benMAPPopulation.GridType.GridDefinitionID == 1 && CommonClass.MainSetup.SetupID == 1 && commonYear != benMAPPopulation.Year)
               {
                   commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.VValue) as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
                       "  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);


               }
               else if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && commonYear != benMAPPopulation.Year)
               {
                   //commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.VValue*c.vvalue) as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b ," +
                   //  // " (select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=22 and normalizationstate in (0,1)) c " +
                   // " (select sourcecolumn, sourcerow, targetcolumn, targetrow,ethnicityid,raceid, VValue from populationgrowthweights ) c " +
                   //           "  where c.ethnicityid=a.ethnicityid and a.raceid=c.raceid and a.CColumn=c.sourcecolumn and a.Row=c.sourcerow  and b.CColumn= c.TargetColumn and b.Row= c.TargetRow and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);
                   //commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue) as VValue   from PopulationEntries a,(select sourcecolumn as CColumn, sourcerow as row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries a," +
                   //      " (select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=22 and normalizationstate in (0,1)) b "
                   //      + " where a.CColumn= b.TargetColumn and a.Row= b.TargetRow and PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +

                   //           "  where a.CColumn=b.CColumn and a.Row=b.Row    and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);

                   commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue) as VValue   from PopulationEntries a," +
                           "(select b.TargetColumn,b.TargetRow,a.VValue*b.VValue as VValue,a.AgerangeID,a.RaceID,a.EthnicityID,a.GenderID from PopulationEntries a,populationgrowthweights b where a.PopulationDatasetID=2 and a.YYear=" + benMAPPopulation.Year + " and a.CColumn=b.targetcolumn and a.Row =b.TargetRow and a.EthnicityID=b.EthnicityID and a.RaceID=b.RaceID) b " +
                              "  where  a.CColumn=b.Targetcolumn and a.Row=b.TargetColumn  and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);
                 

               }
               else
               {
                   commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue) as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);
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
           //-----first get the max of id in griddefinitonpercentages
           ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
           int iMax = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;

           //-----insert into griddefinitonpercentages
           commandText = string.Format("insert into GridDefinitionPercentages values({0},{1})", iMax, dicAllGridPercentage.Key);
           fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
           //-----insert into GridDefinitionPercentageEntries
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
               //----------------批量提交-------------------------

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
                   //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

                   i = 1;

               }
               i++;
               j++;
               //---------------End 批量提交----------------------

               //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
           }
       }
       public static void creatPercentageToDatabase(int big, int small)
       {
           GridDefinition grd = new GridDefinition();
          // Dictionary<string, List<GridRelationshipAttributePercentage>> dicAllGridPercentage = grd.getRelationshipFromBenMAPGridPercentage(CommonClass.GBenMAPGrid.GridDefinitionID, (benMAPPopulation.GridType.GridDefinitionID == 13 ? 7 : benMAPPopulation.GridType.GridDefinitionID));
           Dictionary<string, List<GridRelationshipAttributePercentage>> dicAllGridPercentage = grd.getRelationshipFromBenMAPGridPercentage(big, small);
           updatePercentageToDatabase(dicAllGridPercentage.ToArray()[0]);
           CommonClass.IsAddPercentage = true;
           return;
           //------------------填入数据库-------------------------------
           ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
           foreach (KeyValuePair<string, List<GridRelationshipAttributePercentage>> k in dicAllGridPercentage)
           {
               string commandText = "select max(PercentageID) from GridDefinitionPercentages";
               //-----first get the max of id in griddefinitonpercentages

               int iMax = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText)) + 1;

               //-----insert into griddefinitonpercentages
               commandText = string.Format("insert into GridDefinitionPercentages values({0},{1})", iMax, k.Key);
               fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
               //-----insert into GridDefinitionPercentageEntries
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
                   //----------------批量提交-------------------------

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
                       //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

                       i = 1;

                   }
                   i++;
                   j++;
                   //---------------End 批量提交----------------------

                   //fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
               }
           }
           //------Add to CommonClass.LstGridRelationshipAll
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
       /// 从CRFunction中获取PopulationDataSet
       /// </summary>
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
               //FbConnection _connection = CommonClass.getNewConnection();

               // FbConnection _connection = new FirebirdSql.Data.FirebirdClient.FbConnection(str);
               //List<PopulationAttribute> lstPopulationAttribute = new List<PopulationAttribute>();
               Dictionary<int, float> dicPopulationAttribute = new Dictionary<int, float>();
               //List<PopulationAttribute> lstResult = new List<PopulationAttribute>();
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               Dictionary<string, float> diclstPopulationAttribute = new Dictionary<string, float>();
               Dictionary<string, Dictionary<string, double>> dicPopweightfromPercentage = new Dictionary<string, Dictionary<string, double>>();
               //Dictionary<string, float> diclstPopulationAttributeAge = new Dictionary<string, float>();
               //----直接修改为基准年为2000---------------------如果Setup不是美国区，则这部分需要修正
               //--------modify by xiejp 根据StartAge-EndAge求出所有的--

               string commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", benMAPPopulation.DataSetID);// benMAPPopulation.Year);
               int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
               if (CommonClass.MainSetup.SetupID != 1) commonYear = benMAPPopulation.Year;
               commandText = "";
               string strwhere = "";
               if (CommonClass.MainSetup.SetupID == 1)
                   strwhere = "where AGERANGEID!=42";
               else
                   strwhere = " where 1=1 ";
               string ageCommandText = string.Format("select b.* from PopulationConfigurations a, Ageranges b   where a.PopulationConfigurationID=b.PopulationConfigurationID and a.PopulationConfigurationID=(select PopulationConfigurationID from PopulationDatasets where PopulationDataSetID=" + benMAPPopulation.DataSetID + ")");// + strwhere);
               //if (crSelectFunction.StartAge != -1)
               //{
               //    ageCommandText = string.Format(ageCommandText + " and b.EndAge>={0} ", crSelectFunction.StartAge);
               //}
               //if (crSelectFunction.EndAge != -1)
               //{
               //    ageCommandText = string.Format(ageCommandText + " and b.StartAge<={0} ", crSelectFunction.EndAge);
               //}
               DataSet dsage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, ageCommandText);
               string strsumage = "";
               string strsumageGrowth = "";
               foreach (DataRow dr in dsage.Tables[0].Rows)
               {
                   //-------如果StartAge在区间内--，则sum---不然--考虑-1的情况------------
                   if (strsumageGrowth == "")
                       strsumageGrowth = dr["AgerangeID"].ToString();
                   else
                       strsumageGrowth = strsumageGrowth + "," + dr["AgerangeID"].ToString();
                   if ((Convert.ToInt32(dr["StartAge"]) >= crSelectFunction.StartAge || crSelectFunction.StartAge == -1) && (Convert.ToInt32(dr["EndAge"]) <= crSelectFunction.EndAge || crSelectFunction.EndAge == -1))
                   {
                       //sum
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

                       //-------------------修正---如果setup=1 ,且gridtype=county，则直接和Growth关联，如果Setup!=1,commonYear=benMapPopulationYear--------------
                       if (benMAPPopulation.GridType.GridDefinitionID == 1 && CommonClass.MainSetup.SetupID == 1 && commonYear != benMAPPopulation.Year)
                       {
                           commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue)*" + dDiv + " as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
                               "  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);



                       }
                       else if ((benMAPPopulation.GridType.GridDefinitionID == 28 || benMAPPopulation.GridType.GridDefinitionID == 27) && CommonClass.MainSetup.SetupID == 1 && commonYear != benMAPPopulation.Year)
                       {
                           //commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue*c.percentage*" + dDiv + ") as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b ," +
                           //    " (select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=22 and normalizationstate in (0,1)) c "+
                           //    "  where a.CColumn=c.sourcecolumn and a.Row=c.sourcerow  and b.CColumn= c.TargetColumn and b.Row= c.TargetRow and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);

                           commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue*c.VValue*" + dDiv + ") as VValue   from PopulationEntries a,PopulationEntries b ," +
                               " PopulationGrowthWeights c   where  b.PopulationDatasetID=2 and b.YYear={2} and a.RaceID=c.RaceID and  a.EthnicityID=c.EthnicityID and a.CColumn= c.TargetColumn  " +
" and a.Row=c.Targetrow  and b.CColumn= c.SourceColumn and b.Row= c.SourceRow and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear, CommonClass.BenMAPPopulation.Year);// benMAPPopulation.Year);

                       }
                       else
                       {
                           commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue)*" + dDiv + " as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);
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
                   //commandText =commandText+ string.Format("select   a.CColumn,a.Row,sum(a.vvalue) as VValue   from PopulationEntries a   where       a.PopulationDatasetID={0} and YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);
                   //-------------------修正---如果setup=1 ,且gridtype=county，则直接和Growth关联，如果Setup!=1,commonYear=benMapPopulationYear--------------
                   if (benMAPPopulation.GridType.GridDefinitionID == 1 && CommonClass.MainSetup.SetupID == 1 && commonYear != benMAPPopulation.Year)
                   {
                       commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.VValue) as VValue   from PopulationEntries a,(select CColumn,Row,VValue,AgerangeID,RaceID,EthnicityID,GenderID from PopulationEntries where PopulationDatasetID=2 and YYear=" + benMAPPopulation.Year + ") b " +
                           "  where a.CColumn=b.CColumn and a.Row=b.Row and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);


                   }
                   else if ((benMAPPopulation.GridType.GridDefinitionID == 28 || benMAPPopulation.GridType.GridDefinitionID == 27) && CommonClass.MainSetup.SetupID == 1 && commonYear != benMAPPopulation.Year)
                   {
                       commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue*b.vvalue*c.VValue) as VValue   from PopulationEntries a,PopulationEntries b ," +
                             " PopulationGrowthWeights c   where  b.PopulationDatasetID=2 and b.YYear={2} and a.RaceID=c.RaceID and  a.EthnicityID=c.EthnicityID and a.CColumn= c.TargetColumn  " +
" and a.Row=c.Targetrow  and b.CColumn= c.SourceColumn and b.Row= c.SourceRow and a.AgerangeID=b.AgerangeID and a.RaceID=b.RaceID and a.EthnicityID=b.EthnicityID and a.GenderID=b.GenderID and  a.PopulationDatasetID={0} and a.YYear={1}", benMAPPopulation.DataSetID, commonYear, CommonClass.BenMAPPopulation.Year);// benMAPPopulation.Year);

                   }
                   else
                   {
                       commandText += string.Format("select   a.CColumn,a.Row,sum(a.vvalue) as VValue   from PopulationEntries a  where   a.PopulationDatasetID={0} and YYear={1}", benMAPPopulation.DataSetID, commonYear);// benMAPPopulation.Year);
                   }
                   commandText = string.Format(commandText + " and a.AgerangeID in ({0}) ", strsumage);

                   if (!string.IsNullOrEmpty(crSelectFunction.Race) && crSelectFunction.Race.Trim().ToLower()!="all")
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
              // if (((benMAPPopulation.GridType.GridDefinitionID == 13 || benMAPPopulation.GridType.GridDefinitionID == 7) && CommonClass.MainSetup.SetupID == 1))//&& commonYear != benMAPPopulation.Year))
               if(1==1)
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
                       //fbDataReader.Close();
                      
                       if (DicGrowth == null && CommonClass.BenMAPPopulation.Year!=commonYear)
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
                       string strWeight = "select * from PopulationGrowthWeights where PopulationDataSetID=" + benMAPPopulation.DataSetID + " and YYear="+commonYear;


                       //Dictionary<
                       //Dictionary<string, double> dicGrowth = new Dictionary<string, double>();
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
                               //如果没有pop growth weight,就看有没有人口网格到county的grid percentage
                               string str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + benMAPPopulation.GridType.GridDefinitionID + " and  targetgriddefinitionid =18 ) and normalizationstate in (0,1)";
                               DataSet dsPercentage = null;
                               try
                               {
                                   dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
                                   if (dsPercentage.Tables[0].Rows.Count == 0)
                                   {
                                       //如果没有percentage,就再算一下，当作weight来用。（griddefinition为18是county）
                                       Configuration.ConfigurationCommonClass.creatPercentageToDatabase(18, benMAPPopulation.GridType.GridDefinitionID);
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
                       //-------如果StartAge在区间内--，则sum---不然--考虑-1的情况------------
                       if ((Convert.ToInt32(dr["StartAge"]) >= crSelectFunction.StartAge || crSelectFunction.StartAge == -1) && (Convert.ToInt32(dr["EndAge"]) <= crSelectFunction.EndAge || crSelectFunction.EndAge == -1))
                       {
                           //sum
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
                           //-----------为了修正数据全部置1
                           //dicAge.Add(dr["AgeRangeID"].ToString(), dDiv);
                           dicAge.Add(dr["AgeRangeID"].ToString(), 1);
                       }
                   }
                   dsage.Dispose();

                   string strPop = "select * from PopulationEntries where PopulationDataSetID=" + benMAPPopulation.DataSetID + " and YYear="+commonYear+" and AgeRangeID in (" + sAge + ")";
                   fbDataReader = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, strPop);
                   double d = 0;
                   while (fbDataReader.Read())
                   {
                       //if (!dicPop.ContainsKey(fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString() + "," + fbDataReader["EthnicityID"].ToString() + "," +
                       //    fbDataReader["RaceID"].ToString() + "," + fbDataReader["GenderID"].ToString() + "," + fbDataReader["AgeRangeID"].ToString()))
                       //    dicPop.Add(fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString() + "," + fbDataReader["EthnicityID"].ToString() + "," +
                       //        fbDataReader["RaceID"].ToString() + "," + fbDataReader["GenderID"].ToString() + "," + fbDataReader["AgeRangeID"].ToString(), Convert.ToDouble(fbDataReader["VValue"]));
                       //else
                       //    dicPop[fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString() + "," + fbDataReader["EthnicityID"].ToString() + "," +
                       //    fbDataReader["RaceID"].ToString() + "," + fbDataReader["GenderID"].ToString() + "," + fbDataReader["AgeRangeID"].ToString()] = dicPop[fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString() + "," + fbDataReader["EthnicityID"].ToString() + "," +
                       //    fbDataReader["RaceID"].ToString() + "," + fbDataReader["GenderID"].ToString() + "," + fbDataReader["AgeRangeID"].ToString()] + Convert.ToDouble(fbDataReader["VValue"]);
                       d = 0;//Convert.ToDouble(fbDataReader["VValue"]);
                       //string[] sArray = null;
                       char[] c = new char[] { ',' };
                       if (DicWeight!=null && DicWeight.ContainsKey(fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString()) && dicAge.ContainsKey(fbDataReader["AgeRangeID"].ToString()) && DicGrowth != null && DicGrowth.Count > 0)
                       {
                           string se = fbDataReader["EthnicityID"].ToString(), sr = fbDataReader["RaceID"].ToString(),sg = fbDataReader["GenderID"].ToString(),
                            sga = fbDataReader["GenderID"].ToString() + "," + fbDataReader["AgeRangeID"], sa = fbDataReader["AgeRangeID"].ToString();
                           foreach (KeyValuePair<string, WeightAttribute> k in DicWeight[fbDataReader["CColumn"].ToString() + "," + fbDataReader["Row"].ToString()])
                           {
                               //sArray = k.Key.Split(c);
                               if (k.Value.EthnicityID == se && k.Value.RaceID == sr && DicGrowth.ContainsKey(k.Key + "," + sga)
                                   && (RaceID == -1 || RaceID.ToString() == sr)
                                   && (GenderID == -1 || GenderID.ToString() == sg)
                                   && (EthnicityID == -1 || EthnicityID.ToString() == se)
                                   )
                                   d += Convert.ToDouble(fbDataReader["VValue"]) * DicGrowth[k.Key + "," + sga] * k.Value.Value * dicAge[sa];


                           }
                           //sArray = null;

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
                           if (      (RaceID == -1 || RaceID.ToString() == fbDataReader["RaceID"].ToString())
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
                       //PopulationAttribute pAttribute = new PopulationAttribute()
                       //{
                       //    Col = Convert.ToInt32(s[0]),
                       //    Row = Convert.ToInt32(s[1]),
                       //    Value = Convert.ToSingle(k.Value)
                       //};
                       dicPopulationAttribute.Add(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1]), k.Value);
                       //lstPopulationAttribute.Add(pAttribute);
                   }
                   dicPop12 = dicPopulationAttribute;
                   diclstPopulationAttribute = null;
                   //commandText=getPopulationComandTextFromCRSelectFunctionForPop(crSelectFunction, benMAPPopulation, dicRace, dicEthnicity, dicGender);
               }
               else
               {
                   //DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

                   FbDataReader fbDataReader2 = fb.ExecuteReader(CommonClass.Connection, CommandType.Text, commandText);

                   while (fbDataReader2.Read())
                   {
                       // PopulationAttribute pAttribute = new PopulationAttribute()
                       //{
                       //    Col = Convert.ToInt32(fbDataReader2["CColumn"]),
                       //    Row = Convert.ToInt32(fbDataReader2["Row"]),
                       //    Value = Convert.ToSingle(fbDataReader2["VValue"])
                       //};
                       //lstPopulationAttribute.Add(pAttribute);
                       diclstPopulationAttribute.Add(fbDataReader2["CColumn"].ToString() + "," + fbDataReader2["Row"], Convert.ToSingle(fbDataReader2["VValue"]));
                       dicPopulationAttribute.Add(Convert.ToInt32(fbDataReader2["CColumn"]) * 10000 + Convert.ToInt32(fbDataReader2["Row"]), Convert.ToSingle(fbDataReader2["VValue"]));
                        
                       //------------

                   }
                   dicPop12 = dicPopulationAttribute;
               }
               //-------------------得到一个已经转好坐标系的dicPop12
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
                           creatPercentageToDatabase(CommonClass.GBenMAPGrid.GridDefinitionID, (benMAPPopulation.GridType.GridDefinitionID == 28 ? 27 : benMAPPopulation.GridType.GridDefinitionID));
                           dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
                       }
                       foreach (DataRow dr in dsPercentage.Tables[0].Rows)
                       {
                           if (dicRelationShipForAggregation.ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
                           {
                               if (!dicRelationShipForAggregation[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].ContainsKey(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()))
                                   dicRelationShipForAggregation[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                               //(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString()
                           }
                           else
                           {
                               dicRelationShipForAggregation.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), new Dictionary<string, double>());
                               dicRelationShipForAggregation[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                               //dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
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
                                       dicPopulationAgeAggregation[kin.Key + "," + s[2]] +=Convert.ToSingle( k.Value * kin.Value);
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
                   if (benMAPPopulation.GridType.GridDefinitionID == gridRelationShipPopulation.bigGridID)//Population比较大
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
                                       //lstResult.Add(new PopulationAttribute()
                                       //{
                                       //    Col = rc.Col,
                                       //    Row = rc.Row,
                                       //    Value = diclstPopulationAttribute[gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row] / Convert.ToSingle(gra.smallGridRowCol.Count)//lstPopulationAttribute.Where(p => p.Col == gra.bigGridRowCol.Col && p.Row == gra.bigGridRowCol.Row).Select(a => a.Value).Sum() / gra.smallGridRowCol.Count
                                       //});
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
                                       //lstResult.Add(new PopulationAttribute()
                                       //{
                                       //    Col = rc.Col,
                                       //    Row = rc.Row,
                                       //    Value = diclstPopulationAttribute[gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row] / Convert.ToSingle(gra.smallGridRowCol.Count)//lstPopulationAttribute.Where(p => p.Col == gra.bigGridRowCol.Col && p.Row == gra.bigGridRowCol.Row).Select(a => a.Value).Sum() / gra.smallGridRowCol.Count
                                       //});
                                       dicPopulationAttributeReturn.Add(rc.Col * 10000 + rc.Row, dicPopulationAttribute[gra.bigGridRowCol.Col * 10000 + gra.bigGridRowCol.Row] / Convert.ToSingle(gra.smallGridRowCol.Count));
                                   }
                               }

                           }
                       }
                   }
                   else//网格类型比较大
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
                                   //lstResult.Add(new PopulationAttribute()
                                   //{
                                   //    Col = rc.Col,
                                   //    Row = rc.Row,
                                   //    Value = diclstPopulationAttribute[gra.bigGridRowCol.Col + "," + gra.bigGridRowCol.Row] / Convert.ToSingle(gra.smallGridRowCol.Count)//lstPopulationAttribute.Where(p => p.Col == gra.bigGridRowCol.Col && p.Row == gra.bigGridRowCol.Row).Select(a => a.Value).Sum() / gra.smallGridRowCol.Count
                                   //});
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
               //float dd = lstResult.Sum(p => p.Value);
               //return lstResult;
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
               //----直接修改为基准年为2000---------------------如果Setup不是美国区，则这部分需要修正
               //--------modify by xiejp 根据StartAge-EndAge求出所有的--
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
                       commandTextLevel = "insert into t_poplevel values("+i +"," + k.Key.Row + "," + k.Key.Col + "," + k.Value + ")";
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
               //--
//               AGERANGEID	STARTAGE	ENDAGE	VVALUE
//1	0	0	0.0136931743472815
//2	1	4	0.0544440671801567
//3	5	9	0.0730041638016701
//4	10	14	0.072923868894577
//5	15	19	0.0718525871634483
//6	20	24	0.0673884674906731
//7	25	29	0.068867988884449
//8	30	34	0.0728825107216835
//9	35	39	0.0806736126542091
//10	40	44	0.0797196552157402
//11	45	49	0.0713507384061813
//12	50	54	0.0624626986682415
//13	55	59	0.0478613935410976
//14	60	64	0.0384204462170601
//15	65	69	0.0339006930589676
//16	70	74	0.0314938016235828
//17	75	79	0.0263733938336372
//18	80	84	0.0175950452685356
//19	85	99	0.0150916986167431

               //return dicReturn;
           }
           catch
           {
               //return null;
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

  int iStartAge =  crSelectFunction.StartAge ;
  if (iStartAge == -1) iStartAge = 0;
                   int iEndAge =crSelectFunction.EndAge;
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
                   return Convert.ToSingle( dpop);
       }
       public static Dictionary<int, double> getIncidenceDataSetFromCRSelectFuntionDicold(Dictionary<int, double> dicPopulation, Dictionary<string, double> dicPopulationAge, Dictionary<int, double> dicPopulation12, CRSelectFunction crSelectFunction, bool bPrevalence, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, int GridDefinitionID, GridRelationship gridRelationShipPopulation)
       {
           try
           {
               //getIncidenceLevelFromDatabase();

               // FbConnection _connection = CommonClass.getNewConnection();
              //  getIncidenceLevelFromDatabase();
              //  double dpoplevel = getPopLevelFromCR(crSelectFunction);
               Dictionary<int, double> dicIncidenceRateAttribute = new Dictionary<int, double>();
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               //首先通过crfunction 的metric 筛选所有的base,control,deltaq->
               DataSet dsIncidence = null;
               DataSet dsPrevalence = null;
               //--------------------
               string strbPrevalence = "F";
               int iid = crSelectFunction.IncidenceDataSetID;
               if (bPrevalence)
               {
                   strbPrevalence = "T";
                   iid = crSelectFunction.PrevalenceDataSetID;
               }
               //------------------modify by xiejp----------------先求出该EndPointGroup的年龄段--直接计算常量-----------
               //----求出可能的年龄段以及权重保存起来，然后用于SQL-------
               string commandText = "";
               
               //double dPopLevel = getPopLevelFromCR(crSelectFunction);
               //------------得到populationCommandText,首先从IncidenceDataSet中得到他的GridTypeID,以及SetupID,得到populationDataSetID------------
              int iPopulationDataSetID=Convert.ToInt32( fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, string.Format("select PopulationDataSetID from PopulationDataSets where SetupID={0} and GridDefinitionID= (select GridDefinitionID from IncidenceDataSets where IncidenceDataSetID={1} )", CommonClass.MainSetup.SetupID, iid)));
              int iPopulationDataSetGridID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, string.Format("select GridDefinitionID from IncidenceDataSets where IncidenceDataSetID={1} ", CommonClass.MainSetup.SetupID, iid)));

              BenMAPPopulation benMAPPopulation = new BenMAPPopulation() { DataSetID = iPopulationDataSetID, GridType = new BenMAPGrid() { GridDefinitionID = iPopulationDataSetGridID }, Year = CommonClass.BenMAPPopulation.Year };
              commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", iPopulationDataSetID);// benMAPPopulation.Year);
              int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
              string populationCommandText = getPopulationComandTextFromCRSelectFunction(crSelectFunction, benMAPPopulation, dicRace, dicEthnicity, dicGender);
              commandText = "";
                   //-----------得到该StartAge-->EndAge的Population------------

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
                       //---------求出所占比例-----------
                       //----------第一种StartAge,EndAge都不在此段中 *1--------------------
                       if (commandText != "") commandText = commandText + " union ";
                       if ((crSelectFunction.StartAge < Convert.ToInt32(dr["StartAge"]) || crSelectFunction.StartAge==-1) &&( crSelectFunction.EndAge > Convert.ToInt32(dr["EndAge"])|| crSelectFunction.EndAge==-1))
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
                       //----------2 StartAge,EndAge都在此段中--------------------------
                       else if (crSelectFunction.StartAge >= Convert.ToInt32(dr["StartAge"]) && crSelectFunction.EndAge <= Convert.ToInt32(dr["EndAge"]))
                       {
                           CRSelectFunction cr = new CRSelectFunction() { StartAge = crSelectFunction.StartAge, EndAge = crSelectFunction.EndAge, Ethnicity = crSelectFunction.Ethnicity, Gender = crSelectFunction.Gender, Race = crSelectFunction.Race };
                           //if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4)
                           //{
                           //    populationCommandText = getPopulationComandTextFrom12kmToCounty(cr, CommonClass.BenMAPPopulation, dicRace, dicEthnicity, dicGender);
                           //}
                           //else
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
                               populationCommandText = getPopulationComandTextFromCRSelectFunction(cr, benMAPPopulation, dicRace, dicEthnicity, dicGender);//* " + Convert.ToDouble((crSelectFunction.EndAge - crSelectFunction.StartAge)) / Convert.ToDouble((Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]))) + "

                               commandText = commandText + string.Format("select  a.IncidenceRateID,a.CColumn,a.Row,a.VValue*d.VValue as VValue  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c,(" + populationCommandText + ") d  where  d.CColumn=a.CColumn and a.Row=d.Row and " +
           " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100)" + " and b.Prevalence='" + strbPrevalence + "' " +
           "  and c.IncidenceDatasetID={0} and b.StartAge={1} and b.EndAge={2} ", iid, Convert.ToInt32(dr["StartAge"]), Convert.ToInt32(dr["EndAge"]));
                           }
                       }
                       //----------3 StartAge在此段中-----------------------------------
                       else if (crSelectFunction.StartAge >= Convert.ToInt32(dr["StartAge"]) &&( crSelectFunction.EndAge > Convert.ToInt32(dr["EndAge"])||crSelectFunction.EndAge==-1))
                       {
                           CRSelectFunction cr = new CRSelectFunction() { StartAge = crSelectFunction.StartAge, EndAge = Convert.ToInt32(dr["EndAge"]), Ethnicity = crSelectFunction.Ethnicity, Gender = crSelectFunction.Gender, Race = crSelectFunction.Race };
                           //if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4)
                           //{
                           //    populationCommandText = getPopulationComandTextFrom12kmToCounty(cr, CommonClass.BenMAPPopulation, dicRace, dicEthnicity, dicGender);
                           //}
                           //else
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
                               populationCommandText = getPopulationComandTextFromCRSelectFunction(cr, benMAPPopulation, dicRace, dicEthnicity, dicGender);//* " + Convert.ToDouble((crSelectFunction.EndAge - crSelectFunction.StartAge)) / Convert.ToDouble((Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]))) + "

                               commandText = commandText + string.Format("select  a.IncidenceRateID,a.CColumn,a.Row,a.VValue*d.VValue as VValue  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c,(" + populationCommandText + ") d  where  d.CColumn=a.CColumn and a.Row=d.Row and " +
          " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100)" + " and b.Prevalence='" + strbPrevalence + "' " +
          "  and c.IncidenceDatasetID={0} and b.StartAge={1} and b.EndAge={2} ", iid, Convert.ToInt32(dr["StartAge"]), Convert.ToInt32(dr["EndAge"]));
                           }
                       }
                       //----------4 EndAge在此段中--------------------------------------
                       else if ((crSelectFunction.StartAge < Convert.ToInt32(dr["StartAge"]) || crSelectFunction.StartAge==-1)&& crSelectFunction.EndAge <= Convert.ToInt32(dr["EndAge"]))
                       {
                           CRSelectFunction cr = new CRSelectFunction() { StartAge = Convert.ToInt32(dr["StartAge"]), EndAge = crSelectFunction.EndAge, Ethnicity = crSelectFunction.Ethnicity, Gender = crSelectFunction.Gender, Race = crSelectFunction.Race };
                           //if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4)
                           //{
                           //    populationCommandText = getPopulationComandTextFrom12kmToCounty(cr, CommonClass.BenMAPPopulation, dicRace, dicEthnicity, dicGender);
                           //}
                           //else
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
                               populationCommandText = getPopulationComandTextFromCRSelectFunction(cr, benMAPPopulation, dicRace, dicEthnicity, dicGender);//* " + Convert.ToDouble((crSelectFunction.EndAge - crSelectFunction.StartAge)) / Convert.ToDouble((Convert.ToInt32(dr["EndAge"]) - Convert.ToInt32(dr["StartAge"]))) + "

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
                
               //              //--------------求CommondText---------------------------------------
               //          }
              
               //----------------------------------------------------------------
               //Dictionary<string, double> dicPopLevel = new Dictionary<string, double>();

     //          string commandText = string.Format("select  a.IncidenceRateID,a.CColumn,a.Row,a.VValue,b.StartAge,b.EndAge,b.RaceID,b.EthnicityID,b.GenderID from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c where" +
     //" a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100) and b.Prevalence='" + strbPrevalence + "' " +
     //          string commandText = string.Format("select  a.IncidenceRateID,a.CColumn,a.Row,a.VValue * d.VValue as VValue  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c,t_popLevel d where d.AgeRangeID =-1 and b.StartAge= d.StartAge and b.EndAge=d.EndAge and " +
     // " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and b.Prevalence='" + strbPrevalence + "' " +

     //" and c.IncidenceDatasetID={0}  and b.StartAge<={1} and b.EndAge>={2}", iid, crSelectFunction.EndAge, crSelectFunction.StartAge);
             
               //commandText = "select a.CColumn,a.Row,sum(a.VValue)/"+dPopLevel +" as VValue from ( " + commandText + " ) a group by a.CColumn,a.Row";
                   //if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4)
                   //{
                   //    populationCommandText = getPopulationComandTextFrom12kmToCounty(crSelectFunction, CommonClass.BenMAPPopulation, dicRace, dicEthnicity, dicGender);
                   //}
                   //else   
                   
                   if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4)
                   {
                       populationCommandText = getPopulationComandTextFromCRSelectFunction(crSelectFunction, CommonClass.BenMAPPopulation, dicRace, dicEthnicity, dicGender);
                   }
                   else
                   {
                       populationCommandText = getPopulationComandTextFromCRSelectFunction(crSelectFunction, benMAPPopulation, dicRace, dicEthnicity, dicGender);
                   }
               commandText = "select a.CColumn,a.Row,sum(a.VValue/b.VValue)  as VValue from ( " + commandText + " ) a,("+populationCommandText+") b where a.CColumn=b.CColumn and a.Row=b.Row group by a.CColumn,a.Row";
               Dictionary<string, double> dicPercentage = new Dictionary<string, double>();
               //Dictionary<string, int> dicSourceTarget = new Dictionary<string, int>();
               Dictionary<string, Dictionary<string, double>> dicRelationShip = new Dictionary<string, Dictionary<string, double>>();
               if ((CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 28 || CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 27) && CommonClass.MainSetup.SetupID == 1)
               {
                   string str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=77 and normalizationstate in (0,1)";
                  DataSet dsPercentage= fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);

                  foreach (DataRow dr in dsPercentage.Tables[0].Rows)
                  {
                      if (dicRelationShip.ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
                      {
                          if (!dicRelationShip[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].ContainsKey(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()))
                              dicRelationShip[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add((Convert.ToInt32(dr["targetcolumn"]) * 10000 + Convert.ToInt32(dr["targetrow"].ToString())).ToString(), Convert.ToDouble(dr["Percentage"]));
                          //(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString()
                      }
                      else
                      {
                          dicRelationShip.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), new Dictionary<string, double>());
                          dicRelationShip[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add((Convert.ToInt32(dr["targetcolumn"]) * 10000 + Convert.ToInt32(dr["targetrow"].ToString())).ToString(), Convert.ToDouble(dr["Percentage"]));
                          //dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                      }

                  }
                  foreach (DataRow dr in dsPercentage.Tables[0].Rows)
                  {
                      dicPercentage.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString() + "," + dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["percentage"]));
                      //if (!dicSourceTarget.ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
                      //{
                      //    dicSourceTarget.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToInt32(dr["targetcolumn"]) * 10000 + Convert.ToInt32(dr["targetrow"]));
                      //}
                  }
                  dsPercentage.Dispose();
               }
               if(dsAge!=null)
               dsAge.Dispose();
               if(dsIncidence!=null)
               dsIncidence.Dispose();
               if(dsPrevalence!=null)
               dsPrevalence.Dispose();
               //------------------------modify by xiejp for Inc-------------------------------------
               if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && commonYear != benMAPPopulation.Year && dicPopulationAge != null && dicPopulationAge.Count>0)
               {
                   string strEndAgeOri = " CASE" +
                      " WHEN (b.EndAge> " + crSelectFunction.EndAge + ") THEN " + crSelectFunction.EndAge + " ELSE b.EndAge END ";
                   string strStartAgeOri = " CASE" +
                       " WHEN (b.StartAge< " + crSelectFunction.StartAge + ") THEN " + crSelectFunction.StartAge + " ELSE b.StartAge END ";
                   string strAgeID =string.Format( " select a.startAge,a.EndAge,b.AgeRangeid, " +
 " CASE" +
 " WHEN (b.startAge>=a.StartAge and b.EndAge<=a.EndAge) THEN 1" +
 " WHEN (b.startAge<a.StartAge and b.EndAge<=a.EndAge) THEN  Cast(({1}-a.StartAge+1) as float)/({1}-{0}+1)" + //加上对本身startAge,EndAge的考虑---
 " WHEN (b.startAge<a.StartAge and b.EndAge>a.EndAge) THEN Cast(({1}-{0}+1) as float)/({1}-{0}+1)" +
 "  WHEN (b.startAge>=a.StartAge and b.EndAge>a.EndAge) THEN Cast((a.EndAge-{0}+1) as float)/({1}-{0}+1)" +
 " ELSE 1" +
 " END as weight,b.StartAge as sourceStartAge,b.EndAge as SourceEndAge" +
 "  from ( select distinct startage,endage from Incidencerates )a,ageranges b" +
 " where b.EndAge>=a.StartAge and b.StartAge<=a.EndAge", strStartAgeOri, strEndAgeOri);

                  
               //    string strInc = string.Format("select  a.CColumn,a.Row,a.VValue as VValue,d.AgeRangeID  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c ,(" + strAgeID +
               //        ") d where   b.StartAge=d.StartAge and b.EndAge=d.EndAge and " +
               //" a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100)" + " and b.Prevalence='" + strbPrevalence + "' " +
               //"  and c.IncidenceDatasetID={0} and b.StartAge<={2} and b.EndAge>={1} ", iid, crSelectFunction.StartAge, crSelectFunction.EndAge);

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
                  
                   foreach ( KeyValuePair<string,double> k in dicPopulationAge )
                   {
                       string[] s = k.Key.Split(new char[] { ',' });
                       double dp = 0;

                       if (dicRelationShip.ContainsKey(s[0] + "," + s[1]))//&& dicInc.ContainsKey(dicSourceTarget[dr["CColumn"].ToString() + "," + dr["Row"]] + "," + dr["AgeRangeID"]))
                       {
                           foreach (KeyValuePair<string, double> kin in dicRelationShip[s[0] + "," + s[1]])
                           {
                               dp += dicInc[kin.Key + "," + s[2]] * kin.Value;
                           }
                           //dp = dicInc[dicSourceTarget[dr["CColumn"].ToString() + "," + dr["Row"]] + "," + dr["AgeRangeID"]];
                       }


                       if (dicPopInc.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])) && dicPopulation12.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                       {

                           dicPopInc[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])] += dp * Convert.ToDouble(k.Value) / dicPopulation12[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])];

                       }
                       else
                       {
                           dicPopInc.Add(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1]), dp * Convert.ToDouble(k.Value) / dicPopulation12[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])]);
                       }

                       //}
                       //if(dicIncidenceRateAttribute.ContainsKey
                       //dicIncidenceRateAttribute.Add(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"]), Convert.ToDouble(dr["VValue"]));
                   }
                   dicIncidenceRateAttribute = dicPopInc;
                   //return dicIncidenceRateAttribute;
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
                   //
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

                   //     strAgeID = string.Format("select b.* from (select distinct StartAge,EndAge from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c  where   " +
                   //" a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100)" + " and b.Prevalence='" + strbPrevalence + "' " +
                   //"  and c.IncidenceDatasetID={0} and b.StartAge<={2} and b.EndAge>={1} ) a,(" + strAgeID + ") b where a.StartAge=b.StartAge", iid, crSelectFunction.StartAge, crSelectFunction.EndAge);
                   //     DataSet dsAgeID = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, strAgeID);
                   //     Dictionary<string, Dictionary<string, double>> dicAgeID = new Dictionary<string, Dictionary<string, double>>();
                   //     foreach (DataRow dr in dsAgeID.Tables[0].Rows)
                   //     {
                   //         if (dicAgeID.ContainsKey(dr["AgeRangeid"].ToString()))
                   //         {
                   //             dicAgeID[dr["AgeRangeid"].ToString()].Add(dr["startAge"] + "," + dr["EndAge"], Convert.ToDouble(dr["weight"]));
                   //         }
                   //         else
                   //         {
                   //             dicAgeID.Add(dr["AgeRangeid"].ToString(), new Dictionary<string, double>());
                   //             dicAgeID[dr["AgeRangeid"].ToString()].Add(dr["startAge"] + "," + dr["EndAge"], Convert.ToDouble(dr["weight"]));
                   //         }
                   //         //dicAgeID.Add(dr["startAge"] + "," + dr["EndAge"] + "," + dr["AgeRangeid"], Convert.ToDouble(dr["weight"]));
                   //     }
                   double dp = 0;
                   Dictionary<int, double> dicPopInc = new Dictionary<int, double>();
                   foreach (DataRow dr in dsPopInc.Tables[0].Rows)
                   {
                       dp = 0;
                       if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4)
                       {
                           if (dicRelationShip.ContainsKey(dr["CColumn"].ToString() + "," + dr["Row"]))//&& dicInc.ContainsKey(dicSourceTarget[dr["CColumn"].ToString() + "," + dr["Row"]] + "," + dr["AgeRangeID"]))
                           {
                               foreach (KeyValuePair<string, double> k in dicRelationShip[dr["CColumn"].ToString() + "," + dr["Row"]])
                               {
                                   dp += dicInc[k.Key + "," + dr["AgeRangeID"]] * k.Value;
                               }
                               //dp = dicInc[dicSourceTarget[dr["CColumn"].ToString() + "," + dr["Row"]] + "," + dr["AgeRangeID"]];
                           }
                       }
                       else
                       {
                           // if (dicInc.ContainsKey(dicSourceTarget[dr["CColumn"].ToString() + "," + dr["Row"]] + "," + dr["AgeRangeID"]))
                           dp = dicInc[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"]) + "," + dr["AgeRangeID"]];
                       }

                       //if (dicAgeID.ContainsKey(dr["AgeRangeid"].ToString()))
                       //{
                       //    Dictionary<string, double> dicTemp = dicAgeID[dr["AgeRangeid"].ToString()];
                       //    foreach (KeyValuePair<string, double> k in dicTemp)
                       //    {
                       //        dp += dicInc[k.Key + "," + dr["CColumn"].ToString() + "," + dr["Row"]] * k.Value;
                       //    }
                       if (dicPopInc.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && dicPopulation.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])))
                       {
                           if (CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 27 || CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 28)
                           {
                               if (dicPopulation12.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && dicPopInc.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && dicPopulation12[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])]!=0)
                               dicPopInc[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])] += dp * Convert.ToDouble(dr["VValue"]) / dicPopulation12[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])];
                           }
                           else if (dicPopInc.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])))
                           dicPopInc[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])] += dp * Convert.ToDouble(dr["VValue"]) / dicPopulation[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])];

                       }
                       else
                       {
                           if (CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 27 || CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 28)
                           {
                               if (dicPopulation12.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && !dicPopInc.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && dicPopulation12[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])]!=0)
                               dicPopInc.Add(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"]), dp * Convert.ToDouble(dr["VValue"]) / dicPopulation12[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])]);
                           }
                           else if (dicPopulation.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && !dicPopInc.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])))
                           dicPopInc.Add(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"]), dp * Convert.ToDouble(dr["VValue"]) / dicPopulation[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])]);
                       }
                       if (dicPopInc.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])) && double.IsNaN(dicPopInc[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])]))
                       { 
                       }
                       //}
                       //if(dicIncidenceRateAttribute.ContainsKey
                       //dicIncidenceRateAttribute.Add(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"]), Convert.ToDouble(dr["VValue"]));
                   }
                   dicIncidenceRateAttribute = dicPopInc;
                   //return dicIncidenceRateAttribute;
               }
               
              //--------------------------------------------------modify by xiejp for Inc-------------------------------------
               //commandText = "select a.CColumn,a.Row,sum(a.VValue)  as VValue from ( " + commandText + " ) a group by a.CColumn,a.Row";
               //dsIncidence = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
               ////Dictionary<int, double> dicPop = new Dictionary<int, double>();
               ////DataSet dsPop = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, populationCommandText);
               ////foreach (DataRow dr in dsPop.Tables[0].Rows)
               ////{
               ////    if (!dicPop.Keys.Contains(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])))
               ////        dicPop.Add(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"]), Convert.ToDouble(dr["VValue"]));
               ////}
               //foreach (DataRow dr in dsIncidence.Tables[0].Rows)
               //{
               //    //dicGender.Add(dr["GenderName"].ToString(), Convert.ToInt32(dr["GenderID"]));
               //    //---modify by xiejp --除以该网格总人口
               //    if (!dicIncidenceRateAttribute.Keys.Contains(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])))
               //    {
               //        dicIncidenceRateAttribute.Add(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"]), Convert.ToDouble(dr["VValue"]) );
                       
               //        //if (dicPop.ContainsKey(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])))
               //        //{
               //        //    dicIncidenceRateAttribute.Add(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"]), Convert.ToDouble(dr["VValue"]) / dicPop[Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])]);
 
               //        //}
               //    }
                       
               //    //    new IncidenceRateAttribute()
               //    //{
               //    //    Col = Convert.ToInt32(dr["CColumn"]),
               //    //    Row = Convert.ToInt32(dr["Row"]),
               //    //    Value = Convert.ToDouble(dr["VValue"])
               //    //    //,
               //    //    //IncidenceRateID = Convert.ToInt32(dr["IncidenceRateID"]),
               //    //    //EndAge = Convert.ToInt32(dr["EndAge"]),
               //    //    //EthnicityID = Convert.ToInt32(dr["EthnicityID"]),
               //    //    //GenderID = Convert.ToInt32(dr["GenderID"]),
               //    //    //RaceID = Convert.ToInt32(dr["RaceID"]),
               //    //    //StartAge = Convert.ToInt32(dr["StartAge"])
               //    //});

               //}

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
               Dictionary<int,double> dicResult =new  Dictionary<int,double>();
               double IncidenceValue = 0;
               if (incidenceDataSetGridType == GridDefinitionID)
               {
                   dicResult = dicIncidenceRateAttribute;
               }
               else if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && (CommonClass.GBenMAPGrid.GridDefinitionID== CommonClass.BenMAPPopulation.GridType.GridDefinitionID||( CommonClass.GBenMAPGrid.GridDefinitionID==27 &&CommonClass.BenMAPPopulation.GridType.GridDefinitionID==28) ||( CommonClass.GBenMAPGrid.GridDefinitionID==28 &&CommonClass.BenMAPPopulation.GridType.GridDefinitionID==27)))
               {
                   dicResult = dicIncidenceRateAttribute;
               }
               else
               {
                   //------------------------适应BenMAP4原有的空间关系----------------
                   dicRelationShip = APVX.APVCommonClass.getRelationFromDicRelationShipAll(gridRelationShipPopulation);
                   if (incidenceDataSetGridType == gridRelationShipPopulation.bigGridID)//Population比较大
                   {
                       if (dicRelationShip != null && dicRelationShip.Count > 0)
                       {
                           foreach (KeyValuePair<string,Dictionary<string, double>> k in dicRelationShip)
                           {
                               string[] s =k.Key.Split(new char[]{','});
                               if (dicIncidenceRateAttribute.Keys.Contains(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                               {
                                   foreach (KeyValuePair<string,double> rc in k.Value)
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
                   else//网格类型比较大
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
               if(dsIncidence!=null)
               dsIncidence.Dispose();
               //_connection.Close();
               //_connection.Close();
               return dicResult;
           }
           catch (Exception ex)
           {
               return null;
           }
           //return lstIncidenceRateAttribute;

       }
     
        public static Dictionary<int, double> getIncidenceDataSetFromCRSelectFuntionDic(Dictionary<int, double> dicPopulation, Dictionary<string, double> dicPopulationAge, Dictionary<int, double> dicPopulation12, CRSelectFunction crSelectFunction, bool bPrevalence, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, int GridDefinitionID, GridRelationship gridRelationShipPopulation)
       {
           try
           {
               //getIncidenceLevelFromDatabase();

               // FbConnection _connection = CommonClass.getNewConnection();
              //  getIncidenceLevelFromDatabase();
              //  double dpoplevel = getPopLevelFromCR(crSelectFunction);
               Dictionary<int, double> dicIncidenceRateAttribute = new Dictionary<int, double>();
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               //首先通过crfunction 的metric 筛选所有的base,control,deltaq->
               DataSet dsIncidence = null;
               DataSet dsPrevalence = null;
               //--------------------
               string strbPrevalence = "F";
               int iid = crSelectFunction.IncidenceDataSetID;
               if (bPrevalence)
               {
                   strbPrevalence = "T";
                   iid = crSelectFunction.PrevalenceDataSetID;
               }
               //------------------modify by xiejp----------------先求出该EndPointGroup的年龄段--直接计算常量-----------
               //----求出可能的年龄段以及权重保存起来，然后用于SQL-------
               string commandText = "";
               
               //double dPopLevel = getPopLevelFromCR(crSelectFunction);
               //------------得到populationCommandText,首先从IncidenceDataSet中得到他的GridTypeID,以及SetupID,得到populationDataSetID------------
              int iPopulationDataSetID=Convert.ToInt32( fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, string.Format("select PopulationDataSetID from PopulationDataSets where SetupID={0} and GridDefinitionID= (select GridDefinitionID from IncidenceDataSets where IncidenceDataSetID={1} )", CommonClass.MainSetup.SetupID, iid)));
              int iPopulationDataSetGridID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, string.Format("select GridDefinitionID from IncidenceDataSets where IncidenceDataSetID={1} ", CommonClass.MainSetup.SetupID, iid)));

              BenMAPPopulation benMAPPopulation = new BenMAPPopulation() { DataSetID = iPopulationDataSetID, GridType = new BenMAPGrid() { GridDefinitionID = iPopulationDataSetGridID }, Year = CommonClass.BenMAPPopulation.Year };
              commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", iPopulationDataSetID);// benMAPPopulation.Year);
              int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
              string populationCommandText = getPopulationComandTextFromCRSelectFunction(crSelectFunction, benMAPPopulation, dicRace, dicEthnicity, dicGender);
              commandText = "";
                   //-----------得到该StartAge-->EndAge的Population------------
               //----------------得到Incidence分年龄段的Sql-------------------------------------
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
" WHEN (b.startAge<a.StartAge and b.EndAge<=a.EndAge) THEN  Cast(({3}-{0}+1) as float)/({3}-{2}+1)" + //加上对本身startAge,EndAge的考虑---
" WHEN (b.startAge<a.StartAge and b.EndAge>a.EndAge) THEN Cast(({1}-{0}+1) as float)/({3}-{2}+1)" +
"  WHEN (b.startAge>=a.StartAge and b.EndAge>a.EndAge) THEN Cast(({1}-{2}+1) as float)/({3}-{2}+1)" +
" ELSE 1" +
" END as weight,b.StartAge as sourceStartAge,b.EndAge as SourceEndAge" +
"  from ( select distinct startage,endage from Incidencerates  where IncidenceDataSetID=" + iid + ")a,ageranges b" +
" where b.EndAge>=a.StartAge and b.StartAge<=a.EndAge", straStartAgeOri,straEndAgeOri,strbStartAgeOri,strbEndAgeOri);

            string strInc = string.Format("select  a.CColumn,a.Row,sum(a.VValue*d.Weight) as VValue,d.AgeRangeID  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c ,(" + strAgeID +
                 ") d where   b.StartAge=d.StartAge and b.EndAge=d.EndAge and " +
         " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + strRace+" and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100)" + " and b.Prevalence='" + strbPrevalence + "' " +
         "  and c.IncidenceDatasetID={0} and b.StartAge<={2} and b.EndAge>={1} group by a.CColumn,a.Row ,d.AgeRangeID", iid, crSelectFunction.StartAge, crSelectFunction.EndAge);
              //---------------End得到Incidence分年龄段的Sql-----------------------------------------------------------------------------------------------------------------
                  Dictionary<string, double> dicPercentage = new Dictionary<string, double>();
               //Dictionary<string, int> dicSourceTarget = new Dictionary<string, int>();
               Dictionary<string, Dictionary<string, double>> dicRelationShip = new Dictionary<string, Dictionary<string, double>>();
               if ((CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 28 || CommonClass.BenMAPPopulation.GridType.GridDefinitionID == 27) && CommonClass.MainSetup.SetupID == 1)
               {
                   string str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=77 and normalizationstate in (0,1)";
                  DataSet dsPercentage= fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);

                  foreach (DataRow dr in dsPercentage.Tables[0].Rows)
                  {
                      if (dicRelationShip.ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
                      {
                          if (!dicRelationShip[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].ContainsKey(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()))
                              dicRelationShip[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add((Convert.ToInt32(dr["targetcolumn"]) * 10000 + Convert.ToInt32(dr["targetrow"].ToString())).ToString(), Convert.ToDouble(dr["Percentage"]));
                          //(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString()
                      }
                      else
                      {
                          dicRelationShip.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), new Dictionary<string, double>());
                          dicRelationShip[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add((Convert.ToInt32(dr["targetcolumn"]) * 10000 + Convert.ToInt32(dr["targetrow"].ToString())).ToString(), Convert.ToDouble(dr["Percentage"]));
                          //dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                      }

                  }
                  foreach (DataRow dr in dsPercentage.Tables[0].Rows)
                  {
                      dicPercentage.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString() + "," + dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["percentage"]));
                      //if (!dicSourceTarget.ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
                      //{
                      //    dicSourceTarget.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToInt32(dr["targetcolumn"]) * 10000 + Convert.ToInt32(dr["targetrow"]));
                      //}
                  }
                  dsPercentage.Dispose();
               }
              
               if(dsIncidence!=null)
               dsIncidence.Dispose();
               if(dsPrevalence!=null)
               dsPrevalence.Dispose();
               //------------------------modify by xiejp for Inc-------------------------------------
               if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && dicPopulationAge != null && dicPopulationAge.Count > 0 ) // && commonYear != benMAPPopulation.Year
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

                           if (dicRelationShip.ContainsKey(s[0] + "," + s[1]))//&& dicInc.ContainsKey(dicSourceTarget[dr["CColumn"].ToString() + "," + dr["Row"]] + "," + dr["AgeRangeID"]))
                           {
                               foreach (KeyValuePair<string, double> kin in dicRelationShip[s[0] + "," + s[1]])
                               {
                                   dp += dicInc[kin.Key + "," + s[2]] * kin.Value;
                               }
                               //dp = dicInc[dicSourceTarget[dr["CColumn"].ToString() + "," + dr["Row"]] + "," + dr["AgeRangeID"]];
                           }


                           if (dicPopInc.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])) && dicPopulation12.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                           {

                               dicPopInc[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])] += dp * Convert.ToDouble(k.Value) / dicPopulation12[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])];

                           }
                           else
                           {
                               dicPopInc.Add(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1]), dp * Convert.ToDouble(k.Value) / dicPopulation12[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])]);
                           }

                           //}
                           //if(dicIncidenceRateAttribute.ContainsKey
                           //dicIncidenceRateAttribute.Add(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"]), Convert.ToDouble(dr["VValue"]));
                       }
                       dicIncidenceRateAttribute = dicPopInc;
                   }
                   else
                   {
                       //-------------首先求出Pop的Percentage---Pop-->Grid(36km,county ..) 2.Pop* Incidence 3./Pop
                       string str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =27 and  targetgriddefinitionid = "+ CommonClass.GBenMAPGrid.GridDefinitionID+" ) and normalizationstate in (0,1)";
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
                                   //(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString()
                               }
                               else
                               {
                                   dicRelationShipForAggregation.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), new Dictionary<string, double>());
                                   dicRelationShipForAggregation[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                                   //dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                               }

                           }
                           
                           dsPercentage.Dispose();

                           //Dictionary<string, double> dicPopulationAgeAggregation = new Dictionary<string, double>();
                           //foreach (KeyValuePair<string, double> k in  dicPopulationAge)
                           //{
                           //    string[] s = k.Key.Split(new char[] { ',' });
                           //    if (dicRelationShipForAggregation.ContainsKey(s[0] + "," + s[1]))
                           //    {
                           //        double dPop = 0;
                           //        foreach ( KeyValuePair <string, double> kin in dicRelationShipForAggregation[s[0] + "," + s[1]])
                           //        {
                           //            if (dicPopulationAgeAggregation.ContainsKey(kin.Key + "," + s[2]))
                           //            {
                           //                dicPopulationAgeAggregation[kin.Key + "," + s[2]] += k.Value * kin.Value;
                           //            }
                           //            else
                           //            {
                           //                dicPopulationAgeAggregation.Add(kin.Key + "," + s[2], k.Value * kin.Value);
                           //            }

                           //        }
 
                           //    }
                           //}

                           //---------get Percentage (Incidence and GridType)--
                           int iPercentageID=0;//
                           Dictionary<string, Dictionary<string, double>> dicPercentageForAggregationInc = new Dictionary<string, Dictionary<string, double>>();
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
                                       //(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString()
                                   }
                                   else
                                   {
                                       dicPercentageForAggregationInc.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), new Dictionary<string, double>());
                                       dicPercentageForAggregationInc[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                                       //dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
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
                                   if (dicPercentageForAggregationInc.ContainsKey(s[0] + "," + s[1]))//&& dicInc.ContainsKey(dicSourceTarget[dr["CColumn"].ToString() + "," + dr["Row"]] + "," + dr["AgeRangeID"]))
                                   {
                                       foreach (KeyValuePair<string, double> kin in dicPercentageForAggregationInc[s[0] + "," + s[1]])
                                       {
                                           string[] sin = kin.Key.Split(new char[] { ',' });
                                           double dsin = Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1]);
                                           if (dicInc.ContainsKey(dsin + "," + s[2]))
                                               dp += dicInc[dsin + "," + s[2]] * kin.Value;
                                       }
                                       dp = dp / dicPercentageForAggregationInc[s[0] + "," + s[1]].Sum(p => p.Value);
                                       //dp = dicInc[dicSourceTarget[dr["CColumn"].ToString() + "," + dr["Row"]] + "," + dr["AgeRangeID"]];
                                   }


                                   if (dicPopInc.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])) && dicPopulation.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                                   {

                                       dicPopInc[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])] += dp * Convert.ToDouble(k.Value) / dicPopulation[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])];

                                   }
                                   else if (dicPopulation.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                                   {
                                       dicPopInc.Add(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1]), dp * Convert.ToDouble(k.Value) / dicPopulation[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])]);
                                   }

                                   //}
                                   //if(dicIncidenceRateAttribute.ContainsKey
                                   //dicIncidenceRateAttribute.Add(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"]), Convert.ToDouble(dr["VValue"]));
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
                                           //(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString()
                                       }
                                       else
                                       {
                                           dicPercentageForAggregationInc.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), new Dictionary<string, double>());
                                           dicPercentageForAggregationInc[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                                           //dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                                       }

                                   }
                                  

                                   dsPercentage.Dispose();
                                   Dictionary<int, double> dicPopInc = new Dictionary<int, double>();

                                   foreach (KeyValuePair<string, double> k in dicPopulationAge)
                                   {
                                       string[] s = k.Key.Split(new char[] { ',' });
                                       double dp = 0,dsum=0;

                                       if (dicPercentageForAggregationInc.ContainsKey(s[0] + "," + s[1]))//&& dicInc.ContainsKey(dicSourceTarget[dr["CColumn"].ToString() + "," + dr["Row"]] + "," + dr["AgeRangeID"]))
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
                                           //dp = dicInc[dicSourceTarget[dr["CColumn"].ToString() + "," + dr["Row"]] + "," + dr["AgeRangeID"]];
                                       }


                                       if (dicPopInc.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])) && dicPopulation.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                                       {

                                           dicPopInc[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])] += dp * Convert.ToDouble(k.Value) / dicPopulation[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])];

                                       }
                                       else if(dicPopulation.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                                       {
                                           dicPopInc.Add(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1]), dp * Convert.ToDouble(k.Value) / dicPopulation[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])]);
                                       }

                                       //}
                                       //if(dicIncidenceRateAttribute.ContainsKey
                                       //dicIncidenceRateAttribute.Add(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"]), Convert.ToDouble(dr["VValue"]));
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
                   //return dicIncidenceRateAttribute;
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
                           if (dicRelationShip.ContainsKey(dr["CColumn"].ToString() + "," + dr["Row"]))//&& dicInc.ContainsKey(dicSourceTarget[dr["CColumn"].ToString() + "," + dr["Row"]] + "," + dr["AgeRangeID"]))
                           {
                               foreach (KeyValuePair<string, double> k in dicRelationShip[dr["CColumn"].ToString() + "," + dr["Row"]])
                               {
                                   dp += dicInc[k.Key + "," + dr["AgeRangeID"]] * k.Value;
                               }
                               //dp = dicInc[dicSourceTarget[dr["CColumn"].ToString() + "," + dr["Row"]] + "," + dr["AgeRangeID"]];
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
                   //return dicIncidenceRateAttribute;
               }
               
              //--------------------------------------------------modify by xiejp for Inc-------------------------------------
              

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
               Dictionary<int,double> dicResult =new  Dictionary<int,double>();
               double IncidenceValue = 0;
               if (incidenceDataSetGridType == GridDefinitionID)
               {
                   dicResult = dicIncidenceRateAttribute;
               }
               else if (CommonClass.MainSetup.SetupID == 1 && CommonClass.BenMAPPopulation.DataSetID == 4 && (CommonClass.GBenMAPGrid.GridDefinitionID== CommonClass.BenMAPPopulation.GridType.GridDefinitionID||( CommonClass.GBenMAPGrid.GridDefinitionID==27 &&CommonClass.BenMAPPopulation.GridType.GridDefinitionID==28) ||( CommonClass.GBenMAPGrid.GridDefinitionID==28 &&CommonClass.BenMAPPopulation.GridType.GridDefinitionID==27)))
               {
                   dicResult = dicIncidenceRateAttribute;
               }
               else
               {
                   //------------------------适应BenMAP4原有的空间关系----------------
                   dicRelationShip = APVX.APVCommonClass.getRelationFromDicRelationShipAll(gridRelationShipPopulation);
                   if (incidenceDataSetGridType == gridRelationShipPopulation.bigGridID)//Population比较大
                   {
                       if (dicRelationShip != null && dicRelationShip.Count > 0)
                       {
                           foreach (KeyValuePair<string,Dictionary<string, double>> k in dicRelationShip)
                           {
                               string[] s =k.Key.Split(new char[]{','});
                               if (dicIncidenceRateAttribute.Keys.Contains(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                               {
                                   foreach (KeyValuePair<string,double> rc in k.Value)
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
                   else//网格类型比较大
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
                                              // d = (d + dicIncidenceRateAttribute[Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])] * rc.Value * dicPopulation12[Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])]);
                                              //// dISum += rc.Value;
                                              // dISum += rc.Value * dicPopulation12[Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])];

                                               //----------modify by xiejp 20120625 disum= sum(percentage)
                                               d = (d + dicIncidenceRateAttribute[Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1])] * rc.Value );
                                               // dISum += rc.Value;
                                               dISum += rc.Value ;
                                           }
                                           catch
                                           { 
                                           }
                                       }
                                   }
                                   if (dicPopulation.ContainsKey(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
                                   {
                                       d = d / dISum;// dicPopulation[Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])];//modify by xiejp 20120619-- for bug 36km
                                       if (!dicResult.Keys.Contains(Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])))
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
               if(dsIncidence!=null)
               dsIncidence.Dispose();
               //_connection.Close();
               //_connection.Close();
               return dicResult;
           }
           catch (Exception ex)
           {
               return null;
           }
           //return lstIncidenceRateAttribute;

       }
        public static Dictionary<string, double> getDicAge(CRSelectFunction crSelectFunction)
        {
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               
            string strwhere = "";
            if (CommonClass.MainSetup.SetupID == 1)
                strwhere = "where AGERANGEID!=42";
            else
                strwhere = " where 1=1 ";
            //string ageCommandText = string.Format("select * from Ageranges b   " + strwhere);
            string ageCommandText = string.Format("select b.* from PopulationConfigurations a, Ageranges b   where a.PopulationConfigurationID=b.PopulationConfigurationID and a.PopulationConfigurationID=(select PopulationConfigurationID from PopulationDatasets where PopulationDataSetID=" + CommonClass.BenMAPPopulation.DataSetID + ")");// + strwhere);
               
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
                //-------如果StartAge在区间内--，则sum---不然--考虑-1的情况------------
                if ((Convert.ToInt32(dr["StartAge"]) >= crSelectFunction.StartAge || crSelectFunction.StartAge == -1) && (Convert.ToInt32(dr["EndAge"]) <= crSelectFunction.EndAge || crSelectFunction.EndAge == -1))
                {
                    //sum
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
                //getIncidenceLevelFromDatabase();

                // FbConnection _connection = CommonClass.getNewConnection();
                //  getIncidenceLevelFromDatabase();
                //  double dpoplevel = getPopLevelFromCR(crSelectFunction);
                Dictionary<int, double> dicIncidenceRateAttribute = new Dictionary<int, double>();
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                //首先通过crfunction 的metric 筛选所有的base,control,deltaq->               
                //--------------------
                string strbPrevalence = "F";
                int iid = crSelectFunction.IncidenceDataSetID;
                if (bPrevalence)
                {
                    strbPrevalence = "T";
                    iid = crSelectFunction.PrevalenceDataSetID;
                }
                //------------------modify by xiejp----------------先求出该EndPointGroup的年龄段--直接计算常量-----------
                //----求出可能的年龄段以及权重保存起来，然后用于SQL-------
                string commandText = "";

                //double dPopLevel = getPopLevelFromCR(crSelectFunction);
                //------------得到populationCommandText,首先从IncidenceDataSet中得到他的GridTypeID,以及SetupID,得到populationDataSetID------------
                //int iPopulationDataSetID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, string.Format("select PopulationDataSetID from PopulationDataSets where SetupID={0} and GridDefinitionID= (select GridDefinitionID from IncidenceDataSets where IncidenceDataSetID={1} )", CommonClass.MainSetup.SetupID, iid)));
                int iPopulationDataSetGridID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, string.Format("select GridDefinitionID from IncidenceDataSets where IncidenceDataSetID={1} ", CommonClass.MainSetup.SetupID, iid)));

               // BenMAPPopulation benMAPPopulation = new BenMAPPopulation() { DataSetID = iPopulationDataSetID, GridType = new BenMAPGrid() { GridDefinitionID = iPopulationDataSetGridID }, Year = CommonClass.BenMAPPopulation.Year };
                commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", CommonClass.BenMAPPopulation.DataSetID);// benMAPPopulation.Year);
                int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
                commandText = "";
                //-----------得到该StartAge-->EndAge的Population------------
                //----------------得到Incidence分年龄段的Sql-------------------------------------
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
  " WHEN (b.startAge<a.StartAge and b.EndAge<=a.EndAge and ({3}-{2}+1)>0) THEN  Cast(({3}-{0}+1) as float)/({3}-{2}+1)" + //加上对本身startAge,EndAge的考虑---
  " WHEN (b.startAge<a.StartAge and b.EndAge>a.EndAge and ({3}-{2}+1)>0) THEN Cast(({1}-{0}+1) as float)/({3}-{2}+1)" +
  "  WHEN (b.startAge>=a.StartAge and b.EndAge>a.EndAge and ({3}-{2}+1)>0) THEN Cast(({1}-{2}+1) as float)/({3}-{2}+1)" +
  " WHEN ({3}-{2}+1)<=0 THEN 0 " +
  " ELSE 1" +
  " END as weight,b.StartAge as sourceStartAge,b.EndAge as SourceEndAge" +
  "  from ( select distinct startage,endage from Incidencerates  where IncidenceDataSetID=" + iid + ")a,ageranges b" +
  " where b.EndAge>=a.StartAge and b.StartAge<=a.EndAge and b.PopulationConfigurationID={4}", straStartAgeOri, straEndAgeOri, strbStartAgeOri, strbEndAgeOri,CommonClass.BenMAPPopulation.PopulationConfiguration);

                string strInc = string.Format("select  a.CColumn,a.Row,sum(a.VValue*d.Weight) as VValue,d.AgeRangeID  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c ,(" + strAgeID +
                     ") d where   b.StartAge=d.StartAge and b.EndAge=d.EndAge and " +
             " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + strRace + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100 or b.EndPointID=102)" + " and b.Prevalence='" + strbPrevalence + "' " +
             "  and c.IncidenceDatasetID={0} and b.StartAge<={2} and b.EndAge>={1} group by a.CColumn,a.Row ,d.AgeRangeID", iid, crSelectFunction.StartAge, crSelectFunction.EndAge);
                //---------------End得到Incidence分年龄段的Sql-----------------------------------------------------------------------------------------------------------------
               // int iIncidenceGridTypeID=0;//
                //iPercentageID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, "select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + CommonClass.GBenMAPGrid.GridDefinitionID + " and  targetgriddefinitionid = " + iPopulationDataSetGridID));
               // iIncidenceGridTypeID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, "select gridDefinitionID from IncidenceDataSets where IncidenceDataSetID=" + iid));
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
                    //iPercentageID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, "select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + CommonClass.GBenMAPGrid.GridDefinitionID + " and  targetgriddefinitionid = " + iPopulationDataSetGridID));
                    //string str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( " + iPercentageID + " ) and normalizationstate in (0,1)";

                    string str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" +( CommonClass.GBenMAPGrid.GridDefinitionID==28?27: CommonClass.GBenMAPGrid.GridDefinitionID) + " and  targetgriddefinitionid = " + iPopulationDataSetGridID + " ) and normalizationstate in (0,1)";
                  
                    DataSet dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
                    if (dsPercentage.Tables[0].Rows.Count == 0)
                    {
                        creatPercentageToDatabase(iPopulationDataSetGridID, CommonClass.GBenMAPGrid.GridDefinitionID);
                        int  iPercentageID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, "select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + CommonClass.GBenMAPGrid.GridDefinitionID + " and  targetgriddefinitionid = " + iPopulationDataSetGridID));
                        str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + (CommonClass.GBenMAPGrid.GridDefinitionID == 28 ? 27 : CommonClass.GBenMAPGrid.GridDefinitionID) + " and  targetgriddefinitionid = " + iPopulationDataSetGridID + " ) and normalizationstate in (0,1)";
                        dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
                    }
                    foreach (DataRow dr in dsPercentage.Tables[0].Rows)
                    {
                        if (dicPercentageForAggregationInc.ContainsKey(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()))
                        {
                            if (!dicPercentageForAggregationInc[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].ContainsKey(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()))
                                dicPercentageForAggregationInc[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                            //(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString()
                        }
                        else
                        {
                            dicPercentageForAggregationInc.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), new Dictionary<string, double>());
                            dicPercentageForAggregationInc[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                            //dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                        }

                    }
                    
                    Dictionary<string, double> dicReturn = new Dictionary<string, double>();
                    foreach (KeyValuePair<string, float> k in dicPopulationAge)
                    {
                         string[] s=k.Key.Split(new char[]{','});
                        if(!dicAge.ContainsKey(s[2])) continue;
                         if (dicPercentageForAggregationInc.ContainsKey(s[0] + "," + s[1]))
                        {
                            foreach (KeyValuePair<string, double> kin in dicPercentageForAggregationInc[s[0] + "," + s[1]])
                            {
                                string[] sin = kin.Key.Split(new char[] { ',' });
                                double dsin = Convert.ToInt32(sin[0]) * 10000 + Convert.ToInt32(sin[1]);
                                if(!dicInc.ContainsKey(dsin + "," + s[2])) continue;
                                if (dicReturn.ContainsKey((Convert.ToInt32(s[0]) * 10000 + Convert.ToInt32(s[1])).ToString()+","+s[2]))
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
            //return lstIncidenceRateAttribute;

        }
        public static Dictionary<string, double> getIncidenceDataSetFromCRSelectFuntionDicAllAgeOld(Dictionary<int, float> dicPopulation, Dictionary<string, float> dicPopulationAge, Dictionary<int, float> dicPopulation12, CRSelectFunction crSelectFunction, bool bPrevalence, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, int GridDefinitionID, GridRelationship gridRelationShipPopulation)
        {
            try
            {
                //getIncidenceLevelFromDatabase();

                // FbConnection _connection = CommonClass.getNewConnection();
                //  getIncidenceLevelFromDatabase();
                //  double dpoplevel = getPopLevelFromCR(crSelectFunction);
                Dictionary<int, double> dicIncidenceRateAttribute = new Dictionary<int, double>();
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                //首先通过crfunction 的metric 筛选所有的base,control,deltaq->               
                //--------------------
                string strbPrevalence = "F";
                int iid = crSelectFunction.IncidenceDataSetID;
                if (bPrevalence)
                {
                    strbPrevalence = "T";
                    iid = crSelectFunction.PrevalenceDataSetID;
                }
                //------------------modify by xiejp----------------先求出该EndPointGroup的年龄段--直接计算常量-----------
                //----求出可能的年龄段以及权重保存起来，然后用于SQL-------
                string commandText = "";

                //double dPopLevel = getPopLevelFromCR(crSelectFunction);
                //------------得到populationCommandText,首先从IncidenceDataSet中得到他的GridTypeID,以及SetupID,得到populationDataSetID------------
                int iPopulationDataSetID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, string.Format("select PopulationDataSetID from PopulationDataSets where SetupID={0} and GridDefinitionID= (select GridDefinitionID from IncidenceDataSets where IncidenceDataSetID={1} )", CommonClass.MainSetup.SetupID, iid)));
                int iPopulationDataSetGridID = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, string.Format("select GridDefinitionID from IncidenceDataSets where IncidenceDataSetID={1} ", CommonClass.MainSetup.SetupID, iid)));

                BenMAPPopulation benMAPPopulation = new BenMAPPopulation() { DataSetID = iPopulationDataSetID, GridType = new BenMAPGrid() { GridDefinitionID = iPopulationDataSetGridID }, Year = CommonClass.BenMAPPopulation.Year };
                commandText = string.Format("select  min( Yyear) from t_PopulationDataSetIDYear where PopulationDataSetID={0} ", iPopulationDataSetID);// benMAPPopulation.Year);
                int commonYear = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, System.Data.CommandType.Text, commandText));
                string populationCommandText = getPopulationComandTextFromCRSelectFunction(crSelectFunction, benMAPPopulation, dicRace, dicEthnicity, dicGender);
                commandText = "";
                //-----------得到该StartAge-->EndAge的Population------------
                //----------------得到Incidence分年龄段的Sql-------------------------------------
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
  " WHEN (b.startAge<a.StartAge and b.EndAge<=a.EndAge) THEN  Cast(({3}-{0}+1) as float)/({3}-{2}+1)" + //加上对本身startAge,EndAge的考虑---
  " WHEN (b.startAge<a.StartAge and b.EndAge>a.EndAge) THEN Cast(({1}-{0}+1) as float)/({3}-{2}+1)" +
  "  WHEN (b.startAge>=a.StartAge and b.EndAge>a.EndAge) THEN Cast(({1}-{2}+1) as float)/({3}-{2}+1)" +
  " ELSE 1" +
  " END as weight,b.StartAge as sourceStartAge,b.EndAge as SourceEndAge" +
  "  from ( select distinct startage,endage from Incidencerates  where IncidenceDataSetID=" + iid + ")a,ageranges b" +
  " where b.EndAge>=a.StartAge and b.StartAge<=a.EndAge", straStartAgeOri, straEndAgeOri, strbStartAgeOri, strbEndAgeOri);

                string strInc = string.Format("select  a.CColumn,a.Row,sum(a.VValue*d.Weight) as VValue,d.AgeRangeID  from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c ,(" + strAgeID +
                     ") d where   b.StartAge=d.StartAge and b.EndAge=d.EndAge and " +
             " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + strRace + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100 or b.EndPointID=102)" + " and b.Prevalence='" + strbPrevalence + "' " +
             "  and c.IncidenceDatasetID={0} and b.StartAge<={2} and b.EndAge>={1} group by a.CColumn,a.Row ,d.AgeRangeID", iid, crSelectFunction.StartAge, crSelectFunction.EndAge);
                //---------------End得到Incidence分年龄段的Sql-----------------------------------------------------------------------------------------------------------------
                // int iIncidenceGridTypeID=0;//
                //iPercentageID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, "select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + CommonClass.GBenMAPGrid.GridDefinitionID + " and  targetgriddefinitionid = " + iPopulationDataSetGridID));
                // iIncidenceGridTypeID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, "select gridDefinitionID from IncidenceDataSets where IncidenceDataSetID=" + iid));
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
                    //iPercentageID = Convert.ToInt16(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, "select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + CommonClass.GBenMAPGrid.GridDefinitionID + " and  targetgriddefinitionid = " + iPopulationDataSetGridID));
                    //string str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( " + iPercentageID + " ) and normalizationstate in (0,1)";

                    string str = "select sourcecolumn, sourcerow, targetcolumn, targetrow, percentage, normalizationstate from griddefinitionpercentageentries where percentageid=( select percentageid from  griddefinitionpercentages where sourcegriddefinitionid =" + (CommonClass.GBenMAPGrid.GridDefinitionID == 28 ? 27 : CommonClass.GBenMAPGrid.GridDefinitionID) + " and  targetgriddefinitionid = " + iPopulationDataSetGridID + " ) and normalizationstate in (0,1)";

                    DataSet dsPercentage = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, str);
                    if (dsPercentage.Tables[0].Rows.Count == 0)
                    {
                        creatPercentageToDatabase(iPopulationDataSetGridID, CommonClass.GBenMAPGrid.GridDefinitionID);
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
                            //(Convert.ToInt32(dr["CColumn"]) * 10000 + Convert.ToInt32(dr["Row"])).ToString()
                        }
                        else
                        {
                            dicPercentageForAggregationInc.Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), new Dictionary<string, double>());
                            dicPercentageForAggregationInc[dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString()].Add(dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString(), Convert.ToDouble(dr["Percentage"]));
                            //dicRelationShip[dr["targetcolumn"].ToString() + "," + dr["targetrow"].ToString()].Add(dr["sourcecolumn"].ToString() + "," + dr["sourcerow"].ToString(), Convert.ToDouble(dr["Percentage"]));
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
            //return lstIncidenceRateAttribute;

        }
   
       /// <summary>
       /// 从CRFunction中获取IncidenceDataSet
       /// </summary>
       /// <param name="crSelectFunction"></param>
       /// <param name="bPrevalence"></param>
       /// <param name="dicRace"></param>
       /// <param name="dicEthnicity"></param>
       /// <param name="dicGender"></param>
       /// <param name="GridDefinitionID"></param>
       /// <param name="gridRelationShipPopulation"></param>
       /// <returns></returns>
       public static List<IncidenceRateAttribute> getIncidenceDataSetFromCRSelectFuntion(CRSelectFunction crSelectFunction, bool bPrevalence, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, int GridDefinitionID, GridRelationship gridRelationShipPopulation)
       {
           try
           {
                
              // FbConnection _connection = CommonClass.getNewConnection();
               List<IncidenceRateAttribute> lstIncidenceRateAttribute = new List<IncidenceRateAttribute>();
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               //首先通过crfunction 的metric 筛选所有的base,control,deltaq->
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
     " and c.IncidenceDatasetID={0} ", iid);//and b.StartAge<={1} and b.EndAge>={2}", crSelectFunction.IncidenceDataSetID, crSelectFunction.EndAge, crSelectFunction.StartAge);
               if (crSelectFunction.StartAge != -1)
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
                   //dicGender.Add(dr["GenderName"].ToString(), Convert.ToInt32(dr["GenderID"]));
                   lstIncidenceRateAttribute.Add(new IncidenceRateAttribute()
                   {
                       Col = Convert.ToInt32(dr["CColumn"]),
                       Row = Convert.ToInt32(dr["Row"]),
                       Value = Convert.ToSingle(dr["VValue"])
                       //,
                       //IncidenceRateID = Convert.ToInt32(dr["IncidenceRateID"]),
                       //EndAge = Convert.ToInt32(dr["EndAge"]),
                       //EthnicityID = Convert.ToInt32(dr["EthnicityID"]),
                       //GenderID = Convert.ToInt32(dr["GenderID"]),
                       //RaceID = Convert.ToInt32(dr["RaceID"]),
                       //StartAge = Convert.ToInt32(dr["StartAge"])
                   });

               }

               if(!bPrevalence)
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
                   if (incidenceDataSetGridType == gridRelationShipPopulation.bigGridID)//Population比较大
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
                   else//网格类型比较大
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
               //_connection.Close();
               //_connection.Close();
               return lstResult;
           }
           catch (Exception ex)
           {
               return null;
           }
           //return lstIncidenceRateAttribute;

       }
       /// <summary>
       /// 从CRFunction中获取IncidenceDataSet------已被替换
       /// </summary>
       /// <param name="crSelectFunction"></param>
       /// <param name="bPrevalence"></param>
       /// <param name="dicRace"></param>
       /// <param name="dicEthnicity"></param>
       /// <param name="dicGender"></param>
       /// <returns></returns>
       public static List<IncidenceRateAttribute> getIncidenceDataSetFromCRSelectFuntion(CRSelectFunction crSelectFunction, bool bPrevalence, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender)
       {
           try
           {
               //FbConnection _connection = CommonClass.getNewConnection();
               List<IncidenceRateAttribute> lstIncidenceRateAttribute = new List<IncidenceRateAttribute>();
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               //首先通过crfunction 的metric 筛选所有的base,control,deltaq->
               DataSet dsIncidence = null;
               DataSet dsPrevalence = null;
               string strbPrevalence = "F";
               if (bPrevalence) strbPrevalence = "T";
               string commandText = string.Format("select distinct a.IncidenceRateID,a.CColumn,a.Row,a.VValue,b.StartAge,b.EndAge,b.RaceID,b.EthnicityID,b.GenderID from IncidenceEntries a,IncidenceRates b,IncidenceDatasets c where" +
     " a.IncidenceRateID=b.IncidenceRateID and b.IncidenceDatasetID=c.IncidenceDatasetID and b.EndPointGroupID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointGroupID + " and (b.EndPointID=" + crSelectFunction.BenMAPHealthImpactFunction.EndPointID + "  or b.EndPointID=99 or b.EndPointID=100) and b.Prevalence='" + strbPrevalence + "' " +
     " and c.IncidenceDatasetID={0} ", crSelectFunction.IncidenceDataSetID);//and b.StartAge<={1} and b.EndAge>={2}", crSelectFunction.IncidenceDataSetID, crSelectFunction.EndAge, crSelectFunction.StartAge);
               if (crSelectFunction.StartAge != -1)
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
                   //dicGender.Add(dr["GenderName"].ToString(), Convert.ToInt32(dr["GenderID"]));
                   lstIncidenceRateAttribute.Add(new IncidenceRateAttribute()
                   {
                       Col = Convert.ToInt32(dr["CColumn"]),
                       Row = Convert.ToInt32(dr["Row"]),
                       Value = Convert.ToSingle(dr["VValue"])
                       //,
                       //IncidenceRateID = Convert.ToInt32(dr["IncidenceRateID"]),
                       //EndAge = Convert.ToInt32(dr["EndAge"]),
                       //EthnicityID = Convert.ToInt32(dr["EthnicityID"]),
                       //GenderID = Convert.ToInt32(dr["GenderID"]),
                       //RaceID = Convert.ToInt32(dr["RaceID"]),
                       //StartAge = Convert.ToInt32(dr["StartAge"])
                   });

               }
               //_connection.Close();
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
       /// <summary>
       /// 得到所有系统变量列表
       /// </summary>
       /// <returns></returns>
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
       /// <summary>
       /// 从DataSetID和Year中获取PopulationDataSet
       /// </summary>
       /// <param name="DataSetID"></param>
       /// <param name="Year"></param>
       /// <returns></returns>
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
               //commandText = string.Format("select a.PopulationDatasetID,a.RaceID,a.GenderID,a.AgeRangeID,a.CColumn,a.Row,a.YYear,a.VValue,a.EthnicityID  ,b.StartAge,b.EndAge      from PopulationEntries a,Ageranges b   where a.AgerangeID=b.AgerangeID and a.PopulationDatasetID={0} and YYear={1}", DataSetID, Year);
               //ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
               //benMAPPopulation.PopulationAttributes = new List<PopulationAttribute>();
               //foreach (DataRow drPop in ds.Tables[0].Rows)
               //{
               //    PopulationAttribute populationAttribute = new PopulationAttribute();

               //    populationAttribute.AgeRangeID = Convert.ToInt32(drPop["AgeRangeID"]);
               //    populationAttribute.Col = Convert.ToInt32(drPop["CColumn"]);
               //    populationAttribute.EndAge = Convert.ToInt32(drPop["EndAge"]);
               //    populationAttribute.EthnicityID = Convert.ToInt32(drPop["EthnicityID"]);
               //    populationAttribute.GenderID = Convert.ToInt32(drPop["GenderID"]);
               //    populationAttribute.RaceID = Convert.ToInt32(drPop["RaceID"]);
               //    populationAttribute.Row = Convert.ToInt32(drPop["Row"]);
               //    populationAttribute.StartAge = Convert.ToInt32(drPop["StartAge"]);
               //    populationAttribute.Value = Convert.ToDouble(drPop["VValue"]);
               //    populationAttribute.Year = Convert.ToInt32(drPop["YYear"]);
               //    benMAPPopulation.PopulationAttributes.Add(populationAttribute);

               //}
               ds.Dispose();
               return benMAPPopulation;
           }
           catch (Exception ex)
           {
               return null;
           }
       }
       /// <summary>
       /// Calculate CRSelectFunctions
       /// </summary>
       /// <param name="lstCRSelectFuntion"></param>
       /// <param name="lstBaseControlGroup"></param>
       /// 

       //public static List<CRSelectFunctionCalculateValue> CalculateCRSelectFunctions(double Threshold, int LatinHypercubePoints, bool RunInPointMode, List<GridRelationship> lstGridRelationship, List<CRSelectFunction> lstCRSelectFuntion, List<BaseControlGroup> lstBaseControlGroup, List<RegionTypeGrid> lstRegionTypeGrid, BenMAPPopulation benMAPPopulation)
       //{
       //    try
       //    {
       //        //由每条CRSelectFuntion拼凑出一个 健康冲击 网格；包括 人口数据网格 + Grid+ Base+Control +DeltaQ 每个函数只对一组污染物数据有效 
       //        //如果输入了location,则只在该location 有效，其他为0 ，以相交作为有效的标准。所以系统一选择Grid就应该初始化数据库里所有的Grid和选择的grid的关系，
       //        //除掉自己 State County City and Grid 之间的关系。-> regionid list<col ,row>
       //        string commandText = "";
       //        //得到所有的Race,Ethnicity,Gender
       //        Dictionary<string, int> dicRace = getAllRace();// new Dictionary<string, int>();
       //        Dictionary<string, int> dicEthnicity = getAllEthnicity();// new Dictionary<string, int>();
       //        Dictionary<string, int> dicGender = getAllGender();// new Dictionary<string, int>();

       //        ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
       //        List<CRSelectFunctionCalculateValue> lstCRSelectFunctionCalculateValue = new List<CRSelectFunctionCalculateValue>();
       //        List<string> lstSystemVariableName = getAllSystemVariableNameList();
       //        foreach (CRSelectFunction crSelectFunction in lstCRSelectFuntion)
       //        {
       //            CRSelectFunctionCalculateValue crSelectFunctionCalculateValue = new CRSelectFunctionCalculateValue() { CRSelectFunction = crSelectFunction, CRCalculateValues = new List<CRCalculateValue>() };
       //            //得到所有变量列表
       //            List<SetupVariableJoinAllValues> lstSetupVariable = new List<SetupVariableJoinAllValues>();
       //            getSetupVariableNameListFromDatabaseFunction(crSelectFunction.BenMAPHealthImpactFunction.Function, lstSystemVariableName, ref lstSetupVariable);
       //            getSetupVariableNameListFromDatabaseFunction(crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction, lstSystemVariableName, ref lstSetupVariable);

       //            //每个CRSelectFunction 将对应一个Grid
       //            List<IncidenceRateAttribute> lstIncidenceRateAttribute = new List<IncidenceRateAttribute>();
       //            List<IncidenceRateAttribute> lstPrevalenceRateAttribute = new List<IncidenceRateAttribute>();
       //            int incidenceDataSetGridType = -1;
       //            int PrevalenceDataSetGridType = -1;
       //            var query = from a in lstBaseControlGroup where a.Pollutant.PollutantID == crSelectFunction.BenMAPHealthImpactFunction.Pollutant.PollutantID select a;
       //            if (query == null || query.Count() == 0) break;
       //            BaseControlGroup baseControlGroup = query.First();
       //            GridRelationship gridRelationShip = null;
       //            if (crSelectFunction.Locations != null && crSelectFunction.Locations.Count > 0)
       //            {
       //                foreach (GridRelationship gRelationship in lstGridRelationship)
       //                {
       //                    if (gRelationship.bigGridID == crSelectFunction.Locations.First().GridDifinitionID || gRelationship.smallGridID == crSelectFunction.Locations.First().GridDifinitionID)
       //                    {
       //                        gridRelationShip = gRelationship;
       //                    }
       //                }
       //            }
       //            GridRelationship gridRelationShipIncidence = null;
       //            GridRelationship gridRelationShipPrevalence = null;
       //            GridRelationship gridPopulationShip = null;
       //            foreach (GridRelationship gRelationship in lstGridRelationship)
       //            {
       //                if ((gRelationship.bigGridID == benMAPPopulation.GridType.GridDefinitionID && gRelationship.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID) || (gRelationship.smallGridID == benMAPPopulation.GridType.GridDefinitionID && gRelationship.bigGridID == CommonClass.GBenMAPGrid.GridDefinitionID))
       //                {
       //                    gridPopulationShip = gRelationship;
       //                }
       //            }
       //            //如果incidence有选择，则从数据库获取；需考虑grid的不同，进行计算；如果有race,ethnicity,gender则进行筛选；如果有StartAge EndAge则要进行加减等，
       //            if (crSelectFunction.IncidenceDataSetID > -1)//考虑使用存储过程
       //            {
       //                //得到GridType

       //                // lstIncidenceRateAttribute= 

       //                commandText = string.Format("select GriddefinitionID from IncidenceDatasets where IncidenceDatasetID={0}", crSelectFunction.IncidenceDataSetID);
       //                incidenceDataSetGridType = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));


       //                foreach (GridRelationship gRelationship in lstGridRelationship)
       //                {
       //                    if ((gRelationship.bigGridID == incidenceDataSetGridType && gRelationship.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID) || (gRelationship.smallGridID == incidenceDataSetGridType && gRelationship.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID))
       //                    {
       //                        gridRelationShipIncidence = gRelationship;
       //                    }
       //                }
       //                lstIncidenceRateAttribute = getIncidenceDataSetFromCRSelectFuntion(crSelectFunction, false, dicRace, dicEthnicity, dicGender, baseControlGroup.GridType.GridDefinitionID, gridRelationShipIncidence);


       //            }
       //            //如果prevalence有选择，则从数据库获取；需考虑grid的不同，进行计算如果有race,ethnicity,gender则进行筛选；如果有StartAge EndAge则要进行加减等，
       //            if (crSelectFunction.PrevalenceDataSetID > -1)
       //            {


       //                commandText = string.Format("select GriddefinitionID from IncidenceDatasets where IncidenceDatasetID={0}", crSelectFunction.PrevalenceDataSetID);
       //                PrevalenceDataSetGridType = Convert.ToInt32(fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText));
       //                foreach (GridRelationship gRelationship in lstGridRelationship)
       //                {
       //                    if ((gRelationship.bigGridID == PrevalenceDataSetGridType && gRelationship.smallGridID==CommonClass.GBenMAPGrid.GridDefinitionID) || (gRelationship.smallGridID == PrevalenceDataSetGridType && gRelationship.smallGridID==CommonClass.GBenMAPGrid.GridDefinitionID))
       //                    {
       //                        gridRelationShipPrevalence = gRelationship;
       //                    }
       //                }

       //                lstPrevalenceRateAttribute = getIncidenceDataSetFromCRSelectFuntion(crSelectFunction, true, dicRace, dicEthnicity, dicGender, baseControlGroup.GridType.GridDefinitionID, gridRelationShipPrevalence);
       //            }
       //            //-----------------------------------------------------获取拉丁采样double[]未做------------------------------------------------------

       //            double[] lhsResultArray = null;
       //            if (!RunInPointMode)
       //            {
       //                lhsResultArray = getLHSArray(LatinHypercubePoints);
       //            }


       //            //---------------------------------------------------end 获取拉丁采样double-----------------------------------------------------------
       //            //----------------------------------------------Population----------------------------------------------------------------------------
       //            // var queryPopulation = from a in benMAPPopulation.PopulationAttributes select a;
       //            //PopulationAttribute populationAttribute = queryPopulation.First();
       //            //如果有race,ethnicity,gender则进行筛选；如果有StartAge EndAge则要进行加减等，最后得到population的值
       //            List<RowCol> lstRowColGridType = new List<RowCol>();
       //            var queryRowCol = from a in baseControlGroup.Base.ModelResultAttributes select new RowCol() { Col = a.Col, Row = a.Row };
       //            lstRowColGridType = queryRowCol.ToList();
       //            List<PopulationAttribute> lstPopulation = getPopulationDataSetFromCRSelectFunction(crSelectFunction, benMAPPopulation, dicRace, dicEthnicity, dicGender, baseControlGroup.GridType.GridDefinitionID, gridPopulationShip);



       //            //----------------------------------------------end Population------------------------------------------------------------------------
       //            //通过函数计算值(考虑 Incidence Prevalence Variable库对函数的影响，以及是否有region ,如果有region只对region的范围有效）
       //            foreach (ModelResultAttribute modelResultAttribute in baseControlGroup.Base.ModelResultAttributes)
       //            {
       //                int col = modelResultAttribute.Col;
       //                int row = modelResultAttribute.Row;
       //                //首先查看是否有region的设置，如果有则加入。--------------------------------------------------------
       //                if (crSelectFunction.Locations != null && crSelectFunction.Locations.Count > 0)
       //                {
       //                    //如果该网格cell不在region内则continue
       //                    if (gridRelationShip != null)
       //                    {
       //                        if (crSelectFunction.Locations.First().GridDifinitionID == gridRelationShip.bigGridID)
       //                        {
       //                            //region的网格类型比较大
       //                            RowCol rowCol = new RowCol() { Col = col, Row = row };
       //                            var queryrowCol = from a in gridRelationShip.lstGridRelationshipAttribute where a.smallGridRowCol.Contains(rowCol) select new RowCol() { Col = a.bigGridRowCol.Col, Row = a.bigGridRowCol.Row };
       //                            bool bin = false;

       //                            foreach (Location location in crSelectFunction.Locations)
       //                            {
       //                                if (queryrowCol.ToList().Contains(new RowCol() { Row = location.Row, Col = location.Col }))
       //                                {
       //                                    bin = true;
       //                                }
       //                            }
       //                            if (bin == false) continue;

       //                        }
       //                        else
       //                        {
       //                            //Grid的网格比较大-----------------------------------------------------------------未找到解决方法需要再讨论
       //                            /*
       //                             * 从选择CRFunction就屏蔽掉region比分析的网格还小的情况，只能出现比它还大的study。
       //                             * 然后在GIS选择study的时候可选一个层次的所有study，当比它小时，也只对选择的region有效。
       //                             * */

       //                        }

       //                    }

       //                }
       //                //得到base,control,deltaq
       //                double baseValue = modelResultAttribute.Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];
       //                if (Threshold != 0 && baseValue < Threshold)
       //                    baseValue = Threshold;
       //                var querycontrol = from a in baseControlGroup.Control.ModelResultAttributes where a.Col == col && a.Row == row select a;

       //                double controlValue = baseValue;
       //                double populationValue = 0;
       //                double incidenceValue = 1;
       //                double prevalenceValue = 1;
       //                if (querycontrol != null)
       //                {
       //                    controlValue = querycontrol.First().Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];
       //                }
       //                if (Threshold != 0 && controlValue < Threshold)
       //                    controlValue = Threshold;
       //                double deltaQValue = baseValue - controlValue;

       //                //var queryPopulationColRow = from a in lstPopulation where a.Col == col && a.Row == row select a;
       //                ////PopulationAttribute populationAttribute = queryPopulation.First();
       //                ////如果有race,ethnicity,gender则进行筛选；如果有StartAge EndAge则要进行加减等，最后得到population的值
       //                //{

       //                //    //计算StartAge and EndAge-------------------------------------------
       //                //    populationValue = queryPopulationColRow.Sum(a => a.Value);
       //                //}
       //                // populationValue = getPopulationValueFromColRow(col, row, benMAPPopulation,lstPopulation, baseControlGroup.GridType.GridDefinitionID, gridPopulationShip);
       //                var queryPopulation = from a in lstPopulation where a.Col == col && a.Row == row select a;
       //                double values = 0;

       //                if (queryPopulation.Count() > 0)
       //                    populationValue = queryPopulation.First().Value;// Convert.ToDouble(queryPopulation.Count());
       //                //----------incidenceValue----------------------------------------
       //                var queryincidenceValue = from a in lstIncidenceRateAttribute where a.Col == col && a.Row == row select a;
       //                //double values = 0;

       //                if (queryincidenceValue.Count() > 0)
       //                    incidenceValue = queryincidenceValue.First().Value;// Convert.ToDouble(queryPopulation.Count());

       //                //----------------prevalenceValue-----------------------------------
       //                var queryprevalenceValue = from a in lstPrevalenceRateAttribute where a.Col == col && a.Row == row select a;
       //                //double values = 0;

       //                if (queryprevalenceValue.Count() > 0)
       //                    prevalenceValue = queryprevalenceValue.First().Value;// Convert.ToDouble(queryPopulation.Count());
       //                //-------------------------------------------
       //                //incidenceValue = getIncidenceValueFromColRow(col, row, lstIncidenceRateAttribute, incidenceDataSetGridType, baseControlGroup.GridType.GridDefinitionID, gridRelationShipIncidence);
       //                //prevalenceValue = getPrevalenceValueFromColRow(col, row, lstPrevalenceRateAttribute, PrevalenceDataSetGridType, baseControlGroup.GridType.GridDefinitionID, gridRelationShipPrevalence);
       //                Dictionary<string, double> dicVariable = getDicSetupVariableColRow(col, row, lstSetupVariable, baseControlGroup.GridType.GridDefinitionID, lstGridRelationship);
       //                //计算公式---------------------------------------------------------------------------
       //                {
       //                    CRCalculateValue crCalculateValue = CalculateCRSelectFunctionsOneCel(crSelectFunction, col, row, baseValue, controlValue, populationValue, incidenceValue, prevalenceValue, lhsResultArray, dicVariable);
       //                    crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);
       //                }
       //            }
       //            lstCRSelectFunctionCalculateValue.Add(crSelectFunctionCalculateValue);
       //        }
       //        return lstCRSelectFunctionCalculateValue;
       //    }
       //    catch (Exception ex)
       //    {
       //        return null;
       //    }
       //}
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
               //lstMetricValueAttributes.ToDictionary<stri
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
       /// <summary>
       /// 得到所有Metric的值
       /// </summary>
       /// <param name="baseControlGroup"></param>
       /// <param name="isBase"></param>
       /// <returns></returns>
       public static Dictionary<string, Dictionary<string, float>> getAllMetricDataFromBaseControlGroup(BaseControlGroup baseControlGroup, bool isBase,ref Dictionary<string,Dictionary<string,List<float>>> dicAll365)
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
               //------------------
               Dictionary<string,string> dicMetricAll=new Dictionary<string,string>();
               if(baseControlGroup.Pollutant.Metrics!=null)
               {
                   foreach(Metric m in baseControlGroup.Pollutant.Metrics)
                   {
                       dicMetricAll.Add(m.MetricName,Enum.GetName(typeof(MetricStatic),m is MovingWindowMetric? (m as MovingWindowMetric).WindowStatistic:m is FixedWindowMetric?(m as FixedWindowMetric).Statistic:MetricStatic.Mean));
                   }
               }
               if(baseControlGroup.Pollutant.SesonalMetrics!=null)
               {
                   foreach(SeasonalMetric s in baseControlGroup.Pollutant.SesonalMetrics)
                   {
                       dicMetricAll.Add(s.SeasonalMetricName,Enum.GetName(typeof(MetricStatic),MetricStatic.Mean));
                   }
               }
               foreach (ModelResultAttribute m in benMapLine.ModelResultAttributes)
               {
                   dicReturn.Add(m.Col + "," + m.Row, m.Values);
                   Dictionary<string, float> dicAdd = new Dictionary<string, float>();
                   //m.Values.Where(p => p.Key.Split(new char[]{','}).Count() == 2);
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
                           //None = 0, Mean = 1, Median = 2, Max = 3, Min = 4, Sum = 5
                           if (dicReturn.ContainsKey(m.Col + "," + m.Row))
                           {
                               lstTemp = m.Values.Where(p => p != float.MinValue).ToList();
                               //float f = float.NaN;
                               if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.SeasonalMetric.SeasonalMetricName + "," + "Mean"))
                               {
                                   //--------get mean--------------

                                   dicReturn[m.Col + "," + m.Row].Add(m.SeasonalMetric.SeasonalMetricName + "," + "Mean", lstTemp.Count == 0 ? float.MinValue : lstTemp.Average());
                               }
                               else
                               {
                                   dicReturn[m.Col + "," + m.Row][m.SeasonalMetric.SeasonalMetricName + "," + "Mean"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.Average();
                               }
                               if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.SeasonalMetric.SeasonalMetricName + "," + "Median"))
                               {
                                   //--------get Median--------------
                                   dicReturn[m.Col + "," + m.Row].Add(m.SeasonalMetric.SeasonalMetricName + "," + "Median", lstTemp.Count == 0 ? float.MinValue : lstTemp.OrderBy(p => p).Median());
                               }
                               else
                               {
                                   dicReturn[m.Col + "," + m.Row][m.SeasonalMetric.SeasonalMetricName + "," + "Median"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.OrderBy(p => p).Median();
                               }
                               if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.SeasonalMetric.SeasonalMetricName + "," + "Max"))
                               {
                                   //--------get Max--------------
                                   dicReturn[m.Col + "," + m.Row].Add(m.SeasonalMetric.SeasonalMetricName + "," + "Max", lstTemp.Count == 0 ? float.MinValue : lstTemp.Max());
                               }
                               else
                                   dicReturn[m.Col + "," + m.Row][m.SeasonalMetric.SeasonalMetricName + "," + "Max"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.Max();
                               if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.SeasonalMetric.SeasonalMetricName + "," + "Min"))
                               {
                                   //--------get Min--------------
                                   dicReturn[m.Col + "," + m.Row].Add(m.SeasonalMetric.SeasonalMetricName + "," + "Min", lstTemp.Count == 0 ? float.MinValue : lstTemp.Min());
                               }
                               else
                                   dicReturn[m.Col + "," + m.Row][m.SeasonalMetric.SeasonalMetricName + "," + "Min"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.Min();
                               if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.SeasonalMetric.SeasonalMetricName + "," + "Sum"))
                               {
                                   //--------get Sum--------------
                                   dicReturn[m.Col + "," + m.Row].Add(m.SeasonalMetric.SeasonalMetricName + "," + "Sum", lstTemp.Count == 0 ? float.MinValue : lstTemp.Sum());
                               }
                               else
                                   dicReturn[m.Col + "," + m.Row][m.SeasonalMetric.SeasonalMetricName + "," + "Sum"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.Sum();
                           }

                       }
                       else if (m.Metric != null)
                       {
                           //None = 0, Mean = 1, Median = 2, Max = 3, Min = 4, Sum = 5
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
                                   //--------get mean--------------
                                   dicReturn[m.Col + "," + m.Row].Add(m.Metric.MetricName + "," + "Mean", lstTemp.Count == 0 ? float.MinValue : lstTemp.Average());
                               }
                               else
                                   dicReturn[m.Col + "," + m.Row][m.Metric.MetricName + "," + "Mean"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.Average();
                               if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.Metric.MetricName + "," + "Median"))
                               {
                                   //--------get Median--------------
                                   dicReturn[m.Col + "," + m.Row].Add(m.Metric.MetricName + "," + "Median", lstTemp.Count == 0 ? float.MinValue : lstTemp.OrderBy(p => p).Median());
                               }
                               else
                                   dicReturn[m.Col + "," + m.Row][m.Metric.MetricName + "," + "Median"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.OrderBy(p => p).Median();
                               if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.Metric.MetricName + "," + "Max"))
                               {
                                   //--------get Max--------------
                                   dicReturn[m.Col + "," + m.Row].Add(m.Metric.MetricName + "," + "Max", lstTemp.Count == 0 ? float.MinValue : lstTemp.Max());
                               }
                               else
                                   dicReturn[m.Col + "," + m.Row][m.Metric.MetricName + "," + "Max"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.Max();
                               if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.Metric.MetricName + "," + "Min"))
                               {
                                   //--------get Min--------------
                                   dicReturn[m.Col + "," + m.Row].Add(m.Metric.MetricName + "," + "Min", lstTemp.Count == 0 ? float.MinValue : lstTemp.Min());
                               }
                               else
                                   dicReturn[m.Col + "," + m.Row][m.Metric.MetricName + "," + "Min"] = lstTemp.Count == 0 ? float.MinValue : lstTemp.Min();
                               if (!dicReturn[m.Col + "," + m.Row].ContainsKey(m.Metric.MetricName + "," + "Sum"))
                               {
                                   //--------get Sum--------------
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
       //public static int iCalculateCRID=0;
       public static void CalculateOneCRSelectFunction(string sCRID,List<string> lstAllAgeID, Dictionary<string, double> dicAge, Dictionary<string, Dictionary<string, float>> dicBaseMetricData, Dictionary<string, Dictionary<string, float>> dicControlMetricData,
           Dictionary<string,Dictionary<string,List<float>>> dicBase365,Dictionary<string,Dictionary<string,List<float>>> dicControl365,
           Dictionary<string, ModelResultAttribute> dicControl, Dictionary<string, Dictionary<string, double>> DicAllSetupVariableValues,  Dictionary<string, float> dicPopulationAllAge, Dictionary<string, double> dicIncidenceRateAttribute, Dictionary<string, double> dicPrevalenceRateAttribute, int incidenceDataSetGridType, int PrevalenceDataSetGridType, Dictionary<string, int> dicRace, Dictionary<string, int> dicEthnicity, Dictionary<string, int> dicGender, double Threshold, int LatinHypercubePoints, bool RunInPointMode, List<GridRelationship> lstGridRelationship, CRSelectFunction crSelectFunction, BaseControlGroup baseControlGroup, List<RegionTypeGrid> lstRegionTypeGrid, BenMAPPopulation benMAPPopulation, double[] lhsResultArray)
       {
           try
           {
               // FbConnection _connection = CommonClass.getNewConnection();
               //由每条CRSelectFuntion拼凑出一个 健康冲击 网格；包括 人口数据网格 + Grid+ Base+Control +DeltaQ 每个函数只对一组污染物数据有效 
               //如果输入了location,则只在该location 有效，其他为0 ，以相交作为有效的标准。所以系统一选择Grid就应该初始化数据库里所有的Grid和选择的grid的关系，
               //除掉自己 State County City and Grid 之间的关系。-> regionid list<col ,row>
               //string commandText = "";
               //iCalculateCRID=iCalculateCRID + 1;
               //int sCRID =Convert.ToInt16( iCalculateCRID.ToString());
              // List<string> lstAllAgeID = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "42", };

               CRSelectFunctionCalculateValue crSelectFunctionCalculateValue = new CRSelectFunctionCalculateValue() { CRSelectFunction = crSelectFunction, CRCalculateValues = new List<CRCalculateValue>() };//, ResultCopy = new List<float[]>() };
               //List<double> lstd = new List<double>();


               //GridRelationship gridRelationShip = null;
               try
               {
                   if (benMAPPopulation.GridType.GridDefinitionID != CommonClass.GBenMAPGrid.GridDefinitionID)
                       lstGridRelationship.Where(p => (p.bigGridID == benMAPPopulation.GridType.GridDefinitionID && p.smallGridID == CommonClass.GBenMAPGrid.GridDefinitionID) || (p.smallGridID == benMAPPopulation.GridType.GridDefinitionID && p.bigGridID == CommonClass.GBenMAPGrid.GridDefinitionID)).First();
               }
               catch
               {
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
               if (crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric == null && crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic == MetricStatic.None)
               {
                   i365 = 365;
                   List<SeasonalMetric> lstseasonalMetric = baseControlGroup.Pollutant.SesonalMetrics.Where(p => p.Metric.MetricID == crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricID).ToList();
                   SeasonalMetric seasonalMetric = null;
                   if (lstseasonalMetric.Count > 0)
                       seasonalMetric = lstseasonalMetric.Last();
                   //according to issue 144 - use the length of the seasonal metric season as “days” value to estimate health impact results
                   if (seasonalMetric != null && seasonalMetric.Seasons.Count > 0)
                   {
                       i365 = 0;
                       foreach (Season season in seasonalMetric.Seasons)
                       {
                           i365 = i365 + season.EndDay - season.StartDay + 1;
                           if (season.StartDay < iStartDay) iStartDay = season.StartDay;
                           if (season.EndDay > iEndDay) iEndDay = season.EndDay;
                       }
                   }
                   else
                   {
                       if (crSelectFunction.BenMAPHealthImpactFunction.Pollutant.Seasons != null && crSelectFunction.BenMAPHealthImpactFunction.Pollutant.Seasons.Count != 0)
                       {//不是特别OK，应该判断它的seaons之间的日期

                           i365 = 0;
                           foreach (Season season in crSelectFunction.BenMAPHealthImpactFunction.Pollutant.Seasons)
                           {
                               i365 = i365 + season.EndDay - season.StartDay + 1;
                               if (season.StartDay < iStartDay) iStartDay = season.StartDay;
                               if (season.EndDay > iEndDay) iEndDay = season.EndDay;
                           }

                       }
                   }
               }
               Dictionary<string, double> dicVariable = null;
               double d = 0;
               CRCalculateValue crCalculateValue = new CRCalculateValue();
              
               //----------------------------------------------end Population------------------------------------------------------------------------
               string strBaseLineFunction = ConfigurationCommonClass.getFunctionStringFromDatabaseFunction(crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction);
               bool hasPopInstrBaseLineFunction = crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction.Contains("POP");
               //计算PointEstimate
               string strPointEstimateFunction = ConfigurationCommonClass.getFunctionStringFromDatabaseFunction(crSelectFunction.BenMAPHealthImpactFunction.Function);
               //--------Add by xiejp20131113 For Monitor
               Dictionary<string, MonitorValue> dicBaseMonitor = new Dictionary<string, MonitorValue>();
               Dictionary<string, MonitorValue> dicControlMonitor = new Dictionary<string, MonitorValue>();
               Dictionary<string, List<MonitorNeighborAttribute>> dicAllMonitorNeighborControl = new Dictionary<string, List<MonitorNeighborAttribute>>();
               Dictionary<string, List<MonitorNeighborAttribute>> dicAllMonitorNeighborBase = new Dictionary<string, List<MonitorNeighborAttribute>>();
               

               if (baseControlGroup.Base is MonitorDataLine && baseControlGroup.Control is MonitorDataLine && crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic== MetricStatic.None)
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
               //通过函数计算值(考虑 Incidence Prevalence Variable库对函数的影响，以及是否有region ,如果有region只对region的范围有效）
               foreach (ModelResultAttribute modelResultAttribute in baseControlGroup.Base.ModelResultAttributes)
               {
                   //int col = modelResultAttribute.Col;
                   //int row = modelResultAttribute.Row;
                   //首先查看是否有region的设置，如果有则加入。--------------------------------------------------------
                   //if (crSelectFunction.Locations != null && crSelectFunction.Locations.Count > 0)
                   //{
                   //    //如果该网格cell不在region内则continue
                   //    if (gridRelationShip != null)
                   //    {
                   //        if (crSelectFunction.Locations.First().GridDifinitionID == gridRelationShip.bigGridID)
                   //        {
                   //            //region的网格类型比较大
                   //            RowCol rowCol = new RowCol() { Col = modelResultAttribute.Col, Row = modelResultAttribute.Row };
                   //            var queryrowCol = from a in gridRelationShip.lstGridRelationshipAttribute where a.smallGridRowCol.Contains(rowCol) select new RowCol() { Col = a.bigGridRowCol.Col, Row = a.bigGridRowCol.Row };
                   //            bool bin = false;

                   //            foreach (Location location in crSelectFunction.Locations)
                   //            {
                   //                if (queryrowCol.ToList().Contains(new RowCol() { Row = location.Row, Col = location.Col }))
                   //                {
                   //                    bin = true;
                   //                }
                   //            }
                   //            if (bin == false) continue;

                   //        }
                   //        else
                   //        {
                   //            //Grid的网格比较大-----------------------------------------------------------------未找到解决方法需要再讨论
                   //            /*
                   //             * 从选择CRFunction就屏蔽掉region比分析的网格还小的情况，只能出现比它还大的study。
                   //             * 然后在GIS选择study的时候可选一个层次的所有study，当比它小时，也只对选择的region有效。
                   //             * */

                   //        }

                   //    }

                   //}
                   //Dictionary<string, float> dicMetricValue;
                   //得到base,control,deltaq
                   //---------如果有Seasonal--首先使用Seasonal--如果Seasonal没有Static则用连续值,如果没有则使用相应的值
                   populationValue = 0;
                   incidenceValue = 0;
                   prevalenceValue = 0;
                   //if (dicPopulation != null)
                   //{
                   //    if (dicPopulation.Keys.Contains(Convert.ToInt32(modelResultAttribute.Col) * 10000 + Convert.ToInt32(modelResultAttribute.Row)))
                   //    {
                   //        populationValue = dicPopulation[Convert.ToInt32(modelResultAttribute.Col) * 10000 + Convert.ToInt32(modelResultAttribute.Row)];
                   //    }

                   //}
                   if (dicPopulationAllAge != null)
                   {
                       foreach (KeyValuePair<string,double> s in dicAge)
                       {
                           if(dicPopulationAllAge.Keys.Contains(modelResultAttribute.Col+","+modelResultAttribute.Row+","+s.Key))
                               populationValue+=dicPopulationAllAge[modelResultAttribute.Col+","+modelResultAttribute.Row+","+s.Key]*s.Value;
                       }
                   }
                   if (populationValue == 0)
                       continue;
                   //if (dicIncidenceValue != null) dicIncidenceValue.Clear();
                   //if (dicPrevalenceValue != null) dicPrevalenceValue.Clear();
                   //if (dicPopValue != null) dicPopValue.Clear();
                   dicIncidenceValue = null;// new Dictionary<string, double>();
                   dicPrevalenceValue = null;//new Dictionary<string, double>();
                   dicPopValue = null;//new Dictionary<string, double>();
                   dicIncidenceValue=new Dictionary<string,double>();
                   dicPrevalenceValue = new Dictionary<string, double>();
                   dicPopValue = new Dictionary<string, double>();
                   if (dicIncidenceRateAttribute != null)
                   {
                       foreach (string s in lstAllAgeID)
                       {
                           if (dicIncidenceRateAttribute.Keys.Contains((Convert.ToInt32(modelResultAttribute.Col) * 10000 + Convert.ToInt32(modelResultAttribute.Row)).ToString()+","+s))
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
                           if (dicPopulationAllAge.Keys.Contains(modelResultAttribute.Col+"," + modelResultAttribute.Row + "," + s))
                           {
                               dicPopValue.Add(s, dicPopulationAllAge[modelResultAttribute.Col + "," + modelResultAttribute.Row + "," + s]*dicAge[s]);
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

                   //if(dicBaseMetricData[modelResultAttribute.Col+","+modelResultAttribute.Row].ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
                   //baseValue = modelResultAttribute.Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];
                   if (crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
                   {
                       //-------
                       if (crSelectFunction.BenMAPHealthImpactFunction.MetricStatistic != MetricStatic.None)
                       {
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
                               //------------没有--则不需要往下计算直接为0-------------------
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
                       else
                       {
                           //------------有连续值，使用连续值，没有连续值，使用某值*SeasonsCount----------
                           if (dicBase365.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row) &&
                               dicBase365[modelResultAttribute.Col + "," + modelResultAttribute.Row].ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName)
                               && dicControl365.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row) &&
                               dicControl365[modelResultAttribute.Col + "," + modelResultAttribute.Row].ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
                           {

                               //----------一个一个算！
                               float fPSum = 0, fBaselineSum = 0;
                               List<float> lstFPSum = new List<float>();
                               if (lhsResultArray != null)
                               {
                                   foreach (double dlhs in lhsResultArray)
                                   {
                                       lstFPSum.Add(0);
                                   }
                               }
                               for (int iBase = 0; iBase < dicBase365[modelResultAttribute.Col + "," + modelResultAttribute.Row][crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName].Count; iBase++)
                               {
                                   double fBase, fControl, fDelta;
                                   fBase = dicBase365[modelResultAttribute.Col + "," + modelResultAttribute.Row][crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName][iBase];
                                   fControl = dicControl365[modelResultAttribute.Col + "," + modelResultAttribute.Row][crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName][iBase];
                                   if (fBase != float.MinValue && fControl != float.MinValue)
                                   {
                                       //---do!
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
                                   Delta = 0,//-------可能需要修改-
                                   Incidence = Convert.ToSingle(incidenceValue),
                                   PointEstimate = fPSum,
                                   LstPercentile = lstFPSum,
                                   Population = Convert.ToSingle(populationValue),
                                   Mean = lstFPSum.Count() == 0 ? float.NaN : getMean(lstFPSum),
                                   //StandardDeviation = lstFPSum.Count() == 0 ? float.NaN : getStandardDeviation(lstFPSum, fPSum),
                                   Variance = lstFPSum.Count() == 0 ? float.NaN : getVariance(lstFPSum, fPSum),
                                   Baseline = fBaselineSum,
                                   //PercentOfBaseline = Convert.ToSingle(Math.Round((crCalculateValue.Mean / crCalculateValue.Baseline) * 100, 4))
                               };
                               //--------
                               crCalculateValue.StandardDeviation = lstFPSum.Count() == 0 ? float.NaN : Convert.ToSingle(Math.Sqrt(crCalculateValue.Variance));
                               crCalculateValue.PercentOfBaseline = crCalculateValue.Baseline == 0 ? 0 : Convert.ToSingle(Math.Round((crCalculateValue.Mean / crCalculateValue.Baseline) * 100, 4));
                               double baseValueForDelta = modelResultAttribute.Values[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName];
                               double controlValueForDelta = baseValueForDelta;

                               if (dicControl.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row))// != null)
                               {

                                   if (dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values.Keys.Contains(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
                                       controlValueForDelta = dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName];// querycontrol.First().Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];


                               }
                               if (Threshold != 0 && baseValueForDelta < Threshold)
                                   baseValueForDelta = Threshold;

                               if (Threshold != 0 && controlValueForDelta < Threshold)
                                   controlValueForDelta = Threshold;
                               crCalculateValue.Delta = Convert.ToSingle(baseValueForDelta - controlValueForDelta);
                               crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);
                               continue;

                           }
                           else//-----------直接使用某值 *SeasonsCount
                           {
                               baseValue = modelResultAttribute.Values[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName];
                               controlValue = baseValue;

                               if (dicControl.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row))// != null)
                               {

                                   if (dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values.Keys.Contains(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
                                       controlValue = dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName];// querycontrol.First().Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];


                               }
                               //---Start modify by xiejp 20131113 For Monitor
                               #region
                               if (baseControlGroup.Base is MonitorDataLine && baseControlGroup.Control is MonitorDataLine && baseValue != controlValue && dicAllMonitorNeighborBase != null
                                   && dicAllMonitorNeighborBase.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row)
                                   && dicAllMonitorNeighborControl != null && dicAllMonitorNeighborBase.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row))
                               {
                                   //---Get the Base Result---
                                   //double dfm = 0;
                                   List<float> lstdfmBase = new List<float>();

                                   
                                   foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborBase[modelResultAttribute.Col+","+modelResultAttribute.Row])
                                   {
                                       //dfm += dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] * mnAttribute.Weight;
                                       // dfz += 1.0000 / k.Value;
                                       if (lstdfmBase.Count == 0)
                                       {
                                           if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
                                           {
                                               lstdfmBase = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName].Select(p => p == float.MinValue ? 0 : Convert.ToSingle(p * mnAttribute.Weight)).ToList();
                                               //lstdfz = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.SeasonalMetricName].Select(p => p == float.MinValue ? 0 : 1 / k.Value).ToList();

                                           }
                                       }
                                       else
                                       {
                                           if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
                                           {
                                               for (int idfm = 0; idfm < lstdfmBase.Count; idfm++)
                                               {
                                                   lstdfmBase[idfm] += dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName][idfm] == float.MinValue ? 0 : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName][idfm] * Convert.ToSingle(mnAttribute.Weight);
                                                   // lstdfz[idfm] += k.Key.dicMetricValues365[dicsd.Key][idfm] == float.MinValue ? 0 : 1 / k.Value;

                                               }
                                           }
                                       }
                                   }

                                   //dfm = 0;
                                   List<float> lstdfmControl = new List<float>();


                                   foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborControl[modelResultAttribute.Col + "," + modelResultAttribute.Row])
                                   {
                                       //dfm += dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName] * mnAttribute.Weight;
                                       // dfz += 1.0000 / k.Value;
                                       if (lstdfmControl.Count == 0)
                                       {
                                           if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
                                           {
                                               lstdfmControl = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName].Select(p => p == float.MinValue ? 0 : Convert.ToSingle(p * mnAttribute.Weight)).ToList();
                                               //lstdfz = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.SeasonalMetricName].Select(p => p == float.MinValue ? 0 : 1 / k.Value).ToList();

                                           }
                                       }
                                       else
                                       {
                                           if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
                                           {
                                               for (int idfm = 0; idfm < lstdfmControl.Count; idfm++)
                                               {
                                                   lstdfmControl[idfm] += dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName][idfm] == float.MinValue ? 0 : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName][idfm] * Convert.ToSingle(mnAttribute.Weight);
                                                   // lstdfz[idfm] += k.Key.dicMetricValues365[dicsd.Key][idfm] == float.MinValue ? 0 : 1 / k.Value;

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


                                       for (int iBase = iStartDay; iBase < iEndDay; iBase++)
                                       {
                                           double fBase, fControl, fDelta;
                                           fBase = lstdfmBase[iBase];
                                           fControl = lstdfmControl[iBase];
                                           if (fBase != 0 && fControl != 0)
                                           {
                                               //---do!
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
                                       Delta = 0,//-------可能需要修改-
                                       Incidence = Convert.ToSingle(incidenceValue),
                                       PointEstimate = fPSum,
                                       LstPercentile = lstFPSum,
                                       Population = Convert.ToSingle(populationValue),
                                       Mean = lstFPSum.Count() == 0 ? float.NaN : getMean(lstFPSum),
                                       //StandardDeviation = lstFPSum.Count() == 0 ? float.NaN : getStandardDeviation(lstFPSum, fPSum),
                                       Variance = lstFPSum.Count() == 0 ? float.NaN : getVariance(lstFPSum, fPSum),
                                       Baseline = fBaselineSum,
                                       //PercentOfBaseline = Convert.ToSingle(Math.Round((crCalculateValue.Mean / crCalculateValue.Baseline) * 100, 4))
                                   };
                                   crCalculateValue.StandardDeviation = lstFPSum.Count() == 0 ? float.NaN : Convert.ToSingle(Math.Sqrt(crCalculateValue.Variance));
                                   crCalculateValue.PercentOfBaseline = crCalculateValue.Baseline == 0 ? 0 : Convert.ToSingle(Math.Round((crCalculateValue.Mean / crCalculateValue.Baseline) * 100, 4));
                                   double baseValueForDelta = modelResultAttribute.Values[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName];
                                   double controlValueForDelta = baseValueForDelta;

                                   if (dicControl.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row))// != null)
                                   {

                                       if (dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values.Keys.Contains(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
                                           controlValueForDelta = dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName];// querycontrol.First().Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.SeasonalMetricName];


                                   }
                                   if (Threshold != 0 && baseValueForDelta < Threshold)
                                       baseValueForDelta = Threshold;

                                   if (Threshold != 0 && controlValueForDelta < Threshold)
                                       controlValueForDelta = Threshold;
                                   crCalculateValue.Delta = Convert.ToSingle(baseValueForDelta - controlValueForDelta);
                                   crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);


                                   continue;
                               }
                               #endregion
                               //---End modify by xiejp 20131113 For Monitor
                               baseValue = modelResultAttribute.Values[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName];
                               controlValue = baseValue;

                               if (dicControl.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row))// != null)
                               {

                                   if (dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values.Keys.Contains(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
                                       controlValue = dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName];// querycontrol.First().Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];


                               }
                               i365 = crSelectFunction.BenMAPHealthImpactFunction.Pollutant.Seasons.Count();
                           }
                       }

                   }
                   else
                   {
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
                           else
                           {
                               //------------没有--则不需要往下计算直接为0-------------------
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
                       else
                       {
                           //------------有连续值，使用连续值，没有连续值，使用某值*SeasonsCount----------
                           if (dicBase365.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row) &&
                               dicBase365[modelResultAttribute.Col + "," + modelResultAttribute.Row].ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName)
                               && dicControl365.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row) &&
                               dicControl365[modelResultAttribute.Col + "," + modelResultAttribute.Row].ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
                           {

                               //----------一个一个算！
                               float fPSum = 0, fBaselineSum = 0;
                               List<float> lstFPSum = new List<float>();
                               if (lhsResultArray != null)
                               {
                                   foreach (double dlhs in lhsResultArray)
                                   {
                                       lstFPSum.Add(0);
                                   }
                               }
                               for (int iBase = 0; iBase < dicBase365[modelResultAttribute.Col + "," + modelResultAttribute.Row][crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName].Count; iBase++)
                               {
                                   double fBase, fControl, fDelta;
                                   fBase = dicBase365[modelResultAttribute.Col + "," + modelResultAttribute.Row][crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName][iBase];
                                   fControl = dicControl365[modelResultAttribute.Col + "," + modelResultAttribute.Row][crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName][iBase];
                                   if (fBase != float.MinValue && fControl != float.MinValue)
                                   {
                                       //---do!
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
                                   Delta = 0,//-------可能需要修改-
                                   Incidence = Convert.ToSingle(incidenceValue),
                                   PointEstimate = fPSum,
                                   LstPercentile = lstFPSum,
                                   Population = Convert.ToSingle(populationValue),
                                   Mean = lstFPSum.Count() == 0 ? float.NaN : getMean(lstFPSum),
                                   //StandardDeviation = lstFPSum.Count() == 0 ? float.NaN : getStandardDeviation(lstFPSum, fPSum),
                                   Variance = lstFPSum.Count() == 0 ? float.NaN : getVariance(lstFPSum, fPSum),
                                   Baseline = fBaselineSum,
                                   //PercentOfBaseline = Convert.ToSingle(Math.Round((crCalculateValue.Mean / crCalculateValue.Baseline) * 100, 4))
                               };
                               crCalculateValue.StandardDeviation = lstFPSum.Count() == 0 ? float.NaN : Convert.ToSingle(Math.Sqrt(crCalculateValue.Variance));
                               crCalculateValue.PercentOfBaseline = crCalculateValue.Baseline == 0 ? 0 : Convert.ToSingle(Math.Round((crCalculateValue.Mean / crCalculateValue.Baseline) * 100, 4));
                               double baseValueForDelta = modelResultAttribute.Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];
                               double controlValueForDelta = baseValueForDelta;

                               if (dicControl.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row))// != null)
                               {

                                   if (dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values.Keys.Contains(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
                                       controlValueForDelta = dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];// querycontrol.First().Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];


                               }
                               if (Threshold != 0 && baseValueForDelta < Threshold)
                                   baseValueForDelta = Threshold;

                               if (Threshold != 0 && controlValueForDelta < Threshold)
                                   controlValueForDelta = Threshold;
                               crCalculateValue.Delta =Convert.ToSingle( baseValueForDelta - controlValueForDelta);
                               crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);
                               continue;

                           }
                           else//-----------直接使用某值 *SeasonsCount
                           {
                               baseValue = modelResultAttribute.Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];
                               controlValue = baseValue;

                               if (dicControl.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row))// != null)
                               {

                                   if (dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values.Keys.Contains(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
                                       controlValue = dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];// querycontrol.First().Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];


                               }
                               //---Start modify by xiejp 20131113 For Monitor
                               #region
                               if (baseControlGroup.Base is MonitorDataLine && baseControlGroup.Control is MonitorDataLine && baseValue != controlValue && dicAllMonitorNeighborBase != null 
                                   && dicAllMonitorNeighborBase.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row)
                                   && dicAllMonitorNeighborControl!=null &&dicAllMonitorNeighborBase.ContainsKey(modelResultAttribute.Col + "," + modelResultAttribute.Row))
                               {
                                   //---Get the Base Result---
                                   //double dfm = 0;  
                                   List<float> lstdfmBase = new List<float>();

                                   foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborBase[modelResultAttribute.Col + "," + modelResultAttribute.Row])
                                   {
                                       //dfm += dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] *mnAttribute.Weight;
                                      // dfz += 1.0000 / k.Value;
                                       if (lstdfmBase.Count == 0)
                                       {
                                           if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
                                           {
                                               lstdfmBase = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName].Select(p => p == float.MinValue ? 0 :Convert.ToSingle( p *mnAttribute.Weight)).ToList();
                                               //lstdfz = dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName].Select(p => p == float.MinValue ? 0 : 1 / k.Value).ToList();

                                           }
                                       }
                                       else
                                       {
                                           if (dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
                                           {
                                               for (int idfm = 0; idfm < lstdfmBase.Count; idfm++)
                                               {
                                                   lstdfmBase[idfm] += dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName][idfm] == float.MinValue ? 0 : dicBaseMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName][idfm] *Convert.ToSingle( mnAttribute.Weight);
                                                  // lstdfz[idfm] += k.Key.dicMetricValues365[dicsd.Key][idfm] == float.MinValue ? 0 : 1 / k.Value;

                                               }
                                           }
                                       }
                                   }

                                   //dfm = 0;  
                                   List<float> lstdfmControl = new List<float>();

                                   foreach (MonitorNeighborAttribute mnAttribute in dicAllMonitorNeighborControl[modelResultAttribute.Col + "," + modelResultAttribute.Row])
                                   {
                                       //dfm += dicControlMonitor[mnAttribute.MonitorName].dicMetricValues[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName] * mnAttribute.Weight;
                                       // dfz += 1.0000 / k.Value;
                                       if (lstdfmControl.Count == 0)
                                       {
                                           if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
                                           {
                                               lstdfmControl = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName].Select(p => p == float.MinValue ? 0 : Convert.ToSingle(p * mnAttribute.Weight)).ToList();
                                               //lstdfz = dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName].Select(p => p == float.MinValue ? 0 : 1 / k.Value).ToList();

                                           }
                                       }
                                       else
                                       {
                                           if (dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365 != null && dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365.ContainsKey(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
                                           {
                                               for (int idfm = 0; idfm < lstdfmControl.Count; idfm++)
                                               {
                                                   lstdfmControl[idfm] += dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName][idfm] == float.MinValue ? 0 : dicControlMonitor[mnAttribute.MonitorName].dicMetricValues365[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName][idfm] * Convert.ToSingle(mnAttribute.Weight);
                                                   // lstdfz[idfm] += k.Key.dicMetricValues365[dicsd.Key][idfm] == float.MinValue ? 0 : 1 / k.Value;

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


                                      
                                       for (int iBase = iStartDay; iBase < iEndDay; iBase++)
                                       {
                                           double fBase, fControl, fDelta;
                                           fBase = lstdfmBase[iBase];
                                           fControl = lstdfmControl[iBase];
                                           if (fBase != 0 && fControl != 0)
                                           {
                                               //---do!
                                               if (Threshold != 0 && fBase < Threshold)
                                                   fBase = Threshold;
                                               if (fControl != 0 && fControl < Threshold)
                                                   fControl = Threshold;
                                               fDelta = fBase - fControl;
                                               if(fDelta!=0)
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
                                       Delta = 0,//-------可能需要修改-
                                       Incidence = Convert.ToSingle(incidenceValue),
                                       PointEstimate = fPSum,
                                       LstPercentile = lstFPSum,
                                       Population = Convert.ToSingle(populationValue),
                                       Mean = lstFPSum.Count() == 0 ? float.NaN : getMean(lstFPSum),
                                       //StandardDeviation = lstFPSum.Count() == 0 ? float.NaN : getStandardDeviation(lstFPSum, fPSum),
                                       Variance = lstFPSum.Count() == 0 ? float.NaN : getVariance(lstFPSum, fPSum),
                                       Baseline = fBaselineSum,
                                       //PercentOfBaseline = Convert.ToSingle(Math.Round((crCalculateValue.Mean / crCalculateValue.Baseline) * 100, 4))
                                   };
                                   crCalculateValue.StandardDeviation = lstFPSum.Count() == 0 ? float.NaN : Convert.ToSingle(Math.Sqrt(crCalculateValue.Variance));
                                   crCalculateValue.PercentOfBaseline = crCalculateValue.Baseline == 0 ? 0 : Convert.ToSingle(Math.Round((crCalculateValue.Mean / crCalculateValue.Baseline) * 100, 4));
                                   double baseValueForDelta = modelResultAttribute.Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];
                                   double controlValueForDelta = baseValueForDelta;

                                   if (dicControl.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row))// != null)
                                   {

                                       if (dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values.Keys.Contains(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
                                           controlValueForDelta = dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];// querycontrol.First().Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];


                                   }
                                   if (Threshold != 0 && baseValueForDelta < Threshold)
                                       baseValueForDelta = Threshold;

                                   if (Threshold != 0 && controlValueForDelta < Threshold)
                                       controlValueForDelta = Threshold;
                                   crCalculateValue.Delta = Convert.ToSingle(baseValueForDelta - controlValueForDelta);
                                   crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);


                                   continue;
                               }
                               #endregion
                               //---End modify by xiejp 20131113 For Monitor
                              
                           }
                       }
 
                   }
                   if (Threshold != 0 && baseValue < Threshold)
                       baseValue = Threshold;
                   //var querycontrol = from a in baseControlGroup.Control.ModelResultAttributes where a.Col == modelResultAttribute.Col && a.Row == modelResultAttribute.Row select a;

                   //controlValue = baseValue;
                  
                   //if (dicControl.Keys.Contains(modelResultAttribute.Col + "," + modelResultAttribute.Row))// != null)
                   //{
                   //    if (dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values.Keys.Contains(crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName))
                   //        controlValue = dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];// querycontrol.First().Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];
                   //    if (crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric != null)
                   //    {
                   //        if (dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values.Keys.Contains(crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName))
                   //            controlValue = dicControl[modelResultAttribute.Col + "," + modelResultAttribute.Row].Values[crSelectFunction.BenMAPHealthImpactFunction.SeasonalMetric.SeasonalMetricName];// querycontrol.First().Values[crSelectFunction.BenMAPHealthImpactFunction.Metric.MetricName];

                   //    }
                   //}
                   if (Threshold != 0 && controlValue < Threshold)
                       controlValue = Threshold;
                   deltaQValue = baseValue - controlValue;


                   
                 
                   //计算公式---------------------------------------------------------------------------
                   {
                       crCalculateValue = CalculateCRSelectFunctionsOneCel(sCRID, hasPopInstrBaseLineFunction, i365, crSelectFunction, strBaseLineFunction, strPointEstimateFunction, modelResultAttribute.Col, modelResultAttribute.Row, baseValue, controlValue, dicPopValue, dicIncidenceValue, dicPrevalenceValue, dicVariable, lhsResultArray);

                       crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);
                   }
                   dicVariable = null;
               }

               CommonClass.BaseControlCRSelectFunctionCalculateValue.lstCRSelectFunctionCalculateValue.Add(crSelectFunctionCalculateValue);
               //if (DicAllSetupVariableValues != null) DicAllSetupVariableValues.Clear();
               DicAllSetupVariableValues = null;
               //if (dicControl != null) dicControl.Clear();
               dicControl = null;
               //if (dicVariable != null) dicVariable.Clear();
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
           //crSelectFunctionCalculateValue.CRCalculateValues = new List<CRCalculateValue>();
           //CRCalculateValue crCalculateValue = new CRCalculateValue();
           //bool islhs = false;
           //if (crSelectFunctionCalculateValue.ResultCopy!=null && crSelectFunctionCalculateValue.ResultCopy.Count > 0 && crSelectFunctionCalculateValue.ResultCopy.First().Length>11)
           //{
           //    islhs = true;

           //}
           //foreach (float[] dArray in crSelectFunctionCalculateValue.ResultCopy)
           //{
           //    crCalculateValue = new CRCalculateValue();
           //    crCalculateValue.Col = Convert.ToInt32(dArray[0]);
           //    crCalculateValue.Row = Convert.ToInt32(dArray[1]);
           //    crCalculateValue.PointEstimate = dArray[2];
           //    crCalculateValue.Population = dArray[3];
           //    crCalculateValue.Incidence = dArray[4];
           //    crCalculateValue.Delta = dArray[5];
           //    crCalculateValue.Mean = dArray[6];
           //    crCalculateValue.Baseline = dArray[7];
           //    crCalculateValue.PercentOfBaseline = dArray[8];
           //    crCalculateValue.StandardDeviation = dArray[9];
           //    crCalculateValue.Variance = dArray[10];
           //    if (islhs)
           //    {
           //        crCalculateValue.LstPercentile = dArray.ToList().GetRange(11, dArray.Length - 11);
           //    }
           //    crSelectFunctionCalculateValue.CRCalculateValues.Add(crCalculateValue);
           //}
       }

       /// <summary>
       /// 生成某网格的系统变量Dictionary
       /// </summary>
       /// <param name="Col"></param>
       /// <param name="Row"></param>
       /// <param name="lstVariableJoin"></param>
       /// <param name="GridDefinitionID"></param>
       /// <param name="lstGridRelationship"></param>
       /// <returns></returns>
       public static Dictionary<string, double> getDicSetupVariableColRow(int Col, int Row, List<SetupVariableJoinAllValues> lstVariableJoin, int GridDefinitionID, List<GridRelationship> lstGridRelationship)
       {
           try
           {
               Dictionary<string, double> dicResult = new Dictionary<string, double>();
               foreach (SetupVariableJoinAllValues setupVariableJoinAllValue in lstVariableJoin)
               {
                   //求incidenceValue
                   //如果网格类型相同则取同样的Row Col的数据
                   if (setupVariableJoinAllValue.SetupVariableGridType == GridDefinitionID)
                   {
                       var queryVariable = from a in setupVariableJoinAllValue.lstValues where a.Col == Col && a.Row == Row select a;
                       //暂不考虑startAge,endAge
                       double values = 0;
                       foreach (SetupVariableValues iRateAttributes in queryVariable)
                       {
                           values += iRateAttributes.Value;

                       }
                       if (queryVariable.Count() > 0) values = values / Convert.ToDouble(queryVariable.Count());
                       dicResult.Add(setupVariableJoinAllValue.SetupVariableName, values);
                   }
                   else//如果不是一个Grid，则需要查询用哪些RowCol------------------------------------------------------------
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
                       //--------------如果Grid比incidenceDataSetGridType大，则使用incidence平均，如果比incidenceDataSetGridType小，则直接选大的那个
                       if (setupVariableJoinAllValue.SetupVariableGridType == gridRelationShipVariable.bigGridID)
                       {
                           //region的网格类型比较大

                           var queryrowCol = from a in gridRelationShipVariable.lstGridRelationshipAttribute where a.smallGridRowCol.Contains(rowCol, new RowColComparer()) select new RowCol() { Col = a.bigGridRowCol.Col, Row = a.bigGridRowCol.Row };
                           lstRowCol = queryrowCol.ToList();
                           var queryVariable = from a in setupVariableJoinAllValue.lstValues where lstRowCol.Contains(new RowCol() { Col = a.Col, Row = a.Row }, new RowColComparer()) select a;
                           //暂不考虑startAge,endAge
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
                           //RowCol rowCol = new RowCol() { Col = col, Row = row };
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
                               //暂不考虑startAge,endAge
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
     
       /// <summary>
       /// 查某一网格的Incidence Estimation值
       /// </summary>
       /// <param name="crSelectFunction"></param>
       /// <param name="PointEstimate"></param>
       /// <returns></returns>
       public static CRCalculateValue CalculateCRSelectFunctionsOneCel(string iCRID,bool hasPopInstrBaseLineFunction, float i365, CRSelectFunction crSelectFunction, string strBaseLineFunction, string strPointEstimateFunction, int col, int row, double baseValue, double controlValue, Dictionary<string, double> dicPopulationValue, Dictionary<string, double> dicIncidenceValue, Dictionary<string, double> dicPrevalenceValue, Dictionary<string, double> dicSetupVariables, double[] lhsDesignResult)
       {
           try
           {
               //double deltaQ=baseValue-controlValue;
               double incidenceValue, prevalenceValue, PopValue;
               //bool isIncidence=true,isPrevalence=true;
               //if (dicIncidenceValue == null || dicIncidenceValue.Count == 0) isIncidence = false;
               //if (dicPrevalenceValue == null || dicPrevalenceValue.Count == 0) isPrevalence = false;

               CRCalculateValue crCalculateValue = new CRCalculateValue()
               {
                   Col = col,
                   Row = row,
                   Population = Convert.ToSingle( dicPopulationValue!=null?dicPopulationValue.Sum(p=>p.Value):0 ),
                    Incidence=0,// Convert.ToSingle( dicIncidenceValue),
                   Delta = Convert.ToSingle( baseValue - controlValue )


               };
               //if (dicPrevalenceValue != 0) crCalculateValue.Incidence =Convert.ToSingle( dicPrevalenceValue);
               //double A = crSelectFunction.BenMAPHealthImpactFunction.AContantValue;
               //double B = crSelectFunction.BenMAPHealthImpactFunction.BContantValue;
               //double C = crSelectFunction.BenMAPHealthImpactFunction.CContantValue;
               //double Beta = crSelectFunction.BenMAPHealthImpactFunction.Beta;
               //double DELTAQ = baseValue - controlValue;
               //double DeltaQ = baseValue - controlValue;
               //double Q0 = baseValue;
               //double Q1 = controlValue;
               //double Incidence = incidenceValue;
               //double POP = populationValue;
               //double Prevalence = prevalenceValue;

               //string[] AvailableCompiledFunctions = new string[] { "(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP", "(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*A", "(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*A*B", "(1-(1/EXP(Beta*DELTAQ)))*A*POP", "(1-(1/EXP(Beta*(MAX(Q1,C)-MAX(Q0,C)))))*Incidence*POP", "(1-(1/EXP(Beta*(MAX(Q1,C)-MAX(Q0,C)))))*A*POP", "(1-(1/((1-Incidence)*EXP(Beta*DeltaQ)+Incidence)))*Incidence*POP", "(1-(1/((1-A)*EXP(Beta*DeltaQ)+A)))*A*POP", "(1-(1/((1-A)*EXP(Beta*DeltaQ)+A)))*A*POP*B", "(1-(1/((1-Incidence)*EXP(Beta*DeltaQ)+Incidence)))*Incidence*POP*(1-Prevalence)", "(1-(1/((1-Incidence*A)*EXP(Beta*DeltaQ)+Incidence*A)))*Incidence*A*POP", "(1-(1/((1-Incidence)*EXP(Beta*(MAX(Q1,C)-MAX(Q0,C)))+Incidence)))*Incidence*POP", "(1-(1/((1-A)*EXP(Beta*(MAX(Q1,C)-MAX(Q0,C)))+A)))*A*POP", "(1-(1/((1-A)*EXP(Beta*(MAX(Q1,C)-MAX(Q0,C)))+A)))*A*POP*B", "(1-(1/((1-Incidence)*EXP(Beta*(MAX(Q1,C)-MAX(Q0,C)))+Incidence)))*Incidence*POP*(1-Prevalence)", "(1-(1/((1-Incidence*A)*EXP(Beta*(MAX(Q1,C)-MAX(Q0,C)))+Incidence*A)))*Incidence*A*POP", "if ((Q1>A) and  (Q1<=B)) then result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP else result := 0", "if (Q1>A) then result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP else result := 0", "if (Q1<5) then Result := 0 else if ((Q1>=5)and(Q1<6)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.01 else if ((Q1>=6) and (Q1<7)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.05 else if ((Q1>=7) and (Q1<8)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.10 else if ((Q1>=8) and (Q1<9)) then Result :=(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.18 else if ((Q1>=9) and (Q1<10)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.28 else if ((Q1>=10) and (Q1<11)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.41 else if ((Q1>=11) and (Q1<12)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.56 else if ((Q1>=12) and (Q1<13)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.72 else if ((Q1>=13) and (Q1<14)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.88 else if ((Q1>=14) and (Q1<15)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.97 else if (Q1>=15) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP else Result := 0", "if (Q1 <> 0) then Result := Beta*((Q1-Q0)/Q1)*DAILYWAGEOUTDOOR*(MEDIAN_INCOME/NATL_MEDIAN_INCOME)*POP*(COUNT_FARM_EMPLOYED/POPULATION18TO64)", "((Beta)*(sqr(Q1)-sqr(Q0))*POP)/A", "(Beta/A)*DELTAQ*POP", "Beta*C*DELTAQ*Incidence/B*POP*A", "Beta", "if ((Q1>=A) and  (Q1<=B)) then result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP else result := 0", "if ((Q1<A) or (Q1>B)) then Result :=0 else if ((Q1>=5)and(Q1<6)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.0 else if ((Q1>=6) and (Q1<7)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.2 else if ((Q1>=7) and (Q1<8)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.40 else if ((Q1>=8) and (Q1<9)) then Result :=(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.6 else if ((Q1>=9) and (Q1<10)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.8 else if (Q1>=10) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP else Result := 0", "if ((Q1<A) or (Q1>B)) then Result :=0 else if ((Q1>=0)and(Q1<1)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0 else if ((Q1>=1) and (Q1<2)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.2 else if ((Q1>=2) and (Q1<3)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.40 else if ((Q1>=3) and (Q1<4)) then Result :=(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.6 else if ((Q1>=4) and (Q1<5)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.8 else if (Q1>=5) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP else Result := 0", "if ((Q1<A) or (Q1>B)) then Result :=0 else Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP", "if ((Q1<=A) or (Q1>B)) then Result :=0 else Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP", "(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*B", "(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*C", "if (Q1>A) then result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*B else result := 0", "if ((Q1>A) and  (Q1<=B)) then result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*C else result := 0", "if (Q1>A) then result := (1-(1/((1-Incidence)*EXP(Beta*DELTAQ)+Incidence)))*Incidence*POP*B else result := 0", "(1-(1/((1-Incidence)*EXP(Beta*DeltaQ)+Incidence)))*Incidence*POP*A", "if ((Q1>A) and  (Q1<=B)) then result := (1-(1/((1-Incidence)*EXP(Beta*DELTAQ)+Incidence)))*Incidence*POP*C else result := 0", "POP/365", "(1-EXP(-Beta*(Q1-Q0))*fCRFunction.A*fCurPopulation*fCRFunction.B", "(1-EXP(-Beta*(Q1-Q0))*fCurIncidence*fCurPopulation", "(fCRFunction.A-(fCRFunction.A/((1-fCRFunction.A)*EXP(Beta*(Q1-Q0))+fCRFunction.A)))*fCurPopulation", "(fCRFunction.A-(fCRFunction.A/((1-fCRFunction.A)*EXP(Beta*(Q1-Q0))+fCRFunction.A)))*fCurPopulation*fCRFunction.B", "(fCurIncidence-(fCurIncidence/((1-fCurIncidence)*EXP(Beta*(Q1-Q0))+fCurIncidence)))*fCurPopulation" };
               //string[] BaselineAvailableCompiledFunctions = new string[] { "Incidence*POP", "Incidence*POP*A", "Incidence*POP*A*B", "A*POP", "A*POP*B", "Incidence*POP*(1-Prevalence)", "if ((Q1>A) and  (Q1<=B)) then result := Incidence*POP else result := 0", "if (Q1>A) then result := Incidence*POP else result := 0", "DAILYWAGEOUTDOOR*(MEDIAN_INCOME/NATL_MEDIAN_INCOME)*POP*(COUNT_FARM_EMPLOYED/POPULATION18TO64)", "Incidence/B*POP*A", "Prevalence", "Incidence", "A*POP*Prevalence" };
               //先考虑存在的可以写死的公式
               #region Calculate Baseline
               //switch (crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction)
               //{
               //    case "Incidence*POP":
               //        crCalculateValue.Baseline = Incidence * POP;
               //        break;
               //    case "Incidence*POP*A":
               //        crCalculateValue.Baseline = Incidence * POP * A;
               //        break;
               //    case "Incidence*POP*A*B":
               //        crCalculateValue.Baseline = Incidence * POP * A * B;
               //        break;
               //    case "A*POP":
               //        crCalculateValue.Baseline = A * POP;
               //        break;
               //    case "A*POP*B":
               //        crCalculateValue.Baseline = A * POP * B;
               //        break;
               //    case "Incidence*POP*(1-Prevalence)":
               //        crCalculateValue.Baseline = Incidence * POP * (1 - Prevalence);
               //        break;
               //    case "if ((Q1>A) and  (Q1<=B)) then result := Incidence*POP else result := 0":
               //        if ((Q1 > A) && (Q1 <= B))
               //            crCalculateValue.Baseline = Incidence * POP;
               //        else
               //            crCalculateValue.Baseline = 0;

               //        break;
               //    case "if (Q1>A) then result := Incidence*POP else result := 0":
               //        if (Q1 > A)
               //            crCalculateValue.Baseline = Incidence * POP;
               //        else
               //            crCalculateValue.Baseline = 0;
               //        break;
               //    case "DAILYWAGEOUTDOOR*(MEDIAN_INCOME/NATL_MEDIAN_INCOME)*POP*(COUNT_FARM_EMPLOYED/POPULATION18TO64)"://不知道怎么计算
               //        break;
               //    case "Incidence/B*POP*A":
               //        crCalculateValue.Baseline = Incidence / B * POP * A;
               //        break;
               //    case "Prevalence":
               //        crCalculateValue.Baseline = Prevalence;
               //        break;
               //    case "Incidence":
               //        crCalculateValue.Baseline = Incidence;
               //        break;
               //    case "A*POP*Prevalence":
               //        crCalculateValue.Baseline = A * POP * Prevalence;
               //        break;
               //    default:
               //        break;

               //}
               #endregion
               #region CalculatePointEstimate
               //switch (crSelectFunction.BenMAPHealthImpactFunction.Function)
               //{
               //    case "(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP":
               //        crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP;
               //        break;
               //    case "(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*A":
               //        crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * A;
               //        break;
               //    case "(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*A*B":
               //        crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * A * B;
               //        break;
               //    case "(1-(1/EXP(Beta*DELTAQ)))*A*POP":
               //        crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * A * POP;
               //        break;
               //    case "(1-(1/EXP(Beta*(MAX(Q1,C)-MAX(Q0,C)))))*Incidence*POP":
               //        crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * (Math.Max(Q1, C) - Math.Max(Q0, C))))) * Incidence * POP;
               //        break;
               //    case "(1-(1/EXP(Beta*(MAX(Q1,C)-MAX(Q0,C)))))*A*POP":
               //        crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * (Math.Max(Q1, C) - Math.Max(Q0, C))))) * A * POP;
               //        break;
               //    case "(1-(1/((1-Incidence)*EXP(Beta*DeltaQ)+Incidence)))*Incidence*POP":
               //        crCalculateValue.PointEstimate = (1 - (1 / ((1 - Incidence) * Math.Exp(Beta * DeltaQ) + Incidence))) * Incidence * POP;
               //        break;
               //    case "(1-(1/((1-A)*EXP(Beta*DeltaQ)+A)))*A*POP":
               //        crCalculateValue.PointEstimate = (1 - (1 / ((1 - A) * Math.Exp(Beta * DeltaQ) + A))) * A * POP;
               //        break;
               //    case "(1-(1/((1-A)*EXP(Beta*DeltaQ)+A)))*A*POP*B":
               //        crCalculateValue.PointEstimate = (1 - (1 / ((1 - A) * Math.Exp(Beta * DeltaQ) + A))) * A * POP * B;
               //        break;
               //    case "(1-(1/((1-Incidence)*EXP(Beta*DeltaQ)+Incidence)))*Incidence*POP*(1-Prevalence)":
               //        crCalculateValue.PointEstimate = (1 - (1 / ((1 - Incidence) * Math.Exp(Beta * DeltaQ) + Incidence))) * Incidence * POP * (1 - Prevalence);
               //        break;
               //    case "(1-(1/((1-Incidence*A)*EXP(Beta*DeltaQ)+Incidence*A)))*Incidence*A*POP":
               //        crCalculateValue.PointEstimate = (1 - (1 / ((1 - Incidence * A) * Math.Exp(Beta * DeltaQ) + Incidence * A))) * Incidence * A * POP;
               //        break;
               //    case "(1-(1/((1-Incidence)*EXP(Beta*(MAX(Q1,C)-MAX(Q0,C)))+Incidence)))*Incidence*POP":
               //        crCalculateValue.PointEstimate = (1 - (1 / ((1 - Incidence) * Math.Exp(Beta * (Math.Max(Q1, C) - Math.Max(Q0, C))) + Incidence))) * Incidence * POP;
               //        break;
               //    case "(1-(1/((1-A)*EXP(Beta*(MAX(Q1,C)-MAX(Q0,C)))+A)))*A*POP":
               //        crCalculateValue.PointEstimate = (1 - (1 / ((1 - A) * Math.Exp(Beta * (Math.Max(Q1, C) - Math.Max(Q0, C))) + A))) * A * POP;
               //        break;
               //    case "(1-(1/((1-A)*EXP(Beta*(MAX(Q1,C)-MAX(Q0,C)))+A)))*A*POP*B":
               //        crCalculateValue.PointEstimate = (1 - (1 / ((1 - A) * Math.Exp(Beta * (Math.Max(Q1, C) - Math.Max(Q0, C))) + A))) * A * POP * B;
               //        break;
               //    case "(1-(1/((1-Incidence)*EXP(Beta*(MAX(Q1,C)-MAX(Q0,C)))+Incidence)))*Incidence*POP*(1-Prevalence)":
               //        crCalculateValue.PointEstimate = (1 - (1 / ((1 - Incidence) * Math.Exp(Beta * (Math.Max(Q1, C) - Math.Max(Q0, C))) + Incidence))) * Incidence * POP * (1 - Prevalence);
               //        break;
               //    case "(1-(1/((1-Incidence*A)*EXP(Beta*(MAX(Q1,C)-MAX(Q0,C)))+Incidence*A)))*Incidence*A*POP":
               //        crCalculateValue.PointEstimate = (1 - (1 / ((1 - Incidence * A) * Math.Exp(Beta * (Math.Max(Q1, C) - Math.Max(Q0, C))) + Incidence * A))) * Incidence * A * POP;
               //        break;
               //    case "if ((Q1>A) and  (Q1<=B)) then result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP else result := 0":
               //        if ((Q1 > A) && (Q1 <= B))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP;
               //        else crCalculateValue.PointEstimate = 0;
               //        break;
               //    case "if (Q1>A) then result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP else result := 0":
               //        if (Q1 > A)
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP;
               //        else crCalculateValue.PointEstimate = 0;
               //        break;
               //    case "if (Q1<5) then Result := 0 else if ((Q1>=5)and(Q1<6)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.01 else if ((Q1>=6) and (Q1<7)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.05 else if ((Q1>=7) and (Q1<8)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.10 else if ((Q1>=8) and (Q1<9)) then Result :=(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.18 else if ((Q1>=9) and (Q1<10)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.28 else if ((Q1>=10) and (Q1<11)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.41 else if ((Q1>=11) and (Q1<12)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.56 else if ((Q1>=12) and (Q1<13)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.72 else if ((Q1>=13) and (Q1<14)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.88 else if ((Q1>=14) and (Q1<15)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.97 else if (Q1>=15) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP else Result := 0":
               //        if (Q1 < 5) crCalculateValue.PointEstimate = 0;
               //        else if ((Q1 >= 5) && (Q1 < 6))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.01;
               //        else if ((Q1 >= 6) && (Q1 < 7))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.05;
               //        else if ((Q1 >= 7) && (Q1 < 8))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.10;
               //        else if ((Q1 >= 8) && (Q1 < 9))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.18;
               //        else if ((Q1 >= 9) && (Q1 < 10))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.28;
               //        else if ((Q1 >= 10) && (Q1 < 11))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.41;
               //        else if ((Q1 >= 11) && (Q1 < 12))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.56;
               //        else if ((Q1 >= 12) && (Q1 < 13))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.72;
               //        else if ((Q1 >= 13) && (Q1 < 14))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.88;
               //        else if ((Q1 >= 14) && (Q1 < 15))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.97;
               //        else if (Q1 >= 15)
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP;
               //        else
               //            crCalculateValue.PointEstimate = 0;
               //        break;
               //    case "if (Q1 <> 0) then Result := Beta*((Q1-Q0)/Q1)*DAILYWAGEOUTDOOR*(MEDIAN_INCOME/NATL_MEDIAN_INCOME)*POP*(COUNT_FARM_EMPLOYED/POPULATION18TO64)":
               //        //-----------------------------不知道怎么做---
               //        break;
               //    case "((Beta)*(sqr(Q1)-sqr(Q0))*POP)/A":
               //        crCalculateValue.PointEstimate = ((Beta) * (Math.Sqrt(Q1) - Math.Sqrt(Q0)) * POP) / A;
               //        break;
               //    case "(Beta/A)*DELTAQ*POP":
               //        crCalculateValue.PointEstimate = (Beta / A) * DELTAQ * POP;
               //        break;
               //    case "Beta*C*DELTAQ*Incidence/B*POP*A":
               //        crCalculateValue.PointEstimate = Beta * C * DELTAQ * Incidence / B * POP * A;
               //        break;
               //    case "Beta":
               //        crCalculateValue.PointEstimate = Beta;
               //        break;
               //    case "if ((Q1>=A) and  (Q1<=B)) then result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP else result := 0":
               //        if ((Q1 >= A) && (Q1 <= B))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP;
               //        else
               //            crCalculateValue.PointEstimate = 0;
               //        break;
               //    case "if ((Q1<A) or (Q1>B)) then Result :=0 else if ((Q1>=5)and(Q1<6)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.0 else if ((Q1>=6) and (Q1<7)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.2 else if ((Q1>=7) and (Q1<8)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.40 else if ((Q1>=8) and (Q1<9)) then Result :=(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.6 else if ((Q1>=9) and (Q1<10)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.8 else if (Q1>=10) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP else Result := 0":
               //        if ((Q1 >= A) && (Q1 <= B))
               //            crCalculateValue.PointEstimate = 0;
               //        else if ((Q1 >= 5) && (Q1 < 6))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.0;
               //        else if ((Q1 >= 6) && (Q1 < 7))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.2;
               //        else if ((Q1 >= 7) && (Q1 < 8))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.40;
               //        else if ((Q1 >= 8) && (Q1 < 9))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.6;
               //        else if ((Q1 >= 9) && (Q1 < 10))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.8;
               //        else if (Q1 >= 10)
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP;
               //        else crCalculateValue.PointEstimate = 0;
               //        break;
               //    case "if ((Q1<A) or (Q1>B)) then Result :=0 else if ((Q1>=0)and(Q1<1)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0 else if ((Q1>=1) and (Q1<2)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.2 else if ((Q1>=2) and (Q1<3)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.40 else if ((Q1>=3) and (Q1<4)) then Result :=(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.6 else if ((Q1>=4) and (Q1<5)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.8 else if (Q1>=5) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP else Result := 0":
               //        if ((Q1 < A) || (Q1 > B))
               //            crCalculateValue.PointEstimate = 0;
               //        else if ((Q1 >= 0) && (Q1 < 1))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0;
               //        else if ((Q1 >= 1) && (Q1 < 2))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.2;
               //        else if ((Q1 >= 2) && (Q1 < 3))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.40;
               //        else if ((Q1 >= 3) && (Q1 < 4))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.6;
               //        else if ((Q1 >= 4) && (Q1 < 5))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * 0.8;
               //        else if (Q1 >= 5)
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP;
               //        else
               //            crCalculateValue.PointEstimate = 0;
               //        break;
               //    case "if ((Q1<A) or (Q1>B)) then Result :=0 else Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP":
               //        if ((Q1 < A) || (Q1 > B))
               //            crCalculateValue.PointEstimate = 0;
               //        else
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP;
               //        break;
               //    case "if ((Q1<=A) or (Q1>B)) then Result :=0 else Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP":
               //        if ((Q1 <= A) || (Q1 > B))
               //            crCalculateValue.PointEstimate = 0;
               //        else
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP;
               //        break;
               //    case "(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*B":
               //        crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * B;
               //        break;
               //    case "(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*C":
               //        crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * C;
               //        break;
               //    case "if (Q1>A) then result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*B else result := 0":
               //        if (Q1 > A)
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * B;
               //        else
               //            crCalculateValue.PointEstimate = 0;
               //        break;
               //    case "if ((Q1>A) and  (Q1<=B)) then result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*C else result := 0":
               //        if ((Q1 > A) && (Q1 <= B))
               //            crCalculateValue.PointEstimate = (1 - (1 / Math.Exp(Beta * DELTAQ))) * Incidence * POP * C;
               //        else
               //            crCalculateValue.PointEstimate = 0;
               //        break;
               //    case "if (Q1>A) then result := (1-(1/((1-Incidence)*EXP(Beta*DELTAQ)+Incidence)))*Incidence*POP*B else result := 0":
               //        if (Q1 > A)
               //            crCalculateValue.PointEstimate = (1 - (1 / ((1 - Incidence) * Math.Exp(Beta * DELTAQ) + Incidence))) * Incidence * POP * B;
               //        else
               //            crCalculateValue.PointEstimate = 0;
               //        break;
               //    case "(1-(1/((1-Incidence)*EXP(Beta*DeltaQ)+Incidence)))*Incidence*POP*A":
               //        crCalculateValue.PointEstimate = (1 - (1 / ((1 - Incidence) * Math.Exp(Beta * DeltaQ) + Incidence))) * Incidence * POP * A;
               //        break;
               //    case "if ((Q1>A) and  (Q1<=B)) then result := (1-(1/((1-Incidence)*EXP(Beta*DELTAQ)+Incidence)))*Incidence*POP*C else result := 0":
               //        if ((Q1 > A) && (Q1 <= B))
               //            crCalculateValue.PointEstimate = (1 - (1 / ((1 - Incidence) * Math.Exp(Beta * DELTAQ) + Incidence))) * Incidence * POP * C;
               //        else
               //            crCalculateValue.PointEstimate = 0;
               //        break;
               //    case "POP/365":
               //        crCalculateValue.PointEstimate = POP / 365;
               //        break;
               //    case "(1-EXP(-Beta*(Q1-Q0))*fCRFunction.A*fCurPopulation*fCRFunction.B"://-----------fCRFuntion解释有问题，暂时用这个
               //        crCalculateValue.PointEstimate = 1 - Math.Exp(-Beta * (Q1 - Q0)) * A * POP * B;
               //        break;
               //    case "(1-EXP(-Beta*(Q1-Q0))*fCurIncidence*fCurPopulation":
               //        crCalculateValue.PointEstimate = 1 - Math.Exp(-Beta * (Q1 - Q0)) * Incidence * POP;
               //        break;
               //    case "(fCRFunction.A-(fCRFunction.A/((1-fCRFunction.A)*EXP(Beta*(Q1-Q0))+fCRFunction.A)))*fCurPopulation":
               //        crCalculateValue.PointEstimate = (A - (A / ((1 - A) * Math.Exp(Beta * (Q1 - Q0)) + A))) * POP;
               //        break;
               //    case "(fCRFunction.A-(fCRFunction.A/((1-fCRFunction.A)*EXP(Beta*(Q1-Q0))+fCRFunction.A)))*fCurPopulation*fCRFunction.B":
               //        crCalculateValue.PointEstimate = (A - (A / ((1 - A) * Math.Exp(Beta * (Q1 - Q0)) + A))) * POP * B;
               //        break;
               //    case "(fCurIncidence-(fCurIncidence/((1-fCurIncidence)*EXP(Beta*(Q1-Q0))+fCurIncidence)))*fCurPopulation":
               //        crCalculateValue.PointEstimate = (Incidence - (Incidence / ((1 - Incidence) * Math.Exp(Beta * (Q1 - Q0)) + Incidence))) * POP;
               //        break;
               //    default:
               //        break;
               //};
               #endregion
               //生成Baseline的字符串
               // string strBaseLineFunction = crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction;
               //计算Baseline
               //string strBaseLineFunction = ConfigurationCommonClass.getFunctionStringFromDatabaseFunction(crSelectFunction.BenMAPHealthImpactFunction.BaseLineIncidenceFunction);
               
               //计算PointEstimate
               //string strPointEstimateFunction = ConfigurationCommonClass.getFunctionStringFromDatabaseFunction(crSelectFunction.BenMAPHealthImpactFunction.Function);
               if (dicPopulationValue==null || dicPopulationValue.Count==0 ||dicPopulationValue.Sum(p => p.Value) == 0)
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
                               crSelectFunction.BenMAPHealthImpactFunction.Beta, baseValue - controlValue, controlValue, baseValue, incidenceValue, k.Value, prevalenceValue, dicSetupVariables) * i365;
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
                               crSelectFunction.BenMAPHealthImpactFunction.Beta, baseValue - controlValue, controlValue, baseValue, incidenceValue, k.Value, prevalenceValue, dicSetupVariables) * i365;
                       }
                   }
               }
               if (strBaseLineFunction != " return  ;")
               {
                   if (hasPopInstrBaseLineFunction && crCalculateValue.Population ==0)
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
                                   crSelectFunction.BenMAPHealthImpactFunction.Beta, baseValue - controlValue, controlValue, baseValue, incidenceValue, k.Value, prevalenceValue, dicSetupVariables) * i365;
                           }
                       }
                       else
                       {
                           foreach (KeyValuePair<string, double> k in dicPopulationValue)
                           {
                               incidenceValue = dicIncidenceValue != null && dicIncidenceValue.Count > 0 && dicIncidenceValue.ContainsKey(k.Key) ? dicIncidenceValue[k.Key] : 0;
                               prevalenceValue = dicPrevalenceValue != null && dicPrevalenceValue.Count > 0 && dicPrevalenceValue.ContainsKey(k.Key) ? dicPrevalenceValue[k.Key] : 0;
                               crCalculateValue.Baseline = ConfigurationCommonClass.getValueFromBaseFunctionString(iCRID,strBaseLineFunction, crSelectFunction.BenMAPHealthImpactFunction.AContantValue,
                                   crSelectFunction.BenMAPHealthImpactFunction.BContantValue, crSelectFunction.BenMAPHealthImpactFunction.CContantValue,
                                   crSelectFunction.BenMAPHealthImpactFunction.Beta, baseValue - controlValue, controlValue, baseValue, incidenceValue, k.Value, prevalenceValue, dicSetupVariables) * i365;
                           }
                       }
                   }
               }
               else
               {
                   crCalculateValue.Baseline = crCalculateValue.PointEstimate;
               }
               //if (double.IsNaN(crCalculateValue.PointEstimate)) crCalculateValue.PointEstimate = Convert.ToSingle(0.0);
               //if (double.IsNaN(crCalculateValue.Baseline)) crCalculateValue.Baseline = Convert.ToSingle(0.0);
               crCalculateValue.LstPercentile = new List<float>();
               //根据拉丁方采样求出所有Percentile
               // ---------------忽略拉丁立体方，在显示结果中计算--------------
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

                           foreach (KeyValuePair<string, double> k in dicPopulationValue)
                           {
                               incidenceValue = dicIncidenceValue != null && dicIncidenceValue.Count > 0 && dicIncidenceValue.ContainsKey(k.Key) ? dicIncidenceValue[k.Key] : 0;
                               prevalenceValue = dicPrevalenceValue != null && dicPrevalenceValue.Count > 0 && dicPrevalenceValue.ContainsKey(k.Key) ? dicPrevalenceValue[k.Key] : 0;
                               crCalculateValue.LstPercentile[idlhs] += (ConfigurationCommonClass.getValueFromPointEstimateFunctionString(iCRID, strPointEstimateFunction,
                                   crSelectFunction.BenMAPHealthImpactFunction.AContantValue,
                               crSelectFunction.BenMAPHealthImpactFunction.BContantValue, crSelectFunction.BenMAPHealthImpactFunction.CContantValue,
                                 dlhs, baseValue - controlValue, controlValue, baseValue, incidenceValue, k.Value, prevalenceValue, dicSetupVariables) * i365);//A, B, C, Beta * dlhs, DeltaQ, Q0, Q1, Incidence, POP, Prevalence, dicSetupVariables));
                           }


                           // crSelectFunction.BenMAPHealthImpactFunction.Beta * dlhs, baseValue - controlValue, baseValue, controlValue, incidenceValue, populationValue, prevalenceValue, dicSetupVariables));//A, B, C, Beta * dlhs, DeltaQ, Q0, Q1, Incidence, POP, Prevalence, dicSetupVariables));
                       }
                   }
               }
               //Mean
               
               crCalculateValue.Mean = getMean(crCalculateValue.LstPercentile);
               //方差
               crCalculateValue.Variance = crCalculateValue.LstPercentile.Count() == 0 ? float.NaN : getVariance(crCalculateValue.LstPercentile, crCalculateValue.PointEstimate);
               //标准差
               crCalculateValue.StandardDeviation = crCalculateValue.LstPercentile.Count() == 0 ? float.NaN : Convert.ToSingle(Math.Sqrt(crCalculateValue.Variance));
               //crCalculateValue.StandardDeviation = getStandardDeviation(crCalculateValue.LstPercentile, crCalculateValue.PointEstimate);
               
                //--------------------------------------------------
               //PercentOfBaseline

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
                   //    case "if ((Q1<A) or (Q1>B)) then Result :=0 else if ((Q1>=0)and(Q1<1)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0 else if ((Q1>=1) and (Q1<2)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.2 else if ((Q1>=2) and (Q1<3)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.40 else if ((Q1>=3) and (Q1<4)) then Result :=(1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.6 else if ((Q1>=4) and (Q1<5)) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP*0.8 else if (Q1>=5) then Result := (1-(1/EXP(Beta*DELTAQ)))*Incidence*POP else Result := 0":

                   result = result.Replace(" and", " && ").Replace(")and", ")&&").Replace(" or", " || ").Replace(")or", ")||").Replace(":=", " return ")
                       .Replace("result", " ").Replace("else", ";else").Replace("then", " ").Replace("<>", "!=");
                   result = result + ";";// "; return -999999999;";

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
     //          ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["ConnectionString"];
     //          string strcon = settings.ConnectionString;
     //         // FbConnection _connection = new FirebirdSql.Data.FirebirdClient.FbConnection(strcon);
     //          if (lstFunctionVariables == null) lstFunctionVariables = new List<SetupVariableJoinAllValues>();
     //          DatabaseFunction = DatabaseFunction.Replace("prevalence", "").Replace("incidence", "").Replace("deltaq", "")
     //               .Replace("pop", "").Replace("beta", "").Replace("q0", "").Replace("q1", "").Replace("allgoodsindex", "").Replace("medicalcostindex", "").Replace("wageindex", "")
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
     //              if (DatabaseFunction.Contains(str))
     //              {
     //                  bool inLst = false;
     //                  foreach (SetupVariableJoinAllValues sv in lstFunctionVariables)
     //                  {
     //                      if (sv.SetupVariableName == str)
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
     //                  }
     //                  //   lstFunctionVariables.Add(str);
     //              }

     //          }
     //          //_connection.Close();
     //      }
     //      catch (Exception ex)
     //      {
     //          //  return null;
     //      }
     //      //return lstResult;
     //  }
       public static void getSetupVariableNameListFromDatabaseFunction(int VariableDatasetID,int GridDefinitionID,string DatabaseFunction, List<string> SystemVariableNameList, ref List<SetupVariableJoinAllValues> lstFunctionVariables)
       {
           try
           {
               // List<string> lstResult=new List<string>();
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
                           string commandText = string.Format("select a.SetupVariableID,a.GridDefinitionID from SetupVariables a,SetupVariableDatasets b where a.SetupVariableDatasetID=b.SetupVariableDatasetID and a.SetupVariableName='{0}' and a.SetupVariableDatasetID={1}", str,VariableDatasetID);
                           DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                           DataRow dr = ds.Tables[0].Rows[0];
                           setupVariableJoinAllValues.SetupVariableID = Convert.ToInt32(dr["SetupVariableID"]);
                           setupVariableJoinAllValues.SetupVariableGridType = Convert.ToInt32(dr["GridDefinitionID"]);
                           //add all value to lstvalues
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
                           //int GridDefinitionID = CommonClass.GBenMAPGrid.GridDefinitionID;
                           SetupVariableJoinAllValues setupVariableJoinAllValuesReturn = new SetupVariableJoinAllValues();
                           setupVariableJoinAllValuesReturn.lstValues = new List<SetupVariableValues>();
                           IEnumerable<SetupVariableValues> ies = null;
                           //-----------------------------------直接计算成现在的GridType------------------------------------
                           //----------------modify by xiejp 20120518 -- To county first--------------

                           GridRelationship gridRelationShipPopulation = new GridRelationship();

                           foreach (GridRelationship gRelationship in CommonClass.LstGridRelationshipAll)
                           {
                               if ((gRelationship.bigGridID == setupVariableJoinAllValues.SetupVariableGridType && gRelationship.smallGridID ==  GridDefinitionID) || (gRelationship.smallGridID == setupVariableJoinAllValues.SetupVariableGridType && gRelationship.bigGridID ==  GridDefinitionID))
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
                               //-----------------使用BenMAP4的空间关系---------------------------------------------------------------
                              GridRelationship gridRelationShip= new GridRelationship() { smallGridID = GridDefinitionID, bigGridID = setupVariableJoinAllValues.SetupVariableGridType };
                              Dictionary<string, Dictionary<string, double>> dicRelationShip = APVX.APVCommonClass.getRelationFromDicRelationShipAll(gridRelationShip);
                               //-----------------if 12km--first to 12km then to this!
                             
                              //-setupVariableJoinAllValues To dictionary--
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
                              if (1==2)//(CommonClass.GBenMAPGrid.GridDefinitionID == 7 || CommonClass.GBenMAPGrid.GridDefinitionID == 13) && (setupVariableJoinAllValues.SetupVariableGridType != 7 && setupVariableJoinAllValues.SetupVariableGridType != 13))
                              {
                                  //--to 12------
                                  Dictionary<string, Dictionary<string, double>> dicRelationShipTo12 = APVX.APVCommonClass.getRelationFromDicRelationShipAll(new GridRelationship() { smallGridID = 27, bigGridID = setupVariableJoinAllValues.SetupVariableGridType });
                                  foreach (KeyValuePair<string, Dictionary<string, double>> k in dicRelationShipTo12)
                                  {
                                      string[] s = k.Key.Split(new char[] { ',' });
                                      if (dicOld.ContainsKey(k.Key))
                                      {
                                          d = dicOld[k.Key];// setupVariableJoinAllValues.lstValues.Where(p => p.Col == Convert.ToInt32(s[0]) && p.Row == Convert.ToInt32(s[1])).Average(p => p.Value);
                                          if (k.Value != null && k.Value.Count > 0)
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
                                  //-----to big----
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

                                              d = Convert.ToSingle(dicOld[kO.Key] * k.Value);
                                              if (dicNew.ContainsKey(k.Key))
                                              {
                                                  dicNew[k.Key] += d;
                                              }
                                              else
                                              {
                                                  dicNew.Add(k.Key, d);
                                              }
                                              //setupVariableJoinAllValuesReturn.lstValues.Add(new SetupVariableValues()
                                              //{
                                              //    Col = Convert.ToInt32(s[0]),
                                              //    Row = Convert.ToInt32(s[1]),
                                              //    Value = d
                                              //});
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

                                      if (setupVariableJoinAllValues.SetupVariableGridType == gridRelationShipPopulation.bigGridID)//Population比较大
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
                                                          //if (setupVariableJoinAllValuesReturn.lstValues.Count()==0||setupVariableJoinAllValuesReturn.lstValues.Where(p => p.Col == rc.Col && p.Row == rc.Row) == null)
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
                                                          //if (setupVariableJoinAllValuesReturn.lstValues.Count()==0||setupVariableJoinAllValuesReturn.lstValues.Where(p => p.Col == rc.Col && p.Row == rc.Row) == null)
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
                                      else//网格类型比较大
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
                                                      //if (setupVariableJoinAllValuesReturn.lstValues.Count()==0||setupVariableJoinAllValuesReturn.lstValues.Where(p => p.Col == rc.Col && p.Row == rc.Row) == null)
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
                               setupVariableJoinAllValuesReturn.SetupVariableGridType =  GridDefinitionID;
                               setupVariableJoinAllValuesReturn.SetupVariableID = setupVariableJoinAllValues.SetupVariableID;
                               setupVariableJoinAllValuesReturn.SetupVariableName = setupVariableJoinAllValues.SetupVariableName;

                           }

                           //-----------------------------------------------------------------------------------------------
                           lstFunctionVariables.Add(setupVariableJoinAllValuesReturn);
                           ds.Dispose();
                       }
                       //   lstFunctionVariables.Add(str);
                   }

               }
           }
           catch (Exception ex)
           {
               //  return null;
           }
           //return lstResult;
       }
      
       /// <summary>
       /// 用文件的方式获取DataTable从csv中
       /// </summary>
       /// <param name="file"></param>
       /// <param name="isRowOneHeader"></param>
       /// <returns></returns>
       public static DataTable csvToDataTable(string file, bool isRowOneHeader)
       {

           DataTable csvDataTable = new DataTable();

           //no try/catch - add these in yourselfs or let exception happen
           String[] csvData = File.ReadAllLines(file);

           //if no data in file ‘manually’ throw an exception
           if (csvData.Length == 0)
           {
               throw new Exception("CSV File Appears to be Empty");
           }

           String[] headings = csvData[0].Split(',');
           int index = 0; //will be zero or one depending on isRowOneHeader

           if (isRowOneHeader) //if first record lists headers
           {
               index = 1; //so we won’t take headings as data

               //for each heading
               for (int i = 0; i < headings.Length; i++)
               {
                   //replace spaces with underscores for column names
                   headings[i] = headings[i].Replace(" ", "_");

                   //add a column for each heading
                   csvDataTable.Columns.Add(headings[i], typeof(string));
               }
           }
           else //if no headers just go for col1, col2 etc.
           {
               for (int i = 0; i < headings.Length; i++)
               {
                   //create arbitary column names
                   csvDataTable.Columns.Add("col" + (i + 1).ToString(), typeof(string));
               }
           }

           //populate the DataTable
           for (int i = index; i < csvData.Length; i++)
           {
               //create new rows
               DataRow row = csvDataTable.NewRow();

               for (int j = 0; j < headings.Length; j++)
               {
                   //fill them
                   row[j] = csvData[i].Split(',')[j];
               }

               //add rows to over DataTable
               csvDataTable.Rows.Add(row);
           }


           //return the CSV DataTable
           return csvDataTable;

       }

       private static Tools.CalculateFunctionString _baseeval;
       /// <summary>
       /// 用来计算BaseLine的Eval对象----用于在循环网格时不再生成编译对象
       /// </summary>
       internal static Tools.CalculateFunctionString BaseEval
       {
           get {
               if(_baseeval==null)
                   _baseeval = new Tools.CalculateFunctionString();
               return ConfigurationCommonClass._baseeval; }
           
       }
       private static Tools.CalculateFunctionString _pointEstimateEval;
       /// <summary>
       /// 用来计算PointEstimate的Eval对象----用于在循环网格时不再生成编译对象
       /// </summary>
       internal static Tools.CalculateFunctionString PointEstimateEval
       {
           get
           {
               if (_pointEstimateEval == null)
                   _pointEstimateEval = new Tools.CalculateFunctionString();
               return ConfigurationCommonClass._pointEstimateEval;
           }

       }
       //private static Tools.CalculateFunctionString _functionValue;

       //    internal static Tools.CalculateFunctionString FunctionValue
       //    {
       //        get
       //        {
       //            if(_functionValue==null)
       //                _functionValue=new Tools.CalculateFunctionString();
       //            return ConfigurationCommonClass._functionValue;
       //        }
       //    }
       /// <summary>
       /// 得到BaseLine的值从CRFunction的BaseLineFunction里面
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
       public static float getValueFromBaseFunctionString(string crid,string FunctionString, double A, double B, double C, double Beta, double DeltaQ, double Q0, double Q1, double Incidence, double POP, double Prevalence, Dictionary<string, double> dicSetupVariables)
       {
           try
           {
               object result = BaseEval.BaseLineEval(crid,FunctionString, A, B, C, Beta, DeltaQ, Q0, Q1, Incidence, POP, Prevalence, dicSetupVariables);
               if (result is double)
               {
                   if (double.IsNaN(Convert.ToDouble(result))) return 0;
                   return Convert.ToSingle(Convert.ToDouble(result));
               }
               else
               {
                   //return 0;
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
                   //return -999999999;
           }
           catch (Exception ex)
           {
               return 0;
           }
       }
       /// <summary>
       /// 得到PointEstimate值从CRFunction的FunctionString里面
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
       public static float getValueFromPointEstimateFunctionString(string crid,string FunctionString, double A, double B, double C, double Beta, double DeltaQ, double Q0, double Q1, double Incidence, double POP, double Prevalence, Dictionary<string, double> dicSetupVariables)
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
               {
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
                   //return -999999999;
           }
           catch (Exception ex)
           {
               return 0;
              // return -999999999;

           }

       }
       /// <summary>
       /// 得到FunctionValue值从ValueFunction的FunctionValueString里面
       /// </summary>
       /// <param name="FunctionValueString"></param>
       /// <param name="A"></param>
       /// <param name="B"></param>
       /// <param name="C"></param>
       /// <param name="D"></param>
       /// <param name="AllGoodIndex"></param>
       /// <param name="MedicalCostIndex"></param>
       /// <param name="WageIndex"></param>
       /// <param name="LagAdjustment"></param>
       /// <param name="dicSetupVariables"></param>
       /// <returns></returns>
       //public static double getFunctionValueString(string FunctionString, double A, double B, double C, double D, double AllGoodsIndex, double MedicalCostIndex, double WageIndex, double LagAdjustment, Dictionary<string, double> dicSetupVariables)
       //{

       //    object result =FunctionValue.FunctionValueString (FunctionString, A, B, C, D, AllGoodsIndex, MedicalCostIndex, WageIndex,LagAdjustment,  dicSetupVariables);
       //    if (result is double)
       //        return Convert.ToDouble(result);
       //    else return -999999999;
       //}
        /// <summary>
       /// 得到Mean(拉丁方采样)
       /// </summary>
       /// <param name="values"></param>
       /// <returns></returns>
       public static float getMean(List<float> values)
       {
           if (values == null || values.Count == 0) return 0;
           double sumd = 0;
           foreach (float di in values)
           {
               sumd = sumd +di;
           }
           sumd = sumd / values.Count;// 10.00000; 
           return Convert.ToSingle( sumd);
       }
       /// <summary>
       /// 得到标准差
       /// </summary>
       /// <param name="values"></param>
       /// <param name="PointEstimate"></param>
       /// <returns></returns>
       public static float getStandardDeviation(List<float> values, float PointEstimate)
       {
          // double da = 0.0466;
           //double sumd = 0;
           //foreach (double di in values)
           //{
           //    sumd = sumd + Math.Pow((di - PointEstimate), 2);
           //}
           //sumd = sumd / values.Count;//10.00000;
           //sumd = Math.Sqrt(sumd);
           ////sumd=Meta.Numerics.Statistics.Sample.
           //return Convert.ToSingle( sumd );
           //if (values == null || values.Count == 0) return 0;
           ////if (PointEstimate == 0) return 0;
           //List<float> lstValuesForStandardDeviation = new List<float>();
           //foreach(float f in values)
           //{
           //    lstValuesForStandardDeviation.Add(f);
           //}
           //lstValuesForStandardDeviation.Add(PointEstimate);
           //double avg = lstValuesForStandardDeviation.Average() ;
           //return Convert.ToSingle(Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)))); 
           return Convert.ToSingle(Math.Sqrt(getVariance(values, PointEstimate)));

       }
       /// <summary>
       /// 得到方差
       /// </summary>
       /// <param name="values"></param>
       /// <param name="PointEstimate"></param>
       /// <returns></returns>
       public static float getVariance(List<float> values, float PointEstimate)
       {
           // double da = 0.0466;
           //float sumd = 0;
           //foreach (float di in values)
           //{
           //    sumd =Convert.ToSingle( sumd + Math.Pow((di - PointEstimate), 2));
           //}
           //sumd = Convert.ToSingle(sumd / values.Count);//10.00000);

           //return Convert.ToSingle( sumd);
           if (values == null || values.Count == 0) return 0;
           //if (PointEstimate == 0) return 0;
           //double avg = values.Average();
           List<float> lstValuesForStandardDeviation = new List<float>();
           foreach (float f in values)
           {
               lstValuesForStandardDeviation.Add(f);
           }
           lstValuesForStandardDeviation.Add(PointEstimate);
           double avg = lstValuesForStandardDeviation.Average();
           double dResult = lstValuesForStandardDeviation.Sum(v => Math.Pow(v - avg, 2)) / Convert.ToDouble(lstValuesForStandardDeviation.Count() - 1);
           return Convert.ToSingle(dResult);
           //return Convert.ToSingle(values.Average(v => Math.Pow(v - avg, 2))); 
       }
       


       //public static double CalculateCRSelectFunctionsOneCel(CRSelectFunction crSelectFunction, RowCol rowcol, double baseValue, double controlValue, double populationValue, double incidenceValue, double prevalenceValue)
       //{
       //    throw new NotImplementedException();
       //}

       //public static void CalculateCRSelectFunctions(List<GridRelationship> lstGridRelationship, List<CRSelectFunction> lstCRSelectFuntion, List<BaseControlGroup> lstBaseControlGroup, List<RegionTypeGrid> lstRegionTypeGrid, BenMAPPopulation benMAPPopulation)
       //{
       //    throw new NotImplementedException();
       //}
    }
}
