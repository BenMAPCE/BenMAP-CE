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
                return ms;
            }
            catch (Exception ex)
            {
                return null;
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
                seasonalMetric.Seasons = new List<Season>();
                commandText = string.Format("select a.PollutantID,c.SeasonalMetricID,d.StartDay,d.EndDay,d.SEASONALMETRICTYPE,d.METRICFUNCTION  from Pollutants a ,Metrics b,SeasonalMetrics c ,SeasonalMetricSeasons d " +
" where a.PollutantID=b.PollutantID and b.MetricID=c.MetricID and c.SeasonalMetricID=d.SeasonalMetricID and c.SeasonalMetricID={0}", SeasonalMetricID);


                ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                foreach (DataRow drSeason in ds.Tables[0].Rows)
                {
                    Season season = new Season()
                    {
                        PollutantID = Convert.ToInt32(drSeason["PollutantID"]),
                        PollutantSeasonID = -1,
                        StartDay = Convert.ToInt32(drSeason["StartDay"]),
                        EndDay = Convert.ToInt32(drSeason["EndDay"]),
                        StartHour = -1,
                        EndHour = -1,
                        Numbins = -1,
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
                string commandText = string.Format("select PollutantName,ObservationType,PollutantID from Pollutants where SetupID={0}", setupID);
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
        public static BenMAPPollutant getPollutantFromName(string PollutantName, int SetupID)
        {
            try
            {
                BenMAPPollutant benMAPPollutant = new BenMAPPollutant();
                ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
                string commandText = string.Format("select PollutantID,PollutantName,ObservationType from Pollutants where PollutantName ='{0}' and SetupID={1}", PollutantName, SetupID);
                System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                DataRow dr = ds.Tables[0].Rows[0];
                benMAPPollutant.PollutantID = Convert.ToInt32(dr["PollutantID"].ToString());
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
        public static BenMAPGrid getBenMAPGridFromID(int GridID)
        {

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["ConnectionString"];
            //string str = settings.ConnectionString;
            //if (!str.Contains(":"))
            //    str = str.Substring(0, str.IndexOf("initial catalog=")) + "initial catalog=" + Application.StartupPath + @"\" + str.Substring(str.IndexOf("initial catalog=") + 16);
            //str = str.Replace("##USERDATA##", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            //FbConnection _connection = new FirebirdSql.Data.FirebirdClient.FbConnection(str);
            FbConnection _connection = CommonClass.getNewConnection();
            try
            {
                BenMAPGrid benMAPGrid = new BenMAPGrid();
                BenMAPGrid toReturn = null;
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
                        toReturn= shapefileGrid;
                        break;
                    case GridTypeEnum.Regular:
                        RegularGrid regularGrid = new RegularGrid()
                        {
                            GridDefinitionID = GridID,
                            GridDefinitionName = benMAPGrid.GridDefinitionName,
                            Columns = benMAPGrid.Columns,
                            RRows = benMAPGrid.RRows,
                            SetupID = benMAPGrid.SetupID,
                            TType = benMAPGrid.TType,
                            SetupName = CommonClass.MainSetup.SetupName
                        };
                        commandText = string.Format("select ShapeFileName,GridDefinitionID,MinimumLatitude,MinimumLongitude,ColumnsperLongitude,RowsperLatitude from RegularGridDefinitionDetails where GridDefinitionID={0}", GridID);
                        ds = fb.ExecuteDataset(_connection, CommandType.Text, commandText);
                        dr = ds.Tables[0].Rows[0];
                        regularGrid.MinimumLatitude = Convert.ToDouble(dr["MinimumLatitude"]);
                        regularGrid.MinimumLongitude = Convert.ToDouble(dr["MinimumLongitude"]);
                        regularGrid.ColumnsperLongitude = Convert.ToInt32(dr["ColumnsperLongitude"]);
                        regularGrid.RowsperLatitude = Convert.ToInt32(dr["RowsperLatitude"]);
                        regularGrid.ShapefileName = dr["ShapeFileName"].ToString();
                        toReturn= regularGrid;

                        break;
                }
                _connection.Close();
                if (toReturn == null)
                {
                    toReturn = benMAPGrid;
                }
                return toReturn;

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
                getGridRelationshipFromGrid(benMAPGrid, ref lstGridRelationship);
            }
            return lstGridRelationship;

        }
        public static Region getRegionFromBaseGeometry(DotSpatial.Topology.IBasicGeometry iBaseGeometry)
        {
            Region region;

            System.Drawing.Drawing2D.GraphicsPath gPath = new System.Drawing.Drawing2D.GraphicsPath();
            int i = 0;
            PointF[] pointF = new PointF[iBaseGeometry.Coordinates.Count];
            while (i < iBaseGeometry.Coordinates.Count - 1)
            {
                pointF[i] = new PointF(Convert.ToSingle(iBaseGeometry.Coordinates[i].X), Convert.ToSingle(iBaseGeometry.Coordinates[i].Y));
                i++;
            }
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
            ESIL.DBUtility.FireBirdHelperBase fb = new ESIL.DBUtility.ESILFireBirdHelper();
            System.Data.DataSet ds = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);

            string AppPath = Application.StartupPath;
            AppPath = @"D:\软件项目\Map\正确的BenMAP的SHP\";
            string benMAPGridShapeFile = "";


            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                benMAPGridShapeFile = AppPath + dr["ShapeFileName"].ToString() + ".shp";
                DotSpatial.Data.IFeatureSet benMAPGridFeatureSet = DotSpatial.Data.FeatureSet.Open(benMAPGridShapeFile);
                string GridDefinitionName = dr["GridDefinitionName"].ToString();

                foreach (IFeature feature in benMAPGridFeatureSet.Features)
                {
                    string LocationName = "";
                    if (GridDefinitionName.ToLower() == "state")
                    {
                        LocationName = feature.DataRow["State_Name"].ToString();
                    }
                    else
                    {
                        LocationName = feature.DataRow["Name"].ToString();

                    }
                    LocationName = LocationName.Replace("'", "''");

                    int Col = Convert.ToInt32(feature.DataRow["Col"]);
                    int Row = Convert.ToInt32(feature.DataRow["Row"]);
                    string Fips = feature.DataRow["Fips"].ToString();
                    commandText = string.Format("insert into USAFIPSCODES values('{0}','{1}',{2},{3},'{4}')", GridDefinitionName, LocationName, Col, Row, Fips);
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
        public static void getGridRelationshipFromGrid(BenMAPGrid benMAPGrid, ref List<GridRelationship> lstGridRelationship)
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
                if (lstGridRelationship == null)
                    lstGridRelationship = new List<GridRelationship>();
                string AppPath = Application.StartupPath;
                AppPath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\"; string benMAPGridShapeFile = "";
                if (benMAPGrid is ShapefileGrid) benMAPGridShapeFile = (benMAPGrid as ShapefileGrid).ShapefileName;
                if (benMAPGrid is RegularGrid) benMAPGridShapeFile = (benMAPGrid as RegularGrid).ShapefileName;
                benMAPGridShapeFile = AppPath + benMAPGridShapeFile + ".shp";
                DotSpatial.Data.IFeatureSet benMAPGridFeatureSet = DotSpatial.Data.FeatureSet.Open(benMAPGridShapeFile);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    GridRelationship gridRelationship = new GridRelationship();
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



                    DotSpatial.Data.IFeatureSet relationFeatureSet = DotSpatial.Data.FeatureSet.Open(AppPath + dr["ShapeFileName"].ToString() + ".shp");
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
                        regionFeatureSet = benMAPGridFeatureSet;
                        gridFeatureSet = relationFeatureSet;
                        gridRelationship.bigGridID = benMAPGrid.GridDefinitionID;
                        gridRelationship.smallGridID = gridDefinitionID;
                    }
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
                        while (iNationIn < gridFeatureSet.ShapeIndices.Count)
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
                    if (gridRelationship.bigGridID == 2 && gridRelationship.smallGridID == 1)
                    {
                        commandText = "select Col,Row,Fipscode from USAFipsCodes where LocationTypeName='State'";
                        System.Data.DataSet dsState = fb.ExecuteDataset(CommonClass.Connection, CommandType.Text, commandText);
                        foreach (DataRow drState in dsState.Tables[0].Rows)
                        {
                            GridRelationshipAttribute gridRelationshipAttribute = new GridRelationshipAttribute();
                            gridRelationshipAttribute.bigGridRowCol = new RowCol()
                            {
                                Col = Convert.ToInt32(drState["Col"]),
                                Row = Convert.ToInt32(drState["Row"])
                            };
                            gridRelationshipAttribute.smallGridRowCol = new List<RowCol>();
                            commandText = string.Format("select Col,Row,Fipscode from USAFipsCodes where LocationTypeName='County' and SUBSTRING(fipscode from 1 for 2) ={0}", drState["Fipscode"].ToString());
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
                    if (gridRelationship.bigGridID == 8 && gridRelationship.smallGridID == 2)
                    {
                        GridRelationshipAttribute gridRelationshipAttribute = new GridRelationshipAttribute();
                        gridRelationshipAttribute.bigGridRowCol = new RowCol()
                        {
                            Col = 1,
                            Row = 1
                        };
                        gridRelationshipAttribute.smallGridRowCol = new List<RowCol>();
                        gridRelationshipAttribute.smallGridRowCol.Add(new RowCol() { Col = 6, Row = 1 });
                        gridRelationship.lstGridRelationshipAttribute.Add(gridRelationshipAttribute);

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
                        gridRelationshipAttribute = new GridRelationshipAttribute();
                        gridRelationshipAttribute.bigGridRowCol = new RowCol()
                        {
                            Col = 1,
                            Row = 2
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
                        int i = 0;
                        while (i < regionFeatureSet.ShapeIndices.Count)
                        {
                            try
                            {
                                DotSpatial.Data.IFeature pl = regionFeatureSet.GetFeature(i);


                                Rectangle rect = new Rectangle(Convert.ToInt32(pl.Envelope.X), Convert.ToInt32(pl.Envelope.Y - pl.Envelope.Height), Convert.ToInt32(pl.Envelope.Width), Convert.ToInt32(pl.Envelope.Height));
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



                                        try
                                        {
                                            Feature iFeatureCenter = new Feature();
                                            DotSpatial.Topology.Geometry g = new DotSpatial.Topology.Point(iFeature.Envelope.ToExtent().Center.X, iFeature.Envelope.ToExtent().Center.Y);
                                            iFeatureCenter.BasicGeometry = g;
                                            List<int> lstFidsin = regionFeatureSet.SelectIndices(iFeatureCenter.Envelope.ToExtent());
                                            if (lstFidsin != null && lstFidsin.Count > 0 && lstFidsin.Contains(i))
                                            {
                                                if (gridFeatureSet.ShapeIndices.Count < 5000 && regionFeatureSet.ShapeIndices.Count < 5000)
                                                {

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

                                    }
                                    catch (Exception ex)
                                    {

                                    }



                                }

                                gridRelationship.lstGridRelationshipAttribute.Add(gridRelationshipAttribute);
                            }
                            catch (Exception ex)
                            {
                            }

                            i++;
                        }
                    }
                    gridRelationship = CommonClass.getRelationshipFromBenMAPGrid(gridRelationship.bigGridID, gridRelationship.smallGridID);
                    lstGridRelationship.Add(gridRelationship);



                }
            }

            catch (Exception ex)
            {

                Logger.LogError(ex);
            }
        }

        public static void getGridRelationshipFromGridPercentage(BenMAPGrid benMAPGrid, String popGridRasterLoc)
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
                commandText = string.Format("delete from GridDefinitionPercentageEntries where PercentageID in ( select PercentageID  from GridDefinitionPercentages where SourceGridDefinitionID={0} or TargetGridDefinitionID={0})", benMAPGrid.GridDefinitionID);
                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                commandText = string.Format("delete from GridDefinitionPercentages where SourceGridDefinitionID={0} or TargetGridDefinitionID={0}", benMAPGrid.GridDefinitionID);
                fb.ExecuteNonQuery(CommonClass.Connection, CommandType.Text, commandText);
                string AppPath = Application.StartupPath;
                AppPath = CommonClass.DataFilePath + @"\Data\Shapefiles\" + CommonClass.MainSetup.SetupName + "\\"; string benMAPGridShapeFile = "";
                if (benMAPGrid is ShapefileGrid) benMAPGridShapeFile = (benMAPGrid as ShapefileGrid).ShapefileName;
                if (benMAPGrid is RegularGrid) benMAPGridShapeFile = (benMAPGrid as RegularGrid).ShapefileName;
                benMAPGridShapeFile = AppPath + benMAPGridShapeFile + ".shp";
                DotSpatial.Data.IFeatureSet benMAPGridFeatureSet = DotSpatial.Data.FeatureSet.Open(benMAPGridShapeFile);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {

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


                    CommonClass.getRelationshipFromBenMAPGridPercentage(bigGridID, smallGridID, popGridRasterLoc);



                }
            }

            catch (Exception ex)
            {

                Logger.LogError(ex);
            }
        }
        public static void getGridRelationshipFromFile(string filePath, ref List<GridRelationship> lstGridRelationship)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            var serializertest = new DataContractJsonSerializer(typeof(List<GridRelationship>));
            var mStream = new MemoryStream(data);
            lstGridRelationship = (List<GridRelationship>)serializertest.ReadObject(mStream);
            mStream.Close();
            fs.Close();

        }

        public static void createGridRelationshipFile(string filePath, List<GridRelationship> lstGridRelationship)
        {
            var serializer = new DataContractJsonSerializer(typeof(List<GridRelationship>));
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, lstGridRelationship);
            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
            BinaryWriter w = new BinaryWriter(fs);
            w.Write(stream.ToArray()); w.Close();
            fs.Close();
        }
    }
}
