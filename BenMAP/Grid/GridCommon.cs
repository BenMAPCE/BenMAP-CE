using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESIL.DBUtility;
using System.Data;
using DotSpatial.Controls;
using System.Windows.Forms;
using DotSpatial.Data;
using System.Security.Cryptography;
using System.IO;
using System.Drawing;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;
using FirebirdSql.Data.FirebirdClient;

namespace BenMAP.Grid
{
   public class GridCommon
    {
        // '加密函数 
        public static String Encrypt(String strText, String strEncrKey)
        {
            Byte[] byKey = { };
            Byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            try
            {
                byKey = System.Text.Encoding.UTF8.GetBytes(strEncrKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                Byte[] inputByteArray = Encoding.UTF8.GetBytes(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // '解密函数 
        public static String Decrypt(String strText, String sDecrKey)
        {
            Byte[] byKey = { };
            Byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            Byte[] inputByteArray = new byte[strText.Length];
            try
            {
                byKey = System.Text.Encoding.UTF8.GetBytes(sDecrKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static MemoryStream DecryptToMemoryStream(String strText, String sDecrKey)
        {
            Byte[] byKey = { };
            Byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            Byte[] inputByteArray = new byte[strText.Length];
            try
            {
                byKey = System.Text.Encoding.UTF8.GetBytes(sDecrKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return ms;// encoding.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                return null;// ex.Message;
            }
        } 
       public static Metric getMetricFromID(int MetricID)
       {
           try
           {
               Metric metric = new Metric();
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               string commandText = string.Format("select MetricID,MetricName,PollutantID,HourlyMetricGeneration from Metrics where MetricID={0}", MetricID);
               System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
               DataRow dr = ds.Tables[0].Rows[0];
               int HourlyMetricGeneration = Convert.ToInt32(dr["HOURLYMETRICGENERATION"]);
               switch (HourlyMetricGeneration)
               {
                   case 0:
                       //metric.MetricID = MetricID;
                       //metric.PollutantID = Convert.ToInt32(dr["PollutantID"]);
                       //metric.MetricName = dr["MetricName"].ToString();
                       //metric.HourlyMetricGeneration = HourlyMetricGeneration;
                       ////FixedWindowMetric fixedWindwoMetricNone = new FixedWindowMetric();
                       ////fixedWindwoMetricNone.MetricID = MetricID;
                       ////fixedWindwoMetricNone.PollutantID = Convert.ToInt32(dr["PollutantID"]);
                       ////fixedWindwoMetricNone.MetricName = dr["MetricName"].ToString();
                       ////fixedWindwoMetricNone.HourlyMetricGeneration = HourlyMetricGeneration;
                       ////fixedWindwoMetricNone.StartHour =0;// Convert.ToInt32(dr["StartHour"]);
                       ////fixedWindwoMetricNone.EndHour = 23;//Convert.ToInt32(dr["EndHour"]);
                       ////fixedWindwoMetricNone.Statistic = MetricStatic.Mean;// (MetricStatic)int.Parse(dr["Statistic"].ToString());
                       ////return fixedWindwoMetricNone;
                       //return metric;
                       FixedWindowMetric fixedWindwoMetric0 = new FixedWindowMetric();
                       fixedWindwoMetric0.MetricID = MetricID;
                       fixedWindwoMetric0.PollutantID = Convert.ToInt32(dr["PollutantID"]);
                       fixedWindwoMetric0.MetricName = dr["MetricName"].ToString();
                       fixedWindwoMetric0.HourlyMetricGeneration = HourlyMetricGeneration;

                       fixedWindwoMetric0.StartHour = 0;
                       fixedWindwoMetric0.EndHour = 23;
                       fixedWindwoMetric0.Statistic = MetricStatic.Mean;
                       return fixedWindwoMetric0;
                        
                       break;
                   case 1:
                       FixedWindowMetric fixedWindwoMetric = new FixedWindowMetric();
                       fixedWindwoMetric.MetricID = MetricID;
                       fixedWindwoMetric.PollutantID = Convert.ToInt32(dr["PollutantID"]);
                       fixedWindwoMetric.MetricName = dr["MetricName"].ToString();
                       fixedWindwoMetric.HourlyMetricGeneration = HourlyMetricGeneration;
                       commandText = string.Format("select StartHour,EndHour,Statistic from FixedWindowMetrics where MetricID={0}", MetricID);
                       ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                       dr = ds.Tables[0].Rows[0];
                       fixedWindwoMetric.StartHour = Convert.ToInt32(dr["StartHour"]);
                       fixedWindwoMetric.EndHour = Convert.ToInt32(dr["EndHour"]);
                       fixedWindwoMetric.Statistic = (MetricStatic)int.Parse(dr["Statistic"].ToString());
                       return fixedWindwoMetric;
                       break;
                   case 2:
                       MovingWindowMetric movingWindowMetric = new MovingWindowMetric();
                       movingWindowMetric.MetricID = MetricID;
                       movingWindowMetric.PollutantID = Convert.ToInt32(dr["PollutantID"]);
                       movingWindowMetric.MetricName = dr["MetricName"].ToString();
                       movingWindowMetric.HourlyMetricGeneration = HourlyMetricGeneration;
                       commandText = string.Format("select WindowSize,WindowStatistic,DailyStatistic from MovingWindowMetrics where MetricID={0}", MetricID);
                       ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                       dr = ds.Tables[0].Rows[0];
                       movingWindowMetric.WindowSize = Convert.ToInt32(dr["WindowSize"]);
                       movingWindowMetric.WindowStatistic = (MetricStatic)Convert.ToInt32(dr["WindowStatistic"]);
                       movingWindowMetric.DailyStatistic = (MetricStatic)Convert.ToInt32(dr["DailyStatistic"]);
                       return movingWindowMetric;

                       break;
                   case 3:
                       CustomerMetric customerMetric = new CustomerMetric();
                       customerMetric.MetricID = MetricID;
                       customerMetric.PollutantID = Convert.ToInt32(dr["PollutantID"]);
                       customerMetric.MetricName = dr["MetricName"].ToString();
                       customerMetric.HourlyMetricGeneration = HourlyMetricGeneration;
                       commandText = string.Format("select MetricFunction from CustomMetrics where MetricID={0}", MetricID);
                       ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                       dr = ds.Tables[0].Rows[0];
                       customerMetric.MetricFunction = dr["MetricFunction"].ToString();
                       return customerMetric;
                       break;
               }

               //metric.MetricID= 
               return metric;
           }
           catch (Exception err)
           {

               Logger.LogError(err);
               return null;
           }
       }
       public static List<Season> getSeasonFromPollutantID(int PollutantID)
       {
           try
           {
               List<Season> lstSeason = new List<Season>();
              
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               string commandText = string.Format("select PollutantSeasonID,PollutantID,StartDay,EndDay,StartHour,EndHour,Numbins from PollutantSeasons where PollutantID={0}", PollutantID);
               System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
               foreach (DataRow dr in ds.Tables[0].Rows)
               {
                   Season season = new Season();
                   season.PollutantID = PollutantID;
                   season.PollutantSeasonID = Convert.ToInt32(dr["PollutantSeasonID"]); ;
                   season.StartDay = Convert.ToInt32(dr["StartDay"]);
                   season.EndDay = Convert.ToInt32(dr["EndDay"]);
                   season.StartHour = Convert.ToInt32(dr["StartHour"]);
                   season.EndHour = Convert.ToInt32(dr["EndHour"]);
                   season.Numbins = Convert.ToInt32(dr["Numbins"]);
                   lstSeason.Add(season);
               }
               return lstSeason;
           }
           catch (Exception ex)
           {
               Logger.LogError(ex);
               return null;
           }
       }
       public static SeasonalMetric getSeasonalMetric(int SeasonalMetricID)
       {
           try
           {
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               string commandText = string.Format("select MetricID,SeasonalMetricName from SeasonalMetrics where SeasonalMetricID={0}", SeasonalMetricID);
               System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
               DataRow dr = ds.Tables[0].Rows[0];
               SeasonalMetric seasonalMetric = new SeasonalMetric();
               seasonalMetric.SeasonalMetricID = SeasonalMetricID;
               seasonalMetric.Metric = getMetricFromID(Convert.ToInt32(dr["MetricID"]));
               seasonalMetric.SeasonalMetricName = dr["SeasonalMetricName"].ToString();
               //因为不确定seasons每个污染物重新设置，暂时不写它的List<Season>---------------------------已修正数据库
               seasonalMetric.Seasons = new List<Season>();
               //commandText = string.Format("select a.PollutantSeasonID,a.PollutantID,a.StartDay,a.EndDay,a.StartHour,a.EndHour,a.Numbins from PollutantSeasons a, SeasonalMetricSeasons b where a.PollutantSeasonID= b.PollutantSeasonID and b.SeasonalMetricID={0}" +
               //" union select -1 as PollutantSeasonID,-1 as PollutantID,StartDay,EndDay,-1 as StartHour,-1 as EndHour,-1 as Numbins from SeasonalMetricSeasons where PollutantSeasonID is null and SeasonalMetricID={0}", SeasonalMetricID);
               commandText = string.Format("select a.PollutantID,c.SeasonalMetricID,d.StartDay,d.EndDay,d.SEASONALMETRICTYPE,d.METRICFUNCTION  from Pollutants a ,Metrics b,SeasonalMetrics c ,SeasonalMetricSeasons d "+
                             " where a.PollutantID=b.PollutantID and b.MetricID=c.MetricID and c.SeasonalMetricID=d.SeasonalMetricID and c.SeasonalMetricID={0}", SeasonalMetricID);


                ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow drSeason in ds.Tables[0].Rows)
                {
                    Season season = new Season()
                    {
                        //---------------------------如果为-1,则表示数据库里为空。是整年的数据------------
                        PollutantID = Convert.ToInt32(drSeason["PollutantID"]),
                        PollutantSeasonID =-1,// Convert.ToInt32(drSeason["PollutantSeasonID"]),
                        StartDay = Convert.ToInt32(drSeason["StartDay"]),
                        EndDay = Convert.ToInt32(drSeason["EndDay"]),
                        StartHour =-1,// Convert.ToInt32(drSeason["StartHour"]),
                        EndHour = -1,// Convert.ToInt32(drSeason["EndHour"]),
                        Numbins = -1,// Convert.ToInt32(drSeason["Numbins"])

                    };
                    seasonalMetric.Seasons.Add(season);
                }
               return seasonalMetric;
           }
           catch (Exception ex)
           {
               Logger.LogError(ex);
               return null;
           }
       }
       public static List<Metric> getMetricsFromPollutantID(int PollutantID)
       {
           try
           {
               List<Metric> lstMetric = new List<Metric>();
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               string commandText = string.Format("select MetricID from Metrics where PollutantID={0}", PollutantID);
               System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
               foreach (DataRow dr in ds.Tables[0].Rows)
               {
                   int MetricID = Convert.ToInt32(dr["MetricID"]);
                   Metric metric = getMetricFromID(MetricID);
                   if (metric != null) lstMetric.Add(metric);
               }

               return lstMetric;
           }
           catch (Exception ex)
           {
               return null;
           }
 
       }
       public static Metric getMetricFromPollutantAndID(BenMAPPollutant benMAPPollutant, int metricID)
       {
           foreach (Metric metric in benMAPPollutant.Metrics)
           {
               if (metric.MetricID == metricID)
               {
                   return metric;
               }
           }
           return null;
       }
       public static SeasonalMetric getSeasonalMetricFromPollutantAndID(BenMAPPollutant benMAPPollutant, int seasonalMetricID)
       {
           foreach (SeasonalMetric seasonalMetric in benMAPPollutant.SesonalMetrics)
           {
               if (seasonalMetric.SeasonalMetricID == seasonalMetricID)
                   return seasonalMetric;
           }
           return null;

       }
       public static List<BenMAPPollutant> getAllPollutant(int setupID)
       {
           try
           {

               List<BenMAPPollutant> lstBenMAPPollutant = new List<BenMAPPollutant>();
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               string commandText = string.Format("select PollutantName,ObservationType,PollutantID from Pollutants where SetupID={0}",setupID);
               System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
               foreach (DataRow dr in ds.Tables[0].Rows)
               {
                   BenMAPPollutant benMAPPollutant = new BenMAPPollutant();
                   benMAPPollutant.PollutantID = Convert.ToInt32(dr["PollutantID"]);
                   benMAPPollutant.PollutantName = dr["PollutantName"].ToString();
                   benMAPPollutant.Observationtype = (ObservationtypeEnum)Convert.ToInt32(dr["ObservationType"]);
                   benMAPPollutant.Metrics = getMetricsFromPollutantID(benMAPPollutant.PollutantID);
                   benMAPPollutant.Seasons = getSeasonFromPollutantID(benMAPPollutant.PollutantID);
                   List<SeasonalMetric> lstSeasonalMetric = new List<SeasonalMetric>();
                   commandText = string.Format("select SeasonalMetricID from SeasonalMetrics where MetricID in (select MetricID from Metrics where PollutantID={0})", benMAPPollutant.PollutantID);
                   ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                   foreach (DataRow drSeasonalMetric in ds.Tables[0].Rows)
                   {
                       lstSeasonalMetric.Add(getSeasonalMetric(Convert.ToInt32(drSeasonalMetric["SeasonalMetricID"])));
                   }
                   benMAPPollutant.SesonalMetrics = lstSeasonalMetric;
                   lstBenMAPPollutant.Add(benMAPPollutant);

               }
               return lstBenMAPPollutant;
           }
           catch (Exception ex)
           {
               Logger.LogError(ex);
               return null;
           }
       }
       public static BenMAPPollutant getPollutantFromID(int PollutantID, List<BenMAPPollutant> lstBenMAPPollutantAll)
       {
           try
           {
               foreach (BenMAPPollutant benMAPPollutant in lstBenMAPPollutantAll)
               {
                   if (benMAPPollutant.PollutantID == PollutantID)
                       return benMAPPollutant;
               }
               BenMAPPollutant benmappollutant = getPollutantFromID(PollutantID);
               lstBenMAPPollutantAll.Add(benmappollutant);

               return benmappollutant;
           }
           catch
           { 
           }
           return null;
       }
    /// <summary>
    /// 从污染物ID获取污染物
    /// </summary>
    /// <param name="PollutantID"></param>
    /// <returns></returns>
       public static BenMAPPollutant getPollutantFromID(int PollutantID)
       {
           try
           {

               BenMAPPollutant benMAPPollutant = new BenMAPPollutant();
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               string commandText = string.Format("select PollutantName,ObservationType from Pollutants where PollutantID ={0}", PollutantID);
               System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
               DataRow dr = ds.Tables[0].Rows[0];
               benMAPPollutant.PollutantID = PollutantID;
               benMAPPollutant.PollutantName = dr["PollutantName"].ToString();
               benMAPPollutant.Observationtype = (ObservationtypeEnum)Convert.ToInt32(dr["ObservationType"]);
               benMAPPollutant.Metrics = getMetricsFromPollutantID(PollutantID);
               benMAPPollutant.Seasons = getSeasonFromPollutantID(PollutantID);
               List<SeasonalMetric> lstSeasonalMetric = new List<SeasonalMetric>();
               commandText = string.Format("select SeasonalMetricID from SeasonalMetrics where MetricID in (select MetricID from Metrics where PollutantID={0})", PollutantID);
               ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
               foreach (DataRow drSeasonalMetric in ds.Tables[0].Rows)
               {
                   lstSeasonalMetric.Add(getSeasonalMetric(Convert.ToInt32(drSeasonalMetric["SeasonalMetricID"])));
               }
               benMAPPollutant.SesonalMetrics = lstSeasonalMetric;
               

               return benMAPPollutant;
           }
           catch (Exception ex)
           {
               Logger.LogError(ex);
               return null;
           }
       }
       /// <summary>
       /// 从污染物ID获取污染物
       /// </summary>
       /// <param name="PollutantID"></param>
       /// <returns></returns>
       public static BenMAPPollutant getPollutantFromName(string PollutantName,int SetupID)
       {
           try
           {
               //int PollutantID = 0;
               BenMAPPollutant benMAPPollutant = new BenMAPPollutant();
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               string commandText = string.Format("select PollutantID,PollutantName,ObservationType from Pollutants where PollutantName ='{0}' and SetupID={1}", PollutantName,SetupID);
               System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
               DataRow dr = ds.Tables[0].Rows[0];
               benMAPPollutant.PollutantID = Convert.ToInt32(dr["PollutantID"].ToString()) ;
               benMAPPollutant.PollutantName = dr["PollutantName"].ToString();
               benMAPPollutant.Observationtype = (ObservationtypeEnum)Convert.ToInt32(dr["ObservationType"]);
               benMAPPollutant.Metrics = getMetricsFromPollutantID(benMAPPollutant.PollutantID);
               benMAPPollutant.Seasons = getSeasonFromPollutantID(benMAPPollutant.PollutantID);
               List<SeasonalMetric> lstSeasonalMetric = new List<SeasonalMetric>();
               commandText = string.Format("select SeasonalMetricID from SeasonalMetrics where MetricID in (select MetricID from Metrics where PollutantID={0})", benMAPPollutant.PollutantID);
               ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
               foreach (DataRow drSeasonalMetric in ds.Tables[0].Rows)
               {
                   lstSeasonalMetric.Add(getSeasonalMetric(Convert.ToInt32(drSeasonalMetric["SeasonalMetricID"])));
               }
               benMAPPollutant.SesonalMetrics = lstSeasonalMetric;


               return benMAPPollutant;
           }
           catch (Exception ex)
           {
               Logger.LogError(ex);
               return null;
           }
       }
       public static void getPopulationFromIDYear(int PopulationDatSetID, int Year)
       {
 
       }
       /// <summary>
       /// 从GridID得到Grid
       /// </summary>
       /// <param name="GridID"></param>
       /// <returns></returns>
       public static BenMAPGrid getBenMAPGridFromID(int GridID)
       {
           
               ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["ConnectionString"];
               string str = settings.ConnectionString;
               if (!str.Contains(":"))
                   str = str.Substring(0, str.IndexOf("initial catalog=")) + "initial catalog=" + Application.StartupPath + @"\" + str.Substring(str.IndexOf("initial catalog=") + 16);
               FbConnection _connection = new FirebirdSql.Data.FirebirdClient.FbConnection(str);
               try
               {
               BenMAPGrid benMAPGrid = new BenMAPGrid();
               benMAPGrid.SetupName = CommonClass.MainSetup.SetupName;
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               string commandText = string.Format("select GridDefinitionID,SetupID,GridDefinitionName,Columns,RRows,TType from GridDefinitions where GridDefinitionID ={0}", GridID);
               System.Data.DataSet ds = fb.ExecuteDataset(_connection, CommandType.Text, commandText);
               if (ds.Tables[0].Rows.Count == 0) return null;
               DataRow dr = ds.Tables[0].Rows[0];
               benMAPGrid.GridDefinitionID = GridID;
               benMAPGrid.GridDefinitionName = dr["GridDefinitionName"].ToString();
               benMAPGrid.Columns = Convert.ToInt32(dr["Columns"]);
               benMAPGrid.RRows = Convert.ToInt32(dr["RRows"]);
               benMAPGrid.SetupID = Convert.ToInt32(dr["SetupID"]);
               benMAPGrid.TType = (GridTypeEnum)Convert.ToInt32(dr["TType"]);
               switch (benMAPGrid.TType)
               {
                   case GridTypeEnum.Shapefile:
                       ShapefileGrid shapefileGrid = new ShapefileGrid()
                       {
                           GridDefinitionID = GridID,
                           GridDefinitionName = benMAPGrid.GridDefinitionName,
                           Columns = benMAPGrid.Columns,
                           RRows = benMAPGrid.RRows,
                           SetupID = benMAPGrid.SetupID,
                           TType = benMAPGrid.TType,
                           SetupName = CommonClass.MainSetup.SetupName
                       };
                       commandText = string.Format("select ShapeFileName  from ShapeFileGridDefinitionDetails where GridDefinitionID={0}", GridID);
                       shapefileGrid.ShapefileName = fb.ExecuteScalar(_connection, CommandType.Text, commandText).ToString();
                       return shapefileGrid;
                       break;
                   case GridTypeEnum.Regular://是否让Regular也在ManageSetup里面生成SHP，这样便于之后的空间分析？,加入ShapeFileName的字段到RegularGridDefinitionDetails表，并修正ManageSetup
                       RegularGrid regularGrid=new RegularGrid()
                       {
                           GridDefinitionID = GridID,
                           GridDefinitionName = benMAPGrid.GridDefinitionName,
                           Columns = benMAPGrid.Columns,
                           RRows = benMAPGrid.RRows,
                           SetupID = benMAPGrid.SetupID,
                           TType = benMAPGrid.TType,
                           SetupName = CommonClass.MainSetup.SetupName
                       };
                       commandText =string.Format("select ShapeFileName,GridDefinitionID,MinimumLatitude,MinimumLongitude,ColumnsperLongitude,RowsperLatitude from RegularGridDefinitionDetails where GridDefinitionID={0}",GridID);
                       ds= fb.ExecuteDataset(_connection, CommandType.Text, commandText);
                       dr = ds.Tables[0].Rows[0];
                       regularGrid.MinimumLatitude= Convert.ToDouble(dr["MinimumLatitude"]);
                       regularGrid.MinimumLongitude= Convert.ToDouble(dr["MinimumLongitude"]);
                       regularGrid.ColumnsperLongitude= Convert.ToInt32(dr["ColumnsperLongitude"]);
                       regularGrid.RowsperLatitude=Convert.ToInt32(dr["RowsperLatitude"]);
                       regularGrid.ShapefileName=dr["ShapeFileName"].ToString();
                       return regularGrid;

                       break;
               }
               _connection.Close();
               return benMAPGrid;
              
           }
           catch (Exception ex)
           {
               _connection.Close();
               Logger.LogError(ex);
               return null;
           }
       }

       public static BenMAPGrid getBenMAPGridFromName(string GridName, BenMAPSetup benMAPSetup)
       {
           try
           {
               BenMAPGrid benMAPGrid = new BenMAPGrid();
               benMAPGrid.SetupName = benMAPSetup.SetupName;
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               string commandText = string.Format("select GridDefinitionID,SetupID,GridDefinitionName,Columns,RRows,TType from GridDefinitions where GridDefinitionName ='{0}' and SetupID={1}", GridName, benMAPSetup.SetupID);
               System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
               if (ds.Tables[0].Rows.Count == 0) return null;
               DataRow dr = ds.Tables[0].Rows[0];
               benMAPGrid.GridDefinitionID = Convert.ToInt32(dr["GridDefinitionID"]);
               benMAPGrid.GridDefinitionName = GridName;
               benMAPGrid.Columns = Convert.ToInt32(dr["Columns"]);
               benMAPGrid.RRows = Convert.ToInt32(dr["RRows"]);
               benMAPGrid.SetupID = Convert.ToInt32(dr["SetupID"]);
               benMAPGrid.TType = (GridTypeEnum)Convert.ToInt32(dr["TType"]);
               switch (benMAPGrid.TType)
               {
                   case GridTypeEnum.Shapefile:
                       ShapefileGrid shapefileGrid = new ShapefileGrid()
                       {
                           GridDefinitionID = benMAPGrid.GridDefinitionID,
                           GridDefinitionName = benMAPGrid.GridDefinitionName,
                           Columns = benMAPGrid.Columns,
                           RRows = benMAPGrid.RRows,
                           SetupID = benMAPGrid.SetupID,
                           TType = benMAPGrid.TType,
                           SetupName = benMAPSetup.SetupName
                       };
                       commandText = string.Format("select ShapeFileName from ShapeFileGridDefinitionDetails where GridDefinitionID={0}", benMAPGrid.GridDefinitionID);
                       shapefileGrid.ShapefileName = fb.ExecuteScalar(CommonClass.Connection, CommandType.Text, commandText).ToString();
                       return shapefileGrid;
                   case GridTypeEnum.Regular:
                       RegularGrid regularGrid = new RegularGrid()
                       {
                           GridDefinitionID = benMAPGrid.GridDefinitionID,
                           GridDefinitionName = benMAPGrid.GridDefinitionName,
                           Columns = benMAPGrid.Columns,
                           RRows = benMAPGrid.RRows,
                           SetupID = benMAPGrid.SetupID,
                           TType = benMAPGrid.TType,
                           SetupName = benMAPSetup.SetupName
                       };
                       commandText = string.Format("select ShapeFileName,GridDefinitionID,MinimumLatitude,MinimumLongitude,ColumnsperLongitude,RowsperLatitude from RegularGridDefinitionDetails where GridDefinitionID={0}", benMAPGrid.GridDefinitionID);
                       ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                       dr = ds.Tables[0].Rows[0];
                       regularGrid.MinimumLatitude = Convert.ToDouble(dr["MinimumLatitude"]);
                       regularGrid.MinimumLongitude = Convert.ToDouble(dr["MinimumLongitude"]);
                       regularGrid.ColumnsperLongitude = Convert.ToInt32(dr["ColumnsperLongitude"]);
                       regularGrid.RowsperLatitude = Convert.ToInt32(dr["RowsperLatitude"]);
                       regularGrid.ShapefileName = dr["ShapeFileName"].ToString();
                       return regularGrid;
               }
               return benMAPGrid;

           }
           catch (Exception ex)
           {
               Logger.LogError(ex);
               return null;
           }
       }

       public static List<GridRelationship> getAllGridRelationship()
       {
           List<GridRelationship> lstGridRelationship = new List<GridRelationship>();
           string commandText = string.Format("select GridDefinitionID from GridDefinitions");
           ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
           System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
           foreach (DataRow dr in ds.Tables[0].Rows)
           {
               BenMAPGrid benMAPGrid = getBenMAPGridFromID(Convert.ToInt32(dr["GridDefinitionID"]));
               getGridRelationshipFromGrid(benMAPGrid,ref lstGridRelationship);
           }
           return lstGridRelationship;
 
       }
       public static Region getRegionFromBaseGeometry(DotSpatial.Topology.IBasicGeometry iBaseGeometry)
       {
           Region region;

           System.Drawing.Drawing2D.GraphicsPath gPath = new System.Drawing.Drawing2D.GraphicsPath();
           int i=0 ;
           //foreach (DotSpatial.Topology.Coordinate coor in iBaseGeometry.Coordinates)
           PointF[] pointF = new PointF[iBaseGeometry.Coordinates.Count];
           while(i<iBaseGeometry.Coordinates.Count-1)
           {
               //gPath.AddLine(Convert.ToInt32(iBaseGeometry.Coordinates[i].X * 10.00000), Convert.ToInt32(iBaseGeometry.Coordinates[i].Y * 10.00000)
               //    , Convert.ToInt32(iBaseGeometry.Coordinates[i + 1].X * 10.00000), Convert.ToInt32(iBaseGeometry.Coordinates[i + 1].Y * 10.00000));
               pointF[i] = new PointF(Convert.ToSingle(iBaseGeometry.Coordinates[i].X), Convert.ToSingle(iBaseGeometry.Coordinates[i].Y));
               i++;
           }
           //gPath.AddLine(Convert.ToInt32(iBaseGeometry.Coordinates[i].X * 10.00000), Convert.ToInt32(iBaseGeometry.Coordinates[i].Y * 10.00000)
           //     , Convert.ToInt32(iBaseGeometry.Coordinates[0].X * 10.00000), Convert.ToInt32(iBaseGeometry.Coordinates[0].Y * 10.00000));
           gPath.AddPolygon(pointF);
           
           region = new Region(gPath);
           return region;
       }
       public static void getFipsCodes()
       {
            
            string commandText = "select a.GridDefinitionID,SetupID,GridDefinitionName,Columns,RRows,TType,b.ShapeFileName from GridDefinitions a,ShapeFileGridDefinitionDetails b " +
                                    " where a.GridDefinitionID=b.GridDefinitionID and a.TType=1 and a.GridDefinitionID=1 " +
                                    " union " +
                                   "select a.GridDefinitionID,SetupID,GridDefinitionName,Columns,RRows,TType,b.ShapeFileName from GridDefinitions a,ShapeFileGridDefinitionDetails b " +
                                    " where a.GridDefinitionID=b.GridDefinitionID and a.TType=1 and a.GridDefinitionID=2 ";
           // commandText = "drop table USAFIPSCODES";
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
             //  fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
             //  return;
               System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
              // List<GridRelationship> lstGridRelationship = new List<GridRelationship>();
               
               string AppPath = Application.StartupPath;
               //----------------------------------------xjp-------------仅用于测试！
               AppPath = @"D:\软件项目\Map\正确的BenMAP的SHP\";
               string benMAPGridShapeFile = "";
              
              
               foreach (DataRow dr in ds.Tables[0].Rows)
               {
                   benMAPGridShapeFile = AppPath + dr["ShapeFileName"].ToString() + ".shp";
                   DotSpatial.Data.IFeatureSet benMAPGridFeatureSet = DotSpatial.Data.FeatureSet.Open(benMAPGridShapeFile);
                   string GridDefinitionName = dr["GridDefinitionName"].ToString();

                   foreach (IFeature feature in benMAPGridFeatureSet.Features)
                   {
                       string LocationName="";
                       if (GridDefinitionName.ToLower() == "state")
                       {
                           LocationName = feature.DataRow["State_Name"].ToString();
                       }
                       else
                       {
                           LocationName = feature.DataRow["Name"].ToString();
 
                       }
                       LocationName = LocationName.Replace("'","''");

                       int Col = Convert.ToInt32(feature.DataRow["Col"]);
                       int Row = Convert.ToInt32(feature.DataRow["Row"]);
                       string Fips = feature.DataRow["Fips"].ToString();
                       commandText = string.Format("insert into USAFIPSCODES values('{0}','{1}',{2},{3},'{4}')", GridDefinitionName,LocationName, Col, Row, Fips);
                       fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);

 
                   }

               }
       }
       public static void getGridRelationshipFromGridChina(BenMAPGrid benMAPGrid, ref List<GridRelationship> lstGridRelationship)
       {
           try
           {
           }
           catch
           { }
       }
       /// <summary>
       /// 根据所选的网格，得到所有网格和他的关系
       /// </summary>
       /// <param name="benMAPGrid"></param>
       /// <returns></returns>
       public static void getGridRelationshipFromGrid(BenMAPGrid benMAPGrid, ref List<GridRelationship> lstGridRelationship)
       {
           try
           {

               string commandText = "select a.GridDefinitionID,SetupID,GridDefinitionName,Columns,RRows,TType,b.ShapeFileName from GridDefinitions a,ShapeFileGridDefinitionDetails b " +
                                    " where a.GridDefinitionID=b.GridDefinitionID and a.TType=1  and a.SetupID="+benMAPGrid.SetupID +
                                    " union " +
                                    " select a.GridDefinitionID,SetupID,GridDefinitionName,Columns,RRows,TType,b.ShapeFileName from GridDefinitions a,RegularGridDefinitionDetails b " +
                                    " where a.GridDefinitionID=b.GridDefinitionID and a.TType=0  and a.SetupID=" + benMAPGrid.SetupID;
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
              // List<GridRelationship> lstGridRelationship = new List<GridRelationship>();
               if(lstGridRelationship==null)
                   lstGridRelationship = new List<GridRelationship>();
               string AppPath = Application.StartupPath;
               //----------------------------------------xjp-------------仅用于测试！
               AppPath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\";// @"C:\Program Files\BenMAP 4.0\Shapefiles\";
                 //if (File.Exists(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.RBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
                 //   {
               string benMAPGridShapeFile = "";
               if (benMAPGrid is ShapefileGrid) benMAPGridShapeFile = (benMAPGrid as ShapefileGrid).ShapefileName;
               if (benMAPGrid is RegularGrid) benMAPGridShapeFile = (benMAPGrid as RegularGrid).ShapefileName;
               benMAPGridShapeFile = AppPath + benMAPGridShapeFile + ".shp";
               DotSpatial.Data.IFeatureSet benMAPGridFeatureSet =  DotSpatial.Data.FeatureSet.Open(benMAPGridShapeFile);
               foreach (DataRow dr in ds.Tables[0].Rows)
               {
                   GridRelationship gridRelationship = new GridRelationship();
                   //增加网格和当前网格的对应关系
                   int gridDefinitionID = Convert.ToInt32(dr["GridDefinitionID"]);
                   if (gridDefinitionID == benMAPGrid.GridDefinitionID)
                   {
                       continue;
                   }
                   var query = from a in lstGridRelationship where a.smallGridID == gridDefinitionID && a.bigGridID == benMAPGrid.GridDefinitionID select a;
                   if (query.Count() > 0)
                       continue;
                   query = from a in lstGridRelationship where a.bigGridID == gridDefinitionID && a.smallGridID == benMAPGrid.GridDefinitionID select a;
                   if (query.Count() > 0)
                       continue;
                    


                   gridRelationship.lstGridRelationshipAttribute = new List<GridRelationshipAttribute>();

                   

                   DotSpatial.Data.IFeatureSet relationFeatureSet =  DotSpatial.Data.FeatureSet.Open(AppPath + dr["ShapeFileName"].ToString() + ".shp");
                   DotSpatial.Data.IFeatureSet regionFeatureSet = null;
                   DotSpatial.Data.IFeatureSet gridFeatureSet = null;
                   if (benMAPGridFeatureSet.ShapeIndices.Count > relationFeatureSet.ShapeIndices.Count)
                   {
                       regionFeatureSet = relationFeatureSet;
                       gridFeatureSet = benMAPGridFeatureSet;

                       gridRelationship.bigGridID = gridDefinitionID;
                       gridRelationship.smallGridID = benMAPGrid.GridDefinitionID;
                   }
                   else
                   {
                       regionFeatureSet =benMAPGridFeatureSet ;
                       gridFeatureSet = relationFeatureSet;
                       gridRelationship.bigGridID = benMAPGrid.GridDefinitionID;
                       gridRelationship.smallGridID =gridDefinitionID ;
                   }
                   //Nation 全部
                   if (regionFeatureSet.ShapeIndices.Count == 1)
                   {
                       RowCol nationBigGridRowCol = new RowCol
                       {
                           Col = Convert.ToInt32(regionFeatureSet.GetFeature(0).DataRow["Col"]),
                           Row = Convert.ToInt32(regionFeatureSet.GetFeature(0).DataRow["Row"])
                       };
                       GridRelationshipAttribute gridRelationshipAttribute = new GridRelationshipAttribute();
                       gridRelationshipAttribute.bigGridRowCol = nationBigGridRowCol;
                       gridRelationshipAttribute.smallGridRowCol = new List<RowCol>();
                       int iNationIn = 0;
                       while (  iNationIn< gridFeatureSet.ShapeIndices.Count)
                       {
                           try
                           {
                               IFeature iFeature = gridFeatureSet.GetFeature(iNationIn);
                               RowCol nationSmallGridRowCol = new RowCol
                               {
                                   Col = Convert.ToInt32(iFeature.DataRow["Col"]),
                                   Row = Convert.ToInt32(iFeature.DataRow["Row"])
                               };
                               gridRelationshipAttribute.smallGridRowCol.Add(nationSmallGridRowCol);
                               iNationIn++;

                           }
                           catch
                           {
                               iNationIn++;

                           }

                          
                       }
                       gridRelationship.lstGridRelationshipAttribute.Add(gridRelationshipAttribute);
                       lstGridRelationship.Add(gridRelationship);
                       continue;
                   }
                   //如果属于County State 从数据库读取 --- Report_regions直接写出关系
                   if (gridRelationship.bigGridID == 2 && gridRelationship.smallGridID == 1)
                   {
                       commandText = "select Col,Row,Fipscode from USAFipsCodes where LocationTypeName='State'";
                       System.Data.DataSet dsState=fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                       foreach (DataRow drState in dsState.Tables[0].Rows)
                       {
                           GridRelationshipAttribute gridRelationshipAttribute = new GridRelationshipAttribute();
                           gridRelationshipAttribute.bigGridRowCol = new RowCol() 
                           {
                                Col=Convert.ToInt32(drState["Col"]),
                                Row=Convert.ToInt32(drState["Row"])
                           };
                           gridRelationshipAttribute.smallGridRowCol = new List<RowCol>();
                           commandText = string.Format("select Col,Row,Fipscode from USAFipsCodes where LocationTypeName='County' and SUBSTRING(fipscode from 1 for 2) ={0}",drState["Fipscode"].ToString());
                           System.Data.DataSet dsCounty = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                           foreach (DataRow drCounty in dsCounty.Tables[0].Rows)
                           {
                               gridRelationshipAttribute.smallGridRowCol.Add(new RowCol()
                               {
                                   Col = Convert.ToInt32(drCounty["Col"]),
                                   Row = Convert.ToInt32(drCounty["Row"])

                               });
 
                           }
                           gridRelationship.lstGridRelationshipAttribute.Add(gridRelationshipAttribute);
 
                       }
                       lstGridRelationship.Add(gridRelationship);
                       continue;

                       
                   }
                   //--- Report_regions & state-----
                   if (gridRelationship.bigGridID == 8 && gridRelationship.smallGridID == 2)
                   {
                       GridRelationshipAttribute gridRelationshipAttribute = new GridRelationshipAttribute();
                       gridRelationshipAttribute.bigGridRowCol = new RowCol()
                       {
                           Col =1,
                           Row = 1
                       };
                       gridRelationshipAttribute.smallGridRowCol = new List<RowCol>();
                       gridRelationshipAttribute.smallGridRowCol.Add(new RowCol() { Col = 6, Row = 1 });
                       gridRelationship.lstGridRelationshipAttribute.Add(gridRelationshipAttribute);

                       // east---------10
                       gridRelationshipAttribute = new GridRelationshipAttribute();
                       gridRelationshipAttribute.bigGridRowCol = new RowCol()
                       {
                           Col = 1,
                           Row = 3
                       };
                       gridRelationshipAttribute.smallGridRowCol = new List<RowCol>();
                       gridRelationshipAttribute.smallGridRowCol.Add(new RowCol() { Col = 53, Row = 1 });
                       gridRelationshipAttribute.smallGridRowCol.Add(new RowCol() { Col = 41, Row = 1 });
                       gridRelationshipAttribute.smallGridRowCol.Add(new RowCol() { Col = 16, Row = 1 });
                       gridRelationshipAttribute.smallGridRowCol.Add(new RowCol() { Col = 30, Row = 1 });
                       gridRelationshipAttribute.smallGridRowCol.Add(new RowCol() { Col = 56, Row = 1 });
                       gridRelationshipAttribute.smallGridRowCol.Add(new RowCol() { Col = 32, Row = 1 });
                       gridRelationshipAttribute.smallGridRowCol.Add(new RowCol() { Col = 49, Row = 1 });
                       gridRelationshipAttribute.smallGridRowCol.Add(new RowCol() { Col = 8, Row = 1 });
                       gridRelationshipAttribute.smallGridRowCol.Add(new RowCol() { Col = 4, Row = 1 });
                       gridRelationshipAttribute.smallGridRowCol.Add(new RowCol() { Col = 35, Row = 1 });
                       gridRelationship.lstGridRelationshipAttribute.Add(gridRelationshipAttribute);
                       //west-------------38
                       gridRelationshipAttribute = new GridRelationshipAttribute();
                       gridRelationshipAttribute.bigGridRowCol = new RowCol()
                       {
                           Col = 1,
                           Row =2
                       };
                       gridRelationshipAttribute.smallGridRowCol = new List<RowCol>();
                       commandText = string.Format("select Col,Row,Fipscode from USAFipsCodes where locationtypename='State' and col not in (6,53,41,16,30,56,32,49,8,4,35 )");
                       System.Data.DataSet dsFipscode = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                       foreach (DataRow drFipscode in dsFipscode.Tables[0].Rows)
                       {
                           gridRelationshipAttribute.smallGridRowCol.Add(new RowCol()
                           {
                               Col = Convert.ToInt32(drFipscode["Col"]),
                               Row = Convert.ToInt32(drFipscode["Row"])

                           });

                       }
                       gridRelationship.lstGridRelationshipAttribute.Add(gridRelationshipAttribute);
                       lstGridRelationship.Add(gridRelationship);
                       continue;
                   }
                   //--- Report_regions & county-----
                   if (gridRelationship.bigGridID == 8 && gridRelationship.smallGridID == 1)
                   {
                       GridRelationshipAttribute gridRelationshipAttribute = new GridRelationshipAttribute();
                       gridRelationshipAttribute.bigGridRowCol = new RowCol()
                       {
                           Col = 1,
                           Row = 1
                       };
                       gridRelationshipAttribute.smallGridRowCol = new List<RowCol>();
                       commandText = string.Format("select Col,Row,Fipscode from USAFipsCodes where locationtypename='County' and  substring(FipsCode from 1 for 2)='06'");
                       System.Data.DataSet dsFipscode = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                       foreach (DataRow drFipscode in dsFipscode.Tables[0].Rows)
                       {
                           gridRelationshipAttribute.smallGridRowCol.Add(new RowCol()
                           {
                               Col = Convert.ToInt32(drFipscode["Col"]),
                               Row = Convert.ToInt32(drFipscode["Row"])

                           });

                       }
                       gridRelationship.lstGridRelationshipAttribute.Add(gridRelationshipAttribute);
                       //----------------------------east-----------------------10
                         gridRelationshipAttribute = new GridRelationshipAttribute();
                       gridRelationshipAttribute.bigGridRowCol = new RowCol()
                       {
                           Col = 1,
                           Row = 3
                       };
                       gridRelationshipAttribute.smallGridRowCol = new List<RowCol>();
                       commandText = string.Format("select Col,Row,Fipscode from USAFipsCodes where locationtypename='County' and  substring(FipsCode from 1 for 2) in ( '53','41','16','30','56','32','49','08','04','35' )");
                        dsFipscode = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                       foreach (DataRow drFipscode in dsFipscode.Tables[0].Rows)
                       {
                           gridRelationshipAttribute.smallGridRowCol.Add(new RowCol()
                           {
                               Col = Convert.ToInt32(drFipscode["Col"]),
                               Row = Convert.ToInt32(drFipscode["Row"])

                           });

                       }
                       gridRelationship.lstGridRelationshipAttribute.Add(gridRelationshipAttribute);
                      // --------------------------------west------------------38-------
                       gridRelationshipAttribute = new GridRelationshipAttribute();
                       gridRelationshipAttribute.bigGridRowCol = new RowCol()
                       {
                           Col = 1,
                           Row = 3
                       };
                       gridRelationshipAttribute.smallGridRowCol = new List<RowCol>();
                       commandText = string.Format("select Col,Row,Fipscode from USAFipsCodes where locationtypename='County' and  substring(FipsCode from 1 for 2) not in ('06', '53','41','16','30','56','32','49','08','04','35' )");
                       dsFipscode = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                       foreach (DataRow drFipscode in dsFipscode.Tables[0].Rows)
                       {
                           gridRelationshipAttribute.smallGridRowCol.Add(new RowCol()
                           {
                               Col = Convert.ToInt32(drFipscode["Col"]),
                               Row = Convert.ToInt32(drFipscode["Row"])

                           });

                       }
                       gridRelationship.lstGridRelationshipAttribute.Add(gridRelationshipAttribute);
                       lstGridRelationship.Add(gridRelationship);
                       continue;
                   }
                   if (1 == 2)
                   {
                       #region oldcode
                       int i = 0;
                       while (i < regionFeatureSet.ShapeIndices.Count)
                       {
                           try
                           {
                               DotSpatial.Data.IFeature pl = regionFeatureSet.GetFeature(i);


                               Rectangle rect = new Rectangle(Convert.ToInt32(pl.Envelope.X), Convert.ToInt32(pl.Envelope.Y - pl.Envelope.Height), Convert.ToInt32(pl.Envelope.Width), Convert.ToInt32(pl.Envelope.Height));
                               // List<IFeature> lstFeatures = new List<IFeature>();// gridFeatureSet.Select(regionFeatureSet.GetFeature(i).Envelope.ToExtent());
                               List<int> lstFids = gridFeatureSet.SelectIndices(regionFeatureSet.GetFeature(i).Envelope.ToExtent());


                               RowCol bigGridRowCol = new RowCol()
                               {
                                   Col = Convert.ToInt32(pl.DataRow["Col"]),
                                   Row = Convert.ToInt32(pl.DataRow["Row"])
                               };
                               GridRelationshipAttribute gridRelationshipAttribute = new GridRelationshipAttribute();
                               gridRelationshipAttribute.bigGridRowCol = bigGridRowCol;
                               gridRelationshipAttribute.smallGridRowCol = new List<RowCol>();
                               Region rpl = getRegionFromBaseGeometry(pl.BasicGeometry);
                               foreach (int ilstFids in lstFids)
                               {
                                   try
                                   {
                                       IFeature iFeature = gridFeatureSet.GetFeature(ilstFids);
                                       Rectangle rect1 = new Rectangle(Convert.ToInt32(pl.Envelope.X * 10.0000), Convert.ToInt32(pl.Envelope.Y * 10.0000 - pl.Envelope.Height * 10.0000), Convert.ToInt32(pl.Envelope.Width * 10.0000), Convert.ToInt32(pl.Envelope.Height * 10.0000));

                                       Rectangle rect2 = new Rectangle(Convert.ToInt32(iFeature.Envelope.X * 10.0000), Convert.ToInt32(iFeature.Envelope.Y * 10.0000 - iFeature.Envelope.Height * 10.0000), Convert.ToInt32(iFeature.Envelope.Width * 10.0000), Convert.ToInt32(iFeature.Envelope.Height * 10.0000));
                                       Rectangle rect3 = new Rectangle(Convert.ToInt32(pl.Envelope.X * 10.0000), Convert.ToInt32(pl.Envelope.Y * 10.0000 - pl.Envelope.Height * 10.0000), Convert.ToInt32(pl.Envelope.Width * 10.0000), Convert.ToInt32(pl.Envelope.Height * 10.0000));
                                       if (!rect3.IntersectsWith(rect2))
                                       {
                                           continue;
                                       }
                                       //rect3.Intersect(rect2);
                                       //if (!reg.IsVisible(new Point(Convert.ToInt32(iFeature.Envelope.ToExtent().Center.X), Convert.ToInt32(iFeature.Envelope.ToExtent().Center.Y))))
                                       //{

                                       //    continue;

                                       //}
                                       //if (rect3.Width > System.Math.Min(rect2.Width, rect1.Width) * 0.9000 && rect3.Height > System.Math.Min(rect2.Height, rect1.Height) * 0.9000)
                                       //{

                                       try
                                       {
                                           Feature iFeatureCenter = new Feature();
                                           DotSpatial.Topology.Geometry g = new DotSpatial.Topology.Point(iFeature.Envelope.ToExtent().Center.X, iFeature.Envelope.ToExtent().Center.Y);
                                           iFeatureCenter.BasicGeometry = g;// iFeature.Envelope.ToExtent().Center as DotSpatial.Topology.IBasicGeometry;

                                           List<int> lstFidsin = regionFeatureSet.SelectIndices(iFeatureCenter.Envelope.ToExtent());
                                           if (lstFidsin != null && lstFidsin.Count > 0 && lstFidsin.Contains(i))
                                           {
                                               if (gridFeatureSet.ShapeIndices.Count < 5000 && regionFeatureSet.ShapeIndices.Count < 5000)
                                               {
                                                   //IFeature iFeatureIntersection = pl.Intersection(iFeature);

                                                   DotSpatial.Topology.IGeometry geo = pl.BasicGeometry as DotSpatial.Topology.IGeometry;
                                                   DotSpatial.Topology.IGeometry gResult = geo.Intersection(iFeature.BasicGeometry as DotSpatial.Topology.IGeometry);
                                                   if (gResult == null) continue;
                                                   double d = gResult.Area;
                                                   if (d >= Math.Min(iFeature.Area(), pl.Area()) * 0.500000)
                                                   {
                                                       gridRelationshipAttribute.smallGridRowCol.Add(new RowCol()
                                                       {
                                                           Col = Convert.ToInt32(iFeature.DataRow["Col"]),
                                                           Row = Convert.ToInt32(iFeature.DataRow["Row"])
                                                       });

                                                   }


                                               }
                                               else
                                               {
                                                   gridRelationshipAttribute.smallGridRowCol.Add(new RowCol()
                                                      {
                                                          Col = Convert.ToInt32(iFeature.DataRow["Col"]),
                                                          Row = Convert.ToInt32(iFeature.DataRow["Row"])
                                                      });
                                               }

                                           }

                                       }
                                       catch (Exception ex)
                                       {
                                           gridRelationshipAttribute.smallGridRowCol.Add(new RowCol()
                                           {
                                               Col = Convert.ToInt32(iFeature.DataRow["Col"]),
                                               Row = Convert.ToInt32(iFeature.DataRow["Row"])
                                           });
                                       }

                                       //}
                                   }
                                   catch (Exception ex)
                                   {

                                   }



                               }

                               gridRelationship.lstGridRelationshipAttribute.Add(gridRelationshipAttribute);
                               // gridFeatureSet.Close();
                               //  regionFeatureSet.Close();
                               // relationFeatureSet.Close();
                           }
                           catch (Exception ex)
                           {
                               //relationFeatureSet.Close();
                               // gridFeatureSet.Close();
                               // regionFeatureSet.Close();
                           }

                           i++;
                       }
                       #endregion
                   }
                   //--------modify by xiejp -remove gridRelationship  add percentage to database
                   gridRelationship = CommonClass.getRelationshipFromBenMAPGrid(gridRelationship.bigGridID, gridRelationship.smallGridID);
                   lstGridRelationship.Add(gridRelationship);
                  // CommonClass.getRelationshipFromBenMAPGridPercentage(gridRelationship.bigGridID, gridRelationship.smallGridID);



               }
               //return lstGridRelationship;
           }

           catch (Exception ex)
           {

               Logger.LogError(ex);
               //return null;
           }
       }

       /// <summary>
       /// 根据所选的网格，得到所有网格和他的关系
       /// </summary>
       /// <param name="benMAPGrid"></param>
       /// <returns></returns>
       public static void getGridRelationshipFromGridPercentage(BenMAPGrid benMAPGrid)
       {
           try
           {

               string commandText = "select a.GridDefinitionID,SetupID,GridDefinitionName,Columns,RRows,TType,b.ShapeFileName from GridDefinitions a,ShapeFileGridDefinitionDetails b " +
                                    " where a.GridDefinitionID=b.GridDefinitionID and a.TType=1  and a.SetupID=" + benMAPGrid.SetupID +
                                    " union " +
                                    " select a.GridDefinitionID,SetupID,GridDefinitionName,Columns,RRows,TType,b.ShapeFileName from GridDefinitions a,RegularGridDefinitionDetails b " +
                                    " where a.GridDefinitionID=b.GridDefinitionID and a.TType=0  and a.SetupID=" + benMAPGrid.SetupID;
               ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
               System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
               // List<GridRelationship> lstGridRelationship = new List<GridRelationship>();
               //------------remove from griddefinitionpercentage-------------
               commandText = string.Format("delete from GridDefinitionPercentageEntries where PercentageID in ( select PercentageID  from GridDefinitionPercentages where SourceGridDefinitionID={0} or TargetGridDefinitionID={0})", benMAPGrid.GridDefinitionID);
               fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
               commandText = string.Format("delete from GridDefinitionPercentages where SourceGridDefinitionID={0} or TargetGridDefinitionID={0}", benMAPGrid.GridDefinitionID);
               fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
               string AppPath = Application.StartupPath;
               //----------------------------------------xjp-------------仅用于测试！
               AppPath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\";// @"C:\Program Files\BenMAP 4.0\Shapefiles\";
               //if (File.Exists(Application.StartupPath + @"\Data\Shapefiles\" + (CommonClass.RBenMAPGrid as ShapefileGrid).ShapefileName + ".shp"))
               //   {
               string benMAPGridShapeFile = "";
               if (benMAPGrid is ShapefileGrid) benMAPGridShapeFile = (benMAPGrid as ShapefileGrid).ShapefileName;
               if (benMAPGrid is RegularGrid) benMAPGridShapeFile = (benMAPGrid as RegularGrid).ShapefileName;
               benMAPGridShapeFile = AppPath + benMAPGridShapeFile + ".shp";
               DotSpatial.Data.IFeatureSet benMAPGridFeatureSet = DotSpatial.Data.FeatureSet.Open(benMAPGridShapeFile);
               foreach (DataRow dr in ds.Tables[0].Rows)
               {
                  
                   //增加网格和当前网格的对应关系
                   int gridDefinitionID = Convert.ToInt32(dr["GridDefinitionID"]);
                   if (gridDefinitionID == benMAPGrid.GridDefinitionID)
                   {
                       continue;
                   }




                   int bigGridID, smallGridID;



                   DotSpatial.Data.IFeatureSet relationFeatureSet = DotSpatial.Data.FeatureSet.Open(AppPath + dr["ShapeFileName"].ToString() + ".shp");
                   DotSpatial.Data.IFeatureSet regionFeatureSet = null;
                   DotSpatial.Data.IFeatureSet gridFeatureSet = null;
                   if (benMAPGridFeatureSet.ShapeIndices.Count > relationFeatureSet.ShapeIndices.Count)
                   {
                       regionFeatureSet = relationFeatureSet;
                       gridFeatureSet = benMAPGridFeatureSet;

                       bigGridID = gridDefinitionID;
                       smallGridID = benMAPGrid.GridDefinitionID;
                   }
                   else
                   {
                       regionFeatureSet = benMAPGridFeatureSet;
                       gridFeatureSet = relationFeatureSet;
                       bigGridID = benMAPGrid.GridDefinitionID;
                       smallGridID = gridDefinitionID;
                   }
                   //Nation 全部
                  
                   //--------modify by xiejp -remove gridRelationship  add percentage to database

                   CommonClass.getRelationshipFromBenMAPGridPercentage(bigGridID, smallGridID);



               }
               //return lstGridRelationship;
           }

           catch (Exception ex)
           {

               Logger.LogError(ex);
               //return null;
           }
       }
       public static void getGridRelationshipFromFile(string filePath, ref List<GridRelationship> lstGridRelationship)
       {
           FileStream fs = new FileStream(filePath, FileMode.Open);
           // BinaryReader brtest = new BinaryReader(fstest);
           byte[] data = new byte[fs.Length];
           fs.Read(data, 0, data.Length);
           var serializertest = new DataContractJsonSerializer(typeof(List<GridRelationship>));
           var mStream = new MemoryStream(data);
           //BinaryReader br = new BinaryReader(fs);
           //MemoryStream memoryFile = GridCommon.DecryptToMemoryStream(strFile, "&%#@?,:*");
           lstGridRelationship = (List<GridRelationship>)serializertest.ReadObject(mStream);
           mStream.Close();
           fs.Close();

           //using (FileStream fsSave = new FileStream(@"d:\aa.gr", FileMode.OpenOrCreate))
           //{
           //    BinaryFormatter formatter = new BinaryFormatter();
           //    formatter.Serialize(fsSave, lstGridRelationship);
           //    fs.Close();
           //    fs.Dispose();
           //}
           //using (FileStream fsRead = new FileStream(@"d:\aa.gr", FileMode.Open))
           //{
           //    BinaryFormatter formatter = new BinaryFormatter();
           //    lstGridRelationship = (List<GridRelationship>)formatter.Deserialize(fsRead);//在这里大家要注意咯,他的返回值是object
           //    fsRead.Close();
           //    fsRead.Dispose();
           //    GC.Collect();
           //}
       }

       public static void createGridRelationshipFile(string filePath, List<GridRelationship> lstGridRelationship)
       {
           var serializer = new DataContractJsonSerializer(typeof(List<GridRelationship>));
           MemoryStream stream = new MemoryStream();
           serializer.WriteObject(stream, lstGridRelationship);
           FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
           BinaryWriter w = new BinaryWriter(fs);
           w.Write(stream.ToArray());//dataBytes);
           w.Close();
           fs.Close();
       }
    }
}
